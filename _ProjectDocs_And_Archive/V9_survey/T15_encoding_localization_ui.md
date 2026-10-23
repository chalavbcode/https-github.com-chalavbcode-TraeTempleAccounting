# T15 — Encoding / Localization / UI Standards (survey notes)

Evidence: HEAD = `0c274d5` + full-tree file scan. Line refs are HEAD.

## 1. Source encoding (full-tree scan of .vb/.resx/.vbproj/.manifest/.md/.slnx)

- **131 text files**: 63 UTF-8 **with** BOM, 68 **without** BOM, **0 UTF-8 decode failures, 0 mojibake**.
- Thai literals throughout source/comments/resources are clean; no legacy-TIS620 or CP874 contamination.
- Mixed BOM is harmless for the SDK compiler (net10.0) — only a cosmetic/VS-noise concern.

## 2. Culture handling

- **No global culture pinning anywhere** (`Thread.CurrentCulture`/`CurrentUICulture` never assigned) → the app runs entirely in the OS's culture; on Thai machines that is `th-TH` (Buddhist calendar).
- Defensive parsing is good: import paths try `{CurrentCulture, th-TH, InvariantCulture, en-US}` chains (`FrmMultiMachineImport.vb:653`, `ExcelTransactionImporter.vb:320-321`) before failing.
- **Format strings without `InvariantCulture`: ~19 date sites** (`ToString("yyyy…")` on Now/Value/Today). Known consequences already evidenced in T05:
  - `FrmReports.vb:700` → `Report_{yyyyMMdd_HHmmss}.csv` still yields **Buddhist-year CSV names** on th-TH machines (disk: `Report_25690726_*.csv`).
  - All three log writers → **2569-… timestamps** in `crash.log`/`error.log` (T14).
  - `Db.BackupDatabase` pre-merge backup names historically Buddhist-year; tiered + new engine names are culture-safe (T06).
- ~72 number-format sites (`ToString("N2")`/`"#,##0"`/plain) — display-only, CurrentCulture is *desirable* there (Thai digit grouping); not an issue.
- Report layer does culture **explicitly and correctly**: `ReportEngine` Thai-date helpers + `FrmReports` th-TH conversions with `>2400 → −543` guards (425, 439).

## 3. UI standards

### DPI / scaling (17 forms total)
- **Dpi ×5**: FrmExportForTransfer, FrmMultiMachineImport, FrmOnboardingWizard, FrmTempleSetting (Designer) + **FrmChartPreview (code, line 49)** — with an explicit in-code warning comment in one Designer: `' AutoScaleMode = Dpi (ห้าม Font — จะทำให้ขนาดเพี้ยน)` ("never Font — sizes drift").
- **Font ×6**: FrmHelpDialog, FrmPersonnelManagement, FrmReceiptViewer, FrmReports, FrmTransactions, frmMain.
- **UNSET ×6**: FrmExpense, FrmIncome, FrmLocationImport, FrmMasterData, FrmTransfer (Designer) + FrmBackupVerify (no AutoScaleMode anywhere).
- Process-level: `Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)` in `Program.vb` (86); `app.manifest` has **no DPI-awareness element** (only `asInvoker` + Win10 `supportedOS`) — PerMonitorV2 comes from code, which is sufficient for net10.0 WinForms.

### Fonts
- **Tahoma ×326** (`New Font("Tahoma", …)`) — dominant, Thai-safe; Consolas ×3 (logs/code), Segoe UI Symbol/Emoji ×4 (unicode glyphs). No exotic families, no font fallback traps.

### UiFitter (Thai-text fit system, `UiFitter.vb`)
- Purpose: prevent Thai vowel/tone marks clipping — measures with `TextRenderer` (+8px pad), shrinks font 0.5pt steps (floor 9pt), then 2-line wrap, then grows the button; skips `AutoSize` buttons.
- Call sites: `AutoFitFormButtons` ×**14**, `DisableComboBoxWheel` ×8.
- **3 forms don't use it**: FrmBackupVerify, FrmChartPreview, FrmReceiptViewer (largely code-built/tool windows; ChartPreview's buttons are fixed-width 160px created via `CreateToolButton`).

### F1 help coverage (17 forms)
- **F1 works on 13 forms + frmMain.**
- **Missing on: FrmBackupVerify, FrmOnboardingWizard, FrmReceiptViewer.** (FrmHelpDialog itself legitimately has none.)

## 4. Issues / debt found (T15)

1. **P2 — mixed AutoScaleMode (Dpi 5 / Font 6 / unset 6)**: the six unset forms fall back to `Inherit` → default = Font scaling; combined with PerMonitorV2 this is where V8's "DPI per-form lottery" note stands. A project-wide decision (the in-code comment says Dpi-only) has never been applied.
2. **P2 — remaining culture-sensitive date formats** (~19 sites): visible user impact on th-TH machines = Buddhist-era file names (Report CSV) and log timestamps; hidden impact anywhere a `Date` is serialized to text (T05 has the full inventory).
3. **P3 — no global culture strategy**: relying on OS culture with per-site multi-culture parsing works, but every new `ToString`/`Parse` site is a fresh foot-gun (the project has been bitten 4+ times: error.log, crash.log, Report CSV, pre-merge backup names).
4. **P3 — F1 gaps on 3 forms** (incl. the wizard, which is the very onboarding surface).
5. **Info — no manifest DPI element**: fine with PerMonitorV2-in-code, but the manifest's `supportedOS` list only declares Win10 (adequate; Win8.1 GUID absent — irrelevant for .NET 10).

## 5. Verified-good

- Zero encoding corruption across 131 files; consistent Thai UI language in source strings; Tahoma monoculture; multi-culture parse chains at every import boundary; Thai-report date handling is explicit and era-guarded; UiFitter solves a real Thai-UI problem systematically.
