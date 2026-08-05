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
        Private _templeName As String = ""
        Private _templeAddress As String = ""
        Private _abbotPersonnelID As Integer? = Nothing
        Private _abbotName As String = ""
        Private _waiyawatPersonnelID As Integer? = Nothing
        Private _waiyawatName As String = ""
        Private _inspectorName As String = ""

        Private _rowFont As Font
        Private _headerFont As Font
        Private _titleFont As Font
        Private _subTitleFont As Font
        Private _boldFont As Font
        Private _bigBoldFont As Font
        Private _blackPen As Pen
        Private _pen2 As Pen

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
        Private Const FinalSignatureBlockHeight As Integer = 360  ' Height for 2 signature blocks with proper spacing
        Private Const FinalFooterGapHeight As Integer = 24

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
                Return _abbotName
            End Get
        End Property

        ''' <summary>
        ''' ชื่อไวยาวัจกร (สำหรับแสดงในรายงาน)
        ''' </summary>
        Public ReadOnly Property WaiyawatName As String
            Get
                Return _waiyawatName
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
            _rowFont = New Font("Tahoma", 10.0!)
            _boldFont = New Font("Tahoma", 10.0!, FontStyle.Bold)
            _headerFont = New Font("Tahoma", 10.0!, FontStyle.Bold)
            _bigBoldFont = New Font("Tahoma", 14.0!, FontStyle.Bold)
            _titleFont = New Font("Tahoma", 16.0!, FontStyle.Bold)
            _subTitleFont = New Font("Tahoma", 12.0!, FontStyle.Bold)
            _blackPen = New Pen(Color.Black, 1)
            _pen2 = New Pen(Color.Black, 2)
        End Sub

        Private Sub LoadData()
            Try
                Db.EnsureSchema()
                Using conn = Db.OpenConn()
                    LoadTempleInfo(conn)
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

        Private Sub LoadTempleInfo(conn As OleDbConnection)
            ' STEP 1: Load TempleSetting from database
            Try
                System.Diagnostics.Debug.WriteLine("[TRACE] Step 1: Loading TempleSetting...")
                Dim dt = Db.GetTable(conn, "SELECT TOP 1 * FROM TempleSetting ORDER BY ID DESC")
                If dt.Rows.Count > 0 Then
                    Dim r = dt.Rows(0)
                    ' Print all TempleSetting columns
                    For Each col As DataColumn In dt.Columns
                        System.Diagnostics.Debug.WriteLine("[TRACE]   TS Column: " & col.ColumnName & " = " & r(col).ToString())
                    Next

                    ' Temple basic info
                    If Not IsDBNull(r!TempleName) Then _templeName = CStr(r!TempleName)
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 1a: TempleCode = " & If(dt.Columns.Contains("TempleCode") AndAlso Not IsDBNull(r!TempleCode), r!TempleCode.ToString(), "N/A"))
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 1b: TempleName = " & _templeName)

                    ' STEP 1c: Check AbbotPersonnelID and WaiyawatPersonnelID
                    Dim abbotID As Object = Nothing
                    Dim waiyawatID As Object = Nothing
                    If dt.Columns.Contains("AbbotPersonnelID") Then abbotID = r!AbbotPersonnelID
                    If dt.Columns.Contains("WaiyawatPersonnelID") Then waiyawatID = r!WaiyawatPersonnelID
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 1c: AbbotPersonnelID = " & If(abbotID Is Nothing OrElse abbotID Is DBNull.Value, "NULL", abbotID.ToString()))
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 1c: WaiyawatPersonnelID = " & If(waiyawatID Is Nothing OrElse waiyawatID Is DBNull.Value, "NULL", waiyawatID.ToString()))

                    ' Store PersonnelIDs in private fields
                    If abbotID IsNot Nothing AndAlso Not IsDBNull(abbotID) Then
                        _abbotPersonnelID = Convert.ToInt32(abbotID)
                    End If
                    If waiyawatID IsNot Nothing AndAlso Not IsDBNull(waiyawatID) Then
                        _waiyawatPersonnelID = Convert.ToInt32(waiyawatID)
                    End If

                    ' STEP 1d: Try to read AbbotName (legacy column - might be dropped)
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 1d: Checking for AbbotName column in TempleSetting...")
                    If dt.Columns.Contains("AbbotName") Then
                        System.Diagnostics.Debug.WriteLine("[TRACE] Step 1d: AbbotName EXISTS in TempleSetting = " & If(IsDBNull(r!AbbotName), "NULL", r!AbbotName.ToString()))
                    Else
                        System.Diagnostics.Debug.WriteLine("[TRACE] Step 1d: AbbotName column DOES NOT EXIST in TempleSetting (may have been dropped)")
                    End If

                    ' Address building
                    Dim templeAddressLine = ""
                    If Not IsDBNull(r!TempleAddress) Then templeAddressLine = CStr(r!TempleAddress).Trim()
                    If Not IsDBNull(r!AbbotName) Then _inspectorName = CStr(r!AbbotName)
                    Dim tambon = "", amphoe = "", prov = "", post = ""
                    If Not IsDBNull(r!Tambon) Then tambon = CStr(r!Tambon)
                    If Not IsDBNull(r!Amphoe) Then amphoe = CStr(r!Amphoe)
                    If Not IsDBNull(r!Province) Then prov = CStr(r!Province)
                    If Not IsDBNull(r!PostCode) Then post = CStr(r!PostCode)
                    Dim addressParts As New List(Of String)()
                    If Not String.IsNullOrWhiteSpace(templeAddressLine) Then addressParts.Add(templeAddressLine)
                    If Not String.IsNullOrWhiteSpace(tambon) Then addressParts.Add("ต." & tambon.Trim())
                    If Not String.IsNullOrWhiteSpace(amphoe) Then addressParts.Add("อ." & amphoe.Trim())
                    If Not String.IsNullOrWhiteSpace(prov) Then addressParts.Add("จ." & prov.Trim())
                    If Not String.IsNullOrWhiteSpace(post) Then addressParts.Add(post.Trim())
                    _templeAddress = String.Join(" ", addressParts).Trim()
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 1e: TempleAddress = " & _templeAddress)
                Else
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 1: NO TempleSetting ROWS FOUND")
                End If
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("[TRACE] Step 1 ERROR: " & ex.ToString())
            End Try

            ' STEP 2: Load AbbotName and WaiyawatName via JOIN
            System.Diagnostics.Debug.WriteLine("[TRACE] Step 2: Loading Abbot/Waiyawat via JOIN...")
            Try
                ' ใช้ Personnel.FullName โดยตรงเพื่อหลีกเลี่ยงปัญหา NULL จาก Title/FirstName/LastName
                Dim sql = "SELECT PA.FullName AS AbbotName, PW.FullName AS WaiyawatName " &
                          "FROM (TempleSetting AS TS " &
                          "LEFT JOIN Personnel AS PA ON TS.AbbotPersonnelID = PA.PersonnelID) " &
                          "LEFT JOIN Personnel AS PW ON TS.WaiyawatPersonnelID = PW.PersonnelID"
                System.Diagnostics.Debug.WriteLine("[TRACE] Step 2 SQL: " & sql)
                Dim dt = Db.GetTable(conn, sql)
                System.Diagnostics.Debug.WriteLine("[TRACE] Step 2: JOIN returned " & dt.Rows.Count & " rows")
                If dt.Rows.Count > 0 Then
                    Dim r = dt.Rows(0)
                    For Each col As DataColumn In dt.Columns
                        System.Diagnostics.Debug.WriteLine("[TRACE]   JOIN Column: " & col.ColumnName & " = " & If(IsDBNull(r(col)), "NULL", r(col).ToString()))
                    Next

                    ' STEP 3: Assign to _abbotName and _waiyawatName
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 3: Before assignment...")
                    System.Diagnostics.Debug.WriteLine("[TRACE]   r!AbbotName = " & If(IsDBNull(r!AbbotName), "NULL", r!AbbotName.ToString()))
                    System.Diagnostics.Debug.WriteLine("[TRACE]   r!WaiyawatName = " & If(IsDBNull(r!WaiyawatName), "NULL", r!WaiyawatName.ToString()))

                    _abbotName = If(IsDBNull(r!AbbotName), "", r!AbbotName.ToString())
                    _waiyawatName = If(IsDBNull(r!WaiyawatName), "", r!WaiyawatName.ToString())

                    ' STEP 4: After assignment - verify fields
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 4: After assignment to _fields...")
                    System.Diagnostics.Debug.WriteLine("[TRACE]   _abbotName = '" & _abbotName & "'")
                    System.Diagnostics.Debug.WriteLine("[TRACE]   _waiyawatName = '" & _waiyawatName & "'")
                Else
                    System.Diagnostics.Debug.WriteLine("[TRACE] Step 2: NO ROWS returned from JOIN - TempleSetting may be empty")
                End If
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("[TRACE] Step 2 ERROR: " & ex.ToString())
            End Try
            System.Diagnostics.Debug.WriteLine("[TRACE] LoadTempleInfo COMPLETE. _abbotName='" & _abbotName & "', _waiyawatName='" & _waiyawatName & "'")
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
            Select Case m
                Case 1 : Return "ม.ค."
                Case 2 : Return "ก.พ."
                Case 3 : Return "มี.ค."
                Case 4 : Return "เม.ย."
                Case 5 : Return "พ.ค."
                Case 6 : Return "มิ.ย."
                Case 7 : Return "ก.ค."
                Case 8 : Return "ส.ค."
                Case 9 : Return "ก.ย."
                Case 10 : Return "ต.ค."
                Case 11 : Return "พ.ย."
                Case 12 : Return "ธ.ค."
                Case Else : Return ""
            End Select
        End Function

        Private Function ToBuddhistDateShort(ByVal d As Date) As String
            Return ThaiMonthAbbr(d.Month) & "-" & ToThaiNumerals(((d.Year + 543) Mod 100).ToString())
        End Function

        Private Function ToBuddhistFull(ByVal d As Date) As String
            Return ToThaiNumerals(d.Day.ToString()) & " " & ThaiMonthFull(d.Month) & " พ.ศ. " & ToThaiNumerals((d.Year + 543).ToString())
        End Function

        Private Function ThaiMonthFull(ByVal m As Integer) As String
            Select Case m
                Case 1 : Return "มกราคม"
                Case 2 : Return "กุมภาพันธ์"
                Case 3 : Return "มีนาคม"
                Case 4 : Return "เมษายน"
                Case 5 : Return "พฤษภาคม"
                Case 6 : Return "มิถุนายน"
                Case 7 : Return "กรกฎาคม"
                Case 8 : Return "สิงหาคม"
                Case 9 : Return "กันยายน"
                Case 10 : Return "ตุลาคม"
                Case 11 : Return "พฤศจิกายน"
                Case 12 : Return "ธันวาคม"
                Case Else : Return ""
            End Select
        End Function

        Private Function ToBuddhistYearThai(ByVal y As Integer) As String
            Dim yB = y + 543
            Return ToThaiNumerals(yB.ToString())
        End Function

        Private Function FormatThaiAmount(value As Decimal) As String
            Return ToThaiNumerals(value.ToString("#,##0"))
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

            Dim pageW = e.PageBounds.Width
            _startX = e.MarginBounds.Left
            _pageY = e.MarginBounds.Top
            _pageBottom = e.MarginBounds.Bottom
            _leftX = _startX
            _rightSectionWidth = CInt((e.MarginBounds.Width) / 2) - 6
            _leftSectionWidth = _rightSectionWidth
            _rightX = _startX + _leftSectionWidth + 12
            Dim usableW = _leftSectionWidth

            ' [REPORT DEBUG] - Pagination accuracy testing
            System.Diagnostics.Debug.WriteLine("")
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] ====================================")
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] AbbotPersonnelID = " & If(_abbotPersonnelID.HasValue, _abbotPersonnelID.Value.ToString(), "NULL"))
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] AbbotName = " & _abbotName)
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] WaiyawatPersonnelID = " & If(_waiyawatPersonnelID.HasValue, _waiyawatPersonnelID.Value.ToString(), "NULL"))
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] WaiyawatName = " & _waiyawatName)
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Page Number = " & (_pageIndex + 1))
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Current Y Position = " & _pageY)
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Remaining Height = " & (_pageBottom - _pageY))
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] Signature Reserved Height = " & FinalSignatureBlockHeight)
            System.Diagnostics.Debug.WriteLine("[REPORT DEBUG] ====================================")
            System.Diagnostics.Debug.WriteLine("")

            Dim totalRows = Math.Max(_incomeRows.Count, _expenseRows.Count)
            Dim isLastPage = (_rowIndex >= totalRows)

            ' Only draw header if we still have transactions to print
            If _rowIndex < totalRows Then
                DrawHeader(g, pageW)
            End If

            Dim colW1 = CInt(usableW * 0.16)
            Dim colW2 = CInt(usableW * 0.1)
            Dim colW3 = usableW - colW1 - colW2 - CInt(usableW * 0.24)
            Dim colW4 = CInt(usableW * 0.24)
            Dim rowH = 28

            DrawTableHeader(g, rowH, colW1, colW2, colW3, colW4)

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
                    g.DrawRectangle(_blackPen, _leftX, y, usableW, rowH)
                    g.DrawRectangle(_blackPen, _leftX, y, colW1, rowH)
                    g.DrawRectangle(_blackPen, _leftX + colW1, y, colW2, rowH)
                    g.DrawRectangle(_blackPen, _leftX + colW1 + colW2, y, colW3, rowH)
                    g.DrawRectangle(_blackPen, _leftX + colW1 + colW2 + colW3, y, colW4, rowH)
                    g.DrawLine(_blackPen, _leftX + colW1 + colW2 + colW3, y, _leftX + colW1 + colW2 + colW3, y + rowH)

                    Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                    Dim fmtL As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center}
                    Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}

                    g.DrawString(ToBuddhistDateShort(row.TranDate), _rowFont, Brushes.Black, New RectangleF(_leftX, y, colW1, rowH), fmt)
                    g.DrawString(ToThaiNumerals(row.DayCode.ToString()), _rowFont, Brushes.Black, New RectangleF(_leftX + colW1, y, colW2, rowH), fmt)
                    g.DrawString(Truncate(g, row.Description, _rowFont, colW3 - 6), _rowFont, Brushes.Black,
                                 New RectangleF(_leftX + colW1 + colW2 + 3, y, colW3 - 6, rowH), fmtL)
                    Dim redColor As Color = If(row.IsCarryForward, Color.Red, Color.Black)
                    g.DrawString(FormatThaiAmount(row.Amount), _rowFont, If(row.IsCarryForward, Brushes.Red, Brushes.Black),
                                 New RectangleF(_leftX + colW1 + colW2 + colW3, y, colW4 - 4, rowH), fmtR)
                End If

                If hasRight Then
                    Dim row = _expenseRows(_rowIndex)
                    g.DrawRectangle(_blackPen, _rightX, y, usableW, rowH)
                    g.DrawRectangle(_blackPen, _rightX, y, colW1, rowH)
                    g.DrawRectangle(_blackPen, _rightX + colW1, y, colW2, rowH)
                    g.DrawRectangle(_blackPen, _rightX + colW1 + colW2, y, colW3, rowH)
                    g.DrawRectangle(_blackPen, _rightX + colW1 + colW2 + colW3, y, colW4, rowH)
                    g.DrawLine(_blackPen, _rightX + colW1 + colW2 + colW3, y, _rightX + colW1 + colW2 + colW3, y + rowH)

                    Dim fmt As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                    Dim fmtL As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center}
                    Dim fmtR As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}

                    g.DrawString(ToBuddhistDateShort(row.TranDate), _rowFont, Brushes.Black, New RectangleF(_rightX, y, colW1, rowH), fmt)
                    g.DrawString(ToThaiNumerals(row.DayCode.ToString()), _rowFont, Brushes.Black, New RectangleF(_rightX + colW1, y, colW2, rowH), fmt)
                    g.DrawString(Truncate(g, row.Description, _rowFont, colW3 - 6), _rowFont, Brushes.Black,
                                 New RectangleF(_rightX + colW1 + colW2 + 3, y, colW3 - 6, rowH), fmtL)
                    g.DrawString(FormatThaiAmount(row.Amount), _rowFont, Brushes.Black,
                                 New RectangleF(_rightX + colW1 + colW2 + colW3, y, colW4 - 4, rowH), fmtR)
                Else
                    g.DrawRectangle(_blackPen, _rightX, y, usableW, rowH)
                    g.DrawRectangle(_blackPen, _rightX, y, colW1, rowH)
                    g.DrawRectangle(_blackPen, _rightX + colW1, y, colW2, rowH)
                    g.DrawRectangle(_blackPen, _rightX + colW1 + colW2, y, colW3, rowH)
                    g.DrawRectangle(_blackPen, _rightX + colW1 + colW2 + colW3, y, colW4, rowH)
                    g.DrawLine(_blackPen, _rightX + colW1 + colW2 + colW3, y, _rightX + colW1 + colW2 + colW3, y + rowH)
                End If

                _rowIndex += 1
                _pageY += rowH
            End While

            ' All rows printed - now check if this is the last page with transactions
            If isLastPage Then
                ' Calculate required height for final content (summary + signatures)
                Dim requiredForFinalContent = CalculateFinalContentHeight(rowH)
                Dim availableSpace = _pageBottom - _pageY

                ' Check if we have enough space for final content
                If availableSpace < requiredForFinalContent Then
                    ' Not enough space - create a new page for signatures only
                    e.HasMorePages = True
                    _pageIndex += 1
                    Return
                End If

                ' We have enough space - fill remaining with empty rows, then draw summaries and signatures
                While _pageY < _pageBottom - requiredForFinalContent
                    g.DrawRectangle(_blackPen, _leftX, _pageY, usableW, rowH)
                    g.DrawRectangle(_blackPen, _leftX, _pageY, colW1, rowH)
                    g.DrawRectangle(_blackPen, _leftX + colW1, _pageY, colW2, rowH)
                    g.DrawRectangle(_blackPen, _leftX + colW1 + colW2, _pageY, colW3, rowH)
                    g.DrawRectangle(_blackPen, _leftX + colW1 + colW2 + colW3, _pageY, colW4, rowH)
                    g.DrawLine(_blackPen, _leftX + colW1 + colW2 + colW3, _pageY, _leftX + colW1 + colW2 + colW3, _pageY + rowH)
                    g.DrawRectangle(_blackPen, _rightX, _pageY, usableW, rowH)
                    g.DrawRectangle(_blackPen, _rightX, _pageY, colW1, rowH)
                    g.DrawRectangle(_blackPen, _rightX + colW1, _pageY, colW2, rowH)
                    g.DrawRectangle(_blackPen, _rightX + colW1 + colW2, _pageY, colW3, rowH)
                    g.DrawRectangle(_blackPen, _rightX + colW1 + colW2 + colW3, _pageY, colW4, rowH)
                    g.DrawLine(_blackPen, _rightX + colW1 + colW2 + colW3, _pageY, _rightX + colW1 + colW2 + colW3, _pageY + rowH)
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
        ''' </summary>
        Private Function CalculateFinalContentHeight(rowH As Integer) As Integer
            ' Summary rows (2 main rows + gap)
            Dim summaryHeight = (FinalSummaryRows * rowH) + 12
            ' Signature block height
            Dim signatureHeight = FinalSignatureBlockHeight
            ' Gap before footer
            Dim footerGap = FinalFooterGapHeight
            ' Minimum top margin
            Dim topGap = 12

            Return summaryHeight + signatureHeight + footerGap + topGap
        End Function

        Private Sub DrawEmptyRows(g As Graphics, printed As Integer, max As Integer, rowH As Integer,
                                  c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer, usableW As Integer)
            For i = printed To max - 1
                g.DrawRectangle(_blackPen, _leftX, _pageY, usableW, rowH)
                g.DrawRectangle(_blackPen, _leftX, _pageY, c1, rowH)
                g.DrawRectangle(_blackPen, _leftX + c1, _pageY, c2, rowH)
                g.DrawRectangle(_blackPen, _leftX + c1 + c2, _pageY, c3, rowH)
                g.DrawRectangle(_blackPen, _leftX + c1 + c2 + c3, _pageY, c4, rowH)
                g.DrawLine(_blackPen, _leftX + c1 + c2 + c3, _pageY, _leftX + c1 + c2 + c3, _pageY + rowH)
                g.DrawRectangle(_blackPen, _rightX, _pageY, usableW, rowH)
                g.DrawRectangle(_blackPen, _rightX, _pageY, c1, rowH)
                g.DrawRectangle(_blackPen, _rightX + c1, _pageY, c2, rowH)
                g.DrawRectangle(_blackPen, _rightX + c1 + c2, _pageY, c3, rowH)
                g.DrawRectangle(_blackPen, _rightX + c1 + c2 + c3, _pageY, c4, rowH)
                g.DrawLine(_blackPen, _rightX + c1 + c2 + c3, _pageY, _rightX + c1 + c2 + c3, _pageY + rowH)
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

            g.DrawString("รวมทั้งสิ้น", _boldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawRectangle(_blackPen, _leftX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(runningIncome), _boldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            g.DrawString("รวมทั้งสิ้น", _boldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawRectangle(_blackPen, _rightX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(runningExpense), _boldFont, Brushes.Black,
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
            g.DrawRectangle(_blackPen, _leftX, y, usableW, rowH)
            g.DrawLine(_blackPen, _leftX + c1 + c2, y, _leftX + c1 + c2, y + rowH)
            g.DrawLine(_blackPen, _leftX + c1 + c2 + c3, y, _leftX + c1 + c2 + c3, y + rowH)
            g.DrawString("รวมรายรับ", _bigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(_totalIncome), _bigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' LEFT Grand total row (ยอดยกมา + รายรับ)
            y = baseY + rowH
            g.DrawString("รวมทั้งสิ้น", _bigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawRectangle(_blackPen, _leftX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(_reportGrandTotal), _bigBoldFont, Brushes.Black,
                         New RectangleF(_leftX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' RIGHT Expense Total
            y = baseY
            g.DrawRectangle(_blackPen, _rightX, y, usableW, rowH)
            g.DrawLine(_blackPen, _rightX + c1 + c2, y, _rightX + c1 + c2, y + rowH)
            g.DrawLine(_blackPen, _rightX + c1 + c2 + c3, y, _rightX + c1 + c2 + c3, y + rowH)
            g.DrawString("รวมรายจ่าย", _bigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawString(FormatThaiAmount(_totalExpense), _bigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' RIGHT Carry forward row
            y = baseY + rowH
            Dim carryLabelX = _rightX + 6
            Dim carryLabelWidth = c1 + c2 + c3 - 12
            Using fittedCarryFont = CreateFittedBoldFont(g, balanceLabel, _bigBoldFont, carryLabelWidth, 9.5F)
                g.DrawString(balanceLabel, fittedCarryFont, Brushes.Red,
                             New RectangleF(carryLabelX, y, carryLabelWidth, rowH), fmtL)
            End Using
            g.DrawRectangle(_blackPen, _rightX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(_balance), _bigBoldFont, Brushes.Red,
                         New RectangleF(_rightX + c1 + c2 + c3, y, c4 - 4, rowH), fmtR)

            ' RIGHT Grand total row (ต้องเท่ากับฝั่งรายรับรวมยอดยกมา)
            y = baseY + (rowH * 2)
            g.DrawString("รวมทั้งสิ้น", _bigBoldFont, Brushes.Black,
                         New RectangleF(_rightX + c1 + c2, y, c3, rowH), fmtC)
            g.DrawRectangle(_blackPen, _rightX + c1 + c2 + c3, y, c4, rowH)
            g.DrawString(FormatThaiAmount(_reportGrandTotal), _bigBoldFont, Brushes.Black,
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
            Dim size = baseFont.Size
            Dim bestFit As Font = baseFont

            While size >= minSize
                Dim trial As New Font(baseFont.FontFamily, size, FontStyle.Bold)
                Dim textSize = g.MeasureString(text, trial, Integer.MaxValue, New StringFormat(StringFormatFlags.NoWrap))
                If textSize.Width <= maxWidth Then
                    If Not Object.ReferenceEquals(bestFit, baseFont) Then bestFit.Dispose()
                    Return trial
                End If

                If Not Object.ReferenceEquals(bestFit, baseFont) Then bestFit.Dispose()
                bestFit = trial
                size -= 0.5F
            End While

            Return bestFit
        End Function

        Private Sub DrawSignatures(g As Graphics, rowH As Integer, c1 As Integer, c2 As Integer, c3 As Integer, c4 As Integer, usableW As Integer)
            Dim minimumTop = _pageY + 12

            ' === SHARED LAYOUT CALCULATION ===
            ' Signature block dimensions - centered in each half of page
            ' Each half (leftSection/rightSection) has usableW width
            ' Leave 40px margin on each side for balanced appearance
            Const marginEachSide As Integer = 40
            Dim blockWidth As Integer = Math.Max(usableW - (marginEachSide * 2), 180)
            
            ' Center each block in its respective half
            Dim leftBlockX As Integer = _leftX + CInt((usableW - blockWidth) / 2)
            Dim rightBlockX As Integer = _rightX + CInt((usableW - blockWidth) / 2)

            ' === SHARED SPACING CONSTANTS ===
            Const labelHeight As Integer = 26          ' Height for label text
            Const labelToSignLine As Integer = 50     ' 50px gap above dotted line (signing space)
            Const signLineToName As Integer = 1       ' 1px gap - line very close to name
            Const nameHeight As Integer = 26           ' Height for name text
            Const nameToPosition As Integer = 8       ' Gap between name and position
            Const positionHeight As Integer = 26       ' Height for position text

            ' Calculate total height for signature section
            ' Section: label(26) + gap(50) + signLineToName(1) + name(26) + gap(8) + position(26)
            Dim totalSignatureHeight = labelHeight + labelToSignLine + signLineToName + nameHeight + nameToPosition + positionHeight

            ' Position the signature area - both blocks share same bottom Y
            ' Use smaller bottom margin to move signature block upward
            Dim blockBottom = _pageBottom - 4
            If blockBottom - totalSignatureHeight < minimumTop Then
                blockBottom = minimumTop + totalSignatureHeight + 8
            End If

            ' === SHARED Y COORDINATES ===
            ' Both blocks use the same Y positions (signatureLabelY marks the TOP of each block)
            Dim signatureLabelY As Integer = blockBottom - totalSignatureHeight
            Dim signatureLineY As Integer = signatureLabelY + labelHeight + labelToSignLine
            Dim signatureNameY As Integer = signatureLineY + signLineToName
            Dim signaturePositionY As Integer = signatureNameY + nameHeight + nameToPosition

            ' Signature line: fixed 280px width (250-300px range), centered within block
            Const signLineWidth As Integer = 280
            Dim signLineLeftX As Integer = leftBlockX + CInt((blockWidth - signLineWidth) / 2)
            Dim signLineRightX As Integer = rightBlockX + CInt((blockWidth - signLineWidth) / 2)

            ' Title format
            Dim fmtTitle As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center,
                .FormatFlags = StringFormatFlags.NoWrap
            }

            ' === DRAW LEFT BLOCK (ABBOT) ===
            ' Title: ตรวจถูกต้องแล้ว
            g.DrawString("ตรวจถูกต้องแล้ว", _boldFont, Brushes.Black,
                        New RectangleF(leftBlockX, signatureLabelY, blockWidth, labelHeight), fmtTitle)

            ' Signature dotted line (centered, 80% width) - drawn at signatureLineY
            DrawDottedSignatureLine(g, signLineLeftX, signatureLineY, signLineWidth)

            ' Abbot name
            Dim abbotDisplay As String = If(String.IsNullOrWhiteSpace(_abbotName), ".....................................", "(" & _abbotName & ")")
            Using nameFont As Font = CreateFittedBoldFont(g, abbotDisplay, New Font("Tahoma", 12.0!, FontStyle.Bold), blockWidth - 8, 10.0!)
                g.DrawString(abbotDisplay, nameFont, Brushes.Black,
                           New RectangleF(leftBlockX, signatureNameY, blockWidth, nameHeight), fmtTitle)
            End Using

            ' Abbot position
            g.DrawString("เจ้าอาวาส", _rowFont, Brushes.Black,
                        New RectangleF(leftBlockX, signaturePositionY, blockWidth, positionHeight), fmtTitle)

            ' === DRAW RIGHT BLOCK (WAIYAWAT) ===
            ' Title: ผู้จัดทำบัญชี
            g.DrawString("ผู้จัดทำบัญชี", _boldFont, Brushes.Black,
                        New RectangleF(rightBlockX, signatureLabelY, blockWidth, labelHeight), fmtTitle)

            ' Signature dotted line (centered, 80% width) - drawn at signatureLineY
            DrawDottedSignatureLine(g, signLineRightX, signatureLineY, signLineWidth)

            ' Waiyawat name
            Dim waiyawatDisplay As String = If(String.IsNullOrWhiteSpace(_waiyawatName), ".....................................", "(" & _waiyawatName & ")")
            Using nameFont As Font = CreateFittedBoldFont(g, waiyawatDisplay, New Font("Tahoma", 12.0!, FontStyle.Bold), blockWidth - 8, 10.0!)
                g.DrawString(waiyawatDisplay, nameFont, Brushes.Black,
                           New RectangleF(rightBlockX, signatureNameY, blockWidth, nameHeight), fmtTitle)
            End Using

            ' Waiyawat position
            g.DrawString("ไวยาวัจกร", _rowFont, Brushes.Black,
                        New RectangleF(rightBlockX, signaturePositionY, blockWidth, positionHeight), fmtTitle)

            _pageY = signaturePositionY + positionHeight
        End Sub

        ''' <summary>
        ''' Draws a dotted signature line (not a solid line)
        ''' </summary>
        Private Sub DrawDottedSignatureLine(g As Graphics, x As Integer, y As Integer, width As Integer)
            Const dotSpacing As Integer = 8     ' Space between dots (increased)
            Const dotRadius As Integer = 1.0F   ' Smaller dot size
            Dim dotCount As Integer = CInt(width / dotSpacing)
            Dim totalDotsWidth As Integer = dotCount * dotSpacing
            Dim startX As Integer = x + CInt((width - totalDotsWidth) / 2) + CInt(dotSpacing / 2)

            Using dotBrush As New SolidBrush(Color.Black)
                For i As Integer = 0 To dotCount - 1
                    Dim dotX As Integer = startX + (i * dotSpacing)
                    g.FillEllipse(dotBrush, dotX - dotRadius, y - dotRadius, dotRadius * 2, dotRadius * 2)
                Next
            End Using
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
            g.DrawRectangle(_pen2, _leftX, _pageY, c1 + c2, rowH * 2)

            g.DrawString("เดือน", _headerFont, Brushes.Black, New RectangleF(_leftX, _pageY, c1, rowH), fmtC)
            g.DrawLine(_blackPen, _leftX, _pageY + rowH, _leftX + c1 + c2, _pageY + rowH)
            g.DrawString("วันที่", _headerFont, Brushes.Black, New RectangleF(_leftX + c1, _pageY + rowH, c2, rowH), fmtC)
            g.DrawLine(_blackPen, _leftX + c1, _pageY, _leftX + c1, _pageY + rowH * 2)
            g.DrawString("เดือน", _headerFont, Brushes.Black, New RectangleF(_leftX, _pageY, c1, rowH), fmtC)

            ' รายรับ header spans 2 rows
            g.DrawRectangle(_pen2, _leftX + c1 + c2, _pageY, c3, rowH * 2)
            g.DrawString("รายรับ", _headerFont, Brushes.Black, New RectangleF(_leftX + c1 + c2, _pageY, c3, rowH * 2), fmtC)

            ' รวมเงิน outer spans 2 rows with บาท / ส.ต. inner
            g.DrawRectangle(_pen2, _leftX + c1 + c2 + c3, _pageY, c4, rowH * 2)
            g.DrawString("รวมเงิน", _headerFont, Brushes.Black, New RectangleF(_leftX + c1 + c2 + c3, _pageY, c4, rowH), fmtC)
            g.DrawLine(_blackPen, _leftX + c1 + c2 + c3, _pageY + rowH, _leftX + c1 + c2 + c3 + c4, _pageY + rowH)
            ' split บาท/ส.ต. but since sample shows combined number, draw combined label
            Dim bahtW = CInt(c4 * 0.8)
            Dim satW = c4 - bahtW
            g.DrawLine(_blackPen, _leftX + c1 + c2 + c3 + bahtW, _pageY + rowH, _leftX + c1 + c2 + c3 + c4, _pageY + rowH)
            g.DrawLine(_blackPen, _leftX + c1 + c2 + c3 + bahtW, _pageY + rowH, _leftX + c1 + c2 + c3 + bahtW, _pageY + rowH * 2)
            g.DrawString("บาท", _headerFont, Brushes.Black, New RectangleF(_leftX + c1 + c2 + c3, _pageY + rowH, bahtW, rowH), fmtC)
            g.DrawString("ส.ต.", _headerFont, Brushes.Black, New RectangleF(_leftX + c1 + c2 + c3 + bahtW, _pageY + rowH, satW, rowH), fmtC)

            g.FillRectangle(Brushes.White, _rightX, _pageY, c1 + c2, rowH * 2)
            g.DrawRectangle(_pen2, _rightX, _pageY, c1 + c2, rowH * 2)
            g.DrawLine(_blackPen, _rightX + c1, _pageY, _rightX + c1, _pageY + rowH * 2)
            g.DrawString("เดือน", _headerFont, Brushes.Black, New RectangleF(_rightX, _pageY, c1, rowH), fmtC)
            g.DrawLine(_blackPen, _rightX, _pageY + rowH, _rightX + c1 + c2, _pageY + rowH)
            g.DrawString("วันที่", _headerFont, Brushes.Black, New RectangleF(_rightX + c1, _pageY + rowH, c2, rowH), fmtC)

            g.DrawRectangle(_pen2, _rightX + c1 + c2, _pageY, c3, rowH * 2)
            g.DrawString("รายจ่าย", _headerFont, Brushes.Black, New RectangleF(_rightX + c1 + c2, _pageY, c3, rowH * 2), fmtC)

            g.DrawRectangle(_pen2, _rightX + c1 + c2 + c3, _pageY, c4, rowH * 2)
            g.DrawString("รวมเงิน", _headerFont, Brushes.Black, New RectangleF(_rightX + c1 + c2 + c3, _pageY, c4, rowH), fmtC)
            g.DrawLine(_blackPen, _rightX + c1 + c2 + c3, _pageY + rowH, _rightX + c1 + c2 + c3 + c4, _pageY + rowH)
            g.DrawLine(_blackPen, _rightX + c1 + c2 + c3 + bahtW, _pageY + rowH, _rightX + c1 + c2 + c3 + c4, _pageY + rowH)
            g.DrawLine(_blackPen, _rightX + c1 + c2 + c3 + bahtW, _pageY + rowH, _rightX + c1 + c2 + c3 + bahtW, _pageY + rowH * 2)
            g.DrawString("บาท", _headerFont, Brushes.Black, New RectangleF(_rightX + c1 + c2 + c3, _pageY + rowH, bahtW, rowH), fmtC)
            g.DrawString("ส.ต.", _headerFont, Brushes.Black, New RectangleF(_rightX + c1 + c2 + c3 + bahtW, _pageY + rowH, satW, rowH), fmtC)

            _pageY += rowH * 2
        End Sub

        Private Sub DrawHeader(g As Graphics, pageW As Integer)
            Dim fmtC As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
            Dim y = _pageY
            Dim reportTitle = If(_mode = ReportModes.Summary, "สรุปบัญชีรายรับ - รายจ่าย (แบบย่อ)", "สรุปบัญชีรายรับ - รายจ่าย (แบบละเอียด)")
            g.DrawString(reportTitle, _titleFont, Brushes.Black,
                         New RectangleF(_startX, y, pageW - 2 * _startX, 36), fmtC)
            y += 36
            If String.IsNullOrEmpty(_templeName) Then _templeName = "วัดแหลมยาง"
            If String.IsNullOrEmpty(_templeAddress) Then _templeAddress = "ต.ป่ามะคาบ อ.เมืองพิจิตร จ.พิจิตร"
            g.DrawString(ToThaiNumerals(_templeName & "  " & _templeAddress), _subTitleFont, Brushes.Black,
                         New RectangleF(_startX, y, pageW - 2 * _startX, 30), fmtC)
            y += 30
            Dim yearB = (_fromDate.Year + 543)
            Dim label = "ประจำปี พ.ศ. " & ToThaiNumerals(yearB.ToString()) &
                        "    ตั้งแต่วันที่ ( " & ToBuddhistFull(_fromDate) & " – " & ToBuddhistFull(_toDate) & " )"
            g.DrawString(label, _subTitleFont, Brushes.Black,
                         New RectangleF(_startX, y, pageW - 2 * _startX, 30), fmtC)
            y += 36
            _pageY = y
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
