Public Interface IPacketPortListener
    Inherits IConnectionControl

    Event NewConnection(server As IPacketPortListener, connection As IPacketChannel)
    ReadOnly Property ActiveConnections As IPacketChannel()

End Interface
