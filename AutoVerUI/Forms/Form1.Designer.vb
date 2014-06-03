<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.lvwWatches = New System.Windows.Forms.ListView()
        Me.lvcName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lvcStatus = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lvcStats = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lvcMessage = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.tsbAdd = New System.Windows.Forms.ToolStripButton()
        Me.tsbProperties = New System.Windows.Forms.ToolStripButton()
        Me.tsbDelete = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tspHelp = New System.Windows.Forms.ToolStripButton()
        Me.tspAbout = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsbSettings = New System.Windows.Forms.ToolStripButton()
        Me.tsbPause = New System.Windows.Forms.ToolStripSplitButton()
        Me.tsbPauseAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsbEnsureBackup = New System.Windows.Forms.ToolStripSplitButton()
        Me.tsbEnsureBackupALL = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsbRestore = New System.Windows.Forms.ToolStripButton()
        Me.tsbExplore = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SysTrayIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MainScreenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SysTrayToolStripMenuPauseAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ExporeBackupsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EnsureBackupIsCurrentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RestoreAllFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewPropertiesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddWatchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteWatchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PauseResumeWatchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpProvider1 = New System.Windows.Forms.HelpProvider()
        Me.tmrEngineUpdates = New System.Windows.Forms.Timer(Me.components)
        Me.bwBackupRestore = New System.ComponentModel.BackgroundWorker()
        Me.tmrEnsureSyncUpdates = New System.Windows.Forms.Timer(Me.components)
        Me.ToolStrip1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ContextMenuStrip2.SuspendLayout()
        Me.SuspendLayout()
        '
        'lvwWatches
        '
        Me.lvwWatches.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwWatches.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.lvcName, Me.lvcStatus, Me.lvcStats, Me.lvcMessage})
        Me.lvwWatches.FullRowSelect = True
        Me.lvwWatches.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvwWatches.HideSelection = False
        Me.lvwWatches.Location = New System.Drawing.Point(12, 37)
        Me.lvwWatches.MultiSelect = False
        Me.lvwWatches.Name = "lvwWatches"
        Me.lvwWatches.Size = New System.Drawing.Size(450, 217)
        Me.lvwWatches.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvwWatches.TabIndex = 2
        Me.lvwWatches.UseCompatibleStateImageBehavior = False
        Me.lvwWatches.View = System.Windows.Forms.View.Details
        '
        'lvcName
        '
        Me.lvcName.Text = "Watch"
        Me.lvcName.Width = 355
        '
        'lvcStatus
        '
        Me.lvcStatus.Text = "Status"
        Me.lvcStatus.Width = 95
        '
        'lvcStats
        '
        Me.lvcStats.Text = "Events"
        Me.lvcStats.Width = 80
        '
        'lvcMessage
        '
        Me.lvcMessage.Text = "Messages"
        '
        'ToolStrip1
        '
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(32, 32)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbAdd, Me.tsbProperties, Me.tsbDelete, Me.ToolStripSeparator1, Me.tspHelp, Me.tspAbout, Me.ToolStripSeparator2, Me.tsbSettings, Me.tsbPause, Me.tsbEnsureBackup, Me.tsbRestore, Me.tsbExplore, Me.ToolStripSeparator3})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ToolStrip1.Size = New System.Drawing.Size(474, 39)
        Me.ToolStrip1.TabIndex = 3
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'tsbAdd
        '
        Me.tsbAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbAdd.Image = CType(resources.GetObject("tsbAdd.Image"), System.Drawing.Image)
        Me.tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbAdd.Name = "tsbAdd"
        Me.tsbAdd.Size = New System.Drawing.Size(36, 36)
        Me.tsbAdd.Text = "ToolStripButton1"
        Me.tsbAdd.ToolTipText = "Add new Watcher"
        '
        'tsbProperties
        '
        Me.tsbProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbProperties.Image = CType(resources.GetObject("tsbProperties.Image"), System.Drawing.Image)
        Me.tsbProperties.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbProperties.Name = "tsbProperties"
        Me.tsbProperties.Size = New System.Drawing.Size(36, 36)
        Me.tsbProperties.Text = "ToolStripButton1"
        Me.tsbProperties.ToolTipText = "Watcher Properties"
        '
        'tsbDelete
        '
        Me.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbDelete.Image = CType(resources.GetObject("tsbDelete.Image"), System.Drawing.Image)
        Me.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbDelete.Name = "tsbDelete"
        Me.tsbDelete.Size = New System.Drawing.Size(36, 36)
        Me.tsbDelete.Text = "ToolStripButton1"
        Me.tsbDelete.ToolTipText = "Delete watcher"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 39)
        '
        'tspHelp
        '
        Me.tspHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tspHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tspHelp.Image = CType(resources.GetObject("tspHelp.Image"), System.Drawing.Image)
        Me.tspHelp.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tspHelp.Name = "tspHelp"
        Me.tspHelp.Size = New System.Drawing.Size(36, 36)
        Me.tspHelp.Text = "Help"
        Me.tspHelp.ToolTipText = "Help"
        '
        'tspAbout
        '
        Me.tspAbout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tspAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tspAbout.Image = CType(resources.GetObject("tspAbout.Image"), System.Drawing.Image)
        Me.tspAbout.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tspAbout.Name = "tspAbout"
        Me.tspAbout.Size = New System.Drawing.Size(36, 36)
        Me.tspAbout.Text = "About"
        Me.tspAbout.ToolTipText = "About AutoVer"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 39)
        '
        'tsbSettings
        '
        Me.tsbSettings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tsbSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSettings.Image = CType(resources.GetObject("tsbSettings.Image"), System.Drawing.Image)
        Me.tsbSettings.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSettings.Name = "tsbSettings"
        Me.tsbSettings.Size = New System.Drawing.Size(36, 36)
        Me.tsbSettings.Text = "Application Settings"
        '
        'tsbPause
        '
        Me.tsbPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbPause.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbPauseAll})
        Me.tsbPause.Image = CType(resources.GetObject("tsbPause.Image"), System.Drawing.Image)
        Me.tsbPause.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbPause.Name = "tsbPause"
        Me.tsbPause.Size = New System.Drawing.Size(48, 36)
        Me.tsbPause.Text = "Pause/Resume Watcher"
        '
        'tsbPauseAll
        '
        Me.tsbPauseAll.Image = CType(resources.GetObject("tsbPauseAll.Image"), System.Drawing.Image)
        Me.tsbPauseAll.Name = "tsbPauseAll"
        Me.tsbPauseAll.Size = New System.Drawing.Size(175, 22)
        Me.tsbPauseAll.Text = "Pause/Resume ALL"
        Me.tsbPauseAll.ToolTipText = "Pause/Resume ALL Watchers"
        '
        'tsbEnsureBackup
        '
        Me.tsbEnsureBackup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbEnsureBackup.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbEnsureBackupALL})
        Me.tsbEnsureBackup.Image = CType(resources.GetObject("tsbEnsureBackup.Image"), System.Drawing.Image)
        Me.tsbEnsureBackup.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbEnsureBackup.Name = "tsbEnsureBackup"
        Me.tsbEnsureBackup.Size = New System.Drawing.Size(48, 36)
        Me.tsbEnsureBackup.Text = "Synchronise (Backup now!)"
        Me.tsbEnsureBackup.ToolTipText = "Synchronise the selected Watcher (Backup now!)"
        '
        'tsbEnsureBackupALL
        '
        Me.tsbEnsureBackupALL.Image = CType(resources.GetObject("tsbEnsureBackupALL.Image"), System.Drawing.Image)
        Me.tsbEnsureBackupALL.Name = "tsbEnsureBackupALL"
        Me.tsbEnsureBackupALL.Size = New System.Drawing.Size(208, 22)
        Me.tsbEnsureBackupALL.Text = "Synchronise ALL backups"
        Me.tsbEnsureBackupALL.ToolTipText = "Ensure ALL backups are current. (Sync ALL now!)"
        '
        'tsbRestore
        '
        Me.tsbRestore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbRestore.Image = CType(resources.GetObject("tsbRestore.Image"), System.Drawing.Image)
        Me.tsbRestore.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbRestore.Name = "tsbRestore"
        Me.tsbRestore.Size = New System.Drawing.Size(36, 36)
        Me.tsbRestore.Text = "Restore All Files"
        Me.tsbRestore.ToolTipText = "Restore files for the selected Watcher."
        '
        'tsbExplore
        '
        Me.tsbExplore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbExplore.Image = CType(resources.GetObject("tsbExplore.Image"), System.Drawing.Image)
        Me.tsbExplore.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbExplore.Name = "tsbExplore"
        Me.tsbExplore.Size = New System.Drawing.Size(36, 36)
        Me.tsbExplore.Text = "Explore Backups"
        Me.tsbExplore.ToolTipText = "Explore Backups for the selected Watcher."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 39)
        '
        'SysTrayIcon
        '
        Me.SysTrayIcon.ContextMenuStrip = Me.ContextMenuStrip1
        Me.SysTrayIcon.Icon = CType(resources.GetObject("SysTrayIcon.Icon"), System.Drawing.Icon)
        Me.SysTrayIcon.Text = "AutoVer"
        Me.SysTrayIcon.Visible = True
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MainScreenToolStripMenuItem, Me.SysTrayToolStripMenuPauseAll, Me.ExitToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(186, 70)
        '
        'MainScreenToolStripMenuItem
        '
        Me.MainScreenToolStripMenuItem.Name = "MainScreenToolStripMenuItem"
        Me.MainScreenToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.MainScreenToolStripMenuItem.Text = "AutoVer &Main Screen"
        '
        'SysTrayToolStripMenuPauseAll
        '
        Me.SysTrayToolStripMenuPauseAll.Name = "SysTrayToolStripMenuPauseAll"
        Me.SysTrayToolStripMenuPauseAll.Size = New System.Drawing.Size(185, 22)
        Me.SysTrayToolStripMenuPauseAll.Text = "Pause/Resume All"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit AutoVer"
        '
        'ContextMenuStrip2
        '
        Me.ContextMenuStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExporeBackupsToolStripMenuItem, Me.EnsureBackupIsCurrentToolStripMenuItem, Me.RestoreAllFilesToolStripMenuItem, Me.ViewPropertiesToolStripMenuItem, Me.AddWatchToolStripMenuItem, Me.DeleteWatchToolStripMenuItem, Me.PauseResumeWatchToolStripMenuItem})
        Me.ContextMenuStrip2.Name = "ContextMenuStrip2"
        Me.ContextMenuStrip2.Size = New System.Drawing.Size(202, 158)
        '
        'ExporeBackupsToolStripMenuItem
        '
        Me.ExporeBackupsToolStripMenuItem.Image = CType(resources.GetObject("ExporeBackupsToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ExporeBackupsToolStripMenuItem.Name = "ExporeBackupsToolStripMenuItem"
        Me.ExporeBackupsToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.ExporeBackupsToolStripMenuItem.Text = "Explore Backups"
        '
        'EnsureBackupIsCurrentToolStripMenuItem
        '
        Me.EnsureBackupIsCurrentToolStripMenuItem.Image = CType(resources.GetObject("EnsureBackupIsCurrentToolStripMenuItem.Image"), System.Drawing.Image)
        Me.EnsureBackupIsCurrentToolStripMenuItem.Name = "EnsureBackupIsCurrentToolStripMenuItem"
        Me.EnsureBackupIsCurrentToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.EnsureBackupIsCurrentToolStripMenuItem.Text = "Synchronise Backup"
        '
        'RestoreAllFilesToolStripMenuItem
        '
        Me.RestoreAllFilesToolStripMenuItem.Image = CType(resources.GetObject("RestoreAllFilesToolStripMenuItem.Image"), System.Drawing.Image)
        Me.RestoreAllFilesToolStripMenuItem.Name = "RestoreAllFilesToolStripMenuItem"
        Me.RestoreAllFilesToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.RestoreAllFilesToolStripMenuItem.Text = "Restore all Watcher Files"
        '
        'ViewPropertiesToolStripMenuItem
        '
        Me.ViewPropertiesToolStripMenuItem.Image = CType(resources.GetObject("ViewPropertiesToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ViewPropertiesToolStripMenuItem.Name = "ViewPropertiesToolStripMenuItem"
        Me.ViewPropertiesToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.ViewPropertiesToolStripMenuItem.Text = "Watcher Properties"
        '
        'AddWatchToolStripMenuItem
        '
        Me.AddWatchToolStripMenuItem.Image = CType(resources.GetObject("AddWatchToolStripMenuItem.Image"), System.Drawing.Image)
        Me.AddWatchToolStripMenuItem.Name = "AddWatchToolStripMenuItem"
        Me.AddWatchToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.AddWatchToolStripMenuItem.Text = "Add Watcher"
        '
        'DeleteWatchToolStripMenuItem
        '
        Me.DeleteWatchToolStripMenuItem.Image = CType(resources.GetObject("DeleteWatchToolStripMenuItem.Image"), System.Drawing.Image)
        Me.DeleteWatchToolStripMenuItem.Name = "DeleteWatchToolStripMenuItem"
        Me.DeleteWatchToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.DeleteWatchToolStripMenuItem.Text = "Delete Watcher"
        '
        'PauseResumeWatchToolStripMenuItem
        '
        Me.PauseResumeWatchToolStripMenuItem.Image = CType(resources.GetObject("PauseResumeWatchToolStripMenuItem.Image"), System.Drawing.Image)
        Me.PauseResumeWatchToolStripMenuItem.Name = "PauseResumeWatchToolStripMenuItem"
        Me.PauseResumeWatchToolStripMenuItem.Size = New System.Drawing.Size(201, 22)
        Me.PauseResumeWatchToolStripMenuItem.Text = "Pause/Resume Watcher"
        '
        'HelpProvider1
        '
        Me.HelpProvider1.HelpNamespace = "AutoVer.chm"
        '
        'tmrEngineUpdates
        '
        Me.tmrEngineUpdates.Enabled = True
        Me.tmrEngineUpdates.Interval = 10000
        '
        'bwBackupRestore
        '
        '
        'tmrEnsureSyncUpdates
        '
        Me.tmrEnsureSyncUpdates.Interval = 1000
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(474, 266)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.lvwWatches)
        Me.HelpButton = True
        Me.HelpProvider1.SetHelpKeyword(Me, "2")
        Me.HelpProvider1.SetHelpNavigator(Me, System.Windows.Forms.HelpNavigator.TopicId)
        Me.HelpProvider1.SetHelpString(Me, "2")
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.HelpProvider1.SetShowHelp(Me, True)
        Me.Text = "AutoVer"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ContextMenuStrip2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lvwWatches As System.Windows.Forms.ListView
    Friend WithEvents lvcName As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents tsbAdd As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsbProperties As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsbDelete As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsbExplore As System.Windows.Forms.ToolStripButton
    Friend WithEvents tspAbout As System.Windows.Forms.ToolStripButton
    Friend WithEvents tspHelp As System.Windows.Forms.ToolStripButton
    Friend WithEvents SysTrayIcon As System.Windows.Forms.NotifyIcon
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents MainScreenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsbSettings As System.Windows.Forms.ToolStripButton
    Friend WithEvents ContextMenuStrip2 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ExporeBackupsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewPropertiesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AddWatchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeleteWatchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EnsureBackupIsCurrentToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpProvider1 As System.Windows.Forms.HelpProvider
    Friend WithEvents PauseResumeWatchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsbPause As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents tsbPauseAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsbEnsureBackup As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents tsbEnsureBackupALL As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SysTrayToolStripMenuPauseAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RestoreAllFilesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsbRestore As System.Windows.Forms.ToolStripButton
    Friend WithEvents tmrEngineUpdates As System.Windows.Forms.Timer
    Friend WithEvents bwBackupRestore As System.ComponentModel.BackgroundWorker
    Friend WithEvents lvcStatus As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvcStats As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvcMessage As System.Windows.Forms.ColumnHeader
    Friend WithEvents tmrEnsureSyncUpdates As System.Windows.Forms.Timer

End Class
