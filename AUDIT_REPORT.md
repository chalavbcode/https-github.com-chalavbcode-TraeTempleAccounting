# Audit Report — Temple Accounting System
วันที่ตรวจสอบ: 19 สิงหาคม 2569 (2026-08-19)
ตรวจสอบโดย: Buffy (Codebuff Auditor) — อ่านโค้ดจริงเทียบกับ PROJECT_PROFILE_V7.md

---

## สรุปผู้บริหาร (Executive Summary)

ตรวจสอบโค้ดจริงทั้ง 15 หัวข้อ พบว่า:

- **เอกสาร PROFILE_V7 บางข้อมูลล้าสมัย** — บางบั๊กที่ระบุไว้ใน Section 5.3 ได้รับการแก้ไขแล้วในโค้ดจริง (เช่น `LoadMonkSample` อ่าน legacy columns, hardcoded year 2026)
- **ไม่มีระบบ Login/Permission/Audit Trail เลย** — ใครก็เปิดโปรแกรมได้ แก้ไข/ลบข้อมูลทุกรายการโดยไม่มีการบันทึกว่าใครทำ (ระดับความเสี่ยง: **สูง**)
- **Hard Delete ไม่มี soft-delete** — ข้อมูลบัญชีที่ลบแล้วหายเลย ตรวจสอบย้อนหลังไม่ได้ (ระดับ: **สูง**)
- **Debug harness ยังทำงานใน Release** — ส่ง HTTP request ไป `127.0.0.1:7777` ทุกครั้งที่เปิด/ใช้งาน FrmTransactions (ระดับ: **กลาง**)
- **Backup ไม่มี retention** — สำรองทุกครั้งที่ปิดโปรแกรม แต่ไม่ลบเก่า อาจกินพื้นที่หลาย GB ต่อเดือน (ระดับ: **กลาง**)
- **AutoScaleMode ผสม Font/Dpi** — ฟอร์มหลักใช้ Font แต่ 4 ฟอร์มใช้ Dpi อาจเพี้ยนบนจอ 150%+ (ระดับ: **ต่ำ-กลาง**)

**ควรแก้ก่อน:** ข้อ C11-C12 (ไม่มี auth/audit trail) → ข้อ A2 (debug harness) → ข้อ E15 (backup retention) → ข้อ D14 (concurrent access) → ที่เหลือตามลำดับ

---

## ตารางสรุปความเสี่ยง

| # | หัวข้อ | สถานะ | ความรุนแรง | ไฟล์ที่เกี่ยวข้อง |
|---|--------|-------|-----------|-------------------|
| A1 | Transfer in-grid edit — SaveSelectedRowEdits | ✅ โค้ดมีอยู่จริงและทำงานได้ (เอกสารล้าสมัย) | ต่ำ | Forms/FrmTransactions.vb |
| A2 | Debug harness 127.0.0.1:7777 | ⚠️ ยังรันใน Release, blocking call, ไม่มี guard | กลาง | Forms/FrmTransactions.vb |
| A3 | Glyph เสีย 2 จุดบนปุ่ม receipt-retrofit | ❓ ไม่พบ glyph เสียในซอร์สโค้ด — อาจเป็น runtime font rendering | ต่ำ | Forms/FrmTransactions.Designer.vb |
| A4 | CSV parser ไม่รองรับ quoted field | ✅ ยืนยัน — ใช้ Split(",") ตรงๆ | ต่ำ | Forms/FrmLocationImport.vb |
| A5 | fullName title ไม่มีช่องว่าง + Position delete error | ✅ ยืนยันทั้ง 2 จุด | ต่ำ-กลาง | Forms/FrmPersonnelManagement.vb |
| A6 | LoadMonkSample() legacy columns | ❌ **เอกสารล้าสมัย** — โค้ดจริงอ่าน PersonnelID-based schema แล้ว | ไม่มี | frmMain.vb (บรรทัด 764) |
| A7 | BaseReport dead scaffolding + AutoScaleMode ผสม | ✅ BaseReport ไม่มีในโค้ด; AutoScaleMode ผสมจริง | ต่ำ-กลาง | 10 ไฟล์ Designer.vb |
| B8 | Hardcoded YEAR()=2026 | ❌ **เอกสารล้าสมัย** — ไม่พบ hardcoded 2026 ใน SQL แล้ว | ไม่มี | — |
| B9 | NormalizeGregorianDate — insert vs display | ⚠️ เรียกตอน insert แล้ว (TransactionImporter) แต่ยังมี混合 dates ใน DB ได้ | กลาง | Services/AccessDbProvider.vb, ExcelTransactionImporter.vb |
| B10 | Migration พ.ศ./ค.ศ. | เสนอแนวทาง (ดูรายละเอียดด้านล่าง) | กลาง | — |
| C11 | ไม่มีระบบ Login/Permission | ✅ **ยืนยัน — ไม่มีเลย** | สูง | ทั้งโปรเจกต์ |
| C12 | Hard delete ไม่มี audit trail | ✅ **ยืนยัน — DELETE FROM Transactions โดยตรง** | สูง | Forms/FrmTransactions.vb (บรรทัด 592) |
| D13 | TransactionGUID + SHA-256 dedup | ✅ ทำงานจริง — GUID skip + composite key suspect dedup | ต่ำ | Forms/FrmMultiMachineImport.vb |
| D14 | Access .accdb concurrent access | ⚠️ ไม่มี locking/concurrency guard สำหรับ network share | กลาง | Database.vb, Services/AccessDbProvider.vb |
| E15 | Backup retention/cleanup | ⚠️ ไม่มี retention — สะสมไปเรื่อยๆ | กลาง | DatabaseBackupHelper.vb |

---

## รายละเอียดแต่ละข้อ

### A1. Transfer Row In-Grid Edit — SaveSelectedRowEdits
**สถานะจริง: หักล้างเอกสาร — โค้ดทำงานได้แล้ว**

เอกสาร Section 5.3 ระบุว่า `SaveSelectedRowEdits` เป็น "dead code" แต่จากการตรวจสอบโค้ดจริง:

**โค้ดจริง (FrmTransactions.vb บรรทัด ~475-560):**
```vb
Private Sub btnEdit_Click(...) Handles btnEdit.Click
    If _isEditing Then
        SaveSelectedRowEdits()   ' ← เรียกเมื่อกดปุ่ม "บันทึกแก้ไข" ครั้งที่สอง
        Return
    End If
    ...
    Case "Transfer"
        _editingTransactionId = id
        _isEditing = True
        ApplyEditModeToGrid()    ' ← เปิดโหมดแก้ไขในตาราง
```

`SaveSelectedRowEdits` **มีอยู่จริงและมี logic ครบถ้วน**: อ่านค่าจาก grid → ValidateTransactionByType → UPDATE Transactions (แยก Income/Expense กับ Transfer) → MessageBox สำเร็จ/ล้มเหลว → ExitEditMode → LoadData

**สิ่งที่พบ:** Transfer-row editing **ไม่ใช่ dead code อีกต่อไป** — โค้ดได้รับการ implement ใหม่ให้แก้ไขในตารางโดยตรง (in-grid edit) แล้ว ผู้ใช้เลือกแถว Transfer → กด "แก้ไข" → แก้ไขในตาราง → กด "บันทึกแก้ไข" → บันทึกสำเร็จ

**ความเสี่ยง/ผลกระทบ:** ต่ำ — โค้ดทำงานได้ แต่ UI ไม่ค่อย intuitive (user ต้องกดปุ่มเดิมซ้ำเพื่อบันทึก ไม่ใช่ปุ่มแยก)

**ข้อเสนอแนะ:** พิจารณาเพิ่ม FrmTransfer edit mode แยก (เหมือน FrmIncome/FrmExpense) แทน in-grid edit ที่ไม่ครบวงจร (เช่น ไม่รองรับการเปลี่ยน Fund/Bank dropdown)

---

### A2. Debug Harness `http://127.0.0.1:7777/event`
**สถานะจริง: ยืนยัน — ยังรันใน Release, ไม่มี #If DEBUG guard**

**โค้ดจริง (FrmTransactions.vb บรรทัด 48-76):**
```vb
Private Shared Sub DebugReport(hypothesisId As String, location As String, msg As String, data As Dictionary(Of String, Object))
    Try
        Dim url = GetDebugServerUrl()
        ...
        Using client As New HttpClient()
            client.Timeout = TimeSpan.FromMilliseconds(500)
            Using content As New StringContent(json, Encoding.UTF8, "application/json")
                client.PostAsync(url, content).GetAwaiter().GetResult()  ' ← BLOCKING!
            End Using
        End Using
    Catch
    End Try
End Sub
```

**จุดที่ถูกเรียก (ทุกจุดไม่มี `#If DEBUG` guard):**
| Debug Point | เรียกตอนไหน | จำนวนครั้ง/ฟอร์ม |
|---|---|---|
| `A:form-load` | `FrmTransactions_Load` — ทุกครั้งที่เปิดฟอร์ม | 1 |
| `A:ensure-schema` | หลัง EnsureSchema() | 1 |
| `B:loaddata-start` | ทุกครั้งที่ `LoadData()` เรียก (filter/refresh/ค้นหา) | N |
| `B:db-stats` | ใน LoadData — query stats เพิ่ม 5 queries | N |
| `C:loaddata-result` | หลัง query สำเร็จ | N |
| `D:loaddata-ex` | เมื่อเกิด exception | M |

**สรุป impact:**
- ทุกครั้งที่เปิด FrmTransactions: **2 blocking HTTP calls** (load-start + ensure-schema)
- ทุกครั้งที่ค้นหา/รีเฟรช: **3 blocking HTTP calls** (loaddata-start + db-stats + query-result) + **5 queries เพิ่ม** (db-stats)
- ถ้า endpoint ไม่มีคนฟัง: HTTP call จะ **timeout 500ms** แล้ว catch silently — ไม่ crash แต่ **ช้าลง ~500ms ต่อ request** (สูงสุด ~2.5s ต่อ LoadData)
- `GetAwaiter().GetResult()` บน UI thread = **blocking call** = UI ค้างชั่วคราว

**ความเสี่ยง/ผลกระทบ:** กลาง — ไม่ crash แต่ทำให้用户体验ช้าลงใน Release mode ทุกครั้งที่เปิด/ใช้ฟอร์ม

**ข้อเสนอแนะ:**
1. ครอบด้วย `#If DEBUG Then ... #End If` ทุกจุด
2. หรือเพิ่มตัวแปร flag `Private Const DebugEnabled As Boolean = False`
3. ลบ debug-stats queries 5 ตัวที่เพิ่ม load time

---

### A3. Glyph เสีย 2 จุดบนปุ่ม Receipt-retrofit
**สถานะจริง: ไม่พบ glyph เสียในซอร์สโค้ด — น่าจะเป็น runtime font rendering**

**โค้ดจริง (FrmTransactions.Designer.vb):**
```vb
btnPasteReceipt.Text = "📋 วางรูปย้อนหลัง"     ' ← ซอร์สโค้ดปกติ
btnBrowseReceipt.Text = "📂 เลือกรูปย้อนหลัง"   ' ← ซอร์สโค้ดปกติ
btnViewReceipt.Text = "🔍 ดูใบเสร็จ"             ' ← ปกติ
btnDeleteReceipt.Text = "❌ ลบรูปใบเสร็จ"       ' ← ปกติ
```

ทุกปุ่มมี emoji character ที่ถูกต้องในซอร์สโค้ด ไม่พบ `U+FFFD` (replacement character) ใดๆ

**วิเคราะห์สาเหตุที่อาจเป็นไปได้:**
- ฟอนต์ Tahoma ที่ใช้กับปุ่ม (`Tahoma 10F, Bold`) **ไม่รองรับ emoji ทุกตัว** — เช่น 📋 (U+1F4CB) และ 📂 (U+1F4C2) อาจ render เป็น `□` หรือ `?` บน Windows
- แต่ ❌, 🔍, 💰, 💸 ตัวอื่นๆ ใช้ได้ — อาจเป็นเฉพาะ emoji บางตัวที่ Tahoma ไม่มี glyph

**ความเสี่ยง/ผลกระทบ:** ต่ำ — เป็นปัญหาเครื่องสำอาง (cosmetic) ไม่กระทบการทำงาน

**ข้อเสนอแนะ:**
- ใช้ `Segoe UI Emoji` font สำหรับปุ่มที่มี emoji หรือ
- เปลี่ยน emoji เป็นข้อความ (เช่น "[ดู]", "[วาง]", "[เลือก]") แล้วใส่ icon ผ่าน Image property แทน

---

### A4. FrmLocationImport CSV Parser ไม่รองรับ Quoted Field
**สถานะจริง: ยืนยัน — ใช้ `Split(",")` ตรงๆ ไม่รองรับ quoted fields**

**โค้ดจริง (FrmLocationImport.vb บรรทัด ~136):**
```vb
Dim cols = ln.Split(","c)        ' ← province
If cols.Length < 2 Then Continue For
...
Dim cols = ln.Split(","c)        ' ← amphoe
If cols.Length < 3 Then Continue For
...
Dim cols = ln.Split(","c)        ' ← tambon
If cols.Length < 4 Then Continue For
```

ถ้าชื่อตำบล/อำเภอ/จังหวัดมี comma ในชื่อ (เช่น `"บางรัก, กรุงเทพ"`) จะ split ผิด ได้ column index เลื่อน → INSERT ข้อมูลผิด/ข้ามแถว

**ความเสี่ยง/ผลกระทบ:** ต่ำ — ข้อมูลจังหวัด/อำเภอ/ตำบลของไทยส่วนใหญ่ไม่มี comma แต่ถ้า CSV source มี quoted field จะพังเงียบๆ

**ข้อเสนอแนะ:**
- ใช้ `TextFieldParser` ของ .NET (`Microsoft.VisualBasic.FileIO.TextFieldParser`) ที่รองรับ quoted CSV แล้ว
- หรือใช้ library เช่น CsvHelper

---

### A5. FrmPersonnelManagement — fullName Title ไม่มีช่องว่าง + Position Delete Error
**สถานะจริง: ยืนยันทั้ง 2 จุด**

**5a. fullName concat ไม่มีช่องว่าง (FrmPersonnelManagement.vb บรรทัด ~103):**
```vb
Dim fullName = $"{txtTitle.Text.Trim()}{txtFirstName.Text.Trim()} {txtLastName.Text.Trim()}"
' ผลลัพธ์: "พระครูสมุห์สมชาย " ← ไม่มีช่องว่างระหว่าง title กับชื่อ
```

**เปรียบเทียบกับ AccessDbProvider.vb SeedPersonnel (บรรทัด ~246):**
```vb
Dim fullName = $"{item.Title}{item.First}"
If Not String.IsNullOrWhiteSpace(item.Last) Then fullName &= $" {item.Last}"
' ผลลัพธ์เหมือนกัน: "พระครูสมุห์สมชาย " ← seed data ก็ไม่มีช่องว่าง
```

ทั้งสองจุด concat `Title + FirstName` โดยไม่มีช่องว่าง ผลลัพธ์ได้ "พระครูสมุห์สมชาย" แทน "พระครูสมุห์ สมชาย"

**ความเสี่ยง:** ต่ำ-กลาง — ชื่อแสดงในรายงาน/หน้าจอจะผิด format

**5b. Position Delete ไม่มี friendly FK error (FrmPersonnelManagement.vb บรรทัด ~192):**
```vb
Private Sub DeletePosition()
    ...
    Db.ExecuteNonQuery(conn, "DELETE FROM Positions WHERE PositionID=@id", ...)
    ...
Catch ex As Exception
    MessageBox.Show("ไม่สามารถลบข้อมูลได้: " & ex.Message, "ผิดพลาด", ...)
End Try
```

ถ้า Position ที่จะลบถูกใช้อยู่ในตาราง Personnel (FK constraint) Access จะ throw OLEDB exception ที่มี message ภาษาอังกฤษ的技术术语 → ผู้ใช้ไทยจะงง

**ความเสี่ยง:** กลาง — ผู้ใช้ไม่เข้าใจ error message

**ข้อเสนอแนะ:**
- เพิ่ม `CheckSelfPermission: SELECT COUNT(*) FROM Personnel WHERE PositionID=@id` ก่อนลบ
- ถ้ามี personnel ใช้อยู่ แจ้งเตือนเป็นภาษาไทย: "ไม่สามารถลบตำแหน่งนี้ได้เพราะมีบุคลากร {count} คนใช้ตำแหน่งนี้อยู่"

---

### A6. `LoadMonkSample()` Legacy Columns — เอกสารล้าสมัย
**สถานะจริง: หักล้างเอกสาร — โค้ดได้รับการอัปเดตแล้ว**

เอกสาร Section 5.3 ระบุว่า `LoadMonkSample()` "ยังอ่าน legacy TempleSetting columns" แต่โค้ดจริง (frmMain.vb บรรทัด 764-790):

```vb
Private Sub LoadMonkSample()
    Using conn = Db.OpenConn()
        Dim row = Db.GetTable(conn,
            "SELECT TOP 1 ts.TempleName, " &
            "a.FullName AS AbbotName, w.FullName AS WaiyawatName, b.FullName AS BookkeeperName " &
            "FROM ((TempleSetting ts " &
            "LEFT JOIN Personnel a ON ts.AbbotPersonnelID = a.PersonnelID) " &
            "LEFT JOIN Personnel w ON ts.WaiyawatPersonnelID = w.PersonnelID) " &
            "LEFT JOIN Personnel b ON ts.BookkeeperPersonnelID = b.PersonnelID " &
            "ORDER BY ts.ID DESC")
```

**โค้ดจริงใช้ `PersonnelID`-based schema แล้ว** — ไม่อ่าน `AbbotName`, `WaiyawatName`, `BookkeeperName` (legacy columns) อีกต่อไป และ `IsDBNull` guard ครอบครบทุกคอลัมน์:

```vb
lblCard1Value.Text = If(r("TempleName") Is DBNull.Value, "-", r("TempleName").ToString())
lblCard2Value.Text = If(r("AbbotName") Is DBNull.Value, "-", r("AbbotName").ToString())
lblCard3Value.Text = If(r("WaiyawatName") Is DBNull.Value, "-", r("WaiyawatName").ToString())
lblCard4Value.Text = If(r("BookkeeperName") Is DBNull.Value, "-", r("BookkeeperName").ToString())
```

**ความเสี่ยง:** ไม่มี — เอกสาร Profile V7 ล้าสมัยในจุดนี้

---

### A7. BaseReport Dead Scaffolding + AutoScaleMode ผสม
**สถานะจริง: BaseReport ไม่มีอยู่ในโค้ด; AutoScaleMode ผสมจริง**

**7a. BaseReport:** ค้นหา `BaseReport` ในทั้งโปรเจกต์ → **ไม่พบเลย** (0 matches) — ถูกลบ/ refactor ออกไปแล้ว อาจเหลือแค่ในเอกสาร profile เก่า

**7b. AutoScaleMode ผสม (จาก 10 ไฟล์ Designer.vb):**

| ฟอร์ม | AutoScaleMode | ความเสี่ยง DPI |
|-------|---------------|---------------|
| frmMain | Font | พื้นฐาน |
| FrmTransactions | Font | พื้นฐาน |
| FrmReports | Font | พื้นฐาน |
| FrmPersonnelManagement | Font | พื้นฐาน |
| FrmReceiptViewer | Font | พื้นฐาน |
| FrmHelpDialog | Font | พื้นฐาน |
| **FrmTempleSetting** | **Dpi** | ต่างจากฟอร์มอื่น |
| **FrmMultiMachineImport** | **Dpi** | ต่างจากฟอร์มอื่น |
| **FrmExportForTransfer** | **Dpi** | ต่างจากฟอร์มอื่น |
| **FrmChartPreview** | **Dpi** | ต่างจากฟอร์มอื่น |

4 ฟอร์มใช้ `AutoScaleMode.Dpi` แต่ฟอร์มหลัก/ฟอร์มลูกส่วนใหญ่ใช้ `AutoScaleMode.Font` — ถ้า显示器 scaling 150% (ตาม screenshot ที่ VS เตือน) ฟอร์ม Dpi จะ scale ต่างจาก Font → layout เพี้ยนเมื่อสลับหน้า

**ความเสี่ยง/ผลกระทบ:** ต่ำ-กลาง — layout อาจเพี้ยนบนจอ HiDPI แต่ไม่ crash

**ข้อเสนอแนะ:** Standardize เป็น `AutoScaleMode.Dpi` ทุกฟอร์ม พร้อมปรับ `AutoScaleDimensions` ให้สอดคล้อง

---

### B8. Hardcoded YEAR() = 2026 — เอกสารล้าสมัย
**สถานะจริง: หักล้างเอกสาร — ไม่พบ hardcoded 2026 ใน SQL แล้ว**

ค้นหา `2026` ทั้งโปรเจกต์ `.vb` พบเพียง 3 จุด:
1. `frmMain.vb:118` — copyright string "สงวนลิขสิทธิ์ © 2026" (ไม่ใช่ SQL)
2. `FrmReports.vb:49` — comment "แทน Designer default ที่ hardcode ปี 2026" (comment เท่านั้น)
3. `FrmReports.vb:439` — comment "th-TH culture แปลงปี ค.ศ. เป็น พ.ศ." (comment เท่านั้น)

**ไม่มี SQL query ใดที่ hardcode `YEAR(TranDate) = 2026`** โค้ดใช้ dynamic date ranges ทั้งหมด:
```vb
' frmMain.vb — dashboard queries ใช้ Date.Today.Year
Dim monthStart As New Date(Date.Today.Year, Date.Today.Month, 1)
Dim firstOfNextMonth As Date = monthStart.AddMonths(1)
"AND TranDate >= " & Db.AccessDateLiteral(firstOfMonth) & " " &
"AND TranDate < " & Db.AccessDateLiteral(firstOfNextMonth)
```

**ความเสี่ยง:** ไม่มี — เอกสาร Profile V7 Section 4.3 ล้าสมัย

---

### B9. Db.NormalizeGregorianDate — Insert vs Display
**สถานะจริง: เรียกทั้งตอน insert และ display — แต่ยังมี混合 dates ได้**

**จุดที่เรียก `NormalizeGregorianDate` ตอน INSERT:**
- `ExcelTransactionImporter.vb` (บรรทัด ~94): `Dim normalizedDate = Db.NormalizeGregorianDate(tranDate.Date)` → INSERT INTO Transactions
- `FrmMultiMachineImport.vb`: ใช้ `Db.AccessDateLiteral()` (ซึ่งเรียก `NormalizeGregorianDate` ภายใน) → INSERT INTO Transactions

**จุดที่เรียกตอน DISPLAY/QUERY:**
- `FrmTransactions.vb` — `BuildLedgerQuery`, `LoadFilters` → แปลง year > 2400 ด้วย SQL `IIF(Year(t.TranDate)>2400, DateAdd('yyyy',-543,t.TranDate), t.TranDate)`
- `ReportEngine.vb` — `GetBalanceBeforeDate`, `BuildRowList` → ใช้ `DateSerial(IIF(Year(...) > 2400, Year(...) - 543, Year(...)), Month(...), Day(...))`

**ปัญหาคือ:** ถ้า OS locale ของเครื่องต้นทางเป็น Thai (th-TH) Access อาจเก็บวันที่เป็น Buddhist year (ปี > 2400) โดยอัตโนมัติ — `NormalizeGregorianDate` จะแปลงก่อน insert แต่ถ้าข้อมูลถูก insert ผ่านช่องทางอื่น (manual SQL, Access UI) ที่ไม่ผ่าน `NormalizeGregorianDate` → mixed dates จะหลุดเข้ามา

**วิธีตรวจสอบ:** ไม่สามารถ query ฐานข้อมูลจริงได้จากที่นี้ แต่ pattern ที่ใช้ใน SQL (`IIF(Year > 2400, -543, ...)`) แสดงว่า ** developer รู้ว่ามี混合 dates ใน DB จริง**

**ความเสี่ยง/ผลกระทบ:** กลาง — ถ้ามี mixed dates, queries จะยังทำงานได้ (เพราะมี IIF guard) แต่ performance จะแย่ลง (column scan ทุกครั้ง)

**ข้อเสนอแนะ:** ดูข้อ B10

---

### B10. Migration พ.ศ./ค.ศ. — เสนอแนวทาง
**สถานะจริง: เสนอแนวทาง (ไม่ได้รันจริง)**

**ปัญหา:** ฐานข้อมูลอาจมีวันที่แบบ พ.ศ. (year > 2400) และ ค.ศ. (year < 2400) ปนกันในตาราง Transactions

**ข้อเสนอแนะ Migration Script:**
```sql
-- Step 1: ตรวจสอบว่ามี mixed dates จริงไหม
SELECT MIN(Year(TranDate)) AS MinYear, MAX(Year(TranDate)) AS MaxYear FROM Transactions;
-- ถ้า MinYear < 2400 AND MaxYear > 2400 = มี mixed

-- Step 2: Normalize ทุก record ให้เป็น ค.ศ.
UPDATE Transactions
SET TranDate = DateAdd('yyyy', -543, TranDate)
WHERE Year(TranDate) > 2400;

-- Step 3: หลัง migration — ลบ IIF guard ออกจาก SQL ทั้งหมด
-- แทนที่: IIF(Year(t.TranDate)>2400, DateAdd('yyyy',-543,t.TranDate), t.TranDate)
-- ด้วย:   t.TranDate

-- Step 4: ใน AccessDbProvider — ตรวจสอบ EnsureSchema ว่า
-- NormalizeGregorianDate ยังถูกเรียกตอน INSERT (ป้องกันย้อนกลับ)
```

**ข้อควรระวัง:**
- Backup ก่อน migration เสมอ
- ตรวจสอบว่า ReportEngine ทุก query ที่ใช้ IIF guard ได้รับการอัปเดต
- Access ไม่รองรับ transaction สำหรับ ALTER TABLE ได้ดี — ควร backup ก่อน

---

### C11. ไม่มีระบบ Login/Permission
**สถานะจริง: ยืนยัน — ไม่มีระบบ auth เลย**

ค้นหา `Login`, `UserID`, `Role`, `Permission` ทั้งโปรเจกต์ → พบเพียง:
```vb
' frmMain.vb บรรทัด 873 — placeholder เท่านั้น
MessageBox.Show("ระบบจะกลับสู่หน้าเข้าสู่ระบบ (ตัวอย่างเท่านั้น)" & vbCrLf &
                "นำฟอร์ม frmLogin มาเรียกในที่นี่ได้เลย", ...)
```

**ไม่มี:**
- ไม่มีตาราง Users/Roles ใน DB schema
- ไม่มี Login form
- ไม่มี permission check ใน任何形式
- ไม่มี session management

**ความเสี่ยง/ผลกระทบ:** **สูง** — ใครก็ตามที่เข้าถึงไฟล์ `.accdb` หรือรันโปรแกรมสามารถ:
- แก้ไข/ลบธุรกรรมทุกรายการ
- เปลี่ยนข้อมูลวัด (ชื่อวัด, เจ้าอาวาส, PromptPay)
- ลบประเภท/กองทุน/บัญชีธนาคาร
- สำรอง/กู้ค่าข้อมูล

**ข้อเสนอแนะ:**
- **ระดับต่ำสุด:** เพิ่ม password ให้ Access DB (OLEDB connection string)
- **ระดับกลาง:** เพิ่มตาราง Users + Login form + role-based permission (Admin/Accountant/Viewer)
- **ระดับสูง:** เพิ่ม audit trail (ดูข้อ C12)

---

### C12. Hard Delete — ไม่มี Audit Trail
**สถานะจริง: ยืนยัน — Hard delete ไม่มี log**

**โค้ดจริง (FrmTransactions.vb บรรทัด 589-595):**
```vb
For Each id In ids
    Dim oledbConn = TryCast(conn, OleDbConnection)
    Using cmd = oledbConn.CreateCommand()
        cmd.Transaction = TryCast(trans, OleDbTransaction)
        cmd.CommandText = "DELETE FROM Transactions WHERE ID = @id"  ' ← hard delete
        cmd.Parameters.AddWithValue("@id", id)
        cmd.ExecuteNonQuery()
    End Using
Next
```

**ไม่มี:**
- ไม่มีตาราง `DeletedTransactions` หรือ `AuditLog`
- ไม่มีบันทึกว่าใครลบ เมื่อไหร่ ทำไม
- ไม่มี soft-delete (IsDeleted flag)
- ไม่มีการบันทึกค่าเดิมก่อนแก้ไข

**ความเสี่ยง/ผลกระทบ:** **สูง** — ข้อมูลบัญชีวัดที่ลบแล้ว:
- หายเลย ไม่สามารถกู้คืนได้ (ยกเว้น restore จาก backup)
- ตรวจสอบย้อนหลังไม่ได้ว่าใครลบ/แก้ไขเมื่อไหร่
- ไม่สอดคล้องกับมาตรฐานบัญชี (ที่ต้อง audit trail)

**ข้อเสนอแนะ:**
- เพิ่มตาราง `AuditLog` (ID, TableName, RecordID, Action, OldValues, NewValues, UserName, Timestamp)
- ใช้ soft-delete (เพิ่ม `IsDeleted` + `DeletedAt` + `DeletedBy` columns)
- Trigger/update จาก code ทุกจุดที่มี INSERT/UPDATE/DELETE

---

### D13. TransactionGUID + SHA-256 Dedup
**สถานะจริง: ทำงานจริง — 2-tier deduplication**

**โค้ดจริง (FrmMultiMachineImport.vb บรรทัด ~686-720):**
```vb
' ลำดับ 1: TransactionGUID (แม่นยำที่สุด)
If row.TransactionGUID.Length > 0 Then
    If _dbGuidSet.Contains(row.TransactionGUID) OrElse seenGuids.Contains(row.TransactionGUID) Then
        row.IsDuplicateExact = True     ' ← skip อัตโนมัติ
    End If
End If

' ลำดับ 2: composite key (TranDate + Amount + Type + Description + MachineID)
If Not row.IsDuplicateExact Then
    Dim ck = BuildCompositeKey(row.TranDate.Date, row.TranType, row.Amount, row.Detail, row.MachineID)
    If _dbCompositeSet.Contains(ck) Then
        row.IsSuspectedDuplicate = True  ' ←ถาม user ก่อน skip
    End If
End If
```

**B Behavior:**
1. **GUID duplicate** (ซ้ำแน่นอน): skip เงียบๆ (ไม่ถาม)
2. **Composite key duplicate** (สงสัยซ้ำ): ถาม user ก่อน — "ข้าม" (แนะนำ) / "นำเข้าทั้งหมด" / "ยกเลิก"
3. **User ตัดสินใจ** แล้ว system บันทึก (`LogMergeOutcome`) + สำรอง DB อัตโนมัติก่อน merge

**ข้อสังเกต:**
- ใช้ `SourceMachineID` column (เพิ่มผ่าน migration) สำหรับ composite key
- `ImportHistory` table + SHA-256 file hash ป้องกันนำเข้าไฟล์เดิมซ้ำ

**ความเสี่ยง/ผลกระทบ:** ต่ำ — กลไก dedup ทำงานดี ครอบคลุมทั้ง GUID (แม่น) และ composite key (สงสัย)

---

### D14. Access .accdb Concurrent Access
**สถานะจริง: ไม่มี locking/concurrency guard สำหรับ network share**

**สิ่งที่พบ:**
- ฐานข้อมูลเป็น file-based `.accdb` ผ่าน OLEDB `Microsoft.ACE.OLEDB.12.0`
- ไม่มี `ConnectionTimeout` setting ที่ชัดเจน
- ไม่มี retry logic สำหรับ file lock
- ไม่มี error message ที่เป็นมิตรเมื่อถูก lock

**สิ่งที่ Access ทำตามธรรมชาติ:**
- Access `.accdb` ใช้ **file-level locking** — เครื่องแรกที่เปิด DB จะ lock
- เครื่องที่สองจะได้ "file already in use" error
- ไม่มี row-level locking สำหรับ multi-user

**สิ่งที่ขาดหายไป:**
- ไม่มี `Try-Catch` ที่จับ "file in use" error แล้วแจ้งเตือนเป็นภาษาไทย
- ไม่มี guidance ให้ผู้ใช้รู้ว่าต้อง copy DB ไปเครื่อง local ก่อนใช้
- ไม่มี retry mechanism

**ความเสี่ยง/ผลกระทบ:** กลาง — ถ้าผู้ใช้เปิด DB จาก network share พร้อมกัน 2 เครื่อง เครื่องที่สองจะ crash/error โดยไม่มี error message ที่ชัดเจน

**ข้อเสนอแนะ:**
- เพิ่ม error handler สำหรับ "file in use" → แจ้งเตือน "ฐานข้อมูลกำลังถูกใช้งานโดยเครื่องอื่น กรุณาลองใหม่อีกครั้ง"
- พิจารณาใช้ SQL Server (code มี `SqlServerDbProvider` แล้ว) สำหรับ multi-machine deployment

---

### E15. Backup Full System — Retention/Cleanup
**สถานะจริง: ไม่มี retention — สะสมไปเรื่อยๆ**

**โค้ดจริง (DatabaseBackupHelper.vb บรรทัด 18-60):**
```vb
Public Shared Function BackupFullSystem(Optional targetBaseDir As String = "", Optional isAuto As Boolean = False) As Boolean
    ...
    Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd_HHmmss")
    Dim backupSubFolderName As String = $"Backup_{timestamp}"
    Dim backupPath As String = Path.Combine(targetBaseDir, backupSubFolderName)
    Directory.CreateDirectory(backupPath)
    
    ' Copy DB + Receipts...
    File.Copy(dbSource, dbDestPath, True)
    CopyDirectory(receiptsSource, receiptsDest)
    ...
End Function
```

**ปัญหา:**
- ทุกครั้งที่ปิดโปรแกรม (`FormClosing`) → auto-backup → สร้าง folder ใหม่ (`Backup_20260819_143022`)
- ไม่มีการลบ backup เก่า
- Backup แต่ละชุด = DB file (หลาย MB) + Receipts folder (อาจหลาย GB)
- ถ้าปิดโปรแกรมวันละ 3-4 ครั้ง → ~90-120 backup sets/เดือน → กินพื้นที่มหาศาล

**ไม่มี:**
- ไม่มี max backup count
- ไม่มี max age (retention days)
- ไม่มี disk space check ก่อน backup
- ไม่มี cleanup on startup

**ความเสี่ยง/ผลกระทบ:** กลาง — พื้นที่ดิสก์จะเต็มในที่สุด (อาจเป็นสัปดาห์หรือเดือน ขึ้นอยู่กับขนาด DB + receipts)

**ข้อเสนอแนะ:**
- เพิ่ม retention policy: เก็บ backup ล่าสุด 10 ชุด (หรือ 30 วัน) แล้วลบเก่า
- เพิ่ม disk space check ก่อน backup: ถ้าเหลือ < 500MB ให้แจ้งเตือน
- เพิ่ม cleanup logic ใน `BackupFullSystem` หลังสร้าง backup ใหม่:
```vb
' Clean old backups — keep latest N
Dim allBackups = Directory.GetDirectories(targetBaseDir, "Backup_*")
    .OrderByDescending(Function(d) d).Skip(10).ToList()
For Each old In allBackups
    Directory.Delete(old, True)
Next
```

---

## ข้อเสนอแนะลำดับความสำคัญในการแก้ไข (Priority Roadmap)

### Phase 1: Security & Data Integrity (ควรทำทันที)
| ลำดับ | ข้อ | เหตุผล |
|-------|-----|--------|
| 1 | C12 — เพิ่ม Soft-delete + Audit Log | ข้อมูลบัญชีวัดที่ลบแล้วหายเลย ตรวจสอบย้อนหลังไม่ได้ ไม่สอดคล้องมาตรฐานบัญชี |
| 2 | C11 — เพิ่ม Login/Permission (ระดับต่ำสุด) | ใครก็แก้ไข/ลบข้อมูลได้โดยไม่ต้องยืนยันตัวตน |

### Phase 2: Performance & Reliability (ควรทำภายใน 1-2 สัปดาห์)
| ลำดับ | ข้อ | เหตุผล |
|-------|-----|--------|
| 3 | A2 — ปิด/guard Debug Harness | ทุกครั้งที่เปิด FrmTransactions = 2+ blocking HTTP requests + 5 queries เพิ่ม = UI ช้า ~500ms-2.5s |
| 4 | E15 — เพิ่ม Backup Retention | ไม่ลบ backup เก่า = พื้นที่ดิสก์จะเต็ม |
| 5 | D14 — เพิ่ม error handling สำหรับ concurrent access | ผู้ใช้ไม่เข้าใจ "file in use" error |

### Phase 3: Data Quality (ควรทำภายใน 1 เดือน)
| ลำดับ | ข้อ | เหตุผล |
|-------|-----|--------|
| 6 | A5a — แก้ fullName concat | ชื่อแสดงผิด format (ไม่มีช่องว่างระหว่าง title กับชื่อ) |
| 7 | A5b — เพิ่ม friendly FK-in-use message | ผู้ใช้ไทยไม่เข้าใจ OLEDB error |
| 8 | B10 — Migration พ.ศ./ค.ศ. | ทำให้ queries เร็วขึ้น (ลบ IIF guard) + ป้องกัน bugs ในอนาคต |

### Phase 4: UI Polish (ทำเมื่อมีเวลา)
| ลำดับ | ข้อ | เหตุผล |
|-------|-----|--------|
| 9 | A7b — Standardize AutoScaleMode | ป้องกัน layout เพี้ยนบนจอ HiDPI |
| 10 | A3 — แก้ emoji font rendering | เครื่องสำอาง ไม่กระทบการทำงาน |
| 11 | A4 — CSV parser รองรับ quoted field | ป้องกันการ import จังหวัดพังในอนาคต |

---

*สิ้นสุด Audit Report — ตรวจสอบวันที่ 19 สิงหาคม 2569*
*ตรวจสอบโดย: Buffy (Codebuff) — อ่านโค้ดจริงทั้งหมด 15 หัวข้อ เทียบกับ PROJECT_PROFILE_V7.md*
