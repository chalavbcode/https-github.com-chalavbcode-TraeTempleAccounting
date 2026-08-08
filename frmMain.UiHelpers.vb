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
            AddHandler btnMultiImport.Click, AddressOf NavMenu_Click
            AddHandler btnSetting.Click, AddressOf NavMenu_Click
            AddHandler btnBackup.Click, AddressOf btnBackup_Click
            AddHandler btnRestore.Click, AddressOf btnRestore_Click

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
            AddHandler Me.FormClosing, AddressOf FrmMain_FormClosing
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
                ilIcons.Images.Clear()
                ilIcons.Images.Add("home", MakeIconBitmap("🏠", Color.FromArgb(69, 26, 3)))
                ilIcons.Images.Add("donation", MakeIconBitmap("💰", Color.FromArgb(22, 101, 52)))
                ilIcons.Images.Add("expense", MakeIconBitmap("💸", Color.FromArgb(153, 27, 27)))
                ilIcons.Images.Add("report", MakeIconBitmap("🖨️", Color.FromArgb(30, 64, 175)))
                ilIcons.Images.Add("member", MakeIconBitmap("📖", Color.FromArgb(124, 45, 18)))
                ilIcons.Images.Add("vip", MakeIconBitmap("🥇", Color.FromArgb(161, 98, 7)))
                ilIcons.Images.Add("activity", MakeIconBitmap("🎎", Color.FromArgb(131, 24, 67)))
                ilIcons.Images.Add("multiimport", MakeIconBitmap("🖥️", Color.FromArgb(16, 185, 129)))
                ilIcons.Images.Add("setting", MakeIconBitmap("⚙️", Color.FromArgb(75, 85, 99)))
                ilIcons.Images.Add("backup", MakeIconBitmap("💾", Color.FromArgb(5, 150, 105)))
                ilIcons.Images.Add("restore", MakeIconBitmap("🔄", Color.FromArgb(59, 130, 246)))

                ApplyButtonImage(btnDashboard, "home")
                ApplyButtonImage(btnDonation, "donation")
                ApplyButtonImage(btnExpense, "expense")
                ApplyButtonImage(btnReport, "report")
                ApplyButtonImage(btnMember, "member")
                ApplyButtonImage(btnVip, "vip")
                ApplyButtonImage(btnActivity, "activity")
                ApplyButtonImage(btnMultiImport, "multiimport")
                ApplyButtonImage(btnSetting, "setting")
                ApplyButtonImage(btnBackup, "backup")
                ApplyButtonImage(btnRestore, "restore")
            Catch
            End Try

            Try
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
            End If
        End Sub

        Private Sub ApplyInitialState()
            UpdateStatusTime()
            SetupToolTips()

            Dim tmrStatus As New WinTimer()
            tmrStatus.Interval = 1000
            AddHandler tmrStatus.Tick, Sub(s, e) UpdateStatusTime()
            tmrStatus.Start()
        End Sub

        Private Sub RefreshOverviewLayout()
            If pnlOverview IsNot Nothing Then
                pnlOverview.PerformLayout()
            End If
        End Sub

        Private Sub SetOverviewCompactMode(isCompact As Boolean)
            _compactOverviewMode = isCompact
            If pnlOverview IsNot Nothing Then
                pnlOverview.Visible = Not isCompact
            End If
            RefreshOverviewLayout()
        End Sub

        Private Sub UpdateStatusTime()
            Try
                Dim dbName = Path.GetFileName(AppPaths.DatabaseFile)
                lblStatusCenter.Text = $"🟢 สถานะระบบ: ปกติ | ฐานข้อมูล: {dbName} | {DateTime.Now:dd/MM/yyyy HH:mm:ss}"
            Catch
                lblStatusCenter.Text = $"🟢 สถานะระบบ: ปกติ | ฐานข้อมูล: เชื่อมต่อแล้ว | {DateTime.Now:dd/MM/yyyy HH:mm:ss}"
            End Try
        End Sub

        Private Sub SetupToolTips()
            If ttMain Is Nothing Then Return
            ttMain.SetToolTip(btnDashboard, "กลับไปที่หน้าสรุปภาพรวมของระบบ (Dashboard)")
            ttMain.SetToolTip(btnDonation, "บันทึกข้อมูลรายรับหรือเงินบริจาคเข้าวัด")
            ttMain.SetToolTip(btnExpense, "บันทึกข้อมูลรายจ่ายต่างๆ ของวัด")
            ttMain.SetToolTip(btnReport, "พิมพ์รายงานสรุปรายรับ-รายจ่าย (ย่อ/ละเอียด)")
            ttMain.SetToolTip(btnMember, "จัดการข้อมูลรายชื่อผู้บริจาค/สมาชิก")
            ttMain.SetToolTip(btnVip, "จัดการข้อมูลรายชื่อพระสงฆ์และไวยาวัจกร")
            ttMain.SetToolTip(btnActivity, "บันทึกข้อมูลกิจกรรมงานบุญและเทศกาล")
            ttMain.SetToolTip(btnMultiImport, "นำเข้าและรวมข้อมูลธุรกรรมจากหลายเครื่อง/หลาย Flash Drive เข้าสู่ฐานข้อมูลหลัก")
            ttMain.SetToolTip(btnSetting, "ตั้งค่าข้อมูลวัดและข้อมูลพื้นฐานของระบบ")
            ttMain.SetToolTip(btnBackup, "สำรองข้อมูลฐานข้อมูล (Backup Database)")
            ttMain.SetToolTip(btnRestore, "คืนค่าข้อมูลจากไฟล์สำรอง (Restore Database)")
            ttMain.SetToolTip(btnLogout, "ออกจากระบบและกลับไปหน้า Login")
            ttMain.SetToolTip(btnClose, "ปิดโปรแกรม")
            ttMain.SetToolTip(btnMinimize, "ย่อหน้าต่างโปรแกรมลง")
        End Sub
    End Class
End Namespace
