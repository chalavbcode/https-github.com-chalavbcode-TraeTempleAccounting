Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Data.OleDb
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Namespace TempleAccounting
    ''' <summary>
    ''' ฟอร์มส่งออกข้อมูลไปเครื่องอื่น (Export Data for Transfer / Flash Drive)
    ''' - ตั้งค่า MachineID ครั้งเดียวต่อเครื่อง (เก็บในตาราง LocalSettings) — จะไม่ส่งออกถ้ายังไม่ได้ตั้ง
    ''' - ส่งออกเป็นไฟล์ .xlsx ที่มี Header Block (Metadata) แยกจาก Data:
    '''     Metadata: SchemaVersion, MachineID, ExportDateTime, RowCount, DateRangeFrom/To
    '''     Data: TransactionGUID, TranDate, Type, Description, Amount, Category, Fund, Bank, Note
    ''' - ExportHistory ใช้เพื่อ "ส่งออกเฉพาะรายการใหม่หลังการส่งออกครั้งล่าสุด"
    ''' - หลักการสำคัญ: ไม่อ่าน/แก้ข้อมูลต้นทาง (SELECT เท่านั้น) ไม่ทำเครื่องหมายสถานะใดๆ ใน Transactions
    '''   ไม่มี scheduled export — เป็นการกดปุ่ม manual เท่านั้น
    ''' ฝั่งคู่กัน: FrmMultiMachineImport (นำเข้า/รวมข้อมูล) — โครงสร้างไฟล์นิยามกลางใน TransferFormat.vb
    ''' </summary>
    <DesignerCategory("Form")>
    Partial Public Class FrmExportForTransfer
        Inherits Form

        ' ====================== โครงสร้างข้อมูล ======================
        Private Class ExportRow
            Public Property TransactionGUID As String
            Public Property TranDate As Date
            Public Property TranType As String        ' "Income" / "Expense"
            Public Property Description As String
            Public Property Amount As Decimal
            Public Property Category As String
            Public Property Fund As String
            Public Property Bank As String
            Public Property Note As String
            Public Property ReceiptName As String        ' ชื่อไฟล์แนบใน DB (เช่น Receipt_12.jpg) — "" = ไม่มี
            Public Property ReceiptSourceFile As String ' path จริงของไฟล์แนบที่หาเจอ ("" = ไม่พบ)
        End Class

        ' ====================== State ======================
        Private _machineId As String = ""

        Public Sub New()
            InitializeComponent()
        End Sub

        ' ====================== UI Events ======================
        Private Sub FrmExportForTransfer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.KeyPreview = True
            HelpSystem.SetupHelp(Me, "FrmExportForTransfer", "คำแนะนำ: กดปุ่ม [F1] เพื่อดูวิธีใช้งานหน้านี้")
            Db.EnsureSchema()
            EnsureExportSchema()
            SetupToolTips()
            UiFitter.AutoFitFormButtons(Me)
            LoadMachineId()
            LoadLastExportInfo()
            Log("ระบบพร้อมใช้งาน — ตั้งค่ารหัสเครื่องครั้งเดียวต่อเครื่อง แล้วกดปุ่มส่งออก")
        End Sub

        Private Sub FrmExportForTransfer_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.F1 Then
                e.Handled = True
                e.SuppressKeyPress = True
                HelpSystem.ShowManual("FrmExportForTransfer", Me)
            End If
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(txtMachineID, "รหัสเครื่องต้นทาง เช่น KIOSK-01 / สำนักงานใหญ่ — ตั้งครั้งเดียวต่อเครื่อง ฝังลงในไฟล์ส่งออกทุกไฟล์")
            ttMain.SetToolTip(btnSaveMachineID, "บันทึกรหัสเครื่องลงในเครื่องนี้ — ใช้เป็นค่าเดียวกันกับที่ฝั่งนำเข้าอ่านจากไฟล์")
            ttMain.SetToolTip(btnExportForTransfer, "ส่งออกข้อมูลเป็นไฟล์ .xlsx สำหรับโอนไปเครื่องอื่น/แฟลชไดรฟ์")
            ttMain.SetToolTip(btnOpenExportFolder, "เปิดโฟลเดอร์ที่ใช้บันทึกไฟล์ส่งออก")
            ttMain.SetToolTip(btnClose, "ปิดหน้าจอนี้และกลับไปที่หน้ารายการเงินรับ-จ่าย")
        End Sub

        Private Sub btnSaveMachineID_Click(sender As Object, e As EventArgs) Handles btnSaveMachineID.Click
            SaveMachineId("")
        End Sub

        Private Sub btnOpenExportFolder_Click(sender As Object, e As EventArgs) Handles btnOpenExportFolder.Click
            Try
                System.Diagnostics.Process.Start("explorer.exe", AppPaths.ExportFolder)
            Catch ex As Exception
                MessageBox.Show("ไม่สามารถเปิดโฟลเดอร์ได้: " & ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            ' ฟอร์มนี้เปิดแบบ Modal Dialog (.ShowDialog()) — ปิดแค่ dialog ตัวเอง ฟอร์มหลักที่ฝังใน panel ไม่ถูกแตะต้อง
            Me.Close()
        End Sub

        ' ====================== 1. Machine ID (ตั้งครั้งเดียวต่อเครื่อง) ======================

        Private Sub LoadMachineId()
            Try
                Using conn = Db.OpenConn()
                    Dim dt = Db.GetTable(conn, "SELECT TOP 1 KeyValue FROM LocalSettings WHERE KeyName='MachineID'")
                    If dt.Rows.Count > 0 Then _machineId = Convert.ToString(dt.Rows(0)(0)).Trim()
                End Using
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmExportForTransfer.LoadMachineId")
            End Try

            If _machineId.Length = 0 Then
                lblMachineStatus.Text = "⚠️ ยังไม่ได้ตั้งค่ารหัสเครื่อง — ต้องตั้งก่อนส่งออกครั้งแรก (ระบบจะไม่ส่งออกถ้าไม่มีรหัสเครื่อง)"
                lblMachineStatus.ForeColor = Color.FromArgb(153, 27, 27)
            Else
                lblMachineStatus.Text = "✅ รหัสเครื่องปัจจุบัน: " & _machineId
                lblMachineStatus.ForeColor = Color.FromArgb(22, 101, 52)
            End If
        End Sub

        Private Sub SaveMachineId(Optional machineId As String = "")
            Dim newValue = If(machineId.Length > 0, machineId, txtMachineID.Text.Trim())
            If newValue.Length = 0 Then
                MessageBox.Show("กรุณากรอกรหัสเครื่อง (เช่น KIOSK-01, สำนักงานใหญ่)", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtMachineID.Focus()
                Return
            End If
            If newValue.Length > 50 Then newValue = newValue.Substring(0, 50)

            Using conn = Db.OpenConn()
                Dim exists = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM LocalSettings WHERE KeyName='MachineID'"))
                If exists > 0 Then
                    Db.ExecuteNonQuery(conn, "UPDATE LocalSettings SET KeyValue=@v WHERE KeyName='MachineID'",
                        New Tuple(Of String, Object)("@v", newValue))
                Else
                    Db.ExecuteNonQuery(conn, "INSERT INTO LocalSettings (KeyName, KeyValue) VALUES ('MachineID', @v)",
                        New Tuple(Of String, Object)("@v", newValue))
                End If
            End Using

            _machineId = newValue
            txtMachineID.Text = ""
            LoadMachineId()
            Log("✅ ตั้งค่ารหัสเครื่องแล้ว: " & _machineId)
            MessageBox.Show("ตั้งค่ารหัสเครื่องแล้ว: " & _machineId, "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        ' ====================== 2. ช่วงข้อมูล + ส่งออก ======================

        Private Sub btnExportForTransfer_Click(sender As Object, e As EventArgs) Handles btnExportForTransfer.Click
            ' หลักการข้อ 1: ถ้ายังไม่เคยตั้ง MachineID ให้ตั้งก่อน — ห้าม export โดยไม่มี MachineID
            If _machineId.Length = 0 Then
                MessageBox.Show("กรุณาตั้งค่ารหัสเครื่องก่อนส่งออกข้อมูล" & vbCrLf & vbCrLf &
                                "กรอกรหัส เช่น KIOSK-01 แล้วกด [💾 ตั้งค่า/บันทึกรหัสเครื่อง]" & vbCrLf &
                                "ระบบจะไม่ส่งออกโดยไม่มีรหัสเครื่อง เพราะฝั่งนำเข้าจำเป็นต้องใช้รหัสนี้ในการติดตามข้อมูล",
                                "ตั้งค่ารหัสเครื่องก่อน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtMachineID.Focus()
                Return
            End If

            Dim rangeFrom As Date = DateTime.MinValue
            Dim rangeTo As Date = DateTime.MinValue
            Dim rangeDesc As String = ""
            If Not TryGetDateRange(rangeFrom, rangeTo, rangeDesc) Then Return

            Dim rows As List(Of ExportRow)
            Try
                Log("🔎 อ่านข้อมูลช่วง: " & rangeDesc & " ...")
                rows = LoadRowsForExport(rangeFrom, rangeTo)
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmExportForTransfer.LoadRows")
                MessageBox.Show("อ่านข้อมูลไม่สำเร็จ: " & ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try

            If rows.Count = 0 Then
                MessageBox.Show("ไม่มีรายการข้อมูลในช่วงที่เลือก" & vbCrLf & "ถ้าใช้โหมด 'เฉพาะรายการใหม่' อาจหมายถึงข้อมูลส่งออกไปแล้วก่อนหน้านี้", "ไม่มีข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Log("⏭️ ไม่มีข้อมูลในช่วง: " & rangeDesc)
                Return
            End If

            ' [ไฟล์แนบ] วิเคราะห์ไฟล์แนบ (ใบเสร็จ/สลิป) ที่จะพาไปด้วย — แสดงสรุปก่อน export จริง (กัน Flash Drive เต็ม)
            Dim rcFound As Integer = 0
            Dim rcMissing As Integer = 0
            Dim rcNoGuid As Integer = 0
            Dim rcBytes As Long = 0
            For Each r In rows
                If r.TransactionGUID.Length = 0 Then
                    rcNoGuid += 1
                    Continue For
                End If
                If r.ReceiptSourceFile.Length > 0 AndAlso File.Exists(r.ReceiptSourceFile) Then
                    rcFound += 1
                    Try : rcBytes += New FileInfo(r.ReceiptSourceFile).Length : Catch : End Try
                Else
                    rcMissing += 1
                End If
            Next

            Dim sizeText As String = "ไฟล์แนบ: " & rcFound.ToString("#,##0") & " ไฟล์" &
                If(rcMissing > 0, " / ไม่พบต้นฉบับ " & rcMissing.ToString("#,##0") & " รายการ (ส่งเฉพาะข้อมูล)", "") &
                If(rcNoGuid > 0, " / ไม่มีรหัสอ้างอิง " & rcNoGuid.ToString("#,##0") & " รายการ (โอนไฟล์แนบไม่ได้)", "")
            If rcBytes > 0 Then sizeText &= vbCrLf & "ขนาดไฟล์แนบรวมโดยประมาณ: " & FormatBytes(rcBytes)

            Dim confirmMsg = "📤 สรุปการส่งออก" & vbCrLf & vbCrLf &
                "รหัสเครื่อง: " & _machineId & vbCrLf &
                "ช่วงข้อมูล: " & rangeDesc & vbCrLf &
                "รายการ: " & rows.Count.ToString("#,##0") & " รายการ" & vbCrLf &
                sizeText & vbCrLf & vbCrLf &
                "ยืนยันการส่งออกหรือไม่?"
            If MessageBox.Show(confirmMsg, "ยืนยันการส่งออก", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Log("⏭️ ผู้ใช้ยกเลิก (หน้ายืนยัน)")
                Return
            End If

            ' เลือกตำแหน่งบันทึกเอง (Flash Drive / โฟลเดอร์ที่ผู้ดูแลกำหนด) — ไม่มี scheduled export
            ' ระบบจะสร้างโฟลเดอร์ Export_<MachineID>_<datetime> ภายใน ประกอบด้วย TransactionData.xlsx + โฟลเดอร์ย่อย Receipts/ (สเปคไฟล์แนบข้อ 2)
            Using fbd As New FolderBrowserDialog()
                fbd.Description = "เลือกโฟลเดอร์ปลายทาง (เช่น แฟลชไดรฟ์) — ระบบจะสร้างโฟลเดอร์ Export_... ภายใน"
                fbd.SelectedPath = AppPaths.ExportFolder
                If fbd.ShowDialog() <> DialogResult.OK OrElse String.IsNullOrEmpty(fbd.SelectedPath) Then
                    Log("⏭️ ผู้ใช้ยกเลิกการเลือกตำแหน่งบันทึก")
                    Return
                End If

                Dim folderName = "Export_" & SanitizeFileName(_machineId) & "_" & DateTime.Now.ToString("yyyyMMdd_HHmm")
                Dim exportFolder = Path.Combine(fbd.SelectedPath, folderName)
                If Directory.Exists(exportFolder) Then
                    folderName &= "_" & DateTime.Now.ToString("ss")
                    exportFolder = Path.Combine(fbd.SelectedPath, folderName)
                End If

                btnExportForTransfer.Enabled = False
                prgExport.Value = 0
                prgExport.Maximum = Math.Max(rows.Count, 1)
                Try
                    Directory.CreateDirectory(exportFolder)
                    WriteExportFile(exportFolder, rangeFrom, rangeTo, rows, rangeDesc)
                Catch ex As Exception
                    AppPaths.LogCrash(ex, "FrmExportForTransfer.Write")
                    Log("❌ ส่งออกไม่สำเร็จ: " & ex.Message)
                    MessageBox.Show(FormatUserError(ex, "การส่งออก") & vbCrLf & "โฟลเดอร์ที่สร้างไว้จะถูกลบ (ข้อมูลต้นทางไม่ถูกแตะต้อง)",
                                    "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Try
                        If Directory.Exists(exportFolder) Then Directory.Delete(exportFolder, True)
                    Catch
                    End Try
                    Return
                Finally
                    btnExportForTransfer.Enabled = True
                End Try
            End Using
        End Sub

        Private Function TryGetDateRange(ByRef rangeFrom As Date, ByRef rangeTo As Date, ByRef rangeDesc As String) As Boolean
            ' จนถึงสิ้นวันนี้ (ต้องการให้แถวที่บันทึกเมื่อกี้รวมอยู่ในไฟล์ด้วย)
            rangeTo = DateTime.Today.AddDays(1).AddSeconds(-1)

            If rbSinceLast.Checked Then
                ' ต่อจาก ExportHistory ของเครื่องนี้ — resume point
                Dim lastTo As Object = Nothing
                Try
                    Using conn = Db.OpenConn()
                        lastTo = Db.DbScalar(conn, "SELECT MAX(DateRangeTo) FROM ExportHistory WHERE MachineID=@m",
                                             New Tuple(Of String, Object)("@m", _machineId))
                    End Using
                Catch ex As Exception
                    AppPaths.LogCrash(ex, "FrmExportForTransfer.LastExport")
                End Try

                If lastTo Is Nothing OrElse lastTo Is DBNull.Value Then
                    rangeFrom = New Date(2000, 1, 1)
                    rangeDesc = "ส่งออกครั้งแรกของเครื่องนี้ (ครอบคลุมข้อมูลทั้งหมดที่มี)"
                Else
                    rangeFrom = Db.NormalizeGregorianDate(CDate(lastTo)).Date.AddDays(1)
                    rangeDesc = "เฉพาะรายการใหม่หลังส่งออกครั้งล่าสุด (" & CDate(lastTo).ToString("dd/MM/yyyy HH:mm") & ")"
                End If
            ElseIf rbCustom.Checked Then
                rangeFrom = dtpFrom.Value.Date
                rangeTo = dtpTo.Value.Date.AddDays(1).AddSeconds(-1)
                If rangeFrom > dtpTo.Value.Date Then
                    MessageBox.Show("วันที่เริ่มต้นต้องไม่เกินวันที่สิ้นสุด", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
                rangeDesc = "ช่วงวันที่กำหนด: " & rangeFrom.ToString("dd/MM/yyyy") & " - " & dtpTo.Value.Date.ToString("dd/MM/yyyy")
            Else
                ' rbAll — กรณีพิเศษ เช่น ตั้งเครื่องใหม่
                rangeFrom = New Date(2000, 1, 1)
                rangeDesc = "ข้อมูลทั้งหมด (ทุกช่วงวันที่)"
            End If
            Return True
        End Function

        ''' <summary>อ่านข้อมูลต้นทางด้วย SELECT เท่านั้น — ไม่แก้/ไม่ทำเครื่องหมายใดๆ ใน Transactions (ข้อควรระวังข้อ 2)</summary>
        Private Function LoadRowsForExport(rangeFrom As Date, rangeTo As Date) As List(Of ExportRow)
            Dim rows As New List(Of ExportRow)()
            Using conn = Db.OpenConn()
                Dim sql = "SELECT t.TransactionGUID, DateValue(t.TranDate) AS TranDate, t.TranType AS Type, t.[Detail] AS Description, t.Amount, " &
                          "c.CategoryName AS Category, f.FundName AS Fund, b.BankName AS Bank, IIF(t.[Note] IS NULL,'',t.[Note]) AS Remark, " &
                          "IIF(t.ReceiptPath IS NULL,'',t.ReceiptPath) AS ReceiptName " &
                          "FROM ((Transactions t LEFT JOIN Categories c ON t.CategoryID=c.ID) " &
                          "LEFT JOIN Funds f ON t.FundID=f.ID) LEFT JOIN BankAccounts b ON t.BankID=b.ID " &
                          "WHERE DateValue(t.TranDate) >= @d1 AND DateValue(t.TranDate) <= @d2 ORDER BY t.TranDate, t.ID"
                Dim dt = Db.GetTable(conn, sql,
                    New Tuple(Of String, Object)("@d1", rangeFrom.Date),
                    New Tuple(Of String, Object)("@d2", rangeTo))

                For Each r As DataRow In dt.Rows
                    Dim td As Date = DateTime.MinValue
                    Try
                        td = Db.NormalizeGregorianDate(CDate(r("TranDate")))
                    Catch
                    End Try
                    Dim rcName = SafeStr(r, "ReceiptName")
                    rows.Add(New ExportRow() With {
                        .TransactionGUID = SafeStr(r, "TransactionGUID"),
                        .TranDate = td,
                        .TranType = Convert.ToString(r("Type")).Trim(),
                        .Description = SafeStr(r, "Description"),
                        .Amount = Db.ToDecimalOrZero(r("Amount")),
                        .Category = SafeStr(r, "Category"),
                        .Fund = SafeStr(r, "Fund"),
                        .Bank = SafeStr(r, "Bank"),
                        .Note = SafeStr(r, "Remark"),
                        .ReceiptName = rcName,
                        .ReceiptSourceFile = If(rcName.Length > 0, ReceiptImageHelper.ResolveReceiptPath(rcName), "")
                    })
                Next
            End Using
            Return rows
        End Function

        ''' <summary>
        ''' เขียนชุดไฟล์ export ลงใน exportFolder: TransactionData.xlsx (Metadata + Data) + โฟลเดอร์ย่อย Receipts/ (ไฟล์แนบตั้งชื่อ GUID)
        ''' แล้วบันทึก ExportHistory เฉพาะเมื่อสำเร็จ
        ''' </summary>
        Private Sub WriteExportFile(exportFolder As String, rangeFrom As Date, rangeTo As Date, rows As List(Of ExportRow), rangeDesc As String)
            Dim filePath = Path.Combine(exportFolder, "TransactionData.xlsx")
            Log("📦 กำลังเขียนไฟล์ข้อมูล: " & Path.GetFileName(filePath) & " (" & rows.Count.ToString("#,##0") & " แถว) ...")

            ' ACE จะเพิ่มชีตซ้ำถ้าไฟล์มีอยู่เดิม — ลบไฟล์เก่าออกก่อน (ชื่อไฟล์มีเวลากำกับ ไม่ชนกันอยู่แล้ว)
            If File.Exists(filePath) Then File.Delete(filePath)

            Dim connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & filePath & ";Extended Properties=""Excel 12.0 Xml;HDR=YES;"""
            Using conn As New OleDbConnection(connStr)
                conn.Open()
                Dim cmd = conn.CreateCommand()

                ' สร้างชีต Data (ข้อมูล) — ชื่อคอลัมน์ตรงกับที่ FrmMultiMachineImport อ่าน (ดู TransferFormat.vb)
                ' หมายเหตุ: หลีกเลี่ยงชื่อ "Note" เพราะเป็น reserved word ใน Access DDL ใช้ "Remark" (import อ่านได้จาก alias)
                cmd.CommandText = "CREATE TABLE [" & TransferFormat.DataSheetName & "] (TransactionGUID TEXT(36), TranDate DATETIME, Type TEXT(10), Description TEXT(255), Amount CURRENCY, Category TEXT(100), Fund TEXT(100), Bank TEXT(100), Remark TEXT(255))"
                cmd.ExecuteNonQuery()

                ' สร้างชีต Metadata (Header Block) — import ตรวจก่อนเปิดข้อมูลจริง
                cmd.CommandText = "CREATE TABLE [" & TransferFormat.MetadataSheetName & "] (FieldName TEXT(50), FieldValue TEXT(255))"
                cmd.ExecuteNonQuery()

                ' Header Block
                Dim meta As New List(Of Tuple(Of String, Object))()
                meta.Add(New Tuple(Of String, Object)(TransferFormat.FieldSchemaVersion, TransferFormat.SchemaVersion))
                meta.Add(New Tuple(Of String, Object)(TransferFormat.FieldMachineID, _machineId))
                meta.Add(New Tuple(Of String, Object)(TransferFormat.FieldExportDateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")))
                meta.Add(New Tuple(Of String, Object)(TransferFormat.FieldRowCount, rows.Count))
                meta.Add(New Tuple(Of String, Object)(TransferFormat.FieldDateRangeFrom, rangeFrom.Date.ToString("yyyy-MM-dd")))
                meta.Add(New Tuple(Of String, Object)(TransferFormat.FieldDateRangeTo, rangeTo.Date.ToString("yyyy-MM-dd")))
                For Each m In meta
                    ' หมายเหตุ: Excel ACE OLEDB ต้องใช้ positional "?" + CreateParameter() ไร้ชื่อเท่านั้น (named param ใช้ไม่ได้)
                    cmd.CommandText = "INSERT INTO [" & TransferFormat.MetadataSheetName & "$] (FieldName, FieldValue) VALUES (?, ?)"
                    cmd.Parameters.Clear()
                    AddUnnamedParam(cmd, m.Item1)
                    AddUnnamedParam(cmd, m.Item2)
                    cmd.ExecuteNonQuery()
                Next

                ' Data rows — parameterized insert (ไฟล์ transfer มีขนาดไม่ใหญ่, เร็วพอ)
                Dim idx As Integer = 0
                For Each r In rows
                    ' วันที่เขียนเป็น ISO "yyyy-MM-dd" เพื่อให้อ่านกลับเป็นค่าคงที่ทุก locale (ACE อ่านวันที่เป็น String ตามเครื่อง)
                    cmd.CommandText = "INSERT INTO [" & TransferFormat.DataSheetName & "$] (TransactionGUID, TranDate, Type, Description, Amount, Category, Fund, Bank, Remark) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)"
                    cmd.Parameters.Clear()
                    AddUnnamedParam(cmd, If(r.TransactionGUID.Length > 0, r.TransactionGUID, Nothing))
                    AddUnnamedParam(cmd, r.TranDate.ToString("yyyy-MM-dd"))
                    AddUnnamedParam(cmd, r.TranType)
                    AddUnnamedParam(cmd, If(r.Description.Length > 0, r.Description, Nothing))
                    AddUnnamedParam(cmd, r.Amount)
                    AddUnnamedParam(cmd, If(r.Category.Length > 0, r.Category, Nothing))
                    AddUnnamedParam(cmd, If(r.Fund.Length > 0, r.Fund, Nothing))
                    AddUnnamedParam(cmd, If(r.Bank.Length > 0, r.Bank, Nothing))
                    AddUnnamedParam(cmd, If(r.Note.Length > 0, r.Note, Nothing))
                    cmd.ExecuteNonQuery()
                    idx += 1
                    If idx Mod 200 = 0 Then SetProgress(idx, rows.Count, "เขียนข้อมูล " & idx.ToString("#,##0") & "/" & rows.Count.ToString("#,##0") & " แถว")
                Next
            End Using

            ' [ไฟล์แนบ] copy ไฟล์แนบที่ optimize แล้วไปยังโฟลเดอร์ย่อย Receipts/ ตั้งชื่อด้วย TransactionGUID (ฝั่งนำเข้าจับคู่ตาม GUID)
            ' - แถวไม่มี GUID / ไม่พบต้นฉบับ → ข้าม แต่นับไว้ในสรุป (ห้ามทำให้ export ล้มเหลวทั้งชุด)
            Dim rcCopied As Integer = 0
            Dim rcCopyFailed As Integer = 0
            Dim rcSkippedNoGuid As Integer = 0
            Dim rcSkippedNoFile As Integer = 0
            Dim receiptsDir = Path.Combine(exportFolder, "Receipts")
            For Each r In rows
                If r.TransactionGUID.Length = 0 Then
                    rcSkippedNoGuid += 1
                    Continue For
                End If
                If r.ReceiptSourceFile.Length = 0 OrElse Not File.Exists(r.ReceiptSourceFile) Then
                    rcSkippedNoFile += 1
                    Continue For
                End If
                Try
                    If Not Directory.Exists(receiptsDir) Then Directory.CreateDirectory(receiptsDir)
                    File.Copy(r.ReceiptSourceFile, Path.Combine(receiptsDir, r.TransactionGUID & ".jpg"), True)
                    rcCopied += 1
                Catch ex As Exception
                    rcCopyFailed += 1
                    Log("⚠️ คัดลอกไฟล์แนบไม่สำเร็จ (" & r.TransactionGUID & "): " & ex.Message)
                End Try
            Next
            If rcCopied > 0 Then Log("🖼️ คัดลอกไฟล์แนบสำเร็จ " & rcCopied & " ไฟล์ → " & Path.Combine(Path.GetFileName(exportFolder), "Receipts\"))

            ' บันทึก ExportHistory เฉพาะเมื่อเขียนไฟล์สำเร็จแล้วเท่านั้น (resume point ของโหมด "เฉพาะรายการใหม่")
            Dim batchId As Guid = Guid.NewGuid()
            Using conn = Db.OpenConn()
                Dim oledbConn = TryCast(conn, OleDbConnection)
                Using cmd = oledbConn.CreateCommand()
                    cmd.CommandText = "INSERT INTO ExportHistory (ExportBatchID, ExportDateTime, DateRangeFrom, DateRangeTo, RowCount, FileNameGenerated, MachineID) VALUES (@b, @dt, @f, @t, @n, @fn, @m)"
                    Dim gp = cmd.CreateParameter()
                    gp.ParameterName = "@b"
                    gp.OleDbType = OleDbType.Guid
                    gp.Value = batchId
                    cmd.Parameters.Add(gp)
                    cmd.Parameters.AddWithValue("@dt", DateTime.Now)
                    cmd.Parameters.AddWithValue("@f", rangeFrom)
                    cmd.Parameters.AddWithValue("@t", rangeTo)
                    cmd.Parameters.AddWithValue("@n", rows.Count)
                    cmd.Parameters.AddWithValue("@fn", Path.GetFileName(exportFolder))
                    cmd.Parameters.AddWithValue("@m", _machineId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' สรุปยอดรับ-จ่ายในไฟล์
            Dim sumIncome As Decimal = 0D
            Dim sumExpense As Decimal = 0D
            For Each r In rows
                If r.TranType = "Income" Then sumIncome += r.Amount Else sumExpense += r.Amount
            Next

            SetProgress(rows.Count, rows.Count, "เสร็จสิ้น")
            Log("✅ ส่งออกสำเร็จ: " & Path.GetFileName(exportFolder))
            Log("   ช่วงข้อมูล: " & rangeDesc & " | ยอดรับ " & sumIncome.ToString("#,##0.00") & " / ยอดจ่าย " & sumExpense.ToString("#,##0.00") & " บาท")
            If rcCopied > 0 OrElse rcCopyFailed > 0 OrElse rcSkippedNoGuid > 0 OrElse rcSkippedNoFile > 0 Then
                Log("   ไฟล์แนบ: สำเร็จ " & rcCopied & " / คัดลอกล้มเหลว " & rcCopyFailed & " / ไม่มีรหัสอ้างอิง " & rcSkippedNoGuid & " / ไม่พบต้นฉบับ " & rcSkippedNoFile)
            End If
            LoadLastExportInfo()

            Dim attachLine As String = ""
            If rcCopied > 0 OrElse rcSkippedNoGuid > 0 OrElse rcSkippedNoFile > 0 Then
                attachLine = "ไฟล์แนบ: " & rcCopied.ToString("#,##0") & " ไฟล์" &
                    If(rcCopyFailed > 0, " (คัดลอกล้มเหลว " & rcCopyFailed & ")", "") &
                    If(rcSkippedNoFile > 0, " | ไม่พบต้นฉบับ " & rcSkippedNoFile & " รายการ — ส่งเฉพาะข้อมูล", "") &
                    If(rcSkippedNoGuid > 0, " | ไม่มีรหัสอ้างอิง " & rcSkippedNoGuid & " รายการ — โอนไฟล์แนบไม่ได้", "") &
                    vbCrLf
            End If

            MessageBox.Show(
                "🎉 ส่งออกข้อมูลสำเร็จ!" & vbCrLf & vbCrLf &
                "โฟลเดอร์: " & Path.GetFileName(exportFolder) & vbCrLf &
                "รหัสเครื่อง: " & _machineId & vbCrLf &
                "รายการ: " & rows.Count.ToString("#,##0") & " รายการ" & vbCrLf &
                "ยอดรับรวม: " & sumIncome.ToString("#,##0.00") & " บาท" & vbCrLf &
                "ยอดจ่ายรวม: " & sumExpense.ToString("#,##0.00") & " บาท" & vbCrLf &
                attachLine & vbCrLf &
                "💡 นำโฟลเดอร์นี้ (ทั้งไฟล์ TransactionData.xlsx และโฟลเดอร์ Receipts) ไปเปิดในฝั่ง 'นำเข้าข้อมูลหลายเครื่อง' ของเครื่องเป้าหมาย",
                "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        ' ====================== 3. Schema + ข้อมูลล่าสุด ======================

        ''' <summary>สร้าง LocalSettings + ExportHistory แบบ non-breaking (รันอัตโนมัติตอนเปิดฟอร์ม)</summary>
        Private Sub EnsureExportSchema()
            Try
                Using conn = Db.OpenConn()
                    Dim oledbConn = TryCast(conn, OleDbConnection)
                    Dim tables = oledbConn.GetSchema("Tables")

                    Dim hasLocal As Boolean = False
                    Dim hasExport As Boolean = False
                    For Each r As DataRow In tables.Rows
                        Dim t = Convert.ToString(r("TABLE_NAME"))
                        If t.Equals("LocalSettings", StringComparison.OrdinalIgnoreCase) Then hasLocal = True
                        If t.Equals("ExportHistory", StringComparison.OrdinalIgnoreCase) Then hasExport = True
                    Next

                    If Not hasLocal Then
                        Db.ExecuteNonQuery(conn, "CREATE TABLE LocalSettings (ID COUNTER PRIMARY KEY, KeyName TEXT(50) NOT NULL, KeyValue TEXT(255))")
                        Log("✅ สร้างตารางตั้งค่าเครื่องแล้ว")
                    End If

                    If Not hasExport Then
                        Db.ExecuteNonQuery(conn, "CREATE TABLE ExportHistory (ExportBatchID GUID PRIMARY KEY, ExportDateTime DATETIME, DateRangeFrom DATETIME, DateRangeTo DATETIME, RowCount INTEGER, FileNameGenerated TEXT(255), MachineID TEXT(50))")
                        Log("✅ สร้างตารางประวัติการส่งออกแล้ว")
                    End If

                    ' กันกรณีฐานข้อมูลเก่าที่คอลัมน์ TransactionGUID ยังไม่มี (ปกติ Db.EnsureSchema สร้างให้แล้ว)
                    Dim colNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                    Dim cols = oledbConn.GetSchema("Columns", New String() {Nothing, Nothing, "Transactions", Nothing})
                    For Each r As DataRow In cols.Rows
                        colNames.Add(Convert.ToString(r("COLUMN_NAME")))
                    Next
                    If Not colNames.Contains("TransactionGUID") Then
                        Db.ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN TransactionGUID TEXT(36)")
                        Log("✅ เพิ่มคอลัมน์รหัสอ้างอิงในตารางธุรกรรมแล้ว (ไม่ทำลายข้อมูลเดิม)")
                    End If
                End Using
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmExportForTransfer.EnsureExportSchema")
                MessageBox.Show("ไม่สามารถตรวจสอบ/สร้างโครงสร้างตารางได้: " & ex.Message,
                                "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub

        Private Sub LoadLastExportInfo()
            Try
                Using conn = Db.OpenConn()
                    Dim dt = Db.GetTable(conn, "SELECT TOP 1 ExportDateTime, RowCount, FileNameGenerated FROM ExportHistory ORDER BY ExportDateTime DESC")
                    If dt.Rows.Count > 0 Then
                        Dim d = Db.NormalizeGregorianDate(CDate(dt.Rows(0)("ExportDateTime")))
                        lblLastExport.Text = "ครั้งล่าสุด: " & d.ToString("dd/MM/yyyy HH:mm") &
                                             " (" & Db.ToIntOrZero(dt.Rows(0)("RowCount")).ToString("#,##0") & " แถว — " & SafeStr(dt.Rows(0), "FileNameGenerated") & ")"
                    Else
                        lblLastExport.Text = "ครั้งล่าสุด: ยังไม่เคยส่งออก"
                    End If
                End Using
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmExportForTransfer.LoadLastExport")
            End Try
        End Sub

        ' ====================== Helpers ======================

        Private Shared Function SafeStr(row As DataRow, colName As String) As String
            If row Is Nothing OrElse row.Table Is Nothing OrElse Not row.Table.Columns.Contains(colName) Then Return ""
            If row.IsNull(colName) Then Return ""
            Return Convert.ToString(row(colName)).Replace(ChrW(160), " ").Trim()
        End Function

        ''' <summary>เพิ่ม parameter แบบ positional (ไร้ชื่อ) — จำเป็นสำหรับ Excel ACE OLEDB ซึ่งใช้ AddWithValue/named param ไม่ได้</summary>
        Private Shared Sub AddUnnamedParam(cmd As OleDbCommand, value As Object)
            Dim p = cmd.CreateParameter()
            p.Value = If(value Is Nothing, DBNull.Value, value)
            cmd.Parameters.Add(p)
        End Sub

        ''' <summary>จัดรูปแบบขนาดไฟล์ให้อ่านง่าย (B / KB / MB)</summary>
        Private Shared Function FormatBytes(bytes As Long) As String
            If bytes < 1024 Then Return bytes.ToString("#,##0") & " B"
            If bytes < 1024 * 1024 Then Return (bytes / 1024).ToString("#,##0.0") & " KB"
            Return (bytes / (1024 * 1024)).ToString("#,##0.00") & " MB"
        End Function

        Private Shared Function SanitizeFileName(value As String) As String
            Dim sb As New StringBuilder()
            For Each ch In value.Trim()
                If Array.IndexOf(Path.GetInvalidFileNameChars(), ch) >= 0 Then
                    sb.Append("-")
                ElseIf ch = " "c Then
                    sb.Append("-")
                Else
                    sb.Append(ch)
                End If
            Next
            If sb.Length = 0 Then sb.Append("Machine")
            Return sb.ToString()
        End Function

        Private Sub Log(msg As String)
            Try
                If rtbLog Is Nothing Then Return
                rtbLog.AppendText("[" & DateTime.Now.ToString("HH:mm:ss") & "] " & msg & vbCrLf)
                rtbLog.SelectionStart = rtbLog.TextLength
                rtbLog.ScrollToCaret()
            Catch
            End Try
        End Sub

        Private Sub SetProgress(value As Integer, maximum As Integer, status As String)
            Try
                If maximum > 0 Then prgExport.Maximum = maximum
                prgExport.Value = Math.Max(0, Math.Min(value, prgExport.Maximum))
                lblProgress.Text = status
            Catch
            End Try
        End Sub

        ''' <summary>
        ''' แปลง Exception เป็นข้อความภาษาไทยที่ผู้ใช้เข้าใจได้ โดย log เต็มแยกไว้ใน AppPaths.LogCrash แล้ว
        ''' </summary>
        Private Shared Function FormatUserError(ex As Exception, operationThai As String) As String
            Select Case True
                Case TypeOf ex Is OleDbException
                    Return "ไม่สามารถเชื่อมต่อกับฐานข้อมูลได้ — ไฟล์ฐานข้อมูลอาจเปิดอยู่ในโปรแกรมอื่นหรือเสียหาย รหัสอ้างอิง: " & DateTime.Now.ToString("yyyyMMddHHmmss")
                Case TypeOf ex Is IOException
                    Return "ไม่สามารถอ่านหรือเขียนไฟล์ได้ — อุปกรณ์จัดเก็บข้อมูลอาจเต็ม ถูกถอดออก หรือไม่มีสิทธิ์เข้าถึง รหัสอ้างอิง: " & DateTime.Now.ToString("yyyyMMddHHmmss")
                Case TypeOf ex Is InvalidOperationException
                    Return "การทำงานของ " & operationThai & " ไม่สมบูรณ์ — ข้อมูลอาจถูกใช้โดยอีกกระบวนการหนึ่ง รหัสอ้างอิง: " & DateTime.Now.ToString("yyyyMMddHHmmss")
                Case Else
                    Return "เกิดข้อผิดพลาดระหว่าง" & operationThai & " — หากพบปัญหาซ้ำ กรุณาติดต่อผู้ดูแลระบบ รหัสอ้างอิง: " & DateTime.Now.ToString("yyyyMMddHHmmss")
            End Select
        End Function
    End Class
End Namespace
