﻿Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class TcpTransportTests

    <TestMethod()> Public Sub TT_Connections()
        Dim server As New TCPServer(3040)
        Dim client1 As New TCPTransport()
        Dim client2 As New TCPTransport()
        Dim connectedList As New List(Of IPacketTransport)
        AddHandler server.NewConnection, Sub(s As IPortListener, t As IPacketTransport)
                                             connectedList.Add(t)
                                         End Sub
        client1.Open("localhost:3040", "")
        client2.Open("localhost:3040", "")
        Threading.Thread.Sleep(100)
        If connectedList.FindAll(Function(t As IPacketTransport) t.ID = client1.ID).Count = 1 Then Throw New Exception("Client 1 not received in server.NewConnection")
        If connectedList.FindAll(Function(t As IPacketTransport) t.ID = client2.ID).Count = 1 Then Throw New Exception("Client 2 not received in server.NewConnection")
        If server.ActiveConnections.Count(Function(t As IPacketTransport) t.ID = client1.ID) = 1 Then Throw New Exception("Client 1 not received in server.NewConnection")
        If server.ActiveConnections.Count(Function(t As IPacketTransport) t.ID = client2.ID) = 1 Then Throw New Exception("Client 2 not received in server.NewConnection")
        client1.Dispose()
        client2.Dispose()
        server.Dispose()
    End Sub

    <TestMethod()> Public Sub TT_SendReceiveAsync()
        Dim server As New TCPServer(3042)
        Dim client1 As New TCPTransport()
        Dim serverReceived As BytePacket = Nothing
        Dim serverSent As BytePacket = Nothing
        AddHandler server.ReceivedPacket, Sub(t As IPacketTransport, p As BytePacket)
                                              serverReceived = p
                                          End Sub
        AddHandler server.SentPacket, Sub(t As IPacketTransport, p As BytePacket)
                                          serverSent = p
                                      End Sub
        client1.Open("localhost:3042", "")
        Dim bp1 As New BytePacket(PrepareData(0.2))
        client1.SendPacketAsync(bp1)
        Threading.Thread.Sleep(200)
        Assert.AreEqual(True, bp1.State.TransmitStarted)
        Assert.AreEqual(True, bp1.State.TransmitComplete)
        Assert.AreEqual(CSng(1.0), bp1.State.TransmitProgress)

        Assert.IsNotNull(serverReceived)
        Assert.AreEqual(True, serverReceived.State.TransmitStarted)
        Assert.AreEqual(True, serverReceived.State.TransmitComplete)
        Assert.AreEqual(CSng(1.0), serverReceived.State.TransmitProgress)
        CompareArrays(Of Byte)(bp1.Bytes, serverReceived.Bytes)

        client1.Dispose()
        server.Dispose()
    End Sub

    <TestMethod()> Public Sub TT_SendReceive()
        Dim server As New TCPServer(3041)
        Dim client1 As New TCPTransport()
        Dim client1Received As BytePacket = Nothing
        Dim client1Sent As BytePacket = Nothing
        Dim serverReceived As BytePacket = Nothing
        Dim serverSent As BytePacket = Nothing
        AddHandler server.ReceivedPacket, Sub(t As IPacketTransport, p As BytePacket)
                                              serverReceived = p
                                          End Sub
        AddHandler server.SentPacket, Sub(t As IPacketTransport, p As BytePacket)
                                          serverSent = p
                                      End Sub
        AddHandler client1.ReceivedPacket, Sub(p As BytePacket)
                                               client1Received = p
                                           End Sub
        AddHandler client1.SentPacket, Sub(p As BytePacket)
                                           client1Sent = p
                                       End Sub
        client1.Open("localhost:3041", "")
        Dim bp1 As New BytePacket(PrepareData(0.2))
        Assert.AreEqual(False, bp1.State.TransmitStarted)
        Assert.AreEqual(False, bp1.State.TransmitComplete)
        Assert.AreEqual(CSng(0.0), bp1.State.TransmitProgress)
        client1.SendPacket(bp1)
        Assert.AreEqual(True, bp1.State.TransmitStarted)
        Assert.AreEqual(True, bp1.State.TransmitComplete)
        Assert.AreEqual(CSng(1.0), bp1.State.TransmitProgress)

        Threading.Thread.Sleep(200)
        Assert.IsNotNull(serverReceived)
        Assert.AreEqual(True, serverReceived.State.TransmitStarted)
        Assert.AreEqual(True, serverReceived.State.TransmitComplete)
        Assert.AreEqual(CSng(1.0), serverReceived.State.TransmitProgress)
        CompareArrays(Of Byte)(bp1.Bytes, serverReceived.Bytes)

        Dim bp2 As New BytePacket(PrepareData(0.2))
        server.ActiveConnections(0).SendPacket(bp2)
        Assert.AreEqual(True, bp2.State.TransmitStarted)
        Assert.AreEqual(True, bp2.State.TransmitComplete)
        Assert.AreEqual(CSng(1.0), bp2.State.TransmitProgress)

        Threading.Thread.Sleep(200)
        Assert.IsNotNull(client1Received)
        Assert.AreEqual(True, client1Received.State.TransmitStarted)
        Assert.AreEqual(True, client1Received.State.TransmitComplete)
        Assert.AreEqual(CSng(1.0), client1Received.State.TransmitProgress)
        CompareArrays(Of Byte)(bp2.Bytes, client1Received.Bytes)

        client1.Dispose()
        server.Dispose()
    End Sub
End Class