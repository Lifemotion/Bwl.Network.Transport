Imports Bwl.Network.Transport

Public Class TCPAddressedServer
    Inherits TCPServer
    Implements IAddressedTransport

    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IAddressedTransport.RegisterClientRequest
    Public Event PacketReceived(source As IAddressedTransport, packet As StructuredPacket) Implements IAddressedTransport.PacketReceived
    Public Event PacketSent(source As IAddressedTransport, packet As StructuredPacket) Implements IAddressedTransport.PacketSent

    Public ReadOnly Property MyID As String = "" Implements IAddressedTransport.MyID
    Public ReadOnly Property MyServiceName As String = "" Implements IAddressedTransport.MyServiceName

    Public Sub New()
        AddHandler MyBase.NewConnection, AddressOf NewConnectionHandler
    End Sub

    Private Sub NewConnectionHandler(server As IPortListener, connection As IConnectionInfo)
        AddHandler connection.Transport.PacketReceived, Sub(packet As BytePacket)
                                                            ' RaiseEvent PacketReceived(connection, packet)
                                                        End Sub
        AddHandler connection.Transport.PacketSent, Sub(packet As BytePacket)
                                                        '  RaiseEvent SentPacket(connection, packet)
                                                    End Sub
    End Sub

    Private Sub BytePacketReceived(packet As BytePacket)
        Try
            Dim sbp As New StructuredPacket(packet)
            RaiseEvent PacketReceived(Me, sbp)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IAddressedTransport.RegisterMe
        _MyID = id
        _MyServiceName = serviceName
    End Sub

    Public Shadows Sub SendPacket(message As StructuredPacket) Implements IAddressedTransport.SendPacket
        Throw New NotImplementedException()
    End Sub

    Public Shadows Sub SendPacketAsync(message As StructuredPacket) Implements IAddressedTransport.SendPacketAsync
        Throw New NotImplementedException()
    End Sub

    Public Function GetPeersList(serviceName As String, Optional timeout As Single = 20) As String() Implements IAddressedTransport.GetPeersList
        Throw New NotImplementedException()
    End Function

    Public Function SendPacketWaitAnswer(message As StructuredPacket, Optional timeout As Single = 20) As StructuredPacket Implements IAddressedTransport.SendPacketWaitAnswer
        Throw New NotImplementedException()
    End Function

    Public Function WaitPacket(Optional timeout As Single = 20, Optional pktid As Integer = -1, Optional partKey As String = "") As Object Implements IAddressedTransport.WaitPacket
        Throw New NotImplementedException()
    End Function
End Class
