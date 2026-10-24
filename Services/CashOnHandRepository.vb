Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Data

Namespace TempleAccounting
    ''' <summary>
    ''' ข้อมูลการนับเงินสดจริงแบบเต็ม (แยกมูลค่าแต่ละชนิดธนบัตร/เหรียญ + คาดการณ์ + ผลต่าง)
    ''' หมายเหตุ: ช่อง D1..D1000 เก็บ "มูลค่า" (บาท) ของแต่ละชนิด ไม่ใช่จำนวนใบ/เหรียญ
    ''' </summary>
    Public Class CashCountDetail
        Public Property CountDate As Date
        Public Property D1 As Decimal
        Public Property D2 As Decimal
        Public Property D5 As Decimal
        Public Property D10 As Decimal
        Public Property D20 As Decimal
        Public Property D50 As Decimal
        Public Property D100 As Decimal
        Public Property D500 As Decimal
        Public Property D1000 As Decimal
        Public Property TotalCounted As Decimal
        Public Property ExpectedBefore As Decimal
        Public Property Variance As Decimal
        Public Property Note As String
        Public Property Source As String

        Public Function Denominations() As Decimal()
            Return New Decimal() {D1, D2, D5, D10, D20, D50, D100, D500, D1000}
        End Function
    End Class

    ''' <summary>
    ''' โหลด/บันทึกตาราง CashCounts และ Transactions แล้วคำนวณเงินสดในมือ
    ''' ผ่าน CashOnHandService (pure) — แยกชั้นข้อมูลออกจากตรรกะคำนวณ
    ''' </summary>
    Public Module CashOnHandRepository
        Public Function LoadCounts() As List(Of CashCountDetail)
            Dim result As New List(Of CashCountDetail)()
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn,
                    "SELECT CountDate, D1, D2, D5, D10, D20, D50, D100, D500, D1000, TotalCounted, ExpectedBefore, Variance, [Note], [Source] " &
                    "FROM CashCounts ORDER BY CountDate, ID")
                For Each row As DataRow In dt.Rows
                    If row.IsNull("CountDate") Then Continue For
                    Dim c As New CashCountDetail()
                    c.CountDate = Convert.ToDateTime(row("CountDate"))
                    c.D1 = Db.ToDecimalOrZero(row("D1"))
                    c.D2 = Db.ToDecimalOrZero(row("D2"))
                    c.D5 = Db.ToDecimalOrZero(row("D5"))
                    c.D10 = Db.ToDecimalOrZero(row("D10"))
                    c.D20 = Db.ToDecimalOrZero(row("D20"))
                    c.D50 = Db.ToDecimalOrZero(row("D50"))
                    c.D100 = Db.ToDecimalOrZero(row("D100"))
                    c.D500 = Db.ToDecimalOrZero(row("D500"))
                    c.D1000 = Db.ToDecimalOrZero(row("D1000"))
                    c.TotalCounted = Db.ToDecimalOrZero(row("TotalCounted"))
                    c.ExpectedBefore = Db.ToDecimalOrZero(row("ExpectedBefore"))
                    c.Variance = Db.ToDecimalOrZero(row("Variance"))
                    c.Note = GetText(row("Note"))
                    c.Source = GetText(row("Source"))
                    result.Add(c)
                Next
            End Using
            Return result
        End Function

        Public Function LoadTransactions() As List(Of TransactionRow)
            Dim result As New List(Of TransactionRow)()
            Using conn = Db.OpenConn()
                Dim dt = Db.GetTable(conn,
                    "SELECT TranDate, TranType, ReceiveChannel, BankID, FundID, ToFundID, ToBankID, Amount FROM Transactions")
                For Each row As DataRow In dt.Rows
                    If row.IsNull("TranDate") Then Continue For
                    Dim t As New TransactionRow()
                    t.TranDate = Convert.ToDateTime(row("TranDate"))
                    t.TranType = GetText(row("TranType"))
                    t.ReceiveChannel = GetText(row("ReceiveChannel"))
                    t.BankID = GetIntNullable(row("BankID"))
                    t.FundID = GetIntNullable(row("FundID"))
                    t.ToFundID = GetIntNullable(row("ToFundID"))
                    t.ToBankID = GetIntNullable(row("ToBankID"))
                    t.Amount = Db.ToDecimalOrZero(row("Amount"))
                    result.Add(t)
                Next
            End Using
            Return result
        End Function

        ''' <summary>เงินสดในมือ ณ วันที่ asOf (Nothing ถ้ายังไม่มีการนับเลย)</summary>
        Public Function ComputeCashOnHand(asOf As Date) As Decimal?
            CashOnHandService.CashFundId = ResolveCashFundId()
            Return CashOnHandService.ComputeCashOnHand(asOf, ToRecords(LoadCounts()), LoadTransactions())
        End Function

        ''' <summary>
        ''' หา FundID ของกองทุน "เงินสด" จากชื่อ (ไม่ใช่เลขคงที่) — คืน -1 ถ้าไม่มีกองทุนนี้
        ''' เพื่อให้การจำแนกการโอนไม่พึ่งพา ID ที่บังเอิญตรงกันในบางฐานข้อมูล
        ''' </summary>
        Private Function ResolveCashFundId() As Integer
            Using conn = Db.OpenConn()
                Dim id = Db.DbScalar(conn, "SELECT TOP 1 ID FROM Funds WHERE FundName = @n",
                                     New Tuple(Of String, Object)("@n", "เงินสด"))
                If id Is Nothing OrElse id Is DBNull.Value Then Return -1
                Return Convert.ToInt32(id)
            End Using
        End Function

        ''' <summary>การนับล่าสุด (เรียงตาม CountDate) หรือ Nothing ถ้ายังไม่มี</summary>
        Public Function GetLatestCount() As CashCountDetail
            Dim counts = LoadCounts()
            If counts.Count = 0 Then Return Nothing
            Return counts(counts.Count - 1)
        End Function

        ''' <summary>
        ''' บันทึกการนับใหม่ (แทรกอย่างเดียว ไม่แก้ไข/ลบแถวเดิม)
        ''' คำนวณ TotalCounted = ผลรวมมูลค่าทุกชนิด, ExpectedBefore/Variance จากข้อมูลจริงก่อนบันทึก
        ''' </summary>
        Public Function SaveCount(countDate As Date, denominations As Decimal(), note As String) As CashCountDetail
            If denominations Is Nothing OrElse denominations.Length <> 9 Then
                Throw New ArgumentException("ต้องระบุมูลค่าเงินสดครบ 9 ชนิด")
            End If

            Dim total As Decimal = 0D
            For Each d In denominations
                total += d
            Next

            CashOnHandService.CashFundId = ResolveCashFundId()
            Dim expectedVal = CashOnHandService.ComputeCashOnHand(countDate, ToRecords(LoadCounts()), LoadTransactions())
            Dim expected As Decimal = If(expectedVal.HasValue, expectedVal.Value, 0D)
            Dim variance As Decimal = total - expected

            Using conn = Db.OpenConn()
                Db.ExecuteNonQuery(conn,
                    "INSERT INTO CashCounts (CountDate, D1, D2, D5, D10, D20, D50, D100, D500, D1000, TotalCounted, ExpectedBefore, Variance, [Note], [Source], CreatedAt) " &
                    "VALUES (" & Db.AccessDateLiteral(countDate) & ", @d1, @d2, @d5, @d10, @d20, @d50, @d100, @d500, @d1000, @total, @expected, @variance, @note, @source, " & Db.AccessDateLiteral(DateTime.Now) & ")",
                    New Tuple(Of String, Object)("@d1", denominations(0)),
                    New Tuple(Of String, Object)("@d2", denominations(1)),
                    New Tuple(Of String, Object)("@d5", denominations(2)),
                    New Tuple(Of String, Object)("@d10", denominations(3)),
                    New Tuple(Of String, Object)("@d20", denominations(4)),
                    New Tuple(Of String, Object)("@d50", denominations(5)),
                    New Tuple(Of String, Object)("@d100", denominations(6)),
                    New Tuple(Of String, Object)("@d500", denominations(7)),
                    New Tuple(Of String, Object)("@d1000", denominations(8)),
                    New Tuple(Of String, Object)("@total", total),
                    New Tuple(Of String, Object)("@expected", expected),
                    New Tuple(Of String, Object)("@variance", variance),
                    New Tuple(Of String, Object)("@note", If(String.IsNullOrWhiteSpace(note), DBNull.Value, note.Trim())),
                    New Tuple(Of String, Object)("@source", "Manual count"))
            End Using

            Dim detail As New CashCountDetail()
            detail.CountDate = countDate
            detail.D1 = denominations(0)
            detail.D2 = denominations(1)
            detail.D5 = denominations(2)
            detail.D10 = denominations(3)
            detail.D20 = denominations(4)
            detail.D50 = denominations(5)
            detail.D100 = denominations(6)
            detail.D500 = denominations(7)
            detail.D1000 = denominations(8)
            detail.TotalCounted = total
            detail.ExpectedBefore = expected
            detail.Variance = variance
            detail.Note = note
            detail.Source = "Manual count"
            Return detail
        End Function

        Private Function ToRecords(counts As List(Of CashCountDetail)) As List(Of CashCountRecord)
            Dim recs As New List(Of CashCountRecord)()
            For Each c In counts
                recs.Add(New CashCountRecord() With {.CountDate = c.CountDate, .TotalCounted = c.TotalCounted})
            Next
            Return recs
        End Function

        Private Function GetText(value As Object) As String
            If value Is Nothing OrElse value Is DBNull.Value Then Return ""
            Return Convert.ToString(value)
        End Function

        Private Function GetIntNullable(value As Object) As Integer?
            If value Is Nothing OrElse value Is DBNull.Value Then Return Nothing
            Return Convert.ToInt32(value)
        End Function
    End Module
End Namespace
