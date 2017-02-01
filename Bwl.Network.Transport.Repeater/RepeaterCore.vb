Imports System.Timers
Imports Bwl.Framework
Imports Bwl.Network.Transport

Public Class RepeaterCore
    Private WithEvents _netServer As TCPAddressedServer
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
        _netServer = New TCPServer
        _logger.AddMessage("Created server on " + _port.Value.ToString)
    End Sub

    Public ReadOnly Property PortSetting As IntegerSetting
        Get
            Return _port
        End Get
    End Property

    Private Sub _netServer_ReceivedPacket(transport As IPacketChannel, packet As StructuredPacket) Handles _netServer.PacketReceived

    End Sub

    Private Sub _netServer_SentPacket(transport As IPacketChannel, packet As StructuredPacket) Handles _netServer.SentPacket
        ' If LogMessages Then _logger.AddInformation(client.RegisteredID + "<- " + Message.ToString)
    End Sub

    Private Sub _netServer_NewConnection(server As IPacketPortListener, transport As IPacketChannel) Handles _netServer.NewConnection
        _logger.AddMessage("Connected #" + transport.ID.ToString) '+ ", " + transport.IPAddress + ", " + client.ConnectionTime.ToString + "")
    End Sub

    Private Sub _netServer_ServerReceivedPacket(connection As IConnectedChannel, packet As BytePacket) Handles _netServer.ServerReceivedPacket
        Try
            SyncLock _netServer
                If connection.Info.ID > "" Then
                    If LogMessages Then _logger.AddInformation(connection.Info.ID + "-> " + packet.ToString)
                    _netServer.SendPacket(packet)
                Else
                    _logger.AddWarning(client.ID.ToString + "-> " + "Trying to use repeater without registered id, from " + client.IPAddress)
                End If
            End SyncLock
        Catch ex As Exception
            _logger.AddError(ex.Message)
        End Try
    End Sub

    Private Sub _netServer_ServerSentPacket(connection As IConnectedChannel, packet As BytePacket) Handles _netServer.ServerSentPacket

    End Sub

    Public ReadOnly Property NetServer As TCPServer
        Get
            Return _netServer
        End Get
    End Property
End Class
