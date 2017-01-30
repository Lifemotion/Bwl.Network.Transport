Imports Bwl.Network.Transport

Public Class TCPAddressedServer
    Inherits TCPTransport
    Implements IAddressedTransport
    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IAddressedTransport.RegisterClientRequest
    Public Shadows Event PacketReceived(source As IAddressedTransport, packet As StructuredPacket) Implements IAddressedTransport.PacketReceived
    Public Shadows Event PacketSent(source As IAddressedTransport, packet As StructuredPacket) Implements IAddressedTransport.PacketSent

    Public ReadOnly Property MyID As String Implements IAddressedTransport.MyID
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property MyServiceName As String Implements IAddressedTransport.MyServiceName
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IAddressedTransport.RegisterMe
        Throw New NotImplementedException()
    End Sub

    Public Sub SendPacket(message As StructuredPacket) Implements IAddressedTransport.SendPacket
        Throw New NotImplementedException()
    End Sub

    Public Sub SendPacketAsync(message As StructuredPacket) Implements IAddressedTransport.SendPacketAsync
        Throw New NotImplementedException()
    End Sub

    Public Function GetPeersList(serviceName As String, Optional timeout As Single = 20) As String() Implements IAddressedTransport.GetPeersList
        Throw New NotImplementedException()
    End Function

    Public Function SendPacketWaitAnswer(message As StructuredPacket, Optional timeout As Single = 20) As StructuredPacket Implements IAddressedTransport.SendPacketWaitAnswer
        Throw New NotImplementedException()
    End Function

    Public Function WaitPacket(Optional timeout As Single = 20, Optional pktid As Integer = -1, Optional partKey As String = "") As Object Implements IAddressedTransport.WaitPacket
        Throw New NotImplementedException()
    End Function
End Class
