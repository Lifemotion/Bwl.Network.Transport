Imports System.Timers
Imports Bwl.Framework
Imports Bwl.Network.Transport

Public Class RepeaterCore
    Private WithEvents _netServer As New TCPAddressedServer
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
        _netServer.AllowBroadcastSetting = True
        _netServer.Server.Open("*:" + _port.Value.ToString, "")
        _logger.AddMessage("Created server on " + _port.Value.ToString)
    End Sub

    Public ReadOnly Property PortSetting As IntegerSetting
        Get
            Return _port
        End Get
    End Property

    Private Sub _netServer_RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Handles _netServer.RegisterClientRequest
        allowRegister = True
    End Sub

    Private Sub _netServer_PacketReceived(channel As IAddressedChannel, packet As StructuredPacket) Handles _netServer.PacketReceived
        Try
            SyncLock _netServer
                If channel.MyID > "" Then
                    If LogMessages Then _logger.AddInformation(channel.MyID + "-> " + packet.ToString)
                    _netServer.SendPacket(packet)
                Else
                    _logger.AddWarning(channel.MyID.ToString + "-> " + "Trying to use repeater without registered id, from ") '+ client.IPAddress)
                End If
            End SyncLock
        Catch ex As Exception
            _logger.AddError(ex.Message)
        End Try

    End Sub

    Private Sub _netServer_PacketSent(channel As IAddressedChannel, packet As StructuredPacket) Handles _netServer.PacketSent
        If LogMessages Then _logger.AddInformation(channel.MyID + "<- " + packet.ToString)
    End Sub

    Private Sub _netServer_NewConnection(server As IAddressedServer, connection As IAddressedChannel) Handles _netServer.NewConnection
        '   _logger.AddMessage("Connected #" + connection.channel.ToString) '+ ", " + transport.IPAddress + ", " + client.ConnectionTime.ToString + "")
    End Sub

    Public ReadOnly Property NetServer As TCPAddressedServer
        Get
            Return _netServer
        End Get
    End Property
End Class
