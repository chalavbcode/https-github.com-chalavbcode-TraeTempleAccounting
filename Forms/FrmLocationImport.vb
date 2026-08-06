Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data.OleDb
Imports System.IO

Namespace TempleAccounting
    <DesignerCategory("Form")>
    Partial Public Class FrmLocationImport
        Inherits Form

        Private isImporting As Boolean = False

        Public Sub New()
            InitializeComponent()
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
            Me.KeyPreview = True
            HelpSystem.SetupHelp(Me, "FrmLocationImport")
            Db.EnsureSchema()
            CheckFiles()
            LoadLastImport()
            SetupToolTips()
            btnCheck_Click(Nothing, EventArgs.Empty)
        End Sub

        Private Sub FrmLocationImport_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.F1 Then
                e.Handled = True
                e.SuppressKeyPress = True
                HelpSystem.ShowManual("FrmLocationImport")
            End If
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(btnImport, "นำเข้าข้อมูลจังหวัด/อำเภอ/ตำบลจากไฟล์ CSV ใหม่ (ล้างข้อมูลเก่าตามที่เลือก)")
            ttMain.SetToolTip(btnUpdate, "อัปเดตหรือเพิ่มข้อมูลที่อยู่โดยไม่ลบข้อมูลเดิม")
            ttMain.SetToolTip(btnRebuild, "ล้างข้อมูลที่อยู่ทั้งหมดและสร้างใหม่จากไฟล์ CSV ทันที")
            ttMain.SetToolTip(btnCheck, "ตรวจสอบจำนวนข้อมูลจังหวัด อำเภอ และตำบลที่มีอยู่ในฐานข้อมูลปัจจุบัน")
            ttMain.SetToolTip(btnClose, "ปิดหน้าจอนี้และกลับไปที่หน้าตั้งค่าข้อมูลวัด")
            ttMain.SetToolTip(lnkOpenImportFolder, "เปิดโฟลเดอร์สำหรับใส่ไฟล์ province.csv, amphoe.csv, และ tambon.csv")
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
