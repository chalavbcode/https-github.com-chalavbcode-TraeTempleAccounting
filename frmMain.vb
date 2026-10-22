Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.IO
Imports WinTimer = System.Windows.Forms.Timer

Namespace TempleAccounting
    Partial Public Class frmMain
        Inherits Form

        Private _currentActiveButton As Button
        Private _currentChildForm As Form
        Private _compactOverviewMode As Boolean = False
        Private picHomeLogo As PictureBox
        Private Shared _appIcon As Icon

        ''' <summary>ไอคอนแอปพลิเคชัน (lamYangI&E.ico) ที่ใช้ร่วมกันทั้งหน้าหลักและฟอร์มลูก/ป็อปอัป</summary>
        Public Shared ReadOnly Property AppIcon As Icon
            Get
                If _appIcon Is Nothing Then
                    Try
                        _appIcon = New Icon(Path.Combine(Application.StartupPath, "lamYangI&E.ico"))
                    Catch
                        _appIcon = SystemIcons.Application
                    End Try
                End If
                Return _appIcon
            End Get
        End Property

        Public Sub New()
            Try
                AppPaths.EnsureDirectoriesExist()
                Dim ignore = AppPaths.DatabaseFile
            Catch ex As Exception
                AppPaths.LogCrash(ex, "AppPaths init")
            End Try

            Try
                InitializeComponent()
                Me.Icon = AppIcon
                Me.ShowIcon = True
                Me.ShowInTaskbar = True
                SetupIconsAndImages()
                SetupEventHandlers()
                SetupCardHoverEffects()
                ApplyInitialState()
            Catch ex As Exception
                AppPaths.LogCrash(ex, "frmMain New() UI init")
                MessageBox.Show("⛔ ไม่สามารถสร้างหน้าต่างหลักได้: " & vbCrLf & ex.Message & vbCrLf & vbCrLf &
                                "📄 ดูรายละเอียดที่: " & Path.Combine(AppPaths.LogsFolder, "crash.log"),
                                "ระบบบัญชีวัด - เปิดโปรแกรมไม่สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Throw
            End Try
        End Sub

        Private Sub FrmMain_Load(sender As Object, e As EventArgs)
            Try
                Me.Icon = AppIcon
                Me.KeyPreview = True
                HelpSystem.SetupHelp(Me, "FrmMain")
                Dim tt As New ToolTip()
                tt.SetToolTip(lblStatusCenter, "ดับเบิ้ลคลิก: เปิดโฟลเดอร์ฐานข้อมูล" & vbCrLf & "คลิกขวา: คัดลอกที่อยู่ไฟล์ฐานข้อมูล" & vbCrLf & "ไฟล์: " & AppPaths.DatabaseFile)
                ShowHomeLogo()
                UiFitter.AutoFitFormButtons(Me)
                ' สร้างไอคอนลัด Desktop อัตโนมัติเมื่อเปิดครั้งแรก (ถ้ายังไม่มี)
                DesktopShortcutHelper.CreateDesktopShortcutIfMissing()
            Catch
            End Try
            ShowDashboard()
        End Sub

        Private Sub FrmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.F1 Then
                e.Handled = True
                e.SuppressKeyPress = True
                HelpSystem.ShowManual("FrmMain", Me)
            End If

            If e.Control AndAlso e.Shift AndAlso e.KeyCode = Keys.D Then
                e.Handled = True
                e.SuppressKeyPress = True
                ShowDeveloperCredit()
            End If
        End Sub

        ' ------- Secret Developer Credit Trigger -------
        Private _logoClickCount As Integer = 0
        Private _logoClickTimer As WinTimer

        ''' <summary>ตรวจจับ Triple-Click บนโลโก้วัด (คลิก 3 ครั้งใน 500ms)</summary>
        Private Sub Logo_TripleClickDetect(sender As Object, e As MouseEventArgs)
            If _logoClickTimer Is Nothing Then
                _logoClickTimer = New WinTimer() With {.Interval = 500}
                AddHandler _logoClickTimer.Tick, Sub()
                    _logoClickCount = 0
                    _logoClickTimer.Stop()
                End Sub
            End If

            _logoClickCount += 1
            _logoClickTimer.Stop()
            _logoClickTimer.Start()

            If _logoClickCount >= 3 Then
                _logoClickCount = 0
                _logoClickTimer.Stop()
                ShowDeveloperCredit()
            End If
        End Sub

        ''' <summary>แสดงข้อความสิทธิ์และรายละเอียดผู้พัฒนา (Secret Trigger)</summary>
        Private Sub ShowDeveloperCredit()
            MessageBox.Show(
                "ระบบบัญชีวัดแหลมยาง (Temple Accounting System)" & vbCrLf & vbCrLf &
                "พัฒนาและถือสิทธิ์สร้างสรรค์โดย: พระบุญส่ง โสภณจิตฺโต" & vbCrLf & vbCrLf &
                "สงวนลิขสิทธิ์ © 2026 พระบุญส่ง โสภณจิตฺโต." & vbCrLf & vbCrLf &
                "อนุญาตให้ใช้งานเฉพาะงานกิจการของวัดแหลมยาง" & vbCrLf &
                "ห้ามทำซ้ำ ดัดแปลง หรือนำส่วนหนึ่งส่วนใดไปใช้ในเชิงพาณิชย์โดยไม่ได้รับอนุญาต",
                "ข้อความสิทธิ์และรายละเอียดผู้พัฒนา",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)
        End Sub

        Private Sub StatusCenter_DoubleClick(sender As Object, e As EventArgs)
            Try
                Dim dbPath = AppPaths.DatabaseFile
                Dim dbFolder = Path.GetDirectoryName(dbPath)
                If Directory.Exists(dbFolder) Then
                    System.Diagnostics.Process.Start("explorer.exe", dbFolder)
                End If
            Catch ex As Exception
                MessageBox.Show("ไม่สามารถเปิดโฟลเดอร์ได้: " & ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
        End Sub

        Private Sub StatusCenter_MouseClick(sender As Object, e As MouseEventArgs)
            If e.Button = MouseButtons.Right Then
                Try
                    Clipboard.SetText(AppPaths.DatabaseFile)
                    MessageBox.Show("คัดลอกที่อยู่ไฟล์ฐานข้อมูลแล้ว:" & vbCrLf & AppPaths.DatabaseFile, "คัดลอกสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("ไม่สามารถคัดลอกได้: " & ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Try
            End If
        End Sub

        Private Sub ShowHomeLogo()
            If picHomeLogo Is Nothing Then
                picHomeLogo = New PictureBox()
                picHomeLogo.Image = Image.FromFile(Path.Combine(Application.StartupPath, "Lamyang I&Emain.png"))
                picHomeLogo.SizeMode = PictureBoxSizeMode.Zoom
                picHomeLogo.BackColor = Color.Transparent
            End If
            ' โลโก้เป็นองค์ประกอบของหน้าหลักเท่านั้น: แสดงเฉพาะเมื่ออยู่หน้า Dashboard
            picHomeLogo.Visible = True
            pnlFormHostBody.Controls.Add(picHomeLogo)
            ' ขยายโลโก้ให้เกือบชิดขอบบน-ล่าง และกึ่งกลางแนวนอน พร้อมปรับตามขนาดหน้าต่าง
            AddHandler pnlFormHostBody.Resize, AddressOf PositionHomeLogo
            PositionHomeLogo()
        End Sub

        Private Sub PositionHomeLogo()
            If picHomeLogo Is Nothing OrElse pnlFormHostBody Is Nothing Then Return
            Try
                Const gap As Integer = 5
                Dim hostW As Integer = pnlFormHostBody.ClientSize.Width
                Dim hostH As Integer = pnlFormHostBody.ClientSize.Height

                ' ให้สูงเกือบเต็มความสูง (เว้นช่องบน/ล่าง ~5 px)
                Dim newH As Integer = Math.Max(1, hostH - (gap * 2))

                ' คงอัตราส่วนจริงของรูป (Zoom) โดยขยายตามความสูงที่คำนวณได้
                Dim ratio As Double = 1.0
                If picHomeLogo.Image IsNot Nothing AndAlso picHomeLogo.Image.Height > 0 Then
                    ratio = picHomeLogo.Image.Width / picHomeLogo.Image.Height
                End If

                Dim newW As Integer = CInt(Math.Round(newH * ratio))
                ' ถ้าความกว้างเกินพื้นที่ ให้ย่อตามความกว้างแทน (กันล้นขอบซ้าย-ขวา)
                If newW > hostW - (gap * 2) Then
                    newW = Math.Max(1, hostW - (gap * 2))
                    newH = CInt(Math.Round(newW / ratio))
                End If

                picHomeLogo.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom
                picHomeLogo.Size = New Size(newW, newH)
                picHomeLogo.Location = New Point((hostW - newW) \ 2, gap)
            Catch
            End Try
        End Sub

        Public Sub ShowDashboard()
            CloseActiveForm()
            ' กลับสู่หน้าหลัก: แสดงโลโก้ประจำหน้าหลักอีกครั้ง
            If picHomeLogo IsNot Nothing Then picHomeLogo.Visible = True
            ' แสดงแถบภาพรวม (pnlOverview) และหัวข้อฟอร์ม (pnlFormHostHeader) เฉพาะหน้า Dashboard เท่านั้น
            pnlOverview.Visible = True
            pnlFormHostHeader.Visible = True
            SetActiveButton(btnDashboard)
            lblFormHostTitle.Text = "🪟 หน้าหลัก - ภาพรวมงานประจำวัน"
            lblFormHostHint.Visible = True
            Try
                LoadActualDashboardFromDb()
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("frmMain.ShowDashboard", ex)
                LoadSampleDashboardData()
            End Try
        End Sub

        Private Sub NavMenu_Click(sender As Object, e As EventArgs)
            Dim clickedBtn = TryCast(sender, Button)
            If clickedBtn Is Nothing Then Return
            SetActiveButton(clickedBtn)

            Select Case clickedBtn.Name
                Case "btnDashboard"
                    SetOverviewCompactMode(False)
                    ShowDashboard()

                Case "btnDonation"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmIncome(), "💰 บันทึกรายรับเงินบริจาค")
                    Try
                        LoadActualDonationOverviewFromDb()
                    Catch ex As Exception
                        ErrorLogHelper.LogSilentError("frmMain.NavMenu.DonationOverview", ex)
                        LoadDonationSample()
                    End Try

                Case "btnExpense"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmExpense(), "💸 บันทึกรายจ่ายของวัด")
                    Try
                        LoadActualExpenseOverviewFromDb()
                    Catch ex As Exception
                        ErrorLogHelper.LogSilentError("frmMain.NavMenu.ExpenseOverview", ex)
                        LoadExpenseSample()
                    End Try

                Case "btnReport"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmReports(), "📊 ศูนย์รายงานและส่งออก CSV/Excel")
                    Try
                        LoadActualReportOverviewFromDb()
                    Catch ex As Exception
                        ErrorLogHelper.LogSilentError("frmMain.NavMenu.ReportOverview", ex)
                        LoadReportSample()
                    End Try

                Case "btnMember"
                    SetOverviewCompactMode(True)
                    ShowFormInPanel(New FrmTransactions(), "👥 รายการเงินรับ-จ่ายทั้งหมด (ค้นหา/แก้ไข/ลบ)")
                    Try
                        LoadTransactionOverview()
                    Catch ex As Exception
                        ErrorLogHelper.LogSilentError("frmMain.NavMenu.TransactionOverview", ex)
                    End Try

                Case "btnMultiImport"
                    SetOverviewCompactMode(False)
                    ' เปิดแบบ Modal Dialog ตามสเปค (ห้ามผ่าน ShowFormInPanel — กันฟอร์มถูก Dispose กลางคันระหว่าง merge ไฟล์แนบ)
                    Dim importDlg As New FrmMultiMachineImport()
                    importDlg.Icon = Me.Icon
                    importDlg.ShowDialog()

                Case "btnExportTransfer"
                    SetOverviewCompactMode(False)
                    ' เปิดแบบ Modal Dialog ตามสเปค (ห้ามผ่าน ShowFormInPanel — กันฟอร์มถูก Dispose กลางคันระหว่าง export ไฟล์แนบ)
                    Dim exportDlg As New FrmExportForTransfer()
                    exportDlg.Icon = Me.Icon
                    exportDlg.ShowDialog()

                Case "btnVip"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmTempleSetting(), "🥇 ข้อมูลวัด - พระ/อาวาส/ผู้ทำบัญชี/พร้อมเพย์")
                    Try
                        LoadMonkSample()
                    Catch ex As Exception
                        ErrorLogHelper.LogSilentError("frmMain.NavMenu.MonkSample", ex)
                    End Try

                Case "btnActivity"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmTransfer(), "🎎 โอนเงินภายในระหว่างกองทุน/บัญชีธนาคาร")
                    Try
                        LoadActivitySample()
                    Catch ex As Exception
                        ErrorLogHelper.LogSilentError("frmMain.NavMenu.ActivitySample", ex)
                    End Try

                Case "btnSetting"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmMasterData(), "⚙️ จัดการข้อมูลหลัก ประเภท/กองทุน/บัญชี  และนำเข้าจังหวัด")
                    Try
                        LoadSettingSample()
                    Catch ex As Exception
                        ErrorLogHelper.LogSilentError("frmMain.NavMenu.SettingSample", ex)
                    End Try
            End Select
        End Sub

        Private Sub LoadActualDashboardFromDb()
            Db.EnsureSchema()
            lblOverviewTitle.Text = "🏁 ภาพรวมวันนี้ (คลิกการ์ดเพื่อไปหน้าโดยตรง) — ข้อมูลอัปเดตจากฐานข้อมูลจริง"

            ' กำหนดหัวข้อการ์ดหลัก 4 ใบ (โหลดเดอร์จะอ่านข้อความหัวข้อเหล่านี้เพื่อแมปไปยัง SQL ที่ตรงกัน)
            lblCard1Title.Text = "ผู้บริจาคเดือนนี้"
            lblCard2Title.Text = "ยอดรับเดือนนี้"
            lblCard3Title.Text = "ยอดจ่ายเดือนนี้"
            lblCard4Title.Text = "ยอดคงเหลือปัจจุบัน"

            RefreshDashboardOverview()
        End Sub

        ''' <summary>เมตริกที่การ์ดภาพรวมรองรับ (แมปจากข้อความหัวข้อการ์ด)</summary>
        Private Enum OverviewMetric
            Unknown
            DonorCount
            IncomeMonth
            ExpenseMonth
            Balance
        End Enum

        ''' <summary>
        ''' ตรวจสอบการ์ดทั้งหมดภายใน pnlOverview แบบไดนามิก:
        ''' อ่าน Name/Text ของ Label หัวข้อเพื่อหาเมตริกที่ต้องการ แล้วคำนวณจากฐานข้อมูลจริงผูกกลับไปยัง Label ค่า
        ''' </summary>
        Public Sub RefreshDashboardOverview()
            If pnlCards Is Nothing Then Return

            Using conn = Db.OpenConn()
                Dim firstOfMonth As Date = New Date(Date.Today.Year, Date.Today.Month, 1)
                Dim firstOfNextMonth As Date = firstOfMonth.AddMonths(1)
                For Each card As Control In pnlCards.Controls
                    Dim pnl = TryCast(card, Panel)
                    If pnl Is Nothing Then Continue For

                    Dim titleLbl As Label = Nothing
                    Dim valueLbl As Label = Nothing
                    For Each c As Control In pnl.Controls
                        Dim lbl = TryCast(c, Label)
                        If lbl Is Nothing Then Continue For
                        If lbl.Name.EndsWith("Title") Then titleLbl = lbl
                        If lbl.Name.EndsWith("Value") Then valueLbl = lbl
                    Next

                    If titleLbl Is Nothing OrElse valueLbl Is Nothing Then Continue For

                    Select Case ResolveOverviewMetric(titleLbl.Text)
                        Case OverviewMetric.DonorCount
                            Dim cnt = Db.ToIntOrZero(Db.DbScalar(conn,
                                "SELECT COUNT(*) FROM Transactions " &
                                "WHERE (TranType LIKE '%รับ%' OR TranType = 'Income') " &
                                "AND TranDate >= " & Db.AccessDateLiteral(firstOfMonth) & " " &
                                "AND TranDate < " & Db.AccessDateLiteral(firstOfNextMonth)))
                            valueLbl.Text = cnt.ToString("#,##0")
                        Case OverviewMetric.IncomeMonth
                            Dim total = Db.ToDecimalOrZero(Db.DbScalar(conn,
                                "SELECT SUM(Amount) FROM Transactions " &
                                "WHERE (TranType LIKE '%รับ%' OR TranType = 'Income') " &
                                "AND TranDate >= " & Db.AccessDateLiteral(firstOfMonth) & " " &
                                "AND TranDate < " & Db.AccessDateLiteral(firstOfNextMonth)))
                            valueLbl.Text = total.ToString("#,##0.00")
                        Case OverviewMetric.ExpenseMonth
                            Dim total = Db.ToDecimalOrZero(Db.DbScalar(conn,
                                "SELECT SUM(Amount) FROM Transactions " &
                                "WHERE (TranType LIKE '%จ่าย%' OR TranType = 'Expense') " &
                                "AND TranDate >= " & Db.AccessDateLiteral(firstOfMonth) & " " &
                                "AND TranDate < " & Db.AccessDateLiteral(firstOfNextMonth)))
                            valueLbl.Text = total.ToString("#,##0.00")
                        Case OverviewMetric.Balance
                            Dim bal = Db.ToDecimalOrZero(Db.DbScalar(conn,
                                "SELECT SUM(IIF(TranType='Income',Amount,0)) - SUM(IIF(TranType='Expense',Amount,0)) FROM Transactions"))
                            valueLbl.Text = bal.ToString("#,##0.00")
                        Case Else
                            valueLbl.Text = "0"
                    End Select
                Next
            End Using

            RefreshOverviewLayout()
        End Sub

        ''' <summary>แมปข้อความหัวข้อการ์ดไปยังเมตริก (อ่านจาก Text property แบบไดนามิก)</summary>
        Private Function ResolveOverviewMetric(titleText As String) As OverviewMetric
            Dim t = If(titleText, "").Trim()
            If t.IndexOf("ผู้บริจาค", StringComparison.OrdinalIgnoreCase) >= 0 Then Return OverviewMetric.DonorCount
            If t.IndexOf("คงเหลือ", StringComparison.OrdinalIgnoreCase) >= 0 Then Return OverviewMetric.Balance
            If t.IndexOf("ยอดรับ", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
               t.IndexOf("รายรับ", StringComparison.OrdinalIgnoreCase) >= 0 Then Return OverviewMetric.IncomeMonth
            If t.IndexOf("ยอดจ่าย", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
               t.IndexOf("รายจ่าย", StringComparison.OrdinalIgnoreCase) >= 0 Then Return OverviewMetric.ExpenseMonth
            Return OverviewMetric.Unknown
        End Function

        Private Sub LoadActualDonationOverviewFromDb()
            Db.EnsureSchema()
            Dim monthStart As New Date(Today.Year, Today.Month, 1)
            Dim monthEnd = monthStart.AddMonths(1).AddSeconds(-1)
            Using conn = Db.OpenConn()
                Dim count = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions WHERE TranType='Income' AND TranDate BETWEEN @d1 AND @d2",
                                                     New Tuple(Of String, Object)("@d1", monthStart),
                                                     New Tuple(Of String, Object)("@d2", monthEnd)))
                Dim total = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) FROM Transactions WHERE TranType='Income' AND TranDate BETWEEN @d1 AND @d2",
                                                       New Tuple(Of String, Object)("@d1", monthStart),
                                                       New Tuple(Of String, Object)("@d2", monthEnd)))
                Dim maxAmt = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT MAX(Amount) FROM Transactions WHERE TranType='Income' AND TranDate BETWEEN @d1 AND @d2",
                                                        New Tuple(Of String, Object)("@d1", monthStart),
                                                        New Tuple(Of String, Object)("@d2", monthEnd)))
                Dim avg = If(count > 0, total / count, 0D)

                lblOverviewTitle.Text = "💰 ภาพรวมเงินรับเดือนนี้ (ข้อมูลจริง)"
                lblCard1Title.Text = "จำนวนใบเสร็จรับเงิน"
                lblCard1Value.Text = count.ToString("#,##0") & " ใบ"

                lblCard2Title.Text = "ยอดรับเดือนนี้ (บาท)"
                lblCard2Value.Text = total.ToString("#,##0.00")

                lblCard3Title.Text = "ค่าเฉลี่ย / คน"
                lblCard3Value.Text = avg.ToString("#,##0.00")

                lblCard4Title.Text = "บริจาคสูงสุด"
                lblCard4Value.Text = maxAmt.ToString("#,##0.00")
                RefreshOverviewLayout()
            End Using
        End Sub

        Private Sub LoadActualExpenseOverviewFromDb()
            Db.EnsureSchema()
            Dim monthStart As New Date(Today.Year, Today.Month, 1)
            Dim monthEnd = monthStart.AddMonths(1).AddSeconds(-1)
            Using conn = Db.OpenConn()
                Dim count = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions WHERE TranType='Expense' AND TranDate BETWEEN @d1 AND @d2",
                                                     New Tuple(Of String, Object)("@d1", monthStart),
                                                     New Tuple(Of String, Object)("@d2", monthEnd)))
                Dim total = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) FROM Transactions WHERE TranType='Expense' AND TranDate BETWEEN @d1 AND @d2",
                                                       New Tuple(Of String, Object)("@d1", monthStart),
                                                       New Tuple(Of String, Object)("@d2", monthEnd)))
                Dim bal As Object = Db.DbScalar(conn, "SELECT SUM(IIF(TranType='Income',Amount,0))-SUM(IIF(TranType='Expense',Amount,0)) FROM Transactions")
                Dim avg = If(count > 0, total / count, 0D)

                lblOverviewTitle.Text = "💸 ภาพรวมเงินจ่ายเดือนนี้ (ข้อมูลจริง)"
                lblCard1Title.Text = "จำนวนรายการจ่าย"
                lblCard1Value.Text = count.ToString("#,##0") & " รายการ"

                lblCard2Title.Text = "ยอดจ่ายเดือนนี้"
                lblCard2Value.Text = total.ToString("#,##0.00")

                lblCard3Title.Text = "ค่าเฉลี่ย / รายการ"
                lblCard3Value.Text = avg.ToString("#,##0.00")

                lblCard4Title.Text = "ยอดคงเหลือสุทธิ"
                lblCard4Value.Text = Db.ToDecimalOrZero(bal).ToString("#,##0.00")
                RefreshOverviewLayout()
            End Using
        End Sub

        Private Sub OverviewCard_Click(sender As Object, e As EventArgs)
            Dim p = TryCast(sender, Panel)
            If p Is Nothing Then Return
            If p Is pnlCard1 Then
                btnMember.PerformClick()
            ElseIf p Is pnlCard2 Then
                btnDonation.PerformClick()
            ElseIf p Is pnlCard3 Then
                btnExpense.PerformClick()
            ElseIf p Is pnlCard4 Then
                btnReport.PerformClick()
            End If
        End Sub

        Public Sub ShowFormInPanel(childForm As Form, Optional titleOverride As String = Nothing)
            If childForm Is Nothing Then Return
            CloseActiveForm()
            ' ซ่อนโลโก้หน้าหลักทุกครั้งที่เปิดฟอร์มลูก เพื่อไม่ให้โลโก้ทับซ้อนกับฟอร์มงาน
            If picHomeLogo IsNot Nothing Then picHomeLogo.Visible = False
            ' ซ่อนแถบภาพรวม (pnlOverview) และหัวข้อฟอร์ม (pnlFormHostHeader) เพื่อให้ฟอร์มลูกเต็มความสูง
            pnlOverview.Visible = False
            pnlFormHostHeader.Visible = False
            _currentChildForm = childForm
            Dim hostContainer = If(pnlFormHostBody, pnlFormHost)

            childForm.TopLevel = False
            childForm.Icon = Me.Icon
            childForm.FormBorderStyle = FormBorderStyle.None
            childForm.Dock = DockStyle.Fill
            childForm.BackColor = Color.FromArgb(255, 253, 244)
            childForm.Font = New Font("Tahoma", 10.5!, FontStyle.Regular, GraphicsUnit.Point, 222)

            If pnlFormHostHeader IsNot Nothing AndAlso Not String.IsNullOrEmpty(titleOverride) Then
                lblFormHostTitle.Text = titleOverride
                lblFormHostHint.Visible = False
            End If

            hostContainer.Controls.Add(childForm)
            pnlFormHost.Tag = childForm
            childForm.Show()
        End Sub

        Public Sub CloseActiveForm()
            Dim hostContainer = If(pnlFormHostBody, pnlFormHost)
            If _currentChildForm IsNot Nothing AndAlso Not _currentChildForm.IsDisposed Then
                Try
                    hostContainer.Controls.Remove(_currentChildForm)
                Finally
                    _currentChildForm.Close()
                    _currentChildForm.Dispose()
                    _currentChildForm = Nothing
                End Try
            End If
            pnlFormHost.Tag = Nothing
        End Sub

        Private Sub ShowPlaceholder(formKey As String, title As String, description As String)
            Dim hostContainer = If(pnlFormHostBody, pnlFormHost)
            Dim ph As New Panel()
            ph.Name = "pnlPlaceholder_" & formKey
            ph.Dock = DockStyle.Fill
            ph.BackColor = Color.FromArgb(255, 253, 244)
            ph.Padding = New Padding(40)
            ph.AutoScroll = True

            Dim pct As New PictureBox()
            pct.Size = New Size(96, 96)
            pct.SizeMode = PictureBoxSizeMode.CenterImage
            pct.BackColor = Color.FromArgb(250, 240, 210)
            pct.Image = MakeIconBitmap("🚧", Color.FromArgb(161, 98, 7), New Size(96, 96), 72)
            pct.Location = New Point(40, 40)
            ph.Controls.Add(pct)

            Dim lblTitle As New Label()
            lblTitle.AutoSize = True
            lblTitle.Font = New Font("Tahoma", 16.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblTitle.ForeColor = Color.FromArgb(120, 53, 15)
            lblTitle.Location = New Point(160, 48)
            lblTitle.Text = title
            ph.Controls.Add(lblTitle)

            Dim lblDesc As New Label()
            lblDesc.AutoSize = True
            lblDesc.Font = New Font("Tahoma", 11.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            lblDesc.ForeColor = Color.FromArgb(90, 70, 40)
            lblDesc.Location = New Point(160, 96)
            lblDesc.MaximumSize = New Size(780, 0)
            lblDesc.Text = "💡 คำแนะนำสำหรับทีมพัฒนา" & vbCrLf & vbCrLf & description
            ph.Controls.Add(lblDesc)

            Dim lblHow As New Label()
            lblHow.AutoSize = True
            lblHow.Font = New Font("Tahoma", 10.5!, FontStyle.Regular, GraphicsUnit.Point, 222)
            lblHow.ForeColor = Color.FromArgb(120, 80, 40)
            lblHow.MaximumSize = New Size(880, 0)
            lblHow.Text = vbCrLf & vbCrLf &
                         "📌 วิธีเพิ่มฟอร์มงานในอนาคต:" & vbCrLf &
                         "   1) เพิ่มไฟล์ Form ใหม่ (เช่น frmDonation.vb, frmExpense.vb) ลงในโปรเจกต์" & vbCrLf &
                         "   2) แก้ไขไฟล์ frmMain.vb ที่ Select Case clickedBtn.Name" & vbCrLf &
                         "   3) เปลี่ยนบรรทัด ShowPlaceholder(...) เป็น ShowFormInPanel(New frmDonation(), ""💰 บันทึกเงินบริจาค"")" & vbCrLf &
                         "   4) ฟอร์มลูกจะแสดงตรงพื้นที่นี้ทันที และขยายขนาดฟอนต์อัตโนมัติ"
            lblHow.Location = New Point(40, 200)
            ph.Controls.Add(lblHow)

            CloseActiveForm()
            If picHomeLogo IsNot Nothing Then picHomeLogo.Visible = False
            hostContainer.Controls.Add(ph)
        End Sub

        Private Sub SetActiveButton(activeBtn As Button)
            If _currentActiveButton IsNot Nothing Then
                _currentActiveButton.BackColor = Color.Transparent
                _currentActiveButton.ForeColor = Color.White
                _currentActiveButton.Font = New Font("Tahoma", 11.25!, FontStyle.Regular, GraphicsUnit.Point, 222)
                _currentActiveButton.Height = 50
            End If

            activeBtn.BackColor = Color.FromArgb(234, 179, 8)
            activeBtn.ForeColor = Color.FromArgb(69, 26, 3)
            activeBtn.Font = New Font("Tahoma", 12.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            activeBtn.Height = 54

            _currentActiveButton = activeBtn
        End Sub

        Private Sub LoadSampleDashboardData()
            lblOverviewTitle.Text = "🏁 ภาพรวมวันนี้ (คลิกการ์ดเพื่อไปหน้าโดยตรง)"
            lblCard1Title.Text = "ผู้บริจาคเดือนนี้"
            lblCard1Value.Text = "128 คน"
            ' lblCard1Icon.Text = "📿"

            lblCard2Title.Text = "ยอดรับเงินเดือนนี้"
            lblCard2Value.Text = "248,500"
            ' lblCard2Icon.Text = "💰"

            lblCard3Title.Text = "ยอดจ่ายเงินเดือนนี้"
            lblCard3Value.Text = "86,250"
            ' lblCard3Icon.Text = "💸"

            lblCard4Title.Text = "ยอดคงเหลือทั้งหมด"
            lblCard4Value.Text = "1,892,750"
            ' lblCard4Icon.Text = "💵"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadDonationSample()
            lblOverviewTitle.Text = "💰 ภาพรวมเงินรับเดือนนี้"
            lblCard1Title.Text = "จำนวนใบเสร็จรับเงิน"
            lblCard1Value.Text = "76 ใบ"
            ' lblCard1Icon.Text = "🧾"

            lblCard2Title.Text = "ยอดรับเดือนนี้ (บาท)"
            lblCard2Value.Text = "248,500"
            ' lblCard2Icon.Text = "💰"

            lblCard3Title.Text = "ค่าเฉลี่ย / คน"
            lblCard3Value.Text = "3,270"
            ' lblCard3Icon.Text = "📊"

            lblCard4Title.Text = "บริจาคสูงสุด"
            lblCard4Value.Text = "50,000"
            ' lblCard4Icon.Text = "🌟"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadExpenseSample()
            lblOverviewTitle.Text = "💸 ภาพรวมเงินจ่ายเดือนนี้"
            lblCard1Title.Text = "จำนวนรายการจ่าย"
            lblCard1Value.Text = "32 รายการ"
            ' lblCard1Icon.Text = "📝"

            lblCard2Title.Text = "ยอดจ่ายเดือนนี้"
            lblCard2Value.Text = "86,250"
            ' lblCard2Icon.Text = "💸"

            lblCard3Title.Text = "ค่าเฉลี่ย / รายการ"
            lblCard3Value.Text = "2,695"
            ' lblCard3Icon.Text = "📊"

            lblCard4Title.Text = "งบคงเหลือ"
            lblCard4Value.Text = "213,750"
            ' lblCard4Icon.Text = "🗓️"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadActualReportOverviewFromDb()
            Db.EnsureSchema()
            Dim yearStart As New Date(Today.Year, 1, 1)
            Dim monthStart As New Date(Today.Year, Today.Month, 1)
            Dim monthEnd = monthStart.AddMonths(1).AddSeconds(-1)

            Using conn = Db.OpenConn()
                ' 1. รายการรับ-จ่ายรวมทั้งหมด
                Dim allCount = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions"))

                ' 2. รายได้สุทธิ YTD (ตั้งแต่ต้นปีถึงปัจจุบัน)
                Dim ytdNet = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(IIF(TranType='Income',Amount,0)) - SUM(IIF(TranType='Expense',Amount,0)) FROM Transactions WHERE TranDate >= @y1",
                                                        New Tuple(Of String, Object)("@y1", yearStart)))

                ' 3. รายการที่เกิดขึ้นในเดือนนี้ (แทนที่ "เป้าหมาย" ที่ไม่มีอยู่จริง)
                Dim monthCount = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions WHERE TranDate BETWEEN @m1 AND @m2",
                                                        New Tuple(Of String, Object)("@m1", monthStart),
                                                        New Tuple(Of String, Object)("@m2", monthEnd)))

                ' 4. ยอดรวมต้นปี (ยอดยกมาจากปีก่อนๆ ทั้งหมด)
                Dim prevYearBal = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(IIF(TranType='Income',Amount,0)) - SUM(IIF(TranType='Expense',Amount,0)) FROM Transactions WHERE TranDate < @y1",
                                                            New Tuple(Of String, Object)("@y1", yearStart)))

                lblOverviewTitle.Text = "📊 ภาพรวมรายงานและสถิติ (ข้อมูลจริง)"
                lblCard1Title.Text = "รายการรับ-จ่ายรวม"
                lblCard1Value.Text = allCount.ToString("#,##0") & " รายการ"

                lblCard2Title.Text = "รายได้สุทธิ YTD"
                lblCard2Value.Text = ytdNet.ToString("#,##0.00")

                lblCard3Title.Text = "รายการเดือนนี้"
                lblCard3Value.Text = monthCount.ToString("#,##0") & " รายการ"

                lblCard4Title.Text = "ยอดยกมาต้นปี"
                lblCard4Value.Text = prevYearBal.ToString("#,##0.00")

                RefreshOverviewLayout()
            End Using
        End Sub

        Private Sub LoadReportSample()
            lblOverviewTitle.Text = "📊 ภาพรวมรายงาน 6 เดือน"
            lblCard1Title.Text = "รายการรับ-จ่ายรวม"
            lblCard1Value.Text = "756 รายการ"
            ' lblCard1Icon.Text = "📑"

            lblCard2Title.Text = "รายได้สุทธิ YTD"
            lblCard2Value.Text = "840,150"
            ' lblCard2Icon.Text = "📈"

            lblCard3Title.Text = "เป้าหมายเดือนนี้"
            lblCard3Value.Text = "78.5 %"
            ' lblCard3Icon.Text = "🎯"

            lblCard4Title.Text = "ยอดรวมต้นปี"
            lblCard4Value.Text = "1,285,400"
            ' lblCard4Icon.Text = "🗃️"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadTransactionOverview()
            Try
                Using conn = Db.OpenConn()
                    Dim totalItems = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions"))
                    Dim incomeAmount = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) FROM Transactions WHERE TranType='Income'"))
                    Dim expenseAmount = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) FROM Transactions WHERE TranType='Expense'"))
                    Dim transferAmount = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) FROM Transactions WHERE TranType='Transfer'"))

                    lblOverviewTitle.Text = "📒 ภาพรวมรายการเงินรับ-จ่าย"
                    lblCard1Title.Text = "รายการทั้งหมด"
                    lblCard1Value.Text = totalItems.ToString("#,##0") & " รายการ"
                    ' lblCard1Icon.Text = "📋"

                    lblCard2Title.Text = "รายรับสะสม"
                    lblCard2Value.Text = incomeAmount.ToString("#,##0.00")
                    ' lblCard2Icon.Text = "💰"

                    lblCard3Title.Text = "รายจ่ายสะสม"
                    lblCard3Value.Text = expenseAmount.ToString("#,##0.00")
                    ' lblCard3Icon.Text = "💸"

                    lblCard4Title.Text = "โอนภายในสะสม"
                    lblCard4Value.Text = transferAmount.ToString("#,##0.00")
                    ' lblCard4Icon.Text = "🔁"
                    RefreshOverviewLayout()
                End Using
            Catch
                lblOverviewTitle.Text = "📒 ภาพรวมรายการเงินรับ-จ่าย"
                lblCard1Title.Text = "รายการทั้งหมด"
                lblCard1Value.Text = "0 รายการ"
                ' lblCard1Icon.Text = "📋"

                lblCard2Title.Text = "รายรับสะสม"
                lblCard2Value.Text = "0.00"
                ' lblCard2Icon.Text = "💰"

                lblCard3Title.Text = "รายจ่ายสะสม"
                lblCard3Value.Text = "0.00"
                ' lblCard3Icon.Text = "💸"

                lblCard4Title.Text = "โอนภายในสะสม"
                lblCard4Value.Text = "0.00"
                ' lblCard4Icon.Text = "🔁"
                RefreshOverviewLayout()
            End Try
        End Sub

        Private Sub LoadMemberSample()
            lblOverviewTitle.Text = "👥 ภาพรวมสมาชิกผู้บริจาค"
            lblCard1Title.Text = "สมาชิกทั้งหมด"
            lblCard1Value.Text = "342 คน"
            ' lblCard1Icon.Text = "👥"

            lblCard2Title.Text = "เพิ่มเดือนนี้"
            lblCard2Value.Text = "18 คน"
            ' lblCard2Icon.Text = "➕"

            lblCard3Title.Text = "ระดับทอง"
            lblCard3Value.Text = "24 คน"
            ' lblCard3Icon.Text = "🏅"

            lblCard4Title.Text = "บริจาคประจำเดือน"
            lblCard4Value.Text = "128 คน"
            ' lblCard4Icon.Text = "🔔"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadMonkSample()
            Try
                Using conn = Db.OpenConn()
                    Dim row = Db.GetTable(conn,
                        "SELECT TOP 1 ts.TempleName, " &
                        "a.FullName AS AbbotName, w.FullName AS WaiyawatName, b.FullName AS BookkeeperName " &
                        "FROM ((TempleSetting ts " &
                        "LEFT JOIN Personnel a ON ts.AbbotPersonnelID = a.PersonnelID) " &
                        "LEFT JOIN Personnel w ON ts.WaiyawatPersonnelID = w.PersonnelID) " &
                        "LEFT JOIN Personnel b ON ts.BookkeeperPersonnelID = b.PersonnelID " &
                        "ORDER BY ts.ID DESC")
                    If row.Rows.Count > 0 Then
                        Dim r = row.Rows(0)
                        lblOverviewTitle.Text = "🥇 ข้อมูลบุคลากรและวัด (ข้อมูลจริง)"
                        lblCard1Title.Text = "ชื่อวัด"
                        lblCard1Value.Text = If(r("TempleName") Is DBNull.Value, "-", r("TempleName").ToString())
                        lblCard2Title.Text = "เจ้าอาวาส"
                        lblCard2Value.Text = If(r("AbbotName") Is DBNull.Value, "-", r("AbbotName").ToString())
                        lblCard3Title.Text = "ไวยาวัจกร"
                        lblCard3Value.Text = If(r("WaiyawatName") Is DBNull.Value, "-", r("WaiyawatName").ToString())
                        lblCard4Title.Text = "ผู้ทำบัญชี"
                        lblCard4Value.Text = If(r("BookkeeperName") Is DBNull.Value, "-", r("BookkeeperName").ToString())
                    Else
                        lblOverviewTitle.Text = "🥇 ข้อมูลบุคลากรและวัด (ยังไม่ได้ตั้งค่า)"
                        lblCard1Value.Text = "-" : lblCard2Value.Text = "-" : lblCard3Value.Text = "-" : lblCard4Value.Text = "-"
                    End If
                    RefreshOverviewLayout()
                End Using
            Catch
                lblOverviewTitle.Text = "🥇 ข้อมูลบุคลากรและวัด"
                lblCard1Value.Text = "-" : lblCard2Value.Text = "-" : lblCard3Value.Text = "-" : lblCard4Value.Text = "-"
                RefreshOverviewLayout()
            End Try
        End Sub

        Private Sub LoadActivitySample()
            Try
                Using conn = Db.OpenConn()
                    Dim monthStart As New Date(Today.Year, Today.Month, 1)
                    Dim monthEnd = monthStart.AddMonths(1).AddSeconds(-1)
                    Dim count = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions WHERE TranType='Transfer' AND TranDate BETWEEN @d1 AND @d2",
                                                         New Tuple(Of String, Object)("@d1", monthStart),
                                                         New Tuple(Of String, Object)("@d2", monthEnd)))
                    Dim total = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) FROM Transactions WHERE TranType='Transfer' AND TranDate BETWEEN @d1 AND @d2",
                                                           New Tuple(Of String, Object)("@d1", monthStart),
                                                           New Tuple(Of String, Object)("@d2", monthEnd)))
                    Dim allTotal = Db.ToDecimalOrZero(Db.DbScalar(conn, "SELECT SUM(Amount) FROM Transactions WHERE TranType='Transfer'"))

                    lblOverviewTitle.Text = "🎎 ภาพรวมการโอนเงินภายใน (ข้อมูลจริง)"
                    lblCard1Title.Text = "รายการโอนเดือนนี้"
                    lblCard1Value.Text = count.ToString("#,##0") & " รายการ"
                    lblCard2Title.Text = "ยอดโอนเดือนนี้"
                    lblCard2Value.Text = total.ToString("#,##0.00")
                    lblCard3Title.Text = "ยอดโอนสะสมทั้งหมด"
                    lblCard3Value.Text = allTotal.ToString("#,##0.00")
                    lblCard4Title.Text = "สถานะ"
                    lblCard4Value.Text = "ปกติ"
                    RefreshOverviewLayout()
                End Using
            Catch
                lblOverviewTitle.Text = "🎎 ภาพรวมการโอนเงินภายใน"
                lblCard1Value.Text = "0" : lblCard2Value.Text = "0.00" : lblCard3Value.Text = "0.00" : lblCard4Value.Text = "-"
                RefreshOverviewLayout()
            End Try
        End Sub

        Private Sub LoadSettingSample()
            Try
                Using conn = Db.OpenConn()
                    Dim catCount = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Categories"))
                    Dim fundCount = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM Funds"))
                    Dim bankCount = Db.ToIntOrZero(Db.DbScalar(conn, "SELECT COUNT(*) FROM BankAccounts"))

                    lblOverviewTitle.Text = "⚙️ ภาพรวมการตั้งค่าระบบ (ข้อมูลจริง)"
                    lblCard1Title.Text = "ประเภทรายการ"
                    lblCard1Value.Text = catCount.ToString("#,##0") & " ประเภท"
                    lblCard2Title.Text = "กองทุนทั้งหมด"
                    lblCard2Value.Text = fundCount.ToString("#,##0") & " กองทุน"
                    lblCard3Title.Text = "บัญชีธนาคาร"
                    lblCard3Value.Text = bankCount.ToString("#,##0") & " บัญชี"
                    lblCard4Title.Text = "สถานะฐานข้อมูล"
                    lblCard4Value.Text = "เชื่อมต่ออยู่"
                    RefreshOverviewLayout()
                End Using
            Catch
                lblOverviewTitle.Text = "⚙️ ภาพรวมการตั้งค่าระบบ"
                lblCard1Value.Text = "-" : lblCard2Value.Text = "-" : lblCard3Value.Text = "-" : lblCard4Value.Text = "ผิดพลาด"
                RefreshOverviewLayout()
            End Try
        End Sub

        Private Sub BtnClose_Click(sender As Object, e As EventArgs)
            Dim result = MessageBox.Show("ต้องการออกจากระบบหรือไม่?", "ยืนยันการออก",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                         MessageBoxDefaultButton.Button2)
            If result = DialogResult.Yes Then
                Application.Exit()
            End If
        End Sub

        Private Sub BtnMinimize_Click(sender As Object, e As EventArgs)
            Me.WindowState = FormWindowState.Minimized
        End Sub

        Private Sub BtnLogout_Click(sender As Object, e As EventArgs)
            Dim result = MessageBox.Show("ต้องการออกจากบัญชีผู้ใช้งานหรือไม่?", "ยืนยันการเข้าสู่ระบบ",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                         MessageBoxDefaultButton.Button2)
            If result = DialogResult.Yes Then
                MessageBox.Show("ระบบจะกลับสู่หน้าเข้าสู่ระบบ (ตัวอย่างเท่านั้น)" & vbCrLf & "นำฟอร์ม frmLogin มาเรียกในที่นี่ได้เลย",
                                "ข้อความจากระบบ", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Sub

        Private Sub btnBackup_Click(sender As Object, e As EventArgs)
            Using fbd As New FolderBrowserDialog()
                fbd.Description = "เลือกโฟลเดอร์ปลายทางที่ต้องการเก็บโฟลเดอร์สำรองข้อมูล (ฐานข้อมูลและรูปใบเสร็จ)"
                fbd.UseDescriptionForTitle = True
                fbd.ShowNewFolderButton = True

                If fbd.ShowDialog() = DialogResult.OK Then
                    ' เรียกใช้ Helper เพื่อสำรองข้อมูลแบบเต็มระบบ
                    DatabaseBackupHelper.BackupFullSystem(fbd.SelectedPath, False)
                End If
            End Using
        End Sub

        Private Sub btnRestore_Click(sender As Object, e As EventArgs)
            Dim result = MessageBox.Show("⚠️ คำเตือน: การคืนค่าข้อมูลจะนำข้อมูลจากไฟล์สำรองมาแทนที่ข้อมูลปัจจุบันทั้งหมด" & Environment.NewLine &
                                         "ระบบจะทำการปิดและเริ่มใหม่เพื่อโหลดข้อมูลชุดใหม่" & Environment.NewLine & Environment.NewLine &
                                         "คุณต้องการดำเนินการต่อหรือไม่?", "ยืนยันการคืนค่าข้อมูล",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)

            If result = DialogResult.Yes Then
                Using ofd As New OpenFileDialog()
                    ofd.Title = "เลือกไฟล์ฐานข้อมูลจากโฟลเดอร์สำรองข้อมูล (คืนค่า)"
                    ofd.Filter = "ไฟล์ที่รองรับทั้งหมด|*.accdb;*.jpg;*.jpeg;*.png;*.bmp|ไฟล์ฐานข้อมูล|*.accdb|ไฟล์รูปภาพ|*.jpg;*.jpeg;*.png;*.bmp|ไฟล์ทั้งหมด (*.*)|*.*"
                    ofd.CheckFileExists = True

                    If ofd.ShowDialog() = DialogResult.OK Then
                        ' ใช้โฟลเดอร์ที่ไฟล์นั้นตั้งอยู่เป็น backupFolder
                        Dim backupFolder = Path.GetDirectoryName(ofd.FileName)
                        If DatabaseBackupHelper.RestoreFullSystem(backupFolder) Then
                            MessageBox.Show("✅ คืนค่าข้อมูลสำเร็จ! โปรแกรมจะเริ่มใหม่ทันที", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            Application.Restart()
                        End If
                    End If
                End Using
            End If
        End Sub

        Private Sub btnVerifyBackup_Click(sender As Object, e As EventArgs)
            Using dlg As New FrmBackupVerify()
                dlg.Icon = Me.Icon
                dlg.ShowDialog(Me)
            End Using
        End Sub

        Private Sub FrmMain_FormClosing(sender As Object, e As FormClosingEventArgs)
            Try
                ' Tiered Backup: สำรองข้อมูลแบบ Weekly/Monthly/Yearly เมื่อปิดโปรแกรม
                DatabaseBackupHelper.RunTieredBackup()
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("frmMain.FormClosing.TieredBackup", ex)
            End Try
        End Sub
    End Class
End Namespace
