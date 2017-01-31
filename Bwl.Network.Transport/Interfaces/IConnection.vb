Public Interface IConnection
    ReadOnly Property ID As Long
    Sub Open(address As String, options As String)
    Sub Close()
    ReadOnly Property Stats As PacketTransportStats
    ReadOnly Property IsConnected As Boolean

End Interface
