Public Class BackupFilesWait
    Public Event BackupRestoreCancelled()
    Public WaitMessage As String = "Ensuring backup is current..."
    Public FormTitle As String = "AutoVer Ensuring Backup"
    Public EngineIndex As Integer 'Current engine this is linked to

    Private Sub AppSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.Arrow
        lblWaitMessage.Text = WaitMessage
        Me.Text = FormTitle
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        RaiseEvent BackupRestoreCancelled()
        Me.Close()
    End Sub

End Class