---
name: receipt-clipboard-workflow
description: receipt-clipboard-workflow skill
---

# receipt-clipboard-workflow

ควบคุม workflow การจัดการรูปภาพใบเสร็จ — clipboard paste, browse from file, save to Receipts folder, และ lifecycle management ผ่าน `ReceiptImageHelper`

---

## ปัญหาที่เคยเจอ (Known Issues)

1. **Receipt handling duplicated across 3 forms:** `FrmIncome.vb`, `FrmExpense.vb`, `FrmTransactions.vb` มี logic จัดการรูปใบเสร็จที่เกือบ identically — `selectedSourceReceiptPath`, `isImageFromClipboard`, `clipboardImage`, browse/paste/clear/save logic ถูก copy-paste ทั้งหมด
2. **Clipboard leak after save:** ไม่ clear clipboard หลังบันทึกรูป → ผู้ใช้วางรูปเดิมซ้ำในรายการถัดไป (แก้ไขใน commit `2f4f391`, `d67bbf0`)
3. **Event handler wiring inconsistency:** `btnPasteReceipt.Click` บางครั้งใช้ `Handles` keyword, บางครั้งใช้ `AddHandler`/`RemoveHandler` — ทำให้ handler ซ้ำกัน (แก้ไขใน commit `63378b2`, `18903fa`)
4. **Receipt image not disposed:** `clipboardImage` เป็น GDI+ `Image` object ที่ไม่ dispose → memory leak เมื่อผู้ใช้ paste หลายครั้ง
5. **Cross-machine receipt path mismatch:** Export/Import ระหว่างเครื่องใช้ `TransactionGUID` เป็นชื่อไฟล์ receipt — ถ้า GUID ว่างจะไม่ match (commit `f73a861`, `3c85fd3`)
6. **ReceiptPath stored as filename only:** เก็บเฉพาะชื่อไฟล์ (เช่น `receipt_42_1.jpg`) ไม่ใช่ full path — ต้อง resolve ด้วย `ReceiptImageHelper.ResolveReceiptPath()` ตอนเปิดดู

---

## กฎที่ต้องปฏิบัติ

### Rule 1: Standard Receipt State Variables
ทุกฟอร์มที่มีใบเสร็จต้องมี 3 ตัวแปรนี้ (ไม่มากไม่น้อย):

```vb
Private selectedSourceReceiptPath As String = ""
Private isImageFromClipboard As Boolean = False
Private clipboardImage As Image = Nothing
```

**Location:** ต้นคลาส, ก่อน constructor

### Rule 2: Browse Pattern
```vb
Private Sub btnBrowseReceipt_Click(sender As Object, e As EventArgs) Handles btnBrowseReceipt.Click
    Using ofd As New OpenFileDialog()
        ' Default to LINE downloads folder if exists
        If IO.Directory.Exists("C:\LineDownloads") Then
            ofd.InitialDirectory = "C:\LineDownloads"
        End If
        ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
        ofd.Title = "เลือกรูปภาพใบเสร็จ"

        If ofd.ShowDialog() = DialogResult.OK Then
            selectedSourceReceiptPath = ofd.FileName
            txtReceipt.Text = IO.Path.GetFileName(selectedSourceReceiptPath)
        End If
    End Using
End Sub
```

### Rule 3: Clipboard Paste Pattern
```vb
Private Sub btnPasteReceipt_Click(sender As Object, e As EventArgs)
    If Clipboard.ContainsImage() Then
        clipboardImage = Clipboard.GetImage()
        isImageFromClipboard = True
        selectedSourceReceiptPath = "Clipboard_Image"
        txtReceipt.Text = "[รูปภาพจากคลิปบอร์ด/LINE]"
        btnSave.Focus()
    Else
        MessageBox.Show("ไม่พบรูปภาพใหม่ในคลิปบอร์ด กรุณาไปที่ LINE แล้วกดคัดลอกรูปภาพใบเสร็จใหม่ก่อนกดปุ่มนี้",
                        "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End If
End Sub
```

**กฎ:** ใช้ `AddHandler`/`RemoveHandler` แทน `Handles` keyword สำหรับปุ่มนี้ เพื่อหลีกเลี่ยง BC30506 ในบางสภาพแวดล้อม:

```vb
' ใน SetupToolTips() หรือ Form_Load:
RemoveHandler btnPasteReceipt.Click, AddressOf btnPasteReceipt_Click
AddHandler btnPasteReceipt.Click, AddressOf btnPasteReceipt_Click
```

### Rule 4: Clear Receipt Pattern (Must Dispose + Clear Clipboard)
```vb
Private Sub btnClearReceipt_Click(sender As Object, e As EventArgs) Handles btnClearReceipt.Click
    selectedSourceReceiptPath = ""
    isImageFromClipboard = False
    If clipboardImage IsNot Nothing Then clipboardImage.Dispose()  ' ← CRITICAL: dispose GDI+ object
    clipboardImage = Nothing
    txtReceipt.Clear()

    ' Clear clipboard to prevent pasting old image
    Try
        Clipboard.Clear()
    Catch ex As Exception
        AppPaths.LogCrash(ex, "ClearReceipt.ClearClipboard")
    End Try
End Sub
```

### Rule 5: Save Receipt After Transaction Save
```vb
' หลัง INSERT/UPDATE transaction สำเร็จ:
If Not String.IsNullOrEmpty(selectedSourceReceiptPath) Then
    Dim savedFileName As String = SaveReceiptFile(targetID)
    If Not String.IsNullOrEmpty(savedFileName) Then
        _txnService.UpdateReceiptPath(targetID, savedFileName)
    End If
End If
```

**SaveReceiptFile pattern:**
```vb
Private Function SaveReceiptFile(transactionID As Integer) As String
    If isImageFromClipboard Then
        If clipboardImage Is Nothing Then Return ""
        Return ReceiptImageHelper.SaveOptimizedReceipt(clipboardImage, transactionID)
    End If

    If String.IsNullOrWhiteSpace(selectedSourceReceiptPath) OrElse
       Not IO.File.Exists(selectedSourceReceiptPath) Then
        Return ""
    End If

    Return ReceiptImageHelper.SaveOptimizedReceipt(selectedSourceReceiptPath, transactionID)
End Function
```

### Rule 6: Reset State After Save
หลังบันทึกสำเร็จ ต้อง clear ทุกอย่าง:

```vb
' หลัง MessageBox สำเร็จ:
selectedSourceReceiptPath = ""
If txtReceipt IsNot Nothing Then txtReceipt.Clear()

' Clear clipboard ป้องกันวางรูปเดิมซ้ำ
Try
    Clipboard.Clear()
Catch ex As Exception
    AppPaths.LogCrash(ex, "ClearClipboard")
End Try
```

### Rule 7: Receipt File Resolution for Viewing
`ReceiptPath` ใน DB เก็บแค่ชื่อไฟล์ — ต้อง resolve หลายโฟลเดอร์:

```vb
' ✅ Correct:
Dim fullPath = ReceiptImageHelper.ResolveReceiptPath(fileName)

' ❌ Wrong — ไม่ resolve:
Dim fullPath = IO.Path.Combine(AppPaths.ReceiptsDir, fileName)
' เพราะไฟล์อาจอยู่ใน Export/Import subfolder
```

### Rule 8: Cross-Machine Receipt Matching (Export/Import)
เมื่อ export ใบเสร็จข้ามเครื่อง ใช้ `TransactionGUID` เป็นชื่อไฟล์:

```vb
' Export side (FrmExportForTransfer.vb):
Dim receiptDest = Path.Combine(receiptsExportDir, r.TransactionGUID & ".jpg")

' Import side (FrmMultiMachineImport.vb):
Dim candidate = Path.Combine(receiptsDir, r.TransactionGUID & ".jpg")
```

**ข้อจำกัด:** ถ้า `TransactionGUID` ว่าง → ไม่ export/import receipt (ข้าม row)

---

## Anti-Patterns to Avoid

| Anti-Pattern | ปัญหา | วิธีแก้ |
|---|---|---|
| ไม่ dispose `clipboardImage` | GDI+ memory leak | เรียก `.Dispose()` ทุกครั้งที่ clear |
| ไม่ clear clipboard หลัง save | ผู้ใช้วางรูปเดิมซ้ำในรายการถัดไป | เรียก `Clipboard.Clear()` เสมอ |
| ใช้ `Handles` keyword สำหรับ btnPasteReceipt | BC30506 ในบาง environment | ใช้ `AddHandler`/`RemoveHandler` |
| เก็บ full path ใน `selectedSourceReceiptPath` | ไม่ compact, path อาจไม่ valid ทุกเครื่อง | เก็บ filename จาก `ReceiptImageHelper` |
| ใช้ `File.Delete(path)` ตรงๆ ตอนลบ receipt | ไม่ resolve multi-folder | ใช้ `ReceiptImageHelper.ResolveReceiptPath()` |
| Duplicate receipt logic ใน 3+ forms | เปลี่ยนที่หนึ่งลืมอีกที่ | สร้าง shared `ReceiptWorkflow` helper class |
| ไม่ check `Clipboard.ContainsImage()` | NullReferenceException | Check ก่อนเสมอ |

---

## Git Commit Evidence

| Commit | ปัญหาที่แก้ |
|---|---|
| `2f4f391` | Clear clipboard after saving/clearing receipt images |
| `d67bbf0` | Clear clipboard after saving/clearing receipt images (branch modify_Backup) |
| `63378b2` | Switch to AddHandler for btnPasteReceipt.Click events |
| `18903fa` | Switch to AddHandler for btnPasteReceipt.Click events |
| `3c1e499` | Refactor receipt image handling with helper class |
| `76958a7` | Refactor receipt image handling with helper class |
| `3c85fd3` | Scope selectedSourceReceiptPath to form classes |
| `e1f420e` | Add ReceiptPath support to transactions display |
| `5377c66` | Add ReceiptPath support to transactions display |
| `f73a861` | Add receipt attachment handling to export/import flow |
| `4ebdaf2` | Add FrmReceiptViewer and improve receipt image handling |
| `b13f928` | Add receipt viewing and UI adjustments in FrmTransactions |
| `2d625ff` | Enhance receipt image selection in forms |
| `820ce17` | Add receipt image support to Expense and Income forms |

---

## Hotspot Files Reference

| File | Changes | สาเหตุ |
|---|---|---|
| `Forms/FrmIncome.vb` | 62 | Receipt browse/paste/save/clear + clipboard |
| `Forms/FrmExpense.vb` | 53 | Receipt browse/paste/save/clear (duplicate) |
| `Forms/FrmTransactions.vb` | 86 | Receipt viewer, paste, delete |
| `ReceiptImageHelper.vb` | 2 | Shared helper (stable) |
| `Forms/FrmMultiMachineImport.vb` | 7 | Cross-machine receipt matching |
| `Forms/FrmExportForTransfer.vb` | 5 | Receipt export with GUID naming |
