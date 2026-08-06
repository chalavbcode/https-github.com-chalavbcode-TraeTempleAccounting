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

        Private ReadOnly _categoryFlow As New List(Of Control)()
        Private ReadOnly _fundFlow As New List(Of Control)()
        Private ReadOnly _bankFlow As New List(Of Control)()

        Private selCatId As Integer = -1, selFundId As Integer = -1, selBankId As Integer = -1

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub FrmMasterData_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.KeyPreview = True
            HelpSystem.SetupHelp(Me, "FrmMasterData")
            Db.EnsureSchema()
            SetupToolTips()
            LoadAll()
            SetupEnterNavigation()
        End Sub

        Private Sub FrmMasterData_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.F1 Then
                e.Handled = True
                e.SuppressKeyPress = True
                HelpSystem.ShowManual("FrmMasterData")
            End If
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(btnCatAdd, "เพิ่มประเภทรายการใหม่ (รายรับ/รายจ่าย)")
            ttMain.SetToolTip(btnCatEdit, "แก้ไขชื่อหรือประเภทของรายการที่เลือกในตาราง")
            ttMain.SetToolTip(btnCatDel, "ลบประเภทรายการที่เลือกออกจากระบบ")
            ttMain.SetToolTip(btnFundAdd, "เพิ่มชื่อกองทุนใหม่")
            ttMain.SetToolTip(btnFundDel, "ลบกองทุนที่เลือกออกจากระบบ")
            ttMain.SetToolTip(btnBankAdd, "เพิ่มบัญชีธนาคารใหม่")
            ttMain.SetToolTip(btnBankDel, "ลบบัญชีธนาคารที่เลือกออกจากระบบ")
        End Sub

        Private Sub LoadAll()
            Using conn = Db.OpenConn()
                dgvCategory.DataSource = Db.GetTable(conn, "SELECT ID, CategoryName, TranType, IIF(TranType='Income', 'รายรับ', 'รายจ่าย') as TranTypeDisplay FROM Categories ORDER BY TranType, CategoryName")
                dgvFund.DataSource = Db.GetTable(conn, "SELECT ID, FundName FROM Funds ORDER BY FundName")
                dgvBank.DataSource = Db.GetTable(conn, "SELECT ID, BankName, AccountNo, AccountName FROM BankAccounts ORDER BY BankName")
            End Using

            ' Style columns
            If dgvCategory.Columns.Count > 0 Then
                dgvCategory.Columns("ID").Visible = False
                dgvCategory.Columns("TranType").Visible = False
                dgvCategory.Columns("CategoryName").HeaderText = "ชื่อประเภท"
                dgvCategory.Columns("TranTypeDisplay").HeaderText = "ชนิด"
            End If
            If dgvFund.Columns.Count > 0 Then
                dgvFund.Columns("ID").Visible = False
                dgvFund.Columns("FundName").HeaderText = "ชื่อกองทุน"
            End If
            If dgvBank.Columns.Count > 0 Then
                dgvBank.Columns("ID").Visible = False
                dgvBank.Columns("BankName").HeaderText = "ชื่อธนาคาร"
                dgvBank.Columns("AccountNo").HeaderText = "เลขบัญชี"
                dgvBank.Columns("AccountName").HeaderText = "ชื่อบัญชี"
            End If
        End Sub

        Private Sub SetupEnterNavigation()
            _categoryFlow.AddRange({txtCatName, cboCatType, btnCatAdd})
            _fundFlow.AddRange({txtFundName, btnFundAdd})
            _bankFlow.AddRange({txtBankName, txtBankAccountNo, txtBankAccountName, btnBankAdd})

            For Each ctrl In _categoryFlow : AddHandler ctrl.KeyDown, AddressOf HandleEnter : Next
            For Each ctrl In _fundFlow : AddHandler ctrl.KeyDown, AddressOf HandleEnter : Next
            For Each ctrl In _bankFlow : AddHandler ctrl.KeyDown, AddressOf HandleEnter : Next
        End Sub

        Private Sub HandleEnter(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True
            Dim current = DirectCast(sender, Control)
            Dim flow = If(TabControl1.SelectedTab Is tpCategory, _categoryFlow, If(TabControl1.SelectedTab Is tpFund, _fundFlow, _bankFlow))
            Dim idx = flow.IndexOf(current)
            If idx >= 0 AndAlso idx < flow.Count - 1 Then
                flow(idx + 1).Focus()
                If TypeOf flow(idx + 1) Is ComboBox Then DirectCast(flow(idx + 1), ComboBox).DroppedDown = True
            ElseIf idx = flow.Count - 1 Then
                If flow(idx) Is btnCatAdd Then btnCatAdd_Click(Nothing, Nothing)
                If flow(idx) Is btnFundAdd Then btnFundAdd_Click(Nothing, Nothing)
                If flow(idx) Is btnBankAdd Then btnBankAdd_Click(Nothing, Nothing)
            End If
        End Sub

        Private Function CategoryExists(conn As OleDbConnection, categoryName As String, tranType As String, Optional excludeId As Integer = -1) As Boolean
            Dim sql = "SELECT COUNT(*) FROM Categories WHERE CategoryName = @name AND TranType = @type"
            If excludeId <> -1 Then sql &= " AND ID <> @id"
            
            Dim count As Object
            If excludeId <> -1 Then
                count = Db.DbScalar(conn, sql, 
                    New Tuple(Of String, Object)("@name", categoryName),
                    New Tuple(Of String, Object)("@type", tranType),
                    New Tuple(Of String, Object)("@id", excludeId))
            Else
                count = Db.DbScalar(conn, sql, 
                    New Tuple(Of String, Object)("@name", categoryName),
                    New Tuple(Of String, Object)("@type", tranType))
            End If
            Return Convert.ToInt32(count) > 0
        End Function

        ' --- Category Events ---
        Private Sub btnCatAdd_Click(sender As Object, e As EventArgs) Handles btnCatAdd.Click
            Dim name = txtCatName.Text.Trim()
            Dim type = If(cboCatType.SelectedIndex = 0, "Income", "Expense")
            If name = "" Then MessageBox.Show("กรุณาใส่ชื่อประเภท", "แจ้งเตือน") : Return

            Try
                Using conn = Db.OpenConn()
                    If CategoryExists(conn, name, type) Then
                        MessageBox.Show($"มีชื่อประเภท '{name}' ในหมวด {If(type = "Income", "รายรับ", "รายจ่าย")} อยู่แล้ว", "ข้อมูลซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                    Db.ExecuteNonQuery(conn, "INSERT INTO Categories (CategoryName, TranType) VALUES (@n, @t)",
                        New Tuple(Of String, Object)("@n", name), New Tuple(Of String, Object)("@t", type))
                End Using
                txtCatName.Clear() : LoadAll()
            Catch ex As Exception : MessageBox.Show(ex.Message) : End Try
        End Sub

        Private Sub btnCatEdit_Click(sender As Object, e As EventArgs) Handles btnCatEdit.Click
            If selCatId = -1 Then Return
            Dim name = txtCatName.Text.Trim()
            Dim type = If(cboCatType.SelectedIndex = 0, "Income", "Expense")
            If name = "" Then Return

            Try
                Using conn = Db.OpenConn()
                    If CategoryExists(conn, name, type, selCatId) Then
                        MessageBox.Show($"มีชื่อประเภท '{name}' ในหมวด {If(type = "Income", "รายรับ", "รายจ่าย")} อยู่แล้ว", "ข้อมูลซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                    Db.ExecuteNonQuery(conn, "UPDATE Categories SET CategoryName=@n, TranType=@t WHERE ID=@id",
                        New Tuple(Of String, Object)("@n", name), New Tuple(Of String, Object)("@t", type), New Tuple(Of String, Object)("@id", selCatId))
                End Using
                LoadAll()
            Catch ex As Exception : MessageBox.Show(ex.Message) : End Try
        End Sub

        Private Sub btnCatDel_Click(sender As Object, e As EventArgs) Handles btnCatDel.Click
            If selCatId = -1 Then Return
            If MessageBox.Show("ยืนยันการลบ?", "ยืนยัน", MessageBoxButtons.YesNo) <> DialogResult.Yes Then Return
            Try
                Using conn = Db.OpenConn()
                    Db.ExecuteNonQuery(conn, "DELETE FROM Categories WHERE ID=@id", New Tuple(Of String, Object)("@id", selCatId))
                End Using
                selCatId = -1 : txtCatName.Clear() : LoadAll()
            Catch ex As Exception : MessageBox.Show("ไม่สามารถลบได้ เนื่องจากมีการใช้งานอยู่") : End Try
        End Sub

        Private Sub dgvCategory_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCategory.CellClick
            If e.RowIndex < 0 Then Return
            selCatId = Convert.ToInt32(dgvCategory.Rows(e.RowIndex).Cells("ID").Value)
            txtCatName.Text = dgvCategory.Rows(e.RowIndex).Cells("CategoryName").Value.ToString()
            Dim type = dgvCategory.Rows(e.RowIndex).Cells("TranType").Value.ToString()
            cboCatType.SelectedIndex = If(type = "Income", 0, 1)
        End Sub

        ' --- Fund Events ---
        Private Sub btnFundAdd_Click(sender As Object, e As EventArgs) Handles btnFundAdd.Click
            Dim name = txtFundName.Text.Trim()
            If name = "" Then Return
            Try
                Using conn = Db.OpenConn()
                    Db.ExecuteNonQuery(conn, "INSERT INTO Funds (FundName) VALUES (@n)", New Tuple(Of String, Object)("@n", name))
                End Using
                txtFundName.Clear() : LoadAll()
            Catch ex As Exception : MessageBox.Show(ex.Message) : End Try
        End Sub

        Private Sub btnFundDel_Click(sender As Object, e As EventArgs) Handles btnFundDel.Click
            If selFundId = -1 Then Return
            If MessageBox.Show("ยืนยันการลบ?", "ยืนยัน", MessageBoxButtons.YesNo) <> DialogResult.Yes Then Return
            Try
                Using conn = Db.OpenConn()
                    Db.ExecuteNonQuery(conn, "DELETE FROM Funds WHERE ID=@id", New Tuple(Of String, Object)("@id", selFundId))
                End Using
                selFundId = -1 : txtFundName.Clear() : LoadAll()
            Catch ex As Exception : MessageBox.Show("ไม่สามารถลบได้ เนื่องจากมีการใช้งานอยู่") : End Try
        End Sub

        Private Sub dgvFund_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvFund.CellClick
            If e.RowIndex < 0 Then Return
            selFundId = Convert.ToInt32(dgvFund.Rows(e.RowIndex).Cells("ID").Value)
            txtFundName.Text = dgvFund.Rows(e.RowIndex).Cells("FundName").Value.ToString()
        End Sub

        ' --- Bank Events ---
        Private Sub btnBankAdd_Click(sender As Object, e As EventArgs) Handles btnBankAdd.Click
            Dim name = txtBankName.Text.Trim(), no = txtBankAccountNo.Text.Trim(), acc = txtBankAccountName.Text.Trim()
            if name = "" Then Return
            Try
                Using conn = Db.OpenConn()
                    Db.ExecuteNonQuery(conn, "INSERT INTO BankAccounts (BankName, AccountNo, AccountName) VALUES (@n, @no, @a)",
                        New Tuple(Of String, Object)("@n", name), New Tuple(Of String, Object)("@no", no), New Tuple(Of String, Object)("@a", acc))
                End Using
                txtBankName.Clear() : txtBankAccountNo.Clear() : txtBankAccountName.Clear() : LoadAll()
            Catch ex As Exception : MessageBox.Show(ex.Message) : End Try
        End Sub

        Private Sub btnBankDel_Click(sender As Object, e As EventArgs) Handles btnBankDel.Click
            If selBankId = -1 Then Return
            If MessageBox.Show("ยืนยันการลบ?", "ยืนยัน", MessageBoxButtons.YesNo) <> DialogResult.Yes Then Return
            Try
                Using conn = Db.OpenConn()
                    Db.ExecuteNonQuery(conn, "DELETE FROM BankAccounts WHERE ID=@id", New Tuple(Of String, Object)("@id", selBankId))
                End Using
                selBankId = -1 : txtBankName.Clear() : txtBankAccountNo.Clear() : txtBankAccountName.Clear() : LoadAll()
            Catch ex As Exception : MessageBox.Show("ไม่สามารถลบได้ เนื่องจากมีการใช้งานอยู่") : End Try
        End Sub

        Private Sub dgvBank_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvBank.CellClick
            If e.RowIndex < 0 Then Return
            selBankId = Convert.ToInt32(dgvBank.Rows(e.RowIndex).Cells("ID").Value)
            txtBankName.Text = dgvBank.Rows(e.RowIndex).Cells("BankName").Value.ToString()
            txtBankAccountNo.Text = dgvBank.Rows(e.RowIndex).Cells("AccountNo").Value.ToString()
            txtBankAccountName.Text = dgvBank.Rows(e.RowIndex).Cells("AccountName").Value.ToString()
        End Sub
    End Class
End Namespace
