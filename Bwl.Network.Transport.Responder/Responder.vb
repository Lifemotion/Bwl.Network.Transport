Imports Bwl.Network.Transport

Module Responder
    Dim t1 As New UDPTransport
    Dim WithEvents t2 As New TCPServer(8099)

    Sub Main()
        t1.Open("20.20.25.80:3066:8066", "")
        Do
            Threading.Thread.Sleep(1000)
            Console.Clear()
            Console.WriteLine("BytesReceived        : " + t1.Stats.BytesReceived.ToString("0 000 000"))
            Console.WriteLine("BytesSent            : " + t1.Stats.BytesSent.ToString("0 000 000"))
            Console.WriteLine("PacketsReceived      : " + t1.Stats.PacketsReceived.ToString("0 000"))
            Console.WriteLine("PacketsReceiveFailed : " + t1.Stats.PacketsReceiveFailed.ToString("0 000"))
            Console.WriteLine("PacketsReceiving     : " + t1.Stats.PacketsReceiving.ToString("0"))
            Console.WriteLine("PacketsSent          : " + t1.Stats.PacketsSent.ToString("0 000"))
            Console.WriteLine("PacketsSendFailed    : " + t1.Stats.PacketsSendFailed.ToString("0 000"))
            Console.WriteLine("PacketsSending       : " + t1.Stats.PacketsSending.ToString("0"))
            Console.WriteLine("Retransmits          : " + t1.Stats.Retransmits.ToString("0 000"))
            Console.WriteLine("")
            Console.WriteLine("TcpClients           : " + t2.ActiveConnections.Length.ToString)
        Loop
    End Sub

    Private Sub t2_NewConnection(server As TCPPortListener, transport As TCPTransport) Handles t2.NewConnection
        Console.WriteLine("NewConnection")
        AddHandler transport.ReceivedPacket, Sub(packet As BytePacket)
                                                 Dim b = 1
                                             End Sub
    End Sub
End Module
