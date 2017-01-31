Imports Bwl.Network.Transport

Public Interface IPortListener
    Event NewConnection(server As IPortListener, connection As IConnectionInfo)
    ReadOnly Property ActiveConnections As IConnectionInfo()

End Interface
