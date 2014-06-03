Imports Alphaleonis.Win32

Public Class ViewFile
    Public FilesToView As ArrayList
    Public ftp As Utilities.FTP.FTPclient
    Public Config As ConfigEngine

    Public Sub New(ByRef ConfigInstance As ConfigEngine)
        InitializeComponent()
        Config = ConfigInstance
    End Sub

    Private Sub AppSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Const IMAGE_EXT As String = "JPG|JPEG|JPE|GIF|BMP|DIB|TIF|TIFF|PNG|PCX|TGA|PCD|SUN|RAS|ICO|AVI|IFF|PPM|PGM|PBM|LBM|WMF|EMF|WAV|PSD|MPG|MPE|MPEG|MID|RMI|MOV|CUR|ANI|DCX|EPS|CLP|LWF|LDF|CAM|G3|AIF|AU|SND|PSP|PSPIMAGE|ICL|SFW|KDC|RA|MP3|DCM|ACR|FPX|XBM|XPM|DJVU|SWF|IMG|IW44|WBMP|SGI|RGB|RLE|MED|RLE|SFF|NLM|NOL|NGG|GSM|JP2|JPC|J2K|JPF|FSH|CRW|B3D|WMA|WMV|TTF|MNG|JNG|RAW|ECW|ASF|ICS|IDS|CDR|CMX|DCR|X3F|SRF|PEF|OGG|DWG|DXF|DDS|JPM|CR2|PS|PDF|CGM|SVG|"
        Dim filView As New Filesystem.FileInfo(FilesToView(0))
        Try
            If IMAGE_EXT.Contains(filView.Extension.Replace(".", String.Empty).ToUpper) Then
                txtApp.Text = Config.AppConfigDefault("ImageViewer", "MSPaint.exe")
                'If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("ImageViewer")) Then
                '    txtApp.Text = System.Configuration.ConfigurationManager.AppSettings("ImageViewer")
                'Else
                '    txtApp.Text = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "ImageViewer", "MSPaint.exe")
                'End If
            Else
                txtApp.Text = Config.AppConfigDefault("TextViewer", "Notepad.exe")
                'If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("TextViewer")) Then
                '    txtApp.Text = System.Configuration.ConfigurationManager.AppSettings("TextViewer")
                'Else
                '    txtApp.Text = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "TextViewer", "Notepad.exe")
                'End If
            End If
        Catch ex As Exception
        End Try
        OpenFileDialog1.Filter = "Applications (*.exe)|*.exe"
        OpenFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        OpenFileDialog1.CheckFileExists = True
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        'Compare App
        Try
            Dim fiApp As New System.IO.FileInfo(txtApp.Text)
            OpenFileDialog1.FileName = fiApp.Name
            OpenFileDialog1.InitialDirectory = fiApp.DirectoryName
        Catch
            'OpenFileDialog1.FileName = "Notepad.exe"
            'OpenFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        End Try
        If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtApp.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        If Not IsNothing(ftp) Then
            'Download to temp files
            Dim strFile As String
            Try
                For intFile As Int16 = 0 To FilesToView.Count - 1
                    If FilesToView(intFile).ToString.Contains("/") Then
                        strFile = My.Computer.FileSystem.SpecialDirectories.Temp & "\" & FilesToView(intFile).ToString.Substring(FilesToView(intFile).ToString.LastIndexOf("/", System.StringComparison.Ordinal) + 1)
                        If Not ftp.Download(FilesToView(intFile), strFile, True) Then MsgBox(ftp.ErrorText, MsgBoxStyle.Exclamation)
                        FilesToView(intFile) = strFile
                    End If
                Next
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            End Try
        End If

        If rbAssoc.Checked Then
            Try
                For Each strFile As String In FilesToView
                    If FileSystem.File.Exists(strFile) Then Process.Start(FileUtils.Get83PathIfLong(strFile))
                Next
                Me.Close()
            Catch ex As Exception
                MsgBox("Error opening file: " & ex.Message, MsgBoxStyle.Exclamation)
            End Try
        Else
            Try
                For Each strFile As String In FilesToView
                    If FileSystem.File.Exists(strFile) Then Process.Start(txtApp.Text, """" & FileUtils.Get83PathIfLong(strFile) & """")
                Next
                Me.Close()
            Catch ex As Exception
                MsgBox("Error running app: " & ex.Message, MsgBoxStyle.Exclamation)
            End Try
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        'System.Diagnostics.Process.Start(Application.StartupPath & "\AutoVerHelp.htm", "#OpenFile")
        Help.ShowHelp(Me, "AutoVer.chm", HelpNavigator.TopicId, "5")
    End Sub

End Class