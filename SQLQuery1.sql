USE [TempleAccountingDb];
GO

-- 1. ตารางบันทึกรายการรับ-จ่าย
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Transactions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Transactions](
    [ID] INT IDENTITY(1,1) PRIMARY KEY,
    [TransactionDate] DATETIME NOT NULL,
    [Category] NVARCHAR(100) NOT NULL,
    [Detail] NVARCHAR(500) NULL,
    [Income] DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    [Expense] DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    [Remark] NVARCHAR(255) NULL
);
END
GO

-- 2. ตารางหมวดหมู่รายรับ-รายจ่าย
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Categories](
    [CategoryID] INT IDENTITY(1,1) PRIMARY KEY,
    [CategoryName] NVARCHAR(100) NOT NULL,
    [CategoryType] NVARCHAR(20) NOT NULL -- 'Income' หรือ 'Expense'
);
END
GO

-- 3. ตารางข้อมูลวัดแหลมยางและผู้ลงนาม
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TempleInfo]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TempleInfo](
    [ID] INT PRIMARY KEY DEFAULT 1,
    [TempleName] NVARCHAR(150) NOT NULL DEFAULT N'วัดแหลมยาง',
    [AbbotName] NVARCHAR(150) NULL,
    [TreasurerName] NVARCHAR(150) NULL
);

-- ใส่ข้อมูลตั้งต้นของวัดแหลมยาง
INSERT INTO [dbo].[TempleInfo] ([ID], [TempleName], [AbbotName], [TreasurerName])
VALUES (1, N'วัดแหลมยาง', N'เจ้าอาวาสวัดแหลมยาง', N'ไวยาวัจกรวัดแหลมยาง');
END
GO