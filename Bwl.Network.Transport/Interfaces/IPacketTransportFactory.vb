Public Interface IPacketTransportFactory
    ReadOnly Property TransportClass As Type
    Function Create() As IPacketTransport
End Interface
