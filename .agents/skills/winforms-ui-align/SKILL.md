---
name: winforms-ui-align
description: winforms-ui-align skill
---

# winforms-ui-align

ควบคุมพิกัด, ดีไซน์ UI, alignment, responsive layout และ visual standards ของ WinForms app นี้

---

## ปัญหาที่เคยเจอ (Known Issues)

1. **ปุ่ม ❌ ปิด วางลอยบนตาราง:** ไม่ได้จัดตำแหน่งใน panel ฝั่งซ้าย
2. **dgvRecentTransactions ยืดลงล่างเกินจำเป็น:** Anchor = Top,Bottom ทำให้ตารางขยายไม่จำเป็น
3. **แถบสีเหลืองพาดหัวตารางหาย:** ไม่ได้ maintain header strip เหนือตาราง
4. **Card layout บน Dashboard:** metric cards ต้อง responsive กับขนาดหน้าจอ

---

## กฎที่ต้องปฏิบัติ

### Rule 1: Top Metric Strip
แถบสรุปยอดบน Dashboard ต้อง:
- จัดกึ่งกลาง
- ใช้ `|` คั่นระหว่าง metrics
- บังคับใช้หน่วย **"ครั้ง"** เท่านั้น ห้ามใช้ "ใบ"

### Rule 2: DataGridView Standard
```vb
' Anchor = Top, Left, Right เท่านั้น ห้ามใส่ Bottom
dgvTransactions.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right

' ล็อกความสูง ~10 แถว (~290px) ไม่ให้ยืดลงล่าง
dgvTransactions.Size = New Size(1157, 290)

' ต้องมีพาดหัวสีเหลือง #F1C40F เหนือตารางเสมอ
' Format: "📋 รายการ...ล่าสุด 10 รายการ"
```

**Anti-pattern:** ใส่ `AnchorStyles.Bottom` → ตารางยืดลงล่างเกินจำเป็นเมื่อ resize

### Rule 2b: ApplyFixedVerticalLayout Pattern
`FrmIncome` และ `FrmExpense` มี `ApplyFixedVerticalLayout()` ที่ enforce Y-coordinates แบบ programmatic เพื่อป้องกัน layout drift:

```vb
Private Sub ApplyFixedVerticalLayout()
    ' Lock Anchor = Top, Left only
    lbl7.Anchor = AnchorStyles.Top Or AnchorStyles.Left
    txtRemark.Anchor = AnchorStyles.Top Or AnchorStyles.Left
    ' ... all controls
    btnClose.Anchor = AnchorStyles.Top Or AnchorStyles.Left

    ' Fixed heights
    txtRemark.Height = 55

    ' Enforce Y coordinates with 8px spacing
    lbl7.Top = txtAmount.Bottom + 8
    txtRemark.Top = lbl7.Top
    lbl8.Top = txtRemark.Bottom + 8
    txtReceipt.Top = lbl8.Top
    btnSave.Top = txtReceipt.Bottom + 12
    btnCancel.Top = btnSave.Top
    btnImportExcel.Top = btnSave.Top

    ' Lock btnClose to same row as other action buttons
    btnClose.Top = btnSave.Top
    btnClose.Height = btnSave.Height
    If btnImportExcel IsNot Nothing AndAlso btnImportExcel.Visible Then
        btnClose.Left = btnImportExcel.Right + 8
    Else
        btnClose.Left = btnCancel.Right + 8
    End If
End Sub
```

**กฎ:** Data entry forms ที่มี control หลายแถว ต้องมี `ApplyFixedVerticalLayout()` หรือ `ApplyCompactLayout()` เพื่อล็อกตำแหน่ง control

### Rule 3: Button Alignment
ปุ่มดำเนินการต้องเรียงต่อกันใน **ระนาบเดียวกัน** (Y เดียวกัน) ใน Panel ฝั่งซ้าย:

```
[💾 บันทึก] [🗑️ เคลียร์] [📥 นำเข้า Excel] [❌ ปิด]
   ↑ ทั้ง 4 ปุ่มมี Y = เดียวกัน
```

**กฎเพิ่มเติม — btnClose ต้อง lock ใน `ApplyFixedVerticalLayout()`:**

```vb
' ต้องอยู่ใน ApplyFixedVerticalLayout() เสมอtnClose.Anchor = AnchorStyles.Top Or AnchorStyles.Left  ' ห้ามใส่ Bottom — กัน vertical drift
btnClose.Top = btnSave.Top        ' ← Lock Y เดียวกัน
btnClose.Height = btnSave.Height  ' ← ความสูงเท่ากัน
btnClose.Left = btnImportExcel.Right + 8  ' ← ห่างจากปุ่มก่อนหน้า 8px
```

**Anti-pattern:** btnClose อยู่คนละ Y กับ btnSave — เกิดจาก `ApplyFixedVerticalLayout()` ไม่ได้ตั้งค่า btnClose.Top

ปุ่ม `❌ ปิด` ต้องอยู่ใน panel ฝั่งซ้าย ไม่ใช่ลอยบนตาราง

### Rule 4: Responsive Dashboard Cards
`picHomeLogo` ต้อง responsive กับขนาด `pnlFormHostBody`:

```vb
' ต้องคำนวณตำแหน่ง dynamic:
Dim hostW As Integer = pnlFormHostBody.ClientSize.Width
Dim hostH As Integer = pnlFormHostBody.ClientSize.Height
' Center picHomeLogo based on hostW, hostH
```

**ห้ามล็อกขนาด/ตำแหน่ง picHomeLogo ตายตัว** — ต้อง respond to resize event

### Rule 5: pnlOverview Layout
`pnlOverview` ต้องมี:
- **Background:** สีพื้นหลังที่เข้ากับ theme (ครีม/ขาว)
- ** metric cards:** DonorCount, IncomeMonth, ExpenseMonth, Balance
- **Visibility control:** ซ่อนเมื่อเปิด child form, แสดงเมื่อกลับ Dashboard

### Rule 6: Form Host Panel Hierarchy
```
frmMain
├── Side Nav (left)
├── pnlFormHost (center-right)
│   ├── pnlFormHostHeader (height ~84px, title + nav buttons)
│   │   └── Label for current page title
│   └── pnlFormHostBody (Dock = Fill)
│       ├── pnlOverview (visible on dashboard)
│       ├── picHomeLogo (centered, dynamic position)
│       └── [Child Forms] (Dock = Fill when opened)
```

### Rule 7: MessageBox Patterns
```vb
' สำเร็จ:
MessageBox.Show("✅ บันทึกข้อมูลเรียบร้อยแล้ว!", "สำเร็จ",
                 MessageBoxButtons.OK, MessageBoxIcon.Information)

' ผิดพลาด:
MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ผิดพลาด",
                 MessageBoxButtons.OK, MessageBoxIcon.Error)

' แจ้งเตือน:
MessageBox.Show("กรุณาใส่ข้อมูล...", "แจ้งเตือน",
                 MessageBoxButtons.OK, MessageBoxIcon.Warning)

' ยืนยัน (Yes/No):
If MessageBox.Show("ยืนยันการลบ?", "ยืนยัน",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
```

**กฎ:** ใช้ emoji prefix (✅, ⚠️, ⛔) ใน message text — ไม่ใช่ใน caption

### Rule 8: Thai UI Text Standards
- ปุ่ม: `[emoji] [คำสั่งภาษาไทย]` เช่น `💾 บันทึก`, `📝 แก้ไข`, `❌ ปิด`
- Header: `📋 รายการ...ล่าสุด 10 รายการ`
- Validation messages: ภาษาไทยทั้งหมด, ขึ้นต้นด้วย "กรุณา..."
- Success: ขึ้นต้นด้วย "✅"
- Error: ขึ้นต้นด้วย "ไม่สามารถ..." หรือ "เกิดข้อผิดพลาด"

---

## Anti-Patterns to Avoid

| Anti-Pattern | ปัญหา | วิธีแก้ |
|---|---|---|
| ปุ่ม ❌ ลอยบนตาราง | visual confusion | จัดใน panel ฝั่งซ้าย |
| Anchor Bottom บน DGV | ตารางยืดเกินจำเป็น | ใช้ Top,Left,Right เท่านั้น |
| ไม่มี header strip สีเหลือง | ผู้ใช้ไม่รู้ว่าตารางมีกี่รายการ | เพิ่ม `#F1C40F` header |
| picHomeLogo ตำแหน่งตายตัว | ไม่ responsive | คำนวณ dynamic จาก host size |
| MessageBox ภาษาอังกฤษ | ผู้ใช้ไทยไม่เข้าใจ | ใช้ภาษาไทย + emoji |
| MessageBox caption เป็น "Error" | ไม่ consistent | ใช้ "ผิดพลาด" / "แจ้งเตือน" / "สำเร็จ" |

---

## Color Reference

| Element | Color | Usage |
|---|---|---|
| Header strip | `#F1C40F` (yellow) | พาดหัวเหนือ DataGridView |
| Body background | `Color.FromArgb(255, 253, 244)` (cream) | `pnlFormHostBody.BackColor` |
| Button emoji | System font | 💾 บันทึก, 📝 แก้ไข, ❌ ปิด |
