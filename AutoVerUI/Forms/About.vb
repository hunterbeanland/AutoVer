Imports System.ComponentModel
Imports System.Net.Security
'Imports System.Security.Cryptography.X509Certificates
Imports System.Net
Imports System.IO
'Imports System.Text.RegularExpressions

Public NotInheritable Class About
    Private currVer As String

    Private Sub About_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("About {0}", ApplicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.lblProduct.Text = My.Application.Info.ProductName
        Me.lblDescription.Text = My.Application.Info.Description.Replace("&", "&&")
        currVer = My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build
        Me.lblVersion.Text = String.Format("Version {0}", currVer)
        Me.lblCopyright.Text = My.Application.Info.Copyright
        Me.lblBy.Text = "Written by " & My.Application.Info.CompanyName
        Try
            PictureBox1.Load(Application.StartupPath & "\AutoVerLogo.png")
        Catch ex As Exception
            Dim Log As New Logger
            Log.Warn(ex.Message, "AutoVerLogo")
        End Try

        lnkUpdates.Enabled = False
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub UpdatesGet(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        'Web service can't get FileVersion in Medium trust hosted environment. So scrape from AutoVer's page - header tag
        Try
            Dim wsBeanWeb As New beanlandnetau.BeanAppService
            e.Result = wsBeanWeb.LatestVersion("AutoVer")
        Catch
            e.Result = Nothing
        End Try
        'Dim uri As String = "http://beanland.net.au/AutoVer/?ver=" & currVer
        'Dim strPage As String = HTTPRequestPage(uri)
        ''If IsNothing(strPage) Then strPage = HTTPRequestPage(uri) 'retry
        'If IsNothing(strPage) Then
        '    e.Result = String.Empty
        'Else
        '    'Dim Log As New Logger()
        '    'Log.Debug(strPage, "strPage")
        '    Dim regEx As New System.Text.RegularExpressions.Regex(String.Empty, RegexOptions.Multiline Or RegexOptions.IgnoreCase)
        '    Dim h1 As Match = regEx.Match(strPage, "AutoVer (.*)</h1>")
        '    If h1.Success Then
        '        'Log.Debug(h1.Groups(1).Captures(0).Value, "Captures")
        '        e.Result = h1.Groups(1).Captures(0).Value
        '    Else
        '        e.Result = String.Empty
        '    End If
        'End If
    End Sub

    Private Sub UpdatesResult(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If String.IsNullOrEmpty(e.Result) Then
            lnkUpdates.Enabled = True
            lnkUpdates.Text = "Cannot find updates"
        Else
            Dim newVer() As String = e.Result.ToString.Split("."c)
            Dim intNewVer, intCurrVer, intTemp As Integer
            If newVer.Length > 0 AndAlso Integer.TryParse(newVer(0), intTemp) Then intNewVer += intTemp * 10000
            If newVer.Length > 1 AndAlso Integer.TryParse(newVer(1), intTemp) Then intNewVer += intTemp * 100
            If newVer.Length > 2 AndAlso Integer.TryParse(newVer(2), intTemp) Then intNewVer += intTemp
            intCurrVer = My.Application.Info.Version.Major * 10000 + My.Application.Info.Version.Minor * 100 + My.Application.Info.Version.Build
            If intNewVer > intCurrVer Then
                lnkUpdates.Enabled = True
                lnkUpdates.Text = "Update Found: " & e.Result
                Dim Log As New Logger()
                Log.Info("update: " & intNewVer.ToString & " current:" & intCurrVer.ToString, "UpdateFindVer")
            Else
                lnkUpdates.Text = "There are no updates"
            End If
        End If
    End Sub

    Private Sub butOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butOK.Click
        Me.Close()
    End Sub

    Private Sub lblWebLink_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lblWebLink.LinkClicked
        System.Diagnostics.Process.Start("http://beanland.net.au/AutoVer/")
    End Sub

    Private Sub lnkDonate_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkDonate.LinkClicked
        System.Diagnostics.Process.Start("http://beanland.net.au/AutoVer/#Donate")
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkUpdates.LinkClicked
        System.Diagnostics.Process.Start("http://beanland.net.au/AutoVer/")
    End Sub

    'Private Function HTTPRequestPage(ByVal URIPath As String) As String
    '    'HTTP GET web page.
    '    Dim req As HttpWebRequest = HttpWebRequest.Create(URIPath)
    '    Dim stm As Stream
    '    req.Accept = "*/*"
    '    req.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; AutoVer;)"
    '    req.Timeout = 20000 'ms
    '    req.KeepAlive = False
    '    req.Method = "GET"
    '    req.ContentType = "text/html;charset=""utf-8"""
    '    ServicePointManager.ServerCertificateValidationCallback = New RemoteCertificateValidationCallback(AddressOf TrustAllCertificateValidation)
    '    'Response
    '    Try
    '        Dim resp As HttpWebResponse = req.GetResponse()
    '        stm = resp.GetResponseStream()
    '        Dim stmRead As StreamReader = New StreamReader(stm)
    '        Return stmRead.ReadToEnd()
    '    Catch ex As Exception
    '        Dim Log As New Logger()
    '        Log.Warn("URI gives error: " & URIPath & " - " & ex.Message)
    '        Return Nothing
    '    End Try
    '    Return String.Empty
    'End Function

    'Public Function TrustAllCertificateValidation(ByVal sender As Object, ByVal cert As X509Certificate, _
    '    ByVal chain As X509Chain, ByVal Errors As SslPolicyErrors) As Boolean
    '    'Accept all certificates to connect to any SSL (HTTPS) connection
    '    Return True
    'End Function
End Class
