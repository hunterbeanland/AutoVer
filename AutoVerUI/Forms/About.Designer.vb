<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class About
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
        Me.butOK = New System.Windows.Forms.Button
        Me.lblProduct = New System.Windows.Forms.Label
        Me.lblDescription = New System.Windows.Forms.Label
        Me.lblVersion = New System.Windows.Forms.Label
        Me.lblCopyright = New System.Windows.Forms.Label
        Me.lblBy = New System.Windows.Forms.Label
        Me.lblWebLink = New System.Windows.Forms.LinkLabel
        Me.Label1 = New System.Windows.Forms.Label
        Me.lnkDonate = New System.Windows.Forms.LinkLabel
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.lnkUpdates = New System.Windows.Forms.LinkLabel
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'butOK
        '
        Me.butOK.Image = Global.AutoVer.My.Resources.Resources.icon_tick
        Me.butOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.butOK.Location = New System.Drawing.Point(238, 270)
        Me.butOK.Name = "butOK"
        Me.butOK.Size = New System.Drawing.Size(100, 23)
        Me.butOK.TabIndex = 0
        Me.butOK.Text = "OK"
        Me.butOK.UseVisualStyleBackColor = True
        '
        'lblProduct
        '
        Me.lblProduct.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProduct.Location = New System.Drawing.Point(183, 8)
        Me.lblProduct.Name = "lblProduct"
        Me.lblProduct.Size = New System.Drawing.Size(212, 25)
        Me.lblProduct.TabIndex = 1
        Me.lblProduct.Text = "AutoVer"
        Me.lblProduct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDescription
        '
        Me.lblDescription.Location = New System.Drawing.Point(182, 33)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(215, 17)
        Me.lblDescription.TabIndex = 2
        Me.lblDescription.Text = "Auto Versioning"
        Me.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblVersion
        '
        Me.lblVersion.Location = New System.Drawing.Point(182, 63)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(215, 18)
        Me.lblVersion.TabIndex = 3
        Me.lblVersion.Text = "ver 1.0.0"
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCopyright
        '
        Me.lblCopyright.Location = New System.Drawing.Point(182, 183)
        Me.lblCopyright.Name = "lblCopyright"
        Me.lblCopyright.Size = New System.Drawing.Size(215, 14)
        Me.lblCopyright.TabIndex = 4
        Me.lblCopyright.Text = "Copyright 2007"
        Me.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBy
        '
        Me.lblBy.Location = New System.Drawing.Point(182, 197)
        Me.lblBy.Name = "lblBy"
        Me.lblBy.Size = New System.Drawing.Size(215, 19)
        Me.lblBy.TabIndex = 5
        Me.lblBy.Text = "Written by Hunter Beanland"
        Me.lblBy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblWebLink
        '
        Me.lblWebLink.Location = New System.Drawing.Point(180, 227)
        Me.lblWebLink.Name = "lblWebLink"
        Me.lblWebLink.Size = New System.Drawing.Size(218, 22)
        Me.lblWebLink.TabIndex = 6
        Me.lblWebLink.TabStop = True
        Me.lblWebLink.Text = "www.beanland.net.au/AutoVer"
        Me.lblWebLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(180, 129)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(218, 14)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Freeware"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lnkDonate
        '
        Me.lnkDonate.Location = New System.Drawing.Point(180, 143)
        Me.lnkDonate.Name = "lnkDonate"
        Me.lnkDonate.Size = New System.Drawing.Size(218, 22)
        Me.lnkDonate.TabIndex = 9
        Me.lnkDonate.TabStop = True
        Me.lnkDonate.Text = "Donate"
        Me.lnkDonate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PictureBox1
        '
        Me.PictureBox1.InitialImage = Nothing
        Me.PictureBox1.Location = New System.Drawing.Point(11, 8)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(160, 285)
        Me.PictureBox1.TabIndex = 10
        Me.PictureBox1.TabStop = False
        '
        'lnkUpdates
        '
        Me.lnkUpdates.Location = New System.Drawing.Point(180, 81)
        Me.lnkUpdates.Name = "lnkUpdates"
        Me.lnkUpdates.Size = New System.Drawing.Size(218, 22)
        Me.lnkUpdates.TabIndex = 12
        Me.lnkUpdates.TabStop = True
        Me.lnkUpdates.Text = "Checking for updates..."
        Me.lnkUpdates.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BackgroundWorker1
        '
        '
        'About
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(404, 306)
        Me.Controls.Add(Me.lnkUpdates)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.lnkDonate)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblWebLink)
        Me.Controls.Add(Me.lblBy)
        Me.Controls.Add(Me.lblCopyright)
        Me.Controls.Add(Me.lblVersion)
        Me.Controls.Add(Me.lblDescription)
        Me.Controls.Add(Me.lblProduct)
        Me.Controls.Add(Me.butOK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "About"
        Me.Padding = New System.Windows.Forms.Padding(9)
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "About"
        Me.TopMost = True
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents butOK As System.Windows.Forms.Button
    Friend WithEvents lblProduct As System.Windows.Forms.Label
    Friend WithEvents lblDescription As System.Windows.Forms.Label
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents lblCopyright As System.Windows.Forms.Label
    Friend WithEvents lblBy As System.Windows.Forms.Label
    Friend WithEvents lblWebLink As System.Windows.Forms.LinkLabel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lnkDonate As System.Windows.Forms.LinkLabel
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents lnkUpdates As System.Windows.Forms.LinkLabel
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker

End Class
