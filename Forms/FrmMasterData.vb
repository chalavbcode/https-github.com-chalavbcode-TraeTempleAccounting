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
        Friend WithEvents pCatTop As Panel
        Friend WithEvents pFundTop As Panel
        Friend WithEvents pBankTop As Panel

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
            Dim DataGridViewCellStyle5 As DataGridViewCellStyle = New DataGridViewCellStyle()
            Dim DataGridViewCellStyle6 As DataGridViewCellStyle = New DataGridViewCellStyle()
            lblHeader = New Label()
            TabControl1 = New TabControl()
            tpCategory = New TabPage()
            pCatTop = New Panel()
            lblCatName = New Label()
            txtCatName = New TextBox()
            lblCatType = New Label()
            cboCatType = New ComboBox()
            btnCatAdd = New Button()
            btnCatEdit = New Button()
            btnCatDel = New Button()
            dgvCategory = New DataGridView()
            tpFund = New TabPage()
            pFundTop = New Panel()
            lblFundName = New Label()
            txtFundName = New TextBox()
            btnFundAdd = New Button()
            btnFundDel = New Button()
            dgvFund = New DataGridView()
            tpBank = New TabPage()
            pBankTop = New Panel()
            lblBankName = New Label()
            txtBankName = New TextBox()
            lblBankAccountNo = New Label()
            txtBankAccountNo = New TextBox()
            lblBankAccountName = New Label()
            txtBankAccountName = New TextBox()
            btnBankAdd = New Button()
            btnBankDel = New Button()
            dgvBank = New DataGridView()
            TabControl1.SuspendLayout()
            tpCategory.SuspendLayout()
            pCatTop.SuspendLayout()
            CType(dgvCategory, ISupportInitialize).BeginInit()
            tpFund.SuspendLayout()
            pFundTop.SuspendLayout()
            CType(dgvFund, ISupportInitialize).BeginInit()
            tpBank.SuspendLayout()
            pBankTop.SuspendLayout()
            CType(dgvBank, ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' lblHeader
            ' 
            lblHeader.BackColor = Color.FromArgb(CByte(253), CByte(230), CByte(138))
            lblHeader.Dock = DockStyle.Top
            lblHeader.Font = New Font("Tahoma", 12.5F, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            lblHeader.Location = New Point(0, 0)
            lblHeader.Name = "lblHeader"
            lblHeader.Size = New Size(1111, 42)
            lblHeader.TabIndex = 1
            lblHeader.Text = "🗂️ จัดการข้อมูลหลัก ประเภทรายการ / กองทุน / บัญชีธนาคาร"
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' TabControl1
            ' 
            TabControl1.Controls.Add(tpCategory)
            TabControl1.Controls.Add(tpFund)
            TabControl1.Controls.Add(tpBank)
            TabControl1.Dock = DockStyle.Fill
            TabControl1.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            TabControl1.Location = New Point(0, 42)
            TabControl1.Name = "TabControl1"
            TabControl1.SelectedIndex = 0
            TabControl1.Size = New Size(1111, 463)
            TabControl1.TabIndex = 0
            ' 
            ' tpCategory
            ' 
            tpCategory.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            tpCategory.Controls.Add(pCatTop)
            tpCategory.Controls.Add(dgvCategory)
            tpCategory.Location = New Point(4, 33)
            tpCategory.Name = "tpCategory"
            tpCategory.Size = New Size(1103, 426)
            tpCategory.TabIndex = 0
            tpCategory.Text = "ประเภทรายการ (Category)"
            ' 
            ' pCatTop
            ' 
            pCatTop.Controls.Add(lblCatName)
            pCatTop.Controls.Add(txtCatName)
            pCatTop.Controls.Add(lblCatType)
            pCatTop.Controls.Add(cboCatType)
            pCatTop.Controls.Add(btnCatAdd)
            pCatTop.Controls.Add(btnCatEdit)
            pCatTop.Controls.Add(btnCatDel)
            pCatTop.Location = New Point(0, 0)
            pCatTop.Name = "pCatTop"
            pCatTop.Size = New Size(200, 100)
            pCatTop.TabIndex = 0
            ' 
            ' lblCatName
            ' 
            lblCatName.Location = New Point(0, 0)
            lblCatName.Name = "lblCatName"
            lblCatName.Size = New Size(100, 23)
            lblCatName.TabIndex = 0
            ' 
            ' txtCatName
            ' 
            txtCatName.Location = New Point(0, 0)
            txtCatName.Name = "txtCatName"
            txtCatName.Size = New Size(100, 32)
            txtCatName.TabIndex = 1
            ' 
            ' lblCatType
            ' 
            lblCatType.Location = New Point(0, 0)
            lblCatType.Name = "lblCatType"
            lblCatType.Size = New Size(100, 23)
            lblCatType.TabIndex = 2
            ' 
            ' cboCatType
            ' 
            cboCatType.Items.AddRange(New Object() {"Income (รายรับ)", "Expense (รายจ่าย)"})
            cboCatType.Location = New Point(0, 0)
            cboCatType.Name = "cboCatType"
            cboCatType.Size = New Size(121, 32)
            cboCatType.TabIndex = 3
            ' 
            ' btnCatAdd
            ' 
            btnCatAdd.Location = New Point(0, 0)
            btnCatAdd.Name = "btnCatAdd"
            btnCatAdd.Size = New Size(75, 23)
            btnCatAdd.TabIndex = 4
            ' 
            ' btnCatEdit
            ' 
            btnCatEdit.Location = New Point(0, 0)
            btnCatEdit.Name = "btnCatEdit"
            btnCatEdit.Size = New Size(75, 23)
            btnCatEdit.TabIndex = 5
            ' 
            ' btnCatDel
            ' 
            btnCatDel.Location = New Point(0, 0)
            btnCatDel.Name = "btnCatDel"
            btnCatDel.Size = New Size(75, 23)
            btnCatDel.TabIndex = 6
            ' 
            ' dgvCategory
            ' 
            DataGridViewCellStyle4.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvCategory.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle4
            dgvCategory.ColumnHeadersHeight = 34
            dgvCategory.Location = New Point(0, 0)
            dgvCategory.Name = "dgvCategory"
            dgvCategory.RowHeadersWidth = 62
            dgvCategory.Size = New Size(240, 150)
            dgvCategory.TabIndex = 1
            ' 
            ' tpFund
            ' 
            tpFund.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            tpFund.Controls.Add(pFundTop)
            tpFund.Controls.Add(dgvFund)
            tpFund.Location = New Point(4, 33)
            tpFund.Name = "tpFund"
            tpFund.Size = New Size(192, 63)
            tpFund.TabIndex = 1
            tpFund.Text = "กองทุน (Funds)"
            ' 
            ' pFundTop
            ' 
            pFundTop.Controls.Add(lblFundName)
            pFundTop.Controls.Add(txtFundName)
            pFundTop.Controls.Add(btnFundAdd)
            pFundTop.Controls.Add(btnFundDel)
            pFundTop.Location = New Point(0, 0)
            pFundTop.Name = "pFundTop"
            pFundTop.Size = New Size(200, 100)
            pFundTop.TabIndex = 0
            ' 
            ' lblFundName
            ' 
            lblFundName.Location = New Point(0, 0)
            lblFundName.Name = "lblFundName"
            lblFundName.Size = New Size(100, 23)
            lblFundName.TabIndex = 0
            ' 
            ' txtFundName
            ' 
            txtFundName.Location = New Point(0, 0)
            txtFundName.Name = "txtFundName"
            txtFundName.Size = New Size(100, 32)
            txtFundName.TabIndex = 1
            ' 
            ' btnFundAdd
            ' 
            btnFundAdd.Location = New Point(0, 0)
            btnFundAdd.Name = "btnFundAdd"
            btnFundAdd.Size = New Size(75, 23)
            btnFundAdd.TabIndex = 2
            ' 
            ' btnFundDel
            ' 
            btnFundDel.Location = New Point(0, 0)
            btnFundDel.Name = "btnFundDel"
            btnFundDel.Size = New Size(75, 23)
            btnFundDel.TabIndex = 3
            ' 
            ' dgvFund
            ' 
            DataGridViewCellStyle5.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvFund.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle5
            dgvFund.ColumnHeadersHeight = 34
            dgvFund.Location = New Point(0, 0)
            dgvFund.Name = "dgvFund"
            dgvFund.RowHeadersWidth = 62
            dgvFund.Size = New Size(240, 150)
            dgvFund.TabIndex = 1
            ' 
            ' tpBank
            ' 
            tpBank.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            tpBank.Controls.Add(pBankTop)
            tpBank.Controls.Add(dgvBank)
            tpBank.Location = New Point(4, 33)
            tpBank.Name = "tpBank"
            tpBank.Size = New Size(192, 63)
            tpBank.TabIndex = 2
            tpBank.Text = "บัญชีธนาคาร (Bank Accounts)"
            ' 
            ' pBankTop
            ' 
            pBankTop.Controls.Add(lblBankName)
            pBankTop.Controls.Add(txtBankName)
            pBankTop.Controls.Add(lblBankAccountNo)
            pBankTop.Controls.Add(txtBankAccountNo)
            pBankTop.Controls.Add(lblBankAccountName)
            pBankTop.Controls.Add(txtBankAccountName)
            pBankTop.Controls.Add(btnBankAdd)
            pBankTop.Controls.Add(btnBankDel)
            pBankTop.Location = New Point(0, 0)
            pBankTop.Name = "pBankTop"
            pBankTop.Size = New Size(200, 100)
            pBankTop.TabIndex = 0
            ' 
            ' lblBankName
            ' 
            lblBankName.Location = New Point(0, 0)
            lblBankName.Name = "lblBankName"
            lblBankName.Size = New Size(100, 23)
            lblBankName.TabIndex = 0
            ' 
            ' txtBankName
            ' 
            txtBankName.Location = New Point(0, 0)
            txtBankName.Name = "txtBankName"
            txtBankName.Size = New Size(100, 32)
            txtBankName.TabIndex = 1
            ' 
            ' lblBankAccountNo
            ' 
            lblBankAccountNo.Location = New Point(0, 0)
            lblBankAccountNo.Name = "lblBankAccountNo"
            lblBankAccountNo.Size = New Size(100, 23)
            lblBankAccountNo.TabIndex = 2
            ' 
            ' txtBankAccountNo
            ' 
            txtBankAccountNo.Location = New Point(0, 0)
            txtBankAccountNo.Name = "txtBankAccountNo"
            txtBankAccountNo.Size = New Size(100, 32)
            txtBankAccountNo.TabIndex = 3
            ' 
            ' lblBankAccountName
            ' 
            lblBankAccountName.Location = New Point(0, 0)
            lblBankAccountName.Name = "lblBankAccountName"
            lblBankAccountName.Size = New Size(100, 23)
            lblBankAccountName.TabIndex = 4
            ' 
            ' txtBankAccountName
            ' 
            txtBankAccountName.Location = New Point(0, 0)
            txtBankAccountName.Name = "txtBankAccountName"
            txtBankAccountName.Size = New Size(100, 32)
            txtBankAccountName.TabIndex = 5
            ' 
            ' btnBankAdd
            ' 
            btnBankAdd.Location = New Point(0, 0)
            btnBankAdd.Name = "btnBankAdd"
            btnBankAdd.Size = New Size(75, 23)
            btnBankAdd.TabIndex = 6
            ' 
            ' btnBankDel
            ' 
            btnBankDel.Location = New Point(0, 0)
            btnBankDel.Name = "btnBankDel"
            btnBankDel.Size = New Size(75, 23)
            btnBankDel.TabIndex = 7
            ' 
            ' dgvBank
            ' 
            DataGridViewCellStyle6.BackColor = Color.FromArgb(CByte(255), CByte(251), CByte(235))
            dgvBank.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
            dgvBank.ColumnHeadersHeight = 34
            dgvBank.Location = New Point(0, 0)
            dgvBank.Name = "dgvBank"
            dgvBank.RowHeadersWidth = 62
            dgvBank.Size = New Size(240, 150)
            dgvBank.TabIndex = 1
            ' 
            ' FrmMasterData
            ' 
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1111, 505)
            Controls.Add(TabControl1)
            Controls.Add(lblHeader)
            Font = New Font("Tahoma", 10.5F)
            FormBorderStyle = FormBorderStyle.None
            Name = "FrmMasterData"
            Text = "จัดการข้อมูลหลัก"
            TabControl1.ResumeLayout(False)
            tpCategory.ResumeLayout(False)
            pCatTop.ResumeLayout(False)
            pCatTop.PerformLayout()
            CType(dgvCategory, ISupportInitialize).EndInit()
            tpFund.ResumeLayout(False)
            pFundTop.ResumeLayout(False)
            pFundTop.PerformLayout()
            CType(dgvFund, ISupportInitialize).EndInit()
            tpBank.ResumeLayout(False)
            pBankTop.ResumeLayout(False)
            pBankTop.PerformLayout()
            CType(dgvBank, ISupportInitialize).EndInit()
            ResumeLayout(False)
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

        Private Function CategoryExists(conn As OleDbConnection, categoryName As String, tranType As String, Optional excludeId As Integer = -1) As Boolean
            Dim sql = "SELECT COUNT(*) FROM Categories WHERE UCASE(TRIM(CategoryName))=UCASE(TRIM(@n)) AND TranType=@t"
            Dim params As New List(Of Tuple(Of String, Object)) From {
                New Tuple(Of String, Object)("@n", categoryName.Trim()),
                New Tuple(Of String, Object)("@t", tranType)
            }

            If excludeId >= 0 Then
                sql &= " AND ID<>@id"
                params.Add(New Tuple(Of String, Object)("@id", excludeId))
            End If

            Return Db.ToIntOrZero(Db.DbScalar(conn, sql, params.ToArray())) > 0
        End Function

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
                If CategoryExists(conn, txtCatName.Text, tt) Then
                    MessageBox.Show("มีชื่อประเภทนี้อยู่แล้วในชนิดเดียวกัน ระบบจะไม่เพิ่มข้อมูลซ้ำ", "ข้อมูลซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtCatName.Focus()
                    txtCatName.SelectAll()
                    Return
                End If
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
                If CategoryExists(conn, txtCatName.Text, tt, selCatId) Then
                    MessageBox.Show("มีชื่อประเภทนี้อยู่แล้วในชนิดเดียวกัน ระบบจะไม่บันทึกข้อมูลซ้ำ", "ข้อมูลซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtCatName.Focus()
                    txtCatName.SelectAll()
                    Return
                End If
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
