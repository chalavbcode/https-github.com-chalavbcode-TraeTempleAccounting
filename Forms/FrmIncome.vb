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
    Partial Public Class FrmIncome
        ' ตัวแปรเก็บ Path รูปภาพต้นทางที่ผู้ใช้เลือก (เช่น จาก C:\LineDownloads)
        Private selectedSourceReceiptPath As String = ""
        Private isImageFromClipboard As Boolean = False
        Private clipboardImage As Image = Nothing
        Private _editId As Integer = 0

        Private ReadOnly _enterFlow As New List(Of Control)()

        Public Sub New()
            InitializeComponent()
        End Sub

        ''' <summary>
        ''' รหัสรายการที่ต้องการแก้ไข (ถ้าเป็น 0 หมายถึงเพิ่มใหม่)
        ''' </summary>
        Public Property EditID As Integer
            Get
                Return _editId
            End Get
            Set(value As Integer)
                _editId = value
                If _editId > 0 Then
                    LoadTransactionData(_editId)
                    ' ปรับเปลี่ยน UI สำหรับโหมดแก้ไข
                    btnCancel.Text = "🔙 ย้อนกลับ"
                    btnCancel.BackColor = Color.FromArgb(75, 85, 99) ' สีเทาเข้ม
                    btnImportExcel.Visible = False
                    ttMain.SetToolTip(btnCancel, "ยกเลิกการแก้ไขและย้อนกลับไปหน้าก่อนหน้า")
                Else
                    ' โหมดเพิ่มใหม่
                    btnCancel.Text = "❌ ล้างข้อมูล"
                    btnCancel.BackColor = Color.FromArgb(180, 83, 9) ' สีส้มอิฐเดิม
                    btnImportExcel.Visible = True
                    ttMain.SetToolTip(btnCancel, "ล้างข้อมูลที่กรอกไว้ทั้งหมดเพื่อเริ่มกรอกใหม่")
                End If
            End Set
        End Property

        Public Sub LoadTransactionData(id As Integer)
            Try
                Using conn = Db.OpenConn()
                    Dim dt = Db.GetTable(conn, "SELECT * FROM Transactions WHERE ID = " & id)
                    If dt.Rows.Count > 0 Then
                        Dim dr = dt.Rows(0)
                        dtpDate.Value = Db.NormalizeGregorianDate(Convert.ToDateTime(dr("TranDate")))
                        cboCategory.SelectedValue = dr("CategoryID")
                        cboFund.SelectedValue = dr("FundID")
                        cboBank.SelectedValue = If(dr("BankID") Is DBNull.Value, DBNull.Value, dr("BankID"))
                        txtDescription.Text = Convert.ToString(dr("Detail"))
                        txtAmount.Text = Convert.ToDecimal(dr("Amount")).ToString("N2")
                        txtRemark.Text = Convert.ToString(dr("Note"))
                        txtReceipt.Text = Convert.ToString(dr("ReceiptPath"))
                        
                        lblHeader.Text = "📝 แก้ไขรายการรายรับ (ID: " & id & ")"
                        btnSave.Text = "💾 บันทึกการแก้ไข"
                        btnImportExcel.Visible = False
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("ไม่สามารถโหลดข้อมูลรายการได้: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub FrmIncome_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.KeyPreview = True
            HelpSystem.SetupHelp(Me, "FrmIncome")
            Db.EnsureSchema()
            LoadMasters()
            SetupToolTips()
            SetupEnterNavigation()
            UiFitter.AutoFitFormButtons(Me)
            ResetEntry(True)
            FocusStartField()
        End Sub

        Private Sub FrmIncome_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.F1 Then
                e.Handled = True
                e.SuppressKeyPress = True
                HelpSystem.ShowManual("FrmIncome", Me)
            End If
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(btnSave, "บันทึกข้อมูลรายรับที่กรอกลงในฐานข้อมูล (Enter)")
            ttMain.SetToolTip(btnCancel, "ล้างข้อมูลที่กรอกไว้ทั้งหมดเพื่อเริ่มกรอกใหม่")
            ttMain.SetToolTip(btnImportExcel, "นำข้อมูลรายรับจำนวนมากเข้ามาจากไฟล์ Excel (.xlsx)")
            ttMain.SetToolTip(btnBrowseReceipt, "เลือกรูปภาพหลักฐาน/ใบเสร็จ จากเครื่องคอมพิวเตอร์")
            ttMain.SetToolTip(btnPasteReceipt, "วางรูปภาพหลักฐานที่คัดลอกมาจาก LINE หรือโปรแกรมอื่น (Ctrl+V)")
            ttMain.SetToolTip(btnClearReceipt, "ยกเลิกการเลือกรูปภาพ")

            ' ใช้ AddHandler แทน Handles เพื่อเลี่ยงปัญหา BC30506 ในบางสภาพแวดล้อม
            RemoveHandler btnPasteReceipt.Click, AddressOf btnPasteReceipt_Click
            AddHandler btnPasteReceipt.Click, AddressOf btnPasteReceipt_Click
        End Sub

        Private Sub LoadMasters()
            Using conn = Db.OpenConn()
                Dim cats = Db.GetTable(conn, "SELECT ID, CategoryName FROM Categories WHERE TranType='Income' ORDER BY CategoryName")
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

            _enterFlow.AddRange({dtpDate, cboCategory, cboFund, cboBank, txtDescription, txtAmount, txtRemark, btnBrowseReceipt, btnPasteReceipt, btnSave})

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
            If _editId > 0 Then
                ' ถ้าเป็นโหมดแก้ไข ให้ย้อนกลับไปหน้า Transactions
                Dim f = TryCast(Me.ParentForm, frmMain)
                If f IsNot Nothing Then
                    f.btnMember.PerformClick()
                End If
            Else
                ' ถ้าเป็นโหมดเพิ่มใหม่ ให้ล้างข้อมูล
                ResetEntry(True)
                dtpDate.Focus()
            End If
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            If cboCategory.SelectedValue Is Nothing Then MessageBox.Show("กรุณาเลือกประเภทรายรับ", "แจ้งเตือน") : cboCategory.Focus() : Return
            If cboFund.SelectedValue Is Nothing Then MessageBox.Show("กรุณาเลือกกองทุน", "แจ้งเตือน") : cboFund.Focus() : Return
            Dim amt As Decimal
            If Not Decimal.TryParse(txtAmount.Text, amt) OrElse amt <= 0 Then
                MessageBox.Show("กรุณาใส่จำนวนเงินที่ถูกต้อง", "แจ้งเตือน") : txtAmount.Focus() : Return
            End If
            Dim keepDate = dtpDate.Value.Date
            Dim bankValue = GetSelectedBankValue()
            Try
                Using conn = Db.OpenConn()
                    Dim targetID As Integer = _editId

                    If _editId > 0 Then
                        ' โหมดแก้ไข: UPDATE
                        Db.ExecuteNonQuery(conn, "UPDATE Transactions SET TranDate=" & Db.AccessDateLiteral(dtpDate.Value.Date) & ", CategoryID=@c, FundID=@f, BankID=@b, [Detail]=@de, Amount=@a, [Note]=@n WHERE ID=@id",
                            New Tuple(Of String, Object)("@c", CInt(cboCategory.SelectedValue)),
                            New Tuple(Of String, Object)("@f", CInt(cboFund.SelectedValue)),
                            New Tuple(Of String, Object)("@b", bankValue),
                            New Tuple(Of String, Object)("@de", txtDescription.Text.Trim),
                            New Tuple(Of String, Object)("@a", amt),
                            New Tuple(Of String, Object)("@n", txtRemark.Text.Trim),
                            New Tuple(Of String, Object)("@id", _editId))
                    Else
                        ' โหมดเพิ่มใหม่: INSERT
                        targetID = Db.InsertAndGetId(conn, "INSERT INTO Transactions (TranDate, TranType, CategoryID, FundID, BankID, [Detail], Amount, [Note]) VALUES (" & Db.AccessDateLiteral(dtpDate.Value.Date) & ",'Income',@c,@f,@b,@de,@a,@n)",
                            New Tuple(Of String, Object)("@c", CInt(cboCategory.SelectedValue)),
                            New Tuple(Of String, Object)("@f", CInt(cboFund.SelectedValue)),
                            New Tuple(Of String, Object)("@b", bankValue),
                            New Tuple(Of String, Object)("@de", txtDescription.Text.Trim),
                            New Tuple(Of String, Object)("@a", amt),
                            New Tuple(Of String, Object)("@n", txtRemark.Text.Trim))
                    End If

                    ' 2. จัดการรูปภาพใบเสร็จ (ถ้ามีการเลือกใหม่ หรือวางจากคลิปบอร์ด)
                    If Not String.IsNullOrEmpty(selectedSourceReceiptPath) Then
                        Dim savedFileName As String = SaveReceiptFile(targetID)
                        If Not String.IsNullOrEmpty(savedFileName) Then
                            Using cmd = conn.CreateCommand()
                                cmd.CommandText = "UPDATE Transactions SET ReceiptPath = @rp WHERE ID = @id"
                                cmd.Parameters.AddWithValue("@rp", savedFileName)
                                cmd.Parameters.AddWithValue("@id", targetID)
                                cmd.ExecuteNonQuery()
                            End Using
                        End If
                    End If

                    MessageBox.Show("✅ บันทึกข้อมูลเรียบร้อยแล้ว!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    If _editId > 0 Then
                        ' ถ้าเป็นการแก้ไข ให้ปิดหน้าจอนี้และกลับไปหน้า Transactions
                        Dim f = TryCast(Me.ParentForm, frmMain)
                        If f IsNot Nothing Then
                            f.btnMember.PerformClick()
                        End If
                    Else
                        ResetEntry(False)
                        ' ล้างค่าตัวแปรเก็บ Path รูปภาพที่เคยเลือกไว้
                        selectedSourceReceiptPath = ""
                        If txtReceipt IsNot Nothing Then txtReceipt.Clear()

                        ' ล้าง Clipboard หลังบันทึกรูปสำเร็จ
                        Try
                            Clipboard.Clear()
                        Catch ex As Exception
                            AppPaths.LogCrash(ex, "FrmIncome.ClearClipboard")
                        End Try

                        dtpDate.Value = keepDate
                        dtpDate.Focus()
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("บันทึกไม่สำเร็จ: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ' --- ฟังก์ชันช่วยก๊อบปี้รูปภาพใบเสร็จไปยัง AppPaths.ReceiptsDir โดยการย่อขนาดและบีบอัด ---
        Private Function SaveReceiptFile(transactionID As Integer) As String
            If isImageFromClipboard Then
                If clipboardImage Is Nothing Then Return ""
                Return ReceiptImageHelper.SaveOptimizedReceipt(clipboardImage, transactionID)
            End If

            If String.IsNullOrWhiteSpace(selectedSourceReceiptPath) OrElse Not IO.File.Exists(selectedSourceReceiptPath) Then
                Return ""
            End If

            Return ReceiptImageHelper.SaveOptimizedReceipt(selectedSourceReceiptPath, transactionID)
        End Function

        Private Sub btnImportExcel_Click(sender As Object, e As EventArgs) Handles btnImportExcel.Click
            If cboCategory.SelectedValue Is Nothing OrElse cboFund.SelectedValue Is Nothing Then
                MessageBox.Show("กรุณาเลือกประเภท และกองทุนเริ่มต้นก่อนนำเข้า", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Using ofd As New OpenFileDialog()
                ofd.Title = "เลือกไฟล์ Excel สำหรับนำเข้ารายรับ"
                ofd.Filter = "Excel Files|*.xlsx;*.xlsm;*.xlsb;*.xls|All Files|*.*"
                If ofd.ShowDialog(Me) <> DialogResult.OK Then Return

                Try
                    Dim defaultBankId = GetSelectedBankIdOrNothing()
                    Dim result = ImportTransactionsFromExcel(ofd.FileName, "Income", CInt(cboCategory.SelectedValue), CInt(cboFund.SelectedValue), defaultBankId)
                    LoadMasters()
                    ResetEntry(False)
                    dtpDate.Focus()
                    MessageBox.Show(result.BuildSummaryMessage("รายรับ"), "นำเข้าข้อมูลสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("นำเข้าข้อมูลไม่สำเร็จ: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Sub

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

        Private Sub btnPasteReceipt_Click(sender As Object, e As EventArgs)
            If Clipboard.ContainsImage() Then
                clipboardImage = Clipboard.GetImage()
                isImageFromClipboard = True
                selectedSourceReceiptPath = "Clipboard_Image"
                txtReceipt.Text = "[รูปภาพจากคลิปบอร์ด/LINE]"
                btnSave.Focus()
            Else
                MessageBox.Show("ไม่พบรูปภาพใหม่ใน Clipboard กรุณาไปที่ LINE แล้วกด Copy รูปภาพใบเสร็จรูปใหม่ก่อนกดปุ่มนี้", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End Sub

        Private Sub btnClearReceipt_Click(sender As Object, e As EventArgs) Handles btnClearReceipt.Click
            selectedSourceReceiptPath = ""
            isImageFromClipboard = False
            If clipboardImage IsNot Nothing Then clipboardImage.Dispose()
            clipboardImage = Nothing
            txtReceipt.Clear()

            ' ล้าง Clipboard เพื่อป้องกันการวางรูปเดิมซ้ำ
            Try
                Clipboard.Clear()
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmIncome.ClearReceipt.ClearClipboard")
            End Try
        End Sub

        Private Sub txtAmount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtAmount.KeyPress
            If Char.IsControl(e.KeyChar) Then Return
            Dim tb = DirectCast(sender, TextBox)
            If e.KeyChar = "."c AndAlso tb.Text.Contains(".") Then e.Handled = True : Return
            If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then e.Handled = True
        End Sub
    End Class
End Namespace
