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
        Private ReadOnly _categoryFlow As New List(Of Control)()
        Private ReadOnly _fundFlow As New List(Of Control)()
        Private ReadOnly _bankFlow As New List(Of Control)()
        
        Private selCatId As Integer = -1, selFundId As Integer = -1, selBankId As Integer = -1

        Public Sub New()
            InitializeComponent()
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
