Namespace TempleAccounting
    Partial Class FrmHelpDialog
        Inherits System.Windows.Forms.Form

        Private components As System.ComponentModel.IContainer = Nothing

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.rtbContent = New System.Windows.Forms.RichTextBox()
            Me.pnlFooter = New System.Windows.Forms.Panel()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.pnlHeader = New System.Windows.Forms.Panel()
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.pnlFooter.SuspendLayout()
            Me.pnlHeader.SuspendLayout()
            Me.SuspendLayout()
            '
            'rtbContent
            '
            Me.rtbContent.BackColor = System.Drawing.Color.White
            Me.rtbContent.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.rtbContent.Dock = System.Windows.Forms.DockStyle.Fill
            Me.rtbContent.Font = New System.Drawing.Font("Segoe UI", 11.0!)
            Me.rtbContent.Location = New System.Drawing.Point(20, 70)
            Me.rtbContent.Name = "rtbContent"
            Me.rtbContent.ReadOnly = True
            Me.rtbContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
            Me.rtbContent.Size = New System.Drawing.Size(760, 430)
            Me.rtbContent.TabIndex = 0
            Me.rtbContent.Text = ""
            '
            'pnlFooter
            '
            Me.pnlFooter.BackColor = System.Drawing.Color.White
            Me.pnlFooter.Controls.Add(Me.btnClose)
            Me.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.pnlFooter.Location = New System.Drawing.Point(0, 500)
            Me.pnlFooter.Name = "pnlFooter"
            Me.pnlFooter.Size = New System.Drawing.Size(800, 60)
            Me.pnlFooter.TabIndex = 1
            '
            'btnClose
            '
            Me.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.btnClose.BackColor = System.Drawing.Color.FromArgb(CType(CType(231, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(60, Byte), Integer))
            Me.btnClose.Cursor = System.Windows.Forms.Cursors.Hand
            Me.btnClose.FlatAppearance.BorderSize = 0
            Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnClose.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
            Me.btnClose.ForeColor = System.Drawing.Color.White
            Me.btnClose.Location = New System.Drawing.Point(660, 10)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(120, 40)
            Me.btnClose.TabIndex = 0
            Me.btnClose.Text = "ปิดหน้าต่าง"
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'pnlHeader
            '
            Me.pnlHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(52, Byte), Integer), CType(CType(73, Byte), Integer), CType(CType(94, Byte), Integer))
            Me.pnlHeader.Controls.Add(Me.lblTitle)
            Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
            Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
            Me.pnlHeader.Name = "pnlHeader"
            Me.pnlHeader.Size = New System.Drawing.Size(800, 60)
            Me.pnlHeader.TabIndex = 2
            '
            'lblTitle
            '
            Me.lblTitle.AutoSize = True
            Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
            Me.lblTitle.ForeColor = System.Drawing.Color.White
            Me.lblTitle.Location = New System.Drawing.Point(20, 18)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Size = New System.Drawing.Size(185, 25)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "คู่มือการใช้งานหน้าจอ"
            '
            'FrmHelpDialog
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.BackColor = System.Drawing.Color.White
            Me.ClientSize = New System.Drawing.Size(800, 560)
            Me.Controls.Add(Me.rtbContent)
            Me.Controls.Add(Me.pnlHeader)
            Me.Controls.Add(Me.pnlFooter)
            Me.Font = New System.Drawing.Font("Segoe UI", 9.75!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "FrmHelpDialog"
            Me.Padding = New System.Windows.Forms.Padding(20, 10, 20, 0)
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "ความช่วยเหลือ - ระบบบัญชีวัด"
            Me.pnlFooter.ResumeLayout(False)
            Me.pnlHeader.ResumeLayout(False)
            Me.pnlHeader.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents rtbContent As System.Windows.Forms.RichTextBox
        Friend WithEvents pnlFooter As System.Windows.Forms.Panel
        Friend WithEvents btnClose As System.Windows.Forms.Button
        Friend WithEvents pnlHeader As System.Windows.Forms.Panel
        Friend WithEvents lblTitle As System.Windows.Forms.Label
    End Class
End Namespace
