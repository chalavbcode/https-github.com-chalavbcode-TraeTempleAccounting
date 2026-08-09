# System_Stability_and_Robustness_Audit.md

**ระบบบัญชีวัด — Temple Accounting System**
**Audit type:** In-depth code stability, exception handling & architectural audit
**Based on:** PROJECT_PROFILE_V4 / V5 / V6 + full source re-verification of `Database.vb`, `Program.vb`, `AppPaths.vb`, `ReportEngine.vb`, `IncomeExpenseReport.vb`, `FrmChartPreview.vb`, `FrmReports.vb`, `FrmMultiMachineImport.vb`, `FrmExportForTransfer.vb`, `TransferFormat.vb`
**Date:** 2026-08-09
**Build status at audit time:** Debug & Release 0 warning / 0 error

---

## 0. Executive Summary

The codebase already has a solid defensive baseline:

- **Global exception handlers exist** in [Program.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Program.vb#L56-L83) (`AppDomain.UnhandledException` + `Application.ThreadException`) with crash-log writing to `Logs\crash.log`.
- **All OleDb access is short-lived `Using` blocks** — no leaked connections or readers were found.
- **Multi-machine import already implements** SHA-256 re-import blocking, schema-version warnings, row-count integrity checks, and a full OLEDB transaction with rollback + auto-backup — well above average.

However, the audit identified **17 concrete failure points** in 4 areas, the most serious being:

1. `Db.EnsureSchema()` marks `_schemaChecked = True` **before** doing the work — a mid-way failure permanently disables schema repair for the rest of the session.
2. **GDI+ object leak in the report engine** — `ReportTheme` (9 fonts + 2 pens) is created per report but never disposed (no `Dispose` override in `IncomeExpenseReport`).
3. **Potential infinite print loop** if a single data row is taller than the page (no row-height cap).
4. **Unhandled exception path** in `FrmExportForTransfer` (folder creation outside the Try block).
5. No OLEDB connection retry / lock-wait policy for a network-drive or Office-locked `.accdb`.
6. `Application.DoEvents()` inside long DB loops (merge/import) → re-entrancy risk on the UI thread.

Full details, per-module fixes, and a prioritized roadmap follow.

---

## 1. Identified Vulnerabilities & Crash Risks

### 1.1 Database & File I/O Resilience (`Database.vb`, `AppPaths.vb`, `Program.vb`)

| # | Severity | Location | Risk / Failure Scenario |
|---|---|---|---|
| **DB-1** | **HIGH** | [Database.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Database.vb#L27-L33) — `EnsureSchema()` sets `_schemaChecked = True` at line 29, **before** the `File.Exists` check (line 31) and all migrations | If any step throws (missing driver, locked file, mid-migration failure), the flag stays `True` forever. Every subsequent `EnsureSchema()` call returns immediately without attempting repair, and every `OpenConn()` in the app throws an unhelpful OleDbException. The app degrades into a state where **schema self-healing is permanently disabled for the session**. |
| **DB-2** | HIGH | [Database.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Database.vb#L21-L25) — `OpenConn()` has no connection timeout, retry, or lock-wait strategy | If the `.accdb` is locked by another process (a second app instance, Access, or a stale `.laccdb` lock on a network drive), `conn.Open()` throws immediately. There is no retry and no friendly "ไฟล์ถูกใช้งานอยู่" message — the exception surfaces via the generic global handler. |
| **DB-3** | MEDIUM | [Database.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Database.vb#L384-L392) — `BackupDatabase()` uses `File.Copy` with no retry / no read-lock handling | If the source file is mid-write or locked, the copy throws. In `FrmMultiMachineImport.RunMerge` this is called before the transaction; a backup failure aborts the merge but shows the misleading message "ระบบได้ยกเลิกการบันทึกข้อมูลทั้งหมดแล้ว" even though **nothing was written**. |
| **DB-4** | MEDIUM | [Database.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Database.vb#L316-L336) — `ExecuteNonQuery` / `DbScalar` / `GetTable` have **no internal Try/Catch** | Correct design (callers handle), but many form-level calls rely only on the global ThreadException handler → users get generic crash dialogs instead of contextual error messages. Audit of callers shows inconsistent handling: some catch (e.g. `FrmReports_Load` rethrows), some don't. |
| **DB-5** | MEDIUM | [Program.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Program.vb#L110-L124) — Boot Step 4: `EnsureSchema()` failure shows a warning but **the app continues to open frmMain** | The app starts in a broken state (no DB / no provider). Every form load then fails. Better to block startup or show a retry/exit choice. |
| **DB-6** | MEDIUM | [AppPaths.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/AppPaths.vb#L22-L31) — `LogsFolder` getter creates the directory; logging is best-effort `Catch`-swallowed | Acceptable for logging, but if `AppRoot` is read-only (Program Files deployment), every `Log*` silently fails and no diagnostics exist — acceptable today since the installer uses a writable folder, but should be documented. |
| **DB-7** | LOW | `Program.vb` — no `TaskScheduler.UnobservedTaskException` handler | No `Task.Run`/`Async` usage found in the current codebase, so low priority — add the handler only if async is introduced. |

### 1.2 Report Engine & Printing Engine Edge Cases (`ReportEngine.vb`, `IncomeExpenseReport.vb`)

| # | Severity | Location | Risk / Failure Scenario |
|---|---|---|---|
| **RP-1** | **HIGH** | [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb#L137-L139) — `_theme = ReportTheme.CreateDefault()` in `InitFonts`; **no `Dispose` override anywhere in the class** | Each `New IncomeExpenseReport(...)` allocates 9 `Font` + 2 `Pen` GDI+ objects that are never disposed. Repeated previews/prints (the app's primary workflow) leak GDI+ handles → after enough cycles the print preview can fail with `OutOfMemoryException` or render garbage. **Confirmed by grep: no `Overrides Sub Dispose` in the file.** |
| **RP-2** | HIGH | [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb#L291-L323) — OnPrintPage row loop has **no cap on `currentRowH`** | `currentRowH = Max(rowH, Max(leftH, rightH))` is unbounded. If a description renders taller than the page (extreme content), the `If _pageY + currentRowH > _pageBottom` branch always fires → `HasMorePages = True` forever → **infinite print loop / hung print job**. Low probability, high damage. |
| **RP-3** | MEDIUM | [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb#L430-L431) — `PreCalculatePageBreaks`: `headerH = _estimatedHeaderHeight - 50` and `usableH = pageBottom - (50 + headerH)` with no lower bound | If the measured header is smaller than 50px (unusual but possible with a narrow/odd margin config), `headerH` is negative → `usableH` is overestimated → the pre-calculated `maxRowsNonLast` is too large → rows can crowd the signature area. There is a fallback guard for tiny pages but not for negative header. |
| **RP-4** | MEDIUM | [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb#L526-L562) — `DrawMidSummary` loops `For i = 0 To Math.Min(_rowIndex, _incomeRows.Count) - 1` | Safe today (VB `For` with end < start runs zero times) — noted only as a guard to keep in mind during future refactors. |
| **RP-5** | MEDIUM | `ReportTheme` fonts are **also** shared via `BaseReport.vb` (dead scaffolding) and `LayoutConfig.CreateA4Landscape` hardcodes margins `(50,50,50,50)` while `_pageMargin = 40` is declared but unused | Inconsistent constants are a future maintenance hazard; the dead `BaseReport` keeps a second copy of the font-creation logic that is never disposed. |
| **RP-6** | MEDIUM | Printer failure paths are mostly wrapped, but **`new IncomeExpenseReport(...)` runs `ConfigurePageSettings()` in the constructor**, which touches `Me.PrinterSettings.PaperSizes` | With **no printer installed**, `PrinterSettings` can throw `InvalidPrinterException` inside the constructor — before any of the `Show*` wrappers' Try/Catch can isolate it to a friendly message (the wrappers do catch it, but the message is generic "ไม่สามารถเปิดดูรายงานได้"). Consider a dedicated check. |
| **RP-7** | LOW | [FrmChartPreview.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmChartPreview.vb#L479-L488) — `RenderChartToBitmap` uses `chartPreview.Width/Height` (logical pixels) then `SetResolution(96,96)` | On a 150–200% DPI display the printed/saved chart is lower-resolution than the screen → blurry output. Should render at `Width * ScaleFactor`. |
| **RP-8** | LOW | [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb#L757-L759) — `_selectedPrinterName` is a module-level `Shared` | Session-memory is fine, but it survives across all report instances and is reset on missing printer — no crash risk, just behavior note (a printer removed then re-added mid-session reverts to default). |

### 1.3 UI Threading & High-DPI Scaling (`FrmChartPreview.vb`, `FrmReports.vb`, `UiFitter.vb`)

| # | Severity | Location | Risk / Failure Scenario |
|---|---|---|---|
| **UI-1** | **HIGH** | [FrmMultiMachineImport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmMultiMachineImport.vb#L1289-L1308) — `Log()` and `SetProgress()` call `Application.DoEvents()` **inside long DB merge/analysis loops** (also `FrmLocationImport`, `FrmExportForTransfer`) | During `RunMerge` (a transactional loop), `DoEvents()` re-enters the message pump → the user can click other buttons (`btnClose`, `btnSelectFiles`, `btnClearFiles`) while a transaction is open → **re-entrant DB access / double transaction risk**. Only `btnConfirmMerge` is disabled. |
| **UI-2** | MEDIUM | [FrmChartPreview.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmChartPreview.vb#L38-L53) — constructor: resizable form but **no `MinimumSize`** | The 4 fixed-width toolbar buttons (`140+170+160+150`px) plus padding ≈ 620px can be clipped horizontally if the user shrinks the window below ~650px; the chart then also gets squeezed. Add `MinimumSize = New Size(800, 500)`. |
| **UI-3** | MEDIUM | `FrmChartPreview` — `AutoScaleMode.Dpi` on a code-built form with hardcoded button `Height=36` / `Width` | Buttons do not scale on high-DPI displays (unlike designer forms which scale via AutoScaleMode). Visual inconsistency; on 200% displays buttons may render small. |
| **UI-4** | MEDIUM | [FrmReports.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmReports.vb#L364-L367) — `frm.LoadChart(chartMonthly)` clones the live chart | Currently single-threaded so no race; but `LoadChart` re-adds `MouseWheel`/`MouseDoubleClick` handlers via `AddHandler` — if `LoadChart` were ever called twice on the same instance, handlers would stack and zoom would double-apply. Not triggered today (each `FrmChartPreview` is a new instance) — documented as a guard requirement. |
| **UI-5** | LOW | [FrmReports.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmReports.vb#L464-L513) — `ResetMonthlyChart()` destroys & recreates `chartMonthly` series every render | Safe pattern (avoids stale binding) but heavy; fine for a manual-refresh form. No crash risk found. |
| **UI-6** | LOW | Program.vb sets `HighDpiMode.SystemAware`, but forms use a **mix** of `AutoScaleMode.Font` (designer forms) and `AutoScaleMode.Dpi` (`FrmChartPreview`, `FrmMultiMachineImport`) | Mixed scaling can cause controls to be slightly mis-positioned when the app is moved between monitors of different DPI. Cosmetic, not a crash. |

### 1.4 Multi-Machine Import / Export File Handling (`FrmMultiMachineImport.vb`, `FrmExportForTransfer.vb`, `TransferFormat.vb`)

| # | Severity | Location | Risk / Failure Scenario |
|---|---|---|---|
| **MM-1** | **HIGH** | [FrmExportForTransfer.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmExportForTransfer.vb#L230) — `Directory.CreateDirectory(exportFolder)` is executed **outside** the Try/Catch block | If the target (e.g., write-protected / full USB drive) refuses folder creation, the exception escapes `btnExportForTransfer_Click` unhandled → generic global crash dialog instead of the intended "ส่งออกไม่สำเร็จ ... โฟลเดอร์จะถูกลบ" flow. |
| **MM-2** | MEDIUM | [FrmMultiMachineImport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmMultiMachineImport.vb#L564-L573) — `LoadWorksheet` reads the **entire sheet** into a DataTable with `SELECT *` and no row/size cap | A corrupted or maliciously large `.xlsx` (e.g., 500 MB / 1M rows) can exhaust memory and OOM-crash before `ParseExcelFile`'s row-count check runs. Excel is buffered; the guard only works after the whole table is already loaded. |
| **MM-3** | MEDIUM | [FrmMultiMachineImport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmMultiMachineImport.vb#L660-L670) — `AnalyzeAllDuplicates` loads **all** `TransactionGUID` and composite keys into in-memory HashSets every preview refresh | On a large master DB (100k+ transactions) this is a full-table scan + memory spike per refresh; on the UI thread it freezes the form. No crash, but a responsiveness/scale risk. |
| **MM-4** | MEDIUM | `ParseExcelFile` — schema validation is good (date/amount/type per-row `ErrorMsg`), but **no file-size / row-count limit before opening** and no per-file hard cap | Combined with MM-2; recommend checking `FileInfo.Length` and a `MaxRows` cap before reading. |
| **MM-5** | MEDIUM | [FrmMultiMachineImport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmMultiMachineImport.vb#L303-L313) — re-import flow is guarded, but `FileAnalysis.MachineID` fallback uses the **filename** as a machine ID | Design decision (documented as "ความน่าเชื่อถือต่ำ") — not a crash, but data-integrity risk if files are renamed. Consider warning the user to rename to a known machine ID. |
| **MM-6** | LOW | Error messages in `ParseExcelFile` include raw `ex.Message` (OleDb internals) | Minor info-disclosure; acceptable for a local single-site app, but consider wrapping in a Thai-friendly prefix. |

### 1.5 Cross-cutting

| # | Severity | Risk |
|---|---|---|
| **X-1** | MEDIUM | **Global ThreadException handler** ([Program.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Program.vb#L73-L83)) shows a dialog but does **not** ask the user to restart — the app continues running after the dialog in a possibly corrupted UI state. Recommend offering "Continue / Restart". |
| **X-2** | LOW | `AppPaths.FindPreferredDatabaseFile` / `ProjectRoot` silently return empty on any exception; combined with DB-2, a broken deployment shows confusing later errors rather than a clear startup message. |

---

## 2. Recommended Code Fixes & Protective Safeguards

### 2.1 Database & File I/O

**Fix DB-1 (critical):** In `Db.EnsureSchema()`, move `_schemaChecked = True` to the **end** of the method, and reset it to `False` in a `Catch` so a failure is retryable on the next call:

```vbnet
Public Sub EnsureSchema()
    If _schemaChecked Then Return
    Try
        ' ... all table creation + migrations + seeds ...
        _schemaChecked = True          ' only on full success
    Catch ex As Exception
        AppPaths.LogCrash(ex, "Db.EnsureSchema")
        _schemaChecked = False         ' allow retry on next call
        Throw
    End Try
End Sub
```

**Fix DB-2 (connection retry & lock detection):** Introduce a centralized `Db.OpenConn(Optional retries As Integer = 3)`:

```vbnet
Public Function OpenConn(Optional retries As Integer = 3) As OleDbConnection
    Dim lastEx As Exception = Nothing
    For attempt As Integer = 1 To retries
        Try
            Dim conn As New OleDbConnection(ConnectionString)
            conn.Open()
            Return conn
        Catch ex As OleDbException
            lastEx = ex
            ' ERROR_LOCKED (0x80040E21 / -2147217887) or "is already open exclusively"
            If attempt < retries Then
                System.Threading.Thread.Sleep(500 * attempt)
                Continue For
            End If
        End Try
    Next
    Throw lastEx
End Function
```

Add `OLE DB Services=-2`? — **no**, that disables pooling; instead keep pooling default and rely on retry. Also detect a stale `.laccdb` lock file and surface a Thai message: "ฐานข้อมูลถูกใช้งานอยู่ อาจมีโปรแกรมอื่นเปิดอยู่ — ปิดโปรแกรมอื่นแล้วลองอีกครั้ง".

**Fix DB-3 (backup resilience):** In `BackupDatabase()`, add retry + integrity note:

```vbnet
For attempt As Integer = 1 To 3
    Try
        File.Copy(src, dst, True)
        Return dst
    Catch ex As IOException When attempt < 3
        Threading.Thread.Sleep(800 * attempt)
    End Try
Next
```

And in `RunMerge`, move `Db.BackupDatabase()` **inside** the merge Try so a backup failure reports accurately ("สำรองข้อมูลไม่สำเร็จ — ยกเลิกการรวมข้อมูล" instead of "ระบบได้ยกเลิกการบันทึกข้อมูลทั้งหมดแล้ว").

**Fix DB-5 (startup gate):** In `Program.vb` Step 4, offer a choice: `"Retry / Open without DB (read-only check) / Exit"`. If the DB file is missing, block `Application.Run` rather than starting a broken app.

### 2.2 Report Engine & Printing

**Fix RP-1 (critical — GDI+ leak):** Add a `Dispose` override to `IncomeExpenseReport` and call `_theme.Dispose()`:

```vbnet
Protected Overrides Sub Dispose(disposing As Boolean)
    If disposing AndAlso _theme IsNot Nothing Then
        _theme.Dispose()
        _theme = Nothing
    End If
    MyBase.Dispose(disposing)
End Sub
```

Also fix the leak in `BaseReport.vb` (dead scaffolding) or delete the class entirely.

**Fix RP-2 (row-height cap):** In the `OnPrintPage` loop, cap `currentRowH` to the available page space so an oversized row can never cause an infinite page loop:

```vbnet
Dim maxRowH As Integer = (_pageBottom - e.MarginBounds.Top) - 10
currentRowH = Math.Min(currentRowH, maxRowH)
```

**Fix RP-3 (negative header guard):** In `PreCalculatePageBreaks`, clamp `usableH` to a sane minimum:

```vbnet
Dim headerH As Integer = Math.Max(0, _estimatedHeaderHeight - 50)
Dim usableH As Integer = Math.Max(rowH * 4, pageBottom - (50 + headerH))
```

**Fix RP-6 (printer check):** In `Show*` wrappers, pre-check `PrinterSettings.InstalledPrinters.Count = 0` and show a friendly Thai message ("ไม่พบเครื่องพิมพ์ในระบบ") before constructing the report.

**Fix RP-7 (HiDPI chart bitmap):** Use the form's DPI scale factor:

```vbnet
Dim scale As Single = Me.DeviceDpi / 96.0F
Dim w As Integer = CInt(Math.Max(chartPreview.Width, 1200) * scale)
Dim h As Integer = CInt(Math.Max(chartPreview.Height, 800) * scale)
```

### 2.3 UI Threading & High-DPI

**Fix UI-1 (re-entrancy — critical):** Do **not** use `Application.DoEvents()` inside DB-transaction loops. Options (in order of preference):
1. Run merge/analysis on a background thread (`BackgroundWorker` or `Task`) and marshal progress via `BeginInvoke` (the `Log`/`SetProgress` methods already use `InvokeRequired`/`BeginInvoke` — the plumbing exists).
2. Minimum viable: disable **all** action buttons (`btnSelectFiles`, `btnClearFiles`, `btnClose`, `btnViewDuplicates`) for the duration of `_isMerging`, and replace `DoEvents()` with a short `Threading.Thread.Sleep(1)` + explicit repaint of only the progress labels.

**Fix UI-2 (min size):** In `FrmChartPreview` constructor add `Me.MinimumSize = New Size(800, 500)`.

**Fix UI-3 (DPI buttons):** Compute button heights/widths from `Me.DeviceDpi / 96` when building the toolbar, or place the 4 buttons in a `FlowLayoutPanel` so they wrap instead of clipping.

### 2.4 Multi-Machine Import / Export

**Fix MM-1 (folder creation outside Try):** Move `Directory.CreateDirectory(exportFolder)` inside the existing Try (or wrap it) so a failed USB write shows the intended error flow and cleans up.

**Fix MM-2 / MM-4 (file limits):** Before opening the Excel connection in `ParseExcelFile`, check:

```vbnet
Dim fi As New FileInfo(filePath)
If fi.Length > 50 * 1024 * 1024 Then      ' 50 MB guard
    analysis.SchemaError = "ไฟล์ใหญ่เกินไป (เกิน 50 MB) — ไม่ปลอดภัยที่จะอ่าน กรุณาแบ่งไฟล์ก่อนนำเข้า"
    Return analysis
End If
```

And cap rows in `LoadWorksheet`: stop after e.g. 200,000 rows with a `SchemaError` instead of loading unbounded. Guard against sheet names that could inject SQL (`[` / `]` / `$` / `'`) since `LoadWorksheet` uses string concatenation into `SELECT * FROM [...]`.

**Fix MM-3 (dup analysis scale):** Cache the master DB key sets once per file-selection session (reuse across preview refreshes) instead of re-scanning the whole table every refresh; optionally move to a background thread (see UI-1).

**Fix X-1 (post-crash UX):** In the ThreadException handler, after logging, ask: `"Continue (อาจเกิดความผิดพลาดซ้ำ) / Restart the program"` — with restart via `Application.Restart()` guarded in a Try/Catch.

---

## 3. Actionable Refactoring Roadmap

Prioritized `P0 → P2`. P0 items prevent crashes/data corruption; P1 prevent leaks & hangs; P2 improve resilience/UX.

### P0 — Must fix (crash / data-integrity)

| # | Task | Files | Fix ref |
|---|---|---|---|
| 1 | Make `EnsureSchema()` retryable — move `_schemaChecked = True` to end + reset on failure | `Database.vb` | DB-1 |
| 2 | Fix GDI+ leak — add `Dispose` override calling `_theme.Dispose()`; delete or dispose `BaseReport` | `IncomeExpenseReport.vb`, `BaseReport.vb` | RP-1 |
| 3 | Cap `currentRowH` to prevent infinite print loop | `IncomeExpenseReport.vb` | RP-2 |
| 4 | Move `Directory.CreateDirectory(exportFolder)` into the Try/Catch of `btnExportForTransfer_Click` | `FrmExportForTransfer.vb` | MM-1 |
| 5 | Eliminate `Application.DoEvents()` during DB-transaction loops; disable all action buttons while `_isMerging` (or run merge on a background thread) | `FrmMultiMachineImport.vb`, `FrmLocationImport.vb` | UI-1 |
| 6 | Add file-size (50 MB) + row-count (200k) + sheet-name sanitation guards in `ParseExcelFile`/`LoadWorksheet` | `FrmMultiMachineImport.vb` | MM-2/MM-4 |

### P1 — Should fix (leaks, hangs, confusing errors)

| # | Task | Files | Fix ref |
|---|---|---|---|
| 7 | Add retry + lock detection to `OpenConn()` and `BackupDatabase()`; Thai lock-warning message | `Database.vb` | DB-2/DB-3 |
| 8 | Move `Db.BackupDatabase()` inside the merge Try for accurate failure messaging | `FrmMultiMachineImport.vb` | DB-3 |
| 9 | Pre-check `InstalledPrinters.Count = 0` in all `Show*`/`SaveToPdf` wrappers with friendly message | `IncomeExpenseReport.vb` | RP-6 |
| 10 | Clamp `usableH` (negative-header guard) in `PreCalculatePageBreaks` | `IncomeExpenseReport.vb` | RP-3 |
| 11 | Startup gate in `Program.vb` Step 4 — offer Retry/Exit instead of starting broken | `Program.vb` | DB-5 |
| 12 | Add `MinimumSize` to `FrmChartPreview`; DPI-scale the toolbar buttons | `FrmChartPreview.vb` | UI-2/UI-3 |
| 13 | DPI-scale `RenderChartToBitmap` via `DeviceDpi` | `FrmChartPreview.vb` | RP-7 |
| 14 | After ThreadException dialog, offer Continue/Restart | `Program.vb` | X-1 |

### P2 — Nice to have (scale, UX, hygiene)

| # | Task | Files | Fix ref |
|---|---|---|---|
| 15 | Cache master-DB duplicate key sets across preview refreshes; move dup analysis off the UI thread | `FrmMultiMachineImport.vb` | MM-3 |
| 16 | Wrap raw OleDb error text in Thai-friendly context messages | `FrmMultiMachineImport.vb`, `FrmExportForTransfer.vb` | MM-6 |
| 17 | Guard `LoadChart` against double `AddHandler` (remove before add) if it ever becomes reusable | `FrmChartPreview.vb` | UI-4 |
| 18 | Unify `AutoScaleMode` policy (decide one mode app-wide) and remove unused `_pageMargin`/`LayoutConfig` dead code | `FrmChartPreview.vb`, `IncomeExpenseReport.vb` | UI-6/RP-5 |
| 19 | Add `TaskScheduler.UnobservedTaskException` handler if async is introduced | `Program.vb` | DB-7 |

### Verification steps after each P0/P1 fix

1. `dotnet build TempleAccounting.vbproj -c Debug` and `-c Release` → 0 warning / 0 error.
2. **Leak test (RP-1):** run 30× Print Preview of the Detail report, verify GDI+ handle count (`tasklist /fi "imagename eq TempleAccounting.exe"` or Process Explorer) stays flat.
3. **Infinite-loop guard (RP-2):** add a test transaction with an extremely long detail (e.g., 50,000 chars), print — must terminate and cap the row height.
4. **Lock test (DB-1/DB-2):** open the `.accdb` in Access, launch the app → expect a friendly Thai message + retry, not a crash.
5. **USB-fail test (MM-1):** export to a write-protected flash drive → expected "ส่งออกไม่สำเร็จ" flow with folder cleanup, not the global crash dialog.
6. **Re-entrancy test (UI-1):** during a large import, click `btnClose`/`btnSelectFiles` — must be ignored.

---

*End of System_Stability_and_Robustness_Audit.md — audit of PROJECT_PROFILE V4/V5/V6 claims against live source. No application files were modified; this report is advisory only.*
