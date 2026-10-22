---
name: access-date-safe
description: access-date-safe skill
---

# access-date-safe

ป้องกันบั๊กที่เกี่ยวกับการจัดการวันที่ในระบบ MS Access — Thai Buddhist Era vs. Gregorian, date range queries, และ Access date literal format

---

## ปัญหาที่เคยเจอ (Known Issues)

1. **B8:** Hardcoded `YEAR(TranDate) = 2026` ใน SQL query (FrmIncome, FrmExpense, frmMain) — ทำให้ระบบคำนวณปีถัดไปผิด
2. **Thai locale Buddhist Era bug:** ตัว format specifier `{d:yyyy}` บน Windows ที่ locale เป็น `th-TH` จะคืนค่าปี พ.ศ. (เช่น 2569) แทน ค.ศ. (2026) — ทำให้ชื่อไฟล์ backup ผิดรูปแบบ
3. **SQL-side Buddhist Era filter:** เดิมใช้ `IIF(Year(x)>2400, DateAdd('yyyy',-543,x), x)` ซึ่งทำงานแต่ต้องเข้าใจว่าเป็นการ convert ใน SQL ไม่ใช่ใน VB

---

## กฎที่ต้องปฏิบัติ

### Rule 1: No Hardcoded Year in SQL
ห้ามเขียนปี ค.ศ. ตายตัวใน SQL Query เด็ดขาด

**ห้ามทำ:**
```sql
WHERE YEAR(TranDate) = 2026
WHERE MONTH(TranDate) = MONTH(Date()) AND YEAR(TranDate) = YEAR(Date())
```

**ต้องทำ:**
```vb
Dim firstOfMonth As Date = New Date(Date.Today.Year, Date.Today.Month, 1)
Dim firstOfNextMonth As Date = firstOfMonth.AddMonths(1)
' ...
"AND TranDate >= " & Db.AccessDateLiteral(firstOfMonth) & " " &
"AND TranDate < " & Db.AccessDateLiteral(firstOfNextMonth)
```

ใช้ **half-open range** (`>= firstOfMonth AND < firstOfNextMonth`) เสมอ — ไม่ใช่ `MONTH()/YEAR()` functions

### Rule 2: Access Date Literal Format
เมื่อเขียน Query ที่มีวันที่ ต้องใช้ `Db.AccessDateLiteral()` เสมอ

- **VB side:** `Db.AccessDateLiteral(dateValue)` — เรียก `NormalizeGregorianDate` ภายใน แล้ว format เป็น `#yyyy-MM-dd HH:mm:ss#` ด้วย `InvariantCulture`
- **SQL side (ถ้าหลีกเลี่ยงไม่ได้):** ใช้ `IIF(Year(x)>2400, DateAdd('yyyy',-543,x), x)` ใน SQL expression

**ห้ามใช้:**
```vb
"#" & dateValue.ToString("yyyy-MM-dd") & "#"   ' ไม่ normalize
"'" & dateValue.ToString("yyyy-MM-dd") & "'"    ' wrong delimiter for Access
```

### Rule 3: NormalizeGregorianDate on Read
เมื่ออ่านวันที่จาก MS Access กลับมาแสดงผล ต้องเรียก `Db.NormalizeGregorianDate()` เสมอ

```vb
dtpDate.Value = Db.NormalizeGregorianDate(Convert.ToDateTime(dr("TranDate")))
```

`NormalizeGregorianDate` จะตรวจว่าถ้า `Year > 2400` ให้ subtract 543 → ได้ค่า Gregorian สำหรับ UI

### Rule 4: Gregorian Year for File Names and SQL (Never Thai Locale Format)
เมื่อสร้างชื่อไฟล์หรือ SQL string ที่มีปี ต้องใช้ Gregorian year เสมอ ห้ามใช้ format specifier ที่อาจคืนค่า พ.ศ.

**ห้ามทำ (Thai locale 会产生 พ.ศ.):**
```vb
Dim fileName = $"Backup_{d:yyyy}_{d:MM}.accdb"       ' ❌
Dim fileName = $"Backup_{d.ToString("yyyy")}.accdb"   ' ❌
```

**ต้องทำ (ใช้ .Year / .Month properties):**
```vb
Dim fileName = $"Backup_{d.Year}_{d.Month:D2}.accdb"  ' ✅ Always Gregorian
Dim isoYear = ISOWeek.GetYear(d)                       ' ✅ ISO year is Gregorian
Dim weekNum = ISOWeek.GetWeekOfYear(d)                 ' ✅
```

### Rule 5: Display vs. Backend Split
- **UI display:** สามารถแสดงปี พ.ศ. ให้ผู้ใช้ดูง่าย
- **Backend SQL:** ต้องเป็น ค.ศ. เท่านั้น — ใช้ `Db.AccessDateLiteral()` หรือ `Db.NormalizeGregorianDate()` เป็นตัวกลางเสมอ

### Rule 6: "This Month" Pattern (Standard Template)
สำหรับ query "เดือนนี้" ให้ใช้ template นี้เสมอ:

```vb
Dim firstOfMonth As Date = New Date(Date.Today.Year, Date.Today.Month, 1)
Dim firstOfNextMonth As Date = firstOfMonth.AddMonths(1)

' แล้วใช้ทั้ง 3 queries:
' SELECT COUNT(*) ... WHERE TranType = 'Income' AND TranDate >= [literal1] AND TranDate < [literal2]
' SELECT SUM(Amount) ... WHERE TranType = 'Income' AND TranDate >= [literal1] AND TranDate < [literal2]
' SELECT SUM(Amount) ... WHERE TranType = 'Expense' AND TranDate >= [literal1] AND TranDate < [literal2]
```

 Deklarasikan `firstOfMonth`/`firstOfNextMonth` ครั้งเดียวแล้วใช้ซ้ำ — ไม่ต้องสร้างทุก query

---

## Anti-Patterns to Avoid

| Anti-Pattern | ปัญหา | วิธีแก้ |
|---|---|---|
| `YEAR(TranDate) = 2026` | Hardcoded year breaks next year | ใช้ half-open date range |
| `MONTH(TranDate) = MONTH(Date())` | ใช้ function บน column → index unusable | ใช้ `TranDate >=` range |
| `{$d:yyyy}` in file names | Returns พ.ศ. on Thai locale Windows | ใช้ `d.Year` property |
| `DateSerial()` ที่รับ year จาก DB | อาจได้ค่า พ.ศ. จาก Buddhist Era column | Normalize ก่อนใช้ |
| ไม่เรียก `NormalizeGregorianDate` ตอนอ่าน | DateTimePicker แสดง year ผิด | เรียกเสมอตอน load |

---

## References in Codebase

| Location | ใช้ Rule ไหน |
|---|---|
| `frmMain.vb` L326-327 | Rule 1 + Rule 6 (firstOfMonth template) |
| `Forms/FrmIncome.vb` L255-256 | Rule 1 + Rule 6 |
| `Forms/FrmExpense.vb` L210-211 | Rule 1 + Rule 6 |
| `DatabaseBackupHelper.vb` L284-300 | Rule 4 (Gregorian year in file names) |
| `Forms/FrmTransactions.vb` L119 | Rule 2 (SQL-side Buddhist Era IIF) |
| `Forms/FrmReports.vb` L49-50 | Rule 6 (dynamic month start) |
| `ExcelTransactionImporter.vb` L123 | Rule 3 (NormalizeGregorianDate on read) |
