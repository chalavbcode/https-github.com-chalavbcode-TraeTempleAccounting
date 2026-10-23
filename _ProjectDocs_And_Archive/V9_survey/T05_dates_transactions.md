T05 — Data integrity: dates and transactions (V9 survey)
Generated: 2026-10-23. VERIFIED unless marked. Key new evidence this session: **machine locale is Thai** (th-TH → Buddhist calendar for culture-sensitive date formatting), PROVED by `Logs\crash.log` boot lines like `2569-10-23 06:42:53` written via `DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")` without culture (Program.vb:29) and `error.log` headers `[2569-10-23 …]` (ErrorLogHelper.vb:27,49).

## 1. Date normalization usage (grep counts per file — VERIFIED)
`Db.AccessDateLiteral` call sites: frmMain 6, FrmTransactions 4, FrmExpense 4, ReportEngine 3, AccessTransactionRepository 2, FrmReports 2, FrmIncome 2, ExcelTransactionImporter 2, OnboardingRepository 1, FrmTransfer 1, FrmMultiMachineImport 1 (+ provider defs).
`Db.NormalizeGregorianDate`: FrmTransactions 5, FrmMultiMachineImport 5, ExcelTransactionImporter 5, ReportEngine 3, FrmExportForTransfer 3, IncomeExpenseReport 2, FrmReports 2, OnboardingRepository 1, FrmIncome 1, FrmExpense 1 (+ provider defs).
Every INSERT into `Transactions` verified to embed the date via `Db.AccessDateLiteral(...)` (AccessTransactionRepository.vb:22, ExcelTransactionImporter.vb:143-144, FrmMultiMachineImport.vb:1026-1027, OnboardingRepository.vb:197,204 via `dateLit`). Reads via DateTimePicker use NormalizeGregorianDate (FrmIncome.vb:63, FrmExpense.vb:59). **No raw `.ToString("yyyy-…")` found embedded in SQL.**
SQL-side mixed-era idiom present: `IIF(Year(x)>2400, DateAdd('yyyy',-543,x), x)` (FrmTransactions.vb:119,186), `DateSerial(IIF(Year>2400, Year-543, …))` (FrmReports.vb:128,161), `DateValue(TranDate)` comparisons (ExcelTransactionImporter.vb:128, FrmExportForTransfer.vb:296,301, FrmMultiMachineImport.vb:693).

## 2. Culture-sensitive date formatting sites (BUGGY on this Thai-locale machine) — VERIFIED code + RUNTIME locale
Correct (InvariantCulture or integer properties):
- DatabaseBackupHelper.vb:43 (new folder name `Backup_yyyyMMdd_HHmmss` ✓ Gregorian — registry confirms `Backup_20261023_045135`)
- BackupLegacyEvaluator.vb:193,529 (InvariantCulture, commented rationale line 192)
- DatabaseBackupHelper.vb:677-705 (tier names via ISOWeek.GetYear/GetWeekOfYear and `d.Year`/`d.Month` integers; comments lines 679-680,691-692,703-704 explain Thai-locale `d:yyyy` trap)

**Culture-SENSITIVE (produce พ.ศ. year2569 on this machine):**
| Site | Effect | Evidence |
|---|---|---|
| Program.vb:29 BootLog timestamp | crash.log dated `2569-…` (RUNTIME observed) | Program.vb:26-31 |
| ErrorLogHelper.vb:27,49 | error.log headers `[2569-10-23 …]` (RUNTIME observed) | ErrorLogHelper.vb |
| **AccessDbProvider.vb:361** `TempleAccounting_{DateTime.Now:yyyyMMdd_HHmmss}.accdb` | pre-import/pre-merge DB backups get **Buddhist-year file names** (`Db.BackupDatabase()` callers: ExcelTransactionImporter.vb:87, FrmMultiMachineImport.vb:940) | AccessDbProvider.vb:356-365 |
| **FrmExportForTransfer.vb:224** `Export_{machine}_{yyyyMMdd_HHmm}` | export folder Buddhist-year named | FrmExportForTransfer.vb:224 |
| **FrmReports.vb:700** `Report_{yyyyMMdd_HHmmss}.csv` | CSV files `Report_25690726_*.csv` exist on disk (Export/) — RUNTIME proof | FrmReports.vb:700; disk |
| BackupEngine.vb:623 `.Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")` (no culture) | manifest/report Created field → `2569-…` | BackupEngine.vb:623 |
| BackupEngine.vb:658 same in text summary | Thai-year text in status/summary | BackupEngine.vb:658 |
| DatabaseBackupHelper.vb:140 `.Timestamp = …ToString("yyyy-MM-dd HH:mm:ss")` (no culture) | would write `2569-…` — BUT real registry entries show Gregorian `2026-…`, so the entries in the file were written by a different (InvariantCulture) path — BackupLegacyEvaluator.vb:529. Which path writes `.Timestamp` for new-engine entries = to verify in T06 | DatabaseBackupHelper.vb:140 |
| frmMain.vb UpdateStatusTime `{DateTime.Now:dd/MM/yyyy HH:mm:ss}` | no year → harmless | frmMain.UiHelpers.vb (UpdateStatusTime) |
| FrmExportForTransfer.vb:380 `r.TranDate.ToString("yyyy-MM-dd")` into Excel Data sheet | writes **พ.ศ. year** into the Excel date column; import side normalizes back (year>2400−543) so round-trip survives, but the Excel shows 2569 dates | FrmExportForTransfer.vb:380; importer NormalizeGregorianDate counts |

- TransactionExcelExporter.vb:16 & ReportEngine/IncomeExpenseReport Thai-numeral Buddhist dates = **by design** (display layer; explicitly documented in comments).

## 3. OleDbTransaction sites (complete, VERIFIED) + transaction-correctness audit
6 BeginTransaction sites (Begin→Commit/Rollback):
| # | Site | Commands inside | Correct? |
|---|---|---|---|
| 1 | OnboardingRepository.vb:31 (SaveTempleSetting) → Commit 51 / Rollback 53 | DELETE via explicit cmd WITH tx (l.37-39) + `Db.InsertAndGetId` (l.42) **WITHOUT tx** | ❌ **BROKEN** |
| 2 | OnboardingRepository.vb:94 (LinkPersonnelToTempleSetting) → 106/108 | single explicit cmd with tx (l.95-104) | ✅ |
| 3 | OnboardingRepository.vb:184 (SaveOpeningBalance) → 208/210 | DELETE explicit cmd with tx (l.189-191) + TWO `Db.InsertAndGetId` (l.193,200) **WITHOUT tx** | ❌ **BROKEN** |
| 4 | FrmTempleSetting.vb:315 → 342/344 | DELETE explicit cmd with tx (l.317-321) + `Db.InsertAndGetId` (l.325) **WITHOUT tx** | ❌ **BROKEN** |
| 5 | FrmTransactions.vb:456 → 467/469 | every DELETE via explicit cmd with tx (l.459-464) | ✅ |
| 6 | FrmMultiMachineImport.vb:978 → 1077/1081 | all statements via ExecInTx/ScalarInTx helpers which set `cmd.Transaction = tx` (FrmMultiMachineImport.vb:1149-1165) | ✅ |

### ❌ P0 FINDING — transaction-aware facade defect (VERIFIED + RUNTIME-TESTED)
`AccessDbProvider.ExecuteNonQuery/DbScalar/GetTable/InsertAndGetId` create commands via `conn.CreateCommand()` and **never assign `cmd.Transaction`** (AccessDbProvider.vb:305-345 read in full). System.Data.OleDb enforces assignment:
- Source: dotnet/runtime `OleDbCommand.cs` → `ExecuteNonQuery()` → `ExecuteReaderInternal()` → line 634 `ValidateConnectionAndTransaction(method)` → line 1385 `_connection.ValidateTransaction(Transaction, method)`; `OleDbConnectionInternal.cs` line ~785-790: if connection has a live pending transaction and `transaction is null` → `throw ADP.TransactionRequired(method)` ("…The Transaction property of the command has not been initialized."). Source: github.com/dotnet/runtime src/libraries/System.Data.OleDb (fetched this session).
- **RUNTIME test (PowerShell .NET OleDb, against the disposable DB copy, in V9_survey/tmp/OleDbTxTest.ps1):**
  - INSERT without Transaction while tx pending → **THREW** the exact ADP message.
  - Same INSERT with Transaction → SUCCEEDED; rollback verified, 0 stray rows.
- Net effect in the app: at sites 1/3/4 the **INSERT after the DELETE throws every time** → catch → Rollback → the save fails entirely (TempleSetting save shows "บันทึกข้อมูลวัดไม่สำเร็จ…"; wizard Step 1/Step 4 saves rethrow). Data is not corrupted (rollback restores) but **these saves cannot succeed** while the code stands.
- Blast radius confirmed limited: sites 2/5/6 assign Transaction correctly.
- UNVERIFIED: whether the user has actually attempted a TempleSetting save since commit 05d5f18 (2026-08-20) — requires a manual UI attempt or an error.log/message evidence; error.log contains no such entry (only backup INFO lines).

## 4. Multi-statement writes NOT transactional (VERIFIED list)
1. **FrmLocationImport.RunImport rebuild**: `DELETE FROM SubDistrict/District/Province` (FrmLocationImport.vb:148-150) then ~8,459 row INSERTs, **no transaction**; failure mid-run leaves truncated address tables. Runs on the UI thread.
2. **FrmIncome/FrmExpense save**: INSERT (InsertAndGetId) + separate `UpdateReceiptPath` (AccessTransactionRepository.vb:47-53) on separate connections — a crash between them leaves a row without receipt path (benign-ish) or receipt file without DB row (orphan — T10).
3. **AccessDbProvider seeds** (SeedCategories/Funds/BankAccounts/Positions/Personnel: each row its own command, no tx; only runs when table empty — partial seed possible on first-run failure; each subsequent run skips once count>0 → can leave partial seed permanently).
4. **MigrateTempleSetting** (ALTER/UPDATE series, no tx; individual legacy-col drops have inner empty Catch — AccessDbProvider.vb:124-129).
5. **FrmExportForTransfer schema prep**: CREATE TABLE LocalSettings/ExportHistory + ALTER TransactionGUID (FrmExportForTransfer.vb:496-515) — idempotent-guarded, no tx (acceptable; DDL in Access can't roll back anyway).
6. Backup/restore = file-level (no OleDbTransaction) — by design; covered T06.

## 5. Buddhist-era handling in backup file naming (all paths)
| Path | Naming code | Culture-safe? | Evidence |
|---|---|---|---|
| Tiered Weekly/Monthly/Yearly | ISOWeek integer + `d.Year`/`d.Month` | ✅ (explicit comments) | DatabaseBackupHelper.vb:673-705 |
| New engine full backup folder | `Backup_` & InvariantCulture `yyyyMMdd_HHmmss` | ✅ | DatabaseBackupHelper.vb:43-44; registry `Backup_20261023_045135` |
| Legacy helper folder (historical) | produced `Backup_25690804_145723` and `Backup_25691014_102801` (พ.ศ. years — culture-sensitive old code, or old code without InvariantCulture) | ❌ (historical) | real registry entries; disk folders in bin\Release\Backup |
| `Db.BackupDatabase` file name (pre-import/pre-merge) | interpolated `yyyyMMdd_HHmmss` no provider → พ.ศ. on this machine | ❌ | AccessDbProvider.vb:361; callers ExcelTransactionImporter.vb:87, FrmMultiMachineImport.vb:940 |
| BackupEngine manifest `Created` | `ToString("yyyy-MM-dd HH:mm:ss")` no culture → พ.ศ. | ❌ | BackupEngine.vb:623,658 |
| Legacy evaluator folder-time parse | reads digits from folder name; InvariantCulture on write; **does it interpret 2569 as year 2569 (DateTime accepts) and how that affects newest-first sorting/dedup vs Gregorian names → see T06** | ⚠️ open | BackupLegacyEvaluator.vb:484,192-193 |
| Registry `.Timestamp` (new entries in real file) | Gregorian `2026-…` observed | ✅ in file; source of truth to confirm in T06 (DatabaseBackupHelper.vb:140 lacks culture — contradiction noted) | %AppData%\TempleAccounting\backup_registry.json |

## 6. Open items carried to T06/T07
- Which writer produced registry entries' Gregorian Timestamps (vs line 140).
- Whether legacy Buddhist-named folders (`Backup_2569*`) parse/sort correctly in BackupLegacyEvaluator + FrmBackupVerify UI.
