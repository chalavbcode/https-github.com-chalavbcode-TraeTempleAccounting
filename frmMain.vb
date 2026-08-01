Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports System.IO
Imports WinTimer = System.Windows.Forms.Timer

Namespace TempleAccounting
    Public Partial Class frmMain
        Inherits Form

        Private _currentActiveButton As Button
        Private _currentChildForm As Form
        Private _compactOverviewMode As Boolean = False

        Public Sub New()
            Try
                Dim ignore = AppPaths.DatabaseFile
            Catch ex As Exception
                AppPaths.LogCrash(ex, "AppPaths init")
            End Try

            Try
                InitializeComponent()
                SetupIconsAndImages()
                SetupEventHandlers()
                SetupCardHoverEffects()
                ApplyInitialState()
            Catch ex As Exception
                AppPaths.LogCrash(ex, "frmMain New() UI init")
                MessageBox.Show("⛔ ไม่สามารถสร้างหน้าต่างหลักได้: " & vbCrLf & ex.Message & vbCrLf & vbCrLf &
                                "📄 ดูรายละเอียดที่: " & Path.Combine(AppPaths.LogsFolder, "crash.log"),
                                "TempleAccounting - Startup Failed", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Throw
            End Try
        End Sub

        Private Sub FrmMain_Load(sender As Object, e As EventArgs)
            Try
                Dim tt As New ToolTip()
                tt.SetToolTip(lblStatusCenter, "ดับเบิ้ลคลิก: เปิดโฟลเดอร์ฐานข้อมูล" & vbCrLf & "คลิกขวา: คัดลอกที่อยู่ไฟล์ฐานข้อมูล" & vbCrLf & "ไฟล์: " & AppPaths.DatabaseFile)
            Catch
            End Try
            ShowDashboard()
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

        Public Sub ShowDashboard()
            CloseActiveForm()
            SetActiveButton(btnDashboard)
            lblFormHostTitle.Text = "🪟 หน้าหลัก - ภาพรวมงานประจำวัน"
            lblFormHostHint.Visible = True
            Try
                LoadActualDashboardFromDb()
            Catch
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
                    Catch
                        LoadDonationSample()
                    End Try

                Case "btnExpense"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmExpense(), "💸 บันทึกรายจ่ายของวัด")
                    Try
                        LoadActualExpenseOverviewFromDb()
                    Catch
                        LoadExpenseSample()
                    End Try

                Case "btnReport"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmReports(), "📊 ศูนย์รายงานและส่งออก CSV/Excel")
                    Try
                        LoadReportSample()
                    Catch
                    End Try

                Case "btnMember"
                    SetOverviewCompactMode(True)
                    ShowFormInPanel(New FrmTransactions(), "👥 รายการเงินรับ-จ่ายทั้งหมด (ค้นหา/แก้ไข/ลบ)")
                    Try
                        LoadTransactionOverview()
                    Catch
                    End Try

                Case "btnVip"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmTempleSetting(), "🥇 ข้อมูลวัด - พระ/อาวาส/ผู้ทำบัญชี/พร้อมเพย์")
                    Try
                        LoadMonkSample()
                    Catch
                    End Try

                Case "btnActivity"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmTransfer(), "🎎 โอนเงินภายในระหว่างกองทุน/บัญชีธนาคาร")
                    Try
                        LoadActivitySample()
                    Catch
                    End Try

                Case "btnSetting"
                    SetOverviewCompactMode(False)
                    ShowFormInPanel(New FrmMasterData(), "⚙️ จัดการข้อมูลหลัก ประเภท/กองทุน/บัญชี  และนำเข้าจังหวัด")
                    Try
                        LoadSettingSample()
                    Catch
                    End Try
            End Select
        End Sub

        Private Sub LoadActualDashboardFromDb()
            Db.EnsureSchema()
            Dim monthStart As New Date(Today.Year, Today.Month, 1)
            Dim monthEnd = monthStart.AddMonths(1).AddSeconds(-1)
            Using conn = Db.OpenConn()
                Dim trCount As Object = Db.DbScalar(conn, "SELECT COUNT(*) FROM Transactions WHERE TranType='Income' AND TranDate BETWEEN @d1 AND @d2",
                                                    New Tuple(Of String, Object)("@d1", monthStart),
                                                    New Tuple(Of String, Object)("@d2", monthEnd))
                Dim trIn As Object = Db.DbScalar(conn, "SELECT SUM(Amount) FROM Transactions WHERE TranType='Income' AND TranDate BETWEEN @d1 AND @d2",
                                                    New Tuple(Of String, Object)("@d1", monthStart),
                                                    New Tuple(Of String, Object)("@d2", monthEnd))
                Dim trOut As Object = Db.DbScalar(conn, "SELECT SUM(Amount) FROM Transactions WHERE TranType='Expense' AND TranDate BETWEEN @d1 AND @d2",
                                                    New Tuple(Of String, Object)("@d1", monthStart),
                                                    New Tuple(Of String, Object)("@d2", monthEnd))
                Dim bal As Object = Db.DbScalar(conn, "SELECT SUM(IIF(TranType='Income',Amount,0))-SUM(IIF(TranType='Expense',Amount,0)) FROM Transactions")

                lblOverviewTitle.Text = "🏁 ภาพรวมวันนี้ (คลิกการ์ดเพื่อไปหน้าโดยตรง) — ข้อมูลอัปเดตจากฐานข้อมูลจริง"
                lblCard1Title.Text = "รายการเงินรับ เดือนนี้"
                lblCard1Value.Text = Db.ToIntOrZero(trCount).ToString("#,##0") & " รายการ"

                lblCard2Title.Text = "ยอดเงินรับ เดือนนี้"
                lblCard2Value.Text = Db.ToDecimalOrZero(trIn).ToString("#,##0.00")

                lblCard3Title.Text = "ยอดเงินจ่าย เดือนนี้"
                lblCard3Value.Text = Db.ToDecimalOrZero(trOut).ToString("#,##0.00")

                lblCard4Title.Text = "ยอดคงเหลือทั้งหมด"
                lblCard4Value.Text = Db.ToDecimalOrZero(bal).ToString("#,##0.00")
                RefreshOverviewLayout()
            End Using
        End Sub

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
            _currentChildForm = childForm
            Dim hostContainer = If(pnlFormHostBody, pnlFormHost)

            childForm.TopLevel = False
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
            lblDesc.Text = "💡 คำแนะนำสำหรับทีมพัฒนา (Placeholder)" & vbCrLf & vbCrLf & description
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
                    Dim row = Db.GetTable(conn, "SELECT TOP 1 TempleName, AbbotName, WaiyawatName, BookkeeperName FROM TempleSetting ORDER BY ID DESC")
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
    End Class
End Namespace
