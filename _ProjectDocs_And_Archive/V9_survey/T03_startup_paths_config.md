T03 — Startup, paths, configuration flow (V9 survey)
Generated: 2026-10-23. VERIFIED = read in code this session.

## Program.vb boot sequence (VERIFIED, Program.vb lines cited)
Module `Program`, `<STAThread> Public Sub Main()` (Program.vb:59-60):
| Step | What | Evidence |
|---|---|---|
| 1 | Register `AppDomain.CurrentDomain.UnhandledException` (Thai crash dialog + BootCrash → crash.log) and `Application.ThreadException` (warning dialog + BootCrash) | Program.vb:61-93 |
| 2 | `EnableVisualStyles`, `SetCompatibleTextRenderingDefault(False)`, `SetHighDpiMode(PerMonitorV2)`; failure → dialog + Return | Program.vb:96-105 |
| 2b | `SetCurrentProcessExplicitAppUserModelID("LamyangIE.TempleAccounting.App.v1")` via shell32 P/Invoke; failure only logged | Program.vb:107-115; DllImport at Program.vb:12-13 |
| 3 | Touch `AppPaths.DatabaseFile`; if file missing, only `BootLog("!! DATABASE FILE NOT FOUND ...")` — does NOT stop | Program.vb:117-125 |
| 4 | `Db.EnsureSchema()`; on failure shows ACE-installer warning dialog but CONTINUES to open main form ("กด OK เพื่อเปิดหน้าจอหลักต่อ") | Program.vb:127-141 |
| 4c | `SaddhiviharikaDb.EnsureSchema()` (SQLite); failure logged only | Program.vb:144-149 |
| 4b | `AppPaths.MigrateReceiptsToMaster()`; failure logged only | Program.vb:152-157 |
| 4d | Onboarding gate: `If Not AppPreferences.IsOnboardingCompleted Then` modal `FrmOnboardingWizard`; `<> DialogResult.OK → Return` (app exits) | Program.vb:160-169 |
| 5 | `New frmMain()` + CenterScreen→Maximized; failure → dialog + `Environment.ExitCode=1` + Return | Program.vb:171-185 |
| 6 | `Application.Run(mainForm)`; failure → dialog + ExitCode 1 | Program.vb:187-195 |

V8's boot-step table = VERIFIED overall. Detail V8 omitted: Step 3/4 failures do NOT abort the boot (DB-missing or schema-failure still opens the UI). BootLog/BootCrash write to `Logs\crash.log` with `[BOOT]` markers (Program.vb:26-56).

## AppPaths (AppPaths.vb) — every property, VERIFIED
| Member | Resolves to | Root-aware? | Evidence |
|---|---|---|---|
| `AppRoot` | `AppDomain.CurrentDomain.BaseDirectory` (trimmed); creates it if missing | exe folder | AppPaths.vb:67-75 |
| `DatabaseFile` | `FindPreferredDatabaseFile(BaseDirectory)`: walks up to 12 parent levels; prefers `<projectRoot>\Database\TempleAccounting.accdb` where projectRoot = dir containing TempleAccounting.vbproj/.slnx; else last `Database\TempleAccounting.accdb` found while walking; else fallback `<AppRoot>\Database\TempleAccounting.accdb` (dir created) | **project-root aware** (dev), AppRoot in production | AppPaths.vb:77-89, 128-163 |
| `SaddhiviharikaDbFile` | `<AppRoot>\Database\Saddhiviharika.db` | AppRoot only (NOT project-aware) | AppPaths.vb:91-99 |
| `ImportFolder` / `ExportFolder` / `BackupFolder` / `LogsFolder` | `<AppRoot>\Import|Export|Backup|Logs` (each created on first access) | AppRoot only | AppPaths.vb:101-105, 132-145, 20-28 |
| `AppDataFolder` | `%AppData%\TempleAccounting` (Roaming) — holds `backup_registry.json` | machine-global | AppPaths.vb:165-178 |
| `ManualFile` | `<AppRoot>\USER_MANUAL_TH.md` | AppRoot | AppPaths.vb:180-185 |
| `ProvinceCsv()/DistrictCsv()/SubDistrictCsv()` | `<AppRoot>\Import\province.csv|amphoe.csv|tambon.csv` | AppRoot | AppPaths.vb:187-195 |
| `ProjectRoot` | walks up to 12 levels from AppRoot for .vbproj/.slnx; Nothing in production | — | AppPaths.vb:199-222 |
| `MasterReceiptsDir` | `{ProjectRoot}\Receipts` in dev; `{AppRoot}\Receipts` in production; created on access | **project-root aware** | AppPaths.vb:226-240 |
| `ReceiptsDir` | = `MasterReceiptsDir` (alias) | same | AppPaths.vb:243-249 |
| `MigrateReceiptsToMaster()` | one-time copy of *.jpg from `GetLegacyReceiptFolders()` into master; skips existing (non-destructive); logs count via LogStartup; per-file failures silently skipped | — | AppPaths.vb:254-284 |
| `GetLegacyReceiptFolders()` | AppRoot\Receipts; parent-of-AppRoot\Receipts; ProjectRoot\Receipts; ProjectRoot\Images\Receipts; CurrentDir\Receipts; CurrentDir\Images\Receipts (deduped) | — | AppPaths.vb:287-316 |
| `EnsureDirectoriesExist()` | creates Database dir, Backup, Export, Logs, Receipts; errors → LogCrash | — | AppPaths.vb:319-332 |
| `LogStartup` / `LogCrash` | append to `Logs\crash.log` (UTF-8) | — | AppPaths.vb:30-56 |

## Database file name contradiction — RESOLVED (VERIFIED)
- **Live DB file name = `TempleAccounting.accdb`** (AppPaths.vb:84 fallback; FindPreferredDatabaseFile only ever returns a path ending in `Database\TempleAccounting.accdb`, AppPaths.vb:145,157). Disk evidence: `./Database/TempleAccounting.accdb` (1,826,816 bytes, 2026-10-23), `./bin/Release/net10.0-windows/Database/TempleAccounting.accdb`.
- **`TempleDB_*.accdb` is the OLD backup-copy naming**, used only inside backup folders produced by the legacy `BackupFullSystem`: e.g. `bin/Release/.../Backup/Backup_25690820_081106/TempleDB_25690820_081106.accdb` and registry entry `...Backup_25691014_102801\TempleDB_25691014_102801.accdb` (real `%AppData%\TempleAccounting\backup_registry.json` entry files list). The copy inside a backup keeps that name; the source is always TempleAccounting.accdb. (Naming code to be cited in T06 from DatabaseBackupHelper.vb.)
- Note: running app resolves DB relative to `bin\Release\net10.0-windows\` → walks up finds project root → uses `<root>\Database\TempleAccounting.accdb`... BUT FindPreferredDatabaseFile starts from BaseDirectory=bin\Release\net10.0-windows and checks `<dir>\Database\TempleAccounting.accdb` at EACH level including bin\Release\net10.0-windows itself (which HAS a Database\TempleAccounting.accdb from build copy, AppPaths.vb:145-148 takes lastDbFileFound only if no projectRoot found; projectRoot IS found (root has .vbproj) → returns `<root>\Database\TempleAccounting.accdb`). So the running app uses the PROJECT-ROOT Database\TempleAccounting.accdb (updated 2026-10-23 06:42), NOT the bin copy. The bin\Release copies are build outputs (`Database\**\*` CopyToOutputDirectory in vbproj).

## AppPreferences / user_preferences.ini (VERIFIED)
- File: `{AppRoot}\user_preferences.ini` (AppPreferences.vb:76-79) — i.e., next to the EXE (bin\Release\net10.0-windows in dev). UTF-8 read/write (AppPreferences.vb:88, 113).
- Keys (AppPreferences.vb:93-101 read; 106-119 write): `LastExpenseCategoryID` (def 0), `LastExpenseFundID` (0), `LastIncomeCategoryID` (0), `LastIncomeFundID` (0), `IsOnboardingCompleted` (**def True**, AppPreferences.vb:19 — existing installs never see the wizard).
- Read on first property access (`EnsureLoaded`, AppPreferences.vb:80-106; parse errors → skip line; file errors → ErrorLogHelper). Written by `Save()` (all 5 keys, always full rewrite) via `AppPreferenceHelper.SetOnboardingCompleted/MarkOnboardingComplete/MarkOnboardingIncomplete` (AppPreferenceHelper.vb:16-33) and by FrmIncome/FrmExpense (last category/fund — to confirm in T10).
- Real file on disk (bin\Release\net10.0-windows\user_preferences.ini): LastExpenseCategoryID=8, LastExpenseFundID=7, LastIncomeCategoryID=25, LastIncomeFundID=9, IsOnboardingCompleted=1.
- **NO backup-related keys exist**: the custom backup destination (e.g. D:\สำรองข้อมูลบัญชี) is NOT persisted anywhere; each backup click re-prompts via FolderBrowserDialog (frmMain.vb:894-906). CONTRADICTS any V8/assumption of "backup roots in prefs" — there are none (BackupConfig.BackupRootPaths is hard-wired to AppPaths.BackupFolder only, BackupConfig.vb:17-23).

## Hard-coded path/folder/file names in code (VERIFIED inventory)
| Literal | Where | Note |
|---|---|---|
| "TempleAccounting.accdb" | AppPaths.vb:84,145 (DatabaseFile, FindPreferredDatabaseFile); Services\AccessDbProvider.vb (conn string builder — T04) | the DB file name |
| "Saddhiviharika.db" | AppPaths.vb:96 | SQLite file |
| "Logs","crash.log" | AppPaths.vb:23,26; Program.vb:28 | |
| "Backup" | AppPaths.vb:135 | AppRoot\Backup |
| "Export","Import" | AppPaths.vb:104,139 | |
| "Receipts","Images","Receipts" | AppPaths.vb:235,296,300,306,307 | master + legacy receipt dirs |
| "USER_MANUAL_TH.md" | AppPaths.vb:183 | |
| "province.csv","amphoe.csv","tambon.csv" | AppPaths.vb:189-194 | |
| "user_preferences.ini" | AppPreferences.vb:78 | |
| "backup_registry.json" | DatabaseBackupHelper.vb:20 (in AppDataFolder) | |
| "backup_manifest.json" | BackupRegistry.vb:145,152 | per-backup-set manifest |
| "TempleAccounting" (%AppData% subdir) | AppPaths.vb:174 | |
| "LamyangIE.TempleAccounting.App.v1" | Program.vb:110 | AppUserModelID |
| "lamYangI&E.ico","Lamyang I&Emain.png" | frmMain.vb:34,150 (Application.StartupPath) | |
| "Weekly","Monthly","Yearly" tier subfolder names | DatabaseBackupHelper.vb (T06 to cite exact lines) | tiered backup folders |
| No absolute drive paths anywhere in .vb (searched: 0 matches) | — | all roots come from AppRoot/ProjectRoot/AppData/user choice |
