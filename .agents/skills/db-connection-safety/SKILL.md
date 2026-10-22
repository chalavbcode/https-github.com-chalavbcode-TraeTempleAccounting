---
name: db-connection-safety
description: db-connection-safety skill
---

# db-connection-safety

ควบคุมการจัดการฐานข้อมูล MS Access (OleDb) — connection lifecycle, parameterized queries, transaction safety, SQL string construction, และ defensive null handling

---

## ปัญหาที่เคยเจอ (Known Issues)

1. **SQL String Concatenation for IDs (SQL Injection Risk):** `FrmIncome.vb` L60, `FrmExpense.vb` L56 ใช้ `"SELECT * FROM Transactions WHERE ID = " & id` แทน parameterized query — ไม่ปลอดภัยและไม่ consistent กับ pattern อื่น
2. **DELETE + INSERT Without Transaction (Data Loss):** `FrmTempleSetting.vb` ทำ DELETE แล้ว INSERT ใน TempleSetting โดยไม่มี transaction — ถ้า INSERT fail ข้อมูลหายถาวร (fixed in `05d5f18`)
3. **OleDbConnection Not Using Using Pattern:** บางจุดไม่ wrap connection ใน `Using` block → connection leak ถ้า exception เกิด
4. **Wildcard Character Mismatch:** MS Access ใช้ `*` ใน LIKE แต่ code บางจุดใช้ `%` (MySQL style) — แก้ไขใน commit `fbe9e4f` (main) และ `97109a9`
5. **Buddhist Era Date in SQL:** ค่าวันที่จาก MS Access บางครั้งเป็น พ.ศ. (>2400) — ต้อง normalize ก่อนใช้ใน UI (covered in access-date-safe skill, but affects connection layer too)
6. **Schema Migration Race Condition:** `EnsureSchema()` เรียกใน Form_Load ทุกฟอร์ม — ถ้าเปิด 2 ฟอร์มพร้อมกัน อาจทำ ALTER TABLE ซ้ำกัน

---

## กฎที่ต้องปฏิบัติ

### Rule 1: Always Use Parameterized Queries for IDs
**ห้าม concat ID ลงใน SQL string เด็ดขาด**

```vb
' ❌ Wrong — SQL injection risk + inconsistent:
Dim dt = Db.GetTable(conn, "SELECT * FROM Transactions WHERE ID = " & id)

' ✅ Correct — parameterized:
Dim dt = Db.GetTable(conn, "SELECT * FROM Transactions WHERE ID = @id",
    New Tuple(Of String, Object)("@id", id))
```

**Exception:** `Db.AccessDateLiteral()` เป็น safe helper ที่ใช้ `InvariantCulture` format — ใช้ได้

### Rule 2: Connection Lifecycle with Using
**ต้อง wrap OleDbConnection ใน `Using` เสมอ**

```vb
' ✅ Correct:
Using conn = Db.OpenConn()
    Dim dt = Db.GetTable(conn, "SELECT ...")
    ' ... use dt
End Using  ' ← auto-dispose connection

' ❌ Wrong — connection leak:
Dim conn = Db.OpenConn()
Dim dt = Db.GetTable(conn, "SELECT ...")
' forgot to close/dispose
```

### Rule 3: Transaction Pattern for Multi-Statement Operations
เมื่อทำ DELETE แล้ว INSERT/UPDATE ใน table เดียวกัน ต้อง wrap ด้วย `OleDbTransaction`:

```vb
Using conn = Db.OpenConn()
    Using trans = conn.BeginTransaction()
        Try
            Dim oledbConn = TryCast(conn, OleDbConnection)
            Using cmd = oledbConn.CreateCommand()
                cmd.Transaction = TryCast(trans, OleDbTransaction)

                ' Step 1: DELETE
                cmd.CommandText = "DELETE FROM TempleSetting"
                cmd.ExecuteNonQuery()

                ' Step 2: INSERT
                cmd.CommandText = "INSERT INTO TempleSetting (...) VALUES (...)"
                cmd.ExecuteNonQuery()
            End Using

            trans.Commit()
        Catch ex As Exception
            trans.Rollback()
            MessageBox.Show("บันทึกไม่สำเร็จ: " & ex.Message, "ผิดพลาด",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
    End Using
End Using
```

**Location:** `FrmTempleSetting.vb` L315-344, `FrmMultiMachineImport.vb` L978-1081, `FrmTransactions.vb` L456-469

### Rule 4: Access LIKE Wildcard Character
MS Access ใช้ `*` แทน `%` ใน LIKE clauses:

```vb
' ❌ Wrong — Access ไม่เข้าใจ %:
"WHERE Detail LIKE '%" & searchTerm & "%'"

' ✅ Correct — ใช้ *:
"WHERE Detail LIKE '*" & searchTerm & "*'"

' ✅ Better — parameterized with Escape:
"WHERE Detail LIKE '*" & EscapeAccessLike(searchTerm) & "*' Escape '\\'"
```

**Wildcard escape function:**
```vb
Public Shared Function EscapeAccessLike(input As String) As String
    ' Escape special LIKE characters: [ ] * #
    Return input.Replace("[", "[[]").Replace("*", "[*]").Replace("#", "[#]")
End Function
```

**Git Evidence:** commit `fbe9e4f` — "Use '%' as wildcard in OleDb LIKE queries instead of '*'" (but note: this was actually fixing the REVERSE — code was using `%` when it should use `*` for Access)

### Rule 5: Db.GetTable / Db.ExecuteNonQuery / Db.DbScalar Usage
ใช้ helper methods จาก `Database.vb` เสมอ แทน OleDbCommand ตรง:

```vb
' For reading data:
Dim dt = Db.GetTable(conn, sqlString, param1, param2, ...)

' For scalar values:
Dim count = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM ...", params))
Dim total = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) FROM ...", params))

' For non-query:
Db.ExecuteNonQuery(conn, "UPDATE ...", param1, param2, ...)
```

### Rule 6: Null-Safe Value Reading
Access คืน `DBNull.Value` บ่อย — ต้อง check เสมอ:

```vb
' ✅ Correct null handling patterns:
Dim amount = Db.ToDecimalOrZero(dr("Amount"))           ' NULL → 0
Dim name = If(dr("AbbotName") Is DBNull.Value, "", dr("AbbotName").ToString())  ' NULL → ""
Dim bankId = If(dr("BankID") Is DBNull.Value, Nothing, CInt(dr("BankID")))      ' NULL → Nothing (Integer?)

' ❌ Wrong — NullReferenceException:
Dim amount = CDec(dr("Amount"))  ' crash if NULL
Dim name = dr("AbbotName").ToString()  ' crash if NULL
```

### Rule 7: EnsureSchema Idempotency
`EnsureSchema()` ต้อง idempotent (เรียกซ้ำได้โดยไม่ error):

```vb
' ✅ Pattern: check column exists before ALTER TABLE
If Not ColumnExists(conn, "Transactions", "ToFundID") Then
    ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN ToFundID INTEGER")
End If
```

**ห้ามใช้:** `CREATE TABLE` โดยไม่ check `IF NOT EXISTS` — ถ้า table มีอยู่แล้วจะ error

### Rule 8: Access Date Literal in SQL
เมื่อใส่วันที่ใน SQL ต้องใช้ `Db.AccessDateLiteral()`:

```vb
' ✅ Correct:
"WHERE TranDate >= " & Db.AccessDateLiteral(firstOfMonth) & " " &
"AND TranDate < " & Db.AccessDateLiteral(firstOfNextMonth)

' ❌ Wrong — ไม่ normalize, format ผิด:
"WHERE TranDate >= #" & dateValue.ToString("yyyy-MM-dd") & "#"
"WHERE TranDate >= '" & dateValue.ToString() & "'"
```

### Rule 9: Provider Abstraction (Access vs SQL Server)
โปรเจกต์มี `IDbProvider` interface สำหรับ abstract database:

```vb
' Access (default):
Dim provider As IDbProvider = New AccessDbProvider()

' SQL Server (future):
Dim provider As IDbProvider = New SqlServerDbProvider()
```

**อย่า hardcode** OleDb-specific syntax ใน form code — ใช้ `Db.*` helpers ที่ abstract ผ่าน provider

---

## Anti-Patterns to Avoid

| Anti-Pattern | ปัญหา | วิธีแก้ |
|---|---|---|
| `"SELECT ... WHERE ID = " & id` | SQL injection, inconsistent | ใช้ parameterized query |
| `conn = Db.OpenConn()` โดยไม่มี `Using` | Connection leak | ใช้ `Using conn = Db.OpenConn()` เสมอ |
| DELETE แล้ว INSERT โดยไม่มี Transaction | Data loss ถ้า INSERT fail | Wrap ด้วย `BeginTransaction` |
| ใช้ `%` ใน Access LIKE | Wildcard ไม่ทำงาน | ใช้ `*` สำหรับ MS Access |
| `CDec(dr("Amount"))` โดยไม่ check NULL | NullReferenceException | ใช้ `Db.ToDecimalOrZero()` |
| `EnsureSchema()` ไม่ idempotent | Error ถ้าเรียกซ้ำ | ใช้ `IF NOT EXISTS` / `ColumnExists()` |
| ไม่ Dispose OleDbCommand | Memory leak | ใช้ `Using cmd = ...` |
| Hardcode OleDb syntax ใน form code | ไม่ portable ไป SQL Server | ใช้ `Db.*` helpers |

---

## Transaction Safety Matrix

| Operation | Has Transaction? | Risk if No Transaction |
|---|---|---|
| FrmTempleSetting SAVE (DELETE + INSERT) | ✅ `05d5f18` | Data loss — TempleSetting ว่างเปล่า |
| FrmTransactions DELETE (multi-row) | ✅ L456-469 | Partial delete |
| FrmMultiMachineImport (bulk import) | ✅ L978-1081 | Partial import, duplicate rows |
| FrmIncome SAVE (single INSERT) | ❌ (not needed) | Low risk — single statement |
| FrmExpense SAVE (single INSERT) | ❌ (not needed) | Low risk — single statement |
| ExcelTransactionImporter (bulk) | ❌ (needs review) | Partial import possible |
| DatabaseBackupHelper | N/A (file copy) | N/A |

---

## Git Commit Evidence

| Commit | ปัญหาที่แก้ |
|---|---|
| `05d5f18` | Wrap temple settings save in DB transaction for safety |
| `fbe9e4f` | Use '%' as wildcard in OleDb LIKE queries instead of '*' |
| `9cb0411` | Use parameterized queries; update button FlatStyle |
| `55bbb95` | Refactor date handling in SQL queries for transactions |
| `33eac0e` | Refactor date handling in SQL queries for transactions |
| `97109a9` | Simplify validation for income/expense transactions |
| `a26397e` | Ensure ToFundID/ToBankID set NULL for income/expense |
| `4af2ff1` | Ensure ToFundID/ToBankID set NULL for income/expense |
| `397d08e` | Add provider abstraction for Access and SQL Server support |
| `1ff2766` | Add multi-channel income support and refactor data access |

---

## Hotspot Files Reference

| File | Changes | สาเหตุ |
|---|---|---|
| `Database.vb` | 23 | Core DB helpers, schema migration |
| `Services/AccessDbProvider.vb` | N/A | Access-specific provider |
| `Services/AccessTransactionRepository.vb` | N/A | Transaction INSERT/UPDATE |
| `Forms/FrmTransactions.vb` | 86 | Complex queries, search, filter |
| `Forms/FrmIncome.vb` | 62 | Save with receipt, parameterized |
| `Forms/FrmExpense.vb` | 53 | Save with receipt (duplicate pattern) |
| `Forms/FrmTempleSetting.vb` | 25 | DELETE+INSERT transaction safety |
| `Forms/FrmMultiMachineImport.vb` | 7 | Bulk import with dedup |
| `DatabaseBackupHelper.vb` | 4 | Backup file operations |
| `ExcelTransactionImporter.vb` | 3 | Bulk import from Excel |
