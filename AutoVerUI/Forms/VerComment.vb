Public Class VerComment

    Private Sub VerComment_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Left = My.Computer.Screen.WorkingArea.Width - Me.Width
        Me.Top = My.Computer.Screen.WorkingArea.Height - Me.Height
        DataGridView1.EditMode = DataGridViewEditMode.EditOnEnter
        DataGridView1.Rows.Add(New String() {"abc", ""})
        DataGridView1.Rows.Add(New String() {"sdfggfhfghf.ghf", ""})
        DataGridView1.Rows(0).Cells(1).Selected = True
    End Sub

    Private Sub VerComment_Focus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.MouseCaptureChanged, Me.MouseHover, DataGridView1.MouseEnter, DataGridView1.GotFocus
        Me.Opacity = 1
    End Sub

    Private Sub VerComment_Blur(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DataGridView1.MouseLeave, DataGridView1.LostFocus
        Me.Opacity = 0.6
    End Sub
End Class