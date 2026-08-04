Imports System.Data
Imports System.Data.OleDb
Imports System.Windows.Forms

Namespace TempleAccounting
    Public Class FrmPersonnelManagement
        Private _selectedPersonnelID As Integer = -1
        Private _selectedPositionID As Integer = -1

        Private Sub FrmPersonnelManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            LoadPersonnelData()
            LoadPositionData()
            ClearPersonnelEditor()
            ClearPositionEditor()
        End Sub

        Private Sub tcMain_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tcMain.SelectedIndexChanged
            If tcMain.SelectedTab Is tpPersonnel Then
                btnDelete.Enabled = (_selectedPersonnelID <> -1)
            Else
                btnDelete.Enabled = (_selectedPositionID <> -1)
            End If
        End Sub

        #Region "Personnel Management"
        Private Sub LoadPersonnelData()
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT PersonnelID, Title, FirstName, LastName, FullName, PersonType, Phone FROM Personnel ORDER BY FullName")
                dgvPersonnel.DataSource = dt
                
                If dgvPersonnel.Columns.Count > 0 Then
                    dgvPersonnel.Columns("PersonnelID").Visible = False
                    dgvPersonnel.Columns("Title").HeaderText = "คำนำหน้า"
                    dgvPersonnel.Columns("FirstName").HeaderText = "ชื่อ"
                    dgvPersonnel.Columns("LastName").HeaderText = "นามสกุล"
                    dgvPersonnel.Columns("FullName").HeaderText = "ชื่อ-นามสกุล"
                    dgvPersonnel.Columns("PersonType").HeaderText = "ประเภท"
                    dgvPersonnel.Columns("Phone").HeaderText = "เบอร์โทร"
                End If
            End Using
        End Sub

        Private Sub ClearPersonnelEditor()
            _selectedPersonnelID = -1
            txtTitle.Clear()
            txtFirstName.Clear()
            txtLastName.Clear()
            cboPersonType.SelectedIndex = -1
            txtPhone.Clear()
            If tcMain.SelectedTab Is tpPersonnel Then btnDelete.Enabled = False
            txtTitle.Focus()
        End Sub

        Private Sub dgvPersonnel_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPersonnel.SelectionChanged
            If dgvPersonnel.SelectedRows.Count > 0 Then
                Dim row = dgvPersonnel.SelectedRows(0)
                _selectedPersonnelID = Convert.ToInt32(row.Cells("PersonnelID").Value)
                txtTitle.Text = row.Cells("Title").Value.ToString()
                txtFirstName.Text = row.Cells("FirstName").Value.ToString()
                txtLastName.Text = row.Cells("LastName").Value.ToString()
                cboPersonType.Text = row.Cells("PersonType").Value.ToString()
                txtPhone.Text = row.Cells("Phone").Value.ToString()
                If tcMain.SelectedTab Is tpPersonnel Then btnDelete.Enabled = True
            End If
        End Sub
        #End Region

        #Region "Position Management"
        Private Sub LoadPositionData()
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT PositionID, PositionName FROM Positions ORDER BY PositionName")
                dgvPositions.DataSource = dt
                
                If dgvPositions.Columns.Count > 0 Then
                    dgvPositions.Columns("PositionID").Visible = False
                    dgvPositions.Columns("PositionName").HeaderText = "ชื่อตำแหน่ง"
                End If
            End Using
        End Sub

        Private Sub ClearPositionEditor()
            _selectedPositionID = -1
            txtPositionName.Clear()
            If tcMain.SelectedTab Is tpPositions Then btnDelete.Enabled = False
            txtPositionName.Focus()
        End Sub

        Private Sub dgvPositions_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPositions.SelectionChanged
            If dgvPositions.SelectedRows.Count > 0 Then
                Dim row = dgvPositions.SelectedRows(0)
                _selectedPositionID = Convert.ToInt32(row.Cells("PositionID").Value)
                txtPositionName.Text = row.Cells("PositionName").Value.ToString()
                If tcMain.SelectedTab Is tpPositions Then btnDelete.Enabled = True
            End If
        End Sub
        #End Region

        Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
            If tcMain.SelectedTab Is tpPersonnel Then
                ClearPersonnelEditor()
            Else
                ClearPositionEditor()
            End If
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            If tcMain.SelectedTab Is tpPersonnel Then
                SavePersonnel()
            Else
                SavePosition()
            End If
        End Sub

        Private Sub SavePersonnel()
            If String.IsNullOrWhiteSpace(txtFirstName.Text) Then
                MessageBox.Show("กรุณาระบุชื่อ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim fullName = $"{txtTitle.Text.Trim()}{txtFirstName.Text.Trim()} {txtLastName.Text.Trim()}"

            Try
                Using conn = Db.OpenConn()
                    If _selectedPersonnelID = -1 Then
                        Db.ExecuteNonQuery(conn, "INSERT INTO Personnel (Title, FirstName, LastName, FullName, PersonType, Phone) VALUES (@t, @f, @l, @fn, @pt, @ph)",
                            New Tuple(Of String, Object)("@t", txtTitle.Text.Trim()),
                            New Tuple(Of String, Object)("@f", txtFirstName.Text.Trim()),
                            New Tuple(Of String, Object)("@l", txtLastName.Text.Trim()),
                            New Tuple(Of String, Object)("@fn", fullName),
                            New Tuple(Of String, Object)("@pt", cboPersonType.Text),
                            New Tuple(Of String, Object)("@ph", txtPhone.Text.Trim()))
                    Else
                        Db.ExecuteNonQuery(conn, "UPDATE Personnel SET Title=@t, FirstName=@f, LastName=@l, FullName=@fn, PersonType=@pt, Phone=@ph WHERE PersonnelID=@id",
                            New Tuple(Of String, Object)("@t", txtTitle.Text.Trim()),
                            New Tuple(Of String, Object)("@f", txtFirstName.Text.Trim()),
                            New Tuple(Of String, Object)("@l", txtLastName.Text.Trim()),
                            New Tuple(Of String, Object)("@fn", fullName),
                            New Tuple(Of String, Object)("@pt", cboPersonType.Text),
                            New Tuple(Of String, Object)("@ph", txtPhone.Text.Trim()),
                            New Tuple(Of String, Object)("@id", _selectedPersonnelID))
                    End If
                End Using
                LoadPersonnelData()
                ClearPersonnelEditor()
                MessageBox.Show("✅ บันทึกข้อมูลบุคลากรสำเร็จ", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาดในการบันทึก: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub SavePosition()
            If String.IsNullOrWhiteSpace(txtPositionName.Text) Then
                MessageBox.Show("กรุณาระบุชื่อตำแหน่ง", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Try
                Using conn = Db.OpenConn()
                    If _selectedPositionID = -1 Then
                        Db.ExecuteNonQuery(conn, "INSERT INTO Positions (PositionName) VALUES (@n)",
                            New Tuple(Of String, Object)("@n", txtPositionName.Text.Trim()))
                    Else
                        Db.ExecuteNonQuery(conn, "UPDATE Positions SET PositionName=@n WHERE PositionID=@id",
                            New Tuple(Of String, Object)("@n", txtPositionName.Text.Trim()),
                            New Tuple(Of String, Object)("@id", _selectedPositionID))
                    End If
                End Using
                LoadPositionData()
                ClearPositionEditor()
                MessageBox.Show("✅ บันทึกข้อมูลตำแหน่งสำเร็จ", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาดในการบันทึก: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
            If tcMain.SelectedTab Is tpPersonnel Then
                DeletePersonnel()
            Else
                DeletePosition()
            End If
        End Sub

        Private Sub DeletePersonnel()
            If _selectedPersonnelID = -1 Then Return
            If MessageBox.Show("คุณต้องการลบรายชื่อนี้ใช่หรือไม่?", "ยืนยันการลบ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    Using conn = Db.OpenConn()
                        Db.ExecuteNonQuery(conn, "DELETE FROM Personnel WHERE PersonnelID=@id", New Tuple(Of String, Object)("@id", _selectedPersonnelID))
                    End Using
                    LoadPersonnelData()
                    ClearPersonnelEditor()
                Catch ex As Exception
                    MessageBox.Show("ไม่สามารถลบข้อมูลได้: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Sub

        Private Sub DeletePosition()
            If _selectedPositionID = -1 Then Return
            If MessageBox.Show("คุณต้องการลบตำแหน่งนี้ใช่หรือไม่?", "ยืนยันการลบ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    Using conn = Db.OpenConn()
                        Db.ExecuteNonQuery(conn, "DELETE FROM Positions WHERE PositionID=@id", New Tuple(Of String, Object)("@id", _selectedPositionID))
                    End Using
                    LoadPositionData()
                    ClearPositionEditor()
                Catch ex As Exception
                    MessageBox.Show("ไม่สามารถลบข้อมูลได้: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub
    End Class
End Namespace
