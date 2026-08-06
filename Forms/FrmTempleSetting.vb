Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data
Imports System.Data.OleDb
Imports System.IO

Namespace TempleAccounting
    <DesignerCategory("Form")>
    Partial Public Class FrmTempleSetting
        Inherits Form

        Private ReadOnly _enterFlow As New List(Of Control)()

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub FrmTempleSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Try
                HelpSystem.SetupHelp(Me, "FrmTempleSetting")
                ' ตรวจสอบและสร้าง Schema หากยังไม่มี
                Db.EnsureSchema()

                ' โหลดข้อมูลที่อยู่ (จังหวัด/อำเภอ/ตำบล)
                LoadLocations()

                ' โหลดข้อมูลบุคลากร (เจ้าอาวาส/ไวยาวัจกร/ผู้ทำบัญชี)
                LoadPersonnel()

                ' โหลดข้อมูลวัดจากฐานข้อมูล
                LoadTempleData()

                ' ตั้งค่า Tooltip
                SetupToolTips()

                ' ตั้งค่าการนำทางด้วยปุ่ม Enter
                SetupEnterNavigation()

                ' ทำให้แถบปุ่มด้านล่างอยู่ด้านหน้าเสมอเมื่อ ContentPanel เลื่อน
                pBottom.BringToFront()

                ' โฟกัสที่ช่องรหัสวัด
                If txtTempleCode.CanFocus Then txtTempleCode.Focus()

            Catch ex As Exception
                ' จัดการข้อผิดพลาดอย่างปลอดภัยไม่ให้ฟอร์มล่ม
                MessageBox.Show("เกิดข้อผิดพลาดในการโหลดข้อมูล: " & ex.Message,
                               "ข้อผิดพลาด",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error)
                System.Diagnostics.Debug.WriteLine("[FrmTempleSetting] Load Error: " & ex.ToString())
            End Try
        End Sub

        Private Sub LoadPersonnel()
            Using conn = Db.OpenConn()
                ' โหลดเจ้าอาวาส (กรองเฉพาะกลุ่มพระ)
                Dim abbotTable = Db.GetTable(conn, "SELECT p.PersonnelID, p.FullName " &
                                              "FROM Personnel p INNER JOIN Positions pos ON p.PositionID = pos.PositionID " &
                                              "WHERE pos.PositionGroup = 'พระ' ORDER BY p.FullName")
                cboAbbotName.DisplayMember = "FullName"
                cboAbbotName.ValueMember = "PersonnelID"
                cboAbbotName.DataSource = abbotTable

                ' โหลดไวยาวัจกรและผู้ทำบัญชี (กรองเฉพาะกลุ่มฆราวาส)
                Dim layTable = Db.GetTable(conn, "SELECT p.PersonnelID, p.FullName " &
                                             "FROM Personnel p INNER JOIN Positions pos ON p.PositionID = pos.PositionID " &
                                             "WHERE pos.PositionGroup = 'ฆราวาส' ORDER BY p.FullName")

                cboWaiyawatName.DisplayMember = "FullName"
                cboWaiyawatName.ValueMember = "PersonnelID"
                cboWaiyawatName.DataSource = layTable.Copy()

                cboBookkeeperName.DisplayMember = "FullName"
                cboBookkeeperName.ValueMember = "PersonnelID"
                cboBookkeeperName.DataSource = layTable.Copy()

                ' โหลดรายชื่อผู้ดำรงตำแหน่งในวัดลง DataGridView
                LoadPersonnelGrid()
            End Using
        End Sub

        Private Sub LoadPersonnelGrid()
            Using conn = Db.OpenConn()
                ' ดึงรายชื่อผู้ดำรงตำแหน่งจาก TempleSetting โดยใช้ UNION ALL เพื่อเลี่ยงข้อจำกัด JOIN OR ของ Access
                Dim sql = "SELECT 'เจ้าอาวาส' AS [บทบาทในวัด], p.Title & ' ' & p.FirstName & ' ' & p.LastName AS [ชื่อ-นามสกุล], " &
                         "pos.PositionName AS [ตำแหน่ง], p.PersonType AS [ประเภท], p.Phone AS [เบอร์โทร] " &
                         "FROM ((TempleSetting t INNER JOIN Personnel p ON t.AbbotPersonnelID = p.PersonnelID) " &
                         "LEFT JOIN Positions pos ON p.PositionID = pos.PositionID) " &
                         "UNION ALL " &
                         "SELECT 'ไวยาวัจกร' AS [บทบาทในวัด], p.Title & ' ' & p.FirstName & ' ' & p.LastName AS [ชื่อ-นามสกุล], " &
                         "pos.PositionName AS [ตำแหน่ง], p.PersonType AS [ประเภท], p.Phone AS [เบอร์โทร] " &
                         "FROM ((TempleSetting t INNER JOIN Personnel p ON t.WaiyawatPersonnelID = p.PersonnelID) " &
                         "LEFT JOIN Positions pos ON p.PositionID = pos.PositionID) " &
                         "UNION ALL " &
                         "SELECT 'ผู้ทำบัญชี' AS [บทบาทในวัด], p.Title & ' ' & p.FirstName & ' ' & p.LastName AS [ชื่อ-นามสกุล], " &
                         "pos.PositionName AS [ตำแหน่ง], p.PersonType AS [ประเภท], p.Phone AS [เบอร์โทร] " &
                         "FROM ((TempleSetting t INNER JOIN Personnel p ON t.BookkeeperPersonnelID = p.PersonnelID) " &
                         "LEFT JOIN Positions pos ON p.PositionID = pos.PositionID)"

                Dim dt = Db.GetTable(conn, sql)
                dgvPersonnel.DataSource = dt

                ' ปรับแต่ง Header Text
                If dgvPersonnel.Columns.Count > 0 Then
                    dgvPersonnel.Columns("บทบาทในวัด").HeaderText = "บทบาทในวัด"
                    dgvPersonnel.Columns("ชื่อ-นามสกุล").HeaderText = "ชื่อ-นามสกุล"
                    dgvPersonnel.Columns("ตำแหน่ง").HeaderText = "ตำแหน่ง"
                    dgvPersonnel.Columns("ประเภท").HeaderText = "ประเภท"
                    dgvPersonnel.Columns("เบอร์โทร").HeaderText = "เบอร์โทร"
                End If
            End Using
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(btnSave, "บันทึกข้อมูลวัดและบุคลากรลงในฐานข้อมูล (Enter)")
            ttMain.SetToolTip(btnCancel, "โหลดข้อมูลวัดล่าสุดจากฐานข้อมูลมาแสดงใหม่อีกครั้ง")
            ttMain.SetToolTip(btnManagePersonnel, "จัดการรายชื่อบุคลากร (เพิ่ม/แก้ไข/ลบ)")
            ttMain.SetToolTip(btnLocationImport, "เปิดหน้าจอนำเข้าข้อมูลที่อยู่ (จังหวัด/อำเภอ/ตำบล) จากไฟล์ CSV")
            ttMain.SetToolTip(btnClose, "ปิดหน้าจอนี้และกลับไปที่หน้าหลัก")
        End Sub

        Private Sub SetupEnterNavigation()
            If _enterFlow.Count > 0 Then Return
            _enterFlow.AddRange({
                txtTempleCode, txtTempleName, txtTempleAddress, cboProvince, cboAmphoe, cboTambon,
                txtPostCode, txtTemplePhone, cboAbbotName,
                cboWaiyawatName, cboBookkeeperName,
                chkUsePromptPay, txtPromptPayName, txtPromptPayID, btnSave
            })
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
            If cb IsNot Nothing Then cb.DroppedDown = True

            Dim tb = TryCast(nextCtrl, TextBox)
            If tb IsNot Nothing Then tb.SelectAll()
        End Sub

        Private Sub HandleEnterAdvance(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True
            MoveNextFrom(DirectCast(sender, Control))
        End Sub

        Private Sub LoadLocations()
            Using conn = Db.OpenConn()
                Dim p = Db.GetTable(conn, "SELECT ProvinceID, ProvinceName FROM Province ORDER BY ProvinceName")
                cboProvince.DisplayMember = "ProvinceName"
                cboProvince.ValueMember = "ProvinceID"
                cboProvince.DataSource = p
            End Using
        End Sub

        Private Function GetSelectedIntValue(c As ComboBox, valueField As String) As Integer?
            If c Is Nothing Then Return Nothing

            If c.SelectedValue IsNot Nothing AndAlso Not TypeOf c.SelectedValue Is DataRowView Then
                Dim raw = c.SelectedValue.ToString()
                Dim parsed As Integer
                If Integer.TryParse(raw, parsed) Then Return parsed
            End If

            Dim drv = TryCast(c.SelectedItem, DataRowView)
            If drv IsNot Nothing Then
                Dim raw = drv(valueField)?.ToString()
                Dim parsed As Integer
                If Integer.TryParse(raw, parsed) Then Return parsed
            End If

            Return Nothing
        End Function

        Private Sub cboProvince_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboProvince.SelectedIndexChanged
            Dim pid = GetSelectedIntValue(cboProvince, "ProvinceID")
            If Not pid.HasValue Then Return
            Using conn = Db.OpenConn()
                Dim a = Db.GetTable(conn, "SELECT DistrictID, DistrictName FROM District WHERE ProvinceID=@p ORDER BY DistrictName",
                                    New Tuple(Of String, Object)("@p", pid.Value))
                cboAmphoe.DisplayMember = "DistrictName"
                cboAmphoe.ValueMember = "DistrictID"
                cboAmphoe.DataSource = a
            End Using
        End Sub

        Private Sub cboAmphoe_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboAmphoe.SelectedIndexChanged
            Dim did = GetSelectedIntValue(cboAmphoe, "DistrictID")
            If Not did.HasValue Then Return
            Using conn = Db.OpenConn()
                Dim t = Db.GetTable(conn, "SELECT SubDistrictID, SubDistrictName, ZipCode FROM SubDistrict WHERE DistrictID=@d ORDER BY SubDistrictName",
                                    New Tuple(Of String, Object)("@d", did.Value))
                cboTambon.DisplayMember = "SubDistrictName"
                cboTambon.ValueMember = "SubDistrictID"
                cboTambon.DataSource = t
            End Using
        End Sub

        Private Sub cboTambon_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTambon.SelectedIndexChanged
            Dim sid = GetSelectedIntValue(cboTambon, "SubDistrictID")
            If Not sid.HasValue Then Return
            Using conn = Db.OpenConn()
                Dim zip = Db.DbScalar(conn, "SELECT ZipCode FROM SubDistrict WHERE SubDistrictID=@s", New Tuple(Of String, Object)("@s", sid.Value))
                If zip IsNot Nothing Then txtPostCode.Text = zip.ToString()
            End Using
        End Sub

        Private Sub LoadTempleData()
            Using conn = Db.OpenConn()
                Dim row = Db.GetTable(conn, "SELECT TOP 1 * FROM TempleSetting ORDER BY ID DESC")
                If row.Rows.Count = 0 Then Return
                Dim r = row.Rows(0)
                txtTempleCode.Text = r("TempleCode")?.ToString()
                txtTempleName.Text = r("TempleName")?.ToString()
                txtTempleAddress.Text = r("TempleAddress")?.ToString()
                TrySetComboText(cboProvince, r("Province")?.ToString())
                TrySetComboText(cboAmphoe, r("Amphoe")?.ToString())
                TrySetComboText(cboTambon, r("Tambon")?.ToString())
                txtPostCode.Text = r("PostCode")?.ToString()
                txtTemplePhone.Text = r("TemplePhone")?.ToString()

                ' Load Personnel IDs
                If r.Table.Columns.Contains("AbbotPersonnelID") AndAlso Not IsDBNull(r("AbbotPersonnelID")) Then
                    cboAbbotName.SelectedValue = r("AbbotPersonnelID")
                End If

                If r.Table.Columns.Contains("WaiyawatPersonnelID") AndAlso Not IsDBNull(r("WaiyawatPersonnelID")) Then
                    cboWaiyawatName.SelectedValue = r("WaiyawatPersonnelID")
                End If

                If r.Table.Columns.Contains("BookkeeperPersonnelID") AndAlso Not IsDBNull(r("BookkeeperPersonnelID")) Then
                    cboBookkeeperName.SelectedValue = r("BookkeeperPersonnelID")
                End If

                txtPromptPayName.Text = r("PromptPayName")?.ToString()
                txtPromptPayID.Text = r("PromptPayID")?.ToString()
                chkUsePromptPay.Checked = (Not String.IsNullOrWhiteSpace(txtPromptPayName.Text) OrElse Not String.IsNullOrWhiteSpace(txtPromptPayID.Text))
            End Using
        End Sub

        Private Sub TrySetComboText(c As ComboBox, txt As String)
            If String.IsNullOrWhiteSpace(txt) Then Return
            For i As Integer = 0 To c.Items.Count - 1
                Dim d As Object = c.Items(i)
                If TypeOf d Is DataRowView Then
                    Dim drv As DataRowView = d
                    For Each col As DataColumn In drv.Row.Table.Columns
                        If drv(col.ColumnName)?.ToString() = txt Then c.SelectedIndex = i : Return
                    Next
                ElseIf d?.ToString() = txt Then
                    c.SelectedIndex = i : Return
                End If
            Next
            If c.DropDownStyle = ComboBoxStyle.DropDown Then c.Text = txt
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            If String.IsNullOrWhiteSpace(txtTempleName.Text) Then MessageBox.Show("กรุณาใส่ชื่อวัด", "แจ้งเตือน") : txtTempleName.Focus() : Return
            Try
                Using conn = Db.OpenConn()
                    Dim pn = "", an = "", tn = ""
                    If TypeOf cboProvince.SelectedItem Is DataRowView Then pn = CType(cboProvince.SelectedItem, DataRowView)("ProvinceName")?.ToString() Else pn = cboProvince.Text
                    If TypeOf cboAmphoe.SelectedItem Is DataRowView Then an = CType(cboAmphoe.SelectedItem, DataRowView)("DistrictName")?.ToString() Else an = cboAmphoe.Text
                    If TypeOf cboTambon.SelectedItem Is DataRowView Then tn = CType(cboTambon.SelectedItem, DataRowView)("SubDistrictName")?.ToString() Else tn = cboTambon.Text

                    ' ดึง ID จาก ComboBox
                    Dim abbotID = GetSelectedIntValue(cboAbbotName, "PersonnelID")
                    Dim waiyawatID = GetSelectedIntValue(cboWaiyawatName, "PersonnelID")
                    Dim bookkeeperID = GetSelectedIntValue(cboBookkeeperName, "PersonnelID")

                    ' DEBUG: Log values before save
                    System.Diagnostics.Debug.WriteLine("[FrmTempleSetting] Save Debug:")
                    System.Diagnostics.Debug.WriteLine("  cboAbbotName.SelectedValue = " & If(cboAbbotName.SelectedValue IsNot Nothing, cboAbbotName.SelectedValue.ToString(), "Nothing"))
                    System.Diagnostics.Debug.WriteLine("  cboWaiyawatName.SelectedValue = " & If(cboWaiyawatName.SelectedValue IsNot Nothing, cboWaiyawatName.SelectedValue.ToString(), "Nothing"))
                    System.Diagnostics.Debug.WriteLine("  cboBookkeeperName.SelectedValue = " & If(cboBookkeeperName.SelectedValue IsNot Nothing, cboBookkeeperName.SelectedValue.ToString(), "Nothing"))
                    System.Diagnostics.Debug.WriteLine("  abbotID = " & If(abbotID.HasValue, abbotID.Value.ToString(), "Nothing"))
                    System.Diagnostics.Debug.WriteLine("  waiyawatID = " & If(waiyawatID.HasValue, waiyawatID.Value.ToString(), "Nothing"))
                    System.Diagnostics.Debug.WriteLine("  bookkeeperID = " & If(bookkeeperID.HasValue, bookkeeperID.Value.ToString(), "Nothing"))

                    Dim ppName = If(chkUsePromptPay.Checked, txtPromptPayName.Text.Trim(), "")
                    Dim ppID = If(chkUsePromptPay.Checked, txtPromptPayID.Text.Trim(), "")

                    ' บันทึกข้อมูล TempleInfo - อ้างอิง PersonnelID เท่านั้น (Modern Refactored Structure)
                    Db.ExecuteNonQuery(conn, "DELETE FROM TempleSetting")
                    Db.InsertAndGetId(conn,
                        "INSERT INTO TempleSetting (TempleCode,TempleName,TempleAddress,Tambon,Amphoe,Province,PostCode,TemplePhone," &
                        "PromptPayName,PromptPayID,AbbotPersonnelID,WaiyawatPersonnelID,BookkeeperPersonnelID) " &
                        "VALUES (@a1,@a2,@a3,@a4,@a5,@a6,@a7,@a8,@a9,@a10,@a11,@a12,@a13)",
                        New Tuple(Of String, Object)("@a1", txtTempleCode.Text.Trim()),
                        New Tuple(Of String, Object)("@a2", txtTempleName.Text.Trim()),
                        New Tuple(Of String, Object)("@a3", txtTempleAddress.Text.Trim()),
                        New Tuple(Of String, Object)("@a4", tn),
                        New Tuple(Of String, Object)("@a5", an),
                        New Tuple(Of String, Object)("@a6", pn),
                        New Tuple(Of String, Object)("@a7", txtPostCode.Text.Trim()),
                        New Tuple(Of String, Object)("@a8", txtTemplePhone.Text.Trim()),
                        New Tuple(Of String, Object)("@a9", ppName),
                        New Tuple(Of String, Object)("@a10", ppID),
                        New Tuple(Of String, Object)("@a11", If(abbotID.HasValue, abbotID.Value, DBNull.Value)),
                        New Tuple(Of String, Object)("@a12", If(waiyawatID.HasValue, waiyawatID.Value, DBNull.Value)),
                        New Tuple(Of String, Object)("@a13", If(bookkeeperID.HasValue, bookkeeperID.Value, DBNull.Value)))

                    MessageBox.Show("✅ บันทึกข้อมูลวัดสำเร็จ!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' Clear TemplateInfo cache so reports pick up the new settings
                    ReportEngine.ClearTemplateInfoCache()

                    ' รีเฟรช DataGridView หลังบันทึก
                    LoadPersonnelGrid()
                    txtTempleCode.Focus()
                End Using
            Catch ex As Exception
                MessageBox.Show("บันทึกไม่สำเร็จ: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            LoadTempleData()
            txtTempleCode.Focus()
        End Sub

        Private Sub btnManagePersonnel_Click(sender As Object, e As EventArgs) Handles btnManagePersonnel.Click
            Using f As New FrmPersonnelManagement()
                f.ShowDialog(Me)
            End Using
            LoadPersonnel() ' Refresh ComboBoxes after management
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Dim f = TryCast(Me.ParentForm, frmMain)
            If f IsNot Nothing Then f.CloseActiveForm() : f.ShowDashboard()
        End Sub

        Private Sub btnLocationImport_Click(sender As Object, e As EventArgs) Handles btnLocationImport.Click
            Dim f = TryCast(Me.ParentForm, frmMain)
            If f IsNot Nothing Then
                f.ShowFormInPanel(New FrmLocationImport(), "📍 นำเข้าข้อมูลจังหวัด/อำเภอ/ตำบล")
            Else
                Using g As New FrmLocationImport()
                    g.ShowDialog(Me)
                End Using
                LoadLocations()
            End If
        End Sub
    End Class
End Namespace
