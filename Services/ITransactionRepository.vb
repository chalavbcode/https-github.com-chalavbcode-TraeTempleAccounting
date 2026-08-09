Option Strict Off
Option Explicit On

Imports System.Collections.Generic

Namespace TempleAccounting
    ''' <summary>
    ''' Interface สำหรับบันทึก/โหลดธุรกรรม (Income/Expense/Transfer)
    ''' รองรับการเปลี่ยน Data Provider จาก Access -> SQL Server -> REST API ในอนาคต
    ''' โดยที่ WinForms UI ไม่ต้องแก้ไข
    ''' </summary>
    Public Interface ITransactionRepository

        ''' <summary>บันทึกรายรับใหม่ พร้อม Return TransactionID ที่ถูก Insert</summary>
        Function InsertIncome(tranDate As Date, categoryID As Integer, fundID As Integer,
                              bankID As Integer?, description As String, amount As Decimal,
                              remark As String, receiveChannel As String,
                              referenceNo As String) As Integer

        ''' <summary>อัปเดตรายรับที่มีอยู่แล้ว</summary>
        Sub UpdateIncome(transactionID As Integer, tranDate As Date, categoryID As Integer,
                        fundID As Integer, bankID As Integer?, description As String,
                        amount As Decimal, remark As String, receiveChannel As String,
                        referenceNo As String)

        ''' <summary>อัปเดต ReceiptPath สำหรับ Transaction</summary>
        Sub UpdateReceiptPath(transactionID As Integer, receiptPath As String)

    End Interface
End Namespace
