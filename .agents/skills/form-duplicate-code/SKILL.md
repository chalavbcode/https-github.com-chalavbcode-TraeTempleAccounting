---
name: form-duplicate-code
description: form-duplicate-code skill
---

# form-duplicate-code

ควบคุมปัญหา code duplication ระหว่าง forms — โดยเฉพาะ FrmIncome/FrmExpense ที่มี logic ซ้ำกัน ~80% และการ refactor ที่ต้อง sync หลายจุดพร้อมกัน

---

## ปัญหาที่เคยเจอ (Known Issues)

1. **FrmIncome / FrmExpense ~80% Duplicate:** ทั้งสองฟอร์มมี code ที่ identically หรือเกือบ identically:
   - `ConfigureRecentGridColumns()` — identically (229 lines ที่ต้อง sync)
   - `LoadRecentTransactions()` / `LoadRecentExpenses()` — query เหมือนกัน except `TranType`
   - `LoadMasters()` — identical except WHERE clause (`'Income'` vs `'Expense'`)
   - `SaveReceiptFile()` — identically
   - `btnBrowseReceipt_Click()` — identically
   - `btnPasteReceipt_Click()` — identically
   - `btnClearReceipt_Click()` — identically
   - `txtAmount_KeyPress()` — identically
   - `SetupEnterNavigation()` / `HandleEnterAdvance()` / `MoveNextFrom()` — identically
   - `ResolveComboTextSelection()` — identically
   - `FillAutoCompleteSource()` — identically
   - `SetComboValue()` / `GetSelectedId()` — identically
   - `GetSelectedBankValue()` / `GetSelectedBankIdOrNothing()` — identically
   - `CalculateMonthlySummary()` — near-identical (different TranType + label text)

2. **Column Config Changed in One but Not the Other:** Commit `a7c50ec` changed filter layout in one form but not the other, causing visual inconsistency

3. **Date Picker Setup Differences:** `FrmIncome` has `cboReceiveChannel` in enter flow, `FrmExpense` does not — but this difference is buried in 500+ lines of otherwise identical code

4. **Missing TransactionService in FrmExpense:** `FrmIncome` uses `_txnService As TransactionService` (via `AccessTransactionRepository`), but `FrmExpense` uses raw `Db.ExecuteNonQuery`/`Db.InsertAndGetId` — inconsistent data access pattern

5. **Chart Module Refactored 6+ Times:** `frmMain.vb` chart code was refactored in commits `98415b9`, `a981a87`, `5d619dd`, `10c2a36`, `7b8edf5`, `71c27f4` — each time partially, leaving fragmented state

6. **ReportEngine Grew Organically:** 29 changes to `ReportEngine.vb` — started as small helper, now 500+ lines mixing data preparation, layout, rendering, and shared formatting

---

## กฎที่ต้องปฏิบัติ

### Rule 1: Identify Shared Logic Across Income/Expense Forms
ตารางนี้แสดงฟังก์ชันที่ identically หรือ near-identically ระหว่าง FrmIncome.vb และ FrmExpense.vb:

| Method | Identical? | Difference |
|---|---|---|
| `ConfigureRecentGridColumns()` | ✅ Identically | None |
| `LoadMasters()` | Near | WHERE `TranType='Income'` vs `'Expense'` |
| `LoadRecentTransactions/Expenses()` | Near | Query: `WHERE t.TranType = 'Income'` vs `'Expense'` |
| `CalculateMonthlySummary()` | Near | `LIKE '%รับ%'` vs `'%จ่าย%'`, label text |
| `SaveReceiptFile()` | ✅ Identically | None |
| `btnBrowseReceipt_Click()` | ✅ Identically | None |
| `btnPasteReceipt_Click()` | ✅ Identically | None |
| `btnClearReceipt_Click()` | ✅ Identically | None |
| `txtAmount_KeyPress()` | ✅ Identically | None |
| `SetupEnterNavigation()` | Near | Income has `cboReceiveChannel` in flow |
| `HandleEnterAdvance()` | ✅ Identically | None |
| `MoveNextFrom()` | ✅ Identically | None |
| `ResolveComboTextSelection()` | ✅ Identically | None |
| `FillAutoCompleteSource()` | ✅ Identically | None |
| `SetComboValue()` / `GetSelectedId()` | ✅ Identically | None |
| `GetSelectedBankValue()` / `GetSelectedBankIdOrNothing()` | Near | Expense missing `GetSelectedBankValueNullable()` |
| `FocusStartField()` | ✅ Identically | None |
| `EditID` property setter | Near | Same structure, minor UI text diff |
| `LoadTransactionData()` | Near | Same query, Income has channel fields |
| `btnSave_Click()` | Different | Income uses `_txnService`, Expense uses raw SQL |

### Rule 2: When Changing One, Always Check the Other
**ถ้าแก้ไข method ใดๆ ใน FrmIncome.vb → ต้องตรวจสอบ FrmExpense.vb ว่ามี duplicate method เหมือนกันหรือไม่**

```vb
' Methods ที่ต้อง sync เสมอ:
' - ConfigureRecentGridColumns()
' - LoadRecentTransactions() / LoadRecentExpenses()
' - LoadMasters()
' - CalculateMonthlySummary()
' - SaveReceiptFile()
' - All receipt-related handlers
```

### Rule 3: Shared Helper Extraction Pattern
เมื่อต้องการ refactor duplicate code ให้ใช้ shared module:

```vb
' Forms/Common/FormEntryHelpers.vb (new shared module)
Public Module FormEntryHelpers
    ''' <summary>
    ''' Shared Enter-key navigation flow between controls
    ''' </summary>
    Public Sub HandleEnterAdvance(sender As Object, e As KeyEventArgs, flow As List(Of Control), Optional finalAction As Action = Nothing)
        If e.KeyCode <> Keys.Enter Then Return
        e.SuppressKeyPress = True
        Dim idx = flow.IndexOf(DirectCast(sender, Control))
        If idx < 0 Then Return
        If idx = flow.Count - 1 Then
            finalAction?.Invoke()
            Return
        End If
        Dim nextCtrl = flow(idx + 1)
        nextCtrl.Focus()
        Dim cb = TryCast(nextCtrl, ComboBox)
        If cb IsNot Nothing AndAlso cb.Items.Count > 0 Then cb.DroppedDown = True
        Dim tb = TryCast(nextCtrl, TextBox)
        If tb IsNot Nothing Then tb.SelectAll()
    End Sub

    ''' <summary>
    ''' Shared ConfigureRecentGridColumns for Income/Expense recent grids
    ''' </summary>
    Public Sub ConfigureRecentGrid(dgv As DataGridView)
        If dgv Is Nothing Then Return
        dgv.ScrollBars = ScrollBars.Both
        ' ... shared column configuration
    End Sub

    ''' <summary>
    ''' Shared monthly summary query builder
    ''' </summary>
    Public Function BuildMonthlySummaryQuery(tranType As String, matchPattern As String) As String
        Return "SELECT COUNT(*) AS TotalCount, " &
               "IIF(ISNULL(MAX(Amount)),0,MAX(Amount)) AS MaxAmount, " &
               "IIF(ISNULL(AVG(Amount)),0,AVG(Amount)) AS AvgAmount, " &
               "IIF(ISNULL(SUM(Amount)),0,SUM(Amount)) AS TotalAmount " &
               "FROM Transactions " &
               "WHERE (TranType LIKE '" & matchPattern & "' OR TranType = '" & tranType & "') " &
               "AND TranDate >= " & Db.AccessDateLiteral(New Date(Date.Today.Year, Date.Today.Month, 1)) & " " &
               "AND TranDate < " & Db.AccessDateLiteral(New Date(Date.Today.Year, Date.Today.Month, 1).AddMonths(1))
    End Function
End Module
```

### Rule 4: TransactionService Usage Standard
**ใช้ `TransactionService` pattern เสมอ** แทน raw SQL INSERT/UPDATE:

```vb
' ✅ Correct — consistent data access:
Private ReadOnly _txnService As TransactionService = New TransactionService(New AccessTransactionRepository())

' สำหรับ INSERT:
targetID = _txnService.SaveIncome(date, categoryID, fundID, bankID, detail, amount, note, channel, refNo)

' สำหรับ UPDATE:
_txnService.UpdateIncome(editId, date, categoryID, fundID, bankID, detail, amount, note, channel, refNo)

' ❌ Wrong — raw SQL, bypasses service layer:
Db.InsertAndGetId(conn, "INSERT INTO Transactions (...) VALUES (...)", ...)
Db.ExecuteNonQuery(conn, "UPDATE Transactions SET ...", ...)
```

**ปัญหา:** `FrmExpense.vb` ยังใช้ raw SQL สำหรับ INSERT/UPDATE — ควร refactor ให้ใช้ `TransactionService` เหมือน `FrmIncome.vb`

### Rule 5: Maintain Feature Parity Checklist
เมื่อเพิ่ม feature ใหม่ในฟอร์มใดฟอร์มหนึ่ง ต้อง check ว่าฟอร์ม counterpart ต้องมีด้วย:

```markdown
## Feature Parity Checklist
- [ ] FrmIncome has `cboReceiveChannel` → FrmExpense ไม่ต้อง ( Income-only feature)
- [ ] FrmIncome has `TransactionService` → FrmExpense ควร refactor ให้ใช้ด้วย
- [ ] FrmIncome has `ApplyFixedVerticalLayout()` → FrmExpense ไม่มี (layout ต่างกัน)
- [ ] Column config changed in FrmIncome → Check FrmExpense
- [ ] Receipt handling changed → Check both forms
- [ ] Summary label format changed → Check both forms
```

### Rule 6: Form Layout Stability Pattern
`FrmIncome` มี `ApplyFixedVerticalLayout()` ที่ enforce Y-coordinates แบบ hardcoded — ป้องกัน layout drift:

```vb
Private Sub ApplyFixedVerticalLayout()
    ' Lock Anchor = Top, Left only (no Bottom to prevent stretching)
    lbl7.Anchor = AnchorStyles.Top Or AnchorStyles.Left
    txtRemark.Anchor = AnchorStyles.Top Or AnchorStyles.Left
    ' ... all controls

    ' Fixed heights
    txtRemark.Height = 55

    ' Enforce Y coordinates with 8px spacing
    lbl7.Top = txtAmount.Bottom + 8
    txtRemark.Top = lbl7.Top
    lbl8.Top = txtRemark.Bottom + 8
    txtReceipt.Top = lbl8.Top
    btnBrowseReceipt.Top = txtReceipt.Top
    btnSave.Top = txtReceipt.Bottom + 12
    btnCancel.Top = btnSave.Top
End Sub
```

**กฎ:** ถ้าเพิ่ม control ใหม่ในฟอร์ม data entry → เพิ่มใน `ApplyFixedVerticalLayout()` ด้วย

---

## Anti-Patterns to Avoid

| Anti-Pattern | ปัญหา | วิธีแก้ |
|---|---|---|
| เปลี่ยน ConfigureRecentGridColumns ใน FrmIncome แต่ไม่เปลี่ยน FrmExpense | Column layout ไม่ sync | เปลี่ยนทั้งคู่เสมอ หรือ shared module |
| ใช้ raw SQL ใน FrmExpense แต่ใช้ TransactionService ใน FrmIncome | Data access pattern ไม่ consistent | Refactor ให้ใช้ TransactionService ทั้งคู่ |
| เพิ่ม feature ใน form เดียวโดยไม่ check counterpart | Feature parity drift | ใช้ Feature Parity Checklist |
| ปล่อย duplicate code 300+ lines ไว้ตลอดไป |  maintenance cost สูง | สร้าง shared helper module |
| Refactor chart/report code หลายครั้งแต่ไม่ consolidate | Fragmented logic | Consolidate หลัง refactor 3+ ครั้ง |
| เปลี่ยน Enter navigation flow ใน form เดียว | UX ไม่ consistent | Sync navigation flow ทุก data entry form |

---

## Codebase Duplication Hotspots

| Code Block | Lines | Locations | Impact |
|---|---|---|---|
| `ConfigureRecentGridColumns()` | ~50 | FrmIncome, FrmExpense | High — column config drift |
| Receipt handlers (browse/paste/clear) | ~60 | FrmIncome, FrmExpense, FrmTransactions | High — clipboard/dispose bugs |
| `LoadMasters()` | ~30 | FrmIncome, FrmExpense | Medium — query differences |
| `HandleEnterAdvance()` / `MoveNextFrom()` | ~40 | FrmIncome, FrmExpense, FrmTransactions | Medium — navigation UX |
| `SaveReceiptFile()` | ~15 | FrmIncome, FrmExpense | Low — identical |
| `SetComboValue()` / `GetSelectedId()` | ~20 | FrmIncome, FrmExpense | Low — utility functions |
| `txtAmount_KeyPress()` | ~5 | FrmIncome, FrmExpense | Low — input validation |
| **Total duplicate lines** | **~220+** | 3 forms | — |

---

## Git Commit Evidence

| Commit | ปัญหาที่แก้ |
|---|---|
| `a7c50ec` | Refactor filter UI layout and update control positions |
| `4991e5e` | Refactor Income/Expense forms layout and grid columns |
| `826721f` | Refactor summary labels and layout in FrmExpense/FrmIncome |
| `2c9be30` | Compact FrmIncome layout; improve DataGridView display |
| `377d915` | Add ApplyCompactLayout for consistent FrmIncome layout |
| `6b8bd71` | Add ApplyFixedVerticalLayout to enforce vertical layout |
| `4bc4114` | Refactor FrmIncome layout and update grid columns |
| `0c09181` | Refactor FrmIncome layout and control initialization |
| `ddbc56d` | Refactor DataGridView column initialization inline |
| `e5ce3f0` | Refactor DataGridView columns and update data loading |
| `98be768` | Add monthly summary to Income and Expense forms |
| `53416c6` | Adjust Y-coordinates of controls in FrmIncome.Designer.vb |
| `3c12c7e` | Refactor summary labels; add Close button to forms |
| `96b1079` | Increase btnClose font size in FrmIncome.Designer.vb |

---

## Hotspot Files Reference

| File | Changes | สาเหตุ |
|---|---|---|
| `Forms/FrmIncome.vb` | 62 | Entry form + receipt + grid config |
| `Forms/FrmExpense.vb` | 53 | Duplicate of FrmIncome |
| `Forms/FrmTransactions.vb` | 86 | Complex search/filter/grid |
| `Forms/FrmIncome.Designer.vb` | 35 | Layout churn |
| `Forms/FrmExpense.Designer.vb` | 26 | Layout churn (duplicate) |
| `frmMain.vb` | 39 | Dashboard + chart + form hosting |
| `Reports/ReportEngine.vb` | 29 | Report data + rendering + layout |
| `Forms/FrmReports.vb` | 45 | Report UI + date filtering |
