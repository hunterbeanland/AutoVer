Imports System.IO
Imports System.Net
Imports System.Text
Imports Microsoft.Win32
Imports System.Management
Imports System.Environment
Imports Microsoft.Win32.Registry

Public Class ErrorReporter
    Public ErrorException As Exception
    Private sb As New StringBuilder
    Private DetailsInserted As Boolean = False
    Private strLogFile As String
    'For connection status
    Private Declare Function InternetGetConnectedState Lib "wininet.dll" (ByRef lpdwFlags As Integer, ByVal dwReserved As Integer) As Integer
    'For screen capture
    Private Declare Function BitBlt Lib "gdi32" Alias "BitBlt" (ByVal hDestDC As Integer, ByVal x As Integer, _
        ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hSrcDC As Integer, _
        ByVal xSrc As Integer, ByVal ySrc As Integer, ByVal dwRop As Integer) As Integer
    Private Declare Function GetDC Lib "user32" Alias "GetDC" (ByVal hwnd As Integer) As Integer
    Private Declare Function ReleaseDC Lib "user32" Alias "ReleaseDC" (ByVal hwnd As Integer, ByVal hdc As Integer) As Integer

    Public Sub New(ByVal LogPath As String)
        InitializeComponent()
        strLogFile = LogPath
    End Sub

    Private Sub ErrorReporter_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        strLogFile &= "ErrorLog" & Now.ToString("yyyyMMdd-HHmm") & ".txt"

        Me.Focus()
        Me.Cursor = Cursors.WaitCursor
        txtErrorText.Text = ErrorException.Message
        lblStatus.Text = "Collecting Data..."
        Application.DoEvents()

        sb.AppendLine(My.Application.Info.ProductName & " " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build)
        sb.AppendLine(Now)
        sb.AppendLine(ErrorException.ToString)
        sb.AppendLine()

        'Event viewer
        AttachFiles("*.ini", 30000)
        AttachFiles("*.xml", 50000)
        AttachFiles("*Log.txt", 30000)
        'EventLog("AutoVer", 100) 'move list to .config file
        GetDriveWindowsInfo()
        'Screenshot()
        sb.AppendLine("Hardware ============================================================================================================")
        Management("BIOS")
        Management("Processor")
        Management("PhysicalMemory")
        sb.AppendLine()
        Management("NetworkAdapterConfiguration")
        sb.AppendLine()
        NetworkConnections()
        GetProcesses()
        StartUp()
        'InstalledPrograms()
        sb.AppendLine("END ============================================================================================================")

        WriteLog()
        Me.Cursor = Cursors.Default
        lblStatus.Text = String.Empty
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNo.Click
        Application.Exit()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.Cursor = Cursors.WaitCursor
        lblStatus.Text = "Sending..."
        Application.DoEvents()
        'Dim strImageFile As String = strLogFile.Substring(0, strLogFile.Length - 3) & "jpg"
        'Dim Fs As New System.IO.FileStream(strImageFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)
        'Dim bytFile(Fs.Length - 1) As Byte
        'Fs.Read(bytFile, 0, Fs.Length)
        'Fs.Close()
        If Not DetailsInserted And (txtNameEmail.Text.Trim <> String.Empty Or txtRecreate.Text.Trim <> String.Empty) Then
            DetailsInserted = True
            sb.Insert(sb.ToString.IndexOf(vbCrLf, StringComparison.Ordinal), vbCrLf & txtNameEmail.Text & vbCrLf & txtRecreate.Text)
            WriteLog()
        End If

        Try
            Dim wsBeanWeb As New beanlandnetau.BeanAppService
            wsBeanWeb.ReportError("AutoVer." & Windows.Forms.SystemInformation.ComputerName, txtNameEmail.Text, sb.ToString, Nothing)

            Me.Cursor = Cursors.Default
            Application.Exit()
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            lblStatus.Text = "Error connecting to web site"
        End Try
    End Sub

    Private Sub lklErrorReport_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lklErrorReport.LinkClicked
        Me.Cursor = Cursors.WaitCursor
        Diagnostics.Process.Start(strLogFile)
        Me.Cursor = Cursors.Default
    End Sub

    '****************************************************************************************************************************

    Private Sub AttachFiles(ByVal strFileSpec As String, ByVal lngLastXBytes As Long)
        'Attach last x bytes of all files matching the spec to the output
        Dim strFiles() As String = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\AutoVer\", strFileSpec)
        Dim reader As StreamReader = Nothing
        For Each strFile As String In strFiles
            If Not strFile.StartsWith("Error") Then
                Try
                    'read
                    sb.AppendLine(New FileInfo(strFile).Name & " ============================================================================================================")
                    reader = New StreamReader(strFile, True)
                    If reader.BaseStream.Length > lngLastXBytes Then
                        sb.Append("[Truncated]... ")
                        reader.BaseStream.Seek(reader.BaseStream.Length - lngLastXBytes, SeekOrigin.Begin)
                    End If
                    While Not reader.EndOfStream
                        sb.Append(reader.ReadToEnd)
                    End While
                    sb.AppendLine()
                Catch ex As Exception
                    sb.AppendLine(ex.Message)
                Finally
                    If Not IsNothing(reader) Then reader.Close()
                End Try
            End If
        Next
        If strFiles.Length > 0 Then sb.AppendLine()
    End Sub

    Sub EventLog(ByVal strLogNameStartsWith As String, ByVal LastXEvents As Integer)
        'Read Windows Event log
        strLogNameStartsWith = strLogNameStartsWith.ToLower
        Try
            Dim objEventLog, EventLogs() As EventLog
            Dim objEntry As EventLogEntry
            EventLogs = Diagnostics.EventLog.GetEventLogs
            For Each objEventLog In EventLogs
                If objEventLog.Log.ToString.ToLower.StartsWith(strLogNameStartsWith) Then
                    sb.AppendLine("Event Log: " & objEventLog.Log.ToString & " =========================================================================================================")
                    For intEntry As Integer = objEventLog.Entries.Count - 1 To 0 Step -1
                        objEntry = objEventLog.Entries(intEntry)
                        If objEventLog.Entries.Count - intEntry = LastXEvents Then Exit For
                        sb.Append(objEntry.EntryType.ToString & ", ")
                        If objEntry.Category.ToString <> "(0)" Then sb.Append(objEntry.Category.ToString & ", ")
                        sb.Append(objEntry.Source.ToString & ", ")
                        sb.Append(objEntry.TimeGenerated.ToString() & ", ")
                        sb.AppendLine(objEntry.Message.Replace(String.Concat(vbCrLf, vbCrLf), vbCrLf).Replace(String.Concat(Environment.NewLine, Environment.NewLine), Environment.NewLine))
                    Next
                    sb.AppendLine()
                End If
            Next
        Catch ex As Exception
            sb.AppendLine(ex.Message)
        End Try
    End Sub

    Private Sub GetDriveWindowsInfo()
        'Windows and drive info
        sb.AppendLine("Windows ============================================================================================================")
        Try
            sb.Append("Windows Version: ")
            sb.AppendLine(Environment.OSVersion.VersionString)
            sb.Append("Windows System Folder: ")
            sb.AppendLine(Environment.GetFolderPath(Environment.SpecialFolder.System))
            sb.Append("Program Files Folder: ")
            sb.AppendLine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles))
            sb.Append("Documents Folder: ")
            sb.AppendLine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            sb.Append("Boot Mode: ")
            sb.AppendLine(SystemInformation.BootMode.ToString())
            sb.Append("Windows Up Time: ")
            Dim ts As New TimeSpan(Convert.ToInt64(Environment.TickCount) * 10000)
            sb.AppendLine(String.Format("{0}d, {1}h, {2}m, {3}s", ts.Days, ts.Hours, ts.Minutes, ts.Seconds))
            sb.Append("Screen Resolution: ")
            sb.AppendLine(SystemInformation.VirtualScreen.Width.ToString + "w x " + SystemInformation.VirtualScreen.Height.ToString + "h px")
            sb.Append("Region: ")
            sb.AppendLine(System.Globalization.RegionInfo.CurrentRegion.EnglishName & " (" & System.Globalization.CultureInfo.CurrentCulture.EnglishName & ")")
            sb.Append("User Name: ")
            sb.AppendLine(Windows.Forms.SystemInformation.UserName)
            sb.Append("Computer Name: ")
            sb.AppendLine(Windows.Forms.SystemInformation.ComputerName)
            sb.Append("Host/IP: ")
            sb.AppendLine(Net.Dns.GetHostName & " / " & Net.Dns.GetHostEntry(Net.Dns.GetHostName).AddressList(0).ToString())
            sb.AppendLine()
            sb.Append(".NET Versions: (")
            sb.AppendLine(Environment.Version.ToString & " currently running)")
            Dim componentsKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Active Setup\Installed Components")
            Dim instComps() As String = componentsKey.GetSubKeyNames()
            For Each instComp As String In instComps
                Dim key As RegistryKey = componentsKey.OpenSubKey(instComp)
                Dim friendlyName As String = key.GetValue(Nothing) ' Gets the (Default) value from this key
                If Not IsNothing(friendlyName) AndAlso friendlyName.IndexOf(".NET Framework", System.StringComparison.Ordinal) >= 0 Then
                    Dim version As String = key.GetValue("Version")
                    'if(version!=null && version.Split(',').Length>=4)
                    sb.Append(friendlyName)
                    If IsNothing(version) Then
                        sb.AppendLine()
                    Else
                        sb.AppendLine(" (" & version & ")")
                    End If
                End If
            Next
        Catch ex As Exception
            sb.AppendLine(ex.Message)
        End Try
        sb.AppendLine()

        sb.AppendLine("Drives ============================================================================================================")
        Dim Drive As System.IO.DriveInfo
        For Each Drive In System.IO.DriveInfo.GetDrives
            Try
                sb.Append("Drive Name: ")
                sb.AppendLine(Drive.Name)
                sb.Append("Drive Type: ")
                sb.AppendLine(Drive.DriveType.ToString)
                sb.Append("File System: ")
                sb.AppendLine(Drive.DriveFormat.ToString)
                sb.Append("Size: ")
                sb.AppendLine((Drive.TotalSize / (1024 * 1024)).ToString("#,###,### MB"))
                sb.Append("Free Space: ")
                sb.AppendLine((Drive.TotalFreeSpace / (1024 * 1024)).ToString("#,###,### MB"))
                sb.AppendLine(String.Empty)
            Catch ex As Exception
                sb.AppendLine(ex.Message)
            End Try
        Next
    End Sub

    Public Sub GetProcesses()
        'loading all processes on current PC 
        sb.AppendLine("Processes ============================================================================================================")
        Try
            Dim aPrc(), Prc As Process
            aPrc = System.Diagnostics.Process.GetProcesses
            For Each Prc In aPrc
                Try
                    sb.Append(Prc.ProcessName)
                    Try
                        sb.Append(", Started: ")
                        sb.Append(Prc.StartTime)
                    Catch
                    End Try
                    sb.Append(", WorkingMemory: ")
                    sb.Append((Prc.WorkingSet64 \ 1024).ToString & "KB")
                    If Not Prc.Responding Then sb.Append(", NOT Responding")
                    sb.AppendLine()
                Catch
                End Try
            Next
        Catch ex As Exception
            sb.AppendLine(ex.Message)
        End Try
        sb.AppendLine()
    End Sub

    Public Sub InstalledPrograms()
        'loading all installed programs from registry
        sb.AppendLine("Installed Programs ============================================================================================================")
        Dim Key, PR As RegistryKey
        Dim SK(), DN, XK As String
        Try
            Key = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\")
            SK = Key.GetSubKeyNames()
            For Each XK In SK
                PR = Key.OpenSubKey(XK)
                DN = PR.GetValue("DisplayName", "")
                If DN <> String.Empty Then
                    sb.Append(DN)
                    DN = PR.GetValue("Publisher", "")
                    If DN.Trim <> String.Empty Then
                        sb.Append(" (")
                        sb.Append(DN)
                        sb.Append(")")
                    End If
                    DN = PR.GetValue("DisplayVersion", "")
                    If DN.Trim <> String.Empty Then
                        sb.Append(", Ver: ")
                        sb.Append(DN)
                    End If
                    sb.AppendLine()
                End If
            Next
        Catch ex As Exception
            sb.AppendLine(ex.Message)
        End Try
        sb.AppendLine()
    End Sub

    Public Sub StartUp()
        'loading all StartUp programs from registry 
        sb.AppendLine("Startup Apps ============================================================================================================")
        Dim Key1, Key2, Key3, Key4, Key5, Key6 As RegistryKey
        Dim i1, i2, i3, i4, i5, i6 As Integer
        Dim l1, l2, l3, l4, l5, l6 As Long
        Try
            Key1 = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
            Key2 = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
            Key3 = Registry.Users.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
            Key4 = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\RunOnce", True)
            Key5 = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\RunOnce", True)
            Key6 = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\RunOnceEx", True)
            Try
                l1 = Key1.ValueCount
                For i1 = 0 To l1 - 1
                    sb.Append(Key1.GetValueNames.GetValue(i1))
                    sb.AppendLine(" - HKLM\Run")
                Next
            Catch ex As Exception
            End Try
            Try
                l2 = Key2.ValueCount
                For i2 = 0 To l2 - 1
                    sb.Append(Key2.GetValueNames.GetValue(i2))
                    sb.AppendLine(" - HKCU\Run")
                Next
            Catch ex As Exception
            End Try
            Try
                l3 = Key3.ValueCount
                For i3 = 0 To l3 - 1
                    sb.Append(Key3.GetValueNames.GetValue(i3))
                    sb.AppendLine(" - HKUser\Run")
                Next
            Catch ex As Exception
            End Try
            Try
                l4 = Key4.ValueCount
                For i4 = 0 To l4 - 1
                    sb.Append(Key4.GetValueNames.GetValue(i4))
                    sb.AppendLine(" - HKCU\RunOnce")
                Next
            Catch ex As Exception
            End Try
            Try
                l5 = Key5.ValueCount
                For i5 = 0 To l5 - 1
                    sb.Append(Key5.GetValueNames.GetValue(i5))
                    sb.AppendLine(" - HKLM\RunOnce")
                Next
            Catch ex As Exception
            End Try
            Try
                l6 = Key6.ValueCount
                For i6 = 0 To l6 - 1
                    sb.Append(Key6.GetValueNames.GetValue(i6))
                    sb.AppendLine(" - HKLM\RunOnceEx")
                Next
            Catch ex As Exception
            End Try
        Catch ex As Exception
            sb.AppendLine(ex.Message)
        End Try
        sb.AppendLine()
    End Sub

    Private Function GetEncoderInfo(ByVal strMimeType As String) As Imaging.ImageCodecInfo
        'returns ImageCodecInfo for the specified MIME type
        Dim j As Integer
        Dim objImageCodecInfo() As Imaging.ImageCodecInfo
        objImageCodecInfo = Imaging.ImageCodecInfo.GetImageEncoders()
        j = 0
        While j < objImageCodecInfo.Length
            If objImageCodecInfo(j).MimeType = strMimeType Then
                Return objImageCodecInfo(j)
            End If
            j += 1
        End While
        Return Nothing
    End Function

    Private Sub BitmapToJPEG(ByVal objBitmap As Bitmap, ByVal strFilename As String, Optional ByVal lngCompression As Long = 75)
        'save bitmap object to JPEG of specified quality level
        Dim objEncoderParameters As New Imaging.EncoderParameters(1)
        Dim objImageCodecInfo As Imaging.ImageCodecInfo = GetEncoderInfo("image/jpeg")

        objEncoderParameters.Param(0) = New Imaging.EncoderParameter(Imaging.Encoder.Quality, lngCompression)
        objBitmap.Save(strFilename, objImageCodecInfo, objEncoderParameters)
    End Sub

    Private Sub Screenshot()
        'Take a screenshot of the desktop and saves it as a jpg
        Dim objRectangle As Rectangle = Screen.PrimaryScreen.Bounds
        Dim objBitmap As New Bitmap(objRectangle.Right, objRectangle.Bottom)
        Dim objGraphics As Graphics
        Dim hdcDest As IntPtr
        Dim hdcSrc As Integer
        Const SRCCOPY As Integer = &HCC0020
        objGraphics = Graphics.FromImage(objBitmap)
        '-- get a device context to the windows desktop and our destination  bitmaps
        hdcSrc = GetDC(0)
        hdcDest = objGraphics.GetHdc
        '-- copy what is on the desktop to the bitmap
        BitBlt(hdcDest.ToInt32, 0, 0, objRectangle.Right, objRectangle.Bottom, hdcSrc, 0, 0, SRCCOPY)
        '-- release device contexts
        objGraphics.ReleaseHdc(hdcDest)
        ReleaseDC(0, hdcSrc)
        BitmapToJPEG(objBitmap, strLogFile.Substring(0, strLogFile.Length - 3) & "jpg", 75)
        'objBitmap.Save(Environment.CurrentDirectory & "\ErrorLog" & Now.ToString("yyyyMMdd-HHmm") & ".jpg", Imaging.ImageFormat.Jpeg)
    End Sub

    Private Sub Management(ByVal inf As String)
        'loading informations from management
        Dim count As Integer = 0
        Dim cap As Integer = 0
        Try
            Dim Men As ManagementObject
            Dim MemoClass As New ManagementClass("Win32_" & inf)
            Dim Memo As ManagementObjectCollection = MemoClass.GetInstances()
            Dim MOC As ManagementObjectCollection = MemoClass.GetInstances()
            Dim MemoMenu As ManagementObjectCollection.ManagementObjectEnumerator = Memo.GetEnumerator
            MemoMenu.MoveNext()
            Select Case inf
                Case "BIOS"
                    sb.Append("Manufacturer/Model: ")
                    sb.Append(MemoMenu.Current.Properties("Manufacturer").Value.ToString())
                    sb.Append(", ")
                    MemoClass = New ManagementClass("Win32_ComputerSystem")
                    Memo = MemoClass.GetInstances()
                    MemoMenu = Memo.GetEnumerator
                    MemoMenu.MoveNext()
                    sb.AppendLine(MemoMenu.Current.Properties("Model").Value.ToString())
                Case "Processor"
                    sb.Append("Name: ")
                    sb.AppendLine(MemoMenu.Current.Properties("Name").Value.ToString.Trim())
                Case "PhysicalMemory"
                    For Each Men In Memo
                        count = count + 1
                        cap = cap + (Convert.ToInt64(Men("capacity")) / (1024 * 1024))
                    Next
                    sb.Append("RAM: ")
                    sb.AppendLine(cap & " MB")
                    MemoClass = New ManagementClass("Win32_OperatingSystem")
                    Memo = MemoClass.GetInstances()
                    MemoMenu = Memo.GetEnumerator
                    MemoMenu.MoveNext()
                    sb.Append("Total Visible Memory Size: ")
                    sb.AppendLine(FormatNumber((MemoMenu.Current.Properties("TotalVisibleMemorySize").Value / 1024).ToString, 0) & " MB")
                    sb.Append("Free Physical Memory: ")
                    sb.AppendLine(FormatNumber(MemoMenu.Current.Properties("FreePhysicalMemory").Value.ToString, 0) & " KB")
                Case "NetworkAdapterConfiguration"
                    For Each Men In Memo
                        Try
                            If Men("IPAddress").Length > 0 And Men("IPSubnet").Length > 0 Then
                                sb.Append("NIC: ")
                                sb.AppendLine(Men("Description"))
                                If Men("IPAddress").Length > 0 Then sb.Append("IPAddress: ")
                                'Dim strItems As String = String.Empty
                                For Each strItem As String In Men("IPAddress")
                                    If strItem.Trim <> String.Empty Then sb.AppendLine(strItem)
                                Next
                                If Men("IPSubnet").Length > 0 Then sb.Append("IPSubnet: ")
                                'strItems = String.Empty
                                For Each strItem As String In Men("IPSubnet")
                                    If strItem.Trim <> String.Empty Then sb.AppendLine(strItem)
                                Next
                                sb.AppendLine()
                            End If
                        Catch

                        End Try
                    Next
            End Select
        Catch ex As Exception
            sb.AppendLine(ex.Message)
        End Try
    End Sub

    Public Sub NetworkConnections()
        'loading informations about network connections
        sb.AppendLine("Network Connections ============================================================================================================")
        Dim int As Integer = &H2S
        Dim flags As Integer
        Try
            Call InternetGetConnectedState(flags, 0)
            sb.Append("LAN: ")
            sb.AppendLine(CStr(flags And int))
            sb.Append("Internet: ")
            sb.AppendLine(CStr(InternetGetConnectedState(0, 0)))
            sb.Append("Proxy: ")
            int = &H4S
            Call InternetGetConnectedState(flags, 0)
            sb.AppendLine(CStr(flags And int))
        Catch ex As Exception
            sb.AppendLine(ex.Message)
        End Try
        sb.AppendLine()
    End Sub

    Private Sub WriteLog()
        Dim Fs As FileStream
        Dim writer As StreamWriter
        Try
            'Write
            Fs = New FileStream(strLogFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
            writer = New StreamWriter(Fs)
            writer.WriteLine(sb.ToString)
            writer.Flush()
            writer.Close()
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("FileWriter: " & ex.Message)
        End Try
    End Sub

End Class
