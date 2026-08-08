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

        ''' <summary>
        ''' กำหนดทิศทางกระดาษ: True = A4 แนวนอน (ค่าเริ่มต้น), False = A4 แนวตั้ง
        ''' คอลัมน์ทั้งหมดคำนวณจาก e.MarginBounds.Width แบบอัตโนมัติ จึงรองรับได้ทั้ง 2 แบบ
        ''' </summary>
        Public Property UseLandscape As Boolean = True

        ' Report date range - MUST be set via constructor
        Private ReadOnly _fromDate As Date
        Private ReadOnly _toDate As Date
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
        Private _templateInfo As TempleSettingInfo

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
        Private Const FinalSignatureBlockHeight As Integer = 148  ' เท่ากับ ReportEngine.totalSignatureHeight (24+52+14+26+8+24) เพื่อกันส่วนเซ็นต์ล้นขอบล่าง
        Private Const FinalFooterGapHeight As Integer = 19       ' ตรงกับระยะล่างสุดของบล็อกเซ็นต์จากขอบล่าง (0.5 ซม. ≈ 19px) ใน ReportEngine
        Private Const SectionGap As Integer = 12               ' Gap between left/right sections

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
            ' Normalize dates to Gregorian for consistent internal use
            _fromDate = Db.NormalizeGregorianDate(fromDate)
            _toDate = Db.NormalizeGregorianDate(toDate)
            
            ' DEBUG: Verify received dates
            System.Diagnostics.Debug.WriteLine("[DEBUG IncomeExpenseReport.New] Received fromDate: " & fromDate & " (Normalized: " & _fromDate & ")")
            System.Diagnostics.Debug.WriteLine("[DEBUG IncomeExpenseReport.New] Received toDate: " & toDate & " (Normalized: " & _toDate & ")")
            
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
            ' ระยะขอบปลอดภัย 0.5 นิ้ว (50/96 นิ้ว) ทุกด้าน กันเครื่องพิมพ์รุ่นเก่าตัดขอบภาพ
            Dim pageMargins As New Margins(50, 50, 50, 50)

            Me.DefaultPageSettings.PaperSize = a4Paper
            Me.DefaultPageSettings.Margins = pageMargins
            Me.DefaultPageSettings.Landscape = UseLandscape

            Me.PrinterSettings.DefaultPageSettings.PaperSize = a4Paper
            Me.PrinterSettings.DefaultPageSettings.Margins = pageMargins
            Me.PrinterSettings.DefaultPageSettings.Landscape = UseLandscape
        End Sub

        Private Sub InitFonts()
            _theme = ReportTheme.CreateDefault()
        End Sub

        Private Sub LoadData()
            Try
                ' โหลดข้อมูลรายงานทั้งหมดผ่าน ReportEngine.BuildReportDataset
                ' (query + การคำนวณยอดยกมา/ยอดรวม/ยอดคงเหลือ รวมศูนย์กลางที่ ReportEngine)
                Dim ds = ReportEngine.BuildReportDataset(_fromDate, _toDate,
                                                         (_mode = ReportModes.Summary),
                                                         _manualOpeningBalance,
                                                         _fundID,
                                                         _bankID)

                _templateInfo = ds.TemplateInfo
                _incomeRows = ds.IncomeRows
                _expenseRows = ds.ExpenseRows
                _totalIncome = ds.TotalIncome
                _totalExpense = ds.TotalExpense
                _openingBalance = ds.OpeningBalance
                _reportGrandTotal = ds.GrandTotal
                _balance = ds.Balance

                ' DEBUG: Log what we got
                System.Diagnostics.Debug.WriteLine("[DEBUG IncomeExpenseReport.LoadData] _templateInfo.TempleName: '" & _templateInfo.TempleName & "'")

                ' Initialize ReportInfo for header rendering using shared TemplateInfo
                ' Title is now simplified; the type indicator is moved to the date range line
                Dim reportTitle = "สรุปบัญชีรายรับ - รายจ่าย"
                _reportInfo = New ReportInfo(reportTitle, _templateInfo.TempleName, _templateInfo.TempleAddress, _fromDate, _toDate)
            Catch ex As Exception
                Throw
            End Try
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
            Dim displayYear = d.Year
            If displayYear < 2400 Then displayYear += 543
            Return ThaiMonthAbbr(d.Month) & "-" & ToThaiNumerals((displayYear Mod 100).ToString())
        End Function

        Private Function ToBuddhistFull(ByVal d As Date) As String
            Dim displayYear = d.Year
            If displayYear < 2400 Then displayYear += 543
            Return ToThaiNumerals(d.Day.ToString()) & " " & ThaiMonthFull(d.Month) & " พ.ศ. " & ToThaiNumerals(displayYear.ToString())
        End Function

        Private Function ThaiMonthFull(ByVal m As Integer) As String
            Return ReportEngine.ThaiMonthFull(m)
        End Function

        Private Function ToBuddhistYearThai(ByVal y As Integer) As String
            Dim yB = y
            If yB < 2400 Then yB += 543
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
            ' ระยะขอบปลอดภัย 0.5 นิ้ว ทุกด้าน กันฮาร์ดแวร์มาร์จิ้นของเครื่องพิมพ์เก่าตัดขอบ
            e.PageSettings.Margins = New Margins(50, 50, 50, 50)
            e.PageSettings.Landscape = UseLandscape
        End Sub

        Protected Overrides Sub OnPrintPage(e As PrintPageEventArgs)
            MyBase.OnPrintPage(e)
            Dim g As Graphics = e.Graphics
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

            ' Use LayoutConfig for consistent layout calculations
            ' ระยะขอบตรงกับ PageSettings (50,50,50,50) เพื่อให้คอลัมน์พอดีกับพื้นที่พิมพ์จริง
            _layout = LayoutConfig.CreateA4Landscape(50, 50, 50, 50)

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
            ' We pass _fromDate and _toDate explicitly to ensure the header matches the selected range.
            DrawHeader(g, pageW, _pageIndex + 1, _totalPages, _fromDate, _toDate)

            Dim colW1 = CInt(usableW * 0.16)
            Dim colW2 = CInt(usableW * 0.1)
            Dim colW3 = usableW - colW1 - colW2 - CInt(usableW * 0.24)
            Dim colW4 = CInt(usableW * 0.24)
            Dim rowH = 28

            ' Only draw table headers if we have transactions to print on THIS page
            If _rowIndex < totalRows Then
                DrawTableHeader(g, rowH, colW1, colW2, colW3, colW4)
            End If

            ' Draw rows until we run out of space or data
            While _rowIndex < totalRows
                Dim y = _pageY
                Dim hasLeft = (_rowIndex < _incomeRows.Count)
                Dim hasRight = (_rowIndex < _expenseRows.Count)

                ' Calculate required height for this row (based on description wrapping)
                Dim leftDesc = If(hasLeft, _incomeRows(_rowIndex).Description, "")
                Dim rightDesc = If(hasRight, _expenseRows(_rowIndex).Description, "")
                
                Dim leftH = ReportEngine.MeasureTextHeight(g, leftDesc, _theme.RowFont, colW3 - 6)
                Dim rightH = ReportEngine.MeasureTextHeight(g, rightDesc, _theme.RowFont, colW3 - 6)
                
                ' Use standard row height as minimum, and max of both sides for dynamic height
                Dim currentRowH = Math.Max(rowH, Math.Max(leftH, rightH))
                
                ' Check if this row fits in the current page
                If _pageY + currentRowH > _pageBottom Then
                    ' No space for this row - trigger new page
                    e.HasMorePages = True
                    _pageIndex += 1
                    Return
                End If

                ' Render Income side: Render row data if available, else render empty grid
                g.DrawRectangle(_theme.BlackPen, _leftX, y, usableW, currentRowH)
                g.DrawRectangle(_theme.BlackPen, _leftX, y, colW1, currentRowH)
                g.DrawRectangle(_theme.BlackPen, _leftX + colW1, y, colW2, currentRowH)
                g.DrawRectangle(_theme.BlackPen, _leftX + colW1 + colW2, y, colW3, currentRowH)
                g.DrawRectangle(_theme.BlackPen, _leftX + colW1 + colW2 + colW3, y, colW4, currentRowH)
                g.DrawLine(_theme.BlackPen, _leftX + colW1 + colW2 + colW3, y, _leftX + colW1 + colW2 + colW3, y + currentRowH)

                If hasLeft Then
                    Dim row = _incomeRows(_rowIndex)
                    Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                    Dim fmtL As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center}
                    Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}

                    g.DrawString(ToBuddhistDateShort(row.TranDate), _theme.RowFont, Brushes.Black, New RectangleF(_leftX, y, colW1, currentRowH), fmt)
                    g.DrawString(ToThaiNumerals(row.DayCode.ToString()), _theme.RowFont, Brushes.Black, New RectangleF(_leftX + colW1, y, colW2, currentRowH), fmt)
                    g.DrawString(row.Description, _theme.RowFont, Brushes.Black,
                                 New RectangleF(_leftX + colW1 + colW2 + 3, y, colW3 - 6, currentRowH), fmtL)
                    g.DrawString(FormatThaiAmount(row.Amount), _theme.RowFont, If(row.IsCarryForward, Brushes.Red, Brushes.Black),
                                 New RectangleF(_leftX + colW1 + colW2 + colW3, y, colW4 - 4, currentRowH), fmtR)
                End If

                ' Render Expense side: Render row data if available, else render empty grid
                g.DrawRectangle(_theme.BlackPen, _rightX, y, usableW, currentRowH)
                g.DrawRectangle(_theme.BlackPen, _rightX, y, colW1, currentRowH)
                g.DrawRectangle(_theme.BlackPen, _rightX + colW1, y, colW2, currentRowH)
                g.DrawRectangle(_theme.BlackPen, _rightX + colW1 + colW2, y, colW3, currentRowH)
                g.DrawRectangle(_theme.BlackPen, _rightX + colW1 + colW2 + colW3, y, colW4, currentRowH)
                g.DrawLine(_theme.BlackPen, _rightX + colW1 + colW2 + colW3, y, _rightX + colW1 + colW2 + colW3, y + currentRowH)

                If hasRight Then
                    Dim row = _expenseRows(_rowIndex)
                    Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                    Dim fmtL As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center}
                    Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}

                    g.DrawString(ToBuddhistDateShort(row.TranDate), _theme.RowFont, Brushes.Black, New RectangleF(_rightX, y, colW1, currentRowH), fmt)
                    g.DrawString(ToThaiNumerals(row.DayCode.ToString()), _theme.RowFont, Brushes.Black, New RectangleF(_rightX + colW1, y, colW2, currentRowH), fmt)
                    g.DrawString(row.Description, _theme.RowFont, Brushes.Black,
                                 New RectangleF(_rightX + colW1 + colW2 + 3, y, colW3 - 6, currentRowH), fmtL)
                    g.DrawString(FormatThaiAmount(row.Amount), _theme.RowFont, Brushes.Black,
                                 New RectangleF(_rightX + colW1 + colW2 + colW3, y, colW4 - 4, currentRowH), fmtR)
                End If

                _rowIndex += 1
                _pageY += currentRowH
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

                ' We have enough space - draw summaries and signatures
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


        Private Sub DrawMidSummary(g As Graphics, rowH As Integer, c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer, usableW As Integer)
            Dim y = _pageY
            Dim fmtC As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
            Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}
            
            ' Calculate running totals
            Dim runningIncome As Decimal = 0D
            For i = 0 To Math.Min(_rowIndex, _incomeRows.Count) - 1
                runningIncome += _incomeRows(i).Amount
            Next

            Dim runningExpense As Decimal = 0D
            For i = 0 To Math.Min(_rowIndex, _expenseRows.Count) - 1
                runningExpense += _expenseRows(i).Amount
            Next

            ' LEFT Section Mid-Summary
            g.DrawRectangle(_theme.BlackPen, _leftX, y, usableW, rowH)
            g.DrawLine(_theme.BlackPen, _leftX + c1 + c2, y, _leftX + c1 + c2, y + rowH)
            g.DrawLine(_theme.BlackPen, _leftX + c1 + c2 + c3, y, _leftX + c1 + c2 + c3, y + rowH)
            g.DrawString("รวมทั้งสิ้น", _theme.BoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(runningIncome), _theme.BoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' RIGHT Section Mid-Summary
            g.DrawRectangle(_theme.BlackPen, _rightX, y, usableW, rowH)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2, y, _rightX + c1 + c2, y + rowH)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2 + c3, y, _rightX + c1 + c2 + c3, y + rowH)
            g.DrawString("รวมทั้งสิ้น", _theme.BoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(runningExpense), _theme.BoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)
            
            _pageY += rowH
        End Sub

        Private Sub DrawSummaries(g As Graphics, rowH As Integer, c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer,
                                  Optional blankCount As Integer = 0)
            Dim y = _pageY
            Dim usableW = _leftSectionWidth
            Dim fmtC As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
            Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}
            Dim fmtL As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center, .FormatFlags = StringFormatFlags.NoWrap}
            Dim balanceLabel = BuildCarryForwardLabel()

            ' --- ROW 1: Totals (รวมรายรับ / รวมรายจ่าย) ---
            ' LEFT
            g.DrawRectangle(_theme.BlackPen, _leftX, y, usableW, rowH)
            g.DrawLine(_theme.BlackPen, _leftX + c1 + c2, y, _leftX + c1 + c2, y + rowH)
            g.DrawLine(_theme.BlackPen, _leftX + c1 + c2 + c3, y, _leftX + c1 + c2 + c3, y + rowH)
            g.DrawString("รวมรายรับ", _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(_totalIncome), _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)
            ' RIGHT
            g.DrawRectangle(_theme.BlackPen, _rightX, y, usableW, rowH)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2, y, _rightX + c1 + c2, y + rowH)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2 + c3, y, _rightX + c1 + c2 + c3, y + rowH)
            g.DrawString("รวมรายจ่าย", _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(_totalExpense), _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)
            
            y += rowH

            ' --- ROW 2: Adjustments (Income รวมทั้งสิ้น / Expense ยอดคงเหลือยกไป) ---
            Dim netBalance = _totalIncome - _totalExpense

            ' LEFT (Income รวมทั้งสิ้น - Aligned with Expense carry forward)
            g.DrawRectangle(_theme.BlackPen, _leftX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString("รวมทั้งสิ้น", _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(netBalance), _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' RIGHT (ยอดคงเหลือยกไป)
            g.DrawRectangle(_theme.BlackPen, _rightX, y, usableW, rowH)
            g.DrawLine(_theme.BlackPen, _rightX + c1 + c2 + c3, y, _rightX + c1 + c2 + c3, y + rowH)
            Dim carryLabelX = _rightX + 6
            Dim carryLabelWidth = c1 + c2 + c3 - 12
            Using fittedCarryFont = CreateFittedBoldFont(g, balanceLabel, _theme.BigBoldFont, carryLabelWidth, 9.5F)
                g.DrawString(balanceLabel, fittedCarryFont, Brushes.Red,
                             New RectangleF(carryLabelX, y, carryLabelWidth, rowH), fmtL)
            End Using
            g.DrawString(FormatThaiAmount(netBalance), _theme.BigBoldFont, Brushes.Red,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)
            
            y += rowH

            ' --- ROW 3: Final Grand Totals (Expense side only) ---
            ' LEFT - Blank (No drawing)

            ' RIGHT - รวมทั้งสิ้น
            g.DrawRectangle(_theme.BlackPen, _rightX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString("รวมทั้งสิ้น", _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(_totalIncome), _theme.BigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            _pageY = y + rowH
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

        ''' <summary>
        ''' วาดบล็อกลายเซ็น (ซ้าย: ตรวจถูกต้องแล้ว/เจ้าอาวาส — ขวา: ผู้จัดทำบัญชี/ไวยาวัจกร)
        ''' ผ่าน ReportEngine.DrawSignatureBlock อย่างเดียว (ไม่มีการวาด inline หรือ layout ทับเอง)
        ''' ชื่อเจ้าอาวาส/ผู้จัดทำบัญชี ผูกจาก data properties ของ ReportEngine (GetTemplateInfo) ภายใน ReportEngine.vb
        ''' เส้นประวาดเพียงเส้นเดียว และชื่อแสดงเป็น ( {ชื่อ} ) — fallback ( .................................... ) เฉพาะเมื่อชื่อว่าง
        ''' </summary>
        Private Sub DrawSignatures(g As Graphics, rowH As Integer, c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer, usableW As Integer)
            ' layout + การผูกชื่อผู้ลงนามจัดการภายใน ReportEngine.DrawSignatureBlock ทั้งหมด
            _pageY = ReportEngine.DrawSignatureBlock(g,
                "ตรวจถูกต้องแล้ว", "เจ้าอาวาส",
                "ผู้จัดทำบัญชี", "ไวยาวัจกร",
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

        Private Sub DrawHeader(g As Graphics, pageW As Integer, pageNumber As Integer, totalPages As Integer, fromDate As Date, toDate As Date)
            ' Delegate to shared ReportEngine.DrawHeader for consistent rendering
            ' Uses _reportInfo which is initialized in LoadData, but we pass dates explicitly
            ' to guarantee they match what was passed to the constructor.
            If _reportInfo Is Nothing Then
                ' Fallback if _reportInfo not initialized - use shared TemplateInfo (already has defaults)
                Dim reportTitle = "สรุปบัญชีรายรับ - รายจ่าย"
                _templateInfo = ReportEngine.GetTemplateInfo()
                _reportInfo = New ReportInfo(reportTitle, _templateInfo.TempleName, _templateInfo.TempleAddress, fromDate, toDate)
            End If

            ' Ensure we have valid values before calling engine
            Dim title = If(String.IsNullOrWhiteSpace(_reportInfo.ReportTitle), "รายงานรายรับ-รายจ่าย", _reportInfo.ReportTitle)
            Dim templeName = If(String.IsNullOrWhiteSpace(_reportInfo.TempleName), "วัดแหลมยาง", _reportInfo.TempleName)
            Dim templeAddress = If(String.IsNullOrWhiteSpace(_reportInfo.TempleAddress), "ต.ป่ามะคาบ อ.เมืองพิจิตร จ.พิจิตร", _reportInfo.TempleAddress)

            ' Safety check for fonts
            Dim titleFont = If(_theme?.TitleFont, New Font("Tahoma", 16, FontStyle.Bold))
            Dim subtitleFont = If(_theme?.SubTitleFont, New Font("Tahoma", 12, FontStyle.Bold))

            Dim reportType = If(_mode = ReportModes.Summary, "(แบบย่อ)", "(แบบละเอียด)")

            _pageY = ReportEngine.DrawHeader(g, title, templeName, templeAddress,
                                            fromDate, toDate,
                                            titleFont, subtitleFont, _startX, _pageY, pageW,
                                            pageNumber, totalPages, reportType)
        End Sub

        ' จำเครื่องพิมพ์ที่ผู้ใช้เลือกไว้ (ระดับ session) — เมื่อตั้งค่าแล้ว งานพิมพ์/พรีวิวครั้งต่อไป
        ' จะใช้เครื่องพิมพ์เดิมทันทีโดยไม่ต้องถามซ้ำ จนกว่าผู้ใช้จะเปลี่ยนที่ปุ่ม "🖨️ พิมพ์"
        Private Shared _selectedPrinterName As String = ""

        ''' <summary>
        ''' เปิด Print Preview ตรง ๆ (ไม่ถามเครื่องพิมพ์ก่อน) — ผู้ใช้เลือกเครื่องพิมพ์ได้จาก
        ''' ปุ่ม Print ในแถบเครื่องมือของ PrintPreviewDialog
        ''' ถ้าเคยเลือกเครื่องพิมพ์ไว้ (_selectedPrinterName) ระบบจะตั้งเครื่องพิมพ์นั้นให้อัตโนมัติ
        ''' </summary>
        Public Shared Sub ShowPreview(fromDate As Date, toDate As Date, Optional owner As IWin32Window = Nothing, Optional mode As ReportModes = ReportModes.Detailed, Optional manualOpeningBalance As Decimal? = Nothing, Optional fundID As Integer? = Nothing, Optional bankID As Integer? = Nothing)
            Try
                Dim doc As New IncomeExpenseReport(fromDate, toDate, mode, manualOpeningBalance, fundID, bankID)
                ApplySessionPrinter(doc)

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

        ''' <summary>
        ''' เปิด PrintDialog ให้เลือกเครื่องพิมพ์ แล้วพิมพ์โดยตรง (ไม่มี Print Preview)
        ''' เมื่อเลือกเครื่องพิมพ์แล้วจะถูกจำไว้ (_selectedPrinterName) สำหรับงานพิมพ์/พรีวิวครั้งต่อไป
        ''' </summary>
        Public Shared Sub ShowPrint(fromDate As Date, toDate As Date, Optional owner As IWin32Window = Nothing, Optional mode As ReportModes = ReportModes.Detailed, Optional manualOpeningBalance As Decimal? = Nothing, Optional fundID As Integer? = Nothing, Optional bankID As Integer? = Nothing)
            Try
                Dim doc As New IncomeExpenseReport(fromDate, toDate, mode, manualOpeningBalance, fundID, bankID)

                Using pd As New PrintDialog()
                    pd.Document = doc
                    pd.AllowSomePages = True
                    pd.UseEXDialog = True  ' ใช้กล่องโต้ตอบ Windows ดั้งเดิม — แสดงเครื่องพิมพ์ทั้งหมด (USB/Network/PDF/Thermal)
                    pd.PrinterSettings.MinimumPage = 1
                    pd.PrinterSettings.MaximumPage = 9999
                    pd.PrinterSettings.FromPage = 1
                    pd.PrinterSettings.ToPage = 9999

                    If pd.ShowDialog(If(owner, Nothing)) <> DialogResult.OK Then Return

                    ' จำเครื่องพิมพ์ที่ผู้ใช้เลือกไว้ (session-level) — ครั้งหน้าไม่ต้องถามซ้ำ
                    _selectedPrinterName = pd.PrinterSettings.PrinterName

                    ' ใช้การตั้งค่าเครื่องพิมพ์ที่ผู้ใช้เลือกกับเอกสาร
                    doc.PrinterSettings = pd.PrinterSettings
                    doc.DefaultPageSettings = pd.PrinterSettings.DefaultPageSettings
                    doc.ConfigurePageSettings()

                    doc.Print()
                End Using

                MessageBox.Show("พิมพ์รายงานไปยังเครื่องพิมพ์ " & doc.PrinterSettings.PrinterName & " เรียบร้อยแล้ว",
                                "พิมพ์สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("ไม่สามารถพิมพ์รายงานได้: " & ex.Message,
                                "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ''' <summary>
        ''' ตั้งเครื่องพิมพ์ที่เคยเลือกไว้ (session) ให้กับเอกสารก่อนเปิดพรีวิว/พิมพ์
        ''' เพื่อให้พรีวิวแสดงเครื่องพิมพ์ที่ผู้ใช้ต้องการได้ทันทีโดยไม่ต้องถามซ้ำ
        ''' </summary>
        Private Shared Sub ApplySessionPrinter(doc As IncomeExpenseReport)
            If String.IsNullOrEmpty(_selectedPrinterName) Then Return
            Try
                doc.PrinterSettings.PrinterName = _selectedPrinterName
                doc.ConfigurePageSettings()
            Catch
                ' เครื่องพิมพ์ที่เคยเลือกอาจถูกถอดออกจากระบบแล้ว — รีเซ็ตเป็นเครื่องพิมพ์เริ่มต้น
                _selectedPrinterName = ""
            End Try
        End Sub

        ''' <summary>
        ''' บันทึกรายงานเป็นไฟล์ PDF (A4) ผ่านเครื่องพิมพ์เสมือน "Microsoft Print to PDF"
        ''' ที่ติดตั้งมาพร้อม Windows 10/11 — ไม่ต้องพึ่งไลบรารีภายนอก และผลลัพธ์เป็น PDF ที่แก้ไขไม่ได้
        ''' </summary>
        Public Sub SaveToPdf(filePath As String)
            If String.IsNullOrWhiteSpace(filePath) Then
                Throw New ApplicationException("ไม่ได้ระบุตำแหน่งไฟล์ PDF")
            End If

            ' ค้นหาเครื่องพิมพ์เสมือน Microsoft Print to PDF
            Dim pdfPrinter = "Microsoft Print to PDF"
            Dim found = False
            Try
                For Each name As String In System.Drawing.Printing.PrinterSettings.InstalledPrinters
                    If String.Equals(name, pdfPrinter, StringComparison.OrdinalIgnoreCase) Then
                        found = True
                        Exit For
                    End If
                Next
            Catch
                found = False
            End Try

            If Not found Then
                Throw New ApplicationException("ไม่พบเครื่องพิมพ์ 'Microsoft Print to PDF' ในระบบ" & vbCrLf &
                                               "กรุณาเปิดใช้งานที่: การตั้งค่า Windows > อุปกรณ์ > เครื่องพิมพ์และสแกนเนอร์ > Microsoft Print to PDF")
            End If

            Try
                Me.PrinterSettings.PrinterName = pdfPrinter
                ' กำหนดให้พิมพ์ลงไฟล์โดยตรง (ไม่เปิดกล่องบันทึกซ้ำ) — ใช้กับไดรเวอร์ Microsoft Print to PDF
                Me.PrinterSettings.PrintToFile = True
                Me.PrinterSettings.PrintFileName = filePath

                ' สลับเครื่องพิมพ์แล้ว: เตรียมกระดาษ A4 + ระยะขอบปลอดภัยของเครื่องพิมพ์นั้นใหม่
                ConfigurePageSettings()
                Me.Print()
            Catch ex As Exception
                Me.PrinterSettings.PrintToFile = False
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' เปิดกล่องบันทึกไฟล์ แล้วสร้าง PDF (A4) ของรายงานที่เลือก
        ''' </summary>
        Public Shared Sub ShowSavePdfDialog(fromDate As Date, toDate As Date, Optional owner As IWin32Window = Nothing, Optional mode As ReportModes = ReportModes.Detailed, Optional manualOpeningBalance As Decimal? = Nothing, Optional fundID As Integer? = Nothing, Optional bankID As Integer? = Nothing)
            Using sfd As New SaveFileDialog()
                sfd.Filter = "ไฟล์ PDF|*.pdf"
                sfd.FilterIndex = 1
                sfd.Title = "บันทึกเป็น PDF (A4)"
                sfd.FileName = "รายงาน_" & fromDate.ToString("yyyyMMdd") & "_" & toDate.ToString("yyyyMMdd") & ".pdf"
                sfd.OverwritePrompt = True
                sfd.AddExtension = True
                sfd.DefaultExt = "pdf"

                If sfd.ShowDialog(owner) <> DialogResult.OK Then Return

                Try
                    Dim doc As New IncomeExpenseReport(fromDate, toDate, mode, manualOpeningBalance, fundID, bankID)
                    doc.SaveToPdf(sfd.FileName)
                    MessageBox.Show("บันทึกไฟล์ PDF เรียบร้อยแล้ว:" & vbCrLf & sfd.FileName,
                                    "บันทึกสำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("บันทึก PDF ไม่สำเร็จ:" & vbCrLf & ex.Message,
                                    "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Sub
    End Class
End Namespace
