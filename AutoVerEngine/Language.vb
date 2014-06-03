Imports System.IO
Imports System.Windows.Forms

Public Class Language
    Public _Language As New Generic.Dictionary(Of String, String)
    Private Log As Logger
    Private toolTipCtrl As ToolTip
    Const dotTip As String = ".tip"

    Public Sub New(ByRef commonLog As Logger)
        Log = commonLog
    End Sub

    Public Sub ReadLanguageFile(ByVal langPath As String, ByVal langId As String)
        If langId <> "en" Then ReadLanguageFile(langPath, "en")
        langPath = Path.Combine(langPath, "lang-" & langId & ".ini")
        If File.Exists(langPath) Then
            Try
                If FileUtils.ReadINI(langPath, _Language) Then
                    Log.Info("lang-" & langId & ".ini loaded", "Language")
                Else
                    Log.Warn(langId, "Language Not Found:")
                End If
            Catch ex As Exception
                Log.Warn(ex.Message, langPath)
            End Try
        Else
            Log.Warn(langId, "Language Not Found:")
        End If
    End Sub

    Private Sub WriteLanguageFile(ByVal langPath As String, ByVal langId As String, secName As String)
        langPath = Path.Combine(langPath, "lang-" & langId & ".ini")
        Try
            Using fs As New FileStream(langPath, FileMode.Create, FileAccess.ReadWrite)
                Using writer As New StreamWriter(fs, System.Text.Encoding.UTF8)
                    writer.WriteLine("[" & secName & "]")
                    For Each kvp As KeyValuePair(Of String, String) In _Language
                        If kvp.Key.StartsWith(secName) And kvp.Key.Length > secName.Length Then
                            writer.WriteLine(kvp.Key.Substring(secName.Length + 1) & "=" & kvp.Value)
                        Else
                            writer.WriteLine(kvp.Key & "=" & kvp.Value)
                        End If
                    Next
                    writer.Flush()
                End Using
            End Using
        Catch ex As Exception
            Log.Warn(ex.ToString, langPath)
        End Try
    End Sub

    Public Sub Apply(ByRef Container As Control, tooltipControl As ToolTip)
        toolTipCtrl = tooltipControl
        SetLabelsRecursive(Container, Container.Name)
        toolTipCtrl = Nothing
    End Sub

    Public Sub ExportControlsToFile(ByRef Container As Control, tooltipControl As ToolTip)
        _Language.Clear()
        toolTipCtrl = tooltipControl
        _Language(Container.Name) = Container.Text 'form title
        FindLabelsRecursive(Container, String.Empty)
        toolTipCtrl = Nothing
        WriteLanguageFile(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\AutoVer\", "en-" & Container.Name, Container.Name)
    End Sub

    Private Sub FindLabelsRecursive(ByRef Container As Control, parentId As String)
        Try
            If parentId <> String.Empty Then parentId &= "."
            If Not IsNothing(toolTipCtrl) Then
                Dim tt As String = toolTipCtrl.GetToolTip(Container)
                If Not String.IsNullOrEmpty(tt) Then _Language(parentId & Container.Name & dotTip) = tt
            End If

            If Container.GetType Is GetType(Label) OrElse Container.GetType Is GetType(RadioButton) Or Container.GetType Is GetType(CheckBox) Or Container.GetType Is GetType(Button) Or _
                 Container.GetType Is GetType(ToolStripMenuItem) Or Container.GetType Is GetType(GroupBox) Or Container.GetType Is GetType(TabPage) Then
                _Language(Container.Name) = Container.Text.Replace(vbCrLf, vbTab)
            ElseIf Container.GetType Is GetType(ToolStrip) Then
                For Each tsi As Object In DirectCast(Container, ToolStrip).Items
                    If tsi.GetType Is GetType(ToolStripButton) Then
                        Dim tsb As ToolStripButton = tsi
                        _Language(tsb.Name) = tsb.Text
                        _Language(tsb.Name & dotTip) = tsb.ToolTipText
                    ElseIf tsi.GetType Is GetType(ToolStripSplitButton) Then
                        Dim tsb As ToolStripSplitButton = tsi
                        _Language(tsb.Name) = tsb.Text
                        _Language(tsb.Name & dotTip) = tsb.ToolTipText
                        For Each tsidd As ToolStripMenuItem In tsb.DropDownItems
                            _Language(tsidd.Name) = tsidd.Text
                            _Language(tsidd.Name & dotTip) = tsidd.ToolTipText
                        Next
                    ElseIf tsi.GetType Is GetType(ToolStripDropDownButton) Then
                        Dim tsb As ToolStripDropDownButton = tsi
                        _Language(tsb.Name) = tsb.Text
                        _Language(tsb.Name & dotTip) = tsb.ToolTipText
                        For Each tsidd As ToolStripMenuItem In tsb.DropDownItems
                            _Language(tsidd.Name) = tsidd.Text
                            _Language(tsidd.Name & dotTip) = tsidd.ToolTipText
                        Next
                    End If
                Next
            ElseIf Container.GetType Is GetType(ContextMenuStrip) Then
                For Each tsi As Object In DirectCast(Container, ContextMenuStrip).Items
                    If tsi.GetType Is GetType(ToolStripItem) Then
                        Dim tsb As ToolStripItem = tsi
                        _Language(tsb.Name) = tsb.Text
                        _Language(tsb.Name & dotTip) = tsb.ToolTipText
                    ElseIf tsi.GetType Is GetType(ToolStripMenuItem) Then
                        Dim tsb As ToolStripMenuItem = tsi
                        _Language(tsb.Name) = tsb.Text
                        _Language(tsb.Name & dotTip) = tsb.ToolTipText
                    End If
                Next
            ElseIf Container.GetType Is GetType(TextBox) Then
                'do nothing
            Else
                Log.Debug(Container.Name, "FindLabelsRecursive UnknownType:" & Container.GetType.ToString)
            End If
        Catch ex As Exception
            Log.Error(ex.ToString, "FindLabelsRecursive")
        End Try

        For Each ChildControl As Control In Container.Controls
            FindLabelsRecursive(ChildControl, parentId & Container.Name)
        Next
    End Sub

    Private Sub FindItemsRecursive(ByRef Container As ToolStripMenuItem, parentId As String)
        Log.Debug(Container.Name, "FindItemsRecursive")
        Try
            _Language(parentId & Container.Name) = Container.Text
            For Each tsi As ToolStripMenuItem In Container.DropDownItems
                FindItemsRecursive(tsi, Container.Name)
            Next
        Catch ex As Exception
            Log.Error(ex.ToString, "FindItemsRecursive")
        End Try
    End Sub

    Private Sub SetLabelsRecursive(ByRef Container As Control, parentId As String)
        If Not IsNothing(toolTipCtrl) Then
            If _Language.ContainsKey(parentId & "." & Container.Name & dotTip) Then
                toolTipCtrl.SetToolTip(Container, _Language(parentId & "." & Container.Name & dotTip))
            End If
        End If

        If Container.GetType Is GetType(Label) OrElse Container.GetType Is GetType(RadioButton) Or Container.GetType Is GetType(CheckBox) Or Container.GetType Is GetType(Button) Or _
        Container.GetType Is GetType(ToolStripMenuItem) Or Container.GetType Is GetType(GroupBox) Or Container.GetType Is GetType(TabPage) Then
            If _Language.ContainsKey(parentId & "." & Container.Name) Then Container.Text = _Language(parentId & "." & Container.Name)
        ElseIf Container.GetType Is GetType(Form) Then
            If _Language.ContainsKey(Container.Name) Then Container.Text = _Language(Container.Name)
        ElseIf Container.GetType Is GetType(ToolStrip) Then
            For Each tsi As Object In DirectCast(Container, ToolStrip).Items
                If tsi.GetType Is GetType(ToolStripButton) Then
                    Dim tsb As ToolStripButton = tsi
                    If _Language.ContainsKey(parentId & "." & tsb.Name) Then tsb.Text = _Language(parentId & "." & tsb.Name)
                    If _Language.ContainsKey(parentId & "." & tsb.Name & dotTip) Then tsb.ToolTipText = _Language(parentId & "." & tsb.Name & dotTip)
                ElseIf tsi.GetType Is GetType(ToolStripSplitButton) Then
                    Dim tsb As ToolStripSplitButton = tsi
                    If _Language.ContainsKey(parentId & "." & tsb.Name) Then tsb.Text = _Language(parentId & "." & tsb.Name)
                    If _Language.ContainsKey(parentId & "." & tsb.Name & dotTip) Then tsb.ToolTipText = _Language(parentId & "." & tsb.Name & dotTip)
                    For Each tsidd As ToolStripMenuItem In tsb.DropDownItems
                        If _Language.ContainsKey(parentId & "." & tsidd.Name) Then tsidd.Text = _Language(parentId & "." & tsidd.Name)
                        If _Language.ContainsKey(parentId & "." & tsidd.Name & dotTip) Then tsidd.ToolTipText = _Language(parentId & "." & tsidd.Name & dotTip)
                    Next
                ElseIf tsi.GetType Is GetType(ToolStripDropDownButton) Then
                    Dim tsb As ToolStripDropDownButton = tsi
                    If _Language.ContainsKey(parentId & "." & tsb.Name) Then tsb.Text = _Language(parentId & "." & tsb.Name)
                    If _Language.ContainsKey(parentId & "." & tsb.Name & dotTip) Then tsb.ToolTipText = _Language(parentId & "." & tsb.Name & dotTip)
                    For Each tsidd As ToolStripMenuItem In tsb.DropDownItems
                        If _Language.ContainsKey(parentId & "." & tsidd.Name) Then tsidd.Text = _Language(parentId & "." & tsidd.Name)
                        If _Language.ContainsKey(parentId & "." & tsidd.Name & dotTip) Then tsidd.ToolTipText = _Language(parentId & "." & tsidd.Name & dotTip)
                    Next
                End If
            Next
        ElseIf Container.GetType Is GetType(ContextMenuStrip) Then
            For Each tsi As Object In DirectCast(Container, ContextMenuStrip).Items
                If tsi.GetType Is GetType(ToolStripItem) Then
                    Dim tsb As ToolStripItem = tsi
                    If _Language.ContainsKey(parentId & "." & tsb.Name) Then tsb.Text = _Language(parentId & "." & tsb.Name)
                    If _Language.ContainsKey(parentId & "." & tsb.Name & dotTip) Then tsb.ToolTipText = _Language(parentId & "." & tsb.Name & dotTip)
                ElseIf tsi.GetType Is GetType(ToolStripMenuItem) Then
                    Dim tsb As ToolStripMenuItem = tsi
                    If _Language.ContainsKey(parentId & "." & tsb.Name) Then tsb.Text = _Language(parentId & "." & tsb.Name)
                    If _Language.ContainsKey(parentId & "." & tsb.Name & dotTip) Then tsb.ToolTipText = _Language(parentId & "." & tsb.Name & dotTip)
                End If
            Next
        End If

        For Each ChildControl As Control In Container.Controls
            SetLabelsRecursive(ChildControl, parentId)
        Next
    End Sub
End Class
