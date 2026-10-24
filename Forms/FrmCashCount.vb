Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    ''' <summary>
    ''' หน้าต่าง "นับเงินสด" — บันทึกยอดนับจริง ณ วันที่เลือก
    ''' • กรอกมูลค่า (บาท) แยกตามชนิดธนบัตร/เหรียญ 9 ช่อง พร้อมรวมอัตโนมัติ
    ''' • แสดงยอดคาดการณ์/ผลต่างเทียบกับข้อมูลธุรกรรมก่อนบันทึก
    ''' • แสดงตารางการนับก่อนหน้า (แทรกอย่างเดียว ไม่ลบ/ไม่แก้ไขแถวเดิม)
    ''' • Layout = TableLayoutPanel ล้วน + ฉลาก AutoSize (ฟอนต์ไทยไม่ถูกตัดสระบน/ล่าง)
    ''' • DPI: AutoScaleMode.Dpi + ค่าคงที่ทุกจุดคูด้วย K() (สเกลตาม DeviceDpi) — ปรับแต่งในโค้ด ไม่ใช้ Height ตรง ๆ
    ''' </summary>
    Public Class FrmCashCount
        Inherits Form

        Private dtpDate As DateTimePicker
        Private _denomBoxes As TextBox()
        Private lblTotal As Label
        Private lblExpected As Label
        Private lblVariance As Label
        Private txtNote As TextBox
        Private dgvHistory As DataGridView
        Private btnSave As Button
        Private btnCancel As Button
        Private _expected As Decimal = 0D

        ''' <summary>สเกล DPI ของฟอร์มนี้ (DeviceDpi / 96) — ค่าคงที่ทุกจุดคูด้วย K() ให้เท่ากับฟอนต์ที่ขยายตาม DPI</summary>
        Private _uiScale As Single = 1.0F

        Private Shared ReadOnly DenomLabels As String() = {
            "เหรียญ 1 บาท", "เหรียญ 2 บาท", "เหรียญ 5 บาท", "เหรียญ 10 บาท",
            "ธนบัตร 20 บาท", "ธนบัตร 50 บาท", "ธนบัตร 100 บาท", "ธนบัตร 500 บาท", "ธนบัตร 1,000 บาท"}

        Public Sub New()
            InitializeUI()
            LoadHistory()
            RecalcExpected()
        End Sub

        ''' <summary>แปลงค่าพิกเซลที่ออกแบบที่ 96 DPI ให้เป็นพิกเซล ณ DPI ปัจจุบันของเครื่อง</summary>
        Private Function K(v As Integer) As Integer
            If _uiScale = 1.0F Then Return v
            Return CInt(Math.Round(CDbl(v) * CDbl(_uiScale)))
        End Function

        Private Sub InitializeUI()
            ' ── DPI: คูนค่าคงที่ทั้งหมดตาม DeviceDpi (ค่าฟอนต์/LabelText ขยายตาม point อยู่แล้ว) ──
            Dim dpi As Integer = Me.DeviceDpi
            If dpi <= 0 Then dpi = 96
            _uiScale = CSng(dpi) / 96.0F

            Me.Text = "นับเงินสด"
            Me.StartPosition = FormStartPosition.CenterParent
            Me.AutoScaleDimensions = New SizeF(96.0F, 96.0F)
            Me.AutoScaleMode = AutoScaleMode.Dpi
            Me.Font = New Font("Tahoma", 10.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            Me.BackColor = Color.FromArgb(255, 253, 244)
            Me.FormBorderStyle = FormBorderStyle.Sizable
            Me.ClientSize = New Size(K(740), K(620))
            Me.MinimumSize = New Size(K(660), K(515))   ' content ≈ 445px + ตาราง ≈ 40px + กรอบหน้าต่าง → เห็นครบทุกส่วนทุก DPI

            ' ── Layout หลัก: TableLayoutPanel Dock=Fill แบ่งแถวบนลงล่าง ──
            Dim root As New TableLayoutPanel()
            root.Dock = DockStyle.Fill
            root.ColumnCount = 1
            root.RowCount = 7
            root.Padding = New Padding(K(10), K(8), K(10), K(8))
            root.Margin = New Padding(0)
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
            root.RowStyles.Add(New RowStyle(SizeType.AutoSize))        ' 0 = หัวข้อ
            root.RowStyles.Add(New RowStyle(SizeType.AutoSize))        ' 1 = วันที่นับ
            root.RowStyles.Add(New RowStyle(SizeType.AutoSize))        ' 2 = มูลค่าแยกชนิด
            root.RowStyles.Add(New RowStyle(SizeType.AutoSize))        ' 3 = สรุปยอด
            root.RowStyles.Add(New RowStyle(SizeType.AutoSize))        ' 4 = หมายเหตุ
            root.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F)) ' 5 = ตารางการนับก่อนหน้า
            root.RowStyles.Add(New RowStyle(SizeType.AutoSize))        ' 6 = ปุ่มล่าง
            Me.Controls.Add(root)

            ' ── 1) แถบหัวข้อ: ชื่อจอ + คำอธิบาย (AutoSize, เว้นระหว่างบรรทัด 6px) ──
            Dim pnlHeader As New TableLayoutPanel()
            pnlHeader.Dock = DockStyle.Fill
            pnlHeader.AutoSize = True
            pnlHeader.ColumnCount = 1
            pnlHeader.RowCount = 2
            pnlHeader.Margin = New Padding(0, 0, 0, K(4))
            pnlHeader.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
            pnlHeader.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            pnlHeader.RowStyles.Add(New RowStyle(SizeType.AutoSize))

            Dim lblTitle As New Label()
            lblTitle.AutoSize = True
            lblTitle.Font = New Font("Tahoma", 15.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblTitle.ForeColor = Color.FromArgb(120, 53, 15)
            lblTitle.Text = "💰 นับเงินสด"
            lblTitle.Margin = New Padding(0, 0, 0, K(6))
            pnlHeader.Controls.Add(lblTitle, 0, 0)

            Dim lblHint As New Label()
            lblHint.AutoSize = True
            lblHint.Font = New Font("Tahoma", 9.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            lblHint.ForeColor = Color.FromArgb(120, 80, 40)
            lblHint.Text = "กรอกมูลค่า (บาท) ของเงินสดแต่ละชนิด ระบบจะรวมและเทียบกับยอดคาดการณ์ให้อัตโนมัติ"
            lblHint.Margin = New Padding(0, 0, 0, K(2))
            pnlHeader.Controls.Add(lblHint, 0, 1)
            root.Controls.Add(pnlHeader, 0, 0)

            ' ── 2) แถบวันที่นับ (AutoSize) ──
            Dim pnlDate As New FlowLayoutPanel()
            pnlDate.Dock = DockStyle.Fill
            pnlDate.AutoSize = True
            pnlDate.WrapContents = False
            pnlDate.Margin = New Padding(0, K(2), 0, K(2))
            pnlDate.Padding = New Padding(0)

            Dim lblDate As New Label()
            lblDate.Text = "วันที่นับ"
            lblDate.AutoSize = True
            lblDate.ForeColor = Color.FromArgb(120, 53, 15)
            lblDate.Margin = New Padding(0, K(5), K(8), 0)
            pnlDate.Controls.Add(lblDate)

            dtpDate = New DateTimePicker()
            dtpDate.Format = DateTimePickerFormat.Short
            dtpDate.Value = Date.Today
            dtpDate.Width = K(170)    ' ความสูงให้ฟอนต์กำหนด ไม่ตั้ง Height ตรง ๆ
            dtpDate.Margin = New Padding(0, K(1), 0, 0)
            pnlDate.Controls.Add(dtpDate)
            root.Controls.Add(pnlDate, 0, 1)

            ' ── 3) มูลค่าเงินสดแยกตามชนิด: หัวข้อ AutoSize คนละแถว กับตาราง 3x3 (ไม่มีทางทับกัน) ──
            Dim pnlDenom As New TableLayoutPanel()
            pnlDenom.Dock = DockStyle.Fill
            pnlDenom.AutoSize = True
            pnlDenom.ColumnCount = 1
            pnlDenom.RowCount = 2
            pnlDenom.Margin = New Padding(0, K(4), 0, K(4))
            pnlDenom.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
            pnlDenom.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            pnlDenom.RowStyles.Add(New RowStyle(SizeType.AutoSize))

            Dim lblDenomHint As New Label()
            lblDenomHint.Text = "มูลค่าเงินสดแยกตามชนิด (บาท)"
            lblDenomHint.AutoSize = True
            lblDenomHint.Font = New Font("Tahoma", 9.5!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblDenomHint.ForeColor = Color.FromArgb(120, 80, 40)
            lblDenomHint.Margin = New Padding(0, 0, 0, K(6))
            pnlDenom.Controls.Add(lblDenomHint, 0, 0)

            Dim tbl As New TableLayoutPanel()
            tbl.Dock = DockStyle.Fill
            tbl.AutoSize = True    ' สำคัญ: ถ้า False row AutoSize ของ pnlDenom จะได้ height default 100px → แถว 2-3 ถูกตัด
            tbl.ColumnCount = 3
            tbl.RowCount = 3
            tbl.Margin = New Padding(0)
            tbl.Padding = New Padding(0)
            For i As Integer = 0 To 2
                tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))
                tbl.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            Next
            pnlDenom.Controls.Add(tbl, 0, 1)
            root.Controls.Add(pnlDenom, 0, 2)

            ReDim _denomBoxes(8)
            For i As Integer = 0 To 8
                ' เซลล์ = 2 แถว: ฉลาก (AutoSize) อยู่เหนือ TextBox เสมอ — ไม่มี Height คงที่ จึงไม่ตัดสระบน/ล่าง
                Dim cell As New TableLayoutPanel()
                cell.Dock = DockStyle.Fill
                cell.AutoSize = True
                cell.ColumnCount = 1
                cell.RowCount = 2
                cell.Margin = New Padding(K(6), K(2), K(6), K(4))
                cell.Padding = New Padding(0)
                cell.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
                cell.RowStyles.Add(New RowStyle(SizeType.AutoSize))
                cell.RowStyles.Add(New RowStyle(SizeType.AutoSize))

                Dim lbl As New Label()
                lbl.Text = DenomLabels(i)
                lbl.AutoSize = True
                lbl.Font = New Font("Tahoma", 9.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
                lbl.ForeColor = Color.FromArgb(120, 80, 40)
                lbl.Margin = New Padding(0, 0, 0, K(3))
                cell.Controls.Add(lbl, 0, 0)

                Dim txt As New TextBox()
                txt.Text = "0"
                txt.Font = New Font("Tahoma", 11.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
                txt.TextAlign = HorizontalAlignment.Right
                txt.BorderStyle = BorderStyle.FixedSingle
                txt.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
                txt.Margin = New Padding(0)
                cell.Controls.Add(txt, 0, 1)

                _denomBoxes(i) = txt
                AddHandler txt.TextChanged, AddressOf DenomBox_TextChanged

                tbl.Controls.Add(cell, i Mod 3, i \ 3)
            Next

            ' ── 4) สรุปยอด 3 คอลัมน์: แคปชันอยู่เหนือค่าเสมอ (AutoSize ทั้งคู่ ไม่ทับกัน) ──
            Dim pnlSummary As New TableLayoutPanel()
            pnlSummary.Dock = DockStyle.Fill
            pnlSummary.AutoSize = True
            pnlSummary.ColumnCount = 3
            pnlSummary.RowCount = 2
            pnlSummary.Margin = New Padding(0, K(4), 0, K(4))
            pnlSummary.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 38.0F))
            pnlSummary.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 32.0F))
            pnlSummary.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30.0F))
            pnlSummary.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            pnlSummary.RowStyles.Add(New RowStyle(SizeType.AutoSize))

            Dim lblTotalCaption As New Label()
            lblTotalCaption.Text = "รวมเงินสดที่นับได้ (บาท)"
            lblTotalCaption.AutoSize = True
            lblTotalCaption.ForeColor = Color.FromArgb(120, 53, 15)
            lblTotalCaption.Margin = New Padding(0, 0, 0, K(2))
            pnlSummary.Controls.Add(lblTotalCaption, 0, 0)

            lblTotal = New Label()
            lblTotal.Text = "0.00"
            lblTotal.AutoSize = True
            lblTotal.Font = New Font("Tahoma", 17.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblTotal.ForeColor = Color.FromArgb(22, 101, 52)
            lblTotal.Margin = New Padding(0)
            pnlSummary.Controls.Add(lblTotal, 0, 1)

            Dim lblExpCaption As New Label()
            lblExpCaption.Text = "ยอดคาดการณ์ (จากธุรกรรม)"
            lblExpCaption.AutoSize = True
            lblExpCaption.ForeColor = Color.FromArgb(120, 80, 40)
            lblExpCaption.Margin = New Padding(0, K(4), 0, K(2))
            pnlSummary.Controls.Add(lblExpCaption, 1, 0)

            lblExpected = New Label()
            lblExpected.Text = "0.00"
            lblExpected.AutoSize = True
            lblExpected.Font = New Font("Tahoma", 10.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblExpected.ForeColor = Color.FromArgb(30, 64, 175)
            lblExpected.Margin = New Padding(0, K(4), 0, 0)
            pnlSummary.Controls.Add(lblExpected, 1, 1)

            Dim lblVarCaption As New Label()
            lblVarCaption.Text = "ผลต่าง"
            lblVarCaption.AutoSize = True
            lblVarCaption.ForeColor = Color.FromArgb(120, 80, 40)
            lblVarCaption.Margin = New Padding(0, K(4), 0, K(2))
            pnlSummary.Controls.Add(lblVarCaption, 2, 0)

            lblVariance = New Label()
            lblVariance.Text = "0.00"
            lblVariance.AutoSize = True
            lblVariance.Font = New Font("Tahoma", 10.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblVariance.ForeColor = Color.FromArgb(22, 101, 52)
            lblVariance.Margin = New Padding(0, K(4), 0, 0)
            pnlSummary.Controls.Add(lblVariance, 2, 1)
            root.Controls.Add(pnlSummary, 0, 3)

            ' ── 5) แถบหมายเหตุ (AutoSize) ──
            Dim pnlNote As New TableLayoutPanel()
            pnlNote.Dock = DockStyle.Fill
            pnlNote.AutoSize = True
            pnlNote.ColumnCount = 2
            pnlNote.RowCount = 1
            pnlNote.Margin = New Padding(0, K(2), 0, K(4))
            pnlNote.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
            pnlNote.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
            pnlNote.RowStyles.Add(New RowStyle(SizeType.AutoSize))

            Dim lblNote As New Label()
            lblNote.Text = "หมายเหตุ"
            lblNote.AutoSize = True
            lblNote.ForeColor = Color.FromArgb(120, 53, 15)
            lblNote.Margin = New Padding(0, K(5), K(8), 0)
            pnlNote.Controls.Add(lblNote, 0, 0)

            txtNote = New TextBox()
            txtNote.Font = New Font("Tahoma", 10.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            txtNote.BorderStyle = BorderStyle.FixedSingle
            txtNote.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            txtNote.Margin = New Padding(0, K(1), 0, 0)
            pnlNote.Controls.Add(txtNote, 1, 0)
            root.Controls.Add(pnlNote, 0, 4)

            ' ── 6) ตารางการนับก่อนหน้า (แถว Percent ยืดพื้นที่ที่เหลือทั้งหมด) ──
            Dim pnlHistory As New TableLayoutPanel()
            pnlHistory.Dock = DockStyle.Fill
            pnlHistory.ColumnCount = 1
            pnlHistory.RowCount = 2
            pnlHistory.Margin = New Padding(0)
            pnlHistory.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
            pnlHistory.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            pnlHistory.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

            Dim lblHistory As New Label()
            lblHistory.Text = "📋 การนับก่อนหน้า"
            lblHistory.AutoSize = True
            lblHistory.Font = New Font("Tahoma", 9.5!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblHistory.ForeColor = Color.FromArgb(120, 80, 40)
            lblHistory.Margin = New Padding(0, K(2), 0, K(4))
            pnlHistory.Controls.Add(lblHistory, 0, 0)

            dgvHistory = New DataGridView()
            dgvHistory.Dock = DockStyle.Fill
            dgvHistory.Margin = New Padding(0)
            dgvHistory.ReadOnly = True
            dgvHistory.AllowUserToAddRows = False
            dgvHistory.AllowUserToDeleteRows = False
            dgvHistory.AllowUserToResizeRows = False
            dgvHistory.RowHeadersVisible = False
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvHistory.BackgroundColor = Color.White
            dgvHistory.BorderStyle = BorderStyle.None
            dgvHistory.EnableHeadersVisualStyles = False

            ' หัวตาราง: สีครีมเข้มทุกคอลัมน์เท่ากัน (SelectionBackColor = BackColor แก้หัว "วันที่" เป็นสีน้ำเงิน)
            Dim headerBack As Color = Color.FromArgb(242, 231, 205)
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = headerBack
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(120, 53, 15)
            dgvHistory.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerBack
            dgvHistory.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(120, 53, 15)
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 9.0!, FontStyle.Bold, GraphicsUnit.Point, 222)

            ' แถวข้อมูล: ไฮไลต์สีครีมส้ม แทนสีน้ำเงินของระบบ
            dgvHistory.DefaultCellStyle.BackColor = Color.White
            dgvHistory.DefaultCellStyle.ForeColor = Color.FromArgb(70, 50, 30)
            dgvHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 236, 200)
            dgvHistory.DefaultCellStyle.SelectionForeColor = Color.FromArgb(90, 40, 10)
            dgvHistory.DefaultCellStyle.Font = New Font("Tahoma", 9.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            dgvHistory.GridColor = Color.FromArgb(228, 216, 188)

            pnlHistory.Controls.Add(dgvHistory, 0, 1)
            root.Controls.Add(pnlHistory, 0, 5)

            ' ── 7) ปุ่มล่าง: ยกเลิก (ซ้าย) + บันทึก (ขวา) ขนาดเท่ากัน อยู่ใน layout ไม่ใช่ลอย ──
            Dim pnlButtons As New TableLayoutPanel()
            pnlButtons.Dock = DockStyle.Fill
            pnlButtons.AutoSize = True
            pnlButtons.ColumnCount = 2
            pnlButtons.RowCount = 1
            pnlButtons.Margin = New Padding(0, K(6), 0, K(8))   ' เว้นจากขอบล่าง/แถบ F1 ≥ 8px
            pnlButtons.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
            pnlButtons.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
            pnlButtons.RowStyles.Add(New RowStyle(SizeType.AutoSize))

            btnCancel = MakeButton("ยกเลิก", 0, 0, 150, 40, Color.FromArgb(146, 64, 14))
            btnCancel.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            btnCancel.Margin = New Padding(0, K(2), K(8), 0)
            pnlButtons.Controls.Add(btnCancel, 0, 0)

            btnSave = MakeButton("💾 บันทึกการนับ", 0, 0, 150, 40, Color.FromArgb(5, 150, 105))
            btnSave.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            btnSave.Margin = New Padding(K(8), K(2), 0, 0)
            pnlButtons.Controls.Add(btnSave, 1, 0)
            root.Controls.Add(pnlButtons, 0, 6)

            AddHandler dtpDate.ValueChanged, AddressOf DtpDate_ValueChanged
            AddHandler btnSave.Click, AddressOf BtnSave_Click
            AddHandler btnCancel.Click, AddressOf BtnCancel_Click
        End Sub

        Private Function MakeButton(text As String, x As Integer, y As Integer, w As Integer, h As Integer, color As Color) As Button
            Dim b As New Button()
            b.Text = text
            b.Location = New Point(x, y)
            b.Size = New Size(K(w), K(h))
            b.FlatStyle = FlatStyle.Flat
            b.FlatAppearance.BorderSize = 0
            b.BackColor = color
            b.ForeColor = Color.White
            b.Font = New Font("Tahoma", 10.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            b.Cursor = Cursors.Hand
            Return b
        End Function

        Private Sub DenomBox_TextChanged(sender As Object, e As EventArgs)
            UpdateSummary()
        End Sub

        Private Sub DtpDate_ValueChanged(sender As Object, e As EventArgs)
            RecalcExpected()
        End Sub

        Private Function RecalcTotal() As Decimal
            Dim total As Decimal = 0D
            For Each txt In _denomBoxes
                Dim v As Decimal = 0D
                Decimal.TryParse(txt.Text.Trim(), v)
                If v < 0 Then v = 0D
                total += v
            Next
            Return total
        End Function

        Private Sub RecalcExpected()
            Dim v = CashOnHandRepository.ComputeCashOnHand(dtpDate.Value.Date)
            _expected = If(v.HasValue, v.Value, 0D)
            UpdateSummary()
        End Sub

        Private Sub UpdateSummary()
            Dim total As Decimal = RecalcTotal()
            Dim variance As Decimal = total - _expected
            lblTotal.Text = total.ToString("#,##0.00")
            lblExpected.Text = _expected.ToString("#,##0.00")
            lblVariance.Text = variance.ToString("+#,##0.00;-#,##0.00;0.00")
            If variance >= 0 Then
                lblVariance.ForeColor = Color.FromArgb(22, 101, 52)
            Else
                lblVariance.ForeColor = Color.FromArgb(153, 27, 27)
            End If
        End Sub

        Private Sub LoadHistory()
            Try
                Dim dt As New DataTable()
                dt.Columns.Add("วันที่", GetType(String))
                dt.Columns.Add("นับได้ (บาท)", GetType(String))
                dt.Columns.Add("คาดการณ์", GetType(String))
                dt.Columns.Add("ผลต่าง", GetType(String))
                dt.Columns.Add("หมายเหตุ", GetType(String))
                For Each c In CashOnHandRepository.LoadCounts()
                    Dim d = CashOnHandService.NormalizeDate(c.CountDate)
                    dt.Rows.Add(
                        d.ToString("dd/MM/yyyy"),
                        c.TotalCounted.ToString("#,##0.00"),
                        c.ExpectedBefore.ToString("#,##0.00"),
                        c.Variance.ToString("+#,##0.00;-#,##0.00;0.00"),
                        If(String.IsNullOrWhiteSpace(c.Note), "", c.Note))
                Next
                dgvHistory.DataSource = dt
                If dt.Columns.Count > 0 Then
                    dgvHistory.Columns(0).FillWeight = 30
                    dgvHistory.Columns(1).FillWeight = 25
                    dgvHistory.Columns(2).FillWeight = 25
                    dgvHistory.Columns(3).FillWeight = 20
                    dgvHistory.Columns(4).FillWeight = 40

                    ' วันที่/หมายเหตุชิดซ้าย, คอลัมน์ตัวเลขชิดขวา (ค่า Format N2 มาจากด้านบนแล้ว)
                    dgvHistory.Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                    For c As Integer = 1 To 3
                        dgvHistory.Columns(c).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    Next
                    dgvHistory.Columns(4).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    dgvHistory.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                End If
            Catch ex As Exception
                ErrorLogHelper.LogSilentError("FrmCashCount.LoadHistory", ex)
            End Try
        End Sub

        Private Sub BtnSave_Click(sender As Object, e As EventArgs)
            Dim total As Decimal = RecalcTotal()
            If total < 0 Then
                MessageBox.Show("กรุณากรอกจำนวนเงินให้ถูกต้อง", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim denominations(8) As Decimal
            For i As Integer = 0 To 8
                Dim v As Decimal = 0D
                Decimal.TryParse(_denomBoxes(i).Text.Trim(), v)
                If v < 0 Then v = 0D
                denominations(i) = v
            Next

            Dim d = dtpDate.Value.Date
            Dim confirm = MessageBox.Show(
                "ยืนยันบันทึกการนับเงินสด" & vbCrLf &
                "วันที่ " & d.ToString("dd/MM/yyyy") & "  รวม " & total.ToString("#,##0.00") & " บาท" & vbCrLf & vbCrLf &
                "ยอดคาดการณ์ " & _expected.ToString("#,##0.00") & " บาท  ผลต่าง " &
                (total - _expected).ToString("+#,##0.00;-#,##0.00;0.00") & " บาท",
                "ยืนยันการนับเงินสด",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If confirm <> DialogResult.Yes Then Return

            Try
                Db.EnsureSchema()
                Dim saved = CashOnHandRepository.SaveCount(d, denominations, txtNote.Text.Trim())
                MessageBox.Show("✅ บันทึกการนับเงินสดสำเร็จ" & vbCrLf &
                                "เงินสดในมือ ณ " & d.ToString("dd/MM/yyyy") & " = " & saved.TotalCounted.ToString("#,##0.00") & " บาท",
                                "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Catch ex As Exception
                MessageBox.Show("ผิดพลาด: " & ex.Message, "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub BtnCancel_Click(sender As Object, e As EventArgs)
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Sub
    End Class
End Namespace
