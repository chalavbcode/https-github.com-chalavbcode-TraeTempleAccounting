Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    Partial Class frmMain
        Private components As System.ComponentModel.IContainer = Nothing

        Private pnlHeader As Panel
        Private lblTitle As Label
        Private lblSubtitle As Label
        Private pnlLogo As Panel
        Private pnlSidebar As Panel
        Friend btnDashboard As Button
        Friend btnDonation As Button
        Friend btnExpense As Button
        Friend btnReport As Button
        Friend btnMember As Button
        Friend btnSetting As Button
        Friend btnBackup As Button
        Friend btnRestore As Button
        Friend btnVip As Button
        Friend btnActivity As Button
        Friend btnMultiImport As Button
        Private pnlSidebarSpacer As Panel
        Private pnlContent As Panel
        Private pnlOverview As Panel
        Private pnlFormHostBody As Panel
        Private lblOverviewTitle As Label
        Private pnlCards As Panel
        Private pnlCard1 As Panel
        Private lblCard1Title As Label
        Private lblCard1Value As Label
        Private pnlCard2 As Panel
        Private lblCard2Title As Label
        Private lblCard2Value As Label
        Private pnlCard3 As Panel
        Private lblCard3Title As Label
        Private lblCard3Value As Label
        Private pnlCard4 As Panel
        Private lblCard4Title As Label
        Private lblCard4Value As Label
        Private pnlFormHost As Panel
        Private pnlFormHostHeader As Panel
        Private lblFormHostTitle As Label
        Private lblFormHostHint As Label
        Private pnlStatus As Panel
        Private lblStatusLeft As Label
        Private lblStatusCenter As Label
        Private lblStatusRight As Label
        Private btnLogout As Button
        Private picLogo As PictureBox
        Private btnClose As Button
        Private btnMinimize As Button
        Private pnlHeaderRight As Panel
        Private lblUserInfo As Label
        Private pnlSeparator1 As Panel
        Private pnlSeparator2 As Panel
        Private ilIcons As ImageList
        Private picLogoBadge As PictureBox
        Private ttMain As ToolTip

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            components = New Container()
            ttMain = New ToolTip(components)
            ilIcons = New ImageList(components)
            pnlHeader = New Panel()
            pnlHeaderRight = New Panel()
            btnClose = New Button()
            btnMinimize = New Button()
            lblUserInfo = New Label()
            pnlLogo = New Panel()
            picLogoBadge = New PictureBox()
            picLogo = New PictureBox()
            lblSubtitle = New Label()
            lblTitle = New Label()
            pnlSidebar = New Panel()
            btnLogout = New Button()
            pnlSidebarSpacer = New Panel()
            btnSetting = New Button()
            btnBackup = New Button()
            btnRestore = New Button()
            btnVip = New Button()
            btnActivity = New Button()
            btnMultiImport = New Button()
            btnReport = New Button()
            btnMember = New Button()
            btnExpense = New Button()
            btnDonation = New Button()
            btnDashboard = New Button()
            pnlContent = New Panel()
            pnlFormHost = New Panel()
            pnlFormHostBody = New Panel()
            pnlFormHostHeader = New Panel()
            pnlSeparator2 = New Panel()
            lblFormHostHint = New Label()
            lblFormHostTitle = New Label()
            pnlOverview = New Panel()
            pnlCards = New Panel()
            pnlCard4 = New Panel()
            lblCard4Value = New Label()
            lblCard4Title = New Label()
            pnlCard3 = New Panel()
            lblCard3Value = New Label()
            lblCard3Title = New Label()
            pnlCard2 = New Panel()
            lblCard2Value = New Label()
            lblCard2Title = New Label()
            pnlCard1 = New Panel()
            lblCard1Value = New Label()
            lblCard1Title = New Label()
            lblOverviewTitle = New Label()
            pnlStatus = New Panel()
            lblStatusRight = New Label()
            lblStatusCenter = New Label()
            lblStatusLeft = New Label()
            pnlSeparator1 = New Panel()
            pnlHeader.SuspendLayout()
            pnlHeaderRight.SuspendLayout()
            pnlLogo.SuspendLayout()
            CType(picLogoBadge, ISupportInitialize).BeginInit()
            CType(picLogo, ISupportInitialize).BeginInit()
            pnlSidebar.SuspendLayout()
            pnlContent.SuspendLayout()
            pnlFormHost.SuspendLayout()
            pnlFormHostHeader.SuspendLayout()
            pnlOverview.SuspendLayout()
            pnlCards.SuspendLayout()
            pnlCard4.SuspendLayout()
            pnlCard3.SuspendLayout()
            pnlCard2.SuspendLayout()
            pnlCard1.SuspendLayout()
            pnlStatus.SuspendLayout()
            SuspendLayout()
            ' 
            ' ilIcons
            ' 
            ilIcons.ColorDepth = ColorDepth.Depth32Bit
            ilIcons.ImageSize = New Size(22, 22)
            ilIcons.TransparentColor = Color.Transparent
            ' 
            ' pnlHeader
            ' 
            pnlHeader.BackColor = Color.FromArgb(CByte(120), CByte(53), CByte(15))
            pnlHeader.Controls.Add(pnlHeaderRight)
            pnlHeader.Controls.Add(pnlLogo)
            pnlHeader.Controls.Add(lblSubtitle)
            pnlHeader.Controls.Add(lblTitle)
            pnlHeader.Dock = DockStyle.Top
            pnlHeader.Location = New Point(0, 0)
            pnlHeader.Name = "pnlHeader"
            pnlHeader.Size = New Size(1440, 84)
            pnlHeader.TabIndex = 0
            ' 
            ' pnlHeaderRight
            ' 
            pnlHeaderRight.Controls.Add(btnClose)
            pnlHeaderRight.Controls.Add(btnMinimize)
            pnlHeaderRight.Controls.Add(lblUserInfo)
            pnlHeaderRight.Dock = DockStyle.Right
            pnlHeaderRight.Location = New Point(1070, 0)
            pnlHeaderRight.Name = "pnlHeaderRight"
            pnlHeaderRight.Size = New Size(370, 84)
            pnlHeaderRight.TabIndex = 4
            ' 
            ' btnClose
            ' 
            btnClose.BackColor = Color.Transparent
            btnClose.FlatAppearance.BorderSize = 0
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(200), CByte(30), CByte(30))
            btnClose.FlatStyle = FlatStyle.Flat
            btnClose.Font = New Font("Segoe UI Symbol", 15F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
            btnClose.ForeColor = Color.FromArgb(CByte(255), CByte(248), CByte(220))
            btnClose.Location = New Point(328, 0)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(42, 42)
            btnClose.TabIndex = 2
            btnClose.Text = "✕"
            btnClose.UseVisualStyleBackColor = False
            ' 
            ' btnMinimize
            ' 
            btnMinimize.BackColor = Color.Transparent
            btnMinimize.FlatAppearance.BorderSize = 0
            btnMinimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(160), CByte(82), CByte(45))
            btnMinimize.FlatStyle = FlatStyle.Flat
            btnMinimize.Font = New Font("Segoe UI Symbol", 15F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
            btnMinimize.ForeColor = Color.FromArgb(CByte(255), CByte(248), CByte(220))
            btnMinimize.Location = New Point(284, 0)
            btnMinimize.Name = "btnMinimize"
            btnMinimize.Size = New Size(42, 42)
            btnMinimize.TabIndex = 1
            btnMinimize.Text = "─"
            btnMinimize.UseVisualStyleBackColor = False
            ' 
            ' lblUserInfo
            ' 
            lblUserInfo.Dock = DockStyle.Bottom
            lblUserInfo.Font = New Font("Tahoma", 10.5F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblUserInfo.ForeColor = Color.FromArgb(CByte(255), CByte(235), CByte(150))
            lblUserInfo.Location = New Point(0, 48)
            lblUserInfo.Name = "lblUserInfo"
            lblUserInfo.Padding = New Padding(0, 0, 60, 0)
            lblUserInfo.Size = New Size(370, 36)
            lblUserInfo.TabIndex = 0
            lblUserInfo.Text = "👤 ผู้ดูแลระบบ"
            lblUserInfo.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' pnlLogo
            ' 
            pnlLogo.Controls.Add(picLogoBadge)
            pnlLogo.Controls.Add(picLogo)
            pnlLogo.Location = New Point(22, 12)
            pnlLogo.Name = "pnlLogo"
            pnlLogo.Size = New Size(60, 60)
            pnlLogo.TabIndex = 3
            ' 
            ' picLogoBadge
            ' 
            picLogoBadge.BackColor = Color.FromArgb(CByte(255), CByte(248), CByte(220))
            picLogoBadge.Location = New Point(2, 2)
            picLogoBadge.Name = "picLogoBadge"
            picLogoBadge.Size = New Size(56, 56)
            picLogoBadge.SizeMode = PictureBoxSizeMode.CenterImage
            picLogoBadge.TabIndex = 1
            picLogoBadge.TabStop = False
            ' 
            ' picLogo
            ' 
            picLogo.BackColor = Color.FromArgb(CByte(234), CByte(179), CByte(8))
            picLogo.Dock = DockStyle.Fill
            picLogo.Location = New Point(0, 0)
            picLogo.Name = "picLogo"
            picLogo.Size = New Size(60, 60)
            picLogo.SizeMode = PictureBoxSizeMode.CenterImage
            picLogo.TabIndex = 0
            picLogo.TabStop = False
            ' 
            ' lblSubtitle
            ' 
            lblSubtitle.AutoSize = True
            lblSubtitle.Font = New Font("Tahoma", 10.5F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            lblSubtitle.ForeColor = Color.FromArgb(CByte(255), CByte(230), CByte(150))
            lblSubtitle.Location = New Point(96, 52)
            lblSubtitle.Name = "lblSubtitle"
            lblSubtitle.Size = New Size(414, 25)
            lblSubtitle.TabIndex = 2
            lblSubtitle.Text = "ระบบบัญชีวัด - Temple Accounting Software"
            ' 
            ' lblTitle
            ' 
            lblTitle.AutoSize = True
            lblTitle.Font = New Font("Tahoma", 20F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblTitle.ForeColor = Color.FromArgb(CByte(255), CByte(215), CByte(0))
            lblTitle.Location = New Point(94, 8)
            lblTitle.Name = "lblTitle"
            lblTitle.Size = New Size(329, 48)
            lblTitle.TabIndex = 1
            lblTitle.Text = "📿 ระบบบัญชีวัดฯ"
            ' 
            ' pnlSidebar
            ' 
            pnlSidebar.BackColor = Color.FromArgb(CByte(88), CByte(40), CByte(12))
            pnlSidebar.Controls.Add(btnLogout)
            pnlSidebar.Controls.Add(pnlSidebarSpacer)
            pnlSidebar.Controls.Add(btnSetting)
            pnlSidebar.Controls.Add(btnBackup)
            pnlSidebar.Controls.Add(btnRestore)
            pnlSidebar.Controls.Add(btnVip)
            pnlSidebar.Controls.Add(btnActivity)
            pnlSidebar.Controls.Add(btnMultiImport)
            pnlSidebar.Controls.Add(btnReport)
            pnlSidebar.Controls.Add(btnMember)
            pnlSidebar.Controls.Add(btnExpense)
            pnlSidebar.Controls.Add(btnDonation)
            pnlSidebar.Controls.Add(btnDashboard)
            pnlSidebar.Dock = DockStyle.Left
            pnlSidebar.Location = New Point(0, 84)
            pnlSidebar.Name = "pnlSidebar"
            pnlSidebar.Padding = New Padding(12, 16, 12, 14)
            pnlSidebar.Size = New Size(251, 726)
            pnlSidebar.TabIndex = 1
            ' 
            ' btnLogout
            ' 
            btnLogout.BackColor = Color.FromArgb(CByte(153), CByte(27), CByte(27))
            btnLogout.Cursor = Cursors.Hand
            btnLogout.Dock = DockStyle.Bottom
            btnLogout.FlatAppearance.BorderSize = 0
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(185), CByte(28), CByte(28))
            btnLogout.FlatStyle = FlatStyle.Flat
            btnLogout.Font = New Font("Tahoma", 11.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            btnLogout.ForeColor = Color.White
            btnLogout.ImageAlign = ContentAlignment.MiddleLeft
            btnLogout.Location = New Point(12, 660)
            btnLogout.Name = "btnLogout"
            btnLogout.Padding = New Padding(14, 0, 8, 0)
            btnLogout.Size = New Size(227, 48)
            btnLogout.TabIndex = 10
            btnLogout.Text = "🚪 ออกจากระบบ"
            btnLogout.TextAlign = ContentAlignment.MiddleLeft
            btnLogout.UseVisualStyleBackColor = False
            ' 
            ' pnlSidebarSpacer
            ' 
            pnlSidebarSpacer.Dock = DockStyle.Bottom
            pnlSidebarSpacer.Location = New Point(12, 708)
            pnlSidebarSpacer.Name = "pnlSidebarSpacer"
            pnlSidebarSpacer.Size = New Size(227, 4)
            pnlSidebarSpacer.TabIndex = 9
            ' 
            ' btnSetting
            ' 
            btnSetting.BackColor = Color.Transparent
            btnSetting.Cursor = Cursors.Hand
            btnSetting.Dock = DockStyle.Top
            btnSetting.FlatAppearance.BorderSize = 0
            btnSetting.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnSetting.FlatStyle = FlatStyle.Flat
            btnSetting.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnSetting.ForeColor = Color.White
            btnSetting.ImageAlign = ContentAlignment.MiddleLeft
            btnSetting.Location = New Point(12, 601)
            btnSetting.Name = "btnSetting"
            btnSetting.Padding = New Padding(14, 0, 8, 0)
            btnSetting.Size = New Size(227, 62)
            btnSetting.TabIndex = 8
            btnSetting.Text = "⚙️ ตั้งค่าระบบ"
            btnSetting.TextAlign = ContentAlignment.MiddleLeft
            btnSetting.UseVisualStyleBackColor = False
            ' 
            ' btnBackup
            ' 
            btnBackup.BackColor = Color.Transparent
            btnBackup.Cursor = Cursors.Hand
            btnBackup.Dock = DockStyle.Top
            btnBackup.FlatAppearance.BorderSize = 0
            btnBackup.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnBackup.FlatStyle = FlatStyle.Flat
            btnBackup.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnBackup.ForeColor = Color.White
            btnBackup.ImageAlign = ContentAlignment.MiddleLeft
            btnBackup.Location = New Point(12, 539)
            btnBackup.Name = "btnBackup"
            btnBackup.Padding = New Padding(14, 0, 8, 0)
            btnBackup.Size = New Size(227, 62)
            btnBackup.TabIndex = 9
            btnBackup.Text = "💾 สำรองข้อมูล"
            btnBackup.TextAlign = ContentAlignment.MiddleLeft
            btnBackup.UseVisualStyleBackColor = False
            ' 
            ' btnRestore
            ' 
            btnRestore.BackColor = Color.Transparent
            btnRestore.Cursor = Cursors.Hand
            btnRestore.Dock = DockStyle.Top
            btnRestore.FlatAppearance.BorderSize = 0
            btnRestore.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnRestore.FlatStyle = FlatStyle.Flat
            btnRestore.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnRestore.ForeColor = Color.White
            btnRestore.ImageAlign = ContentAlignment.MiddleLeft
            btnRestore.Location = New Point(12, 477)
            btnRestore.Name = "btnRestore"
            btnRestore.Padding = New Padding(14, 0, 8, 0)
            btnRestore.Size = New Size(227, 62)
            btnRestore.TabIndex = 10
            btnRestore.Text = "🔄 คืนค่าข้อมูล"
            btnRestore.TextAlign = ContentAlignment.MiddleLeft
            btnRestore.UseVisualStyleBackColor = False
            ' 
            ' btnVip
            ' 
            btnVip.BackColor = Color.Transparent
            btnVip.Cursor = Cursors.Hand
            btnVip.Dock = DockStyle.Top
            btnVip.FlatAppearance.BorderSize = 0
            btnVip.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnVip.FlatStyle = FlatStyle.Flat
            btnVip.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnVip.ForeColor = Color.White
            btnVip.ImageAlign = ContentAlignment.MiddleLeft
            btnVip.Location = New Point(12, 403)
            btnVip.Name = "btnVip"
            btnVip.Padding = New Padding(14, 0, 8, 0)
            btnVip.Size = New Size(227, 74)
            btnVip.TabIndex = 6
            btnVip.Text = ChrW(55358) & ChrW(56647) & " พระ / อาวาส"
            btnVip.TextAlign = ContentAlignment.MiddleLeft
            btnVip.UseVisualStyleBackColor = False
            ' 
            ' btnActivity
            ' 
            btnActivity.BackColor = Color.Transparent
            btnActivity.Cursor = Cursors.Hand
            btnActivity.Dock = DockStyle.Top
            btnActivity.FlatAppearance.BorderSize = 0
            btnActivity.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnActivity.FlatStyle = FlatStyle.Flat
            btnActivity.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnActivity.ForeColor = Color.White
            btnActivity.ImageAlign = ContentAlignment.MiddleLeft
            btnActivity.Location = New Point(12, 339)
            btnActivity.Name = "btnActivity"
            btnActivity.Padding = New Padding(14, 0, 8, 0)
            btnActivity.Size = New Size(227, 64)
            btnActivity.TabIndex = 7
            btnActivity.Text = "🎎 โอนเงินภายใน"
            btnActivity.TextAlign = ContentAlignment.MiddleLeft
            btnActivity.UseVisualStyleBackColor = False
            ' 
            ' btnReport
            ' 
            btnReport.BackColor = Color.Transparent
            btnReport.Cursor = Cursors.Hand
            btnReport.Dock = DockStyle.Top
            btnReport.FlatAppearance.BorderSize = 0
            btnReport.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnReport.FlatStyle = FlatStyle.Flat
            btnReport.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnReport.ForeColor = Color.White
            btnReport.ImageAlign = ContentAlignment.MiddleLeft
            btnReport.Location = New Point(12, 275)
            btnReport.Name = "btnReport"
            btnReport.Padding = New Padding(14, 0, 8, 0)
            btnReport.Size = New Size(227, 64)
            btnReport.TabIndex = 4
            btnReport.Text = "🖨️ พิมพ์รายงาน"
            btnReport.TextAlign = ContentAlignment.MiddleLeft
            btnReport.UseVisualStyleBackColor = False
            ' 
            ' btnMultiImport
            ' 
            btnMultiImport.BackColor = Color.Transparent
            btnMultiImport.Cursor = Cursors.Hand
            btnMultiImport.Dock = DockStyle.Top
            btnMultiImport.FlatAppearance.BorderSize = 0
            btnMultiImport.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnMultiImport.FlatStyle = FlatStyle.Flat
            btnMultiImport.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnMultiImport.ForeColor = Color.White
            btnMultiImport.ImageAlign = ContentAlignment.MiddleLeft
            btnMultiImport.Location = New Point(12, 307)
            btnMultiImport.Name = "btnMultiImport"
            btnMultiImport.Padding = New Padding(14, 0, 8, 0)
            btnMultiImport.Size = New Size(227, 64)
            btnMultiImport.TabIndex = 20
            btnMultiImport.Text = "🖥️ นำเข้าหลายเครื่อง"
            btnMultiImport.TextAlign = ContentAlignment.MiddleLeft
            btnMultiImport.UseVisualStyleBackColor = False
            ' 
            ' btnMember
            ' 
            btnMember.BackColor = Color.Transparent
            btnMember.Cursor = Cursors.Hand
            btnMember.Dock = DockStyle.Top
            btnMember.FlatAppearance.BorderSize = 0
            btnMember.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnMember.FlatStyle = FlatStyle.Flat
            btnMember.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnMember.ForeColor = Color.White
            btnMember.ImageAlign = ContentAlignment.MiddleLeft
            btnMember.Location = New Point(12, 212)
            btnMember.Name = "btnMember"
            btnMember.Padding = New Padding(14, 0, 8, 0)
            btnMember.Size = New Size(227, 63)
            btnMember.TabIndex = 5
            btnMember.Text = "📖 รายการทางบัญชี"
            btnMember.TextAlign = ContentAlignment.MiddleLeft
            btnMember.UseVisualStyleBackColor = False
            ' 
            ' btnExpense
            ' 
            btnExpense.BackColor = Color.Transparent
            btnExpense.Cursor = Cursors.Hand
            btnExpense.Dock = DockStyle.Top
            btnExpense.FlatAppearance.BorderSize = 0
            btnExpense.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnExpense.FlatStyle = FlatStyle.Flat
            btnExpense.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnExpense.ForeColor = Color.White
            btnExpense.ImageAlign = ContentAlignment.MiddleLeft
            btnExpense.Location = New Point(12, 141)
            btnExpense.Name = "btnExpense"
            btnExpense.Padding = New Padding(14, 0, 8, 0)
            btnExpense.Size = New Size(227, 71)
            btnExpense.TabIndex = 3
            btnExpense.Text = "💸 บันทึกจ่ายเงิน"
            btnExpense.TextAlign = ContentAlignment.MiddleLeft
            btnExpense.UseVisualStyleBackColor = False
            ' 
            ' btnDonation
            ' 
            btnDonation.BackColor = Color.Transparent
            btnDonation.Cursor = Cursors.Hand
            btnDonation.Dock = DockStyle.Top
            btnDonation.FlatAppearance.BorderSize = 0
            btnDonation.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(146), CByte(64), CByte(14))
            btnDonation.FlatStyle = FlatStyle.Flat
            btnDonation.Font = New Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            btnDonation.ForeColor = Color.White
            btnDonation.ImageAlign = ContentAlignment.MiddleLeft
            btnDonation.Location = New Point(12, 70)
            btnDonation.Name = "btnDonation"
            btnDonation.Padding = New Padding(14, 0, 8, 0)
            btnDonation.Size = New Size(227, 71)
            btnDonation.TabIndex = 2
            btnDonation.Text = "💰 บันทึกรับเงิน"
            btnDonation.TextAlign = ContentAlignment.MiddleLeft
            btnDonation.UseVisualStyleBackColor = False
            ' 
            ' btnDashboard
            ' 
            btnDashboard.BackColor = Color.FromArgb(CByte(234), CByte(179), CByte(8))
            btnDashboard.Cursor = Cursors.Hand
            btnDashboard.Dock = DockStyle.Top
            btnDashboard.FlatAppearance.BorderSize = 0
            btnDashboard.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(250), CByte(204), CByte(21))
            btnDashboard.FlatStyle = FlatStyle.Flat
            btnDashboard.Font = New Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            btnDashboard.ForeColor = Color.FromArgb(CByte(69), CByte(26), CByte(3))
            btnDashboard.ImageAlign = ContentAlignment.MiddleLeft
            btnDashboard.Location = New Point(12, 16)
            btnDashboard.Name = "btnDashboard"
            btnDashboard.Padding = New Padding(14, 0, 8, 0)
            btnDashboard.Size = New Size(227, 54)
            btnDashboard.TabIndex = 1
            btnDashboard.Text = "🏠 หน้าหลัก"
            btnDashboard.TextAlign = ContentAlignment.MiddleLeft
            btnDashboard.UseVisualStyleBackColor = False
            ' 
            ' pnlContent
            ' 
            pnlContent.AutoScroll = True
            pnlContent.BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            pnlContent.Controls.Add(pnlFormHost)
            pnlContent.Controls.Add(pnlOverview)
            pnlContent.Dock = DockStyle.Fill
            pnlContent.Location = New Point(251, 88)
            pnlContent.Name = "pnlContent"
            pnlContent.Padding = New Padding(16, 16, 16, 12)
            pnlContent.Size = New Size(1189, 722)
            pnlContent.TabIndex = 2
            ' 
            ' pnlFormHost
            ' 
            pnlFormHost.BackColor = Color.FromArgb(CByte(255), CByte(253), CByte(244))
            pnlFormHost.Controls.Add(pnlFormHostBody)
            pnlFormHost.Controls.Add(pnlFormHostHeader)
            pnlFormHost.Dock = DockStyle.Fill
            pnlFormHost.Location = New Point(16, 137)
            pnlFormHost.Name = "pnlFormHost"
            pnlFormHost.Size = New Size(1157, 573)
            pnlFormHost.TabIndex = 2
            ' 
            ' pnlFormHostBody
            ' 
            pnlFormHostBody.BackColor = Color.FromArgb(CByte(255), CByte(253), CByte(244))
            pnlFormHostBody.Dock = DockStyle.Fill
            pnlFormHostBody.Location = New Point(0, 84)
            pnlFormHostBody.Name = "pnlFormHostBody"
            pnlFormHostBody.Size = New Size(1157, 489)
            pnlFormHostBody.TabIndex = 1
            ' 
            ' pnlFormHostHeader
            ' 
            pnlFormHostHeader.BackColor = Color.FromArgb(CByte(250), CByte(240), CByte(210))
            pnlFormHostHeader.Controls.Add(pnlSeparator2)
            pnlFormHostHeader.Controls.Add(lblFormHostHint)
            pnlFormHostHeader.Controls.Add(lblFormHostTitle)
            pnlFormHostHeader.Dock = DockStyle.Top
            pnlFormHostHeader.Location = New Point(0, 0)
            pnlFormHostHeader.Name = "pnlFormHostHeader"
            pnlFormHostHeader.Padding = New Padding(20, 12, 20, 12)
            pnlFormHostHeader.Size = New Size(1157, 84)
            pnlFormHostHeader.TabIndex = 0
            ' 
            ' pnlSeparator2
            ' 
            pnlSeparator2.BackColor = Color.FromArgb(CByte(217), CByte(119), CByte(6))
            pnlSeparator2.Dock = DockStyle.Bottom
            pnlSeparator2.Location = New Point(20, 71)
            pnlSeparator2.Name = "pnlSeparator2"
            pnlSeparator2.Size = New Size(1117, 1)
            pnlSeparator2.TabIndex = 2
            ' 
            ' lblFormHostHint
            ' 
            lblFormHostHint.Dock = DockStyle.Top
            lblFormHostHint.Font = New Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            lblFormHostHint.ForeColor = Color.FromArgb(CByte(120), CByte(80), CByte(40))
            lblFormHostHint.Location = New Point(20, 12)
            lblFormHostHint.Name = "lblFormHostHint"
            lblFormHostHint.Padding = New Padding(0, 4, 0, 0)
            lblFormHostHint.Size = New Size(1117, 62)
            lblFormHostHint.TabIndex = 1
            lblFormHostHint.Text = "📌 เลือกเมนูทางด้านซ้ายเพื่อเปิดหน้าจองานต่างๆ" & vbCrLf & "ระบบจะแสดงฟอร์มงานที่นี่ โดยขนาดฟอนต์และปุ่มจะถูกปรับขนาดตามนี้ทั้งหมด"
            ' 
            ' lblFormHostTitle
            ' 
            lblFormHostTitle.AutoSize = True
            lblFormHostTitle.Font = New Font("Tahoma", 14.5F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblFormHostTitle.ForeColor = Color.FromArgb(CByte(120), CByte(53), CByte(15))
            lblFormHostTitle.Location = New Point(20, 12)
            lblFormHostTitle.Name = "lblFormHostTitle"
            lblFormHostTitle.Size = New Size(193, 35)
            lblFormHostTitle.TabIndex = 0
            lblFormHostTitle.Text = ChrW(55358) & ChrW(56991) & " พื้นที่ทำงาน"
            ' 
            ' pnlOverview
            ' 
            pnlOverview.BackColor = Color.Transparent
            pnlOverview.Controls.Add(pnlCards)
            pnlOverview.Controls.Add(lblOverviewTitle)
            pnlOverview.Dock = DockStyle.Top
            pnlOverview.Location = New Point(16, 16)
            pnlOverview.Name = "pnlOverview"
            pnlOverview.Size = New Size(1157, 121)
            pnlOverview.TabIndex = 1
            ' 
            ' pnlCards
            ' 
            pnlCards.Controls.Add(pnlCard4)
            pnlCards.Controls.Add(pnlCard3)
            pnlCards.Controls.Add(pnlCard2)
            pnlCards.Controls.Add(pnlCard1)
            pnlCards.Dock = DockStyle.Top
            pnlCards.Location = New Point(0, 31)
            pnlCards.Name = "pnlCards"
            pnlCards.Size = New Size(1157, 90)
            pnlCards.TabIndex = 1
            ' 
            ' pnlCard4
            ' 
            pnlCard4.BackColor = Color.White
            pnlCard4.Controls.Add(lblCard4Value)
            pnlCard4.Controls.Add(lblCard4Title)
            pnlCard4.Cursor = Cursors.Hand
            pnlCard4.Dock = DockStyle.Right
            pnlCard4.Location = New Point(413, 0)
            pnlCard4.Name = "pnlCard4"
            pnlCard4.Padding = New Padding(12, 10, 12, 10)
            pnlCard4.Size = New Size(248, 90)
            pnlCard4.TabIndex = 3
            ' 
            ' lblCard4Value
            ' 
            lblCard4Value.BackColor = Color.Transparent
            lblCard4Value.Dock = DockStyle.Top
            lblCard4Value.Font = New Font("Tahoma", 17F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblCard4Value.ForeColor = Color.FromArgb(CByte(154), CByte(52), CByte(18))
            lblCard4Value.Location = New Point(12, 46)
            lblCard4Value.Name = "lblCard4Value"
            lblCard4Value.Size = New Size(224, 44)
            lblCard4Value.TabIndex = 1
            lblCard4Value.Text = "0.00"
            lblCard4Value.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblCard4Title
            ' 
            lblCard4Title.BackColor = Color.Transparent
            lblCard4Title.Dock = DockStyle.Top
            lblCard4Title.Font = New Font("Tahoma", 10F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblCard4Title.ForeColor = Color.FromArgb(CByte(120), CByte(80), CByte(40))
            lblCard4Title.Location = New Point(12, 10)
            lblCard4Title.Name = "lblCard4Title"
            lblCard4Title.Size = New Size(224, 36)
            lblCard4Title.TabIndex = 0
            lblCard4Title.Text = "ยอดคงเหลือปัจจุบัน"
            lblCard4Title.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' pnlCard3
            ' 
            pnlCard3.BackColor = Color.White
            pnlCard3.Controls.Add(lblCard3Value)
            pnlCard3.Controls.Add(lblCard3Title)
            pnlCard3.Cursor = Cursors.Hand
            pnlCard3.Dock = DockStyle.Right
            pnlCard3.Location = New Point(661, 0)
            pnlCard3.Name = "pnlCard3"
            pnlCard3.Padding = New Padding(12, 10, 12, 10)
            pnlCard3.Size = New Size(248, 90)
            pnlCard3.TabIndex = 2
            ' 
            ' lblCard3Value
            ' 
            lblCard3Value.BackColor = Color.Transparent
            lblCard3Value.Dock = DockStyle.Top
            lblCard3Value.Font = New Font("Tahoma", 17F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblCard3Value.ForeColor = Color.FromArgb(CByte(153), CByte(27), CByte(27))
            lblCard3Value.Location = New Point(12, 46)
            lblCard3Value.Name = "lblCard3Value"
            lblCard3Value.Size = New Size(224, 44)
            lblCard3Value.TabIndex = 1
            lblCard3Value.Text = "0.00"
            lblCard3Value.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblCard3Title
            ' 
            lblCard3Title.BackColor = Color.Transparent
            lblCard3Title.Dock = DockStyle.Top
            lblCard3Title.Font = New Font("Tahoma", 10F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblCard3Title.ForeColor = Color.FromArgb(CByte(120), CByte(80), CByte(40))
            lblCard3Title.Location = New Point(12, 10)
            lblCard3Title.Name = "lblCard3Title"
            lblCard3Title.Size = New Size(224, 36)
            lblCard3Title.TabIndex = 0
            lblCard3Title.Text = "ยอดจ่ายเงินเดือนนี้"
            lblCard3Title.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' pnlCard2
            ' 
            pnlCard2.BackColor = Color.White
            pnlCard2.Controls.Add(lblCard2Value)
            pnlCard2.Controls.Add(lblCard2Title)
            pnlCard2.Cursor = Cursors.Hand
            pnlCard2.Dock = DockStyle.Right
            pnlCard2.Location = New Point(909, 0)
            pnlCard2.Name = "pnlCard2"
            pnlCard2.Padding = New Padding(12, 10, 12, 10)
            pnlCard2.Size = New Size(248, 90)
            pnlCard2.TabIndex = 1
            ' 
            ' lblCard2Value
            ' 
            lblCard2Value.BackColor = Color.Transparent
            lblCard2Value.Dock = DockStyle.Top
            lblCard2Value.Font = New Font("Tahoma", 17F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblCard2Value.ForeColor = Color.FromArgb(CByte(22), CByte(101), CByte(52))
            lblCard2Value.Location = New Point(12, 46)
            lblCard2Value.Name = "lblCard2Value"
            lblCard2Value.Size = New Size(224, 44)
            lblCard2Value.TabIndex = 1
            lblCard2Value.Text = "0.00"
            lblCard2Value.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblCard2Title
            ' 
            lblCard2Title.BackColor = Color.Transparent
            lblCard2Title.Dock = DockStyle.Top
            lblCard2Title.Font = New Font("Tahoma", 10F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblCard2Title.ForeColor = Color.FromArgb(CByte(120), CByte(80), CByte(40))
            lblCard2Title.Location = New Point(12, 10)
            lblCard2Title.Name = "lblCard2Title"
            lblCard2Title.Size = New Size(224, 36)
            lblCard2Title.TabIndex = 0
            lblCard2Title.Text = "ยอดรับเงินเดือนนี้"
            lblCard2Title.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' pnlCard1
            ' 
            pnlCard1.BackColor = Color.White
            pnlCard1.Controls.Add(lblCard1Value)
            pnlCard1.Controls.Add(lblCard1Title)
            pnlCard1.Cursor = Cursors.Hand
            pnlCard1.Dock = DockStyle.Left
            pnlCard1.Location = New Point(0, 0)
            pnlCard1.Name = "pnlCard1"
            pnlCard1.Padding = New Padding(12, 10, 12, 10)
            pnlCard1.Size = New Size(248, 90)
            pnlCard1.TabIndex = 0
            ' 
            ' lblCard1Value
            ' 
            lblCard1Value.BackColor = Color.Transparent
            lblCard1Value.Dock = DockStyle.Top
            lblCard1Value.Font = New Font("Tahoma", 17F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblCard1Value.ForeColor = Color.FromArgb(CByte(161), CByte(98), CByte(7))
            lblCard1Value.Location = New Point(12, 46)
            lblCard1Value.Name = "lblCard1Value"
            lblCard1Value.Size = New Size(224, 44)
            lblCard1Value.TabIndex = 1
            lblCard1Value.Text = "0"
            lblCard1Value.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblCard1Title
            ' 
            lblCard1Title.BackColor = Color.Transparent
            lblCard1Title.Dock = DockStyle.Top
            lblCard1Title.Font = New Font("Tahoma", 10F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblCard1Title.ForeColor = Color.FromArgb(CByte(120), CByte(80), CByte(40))
            lblCard1Title.Location = New Point(12, 10)
            lblCard1Title.Name = "lblCard1Title"
            lblCard1Title.Size = New Size(224, 36)
            lblCard1Title.TabIndex = 0
            lblCard1Title.Text = "ผู้บริจาคเดือนนี้"
            lblCard1Title.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblOverviewTitle
            ' 
            lblOverviewTitle.AutoSize = True
            lblOverviewTitle.Dock = DockStyle.Top
            lblOverviewTitle.Font = New Font("Tahoma", 13F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblOverviewTitle.ForeColor = Color.FromArgb(CByte(120), CByte(53), CByte(15))
            lblOverviewTitle.Location = New Point(0, 0)
            lblOverviewTitle.Margin = New Padding(0, 0, 0, 8)
            lblOverviewTitle.Name = "lblOverviewTitle"
            lblOverviewTitle.Size = New Size(200, 31)
            lblOverviewTitle.TabIndex = 0
            lblOverviewTitle.Text = "🏁 ภาพรวมวันนี้"
            ' 
            ' pnlStatus
            ' 
            pnlStatus.BackColor = Color.FromArgb(CByte(88), CByte(40), CByte(12))
            pnlStatus.Controls.Add(lblStatusRight)
            pnlStatus.Controls.Add(lblStatusCenter)
            pnlStatus.Controls.Add(lblStatusLeft)
            pnlStatus.Dock = DockStyle.Bottom
            pnlStatus.Location = New Point(0, 810)
            pnlStatus.Name = "pnlStatus"
            pnlStatus.Padding = New Padding(14, 5, 14, 5)
            pnlStatus.Size = New Size(1440, 30)
            pnlStatus.TabIndex = 3
            ' 
            ' lblStatusRight
            ' 
            lblStatusRight.Dock = DockStyle.Right
            lblStatusRight.Font = New Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(222))
            lblStatusRight.ForeColor = Color.FromArgb(CByte(253), CByte(224), CByte(71))
            lblStatusRight.Location = New Point(1110, 5)
            lblStatusRight.Name = "lblStatusRight"
            lblStatusRight.Size = New Size(316, 20)
            lblStatusRight.TabIndex = 2
            lblStatusRight.Text = "v1.0.0"
            lblStatusRight.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' lblStatusCenter
            ' 
            lblStatusCenter.Dock = DockStyle.Fill
            lblStatusCenter.Font = New Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            lblStatusCenter.ForeColor = Color.FromArgb(CByte(254), CByte(249), CByte(195))
            lblStatusCenter.Location = New Point(14, 5)
            lblStatusCenter.Name = "lblStatusCenter"
            lblStatusCenter.Size = New Size(1412, 20)
            lblStatusCenter.TabIndex = 1
            lblStatusCenter.Text = ChrW(55357) & ChrW(57314) & " สถานะระบบ: ปกติ | ฐานข้อมูล: เชื่อมต่อแล้ว"
            lblStatusCenter.TextAlign = ContentAlignment.MiddleLeft
            ' 
            ' lblStatusLeft
            ' 
            lblStatusLeft.Dock = DockStyle.Left
            lblStatusLeft.Font = New Font("Tahoma", 8.5F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            lblStatusLeft.ForeColor = Color.FromArgb(CByte(88), CByte(40), CByte(12))
            lblStatusLeft.Location = New Point(14, 5)
            lblStatusLeft.Name = "lblStatusLeft"
            lblStatusLeft.Size = New Size(0, 20)
            lblStatusLeft.TabIndex = 0
            ' 
            ' pnlSeparator1
            ' 
            pnlSeparator1.BackColor = Color.FromArgb(CByte(234), CByte(179), CByte(8))
            pnlSeparator1.Dock = DockStyle.Top
            pnlSeparator1.Location = New Point(251, 84)
            pnlSeparator1.Name = "pnlSeparator1"
            pnlSeparator1.Size = New Size(1189, 4)
            pnlSeparator1.TabIndex = 2
            ' 
            ' frmMain
            ' 
            AutoScaleDimensions = New SizeF(12F, 25F)
            AutoScaleMode = AutoScaleMode.Font
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            ClientSize = New Size(1440, 840)
            Controls.Add(pnlContent)
            Controls.Add(pnlSeparator1)
            Controls.Add(pnlSidebar)
            Controls.Add(pnlStatus)
            Controls.Add(pnlHeader)
            Font = New Font("Tahoma", 10.5F, FontStyle.Regular, GraphicsUnit.Point, CByte(222))
            ForeColor = Color.FromArgb(CByte(60), CByte(40), CByte(20))
            FormBorderStyle = FormBorderStyle.None
            MinimumSize = New Size(940, 620)
            Name = "frmMain"
            StartPosition = FormStartPosition.CenterScreen
            Text = "ระบบบัญชีเงินทองวัดฯ"
            WindowState = FormWindowState.Maximized
            pnlHeader.ResumeLayout(False)
            pnlHeader.PerformLayout()
            pnlHeaderRight.ResumeLayout(False)
            pnlLogo.ResumeLayout(False)
            CType(picLogoBadge, ISupportInitialize).EndInit()
            CType(picLogo, ISupportInitialize).EndInit()
            pnlSidebar.ResumeLayout(False)
            pnlContent.ResumeLayout(False)
            pnlFormHost.ResumeLayout(False)
            pnlFormHostHeader.ResumeLayout(False)
            pnlFormHostHeader.PerformLayout()
            pnlOverview.ResumeLayout(False)
            pnlOverview.PerformLayout()
            pnlCards.ResumeLayout(False)
            pnlCard4.ResumeLayout(False)
            pnlCard3.ResumeLayout(False)
            pnlCard2.ResumeLayout(False)
            pnlCard1.ResumeLayout(False)
            pnlStatus.ResumeLayout(False)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace
