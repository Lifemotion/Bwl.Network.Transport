Public Class BytePacketSettings
    Public Property PartSize As Integer = 65000
    Public Property AckWaitWindow As Integer = 1
    Public Property SendTimeoutMs As Integer = 30000
    Public Property MaxAllowedRetransmits As Integer = 2
    Public Property RetransmitTimeoutMultiplier As Integer = 5
End Class
