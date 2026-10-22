---
name: childform-mdi-nav
description: childform-mdi-nav skill
---

# childform-mdi-nav

ควบคุมโครงสร้าง MDI Child Form, การเปิด/ปิด/สลับหน้า, และการสื่อสารระหว่าง frmMain กับ Child Forms

---

## ปัญหาที่เคยเจอ (Known Issues)

1. **แถบสีขาว pnlOverview ซ้อนทับ:** เมื่อเปิด Child Form (FrmIncome/FrmExpense) pnlOverview ของ frmMain ยัง Visible=True ทำให้แถบสีขาวไปโผล่ซ้อนในหน้าบันทึกรับ-จ่าย
2. **Child Form ไม่เต็มพื้นที่:** ไม่ได้ตั้ง `Dock = DockStyle.Fill` ทำให้ child form มีขนาดตายตัว ไม่ขยายตาม host panel
3. **ปิด Child Form แล้วไม่กลับ Dashboard:** ปุ่ม btnClose ไม่ได้เรียก `ShowDashboardHome()` ทำให้หน้าจอค้าง

---

## กฎที่ต้องปฏิบัติ

### Rule 1: Visibility Toggle
เมื่อเปิด/ปิด Child Form ต้องจัดการ `pnlOverview.Visible` เสมอ

```vb
' เมื่อเปิด Child Form:
pnlOverview.Visible = False

' เมื่อกลับหน้าหลัก (Dashboard):
pnlOverview.Visible = True
```

** Locations:**
- `frmMain.vb` L199: `pnlOverview.Visible = True` (ตอน show dashboard)
- `frmMain.vb` L472: `pnlOverview.Visible = False` (ตอนเปิด child form)
- `frmMain.UiHelpers.vb` L151: `pnlOverview.Visible = Not isCompact` (responsive layout)

### Rule 2: Docking Standard
Child Form ต้องตั้งค่า `Dock = DockStyle.Fill` ใน `pnlFormHostBody`

```vb
' ใน frmMain.Designer.vb:
pnlFormHostBody.Dock = DockStyle.Fill
```

`pnlFormHostBody` เป็น panel ที่อยู่ภายใน `pnlFormHost` โดยมี header bar อยู่ด้านบน — Child Form จะถูกเพิ่มเป็น Control ของ `pnlFormHostBody` และต้อง fill เต็มพื้นที่ทันที

### Rule 3: Close Button Must Navigate Back
ปุ่ม `btnClose` ใน Child Form ต้องมี event handler ที่เรียก `ShowDashboardHome()` ของหน้าแม่

```vb
' ใน Child Form (FrmIncome, FrmExpense, etc.)
Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
    ' เรียกกลับหน้าหลักของ frmMain
    Me.ParentForm... ' or DirectCast(Me.MdiParent, frmMain).ShowDashboardHome()
End Sub
```

### Rule 4: Form Host Container Hierarchy
```
frmMain
├── pnlFormHost (top-level container)
│   ├── pnlFormHostHeader (title bar, navigation buttons)
│   └── pnlFormHostBody (Dock = Fill, where child forms are placed)
│       └── [Child Form] (Dock = Fill inside pnlFormHostBody)
```

### Rule 5: picHomeLogo Centering
`picHomeLogo` ถูก positioned แบบ dynamic ตามขนาด `pnlFormHostBody`:

```vb
' frmMain.vb L165-169:
Dim hostW As Integer = pnlFormHostBody.ClientSize.Width
Dim hostH As Integer = pnlFormHostBody.ClientSize.Height
' คำนวณตำแหน่งกึ่งกลาง dynamically
```

**ห้ามล็อกขนาด picHomeLogo** ตายตัว — ต้อง respond to resize

### Rule 6: pnlOverview Contains Dashboard Cards
`pnlOverview` มี metric cards (DonorCount, IncomeMonth, ExpenseMonth, Balance) — เมื่อซ่อนแล้วต้องแสดงกลับมาครบถ้วนตอนกลับ Dashboard

---

## Anti-Patterns to Avoid

| Anti-Pattern | ปัญหา | วิธีแก้ |
|---|---|---|
| ไม่ toggle `pnlOverview.Visible` | แถบสีขาวซ้อนทับ child form | ตั้ง `Visible = False` เมื่อเปิด child |
| Child Form ไม่มี `Dock = Fill` | form ไม่ขยายเต็มพื้นที่ | ตั้ง `Dock = DockStyle.Fill` |
| ปุ่ม Close ไม่ navigate กลับ | UI ค้างไม่กลับ dashboard | เรียก `ShowDashboardHome()` |
| เพิ่ม child form โดยไม่ remove control เก่า | memory leak, control ซ้อน | Remove เก่าก่อน Add ใหม่ |
| ใช้ `Show()` แทน `ShowDialog()` สำหรับ MDI child | Form ลอยไม่ผูก MDI parent | ใช้ `MdiParent = frmMain` |

---

## Key Functions Reference

| Function | Location | หน้าที่ |
|---|---|---|
| `ShowDashboardHome()` | `frmMain.vb` | กลับหน้า Dashboard + toggle pnlOverview |
| `OpenIncomeForm()` | `frmMain.vb` | ซ่อน pnlOverview + เปิด FrmIncome |
| `OpenExpenseForm()` | `frmMain.vb` | ซ่อน pnlOverview + เปิด FrmExpense |
| `PositionHomeLogo()` | `frmMain.vb` | คำนวณตำแหน่ง picHomeLogo กลาง pnlFormHostBody |
