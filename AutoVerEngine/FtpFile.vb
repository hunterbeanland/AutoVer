Imports System.Collections.Generic
Imports System.Net
Imports System.IO
Imports System.Text.RegularExpressions


Namespace Utilities.FTP

#Region "FTP file info class"
    ''' <summary>
    ''' Represents a file or directory entry from an FTP listing
    ''' </summary>
    ''' <remarks>
    ''' This class is used to parse the results from a detailed
    ''' directory list from FTP. It supports most formats of
    ''' 
    ''' v1.1 fixed bug in Fullname/path
    ''' 
    ''' v1.2 fixed bug to handle blank size field
    ''' </remarks>
    Public Class FTPfileInfo
        'Stores extended info about FTP file

#Region "Properties"
        Public ReadOnly Property FullName() As String
            Get
                Return Path & Filename
            End Get
        End Property
        Public ReadOnly Property Filename() As String
            Get
                Return _filename
            End Get
        End Property
        ''' <summary>
        ''' Path of the file (always ends in /)
        ''' </summary>
        ''' <remarks>
        ''' 1.1: Modifed to ensure always ends in / - with thanks to jfransella for pointing this out
        ''' </remarks>
        Public ReadOnly Property Path() As String
            Get
                Return _path & IIf(_path.EndsWith("/"), "", "/")
            End Get
        End Property
        Public ReadOnly Property FileType() As DirectoryEntryTypes
            Get
                Return _fileType
            End Get
        End Property
        Public ReadOnly Property Size() As Long
            Get
                Return _size
            End Get
        End Property
        Public Property FileDateTime() As Date
            Get
                Return _fileDateTime
            End Get
            Friend Set(ByVal value As Date)
                _fileDateTime = value
            End Set
        End Property
        Public ReadOnly Property Permission() As String
            Get
                Return _permission
            End Get
        End Property
        Public ReadOnly Property Extension() As String
            Get
                Dim i As Integer = Me.Filename.LastIndexOf(".")
                If i >= 0 And i < (Me.Filename.Length - 1) Then
                    Return Me.Filename.Substring(i + 1)
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property NameOnly() As String
            Get
                Dim i As Integer = Me.Filename.LastIndexOf(".")
                If i > 0 Then
                    Return Me.Filename.Substring(0, i)
                Else
                    Return Me.Filename
                End If
            End Get
        End Property
        Private _filename As String
        Private _path As String
        Private _fileType As DirectoryEntryTypes
        Private _size As Long
        Private _fileDateTime As Date
        Private _permission As String

#End Region

        ''' <summary>
        ''' Identifies entry as either File or Directory
        ''' </summary>
        Public Enum DirectoryEntryTypes
            File
            Directory
        End Enum

        ''' <summary>
        ''' Constructor taking a directory listing line and path
        ''' </summary>
        ''' <param name="line">The line returned from the detailed directory list</param>
        ''' <param name="path">Path of the directory</param>
        ''' <remarks></remarks>
        Sub New(ByVal line As String, ByVal path As String)
            'parse line
            Dim m As Match = GetMatchingRegex(line)
            If m Is Nothing Then
                'failed
                Throw New ApplicationException("Unable to parse line: " & line)
            Else
                _filename = m.Groups("name").Value
                _path = path

                'v1.2 - fix to handle null size fields (copied from C# version)
                Int64.TryParse(m.Groups("size").Value, _size)

                _permission = m.Groups("permission").Value
                Dim _dir As String = m.Groups("dir").Value
                If (_dir <> "" And _dir <> "-") Then
                    _fileType = DirectoryEntryTypes.Directory
                Else
                    _fileType = DirectoryEntryTypes.File
                End If

                'Hunter> change culture
                'Try
                '    _fileDateTime = Date.Parse(m.Groups("timestamp").Value)
                'Catch ex As Exception
                '    _fileDateTime = Nothing
                'End Try
                Dim cult As New Globalization.CultureInfo("en-US", False)
                _fileDateTime = Nothing
                Dim strDateTime As String = m.Groups("timestamp").Value
                If Not DateTime.TryParse(strDateTime, cult.DateTimeFormat, Globalization.DateTimeStyles.None, _fileDateTime) Then
                    If Not Date.TryParse(strDateTime.Substring(3, 2) & "-" & strDateTime.Substring(0, 2) & "-" & strDateTime.Substring(6, 2) & strDateTime.Substring(9), _fileDateTime) Then

                    End If
                End If
            End If
        End Sub

        Private Function GetMatchingRegex(ByVal line As String) As Match
            Dim rx As Regex, m As Match
            For i As Integer = 0 To _ParseFormats.Length - 1
                rx = New Regex(_ParseFormats(i))
                m = rx.Match(line)
                If m.Success Then Return m
            Next
            Return Nothing
        End Function

#Region "Regular expressions for parsing LIST results"
        ''' <summary>
        ''' List of REGEX formats for different FTP server listing formats
        ''' </summary>
        ''' <remarks>
        ''' The first three are various UNIX/LINUX formats, fourth is for MS FTP
        ''' in detailed mode and the last for MS FTP in 'DOS' mode.
        ''' I wish VB.NET had support for Const arrays like C# but there you go
        ''' </remarks>
        Private Shared _ParseFormats As String() = { _
            "(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)", _
            "(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)", _
            "(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)", _
            "(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)", _
            "(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})(\s+)(?<size>(\d+))(\s+)(?<ctbit>(\w+\s\w+))(\s+)(?<size2>(\d+))\s+(?<timestamp>\w+\s+\d+\s+\d{2}:\d{2})\s+(?<name>.+)", _
            "(?<timestamp>\d{2}\-\d{2}\-\d{2}\s+\d{2}:\d{2}[Aa|Pp][mM])\s+(?<dir>\<\w+\>){0,1}(?<size>\d+){0,1}\s+(?<name>.+)"}
#End Region
    End Class
#End Region

End Namespace
