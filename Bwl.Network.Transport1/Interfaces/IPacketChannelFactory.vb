Imports System.Net.Sockets

Public Interface IPacketChannelFactory
    ReadOnly Property TransportClass As Type
    Function Create() As IPacketChannel
    Function Create(socket As Socket, parameters As TCPChannel.TCPTransportParameters) As IPacketChannel
End Interface
