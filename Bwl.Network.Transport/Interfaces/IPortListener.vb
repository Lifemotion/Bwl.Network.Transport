Public Interface IPortListener
    Inherits IConnectionControl

    Event NewConnection(server As IPortListener, connection As IConnectedClient)
    ReadOnly Property ActiveConnections As IConnectedClient()

End Interface
