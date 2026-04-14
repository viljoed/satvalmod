# SVM/SatValMod/Saturation-Value-Modulation image fusion method

## Setup
uv venv .venv --python 3.12
.venv\Scripts\activate
uv pip install "https://github.com/cgohlke/geospatial-wheels/releases/download/v2024.9.22/GDAL-3.9.2-cp312-cp312-win_amd64.whl"
uv pip install "numpy>=1.24,<2.0" Pillow>=10.0

## Usage
usage: sat_val_mod.py [-h] --colour PATH --shade PATH --output PATH [--format {tif,jpg}] [--vmin VMIN] [--vexp VEXP]
                      [--smin SMIN] [--sexp SEXP] [--cutoff CUTOFF] [--shade-max SHADE_MAX] [--dump-luts]

See PPTX for background