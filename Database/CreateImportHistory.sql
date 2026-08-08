-- =============================================================================
--  Script: สร้างตาราง ImportHistory และเพิ่มคอลัมน์ที่จำเป็นในตาราง Transactions
--  สำหรับ: FrmMultiMachineImport.vb (นำเข้าและรวมข้อมูลจากหลายเครื่อง/หลาย Flash Drive)
--  ลักษณะ: Non-breaking — ไม่ลบ/ไม่แก้คอลัมน์เดิมของตารางใด มีแต่เพิ่มเท่านั้น
--
--  วิธีใช้:
--    1. เปิดไฟล์ TempleAccounting.accdb ด้วย Microsoft Access
--    2. สร้าง Query ใหม่ในมุมมอง SQL แล้ววางคำสั่งด้านล่าง Execute
--    หรือไม่ต้องรันก็ได้: FrmMultiMachineImport จะตรวจสอบและสร้าง schema
--    ให้อัตโนมัติเมื่อเปิดฟอร์ม (ฟังก์ชัน EnsureImportSchema)
-- =============================================================================

-- 1) สร้างตาราง ImportHistory (สร้างใหม่เฉพาะเมื่อยังไม่มี — ไม่กระทบตารางเดิม)
CREATE TABLE ImportHistory (
    ImportBatchID            GUID PRIMARY KEY,    -- รหัสการนำเข้าแต่ละครั้ง (batch)
    SourceFileName           TEXT(255),           -- ชื่อไฟล์ตอนเลือก (เพื่ออ้างอิง ไม่ใช่ตัวระบุหลัก)
    FileHash                 TEXT(64) NOT NULL,   -- SHA-256 ของไฟล์ทั้งไฟล์
    MachineID                TEXT(50),            -- รหัสเครื่องต้นทางที่อ่านได้จากไฟล์ (ถ้ามี)
    ImportDateTime           DATETIME,            -- วันเวลาที่ merge สำเร็จ
    ImportedBy               TEXT(100),           -- ผู้ใช้ที่ทำรายการ
    RowCountTotal            INTEGER,             -- จำนวนแถวทั้งหมดในไฟล์
    RowCountImported         INTEGER,             -- จำนวนแถวที่ merge เข้าจริง
    RowCountSkippedDuplicate INTEGER,             -- จำนวนแถวที่ข้ามเพราะซ้ำ
    TotalAmountImported      CURRENCY,            -- ยอดรวมเงินที่นำเข้าจริง
    ImportStatus             TEXT(20)             -- สถานะ: Success / Failed / Cancelled
);

-- 2) เพิ่มคอลัมน์ในตาราง Transactions (nullable, default NULL, non-breaking)
--    ImportBatchID   : อ้างอิงกลับไปที่ ImportHistory (สืบย้อนว่าแถวนี้มาจากการ import ครั้งไหน)
--    TransactionGUID : รหัสธุรกรรมต้นทาง (ใช้ตรวจสอบซ้ำแม่นยำสุด กรณีไฟล์ export มี field นี้)
--    SourceMachineID : รหัสเครื่องต้นทางของแถวนี้ (ใช้ใน composite key กันชนข้ามเครื่อง)
ALTER TABLE Transactions ADD COLUMN ImportBatchID GUID;
ALTER TABLE Transactions ADD COLUMN TransactionGUID TEXT(36);
ALTER TABLE Transactions ADD COLUMN SourceMachineID TEXT(50);

-- 3) [ทางเลือก] สร้าง Foreign Key เพื่อบังคับความถูกต้องของการอ้างอิง
--    หมายเหตุ: ถ้า Access แจ้ง error ให้ข้ามข้อนี้ได้ (ค่า NULL ของข้อมูลเดิมไม่กระทบ
--    และ FrmMultiMachineImport ตรวจสอบความสอดคล้องเองด้วย ImportBatchID อยู่แล้ว)
-- ALTER TABLE Transactions ADD CONSTRAINT FK_Transactions_ImportHistory
--     FOREIGN KEY (ImportBatchID) REFERENCES ImportHistory (ImportBatchID);
