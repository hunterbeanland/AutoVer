Imports AutoVer.Utilities.FTP
Imports System.Runtime.InteropServices
Imports System.Configuration
Imports System.IO
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports Microsoft.Win32
Imports Alphaleonis.Win32

Public Class VerExplorer
    Inherits Form
    Public WatchFolder As String = String.Empty
    Public BackupFolder As String = String.Empty
    Public ViewingWatch As Boolean = True
    Public WatchEngine As BackupEngine
    Public Config As ConfigEngine
    Private ftp As FTPclient
    Private LastSelectedFolder As String
    Friend WithEvents ToolStrip2 As ToolStrip
    Friend WithEvents tspWatch As ToolStripButton
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents lvwVersions As ListView
    Friend WithEvents ColumnDate As ColumnHeader
    Friend WithEvents ColumnSize As ColumnHeader
    Friend WithEvents ColumnName As ColumnHeader
    Friend WithEvents tspBackup As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents tspView As ToolStripButton
    Friend WithEvents tspRefresh As ToolStripButton
    Friend WithEvents tspCompare As ToolStripButton
    Friend WithEvents tspRestore As ToolStripButton
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents OpenViewFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CompareFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RestoreAsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents DeleteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents tsbDelete As ToolStripButton
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    'Private Shared Event1 As New ManualResetEvent(True)
    Friend lvwVersionsSorter As New ListViewColumnSorter
    Friend lvw1Sorter As New ListViewColumnSorter
    Friend WithEvents HelpProvider1 As HelpProvider
    Friend WithEvents trv As TreeView
    Friend WithEvents imlTreeView As ImageList
    Friend WithEvents imlSmallListView As ImageList
    Friend WithEvents lsv As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Private IH As New IconHandler
    Private Log As New Logger
    Private UseRecycleBin As Boolean

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        Application.EnableVisualStyles()
        'Application.DoEvents()
        'Application.Run(New Form1)
        'This call is required by the Windows Form Designer.
        'If IsNothing(SpecificStartFolder) Then
        'Else
        'WatchFolder = SpecificStartFolder
        'End If
        'WatchFolder = "C:\"
        InitializeComponent()
        'Add any initialization after the InitializeComponent() call
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents sbr1 As System.Windows.Forms.StatusBar

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = _
                New System.ComponentModel.ComponentResourceManager(GetType(VerExplorer))
        Me.MainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
        Me.sbr1 = New System.Windows.Forms.StatusBar
        Me.ToolStrip2 = New System.Windows.Forms.ToolStrip
        Me.tspWatch = New System.Windows.Forms.ToolStripButton
        Me.tspBackup = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.tspView = New System.Windows.Forms.ToolStripButton
        Me.tspRefresh = New System.Windows.Forms.ToolStripButton
        Me.tspCompare = New System.Windows.Forms.ToolStripButton
        Me.tspRestore = New System.Windows.Forms.ToolStripButton
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.tsbDelete = New System.Windows.Forms.ToolStripButton
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.trv = New System.Windows.Forms.TreeView
        Me.imlTreeView = New System.Windows.Forms.ImageList(Me.components)
        Me.lsv = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader3 = New System.Windows.Forms.ColumnHeader
        Me.imlSmallListView = New System.Windows.Forms.ImageList(Me.components)
        Me.lvwVersions = New System.Windows.Forms.ListView
        Me.ColumnDate = New System.Windows.Forms.ColumnHeader
        Me.ColumnSize = New System.Windows.Forms.ColumnHeader
        Me.ColumnName = New System.Windows.Forms.ColumnHeader
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenViewFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CompareFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RestoreAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HelpProvider1 = New System.Windows.Forms.HelpProvider
        Me.ToolStrip2.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'sbr1
        '
        Me.sbr1.Location = New System.Drawing.Point(0, 354)
        Me.sbr1.Name = "sbr1"
        Me.sbr1.Size = New System.Drawing.Size(753, 17)
        Me.sbr1.TabIndex = 2
        Me.sbr1.Text = "Ready"
        '
        'ToolStrip2
        '
        Me.ToolStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tspWatch, Me.tspBackup, Me.ToolStripSeparator1, Me.tspView, Me.tspRefresh, Me.tspCompare, Me.tspRestore, Me.ToolStripSeparator3, Me.tsbDelete})
        Me.ToolStrip2.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip2.Name = "ToolStrip2"
        Me.ToolStrip2.Size = New System.Drawing.Size(753, 25)
        Me.ToolStrip2.TabIndex = 4
        Me.ToolStrip2.Text = "ToolStrip2"
        '
        'tspWatch
        '
        Me.tspWatch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tspWatch.Image = CType(resources.GetObject("tspWatch.Image"), System.Drawing.Image)
        Me.tspWatch.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tspWatch.Name = "tspWatch"
        Me.tspWatch.Size = New System.Drawing.Size(23, 22)
        Me.tspWatch.Text = "View Watch Folder"
        '
        'tspBackup
        '
        Me.tspBackup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tspBackup.Image = CType(resources.GetObject("tspBackup.Image"), System.Drawing.Image)
        Me.tspBackup.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tspBackup.Name = "tspBackup"
        Me.tspBackup.Size = New System.Drawing.Size(23, 22)
        Me.tspBackup.Text = "View Backup Folder"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'tspView
        '
        Me.tspView.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tspView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tspView.Image = CType(resources.GetObject("tspView.Image"), System.Drawing.Image)
        Me.tspView.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tspView.Name = "tspView"
        Me.tspView.Size = New System.Drawing.Size(23, 22)
        Me.tspView.Text = "Open/View Files"
        '
        'tspRefresh
        '
        Me.tspRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tspRefresh.Image = CType(resources.GetObject("tspRefresh.Image"), System.Drawing.Image)
        Me.tspRefresh.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tspRefresh.Name = "tspRefresh"
        Me.tspRefresh.Size = New System.Drawing.Size(23, 22)
        Me.tspRefresh.Text = "Refresh Folders"
        '
        'tspCompare
        '
        Me.tspCompare.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tspCompare.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tspCompare.Image = CType(resources.GetObject("tspCompare.Image"), System.Drawing.Image)
        Me.tspCompare.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tspCompare.Name = "tspCompare"
        Me.tspCompare.Size = New System.Drawing.Size(23, 22)
        Me.tspCompare.Text = "Compare a file with current / Compare 2 Files"
        '
        'tspRestore
        '
        Me.tspRestore.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tspRestore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tspRestore.Image = CType(resources.GetObject("tspRestore.Image"), System.Drawing.Image)
        Me.tspRestore.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tspRestore.Name = "tspRestore"
        Me.tspRestore.Size = New System.Drawing.Size(23, 22)
        Me.tspRestore.Text = "Restore File"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'tsbDelete
        '
        Me.tsbDelete.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbDelete.Image = CType(resources.GetObject("tsbDelete.Image"), System.Drawing.Image)
        Me.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbDelete.Name = "tsbDelete"
        Me.tsbDelete.Size = New System.Drawing.Size(23, 22)
        Me.tsbDelete.Text = "Delete Files"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.trv)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lsv)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lvwVersions)
        Me.SplitContainer1.Size = New System.Drawing.Size(753, 329)
        Me.SplitContainer1.SplitterDistance = 178
        Me.SplitContainer1.TabIndex = 90
        '
        'trv
        '
        Me.trv.Dock = System.Windows.Forms.DockStyle.Fill
        Me.trv.HideSelection = False
        Me.trv.ImageIndex = 0
        Me.trv.ImageList = Me.imlTreeView
        Me.trv.Location = New System.Drawing.Point(0, 0)
        Me.trv.Name = "trv"
        Me.trv.SelectedImageIndex = 0
        Me.trv.Size = New System.Drawing.Size(178, 329)
        Me.trv.TabIndex = 1
        '
        'imlTreeView
        '
        Me.imlTreeView.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        Me.imlTreeView.ImageSize = New System.Drawing.Size(16, 16)
        Me.imlTreeView.TransparentColor = System.Drawing.Color.Transparent
        '
        'lsv
        '
        Me.lsv.AllowColumnReorder = True
        Me.lsv.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lsv.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3})
        Me.lsv.HideSelection = False
        Me.lsv.Location = New System.Drawing.Point(0, 0)
        Me.lsv.Name = "lsv"
        Me.lsv.Size = New System.Drawing.Size(312, 329)
        Me.lsv.SmallImageList = Me.imlSmallListView
        Me.lsv.TabIndex = 2
        Me.lsv.UseCompatibleStateImageBehavior = False
        Me.lsv.ListViewItemSorter = lvw1Sorter
        'Me.lsv.ListViewItemSorter = lvw1Sorter

        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Name"
        Me.ColumnHeader1.Width = 180
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Size"
        Me.ColumnHeader2.Width = 0
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Date Changed"
        Me.ColumnHeader3.Width = 120
        '
        'imlSmallListView
        '
        Me.imlSmallListView.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        Me.imlSmallListView.ImageSize = New System.Drawing.Size(16, 16)
        Me.imlSmallListView.TransparentColor = System.Drawing.Color.Transparent
        '
        'lvwVersions
        '
        Me.lvwVersions.AllowColumnReorder = True
        Me.lvwVersions.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnDate, Me.ColumnSize, Me.ColumnName})
        Me.lvwVersions.Dock = System.Windows.Forms.DockStyle.Right
        Me.lvwVersions.Location = New System.Drawing.Point(318, 0)
        Me.lvwVersions.Name = "lvwVersions"
        Me.lvwVersions.Size = New System.Drawing.Size(253, 329)
        Me.lvwVersions.TabIndex = 3
        Me.lvwVersions.UseCompatibleStateImageBehavior = False
        Me.lvwVersions.View = System.Windows.Forms.View.Details
        Me.lvwVersions.ListViewItemSorter = lvwVersionsSorter
        'Me.lvwVersions.ListViewItemSorter = lvwVersionsSorter

        '
        'ColumnDate
        '
        Me.ColumnDate.Text = "Version Date"
        Me.ColumnDate.Width = 140
        '
        'ColumnSize
        '
        Me.ColumnSize.Text = "Size"
        Me.ColumnSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ColumnSize.Width = 90
        '
        'ColumnName
        '
        Me.ColumnName.Text = "Name"
        Me.ColumnName.Width = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenViewFileToolStripMenuItem, Me.CompareFilesToolStripMenuItem, Me.RestoreAsToolStripMenuItem, Me.ToolStripSeparator2, Me.DeleteToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(150, 98)
        '
        'OpenViewFileToolStripMenuItem
        '
        Me.OpenViewFileToolStripMenuItem.Image = CType(resources.GetObject("OpenViewFileToolStripMenuItem.Image"), System.Drawing.Image)
        Me.OpenViewFileToolStripMenuItem.Name = "OpenViewFileToolStripMenuItem"
        Me.OpenViewFileToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.OpenViewFileToolStripMenuItem.Text = "Open/View"
        '
        'CompareFilesToolStripMenuItem
        '
        Me.CompareFilesToolStripMenuItem.Image = CType(resources.GetObject("CompareFilesToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CompareFilesToolStripMenuItem.Name = "CompareFilesToolStripMenuItem"
        Me.CompareFilesToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.CompareFilesToolStripMenuItem.Text = "Compare a file with current / Compare 2 Files"
        '
        'RestoreAsToolStripMenuItem
        '
        Me.RestoreAsToolStripMenuItem.Image = CType(resources.GetObject("RestoreAsToolStripMenuItem.Image"), System.Drawing.Image)
        Me.RestoreAsToolStripMenuItem.Name = "RestoreAsToolStripMenuItem"
        Me.RestoreAsToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.RestoreAsToolStripMenuItem.Text = "Restore As"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(146, 6)
        '
        'DeleteToolStripMenuItem
        '
        Me.DeleteToolStripMenuItem.Image = CType(resources.GetObject("DeleteToolStripMenuItem.Image"), System.Drawing.Image)
        Me.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
        Me.DeleteToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.DeleteToolStripMenuItem.Text = "Delete"
        '
        'HelpProvider1
        '
        Me.HelpProvider1.HelpNamespace = "AutoVer.chm"
        '
        'VerExplorer
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(753, 371)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.ToolStrip2)
        Me.Controls.Add(Me.sbr1)
        Me.HelpProvider1.SetHelpKeyword(Me, "5")
        Me.HelpProvider1.SetHelpNavigator(Me, System.Windows.Forms.HelpNavigator.TopicId)
        Me.HelpProvider1.SetHelpString(Me, "5")
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Menu = Me.MainMenu1
        Me.Name = "VerExplorer"
        Me.HelpProvider1.SetShowHelp(Me, True)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AutoVer Explorer"
        Me.ToolStrip2.ResumeLayout(False)
        Me.ToolStrip2.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

#Region "Form Load & Menus "

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If WatchEngine.FTPEnable Then
            ftp = New FTPclient(WatchEngine.FTPHost, WatchEngine.FTPUser, WatchEngine.FTPPass)
            ftp.UsePassive = WatchEngine.FTPPassive
        End If
        AddGenericIcons()
        'Then add the folder icon to the imagelists of the listview
        IH.AddFolder(imlSmallListView)
        'Then add the drives to the treeview
        'AddDrives()
        AddRootFolder(WatchFolder)
        'Some items in the contextmenu
        lsv.View = View.Details
        Try
            LastSelectedFolder = WatchFolder
        Catch ex As Exception
            MsgBox("Drive not available.", MsgBoxStyle.Critical)
            Log.Warn("Drive not available: " & ex.Message, "VerExplorer")
            Me.Close()
        End Try
        Try
            If Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "RecycleBin", 0) = 1 Then UseRecycleBin = True
        Catch
        End Try
        Try
            If Not IsNothing(ConfigurationManager.AppSettings("RecycleBin")) Then _
                UseRecycleBin = ConfigurationManager.AppSettings("RecycleBin")
        Catch
        End Try
        'Config.Lang.ExportControlsToFile(Me, Nothing)
    End Sub

    'Private Sub cmdExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    mnuExit_Click(sender, e)
    'End Sub

    'Private Sub mnuExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Me.Close()
    'End Sub

    Private Sub tspWatch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tspWatch.Click
        ViewingWatch = True
        Try
            trv.BeginUpdate()
            trv.Nodes.Clear()
            trv.EndUpdate()
            AddRootFolder(WatchFolder)
            'tspRestore.Enabled = True
            tspCompare.Enabled = True
            'tspView.Enabled = True
            ContextMenuStrip1.Items(1).Enabled = True
        Catch ex As Exception
            MsgBox("Drive not available.", MsgBoxStyle.Critical)
            Log.Warn("Drive not available: " & ex.Message, "VerExplorer")
        End Try
    End Sub

    Private Sub tspBackup_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tspBackup.Click
        ViewingWatch = False
        Try
            trv.BeginUpdate()
            trv.Nodes.Clear()
            trv.EndUpdate()
            AddRootFolder(BackupFolder)
            'tspRestore.Enabled = False
            tspCompare.Enabled = False
            'tspView.Enabled = False
            ContextMenuStrip1.Items(1).Enabled = False
        Catch ex As Exception
            MsgBox("Drive not available.", MsgBoxStyle.Critical)
            Log.Warn("Drive not available: " & ex.Message, "VerExplorer")
        End Try
    End Sub

    Private Sub tspRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tspRefresh.Click
        'trv_SelectFolder(LastSelectedFolder)
        trv.BeginUpdate()
        trv.Nodes.Clear()
        trv.EndUpdate()
        If ViewingWatch Then
            AddRootFolder(WatchFolder)
        Else
            AddRootFolder(BackupFolder)
        End If
        lvwVersions.Items.Clear()
    End Sub

#End Region

#Region " Treeview and main file list views "

    Private Sub AddGenericIcons()
        'We make a new IconHandler, so the extension list wont get messed up by adding icons
        'to the treeview's imagelist
        Dim tIH As New IconHandler
        tIH.AddIconByFile("Shell32.dll", 6, imlTreeView, , True) 'The removable drive (Index 0)
        tIH.AddIconByFile("Shell32.dll", 8, imlTreeView, , True) 'The fixed drive (Index 1)
        tIH.AddIconByFile("Shell32.dll", 11, imlTreeView, , True) 'And the CD-Rom (Index 2)
        tIH.AddIconByFile("Shell32.dll", 3, imlTreeView, , True) 'The closed folder (Index 3)
        tIH.AddIconByFile("Shell32.dll", 4, imlTreeView, , True) 'The opened folder (Index 4)
    End Sub

    Private Sub AddRootFolder(ByVal VirtualRoot As String)
        'Add a virtual root folder rather than all the drives (via AddDrives)
        Dim tNode As TreeNode
        trv.BeginUpdate()
        trv.SelectedNode = Nothing
        If Not ViewingWatch And WatchEngine.FTPEnable Then
            tNode = trv.Nodes.Add(VirtualRoot)
        Else
            tNode = trv.Nodes.Add(New Filesystem.DirectoryInfo(VirtualRoot).Name)
        End If
        tNode.Tag = VirtualRoot
        tNode.ImageIndex = 1
        tNode.SelectedImageIndex = 1
        tNode.Expand()
        AddSubDirs(tNode)
        trv.SelectedNode = trv.Nodes(0)
        trv.EndUpdate()
        trv.Nodes(0).Expand()
    End Sub

    'Private Sub AddDrives()
    '    Dim fsoDrive As DriveInfo
    '    Dim tNode, CNode As TreeNode
    '    CNode = New TreeNode()
    '    'Prepare the treeview for an update
    '    trv.BeginUpdate()
    '    For Each fsoDrive In DriveInfo.GetDrives

    '        'Add the drives name to the treeview
    '        trv.SelectedNode = Nothing
    '        tNode = trv.Nodes.Add(fsoDrive.Name)
    '        If fsoDrive.Name = "C:\" Then CNode = tNode
    '        tNode.Tag = fsoDrive.Name
    '        'We use the tags for the full path, checked when adding subdirs

    '        'Then add apropriate icon
    '        Select Case fsoDrive.DriveType
    '            Case DriveType.Removable
    '                tNode.ImageIndex = 0
    '                tNode.SelectedImageIndex = 0
    '            Case DriveType.Fixed
    '                tNode.ImageIndex = 1
    '                tNode.SelectedImageIndex = 1
    '            Case DriveType.CDRom
    '                tNode.ImageIndex = 2
    '                tNode.SelectedImageIndex = 2
    '        End Select

    '        'If the drive is ready, add the subdirectories as well to the node
    '        If fsoDrive.IsReady Then AddSubDirs(tNode)
    '    Next

    '    'Lest make the C:\ selected, then the update is completed :)
    '    trv.SelectedNode = CNode
    '    'trv.Nodes(0)
    '    trv.EndUpdate()
    'End Sub

    Private Sub AddSubDirs(ByRef Node As TreeNode)
        Dim nNode As TreeNode
        Node.Nodes.Clear() 'Should the node contain any nodes already, then we remove them

        If Not ViewingWatch And WatchEngine.FTPEnable Then
            Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(Node.Tag, False)
            For Each ftpFile As FTPfileInfo In ftpList
                If ftpFile.FileType = FTPfileInfo.DirectoryEntryTypes.Directory Then
                    nNode = Node.Nodes.Add(ftpFile.Filename)
                    nNode.Tag = ftpFile.FullName
                    nNode.ImageIndex = 3
                    nNode.SelectedImageIndex = 4
                End If
            Next
        Else
            Dim dInfo As New Filesystem.DirectoryInfo(Node.Tag) 'The tags always contains the full path
            Dim dir As Filesystem.DirectoryInfo
            Try
                For Each dir In dInfo.GetDirectories
                    nNode = Node.Nodes.Add(dir.Name)
                    nNode.Tag = dir.FullName

                    'Set proper icons for this node (closed and opened folders)
                    nNode.ImageIndex = 3
                    nNode.SelectedImageIndex = 4

                    'If you want to make this subroutine recursive, and add every folder
                    'at the same time (which may take some time) uncomment the following line
                    'AddSubDirs(nNode)
                Next
            Catch ex As Exception
                Log.Error(ex.Message, "VerExplorer.AddSubDirs")
            End Try
        End If
    End Sub

    Private Sub trv_BeforeExpand(ByVal sender As Object, ByVal e As TreeViewCancelEventArgs) Handles trv.BeforeExpand
        'Since I do not add all folders, I need to add them as the nodes gets expanded
        Dim nNode As TreeNode

        'Prepare the treeview for an update
        trv.BeginUpdate()
        'Then go through all the nodes of the node being expanded and add the subdirs to each of them
        For Each nNode In e.Node.Nodes
            AddSubDirs(nNode)
        Next
        'And we are done
        trv.EndUpdate()
    End Sub

    Private Sub trv_AfterSelect(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles trv.AfterSelect
        'When a node has been selected, we need to add update the listview with the files
        trv_SelectFolder(e.Node.Tag)
    End Sub

    Private Sub trv_SelectFolder(ByVal strFolder As String)
        'When a node has been selected, we need to add update the listview with the files
        Dim lItem As ListViewItem
        Dim intDirCount, intFileCount As Integer
        lsv.BeginUpdate()
        lsv.Items.Clear()
        lvwVersions.Items.Clear()

        Try 'If the user has selected a drive that is not ready or too long of a file path
            If Not ViewingWatch And WatchEngine.FTPEnable Then
                'FTP lookup
                LastSelectedFolder = strFolder '& "/"
                Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(strFolder, False)
                For Each ftpFile As FTPfileInfo In ftpList
                    If ftpFile.FileType = FTPfileInfo.DirectoryEntryTypes.File Then
                        lItem = lsv.Items.Add(ftpFile.Filename)
                        lItem.Tag = ftpFile.FullName 'Having the full path to the file in the tag might be useful
                        'or else, we add (or get an index) for the icon using its extension
                        lItem.ImageIndex = IH.AddIconByExt(ftpFile.Extension, imlSmallListView)
                        lItem.SubItems.Add(ftpFile.Size)
                        lItem.SubItems.Add(ftpFile.FileDateTime)
                        intFileCount += 1
                    Else
                        intDirCount += 1
                    End If
                Next

                Me.Text = "AutoVer Backup Explorer: " & strFolder
                If intDirCount + intFileCount > 0 Then
                    sbr1.Text = strFolder & "                 " & intDirCount & " Folders, " & intFileCount & " Files"
                Else
                    sbr1.Text = strFolder & " Has No Items"
                End If
            Else
                'Normal drive lookup
                Dim dInfo As New Filesystem.DirectoryInfo(strFolder)
                Dim dir As Filesystem.DirectoryInfo
                Dim file As Filesystem.FileInfo
                LastSelectedFolder = dInfo.FullName & "\"

                For Each dir In dInfo.GetDirectories
                    '    lItem = lsv.Items.Add(dir.Name)
                    '    lItem.Tag = dir.FullName
                    '    lItem.ImageIndex = 0 'Since we added the folder to the image list first
                    intDirCount += 1
                Next

                'And then the files
                For Each file In dInfo.GetFiles
                    lItem = lsv.Items.Add(file.Name)
                    lItem.Tag = file.FullName 'Having the full path to the file in the tag might be useful
                    If file.Extension.ToLower = ".exe" Then
                        'If the file is an .exe, we add (or get the correct index) the icon from the file itself, with the icon index 0
                        lItem.ImageIndex = IH.AddIconByFile(file.FullName, 0, imlSmallListView)
                    Else
                        'or else, we add (or get an index) for the icon using its extension
                        lItem.ImageIndex = IH.AddIconByExt(file.Extension, imlSmallListView)
                    End If
                    'Lets add some sub items for Detailed view
                    lItem.SubItems.Add(file.Length)
                    lItem.SubItems.Add(file.LastWriteTime)
                    intFileCount += 1
                Next

                Me.Text = "AutoVer Backup Explorer: " & dInfo.Name
                If intDirCount + intFileCount > 0 Then
                    sbr1.Text = dInfo.Name & "                 " & intDirCount & " Folders, " & intFileCount & " Files"
                Else
                    sbr1.Text = dInfo.Name & " Has No Items"
                End If
            End If
        Catch ex As Exception
            Log.Error(ex.Message, "VerExplorer.trv_SelectFolder")
            MsgBox(ex.Message)
        End Try

        lsv.EndUpdate()
    End Sub

#End Region

    'Private Sub lsv_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lsv.DoubleClick
    '    If IsNothing(lsv.SelectedItems) OrElse lsv.SelectedItems.Count < 1 Then Exit Sub
    '    If IO.Directory.Exists(lsv.SelectedItems(0).Tag) Then
    '        trv_SelectFolder(lsv.SelectedItems(0).Tag)
    '    End If
    'End Sub

    'Private Sub lsv_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lsv.MouseUp
    '    Dim lvi As ListViewItem = lsv.GetItemAt(e.X, e.Y)
    '    If IsNothing(lsv) Then Exit Sub
    '    If IsNothing(lsv.SelectedItems) OrElse lsv.SelectedItems.Count < 1 Then Exit Sub
    '    If IO.File.Exists(lsv.SelectedItems(0).Tag) Then
    '        GetVersions(lsv.SelectedItems(0).Tag)
    '    End If
    'End Sub

    Private Sub lsv_MouseUp(ByVal sender As Object, ByVal e As ListViewItemSelectionChangedEventArgs) _
        Handles lsv.ItemSelectionChanged
        'Dim lvi As ListViewItem = e.Item
        If IsNothing(lsv) Then Exit Sub
        If IsNothing(lsv.SelectedItems) OrElse lsv.SelectedItems.Count < 1 Then Exit Sub
        If Filesystem.File.Exists(lsv.SelectedItems(0).Tag) Then
            GetVersions(lsv.SelectedItems(0).Tag)
        End If
    End Sub

    Private Sub GetVersions(ByVal FileName As String)
        'Get a list of versions for the selected original file
        lvwVersions.Items.Clear()
        If Not LastSelectedFolder.ToLower.StartsWith(WatchFolder.ToLower) Then Exit Sub
        Dim RelativeFolder, FileExtension As String
        RelativeFolder = LastSelectedFolder.Substring(WatchFolder.Length)
        If FileName.Contains(".") Then
            FileExtension = FileName.Substring(FileName.LastIndexOf(".", StringComparison.Ordinal))
        Else
            FileExtension = String.Empty
        End If

        If WatchEngine.FTPEnable Then
            Dim FileNameOnly As String = New Filesystem.FileInfo(FileName).Name & "."
            Try
                If Not BackupFolder.EndsWith("/") And Not RelativeFolder.StartsWith("/") Then RelativeFolder = "/" & RelativeFolder
                Dim ftpList As FTPdirectory = ftp.ListDirectoryDetail(BackupFolder & RelativeFolder, False)
                For Each ftpFile As FTPfileInfo In ftpList
                    If ftpFile.FileType = FTPfileInfo.DirectoryEntryTypes.File Then
                        If ftpFile.Filename.StartsWith(FileNameOnly) Then
                            'Dim strSize As String
                            'If ftpFile.Size > 102400 Then
                            '    strSize = Format(ftpFile.Size/1024, "#,### KB")
                            'ElseIf ftpFile.Size > 1024 Then
                            '    strSize = Format(ftpFile.Size/1024, "##.0 KB")
                            'Else
                            '    strSize = Format(ftpFile.Size, "##0 Bytes")
                            'End If
                            lvwVersions.Items.Add(New ListViewItem(New String() {ftpFile.FileDateTime, ftpFile.Size, ftpFile.FullName}))
                        End If
                    End If
                Next
            Catch ex As Exception
                Log.Debug(ex.Message, "VerExplorer.GetVersions")
            End Try
        Else
            If Not BackupFolder.EndsWith("\") And Not RelativeFolder.StartsWith("\") Then RelativeFolder = "\" & RelativeFolder
            Dim diBackup As New Filesystem.DirectoryInfo(BackupFolder & RelativeFolder)
            Try
                Dim fiFiles() As Filesystem.FileInfo = diBackup.GetFiles(New Filesystem.FileInfo(FileName).Name & ".*" & FileExtension, SearchOption.TopDirectoryOnly)
                If fiFiles.Length = 0 Then fiFiles = diBackup.GetFiles(New Filesystem.FileInfo(FileName).Name) 'maybe versioning is turned off
                For Each fiBackup As Filesystem.FileInfo In fiFiles
                    Log.Debug(fiBackup.FullName, "fiBackup.FullName")
                    Dim strSize As String
                    If fiBackup.Length > 102400 Then
                        strSize = Format(fiBackup.Length / 1024, "#,### KB")
                    ElseIf fiBackup.Length > 1024 Then
                        strSize = Format(fiBackup.Length / 1024, "##.0 KB")
                    Else
                        strSize = Format(fiBackup.Length, "##0 Bytes")
                    End If
                    lvwVersions.Items.Add(New ListViewItem(New String() {fiBackup.LastWriteTime, strSize, fiBackup.FullName}))
                Next
            Catch ex As Exception
                Log.Debug(ex.Message & " # " & BackupFolder & RelativeFolder & " # " & New Filesystem.FileInfo(FileName).Name & ".*" & FileExtension, "VerExplorer.GetVersions")
            End Try
        End If
        If sbr1.Text.Contains("Back") Then
            sbr1.Text = sbr1.Text.Substring(0, sbr1.Text.LastIndexOf(", ", System.StringComparison.Ordinal))
        End If
        sbr1.Text &= ", " & lvwVersions.Items.Count.ToString & " Backup versions"
    End Sub

    Private Sub tspView_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tspView.Click
        'Open files
        If ViewingWatch Then
            'Viewing Watch & backup versions
            If lvwVersions.SelectedItems.Count = 0 Then
                MsgBox("You must select a file date to open", MsgBoxStyle.Exclamation)
            Else
                Dim aryFiles As New ArrayList
                For Each lvi As ListViewItem In lvwVersions.SelectedItems
                    aryFiles.Add(lvi.SubItems(2).Text)
                Next
                Dim frmFileOpen As New ViewFile(Config)
                frmFileOpen.FilesToView = aryFiles
                frmFileOpen.ftp = ftp
                frmFileOpen.Config = Config
                frmFileOpen.Show()
            End If
        Else
            'Viewing backup folder or the lsv of watch folder (from context menu)
            If lsv.SelectedItems.Count = 0 Then
                MsgBox("You must select a file to open", MsgBoxStyle.Exclamation)
            Else
                Dim aryFiles As New ArrayList
                For Each lvi As ListViewItem In lsv.SelectedItems
                    'If IO.File.Exists(lvi.Tag) Then
                    aryFiles.Add(lvi.Tag)
                Next
                Dim frmFileOpen As New ViewFile(Config)
                frmFileOpen.FilesToView = aryFiles
                frmFileOpen.ftp = ftp
                frmFileOpen.Config = Config
                frmFileOpen.Show()
            End If
        End If
    End Sub

    Private Sub tspCompare_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tspCompare.Click
        'Compare files
        If lvwVersions.SelectedItems.Count <> 1 And lvwVersions.SelectedItems.Count <> 2 Then
            MsgBox("You must select 1 to 2 files dates to compare", MsgBoxStyle.Exclamation)
        Else
            'Dim RelativeFolder As String = LastSelectedFolder.Substring(WatchFolder.Length)
            'Logger.Debug(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\WinMerge\WinMerge.exe " & lvwVersions.SelectedItems(0).SubItems(2).Text & " " & lvwVersions.SelectedItems(1).SubItems(2).Text, "Compare")
            Dim strApp As String = Config.AppConfigDefault("CompareApp", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) & "\WinMerge\WinMergeU.exe ""{0}"" ""{1}""")
            'Try
            '    If Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "CompareApp", String.Empty) <> String.Empty Then
            '        strApp = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\Software\AutoVer", "CompareApp", String.Empty)
            '    Else
            '        strApp = System.Configuration.ConfigurationManager.AppSettings("CompareApp")
            '    End If
            'Catch ex As Exception
            'End Try
            Dim strParams As String = """{0}"" ""{1}"""
            If strApp.Contains(".exe") Then
                strParams = strApp.Substring(strApp.LastIndexOf(".exe", StringComparison.Ordinal) + 4).Trim
                strApp = strApp.Substring(0, strApp.LastIndexOf(".exe", StringComparison.Ordinal) + 4)
            End If
            If Not File.Exists(strApp) Then
                Dim CompSettings As New AppSettings(Config)
                CompSettings.Show()
            Else
                Dim strPriFile, strSecFile As String
                '2 files to compare or compare against current file
                strPriFile = lvwVersions.SelectedItems(0).SubItems(2).Text
                If lvwVersions.SelectedItems.Count = 2 Then
                    strSecFile = lvwVersions.SelectedItems(1).SubItems(2).Text
                    If WatchEngine.FTPEnable Then
                        Dim strTempFile As String = strPriFile.Replace("/", "\")
                        If strTempFile.Contains("\") Then strTempFile = strTempFile.Substring(strTempFile.LastIndexOf("\", StringComparison.Ordinal) + 1)
                        strTempFile = My.Computer.FileSystem.SpecialDirectories.Temp & "\" & "1-" & strTempFile
                        Try
                            If Not ftp.Download(strPriFile, strTempFile, True) Then MsgBox(ftp.ErrorText, MsgBoxStyle.Exclamation)
                            strPriFile = strTempFile
                            strTempFile = strSecFile.Replace("/", "\")
                            If strTempFile.Contains("\") Then strTempFile = strTempFile.Substring(strTempFile.LastIndexOf("\", StringComparison.Ordinal) + 1)
                            strTempFile = My.Computer.FileSystem.SpecialDirectories.Temp & "\" & "2-" & strTempFile
                            If Not ftp.Download(strSecFile, strTempFile, True) Then MsgBox(ftp.ErrorText, MsgBoxStyle.Exclamation)
                            strSecFile = strTempFile
                        Catch ex As Exception
                            Log.Debug(ex.Message, "VerExplorer.Compare")
                            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
                        End Try
                    End If
                Else
                    strSecFile = lsv.SelectedItems(0).Tag
                    If WatchEngine.FTPEnable Then
                        Dim strTempFile As String = strPriFile.Replace("/", "\")
                        If strTempFile.Contains("\") Then strTempFile = strTempFile.Substring(strTempFile.LastIndexOf("\", StringComparison.Ordinal) + 1)
                        strTempFile = My.Computer.FileSystem.SpecialDirectories.Temp & "\" & "2-" & strTempFile
                        Try
                            If Not ftp.Download(strPriFile, strTempFile, True) Then MsgBox(ftp.ErrorText, MsgBoxStyle.Exclamation)
                            strPriFile = strTempFile
                        Catch ex As Exception
                            Log.Debug(ex.Message, "VerExplorer.Compare")
                            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
                        End Try
                    End If
                End If

                Try
                    If Not strParams.Contains("""") Then
                        strPriFile = """" & FileUtils.Get83PathIfLong(strPriFile) & """"
                        strSecFile = """" & FileUtils.Get83PathIfLong(strSecFile) & """"
                    End If
                    Process.Start(strApp, String.Format(strParams, strPriFile, strSecFile))
                Catch ex As Exception
                    Log.Debug(ex.Message, "VerExplorer.Compare")
                    MsgBox("Error opening compare app: " & strApp & ": " & ex.Message, MsgBoxStyle.Exclamation)
                End Try
            End If
        End If
    End Sub

    Private Sub tspRestore_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tspRestore.Click
        'Restore files
        ' Dim Log As New Logger
        If ViewingWatch Then
            'Viewing Watch & backup versions
            If lvwVersions.SelectedItems.Count <> 1 Then
                MsgBox("You must select 1 file date to restore", MsgBoxStyle.Exclamation)
            Else
                'If MsgBox("Are you sure you wish to restore the selected files?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                'Dim RelativeFolder As String = LastSelectedFolder.Substring(WatchFolder.Length)
                Dim frmRestoreAs As New RestoreAs
                frmRestoreAs.BackupFile = lvwVersions.SelectedItems(0).SubItems(2).Text
                frmRestoreAs.RestoreAsFile = lsv.SelectedItems(0).Tag
                frmRestoreAs.ftp = ftp
                frmRestoreAs.Show()
                'Try
                '    IO.File.Copy(lvwVersions.SelectedItems(0).SubItems(2).Text, lsv.SelectedItems(0).Tag, True)
                '    Log.Info(lvwVersions.SelectedItems(0).SubItems(2).Text & " to " & lsv.SelectedItems(0).Tag, "Restored")
                'Catch ex As Exception
                '    MsgBox("Error restoring file: " & ex.Message, MsgBoxStyle.Exclamation)
                'End Try
            End If
        Else
            'Viewing backup folder
            'If IO.File.Exists(lsv.SelectedItems(0).Tag) Then
            If lsv.SelectedItems.Count = 0 Then
                MsgBox("You must select 1 file date to restore", MsgBoxStyle.Exclamation)
            Else
                'If MsgBox("Are you sure you wish to restore the selected file?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                'Strip version timestamp
                Dim fiFile As Filesystem.FileInfo
                If WatchEngine.FTPEnable Then
                    fiFile = New Filesystem.FileInfo(lsv.SelectedItems(0).Tag.ToString.Replace("/", "\"))
                Else
                    fiFile = New Filesystem.FileInfo(lsv.SelectedItems(0).Tag)
                End If
                Dim strFileTo As String = fiFile.Name
                If strFileTo.Contains(".") Then strFileTo = strFileTo.Substring(0, strFileTo.LastIndexOf(".", StringComparison.Ordinal))
                If strFileTo.Contains(".") Then
                    strFileTo = strFileTo.Substring(0, strFileTo.LastIndexOf(".", StringComparison.Ordinal))
                Else
                    strFileTo = fiFile.Name
                End If
                'Dim RegExp As New System.Text.RegularExpressions.Regex("[^0-9]", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                'RegExp.Replace(fiFile.Extension, String.Empty)
                'If RegExp.Replace(fiFile.Extension, String.Empty).Length = WatchEngine.DateTimeStamp.Length + 1 Then
                '    strFileTo = fiFile.Name.Substring(0, fiFile.Name.LastIndexOf("."))
                'ElseIf fiFile.Name.Contains(".") Then
                '    fiFile = New FileInfo(fiFile.Name.Substring(0, fiFile.Name.LastIndexOf(".")))
                '    If RegExp.Replace(fiFile.Extension, String.Empty).Length = WatchEngine.DateTimeStamp.Length + 1 Then
                '        strFileTo = fiFile.Name.Substring(0, fiFile.Name.LastIndexOf("."))
                '    End If
                'End If
                Dim frmRestoreAs As New RestoreAs
                frmRestoreAs.BackupFile = lsv.SelectedItems(0).Tag
                frmRestoreAs.RestoreAsFile = WatchFolder & strFileTo
                frmRestoreAs.ftp = ftp
                frmRestoreAs.Show()
                'Try
                '    Logger.Info(strFile & " to " & WatchFolder & strFileTo, "Restored")
                '    IO.File.Copy(strFile, WatchFolder & strFileTo, True)
                'Catch ex As Exception
                '    MsgBox("Error restoring file: " & ex.Message, MsgBoxStyle.Exclamation)
                'End Try
            End If
            'End If 'IsFile
        End If
    End Sub

    Private Sub tsbDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles tsbDelete.Click
        'Delete files
        If ViewingWatch Then
            'Viewing Watch & backup versions
            If lvwVersions.SelectedItems.Count = 0 Then
                MsgBox("You must select a file to delete", MsgBoxStyle.Exclamation)
            ElseIf MsgBox("Are you sure you wish to delete the selected files?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                For Each lvi As ListViewItem In lvwVersions.SelectedItems
                    Try
                        If WatchEngine.FTPEnable Then
                            If Not ftp.FtpDelete(lvi.SubItems(2).Text) Then Throw New Exception(ftp.ErrorText)
                        ElseIf UseRecycleBin Then
                            Dim FileHelper As New FileUtils()
                            FileHelper.DeleteFileToRecycleBin(lvi.SubItems(2).Text)
                        Else
                            Filesystem.File.Delete(lvi.SubItems(2).Text)
                        End If
                        Log.Info(lvi.SubItems(2).Text, "Deleted")
                    Catch ex As Exception
                        Log.Debug(ex.Message, "VerExplorer.Delete")
                        MsgBox("Error deleting file: " & ex.Message, MsgBoxStyle.Exclamation)
                    End Try
                Next
                GetVersions(lsv.SelectedItems(0).Tag)
            End If
        ElseIf _
            MsgBox("Are you sure you wish to delete the selected original files?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            'Viewing backup folder
            If lsv.SelectedItems.Count = 0 Then
                MsgBox("You must select a file to delete", MsgBoxStyle.Exclamation)
            Else
                For Each lvi As ListViewItem In lsv.SelectedItems
                    Try
                        If WatchEngine.FTPEnable Then
                            If Not ftp.FtpDelete(lvi.Tag) Then Throw New Exception(ftp.ErrorText)
                            Log.Info(lvi.Tag, "Deleted")
                        Else
                            If Filesystem.File.Exists(lvi.Tag) Then
                                If UseRecycleBin Then
                                    Dim FileHelper As New FileUtils()
                                    FileHelper.DeleteFileToRecycleBin(lvi.Tag)
                                Else
                                    Filesystem.File.Delete(lvi.Tag)
                                End If
                                Log.Info(lvi.Tag, "Deleted")
                            End If
                        End If
                    Catch ex As Exception
                        Log.Debug(ex.Message, "VerExplorer.Delete")
                        MsgBox("Error deleting file: " & ex.Message, MsgBoxStyle.Exclamation)
                    End Try
                Next
                For intItem As Integer = lsv.Items.Count - 1 To 0 Step -1
                    If lsv.Items(intItem).Selected Then lsv.Items.RemoveAt(intItem)
                Next
            End If
        End If
    End Sub

    '**** Context menu
    Private Sub OpenViewFileToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OpenViewFileToolStripMenuItem.Click
        tspView_Click(sender.owner.tag, Nothing)
    End Sub

    Private Sub CompareFilesToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CompareFilesToolStripMenuItem.Click
        tspCompare_Click(Nothing, Nothing)
    End Sub

    Private Sub RestoreAsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RestoreAsToolStripMenuItem.Click
        tspRestore_Click(sender.owner.tag, Nothing)
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteToolStripMenuItem.Click
        tsbDelete_Click(Nothing, Nothing)
    End Sub

    Private Sub lvwVersions_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lvwVersions.MouseClick
        'Right click versions
        If e.Button = MouseButtons.Right Then
            ContextMenuStrip1.Tag = Nothing
            ContextMenuStrip1.Items(1).Enabled = True
            ContextMenuStrip1.Items(2).Enabled = True
            ContextMenuStrip1.Items(4).Enabled = True
            ContextMenuStrip1.Show(lvwVersions, e.Location.X, e.Location.Y)
        End If
    End Sub

    Private Sub lsv_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lsv.MouseClick
        'Right click versions
        If e.Button = MouseButtons.Right Then
            ContextMenuStrip1.Tag = "lsv"
            ContextMenuStrip1.Items(1).Enabled = False 'compare
            If ViewingWatch Then
                ContextMenuStrip1.Items(2).Enabled = False 'restore
                ContextMenuStrip1.Items(4).Enabled = False 'delete
            Else
                ContextMenuStrip1.Items(2).Enabled = True 'restore
                ContextMenuStrip1.Items(4).Enabled = True 'delete
            End If
            ContextMenuStrip1.Show(lsv, e.Location.X, e.Location.Y)
        End If
    End Sub

    Private Sub lvwVersions_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles lvwVersions.DoubleClick
        'double click versions
        tspView_Click(Nothing, Nothing)
    End Sub

    Private Sub lsv_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles lsv.DoubleClick
        'double click versions
        tspView_Click("lsv", Nothing)
    End Sub

    Private Sub lvwVersions_ColumnClick(ByVal sender As Object, ByVal e As ColumnClickEventArgs) _
        Handles lvwVersions.ColumnClick
        ' Determine if the clicked column is already the column that is being sorted.
        If (e.Column = lvwVersionsSorter.SortColumn) Then
            ' Reverse the current sort direction for this column.
            If (lvwVersionsSorter.Order = SortOrder.Ascending) Then
                lvwVersionsSorter.Order = SortOrder.Descending
            Else
                lvwVersionsSorter.Order = SortOrder.Ascending
            End If
        Else
            ' Set the column number that is to be sorted; default to ascending.
            lvwVersionsSorter.SortColumn = e.Column
            lvwVersionsSorter.Order = SortOrder.Ascending
        End If
        ' Perform the sort with these new sort options.
        Me.lvwVersions.Sort()
    End Sub

    Private Sub lsv_ColumnClick(ByVal sender As Object, ByVal e As ColumnClickEventArgs) Handles lsv.ColumnClick
        ' Determine if the clicked column is already the column that is being sorted.
        If (e.Column = lvw1Sorter.SortColumn) Then
            ' Reverse the current sort direction for this column.
            If (lvw1Sorter.Order = SortOrder.Ascending) Then
                lvw1Sorter.Order = SortOrder.Descending
            Else
                lvw1Sorter.Order = SortOrder.Ascending
            End If
        Else
            ' Set the column number that is to be sorted; default to ascending.
            lvw1Sorter.SortColumn = e.Column
            lvw1Sorter.Order = SortOrder.Ascending
        End If
        ' Perform the sort with these new sort options.
        Me.lsv.Sort()
    End Sub
End Class


Public Class IconHandler

#Region "API Declarations"

    <DllImport("shell32.dll", CallingConvention := CallingConvention.Cdecl)> _
    Private Shared Function ExtractIcon _
    (ByVal hIcon As IntPtr, _
    ByVal lpszExeFileName As String, _
    ByVal nIconIndex As Integer) As IntPtr
    End Function

    <DllImport("user32", CallingConvention := CallingConvention.Cdecl)> _
    Private Shared Function DestroyIcon _
    (ByVal hIcon As IntPtr) As Boolean
    End Function

#End Region

    'This array holds the fileextensions this class has already extracted. It checks it
    'so it doesn't extract the same icon twice (unless two extensions has the same icon)
    'It also holds the name of .exe files which icons we have extracted
    Public ExtArray As New ArrayList

    'This sub extracts an specified icon from a specified file. I use it to extract the
    'icon from .exe files, which naturally aren't stored in the registry
    Public Function AddIconByFile(ByVal Filename As String, ByVal IconIndex As Integer, _
                                  ByRef SmallImageList As ImageList, _
                                  Optional ByVal LargeImageList As ImageList = Nothing, _
                                  Optional ByVal Generic As Boolean = False) As Integer
        'First, scan through the ExtArray to see if we already stored this icon
        'That is, if it's not trying to get a generic icon (indicated by the boolean)
        If Not Generic And ExtArray.IndexOf(Filename) >= 0 Then Return ExtArray.IndexOf(Filename) ': Exit Function

        'Get the icon handle
        Dim hIcon As IntPtr = ExtractIcon(IntPtr.Zero, Filename, IconIndex)

        'If the file is not found, it returns 0, and if the icon is not found it returns 1.
        'Get the default file icon if that should be the case
        If hIcon.ToInt32 <= 1 Then hIcon = ExtractIcon(IntPtr.Zero, "Shell32.dll", 0)

        'Then add the icon to the imagelist(s)
        Dim test As Icon
        test = Icon.FromHandle(hIcon)
        SmallImageList.Images.Add(Resize16(test.ToBitmap))
        If Not LargeImageList Is Nothing Then LargeImageList.Images.Add(test)

        'Clean up and add Filename to ExtArray
        DestroyIcon(hIcon)
        ExtArray.Add(Filename)

        'Return the index of the new icon, which is the last image in the imagelist
        Return SmallImageList.Images.Count - 1
    End Function

    'This sub checks what default icon is associated with a file extension, then it pretty
    'much does the same as AddIconByFile
    Public Function AddIconByExt(ByVal Ext As String, ByRef SmallImageList As ImageList, _
                                 Optional ByVal LargeImageList As ImageList = Nothing) As Integer
        'No need to add an icon, if it already exists in the imagelist
        If ExtArray.IndexOf(Ext) >= 0 Then Return ExtArray.IndexOf(Ext) ': Exit Function

        'Then try to find the default icon in the registry. The extension is stored in HKEY_CLASSES_ROOT
        'and it contains the name of the key contining the DefaultIcon property.
        'The DefaultIcon is a string of the format <filename>,<iconindex> so we just split the string
        'Based on the ","
        Dim str As String
        Dim IconIndex As Integer
        Dim hIcon As IntPtr
        Try
            str = Registry.ClassesRoot.OpenSubKey(Ext).GetValue(String.Empty)
            str = Registry.ClassesRoot.OpenSubKey(str).OpenSubKey("DefaultIcon").GetValue(String.Empty)
            IconIndex = str.Split(",")(1)
            str = str.Split(",")(0)

            hIcon = ExtractIcon(IntPtr.Zero, str, IconIndex)
            SmallImageList.Images.Add(Resize16(Icon.FromHandle(hIcon).ToBitmap))
            If Not LargeImageList Is Nothing Then LargeImageList.Images.Add(Icon.FromHandle(hIcon))
        Catch
            'Should we fail to find an associated icon, then use the default one
            str = "Shell32.dll"
            IconIndex = 0

            hIcon = ExtractIcon(IntPtr.Zero, str, IconIndex)
            SmallImageList.Images.Add(Resize16(Icon.FromHandle(hIcon).ToBitmap))
            If Not LargeImageList Is Nothing Then LargeImageList.Images.Add(Icon.FromHandle(hIcon))
        End Try

        DestroyIcon(hIcon)
        ExtArray.Add(Ext)
        Return SmallImageList.Images.Count - 1
    End Function

    'This routine just adds the (closed) folder icon. I use it before I add any other icons
    'so I can be sure that the icon's index in the imagelist is 0
    Public Sub AddFolder(ByRef SmallImageList As ImageList, Optional ByVal LargeImageList As ImageList = Nothing)
        Dim hIcon As IntPtr = ExtractIcon(IntPtr.Zero, "Shell32.dll", 3)

        SmallImageList.Images.Add(Resize16(Icon.FromHandle(hIcon).ToBitmap))
        If Not LargeImageList Is Nothing Then LargeImageList.Images.Add(Icon.FromHandle(hIcon))
        DestroyIcon(hIcon)
        'To not screw up the indexes in the extarray and the imagelists, add a bogus entry to the array
        ExtArray.Add("folder")
    End Sub

    'Resize the icons ourselves so that they are smoother
    Private Function Resize16(ByRef bmap As Bitmap) As Bitmap
        Dim bmPhoto As Bitmap = New Bitmap(16, 16, PixelFormat.Format32bppArgb)
        bmPhoto.SetResolution(72, 72)
        Dim grPhoto As Graphics = Graphics.FromImage(bmPhoto)
        'grPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
        grPhoto.InterpolationMode = InterpolationMode.High
        ' grPhoto.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality
        grPhoto.DrawImage(bmap, New Rectangle(0, 0, 16, 16), 0, 0, bmap.Width, bmap.Height, GraphicsUnit.Pixel)
        Return bmPhoto
    End Function
End Class
