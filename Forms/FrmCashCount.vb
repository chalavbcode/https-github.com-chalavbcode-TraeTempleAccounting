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

        Private Shared ReadOnly DenomLabels As String() = {
            "เหรียญ 1 บาท", "เหรียญ 2 บาท", "เหรียญ 5 บาท", "เหรียญ 10 บาท",
            "ธนบัตร 20 บาท", "ธนบัตร 50 บาท", "ธนบัตร 100 บาท", "ธนบัตร 500 บาท", "ธนบัตร 1,000 บาท"}

        Public Sub New()
            InitializeUI()
            LoadHistory()
            RecalcExpected()
        End Sub

        Private Sub InitializeUI()
            Me.Text = "นับเงินสด"
            Me.StartPosition = FormStartPosition.CenterParent
            Me.Size = New Size(660, 760)
            Me.MinimumSize = New Size(620, 640)
            Me.BackColor = Color.FromArgb(255, 253, 244)
            Me.Font = New Font("Tahoma", 10.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            Me.FormBorderStyle = FormBorderStyle.Sizable

            ' ── แถบบน: หัวข้อ + คำอธิบาย ──
            Dim pnlTop As New Panel()
            pnlTop.Dock = DockStyle.Top
            pnlTop.Height = 86
            pnlTop.Padding = New Padding(16, 12, 16, 8)

            Dim lblTitle As New Label()
            lblTitle.AutoSize = True
            lblTitle.Font = New Font("Tahoma", 15.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblTitle.ForeColor = Color.FromArgb(120, 53, 15)
            lblTitle.Text = "💰 นับเงินสด"
            lblTitle.Location = New Point(16, 10)
            pnlTop.Controls.Add(lblTitle)

            Dim lblHint As New Label()
            lblHint.AutoSize = True
            lblHint.Font = New Font("Tahoma", 9.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
            lblHint.ForeColor = Color.FromArgb(120, 80, 40)
            lblHint.Text = "กรอกมูลค่า (บาท) ของเงินสดแต่ละชนิด ระบบจะรวมและเทียบกับยอดคาดการณ์ให้อัตโนมัติ"
            lblHint.Location = New Point(16, 38)
            pnlTop.Controls.Add(lblHint)

            ' ── แถบวันที่ ──
            Dim pnlDate As New Panel()
            pnlDate.Dock = DockStyle.Top
            pnlDate.Height = 44
            pnlDate.Padding = New Padding(16, 6, 16, 4)

            Dim lblDate As New Label()
            lblDate.Text = "วันที่นับ"
            lblDate.Location = New Point(16, 12)
            lblDate.AutoSize = True
            lblDate.ForeColor = Color.FromArgb(120, 53, 15)
            pnlDate.Controls.Add(lblDate)

            dtpDate = New DateTimePicker()
            dtpDate.Format = DateTimePickerFormat.Short
            dtpDate.Value = Date.Today
            dtpDate.Location = New Point(110, 10)
            dtpDate.Size = New Size(160, 28)
            pnlDate.Controls.Add(dtpDate)

            ' ── ช่องกรอกแยกชนิด (TableLayoutPanel 3x3) ──
            Dim pnlDenom As New Panel()
            pnlDenom.Dock = DockStyle.Top
            pnlDenom.Height = 190
            pnlDenom.Padding = New Padding(16, 4, 16, 4)

            Dim lblDenomHint As New Label()
            lblDenomHint.Text = "มูลค่าเงินสดแยกตามชนิด (บาท)"
            lblDenomHint.Font = New Font("Tahoma", 9.5!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblDenomHint.ForeColor = Color.FromArgb(120, 80, 40)
            lblDenomHint.Dock = DockStyle.Top
            lblDenomHint.Height = 22
            pnlDenom.Controls.Add(lblDenomHint)

            Dim tbl As New TableLayoutPanel()
            tbl.Dock = DockStyle.Fill
            tbl.ColumnCount = 3
            tbl.RowCount = 3
            tbl.Padding = New Padding(0)
            For i As Integer = 0 To 2
                tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))
                tbl.RowStyles.Add(New RowStyle(SizeType.Percent, 33.33F))
            Next
            pnlDenom.Controls.Add(tbl)

            ReDim _denomBoxes(8)
            For i As Integer = 0 To 8
                Dim cell As New Panel()
                cell.Dock = DockStyle.Fill
                cell.Margin = New Padding(4)
                cell.Padding = New Padding(4)

                Dim lbl As New Label()
                lbl.Text = DenomLabels(i)
                lbl.Dock = DockStyle.Top
                lbl.Height = 20
                lbl.Font = New Font("Tahoma", 9.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
                lbl.ForeColor = Color.FromArgb(120, 80, 40)
                cell.Controls.Add(lbl)

                Dim txt As New TextBox()
                txt.Text = "0"
                txt.Dock = DockStyle.Top
                txt.Height = 26
                txt.TextAlign = HorizontalAlignment.Right
                txt.Font = New Font("Tahoma", 10.0!, FontStyle.Regular, GraphicsUnit.Point, 222)
                cell.Controls.Add(txt)

                _denomBoxes(i) = txt
                AddHandler txt.TextChanged, AddressOf DenomBox_TextChanged

                tbl.Controls.Add(cell, i Mod 3, i \ 3)
            Next

            ' ── สรุปยอด + คาดการณ์ + ผลต่าง ──
            Dim pnlSummary As New Panel()
            pnlSummary.Dock = DockStyle.Top
            pnlSummary.Height = 96
            pnlSummary.Padding = New Padding(16, 4, 16, 4)

            Dim lblTotalCaption As New Label()
            lblTotalCaption.Text = "รวมเงินสดที่นับได้ (บาท)"
            lblTotalCaption.Location = New Point(16, 8)
            lblTotalCaption.AutoSize = True
            lblTotalCaption.ForeColor = Color.FromArgb(120, 53, 15)
            pnlSummary.Controls.Add(lblTotalCaption)

            lblTotal = New Label()
            lblTotal.Text = "0.00"
            lblTotal.Location = New Point(16, 30)
            lblTotal.Size = New Size(300, 36)
            lblTotal.Font = New Font("Tahoma", 17.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblTotal.ForeColor = Color.FromArgb(22, 101, 52)
            pnlSummary.Controls.Add(lblTotal)

            Dim lblExpCaption As New Label()
            lblExpCaption.Text = "ยอดคาดการณ์ (จากธุรกรรม)"
            lblExpCaption.Location = New Point(360, 8)
            lblExpCaption.AutoSize = True
            lblExpCaption.ForeColor = Color.FromArgb(120, 80, 40)
            pnlSummary.Controls.Add(lblExpCaption)

            lblExpected = New Label()
            lblExpected.Text = "0.00"
            lblExpected.Location = New Point(360, 30)
            lblExpected.Size = New Size(140, 24)
            lblExpected.Font = New Font("Tahoma", 10.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblExpected.ForeColor = Color.FromArgb(30, 64, 175)
            pnlSummary.Controls.Add(lblExpected)

            Dim lblVarCaption As New Label()
            lblVarCaption.Text = "ผลต่าง"
            lblVarCaption.Location = New Point(360, 58)
            lblVarCaption.AutoSize = True
            lblVarCaption.ForeColor = Color.FromArgb(120, 80, 40)
            pnlSummary.Controls.Add(lblVarCaption)

            lblVariance = New Label()
            lblVariance.Text = "0.00"
            lblVariance.Location = New Point(360, 72)
            lblVariance.Size = New Size(140, 20)
            lblVariance.Font = New Font("Tahoma", 10.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            pnlSummary.Controls.Add(lblVariance)

            ' ── หมายเหตุ ──
            Dim pnlNote As New Panel()
            pnlNote.Dock = DockStyle.Top
            pnlNote.Height = 56
            pnlNote.Padding = New Padding(16, 4, 16, 4)

            Dim lblNote As New Label()
            lblNote.Text = "หมายเหตุ"
            lblNote.Location = New Point(16, 6)
            lblNote.AutoSize = True
            lblNote.ForeColor = Color.FromArgb(120, 53, 15)
            pnlNote.Controls.Add(lblNote)

            txtNote = New TextBox()
            txtNote.Location = New Point(110, 4)
            txtNote.Size = New Size(520, 28)
            pnlNote.Controls.Add(txtNote)

            ' ── ตารางการนับก่อนหน้า ──
            Dim lblHistory As New Label()
            lblHistory.Dock = DockStyle.Top
            lblHistory.Height = 28
            lblHistory.Padding = New Padding(16, 4, 0, 0)
            lblHistory.Text = "📋 การนับก่อนหน้า"
            lblHistory.Font = New Font("Tahoma", 9.5!, FontStyle.Bold, GraphicsUnit.Point, 222)
            lblHistory.ForeColor = Color.FromArgb(120, 80, 40)

            dgvHistory = New DataGridView()
            dgvHistory.Dock = DockStyle.Fill
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
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 240, 210)
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(120, 53, 15)
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 9.0!, FontStyle.Bold, GraphicsUnit.Point, 222)
            dgvHistory.DefaultCellStyle.Font = New Font("Tahoma", 9.0!, FontStyle.Regular, GraphicsUnit.Point, 222)

            ' ── ปุ่มล่าง ──
            Dim pnlButtons As New Panel()
            pnlButtons.Dock = DockStyle.Bottom
            pnlButtons.Height = 56
            pnlButtons.Padding = New Padding(16, 8, 16, 8)

            btnCancel = MakeButton("ยกเลิก", 16, 8, 110, 40, Color.FromArgb(146, 64, 14))
            pnlButtons.Controls.Add(btnCancel)

            btnSave = MakeButton("💾 บันทึกการนับ", 0, 8, 160, 40, Color.FromArgb(5, 150, 105))
            btnSave.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            btnSave.Location = New Point(Me.ClientSize.Width - 176, 8)
            pnlButtons.Controls.Add(btnSave)

            ' ── ประกอบ layout (Dock ล่างต้องเพิ่มก่อน Fill) ──
            Me.Controls.Add(dgvHistory)
            Me.Controls.Add(lblHistory)
            Me.Controls.Add(pnlButtons)
            Me.Controls.Add(pnlNote)
            Me.Controls.Add(pnlSummary)
            Me.Controls.Add(pnlDenom)
            Me.Controls.Add(pnlDate)
            Me.Controls.Add(pnlTop)

            AddHandler dtpDate.ValueChanged, AddressOf DtpDate_ValueChanged
            AddHandler btnSave.Click, AddressOf BtnSave_Click
            AddHandler btnCancel.Click, AddressOf BtnCancel_Click
        End Sub

        Private Function MakeButton(text As String, x As Integer, y As Integer, w As Integer, h As Integer, color As Color) As Button
            Dim b As New Button()
            b.Text = text
            b.Location = New Point(x, y)
            b.Size = New Size(w, h)
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
