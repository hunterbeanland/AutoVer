Imports System.IO
Imports System.Diagnostics

'v2.0.0 Enabled New; DebugVerbose; Use Application.Info.ProductName for filenames
'v2.1.0 Added DeleteLogs; ThreadID prefix now T
'v2.2.0 Removed Shared instance; LogLevel, New Public; Fixed DeleteLogs file names
'v2.2.1 DebugVerbose -> ByRef
'v2.2.2 LogLevel check for System.Diagnostics.Debug.WriteLine fix
'v2.3.0 Default Log folder now in ProgramData folder, default to Info level, but changable once config files loaded
'v2.3.1 Use Windows "Application" Event log to avoid service installer issues
'v2.3.2 Fix wrong LogLevel setting on New
'v2.4.0 Add LogFolder on New; Add IsLogWritable

<Serializable()> _
Public Class Logger
    Public Enum LogType
        None = 0
        DebugVerbose = 1
        Debug = 2
        Info = 3
        Warn = 4
        [Error] = 5
    End Enum
    Public Enum CategoryTypes
        None = 0
        ScriptError = 1
        SystemError = 2
        EmailOut = 3
        EmailIn = 4
        SchedulerJob = 5
        Processing = 6
        DataImport = 7
        DataExport = 8
        CodeUpdate = 9
        SQLUpdate = 10
        Login = 11
    End Enum
    Private ReadOnly LogFile As String = My.Application.Info.ProductName & "Log.txt"
    Public LogFilePath As String
    Public LogLevel As LogType = LogType.Debug

#Region "Overload List"
    'Maintain an interface similar to log4net where possible

    Public Overloads Sub [Error](ByVal Message As String)
        Write(LogType.Error, Message, String.Empty, 0)
    End Sub

    Public Overloads Sub [Error](ByVal Message As String, ByVal Category As String)
        Write(LogType.Error, Message, Category, 0)
    End Sub

    Public Overloads Sub [Error](ByVal Message As String, ByVal Category As CategoryTypes)
        Write(LogType.Error, Message, String.Empty, Category)
    End Sub

    Public Overloads Sub Warn(ByVal Message As String)
        Write(LogType.Warn, Message, String.Empty, 0)
    End Sub

    Public Overloads Sub Warn(ByVal Message As String, ByVal Category As String)
        Write(LogType.Warn, Message, Category, 0)
    End Sub

    Public Overloads Sub Warn(ByVal Message As String, ByVal Category As CategoryTypes)
        Write(LogType.Warn, Message, String.Empty, Category)
    End Sub

    Public Overloads Sub Info(ByVal Message As String)
        Write(LogType.Info, Message, String.Empty, 0)
    End Sub

    Public Overloads Sub Info(ByVal Message As String, ByVal Category As String)
        Write(LogType.Info, Message, Category, 0)
    End Sub

    Public Overloads Sub Info(ByVal Message As String, ByVal Category As CategoryTypes)
    End Sub

    Public Overloads Sub Debug(ByVal Message As String)
        Write(LogType.Debug, Message, String.Empty, 0)
    End Sub

    Public Overloads Sub Debug(ByVal Message As String, ByVal Category As String)
        Write(LogType.Debug, Message, Category, 0)
    End Sub

    Public Overloads Sub DebugVerbose(ByRef Message As String)
        Write(LogType.DebugVerbose, Message, String.Empty, 0)
    End Sub

    Public Overloads Sub DebugVerbose(ByRef Message As String, ByVal Category As String)
        Write(LogType.DebugVerbose, Message, Category, 0)
    End Sub
#End Region

    Public Sub New(Optional ByVal MinimumLogLevel As String = "INFO", Optional ByVal LogFolder As String = "")
        'Try
        UpdateLogLevel(MinimumLogLevel, LogFolder)
        'Write(LogType.Info, MinimumLogLevel, LogLevel.ToString, 0)

        'If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("Debug")) AndAlso System.Configuration.ConfigurationManager.AppSettings("Debug") Then LogLevel = LogType.Debug
        'If Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "Debug", 0) = 1 Then LogLevel = LogType.Debug
        'Catch
        'End Try
        'Dim aryCL() As String = Environment.GetCommandLineArgs()
        'LogFilePath = aryCL(0) 'Only command line (not env) contains the correct info when running as a service
        'If LogFilePath.IndexOf("\") > 0 Then
        '    LogFilePath = LogFilePath.Substring(0, LogFilePath.LastIndexOf("\") + 1) & LogFile
        'Else
        'LogFilePath = LogPath & LogFile
        ''Test write to local file, then if failure (Vista/Win7), App Data
        'LogFilePath = New IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly.Location).Directory.FullName & "\" & LogFile
        'Try
        '    Dim Fs As New FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
        '    Fs.Close()
        'Catch
        'End Try
        'End If
    End Sub

    ''' <summary>
    ''' Set a new log level and path
    ''' </summary>
    ''' <param name="MinimumLogLevel">Level, ie: INFO, DEBUG</param>
    ''' <param name="LogFolder">Folder path ending in \</param>
    ''' <remarks></remarks>
    Public Sub UpdateLogLevel(ByVal MinimumLogLevel As String, ByVal LogFolder As String)
        Select Case MinimumLogLevel.ToUpper()
            Case "NONE" : LogLevel = LogType.None
            Case "DEBUG" : LogLevel = LogType.Debug
            Case "DEBUGVERBOSE" : LogLevel = LogType.DebugVerbose
            Case "INFO" : LogLevel = LogType.Info
            Case "WARN" : LogLevel = LogType.Warn
            Case "ERROR" : LogLevel = LogType.Error
        End Select
        If LogFolder.Length = 0 Then
            LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\AutoVer\"
        Else
            LogFilePath = LogFolder
        End If
        Try
            If Not Directory.Exists(LogFilePath) Then Directory.CreateDirectory(LogFilePath)
        Catch
        End Try
        LogFilePath &= LogFile
        'End Try
    End Sub

    ''' <summary>
    ''' Write log info to a trace, application event log and log file
    ''' </summary>
    ''' <param name="LogType">Log Level type</param>
    ''' <param name="Message">Message to log</param>
    ''' <param name="Category">adhoc category or function name</param>
    ''' <param name="CategoryType">Defined category type (if applicable)</param>
    ''' <remarks></remarks>
    Public Sub Write(ByRef LogType As LogType, ByRef Message As String, ByVal Category As String, ByVal CategoryType As CategoryTypes)
        If LogLevel = LogType.Debug Then Diagnostics.Debug.WriteLine(Message)

        'Write to Application event log (all except debug level)
        If LogType > LogType.Debug Then
            Try
                If Not EventLog.SourceExists(My.Application.Info.ProductName) Then
                    EventLog.CreateEventSource(My.Application.Info.ProductName, "Application")
                End If
                Dim objLog As New EventLog()
                objLog.Source = My.Application.Info.ProductName
                If LogType = LogType.Error Then
                    objLog.WriteEntry(Message, EventLogEntryType.Error, CategoryType)
                Else
                    objLog.WriteEntry(Message, EventLogEntryType.Information, CategoryType)
                End If
            Catch ex As Exception
                Diagnostics.Debug.WriteLine("Logger (EventLog): " & ex.Message)
            End Try
        End If

        'Write to file
        If LogType >= LogLevel Then
            Dim LogTime As String = Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
            If Category = String.Empty And CategoryType > CategoryTypes.None Then Category = CategoryType.ToString
            Dim strLogLine As String = String.Concat(String.Concat(LogTime, vbTab, "T", Threading.Thread.CurrentThread.GetHashCode, vbTab, LogType.ToString, vbTab), _
                String.Concat(Category, vbTab, Message))
            'Roll file as it gets big (max 1 file/day)
            Try
                Dim fiLog As New FileInfo(LogFilePath)
                If fiLog.Length > 10000000 Then File.Move(LogFilePath, LogFilePath.Replace(".txt", Now.ToString("yyyyMMdd") & ".txt"))
            Catch
            End Try
            'Write
            Try
                Using Fs As New FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                    Using writer As New StreamWriter(Fs)
                        writer.WriteLine(strLogLine)
                        writer.Flush()
                        writer.Close()
                    End Using
                End Using
            Catch ex As Exception
                Diagnostics.Debug.WriteLine("Logger (FileWriter): " & ex.Message)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Are we logging anything?
    ''' </summary>
    ''' <returns>True if LogLevel is > than None</returns>
    ''' <remarks></remarks>
    Public Function IsEnabled() As Boolean
        Return LogLevel > LogType.None
    End Function

    ''' <summary>
    ''' Open the log file for writing. Closes log and returns the success
    ''' </summary>
    ''' <returns>Opens and closes the log</returns>
    ''' <remarks></remarks>
    Public Function IsLogWritable() As Boolean
        Try
            Using Fs As New FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                Using writer As New StreamWriter(Fs)
                    writer.Close()
                End Using
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Delete old log files depending on age
    ''' </summary>
    ''' <param name="AgeIfMany">Max log age in months if there are &gt; 10 log files</param>
    ''' <param name="AgeIfFew">Max log age in months if there are &lt;= 10 log files</param>
    ''' <remarks></remarks>
    Public Sub DeleteLogs(Optional ByVal AgeIfMany As Int16 = 3, Optional ByVal AgeIfFew As Int16 = 12)
        Dim aryFiles() As String = Directory.GetFiles(LogFilePath.Substring(0, LogFilePath.LastIndexOf("\", StringComparison.Ordinal)), LogFile.Replace(".txt", "*.txt"))
        Dim intAge As Int16
        If aryFiles.Length > 10 Then
            intAge = AgeIfMany
        Else
            intAge = AgeIfFew
        End If
        Dim datModified As DateTime
        Dim intDeleted As Int16 = 0
        Try
            For Each strFile As String In aryFiles
                datModified = File.GetLastWriteTime(strFile)
                If datModified.AddMonths(intAge).CompareTo(Now) < 0 Then
                    File.Delete(strFile)
                    intDeleted += 1
                End If
            Next
        Catch
        End Try
        If intDeleted > 0 Then Info(intDeleted.ToString, "Logs deleted")
    End Sub
End Class
