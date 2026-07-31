Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmIncome
        Inherits Form

        Private components As IContainer = Nothing
        Friend WithEvents dtpDate As DateTimePicker
        Friend WithEvents cboCategory As ComboBox
        Friend WithEvents cboFund As ComboBox
        Friend WithEvents cboBank As ComboBox
        Friend WithEvents txtDescription As TextBox
        Friend WithEvents txtAmount As TextBox
        Friend WithEvents txtRemark As TextBox
        Friend WithEvents btnSave As Button
        Friend WithEvents btnCancel As Button
        Friend WithEvents btnImportExcel As Button
        Friend WithEvents ttMain As ToolTip
        Friend WithEvents lblHeader As Label
        Friend WithEvents lbl1 As Label
        Friend WithEvents lbl2 As Label
        Friend WithEvents lbl3 As Label
        Friend WithEvents lbl4 As Label
        Friend WithEvents lbl5 As Label
        Friend WithEvents lbl6 As Label
        Friend WithEvents lbl7 As Label

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
            Me.components = New Container()
            Me.ttMain = New ToolTip(Me.components)
            lblHeader = New Label()
            lbl1 = New Label()
            lbl2 = New Label()
            lbl3 = New Label()
            lbl4 = New Label()
            lbl5 = New Label()
            lbl6 = New Label()
            lbl7 = New Label()
            dtpDate = New DateTimePicker()
            cboCategory = New ComboBox()
            cboFund = New ComboBox()
            cboBank = New ComboBox()
            txtDescription = New TextBox()
            txtAmount = New TextBox()
            txtRemark = New TextBox()
            btnSave = New Button()
            btnCancel = New Button()
            btnImportExcel = New Button()
            SuspendLayout()
            '
            ' FrmIncome
            '
            Text = "บันทึกรายรับ"
            BackColor = Color.FromArgb(CByte(254), CByte(249), CByte(235))
            Font = New Font("Tahoma", 10.5!)
            ClientSize = New Size(1280, 820)
            MinimumSize = New Size(1100, 720)
            StartPosition = FormStartPosition.CenterScreen
            WindowState = FormWindowState.Maximized
            FormBorderStyle = FormBorderStyle.Sizable
            AutoScroll = True
            '
            ' lblHeader
            '
            lblHeader.Text = "💰 บันทึกรายรับเงินเข้าวัด"
            lblHeader.Font = New Font("Tahoma", 15.0!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(120, 53, 15)
            lblHeader.BackColor = Color.FromArgb(253, 230, 138)
            lblHeader.Dock = DockStyle.Top
            lblHeader.Height = 70
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            lblHeader.Padding = New Padding(0, 8, 0, 8)
            '
            ' lbl1
            '
            lbl1.Text = "วันที่ทำรายการ:"
            lbl1.Location = New Point(40, 110)
            lbl1.Size = New Size(190, 40)
            lbl1.TextAlign = ContentAlignment.MiddleRight
            '
            ' dtpDate
            '
            dtpDate.Location = New Point(240, 110)
            dtpDate.Size = New Size(520, 40)
            dtpDate.Font = New Font("Tahoma", 10.5!)
            dtpDate.Value = Today
            '
            ' lbl2
            '
            lbl2.Text = "ประเภทรายรับ:"
            lbl2.Location = New Point(40, 172)
            lbl2.Size = New Size(190, 40)
            lbl2.TextAlign = ContentAlignment.MiddleRight
            '
            ' cboCategory
            '
            cboCategory.Location = New Point(240, 172)
            cboCategory.Size = New Size(520, 40)
            cboCategory.Font = New Font("Tahoma", 10.5!)
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList
            '
            ' lbl3
            '
            lbl3.Text = "กองทุน:"
            lbl3.Location = New Point(40, 234)
            lbl3.Size = New Size(190, 40)
            lbl3.TextAlign = ContentAlignment.MiddleRight
            '
            ' cboFund
            '
            cboFund.Location = New Point(240, 234)
            cboFund.Size = New Size(520, 40)
            cboFund.Font = New Font("Tahoma", 10.5!)
            cboFund.DropDownStyle = ComboBoxStyle.DropDownList
            '
            ' lbl4
            '
            lbl4.Text = "บัญชีธนาคาร (ถ้ามี):"
            lbl4.Location = New Point(40, 296)
            lbl4.Size = New Size(190, 40)
            lbl4.TextAlign = ContentAlignment.MiddleRight
            '
            ' cboBank
            '
            cboBank.Location = New Point(240, 296)
            cboBank.Size = New Size(520, 40)
            cboBank.Font = New Font("Tahoma", 10.5!)
            cboBank.DropDownStyle = ComboBoxStyle.DropDownList
            '
            ' lbl5
            '
            lbl5.Text = "รายละเอียดรายการ:"
            lbl5.Location = New Point(40, 358)
            lbl5.Size = New Size(190, 40)
            lbl5.TextAlign = ContentAlignment.MiddleRight
            '
            ' txtDescription
            '
            txtDescription.Location = New Point(240, 358)
            txtDescription.Size = New Size(520, 40)
            txtDescription.Font = New Font("Tahoma", 10.5!)
            '
            ' lbl6
            '
            lbl6.Text = "จำนวนเงิน (บาท):"
            lbl6.Location = New Point(40, 420)
            lbl6.Size = New Size(190, 40)
            lbl6.TextAlign = ContentAlignment.MiddleRight
            '
            ' txtAmount
            '
            txtAmount.Location = New Point(240, 420)
            txtAmount.Size = New Size(260, 40)
            txtAmount.Font = New Font("Tahoma", 11.5!, FontStyle.Bold)
            txtAmount.TextAlign = HorizontalAlignment.Right
            txtAmount.ForeColor = Color.FromArgb(22, 101, 52)
            '
            ' lbl7
            '
            lbl7.Text = "หมายเหตุ:"
            lbl7.Location = New Point(40, 482)
            lbl7.Size = New Size(190, 40)
            lbl7.TextAlign = ContentAlignment.MiddleRight
            '
            ' txtRemark
            '
            txtRemark.Location = New Point(240, 482)
            txtRemark.Size = New Size(520, 100)
            txtRemark.Font = New Font("Tahoma", 10.5!)
            txtRemark.Multiline = True
            txtRemark.ScrollBars = ScrollBars.Vertical
            '
            ' btnSave
            '
            btnSave.Text = "💾 บันทึกรายการ"
            btnSave.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            btnSave.BackColor = Color.FromArgb(22, 163, 74)
            btnSave.ForeColor = Color.White
            btnSave.FlatStyle = FlatStyle.Flat
            btnSave.Size = New Size(240, 56)
            btnSave.Location = New Point(240, 602)
            btnSave.Cursor = Cursors.Hand
            '
            ' btnCancel
            '
            btnCancel.Text = "❌ เคลียร์"
            btnCancel.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            btnCancel.BackColor = Color.FromArgb(180, 83, 9)
            btnCancel.ForeColor = Color.White
            btnCancel.FlatStyle = FlatStyle.Flat
            btnCancel.Size = New Size(180, 56)
            btnCancel.Location = New Point(500, 602)
            btnCancel.Cursor = Cursors.Hand
            '
            ' btnImportExcel
            '
            btnImportExcel.Text = "📥 นำเข้าจาก Excel"
            btnImportExcel.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            btnImportExcel.BackColor = Color.FromArgb(37, 99, 235)
            btnImportExcel.ForeColor = Color.White
            btnImportExcel.FlatStyle = FlatStyle.Flat
            btnImportExcel.Size = New Size(240, 56)
            btnImportExcel.Location = New Point(700, 602)
            btnImportExcel.Cursor = Cursors.Hand
            '
            ' Controls
            '
            Controls.Add(lbl1)
            Controls.Add(lbl2)
            Controls.Add(lbl3)
            Controls.Add(lbl4)
            Controls.Add(lbl5)
            Controls.Add(lbl6)
            Controls.Add(lbl7)
            Controls.Add(dtpDate)
            Controls.Add(cboCategory)
            Controls.Add(cboFund)
            Controls.Add(cboBank)
            Controls.Add(txtDescription)
            Controls.Add(txtAmount)
            Controls.Add(txtRemark)
            Controls.Add(btnSave)
            Controls.Add(btnCancel)
            Controls.Add(btnImportExcel)
            Controls.Add(lblHeader)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace

