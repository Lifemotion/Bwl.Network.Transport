Imports Bwl.Network.Transport

Public Class TCPServer
    Inherits TCPPortListener
    Implements IPacketTransportServer, IConnection

    Public ReadOnly Property ID As Long Implements IConnection.ID
    Public ReadOnly Property Stats As New PacketTransportStats Implements IConnection.Stats
    Public ReadOnly Property IsConnected As Boolean Implements IConnection.IsConnected

    Public Event ReceivedPacket(connection As IConnectionInfo, packet As BytePacket) Implements IPacketTransportServer.ReceivedPacket
    Public Event SentPacket(connection As IConnectionInfo, packet As BytePacket) Implements IPacketTransportServer.SentPacket

    Public Sub New()
        AddHandler Me.NewConnection, AddressOf NewConnectionHandler
    End Sub

    Private Sub NewConnectionHandler(server As IPortListener, connection As IConnectionInfo)
        AddHandler connection.Transport.PacketReceived, Sub(packet As BytePacket)
                                                            RaiseEvent ReceivedPacket(connection, packet)
                                                        End Sub
        AddHandler connection.Transport.PacketSent, Sub(packet As BytePacket)
                                                        RaiseEvent SentPacket(connection, packet)
                                                    End Sub
    End Sub

    Public Sub Open(address As String, options As String) Implements IConnection.Open
        MyBase.Start(CInt(address))
    End Sub

    Public Shadows Sub Close() Implements IConnection.Close
        MyBase.Close()
    End Sub
End Class
