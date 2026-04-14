"""
generate_test_images.py
=======================
Generates two 255x255 GeoTIFF test images for colour_shade_fusion.py:

1. colour_wheel.tif  – RGB colour wheel
     - Hue    varies with angle (0-360°)
     - Saturation varies with distance from centre (0 at centre → 1 at edge)
     - Value  fixed at 100 (full brightness)
     - Pixels outside the circle are white (255,255,255)

2. shade_bands.tif   – greyscale image with sinusoidal vertical bands defined
   by the control points: (0,100), (64,25), (128,100), (192,25), (255,100)
   Values are interpolated linearly between each pair of control points and
   written as uint8 (0-255 scaled from 0-100).

Both files carry a trivial geotransform (1 m pixels, origin at 0,0) so they
are valid GeoTIFFs that GDAL tools can open without complaint.
"""

import numpy as np
from osgeo import gdal, osr

SIZE = 255          # image width and height in pixels
CX   = SIZE // 2   # centre column
CY   = SIZE // 2   # centre row
R    = SIZE // 2   # radius of the colour wheel


# ---------------------------------------------------------------------------
# Helper: write a 3-band uint8 GeoTIFF
# ---------------------------------------------------------------------------
def write_rgb_tiff(path: str, r: np.ndarray, g: np.ndarray, b: np.ndarray):
    driver = gdal.GetDriverByName("GTiff")
    ds = driver.Create(path, SIZE, SIZE, 3, gdal.GDT_Byte,
                       ["COMPRESS=LZW", "PHOTOMETRIC=RGB"])
    # Trivial 1 m pixel geotransform, WGS-84
    ds.SetGeoTransform([0.0, 1.0, 0.0, float(SIZE), 0.0, -1.0])
    srs = osr.SpatialReference()
    srs.ImportFromEPSG(4326)
    ds.SetProjection(srs.ExportToWkt())
    ds.GetRasterBand(1).WriteArray(r)
    ds.GetRasterBand(2).WriteArray(g)
    ds.GetRasterBand(3).WriteArray(b)
    ds.GetRasterBand(1).SetColorInterpretation(gdal.GCI_RedBand)
    ds.GetRasterBand(2).SetColorInterpretation(gdal.GCI_GreenBand)
    ds.GetRasterBand(3).SetColorInterpretation(gdal.GCI_BlueBand)
    ds.FlushCache()
    ds = None
    print(f"  Written: {path}")


# ---------------------------------------------------------------------------
# Helper: write a 1-band uint8 GeoTIFF
# ---------------------------------------------------------------------------
def write_grey_tiff(path: str, grey: np.ndarray):
    driver = gdal.GetDriverByName("GTiff")
    ds = driver.Create(path, SIZE, SIZE, 1, gdal.GDT_Byte, ["COMPRESS=LZW"])
    ds.SetGeoTransform([0.0, 1.0, 0.0, float(SIZE), 0.0, -1.0])
    srs = osr.SpatialReference()
    srs.ImportFromEPSG(4326)
    ds.SetProjection(srs.ExportToWkt())
    ds.GetRasterBand(1).WriteArray(grey)
    ds.GetRasterBand(1).SetColorInterpretation(gdal.GCI_GrayIndex)
    ds.FlushCache()
    ds = None
    print(f"  Written: {path}")


# ---------------------------------------------------------------------------
# HSV → RGB  (vectorised)
# ---------------------------------------------------------------------------
def hsv_to_rgb(h: np.ndarray, s: np.ndarray, v: np.ndarray):
    """
    h : [0, 360)   s : [0, 1]   v : [0, 1]
    Returns r, g, b as uint8 arrays.
    """
    c   = v * s
    x   = c * (1.0 - np.abs((h / 60.0) % 2 - 1))
    m   = v - c
    sec = (h / 60.0).astype(int) % 6

    r1 = np.zeros_like(v); g1 = np.zeros_like(v); b1 = np.zeros_like(v)
    for idx, (rv, gv, bv) in enumerate([(c,x,0),(x,c,0),(0,c,x),
                                         (0,x,c),(x,0,c),(c,0,x)]):
        mask = sec == idx
        r1[mask] = rv[mask] if isinstance(rv, np.ndarray) else rv
        g1[mask] = gv[mask] if isinstance(gv, np.ndarray) else gv
        b1[mask] = bv[mask] if isinstance(bv, np.ndarray) else bv

    r = np.clip((r1 + m) * 255, 0, 255).astype(np.uint8)
    g = np.clip((g1 + m) * 255, 0, 255).astype(np.uint8)
    b = np.clip((b1 + m) * 255, 0, 255).astype(np.uint8)
    return r, g, b


# ---------------------------------------------------------------------------
# 1. Colour wheel
# ---------------------------------------------------------------------------
def make_colour_wheel():
    print("Generating colour_wheel.tif …")

    # Pixel coordinate grids
    cols = np.arange(SIZE, dtype=np.float32)
    rows = np.arange(SIZE, dtype=np.float32)
    xg, yg = np.meshgrid(cols - CX, rows - CY)   # yg: positive = down

    dist = np.sqrt(xg**2 + yg**2)                # distance from centre
    sat  = np.clip(dist / R, 0.0, 1.0)           # saturation: 0 → 1

    # Angle: 0° = East, increases counter-clockwise (standard math convention)
    # We flip y so that red (0°) is at the right and hue increases CCW.
    hue  = np.degrees(np.arctan2(-yg, xg)) % 360.0   # [0, 360)

    inside = dist <= R
    val    = np.ones_like(dist)                   # value = 1 everywhere

    r = np.full((SIZE, SIZE), 255, dtype=np.uint8)
    g = np.full((SIZE, SIZE), 255, dtype=np.uint8)
    b = np.full((SIZE, SIZE), 255, dtype=np.uint8)

    ri, gi, bi = hsv_to_rgb(hue[inside], sat[inside], val[inside])
    r[inside] = ri
    g[inside] = gi
    b[inside] = bi

    write_rgb_tiff("colour_wheel.tif", r, g, b)


# ---------------------------------------------------------------------------
# 2. Shade bands
# ---------------------------------------------------------------------------
def make_shade_bands():
    """
    Control points (x, value_0_to_100):
        (0, 100), (64, 25), (128, 100), (192, 25), (255, 100)

    Linear interpolation between each pair; value scaled to uint8 (×2.55).
    The shade is constant in the y direction (vertical bands).
    """
    print("Generating shade_bands.tif …")

    control_pts = [(0, 100), (64, 25), (128, 100), (192, 25), (255, 100)]

    # Use SIZE+1 = 256 sample points so control point x=255 maps cleanly,
    # then trim to the first SIZE (255) columns that match the image width.
    shade_full = np.zeros(256, dtype=np.float64)

    for i in range(len(control_pts) - 1):
        x0, v0 = control_pts[i]
        x1, v1 = control_pts[i + 1]
        for x in range(x0, x1 + 1):
            t = (x - x0) / (x1 - x0) if x1 != x0 else 0.0
            shade_full[x] = v0 + t * (v1 - v0)

    shade_1d = shade_full[:SIZE]

    # Scale 0-100 → 0-255
    shade_1d_u8 = np.clip(shade_1d * 2.55, 0, 255).astype(np.uint8)

    # Broadcast column values across all rows
    shade_2d = np.tile(shade_1d_u8, (SIZE, 1))   # shape (255, 255)

    write_grey_tiff("shade_bands.tif", shade_2d)


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------
if __name__ == "__main__":
    make_colour_wheel()
    make_shade_bands()
    print("\nBoth test images generated successfully.")
    print("Run the fusion with, e.g.:")
    print("  python colour_shade_fusion.py \\")
    print("      --colour colour_wheel.tif \\")
    print("      --shade  shade_bands.tif  \\")
    print("      --output fused_output.tif \\")
    print("      --vmin 0.05 --vexp 1.0 --smin 0.4 --sexp 1.0 --cutoff 200")
