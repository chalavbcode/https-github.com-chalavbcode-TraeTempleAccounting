# FIX_REPORT — แก้ 3 บั๊กอันดับแรก (B8, A6, A1)

**วันที่:** 2026-08-19
**อ้างอิง:** `AUDIT_REPORT.md`
**ผล build:** `dotnet build -c Release` → **Build succeeded. 0 Warning(s) / 0 Error(s)** (ยืนยันหลังแก้แต่ละข้อครบ)

---

## 1. Backup ก่อนแก้

- **Path:** `C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422\Backup\TempleAccounting_before_B8A6A1_fix_20260819.accdb`
- ต้นทางที่ backup: `Database\TempleAccounting.accdb` (ไฟล์ DB หลักของโปรเจกต์ — `AppPaths.DatabaseFile` จะหาโฟลเดอร์ `Database\` ใต้ project root ก่อนเสมอ)
- หมายเหตุ: ยังมี backup เก่าที่ระบบทำเองสะสมใน `bin\Release\net10.0-windows\Backup\` อยู่แล้ว ไม่เกี่ยวข้องกับ backup เฉพาะครั้งนี้

---

## 2. ไฟล์ที่แก้จริง

| ไฟล์ | ข้อ | การแก้ |
|---|---|---|
| `Forms\FrmIncome.vb` | B8 | `CalculateMonthlySummary` — แทนเงื่อนไข `MONTH/YEAR` ด้วยช่วงวันที่ parameter |
| `Forms\FrmExpense.vb` | B8 | `CalculateMonthlySummary` — เช่นเดียวกัน |
| `frmMain.vb` | B8 | `RefreshDashboardOverview` — แก้ 3 การ์ด (DonorCount / IncomeMonth / ExpenseMonth) |
| `frmMain.vb` | A6 | `LoadMonkSample` — เปลี่ยน query เป็น JOIN `Personnel` |
| `Forms\FrmTransactions.vb` | A1 | `btnEdit_Click` — เพิ่ม guard `_isEditing` เรียก `SaveSelectedRowEdits()` |

---

## 3. Diff สำคัญ

### ข้อ 1 — B8: ลบ hardcoded `YEAR(TranDate) = 2026` (5 จุด)

**FrmIncome.vb / FrmExpense.vb** (รูปแบบเดียวกัน ต่างแค่ `Income/รับ` vs `Expense/จ่าย`):

```vb
' ก่อน
"AND MONTH(TranDate) = MONTH(Date()) " &
"AND (YEAR(TranDate) = YEAR(Date()) OR YEAR(TranDate) = 2026)"

' หลัง
Dim firstOfMonth As Date = New Date(Date.Today.Year, Date.Today.Month, 1)
Dim firstOfNextMonth As Date = firstOfMonth.AddMonths(1)
...
"AND TranDate >= " & Db.AccessDateLiteral(firstOfMonth) & " " &
"AND TranDate < " & Db.AccessDateLiteral(firstOfNextMonth)
```

**frmMain.vb** — ประกาศ `firstOfMonth`/`firstOfNextMonth` ครั้งเดียวใน `Using conn` แล้วใช้กับ 3 การ์ด:
- `DonorCount` → `SELECT COUNT(*)`
- `IncomeMonth` → `SELECT SUM(Amount)` (รายรับ)
- `ExpenseMonth` → `SELECT SUM(Amount)` (รายจ่าย)

**ยืนยัน:** grep `YEAR(TranDate) = 2026` ทั้งโปรเจกต์ = 0 match (ไม่เหลือ hardcode ในเงื่อนไข "เดือนนี้")

### ข้อ 2 — A6: `LoadMonkSample()` JOIN Personnel

```vb
' ก่อน
Dim row = Db.GetTable(conn, "SELECT TOP 1 TempleName, AbbotName, WaiyawatName, BookkeeperName FROM TempleSetting ORDER BY ID DESC")

' หลัง
Dim row = Db.GetTable(conn,
    "SELECT TOP 1 ts.TempleName, " &
    "a.FullName AS AbbotName, w.FullName AS WaiyawatName, b.FullName AS BookkeeperName " &
    "FROM ((TempleSetting ts " &
    "LEFT JOIN Personnel a ON ts.AbbotPersonnelID = a.PersonnelID) " &
    "LEFT JOIN Personnel w ON ts.WaiyawatPersonnelID = w.PersonnelID) " &
    "LEFT JOIN Personnel b ON ts.BookkeeperPersonnelID = b.PersonnelID " &
    "ORDER BY ts.ID DESC")
```

- ใช้ `LEFT JOIN` + alias เดิม (`AbbotName`/`WaiyawatName`/`BookkeeperName`) → โค้ด `IsDBNull` guard ด้านล่างไม่ต้องแก้
- เมื่อยังไม่ตั้งค่า (ID เป็น NULL) `LEFT JOIN` ให้ `NULL` → guard แสดง "-" ไม่ throw
- ยืนยัน schema จริงแล้ว: `Personnel(PersonnelID, FullName)`, `TempleSetting(AbbotPersonnelID, WaiyawatPersonnelID, BookkeeperPersonnelID)` ตรงตามที่เขียน

### ข้อ 3 — A1: wire `SaveSelectedRowEdits()` เข้า `btnEdit_Click`

```vb
Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
    If _isEditing Then
        SaveSelectedRowEdits()
        Return
    End If
    ' ... Select Case เดิม
```

**ตรวจสอบก่อนแก้แล้ว (ตามที่ brief กำหนด):**
1. `SaveSelectedRowEdits()` **รองรับ Transfer ครบจริง** — อ่าน `ToFundID`/`ToBankID` และ UPDATE ถูกคอลัมน์ (Income/Expense ล้าง `ToFundID`/`ToBankID` เป็น NULL, Transfer คงไว้) ไม่ผิดตาราง/คอลัมน์
2. ฟังก์ชัน **ทำ reset + reload เองอยู่แล้ว** — หลังบันทึกสำเร็จเรียก `ExitEditMode()` (ตั้ง `_isEditing=False`, `btnEdit.Text="📝 แก้ไข"`) แล้ว `LoadData()` รีเฟรชกริด → ไม่ต้องเพิ่มเอง
3. **ไม่กระทบ Income/Expense** — เส้นทางแก้ไข Income/Expense เปิด `FrmIncome`/`FrmExpense` ผ่าน `frm.EditID` (ไม่ใช้ `_isEditing`) และตัวแปร `_isEditing` จะเป็น `True` เฉพาะโหมดแก้ไข Transfer เท่านั้น guard ใหม่จึงไม่ชนกับ flow เดิม

---

## 4. สิ่งที่ยังไม่ได้ทดสอบด้วยมือ (ให้ผู้ใช้ทดสอบเองก่อน deploy)

ตัว agent ไม่สามารถรัน UI จริงได้ จึงขอให้ผู้ใช้ตรวจสอบเอง:

1. **B8 (regression):** เปิด `FrmIncome` / `FrmExpense` / หน้า dashboard แล้วยืนยันว่ายอด "เดือนนี้" ยังตรงกับข้อมูลเดือนสิงหาคม 2026 เหมือนเดิม (โดยเฉพาะถ้ามีข้อมูลวันที่ 31 ก.ค. หรือ 1 ก.ย. ต้องไม่ถูกนับข้าม/นับเกินช่วงเดือน)
2. **A6:** ตั้งค่าชื่อเจ้าอาวาส/ไวยาวัจกร/ผู้ทำบัญชีใน `FrmTempleSetting` แล้วกลับมา dashboard → การ์ดต้องแสดงชื่อจริง ไม่ใช่ "-"; ถ้ายังไม่ตั้งค่า ต้องแสดง "-" และไม่ crash
3. **A1 (กรณีหลัก):** `FrmTransactions` → เลือกแถว Transfer → กด "📝 แก้ไข" → แก้ค่าในเซลล์ → กด "💾 บันทึกแก้ไข" → ปิด/รีเฟรช → ค่าต้องคงอยู่จริง
4. **A1 (regression):** ทดสอบว่าแก้ไขรายการ Income/Expense เดิมยังทำงานปกติ (เปิดฟอร์มแก้ไข → บันทึก → ค่ากลับมาถูกต้อง)

---

## 5. หมายเหตุเพิ่มเติม

- ค่าตำแหน่งจริงในโค้ดตรงกับที่ brief ระบุเกือบทั้งหมด (`FrmIncome.vb` L263, `FrmExpense.vb` L217, `frmMain.vb` L347/354/361, `LoadMonkSample` L767, `btnEdit_Click` L645, `SaveSelectedRowEdits` L828) — ไม่พบการขยับที่ต้องเบี่ยงเบนจากคำอธิบาย
- ไม่มีการแตะ `New Date(2026, ...)` ที่เป็นค่า default ของ DateTimePicker ใน Designer (อยู่นอกขอบเขตตามที่ brief กำหนด)

---

## 6. A2 — ลบ Debug Harness ออกจาก `FrmTransactions.vb`

**วันที่:** 2026-08-19
**ผล build:** `dotnet build -c Release` → **Build succeeded. 0 Warning(s) / 0 Error(s)**

### 6.1 ขอบเขตที่ลบ

| รายการ | รายละเอียด |
|---|---|
| **Region `"debug-point Z:debug-report"`** | ลบออกทั้ง Region — ประกอบด้วย:<br>• `Private Const DebugSessionId As String`<br>• `Private Const DebugRunId As String`<br>• `Private Shared Function FindUpwards(...)` (ค้นหาไฟล์ .env ขึ้นไปหลายระดับ)<br>• `Private Shared Function GetDebugServerUrl()` (อ่าน env หรือ fallback `http://127.0.0.1:7777/event`)<br>• `Private Shared Sub DebugReport(...)` (สร้าง JSON payload → HttpClient POST แบบ blocking 500ms timeout) |
| **Call site A:form-load** | ใน `FrmTransactions_Load` — `DebugReport("A", ..., "load-start", ...)` |
| **Call site A:ensure-schema** | ใน `FrmTransactions_Load` — `DebugReport("A", ..., "ensure-schema-ok", ...)` |
| **Call site B:loaddata-start** | ใน `LoadData` — `DebugReport("B", ..., "loaddata-start", ...)` พร้อม Dictionary param 9 ค่า |
| **Call site B:db-stats** | ใน `LoadData` — ทั้ง block ประกอบด้วย 7 query + `DebugReport("B", ..., "db-stats", ...)` (ดูรายละเอียดด้านล่าง) |
| **Call site C:loaddata-result** | ใน `LoadData` — `DebugReport("C", ..., "query-result", ...)` |
| **Call site D:loaddata-ex** | ใน `LoadData` — `DebugReport("D", ..., "exception", ...)` (ใน Catch block) |
| **ตัวแปรที่เป็น orphan หลังลบ** | `tranDateExpr`, `d1`, `d2`, `d1Literal`, `d2Literal` ใน `LoadData` — ถูกลบด้วยเพราะไม่มีผู้ใช้อีก (`BuildLedgerQuery` สร้างตัวแปรเหล่านี้เองภายใน) |
| **Imports ที่ลบ** | `Imports System.Net.Http`<br>`Imports System.Text`<br>`Imports System.Text.Json`<br>(ทั้ง 3 ถูกใช้เฉพาะใน debug harness เท่านั้น — ยืนยันด้วย grep ในไฟล์แล้ว) |

### 6.2 ผลวิเคราะห์ db-stats (7 query)

ทั้ง 7 query ถูกลบออกทั้งหมด เพราะผลลัพธ์ถูกส่งเข้า `DebugReport` เท่านั้น ไม่มีการนำไปใช้แสดงผลหรือคำนวณที่อื่น:

| Query | ตัวแปร | ผลลัพธ์ถูกใช้ที่อื่น? | ตัดสินใจ |
|---|---|---|---|
| `SELECT COUNT(*) FROM Transactions` | `totalCount` | ❌ ไม่ | ลบ |
| `SELECT MIN(TranDate) FROM Transactions` | `minDb` | ❌ ไม่ | ลบ |
| `SELECT MAX(TranDate) FROM Transactions` | `maxDb` | ❌ ไม่ | ลบ |
| `SELECT MIN(Year(TranDate)) FROM Transactions` | `minYearDb` | ❌ ไม่ | ลบ |
| `SELECT MAX(Year(TranDate)) FROM Transactions` | `maxYearDb` | ❌ ไม่ | ลบ |
| `SELECT COUNT(*) ... WHERE TranDate BETWEEN ...` (raw) | `rawBetweenCount` | ❌ ไม่ | ลบ |
| `SELECT COUNT(*) ... WHERE expr BETWEEN ...` (computed) | `exprBetweenCount` | ❌ ไม่ | ลบ |

**หมายเหตุ:** `LoadFilters()` มี query `SELECT MIN(...)/MAX(...)` ของตัวเองเพื่อกำหนดค่า `dtpFrom`/`dtpTo` — ไม่ได้ถูกแตะและไม่เกี่ยวกับ debug harness

### 6.3 สิ่งที่ผู้ใช้ควรทดสอบด้วยมือ

1. **เปิด `FrmTransactions`** → สังเกตว่าฟอร์มเปิดเร็วขึ้นจริงไหม (ก่อนแก้จะหน่วง ~500ms ต่อจุด debug × 6 จุด ≈ 3 วินาทีรวม)
2. **ค้นหา/กรองข้อมูล** ในตารางธุรกรรม → ยืนยันว่าไม่มีอาการกระตุกเหมือนก่อน (ก่อนแก้ HTTP POST 500ms ทำงานทุกครั้งที่ `LoadData` ถูกเรียก)
3. **ทดสอบ Exception path** — ตั้งใจให้ DB error (เช่น ปิดไฟล์ .accdb แล้วรีเฟรช) → ต้อง error dialog ปกติ ไม่ freeze
4. **ยืนยันว่าหน้าจออื่นไม่กระทบ** — เปิด dashboard / FrmIncome / FrmExpense ต้องทำงานปกติ

---

## 7. P6 — TempleSetting Transaction Wrap (Data-Loss Risk Fix)

**วันที่:** 2026-08-20
**อ้างอิง:** `PROJECT_HEALTH_REPORT.md` ส่วนที่ 5.2.5 (L3) และตาราง Priority (P6)
**ขอบเขต:** `Forms\FrmTempleSetting.vb` เท่านั้น

### 7.1 ปัญหา

`btnSave_Click` ใน `FrmTempleSetting.vb` บันทึกข้อมูลวัดด้วยรูปแบบ:
```sql
DELETE FROM TempleSetting
INSERT INTO TempleSetting (...) VALUES (...)
```

ทั้งสอง statement ไม่ได้ห่อด้วย transaction เดียวกัน ถ้า DELETE สำเร็จแต่ INSERT ล้มเหลว ข้อมูลวัดทั้งหมด (ชื่อวัด, ที่อยู่, เจ้าอาวาส/ไวยาวัจกร/ผู้ทำบัญชี, PromptPay, เบอร์โทร) จะหายถาวร

### 7.2 Backup

- **Path:** `Database\TempleAccounting_pre_P6_20260820_060611.accdb`
- ต้นทาง: `Database\TempleAccounting.accdb`

### 7.3 Diff

**ไฟล์ที่แก้:** `Forms\FrmTempleSetting.vb` — `btnSave_Click` (บรรทัด ~314-357)

**ก่อนแก้:**
```vb
' บันทึกข้อมูล TempleInfo - อ้างอิง PersonnelID เท่านั้น (Modern Refactored Structure)
Db.ExecuteNonQuery(conn, "DELETE FROM TempleSetting")
Db.InsertAndGetId(conn,
    "INSERT INTO TempleSetting (TempleCode,..." &
    "VALUES (@a1,@a2,...@a13)", ...)

MessageBox.Show("✅ บันทึกข้อมูลวัดสำเร็จ!", ...)
ReportEngine.ClearTemplateInfoCache()
LoadPersonnelGrid()
txtTempleCode.Focus()
```

**หลังแก้:**
```vb
' บันทึกข้อมูล TempleInfo - อ้างอิง PersonnelID เท่านั้น (Modern Refactored Structure)
' ใช้ transaction เพื่อป้องกันข้อมูลหายถ้า DELETE สำเร็จแต่ INSERT ล้มเหลว
Using trans = conn.BeginTransaction()
    Try
        Dim oledbConn = TryCast(conn, OleDbConnection)
        Using cmd = oledbConn.CreateCommand()
            cmd.Transaction = TryCast(trans, OleDbTransaction)
            cmd.CommandText = "DELETE FROM TempleSetting"
            cmd.ExecuteNonQuery()
        End Using

        Db.InsertAndGetId(conn,
            "INSERT INTO TempleSetting (...) VALUES (...)", ...)

        trans.Commit()
    Catch ex As Exception
        trans.Rollback()
        MessageBox.Show("บันทึกข้อมูลวัดไม่สำเร็จ: " & ex.Message,
            "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return
    End Try
End Using

MessageBox.Show("✅ บันทึกข้อมูลวัดสำเร็จ!", ...)
ReportEngine.ClearTemplateInfoCache()
LoadPersonnelGrid()
txtTempleCode.Focus()
```

### 7.4 Transaction Pattern Reference

ใช้รูปแบบเดียวกับ `FrmTransactions.vb` บรรทัด ~456-476 (`btnDelete_Click`) ซึ่งมี:
- `Using trans = conn.BeginTransaction()`
- `Dim oledbConn = TryCast(conn, OleDbConnection)`
- `cmd.Transaction = TryCast(trans, OleDbTransaction)`
- `trans.Commit()` / `trans.Rollback()`

**ไม่ต้องแก้ไฟล์อื่น** — `Db.ExecuteNonQuery()`, `Db.InsertAndGetId()`, และ `Imports System.Data.OleDb` มีอยู่แล้วครบ

### 7.5 ผล Build

```
dotnet build -c Release
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 7.6 สิ่งที่ผู้ใช้ควรทดสอบด้วยมือ

1. **กรณีปกติ:** เปิด `FrmTempleSetting` → แก้ข้อมูลวัด → กดบันทึก → ปิดโปรแกรมแล้วเปิดใหม่ → ข้อมูลต้องอยู่ครบ
2. **กรณีจำลอง failure (สำคัญที่สุด):** ลองใส่ค่าที่ยาวเกิน field length (เช่น ใส่ชื่อวัดยาวๆ เกิน 200 ตัวอักษร) แล้วกดบันทึก → ต้องแสดง error "บันทึกข้อมูลวัดไม่สำเร็จ" และข้อมูลเดิม **ต้องยังอยู่ครบ** (เพราะ Rollback ทำงาน) → ปิดแล้วเปิดใหม่ ยืนยันว่าข้อมูลเดิมไม่หาย
3. **Regression:** ฟีเจอร์อื่นใน `FrmTempleSetting` ต้องยังทำงานปกติ:
   - กดปุ่ม "จัดการบุคลากร" → เปิด `FrmPersonnelManagement` ได้
   - กดปุ่ม "นำเข้าข้อมูลที่อยู่" → เปิด `FrmLocationImport` ได้
   - เลือกจังหวัด → อำเภอ → ตำบล dropdown ทำงานปกติ
   - กดปุ่ม "ยกเลิก" → โหลดข้อมูลเดิมกลับมาแสดง

---

## 8. Tiered Backup Retention (Weekly / Monthly / Yearly)

**วันที่:** 2026-08-20
**อ้างอิง:** `PROJECT_HEALTH_REPORT.md` (E15 / P8)
**ขอบเขต:** `DatabaseBackupHelper.vb` + `frmMain.vb` (`FrmMain_FormClosing`)
**ผล build:** `dotnet build -c Release` → **Build succeeded. 0 Warning(s) / 0 Error(s)**

### 8.1 ปัญหาเดิม

ระบบ backup อัตโนมัติเดิมทำงานตอนปิดโปรแกรม โดยใช้ `BackupFullSystem(isAuto:=True)` ซึ่งสร้างโฟลเดอร์ `Backup_{yyyyMMdd_HHmmss}` สะสมไม่จำกัด — ไม่มี retention policy

### 8.2 สิ่งที่แก้

**แทนที่ auto-backup ด้วยระบบ 3 Tier: Weekly / Monthly / Yearly**

- **ไม่แตะ** manual backup (`btnBackup_Click`) และ safety backup ก่อน import/merge (`Db.BackupDatabase()` / `BackupFullSystem()` เดิม)
- **ไม่ลบ** backup แบบเก่า (`Backup_{yyyyMMdd_HHmmss}`) ที่มีอยู่แล้ว
- **เก็บเฉพาะไฟล์ `.accdb`** ไม่รวมโฟลเดอร์ Receipts (ประหยัดพื้นที่เก็บระยะยาว)

### 8.3 ฟังก์ชันที่เพิ่มใน `DatabaseBackupHelper.vb`

| ฟังก์ชัน | หน้าที่ |
|---|---|
| `RunTieredBackup(Optional overrideDate)` | จัดการเรียก Weekly → Monthly → Yearly (เดือน 12 เท่านั้น) |
| `RunWeeklyBackup(d)` | ชื่อไฟล์ `Backup_Weekly_{ISOYear}_W{weekNum:D2}.accdb` — overwrite-in-place ทุกครั้ง |
| `RunMonthlyBackup(d)` | ชื่อไฟล์ `Backup_Monthly_{yyyy}_{MM}.accdb` — overwrite-in-place ทุกครั้ง |
| `RunYearlyBackup(d)` | ชื่อไฟล์ `Backup_Yearly_{yyyy}.accdb` — สร้างเฉพาะเดือนธันวาคม |
| `EnsureBackupTierFolders()` | สร้างโฟลเดอร์ `Weekly/` `Monthly/` `Yearly/` ใต้ `Backup/` |
| `CleanupWeeklyBackups()` | FIFO ไม่เกิน 8 ไฟล์ |
| `CleanupMonthlyBackups()` | FIFO ไม่เกิน 15 ไฟล์ |
| `CleanupYearlyBackups()` | FIFO ไม่เกิน 5 ไฟล์ |
| `CleanupTierFolder(...)` | Helper กลาง — list ไฟล์ตาม pattern เรียงจากชื่อไฟล์ (ไม่ใช่ `CreationTime`) ลบไฟล์เก่าสุดออกทีละไฟล์จนเหลือ max |
| `CopyDatabaseToTierFolder(...)` | Helper — `File.Copy` จาก `AppPaths.DatabaseFile` ไป tier folder (overwrite) |

### 8.4 เปลี่ยนใน `frmMain.vb`

```vb
' ก่อน
DatabaseBackupHelper.BackupFullSystem(isAuto:=True)

' หลัง
DatabaseBackupHelper.RunTieredBackup()
```

**BackupFullSystem() ยังอยู่ครบ** — ไม่ได้ลบ ยังใช้กับ manual backup และ safety backup ก่อน import

### 8.5 Bug ที่พบระหว่างทดสอบ — Buddhist Era Calendar

**ปัญหา:** บนระบบ Windows ที่ locale เป็นภาษาไทย (`th-TH`) ตัว format specifier `{d:yyyy}` จะคืนค่าปี พ.ศ. (Buddhist Era) เช่น `2567` แทนที่จะเป็น `2024` ทำให้ชื่อไฟล์ backup ผิดรูปแบบ เช่น `Backup_Monthly_2567_01.accdb` แทนที่จะเป็น `Backup_Monthly_2024_01.accdb`

**วิธีแก้:** เปลี่ยนจาก `{d:yyyy}` / `{d:MM}` เป็น `{d.Year}` / `{d.Month:D2}` (คืนค่า Gregorian year เสมอ ไม่ขึ้นกับ locale)

### 8.6 ผลการทดสอบทั้ง 5 กรณี (สร้าง test console project ชั่วคราว แล้วลบออกแล้ว)

**Test 1 — Monthly retention 20 เดือน:**
- เรียก backup ด้วยวันที่ปลอม 20 เดือนติดกัน (Jan 2024 → Aug 2025)
- ผล: `Monthly/` เหลือ **15 ไฟล์** (Jun 2024 → Aug 2025) ✅
- ไฟล์ที่ถูกลบ: Jan–May 2024 (5 ไฟล์เก่าสุด)

**Test 2 — Yearly retention 7 ปี:**
- เรียกด้วยวันที่ธันวาคม 7 ปีติดกัน (2020–2026)
- ผล: `Yearly/` เหลือ **5 ไฟล์** (2022–2026) ✅
- ไฟล์ที่ถูกลบ: 2020, 2021

**Test 3 — Weekly retention 10 สัปดาห์:**
- เรียกด้วยวันที่ข้าม 10 สัปดาห์ติดกัน (Mon March 2 → May 4, 2026)
- ผล: `Weekly/` เหลือ **8 ไฟล์** (W12–W19 ของปี 2026) ✅
- ไฟล์ที่ถูกลบ: W10, W11

**Test 4 — Idempotent overwrite (วันที่เดียวกัน 3 ครั้ง):**
- เรียกด้วยวันที่ 12 ส.ค. 2026 จำนวน 3 ครั้งติดกัน
- ผล: `Weekly/` มี **1 ไฟล์** (`Backup_Weekly_2026_W33.accdb`), `Monthly/` มี **1 ไฟล์** (`Backup_Monthly_2026_08.accdb`) ✅
- ไม่มีไฟล์ซ้ำ — ถูก overwrite ไม่ใช่สร้างใหม่

**Test 5 — Year boundary edge case (ISO week คาบเกี่ยว 2 ปี):**
- Dec 28, 2026 → ISO year=2026, week=53 → `Backup_Weekly_2026_W53.accdb`
- Jan 3, 2027 → ISO year=2026, week=53 → `Backup_Weekly_2026_W53.accdb` (**ไฟล์เดียวกัน** ✅ — อยู่ใน ISO week เดียวกัน)
- Jan 4, 2027 → ISO year=2027, week=1 → `Backup_Weekly_2027_W01.accdb` (**ไฟล์ใหม่** ✅ — สัปดาห์เปลี่ยนแล้ว)
- Dec 28 → สร้าง yearly backup (เดือน 12) ✅
- Jan 4 → ไม่สร้าง yearly backup (เดือน 1) ✅

### 8.7 โครงสร้างโฟลเดอร์หลังใช้งานจริง

```
Backup/
├── Backup_20260820_090000/          ← backup แบบเก่า (ยังอยู่ ไม่ถูกลบ)
│   ├── TempleDB_20260820_090000.accdb
│   └── Receipts/
├── TempleAccounting_before_B8A6A1_fix_20260819.accdb  ← safety backup ก่อนแก้บั๊ก
├── TempleAccounting_before_income_sample_20260726.accdb
├── Weekly/
│   ├── Backup_Weekly_2026_W33.accdb
│   ├── Backup_Weekly_2026_W34.accdb
│   └── ... (สูงสุด 8 ไฟล์)
├── Monthly/
│   ├── Backup_Monthly_2026_07.accdb
│   ├── Backup_Monthly_2026_08.accdb
│   └── ... (สูงสุด 15 ไฟล์)
└── Yearly/
    └── Backup_Yearly_2026.accdb     ← มีเฉพาะถ้าปิดโปรแกรมในเดือนธันวาคม
```

### 8.8 Known Limitation

ถ้าผู้ใช้ไม่เปิดโปรแกรมเลยตลอดเดือนธันวาคมของปีใด ปีนั้นจะไม่มี yearly backup — เพราะ yearly backup ถูก trigger จากการปิดโปรแกรมเท่านั้น (ไม่มี scheduled job ใน WinForms desktop app)

### 8.9 ไฟล์ที่แก้จริง

| ไฟล์ | การแก้ |
|---|---|
| `DatabaseBackupHelper.vb` | เพิ่ม Imports `System.Globalization`; เพิ่มฟังก์ชัน tiered backup ทั้งหมด |
| `frmMain.vb` | `FrmMain_FormClosing` — เปลี่ยนจาก `BackupFullSystem(isAuto:=True)` เป็น `RunTieredBackup()` |

### 8.10 สิ่งที่ผู้ใช้ควรทดสอบด้วยมือ

1. **เปิดโปรแกรม → ปิดโปรแกรม** → ตรวจว่า `Backup/Weekly/` และ `Backup/Monthly/` มีไฟล์ `.accdb` ใหม่เกิดขึ้น 1 ไฟล์ในแต่ละโฟลเดอร์ (ชื่อไฟล์ต้องเป็น ค.ศ. ไม่ใช่ พ.ศ.)
2. **เปิด-ปิดโปรแกรมซ้ำ 3 รอบในวันเดียวกัน** → ยืนยันว่าไม่มีไฟล์ซ้ำ ( overwrite ไฟล์เดิม)
3. **ตรวจสอบโฟลเดอร์ `Backup/` หลัก** → ต้องยังมี backup แบบเก่า (`Backup_{yyyyMMdd_HHmmss}`) อยู่ครบ ไม่ถูกลบ
4. **ทดสอบปุ่ม Manual Backup** (`btnBackup_Click` → เลือกโฟลเดอร์) → ต้องยังทำงานเหมือนเดิม (สร้าง `Backup_{timestamp}` + Receipts)

---

## 9. Append-Only Collision-Safe Receipt Backup

**วันที่:** 2026-08-20
**อ้างอิง:** `FIX_REPORT.md` หัวข้อ 8 (Tiered Backup Retention) — ต่อยอด
**ขอบเขต:** `DatabaseBackupHelper.vb` เท่านั้น
**ผล build:** `dotnet build -c Release` → **Build succeeded. 0 Warning(s) / 0 Error(s)**

### 9.1 ปัญหาเดิม

1. **Dual-copy receipts:** `BackupFullSystem()` copy โฟลเดอร์ `Receipts/` เข้าไปใน `Backup_{timestamp}/Receipts/` ทุกครั้งที่ manual backup — ทำให้รูปใบเสร็จซ้ำซ้อนกระจายอยู่หลายที่
2. **ไม่มี centralized receipt backup:** Tiered backup (Weekly/Monthly/Yearly) ไม่ได้ backup รูปใบเสร็จเลย — มีเฉพาะไฟล์ `.accdb` เท่านั้น
3. **ไม่มี collision protection:** ถ้า filename ซ้ำกัน ระบบจะ overwrite ไฟล์เดิมเงียบๆ

### 9.2 ผลตรวจสอบความ unique ของชื่อไฟล์จริง

**วิธีตั้งชื่อไฟล์ใบเสร็จจริง:**
- `ReceiptImageHelper.SaveOptimizedReceipt()` ตั้งชื่อ `Receipt_{transactionID}.jpg` โดย `transactionID` เป็น auto-increment integer จาก Access DB
- ไม่มี GUID/timestamp ฝังอยู่ในชื่อไฟล์ — ชื่อ deterministic ตาม transaction ID

**ความเสี่ยง collision:** ต่ำมาก (auto-increment ไม่ซ้ำกันใน theory) แต่ logic collision-safe ยังจำเป็นเป็น safety net เพราะ:
- ผู้ใช้อาจ rename ไฟล์ด้วยมือ
- ไฟล์จากเครื่องอื่นที่ import เข้ามาอาจมีชื่อซ้ำ
- ป้องกันกรณี edge case ที่ไม่คาดคิด

### 9.3 Diff: `SyncReceiptsToBackup()` (ฟังก์ชันใหม่)

```vb
' ──────────────────────────────────────────────────────────────
' Centralized Receipt Backup (Append-Only, Collision-Safe)
' ──────────────────────────────────────────────────────────────

Public Shared Sub SyncReceiptsToBackup()
    ' คัดลอกใบเสร็จจาก Receipts/ → Backup/Receipts/
    ' • Hash ตรงกัน → skip (ไม่ copy ซ้ำ)
    ' • ชื่อเดียวกันแต่เนื้อหาต่างกัน → เก็บด้วยชื่อใหม่แบบ deterministic
    ' • ห้ามลบไฟล์ใดๆ (Append-Only)
    
    For Each srcFile In Directory.GetFiles(sourceDir)
        If File.Exists(destFilePath) Then
            Dim srcHash = ComputeMd5Hash(File.ReadAllBytes(srcFile))
            Dim destHash = ComputeMd5Hash(File.ReadAllBytes(destFilePath))
            If srcHash = destHash Then Continue For  ' skip
            ' Collision: ตั้งชื่อด้วย hash สั้น 8 ตัว (deterministic)
            Dim collisionName = $"{nameNoExt}_{GetShortMd5Hash(srcBytes)}{ext}"
            File.WriteAllBytes(finalPath, srcBytes)
        Else
            File.Copy(srcFile, destFilePath)
        End If
    Next
End Sub

Private Shared Function ComputeMd5Hash(data As Byte()) As String
    Using md5 As MD5 = MD5.Create()
        Return BitConverter.ToString(md5.ComputeHash(data)).Replace("-", "").ToLowerInvariant()
    End Using
End Function

Private Shared Function GetShortMd5Hash(data As Byte()) As String
    Return ComputeMd5Hash(data).Substring(0, 8)
End Function
```

**กฎที่บังคับใช้:**
- ❌ ไม่ใช้ Modification Time เป็นเงื่อนไขเปรียบเทียบ
- ❌ ไม่ใช้ timestamp (เวลาที่รัน) ตั้งชื่อไฟล์ collision
- ✅ ใช้ MD5 hash ของเนื้อหาไฟล์ → deterministic: ไฟล์เดียวกันคิดกี่ครั้งก็ hash เดิม → ไม่เกิด infinite duplication

### 9.4 Diff: ลบ Legacy Dual-Copy

**`BackupFullSystem()` — ลบ step 3 เดิม:**

```vb
' ก่อน (ลบ)
Dim receiptsSource As String = AppPaths.ReceiptsDir
If Directory.Exists(receiptsSource) Then
    Dim receiptsDest As String = Path.Combine(backupPath, "Receipts")
    CopyDirectory(receiptsSource, receiptsDest)
End If

' หลัง (แทนที่)
SyncReceiptsToBackup()
```

**`RunTieredBackup()` — เพิ่ม sync หลัง copy DB ทุก tier:**

```vb
Public Shared Sub RunTieredBackup(...)
    ...
    RunWeeklyBackup(d)
    RunMonthlyBackup(d)
    If d.Month = 12 Then RunYearlyBackup(d)

    ' 4. คัดลอกใบเสร็จไปโฟลเดอร์กลาง (หลัง copy DB ทุก tier เสร็จแล้ว)
    SyncReceiptsToBackup()  ' ← เพิ่มใหม่
End Sub
```

### 9.5 ผลทดสอบทั้ง 6 กรณี (temporary test project)

สร้าง test console project ชั่วคราวที่ replicates SyncReceiptsToBackup logic แล้วรัน 6 กรณี ลบออกหลังทดสอบเสร็จ

| # | กรณี | ผล |
|---|---|---|
| 1 | **Sync พื้นฐาน:** สร้างไฟล์ปลอม 5 ไฟล์ → sync → ยืนยัน copy ครบ | ✅ 5 ไฟล์ถูก copy ไป `Backup/Receipts/` |
| 2 | **Skip ไฟล์เหมือนกัน:** เรียกซ้ำโดยไม่เปลี่ยนอะไร | ✅ ไม่มีการ copy ซ้ำ (5 → 5 ไฟล์不变) |
| 3 | **ไฟล์ใหม่เพิ่มมาทีหลัง:** เพิ่มไฟล์ที่ 6 → sync อีกครั้ง | ✅ มีแค่ไฟล์ที่ 6 ถูก copy เพิ่ม (5 → 6) |
| 4 | **Collision case:** สร้างไฟล์ชื่อเดียวกันแต่ size ต่างกัน → sync | ✅ ไฟล์ทั้งคู่ถูกเก็บ (2 ไฟล์ — ต้นฉบับไม่ถูกทับ, ไฟล์ใหม่ได้ชื่อ hash suffix) |
| 5 | **Collision re-sync stability (สำคัญที่สุด):** จากสถานการณ์ข้อ 4 เรียก sync ซ้ำอีก 3 รอบ | ✅ **รอบ 1: 2 ไฟล์ / รอบ 2: 2 ไฟล์ / รอบ 3: 2 ไฟล์ / รอบ 4: 2 ไฟล์** — ไม่มี infinite duplication |
| 6 | **BackupFullSystem ไม่ copy receipts ซ้ำซ้อน:** จำลอง tier folder → sync → ยืนยันไม่มี Receipts/ ภายใน tier | ✅ โฟลเดอร์ `Weekly/` ไม่มี Receipts/ ของตัวเอง, `Backup/Receipts/` มีไฟล์ล่าสุดครบ |

**กรณีที่ 5 พิสูจน์ว่า:** การตั้งชื่อ collision แบบ deterministic (MD5 hash 8 ตัวแรก) ทำงานถูกต้อง — ไฟล์เดิมที่มีอยู่แล้วถูก detect ว่า hash ตรงกัน → skip ไม่ copy ซ้ำ ต่างจากการใช้ timestamp ซึ่งจะสร้างชื่อใหม่ทุกรอบและเกิด infinite duplication

### 9.6 โครงสร้างโฟลเดอร์หลังแก้

```
Backup/
├── Backup_20260820_090000/          ← manual backup (ไม่มี Receipts/ ของตัวเองอีกแล้ว)
│   └── TempleDB_20260820_090000.accdb
├── TempleAccounting_before_B8A6A1_fix_20260819.accdb
├── Weekly/
│   └── Backup_Weekly_2026_W33.accdb    ← มีเฉพาะ .accdb
├── Monthly/
│   └── Backup_Monthly_2026_08.accdb    ← มีเฉพาะ .accdb
├── Yearly/
│   └── Backup_Yearly_2026.accdb         ← มีเฉพาะ .accdb
└── Receipts/                            ← โฟลเดอร์กลางเดียว ใช้ร่วมกัน
    ├── Receipt_1.jpg
    ├── Receipt_2.jpg
    ├── Receipt_3.jpg
    └── Receipt_99_672580db.jpg          ← collision file (hash suffix)
```

### 9.7 ประเมินพื้นที่ดิสก์ที่ใช้จริงโดยประมาณ

- **เดิม (dual-copy):** ถ้ามีรูปใบเสร็จ 500 ไฟล์ เฉลี่ย 200KB/ไฟล์ → ~100MB × N copies (ทุก manual backup)
- **ใหม่ (centralized):** 500 ไฟล์ × 200KB = ~100MB ครั้งเดียว (ไม่ซ้ำซ้อน)
- **ประหยัด:** ~90-95% พื้นที่เก่าที่เสียไปกับการ copy ซ้ำซ้อน
- **Collision files:** หายากมาก (auto-increment ID ไม่ซ้ำกัน) → พื้นที่เพิ่มเติมแทบไม่มี

### 9.8 ไฟล์ที่แก้จริง

| ไฟล์ | การแก้ |
|---|---|
| `DatabaseBackupHelper.vb` | เพิ่ม `Imports System.Security.Cryptography`; เพิ่ม `SyncReceiptsToBackup()`, `ComputeMd5Hash()`, `GetShortMd5Hash()`; ลบ `CopyDirectory(receipts)` จาก step 3 ของ `BackupFullSystem()`; เพิ่ม `SyncReceiptsToBackup()` ท้าย `RunTieredBackup()` |

### 9.9 สิ่งที่ผู้ใช้ควรทดสอบด้วยมือ

1. **Manual Backup:** กดปุ่ม backup → ตรวจว่า `Backup_{timestamp}/` สร้างขึ้นใหม่ **ไม่มี** โฟลเดอร์ `Receipts/` ข้างในอีกต่อไป แต่ `Backup/Receipts/` มีรูปใบเสร็จครบ
2. **Auto-backup (ปิดโปรแกรม):** เปิด-ปิดโปรแกรม 3 รอบ → ตรวจว่า `Backup/Receipts/` มีรูปใบเสร็จครบ ไม่มีไฟล์ซ้ำ
3. **Append-Only:** ลบธุรกรรมที่มีรูปใบเสร็จออกจากระบบ (FrmTransactions) → ปิดแล้วเปิดใหม่ → ตรวจว่ารูปใน `Backup/Receipts/` **ยังอยู่ครบ** (ไม่ถูกลบ)4. **Restore ยังทำงาน:** กด restore backup เก่า → ต้อง restore ได้ปกติ ( RestoreFullSystem ไม่ได้ถูกแก้ไข)

---

## 10. Restore Compatibility Fix

**วันที่:** 2026-08-20
**อ้างอิง:** `FIX_BRIEF_RESTORE_COMPATIBILITY_1.md` (หัวข้อ 9 → ต่อยอด)
**ขอบเขต:** `DatabaseBackupHelper.vb` — `RestoreFullSystem()` เท่านั้น
**ผล build:** `dotnet build -c Release` → **Build succeeded. 0 Warning(s) / 0 Error(s)**

### 10.1 ปัญหาที่พบก่อนแก้ (Pre-Fix Inspection)

**วิธี `RestoreFullSystem()` จัดการรูปใบเสร็จก่อนแก้:**

```vb
' กรณี 1: มีโฟลเดอร์ Receipts อยู่ข้างใน (โครงสร้างมาตรฐาน)
Dim subDirReceipts = Path.Combine(backupFolder, "Receipts")
If Directory.Exists(subDirReceipts) Then receiptsSourcePaths.Add(subDirReceipts)

' กรณี 2: ผู้ใช้เลือกโฟลเดอร์ที่มีรูปอยู่โดยตรง
receiptsSourcePaths.Add(backupFolder)

For Each srcPath In receiptsSourcePaths
    CopyImageFilesOnly(srcPath, AppPaths.ReceiptsDir)
Next
```

**ผลวิเคราะห์:**

| สถานการณ์ | ผลก่อนแก้ | ผลกระทบ |
|---|---|---|
| **Backup แบบเก่า** (มี `Receipts/` ภายใน) | ✅ ทำงานปกติ — คัดลอกจาก `{backupFolder}/Receipts/` | ไม่มีปัญหา |
| **Backup แบบใหม่** (หลัง Fix 9 — ไม่มี `Receipts/` ภายใน) | ❌ **รูปใบเสร็จหายเงียบๆ** — เฉพาะ `.accdb` ถูก restore, รูปไม่ถูกคัดลอกเลย | ผู้ใช้ restore สำเร็จแต่ใบเสร็จรูปหายทั้งหมด |
| **Backup แบบใหม่ + centralized `Backup/Receipts/` มีรูป** | ❌ Same — ไม่มี fallback ไปที่โฟลเดอร์กลาง | ข้อมูลรูปสูญหาย |

**สรุป:** FIX_REPORT.md หัวข้อ 9.9 อ้างว่า "Restore ยังทำงาน" — จริงเฉพาะ `.accdb` restore แต่ **รูปใบเสร็จไม่ได้ถูก restore** จาก backup ชุดใหม่

### 10.2 Diff

**ไฟล์ที่แก้:** `DatabaseBackupHelper.vb` — `RestoreFullSystem()` (ส่วน Restore Receipt Images)

**ก่อนแก้:**
```vb
' กรณี 1: มีโฟลเดอร์ Receipts อยู่ข้างใน (โครงสร้างมาตรฐาน)
Dim subDirReceipts = Path.Combine(backupFolder, "Receipts")
If Directory.Exists(subDirReceipts) Then receiptsSourcePaths.Add(subDirReceipts)

' กรณี 2: ผู้ใช้เลือกโฟลเดอร์ที่มีรูปอยู่โดยตรง
receiptsSourcePaths.Add(backupFolder)

For Each srcPath In receiptsSourcePaths
    CopyImageFilesOnly(srcPath, AppPaths.ReceiptsDir)
Next
```

**หลังแก้:**
```vb
' กรณี 1: มีโฟลเดอร์ Receipts อยู่ข้างใน (โครงสร้างมาตรฐาน — legacy backup)
Dim subDirReceipts = Path.Combine(backupFolder, "Receipts")
If Directory.Exists(subDirReceipts) Then receiptsSourcePaths.Add(subDirReceipts)

' กรณี 2: ผู้ใช้เลือกโฟลเดอร์ที่มีรูปอยู่โดยตรง
receiptsSourcePaths.Add(backupFolder)

' กรณี 3: Backup ใหม่ (หลัง Fix 9) ไม่มี Receipts/ ข้างใน
'         → fallback ไปใช้โฟลเดอร์กลาง Backup/Receipts/ แทน
If receiptsSourcePaths.Count = 1 Then
    Dim centralizedReceipts = Path.Combine(AppPaths.BackupFolder, "Receipts")
    If centralizedReceipts <> subDirReceipts AndAlso Directory.Exists(centralizedReceipts) Then
        receiptsSourcePaths.Add(centralizedReceipts)
    End If
End If

For Each srcPath In receiptsSourcePaths
    CopyImageFilesOnly(srcPath, AppPaths.ReceiptsDir)
Next
```

**หลักการแก้:**
- `receiptsSourcePaths.Count = 1` หมายความว่า Case 1 ไม่เจอ (ไม่มี internal Receipts/) → เป็น backup แบบใหม่
- `centralizedReceipts <> subDirReceipts` guard ป้องกัน edge case ที่ผู้ใช้เลือกโฟลเดอร์ `Backup/` โดยตรง (subDirReceipts = `Backup/Receipts`, centralizedReceipts = `Backup/Receipts` → ซ้ำกัน → skip)
- ไม่กระทบ legacy backup (internal Receipts/ มีอยู่ → Count = 2 → Case 3 ไม่ทำงาน)

### 10.3 Edge Case: Tier Folder Restore

**คำถาม:** `RestoreFullSystem()` รองรับการ restore จากไฟล์ `.accdb` เดี่ยวใน tier folders (Weekly/, Monthly/, Yearly/) หรือไม่?

**ผลวิเคราะห์:**

| สถานการณ์ | ผล |
|---|---|
| ผู้ใช้เลือก `Backup/Weekly/Backup_Weekly_2026_W33.accdb` โดยตรง | ✅ ทำงานได้ — ไฟล์ถูกคัดลอกเป็น `.accdb` file, DB restore สำเร็จ |
| ผู้ใช้เลือกโฟลเดอร์ `Backup/Weekly/` (folder) | ✅ ทำงานได้ — ค้นหา `*.accdb` แล้วเอา `files(0)` (ไฟล์แรก) |
| ผู้ใช้เลือกโฟลเดอร์ `Backup/` (parent) | ⚠️ ทำงานได้แต่ไม่ถูกต้อง — ค้นหา `*.accdb` ทั้งหมดใน Backup/ แล้วเอา `files(0)` ซึ่งอาจเป็นไฟล์ผิด tier |
| **รูปใบเสร็จจาก tier backup** | ❌ ไม่ได้ restore — tier backup มีเฉพาะ `.accdb` ไม่มี Receipts/, fallback ไป centralized ได้ถ้ามีรูป |

**สรุป:** `RestoreFullSystem()` ออกแบบมาให้ restore จากโฟลเดอร์ `Backup_{timestamp}/` เท่านั้น (ไม่ใช่ tier folder) แต่โค้ดเดิมรองรับการเลือก `.accdb` เดี่ยวและโฟลเดอร์ tier ได้บางส่วน ไม่ต้องแก้ไขในรอบนี้ — ให้รายงานไว้เป็น known limitation รอคำสั่งแยกต่างหาก

### 10.4 ผลทดสอบ (Temporary Test Project)

สร้าง console test project ชั่วคราวที่ replicates ตรรกะ receipts restoration ของ `RestoreFullSystem()` (post-fix) แล้วรัน 3 กรณี:

| Test | สถานการณ์ | ผล |
|---|---|---|
| **Test 1** | Legacy backup: มี `Receipts/` ภายใน + centralized `Backup/Receipts/` มีรูปด้วย | ✅ **PASS** — คัดลอกเฉพาะจาก internal `Receipts/` (2 ไฟล์), centralized fallback ไม่ทำงาน |
| **Test 2** | New backup: ไม่มี `Receipts/` ภายใน + centralized `Backup/Receipts/` มีรูป | ✅ **PASS** — fallback ไป centralized สำเร็จ, คัดลอก 2 ไฟล์จาก `Backup/Receipts/` |
| **Test 3a** | New backup + centralized dir ว่างเปล่า (ไม่มีรูป) | ✅ **PASS** — ไม่ throw exception, 0 ไฟล์ถูกคัดลอก, ไม่ blocking |
| **Test 3b** | New backup + centralized dir ไม่มีอยู่เลย | ✅ **PASS** — ไม่ throw exception, 0 ไฟล์, graceful |

**ผลรวม: 11/11 assertions passed, 0 failed**

### 10.5 ไฟล์ที่แก้จริง

| ไฟล์ | การแก้ |
|---|---| |
| `DatabaseBackupHelper.vb` | เพิ่ม Case 3 fallback ใน `RestoreFullSystem()` — ตรวจสอบ centralized `Backup/Receipts/` เมื่อไม่มี internal `Receipts/` |

### 10.6 สิ่งที่ผู้ใช้ควรทดสอบด้วยมือ

1. **Restore legacy backup (แบบเก่า):** เลือก `Backup_{timestamp}/` ที่มี `Receipts/` ข้างใน → restore → ยืนยันว่ารูปใบเสร็จกลับมาครบ
2. **Restore new backup (แบบใหม่):** เลือก `Backup_{timestamp}/` ที่ไม่มี `Receipts/` ข้างใน → restore → ยืนยันว่ารูปถูกดึงมาจาก `Backup/Receipts/`
3. **Restore เมื่อไม่มีรูปเลย:** ลบไฟล์ใน `Backup/Receipts/` → restore → ยืนยันว่า DB ยัง restore ได้ปกติ (ไม่มีรูปก็ไม่เป็นไร)
4. **Regression:** ทดสอบว่า manual backup ยังสร้าง `Backup_{timestamp}/` ได้ปกติ (ไม่กระทบ backup side)

---

## 11. Empty Catch Blocks — Triage + Tier 1 Fix

**วันที่:** 2026-08-20
**อ้างอิง:** `PROJECT_HEALTH_REPORT.md` ส่วนที่ 5.2.1 (P3) — Empty Catch Blocks
**ขอบเขต:** หลายไฟล์ — Triage ทั้งหมด + แก้เฉพาะ Tier 1 (เสี่ยงสูง)
**ผล build:** `dotnet build -c Release` → **Build succeeded. 0 Warning(s) / 0 Error(s)**

### 11.1 จำนวนจริงที่นับได้ (เทียบกับ 63+ เดิม)

| หมวด | จำนวน |
|---|---|
| Empty Catch blocks (multiline, ไม่มี code เลย) | **52** |
| Inline empty catches (`Catch : End Try` บนบรรทัดเดียว) | **3** |
| **รวมทั้งหมด** | **55** |

**เทียบกับ 63+ เดิม:** ตัวเลขจริง **55 จุด** (ต่ำกว่าที่ audit คำนวณไว้ 8 จุด — audit เดิมใช้ regex `Catch\s*$` ซึ่งจับได้มากกว่าความจริง เพราะ regex นับรวม catch ที่มี handling อยู่แล้ว เช่น `Catch ex As Exception \n AppPaths.LogCrash(...)` ซึ่งมีหลายจุดที่ catch แล้ว log อยู่แล้ว ไม่ใช่ empty)

---

### 11.2 ตาราง Triage เต็ม

#### Tier 1 — เสี่ยงสูง (แก้ในรอบนี้) — 12 จุด

| # | ไฟล์ | บรรทัด | บริบท | ประเภท |
|---|---|---|---|---|
| 1 | `AppPreferences.vb` | 86 | `EnsureLoaded()` — อ่าน config file | log-only |
| 2 | `AppPreferences.vb` | 103 | `Save()` — เขียน config file | log-only |
| 3 | `frmMain.vb` | 206 | `ShowDashboard()` — โหลด dashboard จาก DB (มี fallback) | log-only |
| 4 | `frmMain.vb` | 226 | `LoadActualDonationOverviewFromDb()` (มี fallback) | log-only |
| 5 | `frmMain.vb` | 235 | `LoadActualExpenseOverviewFromDb()` (มี fallback) | log-only |
| 6 | `frmMain.vb` | 244 | `LoadActualReportOverviewFromDb()` (มี fallback) | log-only |
| 7 | `frmMain.vb` | 253 | `LoadTransactionOverview()` — ไม่มี fallback | log-only |
| 8 | `frmMain.vb` | 275 | `LoadMonkSample()` — ไม่มี fallback | log-only |
| 9 | `frmMain.vb` | 283 | `LoadActivitySample()` — ไม่มี fallback | log-only |
| 10 | `frmMain.vb` | 291 | `LoadSettingSample()` — ไม่มี fallback | log-only |
| 11 | `frmMain.vb` | 919 | `FrmMain_FormClosing` — tiered backup ตอนปิดโปรแกรม | log-only |
| 12 | `DatabaseBackupHelper.vb` | 192 | `RestoreFullSystem()` — ลบ temp safety backup หลัง restore | log-only |

**เหตุผลที่ทั้งหมดเป็น log-only ไม่ใช่ log+MessageBox:**
- จุด 1-2: config persistence — ถ้า fail โปรแกรมยังทำงานได้ (ใช้ default values) แค่ preferences ไม่ถูก persist
- จุด 3-6: dashboard overview cards — มี fallback โหลด sample data อยู่แล้ว ไม่ต้อง popup
- จุด 7-10: section data load — ถ้า fail panel ว่างเปล่า แต่ popup ทุกครั้งที่ DB query fail จะรบกวนผู้ใช้มากเกินไป (บางครั้ง error เป็นชั่วคราว)
- จุด 11: backup ตอนปิด — brief ระบุชัดว่า "ไม่ต้องแสดง Error ตอนปิดโปรแกรมเพื่อไม่ให้กวนผู้ใช้"
- จุด 12: temp cleanup — ไม่ critical, แค่ temp folder ค้าง

#### Tier 2 — เสี่ยงกลาง (Phase 2) — 27 จุด

| ไฟล์ | จำนวนจุด | บริบท |
|---|---|---|
| `Forms/FrmExportForTransfer.vb` | 4 | export cleanup, date parsing, UI logging |
| `Forms/FrmMultiMachineImport.vb` | 7 | import DB query, date parsing, rollback cleanup, staging dir |
| `Forms/FrmReports.vb` | 2 | chart zoom/reset (มี comment อธิบาย) |
| `Reports/IncomeExpenseReport.vb` | 1 | report generation |
| `Services/AccessDbProvider.vb` | 2 | migration `ALTER TABLE DROP COLUMN`, `TryCreateTable` |
| `TransactionExcelExporter.vb` | 1 | temple name loading |
| `AppPaths.vb` | 7 | file/folder existence checks, migration file copy |
| `Forms/FrmExpense.vb` | 1 | ComboBox SelectedValue set |
| `Forms/FrmIncome.vb` | 1 | ComboBox SelectedValue set |

#### Tier 3 — เสี่ยงต่ำ (Phase 2) — 16 จุด

| ไฟล์ | จำนวนจุด | บริบท |
|---|---|---|
| `Program.vb` | 6 | boot logging, exception handler setup, icon loading |
| `frmMain.vb` | 1 | `FrmMain_Load` UI setup (cosmetic) |
| `frmMain.UiHelpers.vb` | 2 | button image loading |
| `Forms/FrmChartPreview.vb` | 2 | chart zoom/reset |
| `DatabaseBackupHelper.vb` | 1 | cleanup |
| `Forms/FrmLocationImport.vb` | 1 | location import |
| `Forms/FrmReceiptViewer.vb` | 1 | receipt display |
| `DesktopShortcutHelper.vb` | 1 | file existence check (uses try-catch as File.Exists alternative) |

**หมายเหตุ:** catch หลายจุดใน Tier 2-3 เป็น **intentional pattern** ที่ตั้งใจปล่อยว่าง เช่น:
- `Program.vb` boot logging → logging infrastructure ไม่ควรใช้ตัวเอง log ตัวเอง
- `AppPaths.vb` folder existence checks → ใช้ try-catch แทน `Directory.Exists` pattern
- `Forms/FrmExpense.vb` / `FrmIncome.vb` ComboBox → catch `SelectedValue` set แล้ว fallback เป็น `SelectedIndex = 0`
- จุดเหล่านี้ **ไม่จำเป็นต้องแก้** แค่ระบุไว้ใน triage ว่าเป็น intentional

---

### 11.3 Diff: `ErrorLogHelper.vb` (ไฟล์ใหม่)

```vb
' ErrorLogHelper.vb
Public Module ErrorLogHelper
    Private _errorLogPath As String = Nothing

    Public Sub LogSilentError(context As String, ex As Exception)
        Try
            If String.IsNullOrEmpty(_errorLogPath) Then
                _errorLogPath = Path.Combine(AppPaths.LogsFolder, "error.log")
            End If
            Dim line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}: {ex.GetType().Name} — {ex.Message}"
            File.AppendAllText(_errorLogPath, line & Environment.NewLine)
        Catch
            ' การ log เอง fail ก็ไม่ควรทำให้โปรแกรมพัง
        End Try
    End Sub
End Module
```

**เขียนลง:** `{LogsFolder}/error.log` (เคียงข้าง `crash.log` ที่มีอยู่แล้ว)
**ไม่ต้องแก้:** AppPaths.LogCrash() ยังใช้กับ unhandled exceptions เดิมได้ตามปกติ

### 11.4 Diff: Tier 1 Fixes

**AppPreferences.vb** — 2 จุด:
```vb
' Before (L86)
Catch
End Try
' After
Catch ex As Exception
    ErrorLogHelper.LogSilentError("AppPreferences.EnsureLoaded", ex)
End Try

' Before (L103)
Catch
End Try
' After
Catch ex As Exception
    ErrorLogHelper.LogSilentError("AppPreferences.Save", ex)
End Try
```

**frmMain.vb** — 9 จุด (รูปแบบเดียวกัน ต่างแค่ context string):
```vb
' Before (ทุกจุด)
Catch
    [LoadSampleData()]  ' (ถ้ามี fallback)
End Try
' After
Catch ex As Exception
    ErrorLogHelper.LogSilentError("frmMain.[context]", ex)
    [LoadSampleData()]  ' (ถ้ามี fallback)
End Try
```

**DatabaseBackupHelper.vb** — 1 จุด:
```vb
' Before (L192)
Catch
End Try
' After
Catch ex As Exception
    ErrorLogHelper.LogSilentError("DatabaseBackupHelper.RestoreFullSystem.TempCleanup", ex)
End Try
```

### 11.5 ไฟล์ที่แก้จริง

| ไฟล์ | การแก้ |
|---|---|
| `ErrorLogHelper.vb` | **ไฟล์ใหม่** — shared logging helper module |
| `TempleAccounting.vbproj` | เพิ่ม `<Compile Include="ErrorLogHelper.vb" />` |
| `AppPreferences.vb` | แก้ 2 จุด empty catch → `ErrorLogHelper.LogSilentError()` |
| `frmMain.vb` | แก้ 9 จุด empty catch → `ErrorLogHelper.LogSilentError()` |
| `DatabaseBackupHelper.vb` | แก้ 1 จุด empty catch → `ErrorLogHelper.LogSilentError()` |

### 11.6 จำนวนจุดที่เหลือใน Phase 2

- **Tier 2:** 27 จุด (รายงาน/export/migration)
- **Tier 3:** 16 จุด (cosmetic/UI)
- **Total Phase 2:** 43 จุด — รอคำสั่งแยก

### 11.7 สิ่งที่ผู้ใช้ควรทดสอบด้วยมือ

1. **เปิดโปรแกรมปกติ** → ทุกฟอร์มที่มีจุดถูกแก้ต้องเปิดได้ปกติเหมือนเดิม ไม่มี MessageBox popup ที่ไม่เคยมีมาก่อน (ต้อง popup เฉพาะตอน error จริงเท่านั้น)
2. **ทดสอบ log ทำงานจริง:** ลบสิทธิ์เขียนไฟล์ `user_preferences.ini` ชั่วคราว (read-only) → ปิดแล้วเปิดใหม่ → ตรวจว่า `Logs/error.log` มีบรรทัดใหม่บันทึก `AppPreferences.EnsureLoaded` ไว้จริง
3. **Regression:** ทดสอบ flow หลัก — dashboard โหลดปกติ, .Navigator ไปหน้าต่างๆ ได้ปกติ, ปิดโปรแกรมได้ปกติ
