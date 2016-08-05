Imports System.Net
Imports Bwl.Network.Transport

Public Class TestForm
    Private WithEvents _local_receiver As New UDPTransport
    Private _local_sender As New UDPTransport
    '  Private _localresponder_sender As New UDPTransport
    Private _farresponder_sender As New UDPTransport
    Private _sentPacket As BytePacket

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _local_sender.Open("localhost:3055:8055", "")
        _local_receiver.Open("localhost:8055:3055", "")

        '  _localresponder_sender.Open("localhost:8066:3066", "")
        _farresponder_sender.Open("20.20.25.20:8066:3066", "")
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
        _local_sender.SendPacket(_sentPacket)
    End Sub

    Private Sub t2_ReceivedPacket(packet As BytePacket) Handles _local_receiver.ReceivedPacket
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
            _farresponder_sender.SendPacket(_sentPacket)
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
End Class
