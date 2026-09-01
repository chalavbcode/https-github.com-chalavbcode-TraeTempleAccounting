Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmOnboardingWizard
        Inherits Form

        Private components As System.ComponentModel.IContainer = Nothing

#Region "===== Friend Fields ====="

        ' — Root Layout (single child of Form) —
        Friend WithEvents tlpRoot As TableLayoutPanel

        ' — Test Mode Banner (Row 0) —
        Friend WithEvents pnlTestBar As Panel
        Friend WithEvents lblTestBanner As Label
        Friend WithEvents btnCloseTestMode As Button

        ' — Header (Row 1) —
        Friend WithEvents pnlHeader As Panel
        Friend WithEvents tlpHeader As TableLayoutPanel
        Friend WithEvents lblStepTitle As Label
        Friend WithEvents lblStepIndicator As Label
        Friend WithEvents pgbStep As ProgressBar

        ' — Body (Row 2) —
        Friend WithEvents pnlBody As Panel

        ' — Step Panels —
        Friend WithEvents pnlStep1 As Panel
        Friend WithEvents pnlStep2 As Panel
        Friend WithEvents pnlStep3 As Panel
        Friend WithEvents pnlStep4 As Panel
        Friend WithEvents pnlStep5 As Panel

        ' — Step 1: ข้อมูลวัด —
        Friend WithEvents tlpStep1 As TableLayoutPanel
        Friend WithEvents lblTempleName As Label
        Friend WithEvents txtTempleName As TextBox
        Friend WithEvents lblPhone As Label
        Friend WithEvents txtPhone As TextBox
        Friend WithEvents lblTempleAddress As Label
        Friend WithEvents txtTempleAddress As TextBox
        Friend WithEvents lblProvince As Label
        Friend WithEvents cboProvince As ComboBox
        Friend WithEvents lblDistrict As Label
        Friend WithEvents cboDistrict As ComboBox
        Friend WithEvents lblSubDistrict As Label
        Friend WithEvents cboSubDistrict As ComboBox
        Friend WithEvents lblTempleCode As Label
        Friend WithEvents txtTempleCode As TextBox
        Friend WithEvents lblRequiredNote As Label

        ' — Step 2: บุคลากร —
        Friend WithEvents lblStep2Desc As Label
        Friend WithEvents tlpStep2 As TableLayoutPanel
        Friend WithEvents lblAbbot As Label
        Friend WithEvents cboAbbotName As ComboBox
        Friend WithEvents lblWaiyawat As Label
        Friend WithEvents cboWaiyawatName As ComboBox
        Friend WithEvents lblBookkeeper As Label
        Friend WithEvents cboBookkeeperName As ComboBox
        Friend WithEvents dgvPersonnel As DataGridView
        Friend WithEvents lblPersonnelCount As Label

        ' — Step 3: บัญชีธนาคาร —
        Friend WithEvents lblStep3Desc As Label
        Friend WithEvents lblBankHeader As Label
        Friend WithEvents dgvBankAccounts As DataGridView
        Friend WithEvents btnAddBankAccount As Button
        Friend WithEvents grpAddBank As GroupBox
        Friend WithEvents lblNewBankName As Label
        Friend WithEvents txtNewBankName As TextBox
        Friend WithEvents lblNewAccountNo As Label
        Friend WithEvents txtNewAccountNo As TextBox
        Friend WithEvents lblNewAccountName As Label
        Friend WithEvents txtNewAccountName As TextBox
        Friend WithEvents btnSaveNewBank As Button
        Friend WithEvents btnCancelNewBank As Button
        Friend WithEvents lblFundHeader As Label
        Friend WithEvents dgvFunds As DataGridView

        ' — Step 4: ยอดยกมา —
        Friend WithEvents lblStep4Desc As Label
        Friend WithEvents tlpStep4 As TableLayoutPanel
        Friend WithEvents lblBalanceDate As Label
        Friend WithEvents dtpBalanceDate As DateTimePicker
        Friend WithEvents lblCashBalance As Label
        Friend WithEvents txtCashBalance As TextBox
        Friend WithEvents lblBankBalance As Label
        Friend WithEvents txtBankBalance As TextBox
        Friend WithEvents btnSaveBalance As Button

        ' — Step 5: เสร็จสิ้น —
        Friend WithEvents lblCheckmark As Label
        Friend WithEvents lblStep5Subtitle As Label
        Friend WithEvents dgvSummary As DataGridView
        Friend WithEvents btnPrintTestReport As Button
        Friend WithEvents lblCompletionStatus As Label

        ' — Footer (Row 3) —
        Friend WithEvents pnlFooter As Panel
        Friend WithEvents tlpFooter As TableLayoutPanel
        Friend WithEvents lblValidationMessage As Label
        Friend WithEvents btnCancel As Button
        Friend WithEvents btnBack As Button
        Friend WithEvents btnNext As Button

        ' — Helpers —
        Private WithEvents ttValidation As ToolTip
        Private WithEvents errProvider As ErrorProvider

#End Region

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then components.Dispose()
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Me.ttValidation = New ToolTip(Me.components)
            Me.errProvider = New ErrorProvider(Me.components)

            ' ===== สร้างอ็อบเจ็กต์ทั้งหมด =====

            ' — Root Layout —
            Me.tlpRoot = New TableLayoutPanel()

            ' — Test Bar —
            Me.pnlTestBar = New Panel()
            Me.lblTestBanner = New Label()
            Me.btnCloseTestMode = New Button()

            ' — Header —
            Me.pnlHeader = New Panel()
            Me.tlpHeader = New TableLayoutPanel()
            Me.lblStepTitle = New Label()
            Me.lblStepIndicator = New Label()
            Me.pgbStep = New ProgressBar()

            ' — Body —
            Me.pnlBody = New Panel()

            ' — Step panels —
            Me.pnlStep1 = New Panel()
            Me.tlpStep1 = New TableLayoutPanel()
            Me.pnlStep2 = New Panel()
            Me.tlpStep2 = New TableLayoutPanel()
            Me.pnlStep3 = New Panel()
            Me.pnlStep4 = New Panel()
            Me.tlpStep4 = New TableLayoutPanel()
            Me.pnlStep5 = New Panel()

            ' — Step 1 fields —
            Me.lblTempleName = New Label() : Me.txtTempleName = New TextBox()
            Me.lblPhone = New Label() : Me.txtPhone = New TextBox()
            Me.lblTempleAddress = New Label() : Me.txtTempleAddress = New TextBox()
            Me.lblProvince = New Label() : Me.cboProvince = New ComboBox()
            Me.lblDistrict = New Label() : Me.cboDistrict = New ComboBox()
            Me.lblSubDistrict = New Label() : Me.cboSubDistrict = New ComboBox()
            Me.lblTempleCode = New Label() : Me.txtTempleCode = New TextBox()
            Me.lblRequiredNote = New Label()

            ' — Step 2 fields —
            Me.lblStep2Desc = New Label()
            Me.lblAbbot = New Label() : Me.cboAbbotName = New ComboBox()
            Me.lblWaiyawat = New Label() : Me.cboWaiyawatName = New ComboBox()
            Me.lblBookkeeper = New Label() : Me.cboBookkeeperName = New ComboBox()
            Me.dgvPersonnel = New DataGridView()
            Me.lblPersonnelCount = New Label()

            ' — Step 3 fields —
            Me.lblStep3Desc = New Label()
            Me.lblBankHeader = New Label()
            Me.dgvBankAccounts = New DataGridView()
            Me.btnAddBankAccount = New Button()
            Me.grpAddBank = New GroupBox()
            Me.lblNewBankName = New Label() : Me.txtNewBankName = New TextBox()
            Me.lblNewAccountNo = New Label() : Me.txtNewAccountNo = New TextBox()
            Me.lblNewAccountName = New Label() : Me.txtNewAccountName = New TextBox()
            Me.btnSaveNewBank = New Button() : Me.btnCancelNewBank = New Button()
            Me.lblFundHeader = New Label()
            Me.dgvFunds = New DataGridView()

            ' — Step 4 fields —
            Me.lblStep4Desc = New Label()
            Me.lblBalanceDate = New Label() : Me.dtpBalanceDate = New DateTimePicker()
            Me.lblCashBalance = New Label() : Me.txtCashBalance = New TextBox()
            Me.lblBankBalance = New Label() : Me.txtBankBalance = New TextBox()
            Me.btnSaveBalance = New Button()

            ' — Step 5 fields —
            Me.lblCheckmark = New Label()
            Me.lblStep5Subtitle = New Label()
            Me.dgvSummary = New DataGridView()
            Me.btnPrintTestReport = New Button()
            Me.lblCompletionStatus = New Label()

            ' — Footer —
            Me.pnlFooter = New Panel()
            Me.tlpFooter = New TableLayoutPanel()
            Me.lblValidationMessage = New Label()
            Me.btnCancel = New Button()
            Me.btnBack = New Button()
            Me.btnNext = New Button()

            ' ===== SuspendLayout =====
            Me.SuspendLayout()
            Me.tlpRoot.SuspendLayout()
            Me.pnlTestBar.SuspendLayout()
            Me.pnlHeader.SuspendLayout()
            Me.tlpHeader.SuspendLayout()
            Me.pnlBody.SuspendLayout()
            Me.pnlStep1.SuspendLayout() : Me.tlpStep1.SuspendLayout()
            Me.pnlStep2.SuspendLayout() : Me.tlpStep2.SuspendLayout()
            Me.pnlStep3.SuspendLayout()
            Me.pnlStep4.SuspendLayout() : Me.tlpStep4.SuspendLayout()
            Me.pnlStep5.SuspendLayout()
            Me.pnlFooter.SuspendLayout()
            Me.tlpFooter.SuspendLayout()
            CType(Me.dgvPersonnel, ISupportInitialize).BeginInit()
            CType(Me.dgvBankAccounts, ISupportInitialize).BeginInit()
            CType(Me.dgvFunds, ISupportInitialize).BeginInit()
            CType(Me.dgvSummary, ISupportInitialize).BeginInit()
            CType(Me.errProvider, ISupportInitialize).BeginInit()

            ' ============================================================
            '  tlpRoot — Root layout: single child of Form
            '  Row 0: pnlTestBar  (AutoSize, hidden when not in test mode)
            '  Row 1: pnlHeader   (AutoSize, grows with Thai font / DPI)
            '  Row 2: pnlBody     (Percent 100%, scrollable content area)
            '  Row 3: pnlFooter   (AutoSize, buttons + validation message)
            '
            '  AutoSize = False (ห้าม AutoSize ที่ Root — จะดันฟอร์มหด)
            ' ============================================================
            Me.tlpRoot.Dock = DockStyle.Fill
            Me.tlpRoot.AutoSize = False
            Me.tlpRoot.MinimumSize = New Size(960, 660)
            Me.tlpRoot.ColumnCount = 1
            Me.tlpRoot.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0!))
            Me.tlpRoot.RowCount = 4
            Me.tlpRoot.RowStyles.Add(New RowStyle(SizeType.AutoSize))   ' Row 0: TestBar
            Me.tlpRoot.RowStyles.Add(New RowStyle(SizeType.AutoSize))   ' Row 1: Header
            Me.tlpRoot.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!)) ' Row 2: Body
            Me.tlpRoot.RowStyles.Add(New RowStyle(SizeType.AutoSize))   ' Row 3: Footer
            Me.tlpRoot.Margin = New Padding(0)
            Me.tlpRoot.Padding = New Padding(0)

            ' ============================================================
            '  pnlTestBar — แถบสีส้มสำหรับโหมดทดลอง (Row 0)
            ' ============================================================
            Me.pnlTestBar.BackColor = Color.FromArgb(234, 88, 12)
            Me.pnlTestBar.Dock = DockStyle.Fill
            Me.pnlTestBar.Margin = New Padding(0)
            Me.pnlTestBar.Visible = False
            Me.pnlTestBar.AutoSize = True
            Me.pnlTestBar.MinimumSize = New Size(0, 36)

            Me.btnCloseTestMode.BackColor = Color.FromArgb(234, 88, 12)
            Me.btnCloseTestMode.FlatStyle = FlatStyle.Flat
            Me.btnCloseTestMode.Font = New Font("Tahoma", 9.5!, FontStyle.Bold)
            Me.btnCloseTestMode.ForeColor = Color.White
            Me.btnCloseTestMode.Text = WizardText.BtnExitTestMode
            Me.btnCloseTestMode.Dock = DockStyle.Right
            Me.btnCloseTestMode.Size = New Size(170, 36)
            Me.btnCloseTestMode.Visible = False

            Me.lblTestBanner.Dock = DockStyle.Fill
            Me.lblTestBanner.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)
            Me.lblTestBanner.ForeColor = Color.White
            Me.lblTestBanner.Text = WizardText.TestModeBanner
            Me.lblTestBanner.TextAlign = ContentAlignment.MiddleCenter

            ' Add in reverse: btnCloseTestMode (Right) first, then lblTestBanner (Fill)
            Me.pnlTestBar.Controls.Add(Me.btnCloseTestMode)
            Me.pnlTestBar.Controls.Add(Me.lblTestBanner)

            ' ============================================================
            '  pnlHeader — หัวข้อขั้นตอน + Progress (Row 1)
            '  Padding = (20, 10, 20, 10) ให้พื้นที่รอบเนื้อหา
            ' ============================================================
            Me.pnlHeader.BackColor = Color.FromArgb(253, 230, 138)
            Me.pnlHeader.Dock = DockStyle.Fill
            Me.pnlHeader.Margin = New Padding(0)
            Me.pnlHeader.AutoSize = True
            Me.pnlHeader.MinimumSize = New Size(0, 110)
            Me.pnlHeader.Padding = New Padding(20, 12, 20, 12)

            ' — tlpHeader: 2 columns x 2 rows, AutoSize —
            Me.tlpHeader.Dock = DockStyle.Fill
            Me.tlpHeader.AutoSize = True
            Me.tlpHeader.ColumnCount = 2
            Me.tlpHeader.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
            Me.tlpHeader.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
            Me.tlpHeader.RowCount = 2
            Me.tlpHeader.RowStyles.Add(New RowStyle(SizeType.Absolute, 52.0!)) ' Row 0: Title (52px for Thai ascenders/descenders)
            Me.tlpHeader.RowStyles.Add(New RowStyle(SizeType.AutoSize))   ' Row 1: Indicator + Progress
            Me.tlpHeader.Margin = New Padding(0)

            ' — Row 0: lblStepTitle (ColumnSpan=2, 16pt Bold, MiddleCenter) —
            '   AutoSize = False เพื่อไม่ให้ข้อความยาวดันความกว้างฟอร์ม
            Me.lblStepTitle.AutoSize = False
            Me.lblStepTitle.Dock = DockStyle.Fill
            Me.lblStepTitle.Font = New Font("Tahoma", 16.0!, FontStyle.Bold)
            Me.lblStepTitle.ForeColor = Color.FromArgb(69, 26, 3)
            Me.lblStepTitle.Text = WizardText.Step1Title
            Me.lblStepTitle.TextAlign = ContentAlignment.MiddleCenter
            Me.lblStepTitle.Margin = New Padding(0, 0, 0, 6)
            Me.tlpHeader.Controls.Add(Me.lblStepTitle, 0, 0)
            Me.tlpHeader.SetColumnSpan(Me.lblStepTitle, 2)

            ' — Row 1, Col 0: lblStepIndicator (AutoSize, AnchorLeft) —
            Me.lblStepIndicator.AutoSize = True
            Me.lblStepIndicator.Dock = DockStyle.Left
            Me.lblStepIndicator.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)
            Me.lblStepIndicator.ForeColor = Color.FromArgb(69, 26, 3)
            Me.lblStepIndicator.Text = String.Format(WizardText.StepIndicatorFormat, 1, 5)
            Me.lblStepIndicator.TextAlign = ContentAlignment.MiddleLeft
            Me.lblStepIndicator.Padding = New Padding(4, 0, 0, 0)
            Me.tlpHeader.Controls.Add(Me.lblStepIndicator, 0, 1)

            ' — Row 1, Col 1: pgbStep (Dock=Fill, Height=22, Margin Left 20) —
            Me.pgbStep.Dock = DockStyle.Fill
            Me.pgbStep.MinimumSize = New Size(200, 20)
            Me.pgbStep.Minimum = 0
            Me.pgbStep.Maximum = 100
            Me.pgbStep.Value = 20
            Me.pgbStep.Margin = New Padding(20, 6, 0, 6)
            Me.tlpHeader.Controls.Add(Me.pgbStep, 1, 1)

            Me.pnlHeader.Controls.Add(Me.tlpHeader)

            ' ============================================================
            '  pnlBody — พื้นที่แสดง Step Panels ทั้ง 5 (Row 2, Percent 100%)
            '  AutoScroll = True เพื่อรองรับเนื้อหาที่ล้น
            ' ============================================================
            Me.pnlBody.Dock = DockStyle.Fill
            Me.pnlBody.Margin = New Padding(0)
            Me.pnlBody.AutoScroll = True
            Me.pnlBody.Padding = New Padding(0)

            ' ============================================================
            '  STEP 1 — ตั้งค่าข้อมูลวัด
            '  6+ fields: ชื่อวัด, เบอร์โทร, ที่อยู่, จังหวัด, อำเภอ, ตำบล (+ รหัสวัด)
            ' ============================================================
            Me.pnlStep1.Dock = DockStyle.Fill
            Me.pnlStep1.Visible = True
            Me.pnlStep1.BackColor = Color.FromArgb(255, 253, 244)
            Me.pnlStep1.Padding = New Padding(30, 24, 30, 20)
            Me.pnlStep1.AutoScroll = True
            Me.pnlStep1.Margin = New Padding(0)

            ' — tlpStep1: 4 columns —
            '   Col0 = AutoSize (Label ซ้าย) | Col1 = Percent 50% (Input ซ้าย)
            '   Col2 = AutoSize (Label ขวา) | Col3 = Percent 50% (Input ขวา)
            '   เปลี่ยนจาก Absolute 150px เป็น AutoSize เพื่อรองรับ Thai ข้อความยาว
            Me.tlpStep1.Dock = DockStyle.Top
            Me.tlpStep1.AutoSize = True
            Me.tlpStep1.AutoSizeMode = AutoSizeMode.GrowAndShrink
            Me.tlpStep1.ColumnCount = 4
            Me.tlpStep1.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            Me.tlpStep1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
            Me.tlpStep1.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            Me.tlpStep1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
            Me.tlpStep1.RowCount = 5
            Me.tlpStep1.RowStyles.Add(New RowStyle(SizeType.AutoSize)) ' Row 0
            Me.tlpStep1.RowStyles.Add(New RowStyle(SizeType.AutoSize)) ' Row 1
            Me.tlpStep1.RowStyles.Add(New RowStyle(SizeType.AutoSize)) ' Row 2
            Me.tlpStep1.RowStyles.Add(New RowStyle(SizeType.AutoSize)) ' Row 3
            Me.tlpStep1.RowStyles.Add(New RowStyle(SizeType.AutoSize)) ' Row 4
            Me.tlpStep1.Margin = New Padding(0)
            Me.tlpStep1.Padding = New Padding(0)

            ' Row 0: ชื่อวัด * + เบอร์โทร
            Me.tlpStep1.Controls.Add(Me.lblTempleName, 0, 0)
            Me.tlpStep1.Controls.Add(Me.txtTempleName, 1, 0)
            Me.tlpStep1.Controls.Add(Me.lblPhone, 2, 0)
            Me.tlpStep1.Controls.Add(Me.txtPhone, 3, 0)
            ' Row 1: ที่อยู่ * (colspan=3)
            Me.tlpStep1.Controls.Add(Me.lblTempleAddress, 0, 1)
            Me.tlpStep1.Controls.Add(Me.txtTempleAddress, 1, 1)
            Me.tlpStep1.SetColumnSpan(Me.txtTempleAddress, 3)
            ' Row 2: จังหวัด * + อำเภอ *
            Me.tlpStep1.Controls.Add(Me.lblProvince, 0, 2)
            Me.tlpStep1.Controls.Add(Me.cboProvince, 1, 2)
            Me.tlpStep1.Controls.Add(Me.lblDistrict, 2, 2)
            Me.tlpStep1.Controls.Add(Me.cboDistrict, 3, 2)
            ' Row 3: ตำบล * + รหัสวัด
            Me.tlpStep1.Controls.Add(Me.lblSubDistrict, 0, 3)
            Me.tlpStep1.Controls.Add(Me.cboSubDistrict, 1, 3)
            Me.tlpStep1.Controls.Add(Me.lblTempleCode, 2, 3)
            Me.tlpStep1.Controls.Add(Me.txtTempleCode, 3, 3)
            ' Row 4: หมายเหตุ (colspan=4)
            Me.tlpStep1.Controls.Add(Me.lblRequiredNote, 0, 4)
            Me.tlpStep1.SetColumnSpan(Me.lblRequiredNote, 4)

            ' — Labels: right-aligned, Font 11pt, Margin (3,12,8,3), MaximumSize (0,0) —
            '   AutoSize = True, MaximumSize = (0,0) เพื่อให้กว้างตามข้อความไม่ wrap
            For Each lbl As Label In {lblTempleName, lblPhone, lblTempleAddress,
                                       lblProvince, lblDistrict, lblSubDistrict, lblTempleCode}
                lbl.AutoSize = True
                lbl.MaximumSize = New Size(0, 0)
                lbl.Dock = DockStyle.None
                lbl.Anchor = AnchorStyles.Right
                lbl.Font = New Font("Tahoma", 11.0!)
                lbl.ForeColor = Color.FromArgb(69, 26, 3)
                lbl.TextAlign = ContentAlignment.MiddleRight
                lbl.Margin = New Padding(3, 12, 8, 3)
            Next
            Me.lblTempleName.Text = WizardText.TempleName
            Me.lblPhone.Text = WizardText.Phone
            Me.lblTempleAddress.Text = WizardText.Address
            Me.lblProvince.Text = WizardText.Province
            Me.lblDistrict.Text = WizardText.District
            Me.lblSubDistrict.Text = WizardText.Subdistrict
            Me.lblTempleCode.Text = WizardText.Postcode

            ' — TextBoxes: Dock=Fill, Font 12pt, Margin (3,8,3,8), MinimumSize 220x32 —
            For Each tb As TextBox In {txtTempleName, txtPhone, txtTempleAddress, txtTempleCode}
                tb.Dock = DockStyle.Fill
                tb.Font = New Font("Tahoma", 12.0!)
                tb.Margin = New Padding(3, 8, 3, 8)
                tb.MinimumSize = New Size(220, 32)
            Next

            ' — ComboBoxes: Dock=Fill, Font 12pt, Margin (3,8,3,8), MinimumSize 220x32 —
            For Each cb As ComboBox In {cboProvince, cboDistrict, cboSubDistrict}
                cb.Dock = DockStyle.Fill
                cb.DropDownStyle = ComboBoxStyle.DropDownList
                cb.Font = New Font("Tahoma", 12.0!)
                cb.Margin = New Padding(3, 8, 3, 8)
                cb.MinimumSize = New Size(220, 32)
            Next

            ' — TabIndex —
            Me.txtTempleName.TabIndex = 0
            Me.txtPhone.TabIndex = 1
            Me.txtTempleAddress.TabIndex = 2
            Me.cboProvince.TabIndex = 3
            Me.cboDistrict.TabIndex = 4
            Me.cboSubDistrict.TabIndex = 5
            Me.txtTempleCode.MaxLength = 5
            Me.txtTempleCode.TabIndex = 6

            Me.lblRequiredNote.AutoSize = True
            Me.lblRequiredNote.MaximumSize = New Size(0, 0)
            Me.lblRequiredNote.Font = New Font("Tahoma", 9.5!)
            Me.lblRequiredNote.ForeColor = Color.FromArgb(120, 80, 40)
            Me.lblRequiredNote.Text = WizardText.RequiredNote
            Me.lblRequiredNote.Dock = DockStyle.Top
            Me.lblRequiredNote.Margin = New Padding(3, 12, 3, 3)

            ' — Add to pnlStep1 —
            Me.pnlStep1.Controls.Add(Me.tlpStep1)

            ' ============================================================
            '  STEP 2 — ตั้งค่าบุคลากรวัด
            ' ============================================================
            Me.pnlStep2.Dock = DockStyle.Fill
            Me.pnlStep2.Visible = False
            Me.pnlStep2.BackColor = Color.FromArgb(255, 253, 244)
            Me.pnlStep2.Padding = New Padding(30, 20, 30, 20)
            Me.pnlStep2.AutoScroll = True
            Me.pnlStep2.Margin = New Padding(0)

            ' — Description —
            Me.lblStep2Desc.Dock = DockStyle.Top
            Me.lblStep2Desc.Font = New Font("Tahoma", 10.5!)
            Me.lblStep2Desc.ForeColor = Color.FromArgb(100, 80, 50)
            Me.lblStep2Desc.Text = WizardText.Step2Desc
            Me.lblStep2Desc.TextAlign = ContentAlignment.MiddleLeft
            Me.lblStep2Desc.AutoSize = True
            Me.lblStep2Desc.MinimumSize = New Size(0, 35)
            Me.lblStep2Desc.Margin = New Padding(0, 0, 0, 8)

            ' — tlpStep2: 4 columns (Autoize labels) —
            Me.tlpStep2.Dock = DockStyle.Top
            Me.tlpStep2.AutoSize = True
            Me.tlpStep2.AutoSizeMode = AutoSizeMode.GrowAndShrink
            Me.tlpStep2.ColumnCount = 4
            Me.tlpStep2.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            Me.tlpStep2.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
            Me.tlpStep2.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            Me.tlpStep2.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
            Me.tlpStep2.RowCount = 2
            Me.tlpStep2.RowStyles.Add(New RowStyle(SizeType.AutoSize)) ' Row 0
            Me.tlpStep2.RowStyles.Add(New RowStyle(SizeType.AutoSize)) ' Row 1
            Me.tlpStep2.Margin = New Padding(0, 0, 0, 8)
            Me.tlpStep2.Padding = New Padding(0)

            ' Row 0: เจ้าอาวาส + ไวยาวัจกร
            Me.tlpStep2.Controls.Add(Me.lblAbbot, 0, 0)
            Me.tlpStep2.Controls.Add(Me.cboAbbotName, 1, 0)
            Me.tlpStep2.Controls.Add(Me.lblWaiyawat, 2, 0)
            Me.tlpStep2.Controls.Add(Me.cboWaiyawatName, 3, 0)
            ' Row 1: ผู้ทำบัญชี
            Me.tlpStep2.Controls.Add(Me.lblBookkeeper, 0, 1)
            Me.tlpStep2.Controls.Add(Me.cboBookkeeperName, 1, 1)

            For Each lbl As Label In {lblAbbot, lblWaiyawat, lblBookkeeper}
                lbl.AutoSize = True
                lbl.MaximumSize = New Size(0, 0)
                lbl.Dock = DockStyle.None
                lbl.Anchor = AnchorStyles.Right
                lbl.Font = New Font("Tahoma", 11.0!)
                lbl.ForeColor = Color.FromArgb(69, 26, 3)
                lbl.TextAlign = ContentAlignment.MiddleRight
                lbl.Margin = New Padding(3, 12, 8, 3)
            Next
            Me.lblAbbot.Text = WizardText.Abbot
            Me.lblWaiyawat.Text = WizardText.Waiyawat
            Me.lblBookkeeper.Text = WizardText.Bookkeeper

            For Each cb As ComboBox In {cboAbbotName, cboWaiyawatName, cboBookkeeperName}
                cb.Dock = DockStyle.Fill
                cb.DropDownStyle = ComboBoxStyle.DropDownList
                cb.Font = New Font("Tahoma", 12.0!)
                cb.Margin = New Padding(3, 8, 3, 8)
                cb.MinimumSize = New Size(220, 32)
            Next

            Me.cboAbbotName.TabIndex = 10
            Me.cboWaiyawatName.TabIndex = 11
            Me.cboBookkeeperName.TabIndex = 12

            ' — DataGridView: Dock=Fill —
            Me.dgvPersonnel.Dock = DockStyle.Fill
            Me.dgvPersonnel.AllowUserToAddRows = False
            Me.dgvPersonnel.AllowUserToDeleteRows = False
            Me.dgvPersonnel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvPersonnel.BackgroundColor = Color.White
            Me.dgvPersonnel.BorderStyle = BorderStyle.FixedSingle
            Me.dgvPersonnel.ReadOnly = True
            Me.dgvPersonnel.SelectionMode = DataGridViewSelectionMode.FullRowSelect

            ' — Personnel Count: Dock=Bottom —
            Me.lblPersonnelCount.Dock = DockStyle.Bottom
            Me.lblPersonnelCount.Font = New Font("Tahoma", 10.0!)
            Me.lblPersonnelCount.ForeColor = Color.FromArgb(100, 80, 50)
            Me.lblPersonnelCount.Text = String.Format(WizardText.PersonnelCountFormat, "0")
            Me.lblPersonnelCount.TextAlign = ContentAlignment.MiddleLeft
            Me.lblPersonnelCount.AutoSize = True
            Me.lblPersonnelCount.MinimumSize = New Size(0, 28)

            ' — Controls.Add: reverse order for Dock stacking —
            Me.pnlStep2.Controls.Add(Me.dgvPersonnel)
            Me.pnlStep2.Controls.Add(Me.lblPersonnelCount)
            Me.pnlStep2.Controls.Add(Me.tlpStep2)
            Me.pnlStep2.Controls.Add(Me.lblStep2Desc)

            ' ============================================================
            '  STEP 3 — ตั้งค่าบัญชีธนาคาร
            ' ============================================================
            Me.pnlStep3.Dock = DockStyle.Fill
            Me.pnlStep3.Visible = False
            Me.pnlStep3.BackColor = Color.FromArgb(255, 253, 244)
            Me.pnlStep3.Padding = New Padding(30, 20, 30, 20)
            Me.pnlStep3.AutoScroll = True
            Me.pnlStep3.Margin = New Padding(0)

            Me.lblStep3Desc.Dock = DockStyle.Top
            Me.lblStep3Desc.Font = New Font("Tahoma", 10.5!)
            Me.lblStep3Desc.ForeColor = Color.FromArgb(100, 80, 50)
            Me.lblStep3Desc.Text = WizardText.Step3Desc
            Me.lblStep3Desc.TextAlign = ContentAlignment.MiddleLeft
            Me.lblStep3Desc.AutoSize = True
            Me.lblStep3Desc.MinimumSize = New Size(0, 35)
            Me.lblStep3Desc.Margin = New Padding(0, 0, 0, 8)

            Me.lblBankHeader.Dock = DockStyle.Top
            Me.lblBankHeader.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.lblBankHeader.ForeColor = Color.FromArgb(69, 26, 3)
            Me.lblBankHeader.Text = WizardText.BankHeader
            Me.lblBankHeader.TextAlign = ContentAlignment.MiddleLeft
            Me.lblBankHeader.AutoSize = True
            Me.lblBankHeader.MinimumSize = New Size(0, 28)
            Me.lblBankHeader.Margin = New Padding(0, 0, 0, 4)

            Me.dgvBankAccounts.Dock = DockStyle.Top
            Me.dgvBankAccounts.Height = 150
            Me.dgvBankAccounts.AllowUserToAddRows = False
            Me.dgvBankAccounts.AllowUserToDeleteRows = False
            Me.dgvBankAccounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvBankAccounts.BackgroundColor = Color.White
            Me.dgvBankAccounts.BorderStyle = BorderStyle.FixedSingle
            Me.dgvBankAccounts.ReadOnly = True
            Me.dgvBankAccounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.dgvBankAccounts.Margin = New Padding(0, 0, 0, 8)

            Me.btnAddBankAccount.BackColor = Color.FromArgb(37, 99, 235)
            Me.btnAddBankAccount.FlatStyle = FlatStyle.Flat
            Me.btnAddBankAccount.Font = New Font("Tahoma", 10.5!, FontStyle.Bold)
            Me.btnAddBankAccount.ForeColor = Color.White
            Me.btnAddBankAccount.Dock = DockStyle.Top
            Me.btnAddBankAccount.Text = WizardText.AddBank
            Me.btnAddBankAccount.AutoSize = True
            Me.btnAddBankAccount.MinimumSize = New Size(200, 42)
            Me.btnAddBankAccount.Margin = New Padding(0, 0, 0, 8)
            Me.btnAddBankAccount.TabIndex = 20

            ' — grpAddBank —
            Me.grpAddBank.Dock = DockStyle.Top
            Me.grpAddBank.Font = New Font("Tahoma", 10.5!, FontStyle.Bold)
            Me.grpAddBank.ForeColor = Color.FromArgb(69, 26, 3)
            Me.grpAddBank.Text = WizardText.AddBankGroup
            Me.grpAddBank.Visible = False
            Me.grpAddBank.AutoSize = True
            Me.grpAddBank.MinimumSize = New Size(700, 120)
            Me.grpAddBank.Margin = New Padding(0, 0, 0, 8)

            ' — Internal TLP inside grpAddBank —
            Dim tlpBankInputs As New TableLayoutPanel()
            tlpBankInputs.Dock = DockStyle.Fill
            tlpBankInputs.AutoSize = True
            tlpBankInputs.AutoSizeMode = AutoSizeMode.GrowAndShrink
            tlpBankInputs.ColumnCount = 6
            tlpBankInputs.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            tlpBankInputs.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.0!))
            tlpBankInputs.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            tlpBankInputs.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.0!))
            tlpBankInputs.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            tlpBankInputs.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 34.0!))
            tlpBankInputs.RowCount = 2
            tlpBankInputs.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            tlpBankInputs.RowStyles.Add(New RowStyle(SizeType.AutoSize))

            Me.lblNewBankName.AutoSize = True
            Me.lblNewBankName.MaximumSize = New Size(0, 0)
            Me.lblNewBankName.Dock = DockStyle.Fill
            Me.lblNewBankName.Font = New Font("Tahoma", 10.0!)
            Me.lblNewBankName.Text = WizardText.BankNameLabel
            Me.lblNewBankName.TextAlign = ContentAlignment.MiddleRight
            Me.lblNewBankName.Margin = New Padding(3, 12, 8, 3)
            Me.txtNewBankName.Dock = DockStyle.Fill
            Me.txtNewBankName.Font = New Font("Tahoma", 10.0!)
            Me.txtNewBankName.Margin = New Padding(3, 8, 3, 8)
            Me.txtNewBankName.MinimumSize = New Size(120, 28)
            Me.txtNewBankName.TabIndex = 21

            Me.lblNewAccountNo.AutoSize = True
            Me.lblNewAccountNo.MaximumSize = New Size(0, 0)
            Me.lblNewAccountNo.Dock = DockStyle.Fill
            Me.lblNewAccountNo.Font = New Font("Tahoma", 10.0!)
            Me.lblNewAccountNo.Text = WizardText.AccountNoLabel
            Me.lblNewAccountNo.TextAlign = ContentAlignment.MiddleRight
            Me.lblNewAccountNo.Margin = New Padding(3, 12, 8, 3)
            Me.txtNewAccountNo.Dock = DockStyle.Fill
            Me.txtNewAccountNo.Font = New Font("Tahoma", 10.0!)
            Me.txtNewAccountNo.Margin = New Padding(3, 8, 3, 8)
            Me.txtNewAccountNo.MinimumSize = New Size(120, 28)
            Me.txtNewAccountNo.TabIndex = 22

            Me.lblNewAccountName.AutoSize = True
            Me.lblNewAccountName.MaximumSize = New Size(0, 0)
            Me.lblNewAccountName.Dock = DockStyle.Fill
            Me.lblNewAccountName.Font = New Font("Tahoma", 10.0!)
            Me.lblNewAccountName.Text = WizardText.AccountNameLabel
            Me.lblNewAccountName.TextAlign = ContentAlignment.MiddleRight
            Me.lblNewAccountName.Margin = New Padding(3, 12, 8, 3)
            Me.txtNewAccountName.Dock = DockStyle.Fill
            Me.txtNewAccountName.Font = New Font("Tahoma", 10.0!)
            Me.txtNewAccountName.Margin = New Padding(3, 8, 3, 8)
            Me.txtNewAccountName.MinimumSize = New Size(120, 28)
            Me.txtNewAccountName.TabIndex = 23

            Me.btnSaveNewBank.BackColor = Color.FromArgb(22, 163, 74)
            Me.btnSaveNewBank.FlatStyle = FlatStyle.Flat
            Me.btnSaveNewBank.Font = New Font("Tahoma", 10.0!, FontStyle.Bold)
            Me.btnSaveNewBank.ForeColor = Color.White
            Me.btnSaveNewBank.Dock = DockStyle.Fill
            Me.btnSaveNewBank.Margin = New Padding(3, 8, 3, 8)
            Me.btnSaveNewBank.Text = WizardText.SaveBank
            Me.btnSaveNewBank.MinimumSize = New Size(100, 36)
            Me.btnSaveNewBank.TabIndex = 24

            Me.btnCancelNewBank.BackColor = Color.FromArgb(107, 114, 128)
            Me.btnCancelNewBank.FlatStyle = FlatStyle.Flat
            Me.btnCancelNewBank.Font = New Font("Tahoma", 10.0!)
            Me.btnCancelNewBank.ForeColor = Color.White
            Me.btnCancelNewBank.Dock = DockStyle.Fill
            Me.btnCancelNewBank.Margin = New Padding(3, 8, 3, 8)
            Me.btnCancelNewBank.Text = WizardText.CancelBank
            Me.btnCancelNewBank.MinimumSize = New Size(100, 36)
            Me.btnCancelNewBank.TabIndex = 25

            tlpBankInputs.Controls.Add(Me.lblNewBankName, 0, 0)
            tlpBankInputs.Controls.Add(Me.txtNewBankName, 1, 0)
            tlpBankInputs.Controls.Add(Me.lblNewAccountNo, 2, 0)
            tlpBankInputs.Controls.Add(Me.txtNewAccountNo, 3, 0)
            tlpBankInputs.Controls.Add(Me.lblNewAccountName, 4, 0)
            tlpBankInputs.Controls.Add(Me.txtNewAccountName, 5, 0)
            tlpBankInputs.Controls.Add(Me.btnSaveNewBank, 0, 1)
            tlpBankInputs.SetColumnSpan(Me.btnSaveNewBank, 3)
            tlpBankInputs.Controls.Add(Me.btnCancelNewBank, 3, 1)
            tlpBankInputs.SetColumnSpan(Me.btnCancelNewBank, 3)
            Me.grpAddBank.Controls.Add(tlpBankInputs)

            Me.lblFundHeader.Dock = DockStyle.Top
            Me.lblFundHeader.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.lblFundHeader.ForeColor = Color.FromArgb(69, 26, 3)
            Me.lblFundHeader.Text = WizardText.FundHeader
            Me.lblFundHeader.TextAlign = ContentAlignment.MiddleLeft
            Me.lblFundHeader.AutoSize = True
            Me.lblFundHeader.MinimumSize = New Size(0, 28)
            Me.lblFundHeader.Margin = New Padding(0, 0, 0, 4)

            Me.dgvFunds.Dock = DockStyle.Fill
            Me.dgvFunds.AllowUserToAddRows = False
            Me.dgvFunds.AllowUserToDeleteRows = False
            Me.dgvFunds.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvFunds.BackgroundColor = Color.White
            Me.dgvFunds.BorderStyle = BorderStyle.FixedSingle
            Me.dgvFunds.ReadOnly = True
            Me.dgvFunds.SelectionMode = DataGridViewSelectionMode.FullRowSelect

            ' — Controls.Add: reverse order for Dock stacking —
            Me.pnlStep3.Controls.Add(Me.dgvFunds)
            Me.pnlStep3.Controls.Add(Me.lblFundHeader)
            Me.pnlStep3.Controls.Add(Me.grpAddBank)
            Me.pnlStep3.Controls.Add(Me.btnAddBankAccount)
            Me.pnlStep3.Controls.Add(Me.dgvBankAccounts)
            Me.pnlStep3.Controls.Add(Me.lblBankHeader)
            Me.pnlStep3.Controls.Add(Me.lblStep3Desc)

            ' ============================================================
            '  STEP 4 — ยอดยกมาเริ่มต้น
            ' ============================================================
            Me.pnlStep4.Dock = DockStyle.Fill
            Me.pnlStep4.Visible = False
            Me.pnlStep4.BackColor = Color.FromArgb(255, 253, 244)
            Me.pnlStep4.Padding = New Padding(30, 20, 30, 20)
            Me.pnlStep4.AutoScroll = True
            Me.pnlStep4.Margin = New Padding(0)

            Me.lblStep4Desc.Dock = DockStyle.Top
            Me.lblStep4Desc.Font = New Font("Tahoma", 10.5!)
            Me.lblStep4Desc.ForeColor = Color.FromArgb(100, 80, 50)
            Me.lblStep4Desc.Text = WizardText.Step4Desc
            Me.lblStep4Desc.TextAlign = ContentAlignment.MiddleLeft
            Me.lblStep4Desc.AutoSize = True
            Me.lblStep4Desc.MinimumSize = New Size(0, 35)
            Me.lblStep4Desc.Margin = New Padding(0, 0, 0, 8)

            ' — tlpStep4: 4 columns (AutoSize labels) —
            Me.tlpStep4.Dock = DockStyle.Top
            Me.tlpStep4.AutoSize = True
            Me.tlpStep4.AutoSizeMode = AutoSizeMode.GrowAndShrink
            Me.tlpStep4.ColumnCount = 4
            Me.tlpStep4.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            Me.tlpStep4.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
            Me.tlpStep4.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            Me.tlpStep4.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
            Me.tlpStep4.RowCount = 2
            Me.tlpStep4.RowStyles.Add(New RowStyle(SizeType.AutoSize)) ' Row 0
            Me.tlpStep4.RowStyles.Add(New RowStyle(SizeType.AutoSize)) ' Row 1
            Me.tlpStep4.Margin = New Padding(0, 0, 0, 8)
            Me.tlpStep4.Padding = New Padding(0)

            ' Row 0: วันที่เริ่มระบบ
            Me.tlpStep4.Controls.Add(Me.lblBalanceDate, 0, 0)
            Me.tlpStep4.Controls.Add(Me.dtpBalanceDate, 1, 0)
            ' Row 1: เงินสด + เงินฝาก
            Me.tlpStep4.Controls.Add(Me.lblCashBalance, 0, 1)
            Me.tlpStep4.Controls.Add(Me.txtCashBalance, 1, 1)
            Me.tlpStep4.Controls.Add(Me.lblBankBalance, 2, 1)
            Me.tlpStep4.Controls.Add(Me.txtBankBalance, 3, 1)

            For Each lbl As Label In {lblBalanceDate, lblCashBalance, lblBankBalance}
                lbl.AutoSize = True
                lbl.MaximumSize = New Size(0, 0)
                lbl.Dock = DockStyle.None
                lbl.Anchor = AnchorStyles.Right
                lbl.Font = New Font("Tahoma", 11.0!)
                lbl.ForeColor = Color.FromArgb(69, 26, 3)
                lbl.TextAlign = ContentAlignment.MiddleRight
                lbl.Margin = New Padding(3, 12, 8, 3)
            Next
            Me.lblBalanceDate.Text = WizardText.BalanceDate
            Me.lblCashBalance.Text = WizardText.CashBalance
            Me.lblBankBalance.Text = WizardText.BankBalance

            Me.dtpBalanceDate.Dock = DockStyle.Fill
            Me.dtpBalanceDate.Font = New Font("Tahoma", 12.0!)
            Me.dtpBalanceDate.Margin = New Padding(3, 8, 3, 8)
            Me.dtpBalanceDate.Format = DateTimePickerFormat.Short
            Me.dtpBalanceDate.MinimumSize = New Size(220, 32)
            Me.dtpBalanceDate.TabIndex = 30

            Me.txtCashBalance.Dock = DockStyle.Fill
            Me.txtCashBalance.Font = New Font("Tahoma", 12.0!)
            Me.txtCashBalance.Margin = New Padding(3, 8, 3, 8)
            Me.txtCashBalance.Text = "0"
            Me.txtCashBalance.MinimumSize = New Size(220, 32)
            Me.txtCashBalance.TabIndex = 31

            Me.txtBankBalance.Dock = DockStyle.Fill
            Me.txtBankBalance.Font = New Font("Tahoma", 12.0!)
            Me.txtBankBalance.Margin = New Padding(3, 8, 3, 8)
            Me.txtBankBalance.Text = "0"
            Me.txtBankBalance.MinimumSize = New Size(220, 32)
            Me.txtBankBalance.TabIndex = 32

            Me.btnSaveBalance.BackColor = Color.FromArgb(22, 163, 74)
            Me.btnSaveBalance.FlatStyle = FlatStyle.Flat
            Me.btnSaveBalance.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.btnSaveBalance.ForeColor = Color.White
            Me.btnSaveBalance.Dock = DockStyle.Top
            Me.btnSaveBalance.Text = WizardText.SaveBalance
            Me.btnSaveBalance.AutoSize = True
            Me.btnSaveBalance.MinimumSize = New Size(200, 50)
            Me.btnSaveBalance.Margin = New Padding(0, 8, 0, 0)
            Me.btnSaveBalance.TabIndex = 33

            ' — Controls.Add for Step 4: reverse order —
            Me.pnlStep4.Controls.Add(Me.tlpStep4)
            Me.pnlStep4.Controls.Add(Me.btnSaveBalance)
            Me.pnlStep4.Controls.Add(Me.lblStep4Desc)

            ' ============================================================
            '  STEP 5 — เสร็จสิ้น + สรุป
            ' ============================================================
            Me.pnlStep5.Dock = DockStyle.Fill
            Me.pnlStep5.Visible = False
            Me.pnlStep5.BackColor = Color.FromArgb(255, 253, 244)
            Me.pnlStep5.Padding = New Padding(30, 20, 30, 20)
            Me.pnlStep5.AutoScroll = True
            Me.pnlStep5.Margin = New Padding(0)

            Me.lblCheckmark.Dock = DockStyle.Top
            Me.lblCheckmark.Font = New Font("Segoe UI Emoji", 40.0!)
            Me.lblCheckmark.ForeColor = Color.FromArgb(22, 163, 74)
            Me.lblCheckmark.Text = ChrW(&H2705)
            Me.lblCheckmark.TextAlign = ContentAlignment.MiddleCenter
            Me.lblCheckmark.AutoSize = True
            Me.lblCheckmark.MinimumSize = New Size(0, 70)

            Me.lblStep5Subtitle.Dock = DockStyle.Top
            Me.lblStep5Subtitle.Font = New Font("Tahoma", 11.5!, FontStyle.Bold)
            Me.lblStep5Subtitle.ForeColor = Color.FromArgb(22, 163, 74)
            Me.lblStep5Subtitle.Text = WizardText.Step5Subtitle
            Me.lblStep5Subtitle.TextAlign = ContentAlignment.MiddleLeft
            Me.lblStep5Subtitle.AutoSize = True
            Me.lblStep5Subtitle.MinimumSize = New Size(0, 35)
            Me.lblStep5Subtitle.Margin = New Padding(0, 0, 0, 8)

            Me.dgvSummary.Dock = DockStyle.Fill
            Me.dgvSummary.AllowUserToAddRows = False
            Me.dgvSummary.AllowUserToDeleteRows = False
            Me.dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvSummary.BackgroundColor = Color.White
            Me.dgvSummary.BorderStyle = BorderStyle.FixedSingle
            Me.dgvSummary.ReadOnly = True
            Me.dgvSummary.SelectionMode = DataGridViewSelectionMode.FullRowSelect

            Me.btnPrintTestReport.BackColor = Color.FromArgb(37, 99, 235)
            Me.btnPrintTestReport.FlatStyle = FlatStyle.Flat
            Me.btnPrintTestReport.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.btnPrintTestReport.ForeColor = Color.White
            Me.btnPrintTestReport.Dock = DockStyle.Bottom
            Me.btnPrintTestReport.Text = WizardText.PrintReport
            Me.btnPrintTestReport.AutoSize = True
            Me.btnPrintTestReport.MinimumSize = New Size(250, 50)
            Me.btnPrintTestReport.Margin = New Padding(0, 8, 0, 4)
            Me.btnPrintTestReport.TabIndex = 40

            Me.lblCompletionStatus.Dock = DockStyle.Bottom
            Me.lblCompletionStatus.Font = New Font("Tahoma", 11.0!)
            Me.lblCompletionStatus.ForeColor = Color.FromArgb(150, 130, 100)
            Me.lblCompletionStatus.Text = WizardText.CompletionStatus
            Me.lblCompletionStatus.TextAlign = ContentAlignment.MiddleLeft
            Me.lblCompletionStatus.AutoSize = True
            Me.lblCompletionStatus.MinimumSize = New Size(0, 30)

            ' — Controls.Add: reverse order for correct Dock stacking —
            Me.pnlStep5.Controls.Add(Me.dgvSummary)
            Me.pnlStep5.Controls.Add(Me.btnPrintTestReport)
            Me.pnlStep5.Controls.Add(Me.lblCompletionStatus)
            Me.pnlStep5.Controls.Add(Me.lblStep5Subtitle)
            Me.pnlStep5.Controls.Add(Me.lblCheckmark)

            ' ============================================================
            '  เพิ่ม Step Panels เข้า pnlBody
            ' ============================================================
            Me.pnlBody.Controls.Add(Me.pnlStep1)
            Me.pnlBody.Controls.Add(Me.pnlStep2)
            Me.pnlBody.Controls.Add(Me.pnlStep3)
            Me.pnlBody.Controls.Add(Me.pnlStep4)
            Me.pnlBody.Controls.Add(Me.pnlStep5)

            ' ============================================================
            '  pnlFooter — ปุ่มนำทาง + Validation Message (Row 3)
            '  AutoSize = False เพื่อไม่ให้ footer บีบตามเนื้อหา
            ' ============================================================
            Me.pnlFooter.BackColor = Color.FromArgb(245, 245, 240)
            Me.pnlFooter.Dock = DockStyle.Fill
            Me.pnlFooter.AutoSize = False
            Me.pnlFooter.MinimumSize = New Size(0, 76)
            Me.pnlFooter.Margin = New Padding(0)
            Me.pnlFooter.Padding = New Padding(12, 8, 12, 8)

            ' — tlpFooter: 4 columns —
            Me.tlpFooter.Dock = DockStyle.Fill
            Me.tlpFooter.AutoSize = False
            Me.tlpFooter.ColumnCount = 4
            Me.tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0!))
            Me.tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 160.0!))
            Me.tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 160.0!))
            Me.tlpFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 160.0!))
            Me.tlpFooter.RowCount = 1
            Me.tlpFooter.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
            Me.tlpFooter.Margin = New Padding(0)
            Me.tlpFooter.Padding = New Padding(0)

            ' — Col 0: lblValidationMessage —
            Me.lblValidationMessage.AutoSize = False
            Me.lblValidationMessage.Dock = DockStyle.Fill
            Me.lblValidationMessage.Font = New Font("Tahoma", 10.0!)
            Me.lblValidationMessage.ForeColor = Color.Firebrick
            Me.lblValidationMessage.Text = ""
            Me.lblValidationMessage.TextAlign = ContentAlignment.MiddleLeft
            Me.lblValidationMessage.AutoEllipsis = True
            Me.lblValidationMessage.Margin = New Padding(0, 6, 12, 6)
            Me.tlpFooter.Controls.Add(Me.lblValidationMessage, 0, 0)

            ' — Col 1: btnCancel —
            Me.btnCancel.BackColor = Color.FromArgb(107, 114, 128)
            Me.btnCancel.FlatStyle = FlatStyle.Flat
            Me.btnCancel.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.btnCancel.ForeColor = Color.White
            Me.btnCancel.Text = WizardText.BtnCancel
            Me.btnCancel.Size = New Size(150, 50)
            Me.btnCancel.AutoSize = False
            Me.btnCancel.Dock = DockStyle.None
            Me.btnCancel.Anchor = AnchorStyles.None
            Me.btnCancel.Margin = New Padding(6)
            Me.btnCancel.TabIndex = 102
            Me.tlpFooter.Controls.Add(Me.btnCancel, 1, 0)

            ' — Col 2: btnBack —
            Me.btnBack.BackColor = Color.FromArgb(75, 85, 99)
            Me.btnBack.FlatStyle = FlatStyle.Flat
            Me.btnBack.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.btnBack.ForeColor = Color.White
            Me.btnBack.Text = WizardText.BtnBack
            Me.btnBack.Enabled = False
            Me.btnBack.Visible = True
            Me.btnBack.Size = New Size(150, 50)
            Me.btnBack.AutoSize = False
            Me.btnBack.Dock = DockStyle.None
            Me.btnBack.Anchor = AnchorStyles.None
            Me.btnBack.Margin = New Padding(6)
            Me.btnBack.TabIndex = 101
            Me.tlpFooter.Controls.Add(Me.btnBack, 2, 0)

            ' — Col 3: btnNext —
            Me.btnNext.BackColor = Color.FromArgb(234, 88, 12)
            Me.btnNext.FlatStyle = FlatStyle.Flat
            Me.btnNext.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            Me.btnNext.ForeColor = Color.White
            Me.btnNext.Text = WizardText.BtnNext
            Me.btnNext.Enabled = False
            Me.btnNext.Size = New Size(150, 50)
            Me.btnNext.AutoSize = False
            Me.btnNext.Dock = DockStyle.None
            Me.btnNext.Anchor = AnchorStyles.None
            Me.btnNext.Margin = New Padding(6)
            Me.btnNext.TabIndex = 100
            Me.tlpFooter.Controls.Add(Me.btnNext, 3, 0)

            Me.pnlFooter.Controls.Add(Me.tlpFooter)

            ' ============================================================
            '  Add panels to tlpRoot
            ' ============================================================
            Me.tlpRoot.Controls.Add(Me.pnlTestBar, 0, 0)
            Me.tlpRoot.Controls.Add(Me.pnlHeader, 0, 1)
            Me.tlpRoot.Controls.Add(Me.pnlBody, 0, 2)
            Me.tlpRoot.Controls.Add(Me.pnlFooter, 0, 3)

            ' ============================================================
            '  FrmOnboardingWizard — Form Settings
            '  AutoScaleMode = Dpi (ห้าม Font — จะทำให้ขนาดเพี้ยน)
            '  AutoSize = False (ห้าม True — จะทำให้ฟอร์มหด)
            '  ClientSize = 1000x700 (ขนาดที่ใช้งานได้จริง)
            '  MinimumSize = 1000x720 (กันลากย่อจนใช้ไม่ได้)
            ' ============================================================
            Me.AutoScaleMode = AutoScaleMode.Dpi
            Me.ClientSize = New Size(1000, 700)
            Me.MinimumSize = New Size(1000, 720)
            Me.AutoSize = False
            Me.FormBorderStyle = FormBorderStyle.Sizable
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.ControlBox = False

            ' — Only direct child is tlpRoot —
            Me.Controls.Add(Me.tlpRoot)

            ' — Key bindings —
            Me.AcceptButton = Me.btnNext
            Me.CancelButton = Me.btnCancel

            Me.Name = "FrmOnboardingWizard"
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.Text = WizardText.FormTitle
            Me.Font = New Font("Tahoma", 12.0!, FontStyle.Regular, GraphicsUnit.Point, 222)

            ' ============================================================

            '  errProvider — NeverBlink to prevent icon overlapping labels
            Me.errProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink
            Me.errProvider.ContainerControl = Me

            ' ===== ResumeLayout =====
            CType(Me.errProvider, ISupportInitialize).EndInit()
            Me.dgvSummary.ResumeLayout(False)
            Me.dgvFunds.ResumeLayout(False)
            Me.dgvBankAccounts.ResumeLayout(False)
            Me.dgvPersonnel.ResumeLayout(False)
            Me.pnlStep5.ResumeLayout(False) : Me.pnlStep5.PerformLayout()
            Me.tlpStep4.ResumeLayout(False) : Me.tlpStep4.PerformLayout()
            Me.pnlStep4.ResumeLayout(False) : Me.pnlStep4.PerformLayout()
            Me.pnlStep3.ResumeLayout(False) : Me.pnlStep3.PerformLayout()
            Me.tlpStep2.ResumeLayout(False) : Me.tlpStep2.PerformLayout()
            Me.pnlStep2.ResumeLayout(False) : Me.pnlStep2.PerformLayout()
            Me.tlpStep1.ResumeLayout(False) : Me.tlpStep1.PerformLayout()
            Me.pnlStep1.ResumeLayout(False) : Me.pnlStep1.PerformLayout()
            Me.pnlBody.ResumeLayout(False)
            Me.tlpFooter.ResumeLayout(False) : Me.tlpFooter.PerformLayout()
            Me.pnlFooter.ResumeLayout(False) : Me.pnlFooter.PerformLayout()
            Me.tlpHeader.ResumeLayout(False) : Me.tlpHeader.PerformLayout()
            Me.pnlHeader.ResumeLayout(False) : Me.pnlHeader.PerformLayout()
            Me.pnlTestBar.ResumeLayout(False)
            Me.tlpRoot.ResumeLayout(False) : Me.tlpRoot.PerformLayout()
            Me.ResumeLayout(False)
        End Sub

    End Class
End Namespace
