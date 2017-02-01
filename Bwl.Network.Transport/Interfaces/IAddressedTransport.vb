Public Interface IAddressedTransport
    Inherits IConnectionControl, IStatsAndSettings

    ReadOnly Property MyID As String
    ReadOnly Property MyServiceName As String
    Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String)
    Sub RegisterMe(id As String, password As String, serviceName As String, options As String)

    Sub SendPacket(message As StructuredPacket)
    Sub SendPacketAsync(message As StructuredPacket)
    Function SendPacketWaitAnswer(message As StructuredPacket, Optional timeout As Single = 20) As StructuredPacket
    Function WaitPacket(Optional timeout As Single = 20, Optional pktid As Integer = -1, Optional partKey As String = "")

    Event PacketSent(transport As IPacketTransport, packet As StructuredPacket)
    Event PacketReceived(transport As IPacketTransport, packet As StructuredPacket)

    Function GetPeersList(serviceName As String, Optional timeout As Single = 20) As String()
End Interface
