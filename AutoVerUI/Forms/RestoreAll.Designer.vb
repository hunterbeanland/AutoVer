<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RestoreAll
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
        Me.btnOK = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.rbNow = New System.Windows.Forms.RadioButton
        Me.rbDtp = New System.Windows.Forms.RadioButton
        Me.dtpRestore = New System.Windows.Forms.DateTimePicker
        Me.txtHour = New System.Windows.Forms.TextBox
        Me.txtMin = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblMessage = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(93, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Restore all files to:"
        '
        'btnOK
        '
        Me.btnOK.Image = Global.AutoVer.My.Resources.Resources.icon_tick
        Me.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnOK.Location = New System.Drawing.Point(184, 106)
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
        Me.Button2.Location = New System.Drawing.Point(290, 106)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(100, 23)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'rbNow
        '
        Me.rbNow.AutoSize = True
        Me.rbNow.Checked = True
        Me.rbNow.Location = New System.Drawing.Point(113, 23)
        Me.rbNow.Name = "rbNow"
        Me.rbNow.Size = New System.Drawing.Size(47, 17)
        Me.rbNow.TabIndex = 7
        Me.rbNow.TabStop = True
        Me.rbNow.Text = "Now"
        Me.rbNow.UseVisualStyleBackColor = True
        '
        'rbDtp
        '
        Me.rbDtp.AutoSize = True
        Me.rbDtp.Location = New System.Drawing.Point(113, 46)
        Me.rbDtp.Name = "rbDtp"
        Me.rbDtp.Size = New System.Drawing.Size(14, 13)
        Me.rbDtp.TabIndex = 8
        Me.rbDtp.UseVisualStyleBackColor = True
        '
        'dtpRestore
        '
        Me.dtpRestore.Location = New System.Drawing.Point(134, 47)
        Me.dtpRestore.Name = "dtpRestore"
        Me.dtpRestore.Size = New System.Drawing.Size(200, 20)
        Me.dtpRestore.TabIndex = 9
        '
        'txtHour
        '
        Me.txtHour.Location = New System.Drawing.Point(134, 73)
        Me.txtHour.Name = "txtHour"
        Me.txtHour.Size = New System.Drawing.Size(26, 20)
        Me.txtHour.TabIndex = 13
        '
        'txtMin
        '
        Me.txtMin.Location = New System.Drawing.Point(173, 73)
        Me.txtMin.Name = "txtMin"
        Me.txtMin.Size = New System.Drawing.Size(26, 20)
        Me.txtMin.TabIndex = 14
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(162, 76)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(10, 13)
        Me.Label2.TabIndex = 15
        Me.Label2.Text = ":"
        '
        'lblMessage
        '
        Me.lblMessage.AutoSize = True
        Me.lblMessage.Location = New System.Drawing.Point(13, 111)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(52, 13)
        Me.lblMessage.TabIndex = 16
        Me.lblMessage.Text = "Waiting..."
        '
        'RestoreAll
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(402, 141)
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtMin)
        Me.Controls.Add(Me.txtHour)
        Me.Controls.Add(Me.dtpRestore)
        Me.Controls.Add(Me.rbDtp)
        Me.Controls.Add(Me.rbNow)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "RestoreAll"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AutoVer Restore All"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents rbNow As System.Windows.Forms.RadioButton
    Friend WithEvents rbDtp As System.Windows.Forms.RadioButton
    Friend WithEvents dtpRestore As System.Windows.Forms.DateTimePicker
    Friend WithEvents txtHour As System.Windows.Forms.TextBox
    Friend WithEvents txtMin As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblMessage As System.Windows.Forms.Label
End Class
