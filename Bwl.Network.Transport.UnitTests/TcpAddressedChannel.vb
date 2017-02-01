Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class TcpAddressedTransportTests
    <TestMethod()> Public Sub TAT_ConnectionsAddressed()
        Dim server As New TCPAddressedServer()
        Dim client1 As New TCPAddressedChannel()
        Dim client2 As New TCPAddressedChannel()
        Dim connectedList As New List(Of IConnectedChannel)
        AddHandler server.NewConnection, Sub(s As IPacketPortListener, t As IConnectedChannel)
                                             connectedList.Add(t)
                                         End Sub
        AddHandler server.RegisterClientRequest, Sub(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String)
                                                     allowRegister = True
                                                 End Sub
        server.Open("localhost:3043", "")
        client1.Open("localhost:3043", "")
        client2.Open("localhost:3043", "")
        Threading.Thread.Sleep(100)
        client1.RegisterMe("Client 1", "", "Service 1", "")
        client2.RegisterMe("Client 2", "", "Service 2", "")
        If connectedList.FindAll(Function(t As IConnectedChannel) t.Channel.ID = client1.ID).Count = 1 Then Throw New Exception("Client 1 not received in server.NewConnection")
        If connectedList.FindAll(Function(t As IConnectedChannel) t.Channel.ID = client2.ID).Count = 1 Then Throw New Exception("Client 2 not received in server.NewConnection")
        If server.ActiveConnections.Count(Function(t As IConnectedChannel) t.Channel.ID = client1.ID) = 1 Then Throw New Exception("Client 1 not received in server.NewConnection")
        If server.ActiveConnections.Count(Function(t As IConnectedChannel) t.Channel.ID = client2.ID) = 1 Then Throw New Exception("Client 2 not received in server.NewConnection")
        Dim servPeers = server.GetPeersList("")
        Dim clientPeers1 = client1.GetPeersList("")
        Dim clientPeers2 = client1.GetPeersList("Service 1")
        CompareArrays(Of String)(servPeers, {"(Server)", "Client 1", "Client 2"})
        CompareArrays(Of String)(clientPeers1, {"(Server)", "Client 1", "Client 2"})
        CompareArrays(Of String)(clientPeers2, {"Client 1"})
        client1.Dispose()
        client2.Dispose()
        server.Dispose()
    End Sub

    <TestMethod()> Public Sub TAT_AddressedSend()
        Dim server As New TCPAddressedServer()
        Dim client1 As New TCPAddressedChannel()
        Dim client2 As New TCPAddressedChannel()
        AddHandler server.RegisterClientRequest, Sub(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String)
                                                     allowRegister = True
                                                 End Sub
        Dim serverReceived As StructuredPacket = Nothing
        AddHandler server.PacketReceived, Sub(trn As IAddressedChannel, msg As StructuredPacket)
                                              serverReceived = msg
                                          End Sub
        Dim client1Received As StructuredPacket = Nothing
        AddHandler client1.PacketReceived, Sub(trn As IAddressedChannel, msg As StructuredPacket)
                                               client1Received = msg
                                           End Sub
        Dim client2Received As StructuredPacket = Nothing
        AddHandler client2.PacketReceived, Sub(trn As IAddressedChannel, msg As StructuredPacket)
                                               client2Received = msg
                                           End Sub
        server.Open("localhost:3044", "")
        client1.Open("localhost:3044", "")
        client2.Open("localhost:3044", "")
        Threading.Thread.Sleep(100)
        client1.RegisterMe("Client 1", "", "Service 1", "")
        client2.RegisterMe("Client 2", "", "Service 2", "")

        Dim pktTo1 As New StructuredPacket
        pktTo1.Add("Key1", "Cat")
        pktTo1.AddressTo = "Client 1"
        server.SendPacket(pktTo1)
        Dim pktToServer As New StructuredPacket
        pktToServer.Add("Key1", "Cat")
        client2.SendPacket(pktToServer)
        Threading.Thread.Sleep(100)

        Assert.AreEqual("Client 1", pktTo1.AddressTo)
        Assert.AreEqual("Client 1", client1Received.AddressTo)
        Assert.AreEqual("(Server)", pktTo1.AddressFrom)
        Assert.AreEqual("(Server)", client1Received.AddressFrom)

        Assert.AreEqual("Client 2", pktToServer.AddressFrom)
        Assert.AreEqual("Client 2", serverReceived.AddressFrom)
        Assert.AreEqual("", pktToServer.AddressTo)
        Assert.AreEqual("", serverReceived.AddressTo)

        Dim pktBroadcast As New StructuredPacket
        pktBroadcast.Add("Key1", "Cat")
        TestException(Sub()
                          server.SendPacket(pktBroadcast)
                      End Sub, "Broadcasts must be disabled by default")
        server.AllowBroadcastSetting = True
        server.SendPacket(pktBroadcast)
        Threading.Thread.Sleep(100)

        Assert.AreEqual("", pktBroadcast.AddressTo)
        Assert.AreEqual("", client1Received.AddressTo)
        Assert.AreEqual("", client2Received.AddressTo)
        Assert.AreEqual("(Server)", pktBroadcast.AddressFrom)
        Assert.AreEqual("(Server)", client1Received.AddressFrom)
        Assert.AreEqual("(Server)", client2Received.AddressFrom)

        client1.Dispose()
        client2.Dispose()
        server.Dispose()

    End Sub
End Class
