<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AppSettings
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
        Me.components = New System.ComponentModel.Container()
        Me.lblCompareApp = New System.Windows.Forms.Label()
        Me.txtApp = New System.Windows.Forms.TextBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.btnCompApp = New System.Windows.Forms.Button()
        Me.chkStartup = New System.Windows.Forms.CheckBox()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.lblTextView = New System.Windows.Forms.Label()
        Me.txtTextViewer = New System.Windows.Forms.TextBox()
        Me.btnTextView = New System.Windows.Forms.Button()
        Me.btnImageView = New System.Windows.Forms.Button()
        Me.txtImageViewer = New System.Windows.Forms.TextBox()
        Me.lblImageView = New System.Windows.Forms.Label()
        Me.btnLog = New System.Windows.Forms.Button()
        Me.chkWMI = New System.Windows.Forms.CheckBox()
        Me.chkDebug = New System.Windows.Forms.CheckBox()
        Me.chkRecycleBin = New System.Windows.Forms.CheckBox()
        Me.chkAutoElevate = New System.Windows.Forms.CheckBox()
        Me.btnConfigFolder = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.lblConfigLoc = New System.Windows.Forms.Label()
        Me.rbConfigCommon = New System.Windows.Forms.RadioButton()
        Me.rbConfigUser = New System.Windows.Forms.RadioButton()
        Me.rbConfigLocal = New System.Windows.Forms.RadioButton()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'lblCompareApp
        '
        Me.lblCompareApp.AutoSize = True
        Me.lblCompareApp.Location = New System.Drawing.Point(13, 23)
        Me.lblCompareApp.Name = "lblCompareApp"
        Me.lblCompareApp.Size = New System.Drawing.Size(74, 13)
        Me.lblCompareApp.TabIndex = 0
        Me.lblCompareApp.Text = "Compare App:"
        '
        'txtApp
        '
        Me.txtApp.Location = New System.Drawing.Point(93, 20)
        Me.txtApp.Name = "txtApp"
        Me.txtApp.Size = New System.Drawing.Size(214, 20)
        Me.txtApp.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtApp, "Path to the compare app such as: C:\Program Files\WinMerge\WinMergeU.exe ""{0}"" ""{" & _
        "1}""")
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'btnCompApp
        '
        Me.btnCompApp.Location = New System.Drawing.Point(313, 18)
        Me.btnCompApp.Name = "btnCompApp"
        Me.btnCompApp.Size = New System.Drawing.Size(26, 23)
        Me.btnCompApp.TabIndex = 2
        Me.btnCompApp.Text = "..."
        Me.btnCompApp.UseVisualStyleBackColor = True
        '
        'chkStartup
        '
        Me.chkStartup.AutoSize = True
        Me.chkStartup.Checked = True
        Me.chkStartup.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStartup.Location = New System.Drawing.Point(16, 130)
        Me.chkStartup.Name = "chkStartup"
        Me.chkStartup.Size = New System.Drawing.Size(161, 17)
        Me.chkStartup.TabIndex = 7
        Me.chkStartup.Text = "Load UI on Windows startup"
        Me.ToolTip1.SetToolTip(Me.chkStartup, "Load automatically when Windows starts (default)")
        Me.chkStartup.UseVisualStyleBackColor = True
        '
        'btnHelp
        '
        Me.btnHelp.Image = Global.AutoVer.My.Resources.Resources.help
        Me.btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnHelp.Location = New System.Drawing.Point(12, 259)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(100, 23)
        Me.btnHelp.TabIndex = 11
        Me.btnHelp.Text = "Help"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'lblTextView
        '
        Me.lblTextView.AutoSize = True
        Me.lblTextView.Location = New System.Drawing.Point(13, 51)
        Me.lblTextView.Name = "lblTextView"
        Me.lblTextView.Size = New System.Drawing.Size(66, 13)
        Me.lblTextView.TabIndex = 7
        Me.lblTextView.Text = "Text Viewer:"
        '
        'txtTextViewer
        '
        Me.txtTextViewer.Location = New System.Drawing.Point(93, 48)
        Me.txtTextViewer.Name = "txtTextViewer"
        Me.txtTextViewer.Size = New System.Drawing.Size(214, 20)
        Me.txtTextViewer.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txtTextViewer, "Path to the text viewer app such as: C:\Program Files (x86)\RJ TextEd\TextEd.exe")
        '
        'btnTextView
        '
        Me.btnTextView.Location = New System.Drawing.Point(313, 45)
        Me.btnTextView.Name = "btnTextView"
        Me.btnTextView.Size = New System.Drawing.Size(26, 23)
        Me.btnTextView.TabIndex = 4
        Me.btnTextView.Text = "..."
        Me.btnTextView.UseVisualStyleBackColor = True
        '
        'btnImageView
        '
        Me.btnImageView.Location = New System.Drawing.Point(313, 71)
        Me.btnImageView.Name = "btnImageView"
        Me.btnImageView.Size = New System.Drawing.Size(26, 23)
        Me.btnImageView.TabIndex = 6
        Me.btnImageView.Text = "..."
        Me.btnImageView.UseVisualStyleBackColor = True
        '
        'txtImageViewer
        '
        Me.txtImageViewer.Location = New System.Drawing.Point(93, 74)
        Me.txtImageViewer.Name = "txtImageViewer"
        Me.txtImageViewer.Size = New System.Drawing.Size(214, 20)
        Me.txtImageViewer.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.txtImageViewer, "Path to the image viewer app such as: C:\Program Files (x86)\IrfanView\i_view32.e" & _
        "xe")
        '
        'lblImageView
        '
        Me.lblImageView.AutoSize = True
        Me.lblImageView.Location = New System.Drawing.Point(13, 77)
        Me.lblImageView.Name = "lblImageView"
        Me.lblImageView.Size = New System.Drawing.Size(74, 13)
        Me.lblImageView.TabIndex = 10
        Me.lblImageView.Text = "Image Viewer:"
        '
        'btnLog
        '
        Me.btnLog.Image = Global.AutoVer.My.Resources.Resources.icon_detail
        Me.btnLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnLog.Location = New System.Drawing.Point(224, 218)
        Me.btnLog.Name = "btnLog"
        Me.btnLog.Size = New System.Drawing.Size(100, 23)
        Me.btnLog.TabIndex = 10
        Me.btnLog.Text = "View Log"
        Me.btnLog.UseVisualStyleBackColor = True
        '
        'chkWMI
        '
        Me.chkWMI.AutoSize = True
        Me.chkWMI.Checked = True
        Me.chkWMI.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkWMI.Location = New System.Drawing.Point(16, 176)
        Me.chkWMI.Name = "chkWMI"
        Me.chkWMI.Size = New System.Drawing.Size(85, 17)
        Me.chkWMI.TabIndex = 12
        Me.chkWMI.Text = "Enable WMI"
        Me.ToolTip1.SetToolTip(Me.chkWMI, "Use WMI for drive state detection (default). Otherwise it will poll the drives")
        Me.chkWMI.UseVisualStyleBackColor = True
        '
        'chkDebug
        '
        Me.chkDebug.AutoSize = True
        Me.chkDebug.Location = New System.Drawing.Point(16, 222)
        Me.chkDebug.Name = "chkDebug"
        Me.chkDebug.Size = New System.Drawing.Size(99, 17)
        Me.chkDebug.TabIndex = 13
        Me.chkDebug.Text = "Debug Logging"
        Me.ToolTip1.SetToolTip(Me.chkDebug, "Increase the logging level to be more verbose (if trying to solve an issue)")
        Me.chkDebug.UseVisualStyleBackColor = True
        '
        'chkRecycleBin
        '
        Me.chkRecycleBin.AutoSize = True
        Me.chkRecycleBin.Location = New System.Drawing.Point(16, 199)
        Me.chkRecycleBin.Name = "chkRecycleBin"
        Me.chkRecycleBin.Size = New System.Drawing.Size(142, 17)
        Me.chkRecycleBin.TabIndex = 14
        Me.chkRecycleBin.Text = "Delete uses Recycle Bin"
        Me.ToolTip1.SetToolTip(Me.chkRecycleBin, "When files are deleted, use the recycle bin (slower). Otherwise permanently delet" & _
        "e (default).")
        Me.chkRecycleBin.UseVisualStyleBackColor = True
        '
        'chkAutoElevate
        '
        Me.chkAutoElevate.AutoSize = True
        Me.chkAutoElevate.Checked = True
        Me.chkAutoElevate.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkAutoElevate.Location = New System.Drawing.Point(16, 153)
        Me.chkAutoElevate.Name = "chkAutoElevate"
        Me.chkAutoElevate.Size = New System.Drawing.Size(175, 17)
        Me.chkAutoElevate.TabIndex = 15
        Me.chkAutoElevate.Text = "Auto-Elevate UI without service"
        Me.ToolTip1.SetToolTip(Me.chkAutoElevate, "If not running the service or as Admin, elevate to Admin to run (may prompt you o" & _
        "n each start)")
        Me.chkAutoElevate.UseVisualStyleBackColor = True
        '
        'btnConfigFolder
        '
        Me.btnConfigFolder.Image = Global.AutoVer.My.Resources.Resources.icon_doc
        Me.btnConfigFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnConfigFolder.Location = New System.Drawing.Point(224, 189)
        Me.btnConfigFolder.Name = "btnConfigFolder"
        Me.btnConfigFolder.Size = New System.Drawing.Size(100, 23)
        Me.btnConfigFolder.TabIndex = 16
        Me.btnConfigFolder.Text = "Config Folder"
        Me.btnConfigFolder.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Image = Global.AutoVer.My.Resources.Resources.icon_cancel
        Me.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCancel.Location = New System.Drawing.Point(224, 259)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 23)
        Me.btnCancel.TabIndex = 9
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Image = Global.AutoVer.My.Resources.Resources.icon_tick
        Me.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnOK.Location = New System.Drawing.Point(118, 259)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(100, 23)
        Me.btnOK.TabIndex = 8
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'lblConfigLoc
        '
        Me.lblConfigLoc.AutoSize = True
        Me.lblConfigLoc.Location = New System.Drawing.Point(13, 107)
        Me.lblConfigLoc.Name = "lblConfigLoc"
        Me.lblConfigLoc.Size = New System.Drawing.Size(84, 13)
        Me.lblConfigLoc.TabIndex = 17
        Me.lblConfigLoc.Text = "Config Location:"
        '
        'rbConfigCommon
        '
        Me.rbConfigCommon.AutoSize = True
        Me.rbConfigCommon.Checked = True
        Me.rbConfigCommon.Location = New System.Drawing.Point(103, 105)
        Me.rbConfigCommon.Name = "rbConfigCommon"
        Me.rbConfigCommon.Size = New System.Drawing.Size(66, 17)
        Me.rbConfigCommon.TabIndex = 18
        Me.rbConfigCommon.TabStop = True
        Me.rbConfigCommon.Text = "Common"
        Me.ToolTip1.SetToolTip(Me.rbConfigCommon, "Settings are common to all Windows users (recommended default)")
        Me.rbConfigCommon.UseVisualStyleBackColor = True
        '
        'rbConfigUser
        '
        Me.rbConfigUser.AutoSize = True
        Me.rbConfigUser.Location = New System.Drawing.Point(171, 105)
        Me.rbConfigUser.Name = "rbConfigUser"
        Me.rbConfigUser.Size = New System.Drawing.Size(47, 17)
        Me.rbConfigUser.TabIndex = 19
        Me.rbConfigUser.Text = "User"
        Me.ToolTip1.SetToolTip(Me.rbConfigUser, "Settings apply only to the current Windows user")
        Me.rbConfigUser.UseVisualStyleBackColor = True
        '
        'rbConfigLocal
        '
        Me.rbConfigLocal.AutoSize = True
        Me.rbConfigLocal.Location = New System.Drawing.Point(223, 105)
        Me.rbConfigLocal.Name = "rbConfigLocal"
        Me.rbConfigLocal.Size = New System.Drawing.Size(95, 17)
        Me.rbConfigLocal.TabIndex = 20
        Me.rbConfigLocal.Text = "Local/Portable"
        Me.ToolTip1.SetToolTip(Me.rbConfigLocal, "Settings are stored with the AutoVer executable program (If running as a portable" & _
        " app)")
        Me.rbConfigLocal.UseVisualStyleBackColor = True
        '
        'AppSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(347, 294)
        Me.Controls.Add(Me.rbConfigLocal)
        Me.Controls.Add(Me.rbConfigUser)
        Me.Controls.Add(Me.rbConfigCommon)
        Me.Controls.Add(Me.lblConfigLoc)
        Me.Controls.Add(Me.btnConfigFolder)
        Me.Controls.Add(Me.chkAutoElevate)
        Me.Controls.Add(Me.chkRecycleBin)
        Me.Controls.Add(Me.chkDebug)
        Me.Controls.Add(Me.chkWMI)
        Me.Controls.Add(Me.btnLog)
        Me.Controls.Add(Me.btnImageView)
        Me.Controls.Add(Me.txtImageViewer)
        Me.Controls.Add(Me.lblImageView)
        Me.Controls.Add(Me.btnTextView)
        Me.Controls.Add(Me.txtTextViewer)
        Me.Controls.Add(Me.lblTextView)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.chkStartup)
        Me.Controls.Add(Me.btnCompApp)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.txtApp)
        Me.Controls.Add(Me.lblCompareApp)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "AppSettings"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "AutoVer Settings"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblCompareApp As System.Windows.Forms.Label
    Friend WithEvents txtApp As System.Windows.Forms.TextBox
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnCompApp As System.Windows.Forms.Button
    Friend WithEvents chkStartup As System.Windows.Forms.CheckBox
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents lblTextView As System.Windows.Forms.Label
    Friend WithEvents txtTextViewer As System.Windows.Forms.TextBox
    Friend WithEvents btnTextView As System.Windows.Forms.Button
    Friend WithEvents btnImageView As System.Windows.Forms.Button
    Friend WithEvents txtImageViewer As System.Windows.Forms.TextBox
    Friend WithEvents lblImageView As System.Windows.Forms.Label
    Friend WithEvents btnLog As System.Windows.Forms.Button
    Friend WithEvents chkWMI As System.Windows.Forms.CheckBox
    Friend WithEvents chkDebug As System.Windows.Forms.CheckBox
    Friend WithEvents chkRecycleBin As System.Windows.Forms.CheckBox
    Friend WithEvents chkAutoElevate As System.Windows.Forms.CheckBox
    Friend WithEvents btnConfigFolder As System.Windows.Forms.Button
    Friend WithEvents lblConfigLoc As System.Windows.Forms.Label
    Friend WithEvents rbConfigCommon As System.Windows.Forms.RadioButton
    Friend WithEvents rbConfigUser As System.Windows.Forms.RadioButton
    Friend WithEvents rbConfigLocal As System.Windows.Forms.RadioButton
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
End Class
