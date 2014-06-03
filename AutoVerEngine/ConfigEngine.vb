Imports System.Management
Imports System.Threading
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Windows.Forms
Imports System.ServiceModel
Imports System.Linq
Imports Microsoft.Win32
'Imports ZetaLongPaths
Imports Alphaleonis.Win32

<ServiceBehavior(IncludeExceptionDetailInFaults:=True, ConcurrencyMode:=ConcurrencyMode.Single, InstanceContextMode:=InstanceContextMode.Single)> _
Public Class ConfigEngine
    'Inherits MarshalByRefObject  
    Implements IConfigEngine
    Private _IsService As Boolean
    Private _LastMessageText As String
    Private _WatcherConfig As DataTable
    Private _AppConfig As New Generic.Dictionary(Of String, String)
    Private _ConfigFolder, _AppConfigFolder As String
    Private WithEvents _WatcherEngines As New List(Of BackupEngine)
    'Private _LastSelectedWatcher As Guid 'not used?
    'Public AutoVerGUI As Object 'not used?
    Public Log As Logger 'serialisable
    Private WithEvents m_MediaConnectWatcher As ManagementEventWatcher
    Public ArchiveDeleteTimer As New Threading.Timer(AddressOf ArchiveDelete_Tick, Nothing, 180000, System.Threading.Timeout.Infinite) '3min
    Public DrivePollTimer As Threading.Timer 'Not WMI mode
    Private FirstRun As Boolean = True 'First run of this startup
    Private Const AutoVerIni As String = "AutoVer.ini"
    Public CommonFolder, UserFolder, LocalFolder, PFx86 As String
    Public IsMockWatchers As Boolean 'If the watchers are loaded in mock mode (AutoVer Service is running the live copy)
    Public Lang As Language

#Region " Properties "
    'Convert value types to references so Remoting can address them
    Public Event UserMesage(ByVal MessageText As String)

    Public Property IsService() As Boolean
        Get
            Return _IsService
        End Get
        Set(ByVal value As Boolean)
            _IsService = value
        End Set
    End Property

    Public Property WatcherConfig() As DataTable
        Get
            Return _WatcherConfig
        End Get
        Set(ByVal value As DataTable)
            _WatcherConfig = value
        End Set
    End Property

    Public Property AppConfig() As Generic.Dictionary(Of String, String)
        Get
            Return _AppConfig
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, String))
            _AppConfig = value
        End Set
    End Property

    Public Property AppConfigFolder() As String
        Get
            Return _AppConfigFolder
        End Get
        Set(ByVal value As String)
            _AppConfigFolder = value
        End Set
    End Property

    Public Property ConfigFolder() As String
        Get
            Return _ConfigFolder
        End Get
        Set(ByVal value As String)
            _ConfigFolder = value
        End Set
    End Property

    Public Property WatcherEngines() As List(Of BackupEngine)
        Get
            Return _WatcherEngines
        End Get
        Set(ByVal value As List(Of BackupEngine))
            _WatcherEngines = value
        End Set
    End Property

    'Public Property LastSelectedWatcher() As Guid
    '    Get
    '        Return Guid.Empty 'LastSelectedWatcher
    '    End Get
    '    Set(ByVal value As Guid)
    '        '_LastSelectedWatcher = value
    '    End Set
    'End Property

    'Fulfil the contract for WCF 
    Public Function GetUserMesage() As String Implements IConfigEngine.GetUserMesage
        Return _LastMessageText
    End Function

    Public Function GetIsService() As Boolean Implements IConfigEngine.GetIsService
        Return _IsService
    End Function
    'Public Sub SetIsService(isService As Boolean) Implements IConfigEngine.SetIsService
    '    _IsService = isService
    'End Sub

    'Public Function GetWatcherConfig() As DataTable Implements IConfigEngine.GetWatcherConfig
    '    Return _WatcherConfig
    'End Function

    Public Function GetAppConfig() As Generic.Dictionary(Of String, String) Implements IConfigEngine.GetAppConfig
        Return _AppConfig
    End Function

    Public Function GetAppConfigFolder() As String Implements IConfigEngine.GetAppConfigFolder
        Return _AppConfigFolder
    End Function

    Public Function GetConfigFolder() As String Implements IConfigEngine.GetConfigFolder
        Return _ConfigFolder
    End Function

    'Public Function GetWatcherEngines() As List(Of BackupEngine) Implements IConfigEngine.GetWatcherEngines
    '    Return _WatcherEngines
    'End Function

    'Public Function GetLastSelectedWatcher() As GUID Implements IConfigEngine.GetLastSelectedWatcher
    '    Return _LastSelectedWatcher
    'End Function

    Public Function GetWatcherStatus() As List(Of WatcherStatus) Implements IConfigEngine.GetWatcherStatus
        Dim ws As New List(Of WatcherStatus)
        For Each watchEngine As BackupEngine In WatcherEngines
            Dim wsi As New WatcherStatus() With {.Name = watchEngine.Name, .WatcherId = watchEngine.GUID}
            wsi.TotalEvents = watchEngine.CountChanged + watchEngine.CountRenamed + watchEngine.CountDeleted
            wsi.Statistics = watchEngine.Stats
            wsi.Enabled = watchEngine.Enabled
            wsi.Started = watchEngine.Started
            wsi.Paused = watchEngine.Paused
            wsi.UserMessage = watchEngine.Message
            wsi.BackupFolderFailure = watchEngine.BackupFolderFail
            ws.Add(wsi)
        Next
        Return ws
    End Function

    Public Sub ReloadWatcher(ByVal watcherId As Guid) Implements IConfigEngine.ReloadWatcher
        LoadWatcherConfig()
        'Future versions may need individual watcher reload. Currently the config table flags what needs to be be load/reloaded
        'If watcherId = Guid.Empty Then
        '    'All watchers
        '    LoadWatcherConfig()
        'Else
        '    'One watcher
        '    For Each watchEngine As BackupEngine In WatcherEngines
        '        If watcherId = watchEngine.GUID Then

        '            Exit For
        '        End If
        '    Next
        'End If
    End Sub

    Public Sub DeleteWatcher(ByVal watcherId As Guid) Implements IConfigEngine.DeleteWatcher
        For intIndex As Int16 = 0 To WatcherEngines.Count - 1
            If WatcherEngines(intIndex).GUID = watcherId Then
                WatcherEngines(intIndex).StopWatching()
                Log.Info(WatcherEngines(intIndex).Name, "DeleteWatcher")
                WatcherEngines.RemoveAt(intIndex)
                Exit For
            End If
        Next
        For intRow As Int16 = 0 To WatcherConfig.Rows.Count - 1
            If watcherId = WatcherConfig.Rows(intRow)("GUID") Then
                'Log.Info(WatcherConfig.Rows(intRow)("WatchFolder"), "DeleteWatcher")
                WatcherConfig.Rows.RemoveAt(intRow)
                Exit For
            End If
        Next
        SaveWatcherConfig()
    End Sub

    Public Sub PauseWatcher(ByVal watcherId As Guid) Implements IConfigEngine.PauseWatcher
        If watcherId = Guid.Empty Then
            'All watchers
            'Find majority pause state so we can toggle
            Dim watchersPaused As Integer
            Dim pausing As Boolean
            For Each watchEngine As BackupEngine In WatcherEngines
                If watchEngine.Started Then watchersPaused += 1
            Next
            If watchersPaused >= WatcherEngines.Count Then pausing = True
            If pausing Then
                Log.Info("(ALL)", "PauseWatcher")
            Else
                Log.Info("(ALL)", "UnPauseWatcher")
            End If
            For Each watchEngine As BackupEngine In WatcherEngines
                If pausing Then
                    If watchEngine.Started Then watchEngine.StopWatching()
                    watchEngine.Paused = True
                Else
                    If watchEngine.Enabled Then watchEngine.SetupWatcher()
                    watchEngine.Paused = False
                End If
            Next
        Else
            'One watcher
            For Each watchEngine As BackupEngine In WatcherEngines
                If watcherId = watchEngine.GUID Then
                    If watchEngine.Started Then
                        watchEngine.StopWatching()
                        watchEngine.Paused = True
                        Log.Info(watchEngine.Name, "PauseWatcher")
                    ElseIf watchEngine.Enabled Then
                        watchEngine.SetupWatcher()
                        watchEngine.Paused = False
                        Log.Info(watchEngine.Name, "UnPauseWatcher")
                    End If
                    Exit For
                End If
            Next
        End If
    End Sub
#End Region

    Public Sub New()
        Try
            CommonFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\AutoVer\" 'Vista+: C:\ProgramData\; WinXP: C:\Documents and Settings\All Users\Application Data\
            UserFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\AutoVer\" 'C:\Users\{user login}\AppData\Roaming\ WinXP: C:\Documents and Settings\{user login}\UserData\
            LocalFolder = New FileInfo(Assembly.GetExecutingAssembly.Location).Directory.FullName & "\" 'C:\Program Files\AutoVer
            PFx86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") & "\"
            If String.IsNullOrEmpty(PFx86) Then PFx86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\"

            If (LocalFolder.ToLower.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).ToLower) Or LocalFolder.ToLower.StartsWith(PFx86.ToLower)) Then
                _ConfigFolder = CommonFolder
                Log = New Logger("INFO", CommonFolder)
            Else
                'Running from non-standard folder. Store config locally
                _ConfigFolder = LocalFolder
                Log = New Logger("INFO", LocalFolder)
                If Not Log.IsLogWritable() Then
                    _ConfigFolder = CommonFolder
                    Log = New Logger("INFO", CommonFolder)
                    Log.Warn("Non standard folder locally, but folder is not writable. Swapping back to CommonApplicationData", "ConfigStart")
                End If
            End If

            If Not Log.IsLogWritable() Then
                _ConfigFolder = UserFolder
                Log = New Logger("INFO", UserFolder)
                Log.Warn("CommonApplicationData is not writable. Swapping back to Roaming user folder", "ConfigStart")
            End If

            _AppConfigFolder = _ConfigFolder
            If Not Directory.Exists(_ConfigFolder) Then Directory.CreateDirectory(_ConfigFolder)
            Lang = New Language(Log)
        Catch ex As Exception
            Log = New Logger("INFO", _ConfigFolder)
            Log.Error(ex.Message, "Initialising config location")
            Lang = New Language(Log)
        End Try
    End Sub

    ''' <summary>
    ''' Load the common program settings from AutoVer.ini
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadAppConfig() Implements IConfigEngine.LoadAppConfig
        'AutoVer.ini is kept in the Common Data folder unless it is in local/portable mode
        Dim strFilePath As String = _AppConfigFolder & AutoVerIni
        'Defaults
        _AppConfig("LogLevel") = "INFO"
        _AppConfig("WMI") = "1"
        _AppConfig("RecycleBin") = "0"
        _AppConfig("LastArchive") = Now.AddYears(-1)
        _AppConfig("ServicePort") = "9072" 'v2.0
        _AppConfig("AutoElevate") = "0" 'v2.0
        _AppConfig("ConfigFolder") = "COMMON" 'v2.1"
        _AppConfig("Language") = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName 'v2.3
        _AppConfig("EmailOnError") = "0" 'v2.2
        _AppConfig("Email.To") = ""
        _AppConfig("Email.From") = ""
        _AppConfig("Email.Method") = "Network"
        _AppConfig("Email.PickupFolder") = "C:\inetpub\mailroot\Pickup"
        _AppConfig("Email.Host") = "smtp.gmail.com"
        _AppConfig("Email.Port") = "587"
        _AppConfig("Email.Ssl") = "1"
        _AppConfig("Email.UserName") = ""
        _AppConfig("Email.Password") = ""

        If File.Exists(strFilePath) Then
            Try
                FileUtils.ReadINI(strFilePath, _AppConfig)
            Catch ex As Exception
                Log.Warn(ex.Message, "AutoVer.ini read error")
            End Try

            If Not _AppConfig.ContainsKey("LogLevel") Then _AppConfig("LogLevel") = "INFO"
            If Not _AppConfig.ContainsKey("ConfigFolder") Then _AppConfig("ConfigFolder") = "COMMON" 'v2.1
        Else
            'No program settings ini. New install or Try to copy from registry settings of older versions (< v2)
            'UseService = 0 'ignore service
            Try
                Const strHkCu As String = "HKEY_CURRENT_USER\Software\AutoVer"
                _AppConfig("WMI") = Microsoft.Win32.Registry.GetValue(strHkCu, "WMI", "1").ToString
                _AppConfig("LastArchive") = Microsoft.Win32.Registry.GetValue(strHkCu, "LastArchive", Now.ToString).ToString
                _AppConfig("CompareApp") = Microsoft.Win32.Registry.GetValue(strHkCu, "CompareApp", PFx86 & "WinMerge\WinMergeU.exe ""{0}"" ""{1}""").ToString
                If File.Exists(PFx86 & "RJ TextEd\TextEd.exe") Then
                    _AppConfig("TextViewer") = Microsoft.Win32.Registry.GetValue(strHkCu, "TextViewer", PFx86 & "RJ TextEd\TextEd.exe").ToString
                ElseIf File.Exists(PFx86 & "Notepad++\notepad++.exe") Then
                    _AppConfig("TextViewer") = Microsoft.Win32.Registry.GetValue(strHkCu, "TextViewer", PFx86 & "Notepad++\notepad++.exe").ToString
                Else
                    _AppConfig("TextViewer") = Microsoft.Win32.Registry.GetValue(strHkCu, "TextViewer", "Notepad.exe").ToString
                End If
                If File.Exists(PFx86 & "FastStone Image Viewer\FSViewer.exe") Then
                    _AppConfig("ImageViewer") = Microsoft.Win32.Registry.GetValue(strHkCu, "ImageViewer", PFx86 & "FastStone Image Viewer\FSViewer.exe").ToString
                ElseIf File.Exists(PFx86 & "IrfanView\i_view32.exe") Then
                    _AppConfig("ImageViewer") = Microsoft.Win32.Registry.GetValue(strHkCu, "ImageViewer", PFx86 & "IrfanView\i_view32.exe").ToString
                Else
                    _AppConfig("ImageViewer") = Microsoft.Win32.Registry.GetValue(strHkCu, "ImageViewer", "MSPaint.exe").ToString
                End If
                _AppConfig("RecycleBin") = Microsoft.Win32.Registry.GetValue(strHkCu, "RecycleBin", "0").ToString
            Catch
            End Try
        End If

        CheckConfigFolder()
        Log.UpdateLogLevel(_AppConfig("LogLevel"), _AppConfigFolder)
        Log.Info("AutoVer.ini", "Config Loaded")
        Lang.ReadLanguageFile(_AppConfigFolder, AppConfigDefault("Language", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName))
        If Not File.Exists(_AppConfigFolder & AutoVerIni) Then SaveAppConfig()
    End Sub

    Private Sub CheckConfigFolder()
        Select Case _AppConfig("ConfigFolder")
            Case "COMMON"
                _ConfigFolder = CommonFolder
                _AppConfigFolder = CommonFolder
            Case "USER"
                _ConfigFolder = UserFolder
                _AppConfigFolder = CommonFolder
            Case "LOCAL"
                _ConfigFolder = LocalFolder
                If _AppConfigFolder <> LocalFolder Then
                    'Changing to local. Test if we can write the logs there
                    Log = New Logger(_AppConfig("LogLevel"), LocalFolder)
                    If Log.IsLogWritable() Then
                        _AppConfigFolder = LocalFolder
                    Else
                        _AppConfigFolder = CommonFolder
                        Log = New Logger("INFO", _AppConfigFolder)
                        Log.Warn("AutoVer.ini exists locally, but folder is not writable. Swapping back to Common config", "ConfigStart")
                    End If
                End If
        End Select
        If _AppConfigFolder = LocalFolder Then
            'Could be multiple instances - ie dev. comment out for now
            ''rename the common default config so it is not confused
            'If File.Exists(CommonFolder & AutoVerIni) Then
            '    Try
            '        If File.Exists(CommonFolder & AutoVerIni & ".bak") Then
            '            File.Delete(CommonFolder & AutoVerIni & ".bak")
            '        End If
            '        File.Move(CommonFolder & AutoVerIni, CommonFolder & AutoVerIni & ".bak")
            '    Catch ex As Exception
            '        Log.Error(ex.Message, "Renaming old common AutoVer.ini")
            '    End Try
            'End If
        Else
            'rename the local config so it is not confused
            If File.Exists(LocalFolder & AutoVerIni) Then
                Try
                    If File.Exists(LocalFolder & AutoVerIni & ".bak") Then
                        File.Delete(LocalFolder & AutoVerIni & ".bak")
                    End If
                    File.Move(LocalFolder & AutoVerIni, LocalFolder & AutoVerIni & ".bak")
                Catch ex As Exception
                    Log.Error(ex.Message, "Renaming old local AutoVer.ini")
                End Try
            End If
        End If

        Try
            If Not Directory.Exists(_ConfigFolder) Then Directory.CreateDirectory(_ConfigFolder)
            If Not Directory.Exists(_AppConfigFolder) Then Directory.CreateDirectory(_AppConfigFolder)
        Catch ex As Exception
            Log.Error(ex.Message, "Creating ConfigFolder:" & _AppConfig("ConfigFolder"))
        End Try
    End Sub

    ''' <summary>
    ''' Save the common program settings to AutoVer.ini and alter the live environment
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SaveAppConfig() Implements IConfigEngine.SaveAppConfig
        CheckConfigFolder()
        Log.UpdateLogLevel(_AppConfig("LogLevel"), _AppConfigFolder)

        Try
            FileUtils.WriteINI(_AppConfigFolder & AutoVerIni, _AppConfig, "AutoVer program settings")
        Catch ex As Exception
            Log.Warn(ex.Message, "AutoVer.ini write error")
        End Try

        'Reconfig application
        Dim blnRecBin As Boolean = _AppConfig("RecycleBin") = "1"
        For Each watcherEngine As BackupEngine In _WatcherEngines
            watcherEngine.UseRecycleBin = blnRecBin
        Next

        If _AppConfig("WMI") = "1" And IsNothing(m_MediaConnectWatcher) Then
            Try
                Const query2 As String = "SELECT * FROM __InstanceOperationEvent WITHIN 10 WHERE TargetInstance ISA ""Win32_LogicalDisk"""
                ' AND TargetInstance.Name <> ""A:"" 'Win32_DiskDrive"""  'Win32_LogicalDisk
                m_MediaConnectWatcher = New ManagementEventWatcher(query2)
                m_MediaConnectWatcher.Start()
            Catch ex As Exception
                Log.Error(ex.Message, "Removable drive watcher")
            End Try
        ElseIf _AppConfig("WMI") = "0" And Not IsNothing(m_MediaConnectWatcher) Then
            Try
                m_MediaConnectWatcher.Stop()
            Catch ex As Exception
                Log.Error(ex.Message, "Removable drive watcher")
            End Try
        End If
        If _AppConfig("WMI") = "0" Then
            DrivePollTimer = New Threading.Timer(AddressOf DriveEventPolling, Nothing, 120000, 120000) 'Polling, not WMI mode. Every 2 min
        End If
    End Sub

    ''' <summary>
    ''' Return the _AppConfig value or if it does not exist/is empty, return the default value
    ''' </summary>
    ''' <param name="Key">_AppConfig setting</param>
    ''' <param name="DefaultValue">Default Value if key does not exist/is empty </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AppConfigDefault(ByVal Key As String, ByVal DefaultValue As String) As String Implements IConfigEngine.AppConfigDefault
        If Not _AppConfig.ContainsKey(Key) OrElse String.IsNullOrEmpty(_AppConfig(Key)) Then
            Return DefaultValue
        Else
            Return _AppConfig(Key)
        End If
    End Function

    Private Sub CreateWatcherConfig()
        'Create our waatcher config data structure
        _WatcherConfig = New DataTable("WatcherConfig")
        _WatcherConfig.Columns.Add("GUID", GetType(Guid)) 'ver 1.2
        _WatcherConfig.Columns.Add("Name", GetType(System.String)) 'ver 1.2
        _WatcherConfig.Columns.Add("State", GetType(System.String)) 'ver 1.2
        _WatcherConfig.Columns.Add("WatchFolder", GetType(System.String))
        _WatcherConfig.Columns.Add("BackupFolder", GetType(System.String))
        _WatcherConfig.Columns.Add("DeleteOnDelete", GetType(System.Boolean))
        _WatcherConfig.Columns.Add("IncludeFiles", GetType(System.String))
        _WatcherConfig.Columns.Add("ExcludeFiles", GetType(System.String))
        _WatcherConfig.Columns.Add("ExcludeFolders", GetType(System.String))
        _WatcherConfig.Columns.Add("VersionFiles", GetType(System.Boolean))
        _WatcherConfig.Columns.Add("MaxVersionAge", GetType(UInt32)) 'ver 1.1
        _WatcherConfig.Columns.Add("MaxVersionAction", GetType(System.Char)) 'ver 1.1
        _WatcherConfig.Columns.Add("MaxFileSize", GetType(UInt64)) 'ver 1.1
        _WatcherConfig.Columns.Add("SubFolders", GetType(System.Boolean)) 'ver 1.1
        '_WatcherConfig.Columns.Add("EnsureOnStart", GetType(System.Boolean)) 'ver 1.1
        _WatcherConfig.Columns.Add("DateTimeStamp", GetType(System.String)) 'ver 1.2
        _WatcherConfig.Columns.Add("Enabled", GetType(System.Boolean)) 'ver 1.3
        _WatcherConfig.Columns.Add("FTPEnable", GetType(System.Boolean)) 'ver 1.3
        _WatcherConfig.Columns.Add("FTPHost", GetType(System.String)) 'ver 1.3
        _WatcherConfig.Columns.Add("FTPUser", GetType(System.String)) 'ver 1.3
        _WatcherConfig.Columns.Add("FTPPass", GetType(System.String)) 'ver 1.3
        _WatcherConfig.Columns.Add("FTPPassive", GetType(System.Boolean)) 'ver 1.3
        _WatcherConfig.Columns.Add("VersionPrevFiles", GetType(System.Boolean)) 'ver 1.3
        _WatcherConfig.Columns.Add("ShowErrors", GetType(System.Boolean)) 'ver 1.4
        _WatcherConfig.Columns.Add("EnsureSchedule", GetType(System.String)) 'ver 1.4
        _WatcherConfig.Columns.Add("VersionRate", GetType(Int32)) 'ver 1.4
        _WatcherConfig.Columns.Add("SettleDelay", GetType(System.Decimal)) 'ver 1.4
        _WatcherConfig.Columns.Add("RunCopy", GetType(System.String)) 'ver 1.4
        _WatcherConfig.Columns.Add("RunCopyFirst", GetType(System.Boolean)) 'ver 1.4
        _WatcherConfig.Columns.Add("CompareBeforeCopy", GetType(System.Boolean)) 'ver 2.0
        _WatcherConfig.Columns.Add("ShowEvents", GetType(System.Boolean)) 'ver 2.1
        _WatcherConfig.Columns.Add("ZipMode", GetType(System.Char)) 'ver 2.2
    End Sub

    ''' <summary>
    ''' Load Watcher config from AutoVer.xml
    ''' </summary>
    ''' <remarks></remarks>
    Sub LoadWatcherConfig() Implements IConfigEngine.LoadWatcherConfig
        Dim dataSet As DataSet = New DataSet
        CreateWatcherConfig()
        dataSet.Tables.Add(_WatcherConfig)

        Dim strFilePath As String = _ConfigFolder & "AutoVer.xml"
        If Not File.Exists(strFilePath) Then
            'No watcher config, try to find it from older autover versions (< v2)
            If File.Exists(UserFolder & "AutoVer.xml") Then
                File.Copy(UserFolder & "AutoVer.xml", strFilePath, True)
            ElseIf File.Exists(UserFolder & "AutoVer.Config.xml") Then
                File.Copy(UserFolder & "AutoVer.Config.xml", strFilePath, True)
            ElseIf File.Exists(LocalFolder & "AutoVer.Config.xml") Then
                File.Copy(LocalFolder & "AutoVer.Config.xml", strFilePath, True)
            End If
        End If
        Try
            dataSet.ReadXml(_ConfigFolder & "AutoVer.xml")
            Log.Info("AutoVer.xml", "Config Loaded")
        Catch ex As Exception
            'Probably first time running
            'Me.ShowInTaskbar = True
            'Me.WindowState = FormWindowState.Normal
            Log.Warn(ex.Message, "Error loading config: ")
        End Try
        'Upgrade to latest format
        For Each drv As DataRow In _WatcherConfig.Rows
            If IsDBNull(drv("GUID")) Then drv("GUID") = Guid.NewGuid
            If IsDBNull(drv("Name")) Then drv("Name") = drv("WatchFolder")
            If IsDBNull(drv("Enabled")) Then drv("Enabled") = True
            If IsDBNull(drv("State")) Then drv("State") = String.Empty
            If IsDBNull(drv("SubFolders")) Then drv("SubFolders") = True
            If IsDBNull(drv("MaxFileSize")) Then drv("MaxFileSize") = 1000000000 '1G
            If IsDBNull(drv("MaxVersionAge")) Then drv("MaxVersionAge") = 90
            If IsDBNull(drv("MaxVersionAction")) Then drv("MaxVersionAction") = "N"c
            'If IsDBNull(drv("EnsureOnStart")) Then drv("EnsureOnStart") = False
            If IsDBNull(drv("DateTimeStamp")) Then drv("DateTimeStamp") = "yyMMddHHmmss"
            If IsDBNull(drv("FTPEnable")) Then drv("FTPEnable") = False
            If IsDBNull(drv("FTPHost")) Then drv("FTPHost") = String.Empty
            If IsDBNull(drv("FTPUser")) Then drv("FTPUser") = String.Empty
            If IsDBNull(drv("FTPPass")) Then drv("FTPPass") = String.Empty
            'If drv("FTPPass") <> String.Empty Then drv("FTPPass") = General.FromHex(drv("FTPPass"))
            If IsDBNull(drv("FTPPassive")) Then drv("FTPPassive") = True
            If IsDBNull(drv("VersionPrevFiles")) Then drv("VersionPrevFiles") = False
            If IsDBNull(drv("ShowErrors")) Then drv("ShowErrors") = False
            If IsDBNull(drv("EnsureSchedule")) Then drv("EnsureSchedule") = "D"
            If IsDBNull(drv("VersionRate")) Then drv("VersionRate") = 0
            If IsDBNull(drv("SettleDelay")) Then drv("SettleDelay") = 1000
            If IsDBNull(drv("RunCopy")) Then drv("RunCopy") = String.Empty
            If IsDBNull(drv("RunCopyFirst")) Then drv("RunCopyFirst") = True
            If IsDBNull(drv("CompareBeforeCopy")) Then drv("CompareBeforeCopy") = False
            If IsDBNull(drv("ShowEvents")) Then drv("ShowEvents") = False
            If IsDBNull(drv("ZipMode")) Then drv("ZipMode") = "W"c
        Next
        If Log.LogLevel < Logger.LogType.Info Then Log.Debug(General.DataTableToText(_WatcherConfig), "_WatcherConfig") '1st load only

        If Not IsMockWatchers Then
            If AppConfigDefault("WMI", "1") = "1" Then 'Not System.IO.DriveInfo.GetDrives(0).Name.ToUpper.StartsWith("A") Then
                'WMI polls floppies every 10 sec causing it to make a noise. Avoid this.
                If IsNothing(m_MediaConnectWatcher) Then
                    Try
                        '    Dim query2 As String = "SELECT * FROM __InstanceOperationEvent WITHIN 10 WHERE TargetInstance ISA ""Win32_DiskDrive"""  'Win32_LogicalDisk
                        Const query2 As String = "SELECT * FROM __InstanceOperationEvent WITHIN 10 WHERE TargetInstance ISA ""Win32_LogicalDisk"""
                        ' AND TargetInstance.Name <> ""A:"" 'Win32_DiskDrive"""  'Win32_LogicalDisk
                        m_MediaConnectWatcher = New ManagementEventWatcher(query2)
                        m_MediaConnectWatcher.Start()
                    Catch ex As Exception
                        Log.Error(ex.Message, "Removable drive watcher")
                    End Try
                End If
            ElseIf IsNothing(DrivePollTimer) Then
                DrivePollTimer = New Threading.Timer(AddressOf DriveEventPolling, Nothing, 120000, 120000) 'Polling, not WMI mode. Every 2 min
            End If
        End If

        UpdateWatchersList()
    End Sub

    '''' <summary>
    '''' Update a watcher into the _WatcherConfig. Muts be a deep copy of values
    '''' </summary>
    '/''' <param name="aryWatcher">Coloumns of datarow (serialisable)</param>
    '''' <remarks></remarks>
    'Public Sub UpdateWatcher(ByVal aryWatcher() As Object)
    '    Dim blnIsNew As Boolean
    '    If aryWatcher(0) = Guid.Empty Then
    '        aryWatcher(0) = System.Guid.NewGuid
    '        Dim drWatcher As DataRow = _WatcherConfig.NewRow
    '        drWatcher("GUID") = aryWatcher(0)
    '        _WatcherConfig.Rows.Add(drWatcher)
    '        blnIsNew = True
    '    End If

    '    For intRow As Int16 = 0 To _WatcherConfig.Rows.Count - 1
    '        If aryWatcher(0) = _WatcherConfig.Rows(intRow)("GUID") Then
    '            For intCol As Integer = 0 To _WatcherConfig.Columns.Count - 1
    '                _WatcherConfig.Rows(intRow)(intCol) = aryWatcher(intCol)
    '            Next
    '            If blnIsNew Then
    '                _WatcherConfig.Rows(intRow)("State") = "N"
    '            Else
    '                _WatcherConfig.Rows(intRow)("State") = "U"
    '            End If
    '            Exit For
    '        End If
    '    Next
    'End Sub

    ''' <summary>
    ''' Save Watcher config to AutoVer.xml
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SaveWatcherConfig() Implements IConfigEngine.SaveWatcherConfig
        'Save to xml file
        Dim dtTemp As DataTable = _WatcherConfig.Clone()
        dtTemp.TableName = "WatcherConfig"
        For Each drv As DataRow In _WatcherConfig.Rows
            dtTemp.ImportRow(drv)
        Next
        Dim dsTemp As DataSet = New DataSet("AutoVer")
        dsTemp.Tables.Add(dtTemp)
        Try
            dsTemp.WriteXml(_ConfigFolder & "AutoVer.xml")
        Catch ex As Exception
            Log.Error(ex.Message, "SaveWatcherList")
        End Try
    End Sub

    'Private Delegate Sub UpdateWatchersListCallback()

    Public Sub UpdateWatchersList() Implements IConfigEngine.UpdateWatchersList
        'Update watch list
        Dim blnUpdateAll As Boolean = False
        If _WatcherEngines.Count = 0 Then blnUpdateAll = True
        'lvwWatches.Items.Clear()
        'Dim lvi As ListViewItem
        'Dim blnHasErrors, blnEnabled, blnBackupFail As Boolean
        'Log.Debug(_WatcherConfig.Rows(0)("WatchFolder"), "UpdateWatchersList")
        For Each drWatch As DataRow In _WatcherConfig.Rows
            'Update engines
            'blnHasErrors = False
            If blnUpdateAll Or drWatch("State") = "N" Then
                'Adding new
                Log.Info(drWatch("Name") & ": " & drWatch("WatchFolder") & " -> " & IIf(drWatch("FTPEnable"), "(FTP) " & drWatch("FTPHost"), drWatch("BackupFolder")), "Watching")
                Dim watcherEngine As New BackupEngine(Log)
                UpdateWatchFromConfig(watcherEngine, drWatch)
                If Not IsMockWatchers Then
                    watcherEngine.SetupWatcher()
                    AddHandler watcherEngine.UserMessage, AddressOf ShowUserMessage
                End If
                'blnEnabled = WatcherEngine.Enabled
                'blnBackupFail = WatcherEngine.BackupFolderFail
                'If Not WatcherEngine.Started And WatcherEngine.Enabled Then blnHasErrors = True
                _WatcherEngines.Add(watcherEngine)
            ElseIf drWatch("State") = "U" Then
                'Changing existing
                Log.Debug(drWatch("WatchFolder"), "Watching(Updated)")
                For Each watcherEngine As BackupEngine In _WatcherEngines
                    If watcherEngine.GUID = drWatch("GUID") Then
                        If Not IsMockWatchers AndAlso watcherEngine.Started Then watcherEngine.StopWatching()
                        UpdateWatchFromConfig(watcherEngine, drWatch)
                        If Not IsMockWatchers Then
                            watcherEngine.SetupWatcher()
                            AddHandler watcherEngine.UserMessage, AddressOf ShowUserMessage
                        End If
                        'blnEnabled = WatcherEngine.Enabled
                        'blnBackupFail = WatcherEngine.BackupFolderFail
                        'If Not WatcherEngine.Started And WatcherEngine.Enabled Then blnHasErrors = True
                    End If
                Next
            Else
                Log.Debug(drWatch("WatchFolder"), "Watching(no change)")
                'For Each watchEngine As BackupEngine In _WatcherEngines
                'If watchEngine.GUID = drWatch("GUID") Then
                'blnEnabled = WatchEngine.Enabled
                'blnBackupFail = WatchEngine.BackupFolderFail
                'If Not WatchEngine.Started And WatchEngine.Enabled Then blnHasErrors = True
                'End If
                'Next
            End If
            'lvi = New ListViewItem
            'lvi.Tag = drWatch("GUID")
            'lvi.Text = drWatch("Name")
            'If blnHasErrors Then
            '    lvi.Text = "!!! " & lvi.Text & " - Error!"
            'ElseIf blnBackupFail Then
            '    lvi.Text = "!!! " & lvi.Text & " - File Copying Warnings!"
            'End If
            'If Not blnEnabled Then lvi.Text &= " (Disabled)"
            'lvwWatches.Items.Add(lvi)
            drWatch("State") = String.Empty
        Next
    End Sub

    Private Sub UpdateWatchFromConfig(ByRef WatcherEngine As BackupEngine, ByRef drWatch As DataRow)
        'Map the config datarow (from XML file) to the watcher entity
        WatcherEngine.GUID = drWatch("GUID")
        WatcherEngine.Name = drWatch("Name")
        WatcherEngine.Enabled = drWatch("Enabled")
        WatcherEngine.WatchFolder = drWatch("WatchFolder")
        WatcherEngine.BackupFolder = drWatch("BackupFolder")
        WatcherEngine.VersionFiles = drWatch("VersionFiles")
        WatcherEngine.VersionPrev = drWatch("VersionPrevFiles")
        WatcherEngine.IncludeFiles = drWatch("IncludeFiles")
        WatcherEngine.ExcludeFiles = drWatch("ExcludeFiles")
        WatcherEngine.ExcludeFolders = drWatch("ExcludeFolders")
        WatcherEngine.DeleteOnDelete = drWatch("DeleteOnDelete")
        WatcherEngine.MaxVersionAge = drWatch("MaxVersionAge")
        WatcherEngine.MaxVersionAction = drWatch("MaxVersionAction")
        WatcherEngine.MaxFileSize = drWatch("MaxFileSize")
        WatcherEngine.SubFolders = drWatch("SubFolders")
        'WatcherEngine.EnsureOnStart = drWatch("EnsureOnStart")
        WatcherEngine.DateTimeStamp = drWatch("DateTimeStamp")
        WatcherEngine.FTPEnable = drWatch("FTPEnable")
        WatcherEngine.FTPHost = drWatch("FTPHost")
        WatcherEngine.FTPUser = drWatch("FTPUser")
        WatcherEngine.FTPPass = General.FromHex(drWatch("FTPPass"))
        'Log.Debug(WatcherEngine.FTPPass, "FTPPass.Load")
        WatcherEngine.FTPPassive = drWatch("FTPPassive")
        WatcherEngine.ShowErrors = drWatch("ShowErrors")
        WatcherEngine.ShowEvents = drWatch("ShowEvents")
        WatcherEngine.EnsureSchedule = drWatch("EnsureSchedule")
        WatcherEngine.VersionRate = drWatch("VersionRate")
        WatcherEngine.SettleDelay = Convert.ToInt32(drWatch("SettleDelay") * 1000)
        WatcherEngine.RunCopy = drWatch("RunCopy")
        WatcherEngine.RunCopyFirst = drWatch("RunCopyFirst")
        WatcherEngine.CompareBeforeCopy = drWatch("CompareBeforeCopy")
        WatcherEngine.UseRecycleBin = AppConfigDefault("RecycleBin", False)
        WatcherEngine.ZipMode = drWatch("ZipMode")
        WatcherEngine.AppConfig = _AppConfig
    End Sub

    Private Sub DriveEventArrived(ByVal sender As Object, ByVal e As EventArrivedEventArgs) Handles m_MediaConnectWatcher.EventArrived
        'Using WMI, removable drive has been added/removed. Check if we need to Ensure the backup on that device
        Dim mbo, obj As ManagementBaseObject
        mbo = e.NewEvent
        obj = CType(mbo("TargetInstance"), ManagementBaseObject)
        'Dim blnWatchUpdated As Boolean = False
        Select Case mbo.ClassPath.ClassName
            Case "__InstanceCreationEvent"
                Log.Info(obj("Name") & " - " & WMIDriveType(obj("DriveType")), "DriveOnline")
                'If obj("DriveType") = "USB" Or obj("InterfaceType") = "1394" Then
                '    Dim strDrive As String = GetDriveLetterFromDisk(obj("Name"))
                '    RemovableDrives(obj("Name")) = strDrive
                '    Log.Debug(obj("DriveType") & " Insert=" & strDrive & " " & obj("Caption"))
                For Each watcherEngine As BackupEngine In _WatcherEngines
                    If watcherEngine.BackupFolder.ToUpper.StartsWith(obj("Name")) Or watcherEngine.WatchFolder.ToUpper.StartsWith(obj("Name")) Then
                        'restart watch and ensure.
                        If watcherEngine.EnsureOnStart Then
                            watcherEngine.EnsureBackupCurrent() '.BackgroungWorkerRun("ENSURE_SILENT")
                            ' WatcherEngine.EnsureBackupCurrent(Me.WindowState <> FormWindowState.Minimized) 'forces start if not already
                        ElseIf Not watcherEngine.Started Then
                            watcherEngine.SetupWatcher() 'start
                        End If
                        'blnWatchUpdated = True
                    End If
                Next
                'End If
            Case "__InstanceDeletionEvent"
                Log.Info(obj("Name") & " - " & WMIDriveType(obj("DriveType")), "DriveOffline")
                'UPDATE: Keep watching it may come back online very quickly
                'If obj("InterfaceType") = "USB" Or obj("InterfaceType") = "1394" Then
                '    Dim strDrive As String = obj("Name")
                '    If RemovableDrives.Contains(strDrive) Then
                '        strDrive = RemovableDrives(strDrive)
                '        RemovableDrives.Remove(strDrive)
                '    End If
                '    Log.Debug(obj("InterfaceType") & " Remove=" & strDrive & " " & obj("Caption"))
                'For Each WatcherEngine As BackupEngine In _WatcherEngines
                '    If WatcherEngine.BackupFolder.ToUpper.StartsWith(obj("Name")) Or WatcherEngine.WatchFolder.ToUpper.StartsWith(obj("Name")) Then
                '        'If Not IO.Directory.Exists(WatcherEngine.BackupFolder) Or Not IO.Directory.Exists(WatcherEngine.WatchFolder) Then
                '        WatcherEngine.StopWatching() 'stop watching. 
                '        blnWatchUpdated = True
                '    End If
                'Next
                'End If
        End Select
        'If blnWatchUpdated And Not IsNothing(AutoVerGUI) Then
        '    Dim cb As New UpdateWatchersListCallback(AddressOf AutoVerGUI.UpdateWatchersList)
        '    AutoVerGUI.Invoke(cb)
        'End If
    End Sub

    Private Function WMIDriveType(ByVal intType As Integer) As String
        'Windows Management Instrumentation Win32_LogicalDisk DriveType
        Select Case intType
            Case 1
                Return "No Root Directory"
            Case 2
                Return "Removable Disk"
            Case 3
                Return "Local Disk"
            Case 4
                Return "Network Drive"
            Case 5
                Return "Compact Disc"
            Case 6
                Return "RAM Disk"
            Case Else
                Return "Unknown"
        End Select
    End Function

    Private Sub DriveEventPolling(ByVal sender As Object)
        'Using Polling, not WMI, check if  removable drive has been added/removed. Check if we need to Ensure the backup on that device
        ' Dim blnWatchUpdated As Boolean = False
        'Log.Debug("Tick", "DriveEventPolling")
        'Dim DrivesOnline As New Generic.Dictionary(Of String, Boolean)
        Dim blnWatchOnline, blnBackupOnline As Boolean
        For Each watcherEngine As BackupEngine In _WatcherEngines
            If watcherEngine.Enabled Then
                blnWatchOnline = Filesystem.Directory.Exists(watcherEngine.WatchFolder)
                If watcherEngine.FTPEnable Then
                    blnBackupOnline = watcherEngine.FTPBackupFolderExists()
                Else
                    blnBackupOnline = Filesystem.Directory.Exists(watcherEngine.BackupFolder)
                End If
                If watcherEngine.WatchFailure Then
                    If blnWatchOnline And blnBackupOnline Then
                        'reconnected, so resync
                        Log.Info(watcherEngine.Name, "DrivePathOnline")
                        watcherEngine.WatchFailure = False
                        If watcherEngine.EnsureOnStart Then
                            watcherEngine.EnsureBackupCurrent()
                        ElseIf Not watcherEngine.Started Then
                            watcherEngine.SetupWatcher() 'start
                        End If
                        'blnWatchUpdated = True
                    End If
                Else
                    If Not blnWatchOnline Or Not blnBackupOnline Then
                        If Not blnWatchOnline Then Log.Info(watcherEngine.WatchFolder, "DrivePathOffline.Watch")
                        If Not blnBackupOnline Then Log.Info(watcherEngine.BackupFolder, "DrivePathOffline.Backup")
                        watcherEngine.WatchFailure = True
                    End If
                End If
            End If
        Next

        'If blnWatchUpdated And Not IsNothing(AutoVerGUI) Then
        '    Dim cb As New UpdateWatchersListCallback(AddressOf AutoVerGUI.UpdateWatchersList)
        '    AutoVerGUI.Invoke(cb)
        'End If
    End Sub

    ''This API Function will be used to get the UNC of the drive
    'Private Declare Function WNetGetConnection Lib "mpr.dll" Alias "WNetGetConnectionA" _
    '(ByVal lpszLocalName As String, ByVal lpszRemoteName As String, ByRef cbRemoteName As Int32) As Int32

    'Private Function IsDriveReady(ByVal di As DriveInfo) As Boolean
    '    If di.DriveType = IO.DriveType.Network Then
    '        'If the drive is a Network drive only, then ping it to see if it's ready.
    '        'Get the UNC (\\servername\share) for the drive letter returned by dri.Name
    '        Dim UNC As String = Space(100)
    '        WNetGetConnection(di.Name.Substring(0, 2), UNC, 100)
    '        'Presuming the drive is mapped \\servername\share Parse the servername out of the UNC
    '        Dim server As String = UNC.Trim().Substring(2, UNC.Trim().IndexOf("\", 2) - 2)
    '        Dim timeout As Integer = 2 'seconds
    '        Dim pingSender As New System.Net.NetworkInformation.Ping()
    '        Dim options As New System.Net.NetworkInformation.PingOptions()
    '        options.DontFragment = True
    '        Dim ipAddressOrHostName As String = server
    '        Dim data As String = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
    '        Dim buffer As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
    '        Dim reply As System.Net.NetworkInformation.PingReply = pingSender.Send(ipAddressOrHostName, timeout, buffer, options)
    '        If reply.Status = Net.NetworkInformation.IPStatus.Success Then Return True
    '        Log.Debug(reply.Status.ToString, "IsNetDriveReady=" & di.Name)
    '        'If reply.Status = Net.NetworkInformation.IPStatus.DestinationHostUnreachable Or reply.Status = Net.NetworkInformation.IPStatus.DestinationNetworkUnreachable Then Return di.IsReady 'Ping blocked?
    '        Return False
    '    Else
    '        Return di.IsReady
    '    End If
    'End Function

    'Private Function GetDriveLetterFromDisk(ByVal Name As String) As String
    '    'Used by DriveEventArrived
    '    Dim oq_part, oq_disk As ObjectQuery
    '    Dim mos_part, mos_disk As ManagementObjectSearcher
    '    Dim obj_part, obj_disk As ManagementObject
    '    Dim ans As String = String.Empty
    '    ' WMI queries use the "\" as an escape charcter
    '    Name = Replace(Name, "\", "\\")

    '    ' First we map the Win32_DiskDrive instance with the association called Win32_DiskDriveToDiskPartition.  
    '    ' Then we map the Win23_DiskPartion instance with the assocation called Win32_LogicalDiskToPartition
    '    Try

    '        oq_part = New ObjectQuery("ASSOCIATORS OF {Win32_DiskDrive.DeviceID=""" & Name & """} WHERE AssocClass = Win32_DiskDriveToDiskPartition")
    '        mos_part = New ManagementObjectSearcher(oq_part)
    '        For Each obj_part In mos_part.Get()
    '            oq_disk = New ObjectQuery("ASSOCIATORS OF {Win32_DiskPartition.DeviceID=""" & obj_part("DeviceID") & """} WHERE AssocClass = Win32_LogicalDiskToPartition")
    '            mos_disk = New ManagementObjectSearcher(oq_disk)
    '            For Each obj_disk In mos_disk.Get()
    '                ans &= obj_disk("Name") & ","
    '            Next
    '            mos_disk.Dispose()
    '        Next
    '        Return ans.Trim(","c)
    '    Catch ex As Exception
    '        Log.Error(ex.Message, "GetDriveLetterFromRemovableDisk")
    '        Return String.Empty
    '    End Try
    'End Function


    Private Sub ArchiveDelete_Tick(ByVal state As Object)
        'Run daily archive/delete (first tiggers 3 mins after start) only for watchers with Daily or Hourly schedules and no specific time set)
        'Dim blnStartup As Boolean = False
        Const archiveInterval As Integer = 24 * 60 * 60000 '1 day
        If FirstRun Then
            'Run after every app start
            FirstRun = False
            ArchiveDeleteTimer.Change(archiveInterval, archiveInterval)
            '    'Run once a day
            '    Try
            '        'HKEY_LOCAL_MACHINE
            '        strLastArchive = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "LastArchive", Now.AddDays(-1).ToString)
            '        Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\AutoVer", "LastArchive", Now.ToString)
            '    Catch ex As Exception
            '        Log.Error(ex.Message, "Can not get/set HKCU\Software\AutoVer in registry:")
            '    End Try
        End If
        Dim datLastArchive As DateTime
        If Not DateTime.TryParse(_AppConfig("LastArchive"), datLastArchive) Then
            _AppConfig("LastArchive") = Now.AddDays(-1).ToString
        End If

        'Log.Debug("tick", "tick")
        'For Each WatchEngine As BackupEngine In _WatcherEngines
        '    Log.Debug(WatchEngine.Name, WatchEngine.Started)
        'Next

        If DateTime.TryParse(_AppConfig("LastArchive"), datLastArchive) And SystemInformation.BootMode = Windows.Forms.BootMode.Normal Then
            If datLastArchive < Now.AddHours(-23).AddMinutes(-58) Then
                For Each WatchEngine In _WatcherEngines
                    If WatchEngine.Enabled Then
                        WatchEngine.DeleteArchiveVersions()
                        If FirstRun And WatchEngine.EnsureSchedule <> "N" And WatchEngine.EnsureSchedule.Length = 1 Then WatchEngine.EnsureBackupCurrent()
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub ShowUserMessage(ByVal Message As String)
        'Pass on the event
        _LastMessageText = Message
        RaiseEvent UserMesage(Message)
    End Sub
End Class
