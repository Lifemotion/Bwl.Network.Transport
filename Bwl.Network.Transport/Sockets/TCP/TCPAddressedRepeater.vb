Imports Bwl.Network.Transport

Public Class TCPAddressedRepeater
    Inherits TCPAddressedServer

    Private Sub TCPAddressedRepeater_PacketReceived(channel As IPacketChannel, packet As StructuredPacket) Handles Me.PacketReceived
        Me.SendPacketAsync(packet)
    End Sub
End Class
