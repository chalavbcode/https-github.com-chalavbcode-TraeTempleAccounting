Option Strict Off
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms

Namespace TempleAccounting
    Partial Public Class FrmReceiptViewer
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.pnlScroll = New System.Windows.Forms.Panel()
            Me.picReceipt = New System.Windows.Forms.PictureBox()
            Me.lblHeader = New System.Windows.Forms.Label()
            Me.pnlBottom = New System.Windows.Forms.Panel()
            Me.flpButtons = New System.Windows.Forms.FlowLayoutPanel()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.btnOpenExternal = New System.Windows.Forms.Button()
            Me.btnFit = New System.Windows.Forms.Button()
            Me.btnActualSize = New System.Windows.Forms.Button()
            Me.lblFileInfo = New System.Windows.Forms.Label()
            Me.pnlScroll.SuspendLayout()
            CType(Me.picReceipt, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.pnlBottom.SuspendLayout()
            Me.flpButtons.SuspendLayout()
            Me.SuspendLayout()
            '
            'pnlScroll
            '
            Me.pnlScroll.AutoScroll = False
            Me.pnlScroll.BackColor = System.Drawing.Color.FromArgb(CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
            Me.pnlScroll.Controls.Add(Me.picReceipt)
            Me.pnlScroll.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pnlScroll.Location = New System.Drawing.Point(0, 50)
            Me.pnlScroll.Name = "pnlScroll"
            Me.pnlScroll.Size = New System.Drawing.Size(900, 574)
            Me.pnlScroll.TabIndex = 1
            '
            'picReceipt
            '
            Me.picReceipt.BackColor = System.Drawing.Color.FromArgb(CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
            Me.picReceipt.Dock = System.Windows.Forms.DockStyle.Fill
            Me.picReceipt.Location = New System.Drawing.Point(0, 0)
            Me.picReceipt.Name = "picReceipt"
            Me.picReceipt.Size = New System.Drawing.Size(900, 574)
            Me.picReceipt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.picReceipt.TabIndex = 0
            Me.picReceipt.TabStop = False
            '
            'lblHeader
            '
            Me.lblHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(253, Byte), Integer), CType(CType(230, Byte), Integer), CType(CType(138, Byte), Integer))
            Me.lblHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.lblHeader.Font = New System.Drawing.Font("Tahoma", 13.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(222, Byte))
            Me.lblHeader.ForeColor = System.Drawing.Color.FromArgb(CType(CType(69, Byte), Integer), CType(CType(26, Byte), Integer), CType(CType(3, Byte), Integer))
            Me.lblHeader.Location = New System.Drawing.Point(0, 0)
            Me.lblHeader.Name = "lblHeader"
            Me.lblHeader.Size = New System.Drawing.Size(900, 50)
            Me.lblHeader.TabIndex = 0
            Me.lblHeader.Text = "🧾 ดูใบเสร็จ (Receipt Viewer)"
            Me.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'pnlBottom
            '
            Me.pnlBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(245, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(220, Byte), Integer))
            Me.pnlBottom.Controls.Add(Me.flpButtons)
            Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.pnlBottom.Location = New System.Drawing.Point(0, 624)
            Me.pnlBottom.Name = "pnlBottom"
            Me.pnlBottom.Size = New System.Drawing.Size(900, 56)
            Me.pnlBottom.TabIndex = 2
            '
            'flpButtons
            '
            Me.flpButtons.Controls.Add(Me.btnClose)
            Me.flpButtons.Controls.Add(Me.btnOpenExternal)
            Me.flpButtons.Controls.Add(Me.btnFit)
            Me.flpButtons.Controls.Add(Me.btnActualSize)
            Me.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill
            Me.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
            Me.flpButtons.Location = New System.Drawing.Point(0, 0)
            Me.flpButtons.Name = "flpButtons"
            Me.flpButtons.Padding = New System.Windows.Forms.Padding(12, 10, 12, 8)
            Me.flpButtons.Size = New System.Drawing.Size(900, 56)
            Me.flpButtons.TabIndex = 0
            '
            'btnClose
            '
            Me.btnClose.BackColor = System.Drawing.Color.FromArgb(CType(CType(75, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(99, Byte), Integer))
            Me.btnClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnClose.FlatAppearance.BorderSize = 0
            Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnClose.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(222, Byte))
            Me.btnClose.ForeColor = System.Drawing.Color.White
            Me.btnClose.Location = New System.Drawing.Point(762, 7)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(126, 38)
            Me.btnClose.TabIndex = 0
            Me.btnClose.Text = "❌ ปิด"
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'btnOpenExternal
            '
            Me.btnOpenExternal.BackColor = System.Drawing.Color.FromArgb(CType(CType(37, Byte), Integer), CType(CType(99, Byte), Integer), CType(CType(235, Byte), Integer))
            Me.btnOpenExternal.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnOpenExternal.FlatAppearance.BorderSize = 0
            Me.btnOpenExternal.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnOpenExternal.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(222, Byte))
            Me.btnOpenExternal.ForeColor = System.Drawing.Color.White
            Me.btnOpenExternal.Location = New System.Drawing.Point(611, 7)
            Me.btnOpenExternal.Name = "btnOpenExternal"
            Me.btnOpenExternal.Size = New System.Drawing.Size(145, 38)
            Me.btnOpenExternal.TabIndex = 1
            Me.btnOpenExternal.Text = "🖥️ เปิดภายนอก"
            Me.btnOpenExternal.UseVisualStyleBackColor = False
            '
            'btnFit
            '
            Me.btnFit.BackColor = System.Drawing.Color.FromArgb(CType(CType(5, Byte), Integer), CType(CType(150, Byte), Integer), CType(CType(105, Byte), Integer))
            Me.btnFit.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnFit.FlatAppearance.BorderSize = 0
            Me.btnFit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnFit.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(222, Byte))
            Me.btnFit.ForeColor = System.Drawing.Color.White
            Me.btnFit.Location = New System.Drawing.Point(500, 7)
            Me.btnFit.Name = "btnFit"
            Me.btnFit.Size = New System.Drawing.Size(105, 38)
            Me.btnFit.TabIndex = 2
            Me.btnFit.Text = "↔️ พอดีหน้าจอ"
            Me.btnFit.UseVisualStyleBackColor = False
            '
            'btnActualSize
            '
            Me.btnActualSize.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(119, Byte), Integer), CType(CType(6, Byte), Integer))
            Me.btnActualSize.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnActualSize.FlatAppearance.BorderSize = 0
            Me.btnActualSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnActualSize.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(222, Byte))
            Me.btnActualSize.ForeColor = System.Drawing.Color.White
            Me.btnActualSize.Location = New System.Drawing.Point(384, 7)
            Me.btnActualSize.Name = "btnActualSize"
            Me.btnActualSize.Size = New System.Drawing.Size(110, 38)
            Me.btnActualSize.TabIndex = 3
            Me.btnActualSize.Text = "🔍 ขนาดจริง"
            Me.btnActualSize.UseVisualStyleBackColor = False
            '
            'lblFileInfo
            '
            Me.lblFileInfo.BackColor = System.Drawing.Color.FromArgb(CType(CType(245, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(220, Byte), Integer))
            Me.lblFileInfo.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.lblFileInfo.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(222, Byte))
            Me.lblFileInfo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(75, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(99, Byte), Integer))
            Me.lblFileInfo.Location = New System.Drawing.Point(0, 598)
            Me.lblFileInfo.Name = "lblFileInfo"
            Me.lblFileInfo.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
            Me.lblFileInfo.Size = New System.Drawing.Size(900, 26)
            Me.lblFileInfo.TabIndex = 3
            Me.lblFileInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'FrmReceiptViewer
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(249, Byte), Integer), CType(CType(235, Byte), Integer))
            Me.ClientSize = New System.Drawing.Size(900, 680)
            Me.Controls.Add(Me.pnlScroll)
            Me.Controls.Add(Me.lblFileInfo)
            Me.Controls.Add(Me.pnlBottom)
            Me.Controls.Add(Me.lblHeader)
            Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(222, Byte))
            Me.MinimumSize = New System.Drawing.Size(640, 480)
            Me.Name = "FrmReceiptViewer"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "ดูใบเสร็จ"
            Me.pnlScroll.ResumeLayout(False)
            CType(Me.picReceipt, System.ComponentModel.ISupportInitialize).EndInit()
            Me.pnlBottom.ResumeLayout(False)
            Me.flpButtons.ResumeLayout(False)
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents pnlScroll As Panel
        Friend WithEvents picReceipt As PictureBox
        Friend WithEvents lblHeader As Label
        Friend WithEvents pnlBottom As Panel
        Friend WithEvents flpButtons As FlowLayoutPanel
        Friend WithEvents btnClose As Button
        Friend WithEvents btnOpenExternal As Button
        Friend WithEvents btnFit As Button
        Friend WithEvents btnActualSize As Button
        Friend WithEvents lblFileInfo As Label
    End Class
End Namespace
