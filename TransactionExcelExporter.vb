Option Strict Off
Option Explicit On

Imports System
Imports System.Data
Imports System.IO
Imports ClosedXML.Excel

Namespace TempleAccounting
    ''' <summary>
    ''' ส่งออกข้อมูลสมุดรายวัน (รายรับ-จ่าย) เป็นไฟล์ Excel (.xlsx) โดยใช้ ClosedXML
    ''' </summary>
    Public Module TransactionExcelExporter

        ''' <summary>
        ''' สร้างชื่อไฟล์เริ่มต้นตามช่วงวันที่ เช่น สมุดรายวัน_สิงหาคม_2569.xlsx
        ''' หรือ สมุดรายวัน_01-31_ส.ค._2569.xlsx เมื่อวันที่เริ่ม/สิ้นสุดอยู่คนละเดือน
        ''' </summary>
        Public Function BuildDefaultFileName(fromDate As Date, toDate As Date) As String
            Dim bYear = BuddhistYear(toDate)
            If fromDate.Year = toDate.Year AndAlso fromDate.Month = toDate.Month Then
                Return "สมุดรายวัน_" & ReportEngine.ThaiMonthFull(toDate.Month) & "_" & bYear.ToString() & ".xlsx"
            End If
            Return "สมุดรายวัน_" & fromDate.Day.ToString("00") & "-" & toDate.Day.ToString("00") & "_" &
                   ReportEngine.ThaiMonthAbbr(toDate.Month) & "_" & bYear.ToString() & ".xlsx"
        End Function

        ''' <summary>
        ''' เขียนตารางรายการที่กรองแล้วลงไฟล์ Excel พร้อมหัววัด ชื่อรายงาน ช่วงวันที่ และยอดสรุป
        ''' </summary>
        Public Sub ExportTransactionsToExcel(rows As DataTable, fromDate As Date, toDate As Date, filePath As String)
            Dim templeName = "วัดแหลมยาง"
            Try
                Dim info = ReportEngine.GetTemplateInfo()
                If info IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(info.TempleName) Then
                    templeName = info.TempleName
                End If
            Catch
                ' ใช้ชื่อเริ่มต้นหากโหลดข้อมูลวัดไม่สำเร็จ
            End Try

            Dim fontName = "Tahoma"

            Using wb As New XLWorkbook()
                Dim ws = wb.Worksheets.Add("สมุดรายวัน")

                ' --- ส่วนหัวรายงาน (แถว 1-3) ---
                ws.Cell(1, 1).Value = templeName
                ws.Cell(2, 1).Value = "สมุดรายวันรับ-จ่าย"
                ws.Cell(3, 1).Value = "ช่วงวันที่ " & FormatBuddhistDate(fromDate) & " ถึง " & FormatBuddhistDate(toDate)

                ws.Range("A1:G1").Merge()
                ws.Range("A2:G2").Merge()
                ws.Range("A3:G3").Merge()

                ws.Cell(1, 1).Style.Font.FontName = fontName
                ws.Cell(1, 1).Style.Font.FontSize = 16
                ws.Cell(1, 1).Style.Font.Bold = True
                ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center

                ws.Cell(2, 1).Style.Font.FontName = fontName
                ws.Cell(2, 1).Style.Font.FontSize = 14
                ws.Cell(2, 1).Style.Font.Bold = True
                ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center

                ws.Cell(3, 1).Style.Font.FontName = fontName
                ws.Cell(3, 1).Style.Font.FontSize = 11
                ws.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center

                ' --- หัวตาราง (แถว 5) ---
                Const headerRow As Integer = 5
                Dim headers = {"ลำดับ", "วันที่ทำรายการ", "ชนิด", "หมวดหมู่/ประเภท", "กองทุน", "รายละเอียดรายการ", "จำนวนเงิน"}
                For c As Integer = 0 To headers.Length - 1
                    Dim cell = ws.Cell(headerRow, c + 1)
                    cell.Value = headers(c)
                    cell.Style.Font.FontName = fontName
                    cell.Style.Font.Bold = True
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(253, 224, 71)
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center
                Next

                ' --- ข้อมูล (แถว 6 เป็นต้นไป) ---
                Dim dataStart = headerRow + 1
                Dim r = dataStart
                Dim totalIncome As Decimal = 0D
                Dim totalExpense As Decimal = 0D

                If rows IsNot Nothing Then
                    For Each row As DataRow In rows.Rows
                        Dim tranType = SafeStr(row, "TranType")
                        Dim amount = Db.ToDecimalOrZero(row("Amount"))

                        ws.Cell(r, 1).Value = r - dataStart + 1
                        If Not row.IsNull("TranDate") Then
                            ws.Cell(r, 2).Value = Convert.ToDateTime(row("TranDate"))
                            ws.Cell(r, 2).Style.NumberFormat.Format = "dd/MM/yyyy"
                        End If
                        ws.Cell(r, 3).Value = SafeStr(row, "TranTypeDisplay")
                        ws.Cell(r, 4).Value = SafeStr(row, "CategoryName")
                        ws.Cell(r, 5).Value = SafeStr(row, "FundName")
                        ws.Cell(r, 6).Value = SafeStr(row, "Detail")
                        ws.Cell(r, 7).Value = amount
                        ws.Cell(r, 7).Style.NumberFormat.Format = "#,##0.00"
                        ws.Cell(r, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right

                        If tranType = "Income" Then totalIncome += amount
                        If tranType = "Expense" Then totalExpense += amount

                        r += 1
                    Next
                End If

                Dim lastDataRow = r - 1

                ' --- ยอดสรุปท้ายตาราง ---
                Dim netBalance = totalIncome - totalExpense
                Dim sumStart = r + 1

                ws.Cell(sumStart, 6).Value = "ยอดรวมรายรับทั้งหมด"
                ws.Cell(sumStart, 7).Value = totalIncome
                ws.Cell(sumStart, 7).Style.NumberFormat.Format = "#,##0.00"

                ws.Cell(sumStart + 1, 6).Value = "ยอดรวมรายจ่ายทั้งหมด"
                ws.Cell(sumStart + 1, 7).Value = totalExpense
                ws.Cell(sumStart + 1, 7).Style.NumberFormat.Format = "#,##0.00"

                ws.Cell(sumStart + 2, 6).Value = "ยอดคงเหลือสุทธิ"
                ws.Cell(sumStart + 2, 7).Value = netBalance
                ws.Cell(sumStart + 2, 7).Style.NumberFormat.Format = "#,##0.00"

                For i As Integer = 0 To 2
                    ws.Cell(sumStart + i, 6).Style.Font.Bold = True
                    ws.Cell(sumStart + i, 7).Style.Font.Bold = True
                    ws.Cell(sumStart + i, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right
                Next

                ' --- เส้นตาราง (เส้นบาง) ---
                Dim tableRange = ws.Range(headerRow, 1, Math.Max(headerRow, lastDataRow), 7)
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin

                Dim summaryRange = ws.Range(sumStart, 6, sumStart + 2, 7)
                summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin
                summaryRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin

                ' --- ปรับความกว้างคอลัมน์อัตโนมัติ (ตามหัวตาราง/ข้อมูล/สรุป) ---
                ws.Columns(1, 7).AdjustToContents(headerRow, sumStart + 2)
                ' จำกัดความกว้างคอลัมน์รายละเอียดให้ไม่กว้างเกินไป
                If ws.Column(6).Width > 60 Then
                    ws.Column(6).Width = 60
                End If

                wb.SaveAs(filePath)
            End Using
        End Sub

        Private Function SafeStr(row As DataRow, columnName As String) As String
            If row Is Nothing OrElse row.Table Is Nothing OrElse Not row.Table.Columns.Contains(columnName) OrElse row.IsNull(columnName) Then Return ""
            Return Convert.ToString(row(columnName)).Trim()
        End Function

        Private Function BuddhistYear(d As Date) As Integer
            Return If(d.Year < 2400, d.Year + 543, d.Year)
        End Function

        Private Function FormatBuddhistDate(d As Date) As String
            Return d.Day.ToString() & " " & ReportEngine.ThaiMonthFull(d.Month) & " " & BuddhistYear(d).ToString()
        End Function

    End Module
End Namespace
