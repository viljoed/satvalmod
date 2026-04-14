"""
sat_val_mod.py
======================
Fuses a colour (RGB) raster with a greyscale shade/hillshade raster by
modulating the HSV saturation and value channels of the colour image using
lookup tables derived from the shade pixel values.

Replaces the original VB.NET tool suite (clsColorTransformer, clsSatValMod,
clsBILFileWriter, clsFileUtils, clsStringUtils) and swaps the ESRI data-access
layer for GDAL.  Outputs GeoTIFF or JPEG.

Dependencies
------------
    pip install gdal numpy          # GDAL Python bindings + NumPy
    # On Conda:  conda install -c conda-forge gdal numpy

Usage (command line)
---------------------
    python sat_val_mod.py \
        --colour  colour_image.tif  \
        --shade   hillshade.tif     \
        --output  fused_output.tif  \
        [--format tif|jpg]          \
        [--vmin 0.0] [--vexp 1.0]   \
        [--smin 0.5] [--sexp 1.0]   \
        [--cutoff 200]              \
        [--shade-max 255]           \
        [--dump-luts]

Parameter notes
---------------
    --vmin      Minimum value (brightness) modulation factor   [0.0 – 1.0, default 0.0]
    --vexp      Value modulation curve exponent                [>0,        default 1.0]
    --smin      Minimum saturation modulation factor           [0.0 – 1.0, default 0.5]
    --sexp      Saturation modulation curve exponent           [>0,        default 1.0]
    --cutoff    Shade pixel value below which sat stays at 1   [0 – shade-max, default 200]
    --shade-max Maximum shade pixel value (usually 255 or 100) [default 255]
    --dump-luts Write the value/saturation LUTs to CSV files for inspection

Algorithm (mirrors clsSatValMod / clsColorTransformer logic)
-------------------------------------------------------------
For every pixel:
  1. Convert colour (R,G,B) → (H,S,V)  [H: 0-360, S: 0-100, V: 0-100]
  2. Look up value modulation:      vm = Vmin + (1-Vmin) * (shade/cutoff)^Vexp
                                    (vm = 1.0 when shade > cutoff)
  3. Look up saturation modulation: sm = 1.0 when shade <= cutoff
                                    sm = 1 - (1-Smin) * ((shade-cutoff)/(ShadeMax-cutoff))^Sexp
  4. Modulate:  S' = S * sm,  V' = V * vm
  5. Convert back (H, S', V') → (R', G', B')
  6. Write pixel to output raster preserving georeference.
"""

import argparse
import os
import sys
import csv

import numpy as np

try:
    from osgeo import gdal, osr
except ImportError:
    import gdal
    import osr

gdal.UseExceptions()


# ---------------------------------------------------------------------------
# Colour-space conversion  (clsColorTransformer)
# ---------------------------------------------------------------------------

def rgb_to_hsv(r: np.ndarray, g: np.ndarray, b: np.ndarray):
    """
    Vectorised RGB → HSV conversion.

    Input arrays: uint8 or float in [0, 255].
    Returns:
        h  float32  [0, 360)
        s  float32  [0, 100]
        v  float32  [0, 100]
    """
    rf = r.astype(np.float32) / 255.0
    gf = g.astype(np.float32) / 255.0
    bf = b.astype(np.float32) / 255.0

    cmax = np.maximum(np.maximum(rf, gf), bf)
    cmin = np.minimum(np.minimum(rf, gf), bf)
    delta = cmax - cmin

    # Value
    v = cmax * 100.0

    # Saturation
    s = np.where(cmax > 0, (delta / cmax) * 100.0, 0.0).astype(np.float32)

    # Hue
    h = np.zeros_like(rf)
    mask_r = (cmax == rf) & (delta > 0)
    mask_g = (cmax == gf) & (delta > 0)
    mask_b = (cmax == bf) & (delta > 0)

    h[mask_r] = 60.0 * (((gf[mask_r] - bf[mask_r]) / delta[mask_r]) % 6)
    h[mask_g] = 60.0 * (((bf[mask_g] - rf[mask_g]) / delta[mask_g]) + 2)
    h[mask_b] = 60.0 * (((rf[mask_b] - gf[mask_b]) / delta[mask_b]) + 4)
    h = h % 360.0

    return h.astype(np.float32), s.astype(np.float32), v.astype(np.float32)


def hsv_to_rgb(h: np.ndarray, s: np.ndarray, v: np.ndarray):
    """
    Vectorised HSV → RGB conversion.

    Input:
        h  float  [0, 360)
        s  float  [0, 100]
        v  float  [0, 100]
    Returns:
        r, g, b  uint8 arrays in [0, 255]
    """
    s_f = s / 100.0
    v_f = v / 100.0

    c = v_f * s_f                       # chroma
    x = c * (1.0 - np.abs((h / 60.0) % 2 - 1))
    m = v_f - c

    h_sec = (h / 60.0).astype(np.int32) % 6

    r1 = np.zeros_like(v_f)
    g1 = np.zeros_like(v_f)
    b1 = np.zeros_like(v_f)

    for sec, (rv, gv, bv) in enumerate([
        (c, x, 0), (x, c, 0), (0, c, x),
        (0, x, c), (x, 0, c), (c, 0, x),
    ]):
        mask = h_sec == sec
        r1[mask] = rv[mask] if isinstance(rv, np.ndarray) else rv
        g1[mask] = gv[mask] if isinstance(gv, np.ndarray) else gv
        b1[mask] = bv[mask] if isinstance(bv, np.ndarray) else bv

    r = np.clip((r1 + m) * 255.0, 0, 255).astype(np.uint8)
    g = np.clip((g1 + m) * 255.0, 0, 255).astype(np.uint8)
    b = np.clip((b1 + m) * 255.0, 0, 255).astype(np.uint8)

    return r, g, b


# ---------------------------------------------------------------------------
# Modulation LUT builders  (clsSatValMod)
# ---------------------------------------------------------------------------

def build_value_lut(vmin: float, vexp: float, cutoff: int,
                    shade_max: int) -> np.ndarray:
    """
    Build the Value modulation lookup table (indices 0 … shade_max).

    For shade <= cutoff:  vm = vmin + (1 - vmin) * (shade / cutoff) ^ vexp
    For shade >  cutoff:  vm = 1.0
    """
    lut = np.ones(shade_max + 1, dtype=np.float64)
    for i in range(min(cutoff, shade_max) + 1):
        lut[i] = vmin + (1.0 - vmin) * ((i / cutoff) ** vexp) if cutoff > 0 else 1.0
    return lut


def build_saturation_lut(smin: float, sexp: float, cutoff: int,
                          shade_max: int) -> np.ndarray:
    """
    Build the Saturation modulation lookup table (indices 0 … shade_max).

    For shade <= cutoff:  sm = 1.0
    For shade >  cutoff:  sm = 1 - (1 - smin) * ((shade - cutoff) / (shade_max - cutoff)) ^ sexp
    """
    lut = np.ones(shade_max + 1, dtype=np.float64)
    denom = shade_max - cutoff
    for i in range(cutoff + 1, shade_max + 1):
        if denom > 0:
            lut[i] = 1.0 - (1.0 - smin) * ((i - cutoff) / denom) ** sexp
        else:
            lut[i] = smin
    return lut


def dump_lut_to_csv(lut: np.ndarray, filepath: str):
    """Write a LUT array to a two-column CSV file (index, value)."""
    with open(filepath, "w", newline="") as fh:
        writer = csv.writer(fh)
        writer.writerow(["shade_index", "modulation_factor"])
        for i, val in enumerate(lut):
            writer.writerow([i, f"{val:.6f}"])
    print(f"  LUT written → {filepath}")


# ---------------------------------------------------------------------------
# GDAL helpers
# ---------------------------------------------------------------------------

def open_raster(path: str) -> gdal.Dataset:
    ds = gdal.Open(path, gdal.GA_ReadOnly)
    if ds is None:
        raise FileNotFoundError(f"Cannot open raster: {path}")
    return ds


def read_band_as_array(ds: gdal.Dataset, band_index: int = 1) -> np.ndarray:
    """Read a single band (1-based index) as a 2-D NumPy array."""
    band = ds.GetRasterBand(band_index)
    return band.ReadAsArray()


def create_output_raster(out_path: str, fmt: str,
                         cols: int, rows: int,
                         geotransform, projection: str) -> gdal.Dataset:
    """Create a 3-band (RGB) output dataset."""
    driver_name = "GTiff" if fmt.lower() in ("tif", "tiff", "geotiff") else "JPEG"

    driver = gdal.GetDriverByName(driver_name)
    if driver is None:
        raise RuntimeError(f"GDAL driver '{driver_name}' not available.")

    options = []
    if driver_name == "GTiff":
        options = ["COMPRESS=LZW", "TILED=YES", "PHOTOMETRIC=RGB"]

    out_ds = driver.Create(out_path, cols, rows, 3, gdal.GDT_Byte, options)
    if out_ds is None:
        raise RuntimeError(f"Cannot create output file: {out_path}")

    out_ds.SetGeoTransform(geotransform)
    out_ds.SetProjection(projection)
    return out_ds


# ---------------------------------------------------------------------------
# Core fusion routine
# ---------------------------------------------------------------------------

def fuse_colour_with_shade(
    colour_path: str,
    shade_path: str,
    output_path: str,
    output_format: str = "tif",
    vmin: float = 0.0,
    vexp: float = 1.0,
    smin: float = 0.5,
    sexp: float = 1.0,
    cutoff: int = 200,
    shade_max: int = 255,
    dump_luts: bool = False,
):
    """
    Main fusion function.

    Parameters
    ----------
    colour_path   : Path to the input colour (RGB) raster.
    shade_path    : Path to the greyscale shade/hillshade raster.
    output_path   : Path for the output raster.
    output_format : 'tif' (default) or 'jpg'.
    vmin          : Minimum value modulation factor (0.0–1.0).
    vexp          : Value curve exponent (>0).
    smin          : Minimum saturation modulation factor (0.0–1.0).
    sexp          : Saturation curve exponent (>0).
    cutoff        : Shade value threshold; value is modulated below, sat above.
    shade_max     : Maximum shade raster value (usually 255).
    dump_luts     : If True, write LUT CSVs alongside the output file.
    """

    # ------------------------------------------------------------------
    # 1. Open inputs
    # ------------------------------------------------------------------
    print(f"Opening colour raster : {colour_path}")
    colour_ds = open_raster(colour_path)

    print(f"Opening shade raster  : {shade_path}")
    shade_ds = open_raster(shade_path)

    c_cols, c_rows = colour_ds.RasterXSize, colour_ds.RasterYSize
    s_cols, s_rows = shade_ds.RasterXSize, shade_ds.RasterYSize

    if colour_ds.RasterCount < 3:
        raise ValueError(
            f"Colour raster must have at least 3 bands (R, G, B). "
            f"Found {colour_ds.RasterCount}."
        )

    # ------------------------------------------------------------------
    # 2. Build and optionally dump LUTs
    # ------------------------------------------------------------------
    print("Building modulation LUTs …")
    val_lut = build_value_lut(vmin, vexp, cutoff, shade_max)
    sat_lut = build_saturation_lut(smin, sexp, cutoff, shade_max)

    if dump_luts:
        base = os.path.splitext(output_path)[0]
        dump_lut_to_csv(val_lut, base + "_value_lut.csv")
        dump_lut_to_csv(sat_lut, base + "_saturation_lut.csv")

    # ------------------------------------------------------------------
    # 3. Read raster data
    # ------------------------------------------------------------------
    print("Reading colour bands …")
    r_band = read_band_as_array(colour_ds, 1).astype(np.uint8)
    g_band = read_band_as_array(colour_ds, 2).astype(np.uint8)
    b_band = read_band_as_array(colour_ds, 3).astype(np.uint8)

    print("Reading shade band …")
    shade_raw = read_band_as_array(shade_ds, 1)

    # ------------------------------------------------------------------
    # 4. Align extents if rasters differ in size
    #    (crop to the overlap; reproject is left to the user)
    # ------------------------------------------------------------------
    if (c_cols, c_rows) != (s_cols, s_rows):
        print(
            f"  WARNING: Colour ({c_cols}×{c_rows}) and shade ({s_cols}×{s_rows}) "
            "differ in size.\n  Cropping to smaller common extent."
        )
        cols = min(c_cols, s_cols)
        rows = min(c_rows, s_rows)
        r_band = r_band[:rows, :cols]
        g_band = g_band[:rows, :cols]
        b_band = b_band[:rows, :cols]
        shade_raw = shade_raw[:rows, :cols]
    else:
        cols, rows = c_cols, c_rows

    # Clamp shade values to [0, shade_max]
    shade = np.clip(shade_raw.astype(np.int32), 0, shade_max)

    # ------------------------------------------------------------------
    # 5. RGB → HSV
    # ------------------------------------------------------------------
    print("Converting RGB → HSV …")
    h, s, v = rgb_to_hsv(r_band, g_band, b_band)

    # ------------------------------------------------------------------
    # 6. Apply modulation via LUT lookup
    # ------------------------------------------------------------------
    print("Applying saturation / value modulation …")
    vm = val_lut[shade].astype(np.float32)   # shape (rows, cols)
    sm = sat_lut[shade].astype(np.float32)

    s_mod = np.clip(s * sm, 0.0, 100.0).astype(np.float32)
    v_mod = np.clip(v * vm, 0.0, 100.0).astype(np.float32)

    # ------------------------------------------------------------------
    # 7. HSV → RGB
    # ------------------------------------------------------------------
    print("Converting HSV → RGB …")
    r_out, g_out, b_out = hsv_to_rgb(h, s_mod, v_mod)

    # ------------------------------------------------------------------
    # 8. Write output
    # ------------------------------------------------------------------
    print(f"Writing output → {output_path}")
    geotransform = colour_ds.GetGeoTransform()
    projection = colour_ds.GetProjection()

    out_ds = create_output_raster(
        output_path, output_format, cols, rows, geotransform, projection
    )
    out_ds.GetRasterBand(1).WriteArray(r_out)
    out_ds.GetRasterBand(2).WriteArray(g_out)
    out_ds.GetRasterBand(3).WriteArray(b_out)

    # Set colour interpretation metadata
    out_ds.GetRasterBand(1).SetColorInterpretation(gdal.GCI_RedBand)
    out_ds.GetRasterBand(2).SetColorInterpretation(gdal.GCI_GreenBand)
    out_ds.GetRasterBand(3).SetColorInterpretation(gdal.GCI_BlueBand)

    out_ds.FlushCache()
    out_ds = None
    colour_ds = None
    shade_ds = None

    print("Done.")


# ---------------------------------------------------------------------------
# Command-line interface
# ---------------------------------------------------------------------------

def _positive_float(value):
    fval = float(value)
    if fval <= 0:
        raise argparse.ArgumentTypeError("Value must be > 0")
    return fval


def _unit_float(value):
    fval = float(value)
    if not (0.0 <= fval <= 1.0):
        raise argparse.ArgumentTypeError("Value must be in [0.0, 1.0]")
    return fval


def main():
    parser = argparse.ArgumentParser(
        description=(
            "Fuse a colour raster with a greyscale shade/hillshade raster "
            "by modulating HSV saturation and value channels."
        ),
        formatter_class=argparse.ArgumentDefaultsHelpFormatter,
    )

    # Required
    parser.add_argument("--colour", required=True, metavar="PATH",
                        help="Input colour (RGB) raster (any GDAL-supported format).")
    parser.add_argument("--shade", required=True, metavar="PATH",
                        help="Input greyscale shade/hillshade raster.")
    parser.add_argument("--output", required=True, metavar="PATH",
                        help="Output raster path.")

    # Optional
    parser.add_argument("--format", choices=["tif", "jpg"], default="tif",
                        help="Output file format.")
    parser.add_argument("--vmin", type=_unit_float, default=0.3,
                        help="Minimum value (brightness) modulation factor.")
    parser.add_argument("--vexp", type=_positive_float, default=1.0,
                        help="Value modulation curve exponent.")
    parser.add_argument("--smin", type=_unit_float, default=0.3,
                        help="Minimum saturation modulation factor.")
    parser.add_argument("--sexp", type=_positive_float, default=1.0,
                        help="Saturation modulation curve exponent.")
    parser.add_argument("--cutoff", type=int, default=180,
                        help="Shade value threshold (0 → shade-max).")
    parser.add_argument("--shade-max", type=int, default=255,
                        help="Maximum shade raster value (usually 255 or 100).")
    parser.add_argument("--dump-luts", action="store_true",
                        help="Write saturation and value LUTs to CSV files.")

    args = parser.parse_args()

    if args.cutoff < 0 or args.cutoff > args.shade_max:
        parser.error(f"--cutoff must be in [0, {args.shade_max}]")

    fuse_colour_with_shade(
        colour_path=args.colour,
        shade_path=args.shade,
        output_path=args.output,
        output_format=args.format,
        vmin=args.vmin,
        vexp=args.vexp,
        smin=args.smin,
        sexp=args.sexp,
        cutoff=args.cutoff,
        shade_max=args.shade_max,
        dump_luts=args.dump_luts,
    )


if __name__ == "__main__":
    main()
