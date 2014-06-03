Imports System.IO
Imports System.Threading
Imports AutoVer.Utilities.FTP
Imports System.Reflection
Imports System.Media
Imports SevenZip
Imports Alphaleonis.Win32
Imports Ionic.Zip
Imports System.Runtime.Serialization

<DataContractAttribute()> _
Public Class BackupEngine
    Inherits MarshalByRefObject
    Private FileWatcher As FileSystemWatcher
    Private FileFilter As New FileFolderFilter()
    Private CopyQueue As New Queue(Of ChangeDetails)
    Private RetryQueue As New Queue(Of ChangeDetails)
    Private FileHelper As New FileUtils()
    Public BackupFolderFail, WatchFailure As Boolean
    'WatchFailure is an unrecoverable (unbuffered) failure which needs "Ensuring"
    Private WriteDelay As Integer = 1000
    Private LockObject As New Object
    Private Const MaxRetries As Int16 = 120
    '2 mins
    Private WithEvents tmrDelayWrite As Timer
    Private tmrDelayWriteInterval As Long
    'ms. 0 = disabled
    'Private WithEvents frmBackupRestoreRunning As BackupFilesWait
    Private OperationCount As Integer
    'Files deleted/archived/copied for maint functions
    Private Const ZipFileName As String = "BackupVersions.zip"
    Private ftp As FTPclient
    'Private WithEvents bwBackupRestore As New BackgroundWorker
    Private AppStartPath As String
    Public GUID As Guid
    Public Log As Logger
    Public Name, WatchFolder, BackupFolder, IncludeFiles, ExcludeFiles, ExcludeFolders, DateTimeStamp As String
    Public Enabled As Boolean = True
    Public Paused As Boolean = False
    Public DeleteOnDelete As Boolean
    Public VersionFiles As Boolean = True
    Public VersionPrev As Boolean
    Public SubFolders As Boolean = True
    Public EnsureOnStart As Boolean
    Public EnsureRestoreCancelled As Boolean
    Public MaxVersionAge As UInt32
    Public MaxFileSize As UInt64
    Public MaxVersionAction As Char
    'D,Z,N,7
    Public FTPEnable As Boolean = False
    Public FTPPassive As Boolean = True
    Public FTPHost, FTPUser, FTPPass As String
    Public Started As Boolean = False
    Public ShowErrors, ShowEvents As Boolean
    Public EnsureSchedule As String
    'N,D,Dxx:xx,H
    Public EnsureTimer As Timer
    Public VersionRate As Integer
    Public SettleDelay As Integer = 1000
    Public RunCopy, RunCopyArgs As String
    Public RunCopyFirst As Boolean = True
    Public CompareBeforeCopy As Boolean = False
    ' Public SysTrayIcon As System.Windows.Forms.NotifyIcon
    Public CountChanged, CountRenamed, CountDeleted As Integer
    Public UseRecycleBin As Boolean
    Public Message As String = String.Empty
    Public EnsureMessage As String = String.Empty
    Public Event UserMessage(ByVal MessageText As String)

#Region " Properties "

    'Convert value types to references so Remoting can address them

    ReadOnly Property Stats() As String
        Get
            Return _
                "Changed:" & CountChanged.ToString & ", Renamed:" & CountRenamed.ToString & ", Deleted:" & _
                CountDeleted.ToString
        End Get
    End Property

#End Region

#Region " Setup "

    Public Sub New(ByVal LoggerInstance As Logger)
        Log = LoggerInstance
    End Sub

    Public Sub SetupWatcher()
        Dim strError As String = String.Empty
        AppStartPath = New FileInfo(Assembly.GetExecutingAssembly.Location).Directory.FullName
        If Not AppStartPath.EndsWith("\") Then AppStartPath &= "\"
        'WatchFolder = WatchFolder.ToLower
        WatchFolder = General.PathToAbsolute(WatchFolder, AppStartPath)
        If WatchFolder.ToLower = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 3).ToLower _
            Then strError = "Watching all of a drive where Windows is installed is not allowed"
        If _
            Not WatchFolder.StartsWith("\\") AndAlso WatchFolder.Length > 1 AndAlso _
            New DriveInfo(WatchFolder.Substring(0, 2)).DriveType = DriveType.Fixed Then
            'Ignore USB/Network offline issues - we'll buffer for a while in case it comes online
            If WatchFolder = String.Empty OrElse Not Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(WatchFolder)) Then _
                strError = "Watch folder is invalid: " & WatchFolder
        End If
        If FTPEnable Then
            If BackupFolder = String.Empty Or Not BackupFolder.EndsWith("/") Then BackupFolder &= "/"
            ftp = New FTPclient(FTPHost, FTPUser, FTPPass)
            ftp.UsePassive = FTPPassive
        Else
            'BackupFolder = BackupFolder.ToLower
            BackupFolder = General.PathToAbsolute(BackupFolder, WatchFolder)
            If _
                Not BackupFolder.StartsWith("\\") AndAlso BackupFolder.Length > 1 AndAlso _
                New DriveInfo(BackupFolder.Substring(0, 2)).DriveType = DriveType.Fixed Then
                'Ignore USB/Network offline issues - we'll buffer for a while in case it comes online
                If BackupFolder = String.Empty OrElse Not Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(BackupFolder)) Then _
                    strError = "Backup folder is invalid: " & BackupFolder
            End If
        End If
        RunCopy = RunCopy.Trim
        If RunCopy <> String.Empty Then
            If RunCopy.Contains(".exe ") Then
                RunCopyArgs = RunCopy.Substring(RunCopy.IndexOf(".exe ", StringComparison.CurrentCultureIgnoreCase) + 5)
                RunCopy = RunCopy.Substring(0, RunCopy.IndexOf(".exe ", StringComparison.CurrentCultureIgnoreCase) + 4)
            Else
                RunCopyArgs = "{0}"
            End If
        Else
            RunCopyArgs = String.Empty
        End If
        If strError = String.Empty Then
            FileFilter.SetupFilters(IncludeFiles, ExcludeFiles, ExcludeFolders)
            StartWatching()
        Else
            Log.Warn(strError, "EngineStart")
            'MsgBox(strError, MsgBoxStyle.Critical)
        End If
    End Sub

    Private Sub StartWatching()
        'Start watching
        If Not Enabled Then Exit Sub
        Paused = False
        If Not Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(WatchFolder)) Then
            Log.Info(WatchFolder, "Folder not available")
            Exit Sub
        End If
        tmrDelayWrite = New Timer(AddressOf tmrDelayWrite_Tick, Nothing, Timeout.Infinite, Timeout.Infinite)
        'Must use shared services timer running as a service. System.Timers.Timer()
        Try
            FileWatcher = New FileSystemWatcher
            FileWatcher.IncludeSubdirectories = SubFolders
            FileWatcher.NotifyFilter = NotifyFilters.DirectoryName Or NotifyFilters.FileName Or NotifyFilters.LastWrite Or _
                                       NotifyFilters.Size
            FileWatcher.Filter = IIf(IncludeFiles.Contains(";"), "*.*", IncludeFiles)
            FileWatcher.Path = WatchFolder
            If SettleDelay < 10 Then SettleDelay = 10
            WriteDelay = SettleDelay

            AddHandler FileWatcher.Changed, AddressOf FileChanged
            AddHandler FileWatcher.Created, AddressOf FileChanged
            AddHandler FileWatcher.Deleted, AddressOf FileChanged
            AddHandler FileWatcher.Renamed, AddressOf FileRenamed
            FileWatcher.EnableRaisingEvents = True
            Started = True
        Catch ex As Exception
            Log.Error(ex.Message, "FileSystemWatcher.Start")
        End Try

        'Try
        '    If Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "RecycleBin", 0) = 1 Then UseRecycleBin = True
        'Catch
        'End Try
        'Try
        '    If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("RecycleBin")) Then UseRecycleBin = System.Configuration.ConfigurationManager.AppSettings("RecycleBin")
        'Catch
        'End Try

        EnsureTimer = New Timer(AddressOf EnsureTimerElapsed, Nothing, Timeout.Infinite, Timeout.Infinite)
        'System.Timers.Timer()
        If EnsureSchedule.StartsWith("D") Then
            If EnsureSchedule = "D" Then
                EnsureTimer.Change(86400000, 0)
                '24hours
            Else
                Dim tsp As TimeSpan
                If TimeSpan.TryParse(EnsureSchedule.Substring(1) & ":00", tsp) Then
                    If tsp.CompareTo(New TimeSpan(Now.Hour, Now.Minute, Now.Second)) > 0 Then
                        EnsureTimer.Change( _
                            Convert.ToInt32( _
                                tsp.Subtract(New TimeSpan(Now.Hour, Now.Minute, Now.Second)).TotalMilliseconds), 0)
                    Else
                        EnsureTimer.Change( _
                            Convert.ToInt32( _
                                New TimeSpan(23, 59, 59).Subtract(tsp).Add(New TimeSpan(Now.Hour, Now.Minute, Now.Second)) _
                                               .TotalMilliseconds), 0)
                    End If
                    'Log.Debug(EnsureTimer.Interval.ToString, "msec to next Ensure")
                Else
                    Log.Warn(EnsureSchedule.Substring(1), "Unable to read Ensure time")
                    EnsureTimer.Change(86400000, 0)
                    '24hours
                End If
            End If
            'EnsureTimer.AutoReset = True
            'AddHandler EnsureTimer.Elapsed, AddressOf EnsureTimerElapsed
            'EnsureTimer.Enabled = True
        ElseIf EnsureSchedule = "H" Then
            EnsureTimer.Change(3600000, 0)
            '1hour
            'EnsureTimer.AutoReset = True
            'AddHandler EnsureTimer.Elapsed, AddressOf EnsureTimerElapsed
            'EnsureTimer.Enabled = True
        End If
    End Sub

    Public Sub EnsureTimerElapsed(ByVal state As Object) _
        'ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
        Log.Debug("", "EnsureTimerElapsed")
        If EnsureSchedule.StartsWith("D") And EnsureSchedule.Length > 1 Then
            EnsureTimer.Change(86400000, 0)
            '24hours
        End If
        EnsureBackupCurrent()
    End Sub

    Public Sub StopWatching()
        'Shutdown watcher
        If Started Then
            FileWatcher.EnableRaisingEvents = False
            EnsureTimer.Change(Timeout.Infinite, Timeout.Infinite)
            EnsureTimer.Dispose()
            'If EnsureTimer.Enabled Then EnsureTimer.Enabled = False
            RemoveHandler FileWatcher.Changed, AddressOf FileChanged
            RemoveHandler FileWatcher.Created, AddressOf FileChanged
            RemoveHandler FileWatcher.Deleted, AddressOf FileChanged
            RemoveHandler FileWatcher.Renamed, AddressOf FileRenamed
            FileWatcher.Dispose()
            tmrDelayWriteInterval = 0
            tmrDelayWrite.Change(Timeout.Infinite, Timeout.Infinite)
            tmrDelayWrite.Dispose()
            'RemoveHandler EnsureTimer.Elapsed, AddressOf EnsureTimerElapsed
            Started = False
        End If
    End Sub

    Private Sub FileChanged(ByVal source As Object, ByVal e As FileSystemEventArgs)
        'File has changed
        'Log.Debug("event", "FileChanged")
        AddToQueue(e.ChangeType, e.FullPath, String.Empty)
    End Sub

    Private Sub FileRenamed(ByVal source As Object, ByVal e As RenamedEventArgs)
        'File has been renamed
        'Log.Debug("event", "FileRenamed")
        AddToQueue(e.ChangeType, e.OldFullPath, e.FullPath)
    End Sub

    Private Sub AddToQueue(ByRef ChangeType As WatcherChangeTypes, ByRef FirstPath As String, ByRef SecondPath As String)
        Try
            'Log.Debug("checkingPaths", "AddToQueue")
            If _
                FirstPath.StartsWith(BackupFolder) Or _
                (SecondPath <> String.Empty AndAlso SecondPath.StartsWith(BackupFolder)) Then Exit Sub
            If ChangeType = WatcherChangeTypes.Renamed Then
                Log.Debug(FirstPath & " to " & SecondPath, ChangeType.ToString())
            Else
                Log.Debug(FirstPath, ChangeType.ToString())
            End If
            'Wait for 1 sec past last update to do the write - to avoid multiple events to the one file
            SyncLock (LockObject)
                Dim chgDetailNew As New ChangeDetails(ChangeType, FirstPath, SecondPath)
                Dim blnFound As Boolean = False
                Dim newChangeNormalised As WatcherChangeTypes
                'Add a file once only for each change type. Treat created and changed the same (same resulting file operation)
                For Each chgDetail As ChangeDetails In CopyQueue
                    newChangeNormalised = IIf(chgDetailNew.ChangeType = WatcherChangeTypes.Created, _
                                              WatcherChangeTypes.Changed, chgDetailNew.ChangeType)
                    If _
                        chgDetail.FirstPath = chgDetailNew.FirstPath And _
                        IIf(chgDetail.ChangeType = WatcherChangeTypes.Created, WatcherChangeTypes.Changed, _
                            chgDetail.ChangeType) = newChangeNormalised Then
                        blnFound = True
                    End If
                Next
                If Not blnFound Then CopyQueue.Enqueue(chgDetailNew)
                If tmrDelayWriteInterval = 0 Then
                    'Init delayed write timer
                    'tmrDelayWrite.Interval = WriteDelay
                    'tmrDelayWrite.Start()
                    tmrDelayWriteInterval = WriteDelay
                    tmrDelayWrite.Change(WriteDelay, 0)
                ElseIf tmrDelayWriteInterval < 60000 Then
                    'reset if not already copying
                    tmrDelayWriteInterval = WriteDelay
                    tmrDelayWrite.Change(WriteDelay, 0)
                    'tmrDelayWrite.Stop()
                    'tmrDelayWrite.Start()
                End If
            End SyncLock
        Catch ex As Exception
            Log.Debug(ex.ToString, "AddToQueue")
        End Try
    End Sub

    Protected Sub tmrDelayWrite_Tick(ByVal state As Object) _
        'ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles tmrDelayWrite.Elapsed
        'Process copy queue. Release lock every file in case new file is waiting to be added
        Try
            If _
                (Not FTPEnable AndAlso Not Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(BackupFolder))) OrElse _
                (FTPEnable AndAlso Not ftp.FtpDirectoryExists(BackupFolder)) Then
                If Not BackupFolderFail Then Log.Warn("Backup folder is inaccessible!")
                BackupFolderFail = True
                WatchFailure = True
                'Throttle down copy tries to once every 15 sec (or 15 x the settling time)
                WriteDelay = WriteDelay * 15
                tmrDelayWriteInterval = WriteDelay
                tmrDelayWrite.Change(WriteDelay, 0)
                'tmrDelayWrite.Interval = WriteDelay
                'tmrDelayWrite.Start()
                'Backup drive must be offline, we've buffered all we can. Start dumping the oldest items as needed
                If CopyQueue.Count > 1000 Then CopyQueue.Dequeue()
                '.Clear()
                Exit Sub
            ElseIf BackupFolderFail Then
                Log.Info("Backup folder back online!", "BackupEngine")
                BackupFolderFail = False
                WatchFailure = False
                WriteDelay = SettleDelay
            End If
            tmrDelayWriteInterval = 60000
            tmrDelayWrite.Change(60000, 0)
            'tmrDelayWrite.Interval = 60000
            Log.Debug(CopyQueue.Count, "DelayWrite")
            Do While CopyQueue.Count > 0
                SyncLock (LockObject)
                    Dim changeDetail As ChangeDetails = CopyQueue.Dequeue
                    'Log.Debug(ChangeDetail.ChangeType.ToString, "ChangeType")
                    If changeDetail.ChangeType = WatcherChangeTypes.Renamed Then
                        RenameFile(changeDetail)
                    ElseIf changeDetail.ChangeType = WatcherChangeTypes.Deleted Then
                        DeleteFile(changeDetail)
                    Else
                        BackupFile(changeDetail)
                    End If
                End SyncLock
            Loop
            SyncLock (LockObject)
                'Retry failed operations for MaxRetries times
                tmrDelayWriteInterval = WriteDelay
                tmrDelayWrite.Change(WriteDelay, 0)
                'tmrDelayWrite.Interval = WriteDelay
                Do While RetryQueue.Count > 0
                    CopyQueue.Enqueue(RetryQueue.Dequeue)
                    tmrDelayWriteInterval = WriteDelay * 15
                    tmrDelayWrite.Change(tmrDelayWriteInterval, 0)
                    'tmrDelayWrite.Interval = WriteDelay * 15 'throtle down
                Loop
                'Restart if there are late entries
                'tmrDelayWrite.Stop()
                If CopyQueue.Count > 0 Then
                    tmrDelayWriteInterval = WriteDelay
                    tmrDelayWrite.Change(WriteDelay, 0)
                Else
                    tmrDelayWriteInterval = 0
                    tmrDelayWrite.Change(Timeout.Infinite, Timeout.Infinite)
                End If
            End SyncLock
        Catch ex As Exception
            Log.Debug(ex.ToString, "tmrDelayWrite_Tick")
        End Try
    End Sub

    Public Function FTPBackupFolderExists() As Boolean
        'Check if backup path exists. Can be called by config engine/watch settings
        Return ftp.FtpDirectoryExists(BackupFolder)
    End Function

    Private Function FTPUpload(ByRef source As String, ByRef target As String) As Boolean
        'If the directory does not exist, then check each folder and create if required, then upload the file.
        If target.Contains("/") Then
            Dim FullPath As String = target.Substring(0, target.LastIndexOf("/", StringComparison.Ordinal))
            'Log.Debug(FullPath, "FTPChk")
            If Not ftp.FtpDirectoryExists(FullPath) Then
                Dim intDir As Integer
                Dim PartPath As String
                Do Until intDir = FullPath.Length
                    intDir = FullPath.IndexOf("/", intDir + 1, StringComparison.Ordinal)
                    If intDir < 0 Then intDir = FullPath.Length
                    PartPath = FullPath.Substring(0, intDir)
                    'Log.Debug(PartPath, "FTPChk2")
                    If Not ftp.FtpDirectoryExists(PartPath) Then
                        'Log.Debug(PartPath, "FTPCreate")
                        ftp.FtpCreateDirectory(PartPath)
                    End If
                Loop
            End If
        End If
        Return ftp.Upload(source, target)
    End Function

#End Region

#Region " File Actions "

    Private Sub RenameFile(ByVal ChangeDetail As ChangeDetails)
        'File/Folder renamed (or moved)
        'Log.Debug("Starting", "RenameFile")
        'Log.Debug("Start " & ChangeDetail.FirstPath, "RenameFile")
        If ChangeDetail.FirstPath = String.Empty Then Exit Sub
        'Entry we invalidated earlier due to temp file swap
        For Each ChgDetailEnum As ChangeDetails In CopyQueue
            'Log.Debug(ChgDetailEnum.ChangeType.ToString & ": " & ChgDetailEnum.FirstPath)
            If _
                ChgDetailEnum.ChangeType = WatcherChangeTypes.Created And _
                ChgDetailEnum.FirstPath = ChangeDetail.FirstPath Then
                'This file has been renamed and new file created of the same name replacing it
                For Each ChgDetailCheck As ChangeDetails In CopyQueue
                    'Kill anything with our temp file in it. Then abort this item (creation item will invoke a backup of it soon).
                    If ChgDetailCheck.FirstPath = ChangeDetail.SecondPath Then
                        'We can't remove from the queue so invalidate the entry. 
                        ChgDetailCheck.ChangeType = WatcherChangeTypes.Renamed
                        ChgDetailCheck.FirstPath = String.Empty
                    End If
                Next
                Log.Debug(ChangeDetail.FirstPath, "Changed:TempCreatedOrig")
                Exit Sub
            ElseIf _
                ChgDetailEnum.ChangeType = WatcherChangeTypes.Renamed And _
                ChgDetailEnum.SecondPath = ChangeDetail.FirstPath Then
                'This file has been renamed and a another file renamed to the same name to replace it
                For Each ChgDetailCheck As ChangeDetails In CopyQueue
                    'Kill anything with the other temp file in it.
                    If ChgDetailCheck.FirstPath = ChgDetailEnum.FirstPath Then
                        'We can't remove from the queue so invalidate the entry. 
                        ChgDetailCheck.ChangeType = WatcherChangeTypes.Renamed
                        ChgDetailCheck.FirstPath = String.Empty
                    End If
                Next
                Log.Debug(ChangeDetail.FirstPath, "Changed:TempSwapOrig")
                'Do a backup of our file
                BackupFile(New ChangeDetails(WatcherChangeTypes.Changed, ChangeDetail.FirstPath, String.Empty))
                Exit Sub
            End If
        Next

        Dim strOldFile, strNewFile, strNewExt, strOldExt, strTimeStamp, RelativeFileName As String
        'Find relative folder to backup folder root
        RelativeFileName = ChangeDetail.FirstPath.Substring(WatchFolder.Length)
        If FTPEnable Then RelativeFileName = RelativeFileName.Replace("\", "/")
        If Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(ChangeDetail.SecondPath)) Then
            'Is folder
            'Log.Debug("IsFolder", "RenameFile")
            Try
                If FTPEnable Then
                    ftp.FtpRename(String.Concat(BackupFolder, RelativeFileName), _
                                  String.Concat(BackupFolder, _
                                                ChangeDetail.SecondPath.Substring(WatchFolder.Length).Replace("\", "/")))
                Else
                    Filesystem.Directory.Move(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFileName)), _
                                              Filesystem.Path.GetLongPath(String.Concat(BackupFolder, ChangeDetail.SecondPath.Substring(WatchFolder.Length))))
                End If
                If ShowEvents Then _
                    RaiseEvent UserMessage("Folder renamed: " & String.Concat(BackupFolder, RelativeFileName))
            Catch
                Log.Warn(RelativeFileName, "Error renaming/moving folder:")
                ChangeDetail.Retries += 1
                If ChangeDetail.Retries <= MaxRetries Then
                    RetryQueue.Enqueue(ChangeDetail)
                Else
                    WatchFailure = True
                End If
                If ShowErrors And ChangeDetail.Retries = 1 Then
                    Message = "Error renaming/moving folder (will retry over next " & (MaxRetries * 10 / 60).ToString("#") & _
                              " mins):" & vbCrLf & RelativeFileName
                    RaiseEvent UserMessage(Message)
                    SystemSounds.Hand.Play()
                End If
            End Try
            Exit Sub
        End If
        If Not Filesystem.File.Exists(Filesystem.Path.GetLongPath(ChangeDetail.SecondPath)) Then
            'File no longer exists (temp files don't last long)
            Log.Debug(ChangeDetail.SecondPath, "File no longer exists:")
            Exit Sub
        End If

        strOldFile = New Filesystem.FileInfo(Filesystem.Path.GetLongPath(ChangeDetail.FirstPath)).Name
        strNewFile = New Filesystem.FileInfo(Filesystem.Path.GetLongPath(ChangeDetail.SecondPath)).Name
        'If (Not strOldFile.Contains(".") Or strOldFile.ToLower.EndsWith(".tmp")) And Not strNewFile.ToLower.EndsWith(".tmp") And strNewFile.Contains(".") Then
        '    'New file created and renamed to replace old file
        '    Log.Debug(strOldFile, "Temp file swap:")
        '    BackupFile(New ChangeDetails(WatcherChangeTypes.Changed, ChangeDetail.SecondPath, String.Empty))
        '    Exit Sub
        'End If

        'Log.Debug("Ready", "RenameFile")
        Dim RelativeFolder As String
        If FTPEnable Then
            If RelativeFileName.Contains("/") Then
                RelativeFolder = RelativeFileName.Substring(0, RelativeFileName.LastIndexOf("/", StringComparison.Ordinal) + 1)
            Else
                RelativeFolder = String.Empty
            End If
        Else
            If RelativeFileName.Contains("\") Then
                RelativeFolder = RelativeFileName.Substring(0, RelativeFileName.LastIndexOf("\", StringComparison.Ordinal) + 1)
            Else
                RelativeFolder = String.Empty
                '"\"
            End If
        End If
        If strNewFile.Contains(".") Then
            strNewExt = strNewFile.Substring(strNewFile.LastIndexOf(".", StringComparison.Ordinal))
        Else
            strNewExt = String.Empty
        End If
        If strOldFile.Contains(".") Then
            strOldExt = strOldFile.Substring(strOldFile.LastIndexOf(".", StringComparison.Ordinal))
        Else
            strOldExt = String.Empty
        End If

        If FileFilter.CanCopy(strNewFile, ChangeDetail.SecondPath.Substring(WatchFolder.Length)) Then _
            'FileFilter.CanCopy(strOldFile, RelativeFolder) Or
            'Get old backup filenames
            'Log.Debug(BackupFolder & RelativeFolder & " " & strOldFile & ".*" & strOldExt, "ren getfiles")
            'Log.Debug("FilterPass", "RenameFile")
            Dim aryFiles() As String
            If VersionFiles Then
                If FTPEnable Then
                    RelativeFolder = RelativeFolder.Replace("\", "/")
                    Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(String.Concat(BackupFolder, RelativeFolder), _
                                                                          False)
                    aryFiles = New String() {}
                    For Each ftpFile As FTPfileInfo In ftpList
                        If ftpFile.Filename.StartsWith(strOldFile) Then
                            If IsNothing(aryFiles) Then
                                ReDim aryFiles(0)
                            Else
                                ReDim Preserve aryFiles(aryFiles.Length)
                            End If
                            aryFiles(aryFiles.Length - 1) = String.Concat(BackupFolder, RelativeFolder, ftpFile.Filename)
                        End If
                    Next
                    'RelativeFolder.Replace("\", "/")
                Else
                    aryFiles = Filesystem.Directory.GetFiles(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)), String.Concat(strOldFile, ".*", strOldExt))
                    RelativeFolder = ChangeDetail.SecondPath.Substring(WatchFolder.Length)
                    If RelativeFolder.Contains("\") Then
                        RelativeFolder = RelativeFolder.Substring(0, RelativeFolder.LastIndexOf("\", StringComparison.Ordinal) + 1)
                    Else
                        RelativeFolder = String.Empty
                        '"\"
                    End If
                End If
                If aryFiles.Length = 0 Then
                    'May have been an uncaught temp file swap. Treat as changed
                    Log.Warn(ChangeDetail.SecondPath, "Rename source backup not found - possible temp file swap")
                    BackupFile(New ChangeDetails(WatcherChangeTypes.Changed, ChangeDetail.SecondPath, String.Empty))
                End If

                'Log.Debug("Start rename " & aryFiles.Length, "RenameFile")
                For Each strOldFileFolder As String In aryFiles
                    strOldFile = strOldFileFolder.Substring(strOldFileFolder.LastIndexOf(IIf(FTPEnable, "/", "\")) + 1)
                    If strOldFile.Contains(".") Then
                        strTimeStamp = strOldFile.Substring(0, strOldFile.LastIndexOf(".", StringComparison.Ordinal))
                        If strTimeStamp.Contains(".") Then
                            strTimeStamp = strTimeStamp.Substring(strTimeStamp.LastIndexOf(".", StringComparison.Ordinal))
                        Else
                            'no ext - only timestamp 
                            strTimeStamp = strOldFile.Substring(strOldFile.LastIndexOf(".", StringComparison.Ordinal))
                        End If
                    Else
                        strTimeStamp = String.Empty
                        strNewExt = String.Empty
                    End If
                    'Rename
                    'Log.Debug(strOldFileFolder & " to " & BackupFolder & RelativeFolder & strNewFile & strTimeStamp & strNewExt, "rename")
                    Try
                        If FTPEnable Then
                            If _
                                Not _
                                ftp.FtpRename(String.Concat(BackupFolder, RelativeFolder, strOldFile), _
                                              String.Concat(BackupFolder, RelativeFolder, strNewFile, strTimeStamp, _
                                                            strNewExt)) Then _
                                Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                        Else
                            Filesystem.File.Move(Filesystem.Path.GetLongPath(strOldFileFolder), _
                                                 Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder, strNewFile, strTimeStamp, strNewExt)))
                        End If
                        If ShowEvents Then RaiseEvent UserMessage("File renamed: " & strOldFileFolder)
                        CountRenamed += 1
                    Catch
                        Log.Warn(strOldFile, "Error renaming/moving file:")
                        ChangeDetail.Retries += 1
                        If ChangeDetail.Retries <= MaxRetries Then RetryQueue.Enqueue(ChangeDetail)
                        If ShowErrors And ChangeDetail.Retries = 1 Then
                            Message = "Error renaming/moving file (will retry over next " & _
                                      (MaxRetries * 10 / 60).ToString("#") & " mins):" & vbCrLf & strOldFile
                            RaiseEvent UserMessage(Message)
                            SystemSounds.Hand.Play()
                        End If
                    End Try
                Next
            Else
                'Not VersionFiles
                If FTPEnable Then
                    RelativeFolder = RelativeFolder.Replace("\", "/")
                    Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(String.Concat(BackupFolder, RelativeFolder), _
                                                                          False)
                    aryFiles = New String() {}
                    For Each ftpFile As FTPfileInfo In ftpList
                        If ftpFile.Filename = strOldFile Then
                            ReDim aryFiles(0)
                            aryFiles(0) = strOldFile
                        End If
                    Next
                Else
                    aryFiles = Filesystem.Directory.GetFiles(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)), strOldFile)
                    RelativeFolder = ChangeDetail.SecondPath.Substring(WatchFolder.Length)
                    If RelativeFolder.Contains("\") Then
                        RelativeFolder = RelativeFolder.Substring(0, RelativeFolder.LastIndexOf("\", StringComparison.Ordinal) + 1)
                    Else
                        RelativeFolder = String.Empty
                        '"\"
                    End If
                End If
                If aryFiles.Length = 0 Then
                    'May have been an uncaught temp file swap. Treat as changed
                    Log.Warn(ChangeDetail.SecondPath, "Rename source backup not found - possible temp file swap")
                    BackupFile(New ChangeDetails(WatcherChangeTypes.Changed, ChangeDetail.SecondPath, String.Empty))
                End If
                For Each strOldFileFolder As String In aryFiles
                    'Rename
                    Try
                        If FTPEnable Then
                            If ftp.FtpFileExists(String.Concat(BackupFolder, RelativeFolder, strNewFile)) Then
                                ftp.FtpDelete(String.Concat(BackupFolder, RelativeFolder, strNewFile))
                            End If
                            If _
                                ftp.FtpRename(String.Concat(BackupFolder, RelativeFolder, strOldFile), _
                                              String.Concat(BackupFolder, RelativeFolder, strNewFile)) Then
                                If ShowEvents Then RaiseEvent UserMessage("File renamed: " & strOldFileFolder)
                            Else
                                Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                            End If
                        Else
                            If Filesystem.File.Exists(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder, strNewFile))) Then
                                Filesystem.File.Delete(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder, strNewFile)))
                            End If
                            Filesystem.File.Move(Filesystem.Path.GetLongPath(strOldFileFolder), _
                                                 Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder, strNewFile)))
                            If ShowEvents Then RaiseEvent UserMessage("File renamed: " & strOldFileFolder)
                        End If
                        CountRenamed += 1
                    Catch
                        Log.Warn(strOldFile, "Error renaming/moving file:")
                        ChangeDetail.Retries += 1
                        If ChangeDetail.Retries <= MaxRetries Then
                            RetryQueue.Enqueue(ChangeDetail)
                        Else
                            WatchFailure = True
                        End If
                        If ShowErrors And ChangeDetail.Retries = 1 Then
                            Message = "Error renaming/moving file (will retry over next " & _
                                      (MaxRetries * 10 / 60).ToString("#") & " mins):" & vbCrLf & strOldFile
                            RaiseEvent UserMessage(Message)
                            SystemSounds.Hand.Play()
                        End If
                    End Try
                Next
            End If
        Else
            Log.Debug(strOldFile & " or " & strNewFile, "File excluded:")
        End If
    End Sub

    Private Sub BackupFile(ByVal ChangeDetail As ChangeDetails)
        'Check if file creation is a temp file renamed to the final file
        'Log.Debug(ChangeDetail.FirstPath, "RenameCheck")
        'Dim blnAlreadyQueued As Boolean = False
        'For Each ChgDetailEnum As ChangeDetails In CopyQueue
        '    'Log.Debug(ChgDetailEnum.ChangeType.ToString & ": " & ChgDetailEnum.FirstPath)
        '    If ChgDetailEnum.ChangeType = WatcherChangeTypes.Renamed And ChgDetailEnum.FirstPath = ChangeDetail.FirstPath Then
        '        'This file has been renamed
        '        For Each ChgDetailCheck As ChangeDetails In CopyQueue
        '            'Already in copy queue?
        '            If (ChgDetailCheck.ChangeType = WatcherChangeTypes.Changed Or ChgDetailCheck.ChangeType = WatcherChangeTypes.Created) And ChgDetailCheck.FirstPath = ChgDetailEnum.SecondPath Then
        '                blnAlreadyQueued = True
        '            End If
        '        Next
        '        If Not blnAlreadyQueued Then
        '            ChgDetailEnum.ChangeType = WatcherChangeTypes.Changed
        '            ChgDetailEnum.FirstPath = ChgDetailEnum.SecondPath
        '            Log.Debug(ChgDetailEnum.FirstPath, "TempCreated")
        '        End If
        '        Exit Sub
        '    End If
        'Next
        'Log.Debug("Start", "CopyBackupFile")
        Dim RelativeFileName As String
        RelativeFileName = ChangeDetail.FirstPath.Substring(WatchFolder.Length)
        Dim RelativeFolder As String
        If RelativeFileName.Contains("\") Then 'ftp doesn't use RelativeFolder, so \ not / does not matter
            RelativeFolder = RelativeFileName.Substring(0, RelativeFileName.LastIndexOf("\", StringComparison.Ordinal))
        Else
            RelativeFolder = String.Empty
        End If

        Dim FirstFile As New Filesystem.FileInfo(Filesystem.Path.GetLongPath(ChangeDetail.FirstPath))
        Dim filChanged As Filesystem.FileInfo
        Dim strFullName As String
        Try
            If FirstFile.Exists Then
                'Check file size limits
                If FirstFile.Length > MaxFileSize Then
                    Log.Debug(ChangeDetail.FirstPath, "File excluded: MaxFileSize: ")
                    Exit Sub
                End If
            ElseIf Not FTPEnable Then
                'May have already been deleted or directory
                If _
                    FileFilter.CanCopy(ChangeDetail.FirstPath, RelativeFolder) And Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(ChangeDetail.FirstPath)) Then
                    'Is directory only - if not on backup, create it
                    If Not Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFileName))) Then _
                        Filesystem.Directory.CreateDirectory(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFileName)))
                Else
                    Log.Debug(FirstFile.FullName, "Does not exist")
                End If
                Exit Sub
            End If

            If FTPEnable Then
                filChanged = FirstFile
                RelativeFolder = RelativeFolder.Replace("\", "/")
                'If Not ftp.FtpDirectoryExists(String.Concat(BackupFolder, RelativeFolder)) Then
                '    If Not ftp.FtpCreateDirectory(String.Concat(BackupFolder, RelativeFolder)) Then
                '        Log.Debug(ftp.ErrorText)
                '    End If
                'End If
                strFullName = String.Concat(BackupFolder, RelativeFileName.Replace("\", "/"))
            Else
                filChanged = New Filesystem.FileInfo(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFileName)))
                strFullName = filChanged.FullName
            End If
        Catch ex As Exception
            Log.Warn(ChangeDetail.FirstPath & " " & ex.Message, "Error creating path:")
            ChangeDetail.Retries += 1
            If ChangeDetail.Retries <= MaxRetries Then
                RetryQueue.Enqueue(ChangeDetail)
            Else
                WatchFailure = True
            End If
            If ShowErrors And ChangeDetail.Retries = 1 Then
                Message = "Error creating path (will retry over next " & (MaxRetries * 10 / 60).ToString("#") & " mins):" & vbCrLf & ChangeDetail.FirstPath
                RaiseEvent UserMessage(Message)
                SystemSounds.Hand.Play()
            End If
            Exit Sub
        End Try

        'If FirstFile.Exists Then 'Not a folder - don't check earlier as folder may be being created
        ' Log.Debug("CheckFilter", "CopyBackupFile")
        If FileFilter.CanCopy(FirstFile.Name, RelativeFolder) Then
            'Copy the new file
            'Log.Debug("FilterPass " & VersionFiles.ToString & FTPEnable.ToString, "CopyBackupFile")
            Try
                If VersionFiles Then
                    If FTPEnable Then
                        If VersionRate > 0 Then
                            Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(String.Concat(BackupFolder, RelativeFolder), False)
                            Dim blnWithinRate As Boolean
                            For Each ftpFile As FTPfileInfo In ftpList
                                If ftpFile.Filename.StartsWith(filChanged.Name) Then
                                    If ftpFile.FileDateTime > Now.AddSeconds(-VersionRate) Then
                                        blnWithinRate = True
                                        Exit For
                                    End If
                                End If
                            Next
                            If blnWithinRate Then
                                Log.Debug("Skipping backup. Exceeds versioning rate: " & strFullName, "FileCopy")
                            Else
                                If FTPUpload(ChangeDetail.FirstPath, String.Concat(strFullName, ".", Now.ToString(DateTimeStamp), filChanged.Extension)) Then
                                    If ShowEvents Then RaiseEvent UserMessage("File Copied: " & ChangeDetail.FirstPath)
                                Else
                                    Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                                End If
                            End If
                        Else
                            If FTPUpload(ChangeDetail.FirstPath, String.Concat(strFullName, ".", Now.ToString(DateTimeStamp), filChanged.Extension)) Then
                                If ShowEvents Then RaiseEvent UserMessage("File Copied: " & ChangeDetail.FirstPath)
                            Else
                                Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                            End If
                        End If
                    Else
                        'Log.Debug("CheckRate", "CopyBackupFile")
                        Dim diBackupFolder As New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)))
                        If Not diBackupFolder.Exists Then Filesystem.Directory.CreateDirectory(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)))
                        Dim strLastFile As String = String.Empty
                        Dim datLastFile As DateTime
                        Dim blnWithinRate As Boolean
                        Dim datLastCutoOffDate As DateTime = Now.AddSeconds(-VersionRate)
                        Dim aryFiles() As Filesystem.FileInfo = New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder))).GetFiles(filChanged.Name & ".*")
                        For Each bakFile As Filesystem.FileInfo In aryFiles
                            If VersionRate > 0 AndAlso Not blnWithinRate AndAlso bakFile.LastWriteTime > datLastCutoOffDate Then
                                blnWithinRate = True
                            End If
                            If strLastFile = String.Empty Then strLastFile = bakFile.FullName
                            If bakFile.LastWriteTime > datLastFile Then
                                strLastFile = bakFile.FullName
                                datLastFile = bakFile.LastWriteTime
                            End If
                        Next
                        If VersionRate > 0 AndAlso blnWithinRate Then
                            Log.Debug("Skipping backup. Exceeds versioning rate: " & strFullName, "FileCopy")
                        ElseIf CompareBeforeCopy AndAlso FileHelper.CompareFilesSame(ChangeDetail.FirstPath, strLastFile) Then
                            Log.Debug("Skipping backup. File content not changed: " & strLastFile, "FileCopy")
                        Else
                            If RunCopyFirst Then
                                strLastFile = String.Concat(strFullName, ".", Now.ToString(DateTimeStamp), filChanged.Extension)
                                Filesystem.File.Copy(Filesystem.Path.GetLongPath(ChangeDetail.FirstPath), Filesystem.Path.GetLongPath(strLastFile), True)
                                Filesystem.File.SetAttributes(Filesystem.Path.GetLongPath(strLastFile), FileAttributes.Normal)
                                If ShowEvents Then RaiseEvent UserMessage("File Copied: " & ChangeDetail.FirstPath)
                            End If
                            If RunCopy <> String.Empty Then
                                Try
                                    Process.Start(RunCopy, RunCopyArgs.Replace("{0}", ChangeDetail.FirstPath).Replace("{1}", _
                                                  String.Concat(strFullName, ".", Now.ToString(DateTimeStamp), filChanged.Extension)))
                                Catch ex As Exception
                                    Log.Warn( _
                                        "Run " & RunCopy & " on copy error: " & ex.Message & " " & ChangeDetail.FirstPath, "RunOnCopy")
                                    If ShowErrors Then
                                        Message = "Run " & RunCopy & " on copy error: " & ex.Message & vbCrLf & ChangeDetail.FirstPath
                                        RaiseEvent UserMessage(Message)
                                        SystemSounds.Hand.Play()
                                    End If
                                End Try
                            End If
                        End If
                    End If
                Else
                    'not versioning
                    If FTPEnable Then
                        If VersionPrev Then
                            Try
                                ftp.FtpRename(strFullName, String.Concat(strFullName, ".", ftp.GetDateTimestamp(strFullName).ToString(DateTimeStamp), filChanged.Extension))
                            Catch
                            End Try
                        End If
                        If FTPUpload(ChangeDetail.FirstPath, strFullName) Then
                            If ShowEvents Then RaiseEvent UserMessage("File Copied: " & ChangeDetail.FirstPath)
                        Else
                            Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                        End If
                    Else
                        Dim diBackupFolder As New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)))
                        If Not diBackupFolder.Exists Then Filesystem.Directory.CreateDirectory(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)))
                        If VersionPrev Then
                            Try
                                If Filesystem.File.Exists(Filesystem.Path.GetLongPath(strFullName)) Then
                                    Filesystem.File.Move(Filesystem.Path.GetLongPath(strFullName), Filesystem.Path.GetLongPath(String.Concat(strFullName, ".", New Filesystem.FileInfo(strFullName).LastWriteTime.ToString(DateTimeStamp), filChanged.Extension)))
                                End If
                            Catch
                            End Try
                        End If
                        If RunCopyFirst Then
                            Filesystem.File.Copy(Filesystem.Path.GetLongPath(ChangeDetail.FirstPath), Filesystem.Path.GetLongPath(strFullName), True)
                            Filesystem.File.SetAttributes(Filesystem.Path.GetLongPath(strFullName), FileAttributes.Normal)
                            If ShowEvents Then RaiseEvent UserMessage("File Copied: " & ChangeDetail.FirstPath)
                        End If
                        If RunCopy <> String.Empty Then
                            Try
                                Process.Start(RunCopy, RunCopyArgs.Replace("{0}", ChangeDetail.FirstPath).Replace("{1}", strFullName))
                            Catch ex As Exception
                                Log.Warn("Run " & RunCopy & " on copy error: " & ex.Message & " " & ChangeDetail.FirstPath, "RunOnCopy")
                                If ShowErrors Then
                                    Message = "Run " & RunCopy & " on copy error: " & ex.Message & vbCrLf & ChangeDetail.FirstPath
                                    RaiseEvent UserMessage(Message)
                                    SystemSounds.Hand.Play()
                                End If
                            End Try
                        End If
                    End If
                End If
                'Log.Debug(ChangeDetail.FirstPath, "FileCopied")
                CountChanged += 1
            Catch ex As Exception
                Log.Warn(ex.Message, "Error copying file (try " & ChangeDetail.Retries & "):")
                ChangeDetail.Retries += 1
                WatchFailure = True
                If ChangeDetail.Retries <= MaxRetries Then
                    RetryQueue.Enqueue(ChangeDetail)
                Else
                    WatchFailure = True
                End If
                If ShowErrors And ChangeDetail.Retries = 1 Then
                    Message = "Error copying file (will retry over next " & (MaxRetries * 10 / 60).ToString("#") & " mins):" & _
                              vbCrLf & ChangeDetail.FirstPath
                    RaiseEvent UserMessage(Message)
                    SystemSounds.Hand.Play()
                End If
            End Try
        Else
            Log.Debug(ChangeDetail.FirstPath, "File excluded:")
        End If
        'End If
    End Sub

    Private Sub DeleteFile(ByVal ChangeDetail As ChangeDetails)
        'Delete backups on watched file deletion
        'Log.Debug("start", "DeleteFile")
        For Each ChgDetailEnum As ChangeDetails In CopyQueue
            'Log.Debug(ChgDetailEnum.ChangeType.ToString & ": " & ChgDetailEnum.FirstPath)
            If ChgDetailEnum.ChangeType = WatcherChangeTypes.Created And ChgDetailEnum.FirstPath = ChangeDetail.FirstPath Then
                'This file has been deleted and new file created of the same name replacing it
                Log.Debug(ChangeDetail.FirstPath, "Changed:TempCreatedDelOrig")
                Exit Sub
            ElseIf _
                ChgDetailEnum.ChangeType = WatcherChangeTypes.Renamed And ChgDetailEnum.SecondPath = ChangeDetail.FirstPath Then
                'This file has been deleted and a another file renamed to the same name to replace it
                For Each ChgDetailCheck As ChangeDetails In CopyQueue
                    'Kill anything with the other temp file in it.
                    If ChgDetailCheck.FirstPath = ChgDetailEnum.FirstPath Then
                        'We can't remove from the queue so invalidate the entry. 
                        ChgDetailCheck.ChangeType = WatcherChangeTypes.Renamed
                        ChgDetailCheck.FirstPath = String.Empty
                    End If
                Next
                Log.Debug(ChangeDetail.FirstPath, "Changed:TempSwapDelOrig")
                'Do a backup of our file
                BackupFile(New ChangeDetails(WatcherChangeTypes.Changed, ChangeDetail.FirstPath, String.Empty))
                Exit Sub
            End If
        Next

        Dim strFile, strExt, RelativeFileName As String
        RelativeFileName = ChangeDetail.FirstPath.Substring(WatchFolder.Length)
        Dim RelativeFolder As String
        If RelativeFileName.Contains("\") Then
            RelativeFolder = RelativeFileName.Substring(0, RelativeFileName.LastIndexOf("\", StringComparison.Ordinal))
        Else
            RelativeFolder = String.Empty
        End If
        Dim fiFile As New Filesystem.FileInfo(Filesystem.Path.GetLongPath(ChangeDetail.FirstPath))
        strExt = fiFile.Extension
        '.Replace(".", String.Empty)
        strFile = fiFile.Name
        'Log.Debug("ready", "DeleteFile")

        If FileFilter.CanCopy(strFile, RelativeFolder) Then
            'Log.Debug(BackupFolder & RelativeFolder & " " & strFile & ".*.*", "del getfiles")
            'Log.Debug("filter pass", "DeleteFile")

            Dim aryFiles() As String
            If FTPEnable Then
                RelativeFolder = RelativeFolder.Replace("\", "/")
                Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(String.Concat(BackupFolder, RelativeFolder), False)
                aryFiles = New String() {}
                For Each ftpFile As FTPfileInfo In ftpList
                    If ftpFile.Filename.StartsWith(strFile) Then
                        If IsNothing(aryFiles) Then
                            ReDim aryFiles(0)
                        Else
                            ReDim Preserve aryFiles(aryFiles.Length)
                        End If
                        aryFiles(aryFiles.Length - 1) = String.Concat(BackupFolder, RelativeFolder, "/", ftpFile.Filename)
                    End If
                Next
            Else
                If VersionFiles Then
                    aryFiles = Filesystem.Directory.GetFiles(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)), String.Concat(strFile, ".*", strExt))
                Else
                    aryFiles = Filesystem.Directory.GetFiles(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)), strFile)
                End If
            End If
            If aryFiles.Length > 0 Then
                'It is file
                'Is Move? In same delayed write, file deleted and file created with same name
                Dim blnIsMove As Boolean = False
                'If is move, Remap pending create operation to a move (rename). Abort current operation
                For Each ChgDetailEnum As ChangeDetails In CopyQueue
                    If _
                        ChgDetailEnum.ChangeType = WatcherChangeTypes.Created And _
                        ChgDetailEnum.FirstPath.EndsWith(strFile) Then
                        ChgDetailEnum.ChangeType = WatcherChangeTypes.Renamed
                        ChgDetailEnum.SecondPath = ChgDetailEnum.FirstPath
                        ChgDetailEnum.FirstPath = ChangeDetail.FirstPath
                        blnIsMove = True
                        'Log.Debug(strFile, "FileMove")
                        'Log.Debug(ChangeDetail.FirstPath & " to " & ChgDetailEnum.SecondPath, "IsMove")
                    End If
                Next
                'Log.Debug("prep " & blnIsMove.ToString, "DeleteFile")

                If Not blnIsMove Then
                    For Each strFile In aryFiles
                        'Log.Debug(strFile, "del")
                        If DeleteOnDelete Then
                            Try
                                If FTPEnable Then
                                    If ftp.FtpDelete(strFile) Then
                                        If ShowEvents Then RaiseEvent UserMessage("File deleted: " & strFile)
                                    Else
                                        Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                                    End If
                                ElseIf UseRecycleBin Then
                                    FileHelper.DeleteFileToRecycleBin(strFile)
                                    If ShowEvents Then RaiseEvent UserMessage("File deleted: " & strFile)
                                    'My.Computer.FileSystem.DeleteFile(strFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.ThrowException)
                                Else
                                    Filesystem.File.Delete(Filesystem.Path.GetLongPath(strFile))
                                    If ShowEvents Then RaiseEvent UserMessage("File deleted: " & strFile)
                                End If
                                CountDeleted += 1
                            Catch
                                Log.Warn(strFile, "Error deleting file:")
                                ChangeDetail.Retries += 1
                                If ChangeDetail.Retries <= MaxRetries Then
                                    RetryQueue.Enqueue(ChangeDetail)
                                Else
                                    WatchFailure = True
                                End If
                                If ShowErrors And ChangeDetail.Retries = 1 Then
                                    Message = "Error deleting file (will retry over next " & (MaxRetries * 10 / 60).ToString("#") & " mins):" & vbCrLf & strFile
                                    RaiseEvent UserMessage(Message)
                                    SystemSounds.Hand.Play()
                                End If
                            End Try
                        End If
                    Next
                End If
            Else
                'Log.Debug("isfolder ", "DeleteFile")
                'It is folder
                'Is Move? In same delayed write, folder deleted and folder created with same child name
                If RelativeFileName.EndsWith("\") Then _
                    RelativeFileName = RelativeFileName.Substring(0, RelativeFileName.Length - 1)
                If RelativeFileName.Contains("\") Then
                    RelativeFolder = RelativeFileName.Substring(RelativeFileName.LastIndexOf("\", StringComparison.Ordinal))
                Else
                    RelativeFolder = String.Concat("\", RelativeFileName)
                End If
                Dim blnIsMove As Boolean = False
                'If is move, Remap pending create operation to a move (rename). Abort current operation
                For Each ChgDetailEnum As ChangeDetails In CopyQueue
                    If _
                        ChgDetailEnum.ChangeType = WatcherChangeTypes.Created And ChgDetailEnum.FirstPath.EndsWith(RelativeFolder) Then
                        ChgDetailEnum.ChangeType = WatcherChangeTypes.Renamed
                        ChgDetailEnum.SecondPath = ChgDetailEnum.FirstPath
                        ChgDetailEnum.FirstPath = ChangeDetail.FirstPath
                        blnIsMove = True
                        'Log.Debug(RelativeFolder, "FolderMove")
                        'Log.Debug(ChangeDetail.FirstPath & " to " & ChgDetailEnum.SecondPath, "IsMove")
                    End If
                Next
                If Not blnIsMove And DeleteOnDelete Then
                    Try
                        If FTPEnable Then
                            If ftp.FtpDeleteDirectory(String.Concat(BackupFolder, RelativeFileName)) Then
                                If ShowEvents Then RaiseEvent UserMessage("File deleted: " & String.Concat(BackupFolder, RelativeFileName))
                            Else
                                Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                            End If
                        Else
                            If Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFileName))) Then
                                If UseRecycleBin Then
                                    FileHelper.DeleteFileToRecycleBin(String.Concat(BackupFolder, RelativeFileName))
                                    'My.Computer.FileSystem.DeleteDirectory(String.Concat(BackupFolder, RelativeFileName), FileIO.DeleteDirectoryOption.DeleteAllContents, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.ThrowException)
                                Else
                                    Filesystem.Directory.Delete(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFileName)), True)
                                End If
                                If ShowEvents Then RaiseEvent UserMessage("Folder deleted: " & String.Concat(BackupFolder, RelativeFileName))
                            End If
                        End If
                        CountDeleted += 1
                    Catch
                        Log.Warn(BackupFolder & RelativeFileName, "Error deleting folder:")
                        ChangeDetail.Retries += 1
                        If ChangeDetail.Retries <= MaxRetries Then
                            RetryQueue.Enqueue(ChangeDetail)
                        Else
                            WatchFailure = True
                        End If
                        If ShowErrors And ChangeDetail.Retries = 1 Then
                            Message = "Error deleting folder (will retry over next " & (MaxRetries * 10 / 60).ToString("#") & " mins):" & vbCrLf & BackupFolder & RelativeFileName
                            RaiseEvent UserMessage(Message)
                            SystemSounds.Hand.Play()
                        End If
                    End Try
                End If
            End If
            'File or Folder
        Else
            Log.Debug(strFile, "File excluded:")
        End If
    End Sub

#End Region

#Region " Maintenence Functions "

    'Public Sub BackgroungWorkerRun(ByVal strMode As String)
    '    'Run on background thread so UI thread is free
    '    If bwBackupRestore.IsBusy Then
    '        Log.Warn("BackupRestore thread already busy. Cannot start " & strMode, "BackgroundWorker")
    '        MessageBox.Show("AutoVer is currently doing a Backup/Restore operation. Retry later.", "Backup/Restore", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    '    Else
    '        If Not strMode.EndsWith("SILENT") Then
    '            frmBackupRestoreRunning = New BackupFilesWait
    '            frmBackupRestoreRunning.WaitMessage = "Ensuring backup is current..."
    '            frmBackupRestoreRunning.FormTitle = "AutoVer Ensuring Backup"
    '            frmBackupRestoreRunning.Show()
    '            Application.DoEvents()
    '        End If
    '        bwBackupRestore.RunWorkerAsync(strMode)
    '    End If
    'End Sub

    'Private Sub bwBackupRestore_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles bwBackupRestore.DoWork
    '    Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)
    '    Log.Debug(e.Argument, "BackupRestoreWorker")
    '    If e.Argument.ToString.StartsWith("ENSURE") Then
    '        EnsureBackupCurrent()
    '    ElseIf e.Argument.ToString.StartsWith("ARCHIVE/DELETE") Then
    '        DeleteArchiveVersions()
    '    End If
    '    If EnsureRestoreCancelled Then e.Cancel = True
    '    e.Result = e.Argument '& "-OK"
    'End Sub

    'Private Sub bwBackupRestore_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles bwBackupRestore.RunWorkerCompleted
    '    Log.Debug(e.Result, "BackupRestoreWorker")
    '    If (e.Error IsNot Nothing) Then
    '        Log.Warn(e.Error.Message, "BackgroundWorker")
    '        MessageBox.Show(e.Error.Message)
    '    ElseIf e.Cancelled Then
    '        Log.Warn("User Cancelled", "BackgroundWorker")
    '    End If
    '    If frmBackupRestoreRunning.Visible Then frmBackupRestoreRunning.Hide()
    'End Sub

    Public Sub EnsureBackupCurrent()
        Try
            If Not Started Then
                'Removable drive may have just come online. Start it first (it will check both folders first).
                SetupWatcher()
            End If
            If Started Then
                EnsureMessage = "Ensuring " & Name & " backup..."
                If ShowEvents Then RaiseEvent UserMessage(EnsureMessage)
                OperationCount = 0
                EnsureRestoreCancelled = False
                Log.Info("Ensuring backup files are current", Name)
                If FTPEnable Then
                    If Not Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(WatchFolder)) Or Not ftp.FtpDirectoryExists(BackupFolder) Then
                        Log.Warn(Name & ": Watch or Backup folder does not exist!", "EnsureBackupCurrent")
                        Exit Sub
                    End If
                Else
                    If Not Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(WatchFolder)) Or Not Filesystem.Directory.Exists(Filesystem.Path.GetLongPath(BackupFolder)) _
                        Then
                        Log.Warn(Name & ": Watch or Backup folder does not exist!", "EnsureBackupCurrent")
                        Exit Sub
                    End If
                End If

                If FTPEnable Then
                    EnsureBackupCopyFTP(New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(WatchFolde)r))
                Else
                    EnsureBackupCopy(New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(WatchFolder)))
                End If
                EnsureMessage = String.Concat(IIf(EnsureRestoreCancelled, "Copy aborted by user: ", String.Empty), Name, _
                                              "> Files copied to ensure backup: ", OperationCount.ToString)
                Log.Info(EnsureMessage, Name)
                'EnsureRestoreCancelled = True
                If ShowEvents Or EnsureRestoreCancelled Then RaiseEvent UserMessage(EnsureMessage)
            End If

        Catch ex As Exception
            Log.Error(ex.ToString, "EnsureBackupCurrent")
        End Try
    End Sub

    'Private Sub BackupRestoreCancelled() Handles frmBackupRestoreRunning.BackupRestoreCancelled
    '    EnsureRestoreCancelled = True
    'End Sub

    Private Sub EnsureBackupCopy(ByVal Directory As Filesystem.DirectoryInfo)
        'Check all files are backed up
        If EnsureRestoreCancelled Then Exit Sub
        Log.Debug(Directory.FullName, "EnsureBackupCopy")
        'Application.DoEvents()
        Dim File As Filesystem.FileInfo
        'Create backup folder if it doesn't exist
        Dim RelativeFolder As String = String.Concat(Directory.FullName.Substring(WatchFolder.Length), "\")
        If RelativeFolder = "\" Then RelativeFolder = String.Empty
        Dim DestinFile As String
        Try
            Dim diBackupFolder As New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)))
            If Not diBackupFolder.Exists Then Filesystem.Directory.CreateDirectory(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder)))
            'Dim FileFilter As New FileFolderFilter()
            'FileFilter.SetupFilters(IncludeFiles, ExcludeFiles, ExcludeFolders)
            '  If Not IsNothing(frmBackupRestoreRunning) Then frmBackupRestoreRunning.lblStatus.Text 
            EnsureMessage = String.Concat("Folder: ...\", RelativeFolder)

            'Copy files
            If VersionFiles Then
                Dim blnFileExists As Boolean
                Dim aryFiles() As Filesystem.FileInfo
                Dim datLastWriteMin, datFileLastWrite As DateTime
                For Each File In Directory.GetFiles(IncludeFiles)
                    If FileFilter.CanCopy(File.Name, RelativeFolder) Then
                        aryFiles = New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder))).GetFiles(String.Concat(File.Name, ".*"))
                        blnFileExists = False
                        Try
                            datLastWriteMin = File.LastWriteTime.AddSeconds(-3)
                            '3 sec variance is ok due to copying latency and storage time accuracy
                            'datLastWriteMax = File.LastWriteTime.AddSeconds(3)
                            For Each bakFile As Filesystem.FileInfo In aryFiles
                                datFileLastWrite = bakFile.LastWriteTime
                                If datFileLastWrite >= datLastWriteMin Then blnFileExists = True
                                'And datFileLastWrite <= datLastWriteMax
                            Next
                            If File.Length > MaxFileSize Then blnFileExists = True
                            If Not blnFileExists Then
                                DestinFile = String.Concat(BackupFolder, RelativeFolder, File.Name, ".", Now.ToString(DateTimeStamp), File.Extension)
                                Filesystem.File.Copy(Filesystem.Path.GetLongPath(File.FullName), Filesystem.Path.GetLongPath(DestinFile), True)
                                Filesystem.File.SetAttributes(Filesystem.Path.GetLongPath(DestinFile), FileAttributes.Normal)
                                OperationCount += 1
                            End If
                        Catch ex As Exception
                            Log.Warn(String.Format("{0} ({1})", File.FullName, ex.Message), "Error copying file:")
                        End Try
                    End If
                Next
            Else
                For Each File In Directory.GetFiles(IncludeFiles)
                    If FileFilter.CanCopy(File.Name, RelativeFolder) Then
                        'Copy file if not filtered and different from existing file
                        Try
                            If Filesystem.File.Exists(Filesystem.Path.GetLongPath(BackupFolder & RelativeFolder & File.Name)) Then
                                If File.LastWriteTime <> Filesystem.File.GetLastWriteTime(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder, File.Name))) And File.Length <= MaxFileSize Then
                                    DestinFile = String.Concat(BackupFolder, RelativeFolder, File.Name)
                                    Filesystem.File.Copy(Filesystem.Path.GetLongPath(File.FullName), Filesystem.Path.GetLongPath(DestinFile), True)
                                    Filesystem.File.SetAttributes(Filesystem.Path.GetLongPath(DestinFile), FileAttributes.Normal)
                                    OperationCount += 1
                                End If
                            ElseIf File.Length <= MaxFileSize Then
                                DestinFile = String.Concat(BackupFolder, RelativeFolder, File.Name)
                                Filesystem.File.Copy(Filesystem.Path.GetLongPath(File.FullName), Filesystem.Path.GetLongPath(DestinFile), True)
                                Filesystem.File.SetAttributes(Filesystem.Path.GetLongPath(DestinFile), FileAttributes.Normal)
                                OperationCount += 1
                            End If
                        Catch ex As Exception
                            Log.Warn(String.Format("{0} ({1})", File.FullName, ex.Message), "Error copying file:")
                        End Try
                    End If
                Next
            End If
            'Recurse directories
            If SubFolders Then
                For Each SubDir As Filesystem.DirectoryInfo In Directory.GetDirectories
                    'Application.DoEvents()
                    If FileFilter.CanCopyFolder(SubDir.FullName) Then EnsureBackupCopy(SubDir)
                Next
            End If
        Catch ex As Exception
            Log.Warn( _
                String.Concat(ex.Message, " # Source:", Directory.FullName, ", Target:", BackupFolder, RelativeFolder), "EnsureBackupCopy:")
        End Try
    End Sub

    Private Sub EnsureBackupCopyFTP(ByVal Directory As Filesystem.DirectoryInfo)
        'Check all files are backed up
        If EnsureRestoreCancelled Then Exit Sub
        'Application.DoEvents()
        Dim File As Filesystem.FileInfo
        'Create backup folder if it doesn't exist
        Dim RelativeFolder As String = String.Concat(Directory.FullName.Substring(WatchFolder.Length), "/")
        If RelativeFolder = "/" Then RelativeFolder = String.Empty
        RelativeFolder = RelativeFolder.Replace("\", "/")
        Try
            Dim strBackupFolder As String = String.Concat(BackupFolder, RelativeFolder)
            If Not ftp.FtpDirectoryExists(strBackupFolder) Then ftp.FtpCreateDirectory(strBackupFolder)
            'Dim FileFilter As New FileFolderFilter()
            'FileFilter.SetupFilters(IncludeFiles, ExcludeFiles, ExcludeFolders)
            'If Not IsNothing(frmBackupRestoreRunning) Then frmBackupRestoreRunning.lblStatus.Text
            EnsureMessage = String.Concat("Folder: .../", RelativeFolder)

            'Copy files
            If VersionFiles Then
                Dim blnFileExists As Boolean
                Dim aryFiles() As String
                Dim datLastWriteMin As DateTime
                For Each File In Directory.GetFiles(IncludeFiles)
                    If FileFilter.CanCopy(File.Name, RelativeFolder) Then
                        Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(String.Concat(BackupFolder, RelativeFolder), False)
                        aryFiles = New String() {}
                        blnFileExists = False
                        datLastWriteMin = File.LastWriteTime.AddSeconds(-5)
                        '5 sec variance is ok due to copying latency and storage time accuracy
                        For Each ftpFile As FTPfileInfo In ftpList
                            If ftpFile.Filename.StartsWith(File.Name) Then
                                If IsNothing(aryFiles) Then
                                    ReDim aryFiles(0)
                                Else
                                    ReDim Preserve aryFiles(aryFiles.Length)
                                End If
                                aryFiles(aryFiles.Length - 1) = String.Concat(BackupFolder, RelativeFolder, "/", ftpFile.Filename)
                                If ftpFile.FileDateTime >= datLastWriteMin Then blnFileExists = True
                                If File.Length > MaxFileSize Then blnFileExists = True
                            End If
                        Next
                        Try
                            If Not blnFileExists Then
                                If Not ftp.Upload(File.FullName, String.Concat(BackupFolder, RelativeFolder, File.Name, ".", Now.ToString(DateTimeStamp), File.Extension)) Then _
                                    Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                                OperationCount += 1
                            End If
                        Catch ex As Exception
                            Log.Warn(String.Format("{0} ({1})", File.FullName, ex.Message), "Error copying file:")
                        End Try
                    End If
                Next
            Else
                Dim datLastWriteFile, datLastWriteFTP As DateTime
                For Each File In Directory.GetFiles
                    If FileFilter.CanCopy(File.Name, RelativeFolder) Then
                        'Copy file if not filtered and different from existing file
                        Try
                            If ftp.FtpFileExists(BackupFolder & RelativeFolder & File.Name) Then
                                datLastWriteFile = File.LastWriteTime
                                datLastWriteFTP = ftp.GetDateTimestamp(String.Concat(BackupFolder, RelativeFolder, File.Name))
                                If (datLastWriteFTP > datLastWriteFile.AddSeconds(5) Or datLastWriteFTP < datLastWriteFile.AddSeconds(-5)) And File.Length <= MaxFileSize Then
                                    Log.Debug(datLastWriteFTP.ToString & " <> " & datLastWriteFile.ToString, "EnsureBackupCopyFTP")
                                    If Not ftp.Upload(File.FullName, String.Concat(BackupFolder, RelativeFolder, File.Name)) Then Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                                    OperationCount += 1
                                End If
                            ElseIf File.Length <= MaxFileSize Then
                                If Not ftp.Upload(File.FullName, String.Concat(BackupFolder, RelativeFolder, File.Name)) Then Throw New Exception("FTP Returned error: " & ftp.ErrorText)
                                OperationCount += 1
                            End If
                        Catch ex As Exception
                            Log.Warn(String.Format("{0} ({1})", File.FullName, ex.Message), "Error copying file:")
                        End Try
                    End If
                Next
            End If
            'Recurse directories
            If SubFolders Then
                For Each SubDir As Filesystem.DirectoryInfo In Directory.GetDirectories
                    ' Application.DoEvents()
                    If FileFilter.CanCopyFolder(SubDir.FullName) Then EnsureBackupCopyFTP(SubDir)
                Next
            End If
        Catch ex As Exception
            Log.Warn(String.Concat(ex.Message, " # Source:", Directory.FullName, ", Target:", BackupFolder, RelativeFolder), "EnsureBackupCopy:")
        End Try
    End Sub

    Public Sub DeleteArchiveVersions()
        If FTPEnable Then Exit Sub
        If MaxVersionAction = "N"c Then Exit Sub
        OperationCount = 0
        EnsureRestoreCancelled = False
        Dim FilesToZip As New ArrayList
        If MaxVersionAction = "Z"c Then
            Log.Info("Zipping old files", Name)
            ZipOldVersions(New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(WatchFolder)), FilesToZip)

            'Zip the files - if any
            If FilesToZip.Count > 0 Then
                Dim FilesToDelete As New ArrayList

                Try
                    Dim zipFile As ZipFile
                    Dim blnCreatingNewZip As Boolean
                    If Filesystem.File.Exists(Filesystem.Path.GetLongPath(BackupFolder & ZipFileName)) Then
                        blnCreatingNewZip = False
                        zipFile = zipFile.Read(Filesystem.Path.GetLongPath(BackupFolder & ZipFileName))
                    Else
                        blnCreatingNewZip = True
                        zipFile = New ZipFile()
                        zipFile.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression
                        zipFile.Comment = "AutoVer old versions backup file"
                    End If

                    'Add files to the zip. Add 1 by 1 for granular fault tolerance (zip what we can). Does not support long paths.
                    Dim StorePath As String
                    'zipFile.AddFiles(aryFilesToZip, RelativeFolder)
                    For Each strFile As String In FilesToZip
                        StorePath = strFile.Substring(BackupFolder.Length)
                        If StorePath.Contains("\") Then
                            StorePath = StorePath.Substring(0, StorePath.LastIndexOf("\", StringComparison.Ordinal))
                        Else
                            StorePath = String.Empty
                        End If
                        Try
                            zipFile.UpdateFile(strFile, StorePath)
                            FilesToDelete.Add(strFile)
                        Catch ex As Exception
                            Log.Warn(ex.Message & ": " & strFile, "Error coping file to zip:")
                        End Try
                    Next

                    Try
                        If blnCreatingNewZip Then
                            zipFile.Save(BackupFolder & ZipFileName)
                        Else
                            zipFile.Save()
                        End If
                    Catch ex As Exception
                        Log.Warn(zipFile.Name & vbNewLine & ex.Message, "Error saving zip file:")
                        FilesToDelete.Clear()
                    End Try
                Catch ex As Exception
                    Log.Error(ex.Message, "Zip File:")
                    FilesToDelete.Clear()
                End Try

                For Each strFile As String In FilesToDelete
                    Try
                        If UseRecycleBin Then
                            FileHelper.DeleteFileToRecycleBin(strFile)
                        Else
                            Filesystem.File.Delete(Filesystem.Path.GetLongPath(strFile))
                        End If
                    Catch ex As Exception
                        Log.Warn(ex.Message & ": " & strFile, "Error deleting file after copy to zip:")
                    End Try
                Next
            End If
            Log.Info( _
                String.Concat(IIf(EnsureRestoreCancelled, "Zip aborted by user. ", String.Empty), _
                              "Old version files zipped per schedule: ", FilesToZip.Count.ToString), Name)
        ElseIf MaxVersionAction = "7"c Then
            Log.Info("7-Zipping old files", Name)
            ZipOldVersions(New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(WatchFolder)), FilesToZip)

            '7-Zip the files - if any
            If FilesToZip.Count > 0 Then
                Dim FilesToDelete As New ArrayList

                Try
                    Dim strZipDll As String
                    Dim pfx86 As String = Environment.GetEnvironmentVariable("ProgramFiles(x86)")
                    If String.IsNullOrEmpty(pfx86) Then pfx86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                    If File.Exists(AppStartPath & "7z.dll") Then
                        strZipDll = AppStartPath & "7z.dll"
                    ElseIf File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\7-Zip\7z.dll") Then
                        strZipDll = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\7-Zip\7z.dll"
                    ElseIf File.Exists(pfx86 & "\7-Zip\7z.dll") Then
                        strZipDll = pfx86 & "\7-Zip\7z.dll"
                    Else
                        Log.Warn("7z.dll not found", "7ZipFiles")
                        Exit Sub
                    End If
                    SevenZipCompressor.SetLibraryPath(strZipDll)
                    Dim zip7File As SevenZipCompressor = New SevenZipCompressor()
                    zip7File.ArchiveFormat = OutArchiveFormat.SevenZip
                    zip7File.CompressionLevel = CompressionLevel.High
                    zip7File.DirectoryStructure = True
                    'zip7File.PreserveDirectoryRoot = False
                    If Filesystem.File.Exists(Filesystem.Path.GetLongPath(BackupFolder & ZipFileName.Replace(".zip", ".7z"))) Then
                        zip7File.CompressionMode = CompressionMode.Append
                    Else
                        zip7File.CompressionMode = CompressionMode.Create
                        ' zip7File.ArchiveFileInfo.Comment = "AutoVer old versions backup file"
                    End If
                    'Have to add all files at once.
                    Dim aryFiles(FilesToZip.Count - 1) As String
                    FilesToZip.CopyTo(aryFiles)
                    zip7File.CompressFiles(Filesystem.Path.GetLongPath(BackupFolder & ZipFileName.Replace(".zip", ".7z")), aryFiles)
                    FilesToDelete = FilesToZip
                Catch ex As Exception
                    Log.Warn(ex.Message, "Error saving 7-zip file:")
                End Try

                For Each strFile As String In FilesToDelete
                    Try
                        If UseRecycleBin Then
                            FileHelper.DeleteFileToRecycleBin(strFile)
                        Else
                            Filesystem.File.Delete(Filesystem.Path.GetLongPath(strFile))
                        End If
                        If File.Exists(strFile) Then
                            Log.Debug(strFile, "FileStillExists:" & UseRecycleBin.ToString)
                            File.Delete(strFile)
                            Log.Debug(strFile, "FileSysDel")
                        End If
                    Catch ex As Exception
                        Log.Warn(ex.Message & ": " & strFile, "Error deleting file after copy to zip:")
                    End Try
                Next
            End If
            Log.Info(String.Concat(IIf(EnsureRestoreCancelled, "7-Zip aborted by user. ", String.Empty), "Old version files zipped per schedule: ", FilesToZip.Count.ToString), Name)
        ElseIf MaxVersionAction = "D"c Then
            Log.Info("Deleting old files", Name)
            DeleteOldVersions(New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(WatchFolder)))
            Log.Info(String.Concat(IIf(EnsureRestoreCancelled, "Delete aborted by user. ", String.Empty), "Old version files deleted per schedule: ", OperationCount.ToString), Name)
        End If
    End Sub

    Private Sub ZipOldVersions(ByVal ScanDirectory As Filesystem.DirectoryInfo, ByRef FilesToZip As ArrayList)
        'Zip old files, but keep last version regardless
        If EnsureRestoreCancelled Then Exit Sub
        Dim SourceFile, BackupFile As Filesystem.FileInfo
        Dim RelativeFolder As String = String.Concat(ScanDirectory.FullName.Substring(WatchFolder.Length), "\")
        If RelativeFolder = "\" Then RelativeFolder = String.Empty
        Try
            Dim diBackupFolder As New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(BackupFolder & RelativeFolder))
            If Not diBackupFolder.Exists Then Filesystem.Directory.CreateDirectory(Filesystem.Path.GetLongPath(BackupFolder & RelativeFolde)r)
            Dim datCutOff As DateTime = Now.AddDays(-MaxVersionAge)

            If VersionFiles Then
                Dim aryCheckedFiles As New ArrayList
                ' Dim aryFilesToZip As New ArrayList
                Dim aryFiles() As Filesystem.FileInfo
                Dim strLastFile As String
                Dim datLastFile As DateTime

                For Each SourceFile In ScanDirectory.GetFiles()
                    'For each watched file, find the latest backup version
                    strLastFile = String.Empty
                    datLastFile = New DateTime(1900, 1, 1)
                    aryFiles = _
                        New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(String.Concat(BackupFolder, RelativeFolder))).GetFiles(String.Concat(SourceFile.Name, ".*", SourceFile.Extension))
                    For Each BackupFile In aryFiles
                        If strLastFile = String.Empty Then strLastFile = BackupFile.Name
                        If BackupFile.LastWriteTime > datLastFile Then
                            strLastFile = BackupFile.Name
                            datLastFile = BackupFile.LastWriteTime
                        End If
                    Next
                    'Prep to zip all but last version and those older than cut off date
                    For Each BackupFile In aryFiles
                        If BackupFile.Name <> strLastFile And BackupFile.LastWriteTime < datCutOff And BackupFile.FullName <> String.Concat(BackupFolder, ZipFileName) Then
                            FilesToZip.Add(BackupFile.FullName)
                            Log.Debug(BackupFile.Name & "; " & strLastFile & "; " & BackupFile.LastWriteTime & " < " & datCutOff, "ZipOldVer")
                        Else
                            aryCheckedFiles.Add(BackupFile.Name)
                        End If
                    Next
                Next
                'Prep to zip all other (orphaned) files older than cut off date
                aryFiles = New Filesystem.DirectoryInfo(Filesystem.Path.GetLongPath(BackupFolder & RelativeFolder)).GetFiles()
                For Each BackupFile In aryFiles
                    If Not aryCheckedFiles.Contains(BackupFile.Name) And Not FilesToZip.Contains(BackupFile.FullName) And BackupFile.FullName <> String.Concat(BackupFolder, ZipFileName) And _
                        BackupFile.LastWriteTime < datCutOff Then
                        FilesToZip.Add(BackupFile.FullName)
                    End If
                Next
            End If
            'Recurse directories
            If SubFolders Then
                'Find & recurse orphaned folders first
                Dim SubDir As Filesystem.DirectoryInfo
                Dim arySourceFolders As New ArrayList
                For Each SubDir In ScanDirectory.GetDirectories()
                    arySourceFolders.Add(SubDir.Name)
                Next
                Dim BackupDir As New Filesystem.DirectoryInfoFilesystem.Path.GetLongPath((String.Concat(BackupFolder, RelativeFolder)))
                For Each SubDir In BackupDir.GetDirectories()
                    If Not arySourceFolders.Contains(SubDir.Name) Then
                        ZipOldOrphanedVersions(SubDir, FilesToZip)
                    End If
                Next
                For Each SubDir In ScanDirectory.GetDirectories()
                    ZipOldVersions(SubDir, FilesToZip)
                Next
            End If
        Catch ex As Exception
            Log.Warn(String.Concat(ex.Message, " # Source:", ScanDirectory.FullName, ", Target:", BackupFolder, RelativeFolder), "ZipOldVersions:")
        End Try
    End Sub

    Private Sub ZipOldOrphanedVersions(ByVal ScanDirectory As Filesystem.DirectoryInfo, ByRef FilesToZip As ArrayList)
        'Zip all but those newer than cut off date
        If EnsureRestoreCancelled Then Exit Sub
        Dim datCutOff As DateTime = Now.AddDays(-MaxVersionAge)
        Try
            If ScanDirectory.GetFiles.Length > 0 Then
                'Add files to the zip queue
                'Dim blnErrorOnZip As Boolean = False
                For Each BackupFile As Filesystem.FileInfo In ScanDirectory.GetFiles
                    Log.Debug(BackupFile.Name & "; " & BackupFile.LastWriteTime & " < " & datCutOff, "ZipOldOrphan")
                    FilesToZip.Add(BackupFile.FullName)
                Next
            End If
            If ScanDirectory.GetFiles.Length = 0 And ScanDirectory.GetDirectories.Length = 0 Then _
                ScanDirectory.Delete(True)
        Catch ex As Exception
            Log.Warn(ex.Message, "Error deleting file:")
        End Try
        If ScanDirectory.Exists() Then
            'May have already been deleted above. If not recurse
            For Each SubDir As Filesystem.DirectoryInfo In ScanDirectory.GetDirectories()
                ZipOldOrphanedVersions(SubDir, FilesToZip)
            Next
        End If
    End Sub

    Private Sub DeleteOldVersions(ByVal ScanDirectory As Filesystem.DirectoryInfo)
        'Delete old files, but keep last version regardless
        If EnsureRestoreCancelled Then Exit Sub
        Dim SourceFile, BackupFile As Filesystem.FileInfo
        Dim RelativeFolder As String = String.Concat(ScanDirectory.FullName.Substring(WatchFolder.Length), "\")
        If RelativeFolder = "\" Then RelativeFolder = String.Empty
        Try
            Dim diBackupFolder As New Filesystem.DirectoryInfo(String.Concat(BackupFolder, RelativeFolder))
            If Not diBackupFolder.Exists Then Filesystem.Directory.CreateDirectory(String.Concat(BackupFolder, RelativeFolder))
            Dim datCutOff As DateTime = Now.AddDays(-MaxVersionAge)

            If VersionFiles Then
                Dim aryCheckedFiles As New ArrayList
                Dim aryFiles() As Filesystem.FileInfo
                Dim strLastFile As String
                Dim datLastFile As DateTime

                For Each SourceFile In ScanDirectory.GetFiles()
                    'For each watched file, find the latest backup version
                    strLastFile = String.Empty
                    datLastFile = New DateTime(1900, 1, 1)
                    aryFiles = _
                        New Filesystem.DirectoryInfo(String.Concat(BackupFolder, RelativeFolder)).GetFiles(String.Concat(SourceFile.Name, ".*", SourceFile.Extension))
                    For Each BackupFile In aryFiles
                        If strLastFile = String.Empty Then strLastFile = BackupFile.Name
                        If BackupFile.LastWriteTime > datLastFile Then
                            strLastFile = BackupFile.Name
                            datLastFile = BackupFile.LastWriteTime
                        End If
                    Next
                    'Delete all but last version and those newer than cut off date
                    Try
                        For Each BackupFile In aryFiles
                            If BackupFile.Name <> strLastFile And BackupFile.LastWriteTime < datCutOff Then
                                If UseRecycleBin Then
                                    FileHelper.DeleteFileToRecycleBin(BackupFile.FullName)
                                    'My.Computer.FileSystem.DeleteFile(BackupFile.FullName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.ThrowException)
                                Else
                                    BackupFile.Delete()
                                End If
                                OperationCount += 1
                            Else
                                aryCheckedFiles.Add(BackupFile.Name)
                            End If
                        Next
                    Catch ex As Exception
                        Log.Warn(ex.Message, "Error deleting file:")
                    End Try
                Next
                'Delete all other (orphaned) files newer than cut off date
                aryFiles = New Filesystem.DirectoryInfo(String.Concat(BackupFolder, RelativeFolder)).GetFiles()
                Try
                    For Each BackupFile In aryFiles
                        If Not aryCheckedFiles.Contains(BackupFile.Name) And BackupFile.LastWriteTime < datCutOff Then
                            If UseRecycleBin Then
                                FileHelper.DeleteFileToRecycleBin(BackupFile.FullName)
                                'My.Computer.FileSystem.DeleteFile(BackupFile.FullName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.ThrowException)
                            Else
                                BackupFile.Delete()
                            End If
                            OperationCount += 1
                        End If
                    Next
                Catch ex As Exception
                    Log.Warn(ex.Message, "Error deleting file:")
                End Try
            End If
            'Recurse directories
            If SubFolders Then
                Dim SubDir As Filesystem.DirectoryInfo
                If DeleteOnDelete Then
                    'Find & recurse orphaned folders first
                    Dim arySourceFolders As New ArrayList
                    For Each SubDir In ScanDirectory.GetDirectories()
                        arySourceFolders.Add(SubDir.Name)
                    Next
                    Dim BackupDir As New Filesystem.DirectoryInfo(String.Concat(BackupFolder, RelativeFolder))
                    For Each SubDir In BackupDir.GetDirectories()
                        If Not arySourceFolders.Contains(SubDir.Name) Then
                            DeleteOldOrphanedVersions(SubDir)
                        End If
                    Next
                End If
                For Each SubDir In ScanDirectory.GetDirectories()
                    DeleteOldVersions(SubDir)
                Next
            End If
        Catch ex As Exception
            Log.Warn(String.Concat(ex.Message, " # Source:", ScanDirectory.FullName, ", Target:", BackupFolder, RelativeFolder), "DeleteOldVersions:")
        End Try
    End Sub

    Private Sub DeleteOldOrphanedVersions(ByVal ScanDirectory As Filesystem.DirectoryInfo)
        'Delete all but those newer than cut off date
        If EnsureRestoreCancelled Then Exit Sub
        Dim datCutOff As DateTime = Now.AddDays(-MaxVersionAge)
        Try
            For Each BackupFile As Filesystem.FileInfo In ScanDirectory.GetFiles
                If BackupFile.LastWriteTime < datCutOff Then
                    If UseRecycleBin Then
                        FileHelper.DeleteFileToRecycleBin(BackupFile.FullName)
                        'My.Computer.FileSystem.DeleteFile(BackupFile.FullName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin, FileIO.UICancelOption.ThrowException)
                    Else
                        BackupFile.Delete()
                    End If
                    OperationCount += 1
                End If
            Next
            If ScanDirectory.GetFiles.Length = 0 And ScanDirectory.GetDirectories.Length = 0 Then _
                ScanDirectory.Delete(True)
        Catch ex As Exception
            Log.Warn(ex.Message, "Error deleting file:")
        End Try
        If ScanDirectory.Exists() Then
            'May have already been deleted above. If not recurse
            For Each SubDir As Filesystem.DirectoryInfo In ScanDirectory.GetDirectories()
                DeleteOldOrphanedVersions(SubDir)
            Next
        End If
    End Sub

    Public Sub RestoreAll(ByVal MaxFileDate As DateTime)
        'Restore all files (or latest version of)
        Log.Info("Restoring all files < " & MaxFileDate.ToString, Name)
        If FTPEnable Then
            If Not Filesystem.Directory.Exists(WatchFolder) Or Not ftp.FtpDirectoryExists(BackupFolder) Then
                EnsureMessage = Name & ": Watch or Backup folder does not exist!"
                Log.Warn(EnsureMessage, "RestoreAll")
                RaiseEvent UserMessage(EnsureMessage)
                Exit Sub
            End If
        Else
            If Not Filesystem.Directory.Exists(WatchFolder) Then Filesystem.Directory.CreateDirectory(WatchFolder)
            If Not Filesystem.Directory.Exists(WatchFolder) Or Not Filesystem.Directory.Exists(BackupFolder) Then
                EnsureMessage = Name & ": Watch or Backup folder does not exist!"
                Log.Warn(EnsureMessage, "RestoreAll")
                RaiseEvent UserMessage(EnsureMessage)
                Exit Sub
            End If
        End If
        'frmBackupRestoreRunning = New BackupFilesWait
        'frmBackupRestoreRunning.WaitMessage = "Restoring all files..."
        'frmBackupRestoreRunning.FormTitle = "AutoVer Restoring Backup"
        'frmBackupRestoreRunning.Show()
        'Application.DoEvents()

        EnsureRestoreCancelled = False
        Dim prevState As Boolean = Started
        If Started Then StopWatching()
        OperationCount = 0

        If FTPEnable Then
            RestoreAllFTP(BackupFolder, MaxFileDate)
        Else
            RestoreAllCopy(New Filesystem.DirectoryInfo(BackupFolder), MaxFileDate)
        End If

        If prevState Then StartWatching()
        'frmBackupRestoreRunning.lblStatus.Text()
        EnsureMessage = String.Concat(IIf(EnsureRestoreCancelled, "Copy aborted by user. ", String.Empty), Name & "> Files restored: ", OperationCount.ToString)
        Log.Info(EnsureMessage, Name)
        EnsureRestoreCancelled = True
        RaiseEvent UserMessage(EnsureMessage)
        ' Application.DoEvents()
        'MessageBox.Show(frmBackupRestoreRunning.lblStatus.Text, "Restore: " & Name, MessageBoxButtons.OK)
        'frmBackupRestoreRunning.Hide()
    End Sub

    Private Sub RestoreAllCopy(ByVal Directory As Filesystem.DirectoryInfo, ByVal MaxFileDate As DateTime)
        'Restore all files in this directory
        If EnsureRestoreCancelled Then Exit Sub
        Dim File As Filesystem.FileInfo
        'Create restore folder if it doesn't exist
        Dim RelativeFolder As String = String.Concat(Directory.FullName.Substring(BackupFolder.Length), "\")
        If RelativeFolder = "\" Then RelativeFolder = String.Empty
        Try
            Dim diWatchFolder As New Filesystem.DirectoryInfo(String.Concat(WatchFolder, RelativeFolder))
            If Not diWatchFolder.Exists Then Filesystem.Directory.CreateDirectory(String.Concat(WatchFolder, RelativeFolder))
            'Dim FileFilter As New FileFolderFilter()
            'FileFilter.SetupFilters(IncludeFiles, ExcludeFiles, ExcludeFolders)
            ' frmBackupRestoreRunning.lblStatus.Text 
            EnsureMessage = String.Concat("Folder: ...\", RelativeFolder)

            'Copy files
            If VersionFiles Then
                Dim aryFiles() As Filesystem.FileInfo
                Dim strLastFileActual As String = String.Empty
                Dim strFile As String
                Dim datFileLastWrite, datFileModified As DateTime
                'Dim aryCopy As New ArrayList

                aryFiles = New Filesystem.DirectoryInfo(String.Concat(BackupFolder, RelativeFolder)).GetFiles("*.*")
                'Build a list of files minus the timestamp
                Dim DestinFiles As New ArrayList()
                For Each bakFileActual As Filesystem.FileInfo In aryFiles
                    strFile = bakFileActual.FullName
                    If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                    If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                    If Not DestinFiles.Contains(strFile) Then DestinFiles.Add(strFile)
                Next

                For Each DestinFile As String In DestinFiles
                    'For each destination file, compare the file time of all versions of that file
                    datFileLastWrite = DateTime.MinValue
                    For Each bakFileActual As Filesystem.FileInfo In aryFiles
                        'Get the latest file within time spec of the same watch file, if any
                        strFile = bakFileActual.FullName
                        If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                        If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                        datFileModified = bakFileActual.LastWriteTime
                        If strFile = DestinFile Then
                            If datFileModified > datFileLastWrite And datFileModified <= MaxFileDate Then
                                strLastFileActual = bakFileActual.FullName
                                datFileLastWrite = datFileModified
                            End If
                        End If
                    Next
                    If datFileLastWrite > DateTime.MinValue Then
                        Try
                            Log.Debug(strLastFileActual & " " & datFileLastWrite & " > " & String.Concat(WatchFolder, RelativeFolder, DestinFile.Substring(DestinFile.LastIndexOf("\", StringComparison.Ordinal) + 1)))
                            If Filesystem.File.Exists(String.Concat(WatchFolder, RelativeFolder, DestinFile.Substring(DestinFile.LastIndexOf("\", StringComparison.Ordinal) + 1))) Then
                                Filesystem.File.SetAttributes(String.Concat(WatchFolder, RelativeFolder, DestinFile.Substring(DestinFile.LastIndexOf("\", StringComparison.Ordinal) + 1)), FileAttributes.Normal)
                            End If
                            Filesystem.File.Copy(strLastFileActual, String.Concat(WatchFolder, RelativeFolder, DestinFile.Substring(DestinFile.LastIndexOf("\", StringComparison.Ordinal) + 1)), True)
                            OperationCount += 1
                        Catch ex As Exception
                            Log.Warn(String.Format("{0} to {1} ({2})", strLastFileActual, _
                                              String.Concat(WatchFolder, RelativeFolder, DestinFile.Substring(DestinFile.LastIndexOf("\", StringComparison.Ordinal) + 1)), ex.Message), "Error copying file:")
                        End Try
                    End If
                Next
            Else
                'Non versioning copy
                Dim datFileModified As DateTime
                For Each File In Directory.GetFiles
                    datFileModified = File.LastWriteTime
                    If datFileModified <= MaxFileDate Then
                        Try
                            Log.Debug(File.FullName & " " & String.Concat(WatchFolder, RelativeFolder, File.Name))
                            If Filesystem.File.Exists(String.Concat(WatchFolder, RelativeFolder, File.Name)) Then
                                Filesystem.File.SetAttributes(String.Concat(WatchFolder, RelativeFolder, File.Name), _
                                                              FileAttributes.Normal)
                            End If
                            Filesystem.File.Copy(File.FullName, String.Concat(WatchFolder, RelativeFolder, File.Name), _
                                                 True)
                            OperationCount += 1
                        Catch ex As Exception
                            Log.Warn(String.Format("{0} ({1})", File.FullName, ex.Message), "Error copying file:")
                        End Try
                    End If
                Next
            End If
            'Recurse directories
            If SubFolders Then
                For Each SubDir As Filesystem.DirectoryInfo In Directory.GetDirectories
                    'Application.DoEvents()
                    RestoreAllCopy(SubDir, MaxFileDate)
                Next
            End If
        Catch ex As Exception
            Log.Warn( _
                String.Concat(ex.Message, " # Source:", Directory.FullName, ", Target:", BackupFolder, RelativeFolder), _
                "RestoreAll:")
        End Try
    End Sub

    Private Sub RestoreAllFTP(ByVal Directory As String, ByVal MaxFileDate As DateTime)
        'Restore all files in this directory
        If EnsureRestoreCancelled Then Exit Sub
        'Create restore folder if it doesn't exist
        Dim RelativeFolder As String = String.Concat(Directory.Substring(BackupFolder.Length), "/")
        If RelativeFolder = "/" Then RelativeFolder = String.Empty
        Dim RelativeFolderLocal As String = RelativeFolder.Replace("/", "\")
        Try
            If Not Filesystem.Directory.Exists(String.Concat(WatchFolder, RelativeFolderLocal)) Then _
                Filesystem.Directory.CreateDirectory(String.Concat(WatchFolder, RelativeFolderLocal))
            'Dim FileFilter As New FileFolderFilter()
            'FileFilter.SetupFilters(IncludeFiles, ExcludeFiles, ExcludeFolders)
            ' frmBackupRestoreRunning.lblStatus.Text 
            EnsureMessage = String.Concat("Folder: .../", RelativeFolder)

            'Copy files
            If VersionFiles Then
                Dim strLastFile As String = String.Empty
                Dim strLastFileActual As String = String.Empty
                Dim strFile As String
                Dim datFileLastWrite, datFileModified As DateTime
                Dim sdCopy As New SortedDictionary(Of String, DateTime)

                Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(String.Concat(BackupFolder, RelativeFolder), False)
                For Each ftpFile As FTPfileInfo In ftpList
                    If ftpFile.FileType = FTPfileInfo.DirectoryEntryTypes.File Then _
                        sdCopy.Add(ftpFile.FullName, ftpFile.FileDateTime)
                Next

                If sdCopy.Count > 0 Then
                    'ftpList.Sort()
                    For Each kvpCopy As KeyValuePair(Of String, DateTime) In sdCopy
                        strFile = kvpCopy.Key
                        If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                        If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                        strLastFile = strFile
                        strLastFileActual = kvpCopy.Key
                        datFileLastWrite = kvpCopy.Value
                        Exit For
                    Next
                    'strFile = ftpList(0).FullName
                    'If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                    'If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                    'strLastFile = strFile
                    'strLastFileActual = ftpList(0).FullName
                    'datFileLastWrite = ftpList(0).FileDateTime
                    'For Each ftpFile As Utilities.FTP.FTPfileInfo In ftpList
                    For Each kvpCopy As KeyValuePair(Of String, DateTime) In sdCopy
                        'Get the latest file of the same watch file, then copy it
                        'If ftpFile.FileType = Utilities.FTP.FTPfileInfo.DirectoryEntryTypes.File Then
                        strFile = kvpCopy.Key
                        'ftpFile.FullName
                        If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                        If strFile.Contains(".") Then strFile = strFile.Substring(0, strFile.LastIndexOf(".", StringComparison.Ordinal))
                        datFileModified = kvpCopy.Value
                        'ftpFile.FileDateTime
                        If strFile = strLastFile Then
                            If datFileModified > datFileLastWrite And datFileModified <= MaxFileDate Then
                                strLastFileActual = kvpCopy.Key
                                'ftpFile.FullName
                                datFileLastWrite = datFileModified
                            End If
                        ElseIf datFileModified <= MaxFileDate Then
                            Try
                                Log.Debug(strLastFileActual & " " & datFileLastWrite & " > " & _
                                    String.Concat(WatchFolder, RelativeFolderLocal, strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1)))
                                If Filesystem.File.Exists(String.Concat(WatchFolder, RelativeFolderLocal, _
                                                                         strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1))) Then
                                    Filesystem.File.SetAttributes(String.Concat(WatchFolder, RelativeFolderLocal, _
                                                      strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1)), FileAttributes.Normal)
                                End If
                                ftp.Download(strLastFileActual, String.Concat(WatchFolder, RelativeFolderLocal, _
                                                           strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1)), True)
                                OperationCount += 1
                            Catch ex As Exception
                                Log.Warn(String.Format("{0} to {1} ({2})", strLastFileActual, _
                                                  String.Concat(WatchFolder, RelativeFolderLocal, strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1)), ex.ToString), "Error downloading file:")
                            End Try
                            strLastFileActual = kvpCopy.Key
                            'ftpFile.FullName
                            strLastFile = strFile
                            datFileLastWrite = datFileLastWrite
                        End If
                        'End If
                    Next
                    If datFileModified <= MaxFileDate Then
                        Try
                            Log.Debug(strLastFileActual & " " & datFileLastWrite & " > " & _
                                String.Concat(WatchFolder, RelativeFolderLocal, strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1)))
                            If Filesystem.File.Exists(String.Concat(WatchFolder, RelativeFolderLocal, _
                                                                     strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1))) Then
                                Filesystem.File.SetAttributes(String.Concat(WatchFolder, RelativeFolderLocal, _
                                                  strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1)), _
                                    FileAttributes.Normal)
                            End If
                            ftp.Download(strLastFileActual, _
                                         String.Concat(WatchFolder, RelativeFolderLocal, strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1)), True)
                            OperationCount += 1
                        Catch ex As Exception
                            Log.Warn(String.Format("{0} to {1} ({2})", strLastFileActual, _
                                              String.Concat(WatchFolder, RelativeFolderLocal, strLastFile.Substring(strLastFile.LastIndexOf("/", StringComparison.Ordinal) + 1)), ex.Message), "Error downloading file:")
                        End Try
                    End If
                End If
            Else
                'Non versioning copy
                Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(Directory, False)
                Dim datFileModified As DateTime
                For Each ftpFile As FTPfileInfo In ftpList
                    datFileModified = ftpFile.FileDateTime
                    If datFileModified <= MaxFileDate Then
                        Try
                            Log.Debug(ftpFile.FullName & " " & _
                                String.Concat(WatchFolder, RelativeFolderLocal, ftpFile.Filename))
                            If Filesystem.File.Exists(String.Concat(WatchFolder, RelativeFolderLocal, ftpFile.Filename)) Then
                                Filesystem.File.SetAttributes(String.Concat(WatchFolder, RelativeFolderLocal, ftpFile.Filename), FileAttributes.Normal)
                            End If
                            ftp.Download(ftpFile.FullName, String.Concat(WatchFolder, RelativeFolderLocal, ftpFile.Filename), True)
                            OperationCount += 1
                        Catch ex As Exception
                            Log.Warn(String.Format("{0} ({1})", ftpFile.FullName, ex.Message), "Error downloading file:")
                        End Try
                    End If
                Next
            End If
            'Recurse directories
            If SubFolders Then
                For Each ftpFile As FTPfileInfo In ftp.ListDirectoryDetail(Directory)
                    If ftpFile.FileType = FTPfileInfo.DirectoryEntryTypes.Directory Then
                        'Application.DoEvents()
                        RestoreAllFTP(ftpFile.FullName, MaxFileDate)
                    End If
                Next
            End If
        Catch ex As Exception
            Log.Warn(String.Concat(ex.Message, " # Source:", Directory, ", Target:", BackupFolder, RelativeFolder), _
                     "RestoreAllFTP:")
        End Try
    End Sub

#End Region
End Class

