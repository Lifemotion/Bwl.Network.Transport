Public Interface IPacketTransport
    ReadOnly Property ID As Long
    Property ServiceName As String
    Sub Open(address As String, options As String)
    Sub Close()
    Event ReceivedPacket(packet As BytePacket)
    Sub SendPacket(packet As BytePacket)
    Sub SendPacketAsync(packet As BytePacket)
    Function SendPacketWaitAnswer(packet As BytePacket, Optional timeout As Single = 500) As BytePacket
    Function Ping(maximumTimeoutMs As Integer) As Integer
    ReadOnly Property Stats As PacketTransportStats
    Property DefaultSettings As BytePacketSettings
    ReadOnly Property IsConnected() As Boolean
    Event SentPacket(packet As BytePacket)
End Interface
