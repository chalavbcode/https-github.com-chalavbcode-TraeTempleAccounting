# Audit Report — Temple Accounting System (ระบบบัญชีวัด)

**วันที่ตรวจสอบ:** 2026-08-19
**Repo:** `C:\Users\fantasy\Documents\TempleAccounting_FullProject_20260726_090422`
**อ้างอิง:** `PROJECT_PROFILE_V7.md`
**ขอบเขต:** อ่านโค้ดจริงเท่านั้น ไม่มีการแก้ไขโค้ดใด ๆ ในรอบนี้

---

## สรุปผู้บริหาร (Executive Summary)

ตรวจสอบพบประเด็นที่ต้องสนใจรวม **15 ข้อ** แบ่งเป็น **ยืนยันว่ามีจริง 11 ข้อ** / **แก้ไปแล้วหรือไม่มีในโค้ดปัจจุบัน 2 ข้อ** (A3, ส่วน BaseReport ของ A7) / **พบข้อมูลใหม่ 2 ข้อ** (B9, D14)

ความรุนแรงโดยรวมอยู่ในระดับ **สูง** โดย 4 ปัญหาเร่งด่วนที่สุดคือ:
1. **B8 — hardcoded `YEAR(TranDate) = 2026`** ใน query สรุปยอด 5 จุด จะให้ตัวเลขบัญชีผิดตั้งแต่ต้นปี 2027 (และไม่ได้ช่วยกรณีข้อมูลปี พ.ศ. จริงเลย)
2. **A6 — `LoadMonkSample()` อ่านคอลัมน์ที่ migration ลบไปแล้ว** ทำให้ dashboard การ์ด "เจ้าอาวาส/ไวยาวัจกร/ผู้ทำบัญชี" แสดง "-" ตลอด
3. **A1 — การแก้ไขแถว Transfer ในตารางใช้ไม่ได้จริง** (dead code) ผู้ใช้แก้แล้วกดบันทึกแต่ข้อมูลหายเงียบ ๆ
4. **C11+C12 — ไม่มีระบบยืนยันตัวตน/สิทธิ์ และลบแบบ hard delete ไม่มี audit trail** เสี่ยงต่อความโปร่งใสของบัญชีวัด

สิ่งที่ต้องแก้ก่อนคือ **B8 และ A6** เพราะกระทบความถูกต้องของตัวเลขบัญชีโดยตรงและแก้ได้เร็ว

---

## ตารางสรุปความเสี่ยง

| # | หัวข้อ | สถานะ | ความรุนแรง | ไฟล์ที่เกี่ยวข้อง |
|---|---|---|---|---|
| A1 | Transfer in-grid edit ใช้ไม่ได้ (dead code) | ยืนยัน | **สูง** | `Forms\FrmTransactions.vb` |
| A2 | Debug harness `127.0.0.1:7777` ไม่มี guard | ยืนยัน | ต่ำ | `Forms\FrmTransactions.vb` |
| A3 | `�` glyph เสีย 2 จุด | ไม่พบ (แก้ไปแล้ว) | — | `Forms\FrmTransactions.vb` / `.Designer.vb` |
| A4 | CSV parser ไม่รองรับ quoted field | ยืนยัน | กลาง | `Forms\FrmLocationImport.vb` |
| A5 | `fullName` title ติดกัน + Delete ตำแหน่งขึ้น error ดิบ | ยืนยัน | กลาง | `Forms\FrmPersonnelManagement.vb` |
| A6 | `LoadMonkSample()` อ่านคอลัมน์ legacy ที่ถูกลบ | ยืนยัน | **สูง** | `frmMain.vb`, `Services\AccessDbProvider.vb` |
| A7 | BaseReport + AutoScaleMode ผสมกัน | BaseReport แก้แล้ว / AutoScaleMode ยังผสม | กลาง | หลาย `.Designer.vb` |
| B8 | hardcoded `YEAR(TranDate) = 2026` | ยืนยัน | **สูง** | `FrmIncome.vb`, `FrmExpense.vb`, `frmMain.vb` |
| B9 | พ.ศ./ค.ศ. ปนกันใน DB | เขียน normalize แล้ว / อ่านยังกันซ้ำซ้อน | กลาง | `Services\AccessDbProvider.vb` |
| B10 | แนวทาง migration ปี พ.ศ.→ค.ศ. | ข้อเสนอ | — | (ยังไม่ทำ) |
| C11 | ไม่มี login/role/permission | ยืนยัน | **สูง** | ทั้งโปรเจกต์ |
| C12 | hard delete + ไม่มี audit trail | ยืนยัน | **สูง** | `FrmTransactions.vb`, `AccessDbProvider.vb` |
| D13 | Dedup GUID: แก้ไขซ้ำถูกข้ามเงียบ ๆ | ยืนยัน | กลาง | `Forms\FrmMultiMachineImport.vb` |
| D14 | Access file-based ไม่กันการเปิดพร้อมกัน | ยืนยัน (ไม่มีการจัดการ) | กลาง | `Services\AccessDbProvider.vb` |
| E15 | Backup ไม่มี retention/cleanup | ยืนยัน | กลาง | `DatabaseBackupHelper.vb` |

---

## รายละเอียดแต่ละข้อ

### A1. Transfer in-grid edit ใช้ไม่ได้ (dead code) — ยืนยัน

**สถานะจริง:**
- ฟังก์ชัน `SaveSelectedRowEdits()` อยู่ที่ [FrmTransactions.vb#L828](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTransactions.vb#L828) และมีโค้ด UPDATE ครบถ้วน (รองรับ Income/Expense/Transfer) แต่ **ไม่ถูกเรียกจากที่ไหนเลย** — grep ทั้งโปรเจกต์พบแค่บรรทัดประกาศ (L828) ไม่มี call site
- ปุ่มแก้ไขโอนเงินที่ [FrmTransactions.vb#L670-L675](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTransactions.vb#L670-L675) เข้า `Case "Transfer"` แล้วเพียงแค่ `_isEditing = True` + `ApplyEditModeToGrid()` + แสดง MessageBox "แก้ไขข้อมูลในแถวที่เลือกได้เลย แล้วกดปุ่ม 'บันทึกแก้ไข' เพื่อบันทึก"
- ปุ่ม "💾 บันทึกแก้ไข" คือปุ่ม `btnEdit` ตัวเดิม (เปลี่ยนแค่ `.Text`) เมื่อกดซ้ำจะวิ่งเข้า `btnEdit_Click` อีกครั้ง → เข้า `Case "Transfer"` อีก → แสดง MessageBox ซ้ำ **ไม่มีการบันทึกจริง**

```vb
Case "Transfer"
    ' สำหรับรายการโอน ยังใช้โหมดแก้ไขในตารางเดิม (หรือจะเพิ่ม FrmTransfer Edit ในอนาคต)
    _editingTransactionId = id
    _isEditing = True
    ApplyEditModeToGrid()
    MessageBox.Show("แก้ไขข้อมูลในแถวที่เลือกได้เลย แล้วกดปุ่ม 'บันทึกแก้ไข' เพื่อบันทึก", ...)
```

**ความเสี่ยง/ผลกระทบ:** ผู้ใช้แก้เซลล์ในตารางเสร็จแล้วกด "บันทึกแก้ไข" → ข้อมูลที่แก้**หายเงียบ ๆ** (silent data loss) เพราะไม่มี exception และ `LoadData()` จะ query ใหม่ทับค่าเดิม ไม่มีทางรู้ว่าไม่ได้บันทึก

**ข้อเสนอแนะ:** ใน `btnEdit_Click` เพิ่ม `If _isEditing Then SaveSelectedRowEdits() : Return` ตั้งแต่ต้น (ก่อน Select Case) หรือ route รายการ Transfer ไปเปิดฟอร์มแก้ไขเฉพาะทางแบบเดียวกับ Income/Expense

---

### A2. Debug harness `http://127.0.0.1:7777/event` — ยืนยัน

**สถานะจริง:**
- [FrmTransactions.vb#L24-L26](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTransactions.vb#L24-L26) ประกาศ `DebugSessionId = "transactions-grid-empty"` / `DebugRunId = "post-fix"` เป็นค่าตายตัว
- `DebugReport()` ที่ [L61-L83](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTransactions.vb#L61-L83) ส่ง JSON ไป `http://127.0.0.1:7777/event` ด้วย `HttpClient`
- ถูกเรียกตอน `FrmTransactions_Load` ทุกครั้ง (L97, L105) และ **ไม่มี `#If DEBUG` ครอบ** → รันใน Release ด้วย
- `client.PostAsync(...).GetAwaiter().GetResult()` เป็น **synchronous blocking call** แต่ตั้ง `Timeout = 500ms` → ถ้าไม่มี server ฟัง จะ block ไม่เกิน 500ms แล้ว throw ถูก `Catch` เงียบ ๆ

```vb
client.Timeout = TimeSpan.FromMilliseconds(500)
...
client.PostAsync(url, content).GetAwaiter().GetResult()
```

**ความเสี่ยง/ผลกระทบ:** (1) หน่วงโหลดฟอร์มสูงสุด ~0.5 วินาทีถ้า endpoint เป็น blackhole (2) เป็นโค้ด debug ที่หลุดไป production (code smell) (3) ไม่ใช่ช่องโหว่ด้านความปลอดภัยเพราะเป็น localhost เท่านั้น

**ข้อเสนอแนะ:** ลบ `#Region "debug-point Z"` ทั้ง region หรือครอบ `DebugReport` ด้วย `#If DEBUG Then` ทั้งหมด

---

### A3. `�` glyph เสีย 2 จุด — ไม่พบในโค้ดปัจจุบัน (แก้ไปแล้ว)

**สถานะจริง:** grep ตัวอักษร `�` (U+FFFD) ใน `FrmTransactions.vb` และ `FrmTransactions.Designer.vb` **ไม่พบ match** ปุ่ม receipt-retrofit ทั้ง 4 ปุ่มมีข้อความสมบูรณ์ครบ:
- `btnViewReceipt` = "🔍 ดูใบเสร็จ"
- `btnPasteReceipt` = "📋 วางรูปย้อนหลัง"
- `btnBrowseReceipt` = "📂 เลือกรูปย้อนหลัง"
- `btnDeleteReceipt` = "❌ ลบรูปใบเสร็จ"

**ความเสี่ยง/ผลกระทบ:** ไม่มี (ประเด็นนี้ถูกแก้ไปแล้ว หรือรายงานใน profile คลาดเคลื่อน)

**ข้อเสนอแนะ:** ไม่ต้องดำเนินการ

---

### A4. `FrmLocationImport` CSV parser ไม่รองรับ quoted field — ยืนยัน

**สถานะจริง:** ใช้ `ln.Split(","c)` ตรง ๆ ที่ [FrmLocationImport.vb#L166](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmLocationImport.vb#L166), [L188](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmLocationImport.vb#L188), [L212](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmLocationImport.vb#L212) ไม่ใช้ library ใด ๆ

```vb
Dim cols = ln.Split(","c)
Dim id As Integer : If Not Integer.TryParse(cols(0).Trim(), id) Then Continue For
```

**ความเสี่ยง/ผลกระทบ:** ถ้าค่ามี comma ในเครื่องหมายคำพูด (เช่น `"1","บางกอก, น้อย"`) จะถูกตัดเป็นคอลัมน์ผิด → ชื่อเพี้ยน หรือ index เกินจน `TryParse`/`cols(1)` พัง (บางกรณี throw `IndexOutOfRangeException` ที่ไม่ถูก catch ต่อ field) ความเสี่ยงจริงค่อนข้างต่ำเพราะชื่อจังหวัด/อำเภอ/ตำบลไทยปกติไม่มี comma แต่ไฟล์ CSV มาตรฐานจากแหล่งภายนอกอาจมี

**ข้อเสนอแนะ:** ใช้ `Microsoft.VisualBasic.FileIO.TextFieldParser` (มี built-in รองรับ quoted field) หรือไลบรารี CSV มาตรฐาน

---

### A5. `FrmPersonnelManagement` — fullName title ติดกัน + Delete ตำแหน่ง error ดิบ — ยืนยัน

**สถานะจริง:**
- Concat ชื่อที่ [FrmPersonnelManagement.vb#L154](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmPersonnelManagement.vb#L154) **ไม่มีช่องว่าง** ระหว่างคำนำหน้ากับชื่อ:

```vb
Dim fullName = $"{txtTitle.Text.Trim()}{txtFirstName.Text.Trim()} {txtLastName.Text.Trim()}"
' ผลลัพธ์: "พระสมชาย ใจดี" แทน "พระ สมชาย ใจดี"
```

- Delete ตำแหน่งที่ [L236-L246](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmPersonnelManagement.vb#L236-L246) แสดง error ดิบจาก OLEDB:

```vb
Db.ExecuteNonQuery(conn, "DELETE FROM Positions WHERE PositionID=@id", ...)
...
Catch ex As Exception
    MessageBox.Show("ไม่สามารถลบข้อมูลได้: " & ex.Message, ...)
```

**ความเสี่ยง/ผลกระทบ:** (1) ชื่อเต็มขึ้นเป็น "พระสมชาย" ดูไม่สวย/ไม่ถูกต้องตามธรรมเนียม (2) ถ้าลบตำแหน่งที่ยังถูกอ้างอิงใน `Personnel` ผู้ใช้จะเห็นข้อความ error ดิบของ OLEDB (อังกฤษ/อ่านยาก) ไม่ใช่ข้อความไทยที่เป็นมิตร

**ข้อเสนอแนะ:** แก้ concat เป็น `$"{txtTitle.Text.Trim()} {txtFirstName.Text.Trim()} {txtLastName.Text.Trim()}"`; ใน `DeletePosition` ตรวจ FK ก่อน (`SELECT COUNT(*) FROM Personnel WHERE PositionID=@id`) แล้วแสดงข้อความไทย "ไม่สามารถลบได้ เนื่องจากตำแหน่งนี้มีบุคลากรใช้งานอยู่"

---

### A6. `LoadMonkSample()` อ่านคอลัมน์ legacy ที่ถูกลบ — ยืนยัน

**สถานะจริง:**
- [frmMain.vb#L765](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/frmMain.vb#L765) query คอลัมน์ที่ถูก migration ลบไปแล้ว:

```vb
Dim row = Db.GetTable(conn, "SELECT TOP 1 TempleName, AbbotName, WaiyawatName, BookkeeperName FROM TempleSetting ORDER BY ID DESC")
```

- แต่ `MigrateTempleSetting()` ใน [AccessDbProvider.vb#L122-L130](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Services/AccessDbProvider.vb#L122-L130) **DROP คอลัมน์เหล่านั้นทิ้ง** หลังย้ายข้อมูลไป `AbbotPersonnelID`/`WaiyawatPersonnelID`/`BookkeeperPersonnelID`:

```vb
Dim legacyCols = {"ABBOTNAME", "ABBOTOFFICESTATUS", "WAIYAWATNAME", "WAIYAWATOFFICESTATUS", "BOOKKEEPERNAME", ...}
For Each lc In legacyCols
    If colNames.Contains(lc) Then
        ExecuteNonQuery(conn, $"ALTER TABLE TempleSetting DROP COLUMN {lc}")
```

- การ guard `If(r("AbbotName") Is DBNull.Value, ...)` (L772-L776) กันได้เฉพาะค่า NULL **ไม่กันคอลัมน์ที่ไม่มีอยู่** → query จะ throw `ArgumentException` (column ไม่มีในตาราง) ถูก `Catch` ที่ L783 จับ → การ์ดแสดง "-" ตลอด

**ความเสี่ยง/ผลกระทบ:** Dashboard การ์ด "เจ้าอาวาส / ไวยาวัจกร / ผู้ทำบัญชี" **ไม่แสดงข้อมูลจริงเลย** แม้ผู้ใช้ตั้งค่าไว้แล้ว เป็นบั๊กเงียบ (silent, มี fallback "-")

**ข้อเสนอแนะ:** เปลี่ยน query เป็น JOIN กับ `Personnel` ผ่าน `AbbotPersonnelID`/`WaiyawatPersonnelID`/`BookkeeperPersonnelID` (เช่น `LEFT JOIN Personnel a ON TempleSetting.AbbotPersonnelID = a.PersonnelID` แล้วอ่าน `a.FullName`) หรืออ่าน ID แล้วไป lookup ชื่อแยก

---

### A7. `BaseReport` + `AutoScaleMode` ผสมกัน

**สถานะจริง:**
- **BaseReport:** grep `Class BaseReport` ทั้งโปรเจกต์ **ไม่พบ class จริง** — พบเฉพาะในไฟล์เอกสาร/checklist ซึ่งระบุว่า "deleted from disk + removed from `.vbproj`" (ดู `Docs\Temple_Accounting_Prerelease_Checklist.md`). **ประเด็น BaseReport ใน profile V7 ข้อ 7 จึง outdated — แก้ไปแล้ว**
- **AutoScaleMode ยังผสมกันจริง:**

| ฟอร์ม | AutoScaleMode |
|---|---|
| FrmExportForTransfer, FrmTempleSetting, FrmMultiMachineImport | `Dpi` |
| frmMain, FrmReports, FrmTransactions, FrmPersonnelManagement, FrmHelpDialog, FrmReceiptViewer | `Font` |
| FrmIncome, FrmExpense, FrmTransfer, FrmMasterData, FrmLocationImport | ไม่ระบุ (ค่า default) |

**ความเสี่ยง/ผลกระทบ:** ฟอร์มที่ออกแบบด้วย `Font` (ออกแบบที่ 100% DPI) อาจแสดงผลคลาดเคลื่อน/ตัวหนังสือล้น/ทับซ้อนเมื่อจอเป็น 150% (ตรงกับคำเตือนของ Visual Studio "Scaling on your main display is set to 150%") การผสม `Dpi`/`Font`/ไม่ระบุ ทำให้พฤติกรรมไม่สม่ำเสมอระหว่างหน้าจอ

**ข้อเสนอแนะ:** กำหนดมาตรฐานเดียว เช่น `AutoScaleMode.Dpi` + `AutoScaleDimensions` ที่สอดคล้องกันทุกฟอร์ม แล้วเปิดตรวจบนจอ 150% จริง

---

### B8. hardcoded `YEAR(TranDate) = 2026` — ยืนยัน (ความเสี่ยงสูงสุด)

**สถานะจริง:** พบ `OR YEAR(TranDate) = 2026` ใน query สรุปยอด **5 จุด**:

| ไฟล์ | บรรทัด | ใช้กับ |
|---|---|---|
| `Forms\FrmIncome.vb` | L263 | สรุปยอดรายรับเดือนนี้ |
| `Forms\FrmExpense.vb` | L217 | สรุปยอดรายจ่ายเดือนนี้ |
| `frmMain.vb` | L347 | การ์ดจำนวนผู้บริจาค |
| `frmMain.vb` | L354 | การ์ดรายรับเดือนนี้ |
| `frmMain.vb` | L361 | การ์ดรายจ่ายเดือนนี้ |

ตัวอย่าง (FrmExpense.vb L215-L217):
```vb
"WHERE (TranType LIKE '%จ่าย%' OR TranType = 'Expense') " &
"AND MONTH(TranDate) = MONTH(Date()) " &
"AND (YEAR(TranDate) = YEAR(Date()) OR YEAR(TranDate) = 2026)")
```

นอกจากนี้ยังมีค่า `New Date(2026, ...)` ฝังใน Designer ของ `FrmIncome`/`FrmExpense`/`FrmReports` (dtpDate/dtFrom/dtTo default) ซึ่งเป็นค่า seed ของ date picker

**ความเสี่ยง/ผลกระทบ (สำคัญ):**
1. ในปี 2026 เงื่อนไขนี้**ซ้ำซ้อน** (`YEAR(Date())` = 2026 อยู่แล้ว)
2. ตั้งแต่ 2027 ขึ้นไป `MONTH(TranDate)=MONTH(Date()) AND YEAR(TranDate)=2026` จะไป**ดึงข้อมูลเดือนเดียวกันของปี 2026 มาปนกับปี 2027** → ยอดสรุป "เดือนนี้" ผิดทันที (over-count)
3. ค่า `2026` นี้ **ไม่ได้ช่วยกรณีข้อมูลปี พ.ศ. (2569) เลย** — ถ้าข้อมูล legacy เก็บเป็น พ.ศ. ค่าจะเป็น 2569 ไม่ใช่ 2026

**ข้อเสนอแนะ:** เปลี่ยนเป็น query ที่ไม่ hardcode ปี เช่น `AND TranDate >= <first day of current month>` + `<first day of next month>` (คำนวณใน VB แล้วส่งเป็น parameter) หรือลบ `OR YEAR(TranDate)=2026` ออกแล้วพึ่ง `NormalizeGregorianDate` + parameter แทน

---

### B9. พ.ศ./ค.ศ. ปนกันใน DB — พบข้อมูลใหม่

**สถานะจริง (สำคัญ — ต่างจากที่ brief สงสัย):**
- ฟังก์ชัน `NormalizeGregorianDate()` [AccessDbProvider.vb#L339-L344](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Services/AccessDbProvider.vb#L339-L344) ถ้า `Year > 2400` ให้ลบ 543 (พ.ศ.→ค.ศ.)
- จุดสำคัญ: `AccessDateLiteral()` [L346-L349](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Services/AccessDbProvider.vb#L346-L349) **เรียก `NormalizeGregorianDate` ภายในตัวเอง**:

```vb
Public Function AccessDateLiteral(value As Date) As String Implements IDbProvider.AccessDateLiteral
    Dim safeValue = NormalizeGregorianDate(value)
    Return "#" & safeValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) & "#"
End Function
```

- เส้นทาง**เขียน**ข้อมูลใช้ `AccessDateLiteral` ทั้งหมด จึง **normalize เป็น ค.ศ. ตั้งแต่ตอน INSERT/UPDATE แล้ว**:
  - `AccessTransactionRepository.InsertIncome/UpdateIncome` (FrmIncome)
  - `FrmExpense.vb` L412 (UPDATE) และ L422 (INSERT) ใช้ `Db.AccessDateLiteral(dtpDate.Value.Date)`

**สรุป:** ข้อมูลที่เขียนใหม่ผ่านแอปเป็น ค.ศ. อยู่แล้ว (normalize ที่ write) ส่วนการเรียก `NormalizeGregorianDate` กระจายอยู่ตามจุด**อ่าน/แสดงผล** (FrmIncome L63, FrmExpense L59, FrmTransactions L202-212/L757, FrmReports L122, ReportEngine L404/439, etc.) เพื่อกันข้อมูล legacy ที่อาจเป็น พ.ศ.

**ความเสี่ยง/ผลกระทบ:** ไม่สามารถยืนยันได้ 100% ว่ามีข้อมูล พ.ศ. ปนใน `.accdb` จริงหรือไม่ (ไม่มี Access driver ให้ query ตรงนี้) แต่การอ่านยัง normalize ซ้ำซ้อนบ่งชี้ว่าเคยเจอข้อมูล พ.ศ. ถ้ามีข้อมูล legacy เป็น พ.ศ. (ปี>2400) อยู่จริง จะ**หายไปจาก summary** เพราะ `YEAR(TranDate)=2026` ไม่ match 2569 (สอดคล้องกับ B8)

**ข้อเสนอแนะ:** (ต่อใน B10)

---

### B10. แนวทาง migration ปี พ.ศ.→ค.ศ. (ข้อเสนอ — ยังไม่ทำ)

**แนวทาง:**
1. **ยืนยันข้อมูล:** เขียน query วินิจฉัย `SELECT MIN(TranDate), MAX(TranDate), COUNT(*) FROM Transactions WHERE YEAR(TranDate) > 2400` เพื่อดูว่ามีแถว พ.ศ. จริงไหม
2. **migration ครั้งเดียว:** `UPDATE Transactions SET TranDate = DateAdd('yyyy', -543, TranDate) WHERE YEAR(TranDate) > 2400` (รันหลัง backup)
3. **เลิกพึ่ง hardcoded year:** ลบ `OR YEAR(TranDate)=2026` ออกทั้งหมด แล้วคำนวณช่วงเดือนใน VB ส่งเป็น parameter (`@from`, `@to`) ผ่าน `AccessDateLiteral`
4. **คง `NormalizeGregorianDate` ไว้**ที่ write path เท่านั้น (กันกรณี DateTimePicker/culture เอา พ.ศ. มา) และค่อย ๆ ลดการใช้ที่ read path ลงหลัง migration เสร็จ

---

### C11. ไม่มีระบบ user/login/role — ยืนยัน

**สถานะจริง:** grep `Login|UserID|Role|Permission|CurrentUser|Password` ทั้งโปรเจกต์ พบ**เพียงจุดเดียว**คือ comment/placeholder ใน [frmMain.vb#L859-L866](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/frmMain.vb#L859-L866):

```vb
Private Sub BtnLogout_Click(sender As Object, e As EventArgs)
    ...
    MessageBox.Show("ระบบจะกลับสู่หน้าเข้าสู่ระบบ (ตัวอย่างเท่านั้น)" & vbCrLf & "นำฟอร์ม frmLogin มาเรียกในที่นี่ได้เลย", ...)
```

**ไม่มี** frmLogin, ไม่มีตาราง user, ไม่มีการตรวจ role/permission ใด ๆ

**ความเสี่ยง/ผลกระทบ:** ใครก็ตามที่เปิดโปรแกรมสามารถแก้ไข/เพิ่ม/ลบรายการบัญชีได้ทั้งหมดโดยไม่มีการยืนยันตัวตนหรือแยกสิทธิ์ — เสี่ยงต่อความถูกต้องและความโปร่งใสของบัญชีวัด

**ข้อเสนอแนะ:** อย่างน้อยที่สุดเพิ่ม (1) การยืนยันตัวตนขั้นพื้นฐาน (username/password เก็บ hash) และ (2) ระดับสิทธิ์ (ดูอย่างเดียว / แก้ไข / ผู้ดูแล) แม้เป็นโหมด single-machine ก็ตาม

---

### C12. hard delete + ไม่มี audit trail — ยืนยัน

**สถานะจริง:**
- การลบรายการที่ [FrmTransactions.vb#L592](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmTransactions.vb#L592) เป็น hard delete:

```vb
cmd.CommandText = "DELETE FROM Transactions WHERE ID = @id"
```

- Schema `Transactions` ([AccessDbProvider.vb#L50](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Services/AccessDbProvider.vb#L50)) **ไม่มี**คอลัมน์ `DeletedDate`/`IsDeleted`/`ModifiedBy`/`CreatedBy` และไม่มีตาราง audit log (grep `SoftDelete|DeletedDate|IsDeleted|ModifiedBy|CreatedBy|AuditLog` = ไม่พบ)
- มี `ImportHistory` อยู่ แต่ใช้กับ multi-machine import เท่านั้น ไม่เกี่ยวกับการลบ/แก้ไขโดยผู้ใช้

**ความเสี่ยง/ผลกระทบ:** ลบแล้วหายถาวร ไม่มีบันทึกว่าใครลบ/เมื่อไหร่/ลบอะไร → ตรวจสอบย้อนหลังไม่ได้ (auditability ของบัญชีวัด) เป็นความเสี่ยงด้านความโปร่งใสสูง

**ข้อเสนอแนะ:** เพิ่ม audit trail ขั้นต่ำ เช่น ตาราง `TransactionLog` (LogID, TransactionID, Action [INSERT/UPDATE/DELETE], OldValue/NewValue, ChangedAt, ChangedBy) หรือเปลี่ยนเป็น soft delete (`IsDeleted` flag) สำหรับรายการบัญชี

---

### D13. Dedup GUID: แก้ไขซ้ำถูกข้ามเงียบ ๆ — ยืนยัน

**สถานะจริง:**
- `AnalyzeAllDuplicates()` [FrmMultiMachineImport.vb#L681-L735](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmMultiMachineImport.vb#L681-L735) โหลด `TransactionGUID` ทั้งหมดลง `_dbGuidSet` แล้วทำเครื่องหมาย:
  - GUID ตรง → `IsDuplicateExact = True` (ซ้ำแน่นอน)
  - composite key ตรง (ไม่มี GUID) → `IsSuspectedDuplicate = True` (สงสัยซ้ำ)
- ตอน merge [L1004-L1009](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Forms/FrmMultiMachineImport.vb#L1004-L1009):

```vb
If row.IsDuplicateExact Then
    fileSkipped += 1        ' ข้าม (ไม่ overwrite)
...
If row.IsSuspectedDuplicate AndAlso skipSuspected Then
    fileSkipped += 1        ' ข้ามถ้าผู้ใช้ยืนยัน
```

**ความเสี่ยง/ผลกระทบ:** ถ้าเครื่องสองเครื่องแก้ไข record **เดิม (GUID เดียวกัน) ให้ค่าต่างกัน** ระบบจะ **skip เงียบ ๆ** (นับเป็น "ข้าม N รายการ") โดยไม่ overwrite ไม่ merge ไม่แจ้ง conflict → การแก้ไขของเครื่องที่สองหายถาวร ไม่มีกลไก "last-write-wins" หรือหน้าจอเลือกเวอร์ชัน

**ข้อเสนอแนะ:** เพิ่มการแจ้งเตือนชัดเจนเมื่อพบ exact-duplicate ที่ค่าต่างกัน (เปรียบเทียบ field) และให้ผู้ใช้เลือกว่าจะ skip/overwrite/เก็บทั้งคู่ หรืออย่างน้อย log รายการที่ถูกข้ามพร้อมเหตุผล

---

### D14. Access file-based ไม่กันการเปิดพร้อมกัน — ยืนยัน (ไม่มีการจัดการ)

**สถานะจริง:**
- Connection string [AccessDbProvider.vb#L22](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Services/AccessDbProvider.vb#L22) เป็นค่ามาตรฐาน ไม่มี flag พิเศษ:

```vb
Return $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;"
```

- grep คำว่า `in use|locked|exclusive|ไฟล์ถูกล็อก` ไม่พบโค้ดตรวจสอบ/จัดการ lock ของ DB เอง (ที่พบคือ comment ใน `DatabaseBackupHelper` ที่ skip ไฟล์ตอน copy เท่านั้น และ `SaddhiviharikaDb` เป็น `SyncLock` คนละเรื่อง)

**ความเสี่ยง/ผลกระทบ:** ถ้าเปิด `.accdb` พร้อมกันจากสองเครื่องผ่าน network share จะเกิด OLEDB lock error ("Could not lock file" / "File already in use") แบบ raw exception โดยไม่มีข้อความแนะนำผู้ใช้ โปรเจกต์นี้ออกแบบเป็น single-machine + ใช้ offline export/import สำหรับ multi-machine อยู่แล้ว จึงความเสี่ยงจริงอยู่ในระดับกลาง แต่ถ้ามีผู้ใช้เผลอเปิดผ่าน share จะเจอ error อ่านยาก

**ข้อเสนอแนะ:** เพิ่มการตรวจก่อนเปิด (ตรวจ lock file `.ldb`/`.laccdb` หรือลองเปิด exclusive) แล้วแสดงข้อความไทย "ฐานข้อมูลถูกเปิดใช้งานอยู่ที่อื่น กรุณาปิดเครื่องอื่นก่อน" หรือกำหนดให้รันแบบ single-instance

---

### E15. Backup ไม่มี retention/cleanup — ยืนยัน

**สถานะจริง:**
- `BackupFullSystem()` [DatabaseBackupHelper.vb#L18-L69](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/DatabaseBackupHelper.vb#L18-L69) สร้างโฟลเดอร์ `Backup_{yyyyMMdd_HHmmss}` ทุกครั้ง **ไม่มีโค้ดลบ backup เก่า** (ไม่มี retention/cleanup)
- จุดเรียก: manual (`btnBackup_Click` frmMain L877), auto ตอนปิดโปรแกรม (`FrmMain_FormClosing` L909), `Db.BackupDatabase()` ก่อน import Excel (L87) และก่อน merge (FrmMultiMachineImport L940)

**ความเสี่ยง/ผลกระทบ:** auto-backup ทุกครั้งที่ปิดโปรแกรม + manual + ก่อน import → backup สะสมไม่จำกัด กินพื้นที่ดิสก์เรื่อย ๆ (ความเสี่ยงต่ำ-กลาง แต่ควรมีเพดาน)

**ข้อเสนอแนะ:** เพิ่ม retention เช่น เก็บย้อนหลัง N วัน / N ชุดล่าสุด (ลบโฟลเดอร์ `Backup_*` ที่เก่ากว่ากำหนดตอนเริ่ม backup) หรือจำกัดจำนวนชุดต่อเดือน

---

## ข้อเสนอแนะลำดับความสำคัญในการแก้ไข (Priority Roadmap)

| ลำดับ | รายการ | เหตุผล |
|---|---|---|
| 1 | **B8 — ลบ hardcoded `YEAR(TranDate)=2026`** (5 จุด) เปลี่ยนเป็น parameter ช่วงเดือน | กระทบความถูกต้องของตัวเลขบัญชีโดยตรง และจะพังแน่นอนในปี 2027 |
| 2 | **A6 — แก้ `LoadMonkSample()`** ให้ JOIN `Personnel` ผ่าน `*PersonnelID` | Dashboard แสดงข้อมูลบุคลากร "-" ตลอด แก้ได้เร็วและเห็นผลชัด |
| 3 | **A1 — ต่อ `SaveSelectedRowEdits` เข้า `btnEdit`** เมื่อ `_isEditing` | ผู้ใช้แก้โอนเงินแล้วข้อมูลหายเงียบ ๆ (silent data loss) |
| 4 | **C11 + C12 — เพิ่ม login/role ขั้นต่ำ + audit trail / soft delete** | ความโปร่งใสบัญชีวัด กระทบเชิงนโยบาย/ความน่าเชื่อถือ |
| 5 | **B9/B10 — migration รวมปี พ.ศ.→ค.ศ. ครั้งเดียว** | ทำให้ข้อมูลเป็นมาตรฐานเดียวและเลิกพึ่ง workaround |
| 6 | **D13 — แจ้ง conflict เมื่อ dedup พบ GUID ซ้ำค่าต่างกัน** | ป้องกันการแก้ไขข้ามเครื่องหายเงียบ ๆ |
| 7 | **A5 — แก้ fullName concat + error delete ตำแหน่ง** | UX/ความถูกต้องของชื่อบุคลากร |
| 8 | **A7 — ทำ AutoScaleMode ให้เป็นมาตรฐานเดียว (Dpi)** | แก้ layout เพี้ยนบนจอ 150% DPI |
| 9 | **D14 — แจ้งเตือนเมื่อ DB ถูกเปิดพร้อมกัน** | ป้องกัน lock error อ่านยาก |
| 10 | **E15 — เพิ่ม backup retention** | กันดิสก์เต็ม |
| 11 | **A4 — เปลี่ยน CSV parser เป็น TextFieldParser** | ความถูกต้องของข้อมูลที่อยู่ |
| 12 | **A2 — ปิด/guard debug harness** | ลบโค้ด debug ออกจาก production |

---

*หมายเหตุ: รายงานนี้เกิดจากการอ่านโค้ดจริง ณ วันที่ตรวจสอบ ไม่มีการแก้ไขโค้ดใด ๆ ตามข้อกำหนดของ audit brief ประเด็นที่รอการยืนยันข้อมูลจริง (เช่น จำนวนแถว พ.ศ. ใน `.accdb`) จำเป็นต้องใช้ Access driver query จริงเพิ่มเติมก่อนทำ migration*
