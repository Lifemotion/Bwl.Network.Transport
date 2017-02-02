Public Interface IPacketPortListener
    Inherits IConnectionControl

    Event NewConnection(server As IPacketPortListener, connection As IConnectedChannel)
    ReadOnly Property ActiveConnections As IConnectedChannel()

End Interface
