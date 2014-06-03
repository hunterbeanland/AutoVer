Imports System.Collections.Generic
Imports System.Net
'Imports System.IO
Imports System.Text.RegularExpressions
Imports Alphaleonis.Win32.Filesystem

Namespace Utilities.FTP

#Region "FTP client class"
    ''' <summary>
    ''' A wrapper class for .NET 2.0 FTP
    ''' </summary>
    ''' <remarks>
    ''' Version 1.2
    ''' 
    ''' Currently this class does not hold open an FTP connection but 
    ''' instead is stateless: for each FTP request it connects, performs the request and disconnects.
    ''' 
    ''' v1.0 original version
    ''' 
    ''' v1.1 added support for EnableSSL, UsePassive and Proxy connections
    '''    Also added support for downloading correct date/time from FTP server for each file
    '''    Added FtpDirectoryExists function to check for existence of remote directory
    ''' 
    ''' </remarks>
    Public Class FTPclient

#Region "CONSTRUCTORS"
        ''' <summary>
        ''' Blank constructor
        ''' </summary>
        ''' <remarks>Hostname, username and password must be set manually</remarks>
        Sub New()
        End Sub

        ''' <summary>
        ''' Constructor just taking the hostname
        ''' </summary>
        ''' <param name="Hostname">in either ftp://ftp.host.com or ftp.host.com form</param>
        ''' <remarks></remarks>
        Sub New(ByVal Hostname As String)
            _hostname = Hostname
        End Sub

        ''' <summary>
        ''' Constructor taking hostname, username and password
        ''' </summary>
        ''' <param name="Hostname">in either ftp://ftp.host.com or ftp.host.com form</param>
        ''' <param name="Username">Leave blank to use 'anonymous' but set password to your email</param>
        ''' <param name="Password">Password for username</param>
        ''' <param name="KeepAlive">[optional] keep connections alive between requests (v1.1)</param>
        ''' <remarks></remarks>
        Sub New(ByVal Hostname As String, ByVal Username As String, ByVal Password As String, Optional ByVal KeepAlive As Boolean = False)
            _hostname = Hostname
            _username = Username
            _password = Password
        End Sub
#End Region

#Region "Directory functions"
        ''' <summary>
        ''' Return a simple directory listing
        ''' </summary>
        ''' <param name="directory">Directory to list, e.g. /pub</param>
        ''' <returns>A list of filenames and directories as a List(of String)</returns>
        ''' <remarks>For a detailed directory listing, use ListDirectoryDetail</remarks>
        Public Function ListDirectory(Optional ByVal directory As String = "") As List(Of String)
            'return a simple list of filenames in directory
            Dim ftp As Net.FtpWebRequest = GetRequest(GetDirectory(directory))
            'Set request to do simple list
            ftp.Method = Net.WebRequestMethods.Ftp.ListDirectory

            Dim str As String = GetStringResponse(ftp)
            'replace CRLF to CR, remove last instance
            str = str.Replace(vbCrLf, vbCr).TrimEnd(Chr(13))
            'split the string into a list
            Dim result As New List(Of String)
            result.AddRange(str.Split(Chr(13)))
            Return result
        End Function

        ''' <summary>
        ''' Return a detailed directory listing
        ''' </summary>
        ''' <param name="directory">Directory to list, e.g. /pub/etc</param>
        ''' <param name="doDateTimeStamp">Boolean: set to True to also download the file date/time stamps</param>
        ''' <returns>An FTPDirectory object</returns>
        Public Function ListDirectoryDetail(Optional ByVal directory As String = "", _
                Optional ByVal doDateTimeStamp As Boolean = False) As FTPdirectory
            Dim ftp As Net.FtpWebRequest = GetRequest(GetDirectory(directory))
            'Set request to do simple list
            ftp.Method = Net.WebRequestMethods.Ftp.ListDirectoryDetails

            Dim str As String = GetStringResponse(ftp)
            'replace CRLF to CR, remove last instance
            str = str.Replace(vbCrLf, vbCr).TrimEnd(Chr(13))
            'split the string into a list
            Dim dir As New FTPdirectory(str, _lastDirectory)

            'Download timestamps if requested?
            If doDateTimeStamp Then
                For Each fi As FTPfileInfo In dir
                    fi.FileDateTime = Me.GetDateTimestamp(fi)
                Next
            End If

            Return dir
        End Function

#End Region

#Region "Upload: File transfer TO ftp server"
        ''' <summary>
        ''' Copy a local file to the FTP server (local filename as string)
        ''' </summary>
        ''' <param name="localFilename">Full path of the local file</param>
        ''' <param name="targetFilename">Target filename, if required</param>
        ''' <returns></returns>
        ''' <remarks>If the target filename is blank, the source filename is used
        ''' (assumes current directory). Otherwise use a filename to specify a name
        ''' or a full path and filename if required.</remarks>
        Public Function Upload(ByVal localFilename As String, Optional ByVal targetFilename As String = "") As Boolean
            '1. check source
            If Not File.Exists(localFilename) Then
                Throw New ApplicationException("File " & localFilename & " not found")
            End If
            'copy to FI
            Dim fi As New FileInfo(localFilename)
            Return Upload(fi, targetFilename)
        End Function

        '''' <summary>
        '''' Upload a local file to the FTP server (local file as IO.FileInfo)
        '''' </summary>
        '''' <param name="fi">Source file</param>
        '''' <param name="targetFilename">Target filename (optional)</param>
        '''' <returns></returns>
        'Public Function Upload(ByVal fi As FileInfo, Optional ByVal targetFilename As String = "") As Boolean
        '    'copy the file specified to target file: target file can be full path or just filename (uses current dir)

        '    '1. check target
        '    Dim target As String
        '    If targetFilename.Trim = "" Then
        '        'Blank target: use source filename & current dir
        '        target = Me.CurrentDirectory & fi.Name
        '    ElseIf targetFilename.Contains("/") Then
        '        'If contains / treat as a full path
        '        target = AdjustDir(targetFilename)
        '    Else
        '        'otherwise treat as filename only, use current directory
        '        target = CurrentDirectory & targetFilename
        '    End If

        '    Dim URI As String = Hostname & target
        '    'perform copy
        '    Dim ftp As Net.FtpWebRequest = GetRequest(URI)

        '    'Set request to upload a file in binary
        '    ftp.Method = Net.WebRequestMethods.Ftp.UploadFile
        '    ftp.UseBinary = True

        '    'Notify FTP of the expected size
        '    ftp.ContentLength = fi.Length

        '    'create byte array to store: ensure at least 1 byte!
        '    Const BufferSize As Integer = 2048
        '    Dim content(BufferSize - 1) As Byte, dataRead As Integer

        '    'open file for reading 
        '    Using fs As FileStream = fi.OpenRead()
        '        Try
        '            'open request to send
        '            Using rs As Stream = ftp.GetRequestStream
        '                Do
        '                    dataRead = fs.Read(content, 0, BufferSize)
        '                    rs.Write(content, 0, dataRead)
        '                Loop Until dataRead < BufferSize
        '                rs.Close()
        '            End Using
        '        Catch ex As Exception

        '        Finally
        '            'ensure file closed
        '            fs.Close()
        '        End Try

        '    End Using

        '    ftp = Nothing
        '    Return True

        'End Function

        ''' <summary>
        ''' Upload a local file to the FTP server
        ''' </summary>
        ''' <param name="fi">Source file</param>
        ''' <param name="targetFilename">Target filename (optional)</param>
        ''' <returns></returns>
        Public Function Upload(ByVal fi As FileInfo, ByVal targetFilename As String) As Boolean
            'copy the file specified to target file: target file can be full path or just filename (uses current dir)
            '1. check target
            Dim target As String
            If targetFilename.Trim() = "" Then
                'Blank target: use source filename & current dir
                target = Me.CurrentDirectory + fi.Name
            ElseIf targetFilename.Contains("/") Then
                'If contains / treat as a full path
                target = AdjustDir(targetFilename)
            Else
                'otherwise treat as filename only, use current directory
                target = CurrentDirectory + targetFilename
            End If
            Using fs As System.IO.FileStream = fi.OpenRead()
                Try
                    Return Upload(fs, target)

                Catch ex As Exception
                    ErrorText = ex.Message
                    System.Diagnostics.Trace.WriteLine(ex.ToString())

                Finally

                    'ensure file closed
                    fs.Close()
                End Try
            End Using

            Return False
        End Function

        ''' <summary>
        ''' Upload a source stream to the FTP server 
        ''' </summary>
        ''' <param name="sourceStream">Source Stream</param>
        ''' <param name="targetFilename">Target filename</param>
        ''' <returns></returns>
        Public Function Upload(ByVal sourceStream As System.IO.Stream, ByVal targetFilename As String) As Boolean
            'validate target file
            Dim target As String
            If String.IsNullOrEmpty(targetFilename) Then
                Throw New ApplicationException("Target filename must be specified")
            ElseIf targetFilename.Contains("/") Then
                'If contains / treat as a full path
                target = AdjustDir(targetFilename)
            Else
                'otherwise treat as filename only, use current directory
                target = CurrentDirectory & targetFilename
            End If

            'Build full target URL
            Dim URI As String = Hostname & target
            'perform copy
            Dim ftp As System.Net.FtpWebRequest = GetRequest(URI)

            'Set request to upload a file in binary
            ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile
            ftp.UseBinary = True

            'Notify FTP of the expected size
            ftp.ContentLength = sourceStream.Length

            'create byte array to store: ensure at least 1 byte!
            Const BufferSize As Integer = 2048
            Dim content(BufferSize) As Byte
            Dim dataRead As Integer

            'open file for reading
            Using sourceStream

                Try

                    sourceStream.Position = 0
                    'open request to send
                    Using rs As System.IO.Stream = ftp.GetRequestStream()
                        Do
                            dataRead = sourceStream.Read(content, 0, BufferSize)
                            rs.Write(content, 0, dataRead)
                        Loop Until (dataRead < BufferSize)
                        rs.Close()
                    End Using

                    Return True

                Catch ex As Exception
                    ErrorText = ex.Message
                Finally
                    'ensure file closed
                    sourceStream.Close()
                    ftp = Nothing
                End Try
            End Using

            Return False
        End Function


#End Region

#Region "Download: File transfer FROM ftp server"
        ''' <summary>
        ''' Copy a file from FTP server to local
        ''' </summary>
        ''' <param name="sourceFilename">Target filename, if required</param>
        ''' <param name="localFilename">Full path of the local file</param>
        ''' <returns></returns>
        ''' <remarks>Target can be blank (use same filename), or just a filename
        ''' (assumes current directory) or a full path and filename</remarks>
        Public Function Download(ByVal sourceFilename As String, ByVal localFilename As String, Optional ByVal PermitOverwrite As Boolean = False) As Boolean
            '2. determine target file
            Dim fi As New FileInfo(localFilename)
            Return Me.Download(sourceFilename, fi, PermitOverwrite)
        End Function

        'Version taking an FtpFileInfo
        Public Function Download(ByVal file As FTPfileInfo, ByVal localFilename As String, Optional ByVal PermitOverwrite As Boolean = False) As Boolean
            Return Me.Download(file.FullName, localFilename, PermitOverwrite)
        End Function

        'Another version taking FtpFileInfo and FileInfo
        Public Function Download(ByVal file As FTPfileInfo, ByVal localFI As FileInfo, Optional ByVal PermitOverwrite As Boolean = False) As Boolean
            Return Me.Download(file.FullName, localFI, PermitOverwrite)
        End Function

        'Version taking string/FileInfo
        Public Function Download(ByVal sourceFilename As String, ByVal targetFI As FileInfo, Optional ByVal PermitOverwrite As Boolean = False) As Boolean
            '1. check target
            If targetFI.Exists And Not (PermitOverwrite) Then Throw New ApplicationException("Target file already exists")

            '2. check source
            Dim target As String
            If sourceFilename.Trim = "" Then
                Throw New ApplicationException("File not specified")
            ElseIf sourceFilename.Contains("/") Then
                'treat as a full path
                target = AdjustDir(sourceFilename)
            Else
                'treat as filename only, use current directory
                target = CurrentDirectory & sourceFilename
            End If

            Dim URI As String = Hostname & target

            '3. perform copy
            Dim ftp As Net.FtpWebRequest = GetRequest(URI)

            'Set request to download a file in binary mode
            ftp.Method = Net.WebRequestMethods.Ftp.DownloadFile
            ftp.UseBinary = True

            'open request and get response stream
            Using response As FtpWebResponse = CType(ftp.GetResponse, FtpWebResponse)
                Using responseStream As System.IO.Stream = response.GetResponseStream
                    'loop to read & write to file
                    Using fs As System.IO.FileStream = targetFI.OpenWrite
                        Try
                            Dim buffer(2047) As Byte
                            Dim read As Integer = 0
                            Do
                                read = responseStream.Read(buffer, 0, buffer.Length)
                                fs.Write(buffer, 0, read)
                            Loop Until read = 0
                            responseStream.Close()
                            fs.Flush()
                            fs.Close()
                        Catch ex As Exception
                            'catch error and delete file only partially downloaded
                            fs.Close()
                            'delete target file as it's incomplete
                            targetFI.Delete()
                            ErrorText = ex.Message
                            Throw
                        End Try
                    End Using
                    responseStream.Close()
                End Using
                response.Close()
            End Using

            Return True
        End Function
#End Region

#Region "Other functions: Delete rename etc."
        ''' <summary>
        ''' Delete remote file
        ''' </summary>
        ''' <param name="filename">filename or full path</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FtpDelete(ByVal filename As String) As Boolean
            'Determine if file or full path
            Dim URI As String = Me.Hostname & GetFullPath(filename)

            Dim ftp As Net.FtpWebRequest = GetRequest(URI)
            'Set request to delete
            ftp.Method = Net.WebRequestMethods.Ftp.DeleteFile
            Try
                'get response but ignore it
                Dim str As String = GetStringResponse(ftp)
            Catch ex As Exception
                ErrorText = ex.Message
                Return False
            End Try
            Return True
        End Function

        ''' <summary>
        ''' Determine if file exists on remote FTP site
        ''' </summary>
        ''' <param name="filename">Filename (for current dir) or full path</param>
        ''' <returns></returns>
        ''' <remarks>Note this only works for files</remarks>
        Public Function FtpFileExists(ByVal filename As String) As Boolean
            'Try to obtain filesize: if we get error msg containing "550"
            'the file does not exist
            Try
                Dim size As Long = GetFileSize(filename)
                Return True

            Catch webex As System.Net.WebException
                'Dir not exists message is 550
                If webex.Message.Contains("550") Then Return False
                'other errors - fail
                Throw

            Catch
                'all other errors - throw
                Throw
            End Try
        End Function

        ''' <summary>
        ''' Determine if a directory exists on remote ftp server
        ''' </summary>
        ''' <param name="remoteDir">Directory path, e.g. /pub/test</param>
        ''' <returns>True if directory exists, otherwise false</returns>
        ''' <remarks></remarks>
        Public Function FtpDirectoryExists(ByVal remoteDir As String) As Boolean
            Try
                'Attempt directory listing - if it fails we catch the exception
                Dim files As List(Of String) = Me.ListDirectory(remoteDir)
                Return True

            Catch webex As System.Net.WebException
                'Should contain 550 error code if directory not found
                If webex.Message.Contains("550") Then Return False
                'all other errors, throw
                Throw

            Catch ex As Exception
                Throw
            End Try
        End Function

        ''' <summary>
        ''' Determine size of remote file
        ''' </summary>
        ''' <param name="filename"></param>
        ''' <returns></returns>
        ''' <remarks>Throws an exception if file does not exist</remarks>
        Public Function GetFileSize(ByVal filename As String) As Long
            Dim path As String
            If filename.Contains("/") Then
                path = AdjustDir(filename)
            Else
                path = Me.CurrentDirectory & filename
            End If
            Dim URI As String = Me.Hostname & path
            Dim ftp As Net.FtpWebRequest = GetRequest(URI)
            'Try to get info on file/dir?
            ftp.Method = Net.WebRequestMethods.Ftp.GetFileSize
            Dim tmp As String = Me.GetStringResponse(ftp)
            Return GetSize(ftp)
        End Function

        ''' <summary>
        ''' Rename a remote file on FTP server
        ''' </summary>
        ''' <param name="sourceFilename">Full/partial remote file </param>
        ''' <param name="newName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Source and target names should both be same type (partial or full paths)
        ''' </remarks>
        Public Function FtpRename(ByVal sourceFilename As String, ByVal newName As String) As Boolean
            'Does file exist?
            Dim source As String = GetFullPath(sourceFilename)
            If Not FtpFileExists(source) Then
                Throw New System.IO.FileNotFoundException("File " & source & " not found")
            End If

            'build target name, ensure it does not exist
            Dim target As String = GetFullPath(newName)
            If target = source Then
                Throw New ApplicationException("Source and target are the same")
            ElseIf FtpFileExists(target) Then
                Throw New ApplicationException("Target file " & target & " already exists")
            End If

            'perform rename
            Dim URI As String = Me.Hostname & source

            Dim ftp As Net.FtpWebRequest = GetRequest(URI)
            'Set request to delete
            ftp.Method = Net.WebRequestMethods.Ftp.Rename
            ftp.RenameTo = target
            Try
                'get response but ignore it
                Dim str As String = GetStringResponse(ftp)
            Catch ex As Exception
                ErrorText = ex.Message
                Return False
            End Try
            Return True
        End Function

        ''' <summary>
        ''' Create a directory on remote FTP server
        ''' </summary>
        ''' <param name="dirpath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FtpCreateDirectory(ByVal dirpath As String) As Boolean
            'perform create
            Dim URI As String = Me.Hostname & AdjustDir(dirpath)
            Dim ftp As Net.FtpWebRequest = GetRequest(URI)
            'Set request to MkDir
            ftp.Method = Net.WebRequestMethods.Ftp.MakeDirectory
            Try
                'get response but ignore it
                Dim str As String = GetStringResponse(ftp)
            Catch ex As Exception
                ErrorText = ex.Message
                Return False
            End Try
            Return True
        End Function

        ''' <summary>
        ''' Delete a directory on remote FTP server
        ''' </summary>
        ''' <param name="dirpath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FtpDeleteDirectory(ByVal dirpath As String) As Boolean
            'perform remove
            Dim URI As String = Me.Hostname & AdjustDir(dirpath)
            Dim ftp As Net.FtpWebRequest = GetRequest(URI)
            'Set request to RmDir
            ftp.Method = Net.WebRequestMethods.Ftp.RemoveDirectory
            Try
                'get response but ignore it
                Dim str As String = GetStringResponse(ftp)
            Catch ex As Exception
                ErrorText = ex.Message
                Return False
            End Try
            Return True
        End Function

        ''' <summary>
        ''' Obtain datetimestamp for remote file
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDateTimestamp(ByVal file As FTPfileInfo) As Date
            Dim result As Date
            'Hunter> Add Try
            Try
                result = Me.GetDateTimestamp(file.Filename)
            Catch
                result = Date.MinValue
            End Try
            file.FileDateTime = result
            Return result
        End Function

        ''' <summary>
        ''' Obtain datetimestamp for remote file
        ''' </summary>
        ''' <param name="filename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDateTimestamp(ByVal filename As String) As DateTime
            Dim path As String
            If filename.Contains("/") Then
                path = AdjustDir(filename)
            Else
                path = Me.CurrentDirectory & filename
            End If
            Dim URI As String = Me.Hostname & path
            Dim ftp As Net.FtpWebRequest = GetRequest(URI)
            'Try to get info on file/dir?
            ftp.Method = Net.WebRequestMethods.Ftp.GetDateTimestamp
            Return Me.GetLastModified(ftp)
        End Function

#End Region

#Region "private supporting fns"
        'Get the basic FtpWebRequest object with the
        'common settings and security
        Private Function GetRequest(ByVal URI As String) As FtpWebRequest
            'create request
            Dim result As FtpWebRequest = CType(FtpWebRequest.Create(URI), FtpWebRequest)
            'Set the login details
            result.Credentials = GetCredentials()
            'Set SSL state 
            result.EnableSsl = EnableSSL
            'determine if connection should be kept alive
            result.KeepAlive = KeepAlive
            'set passivity? not supported though
            result.UsePassive = UsePassive
            'Support for proxy setttings
            result.Proxy = Proxy

            Return result
        End Function


        ''' <summary>
        ''' Get the credentials from username/password
        ''' </summary>
        Private Function GetCredentials() As Net.ICredentials
            Return New Net.NetworkCredential(Username, Password)
        End Function

        ''' <summary>
        ''' returns a full path using CurrentDirectory for a relative file reference
        ''' </summary>
        Private Function GetFullPath(ByVal file As String) As String
            If file.Contains("/") Then
                Return AdjustDir(file)
            Else
                Return Me.CurrentDirectory & file
            End If
        End Function

        ''' <summary>
        ''' Amend an FTP path so that it always starts with /
        ''' </summary>
        ''' <param name="path">Path to adjust</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AdjustDir(ByVal path As String) As String
            Return CStr(IIf(path.StartsWith("/"), "", "/")) & path
        End Function

        Private Function GetDirectory(Optional ByVal directory As String = "") As String
            Dim URI As String
            If directory = "" Then
                'build from current
                URI = Hostname & Me.CurrentDirectory
                _lastDirectory = Me.CurrentDirectory
            Else
                If Not directory.StartsWith("/") Then Throw New ApplicationException("Directory should start with /")
                URI = Me.Hostname & directory
                _lastDirectory = directory
            End If
            Return URI
        End Function

        'stores last retrieved/set directory
        Private _lastDirectory As String = ""

        ''' <summary>
        ''' Obtains a response stream as a string
        ''' </summary>
        ''' <param name="ftp">current FTP request</param>
        ''' <returns>String containing response</returns>
        ''' <remarks>
        ''' FTP servers typically return strings with CR and
        ''' not CRLF. Use respons.Replace(vbCR, vbCRLF) to convert
        ''' to an MSDOS string
        ''' </remarks>
        Private Function GetStringResponse(ByVal ftp As FtpWebRequest) As String
            'Get the result, streaming to a string
            Dim result As String = ""
            Using response As FtpWebResponse = CType(ftp.GetResponse, FtpWebResponse)
                Dim size As Long = response.ContentLength
                Using datastream As System.IO.Stream = response.GetResponseStream
                    Using sr As New System.IO.StreamReader(datastream, System.Text.Encoding.UTF8)
                        result = sr.ReadToEnd()
                        sr.Close()
                    End Using
                    datastream.Close()
                End Using
                response.Close()
            End Using
            Return result
        End Function

        ''' <summary>
        ''' Gets the size of an FTP request
        ''' </summary>
        ''' <param name="ftp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetSize(ByVal ftp As FtpWebRequest) As Long
            Dim size As Long
            Using response As FtpWebResponse = CType(ftp.GetResponse, FtpWebResponse)
                size = response.ContentLength
                response.Close()
            End Using
            Return size
        End Function

        ''' <summary>
        ''' Internal function to get the modified datetime stamp via FTP
        ''' </summary>
        ''' <param name="ftp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetLastModified(ByVal ftp As FtpWebRequest) As Date
            Dim lastmodified As Date
            Using response As FtpWebResponse = CType(ftp.GetResponse, FtpWebResponse)
                lastmodified = response.LastModified
                response.Close()
            End Using
            Return lastmodified
        End Function
#End Region

#Region "Properties"
        ''' <summary>
        ''' Hostname
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Hostname can be in either the full URL format
        ''' ftp://ftp.myhost.com or just ftp.myhost.com
        ''' </remarks>
        Public Property Hostname() As String
            Get
                If _hostname.StartsWith("ftp://") Then
                    Return _hostname
                Else
                    Return "ftp://" & _hostname
                End If
            End Get
            Set(ByVal value As String)
                _hostname = value
            End Set
        End Property
        Private _hostname As String

        ''' <summary>
        ''' Username property
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Can be left blank, in which case 'anonymous' is returned</remarks>
        Public Property Username() As String
            Get
                Return IIf(_username = "", "anonymous", _username)
            End Get
            Set(ByVal value As String)
                _username = value
            End Set
        End Property
        Private _username As String

        ''' <summary>
        ''' Password for account
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Password() As String
            Get
                Return _password
            End Get
            Set(ByVal value As String)
                _password = value
            End Set
        End Property
        Private _password As String

        ''' <summary>
        ''' The CurrentDirectory value
        ''' </summary>
        ''' <remarks>Defaults to the root '/'</remarks>
        Public Property CurrentDirectory() As String
            Get
                'return directory, ensure it ends with /
                Return _currentDirectory & CStr(IIf(_currentDirectory.EndsWith("/"), "", "/"))
            End Get
            Set(ByVal value As String)
                If Not value.StartsWith("/") Then Throw New ApplicationException("Directory should start with /")
                _currentDirectory = value
            End Set
        End Property
        Private _currentDirectory As String = "/"

        ''' <summary>
        ''' Support for FTP over SSL
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' Please note that FTP over SSL is NOT the same
        ''' as SFTP (secure FTP). I have no FTP server for this
        ''' so have not been able to test this.
        ''' </remarks>
        Public Property EnableSSL() As Boolean
            Get
                Return _enableSSL
            End Get
            Set(ByVal value As Boolean)
                _enableSSL = value
            End Set
        End Property
        Private _enableSSL As Boolean = False

        ''' <summary>
        ''' Support for KeepAlive (reusing FTP connection)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property KeepAlive() As Boolean
            Get
                Return _keepAlive
            End Get
            Set(ByVal value As Boolean)
                _keepAlive = value
            End Set
        End Property
        Private _keepAlive As Boolean = False

        ''' <summary>
        ''' Provided for pass-through visibility but not supported/tested!
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UsePassive() As Boolean
            Get
                Return _usePassive
            End Get
            Set(ByVal value As Boolean)
                _usePassive = value
            End Set
        End Property
        Private _usePassive As Boolean = False

        ''' <summary>
        ''' Provided for pass-through visibility but not supported/tested
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Proxy() As System.Net.IWebProxy
            Get
                Return _proxy
            End Get
            Set(ByVal value As System.Net.IWebProxy)
                _proxy = value
            End Set
        End Property
        Private _proxy As IWebProxy = Nothing
        'Hunter
        Public ErrorText As String = String.Empty
#End Region

    End Class
#End Region

End Namespace
