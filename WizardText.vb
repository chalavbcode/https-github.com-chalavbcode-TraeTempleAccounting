Option Strict Off
Option Explicit On

Namespace TempleAccounting

    ''' <summary>
    ''' ข้อความภาษาไทยทั้งหมดของ Onboarding Wizard
    ''' เก็บไว้ที่นี่ที่เดียว เพื่อป้องกันปัญหา ChrW encoding ผิด
    ''' ไฟล์นี้ต้องบันทึกเป็น UTF-8 with BOM เท่านั้น
    ''' </summary>
    Friend Module WizardText

        ' ===== Form Title =====
        Public Const FormTitle As String = "ตั้งค่าเริ่มต้นระบบ — Onboarding Wizard"

        ' ===== Test Mode Banner =====
        Public Const TestModeBanner As String = "🧪 โหมดทดลอง — ข้อมูลจะไม่ถูกบันทึกจริง"
        Public Const BtnExitTestMode As String = "🔒 ปิดโหมดทดลอง"

        ' ===== Header / Step Titles =====
        Public Const Step1Title As String = "ขั้นที่ 1: ตั้งค่าข้อมูลวัด"
        Public Const Step2Title As String = "ขั้นที่ 2: ข้อมูลเจ้าอาวาส"
        Public Const Step3Title As String = "ขั้นที่ 3: ตั้งค่าบัญชีธนาคารและกองทุน"
        Public Const Step4Title As String = "ขั้นที่ 4: กำหนดยอดยกมา"
        Public Const Step5Title As String = "ขั้นที่ 5: ตรวจสอบและยืนยัน"

        Public Const StepIndicatorFormat As String = "ขั้นที่ {0} จาก {1}"

        ' ===== Step 1: ข้อมูลวัด =====
        Public Const TempleName As String = "ชื่อวัด *:"
        Public Const Phone As String = "เบอร์โทร:"
        Public Const Address As String = "ที่อยู่ *:"
        Public Const Province As String = "จังหวัด *:"
        Public Const District As String = "อำเภอ *:"
        Public Const Subdistrict As String = "ตำบล *:"
        Public Const Postcode As String = "รหัสไปรษณีย์:"
        Public Const RequiredNote As String = "* ช่องที่มีเครื่องหมายดอกจันจำเป็นต้องกรอก"

        ' ===== Step 2: ข้อมูลเจ้าอาวาส / บุคลากร =====
        Public Const Step2Desc As String = "เลือกตำแหน่งบุคลากรวัดที่เหมาะสม และกำหนดผู้รับผิดชอบ"
        Public Const Abbot As String = "เจ้าอาวาส :"
        Public Const Waiyawat As String = "ไวยาวัจกร :"
        Public Const Bookkeeper As String = "ผู้ทำบัญชี :"
        Public Const PersonnelCountFormat As String = "ข้อมูลบุคลากรในระบบ: {0} คน..."

        ' ===== Step 3: บัญชีธนาคาร =====
        Public Const Step3Desc As String = "กำหนดช่องทางธนาคารของวัด และเพิ่มข้อมูลบัญชีที่ใช้จริง"
        Public Const BankHeader As String = "ช่องทางธนาคาร:"
        Public Const AddBank As String = "+ เพิ่มบัญชีธนาคาร"
        Public Const AddBankGroup As String = "+ เพิ่มบัญชีธนาคาร ใหม่"
        Public Const BankNameLabel As String = "ธนาคาร :"
        Public Const AccountNoLabel As String = "เลขที่บัญชี :"
        Public Const AccountNameLabel As String = "ชื่อบัญชี :"
        Public Const SaveBank As String = "💾 บันทึกข้อมูล"
        Public Const CancelBank As String = "ยกเลิก"
        Public Const FundHeader As String = "กองทุน:"

        ' ===== Step 4: ยอดยกมา =====
        Public Const Step4Desc As String = "กรอกยอดเงินคงเหลือ ณ วันเริ่มระบบ"
        Public Const BalanceDate As String = "วันที่เริ่มระบบ :"
        Public Const CashBalance As String = "เงินสด :"
        Public Const BankBalance As String = "เงินฝาก :"
        Public Const SaveBalance As String = "💾 บันทึกยอดยกมา"

        ' ===== Step 5: เสร็จสิ้น =====
        Public Const Step5Subtitle As String = "ตั้งค่าข้อมูลเสร็จสิ้น! ตรวจสอบรายละเอียด"
        Public Const PrintReport As String = "🖨️ พิมพ์รายงานหน้า (ตรวจสอบ)"
        Public Const CompletionStatus As String = ChrW(&H23F3) & " รอการตรวจสอบรายงาน"
        Public Const CompletionStatusDone As String = ChrW(&H2705) & " ตรวจสอบรายงานเรียบร้อย — พร้อมเริ่มใช้งาน!"

        ' ===== Navigation Buttons =====
        Public Const BtnCancel As String = "ยกเลิก"
        Public Const BtnBack As String = ChrW(&H2190) & " ย้อนกลับ"
        Public Const BtnNext As String = "ถัดไป " & ChrW(&H2192)
        Public Const BtnFinish As String = ChrW(&H2713) & " เริ่มใช้งานโปรแกรม"

    End Module

End Namespace
