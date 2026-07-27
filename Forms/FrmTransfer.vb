Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data.OleDb

Namespace TempleAccounting
    Partial Public Class FrmTransfer
        Inherits Form

        Private Const EmptySelectionId As Integer = 0

        Private components As IContainer = Nothing
        Private ReadOnly _enterFlow As New List(Of Control)()
        Friend WithEvents dtpDate As DateTimePicker
        Friend WithEvents cboFromFund As ComboBox, cboFromBank As ComboBox
        Friend WithEvents cboToFund As ComboBox, cboToBank As ComboBox
        Friend WithEvents txtAmount As TextBox, txtRemark As TextBox
        Friend WithEvents btnSave As Button, btnCancel As Button, lblHeader As Label

        Private l1, l2, l3, l4, l5, l6 As Label

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.Text = "โอนเงินภายใน"
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.FormBorderStyle = FormBorderStyle.None
            Me.Dock = DockStyle.Fill
            Me.AutoScroll = True

            lblHeader = New Label()
            lblHeader.Text = "🔁 โอนเงินภายในระหว่างกองทุน/บัญชี"
            lblHeader.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(88, 28, 135)
            lblHeader.BackColor = Color.FromArgb(233, 213, 255)
            lblHeader.Dock = DockStyle.Top : lblHeader.Height = 70
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            dtpDate = New DateTimePicker With {.Font = New Font("Tahoma", 10.5!), .Value = Today}
            cboFromFund = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.5!)}
            cboFromBank = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.5!)}
            cboToFund = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.5!)}
            cboToBank = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.5!)}
            txtAmount = New TextBox With {.Font = New Font("Tahoma", 11.5!, FontStyle.Bold), .ForeColor = Color.FromArgb(88, 28, 135), .TextAlign = HorizontalAlignment.Right}
            txtRemark = New TextBox With {.Font = New Font("Tahoma", 10.5!), .Multiline = True, .ScrollBars = ScrollBars.Vertical}
            btnSave = New Button With {.Text = "💾 บันทึกการโอน", .ForeColor = Color.White, .BackColor = Color.FromArgb(126, 34, 206), .FlatStyle = FlatStyle.Flat, .Size = New Size(260, 56), .Font = New Font("Tahoma", 11.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnCancel = New Button With {.Text = "❌ เคลียร์", .ForeColor = Color.White, .BackColor = Color.FromArgb(75, 85, 99), .FlatStyle = FlatStyle.Flat, .Size = New Size(180, 56), .Font = New Font("Tahoma", 11.0!, FontStyle.Bold), .Cursor = Cursors.Hand}

            Dim y As Integer = 110, lx = 40, tx = 260, fw = 520, gap = 20
            l1 = MakeLbl("วันที่โอน:", New Point(lx, y))
            dtpDate.Location = New Point(tx, y) : dtpDate.Size = New Size(fw, 40)
            y += 62
            l2 = MakeLbl("จากกองทุน:", New Point(lx, y))
            cboFromFund.Location = New Point(tx, y) : cboFromFund.Size = New Size(fw, 40)
            y += 62
            l3 = MakeLbl("จากบัญชีธนาคาร:", New Point(lx, y))
            cboFromBank.Location = New Point(tx, y) : cboFromBank.Size = New Size(fw, 40)
            y += 62
            l4 = MakeLbl("ไปยังกองทุน:", New Point(lx, y))
            cboToFund.Location = New Point(tx, y) : cboToFund.Size = New Size(fw, 40)
            y += 62
            l5 = MakeLbl("ไปยังบัญชี:", New Point(lx, y))
            cboToBank.Location = New Point(tx, y) : cboToBank.Size = New Size(fw, 40)
            y += 62
            l6 = MakeLbl("จำนวนเงินที่โอน:", New Point(lx, y))
            txtAmount.Location = New Point(tx, y) : txtAmount.Size = New Size(260, 40)
            y += 62
            Dim lr = MakeLbl("เหตุผลการโอน:", New Point(lx, y))
            txtRemark.Location = New Point(tx, y) : txtRemark.Size = New Size(fw, 100)
            y += 130
            btnSave.Location = New Point(tx, y)
            btnCancel.Location = New Point(tx + 280, y)

            Me.Controls.AddRange(New Control() {lblHeader, l1, l2, l3, l4, l5, l6, lr,
                dtpDate, cboFromFund, cboFromBank, cboToFund, cboToBank, txtAmount, txtRemark,
                btnSave, btnCancel})
        End Sub

        Private Function MakeLbl(t As String, p As Point) As Label
            Return New Label With {.Text = t, .Location = p, .Size = New Size(210, 40), .TextAlign = ContentAlignment.MiddleRight, .Font = New Font("Tahoma", 10.5!)}
        End Function

        Private Sub FrmTransfer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Db.EnsureSchema()
            Using conn = Db.OpenConn()
                Dim funds = Db.GetTable(conn, "SELECT ID, FundName FROM Funds ORDER BY FundName")
                AddBlankOption(funds, "FundName")
                cboFromFund.DisplayMember = "FundName" : cboFromFund.ValueMember = "ID" : cboFromFund.DataSource = funds.Copy()
                cboToFund.DisplayMember = "FundName" : cboToFund.ValueMember = "ID" : cboToFund.DataSource = funds

                Dim banks = Db.GetTable(conn, "SELECT ID, BankName & '  ' & IIF(AccountNo IS NULL,'',AccountNo) & '  (' & IIF(AccountName IS NULL,'',AccountName) & ')' AS Disp FROM BankAccounts ORDER BY BankName")
                AddBlankOption(banks, "Disp")
                cboFromBank.DisplayMember = "Disp" : cboFromBank.ValueMember = "ID" : cboFromBank.DataSource = banks.Copy()
                cboToBank.DisplayMember = "Disp" : cboToBank.ValueMember = "ID" : cboToBank.DataSource = banks
            End Using
            SetupEnterNavigation()
            ResetEntry(True)
            FocusStartField()
        End Sub

        Private Sub AddBlankOption(table As DataTable, displayColumn As String)
            Dim row = table.NewRow()
            row("ID") = EmptySelectionId
            row(displayColumn) = ""
            table.Rows.InsertAt(row, 0)
        End Sub

        Private Sub ResetEntry(resetToToday As Boolean)
            If resetToToday Then
                dtpDate.Value = Today
            End If
            If cboFromFund.Items.Count > 0 Then cboFromFund.SelectedIndex = 0
            If cboFromBank.Items.Count > 0 Then cboFromBank.SelectedIndex = 0
            If cboToFund.Items.Count > 0 Then cboToFund.SelectedIndex = 0
            If cboToBank.Items.Count > 0 Then cboToBank.SelectedIndex = 0
            txtAmount.Clear()
            txtRemark.Clear()
        End Sub

        Private Sub SetupEnterNavigation()
            If _enterFlow.Count > 0 Then Return
            _enterFlow.AddRange({dtpDate, cboFromFund, cboFromBank, cboToFund, cboToBank, txtAmount, txtRemark, btnSave})
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
            If cb IsNot Nothing AndAlso cb.Items.Count > 0 Then cb.DroppedDown = True

            Dim tb = TryCast(nextCtrl, TextBox)
            If tb IsNot Nothing Then tb.SelectAll()
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
            Dim fromFundId = SelectedIdOrZero(cboFromFund)
            Dim fromBankId = SelectedIdOrZero(cboFromBank)
            Dim toFundId = SelectedIdOrZero(cboToFund)
            Dim toBankId = SelectedIdOrZero(cboToBank)

            If fromFundId = EmptySelectionId AndAlso fromBankId = EmptySelectionId Then
                MessageBox.Show("กรุณาเลือกต้นทางอย่างน้อย 1 ช่อง เช่น กองทุนหรือบัญชีธนาคาร", "แจ้งเตือน") : Return
            End If
            If toFundId = EmptySelectionId AndAlso toBankId = EmptySelectionId Then
                MessageBox.Show("กรุณาเลือกปลายทางอย่างน้อย 1 ช่อง เช่น กองทุนหรือบัญชีธนาคาร", "แจ้งเตือน") : Return
            End If
            If fromFundId = toFundId AndAlso fromBankId = toBankId Then
                MessageBox.Show("ต้นทางและปลายทางต้องแตกต่างกัน", "แจ้งเตือน") : Return
            End If
            Dim amt As Decimal
            If Not Decimal.TryParse(txtAmount.Text, amt) OrElse amt <= 0 Then
                MessageBox.Show("กรุณาใส่จำนวนเงิน", "แจ้งเตือน") : Return
            End If
            Dim keepDate = dtpDate.Value.Date
            Try
                Using conn = Db.OpenConn()
                    Db.ExecuteNonQuery(conn,
"INSERT INTO Transactions (TranDate, TranType, FundID, BankID, ToFundID, ToBankID, [Detail], Amount, [Note]) VALUES (" & Db.AccessDateLiteral(dtpDate.Value.Date) & ",'Transfer',@ff,@fb,@tf,@tb,@de,@a,@n)",
New Tuple(Of String, Object)("@ff", ToDbNullableId(fromFundId)),
New Tuple(Of String, Object)("@fb", ToDbNullableId(fromBankId)),
New Tuple(Of String, Object)("@tf", ToDbNullableId(toFundId)),
New Tuple(Of String, Object)("@tb", ToDbNullableId(toBankId)),
New Tuple(Of String, Object)("@de", BuildTransferDetail(fromFundId, fromBankId, toFundId, toBankId)),
New Tuple(Of String, Object)("@a", amt),
New Tuple(Of String, Object)("@n", txtRemark.Text.Trim))
                    MessageBox.Show("✅ โอนเงินภายในสำเร็จ!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    ResetEntry(False)
                    dtpDate.Value = keepDate
                    dtpDate.Focus()
                End Using
            Catch ex As Exception
                MessageBox.Show("ผิดพลาด: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub txtAmount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtAmount.KeyPress
            If Char.IsControl(e.KeyChar) Then Return
            If e.KeyChar = "."c AndAlso txtAmount.Text.Contains(".") Then e.Handled = True : Return
            If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then e.Handled = True
        End Sub

        Private Function SelectedIdOrZero(cbo As ComboBox) As Integer
            If cbo.SelectedValue Is Nothing OrElse cbo.SelectedValue Is DBNull.Value Then Return EmptySelectionId
            Return Convert.ToInt32(cbo.SelectedValue)
        End Function

        Private Function ToDbNullableId(value As Integer) As Object
            If value = EmptySelectionId Then Return DBNull.Value
            Return value
        End Function

        Private Function BuildTransferDetail(fromFundId As Integer, fromBankId As Integer, toFundId As Integer, toBankId As Integer) As String
            Dim sourceParts As New List(Of String)()
            Dim destParts As New List(Of String)()

            If fromFundId <> EmptySelectionId AndAlso cboFromFund.Text.Trim() <> "" Then sourceParts.Add("กองทุน " & cboFromFund.Text.Trim())
            If fromBankId <> EmptySelectionId AndAlso cboFromBank.Text.Trim() <> "" Then sourceParts.Add("บัญชี " & cboFromBank.Text.Trim())
            If toFundId <> EmptySelectionId AndAlso cboToFund.Text.Trim() <> "" Then destParts.Add("กองทุน " & cboToFund.Text.Trim())
            If toBankId <> EmptySelectionId AndAlso cboToBank.Text.Trim() <> "" Then destParts.Add("บัญชี " & cboToBank.Text.Trim())

            Return "โอนเงินภายใน: " & String.Join(" / ", sourceParts) & " -> " & String.Join(" / ", destParts)
        End Function
    End Class
End Namespace
