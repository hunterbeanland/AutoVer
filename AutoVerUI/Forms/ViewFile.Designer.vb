<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ViewFile
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtApp = New System.Windows.Forms.TextBox
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.btnOK = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.btnHelp = New System.Windows.Forms.Button
        Me.Button3 = New System.Windows.Forms.Button
        Me.rbAssoc = New System.Windows.Forms.RadioButton
        Me.rbOther = New System.Windows.Forms.RadioButton
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(58, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Open with:"
        '
        'txtApp
        '
        Me.txtApp.Location = New System.Drawing.Point(93, 69)
        Me.txtApp.Name = "txtApp"
        Me.txtApp.Size = New System.Drawing.Size(214, 20)
        Me.txtApp.TabIndex = 1
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'btnOK
        '
        Me.btnOK.Image = Global.AutoVer.My.Resources.Resources.icon_tick
        Me.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnOK.Location = New System.Drawing.Point(184, 119)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(100, 23)
        Me.btnOK.TabIndex = 2
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Image = Global.AutoVer.My.Resources.Resources.icon_cancel
        Me.Button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Button2.Location = New System.Drawing.Point(290, 119)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(100, 23)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'btnHelp
        '
        Me.btnHelp.Image = Global.AutoVer.My.Resources.Resources.help
        Me.btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnHelp.Location = New System.Drawing.Point(12, 119)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(100, 23)
        Me.btnHelp.TabIndex = 6
        Me.btnHelp.Text = "Help"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(313, 67)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(26, 23)
        Me.Button3.TabIndex = 4
        Me.Button3.Text = "..."
        Me.Button3.UseVisualStyleBackColor = True
        '
        'rbAssoc
        '
        Me.rbAssoc.AutoSize = True
        Me.rbAssoc.Checked = True
        Me.rbAssoc.Location = New System.Drawing.Point(75, 23)
        Me.rbAssoc.Name = "rbAssoc"
        Me.rbAssoc.Size = New System.Drawing.Size(132, 17)
        Me.rbAssoc.TabIndex = 7
        Me.rbAssoc.TabStop = True
        Me.rbAssoc.Text = "Associated Application"
        Me.rbAssoc.UseVisualStyleBackColor = True
        '
        'rbOther
        '
        Me.rbOther.AutoSize = True
        Me.rbOther.Location = New System.Drawing.Point(75, 46)
        Me.rbOther.Name = "rbOther"
        Me.rbOther.Size = New System.Drawing.Size(109, 17)
        Me.rbOther.TabIndex = 8
        Me.rbOther.Text = "Other Application:"
        Me.rbOther.UseVisualStyleBackColor = True
        '
        'ViewFile
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(402, 154)
        Me.Controls.Add(Me.rbOther)
        Me.Controls.Add(Me.rbAssoc)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.txtApp)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "ViewFile"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AutoVer Open/View File"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtApp As System.Windows.Forms.TextBox
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents rbAssoc As System.Windows.Forms.RadioButton
    Friend WithEvents rbOther As System.Windows.Forms.RadioButton
End Class
