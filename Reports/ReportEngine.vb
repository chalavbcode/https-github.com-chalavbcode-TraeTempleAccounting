Option Strict Off
Option Explicit On

Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Printing
Imports System.Drawing.Text
Imports System.Globalization

Namespace TempleAccounting
    ''' <summary>
    ''' Report Layout Configuration - stores all layout parameters for a report page
    ''' Replaces hardcoded coordinates with a reusable configuration object
    ''' </summary>
    Public Class LayoutConfig
        ' Page dimensions
        Public Property PageWidth As Integer
        Public Property PageHeight As Integer

        ' Margins
        Public Property MarginLeft As Integer
        Public Property MarginRight As Integer
        Public Property MarginTop As Integer
        Public Property MarginBottom As Integer

        ' Calculated positions
        Public Property HeaderTop As Integer      ' Where header starts (MarginTop)
        Public Property TableTop As Integer       ' Where table starts (after header)
        Public Property FooterTop As Integer      ' Where footer/summary starts
        Public Property SignatureTop As Integer    ' Where signature block starts

        ' Dimensions
        Public Property RowHeight As Integer      ' Height of each data row
        Public Property TableWidth As Integer     ' Usable width for table
        Public Property LeftSectionWidth As Integer
        Public Property RightSectionWidth As Integer
        Public Property SectionGap As Integer     ' Gap between left and right sections

        ' Column widths (percentages applied to TableWidth)
        Public Property Col1Percent As Single    ' Date column
        Public Property Col2Percent As Single    ' Day column
        Public Property Col3Percent As Single    ' Description column
        Public Property Col4Percent As Single    ' Amount column

        ' Start positions
        Public Property StartX As Integer        ' Left edge of content
        Public Property LeftX As Integer          ' Left section X position
        Public Property RightX As Integer        ' Right section X position

        ' Page bounds
        Public Property PageBottom As Integer     ' Bottom printable Y position

        ''' <summary>
        ''' Create a default A4 Landscape layout configuration
        ''' </summary>
        Public Shared Function CreateA4Landscape(Optional leftMargin As Integer = 32, Optional rightMargin As Integer = 32, Optional topMargin As Integer = 28, Optional bottomMargin As Integer = 28) As LayoutConfig
            ' A4 at 96 DPI: 827 x 1169 pixels (landscape swaps these)
            Dim pageW = 1169
            Dim pageH = 827

            Dim cfg As New LayoutConfig()
            cfg.PageWidth = pageW
            cfg.PageHeight = pageH
            cfg.MarginLeft = leftMargin
            cfg.MarginRight = rightMargin
            cfg.MarginTop = topMargin
            cfg.MarginBottom = bottomMargin

            ' Calculated positions
            cfg.HeaderTop = topMargin
            cfg.StartX = leftMargin
            cfg.PageBottom = pageH - bottomMargin
            cfg.RowHeight = 28

            ' Column percentages
            cfg.Col1Percent = 0.16F    ' Date
            cfg.Col2Percent = 0.1F     ' Day
            cfg.Col3Percent = 0.4F     ' Description (calculated as remainder)
            cfg.Col4Percent = 0.24F    ' Amount

            ' Section gap
            cfg.SectionGap = 12

            ' Calculate section widths
            Dim usableW = pageW - leftMargin - rightMargin
            cfg.TableWidth = usableW
            cfg.LeftSectionWidth = CInt(usableW / 2) - CInt(cfg.SectionGap / 2)
            cfg.RightSectionWidth = cfg.LeftSectionWidth
            cfg.LeftX = cfg.StartX
            cfg.RightX = cfg.StartX + cfg.LeftSectionWidth + cfg.SectionGap

            Return cfg
        End Function

        ''' <summary>
        ''' Calculate column widths based on table width
        ''' </summary>
        Public Sub CalculateColumnWidths()
            Dim col4 = CInt(TableWidth * Col4Percent)
            Dim col1 = CInt(TableWidth * Col1Percent)
            Dim col2 = CInt(TableWidth * Col2Percent)
            Dim col3 = TableWidth - col1 - col2 - col4
        End Sub

        ''' <summary>
        ''' Get remaining printable height from a given Y position
        ''' </summary>
        Public Function GetRemainingHeight(currentY As Integer) As Integer
            Return PageBottom - currentY
        End Function

        ''' <summary>
        ''' Check if content fits in remaining space
        ''' </summary>
        Public Function HasSpaceFor(currentY As Integer, requiredHeight As Integer) As Boolean
            Return GetRemainingHeight(currentY) >= requiredHeight
        End Function

    End Class

    ''' <summary>
    ''' Report Theme - encapsulates all visual settings for reports
    ''' Includes fonts, pens, colors, spacing, and drawing styles
    ''' </summary>
    Public Class ReportTheme
        ' Fonts
        Public Property RowFont As Font
        Public Property BoldFont As Font
        Public Property HeaderFont As Font
        Public Property BigBoldFont As Font
        Public Property TitleFont As Font
        Public Property SubTitleFont As Font
        Public Property NameFont As Font

        ' Pens
        Public Property BlackPen As Pen
        Public Property Pen2 As Pen

        ' Colors
        Public Property TextColor As Color
        Public Property RedColor As Color
        Public Property WhiteColor As Color

        ' Spacing
        Public Property RowHeight As Integer
        Public Property HeaderHeight As Integer
        Public Property CellPadding As Integer
        Public Property SectionGap As Integer
        Public Property SmallGap As Integer
        Public Property MediumGap As Integer
        Public Property LargeGap As Integer

        ' Table header
        Public Property TableHeaderHeightMultiplier As Integer

        ' Signature block
        Public Property SignatureLabelHeight As Integer
        Public Property SignatureLineGap As Integer
        Public Property SignatureNameGap As Integer
        Public Property SignaturePositionGap As Integer

        ''' <summary>
        ''' Create a default theme with standard Thai accounting report settings
        ''' </summary>
        Public Shared Function CreateDefault() As ReportTheme
            Dim theme As New ReportTheme()

            ' Fonts - Tahoma family
            theme.RowFont = New Font("Tahoma", 10.0!)
            theme.BoldFont = New Font("Tahoma", 10.0!, FontStyle.Bold)
            theme.HeaderFont = New Font("Tahoma", 10.0!, FontStyle.Bold)
            theme.BigBoldFont = New Font("Tahoma", 14.0!, FontStyle.Bold)
            theme.TitleFont = New Font("Tahoma", 16.0!, FontStyle.Bold)
            theme.SubTitleFont = New Font("Tahoma", 12.0!, FontStyle.Bold)
            theme.NameFont = New Font("Tahoma", 12.0!, FontStyle.Bold)

            ' Pens
            theme.BlackPen = New Pen(Color.Black, 1)
            theme.Pen2 = New Pen(Color.Black, 2)

            ' Colors
            theme.TextColor = Color.Black
            theme.RedColor = Color.Red
            theme.WhiteColor = Color.White

            ' Spacing
            theme.RowHeight = 28
            theme.HeaderHeight = 28
            theme.CellPadding = 4
            theme.SectionGap = 12
            theme.SmallGap = 4
            theme.MediumGap = 8
            theme.LargeGap = 12

            ' Table header
            theme.TableHeaderHeightMultiplier = 2

            ' Signature block
            theme.SignatureLabelHeight = 26
            theme.SignatureLineGap = 50
            theme.SignatureNameGap = 0
            theme.SignaturePositionGap = 8

            Return theme
        End Function

        ''' <summary>
        ''' Create theme for A4 Landscape reports
        ''' </summary>
        Public Shared Function CreateA4Landscape() As ReportTheme
            Return CreateDefault()
        End Function

        ''' <summary>
        ''' Dispose of all managed resources
        ''' </summary>
        Public Sub Dispose()
            If RowFont IsNot Nothing Then RowFont.Dispose()
            If BoldFont IsNot Nothing Then BoldFont.Dispose()
            If HeaderFont IsNot Nothing Then HeaderFont.Dispose()
            If BigBoldFont IsNot Nothing Then BigBoldFont.Dispose()
            If TitleFont IsNot Nothing Then TitleFont.Dispose()
            If SubTitleFont IsNot Nothing Then SubTitleFont.Dispose()
            If NameFont IsNot Nothing Then NameFont.Dispose()
            If BlackPen IsNot Nothing Then BlackPen.Dispose()
            If Pen2 IsNot Nothing Then Pen2.Dispose()
        End Sub
    End Class

    ''' <summary>
    ''' Report Engine - Common drawing utilities for TempleAccounting reports
    ''' Provides reusable methods for borders, page numbers, fonts, and formatting
    ''' </summary>
    Public Module ReportEngine

        ''' <summary>
        ''' Draw a page border around the printable area
        ''' </summary>
        Public Sub DrawBorder(g As Graphics, left As Integer, top As Integer, width As Integer, height As Integer, Optional pen As Pen = Nothing)
            If pen Is Nothing Then
                pen = New Pen(Color.Black, 1)
            End If
            g.DrawRectangle(pen, left, top, width, height)
        End Sub

        ''' <summary>
        ''' Draw page number in format "หน้า X" at bottom center of page
        ''' </summary>
        Public Sub DrawPageNumber(g As Graphics, pageIndex As Integer, totalPages As Integer, pageBottom As Integer, pageWidth As Integer, font As Font, Optional showTotal As Boolean = True)
            Dim pageNumText As String
            If showTotal AndAlso totalPages > 0 Then
                pageNumText = "หน้า " & ThaiNumerals(pageIndex.ToString()) & " / " & ThaiNumerals(totalPages.ToString())
            Else
                pageNumText = "หน้า " & ThaiNumerals(pageIndex.ToString())
            End If

            Dim fmt As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }

            Dim textSize = g.MeasureString(pageNumText, font)
            Dim x = (pageWidth - textSize.Width) / 2
            Dim y = pageBottom + 8

            g.DrawString(pageNumText, font, Brushes.Black, New RectangleF(x, y, textSize.Width, textSize.Height), fmt)
        End Sub

        ''' <summary>
        ''' Draw top border line (double line for official look)
        ''' </summary>
        Public Sub DrawTopBorder(g As Graphics, x As Integer, y As Integer, width As Integer, pen As Pen, Optional gap As Integer = 3)
            g.DrawLine(pen, x, y, x + width, y)
            g.DrawLine(pen, x, y + gap, x + width, y + gap)
        End Sub

        ''' <summary>
        ''' Draw standard report header with title, temple info, and date range
        ''' Returns the Y position after the header (for continuing layout)
        ''' </summary>
        Public Function DrawHeader(g As Graphics, title As String, templeName As String, templeAddress As String,
                                  fromDate As Date, toDate As Date,
                                  titleFont As Font, subtitleFont As Font,
                                  startX As Integer, startY As Integer, pageWidth As Integer) As Integer
            Dim fmtC As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }
            Dim y = startY

            ' Report title
            g.DrawString(title, titleFont, Brushes.Black,
                         New RectangleF(startX, y, pageWidth - 2 * startX, 36), fmtC)
            y += 36

            ' Temple name and address
            Dim templeInfo = templeName
            If Not String.IsNullOrEmpty(templeAddress) Then
                templeInfo &= "  " & templeAddress
            End If
            g.DrawString(ThaiNumerals(templeInfo), subtitleFont, Brushes.Black,
                         New RectangleF(startX, y, pageWidth - 2 * startX, 30), fmtC)
            y += 30

            ' Date range
            Dim yearB = (fromDate.Year + 543)
            Dim dateLabel = "ประจำปี พ.ศ. " & ThaiNumerals(yearB.ToString()) &
                           "    ตั้งแต่วันที่ ( " & ToBuddhistFull(fromDate) & " – " & ToBuddhistFull(toDate) & " )"
            g.DrawString(dateLabel, subtitleFont, Brushes.Black,
                         New RectangleF(startX, y, pageWidth - 2 * startX, 30), fmtC)
            y += 36

            Return y
        End Function

        ''' <summary>
        ''' Convert Arabic digits to Thai numerals
        ''' </summary>
        Public Function ThaiNumerals(s As String) As String
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

        ''' <summary>
        ''' Get Thai abbreviated month name
        ''' </summary>
        Public Function ThaiMonthAbbr(ByVal m As Integer) As String
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

        ''' <summary>
        ''' Get full Thai month name
        ''' </summary>
        Public Function ThaiMonthFull(ByVal m As Integer) As String
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

        ''' <summary>
        ''' Convert Date to Buddhist date short format (ม.ค.-56)
        ''' </summary>
        Public Function ToBuddhistDateShort(ByVal d As Date) As String
            Return ThaiMonthAbbr(d.Month) & "-" & ThaiNumerals(((d.Year + 543) Mod 100).ToString())
        End Function

        ''' <summary>
        ''' Convert Date to Buddhist full date format (1 มกราคม 2566 พ.ศ.)
        ''' </summary>
        Public Function ToBuddhistFull(ByVal d As Date) As String
            Return ThaiNumerals(d.Day.ToString()) & " " & ThaiMonthFull(d.Month) & " พ.ศ. " & ThaiNumerals((d.Year + 543).ToString())
        End Function

        ''' <summary>
        ''' Convert Buddhist year to Gregorian
        ''' </summary>
        Public Function ToBuddhistYearThai(ByVal y As Integer) As Integer
            Return y + 543
        End Function

        ''' <summary>
        ''' Format amount with Thai numerals and comma separator
        ''' </summary>
        Public Function FormatThaiAmount(value As Decimal) As String
            Return ThaiNumerals(value.ToString("#,##0"))
        End Function

        ''' <summary>
        ''' Create a fitted font that scales down text to fit within maxWidth
        ''' </summary>
        Public Function CreateFittedFont(g As Graphics, text As String, baseFont As Font, maxWidth As Integer, minSize As Single, Optional style As FontStyle = FontStyle.Regular) As Font
            Dim size = baseFont.Size
            Dim bestFit As Font = baseFont
            If style = FontStyle.Regular AndAlso baseFont.Style <> FontStyle.Regular Then
                style = baseFont.Style
            End If

            While size >= minSize
                Dim trial As New Font(baseFont.FontFamily, size, style)
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

        ''' <summary>
        ''' Draw a centered string with the specified font
        ''' </summary>
        Public Sub DrawCenteredString(g As Graphics, text As String, font As Font, brush As Brush, x As Single, y As Single, width As Single, height As Single)
            Dim fmt As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center,
                .FormatFlags = StringFormatFlags.NoWrap
            }
            g.DrawString(text, font, brush, New RectangleF(x, y, width, height), fmt)
        End Sub

        ''' <summary>
        ''' Draw a dotted signature line
        ''' </summary>
        Public Sub DrawDottedLine(g As Graphics, x As Integer, y As Integer, width As Integer, Optional dotSpacing As Integer = 8, Optional dotRadius As Single = 1.5F)
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

        ''' <summary>
        ''' Measure string width without wrapping
        ''' </summary>
        Public Function MeasureStringWidth(g As Graphics, text As String, font As Font) As SizeF
            Return g.MeasureString(text, font, Integer.MaxValue, New StringFormat(StringFormatFlags.NoWrap))
        End Function

        ''' <summary>
        ''' Create standard report fonts (Tahoma family)
        ''' </summary>
        Public Function CreateReportFonts(Optional baseSize As Single = 10.0!) As FontFamily
            Return New FontFamily("Tahoma")
        End Function

        ''' <summary>
        ''' Get available paper size by name
        ''' </summary>
        Public Function GetPaperSize(printerSettings As PrinterSettings, paperName As String) As PaperSize
            For Each paper As PaperSize In printerSettings.PaperSizes
                If paper.Kind = PaperKind.A4 OrElse paper.PaperName.IndexOf("A4", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return paper
                End If
            Next
            Return New PaperSize("A4", 827, 1169)
        End Function

        ''' <summary>
        ''' Draw signature section with two aligned blocks (left and right signers)
        ''' Both blocks share identical Y coordinates for perfect alignment
        ''' Returns the Y position after drawing the signature block
        ''' </summary>
        Public Function DrawSignatureBlock(g As Graphics,
                                          leftTitle As String, leftName As String, leftPosition As String,
                                          rightTitle As String, rightName As String, rightPosition As String,
                                          leftX As Integer, rightX As Integer, blockWidth As Integer,
                                          pageBottom As Integer, currentY As Integer,
                                          boldFont As Font, rowFont As Font,
                                          Optional dotSpacing As Integer = 8, Optional dotRadius As Single = 1.5F,
                                          Optional signLineWidth As Integer = 280) As Integer

            Const labelHeight As Integer = 26
            Const labelToSignLine As Integer = 50
            Const signLineToName As Integer = 0
            Const nameHeight As Integer = 26
            Const nameToPosition As Integer = 8
            Const positionHeight As Integer = 26

            ' Calculate total signature section height
            Dim totalSignatureHeight = labelHeight + labelToSignLine + signLineToName + nameHeight + nameToPosition + positionHeight

            ' Position signature block - both blocks share same bottom Y
            Dim blockBottom = pageBottom - 2
            Dim minimumTop = currentY + 12
            If blockBottom - totalSignatureHeight < minimumTop Then
                blockBottom = minimumTop + totalSignatureHeight + 8
            End If

            ' Shared Y coordinates for both blocks
            Dim signatureLabelY As Integer = blockBottom - totalSignatureHeight
            Dim signatureLineY As Integer = signatureLabelY + labelHeight + labelToSignLine
            Dim signatureNameY As Integer = signatureLineY + signLineToName
            Dim signaturePositionY As Integer = signatureNameY + nameHeight + nameToPosition

            ' Center each block in its respective half
            Dim leftBlockX As Integer = leftX + CInt((blockWidth - signLineWidth) / 2)
            Dim rightBlockX As Integer = rightX + CInt((blockWidth - signLineWidth) / 2)

            ' Title format
            Dim fmtTitle As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center,
                .FormatFlags = StringFormatFlags.NoWrap
            }

            ' === DRAW LEFT BLOCK ===
            ' Title
            g.DrawString(leftTitle, boldFont, Brushes.Black,
                        New RectangleF(leftBlockX, signatureLabelY, blockWidth, labelHeight), fmtTitle)

            ' Dotted signature line
            DrawDottedLine(g, leftBlockX + CInt((blockWidth - signLineWidth) / 2), signatureLineY, signLineWidth, dotSpacing, dotRadius)

            ' Name with fallback dots
            Dim leftDisplay As String = If(String.IsNullOrWhiteSpace(leftName), ".....................................",
                                           "(" & leftName & ")")
            Using nameFont As Font = CreateFittedFont(g, leftDisplay, New Font("Tahoma", 12.0!, FontStyle.Bold), blockWidth - 8, 10.0!, FontStyle.Bold)
                g.DrawString(leftDisplay, nameFont, Brushes.Black,
                           New RectangleF(leftBlockX, signatureNameY, blockWidth, nameHeight), fmtTitle)
            End Using

            ' Position
            g.DrawString(leftPosition, rowFont, Brushes.Black,
                        New RectangleF(leftBlockX, signaturePositionY, blockWidth, positionHeight), fmtTitle)

            ' === DRAW RIGHT BLOCK ===
            ' Title
            g.DrawString(rightTitle, boldFont, Brushes.Black,
                        New RectangleF(rightBlockX, signatureLabelY, blockWidth, labelHeight), fmtTitle)

            ' Dotted signature line
            DrawDottedLine(g, rightBlockX + CInt((blockWidth - signLineWidth) / 2), signatureLineY, signLineWidth, dotSpacing, dotRadius)

            ' Name with fallback dots
            Dim rightDisplay As String = If(String.IsNullOrWhiteSpace(rightName), ".....................................",
                                           "(" & rightName & ")")
            Using nameFont As Font = CreateFittedFont(g, rightDisplay, New Font("Tahoma", 12.0!, FontStyle.Bold), blockWidth - 8, 10.0!, FontStyle.Bold)
                g.DrawString(rightDisplay, nameFont, Brushes.Black,
                           New RectangleF(rightBlockX, signatureNameY, blockWidth, nameHeight), fmtTitle)
            End Using

            ' Position
            g.DrawString(rightPosition, rowFont, Brushes.Black,
                        New RectangleF(rightBlockX, signaturePositionY, blockWidth, positionHeight), fmtTitle)

            Return signaturePositionY + positionHeight
        End Function

    End Module
End Namespace
