'Imports system.IO
Imports Alphaleonis.Win32
Imports System.ComponentModel

Public Class Settings
    'Friend WatcherConfig As DataTable
    Friend WatcherGUID As System.Guid
    Friend WatcherEngine As BackupEngine
    Public Config As ConfigEngine
    Public ConfigRemote As ConfigService.ConfigEngineClient
    Public Event SettingsUpdated()
    Private MaxVersionAge As UInt16
    Private MaxFileSize As UInt64
    Private WatcherEngineTemp As BackupEngine
    Private WithEvents frmBackupRestoreRunning As BackupFilesWait

    Private Sub Settings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim pfx86 As String = Environment.GetEnvironmentVariable("ProgramFiles(x86)")
        If String.IsNullOrEmpty(pfx86) Then pfx86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        If Not (IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\7-Zip\7z.dll") OrElse _
           IO.File.Exists(pfx86 & "\7-Zip\7z.dll") OrElse _
           IO.File.Exists(Application.StartupPath() & "\" & "7z.dll")) Then
            rad7Zip.Enabled = False
        End If

        If Not IsNothing(WatcherEngine) Then 'WatcherGUID <> Guid.Empty Then
            'For Each drWatcher As DataRow In WatcherConfig.Rows
            'If drWatcher("GUID") = WatcherGUID Then
            Me.Text = "AutoVer: " & WatcherEngine.Name
            txtName.Text = WatcherEngine.Name
            chkEnabled.Checked = WatcherEngine.Enabled
            txtWatch.Text = Filesystem.Path.GetRegularPath(WatcherEngine.WatchFolder)
            If WatcherEngine.FTPEnable Then
                txtBackup.Text = WatcherEngine.BackupFolder
            Else
                txtBackup.Text = Filesystem.Path.GetRegularPath(WatcherEngine.BackupFolder)
            End If
            If WatcherEngine.VersionFiles Then
                rbVersionAll.Checked = True
            Else
                If WatcherEngine.VersionPrev Then
                    rbVersionPrev.Checked = True
                Else
                    rbNone.Checked = True
                End If
            End If
            txtIncludeFiles.Text = WatcherEngine.IncludeFiles
            txtExcludeFiles.Text = WatcherEngine.ExcludeFiles
            txtExcludeFolders.Text = WatcherEngine.ExcludeFolders
            txtDateTimeStamp.Text = WatcherEngine.DateTimeStamp
            txtMaxVersionAge.Text = WatcherEngine.MaxVersionAge
            Select Case WatcherEngine.MaxVersionAction
                Case "D"c
                    radDelete.Checked = True
                Case "Z"c
                    radZip.Checked = True
                Case "7"c
                    rad7Zip.Checked = True
                Case Else
                    radNothing.Checked = True
            End Select
            txtSettleDelay.Text = WatcherEngine.SettleDelay / 1000
            txtRunCopy.Text = WatcherEngine.RunCopy & IIf(String.IsNullOrEmpty(WatcherEngine.RunCopyArgs), String.Empty, " " & WatcherEngine.RunCopyArgs)
            chkRunCopyFirst.Checked = WatcherEngine.RunCopyFirst
            txtMaxFileSize.Text = WatcherEngine.MaxFileSize
            chkSubFolders.Checked = WatcherEngine.SubFolders
            chkDelete.Checked = WatcherEngine.DeleteOnDelete
            chkCompareBeforeCopy.Checked = WatcherEngine.CompareBeforeCopy
            Select Case Microsoft.VisualBasic.Left(WatcherEngine.EnsureSchedule, 1)
                Case "D"
                    rbEnsureDaily.Checked = True
                    If Len(WatcherEngine.EnsureSchedule) > 1 Then txtEnsureTime.Text = Microsoft.VisualBasic.Right(WatcherEngine.EnsureSchedule, Len(WatcherEngine.EnsureSchedule) - 1)
                Case "H"
                    rbEnsureHourly.Checked = True
                    'txtEnsureTime.Enabled = False
                Case Else
                    rbEnsureNever.Checked = True
                    'txtEnsureTime.Enabled = False
            End Select
            If WatcherEngine.VersionRate = 0 Then
                rbVerAll.Checked = True
                txtVerRateS.Text = "0"
            Else
                rbVerRate.Checked = True
                Dim intVerRate As Integer = WatcherEngine.VersionRate Mod 60
                txtVerRateS.Text = intVerRate.ToString
                intVerRate = WatcherEngine.VersionRate \ 60
                txtVerRateM.Text = (intVerRate Mod 60).ToString
                txtVerRateH.Text = (intVerRate \ 60).ToString
            End If
            If WatcherEngine.ZipMode = "W" Then
                rbZipModeW.Checked = True
            ElseIf WatcherEngine.ZipMode = "D" Then
                rbZipModeD.Checked = True
            Else
                rbZipModeF.Checked = True
            End If
            chkShowErrors.Checked = WatcherEngine.ShowErrors
            chkShowEvents.Checked = WatcherEngine.ShowEvents
            rbFTP.Checked = WatcherEngine.FTPEnable
            txtFTPHost.Text = WatcherEngine.FTPHost
            txtFTPLogin.Text = WatcherEngine.FTPUser
            txtFTPPassword.Text = WatcherEngine.FTPPass 'General.FromHex(
            chkFTPPassive.Checked = WatcherEngine.FTPPassive
            If Config.IsService Then
                Try
                    Dim ws() As WatcherStatus = ConfigRemote.GetWatcherStatus
                    For Each wsi As WatcherStatus In ws
                        If WatcherEngine.GUID = wsi.WatcherId Then
                            lblCounts.Text = wsi.Statistics.Replace(", ", vbCrLf)
                            Exit For
                        End If
                    Next
                Catch
                    lblCounts.Text = "Service Error"
                End Try
            Else
                lblCounts.Text = WatcherEngine.Stats.Replace(", ", vbCrLf)
            End If
            Dim strError As String = CheckSettings()
            If WatcherEngine.BackupFolderFail Then
                If strError <> String.Empty Then strError &= vbCrLf
                strError = "This watcher has current file copying warnings. AutoVer will continue to retry the copy operations for a finite time (see Log/Help)."
            End If
            If strError <> String.Empty Then MsgBox(strError, MsgBoxStyle.Critical)
            'Exit For
            'End If
            'Next
        Else
            Me.Text = "New Watcher Settings"
        End If
        'Config.Lang.ExportControlsToFile(Me, ToolTip1)
    End Sub

    Private Sub txtWatch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtWatch.LostFocus
        Dim strError As String = String.Empty
        txtWatch.Text = txtWatch.Text.Trim
        Dim strPath As String = General.PathToAbsolute(txtWatch.Text, Application.StartupPath)
        If strPath.Length < 2 OrElse Not IO.Directory.Exists(strPath) Then strError = "Watch folder is invalid: " & strPath
        If strError <> String.Empty Then MsgBox(strError, MsgBoxStyle.Exclamation)
    End Sub

    Private Sub txtBackup_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBackup.LostFocus

    End Sub

    Private Function CheckSettings() As String
        Dim strError As String = String.Empty
        If txtName.Text.Trim = String.Empty Then txtName.Text = txtWatch.Text
        txtDateTimeStamp.Text = txtDateTimeStamp.Text.Replace(":", ";").Replace(".", ",").Replace("\", "-").Replace("/", "-")
        txtWatch.Text = txtWatch.Text.Trim.Replace("/", "\")
        Dim strWFolder As String = General.PathToAbsolute(txtWatch.Text, Application.StartupPath)
        If strWFolder.Length < 2 OrElse Not Filesystem.Directory.Exists(strWFolder) Then strError = "Watch folder is invalid: " & strWFolder
        If strWFolder.ToLower = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 3).ToLower Then strError = "Watching all of a drive where Windows is installed is not allowed"
        If Environment.SystemDirectory.StartsWith(strWFolder) Then strError = "Can not watch Windows folder"
        If txtIncludeFiles.Text.Trim = String.Empty Then txtIncludeFiles.Text = "*.*"
        If txtMaxFileSize.Text.Trim = String.Empty Then txtMaxFileSize.Text = "1G"
        If Not UInt64.TryParse(cNumber(txtMaxFileSize.Text, True), MaxFileSize) Then strError = "Invalid maximum file size"
        If txtDateTimeStamp.Text = String.Empty Then txtDateTimeStamp.Text = "yyMMddHHmmss"
        If txtMaxVersionAge.Text = String.Empty Then txtMaxVersionAge.Text = "90"
        If Not UInt32.TryParse(cNumber(txtMaxVersionAge.Text, False), MaxVersionAge) Then strError = "Invalid maximum version age"
        If rbFTP.Checked Then
            txtBackup.Text = txtBackup.Text.Trim
            If txtBackup.Text = String.Empty Or Not txtBackup.Text.EndsWith("/") Then txtBackup.Text &= "/"
            txtFTPHost.Text = txtFTPHost.Text.Trim
            'If Not txtFTPHost.Text.ToLower.StartsWith("ftp://") Then txtFTPHost.Text = "ftp://" & txtFTPHost.Text
            If txtFTPLogin.Text.Trim = String.Empty And txtFTPPassword.Text.Trim = String.Empty Then txtFTPPassword.Text = My.Computer.Name & ".AutoVer"
            If txtFTPHost.Text = String.Empty Then
                strError = "All FTP details required!"
            Else
                Try
                    Dim ftp As New Utilities.FTP.FTPclient(txtFTPHost.Text, txtFTPLogin.Text, txtFTPPassword.Text)
                    ftp.UsePassive = chkFTPPassive.Checked
                    If Not ftp.FtpDirectoryExists(txtBackup.Text) Then strError = "Backup FTP path is invalid"
                Catch ex As Exception
                    strError = "FTP connection invalid"
                End Try
            End If
        Else
            txtBackup.Text = txtBackup.Text.Trim.Replace("/", "\")
            Dim strBFolder As String = General.PathToAbsolute(txtBackup.Text, strWFolder)
            If strBFolder.Length < 2 Then strError = "Backup folder is invalid: " & strBFolder
            If Not Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(strBFolder)) Then
                If MessageBox.Show("Backup folder does not exist. Create it?", "Backup Folder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                    Try
                        Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(strBFolder)
                    Catch ex As Exception
                        strError = "Backup folder cannot be created: " & strBFolder
                    End Try
                Else
                    strError = "Backup folder is invalid: " & strBFolder
                End If
            End If
        End If
        Dim intValue As Integer
        Dim decValue As Decimal
        Decimal.TryParse(txtSettleDelay.Text, decValue)
        If decValue < 0.01 Or decValue > 300 Then txtSettleDelay.Text = "1"
        If txtRunCopy.Text.Trim.Length < 5 Then chkRunCopyFirst.Checked = True 'absolute minimum: r {1}
        Integer.TryParse(txtVerRateH.Text, intValue)
        If intValue < 1 Then txtVerRateH.Text = "0"
        If intValue > 720 Then txtVerRateH.Text = "720" 'max 1 month
        Integer.TryParse(txtVerRateM.Text, intValue)
        If intValue < 1 Then txtVerRateM.Text = "0"
        If intValue > 60 Then txtVerRateM.Text = "60"
        Integer.TryParse(txtVerRateS.Text, intValue)
        If intValue < 1 Then txtVerRateS.Text = "0"
        If intValue > 60 Then txtVerRateS.Text = "60"
        If Not rbEnsureDaily.Checked Then txtEnsureTime.Text = String.Empty
        txtEnsureTime.Text = txtEnsureTime.Text.Trim
        If txtEnsureTime.Text <> String.Empty Then
            If txtEnsureTime.Text.Contains(":") Then
                Dim tspValue As TimeSpan
                If TimeSpan.TryParse(txtEnsureTime.Text & ":00", tspValue) Then
                    If tspValue.Hours > 23 Then tspValue = New TimeSpan(24, 0, 0)
                    txtEnsureTime.Text = tspValue.Hours.ToString & ":" & tspValue.Minutes.ToString("00")
                Else
                    txtEnsureTime.Text = String.Empty
                End If
            Else
                Integer.TryParse(txtEnsureTime.Text, intValue)
                If intValue > 0 And intValue < 24 Then
                    txtEnsureTime.Text = intValue.ToString & ":00"
                Else
                    txtEnsureTime.Text = String.Empty
                End If
            End If
        End If
        Return strError
    End Function

    Private Sub butOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butOK.Click
        'Check the folders and backup the full watch folder structure to the backup folder
        Dim strError As String = CheckSettings()
        If strError = String.Empty Then
            Dim di As IO.DriveInfo
            Dim strFolder As String = General.PathToAbsolute(txtWatch.Text, Application.StartupPath)
            If strFolder.Length <= 3 Then
                di = New IO.DriveInfo(strFolder.Substring(0, 1))
                If di.DriveType <> IO.DriveType.Removable Then
                    If MsgBox("Watching a root folder is dangerous due to a potentially large amount of file activity. Are you sure?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
                End If
            End If
            If WatcherGUID = Guid.Empty And rbEnsureNever.Checked Then
                'default usb/firewire to ensure when creating
                di = New IO.DriveInfo(txtWatch.Text.Substring(0, 1))
                If di.DriveType = IO.DriveType.Removable Or di.DriveType = IO.DriveType.Network Then rbEnsureDaily.Checked = True
                If rbFolder.Checked Then
                    di = New IO.DriveInfo(txtBackup.Text.Substring(0, 1))
                    If di.DriveType = IO.DriveType.Removable Or di.DriveType = IO.DriveType.Network Then rbEnsureDaily.Checked = True
                End If
            End If
            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()
            If chkBackup.Checked And chkEnabled.Checked Then
                'Create new temp engine to run backup now
                'Dim frmBackup As New BackupFiles
                'frmBackup.Show()
                WatcherEngineTemp = New BackupEngine(Config.Log)
                WatcherEngineTemp.WatchFolder = txtWatch.Text.Trim
                WatcherEngineTemp.BackupFolder = txtBackup.Text.Trim
                WatcherEngineTemp.VersionFiles = rbVersionAll.Checked
                WatcherEngineTemp.VersionPrev = rbVersionPrev.Checked
                WatcherEngineTemp.IncludeFiles = txtIncludeFiles.Text.Trim
                WatcherEngineTemp.ExcludeFiles = txtExcludeFiles.Text.Trim
                WatcherEngineTemp.ExcludeFolders = txtExcludeFolders.Text.Trim
                WatcherEngineTemp.DeleteOnDelete = chkDelete.Checked
                WatcherEngineTemp.EnsureSchedule = "N" 'none
                WatcherEngineTemp.SettleDelay = 1000
                WatcherEngineTemp.RunCopy = txtRunCopy.Text
                WatcherEngineTemp.RunCopyFirst = chkRunCopyFirst.Checked
                WatcherEngineTemp.MaxFileSize = MaxFileSize
                WatcherEngineTemp.SubFolders = chkSubFolders.Checked
                WatcherEngineTemp.DateTimeStamp = txtDateTimeStamp.Text
                WatcherEngineTemp.CompareBeforeCopy = chkCompareBeforeCopy.Checked
                WatcherEngineTemp.ZipMode = "W"
                WatcherEngineTemp.FTPEnable = rbFTP.Checked
                WatcherEngineTemp.FTPHost = txtFTPHost.Text
                WatcherEngineTemp.FTPUser = txtFTPLogin.Text
                WatcherEngineTemp.FTPPass = txtFTPPassword.Text
                WatcherEngineTemp.FTPPassive = chkFTPPassive.Checked
                WatcherEngineTemp.ShowErrors = chkShowErrors.Checked

                'WatcherEngine.SysTrayIcon = New Windows.Forms.NotifyIcon()
                'WatcherEngineTemp.EnsureBackupCurrent() '.BackgroungWorkerRun("ENSURE")
                If bwBackupRestore.IsBusy Then
                    MessageBox.Show("AutoVer is currently doing a Backup/Restore operation. Retry later.", "Backup/Restore", _
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Else
                    ShowBackupFilesWait(0)
                    bwBackupRestore.RunWorkerAsync("")
                End If
                SaveConfig()
            Else
                SaveConfig()
                Me.Cursor = Cursors.Default
                Me.Close()
            End If
        Else
            MsgBox(strError, MsgBoxStyle.Critical)
        End If
    End Sub

    Sub SaveConfig()
        'Update the config and raise event to main form to save to disk and update the engine list
        Dim drWatcher As DataRow = Nothing
        Dim dtWatcherConfig As DataTable = Config.WatcherConfig
        If WatcherGUID = Guid.Empty Then
            drWatcher = dtWatcherConfig.NewRow
            drWatcher("State") = "N"
            drWatcher("GUID") = Guid.NewGuid
        Else
            For intRow As Int16 = 0 To dtWatcherConfig.Rows.Count - 1
                If WatcherGUID = dtWatcherConfig.Rows(intRow)("GUID") Then
                    drWatcher = dtWatcherConfig.Rows(intRow)
                    Exit For
                End If
            Next
            'If IsNothing(drWatcher) Then Exit Sub
            drWatcher("State") = "U"
        End If
        drWatcher("Name") = txtName.Text.Trim
        drWatcher("Enabled") = chkEnabled.Checked
        drWatcher("WatchFolder") = txtWatch.Text.Trim
        drWatcher("BackupFolder") = txtBackup.Text.Trim
        drWatcher("DeleteOnDelete") = chkDelete.Checked
        drWatcher("IncludeFiles") = txtIncludeFiles.Text.Trim
        drWatcher("ExcludeFiles") = txtExcludeFiles.Text.Trim
        drWatcher("ExcludeFolders") = txtExcludeFolders.Text.Trim
        drWatcher("VersionFiles") = rbVersionAll.Checked
        drWatcher("VersionPrevFiles") = rbVersionPrev.Checked
        drWatcher("DateTimeStamp") = txtDateTimeStamp.Text
        drWatcher("MaxVersionAge") = MaxVersionAge
        If radDelete.Checked Then
            drWatcher("MaxVersionAction") = "D"c
        ElseIf radZip.Checked Then
            drWatcher("MaxVersionAction") = "Z"c
        ElseIf rad7Zip.Checked Then
            drWatcher("MaxVersionAction") = "7"c
        Else
            drWatcher("MaxVersionAction") = "N"c
        End If
        Dim decValue As Decimal
        Decimal.TryParse(txtSettleDelay.Text, decValue)
        drWatcher("SettleDelay") = decValue
        drWatcher("RunCopy") = txtRunCopy.Text.Trim
        drWatcher("RunCopyFirst") = chkRunCopyFirst.Checked
        drWatcher("CompareBeforeCopy") = chkCompareBeforeCopy.Checked
        drWatcher("MaxFileSize") = MaxFileSize
        drWatcher("SubFolders") = chkSubFolders.Checked
        drWatcher("EnsureSchedule") = IIf(rbEnsureHourly.Checked, "H", IIf(rbEnsureNever.Checked, "N", "D" & txtEnsureTime.Text))
        drWatcher("ShowErrors") = chkShowErrors.Checked
        drWatcher("ShowEvents") = chkShowEvents.Checked
        If rbVerAll.Checked Then
            drWatcher("VersionRate") = 0
        Else
            drWatcher("VersionRate") = (General.cInteger(txtVerRateH.Text, 0) * 3600) + (General.cInteger(txtVerRateM.Text, 0) * 60) + General.cInteger(txtVerRateS.Text, 0)
        End If
        drWatcher("ZipMode") = IIf(rbZipModeW.Checked, "W"c, IIf(rbZipModeD.Checked, "D"c, "F"c))
        drWatcher("FTPEnable") = rbFTP.Checked
        drWatcher("FTPHost") = txtFTPHost.Text
        drWatcher("FTPUser") = txtFTPLogin.Text
        drWatcher("FTPPass") = General.ToHex(txtFTPPassword.Text)
        drWatcher("FTPPassive") = chkFTPPassive.Checked
        If WatcherGUID = Guid.Empty Then
            dtWatcherConfig.Rows.Add(drWatcher)
        End If
        'Config.UpdateWatcher(drWatcher.ItemArray())
        Config.WatcherConfig = dtWatcherConfig
        Config.SaveWatcherConfig()
        Config.UpdateWatchersList()
        RaiseEvent SettingsUpdated()
        'Me.Cursor = Cursors.Default
        'Me.Close()
    End Sub

    Private Sub bwBackupRestore_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bwBackupRestore.DoWork
        Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)
        Config.Log.Debug(e.Argument, "BackupNowWorker")
        WatcherEngineTemp.EnsureBackupCurrent()
        e.Result = "OK" 'e.Argument '& "-OK"
    End Sub

    Private Sub bwBackupRestore_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bwBackupRestore.RunWorkerCompleted
        Config.Log.Debug(e.Result, "BackupNowWorker")
        If (e.Error IsNot Nothing) Then
            Config.Log.Warn(e.Error.Message, "BackupNowWorker")
            MessageBox.Show(e.Error.Message)
        ElseIf e.Cancelled Then
            Config.Log.Warn("User Cancelled", "BackupNowWorker")
        End If
        If Not IsNothing(frmBackupRestoreRunning) Then frmBackupRestoreRunning.Close()
        Me.Cursor = Cursors.Default
        Me.Close()
    End Sub

    Private Sub ShowBackupFilesWait(ByVal EngIndex As Integer)
        'Display the Backing up files, please wait dialog. Start the updates timer
        frmBackupRestoreRunning = New BackupFilesWait()
        frmBackupRestoreRunning.WaitMessage = "Syncing ..."
        'frmBackupRestoreRunning.EngineIndex = EngIndex
        frmBackupRestoreRunning.FormTitle = "AutoVer Syncing Backup"
        frmBackupRestoreRunning.Show()
        tmrEnsureSyncUpdates.Start()
    End Sub

    Private Sub tmrEnsureSyncUpdates_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles tmrEnsureSyncUpdates.Tick
        'Backup: Polling the backup engine status every 1 sec
        frmBackupRestoreRunning.WaitMessage = WatcherEngineTemp.EnsureMessage
        If WatcherEngineTemp.EnsureRestoreCancelled Then
            bwBackupRestore.CancelAsync()
            tmrEnsureSyncUpdates.Stop()
            If Not IsNothing(frmBackupRestoreRunning) Then frmBackupRestoreRunning.Close()
        End If
    End Sub

    Private Function cNumber(ByVal TextString As String, ByVal ConvertUnits As Boolean) As String
        Dim RegExp As New System.Text.RegularExpressions.Regex("[^0-9]", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        If ConvertUnits Then
            TextString = TextString.ToUpper
            TextString = TextString.Replace("K", "000")
            TextString = TextString.Replace("M", "000000")
            TextString = TextString.Replace("G", "000000000")
        End If
        Return RegExp.Replace(TextString, String.Empty)
    End Function

    Private Sub butCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butCancel.Click
        RaiseEvent SettingsUpdated()
        Me.Close()
    End Sub

    Private Sub btnWatchFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWatchFolder.Click
        If txtWatch.Text.Trim <> String.Empty Then FolderBrowserDialog1.SelectedPath = txtWatch.Text
        If FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtWatch.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub butBackupFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butBackupFolder.Click
        If txtBackup.Text.Trim <> String.Empty Then FolderBrowserDialog1.SelectedPath = txtBackup.Text
        If FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtBackup.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        'System.Diagnostics.Process.Start(Application.StartupPath & "\AutoVerHelp.htm", "#Properties")
        Dim strHelpID As String
        Select Case tabControl.SelectedTab.Name
            Case "Gen"
                strHelpID = "11"
            Case "Adv"
                strHelpID = "12"
            Case "Ver"
                strHelpID = "13"
            Case "FTP"
                strHelpID = "14"
            Case Else
                strHelpID = "3"
        End Select
        Help.ShowHelp(Me, "AutoVer.chm", HelpNavigator.TopicId, strHelpID)
    End Sub

    Private Sub rbFTP_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbFTP.CheckedChanged
        If rbFTP.Checked Then
            butBackupFolder.Visible = False
        Else
            butBackupFolder.Visible = True
        End If
    End Sub

    Private Sub rbFolder_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbFolder.CheckedChanged
        If rbFolder.Checked Then
            butBackupFolder.Visible = True
        Else
            butBackupFolder.Visible = False
        End If
    End Sub
End Class
