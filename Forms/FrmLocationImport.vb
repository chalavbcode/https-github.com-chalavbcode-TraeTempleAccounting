Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data.OleDb
Imports System.IO

Namespace TempleAccounting
    Partial Public Class FrmLocationImport
        Inherits Form

        Private components As IContainer = Nothing
        Friend WithEvents lnkOpenImportFolder As LinkLabel
        Friend WithEvents rtbLog As RichTextBox
        Friend WithEvents prgImport As ProgressBar
        Friend WithEvents chkClearBeforeImport As CheckBox
        Friend WithEvents btnImport, btnUpdate, btnCheck, btnRebuild, btnClose As Button
        Friend WithEvents lblStatus, lblProgress, lblLastImport, lblLastImportTitle As Label
        Friend WithEvents lblProvinceCount, lblDistrictCount, lblSubDistrictCount As Label
        Friend WithEvents lblProvinceFile, lblDistrictFile, Label2, Label3, Label4 As Label
        Friend WithEvents grp1, grp2, grp3, grp4, grp5 As GroupBox
        Friend WithEvents lblHeader As Label

        Private isImporting As Boolean = False

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then components.Dispose()
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.Text = "นำเข้าข้อมูลจังหวัด"
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.Font = New Font("Tahoma", 10.5!)
            Me.FormBorderStyle = FormBorderStyle.None
            Me.Dock = DockStyle.Fill
            Me.AutoScroll = True

            lblHeader = New Label()
            lblHeader.Text = "📍 นำเข้าข้อมูลจังหวัด/อำเภอ/ตำบล"
            lblHeader.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(12, 74, 110)
            lblHeader.BackColor = Color.FromArgb(186, 230, 253)
            lblHeader.Dock = DockStyle.Top : lblHeader.Height = 66
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            grp1 = New GroupBox With {.Text = "ไฟล์ CSV ต้นฉบับ", .Location = New Point(20, 80), .Size = New Size(840, 130), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            lblProvinceFile = New Label With {.Text = "province.csv:", .Location = New Point(20, 30), .AutoSize = True, .Font = New Font("Tahoma", 10.0!)}
            lblDistrictFile = New Label With {.Text = "amphoe.csv:", .Location = New Point(20, 60), .AutoSize = True, .Font = New Font("Tahoma", 10.0!)}
            Label2 = New Label With {.Text = "tambon.csv:", .Location = New Point(20, 90), .AutoSize = True, .Font = New Font("Tahoma", 10.0!)}
            lblProvinceCount = New Label With {.Text = "-", .Location = New Point(200, 30), .AutoSize = True, .ForeColor = Color.FromArgb(22, 101, 52), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            lblDistrictCount = New Label With {.Text = "-", .Location = New Point(200, 60), .AutoSize = True, .ForeColor = Color.FromArgb(22, 101, 52), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            lblSubDistrictCount = New Label With {.Text = "-", .Location = New Point(200, 90), .AutoSize = True, .ForeColor = Color.FromArgb(22, 101, 52), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            lnkOpenImportFolder = New LinkLabel With {.Text = "📂 เปิดโฟลเดอร์ Import", .Location = New Point(580, 30), .AutoSize = True, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            grp1.Controls.AddRange(New Control() {lblProvinceFile, lblDistrictFile, Label2, lblProvinceCount, lblDistrictCount, lblSubDistrictCount, lnkOpenImportFolder})

            grp2 = New GroupBox With {.Text = "ขั้นตอนการนำเข้า", .Location = New Point(20, 220), .Size = New Size(840, 90), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            chkClearBeforeImport = New CheckBox With {.Text = "ล้างข้อมูลเก่าก่อนนำเข้า (แนะนำครั้งแรก)", .Location = New Point(20, 30), .Size = New Size(520, 40), .Font = New Font("Tahoma", 10.0!)}
            btnImport = New Button With {.Text = "1⃣ นำเข้าใหม่", .Location = New Point(380, 22), .Size = New Size(140, 50), .BackColor = Color.FromArgb(37, 99, 235), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnUpdate = New Button With {.Text = "2⃣ อัปเดตเพิ่ม", .Location = New Point(530, 22), .Size = New Size(140, 50), .BackColor = Color.FromArgb(5, 150, 105), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnRebuild = New Button With {.Text = "3⃣ สร้างใหม่ทั้งหมด", .Location = New Point(680, 22), .Size = New Size(140, 50), .BackColor = Color.FromArgb(180, 83, 9), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            grp2.Controls.AddRange(New Control() {chkClearBeforeImport, btnImport, btnUpdate, btnRebuild})

            grp3 = New GroupBox With {.Text = "ตรวจสอบ", .Location = New Point(870, 80), .Size = New Size(330, 230), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            btnCheck = New Button With {.Text = "🔍 นับจำนวนในฐานข้อมูล", .Location = New Point(20, 30), .Size = New Size(290, 52), .BackColor = Color.FromArgb(79, 70, 229), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            Label3 = New Label With {.Text = "รายการในตาราง:", .Location = New Point(20, 96), .AutoSize = True}
            Label4 = New Label With {.Text = "Province / District / SubDistrict", .Location = New Point(20, 124), .AutoSize = True, .Font = New Font("Tahoma", 9.5!), .ForeColor = Color.FromArgb(75, 85, 99)}
            lblStatus = New Label With {.Text = "รอการตรวจสอบ", .Location = New Point(20, 160), .AutoSize = True, .ForeColor = Color.FromArgb(22, 101, 52), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            grp3.Controls.AddRange(New Control() {btnCheck, Label3, Label4, lblStatus})

            grp4 = New GroupBox With {.Text = "Progress", .Location = New Point(20, 320), .Size = New Size(1180, 90), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            prgImport = New ProgressBar With {.Location = New Point(20, 32), .Size = New Size(800, 32), .Style = ProgressBarStyle.Continuous}
            lblProgress = New Label With {.Text = "รอการทำงาน", .Location = New Point(840, 32), .AutoSize = True, .Font = New Font("Tahoma", 10.5!, FontStyle.Bold)}
            grp4.Controls.AddRange(New Control() {prgImport, lblProgress})

            grp5 = New GroupBox With {.Text = "ประวัติการทำงานล่าสุด", .Location = New Point(20, 420), .Size = New Size(1180, 260), .BackColor = Color.White, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(12, 74, 110)}
            rtbLog = New RichTextBox With {.Location = New Point(16, 32), .Size = New Size(1148, 180), .Font = New Font("Consolas", 9.0!), .BackColor = Color.FromArgb(15, 23, 42), .ForeColor = Color.FromArgb(226, 232, 240), .ReadOnly = True}
            lblLastImportTitle = New Label With {.Text = "ครั้งล่าสุด:", .Location = New Point(16, 220), .AutoSize = True}
            lblLastImport = New Label With {.Text = "-", .Location = New Point(120, 220), .AutoSize = True, .ForeColor = Color.FromArgb(79, 70, 229), .Font = New Font("Tahoma", 10.0!, FontStyle.Bold)}
            grp5.Controls.AddRange(New Control() {rtbLog, lblLastImportTitle, lblLastImport})

            btnClose = New Button With {.Text = "ปิดหน้านี้", .Location = New Point(1050, 690), .Size = New Size(150, 50), .BackColor = Color.FromArgb(75, 85, 99), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}

            Me.Controls.AddRange(New Control() {lblHeader, grp1, grp2, grp3, grp4, grp5, btnClose})
        End Sub

        Private Sub Log(msg As String)
            If rtbLog.InvokeRequired Then
                rtbLog.BeginInvoke(Sub() Log(msg))
                Return
            End If
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}" & Environment.NewLine)
            rtbLog.ScrollToCaret()
            Application.DoEvents()
        End Sub

        Private Sub SetProgress(cur As Integer, tot As Integer, Optional msg As String = "")
            If prgImport.InvokeRequired Then
                prgImport.BeginInvoke(Sub() SetProgress(cur, tot, msg))
                Return
            End If
            prgImport.Maximum = Math.Max(1, tot)
            prgImport.Value = Math.Max(0, Math.Min(tot, cur))
            If Not String.IsNullOrEmpty(msg) Then lblProgress.Text = msg
            Application.DoEvents()
        End Sub

        Private Sub FrmLocationImport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Db.EnsureSchema()
            CheckFiles()
            LoadLastImport()
            btnCheck_Click(Nothing, EventArgs.Empty)
        End Sub

        Private Sub CheckFiles()
            lblProvinceCount.Text = If(File.Exists(AppPaths.ProvinceCsv()), New FileInfo(AppPaths.ProvinceCsv()).Length.ToString("#,##0") & " bytes", "❌ ไม่พบไฟล์")
            lblDistrictCount.Text = If(File.Exists(AppPaths.DistrictCsv()), New FileInfo(AppPaths.DistrictCsv()).Length.ToString("#,##0") & " bytes", "❌ ไม่พบไฟล์")
            lblSubDistrictCount.Text = If(File.Exists(AppPaths.SubDistrictCsv()), New FileInfo(AppPaths.SubDistrictCsv()).Length.ToString("#,##0") & " bytes", "❌ ไม่พบไฟล์")
        End Sub

        Private Sub lnkOpenImportFolder_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkOpenImportFolder.LinkClicked
            Try
                Process.Start(New ProcessStartInfo With {.FileName = AppPaths.ImportFolder, .UseShellExecute = True})
            Catch
                Clipboard.SetText(AppPaths.ImportFolder)
                MessageBox.Show("Path ของโฟลเดอร์ Import: " & AppPaths.ImportFolder & " (คัดลอกแล้ววางใน Explorer ได้เลย)", "แจ้งเตือน")
            End Try
        End Sub

        Private Sub LoadLastImport()
            Dim f = Path.Combine(AppPaths.LogsFolder, "LastLocationImport.txt")
            If File.Exists(f) Then lblLastImport.Text = File.ReadAllText(f)
        End Sub
        Private Sub SaveLastImport(msg As String)
            File.WriteAllText(Path.Combine(AppPaths.LogsFolder, "LastLocationImport.txt"), msg)
            lblLastImport.Text = msg
        End Sub

        Private Sub btnCheck_Click(sender As Object, e As EventArgs) Handles btnCheck.Click
            Try
                Using conn = Db.OpenConn()
                    Dim p = CInt(Db.DbScalar(conn, "SELECT COUNT(*) FROM Province"))
                    Dim a = CInt(Db.DbScalar(conn, "SELECT COUNT(*) FROM District"))
                    Dim t = CInt(Db.DbScalar(conn, "SELECT COUNT(*) FROM SubDistrict"))
                    lblStatus.Text = $"Province: {p} / District: {a} / SubDistrict: {t}"
                    Log($"ตรวจสอบ: จังหวัด {p} แถว, อำเภอ {a} แถว, ตำบล {t} แถว")
                End Using
            Catch ex As Exception
                lblStatus.Text = "❌ " & ex.Message
            End Try
        End Sub

        Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
            If isImporting Then Return
            isImporting = True
            Try
                RunImport(clearBefore:=chkClearBeforeImport.Checked, rebuild:=False)
            Finally
                isImporting = False
            End Try
        End Sub
        Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
            If isImporting Then Return
            isImporting = True
            Try
                RunImport(clearBefore:=False, rebuild:=False)
            Finally
                isImporting = False
            End Try
        End Sub
        Private Sub btnRebuild_Click(sender As Object, e As EventArgs) Handles btnRebuild.Click
            If isImporting Then Return
            If MessageBox.Show("จะล้างข้อมูลจังหวัดทั้งหมดและนำเข้าใหม่ทุกแถว?", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
            isImporting = True
            Try
                RunImport(clearBefore:=True, rebuild:=True)
            Finally
                isImporting = False
            End Try
        End Sub

        Private Sub RunImport(clearBefore As Boolean, rebuild As Boolean)
            Log("=== เริ่มทำงาน Import ===")
            CheckFiles()
            If Not File.Exists(AppPaths.ProvinceCsv()) OrElse Not File.Exists(AppPaths.DistrictCsv()) OrElse Not File.Exists(AppPaths.SubDistrictCsv()) Then
                MessageBox.Show("ไม่พบไฟล์ CSV ในโฟลเดอร์ Import กรุณาดาวน์โหลดไฟล์ province/amphoe/tambon.csv มาใส่ก่อน", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            Using conn = Db.OpenConn()
                If clearBefore Then
                    Db.ExecuteNonQuery(conn, "DELETE FROM SubDistrict")
                    Db.ExecuteNonQuery(conn, "DELETE FROM District")
                    Db.ExecuteNonQuery(conn, "DELETE FROM Province")
                    Log("ล้างข้อมูลเก่าแล้ว")
                End If

                Dim provLines = File.ReadAllLines(AppPaths.ProvinceCsv(), System.Text.Encoding.UTF8)
                Dim amphoeLines = File.ReadAllLines(AppPaths.DistrictCsv(), System.Text.Encoding.UTF8)
                Dim tambonLines = File.ReadAllLines(AppPaths.SubDistrictCsv(), System.Text.Encoding.UTF8)
                Dim total = provLines.Length + amphoeLines.Length + tambonLines.Length
                Dim cur As Integer = 0
                Dim pIns As Integer = 0, aIns As Integer = 0, tIns As Integer = 0

                Log($"อ่านไฟล์ได้ จังหวัด {provLines.Length} บรรทัด / อำเภอ {amphoeLines.Length} / ตำบล {tambonLines.Length}")

                For Each ln In provLines
                    cur += 1 : SetProgress(cur, total, $"จังหวัด {cur}/{provLines.Length}")
                    If String.IsNullOrWhiteSpace(ln) Then Continue For
                    Dim cols = ln.Split(","c)
                    If cols.Length < 2 Then Continue For
                    Dim id As Integer : If Not Integer.TryParse(cols(0).Trim(), id) Then Continue For
                    If id < 0 Then Continue For
                    If rebuild Then
                        Db.ExecuteNonQuery(conn, "INSERT INTO Province (ProvinceID, ProvinceName) VALUES (@i,@n)", New Tuple(Of String, Object)("@i", id), New Tuple(Of String, Object)("@n", cols(1).Trim()))
                        pIns += 1
                    Else
                        Dim exist = Db.DbScalar(conn, "SELECT 1 FROM Province WHERE ProvinceID=@i", New Tuple(Of String, Object)("@i", id))
                        If exist Is Nothing Then
                            Db.ExecuteNonQuery(conn, "INSERT INTO Province (ProvinceID, ProvinceName) VALUES (@i,@n)", New Tuple(Of String, Object)("@i", id), New Tuple(Of String, Object)("@n", cols(1).Trim()))
                            pIns += 1
                        Else
                            Db.ExecuteNonQuery(conn, "UPDATE Province SET ProvinceName=@n WHERE ProvinceID=@i", New Tuple(Of String, Object)("@i", id), New Tuple(Of String, Object)("@n", cols(1).Trim()))
                        End If
                    End If
                Next
                Log("Import จังหวัดสำเร็จ -> เพิ่ม " & pIns & " แถว")

                For Each ln In amphoeLines
                    cur += 1 : SetProgress(cur, total, $"อำเภอ {cur - provLines.Length}/{amphoeLines.Length}")
                    If String.IsNullOrWhiteSpace(ln) Then Continue For
                    Dim cols = ln.Split(","c)
                    If cols.Length < 3 Then Continue For
                    Dim did As Integer, pid As Integer
                    If Not Integer.TryParse(cols(0).Trim(), did) Then Continue For
                    If Not Integer.TryParse(cols(1).Trim(), pid) Then Continue For
                    If did < 0 OrElse pid < 0 Then Continue For
                    If rebuild Then
                        Db.ExecuteNonQuery(conn, "INSERT INTO District (DistrictID, ProvinceID, DistrictName) VALUES (@d,@p,@n)", New Tuple(Of String, Object)("@d", did), New Tuple(Of String, Object)("@p", pid), New Tuple(Of String, Object)("@n", cols(2).Trim()))
                        aIns += 1
                    Else
                        Dim exist = Db.DbScalar(conn, "SELECT 1 FROM District WHERE DistrictID=@d", New Tuple(Of String, Object)("@d", did))
                        If exist Is Nothing Then
                            Db.ExecuteNonQuery(conn, "INSERT INTO District (DistrictID, ProvinceID, DistrictName) VALUES (@d,@p,@n)", New Tuple(Of String, Object)("@d", did), New Tuple(Of String, Object)("@p", pid), New Tuple(Of String, Object)("@n", cols(2).Trim()))
                            aIns += 1
                        Else
                            Db.ExecuteNonQuery(conn, "UPDATE District SET ProvinceID=@p, DistrictName=@n WHERE DistrictID=@d", New Tuple(Of String, Object)("@d", did), New Tuple(Of String, Object)("@p", pid), New Tuple(Of String, Object)("@n", cols(2).Trim()))
                        End If
                    End If
                Next
                Log("Import อำเภอสำเร็จ -> เพิ่ม " & aIns & " แถว")

                For Each ln In tambonLines
                    cur += 1 : SetProgress(cur, total, $"ตำบล {cur - provLines.Length - amphoeLines.Length}/{tambonLines.Length}")
                    If String.IsNullOrWhiteSpace(ln) Then Continue For
                    Dim cols = ln.Split(","c)
                    If cols.Length < 4 Then Continue For
                    Dim sid As Integer, did As Integer
                    If Not Integer.TryParse(cols(0).Trim(), sid) Then Continue For
                    If Not Integer.TryParse(cols(1).Trim(), did) Then Continue For
                    If sid < 0 OrElse did < 0 Then Continue For
                    Dim zip = cols(3).Trim()
                    If rebuild Then
                        Db.ExecuteNonQuery(conn, "INSERT INTO SubDistrict (SubDistrictID, DistrictID, SubDistrictName, ZipCode) VALUES (@s,@d,@n,@z)", New Tuple(Of String, Object)("@s", sid), New Tuple(Of String, Object)("@d", did), New Tuple(Of String, Object)("@n", cols(2).Trim()), New Tuple(Of String, Object)("@z", zip))
                        tIns += 1
                    Else
                        Dim exist = Db.DbScalar(conn, "SELECT 1 FROM SubDistrict WHERE SubDistrictID=@s", New Tuple(Of String, Object)("@s", sid))
                        If exist Is Nothing Then
                            Db.ExecuteNonQuery(conn, "INSERT INTO SubDistrict (SubDistrictID, DistrictID, SubDistrictName, ZipCode) VALUES (@s,@d,@n,@z)", New Tuple(Of String, Object)("@s", sid), New Tuple(Of String, Object)("@d", did), New Tuple(Of String, Object)("@n", cols(2).Trim()), New Tuple(Of String, Object)("@z", zip))
                            tIns += 1
                        Else
                            Db.ExecuteNonQuery(conn, "UPDATE SubDistrict SET DistrictID=@d, SubDistrictName=@n, ZipCode=@z WHERE SubDistrictID=@s", New Tuple(Of String, Object)("@s", sid), New Tuple(Of String, Object)("@d", did), New Tuple(Of String, Object)("@n", cols(2).Trim()), New Tuple(Of String, Object)("@z", zip))
                        End If
                    End If
                Next
                Log("Import ตำบลสำเร็จ -> เพิ่ม " & tIns & " แถว")
                SaveLastImport($"{DateTime.Now:dd MMM yyyy HH:mm} เพิ่ม จังหวัด {pIns} / อำเภอ {aIns} / ตำบล {tIns}")
            End Using
            SetProgress(1, 1, "✅ สำเร็จ")
            btnCheck_Click(Nothing, EventArgs.Empty)
            MessageBox.Show("นำเข้าข้อมูลจังหวัดสำเร็จ!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Dim f = TryCast(Me.ParentForm, frmMain)
            If f IsNot Nothing Then
                f.ShowFormInPanel(New FrmTempleSetting(), "🏛️ ตั้งค่าข้อมูลวัด")
            Else
                Me.Close()
            End If
        End Sub
    End Class
End Namespace
