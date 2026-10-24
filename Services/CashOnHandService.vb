Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic

Namespace TempleAccounting
    ''' <summary>
    ''' ข้อมูลการนับเงินสดจริง (physical count checkpoint)
    ''' </summary>
    Public Class CashCountRecord
        Public Property CountDate As Date
        Public Property TotalCounted As Decimal
    End Class

    ''' <summary>
    ''' ข้อมูลธุรกรรมดิบสำหรับคำนวณเงินสด (ยังไม่จำแนกว่าเป็นเงินสดหรือธนาคาร)
    ''' </summary>
    Public Class TransactionRow
        Public Property TranDate As Date
        Public Property TranType As String
        Public Property ReceiveChannel As String
        Public Property BankID As Integer?
        Public Property FundID As Integer?
        Public Property ToFundID As Integer?
        Public Property ToBankID As Integer?
        Public Property Amount As Decimal
    End Class

    ''' <summary>
    ''' คำนวณเงินสดในมือแบบบริสุทธิ์ (pure) — ไม่แตะฐานข้อมูล
    ''' </summary>
    Public Module CashOnHandService
        ''' <summary>
        ''' กองทุนที่ใช้เก็บ "เงินสด" สำหรับจำแนกการโอน (Transfer) — ค่าเริ่มต้น -1 = ยังไม่กำหนด
        ''' ชั้นข้อมูล (CashOnHandRepository) จะ resolve ค่าจริงจากชื่อกองทุน "เงินสด" ทุกครั้งก่อนคำนวณ
        ''' เพราะ FundID ของ "เงินสด" ต่างกันได้ในแต่ละฐานข้อมูล (DB ใหม่ FundID 1 คือ "กองทุนทั่วไป")
        ''' — การ hard-code เลขคงที่จะทำให้จำแนกการโอนผิดเงียบ ๆ
        ''' </summary>
        Public Property CashFundId As Integer = -1

        ''' <summary>
        ''' แปลงปีพุทธศักราช (&gt;2400) เป็นคริสต์ศักราช เช่นเดียวกับ Db.NormalizeGregorianDate
        ''' </summary>
        Public Function NormalizeDate(value As Date) As Date
            If value.Year > 2400 Then
                Return New Date(value.Year - 543, value.Month, value.Day, value.Hour, value.Minute, value.Second)
            End If
            Return value
        End Function

        ''' <summary>
        ''' จำแนกธุรกรรมหนึ่งรายการว่าเป็นการเคลื่อนไหวเงินสดเท่าใด (จำนวนเงินมีเครื่องหมาย)
        ''' รายรับเงินสด: +Amount, รายจ่ายเงินสด: -Amount, โอนธนาคาร→เงินสด: +Amount, เงินสด→ธนาคาร: -Amount
        ''' อย่างอื่นคืน 0
        ''' </summary>
        Public Function ClassifyCashAmount(tranType As String, receiveChannel As String,
                                           bankID As Integer?, fundID As Integer?,
                                           toFundID As Integer?, toBankID As Integer?,
                                           amount As Decimal) As Decimal
            If String.IsNullOrWhiteSpace(tranType) Then Return 0D

            Select Case tranType.Trim().ToUpperInvariant()
                Case "INCOME"
                    If String.IsNullOrEmpty(receiveChannel) OrElse receiveChannel.Trim().Equals("Cash", StringComparison.OrdinalIgnoreCase) Then
                        Return amount
                    End If
                    Return 0D

                Case "EXPENSE"
                    If Not bankID.HasValue Then
                        Return -amount
                    End If
                    Return 0D

                Case "TRANSFER"
                    If bankID.HasValue AndAlso toFundID.HasValue AndAlso toFundID.Value = CashFundId Then
                        Return amount
                    End If
                    If fundID.HasValue AndAlso fundID.Value = CashFundId AndAlso toBankID.HasValue Then
                        Return -amount
                    End If
                    Return 0D

                Case Else
                    Return 0D
            End Select
        End Function

        ''' <summary>
        ''' เงินสดในมือ ณ วันที่ asOf
        ''' = TotalCounted ของการนับล่าสุด (CountDate &lt;= asOf)
        '''   + ผลรวมการเคลื่อนไหวเงินสดที่มี TranDate มากกว่า CountDate นั้น และ &lt;= asOf
        ''' ถ้าไม่มีการนับเลย คืน Nothing ("ยังไม่ตั้งค่า")
        ''' </summary>
        Public Function ComputeCashOnHand(asOf As Date,
                                          counts As IList(Of CashCountRecord),
                                          rows As IList(Of TransactionRow)) As Decimal?
            Dim asOfNorm As Date = NormalizeDate(asOf)

            Dim latest As CashCountRecord = Nothing
            For Each c In counts
                Dim cd As Date = NormalizeDate(c.CountDate)
                If cd <= asOfNorm Then
                    If latest Is Nothing OrElse cd > NormalizeDate(latest.CountDate) Then
                        latest = c
                    End If
                End If
            Next

            If latest Is Nothing Then Return Nothing

            Dim baseDate As Date = NormalizeDate(latest.CountDate)
            Dim total As Decimal = latest.TotalCounted

            For Each r In rows
                Dim md As Date = NormalizeDate(r.TranDate)
                If md > baseDate AndAlso md <= asOfNorm Then
                    total += ClassifyCashAmount(r.TranType, r.ReceiveChannel, r.BankID, r.FundID, r.ToFundID, r.ToBankID, r.Amount)
                End If
            Next

            Return total
        End Function
    End Module
End Namespace
