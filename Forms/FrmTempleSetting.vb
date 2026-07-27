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
    Partial Public Class FrmTempleSetting
        Inherits Form

        Private components As IContainer = Nothing
        Private ReadOnly _enterFlow As New List(Of Control)()

        Friend WithEvents txtTempleCode, txtTempleName, txtTempleAddress, txtPostCode As TextBox
        Friend WithEvents txtTemplePhone, txtAbbotName, txtWaiyawatName, txtBookkeeperName As TextBox
        Friend WithEvents txtPromptPayName, txtPromptPayID As TextBox
        Friend WithEvents cboProvince, cboAmphoe, cboTambon As ComboBox
        Friend WithEvents cboAbbotOfficeStatus, cboWaiyawatOfficeStatus, cboBookkeeperType As ComboBox
        Friend WithEvents chkUsePromptPay As CheckBox
        Friend WithEvents btnSave, btnCancel, btnClose, btnLocationImport As Button
        Friend WithEvents lblHeader As Label

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Private Function Lbl(t As String, x As Integer, y As Integer, Optional w As Integer = 200) As Label
            Return New Label With {.Text = t, .Location = New Point(x, y), .Size = New Size(w, 40), .TextAlign = ContentAlignment.MiddleRight, .Font = New Font("Tahoma", 10.0!)}
        End Function
        Private Function Tb(Optional y As Integer = 0) As TextBox
            Dim t As New TextBox()
            t.Font = New Font("Tahoma", 10.5!)
            t.Size = New Size(520, 40)
            t.Location = New Point(250, y)
            Return t
        End Function
        Private Function Cb(y As Integer) As ComboBox
            Dim c As New ComboBox()
            c.Font = New Font("Tahoma", 10.0!)
            c.Size = New Size(520, 40)
            c.Location = New Point(250, y)
            c.DropDownStyle = ComboBoxStyle.DropDownList
            Return c
        End Function

        Private Sub InitializeComponent()
            Me.Text = "ตั้งค่าข้อมูลวัด"
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.FormBorderStyle = FormBorderStyle.None
            Me.Dock = DockStyle.Fill
            Me.AutoScroll = True

            lblHeader = New Label()
            lblHeader.Text = "🏛️ ข้อมูลวัด และผู้ทำงาน"
            lblHeader.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(69, 26, 3)
            lblHeader.BackColor = Color.FromArgb(253, 230, 138)
            lblHeader.Dock = DockStyle.Top : lblHeader.Height = 66
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            txtTempleCode = Tb(16)
            txtTempleName = Tb(16 + 40 + 14)
            txtTempleAddress = New TextBox With {.Font = New Font("Tahoma", 10.5!), .Multiline = True, .ScrollBars = ScrollBars.Vertical, .Size = New Size(520, 70), .Location = New Point(250, 16 + 2 * (40 + 14))}
            cboTambon = Cb(16 + 3 * (40 + 14) + 34)
            cboAmphoe = Cb(16 + 4 * (40 + 14) + 34)
            cboProvince = Cb(16 + 5 * (40 + 14) + 34)
            txtPostCode = Tb(16 + 6 * (40 + 14) + 34)
            txtTemplePhone = Tb(16 + 7 * (40 + 14) + 34)

            txtAbbotName = Tb(16 + 8 * (40 + 14) + 34)
            cboAbbotOfficeStatus = Cb(16 + 9 * (40 + 14) + 34)
            cboAbbotOfficeStatus.DropDownStyle = ComboBoxStyle.DropDown
            cboAbbotOfficeStatus.Items.AddRange({"เจ้าคณะรอง", "พระครู", "พระราชาคณะ", "อื่นๆ"})

            txtWaiyawatName = Tb(16 + 10 * (40 + 14) + 34)
            cboWaiyawatOfficeStatus = Cb(16 + 11 * (40 + 14) + 34)
            cboWaiyawatOfficeStatus.DropDownStyle = ComboBoxStyle.DropDown
            cboWaiyawatOfficeStatus.Items.AddRange({"ผู้ดูแลวัด", "อาวาส", "เจ้าสำนัก", "อื่นๆ"})

            txtBookkeeperName = Tb(16 + 12 * (40 + 14) + 34)
            cboBookkeeperType = Cb(16 + 13 * (40 + 14) + 34)
            cboBookkeeperType.DropDownStyle = ComboBoxStyle.DropDown
            cboBookkeeperType.Items.AddRange({"เจ้าหน้าที่วัด", "ชาวบ้านสมัครใจ", "ที่ปรึกษาบัญชี", "อื่นๆ"})

            chkUsePromptPay = New CheckBox With {.Text = "เปิดใช้งานพร้อมเพย์", .Location = New Point(250, 16 + 14 * (40 + 14) + 34), .Size = New Size(280, 40), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .ForeColor = Color.FromArgb(15, 118, 110)}
            txtPromptPayName = Tb(16 + 15 * (40 + 14) + 34)
            txtPromptPayID = Tb(16 + 16 * (40 + 14) + 34)

            Dim yBtns As Integer = 16 + 17 * (40 + 14) + 54
            btnSave = New Button With {.Text = "💾 บันทึกข้อมูล", .BackColor = Color.FromArgb(22, 163, 74), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(220, 56), .Location = New Point(250, yBtns), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnCancel = New Button With {.Text = "🔄 โหลดใหม่", .BackColor = Color.FromArgb(217, 119, 6), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(170, 56), .Location = New Point(480, yBtns), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnLocationImport = New Button With {.Text = "📍 นำเข้าจังหวัด/อำเภอ", .BackColor = Color.FromArgb(37, 99, 235), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(240, 56), .Location = New Point(660, yBtns), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnClose = New Button With {.Text = "ปิด", .BackColor = Color.FromArgb(75, 85, 99), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Size = New Size(120, 56), .Location = New Point(910, yBtns), .Font = New Font("Tahoma", 10.5!, FontStyle.Bold), .Cursor = Cursors.Hand}

            Me.Controls.Add(lblHeader)
            Dim labels As New List(Of Control) From {
                Lbl("รหัสวัด:", 40, 16), Lbl("ชื่อวัด:", 40, 16 + 1 * (40 + 14)),
                Lbl("ที่อยู่วัด:", 40, 16 + 2 * (40 + 14)),
                Lbl("ตำบล:", 40, 16 + 3 * (40 + 14) + 34), Lbl("อำเภอ:", 40, 16 + 4 * (40 + 14) + 34), Lbl("จังหวัด:", 40, 16 + 5 * (40 + 14) + 34),
                Lbl("รหัสไปรษณีย์:", 40, 16 + 6 * (40 + 14) + 34), Lbl("เบอร์ติดต่อวัด:", 40, 16 + 7 * (40 + 14) + 34),
                Lbl("ชื่อสมเด็จพระเจ้าอาวาส:", 40, 16 + 8 * (40 + 14) + 34), Lbl("ตำแหน่งเจ้าอาวาส:", 40, 16 + 9 * (40 + 14) + 34),
                Lbl("ชื่อไวยาวัจกร:", 40, 16 + 10 * (40 + 14) + 34), Lbl("ตำแหน่งไวยาวัจกร:", 40, 16 + 11 * (40 + 14) + 34),
                Lbl("ชื่อผู้ทำบัญชี:", 40, 16 + 12 * (40 + 14) + 34), Lbl("ประเภทผู้ทำบัญชี:", 40, 16 + 13 * (40 + 14) + 34),
                New Label With {.Location = New Point(40, 16 + 14 * (40 + 14) + 34), .Size = New Size(200, 40), .Visible = False},
                Lbl("ชื่อบัญชีพร้อมเพย์:", 40, 16 + 15 * (40 + 14) + 34), Lbl("เลขพร้อมเพย์/เลขบัญชี:", 40, 16 + 16 * (40 + 14) + 34)
            }
            Me.Controls.AddRange(labels.ToArray())
            Me.Controls.AddRange(New Control() {
                txtTempleCode, txtTempleName, txtTempleAddress, cboTambon, cboAmphoe, cboProvince, txtPostCode, txtTemplePhone,
                txtAbbotName, cboAbbotOfficeStatus, txtWaiyawatName, cboWaiyawatOfficeStatus, txtBookkeeperName, cboBookkeeperType,
                chkUsePromptPay, txtPromptPayName, txtPromptPayID,
                btnSave, btnCancel, btnLocationImport, btnClose
            })
        End Sub

        Private Sub FrmTempleSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Db.EnsureSchema()
            LoadLocations()
            LoadTempleData()
            SetupEnterNavigation()
            txtTempleCode.Focus()
        End Sub

        Private Sub SetupEnterNavigation()
            If _enterFlow.Count > 0 Then Return
            _enterFlow.AddRange({
                txtTempleCode, txtTempleName, txtTempleAddress, cboTambon, cboAmphoe, cboProvince,
                txtPostCode, txtTemplePhone, txtAbbotName, cboAbbotOfficeStatus,
                txtWaiyawatName, cboWaiyawatOfficeStatus, txtBookkeeperName, cboBookkeeperType,
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
                txtAbbotName.Text = r("AbbotName")?.ToString()
                TrySetComboText(cboAbbotOfficeStatus, r("AbbotOfficeStatus")?.ToString())
                txtWaiyawatName.Text = r("WaiyawatName")?.ToString()
                TrySetComboText(cboWaiyawatOfficeStatus, r("WaiyawatOfficeStatus")?.ToString())
                txtBookkeeperName.Text = r("BookkeeperName")?.ToString()
                TrySetComboText(cboBookkeeperType, r("BookkeeperType")?.ToString())
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
                    Dim ppName = If(chkUsePromptPay.Checked, txtPromptPayName.Text.Trim(), "")
                    Dim ppID = If(chkUsePromptPay.Checked, txtPromptPayID.Text.Trim(), "")
                    Db.ExecuteNonQuery(conn, "DELETE FROM TempleSetting")
                    Db.InsertAndGetId(conn, "INSERT INTO TempleSetting (TempleCode,TempleName,TempleAddress,Tambon,Amphoe,Province,PostCode,TemplePhone,AbbotName,AbbotOfficeStatus,WaiyawatName,WaiyawatOfficeStatus,BookkeeperName,BookkeeperType,PromptPayName,PromptPayID) VALUES (@a1,@a2,@a3,@a4,@a5,@a6,@a7,@a8,@a9,@a10,@a11,@a12,@a13,@a14,@a15,@a16)",
                        New Tuple(Of String, Object)("@a1", txtTempleCode.Text.Trim()),
                        New Tuple(Of String, Object)("@a2", txtTempleName.Text.Trim()),
                        New Tuple(Of String, Object)("@a3", txtTempleAddress.Text.Trim()),
                        New Tuple(Of String, Object)("@a4", tn),
                        New Tuple(Of String, Object)("@a5", an),
                        New Tuple(Of String, Object)("@a6", pn),
                        New Tuple(Of String, Object)("@a7", txtPostCode.Text.Trim()),
                        New Tuple(Of String, Object)("@a8", txtTemplePhone.Text.Trim()),
                        New Tuple(Of String, Object)("@a9", txtAbbotName.Text.Trim()),
                        New Tuple(Of String, Object)("@a10", cboAbbotOfficeStatus.Text.Trim()),
                        New Tuple(Of String, Object)("@a11", txtWaiyawatName.Text.Trim()),
                        New Tuple(Of String, Object)("@a12", cboWaiyawatOfficeStatus.Text.Trim()),
                        New Tuple(Of String, Object)("@a13", txtBookkeeperName.Text.Trim()),
                        New Tuple(Of String, Object)("@a14", cboBookkeeperType.Text.Trim()),
                        New Tuple(Of String, Object)("@a15", ppName),
                        New Tuple(Of String, Object)("@a16", ppID))
                    MessageBox.Show("✅ บันทึกข้อมูลวัดสำเร็จ!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
