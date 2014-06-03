Imports System.Collections.Generic
Imports System.Net
Imports System.IO
Imports System.Text.RegularExpressions


Namespace Utilities.FTP



#Region "FTP Directory class"
    ''' <summary>
    ''' Stores a list of files and directories from an FTP result
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FTPdirectory
        Inherits List(Of FTPfileInfo)

        Sub New()
            'creates a blank directory listing
        End Sub

        ''' <summary>
        ''' Constructor: create list from a (detailed) directory string
        ''' </summary>
        ''' <param name="dir">directory listing string</param>
        ''' <param name="path"></param>
        ''' <remarks></remarks>
        Sub New(ByVal dir As String, ByVal path As String)
            For Each line As String In dir.Replace(vbLf, "").Split(CChar(vbCr))
                'parse
                If line <> "" Then Me.Add(New FTPfileInfo(line, path))
            Next
        End Sub

        ''' <summary>
        ''' Filter out only files from directory listing
        ''' </summary>
        ''' <param name="ext">optional file extension filter</param>
        ''' <returns>FTPdirectory listing</returns>
        Public Function GetFiles(Optional ByVal ext As String = "") As FTPdirectory
            Return Me.GetFileOrDir(FTPfileInfo.DirectoryEntryTypes.File, ext)
        End Function

        ''' <summary>
        ''' Returns a list of only subdirectories
        ''' </summary>
        ''' <returns>FTPDirectory list</returns>
        ''' <remarks></remarks>
        Public Function GetDirectories() As FTPdirectory
            Return Me.GetFileOrDir(FTPfileInfo.DirectoryEntryTypes.Directory)
        End Function

        'internal: share use function for GetDirectories/Files
        Private Function GetFileOrDir(ByVal type As FTPfileInfo.DirectoryEntryTypes, Optional ByVal ext As String = "") As FTPdirectory
            Dim result As New FTPdirectory()
            For Each fi As FTPfileInfo In Me
                If fi.FileType = type Then
                    If ext = "" Then
                        result.Add(fi)
                    ElseIf ext = fi.Extension Then
                        result.Add(fi)
                    End If
                End If
            Next
            Return result

        End Function

        Public Function FileExists(ByVal filename As String) As Boolean
            For Each ftpfile As FTPfileInfo In Me
                If ftpfile.Filename = filename Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Const slash As Char = "/"

        Public Shared Function GetParentDirectory(ByVal dir As String) As String
            Dim tmp As String = dir.TrimEnd(slash)
            Dim i As Integer = tmp.LastIndexOf(slash)
            If i > 0 Then
                Return tmp.Substring(0, i - 1)
            Else
                Throw New ApplicationException("No parent for root")
            End If
        End Function
    End Class
#End Region

End Namespace
