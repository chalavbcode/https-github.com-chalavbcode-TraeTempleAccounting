Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data.OleDb

Namespace TempleAccounting
    Partial Public Class FrmExpense
        Inherits Form

        Private components As IContainer = Nothing
        Private ReadOnly _enterFlow As New List(Of Control)()
        Friend WithEvents dtpDate As DateTimePicker
        Friend WithEvents cboCategory As ComboBox
        Friend WithEvents cboFund As ComboBox
        Friend WithEvents cboBank As ComboBox
        Friend WithEvents txtDescription As TextBox
        Friend WithEvents txtAmount As TextBox
        Friend WithEvents txtRemark As TextBox
        Friend WithEvents btnSave As Button
        Friend WithEvents btnCancel As Button
        Friend WithEvents btnImportExcel As Button
        Friend WithEvents lblHeader As Label

        Private lbl1, lbl2, lbl3, lbl4, lbl5, lbl6, lbl7 As Label

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.lblHeader = New Label()
            Me.lbl1 = New Label() : Me.lbl2 = New Label() : Me.lbl3 = New Label()
            Me.lbl4 = New Label() : Me.lbl5 = New Label() : Me.lbl6 = New Label() : Me.lbl7 = New Label()
            Me.dtpDate = New DateTimePicker()
            Me.cboCategory = New ComboBox()
            Me.cboFund = New ComboBox()
            Me.cboBank = New ComboBox()
            Me.txtDescription = New TextBox()
            Me.txtAmount = New TextBox()
            Me.txtRemark = New TextBox()
            Me.btnSave = New Button()
            Me.btnCancel = New Button()
            Me.btnImportExcel = New Button()

            Me.SuspendLayout()
            Me.Text = "บันทึกรายจ่าย"
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.FormBorderStyle = FormBorderStyle.None
            Me.Dock = DockStyle.Fill
            Me.AutoScroll = True

            Me.lblHeader.Text = "💸 บันทึกรายจ่ายของวัด"
            Me.lblHeader.Font = New Font("Tahoma", 15.0!, FontStyle.Bold)
            Me.lblHeader.ForeColor = Color.FromArgb(127, 29, 29)
            Me.lblHeader.BackColor = Color.FromArgb(254, 202, 202)
            Me.lblHeader.Dock = DockStyle.Top
            Me.lblHeader.Height = 70
            Me.lblHeader.TextAlign = ContentAlignment.MiddleCenter
            Me.lblHeader.Padding = New Padding(0, 8, 0, 8)

            Dim yBase As Integer = 110
            Dim lx As Integer = 40
            Dim tx As Integer = 240
            Dim fw As Integer = 520
            Dim fh As Integer = 40
            Dim gap As Integer = 22

            Me.lbl1.Text = "วันที่ทำรายการ:" : Me.lbl1.Location = New Point(lx, yBase)
            Me.lbl1.Size = New Size(190, 40) : Me.lbl1.TextAlign = ContentAlignment.MiddleRight
            Me.dtpDate.Location = New Point(tx, yBase) : Me.dtpDate.Size = New Size(fw, 40)
            Me.dtpDate.Font = New Font("Tahoma", 10.5!) : Me.dtpDate.Value = Today

            yBase += fh + gap
            Me.lbl2.Text = "ประเภทรายจ่าย:" : Me.lbl2.Location = New Point(lx, yBase)
            Me.lbl2.Size = New Size(190, 40) : Me.lbl2.TextAlign = ContentAlignment.MiddleRight
            Me.cboCategory.Location = New Point(tx, yBase) : Me.cboCategory.Size = New Size(fw, 40)
            Me.cboCategory.Font = New Font("Tahoma", 10.5!) : Me.cboCategory.DropDownStyle = ComboBoxStyle.DropDownList

            yBase += fh + gap
            Me.lbl3.Text = "กองทุน:" : Me.lbl3.Location = New Point(lx, yBase)
            Me.lbl3.Size = New Size(190, 40) : Me.lbl3.TextAlign = ContentAlignment.MiddleRight
            Me.cboFund.Location = New Point(tx, yBase) : Me.cboFund.Size = New Size(fw, 40)
            Me.cboFund.Font = New Font("Tahoma", 10.5!) : Me.cboFund.DropDownStyle = ComboBoxStyle.DropDownList

            yBase += fh + gap
            Me.lbl4.Text = "บัญชีธนาคาร (ถ้ามี):" : Me.lbl4.Location = New Point(lx, yBase)
            Me.lbl4.Size = New Size(190, 40) : Me.lbl4.TextAlign = ContentAlignment.MiddleRight
            Me.cboBank.Location = New Point(tx, yBase) : Me.cboBank.Size = New Size(fw, 40)
            Me.cboBank.Font = New Font("Tahoma", 10.5!) : Me.cboBank.DropDownStyle = ComboBoxStyle.DropDownList

            yBase += fh + gap
            Me.lbl5.Text = "รายละเอียดรายการ:" : Me.lbl5.Location = New Point(lx, yBase)
            Me.lbl5.Size = New Size(190, 40) : Me.lbl5.TextAlign = ContentAlignment.MiddleRight
            Me.txtDescription.Location = New Point(tx, yBase) : Me.txtDescription.Size = New Size(fw, 40)
            Me.txtDescription.Font = New Font("Tahoma", 10.5!)

            yBase += fh + gap
            Me.lbl6.Text = "จำนวนเงิน (บาท):" : Me.lbl6.Location = New Point(lx, yBase)
            Me.lbl6.Size = New Size(190, 40) : Me.lbl6.TextAlign = ContentAlignment.MiddleRight
            Me.txtAmount.Location = New Point(tx, yBase) : Me.txtAmount.Size = New Size(260, 40)
            Me.txtAmount.Font = New Font("Tahoma", 11.5!, FontStyle.Bold)
            Me.txtAmount.TextAlign = HorizontalAlignment.Right
            Me.txtAmount.ForeColor = Color.FromArgb(153, 27, 27)

            yBase += fh + gap
            Me.lbl7.Text = "หมายเหตุ:" : Me.lbl7.Location = New Point(lx, yBase)
            Me.lbl7.Size = New Size(190, 40) : Me.lbl7.TextAlign = ContentAlignment.MiddleRight
            Me.txtRemark.Location = New Point(tx, yBase) : Me.txtRemark.Size = New Size(fw, 100)
            Me.txtRemark.Font = New Font("Tahoma", 10.5!)
            Me.txtRemark.Multiline = True : Me.txtRemark.ScrollBars = ScrollBars.Vertical

            yBase += 120
            Me.btnSave.Text = "💾 บันทึกรายการ"
            Me.btnSave.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.btnSave.BackColor = Color.FromArgb(180, 83, 9)
            Me.btnSave.ForeColor = Color.White
            Me.btnSave.FlatStyle = FlatStyle.Flat
            Me.btnSave.Size = New Size(240, 56)
            Me.btnSave.Location = New Point(tx, yBase)
            Me.btnSave.Cursor = Cursors.Hand

            Me.btnCancel.Text = "❌ เคลียร์"
            Me.btnCancel.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.btnCancel.BackColor = Color.FromArgb(75, 85, 99)
            Me.btnCancel.ForeColor = Color.White
            Me.btnCancel.FlatStyle = FlatStyle.Flat
            Me.btnCancel.Size = New Size(180, 56)
            Me.btnCancel.Location = New Point(tx + 260, yBase)
            Me.btnCancel.Cursor = Cursors.Hand

            Me.btnImportExcel.Text = "📥 นำเข้าจาก Excel"
            Me.btnImportExcel.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.btnImportExcel.BackColor = Color.FromArgb(37, 99, 235)
            Me.btnImportExcel.ForeColor = Color.White
            Me.btnImportExcel.FlatStyle = FlatStyle.Flat
            Me.btnImportExcel.Size = New Size(240, 56)
            Me.btnImportExcel.Location = New Point(tx + 460, yBase)
            Me.btnImportExcel.Cursor = Cursors.Hand

            Me.Controls.AddRange(New Control() {lbl1, lbl2, lbl3, lbl4, lbl5, lbl6, lbl7,
                dtpDate, cboCategory, cboFund, cboBank, txtDescription, txtAmount, txtRemark,
                btnSave, btnCancel, btnImportExcel, lblHeader})

            Me.ResumeLayout(False)
        End Sub

        Private Sub FrmExpense_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Db.EnsureSchema()
            LoadMasters()
            SetupEnterNavigation()
            ResetEntry(True)
            FocusStartField()
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

            _enterFlow.AddRange({dtpDate, cboCategory, cboFund, cboBank, txtDescription, txtAmount, txtRemark, btnSave})

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
                    Db.InsertAndGetId(conn,
"INSERT INTO Transactions (TranDate, TranType, CategoryID, FundID, BankID, [Detail], Amount, [Note]) VALUES (" & Db.AccessDateLiteral(dtpDate.Value.Date) & ",'Expense',@c,@f,@b,@de,@a,@n)",
New Tuple(Of String, Object)("@c", CInt(cboCategory.SelectedValue)),
New Tuple(Of String, Object)("@f", CInt(cboFund.SelectedValue)),
New Tuple(Of String, Object)("@b", bankValue),
New Tuple(Of String, Object)("@de", txtDescription.Text.Trim),
New Tuple(Of String, Object)("@a", amt),
New Tuple(Of String, Object)("@n", txtRemark.Text.Trim))
                    MessageBox.Show("✅ บันทึกรายจ่ายสำเร็จ!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ResetEntry(False)
                    dtpDate.Value = keepDate
                    dtpDate.Focus()
                End Using
            Catch ex As Exception
                MessageBox.Show("บันทึกไม่สำเร็จ: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

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

        Private Sub txtAmount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtAmount.KeyPress
            If Char.IsControl(e.KeyChar) Then Return
            If e.KeyChar = "."c AndAlso txtAmount.Text.Contains(".") Then e.Handled = True : Return
            If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then e.Handled = True
        End Sub
    End Class
End Namespace
