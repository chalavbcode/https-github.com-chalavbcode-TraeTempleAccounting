---
name: vb-audit-checker
description: vb-audit-checker skill
---

# vb-audit-checker

ตรวจทานบั๊กและ code quality ใน VB.NET WinForms project ที่ใช้ MS Access — ครอบคลุม dead code, transaction safety, guard patterns, debug harness, และ data integrity

---

## ปัญหาที่เคยเจอ (Known Issues)

1. **A1 — Dead Code:** `SaveSelectedRowEdits()` ถูกสร้างแต่ไม่ได้ wire เข้า `btnEdit_Click` — ทำให้การแก้ไข Transfer บันทึกไม่ได้
2. **A2 — Debug Harness ค้าง:** `DebugReport()` region ใน `FrmTransactions.vb` ส่ง HTTP POST 500ms ทุกครั้งที่ `LoadData` ถูกเรียก — ทำให้ฟอร์มช้า ~3 วินาที
3. **P6 — DELETE+INSERT ไม่มี Transaction:** `FrmTempleSetting` ทำ `DELETE FROM TempleSetting` แล้ว `INSERT` ถ้า INSERT fail ข้อมูลทั้งหมดหายถาวร
4. **A6 — Legacy Column Query:** `LoadMonkSample()` อ่านคอลัมน์ชื่อตรงจาก TempleSetting แทนที่จะ JOIN Personnel
5. **P6a — FrmTempleSetting DB Transaction Missing (fixed in 05d5f18):** `FrmTempleSetting.btnSave_Click` ทำ DELETE แล้ว INSERT ใน TempleSetting โดยไม่มี transaction wrap — ถ้า INSERT fail ข้อมูลทั้งหมดหายถาวร
6. **A7 — Duplicate ConfigureRecentGridColumns:** `FrmIncome.vb` และ `FrmExpense.vb` มี `ConfigureRecentGridColumns()` ที่ identically — เปลี่ยนที่หนึ่งลืมอีกที่ (see datagridview-column-setup skill)
7. **A8 — SQL String Concatenation for IDs:** `FrmIncome.vb` L60, `FrmExpense.vb` L56 ใช้ `"SELECT * FROM Transactions WHERE ID = " & id` แทน parameterized query — SQL injection risk (see db-connection-safety skill)
8. **A9 — FrmExpense Missing TransactionService:** `FrmIncome` ใช้ `_txnService As TransactionService` แต่ `FrmExpense` ใช้ raw `Db.ExecuteNonQuery`/`Db.InsertAndGetId` — data access pattern ไม่ consistent (see form-duplicate-code skill)
9. **A10 — Debug Logging Residue Across Codebase:** มี `System.Diagnostics.Debug.WriteLine` หลายสิบจุดใน `IncomeExpenseReport.vb`, `FrmTransactions.vb`, `ReportEngine.vb` — ต้องลบก่อน release (see report-rendering-layout skill Rule 8)

---

## กฎที่ต้องปฏิบัติ

### Rule 1: Active Wiring Check
เวลาแก้ไขฟอร์ม ให้เช็กเสมอว่า **อีเวนต์ปุ่มมีการเรียกฟังก์ชันจริง** — ไม่ปล่อยให้เป็น Dead Code

```vb
' ✅ Correct: btnEdit_Click wires SaveSelectedRowEdits
Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
    If _isEditing Then
        SaveSelectedRowEdits()  ' ← actually called
        Return
    End If
    ' ...
```

**Pattern ที่ต้องระวัง:**
- ฟังก์ชัน `SaveXxx()`, `UpdateXxx()`, `DeleteXxx()` ที่สร้างใหม่ → ตรวจสอบว่าถูกเรียกจาก UI event จริง
- State variables เช่น `_isEditing` → ต้องมี guard check ใน event handler ที่เกี่ยวข้อง

### Rule 2: Transaction Wrap Pattern (DELETE + INSERT)
เมื่อทำ DELETE แล้ว INSERT ใน table เดียวกัน ต้อง wrap ด้วย Transaction เสมอ

**Standard Pattern:**
```vb
Using trans = conn.BeginTransaction()
    Try
        Dim oledbConn = TryCast(conn, OleDbConnection)
        Using cmd = oledbConn.CreateCommand()
            cmd.Transaction = TryCast(trans, OleDbTransaction)
            cmd.CommandText = "DELETE FROM TempleSetting"
            cmd.ExecuteNonQuery()
        End Using

        Db.InsertAndGetId(conn, "INSERT INTO ...", ...)

        trans.Commit()
    Catch ex As Exception
        trans.Rollback()
        MessageBox.Show("บันทึกไม่สำเร็จ: " & ex.Message, "ผิดพลาด",
                         MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return
    End Try
End Using
```

**Locations ที่ใช้ Pattern นี้:**
- `FrmTransactions.vb` L456-469 (`btnDelete_Click` — multi-row delete)
- `FrmTempleSetting.vb` L315-344 (`btnSave_Click` — DELETE + INSERT) — **fixed in commit 05d5f18**
- `FrmMultiMachineImport.vb` L978-1081 (bulk import with rollback)

**Git Evidence:**
- `05d5f18` — "Wrap temple settings save in DB transaction for safety" — FrmTempleSetting was doing DELETE + INSERT without transaction wrap, risking data loss on INSERT failure

**กฎ:** ห้ามมี `DELETE` ตามด้วย `INSERT` โดยไม่มี `BeginTransaction()` / `Commit()` / `Rollback()` คู่กัน

### Rule 3: Personnel Join Pattern
การดึงข้อมูลเจ้าอาวาส/ไวยาวัจกร/ผู้ทำบัญชี ต้องใช้ **LEFT JOIN Personnel** ผ่าน PersonnelID เสมอ

```vb
' ✅ Correct pattern:
"SELECT TOP 1 ts.TempleName, " &
"a.FullName AS AbbotName, w.FullName AS WaiyawatName, b.FullName AS BookkeeperName " &
"FROM ((TempleSetting ts " &
"LEFT JOIN Personnel a ON ts.AbbotPersonnelID = a.PersonnelID) " &
"LEFT JOIN Personnel w ON ts.WaiyawatPersonnelID = w.PersonnelID) " &
"LEFT JOIN Personnel b ON ts.BookkeeperPersonnelID = b.PersonnelID " &
"ORDER BY ts.ID DESC"
```

**ห้ามทำ:**
```vb
' ❌ Legacy pattern — TempleSetting ไม่มีคอลัมน์ชื่อ anymore
"SELECT AbbotName, WaiyawatName, BookkeeperName FROM TempleSetting"
```

**Guard สำหรับ NULL:**
```vb
If r("AbbotName") Is DBNull.Value Then
    lblAbbot.Text = "-"
Else
    lblAbbot.Text = r("AbbotName").ToString()
End If
```

### Rule 4: Debug Code Cleanup
**ห้ามมี debug harness ที่ส่ง HTTP/ network request ใน production code**

สัญญาณเตือนที่ต้องตรวจ:
- `Imports System.Net.Http` / `Imports System.Text.Json`
- `HttpClient` / POST request ใน form code
- `DebugReport()` / `DebugSessionId` / `DebugRunId` constants
- `FindUpwards()` functions ที่ค้นหา `.env` files
- Region ที่ชื่อมี "debug" หรือ "test"

ถ้าพบ → ลบ debug region ทั้งหมด + call sites ทั้งหมด + Imports ที่เป็น orphan

### Rule 5: State Guard Pattern (Edit Mode)
เมื่อมี edit mode (`_isEditing`), ต้องมี guard check ใน event handler ที่เกี่ยวข้องทุกตัว:

```vb
Private _isEditing As Boolean = False

' Guard ใน btnEdit_Click:
If _isEditing Then
    SaveSelectedRowEdits()
    Return
End If

' Guard ใน btnRefresh:
If _isEditing Then
    MessageBox.Show("กรุณาบันทึกหรือยกเลิกการแก้ไขก่อน") : Return
End If

' Guard ใน btnDelete:
If _isEditing Then
    MessageBox.Show("กรุณาบันทึกหรือยกเลิกการแก้ไขก่อนลบรายการ") : Return
End If
```

### Rule 6: Receipt File Cleanup on Delete
เมื่อลบ transaction ที่มี receipt image ต้องลบไฟล์รูปจริงด้วย ไม่ใช่แค่ DB record:

```vb
' หลัง DELETE จาก DB:
For Each fileName In filesToDelete
    Dim fullPath = ReceiptImageHelper.ResolveReceiptPath(fileName)
    If Not String.IsNullOrEmpty(fullPath) Then File.Delete(fullPath)
Next
```

ใช้ `ReceiptImageHelper.ResolveReceiptPath()` ไม่ใช่ `File.Delete(path)` ตรงๆ เพราะต้องค้นหาหลายโฟลเดอร์

### Rule 7: ClearTemplateInfoCache After Settings Change
เมื่อบันทึกข้อมูลวัด (TempleSetting) ต้องเรียก `ReportEngine.ClearTemplateInfoCache()` เพื่อให้ report ใช้ข้อมูลใหม่:

```vb
MessageBox.Show("✅ บันทึกข้อมูลวัดสำเร็จ!")
ReportEngine.ClearTemplateInfoCache()
LoadPersonnelGrid()
txtTempleCode.Focus()
```

---

## Anti-Patterns to Avoid

| Anti-Pattern | ปัญหา | วิธีแก้ |
|---|---|---|
| สร้างฟังก์ชันแต่ไม่ wire เข้า UI event | Dead code, feature ไม่ทำงาน | ตรวจสอบ event wiring ทุกครั้ง |
| DELETE + INSERT โดยไม่มี Transaction | Data loss ถ้า INSERT fail | Wrap ด้วย BeginTransaction |
| อ่านคอลัมน์ชื่อตรงจาก TempleSetting | Legacy columns ถูก DROP แล้ว | LEFT JOIN Personnel ผ่าน PersonnelID |
| Debug HTTP POST ใน form code | ทำให้ form ช้า ~500ms ต่อจุด | ลบ debug harness ทั้งหมด |
| ไม่มี guard check สำหรับ edit mode | ผู้ใช้แก้ไขค้างอยู่แล้ว操作 อื่น | เพิ่ม `_isEditing` guard ทุก event |
| ไม่ลบ receipt image ตอนลบ transaction | orphan files กินพื้นที่ | ใช้ `ReceiptImageHelper.ResolveReceiptPath()` |
| Duplicated ConfigureRecentGridColumns ใน 2 forms | เปลี่ยนที่หนึ่งลืมอีกที่ | Shared module หรือแก้ทั้งคู่เสมอ (see datagridview-column-setup skill) |
| เปลี่ยน signature block constants โดยไม่ sync จุดอื่น | Signature ล้นขอบ | Sync ค่าทั้ง ReportEngine + IncomeExpenseReport (see report-rendering-layout skill) |
| SQL string concat แทน parameterized query | SQL injection, inconsistent | ใช้ parameterized queries เสมอ (see db-connection-safety skill) |
| FrmExpense ใช้ raw SQL แทน TransactionService | Data access pattern ไม่ consistent | Refactor ให้ใช้ TransactionService (see form-duplicate-code skill) |
| Debug.WriteLine ค้างใน production code | Report ช้า, output เกินจำเป็น | ลบ debug logging ก่อน release |

---

## 3-Tier Backup Strategy Reference

ระบบ backup แบบ auto ตอนปิดโปรแกรมใช้ 3 tiers:

| Tier | ชื่อไฟล์ | Retention | Trigger |
|---|---|---|---|
| Weekly | `Backup_Weekly_{ISOYear}_W{weekNum:D2}.accdb` | FIFO ไม่เกิน 8 ไฟล์ | ทุกครั้งที่ปิดโปรแกรม |
| Monthly | `Backup_Monthly_{yyyy}_{MM}.accdb` | FIFO ไม่เกิน 15 ไฟล์ | ทุกครั้งที่ปิดโปรแกรม |
| Yearly | `Backup_Yearly_{yyyy}.accdb` | FIFO ไม่เกิน 5 ไฟล์ | เฉพาะเดือนธันวาคม |

**Location:** `DatabaseBackupHelper.vb` — `RunTieredBackup()`, `RunWeeklyBackup()`, `RunMonthlyBackup()`, `RunYearlyBackup()`

**ไม่กระทบ:** Manual backup (`BackupFullSystem`) และ safety backup ก่อน import/merge

---

## Code Quality Checklist

- [ ] ไม่มี hardcoded year ใน SQL
- [ ] DELETE + INSERT มี Transaction wrap
- [ ] Personnel queries ใช้ LEFT JOIN (ไม่ใช่คอลัมน์ชื่อตรง)
- [ ] ไม่มี debug harness ใน production code
- [ ] Edit mode state มี guard check ในทุก event
- [ ] Receipt files ถูกลบเมื่อ delete transaction
- [ ] TempleSetting save เรียก `ClearTemplateInfoCache()`
- [ ] ไม่มี orphan Imports หลังลบ code
- [ ] Duplicated column config code ถูก sync หรือ shared
- [ ] Signature block constants sync ระหว่าง ReportEngine + IncomeExpenseReport
- [ ] ไม่มี Debug.WriteLine ใน production report code
- [ ] ไม่มี SQL string concat สำหรับ IDs (ใช้ parameterized queries)
- [ ] FrmExpense ใช้ TransactionService เหมือน FrmIncome
- [ ] Receipt clipboard ถูก clear หลังบันทึก
- [ ] clipboardImage ถูก dispose ทุกครั้งที่ clear
- [ ] Build succeeds: `dotnet build -c Release` → 0 Warning(s) / 0 Error(s)
