Imports Bwl.Network.Transport

Public Class TCPServer
    Inherits TCPPortListener
    Implements IPacketServer, IConnectionControl

    Public Event ServerReceivedPacket(connection As IPacketChannel, packet As BytePacket) Implements IPacketServer.ServerReceivedPacket
    Public Event ServerSentPacket(connection As IPacketChannel, packet As BytePacket) Implements IPacketServer.ServerSentPacket

    Public Sub New()
        AddHandler Me.NewConnection, AddressOf NewConnectionHandler
    End Sub

    Private Sub NewConnectionHandler(server As IPacketPortListener, connection As IPacketChannel)
        AddHandler connection.PacketReceived, Sub(transport As IPacketChannel, packet As BytePacket)
                                                  RaiseEvent ServerReceivedPacket(connection, packet)
                                              End Sub
        AddHandler connection.PacketSent, Sub(transport As IPacketChannel, packet As BytePacket)
                                              RaiseEvent ServerSentPacket(connection, packet)
                                          End Sub
    End Sub
End Class
