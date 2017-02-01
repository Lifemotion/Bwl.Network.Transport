Imports Bwl.Network.Transport

Public Class TCPServer
    Inherits TCPPortListener
    Implements IPacketTransportServer, IConnectionControl

    Public Event ReceivedPacket(connection As IConnectedClient, packet As BytePacket) Implements IPacketTransportServer.ReceivedPacket
    Public Event SentPacket(connection As IConnectedClient, packet As BytePacket) Implements IPacketTransportServer.SentPacket

    Public Sub New()
        AddHandler Me.NewConnection, AddressOf NewConnectionHandler
    End Sub

    Private Sub NewConnectionHandler(server As IPortListener, connection As IConnectedClient)
        AddHandler connection.Transport.PacketReceived, Sub(transport As IPacketTransport, packet As BytePacket)
                                                            RaiseEvent ReceivedPacket(connection, packet)
                                                        End Sub
        AddHandler connection.Transport.PacketSent, Sub(transport As IPacketTransport, packet As BytePacket)
                                                        RaiseEvent SentPacket(connection, packet)
                                                    End Sub
    End Sub
End Class
