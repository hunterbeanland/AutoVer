Imports System.ComponentModel
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.Security.Principal
Imports System.IO
Imports System.Reflection
Imports System.Threading
'Imports System.Runtime.InteropServices

Public Class frmMain
    Private WithEvents Config As New AutoVer.ConfigEngine()
    Private WithEvents ConfigRemote As ConfigService.ConfigEngineClient
    Private host As ServiceHost
    Private Log As Logger
    Private WindowStateLast As FormWindowState
    Private WithEvents frmSettings As Settings
    Private WithEvents frmBackupRestoreRunning As BackupFilesWait
    Private bwEngIndex As Integer 'Background worker engine index
    Private BackupRestoreCancelling As Boolean 'User requests cancel (bubble down from form)

#Region " Events "
    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Log = Config.Log 'Use the config engines log which will update its log level from INFO to what is in the config file.
        Log.Info(My.Application.Info.ProductName & " " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build, "Starting")
        Config.LoadAppConfig()
        If Config.ConfigFolder <> Config.AppConfigFolder Then
            Log.Info(Config.AppConfigFolder & "; " & Config.ConfigFolder, "ConfigPath (App; Watchers)")
        Else
            Log.Info(Config.ConfigFolder, "ConfigPath")
        End If
        Log.Info(Log.LogLevel.ToString, "LogLevel")
        'Log.Info(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, "Lang")

        If My.Application.CommandLineArgs.Count = 0 OrElse Not My.Application.CommandLineArgs(0).ToLower.StartsWith("-ei") Then
            'If not Elevated Instance - other instance may still be shutting down.
            '.NET single instance apps conflict, do it ourselves here.
            Dim appInstances() As Process = Process.GetProcessesByName("AutoVer")
            If appInstances.Length > 1 Then
                MsgBox("AutoVer is already running!" & vbLf & "Right click the AutoVer icon in the System Tray to open", MsgBoxStyle.Information)
                Log.Info("AutoVer already running. Exiting", "Starting")
                Application.Exit()
            End If
        End If

        If File.Exists(Config.ConfigFolder & "AutoVer.xml") Then
            ' MyBase.Visible = False 'Hide from Alt-Tab
            ' MyBase.ShowInTaskbar = False
            'SysTrayIcon.BalloonTipText = "AutoVer has been minimised to the System tray. Right click for menu options."
            'SysTrayIcon.ShowBalloonTip(4000) 'don't show on startup or after first showing
            MyBase.WindowState = FormWindowState.Minimized
            WindowStateLast = FormWindowState.Minimized
            Me.ShowInTaskbar = False
            Me.Visible = False
        End If

        Dim wp As New WindowsPrincipal(WindowsIdentity.GetCurrent())
        If wp.IsInRole(WindowsBuiltInRole.Administrator) Then
            Log.Info("Admin", "UserRole")
        ElseIf wp.IsInRole(WindowsBuiltInRole.User) Then
            Log.Info("User", "UserRole")
        ElseIf wp.IsInRole(WindowsBuiltInRole.SystemOperator) Then
            Log.Info("SystemOperator", "UserRole")
        ElseIf wp.IsInRole(WindowsBuiltInRole.PowerUser) Then
            Log.Info("PowerUser", "UserRole")
        ElseIf wp.IsInRole(WindowsBuiltInRole.BackupOperator) Then
            Log.Info("BackupOperator", "UserRole")
        ElseIf wp.IsInRole(WindowsBuiltInRole.Guest) Then
            Log.Info("Guest", "UserRole")
        End If

        Dim zoneRemover As New ZoneStripper(Log)
        zoneRemover.Unblock("C:\Program Files\AutoVer\", False) 'Application.StartupPath
        If zoneRemover.ZoneIdRemoved > 0 Then Log.Info(zoneRemover.ZoneIdRemoved.ToString & " of " & zoneRemover.FilesScanned.ToString, "ZoneStripper")

        Log.DeleteLogs(6, 12)

        'Service running? Yes, connect to it. No, operate the backup engine directly
        If Config.AppConfigDefault("UseService", "1") = "1" Then
            Try
                Dim appProcs() As Process = Process.GetProcessesByName("AutoVerService")
                If appProcs.Length = 0 Then
                    Log.Debug("Service not running", "AutoVerService")
                    Config.IsService = False
                Else
                    Dim address As New EndpointAddress(New Uri("http://127.0.0.1:" & Config.AppConfigDefault("ServicePort", "9072") & "/AutoVer/mex"))
                    Dim binding As New WSHttpBinding
                    Dim sec As New WSHttpSecurity()
                    sec.Mode = SecurityMode.None
                    binding.Security = sec
                    ConfigRemote = New ConfigService.ConfigEngineClient(binding, address)

                    'ConfigRemote = New ConfigService.ConfigEngineClient("MetadataExchangeHttpBinding_IConfigEngine") '"http://127.0.0.1:" & Config.AppConfigDefault("ServicePort", "9072") & "/AutoVer/")
                    If IsNothing(ConfigRemote) Then
                        Log.Warn("Can't connect to service on port " & Config.AppConfigDefault("ServicePort", "9072") & ". Running local", "AutoVerService")
                        Config.IsService = False
                    Else
                        Log.Debug("Connected to service on port " & Config.AppConfigDefault("ServicePort", "9072"), "AutoVerService")
                        Log.Debug(ConfigRemote.GetIsService().ToString, "GetIsService")
                        Config.IsService = True
                        Config.IsMockWatchers = True
                    End If
                End If
                Config.LoadWatcherConfig()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Log.Warn("Can't connect to service on port " & Config.AppConfigDefault("ServicePort", "9072") & ". Running local", "AutoVerService")
                Log.Warn(ex.Message, "AutoVerService.Connect")
                Config.LoadWatcherConfig()
            End Try
        Else
            Config.LoadWatcherConfig()
        End If
        If Config.IsService Then
            Me.Text &= " (using AutoVer service)"
            Config.ArchiveDeleteTimer.Dispose()
        Else
            'Me.Text &= " (using AutoVer local)"
            If Config.AppConfigDefault("AutoElevate", "1") = "1" And Not wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator) Then
                'http://stackoverflow.com/questions/562350/requested-registry-access-is-not-allowed/562389#562389
                Dim proc As New Process()
                proc.StartInfo.FileName = Assembly.GetExecutingAssembly.Location
                proc.StartInfo.Arguments = "-ei"
                proc.StartInfo.UseShellExecute = True
                proc.StartInfo.Verb = "runas"
                proc.Start()
                Application.Exit()
            End If
        End If
        'For Each WatchEngine As BackupEngine In Config.WatcherEngines
        '    Log.Debug(WatchEngine.Name)
        'Next
        'Config.AutoVerGUI = Me
        RefreshWatchersStatus()

        'Config.Lang.Apply(Me, Nothing)
        'Config.Lang.Apply(ContextMenuStrip1, Nothing)
        'Config.Lang.Apply(ContextMenuStrip2, Nothing)

        'Dim frmComment As New VerComment()
        'frmComment.Show()
    End Sub

    Private Sub frmMain_Closing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not Config.IsService And Not e.CloseReason = CloseReason.WindowsShutDown And Not e.CloseReason = CloseReason.ApplicationExitCall Then
            'If MsgBox("Are you sure you wish to terminate AutoVer?" & vbLf & _
            '          "If Not, it will be minimised to the System tray and kept running in the background.", _
            '          MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            '    Log.Info(My.Application.Info.ProductName, "Closing")
            '    e.Cancel = False
            '    'My.MySettings.Default.ApplicationPosition = Me.Location
            '    'My.MySettings.Default.ApplicationSize = Me.Size
            '    ' If Not Config.IsService Then
            '    Try
            '        '    m_MediaConnectWatcher.Stop()
            '        For Each watchEngine As BackupEngine In Config.WatcherEngines
            '            Log.Info(watchEngine.Name & ": " & watchEngine.Stats, "Stats")
            '            watchEngine.StopWatching()
            '        Next
            '    Catch
            '    End Try
            '    'End If
            'Else
            e.Cancel = True
            MyBase.WindowState = FormWindowState.Minimized
            MyBase.ShowInTaskbar = False
            'End If
        End If
    End Sub

    Private Sub frmMain_Minimise(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.SizeChanged
        If WindowStateLast <> Me.WindowState Then
            WindowStateLast = Me.WindowState
            If Me.WindowState = FormWindowState.Minimized Then
                Me.ShowInTaskbar = False
                Me.Visible = False
            Else
                Me.ShowInTaskbar = True
                Me.Visible = True
                RefreshWatchersStatus()
                tmrEngineUpdates.Interval = 5000 ' 5 sec
            End If
        End If

        'If Me.WindowState = FormWindowState.Minimized Then
        '    'Hide me (check Me.Visible so it doesn't trigger this event in a loop)
        '    If Me.Visible Then
        '        Me.ShowInTaskbar = False 'MyBase
        '        'Hide from Alt-Tab
        '        'http://www.csharp411.com/hide-form-from-alttab/
        '        'Dim GWL_EXSTYLE As Integer = -20
        '        'Dim WS_EX_TOOLWINDOW As Integer = &H80
        '        'Dim WS_EX_APPWINDOW As Integer = &H40000
        '        'Dim windowStyle As Integer = GetWindowLong(Handle, GWL_EXSTYLE)
        '        'SetWindowLong(Handle, GWL_EXSTYLE, windowStyle Or WS_EX_TOOLWINDOW)

        '        ' me.FormBorderStyle = FormBorderStyle.FixedToolWindow; /FormBorderStyle.FixedSingle;
        '        Me.Visible = False
        '        'form1.Opacity = 0
        '    End If
        'ElseIf Not Me.Visible Then
        '    'Show
        '    Me.ShowInTaskbar = True
        '    Me.Visible = True
        '    RefreshWatchersStatus()
        'End If
    End Sub

    Private Sub ShowMessage(ByVal message As String) Handles Config.UserMesage
        SysTrayIcon.BalloonTipText = message
        SysTrayIcon.ShowBalloonTip(4000)
    End Sub

    Private Sub lvwWatches_ItemActivate(ByVal sender As Object, ByVal e As EventArgs) Handles lvwWatches.ItemActivate
        Me.Cursor = Cursors.WaitCursor
        ViewBackups()
        Thread.Sleep(1000)
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub SettingsUpdated() Handles frmSettings.SettingsUpdated
        'After the settings have been viewed and ok pressed, this event is called to update the rest of the system
        If Config.IsService Then
            Try
                ConfigRemote.ReloadWatcher(Guid.Empty)
            Catch ex As Exception
                Log.Warn(ex.Message, "ServiceConnect.ReloadWatcher")
                MsgBox("Error connecting to Windows Service", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "AutoVer Service")
            End Try
            'Else
            'Config.SaveWatcherList()
            'Config.UpdateWatchersList()
        End If
        RefreshWatchersStatus()
    End Sub

    'Toolbar strip events
    Private Sub tsbAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbAdd.Click
        'Dim cb As New delegatecall(AddressOf SettingsUpdated)
        'Dim frmSettings As New Settings(cb)
        frmSettings = New Settings
        'frmSettings.WatcherConfig = Config.WatcherConfig
        frmSettings.Config = Config
        frmSettings.ConfigRemote = ConfigRemote
        frmSettings.WatcherEngine = Nothing
        frmSettings.WatcherGUID = Guid.Empty
        frmSettings.Show()
    End Sub

    Private Sub tsbProperties_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbProperties.Click
        'Show the properties of the selected watcher.
        If lvwWatches.SelectedItems.Count = 0 Then
            MsgBox("You must select a watcher item first!", MsgBoxStyle.Information)
        Else
            ViewProperties()
        End If
    End Sub

    Private Sub tsbDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbDelete.Click
        If lvwWatches.SelectedItems.Count = 0 Then
            MsgBox("You must select a watcher item first!")
        Else
            If MsgBox("Are you sure you wish to delete this watcher?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = _
                MsgBoxResult.Yes Then DeleteWatcher()
        End If
    End Sub

    Private Sub tsbEnsureBackup_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbEnsureBackup.ButtonClick
        'Ensure the backup of the selected watcher.
        If lvwWatches.SelectedItems.Count = 0 Then
            MsgBox("You must select a watcher item first!", MsgBoxStyle.Information)
        Else
            If MsgBox("Are you sure you wish ensure this backup is up to date?", _
                       MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then EnsureBackup()
        End If
    End Sub

    Private Sub tsbEnsureBackupAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbEnsureBackupALL.Click
        'Ensure the backup of all watchers.
        If MsgBox("Are you sure you wish ensure all backups are up to date?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = _
            MsgBoxResult.Yes Then EnsureAll()
    End Sub

    Private Sub tsbRestore_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbRestore.Click
        'Restore the backup of the selected watcher.
        If lvwWatches.SelectedItems.Count = 0 Then
            MsgBox("You must select a watcher item first!", MsgBoxStyle.Information)
        Else
            'If MsgBox("Are you sure you wish to restore all files from the backup?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then RestoreAll()
            RestoreAll()
        End If
    End Sub

    Private Sub tsbExplore_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbExplore.Click
        If lvwWatches.SelectedItems.Count = 0 Then
            MsgBox("You must select a watcher item first!", MsgBoxStyle.Information)
        Else
            ViewBackups()
        End If
    End Sub

    Private Sub tsbSettings_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbSettings.Click
        Dim applicSettings As New AppSettings(Config)
        applicSettings.Show()
    End Sub

    Private Sub tspAbout_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tspAbout.Click
        Dim frmAbout As New About
        frmAbout.Show()
    End Sub

    Private Sub tspHelp_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tspHelp.Click
        'System.Diagnostics.Process.Start(Application.StartupPath & "\AutoVerHelp.htm", "#Main")
        Help.ShowHelp(Me, "AutoVer.chm", HelpNavigator.TopicId, "2")
    End Sub

    Private Sub tsbPause_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbPause.ButtonClick
        'Toggle the pause/resume of a watcher
        If lvwWatches.SelectedItems.Count = 0 Then
            MsgBox("You must select a watcher item first!", MsgBoxStyle.Information)
        Else
            PauseResume(False)
        End If
    End Sub

    Private Sub tsbPauseAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbPauseAll.Click
        'Toggle the pause/resume of all watchers
        PauseResume(True)
    End Sub

    'Context menu
    Private Sub lvwWatches_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lvwWatches.MouseClick
        If e.Button = MouseButtons.Right Then
            ContextMenuStrip2.Show(lvwWatches, e.Location.X, e.Location.Y)
        End If
    End Sub

    Private Sub ExporeBackupsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ExporeBackupsToolStripMenuItem.Click
        tsbExplore_Click(Nothing, Nothing)
    End Sub

    Private Sub ViewPropertiesToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ViewPropertiesToolStripMenuItem.Click
        tsbProperties_Click(Nothing, Nothing)
    End Sub

    Private Sub AddWatchToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles AddWatchToolStripMenuItem.Click
        tsbAdd_Click(Nothing, Nothing)
    End Sub

    Private Sub DeleteWatchToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles DeleteWatchToolStripMenuItem.Click
        tsbDelete_Click(Nothing, Nothing)
    End Sub

    Private Sub EnsureBackupIsCurrentToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles EnsureBackupIsCurrentToolStripMenuItem.Click
        tsbEnsureBackup_Click(Nothing, Nothing)
    End Sub

    Private Sub RestoreAllFilesToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles RestoreAllFilesToolStripMenuItem.Click
        tsbRestore_Click(Nothing, Nothing)
    End Sub

    Private Sub PauseResumeWatchToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles PauseResumeWatchToolStripMenuItem.Click
        tsbPause_Click(Nothing, Nothing)
    End Sub

    Private Sub SysTrayToolStripMenuPauseAll_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles SysTrayToolStripMenuPauseAll.Click
        tsbPauseAll_Click(Nothing, Nothing)
    End Sub

    'System tray Context menu
    Private Sub NotifyIcon1_Click(ByVal sender As Object, ByVal e As MouseEventArgs) Handles SysTrayIcon.MouseClick
        'Do nothing if context click
        If e.Button = MouseButtons.Left Then
            NotifyIcon1_DoubleClick(sender, Nothing)
        End If
    End Sub

    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles SysTrayIcon.DoubleClick
        If Me.WindowState <> FormWindowState.Minimized Then
            WindowStateLast = FormWindowState.Minimized
            Me.WindowState = FormWindowState.Minimized
            Me.ShowInTaskbar = False
            Me.Visible = False
        Else
            WindowStateLast = FormWindowState.Normal
            Me.ShowInTaskbar = True
            Me.Visible = True
            Me.WindowState = FormWindowState.Normal
            RefreshWatchersStatus()
        End If
    End Sub

    'Private Sub SysTrayIcon_LeftClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles SysTrayIcon.Click
    '    Dim pnt As System.Drawing.Point = Cursor.Position
    '    pnt.X -= ContextMenuStrip1.Width
    '    pnt.Y -= ContextMenuStrip1.Height
    '    ContextMenuStrip1.Show(pnt)
    'End Sub

    Private Sub MainScreenToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles MainScreenToolStripMenuItem.Click
        WindowStateLast = FormWindowState.Normal
        Me.ShowInTaskbar = True
        Me.Visible = True
        Me.WindowState = FormWindowState.Normal
        RefreshWatchersStatus()
    End Sub

    'Private Sub ExploreBackupsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
    'Dim frmVerExplore As New VerExplorer
    'frmVerExplore.Show()
    'End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub


#End Region

    'Private Delegate Sub UpdateWatchersListCallback()
    Private Sub tmrEngineUpdates_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrEngineUpdates.Tick
        'Update the engine status depending how visible we are
        ' If Me.Visible And Me.Focused Then
        ' tmrEngineUpdates.Interval = 5000 ' 5 sec
        ' Else
        If Me.Visible Then
            tmrEngineUpdates.Interval = 15000 '15 sec
        Else
            tmrEngineUpdates.Interval = 10 * 60000 '10 min
        End If
        RefreshWatchersStatus()
    End Sub

    Private Sub RefreshWatchersStatus()
        'Update watch list
        Dim lastSelectedWatcher As Guid
        If lvwWatches.SelectedItems.Count = 0 Then
            lastSelectedWatcher = Guid.Empty
        Else
            lastSelectedWatcher = lvwWatches.SelectedItems(0).Tag
        End If
        If Config.IsService Then
            Try
                Dim ws() As WatcherStatus = ConfigRemote.GetWatcherStatus
                lvwWatches.BeginUpdate()
                lvwWatches.Items.Clear()
                For Each wsi As WatcherStatus In ws
                    Dim lvi As New ListViewItem()
                    lvi.Tag = wsi.WatcherId
                    lvi.Text = wsi.Name
                    If Not wsi.Enabled Then
                        lvi.SubItems.Add("Disabled")
                        lvi.SubItems(0).ForeColor = Color.Orange
                    ElseIf wsi.Paused Then
                        lvi.SubItems.Add("Paused")
                        lvi.SubItems(0).ForeColor = Color.Goldenrod
                    ElseIf Not wsi.Started And wsi.Enabled Then
                        lvi.SubItems.Add("Error!")
                        lvi.SubItems(0).ForeColor = Color.Red
                    ElseIf wsi.BackupFolderFailure Then
                        lvi.SubItems.Add("Copying Warnings")
                        lvi.SubItems(0).ForeColor = Color.Orange
                    Else
                        lvi.SubItems.Add("OK")
                        lvi.SubItems(0).ForeColor = Color.Green
                    End If
                    lvi.SubItems.Add(wsi.TotalEvents)
                    lvi.SubItems.Add(wsi.UserMessage)
                    If wsi.WatcherId = lastSelectedWatcher Then lvi.Selected = True
                    lvwWatches.Items.Add(lvi)
                Next
            Catch ex As Exception
                Log.Warn(ex.Message, "ServiceConnect.GetWatcherStatus")
                SysTrayIcon.BalloonTipText = "Error connecting to Windows Service"
                SysTrayIcon.ShowBalloonTip(4000)
            End Try
        Else
            lvwWatches.BeginUpdate()
            lvwWatches.Items.Clear()
            For Each watchEngine As BackupEngine In Config.WatcherEngines
                Dim lvi As New ListViewItem()
                lvi.Tag = watchEngine.GUID
                lvi.Text = watchEngine.Name
                If Not watchEngine.Enabled Then
                    lvi.SubItems.Add("Disabled")
                    lvi.SubItems(0).ForeColor = Color.Orange
                ElseIf watchEngine.Paused Then
                    lvi.SubItems.Add("Paused")
                    lvi.SubItems(0).ForeColor = Color.Goldenrod
                ElseIf Not watchEngine.Started And watchEngine.Enabled Then
                    lvi.SubItems.Add("Error!")
                    lvi.SubItems(0).ForeColor = Color.Red
                ElseIf watchEngine.BackupFolderFail Then
                    lvi.SubItems.Add("Copying Warnings")
                    lvi.SubItems(0).ForeColor = Color.Orange
                Else
                    lvi.SubItems.Add("OK")
                    lvi.SubItems(0).ForeColor = Color.Green
                End If
                lvi.SubItems.Add((watchEngine.CountChanged + watchEngine.CountRenamed + watchEngine.CountDeleted).ToString)
                lvi.SubItems.Add(watchEngine.Message)
                If watchEngine.GUID = lastSelectedWatcher Then lvi.Selected = True
                lvwWatches.Items.Add(lvi)
            Next
        End If
        lvwWatches.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        For intCol As Integer = 0 To lvwWatches.Columns.Count - 1
            If lvwWatches.Columns(intCol).Width < 70 Then lvwWatches.Columns(intCol).Width = 70
        Next
        'If lvwWatches.Columns(2).Width < 45 Then lvwWatches.Columns(2).Width = 45
        lvwWatches.EndUpdate()
    End Sub

    'Private Sub AddWatchToList(ByRef WatchEngine As BackupEngine)
    '    Dim lvi As New ListViewItem()
    '    lvi.Tag = WatchEngine.GUID
    '    lvi.Text = WatchEngine.Name
    '    If Not WatchEngine.Enabled Then
    '        lvi.SubItems.Add("Disabled")
    '        lvi.SubItems(0).ForeColor = Color.Orange
    '    ElseIf Not WatchEngine.Started And WatchEngine.Enabled Then
    '        lvi.SubItems.Add("Error!")
    '        lvi.SubItems(0).ForeColor = Color.Red
    '    ElseIf WatchEngine.BackupFolderFail Then
    '        lvi.SubItems.Add("Copying Warnings")
    '        lvi.SubItems(0).ForeColor = Color.Orange
    '    Else
    '        lvi.SubItems.Add("OK")
    '        lvi.SubItems(0).ForeColor = Color.Green
    '    End If
    '    If WatchEngine.GUID = Config.LastSelectedWatcher Then lvi.Selected = True
    '    lvwWatches.Items.Add(lvi)
    'End Sub

    Private Sub ViewProperties()
        'Show the properties of the selected watcher.
        If Not IsNothing(frmSettings) Then
            frmSettings.Close()
        End If
        Dim lastSelectedWatcher As Guid
        If lvwWatches.SelectedItems.Count = 0 Then
            lastSelectedWatcher = Guid.Empty
        Else
            lastSelectedWatcher = lvwWatches.SelectedItems(0).Tag
        End If
        frmSettings = New Settings
        Dim we As List(Of BackupEngine) = Config.WatcherEngines
        For Each watchEngine As BackupEngine In we
            If watchEngine.GUID = lastSelectedWatcher Then
                frmSettings.WatcherEngine = watchEngine
            End If
        Next
        frmSettings.Config = Config
        frmSettings.ConfigRemote = ConfigRemote
        ' frmSettings.WatcherConfig = Config.WatcherConfig
        frmSettings.WatcherGUID = lastSelectedWatcher
        frmSettings.Show()
    End Sub

    Private Sub DeleteWatcher()
        'Delete the watcher and save
        If Config.IsService Then
            Try
                ConfigRemote.DeleteWatcher(lvwWatches.SelectedItems(0).Tag)
            Catch ex As Exception
                Log.Warn(ex.Message, "ServiceConnect.DeleteWatcher")
                MsgBox("Error connecting to Windows Service", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "AutoVer Service")
            End Try
        Else
            Config.DeleteWatcher(lvwWatches.SelectedItems(0).Tag)
            'For Each watchEngine As BackupEngine In Config.WatcherEngines
            '    If watchEngine.GUID = lvwWatches.SelectedItems(0).Tag Then
            '        watchEngine.StopWatching()
            '    End If
            'Next
            'Dim dtWatcherConfig As DataTable = Config.WatcherConfig
            'For intRow As Int16 = 0 To dtWatcherConfig.Rows.Count - 1
            '    If lvwWatches.SelectedItems(0).Tag = dtWatcherConfig.Rows(intRow)("GUID") Then
            '        Log.Info(dtWatcherConfig.Rows(intRow)("WatchFolder"), "DeleteWatcher")
            '        lvwWatches.SelectedItems(0).Remove()
            '        dtWatcherConfig.Rows(intRow).Delete()
            '        Exit For
            '    End If
            'Next
            'Config.WatcherConfig = dtWatcherConfig
            'Config.SaveWatcherConfig()
        End If
        RefreshWatchersStatus()
    End Sub

    Private Sub PauseResume(ByVal applyToAll As Boolean)
        'Toggle pause/resume of the selected watcher.
        Me.Cursor = Cursors.WaitCursor
        If Config.IsService Then
            Try
                If applyToAll Then
                    ConfigRemote.PauseWatcher(Guid.Empty)
                Else
                    ConfigRemote.PauseWatcher(lvwWatches.SelectedItems(0).Tag)
                End If
            Catch ex As Exception
                Log.Warn(ex.Message, "ServiceConnect.PauseWatcher")
                MsgBox("Error connecting to Windows Service", MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "AutoVer Service")
            End Try
        Else
            If applyToAll Then
                Config.PauseWatcher(Guid.Empty)
            Else
                Config.PauseWatcher(lvwWatches.SelectedItems(0).Tag)
            End If
        End If
        RefreshWatchersStatus()
        'If ApplyToAll Then
        '    For Each WatchEngine As BackupEngine In Config.WatcherEngines
        '        If WatchEngine.Started Then
        '            WatchEngine.StopWatching()
        '            For Each li As ListViewItem In lvwWatches.Items
        '                If li.Tag = WatchEngine.GUID Then
        '                    li.SubItems(1).Text = "Paused"
        '                    li.SubItems(0).ForeColor = Color.Orange
        '                End If
        '            Next
        '        ElseIf WatchEngine.Enabled Then
        '            WatchEngine.SetupWatcher()
        '            For Each li As ListViewItem In lvwWatches.Items
        '                If li.Tag = WatchEngine.GUID Then
        '                    If Not WatchEngine.Started Then
        '                        li.SubItems(1).Text = "Error"
        '                        li.SubItems(0).ForeColor = Color.Red
        '                    Else
        '                        li.SubItems(1).Text = "OK"
        '                        li.SubItems(0).ForeColor = Color.Green
        '                    End If
        '                End If
        '            Next
        '        End If
        '    Next
        'Else
        '    For Each WatchEngine As BackupEngine In Config.WatcherEngines
        '        If lvwWatches.SelectedItems(0).Tag = WatchEngine.GUID Then
        '            If WatchEngine.Started Then
        '                WatchEngine.StopWatching()
        '                lvwWatches.SelectedItems(0).SubItems(1).Text = "Paused"
        '                lvwWatches.SelectedItems(0).SubItems(0).ForeColor = Color.Orange
        '            ElseIf WatchEngine.Enabled Then
        '                WatchEngine.SetupWatcher()
        '                If Not WatchEngine.Started Then
        '                    lvwWatches.SelectedItems(0).SubItems(1).Text = "Error"
        '                    lvwWatches.SelectedItems(0).SubItems(0).ForeColor = Color.Red
        '                Else
        '                    lvwWatches.SelectedItems(0).SubItems(1).Text = "OK"
        '                    lvwWatches.SelectedItems(0).SubItems(0).ForeColor = Color.Green
        '                End If
        '            End If
        '            Exit For
        '        End If
        '    Next
        'End If
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub ViewBackups()
        'Load backup explorer
        Dim lastSelectedWatcher As Guid
        If lvwWatches.SelectedItems.Count = 0 Then
            lastSelectedWatcher = Guid.Empty
        Else
            lastSelectedWatcher = lvwWatches.SelectedItems(0).Tag
        End If
        Dim frmVerExplore As New VerExplorer()
        For Each watchEngine As BackupEngine In Config.WatcherEngines
            If watchEngine.GUID = lastSelectedWatcher Then
                frmVerExplore.WatchEngine = watchEngine
                frmVerExplore.Config = Config
                frmVerExplore.WatchFolder = watchEngine.WatchFolder
                frmVerExplore.BackupFolder = watchEngine.BackupFolder
                frmVerExplore.Show()
                Exit For
            End If
        Next
    End Sub

    Private Sub EnsureBackup()
        'Ensure the backup of the selected watcher.
        Me.Cursor = Cursors.WaitCursor
        Dim dtWatcherConfig As DataTable = Config.WatcherConfig
        For intRow As Int16 = 0 To dtWatcherConfig.Rows.Count - 1
            If lvwWatches.SelectedItems(0).Tag = dtWatcherConfig.Rows(intRow)("GUID") Then
                'Try
                '    'HKEY_LOCAL_MACHINE
                '    Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\AutoVer", "LastArchive", Now.ToString)
                'Catch ex As Exception
                '    Log.Error(ex.Message, "Can not set HKCU\Software\AutoVer in registry:")
                'End Try
                If Config.IsService Then
                    'If Me.WindowState <> FormWindowState.Minimized Then
                    '    ShowBackupFilesWait(intRow)
                    'End If
                    'Config.WatcherEngines(intRow).EnsureBackupCurrent()
                    'If Not BackupRestoreCancelling Then Config.WatcherEngines(intRow).DeleteArchiveVersions()
                Else
                    'local mode. Use background thread
                    If Me.WindowState <> FormWindowState.Minimized Then
                        BackgroungWorkerRun("ENSURE", intRow)
                    Else
                        BackgroungWorkerRun("ENSURE_SILENT", intRow)
                    End If
                End If
                Exit For
            End If
        Next
        Me.Cursor = Cursors.Default
    End Sub

    Public Sub BackgroungWorkerRun(ByVal strMode As String, ByVal EngIndex As Integer)
        'Run on background thread so UI thread is free
        If bwBackupRestore.IsBusy Then
            Log.Warn("BackupRestore thread already busy. Cannot start " & strMode, "BackgroundWorker")
            MessageBox.Show("AutoVer is currently doing a Backup/Restore operation. Retry later.", "Backup/Restore", _
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            If Not strMode.EndsWith("SILENT") Then
                'Display the Backing up files, please wait dialog. Start the updates timer
                ShowBackupFilesWait(EngIndex)
            End If
            bwEngIndex = EngIndex
            bwBackupRestore.RunWorkerAsync(strMode)
        End If
    End Sub

    Private Sub bwBackupRestore_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bwBackupRestore.DoWork
        Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)
        Log.Debug(e.Argument, "BackupRestoreWorker")
        If e.Argument.ToString.StartsWith("ENSURE") Then
            Config.WatcherEngines(bwEngIndex).EnsureBackupCurrent()
            If Not BackupRestoreCancelling Then Config.WatcherEngines(bwEngIndex).DeleteArchiveVersions()
        ElseIf e.Argument.ToString.StartsWith("ARCHIVE/DELETE") Then
            Config.WatcherEngines(bwEngIndex).DeleteArchiveVersions()
        End If
        If BackupRestoreCancelling Then e.Cancel = True
        e.Result = e.Argument '& "-OK"
    End Sub

    Private Sub bwBackupRestore_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bwBackupRestore.RunWorkerCompleted
        Log.Debug(e.Result, "BackupRestoreWorker")
        If (e.Error IsNot Nothing) Then
            Log.Warn(e.Error.Message, "BackgroundWorker")
            MessageBox.Show(e.Error.Message)
        ElseIf e.Cancelled Then
            Log.Warn("User Cancelled", "BackgroundWorker")
        End If
        If Not IsNothing(frmBackupRestoreRunning) Then frmBackupRestoreRunning.Close()
    End Sub

    Private Sub ShowBackupFilesWait(ByVal EngIndex As Integer)
        'Display the Backing up files, please wait dialog. Start the updates timer
        BackupRestoreCancelling = False
        frmBackupRestoreRunning = New BackupFilesWait()
        frmBackupRestoreRunning.WaitMessage = "Ensuring backup is current..."
        frmBackupRestoreRunning.EngineIndex = EngIndex
        frmBackupRestoreRunning.FormTitle = "AutoVer Syncing Backup"
        frmBackupRestoreRunning.Show()
        tmrEnsureSyncUpdates.Start()
    End Sub

    Private Sub tmrEnsureSyncUpdates_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrEnsureSyncUpdates.Tick
        'Backup/Restore: Polling the backup engine status every 1 sec
        frmBackupRestoreRunning.WaitMessage = Config.WatcherEngines(frmBackupRestoreRunning.EngineIndex).EnsureMessage
        If Config.WatcherEngines(frmBackupRestoreRunning.EngineIndex).EnsureRestoreCancelled Then
            tmrEnsureSyncUpdates.Stop()
            If Not IsNothing(frmBackupRestoreRunning) Then frmBackupRestoreRunning.Close()
        End If
    End Sub

    Private Sub RestoreAll()
        'Restore all files (or latest version of) on the selected watcher only
        Me.Cursor = Cursors.WaitCursor
        Dim dtWatcherConfig As DataTable = Config.WatcherConfig
        For intRow As Int16 = 0 To dtWatcherConfig.Rows.Count - 1
            If lvwWatches.SelectedItems(0).Tag = dtWatcherConfig.Rows(intRow)("GUID") Then
                Dim frmRestoreAll As New RestoreAll()
                frmRestoreAll.WatcherEngine = Config.WatcherEngines(intRow)
                frmRestoreAll.Show()
                'WatcherEngines(intRow).RestoreAll(Now.AddYears(10)) 'cover wrong clocks
                Exit For
            End If
        Next
        Me.Cursor = Cursors.Default
    End Sub

    Private Sub EnsureAll()
        'Ensure all files on all watchers
        'SysTrayIcon.BalloonTipText = "AutoVer is archiving..."
        Me.Cursor = Cursors.WaitCursor
        For intRow As Int16 = 0 To Config.WatcherConfig.Rows.Count - 1
            If Config.WatcherEngines(intRow).Enabled Then
                If Config.IsService Then
                    If Me.WindowState <> FormWindowState.Minimized Then ShowBackupFilesWait(intRow)
                    Config.WatcherEngines(intRow).EnsureBackupCurrent()
                    If Not BackupRestoreCancelling Then Config.WatcherEngines(intRow).DeleteArchiveVersions()
                Else
                    'local mode. Use background thread
                    If Me.WindowState <> FormWindowState.Minimized Then
                        BackgroungWorkerRun("ENSURE", intRow)
                    Else
                        BackgroungWorkerRun("ENSURE_SILENT", intRow)
                    End If
                End If
            End If
        Next
        'SysTrayIcon.Text = "AutoVer"
        Me.Cursor = Cursors.Default
    End Sub
End Class
