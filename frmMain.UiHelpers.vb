Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Windows.Forms
Imports WinTimer = System.Windows.Forms.Timer

Namespace TempleAccounting
    Public Partial Class frmMain
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
    End Class
End Namespace
