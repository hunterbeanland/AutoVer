Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp
Imports AutoVer

Public Class MainForm
    Private timManager As System.Threading.Timer
    Private BPPollingPri, BPPollingSec As AutoVer.ConfigEngine
    Private PollsPer, PollsTimeFrame, ConnectsPer, ConnectsTimeFrame As Integer
    Private Log As New Logger

    Protected Sub OnStart(ByVal args() As String) 'Overrides

        'Info for log
        Dim fvi As FileVersionInfo
        fvi = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location)
        Log.Info(fvi.ProductName & " " & fvi.FileMajorPart & "." & fvi.FileMinorPart & "." & fvi.FileBuildPart, "Start")
        Dim strLogPath As String = System.Configuration.ConfigurationManager.AppSettings("LogPath")
        If strLogPath.Trim = String.Empty Then
            Dim fi As New IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly.Location)
            strLogPath = fi.Directory.FullName
        ElseIf strLogPath.StartsWith(".") Then
            Dim fi As New IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly.Location)
            My.Computer.FileSystem.CurrentDirectory = fi.Directory.FullName
            strLogPath = IO.Path.GetFullPath(strLogPath)
        End If
        If Not strLogPath.EndsWith("\") Then strLogPath &= "\"
        Log.Info(System.Reflection.Assembly.GetExecutingAssembly.Location & " | " & strLogPath, "Path/LogPath")
        Log.Info(Net.Dns.GetHostName & " / " & Net.Dns.GetHostEntry(Net.Dns.GetHostName).AddressList(0).ToString(), "Host")


        'Get reference to the singelton for the Manager thread
        BPPollingPri = CType(Activator.GetObject(GetType(AutoVer.ConfigEngine), "tcp://localhost:9091/SymPulsePolling/Bluepulse"), AutoVer.ConfigEngine)
        BPPollingSec = CType(Activator.GetObject(GetType(AutoVer.ConfigEngine), "tcp://localhost:9091/SymPulsePolling/Bluepulse"), AutoVer.ConfigEngine)
        timManager = New System.Threading.Timer(AddressOf timManager_OnTick, Nothing, 15000, 15000)
        Log.Info(BPPollingPri.GetHashCode.ToString, "GetHashCode")
        Log.Info(BPPollingPri.IsService.ToString, "IsAService")

        'BPPollingPri.IsAService = True
        ' BPPollingPri.LoadAppConfig()
        'BPPollingPri.LoadWatcherConfig()
        Log.Info("ready to enum", "enum")
        For Each WatchEngine As BackupEngine In BPPollingPri.WatcherEngines
            Log.Info(WatchEngine.Name)
        Next
    End Sub

    Protected Sub OnStop() 'Overrides
        Log.Info("Stopping", "Stop")
        Try
            timManager.Dispose()
        Catch
        End Try
    End Sub

    Protected Sub timManager_OnTick(ByVal state As Object)
        Dim intPolls, intConnects As Integer
        Try
            Console.WriteLine(BPPollingPri.IsAService.ToString)
        Catch
            intPolls = 0
        End Try
        Try
            Console.WriteLine(BPPollingSec.IsAService.ToString)
        Catch
        End Try
        If intPolls < PollsPer Then
            Log.Info("Poll warning. Polls=" & intPolls.ToString, "PollAlert")
            Dim strBody As String = "<div style='font-family:arial'>WARNING: SymPulse attempted Bluepulse polling of " & intPolls.ToString & " times has fallen under critical levels of " & PollsPer.ToString & " times per " & PollsTimeFrame.ToString & " secs.</div>"
            Email(System.Configuration.ConfigurationManager.AppSettings("EmailAlerts"), """SymPulse"" <NoReply@symphony.com.my>", "SymPulse Polling Alert", strBody)
        Else
            Log.Debug(intPolls.ToString, "Polls")
        End If

        If ConnectsPer > 0 Then
            Try
                '   intConnects = intPolls + BPPollingPri.ConnectsSince(Now.AddSeconds(-ConnectsTimeFrame))
            Catch
                intConnects = intPolls
            End Try
            Try
                '  intConnects += BPPollingSec.ConnectsSince(Now.AddSeconds(-ConnectsTimeFrame))
            Catch
            End Try
            If intConnects < ConnectsPer Then
                Log.Info("Connect warning. Connects=" & intConnects.ToString, "ConnectAlert")
                Dim strBody As String = "<div style='font-family:arial'>WARNING: SymPulse attempted Bluepulse connections of " & intConnects.ToString & " times has fallen under critical levels of " & ConnectsPer.ToString & " times per " & ConnectsTimeFrame.ToString & " secs.</div>"
                Email(System.Configuration.ConfigurationManager.AppSettings("EmailAlerts"), """SymPulse"" <NoReply@symphony.com.my>", "SymPulse Connection Alert", strBody)
            Else
                Log.Debug(intConnects.ToString, "Connects")
            End If
        End If

    End Sub

    Public Overloads Sub Email(ByVal EmailTo As String, ByVal EmailFrom As String, ByVal Subject As String, ByVal BodyHTML As String)
        'Send an email
        Dim RegExp As New System.Text.RegularExpressions.Regex("\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        Dim blnAddressOK As Boolean = True
        Dim aryAddresses() As String
        Dim strAddresses As String = String.Empty
        Dim intIndex As Integer

        If EmailTo.Trim = String.Empty Then
            Log.Debug("No email address to send to")
            Exit Sub
        End If
        'Can't add From address after message object is New'ed, so add to New, but then that needs To address, so valid it as a string to add it
        'Filter invalid addresses as they will cause an exception
        aryAddresses = EmailTo.Split(New Char() {","})
        For intIndex = 0 To aryAddresses.Length - 1
            If RegExp.IsMatch(aryAddresses(intIndex)) And aryAddresses(intIndex).ToLower <> "x@Symphony.com" Then
                If intIndex > 0 Then strAddresses &= ", "
                strAddresses &= aryAddresses(intIndex)
            End If
        Next
        If strAddresses = String.Empty Then
            Log.Debug("No VALID email address to send to")
            Exit Sub
        End If

        Dim objEmail As New System.Net.Mail.MailMessage(EmailFrom, strAddresses)
        objEmail.Subject = Subject
        objEmail.Body = BodyHTML
        objEmail.IsBodyHtml = True
        Dim objSMTP As New System.Net.Mail.SmtpClient
        objSMTP.Send(objEmail)
    End Sub

    '**************** WinForms specific test harness code
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Form1_UnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.FormClosing
        OnStop()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Label1.Text = "Starting"
        OnStart(New String() {""})
    End Sub

End Class


