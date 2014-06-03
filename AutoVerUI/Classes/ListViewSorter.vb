Imports System.Collections
Imports System.Windows.Forms

Public Class ListViewColumnSorter
    Implements System.Collections.IComparer

    Private ColumnToSort As Integer
    Private OrderOfSort As SortOrder
    Private ObjectCompare As CaseInsensitiveComparer

    Public Sub New()
        ' Initialize the column to '0'.
        ColumnToSort = 0

        ' Initialize the sort order to 'none'.
        OrderOfSort = SortOrder.None

        ' Initialize the CaseInsensitiveComparer object.
        ObjectCompare = New CaseInsensitiveComparer()
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim compareResult As Integer
        Dim listviewX As ListViewItem
        Dim listviewY As ListViewItem

        ' Cast the objects to be compared to ListViewItem objects.
        listviewX = CType(x, ListViewItem)
        listviewY = CType(y, ListViewItem)

        ' Compare the two items. Try converting to date, size, and decimal before sorting.
        Dim decX, decY As Decimal
        Dim strX, strY As String
        strX = listviewX.SubItems(ColumnToSort).Text
        strY = listviewY.SubItems(ColumnToSort).Text
        Dim datX, datY As DateTime
        If DateTime.TryParse(strX, datX) And DateTime.TryParse(strY, datY) Then
            compareResult = ObjectCompare.Compare(datX, datY)
        ElseIf strX.EndsWith("KB") And strY.EndsWith("KB") Then
            strX = strX.Replace("KB", String.Empty)
            strY = strY.Replace("KB", String.Empty)
            If Decimal.TryParse(strX, decX) And Decimal.TryParse(strY, decY) Then
                compareResult = ObjectCompare.Compare(decX, decY)
            Else
                compareResult = ObjectCompare.Compare(listviewX.SubItems(ColumnToSort).Text, listviewY.SubItems(ColumnToSort).Text)
            End If
        ElseIf Decimal.TryParse(strX, decX) And Decimal.TryParse(strY, decY) Then
            compareResult = ObjectCompare.Compare(decX, decY)
        Else
            compareResult = ObjectCompare.Compare(strX, strY)
        End If
        ' Calculate the correct return value based on the object 
        ' comparison.
        If (OrderOfSort = SortOrder.Ascending) Then
            ' Ascending sort is selected, return typical result of 
            ' compare operation.
            Return compareResult
        ElseIf (OrderOfSort = SortOrder.Descending) Then
            ' Descending sort is selected, return negative result of 
            ' compare operation.
            Return (-compareResult)
        Else
            ' Return '0' to indicate that they are equal.
            Return 0
        End If
    End Function

    Public Property SortColumn() As Integer
        Set(ByVal Value As Integer)
            ColumnToSort = Value
        End Set

        Get
            Return ColumnToSort
        End Get
    End Property

    Public Property Order() As SortOrder
        Set(ByVal Value As SortOrder)
            OrderOfSort = Value
        End Set

        Get
            Return OrderOfSort
        End Get
    End Property
End Class
