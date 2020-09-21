Public Interface IAddressedServer
    Inherits IAddressedChannel
    ReadOnly Property Server As IPacketServer
    Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String)
    Event NewConnection(server As IAddressedServer, connection As IAddressedChannel)
    ReadOnly Property ActiveConnections As IAddressedChannel()
End Interface
