
Public Class AppSettings
    'Private InitSettings As String
    'Private ReportStopWatch As New System.Diagnostics.Stopwatch()
    'Private OperationCount As Integer
    Public Config As ConfigEngine

    Public Sub New(ByRef ConfigInstance As ConfigEngine)
        InitializeComponent()
        Config = ConfigInstance
    End Sub

    Private Sub AppSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'defaults usually get set in the Config engine during the AutoVer.ini read
        Dim pfx86 As String = Environment.GetEnvironmentVariable("ProgramFiles(x86)")
        If String.IsNullOrEmpty(pfx86) Then pfx86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        txtApp.Text = Config.AppConfigDefault("CompareApp", pfx86 & "\WinMerge\WinMergeU.exe ""{0}"" ""{1}""")
        txtTextViewer.Text = Config.AppConfigDefault("TextViewer", "Notepad.exe")
        txtImageViewer.Text = Config.AppConfigDefault("ImageViewer", "MSPaint.exe")
        Select Case Config.AppConfigDefault("ConfigFolder", "COMMON")
            Case "COMMON"
                rbConfigCommon.Checked = True
            Case "USER"
                rbConfigUser.Checked = True
            Case "LOCAL"
                rbConfigLocal.Checked = True
        End Select
        chkAutoElevate.Checked = Config.AppConfigDefault("AutoElevate", "1") = "1"
        chkWMI.Checked = Config.AppConfigDefault("WMI", "1") = "1"
        chkDebug.Checked = Config.AppConfigDefault("LogLevel", "INFO") = "DEBUG"
        chkRecycleBin.Checked = Config.AppConfigDefault("RecycleBin", "0") = "1"
        'AppConfig("ServicePort") = "9090"

        Try
            If Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", "AutoVer", String.Empty) = String.Empty Then chkStartup.Checked = False
            '    If Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "CompareApp", String.Empty) = String.Empty Then
            '        txtApp.Text = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\WinMerge\WinMerge.exe ""{0}"" ""{1}"""
            '    Else
            '        txtApp.Text = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "CompareApp", String.Empty)
            '    End If
            '    txtTextViewer.Text = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "TextViewer", "Notepad.exe")
            '    txtImageViewer.Text = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "ImageViewer", "MSPaint.exe")
            '    If Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "WMI", 1) = 0 Then chkWMI.Checked = False
            '    If Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "Debug", 0) = 1 Then chkDebug.Checked = True
            '    If Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "RecycleBin", 0) = 1 Then chkRecycleBin.Checked = True
        Catch ex As Exception
            Dim Log As New Logger
            Log.Error(ex.Message, "Can not get CKCU\..\Run in registry:")
        End Try
        'Try
        '    If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("CompareApp")) Then txtApp.Text = System.Configuration.ConfigurationManager.AppSettings("CompareApp")
        '    If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("TextViewer")) Then txtTextViewer.Text = System.Configuration.ConfigurationManager.AppSettings("TextViewer")
        '    If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("ImageViewer")) Then txtImageViewer.Text = System.Configuration.ConfigurationManager.AppSettings("ImageViewer")
        '    If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("WMI")) Then chkWMI.Checked = System.Configuration.ConfigurationManager.AppSettings("WMI")
        '    If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("Debug")) Then chkDebug.Checked = System.Configuration.ConfigurationManager.AppSettings("Debug")
        '    If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("RecycleBin")) Then chkRecycleBin.Checked = System.Configuration.ConfigurationManager.AppSettings("RecycleBin")
        'Catch
        'End Try
        'InitSettings = IIf(chkWMI.Checked, "1", "0") & IIf(chkDebug.Checked, "1", "0") & IIf(chkRecycleBin.Checked, "1", "0")
        OpenFileDialog1.Filter = "Applications (*.exe)|*.exe"
        OpenFileDialog1.CheckFileExists = True
        'Config.Lang.ExportControlsToFile(Me, ToolTip1)
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCompApp.Click
        'Compare App
        Dim strApp As String = txtApp.Text
        If strApp.Contains(".exe") Then strApp = strApp.Substring(0, strApp.LastIndexOf(".exe", System.StringComparison.Ordinal) + 4)
        Try
            Dim fiApp As New System.IO.FileInfo(strApp)
            OpenFileDialog1.FileName = fiApp.Name
            OpenFileDialog1.InitialDirectory = fiApp.DirectoryName
        Catch
            OpenFileDialog1.FileName = "WinMergeU.exe"
            OpenFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        End Try
        If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtApp.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTextView.Click
        'Text Viewer
        Try
            Dim fiApp As New System.IO.FileInfo(txtTextViewer.Text)
            OpenFileDialog1.FileName = fiApp.Name
            OpenFileDialog1.InitialDirectory = fiApp.DirectoryName
        Catch
            OpenFileDialog1.FileName = "Notepad.exe"
            OpenFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        End Try
        If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtTextViewer.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImageView.Click
        'Image Viewer
        Try
            Dim fiApp As New System.IO.FileInfo(txtImageViewer.Text)
            OpenFileDialog1.FileName = fiApp.Name
            OpenFileDialog1.InitialDirectory = fiApp.DirectoryName
        Catch
            OpenFileDialog1.FileName = "MSPaint.exe"
            OpenFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        End Try
        If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtImageViewer.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Dim Log As New Logger
        Try
            If chkStartup.Checked Then
                Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", "AutoVer", Application.ExecutablePath)
            Else
                Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", "AutoVer", String.Empty)
            End If
        Catch ex As Exception
            Log.Error(ex.Message, "Can not set CKCU\..\Run in registry:")
        End Try
        If txtApp.Text.Trim <> String.Empty Then
            Dim strApp As String = txtApp.Text
            If strApp.Contains(".exe") Then strApp = strApp.Substring(0, strApp.LastIndexOf(".exe", System.StringComparison.Ordinal) + 4)
            If Not System.IO.File.Exists(strApp) Then
                MsgBox("Application not found. Select a valid application to compare your files! (or blank if none)", MsgBoxStyle.Exclamation)
                Exit Sub
            Else
                If Not txtApp.Text.Contains("{0}") Then txtApp.Text &= " {0}"
                If Not txtApp.Text.Contains("{1}") Then txtApp.Text &= " {1}"
            End If
        End If
        Me.Cursor = Cursors.WaitCursor
        Dim AppSettings As Generic.Dictionary(Of String, String) = Config.AppConfig
        If rbConfigCommon.Checked Then
            AppSettings("ConfigFolder") = "COMMON"
        ElseIf rbConfigUser.Checked Then
            AppSettings("ConfigFolder") = "USER"
        Else
            AppSettings("ConfigFolder") = "LOCAL"
        End If
        AppSettings("LogLevel") = IIf(chkDebug.Checked, "DEBUG", "INFO")
        AppSettings("AutoElevate") = IIf(chkAutoElevate.Checked, "1", "0")
        AppSettings("WMI") = IIf(chkWMI.Checked, "1", "0")
        AppSettings("CompareApp") = txtApp.Text
        AppSettings("TextViewer") = txtTextViewer.Text
        AppSettings("ImageViewer") = txtImageViewer.Text
        AppSettings("RecycleBin") = IIf(chkRecycleBin.Checked, "1", "0")
        Config.AppConfig = AppSettings

        'Config.Log.UpdateLogLevel(AppSettings("LogLevel"))
        Config.SaveAppConfig() 'Updates the live app too

        If Not IO.File.Exists(Config.AppConfigFolder & "AutoVer.ini") Then
            MsgBox("Error writing to the config file. Select a different Config Location.", MsgBoxStyle.Exclamation)
            Me.Cursor = Cursors.Default
            Exit Sub
        End If
        If Not IO.File.Exists(Config.ConfigFolder & "AutoVer.xml") Then
            Config.SaveWatcherConfig()
            If Not IO.File.Exists(Config.ConfigFolder & "AutoVer.xml") Then
                MsgBox("Error writing to the config file. Select a different Config Location.", MsgBoxStyle.Exclamation)
                Me.Cursor = Cursors.Default
                Exit Sub
            End If
        End If

        Me.Cursor = Cursors.Default
        'Try
        '    UpdateAppSettings()
        'Catch ex As Exception
        '    Log.Error(ex.Message, "Can not set settings in .config file:")
        'End Try
        'Try
        '    'HKEY_LOCAL_MACHINE
        '    Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\AutoVer", "CompareApp", txtApp.Text.Trim)
        '    Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\AutoVer", "TextViewer", txtTextViewer.Text.Trim)
        '    Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\AutoVer", "ImageViewer", txtImageViewer.Text.Trim)
        '    Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\AutoVer", "WMI", IIf(chkWMI.Checked, 1, 0))
        '    Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\AutoVer", "Debug", IIf(chkDebug.Checked, 1, 0))
        '    Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\Software\AutoVer", "RecycleBin", IIf(chkRecycleBin.Checked, 1, 0))
        'Catch ex As Exception
        '    Log.Error(ex.Message, "Can not set HKCU\Software\AutoVer in registry:")
        'End Try
        'If InitSettings <> (IIf(chkWMI.Checked, "1", "0") & IIf(chkDebug.Checked, "1", "0") & IIf(chkRecycleBin.Checked, "1", "0")) Then
        '    MessageBox.Show("You must restart Autover/Service for these changes to take effect", "Restart", MessageBoxButtons.OK, MessageBoxIcon.Information)
        'End If
        Me.Close()
    End Sub

    'Private Sub UpdateAppSettings()
    '    'Update AutoVer.exe.config    
    '    Dim config As System.Configuration.Configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None)
    '    If Array.IndexOf(config.AppSettings.Settings.AllKeys, "CompareApp") > -1 Then
    '        config.AppSettings.Settings("CompareApp").Value = txtApp.Text.Trim
    '    Else
    '        config.AppSettings.Settings.Add("CompareApp", txtApp.Text.Trim)
    '    End If
    '    If Array.IndexOf(config.AppSettings.Settings.AllKeys, "TextViewer") > -1 Then
    '        config.AppSettings.Settings("TextViewer").Value = txtTextViewer.Text.Trim
    '    Else
    '        config.AppSettings.Settings.Add("TextViewer", txtTextViewer.Text.Trim)
    '    End If
    '    If Array.IndexOf(config.AppSettings.Settings.AllKeys, "ImageViewer") > -1 Then
    '        config.AppSettings.Settings("ImageViewer").Value = txtImageViewer.Text.Trim
    '    Else
    '        config.AppSettings.Settings.Add("ImageViewer", txtImageViewer.Text.Trim)
    '    End If
    '    If Array.IndexOf(config.AppSettings.Settings.AllKeys, "WMI") > -1 Then
    '        config.AppSettings.Settings("WMI").Value = chkWMI.Checked
    '    Else
    '        config.AppSettings.Settings.Add("WMI", chkWMI.Checked)
    '    End If
    '    If Array.IndexOf(config.AppSettings.Settings.AllKeys, "Debug") > -1 Then
    '        config.AppSettings.Settings("Debug").Value = chkDebug.Checked
    '    Else
    '        config.AppSettings.Settings.Add("Debug", chkDebug.Checked)
    '    End If
    '    If Array.IndexOf(config.AppSettings.Settings.AllKeys, "RecycleBin") > -1 Then
    '        config.AppSettings.Settings("RecycleBin").Value = chkRecycleBin.Checked
    '    Else
    '        config.AppSettings.Settings.Add("RecycleBin", chkRecycleBin.Checked)
    '    End If
    '    config.Save(System.Configuration.ConfigurationSaveMode.Modified)
    '    System.Configuration.ConfigurationManager.RefreshSection("appSettings")
    'End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        Help.ShowHelp(Me, "AutoVer.chm", HelpNavigator.TopicId, "4")
        'System.Diagnostics.Process.Start(Application.StartupPath & "\AutoVerHelp.htm", "#AppSettings")
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLog.Click
        Try
            If Config.IsService Then
                System.Diagnostics.Process.Start(txtTextViewer.Text.Trim, """" & Config.Log.LogFilePath.Replace("AutoVerService", "AutoVer") & """")
            ElseIf IO.File.Exists(Config.Log.LogFilePath.Replace("AutoVer", "AutoVerService")) Then
                System.Diagnostics.Process.Start(txtTextViewer.Text.Trim, """" & Config.Log.LogFilePath.Replace("AutoVer", "AutoVerService") & """")
            End If
            System.Diagnostics.Process.Start(txtTextViewer.Text.Trim, """" & Config.Log.LogFilePath & """")
        Catch ex As Exception
            System.Diagnostics.Process.Start("""" & Config.Log.LogFilePath & """")
        End Try
    End Sub

    Private Sub btnConfigFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfigFolder.Click
        Try
            System.Diagnostics.Process.Start("""" & Config.AppConfigFolder & """")
            If Config.ConfigFolder <> Config.AppConfigFolder Then System.Diagnostics.Process.Start("""" & Config.ConfigFolder & """")
        Catch ex As Exception
        End Try
    End Sub

End Class