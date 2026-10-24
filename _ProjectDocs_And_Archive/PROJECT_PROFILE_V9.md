# PROJECT_PROFILE_V9 — Temple Accounting System (ระบบบัญชีวัด)

> **Document version:** V9 (full current-state snapshot + V8→V9 delta)
> **Generated:** 2026-10-23
> **Evidence base:** HEAD = `0c274d5`, branch `modify_backupoct`. Every factual statement cites `<file>:<line>`.
> **Label legend:** VERIFIED (read in code) / RUNTIME (observed by running or on disk) / DOC-ONLY (from V8 text, not re-confirmed) / CONTRADICTED (V8 is wrong at HEAD) / UNVERIFIED (cannot be confirmed without additional evidence).
> **Method:** V8 claims re-verified against the code task-by-task (T01–T18). Companion survey notes live in `_ProjectDocs_And_Archive/V9_survey/T01_*.md` … `T18_*.md`.

---

## 1. Executive Summary & Version History

### 1.1 Executive Summary

Temple Accounting System (lamYangI&E) is a VB.NET / WinForms desktop application for temple income–expense bookkeeping on **MS Access (OLEDB)**, .NET 10 (`net10.0-windows`). It is single-user and fully offline. The V9 survey is a **read-only, evidence-based re-audit**: it confirms the V8 architecture is intact, documents a major post-V8 **backup-system rewrite** (async engine + SHA-256 verification + JSON registry + verify form), and surfaces **one P0 data-integrity defect** and several P1/P2 risks that V8 either missed or mis-stated.

Headline findings:
- **P0 — transaction-facade defect**: `AccessDbProvider` commands never assign `cmd.Transaction`, so three "DELETE then INSERT" saves (`OnboardingRepository.SaveTempleSetting`, `SaveOpeningBalance`, `FrmTempleSetting.btnSave_Click`) **throw every time** and cannot succeed (VERIFIED in code + RUNTIME-proven with a disposable DB). V8's §5.1 "all DELETE→INSERT sequences are atomic" claim is CONTRADICTED.
- **P0/P1 — SeedPositions rewrites 19 rows on every boot** (`AccessDbProvider.vb:232-260`), an unconditional UPDATE that invalidates backup hash-equality and contradicts V8's "seeds only when empty".
- **Backup freeze fixed**: the reported "Not Responding after choosing destination" was a synchronous `BackupFullSystem(...).GetAwaiter().GetResult()` blocking the UI thread; resolved at `f943ecb` (async `Await` + `Task.Run` + progress + cancel). The running build (exe mtime 05:39) includes the fix (RUNTIME).
- **Receipt reconciliation**: 110 transactions, 20 receipt-linked rows, 22 files on disk, **0 missing, 2 orphans** (`Receipt_13.jpg`, `Receipt_56.jpg`).

### 1.2 Version History (highlights)

| Version | Date | Key highlights |
|---|---|---|
| **V9** | 2026-10 | Async backup engine (`BackupEngine` + `Task.Run`/`IProgress`/`CancellationToken`), SHA-256 verification, `backup_registry.json`/`backup_manifest.json` + `manifest.json/csv` + `backup.log`, `FrmBackupVerify` (ตรวจสอบ Backup), `BackupLegacyEvaluator`/`BackupStatusBuilder`, 26 backup tests; freeze root cause fixed (`f943ecb`); P0 transaction defect + SeedPositions churn identified |
| **V8** | 2026-09 | 5-step Onboarding Wizard; tiered Weekly×8/Monthly×15/Yearly×5 FIFO backup; centralized `Backup/Receipts/` MD5 sync; `ErrorLogHelper`; transactional temple-settings save (claimed); UTF-8 BOM enforced (claimed) |
| V7 | 2026-08 | Amber metric strip, gold header, fixed 10-row grid, unified action row, FrmIncome↔FrmExpense parity | *(DOC-ONLY)* |
| V6 | 2026-08 | FrmChartPreview (zoom/pan/toggle/print/save), preview-first PDF + signature block, page balancing, centralized ReportEngine | *(DOC-ONLY)* |
| V5 | 2026-08 | FrmMultiMachineImport DPI rebuild, print hardening, `btnSavePdf` | *(DOC-ONLY)* |
| V4 | 2026-08 | 8-button toolbar baseline, 12-item known-issue log | *(DOC-ONLY)* |
| V3 | 2026-08 | UiFitter, personnel-ID migration | *(DOC-ONLY)* |
| V2 | 2026-07 | Post-refactor state, F1 Help System | *(DOC-ONLY)* |

---

## 2. Architecture & Tech Stack (VERIFIED)

| Layer | Technology | Evidence |
|---|---|---|
| Language | VB.NET; `Option Strict Off` / `Option Explicit On` / `Option Compare Binary` / `Option Infer On` | `TempleAccounting.vbproj:16-19` |
| Runtime | `net10.0-windows`, WinExe, `StartupObject = TempleAccounting.Program` | `TempleAccounting.vbproj:4-8` |
| SDK style | `Microsoft.NET.Sdk`, `EnableDefaultCompileItems=false` (explicit `<Compile>` only) | `TempleAccounting.vbproj:1,20-22` |
| UI | WinForms, panel-hosted (non-MDI); `Application.SetHighDpiMode(PerMonitorV2)`; font Tahoma | `Program.vb:96-105` |
| DB (primary) | MS Access `.accdb`, OLEDB `Microsoft.ACE.OLEDB.12.0`, no password, `Persist Security Info=False` | `AccessDbProvider.vb:20-23` |
| DB (secondary) | SQLite `Saddhiviharika.db` (ordination registry — dormant, see §13) | `SaddhiviharikaDb.vb` |
| Data layer | `Db` facade → `IDbProvider`/`DbProviderFactory` (Access default; SqlServer provider dead) | `Database.vb:16-19`; `DbProviderFactory.vb:29-59` |
| Excel | ACE OLEDB import/export; `ClosedXML` 0.104.2 for multi-machine export | `TempleAccounting.vbproj` |
| Reports | GDI+ `PrintDocument` (`IncomeExpenseReport`) + `ReportEngine`; charts via `WinForms.DataVisualization` | `Reports/*.vb` |
| Help | `USER_MANUAL_TH.md` + `HelpSystem`/`FrmHelpDialog` (F1) | `AppPaths.vb:180-185` |

**NuGet packages** (`TempleAccounting.vbproj`): `Microsoft.Data.SqlClient 6.0.2`, `Microsoft.Data.Sqlite 10.0.11`, `System.Data.OleDb 10.0.10`, `WinForms.DataVisualization 1.10.2`, `ClosedXML 0.104.2`.

---

## 3. Repository Inventory & Git State

### 3.1 Git state (VERIFIED)

| Item | Value |
|---|---|
| Current branch | `modify_backupoct` (user premise "modify_incomeform" CONTRADICTED) |
| Uncommitted changes | 0 (clean tree) |
| Unpushed commits | 398 vs `origin/main` (branch exists only locally) |
| Total commits | 400 |
| Tags | `v1.0.0` (23adbc0), `v1.1.0-backup-complete` (720dc07) |
| V8 snapshot | `91d2a5e` ("Major audit, docs, and backup/reporting updates"); 2 commits after (`f943ecb`, `fdeb598`) |
| Files changed since V8 | 8 files, +1583/−285 (BackupEngine.vb, BackupLegacyEvaluator.vb **new**, BackupRegistry.vb, DatabaseBackupHelper.vb, Forms/FrmBackupVerify.vb, TempleAccounting.vbproj, frmMain.UiHelpers.vb, frmMain.vb) |

### 3.2 Scale (VERIFIED)

- Main project: **26,077 VB lines across 66 files** (excl. bin/obj/tools/BackupTests).
- 150 tracked files; 384 files on disk excluding `.git`.
- Largest files: `FrmMultiMachineImport.vb` 1355, `ReportEngine.vb` 1141, `frmMain.vb` 1090, `FrmOnboardingWizard.Designer.vb` 1052, `IncomeExpenseReport.vb` 990, `frmMain.Designer.vb` 957, `FrmTransactions.vb` 955, `FrmTempleSetting.Designer.vb` 873, `DatabaseBackupHelper.vb` 857, `FrmReports.vb` 825.
- `.gitignore` `Backup*/` rule hides **both** backup folders **and** the `BackupTests/` project (untracked — risk of losing test code).

---

## 4. Startup, Paths, Configuration

### 4.1 Boot sequence (`Program.vb:59-195`, VERIFIED)

| Step | Action | Note |
|---|---|---|
| 1 | Register `UnhandledException` + `ThreadException` handlers | `Program.vb:61-93` |
| 2 | `EnableVisualStyles`, `SetCompatibleTextRenderingDefault(False)`, `SetHighDpiMode(PerMonitorV2)` | `Program.vb:96-105` |
| 2b | `SetCurrentProcessExplicitAppUserModelID("LamyangIE.TempleAccounting.App.v1")` | `Program.vb:107-115` |
| 3 | Touch `AppPaths.DatabaseFile`; missing → log only (does NOT stop) | `Program.vb:117-125` |
| 4 | `Db.EnsureSchema()`; failure → ACE warning dialog, **continues** | `Program.vb:127-141` |
| 4c | `SaddhiviharikaDb.EnsureSchema()` (SQLite); failure logged only | `Program.vb:144-149` |
| 4b | `AppPaths.MigrateReceiptsToMaster()` | `Program.vb:152-157` |
| 4d | Onboarding gate: `If Not AppPreferences.IsOnboardingCompleted Then` → modal wizard; `<> OK → Return` | `Program.vb:160-169` |
| 5 | `New frmMain()` (CenterScreen→Maximized) | `Program.vb:171-185` |
| 6 | `Application.Run(mainForm)` | `Program.vb:187-195` |

### 4.2 Database file name — resolved (VERIFIED)

Live DB file = **`TempleAccounting.accdb`** (`AppPaths.vb:84,145,157`). `TempleDB_*.accdb` is the **legacy backup-copy naming** used only inside backup folders produced by old `BackupFullSystem`. The running app resolves `<root>\Database\TempleAccounting.accdb` (project-root aware via `FindPreferredDatabaseFile`, `AppPaths.vb:128-163`).

### 4.3 `user_preferences.ini` (VERIFIED)

`{AppRoot}\user_preferences.ini`, 5 keys: `LastExpenseCategoryID`, `LastExpenseFundID`, `LastIncomeCategoryID`, `LastIncomeFundID`, `IsOnboardingCompleted` (default **True** — existing installs never see the wizard). **No backup-destination/root keys exist**; the custom destination is re-prompted each click and never persisted (`frmMain.vb:894-906`).

### 4.4 Hard-coded paths (VERIFIED)

No absolute drive paths in source (0 matches for `C:\`/`D:\`). Exceptions: `C:\LineDownloads` receipt-browse initial dir in `FrmIncome.vb:617-619` and `FrmExpense.vb:574-576` (guarded by `Directory.Exists` but literal). All roots resolve via `AppRoot`/`ProjectRoot`/`%AppData%`/user choice.

---

## 5. Database Schema & Data-Integrity Rules

### 5.1 Schema (VERIFIED vs real DB — RUNTIME)

`Db.EnsureSchema()` (`AccessDbProvider.vb:38-66`) creates 10 tables + migrations. Real DB copy (`V9_survey/tmp/dbcopy.accdb`, opened read-only) has **13 tables**:

| Table | Rows |
|---|---|
| BankAccounts | 2 |
| Categories | 21 |
| District | 930 |
| ExportHistory | 0 |
| Funds | 6 |
| ImportHistory | 0 |
| LocalSettings | 0 |
| Personnel | 6 |
| Positions | 19 |
| Province | 77 |
| SubDistrict | 7452 |
| TempleSetting | 1 |
| Transactions | 110 |

**Schema drift (real DB vs `EnsureSchema`) — key integrity findings:**

1. `Transactions` has 2 extra columns `ImportBatchID GUID`, `SourceMachineID VARCHAR(50)` added ad-hoc by `FrmMultiMachineImport` (`FrmMultiMachineImport.vb:1272,1280`) — schema drift lives in a form, not the schema module.
2. `TempleSetting.UsePromptPay BIT NOT NULL` exists in the DB but is **not** in `EnsureSchema` and has no ALTER anywhere → **latent fresh-install bug** (column absent on a new DB; only masked because `FrmTempleSetting` never names it).
3. Three tables created outside `EnsureSchema`: `ImportHistory`, `ExportHistory`, `LocalSettings`.
4. `TranType` in real DB is VARCHAR(255), not TEXT(10) (table predates the CREATE).
5. **No FOREIGN KEY constraints anywhere** in the Access schema (`REFERENCES` appears only in `SaddhiviharikaDb.vb`).

### 5.2 Transaction correctness — P0 (VERIFIED + RUNTIME)

`AccessDbProvider.ExecuteNonQuery/DbScalar/GetTable/InsertAndGetId` create commands via `conn.CreateCommand()` and **never assign `cmd.Transaction`** (`AccessDbProvider.vb:305-345`). OleDb enforces assignment: a command run while a transaction is pending on the connection throws "The Transaction property of the command has not been initialized." (RUNTIME-proven against the disposable DB copy).

Affected sites (all **BROKEN — save fails every time**):

| Site | Pattern | Evidence |
|---|---|---|
| `OnboardingRepository.SaveTempleSetting` | DELETE w/ tx + `InsertAndGetId` w/o tx | `OnboardingRepository.vb:31-57` |
| `OnboardingRepository.SaveOpeningBalance` | DELETE w/ tx + 2× `InsertAndGetId` w/o tx | `OnboardingRepository.vb:184-216` |
| `FrmTempleSetting.btnSave_Click` | DELETE w/ tx + `InsertAndGetId` w/o tx | `FrmTempleSetting.vb:315-344` |

Correct sites (assign tx): `LinkPersonnelToTempleSetting`, `FrmTransactions` delete, `FrmMultiMachineImport` (via `ExecInTx`/`ScalarInTx` helpers).

### 5.3 Non-transactional multi-statement writes (VERIFIED)

1. `FrmLocationImport` rebuild: 3× `DELETE FROM Province/District/SubDistrict` (no WHERE) then ~8,459 INSERTs, **no transaction** (`FrmLocationImport.vb:148-150`).
2. FrmIncome/FrmExpense save: INSERT + separate `UpdateReceiptPath` on separate connections.
3. `AccessDbProvider` seeds (each row its own command).
4. `SeedPositions` — unconditional `UPDATE Positions` ×19 rows every boot (`AccessDbProvider.vb:232-260`).

### 5.4 Dates (VERIFIED)

Date literals correctly go through `Db.AccessDateLiteral` (InvariantCulture `#yyyy-MM-dd HH:mm:ss#`). `NormalizeGregorianDate` guards `>2400 − 543`. Machine locale is `th-TH` (Buddhist calendar) — evidenced by log timestamps `2569-…`. ~19 culture-sensitive `ToString("yyyy…")` sites remain (see §12).

---

## 6. Modules / Forms Catalog & Navigation Map

### 6.1 Form catalog (VERIFIED, 17 forms + frmMain partial)

| Form | Base | AutoScaleMode | F1 | UiFitter | Tables touched |
|---|---|---|---|---|---|
| frmMain (+UiHelpers+Designer) | Form | Font | Y | Y | Transactions, Categories, Funds, BankAccounts, TempleSetting, Personnel, Positions |
| FrmIncome | Form | unset | Y | Y | Transactions, Categories, Funds, BankAccounts |
| FrmExpense | Form | unset | Y | Y | Transactions, Categories, Funds, BankAccounts |
| FrmTransactions | Form | Font | Y | Y | Transactions, Categories, Funds, BankAccounts |
| FrmTransfer | Form | unset | Y | Y | Transactions, Funds, BankAccounts |
| FrmReports | Form | Font | Y | Y | Transactions, Categories, Funds, BankAccounts |
| FrmChartPreview | Form (programmatic) | Dpi (code) | Y | – | via ReportEngine |
| FrmTempleSetting | Form | Dpi | Y | Y | TempleSetting, Personnel, Positions, Province, District, SubDistrict |
| FrmPersonnelManagement | Form | Font | Y | Y | Personnel, Positions |
| FrmMasterData | Form | unset | Y | Y | Categories, Funds, BankAccounts |
| FrmLocationImport | Form | unset | Y | Y | Province, District, SubDistrict |
| FrmMultiMachineImport | Form | Dpi | Y | Y | Transactions, Categories, Funds, BankAccounts, ImportHistory |
| FrmExportForTransfer | Form | Dpi | Y | Y | Transactions, Categories, Funds, BankAccounts, ExportHistory, LocalSettings |
| FrmOnboardingWizard | Form | Dpi | – | Y | via OnboardingRepository |
| FrmHelpDialog | Form | Font | n/a | Y | none |
| FrmReceiptViewer | Form | Font | – | – | none |
| FrmBackupVerify (NEW) | Form (programmatic) | unset | – | – | via DatabaseBackupHelper/registry |

**AutoScaleMode**: Dpi ×5, Font ×6, **unset ×6** (V8 "mixed" claim VERIFIED). **F1 missing**: FrmReceiptViewer, FrmOnboardingWizard, FrmBackupVerify.

### 6.2 Navigation map (VERIFIED)

Panel-hosted via `frmMain.ShowFormInPanel` (`frmMain.vb:406-433`): btnDashboard→dashboard, btnDonation→FrmIncome, btnExpense→FrmExpense, btnReport→FrmReports, btnMember→FrmTransactions, btnVip→FrmTempleSetting, btnActivity→FrmTransfer, btnSetting→FrmMasterData. **Modal**: btnMultiImport→FrmMultiMachineImport, btnExportTransfer→FrmExportForTransfer, btnVerifyBackup→FrmBackupVerify.

**Dev-only buttons** ("🧪 ทดลอง Onboarding", "🔄 รีเซ็ต Onboarding") are created at runtime in `SetupDeveloperTools` with **no `#If DEBUG` guard** — visible and functional in Release (`frmMain.UiHelpers.vb:14-65`).

---

## 7. Backup & Restore System

Two generations coexist and compose: the **new engine family** (`BackupConfig`, `BackupEngine`, `BackupRegistry`, `BackupLegacyEvaluator`+`BackupStatusBuilder`, `BackupHashing`) and the **legacy `DatabaseBackupHelper`** (tiered close backup, restore, receipt MD5 sync, scan orchestration).

### 7.1 Entry-point call-chain table (VERIFIED)

| Entry point | Thread | Backs up | Destination | Verification | Error handling |
|---|---|---|---|---|---|
| 💾 btnBackup → `RunBackupAsync` → `BackupFullSystemAsync` → `BackupEngine.ExecuteBackupAsync` | background (`Task.Run`) | DB + full Receipts | `{chosen}\Backup_{ts}` (InvariantCulture) | **SHA-256 + size + existence**; manifest.json/csv + backup.log | Retry/Cancel (≤3), CTS cancel, exceptions logged + dialog |
| 🔄 btnRestore → `RestoreFullSystem` | **UI (sync)** | restore DB + receipts | overwrites DB + ReceiptsDir | **none** (returns True on no exception) | safety copy → rollback; `Application.Restart()` |
| 🔍 btnVerifyBackup → `FrmBackupVerify` | background (`Task.Run`) | read-only re-hash + scan | writes only registry | SHA-256 + Access header + receipt completeness | per-set try/catch, cancel token |
| `frmMain.FormClosing` → `RunTieredBackup` | **UI (sync)** | DB only (tier folders) + `Backup\Receipts` MD5 sync | `AppRoot\Backup\{Weekly,Monthly,Yearly}` | **none** | one Try/Catch → silent log |
| `Db.BackupDatabase` (pre-import/merge) | UI | DB only | `AppRoot\Backup\TempleAccounting_{ts}.accdb` | **none** | propagates to caller |
| Startup / scheduled | — | **none** (no scheduler/timer/autobackup) | — | — | — |

### 7.2 Answers T06 (a)–(g) (VERIFIED unless RUNTIME)

- **a) Receipts copied?** YES: 💾 engine; FormClosing tier (but only to local `Backup\Receipts`); restore. NO: tier `.accdb` copies; `Db.BackupDatabase`; scan/verify.
- **b) Why `D:\…\Backup_25691014_102801` had only `.accdb`**: old `BackupFullSystem` copied only the DB into the set and sent receipts to local `Backup\Receipts`; Buddhist-year name from culture-sensitive `ToString` (no InvariantCulture); reported "สำรองข้อมูลครบถ้วนแล้ว!" with zero verification.
- **c) Custom destination**: `FolderBrowserDialog` → `RunBackupAsync(selectedPath)` → `BackupFullSystemAsync` → `FindContainmentProblem` refuses destinations inside DB/Receipts (`BackupEngine.vb:133-157`). Not persisted.
- **d) Two implementations?** Yes, but now they **compose**; duplicated/dead: `BackupConfig.BackupRootPaths` (defined, never referenced); MD5 vs SHA-256 hash stacks; `CopyDirectory` vs engine copy; two manifest schemas (`manifest.json` vs `backup_manifest.json`).
- **e) Verification?** 💾 and 🔍 YES (SHA-256). Tier close, `SyncReceiptsToBackup`, `RestoreFullSystem`, `Db.BackupDatabase` **no verification**; old code reported success with none.
- **f) Registry**: `%AppData%\TempleAccounting\backup_registry.json`, UTF-8 no-BOM. **5 entries right now** (RUNTIME). Entries 1–2 registered by the 05:39:47 scan, not their own run → **the engine's own `AddEntry` path has never been observed to run** (UNVERIFIED).
- **g) Retention**: tiers Weekly≤8/Monthly≤15/Yearly≤5, cleanup sorts by filename and **has no completeness interlock** (could delete an old DB-only tier file). New engine sets are append-only with **no retention**.

### 7.3 Disk backup inventory (RUNTIME, read-only)

| Location | Contents | Receipts | manifest/log |
|---|---|---|---|
| `D:\สำรองข้อมูลบัญชี\Backup_20261023_045135` | 26 files (accdb + 22 receipts + manifest+csv+log) | YES | YES |
| `D:\สำรองข้อมูลบัญชี\Backup_20261022_214049` | same shape | YES | YES |
| `D:\สำรองข้อมูลบัญชี\Backup_25690804_145723` (+2 nested) | 34 files, 4 accdb | parent/nested yes; `102801` **no** | none |
| `bin\Release\…\Backup\` | ~175 legacy `Backup_2569*` sets + Weekly(8)/Monthly(3)/Receipts(22) | yes in sets; no in tiers | none |

**Completeness verdict**: two complete, verified sets exist on `D:\` (23 files each: DB + 22 receipts + SHA-256 manifest). The ~181 local sets are **not registered** because `ScanForBackupFolders` runs at maxDepth=3 from the drive root (`BackupEngine.vb:434-448`) while the app's own backup tree sits at depth ≥6 — the 🔍 "scan all drives" feature cannot see the machine's default backups.

### 7.4 SeedPositions bonus finding (VERIFIED)

`AccessDbProvider.SeedPositions` runs on every `EnsureSchema` and unconditionally `UPDATE Positions SET PositionGroup=@g WHERE PositionName=@n` for all 19 rows (`AccessDbProvider.vb:232-260`). DB mtime = boot time (RUNTIME). Consequence: every boot invalidates backup hash-equality → complete backups carry "ฐานข้อมูลถูกแก้ไขหลังการสำรอง" problems even with no user edits. V8's "seeds only when empty" is CONTRADICTED for Positions.

---

## 8. UI-Thread / Freeze Findings (ranked)

Global facts (VERIFIED): `Thread.Sleep` ×1 (`BackupEngine.vb:588`, non-cancellable path only); `.Result`/`.Wait`/`GetAwaiter().GetResult()` ×0 at HEAD; `Task.Run` only in `BackupEngine.vb` and `FrmBackupVerify.vb`; **every other long operation is on the UI thread**.

| Rank | Root cause | Status | Evidence |
|---|---|---|---|
| 1 | Manual backup synchronous on UI thread (raw copy ≤`3cf6363`; `.GetAwaiter().GetResult()` `9d011b1`–`91d2a5e`) — freeze starts after destination chosen | **FIXED** `f943ecb` | `git show 91d2a5e:DatabaseBackupHelper.vb:23-24`; `frmMain.vb:909,924` |
| 2 | Excel import sync on UI thread (ClosedXML + per-row INSERT + pre-import copy + GC) | ACTIVE P1 | `FrmIncome.vb:602`; `ExcelTransactionImporter.vb:87,143-153` |
| 3 | Multi-machine merge sync on UI thread (one tx + per-row receipt staging + pre-merge copy) | ACTIVE P1 | `FrmMultiMachineImport.vb:978-1081` |
| 4 | Location import rebuild sync on UI thread (3 DELETEs + 8.4k INSERTs) | ACTIVE P1 | `FrmLocationImport.vb:136-248` |
| 5 | Restore sync on UI thread | ACTIVE P2 | `frmMain.vb:1030-1056` |
| 6 | FormClosing tiered backup sync + blocking `GC.Collect` | ACTIVE P2 (bounded; cannot wedge close) | `frmMain.vb:1081-1088` |

**FormClosing specifics** (VERIFIED): if `_isBackingUp`, a prompt offers cancel-and-exit vs wait; choosing "No" sets `e.Cancel=True` (flag can block close). `RunTieredBackup()` then runs synchronously on the UI thread; it is double-wrapped in Try/Catch, so **an exception cannot leave the app unable to close** (worst case = delayed close).

---

## 9. Receipts Subsystem & Reconciliation

### 9.1 Subsystem (VERIFIED)

`ReceiptImageHelper` resizes to ≤1600 px (JPEG q80), names `Receipt_{transactionID}.jpg`, stores filename (not path) in `Transactions.ReceiptPath`. `ResolveReceiptPath` fallback chain: master dir → legacy dirs (`AppPaths.vb:287-316`). `MigrateReceiptsToMaster` is non-destructive. The **only** `File.Delete` site is `FrmTransactions.vb:476-484`, wrapped in an empty Catch ("ปล่อยผ่านถ้าไฟล์ถูกล็อค").

### 9.2 Reconciliation (RUNTIME, on DB copy vs `Receipts\`)

| Metric | Value |
|---|---|
| Transactions | 110 (Income 63 / Expense 46 / Transfer 1) |
| Rows with non-empty ReceiptPath | 20 |
| Files `Receipts\*.jpg` | 22 |
| DB row without file (missing) | **0** |
| File without DB row (orphan) | **2**: `Receipt_13.jpg`, `Receipt_56.jpg` |

Orphans confirmed true (`SELECT ID FROM Transactions WHERE ID IN (13,56)` → 0 rows). Cause class = empty-Catch file delete (locked/failed delete) or restore-merge; the specific historical event is UNVERIFIED.

---

## 10. Reports, Import/Export, Master Data

### 10.1 Reports/Charts/Printing (VERIFIED)

- Pipeline: `FrmReports` → `IncomeExpenseReport` (`ShowPreview` / `ShowPreviewWithPdfExport` / `ShowPrint`) → `ReportEngine`.
- **PDF export uses the OS virtual printer "Microsoft Print to PDF"** (`IncomeExpenseReport.vb:911-955`) — no PDF library; synchronous UI-thread print; hard external dependency.
- Page balancing + signature block + charts re-verified (V6 features intact).
- Debt: duplicated Thai-format helpers (drifted — `ToBuddhistYearThai` Integer vs String), empty `dgvReport_CellContentClick` handler, PDF on UI thread.

### 10.2 Import/Export/Multi-machine (VERIFIED)

- Two import paths: single-file `ExcelTransactionImporter` (non-transactional, no `TransactionGUID`, auto-creates master data) vs multi-machine `FrmMultiMachineImport` (single tx, SHA-256 dedup, GUID+composite two-tier dedup, receipt staging, `ImportHistory` inside tx).
- `TransferFormat` centralizes the cross-machine contract (`SchemaVersion "1.0"`).
- 8 helper functions duplicated verbatim between the two importers.

### 10.3 Master data & personnel (VERIFIED + RUNTIME)

- **No FK constraints** (proven: `INSERT` bogus `CategoryID=99999` succeeded; `DELETE` in-use category succeeded). The "ไม่สามารถลบได้ เนื่องจากมีการใช้งานอยู่" message can **never** fire for reference reasons — misleading.
- Consumers degrade via `LEFT JOIN` + NULL fallbacks (blank/'ไม่ระบุ' labels).
- Personnel/Positions delete has no link guard; `TempleSetting` role IDs may dangle (signature block silently loses names).

---

## 11. Logging, Error Handling, Security, Data-Safety

### 11.1 Logging (VERIFIED + RUNTIME)

Two files in `Logs\`: `crash.log` (boot + unhandled) and `error.log` (silent-catch + INFO). **No rotation/size cap** (75 KB and growing). All three writers use culture-sensitive `ToString("yyyy-MM-dd HH:mm:ss")` → **Buddhist-era timestamps** (`2569-10-23 …`, RUNTIME-observed). ~54 empty `Catch` blocks remain (of 255 total).

### 11.2 Security/privacy (VERIFIED)

No network code, no elevation (`asInvoker`), no secrets, parameterized SQL throughout, no macro execution (ACE row cap 200,000). Exposure: **DB + backups + exports unencrypted at rest** (PromptPay ID, personnel phones, financials); unbounded unredacted logs; `ReceiptPath` not sanitized before `File.Delete`/shell-open (P3).

### 11.3 Data-safety copies matrix (VERIFIED + RUNTIME)

| Copy | Location | Complete (DB+receipts)? | Verified? |
|---|---|---|---|
| Source DB | `<root>\Database\TempleAccounting.accdb` | DB only | n/a |
| Engine sets (2) | `D:\สำรองข้อมูลบัญชี\Backup_2026*` | YES | SHA-256 ✓ |
| Legacy sets (~181) | `bin\Release\…\Backup\Backup_2569*` | mostly | none |
| Tier folders | `AppRoot\Backup\{Weekly,Monthly,Yearly}` | DB only | none |
| Central receipts | `AppRoot\Backup\Receipts` | receipts only | MD5-collision-safe |
| Exports | `Export\` / user drive | DB subset + receipts | none |

**Verdict**: at least **two independent, complete, SHA-256-verified copies exist today on `D:\`** (plus the live DB). No offsite/encrypted copy.

---

## 12. Encoding / Localization / UI-Standard Compliance

### 12.1 Encoding (VERIFIED, full-tree scan)

131 text files: 63 UTF-8 with BOM, 68 without, **0 mojibake, 0 decode failures**. V8's "0 violations / 100% BOM" claim is **CONTRADICTED** (68 files still lack BOM; harmless to the SDK compiler but cosmetic VS noise).

### 12.2 Culture (VERIFIED + RUNTIME)

No global culture pinning; app runs in OS culture (`th-TH`). ~19 culture-sensitive `ToString("yyyy…")` sites produce Buddhist-era text: log timestamps, `Report_*.csv` names (`FrmReports.vb:700`), `Db.BackupDatabase` pre-merge names (`AccessDbProvider.vb:361`), export folder names (`FrmExportForTransfer.vb:224`), `BackupEngine` manifest `Created` (`BackupEngine.vb:623,658`). Tiered + new-engine backup folder names are culture-safe (InvariantCulture/integer properties).

### 12.3 UI standards (VERIFIED)

Mixed `AutoScaleMode` (Dpi 5 / Font 6 / unset 6); Tahoma dominant (×326); `UiFitter` on 14 forms; F1 gaps on 3 forms. In-code comment "ห้าม Font" (never Font) states the intended Dpi-only policy, which has never been applied project-wide.

---

## 13. Technical Debt, Dead Code, V8 Outstanding-Items Status

### 13.1 Dead/dormant (VERIFIED)

- `Saddhiviharika` registry: boot-initialized every run, **write-never-read from UI**.
- `SqlServerDbProvider` (482 lines): fully implemented but never selected.
- `BaseReport`: removed from code; survives only in 10 markdown docs.
- `_testReportViewed` (wizard Step 5), `dgvReport_CellContentClick` (empty handler), `BackupConfig.BackupRootPaths` (never referenced).
- Stale `.sql` scripts (`CreateImportHistory.sql`, `CreateTransferSchema.sql`) superseded by in-code `EnsureImportSchema`/`EnsureExportSchema`.

### 13.2 Duplication (VERIFIED)

FrmIncome↔FrmExpense ~79% identical; 8 Excel helpers duplicated; Thai-format helpers ×2 (drifted); ReceiptPath update ×2; two MessageBox systems; two log files. Root cause: repository pattern consumed by **only one** form (`FrmIncome.vb:23`).

### 13.3 V8 outstanding items status

| # | V8 outstanding item | V9 status |
|---|---|---|
| 1 | `FrmTransactions.SaveSelectedRowEdits` dead code | UNVERIFIED (not re-audited this session) |
| 2 | `FrmLocationImport` naive CSV parser | still open (parser unchanged; rebuild also non-transactional) |
| 3 | Personnel title-gluing no space; Positions delete lacks FK message | delete-guard confirmed absent (P2); title-gluing UNVERIFIED |
| 4 | `LoadMonkSample` reads legacy TempleSetting columns | UNVERIFIED (not re-audited) |
| 5 | `WizardText.Step3Title` mismatch | **STILL PRESENT** (`WizardText.vb:23`) |
| 6 | `BaseReport` dead scaffolding; AutoScaleMode mixed | BaseReport **RESOLVED** (removed); AutoScaleMode still mixed |

---

## 14. V8 → V9 Delta

### Added (new since V8)

- `BackupConfig.vb`, `BackupEngine.vb`, `BackupRegistry.vb`, `BackupLegacyEvaluator.vb` (incl. `BackupStatusBuilder`), `BackupHashing`, `Forms/FrmBackupVerify.vb`.
- SHA-256 verification, `backup_registry.json`, `backup_manifest.json`, `manifest.json`/`manifest.csv`/`backup.log`.
- Async backup (Task.Run + `IProgress` + `CancellationToken`), progress UI + cancel button.
- `BackupTests/` project (gitignored, 26 passing tests).

### Changed

- Manual backup: sync `BackupFullSystem` → async `BackupFullSystemAsync` (`f943ecb`).
- `frmMain`: `RunBackupAsync`, progress panel, cancel, FormClosing `_isBackingUp` guard, `IsDisposed` checks.
- `DatabaseBackupHelper.vb` +426/− lines; `frmMain.vb` +157.

### Removed

- Synchronous `BackupFullSystem` wrapper (`f943ecb`).
- `BaseReport` class (removed before `91d2a5e`).

### Contradicted (V8 wrong/outdated at HEAD)

1. Branch name "modify_incomeform" → actual `modify_backupoct`.
2. "Seeds only when empty" → `SeedPositions` rewrites 19 rows every boot.
3. "All DELETE→INSERT sequences are atomic" → 3 sites broken (P0).
4. "BaseReport remains dead scaffolding" → already removed.
5. "UTF-8 BOM 0 violations" → 68/131 files still lack BOM.

---

## 15. Prioritized Risk & Action List (suggested fixes in words only — no code changes)

### P0 — critical

1. **Transaction-facade defect breaks saves.** `AccessDbProvider` commands don't assign `cmd.Transaction`; three DELETE+INSERT saves throw and fail (TempleSetting, wizard Step 1/Step 4). Evidence: `AccessDbProvider.vb:305-345`, `OnboardingRepository.vb:31-57,184-216`, `FrmTempleSetting.vb:315-344` (RUNTIME-proven). *Fix (words): make the facade assign the ambient transaction to every command it creates, or stop mixing explicit-transaction commands with facade `InsertAndGetId` inside a transaction scope.*

2. **SeedPositions rewrites data every boot.** Unconditional `UPDATE Positions` ×19 rows on each `EnsureSchema`, invalidating backup hash-equality. Evidence: `AccessDbProvider.vb:232-260`. *Fix (words): guard the UPDATE to run only when a position is missing its `PositionGroup`, matching the other seeders' existence checks.*

### P1 — high

3. **UI-thread freeze risks remain outside backup**: Excel import (`ExcelTransactionImporter.vb:87,143-153`), multi-machine merge (`FrmMultiMachineImport.vb:978-1081`), location rebuild (`FrmLocationImport.vb:136-248`). *Fix: move heavy loops to `Task.Run` with progress + cancel, mirroring the fixed backup path.*
4. **No referential integrity**: in-use master/personnel deletes succeed; misleading "in use" message. *Fix: pre-delete `COUNT(*)` reference guards in FrmMasterData/FrmPersonnelManagement.*
5. **Non-transactional single-file Excel import** (partial import on crash; no `TransactionGUID`). *Fix: wrap in one transaction and write `TransactionGUID`/`ImportBatchID`.*
6. **FrmLocationImport whole-table DELETE is non-transactional**; failure leaves truncated address tables. *Fix: transaction + staging.*
7. **Registry `AddEntry` never observed at runtime** — own-backup registration path unverified. *Fix: verify with a manual 💾 backup and assert a registry entry is written.*
8. **Scan depth ≤3 hides local backups** (`BackupEngine.vb:434-448`). *Fix: raise depth or scan the known `AppRoot\Backup` tree explicitly.*
9. **Tier retention has no completeness interlock** (`DatabaseBackupHelper.vb:826-851`). *Fix: consult registry/status before deleting any tier file.*

### P2 — medium

10. **Data at rest unencrypted** (DB/backups/exports contain PromptPay ID, phones, financials). *Fix: document + optionally Access DB encryption / BitLocker guidance.*
11. **Unbounded, unredacted, Buddhist-timestamped logs.** *Fix: rotation/size cap + InvariantCulture timestamps.*
12. **~19 culture-sensitive date formats** → Buddhist-era filenames/logs. *Fix: InvariantCulture on all `ToString("yyyy…")` serialization sites.*
13. **Mixed AutoScaleMode** (Dpi 5 / Font 6 / unset 6). *Fix: apply the intended Dpi-only policy project-wide.*
14. **Restore and FormClosing tiered backup on UI thread** + blocking `GC.Collect`. *Fix: async restore; background close backup with progress.*
15. **Step3Title mismatch + postcode round-trip bug** (wizard). *Fix: correct title; persist/load PostCode from the correct column.*

### P3 — low

Empty catches (~54); two dialog/log systems; README 46 bytes; stale skills; no CI; dead code (`_testReportViewed`, empty handler, dead `BackupConfig.BackupRootPaths`); hard-coded `C:\LineDownloads`; hard-coded fallback temple identity in `ReportEngine`; `ReceiptPath` unsanitized.

---

## 16. Appendix

### 16.1 File-by-file table (key files)

| File | LOC | Role |
|---|---|---|
| frmMain.vb / .Designer / .UiHelpers | 1090 / 957 / 284 | shell, nav, backup/restore buttons, FormClosing |
| Forms/FrmMultiMachineImport.vb | 1355 | cross-machine merge |
| Reports/ReportEngine.vb | 1141 | shared report engine + Thai formatting |
| Reports/IncomeExpenseReport.vb | 990 | GDI+ print/PDF |
| Forms/FrmTransactions.vb | 955 | ledger CRUD |
| DatabaseBackupHelper.vb | 857 | tiered backup, restore, receipt sync, scan orchestration |
| Forms/FrmReports.vb | 825 | report center |
| Forms/FrmOnboardingWizard.vb / .Designer | 819 / 1052 | 5-step wizard |
| BackupLegacyEvaluator.vb | 777 | backup evaluation/classification/status |
| BackupEngine.vb | 686 | async copy + SHA-256 verify + manifest |
| Forms/FrmIncome.vb / FrmExpense.vb | 666 / 622 | entry forms (~79% duplicated) |
| Forms/FrmExportForTransfer.vb | 612 | transfer export |
| Forms/FrmChartPreview.vb | 509 | chart viewer |
| Forms/FrmBackupVerify.vb | 439 | backup verify/scan form |
| AccessDbProvider.vb / Database.vb / SqlServerDbProvider.vb | — | data layer |

### 16.2 SQL inventory (summary)

SELECT hot spots: frmMain 25, OnboardingRepository 21, FrmMultiMachineImport 14, FrmReports 13, FrmTempleSetting 10. DELETE without WHERE: `FrmLocationImport.vb:148-150` (3 tables, non-transactional); `OnboardingRepository.vb:37,190`; `FrmTempleSetting.vb:320`. UPDATE without WHERE: `OnboardingRepository.vb:100`; `AccessDbProvider.vb:252-256` (SeedPositions). Non-parameterized: `FrmIncome.vb:60`/`FrmExpense.vb:56` (`WHERE ID = " & id`, Integer — type-safe). Transaction sites (complete): 6 `BeginTransaction`, of which 3 are broken (see §5.2).

### 16.3 Backup entry-point table

See §7.1.

### 16.4 UNVERIFIED items (and what is needed to verify)

1. **Fresh-install wizard end-to-end save failure** — needs a fresh `user_preferences.ini`/DB run (mechanism proven, full flow untested).
2. **Whether the user attempted a TempleSetting save since `05d5f18`** — needs error.log/message evidence.
3. **`BackupFullSystemAsync` own `AddEntry` (registry write) at runtime** — needs a manual 💾 backup run.
4. **Legacy Buddhist folder parse/sort correctness at runtime** — unit test exists; needs UI scan against real `Backup_2569*` sets.
5. **Unset `AutoScaleMode` effective runtime default** — needs a high-DPI display test.
6. **Historical cause of the 2 orphan receipts** — needs forensics on the delete/restore event.
7. **Backup manifest `.Created` Buddhist text** — code defect VERIFIED; actual `manifest.json` contents not opened.
8. **`FrmTransactions.SaveSelectedRowEdits` dead-code status** — not re-audited this session.
9. **ACE OLEDB 12.0 provider present on this machine** — required at runtime; not verified this session.

---

## Top-10 Risk Summary

1. **P0 — Transaction-facade defect**: `cmd.Transaction` never assigned in `AccessDbProvider`, so TempleSetting save and wizard Step 1/Step 4 saves throw and fail every time.
2. **P0 — SeedPositions rewrites 19 rows on every boot**, invalidating backup hash-equality and contradicting "seeds only when empty".
3. **P1 — UI-thread freeze risks remain** in Excel import, multi-machine merge, and location rebuild (the backup freeze was fixed, but the same pattern persists elsewhere).
4. **P1 — No foreign-key constraints**: in-use master/personnel rows delete silently with a misleading "in use" message.
5. **P1 — Non-transactional single-file Excel import** and non-transactional whole-table location rebuild can leave partial data.
6. **P1 — Registry own-backup registration never observed**; scan depth ≤3 hides the machine's own local backups from the verify feature.
7. **P1 — Tier retention deletes by count with no completeness interlock**.
8. **P2 — Data at rest unencrypted** (PromptPay IDs, phones, financials in DB/backups/exports).
9. **P2 — Culture-sensitive date formats (~19 sites)** produce Buddhist-era filenames and log timestamps on `th-TH` machines.
10. **P2 — Unbounded unredacted logs** + mixed AutoScaleMode + missing F1 on 3 forms.

---

## Label tally

Counts of the explicit non-verified labels and verified-claim scope used in this document:

| Label | Count |
|---|---|
| VERIFIED | default for every `<file>:<line>`-cited statement in §2–§13 (numbering in the hundreds, not individually tallied) |
| RUNTIME | 10 (build failure, locale, DB copy/schema-drift, reconciliation, registry entries, disk inventory, 26 tests, log contents, running-exe mtime, FK probes) |
| DOC-ONLY | 6 (V2–V7 version-history rows carried from V8 text) |
| CONTRADICTED | 5 (branch name, seeds-only-when-empty, transactional-saves, BaseReport, UTF-8 BOM) |
| UNVERIFIED | 9 (see §16.4) |
