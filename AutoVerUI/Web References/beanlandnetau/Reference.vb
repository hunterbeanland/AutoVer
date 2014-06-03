﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.17929
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.17929.
'
Namespace beanlandnetau
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="BeanAppServiceSoap", [Namespace]:="http://beanland.net.au/")>  _
    Partial Public Class BeanAppService
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private LatestVersionOperationCompleted As System.Threading.SendOrPostCallback
        
        Private ReportErrorOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = Global.AutoVer.My.MySettings.Default.SymPulsePollAlertWin_beanlandnetau_BeanAppService
            If (Me.IsLocalFileSystemWebService(Me.Url) = true) Then
                Me.UseDefaultCredentials = true
                Me.useDefaultCredentialsSetExplicitly = false
            Else
                Me.useDefaultCredentialsSetExplicitly = true
            End If
        End Sub
        
        Public Shadows Property Url() As String
            Get
                Return MyBase.Url
            End Get
            Set
                If (((Me.IsLocalFileSystemWebService(MyBase.Url) = true)  _
                            AndAlso (Me.useDefaultCredentialsSetExplicitly = false))  _
                            AndAlso (Me.IsLocalFileSystemWebService(value) = false)) Then
                    MyBase.UseDefaultCredentials = false
                End If
                MyBase.Url = value
            End Set
        End Property
        
        Public Shadows Property UseDefaultCredentials() As Boolean
            Get
                Return MyBase.UseDefaultCredentials
            End Get
            Set
                MyBase.UseDefaultCredentials = value
                Me.useDefaultCredentialsSetExplicitly = true
            End Set
        End Property
        
        '''<remarks/>
        Public Event LatestVersionCompleted As LatestVersionCompletedEventHandler
        
        '''<remarks/>
        Public Event ReportErrorCompleted As ReportErrorCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://beanland.net.au/LatestVersion", RequestNamespace:="http://beanland.net.au/", ResponseNamespace:="http://beanland.net.au/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function LatestVersion(ByVal AppID As String) As String
            Dim results() As Object = Me.Invoke("LatestVersion", New Object() {AppID})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub LatestVersionAsync(ByVal AppID As String)
            Me.LatestVersionAsync(AppID, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub LatestVersionAsync(ByVal AppID As String, ByVal userState As Object)
            If (Me.LatestVersionOperationCompleted Is Nothing) Then
                Me.LatestVersionOperationCompleted = AddressOf Me.OnLatestVersionOperationCompleted
            End If
            Me.InvokeAsync("LatestVersion", New Object() {AppID}, Me.LatestVersionOperationCompleted, userState)
        End Sub
        
        Private Sub OnLatestVersionOperationCompleted(ByVal arg As Object)
            If (Not (Me.LatestVersionCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent LatestVersionCompleted(Me, New LatestVersionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://beanland.net.au/ReportError", RequestNamespace:="http://beanland.net.au/", ResponseNamespace:="http://beanland.net.au/", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub ReportError(ByVal AppID As String, ByVal UserID As String, ByVal ErrorLog As String, <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")> ByVal ErrorScreenshot() As Byte)
            Me.Invoke("ReportError", New Object() {AppID, UserID, ErrorLog, ErrorScreenshot})
        End Sub
        
        '''<remarks/>
        Public Overloads Sub ReportErrorAsync(ByVal AppID As String, ByVal UserID As String, ByVal ErrorLog As String, ByVal ErrorScreenshot() As Byte)
            Me.ReportErrorAsync(AppID, UserID, ErrorLog, ErrorScreenshot, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub ReportErrorAsync(ByVal AppID As String, ByVal UserID As String, ByVal ErrorLog As String, ByVal ErrorScreenshot() As Byte, ByVal userState As Object)
            If (Me.ReportErrorOperationCompleted Is Nothing) Then
                Me.ReportErrorOperationCompleted = AddressOf Me.OnReportErrorOperationCompleted
            End If
            Me.InvokeAsync("ReportError", New Object() {AppID, UserID, ErrorLog, ErrorScreenshot}, Me.ReportErrorOperationCompleted, userState)
        End Sub
        
        Private Sub OnReportErrorOperationCompleted(ByVal arg As Object)
            If (Not (Me.ReportErrorCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent ReportErrorCompleted(Me, New System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        Public Shadows Sub CancelAsync(ByVal userState As Object)
            MyBase.CancelAsync(userState)
        End Sub
        
        Private Function IsLocalFileSystemWebService(ByVal url As String) As Boolean
            If ((url Is Nothing)  _
                        OrElse (url Is String.Empty)) Then
                Return false
            End If
            Dim wsUri As System.Uri = New System.Uri(url)
            If ((wsUri.Port >= 1024)  _
                        AndAlso (String.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) = 0)) Then
                Return true
            End If
            Return false
        End Function
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")>  _
    Public Delegate Sub LatestVersionCompletedEventHandler(ByVal sender As Object, ByVal e As LatestVersionCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class LatestVersionCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")>  _
    Public Delegate Sub ReportErrorCompletedEventHandler(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
End Namespace