Imports System.IO
Imports System.Security
Imports System.Security.Permissions
Imports System.Runtime.InteropServices

Public Class ZoneStripper

    Private _FilesScanned As Integer
    Private _ZoneIdRemoved As Integer
    Private Const ZONE_IDENTIFIER As String = "Zone.Identifier"
    Private Log As Logger

    Public Sub New(LogInstance As Logger)
        Log = LogInstance
    End Sub

    Public ReadOnly Property FilesScanned() As Integer
        Get
            Return _FilesScanned
        End Get
    End Property

    Public ReadOnly Property ZoneIdRemoved() As Integer
        Get
            Return _ZoneIdRemoved
        End Get
    End Property

    <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    Public Sub Unblock(ByVal directory As String, ByVal recurse As Boolean)

        Try
            Dim di As New DirectoryInfo(directory)

            For Each file As FileInfo In di.GetFiles()
                _FilesScanned += 1
                WipeZoneId(file.FullName)
            Next

            For Each dir As DirectoryInfo In di.GetDirectories()
                WipeZoneId(dir.FullName)
                If recurse Then
                    RecurseDirectory(dir.FullName)
                End If
            Next
        Catch ex As Exception
            Log.Error(ex.Message, "ZoneStripper.Unblock")
        End Try

    End Sub

    <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    Private Sub RecurseDirectory(ByVal dirName As String)

        Try
            Dim di As New DirectoryInfo(dirName)

            For Each file As FileInfo In di.GetFiles()
                _FilesScanned += 1
                WipeZoneId(file.FullName)
            Next
        Catch ex As Exception
            Log.Error(ex.Message, "ZoneStripper.Unblock")
        End Try

        Try
            Dim di As New DirectoryInfo(dirName)

            For Each subdir As DirectoryInfo In di.GetDirectories()
                WipeZoneId(subdir.FullName)
                RecurseDirectory(subdir.FullName)
            Next
        Catch ex As Exception
            Log.Error(ex.Message, "ZoneStripper.Unblock")
        End Try

    End Sub

    <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    Private Sub WipeZoneId(ByVal fullPathName As String)

        Try
            If StreamExists(fullPathName, ZONE_IDENTIFIER) Then
                DeleteStream(fullPathName, ZONE_IDENTIFIER)
                _ZoneIdRemoved += 1
            End If
        Catch ex As Exception
            Log.Debug(String.Format("Unable to remove Zone Identifier from {0}: {1}", fullPathName, ex.Message), "ZoneStripper.WipeZoneId")
        End Try

    End Sub


#Region "Native Methods"

    <SecurityPermission(SecurityAction.LinkDemand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    Public Shared Sub DeleteStream(ByVal path As String, ByVal stream As String)

        If Not DeleteFile(path + ":" + stream) Then
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error())
        End If

    End Sub

    Public Shared Function StreamExists(ByVal path As String, ByVal stream As String) As Boolean

        Dim handle As IntPtr

        handle = CreateFile(path + ":" + stream, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, _
        IntPtr.Zero)

        Dim retVal As Boolean = CBool(IIf(CInt(handle) = -1, False, True))
        CloseHandle(handle)
        Return retVal

    End Function


    Private Const GENERIC_READ As UInteger = 2147483648
    'Private Const GENERIC_WRITE As UInteger = 1073741824

    'Private Const CREATE_NEW As UInteger = 1
    'Private Const CREATE_ALWAYS As UInteger = 2
    Private Const OPEN_EXISTING As UInteger = 3
    'Private Const OPEN_ALWAYS As UInteger = 4
    'Private Const TRUNCATE_EXISTING As UInteger = 5

    'Private Const FILE_BEGIN As UInteger = 0
    'Private Const FILE_CURRENT As UInteger = 1
    'Private Const FILE_END As UInteger = 2

    'Private Const FILE_SHARE_NONE As UInteger = 0
    Private Const FILE_SHARE_READ As UInteger = 1


    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Private Shared Function CreateFile(ByVal fileName As String, ByVal access As UInteger, _
                                       ByVal sharemode As UInteger, ByVal security_attributes As IntPtr, _
                                       ByVal creation As UInteger, ByVal flags As UInteger, _
    ByVal template As IntPtr) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Private Shared Function CloseHandle(ByVal handle As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Private Shared Function DeleteFile(ByVal fileName As String) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

#End Region

End Class




