Namespace My

    ' The following events are availble for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        'Sub StartupNextInstance_Alert(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) Handles Me.StartupNextInstance
        '    MsgBox("AutoVer is already running!" & vbLf & "Right click the AutoVer icon in the System Tray to open", MsgBoxStyle.Information)
        '    e.BringToForeground = True
        'End Sub
        Private frmErrorReporter As ErrorReporter

        Sub App_UnhandledException(ByVal sender As System.Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles MyBase.UnhandledException
            Dim strLogFolder As String
            Try
                Dim Config As New ConfigEngine()
                strLogFolder = Config.AppConfigFolder
                Config.Log.Error(e.Exception.ToString, "UnhandledException")
            Catch ex As Exception
                strLogFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\AutoVer\"
                Try
                    If Not IO.Directory.Exists(strLogFolder) Then IO.Directory.CreateDirectory(strLogFolder)
                Catch
                End Try
            End Try
            If IsNothing(frmErrorReporter) OrElse Not frmErrorReporter.Visible Then frmErrorReporter = New ErrorReporter(strLogFolder)
            frmErrorReporter.ErrorException = e.Exception
            frmErrorReporter.Show()
            frmErrorReporter.Activate()
            e.ExitApplication = False
        End Sub
    End Class

End Namespace

