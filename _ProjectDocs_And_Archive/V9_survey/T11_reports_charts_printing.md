# T11 — Reports, Charts, Printing (survey notes)

Evidence: HEAD = `0c274d5`. Line refs are HEAD.

## 1. Architecture (confirmed)

| Piece | File | Role |
|---|---|---|
| `ReportEngine` (Module) | `Reports/ReportEngine.vb` (1141 LOC) | Shared: `BuildReportDataset`, `GetBalanceBeforeDate`, `BuildRowList`, template info cache (`GetTemplateInfo`/`LoadTemplateInfoFromDatabase`/`ClearTemplateInfoCache`), GDI+ helpers (`DrawHeader`, `DrawBorder`, `DrawPageNumber`), Thai formatting (`ThaiNumerals`, `ToThaiNumber`, `ThaiMonthAbbr/Full`, `ToBuddhistDateShort/Full/YearThai`, `FormatThaiAmount`, `CreateFittedFont`) |
| `IncomeExpenseReport` | `Reports/IncomeExpenseReport.vb` (990 LOC) | `PrintDocument` subclass; `ReportModes` enum (Detailed/Summary); A4 paper config (`CreateA4Paper`/`ConfigurePageSettings`); full GDI+ `OnPrintPage` |
| `FrmReports` | `Forms/FrmReports.vb` (825 LOC) | Report center: date range, fund/bank combos, manual opening balance, preview/print/PDF/CSV buttons, monthly chart |
| `FrmChartPreview` | `Forms/FrmChartPreview.vb` (509 LOC) | Full-screen chart viewer (clone of source Chart) |
| Data types | ReportEngine.vb | `ReportInfo`, `TempleSettingInfo`, `ReportRow`, `ReportDataset`, `LayoutConfig` (A4Landscape + `CalculateColumnWidths`/`GetRemainingHeight`/`HasSpaceFor`), `ReportTheme` (IDisposable) |

`BuildReportDataset` consumers: **only** `IncomeExpenseReport.LoadData` (line 145). `GetTemplateInfo` also consumed by `TransactionExcelExporter.vb:38`. `ClearTemplateInfoCache` called from `FrmTempleSetting.vb:353` and `FrmOnboardingWizard.vb:658` (after saving temple/personnel settings — correct invalidation).

## 2. Report pipeline (verified)

- Dates from UI: `dtpFrom.Value.Date` … `dtpTo.Value.Date.AddDays(1).AddSeconds(-1)` (include whole end day) — used identically in `btnPrintSummary_Click` (762-775), `btnSavePdf_Click` (777-790), `btnPrintReport_Click` (797-810).
- Three entry points, all via `IncomeExpenseReport`:
  - `ShowPreview(...)` → `PrintPreviewDialog` (default path)
  - `ShowPreviewWithPdfExport(...)` → preview + injected "💾 บันทึกเป็น PDF" toolbar button (`AddPdfButtonToPreviewToolbar` scans `ppd.Controls` for the ToolStrip; silently returns if not found — graceful degradation)
  - `ShowPrint(...)` → direct `PrintDialog` without preview (Ctrl+P-style)
- `_lastReportMode` remembers Detailed/Summary between actions in-session.
- **Session printer memory:** `_selectedPrinterName` (shared field) + `ApplySessionPrinter(doc)` reapplies the chosen printer to every new report doc; resets itself if the printer was uninstalled (try/catch).

## 3. PDF export mechanism (important design fact)

`SaveToPdf(filePath)` (IncomeExpenseReport.vb:911-955):
- Uses the **Windows virtual printer "Microsoft Print to PDF"** — enumerates `PrinterSettings.InstalledPrinters`; if absent throws `ApplicationException` with Thai instructions (Settings > Devices > enable it).
- Sets `PrinterSettings.PrintToFile = True` + `PrintFileName = filePath`, re-runs `ConfigurePageSettings()` (A4 + safe margins for the switched printer), then `Me.Print()`. On failure resets `PrintToFile` and rethrows.
- **No external PDF library** (no iText/PdfSharp NuGet — zero matches project-wide). `ShowSavePdfDialog` (958-997) prenames `รายงาน_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf` (InvariantCulture stamp — culture-safe).
- Consequence: PDF output quality/behavior depends on an OS printer driver, not code. Also PDF saving is a **synchronous UI-thread print** (see T07 category, P2).

## 4. Page balancing + signature block (V6 feature, re-verified)

- `PreCalculatePageBreaks(rowH, pageBottom)` (416+) computes per-page row counts up front using `H_perPage` (table header + rows + final summary + signature) and `CalculateFinalContentHeight` = `H_summary + H_signature + H_footerGap + H_topGap` (514-527).
- If the final page lacks room for summary+signatures, it force-breaks to a fresh page for signatures only (389-395).
- `DrawSummaries` (mid + final) and `DrawMidSummary` (529+) with `BuildCarryForwardLabel` (632) for carried-forward opening balance.
- `DrawSignatures` (654+): left = "ตรวจถูกต้องแล้ว / เจ้าอาวาส", right = "ผู้จัดทำบัญชี / ไวยาวัจกร"; dotted lines via `DrawDottedSignatureLine`; names from `ReportEngine.GetTemplateInfo()` (TempleSetting-backed, cached).
- Text fitting: `CreateFittedBoldFont`/`Truncate` shrink/truncate to column width.

## 5. Charts (V6 feature, re-verified)

- `System.Windows.Forms.DataVisualization.Chart` (WinForms DataVisualization — not a NuGet chart lib).
- `FrmReports.vb:368`: re-renders `chartMonthly` and opens `FrmChartPreview` via `Using frm As New FrmChartPreview()`; `LoadChart(source)` **clones** ChartAreas/Series (incl. all data points)/Legends/Titles into its own Chart (palette forced to None, then re-set), enables `ScaleView.Zoomable` on both axes.
- Features: mouse-wheel zoom on Y (ChartPreview_MouseWheel, 270+), double-click zoom reset, pan/scroll, "รวม" totals toggle (`btnToggleTotals_Click` + per-point color save/restore map), print via `PrintDocument` over a pre-rendered `_chartBitmap`, save image PNG/JPEG (`btnSaveImage_Click`, 396+).
- Only FrmReports opens it (single call site).

## 6. CSV export from report grid

`FrmReports.vb:700`: `Path.Combine(AppPaths.ExportFolder, $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv")` — `DateTime.Now` format string with InvariantCulture digits → culture-safe at HEAD. (Old builds on disk produced Buddhist-year names `Report_25690726_*.csv` — see T05; consistent with the pre-InvariantCulture era.)

## 7. Issues / debt found

1. **Duplicated Thai-formatting helpers:** `IncomeExpenseReport` carries 6 private copies (`ToThaiNumerals`, `ThaiMonthAbbr`, `ThaiMonthFull`, `ToBuddhistDateShort`, `ToBuddhistFull`, `ToBuddhistYearThai`, `FormatThaiAmount` — lines 172-227) that duplicate `ReportEngine`'s public ones. Drift already present: `ReportEngine.ToBuddhistYearThai` returns **Integer**, the private copy returns **String**. Should delegate to ReportEngine (single source of truth).
2. `dgvReport_CellContentClick` (812-814) is an **empty event handler** — dead stub wired in Designer.
3. PDF export + direct print run synchronously on the UI thread (large reports with many receipt rows will hitch; no async/Task.Run — same pattern family as T07 P1s, but lower frequency).
4. PDF depends on a named OS printer ("Microsoft Print to PDF"); localized/renamed Windows installs or machines without the optional feature get an exception with guidance (handled, but a hard external dependency).
5. `ShowPreviewWithPdfExport`'s toolbar-button injection silently no-ops if the dialog's ToolStrip layout changes (Windows update) — user then has no PDF button but no error either (acceptable degradation, worth knowing).

## 8. V8 delta items (reports)

- ✅ V8 outstanding #6 — **"BaseReport remains dead scaffolding" is RESOLVED**: `Class BaseReport` was added in `71c27f4` and removed before `91d2a5e`; at HEAD `BaseReport` exists **only in 10 markdown docs** (AUDIT_REPORT, Docs/*, PROJECT_PROFILE_V4-V8, System_Stability audit). No code reference. V9 profile should drop that line.
- V6/V5 feature claims re-verified true: FrmChartPreview zoom/pan/toggle/print/save; preview-first PDF with signature block; dynamic page balancing; `btnSavePdf`; centralized ReportEngine.
