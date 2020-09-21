Public Interface IAddressedClient
    Inherits IAddressedChannel
    ReadOnly Property Channel As IPacketChannel
End Interface
