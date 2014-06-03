<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ErrorReporter
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ErrorReporter))
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lklErrorReport = New System.Windows.Forms.LinkLabel
        Me.btnOK = New System.Windows.Forms.Button
        Me.btnNo = New System.Windows.Forms.Button
        Me.lblStatus = New System.Windows.Forms.Label
        Me.txtErrorText = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtNameEmail = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtRecreate = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(181, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(152, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "A fatal error has occured."
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 93)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(292, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Details about this error can be sent to the author for analysis."
        '
        'lklErrorReport
        '
        Me.lklErrorReport.AutoSize = True
        Me.lklErrorReport.Location = New System.Drawing.Point(419, 264)
        Me.lklErrorReport.Name = "lklErrorReport"
        Me.lklErrorReport.Size = New System.Drawing.Size(84, 13)
        Me.lklErrorReport.TabIndex = 2
        Me.lklErrorReport.TabStop = True
        Me.lklErrorReport.Text = "View error report"
        '
        'btnOK
        '
        Me.btnOK.Image = Global.AutoVer.My.Resources.Resources.icon_tick
        Me.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnOK.Location = New System.Drawing.Point(128, 233)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(123, 23)
        Me.btnOK.TabIndex = 3
        Me.btnOK.Text = "OK, send"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnNo
        '
        Me.btnNo.Image = Global.AutoVer.My.Resources.Resources.icon_cancel
        Me.btnNo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNo.Location = New System.Drawing.Point(257, 233)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(123, 23)
        Me.btnNo.TabIndex = 4
        Me.btnNo.Text = "No, don't send"
        Me.btnNo.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(12, 264)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(43, 13)
        Me.lblStatus.TabIndex = 6
        Me.lblStatus.Text = "{status}"
        '
        'txtErrorText
        '
        Me.txtErrorText.BackColor = System.Drawing.SystemColors.Control
        Me.txtErrorText.Location = New System.Drawing.Point(12, 25)
        Me.txtErrorText.Multiline = True
        Me.txtErrorText.Name = "txtErrorText"
        Me.txtErrorText.Size = New System.Drawing.Size(491, 52)
        Me.txtErrorText.TabIndex = 7
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 115)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(129, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Optional extra information:"
        '
        'txtNameEmail
        '
        Me.txtNameEmail.Location = New System.Drawing.Point(38, 148)
        Me.txtNameEmail.Name = "txtNameEmail"
        Me.txtNameEmail.Size = New System.Drawing.Size(465, 20)
        Me.txtNameEmail.TabIndex = 9
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(35, 132)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(90, 13)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Your name/email:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(35, 171)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(335, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "What you did just before this error occured (how to recreate the error):"
        '
        'txtRecreate
        '
        Me.txtRecreate.Location = New System.Drawing.Point(38, 187)
        Me.txtRecreate.Name = "txtRecreate"
        Me.txtRecreate.Size = New System.Drawing.Size(465, 20)
        Me.txtRecreate.TabIndex = 12
        '
        'ErrorReporter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(515, 286)
        Me.Controls.Add(Me.txtRecreate)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtNameEmail)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtErrorText)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.btnNo)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.lklErrorReport)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "ErrorReporter"
        Me.Text = "Application Error"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lklErrorReport As System.Windows.Forms.LinkLabel
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnNo As System.Windows.Forms.Button
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents txtErrorText As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtNameEmail As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtRecreate As System.Windows.Forms.TextBox
End Class
