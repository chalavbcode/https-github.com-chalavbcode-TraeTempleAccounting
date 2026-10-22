Option Strict Off
Option Explicit On

Imports System

Namespace TempleAccounting
    ''' <summary>
    ''' นิยามรูปแบบไฟล์ Transfer (ส่งออก/นำเข้า ข้ามเครื่อง) แบบกลาง
    ''' ใช้ร่วมกันทั้ง FrmExportForTransfer (ฝั่งเขียน) และ FrmMultiMachineImport (ฝั่งอ่าน)
    ''' เพื่อกันโครงสร้างไฟล์ไม่ตรงกัน (Schema drift) เมื่อโปรแกรมแต่ละเครื่องอัปเดตไม่พร้อมกัน
    ''' </summary>
    Public Module TransferFormat
        ' เวอร์ชันโครงสร้างไฟล์ export — ต้องอัปเดตพร้อมกันทั้งฝั่ง export และ import
        ' เมื่อใดก็ตามที่รูปแบบไฟล์เปลี่ยน (เพิ่ม/เปลี่ยนคอลัมน์, เปลี่ยน Metadata)
        Public Const SchemaVersion As String = "1.0"

        ' ชื่อชีตภายในไฟล์ .xlsx (import จะอ่าน Metadata ก่อนเปิดข้อมูลจริง)
        Public Const MetadataSheetName As String = "Metadata"
        Public Const DataSheetName As String = "Data"

        ' ชื่อฟิลด์ภายในชีต Metadata (FieldName / FieldValue)
        Public Const FieldSchemaVersion As String = "SchemaVersion"
        Public Const FieldMachineID As String = "MachineID"
        Public Const FieldExportDateTime As String = "ExportDateTime"
        Public Const FieldRowCount As String = "RowCount"
        Public Const FieldDateRangeFrom As String = "DateRangeFrom"
        Public Const FieldDateRangeTo As String = "DateRangeTo"

        ' คอลัมน์ในชีต Data — ชื่อต้องตรงกับ alias ที่ FrmMultiMachineImport ใช้ FindColumn อ่าน
        ' (การหาเจอเป็นแบบ case-insensitive + ตัดอักขระพิเศษ ดังนั้น ลำดับไม่สำคัญ แต่คงไว้เป็นมาตรฐาน)
        ' หมายเหตุ: ใช้ "Remark" แทน "Note" (Note เป็น reserved word ใน Access DDL ตอนสร้างชีต)
        Public ReadOnly DataColumns As String() = {
            "TransactionGUID", "TranDate", "Type", "Description",
            "Amount", "Category", "Fund", "Bank", "Remark"
        }
    End Module
End Namespace
