Imports System.ComponentModel
Imports System.ServiceModel
Imports System.ServiceProcess
Imports System.Configuration
Imports System.Configuration.Install

<ServiceContract(Namespace:="http://Beanland.AutoVer.Config")> _
Public Interface IConfigEngine

    'Method implementations of properties (purely for WCF)

    <OperationContract()> _
    Function GetUserMesage() As String

    <OperationContract()> _
    Function GetIsService() As Boolean
    '<OperationContract()> _
    'Sub SetIsService(ByVal isService As Boolean)

    '<OperationContract()> _
    'Function GetWatcherConfig() As DataTable

    <OperationContract()> _
    Function GetAppConfig() As Generic.Dictionary(Of String, String)

    <OperationContract()> _
    Function GetAppConfigFolder() As String

    <OperationContract()> _
    Function GetConfigFolder() As String

    <OperationContract()> _
    Function GetWatcherStatus() As List(Of WatcherStatus)

    '<OperationContract()> _
    'Function GetWatcherEngines() As List(Of BackupEngine)

    '<OperationContract()> _
    'Function GetLastSelectedWatcher() As Guid

    'Pure methods

    <OperationContract()> _
    Sub LoadAppConfig()

    <OperationContract()> _
    Sub SaveAppConfig()

    <OperationContract()> _
    Sub LoadWatcherConfig()

    <OperationContract()> _
    Sub SaveWatcherConfig()

    <OperationContract()> _
    Sub UpdateWatchersList()

    <OperationContract()> _
    Function AppConfigDefault(ByVal Key As String, ByVal DefaultValue As String) As String

    <OperationContract()> _
    Sub ReloadWatcher(ByVal watcherId As Guid)

    <OperationContract()> _
    Sub DeleteWatcher(ByVal watcherId As Guid)

    <OperationContract()> _
    Sub PauseWatcher(ByVal watcherId As Guid)
    
End Interface

<Serializable()> _
Public Class WatcherStatus

    Public Property WatcherId As Guid
    Public Property Name As String
    Public Property TotalEvents As Integer
    Public Property Statistics As String
    Public Property Enabled As Boolean
    Public Property Started As Boolean
    Public Property Paused As Boolean
    Public Property BackupFolderFailure As Boolean
    Public Property UserMessage As String

End Class