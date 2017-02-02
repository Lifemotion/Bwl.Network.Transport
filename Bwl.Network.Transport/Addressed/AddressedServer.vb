Imports Bwl.Network.Transport

Public Class AddressedServer
    Implements IAddressedServer, IConnectionControl

    Protected _server As IPacketServer

    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IAddressedServer.RegisterClientRequest
    Public Event PacketReceived(transport As IAddressedChannel, packet As StructuredPacket) Implements IAddressedChannel.PacketReceived
    Public Event PacketSent(transport As IAddressedChannel, packet As StructuredPacket) Implements IAddressedChannel.PacketSent
    Public Event NewConnection(server As IAddressedServer, connection As IAddressedChannel) Implements IAddressedServer.NewConnection
    Public ReadOnly Property MyID As String = "(Server)" Implements IAddressedChannel.MyID
    Public ReadOnly Property MyServiceName As String = "" Implements IAddressedChannel.MyServiceName

    Public Property AllowBroadcastSetting As Boolean = False

    Public ReadOnly Property Server As IPacketServer Implements IAddressedServer.Server
        Get
            Return _server
        End Get
    End Property

    Public ReadOnly Property ID As Long Implements IConnectionControl.ID
        Get
            Return _server.ID
        End Get
    End Property

    Public ReadOnly Property IsConnected As Boolean Implements IConnectionControl.IsConnected
        Get
            Return _server.IsConnected
        End Get
    End Property

    Public Sub Dispose() Implements IDisposable.Dispose
        _server.Dispose()
    End Sub

    Public Sub Open(address As String, options As String) Implements IConnectionControl.Open
        _server.Open(address, options)
    End Sub

    Public Sub Close() Implements IConnectionControl.Close
        _server.Close()
    End Sub

    Public ReadOnly Property ActiveConnections As IAddressedChannel() Implements IAddressedServer.ActiveConnections
        Get
            Dim list As New List(Of IAddressedClient)
            For Each client In _server.ActiveConnections.ToArray
                list.Add(client.Tag)
            Next
            Return list.ToArray
        End Get
    End Property

    Friend Sub New(server As IPacketServer)
        _server = server
        AddHandler _server.NewConnection, AddressOf NewConnectionHandler
    End Sub

    Private Sub NewConnectionHandler(server As IPacketPortListener, connection As IConnectedChannel)
        Dim addressedChannel = New AddressedChannelBase(connection.Channel)
        connection.Tag = addressedChannel
        AddHandler addressedChannel.PacketReceived, Sub(transport As IAddressedChannel, packet As StructuredPacket)
                                                        If PacketReceivedHandler(packet, transport) = False Then RaiseEvent PacketReceived(transport, packet)
                                                    End Sub
        AddHandler addressedChannel.PacketSent, Sub(transport As IAddressedChannel, packet As StructuredPacket) RaiseEvent PacketSent(transport, packet)
        RaiseEvent NewConnection(Me, addressedChannel)
    End Sub

    Private Function PacketReceivedHandler(packet As StructuredPacket, transport As IAddressedChannel)
        If packet.Parts.ContainsKey("@RegisterMe") Then
            Try
                Dim id As String = packet.Parts("@RegisterMe")
                Dim pass As String = packet.Parts("@RegisterPassword")
                Dim service As String = packet.Parts("@RegisterService")
                Dim method As String = packet.Parts("@RegisterMethod")
                Dim options As String = packet.Parts("@RegisterOptions")
                Dim info As New Dictionary(Of String, String)
                Dim allow As Boolean
                Dim returnInfo As String = ""
                RaiseEvent RegisterClientRequest(info, id, method, pass, service, options, allow, returnInfo)
                Dim response As New StructuredPacket(packet)
                response.Add("@RegisterResultMessage", returnInfo)
                If allow And id > "" Then
                    response.Add("@RegisterResult", "OK")
                    transport.RegisterMe(id, pass, service, options)
                Else
                    response.Add("@RegisterResult", "NotAllowed")
                End If
                transport.SendPacketAsync(response)
            Catch ex As Exception
                Dim response As New StructuredPacket(packet)
                response.Add("@RegisterResult", "Error")
                response.Add("@RegisterResultMessage", "Unknown error")
                Try
                    transport.SendPacketAsync(response)
                Catch ex1 As Exception
                End Try
            End Try
            Return True
        ElseIf packet.Parts.ContainsKey("@GetPeersList") Then
            Try
                Dim service As String = packet.Parts("@GetPeersList")
                Dim response As New StructuredPacket(packet)
                Dim list = GetPeersList(service).ToArray
                response.Add("@PeersList", list)
                transport.SendPacketAsync(response)
            Catch ex As Exception
            End Try
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IAddressedChannel.RegisterMe
        _MyID = id
        _MyServiceName = serviceName
    End Sub

    Private Sub SendPacket(message As StructuredPacket, async As Boolean)
        If message.AddressFrom = "" Then message.AddressFrom = MyID
        If message.AddressTo = "" Then
            If AllowBroadcastSetting Then
                For Each client In _server.ActiveConnections.ToArray
                    Try
                        If async Then client.Tag.SendPacketAsync(message) Else client.Tag.SendPacket(message)
                    Catch ex As Exception
                    End Try
                Next
            Else
                Throw New Exception("SendPacket: Broadcasts not allowed")
            End If
        Else
            For Each client In _server.ActiveConnections.ToArray
                If client.Tag.MyID.ToLower = message.AddressTo.ToLower Then
                    If async Then client.Tag.SendPacketAsync(message) Else client.Tag.SendPacket(message)
                    Return
                End If
            Next
            Throw New Exception("SendPacket: Client with address " + message.AddressTo + " not found")
        End If
    End Sub


    Public Sub SendPacket(message As StructuredPacket) Implements IAddressedChannel.SendPacket
        SendPacket(message, False)
    End Sub

    Public Sub SendPacketAsync(message As StructuredPacket) Implements IAddressedChannel.SendPacketAsync
        SendPacket(message, True)
    End Sub

    Public Function GetPeersList(serviceName As String, Optional timeout As Single = 20) As String() Implements IAddressedChannel.GetPeersList
        Dim list As New List(Of String)
        If serviceName = "" Or serviceName = MyServiceName Then list.Add(MyID)
        For Each client In _server.ActiveConnections.ToArray
            Try
                If client.Tag.MyServiceName.ToLower = serviceName.ToLower Or serviceName = "" Then
                    If client.Tag.MyID > "" Then list.Add(client.Tag.MyID)
                End If
            Catch ex As Exception
            End Try
        Next
        Return list.ToArray
    End Function

    Public Function SendPacketWaitAnswer(message As StructuredPacket, Optional timeout As Single = 20) As StructuredPacket Implements IAddressedChannel.SendPacketWaitAnswer
        Throw New NotImplementedException()
    End Function

    Public Function WaitPacket(Optional timeout As Single = 20, Optional pktid As Integer = -1, Optional partKey As String = "") As Object Implements IAddressedChannel.WaitPacket
        Throw New NotImplementedException()
    End Function

End Class
