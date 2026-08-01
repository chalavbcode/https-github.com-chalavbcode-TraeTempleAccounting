Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data.OleDb

Namespace TempleAccounting
    <DesignerCategory("Form")>
    Partial Public Class FrmExpense
        Inherits Form

        Private ReadOnly _enterFlow As New List(Of Control)()

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub FrmExpense_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Db.EnsureSchema()
            LoadMasters()
            SetupToolTips()
            SetupEnterNavigation()
            ResetEntry(True)
            FocusStartField()
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(btnSave, "บันทึกข้อมูลรายจ่ายที่กรอกลงในฐานข้อมูล (Enter)")
            ttMain.SetToolTip(btnCancel, "ล้างข้อมูลที่กรอกไว้ทั้งหมดเพื่อเริ่มกรอกใหม่")
            ttMain.SetToolTip(btnImportExcel, "นำข้อมูลรายจ่ายจำนวนมากเข้ามาจากไฟล์ Excel (.xlsx)")
            ttMain.SetToolTip(btnBrowseReceipt, "เลือกรูปภาพหลักฐาน/ใบเสร็จ จากเครื่องคอมพิวเตอร์")
            ttMain.SetToolTip(btnClearReceipt, "ยกเลิกการเลือกรูปภาพ")
        End Sub

        Private Sub LoadMasters()
            Using conn = Db.OpenConn()
                Dim cats = Db.GetTable(conn, "SELECT ID, CategoryName FROM Categories WHERE TranType='Expense' ORDER BY CategoryName")
                cboCategory.DisplayMember = "CategoryName"
                cboCategory.ValueMember = "ID"
                cboCategory.DataSource = cats

                Dim funds = Db.GetTable(conn, "SELECT ID, FundName FROM Funds ORDER BY FundName")
                cboFund.DisplayMember = "FundName"
                cboFund.ValueMember = "ID"
                cboFund.DataSource = funds

                Dim banks = Db.GetTable(conn, "SELECT ID, BankName & '  ' & IIF(AccountNo IS NULL,'',AccountNo) & '  (' & IIF(AccountName IS NULL,'',AccountName) & ')' AS Disp FROM BankAccounts ORDER BY BankName")
                Dim blankBank = banks.NewRow()
                blankBank("ID") = DBNull.Value
                blankBank("Disp") = ""
                banks.Rows.InsertAt(blankBank, 0)
                cboBank.DisplayMember = "Disp"
                cboBank.ValueMember = "ID"
                cboBank.DataSource = banks
            End Using
        End Sub

        Private Sub ResetEntry(resetToToday As Boolean)
            If resetToToday Then
                dtpDate.Value = Today
            End If
            If cboCategory.Items.Count > 0 Then cboCategory.SelectedIndex = 0
            If cboFund.Items.Count > 0 Then cboFund.SelectedIndex = 0
            cboBank.SelectedIndex = 0
            txtDescription.Clear()
            txtAmount.Clear()
            txtRemark.Clear()
            txtReceipt.Clear()
        End Sub

        Private Function GetSelectedBankValue() As Object
            If cboBank.SelectedValue Is Nothing OrElse cboBank.SelectedValue Is DBNull.Value Then
                Return DBNull.Value
            End If
            Dim bankId As Integer
            If Integer.TryParse(cboBank.SelectedValue.ToString(), bankId) Then
                Return bankId
            End If
            Return DBNull.Value
        End Function

        Private Function GetSelectedBankIdOrNothing() As Integer?
            Dim bankValue = GetSelectedBankValue()
            If bankValue Is DBNull.Value Then Return Nothing
            Return CInt(bankValue)
        End Function

        Private Sub SetupEnterNavigation()
            If _enterFlow.Count > 0 Then Return

            _enterFlow.AddRange({dtpDate, cboCategory, cboFund, cboBank, txtDescription, txtAmount, txtRemark, btnBrowseReceipt, btnSave})

            For Each ctrl In _enterFlow
                AddHandler ctrl.KeyDown, AddressOf HandleEnterAdvance
            Next
        End Sub

        Private Sub MoveNextFrom(current As Control)
            Dim idx = _enterFlow.IndexOf(current)
            If idx < 0 Then Return

            If idx = _enterFlow.Count - 1 Then
                btnSave.PerformClick()
                Return
            End If

            Dim nextCtrl = _enterFlow(idx + 1)
            nextCtrl.Focus()

            Dim cb = TryCast(nextCtrl, ComboBox)
            If cb IsNot Nothing AndAlso cb.Items.Count > 0 Then
                cb.DroppedDown = True
            End If

            Dim tb = TryCast(nextCtrl, TextBox)
            If tb IsNot Nothing Then
                tb.SelectAll()
            End If
        End Sub

        Private Sub HandleEnterAdvance(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True
            MoveNextFrom(DirectCast(sender, Control))
        End Sub

        Private Sub FocusStartField()
            If Not IsHandleCreated Then Return
            BeginInvoke(New Action(Sub()
                                       dtpDate.Focus()
                                   End Sub))
        End Sub

        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            ResetEntry(False)
            dtpDate.Focus()
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            If cboCategory.SelectedValue Is Nothing Then MessageBox.Show("กรุณาเลือกประเภทรายจ่าย", "แจ้งเตือน") : cboCategory.Focus() : Return
            If cboFund.SelectedValue Is Nothing Then MessageBox.Show("กรุณาเลือกกองทุน", "แจ้งเตือน") : cboFund.Focus() : Return
            Dim amt As Decimal
            If Not Decimal.TryParse(txtAmount.Text, amt) OrElse amt <= 0 Then
                MessageBox.Show("กรุณาใส่จำนวนเงินที่ถูกต้อง", "แจ้งเตือน") : txtAmount.Focus() : Return
            End If
            Dim keepDate = dtpDate.Value.Date
            Dim bankValue = GetSelectedBankValue()
            Try
                Using conn = Db.OpenConn()
                    ' 1. บันทึกรายการลงตาราง Transactions และรับ newID กลับมา
                    Dim newID As Integer = Db.InsertAndGetId(conn, "INSERT INTO Transactions (TranDate, TranType, CategoryID, FundID, BankID, [Detail], Amount, [Note]) VALUES (" & Db.AccessDateLiteral(dtpDate.Value.Date) & ",'Expense',@c,@f,@b,@de,@a,@n)", New Tuple(Of String, Object)("@c", CInt(cboCategory.SelectedValue)), New Tuple(Of String, Object)("@f", CInt(cboFund.SelectedValue)), New Tuple(Of String, Object)("@b", bankValue), New Tuple(Of String, Object)("@de", txtDescription.Text.Trim), New Tuple(Of String, Object)("@a", amt), New Tuple(Of String, Object)("@n", txtRemark.Text.Trim))

                    ' 2. หากมีการเลือกรูปภาพ ให้จัดการก๊อบปี้ไฟล์และอัปเดตชื่อไฟล์ลงคอลัมน์ ReceiptPath
                    Dim savedFileName As String = SaveReceiptFile(newID)
                    If Not String.IsNullOrEmpty(savedFileName) Then
                        Using cmd = conn.CreateCommand()
                            cmd.CommandText = "UPDATE Transactions SET ReceiptPath = @rp WHERE ID = @id"
                            cmd.Parameters.AddWithValue("@rp", savedFileName)
                            cmd.Parameters.AddWithValue("@id", newID)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If

                    MessageBox.Show("✅ บันทึกรายจ่ายสำเร็จ!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ResetEntry(False)

                    ' ล้างค่าตัวแปรเก็บ Path รูปภาพที่เคยเลือกไว้ (เพื่อไม่ให้หลุดไปรายการถัดไป)
                    selectedSourceReceiptPath = ""
                    If txtReceipt IsNot Nothing Then txtReceipt.Clear()

                    dtpDate.Value = keepDate
                    dtpDate.Focus()
                End Using
            Catch ex As Exception
                MessageBox.Show("บันทึกไม่สำเร็จ: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ' --- ฟังก์ชันช่วยก๊อบปี้รูปภาพใบเสร็จไปยัง AppPaths.ReceiptsDir ---
        Private Function SaveReceiptFile(transactionID As Integer) As String
            If String.IsNullOrWhiteSpace(selectedSourceReceiptPath) OrElse Not IO.File.Exists(selectedSourceReceiptPath) Then
                Return ""
            End If

            Try
                AppPaths.EnsureDirectoriesExist()

                ' ตั้งชื่อไฟล์ใหม่ตาม ID ของรายการ เช่น Receipt_105.jpg
                Dim ext As String = IO.Path.GetExtension(selectedSourceReceiptPath)
                Dim newFileName As String = $"Receipt_{transactionID}{ext}"
                Dim destPath As String = IO.Path.Combine(AppPaths.ReceiptsDir, newFileName)

                ' ก๊อบปี้ไฟล์รูปจากโฟลเดอร์ต้นทาง (LINE Downloads) ไปวางที่โฟลเดอร์ Receipts
                IO.File.Copy(selectedSourceReceiptPath, destPath, True)

                Return newFileName
            Catch ex As Exception
                AppPaths.LogCrash(ex, "SaveReceiptFile")
                Return ""
            End Try
        End Function

        Private Sub btnImportExcel_Click(sender As Object, e As EventArgs) Handles btnImportExcel.Click
            If cboCategory.SelectedValue Is Nothing OrElse cboFund.SelectedValue Is Nothing Then
                MessageBox.Show("กรุณาเลือกประเภท และกองทุนเริ่มต้นก่อนนำเข้า", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Using ofd As New OpenFileDialog()
                ofd.Title = "เลือกไฟล์ Excel สำหรับนำเข้ารายจ่าย"
                ofd.Filter = "Excel Files|*.xlsx;*.xlsm;*.xlsb;*.xls|All Files|*.*"
                If ofd.ShowDialog(Me) <> DialogResult.OK Then Return

                Try
                    Dim defaultBankId = GetSelectedBankIdOrNothing()
                    Dim result = ImportTransactionsFromExcel(ofd.FileName, "Expense", CInt(cboCategory.SelectedValue), CInt(cboFund.SelectedValue), defaultBankId)
                    LoadMasters()
                    ResetEntry(False)
                    dtpDate.Focus()
                    MessageBox.Show(result.BuildSummaryMessage("รายจ่าย"), "นำเข้าข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("นำเข้าข้อมูลไม่สำเร็จ: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Sub

        ' ตัวแปรเก็บ Path ของไฟล์รูปต้นทางที่ผู้ใช้เลือก
        Private selectedSourceReceiptPath As String = ""

        ' ปุ่มกดเลือกรูปภาพ
        Private Sub btnBrowseReceipt_Click(sender As Object, e As EventArgs) Handles btnBrowseReceipt.Click
            Using ofd As New OpenFileDialog()
                ' กำหนดโฟลเดอร์เริ่มต้นไปที่จุดดาวน์โหลดของ LINE Desktop
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

        ' ปุ่มยกเลิกรูปภาพ (ถ้ามี)
        Private Sub btnClearReceipt_Click(sender As Object, e As EventArgs) Handles btnClearReceipt.Click
            selectedSourceReceiptPath = ""
            txtReceipt.Clear()
        End Sub
        Private Sub txtAmount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtAmount.KeyPress
            If Char.IsControl(e.KeyChar) Then Return
            If e.KeyChar = "."c AndAlso txtAmount.Text.Contains(".") Then e.Handled = True : Return
            If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then e.Handled = True
        End Sub
    End Class
End Namespace
