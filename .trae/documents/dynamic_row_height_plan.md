# แผนการปรับปรุงรายงานให้รองรับความสูงแถวแบบไดนามิก (Dynamic Row Height)

แผนงานนี้มีวัตถุประสงค์เพื่อปรับปรุงการวาดตารางในรายงานสรุปบัญชีรายรับ-รายจ่าย ให้รองรับการขึ้นบรรทัดใหม่ (Text Wrapping) เมื่อคำอธิบายยาวเกินไป และทำให้ความสูงของแถวในฝั่งรายรับและรายจ่ายเท่ากันเสมอ เพื่อป้องกันช่องว่างขาวกลางตารางและจัดการการแบ่งหน้า (Pagination) ให้มีประสิทธิภาพมากขึ้น

## 1. การวิเคราะห์สถานะปัจจุบัน (Current State Analysis)
- ไฟล์ที่เกี่ยวข้องหลัก: [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb) และ [ReportEngine.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/ReportEngine.vb)
- ปัจจุบันมีการใช้ค่าความสูงแถวคงที่ `rowH = 28`
- มีการจำกัดจำนวนแถวไว้ที่ 15 แถวต่อหน้า (`maxRowsPerPage = 15`)
- ข้อความที่ยาวเกินไปจะถูกตัดด้วยฟังก์ชัน `Truncate` แทนการขึ้นบรรทัดใหม่

## 2. การเปลี่ยนแปลงที่เสนอ (Proposed Changes)

### **2.1 ปรับปรุง [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb)**
- **ลบข้อจำกัดจำนวนแถว:** นำ `maxRowsPerPage = 15` ออก เพื่อให้ระบบคำนวณจำนวนแถวตามพื้นที่จริงที่เหลือในแต่ละหน้า
- **คำนวณจำนวนแถวสูงสุด (Task 1):** ใช้ `totalRows = Math.Max(_incomeRows.Count, _expenseRows.Count)` เพื่อเป็นเกณฑ์ในการวนลูปข้อมูลทั้งหมด
- **ปรับปรุงลูปการวาดแถวใน `OnPrintPage`:**
    - เปลี่ยนจากลูป `For` 15 ครั้ง เป็นลูปที่ตรวจสอบพื้นที่ว่างจริง
    - ในแต่ละแถว ให้วัดความสูงของคำอธิบาย (Description) ทั้งฝั่งรายรับและรายจ่ายโดยใช้ `g.MeasureString` ร่วมกับความกว้างคอลัมน์ที่กำหนด
    - กำหนดความสูงแถวปัจจุบัน `currentRowH = Math.Max(28, Math.Max(incomeDescHeight, expenseDescHeight))`
    - ตรวจสอบว่า `_pageY + currentRowH` เกินขอบล่างของหน้าหรือไม่ หากเกินให้ขึ้นหน้าใหม่
    - วาดเซลล์ทั้งสองฝั่งด้วยความสูง `currentRowH` ที่เท่ากัน เพื่อให้เส้นตารางเชื่อมต่อกันอย่างสวยงาม
- **รองรับ Text Wrapping:** เปลี่ยนจากการใช้ `Truncate` เป็นการวาดข้อความลงใน `RectangleF` โดยใช้ `StringFormat` ที่รองรับการตัดคำ

### **2.2 ปรับปรุง [ReportEngine.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/ReportEngine.vb)**
- เพิ่มฟังก์ชัน Helper สำหรับการวัดความสูงข้อความภาษาไทย (Thai Text Measurement) เพื่อความแม่นยำในการคำนวณพื้นที่

## 3. ขั้นตอนการดำเนินการ (Implementation Steps)

### **Task 1: คำนวณจำนวนแถวที่มีข้อมูลจริง**
- เปิด [IncomeExpenseReport.vb](file:///c:/Users/fantasy/Documents/TempleAccounting_FullProject_20260726_090422/Reports/IncomeExpenseReport.vb)
- ตรวจสอบและยืนยันการใช้ `totalRows = Math.Max(_incomeRows.Count, _expenseRows.Count)`

### **Task 2: ปรับปรุง Logic การวัดความสูงและการวาดแถว**
- แก้ไขลูปใน `OnPrintPage` ให้ใช้ความสูงแบบ Dynamic
- ปรับปรุงการวาด `g.DrawString` ให้รองรับหลายบรรทัด

### **Task 3: ปรับปรุงการแบ่งหน้า (Pagination)**
- ปรับ Logic การตรวจสอบพื้นที่สำหรับสรุปท้ายหน้า (Summary) และลายเซ็น (Signature) ให้สัมพันธ์กับตำแหน่ง `_pageY` ล่าสุด

## 4. การตรวจสอบความถูกต้อง (Verification)
- **Clean & Rebuild Solution:** เพื่อให้แน่ใจว่าไม่มีข้อผิดพลาดในการ Compile
- **Test Case 1 (Long Description):** ใส่คำอธิบายยาวๆ ในรายรับหรือรายจ่าย เพื่อดูว่าแถวขยายความสูงและฝั่งตรงข้ามขยายตามหรือไม่
- **Test Case 2 (Multi-page):** ทดสอบรายงานที่มีข้อมูลจำนวนมาก เพื่อดูการตัดหน้ากระดาษว่าถูกต้องและไม่มีช่องว่างขาวทิ้งไว้
- **Test Case 3 (Boundary):** ทดสอบกรณีที่แถวสุดท้ายเกือบจะตกหน้าพอดี เพื่อดูว่าลายเซ็นถูกผลักไปหน้าถัดไปอย่างถูกต้องหรือไม่

## 5. การตัดสินใจและข้อสมมติฐาน (Decisions & Assumptions)
- ใช้ฟอนต์เดิม (Tahoma 10pt) เป็นเกณฑ์ในการวัดขนาด
- ความสูงขั้นต่ำของแถวคือ 28 พิกเซล เพื่อความสวยงาม
- การแบ่งหน้าจะยังคงเว้นพื้นที่สำหรับส่วนสรุปและลายเซ็นตามที่กำหนดไว้ใน `CalculateFinalContentHeight`
