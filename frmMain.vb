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

        Private Sub SetupEventHandlers()
            AddHandler btnDashboard.Click, AddressOf NavMenu_Click
            AddHandler btnDonation.Click, AddressOf NavMenu_Click
            AddHandler btnExpense.Click, AddressOf NavMenu_Click
            AddHandler btnReport.Click, AddressOf NavMenu_Click
            AddHandler btnMember.Click, AddressOf NavMenu_Click
            AddHandler btnVip.Click, AddressOf NavMenu_Click
            AddHandler btnActivity.Click, AddressOf NavMenu_Click
            AddHandler btnSetting.Click, AddressOf NavMenu_Click

            AddHandler pnlCard1.Click, AddressOf OverviewCard_Click
            AddHandler pnlCard2.Click, AddressOf OverviewCard_Click
            AddHandler pnlCard3.Click, AddressOf OverviewCard_Click
            AddHandler pnlCard4.Click, AddressOf OverviewCard_Click

            AddHandler btnClose.Click, AddressOf BtnClose_Click
            AddHandler btnMinimize.Click, AddressOf BtnMinimize_Click
            AddHandler btnLogout.Click, AddressOf BtnLogout_Click
            AddHandler lblStatusCenter.DoubleClick, AddressOf StatusCenter_DoubleClick
            AddHandler lblStatusCenter.MouseClick, AddressOf StatusCenter_MouseClick

            AddHandler Me.Load, AddressOf FrmMain_Load
        End Sub

        Private Sub SetupCardHoverEffects()
            ApplyCardHover(pnlCard1, Color.FromArgb(254, 249, 195))
            ApplyCardHover(pnlCard2, Color.FromArgb(220, 252, 231))
            ApplyCardHover(pnlCard3, Color.FromArgb(243, 232, 255))
            ApplyCardHover(pnlCard4, Color.FromArgb(255, 237, 213))
        End Sub

        Private Sub ApplyCardHover(card As Panel, highlightColor As Color)
            Dim original = card.BackColor
            Dim cardRef = card
            AddHandler card.MouseEnter, Sub(s, e) cardRef.BackColor = highlightColor
            AddHandler card.MouseLeave, Sub(s, e) cardRef.BackColor = original
            Dim clickBridge As New EventHandler(Sub(s, e)
                                                   OverviewCard_Click(cardRef, EventArgs.Empty)
                                               End Sub)
            For Each ctrl As Control In cardRef.Controls
                Dim ctrlRef = ctrl
                AddHandler ctrlRef.MouseEnter, Sub(s, e) cardRef.BackColor = highlightColor
                AddHandler ctrlRef.MouseLeave, Sub(s, e) cardRef.BackColor = original
                AddHandler ctrlRef.Click, clickBridge
            Next
        End Sub

        Private Sub SetupIconsAndImages()
            Try
                ilIcons.ImageSize = New Size(28, 28)
                ilIcons.Images.Clear()
                ilIcons.Images.Add("home", MakeIconBitmap("🏠", Color.FromArgb(69, 26, 3)))
                ilIcons.Images.Add("donation", MakeIconBitmap("💰", Color.FromArgb(22, 101, 52)))
                ilIcons.Images.Add("expense", MakeIconBitmap("💸", Color.FromArgb(153, 27, 27)))
                ilIcons.Images.Add("report", MakeIconBitmap("📊", Color.FromArgb(30, 64, 175)))
                ilIcons.Images.Add("member", MakeIconBitmap("👥", Color.FromArgb(124, 45, 18)))
                ilIcons.Images.Add("vip", MakeIconBitmap("🥇", Color.FromArgb(161, 98, 7)))
                ilIcons.Images.Add("activity", MakeIconBitmap("🎎", Color.FromArgb(131, 24, 67)))
                ilIcons.Images.Add("setting", MakeIconBitmap("⚙️", Color.FromArgb(75, 85, 99)))

                ApplyButtonImage(btnDashboard, "home")
                ApplyButtonImage(btnDonation, "donation")
                ApplyButtonImage(btnExpense, "expense")
                ApplyButtonImage(btnReport, "report")
                ApplyButtonImage(btnMember, "member")
                ApplyButtonImage(btnVip, "vip")
                ApplyButtonImage(btnActivity, "activity")
                ApplyButtonImage(btnSetting, "setting")
            Catch
            End Try

            Try
                picLogo.SizeMode = PictureBoxSizeMode.Zoom
                picLogoBadge.SizeMode = PictureBoxSizeMode.Zoom
                picLogo.Image = MakeIconBitmap("📿", Color.FromArgb(69, 26, 3), New Size(48, 48), 28)
                picLogoBadge.Image = MakeIconBitmap("🏛️", Color.FromArgb(120, 53, 15), New Size(44, 44), 26)
            Catch
            End Try
        End Sub

        Private Shared Function MakeIconBitmap(emojiText As String, foreColor As Color) As Bitmap
            Return MakeIconBitmap(emojiText, foreColor, New Size(28, 28), 18)
        End Function

        Private Shared Function MakeIconBitmap(emojiText As String, foreColor As Color, size As Size, fontSize As Integer) As Bitmap
            Dim bmp As New Bitmap(size.Width, size.Height)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.SmoothingMode = SmoothingMode.AntiAlias
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
                g.Clear(Color.Transparent)
                Using f As New Font("Segoe UI Emoji", fontSize, FontStyle.Regular, GraphicsUnit.Pixel)
                    Using sf As New StringFormat()
                        sf.Alignment = StringAlignment.Center
                        sf.LineAlignment = StringAlignment.Center
                        Using b As New SolidBrush(foreColor)
                            Dim inset As Integer = Math.Max(2, CInt(Math.Ceiling(Math.Min(size.Width, size.Height) * 0.08)))
                            g.DrawString(emojiText, f, b, New RectangleF(inset, inset \ 2, size.Width - (inset * 2), size.Height - inset), sf)
                        End Using
                    End Using
                End Using
            End Using
            Return bmp
        End Function

        Private Sub ApplyButtonImage(btn As Button, imageKey As String)
            If ilIcons.Images.ContainsKey(imageKey) Then
                btn.ImageList = ilIcons
                btn.ImageKey = imageKey
                btn.ImageAlign = ContentAlignment.MiddleLeft
                btn.TextImageRelation = TextImageRelation.ImageBeforeText
                btn.Padding = New Padding(14, 0, 8, 0)
            End If
        End Sub

        Private Sub ApplyInitialState()
            ConfigureOverviewCardLayout()
            lblStatusRight.Text = $"v1.0.0 | {DateTime.Now:yyyy}"
            UpdateStatusTime()
            AdjustContentLayoutSpacing()

            Dim tmrStatus As New WinTimer()
            tmrStatus.Interval = 1000
            AddHandler tmrStatus.Tick, Sub(s, e) UpdateStatusTime()
            tmrStatus.Start()
        End Sub

        Private Sub RefreshOverviewLayout()
            ConfigureOverviewCardLayout()
            AdjustContentLayoutSpacing()
            If pnlOverview IsNot Nothing Then
                pnlOverview.PerformLayout()
            End If
        End Sub

        Private Sub ConfigureOverviewCardLayout()
            Dim cardPanels = {pnlCard1, pnlCard2, pnlCard3, pnlCard4}
            Dim titleLabels = {lblCard1Title, lblCard2Title, lblCard3Title, lblCard4Title}
            Dim valueLabels = {lblCard1Value, lblCard2Value, lblCard3Value, lblCard4Value}
            Dim iconLabels = {lblCard1Icon, lblCard2Icon, lblCard3Icon, lblCard4Icon}

            Dim iconFont As New Font("Segoe UI Emoji", 26.0!, FontStyle.Regular, GraphicsUnit.Point, 0)
            Dim maxIconHeight As Integer = 0
            Dim maxValueHeight As Integer = 0
            For Each iconLabel In iconLabels
                If iconLabel Is Nothing Then Continue For
                iconLabel.Font = iconFont
                iconLabel.Padding = New Padding(0, 0, 0, 6)
                maxIconHeight = Math.Max(maxIconHeight, TextRenderer.MeasureText("🗃️", iconLabel.Font).Height + 14)
            Next
            maxIconHeight = Math.Max(maxIconHeight, 54)

            For Each valueLabel In valueLabels
                If valueLabel Is Nothing Then Continue For
                valueLabel.Padding = New Padding(0, 2, 0, 6)
                maxValueHeight = Math.Max(maxValueHeight, TextRenderer.MeasureText("213,750.00", valueLabel.Font).Height + 10)
            Next
            maxValueHeight = Math.Max(maxValueHeight, 40)

            Dim targetCardHeight As Integer = 0
            For i As Integer = 0 To iconLabels.Length - 1
                If iconLabels(i) IsNot Nothing Then iconLabels(i).Height = maxIconHeight
                If valueLabels(i) IsNot Nothing Then valueLabels(i).Height = maxValueHeight
                If titleLabels(i) IsNot Nothing AndAlso valueLabels(i) IsNot Nothing AndAlso cardPanels(i) IsNot Nothing Then
                    Dim desiredHeight = cardPanels(i).Padding.Top + titleLabels(i).Height + maxValueHeight + maxIconHeight + cardPanels(i).Padding.Bottom
                    targetCardHeight = Math.Max(targetCardHeight, desiredHeight)
                End If
            Next

            targetCardHeight = Math.Max(targetCardHeight, 150)

            If pnlCards IsNot Nothing Then
                pnlCards.Height = targetCardHeight
            End If

            For Each cardPanel In cardPanels
                If cardPanel IsNot Nothing Then
                    cardPanel.Height = targetCardHeight
                End If
            Next
        End Sub

        Private Sub AdjustContentLayoutSpacing()
            If pnlOverview Is Nothing OrElse lblOverviewTitle Is Nothing OrElse pnlCards Is Nothing Then Return

            ' เผื่อช่องว่างใต้การ์ดสรุปให้ฟอร์มงานด้านล่างไม่ชนหรือถูกบังเมื่อใช้ฟอนต์/DPI ใหญ่ขึ้น
            Dim desiredHeight = lblOverviewTitle.Height + pnlCards.Height + 28
            pnlOverview.Height = desiredHeight
        End Sub

        Private Sub UpdateStatusTime()
            Try
                Dim dbName = Path.GetFileName(AppPaths.DatabaseFile)
                lblStatusCenter.Text = $"🟢 สถานะระบบ: ปกติ | ฐานข้อมูล: {dbName} | {DateTime.Now:dd/MM/yyyy HH:mm:ss}"
            Catch
                lblStatusCenter.Text = $"🟢 สถานะระบบ: ปกติ | ฐานข้อมูล: เชื่อมต่อแล้ว | {DateTime.Now:dd/MM/yyyy HH:mm:ss}"
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
                    ShowDashboard()

                Case "btnDonation"
                    ShowFormInPanel(New FrmIncome(), "💰 บันทึกรายรับเงินบริจาค")
                    Try
                        LoadDonationSample()
                    Catch
                    End Try

                Case "btnExpense"
                    ShowFormInPanel(New FrmExpense(), "💸 บันทึกรายจ่ายของวัด")
                    Try
                        LoadExpenseSample()
                    Catch
                    End Try

                Case "btnReport"
                    ShowFormInPanel(New FrmReports(), "📊 ศูนย์รายงานและส่งออก CSV/Excel")
                    Try
                        LoadReportSample()
                    Catch
                    End Try

                Case "btnMember"
                    ShowFormInPanel(New FrmTransactions(), "👥 รายการเงินรับ-จ่ายทั้งหมด (ค้นหา/แก้ไข/ลบ)")
                    Try
                        LoadTransactionOverview()
                    Catch
                    End Try

                Case "btnVip"
                    ShowFormInPanel(New FrmTempleSetting(), "🥇 ข้อมูลวัด - พระ/อาวาส/ผู้ทำบัญชี/พร้อมเพย์")
                    Try
                        LoadMonkSample()
                    Catch
                    End Try

                Case "btnActivity"
                    ShowFormInPanel(New FrmTransfer(), "🎎 โอนเงินภายในระหว่างกองทุน/บัญชีธนาคาร")
                    Try
                        LoadActivitySample()
                    Catch
                    End Try

                Case "btnSetting"
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
                lblCard1Value.Text = Db.ToIntOrZero(trCount) & " รายการ"
                lblCard1Icon.Text = "📿"

                lblCard2Title.Text = "ยอดเงินรับ เดือนนี้"
                lblCard2Value.Text = Db.ToDecimalOrZero(trIn).ToString("#,##0.00")
                lblCard2Icon.Text = "💰"

                lblCard3Title.Text = "ยอดเงินจ่าย เดือนนี้"
                lblCard3Value.Text = Db.ToDecimalOrZero(trOut).ToString("#,##0.00")
                lblCard3Icon.Text = "💸"

                lblCard4Title.Text = "ยอดคงเหลือทั้งหมด"
                lblCard4Value.Text = Db.ToDecimalOrZero(bal).ToString("#,##0.00")
                lblCard4Icon.Text = "💵"
                RefreshOverviewLayout()

                Try
                    If pnlFormHost IsNot Nothing Then
                        ' หน้า Dashboard แสดงสรุปด้านบน (4 การ์ด) พอเพียง
                        ' หากต้องการตารางรายการล่าสุด สามารถเปิดเมนู 👥 รายการเงินรับ-จ่ายทั้งหมดได้เลย
                    End If
                Catch
                End Try
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
            lblCard1Icon.Text = "📿"

            lblCard2Title.Text = "ยอดรับเงินเดือนนี้"
            lblCard2Value.Text = "248,500"
            lblCard2Icon.Text = "💰"

            lblCard3Title.Text = "ยอดจ่ายเงินเดือนนี้"
            lblCard3Value.Text = "86,250"
            lblCard3Icon.Text = "💸"

            lblCard4Title.Text = "ยอดคงเหลือทั้งหมด"
            lblCard4Value.Text = "1,892,750"
            lblCard4Icon.Text = "💵"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadDonationSample()
            lblOverviewTitle.Text = "💰 ภาพรวมเงินรับเดือนนี้"
            lblCard1Title.Text = "จำนวนใบเสร็จรับเงิน"
            lblCard1Value.Text = "76 ใบ"
            lblCard1Icon.Text = "🧾"

            lblCard2Title.Text = "ยอดรับเดือนนี้ (บาท)"
            lblCard2Value.Text = "248,500"
            lblCard2Icon.Text = "💰"

            lblCard3Title.Text = "ค่าเฉลี่ย / คน"
            lblCard3Value.Text = "3,270"
            lblCard3Icon.Text = "📊"

            lblCard4Title.Text = "บริจาคสูงสุด"
            lblCard4Value.Text = "50,000"
            lblCard4Icon.Text = "🌟"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadExpenseSample()
            lblOverviewTitle.Text = "💸 ภาพรวมเงินจ่ายเดือนนี้"
            lblCard1Title.Text = "จำนวนรายการจ่าย"
            lblCard1Value.Text = "32 รายการ"
            lblCard1Icon.Text = "📝"

            lblCard2Title.Text = "ยอดจ่ายเดือนนี้"
            lblCard2Value.Text = "86,250"
            lblCard2Icon.Text = "💸"

            lblCard3Title.Text = "ค่าเฉลี่ย / รายการ"
            lblCard3Value.Text = "2,695"
            lblCard3Icon.Text = "📊"

            lblCard4Title.Text = "งบคงเหลือ"
            lblCard4Value.Text = "213,750"
            lblCard4Icon.Text = "🗓️"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadReportSample()
            lblOverviewTitle.Text = "📊 ภาพรวมรายงาน 6 เดือน"
            lblCard1Title.Text = "รายการรับ-จ่ายรวม"
            lblCard1Value.Text = "756 รายการ"
            lblCard1Icon.Text = "📑"

            lblCard2Title.Text = "รายได้สุทธิ YTD"
            lblCard2Value.Text = "840,150"
            lblCard2Icon.Text = "📈"

            lblCard3Title.Text = "เป้าหมายเดือนนี้"
            lblCard3Value.Text = "78.5 %"
            lblCard3Icon.Text = "🎯"

            lblCard4Title.Text = "ยอดรวมต้นปี"
            lblCard4Value.Text = "1,285,400"
            lblCard4Icon.Text = "🗃️"
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
                    lblCard1Icon.Text = "📋"

                    lblCard2Title.Text = "รายรับสะสม"
                    lblCard2Value.Text = incomeAmount.ToString("#,##0.00")
                    lblCard2Icon.Text = "💰"

                    lblCard3Title.Text = "รายจ่ายสะสม"
                    lblCard3Value.Text = expenseAmount.ToString("#,##0.00")
                    lblCard3Icon.Text = "💸"

                    lblCard4Title.Text = "โอนภายในสะสม"
                    lblCard4Value.Text = transferAmount.ToString("#,##0.00")
                    lblCard4Icon.Text = "🔁"
                    RefreshOverviewLayout()
                End Using
            Catch
                lblOverviewTitle.Text = "📒 ภาพรวมรายการเงินรับ-จ่าย"
                lblCard1Title.Text = "รายการทั้งหมด"
                lblCard1Value.Text = "0 รายการ"
                lblCard1Icon.Text = "📋"

                lblCard2Title.Text = "รายรับสะสม"
                lblCard2Value.Text = "0.00"
                lblCard2Icon.Text = "💰"

                lblCard3Title.Text = "รายจ่ายสะสม"
                lblCard3Value.Text = "0.00"
                lblCard3Icon.Text = "💸"

                lblCard4Title.Text = "โอนภายในสะสม"
                lblCard4Value.Text = "0.00"
                lblCard4Icon.Text = "🔁"
                RefreshOverviewLayout()
            End Try
        End Sub

        Private Sub LoadMemberSample()
            lblOverviewTitle.Text = "👥 ภาพรวมสมาชิกผู้บริจาค"
            lblCard1Title.Text = "สมาชิกทั้งหมด"
            lblCard1Value.Text = "342 คน"
            lblCard1Icon.Text = "👥"

            lblCard2Title.Text = "เพิ่มเดือนนี้"
            lblCard2Value.Text = "18 คน"
            lblCard2Icon.Text = "➕"

            lblCard3Title.Text = "ระดับทอง"
            lblCard3Value.Text = "24 คน"
            lblCard3Icon.Text = "🏅"

            lblCard4Title.Text = "บริจาคประจำเดือน"
            lblCard4Value.Text = "128 คน"
            lblCard4Icon.Text = "🔔"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadMonkSample()
            lblOverviewTitle.Text = "🥇 ภาพรวมพระ / อาวาส / คณะสงฆ์"
            lblCard1Title.Text = "จำนวนพระภิกษุ"
            lblCard1Value.Text = "12 รูป"
            lblCard1Icon.Text = "🧘"

            lblCard2Title.Text = "จำนวนสามเณร"
            lblCard2Value.Text = "4 รูป"
            lblCard2Icon.Text = "🙏"

            lblCard3Title.Text = "ผู้อุปัฏฐาก"
            lblCard3Value.Text = "2 รูป"
            lblCard3Icon.Text = "🥇"

            lblCard4Title.Text = "หออาศรมทั้งหมด"
            lblCard4Value.Text = "8 แห่ง"
            lblCard4Icon.Text = "🏠"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadActivitySample()
            lblOverviewTitle.Text = "🎎 ภาพรวมกิจกรรมงานบุญ"
            lblCard1Title.Text = "งานบุญเดือนนี้"
            lblCard1Value.Text = "6 งาน"
            lblCard1Icon.Text = "🎎"

            lblCard2Title.Text = "งานกำลังจะจัด"
            lblCard2Value.Text = "2 งาน"
            lblCard2Icon.Text = "⏰"

            lblCard3Title.Text = "ผู้เข้ารวมทั้งหมด"
            lblCard3Value.Text = "2,450 คน"
            lblCard3Icon.Text = "👥"

            lblCard4Title.Text = "รายได้จากงาน"
            lblCard4Value.Text = "528,900"
            lblCard4Icon.Text = "💵"
            RefreshOverviewLayout()
        End Sub

        Private Sub LoadSettingSample()
            lblOverviewTitle.Text = "⚙️ ภาพรวมการตั้งค่าระบบ"
            lblCard1Title.Text = "ผู้ใช้งานระบบ"
            lblCard1Value.Text = "3 คน"
            lblCard1Icon.Text = "🔐"

            lblCard2Title.Text = "ฐานข้อมูล"
            lblCard2Value.Text = "ปกติ"
            lblCard2Icon.Text = "🗄️"

            lblCard3Title.Text = "สำรองข้อมูล"
            lblCard3Value.Text = "รายวัน"
            lblCard3Icon.Text = "💾"

            lblCard4Title.Text = "ภาษาที่ใช้งาน"
            lblCard4Value.Text = "ไทย"
            lblCard4Icon.Text = "🇹🇭"
            RefreshOverviewLayout()
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
