Public Class BytePacket
    Public Property Bytes As Byte()
    Public Property Settings As New BytePacketSettings
    Public ReadOnly Property State As New BytePacketState

    Public Sub New()

    End Sub

    Public Sub New(bytes As Byte())
        _Bytes = bytes
    End Sub

    Public Sub New(bytes As Byte(), settings As BytePacketSettings)
        _Bytes = bytes
        _Settings = settings
    End Sub

    Public Sub New(packet As BytePacket)
        _Bytes = packet.Bytes
        _Settings = packet.Settings
        _State = packet.State
    End Sub
End Class
