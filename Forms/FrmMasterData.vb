Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.OleDb

Namespace TempleAccounting
    Partial Public Class FrmMasterData
        Inherits Form

        Private components As IContainer = Nothing
        Private ReadOnly _categoryFlow As New List(Of Control)()
        Private ReadOnly _fundFlow As New List(Of Control)()
        Private ReadOnly _bankFlow As New List(Of Control)()
        Friend WithEvents TabControl1 As TabControl
        Friend WithEvents tpCategory, tpFund, tpBank As TabPage
        Friend WithEvents lblHeader As Label
        ' Category
        Friend WithEvents dgvCategory, dgvFund, dgvBank As DataGridView
        Friend WithEvents txtCatName As TextBox, cboCatType As ComboBox
        Friend WithEvents btnCatAdd, btnCatEdit, btnCatDel As Button
        Friend WithEvents lblCatName, lblCatType As Label
        ' Fund
        Friend WithEvents txtFundName As TextBox
        Friend WithEvents btnFundAdd, btnFundDel As Button
        Friend WithEvents lblFundName As Label
        ' Bank
        Friend WithEvents txtBankName, txtBankAccountNo, txtBankAccountName As TextBox
        Friend WithEvents btnBankAdd, btnBankDel As Button
        Friend WithEvents lblBankName, lblBankAccountNo, lblBankAccountName As Label

        Private selCatId As Integer = -1, selFundId As Integer = -1, selBankId As Integer = -1

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.Text = "จัดการข้อมูลหลัก"
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.FormBorderStyle = FormBorderStyle.None
            Me.Dock = DockStyle.Fill
            Me.AutoScroll = True

            lblHeader = New Label()
            lblHeader.Text = "🗂️ จัดการข้อมูลหลัก ประเภทรายการ / กองทุน / บัญชีธนาคาร"
            lblHeader.Font = New Font("Tahoma", 13.5!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(69, 26, 3)
            lblHeader.BackColor = Color.FromArgb(253, 230, 138)
            lblHeader.Dock = DockStyle.Top : lblHeader.Height = 62
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            TabControl1 = New TabControl()
            TabControl1.Dock = DockStyle.Fill
            TabControl1.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            tpCategory = New TabPage("ประเภทรายการ (Category)")
            tpFund = New TabPage("กองทุน (Funds)")
            tpBank = New TabPage("บัญชีธนาคาร (Bank Accounts)")
            tpCategory.BackColor = Color.FromArgb(254, 249, 235)
            tpFund.BackColor = Color.FromArgb(254, 249, 235)
            tpBank.BackColor = Color.FromArgb(254, 249, 235)
            TabControl1.TabPages.AddRange({tpCategory, tpFund, tpBank})
            TabControl1.Dock = DockStyle.Fill

            ' ===== Category Tab =====
            dgvCategory = New DataGridView With {.Location = New Point(16, 110), .Size = New Size(880, 420), .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .ReadOnly = True, .AllowUserToAddRows = False, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .BackgroundColor = Color.White, .Font = New Font("Tahoma", 10.0!), .BorderStyle = BorderStyle.None, .RowTemplate = New DataGridViewRow() With {.Height = 32}}
            dgvCategory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235)

            lblCatName = New Label With {.Text = "ชื่อประเภท:", .Location = New Point(16, 20), .AutoSize = True, .ForeColor = Color.FromArgb(69, 26, 3)}
            txtCatName = New TextBox With {.Location = New Point(130, 16), .Size = New Size(440, 40), .Font = New Font("Tahoma", 10.5!)}
            lblCatType = New Label With {.Text = "ชนิด:", .Location = New Point(590, 20), .AutoSize = True}
            cboCatType = New ComboBox With {.Location = New Point(650, 16), .Size = New Size(180, 40), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.0!)}
            cboCatType.Items.AddRange({"Income (รายรับ)", "Expense (รายจ่าย)"}) : cboCatType.SelectedIndex = 0
            btnCatAdd = New Button With {.Text = "➕ เพิ่ม", .Location = New Point(16, 60), .Size = New Size(140, 44), .BackColor = Color.FromArgb(22, 163, 74), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnCatEdit = New Button With {.Text = "📝 แก้ไข", .Location = New Point(166, 60), .Size = New Size(140, 44), .BackColor = Color.FromArgb(217, 119, 6), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnCatDel = New Button With {.Text = "🗑️ ลบที่เลือก", .Location = New Point(316, 60), .Size = New Size(160, 44), .BackColor = Color.FromArgb(153, 27, 27), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            tpCategory.Controls.AddRange(New Control() {dgvCategory, lblCatName, txtCatName, lblCatType, cboCatType, btnCatAdd, btnCatEdit, btnCatDel})

            ' ===== Fund Tab =====
            dgvFund = New DataGridView With {.Location = New Point(16, 100), .Size = New Size(880, 420), .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .ReadOnly = True, .AllowUserToAddRows = False, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .BackgroundColor = Color.White, .Font = New Font("Tahoma", 10.0!), .BorderStyle = BorderStyle.None, .RowTemplate = New DataGridViewRow() With {.Height = 32}}
            dgvFund.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235)
            lblFundName = New Label With {.Text = "ชื่อกองทุน:", .Location = New Point(16, 20), .AutoSize = True, .ForeColor = Color.FromArgb(69, 26, 3)}
            txtFundName = New TextBox With {.Location = New Point(140, 16), .Size = New Size(520, 40), .Font = New Font("Tahoma", 10.5!)}
            btnFundAdd = New Button With {.Text = "➕ เพิ่ม/อัปเดต", .Location = New Point(16, 54), .Size = New Size(200, 44), .BackColor = Color.FromArgb(22, 163, 74), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnFundDel = New Button With {.Text = "🗑️ ลบที่เลือก", .Location = New Point(226, 54), .Size = New Size(180, 44), .BackColor = Color.FromArgb(153, 27, 27), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            tpFund.Controls.AddRange(New Control() {dgvFund, lblFundName, txtFundName, btnFundAdd, btnFundDel})

            ' ===== Bank Tab =====
            dgvBank = New DataGridView With {.Location = New Point(16, 130), .Size = New Size(880, 390), .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .ReadOnly = True, .AllowUserToAddRows = False, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .BackgroundColor = Color.White, .Font = New Font("Tahoma", 10.0!), .BorderStyle = BorderStyle.None, .RowTemplate = New DataGridViewRow() With {.Height = 32}}
            dgvBank.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235)
            lblBankName = New Label With {.Text = "ชื่อธนาคาร:", .Location = New Point(16, 16), .AutoSize = True, .ForeColor = Color.FromArgb(69, 26, 3)}
            txtBankName = New TextBox With {.Location = New Point(150, 12), .Size = New Size(480, 40), .Font = New Font("Tahoma", 10.5!)}
            lblBankAccountNo = New Label With {.Text = "เลขที่บัญชี:", .Location = New Point(16, 58), .AutoSize = True}
            txtBankAccountNo = New TextBox With {.Location = New Point(150, 54), .Size = New Size(360, 40), .Font = New Font("Tahoma", 10.5!)}
            lblBankAccountName = New Label With {.Text = "ชื่อบัญชี:", .Location = New Point(526, 58), .AutoSize = True}
            txtBankAccountName = New TextBox With {.Location = New Point(630, 54), .Size = New Size(340, 40), .Font = New Font("Tahoma", 10.5!)}
            btnBankAdd = New Button With {.Text = "➕ เพิ่ม/อัปเดต", .Location = New Point(16, 90), .Size = New Size(200, 40), .BackColor = Color.FromArgb(22, 163, 74), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnBankDel = New Button With {.Text = "🗑️ ลบบัญชีที่เลือก", .Location = New Point(226, 90), .Size = New Size(220, 40), .BackColor = Color.FromArgb(153, 27, 27), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            tpBank.Controls.AddRange(New Control() {dgvBank, lblBankName, txtBankName, lblBankAccountNo, txtBankAccountNo, lblBankAccountName, txtBankAccountName, btnBankAdd, btnBankDel})

            Me.Controls.Add(TabControl1)
            Me.Controls.Add(lblHeader)
        End Sub

        Private Sub FrmMasterData_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Db.EnsureSchema()
            ReloadAll()
            SetupEnterNavigation()
            txtCatName.Focus()
        End Sub

        Private Sub SetupEnterNavigation()
            If _categoryFlow.Count = 0 Then
                _categoryFlow.AddRange({txtCatName, cboCatType, btnCatAdd})
                For Each ctrl In _categoryFlow
                    AddHandler ctrl.KeyDown, AddressOf HandleCategoryEnterAdvance
                Next
            End If

            If _fundFlow.Count = 0 Then
                _fundFlow.AddRange({txtFundName, btnFundAdd})
                For Each ctrl In _fundFlow
                    AddHandler ctrl.KeyDown, AddressOf HandleFundEnterAdvance
                Next
            End If

            If _bankFlow.Count = 0 Then
                _bankFlow.AddRange({txtBankName, txtBankAccountNo, txtBankAccountName, btnBankAdd})
                For Each ctrl In _bankFlow
                    AddHandler ctrl.KeyDown, AddressOf HandleBankEnterAdvance
                Next
            End If
        End Sub

        Private Sub MoveNextFrom(flow As List(Of Control), current As Control, submitButton As Button)
            Dim idx = flow.IndexOf(current)
            If idx < 0 Then Return
            If idx = flow.Count - 1 Then
                submitButton.PerformClick()
                Return
            End If

            Dim nextCtrl = flow(idx + 1)
            nextCtrl.Focus()
            Dim cb = TryCast(nextCtrl, ComboBox)
            If cb IsNot Nothing AndAlso cb.Items.Count > 0 Then cb.DroppedDown = True
            Dim tb = TryCast(nextCtrl, TextBox)
            If tb IsNot Nothing Then tb.SelectAll()
        End Sub

        Private Sub HandleCategoryEnterAdvance(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True
            MoveNextFrom(_categoryFlow, DirectCast(sender, Control), btnCatAdd)
        End Sub

        Private Sub HandleFundEnterAdvance(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True
            MoveNextFrom(_fundFlow, DirectCast(sender, Control), btnFundAdd)
        End Sub

        Private Sub HandleBankEnterAdvance(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True
            MoveNextFrom(_bankFlow, DirectCast(sender, Control), btnBankAdd)
        End Sub

        Private Sub ReloadAll()
            ReloadCategory()
            ReloadFund()
            ReloadBank()
        End Sub

        Private Sub ReloadCategory()
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT ID, CategoryName AS ชื่อประเภท, TranType AS ชนิด FROM Categories ORDER BY TranType, CategoryName")
                dgvCategory.DataSource = dt
                If dgvCategory.Columns.Contains("ID") Then dgvCategory.Columns("ID").Visible = False
            End Using
        End Sub
        Private Sub ReloadFund()
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT ID, FundName AS ชื่อกองทุน FROM Funds ORDER BY FundName")
                dgvFund.DataSource = dt
                If dgvFund.Columns.Contains("ID") Then dgvFund.Columns("ID").Visible = False
            End Using
        End Sub
        Private Sub ReloadBank()
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT ID, BankName AS ธนาคาร, AccountNo AS เลขบัญชี, AccountName AS ชื่อบัญชี FROM BankAccounts ORDER BY BankName")
                dgvBank.DataSource = dt
                If dgvBank.Columns.Contains("ID") Then dgvBank.Columns("ID").Visible = False
            End Using
        End Sub

        Private Sub dgvCategory_SelectionChanged(sender As Object, e As EventArgs) Handles dgvCategory.SelectionChanged
            If dgvCategory.CurrentRow Is Nothing Then Return
            Dim row = DirectCast(dgvCategory.CurrentRow.DataBoundItem, DataRowView).Row
            selCatId = CInt(row("ID"))
            txtCatName.Text = row("ชื่อประเภท").ToString()
            If row("ชนิด").ToString() = "Income" Then cboCatType.SelectedIndex = 0 Else cboCatType.SelectedIndex = 1
        End Sub
        Private Sub dgvFund_SelectionChanged(sender As Object, e As EventArgs) Handles dgvFund.SelectionChanged
            If dgvFund.CurrentRow Is Nothing Then Return
            Dim row = DirectCast(dgvFund.CurrentRow.DataBoundItem, DataRowView).Row
            selFundId = CInt(row("ID"))
            txtFundName.Text = row("ชื่อกองทุน").ToString()
        End Sub
        Private Sub dgvBank_SelectionChanged(sender As Object, e As EventArgs) Handles dgvBank.SelectionChanged
            If dgvBank.CurrentRow Is Nothing Then Return
            Dim row = DirectCast(dgvBank.CurrentRow.DataBoundItem, DataRowView).Row
            selBankId = CInt(row("ID"))
            txtBankName.Text = row("ธนาคาร").ToString()
            txtBankAccountNo.Text = row("เลขบัญชี").ToString()
            txtBankAccountName.Text = row("ชื่อบัญชี").ToString()
        End Sub

        Private Sub btnCatAdd_Click(sender As Object, e As EventArgs) Handles btnCatAdd.Click
            If String.IsNullOrWhiteSpace(txtCatName.Text) Then MessageBox.Show("กรุณาใส่ชื่อประเภท", "แจ้งเตือน") : Return
            Dim tt = If(cboCatType.SelectedIndex = 0, "Income", "Expense")
            Using conn = Db.OpenConn()
                Db.ExecuteNonQuery(conn, "INSERT INTO Categories (CategoryName, TranType) VALUES (@n,@t)",
                                   New Tuple(Of String, Object)("@n", txtCatName.Text.Trim()),
                                   New Tuple(Of String, Object)("@t", tt))
            End Using
            ReloadCategory()
            txtCatName.Clear()
            txtCatName.Focus()
        End Sub
        Private Sub btnCatEdit_Click(sender As Object, e As EventArgs) Handles btnCatEdit.Click
            If selCatId < 0 Then MessageBox.Show("เลือกรายการก่อน", "แจ้งเตือน") : Return
            If String.IsNullOrWhiteSpace(txtCatName.Text) Then MessageBox.Show("กรุณาใส่ชื่อประเภท", "แจ้งเตือน") : Return
            Dim tt = If(cboCatType.SelectedIndex = 0, "Income", "Expense")
            Using conn = Db.OpenConn()
                Db.ExecuteNonQuery(conn, "UPDATE Categories SET CategoryName=@n, TranType=@t WHERE ID=@id",
                                   New Tuple(Of String, Object)("@n", txtCatName.Text.Trim()),
                                   New Tuple(Of String, Object)("@t", tt),
                                   New Tuple(Of String, Object)("@id", selCatId))
            End Using
            ReloadCategory()
            txtCatName.Focus()
        End Sub
        Private Sub btnCatDel_Click(sender As Object, e As EventArgs) Handles btnCatDel.Click
            If selCatId < 0 Then MessageBox.Show("เลือกรายการก่อน", "แจ้งเตือน") : Return
            If MessageBox.Show("ลบรายการนี้ใช่หรือไม่?", "ยืนยัน", MessageBoxButtons.YesNo) <> DialogResult.Yes Then Return
            Using conn = Db.OpenConn()
                Db.ExecuteNonQuery(conn, "DELETE FROM Categories WHERE ID=@id", New Tuple(Of String, Object)("@id", selCatId))
            End Using
            ReloadCategory()
            txtCatName.Focus()
        End Sub

        Private Sub btnFundAdd_Click(sender As Object, e As EventArgs) Handles btnFundAdd.Click
            If String.IsNullOrWhiteSpace(txtFundName.Text) Then MessageBox.Show("กรุณาใส่ชื่อกองทุน", "แจ้งเตือน") : Return
            Using conn = Db.OpenConn()
                If selFundId >= 0 Then
                    Db.ExecuteNonQuery(conn, "UPDATE Funds SET FundName=@n WHERE ID=@id",
                                       New Tuple(Of String, Object)("@n", txtFundName.Text.Trim()),
                                       New Tuple(Of String, Object)("@id", selFundId))
                Else
                    Db.ExecuteNonQuery(conn, "INSERT INTO Funds (FundName) VALUES (@n)", New Tuple(Of String, Object)("@n", txtFundName.Text.Trim()))
                End If
            End Using
            ReloadFund()
            txtFundName.Clear() : selFundId = -1
            txtFundName.Focus()
        End Sub
        Private Sub btnFundDel_Click(sender As Object, e As EventArgs) Handles btnFundDel.Click
            If selFundId < 0 Then MessageBox.Show("เลือกก่อน", "แจ้งเตือน") : Return
            If MessageBox.Show("ลบกองทุนนี้ใช่หรือไม่?", "ยืนยัน", MessageBoxButtons.YesNo) <> DialogResult.Yes Then Return
            Using conn = Db.OpenConn()
                Db.ExecuteNonQuery(conn, "DELETE FROM Funds WHERE ID=@id", New Tuple(Of String, Object)("@id", selFundId))
            End Using
            ReloadFund() : selFundId = -1
            txtFundName.Focus()
        End Sub

        Private Sub btnBankAdd_Click(sender As Object, e As EventArgs) Handles btnBankAdd.Click
            If String.IsNullOrWhiteSpace(txtBankName.Text) Then MessageBox.Show("ใส่ชื่อธนาคาร", "แจ้งเตือน") : Return
            Using conn = Db.OpenConn()
                If selBankId >= 0 Then
                    Db.ExecuteNonQuery(conn, "UPDATE BankAccounts SET BankName=@n, AccountNo=@a, AccountName=@nm WHERE ID=@id",
                                       New Tuple(Of String, Object)("@n", txtBankName.Text.Trim()),
                                       New Tuple(Of String, Object)("@a", txtBankAccountNo.Text.Trim()),
                                       New Tuple(Of String, Object)("@nm", txtBankAccountName.Text.Trim()),
                                       New Tuple(Of String, Object)("@id", selBankId))
                Else
                    Db.ExecuteNonQuery(conn, "INSERT INTO BankAccounts (BankName, AccountNo, AccountName) VALUES (@n,@a,@nm)",
                                       New Tuple(Of String, Object)("@n", txtBankName.Text.Trim()),
                                       New Tuple(Of String, Object)("@a", txtBankAccountNo.Text.Trim()),
                                       New Tuple(Of String, Object)("@nm", txtBankAccountName.Text.Trim()))
                End If
            End Using
            ReloadBank()
            txtBankName.Clear() : txtBankAccountNo.Clear() : txtBankAccountName.Clear() : selBankId = -1
            txtBankName.Focus()
        End Sub
        Private Sub btnBankDel_Click(sender As Object, e As EventArgs) Handles btnBankDel.Click
            If selBankId < 0 Then MessageBox.Show("เลือกบัญชีก่อน", "แจ้งเตือน") : Return
            If MessageBox.Show("ลบบัญชีนี้ใช่หรือไม่?", "ยืนยัน", MessageBoxButtons.YesNo) <> DialogResult.Yes Then Return
            Using conn = Db.OpenConn()
                Db.ExecuteNonQuery(conn, "DELETE FROM BankAccounts WHERE ID=@id", New Tuple(Of String, Object)("@id", selBankId))
            End Using
            ReloadBank() : selBankId = -1
            txtBankName.Focus()
        End Sub
    End Class
End Namespace
