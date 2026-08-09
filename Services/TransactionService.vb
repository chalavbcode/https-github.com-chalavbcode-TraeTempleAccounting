Option Strict Off
Option Explicit On

Imports System
Imports System.Windows.Forms

Namespace TempleAccounting
    ''' <summary>
    ''' Business Logic Layer สำหรับบันทึกรายรับ
    ''' แยก Validation / Business Rules ออกจาก UI และ Data Access
    ''' </summary>
    Public Class TransactionService
        Private ReadOnly _repo As ITransactionRepository

        ''' <summary>
        ''' Constructor — รับ Repository ตาม Data Provider ปัจจุบัน (Access / SQL / REST)
        ''' UI Layer ไม่ต้องรู้ว่า Repository เป็น Access หรืออย่างอื่น
        ''' </summary>
        Public Sub New(repository As ITransactionRepository)
            _repo = repository
        End Sub

        ''' <summary>
        ''' บันทึกรายรับใหม่ — validate ก่อนส่งให้ Repository
        ''' Return TransactionID ที่ถูก Insert (ใช้สำหรับอัปโหลดรูปใบเสร็จต่อ)
        ''' </summary>
        Public Function SaveIncome(tranDate As Date, categoryID As Integer, fundID As Integer,
                                    bankID As Integer?, description As String, amount As Decimal,
                                    remark As String, receiveChannel As String,
                                    referenceNo As String) As Integer

            ' Validate
            If categoryID <= 0 Then Throw New ArgumentException("กรุณาเลือกประเภทรายรับ")
            If fundID <= 0 Then Throw New ArgumentException("กรุณาเลือกกองทุน")
            If amount <= 0 Then Throw New ArgumentException("กรุณากรอกจำนวนเงินที่ถูกต้อง")
            If String.IsNullOrWhiteSpace(description) Then
                Throw New ArgumentException("กรุณากรอกรายละเอียดรายการ")
            End If

            Return _repo.InsertIncome(tranDate, categoryID, fundID, bankID,
                                       description, amount, remark,
                                       receiveChannel, referenceNo)
        End Function

        ''' <summary>
        ''' อัปเดตรายรับที่มีอยู่แล้ว
        ''' </summary>
        Public Sub UpdateIncome(transactionID As Integer, tranDate As Date, categoryID As Integer,
                                fundID As Integer, bankID As Integer?, description As String,
                                amount As Decimal, remark As String, receiveChannel As String,
                                referenceNo As String)

            If categoryID <= 0 Then Throw New ArgumentException("กรุณาเลือกประเภทรายรับ")
            If fundID <= 0 Then Throw New ArgumentException("กรุณาเลือกกองทุน")
            If amount <= 0 Then Throw New ArgumentException("กรุณากรอกจำนวนเงินที่ถูกต้อง")

            _repo.UpdateIncome(transactionID, tranDate, categoryID, fundID,
                               bankID, description, amount, remark,
                               receiveChannel, referenceNo)
        End Sub

        ''' <summary>
        ''' อัปเดต ReceiptPath หลังบันทึกรายการและจัดการรูปภาพสำเร็จ
        ''' </summary>
        Public Sub UpdateReceiptPath(transactionID As Integer, receiptPath As String)
            _repo.UpdateReceiptPath(transactionID, receiptPath)
        End Sub
    End Class
End Namespace
