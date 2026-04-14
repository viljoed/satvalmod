"""
sat_val_mod_ui.py
=================
Tkinter GUI front-end for sat_val_mod.py (colour / shade fusion).

Layout
------
  - File selection  : colour raster, shade raster, output file
  - Parameters      : vmin, vexp, smin, sexp, cutoff, shade-max, format, dump LUTs
  - LUT preview     : live canvas charts of value and saturation curves
  - Run button      : executes the fusion in a background thread
  - Log panel       : scrollable output showing progress and errors

Usage
-----
    python sat_val_mod_ui.py
"""

import os
import sys
import threading
import tkinter as tk
from tkinter import filedialog, messagebox, ttk

# ---------------------------------------------------------------------------
# Locate sat_val_mod.py — must be in the same directory as this UI script
# ---------------------------------------------------------------------------
_SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, _SCRIPT_DIR)

try:
    import sat_val_mod as svm
except ImportError:
    messagebox.showerror(
        "Import Error",
        "Cannot find sat_val_mod.py.\n"
        "Please place sat_val_mod_ui.py in the same folder as sat_val_mod.py."
    )
    sys.exit(1)

try:
    import numpy as np
except ImportError:
    messagebox.showerror("Import Error", "numpy is required.  Run: uv pip install numpy")
    sys.exit(1)


# ---------------------------------------------------------------------------
# Colours / styling
# ---------------------------------------------------------------------------
BG          = "#f5f5f5"
PANEL_BG    = "#ffffff"
ACCENT      = "#2d6a9f"
ACCENT_LITE = "#e8f0fa"
BTN_RUN     = "#2d6a9f"
BTN_RUN_FG  = "#ffffff"
LABEL_FG    = "#222222"
ENTRY_BG    = "#ffffff"
LOG_BG      = "#1e1e1e"
LOG_FG      = "#d4d4d4"
LOG_ERR     = "#f48771"
LOG_OK      = "#89d185"
CURVE_VAL   = "#e07b39"   # orange  — value LUT
CURVE_SAT   = "#4ea6dc"   # blue    — saturation LUT
CHART_BG    = "#fafafa"
CHART_GRID  = "#e0e0e0"

PAD = 8


# ---------------------------------------------------------------------------
# Helper — labelled entry row
# ---------------------------------------------------------------------------
def _labelled_entry(parent, label, var, row, tooltip=None,
                    width=10, col=0, padx=PAD, pady=3):
    tk.Label(parent, text=label, bg=PANEL_BG, fg=LABEL_FG,
             anchor="w").grid(row=row, column=col, sticky="w",
                              padx=(padx, 2), pady=pady)
    e = tk.Entry(parent, textvariable=var, width=width,
                 bg=ENTRY_BG, relief="solid", bd=1)
    e.grid(row=row, column=col + 1, sticky="w", padx=(0, padx), pady=pady)
    if tooltip:
        _Tooltip(e, tooltip)
    return e


# ---------------------------------------------------------------------------
# Simple tooltip
# ---------------------------------------------------------------------------
class _Tooltip:
    def __init__(self, widget, text):
        self._widget = widget
        self._text   = text
        self._win    = None
        widget.bind("<Enter>", self._show)
        widget.bind("<Leave>", self._hide)

    def _show(self, _event=None):
        x = self._widget.winfo_rootx() + 20
        y = self._widget.winfo_rooty() + self._widget.winfo_height() + 4
        self._win = tw = tk.Toplevel(self._widget)
        tw.wm_overrideredirect(True)
        tw.wm_geometry(f"+{x}+{y}")
        tk.Label(tw, text=self._text, bg="#ffffc0", fg="#000000",
                 relief="solid", bd=1, padx=4, pady=2,
                 font=("Segoe UI", 8)).pack()

    def _hide(self, _event=None):
        if self._win:
            self._win.destroy()
            self._win = None


# ---------------------------------------------------------------------------
# LUT preview canvas
# ---------------------------------------------------------------------------
class LutCanvas(tk.Canvas):
    """Draws value and saturation modulation curves from current parameters."""

    W, H   = 340, 160
    MARGIN = 30

    def __init__(self, parent, **kwargs):
        super().__init__(parent, width=self.W, height=self.H,
                         bg=CHART_BG, bd=0, highlightthickness=1,
                         highlightbackground="#cccccc", **kwargs)

    def redraw(self, val_lut: np.ndarray, sat_lut: np.ndarray):
        self.delete("all")
        w, h   = self.W, self.H
        mx, my = self.MARGIN, self.MARGIN // 2
        pw     = w - mx - 6        # plot width
        ph     = h - my - 18       # plot height

        # Grid lines
        for frac in (0.0, 0.25, 0.5, 0.75, 1.0):
            y = my + ph - int(frac * ph)
            self.create_line(mx, y, mx + pw, y, fill=CHART_GRID, dash=(2, 3))
            self.create_text(mx - 3, y, text=f"{frac:.2f}", anchor="e",
                             font=("Segoe UI", 7), fill="#888888")

        # Axes
        self.create_line(mx, my, mx, my + ph, fill="#aaaaaa", width=1)
        self.create_line(mx, my + ph, mx + pw, my + ph, fill="#aaaaaa", width=1)

        n = len(val_lut)

        def _pts(lut, colour):
            pts = []
            for i, val in enumerate(lut):
                x = mx + int(i / (n - 1) * pw)
                y = my + ph - int(np.clip(val, 0, 1) * ph)
                pts.extend([x, y])
            if len(pts) >= 4:
                self.create_line(*pts, fill=colour, width=2, smooth=True)

        _pts(val_lut, CURVE_VAL)
        _pts(sat_lut, CURVE_SAT)

        # Legend
        self.create_line(mx + pw - 110, my + 8,
                         mx + pw - 90,  my + 8, fill=CURVE_VAL, width=2)
        self.create_text(mx + pw - 86, my + 8, anchor="w",
                         text="Value", font=("Segoe UI", 8), fill=CURVE_VAL)
        self.create_line(mx + pw - 50, my + 8,
                         mx + pw - 30, my + 8, fill=CURVE_SAT, width=2)
        self.create_text(mx + pw - 26, my + 8, anchor="w",
                         text="Sat", font=("Segoe UI", 8), fill=CURVE_SAT)

        # X-axis label
        self.create_text(mx + pw // 2, h - 4, text="Shade value →",
                         font=("Segoe UI", 7), fill="#888888")


# ---------------------------------------------------------------------------
# Main application window
# ---------------------------------------------------------------------------
class App(tk.Tk):

    def __init__(self):
        super().__init__()
        self.title("Sat/Val Modulation — Colour Shade Fusion")
        self.configure(bg=BG)
        self.resizable(False, False)

        # ── Tk variables ────────────────────────────────────────────────
        self.v_colour    = tk.StringVar()
        self.v_shade     = tk.StringVar()
        self.v_output    = tk.StringVar()
        self.v_format    = tk.StringVar(value="tif")
        self.v_vmin      = tk.StringVar(value="0.0")
        self.v_vexp      = tk.StringVar(value="1.0")
        self.v_smin      = tk.StringVar(value="0.5")
        self.v_sexp      = tk.StringVar(value="1.0")
        self.v_cutoff    = tk.StringVar(value="200")
        self.v_shade_max = tk.StringVar(value="255")
        self.v_dump_luts = tk.BooleanVar(value=False)

        # Trigger live LUT preview on parameter change
        for v in (self.v_vmin, self.v_vexp, self.v_smin,
                  self.v_sexp, self.v_cutoff, self.v_shade_max):
            v.trace_add("write", self._schedule_lut_update)

        self._lut_after_id = None

        self._build_ui()
        self._update_lut_preview()

    # ── UI construction ──────────────────────────────────────────────────

    def _build_ui(self):
        outer = tk.Frame(self, bg=BG, padx=PAD, pady=PAD)
        outer.pack(fill="both", expand=True)

        # ── File selection panel ─────────────────────────────────────────
        fp = self._panel(outer, "Input / Output Files")
        fp.pack(fill="x", pady=(0, PAD))
        self._file_row(fp, "Colour raster:",  self.v_colour,  0,
                       self._browse_colour, "(RGB, any GDAL format)")
        self._file_row(fp, "Shade raster:",   self.v_shade,   1,
                       self._browse_shade,  "(greyscale hillshade)")
        self._file_row(fp, "Output file:",    self.v_output,  2,
                       self._browse_output, "(.tif or .jpg)")

        # ── Parameters + LUT preview (side by side) ──────────────────────
        mid = tk.Frame(outer, bg=BG)
        mid.pack(fill="x", pady=(0, PAD))

        # Parameters panel
        pp = self._panel(mid, "Modulation Parameters")
        pp.pack(side="left", fill="y", padx=(0, PAD))
        self._build_params(pp)

        # LUT preview panel
        lp = self._panel(mid, "LUT Preview")
        lp.pack(side="left", fill="both", expand=True)
        self._build_lut_panel(lp)

        # ── Run button ───────────────────────────────────────────────────
        btn_frame = tk.Frame(outer, bg=BG)
        btn_frame.pack(fill="x", pady=(0, PAD))

        self._run_btn = tk.Button(
            btn_frame, text="▶  Run Fusion",
            command=self._run,
            bg=BTN_RUN, fg=BTN_RUN_FG,
            font=("Segoe UI", 10, "bold"),
            relief="flat", padx=18, pady=6, cursor="hand2",
            activebackground="#1e4f7a", activeforeground=BTN_RUN_FG,
        )
        self._run_btn.pack(side="left")

        self._progress = ttk.Progressbar(btn_frame, mode="indeterminate",
                                          length=200)
        self._progress.pack(side="left", padx=(PAD, 0))

        # ── Log panel ────────────────────────────────────────────────────
        lf = self._panel(outer, "Log")
        lf.pack(fill="both", expand=True)
        self._build_log(lf)

    def _panel(self, parent, title):
        """Return a labelled frame with white background."""
        f = tk.LabelFrame(parent, text=f"  {title}  ",
                          bg=PANEL_BG, fg=ACCENT,
                          font=("Segoe UI", 9, "bold"),
                          bd=1, relief="solid",
                          padx=PAD, pady=PAD)
        return f

    def _file_row(self, parent, label, var, row, browse_cmd, hint=""):
        tk.Label(parent, text=label, bg=PANEL_BG, fg=LABEL_FG,
                 width=14, anchor="w").grid(row=row, column=0,
                                            sticky="w", pady=3)
        e = tk.Entry(parent, textvariable=var, width=46,
                     bg=ENTRY_BG, relief="solid", bd=1)
        e.grid(row=row, column=1, sticky="ew", padx=(0, 4), pady=3)
        tk.Button(parent, text="Browse…", command=browse_cmd,
                  bg=ACCENT_LITE, fg=ACCENT, relief="flat",
                  padx=6, cursor="hand2").grid(row=row, column=2,
                                               sticky="w", pady=3)
        if hint:
            tk.Label(parent, text=hint, bg=PANEL_BG,
                     fg="#888888", font=("Segoe UI", 8)
                     ).grid(row=row, column=3, sticky="w", padx=(4, 0))
        parent.columnconfigure(1, weight=1)

    def _build_params(self, parent):
        # Value section
        tk.Label(parent, text="Value modulation",
                 bg=PANEL_BG, fg=ACCENT,
                 font=("Segoe UI", 8, "bold")).grid(
            row=0, column=0, columnspan=2, sticky="w", pady=(0, 2))

        _labelled_entry(parent, "Vmin (0–1):", self.v_vmin, 1,
                        tooltip="Minimum brightness when shade=0. 0=black, 1=unchanged.")
        _labelled_entry(parent, "Vexp (>0):",  self.v_vexp, 2,
                        tooltip="Curve shape. <1=concave, 1=linear, >1=convex.")

        tk.Frame(parent, bg="#e0e0e0", height=1).grid(
            row=3, column=0, columnspan=2, sticky="ew", pady=6)

        # Saturation section
        tk.Label(parent, text="Saturation modulation",
                 bg=PANEL_BG, fg=ACCENT,
                 font=("Segoe UI", 8, "bold")).grid(
            row=4, column=0, columnspan=2, sticky="w", pady=(0, 2))

        _labelled_entry(parent, "Smin (0–1):", self.v_smin, 5,
                        tooltip="Minimum saturation factor above cutoff. 0=grey, 1=unchanged.")
        _labelled_entry(parent, "Sexp (>0):",  self.v_sexp, 6,
                        tooltip="Curve shape for saturation roll-off above cutoff.")

        tk.Frame(parent, bg="#e0e0e0", height=1).grid(
            row=7, column=0, columnspan=2, sticky="ew", pady=6)

        # Threshold section
        tk.Label(parent, text="Thresholds",
                 bg=PANEL_BG, fg=ACCENT,
                 font=("Segoe UI", 8, "bold")).grid(
            row=8, column=0, columnspan=2, sticky="w", pady=(0, 2))

        _labelled_entry(parent, "Cutoff:", self.v_cutoff, 9,
                        tooltip="Shade value below which value is modulated; "
                                "saturation is modulated above this.")
        _labelled_entry(parent, "Shade max:", self.v_shade_max, 10,
                        tooltip="Maximum shade pixel value (usually 255, sometimes 100).")

        tk.Frame(parent, bg="#e0e0e0", height=1).grid(
            row=11, column=0, columnspan=2, sticky="ew", pady=6)

        # Output format
        tk.Label(parent, text="Output format",
                 bg=PANEL_BG, fg=ACCENT,
                 font=("Segoe UI", 8, "bold")).grid(
            row=12, column=0, columnspan=2, sticky="w", pady=(0, 4))

        fmt_frame = tk.Frame(parent, bg=PANEL_BG)
        fmt_frame.grid(row=13, column=0, columnspan=2, sticky="w")
        for fmt in ("tif", "jpg"):
            tk.Radiobutton(fmt_frame, text=fmt.upper(),
                           variable=self.v_format, value=fmt,
                           bg=PANEL_BG, fg=LABEL_FG,
                           activebackground=PANEL_BG,
                           selectcolor=ACCENT_LITE).pack(side="left", padx=(0, 8))

        # Dump LUTs
        tk.Checkbutton(parent, text="Dump LUTs to CSV",
                       variable=self.v_dump_luts,
                       bg=PANEL_BG, fg=LABEL_FG,
                       activebackground=PANEL_BG,
                       selectcolor=ACCENT_LITE,
                       font=("Segoe UI", 8)
                       ).grid(row=14, column=0, columnspan=2,
                              sticky="w", pady=(8, 0))

    def _build_lut_panel(self, parent):
        self._lut_canvas = LutCanvas(parent)
        self._lut_canvas.pack(padx=PAD, pady=PAD)
        tk.Label(parent,
                 text="Orange = Value curve    Blue = Saturation curve",
                 bg=PANEL_BG, fg="#666666",
                 font=("Segoe UI", 8)).pack()

    def _build_log(self, parent):
        self._log = tk.Text(parent, height=10, bg=LOG_BG, fg=LOG_FG,
                            font=("Consolas", 9), relief="flat",
                            state="disabled", wrap="word")
        self._log.tag_configure("error", foreground=LOG_ERR)
        self._log.tag_configure("ok",    foreground=LOG_OK)
        sb = ttk.Scrollbar(parent, command=self._log.yview)
        self._log.configure(yscrollcommand=sb.set)
        self._log.pack(side="left", fill="both", expand=True)
        sb.pack(side="right", fill="y")

    # ── File browsers ─────────────────────────────────────────────────────

    def _browse_colour(self):
        p = filedialog.askopenfilename(
            title="Select colour raster",
            filetypes=[("Raster files", "*.tif *.tiff *.jpg *.jpeg *.img *.vrt"),
                       ("All files", "*.*")])
        if p:
            self.v_colour.set(p)
            if not self.v_output.get():
                base = os.path.splitext(p)[0]
                ext  = ".tif" if self.v_format.get() == "tif" else ".jpg"
                self.v_output.set(base + "_fused" + ext)

    def _browse_shade(self):
        p = filedialog.askopenfilename(
            title="Select shade / hillshade raster",
            filetypes=[("Raster files", "*.tif *.tiff *.jpg *.jpeg *.img *.vrt"),
                       ("All files", "*.*")])
        if p:
            self.v_shade.set(p)

    def _browse_output(self):
        fmt = self.v_format.get()
        ext = ".tif" if fmt == "tif" else ".jpg"
        p = filedialog.asksaveasfilename(
            title="Save output raster as",
            defaultextension=ext,
            filetypes=[("GeoTIFF", "*.tif"), ("JPEG", "*.jpg"),
                       ("All files", "*.*")])
        if p:
            self.v_output.set(p)

    # ── LUT preview ───────────────────────────────────────────────────────

    def _schedule_lut_update(self, *_args):
        """Debounce: update preview 300 ms after the last keystroke."""
        if self._lut_after_id:
            self.after_cancel(self._lut_after_id)
        self._lut_after_id = self.after(300, self._update_lut_preview)

    def _update_lut_preview(self):
        try:
            vmin      = float(self.v_vmin.get())
            vexp      = float(self.v_vexp.get())
            smin      = float(self.v_smin.get())
            sexp      = float(self.v_sexp.get())
            cutoff    = int(self.v_cutoff.get())
            shade_max = int(self.v_shade_max.get())
            if shade_max < 1 or cutoff < 0 or cutoff > shade_max:
                return
            val_lut = svm.build_value_lut(vmin, vexp, cutoff, shade_max)
            sat_lut = svm.build_saturation_lut(smin, sexp, cutoff, shade_max)
            self._lut_canvas.redraw(val_lut, sat_lut)
        except (ValueError, ZeroDivisionError):
            pass   # silently ignore incomplete input while typing

    # ── Validation ────────────────────────────────────────────────────────

    def _validate(self):
        errors = []
        if not self.v_colour.get():
            errors.append("Colour raster path is required.")
        elif not os.path.isfile(self.v_colour.get()):
            errors.append(f"Colour raster not found:\n  {self.v_colour.get()}")

        if not self.v_shade.get():
            errors.append("Shade raster path is required.")
        elif not os.path.isfile(self.v_shade.get()):
            errors.append(f"Shade raster not found:\n  {self.v_shade.get()}")

        if not self.v_output.get():
            errors.append("Output file path is required.")

        def _chk_float(name, var, lo=None, hi=None, excl_zero=False):
            try:
                val = float(var.get())
                if lo is not None and val < lo:
                    errors.append(f"{name} must be ≥ {lo}.")
                if hi is not None and val > hi:
                    errors.append(f"{name} must be ≤ {hi}.")
                if excl_zero and val <= 0:
                    errors.append(f"{name} must be > 0.")
            except ValueError:
                errors.append(f"{name} must be a number.")

        _chk_float("Vmin", self.v_vmin, lo=0.0, hi=1.0)
        _chk_float("Vexp", self.v_vexp, excl_zero=True)
        _chk_float("Smin", self.v_smin, lo=0.0, hi=1.0)
        _chk_float("Sexp", self.v_sexp, excl_zero=True)

        try:
            cutoff    = int(self.v_cutoff.get())
            shade_max = int(self.v_shade_max.get())
            if shade_max < 1:
                errors.append("Shade max must be ≥ 1.")
            elif cutoff < 0 or cutoff > shade_max:
                errors.append(f"Cutoff must be in [0, {shade_max}].")
        except ValueError:
            errors.append("Cutoff and Shade max must be integers.")

        return errors

    # ── Run ───────────────────────────────────────────────────────────────

    def _run(self):
        errors = self._validate()
        if errors:
            messagebox.showerror("Validation Error", "\n".join(errors))
            return

        self._run_btn.configure(state="disabled")
        self._progress.start(12)
        self._log_clear()
        self._log_write("Starting fusion…\n")

        params = dict(
            colour_path   = self.v_colour.get(),
            shade_path    = self.v_shade.get(),
            output_path   = self.v_output.get(),
            output_format = self.v_format.get(),
            vmin          = float(self.v_vmin.get()),
            vexp          = float(self.v_vexp.get()),
            smin          = float(self.v_smin.get()),
            sexp          = float(self.v_sexp.get()),
            cutoff        = int(self.v_cutoff.get()),
            shade_max     = int(self.v_shade_max.get()),
            dump_luts     = self.v_dump_luts.get(),
        )

        threading.Thread(target=self._worker, kwargs=params, daemon=True).start()

    def _worker(self, **params):
        """Run fusion in a background thread; redirect stdout to the log."""
        import io

        class _LogCapture(io.TextIOBase):
            def __init__(self, callback):
                self._cb = callback
            def write(self, s):
                if s:
                    self._cb(s)
                return len(s)
            def flush(self):
                pass

        old_stdout = sys.stdout
        sys.stdout = _LogCapture(lambda s: self.after(0, self._log_write, s))

        try:
            svm.fuse_colour_with_shade(**params)
            self.after(0, self._log_write, "\n✔ Fusion complete.\n", "ok")
            self.after(0, lambda: messagebox.showinfo(
                "Done", f"Output written to:\n{params['output_path']}"))
        except Exception as exc:
            msg = f"\n✘ Error: {exc}\n"
            self.after(0, self._log_write, msg, "error")
            self.after(0, lambda: messagebox.showerror("Fusion Error", str(exc)))
        finally:
            sys.stdout = old_stdout
            self.after(0, self._run_finished)

    def _run_finished(self):
        self._progress.stop()
        self._run_btn.configure(state="normal")

    # ── Log helpers ───────────────────────────────────────────────────────

    def _log_clear(self):
        self._log.configure(state="normal")
        self._log.delete("1.0", "end")
        self._log.configure(state="disabled")

    def _log_write(self, text, tag=None):
        self._log.configure(state="normal")
        if tag:
            self._log.insert("end", text, tag)
        else:
            self._log.insert("end", text)
        self._log.see("end")
        self._log.configure(state="disabled")


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------
if __name__ == "__main__":
    app = App()
    app.mainloop()
