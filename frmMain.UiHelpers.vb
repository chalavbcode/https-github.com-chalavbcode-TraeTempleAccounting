Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Windows.Forms
Imports WinTimer = System.Windows.Forms.Timer

Namespace TempleAccounting
    Public Partial Class frmMain
        ' ===== Developer Tools (Onboarding Wizard) =====
        Private WithEvents btnTestWizard As Button
        Private WithEvents btnResetOnboarding As Button

        Private Sub SetupDeveloperTools()
            Try
                ' สร้าง Panel สำหรับเครื่องมือนักพัฒนา
                Dim pnlDev As New Panel()
                pnlDev.Dock = DockStyle.Top
                pnlDev.Height = 80
                pnlDev.BackColor = Color.FromArgb(75, 85, 99)
                pnlDev.Padding = New Padding(8, 6, 8, 4)

                ' ปุ่มทดลอง Onboarding Wizard
                btnTestWizard = New Button()
                btnTestWizard.Dock = DockStyle.Top
                btnTestWizard.Height = 32
                btnTestWizard.FlatStyle = FlatStyle.Flat
                btnTestWizard.FlatAppearance.BorderSize = 0
                btnTestWizard.BackColor = Color.FromArgb(55, 65, 81)
                btnTestWizard.ForeColor = Color.FromArgb(253, 224, 71)
                btnTestWizard.Font = New Font("Tahoma", 9.5!, FontStyle.Bold)
                btnTestWizard.Text = "🧪 ทดลอง Onboarding"
                btnTestWizard.TextAlign = ContentAlignment.MiddleLeft
                btnTestWizard.Padding = New Padding(10, 0, 0, 0)
                AddHandler btnTestWizard.Click, AddressOf BtnTestWizard_Click

                ' ปุ่มรีเซ็ตสถานะ Onboarding
                btnResetOnboarding = New Button()
                btnResetOnboarding.Dock = DockStyle.Top
                btnResetOnboarding.Height = 32
                btnResetOnboarding.FlatStyle = FlatStyle.Flat
                btnResetOnboarding.FlatAppearance.BorderSize = 0
                btnResetOnboarding.BackColor = Color.FromArgb(55, 65, 81)
                btnResetOnboarding.ForeColor = Color.FromArgb(252, 165, 165)
                btnResetOnboarding.Font = New Font("Tahoma", 9.5!)
                btnResetOnboarding.Text = "🔄 รีเซ็ต Onboarding"
                btnResetOnboarding.TextAlign = ContentAlignment.MiddleLeft
                btnResetOnboarding.Padding = New Padding(10, 0, 0, 0)
                AddHandler btnResetOnboarding.Click, AddressOf BtnResetOnboarding_Click

                ' เพิ่มปุ่มลงใน Panel (เพิ่ม btnReset ก่อนเพื่อให้แสดงด้านบน)
                pnlDev.Controls.Add(btnResetOnboarding)
                pnlDev.Controls.Add(btnTestWizard)

                ' เพิ่ม Panel ลงใน Sidebar (Insert ก่อน spacer)
                ' ใช้ index = pnlSidebar.Controls.IndexOf(pnlSidebarSpacer) เพื่อแทรกก่อน spacer
                Dim spacerIdx As Integer = pnlSidebar.Controls.IndexOf(pnlSidebarSpacer)
                If spacerIdx >= 0 Then
                    pnlSidebar.Controls.Add(pnlDev) ' Controls.Add จะวางท้ายสุด แต่ Dock.Bottom จะจัดเรียงตาม index
                Else
                    pnlSidebar.Controls.Add(pnlDev)
                End If
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("frmMain.SetupDeveloperTools", ex)
            End Try
        End Sub

        ''' <summary>เปิด Onboarding Wizard ในโหมดทดลอง</summary>
        Private Sub BtnTestWizard_Click(sender As Object, e As EventArgs)
            Dim result = MessageBox.Show(
                "🧪 ต้องการทดลอง Onboarding Wizard หรือไม่?" & vbCrLf & vbCrLf &
                "ข้อมูลจะไม่ถูกบันทึกจริง (Dry-run mode)",
                "ยืนยันการทดลอง",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                Using wizard As New FrmOnboardingWizard(isTestMode:=True)
                    wizard.ShowDialog(Me)
                End Using
            End If
        End Sub

        ''' <summary>รีเซ็ตสถานะ Onboarding (สำหรับนักพัฒนา)</summary>
        Private Sub BtnResetOnboarding_Click(sender As Object, e As EventArgs)
            Dim result = MessageBox.Show(
                "🔄 ต้องการรีเซ็ตสถานะ Onboarding หรือไม่?" & vbCrLf & vbCrLf &
                "โปรแกรมจะแสดง Onboarding Wizard อีกครั้งในครั้งถัดไปที่เปิดโปรแกรม",
                "ยืนยันการรีเซ็ต",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2)
            If result = DialogResult.Yes Then
                OnboardingRepository.ResetOnboardingStatus()
                MessageBox.Show("✅ รีเซ็ตสถานะ Onboarding เรียบร้อย!" & vbCrLf &
                                "โปรแกรมจะแสดง Wizard ในครั้งถัดไปที่เปิดโปรแกรม",
                                "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Sub

        Private Sub SetupEventHandlers()
            ' เรียก SetupDeveloperTools เพื่อสร้างปุ่มเครื่องมือนักพัฒนา
            SetupDeveloperTools()

            AddHandler btnDashboard.Click, AddressOf NavMenu_Click
            AddHandler btnDonation.Click, AddressOf NavMenu_Click
            AddHandler btnExpense.Click, AddressOf NavMenu_Click
            AddHandler btnReport.Click, AddressOf NavMenu_Click
            AddHandler btnMember.Click, AddressOf NavMenu_Click
            AddHandler btnVip.Click, AddressOf NavMenu_Click
            AddHandler btnActivity.Click, AddressOf NavMenu_Click
            AddHandler btnMultiImport.Click, AddressOf NavMenu_Click
            AddHandler btnExportTransfer.Click, AddressOf NavMenu_Click
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
            AddHandler picLogo.MouseClick, AddressOf Logo_TripleClickDetect

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
                ilIcons.Images.Add("exporttransfer", MakeIconBitmap("📤", Color.FromArgb(37, 99, 235)))
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
                ApplyButtonImage(btnExportTransfer, "exporttransfer")
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
            If btnTestWizard IsNot Nothing Then ttMain.SetToolTip(btnTestWizard, "เปิด Onboarding Wizard ในโหมดทดลอง (ไม่บันทึกข้อมูลจริง)")
            If btnResetOnboarding IsNot Nothing Then ttMain.SetToolTip(btnResetOnboarding, "รีเซ็ตสถานะ Onboarding ให้แสดง Wizard อีกครั้ง")
            ttMain.SetToolTip(btnDashboard, "กลับไปที่หน้าสรุปภาพรวมของระบบ")
            ttMain.SetToolTip(btnDonation, "บันทึกข้อมูลรายรับหรือเงินบริจาคเข้าวัด")
            ttMain.SetToolTip(btnExpense, "บันทึกข้อมูลรายจ่ายต่างๆ ของวัด")
            ttMain.SetToolTip(btnReport, "พิมพ์รายงานสรุปรายรับ-รายจ่าย (ย่อ/ละเอียด)")
            ttMain.SetToolTip(btnMember, "จัดการข้อมูลรายชื่อผู้บริจาค/สมาชิก")
            ttMain.SetToolTip(btnVip, "จัดการข้อมูลรายชื่อพระสงฆ์และไวยาวัจกร")
            ttMain.SetToolTip(btnActivity, "บันทึกข้อมูลกิจกรรมงานบุญและเทศกาล")
            ttMain.SetToolTip(btnMultiImport, "นำเข้าและรวมข้อมูลธุรกรรมจากหลายเครื่อง/หลายแฟลชไดรฟ์ เข้าสู่ฐานข้อมูลหลัก")
            ttMain.SetToolTip(btnExportTransfer, "ส่งออกข้อมูลธุรกรรมของเครื่องนี้เป็นไฟล์ .xlsx สำหรับโอนไปเครื่องอื่น (แฟลชไดรฟ์)")
            ttMain.SetToolTip(btnSetting, "ตั้งค่าข้อมูลวัดและข้อมูลพื้นฐานของระบบ")
            ttMain.SetToolTip(btnBackup, "สำรองข้อมูลฐานข้อมูล")
            ttMain.SetToolTip(btnRestore, "คืนค่าข้อมูลจากไฟล์สำรอง")
            ttMain.SetToolTip(btnLogout, "ออกจากระบบและกลับไปหน้าเข้าสู่ระบบ")
            ttMain.SetToolTip(btnClose, "ปิดโปรแกรม")
            ttMain.SetToolTip(btnMinimize, "ย่อหน้าต่างโปรแกรมลง")
        End Sub
    End Class
End Namespace
