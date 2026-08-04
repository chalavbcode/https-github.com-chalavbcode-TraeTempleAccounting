Imports System.Data
Imports System.Data.OleDb
Imports System.Windows.Forms

Namespace TempleAccounting
    Public Class FrmPersonnelManagement
        Private _selectedID As Integer = -1

        Private Sub FrmPersonnelManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            LoadData()
            ClearEditor()
        End Sub

        Private Sub LoadData()
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn, "SELECT PersonnelID, Title, FirstName, LastName, FullName, PersonType, Phone FROM Personnel ORDER BY FullName")
                dgvPersonnel.DataSource = dt
                
                ' Format Grid
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

        Private Sub ClearEditor()
            _selectedID = -1
            txtTitle.Clear()
            txtFirstName.Clear()
            txtLastName.Clear()
            cboPersonType.SelectedIndex = -1
            txtPhone.Clear()
            btnDelete.Enabled = False
            txtTitle.Focus()
        End Sub

        Private Sub dgvPersonnel_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPersonnel.SelectionChanged
            If dgvPersonnel.SelectedRows.Count > 0 Then
                Dim row = dgvPersonnel.SelectedRows(0)
                _selectedID = Convert.ToInt32(row.Cells("PersonnelID").Value)
                txtTitle.Text = row.Cells("Title").Value.ToString()
                txtFirstName.Text = row.Cells("FirstName").Value.ToString()
                txtLastName.Text = row.Cells("LastName").Value.ToString()
                cboPersonType.Text = row.Cells("PersonType").Value.ToString()
                txtPhone.Text = row.Cells("Phone").Value.ToString()
                btnDelete.Enabled = True
            End If
        End Sub

        Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
            ClearEditor()
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            If String.IsNullOrWhiteSpace(txtFirstName.Text) Then
                MessageBox.Show("กรุณาระบุชื่อ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim fullName = $"{txtTitle.Text.Trim()}{txtFirstName.Text.Trim()} {txtLastName.Text.Trim()}"

            Try
                Using conn = Db.OpenConn()
                    If _selectedID = -1 Then
                        ' Insert
                        Db.ExecuteNonQuery(conn, "INSERT INTO Personnel (Title, FirstName, LastName, FullName, PersonType, Phone) VALUES (@t, @f, @l, @fn, @pt, @ph)",
                            New Tuple(Of String, Object)("@t", txtTitle.Text.Trim()),
                            New Tuple(Of String, Object)("@f", txtFirstName.Text.Trim()),
                            New Tuple(Of String, Object)("@l", txtLastName.Text.Trim()),
                            New Tuple(Of String, Object)("@fn", fullName),
                            New Tuple(Of String, Object)("@pt", cboPersonType.Text),
                            New Tuple(Of String, Object)("@ph", txtPhone.Text.Trim()))
                    Else
                        ' Update
                        Db.ExecuteNonQuery(conn, "UPDATE Personnel SET Title=@t, FirstName=@f, LastName=@l, FullName=@fn, PersonType=@pt, Phone=@ph WHERE PersonnelID=@id",
                            New Tuple(Of String, Object)("@t", txtTitle.Text.Trim()),
                            New Tuple(Of String, Object)("@f", txtFirstName.Text.Trim()),
                            New Tuple(Of String, Object)("@l", txtLastName.Text.Trim()),
                            New Tuple(Of String, Object)("@fn", fullName),
                            New Tuple(Of String, Object)("@pt", cboPersonType.Text),
                            New Tuple(Of String, Object)("@ph", txtPhone.Text.Trim()),
                            New Tuple(Of String, Object)("@id", _selectedID))
                    End If
                End Using
                
                LoadData()
                ClearEditor()
                MessageBox.Show("✅ บันทึกข้อมูลบุคลากรสำเร็จ", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาดในการบันทึก: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
            If _selectedID = -1 Then Return
            
            If MessageBox.Show("คุณต้องการลบรายชื่อนี้ใช่หรือไม่?", "ยืนยันการลบ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    Using conn = Db.OpenConn()
                        Db.ExecuteNonQuery(conn, "DELETE FROM Personnel WHERE PersonnelID=@id", New Tuple(Of String, Object)("@id", _selectedID))
                    End Using
                    LoadData()
                    ClearEditor()
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
