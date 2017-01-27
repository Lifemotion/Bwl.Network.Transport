Imports System.Timers
Imports Bwl.Framework
Imports Bwl.Network.Transport

Public Class RepeaterCore
    Private WithEvents _netServer As TCPServer
    Private _logger As Logger
    Private _storage As SettingsStorage
    Private _port As IntegerSetting

    Public Property LogMessages As Boolean = False

    Public Sub New(app As AppBase)
        _storage = app.RootStorage.CreateChildStorage("NetClientRepeater")
        _logger = app.RootLogger.CreateChildLogger("NetClientRepeater")
        _port = _storage.CreateIntegerSetting("Port", 4180)
    End Sub

    Public Sub Start()
        _netServer = New TCPServer(_port.Value)
        _logger.AddMessage("Created server on " + _port.Value.ToString)
    End Sub

    Public ReadOnly Property PortSetting As IntegerSetting
        Get
            Return _port
        End Get
    End Property

    Private Sub _netServer_ReceivedPacket(transport As IPacketTransport, packet As BytePacket) Handles _netServer.ReceivedPacket
        Try
            SyncLock _netServer
                '  If client.RegisteredID > "" Then
                '    If LogMessages Then _logger.AddInformation(client.RegisteredID + "-> " + Message.ToString)
                '    _netServer.SendMessage(Message)
                '  Else
                '     _logger.AddWarning(client.ID.ToString + "-> " + "Trying to use repeater without registered id, from " + client.IPAddress)
                '  End If
            End SyncLock
        Catch ex As Exception
            _logger.AddError(ex.Message)
        End Try
    End Sub

    Private Sub _netServer_SentPacket(transport As IPacketTransport, packet As BytePacket) Handles _netServer.SentPacket
        ' If LogMessages Then _logger.AddInformation(client.RegisteredID + "<- " + Message.ToString)
    End Sub

    Private Sub _netServer_NewConnection(server As IPortListener, transport As IPacketTransport) Handles _netServer.NewConnection
        _logger.AddMessage("Connected #" + transport.ID.ToString) '+ ", " + transport.IPAddress + ", " + client.ConnectionTime.ToString + "")
    End Sub

    Public ReadOnly Property NetServer As TCPServer
        Get
            Return _netServer
        End Get
    End Property
End Class
