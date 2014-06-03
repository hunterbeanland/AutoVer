<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Settings
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
        Me.tabControl = New System.Windows.Forms.TabControl()
        Me.tabGen = New System.Windows.Forms.TabPage()
        Me.chkShowEvents = New System.Windows.Forms.CheckBox()
        Me.chkSubFolders = New System.Windows.Forms.CheckBox()
        Me.chkShowErrors = New System.Windows.Forms.CheckBox()
        Me.lblStats = New System.Windows.Forms.Label()
        Me.lblCounts = New System.Windows.Forms.Label()
        Me.chkEnabled = New System.Windows.Forms.CheckBox()
        Me.grpType = New System.Windows.Forms.GroupBox()
        Me.rbFolder = New System.Windows.Forms.RadioButton()
        Me.rbFTP = New System.Windows.Forms.RadioButton()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.chkBackup = New System.Windows.Forms.CheckBox()
        Me.butBackupFolder = New System.Windows.Forms.Button()
        Me.btnWatchFolder = New System.Windows.Forms.Button()
        Me.txtWatch = New System.Windows.Forms.TextBox()
        Me.txtBackup = New System.Windows.Forms.TextBox()
        Me.lblBackFold = New System.Windows.Forms.Label()
        Me.lblWatchFold = New System.Windows.Forms.Label()
        Me.tabAdv = New System.Windows.Forms.TabPage()
        Me.chkCompareBeforeCopy = New System.Windows.Forms.CheckBox()
        Me.txtSettleDelay = New System.Windows.Forms.TextBox()
        Me.lblSettle = New System.Windows.Forms.Label()
        Me.chkRunCopyFirst = New System.Windows.Forms.CheckBox()
        Me.lblRunCopy = New System.Windows.Forms.Label()
        Me.txtRunCopy = New System.Windows.Forms.TextBox()
        Me.txtEnsureTime = New System.Windows.Forms.TextBox()
        Me.rbEnsureHourly = New System.Windows.Forms.RadioButton()
        Me.rbEnsureDaily = New System.Windows.Forms.RadioButton()
        Me.rbEnsureNever = New System.Windows.Forms.RadioButton()
        Me.lblSync = New System.Windows.Forms.Label()
        Me.lblFoldSep = New System.Windows.Forms.Label()
        Me.lblFileSep = New System.Windows.Forms.Label()
        Me.txtMaxFileSize = New System.Windows.Forms.TextBox()
        Me.lblMaxFileSize = New System.Windows.Forms.Label()
        Me.txtExcludeFiles = New System.Windows.Forms.TextBox()
        Me.lblExclFiles = New System.Windows.Forms.Label()
        Me.txtExcludeFolders = New System.Windows.Forms.TextBox()
        Me.lblExclFold = New System.Windows.Forms.Label()
        Me.txtIncludeFiles = New System.Windows.Forms.TextBox()
        Me.lblIncFiles = New System.Windows.Forms.Label()
        Me.chkDelete = New System.Windows.Forms.CheckBox()
        Me.tabVer = New System.Windows.Forms.TabPage()
        Me.grpArc = New System.Windows.Forms.GroupBox()
        Me.lblVersOlder = New System.Windows.Forms.Label()
        Me.txtMaxVersionAge = New System.Windows.Forms.TextBox()
        Me.lblVerDays = New System.Windows.Forms.Label()
        Me.radNothing = New System.Windows.Forms.RadioButton()
        Me.radDelete = New System.Windows.Forms.RadioButton()
        Me.radZip = New System.Windows.Forms.RadioButton()
        Me.rad7Zip = New System.Windows.Forms.RadioButton()
        Me.grpArcTo = New System.Windows.Forms.GroupBox()
        Me.rbZipModeF = New System.Windows.Forms.RadioButton()
        Me.rbZipModeD = New System.Windows.Forms.RadioButton()
        Me.rbZipModeW = New System.Windows.Forms.RadioButton()
        Me.lblZip1 = New System.Windows.Forms.Label()
        Me.grpVerMax = New System.Windows.Forms.GroupBox()
        Me.txtVerRateS = New System.Windows.Forms.TextBox()
        Me.lblHourMinSec = New System.Windows.Forms.Label()
        Me.txtVerRateM = New System.Windows.Forms.TextBox()
        Me.txtVerRateH = New System.Windows.Forms.TextBox()
        Me.rbVerRate = New System.Windows.Forms.RadioButton()
        Me.rbVerAll = New System.Windows.Forms.RadioButton()
        Me.grpVerMode = New System.Windows.Forms.GroupBox()
        Me.rbVersionPrev = New System.Windows.Forms.RadioButton()
        Me.rbVersionAll = New System.Windows.Forms.RadioButton()
        Me.rbNone = New System.Windows.Forms.RadioButton()
        Me.lblTimeStamp = New System.Windows.Forms.Label()
        Me.txtDateTimeStamp = New System.Windows.Forms.TextBox()
        Me.tabFTP = New System.Windows.Forms.TabPage()
        Me.chkFTPPassive = New System.Windows.Forms.CheckBox()
        Me.lblFtpAnon = New System.Windows.Forms.Label()
        Me.lblFtpPass = New System.Windows.Forms.Label()
        Me.txtFTPPassword = New System.Windows.Forms.TextBox()
        Me.lblFtpUser = New System.Windows.Forms.Label()
        Me.txtFTPLogin = New System.Windows.Forms.TextBox()
        Me.lblFtpHost = New System.Windows.Forms.Label()
        Me.txtFTPHost = New System.Windows.Forms.TextBox()
        Me.lblFtpIf = New System.Windows.Forms.Label()
        Me.butCancel = New System.Windows.Forms.Button()
        Me.butOK = New System.Windows.Forms.Button()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.HelpProvider1 = New System.Windows.Forms.HelpProvider()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.tmrEnsureSyncUpdates = New System.Windows.Forms.Timer(Me.components)
        Me.bwBackupRestore = New System.ComponentModel.BackgroundWorker()
        Me.tabControl.SuspendLayout()
        Me.tabGen.SuspendLayout()
        Me.grpType.SuspendLayout()
        Me.tabAdv.SuspendLayout()
        Me.tabVer.SuspendLayout()
        Me.grpArc.SuspendLayout()
        Me.grpArcTo.SuspendLayout()
        Me.grpVerMax.SuspendLayout()
        Me.grpVerMode.SuspendLayout()
        Me.tabFTP.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabControl
        '
        Me.tabControl.Controls.Add(Me.tabGen)
        Me.tabControl.Controls.Add(Me.tabAdv)
        Me.tabControl.Controls.Add(Me.tabVer)
        Me.tabControl.Controls.Add(Me.tabFTP)
        Me.tabControl.Location = New System.Drawing.Point(12, 12)
        Me.tabControl.Name = "tabControl"
        Me.tabControl.SelectedIndex = 0
        Me.tabControl.Size = New System.Drawing.Size(381, 308)
        Me.tabControl.TabIndex = 0
        '
        'tabGen
        '
        Me.tabGen.Controls.Add(Me.chkShowEvents)
        Me.tabGen.Controls.Add(Me.chkSubFolders)
        Me.tabGen.Controls.Add(Me.chkShowErrors)
        Me.tabGen.Controls.Add(Me.lblStats)
        Me.tabGen.Controls.Add(Me.lblCounts)
        Me.tabGen.Controls.Add(Me.chkEnabled)
        Me.tabGen.Controls.Add(Me.grpType)
        Me.tabGen.Controls.Add(Me.lblName)
        Me.tabGen.Controls.Add(Me.txtName)
        Me.tabGen.Controls.Add(Me.chkBackup)
        Me.tabGen.Controls.Add(Me.butBackupFolder)
        Me.tabGen.Controls.Add(Me.btnWatchFolder)
        Me.tabGen.Controls.Add(Me.txtWatch)
        Me.tabGen.Controls.Add(Me.txtBackup)
        Me.tabGen.Controls.Add(Me.lblBackFold)
        Me.tabGen.Controls.Add(Me.lblWatchFold)
        Me.tabGen.Location = New System.Drawing.Point(4, 22)
        Me.tabGen.Name = "tabGen"
        Me.tabGen.Padding = New System.Windows.Forms.Padding(3)
        Me.tabGen.Size = New System.Drawing.Size(373, 282)
        Me.tabGen.TabIndex = 0
        Me.tabGen.Text = "General"
        Me.tabGen.UseVisualStyleBackColor = True
        '
        'chkShowEvents
        '
        Me.chkShowEvents.AutoSize = True
        Me.chkShowEvents.Location = New System.Drawing.Point(6, 205)
        Me.chkShowEvents.Name = "chkShowEvents"
        Me.chkShowEvents.Size = New System.Drawing.Size(143, 17)
        Me.chkShowEvents.TabIndex = 18
        Me.chkShowEvents.Text = "Show on all other events"
        Me.ToolTip1.SetToolTip(Me.chkShowEvents, "Show balloon alert in the System Tray on each copy/rename/delete event")
        Me.chkShowEvents.UseVisualStyleBackColor = True
        '
        'chkSubFolders
        '
        Me.chkSubFolders.AutoSize = True
        Me.chkSubFolders.Checked = True
        Me.chkSubFolders.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSubFolders.Location = New System.Drawing.Point(6, 136)
        Me.chkSubFolders.Name = "chkSubFolders"
        Me.chkSubFolders.Size = New System.Drawing.Size(115, 17)
        Me.chkSubFolders.TabIndex = 17
        Me.chkSubFolders.Text = "Include sub folders"
        Me.chkSubFolders.UseVisualStyleBackColor = True
        '
        'chkShowErrors
        '
        Me.chkShowErrors.AutoSize = True
        Me.chkShowErrors.Location = New System.Drawing.Point(6, 182)
        Me.chkShowErrors.Name = "chkShowErrors"
        Me.chkShowErrors.Size = New System.Drawing.Size(120, 17)
        Me.chkShowErrors.TabIndex = 16
        Me.chkShowErrors.Text = "Show alert on errors"
        Me.ToolTip1.SetToolTip(Me.chkShowErrors, "Show balloon alert in the System Tray on each error")
        Me.chkShowErrors.UseVisualStyleBackColor = True
        '
        'lblStats
        '
        Me.lblStats.AutoSize = True
        Me.lblStats.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStats.Location = New System.Drawing.Point(272, 220)
        Me.lblStats.Name = "lblStats"
        Me.lblStats.Size = New System.Drawing.Size(88, 13)
        Me.lblStats.TabIndex = 15
        Me.lblStats.Text = "Session Stats:"
        '
        'lblCounts
        '
        Me.lblCounts.AutoSize = True
        Me.lblCounts.Location = New System.Drawing.Point(275, 233)
        Me.lblCounts.Name = "lblCounts"
        Me.lblCounts.Size = New System.Drawing.Size(65, 39)
        Me.lblCounts.TabIndex = 14
        Me.lblCounts.Text = "Changed: 0 " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Renamed: 0" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Deleted: 0"
        Me.ToolTip1.SetToolTip(Me.lblCounts, "File statistics since AutoVer last started")
        '
        'chkEnabled
        '
        Me.chkEnabled.AutoSize = True
        Me.chkEnabled.Checked = True
        Me.chkEnabled.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkEnabled.Location = New System.Drawing.Point(298, 17)
        Me.chkEnabled.Name = "chkEnabled"
        Me.chkEnabled.Size = New System.Drawing.Size(65, 17)
        Me.chkEnabled.TabIndex = 1
        Me.chkEnabled.Text = "Enabled"
        Me.ToolTip1.SetToolTip(Me.chkEnabled, "Enable or disable the watcher")
        Me.chkEnabled.UseVisualStyleBackColor = True
        '
        'grpType
        '
        Me.grpType.Controls.Add(Me.rbFolder)
        Me.grpType.Controls.Add(Me.rbFTP)
        Me.grpType.Location = New System.Drawing.Point(80, 94)
        Me.grpType.Name = "grpType"
        Me.grpType.Size = New System.Drawing.Size(150, 36)
        Me.grpType.TabIndex = 12
        Me.grpType.TabStop = False
        Me.grpType.Text = "Backup Type"
        Me.ToolTip1.SetToolTip(Me.grpType, "Folder or FTP address selected in ""Backup to"" above")
        '
        'rbFolder
        '
        Me.rbFolder.AutoSize = True
        Me.rbFolder.Checked = True
        Me.rbFolder.Location = New System.Drawing.Point(21, 13)
        Me.rbFolder.Name = "rbFolder"
        Me.rbFolder.Size = New System.Drawing.Size(54, 17)
        Me.rbFolder.TabIndex = 6
        Me.rbFolder.TabStop = True
        Me.rbFolder.Text = "Folder"
        Me.ToolTip1.SetToolTip(Me.rbFolder, "Backup to any Windows accessable folder or drive")
        Me.rbFolder.UseVisualStyleBackColor = True
        '
        'rbFTP
        '
        Me.rbFTP.AutoSize = True
        Me.rbFTP.Location = New System.Drawing.Point(89, 13)
        Me.rbFTP.Name = "rbFTP"
        Me.rbFTP.Size = New System.Drawing.Size(45, 17)
        Me.rbFTP.TabIndex = 6
        Me.rbFTP.TabStop = True
        Me.rbFTP.Text = "FTP"
        Me.ToolTip1.SetToolTip(Me.rbFTP, "Ensure the FTP details are filled in on the FTP tab")
        Me.rbFTP.UseVisualStyleBackColor = True
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(3, 18)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(38, 13)
        Me.lblName.TabIndex = 9
        Me.lblName.Text = "Name:"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(81, 15)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(211, 20)
        Me.txtName.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.txtName, "Name for this watcher setting")
        '
        'chkBackup
        '
        Me.chkBackup.AutoSize = True
        Me.chkBackup.Location = New System.Drawing.Point(6, 159)
        Me.chkBackup.Name = "chkBackup"
        Me.chkBackup.Size = New System.Drawing.Size(122, 17)
        Me.chkBackup.TabIndex = 7
        Me.chkBackup.Text = "Create initial backup"
        Me.ToolTip1.SetToolTip(Me.chkBackup, "Create a full backup now")
        Me.chkBackup.UseVisualStyleBackColor = True
        '
        'butBackupFolder
        '
        Me.butBackupFolder.Location = New System.Drawing.Point(297, 66)
        Me.butBackupFolder.Name = "butBackupFolder"
        Me.butBackupFolder.Size = New System.Drawing.Size(26, 23)
        Me.butBackupFolder.TabIndex = 5
        Me.butBackupFolder.Text = "..."
        Me.butBackupFolder.UseVisualStyleBackColor = True
        '
        'btnWatchFolder
        '
        Me.btnWatchFolder.Location = New System.Drawing.Point(297, 41)
        Me.btnWatchFolder.Name = "btnWatchFolder"
        Me.btnWatchFolder.Size = New System.Drawing.Size(26, 23)
        Me.btnWatchFolder.TabIndex = 3
        Me.btnWatchFolder.Text = "..."
        Me.btnWatchFolder.UseVisualStyleBackColor = True
        '
        'txtWatch
        '
        Me.txtWatch.Location = New System.Drawing.Point(80, 41)
        Me.txtWatch.Name = "txtWatch"
        Me.txtWatch.Size = New System.Drawing.Size(211, 20)
        Me.txtWatch.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtWatch, "Absolute or relative path (to AutoVer) to the files and folders to watch for back" & _
        "up ")
        '
        'txtBackup
        '
        Me.txtBackup.Location = New System.Drawing.Point(80, 68)
        Me.txtBackup.Name = "txtBackup"
        Me.txtBackup.Size = New System.Drawing.Size(211, 20)
        Me.txtBackup.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txtBackup, "Absolute or relative path (to watch folder) to the backup folder. May be an FTP a" & _
        "ddress")
        '
        'lblBackFold
        '
        Me.lblBackFold.AutoSize = True
        Me.lblBackFold.Location = New System.Drawing.Point(3, 71)
        Me.lblBackFold.Name = "lblBackFold"
        Me.lblBackFold.Size = New System.Drawing.Size(59, 13)
        Me.lblBackFold.TabIndex = 1
        Me.lblBackFold.Text = "Backup to:"
        '
        'lblWatchFold
        '
        Me.lblWatchFold.AutoSize = True
        Me.lblWatchFold.Location = New System.Drawing.Point(3, 44)
        Me.lblWatchFold.Name = "lblWatchFold"
        Me.lblWatchFold.Size = New System.Drawing.Size(71, 13)
        Me.lblWatchFold.TabIndex = 0
        Me.lblWatchFold.Text = "Watch folder:"
        '
        'tabAdv
        '
        Me.tabAdv.Controls.Add(Me.chkCompareBeforeCopy)
        Me.tabAdv.Controls.Add(Me.txtSettleDelay)
        Me.tabAdv.Controls.Add(Me.lblSettle)
        Me.tabAdv.Controls.Add(Me.chkRunCopyFirst)
        Me.tabAdv.Controls.Add(Me.lblRunCopy)
        Me.tabAdv.Controls.Add(Me.txtRunCopy)
        Me.tabAdv.Controls.Add(Me.txtEnsureTime)
        Me.tabAdv.Controls.Add(Me.rbEnsureHourly)
        Me.tabAdv.Controls.Add(Me.rbEnsureDaily)
        Me.tabAdv.Controls.Add(Me.rbEnsureNever)
        Me.tabAdv.Controls.Add(Me.lblSync)
        Me.tabAdv.Controls.Add(Me.lblFoldSep)
        Me.tabAdv.Controls.Add(Me.lblFileSep)
        Me.tabAdv.Controls.Add(Me.txtMaxFileSize)
        Me.tabAdv.Controls.Add(Me.lblMaxFileSize)
        Me.tabAdv.Controls.Add(Me.txtExcludeFiles)
        Me.tabAdv.Controls.Add(Me.lblExclFiles)
        Me.tabAdv.Controls.Add(Me.txtExcludeFolders)
        Me.tabAdv.Controls.Add(Me.lblExclFold)
        Me.tabAdv.Controls.Add(Me.txtIncludeFiles)
        Me.tabAdv.Controls.Add(Me.lblIncFiles)
        Me.tabAdv.Controls.Add(Me.chkDelete)
        Me.tabAdv.Location = New System.Drawing.Point(4, 22)
        Me.tabAdv.Name = "tabAdv"
        Me.tabAdv.Padding = New System.Windows.Forms.Padding(3)
        Me.tabAdv.Size = New System.Drawing.Size(373, 282)
        Me.tabAdv.TabIndex = 1
        Me.tabAdv.Text = "Advanced"
        Me.tabAdv.UseVisualStyleBackColor = True
        '
        'chkCompareBeforeCopy
        '
        Me.chkCompareBeforeCopy.AutoSize = True
        Me.chkCompareBeforeCopy.Location = New System.Drawing.Point(6, 173)
        Me.chkCompareBeforeCopy.Name = "chkCompareBeforeCopy"
        Me.chkCompareBeforeCopy.Size = New System.Drawing.Size(165, 17)
        Me.chkCompareBeforeCopy.TabIndex = 37
        Me.chkCompareBeforeCopy.Text = "Only copy if content changed"
        Me.ToolTip1.SetToolTip(Me.chkCompareBeforeCopy, "If not checked, AutoVer will copy the file every time it is saved - even if the f" & _
        "ile content did not change. Checking for changes slows performance.")
        Me.chkCompareBeforeCopy.UseVisualStyleBackColor = True
        '
        'txtSettleDelay
        '
        Me.txtSettleDelay.Location = New System.Drawing.Point(87, 124)
        Me.txtSettleDelay.MaxLength = 4
        Me.txtSettleDelay.Name = "txtSettleDelay"
        Me.txtSettleDelay.Size = New System.Drawing.Size(40, 20)
        Me.txtSettleDelay.TabIndex = 29
        Me.txtSettleDelay.Text = "1"
        Me.ToolTip1.SetToolTip(Me.txtSettleDelay, "Time in seconds after a change to a file before the copy/rename etc is done. (0.0" & _
        "1 to 300s) ")
        '
        'lblSettle
        '
        Me.lblSettle.AutoSize = True
        Me.lblSettle.Location = New System.Drawing.Point(4, 127)
        Me.lblSettle.Name = "lblSettle"
        Me.lblSettle.Size = New System.Drawing.Size(71, 13)
        Me.lblSettle.TabIndex = 28
        Me.lblSettle.Text = "Settling Time:"
        '
        'chkRunCopyFirst
        '
        Me.chkRunCopyFirst.AutoSize = True
        Me.chkRunCopyFirst.Checked = True
        Me.chkRunCopyFirst.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkRunCopyFirst.Location = New System.Drawing.Point(280, 104)
        Me.chkRunCopyFirst.Name = "chkRunCopyFirst"
        Me.chkRunCopyFirst.Size = New System.Drawing.Size(85, 17)
        Me.chkRunCopyFirst.TabIndex = 27
        Me.chkRunCopyFirst.Text = "Copy file first"
        Me.ToolTip1.SetToolTip(Me.chkRunCopyFirst, "Copy the file before running the specified app. Unticked = no copying done (your " & _
        "app to do)")
        Me.chkRunCopyFirst.UseVisualStyleBackColor = True
        '
        'lblRunCopy
        '
        Me.lblRunCopy.AutoSize = True
        Me.lblRunCopy.Location = New System.Drawing.Point(4, 105)
        Me.lblRunCopy.Name = "lblRunCopy"
        Me.lblRunCopy.Size = New System.Drawing.Size(71, 13)
        Me.lblRunCopy.TabIndex = 26
        Me.lblRunCopy.Text = "Run on copy:"
        '
        'txtRunCopy
        '
        Me.txtRunCopy.Location = New System.Drawing.Point(87, 102)
        Me.txtRunCopy.Name = "txtRunCopy"
        Me.txtRunCopy.Size = New System.Drawing.Size(187, 20)
        Me.txtRunCopy.TabIndex = 25
        Me.ToolTip1.SetToolTip(Me.txtRunCopy, "Run this app on each file change/copy. {0} = original file, {1} = Backup file")
        '
        'txtEnsureTime
        '
        Me.txtEnsureTime.Location = New System.Drawing.Point(247, 195)
        Me.txtEnsureTime.MaxLength = 5
        Me.txtEnsureTime.Name = "txtEnsureTime"
        Me.txtEnsureTime.Size = New System.Drawing.Size(40, 20)
        Me.txtEnsureTime.TabIndex = 24
        Me.ToolTip1.SetToolTip(Me.txtEnsureTime, "Specifiy the time of day in 24 hour format (ie 23:00) or leave blank for auto.")
        '
        'rbEnsureHourly
        '
        Me.rbEnsureHourly.AutoSize = True
        Me.rbEnsureHourly.Location = New System.Drawing.Point(303, 196)
        Me.rbEnsureHourly.Name = "rbEnsureHourly"
        Me.rbEnsureHourly.Size = New System.Drawing.Size(55, 17)
        Me.rbEnsureHourly.TabIndex = 23
        Me.rbEnsureHourly.Text = "Hourly"
        Me.rbEnsureHourly.UseVisualStyleBackColor = True
        '
        'rbEnsureDaily
        '
        Me.rbEnsureDaily.AutoSize = True
        Me.rbEnsureDaily.Checked = True
        Me.rbEnsureDaily.Location = New System.Drawing.Point(200, 196)
        Me.rbEnsureDaily.Name = "rbEnsureDaily"
        Me.rbEnsureDaily.Size = New System.Drawing.Size(51, 17)
        Me.rbEnsureDaily.TabIndex = 22
        Me.rbEnsureDaily.TabStop = True
        Me.rbEnsureDaily.Text = "Daily:"
        Me.rbEnsureDaily.UseVisualStyleBackColor = True
        '
        'rbEnsureNever
        '
        Me.rbEnsureNever.AutoSize = True
        Me.rbEnsureNever.Location = New System.Drawing.Point(139, 196)
        Me.rbEnsureNever.Name = "rbEnsureNever"
        Me.rbEnsureNever.Size = New System.Drawing.Size(54, 17)
        Me.rbEnsureNever.TabIndex = 21
        Me.rbEnsureNever.Text = "Never"
        Me.rbEnsureNever.UseVisualStyleBackColor = True
        '
        'lblSync
        '
        Me.lblSync.AutoSize = True
        Me.lblSync.Location = New System.Drawing.Point(5, 198)
        Me.lblSync.Name = "lblSync"
        Me.lblSync.Size = New System.Drawing.Size(107, 13)
        Me.lblSync.TabIndex = 20
        Me.lblSync.Text = "Synchronise backup:"
        '
        'lblFoldSep
        '
        Me.lblFoldSep.AutoSize = True
        Me.lblFoldSep.Location = New System.Drawing.Point(295, 61)
        Me.lblFoldSep.Name = "lblFoldSep"
        Me.lblFoldSep.Size = New System.Drawing.Size(65, 13)
        Me.lblFoldSep.TabIndex = 19
        Me.lblFoldSep.Text = "(; separates)"
        '
        'lblFileSep
        '
        Me.lblFileSep.AutoSize = True
        Me.lblFileSep.Location = New System.Drawing.Point(295, 39)
        Me.lblFileSep.Name = "lblFileSep"
        Me.lblFileSep.Size = New System.Drawing.Size(65, 13)
        Me.lblFileSep.TabIndex = 18
        Me.lblFileSep.Text = "(; separates)"
        '
        'txtMaxFileSize
        '
        Me.txtMaxFileSize.Location = New System.Drawing.Point(87, 80)
        Me.txtMaxFileSize.Name = "txtMaxFileSize"
        Me.txtMaxFileSize.Size = New System.Drawing.Size(92, 20)
        Me.txtMaxFileSize.TabIndex = 4
        Me.txtMaxFileSize.Text = "1G"
        Me.ToolTip1.SetToolTip(Me.txtMaxFileSize, "Maximum file size in bytes to include in backup")
        '
        'lblMaxFileSize
        '
        Me.lblMaxFileSize.AutoSize = True
        Me.lblMaxFileSize.Location = New System.Drawing.Point(3, 83)
        Me.lblMaxFileSize.Name = "lblMaxFileSize"
        Me.lblMaxFileSize.Size = New System.Drawing.Size(72, 13)
        Me.lblMaxFileSize.TabIndex = 15
        Me.lblMaxFileSize.Text = "Max File Size:"
        '
        'txtExcludeFiles
        '
        Me.txtExcludeFiles.Location = New System.Drawing.Point(87, 36)
        Me.txtExcludeFiles.Name = "txtExcludeFiles"
        Me.txtExcludeFiles.Size = New System.Drawing.Size(187, 20)
        Me.txtExcludeFiles.TabIndex = 2
        Me.txtExcludeFiles.Text = "*.tmp;~$*.doc"
        Me.ToolTip1.SetToolTip(Me.txtExcludeFiles, "File masks to exclude. "";"" separates multiple items")
        '
        'lblExclFiles
        '
        Me.lblExclFiles.AutoSize = True
        Me.lblExclFiles.Location = New System.Drawing.Point(3, 39)
        Me.lblExclFiles.Name = "lblExclFiles"
        Me.lblExclFiles.Size = New System.Drawing.Size(72, 13)
        Me.lblExclFiles.TabIndex = 11
        Me.lblExclFiles.Text = "Exclude Files:"
        '
        'txtExcludeFolders
        '
        Me.txtExcludeFolders.Location = New System.Drawing.Point(87, 58)
        Me.txtExcludeFolders.Name = "txtExcludeFolders"
        Me.txtExcludeFolders.Size = New System.Drawing.Size(187, 20)
        Me.txtExcludeFolders.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txtExcludeFolders, "Folders (or masks) to exclude. "";"" separates multiple items. ")
        '
        'lblExclFold
        '
        Me.lblExclFold.AutoSize = True
        Me.lblExclFold.Location = New System.Drawing.Point(3, 61)
        Me.lblExclFold.Name = "lblExclFold"
        Me.lblExclFold.Size = New System.Drawing.Size(85, 13)
        Me.lblExclFold.TabIndex = 9
        Me.lblExclFold.Text = "Exclude Folders:"
        '
        'txtIncludeFiles
        '
        Me.txtIncludeFiles.Location = New System.Drawing.Point(87, 14)
        Me.txtIncludeFiles.Name = "txtIncludeFiles"
        Me.txtIncludeFiles.Size = New System.Drawing.Size(187, 20)
        Me.txtIncludeFiles.TabIndex = 1
        Me.txtIncludeFiles.Text = "*.*"
        Me.ToolTip1.SetToolTip(Me.txtIncludeFiles, "File masks to include. "";"" separates multiple items")
        '
        'lblIncFiles
        '
        Me.lblIncFiles.AutoSize = True
        Me.lblIncFiles.Location = New System.Drawing.Point(3, 17)
        Me.lblIncFiles.Name = "lblIncFiles"
        Me.lblIncFiles.Size = New System.Drawing.Size(69, 13)
        Me.lblIncFiles.TabIndex = 7
        Me.lblIncFiles.Text = "Include Files:"
        '
        'chkDelete
        '
        Me.chkDelete.AutoSize = True
        Me.chkDelete.Location = New System.Drawing.Point(6, 150)
        Me.chkDelete.Name = "chkDelete"
        Me.chkDelete.Size = New System.Drawing.Size(230, 17)
        Me.chkDelete.TabIndex = 6
        Me.chkDelete.Text = "Delete backup files when original is deleted"
        Me.chkDelete.UseVisualStyleBackColor = True
        '
        'tabVer
        '
        Me.tabVer.Controls.Add(Me.grpArc)
        Me.tabVer.Controls.Add(Me.grpArcTo)
        Me.tabVer.Controls.Add(Me.grpVerMax)
        Me.tabVer.Controls.Add(Me.grpVerMode)
        Me.tabVer.Controls.Add(Me.lblTimeStamp)
        Me.tabVer.Controls.Add(Me.txtDateTimeStamp)
        Me.tabVer.Location = New System.Drawing.Point(4, 22)
        Me.tabVer.Name = "tabVer"
        Me.tabVer.Padding = New System.Windows.Forms.Padding(3)
        Me.tabVer.Size = New System.Drawing.Size(373, 282)
        Me.tabVer.TabIndex = 2
        Me.tabVer.Text = "Versioning"
        Me.tabVer.UseVisualStyleBackColor = True
        '
        'grpArc
        '
        Me.grpArc.Controls.Add(Me.lblVersOlder)
        Me.grpArc.Controls.Add(Me.txtMaxVersionAge)
        Me.grpArc.Controls.Add(Me.lblVerDays)
        Me.grpArc.Controls.Add(Me.radNothing)
        Me.grpArc.Controls.Add(Me.radDelete)
        Me.grpArc.Controls.Add(Me.radZip)
        Me.grpArc.Controls.Add(Me.rad7Zip)
        Me.grpArc.Location = New System.Drawing.Point(9, 128)
        Me.grpArc.Name = "grpArc"
        Me.grpArc.Size = New System.Drawing.Size(358, 77)
        Me.grpArc.TabIndex = 41
        Me.grpArc.TabStop = False
        Me.grpArc.Text = "Archiving"
        '
        'lblVersOlder
        '
        Me.lblVersOlder.AutoSize = True
        Me.lblVersOlder.Location = New System.Drawing.Point(6, 25)
        Me.lblVersOlder.Name = "lblVersOlder"
        Me.lblVersOlder.Size = New System.Drawing.Size(114, 13)
        Me.lblVersOlder.TabIndex = 24
        Me.lblVersOlder.Text = "For versions older than"
        '
        'txtMaxVersionAge
        '
        Me.txtMaxVersionAge.Location = New System.Drawing.Point(126, 22)
        Me.txtMaxVersionAge.MaxLength = 3
        Me.txtMaxVersionAge.Name = "txtMaxVersionAge"
        Me.txtMaxVersionAge.Size = New System.Drawing.Size(35, 20)
        Me.txtMaxVersionAge.TabIndex = 3
        Me.txtMaxVersionAge.Text = "90"
        '
        'lblVerDays
        '
        Me.lblVerDays.AutoSize = True
        Me.lblVerDays.Location = New System.Drawing.Point(167, 25)
        Me.lblVerDays.Name = "lblVerDays"
        Me.lblVerDays.Size = New System.Drawing.Size(38, 13)
        Me.lblVerDays.TabIndex = 26
        Me.lblVerDays.Text = "days..."
        '
        'radNothing
        '
        Me.radNothing.AutoSize = True
        Me.radNothing.Checked = True
        Me.radNothing.Location = New System.Drawing.Point(217, 25)
        Me.radNothing.Name = "radNothing"
        Me.radNothing.Size = New System.Drawing.Size(62, 17)
        Me.radNothing.TabIndex = 4
        Me.radNothing.TabStop = True
        Me.radNothing.Text = "Nothing"
        Me.ToolTip1.SetToolTip(Me.radNothing, "Default. Don't touch the old versions")
        Me.radNothing.UseVisualStyleBackColor = True
        '
        'radDelete
        '
        Me.radDelete.AutoSize = True
        Me.radDelete.Location = New System.Drawing.Point(285, 25)
        Me.radDelete.Name = "radDelete"
        Me.radDelete.Size = New System.Drawing.Size(56, 17)
        Me.radDelete.TabIndex = 5
        Me.radDelete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.radDelete, "Delete the old versions")
        Me.radDelete.UseVisualStyleBackColor = True
        '
        'radZip
        '
        Me.radZip.AutoSize = True
        Me.radZip.Location = New System.Drawing.Point(218, 48)
        Me.radZip.Name = "radZip"
        Me.radZip.Size = New System.Drawing.Size(40, 17)
        Me.radZip.TabIndex = 6
        Me.radZip.Text = "Zip"
        Me.ToolTip1.SetToolTip(Me.radZip, "Good compression and very fast")
        Me.radZip.UseVisualStyleBackColor = True
        '
        'rad7Zip
        '
        Me.rad7Zip.AutoSize = True
        Me.rad7Zip.Location = New System.Drawing.Point(285, 48)
        Me.rad7Zip.Name = "rad7Zip"
        Me.rad7Zip.Size = New System.Drawing.Size(49, 17)
        Me.rad7Zip.TabIndex = 34
        Me.rad7Zip.Text = "7-Zip"
        Me.ToolTip1.SetToolTip(Me.rad7Zip, "7-Zip must be installed. Best compression")
        Me.rad7Zip.UseVisualStyleBackColor = True
        '
        'grpArcTo
        '
        Me.grpArcTo.Controls.Add(Me.rbZipModeF)
        Me.grpArcTo.Controls.Add(Me.rbZipModeD)
        Me.grpArcTo.Controls.Add(Me.rbZipModeW)
        Me.grpArcTo.Controls.Add(Me.lblZip1)
        Me.grpArcTo.Location = New System.Drawing.Point(9, 211)
        Me.grpArcTo.Name = "grpArcTo"
        Me.grpArcTo.Size = New System.Drawing.Size(358, 58)
        Me.grpArcTo.TabIndex = 42
        Me.grpArcTo.TabStop = False
        Me.grpArcTo.Text = "Archive To"
        '
        'rbZipModeF
        '
        Me.rbZipModeF.AutoSize = True
        Me.rbZipModeF.Location = New System.Drawing.Point(285, 24)
        Me.rbZipModeF.Name = "rbZipModeF"
        Me.rbZipModeF.Size = New System.Drawing.Size(41, 17)
        Me.rbZipModeF.TabIndex = 44
        Me.rbZipModeF.Text = "File"
        Me.ToolTip1.SetToolTip(Me.rbZipModeF, "Archive file will be the file name.zip/7z in the backed up folder")
        Me.rbZipModeF.UseVisualStyleBackColor = True
        '
        'rbZipModeD
        '
        Me.rbZipModeD.AutoSize = True
        Me.rbZipModeD.Location = New System.Drawing.Point(217, 24)
        Me.rbZipModeD.Name = "rbZipModeD"
        Me.rbZipModeD.Size = New System.Drawing.Size(54, 17)
        Me.rbZipModeD.TabIndex = 43
        Me.rbZipModeD.Text = "Folder"
        Me.ToolTip1.SetToolTip(Me.rbZipModeD, "Archive file will be the folder name.zip/7z in the backed up folder")
        Me.rbZipModeD.UseVisualStyleBackColor = True
        '
        'rbZipModeW
        '
        Me.rbZipModeW.AutoSize = True
        Me.rbZipModeW.Checked = True
        Me.rbZipModeW.Location = New System.Drawing.Point(126, 24)
        Me.rbZipModeW.Name = "rbZipModeW"
        Me.rbZipModeW.Size = New System.Drawing.Size(66, 17)
        Me.rbZipModeW.TabIndex = 42
        Me.rbZipModeW.TabStop = True
        Me.rbZipModeW.Text = "Watcher"
        Me.ToolTip1.SetToolTip(Me.rbZipModeW, "Archive file will be BackupVersions.zip/7z in the Backup folder root")
        Me.rbZipModeW.UseVisualStyleBackColor = True
        '
        'lblZip1
        '
        Me.lblZip1.AutoSize = True
        Me.lblZip1.Location = New System.Drawing.Point(6, 26)
        Me.lblZip1.Name = "lblZip1"
        Me.lblZip1.Size = New System.Drawing.Size(80, 13)
        Me.lblZip1.TabIndex = 41
        Me.lblZip1.Text = "Zip to 1 file per:"
        '
        'grpVerMax
        '
        Me.grpVerMax.Controls.Add(Me.txtVerRateS)
        Me.grpVerMax.Controls.Add(Me.lblHourMinSec)
        Me.grpVerMax.Controls.Add(Me.txtVerRateM)
        Me.grpVerMax.Controls.Add(Me.txtVerRateH)
        Me.grpVerMax.Controls.Add(Me.rbVerRate)
        Me.grpVerMax.Controls.Add(Me.rbVerAll)
        Me.grpVerMax.Location = New System.Drawing.Point(202, 6)
        Me.grpVerMax.Name = "grpVerMax"
        Me.grpVerMax.Size = New System.Drawing.Size(165, 90)
        Me.grpVerMax.TabIndex = 33
        Me.grpVerMax.TabStop = False
        Me.grpVerMax.Text = "Max 1 version per "
        '
        'txtVerRateS
        '
        Me.txtVerRateS.Location = New System.Drawing.Point(99, 44)
        Me.txtVerRateS.MaxLength = 2
        Me.txtVerRateS.Name = "txtVerRateS"
        Me.txtVerRateS.Size = New System.Drawing.Size(30, 20)
        Me.txtVerRateS.TabIndex = 6
        Me.txtVerRateS.Text = "0"
        '
        'lblHourMinSec
        '
        Me.lblHourMinSec.AutoSize = True
        Me.lblHourMinSec.Location = New System.Drawing.Point(24, 69)
        Me.lblHourMinSec.Name = "lblHourMinSec"
        Me.lblHourMinSec.Size = New System.Drawing.Size(99, 13)
        Me.lblHourMinSec.TabIndex = 5
        Me.lblHourMinSec.Text = "Hours : Mins : Secs"
        '
        'txtVerRateM
        '
        Me.txtVerRateM.Location = New System.Drawing.Point(63, 44)
        Me.txtVerRateM.MaxLength = 2
        Me.txtVerRateM.Name = "txtVerRateM"
        Me.txtVerRateM.Size = New System.Drawing.Size(30, 20)
        Me.txtVerRateM.TabIndex = 4
        '
        'txtVerRateH
        '
        Me.txtVerRateH.Location = New System.Drawing.Point(27, 44)
        Me.txtVerRateH.MaxLength = 3
        Me.txtVerRateH.Name = "txtVerRateH"
        Me.txtVerRateH.Size = New System.Drawing.Size(30, 20)
        Me.txtVerRateH.TabIndex = 2
        '
        'rbVerRate
        '
        Me.rbVerRate.AutoSize = True
        Me.rbVerRate.Location = New System.Drawing.Point(7, 44)
        Me.rbVerRate.Name = "rbVerRate"
        Me.rbVerRate.Size = New System.Drawing.Size(14, 13)
        Me.rbVerRate.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.rbVerRate, "Only copy the file every so often when it is changed/saved")
        Me.rbVerRate.UseVisualStyleBackColor = True
        '
        'rbVerAll
        '
        Me.rbVerAll.AutoSize = True
        Me.rbVerAll.Checked = True
        Me.rbVerAll.Location = New System.Drawing.Point(7, 20)
        Me.rbVerAll.Name = "rbVerAll"
        Me.rbVerAll.Size = New System.Drawing.Size(81, 17)
        Me.rbVerAll.TabIndex = 0
        Me.rbVerAll.TabStop = True
        Me.rbVerAll.Text = "All Changes"
        Me.ToolTip1.SetToolTip(Me.rbVerAll, "Make a new copy every time the file is changed or saved")
        Me.rbVerAll.UseVisualStyleBackColor = True
        '
        'grpVerMode
        '
        Me.grpVerMode.Controls.Add(Me.rbVersionPrev)
        Me.grpVerMode.Controls.Add(Me.rbVersionAll)
        Me.grpVerMode.Controls.Add(Me.rbNone)
        Me.grpVerMode.Location = New System.Drawing.Point(9, 6)
        Me.grpVerMode.Name = "grpVerMode"
        Me.grpVerMode.Size = New System.Drawing.Size(187, 90)
        Me.grpVerMode.TabIndex = 32
        Me.grpVerMode.TabStop = False
        Me.grpVerMode.Text = "Versioning Mode"
        '
        'rbVersionPrev
        '
        Me.rbVersionPrev.AutoSize = True
        Me.rbVersionPrev.Location = New System.Drawing.Point(6, 65)
        Me.rbVersionPrev.Name = "rbVersionPrev"
        Me.rbVersionPrev.Size = New System.Drawing.Size(169, 17)
        Me.rbVersionPrev.TabIndex = 9
        Me.rbVersionPrev.Text = "Version previous backups only"
        Me.ToolTip1.SetToolTip(Me.rbVersionPrev, "Previous backup is renamed to include a timestamp. Current backup does not have a" & _
        " timestamp")
        Me.rbVersionPrev.UseVisualStyleBackColor = True
        '
        'rbVersionAll
        '
        Me.rbVersionAll.AutoSize = True
        Me.rbVersionAll.Checked = True
        Me.rbVersionAll.Location = New System.Drawing.Point(6, 42)
        Me.rbVersionAll.Name = "rbVersionAll"
        Me.rbVersionAll.Size = New System.Drawing.Size(133, 17)
        Me.rbVersionAll.TabIndex = 8
        Me.rbVersionAll.TabStop = True
        Me.rbVersionAll.Text = "Version all backup files"
        Me.ToolTip1.SetToolTip(Me.rbVersionAll, "Every backup file has a timestamp")
        Me.rbVersionAll.UseVisualStyleBackColor = True
        '
        'rbNone
        '
        Me.rbNone.AutoSize = True
        Me.rbNone.Location = New System.Drawing.Point(6, 19)
        Me.rbNone.Name = "rbNone"
        Me.rbNone.Size = New System.Drawing.Size(172, 17)
        Me.rbNone.TabIndex = 7
        Me.rbNone.Text = "None - Update backup file only"
        Me.ToolTip1.SetToolTip(Me.rbNone, "No versioning (time stamping). Just backup the file to the same name")
        Me.rbNone.UseVisualStyleBackColor = True
        '
        'lblTimeStamp
        '
        Me.lblTimeStamp.AutoSize = True
        Me.lblTimeStamp.Location = New System.Drawing.Point(7, 105)
        Me.lblTimeStamp.Name = "lblTimeStamp"
        Me.lblTimeStamp.Size = New System.Drawing.Size(92, 13)
        Me.lblTimeStamp.TabIndex = 31
        Me.lblTimeStamp.Text = "Date/Time stamp:"
        '
        'txtDateTimeStamp
        '
        Me.txtDateTimeStamp.Location = New System.Drawing.Point(105, 102)
        Me.txtDateTimeStamp.MaxLength = 50
        Me.txtDateTimeStamp.Name = "txtDateTimeStamp"
        Me.txtDateTimeStamp.Size = New System.Drawing.Size(116, 20)
        Me.txtDateTimeStamp.TabIndex = 2
        Me.txtDateTimeStamp.Text = "yyMMddHHmmss"
        Me.ToolTip1.SetToolTip(Me.txtDateTimeStamp, "Timestamp format. No colons or backslashes (:\)")
        '
        'tabFTP
        '
        Me.tabFTP.Controls.Add(Me.chkFTPPassive)
        Me.tabFTP.Controls.Add(Me.lblFtpAnon)
        Me.tabFTP.Controls.Add(Me.lblFtpPass)
        Me.tabFTP.Controls.Add(Me.txtFTPPassword)
        Me.tabFTP.Controls.Add(Me.lblFtpUser)
        Me.tabFTP.Controls.Add(Me.txtFTPLogin)
        Me.tabFTP.Controls.Add(Me.lblFtpHost)
        Me.tabFTP.Controls.Add(Me.txtFTPHost)
        Me.tabFTP.Controls.Add(Me.lblFtpIf)
        Me.tabFTP.Location = New System.Drawing.Point(4, 22)
        Me.tabFTP.Name = "tabFTP"
        Me.tabFTP.Padding = New System.Windows.Forms.Padding(3)
        Me.tabFTP.Size = New System.Drawing.Size(373, 282)
        Me.tabFTP.TabIndex = 3
        Me.tabFTP.Text = "FTP"
        Me.tabFTP.UseVisualStyleBackColor = True
        '
        'chkFTPPassive
        '
        Me.chkFTPPassive.AutoSize = True
        Me.chkFTPPassive.Checked = True
        Me.chkFTPPassive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkFTPPassive.Location = New System.Drawing.Point(10, 117)
        Me.chkFTPPassive.Name = "chkFTPPassive"
        Me.chkFTPPassive.Size = New System.Drawing.Size(63, 17)
        Me.chkFTPPassive.TabIndex = 4
        Me.chkFTPPassive.Text = "Passive"
        Me.ToolTip1.SetToolTip(Me.chkFTPPassive, "Passive or Active connection")
        Me.chkFTPPassive.UseVisualStyleBackColor = True
        '
        'lblFtpAnon
        '
        Me.lblFtpAnon.AutoSize = True
        Me.lblFtpAnon.Location = New System.Drawing.Point(182, 59)
        Me.lblFtpAnon.Name = "lblFtpAnon"
        Me.lblFtpAnon.Size = New System.Drawing.Size(100, 13)
        Me.lblFtpAnon.TabIndex = 7
        Me.lblFtpAnon.Text = "Blank = anonymous"
        '
        'lblFtpPass
        '
        Me.lblFtpPass.AutoSize = True
        Me.lblFtpPass.Location = New System.Drawing.Point(7, 89)
        Me.lblFtpPass.Name = "lblFtpPass"
        Me.lblFtpPass.Size = New System.Drawing.Size(56, 13)
        Me.lblFtpPass.TabIndex = 6
        Me.lblFtpPass.Text = "Password:"
        '
        'txtFTPPassword
        '
        Me.txtFTPPassword.Location = New System.Drawing.Point(76, 82)
        Me.txtFTPPassword.Name = "txtFTPPassword"
        Me.txtFTPPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtFTPPassword.Size = New System.Drawing.Size(100, 20)
        Me.txtFTPPassword.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txtFTPPassword, "Password or email for anonymous")
        Me.txtFTPPassword.UseSystemPasswordChar = True
        '
        'lblFtpUser
        '
        Me.lblFtpUser.AutoSize = True
        Me.lblFtpUser.Location = New System.Drawing.Point(7, 59)
        Me.lblFtpUser.Name = "lblFtpUser"
        Me.lblFtpUser.Size = New System.Drawing.Size(63, 13)
        Me.lblFtpUser.TabIndex = 4
        Me.lblFtpUser.Text = "User Name:"
        '
        'txtFTPLogin
        '
        Me.txtFTPLogin.Location = New System.Drawing.Point(76, 56)
        Me.txtFTPLogin.Name = "txtFTPLogin"
        Me.txtFTPLogin.Size = New System.Drawing.Size(100, 20)
        Me.txtFTPLogin.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtFTPLogin, "Login or User ID. Blank for anonymous")
        '
        'lblFtpHost
        '
        Me.lblFtpHost.AutoSize = True
        Me.lblFtpHost.Location = New System.Drawing.Point(7, 33)
        Me.lblFtpHost.Name = "lblFtpHost"
        Me.lblFtpHost.Size = New System.Drawing.Size(32, 13)
        Me.lblFtpHost.TabIndex = 2
        Me.lblFtpHost.Text = "Host:"
        '
        'txtFTPHost
        '
        Me.txtFTPHost.Location = New System.Drawing.Point(76, 30)
        Me.txtFTPHost.Name = "txtFTPHost"
        Me.txtFTPHost.Size = New System.Drawing.Size(178, 20)
        Me.txtFTPHost.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtFTPHost, "FTP URL")
        '
        'lblFtpIf
        '
        Me.lblFtpIf.AutoSize = True
        Me.lblFtpIf.Location = New System.Drawing.Point(7, 7)
        Me.lblFtpIf.Name = "lblFtpIf"
        Me.lblFtpIf.Size = New System.Drawing.Size(150, 13)
        Me.lblFtpIf.TabIndex = 0
        Me.lblFtpIf.Text = "If FTP Backup folder is used..."
        '
        'butCancel
        '
        Me.butCancel.Image = Global.AutoVer.My.Resources.Resources.icon_cancel
        Me.butCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.butCancel.Location = New System.Drawing.Point(290, 326)
        Me.butCancel.Name = "butCancel"
        Me.butCancel.Size = New System.Drawing.Size(100, 23)
        Me.butCancel.TabIndex = 9
        Me.butCancel.Text = "Cancel"
        Me.butCancel.UseVisualStyleBackColor = True
        '
        'butOK
        '
        Me.butOK.Image = Global.AutoVer.My.Resources.Resources.icon_tick
        Me.butOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.butOK.Location = New System.Drawing.Point(184, 326)
        Me.butOK.Name = "butOK"
        Me.butOK.Size = New System.Drawing.Size(100, 23)
        Me.butOK.TabIndex = 8
        Me.butOK.Text = "OK"
        Me.butOK.UseVisualStyleBackColor = True
        '
        'btnHelp
        '
        Me.HelpProvider1.SetHelpKeyword(Me.btnHelp, "ApplicationSettings")
        Me.HelpProvider1.SetHelpString(Me.btnHelp, "Help me###")
        Me.btnHelp.Image = Global.AutoVer.My.Resources.Resources.help
        Me.btnHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnHelp.Location = New System.Drawing.Point(15, 326)
        Me.btnHelp.Name = "btnHelp"
        Me.HelpProvider1.SetShowHelp(Me.btnHelp, True)
        Me.btnHelp.Size = New System.Drawing.Size(100, 23)
        Me.btnHelp.TabIndex = 10
        Me.btnHelp.Text = "Help"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'HelpProvider1
        '
        Me.HelpProvider1.HelpNamespace = "AutoVer.chm"
        '
        'ToolTip1
        '
        Me.ToolTip1.AutomaticDelay = 200
        Me.ToolTip1.AutoPopDelay = 5000
        Me.ToolTip1.InitialDelay = 200
        Me.ToolTip1.ReshowDelay = 40
        '
        'tmrEnsureSyncUpdates
        '
        Me.tmrEnsureSyncUpdates.Interval = 1000
        '
        'bwBackupRestore
        '
        '
        'Settings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(405, 357)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.butCancel)
        Me.Controls.Add(Me.tabControl)
        Me.Controls.Add(Me.butOK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.HelpProvider1.SetHelpKeyword(Me, "3")
        Me.HelpProvider1.SetHelpNavigator(Me, System.Windows.Forms.HelpNavigator.TopicId)
        Me.HelpProvider1.SetHelpString(Me, "3")
        Me.MaximizeBox = False
        Me.Name = "Settings"
        Me.HelpProvider1.SetShowHelp(Me, True)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Settings"
        Me.tabControl.ResumeLayout(False)
        Me.tabGen.ResumeLayout(False)
        Me.tabGen.PerformLayout()
        Me.grpType.ResumeLayout(False)
        Me.grpType.PerformLayout()
        Me.tabAdv.ResumeLayout(False)
        Me.tabAdv.PerformLayout()
        Me.tabVer.ResumeLayout(False)
        Me.tabVer.PerformLayout()
        Me.grpArc.ResumeLayout(False)
        Me.grpArc.PerformLayout()
        Me.grpArcTo.ResumeLayout(False)
        Me.grpArcTo.PerformLayout()
        Me.grpVerMax.ResumeLayout(False)
        Me.grpVerMax.PerformLayout()
        Me.grpVerMode.ResumeLayout(False)
        Me.grpVerMode.PerformLayout()
        Me.tabFTP.ResumeLayout(False)
        Me.tabFTP.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tabControl As System.Windows.Forms.TabControl
    Friend WithEvents tabGen As System.Windows.Forms.TabPage
    Friend WithEvents lblBackFold As System.Windows.Forms.Label
    Friend WithEvents lblWatchFold As System.Windows.Forms.Label
    Friend WithEvents tabAdv As System.Windows.Forms.TabPage
    Friend WithEvents butCancel As System.Windows.Forms.Button
    Friend WithEvents butOK As System.Windows.Forms.Button
    Friend WithEvents txtWatch As System.Windows.Forms.TextBox
    Friend WithEvents txtBackup As System.Windows.Forms.TextBox
    Friend WithEvents chkDelete As System.Windows.Forms.CheckBox
    Friend WithEvents butBackupFolder As System.Windows.Forms.Button
    Friend WithEvents btnWatchFolder As System.Windows.Forms.Button
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents chkBackup As System.Windows.Forms.CheckBox
    Friend WithEvents txtIncludeFiles As System.Windows.Forms.TextBox
    Friend WithEvents lblIncFiles As System.Windows.Forms.Label
    Friend WithEvents txtExcludeFiles As System.Windows.Forms.TextBox
    Friend WithEvents lblExclFiles As System.Windows.Forms.Label
    Friend WithEvents txtExcludeFolders As System.Windows.Forms.TextBox
    Friend WithEvents lblExclFold As System.Windows.Forms.Label
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents txtMaxFileSize As System.Windows.Forms.TextBox
    Friend WithEvents lblMaxFileSize As System.Windows.Forms.Label
    Friend WithEvents tabVer As System.Windows.Forms.TabPage
    Friend WithEvents radZip As System.Windows.Forms.RadioButton
    Friend WithEvents radDelete As System.Windows.Forms.RadioButton
    Friend WithEvents radNothing As System.Windows.Forms.RadioButton
    Friend WithEvents lblVerDays As System.Windows.Forms.Label
    Friend WithEvents txtMaxVersionAge As System.Windows.Forms.TextBox
    Friend WithEvents lblVersOlder As System.Windows.Forms.Label
    Friend WithEvents lblFileSep As System.Windows.Forms.Label
    Friend WithEvents lblName As System.Windows.Forms.Label
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents lblTimeStamp As System.Windows.Forms.Label
    Friend WithEvents txtDateTimeStamp As System.Windows.Forms.TextBox
    Friend WithEvents HelpProvider1 As System.Windows.Forms.HelpProvider
    Friend WithEvents tabFTP As System.Windows.Forms.TabPage
    Friend WithEvents lblFtpAnon As System.Windows.Forms.Label
    Friend WithEvents lblFtpPass As System.Windows.Forms.Label
    Friend WithEvents txtFTPPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblFtpUser As System.Windows.Forms.Label
    Friend WithEvents txtFTPLogin As System.Windows.Forms.TextBox
    Friend WithEvents lblFtpHost As System.Windows.Forms.Label
    Friend WithEvents txtFTPHost As System.Windows.Forms.TextBox
    Friend WithEvents lblFtpIf As System.Windows.Forms.Label
    Friend WithEvents chkFTPPassive As System.Windows.Forms.CheckBox
    Friend WithEvents rbFolder As System.Windows.Forms.RadioButton
    Friend WithEvents grpType As System.Windows.Forms.GroupBox
    Friend WithEvents rbFTP As System.Windows.Forms.RadioButton
    Friend WithEvents chkEnabled As System.Windows.Forms.CheckBox
    Friend WithEvents lblCounts As System.Windows.Forms.Label
    Friend WithEvents lblStats As System.Windows.Forms.Label
    Friend WithEvents grpVerMode As System.Windows.Forms.GroupBox
    Friend WithEvents rbVersionPrev As System.Windows.Forms.RadioButton
    Friend WithEvents rbVersionAll As System.Windows.Forms.RadioButton
    Friend WithEvents rbNone As System.Windows.Forms.RadioButton
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents grpVerMax As System.Windows.Forms.GroupBox
    Friend WithEvents rbVerRate As System.Windows.Forms.RadioButton
    Friend WithEvents rbVerAll As System.Windows.Forms.RadioButton
    Friend WithEvents txtVerRateS As System.Windows.Forms.TextBox
    Friend WithEvents lblHourMinSec As System.Windows.Forms.Label
    Friend WithEvents txtVerRateM As System.Windows.Forms.TextBox
    Friend WithEvents txtVerRateH As System.Windows.Forms.TextBox
    Friend WithEvents chkShowErrors As System.Windows.Forms.CheckBox
    Friend WithEvents lblFoldSep As System.Windows.Forms.Label
    Friend WithEvents rbEnsureDaily As System.Windows.Forms.RadioButton
    Friend WithEvents rbEnsureNever As System.Windows.Forms.RadioButton
    Friend WithEvents lblSync As System.Windows.Forms.Label
    Friend WithEvents rbEnsureHourly As System.Windows.Forms.RadioButton
    Friend WithEvents txtEnsureTime As System.Windows.Forms.TextBox
    Friend WithEvents chkRunCopyFirst As System.Windows.Forms.CheckBox
    Friend WithEvents lblRunCopy As System.Windows.Forms.Label
    Friend WithEvents txtRunCopy As System.Windows.Forms.TextBox
    Friend WithEvents txtSettleDelay As System.Windows.Forms.TextBox
    Friend WithEvents lblSettle As System.Windows.Forms.Label
    Friend WithEvents chkSubFolders As System.Windows.Forms.CheckBox
    Friend WithEvents rad7Zip As System.Windows.Forms.RadioButton
    Friend WithEvents chkShowEvents As System.Windows.Forms.CheckBox
    Friend WithEvents grpArc As System.Windows.Forms.GroupBox
    Friend WithEvents grpArcTo As System.Windows.Forms.GroupBox
    Friend WithEvents rbZipModeF As System.Windows.Forms.RadioButton
    Friend WithEvents rbZipModeD As System.Windows.Forms.RadioButton
    Friend WithEvents rbZipModeW As System.Windows.Forms.RadioButton
    Friend WithEvents lblZip1 As System.Windows.Forms.Label
    Friend WithEvents chkCompareBeforeCopy As System.Windows.Forms.CheckBox
    Friend WithEvents tmrEnsureSyncUpdates As System.Windows.Forms.Timer
    Friend WithEvents bwBackupRestore As System.ComponentModel.BackgroundWorker
End Class
