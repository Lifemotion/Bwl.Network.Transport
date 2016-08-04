Imports System.Net
Imports Bwl.Network.Transport

Public Class TestForm
    Private WithEvents _local_receiver As New UDPTransport
    Private _local_sender As New UDPTransport
    '  Private _localresponder_sender As New UDPTransport
    Private _farresponder_sender As New UDPTransport
    Private _sentPacket As BytePacket
    Private _start As DateTime
    Private _end As DateTime

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _local_sender.Open("localhost:3055:8055", "")
        _local_receiver.Open("localhost:8055:3055", "")

        '  _localresponder_sender.Open("localhost:8066:3066", "")
        _farresponder_sender.Open("20.20.25.20:8066:3066", "")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim rnd As New Random
        Dim mb = CInt(InputBox("megabytes",, "1"))
        Dim buff(1024 * 1024 * mb - 1) As Byte
        For i = 0 To buff.Length - 1
            buff(i) = rnd.Next(0, 255)
        Next
        _sentPacket = New BytePacket(buff, New BytePacketSettings With {.AckWaitWindow = 4})
        _start = Now
        _local_sender.SendPacket(_sentPacket)
        _end = Now
    End Sub

    Private Sub t2_ReceivedPacket(packet As BytePacket) Handles _local_receiver.ReceivedPacket
        Dim time = Now
        For i = 0 To packet.Bytes.Length - 1
            If _sentPacket.Bytes(i) <> packet.Bytes(i) Then
                Throw New Exception()
            End If
        Next
        MsgBox("Size: " + (packet.Bytes.Length / 1024 / 1024).ToString("0.000") + " mb, send time: " + (_end - _start).TotalMilliseconds.ToString + " ms, send-recv time: " + (time - _start).TotalMilliseconds.ToString + " ms")
    End Sub

    Private Sub TestForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        End
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim rnd As New Random
        Dim mb = CInt(InputBox("megabytes",, "1"))
        Dim buff(1024 * 1024 * mb - 1) As Byte

        For i = 0 To buff.Length - 1
            buff(i) = rnd.Next(0, 255)
        Next
        _sentPacket = New BytePacket(buff, New BytePacketSettings With {.AckWaitWindow = 4})
        _start = Now
        '  _localresponder_sender.SendPacket(_sentPacket)
        _end = Now
        MsgBox("Size: " + (_sentPacket.Bytes.Length / 1024 / 1024).ToString("0.000") + " mb, send time: " + (_end - _start).TotalMilliseconds.ToString + " ms")

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim rnd As New Random
        Dim mb = CInt(InputBox("kbytes",, "1"))
        Dim buff(1024 * mb - 1) As Byte
        '  Dim buff(102) As Byte
        For i = 0 To buff.Length - 1
            buff(i) = rnd.Next(0, 255)
        Next
        _sentPacket = New BytePacket(buff, New BytePacketSettings With {.AckWaitWindow = 1, .SendTimeoutMs = 3000, .PartSize = 1400})
        _start = Now
        Try
            _farresponder_sender.SendPacket(_sentPacket)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        _end = Now
        MsgBox("Size: " + (_sentPacket.Bytes.Length / 1024 / 1024).ToString("0.000") + " mb, send time: " + (_end - _start).TotalMilliseconds.ToString + " ms")

    End Sub
End Class
