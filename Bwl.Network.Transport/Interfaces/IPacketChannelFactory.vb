Public Interface IPacketChannelFactory
    ReadOnly Property TransportClass As Type
    Function Create() As IPacketChannel
End Interface
