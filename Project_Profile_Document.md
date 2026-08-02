# Project Profile Document

## TempleAccounting (ระบบบัญชีวัด)

| รายการ | รายละเอียด |
|---|---|
| **เวอร์ชัน** | 1.5.0 |
| **วันที่ปล่อย** | 2026-07-31 |
| **สถานะ** | Active Development |
| **ภาษาหลัก** | Thai |

---

## 1. บทสรุปผู้บริหาร (Executive Summary)

**TempleAccounting** คือระบบบัญชีสำหรับวัดไทยที่พัฒนาด้วย **VB.NET WinForms** และฐานข้อมูล **MS Access (.accdb)** ผ่าน **OleDb** ระบบนี้ช่วยให้วัดสามารถบันทึกรายรับ (เงินบริจาค) รายจ่าย (ค่าใช้จ่าย) และการโอนเงินภายในระหว่างกองทุนหรือบัญชีธนาคารต่างๆ ได้อย่างเป็นระบบ พร้อมจัดพิมพ์รายงานภาษี รายงานรายเดือน และสมุดบัญชีรายวันที่สอดคล้องกับมาตรฐานบัญชีสำหรับวัด

**เทคโนโลยีหลักที่ใช้:**
- **VB.NET 10.0 / .NET** สำหรับ Windows Desktop Application
- **MS Access Database (.accdb)** ผ่าน System.Data.OleDb
- **GDI+ / System.Drawing** สำหรับการพิมพ์รายงานและประมวลผลรูปภาพ
- **Git / GitHub** สำหรับการจัดการซอร์สโค้ดและ Version Control
- **Visual Studio 2022** สำหรับการพัฒนาและ Debugging

---

## 2. โครงสร้างระบบไฟล์ (File System Architecture)

ระบบใช้คลาสกลาง **AppPaths** ในการกำหนดเส้นทางโฟลเดอร์ที่จำเป็นทั้งหมด โดยทุกเส้นทางจะอยู่ภายใต้โฟลเดอร์หลักของโปรเจกต์

```
📁 TempleAccounting/
├── 📁 Database/
│   └── TempleAccounting.accdb          # ฐานข้อมูล Access หลัก
├── 📁 Receipts/
│   └── Receipt_*.jpg                    # ไฟล์รูปภาพใบเสร็จ (ย่อแล้ว)
├── 📁 Backup/
│   └── *.accdb                         # สำรองฐานข้อมูลอัตโนมัติ
├── 📁 Export/
│   └── *.xlsx                          # ส่งออกรายงานเป็น Excel
├── 📁 Logs/
│   ├── app.log                         # Log การทำงานทั่วไป
│   └── crash.log                       # Log การเกิด Error/Crash
├── 📁 Import/
│   ├── province.csv                    # ข้อมูลจังหวัด (สำหรับนำเข้า)
│   ├── amphoe.csv                       # ข้อมูลอำเภอ
│   └── tambon.csv                      # ข้อมูลตำบล
└── 📁 obj/ / 📁 bin/                   # Build artifacts (ไม่ต้อง commit)
```

**นโยบายการจัดการโฟลเดอร์:**
- โฟลเดอร์ `Receipts/` เก็บเฉพาะไฟล์รูปภาพใบเสร็จที่ผ่านการย่อแล้ว (ไม่เก็นไฟล์ต้นฉบับ)
- โฟลเดอร์ `Backup/` สำหรับสำรองฐานข้อมูลอัตโนมัติก่อนทำการ Import ข้อมูลจำนวนมาก
- โฟลเดอร์ `Logs/` สำหรับบันทึกประวัติการทำงานและการเกิด Error เพื่อการตรวจสอบย้อนหลัง
- **ไม่ commit โฟลเดอร์ `obj/` และ `bin/` เข้า Git** เพื่อป้องกันปัญหา File Lock และ Error ตอน Build

---

## 3. โครงสร้างฐานข้อมูล (Database Schema)

ฐานข้อมูล Access ใช้โมเดล **Flat Table Design** ที่เน้นความเรียบง่ายและเข้ากันได้กับ MS Access โดยใช้ฟังก์ชัน `EnsureSchema()` ในการ Bootstrap Schema อัตโนมัติเมื่อโปรแกรมเริ่มทำงาน

**ตารางหลัก (Core Tables):**

| ตาราง | คำอธิบาย | ฟิลด์สำคัญ |
|---|---|---|
| `Transactions` | รายการรายรับ/รายจ่าย/โอนเงิน | ID, TranDate, TranType, Amount, Detail, ReceiptPath |
| `Categories` | ประเภทรายการ (รายรับ/รายจ่าย) | CategoryID, CategoryName, CategoryType |
| `Funds` | กองทุนภายในวัด | FundID, FundName |
| `Banks` | บัญชีธนาคาร | BankID, BankName, AccountNo |
| `Province/District/SubDistrict` | ข้อมูลที่อยู่ประเทศไทย | สำหรับการนำเข้าจาก CSV |
| `TempleSetting` | ข้อมูลวัดและบุคลากร | ชื่อวัด, เจ้าอาวาส, ไวยาวัจกร |

**การอัปเกรด Schema ล่าสุด (v1.5.0):**
- เพิ่มคอลัมน์ `ReceiptPath TEXT(255)` ในตาราง `Transactions` สำหรับเก็บชื่อไฟล์รูปภาพใบเสร็จ (เก็บเฉพาะชื่อไฟล์ ไม่เก็บ Binary ลงใน Access)

---

## 4. ฟีเจอร์หลักและการอัปเดตล่าสุด (Key Features & Latest Updates)

### 4.1 ระบบจัดการใบเสร็จ (Receipts Management System)

**ภาพรวม:** ระบบใบเสร็จถูกออกแบบให้แนบรูปภาพหลักฐานการรับ-จ่ายเงินโดยไม่ทำให้ไฟล์ Access บวม โดยเก็บไฟล์รูปไว้ในโฟลเดอร์ `Receipts/` และเก็บเฉพาะชื่อไฟล์ในฐานข้อมูล

**แหล่งที่มาของรูปภาพ:**
- **เลือกไฟล์จากเครื่อง:** ผ่านปุ่ม "📂 เลือกรูปภาพ" รองรับไฟล์ .jpg, .png, .bmp
- **วางจาก LINE Desktop:** ผ่านปุ่ม "📋 วางจาก LINE" ดึงรูปภาพจาก Clipboard โดยตรง (Copy รูปใน LINE แล้วกดวาง)
- **แนบย้อนหลัง:** จากหน้า FrmTransactions สามารถแนบรูปให้รายการเดิมที่ยังไม่มีรูปได้

**การประมวลผลรูปภาพ (Image Optimization):**
```vb
' ฟังก์ชันหลักใน ReceiptImageHelper.vb
SaveOptimizedReceipt(sourceImage, transactionID):
  - ย่อขนาดถ้ากว้าง/สูงเกิน 1600px (คง Aspect Ratio)
  - แปลงเป็น JPEG Quality 80%
  - บันทึกลง AppPaths.ReceiptsDir เป็น Receipt_{ID}.jpg
  - Return ชื่อไฟล์ (เช่น "Receipt_105.jpg")
```

**ข้อดีของการย่อรูป:**
- ลดขนาดไฟล์จาก 5-10 MB ลงเหลือ 200-500 KB (ประหยัดพื้นที่ ~95%)
- เปิดดูรูปและสำรองข้อมูลเร็วขึ้น
- ป้องกันปัญหาโปรแกรมอืดเมื่อมีรูปภาพสะสมจำนวนมาก

**การจัดการ Clipboard อัจฉริยะ:**
- หลังบันทึกรูปภาพสำเร็จ ระบบจะล้าง Clipboard ทันทีด้วย `Clipboard.Clear()` เพื่อป้องกันการวางรูปเดิมซ้ำในรายการถัดไป
- หาก Clipboard ไม่มีรูปภาพ ระบบจะแจ้งเตือนให้ Copy รูปใหม่ก่อน

**การลบไฟล์ใบเสร็จ:**
- เมื่อลบรายการใน FrmTransactions (ทั้งแถวเดียวหรือหลายแถว) ระบบจะตามไปลบไฟล์รูปจริงในโฟลเดอร์ `Receipts/` ด้วยอัตโนมัติ

### 4.2 การปรับปรุง FrmTransactions

**การเลือกและลบหลายแถวพร้อมกัน (Multi-Row Selection & Deletion):**
```vb
' ตั้งค่า DataGridView
dgvTransactions.MultiSelect = True
dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect

' ลบหลายแถวพร้อมกัน
For Each row In dgvTransactions.SelectedRows
    ' ลบจากฐานข้อมูล
    ' ลบไฟล์ใบเสร็จ (ถ้ามี)
Next
LoadData() ' โหลดข้อมูลใหม่หลังลบ
```

**การค้นหาขั้นสูง (Advanced Partial Search):**
```sql
-- ค้นหาคำบางส่วนในหลายฟิลด์พร้อมกัน (OleDb LIKE wildcard = %)
SELECT ... WHERE (
    t.Detail LIKE @s OR
    t.Note LIKE @s OR
    c.CategoryName LIKE @s OR
    f.FundName LIKE @s OR
    b.BankName LIKE @s OR
    f2.FundName LIKE @s OR
    b2.BankName LIKE @s
)
-- Escape ตัวอักษรพิเศษ ([, ?, #) สำหรับ Access LIKE patterns
```

**การแก้ไขรายการ (Edit Validation Fix):**
- สำหรับรายการประเภท Income/Expense ระบบจะล้างค่า ToFundID และ ToBankID เป็น NULL ก่อน UPDATE
- ป้องกัน Error "รายการรับ/จ่ายทั่วไปไม่ควรมี ToFundID หรือ ToBankID" เมื่อผู้ใช้แก้ไขข้อมูล

### 4.3 ระบบรายงาน (Reporting System)

รายงานถูกสร้างด้วย **GDI+ Drawing** โดยมี 2 รูปแบบหลัก:
- **รายงานแบบละเอียด:** แสดงรายการทุกรายการในช่วงวันที่ที่เลือก
- **รายงานแบบย่อ:** แสดงเฉพาะยอดรวมจำนวนเงินแยกตามประเภท

**ยอดยกมา (Forward Balance):**
- หากผู้ใช้กรอกยอดยกมาในช่อง `txtBalance` ระบบจะใช้ค่านั้นโดยตรง
- หากไม่กรอก (เป็น 0) ระบบจะคำนวณจาก (รายรับ - รายจ่าย) สะสมย้อนหลังถึงวันก่อนวันเริ่มต้นที่เลือก

### 4.4 ระบบ ToolTips

ทุกปุ่มในโปรแกรมมี ToolTip อธิบายหน้าที่เมื่อนำเมาส์ไปชี้ เพื่อเพิ่มความสะดวกในการใช้งานและลดความสับสน

---

## 5. ข้อจำกัดทางเทคนิค (Technical Constraints)

| หัวข้อ | รายละเอียด |
|---|---|
| **Access Database Size** | ไม่แนะนำให้ฐานข้อมูล Access เกิน 2 GB (ใช้โฟลเดอร์ Receipts แทนไฟล์รูปภาพ) |
| **File Lock** | ต้องปิดไฟล์ Access ก่อนทำ Git Commit/Pull/Push |
| **Git Exclusions** | ต้องตรวจสอบว่าโฟลเดอร์ `bin/` และ `obj/` ถูกยกเว้นใน `.gitignore` |
| **EnableDefaultCompileItems** | เมื่อตั้งค่าเป็น `false` ใน .vbproj ต้องลงทะเบียนไฟล์ใหม่ทุกไฟล์ด้วยตนเอง |
| **WinForms Designer** | ต้องใช้ Partial Class (แยก .vb, .Designer.vb, .resx) เพื่อรองรับ Designer 100% |

---

## 6. แนวทางการพัฒนาในอนาคต (Future Roadmap)

| ลำดับ | ฟีเจอร์ | สถานะ |
|---|---|---|
| 1 | รายงานภาษีมูลนิธิ/วัด (ฉบับเต็ม) | รออนุมัติ |
| 2 | ระบบสำรองข้อมูลอัตโนมัติ (Auto Backup) | พิจารณา |
| 3 | รองรับการ Export PDF รายงาน | พิจารณา |
| 4 | ระบบ User Authentication (เข้ารหัสผ่าน) | พิจารณา |
| 5 | รายงานสรุปประจำปีแบบละเอียด | พิจารณา |

---

## 7. Changelog (บันทึกการอัปเดต)

### v1.5.0 (2026-07-31)
**การอัปเดตฟีเจอร์ใหม่:**
- เพิ่มระบบจัดการใบเสร็จ (ReceiptPath) แนบรูปภาพหลักฐานการรับ-จ่าย
- เพิ่ม ReceiptImageHelper สำหรับย่อและบีบอัดรูปภาพ (1600px, JPEG 80%)
- เพิ่มการรองรับ Clipboard/LINE Integration (วางรูปโดยตรง)
- ปรับปรุง FrmTransactions ให้รองรับ Multi-Row Selection & Deletion
- ปรับปรุงการค้นหาให้เป็น Partial Match หลายฟิลด์
- แก้ไข Edit Validation สำหรับรายการ Income/Expense
- เพิ่ม ToolTips ให้ทุกปุ่มในโปรเจกต์
- แก้ไข Mock Data ที่ไม่ตรงกับความจริงใน Dashboard
- ปรับปรุง UI ให้ Designer-friendly

### v1.0.0 (2026-07-26)
**เริ่มต้นโปรเจกต์:**
- ระบบบันทึกรายรับ/รายจ่าย/โอนเงิน
- ระบบจัดการประเภท/กองทุน/บัญชีธนาคาร
- ระบบรายงาน GDI+ (แบบละเอียด/แบบย่อ/สมุดบัญชีรายวัน)
- ระบบ Import ข้อมูลที่อยู่จาก CSV
- ระบบนำเข้าข้อมูลจาก Excel

---

## 8. ข้อมูลสำหรับติดต่อและการบำรุงรักษา

**ผู้พัฒนา:** ทีมพัฒนาระบบบัญชีวัด  
**Repository:** GitHub TempleAccounting_FullProject  
**เอกสารอ้างอิง:** Project_Profile_Document.md  
**วันที่สร้างเอกสาร:** 2026-07-31  
**เวอร์ชันเอกสาร:** 1.5.0

---

*เอกสารนี้จัดทำจากการตรวจสอบซอร์สโค้ดและโครงสร้างโปรเจกต์จริง เพื่อใช้เป็นเอกสารอ้างอิงกลางสำหรับการพัฒนา การบำรุงรักษา และการส่งมอบงานในอนาคต*
