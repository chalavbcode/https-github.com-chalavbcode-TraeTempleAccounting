Namespace TempleAccounting
    Partial Class FrmHelpDialog
        Inherits System.Windows.Forms.Form

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.lblTitle = New System.Windows.Forms.Label()
            Me.rtbContent = New System.Windows.Forms.RichTextBox()
            Me.pnlBottom = New System.Windows.Forms.Panel()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.pnlBottom.SuspendLayout()
            Me.SuspendLayout()
            '
            'lblTitle
            '
            Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Top
            Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me.lblTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
            Me.lblTitle.Location = New System.Drawing.Point(0, 0)
            Me.lblTitle.Name = "lblTitle"
            Me.lblTitle.Padding = New System.Windows.Forms.Padding(15, 15, 15, 10)
            Me.lblTitle.Size = New System.Drawing.Size(684, 50)
            Me.lblTitle.TabIndex = 0
            Me.lblTitle.Text = "คู่มือการใช้งาน: ..."
            Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'rtbContent
            '
            Me.rtbContent.BackColor = System.Drawing.Color.White
            Me.rtbContent.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.rtbContent.Dock = System.Windows.Forms.DockStyle.Fill
            Me.rtbContent.Font = New System.Drawing.Font("Tahoma", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.rtbContent.Location = New System.Drawing.Point(0, 50)
            Me.rtbContent.Padding = New System.Windows.Forms.Padding(15, 10, 15, 10)
            Me.rtbContent.Name = "rtbContent"
            Me.rtbContent.ReadOnly = True
            Me.rtbContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
            Me.rtbContent.Size = New System.Drawing.Size(684, 313)
            Me.rtbContent.TabIndex = 1
            Me.rtbContent.Text = ""
            Me.rtbContent.WordWrap = True
            '
            'pnlBottom
            '
            Me.pnlBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer))
            Me.pnlBottom.Controls.Add(Me.btnClose)
            Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.pnlBottom.Location = New System.Drawing.Point(0, 363)
            Me.pnlBottom.Name = "pnlBottom"
            Me.pnlBottom.Padding = New System.Windows.Forms.Padding(10)
            Me.pnlBottom.Size = New System.Drawing.Size(684, 55)
            Me.pnlBottom.TabIndex = 2
            '
            'btnClose
            '
            Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.btnClose.BackColor = System.Drawing.Color.FromArgb(CType(CType(52, Byte), Integer), CType(CType(73, Byte), Integer), CType(CType(94, Byte), Integer))
            Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.btnClose.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.btnClose.ForeColor = System.Drawing.Color.White
            Me.btnClose.Location = New System.Drawing.Point(579, 10)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(95, 32)
            Me.btnClose.TabIndex = 0
            Me.btnClose.Text = "ปิด (Close)"
            Me.btnClose.UseVisualStyleBackColor = False
            '
            'FrmHelpDialog
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(684, 418)
            Me.Controls.Add(Me.rtbContent)
            Me.Controls.Add(Me.lblTitle)
            Me.Controls.Add(Me.pnlBottom)
            Me.MinimumSize = New System.Drawing.Size(500, 350)
            Me.Name = "FrmHelpDialog"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "ความช่วยเหลือ - ระบบบัญชีวัด"
            Me.pnlBottom.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents rtbContent As System.Windows.Forms.RichTextBox
        Friend WithEvents pnlBottom As System.Windows.Forms.Panel
        Friend WithEvents btnClose As System.Windows.Forms.Button
    End Class
End Namespace
