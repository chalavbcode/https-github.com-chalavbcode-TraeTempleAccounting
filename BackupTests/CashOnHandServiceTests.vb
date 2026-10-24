Option Strict Off
Option Explicit On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Collections.Generic

Namespace TempleAccounting

    ''' <summary>
    ''' ทดสอบการคำนวณเงินสดในมือแบบบริสุทธิ์ (CashOnHandService)
    ''' ครอบคลุม: ไม่มีการนับ, รายรับ/รายจ่าย, โอนธนาคาร→เงินสด, รายการย้อนหลัง, นับสองครั้ง, ปี พ.ศ., วันเดียวกับวันนับ
    ''' </summary>
    <TestClass>
    Public Class CashOnHandServiceTests

        Private Function CountOf(d As Date, total As Decimal) As CashCountRecord
            Return New CashCountRecord() With {.CountDate = d, .TotalCounted = total}
        End Function

        Private Function Row(d As Date, ttype As String, amount As Decimal,
                             Optional receiveChannel As String = Nothing,
                             Optional bankID As Integer? = Nothing,
                             Optional fundID As Integer? = Nothing,
                             Optional toFundID As Integer? = Nothing,
                             Optional toBankID As Integer? = Nothing) As TransactionRow
            Return New TransactionRow() With {
                .TranDate = d, .TranType = ttype, .Amount = amount,
                .ReceiveChannel = receiveChannel,
                .BankID = bankID, .FundID = fundID, .ToFundID = toFundID, .ToBankID = toBankID}
        End Function

        ' ── ClassifyCashAmount: กฎการจำแนกเงินสด ──

        <TestMethod>
        Public Sub Classify_IncomeWithCashChannel_Positive()
            Assert.AreEqual(100D, CashOnHandService.ClassifyCashAmount("Income", "Cash", Nothing, Nothing, Nothing, Nothing, 100D))
        End Sub

        <TestMethod>
        Public Sub Classify_IncomeWithBankChannel_Zero()
            Assert.AreEqual(0D, CashOnHandService.ClassifyCashAmount("Income", "โอนธนาคาร", Nothing, Nothing, Nothing, Nothing, 100D))
        End Sub

        <TestMethod>
        Public Sub Classify_IncomeWithNullChannel_CountsAsCash()
            Assert.AreEqual(100D, CashOnHandService.ClassifyCashAmount("Income", Nothing, Nothing, Nothing, Nothing, Nothing, 100D))
        End Sub

        <TestMethod>
        Public Sub Classify_ExpenseWithoutBank_Negative()
            Assert.AreEqual(-100D, CashOnHandService.ClassifyCashAmount("Expense", Nothing, Nothing, Nothing, Nothing, Nothing, 100D))
        End Sub

        <TestMethod>
        Public Sub Classify_ExpenseWithBank_Zero()
            Assert.AreEqual(0D, CashOnHandService.ClassifyCashAmount("Expense", Nothing, 2, Nothing, Nothing, Nothing, 100D))
        End Sub

        <TestMethod>
        Public Sub Classify_TransferBankToCash_Positive()
            Assert.AreEqual(20000D, CashOnHandService.ClassifyCashAmount("Transfer", Nothing, 2, Nothing, 1, Nothing, 20000D))
        End Sub

        <TestMethod>
        Public Sub Classify_TransferCashToBank_Negative()
            Assert.AreEqual(-20000D, CashOnHandService.ClassifyCashAmount("Transfer", Nothing, Nothing, 1, Nothing, 2, 20000D))
        End Sub

        ' ── ComputeCashOnHand: สถานการณ์หลัก ──

        <TestMethod>
        Public Sub NoCount_ReturnsNotSet()
            Dim counts As New List(Of CashCountRecord)()
            Dim rows As New List(Of TransactionRow)()
            Dim result = CashOnHandService.ComputeCashOnHand(New Date(2026, 5, 1), counts, rows)
            Assert.IsFalse(result.HasValue, "ไม่มีการนับ ต้องคืน 'ยังไม่ตั้งค่า' (Nothing)")
        End Sub

        <TestMethod>
        Public Sub CountThenIncomeAndExpense_AddsIncomeSubtractsExpense()
            Dim counts As New List(Of CashCountRecord)() From {CountOf(New Date(2026, 4, 30), 26820D)}
            Dim rows As New List(Of TransactionRow)() From {
                Row(New Date(2026, 5, 2), "Income", 1000D, receiveChannel:="Cash"),
                Row(New Date(2026, 5, 3), "Expense", 250D)   ' BankID = Nothing → รายจ่ายเงินสด
            }
            Dim result = CashOnHandService.ComputeCashOnHand(New Date(2026, 5, 10), counts, rows)
            Assert.IsTrue(result.HasValue)
            Assert.AreEqual(27570D, result.Value)
        End Sub

        <TestMethod>
        Public Sub BankToCashTransfer_AddsAmount()
            Dim counts As New List(Of CashCountRecord)() From {CountOf(New Date(2026, 4, 30), 26820D)}
            ' bankID = 2 (ธนาคาร) → toFundID = 1 (เงินสด) = โอนธนาคาร→เงินสด
            Dim rows As New List(Of TransactionRow)() From {
                Row(New Date(2026, 5, 1), "Transfer", 20000D, bankID:=2, toFundID:=1)
            }
            Dim result = CashOnHandService.ComputeCashOnHand(New Date(2026, 5, 10), counts, rows)
            Assert.IsTrue(result.HasValue)
            Assert.AreEqual(46820D, result.Value)
        End Sub

        <TestMethod>
        Public Sub BackDatedRow_NotCounted()
            Dim counts As New List(Of CashCountRecord)() From {CountOf(New Date(2026, 4, 30), 26820D)}
            Dim rows As New List(Of TransactionRow)() From {
                Row(New Date(2026, 4, 10), "Income", 500D, receiveChannel:="Cash")   ' ก่อนวันนับ → ไม่นับ
            }
            Dim result = CashOnHandService.ComputeCashOnHand(New Date(2026, 5, 10), counts, rows)
            Assert.IsTrue(result.HasValue)
            Assert.AreEqual(26820D, result.Value)
        End Sub

        <TestMethod>
        Public Sub TwoCounts_UsesLatestCountAsBase_AndSkipsBetweenCountRows()
            Dim counts As New List(Of CashCountRecord)() From {
                CountOf(New Date(2026, 4, 30), 26820D),
                CountOf(New Date(2026, 6, 9), 19390D)
            }
            Dim rows As New List(Of TransactionRow)() From {
                Row(New Date(2026, 5, 10), "Income", 500D, receiveChannel:="Cash"),    ' อยู่ระหว่างสองการนับ → รวมอยู่ในการนับครั้งที่สองแล้ว
                Row(New Date(2026, 6, 10), "Income", 1000D, receiveChannel:="Cash")   ' หลังการนับครั้งที่สอง → นับเพิ่ม
            }
            Dim result = CashOnHandService.ComputeCashOnHand(New Date(2026, 6, 15), counts, rows)
            Assert.IsTrue(result.HasValue)
            Assert.AreEqual(20390D, result.Value)
        End Sub

        <TestMethod>
        Public Sub BuddhistEraYear_IsNormalized()
            Dim counts As New List(Of CashCountRecord)() From {CountOf(New Date(2026, 4, 30), 26820D)}
            ' ปี พ.ศ. 2569 = ค.ศ. 2026 (เก็บเป็นปีพุทธศักราชในฐานข้อมูล)
            Dim rows As New List(Of TransactionRow)() From {
                Row(New Date(2569, 5, 2), "Income", 1000D, receiveChannel:="Cash")
            }
            Dim result = CashOnHandService.ComputeCashOnHand(New Date(2026, 5, 10), counts, rows)
            Assert.IsTrue(result.HasValue)
            Assert.AreEqual(27820D, result.Value)
        End Sub

        <TestMethod>
        Public Sub SameDayRow_IsNotCounted()
            Dim counts As New List(Of CashCountRecord)() From {CountOf(New Date(2026, 4, 30), 26820D)}
            ' วันเดียวกับวันนับ (นับปลายวัน → รวมอยู่แล้ว) → ไม่นับเพิ่ม
            Dim rows As New List(Of TransactionRow)() From {
                Row(New Date(2026, 4, 30), "Income", 1000D, receiveChannel:="Cash")
            }
            Dim result = CashOnHandService.ComputeCashOnHand(New Date(2026, 5, 10), counts, rows)
            Assert.IsTrue(result.HasValue)
            Assert.AreEqual(26820D, result.Value)
        End Sub

    End Class
End Namespace
