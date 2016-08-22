Imports Bwl.Network.Transport

Public Interface IPortListener
    Event NewConnection(server As IPortListener, transport As IPacketTransport)
End Interface
