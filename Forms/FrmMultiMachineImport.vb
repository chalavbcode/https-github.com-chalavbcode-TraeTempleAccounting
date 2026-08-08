Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Data.OleDb
Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Windows.Forms

Namespace TempleAccounting
    ''' <summary>
    ''' ฟอร์มนำเข้าและรวมข้อมูลธุรกรรมออฟไลน์จากหลายเครื่อง / หลาย Flash Drive
    ''' มาสู่ฐานข้อมูลหลัก TempleAccounting.accdb
    ''' - อ่าน MachineID ที่ฝังในไฟล์ (ไม่ใช้ชื่อไฟล์/ชื่อเครื่อง Windows เป็นตัวระบุหลัก)
    ''' - คำนวณ SHA-256 ทั้งไฟล์ และเช็คกับตาราง ImportHistory เพื่อกันนำเข้าซ้ำ
    ''' - ตรวจสอบซ้ำแบบ Smart Deduplication (TransactionGUID -> composite key)
    ''' - รวมข้อมูลด้วย OLEDB Transaction + สำรองฐานข้อมูลอัตโนมัติก่อน merge
    ''' หมายเหตุ: ห้ามแก้ไข ExcelTransactionImporter.vb เดิม — ฟอร์มนี้ implement แยกต่างหาก
    ''' </summary>
    <DesignerCategory("Form")>
    Partial Public Class FrmMultiMachineImport
        Inherits Form

        ' ====================== โครงสร้างข้อมูล (per-file / per-row) ======================
        Private Class ParsedRow
            Public Property RowNumber As Integer
            Public Property TranDate As Date
            Public Property TranType As String        ' "Income" / "Expense"
            Public Property Detail As String
            Public Property Amount As Decimal
            Public Property CategoryName As String
            Public Property FundName As String
            Public Property BankName As String
            Public Property Note As String
            Public Property TransactionGUID As String
            Public Property MachineID As String
            Public Property DateWarning As String      ' "" / "⚠️ วันที่ในอนาคต" / "⚠️ วันที่เก่าผิดปกติ"
            Public Property ErrorMsg As String         ' "" = แถวใช้ได้
            Public Property IsDuplicateExact As Boolean       ' ตรงกับ TransactionGUID (ซ้ำแน่นอน)
            Public Property IsSuspectedDuplicate As Boolean   ' ตรงกับ composite key (สงสัยซ้ำ)
        End Class

        Private Class FileAnalysis
            Public Property FilePath As String
            Public Property FileHash As String
            Public Property MachineID As String
            Public Property MachineIDSource As String   ' "ฝังในไฟล์" / "ใช้ชื่อไฟล์แทน (ความน่าเชื่อถือต่ำ)" / ""
            Public Property PreviousImportInfo As String ' "" หรือ "เมื่อ dd/MM/yyyy โดย xxx"
            Public Property Rows As New List(Of ParsedRow)
            Public Property SchemaError As String
            Public Property IncomeSum As Decimal
            Public Property ExpenseSum As Decimal
            Public Property GuidDuplicateCount As Integer
            Public Property SuspectedDuplicateCount As Integer
            Public Property DateWarningCount As Integer
            Public Property RowErrorCount As Integer
        End Class

        ' ====================== State ======================
        Private _files As New List(Of FileAnalysis)
        Private _isMerging As Boolean = False
        Private _dbGuidSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Private _dbCompositeSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ' ====================== Column Aliases (รองรับทั้งไทย/อังกฤษ) ======================
        Private ReadOnly DateAliases As String() = {"วันที่", "date", "trandate", "transactiondate", "วันทำรายการ"}
        Private ReadOnly DetailAliases As String() = {"รายการ", "detail", "description", "รายละเอียด", "desc"}
        Private ReadOnly AmountAliases As String() = {"จำนวนเงิน", "amount", "ยอดเงิน", "ยอด", "money"}
        Private ReadOnly TypeAliases As String() = {"ประเภท", "type", "trantype", "transacttype", "รับจ่าย", "ชนิด"}
        Private ReadOnly CategoryAliases As String() = {"หมวด", "category", "categoryname", "หมวดหมู่", "ประเภทรายการ"}
        Private ReadOnly FundAliases As String() = {"กองทุน", "fund", "fundname"}
        Private ReadOnly BankAliases As String() = {"ธนาคาร", "bank", "bankname", "bankinfo", "บัญชีธนาคาร"}
        Private ReadOnly NoteAliases As String() = {"หมายเหตุ", "note", "remark", "remarks"}
        Private ReadOnly MachineIdAliases As String() = {"machineid", "kioskcode", "รหัสเครื่อง", "รหัสเครื่องต้นทาง", "machine"}
        Private ReadOnly GuidAliases As String() = {"transactionguid", "guid", "transactionid", "รหัสธุรกรรม", "txid", "uuid"}

        Public Sub New()
            InitializeComponent()
        End Sub

        ' ====================== UI Events ======================
        Private Sub FrmMultiMachineImport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.KeyPreview = True
            HelpSystem.SetupHelp(Me, "FrmMultiMachineImport")
            Db.EnsureSchema()
            EnsureImportSchema()
            SetupToolTips()
            UiFitter.AutoFitFormButtons(Me)
            Log("ระบบพร้อมใช้งาน — เลือกไฟล์ Export จากเครื่องปลายทาง (เลือกได้หลายไฟล์) เพื่อตรวจสอบและรวมข้อมูล")
        End Sub

        Private Sub FrmMultiMachineImport_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.F1 Then
                e.Handled = True
                e.SuppressKeyPress = True
                HelpSystem.ShowManual("FrmMultiMachineImport", Me)
            End If
        End Sub

        Private Sub SetupToolTips()
            ttMain.SetToolTip(btnSelectFiles, "เลือกไฟล์ Export (.xlsx) จากหลายเครื่อง/Flash Drive ได้พร้อมกันหลายไฟล์")
            ttMain.SetToolTip(btnClearFiles, "ล้างรายการไฟล์ที่เลือกทั้งหมด")
            ttMain.SetToolTip(btnViewDuplicates, "ดูรายละเอียดรายการที่ตรวจพบว่าซ้ำ/สงสัยซ้ำ ก่อนตัดสินใจรวมข้อมูล")
            ttMain.SetToolTip(btnConfirmMerge, "สำรองฐานข้อมูลอัตโนมัติ แล้วรวมข้อมูลทั้งหมดลงฐานข้อมูลหลัก (Transaction เดียว)")
            ttMain.SetToolTip(btnClose, "ปิดหน้าจอนี้และกลับไปที่หน้ารายการเงินรับ-จ่าย")
        End Sub

        Private Sub btnSelectFiles_Click(sender As Object, e As EventArgs) Handles btnSelectFiles.Click
            Using ofd As New OpenFileDialog()
                ofd.Title = "เลือกไฟล์ Export จากหลายเครื่อง / Flash Drive (เลือกได้หลายไฟล์)"
                ofd.Filter = "ไฟล์ Excel Export (*.xlsx;*.xlsm;*.xlsb;*.xls)|*.xlsx;*.xlsm;*.xlsb;*.xls|ไฟล์ทั้งหมด (*.*)|*.*"
                ofd.Multiselect = True
                ofd.CheckFileExists = True
                If ofd.ShowDialog() <> DialogResult.OK Then Return

                Dim addedCount As Integer = 0
                Dim fallbackCount As Integer = 0
                For Each p In ofd.FileNames
                    Dim before = _files.Count
                    AddFileForAnalysis(p)
                    If _files.Count > before Then
                        addedCount += 1
                        If _files(_files.Count - 1).MachineIDSource = "ใช้ชื่อไฟล์แทน (ความน่าเชื่อถือต่ำ)" Then fallbackCount += 1
                    End If
                Next

                If addedCount = 0 Then Return

                If fallbackCount > 0 Then
                    MessageBox.Show(
                        "⚠️ ไม่พบรหัสเครื่องต้นทาง (MachineID / KioskCode) ในไฟล์จำนวน " & fallbackCount & " ไฟล์" & vbCrLf & vbCrLf &
                        "ระบบจะใช้ชื่อไฟล์แทน (ความน่าเชื่อถือต่ำ)" & vbCrLf &
                        "กรุณาตรวจสอบว่าไฟล์ export จากเครื่องปลายทางมีคอลัมน์ MachineID หรือ KioskCode ฝังอยู่ในไฟล์",
                        "คำเตือน: ไม่พบรหัสเครื่องต้นทาง", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                RefreshPreviewGrid()
                Log("=== เลือกไฟล์ " & addedCount & " ไฟล์ เรียบร้อย (รวม " & _files.Count & " ไฟล์ในรายการ) ===")
            End Using
        End Sub

        Private Sub btnClearFiles_Click(sender As Object, e As EventArgs) Handles btnClearFiles.Click
            _files.Clear()
            RefreshPreviewGrid()
            Log("🗑️ ล้างรายการไฟล์แล้ว")
        End Sub

        Private Sub btnViewDuplicates_Click(sender As Object, e As EventArgs) Handles btnViewDuplicates.Click
            ShowDuplicateDetailDialog(Nothing)
        End Sub

        Private Sub dgvImportPreview_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvImportPreview.CellDoubleClick
            If e.RowIndex < 0 OrElse dgvImportPreview.Rows(e.RowIndex).Tag Is Nothing Then Return
            ShowDuplicateDetailDialog(TryCast(dgvImportPreview.Rows(e.RowIndex).Tag, FileAnalysis))
        End Sub

        Private Sub btnConfirmMerge_Click(sender As Object, e As EventArgs) Handles btnConfirmMerge.Click
            If _isMerging Then Return
            If _files.Count = 0 Then
                MessageBox.Show("กรุณาเลือกไฟล์ Export ก่อน", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            ' รวมสถิติ
            Dim totalValid As Integer = 0
            Dim exactDup As Integer = 0
            Dim suspected As Integer = 0
            For Each f In _files
                If f.SchemaError.Length > 0 Then Continue For
                For Each r In f.Rows
                    If r.ErrorMsg.Length > 0 Then Continue For
                    totalValid += 1
                    If r.IsDuplicateExact Then exactDup += 1
                    If r.IsSuspectedDuplicate Then suspected += 1
                Next
            Next

            If totalValid = 0 Then
                MessageBox.Show("ไม่มีแถวข้อมูลที่นำเข้าได้ (ตรวจสอบ schema / ข้อมูลผิดพลาดในคอลัมน์ Preview)", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' ตัดสินใจกับรายการ "สงสัยซ้ำ" — ห้าม auto-skip แบบเงียบๆ
            Dim confirmedSkipSuspected As Boolean = True
            If suspected > 0 Then
                Dim r = MessageBox.Show(
                    "พบรายการสงสัยซ้ำ " & suspected.ToString("#,##0") & " รายการ" & vbCrLf &
                    "(ตรวจจาก composite key: วันที่ + จำนวนเงิน + ประเภท + รายละเอียด + รหัสเครื่อง เทียบกับฐานข้อมูลหลัก)" & vbCrLf & vbCrLf &
                    "กด [Yes] = ข้ามรายการสงสัยซ้ำ (แนะนำ)" & vbCrLf &
                    "กด [No] = นำเข้าทุกแถว รวมรายการสงสัยซ้ำ" & vbCrLf &
                    "กด [Cancel] = ยกเลิกการรวมข้อมูล",
                    "ยืนยันรายการสงสัยซ้ำ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                If r = DialogResult.Cancel Then
                    LogMergeOutcome("Cancelled")
                    Log("⛔ ผู้ใช้ยกเลิกการรวมข้อมูล (มีรายการสงสัยซ้ำ)")
                    Return
                End If
                confirmedSkipSuspected = (r = DialogResult.Yes)
            End If

            ' ยืนยันสุดท้าย
            Dim msg As String = "📊 สรุปก่อนรวมข้อมูล" & vbCrLf &
                "ไฟล์ที่เลือก: " & _files.Count & " ไฟล์" & vbCrLf &
                "แถวที่นำเข้าได้: " & totalValid.ToString("#,##0") & " รายการ" & vbCrLf &
                "ข้าม (GUID ซ้ำแน่นอน): " & exactDup.ToString("#,##0") & " รายการ"
            If suspected > 0 Then
                msg &= vbCrLf & "ข้าม (สงสัยซ้ำ): " & If(confirmedSkipSuspected, suspected.ToString("#,##0") & " รายการ", "0 รายการ (นำเข้าทั้งหมด)")
            End If
            msg &= vbCrLf & vbCrLf & "ระบบจะสำรองฐานข้อมูลอัตโนมัติก่อนรวมทุกครั้ง" & vbCrLf & "ยืนยันการรวมข้อมูลหรือไม่?"

            If MessageBox.Show(msg, "ยืนยันการรวมข้อมูล", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                LogMergeOutcome("Cancelled")
                Log("⛔ ผู้ใช้ยกเลิกการรวมข้อมูล (หน้ายืนยันสุดท้าย)")
                Return
            End If

            ' ป้องกันการกดปุ่มซ้ำระหว่างประมวลผล (กัน transaction ซ้อนกัน)
            btnConfirmMerge.Enabled = False
            _isMerging = True
            Try
                RunMerge(confirmedSkipSuspected)
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmMultiMachineImport.Merge")
                LogMergeOutcome("Failed")
                Log("❌ เกิดข้อผิดพลาดระหว่างรวมข้อมูล: " & ex.Message)
                MessageBox.Show("เกิดข้อผิดพลาดระหว่างการรวมข้อมูล: " & ex.Message & vbCrLf & "ระบบ Rollback ข้อมูลทั้งหมดแล้ว", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                btnConfirmMerge.Enabled = True
                _isMerging = False
            End Try
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Dim f = TryCast(Me.ParentForm, frmMain)
            If f IsNot Nothing Then
                f.ShowFormInPanel(New FrmTransactions(), "📖 รายการเงินรับ-จ่ายทั้งหมด (ค้นหา/แก้ไข/ลบ)")
            Else
                Me.Close()
            End If
        End Sub

        ' ====================== 2. Machine ID + File Hash (SHA-256) ======================

        ''' <summary>คำนวณ SHA-256 ของไฟล์ทั้งไฟล์ (กันนำเข้าซ้ำแม้เปลี่ยนชื่อไฟล์)</summary>
        Private Shared Function ComputeSha256(filePath As String) As String
            Using fs As FileStream = File.OpenRead(filePath)
                Using sha As SHA256 = SHA256.Create()
                    Dim hashBytes = sha.ComputeHash(fs)
                    Dim sb As New StringBuilder(64)
                    For Each b In hashBytes
                        sb.Append(b.ToString("x2"))
                    Next
                    Return sb.ToString()
                End Using
            End Using
        End Function

        ''' <summary>ค้นหาประวัติ import สำเร็จที่ hash ตรงกัน (ถ้าเคยนำเข้าแล้ว)</summary>
        Private Function FindPreviousImport(hash As String) As DataRow
            Try
                Using conn = Db.OpenConn()
                    Dim dt = Db.GetTable(conn,
                        "SELECT TOP 1 ImportDateTime, ImportedBy, SourceFileName FROM ImportHistory WHERE FileHash=@h AND ImportStatus='Success' ORDER BY ImportDateTime DESC",
                        New Tuple(Of String, Object)("@h", hash))
                    If dt.Rows.Count > 0 Then Return dt.Rows(0)
                End Using
            Catch
            End Try
            Return Nothing
        End Function

        ''' <summary>เพิ่มไฟล์เข้า list + คำนวณ hash + ตรวจ ImportHistory (ไม่เคย auto-skip แบบเงียบๆ)</summary>
        Private Sub AddFileForAnalysis(filePath As String)
            If _files.Any(Function(f) String.Equals(f.FilePath, filePath, StringComparison.OrdinalIgnoreCase)) Then
                MessageBox.Show("ไฟล์นี้ถูกเลือกไปแล้ว: " & Path.GetFileName(filePath), "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim hash As String = ""
            Try
                hash = ComputeSha256(filePath)
            Catch ex As Exception
                MessageBox.Show("ไม่สามารถคำนวณ File Hash (SHA-256) ของไฟล์นี้ได้: " & ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
            Log("🔑 SHA-256: " & hash.Substring(0, 12) & "... (" & Path.GetFileName(filePath) & ")")

            Dim prev = FindPreviousImport(hash)
            If prev IsNot Nothing Then
                Dim prevDate As String = ""
                Try : prevDate = CDate(prev("ImportDateTime")).ToString("dd/MM/yyyy HH:mm") : Catch : End Try
                Dim prevUser = Convert.ToString(prev("ImportedBy"))
                If prevUser.Length = 0 Then prevUser = "-"
                Dim ans = MessageBox.Show(
                    "⚠️ ไฟล์นี้เคยถูกนำเข้าระบบแล้วเมื่อ " & prevDate & " โดย " & prevUser & vbCrLf &
                    "ไฟล์เดิมที่บันทึก: " & Convert.ToString(prev("SourceFileName")) & vbCrLf & vbCrLf &
                    "กด [Yes] = นำเข้าซ้ำแบบตั้งใจ (ระบบจะตรวจสอบรายการซ้ำอีกครั้ง)" & vbCrLf &
                    "กด [No] = ข้ามไฟล์นี้",
                    "ไฟล์เคยถูกนำเข้าแล้ว (SHA-256 ตรงกัน)", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                If ans = DialogResult.No Then
                    Log("⏭️ ข้ามไฟล์ (เคย import สำเร็จแล้ว): " & Path.GetFileName(filePath))
                    Return
                End If
            End If

            Dim analysis = ParseExcelFile(filePath)
            analysis.FileHash = hash
            If prev IsNot Nothing Then
                Dim prevDate As String = ""
                Try : prevDate = CDate(prev("ImportDateTime")).ToString("dd/MM/yyyy HH:mm") : Catch : End Try
                analysis.PreviousImportInfo = "เมื่อ " & prevDate & " โดย " & Convert.ToString(prev("ImportedBy"))
            End If

            ' MachineID fallback: ถ้าไม่พบ field ฝังในไฟล์ ให้ใช้ชื่อไฟล์แทน (ความน่าเชื่อถือต่ำ) — ไม่ block
            If analysis.SchemaError.Length = 0 AndAlso analysis.MachineIDSource <> "ฝังในไฟล์" Then
                analysis.MachineID = Path.GetFileNameWithoutExtension(filePath)
                analysis.MachineIDSource = "ใช้ชื่อไฟล์แทน (ความน่าเชื่อถือต่ำ)"
            End If

            _files.Add(analysis)
            If analysis.SchemaError.Length > 0 Then
                Log("❌ ไฟล์ " & Path.GetFileName(filePath) & ": " & analysis.SchemaError)
            Else
                Log("📄 " & Path.GetFileName(filePath) & ": " & analysis.Rows.Count & " แถว | เครื่อง=" & analysis.MachineID & " (" & analysis.MachineIDSource & ")")
            End If
        End Sub

        ' ====================== 1. อ่านไฟล์ Excel + Validate Schema ======================

        Private Function ParseExcelFile(filePath As String) As FileAnalysis
            Dim analysis As New FileAnalysis()
            analysis.FilePath = filePath

            Dim ext = Path.GetExtension(filePath).ToLowerInvariant()
            Dim connStr As String
            Select Case ext
                Case ".xlsx", ".xlsm", ".xlsb"
                    connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & filePath & ";Extended Properties=""Excel 12.0 Xml;HDR=YES;IMEX=1"";"
                Case ".xls"
                    connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & filePath & ";Extended Properties=""Excel 8.0;HDR=YES;IMEX=1"";"
                Case Else
                    analysis.SchemaError = "นามสกุลไฟล์ไม่รองรับ (รองรับ .xlsx, .xlsm, .xlsb, .xls)"
                    Return analysis
            End Select

            Try
                Using excelConn As New OleDbConnection(connStr)
                    excelConn.Open()
                    Dim sheetNames = GetWorksheetNames(excelConn)
                    If sheetNames.Count = 0 Then
                        analysis.SchemaError = "ไม่พบชีตข้อมูลในไฟล์ Excel"
                        Return analysis
                    End If

                    For Each sheetName In sheetNames
                        Dim dt = LoadWorksheet(excelConn, sheetName)
                        Dim dateCol = FindColumn(dt, DateAliases)
                        Dim detailCol = FindColumn(dt, DetailAliases)
                        Dim amountCol = FindColumn(dt, AmountAliases)
                        Dim typeCol = FindColumn(dt, TypeAliases)
                        If dateCol = "" OrElse detailCol = "" OrElse amountCol = "" OrElse typeCol = "" Then Continue For

                        ' พบชีตที่ schema ครบ (วันที่/รายการ/จำนวนเงิน/ประเภท) — เริ่มอ่าน
                        ' MachineID: อ่านจากคอลัมน์ MachineID/KioskCode (ฝังในไฟล์) ไม่ใช่ชื่อไฟล์
                        Dim machineCol = FindColumn(dt, MachineIdAliases)
                        Dim guidCol = FindColumn(dt, GuidAliases)
                        If machineCol <> "" Then
                            For Each r As DataRow In dt.Rows
                                Dim mv = SafeCellText(r, machineCol)
                                If mv.Length > 0 Then
                                    analysis.MachineID = mv
                                    analysis.MachineIDSource = "ฝังในไฟล์"
                                    Exit For
                                End If
                            Next
                        End If

                        Dim categoryCol = FindColumn(dt, CategoryAliases)
                        Dim fundCol = FindColumn(dt, FundAliases)
                        Dim bankCol = FindColumn(dt, BankAliases)
                        Dim noteCol = FindColumn(dt, NoteAliases)

                        Dim rows As New List(Of ParsedRow)()
                        For rowIndex As Integer = 0 To dt.Rows.Count - 1
                            Dim row = dt.Rows(rowIndex)
                            If IsBlankImportRow(row, {dateCol, detailCol, amountCol, typeCol}) Then Continue For

                            Dim pr As New ParsedRow() With {.RowNumber = rowIndex + 2}
                            Dim d As Date
                            If Not TryParseImportDate(row(dateCol), d) Then
                                pr.ErrorMsg = "วันที่ไม่ถูกต้อง"
                            Else
                                pr.TranDate = d.Date
                                ' Sanity check วันที่ (นาฬิกาเครื่องต้นทางเพี้ยน)
                                If pr.TranDate > DateTime.Today.AddDays(2) Then
                                    pr.DateWarning = "⚠️ วันที่ในอนาคต"
                                ElseIf pr.TranDate < New Date(2000, 1, 1) Then
                                    pr.DateWarning = "⚠️ วันที่เก่าผิดปกติ"
                                End If
                            End If

                            pr.Detail = SafeCellText(row, detailCol).Trim()
                            If pr.Detail.Length = 0 Then
                                pr.ErrorMsg = AppendError(pr.ErrorMsg, "ไม่มีรายละเอียดรายการ")
                            End If

                            Dim amt As Decimal
                            If Not TryParseImportAmount(row(amountCol), amt) OrElse amt <= 0D Then
                                pr.ErrorMsg = AppendError(pr.ErrorMsg, "จำนวนเงินไม่ถูกต้อง")
                            Else
                                pr.Amount = amt
                            End If

                            pr.TranType = ResolveTranType(SafeCellText(row, typeCol))
                            If pr.TranType.Length = 0 Then
                                pr.ErrorMsg = AppendError(pr.ErrorMsg, "ประเภทไม่ถูกต้อง (ต้องเป็น รับ/Income หรือ จ่าย/Expense)")
                            End If

                            If categoryCol <> "" Then pr.CategoryName = SafeCellText(row, categoryCol).Trim()
                            If fundCol <> "" Then pr.FundName = SafeCellText(row, fundCol).Trim()
                            If bankCol <> "" Then pr.BankName = SafeCellText(row, bankCol).Trim()
                            If noteCol <> "" Then pr.Note = SafeCellText(row, noteCol).Trim()
                            If guidCol <> "" Then pr.TransactionGUID = SafeCellText(row, guidCol).Trim()
                            pr.MachineID = analysis.MachineID
                            rows.Add(pr)
                        Next

                        analysis.Rows = rows
                        analysis.SchemaError = ""
                        Return analysis
                    Next

                    analysis.SchemaError = "ไม่พบชีตที่มีคอลัมน์ครบ (วันที่, รายการ, จำนวนเงิน, ประเภท)"
                    Return analysis
                End Using
            Catch ex As Exception
                analysis.SchemaError = "อ่านไฟล์ไม่สำเร็จ: " & ex.Message
                Return analysis
            End Try
        End Function

        Private Shared Function AppendError(current As String, add As String) As String
            If current.Length = 0 Then Return add
            Return current & " | " & add
        End Function

        Private Shared Function ResolveTranType(text As String) As String
            Dim t = text.Trim().ToLowerInvariant()
            If t.Length = 0 Then Return ""
            If t.Contains("income") OrElse t.Contains("รับ") OrElse t = "i" OrElse t = "1" Then Return "Income"
            If t.Contains("expense") OrElse t.Contains("จ่าย") OrElse t = "e" OrElse t = "0" Then Return "Expense"
            Return ""
        End Function

        Private Function GetWorksheetNames(excelConn As OleDbConnection) As List(Of String)
            Dim names As New List(Of String)()
            Dim schema = excelConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
            If schema Is Nothing Then Return names
            For Each row As DataRow In schema.Rows
                Dim tableName = Convert.ToString(row("TABLE_NAME"))
                If tableName.EndsWith("$") OrElse tableName.EndsWith("$'") Then
                    names.Add(tableName.Trim("'"c))
                End If
            Next
            Return names
        End Function

        Private Function LoadWorksheet(excelConn As OleDbConnection, sheetName As String) As DataTable
            Using cmd = excelConn.CreateCommand()
                cmd.CommandText = "SELECT * FROM [" & sheetName & "]"
                Using da As New OleDbDataAdapter(cmd)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Function

        Private Function FindColumn(sourceTable As DataTable, aliases As IEnumerable(Of String)) As String
            Dim normalized = New HashSet(Of String)(aliases.Select(Function(a) NormalizeColumnKey(a)), StringComparer.OrdinalIgnoreCase)
            For Each col As DataColumn In sourceTable.Columns
                If normalized.Contains(NormalizeColumnKey(col.ColumnName)) Then
                    Return col.ColumnName
                End If
            Next
            Return ""
        End Function

        Private Shared Function NormalizeColumnKey(value As String) As String
            If String.IsNullOrWhiteSpace(value) Then Return ""
            Return New String(value.Trim().ToLowerInvariant().Where(Function(ch) Char.IsLetterOrDigit(ch)).ToArray())
        End Function

        Private Shared Function SafeCellText(row As DataRow, columnName As String) As String
            If String.IsNullOrWhiteSpace(columnName) Then Return ""
            If row Is Nothing OrElse row.Table Is Nothing OrElse Not row.Table.Columns.Contains(columnName) Then Return ""
            If row.IsNull(columnName) Then Return ""
            Return Convert.ToString(row(columnName)).Replace(ChrW(160), " ").Trim()
        End Function

        Private Shared Function IsBlankImportRow(row As DataRow, columnNames As IEnumerable(Of String)) As Boolean
            For Each columnName In columnNames
                If String.IsNullOrWhiteSpace(columnName) Then Continue For
                If row.Table.Columns.Contains(columnName) AndAlso SafeCellText(row, columnName).Length > 0 Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Shared Function TryParseImportDate(value As Object, ByRef parsedDate As Date) As Boolean
            parsedDate = Date.MinValue
            If value Is Nothing OrElse value Is DBNull.Value Then Return False
            If TypeOf value Is Date Then
                parsedDate = Db.NormalizeGregorianDate(CDate(value))
                Return True
            End If
            If TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Decimal OrElse TypeOf value Is Integer OrElse TypeOf value Is Long Then
                parsedDate = Db.NormalizeGregorianDate(DateTime.FromOADate(Convert.ToDouble(value)))
                Return True
            End If
            Dim text = Convert.ToString(value).Replace(ChrW(160), " ").Trim()
            If String.IsNullOrWhiteSpace(text) Then Return False

            Dim oaValue As Double
            If Double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, oaValue) AndAlso oaValue > 20000 Then
                parsedDate = Db.NormalizeGregorianDate(DateTime.FromOADate(oaValue))
                Return True
            End If

            Dim cultures = {CultureInfo.CurrentCulture, New CultureInfo("th-TH"), CultureInfo.InvariantCulture, New CultureInfo("en-US")}
            For Each culture In cultures
                Dim tempDate As DateTime
                If DateTime.TryParse(text, culture, DateTimeStyles.AllowWhiteSpaces, tempDate) Then
                    parsedDate = Db.NormalizeGregorianDate(tempDate)
                    Return True
                End If
            Next
            Return False
        End Function

        Private Shared Function TryParseImportAmount(value As Object, ByRef amount As Decimal) As Boolean
            amount = 0D
            If value Is Nothing OrElse value Is DBNull.Value Then Return False
            If TypeOf value Is Decimal OrElse TypeOf value Is Double OrElse TypeOf value Is Single OrElse TypeOf value Is Integer OrElse TypeOf value Is Long Then
                amount = Convert.ToDecimal(value)
                Return True
            End If
            Dim text = Convert.ToString(value).Replace(ChrW(160), " ").Replace(",", "").Replace("บาท", "").Trim()
            If String.IsNullOrWhiteSpace(text) Then Return False
            If Decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, amount) Then Return True
            If Decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, amount) Then Return True
            Return False
        End Function

        ' ====================== 3. Preview + 4. Smart Deduplication ======================

        ''' <summary>ตรวจสอบซ้ำกับฐานข้อมูลหลัก: TransactionGUID (ซ้ำแน่นอน) -> composite key (สงสัยซ้ำ)</summary>
        Private Sub AnalyzeAllDuplicates()
            _dbGuidSet.Clear()
            _dbCompositeSet.Clear()
            Try
                Using conn = Db.OpenConn()
                    Dim dtg = Db.GetTable(conn, "SELECT TransactionGUID FROM Transactions WHERE TransactionGUID IS NOT NULL")
                    For Each r As DataRow In dtg.Rows
                        Dim g = Convert.ToString(r(0)).Trim()
                        If g.Length > 0 Then _dbGuidSet.Add(g)
                    Next

                    Dim dtc = Db.GetTable(conn,
                        "SELECT DateValue(TranDate) AS D, TranType AS T, Amount AS A, IIF([Detail] IS NULL,'',[Detail]) AS De, IIF(SourceMachineID IS NULL,'',SourceMachineID) AS M FROM Transactions")
                    For Each r As DataRow In dtc.Rows
                        _dbCompositeSet.Add(BuildCompositeKey(r("D"), r("T"), r("A"), r("De"), r("M")))
                    Next
                End Using
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmMultiMachineImport.AnalyzeDuplicates.LoadDb")
            End Try

            Dim seenGuids As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each f In _files
                f.GuidDuplicateCount = 0
                f.SuspectedDuplicateCount = 0
                f.DateWarningCount = 0
                f.RowErrorCount = 0
                For Each row In f.Rows
                    row.IsDuplicateExact = False
                    row.IsSuspectedDuplicate = False
                    If row.ErrorMsg.Length > 0 Then
                        f.RowErrorCount += 1
                        Continue For
                    End If
                    If row.DateWarning.Length > 0 Then f.DateWarningCount += 1

                    ' ลำดับ 1: TransactionGUID (แม่นยำที่สุด) — เทียบกับ DB + ภายในชุดไฟล์ที่เลือก
                    If row.TransactionGUID.Length > 0 Then
                        If _dbGuidSet.Contains(row.TransactionGUID) OrElse seenGuids.Contains(row.TransactionGUID) Then
                            row.IsDuplicateExact = True
                        Else
                            seenGuids.Add(row.TransactionGUID)
                        End If
                    End If

                    ' ลำดับ 2: composite key (TranDate + Amount + Type + Description + MachineID) — สงสัยซ้ำ
                    If Not row.IsDuplicateExact Then
                        Dim ck = BuildCompositeKey(row.TranDate.Date, row.TranType, row.Amount, row.Detail, row.MachineID)
                        If _dbCompositeSet.Contains(ck) Then
                            row.IsSuspectedDuplicate = True
                        End If
                    End If

                    If row.IsDuplicateExact Then f.GuidDuplicateCount += 1
                    If row.IsSuspectedDuplicate Then f.SuspectedDuplicateCount += 1
                Next
            Next
        End Sub

        Private Shared Function BuildCompositeKey(d As Object, t As Object, a As Object, de As Object, m As Object) As String
            Dim datePart As String = ""
            Try
                Dim dt = If(TypeOf d Is Date, CDate(d), DateTime.FromOADate(Convert.ToDouble(d)))
                datePart = dt.ToString("yyyy-MM-dd")
            Catch
            End Try
            Dim amt As Decimal = Db.ToDecimalOrZero(a)
            Return datePart & "|" & Convert.ToString(t).Trim() & "|" & amt.ToString("0.00", CultureInfo.InvariantCulture) &
                   "|" & Convert.ToString(de).Trim() & "|" & Convert.ToString(m).Trim()
        End Function

        Private Sub RefreshPreviewGrid()
            ' คำนวณยอดรวมต่อไฟล์ + ตรวจสอบซ้ำ (ข้อมูลฐานข้อมูลหลักอาจเปลี่ยนไป)
            For Each f In _files
                Dim inc As Decimal = 0D
                Dim exp As Decimal = 0D
                For Each r In f.Rows
                    If r.ErrorMsg.Length > 0 Then Continue For
                    If r.TranType = "Income" Then
                        inc += r.Amount
                    ElseIf r.TranType = "Expense" Then
                        exp += r.Amount
                    End If
                Next
                f.IncomeSum = inc
                f.ExpenseSum = exp
            Next
            AnalyzeAllDuplicates()

            dgvImportPreview.Rows.Clear()
            Dim totalRows As Integer = 0
            Dim filesWithData As Integer = 0
            Dim fallbackCount As Integer = 0
            For Each f In _files
                Dim machineDisplay As String
                If f.MachineIDSource = "ฝังในไฟล์" Then
                    machineDisplay = f.MachineID
                ElseIf f.MachineIDSource = "ใช้ชื่อไฟล์แทน (ความน่าเชื่อถือต่ำ)" Then
                    machineDisplay = f.MachineID & " (ใช้ชื่อไฟล์แทน)"
                    fallbackCount += 1
                Else
                    machineDisplay = "ไม่ทราบ/Unknown"
                End If

                Dim hashStatus As String = If(f.PreviousImportInfo.Length > 0, "เคย Import แล้ว " & f.PreviousImportInfo, "ใหม่")
                Dim dupStatus As String = "Ready"
                If f.GuidDuplicateCount > 0 OrElse f.SuspectedDuplicateCount > 0 Then
                    dupStatus = "พบซ้ำ " & f.GuidDuplicateCount.ToString("#,##0") & " (GUID) / สงสัยซ้ำ " & f.SuspectedDuplicateCount.ToString("#,##0")
                End If

                Dim warnings As String = ""
                If f.SchemaError.Length > 0 Then
                    warnings = "❌ " & f.SchemaError
                Else
                    If f.DateWarningCount > 0 Then warnings &= "⚠️ วันที่ผิดปกติ " & f.DateWarningCount.ToString("#,##0") & " รายการ"
                    If f.RowErrorCount > 0 Then
                        If warnings.Length > 0 Then warnings &= " | "
                        warnings &= "แถวผิดพลาด " & f.RowErrorCount.ToString("#,##0")
                    End If
                End If

                Dim idx = dgvImportPreview.Rows.Add(
                    Path.GetFileName(f.FilePath),
                    machineDisplay,
                    hashStatus,
                    f.Rows.Count.ToString("#,##0"),
                    f.IncomeSum.ToString("#,##0.00"),
                    f.ExpenseSum.ToString("#,##0.00"),
                    dupStatus,
                    warnings)
                dgvImportPreview.Rows(idx).Tag = f

                If f.SchemaError.Length = 0 Then
                    totalRows += f.Rows.Count
                    filesWithData += 1
                End If
            Next

            lblFileSummary.Text = "เลือกแล้ว " & _files.Count & " ไฟล์ | รวมแถวข้อมูล " & totalRows.ToString("#,##0") & " แถว"
            btnConfirmMerge.Enabled = (_files.Count > 0 AndAlso filesWithData > 0)

            If fallbackCount > 0 Then
                lblMachineStatus.Text = "⚠️ " & fallbackCount & " ไฟล์ ไม่พบ MachineID ในไฟล์ — ใช้ชื่อไฟล์แทน (ความน่าเชื่อถือต่ำ)"
                lblMachineStatus.ForeColor = Color.FromArgb(153, 27, 27)
            ElseIf _files.Count = 0 Then
                lblMachineStatus.Text = "⏳ ยังไม่มีการเลือกไฟล์"
                lblMachineStatus.ForeColor = Color.FromArgb(153, 27, 27)
            Else
                lblMachineStatus.Text = "✅ อ่านรหัสเครื่องจากไฟล์เรียบร้อย"
                lblMachineStatus.ForeColor = Color.FromArgb(22, 101, 52)
            End If
        End Sub

        ''' <summary>แสดงรายละเอียดรายการซ้ำ/สงสัยซ้ำ (ปุ่ม + double-click) — ผู้ใช้เห็นก่อนตัดสินใจเสมอ</summary>
        Private Sub ShowDuplicateDetailDialog(Optional filterFile As FileAnalysis = Nothing)
            Dim rows As New List(Of ParsedRow)()
            For Each f In _files
                If filterFile IsNot Nothing AndAlso Not ReferenceEquals(f, filterFile) Then Continue For
                For Each r In f.Rows
                    If r.IsDuplicateExact OrElse r.IsSuspectedDuplicate Then rows.Add(r)
                Next
            Next
            If rows.Count = 0 Then
                MessageBox.Show("ไม่พบรายการที่ตรวจพบว่าซ้ำ / สงสัยซ้ำ ในข้อมูลที่เลือก", "รายการซ้ำ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim dt As New DataTable()
            dt.Columns.Add("ไฟล์", GetType(String))
            dt.Columns.Add("แถวที่", GetType(Integer))
            dt.Columns.Add("วันที่", GetType(String))
            dt.Columns.Add("ประเภท", GetType(String))
            dt.Columns.Add("รายละเอียด", GetType(String))
            dt.Columns.Add("จำนวนเงิน", GetType(Decimal))
            dt.Columns.Add("MachineID", GetType(String))
            dt.Columns.Add("สถานะ", GetType(String))

            For Each r In rows
                Dim fileName As String = ""
                For Each f In _files
                    If f.Rows.Contains(r) Then
                        fileName = Path.GetFileName(f.FilePath)
                        Exit For
                    End If
                Next
                Dim status As String = If(r.IsDuplicateExact, "ซ้ำแน่นอน (TransactionGUID ตรง)", "สงสัยซ้ำ (วันที่+จำนวน+ประเภท+รายละเอียด+เครื่อง ตรงกับฐานข้อมูล)")
                Dim dateText As String = If(r.TranDate = Date.MinValue, "-", r.TranDate.ToString("dd/MM/yyyy"))
                dt.Rows.Add(fileName, r.RowNumber, dateText, r.TranType, r.Detail, r.Amount,
                            If(r.MachineID.Length > 0, r.MachineID, "-"), status)
            Next

            Dim dlg As New Form()
            dlg.Text = "รายการที่ตรวจพบว่าซ้ำ / สงสัยซ้ำ (" & rows.Count.ToString("#,##0") & " รายการ)"
            dlg.StartPosition = FormStartPosition.CenterParent
            dlg.Size = New Size(1080, 560)
            dlg.MinimumSize = New Size(780, 420)
            dlg.Font = New Font("Tahoma", 10.5F)
            dlg.BackColor = Color.FromArgb(255, 253, 244)
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog
            dlg.MaximizeBox = False
            dlg.MinimizeBox = False

            Dim dgv As New DataGridView()
            dgv.Dock = DockStyle.Fill
            dgv.AllowUserToAddRows = False
            dgv.AllowUserToDeleteRows = False
            dgv.ReadOnly = True
            dgv.RowHeadersVisible = False
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgv.BackgroundColor = Color.FromArgb(255, 253, 244)
            dgv.DataSource = dt

            Dim lblNote As New Label()
            lblNote.Dock = DockStyle.Bottom
            lblNote.Height = 46
            lblNote.Font = New Font("Tahoma", 9.5F)
            lblNote.ForeColor = Color.FromArgb(153, 27, 27)
            lblNote.Padding = New Padding(10, 6, 10, 0)
            lblNote.Text = "⚠️ รายการ " & ChrW(34) & "สงสัยซ้ำ" & ChrW(34) & " ตรวจจาก composite key (วันที่+จำนวนเงิน+ประเภท+รายละเอียด+รหัสเครื่อง) เทียบกับฐานข้อมูลหลัก — " &
                "อาจเป็น false positive ได้ (ธุรกรรมที่เหมือนกันโดยบังเอิญ) จึงไม่ถูกข้ามอัตโนมัติ ต้องยืนยันอีกครั้งตอนกดรวมข้อมูล"

            Dim btnOk As New Button()
            btnOk.Dock = DockStyle.Bottom
            btnOk.Height = 46
            btnOk.Text = "ปิดหน้าต่าง"
            btnOk.BackColor = Color.FromArgb(75, 85, 99)
            btnOk.ForeColor = Color.White
            btnOk.FlatStyle = FlatStyle.Flat
            btnOk.Cursor = Cursors.Hand
            btnOk.Font = New Font("Tahoma", 10F, FontStyle.Bold)
            AddHandler btnOk.Click, Sub(s2, e2) dlg.Close()

            dlg.Controls.Add(dgv)
            dlg.Controls.Add(lblNote)
            dlg.Controls.Add(btnOk)
            dlg.ShowDialog(Me)
            dgv.Dispose()
            dlg.Dispose()
        End Sub

        ' ====================== 5. Merge Execution & Transaction Handling ======================

        Private Sub RunMerge(skipSuspected As Boolean)
            Log("=== เริ่มรวมข้อมูล (Merge) ===")

            ' ก่อน merge: สำรองฐานข้อมูลอัตโนมัติ (เซฟตี้เน็ตสุดท้าย — transaction ป้องกัน insert ผิดพลาด แต่ไม่กันไฟล์ corrupt)
            Log("📦 สำรองฐานข้อมูลอัตโนมัติก่อนรวมข้อมูล...")
            Db.BackupDatabase()
            Log("✅ สำรองฐานข้อมูลสำเร็จ")

            Dim mergedTotal As Integer = 0
            Dim mergedAmount As Decimal = 0D
            Dim batchInfos As New List(Of String)()

            Using conn = Db.OpenConn()
                ' โหลด map ชื่อ -> ID (Categories/Funds/BankAccounts) สำหรับ resolve
                Dim categoryMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                Dim dtCat = Db.GetTable(conn, "SELECT ID, CategoryName, TranType FROM Categories")
                For Each r As DataRow In dtCat.Rows
                    Dim k = Convert.ToString(r("TranType")).Trim() & "|" & Convert.ToString(r("CategoryName")).Trim()
                    If Not categoryMap.ContainsKey(k) Then categoryMap(k) = Convert.ToInt32(r("ID"))
                Next

                Dim fundMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                Dim dtFund = Db.GetTable(conn, "SELECT ID, FundName FROM Funds")
                For Each r As DataRow In dtFund.Rows
                    Dim k = Convert.ToString(r("FundName")).Trim()
                    If Not fundMap.ContainsKey(k) Then fundMap(k) = Convert.ToInt32(r("ID"))
                Next

                Dim bankMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                Dim dtBank = Db.GetTable(conn, "SELECT ID, BankName FROM BankAccounts")
                For Each r As DataRow In dtBank.Rows
                    Dim k = Convert.ToString(r("BankName")).Trim()
                    If Not bankMap.ContainsKey(k) Then bankMap(k) = Convert.ToInt32(r("ID"))
                Next

                Dim tx As OleDbTransaction = conn.BeginTransaction()
                Try
                    Dim fileIndex As Integer = 0
                    For Each f In _files
                        fileIndex += 1
                        If f.SchemaError.Length > 0 Then
                            Log("⚠️ ข้ามไฟล์ " & Path.GetFileName(f.FilePath) & " (schema ไม่ถูกต้อง: " & f.SchemaError & ")")
                            Continue For
                        End If

                        ' แต่ละไฟล์ = 1 ImportBatch (สืบย้อนได้ว่าแถวไหนมาจากไฟล์/เครื่องไหน)
                        Dim batchId As Guid = Guid.NewGuid()
                        Dim fileImported As Integer = 0
                        Dim fileSkipped As Integer = 0
                        Dim fileAmount As Decimal = 0D
                        Dim fileTotal As Integer = 0

                        For Each row In f.Rows
                            If row.ErrorMsg.Length > 0 Then
                                fileSkipped += 1
                                Continue For
                            End If
                            fileTotal += 1

                            ' ข้ามซ้ำแน่นอน (GUID) เสมอ; ข้ามสงสัยซ้ำเฉพาะเมื่อผู้ใช้เลือก
                            If row.IsDuplicateExact Then
                                fileSkipped += 1
                                Continue For
                            End If
                            If row.IsSuspectedDuplicate AndAlso skipSuspected Then
                                fileSkipped += 1
                                Continue For
                            End If

                            Dim catId As Object = ResolveCategoryId(conn, tx, row.TranType, row.CategoryName, categoryMap)
                            Dim fundId As Object = ResolveFundId(conn, tx, row.FundName, fundMap)
                            Dim bankId As Object = ResolveBankId(conn, tx, row.BankName, bankMap)

                            ExecInTx(conn, tx,
                                "INSERT INTO Transactions (TranDate, TranType, CategoryID, FundID, BankID, [Detail], Amount, [Note], TransactionGUID, SourceMachineID, ImportBatchID) VALUES (" &
                                Db.AccessDateLiteral(Db.NormalizeGregorianDate(row.TranDate.Date)) & ", @t, @c, @f, @b, @d, @a, @n, @g, @m, @ib)",
                                New Tuple(Of String, Object)("@t", row.TranType),
                                New Tuple(Of String, Object)("@c", catId),
                                New Tuple(Of String, Object)("@f", fundId),
                                New Tuple(Of String, Object)("@b", bankId),
                                New Tuple(Of String, Object)("@d", row.Detail),
                                New Tuple(Of String, Object)("@a", row.Amount),
                                New Tuple(Of String, Object)("@n", If(row.Note.Length > 0, row.Note, Nothing)),
                                New Tuple(Of String, Object)("@g", If(row.TransactionGUID.Length > 0, row.TransactionGUID, Nothing)),
                                New Tuple(Of String, Object)("@m", If(row.MachineID.Length > 0, row.MachineID, Nothing)),
                                New Tuple(Of String, Object)("@ib", batchId))
                            fileImported += 1
                            fileAmount += row.Amount
                        Next

                        ' บันทึก ImportHistory ภายใน transaction เดียวกัน — commit ต่อเมื่อทุกไฟล์สำเร็จ
                        ExecInTx(conn, tx,
                            "INSERT INTO ImportHistory (ImportBatchID, SourceFileName, FileHash, MachineID, ImportDateTime, ImportedBy, RowCountTotal, RowCountImported, RowCountSkippedDuplicate, TotalAmountImported, ImportStatus) VALUES (@ib, @fn, @h, @m, @dt, @by, @tot, @imp, @skp, @amt, 'Success')",
                            New Tuple(Of String, Object)("@ib", batchId),
                            New Tuple(Of String, Object)("@fn", Path.GetFileName(f.FilePath)),
                            New Tuple(Of String, Object)("@h", f.FileHash),
                            New Tuple(Of String, Object)("@m", If(f.MachineID.Length > 0, f.MachineID, Nothing)),
                            New Tuple(Of String, Object)("@dt", DateTime.Now),
                            New Tuple(Of String, Object)("@by", Environment.UserName),
                            New Tuple(Of String, Object)("@tot", fileTotal),
                            New Tuple(Of String, Object)("@imp", fileImported),
                            New Tuple(Of String, Object)("@skp", fileSkipped),
                            New Tuple(Of String, Object)("@amt", fileAmount))

                        mergedTotal += fileImported
                        mergedAmount += fileAmount
                        batchInfos.Add("  • " & Path.GetFileName(f.FilePath) & " → เครื่อง " & If(f.MachineID.Length > 0, f.MachineID, "ไม่ทราบ") &
                                       ": นำเข้า " & fileImported.ToString("#,##0") & " รายการ, ข้าม " & fileSkipped.ToString("#,##0") &
                                       ", ยอด " & fileAmount.ToString("#,##0.00") & " บาท")
                        Log("✔️ " & Path.GetFileName(f.FilePath) & ": นำเข้า " & fileImported & " / ข้าม " & fileSkipped & " / ยอด " & fileAmount.ToString("#,##0.00"))
                        SetProgress(fileIndex, _files.Count, "รวมไฟล์ " & fileIndex & "/" & _files.Count)
                    Next

                    tx.Commit()
                    Log("✅ Commit สำเร็จ (เขียนลง ImportHistory เฉพาะเมื่อสำเร็จเท่านั้น)")
                Catch ex As Exception
                    Try
                        tx.Rollback()
                        Log("↩️ Rollback ทั้งหมด เนื่องจาก error: " & ex.Message)
                    Catch
                    End Try
                    Throw
                End Try
            End Using

            Dim sb As New StringBuilder()
            sb.AppendLine("🎉 รวมข้อมูลสำเร็จ!")
            sb.AppendLine("นำเข้าจริง: " & mergedTotal.ToString("#,##0") & " รายการ")
            sb.AppendLine("ยอดรวมเงินที่เพิ่ม: " & mergedAmount.ToString("#,##0.00") & " บาท")
            sb.AppendLine("รายละเอียดต่อไฟล์:")
            sb.AppendLine(String.Join(vbCrLf, batchInfos))
            MessageBox.Show(sb.ToString(), "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
            lblMergeStatus.Text = "✅ ล่าสุด: นำเข้า " & mergedTotal.ToString("#,##0") & " รายการ / ยอด " & mergedAmount.ToString("#,##0.00") & " บาท"

            RefreshPreviewGrid()
            Log("=== จบ Merge ===")
        End Sub

        ' ---------- Helpers สำหรับ OLEDB Transaction ----------

        Private Shared Sub AddParams(cmd As OleDbCommand, params As Tuple(Of String, Object)())
            If params Is Nothing Then Return
            For Each p In params
                If p Is Nothing Then Continue For
                If TypeOf p.Item2 Is Guid Then
                    ' สร้าง parameter แบบระบุ OleDbType.Guid ชัดเจน เพื่อกัน Data type mismatch กับคอลัมน์ GUID ใน Access
                    Dim gp = cmd.CreateParameter()
                    gp.ParameterName = p.Item1
                    gp.OleDbType = OleDbType.Guid
                    gp.Value = p.Item2
                    cmd.Parameters.Add(gp)
                Else
                    cmd.Parameters.AddWithValue(p.Item1, If(p.Item2 Is Nothing, DBNull.Value, p.Item2))
                End If
            Next
        End Sub

        Private Function ExecInTx(conn As OleDbConnection, tx As OleDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer
            Using cmd = conn.CreateCommand()
                cmd.Transaction = tx
                cmd.CommandText = sql
                AddParams(cmd, params)
                Return cmd.ExecuteNonQuery()
            End Using
        End Function

        Private Function ScalarInTx(conn As OleDbConnection, tx As OleDbTransaction, sql As String, ParamArray params As Tuple(Of String, Object)()) As Object
            Using cmd = conn.CreateCommand()
                cmd.Transaction = tx
                cmd.CommandText = sql
                AddParams(cmd, params)
                Return cmd.ExecuteScalar()
            End Using
        End Function

        Private Function ExecNoTx(conn As OleDbConnection, sql As String, ParamArray params As Tuple(Of String, Object)()) As Integer
            Using cmd = conn.CreateCommand()
                cmd.CommandText = sql
                AddParams(cmd, params)
                Return cmd.ExecuteNonQuery()
            End Using
        End Function

        ' ---------- Resolve ID (Categories / Funds / BankAccounts) ----------

        Private Function ResolveCategoryId(conn As OleDbConnection, tx As OleDbTransaction, tranType As String, name As String, cache As Dictionary(Of String, Integer)) As Object
            Dim n = name.Trim()
            If n.Length = 0 Then Return DBNull.Value
            Dim key = tranType.Trim() & "|" & n
            If cache.ContainsKey(key) Then Return cache(key)
            Dim exist = ScalarInTx(conn, tx, "SELECT ID FROM Categories WHERE TranType=@t AND CategoryName=@n",
                                   New Tuple(Of String, Object)("@t", tranType),
                                   New Tuple(Of String, Object)("@n", n))
            If exist IsNot Nothing Then
                Dim id = Convert.ToInt32(exist)
                cache(key) = id
                Return id
            End If
            ExecInTx(conn, tx, "INSERT INTO Categories (CategoryName, TranType) VALUES (@n, @t)",
                     New Tuple(Of String, Object)("@n", n),
                     New Tuple(Of String, Object)("@t", tranType))
            Dim newId = Convert.ToInt32(ScalarInTx(conn, tx, "SELECT @@IDENTITY"))
            cache(key) = newId
            Return newId
        End Function

        Private Function ResolveFundId(conn As OleDbConnection, tx As OleDbTransaction, name As String, cache As Dictionary(Of String, Integer)) As Object
            Dim n = name.Trim()
            If n.Length = 0 Then Return DBNull.Value
            If cache.ContainsKey(n) Then Return cache(n)
            Dim exist = ScalarInTx(conn, tx, "SELECT ID FROM Funds WHERE FundName=@n",
                                   New Tuple(Of String, Object)("@n", n))
            If exist IsNot Nothing Then
                Dim id = Convert.ToInt32(exist)
                cache(n) = id
                Return id
            End If
            ExecInTx(conn, tx, "INSERT INTO Funds (FundName) VALUES (@n)",
                     New Tuple(Of String, Object)("@n", n))
            Dim newId = Convert.ToInt32(ScalarInTx(conn, tx, "SELECT @@IDENTITY"))
            cache(n) = newId
            Return newId
        End Function

        Private Function ResolveBankId(conn As OleDbConnection, tx As OleDbTransaction, name As String, cache As Dictionary(Of String, Integer)) As Object
            Dim n = name.Trim()
            If n.Length = 0 Then Return DBNull.Value
            If cache.ContainsKey(n) Then Return cache(n)
            Dim exist = ScalarInTx(conn, tx, "SELECT ID FROM BankAccounts WHERE BankName=@n",
                                   New Tuple(Of String, Object)("@n", n))
            If exist IsNot Nothing Then
                Dim id = Convert.ToInt32(exist)
                cache(n) = id
                Return id
            End If
            ExecInTx(conn, tx, "INSERT INTO BankAccounts (BankName) VALUES (@n)",
                     New Tuple(Of String, Object)("@n", n))
            Dim newId = Convert.ToInt32(ScalarInTx(conn, tx, "SELECT @@IDENTITY"))
            cache(n) = newId
            Return newId
        End Function

        ' ====================== 2.3 ImportHistory — schema + log ======================

        ''' <summary>สร้างตาราง ImportHistory และคอลัมน์ใหม่ใน Transactions แบบ non-breaking (รันอัตโนมัติตอนเปิดฟอร์ม)</summary>
        Private Sub EnsureImportSchema()
            Try
                Using conn = Db.OpenConn()
                    Dim hasImportHistory As Boolean = False
                    Dim tables = conn.GetSchema("Tables")
                    For Each r As DataRow In tables.Rows
                        If Convert.ToString(r("TABLE_NAME")).Equals("ImportHistory", StringComparison.OrdinalIgnoreCase) Then
                            hasImportHistory = True
                            Exit For
                        End If
                    Next
                    If Not hasImportHistory Then
                        Db.ExecuteNonQuery(conn,
                            "CREATE TABLE ImportHistory (ImportBatchID GUID PRIMARY KEY, SourceFileName TEXT(255), FileHash TEXT(64) NOT NULL, MachineID TEXT(50), ImportDateTime DATETIME, ImportedBy TEXT(100), RowCountTotal INTEGER, RowCountImported INTEGER, RowCountSkippedDuplicate INTEGER, TotalAmountImported CURRENCY, ImportStatus TEXT(20))")
                        Log("✅ สร้างตาราง ImportHistory ใหม่แล้ว")
                    End If

                    Dim colNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                    Dim cols = conn.GetSchema("Columns", New String() {Nothing, Nothing, "Transactions", Nothing})
                    For Each r As DataRow In cols.Rows
                        colNames.Add(Convert.ToString(r("COLUMN_NAME")))
                    Next
                    If Not colNames.Contains("ImportBatchID") Then
                        Db.ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN ImportBatchID GUID")
                        Log("✅ เพิ่มคอลัมน์ ImportBatchID ใน Transactions แล้ว (non-breaking)")
                    End If
                    If Not colNames.Contains("TransactionGUID") Then
                        Db.ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN TransactionGUID TEXT(36)")
                        Log("✅ เพิ่มคอลัมน์ TransactionGUID ใน Transactions แล้ว (non-breaking)")
                    End If
                    If Not colNames.Contains("SourceMachineID") Then
                        Db.ExecuteNonQuery(conn, "ALTER TABLE Transactions ADD COLUMN SourceMachineID TEXT(50)")
                        Log("✅ เพิ่มคอลัมน์ SourceMachineID ใน Transactions แล้ว (non-breaking)")
                    End If
                End Using
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmMultiMachineImport.EnsureImportSchema")
                MessageBox.Show("ไม่สามารถตรวจสอบ/สร้างโครงสร้างตาราง ImportHistory ได้: " & ex.Message,
                                "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub

        ''' <summary>บันทึกผลลัพธ์ลง ImportHistory ในกรณี Failed / Cancelled (บันทึกเฉพาะตอน commit สำเร็จเท่านั้น จึง log นอก transaction)</summary>
        Private Sub LogMergeOutcome(status As String)
            Try
                Using conn = Db.OpenConn()
                    For Each f In _files
                        If f.SchemaError.Length > 0 Then Continue For
                        ExecNoTx(conn,
                            "INSERT INTO ImportHistory (ImportBatchID, SourceFileName, FileHash, MachineID, ImportDateTime, ImportedBy, RowCountTotal, RowCountImported, RowCountSkippedDuplicate, TotalAmountImported, ImportStatus) VALUES (@ib, @fn, @h, @m, @dt, @by, @tot, 0, 0, 0, @st)",
                            New Tuple(Of String, Object)("@ib", Guid.NewGuid()),
                            New Tuple(Of String, Object)("@fn", Path.GetFileName(f.FilePath)),
                            New Tuple(Of String, Object)("@h", f.FileHash),
                            New Tuple(Of String, Object)("@m", If(f.MachineID.Length > 0, f.MachineID, Nothing)),
                            New Tuple(Of String, Object)("@dt", DateTime.Now),
                            New Tuple(Of String, Object)("@by", Environment.UserName),
                            New Tuple(Of String, Object)("@tot", f.Rows.Count),
                            New Tuple(Of String, Object)("@st", status))
                    Next
                End Using
                Log("📝 บันทึกสถานะ '" & status & "' ลง ImportHistory แล้ว (" & _files.Count & " ไฟล์)")
            Catch ex As Exception
                AppPaths.LogCrash(ex, "FrmMultiMachineImport.LogMergeOutcome")
            End Try
        End Sub

        ' ====================== Log / Progress ======================

        Private Sub Log(msg As String)
            If rtbLog.InvokeRequired Then
                rtbLog.BeginInvoke(Sub() Log(msg))
                Return
            End If
            rtbLog.AppendText("[" & DateTime.Now.ToString("HH:mm:ss") & "] " & msg & Environment.NewLine)
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
    End Class
End Namespace
