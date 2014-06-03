Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AutoVer

<TestClass()> Public Class UnitTest1
    Dim fc As FileFolderFilter

    <TestInitialize> Public Sub FileUtilsInit()
        fc = New FileFolderFilter()
        fc.SetupFilters("*.txt", "temp.*;other", "c:\path\nextpath;otherpath;\anotherpath\other2;temp*;single?")
    End Sub

    <TestMethod()> Public Sub CanCopyIncludeFile()
        Assert.IsTrue(fc.CanCopy("abc.txt", "c:\abc\"), "CanCopyIncludeFile")
    End Sub

    <TestMethod()> Public Sub CanCopyExcludeFile()
        Assert.IsFalse(fc.CanCopy("abc.vb", "c:\abc"), "ExcludeFile")
        Assert.IsFalse(fc.CanCopy("temp.txt", "c:\txt\"), "ExcludeFile2")
    End Sub

    <TestMethod()> Public Sub CanCopyInludeFolder()
        Assert.IsTrue(fc.CanCopy("abc.txt", "c:\abc\"), "Include")
        Assert.IsTrue(fc.CanCopy("abc.txt", "c:\anotherpath\other1\2"), "SimilarMulti")
        Assert.IsTrue(fc.CanCopy("abc.txt", "c:\path\single12\nextpath\"), "SingleWildCard2")
    End Sub

    <TestMethod()> Public Sub CanCopyExcludeFolder()
        Assert.IsFalse(fc.CanCopy("abc.txt", "c:\path\nextpath\"), "ExcludePath")
        Assert.IsFalse(fc.CanCopy("abc.txt", "c:\path\anotherpath\other2\nextpath\"), "ExcludePathMulti")
        Assert.IsFalse(fc.CanCopy("abc.txt", "c:\path\temp34\nextpath\"), "Wildcard")
        Assert.IsFalse(fc.CanCopy("abc.txt", "c:\path\single1\nextpath\"), "SingleWildCard")
    End Sub

End Class