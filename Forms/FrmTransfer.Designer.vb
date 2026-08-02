Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmTransfer
        Inherits Form

        Private components As IContainer = Nothing
        Friend WithEvents dtpDate As DateTimePicker
        Friend WithEvents cboFromFund As ComboBox
        Friend WithEvents cboFromBank As ComboBox
        Friend WithEvents cboToFund As ComboBox
        Friend WithEvents cboToBank As ComboBox
        Friend WithEvents txtAmount As TextBox
        Friend WithEvents txtRemark As TextBox
        Friend WithEvents btnSave As Button
        Friend WithEvents btnCancel As Button
        Friend WithEvents ttMain As ToolTip
        Friend WithEvents lblHeader As Label
        Friend WithEvents l1 As Label
        Friend WithEvents l2 As Label
        Friend WithEvents l3 As Label
        Friend WithEvents l4 As Label
        Friend WithEvents l5 As Label
        Friend WithEvents l6 As Label
        Friend WithEvents lr As Label

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
            dtpDate = New DateTimePicker()
            cboFromFund = New ComboBox()
            cboFromBank = New ComboBox()
            cboToFund = New ComboBox()
            cboToBank = New ComboBox()
            txtAmount = New TextBox()
            txtRemark = New TextBox()
            btnSave = New Button()
            btnCancel = New Button()
            l1 = New Label()
            l2 = New Label()
            l3 = New Label()
            l4 = New Label()
            l5 = New Label()
            l6 = New Label()
            lr = New Label()
            SuspendLayout()
            '
            ' FrmTransfer
            '
            Text = "โอนเงินภายใน"
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
            lblHeader.Text = "🔁 โอนเงินภายในระหว่างกองทุน/บัญชี"
            lblHeader.Font = New Font("Tahoma", 14.0!, FontStyle.Bold)
            lblHeader.ForeColor = Color.FromArgb(88, 28, 135)
            lblHeader.BackColor = Color.FromArgb(233, 213, 255)
            lblHeader.Dock = DockStyle.Top
            lblHeader.Height = 70
            lblHeader.TextAlign = ContentAlignment.MiddleCenter
            '
            ' dtpDate
            '
            dtpDate.Font = New Font("Tahoma", 10.5!)
            dtpDate.Value = Today
            dtpDate.Location = New Point(260, 110)
            dtpDate.Size = New Size(520, 40)
            '
            ' cboFromFund
            '
            cboFromFund.DropDownStyle = ComboBoxStyle.DropDownList
            cboFromFund.Font = New Font("Tahoma", 10.5!)
            cboFromFund.Location = New Point(260, 172)
            cboFromFund.Size = New Size(520, 40)
            '
            ' cboFromBank
            '
            cboFromBank.DropDownStyle = ComboBoxStyle.DropDownList
            cboFromBank.Font = New Font("Tahoma", 10.5!)
            cboFromBank.Location = New Point(260, 234)
            cboFromBank.Size = New Size(520, 40)
            '
            ' cboToFund
            '
            cboToFund.DropDownStyle = ComboBoxStyle.DropDownList
            cboToFund.Font = New Font("Tahoma", 10.5!)
            cboToFund.Location = New Point(260, 296)
            cboToFund.Size = New Size(520, 40)
            '
            ' cboToBank
            '
            cboToBank.DropDownStyle = ComboBoxStyle.DropDownList
            cboToBank.Font = New Font("Tahoma", 10.5!)
            cboToBank.Location = New Point(260, 358)
            cboToBank.Size = New Size(520, 40)
            '
            ' txtAmount
            '
            txtAmount.Font = New Font("Tahoma", 11.5!, FontStyle.Bold)
            txtAmount.ForeColor = Color.FromArgb(88, 28, 135)
            txtAmount.TextAlign = HorizontalAlignment.Right
            txtAmount.Location = New Point(260, 420)
            txtAmount.Size = New Size(260, 40)
            '
            ' txtRemark
            '
            txtRemark.Font = New Font("Tahoma", 10.5!)
            txtRemark.Multiline = True
            txtRemark.ScrollBars = ScrollBars.Vertical
            txtRemark.Location = New Point(260, 482)
            txtRemark.Size = New Size(520, 100)
            '
            ' btnSave
            '
            btnSave.Text = "💾 บันทึกการโอน"
            btnSave.ForeColor = Color.White
            btnSave.BackColor = Color.FromArgb(126, 34, 206)
            btnSave.FlatStyle = FlatStyle.Flat
            btnSave.Size = New Size(260, 56)
            btnSave.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            btnSave.Cursor = Cursors.Hand
            btnSave.Location = New Point(260, 612)
            '
            ' btnCancel
            '
            btnCancel.Text = "❌ เคลียร์"
            btnCancel.ForeColor = Color.White
            btnCancel.BackColor = Color.FromArgb(75, 85, 99)
            btnCancel.FlatStyle = FlatStyle.Flat
            btnCancel.Size = New Size(180, 56)
            btnCancel.Font = New Font("Tahoma", 11.0!, FontStyle.Bold)
            btnCancel.Cursor = Cursors.Hand
            btnCancel.Location = New Point(540, 612)
            '
            ' labels
            '
            l1.Text = "วันที่โอน:"
            l1.Location = New Point(40, 110)
            l1.Size = New Size(210, 40)
            l1.TextAlign = ContentAlignment.MiddleRight
            l1.Font = New Font("Tahoma", 10.5!)

            l2.Text = "จากกองทุน:"
            l2.Location = New Point(40, 172)
            l2.Size = New Size(210, 40)
            l2.TextAlign = ContentAlignment.MiddleRight
            l2.Font = New Font("Tahoma", 10.5!)

            l3.Text = "จากบัญชีธนาคาร:"
            l3.Location = New Point(40, 234)
            l3.Size = New Size(210, 40)
            l3.TextAlign = ContentAlignment.MiddleRight
            l3.Font = New Font("Tahoma", 10.5!)

            l4.Text = "ไปยังกองทุน:"
            l4.Location = New Point(40, 296)
            l4.Size = New Size(210, 40)
            l4.TextAlign = ContentAlignment.MiddleRight
            l4.Font = New Font("Tahoma", 10.5!)

            l5.Text = "ไปยังบัญชี:"
            l5.Location = New Point(40, 358)
            l5.Size = New Size(210, 40)
            l5.TextAlign = ContentAlignment.MiddleRight
            l5.Font = New Font("Tahoma", 10.5!)

            l6.Text = "จำนวนเงินที่โอน:"
            l6.Location = New Point(40, 420)
            l6.Size = New Size(210, 40)
            l6.TextAlign = ContentAlignment.MiddleRight
            l6.Font = New Font("Tahoma", 10.5!)

            lr.Text = "เหตุผลการโอน:"
            lr.Location = New Point(40, 482)
            lr.Size = New Size(210, 40)
            lr.TextAlign = ContentAlignment.MiddleRight
            lr.Font = New Font("Tahoma", 10.5!)

            Controls.Add(lblHeader)
            Controls.Add(l1)
            Controls.Add(l2)
            Controls.Add(l3)
            Controls.Add(l4)
            Controls.Add(l5)
            Controls.Add(l6)
            Controls.Add(lr)
            Controls.Add(dtpDate)
            Controls.Add(cboFromFund)
            Controls.Add(cboFromBank)
            Controls.Add(cboToFund)
            Controls.Add(cboToBank)
            Controls.Add(txtAmount)
            Controls.Add(txtRemark)
            Controls.Add(btnSave)
            Controls.Add(btnCancel)
            ResumeLayout(False)
        End Sub
    End Class
End Namespace

