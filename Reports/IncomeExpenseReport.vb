Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.OleDb
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Globalization
Imports System.Linq
Imports System.Windows.Forms

Namespace TempleAccounting
    Public Class IncomeExpenseReport
        Inherits PrintDocument

        Public Enum ReportModes
            Detailed = 0
            Summary = 1
        End Enum

        Private _fromDate As Date
        Private _toDate As Date
        Private _mode As ReportModes = ReportModes.Detailed
        Private _fundID As Integer? = Nothing
        Private _bankID As Integer? = Nothing
        Private _incomeRows As New List(Of ReportRow)()
        Private _expenseRows As New List(Of ReportRow)()
        Private _totalIncome As Decimal
        Private _totalExpense As Decimal
        Private _openingBalance As Decimal
        Private _manualOpeningBalance As Decimal? = Nothing
        Private _reportGrandTotal As Decimal
        Private _balance As Decimal
        Private _templateInfo As TemplateInfo

        Private _layout As LayoutConfig
        Private _theme As ReportTheme
        Private _reportInfo As ReportInfo
        Private _totalPages As Integer = 0

        Private _leftSectionWidth As Integer
        Private _rightSectionWidth As Integer
        Private _pageY As Integer
        Private _pageBottom As Integer
        Private _pageMargin As Integer = 40
        Private _startX As Integer
        Private _leftX As Integer
        Private _rightX As Integer
        Private _maxRowsPerPage As Integer
        Private _pageIndex As Integer = 0
        Private _rowIndex As Integer = 0

        Private Const FinalSummaryRows As Integer = 3
        Private Const FinalSignatureBlockHeight As Integer = 135  ' Reduced from 145
        Private Const FinalFooterGapHeight As Integer = 8        ' Reduced from 12
        Private Const SectionGap As Integer = 12               ' Gap between left/right sections

        Private Structure ReportRow
            Public TranDate As Date
            Public DayCode As Integer
            Public Description As String
            Public Amount As Decimal
            Public IsCarryForward As Boolean
            Public IsCategorySummary As Boolean
        End Structure

        ''' <summary>
        ''' ชื่อเจ้าอาวาส (สำหรับแสดงในรายงาน)
        ''' </summary>
        Public ReadOnly Property AbbotName As String
            Get
                Return If(_templateInfo IsNot Nothing, _templateInfo.AbbotName, "")
            End Get
        End Property

        ''' <summary>
        ''' ชื่อผู้จัดทำบัญชี (สำหรับแสดงในรายงาน)
        ''' </summary>
        Public ReadOnly Property AccountantName As String
            Get
                Return If(_templateInfo IsNot Nothing, _templateInfo.AccountantName, "")
            End Get
        End Property

        Public Sub New(fromDate As Date, toDate As Date, Optional mode As ReportModes = ReportModes.Detailed, Optional manualOpeningBalance As Decimal? = Nothing, Optional fundID As Integer? = Nothing, Optional bankID As Integer? = Nothing)
            MyBase.New()
            _fromDate = Db.NormalizeGregorianDate(fromDate)
            _toDate = Db.NormalizeGregorianDate(toDate)
            _mode = mode
            _manualOpeningBalance = manualOpeningBalance
            _fundID = fundID
            _bankID = bankID
            Me.DocumentName = If(_mode = ReportModes.Summary, "สรุปบัญชีรายรับ-รายจ่าย (ย่อ)", "สรุปบัญชีรายรับ-รายจ่าย (ละเอียด)")
            ConfigurePageSettings()
            LoadData()
            InitFonts()
        End Sub

        Private Shared Function CreateA4Paper(Optional printerSettings As PrinterSettings = Nothing) As PaperSize
            If printerSettings IsNot Nothing Then
                For Each paper As PaperSize In printerSettings.PaperSizes
                    If paper.Kind = PaperKind.A4 OrElse paper.PaperName.IndexOf("A4", StringComparison.OrdinalIgnoreCase) >= 0 Then
                        Return paper
                    End If
                Next
            End If

            Return New PaperSize("A4", 827, 1169)
        End Function

        Private Sub ConfigurePageSettings()
            Dim a4Paper = CreateA4Paper(Me.PrinterSettings)
            Dim pageMargins As New Margins(32, 32, 28, 28)

            Me.DefaultPageSettings.PaperSize = a4Paper
            Me.DefaultPageSettings.Margins = pageMargins
            Me.DefaultPageSettings.Landscape = True

            Me.PrinterSettings.DefaultPageSettings.PaperSize = a4Paper
            Me.PrinterSettings.DefaultPageSettings.Margins = pageMargins
            Me.PrinterSettings.DefaultPageSettings.Landscape = True
        End Sub

        Private Sub InitFonts()
            _theme = ReportTheme.CreateDefault()
        End Sub

        Private Sub LoadData()
            Try
                Db.EnsureSchema()

                ' Get shared TemplateInfo (loaded once from database)
                _templateInfo = ReportEngine.GetTemplateInfo()
                
                ' DEBUG: Log what we got
                System.Diagnostics.Debug.WriteLine("[DEBUG IncomeExpenseReport.LoadData] _templateInfo.TempleName: '" & _templateInfo.TempleName & "'")

                Using conn = Db.OpenConn()
                    _incomeRows.Clear()
                    _expenseRows.Clear()
                    _totalIncome = 0
                    _totalExpense = 0
                    
                    If _manualOpeningBalance.HasValue Then
                        _openingBalance = _manualOpeningBalance.Value
                    Else
                        _openingBalance = GetBalanceBeforeDate(conn, _fromDate, _fundID, _bankID)
                    End If

                    LoadRowsByMode(conn, "Income", _incomeRows, _totalIncome, _fundID, _bankID)
                    LoadRowsByMode(conn, "Expense", _expenseRows, _totalExpense, _fundID, _bankID)

                    _incomeRows.Insert(0, BuildOpeningBalanceRow())
                    _reportGrandTotal = _openingBalance + _totalIncome
                    _balance = _reportGrandTotal - _totalExpense

                    ' Initialize ReportInfo for header rendering using shared TemplateInfo
                    Dim reportTitle = If(_mode = ReportModes.Summary, "สรุปบัญชีรายรับ - รายจ่าย (แบบย่อ)", "สรุปบัญชีรายรับ - รายจ่าย (แบบละเอียด)")
                    _reportInfo = New ReportInfo(reportTitle, _templateInfo.TempleName, _templateInfo.TempleAddress, _fromDate, _toDate)
                End Using
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Function GetBalanceBeforeDate(conn As OleDbConnection, beforeDate As Date, fundID As Integer?, bankID As Integer?) As Decimal
            Dim sql = "SELECT SUM(IIF(t.TranType='Income', t.Amount, -t.Amount)) " &
                      "FROM Transactions t " &
                      "WHERE DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)) < " &
                      Db.AccessDateLiteral(beforeDate)
            
            Dim params As New List(Of Tuple(Of String, Object))()
            If fundID.HasValue AndAlso fundID.Value <> 0 Then
                sql &= " AND t.FundID = @f"
                params.Add(New Tuple(Of String, Object)("@f", fundID.Value))
            End If
            If bankID.HasValue AndAlso bankID.Value <> 0 Then
                sql &= " AND t.BankID = @b"
                params.Add(New Tuple(Of String, Object)("@b", bankID.Value))
            End If

            Dim result = Db.DbScalar(conn, sql, params.ToArray())
            If result Is Nothing OrElse IsDBNull(result) Then Return 0D
            Return Convert.ToDecimal(result)
        End Function

        Private Function BuildOpeningBalanceRow() As ReportRow
            Return New ReportRow With {
                .TranDate = _fromDate,
                .DayCode = _fromDate.Day,
                .Description = "ยอดยกมา" & If(_manualOpeningBalance.HasValue, " (กรอกเอง)", ""),
                .Amount = _openingBalance,
                .IsCarryForward = True
            }
        End Function

        Private Sub LoadRowsByMode(conn As OleDbConnection, tranType As String, targetRows As List(Of ReportRow), ByRef runningTotal As Decimal, fundID As Integer?, bankID As Integer?)
            targetRows.Clear()
            runningTotal = 0D

            Dim sql As String
            Dim params As New List(Of Tuple(Of String, Object))()

            Dim whereClause = "WHERE t.TranType='" & tranType & "' AND DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)) " &
                              "BETWEEN " & Db.AccessDateLiteral(_fromDate) & " AND " & Db.AccessDateLiteral(_toDate)
            
            If fundID.HasValue AndAlso fundID.Value <> 0 Then
                whereClause &= " AND t.FundID = @f"
                params.Add(New Tuple(Of String, Object)("@f", fundID.Value))
            End If
            If bankID.HasValue AndAlso bankID.Value <> 0 Then
                whereClause &= " AND t.BankID = @b"
                params.Add(New Tuple(Of String, Object)("@b", bankID.Value))
            End If

            If _mode = ReportModes.Summary Then
                sql = "SELECT DateSerial(Year(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate) + 1, 0)), " &
                      "Month(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate) + 1, 0)), " &
                      "Day(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate) + 1, 0))) AS TranDate, " &
                      "IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName) AS Cat, SUM(t.Amount) AS Amount " &
                      "FROM Transactions t LEFT JOIN Categories c ON t.CategoryID=c.ID " &
                      whereClause & " " &
                      "GROUP BY DateSerial(Year(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate) + 1, 0)), " &
                      "Month(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate) + 1, 0)), " &
                      "Day(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate) + 1, 0))), " &
                      "IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName) " &
                      "ORDER BY DateSerial(Year(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate) + 1, 0)), " &
                      "Month(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate) + 1, 0)), " &
                      "Day(DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate) + 1, 0))), " &
                      "IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName)"
            Else
                sql = "SELECT DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)) AS TranDate, " &
                      "IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName) AS Cat, SUM(t.Amount) AS Amount " &
                      "FROM Transactions t LEFT JOIN Categories c ON t.CategoryID=c.ID " &
                      whereClause & " " &
                      "GROUP BY DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)), " &
                      "IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName) " &
                      "ORDER BY DateSerial(IIF(Year(t.TranDate) > 2400, Year(t.TranDate) - 543, Year(t.TranDate)), Month(t.TranDate), Day(t.TranDate)), " &
                      "IIF(c.CategoryName IS NULL,'ไม่ระบุ',c.CategoryName)"
            End If

            Dim dt = Db.GetTable(conn, sql, params.ToArray())
            For Each r As DataRow In dt.Rows
                Dim d = CDate(r!TranDate)
                Dim amount = CDec(r!Amount)
                targetRows.Add(New ReportRow With {
                    .TranDate = d,
                    .DayCode = d.Day,
                    .Description = CStr(r!Cat),
                    .Amount = amount,
                    .IsCarryForward = False
                })
                runningTotal += amount
            Next
        End Sub

        Private Function ToThaiNumerals(s As String) As String
            Dim result As String = ""
            For Each ch In s
                If Char.IsDigit(ch) Then
                    result &= ChrW(AscW("๐") + (AscW(ch) - AscW("0")))
                Else
                    result &= ch
                End If
            Next
            Return result
        End Function

        Private Function ThaiMonthAbbr(ByVal m As Integer) As String
            Return ReportEngine.ThaiMonthAbbr(m)
        End Function

        Private Function ToBuddhistDateShort(ByVal d As Date) As String
            Return ThaiMonthAbbr(d.Month) & "-" & ToThaiNumerals(((d.Year + 543) Mod 100).ToString())
        End Function

        Private Function ToBuddhistFull(ByVal d As Date) As String
            Return ToThaiNumerals(d.Day.ToString()) & " " & ThaiMonthFull(d.Month) & " พ.ศ. " & ToThaiNumerals((d.Year + 543).ToString())
        End Function

        Private Function ThaiMonthFull(ByVal m As Integer) As String
            Return ReportEngine.ThaiMonthFull(m)
        End Function

        Private Function ToBuddhistYearThai(ByVal y As Integer) As String
            Dim yB = y + 543
            Return ToThaiNumerals(yB.ToString())
        End Function

        Private Function FormatThaiAmount(value As Decimal) As String
            Return ReportEngine.FormatThaiAmount(value)
        End Function

        Protected Overrides Sub OnBeginPrint(e As PrintEventArgs)
            MyBase.OnBeginPrint(e)
            ConfigurePageSettings()
            _pageIndex = 0
            _rowIndex = 0
        End Sub

        Protected Overrides Sub OnQueryPageSettings(e As QueryPageSettingsEventArgs)
            MyBase.OnQueryPageSettings(e)
            e.PageSettings.PaperSize = CreateA4Paper(Me.PrinterSettings)
            e.PageSettings.Margins = New Margins(32, 32, 28, 28)
            e.PageSettings.Landscape = True
        End Sub

        Protected Overrides Sub OnPrintPage(e As PrintPageEventArgs)
            MyBase.OnPrintPage(e)
            Dim g As Graphics = e.Graphics
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

            ' Use LayoutConfig for consistent layout calculations
            _layout = LayoutConfig.CreateA4Landscape(32, 32, 28, 28)

            Dim pageW = e.PageBounds.Width
            _startX = e.MarginBounds.Left
            _pageY = e.MarginBounds.Top
            _pageBottom = e.MarginBounds.Bottom
            _leftX = _startX
            _rightSectionWidth = CInt((e.MarginBounds.Width) / 2) - 6
            _leftSectionWidth = _rightSectionWidth
            _rightX = _startX + _leftSectionWidth + SectionGap
            Dim usableW = _leftSectionWidth

            ' [REPORT DEBUG] - Pagination accuracy testing
            System.Diagnostics.Debug.WriteLine("")
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] ====================================")
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] AbbotName = " & _templateInfo.AbbotName)
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] AccountantName = " & _templateInfo.AccountantName)
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Page Number = " & (_pageIndex + 1))
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Current Y Position = " & _pageY)
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Remaining Height = " & (_pageBottom - _pageY))
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Required For Final Content = " & CalculateFinalContentHeight(28))
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Signature Reserved Height = " & FinalSignatureBlockHeight)
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Total Rows = " & Math.Max(_incomeRows.Count, _expenseRows.Count))
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Current Row Index = " & _rowIndex)
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Is Last Page = " & (_rowIndex >= Math.Max(_incomeRows.Count, _expenseRows.Count)))
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] ====================================")
            System.Diagnostics.Debug.WriteLine("")

            Dim totalRows = Math.Max(_incomeRows.Count, _expenseRows.Count)

            ' Draw header on EVERY page with page number in upper-right corner
            ' Page number uses Thai numerals: หน้า ๑ / ๓
            DrawHeader(g, pageW, _pageIndex + 1, _totalPages)

            Dim colW1 = CInt(usableW * 0.16)
            Dim colW2 = CInt(usableW * 0.1)
            Dim colW3 = usableW - colW1 - colW2 - CInt(usableW * 0.24)
            Dim colW4 = CInt(usableW * 0.24)
            Dim rowH = 28

            ' Only draw table headers if we have transactions to print on THIS page
            If _rowIndex < totalRows Then
                DrawTableHeader(g, rowH, colW1, colW2, colW3, colW4)
            End If

            ' Dynamic pagination: draw rows until we run out of space
            While _rowIndex < totalRows
                ' Check if we can fit the next row
                If _pageY + rowH > _pageBottom Then
                    ' Cannot fit this row - need new page (middle page, no signatures)
                    e.HasMorePages = True
                    _pageIndex += 1
                    Return
                End If

                ' Draw the row
                Dim y = _pageY
                Dim hasLeft = (_rowIndex < _incomeRows.Count)
                Dim hasRight = (_rowIndex < _expenseRows.Count)

                If hasLeft Then
                    Dim row = _incomeRows(_rowIndex)
                    Dim col = 0
                    g.DrawRectangle(_theme.BlackPen, _leftX, y, usableW, rowH)
                    g.DrawRectangle(_theme.BlackPen, _leftX, y, colW1, rowH)
                    g.DrawRectangle(_theme.BlackPen, _leftX + colW1, y, colW2, rowH)
                    g.DrawRectangle(_theme.BlackPen, _leftX + colW1 + colW2, y, colW3, rowH)
                    g.DrawRectangle(_theme.BlackPen, _leftX + colW1 + colW2 + colW3, y, colW4, rowH)
                    g.DrawLine(_theme.BlackPen, _leftX + colW1 + colW2 + colW3, y, _leftX + colW1 + colW2 + colW3, y + rowH)

                    Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                    Dim fmtL As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center}
                    Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}

                    g.DrawString(ToBuddhistDateShort(row.TranDate), _theme.RowFont, Brushes.Black, New RectangleF(_leftX, y, colW1, rowH), fmt)
                    g.DrawString(ToThaiNumerals(row.DayCode.ToString()), _theme.RowFont, Brushes.Black, New RectangleF(_leftX + colW1, y, colW2, rowH), fmt)
                    g.DrawString(Truncate(g, row.Description, _theme.RowFont, colW3 - 6), _theme.RowFont, Brushes.Black,
                                 New RectangleF(_leftX + colW1 + colW2 + 3, y, colW3 - 6, rowH), fmtL)
                    Dim redColor As Color = If(row.IsCarryForward, Color.Red, Color.Black)
                    g.DrawString(FormatThaiAmount(row.Amount), _theme.RowFont, If(row.IsCarryForward, Brushes.Red, Brushes.Black),
                                 New RectangleF(_leftX + colW1 + colW2 + colW3, y, colW4 - 4, rowH), fmtR)
                End If

                If hasRight Then
                    Dim row = _expenseRows(_rowIndex)
                    g.DrawRectangle(_theme.BlackPen, _rightX, y, usableW, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX, y, colW1, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX + colW1, y, colW2, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX + colW1 + colW2, y, colW3, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX + colW1 + colW2 + colW3, y, colW4, rowH)
                    g.DrawLine(_theme.BlackPen, _rightX + colW1 + colW2 + colW3, y, _rightX + colW1 + colW2 + colW3, y + rowH)

                    Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                    Dim fmtL As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center}
                    Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}

                    g.DrawString(ToBuddhistDateShort(row.TranDate), _theme.RowFont, Brushes.Black, New RectangleF(_rightX, y, colW1, rowH), fmt)
                    g.DrawString(ToThaiNumerals(row.DayCode.ToString()), _theme.RowFont, Brushes.Black, New RectangleF(_rightX + colW1, y, colW2, rowH), fmt)
                    g.DrawString(Truncate(g, row.Description, _theme.RowFont, colW3 - 6), _theme.RowFont, Brushes.Black,
                                 New RectangleF(_rightX + colW1 + colW2 + 3, y, colW3 - 6, rowH), fmtL)
                    g.DrawString(FormatThaiAmount(row.Amount), _theme.RowFont, Brushes.Black,
                                 New RectangleF(_rightX + colW1 + colW2 + colW3, y, colW4 - 4, rowH), fmtR)
                Else
                    g.DrawRectangle(_theme.BlackPen, _rightX, y, usableW, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX, y, colW1, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX + colW1, y, colW2, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX + colW1 + colW2, y, colW3, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX + colW1 + colW2 + colW3, y, colW4, rowH)
                    g.DrawLine(_theme.BlackPen, _rightX + colW1 + colW2 + colW3, y, _rightX + colW1 + colW2 + colW3, y + rowH)
                End If

                _rowIndex += 1
                _pageY += rowH
            End While

            ' After drawing rows, check if we've reached the end of the data
            Dim isLastPage = (_rowIndex >= totalRows)

            ' All rows printed - now check if this is the last page with transactions
            If isLastPage Then
                ' H_page = e.PageBounds.Height (A4 Landscape = 1122px at 96dpi)
                ' H_remaining = H_page - TopMargin - BottomMargin - H_header - H_tableData
                ' In code: availableSpace = _pageBottom - _pageY
                Dim requiredForFinalContent = CalculateFinalContentHeight(rowH)
                Dim availableSpace = _pageBottom - _pageY

                ' If H_remaining >= H_sig: render on current page
                ' If H_remaining < H_sig: trigger new page
                If availableSpace < requiredForFinalContent Then
                    ' Not enough space - create a new page for signatures only
                    e.HasMorePages = True
                    _pageIndex += 1
                    Return
                End If

                ' We have enough space - fill remaining with empty rows, then draw summaries and signatures
                While _pageY < _pageBottom - requiredForFinalContent
                    g.DrawRectangle(_theme.BlackPen, _leftX, _pageY, usableW, rowH)
                    g.DrawRectangle(_theme.BlackPen, _leftX, _pageY, colW1, rowH)
                    g.DrawRectangle(_theme.BlackPen, _leftX + colW1, _pageY, colW2, rowH)
                    g.DrawRectangle(_theme.BlackPen, _leftX + colW1 + colW2, _pageY, colW3, rowH)
                    g.DrawRectangle(_theme.BlackPen, _leftX + colW1 + colW2 + colW3, _pageY, colW4, rowH)
                    g.DrawLine(_theme.BlackPen, _leftX + colW1 + colW2 + colW3, _pageY, _leftX + colW1 + colW2 + colW3, _pageY + rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX, _pageY, usableW, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX, _pageY, colW1, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX + colW1, _pageY, colW2, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX + colW1 + colW2, _pageY, colW3, rowH)
                    g.DrawRectangle(_theme.BlackPen, _rightX + colW1 + colW2 + colW3, _pageY, colW4, rowH)
                    g.DrawLine(_theme.BlackPen, _rightX + colW1 + colW2 + colW3, _pageY, _rightX + colW1 + colW2 + colW3, _pageY + rowH)
                    _pageY += rowH
                End While

                DrawSummaries(g, rowH, colW1, colW2, colW3, colW4)
                DrawSignatures(g, rowH, colW1, colW2, colW3, colW4, usableW)
                e.HasMorePages = False
            Else
                ' Not the last page - draw mid-summary
                DrawMidSummary(g, rowH, colW1, colW2, colW3, colW4, usableW)
                e.HasMorePages = True
                _pageIndex += 1
            End If
        End Sub

        ''' <summary>
        ''' Calculate the total height required for final summary section and signatures
        ''' H_sig = H_summary + H_signature + H_footerGap + H_topGap
        ''' </summary>
        Private Function CalculateFinalContentHeight(rowH As Integer) As Integer
            ' Summary rows (3 rows * 28px = 84px) + gap(4px) = 88px
            Dim summaryHeight = (FinalSummaryRows * rowH) + 4
            ' Signature block height = 135px
            Dim signatureHeight = FinalSignatureBlockHeight
            ' Gap before footer = 8px
            Dim footerGap = FinalFooterGapHeight
            ' Minimum top gap = 4px
            Dim topGap = 4

            ' Total: 88 + 135 + 8 + 4 = 235px (Reduced from 257px)
            Return summaryHeight + signatureHeight + footerGap + topGap
        End Function

        Private Sub DrawEmptyRows(g As Graphics, printed As Integer, max As Integer, rowH As Integer,
                                  c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer, usableW As Integer)
            For i = printed To max - 1
                g.DrawRectangle(_theme.BlackPen, _leftX, _pageY, usableW, rowH)
                g.DrawRectangle(_theme.BlackPen, _leftX, _pageY, c1, rowH)
                g.DrawRectangle(_theme.BlackPen, _leftX + c1, _pageY, c2, rowH)
                g.DrawRectangle(_theme.BlackPen, _leftX + c1 + c2, _pageY, c3, rowH)
                g.DrawRectangle(_theme.BlackPen, _leftX + c1 + c2 + c3, _pageY, c4, rowH)
                g.DrawLine(_theme.BlackPen, _leftX + c1 + c2 + c3, _pageY, _leftX + c1 + c2 + c3, _pageY + rowH)
                g.DrawRectangle(_theme.BlackPen, _rightX, _pageY, usableW, rowH)
                g.DrawRectangle(_theme.BlackPen, _rightX, _pageY, c1, rowH)
                g.DrawRectangle(_theme.BlackPen, _rightX + c1, _pageY, c2, rowH)
                g.DrawRectangle(_theme.BlackPen, _rightX + c1 + c2, _pageY, c3, rowH)
                g.DrawRectangle(_theme.BlackPen, _rightX + c1 + c2 + c3, _pageY, c4, rowH)
                g.DrawLine(_theme.BlackPen, _rightX + c1 + c2 + c3, _pageY, _rightX + c1 + c2 + c3, _pageY + rowH)
                _pageY += rowH
            Next
        End Sub

        Private Sub DrawMidSummary(g As Graphics, rowH As Integer, c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer, usableW As Integer)
            Dim y = _pageY
            Dim fmtC As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
            Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}
            Dim runningIncome As Decimal = 0D
            For i = 0 To Math.Min(_rowIndex, _incomeRows.Count) - 1
                runningIncome += _incomeRows(i).Amount
            Next

            Dim runningExpense As Decimal = 0D
            For i = 0 To Math.Min(_rowIndex, _expenseRows.Count) - 1
                runningExpense += _expenseRows(i).Amount
            Next

            g.DrawString("รวมทั้งสิ้น", _theme.BoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawRectangle(_theme.BlackPen, _leftX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(runningIncome), _theme.BoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            g.DrawString("รวมทั้งสิ้น", _theme.BoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawRectangle(_theme.BlackPen, _rightX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(runningExpense), _theme.BoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)
        End Sub

        Private Sub DrawSummaries(g As Graphics, rowH As Integer, c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer,
                                  Optional blankCount As Integer = 0)
            Dim baseY = _pageY
            Dim usableW = _leftSectionWidth
            Dim fmtC As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
            Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}
            Dim fmtL As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center, .FormatFlags = StringFormatFlags.NoWrap}
            Dim balanceLabel = BuildCarryForwardLabel()

            ' LEFT Income Total
            Dim y = baseY
            g.DrawRectangle(_theme.BlackPen, _leftX, y, usableW, rowH)
            g.DrawLine(_theme.BlackPen, _leftX + c1 + c2, y, _leftX + c1 + c2, y + rowH)
            g.DrawLine(_theme.BlackPen, _leftX + c1 + c2 + c3, y, _leftX + c1 + c2 + c3, y + rowH)
            g.DrawString("รวมรายรับ", _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(_totalIncome), _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' LEFT Grand total row (ยอดยกมา + รายรับ)
            y = baseY + rowH
            g.DrawString("รวมทั้งสิ้น", _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawRectangle(_theme.BlackPen, _leftX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(_reportGrandTotal), _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' RIGHT Expense Total
            y = baseY
            g.DrawRectangle(_theme.BlackPen, _rightX, y, usableW, rowH)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2, y, _rightX + c1 + c2, y + rowH)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2 + c3, y, _rightX + c1 + c2 + c3, y + rowH)
            g.DrawString("รวมรายจ่าย", _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(_totalExpense), _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' RIGHT Carry forward row
            y = baseY + rowH
            Dim carryLabelX = _rightX + 6
            Dim carryLabelWidth = c1 + c2 + c3 - 12
            Using fittedCarryFont = CreateFittedBoldFont(g, balanceLabel, _theme.BigBoldFont, carryLabelWidth, 9.5F)
                g.DrawString(balanceLabel, fittedCarryFont, Brushes.Red,
                             New RectangleF(carryLabelX, y, carryLabelWidth, rowH), fmtL)
            End Using
            g.DrawRectangle(_theme.BlackPen, _rightX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(_balance), _theme.BigBoldFont, Brushes.Red,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' RIGHT Grand total row (ต้องเท่ากับฝั่งรายรับรวมยอดยกมา)
            y = baseY + (rowH * 2)
            g.DrawString("รวมทั้งสิ้น", _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawRectangle(_theme.BlackPen, _rightX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(_reportGrandTotal), _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)
            _pageY = baseY + (rowH * 3)
        End Sub

        Private Function BuildCarryForwardLabel() As String
            Dim buddhistYear = _toDate.Year + 543
            Dim nextBuddhistYear = buddhistYear + 1
            Return "ยอดคงเหลือ ณ " &
                   ToThaiNumerals(_toDate.Day.ToString()) & " " &
                   ThaiMonthAbbr(_toDate.Month) & " " &
                   ToThaiNumerals((buddhistYear Mod 100).ToString("00")) &
                   " ยกไปปี " &
                   ToThaiNumerals((nextBuddhistYear Mod 100).ToString("00"))
        End Function

        Private Function CreateFittedBoldFont(g As Graphics, text As String, baseFont As Font, maxWidth As Integer, minSize As Single) As Font
            ' Delegate to ReportEngine with Bold style
            Return ReportEngine.CreateFittedFont(g, text, baseFont, maxWidth, minSize, FontStyle.Bold)
        End Function

        Private Sub DrawSignatures(g As Graphics, rowH As Integer, c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer, usableW As Integer)
            ' Delegate to shared ReportEngine.DrawSignatureBlock for consistent rendering
            _pageY = ReportEngine.DrawSignatureBlock(g,
                "ตรวจถูกต้องแล้ว", _templateInfo.AbbotName, "เจ้าอาวาส",
                "ผู้จัดทำบัญชี", _templateInfo.AccountantName, "ไวยาวัจกร",
                _leftX, _rightX, _leftSectionWidth, _pageBottom, _pageY,
                _theme.BoldFont, _theme.RowFont)
        End Sub

        ''' <summary>
        ''' Draws a dotted signature line (not a solid line)
        ''' </summary>
        Private Sub DrawDottedSignatureLine(g As Graphics, x As Integer, y As Integer, width As Integer)
            ' Delegate to ReportEngine with default dotSpacing=8 and dotRadius=1.5
            ReportEngine.DrawDottedLine(g, x, y, width, 8, 1.5!)
        End Sub

        Private Function Truncate(g As Graphics, s As String, f As Font, maxW As Integer) As String
            If String.IsNullOrEmpty(s) Then Return ""
            If g.MeasureString(s, f).Width <= maxW Then Return s
            Dim res = s
            While res.Length > 2 AndAlso g.MeasureString(res & "…", f).Width > maxW
                res = res.Substring(0, res.Length - 1)
            End While
            Return res & "…"
        End Function

        Private Sub DrawTableHeader(g As Graphics, rowH As Integer, c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer)
            Dim usableW = _leftSectionWidth
            Dim fmtC As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}

            g.FillRectangle(Brushes.White, _leftX, _pageY, c1 + c2, rowH * 2)
            g.DrawRectangle(_theme.Pen2, _leftX, _pageY, c1 + c2, rowH * 2)

            g.DrawString("เดือน", _theme.HeaderFont, Brushes.Black, New RectangleF(_leftX, _pageY, c1, rowH), fmtC)
            g.DrawLine(_theme.BlackPen, _leftX, _pageY + rowH, _leftX + c1 + c2, _pageY + rowH)
            g.DrawString("วันที่", _theme.HeaderFont, Brushes.Black, New RectangleF(_leftX + c1, _pageY + rowH, c2, rowH), fmtC)
            g.DrawLine(_theme.BlackPen, _leftX + c1, _pageY, _leftX + c1, _pageY + rowH * 2)
            g.DrawString("เดือน", _theme.HeaderFont, Brushes.Black, New RectangleF(_leftX, _pageY, c1, rowH), fmtC)

            ' รายรับ header spans 2 rows
            g.DrawRectangle(_theme.Pen2, _leftX + c1 + c2, _pageY, c3, rowH * 2)
            g.DrawString("รายรับ", _theme.HeaderFont, Brushes.Black, New RectangleF(_leftX + c1 + c2, _pageY, c3, rowH * 2), fmtC)

            ' รวมเงิน outer spans 2 rows with บาท / ส.ต. inner
            g.DrawRectangle(_theme.Pen2, _leftX + c1 + c2 + c3, _pageY, c4, rowH * 2)
            g.DrawString("รวมเงิน", _theme.HeaderFont, Brushes.Black, New RectangleF(_leftX + c1 + c2 + c3, _pageY, c4, rowH), fmtC)
            g.DrawLine(_theme.BlackPen, _leftX + c1 + c2 + c3, _pageY + rowH, _leftX + c1 + c2 + c3 + c4, _pageY + rowH)
            ' split บาท/ส.ต. but since sample shows combined number, draw combined label
            Dim bahtW = CInt(c4 * 0.8)
            Dim satW = c4 - bahtW
            g.DrawLine(_theme.BlackPen, _leftX + c1 + c2 + c3 + bahtW, _pageY + rowH, _leftX + c1 + c2 + c3 + c4, _pageY + rowH)
            g.DrawLine(_theme.BlackPen, _leftX + c1 + c2 + c3 + bahtW, _pageY + rowH, _leftX + c1 + c2 + c3 + bahtW, _pageY + rowH * 2)
            g.DrawString("บาท", _theme.HeaderFont, Brushes.Black, New RectangleF(_leftX + c1 + c2 + c3, _pageY + rowH, bahtW, rowH), fmtC)
            g.DrawString("ส.ต.", _theme.HeaderFont, Brushes.Black, New RectangleF(_leftX + c1 + c2 + c3 + bahtW, _pageY + rowH, satW, rowH), fmtC)

            g.FillRectangle(Brushes.White, _rightX, _pageY, c1 + c2, rowH * 2)
            g.DrawRectangle(_theme.Pen2, _rightX, _pageY, c1 + c2, rowH * 2)
            g.DrawLine(_theme.BlackPen, _rightX + c1, _pageY, _rightX + c1, _pageY + rowH * 2)
            g.DrawString("เดือน", _theme.HeaderFont, Brushes.Black, New RectangleF(_rightX, _pageY, c1, rowH), fmtC)
            g.DrawLine(_theme.BlackPen, _rightX, _pageY + rowH, _rightX + c1 + c2, _pageY + rowH)
            g.DrawString("วันที่", _theme.HeaderFont, Brushes.Black, New RectangleF(_rightX + c1, _pageY + rowH, c2, rowH), fmtC)

            g.DrawRectangle(_theme.Pen2, _rightX + c1 + c2, _pageY, c3, rowH * 2)
            g.DrawString("รายจ่าย", _theme.HeaderFont, Brushes.Black, New RectangleF(_rightX + c1 + c2, _pageY, c3, rowH * 2), fmtC)

            g.DrawRectangle(_theme.Pen2, _rightX + c1 + c2 + c3, _pageY, c4, rowH * 2)
            g.DrawString("รวมเงิน", _theme.HeaderFont, Brushes.Black, New RectangleF(_rightX + c1 + c2 + c3, _pageY, c4, rowH), fmtC)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2 + c3, _pageY + rowH, _rightX + c1 + c2 + c3 + c4, _pageY + rowH)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2 + c3 + bahtW, _pageY + rowH, _rightX + c1 + c2 + c3 + c4, _pageY + rowH)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2 + c3 + bahtW, _pageY + rowH, _rightX + c1 + c2 + c3 + bahtW, _pageY + rowH * 2)
            g.DrawString("บาท", _theme.HeaderFont, Brushes.Black, New RectangleF(_rightX + c1 + c2 + c3, _pageY + rowH, bahtW, rowH), fmtC)
            g.DrawString("ส.ต.", _theme.HeaderFont, Brushes.Black, New RectangleF(_rightX + c1 + c2 + c3 + bahtW, _pageY + rowH, satW, rowH), fmtC)

            _pageY += rowH * 2
        End Sub

        Private Sub DrawHeader(g As Graphics, pageW As Integer, pageNumber As Integer, totalPages As Integer)
            ' Delegate to shared ReportEngine.DrawHeader for consistent rendering
            ' Uses _reportInfo which is initialized in LoadData
            If _reportInfo Is Nothing Then
                ' Fallback if _reportInfo not initialized - use shared TemplateInfo (already has defaults)
                Dim reportTitle = If(_mode = ReportModes.Summary, "สรุปบัญชีรายรับ - รายจ่าย (แบบย่อ)", "สรุปบัญชีรายรับ - รายจ่าย (แบบละเอียด)")
                Dim template = ReportEngine.GetTemplateInfo()
                _reportInfo = New ReportInfo(reportTitle, template.TempleName, template.TempleAddress, _fromDate, _toDate)
            End If

            ' Safety check for _reportInfo properties before sending to DrawHeader
            Dim title = If(_reportInfo.ReportTitle, "รายงานรายรับ-รายจ่าย")
            Dim templeName = If(_reportInfo.TempleName, "วัด (ไม่ได้ระบุชื่อ)")
            Dim templeAddress = If(_reportInfo.TempleAddress, "-")

            ' Safety check for fonts
            Dim titleFont = If(_theme?.TitleFont, New Font("Tahoma", 16, FontStyle.Bold))
            Dim subtitleFont = If(_theme?.SubTitleFont, New Font("Tahoma", 12, FontStyle.Bold))

            _pageY = ReportEngine.DrawHeader(g, title, templeName, templeAddress,
                                            _reportInfo.FromDate, _reportInfo.ToDate,
                                            titleFont, subtitleFont, _startX, _pageY, pageW,
                                            pageNumber, totalPages)
        End Sub

        Public Shared Sub ShowPreview(fromDate As Date, toDate As Date, Optional owner As IWin32Window = Nothing, Optional mode As ReportModes = ReportModes.Detailed, Optional manualOpeningBalance As Decimal? = Nothing, Optional fundID As Integer? = Nothing, Optional bankID As Integer? = Nothing)
            Try
                Dim doc As New IncomeExpenseReport(fromDate, toDate, mode, manualOpeningBalance, fundID, bankID)
                Using ppd As New PrintPreviewDialog()
                    ppd.Document = doc
                    ppd.WindowState = FormWindowState.Maximized
                    ppd.Width = 1200 : ppd.Height = 800
                    ppd.PrintPreviewControl.Rows = 1
                    ppd.PrintPreviewControl.Columns = 1
                    ppd.PrintPreviewControl.AutoZoom = True
                    If owner IsNot Nothing Then
                        ppd.ShowDialog(owner)
                    Else
                        ppd.ShowDialog()
                    End If
                End Using
            Catch ex As Exception
                MessageBox.Show("ไม่สามารถเปิดดูรายงานได้: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace
