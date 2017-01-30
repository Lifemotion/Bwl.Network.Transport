Public Interface IConnection
    ReadOnly Property ID As Long
    Sub Open(address As String, options As String)
    Sub Close()
    ReadOnly Property Stats As PacketTransportStats
    ReadOnly Property IsConnected As Boolean
    Function Ping(maximumTimeoutMs As Integer) As Integer

End Interface
