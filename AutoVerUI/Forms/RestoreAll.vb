Public Class RestoreAll
    Public WatcherEngine As BackupEngine

    Private Sub RestoreAll_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtHour.Text = Now.Hour.ToString("00")
        txtMin.Text = Now.Minute.ToString("00")
        Me.Text &= ": " & WatcherEngine.Name
        lblMessage.Text = String.Empty
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.Cursor = Cursors.WaitCursor
        btnOK.Enabled = False
        lblMessage.Text = "Restoring..."
        If rbNow.Checked Then
            WatcherEngine.RestoreAll(Now.AddYears(10)) 'cover wrong clocks
        Else
            Dim intHour, intMin As Integer
            Integer.TryParse(txtHour.Text, intHour)
            Integer.TryParse(txtMin.Text, intMin)
            If intHour < 0 Or intHour > 23 Then intHour = 23
            If intMin < 0 Or intMin > 59 Then intHour = 59
            txtHour.Text = intHour.ToString("00")
            txtMin.Text = intMin.ToString("00")
            WatcherEngine.RestoreAll(New DateTime(dtpRestore.Value.Year, dtpRestore.Value.Month, dtpRestore.Value.Day, intHour, intMin, 59))
        End If
        System.Media.SystemSounds.Asterisk.Play()

        Me.Cursor = Cursors.Default
        lblMessage.Text = "Done"
        Button2.Text = "Close"
        'Threading.Thread.Sleep(4000)
        'Me.Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    'Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Help.ShowHelp(Me, "AutoVer.chm", HelpNavigator.TopicId, "5")
    'End Sub

    Private Sub dtpRestore_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpRestore.GotFocus
        rbDtp.Checked = True
    End Sub

End Class