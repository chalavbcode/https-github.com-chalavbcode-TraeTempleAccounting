Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Drawing.Printing
Imports System.Windows.Forms
Imports System.Windows.Forms.DataVisualization.Charting

Namespace TempleAccounting
    ''' <summary>
    ''' ฟอร์มแสดงกราฟแบบเต็มจอ (Full-Screen Chart Preview)
    ''' พร้อมปุ่มพิมพ์, บันทึกรูปภาพ (PNG/JPEG), และ Zoom/Pan ด้วยเมาส์
    ''' </summary>
    <DesignerCategory("Form")>
    Partial Public Class FrmChartPreview
        Inherits Form

        ' ====================== Controls ======================
        Private WithEvents btnPrintChart As Button
        Private WithEvents btnSaveImage As Button
        Private WithEvents btnClose As Button
        Private WithEvents chartPreview As Chart
        Private panelTop As Panel
        Private _doc As PrintDocument
        Private _chartBitmap As Bitmap   ' bitmap ล่าสุดของกราฟ (สร้างก่อนพิมพ์)

        ' ====================== Constructor ======================
        Public Sub New()
            Me.WindowState = FormWindowState.Maximized
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.Text = "📊 ดูกราฟเต็มจอ — Temple Accounting"
            Me.Font = New Font("Tahoma", 10.0F)
            Me.BackColor = Color.FromArgb(254, 249, 235)
            Me.KeyPreview = True

            BuildUI()
        End Sub

        ' ====================== สร้าง UI ด้วยโค้ด ======================
        Private Sub BuildUI()
            ' --- Top Panel ---
            panelTop = New Panel() With {
                .Dock = DockStyle.Top,
                .Height = 48,
                .Padding = New Padding(12, 6, 12, 6),
                .BackColor = Color.FromArgb(52, 73, 94)
            }
            Me.Controls.Add(panelTop)

            ' --- ปุ่ม Close (ขวาสุด) ---
            btnClose = CreateToolButton("❌ ปิดหน้าต่าง", Color.FromArgb(231, 76, 60), DockStyle.Right)
            btnClose.Width = 140
            panelTop.Controls.Add(btnClose)

            ' --- ปุ่ม Save Image ---
            btnSaveImage = CreateToolButton("💾 บันทึกรูปภาพ", Color.FromArgb(39, 174, 96), DockStyle.Right)
            btnSaveImage.Width = 160
            panelTop.Controls.Add(btnSaveImage)

            ' --- ปุ่ม Print ---
            btnPrintChart = CreateToolButton("🖨️ พิมพ์กราฟ", Color.FromArgb(41, 128, 185), DockStyle.Right)
            btnPrintChart.Width = 150
            panelTop.Controls.Add(btnPrintChart)

            ' --- Chart Control (เต็มพื้นที่ที่เหลือ) ---
            chartPreview = New Chart() With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.White,
                .BorderlineColor = Color.FromArgb(200, 200, 200),
                .BorderlineDashStyle = ChartDashStyle.Solid,
                .BorderlineWidth = 1,
                .BorderSkin = New BorderSkin() With {
                    .SkinStyle = BorderSkinStyle.None
                }
            }
            Me.Controls.Add(chartPreview)

            ' --- PrintDocument ---
            _doc = New PrintDocument()
            AddHandler _doc.PrintPage, AddressOf PrintChartPage
        End Sub

        ''' <summary>
        ''' สร้างปุ่มแถบเครื่องมือ — Flat style, สีขาว, Dock ตามที่กำหนด
        ''' </summary>
        Private Function CreateToolButton(text As String, bgColor As Color, dock As DockStyle) As Button
            Dim btn As New Button() With {
                .Text = text,
                .BackColor = bgColor,
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Tahoma", 9.0F, FontStyle.Bold),
                .Height = 36,
                .Dock = dock,
                .Cursor = Cursors.Hand,
                .UseVisualStyleBackColor = False,
                .TextAlign = ContentAlignment.MiddleCenter
            }
            btn.FlatAppearance.BorderSize = 0
            Return btn
        End Function

        ' ====================== โหลดข้อมูลกราฟจากภายนอก ======================
        ''' <summary>
        ''' Clone ข้อมูลกราฟ (ChartArea, Series, Legend, Titles) จาก source Chart
        ''' เข้าสู่ chartPreview — พร้อมเปิด Zoom/Pan/Scroll
        ''' </summary>
        Public Sub LoadChart(source As Chart)
            If source Is Nothing Then Return

            chartPreview.DataSource = Nothing
            chartPreview.Titles.Clear()
            chartPreview.Annotations.Clear()
            chartPreview.Series.Clear()
            chartPreview.ChartAreas.Clear()
            chartPreview.Legends.Clear()
            chartPreview.Palette = ChartColorPalette.None

            ' --- Clone ChartAreas ---
            For Each srcArea As ChartArea In source.ChartAreas
                Dim dstArea As New ChartArea(srcArea.Name)
                dstArea.BackColor = srcArea.BackColor

                ' แกน X
                dstArea.AxisX.Interval = srcArea.AxisX.Interval
                dstArea.AxisX.LabelStyle.Font = CloneFont(srcArea.AxisX.LabelStyle.Font)
                dstArea.AxisX.LabelStyle.Angle = srcArea.AxisX.LabelStyle.Angle
                dstArea.AxisX.MajorGrid.Enabled = srcArea.AxisX.MajorGrid.Enabled
                dstArea.AxisX.MajorGrid.LineColor = srcArea.AxisX.MajorGrid.LineColor
                dstArea.AxisX.Title = srcArea.AxisX.Title
                dstArea.AxisX.TitleFont = CloneFont(srcArea.AxisX.TitleFont)

                ' แกน Y
                dstArea.AxisY.IsStartedFromZero = srcArea.AxisY.IsStartedFromZero
                dstArea.AxisY.LabelStyle.Format = srcArea.AxisY.LabelStyle.Format
                dstArea.AxisY.LabelStyle.Font = CloneFont(srcArea.AxisY.LabelStyle.Font)
                dstArea.AxisY.Title = srcArea.AxisY.Title
                dstArea.AxisY.TitleFont = CloneFont(srcArea.AxisY.TitleFont)
                dstArea.AxisY.MajorGrid.Enabled = srcArea.AxisY.MajorGrid.Enabled
                dstArea.AxisY.MajorGrid.LineColor = srcArea.AxisY.MajorGrid.LineColor

                ' Zoom/Scroll (เปิดทั้ง X และ Y ใน preview แบบเต็มจอ)
                dstArea.AxisX.ScaleView.Zoomable = True
                dstArea.AxisX.ScrollBar.Enabled = True
                dstArea.AxisX.ScrollBar.BackColor = Color.Gainsboro
                dstArea.CursorX.IsUserSelectionEnabled = True
                dstArea.CursorX.IsUserEnabled = True

                dstArea.AxisY.ScaleView.Zoomable = True
                dstArea.AxisY.ScrollBar.Enabled = True
                dstArea.AxisY.ScrollBar.BackColor = Color.Gainsboro
                dstArea.CursorY.IsUserSelectionEnabled = True
                dstArea.CursorY.IsUserEnabled = True

                chartPreview.ChartAreas.Add(dstArea)
            Next

            ' --- Clone Legends ---
            For Each srcLegend As Legend In source.Legends
                Dim dstLegend As New Legend(srcLegend.Name) With {
                    .Docking = srcLegend.Docking,
                    .Font = CloneFont(srcLegend.Font)
                }
                chartPreview.Legends.Add(dstLegend)
            Next

            ' --- Clone Series (with all data points and properties) ---
            For Each srcSeries As Series In source.Series
                Dim dstSeries As New Series(srcSeries.Name) With {
                    .ChartType = srcSeries.ChartType,
                    .ChartArea = srcSeries.ChartArea,
                    .Color = srcSeries.Color,
                    .BorderWidth = srcSeries.BorderWidth,
                    .IsValueShownAsLabel = srcSeries.IsValueShownAsLabel,
                    .LabelFormat = srcSeries.LabelFormat,
                    .Font = CloneFont(srcSeries.Font)
                }

                ' Custom properties
                If srcSeries.IsCustomPropertySet("PointWidth") Then
                    dstSeries("PointWidth") = srcSeries("PointWidth")
                End If
                dstSeries.SmartLabelStyle.Enabled = srcSeries.SmartLabelStyle.Enabled
                dstSeries.SmartLabelStyle.CalloutLineColor = srcSeries.SmartLabelStyle.CalloutLineColor

                ' Clone data points
                For Each srcPt As DataPoint In srcSeries.Points
                    Dim dstPt As New DataPoint(srcPt.XValue, srcPt.YValues)
                    If Not String.IsNullOrEmpty(srcPt.AxisLabel) Then dstPt.AxisLabel = srcPt.AxisLabel
                    If srcPt.Font IsNot Nothing Then
                        dstPt.Font = CloneFont(srcPt.Font)
                    End If

                    ' รวมแท่ง (Total) — ถ้ามีสีเฉพาะของจุดนั้น ให้ใช้สีนั้นแทนสีของ Series
                    If srcPt.Color <> srcSeries.Color AndAlso srcPt.Color <> Color.Empty Then
                        dstPt.Color = srcPt.Color
                    End If

                    dstSeries.Points.Add(dstPt)
                Next

                chartPreview.Series.Add(dstSeries)
            Next

            ' --- Clone Titles ---
            For Each srcTitle As Title In source.Titles
                Dim dstTitle As New Title(srcTitle.Text) With {
                    .Font = CloneFont(srcTitle.Font),
                    .Docking = srcTitle.Docking,
                    .ForeColor = srcTitle.ForeColor
                }
                chartPreview.Titles.Add(dstTitle)
            Next

            ' --- Mouse Event Handlers สำหรับ Zoom/Pan (ล้อเมาส์ + ดับเบิลคลิกรีเซ็ต) ---
            AddHandler chartPreview.MouseWheel, AddressOf ChartPreview_MouseWheel
            AddHandler chartPreview.MouseDoubleClick, AddressOf ChartPreview_MouseDoubleClick
            AddHandler chartPreview.MouseEnter, AddressOf ChartPreview_MouseEnter
        End Sub

        ''' <summary>
        ''' Clone Font (Font เป็น MarshalByRefObject — Clone() ใช้ได้โดยตรง)
        ''' </summary>
        Private Shared Function CloneFont(f As Font) As Font
            If f Is Nothing Then Return Nothing
            Return CType(f.Clone(), Font)
        End Function

        ' ====================== Zoom / Pan ======================
        Private Sub ChartPreview_MouseWheel(sender As Object, e As MouseEventArgs)
            Try
                If chartPreview.ChartAreas.Count = 0 Then Return
                Dim area = chartPreview.ChartAreas(0)
                Dim axisY = area.AxisY

                ' หาช่วงข้อมูลจริง
                Dim fullMin As Double = Double.MaxValue
                Dim fullMax As Double = Double.MinValue
                For Each s As Series In chartPreview.Series
                    For Each p As DataPoint In s.Points
                        If p.YValues IsNot Nothing AndAlso p.YValues.Length > 0 Then
                            Dim v = p.YValues(0)
                            If v < fullMin Then fullMin = v
                            If v > fullMax Then fullMax = v
                        End If
                    Next
                Next
                If fullMin >= fullMax Then Return
                Dim margin = (fullMax - fullMin) * 0.05
                fullMin -= margin
                fullMax += margin
                Dim fullRange As Double = fullMax - fullMin
                If fullRange <= 0 Then Return

                Dim viewMin As Double = axisY.ScaleView.ViewMinimum
                Dim viewMax As Double = axisY.ScaleView.ViewMaximum
                If Double.IsNaN(viewMin) OrElse Double.IsNaN(viewMax) Then
                    viewMin = fullMin
                    viewMax = fullMax
                End If

                Dim zoomFactor As Double = If(e.Delta > 0, 0.9, 1.1)
                Dim range = viewMax - viewMin
                Dim newRange = range * zoomFactor

                If newRange >= fullRange Then
                    axisY.ScaleView.ZoomReset()
                Else
                    Dim center = viewMin + range / 2
                    Dim newMin = center - newRange / 2
                    Dim newMax = center + newRange / 2
                    If newMin < fullMin Then newMin = fullMin
                    If newMax > fullMax Then newMax = fullMax
                    If newMax - newMin < 0.001 Then Return
                    axisY.ScaleView.Zoom(newMin, newMax)
                End If
                chartPreview.Invalidate()
            Catch
            End Try
        End Sub

        Private Sub ChartPreview_MouseDoubleClick(sender As Object, e As MouseEventArgs)
            Try
                If chartPreview.ChartAreas.Count > 0 Then
                    chartPreview.ChartAreas(0).AxisY.ScaleView.ZoomReset()
                    chartPreview.ChartAreas(0).AxisX.ScaleView.ZoomReset()
                    chartPreview.Invalidate()
                End If
            Catch
            End Try
        End Sub

        Private Sub ChartPreview_MouseEnter(sender As Object, e As EventArgs)
            chartPreview.Focus()
        End Sub

        ' ====================== Toolbar Actions ======================
        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

        Private Sub btnSaveImage_Click(sender As Object, e As EventArgs) Handles btnSaveImage.Click
            Try
                Using sfd As New SaveFileDialog()
                    sfd.Title = "บันทึกกราฟเป็นรูปภาพ"
                    sfd.Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg"
                    sfd.FilterIndex = 1
                    sfd.FileName = "กราฟสรุปรายเดือน.png"
                    sfd.OverwritePrompt = True
                    sfd.AddExtension = True

                    If sfd.ShowDialog(Me) <> DialogResult.OK Then Return

                    Dim fmt As Imaging.ImageFormat = If(
                        sfd.FilterIndex = 2, Imaging.ImageFormat.Jpeg, Imaging.ImageFormat.Png)

                    ' Render chart เป็น bitmap ขนาดเต็ม
                    Dim bmp As Bitmap = RenderChartToBitmap()
                    bmp.Save(sfd.FileName, fmt)
                    bmp.Dispose()

                    MessageBox.Show("บันทึกเรียบร้อย:" & vbCrLf & sfd.FileName,
                                    "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            Catch ex As Exception
                MessageBox.Show("บันทึกไม่สำเร็จ: " & ex.Message,
                                "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub btnPrintChart_Click(sender As Object, e As EventArgs) Handles btnPrintChart.Click
            Try
                ' สร้าง bitmap ก่อน แล้วพิมพ์ผ่าน PrintPreviewDialog
                _chartBitmap = RenderChartToBitmap()

                Using ppd As New PrintPreviewDialog()
                    ppd.Document = _doc
                    ppd.WindowState = FormWindowState.Maximized
                    ppd.Width = 1200 : ppd.Height = 800
                    ppd.PrintPreviewControl.Rows = 1
                    ppd.PrintPreviewControl.Columns = 1
                    ppd.PrintPreviewControl.AutoZoom = True
                    ppd.ShowDialog(Me)
                End Using
            Catch ex As Exception
                MessageBox.Show("พิมพ์ไม่สำเร็จ: " & ex.Message,
                                "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If _chartBitmap IsNot Nothing Then
                    _chartBitmap.Dispose()
                    _chartBitmap = Nothing
                End If
            End Try
        End Sub

        ' ====================== การพิมพ์ ======================
        Private Sub PrintChartPage(sender As Object, e As PrintPageEventArgs)
            If _chartBitmap Is Nothing Then Return

            ' พิมพ์แนวนอน (Landscape) — เลือกขอบเขตที่ใหญ่ที่สุดเท่าที่พอดี
            Dim margin = e.MarginBounds
            Dim pageW = margin.Width
            Dim pageH = margin.Height

            ' Scale bitmap ให้พอดีหน้ากระดาษ (รักษาสัดส่วน)
            Dim ratioW As Single = pageW / CSng(_chartBitmap.Width)
            Dim ratioH As Single = pageH / CSng(_chartBitmap.Height)
            Dim ratio As Single = Math.Min(ratioW, ratioH)

            Dim drawW As Integer = CInt(_chartBitmap.Width * ratio)
            Dim drawH As Integer = CInt(_chartBitmap.Height * ratio)

            ' จัดกึ่งกลางกระดาษ
            Dim x = margin.Left + CInt((pageW - drawW) / 2)
            Dim y = margin.Top + CInt((pageH - drawH) / 2)

            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
            e.Graphics.DrawImage(_chartBitmap, New Rectangle(x, y, drawW, drawH))

            e.HasMorePages = False
        End Sub

        ' ====================== Render Chart → Bitmap ======================
        Private Function RenderChartToBitmap() As Bitmap
            ' ใช้ขนาดตามความกว้างของหน้าจอปัจจุบัน (HiDPI aware)
            Dim w As Integer = Math.Max(chartPreview.Width, 1200)
            Dim h As Integer = Math.Max(chartPreview.Height, 800)

            Dim bmp As New Bitmap(w, h)
            bmp.SetResolution(96, 96)
            chartPreview.DrawToBitmap(bmp, New Rectangle(0, 0, w, h))
            Return bmp
        End Function

        ' ====================== F1 Help (กด F1 เพื่อดูวิธีใช้) ======================
        Private Sub FrmChartPreview_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.KeyCode = Keys.F1 Then
                e.Handled = True
                e.SuppressKeyPress = True
                HelpSystem.ShowManual("", Me)
            End If
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                If _chartBitmap IsNot Nothing Then
                    _chartBitmap.Dispose()
                    _chartBitmap = Nothing
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
