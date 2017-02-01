Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class TcpAddressedTransportTests
    <TestMethod()> Public Sub TAT_ConnectionsAddressed()
        Dim server As New TCPAddressedServer()
        Dim client1 As New TCPAddressedTransport()
        Dim client2 As New TCPAddressedTransport()
        Dim connectedList As New List(Of IConnectedClient)
        AddHandler server.NewConnection, Sub(s As IPortListener, t As IConnectedClient)
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
        If connectedList.FindAll(Function(t As IConnectedClient) t.Transport.ID = client1.ID).Count = 1 Then Throw New Exception("Client 1 not received in server.NewConnection")
        If connectedList.FindAll(Function(t As IConnectedClient) t.Transport.ID = client2.ID).Count = 1 Then Throw New Exception("Client 2 not received in server.NewConnection")
        If server.ActiveConnections.Count(Function(t As IConnectedClient) t.Transport.ID = client1.ID) = 1 Then Throw New Exception("Client 1 not received in server.NewConnection")
        If server.ActiveConnections.Count(Function(t As IConnectedClient) t.Transport.ID = client2.ID) = 1 Then Throw New Exception("Client 2 not received in server.NewConnection")
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
End Class
