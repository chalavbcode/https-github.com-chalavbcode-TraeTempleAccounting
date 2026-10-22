---
name: datagridview-column-setup
description: datagridview-column-setup skill
---

# datagridview-column-setup

ควบคุมการตั้งค่าคอลัมน์ DataGridView ใน WinForms app — ป้องกันการแก้ไขซ้ำแล้วซ้ำอีก (20+ commits แก้เรื่องเดิม) และรักษาความ consistent ระหว่าง forms

---

## ปัญหาที่เคยเจอ (Known Issues)

1. **Column Sizing Churn:** มี commit อย่างน้อย 20 ครั้งที่แก้ไข column sizing/widths/headers ใน FrmIncome, FrmExpense, FrmTransactions — มีการเปลี่ยนไปมาหลายครั้งระหว่าง `AllCells`, `AllCellsExceptHeader`, `DisplayedCells`, `Fill`
2. **Duplicated Configuration Code:** `ConfigureRecentGridColumns()` ถูก copy-paste ระหว่าง FrmIncome.vb และ FrmExpense.vb โดยเกือบ identically — เปลี่ยนที่หนึ่งแต่ไม่ได้เปลี่ยนอีกที่
3. **Inconsistent AutoSizeMode:** FrmTransactions ใช้ `AllCellsExceptHeader`, FrmIncome/FrmExpense ใช้ `DisplayedCells` + `Fill`, FrmReports/FrmMasterData ใช้ `Fill` — ไม่มี standard
4. **Column Height/Wrapping Issues:** `ColumnHeadersHeightSizeMode` เปลี่ยนไปมาระหว่าง `AutoSize` (FrmIncome, FrmExpense, FrmReports) และ `DisableResizing` (FrmTransactions) — ทำให้ header wrapping ไม่ consistent
5. **Missing Column Guard:** ไม่ check `Columns.Contains()` ก่อนเข้าถึงคอลัมน์ → runtime error ถ้า query schema เปลี่ยน
6. **FrmTransactions Grid Empty Bug (branch: modify-frmTransaction):** มี debug session ยาวนาน (10+ commits) เพื่อ debug ปัญหา grid ว่างเปล่า — root cause คือ date range query ใช้ Buddhist Era year ผิด + connection timing issue
7. **Compact Overview Mode Added (commit `30d24fb`, `907dd22`):** `frmMain` มี compact mode สำหรับ metric cards ที่กระทบ layout ของทุก forms

---

## กฎที่ต้องปฏิบัติ

### Rule 1: Guard Pattern for Column Access
**ห้ามเข้าถึงคอลัมน์โดยไม่ check Contains() ก่อนเสมอ**

```vb
' ✅ Correct:
If dgv.Columns.Contains("TranDate") Then
    dgv.Columns("TranDate").HeaderText = "วันที่ทำรายการ"
End If

' ❌ Wrong — runtime crash ถ้าคอลัมน์ไม่มี:
dgv.Columns("TranDate").HeaderText = "วันที่ทำรายการ"
```

### Rule 2: Standardize AutoSizeMode by Grid Type
กำหนด AutoSizeMode ตาม type ของ grid — ห้ามสุ่มเลือก

| Grid Type | AutoSizeMode | ColumnHeadersHeightSizeMode | Anchor |
|---|---|---|---|
| **Main Transaction Grid** (FrmTransactions) | `AllCellsExceptHeader` | `DisableResizing` (height=40) | Top,Left,Right |
| **Recent Items Grid** (FrmIncome/FrmExpense) | Mixed: fixed cols=`DisplayedCells`, text col=`Fill` | `AutoSize` | Top,Left,Right |
| **Report Grid** (FrmReports) | `Fill` | `AutoSize` | Top,Left,Right |
| **Master Data Grid** (FrmMasterData) | `Fill` | `AutoSize` | Top,Left,Right |

### Rule 3: Recent Grid Column Pattern (FrmIncome/FrmExpense)
สำหรับ `dgvRecentTransactions` ใน FrmIncome/FrmExpense — ใช้ pattern นี้เสมอ:

```vb
Private Sub ConfigureRecentGridColumns()
    If dgvRecentTransactions Is Nothing Then Return
    dgvRecentTransactions.ScrollBars = ScrollBars.Both

    ' Step 1: Set HeaderText and alignment
    If dgvRecentTransactions.Columns.Contains("TransactionDate") Then
        dgvRecentTransactions.Columns("TransactionDate").HeaderText = "วันที่ทำรายการ"
        dgvRecentTransactions.Columns("TransactionDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
    End If
    If dgvRecentTransactions.Columns.Contains("CategoryName") Then
        dgvRecentTransactions.Columns("CategoryName").HeaderText = "ประเภท"
    End If
    If dgvRecentTransactions.Columns.Contains("FundName") Then
        dgvRecentTransactions.Columns("FundName").HeaderText = "กองทุน"
    End If
    If dgvRecentTransactions.Columns.Contains("Amount") Then
        dgvRecentTransactions.Columns("Amount").HeaderText = "จำนวนเงิน"
        dgvRecentTransactions.Columns("Amount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
    End If
    If dgvRecentTransactions.Columns.Contains("Description") Then
        dgvRecentTransactions.Columns("Description").HeaderText = "รายละเอียด"
    End If

    ' Step 2: Set AutoSizeMode + MinimumWidth per column
    For Each col As DataGridViewColumn In dgvRecentTransactions.Columns
        Select Case col.Name
            Case "TransactionDate"
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                col.MinimumWidth = 90
            Case "CategoryName"
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                col.MinimumWidth = 110
            Case "FundName"
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                col.MinimumWidth = 120
            Case "Amount"
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                col.MinimumWidth = 90
            Case "Description"
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                col.MinimumWidth = 350
        End Select
    Next
End Sub
```

### Rule 4: Main Transaction Grid Pattern (FrmTransactions)
สำหรับ `dgvTransactions` ใน FrmTransactions — ใช้ pattern นี้:

```vb
' Step 1: Set global grid properties
dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader
dgvTransactions.AllowUserToResizeColumns = True
dgvTransactions.DefaultCellStyle.WrapMode = DataGridViewTriState.True
dgvTransactions.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
dgvTransactions.ScrollBars = ScrollBars.Both

' Step 2: Hide technical columns (keep for logic but invisible)
For Each colName In New String() {"TranType", "CategoryID", "FundID", "BankID", "ToFundID", "ToBankID", "ReceiptPath", "CreateDate"}
    If dgvTransactions.Columns.Contains(colName) Then
        dgvTransactions.Columns(colName).Visible = False
    End If
Next

' Step 3: Set display order (DisplayIndex)
Dim displayOrder As String() = New String() {
    "ID", "TranDate", "TranTypeDisplay", "CategoryName", "FundName",
    "Detail", "Amount", "HasReceiptDisplay", "BankName", "Note", "ToFundName", "ToBankName"
}
Dim nextIndex As Integer = 0
For Each colName In displayOrder
    If dgvTransactions.Columns.Contains(colName) Then
        dgvTransactions.Columns(colName).DisplayIndex = nextIndex
        nextIndex += 1
    End If
Next

' Step 4: Set HeaderText
Dim headers As New Dictionary(Of String, String) From {
    {"ID", "ลำดับ"}, {"TranDate", "วันที่"}, {"TranTypeDisplay", "ชนิด"},
    {"CategoryName", "ประเภท"}, {"FundName", "กองทุน"}, {"BankName", "ธนาคาร"},
    {"Detail", "รายละเอียด"}, {"Amount", "จำนวนเงิน"}, {"Note", "หมายเหตุ"},
    {"CreateDate", "วันที่บันทึก"}, {"ToFundName", "ไปยังกองทุน"}, {"ToBankName", "ไปยังธนาคาร"},
    {"ReceiptPath", "เอกสารแนบ"}
}
For Each pair In headers
    If dgvTransactions.Columns.Contains(pair.Key) Then
        dgvTransactions.Columns(pair.Key).HeaderText = pair.Value
    End If
Next

' Step 5: Set formats and alignment
If dgvTransactions.Columns.Contains("Amount") Then
    dgvTransactions.Columns("Amount").DefaultCellStyle.Format = "N2"
    dgvTransactions.Columns("Amount").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
End If

' Step 6: Set ReadOnly + background for display-only columns
For Each readOnlyName In New String() {"HasReceiptDisplay", "CategoryName", "FundName", "BankName"}
    If dgvTransactions.Columns.Contains(readOnlyName) Then
        dgvTransactions.Columns(readOnlyName).ReadOnly = True
        dgvTransactions.Columns(readOnlyName).DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
    End If
Next
```

### Rule 5: Column Height Constants
| Grid | ColumnHeadersHeight | ColumnHeadersHeightSizeMode |
|---|---|---|
| Main Transaction Grid | 40px | DisableResizing |
| Recent Items Grid | Auto (default) | AutoSize |
| Report Grid | Auto (default) | AutoSize |
| Master Data Grid | 34px | AutoSize |

### Rule 6: Anchor Rule for All DGVs
**ห้ามใส่ `AnchorStyles.Bottom` ใน DataGridView เด็ดขาด**

```vb
' ✅ Correct — Anchor Top only, fixed height ~10 rows (~290px)
dgv.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
dgv.Size = New Size(1157, 290)

' ❌ Wrong — grid will stretch down when form resizes
dgv.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
```

### Rule 7: Avoid Code Duplication
ถ้า FrmIncome และ FrmExpense มี `ConfigureRecentGridColumns()` ที่ identically → ย้ายไป shared module:

```vb
' DataGridViewHelper.vb (shared module)
Public Shared Sub ConfigureRecentGrid(dgv As DataGridView)
    ' ... shared logic
End Sub
```

หรือถ้ายังไม่พร้อม refactor → **ต้องแก้ไขทั้งสองจุดเสมอ** เมื่อเปลี่ยน column configuration

---

## Anti-Patterns to Avoid

| Anti-Pattern | ปัญหา | วิธีแก้ |
|---|---|---|
| เข้าถึงคอลัมน์โดยไม่ Contains() guard | Runtime crash ถ้า query schema เปลี่ยน | ใช้ `If dgv.Columns.Contains(col) Then` เสมอ |
| เปลี่ยน AutoSizeMode ไปมา 20+ commits | churn, regression | เลือก mode ตาม Rule 2 แล้ว stick กับมัน |
| Copy-paste ConfigureRecentGridColumns 2 จุด | เปลี่ยนที่หนึ่งลืมอีกที่ | Shared module หรือแก้ทั้งคู่เสมอ |
| ใส่ AnchorStyles.Bottom ใน DGV | Grid ยืดลงล่างเกินจำเป็น | ใช้ Top,Left,Right + fixed height |
| เปลี่ยน ColumnHeadersHeightSizeMode บ่อย | Header wrapping ไม่ consistent | ใช้ constants ตาม Rule 5 |
| ไม่ตั้ง MinimumWidth | คอลัมน์เล็กเกินไปซ่อนข้อมูล | ตั้ง MinimumWidth ทุกคอลัมน์ |

---

## Hotspot Files Reference

| File | Changes | สาเหตุ |
|---|---|---|
| `Forms/FrmIncome.Designer.vb` | 24 | Layout + DGV column changes |
| `Forms/FrmIncome.vb` | 21 | ConfigureRecentGridColumns, layout |
| `Forms/FrmTransactions.vb` | 17 | Column ordering, sizing, headers |
| `Forms/FrmExpense.vb` | 16 | ConfigureRecentGridColumns (duplicate) |
| `Forms/FrmExpense.Designer.vb` | 15 | Layout + DGV column changes |

---

## Git Commit Evidence

| Commit | ปัญหาที่แก้ |
|---|---|
| `98deb36` | Improve DataGridView column sizing and header wrapping |
| `fd06e1e` | Refine DataGridView column sizing and widths |
| `c782a83` | Improve DataGridView column sizing and order |
| `d5f8823` | Hide "CreateDate" column in DataGridView |
| `bd0e999` | Update DataGridView headers and column order |
| `ddbc56d` | Refactor DataGridView column initialization inline |
| `e5ce3f0` | Refactor DataGridView columns and update data loading |
| `4991e5e` | Refactor Income/Expense forms layout and grid columns |
| `826721f` | Refactor summary labels and layout in FrmExpense/FrmIncome |
| `a514717` | Improve DataGridView column order and sizing in transactions |
