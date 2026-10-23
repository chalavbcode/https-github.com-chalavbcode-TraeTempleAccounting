T04 — Database layer and schema (V9 survey)
Generated: 2026-10-23. VERIFIED unless noted. Evidence = file:line.

## Provider architecture (VERIFIED)
- `Db` facade (Database.vb:16-84) delegates EVERYTHING to `DbProviderFactory.Current` (Database.vb:17-19,25,31,37…82). Members: ConnectionString, OpenConn, EnsureSchema, ExecuteNonQuery, DbScalar, GetTable, InsertAndGetId, NormalizeGregorianDate, AccessDateLiteral, EscapeLikeText, ToDecimalOrZero, ToIntOrZero, BackupDatabase.
- `DbProviderFactory` (Services/DbProviderFactory.vb:29-65): default `_providerType = DbProviderType.Access` (line 29); `Current` lazily news AccessDbProvider or SqlServerDbProvider (lines 52-59). **No code anywhere sets ProviderType = SqlServer** (grep: only docs/property itself) → SqlServerDbProvider.vb (482 lines) is DEAD in production.
- `AccessDbProvider`:
  - Connection string: `Provider=Microsoft.ACE.OLEDB.12.0;Data Source={AppPaths.DatabaseFile};Persist Security Info=False;` (AccessDbProvider.vb:20-23). No password.
  - `OpenConn()` = new OleDbConnection + Open (lines 25-29). **Every helper opens/closes its own connection at call site via `Using conn = Db.OpenConn()`** — no shared connection, no connection pooling config.
  - `EnsureSchema()` guarded by `_schemaChecked` flag (lines 15, 31); throws FileNotFoundException if .accdb missing (lines 34-36); all schema ops inside one `Using conn` (lines 38-61); failure resets flag + rethrows (lines 62-65).
- `SqlServerDbProvider` (unused): hard-coded `Server=localhost\SQLEXPRESS;Database=TempleAccountingDb;Integrated Security=True;TrustServerCertificate=True` (SqlServerDbProvider.vb:26; master db at line 105).

## Schema created by Db.EnsureSchema (AccessDbProvider.vb:38-61) — VERIFIED, with real-DB column audit
| Table | Columns (from CREATE TABLE, AccessDbProvider.vb) | Line |
|---|---|---|
| Province | ProvinceID INTEGER PK, ProvinceName TEXT(100) NOT NULL | 40 |
| District | DistrictID PK, ProvinceID NOT NULL, DistrictName NOT NULL | 41 |
| SubDistrict | SubDistrictID PK, DistrictID NOT NULL, SubDistrictName NOT NULL, ZipCode TEXT(10) | 42 |
| TempleSetting | ID COUNTER PK, TempleCode TEXT(20), TempleName TEXT(200), TempleAddress MEMO, Tambon, Amphoe, Province, PostCode, TemplePhone TEXT(30), AbbotPersonnelID, WaiyawatPersonnelID, BookkeeperPersonnelID, PromptPayName TEXT(100), PromptPayID TEXT(50) | 44 |
| Categories | ID PK, CategoryName TEXT(200) NOT NULL, TranType TEXT(10) NOT NULL | 46 |
| Funds | ID PK, FundName TEXT(200) NOT NULL | 47 |
| BankAccounts | ID PK, BankName TEXT(100) NOT NULL, AccountNo TEXT(50), AccountName TEXT(200) | 48 |
| Transactions | ID COUNTER PK, TranDate DATETIME NOT NULL, TranType TEXT(10) NOT NULL, CategoryID, FundID, BankID, Detail TEXT(255), Amount CURRENCY NOT NULL, Note MEMO, CreateDate DATETIME DEFAULT Now(), ToFundID, ToBankID, ReceiptPath TEXT(255) | 49 |
| Personnel | PersonnelID PK, Title TEXT(50), FirstName, LastName, FullName TEXT(255), PersonType TEXT(50), PositionID, Phone TEXT(50) | 50 |
| Positions | PositionID PK, PositionName TEXT(100), PositionGroup TEXT(20) (added by migration) | 51, 68-77 |

Migrations (lines 63-66): MigratePositions (+PositionGroup), MigrateTempleSetting (+3 PersonnelID cols, backfill from legacy AbbotName/WaiyawatName/BookkeeperName or AbbotID…, then DROP 9 legacy cols — lines 87-131), MigratePersonnel (+PositionID + backfill from PersonType, lines 134-153), MigrateTransactions (+ToFundID, ToBankID, ReceiptPath, TransactionGUID, ReceiveChannel, ReferenceNo — lines 156-184). All migrations wrapped in their own Try/Catch → AppPaths.LogCrash (non-fatal).
Seeds (only when empty): Categories 10 (lines 187-211), Funds 4 (213-220), BankAccounts 1 (222-230), Positions 19 (232-260), Personnel 6 samples (262-297).

**No FOREIGN KEY constraints anywhere in Access schema** (no REFERENCES in any CREATE TABLE) — referential integrity is application-level only. SQLite Saddhiviharika does declare FKs (SaddhiviharikaDb.vb:46-68) with PRAGMA foreign_keys=ON (SaddhiviharikaDb.vb:24-28).

## REAL database (read-only copy in V9_survey/tmp/dbcopy.accdb, opened with pyodbc ReadOnly) — RUNTIME evidence
13 tables + row counts (2026-10-23 copy of Database/TempleAccounting.accdb):
| Table | Rows |
|---|---|
| BankAccounts | 2 |
| Categories | 21 |
| District | 930 |
| **ExportHistory** | 0 |
| Funds | 6 |
| **ImportHistory** | 0 |
| **LocalSettings** | 0 |
| Personnel | 6 |
| Positions | 19 |
| Province | 77 |
| SubDistrict | 7452 |
| TempleSetting | 1 |
| Transactions | 110 |

### Differences real-DB vs EnsureSchema (IMPORTANT):
1. **Transactions has 2 extra columns not created by EnsureSchema**: `ImportBatchID GUID`, `SourceMachineID VARCHAR(50)` — added ad-hoc by FrmMultiMachineImport (FrmMultiMachineImport.vb:1272,1280: `ALTER TABLE Transactions ADD COLUMN ImportBatchID GUID` / `SourceMachineID TEXT(50)`), i.e. schema drift lives in a FORM, not the schema module.
2. **TempleSetting has extra column `UsePromptPay BIT NOT NULL`** — NOT in EnsureSchema CREATE, and NO code ALTERs it (grep found no `ADD COLUMN UsePromptPay`). ⇒ On a **fresh database** this column will NOT exist. Code avoids crashing only because FrmTempleSetting's INSERT never names it (FrmTempleSetting.vb:325-327) and chkUsePromptPay is UI-only (FrmTempleSetting.vb:267,310-311). Status: **latent fresh-install bug** (UNVERIFIED whether fresh install path works end-to-end with this column missing; the existing DB has it so app runs today).
3. **3 tables created outside EnsureSchema**: ImportHistory (FrmMultiMachineImport.vb:1251), ExportHistory (FrmExportForTransfer.vb:501), LocalSettings (FrmExportForTransfer.vb:496). Real DB contains all three (0 rows each).
4. TranType in real DB is VARCHAR(255) not TEXT(10) — suggests the table was created by an older/other tool; lengths are not enforced retroactively by Access ALTER. (Observed via pyodbc column metadata; TEXT(10) from CREATE never applied because table already existed.)
- SaddhiviharikaDb (SQLite, AppRoot\Database\Saddhiviharika.db): tables temples, monks, saddhiviharika + 6 indexes, seeded with 5 temples/6 monks/1 sample row (SaddhiviharikaDb.vb:35-96, 105-196). File NOT present in repo root Database/ (only TempleAccounting.accdb + pre_P6 copy) → created on first run in AppRoot\Database; bin copy exists? (bin\Release\Database\ contains only .accdb per listing → SQLite file likely created at runtime.)

## SQL inventory (grep across all .vb, excluding bin/obj/tests) — VERIFIED
Counts of SQL-verb occurrences per file (approximate, string-literal based):
- SELECT hot spots: frmMain.vb 25, OnboardingRepository 21, FrmMultiMachineImport 14, FrmReports 13, FrmTempleSetting 10, …
- INSERT hot spots: FrmMultiMachineImport 6, FrmLocationImport 6, AccessDbProvider 5, SaddhiviharikaDb 4, OnboardingRepository 4, FrmExportForTransfer 4, ExcelTransactionImporter 4, FrmMasterData 3…
- UPDATE hot spots: SqlServerDbProvider 10 (dead), AccessDbProvider 9 (migrations/seeds), FrmTransactions 5, FrmLocationImport 3, AccessTransactionRepository 2, FrmPersonnelManagement 2, FrmExpense 2…
- DELETE (complete list, with WHERE check):
  - **FrmLocationImport.vb:148-150 `DELETE FROM SubDistrict` / `District` / `Province` — NO WHERE, NOT in a transaction** (rebuild path; whole-table wipe then ~8.5k INSERTs, UI-thread). If it fails mid-way, address data is left partial. P1 risk.
  - OnboardingRepository.vb:37 `DELETE FROM TempleSetting` (no WHERE — singleton table; inside transaction line 31, OK) ; OnboardingRepository.vb:190 `DELETE FROM Transactions WHERE TranType='OpeningBalance'` (in transaction line 184).
  - FrmTempleSetting.vb:320 `DELETE FROM TempleSetting` (no WHERE — singleton; inside transaction line 315).
  - WITH WHERE: FrmMasterData 168/199/229 (Categories/Funds/BankAccounts), FrmPersonnelManagement 226/241 (Personnel/Positions), FrmTransactions 462 (in transaction 456).
- **UPDATE without WHERE**: OnboardingRepository.vb:100 `UPDATE TempleSetting SET AbbotPersonnelID=@a, WaiyawatPersonnelID=@w, BookkeeperPersonnelID=@b` — no WHERE (inside transaction line 94). Real DB has 1 TempleSetting row, but if >1 row existed ALL would be overwritten. (AccessTransactionRepository.vb:47's UPDATE DOES have WHERE ID=@id on next line — verified in full read.)
- Non-parameterized / concatenated SQL (flagged):
  - **FrmIncome.vb:60 & FrmExpense.vb:56**: `"SELECT * FROM Transactions WHERE ID = " & id` — id is Integer from grid (not injectable), but unparameterized (inconsistent with house style).
  - ExcelTransactionImporter.vb:197 / FrmMultiMachineImport.vb:590: `SELECT * FROM [" & sheetName & "]"` — sheet names from ClosedXML/OleDb schema; FrmMultiMachineImport uses `safeName` (sanitized — name implies escaping; verify in T12).
  - FrmExportForTransfer.vb:366,377: `INSERT INTO [" & …SheetName & "$]` — fixed constants from TransferFormat, low risk.
  - FrmReports.vb:168-197, 678-679: `… " & fromWhere` — fromWhere built from parameterized fragments (params list passed separately; line 166-169 show ps.ToArray()) → parameters preserved, NOT injectable.
  - All date literals go through Db.AccessDateLiteral (concatenated but culture-invariant #yyyy-MM-dd HH:mm:ss#, by design).
- OleDbTransaction sites (Complete): OnboardingRepository.vb:31/94/184 (Commit 51/106/208, Rollback 53/108/210), FrmTempleSetting.vb:315 (342/344), FrmTransactions.vb:456 (467/469), FrmMultiMachineImport.vb:978 (1077/1081). **Backup/restore code uses NO transactions** (file-level copy by design). See T05 for multi-statement non-transactional list.

## Copies cleanup
dbcopy.accdb in V9_survey/tmp/ will be DELETED at end of survey (required by task rules).
