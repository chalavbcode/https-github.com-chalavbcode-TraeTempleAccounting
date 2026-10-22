# PROJECT_PROFILE_V8 — Temple Accounting System (ระบบบัญชีวัด)

> **Document version:** V8 (full current-state snapshot + V7→V8 delta)
> **Generated:** 2026-09-06
> **Scope:** Complete code audit of the TempleAccounting repository — startup/initialization flow (`Program.vb`), the new **Onboarding Wizard** module (`FrmOnboardingWizard` + `OnboardingRepository` + `AppPreferences` + `WizardText`), database & transaction integrity (`Db` facade, `OleDbTransaction` usage, Gregorian date normalization), the tiered backup & centralized receipt system (`DatabaseBackupHelper`), UI/UX & localization standards (`frmMain`, entry forms, UTF-8 with BOM enforcement), and the full Agent Skill Registry (`.agents/skills/`).
> **Companion documents (historical, NOT modified):** `PROJECT_PROFILE_V7.md` … `V4`, `_ProjectDocs_And_Archive/*`.

---

## 1. Executive Summary & Version History

### 1.1 Executive Summary

Temple Accounting System (lamYangI&E) is a VB.NET / WinForms desktop application for temple income–expense bookkeeping on **MS Access (OLEDB)**. V8 is the **onboarding & hardening release**: it introduces an interactive **5-step Onboarding Wizard** that walks a brand-new install through temple profile, personnel, bank/fund, and opening-balance setup before the main window opens, plus **tiered backup retention** (Weekly/Monthly/Yearly FIFO), **MD5-collision-safe centralized receipt backup**, an **error-logging helper** replacing silent empty `Catch` blocks, and **transactional safety** around temple-settings and opening-balance writes. The V8 audit also **enforced the UTF-8-with-BOM encoding standard** across all 56 Thai-containing `.vb` source files (BOM added to 53 files that lacked it) and re-verified `dotnet build -c Release` at **0 Warnings / 0 Errors**.

### 1.2 Version History (highlights)

| Version | Date | Key highlights |
|---|---|---|
| **V8** | 2026-09 | **Interactive Onboarding Wizard** (5 steps, test-mode dry run, DPI-safe, transactional writes); **tiered backup retention** (Weekly×8 / Monthly×15 / Yearly×5 FIFO, ISO-week naming immune to Buddhist-era year formats); **centralized `Backup/Receipts/`** append-only MD5-collision-safe sync; `ErrorLogHelper` (error.log) replacing silent empty catches; temple-settings save wrapped in `OleDbTransaction`; FrmIncome header+grid grouped in `TableLayoutPanel`; **UTF-8 with BOM enforced on all Thai source files**; `user_preferences.ini` persistence (`AppPreferences`) |
| V7 | 2026-08 | Unified amber metric strip ("ครั้ง" unit), gold `lblRecentHeader` table header, fixed 412 px 10-row grid (no Bottom anchor), unified 4-button action row with aligned `btnClose`, FrmIncome↔FrmExpense design parity; `PROJECT_PROFILE_V7` snapshot |
| V6 | 2026-08 | `FrmChartPreview` full-screen chart (zoom/pan/toggle/print/save), preview-first PDF export with signature block, dynamic page-row balancing, F1 manual sections 10–11, centralized `ReportEngine` |
| V5 | 2026-08 | `FrmMultiMachineImport` DPI rebuild, print hardening (`UseLandscape`, safe margins), `btnSavePdf`, multi-machine import/export forms |
| V4 | 2026-08 | 8-button toolbar baseline, 12-item known-issue log |
| V3 | 2026-08 | UI auto-fit (`UiFitter`), personnel-ID migration |
| V2 | 2026-07 | Post-refactor state, F1 Help System intro |

---

## 2. System Architecture & Tech Stack

### 2.1 Application Identity

| Field | Value |
|---|---|
| Product name | Temple Accounting System — ระบบบัญชีวัด (lamYangI&E) |
| Purpose | บันทึกบัญชีรับ-จ่ายเงินวัด: รายรับ (เงินบริจาค), รายจ่าย, โอนเงินภายในกองทุน/บัญชีธนาคาร, รายงานสรุป (ละเอียด/ย่อ/รายเดือน), แนบรูปใบเสร็จ, นำเข้า/ส่งออกข้ามเครื่อง, พร้อมเพย์ / e-Donation |
| Publisher (installer) | วัดดอนเมือง |
| Version | 1.0.0 (Inno Setup `TempleAccountingSetup.iss`, `SimpleInstaller.ps1`) |
| License | The Unlicense |
| Solution | `TempleAccounting.slnx` (SDK-style, `EnableDefaultCompileItems=false`, explicit `<Compile>`/`<EmbeddedResource>` items) |
| Output | `lamYangI&E.exe` (`bin\Release\net10.0-windows`) |

### 2.2 Technology Stack

| Layer | Technology |
|---|---|
| Language | VB.NET, `Option Strict Off` / `Option Explicit On`, `StartupObject = TempleAccounting.Program` |
| Runtime | .NET 10.0-windows (`net10.0-windows`); `SetHighDpiMode(PerMonitorV2)` |
| UI | WinForms; panel-hosted (non-MDI) child forms; standard font **Tahoma**; `AutoScaleMode.Font` baseline with per-form DPI modes; emoji icons rendered to bitmaps via GDI+ (`MakeIconBitmap`) |
| Database (primary) | MS Access `.accdb` via OLEDB `Microsoft.ACE.OLEDB.12.0` (NuGet `System.Data.OleDb` 10.0.10); self-healing schema via `Db.EnsureSchema()` |
| Database (secondary) | SQLite `Saddhiviharika.db` (ordination registry, `SaddhiviharikaDb.vb`) |
| Data layer | Provider-agnostic facade `Db` → `IDbProvider` (`AccessDbProvider`, `SqlServerDbProvider`, `DbProviderFactory`); repository pattern `ITransactionRepository` / `AccessTransactionRepository` / `TransactionService`; `OnboardingRepository` DAL for wizard |
| Persistence of prefs | `user_preferences.ini` (UTF-8) via `AppPreferences` module + `AppPreferenceHelper` (last category/fund per form; `IsOnboardingCompleted` flag) |
| Excel | `ExcelTransactionImporter.vb` (นำเข้ารายรับ/รายจ่าย), `TransactionExcelExporter.vb`, `TransferFormat.vb` |
| Reports | GDI+ `PrintDocument` subclass `IncomeExpenseReport` + `ReportEngine` module; charts via `System.Windows.Forms.DataVisualization` |
| Help | Markdown `USER_MANUAL_TH.md` + `HelpSystem` / `FrmHelpDialog` (F1 contextual) |

### 2.3 Startup & Initialization Flow (`Program.vb`)

Ordered, exception-guarded boot sequence (every step wrapped in `Try/Catch` with `BootCrash` → `crash.log`):

| Step | Action |
|---|---|
| 0 | Register `AppDomain.CurrentDomain.UnhandledException` + `Application.ThreadException` handlers (Thai crash dialogs, log to `Logs/crash.log`) |
| 1 | WinForms init: `EnableVisualStyles`, `SetCompatibleTextRenderingDefault(False)`, `SetHighDpiMode(PerMonitorV2)` |
| 2 | Set unique `AppUserModelID` (`LamyangIE.TempleAccounting.App.v1`) so Taskbar groups don't collide with legacy shortcuts |
| 3 | Resolve `AppPaths.DatabaseFile` (walks up to project root / `Database/`), verify existence |
| 4 | **`Db.EnsureSchema()`** — idempotent table creation, migrations, seeders (Access OLEDB) |
| 4c | **`SaddhiviharikaDb.EnsureSchema()`** — SQLite ordination registry |
| 4b | **`AppPaths.MigrateReceiptsToMaster()`** — migrate legacy receipt images into permanent project-root `Receipts/` (survives VS Rebuild) |
| 4d | **Onboarding gate** — `If Not AppPreferences.IsOnboardingCompleted Then` → modal `FrmOnboardingWizard`; if wizard is cancelled (`<> DialogResult.OK`) the app **exits immediately** (`Return`) so no un-configured install proceeds |
| 5 | Construct `frmMain` (`CenterScreen` → `Maximized`); failure → error dialog + `ExitCode = 1` |
| 6 | `Application.Run(mainForm)`; `FormClosing` triggers tiered backup |

**Onboarding gate semantics (audited):** `AppPreferences.IsOnboardingCompleted` **defaults to `True`** so existing installs never re-run the wizard; only a brand-new install with no `user_preferences.ini` (or an explicit developer reset via the "🔄 รีเซ็ต Onboarding" sidebar button) will see it. `OnboardingRepository.ResetOnboardingStatus()` flips the flag to `False` and persists.

---

## 3. Core Modules & Data Models

### 3.1 Database Schema (MS Access, via `AccessDbProvider`)

**Core ledger table — `Transactions`** (migrations add: `ToFundID`, `ToBankID`, `ReceiptPath`, `TransactionGUID`, `ReceiveChannel`, `ReferenceNo`):

| Column | Type | Notes |
|---|---|---|
| `ID` | COUNTER PK | auto-increment |
| `TranDate` | DATETIME NOT NULL | Gregorian-normalized |
| `TranType` | TEXT(10) NOT NULL | `Income` / `Expense` / `Transfer` / `OpeningBalance` |
| `CategoryID` / `FundID` / `BankID` | INTEGER | → `Categories.ID` / `Funds.ID` / `BankAccounts.ID` (FKs nullable) |
| `Detail` | TEXT(255) | description |
| `Amount` | CURRENCY NOT NULL | money |
| `Note` | MEMO | remark |
| `CreateDate` | DATETIME DEFAULT Now() | |
| `ToFundID` / `ToBankID` | INTEGER | transfer targets (migrated) |
| `ReceiptPath` | TEXT(255) | attached receipt image (migrated) |
| `TransactionGUID` | TEXT(36) | cross-machine dedup (migrated) |
| `ReceiveChannel` / `ReferenceNo` | TEXT(20) / TEXT(50) | PromptPay / e-Donation (migrated) |

**Support tables:** `Categories` (ID, CategoryName, TranType), `Funds` (ID, FundName), `BankAccounts` (ID, BankName, AccountNo, AccountName), `TempleSetting` (ID, TempleCode, TempleName, TempleAddress MEMO, Tambon, Amphoe, Province, PostCode, TemplePhone, **AbbotPersonnelID, WaiyawatPersonnelID, BookkeeperPersonnelID**, PromptPayName, PromptPayID), `Personnel` (PersonnelID, Title, FirstName, LastName, FullName, PersonType, PositionID, Phone), `Positions` (PositionID, PositionName, PositionGroup: พระ/ฆราวาส/ทั่วไป), `Province`/`District`/`SubDistrict` (address cascade incl. `ZipCode`).

**Seeds (only when empty):** 10 Categories (5 Income / 5 Expense), 4 Funds (กองทุนทั่วไป / กฤติยาธรรม / กู้ภัยวัด / ซ่อมแซม), 1 BankAccount (ธนาคารออมสิน — วัดแหลมยาง), 19 Positions, 6 sample Personnel.

### 3.2 Form Modules

| Module | Responsibility |
|---|---|
| `frmMain` (+ `.Designer`, `.UiHelpers`) | Dashboard shell, panel-hosted navigation, metric cards, dev tools (test wizard / reset onboarding), status center, backup/restore buttons, FormClosing tiered backup |
| `FrmIncome` / `FrmExpense` | Income/expense entry — summary strip, gold header, fixed 10-row recent grid, 4-button action row, receipt browse/paste/clear |
| `FrmTransactions` | Full ledger search/edit/delete (in-grid edit path remains partially dead code — see §7) |
| `FrmTransfer` | Intra-fund/bank transfers (single INSERT with To-columns, GUID) |
| `FrmReports` / `IncomeExpenseReport` / `ReportEngine` / `FrmChartPreview` | Report center, GDI+ rendering, charts, PDF export |
| `FrmTempleSetting` | Temple profile + personnel links + PromptPay (transactional save) |
| `FrmPersonnelManagement`, `FrmMasterData`, `FrmLocationImport` | Personnel CRUD, master data (categories/funds/banks), CSV address import |
| `FrmMultiMachineImport` / `FrmExportForTransfer` | Cross-machine merge (transaction + import history) / export |
| `FrmOnboardingWizard` | **V8** — first-run 5-step setup (see §4.1) |
| `FrmHelpDialog`, `FrmReceiptViewer`, `FrmChartPreview` | Help, receipt preview, chart preview |

### 3.3 Helper Services

- **`AppPaths`** — path resolution: AppRoot, DatabaseFile (project-root aware), BackupFolder, ExportFolder, LogsFolder (`crash.log`), **MasterReceiptsDir** (permanent project-root `Receipts/`, survives Rebuild), legacy receipt migration, `EnsureDirectoriesExist`, `LogStartup`/`LogCrash`.
- **`ErrorLogHelper`** — **V8**: `LogSilentError(context, ex)` → `Logs/error.log`; the designated replacement for silent empty `Catch` blocks.
- **`UiFitter`** — `AutoFitFormButtons`, `DisableComboBoxWheel`, fixed-vertical-layout enforcement.
- **`ReceiptImageHelper`** — resize→1600px, JPEG q80, `Receipt_{transactionID}.jpg`, multi-location `ResolveReceiptPath` fallback chain.
- **`DesktopShortcutHelper`** — creates desktop shortcut on first launch.
- **`MessageBoxHelper`** — centralized Thai dialogs.
- **`HelpSystem`** — F1 contextual manual.

---

## 4. New Features in V8

### 4.1 Interactive Onboarding Wizard (`FrmOnboardingWizard` + `OnboardingRepository` + `WizardText` + `AppPreferences`)

A modal, DPI-safe (min 960×660 layout, 1000×720 form guard), 5-panel wizard presented only on fresh installs. All Thai strings centralized in `WizardText.vb` (BOM-enforced; special glyphs via `ChrW` code points); all data access isolated in `OnboardingRepository`; all multi-statement writes wrapped in `OleDbTransaction`.

| Step | Panel | Workflow | Validation gate | Persistence |
|---|---|---|---|---|
| **1 — Temple Profile** | `pnlStep1` | Temple code/name/address/phone + Province→District→SubDistrict cascade combos (SQL `WHERE`-chained, `_isLoading` guard) | Required: name, address, province, district, sub-district (MistyRose/Honeydew visual feedback + ToolTip, no ErrorProvider) | `SaveTempleSetting()` — `DELETE FROM TempleSetting` + INSERT in one transaction |
| **2 — Personnel Setup** | `pnlStep2` | Abbot combo (PositionGroup='พระ'), Waiyawat + Bookkeeper combos ('ฆราวาส'); live personnel-count label; linked-role summary grid (`GetLinkedPersonnelSummary` UNION ALL) | ≥1 personnel row in DB; ≥1 of the three roles selected | `LinkPersonnelToTempleSetting()` — single transactional UPDATE; clears `ReportEngine` template cache |
| **3 — Bank/Fund Verification** | `pnlStep3` | Bank accounts grid + inline `grpAddBank` add-bank form; funds grid (read-only verification) | ≥1 bank account in DB (`GetBankAccountCount`) | `AddBankAccount()` INSERT (wizard test-mode skips DB writes) |
| **4 — Opening Balances** | `pnlStep4` | Balance date picker (defaults today) + cash + bank amounts | Numeric ≥ 0 (0 allowed) | `SaveOpeningBalance()` — DELETE `TranType='OpeningBalance'` then INSERT Cash/Bank rows in one transaction; date via `Db.AccessDateLiteral(Db.NormalizeGregorianDate(...))` |
| **5 — Test Report Verification** | `pnlStep5` | Summary grid (`GetOnboardingSummary`: temple, abbot/waiyawat/bookkeeper, banks, funds, cash/bank opening totals) + "🖨️ พิมพ์รายงานหน้า (ตรวจสอบ)" printing current month's report via `IncomeExpenseReport.ShowPreview` | Always enabled | Finish → `AppPreferenceHelper.MarkOnboardingComplete()` → `DialogResult.OK` |

**Audited behaviors:**
- **Test mode** (`isTestMode:=True`, reachable from the sidebar "🧪 ทดลอง Onboarding" button): orange banner, `ControlBox` enabled, Close-Test-Mode button, `btnNext` orange, and **all Save methods bypassed** (dry-run only).
- **Re-entry guard:** if `TempleSetting` rows already exist in normal mode, the wizard marks onboarding complete and closes immediately (`DialogResult.OK`).
- **Part 7 guard:** `Shown` event enforces minimum size (1000×720) against external shrinkers; `#If DEBUG` `AuditOverlap()` reports control overlaps to Output.
- `ValidateCurrentStep()` re-runs on every `TextChanged`/`SelectedIndexChanged`; Next is disabled until the current step's gate passes; Back reloads persisted values (`LoadStep1FromDb`, `RefreshStep2Data`, `RefreshStep3Data`).

### 4.2 Tiered Backup Retention (`DatabaseBackupHelper`)

Triggered from `frmMain.FormClosing` (`RunTieredBackup()`); manual full backup via sidebar "💾" (`BackupFullSystem`).

| Tier | File pattern | Retention (FIFO) | Overwrite behavior |
|---|---|---|---|
| **Weekly** | `Backup_Weekly_{ISOYear}_W{week:D2}.accdb` | max **8** | overwrite-in-place per ISO week |
| **Monthly** | `Backup_Monthly_{yyyy}_{MM}.accdb` | max **15** | overwrite-in-place per month |
| **Yearly** | `Backup_Yearly_{yyyy}.accdb` | max **5** | December only, per year |

Key correctness details:
- **Buddhist-era immunity:** names built from `ISOWeek.GetYear/GetWeekOfYear` and `d.Year / d.Month` **properties**, never `d:yyyy` / `d:MM` format specifiers (on a Thai-locale PC `d:yyyy` yields พ.ศ. year). Commented rationale in source.
- **FIFO cleanup** (`CleanupTierFolder`): sorts by **file name** (`StringComparer.OrdinalIgnoreCase`) rather than `CreationTime` (filenames embed the timestamp, so ordering is deterministic); deletes oldest until ≤ max.
- Pre-copy `GC.Collect()` + `WaitForPendingFinalizers()` as best-effort against OLEDB file locks; per-file failures logged, never fatal.

### 4.3 Centralized Receipt Backup (`Backup/Receipts/`, MD5-Collision-Safe)

`SyncReceiptsToBackup()` copies `Receipts/` → `Backup/Receipts/` **append-only** (never deletes anything):

1. File absent at destination → straight copy.
2. Same name, **identical MD5** → skip (no duplicate).
3. Same name, **different MD5** (content collision) → store both under a deterministic name `{nameNoExt}_{md5[:8]}{ext}`; rare re-collision handled with a numeric suffix counter.
4. Restore (`RestoreFullSystem`) merges images from the selected backup folder, its legacy `Receipts/` subfolder, and/or the centralized `Backup/Receipts/` fallback (`CopyImageFilesOnly` — image extensions + `Receipt_` prefix heuristic).
5. **Pre-restore safety backup** (`PreRestore_Temp/`): current DB + receipts staged before overwrite; on any failure the original data is rolled back automatically; success deletes the staging folder. Restore completes with `Application.Restart()`.

---

## 5. Safety Guardrails & Compliance

### 5.1 Transaction Safety (OleDbTransaction)

All "DELETE then INSERT/UPDATE" sequences are atomic:

| Site | Pattern |
|---|---|
| `FrmTempleSetting.btnSave_Click` | `DELETE FROM TempleSetting` + INSERT inside `BeginTransaction`/`Commit`/`Rollback` (commit 05d5f18) |
| `OnboardingRepository.SaveTempleSetting` | DELETE + INSERT, rollback + rethrow on failure |
| `OnboardingRepository.LinkPersonnelToTempleSetting` | single UPDATE inside transaction |
| `OnboardingRepository.SaveOpeningBalance` | DELETE OpeningBalance rows + INSERT Cash/Bank rows inside transaction |
| `FrmTransactions` (row delete) | transactional delete |
| `FrmMultiMachineImport` | `ExecInTx` / `ScalarInTx` helpers run the whole merge in one `OleDbTransaction` with pre-merge on-disk backup |

**Backup/restore** is file-level (not `OleDbTransaction` by design): snapshot copy → safety staging → overwrite → rollback-on-failure, which is the correct integrity model for whole-file restore.

### 5.2 Gregorian Date Normalization

- **`Db.NormalizeGregorianDate(value)`** — if `value.Year > 2400` (Buddhist era), subtract 543; used on **read** (populate `DateTimePicker`, report ranges) and on **write** (before insert).
- **`Db.AccessDateLiteral(value)`** — normalizes, then emits `#yyyy-MM-dd HH:mm:ss#` via `InvariantCulture` — the only legal way to embed dates in SQL (audited across `FrmIncome`, `FrmExpense`, `FrmTransactions`, `FrmTransfer`, `FrmReports`, `ReportEngine`, `ExcelTransactionImporter`, `FrmMultiMachineImport`, `OnboardingRepository`).
- SQL-side idiom for mixed-era data: `IIF(Year(x)>2400, DateAdd('yyyy',-543,x), x)` (report carry-forward); dashboard monthly cards use `MONTH(TranDate)=MONTH(Date()) AND (YEAR(...)=YEAR(Date()) OR YEAR(...)=2026)` to survive Thai-locale Buddhist-year storage.
- All aggregates read through `Db.ToDecimalOrZero` / `Db.ToIntOrZero` (null-safe).

### 5.3 UI Layout Discipline

- **Panel-hosted (non-MDI)** navigation; strict dashboard visibility contract (`pnlOverview`/`pnlFormHostHeader`/`picHomeLogo` toggling).
- **4-button action row**: same Y, `Anchor = Top Or Left` (no stretch); `btnClose.Location.X = btnImportExcel.Right + 8`; Tahoma 11 Bold; `FlatStyle.Flat`; `Cursor = Hand`.
- **Fixed 10-row grid** (`Height = 412`, no `Bottom` anchor); gold header `lblRecentHeader` (`#F1C40F`, Tahoma 10 Bold) positioned `Bottom == grid.Top`; newest V8 refinement groups header + grid in **`tlpRecentList` TableLayoutPanel** (1007×429 @ (766,85), 32 px header row + 100% grid row) so the pair moves as one unit.
- **Amber summary strip** (`lblSummary`, 36 px, center-aligned, `|` separators, "ครั้ง" unit, `#,##0.00` money).
- Wizard/child forms: `UiFitter.AutoFitFormButtons` + `DisableComboBoxWheel` on load; `#If DEBUG` overlap auditor.

### 5.4 UTF-8 with BOM Encoding Standard (enforced in V8 audit)

**Standard:** every `.vb` source file containing Thai text must be **UTF-8 with BOM** (Roslyn's UTF-8 default makes BOM-less files compile, but the BOM guarantees correct decoding across toolchains/editors and prevents Thai-locale ANSI misinterpretation).

**Audit result & action taken:** a byte-level scan of the tree found **53 of 56 Thai-containing `.vb` files without a BOM** (only `frmMain.vb`, `frmMain.Designer.vb`, `WizardText.vb` had it). All 53 were verified valid UTF-8 (no cp874/ANSI corruption), then **BOM-prefixed** (byte-level prepend; content untouched, line endings preserved). Post-fix scan: **0 violations** — 100% of Thai-containing source files now carry UTF-8 BOM. `dotnet build -c Release` re-verified afterward: **0 Warnings / 0 Errors**.

---

## 6. Agent Skill Registry (`.agents/skills/`)

Nine active architectural skills codify the project's hard-won conventions (each is a `SKILL.md` with front-matter, known-issue history, and do/don't rules):

| Skill | Domain covered |
|---|---|
| `access-date-safe` | Thai Buddhist Era vs. Gregorian date handling, `Db.AccessDateLiteral()`/`NormalizeGregorianDate()` invariants, date-range query patterns, per-file audit table of remaining call sites |
| `childform-mdi-nav` | Panel-hosted child-form lifecycle, open/close/switch, frmMain↔child communication |
| `datagridview-column-setup` | DataGridView column configuration consistency across forms (prevents the 20+ commit churn on grid tweaks) |
| `db-connection-safety` | OleDb connection lifecycle, parameterized queries, `BeginTransaction`/`Commit`/`Rollback` rules (no bare DELETE+INSERT), defensive null handling |
| `form-duplicate-code` | FrmIncome/FrmExpense ~80% shared logic — sync discipline when refactoring both forms |
| `receipt-clipboard-workflow` | Clipboard paste / file browse / save-to-Receipts / lifecycle via `ReceiptImageHelper` |
| `report-rendering-layout` | GDI+ report rendering — signature block, page balancing, margin/coordinate constants |
| `vb-audit-checker` | VB.NET/WinForms/MS-Access bug & code-quality audit patterns (dead code, transaction safety, guard patterns, debug harness, data integrity) |
| `winforms-ui-align` | Coordinates, alignment, responsive layout, visual standards for this app |

---

## 7. Build & Deployment Status

| Item | Value |
|---|---|
| **Build command** | `dotnet build -c Release` |
| **Result (re-verified 2026-09-06)** | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| Output | `bin\Release\net10.0-windows\lamYangI&E.dll` / `lamYangI&E.exe` |
| Runtime deps | .NET 10 (net10.0-windows); 64-bit Microsoft Access Database Engine 2016 (OLEDB ACE.12.0); `Microsoft Print to PDF` virtual printer |
| Installer | Inno Setup `TempleAccountingSetup.iss` (v1.0.0) + `SimpleInstaller.ps1` |
| DB | `TempleAccounting.accdb` (schema self-healing) + `Saddhiviharika.db` (SQLite) |
| Logs | `Logs/crash.log` (boot + unhandled) and `Logs/error.log` (silent-catch) |
| Help doc | `USER_MANUAL_TH.md` copied to output |

### 7.1 Outstanding Items (carried forward / observed)

1. `FrmTransactions.SaveSelectedRowEdits` remains wired but Transfer-row in-grid editing has no working save path (dead code).
2. `FrmLocationImport` CSV parser is naive (no quoted-field handling).
3. `FrmPersonnelManagement` `fullName` title gluing has no space; Positions delete lacks a friendly FK-in-use message.
4. `LoadMonkSample()` in frmMain reads legacy TempleSetting columns (guarded only by `IsDBNull`).
5. **Cosmetic copy/paste inconsistency found during V8 audit:** `WizardText.Step3Title` reads "ขั้นที่ 3: ตั้งค่าปีงบประมาณ" (fiscal year) while step 3 actually performs Bank/Fund verification — title text should be aligned with the `Step3Desc` content.
6. `BaseReport` remains dead scaffolding; `AutoScaleMode` still mixed across forms.

---

*End of PROJECT_PROFILE_V8.md — full current-state snapshot + V7→V8 delta: interactive 5-step Onboarding Wizard (transactional writes, test mode, DPI-safe), tiered Weekly/Monthly/Yearly FIFO backup with Buddhist-era-safe naming, MD5-collision-safe centralized `Backup/Receipts/`, `ErrorLogHelper` silent-error logging, transactional temple-settings save, TableLayoutPanel grid grouping on FrmIncome, UTF-8-with-BOM enforced across all 56 Thai-containing source files, and re-verified 0-Warning/0-Error Release build. All application source files and historical profile documents (V2–V7) remain unmodified except the BOM normalization described in §5.4.*