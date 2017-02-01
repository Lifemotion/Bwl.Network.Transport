Imports Bwl.Network.Transport

Public Class TCPClientInfo
    Public Property ID As String = ""
    Public Property ServiceName As String = ""
End Class

Public Class TCPAddressedServer
    Inherits TCPServer
    Implements IAddressedChannel

    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IAddressedChannel.RegisterClientRequest
    Public Event PacketReceived(transport As IPacketChannel, packet As StructuredPacket) Implements IAddressedChannel.PacketReceived
    Public Event PacketSent(transport As IPacketChannel, packet As StructuredPacket) Implements IAddressedChannel.PacketSent

    Public ReadOnly Property MyID As String = "(Server)" Implements IAddressedChannel.MyID
    Public ReadOnly Property MyServiceName As String = "" Implements IAddressedChannel.MyServiceName

    Public ReadOnly Property Stats As New PacketTransportStats Implements IAddressedChannel.Stats

    Public Property DefaultSettings As New BytePacketSettings Implements IPacketStatsAndSettings.DefaultSettings
    Public Property AllowBroadcastSetting As Boolean = False

    Public Sub New()
        AddHandler NewConnection, AddressOf NewConnectionHandler
    End Sub

    Private Sub PacketReceivedHandler(packet As StructuredPacket, transport As IConnectedChannel)
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
                If allow Then
                    response.Add("@RegisterResult", "OK")
                    transport.Info.ID = id
                    transport.Info.ServiceName = service
                Else
                    response.Add("@RegisterResult", "NotAllowed")
                End If
                transport.Channel.SendPacketAsync(response.ToBytePacket)
            Catch ex As Exception
                Dim response As New StructuredPacket(packet)
                response.Add("@RegisterResult", "Error")
                response.Add("@RegisterResultMessage", "Unknown error")
                Try
                    transport.Channel.SendPacketAsync(response.ToBytePacket)
                Catch ex1 As Exception
                End Try
            End Try
        End If

        If packet.Parts.ContainsKey("@GetPeersList") Then
            Try
                Dim service As String = packet.Parts("@GetPeersList")
                Dim response As New StructuredPacket(packet)
                Dim list = GetPeersList(service).ToArray
                response.Add("@PeersList", list)
                transport.Channel.SendPacketAsync(response.ToBytePacket)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub NewConnectionHandler(server As IPacketPortListener, connection As IConnectedChannel)
        connection.Channel.DefaultSettings = DefaultSettings
        connection.Info = New TCPClientInfo
        AddHandler connection.Channel.PacketReceived, Sub(transport As IPacketChannel, packet As BytePacket)
                                                          Try
                                                              Dim sbp As New StructuredPacket(packet)
                                                              PacketReceivedHandler(sbp, connection)
                                                              RaiseEvent PacketReceived(transport, sbp)
                                                          Catch ex As Exception
                                                          End Try
                                                      End Sub
        AddHandler connection.Channel.PacketSent, Sub(transport As IPacketChannel, packet As BytePacket)
                                                      Try
                                                          Dim sbp As New StructuredPacket(packet)
                                                          RaiseEvent PacketSent(transport, sbp)
                                                      Catch ex As Exception
                                                      End Try
                                                  End Sub
    End Sub

    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IAddressedChannel.RegisterMe
        _MyID = id
        _MyServiceName = serviceName
    End Sub

    Public Shadows Sub SendPacket(message As StructuredPacket) Implements IAddressedChannel.SendPacket
        If message.AddressFrom = "" Then message.AddressFrom = MyID
        If message.AddressTo = "" Then
            If AllowBroadcastSetting Then
                Dim bp = message.ToBytePacket
                For Each client In ActiveConnections.ToArray
                    Try
                        client.Channel.SendPacket(bp)
                    Catch ex As Exception
                    End Try
                Next
            Else
                Throw New Exception("SendPacket: Broadcasts not allowed")
            End If
        Else
            For Each client In ActiveConnections.ToArray
                If client.Info.ID.ToLower = message.AddressTo.ToLower Then
                    Dim bp = message.ToBytePacket
                    client.Channel.SendPacket(bp)
                    Return
                End If
            Next
            Throw New Exception("SendPacket: Client with address " + message.AddressTo + " not found")
        End If
    End Sub

    Public Shadows Sub SendPacketAsync(message As StructuredPacket) Implements IAddressedChannel.SendPacketAsync
        If message.AddressTo = "" Then
            If AllowBroadcastSetting Then
                Dim bp = message.ToBytePacket
                For Each client In ActiveConnections.ToArray
                    Try
                        client.Channel.SendPacketAsync(bp)
                    Catch ex As Exception
                    End Try
                Next
            Else
                Throw New Exception("SendPacketAsync: Broadcasts not allowed")
            End If
        Else
            For Each client In ActiveConnections.ToArray
                If client.Info.ID.ToLower = message.AddressTo.ToLower Then
                    Dim bp = message.ToBytePacket
                    client.Channel.SendPacketAsync(bp)
                    Return
                End If
            Next
            Throw New Exception("SendPacketAsync: Client with address " + message.AddressTo + " not found")
        End If
    End Sub

    Public Function GetPeersList(serviceName As String, Optional timeout As Single = 20) As String() Implements IAddressedChannel.GetPeersList
        Dim list As New List(Of String)
        If  serviceName = "" Or serviceName = MyServiceName Then list.Add(MyID)
        For Each client In ActiveConnections.ToArray
            Try
                If client.Info.ServiceName.ToLower = serviceName.ToLower Or serviceName = "" Then
                    If client.Info.ID > "" Then list.Add(client.Info.ID)
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
