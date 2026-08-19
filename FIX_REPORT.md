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
