Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Windows.Forms

Namespace TempleAccounting
    <DesignerCategory("Form")>
    Partial Public Class FrmOnboardingWizard
        Inherits Form

#Region "===== ตัวแปรสถานะ ====="

        Private _currentStep As Integer = 1
        Private Const TotalSteps As Integer = 5
        Private _isTestMode As Boolean = False
        Private _testReportViewed As Boolean = False
        Private _originalControlColors As New Dictionary(Of Control, Color)
        Private _validationTip As New ToolTip()
        Private _bankAccountAdded As Boolean = False
        Private _isLoading As Boolean = False

#End Region

#Region "===== Constructor ====="

        ''' <summary>
        ''' สร้าง Onboarding Wizard
        ''' </summary>
        ''' <param name="isTestMode">True = โหมดทดลอง (ไม่บันทึกจริง, เปิด ControlBox ได้)</param>
        Public Sub New(Optional isTestMode As Boolean = False)
            InitializeComponent()
            _isTestMode = isTestMode
        End Sub

#End Region

#Region "===== Form Load / Shown ====="

        Private Sub FrmOnboardingWizard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Try
                ' ตั้งค่าโหมดทดลอง
                If _isTestMode Then
                    SetupTestMode()
                End If

                Db.EnsureSchema()

                ' ===== Re-entry guard: ถ้ามีข้อมูลวัดอยู่แล้ว ข้าม wizard (เฉพาะโหมดปกติ) =====
                If Not _isTestMode Then
                    If OnboardingRepository.GetTempleSettingCount() > 0 Then
                        AppPreferences.IsOnboardingCompleted = True
                        AppPreferences.Save()
                        Me.DialogResult = DialogResult.OK
                        Me.Close()
                        Return
                    End If
                End If

                ' ===== โหลดข้อมูลเริ่มต้น =====
                LoadStep1Locations()
                LoadStep2Personnel()
                LoadStep3BankAndFunds()
                dtpBalanceDate.Value = Date.Today
                txtCashBalance.Text = "0"
                txtBankBalance.Text = "0"

                ' ===== เก็บสีเดิมสำหรับ Validation =====
                StoreOriginalColor(txtTempleName)
                StoreOriginalColor(txtTempleAddress)
                StoreOriginalColor(cboProvince)
                StoreOriginalColor(cboDistrict)
                StoreOriginalColor(cboSubDistrict)
                StoreOriginalColor(txtPhone)

                ' ===== แสดงขั้นตอนที่ 1 =====
                ShowStep(1)

                UiFitter.AutoFitFormButtons(Me)
                UiFitter.DisableComboBoxWheel(Me)
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ข้อผิดพลาด",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub FrmOnboardingWizard_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
            ' ===== PART 7 Guard: บังคับขนาดขั้นต่ำ กันกรณีมีโค้ดอื่นแอบย่อฟอร์ม =====
            If Me.Width < Me.MinimumSize.Width OrElse Me.Height < Me.MinimumSize.Height Then
                Me.Size = New Size(1000, 720)
                Debug.WriteLine("[GUARD] " & T(&H0E1F,&H0E2D,&H0E23,&H0E4C,&H0E21) & " " & T(&H0E16,&H0E39,&H0E01) & " " & T(&H0E22,&H0E48,&H0E2D) & " " & T(&H0E1C,&H0E34,&H0E14,&H0E1B,&H0E01,&H0E15,&H0E34) & " -- " & T(&H0E1A,&H0E31,&H0E07,&H0E04,&H0E31,&H0E1A) & " " & T(&H0E01,&H0E25,&H0E31,&H0E1A) & " " & T(&H0E1B,&H0E40,&H0E19) & " " & "1000x720")
            End If
            Debug.WriteLine($"[SIZE] Form={Me.Size} Client={Me.ClientSize} tlpRoot={tlpRoot.Size}")
#If DEBUG Then
            AuditOverlap(Me)
#End If
        End Sub

#End Region

#Region "===== Helper: สร้างข้อความไทยจาก Unicode Code Points ====="

        ''' <summary>สร้างข้อความไทยจาก array ของ Unicode code points (ใช้หลีกเลี่ยง BOM/Encoding ปัญหา)</summary>
        Private Function T(ParamArray codes() As Integer) As String
            Dim sb As New System.Text.StringBuilder(codes.Length)
            For Each c In codes : sb.Append(ChrW(c)) : Next
            Return sb.ToString()
        End Function

#End Region

#Region "===== AuditOverlap — Debug Helper ตรวจจับ Control ทับกัน ====="

#If DEBUG Then
        ''' <summary>
        ''' ตรวจหา Control ที่พื้นที่ทับซ้อนกันใน Parent เดียวกัน แล้วรายงานลง Output
        ''' กรองคู่ Container กับลูกของตัวเองออก (ไม่นับ)
        ''' </summary>
        Private Sub AuditOverlap(ByVal parent As Control)
            Dim visibleControls = parent.Controls.Cast(Of Control)().
                                  Where(Function(c) c.Visible).ToList()
            For i As Integer = 0 To visibleControls.Count - 2
                For j As Integer = i + 1 To visibleControls.Count - 1
                    Dim a = visibleControls(i), b = visibleControls(j)
                    ' กรองคู่ที่เป็น Container กับลูกของตัวเอง
                    If a Is b.Parent OrElse b Is a.Parent Then Continue For
                    If a.Bounds.IntersectsWith(b.Bounds) Then
                        Debug.WriteLine($"[OVERLAP] {a.Name} ทับ {b.Name} ใน {parent.Name}")
                    End If
                Next
            Next
            For Each child As Control In visibleControls
                If child.HasChildren Then AuditOverlap(child)
            Next
        End Sub
#End If

#End Region

#Region "===== Test Mode Setup ====="

        ''' <summary>ตั้งค่าฟอร์มสำหรับโหมดทดลอง</summary>
        Private Sub SetupTestMode()
            ' แสดงแถบสีส้ม
            pnlTestBar.Visible = True
            ' เปิด ControlBox เพื่อให้ปิดได้
            Me.ControlBox = True
            Me.MinimizeBox = True
            ' แสดงปุ่มปิดโหมดทดลอง
            btnCloseTestMode.Visible = True
            ' เปลี่ยนสี Next button เป็นสีส้ม (แทนสีเขียว)
            btnNext.BackColor = Color.FromArgb(234, 88, 12)
            ' เปลี่ยนสี header
            pnlHeader.BackColor = Color.FromArgb(255, 200, 150)
            lblStepTitle.ForeColor = Color.FromArgb(154, 52, 18)
            ' เพิ่ม ToolTip
            ttValidation.SetToolTip(btnCloseTestMode, "ปิด Wizard โหมดทดลอง (ไม่บันทึกข้อมูล)")
        End Sub

#End Region

#Region "===== Validation Visual Helpers ====="

        Private Sub StoreOriginalColor(c As Control)
            If Not _originalControlColors.ContainsKey(c) Then _originalControlColors(c) = c.BackColor
        End Sub

        ''' <summary>ตั้งค่า Control ที่ยังไม่ผ่าน Validate: BackColor = MistyRose + ToolTip (ไม่ใช้ ErrorProvider กันไอคอนทับ Label)</summary>
        Private Sub SetControlInvalid(c As Control, tip As String)
            c.BackColor = Color.MistyRose
            _validationTip.SetToolTip(c, tip)
        End Sub

        ''' <summary>ล้าง Validation: BackColor = Honeydew</summary>
        Private Sub ClearControlValidation(c As Control)
            c.BackColor = Color.Honeydew
            _validationTip.SetToolTip(c, "")
        End Sub

#End Region

#Region "===== ValidateCurrentStep — ตรวจสอบเงื่อนไขปลดล็อกปุ่มถัดไป ====="

        ''' <summary>
        ''' ตรวจสอบความถูกต้องของข้อมูลในขั้นตอนปัจจุบัน
        ''' เรียกใช้ทุกครั้งที่มีการเปลี่ยนแปลงข้อมูล (TextChanged / SelectedIndexChanged / หลังบันทึก)
        ''' ใช้ Select Case ตาม _currentStep
        ''' </summary>
        Private Function ValidateCurrentStep() As Boolean
            btnNext.Enabled = False
            lblValidationMessage.Text = ""

            Select Case _currentStep
                Case 1
                    ' ===== Step 1: TempleSetting ต้องไม่ว่าง =====
                    Dim nameOk = Not String.IsNullOrWhiteSpace(txtTempleName.Text)
                    Dim addrOk = Not String.IsNullOrWhiteSpace(txtTempleAddress.Text)
                    Dim provOk = cboProvince.SelectedValue IsNot Nothing
                    Dim distOk = cboDistrict.SelectedValue IsNot Nothing
                    Dim subOk = cboSubDistrict.SelectedValue IsNot Nothing

                    ' Visual feedback: MistyRose = invalid, Honeydew = valid
                    If nameOk Then ClearControlValidation(txtTempleName) Else SetControlInvalid(txtTempleName, "กรุณากรอกชื่อวัด")
                    If addrOk Then ClearControlValidation(txtTempleAddress) Else SetControlInvalid(txtTempleAddress, "กรุณากรอกที่อยู่")
                    If provOk Then ClearControlValidation(cboProvince) Else SetControlInvalid(cboProvince, "กรุณาเลือกจังหวัด")
                    If distOk Then ClearControlValidation(cboDistrict) Else SetControlInvalid(cboDistrict, "กรุณาเลือกอำเภอ")
                    If subOk Then ClearControlValidation(cboSubDistrict) Else SetControlInvalid(cboSubDistrict, "กรุณาเลือกตำบล")

                    If nameOk AndAlso addrOk AndAlso provOk AndAlso distOk AndAlso subOk Then
                        btnNext.Enabled = True
                    Else
                        ' แสดงเหตุผลที่กดถัดไปไม่ได้
                        Dim reasons As New List(Of String)
                        If Not nameOk Then reasons.Add("ชื่อวัด")
                        If Not addrOk Then reasons.Add("ที่อยู่")
                        If Not provOk Then reasons.Add("จังหวัด")
                        If Not distOk Then reasons.Add("อำเภอ")
                        If Not subOk Then reasons.Add("ตำบล")
                        lblValidationMessage.Text = "⚠ กรุณากรอก " & String.Join(", ", reasons) & " ให้ครบถ้วน"
                    End If

                Case 2
                    ' ===== Step 2: ต้องมีบุคลากรในฐานข้อมูลอย่างน้อย 1 คน (เช็ก DB จริง) =====
                    Dim personnelCount = OnboardingRepository.GetPersonnelCount()
                    If personnelCount >= 1 Then
                        Dim hasSelection As Boolean =
                            GetSelectedIntValue(cboAbbotName).HasValue OrElse
                            GetSelectedIntValue(cboWaiyawatName).HasValue OrElse
                            GetSelectedIntValue(cboBookkeeperName).HasValue
                        If hasSelection Then
                            btnNext.Enabled = True
                        Else
                            lblValidationMessage.Text = "⚠ กรุณาเลือกบุคลากรอย่างน้อย 1 ตำแหน่ง (เจ้าอาวาส / ไวยาวัจกร / ผู้ทำบัญชี)"
                        End If
                    Else
                        lblValidationMessage.Text = "⚠ ไม่พบข้อมูลบุคลากรในระบบ — กรุณาเพิ่มบุคลากรผ่านเมนู 'พระ / อาวาส' ก่อน"
                    End If

                Case 3
                    ' ===== Step 3: ต้องมี BankAccounts อย่างน้อย 1 บัญชี (เช็ก DB จริง) =====
                    Dim bankCount = OnboardingRepository.GetBankAccountCount()
                    If bankCount >= 1 Then
                        btnNext.Enabled = True
                    Else
                        lblValidationMessage.Text = "⚠ กรุณาเพิ่มบัญชีธนาคารอย่างน้อย 1 บัญชี (คลิกปุ่ม '➕ เพิ่มบัญชีธนาคาร')"
                    End If

                Case 4
                    ' ===== Step 4: ยอมรับค่า 0 ได้ แต่ต้องเป็นตัวเลข >= 0 =====
                    Dim cashOk As Boolean = False
                    Dim bankOk As Boolean = False
                    Dim cashVal As Decimal = 0D
                    Dim bankVal As Decimal = 0D
                    cashOk = Decimal.TryParse(txtCashBalance.Text.Trim(), cashVal) AndAlso cashVal >= 0
                    bankOk = Decimal.TryParse(txtBankBalance.Text.Trim(), bankVal) AndAlso bankVal >= 0

                    If cashOk AndAlso bankOk Then
                        btnNext.Enabled = True
                    Else
                        lblValidationMessage.Text = "⚠ กรุณากรอกยอดเงินสดและเงินฝากให้ถูกต้อง (ต้องเป็นตัวเลขไม่ติดลบ)"
                    End If

                Case 5
                    ' ===== Step 5: เปิดให้กดได้เลย =====
                    btnNext.Enabled = True

            End Select

            Return btnNext.Enabled
        End Function

#End Region

#Region "===== ShowStep — แสดงขั้นตอนที่ต้องการ ====="

        ''' <summary>
        ''' แสดงขั้นตอนที่กำหนด จัดการ Visible ของ Panel, ข้อความ Header,
        ''' ProgressBar, สถานะปุ่มนำทาง, โฟกัสอัตโนมัติ
        ''' </summary>
        Private Sub ShowStep(stepNum As Integer)
            If stepNum < 1 OrElse stepNum > TotalSteps Then Return
            _currentStep = stepNum

            ' สลับ visibility ของ Step Panels
            Dim panels As Panel() = {pnlStep1, pnlStep2, pnlStep3, pnlStep4, pnlStep5}
            For i As Integer = 0 To panels.Length - 1
                panels(i).Visible = (i = stepNum - 1)
            Next

            ' ===== ตั้งค่า Header =====
            Dim stepTitles As String() = {
                WizardText.Step1Title,
                WizardText.Step2Title,
                WizardText.Step3Title,
                WizardText.Step4Title,
                WizardText.Step5Title
            }
            lblStepTitle.Text = stepTitles(stepNum - 1)
            lblStepIndicator.Text = String.Format(WizardText.StepIndicatorFormat, stepNum, TotalSteps)
            pgbStep.Value = CInt(CDbl(stepNum) / CDbl(TotalSteps) * 100.0)

            ' ===== ตั้งค่าปุ่มนำทาง =====
            ' ปุ่มย้อนกลับ: แสดงเสมอแต่ disabled ใน Step 1 เพื่อไม่ให้ตำแหน่งปุ่มขยับ
            btnBack.Enabled = (stepNum > 1)

            If stepNum < TotalSteps Then
                btnNext.Text = WizardText.BtnNext
            ElseIf stepNum = TotalSteps Then
                btnNext.Text = WizardText.BtnFinish
            End If

            ' ===== โหลดข้อมูลเมื่อกลับไปขั้นตอนก่อนหน้า =====
            If stepNum = 1 Then LoadStep1FromDb()
            If stepNum = 2 Then RefreshStep2Data()
            If stepNum = 3 Then RefreshStep3Data()

            ' ===== แสดง summary ใน Step 5 =====
            If stepNum = 5 Then LoadSummary()

            ' ===== Validate =====
            ValidateCurrentStep()

            ' ===== โฟกัสอัตโนมัติที่ Input ตัวแรกของแต่ละ Step =====
            FocusFirstInput(stepNum)

#If DEBUG Then
            AuditOverlap(Me)
#End If
        End Sub

        ''' <summary>โฟกัสที่ Input ตัวแรกของแต่ละ Step</summary>
        Private Sub FocusFirstInput(stepNum As Integer)
            Select Case stepNum
                Case 1
                    If txtTempleName.CanFocus Then txtTempleName.Focus()
                Case 2
                    If cboAbbotName.CanFocus Then cboAbbotName.Focus()
                Case 3
                    If btnAddBankAccount.CanFocus Then btnAddBankAccount.Focus()
                Case 4
                    If dtpBalanceDate.CanFocus Then dtpBalanceDate.Focus()
                Case 5
                    If btnPrintTestReport.CanFocus Then btnPrintTestReport.Focus()
            End Select
        End Sub

#End Region

#Region "===== Navigation Button Events ====="

        Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
            If Not btnNext.Enabled Then Return
            Try
                Select Case _currentStep
                    Case 1
                        If Not _isTestMode Then SaveStep1()
                        ShowStep(2)
                    Case 2
                        If Not _isTestMode Then SaveStep2()
                        ShowStep(3)
                    Case 3
                        ShowStep(4)
                    Case 4
                        If Not _isTestMode Then SaveStep4()
                        ShowStep(5)
                    Case 5
                        ' ===== เสร็จสิ้น — ตั้งค่า IsOnboardingCompleted =====
                        If Not _isTestMode Then
                            AppPreferenceHelper.MarkOnboardingComplete()
                        Else
                            Debug.WriteLine("[OnboardingWizard TEST] เสร็จสิ้น — ไม่ได้ตั้งค่า IsOnboardingCompleted")
                        End If
                        Me.DialogResult = DialogResult.OK
                        Me.Close()
                End Select
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ข้อผิดพลาด",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
            If _currentStep > 1 Then ShowStep(_currentStep - 1)
        End Sub

        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            Dim msg As String = "ต้องการยกเลิกการตั้งค่าเริ่มต้นหรือไม่?"
            If _isTestMode Then msg = "ต้องการปิดโหมดทดลองหรือไม่?"
            Dim result = MessageBox.Show(msg, "ยืนยัน",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                         MessageBoxDefaultButton.Button2)
            If result = DialogResult.Yes Then
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            End If
        End Sub

        Private Sub btnCloseTestMode_Click(sender As Object, e As EventArgs) Handles btnCloseTestMode.Click
            Dim result = MessageBox.Show("ต้องการปิดโหมดทดลองหรือไม่? (ข้อมูลที่กรอกจะไม่ถูกบันทึก)",
                                         "ปิดโหมดทดลอง",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                         MessageBoxDefaultButton.Button2)
            If result = DialogResult.Yes Then
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            End If
        End Sub

#End Region

#Region "===== FormClosing (Test Mode) ====="

        Private Sub FrmOnboardingWizard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
            If _isTestMode AndAlso Me.DialogResult <> DialogResult.OK AndAlso Me.DialogResult <> DialogResult.Cancel Then
                Dim result = MessageBox.Show("ต้องการปิดโหมดทดลองหรือไม่?",
                                             "ปิดโหมดทดลอง",
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                             MessageBoxDefaultButton.Button2)
                If result <> DialogResult.Yes Then
                    e.Cancel = True
                Else
                    Me.DialogResult = DialogResult.Cancel
                End If
            End If
        End Sub

#End Region

#Region "===== Step 1: ข้อมูลวัด ====="

        Private Sub LoadStep1Locations()
            Using conn = Db.OpenConn()
                Dim p = Db.GetTable(conn, "SELECT ProvinceID, ProvinceName FROM Province ORDER BY ProvinceName")
                cboProvince.DisplayMember = "ProvinceName"
                cboProvince.ValueMember = "ProvinceID"
                cboProvince.DataSource = p
            End Using
        End Sub

        ''' <summary>โหลดข้อมูลวัดจาก DB (ใช้เมื่อกลับไป Step 1)</summary>
        Private Sub LoadStep1FromDb()
            Try
                Dim row = OnboardingRepository.LoadTempleSetting()
                If row IsNot Nothing Then
                    ' txtTempleCode คือช่อง "รหัสไปรษณีย์" (ชื่อ control เดิมตั้งผิด) → ต้อง map กับคอลัมน์ PostCode เสมอ
                    ' (เดิมอ่านจาก TempleCode ทำให้รอบ Back→Next เขียนทับรหัสไปรษณีย์ที่กรอกไว้ด้วยค่าว่าง)
                    txtTempleCode.Text = If(row("PostCode") Is DBNull.Value, "", row("PostCode").ToString())
                    txtTempleName.Text = If(row("TempleName") Is DBNull.Value, "", row("TempleName").ToString())
                    txtTempleAddress.Text = If(row("TempleAddress") Is DBNull.Value, "", row("TempleAddress").ToString())
                    txtPhone.Text = If(row("TemplePhone") Is DBNull.Value, "", row("TemplePhone").ToString())
                End If
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("FrmOnboardingWizard.LoadStep1FromDb", ex)
            End Try
        End Sub

        ''' <summary>
        ''' อ่านค่า TempleCode (รหัสวัด) เดิมจาก DB เพื่อคงไว้ตอนบันทึก — wizard ไม่มีช่องกรอกรหัสวัด
        ''' จึงห้ามเขียนทับด้วยค่าว่าง (ค่าเดิมมาจาก FrmTempleSetting)
        ''' </summary>
        Private Function GetExistingTempleCode() As String
            Try
                Dim row = OnboardingRepository.LoadTempleSetting()
                If row IsNot Nothing AndAlso row("TempleCode") IsNot DBNull.Value Then
                    Return row("TempleCode").ToString()
                End If
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("FrmOnboardingWizard.GetExistingTempleCode", ex)
            End Try
            Return ""
        End Function

        ''' <summary>บันทึกข้อมูลวัดลง DB (ภายใน Transaction)</summary>
        Private Sub SaveStep1()
            Dim provinceName As String = ""
            Dim districtName As String = ""
            Dim subDistrictName As String = ""

            If TypeOf cboProvince.SelectedItem Is DataRowView Then
                provinceName = CType(cboProvince.SelectedItem, DataRowView)("ProvinceName")?.ToString()
            Else
                provinceName = cboProvince.Text
            End If
            If TypeOf cboDistrict.SelectedItem Is DataRowView Then
                districtName = CType(cboDistrict.SelectedItem, DataRowView)("DistrictName")?.ToString()
            Else
                districtName = cboDistrict.Text
            End If
            If TypeOf cboSubDistrict.SelectedItem Is DataRowView Then
                subDistrictName = CType(cboSubDistrict.SelectedItem, DataRowView)("SubDistrictName")?.ToString()
            Else
                subDistrictName = cboSubDistrict.Text
            End If

            ' หมายเหตุ: txtTempleCode คือช่องรหัสไปรษณีย์ (ชื่อ control เดิมตั้งผิด) → เก็บลง PostCode
            ' ส่วน TempleCode คงค่าเดิมไว้ (GetExistingTempleCode) เพราะ wizard ไม่มีช่องกรอกรหัสวัด
            OnboardingRepository.SaveTempleSetting(
                templeCode:=GetExistingTempleCode(),
                templeName:=txtTempleName.Text.Trim(),
                templeAddress:=txtTempleAddress.Text.Trim(),
                tambon:=subDistrictName,
                amphoe:=districtName,
                province:=provinceName,
                postCode:=txtTempleCode.Text.Trim(),
                phone:=txtPhone.Text.Trim())

            Debug.WriteLine("[OnboardingWizard] SaveStep1: บันทึกข้อมูลวัดเรียบร้อย — " & txtTempleName.Text.Trim())
        End Sub

        ''' <summary>handled: TextChanged ของฟิลด์ Step 1 → re-validate</summary>
        Private Sub txtStep1Field_TextChanged(sender As Object, e As EventArgs) _
            Handles txtTempleName.TextChanged, txtTempleAddress.TextChanged, txtTempleCode.TextChanged
            ValidateCurrentStep()
        End Sub

        ''' <summary>จำกัด txtTempleCode ให้รับเฉพาะตัวเลข (รหัสไปรษณีย์)</summary>
        Private Sub txtTempleCode_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtTempleCode.KeyPress
            If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
                e.Handled = True
            End If
        End Sub

#End Region

#Region "===== ComboBox SelectedValue Helper ====="

        ''' <summary>ดึงค่า Integer จาก ComboBox SelectedValue อย่างปลอดภัย</summary>
        Private Function GetSelectedIntValue(c As ComboBox) As Integer?
            If c Is Nothing Then Return Nothing
            If c.SelectedValue IsNot Nothing AndAlso Not TypeOf c.SelectedValue Is DataRowView Then
                Dim raw = c.SelectedValue.ToString()
                Dim parsed As Integer
                If Integer.TryParse(raw, parsed) Then Return parsed
            End If
            ' Fallback: ลองจาก DataRowView
            Dim drv = TryCast(c.SelectedItem, DataRowView)
            If drv IsNot Nothing Then
                Dim raw = drv(c.DisplayMember)?.ToString()
                Dim parsed As Integer
                If Integer.TryParse(raw, parsed) Then Return parsed
            End If
            Return Nothing
        End Function

#End Region

#Region "===== Step 1: Province → District → SubDistrict Cascade ====="

        Private Sub cboProvince_SelectedIndexChanged_Cascade(sender As Object, e As EventArgs) _
            Handles cboProvince.SelectedIndexChanged
            If _isLoading Then Return
            _isLoading = True
            Try
                ' ล้างลูก combo ก่อน
                cboDistrict.DataSource = Nothing
                cboDistrict.Items.Clear()
                cboDistrict.Text = ""
                cboSubDistrict.DataSource = Nothing
                cboSubDistrict.Items.Clear()
                cboSubDistrict.Text = ""

                Dim pid = GetSelectedIntValue(cboProvince)
                If Not pid.HasValue Then Return

                Using conn = Db.OpenConn()
                    cboDistrict.DataSource = Db.GetTable(conn,
                        "SELECT DistrictID, DistrictName FROM District WHERE ProvinceID=@p ORDER BY DistrictName",
                        New Tuple(Of String, Object)("@p", pid.Value))
                    cboDistrict.DisplayMember = "DistrictName"
                    cboDistrict.ValueMember = "DistrictID"
                End Using
            Finally
                _isLoading = False
            End Try
            ValidateCurrentStep()
        End Sub

        Private Sub cboDistrict_SelectedIndexChanged_Cascade(sender As Object, e As EventArgs) _
            Handles cboDistrict.SelectedIndexChanged
            If _isLoading Then Return
            _isLoading = True
            Try
                ' ล้างลูก combo ก่อน
                cboSubDistrict.DataSource = Nothing
                cboSubDistrict.Items.Clear()
                cboSubDistrict.Text = ""

                Dim did = GetSelectedIntValue(cboDistrict)
                If Not did.HasValue Then Return

                Using conn = Db.OpenConn()
                    cboSubDistrict.DataSource = Db.GetTable(conn,
                        "SELECT SubDistrictID, SubDistrictName FROM SubDistrict WHERE DistrictID=@d ORDER BY SubDistrictName",
                        New Tuple(Of String, Object)("@d", did.Value))
                    cboSubDistrict.DisplayMember = "SubDistrictName"
                    cboSubDistrict.ValueMember = "SubDistrictID"
                End Using

                ' ปิดการใช้งานถ้าไม่มีตำบล
                If cboSubDistrict.Items.Count = 0 Then
                    cboSubDistrict.Enabled = False
                Else
                    cboSubDistrict.Enabled = True
                End If
            Finally
                _isLoading = False
            End Try
            ValidateCurrentStep()
        End Sub

        ''' <summary>Validate ทุกครั้งที่เปลี่ยนComboBox ใน Step 1</summary>
        Private Sub cboStep1_SelectedIndexChanged(sender As Object, e As EventArgs) _
            Handles cboProvince.SelectedIndexChanged, cboDistrict.SelectedIndexChanged, cboSubDistrict.SelectedIndexChanged
            ' Note: Cascade handlers above fire first, this handler fires after
            ' ValidateCurrentStep is already called by cascade handlers
        End Sub

#End Region

#Region "===== Step 2: บุคลากร ====="

        ''' <summary>โหลดข้อมูลบุคลากรสำหรับ ComboBox (พระ/ฆราวาส)</summary>
        Private Sub LoadStep2Personnel()
            Dim monkGroup = "พระ"
            Dim layGroup = "ฆราวาส"

            ' โหลดบุคลากรพระ (เจ้าอาวาส)
            Dim abbotTable = OnboardingRepository.GetPersonnelByGroup(monkGroup)
            cboAbbotName.DisplayMember = "FullName"
            cboAbbotName.ValueMember = "PersonnelID"
            cboAbbotName.DataSource = abbotTable

            ' โหลดบุคลากรฆราวาส (ไวยาวัจกร / ผู้ทำบัญชี)
            Dim layTable = OnboardingRepository.GetPersonnelByGroup(layGroup)
            cboWaiyawatName.DisplayMember = "FullName"
            cboWaiyawatName.ValueMember = "PersonnelID"
            cboWaiyawatName.DataSource = layTable.Copy()

            cboBookkeeperName.DisplayMember = "FullName"
            cboBookkeeperName.ValueMember = "PersonnelID"
            cboBookkeeperName.DataSource = layTable.Copy()

            ' แสดงจำนวนบุคลากร
            Dim count = OnboardingRepository.GetPersonnelCount()
            lblPersonnelCount.Text = String.Format(WizardText.PersonnelCountFormat, count.ToString())

            ' โหลดตารางสรุปบุคลากรที่ลิงก์แล้ว
            RefreshStep2Data()
        End Sub

        ''' <summary>รีเฟรชข้อมูล Step 2 (ตารางสรุปบุคลากร)</summary>
        Private Sub RefreshStep2Data()
            Try
                dgvPersonnel.DataSource = OnboardingRepository.GetLinkedPersonnelSummary()
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("FrmOnboardingWizard.RefreshStep2Data", ex)
            End Try
        End Sub

        ''' <summary>บันทึกการลิงก์บุคลากรเข้า TempleSetting</summary>
        Private Sub SaveStep2()
            Dim abbotID = GetSelectedIntValue(cboAbbotName)
            Dim waiyawatID = GetSelectedIntValue(cboWaiyawatName)
            Dim bookkeeperID = GetSelectedIntValue(cboBookkeeperName)

            Dim abbotObj As Object = If(abbotID.HasValue, CType(abbotID.Value, Object), DBNull.Value)
            Dim waiyawatObj As Object = If(waiyawatID.HasValue, CType(waiyawatID.Value, Object), DBNull.Value)
            Dim bookkeeperObj As Object = If(bookkeeperID.HasValue, CType(bookkeeperID.Value, Object), DBNull.Value)

            OnboardingRepository.LinkPersonnelToTempleSetting(abbotObj, waiyawatObj, bookkeeperObj)

            ' ล้างแคช ReportEngine
            ReportEngine.ClearTemplateInfoCache()

            Debug.WriteLine("[OnboardingWizard] SaveStep2: ลิงก์บุคลากรเรียบร้อย — " &
                            "Abbot=" & abbotID.ToString() &
                            " Waiyawat=" & waiyawatID.ToString() &
                            " Bookkeeper=" & bookkeeperID.ToString())
        End Sub

        Private Sub cboAbbotName_SelectedIndexChanged_Validate(sender As Object, e As EventArgs) _
            Handles cboAbbotName.SelectedIndexChanged, cboWaiyawatName.SelectedIndexChanged, cboBookkeeperName.SelectedIndexChanged
            ValidateCurrentStep()
        End Sub

#End Region

#Region "===== Step 3: บัญชีธนาคาร ====="

        ''' <summary>โหลดข้อมูลบัญชีธนาคารและกองทุน</summary>
        Private Sub LoadStep3BankAndFunds()
            RefreshStep3Data()
        End Sub

        ''' <summary>รีเฟรชข้อมูล Step 3</summary>
        Private Sub RefreshStep3Data()
            Try
                dgvBankAccounts.DataSource = OnboardingRepository.GetBankAccounts()
                dgvFunds.DataSource = OnboardingRepository.GetFunds()
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("FrmOnboardingWizard.RefreshStep3Data", ex)
            End Try
        End Sub

        ''' <summary>แสดง/ซ่อนฟอร์มเพิ่มบัญชีธนาคาร</summary>
        Private Sub btnAddBankAccount_Click(sender As Object, e As EventArgs) Handles btnAddBankAccount.Click
            grpAddBank.Visible = Not grpAddBank.Visible
            If grpAddBank.Visible Then
                txtNewBankName.Text = ""
                txtNewAccountNo.Text = ""
                txtNewAccountName.Text = ""
                txtNewBankName.Focus()
            End If
        End Sub

        ''' <summary>บันทึกบัญชีธนาคารใหม่</summary>
        Private Sub btnSaveNewBank_Click(sender As Object, e As EventArgs) Handles btnSaveNewBank.Click
            If String.IsNullOrWhiteSpace(txtNewBankName.Text) Then
                MessageBox.Show("กรุณากรอกชื่อธนาคาร", "ข้อมูลไม่ครบ",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtNewBankName.Focus()
                Return
            End If

            Try
                If _isTestMode Then
                    Debug.WriteLine("[OnboardingWizard TEST] เพิ่มบัญชีธนาคาร: " &
                                    "Bank=" & txtNewBankName.Text.Trim() &
                                    " AccountNo=" & txtNewAccountNo.Text.Trim() &
                                    " AccountName=" & txtNewAccountName.Text.Trim())
                Else
                    OnboardingRepository.AddBankAccount(
                        txtNewBankName.Text.Trim(),
                        txtNewAccountNo.Text.Trim(),
                        txtNewAccountName.Text.Trim())
                End If

                ' รีเฟรชตาราง
                RefreshStep3Data()
                grpAddBank.Visible = False
                _bankAccountAdded = True
                ValidateCurrentStep()
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ข้อผิดพลาด",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ''' <summary>ยกเลิกการเพิ่มบัญชี</summary>
        Private Sub btnCancelNewBank_Click(sender As Object, e As EventArgs) Handles btnCancelNewBank.Click
            grpAddBank.Visible = False
        End Sub

#End Region

#Region "===== Step 4: ยอดยกมา ====="

        ''' <summary>บันทึกยอดยกมาเริ่มต้น</summary>
        Private Sub SaveStep4()
            Dim cashVal As Decimal = 0D
            Dim bankVal As Decimal = 0D
            Decimal.TryParse(txtCashBalance.Text.Trim(), cashVal)
            Decimal.TryParse(txtBankBalance.Text.Trim(), bankVal)

            OnboardingRepository.SaveOpeningBalance(dtpBalanceDate.Value.Date, cashVal, bankVal)

            _testReportViewed = True ' อนุญาตให้ไป Step 5
            Debug.WriteLine("[OnboardingWizard] SaveStep4: บันทึกยอดยกมา — Cash=" & cashVal.ToString() & " Bank=" & bankVal.ToString())
        End Sub

        Private Sub btnSaveBalance_Click(sender As Object, e As EventArgs) Handles btnSaveBalance.Click
            Try
                Dim cashVal As Decimal = 0D
                Dim bankVal As Decimal = 0D
                Decimal.TryParse(txtCashBalance.Text.Trim(), cashVal)
                Decimal.TryParse(txtBankBalance.Text.Trim(), bankVal)

                If _isTestMode Then
                    Debug.WriteLine("[OnboardingWizard TEST] บันทึกยอดยกมา — Cash=" & cashVal.ToString() & " Bank=" & bankVal.ToString())
                Else
                    OnboardingRepository.SaveOpeningBalance(dtpBalanceDate.Value.Date, cashVal, bankVal)
                End If

                _testReportViewed = True
                MessageBox.Show("บันทึกยอดยกมาเรียบร้อย!", "สำเร็จ",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ข้อผิดพลาด",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub txtStep4Balance_TextChanged(sender As Object, e As EventArgs) _
            Handles txtCashBalance.TextChanged, txtBankBalance.TextChanged
            ValidateCurrentStep()
        End Sub

#End Region

#Region "===== Step 5: สรุปข้อมูล ====="

        ''' <summary>โหลดข้อมูลสรุปสำหรับแสดงใน Step 5</summary>
        Private Sub LoadSummary()
            Try
                dgvSummary.DataSource = OnboardingRepository.GetOnboardingSummary()
                ' ตั้งค่าความกว้างคอลัมน์
                If dgvSummary.Columns.Count >= 2 Then
                    dgvSummary.Columns(0).Width = 200
                    dgvSummary.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                End If
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("FrmOnboardingWizard.LoadSummary", ex)
            End Try
        End Sub

        Private Sub btnPrintTestReport_Click(sender As Object, e As EventArgs) Handles btnPrintTestReport.Click
            Try
                Dim startDate As New Date(Date.Today.Year, Date.Today.Month, 1)
                IncomeExpenseReport.ShowPreview(startDate, startDate.AddMonths(1).AddDays(-1), Me,
                                                IncomeExpenseReport.ReportModes.Detailed)
                _testReportViewed = True
                lblCompletionStatus.Text = WizardText.CompletionStatusDone
                lblCompletionStatus.ForeColor = Color.FromArgb(22, 163, 74)
                ValidateCurrentStep()
            Catch ex As Exception
                MessageBox.Show("เกิดข้อผิดพลาด: " & ex.Message, "ข้อผิดพลาด",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

#End Region

    End Class
End Namespace
