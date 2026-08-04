Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Public Class FrmPersonnelManagement
        Inherits Form

        Private components As IContainer = Nothing

        Friend WithEvents pnlHeader As Panel
        Friend WithEvents lblHeader As Label
        Friend WithEvents pnlMain As Panel
        Friend WithEvents tcMain As TabControl
        Friend WithEvents tpPersonnel As TabPage
        Friend WithEvents tpPositions As TabPage
        Friend WithEvents dgvPersonnel As DataGridView
        Friend WithEvents pnlEditor As Panel
        Friend WithEvents tlpEditor As TableLayoutPanel
        Friend WithEvents lblTitle As Label
        Friend WithEvents txtTitle As TextBox
        Friend WithEvents lblFirstName As Label
        Friend WithEvents txtFirstName As TextBox
        Friend WithEvents lblLastName As Label
        Friend WithEvents txtLastName As TextBox
        Friend WithEvents lblPersonType As Label
        Friend WithEvents cboPersonType As ComboBox
        Friend WithEvents lblPhone As Label
        Friend WithEvents txtPhone As TextBox
        Friend WithEvents dgvPositions As DataGridView
        Friend WithEvents pnlPosEditor As Panel
        Friend WithEvents tlpPosEditor As TableLayoutPanel
        Friend WithEvents lblPositionName As Label
        Friend WithEvents txtPositionName As TextBox
        Friend WithEvents pnlButtons As Panel
        Friend WithEvents flpButtons As FlowLayoutPanel
        Friend WithEvents btnSave As Button
        Friend WithEvents btnDelete As Button
        Friend WithEvents btnNew As Button
        Friend WithEvents btnClose As Button

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Me.pnlHeader = New System.Windows.Forms.Panel()
            Me.lblHeader = New System.Windows.Forms.Label()
            Me.pnlMain = New System.Windows.Forms.Panel()
            Me.tcMain = New System.Windows.Forms.TabControl()
            Me.tpPersonnel = New System.Windows.Forms.TabPage()
            Me.dgvPersonnel = New System.Windows.Forms.DataGridView()
            Me.pnlEditor = New System.Windows.Forms.Panel()
            Me.tlpEditor = New System.Windows.Forms.TableLayoutPanel()
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.txtTitle = New System.Windows.Forms.TextBox()
            Me.lblFirstName = New System.Windows.Forms.Label()
            Me.txtFirstName = New System.Windows.Forms.TextBox()
            Me.lblLastName = New System.Windows.Forms.Label()
            Me.txtLastName = New System.Windows.Forms.TextBox()
            Me.lblPersonType = New System.Windows.Forms.Label()
            Me.cboPersonType = New System.Windows.Forms.ComboBox()
            Me.lblPhone = New System.Windows.Forms.Label()
            Me.txtPhone = New System.Windows.Forms.TextBox()
            Me.tpPositions = New System.Windows.Forms.TabPage()
            Me.dgvPositions = New System.Windows.Forms.DataGridView()
            Me.pnlPosEditor = New System.Windows.Forms.Panel()
            Me.tlpPosEditor = New System.Windows.Forms.TableLayoutPanel()
            Me.lblPositionName = New System.Windows.Forms.Label()
            Me.txtPositionName = New System.Windows.Forms.TextBox()
            Me.pnlButtons = New System.Windows.Forms.Panel()
            Me.flpButtons = New System.Windows.Forms.FlowLayoutPanel()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.btnDelete = New System.Windows.Forms.Button()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.btnNew = New System.Windows.Forms.Button()
            Me.pnlHeader.SuspendLayout()
            Me.pnlMain.SuspendLayout()
            Me.tcMain.SuspendLayout()
            Me.tpPersonnel.SuspendLayout()
            CType(Me.dgvPersonnel, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlEditor.SuspendLayout()
            Me.tlpEditor.SuspendLayout()
            Me.tpPositions.SuspendLayout()
            CType(Me.dgvPositions, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlPosEditor.SuspendLayout()
            Me.tlpPosEditor.SuspendLayout()
            Me.pnlButtons.SuspendLayout()
            Me.flpButtons.SuspendLayout()
            Me.SuspendLayout()
            '
            'pnlHeader
            '
            Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(253, 230, 138)
            Me.pnlHeader.Controls.Add(Me.lblHeader)
            Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
            Me.pnlHeader.Name = "pnlHeader"
            Me.pnlHeader.Size = New System.Drawing.Size(900, 42)
            Me.pnlHeader.TabIndex = 0
            '
            'lblHeader
            '
            Me.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lblHeader.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold)
            Me.lblHeader.ForeColor = System.Drawing.Color.FromArgb(69, 26, 3)
            Me.lblHeader.Location = New System.Drawing.Point(0, 0)
            Me.lblHeader.Name = "lblHeader"
            Me.lblHeader.Size = New System.Drawing.Size(900, 42)
            Me.lblHeader.TabIndex = 0
            Me.lblHeader.Text = "👤 จัดการรายชื่อบุคลากรและตำแหน่ง"
            Me.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'pnlMain
            '
            Me.pnlMain.Controls.Add(Me.tcMain)
            Me.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlMain.Location = New System.Drawing.Point(0, 42)
            Me.pnlMain.Name = "pnlMain"
            Me.pnlMain.Size = New System.Drawing.Size(900, 508)
            Me.pnlMain.TabIndex = 1
            '
            'tcMain
            '
            Me.tcMain.Controls.Add(Me.tpPersonnel)
            Me.tcMain.Controls.Add(Me.tpPositions)
            Me.tcMain.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tcMain.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.tcMain.Location = New System.Drawing.Point(0, 0)
            Me.tcMain.Name = "tcMain"
            Me.tcMain.SelectedIndex = 0
            Me.tcMain.Size = New System.Drawing.Size(900, 508)
            Me.tcMain.TabIndex = 0
            '
            'tpPersonnel
            '
            Me.tpPersonnel.Controls.Add(Me.dgvPersonnel)
            Me.tpPersonnel.Controls.Add(Me.pnlEditor)
            Me.tpPersonnel.Location = New System.Drawing.Point(4, 30)
            Me.tpPersonnel.Name = "tpPersonnel"
            Me.tpPersonnel.Padding = New System.Windows.Forms.Padding(3)
            Me.tpPersonnel.Size = New System.Drawing.Size(892, 474)
            Me.tpPersonnel.TabIndex = 0
            Me.tpPersonnel.Text = "บุคลากร"
            Me.tpPersonnel.UseVisualStyleBackColor = True
            '
            'dgvPersonnel
            '
            Me.dgvPersonnel.AllowUserToAddRows = False
            Me.dgvPersonnel.AllowUserToDeleteRows = False
            Me.dgvPersonnel.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvPersonnel.BackgroundColor = System.Drawing.Color.White
            Me.dgvPersonnel.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.dgvPersonnel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvPersonnel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvPersonnel.Location = New System.Drawing.Point(3, 223)
            Me.dgvPersonnel.MultiSelect = False
            Me.dgvPersonnel.Name = "dgvPersonnel"
            Me.dgvPersonnel.ReadOnly = True
            Me.dgvPersonnel.RowHeadersWidth = 51
            Me.dgvPersonnel.RowTemplate.Height = 30
            Me.dgvPersonnel.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvPersonnel.Size = New System.Drawing.Size(886, 248)
            Me.dgvPersonnel.TabIndex = 1
            '
            'pnlEditor
            '
            Me.pnlEditor.BackColor = System.Drawing.Color.White
            Me.pnlEditor.Controls.Add(Me.tlpEditor)
            Me.pnlEditor.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlEditor.Location = New System.Drawing.Point(3, 3)
            Me.pnlEditor.Name = "pnlEditor"
            Me.pnlEditor.Padding = New System.Windows.Forms.Padding(20)
            Me.pnlEditor.Size = New System.Drawing.Size(886, 220)
            Me.pnlEditor.TabIndex = 0
            '
            'tlpEditor
            '
            Me.tlpEditor.ColumnCount = 4
            Me.tlpEditor.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120.0!))
            Me.tlpEditor.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.tlpEditor.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120.0!))
            Me.tlpEditor.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.tlpEditor.Controls.Add(Me.lblTitle, 0, 0)
            Me.tlpEditor.Controls.Add(Me.txtTitle, 1, 0)
            Me.tlpEditor.Controls.Add(Me.lblFirstName, 0, 1)
            Me.tlpEditor.Controls.Add(Me.txtFirstName, 1, 1)
            Me.tlpEditor.Controls.Add(Me.lblLastName, 2, 1)
            Me.tlpEditor.Controls.Add(Me.txtLastName, 3, 1)
            Me.tlpEditor.Controls.Add(Me.lblPersonType, 0, 2)
            Me.tlpEditor.Controls.Add(Me.cboPersonType, 1, 2)
            Me.tlpEditor.Controls.Add(Me.lblPhone, 2, 2)
            Me.tlpEditor.Controls.Add(Me.txtPhone, 3, 2)
            Me.tlpEditor.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tlpEditor.Location = New System.Drawing.Point(20, 20)
            Me.tlpEditor.Name = "tlpEditor"
            Me.tlpEditor.RowCount = 3
            Me.tlpEditor.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
            Me.tlpEditor.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
            Me.tlpEditor.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
            Me.tlpEditor.Size = New System.Drawing.Size(846, 180)
            Me.tlpEditor.TabIndex = 0
            '
            'lblTitle
            '
            Me.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblTitle.Location = New System.Drawing.Point(40, 19)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(77, 21)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "คำนำหน้า:"
            '
            'txtTitle
            '
            Me.txtTitle.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtTitle.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtTitle.Location = New System.Drawing.Point(123, 16)
            Me.txtTitle.Name = "txtTitle"
            Me.txtTitle.Size = New System.Drawing.Size(297, 28)
            Me.txtTitle.TabIndex = 1
            '
            'lblFirstName
            '
            Me.lblFirstName.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.lblFirstName.AutoSize = True
            Me.lblFirstName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblFirstName.Location = New System.Drawing.Point(82, 79)
            Me.lblFirstName.Name = "lblFirstName"
            Me.lblFirstName.Size = New System.Drawing.Size(35, 21)
            Me.lblFirstName.TabIndex = 2
            Me.lblFirstName.Text = "ชื่อ:"
            '
            'txtFirstName
            '
            Me.txtFirstName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtFirstName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtFirstName.Location = New System.Drawing.Point(123, 76)
            Me.txtFirstName.Name = "txtFirstName"
            Me.txtFirstName.Size = New System.Drawing.Size(297, 28)
            Me.txtFirstName.TabIndex = 3
            '
            'lblLastName
            '
            Me.lblLastName.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.lblLastName.AutoSize = True
            Me.lblLastName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblLastName.Location = New System.Drawing.Point(445, 79)
            Me.lblLastName.Name = "lblLastName"
            Me.lblLastName.Size = New System.Drawing.Size(75, 21)
            Me.lblLastName.TabIndex = 4
            Me.lblLastName.Text = "นามสกุล:"
            '
            'txtLastName
            '
            Me.txtLastName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtLastName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtLastName.Location = New System.Drawing.Point(526, 76)
            Me.txtLastName.Name = "txtLastName"
            Me.txtLastName.Size = New System.Drawing.Size(317, 28)
            Me.txtLastName.TabIndex = 5
            '
            'lblPersonType
            '
            Me.lblPersonType.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.lblPersonType.AutoSize = True
            Me.lblPersonType.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblPersonType.Location = New System.Drawing.Point(50, 139)
            Me.lblPersonType.Name = "lblPersonType"
            Me.lblPersonType.Size = New System.Drawing.Size(67, 21)
            Me.lblPersonType.TabIndex = 6
            Me.lblPersonType.Text = "ประเภท:"
            '
            'cboPersonType
            '
            Me.cboPersonType.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.cboPersonType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboPersonType.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.cboPersonType.FormattingEnabled = True
            Me.cboPersonType.Items.AddRange(New Object() {"Monk", "Layperson"})
            Me.cboPersonType.Location = New System.Drawing.Point(123, 135)
            Me.cboPersonType.Name = "cboPersonType"
            Me.cboPersonType.Size = New System.Drawing.Size(297, 29)
            Me.cboPersonType.TabIndex = 7
            '
            'lblPhone
            '
            Me.lblPhone.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.lblPhone.AutoSize = True
            Me.lblPhone.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblPhone.Location = New System.Drawing.Point(444, 139)
            Me.lblPhone.Name = "lblPhone"
            Me.lblPhone.Size = New System.Drawing.Size(76, 21)
            Me.lblPhone.TabIndex = 8
            Me.lblPhone.Text = "เบอร์โทร:"
            '
            'txtPhone
            '
            Me.txtPhone.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtPhone.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtPhone.Location = New System.Drawing.Point(526, 136)
            Me.txtPhone.Name = "txtPhone"
            Me.txtPhone.Size = New System.Drawing.Size(317, 28)
            Me.txtPhone.TabIndex = 9
            '
            'tpPositions
            '
            Me.tpPositions.Controls.Add(Me.dgvPositions)
            Me.tpPositions.Controls.Add(Me.pnlPosEditor)
            Me.tpPositions.Location = New System.Drawing.Point(4, 30)
            Me.tpPositions.Name = "tpPositions"
            Me.tpPositions.Padding = New System.Windows.Forms.Padding(3)
            Me.tpPositions.Size = New System.Drawing.Size(892, 474)
            Me.tpPositions.TabIndex = 1
            Me.tpPositions.Text = "ตำแหน่งหน้าที่"
            Me.tpPositions.UseVisualStyleBackColor = True
            '
            'dgvPositions
            '
            Me.dgvPositions.AllowUserToAddRows = False
            Me.dgvPositions.AllowUserToDeleteRows = False
            Me.dgvPositions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
            Me.dgvPositions.BackgroundColor = System.Drawing.Color.White
            Me.dgvPositions.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.dgvPositions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvPositions.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvPositions.Location = New System.Drawing.Point(3, 103)
            Me.dgvPositions.MultiSelect = False
            Me.dgvPositions.Name = "dgvPositions"
            Me.dgvPositions.ReadOnly = True
            Me.dgvPositions.RowHeadersWidth = 51
            Me.dgvPositions.RowTemplate.Height = 30
            Me.dgvPositions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvPositions.Size = New System.Drawing.Size(886, 368)
            Me.dgvPositions.TabIndex = 1
            '
            'pnlPosEditor
            '
            Me.pnlPosEditor.BackColor = System.Drawing.Color.White
            Me.pnlPosEditor.Controls.Add(Me.tlpPosEditor)
            Me.pnlPosEditor.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlPosEditor.Location = New System.Drawing.Point(3, 3)
            Me.pnlPosEditor.Name = "pnlPosEditor"
            Me.pnlPosEditor.Padding = New System.Windows.Forms.Padding(20)
            Me.pnlPosEditor.Size = New System.Drawing.Size(886, 100)
            Me.pnlPosEditor.TabIndex = 0
            '
            'tlpPosEditor
            '
            Me.tlpPosEditor.ColumnCount = 2
            Me.tlpPosEditor.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150.0!))
            Me.tlpPosEditor.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.tlpPosEditor.Controls.Add(Me.lblPositionName, 0, 0)
            Me.tlpPosEditor.Controls.Add(Me.txtPositionName, 1, 0)
            Me.tlpPosEditor.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tlpPosEditor.Location = New System.Drawing.Point(20, 20)
            Me.tlpPosEditor.Name = "tlpPosEditor"
            Me.tlpPosEditor.RowCount = 1
            Me.tlpPosEditor.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.tlpPosEditor.Size = New System.Drawing.Size(846, 60)
            Me.tlpPosEditor.TabIndex = 0
            '
            'lblPositionName
            '
            Me.lblPositionName.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.lblPositionName.AutoSize = True
            Me.lblPositionName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.lblPositionName.Location = New System.Drawing.Point(34, 19)
            Me.lblPositionName.Name = "lblPositionName"
            Me.lblPositionName.Size = New System.Drawing.Size(113, 21)
            Me.lblPositionName.TabIndex = 0
            Me.lblPositionName.Text = "ชื่อตำแหน่ง:"
            '
            'txtPositionName
            '
            Me.txtPositionName.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
            Me.txtPositionName.Font = New System.Drawing.Font("Tahoma", 10.0!)
            Me.txtPositionName.Location = New System.Drawing.Point(153, 16)
            Me.txtPositionName.Name = "txtPositionName"
            Me.txtPositionName.Size = New System.Drawing.Size(690, 28)
            Me.txtPositionName.TabIndex = 1
            '
            'pnlButtons
            '
            Me.pnlButtons.BackColor = System.Drawing.Color.FromArgb(245, 245, 240)
            Me.pnlButtons.Controls.Add(Me.flpButtons)
            Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.pnlButtons.Location = New System.Drawing.Point(0, 550)
            Me.pnlButtons.Name = "pnlButtons"
            Me.pnlButtons.Size = New System.Drawing.Size(900, 70)
            Me.pnlButtons.TabIndex = 2
            '
            'flpButtons
            '
            Me.flpButtons.Controls.Add(Me.btnClose)
            Me.flpButtons.Controls.Add(Me.btnDelete)
            Me.flpButtons.Controls.Add(Me.btnSave)
            Me.flpButtons.Controls.Add(Me.btnNew)
            Me.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill
            Me.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
            Me.flpButtons.Location = New System.Drawing.Point(0, 0)
            Me.flpButtons.Name = "flpButtons"
            Me.flpButtons.Padding = New System.Windows.Forms.Padding(10, 15, 10, 0)
            Me.flpButtons.Size = New System.Drawing.Size(900, 70)
            Me.flpButtons.TabIndex = 0
            '
            'btnClose
            '
            Me.btnClose.BackColor = System.Drawing.Color.FromArgb(75, 85, 99)
            Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnClose.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnClose.ForeColor = System.Drawing.Color.White
            Me.btnClose.Location = New System.Drawing.Point(777, 18)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(100, 40)
            Me.btnClose.TabIndex = 3
            Me.btnClose.Text = "ปิด"
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'btnDelete
            '
            Me.btnDelete.BackColor = System.Drawing.Color.FromArgb(153, 27, 27)
            Me.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnDelete.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnDelete.ForeColor = System.Drawing.Color.White
            Me.btnDelete.Location = New System.Drawing.Point(651, 18)
            Me.btnDelete.Name = "btnDelete"
            Me.btnDelete.Size = New System.Drawing.Size(120, 40)
            Me.btnDelete.TabIndex = 1
            Me.btnDelete.Text = "🗑️ ลบ"
            Me.btnDelete.UseVisualStyleBackColor = False
            '
            'btnSave
            '
            Me.btnSave.BackColor = System.Drawing.Color.FromArgb(22, 163, 74)
            Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnSave.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnSave.ForeColor = System.Drawing.Color.White
            Me.btnSave.Location = New System.Drawing.Point(525, 18)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(120, 40)
            Me.btnSave.TabIndex = 0
            Me.btnSave.Text = "💾 บันทึก"
            Me.btnSave.UseVisualStyleBackColor = False
            '
            'btnNew
            '
            Me.btnNew.BackColor = System.Drawing.Color.FromArgb(37, 99, 235)
            Me.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnNew.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnNew.ForeColor = System.Drawing.Color.White
            Me.btnNew.Location = New System.Drawing.Point(399, 18)
            Me.btnNew.Name = "btnNew"
            Me.btnNew.Size = New System.Drawing.Size(120, 40)
            Me.btnNew.TabIndex = 2
            Me.btnNew.Text = "➕ เพิ่มใหม่"
            Me.btnNew.UseVisualStyleBackColor = False
            '
            'FrmPersonnelManagement
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(900, 620)
            Me.Controls.Add(Me.pnlMain)
            Me.Controls.Add(Me.pnlButtons)
            Me.Controls.Add(Me.pnlHeader)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "FrmPersonnelManagement"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "จัดการรายชื่อบุคลากรและตำแหน่ง"
            Me.pnlHeader.ResumeLayout(False)
            Me.pnlMain.ResumeLayout(False)
            Me.tcMain.ResumeLayout(False)
            Me.tpPersonnel.ResumeLayout(False)
            CType(Me.dgvPersonnel, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlEditor.ResumeLayout(False)
            Me.tlpEditor.ResumeLayout(False)
            Me.tlpEditor.PerformLayout()
            Me.tpPositions.ResumeLayout(False)
            CType(Me.dgvPositions, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlPosEditor.ResumeLayout(False)
            Me.tlpPosEditor.ResumeLayout(False)
            Me.tlpPosEditor.PerformLayout()
            Me.pnlButtons.ResumeLayout(False)
            Me.flpButtons.ResumeLayout(False)
            Me.ResumeLayout(False)
        End Sub
    End Class
End Namespace
