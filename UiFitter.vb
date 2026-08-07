Option Strict Off
Option Explicit On

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting

    ''' <summary>
    ''' ชุดเครื่องมือปรับขนาดปุ่มและฟอนต์อัตโนมัติ เพื่อป้องกันข้อความไทยถูกตัด/ซ่อน
    ''' (โดยเฉพาะสระ-วรรณยุกต์ที่อยู่เหนือ/ใต้บรรทัด)
    ''' ใช้ TextRenderer.MeasureText เทียบกับขนาดปุ่ม โดยเผื่อระยะกัน 8px ต่อด้าน
    ''' </summary>
    Public Module UiFitter

        Private Const PadX As Integer = 8 ' ระยะกันซ้าย-ขวา (px)
        Private Const PadY As Integer = 8 ' ระยะกันบน-ล่าง (px)

        ''' <summary>
        ''' ปรับปุ่มเดียวให้ข้อความแสดงเต็ม:
        ''' 1) ลดฟอนต์ทีละ 0.5pt (ไม่ต่ำกว่า minFontSize, ไม่เกิน maxFontSize)
        ''' 2) ถ้ายังไม่พอที่ minFontSize → ขึ้น 2 บรรทัด (vbCrLf)
        ''' 3) ถ้ายังไม่พอ → ขยายขนาดปุ่ม (คงฟอนต์ขั้นต่ำ)
        ''' </summary>
        Public Sub AutoFitButtonText(btn As Button, Optional minFontSize As Single = 9, Optional maxFontSize As Single = 11)
            If btn Is Nothing OrElse String.IsNullOrEmpty(btn.Text) Then Return

            btn.AutoSize = False
            btn.AutoEllipsis = False

            ' 1) ถ้าฟอนต์ใหญ่เกิน maxFontSize ให้ลดลงมาก่อน
            If btn.Font.SizeInPoints > maxFontSize Then
                btn.Font = New Font(btn.Font.FontFamily, maxFontSize, btn.Font.Style, GraphicsUnit.Point)
            End If

            ' 2) ลดฟอนต์ทีละ 0.5pt จนกระทั่งข้อความพอดีกับขนาดปุ่ม
            Dim cur As Single = btn.Font.SizeInPoints
            While Not Fits(btn) AndAlso cur - 0.5F >= minFontSize
                cur -= 0.5F
                btn.Font = New Font(btn.Font.FontFamily, cur, btn.Font.Style, GraphicsUnit.Point)
            End While

            ' 3) ถ้ายังล้นที่ minFontSize → ลองขึ้น 2 บรรทัด
            If Not Fits(btn) Then
                Dim wrapped As String = TryWrapTwoLines(btn)
                If wrapped IsNot Nothing Then
                    btn.Text = wrapped
                End If
            End If

            ' 4) ถ้ายังล้น → ขยายขนาดปุ่มให้พอดี (คงฟอนต์ขั้นต่ำไว้)
            If Not Fits(btn) Then
                Dim need As Size = ButtonTextNeed(btn)
                If need.Width > btn.Width Then btn.Width = need.Width
                If need.Height > btn.Height Then btn.Height = need.Height
            End If
        End Sub

        ''' <summary>
        ''' ปรับปุ่มทั้งฟอร์ม (ไล่ลงทุก container รวม ToolStrip/StatusStrip)
        ''' เรียกครั้งเดียวใน Load event ของแต่ละฟอร์ม
        ''' </summary>
        Public Sub AutoFitFormButtons(frm As Form, Optional minFontSize As Single = 9, Optional maxFontSize As Single = 11)
            If frm Is Nothing Then Return
            FitControlTree(frm.Controls, minFontSize, maxFontSize)
        End Sub

        ''' <summary>
        ''' ทำปุ่มในกลุ่มเดียวกันให้มีขนาดเท่ากัน (ใช้ขนาดที่กว้างที่สุด + ระยะกัน 3px)
        ''' เช่น แถวปุ่ม toolbar ของหน้าจอรายงาน เพื่อให้แถวเรียงสวย ไม่ปุ่มเล็กปุ่มใหญ่
        ''' </summary>
        Public Sub UniformButtonGroup(ParamArray btns() As Button)
            If btns Is Nothing OrElse btns.Length = 0 Then Return

            ' ให้แต่ละปุ่ม fit ก่อน (ลดฟอนต์ถ้าจำเป็น)
            For Each b As Button In btns
                If b IsNot Nothing Then AutoFitButtonText(b)
            Next

            ' หาขนาดกว้างสุด/สูงสุดที่จำเป็น
            Dim maxW As Integer = 0
            Dim maxH As Integer = 0
            For Each b As Button In btns
                If b Is Nothing Then Continue For
                Dim need As Size = ButtonTextNeed(b)
                If need.Width > maxW Then maxW = need.Width
                If need.Height > maxH Then maxH = need.Height
            Next

            If maxW > 0 Then maxW += 3 ' ระยะกันเล็กน้อยเพื่อไม่ให้ชิดเกินไป

            ' กำหนดขนาดเท่ากันทุกปุ่ม
            For Each b As Button In btns
                If b Is Nothing Then Continue For
                b.AutoSize = False
                b.Width = maxW
                If maxH > b.Height Then b.Height = maxH
            Next
        End Sub

        ''' <summary>
        ''' คำนวณขนาดจริง (กว้าง+สูง) ที่ปุ่มต้องใช้เพื่อแสดงข้อความเต็ม รวมระยะกันแล้ว
        ''' </summary>
        Public Function ButtonTextNeed(btn As Button) As Size
            If btn Is Nothing OrElse String.IsNullOrEmpty(btn.Text) Then Return Size.Empty
            Dim sz As Size = MeasureText(btn.Text, btn.Font)
            Return New Size(sz.Width + PadX * 2, sz.Height + PadY * 2)
        End Function

        ' ------------------------------------------------------------------
        ' ส่วนภายใน (private helpers)
        ' ------------------------------------------------------------------

        Private Function Fits(btn As Button) As Boolean
            If btn Is Nothing OrElse String.IsNullOrEmpty(btn.Text) Then Return True
            Dim need As Size = ButtonTextNeed(btn)
            Return need.Width <= btn.Width AndAlso need.Height <= btn.Height
        End Function

        ''' <summary>
        ''' วัดขนาดข้อความจริง (ไม่ถูกบีบ/clip) — ข้อความหลายบรรทัดจะได้
        ''' ความกว้างของบรรทัดที่กว้างสุด + ความสูงรวมทุกบรรทัด
        ''' </summary>
        Private Function MeasureText(text As String, font As Font) As Size
            Dim flags As TextFormatFlags = TextFormatFlags.NoPadding Or TextFormatFlags.NoClipping
            If Not text.Contains(vbCrLf) Then
                flags = flags Or TextFormatFlags.SingleLine
            End If
            Return TextRenderer.MeasureText(text, font, New Size(Integer.MaxValue, Integer.MaxValue), flags)
        End Function

        ''' <summary>
        ''' ลองแบ่งข้อความเป็น 2 บรรทัด ณ จุดเว้นวรรคที่ทำให้บรรทัดกว้างสุดสั้นที่สุด
        ''' ใช้ได้เฉพาะเมื่อ 2 บรรทัดพอดีกับความสูงของปุ่มจริง ๆ
        ''' </summary>
        Private Function TryWrapTwoLines(btn As Button) As String
            Dim parts As String() = btn.Text.Split(" "c)
            If parts.Length < 2 Then Return Nothing

            Dim best As String = Nothing
            Dim bestMaxW As Integer = Integer.MaxValue

            For i As Integer = 1 To parts.Length - 1
                Dim l1 As String = String.Join(" ", parts, 0, i)
                Dim l2 As String = String.Join(" ", parts, i, parts.Length - i)
                Dim s1 As Size = MeasureText(l1, btn.Font)
                Dim s2 As Size = MeasureText(l2, btn.Font)
                Dim maxW As Integer = Math.Max(s1.Width, s2.Width)
                If maxW < bestMaxW Then
                    bestMaxW = maxW
                    best = l1 & vbCrLf & l2
                End If
            Next

            If best IsNot Nothing Then
                Dim need As Size = MeasureText(best, btn.Font)
                If need.Width + PadX * 2 <= btn.Width AndAlso need.Height + PadY * 2 <= btn.Height Then
                    Return best
                End If
            End If
            Return Nothing
        End Function

        Private Sub FitControlTree(controls As Control.ControlCollection, minFontSize As Single, maxFontSize As Single)
            For Each ctrl As Control In controls
                Dim b As Button = TryCast(ctrl, Button)
                If b IsNot Nothing Then
                    AutoFitButtonText(b, minFontSize, maxFontSize)
                Else
                    Dim ts As ToolStrip = TryCast(ctrl, ToolStrip)
                    If ts IsNot Nothing Then
                        For Each item As ToolStripItem In ts.Items
                            Dim tb As ToolStripButton = TryCast(item, ToolStripButton)
                            If tb IsNot Nothing Then
                                AutoFitToolStripButton(tb, minFontSize, maxFontSize)
                            End If
                        Next
                    End If
                End If
                If ctrl.HasChildren Then
                    FitControlTree(ctrl.Controls, minFontSize, maxFontSize)
                End If
            Next
        End Sub

        Private Sub AutoFitToolStripButton(tb As ToolStripButton, minFontSize As Single, maxFontSize As Single)
            If tb Is Nothing OrElse String.IsNullOrEmpty(tb.Text) Then Return

            Dim font As Font = tb.Font
            Dim need As Size = MeasureText(tb.Text, font)
            If need.Width + PadX * 2 <= tb.Bounds.Width AndAlso need.Height + PadY * 2 <= tb.Bounds.Height Then
                Return
            End If

            Dim cur As Single = font.SizeInPoints
            While cur - 0.5F >= minFontSize
                cur -= 0.5F
                Dim f As New Font(font.FontFamily, cur, font.Style, GraphicsUnit.Point)
                need = MeasureText(tb.Text, f)
                If need.Width + PadX * 2 <= tb.Bounds.Width AndAlso need.Height + PadY * 2 <= tb.Bounds.Height Then
                    tb.Font = f
                    Return
                End If
            End While
        End Sub

        ' ==================================================================
        ' ComboBox: ป้องกันการเลื่อน Mouse Wheel เปลี่ยนค่าโดยไม่ตั้งใจ
        ' ผู้ใช้ต้องคลิกเปิด dropdown หรือใช้คีย์บอร์ด (ลูกศร) เท่านั้นจึงจะเปลี่ยนค่า
        ' ==================================================================

        ''' <summary>
        ''' ไล่ติดตั้งตัวกัน Mouse Wheel เปลี่ยนค่าให้ ComboBox ทุกตัวในฟอร์ม
        ''' (รวมที่ซ้อนอยู่ใน GroupBox/TableLayoutPanel/Panel) แบบวนซ้ำอัตโนมัติ
        ''' เรียกครั้งเดียวใน Load event ของฟอร์ม เช่น: UiFitter.DisableComboBoxWheel(Me)
        ''' </summary>
        Public Sub DisableComboBoxWheel(parent As Control)
            If parent Is Nothing Then Return
            For Each ctrl As Control In parent.Controls
                If TypeOf ctrl Is ComboBox Then
                    AddHandler ctrl.MouseWheel, AddressOf ComboBox_PreventWheelChange
                End If
                If ctrl.HasChildren Then
                    DisableComboBoxWheel(ctrl)
                End If
            Next
        End Sub

        ''' <summary>
        ''' ตัวจัดการเหตุการณ์ MouseWheel ของ ComboBox:
        ''' 1) ระงับไม่ให้ค่าเปลี่ยนเมื่อล้อเมาส์เลื่อนพาดผ่าน (แม้ dropdown ปิดอยู่)
        ''' 2) ส่งต่อการเลื่อนไปยังคอนเทนเนอร์แม่ที่ AutoScroll เพื่อให้หน้าจอเลื่อนตามปกติ
        ''' </summary>
        Private Sub ComboBox_PreventWheelChange(sender As Object, e As MouseEventArgs)
            Dim he = TryCast(e, HandledMouseEventArgs)
            If he Is Nothing Then Return

            ' ระงับการเปลี่ยนค่า (Handled=True ทำให้ Wheel ไม่ถูกส่งต่อให้ native control)
            he.Handled = True

            ' ส่งต่อการเลื่อนให้ panel แม่ที่เลื่อนได้ (AutoScroll) เพื่อให้หน้าจอยังเลื่อนปกติ
            Dim combo = TryCast(sender, Control)
            If combo Is Nothing Then Return
            Dim p As Control = combo.Parent
            While p IsNot Nothing
                If TypeOf p Is ScrollableControl Then
                    Dim sc = DirectCast(p, ScrollableControl)
                    If sc.AutoScroll Then
                        sc.AutoScrollPosition = New Point(sc.AutoScrollPosition.X, sc.AutoScrollPosition.Y + e.Delta)
                        Exit While
                    End If
                End If
                p = p.Parent
            End While
        End Sub

    End Module
End Namespace
