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
    ''' <summary>
    ''' Base Report class - abstract base for all reports
    ''' Provides common page setup, pagination, and drawing functionality
    ''' Individual reports must implement: LoadData(), DrawRows(), DrawSummary()
    ''' </summary>
    Public MustInherit Class BaseReport
        Inherits PrintDocument

        ' Common layout and theme
        Protected _layout As LayoutConfig
        Protected _theme As ReportTheme

        ' Page state
        Protected _startX As Integer
        Protected _pageW As Integer
        Protected _pageY As Integer
        Protected _pageBottom As Integer
        Protected _leftX As Integer
        Protected _rightX As Integer
        Protected _leftSectionWidth As Integer
        Protected _rightSectionWidth As Integer
        Protected _pageIndex As Integer = 0
        Protected _rowIndex As Integer = 0
        Protected _maxRowsPerPage As Integer

        ' Report info (set by derived classes)
        Protected _reportTitle As String = ""
        Protected _templeName As String = ""
        Protected _templeAddress As String = ""
        Protected _fromDate As Date
        Protected _toDate As Date

        ' Constants
        Protected Const SectionGap As Integer = 12

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        Protected Sub New()
            MyBase.New()
            _theme = ReportTheme.CreateDefault()
        End Sub

        ''' <summary>
        ''' Constructor with document name
        ''' </summary>
        Protected Sub New(documentName As String)
            MyBase.New()
            Me.DocumentName = documentName
            _theme = ReportTheme.CreateDefault()
        End Sub

        ''' <summary>
        ''' Configure A4 Landscape page settings
        ''' </summary>
        Protected Sub ConfigurePageSettings()
            Dim a4Paper = CreateA4Paper(Me.PrinterSettings)
            Dim pageMargins As New Margins(32, 32, 28, 28)

            Me.DefaultPageSettings.PaperSize = a4Paper
            Me.DefaultPageSettings.Margins = pageMargins
            Me.DefaultPageSettings.Landscape = True

            Me.PrinterSettings.DefaultPageSettings.PaperSize = a4Paper
            Me.PrinterSettings.DefaultPageSettings.Margins = pageMargins
            Me.PrinterSettings.DefaultPageSettings.Landscape = True
        End Sub

        ''' <summary>
        ''' Create A4 paper size
        ''' </summary>
        Protected Shared Function CreateA4Paper(Optional printerSettings As PrinterSettings = Nothing) As PaperSize
            If printerSettings IsNot Nothing Then
                For Each paper As PaperSize In printerSettings.PaperSizes
                    If paper.Kind = PaperKind.A4 OrElse paper.PaperName.IndexOf("A4", StringComparison.OrdinalIgnoreCase) >= 0 Then
                        Return paper
                    End If
                Next
            End If
            Return New PaperSize("A4", 827, 1169)
        End Function

        ''' <summary>
        ''' Initialize fonts from theme
        ''' </summary>
        Protected Sub InitFonts()
            ' Fonts are already created in ReportTheme.CreateDefault()
        End Sub

        ''' <summary>
        ''' Setup layout for current page
        ''' </summary>
        Protected Sub SetupLayout(e As PrintPageEventArgs)
            _layout = LayoutConfig.CreateA4Landscape(32, 32, 28, 28)

            _pageW = e.PageBounds.Width
            _startX = e.MarginBounds.Left
            _pageY = e.MarginBounds.Top
            _pageBottom = e.MarginBounds.Bottom
            _leftX = _startX
            _rightSectionWidth = CInt((e.MarginBounds.Width) / 2) - 6
            _leftSectionWidth = _rightSectionWidth
            _rightX = _startX + _leftSectionWidth + SectionGap
        End Sub

        ''' <summary>
        ''' Draw page border
        ''' </summary>
        Protected Sub DrawBorder(g As Graphics)
            Dim width = _layout.PageWidth - _layout.MarginLeft - _layout.MarginRight
            Dim height = _layout.PageHeight - _layout.MarginTop - _layout.MarginBottom
            ReportEngine.DrawBorder(g, _startX, _layout.MarginTop, width, height, _theme.BlackPen)
        End Sub

        ''' <summary>
        ''' Draw page number at bottom center
        ''' </summary>
        Protected Sub DrawPageNumber(g As Graphics, totalPages As Integer)
            ReportEngine.DrawPageNumber(g, _pageIndex + 1, totalPages, _pageBottom, _layout.PageWidth, _theme.RowFont)
        End Sub

        ''' <summary>
        ''' Calculate final content height (summary rows + signature block)
        ''' </summary>
        Protected Overridable Function CalculateFinalContentHeight(rowH As Integer) As Integer
            Return 235 ' Override in derived class if needed
        End Function

        ''' <summary>
        ''' Must be implemented by derived classes to load report data
        ''' </summary>
        Protected MustOverride Sub LoadData()

        ''' <summary>
        ''' Must be implemented by derived classes to draw data rows
        ''' Returns number of rows drawn
        ''' </summary>
        Protected MustOverride Function DrawRows(g As Graphics, rowH As Integer, totalRows As Integer) As Integer

        ''' <summary>
        ''' Must be implemented by derived classes to draw summary section
        ''' </summary>
        Protected MustOverride Sub DrawSummary(g As Graphics, rowH As Integer)

        ''' <summary>
        ''' Draw report header with title, temple info, and date range
        ''' </summary>
        Protected Sub DrawReportHeader(g As Graphics, pageW As Integer, Optional reportType As String = "")
            _pageY = ReportEngine.DrawHeader(g, _reportTitle, _templeName, _templeAddress,
                                           _fromDate, _toDate, _theme.TitleFont, _theme.SubTitleFont,
                                           _startX, _pageY, pageW, _pageIndex + 1, 0, reportType)
        End Sub

        ''' <summary>
        ''' Draw signature block
        ''' </summary>
        Protected Sub DrawSignatureBlock(g As Graphics, leftTitle As String, leftName As String, leftPosition As String,
                                       rightTitle As String, rightName As String, rightPosition As String)
            _pageY = ReportEngine.DrawSignatureBlock(g,
                leftTitle, leftName, leftPosition,
                rightTitle, rightName, rightPosition,
                _leftX, _rightX, _leftSectionWidth, _pageBottom, _pageY,
                _theme.BoldFont, _theme.RowFont)
        End Sub

        ''' <summary>
        ''' Common print page handler - calls abstract methods
        ''' </summary>
        Protected Overrides Sub OnPrintPage(e As PrintPageEventArgs)
            MyBase.OnPrintPage(e)
            Dim g As Graphics = e.Graphics
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

            ' Setup layout
            SetupLayout(e)

            ' Draw border
            DrawBorder(g)

            ' Draw header
            DrawReportHeader(g, _pageW)

            ' Calculate total rows (derived class provides data)
            Dim totalRows = GetTotalRowCount()

            ' Only draw header row if we have transactions
            If _rowIndex < totalRows Then
                DrawTableHeader(g, _theme.RowHeight)
            End If

            ' Draw data rows until we run out of space or rows
            While _rowIndex < totalRows
                Dim rowH = _theme.RowHeight
                If _pageY + rowH > _pageBottom Then
                    ' Cannot fit this row - need new page
                    e.HasMorePages = True
                    _pageIndex += 1
                    Return
                End If
                ' Draw row
                Dim drawn = DrawRows(g, rowH, totalRows)
                If drawn = 0 Then Exit While
            End While

            ' Check if all rows are printed
            Dim isLastPage = (_rowIndex >= totalRows)

            If isLastPage Then
                ' All rows printed - check if we have room for final content
                Dim requiredForFinalContent = CalculateFinalContentHeight(_theme.RowHeight)
                Dim availableSpace = _pageBottom - _pageY

                If availableSpace >= requiredForFinalContent Then
                    ' Fill blank rows
                    While _pageY < _pageBottom - requiredForFinalContent
                        _pageY += _theme.RowHeight
                    End While

                    ' Draw summary
                    DrawSummary(g, _theme.RowHeight)

                    ' Draw signature block
                    DrawSignatureBlock(g, "ตรวจถูกต้องแล้ว", GetLeftSignerName(), GetLeftSignerPosition(),
                                      "ผู้จัดทำบัญชี", GetRightSignerName(), GetRightSignerPosition())

                    e.HasMorePages = False
                Else
                    ' Not enough room - signature on next page
                    e.HasMorePages = True
                    _pageIndex += 1
                End If
            Else
                ' Not last page - draw mid-summary
                DrawMidSummary(g, _theme.RowHeight)
                e.HasMorePages = True
                _pageIndex += 1
            End If
        End Sub

        ''' <summary>
        ''' Get total row count - override in derived class
        ''' </summary>
        Protected Overridable Function GetTotalRowCount() As Integer
            Return 0
        End Function

        ''' <summary>
        ''' Draw table header - override in derived class for specific columns
        ''' </summary>
        Protected Overridable Sub DrawTableHeader(g As Graphics, rowH As Integer)
            ' Override in derived class
        End Sub

        ''' <summary>
        ''' Draw mid-summary (for non-final pages) - override in derived class
        ''' </summary>
        Protected Overridable Sub DrawMidSummary(g As Graphics, rowH As Integer)
            ' Override in derived class
        End Sub

        ''' <summary>
        ''' Get left signer name - override in derived class
        ''' </summary>
        Protected Overridable Function GetLeftSignerName() As String
            Return ""
        End Function

        ''' <summary>
        ''' Get left signer position - override in derived class
        ''' </summary>
        Protected Overridable Function GetLeftSignerPosition() As String
            Return ""
        End Function

        ''' <summary>
        ''' Get right signer name - override in derived class
        ''' </summary>
        Protected Overridable Function GetRightSignerName() As String
            Return ""
        End Function

        ''' <summary>
        ''' Get right signer position - override in derived class
        ''' </summary>
        Protected Overridable Function GetRightSignerPosition() As String
            Return ""
        End Function

    End Class
End Namespace
