# Project Health Report — Temple Accounting System
**วันที่ตรวจสอบ:** 20 สิงหาคม 2569 (20 August 2026)
**ผู้ตรวจสอบ:** Buffy (Codebuff Audit Agent)
**เวอร์ชันโค้ด:** ตรวจสอบจาก source code ปัจจุบัน (ไม่อ้างอิงจากเอกสาร PROFILE/AUDIT เดิม)

---

## สรุปผู้บริหาร

ระบบบัญชีวัดแหลมยาง (Temple Accounting System) เป็นแอปพลิเคชัน Windows Forms (.NET 10.0, VB.NET) ขนาดเล็ก-กลาง สำหรับจัดการบัญชีรายรับ-รายจ่ายของวัดในศาสนาพุทธ ตัวระบบมีโครงสร้างที่เริ่มวางสถาปัตยกรรมแบบ Service Layer แล้ว (IDbProvider, ITransactionRepository, TransactionService) แต่ฟอร์มส่วนใหญ่ยัง bypass layer นี้เขียน SQL ตรงๆ เอง Build ผ่านเรียบร้อย 0 Warning, 0 Error โครงสร้างข้อมูลมี 10 ตารางหลักใน Access DB + 3 ตารางใน SQLite สำหรับระบบสัทธิวิหาริก

ระดับของระบบ: **ใช้งานได้จริงระดับพื้นฐาน (Functional MVP)** — มีครบทุกงานหลัก (บันทึกรายรับ/รายจ่าย/โอน/รายงาน) แต่ยังขาดระบบความปลอดภัย (ไม่มีผู้ใช้, ไม่มี audit trail, ไม่มี encryption) และมี technical debt จำนวนมากจาก Catch blocks ที่กลืน exception เงียบๆ, โค้ดที่ไม่ได้ใช้, และ inconsistent patterns ในการเข้าถึงข้อมูล

ปัญหาที่ควรสนใจที่สุด: **(1)** TransactionService ทำงานเฉพาะ Income — FrmExpense, FrmTransfer, FrmTransactions bypass ทั้งหมด **(2)** Empty Catch blocks กระจายตัว 63+ จุด ทำให้ error หายไปเงียบๆ **(3)** ไม่มี user authentication เลย ใครก็เปิดใช้ได้

---

## ส่วนที่ 1: Project Health Snapshot

### 1.1 ขนาดและองค์ประกอบ

| รายการ | จำนวน |
|---|---|
| ไฟล์ `.vb` ทั้งหมด (ไม่รวม obj/) | 53 ไฟล์ |
| ไฟล์ `.vb` ที่เขียนเอง (ไม่รวม Designer) | 38 ไฟล์ |
| บรรทัดโค้ดรวม (ไม่รวม Designer) | ~14,028 บรรทัด |
| ฟอร์ม (Forms) | 14 ฟอร์ม (13 + FrmChartPreview ซึ่งเปิดแบบ ShowDialog) |
| Service/Helper class | 7 ไฟล์ (IDbProvider, ITransactionRepository, AccessDbProvider, AccessTransactionRepository, SqlServerDbProvider, DbProviderFactory, TransactionService) |
| Helper Module | 8 ไฟล์ (AppPaths, AppPreferences, Database, DatabaseBackupHelper, DesktopShortcutHelper, HelpSystem, MessageBoxHelper, UiFitter) |
| ตารางในฐานข้อมูล Access (TempleAccounting.accdb) | 10 ตาราง |
| ตารางใน SQLite (Saddhiviharika.db) | 3 ตาราง (temples, monks, saddhiviharika) |
| NuGet packages | 5 (Microsoft.Data.SqlClient, Microsoft.Data.Sqlite, System.Data.OleDb, WinForms.DataVisualization, ClosedXML) |

### 1.2 Build Health

| รายการ | ผลลัพธ์ |
|---|---|
| `dotnet build -c Release` | ✅ **Build succeeded** — 0 Warning(s), 0 Error(s) |
| `#Disable Warning` ในโค้ด | ❌ **ไม่พบ** — ไม่มีการปิด warning ใดๆ ในโค้ด |
| NoWarn ใน .vbproj | ⚠️ `<NoWarn>WFO1000</NoWarn>` — ปิด WinForms Analyzer warning WFO1000 (เกี่ยวกับ `Invoke/BeginInvoke`) |

**หมายเหตุ:** `<NoWarn>WFO1000</NoWarn>` ใน .vbproj (บรรทัดที่ 14) เป็นการปิด WinForms Analyzer WFO1000 ซึ่งเป็น warning ที่เกี่ยวกับการใช้ `Control.Invoke`/`Control.BeginInvoke` บน thread ที่ไม่ใช่ UI thread ควรพิจารณาว่า warning ที่ถูกปิดไปนี้มีผลกระทบจริงหรือไม่

### 1.3 ความสอดคล้องของโค้ด (Consistency)

**สิ่งที่สอดคล้องกัน:**
- ทุกฟอร์มใช้ `Option Strict Off` + `Option Explicit On` เหมือนกัน
- ทุกฟอร์มใช้ Namespace `TempleAccounting`
- ทุกฟอร์มเรียก `HelpSystem.SetupHelp(Me, "FrmXxx")` + `UiFitter.AutoFitFormButtons(Me)` ใน Load event
- ทุกฟอร์มมี Enter-key navigation pattern เหมือนกัน
- ทุกฟอร์มมี ToolTip ตั้งค่าใน `SetupToolTips()`

**สิ่งที่ไม่สอดคล้องกัน (ตัวอย่างจริง):**

1. **การเข้าถึงข้อมูล — ผ่าน Db facade vs. เปิด OleDbConnection ตรงๆ:**
   - `FrmIncome` ใช้ `_txnService.SaveIncome()` (ผ่าน TransactionService) — ดี ✅
   - `FrmExpense` เขียน SQL INSERT/UPDATE ตรงๆ ผ่าน `Db.ExecuteNonQuery(conn, ...)` โดยไม่ผ่าน TransactionService — ข้าม layer ❌
   - `FrmTransfer` เขียน SQL INSERT ตรงๆ ผ่าน `Db.InsertAndGetId(conn, ...)` — ข้าม layer ❌
   - `FrmTransactions` ลบรายการด้วย `OleDbCommand` ตรงๆ (`Dim oledbConn = TryCast(conn, OleDbConnection)`) — bypass ทั้ง Db facade ❌

2. **การจัดการรูปใบเสร็จ:**
   - `FrmIncome`: ใช้ `_txnService.UpdateReceiptPath()` (ผ่าน Repository) ✅
   - `FrmExpense`: ใช้ `OleDbCommand` ตรงๆ อัปเดต ReceiptPath (บรรทัดที่ 438-444) ❌
   - `FrmTransactions`: ใช้ `Db.ExecuteNonQuery()` ผ่าน Db facade (ไม่ผ่าน Repository) ⚠️

3. **Error handling ไม่สอดคล้อง:**
   - `FrmIncome.btnSave_Click`: `Catch ex As Exception` → แสดง MessageBox ให้ user เห็น ✅
   - `frmMain.ShowDashboard`: มี fallback `Catch` ว่างเปล่าหลายจุด ❌
   - `FrmReports.btnLedger_Click`: `Catch ex As Exception : Throw` (re-throw ทันที ไม่ log) ⚠️
   - `AppPreferences.EnsureLoaded`: `Catch` ว่างเปล่า — ถ้าอ่าน config ไม่ได้ จะเงียบไป ❌

4. **ตัวแปร naming:**
   - ส่วนใหญ่ใช้ PascalCase (`_editId`, `_enterFlow`) ซึ่งถูกต้อง
   - แต่บางจุดใช้ camelCase ปนกัน เช่น `selCatId`, `selFundId` ใน `FrmMasterData` (ไม่ได้ prefix ด้วย `_` เหมือนที่อื่น)

### 1.4 Technical Debt สะสม

**TODO/FIXME/HACK Comments:** ไม่พบ comment ที่มีคำว่า TODO, FIXME, หรือ HACK ในโค้ด (grep ค้นทั้งโปรเจกต์แล้ว — ผลลัพธ์ 0 match)

**Technical Debt รูปแบบอื่นๆ ที่พบ:**

1. **Empty Catch blocks จำนวนมาก (63+ จุด)** — ดูรายละเอียดในส่วนที่ 5.2
2. **Debug.WriteLine ค้างใน Production code** — พบใน `FrmReports.vb` (บรรทัดที่ 383, 413), `FrmTempleSetting.vb` (บรรทัดที่ 312-320) — คำสั่ง debug ที่ไม่ควรอยู่ใน release
3. **Sample data fallback หลายจุด** — `frmMain.vb` มี `LoadSampleDashboardData()`, `LoadDonationSample()`, `LoadExpenseSample()`, `LoadReportSample()`, `LoadMemberSample()`, `LoadActivitySample()`, `LoadSettingSample()` — ข้อมูลตัวอย่าง hardcoded ที่ fallback เมื่อ DB query ล้มเหลว
4. **FrmTransfer.vb:25 `MakeLbl` function** — ประกาศ Private Function ไว้แต่ไม่เคยถูกเรียกใช้ (dead code)
5. **FrmReports.vb:29 `MakeLbl` function** — เหมือนกัน ประกาศไว้แต่ไม่ได้ใช้

---

## ส่วนที่ 2: เทียบมาตรฐานระบบประเภทเดียวกัน

### ตารางเปรียบเทียบ

| องค์ประกอบมาตรฐาน | มี/ไม่มี/บางส่วน | หลักฐาน |
|---|---|---|
| ระบบผู้ใช้ + สิทธิ์ (multi-role) | **ไม่มี** | ไม่พบตาราง Users/Roles/Permissions ใน DB schema, ไม่มี Login form, ไม่มี authorization check ในฟอร์มใดๆ |
| Audit trail / change log | **ไม่มี** | ไม่มีตาราง AuditLog/ChangeHistory, ไม่มี log ว่าใครแก้ไขรายการ何时. `CreateDate` ใน Transactions เป็นแค่วันเวลาที่สร้าง ไม่ใช่ audit trail |
| การออกใบเสร็จ/เอกสารให้บุคคลภายนอก | **บางส่วน** | มีการแนบรูปใบเสร็จ (ReceiptPath) และ `FrmReceiptViewer` สำหรับดูรูป แต่ไม่สามารถ **พิมพ์/สร้างใบเสร็จใหม่** จากระบบได้ (ไม่มี print receipt feature) |
| การกระทบยอด (reconciliation) กับบัญชีธนาคารจริง | **ไม่มี** | ไม่มีฟีเจอร์ import statement จากธนาคาร, ไม่มี reconcile function, ไม่มี BankStatement table |
| Chart of accounts / งบดุลมาตรฐานบัญชี | **บางส่วน** | มี `Categories` (Income/Expense) และ `Funds` แต่ไม่มี Chart of Accounts แบบ double-entry, ไม่มี Balance Sheet, ไม่มี General Ledger ที่เป็นมาตรฐานบัญชี |
| การอนุมัติหลายขั้น (approval workflow) | **ไม่มี** | ไม่มี workflow/_statuses ใน Transactions, ไม่มี ApprovalStatus field, ไม่มี multi-level approval |
| Data export ที่ตรวจสอบได้ | **บางส่วน** | มี Export Excel (TransactionExcelExporter.vb) ด้วย ClosedXML, มี IncomeExpenseReport.vb สำหรับพิมพ์ PDF แต่ไม่มี checksum/digital signature |
| การเข้ารหัส/ป้องกันไฟล์ฐานข้อมูล | **ไม่มี** | ไฟล์ `.accdb` ไม่ได้ถูกเข้ารหัส, ไม่มี password protection, ไม่มี code ที่เกี่ยวกับ DB encryption |
| Multi-currency หรือ multi-fund accounting ที่แยกบัญชีชัดเจน | **บางส่วน** | มี `Funds` table สำหรับแยกกองทุน (4 กองทุน seed data) แต่ไม่มี multi-currency support (เฉพาะบาทไทยเท่านั้น) |
| Scheduled/automated backup ไปนอกเครื่อง (cloud/offsite) | **ไม่มี** | มี `DatabaseBackupHelper.BackupFullSystem()` แต่ backup ไปเฉพาะ local folder (`AppPaths.BackupFolder`) ไม่มี cloud/offsite sync |
| Reporting ตามมาตรฐาน | **บางส่วน** | มีรายงานหลายรูปแบบ: Ledger, Summary Income/Expense, Monthly Summary, Chart (IncomeExpenseReport.vb, FrmReports.vb) แต่ไม่มีงบดุล (Balance Sheet) หรือ งบกำไรขาดทุน แบบมาตรฐานบัญชี |
| Version control ของการตั้งค่าระบบ | **ไม่มี** | `TempleSetting` table ถูก DELETE แล้ว INSERT ใหม่ทุกครั้งที่บันทึก (FrmTempleSetting.vb:325-326) — ไม่มี version history |

### สรุประดับเทียบมาตรฐาน

ระบบอยู่ในระดับ **"ใช้งานได้จริงระดับพื้นฐาน" (Functional MVP)** — สามารถบันทึกรายรับ/รายจ่าย/โอน/ดูรายงานได้จริง แต่ยังมีช่องว่างสำคัญเมื่อเทียบกับระบบบัญชีวัด/มูลนิธิทั่วไปที่ใช้ในทางการ

**ช่องว่าง 3 อันดับแรก:**

1. **ไม่มีระบบผู้ใช้/สิทธิ์ (CRITICAL)** — ไม่มีการ Login, ไม่มี Role, ใครก็เปิดใช้ได้ ในทางปฏิบัติถ้าต้องการให้ผู้ทำบัญชีหลายร่วมใช้ หรือส่งรายงานให้คณะกรรมการวัด ต้องมีการควบคุมสิทธิ์อย่างน้อยระดับ (ผู้บันทึก / ผู้ดู / ผู้บริหาร)

2. **ไม่มี Audit Trail (HIGH)** — ไม่มีบันทึกว่าใครแก้ไข/ลบรายการ何时 ในระบบบัญชี นี่คือจุดที่ถูกตรวจสอบจากผู้สอบบัญชีและหน่วยงานกำกับดูแล

3. **ไม่มี DB Encryption / Backup ไปนอกเครื่อง (MEDIUM)** — ไฟล์ `.accdb` ตั้งอยู่บนเครื่องแบบ plain-text, backup ไปเฉพาะ local folder เท่านั้น ถ้าเครื่องพัง ข้อมูลหายทั้งหมด

---

## ส่วนที่ 3: สถาปัตยกรรมและความสัมพันธ์ของข้อมูล

### 3.1 Data Model Map

```
═══════════════════════════════════════════════════════════════
                    DATA MODEL MAP (Access DB)
═══════════════════════════════════════════════════════════════

[Province]
  PK: ProvinceID (INTEGER)
  Columns: ProvinceName TEXT(100)

[District]
  PK: DistrictID (INTEGER)
  FK: ProvinceID → Province.ProvinceID  ⚠️ NO DB CONSTRAINT
  Columns: DistrictName TEXT(100)

[SubDistrict]
  PK: SubDistrictID (INTEGER)
  FK: DistrictID → District.DistrictID  ⚠️ NO DB CONSTRAINT
  Columns: SubDistrictName TEXT(100), ZipCode TEXT(10)

[Categories]
  PK: ID (COUNTER/AUTO)
  Columns: CategoryName TEXT(200), TranType TEXT(10) -- 'Income' | 'Expense'

[Funds]
  PK: ID (COUNTER/AUTO)
  Columns: FundName TEXT(200)

[BankAccounts]
  PK: ID (COUNTER/AUTO)
  Columns: BankName TEXT(100), AccountNo TEXT(50), AccountName TEXT(200)

[Positions]
  PK: PositionID (COUNTER/AUTO)
  Columns: PositionName TEXT(100), PositionGroup TEXT(20) -- 'พระ' | 'ฆราวาส' | 'ทั่วไป'

[Personnel]
  PK: PersonnelID (COUNTER/AUTO)
  FK: PositionID → Positions.PositionID  ⚠️ NO DB CONSTRAINT
  Columns: Title TEXT(50), FirstName TEXT(100), LastName TEXT(100),
           FullName TEXT(255), PersonType TEXT(50), Phone TEXT(50)

[TempleSetting]
  PK: ID (COUNTER/AUTO) — SINGLE-ROW TABLE (DELETE all + INSERT on save)
  FK: AbbotPersonnelID → Personnel.PersonnelID  ⚠️ NO DB CONSTRAINT
  FK: WaiyawatPersonnelID → Personnel.PersonnelID  ⚠️ NO DB CONSTRAINT
  FK: BookkeeperPersonnelID → Personnel.PersonnelID  ⚠️ NO DB CONSTRAINT
  Columns: TempleCode TEXT(20), TempleName TEXT(200), TempleAddress MEMO,
           Tambon TEXT(100), Amphoe TEXT(100), Province TEXT(100),
           PostCode TEXT(10), TemplePhone TEXT(30),
           PromptPayName TEXT(100), PromptPayID TEXT(50)

[Transactions] *** CENTRAL TABLE ***
  PK: ID (COUNTER/AUTO)
  FK: CategoryID → Categories.ID  ⚠️ NO DB CONSTRAINT
  FK: FundID → Funds.ID  ⚠️ NO DB CONSTRAINT
  FK: BankID → BankAccounts.ID  ⚠️ NO DB CONSTRAINT
  FK: ToFundID → Funds.ID  ⚠️ NO DB CONSTRAINT
  FK: ToBankID → BankAccounts.ID  ⚠️ NO DB CONSTRAINT
  Columns: TranDate DATETIME, TranType TEXT(10) -- 'Income'|'Expense'|'Transfer',
           Detail TEXT(255), Amount CURRENCY, Note MEMO,
           CreateDate DATETIME, ReceiptPath TEXT(255),
           TransactionGUID TEXT(36), ReceiveChannel TEXT(20),
           ReferenceNo TEXT(50)

═══════════════════════════════════════════════════════════════
                  DATA MODEL MAP (SQLite DB)
═══════════════════════════════════════════════════════════════

[temples]
  PK: id INTEGER AUTOINCREMENT
  Columns: temple_name, district, province

[monks]
  PK: id INTEGER AUTOINCREMENT
  FK: temple_id → temples.id  ✅ ENFORCED (PRAGMA foreign_keys = ON)

[saddhiviharika]
  PK: id INTEGER AUTOINCREMENT
  FK: ordination_temple_id → temples.id  ✅ ENFORCED
  FK: residence_temple_id → temples.id  ✅ ENFORCED
  FK: kammavaca_monk_id → monks.id  ✅ ENFORCED
  FK: anusavana_monk_id → monks.id  ✅ ENFORCED
  Columns: seq_no, first_name, last_name, dhamma_name,
           birth_date, ordination_date, age,
           former_occupation, remarks
```

**ข้อสังเกตสำคัญ:**
- Access DB ไม่มี FK constraint จริงทุกจุด — ความสัมพันธ์เป็นแค่ convention ในโค้ด
- SQLite DB (Saddhiviharika) มี FK enforcement จริง (PRAGMA foreign_keys = ON)
- `Transactions` เป็น "God Table" — ตารางเดียวเก็บทั้ง Income, Expense, Transfer โดยใช้ `TranType` แยก
- `TempleSetting` เป็น single-row table ที่ DELETE แล้ว INSERT ใหม่ทุกครั้งที่บันทึก — ไม่มี version history

**ความเสี่ยง Orphan Records:**
- ถ้าลบ Categories/Funds/BankAccounts ที่ถูกใช้อยู่ Transactions จะมี FK ที่ชี้ไปไม่ถึง (ไม่มี DB constraint บังคับ)
- `FrmMasterData` มีปุ่มลบ Categories/Funds/BankAccounts โดยตรง โดยไม่ check ว่ามี Transactions ใช้งานอยู่หรือไม่

### 3.2 Form Navigation Map

```
═══════════════════════════════════════════════════════════════
                      FORM NAVIGATION MAP
═══════════════════════════════════════════════════════════════

frmMain (Main Dashboard)
  │
  ├─ [ShowFormInPanel] → FrmIncome (💰 รายรับ)
  ├─ [ShowFormInPanel] → FrmExpense (💸 รายจ่าย)
  ├─ [ShowFormInPanel] → FrmReports (📊 รายงาน)
  ├─ [ShowFormInPanel] → FrmTransactions (👥 รายการทั้งหมด)
  ├─ [ShowFormInPanel] → FrmTempleSetting (🥇 ข้อมูลวัด)
  ├─ [ShowFormInPanel] → FrmTransfer (🎎 โอนเงินภายใน)
  ├─ [ShowFormInPanel] → FrmMasterData (⚙️ ข้อมูลหลัก)
  ├─ [ShowDialog]      → FrmMultiMachineImport (นำเข้าหลายเครื่อง)
  └─ [ShowDialog]      → FrmExportForTransfer (ส่งออกหลายเครื่อง)

FrmTransactions (รายการทั้งหมด)
  ├─ [ShowFormInPanel] → FrmIncome (แก้ไขรายรับ)
  ├─ [ShowFormInPanel] → FrmExpense (แก้ไขรายจ่าย)
  ├─ [ShowFormInPanel] → FrmTransfer (แก้ไขโอน — ยังไม่สมบูรณ์)
  ├─ [ShowDialog]      → FrmReceiptViewer (ดูใบเสร็จ)
  └─ [ShowDialog]      → OpenFileDialog (เลือกรูปใบเสร็จ)

FrmTempleSetting (ข้อมูลวัด)
  ├─ [ShowDialog]      → FrmPersonnelManagement (จัดการบุคลากร)
  └─ [ShowFormInPanel] → FrmLocationImport (นำเข้าจังหวัด)

FrmLocationImport (นำเข้าจังหวัด)
  └─ [ShowFormInPanel] → FrmTempleSetting (กลับหน้าตั้งค่า)

FrmReports (รายงาน)
  └─ [ShowDialog]      → FrmChartPreview (ดูกราฟเต็มจอ)

FrmIncome / FrmExpense / FrmTransfer
  └─ btnClose → CloseActiveForm() → ShowDashboard()

FrmIncome (btnImportExcel)
  └─ ExcelTransactionImporter.ImportTransactionsFromExcel()

FrmExpense (btnImportExcel)
  └─ ExcelTransactionImporter.ImportTransactionsFromExcel()
```

**ฟอร์มที่เข้าถึงได้จากทุกหน้า:**
- ทุกฟอร์มมีปุ่ม Close → `CloseActiveForm()` → `ShowDashboard()`
- ทุกฟอร์มกด F1 → `HelpSystem.ShowManual()`

**ไม่มี unreachable form** — ทุกฟอร์มถูกเรียกจาก navigation หลักหรือจากฟอร์มอื่น

### 3.3 Data Access Map

| ตาราง | เขียนจากไฟล์ไหนบ้าง (INSERT/UPDATE/DELETE) | อ่านจากไฟล์ไหนบ้าง (SELECT) |
|---|---|---|
| **Transactions** | `AccessTransactionRepository.vb` (Income only), `FrmExpense.vb` (Expense), `FrmTransfer.vb` (Transfer), `FrmTransactions.vb` (Edit/Delete), `FrmExportForTransfer.vb` (export), `FrmMultiMachineImport.vb` (import), `ExcelTransactionImporter.vb` (import) | `frmMain.vb` (dashboard), `FrmIncome.vb`, `FrmExpense.vb`, `FrmReports.vb`, `FrmTransactions.vb`, `TransactionExcelExporter.vb` |
| **Categories** | `AccessDbProvider.vb` (Seed), `FrmMasterData.vb` (CRUD), `ExcelTransactionImporter.vb` (auto-create) | `FrmIncome.vb`, `FrmExpense.vb`, `FrmReports.vb`, `FrmTransactions.vb`, `FrmMasterData.vb`, `frmMain.vb` |
| **Funds** | `AccessDbProvider.vb` (Seed), `FrmMasterData.vb` (Add/Delete), `FrmTransfer.vb` (引用), `ExcelTransactionImporter.vb` (auto-create) | `FrmIncome.vb`, `FrmExpense.vb`, `FrmTransfer.vb`, `FrmReports.vb`, `FrmTransactions.vb`, `FrmMasterData.vb`, `frmMain.vb` |
| **BankAccounts** | `AccessDbProvider.vb` (Seed), `FrmMasterData.vb` (Add/Delete), `ExcelTransactionImporter.vb` (auto-create) | `FrmIncome.vb`, `FrmExpense.vb`, `FrmTransfer.vb`, `FrmReports.vb`, `FrmTransactions.vb`, `FrmMasterData.vb`, `frmMain.vb` |
| **TempleSetting** | `FrmTempleSetting.vb` (DELETE all + INSERT) | `FrmTempleSetting.vb`, `FrmReports.vb` (via ReportEngine), `frmMain.vb` |
| **Personnel** | `AccessDbProvider.vb` (Seed), `FrmPersonnelManagement.vb` (CRUD) | `FrmTempleSetting.vb`, `AccessDbProvider.vb` |
| **Positions** | `AccessDbProvider.vb` (Seed + Update) | `FrmTempleSetting.vb`, `FrmPersonnelManagement.vb` |
| **Province/District/SubDistrict** | `FrmLocationImport.vb` (CSV import) | `FrmTempleSetting.vb` |

**ตารางที่ถูกแก้ไขจากหลายจุด (เสี่ยง logic กระจัดกระจาย):**
- `Transactions` — **7 ไฟล์** เขียนถึง! นี่คือตารางที่มีความเสี่ยงสูงสุด
- `Categories` — 3 ไฟล์เขียนถึง
- `Funds` — 3 ไฟล์เขียนถึง
- `BankAccounts` — 3 ไฟล์เขียนถึง

### 3.4 Service Layer Coverage

**สถาปัตยกรรมที่ออกแบบไว้:**

```
UI Layer (Forms)
    ↓ (should call)
TransactionService (Business Logic + Validation)
    ↓ (calls)
ITransactionRepository (Data Access Abstraction)
    ↓ (implemented by)
AccessTransactionRepository / (future SqlTransactionRepository)
    ↓ (uses)
IDbProvider (Database Connection Abstraction)
    ↓ (implemented by)
AccessDbProvider / SqlServerDbProvider
```

**ความเป็นจริง — การใช้งานจริง:**

| ฟอร์ม | ผ่าน TransactionService? | ผ่าน Db facade? | เขียน SQL ตรง? |
|---|---|---|---|
| FrmIncome (INSERT/UPDATE) | ✅ ใช้ `_txnService.SaveIncome()` / `UpdateIncome()` | ✅ ใช้ `Db.OpenConn()` | ❌ |
| FrmIncome (UPDATE ReceiptPath) | ✅ ใช้ `_txnService.UpdateReceiptPath()` | ❌ | ❌ |
| FrmExpense (INSERT/UPDATE) | ❌ **ไม่ใช้** | ✅ ใช้ `Db.InsertAndGetId()` / `Db.ExecuteNonQuery()` | ❌ |
| FrmExpense (UPDATE ReceiptPath) | ❌ | ❌ | ✅ **ใช้ OleDbCommand ตรง** |
| FrmTransfer (INSERT) | ❌ **ไม่ใช้** | ✅ ใช用 `Db.InsertAndGetId()` | ❌ |
| FrmTransactions (DELETE) | ❌ **ไม่ใช้** | ❌ | ✅ **ใช้ OleDbCommand + OleDbTransaction ตรง** |
| FrmTransactions (UPDATE) | ❌ **ไม่ใช้** | ✅ ใช้ `Db.ExecuteNonQuery()` | ❌ |
| FrmTransactions (UPDATE ReceiptPath) | ❌ | ✅ ใช้ `Db.ExecuteNonQuery()` | ❌ |
| ExcelTransactionImporter (INSERT) | ❌ **ไม่ใช้** | ✅ ใช用 `Db.ExecuteNonQuery()` | ❌ |

**สรุป覆盖率:**
- **ผ่าน TransactionService จริง:** ~20% (เฉพาะ FrmIncome เท่านั้น)
- **ผ่าน Db facade:** ~60%
- **เขียน SQL/OleDbCommand ตรง:** ~20% (FrmExpense ReceiptPath, FrmTransactions DELETE)
- **FrmExpense, FrmTransfer, FrmTransactions ไม่ได้ผ่าน TransactionService เลย** — TransactionService ครอบคลุมเฉพาะ Income เท่านั้น

---

## ส่วนที่ 4: Dead Code Inventory

### 4.1 ฟังก์ชัน/Sub ที่ไม่มีใครเรียก

| ไฟล์ | ชื่อฟังก์ชัน | บรรทัด | สถานะ |
|---|---|---|---|
| `FrmTransfer.vb` | `Private Function MakeLbl(...)` | 25 | ❌ **ไม่ถูกเรียก** — ประกาศแล้วไม่มีการใช้งาน |
| `FrmReports.vb` | `Private Function MakeLbl(...)` | 29 | ❌ **ไม่ถูกเรียก** — ประกาศแล้วไม่มีการใช้งาน |
| `FrmReports.vb` | `Private Sub FrmReports_KeyDown(...)` | 59 | ✅ ถูก `Handles Me.KeyDown` โดย WinForms framework |
| `frmMain.vb` | `LoadSampleDashboardData()` | 577 | ✅ ถูกเรียกเป็น fallback ใน `LoadActualDashboardFromDb()` |
| `frmMain.vb` | `LoadDonationSample()` | 597 | ✅ ถูกเรียกเป็น fallback ใน `NavMenu_Click` |
| `frmMain.vb` | `LoadExpenseSample()` | 617 | ✅ ถูกเรียกเป็น fallback |
| `frmMain.vb` | `LoadReportSample()` | 677 | ✅ ถูกเรียกเป็น fallback |
| `frmMain.vb` | `LoadMemberSample()` | 744 | ⚠️ **ไม่แน่ใจ 100%** — ประกาศอยู่แต่ไม่พบที่ถูกเรียกจากการ grep อาจเป็น dead code |
| `frmMain.vb` | `ShowPlaceholder(...)` | 508 | ⚠️ ไม่พบที่ถูกเรียก — อาจเป็น dead code (เป็นฟังก์ชันสำหรับวาง placeholder กรณีฟอร์มยังไม่พร้อม) |
| `FrmTransactions.vb` | `dgvTransactions_CellContentClick(...)` | 780 | ⚠️ Handler ว่างเปล่า — ไม่มีโค้ดภายใน (auto-generated แต่ไม่ได้ลบ) |
| `FrmTransactions.vb` | `lblSearch_Click(...)` | 784 | ⚠️ Handler ว่างเปล่า |
| `Program.vb` | `C(msg As String)` | 16 | ⚠️ Console output helper — ถูกเรียกใน `BootLog` แต่เป็น debug helper ไม่ใช่ production code |

### 4.2 ตัวแปร field-level ที่ไม่ได้ใช้จริง

| ไฟล์ | ตัวแปร | บรรทัด | สถานะ |
|---|---|---|---|
| `frmMain.vb` | `_compactOverviewMode` | 14 | ⚠️ ถูก set โดย `SetOverviewCompactMode()` แต่ไม่พบที่อ่านค่าจริง |

### 4.3 ไฟล์ทั้งไฟล์ที่ไม่ถูก reference

- **ไม่มีไฟล์ `.vb` ที่ไม่ถูก compile** — ทุกไฟล์มี `<Compile Include="...">` ใน `.vbproj`
- **ไม่มีไฟล์ `.Designer.vb` ที่ไม่ถูก compile** — ทุก Designer file มี `<DependentUpon>`
- `FrmChartPreview.vb` ไม่มี Designer.vb file — สร้าง UI ด้วย code (ไม่ผิด แต่ไม่สอดคล้องกับ pattern อื่น)
- `MessageBoxHelper.vb` — อยู่นอก Namespace `TempleAccounting` (ไม่มี Namespace) — ถูก compile แต่ไม่สอดคล้องกับโครงสร้าง

### 4.4 Commented-out Code Block ขนาดใหญ่

| ไฟล์ | บรรทัด | คำอธิบาย |
|---|---|---|
| ไม่พบ commented-out code block ขนาดใหญ่ | — | โค้ดส่วนใหญ่ไม่มี commented-out blocks ขนาดใหญ่ค้างอยู่ — เป็นเรื่องดี |
| `frmMain.Designer.vb` (lines commented out) | หลายจุด | มี `' lblCard1Icon.Text = "📿"` แบบ comment-out หลายจุดใน `LoadSampleDashboardData()` etc. — แต่เป็นเล็กน้อย |

### 4.5 Legacy/Migration Scaffolding ที่ทำงานเสร็จแล้ว

| ไฟล์ | ฟังก์ชัน | บรรทัด | สถานะ |
|---|---|---|---|
| `AccessDbProvider.vb` | `MigratePositions()` | 75 | ⚠️ Migration ที่ทำงานเฉพาะเมื่อคอลัมน์ `PositionGroup` ยังไม่มี — เมื่อ schema ใหม่สุด migration นี้จะไม่ trigger อีก แต่ยังถูกเรียกทุกครั้ง |
| `AccessDbProvider.vb` | `MigrateTempleSetting()` | 93 | ⚠️ เหมือนกัน — เช็คคอลัมน์เก่า AbbotName/WaiyawatName/etc. แล้ว DROP — เมื่อ DROP แล้วจะไม่ trigger อีก |
| `AccessDbProvider.vb` | `MigratePersonnel()` | 137 | ⚠️ เช็ค PositionID — เมื่อมีแล้วจะไม่ trigger |
| `AccessDbProvider.vb` | `MigrateTransactions()` | 157 | ⚠️ เช็ค ToFundID, ToBankID, ReceiptPath, TransactionGUID, ReceiveChannel, ReferenceNo — เมื่อมีแล้วจะไม่ trigger |
| `AppPaths.vb` | `MigrateReceiptsToMaster()` | 252 | ⚠️ One-time migration ที่เรียกทุกครั้ง startup — เช็ค legacy receipt folders แล้ว copy ไฟล์ไป master dir. จะไม่ทำอะไรเมื่อไม่มีไฟล์ใหม่ แต่ยัง iterate ผ่าน folders ทุกครั้ง |

**หมายเหตุ:** Legacy migration scaffolding ข้างต้น **ไม่ใช่ปัญหา** แต่เป็น inefficiency เล็กน้อย — ทุกฟังก์ชันมี guard condition ที่จะ skip เมื่อไม่จำเป็น ไม่ควรลบเพราะต้องรองรับผู้ใช้ที่ยังมี schema เก่า

---

## ส่วนที่ 5: Gaps & Latent Issues

### 5.1 สิ่งที่ควรพัฒนาเพิ่ม (จาก Section 2)

| # | หัวข้อ | ความยากง่าย | ลำดับความสำคัญ |
|---|---|---|---|
| G1 | ระบบผู้ใช้ + สิทธิ์ (Login + Role) | กลาง | CRITICAL |
| G2 | Audit trail (ตาราง AuditLog) | เล็ก-กลาง | HIGH |
| G3 | DB encryption / Password protection | เล็ก | HIGH |
| G4 | Cloud/offsite backup | กลาง | HIGH |
| G5 | Print receipt (ใบเสร็จรับเงินจริง) | กลาง | MEDIUM |
| G6 | Bank reconciliation | กลาง-ใหญ่ | MEDIUM |
| G7 | Multi-fund accounting แบบ double-entry | ใหญ่ | MEDIUM |
| G8 | งบดุล/งบกำไร-ขาดทุน ตามมาตรฐานบัญชี | ใหญ่ | LOW |
| G9 | Approval workflow สำหรับรายจ่ายก้อนใหญ่ | กลาง | LOW |
| G10 | Version control สำหรับ TempleSetting | เล็ก | LOW |

### 5.2 ปัญหาแฝงที่ยังไม่เคยถูกพูดถึงในรายงานก่อนหน้า

#### 5.2.1 Empty Catch Blocks (Silent Exception Swallowing)

พบ **63+ จุด** ที่มี `Catch` block ว่างเปล่า (ไม่ log, ไม่แสดง error) กระจายตัวทั่วโปรเจกต์:

| ไฟล์ | จำนวนจุดว่างเปล่า | ตัวอย่างบรรทัด |
|---|---|---|
| `AppPaths.vb` | 8 จุด | 38, 57, 131, 205, 270, 274, 302, 310, 318 |
| `frmMain.vb` | 10 จุด | 24, 69, 190, 206, 226, 235, 244, 253, 275, 283, 291 |
| `Program.vb` | 5 จุด | 19, 30, 53, 74, 85, 160 |
| `FrmExportForTransfer.vb` | 4 จุด | 244, 310, 583, 592 |
| `FrmMultiMachineImport.vb` | 4 จุด | 280, 745, 1083, 1109 |
| `FrmReports.vb` | 4 จุด | 428, 443, 596, 619 |
| `FrmExpense.vb` | 1 จุด | 266 |
| `FrmIncome.vb` | 1 จุด | 313 |
| `FrmChartPreview.vb` | 2 จุด | 318, 329 |
| `AccessDbProvider.vb` | 2 จุด | 128, 199 |
| `TransactionExcelExporter.vb` | 1 จุด | 42 |
| `DatabaseBackupHelper.vb` | 1 จุด | 185 |
| `DesktopShortcutHelper.vb` | 1 จุด | 35 |
| `frmMain.UiHelpers.vb` | 3 จุด | 91, 97, 160 |
| `AppPreferences.vb` | 2 จุด | 86, 103 |
| `Reports/IncomeExpenseReport.vb` | 3 จุด | 854, 906, 931 |
| `FrmTransactions.vb` | 2 จุด | 480, 931 |
| `FrmLocationImport.vb` | 1 จุด | 81 |

**ผลกระทบ:** Error หายไปเงียบๆ ทำให้ bug ยากต่อการ debug. ตัวอย่างที่เสี่ยง:
- `frmMain.vb:69` — Catch ใน `FrmMain_Load` ว่างเปล่า — ถ้า Initialize ล้มเหลว program จะเปิดแต่ไม่ทำงาน
- `FrmReports.vb:428` — Catch ใน help hint setup ว่างเปล่า — ถ้า setup ไม่สำเร็จ user ไม่รู้

#### 5.2.2 SQL Injection Risk (String Concatenation)

| ไฟล์ | บรรทัด | ตัวอย่าง | ความเสี่ยง |
|---|---|---|---|
| `FrmIncome.vb` | 60 | `"SELECT * FROM Transactions WHERE ID = " & id` | ⚠️ id เป็น Integer จาก code ไม่ใช่ user input โดยตรง ความเสี่ยงต่ำ |
| `FrmExpense.vb` | 56 | `"SELECT * FROM Transactions WHERE ID = " & id` | ⚠️ เหมือนกัน |
| `FrmIncome.vb` | 190 | `"SELECT ... WHERE TranType LIKE '%รับ%' OR TranType = 'Income'"` | ⚠️ Hardcoded string ความเสี่ยงต่ำ |
| `FrmReports.vb` | 177-200 | `fromWhere.Replace("WHERE", "WHERE t.TranType='Income' AND ")` | ⚠️ String replace สร้าง SQL — ไม่ใช่ user input ตรง แต่ pattern นี้ Fragile |

**สรุป:** ไม่พบ SQL injection ร้ายแรงที่มาจาก user input โดยตรง (พารามิเตอร์ทั้งหมดใช้ `@param` อย่างถูกต้อง). แต่ pattern `"WHERE ID = " & id` ควรใช้ parameterized query เสมอ

#### 5.2.3 Numeric Type Inconsistency (Decimal vs Double)

| จุดที่พบ | ไฟล์ | บรรทัด |
|---|---|---|
| `Decimal.TryParse(amount)` → `Convert.ToDecimal(dr("Amount"))` | `FrmIncome.vb` | 606, 630 |
| `Db.ToDecimalOrZero(row("Amount"))` → `Decimal` | ทุกฟอร์ม | หลายจุด |
| `ExcelTransactionImporter.vb:TryParseImportAmount()` — แปลง `Double` → `Decimal` | `ExcelTransactionImporter.vb` | 336-340 |
| Access CURRENCY type → .NET Decimal (ตรงกัน) | ทั่วไป | ✅ |
| SQL Server MONEY type → .NET Decimal (ตรงกัน) | ทั่วไป | ✅ |

**สรุป:** โดยรวมจัดการ Decimal ได้ถูกต้อง — Access CURRENCY → .NET Decimal ตรงกัน ไม่พบ rounding error ที่ชัดเจน แต่ Excel import อาจแปลง Double เป็น Decimal แล้วสูญเสีย precision เล็กน้อย

#### 5.2.4 Single-Instance Form Check

**ไม่พบการตรวจสอบ single-instance ในฟอร์มใดๆ** — ทุกฟอร์มถูกสร้างใหม่ทุกครั้งที่เรียก (`New FrmIncome()`, `New FrmExpense()`, etc.) ผ่าน `ShowFormInPanel()` ซึ่งมี `CloseActiveForm()` ปิดฟอร์มเก่าก่อน

**วิเคราะห์:** ไม่ใช่ปัญหาจริงในระบบนี้ เพราะ:
- `ShowFormInPanel()` เรียก `CloseActiveForm()` ก่อน → ปิดฟอร์มเก่าก่อนเปิดใหม่
- ไม่มีฟอร์มที่เปิดแบบ top-level (ทุกฟอร์ม embed ใน panel)
- **ข้อยกเว้น:** `FrmMultiMachineImport` และ `FrmExportForTransfer` เปิดแบบ `ShowDialog()` — ถ้า user คลิกซ้ำเร็ว อาจเปิด 2 instance ได้

#### 5.2.5 ปัญหาอื่นๆ ที่พบใหม่

| # | หัวข้อ | ไฟล์ | บรรทัด | รายละเอียด |
|---|---|---|---|---|
| L1 | **Debug.WriteLine ใน Production** | `FrmReports.vb` | 383, 413 | `System.Diagnostics.Debug.WriteLine(String.Format("MONTH=..."))` — คำสั่ง debug ที่ไม่ควรอยู่ใน release build |
| L2 | **Debug.WriteLine ใน Production** | `FrmTempleSetting.vb` | 312-320 | 7 บรรทัด Debug.WriteLine สำหรับ debug save |
| L3 | **TempleSetting DELETE+INSERT pattern** | `FrmTempleSetting.vb` | 325-326 | `DELETE FROM TempleSetting` แล้ว INSERT ใหม่ทุกครั้ง — ถ้า INSERT ล้มเหลวหลัง DELETE ข้อมูลจะหาย |
| L4 | **FrmReports Catch ex As Exception : Throw** | `FrmReports.vb` | 364, 447 | `Catch ex As Exception : Throw` — re-throw ทันทีโดยไม่ log, ไม่ clean up resource |
| L5 | **Hardcoded C:\LineDownloads** | `FrmIncome.vb`, `FrmExpense.vb` | 615, 538 | `If IO.Directory.Exists("C:\LineDownloads")` — hardcode path เฉพาะเครื่องผู้ใช้ |
| L6 | **MessageBoxHelper.Font size** | `MessageBoxHelper.vb` | 9 | `Private Shared ReadOnly LargeFont As New Font("Tahoma", 8)` — ตั้งชื่อ "LargeFont" แต่ size 8pt (เล็กกว่า default) |

---

## ตาราง Priority Summary

| # | หัวข้อ | ความรุ้งแรง | ไฟล์ที่เกี่ยวข้อง | ประเภท |
|---|---|---|---|---|
| **P1** | ไม่มีระบบผู้ใช้/สิทธิ์ | 🔴 CRITICAL | ทั้งระบบ (ไม่มี code เลย) | Feature Gap |
| **P2** | ไม่มี Audit Trail | 🔴 HIGH | ทั้งระบบ | Feature Gap |
| **P3** | Empty Catch blocks 63+ จุด | 🟡 MEDIUM | ดู Section 5.2.1 ทั้งหมด | Code Quality |
| **P4** | TransactionService ครอบคลุมเฉพาะ Income | 🟡 MEDIUM | `TransactionService.vb`, `FrmExpense.vb`, `FrmTransfer.vb`, `FrmTransactions.vb` | Architecture |
| **P5** | FrmTransactions DELETE ใช้ OleDbCommand ตรง bypass Db facade | 🟡 MEDIUM | `FrmTransactions.vb:459` | Architecture |
| **P6** | TempleSetting DELETE+INSERT (ไม่มี transaction) | 🟡 MEDIUM | `FrmTempleSetting.vb:325-326` | Data Integrity |
| **P7** | DB ไม่มี encryption/password | 🟡 MEDIUM | ทั้งระบบ (.accdb ตั้ง plain-text) | Security |
| **P8** | ไม่มี cloud/offsite backup | 🟡 MEDIUM | `DatabaseBackupHelper.vb` | Operations |
| **P9** | Debug.WriteLine ใน Production code | 🟢 LOW | `FrmReports.vb`, `FrmTempleSetting.vb` | Code Quality |
| **P10** | Hardcoded C:\LineDownloads path | 🟢 LOW | `FrmIncome.vb`, `FrmExpense.vb` | Portability |
| **P11** | Dead code: MakeLbl ใน FrmTransfer + FrmReports | 🟢 LOW | `FrmTransfer.vb:25`, `FrmReports.vb:29` | Code Quality |
| **P12** | Empty event handlers (dgvTransactions_CellContentClick, lblSearch_Click) | 🟢 LOW | `FrmTransactions.vb:780,784` | Code Quality |
| **P13** | NoWarn WFO1000 ซ่อน WinForms warning | 🟢 LOW | `TempleAccounting.vbproj:14` | Build Config |
| **P14** | Hardcoded seed data (Personnel) ที่ INSERT ทุกครั้ง startup | 🟢 LOW | `AccessDbProvider.vb:265-298` | Efficiency |

---

## ภาคผนวก: วิธีตรวจสอบ (สำหรับให้ผู้ใช้ทวนซ้ำได้เอง)

### นับไฟล์และบรรทัด
```bash
# นับไฟล์ .vb ทั้งหมด (ไม่รวม Designer)
find . -name "*.vb" ! -name "*.Designer.vb" ! -path "./obj/*" | wc -l

# นับบรรทัดโค้ดรวม
find . -name "*.vb" ! -name "*.Designer.vb" ! -path "./obj/*" | xargs wc -l | tail -1

# นับฟอร์ม (Designer files)
find . -name "*.Designer.vb" ! -path "./obj/*" | wc -l
```

### Build Health
```bash
dotnet build -c Release 2>&1 | grep -E "Warning|Error|Build succeeded"
```

### ค้นหา Empty Catch Blocks
```bash
# Catch ว่างเปล่า (ไม่มี ex As Exception ไม่มี log)
grep -rn "Catch\s*$" --include="*.vb" --exclude="*.Designer.vb" --exclude-dir=obj

# จำนวนจุดทั้งหมด
grep -rn "Catch\s*$" --include="*.vb" --exclude="*.Designer.vb" --exclude-dir=obj | wc -l
```

### ค้นหา TODO/FIXME/HACK
```bash
grep -rni "TODO\|FIXME\|HACK" --include="*.vb" --exclude="*.Designer.vb" --exclude-dir=obj
```

### ค้นหา Debug.WriteLine ใน Production
```bash
grep -rn "Debug\.Write" --include="*.vb" --exclude="*.Designer.vb" --exclude-dir=obj
```

### ค้นหา SQL Injection Risk (string concatenation ใน query)
```bash
grep -rn '"SELECT.*" &' --include="*.vb" --exclude="*.Designer.vb" --exclude-dir=obj
grep -rn '"UPDATE.*" &' --include="*.vb" --exclude="*.Designer.vb" --exclude-dir=obj
```

### ค้นหา Bypass TransactionService
```bash
# ดูว่า frmExpense/frmTransfer/frmTransactions เขียน SQL ตรง
grep -n "Db\.\(InsertAndGetId\|ExecuteNonQuery\|DbScalar\)" Forms/FrmExpense.vb Forms/FrmTransfer.vb Forms/FrmTransactions.vb
grep -n "OleDbCommand\|OleDbConnection\|OleDbTransaction" Forms/FrmExpense.vb Forms/FrmTransfer.vb Forms/FrmTransactions.vb
```

### ค้นหา Dead Code (MakeLbl)
```bash
grep -rn "MakeLbl" --include="*.vb" --exclude="*.Designer.vb"
```

### ค้นหา Files ไม่ได้ Compile
```bash
# เปรียบเทียบไฟล์บนดิสก์กับ .vbproj
find . -name "*.vb" ! -path "./obj/*" ! -path "./.git/*" | sed 's|^\./||' | while read f; do
  grep -q "\"$f\"" TempleAccounting.vbproj || echo "NOT IN PROJ: $f"
done
```

### ตรวจสอบ Table Schema จาก Access
```bash
# ใช้ mdb-tools (Linux) หรือ OleDB query
echo "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='TABLE'" | \
  dotnet script --no-cache - << 'EOF'
EOF
```

---

*รายงานนี้ตรวจสอบจาก source code จริง ณ วันที่ 20 สิงหาคม 2569 — ไม่ได้คัดลอกข้อสรุปจากเอกสาร PROJECT_PROFILE หรือ AUDIT_REPORT เดิม*
