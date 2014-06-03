Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports AutoVer

'http://msdn.microsoft.com/en-us/library/ms733069.aspx
'http://msdn.microsoft.com/en-us/library/ms731758.aspx

Public Class AutoVerServiceEngine
    Inherits System.ServiceProcess.ServiceBase

    Private Config As AutoVer.ConfigEngine
    Private host As ServiceHost
    Private timManager As System.Threading.Timer
    Private Log As Logger

#Region " Component Designer generated code "

    Public Sub New()
        MyBase.New()

        ' This call is required by the Component Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call
    End Sub

    'UserService overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' The main entry point for the process
    <MTAThread()> _
    Shared Sub Main()
        If My.Application.CommandLineArgs.Count > 0 Then
            'Windows Service install/uninstall
            Dim ConfigLocal As New ConfigEngine
            Dim Log As Logger = ConfigLocal.Log 'Use the config engines log which will update its log level from INFO to what is in the config file.
            Dim fvi As FileVersionInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location)
            Log.Info(fvi.ProductName & " " & fvi.FileMajorPart & "." & fvi.FileMinorPart & "." & fvi.FileBuildPart, "CommandLineStart")
            ConfigLocal.LoadAppConfig()
            Log.Info(ConfigLocal.ConfigFolder, "ConfigPath")
            Log.Info(Log.LogLevel.ToString, "LogLevel")
            Log.Debug(My.Application.CommandLineArgs(0), "CommandLine")

            Dim installUtil As String = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(GetType(System.String).Assembly.Location), "InstallUtil.exe")
            Log.Info(installUtil, "ServiceInstaller")

            Dim dom As AppDomain = AppDomain.CreateDomain("domInstallUtil")
            If My.Application.CommandLineArgs(0).ToLower.StartsWith("/i") Or My.Application.CommandLineArgs(0).ToLower.StartsWith("-i") Then
                dom.ExecuteAssembly(installUtil, New String() {"/LogFile=" & Log.LogFilePath, "/LogToConsole=true", System.Reflection.Assembly.GetExecutingAssembly().Location})
            ElseIf My.Application.CommandLineArgs(0).ToLower.StartsWith("/u") Or My.Application.CommandLineArgs(0).ToLower.StartsWith("-u") Then
                dom.ExecuteAssembly(installUtil, New String() {"/u", "/LogFile=" & Log.LogFilePath, "/LogToConsole=true", System.Reflection.Assembly.GetExecutingAssembly().Location})
            ElseIf My.Application.CommandLineArgs(0).ToLower.StartsWith("/r") Or My.Application.CommandLineArgs(0).ToLower.StartsWith("-r") Then
                Console.WriteLine("Service running direct. Press a key to exit...")
                Dim AutoVerService As New AutoVerServiceEngine()
                AutoVerService.OnStart(New String() {""})
                'Do While Not Console.KeyAvailable
                '    Threading.Thread.Sleep(1000) 'You can enter but never leave...
                'Loop
                'Console.ReadKey(True)
            Else
                Console.WriteLine("Run with -i to install. -u to uninstall service.")
            End If
            'Console is not enabled unless "Interact with Desktop" is enabled - and that is bad security
            Console.WriteLine("See " & Log.LogFilePath & " for service installation status.")
        Else
            Dim ServicesToRun() As System.ServiceProcess.ServiceBase

            ' More than one NT Service may run within the same process. To add
            ' another service to this process, change the following line to
            ' create a second service object. For example,
            '
            '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
            '
            ServicesToRun = New System.ServiceProcess.ServiceBase() {New AutoVerServiceEngine()}

            System.ServiceProcess.ServiceBase.Run(ServicesToRun)
        End If
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        '
        'ScheduleService
        '
        Me.ServiceName = "AutoVer Service"

    End Sub

#End Region

    Protected Overrides Sub OnStart(ByVal args() As String)
        Try
            Config = New ConfigEngine()
            Log = Config.Log 'Use the config engines log which will update its log level from INFO to what is in the config file.
            Dim fvi As FileVersionInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location)
            Log.Info(fvi.ProductName & " " & fvi.FileMajorPart & "." & fvi.FileMinorPart & "." & fvi.FileBuildPart, "Starting")
            Log.Info(System.Reflection.Assembly.GetExecutingAssembly.Location, "Location")
            Config.LoadAppConfig()
            Dim TCPPort As Integer = CInt(Config.AppConfigDefault("ServicePort", "9072"))
            Log.Info(Net.Dns.GetHostName & " / " & Net.Dns.GetHostEntry(Net.Dns.GetHostName).AddressList(0).ToString() & " | 127.0.0.1:" & TCPPort.ToString, "Host PC | TCP Port")
            Log.Info(Config.ConfigFolder, "ConfigPath")
            Log.Info(Log.LogLevel.ToString, "LogLevel")
            Log.DeleteLogs(6, 12)

            Dim wp As New System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent())
            If wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator) Then
                Log.Info(wp.Identity.Name, "UserRole:Admin")
            ElseIf wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.User) Then
                Log.Info(wp.Identity.Name, "UserRole:User")
            ElseIf wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.SystemOperator) Then
                Log.Info(wp.Identity.Name, "UserRole:SystemOperator")
            ElseIf wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.PowerUser) Then
                Log.Info(wp.Identity.Name, "UserRole:PowerUser")
            ElseIf wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.BackupOperator) Then
                Log.Info(wp.Identity.Name, "UserRole:BackupOperator")
            ElseIf wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Guest) Then
                Log.Info(wp.Identity.Name, "UserRole:Guest")
            Else
                Log.Info(wp.Identity.Name, "UserRole:?")
            End If

            'Create WCF endpoint to the singleton config object and start listening to the port
            Try
                Dim baseAddress As Uri = New Uri("http://127.0.0.1:" & TCPPort.ToString & "/AutoVer/")
                host = New ServiceHost(Config, baseAddress)
                Dim smb As New ServiceMetadataBehavior()
                smb.HttpGetEnabled = True
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15
                host.Description.Behaviors.Add(smb)
                Dim mexBinding As Channels.Binding = MetadataExchangeBindings.CreateMexHttpBinding()
                host.AddServiceEndpoint(GetType(IConfigEngine), mexBinding, "mex")
                Dim httpBinding As New WSHttpBinding()
                ' Dim ep As New ServiceEndpoint(
                host.AddServiceEndpoint(GetType(IConfigEngine), httpBinding, baseAddress)
                host.Open()

                Log.Debug(host.State.ToString, "BindingEndPointState")
            Catch ex As Exception
                Log.Error(ex.Message, "BindingEndPoint")
            End Try
            Config.IsService = True
            Config.LoadAppConfig()
            Config.LoadWatcherConfig()

            'For Each WatchEngine As BackupEngine In Config.WatcherEngines
            '    Log.Debug(WatchEngine.Name, WatchEngine.Started)
            'Next
        Catch ex As Exception
            Log.Error(ex.Message, "Service")
        End Try
    End Sub

    Protected Overrides Sub OnStop()
        Log.Info("Stopping", "Stop")
        Try
            timManager.Dispose()
            host.Close()
        Catch
        End Try
    End Sub


End Class