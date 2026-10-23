# T12 — Import / Export / Multi-Machine (survey notes)

Evidence: HEAD = `0c274d5`. Line refs are HEAD.

## 1. The two import paths (distinct!)

### A. Single-file bulk import — `ExcelTransactionImporter.vb` (Module, 298 LOC)
- **Entry points:** `FrmIncome.vb:602` and `FrmExpense.vb:558` (file picker inside the income/expense forms; imports rows into the currently-open form's tranType with the form's selected defaults).
- Excel via **ACE OLEDB**: `.xlsx/.xlsm/.xlsb` → `Excel 12.0 Xml;HDR=YES;IMEX=1`; `.xls` → `Excel 8.0` (163-172). Other extensions rejected with Thai message.
- Flexible headers: `FindColumn` + alias lists + `NormalizeColumnKey` (case-insensitive, strips special chars). Required: date/detail/amount — else `ApplicationException` listing missing Thai names.
- Per-row flow (88-150): blank-row skip → `TryParseImportDate` → detail required → `TryParseImportAmount` (> 0) → Resolve category/fund/bank → **duplicate check** (`COUNT(*)` on 7-field composite: Type+DateValue+Category+Fund+Bank-null-normalized+Detail+Amount+Note) → INSERT (parameterized, `Db.AccessDateLiteral`).
- **Resolvers AUTO-CREATE missing master data** (`Db.InsertAndGetId` into Categories/Funds/BankAccounts; counted as `CreatedCategoryCount` etc. in the result summary). Blank name → falls back to the form-selected default ID.
- Safety: `Db.EnsureSchema()` + `Db.BackupDatabase()` before the loop (85-86).
- ⚠️ **No OleDbTransaction** — rows autocommit one by one; a mid-import crash leaves a partial import (rows are individually validated, and re-running the same file is safe because the composite duplicate check skips already-inserted rows).
- ⚠️ **No `TransactionGUID` / `SourceMachineID` written** — single-file imports are invisible to the multi-machine GUID dedup tier (they only match the composite "suspected" tier).

### B. Multi-machine transfer — `FrmExportForTransfer.vb` (export) + `FrmMultiMachineImport.vb` (import, 1249+ LOC)
- Opened from `frmMain.vb:274` (import) and `:281` (export) as the deliberate modal dialogs (see T09).
- Shared contract: **`TransferFormat.vb`** module — `SchemaVersion "1.0"`, sheet names `Metadata`/`Data`, metadata field names, `DataColumns` array. Explicitly exists to prevent schema drift between differently-versioned machines.

#### Export side (`FrmExportForTransfer`)
- **MachineID** set once per machine, stored in `LocalSettings` (KeyValue='MachineID'); export blocked until set (17, 103-134).
- Schema self-provision at load: `EnsureExportSchema` (481-523) creates `LocalSettings` + `ExportHistory` (GUID PK, datetime range, RowCount, FileNameGenerated, MachineID) if absent — non-breaking.
- Range modes (`TryGetDateRange`, 253-293): **all** (from 2000-01-01), **since-last-export** (resume point = `MAX(DateRangeTo)` from ExportHistory for this MachineID → next day; first run covers everything), **custom** (validated from ≤ to). `rangeTo` = end of today by default.
- Source read is **pure SELECT** — rows never marked/modified (293-335, deliberate).
- `WriteExportFile` (335-480): writes `TransactionData.xlsx` into a timestamped export folder via **ACE CREATE TABLE** (deletes pre-existing file first — ACE appends duplicate sheets otherwise); Data sheet columns = TransferFormat.DataColumns (Note exported as **`Remark`** — "Note" is reserved in Access DDL); metadata header block written with positional `?` params (ACE can't do named params); **dates written as ISO strings** `yyyy-MM-dd` for locale-independence; per-200-row progress updates.
- Receipts: copied to `<exportFolder>\Receipts\{TransactionGUID}.jpg` (re-optimized copies); rows without GUID or missing source counted, never fail the batch.
- `ExportHistory` row inserted **only after the file write succeeds** (421-426) — the resume point can't advance on a failed export.

#### Import side (`FrmMultiMachineImport`)
- Multi-select files; per file: **SHA-256** (`ComputeSha256`) checked against `ImportHistory` (`FindPreviousImport` — warns "imported before on date X", never silently auto-skips, 272-286).
- Metadata sheet validated first (`ParseExcelFile`, 385+); schema mismatch → file marked `SchemaError` and skipped at merge.
- Receipt attachments discovered from sibling `Receipts\` folder by GUID filename (`DiscoverReceiptAttachments`, 358).
- **Two-tier dedup** (`AnalyzeAllDuplicates`, 681-752):
  1. `TransactionGUID` vs DB (`SELECT TransactionGUID … WHERE NOT NULL`) + within the selected file set → `IsDuplicateExact` — always skipped.
  2. Composite key `yyyy-MM-dd|Type|Amount(0.00 invariant)|Detail|MachineID` (`BuildCompositeKey`, 740) vs DB → `IsSuspectedDuplicate` — skipped only when the user chooses "skip suspected" (preview grid + duplicate-detail dialog first).
- **Merge (`RunMerge`, 935-1132):**
  - Pre-merge: `Db.BackupDatabase()` (explicit "safety net" comment).
  - Receipt **staging**: files copied to `%TEMP%\TempleImport_Staging_{guid}` before INSERT; after successful commit moved to `AppPaths.ReceiptsDir` as `Receipt_{newId}.jpg` (matched via `@@IDENTITY` inside the same tx); staging dir always deleted in `Finally`. DB stays consistent even if file moves fail (logged, reported).
  - **Single `OleDbTransaction` across ALL files**; per-file `ImportBatchID` GUID; rows carry `TransactionGUID`, `SourceMachineID`, `ImportBatchID`; `ImportHistory` INSERT happens **inside the same tx** (commit only when everything succeeds; rollback + rethrow on error, 1053-1110).
  - `ResolveCategoryId/FundId/BankId` (1177-1236): in-tx lookup with dictionary cache; **auto-create missing Categories/Funds/BankAccounts** inside the same transaction.
- Schema self-provision at load: `EnsureImportSchema` (1237+) creates `ImportHistory` + adds `SourceMachineID`/`ImportBatchID` columns to `Transactions` — non-breaking.

## 2. Issues / debt found

1. **Duplicated importer helpers:** 8 functions exist verbatim in BOTH `ExcelTransactionImporter.vb` and `FrmMultiMachineImport.vb`: `FindColumn`, `NormalizeColumnKey`, `SafeCellText`, `IsBlankImportRow`, `TryParseImportDate`, `TryParseImportAmount`, `GetWorksheetNames`, `LoadWorksheet`. Drift risk — should be a shared module (e.g., next to `TransferFormat`).
2. Single-file import writes no `TransactionGUID` → those rows can only be deduped by the "suspected" composite tier in the multi-machine merge (exact-GUID tier misses them).
3. Single-file import is non-transactional (row-by-row autocommit). Safe to re-run (composite dedup), but a crash mid-run leaves a partial import with no batch marker.
4. Both import paths auto-create master data (categories/funds/banks) from arbitrary Excel text — a typo in a spreadsheet silently creates a new category (counted in the summary, but still).
5. Both import forms run their heavy loops on the UI thread (see T07 P1: Excel import + merge).
6. `ExcelTransactionImporter` inserts have no `ImportBatchID` — not traceable to a source file in `ImportHistory`.

## 3. Verified-good design points

- Export resume point cannot advance on failure (history written only after successful write).
- Import merge: one transaction for all files, receipts staged before commit, staging always cleaned, per-file ImportBatchID traceability, ImportHistory inside the tx.
- Two-tier dedup with user control over the fuzzy tier; preview + duplicate detail dialog before commit.
- `TransferFormat` centralizes the cross-machine file contract (SchemaVersion 1.0).
- ISO date strings in the transfer file + invariant composite keys → locale-proof.
- Pre-merge automatic DB backup.
