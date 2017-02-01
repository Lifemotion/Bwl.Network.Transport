Imports System.Net
Imports Bwl.Network.Transport

Public Class TestForm
    ' Private WithEvents _local_receiver As New UDPTransport
    '  Private _local_sender As New UDPTransport
    '  Private _localresponder_sender As New UDPTransport
    '   Private _farresponder_sender As New UDPTransport
    Private _sentPacket As BytePacket

    Private _localTcpServer As New TCPPortListener
    Private _localTcpClient1 As New TCPChannel
    Private _localTcpClient2 As New TCPChannel
    Private _farTcpClient2 As New TCPChannel

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim pkt As New StructuredPacket
        pkt.Add("Int1", 10)
        pkt.Add("Dbl1", 22.5)
        pkt.Add("String", "CatsCatsCats")
        Dim coded = pkt.ToBytePacket
        Dim decoded As New StructuredPacket(coded)

        '_local_sender.Open("localhost:3055:8055", "")
        ' _local_receiver.Open("localhost:8055:3055", "")

        '  _localresponder_sender.Open("localhost:8066:3066", "")
        ' _farresponder_sender.Open("20.20.25.20:8066:3066", "")
        _localTcpClient1.Open("localhost:8077", "")
        _localTcpClient2.Open("localhost:8099", "ignoreerrors")
        '   _farTcpClient2.Open("20.20.25.20:8099", "")
        _farTcpClient2.Open("20.20.25.10:8077", "")
    End Sub

    Private Function PrepareData() As Byte()
        Dim rnd As New Random
        Dim mb = CSng(InputBox("Megabytes to send",, "1").Replace (".",","))
        Dim buff(1024 * 1024 * mb - 1) As Byte
        For i = 0 To buff.Length - 1
            buff(i) = rnd.Next(0, 255)
        Next
        Return buff
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        _sentPacket = New BytePacket(PrepareData(), New BytePacketSettings With {.AckWaitWindow = 4})
        '    _local_sender.SendPacket(_sentPacket)
    End Sub

    Private Sub t2_ReceivedPacket(packet As BytePacket) 'Handles _local_receiver.ReceivedPacket
        Dim time = Now
        For i = 0 To packet.Bytes.Length - 1
            If _sentPacket.Bytes(i) <> packet.Bytes(i) Then
                Throw New Exception()
            End If
        Next
        MsgBox("Size: " + (_sentPacket.Bytes.Length / 1024 / 1024).ToString("0.000") + " mb, send time: " + (_sentPacket.State.TransmitFinishTime - _sentPacket.State.TransmitStartTime).TotalMilliseconds.ToString + " ms")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        _sentPacket = New BytePacket(PrepareData, New BytePacketSettings With {.AckWaitWindow = 4})
        '  _localresponder_sender.SendPacket(_sentPacket)
        MsgBox("Size: " + (_sentPacket.Bytes.Length / 1024 / 1024).ToString("0.000") + " mb, send time: " + (_sentPacket.State.TransmitFinishTime - _sentPacket.State.TransmitStartTime).TotalMilliseconds.ToString + " ms")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        _sentPacket = New BytePacket(PrepareData, New BytePacketSettings With {.AckWaitWindow = 1, .SendTimeoutMs = 3000, .PartSize = 64000, .RetransmitTimeoutMultiplier = 5})
        Try
            ' _farresponder_sender.SendPacket(_sentPacket)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Dim size = (_sentPacket.Bytes.Length / 1024 / 1024).ToString("0.000")
        Dim time = (_sentPacket.State.TransmitFinishTime - _sentPacket.State.TransmitStartTime).TotalMilliseconds.ToString("0")
        Dim retran = _sentPacket.State.RetransmitCount.ToString
        MsgBox("Size: " + size + " mb, send time: " + time + " ms, " + retran + " retransmits")
    End Sub

    Private Sub TestForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        End
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        _sentPacket = New BytePacket(PrepareData, New BytePacketSettings With {.AckWaitWindow = 1})
        _localTcpClient1.SendPacket(_sentPacket)
        Dim st = (_sentPacket.State.TransmitFinishTime - _sentPacket.State.TransmitStartTime)
        MsgBox("Size: " + (_sentPacket.Bytes.Length / 1024 / 1024).ToString("0.000") + " mb, send time: " + st.TotalMilliseconds.ToString + " ms, speed: " + (_sentPacket.Bytes.Length / 1024 / 1024 * 8 / st.TotalSeconds).ToString("0.00") + " Mbit\s")
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        _sentPacket = New BytePacket(PrepareData, New BytePacketSettings With {.AckWaitWindow = 1})
        _localTcpClient2.SendPacket(_sentPacket)
        Dim st = (_sentPacket.State.TransmitFinishTime - _sentPacket.State.TransmitStartTime)
        MsgBox("Size: " + (_sentPacket.Bytes.Length / 1024 / 1024).ToString("0.000") + " mb, send time: " + st.TotalMilliseconds.ToString + " ms, speed: " + (_sentPacket.Bytes.Length / 1024 / 1024 * 8 / st.TotalSeconds).ToString("0.00") + " Mbit\s")

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        _sentPacket = New BytePacket(PrepareData, New BytePacketSettings With {.AckWaitWindow = 1})
        _farTcpClient2.SendPacket(_sentPacket)
        Dim st = (_sentPacket.State.TransmitFinishTime - _sentPacket.State.TransmitStartTime)
        MsgBox("Size: " + (_sentPacket.Bytes.Length / 1024 / 1024).ToString("0.000") + " mb, send time: " + st.TotalMilliseconds.ToString + " ms, speed: " + (_sentPacket.Bytes.Length / 1024 / 1024 * 8 / st.TotalSeconds).ToString("0.00") + " Mbit\s")
        MsgBox("Ping: " + _farTcpClient2.Ping(1000).ToString)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim pkt As New StructuredPacket
        pkt.Add("Int1", 10)
        pkt.Add("Dbl1", 22.5)
        pkt.Add("String", "CatsCatsCats")
        pkt.Add("Bytes", PrepareData)
        Dim codeStart = Now
        _sentPacket = pkt.ToBytePacket(New BytePacketSettings With {.AckWaitWindow = 1})
        Dim codeEnd = Now
        _localTcpClient1.SendPacket(_sentPacket)
        Dim cd = (codeEnd - codeStart)
        Dim st = (_sentPacket.State.TransmitFinishTime - _sentPacket.State.TransmitStartTime)

        MsgBox("Size: " + (_sentPacket.Bytes.Length / 1024 / 1024).ToString("0.000") + " mb, send time: " + st.TotalMilliseconds.ToString + " ms, speed: " + (_sentPacket.Bytes.Length / 1024 / 1024 * 8 / st.TotalSeconds).ToString("0.00") + " Mbit\s, code time: " + cd.TotalMilliseconds.ToString + " ms")
    End Sub
End Class
