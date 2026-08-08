# Spec Addendum: FrmExportForTransfer.vb — Export Data for Transfer to Other Machines

This document supplements `Spec_FrmMultiMachineImport.md`. Its purpose is to make the **Export side** and the **Import side** use the same data structure, so the deduplication and MachineID tracking designed on the import side actually work in practice, not just in theory.

---

## 0. Most Important Principle: GUID Must Be Generated at "Transaction Save Time," Not at "Export Time"

**This is a change that affects files outside the export/import forms** — it must be made at the core transaction-saving point of the system (the existing form/function used to record income/expense entries), not just inside the new forms.

- Add a `TransactionGUID` column (Text/GUID, nullable) to the master transaction table if it doesn't already exist.
- Modify the existing save routine so that `Guid.NewGuid()` is generated immediately whenever a new transaction is inserted.
- Existing records already in the database (before this column existed) will have no GUID — export/import should fall back to the composite key described in the import spec (Section 4) for old records only.
- **Caution:** This change carries the highest risk of impacting existing system behavior in the entire project. Have the AI agent show a diff of the modified file(s) for review before committing, and re-test normal transaction entry multiple times afterward to confirm nothing has changed (and that queries on old records with a null GUID field don't throw errors).

---

## 1. Machine ID: Must Be Set Once Per Machine — Not Randomized on Every Export

- Add a table/config file on the local machine (e.g., a `LocalSettings` table in the database, or a separate local config file) storing a `MachineID` value that an administrator sets once during installation (e.g., "KIOSK-01", "Main Office").
- If it has never been set, prompt the user to set a machine name the first time the export button is pressed — do not export immediately without a MachineID.
- This `MachineID` is the same value the import side will read and display in the preview column per the earlier spec.

---

## 2. FrmExportForTransfer.vb — Export Form

### Form Opening Pattern
Like `FrmMultiMachineImport.vb`, this form must open as a **Modal Dialog (`.ShowDialog()`)**, not embedded in `pnlFormHostBody` via `ShowFormInPanel(...)`. Same reason: exporting involves copying attachment files, which takes time. If embedded in the panel and the user switches menus mid-process, the form would be disposed mid-flight by `CloseActiveForm()`, risking an incomplete export.

### Main Button
- `btnExportForTransfer` ("Export Data to Another Machine")

### Data Range Options
- **Export only new records since the last export** (default/recommended) — based on that machine's `ExportHistory`
- **Export by custom date range** — for re-sending or backdated exports
- **Export everything** — for special cases only, e.g., setting up a new machine

### Required Export File Structure (so import can read it correctly)
The export file must include a **header/metadata block** separate from the data table, so the import side can validate it before opening the actual data:

| Header Field | Description |
|---|---|
| `MachineID` | Source machine identifier (Section 1) |
| `ExportDateTime` | Date and time of export |
| `SchemaVersion` | File structure version number (see Section 3) |
| `RowCount` | Number of rows in the file (lets import verify the actual row count read matches, catching corrupted or truncated files) |

Each data row must include at minimum: `TransactionGUID` (may be blank for old records), `TranDate`, `Amount`, `Type`, `Description` — column names and order must exactly match what `FrmMultiMachineImport.vb` expects.

### ExportHistory (paired table with ImportHistory)
Log every successful export: `ExportBatchID, ExportDateTime, DateRangeFrom, DateRangeTo, RowCount, FileNameGenerated`
Used so the "export only new records" option knows where to resume from, and to prevent confusion about what has already been exported.

---

## 3. Schema Version — Guarding Against Machines Updated at Different Times

In practice, each machine may get the program updated at a different time, causing the export file structure to diverge across versions.

- Define a constant `SCHEMA_VERSION` in code (e.g., "1.0"), embedded in the header at export time.
- `FrmMultiMachineImport.vb` must check this value before reading data. If the version doesn't match what the import form supports, show a clear warning ("This file was exported from a different program version and may be read incorrectly") instead of attempting to read it and failing with an unclear error.

---

## Precautions (Export-side specific — not duplicated from the import spec)

1. **Do not implement automatic scheduled export.** Keep it a manual button-press only, so the administrator controls where the file goes / which flash drive it's copied to, reducing the risk of data leaving the system unintentionally.
2. **Export must never modify the source data.** It must be a read-only query. Do not mark any status in the source transaction table as "already exported," since that would affect the existing accounting logic. Use the separate `ExportHistory` table to track this instead.
3. **Name export files with MachineID + date embedded** (e.g., `Export_KIOSK-01_20260808.xlsx`) for the user's convenience when selecting files — but **the system must never rely on the file name as the source of truth.** Always treat the MachineID embedded in the header as authoritative; the file name is a visual convenience only.
4. **Test a full round trip before going live.** Export from Machine A, then import into Machine B (or a test database), and confirm the row count and total amounts match exactly before using this on the temple's real data.

---

## Additional Deliverables from the AI Agent

1. New `FrmExportForTransfer.vb` file (+ `.Designer.vb`)
2. Scripts to create the `ExportHistory` and `LocalSettings` tables (if not already present), and the `TransactionGUID` column on the master transaction table
3. **Modification of the existing transaction-save routine** (clearly name the file(s) changed in the results report) to generate a GUID when a new transaction is saved — including confirmation that normal saving was tested and behaves exactly as before
4. Add a `"Export Data to Another Machine"` button in `frmMain.vb`'s `pnlSidebar` (same location as the import button) — the event handler should call `New FrmExportForTransfer().ShowDialog()` directly, not `ShowFormInPanel(...)`
5. A report of at least one successful export → import round-trip test, including matching row counts and total amounts
