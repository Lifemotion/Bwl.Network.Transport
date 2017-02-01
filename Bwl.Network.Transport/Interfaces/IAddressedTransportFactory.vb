Public Interface IAddressedTransportFactory
    ReadOnly Property TransportClass As Type
    Function Create() As IAddressedTransport
End Interface
