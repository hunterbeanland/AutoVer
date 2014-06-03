Public Class RestoreAs
    Public BackupFile, RestoreAsFile As String
    Public ftp As Utilities.FTP.FTPclient

    Private Sub AppSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtFile.Text = FileUtils.GetShortPathIfNotLong(RestoreAsFile)
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        If IsNothing(ftp) Then
            Try
                Alphaleonis.Win32.Filesystem.File.Copy(BackupFile, txtFile.Text, True)
                Dim Log As New Logger
                Log.Info(BackupFile & " to " & txtFile.Text, "Restored")
                Me.Close()
            Catch ex As Exception
                MsgBox("Error restoring file: " & ex.Message, MsgBoxStyle.Exclamation)
            End Try
        Else
            Try
                If ftp.Download(BackupFile, txtFile.Text, True) Then
                    Dim Log As New Logger
                    Log.Info(BackupFile & " to " & txtFile.Text, "Restored")
                    Me.Close()
                Else
                    MsgBox("Error restoring file: " & ftp.ErrorText, MsgBoxStyle.Exclamation)
                End If
            Catch ex As Exception
                MsgBox("Error restoring file: " & ex.Message, MsgBoxStyle.Exclamation)
            End Try
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        'System.Diagnostics.Process.Start(Application.StartupPath & "\AutoVerHelp.htm", "#RestoreAs")
        Help.ShowHelp(Me, "AutoVer.chm", HelpNavigator.TopicId, "5")
    End Sub

End Class