Public Interface IAddressedChannelFactory
    ReadOnly Property ChannelClass As Type
    Function Create() As IAddressedChannel
End Interface
