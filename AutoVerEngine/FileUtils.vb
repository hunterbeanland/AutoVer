Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Security.Cryptography
Imports Alphaleonis.Win32
Imports System.Runtime.Serialization

Public Class FileUtils

#Region " Recycle Bin "
    Private Structure SHFILEOPTSTRUCT
        Dim hWnd As Long
        Dim wFunc As Long
        Dim pFrom As String
        Dim pTo As String
        Dim fFlags As Integer
        Dim fAnyOperationsAborted As Long
        Dim hNameMappings As Long
        Dim lpszProgressTitle As Long
    End Structure

    Private Declare Function SHFileOperation Lib "Shell32.dll" Alias "SHFileOperationA" (ByVal lpFileOp As SHFILEOPTSTRUCT) As Long

    Public Sub DeleteFileToRecycleBin(ByVal Filename As String)
        If Filename.Length > 260 Then Filename = GetLongWin32Path(Filename)
        Dim fop As New SHFILEOPTSTRUCT
        With fop
            .wFunc = &H3 'FO_DELETE
            .pFrom = Filename
            .fFlags = &H40 Or &H10 Or &H400 Or &H4 'FOF_ALLOWUNDO or FOF_NOCONFIRMATION or FOF_NOERRORUI or FOF_SILENT
        End With
        SHFileOperation(fop)
    End Sub

    'Sub DeleteFileWoDialog(ByVal FileName As String)
    '    'Imports Shell32 'Reference Microsoft Shell Controls And Animation on the COM tab.
    '    'Imports System.Runtime.InteropServices
    '    Dim Sh As New Shell
    '    Dim RecycleBin As Folder = Sh.NameSpace(10) 'ssfBITBUCKET
    '    RecycleBin.MoveHere(FileName, 1024 Or 64 Or 16 Or 4)  'Options in help.
    '    Marshal.FinalReleaseComObject(Sh)
    'End Sub
#End Region

#Region " Long Path File and Directory Functions "
    'We use AlphaFS component for most of this.
    'Refer: http://msdn.microsoft.com/en-gb/library/aa365247.aspx

    ' Private Declare Function Win32CopyFile Lib "kernel32" Alias "CopyFileA" (ByVal lpExistingFileName As String, ByVal lpNewFileName As String, ByVal bFailIfExists As Long) As Long

    Public Shared Function GetLongWin32Path(ByVal dir As String) As String
        If dir.StartsWith("\\?\") Then Return dir
        If dir.StartsWith("\\") Then Return String.Concat("\\?\UNC\", dir.Substring(2, (dir.Length - 2)))
        Return String.Concat("\\?\", dir)
    End Function

    Public Shared Function GetNormalPath(ByVal dir As String) As String
        If dir.StartsWith("\\?\UNC\") Then Return ("\\" & dir.Substring(8, (dir.Length - 8)))
        If dir.StartsWith("\\?\") Then Return dir.Substring(4, (dir.Length - 4))
        Return dir
    End Function

    ''' <summary>
    ''' If the path is long (>250 chars) then shorten it to a 8.3 path, otherwise return the regular path
    ''' </summary>
    ''' <param name="dir">Absolute/Relative/Long/Regular path</param>
    ''' <returns></returns>
    ''' <remarks>MAX_PATH = 260 (248 directory max + \ + 12 file name). Safe: 250 = 248+\+1</remarks>
    Public Shared Function Get83PathIfLong(ByVal dir As String) As String
        If dir.Length > 250 Then
            Return Filesystem.Path.GetShort83Path(dir)
        Else
            Return Filesystem.Path.GetRegularPath(dir)
        End If
    End Function

    ''' <summary>
    ''' If the path is not long (less than 250 chars) return the regular path
    ''' </summary>
    ''' <param name="dir">Absolute/Relative/Long/Regular path</param>
    ''' <returns></returns>
    ''' <remarks>MAX_PATH = 260 (248 directory max + \ + 12 file name). Safe: 250 = 248+\+1</remarks>
    Public Shared Function GetShortPathIfNotLong(ByVal dir As String) As String
        If dir.Length > 250 Then
            Return dir
        Else
            Return Filesystem.Path.GetRegularPath(dir)
        End If
    End Function

    ''' <summary>
    ''' Get the base filename excluding the extension. ie File.vb.130211.vb -> File.vb | FileNoExt.130211 -> FileNoExt
    ''' </summary>
    ''' <param name="fileName">Full/Relative path</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFileNameExExt(ByVal fileName As String) As String
        If fileName.Contains(".") Then
            fileName = fileName.Substring(0, fileName.LastIndexOf("."))
            If fileName.Contains(".") Then
                Return fileName.Substring(0, fileName.LastIndexOf("."))
            Else
                Return fileName
            End If
        Else
            Return fileName
        End If
    End Function

    'Public Shared Sub GetLastErrorAndThrowIfFailed(ByVal extra As String)
    '    If (Runtime.InteropServices.Marshal.GetLastWin32Error <> 0) Then
    '        Throw New Exception((New ComponentModel.Win32Exception(Runtime.InteropServices.Marshal.GetLastWin32Error).Message & " : " & extra))
    '    End If
    'End Sub

    'Public Function CopyFile(ByVal source As String, ByVal destination As String, ByVal overwrite As Boolean) As Boolean
    '    Dim flag As Boolean = Win32CopyFile(GetLongWin32Path(source), GetLongWin32Path(destination), Not overwrite)
    '    If Not flag Then GetLastErrorAndThrowIfFailed(GetNormalPath(source))
    '    Return flag
    'End Function

    'Public Function ZLPFileInfoToStringArray(ByRef aryZlpFiles() As FileSystem.FileInfo) As String()
    '    'Convert array of ZlpFileInfo to String
    '    If aryZlpFiles.Length = 0 Then
    '        Dim aryFilesNone(-1) As String
    '        Return aryFilesNone
    '    End If
    '    Dim aryFiles(aryZlpFiles.GetUpperBound(0)) As String
    '    Dim intIndex As Integer
    '    For Each zlpFile As Filesystem.FileInfo In aryZlpFiles
    '        aryFiles(intIndex) = zlpFile.FullName
    '        intIndex += 1
    '    Next
    '    Return aryFiles
    'End Function

#End Region

#Region " File Compare "

    ''' <summary>
    ''' Compare the content of 2 files
    ''' </summary>
    ''' <param name="filePathOne">Absolute path of primary file</param>
    ''' <param name="filePathTwo">Absolute path of backup file. Empty = no file</param>
    ''' <returns>True is identical content</returns>
    ''' <remarks></remarks>
    Public Function CompareFilesSame(ByVal filePathOne As String, ByVal filePathTwo As String) As Boolean
        Try
            'If filePathTwo.Length < 4 Or filePathTwo.Length > 260 Then Return False
            If New Filesystem.FileInfo(filePathOne).Length <> New Filesystem.FileInfo(filePathTwo).Length Then Return False
            'Calc SHA1 hash
            If ComputeFileHash(filePathOne) <> ComputeFileHash(filePathTwo) Then Return False
        Catch ex As Exception
            Dim Log As New Logger
            Log.Error(ex.Message, "ComputeFileHash")
            Return False
        End Try
        Return True
    End Function

    Private Function ComputeFileHash(ByVal filePath As String) As String
        Dim theHMACSHA1 As HMACSHA1
        Dim theKeyValue As Byte()
        Dim theHashValue As Byte()
        theKeyValue = (New Text.UnicodeEncoding).GetBytes("HashingKey")
        theHMACSHA1 = New HMACSHA1(theKeyValue, True)

        Dim inStream As IO.FileStream = Filesystem.File.OpenRead(filePath)
        'Using theInStream As New FileStream(filePath, FileMode.Open, FileAccess.Read)
        theHashValue = theHMACSHA1.ComputeHash(inStream)
        inStream.Close()
        'End Using

        'Dim SHA256 As New SHA256Managed()
        'Using theInStream As New FileStream(filePath, FileMode.Open, FileAccess.Read)
        '    theHashValue = SHA256.ComputeHash(theInStream)
        '    theInStream.Close()
        'End Using

        Return Text.Encoding.UTF8.GetString(theHashValue)
    End Function
#End Region

#Region " INI Files "


    ''' <summary>
    ''' Read INI file into a dictionary
    ''' </summary>
    ''' <param name="FilePath">Full path</param>
    ''' <param name="settings">Dictionary to update</param>
    ''' <returns>INI section names are appended to the key name: sect.key</returns>
    ''' <remarks></remarks>
    Public Shared Function ReadINI(ByRef FilePath As String, ByRef settings As Generic.Dictionary(Of String, String)) As Boolean
        If settings Is Nothing Then settings = New Generic.Dictionary(Of String, String)
        If File.Exists(FilePath) Then
            Dim rowLine As String
            Dim sectionName As String = String.Empty
            Using fs As New FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Using reader As New StreamReader(fs, System.Text.Encoding.UTF8)
                    Do While Not reader.EndOfStream
                        rowLine = reader.ReadLine.Trim
                        If rowLine.StartsWith("[") Then
                            sectionName = rowLine.Substring(1)
                            If sectionName.EndsWith("]") Then sectionName = sectionName.Substring(0, sectionName.Length - 1)
                            sectionName &= "."
                        ElseIf Not rowLine.StartsWith(";") AndAlso rowLine.Contains("=") Then
                            settings(sectionName & rowLine.Substring(0, rowLine.IndexOf("=", StringComparison.Ordinal)).TrimEnd) = Right(rowLine, rowLine.Length - rowLine.IndexOf("=", StringComparison.Ordinal) - 1)
                        End If
                    Loop
                End Using
                Return True
            End Using
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Write INI file from the dictionary
    ''' </summary>
    ''' <param name="FilePath">Full path</param>
    ''' <param name="settings">Dictionary to write</param>
    ''' <param name="HeaderComment">Comment to appear at top of file</param>
    ''' <returns>Any text before a dot (.) in the key is written as INI section: sect.key</returns>
    ''' <remarks></remarks>
    Public Shared Function WriteINI(ByRef FilePath As String, ByRef settings As Generic.Dictionary(Of String, String), ByRef HeaderComment As String) As Boolean
        Dim secName As String = String.Empty

        Dim sections As New Queue()
        For Each kvp As KeyValuePair(Of String, String) In settings
            If kvp.Key.Contains(".") Then
                secName = kvp.Key.Substring(0, kvp.Key.IndexOf("."))
                If Not sections.Contains(secName) Then
                    sections.Enqueue(secName)
                End If
            End If
        Next

        Using fs As New FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite)
            Using writer As New StreamWriter(fs, System.Text.Encoding.UTF8)
                If Not String.IsNullOrEmpty(HeaderComment) Then writer.WriteLine(";" & HeaderComment.Replace(vbCrLf, ";" & vbCrLf))

                For Each kvp As KeyValuePair(Of String, String) In settings
                    If Not kvp.Key.Contains(".") Then
                        writer.WriteLine(kvp.Key & "=" & kvp.Value)
                    End If
                Next

                Do While sections.Count > 0
                    secName = sections.Dequeue()
                    writer.WriteLine()
                    writer.WriteLine("[" & secName & "]")
                    For Each kvp As KeyValuePair(Of String, String) In settings
                        If kvp.Key.StartsWith(secName & ".") Then
                            writer.WriteLine(kvp.Key.Substring(secName.Length + 1) & "=" & kvp.Value)
                        End If
                    Next
                Loop
                writer.Flush()
            End Using
        End Using
    End Function

#End Region
End Class

<DataContractAttribute()> _
Public Class ChangeDetails
    Public ChangeType As WatcherChangeTypes
    Public FirstPath As String
    Public SecondPath As String
    Public Retries As Int16

    Public Sub New(ByVal Change As WatcherChangeTypes, ByVal First As String, ByVal Second As String)
        ChangeType = Change
        FirstPath = First
        SecondPath = Second
    End Sub
End Class

Public Class FileFolderFilter
    Private strIncludeFiles, strExcludeFiles, strExcludeFolders As String
    Private aryIncludeFiles(), aryExcludeFiles(), aryExcludeFolders() As String
    'Private Log As New Logger

    Public Sub SetupFilters(ByVal IncludeFiles As String, ByVal ExcludeFiles As String, ByVal ExcludeFolders As String)
        'Setup the filter settings before we check files/folders for a match
        strIncludeFiles = IIf(IncludeFiles.Contains(";"), FileMaskToRegEx(IncludeFiles), IncludeFiles)
        strExcludeFiles = FileMaskToRegEx(ExcludeFiles)
        strExcludeFolders = FileMaskToRegEx(ExcludeFolders)
        'Dim Log As New Logger
        'Log.Debug(strExcludeFiles, "strExcludeFiles")
        'Log.Debug(strExcludeFolders, "strExcludeFolders")
        aryIncludeFiles = strIncludeFiles.Split(New Char() {";"})
        aryExcludeFiles = strExcludeFiles.Split(New Char() {";"})
        aryExcludeFolders = strExcludeFolders.Split(New Char() {";"})
        Dim aryTemp As New ArrayList
        Dim intItem As Int16

        For intItem = 0 To aryIncludeFiles.Length - 1
            If aryIncludeFiles(intItem).Trim.Length > 0 Then aryTemp.Add(String.Concat("^", aryIncludeFiles(intItem).Trim, "$"))
        Next
        ReDim aryIncludeFiles(aryTemp.Count - 1)
        aryTemp.CopyTo(aryIncludeFiles)

        aryTemp.Clear()
        For intItem = 0 To aryExcludeFiles.Length - 1
            If aryExcludeFiles(intItem).Trim.Length > 0 Then aryTemp.Add(String.Concat("^", aryExcludeFiles(intItem).Trim, "$"))
        Next
        ReDim aryExcludeFiles(aryTemp.Count - 1)
        aryTemp.CopyTo(aryExcludeFiles)

        'Dim Log As New Logger("DEBUG", "C:\Code\AutoVer\AutoVerUI\bin\Debug\")
        aryTemp.Clear()
        For intItem = 0 To aryExcludeFolders.Length - 1
            If aryExcludeFolders(intItem).Trim.Length > 0 Then
                aryExcludeFolders(intItem) = aryExcludeFolders(intItem).Trim.Replace("/", "\")
                'aryExcludeFolders(intItem) = aryExcludeFolders(intItem).Replace("\", "\\")
                If Not aryExcludeFolders(intItem).StartsWith("\") Then
                    If aryExcludeFolders(intItem).Length > 1 Then
                        If aryExcludeFolders(intItem).Substring(2, 1) <> ":" Then aryExcludeFolders(intItem) = "\\" & aryExcludeFolders(intItem) 'Drive letter regex encoded already: c\:
                    End If
                End If
                If Not aryExcludeFolders(intItem).EndsWith("\") Then aryExcludeFolders(intItem) &= "\\"
                aryTemp.Add(aryExcludeFolders(intItem))
            End If
        Next
        ReDim aryExcludeFolders(aryTemp.Count - 1)
        aryTemp.CopyTo(aryExcludeFolders)
    End Sub

    Public Function FileMaskToRegEx(ByVal FileMask As String) As String
        'Convert Windows file mask to Regex file mask
        Dim reg As New Regex("([^\w\s\*\?;]){1}", Text.RegularExpressions.RegexOptions.IgnoreCase)
        FileMask = reg.Replace(FileMask, "\$0")
        Return FileMask.Replace("*", ".*").Replace("?", ".")
    End Function

    Public Function CanCopy(ByVal File As String, ByVal Folder As String) As Boolean
        'Apply filters
        Dim blnCopyFile As Boolean = True
        Dim intMask As Int16
        If strIncludeFiles <> "*.*" Then
            blnCopyFile = False
            For intMask = 0 To aryIncludeFiles.Length - 1
                If Regex.IsMatch(File, aryIncludeFiles(intMask), RegexOptions.IgnoreCase) Then blnCopyFile = True
            Next
        End If
        If aryExcludeFiles.Length > 0 Then
            For intMask = 0 To aryExcludeFiles.Length - 1
                If Regex.IsMatch(File, aryExcludeFiles(intMask), RegexOptions.IgnoreCase) Then blnCopyFile = False
            Next
        End If
        If aryExcludeFolders.Length > 0 Then
            Folder = Folder.Replace("/", "\") 'normalise ftp folders
            If Not Folder.StartsWith("\") And Not Path.IsPathRooted(Folder) Then Folder = String.Concat("\", Folder)
            If Not Folder.EndsWith("\") Then Folder = String.Concat(Folder, "\")
            For intMask = 0 To aryExcludeFolders.Length - 1
                If Regex.IsMatch(Folder, aryExcludeFolders(intMask), RegexOptions.IgnoreCase) Then blnCopyFile = False
            Next
        End If
        'Log.Debug(blnCopyFile.ToString, File)
        Return blnCopyFile
    End Function

    Public Function CanCopyFolder(ByVal Folder As String) As Boolean
        'Apply filters
        Dim blnCopyFile As Boolean = True
        Dim intMask As Int16
        If aryExcludeFolders.Length > 0 Then
            Folder = Folder.Replace("/", "\") 'normalise ftp folders
            If Not Folder.StartsWith("\") And Not Path.IsPathRooted(Folder) Then Folder = String.Concat("\", Folder)
            If Not Folder.EndsWith("\") Then Folder = String.Concat(Folder, "\")
            For intMask = 0 To aryExcludeFolders.Length - 1
                If Regex.IsMatch(Folder, aryExcludeFolders(intMask), RegexOptions.IgnoreCase) Then blnCopyFile = False
            Next
        End If
        'Log.Debug(blnCopyFile.ToString, File)
        Return blnCopyFile
    End Function
End Class
