# T16 — Dead Code, Duplication, Tech Debt (survey notes)

Evidence: HEAD = `0c274d5`. Cross-references T01-T15 notes.

## 1. Project hygiene (vbproj vs disk)

- vbproj: **64 Compile + 14 EmbeddedResource + 5 None** items; globs `Database/**/*`, `Import/**/*`.
- Disk `.vb` NOT in vbproj (correctly excluded, own projects): `BackupTests/BackupEngineTests.vb`, `BackupTests/LegacyBackupTests.vb` (test project), `tools/BackupScanDiag/Program.vb` (own `BackupScanDiag.vbproj` diagnostic tool).
- `.gitignore` = standard VS template; **bin/obj not tracked** (0 files); `*.accdb` ignored (`.gitignore:370`) — the real DB and its pre-P6 snapshot stay out of git. ✔
- Tracked oddities: **2 real export artifacts committed** — `Export/Report_25690726_010655.csv` and `…_083321.csv` (5 bytes each, Buddhist-year names — harmless but repo litter); `Database/CreateImportHistory.sql` + `CreateTransferSchema.sql` are **doc copies of DDL superseded by `EnsureImportSchema`/`EnsureExportSchema` in code** (drift risk if schema changes).
- **Zero `TODO`/`FIXME`/`HACK`/`XXX` markers** anywhere in .vb sources.

## 2. Dormant / dead functionality

1. **Saddhiviharika registry is write-never-read from UI** — `SaddhiviharikaDb.EnsureSchema` runs at every boot (Program.vb, "Step 4c", full `temples`/`monks`/ordination schema with REFERENCES) but **no form, report or service queries its tables** (only Program.vb/AppPaths.vb reference the module). Dormant feature: boot cost + schema maintenance for zero product surface. (V8 profile mentions it as init-only too — V9 should flag as *unintegrated*.)
2. **`BaseReport`** — removed from code before `91d2a5e`; survives only in 10 markdown docs (T11).
3. **`_testReportViewed`** — dead flag in wizard Step 5 (T08).
4. **`dgvReport_CellContentClick`** — empty wired handler (T11).
5. **`SqlServerDbProvider`** — fully implemented `IDbProvider` behind `DbProviderFactory`, but `ProviderType` is **never set anywhere** (`Database.vb` facade always resolves Access). The abstraction's payoff ("switch by changing ProviderType") has never been exercised; SqlServer path is compile-only.
6. Legacy `.sql` scripts (see §1) and old-format backup folders in `Database/` (T06) — kept as artifacts.

## 3. Duplication inventory (quantified)

| Pair / group | Metric | Note |
|---|---|---|
| `FrmIncome.vb` (666) vs `FrmExpense.vb` (622) | **~79% identical** after token normalization (T10) | Income-only: PromptPay/ReceiveChannel (16 refs vs 0); drift: FrmExpense writes `ReceiptPath` by inline OleDb UPDATE while FrmIncome uses `UpdateReceiptPath` |
| Excel importer helpers ×2 | **8 functions verbatim in both** `ExcelTransactionImporter.vb` and `FrmMultiMachineImport.vb` (1355 LOC): FindColumn, NormalizeColumnKey, SafeCellText, IsBlankImportRow, TryParseImportDate, TryParseImportAmount, GetWorksheetNames, LoadWorksheet (T12) | drift risk across a cross-machine file contract |
| Thai-format helpers ×2 | 6-7 private copies in `IncomeExpenseReport` duplicating `ReportEngine` publics; **already drifted** (`ToBuddhistYearThai` Integer vs String) (T11) | |
| ReceiptPath update | 2 implementations (repository method vs inline SQL) (T10) | |
| Dialogs & logs | 2 MessageBox systems (`MessageBoxHelper` + raw `MessageBox.Show`) and 2 log files (`crash.log`/`error.log`) (T14) | support discoverability |

**Root cause of form duplication — half-adopted layering:** the repository pattern exists (`ITransactionRepository` → `AccessTransactionRepository` → `TransactionService`) but is **consumed by exactly one form**: `FrmIncome.vb:23` (`New TransactionService(New AccessTransactionRepository())`). FrmExpense, FrmTransactions, FrmTransfer, wizard, imports all hand-roll SQL — so every behavior change must be made N times.

## 4. Structural debt (beyond duplication)

- **No FK constraints / referential guards** (proven, T13) — consequences: silent label loss, misleading "in use" error copy.
- **UI-thread heavy work** — Excel import, merge, location rebuild, restore, PDF (T07).
- **Culture-sensitive dates remain** (~19 format sites → Buddhist-year filenames/logs; T05/T14/T15).
- **Non-transactional multi-writes**: single-file Excel import (row autocommit), FrmLocationImport 3 DELETEs + 8.4k INSERTs (T04/T12), SeedPositions UPDATE ×19 rows **every boot** (T06 bonus P1 — DB mtime = boot time).
- **Log growth unbounded** (T14).
- **AutoScaleMode lottery** Dpi 5 / Font 6 / unset 6 (T09/T15).
- Tests: 26 backup tests pass (RUNTIME) but **no test files detected in-repo conventions** (BackupTests project only; no CI).

## 5. Recommended dedup targets (priority order for V9+ work)

1. Extract shared transaction-entry logic (Income/Expense) — biggest win (~700 duplicated lines).
2. Move the 8 Excel helpers next to `TransferFormat` into a shared module (cross-machine correctness).
3. Delegate Thai-format helpers in `IncomeExpenseReport` to `ReportEngine`.
4. Route FrmExpense (and others) through `ITransactionRepository` or delete the half-layer.
5. Delete/quarantine dead bits (`_testReportViewed`, empty handler, doc-only BaseReport references, orphan Export CSVs, stale .sql).
6. Decide Saddhiviharika's fate: integrate (UI) or stop boot-initializing it.
