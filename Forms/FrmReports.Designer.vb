Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmReports
        Inherits Form

        Private components As IContainer = Nothing
        Friend WithEvents lblHeader As Label
        Friend WithEvents dtpFrom As DateTimePicker
        Friend WithEvents dtpTo As DateTimePicker
        Friend WithEvents cboType As ComboBox
        Friend WithEvents cboFund As ComboBox
        Friend WithEvents cboBank As ComboBox
        Friend WithEvents lbl1 As Label
        Friend WithEvents lbl2 As Label
        Friend WithEvents lbl3 As Label
        Friend WithEvents lbl4 As Label
        Friend WithEvents lbl5 As Label
        Friend WithEvents btnSummaryIncome As Button
        Friend WithEvents btnSummaryExpense As Button
        Friend WithEvents btnMonthly As Button
        Friend WithEvents btnLedger As Button
        Friend WithEvents btnPrint As Button
        Friend WithEvents btnRefresh As Button
        Friend WithEvents btnPrintDetail As Button
        Friend WithEvents btnPrintSummary As Button
        Friend WithEvents dgvReport As DataGridView
        Friend WithEvents lblSummary As Label
        Friend WithEvents lblBalance As Label
        Friend WithEvents txtBalance As TextBox
        Friend WithEvents btnCalcBalance As Button

        <DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Text = "รายงาน"
            BackColor = Color.FromArgb(254, 249, 235)
            Font = New Font("Tahoma", 10.5!)
            ClientSize = New Size(1500, 860)
            MinimumSize = New Size(1280, 760)
            StartPosition = FormStartPosition.CenterScreen
            WindowState = FormWindowState.Maximized
            FormBorderStyle = FormBorderStyle.Sizable
            AutoScroll = True

            lblHeader = New Label()
            lblHeader.Text = "📊 ศูนย์รายงาน (วางแผน RDLC ในรุ่นถัดไป - ตอนนี้ Preview ก่อนพิมพ์ได้)"
            lblHeader.Font = New Font("Tahoma", 13.5!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(24, 83, 63)
            lblHeader.BackColor = Color.FromArgb(167, 243, 208)
            lblHeader.Dock = DockStyle.Top
            lblHeader.Height = 62
            lblHeader.TextAlign = ContentAlignment.MiddleCenter

            Dim p = New Panel With {.BackColor = Color.White, .Dock = DockStyle.Top, .Height = 120, .Padding = New Padding(16)}
            lbl1 = MakeLbl("จากวันที่:", New Point(16, 16))
            dtpFrom = New DateTimePicker With {.Location = New Point(130, 12), .Size = New Size(200, 40), .Font = New Font("Tahoma", 10.0!), .Value = New Date(Today.Year, Today.Month, 1)}
            lbl2 = MakeLbl("ถึงวันที่:", New Point(350, 16))
            dtpTo = New DateTimePicker With {.Location = New Point(460, 12), .Size = New Size(200, 40), .Font = New Font("Tahoma", 10.0!), .Value = Today}
            lbl3 = MakeLbl("ประเภท:", New Point(680, 16))
            cboType = New ComboBox With {.Location = New Point(760, 12), .Size = New Size(200, 40), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.0!)}
            cboType.Items.AddRange({"ทั้งหมด", "รายรับ", "รายจ่าย", "โอนภายใน"})
            cboType.SelectedIndex = 0
            lbl4 = MakeLbl("กองทุน:", New Point(16, 58))
            cboFund = New ComboBox With {.Location = New Point(130, 54), .Size = New Size(260, 40), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.0!)}
            lbl5 = MakeLbl("ธนาคาร:", New Point(410, 58))
            cboBank = New ComboBox With {.Location = New Point(510, 54), .Size = New Size(260, 40), .DropDownStyle = ComboBoxStyle.DropDownList, .Font = New Font("Tahoma", 10.0!)}
            lblBalance = MakeLbl("ยอดยกมา:", New Point(790, 58))
            txtBalance = New TextBox With {.Location = New Point(890, 54), .Size = New Size(100, 40), .Font = New Font("Tahoma", 10.0!), .Text = "0.00", .TextAlign = HorizontalAlignment.Right}
            btnCalcBalance = New Button With {.Text = "🧮", .Location = New Point(995, 54), .Size = New Size(40, 40), .BackColor = Color.FromArgb(107, 114, 128), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand, .ToolTipText = "คำนวณยอดยกมาอัตโนมัติ"}
            btnRefresh = New Button With {.Text = "🔍 ดูรายงาน", .Location = New Point(1040, 54), .Size = New Size(170, 44), .BackColor = Color.FromArgb(37, 99, 235), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 10.0!, FontStyle.Bold), .Cursor = Cursors.Hand}
            p.Controls.AddRange(New Control() {lbl1, dtpFrom, lbl2, dtpTo, lbl3, cboType, lbl4, cboFund, lbl5, cboBank, lblBalance, txtBalance, btnCalcBalance, btnRefresh})

            Dim pa = New Panel With {.Dock = DockStyle.Top, .Height = 76, .BackColor = Color.FromArgb(245, 240, 220), .Padding = New Padding(14, 14, 14, 14)}
            btnPrintDetail = New Button With {.Text = "📜 พิมพ์รายงานละเอียด", .Dock = DockStyle.Left, .Size = New Size(240, 48), .BackColor = Color.FromArgb(185, 28, 28), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnPrintSummary = New Button With {.Text = "🧾 พิมพ์รายงานย่อ", .Dock = DockStyle.Left, .Size = New Size(220, 48), .BackColor = Color.FromArgb(146, 64, 14), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnSummaryIncome = New Button With {.Text = "💵 สรุปรายรับแยกประเภท", .Dock = DockStyle.Left, .Size = New Size(240, 48), .BackColor = Color.FromArgb(22, 163, 74), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnSummaryExpense = New Button With {.Text = "💸 สรุปรายจ่ายแยกประเภท", .Dock = DockStyle.Left, .Size = New Size(240, 48), .BackColor = Color.FromArgb(190, 18, 60), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnMonthly = New Button With {.Text = "📈 รายงานรายเดือน", .Dock = DockStyle.Left, .Size = New Size(220, 48), .BackColor = Color.FromArgb(126, 34, 206), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnLedger = New Button With {.Text = "📒 สมุดรายวัน (แสดงทั้งหมด)", .Dock = DockStyle.Left, .Size = New Size(260, 48), .BackColor = Color.FromArgb(180, 83, 9), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            btnPrint = New Button With {.Text = "🖨️ ส่งไป Excel", .Dock = DockStyle.Right, .Size = New Size(180, 48), .BackColor = Color.FromArgb(30, 64, 175), .ForeColor = Color.White, .FlatStyle = FlatStyle.Flat, .Font = New Font("Tahoma", 9.5!, FontStyle.Bold), .Cursor = Cursors.Hand}
            pa.Controls.AddRange(New Control() {btnPrint, btnLedger, btnMonthly, btnSummaryExpense, btnSummaryIncome, btnPrintSummary, btnPrintDetail})

            lblSummary = New Label With {.Dock = DockStyle.Top, .Height = 50, .BackColor = Color.FromArgb(253, 224, 71), .Font = New Font("Tahoma", 11.0!, FontStyle.Bold), .ForeColor = Color.FromArgb(69, 26, 3), .TextAlign = ContentAlignment.MiddleCenter, .Text = "รายรับรวม 0.00  |  รายจ่ายรวม 0.00  |  ส่วนเกิน 0.00"}

            dgvReport = New DataGridView With {.Dock = DockStyle.Fill, .BackgroundColor = Color.White, .ReadOnly = True, .AllowUserToAddRows = False, .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, .SelectionMode = DataGridViewSelectionMode.FullRowSelect, .BorderStyle = BorderStyle.None, .Font = New Font("Tahoma", 9.5!), .RowTemplate = New DataGridViewRow() With {.Height = 30}}
            dgvReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235)

            Controls.Add(dgvReport)
            Controls.Add(lblSummary)
            Controls.Add(pa)
            Controls.Add(p)
            Controls.Add(lblHeader)
        End Sub
    End Class
End Namespace

