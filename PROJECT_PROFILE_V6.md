# PROJECT_PROFILE_V6 — Temple Accounting System (ระบบบัญชีวัด)

> **Document version:** V6 (deep audit of V5→V6 delta + full current-state snapshot)
> **Generated:** 2026-08-09
> **Scope:** Every change landed after `PROJECT_PROFILE_V5.md` was written (V5 = 2026-08-08), plus the audited end-state of the report engine, chart preview, F1 help system, and git repository layout.
> **Companion documents (kept as historical references, NOT modified):** `PROJECT_PROFILE_V5.md`, `PROJECT_PROFILE_V4.md`, `PROJECT_PROFILE_V3.md`, `PROJECT_PROFILE_V2.md`, `Project_Profile_Document.md`.

---

## 1. Project Metadata & Versioning History

### 1.1 Document / Versioning History

| Profile | Generated | Covers | Key Delta |
|---|---|---|---|
| Project_Profile_Document.md | early | Full system (pre-V2 baseline) | Original audit |
| V2 | 2026-08-08 | Full system | Post-refactor state, F1 Help System intro |
| V3 | 2026-08-08 | Full system | UI auto-fit (UiFitter), personnel-ID migration |
| V4 | 2026-08-08 | Full system | 12-item known-issue log, 8-button toolbar baseline |
| **V5** | 2026-08-08 | Full system | `FrmMultiMachineImport` DPI rebuild, print hardening + `UseLandscape`/safe margins, `btnSavePdf`, MSB3577 fix, multi-machine forms documented |
| **V6 (this)** | **2026-08-09** | Full system + V6 delta | `FrmChartPreview` full-screen chart, FlowLayoutPanel button bar, dynamic page-row balancing, signature-block bottom anchoring, Preview→PDF export flow, F1 manual sections 10–11 added, git `perfect-point-v0` branch |

### 1.2 Project Identity

| Field | Value |
|---|---|
| Product name | Temple Accounting System — ระบบบัญชีวัด |
| Publisher (installer) | วัดดอนเมือง |
| Version | 1.0.0 (Inno Setup `TempleAccountingSetup.iss`, `SimpleInstaller.ps1`) |
| License | The Unlicense |
| Repository | `TempleAccounting_FullProject_20260726_090422` |
| Solution | `TempleAccounting.slnx` (SDK-style, `EnableDefaultCompileItems=false`, explicit `<Compile>`/`<EmbeddedResource>` items) |

### 1.3 Technology Stack (unchanged baseline, V6 re-verified)

| Layer | Technology |
|---|---|
| Language | VB.NET, `Option Strict Off` / `Option Explicit On` |
| Runtime | .NET 10.0-windows (`net10.0-windows`), preview SDK 10.0.400-preview.0.26322.102, `StartupObject = TempleAccounting.Program` |
| UI | WinForms, `AutoScaleMode.Font` (default) / `AutoScaleMode.Dpi` on `FrmTempleSetting`, `FrmMultiMachineImport`, **and now `FrmChartPreview`** |
| DB | MS Access `.accdb` via OLEDB `Microsoft.ACE.OLEDB.12.0` (NuGet `System.Data.OleDb` 10.0.10); `Db.EnsureSchema()` self-healing |
| Charts | `System.Windows.Forms.DataVisualization` (NuGet `WinForms.DataVisualization` 1.10.2) — `FrmReports.chartMonthly` + **new `FrmChartPreview.chartPreview`** |
| Printing | GDI+ `PrintDocument` subclass (`IncomeExpenseReport`) + `PrintPreviewDialog`; chart printing via bitmap render |
| Help | Markdown `USER_MANUAL_TH.md` + `HelpSystem` / `FrmHelpDialog` (F1 contextual) — **now 12 manual sections** |

### 1.4 Build Verification (V6)

- `dotnet build TempleAccounting.vbproj -c Debug` → `Build succeeded, 0 Warning(s), 0 Error(s)`.
- `dotnet build TempleAccounting.vbproj -c Release` → `Build succeeded, 0 Warning(s), 0 Error(s)`.
- Smoke tests: report Print Preview (Detail + Summary), chart preview open/print/save-image/toggle-totals, F1 help dialogs on `FrmReports`, `FrmMultiMachineImport`, `FrmExportForTransfer`.

---

## 2. New Features & Enhancements (V5 → V6 delta)

### 2.1 NEW — `FrmChartPreview` (Full-Screen Chart Preview Form) **[feature: V6]**

**Files:** `Forms\FrmChartPreview.vb` (code-built UI — **no Designer file**), `.resx` registered in `TempleAccounting.vbproj`:
```xml
<Compile Include="Forms\FrmChartPreview.vb">
  <SubType>Form</SubType>
</Compile>
<EmbeddedResource Include="Forms\FrmChartPreview.resx" />
```

**Purpose (class doc):** dedicated full-screen interactive chart preview with print (A4 landscape) and PNG/JPEG export. Launched modal from `FrmReports.btnShowChart` — keeps the underlying report grid clean.

**Constructor & window (final state after 3 refinement passes):**
- `WindowState = Normal`, `StartPosition = CenterScreen`, `FormBorderStyle = Sizable`, `MaximizeBox/MinimizeBox = True` (task #8 removed full-screen Maximized; task #9 made it resizable).
- `Width = 1200`, `Height = 750`; `Font = Tahoma 10`; `BackColor = (254,249,235)` cream; **`AutoScaleMode = Dpi`**; `KeyPreview = True`.

**UI structure (`BuildUI`):**
- `panelTop` — `Dock=Top`, `Height=45`, `Padding(12,6,12,6)`, header color `#34495E` (`Color.FromArgb(52,73,94)`).
- 4 toolbar buttons (`Dock=Right`, width-overridden, `CreateToolButton` = Flat, `FlatAppearance.BorderSize=0` set **after** object initializer — VB can't set `FlatAppearance` in an initializer):
  - `btnClose` "❌ ปิดหน้าต่าง" (140px, `#E74C3C`)
  - `btnToggleTotals` "📊 ซ่อน/แสดง ยอดรวม" (170px, `#8E44AD`) **[NEW task #12]**
  - `btnSaveImage` "💾 บันทึกรูปภาพ" (160px, `#27AE60`)
  - `btnPrintChart` "🖨️ พิมพ์กราฟ" (150px, `#2980B9`)
- `chartPreview` — `Dock=Fill`, `BackColor=White`, **`Padding = New Padding(12,12,12,12)`** (0.3-cm-style inset), `BorderSkin.None`, `chartPreview.BringToFront()`.

**`LoadChart(source As Chart)` — deep clone pipeline (tasks #5–#7):**
1. Clears `DataSource/Titles/Annotations/Series/ChartAreas/Legends`, `Palette = None`.
2. **ChartArea clone:** X/Y axis interval, label fonts (`CloneFont` — Font is MarshalByRefObject, `.Clone()` used), grids, titles; **zoom/pan enabled both axes** (`AxisX/Y.ScaleView.Zoomable=True`, `ScrollBar.Enabled=True`, `CursorX/Y.IsUserSelectionEnabled=True`). **`dstArea.Position.Auto = True` + `InnerPlotPosition.Auto = True`** — the blank-screen fix: manual `Position.Y` overrides were removed entirely (task #7).
3. **Legend clone:** `Docking=Top`, `Alignment=Near`, `IsDockedInsideChartArea=False`, `BackColor=Transparent` — fixes clipped legend (task #6).
4. **Series clone:** ChartType/ChartArea/Color/BorderWidth/IsValueShownAsLabel/LabelFormat/Font, `PointWidth` custom prop, SmartLabelStyle, and per-point `DataPoint(XValue, YValues)` with `AxisLabel`, per-point color override (keeps bold "รวม" bar colors).
5. **Titles clone** (Docking/Font/ForeColor).
6. Mouse handlers: `MouseWheel` → Y-axis zoom (0.9/1.1 factor, full-range clamp, `ZoomReset` when reaching full); `MouseDoubleClick` → zoom reset; `MouseEnter` → `chartPreview.Focus()`.
7. Ends with `ChartAreas(0).RecalculateAxesScale()` + `chartPreview.Update()`.

**Print (task #5):** `btnPrintChart_Click` → `RenderChartToBitmap()` (min 1200×800, `SetResolution(96,96)`, `DrawToBitmap`) → `PrintPreviewDialog` (Maximized, Rows=1, Columns=1, AutoZoom, `_doc` PrintPage = `PrintChartPage`) → draws bitmap min-ratio scaled to `e.MarginBounds` (A4 landscape), centered, `InterpolationMode.HighQualityBicubic`, `e.HasMorePages = False`. Bitmap disposed in `Finally`.

**Save image:** `btnSaveImage_Click` → `SaveFileDialog` (`PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg`, default `กราฟสรุปรายเดือน.png`) → `RenderChartToBitmap().Save(path, format)`.

**Toggle Totals (task #12):** `ToggleTotalBars()` identifies "รวม" bars by `DataPoint.AxisLabel` containing "รวม" (matches the `AxisLabel="รวม"` convention injected by `FrmReports` when appending total bars). Hide = store `SeriesName|PointIndex → Color` in `_totalPointColors`, set `Color = Transparent` + `IsEmpty = True`; Show = restore + `IsEmpty = False`. Either way resets `AxisY.Minimum/Maximum = Double.NaN` + `RecalculateAxesScale()` so the Y-axis rescales when totals are hidden (task #12 requirement). Button text/color toggles between `"📊 ซ่อน/แสดง ยอดรวม"` (#8E44AD) and `"📊 แสดง ยอดรวม"` (#641E82).

**F1 Help:** `FrmChartPreview_KeyDown Handles Me.KeyDown` → `HelpSystem.ShowManual("", Me)` — no dedicated manual section; opens the raw `USER_MANUAL_TH.md` (empty-formName path).

**Dispose:** `_chartBitmap` disposed with the form.

### 2.2 ENHANCED — PDF Export Flow: Preview-first + Toolbar "Save as PDF" Button **[feature: V6]**

`btnSavePdf` on `FrmReports` no longer jumps straight to a save dialog. New flow:

```
btnSavePdf_Click (FrmReports)
  └─ IncomeExpenseReport.ShowPreviewWithPdfExport(fromDate, toDate, Me, _lastReportMode, GetManualBalance(), fundID?, bankID?)
       ├─ new IncomeExpenseReport(...)  (same args as ShowPreview → same report the user last viewed)
       ├─ ApplySessionPrinter(doc)      (session printer memory)
       ├─ PrintPreviewDialog (Maximized, 1200×800, Rows/Columns=1, AutoZoom)
       ├─ AddPdfButtonToPreviewToolbar(ppd, ...)
       │    ├─ locate ToolStrip among ppd.Controls (TryCast)
       │    ├─ add ToolStripSeparator + ToolStripButton "💾 บันทึกเป็น PDF" (ImageAndText, Thai ToolTip)
       │    └─ AddHandler Click → closure → ShowSavePdfDialog(same args)   (Try/Catch: preview still works if button injection fails)
       └─ ppd.ShowDialog(owner)
```

- `ShowSavePdfDialog(...)` unchanged from V5: `SaveFileDialog` (`ไฟล์ PDF|*.pdf`, default `รายงาน_yyyyMMdd_yyyyMMdd.pdf`, OverwritePrompt) → `SaveToPdf` via **Microsoft Print to PDF** virtual printer → Thai success/error message.
- Tooltip on `btnSavePdf` updated: "เปิด Print Preview ให้ตรวจดูก่อน แล้วกดปุ่ม 💾 บันทึกเป็น PDF บน Toolbar".
- New session printer memory (`_selectedPrinterName` module-level `Shared`): `ShowPrint` remembers the chosen printer; `ShowPreview`/`ShowPreviewWithPdfExport`/`ShowSavePdfDialog` auto-apply it via `ApplySessionPrinter`; missing printer resets the memory.

### 2.3 ENHANCED — F1 Help Manual: Multi-Machine Sections Now Available **[feature: V6, closes V5 §7.1 #13]**

`USER_MANUAL_TH.md` grew from 10 → **12 sections** (V5 documented the missing-sections gap as an open issue; both sections now exist):

| # | Section Header | Form |
|---|---|---|
| 1 | หน้าจอรายการรับ-จ่ายทั้งหมด | FrmTransactions |
| 2 | หน้าจอรายงาน | FrmReports |
| 3 | หน้าจอโอนเงินภายใน | FrmTransfer |
| 4 | หน้าจอจัดการรายชื่อบุคลากร | FrmPersonnelManagement |
| 5 | หน้าจอตั้งค่าข้อมูลวัด | FrmTempleSetting |
| 6 | หน้าจอบันทึกรายรับ | FrmIncome |
| 7 | หน้าจอบันทึกรายจ่าย | FrmExpense |
| 8 | หน้าจอจัดการข้อมูลหลัก | FrmMasterData |
| 9 | หน้าจอนำเข้าข้อมูลที่อยู่ | FrmLocationImport |
| **10** | **หน้าจอส่งออกข้อมูลไปเครื่องอื่น** | **FrmExportForTransfer** |
| **11** | **หน้าจอนำเข้าข้อมูลหลายเครื่อง** | **FrmMultiMachineImport** |
| 12 | หน้าจอหลัก | FrmMain |

- **Section 10 (FrmExportForTransfer):** setting the MachineID once per machine (refuses export until set), export scopes (เฉพาะรายการใหม่ / กำหนดช่วงวันที่ / เลือกข้อมูลทั้งหมด), export folder structure `Export_<รหัสเครื่อง>_<วันเวลา>\` with `TransactionData.xlsx` + `Receipts\`, and the "เปิดโฟลเดอร์ส่งออก" step.
- **Section 11 (FrmMultiMachineImport):** multi-file selection (.xlsx/.xlsm/.xlsb/.xls), embedded MachineID + SHA-256 vs `ImportHistory` re-import blocking, duplicate statuses (ซ้ำแน่นอน by TransactionGUID / สงสัยซ้ำ by composite key), one OLEDB transaction with pre-merge auto-backup, receipt merge, and log review.
- Both sections match `FrmHelpDialog.ExtractSection` regex (`## N. ชื่อ (FormName)` … `---`).

### 2.4 ENHANCED — HelpSystem: Custom Status-Bar Hints **[feature: V6]**

- `SetupHelp(frm, Optional sectionName = "", Optional hintText = Nothing)` — `hintText` param lets a form override the default hint `"💡 คำแนะนำ: กดปุ่ม [F1] เพื่อดูวิธีใช้งานหน้าจอนี้"`. `AddStatusBarHint` now re-uses an existing `lblHelpHint` ToolStripStatusLabel (updates text) instead of always adding a duplicate.
- F1 integration contract unchanged: `KeyPreview=True` via `SetupHelp`, per-form `Handles Me.KeyDown` → `ShowManual("<FormName>", Me)` with `e.Handled/SuppressKeyPress=True` (no central anonymous AddHandler — lesson learned). Verified present on `FrmReports`, `FrmTransactions`, `FrmIncome/Expense/Transfer`, `FrmTempleSetting`, `FrmMasterData`, `FrmLocationImport`, `FrmPersonnelManagement`, `FrmMultiMachineImport`, `FrmExportForTransfer`, and (empty-formName variant) `FrmChartPreview`.

### 2.5 ENHANCED — Report Data Service Centralized in `ReportEngine` **[refactor: V6]**

`IncomeExpenseReport` no longer embeds query logic in its renderer. `ReportEngine.vb` (module) now owns the full data path (commit `25a230a` "Refactor report data logic into ReportEngine module"):

| Member | Signature | Purpose |
|---|---|---|
| `BuildReportDataset` | `Function(fromDate, toDate, Optional summaryOnly, Optional manualOpeningBalance, Optional fundID, Optional bankID) As ReportDataset` | Entry point: ensures schema, loads `GetTemplateInfo()`, computes opening balance (manual override or `GetBalanceBeforeDate`), builds income+expense row lists, **inserts the "ยอดยกมา" carry-forward row as row 0** of income, computes `GrandTotal = OpeningBalance + TotalIncome` and `Balance = GrandTotal - TotalExpense` |
| `GetBalanceBeforeDate` | `Private Function(conn, beforeDate, fundID?, bankID?) As Decimal` | `SUM(IIF(TranType='Income',Amount,-Amount)) WHERE DateSerial(...) < #literal#` (+ fund/bank filters) |
| `BuildRowList` | `Private Sub(conn, tranType, fromDate, toDate, fundID?, bankID?, summaryOnly, targetRows, ByRef runningTotal)` | Detailed mode = GROUP BY day+category; Summary mode = GROUP BY month-end+category; `DateSerial` + Buddhist→Gregorian normalization per system hard rule |

**New shared data classes (in ReportEngine.vb):** `ReportRow` (TranDate, DayCode, Description, Amount, IsCarryForward, IsCategorySummary), `ReportDataset` (IncomeRows, ExpenseRows, TotalIncome, TotalExpense, OpeningBalance, GrandTotal, Balance, TemplateInfo). This is the "centralized data structure over repeated queries" pattern the user prefers.

---

## 3. Layout & Printing Engine Overhauls (V5 → V6 delta)

### 3.1 Signature Block — Anchored to Absolute Page Bottom **[task: #1, #2, #11]**

**`ReportEngine.DrawSignatureBlock` (core 6-parameter geometry, unchanged constants):**
```
labelHeight = 24  |  labelToSignLine = 52  |  signLineToName = 14  |  nameHeight = 26  |  nameToPosition = 8  |  positionHeight = 24
totalSignatureHeight = 24+52+14+26+8+24 = 148 px
```

**Bottom-anchoring formula (final state after tasks 1→2→11):**
```vb
Const bottomGapPx As Integer = 4    ' 0.1 ซม. กล่อง→ขอบ + ชดเชย ~4px = glyph ล่างสุด ≈8px ≈ 0.2 ซม.
Dim posCenterOffset As Integer = CInt((positionHeight - safeRowFont.Height) / 2)
Dim blockBottom = pageBottom - bottomGapPx - posCenterOffset
Dim minimumTop = currentY + 4      ' เก็บไว้เท่านั้น — safety push-up ถูกลบ (task #11)
```
- **Task #1:** shifted baseline down so the bottom-most glyph line (ตำแหน่ง: เจ้าอาวาส / ไวยาวัจกร) sits ~0.5 cm above the printable bottom — internal spacing preserved.
- **Task #2:** tightened to a 0.3 cm bottom gap (~11–12 px @96 DPI).
- **Task #11 (final):** **bypassed all safety margins/push-up rules** — the old `blockBottom = minimumTop + totalSignatureHeight + 8` guard is deleted; `blockBottom` is always derived from `pageBottom`, even overlapping the data area. The renderer (`IncomeExpenseReport.OnPrintPage`) decides whether to start a new page. Bottom clearance = `bottomGapPx(4)` + font-centering offset ≈ **8 px ≈ 0.2 cm** glyph-to-edge.

**Shared Y coordinates** (both blocks perfectly aligned): `signatureLabelY = blockBottom − 148`; `signatureLineY = +24+52`; `signatureNameY = +14`; `signaturePositionY = +26+8`. Blocks centered in their half: `leftBlockX = leftX + (blockWidth − 280)/2`.

**Name rendering (final):** single dotted line (no double); name shown as `( {ชื่อจริง} )` from `GetTemplateInfo()`; the dotted placeholder `( .................................... )` is used **only** when the bound name is empty (never hardcoded over a real name). Names rendered with `CreateFittedFont` (Tahoma 12 bold, min 10, shrink-to-fit `blockWidth − 8`).

**Name-bound convenience overload (V6):** `DrawSignatureBlock(g, leftTitle, leftPosition, rightTitle, rightPosition, leftX, rightX, blockWidth, pageBottom, currentY, boldFont, rowFont)` internally calls `GetTemplateInfo()` for `AbbotName`/`AccountantName` — the renderer no longer passes names.

### 3.2 `IncomeExpenseReport` — Final-Page Reservation & Dynamic Page-Row Balancing **[task: #13, feature: V6]**

**New constants:**
```vb
Private Const FinalSummaryRows As Integer = 3
Private Const FinalSignatureBlockHeight As Integer = 148   ' = ReportEngine.totalSignatureHeight
Private Const FinalFooterGapHeight As Integer = 8          ' 4px gap + กึ่งกลางชดเชย ≈ glyph 8px (0.2 ซม.)
Private Const SectionGap As Integer = 12
```

**`CalculateFinalContentHeight(rowH)`** — height that MUST fit on the last data page:
```
summaryHeight = (3 × rowH) + 4    →  88 px  (@rowH=28)
signatureHeight = 148
footerGap = 8
topGap = 4
TOTAL = 88 + 148 + 8 + 4 = 248 px
```

**`PreCalculatePageBreaks(rowH, pageBottom)`** — the orphan-page-prevention algorithm (runs once, after the first `DrawHeader` sets `_estimatedHeaderHeight`):
1. `totalRows = Max(IncomeRows.Count, ExpenseRows.Count)`; `totalRows = 0` → single page, no breaks.
2. Space accounting per page (excluding header): `tableHeaderH = rowH × 2`, `midSummaryH = rowH`, `finalReserveH = CalculateFinalContentHeight(rowH)`; `usableH = pageBottom − (50 + headerH)`.
3. `maxRowsNonLast = ⌊(usableH − tableHeaderH − midSummaryH) / rowH⌋`; `maxRowsLast = ⌊(usableH − tableHeaderH − finalReserveH) / rowH⌋`.
4. **Natural fill simulation:** repeatedly take `Min(maxRowsNonLast, remaining − maxRowsLast)` rows per page until `remaining ≤ maxRowsLast`.
5. **Orphan detection:** `remaining <= 0` (final page would hold zero data rows) → **redistribute**: `maxRowsNonLast = Max(1, maxRowsNonLast − 3)` and re-simulate, pulling ~5–6 rows into the last page.
6. Stores `_pageBreakIndices` (cumulative `_rowIndex` break points) + `_totalPages = breaks.Count + 1`; extensive `[PAGE BALANCING]` Debug.WriteLine tracing.

**OnPrintPage enforcement:**
```vb
' หลัง DrawHeader หน้าแรก:
Dim rowH = 28
If Not _preCalcDone Then
    _estimatedHeaderHeight = _pageY
    PreCalculatePageBreaks(rowH, _pageBottom)
End If

' ใน While loop ก่อนตรวจพื้นที่:
Dim forcePageBreak As Boolean = False
If _pageBreakIndices IsNot Nothing AndAlso _pageBreakIndices.Count > 0 AndAlso
   _pageIndex < _pageBreakIndices.Count Then
    If _rowIndex >= _pageBreakIndices(_pageIndex) Then forcePageBreak = True
End If
If _pageY + currentRowH > _pageBottom OrElse forcePageBreak Then
    e.HasMorePages = True : _pageIndex += 1 : Return
End If
```
- Fallback: no break indices (tiny-page guard) → plain fill-to-bottom as before.
- Final-page guard retained: after the last row, if `availableSpace < CalculateFinalContentHeight(rowH)` → new signature-only page; else `DrawSummaries` + `DrawSignatures` on the same page. This prevents both orphan *summary* pages and signature/footer overflow at the new 0.2 cm bottom clearance.

### 3.3 Dynamic Row Height (unchanged baseline, re-verified)

Each row height = `Max(rowH, Max(MeasureTextHeight(leftDesc), MeasureTextHeight(rightDesc)))` measured against `colW3 − 6` — long Thai descriptions auto-grow the row; both sides stay aligned.

### 3.4 Print Flow Surface (V6 consolidated API — `IncomeExpenseReport` shared statics)

| Member | Behavior |
|---|---|
| `ShowPreview(from, to, owner?, mode?, manualOpeningBalance?, fundID?, bankID?)` | PrintPreviewDialog only (V5) |
| **`ShowPreviewWithPdfExport(...)`** | **V6 new** — preview + injected "💾 บันทึกเป็น PDF" toolbar button |
| `ShowPrint(from, to, owner?, mode?, ...)` | Direct `PrintDialog` (`UseEXDialog`, all printers incl. USB/Network/PDF/Thermal); remembers printer session-wide |
| `ShowSavePdfDialog(...)` | SaveFileDialog → `SaveToPdf` (Microsoft Print to PDF) |
| `SaveToPdf(filePath)` | Virtual-printer PDF; Thai error with enable instructions if printer missing |

All print surfaces share: A4 (`CreateA4Paper`), `UseLandscape=True`, safe margins `Margins(50,50,50,50)` on `DefaultPageSettings` + `PrinterSettings.DefaultPageSettings` + `OnQueryPageSettings`, proportional columns (`16/10/50/24%` of `MarginBounds.Width`), Tahoma-only fonts via `ReportTheme`.

---

## 4. UI/UX & Accessibility Fixes (V5 → V6 delta)

### 4.1 FrmReports Toolbar — FlowLayoutPanel + Autosize Buttons **[task: #3]**

Fixed cramped/overlapping/truncated report nav buttons (btnPrintDetail, btnPrintSummary, btnSummaryIncome, btnSummaryExpense, btnShowChart, btnMonthly, btnLedger, btnSavePdf, btnPrintReport, btnPrint):

| Property | Value |
|---|---|
| Container | `pa` FlowLayoutPanel — `AutoSize=True`, `AutoSizeMode=GrowAndShrink`, `Dock=Top`, `Location (0,90)`, `Padding(8,6,8,6)`, BackColor `#F5F0DC` |
| Every button | `AutoSize=True` + `AutoSizeMode=GrowAndShrink`, `Padding = New Padding(12,4,12,4)`, `Margin = New Padding(3,2,3,2)`, Tahoma 9 bold, `FlatAppearance.BorderSize=0`, `Cursor=Hand` |
| Wrapping | FlowLayoutPanel default `WrapContents=True` (inherited — not explicitly set) |

Designer `Size` values (e.g. `btnShowChart 221×40`, `btnSavePdf 159×40`) remain as designer fallbacks; at runtime AutoSize recomputes from text. **V6 caution:** the `pa` panel's `Size` is a designer snapshot — since `AutoSizeMode=GrowAndShrink` + `Dock=Top`, runtime height follows content; ensure a `MinimumSize` is not needed for narrow windows (see §5.3 roadmap).

### 4.2 FrmChartPreview — Iterative UX Fixes (task #5 → #10) **[all V6]**

| Task | Issue | Fix |
|---|---|---|
| #5 | (new form) | Full-screen design + print + save-image; clone-from-source architecture |
| #6 | Legend clipped top-left | `Docking.Top` + `Alignment.Near` + `IsDockedInsideChartArea=False` + `BackColor.Transparent` |
| #7 | Blank screen | Manual `ChartArea.Position/InnerPlotPosition` overrides removed → `Auto=True` both; `chartPreview.Padding(12,12,12,12)`; `BringToFront()` |
| #8 | Full-screen rejected | `WindowState=Normal`, manual 1200×750, 0.3 cm-style inset padding |
| #9 | Resizable requirement | `FormBorderStyle=Sizable`, `MaximizeBox/MinimizeBox=True`, panelTop fixed Height=45, chart Dock=Fill, zoom/pan preserved, AutoLayout |
| #10 | Hardcoded pixel layout cleanup | standard WinForms responsive layout: `panelTop Height=45`, Dock Fill, `AutoScaleMode.Dpi`, `chartPreview.BringToFront()` |
| #12 | Toggle totals | New 4th toolbar button; Y-axis auto-rescale on hide/show (§2.1) |

### 4.3 Accessibility & Input (unchanged V5 baseline, re-verified in V6)

- F1 contextual help on every primary form (status-bar hint + centered `FrmHelpDialog`).
- `UiFitter.AutoFitFormButtons` (Thai vowel/mark-safe) + `DisableComboBoxWheel` on all ComboBox-bearing forms.
- Enter-key field navigation on all entry forms; report filter Enter chain ends at `btnRefresh`.
- Chart mouse-wheel zoom + double-click reset in both `chartMonthly` (FrmReports) and `chartPreview` (FrmChartPreview).
- All new report/print surfaces use Thai messages and Tahoma fonts.

---

## 5. Branch & Environment Status

### 5.1 Git Repository State (audited 2026-08-09)

| Item | Value |
|---|---|
| Active branch | **`perfect-point-v0`** (task #14) |
| Working tree | Clean |
| Baseline commit | `e867b6f` "Add chart totals toggle & improve report pagination/layout" (2026-08-09) |
| Branch parent | `stable-report-layout-v1` @ `e867b6f` — `perfect-point-v0` was created from this stable point and tracks it |
| Ahead of origin | `stable-report-layout-v1` is 29 commits ahead of `origin/stable-report-layout-v1` |
| Local branches | `main`, `stable-report-layout-v1` *, `perfect-point-v0` *, `modify-frmReports`, `modify-frmTransaction`, `modify_Backup`, `modify_FrmTempleSetting`, `backup/temple-setting-safe`, `recovery-branch`, `restore-aug2-1534pm`, `restore-aug2-15pm` (* = relevant current work) |
| Remote | `origin` — `main`, `modify-frmReports`, `modify-frmTransaction`, `modify_Backup`, `modify_FrmTempleSetting`, `recovery-branch`, `stable-report-layout-v1` |

**Commit timeline (most recent 30, top = HEAD `e867b6f`):**

| Commit | Subject |
|---|---|
| e867b6f | Add chart totals toggle & improve report pagination/layout |
| 6414cf9 | Add InitializeComponent to FrmChartPreview and reformat resx |
| 9dcdde1 | Set FrmChartPreview to fixed size and add resource file |
| a749fa9 | Improve Chart layout and auto-scale rendering |
| a652ba2 | Improve legend positioning and ChartArea spacing |
| 4471912 | Add FrmChartPreview for full-screen chart preview |
| b3f0f97 | Refactor report buttons and update FrmReports layout |
| cffae35 | Reduce bottom gap for signature/footer in report layout |
| 9209b16 | Improve PDF export workflow and UI feedback |
| b1574f9 | Refactor toolbar to use FlowLayoutPanel and autosizing |
| ccf4b67 | Improve signature block positioning in report layout |
| be6faab | Align footer and signature margins to layout box |
| ca8206d | Allow custom status bar hints per form in HelpSystem |
| 468d485 | Align signature block bottom margin to 19px standard |
| 856b78b | Refactor signature block layout and data binding |
| 25a230a | Refactor report data logic into ReportEngine module |
| e65930b | Refactor signature block rendering and name formatting |
| 37453e8 | Improve FormatSignerName formatting and placeholder logic |
| b23d60f | Refactor signature block rendering in report |
| 4fed94d | Improve signature block layout and personnel name logic |
| b2b84bf | Streamline report print/preview workflow and remember printer |
| 30a1e3b | (placeholder commit) |
| 6d53aee | Add PROJECT_PROFILE_V5.md with full system audit and changelog |
| 6530b66 | Remove FrmMultiMachineImport.Designer.resx resource |
| 72c0cd1 | Add "Save as PDF" to reports and improve form layouts |
| 801c535 | Localize UI and messages to clearer Thai terminology |
| f73a861 | Add receipt attachment handling to export/import flow |
| 5312a5e | Add export/import feature for cross-machine transactions |
| 620315b | Add multi-machine import form and integration |

**`USER_MANUAL_TH.md` history:** `ca8206d` (2026-08-08, custom hints) / `965e9bf` (2026-08-07, "Add and expand Thai user manual; update project config" — added sections 10–11) / `5860704` / `658dd89` (help dialog modernize + HelpSystem intro).

### 5.2 Environment

| Item | Value |
|---|---|
| OS | Windows (PowerShell; timezone Asia/Bangkok) |
| Build | `dotnet build TempleAccounting.vbproj -c Debug` / `-c Release` → 0/0 |
| Runtime deps | .NET 10 preview SDK 10.0.400-preview.0.26322.102; 64-bit Microsoft Access Database Engine 2016 (OLEDB); `Microsoft Print to PDF` virtual printer (PDF export) |
| Installer | Inno Setup `TempleAccountingSetup.iss` (v1.0.0, publisher วัดดอนเมือง) |
| DB | `Database\TempleAccounting.accdb` (ACE OLEDB 12.0), schema self-healing via `Db.EnsureSchema()` |
| Help doc | `USER_MANUAL_TH.md` copied to output (see `.vbproj` comment) |

### 5.3 Outstanding Items Carried From V5 (still open at V6)

1. `FrmTransactions` in-grid edit path (`SaveSelectedRowEdits`) remains dead code — Transfer-row editing has no working save.
2. Two corrupted `�` glyphs remain on `FrmTransactions` receipt-retrofit buttons.
3. `FrmTransactions` debug-report harness (`http://127.0.0.1:7777/event`) still wired in.
4. `FrmLocationImport` CSV parser remains naive (no quoted-field handling).
5. `FrmPersonnelManagement` `fullName` title gluing (no space); Positions delete lacks friendly FK-in-use message.
6. `LoadMonkSample()` in frmMain still reads legacy TempleSetting columns (guarded only by `IsDBNull`).
7. `BaseReport` remains dead scaffolding; `AutoScaleMode` still mixed across forms.
8. **V6 new observation:** `FrmReports.pa` FlowLayoutPanel has a designer `Size` fallback; with `GrowAndShrink` + 10 autosize buttons the bar may need a `MinimumSize`/`AutoScroll` guard for very narrow host widths. Also `FrmChartPreview` is opened via `ShowDialog` (modal) while other previews are panel-hosted — deliberate per design, but worth confirming at deployment.

---

*End of PROJECT_PROFILE_V6.md — deep audit of the V5→V6 delta: new `FrmChartPreview` (full-screen chart + print + save + toggle-totals), FlowLayoutPanel report toolbar, bottom-anchored signature block, dynamic page-row balancing, preview-first PDF export with toolbar save button, F1 manual sections 10–11, HelpSystem custom hints, centralized `ReportEngine` data service, and the `perfect-point-v0` branch state. `PROJECT_PROFILE_V5.md` (and earlier profile documents) and all application source files remain unmodified.*
