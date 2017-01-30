Public Interface IAddressedTransport
    Inherits IConnection

    ReadOnly Property MyID As String
    ReadOnly Property MyServiceName As String
    Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String)
    Function SendPacketWaitAnswer(message As StructuredPacket, Optional timeout As Single = 20) As StructuredPacket
    Sub SendPacket(message As StructuredPacket)
    Sub SendPacketAsync(message As StructuredPacket)
    Function WaitPacket(Optional timeout As Single = 20, Optional pktid As Integer = -1, Optional partKey As String = "")
    Sub RegisterMe(id As String, password As String, serviceName As String, options As String)
    Function GetPeersList(serviceName As String, Optional timeout As Single = 20) As String()
    Event PacketSent(source As IAddressedTransport, packet As StructuredPacket)
    Event PacketReceived(source As IAddressedTransport, packet As StructuredPacket)
End Interface
