# Project Profile Document

## ชื่อโครงการ
`TempleAccounting_FullProject_20260726_090422`  
ระบบจัดการบัญชีและการเงินสำหรับวัด

## สถานะเอกสาร
- ประเภทเอกสาร: `Project Profile / Technical Reference / Historical Record`
- วัตถุประสงค์: ใช้เป็นเอกสารอ้างอิงกลางสำหรับการพัฒนา การบำรุงรักษา การตรวจสอบขอบเขตระบบ และการส่งมอบงานในอนาคต
- แหล่งอ้างอิง: จัดทำจากการตรวจสอบซอร์สโค้ดและโครงสร้างโปรเจกต์จริง ณ วันที่ `2026-08-01` (อัพเดตครั้งแรกจาก `2026-07-26`)

## หมายเหตุ
เอกสารฉบับนี้จัดทำขึ้นเพื่อเป็นจุดอ้างอิงเชิงเทคนิคและเชิงประวัติโครงการ โดยยึดข้อมูลจากซอร์สโค้ด โครงสร้างไฟล์ และองค์ประกอบของระบบที่มีอยู่จริงใน workspace ปัจจุบันเป็นหลัก

## 1. ข้อมูลทั่วไปของโครงการ

### 1.1 ภาพรวมระบบ
ระบบนี้เป็นแอปพลิเคชัน Desktop สำหรับงานบัญชีวัด พัฒนาด้วย Windows Forms เพื่อรองรับการบันทึกรายรับ รายจ่าย การโอนภายใน การตั้งค่าข้อมูลวัด การจัดการข้อมูลหลัก และการออกรายงานทางการเงินจากฐานข้อมูลจริงของหน่วยงาน

### 1.2 เป้าหมายของระบบ
- สนับสนุนการบันทึกข้อมูลรายรับ รายจ่าย และการโอนระหว่างกองทุนหรือบัญชีธนาคาร
- จัดเก็บข้อมูลทางบัญชีในโครงสร้างที่ตรวจสอบย้อนหลังได้
- รองรับการพิมพ์รายงานสรุปบัญชีรายรับ-รายจ่ายทั้งแบบละเอียดและแบบย่อ
- รองรับการนำเข้าข้อมูลจาก Excel เพื่อลดภาระการคีย์ข้อมูลจำนวนมาก
- สนับสนุนการดูแลข้อมูลวัด ข้อมูลพระ/เจ้าอาวาส/ไวยาวัจกร/ผู้ทำบัญชี/พร้อมเพย์ และข้อมูลที่ตั้ง

### 1.3 กลุ่มผู้ใช้งานหลัก
- เจ้าอาวาส
- ไวยาวัจกร
- ผู้ทำบัญชี / เหรัญญิกวัด
- ผู้ดูแลระบบข้อมูลหลัก

## 2. สถาปัตยกรรมระบบและเทคโนโลยีที่ใช้

### 2.1 เทคโนโลยีจริงที่ตรวจพบจากซอร์ส
- ภาษา: `VB.NET`
- Framework: `.NET 10.0 Windows Forms`
- ประเภทแอปพลิเคชัน: `WinExe`
- Solution file: `TempleAccounting.slnx`
- Project file: `TempleAccounting.vbproj`
- Data access: `System.Data.OleDb`
- ฐานข้อมูลหลัก: `Microsoft Access (.accdb)`

### 2.2 ข้อสรุปเชิงเทคนิค
จากการตรวจสอบไฟล์ `TempleAccounting.vbproj` และซอร์สจริง ไม่พบว่าโปรเจกต์หลักใช้ `C#` หรือ `SQLite` ใน runtime หลักของระบบ ณ เวลาที่จัดทำเอกสารฉบับนี้ ระบบทำงานบน `VB.NET + OleDb + Microsoft Access`

### 2.3 องค์ประกอบระดับสถาปัตยกรรม
- `Program.vb` ทำหน้าที่เป็นจุดเริ่มต้นระบบ
- `AppPaths.vb` จัดการ path ของฐานข้อมูล, Logs, Backup, Export และไฟล์นำเข้าประกอบ
- `Database.vb` เป็น data access layer และ schema bootstrap กลาง
- `frmMain.vb` เป็น shell หลักของแอปพลิเคชัน
- กลุ่มฟอร์มในโฟลเดอร์ `Forms\` เป็นหน้าจอปฏิบัติงาน
- `Reports\IncomeExpenseReport.vb` เป็นเอนจินรายงานพิมพ์
- `ExcelTransactionImporter.vb` เป็นกลไกนำเข้าข้อมูลจาก Excel

## 3. ฟังก์ชันการทำงานหลักของระบบ

### 3.1 โมดูลบันทึกรายรับ
ไฟล์หลัก: `Forms\FrmIncome.vb`

ความสามารถ:
- บันทึกรายรับเข้าตาราง `Transactions`
- เลือกประเภท (`Categories`) เฉพาะฝั่ง `Income`
- เลือกกองทุน (`Funds`)
- เลือกบัญชีธนาคารได้กรณีมีการฝากเงินจริง
- รองรับกรณีเงินสดโดยไม่จำเป็นต้องระบุธนาคาร
- นำเข้าข้อมูลรายรับจาก Excel ผ่าน `ExcelTransactionImporter.vb`

### 3.2 โมดูลบันทึกรายจ่าย
ไฟล์หลัก: `Forms\FrmExpense.vb`

ความสามารถ:
- บันทึกรายจ่ายเข้าตาราง `Transactions`
- เลือกประเภท (`Categories`) เฉพาะฝั่ง `Expense`
- เลือกกองทุนและบัญชีธนาคารตามสภาพจริง
- รองรับกรณีจ่ายเป็นเงินสดโดยไม่จำเป็นต้องเลือกบัญชีธนาคาร
- นำเข้าข้อมูลรายจ่ายจาก Excel

### 3.3 โมดูลโอนเงินภายใน
ไฟล์หลัก: `Forms\FrmTransfer.vb`

ความสามารถ:
- บันทึกธุรกรรมชนิด `Transfer`
- ใช้ `FundID` / `BankID` เป็นต้นทาง
- ใช้ `ToFundID` / `ToBankID` เป็นปลายทาง
- รองรับกรณีต้นทางหรือปลายทางบางมิติเป็นค่าว่างตามการใช้งานจริง

### 3.4 โมดูลรายการรับ-จ่ายทั้งหมด
ไฟล์หลัก: `Forms\FrmTransactions.vb`

ความสามารถ:
- แสดงข้อมูลจากตาราง `Transactions` ทั้งหมด
- ค้นหา กรอง และสรุปยอดรายรับ/รายจ่าย/คงเหลือ/โอนภายใน
- แสดงข้อมูลประกอบจาก `Categories`, `Funds`, `BankAccounts`
- รองรับการแก้ไขและลบรายการจากหน้ากริด
- ใช้งานร่วมกับ workflow แบบ Visual Studio Debug ได้โดยชี้ฐานข้อมูลไปยังโฟลเดอร์โปรเจกต์
- **UI ภาษาไทย**: แปลฟิลด์ DataGridView เป็นภาษาไทยใน ConfigureGridColumns():
  - ID → ID
  - TranDate → วันที่
  - TranTypeDisplay → ชนิด
  - CategoryName → ประเภท
  - FundName → กองทุน
  - BankName → ธนาคาร
  - Detail → รายละเอียด
  - Amount → จำนวนเงิน
  - Note → หมายเหตุ
  - CreateDate → วันที่บันทึก
  - ToFundName → ไปยังกองทุน
  - ToBankName → ไปยังธนาคาร

### 3.5 โมดูลรายงาน
ไฟล์หลัก:
- `Forms\FrmReports.vb`
- `Reports\IncomeExpenseReport.vb`

ความสามารถ:
- รายงานสรุปบัญชีรายรับ-รายจ่าย
- รองรับ `รายงานแบบละเอียด` และ `รายงานแบบย่อ`
- จัดหน้ากระดาษ A4 แนวนอน
- รองรับยอดยกมาและยอดยกไปตามช่วงวันที่
- ส่งออกข้อมูลรายงานเป็น CSV ไปยังโฟลเดอร์ `Export`
- **ฟีเจอร์ยอดยกมาแบบกำหนดเอง**:
  - เพิ่ม TextBox (txtBalance) สำหรับกรอกยอดยกมาเอง
  - เพิ่ม Button (btnCalcBalance) สำหรับคำนวณยอดยกมาจากฐานข้อมูล
  - GetManualBalance() function สำหรับอ่านค่าจาก txtBalance
  - IncomeExpenseReport constructor รองรับ manualOpeningBalance parameter
  - ToolTips สำหรับแนะนำการใช้งาน (txtBalance, btnCalcBalance)
- **การปรับปรุง UI**:
  - ปรับ layout ของฟอร์มรายงานให้กระชับขึ้น
  - เพิ่มขนาด DataGridView และลด header
  - ปรับขนาด controls และ fonts ให้เหมาะสม

### 3.6 โมดูลตั้งค่าข้อมูลวัด
ไฟล์หลัก: `Forms\FrmTempleSetting.vb`

ความสามารถ:
- บันทึกข้อมูลวัดลงตาราง `TempleSetting`
- จัดเก็บชื่อวัด ที่อยู่ ตำบล อำเภอ จังหวัด รหัสไปรษณีย์ โทรศัพท์
- จัดเก็บข้อมูลเจ้าอาวาส ไวยาวัจกร ผู้ทำบัญชี และพร้อมเพย์
- ผูกข้อมูลกับตารางที่ตั้ง `Province`, `District`, `SubDistrict`

### 3.7 โมดูลข้อมูลหลัก
ไฟล์หลัก: `Forms\FrmMasterData.vb`

ความสามารถ:
- จัดการ `Categories`
- จัดการ `Funds`
- จัดการ `BankAccounts`

### 3.8 โมดูลนำเข้าข้อมูลพื้นที่
ไฟล์หลัก: `Forms\FrmLocationImport.vb`

ความสามารถ:
- นำเข้าข้อมูล `Province`, `District`, `SubDistrict`
- ใช้ไฟล์ CSV จากโฟลเดอร์ `Import`
- อัปเดตข้อมูลภูมิศาสตร์สำหรับหน้าตั้งค่าวัด

### 3.9 การปรับปรุง UI/UX
ไฟล์หลัก: `frmMain.Designer.vb`, `FrmReports.Designer.vb`

การปรับปรุงที่ดำเนินการ:
- **Dashboard Cards**: ลดความสูงของ pnlCard1-4 จาก 95px เป็น 90px เพื่อให้ dashboard กระชับขึ้น
- **Reports Layout**: ปรับ layout ของฟอร์มรายงานให้กระชับขึ้น:
  - เพิ่มขนาด DataGridView
  - ลด header
  - ปรับขนาด controls และ fonts ให้เหมาะสม
- **Thai Localization**: แปลฟิลด์ DataGridView ใน FrmTransactions เป็นภาษาไทยทั้งหมด
- **Error Fixes**: แก้ไขปัญหา BC30451 errors โดย comment out บรรทัดที่อ้างถึง lblCardXIcon ที่ไม่มีใน Designer

## 4. โครงสร้างฐานข้อมูล

### 4.1 ตารางหลักที่ตรวจพบ
- `TempleSetting`
- `Categories`
- `Funds`
- `BankAccounts`
- `Transactions`
- `Province`
- `District`
- `SubDistrict`

### 4.2 ตารางธุรกรรมหลัก
ตาราง `Transactions` มีฟิลด์หลักดังนี้:
- `ID`
- `TranDate`
- `TranType`
- `CategoryID`
- `FundID`
- `BankID`
- `Detail`
- `Amount`
- `Note`
- `CreateDate`
- `ToFundID`
- `ToBankID`

### 4.3 ความสัมพันธ์เชิงตรรกะ
- `CategoryID` เชื่อมกับ `Categories.ID`
- `FundID` เชื่อมกับ `Funds.ID`
- `BankID` เชื่อมกับ `BankAccounts.ID`
- `ToFundID` และ `ToBankID` ใช้สำหรับธุรกรรมโอนภายใน

### 4.4 หมายเหตุด้านวันที่
ระบบใช้ helper `AccessDateLiteral(...)` และ logic ปรับปี พ.ศ./ค.ศ. เพื่อให้ Access ตีความวันที่ถูกต้องในรายงานและหน้ารายการ

## 5. โครงสร้างโฟลเดอร์ของโปรเจกต์

### 5.1 โฟลเดอร์สำคัญ
- `Backup` เก็บไฟล์สำรองฐานข้อมูล
- `Database` เก็บฐานข้อมูลหลัก `TempleAccounting.accdb`
- `Export` เก็บไฟล์รายงานหรือไฟล์ส่งออก
- `Forms` เก็บหน้าจอหลักของระบบ
- `Import` เก็บไฟล์ template และไฟล์นำเข้าประกอบ
- `Logs` เก็บ log ที่จำเป็น เช่น `crash.log`
- `Reports` เก็บ source ของรายงานพิมพ์

### 5.2 ไฟล์สำคัญระดับระบบ
- `Program.vb`
- `AppPaths.vb`
- `Database.vb`
- `ExcelTransactionImporter.vb`
- `frmMain.vb`
- `TempleAccounting.vbproj`
- `TempleAccounting.slnx`

## 6. การสำรองข้อมูลและความปลอดภัยของข้อมูล

### 6.1 แนวทางการสำรองข้อมูล
- ระบบมีฟังก์ชัน `Db.BackupDatabase()`
- ค่าเริ่มต้นจะสำรองไฟล์ฐานข้อมูลไปยังโฟลเดอร์ `Backup`
- การนำเข้า Excel มีการเรียกสำรองฐานข้อมูลก่อนนำเข้าจริง

### 6.2 แนวทางด้านความถูกต้องของข้อมูล
- มีการเรียก `Db.EnsureSchema()` ตอนเริ่มระบบเพื่อให้ schema หลักพร้อมใช้งาน
- มี logic migration สำหรับฟิลด์ที่อาจขาดในตาราง `Transactions`
- มีการ seed ข้อมูลหลักบางส่วน เช่น หมวดหมู่ กองทุน และบัญชีธนาคารเริ่มต้น

### 6.3 แนวทางด้าน log
- โปรเจกต์ถูกปรับให้คงไว้เฉพาะ `critical log`
- log หลักด้านความผิดปกติถูกเก็บใน `Logs\crash.log`

## 7. ขอบเขตระบบตามของจริงในเวลาจัดทำเอกสาร

### 7.1 สิ่งที่มีอยู่จริง
- ระบบรายรับ
- ระบบรายจ่าย
- ระบบโอนภายใน
- ระบบรายการรับ-จ่ายทั้งหมด
- ระบบข้อมูลหลัก
- ระบบข้อมูลวัด
- ระบบรายงานพิมพ์
- ระบบนำเข้าข้อมูลจาก Excel
- ระบบนำเข้าข้อมูลจังหวัด/อำเภอ/ตำบล
- ระบบสำรองฐานข้อมูล

### 7.2 สิ่งที่ยังไม่พบเป็นโมดูลสมบูรณ์ในซอร์สหลัก
- ระบบสมาชิก/ผู้บริจาคแบบเฉพาะทางเป็นโมดูลแยกเต็มรูปแบบ
- ระบบสิทธิ์ผู้ใช้งานหลายระดับแบบสมบูรณ์
- การใช้งาน SQLite เป็นฐานข้อมูลหลัก
- โมดูล export PDF แบบแยกชัดเจนในซอร์สที่ตรวจพบ

## 8. ข้อเสนอแนะสำหรับการพัฒนาต่อ
- จัดทำ data dictionary แยกจากเอกสารฉบับนี้
- จัดทำ user manual ตามบทบาทผู้ใช้
- แยก business rules ของเงินสด/ธนาคาร/กองทุนให้ชัดเจนยิ่งขึ้น
- พิจารณาซ่อนคอลัมน์รหัสดิบในหน้ารายการสำหรับผู้ใช้ทั่วไป
- เพิ่มรายงานยอดคงเหลือตามกองทุนและตามบัญชีธนาคาร

## 9. สรุป
โปรเจกต์ `TempleAccounting_FullProject_20260726_090422` เป็นระบบบัญชีวัดแบบ Windows Forms ที่พัฒนาบน `VB.NET + .NET 10 + Microsoft Access` มีโครงสร้างการทำงานจริงครอบคลุมธุรกรรมรายรับ รายจ่าย การโอนภายใน รายงาน และการดูแลข้อมูลหลัก โดยข้อมูลในเอกสารฉบับนี้ยึดจากซอร์สและโครงสร้างโปรเจกต์จริง ไม่ใช่เพียงข้อกำหนดเชิงสมมติ

## 10. Historical Technical Incident Record

### 10.1 แหล่งข้อมูลเหตุการณ์
บันทึกจาก startup log และ unhandled exception log ที่ตรวจพบในช่วงวันที่ `2026-07-26`

### 10.2 เหตุการณ์สำคัญที่ตรวจพบจาก log

#### กรณีที่ 1: การใช้ฟังก์ชัน `NZ()` ในคำสั่ง SQL ผ่าน OleDb
- ข้อความผิดพลาด: `Undefined function 'NZ' in expression.`
- จุดที่พบ:
  - `FrmReports.btnLedger_Click`
  - `FrmReports.btnSummaryIncome_Click`
  - `FrmReports_Load`
- ความหมายเชิงเทคนิค:
  - คำสั่ง SQL บางส่วนอ้างอิงฟังก์ชัน `NZ()` แบบที่ใช้ใน Access UI/Query Designer แต่ไม่สามารถพึ่งพาได้เสมอเมื่อรันผ่าน `System.Data.OleDb`
- ผลกระทบ:
  - รายงานบางส่วนเปิดไม่ได้หรือแสดงผลล้มเหลว
- ข้อสรุป:
  - ควรหลีกเลี่ยงการพึ่งพา `NZ()` ใน SQL runtime ของแอป และใช้ `IIF(... IS NULL, ..., ...)` หรือจัดการค่า `NULL` ที่ชั้นโค้ดแทน

#### กรณีที่ 2: คำสั่ง SQL มีพารามิเตอร์ไม่ครบหรืออ้างอิงชื่อฟิลด์ไม่ตรง
- ข้อความผิดพลาด: `No value given for one or more required parameters.`
- จุดที่พบ:
  - `FrmReports.btnMonthly_Click`
- ความหมายเชิงเทคนิค:
  - มีความเป็นไปได้ว่าคำสั่ง SQL อ้างชื่อฟิลด์, alias หรือ parameter ไม่ตรงกับ schema จริงของตาราง
- ผลกระทบ:
  - รายงานบางปุ่มไม่สามารถประมวลผลได้

#### กรณีที่ 3: โครงสร้าง `JOIN` ไม่ถูกต้องใน Access SQL
- ข้อความผิดพลาด:
  - `Syntax error (missing operator) in query expression 't.BankID=b.ID LEFT JOIN Funds f2 ON t.ToFundID=f2.ID LEFT JOIN BankAccounts b2 ON t.ToBankID=b2.I'`
- จุดที่พบ:
  - `FrmTransactions.LoadData()`
- ความหมายเชิงเทคนิค:
  - รูปแบบ `LEFT JOIN` แบบต่อเนื่องใน Access ต้องจัดวงเล็บและลำดับการ join อย่างเคร่งครัด หากประกอบ SQL ผิดเพียงเล็กน้อยจะทำให้ query ทั้งชุดล้มเหลว
- ผลกระทบ:
  - หน้า `FrmTransactions` อาจแสดงเฉพาะหัวคอลัมน์แต่ไม่ขึ้นข้อมูล

### 10.3 ข้อเท็จจริงจาก startup log
- ระบบทำงานบน `Windows 64-bit`
- Process ที่รันเป็น `64-bit`
- .NET runtime ที่ใช้งานในช่วง log คือ `10.0.10`
- ระบบสามารถผ่านขั้นตอน:
  - ติดตั้ง global exception handlers
  - เปิดใช้ WinForms subsystem
  - ตรวจพบฐานข้อมูลจริง
  - เรียก `Db.EnsureSchema()` สำเร็จ
  - สร้าง `frmMain` สำเร็จ
- ดังนั้นปัญหาหลักที่ปรากฏใน log ไม่ได้มาจากการบูตระบบไม่ขึ้น แต่เกิดจากคำสั่ง SQL runtime ภายในบางหน้าจอ

### 10.4 บทเรียนเชิงสถาปัตยกรรม
- Microsoft Access ผ่าน OleDb มีข้อจำกัดด้าน SQL syntax มากกว่าที่ query บน Access UI ดูเหมือนจะรองรับ
- ฟังก์ชันที่ใช้ได้ใน Access query designer ไม่ควรถูกสมมติว่าใช้ได้เสมอใน runtime ผ่าน OleDb
- การประกอบ SQL แบบหลาย `LEFT JOIN` ควรทดสอบกับ Access syntax โดยตรง
- การจัดการวันที่ควรใช้ helper กลาง เช่น `AccessDateLiteral(...)`
- การเก็บ critical log และ stack trace มีความสำคัญอย่างยิ่งต่อการวิเคราะห์ปัญหาในภายหลัง

## 11. ประวัติการอัพเดตระบบ

### 11.1 อัพเดตวันที่ 2026-08-01
ครั้งที่ 1 - อัพเดตครั้งแรกจากเอกสารเดิมวันที่ 2026-07-26

#### UI/UX Improvements
- **Dashboard Cards**: ลดความสูงของ pnlCard1-4 จาก 95px เป็น 90px เพื่อให้ dashboard กระชับขึ้น
- **Reports Layout**: ปรับ layout ของฟอร์มรายงานให้กระชับขึ้น:
  - เพิ่มขนาด DataGridView
  - ลด header
  - ปรับขนาด controls และ fonts ให้เหมาะสม
- **Thai Localization**: แปลฟิลด์ DataGridView ใน FrmTransactions เป็นภาษาไทยทั้งหมด:
  - TranDate → วันที่
  - TranTypeDisplay → ชนิด
  - CategoryName → ประเภท
  - FundName → กองทุน
  - BankName → ธนาคาร
  - Detail → รายละเอียด
  - Amount → จำนวนเงิน
  - Note → หมายเหตุ
  - CreateDate → วันที่บันทึก
  - ToFundName → ไปยังกองทุน
  - ToBankName → ไปยังธนาคาร

#### ฟีเจอร์ใหม่
- **ยอดยกมาแบบกำหนดเอง**:
  - เพิ่ม TextBox (txtBalance) สำหรับกรอกยอดยกมาเองใน FrmReports
  - เพิ่ม Button (btnCalcBalance) สำหรับคำนวณยอดยกมาจากฐานข้อมูล
  - เพิ่ม GetManualBalance() function สำหรับอ่านค่าจาก txtBalance
  - แก้ไข IncomeExpenseReport constructor รองรับ manualOpeningBalance parameter
  - เพิ่ม ToolTips สำหรับแนะนำการใช้งาน

#### การแก้ไขปัญหา
- **BC30451 Errors**: แก้ไขปัญหา lblCardXIcon ไม่ถูกประกาศใน Designer โดย comment out บรรทัดที่อ้างถึง
- **Debug Infrastructure**: เพิ่ม debug reporting system ใน FrmTransactions สำหรับติดตามปัญหาการโหลดข้อมูล

#### ไฟล์ที่แก้ไข
- `frmMain.Designer.vb` - ปรับความสูง dashboard cards
- `FrmTransactions.vb` - แปลภาษาฟิลด์เป็นภาษาไทย, เพิ่ม debug reporting
- `FrmReports.Designer.vb` - เพิ่ม txtBalance และ btnCalcBalance, ปรับ layout
- `FrmReports.vb` - เพิ่ม logic สำหรับยอดยกมาแบบกำหนดเอง
- `IncomeExpenseReport.vb` - แก้ไข constructor รองรับ manualOpeningBalance
- `Database.vb` - ปรับปรุง logic การจัดการฐานข้อมูล
