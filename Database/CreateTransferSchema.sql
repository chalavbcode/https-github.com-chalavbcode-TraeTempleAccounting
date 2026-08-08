-- ============================================================================
-- CreateTransferSchema.sql
-- โครงสร้างตารางฝั่ง "ส่งออกข้อมูลไปเครื่องอื่น" (Export Data for Transfer)
-- + คอลัมน์ TransactionGUID ในตาราง Transactions (non-breaking, nullable)
--
-- หมายเหตุ:
--   - โค้ดใน FrmExportForTransfer.vb (EnsureExportSchema) จะสร้างตารางเหล่านี้
--     ให้อัตโนมัติเมื่อเปิดฟอร์ม เช่นเดียวกับฝั่ง import (EnsureImportSchema)
--   - สคริปต์นี้ใช้สำหรับเครื่องมือ DDL ภายนอก หรือตรวจสอบ/สร้างด้วยมือ
--   - คอลัมน์ TransactionGUID ถูกเพิ่มโดย Db.EnsureSchema() ที่ startup ด้วยเช่นกัน
-- ============================================================================

-- 1) LocalSettings: เก็บค่าเฉพาะเครื่องที่ตั้งครั้งเดียว (เช่น MachineID = "KIOSK-01")
CREATE TABLE LocalSettings (
    ID       COUNTER PRIMARY KEY,
    KeyName  TEXT(50)  NOT NULL,
    KeyValue TEXT(255)
);

-- 2) ExportHistory: บันทึกการส่งออกที่สำเร็จทุกครั้ง
--    (ฝั่งคู่กับ ImportHistory) ใช้สำหรับ "ส่งออกเฉพาะรายการใหม่หลังการส่งออกครั้งล่าสุด"
CREATE TABLE ExportHistory (
    ExportBatchID    GUID PRIMARY KEY,
    ExportDateTime   DATETIME,
    DateRangeFrom    DATETIME,
    DateRangeTo      DATETIME,
    RowCount         INTEGER,
    FileNameGenerated TEXT(255),
    MachineID        TEXT(50)
);

-- 3) คอลัมน์ TransactionGUID ใน Transactions (nullable — แถวเก่าที่ไม่มีจะเป็น NULL
--    และฝั่ง import จะ fallback ไปใช้ composite key ตามสเปค)
--    ถ้าคอลัมน์มีอยู่แล้ว ข้ามขั้นตอนนี้ได้ (ตรวจสอบก่อน)
ALTER TABLE Transactions ADD COLUMN TransactionGUID TEXT(36);

-- 4) คอลัมน์ที่เกี่ยวข้องฝั่ง import (อ้างอิงจาก Spec FrmMultiMachineImport)
--    ถ้าคอลัมน์มีอยู่แล้ว ข้ามขั้นตอนนี้ได้ (ตรวจสอบก่อน)
ALTER TABLE Transactions ADD COLUMN SourceMachineID TEXT(50);
ALTER TABLE Transactions ADD COLUMN ImportBatchID GUID;

-- ============================================================================
-- (ทางเลือก) FK traceability: จาก Transactions.ImportBatchID -> ImportHistory.ImportBatchID
-- Access ผ่าน OLEDB ไม่บังคับ FK ในทุกกรณี; คอลัมน์เป็นแค่การอ้างอิงเชิงเอกสาร
-- ALTER TABLE Transactions ADD CONSTRAINT FK_Transactions_ImportBatch
--     FOREIGN KEY (ImportBatchID) REFERENCES ImportHistory (ImportBatchID);
-- ============================================================================
