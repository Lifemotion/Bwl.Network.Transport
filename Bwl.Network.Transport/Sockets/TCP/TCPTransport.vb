Imports System.Net.Sockets
Imports System.Net
Imports Bwl.Network.Transport

''' <summary>
''' UDP-base Packet Transport with big packets support, sync or async sending.
''' If SendPacket completed (no exception), it means packet fully sent and fully received by other side.
''' </summary>
Public Class TCPTransport
    Implements IPacketTransport, IDisposable
    Public Event ReceivedPacket(packet As BytePacket) Implements IPacketTransport.ReceivedPacket
    Public Event SentPacket(packet As BytePacket) Implements IPacketTransport.SentPacket
    Public ReadOnly Property AverageTransmitTime As Integer
    Public Property DefaultSettings As New BytePacketSettings Implements IPacketTransport.DefaultSettings
    Public ReadOnly Property Stats As New PacketTransportStats Implements IPacketTransport.Stats

    Private _socket As Socket
    Private _receiveThread As New Threading.Thread(AddressOf ReceiveThread)
    Private _receiveBodyBuffer(65535) As Byte
    Private _receiveHeadBuffer(35) As Byte

    Private _ackBuffer(35) As Byte
    Private _sendBuffer(65535 + 35) As Byte
    Private _rnd As New Random

    Private _cleanThread As New Threading.Thread(AddressOf CleanThread)

    Private _receivingPackets As New List(Of SocketBytePacket)
    Private _sendingPackets As New List(Of SocketBytePacket)
    Private _receiveThreadDelay As TimeSpan = TimeSpan.FromTicks(1)
    Private _sendPacketDelay As TimeSpan = TimeSpan.FromTicks(1)

    Public Sub New()
        Me.New(Nothing)
    End Sub

    Public Sub New(socket As Socket)
        _socket = socket
        _receiveThread.Name = "UDPTransport_ReceiveThread"
        _receiveThread.Start()

        _cleanThread.Name = "UDPTransport_CleanThread"
        _cleanThread.IsBackground = True
        _cleanThread.Start()
    End Sub

    Private Function GetReceivingPacket(id As ULong, partCount As Integer, totalBytes As Integer) As SocketBytePacket
        SyncLock _receivingPackets
            For Each pkt In _receivingPackets
                If pkt.PacketID = id Then Return pkt
            Next
            Dim newPkt As New SocketBytePacket(id, partCount, totalBytes)
            newPkt.State.TransmitStarted = True
            newPkt.State.TransmitStartTime = Now
            newPkt.Settings = DefaultSettings
            _receivingPackets.Add(newPkt)
            Return newPkt
        End SyncLock
    End Function

    Private Sub CleanThread()
        Do
            Try
                Dim removePacket As SocketBytePacket = Nothing
                SyncLock _receivingPackets
                    Stats.PacketsReceiving = _receivingPackets.Count
                    Stats.PacketsSending = _sendingPackets.Count

                    For Each pkt In _receivingPackets
                        If pkt.State.TransmitComplete Then
                            '  removePacket = pkt
                        ElseIf (Now - pkt.State.TransmitStartTime).TotalMilliseconds > pkt.Settings.SendTimeoutMs Then
                            removePacket = pkt
                        End If
                    Next
                    If removePacket IsNot Nothing Then
                        Stats.PacketsReceiveFailed += 1
                        _receivingPackets.Remove(removePacket)
                    End If
                End SyncLock
            Catch ex As Exception

            End Try
            Threading.Thread.Sleep(10)
        Loop
    End Sub

    Private Sub ReceiveThread()
        Dim receivingBody As Boolean
        Dim receivingPacket As SocketBytePacket = Nothing

        Dim partIndex As Int32
        Dim partCount As Int32
        Dim offset As Int32
        Dim length As Int32
        Dim total As Int32

        Do
            Try
                If _socket IsNot Nothing AndAlso _socket.Connected Then
                    If Not receivingBody Then
                        If _socket.Available >= 36 Then
                            _socket.Receive(_receiveHeadBuffer)
                            'check sync
                            If _receiveHeadBuffer(30) = 45 And
                             _receiveHeadBuffer(31) = 7 And
                             _receiveHeadBuffer(32) = 244 And
                             _receiveHeadBuffer(33) = 0 And
                             _receiveHeadBuffer(34) = 170 And
                             _receiveHeadBuffer(35) = 4 Then
                                Dim operation = _receiveHeadBuffer(0)
                                Dim id = BitConverter.ToUInt64(_receiveHeadBuffer, 2)
                                Select Case operation
                                    Case 1
                                        partIndex = BitConverter.ToInt32(_receiveHeadBuffer, 10)
                                        partCount = BitConverter.ToInt32(_receiveHeadBuffer, 14)
                                        offset = BitConverter.ToInt32(_receiveHeadBuffer, 18)
                                        length = BitConverter.ToInt32(_receiveHeadBuffer, 22)
                                        total = BitConverter.ToInt32(_receiveHeadBuffer, 26)
                                        receivingPacket = GetReceivingPacket(id, partCount, total)
                                        receivingBody = True
                                    Case 2
                                        partIndex = BitConverter.ToInt32(_receiveHeadBuffer, 10)
                                        For Each pkt In _sendingPackets
                                            If pkt.PacketID = id Then
                                                If pkt.Parts(partIndex).Transmitted = False Then
                                                    pkt.TransmittedCount += 1
                                                    pkt.Parts(partIndex).Transmitted = True
                                                    Dim delay = (Now - pkt.Parts(partIndex).SendTime).TotalMilliseconds
                                                    If AverageTransmitTime = 0 Then
                                                        _AverageTransmitTime = delay
                                                    Else
                                                        Dim koeff = 0.1
                                                        _AverageTransmitTime = _AverageTransmitTime * (1 - koeff) + delay * koeff
                                                    End If
                                                    Exit For
                                                End If
                                            End If
                                        Next
                                End Select
                            Else
                                'sync fault
                                _socket.Shutdown(SocketShutdown.Both)
                                _socket.Close()
                            End If
                        End If
                    Else
                        If _socket.Available >= length Then
                            _socket.Receive(_receiveBodyBuffer, 0, length, SocketFlags.None)
                            receivingBody = False

                            SyncLock receivingPacket
                                Dim part = receivingPacket.Parts(partIndex)
                                If part.Transmitted = True Then Throw New Exception("Part already transmitted, impossible with TCP")
                                receivingPacket.TransmittedCount += 1
                                part.Transmitted = True
                                part.Length = length
                                part.Offset = offset
                                Array.Copy(_receiveBodyBuffer, 32, receivingPacket.Bytes, part.Offset, part.Length)

                                If receivingPacket.TransmittedCount = receivingPacket.Parts.Count Then
                                    receivingPacket.State.TransmitStarted = True
                                    receivingPacket.State.TransmitComplete = True
                                    receivingPacket.State.TransmitProgress = 1
                                Else
                                    receivingPacket.State.TransmitProgress = receivingPacket.TransmittedCount / receivingPacket.Parts.Count
                                End If
                                _ackBuffer(0) = 2
                                _ackBuffer(1) = 0
                                'sync sequence
                                _ackBuffer(30) = 45
                                _ackBuffer(31) = 7
                                _ackBuffer(32) = 244
                                _ackBuffer(33) = 0
                                _ackBuffer(34) = 170
                                _ackBuffer(35) = 4
                                For i = 2 To 13
                                    _ackBuffer(i) = _receiveHeadBuffer(i)
                                Next
                                Stats.BytesSent += _ackBuffer.Length
                                _socket.Send(_ackBuffer)
                            End SyncLock
                            If receivingPacket.State.TransmitComplete Then
                                _receivingPackets.Remove(receivingPacket)
                                Stats.PacketsReceived += 1
                                RaiseEvent ReceivedPacket(receivingPacket)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(_receiveThreadDelay)
        Loop
    End Sub

    Public ReadOnly Property Socket As Socket
        Get
            Return _socket
        End Get
    End Property

    Public ReadOnly Property IsConnected As Boolean Implements IPacketTransport.IsConnected
        Get
            If _socket Is Nothing Then Return False
            Return _socket.Connected
        End Get
    End Property

    Public Sub Close() Implements IPacketTransport.Close
        Try
            _socket.Close()
        Catch ex As Exception
        End Try
        _socket = Nothing
    End Sub

    Public Sub Open(address As String, options As String) Implements IPacketTransport.Open
        Close()
        Dim parts = address.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length < 2 Or parts.Length > 3 Then Throw New Exception("Address has wrong format! Must be remote_hostname:remote_port or remote_hostname:remote_port:local_port")
        If IsNumeric(parts(1)) = False Then Throw New Exception("Address has wrong format! Must be remote_hostname:remote_port or remote_hostname:remote_port:local_port")

        _socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

        If parts.Length = 3 Then
            If IsNumeric(parts(2)) = False Then Throw New Exception("Address has wrong format! Must be remote_hostname:remote_port:local_port")
            Dim locport = CInt(parts(2))
            _socket.Bind(New IPEndPoint(IPAddress.Any, locport))
        End If
        If parts(0) = "*" Then
            '  _socket.Connect(New IPEndPoint(IPAddress.Broadcast, CInt(parts(1))))
        Else
            _socket.Connect(parts(0), CInt(parts(1)))
        End If
    End Sub

    Public Sub SendPacketAsync(packet As BytePacket) Implements IPacketTransport.SendPacketAsync
        Dim thread As New Threading.Thread(Sub()
                                               Try
                                                   SendPacket(packet)
                                               Catch ex As Exception
                                               End Try
                                           End Sub)
        thread.Start()
    End Sub

    Public Sub SendPacket(packet As BytePacket) Implements IPacketTransport.SendPacket
        If packet.Settings Is Nothing Then packet.Settings = DefaultSettings

        Dim spacket = PrepareSocketBytePacket(packet)
        spacket.State.TransmitComplete = False
        spacket.State.TransmitProgress = 0
        spacket.State.TransmitStarted = True
        spacket.State.TransmitStartTime = Now

        _sendingPackets.Add(spacket)

        Dim started = Now
        Dim sent = 0
        Dim transmitted = 0

        Do While transmitted < spacket.Parts.Count And (Now - started).TotalMilliseconds < spacket.Settings.SendTimeoutMs
            transmitted = spacket.TransmittedCount
            Dim noAck = sent - transmitted
            If noAck < spacket.Settings.AckWaitWindow And sent < spacket.Parts.Count Then
                SendPart(spacket, sent)
                spacket.State.TransmitProgress = transmitted / spacket.Parts.Count
                sent += 1
            Else
                Threading.Thread.Sleep(_sendPacketDelay)
            End If
        Loop
        spacket.State.TransmitFinishTime = Now
        _sendingPackets.Remove(spacket)
        If transmitted = spacket.Parts.Count Then
            spacket.State.TransmitProgress = 1
            spacket.State.TransmitComplete = True
            Stats.PacketsSent += 1
        Else
            Stats.PacketsSendFailed += 1
            Throw New Exception("Transmit incomplete, timeout, only " + transmitted.ToString + " parts from " + spacket.Parts.Count.ToString + " transmitted")
        End If
    End Sub

    Private Function PrepareSocketBytePacket(packet As BytePacket) As SocketBytePacket
        If packet.Settings Is Nothing Then packet.Settings = DefaultSettings

        Dim spacket As New SocketBytePacket(packet)
        spacket.PacketID = _rnd.Next

        Dim parts As Integer = Math.Ceiling(packet.Bytes.LongLength / packet.Settings.PartSize)
        For i = 0 To parts - 1
            Dim part As New SocketBytePacket.Part
            part.Offset = i * packet.Settings.PartSize
            part.Length = packet.Settings.PartSize
            spacket.Parts.Add(part)
        Next
        Dim lastPart = spacket.Parts.Last
        lastPart.Length = packet.Bytes.LongLength - lastPart.Offset

        Return spacket
    End Function

    Private Sub SendPart(packet As SocketBytePacket, partIndex As Integer)
        SyncLock _sendBuffer
            Dim part = packet.Parts(partIndex)
            Dim offsetBytes = BitConverter.GetBytes(part.Offset)
            Dim lengthBytes = BitConverter.GetBytes(part.Length)
            Dim idBytes = BitConverter.GetBytes(packet.PacketID)
            Dim partIndexBytes = BitConverter.GetBytes(partIndex)
            Dim partCountBytes = BitConverter.GetBytes(packet.Parts.Count)
            Dim totalBytes = BitConverter.GetBytes(packet.Bytes.Length)

            _sendBuffer(0) = 1 'packet-send
            _sendBuffer(1) = 0

            For i = 0 To 7
                _sendBuffer(2 + i) = idBytes(i)
            Next

            For i = 0 To 3
                _sendBuffer(10 + i) = partIndexBytes(i)
                _sendBuffer(14 + i) = partCountBytes(i)
                _sendBuffer(18 + i) = offsetBytes(i)
                _sendBuffer(22 + i) = lengthBytes(i)
                _sendBuffer(26 + i) = totalBytes(i)
            Next

            'sync sequence
            _sendBuffer(30) = 45
            _sendBuffer(31) = 7
            _sendBuffer(32) = 244
            _sendBuffer(33) = 0
            _sendBuffer(34) = 170
            _sendBuffer(35) = 4

            Array.Copy(packet.Bytes, part.Offset, _sendBuffer, 36, part.Length)
            part.SendTime = Now
            Stats.BytesSent += part.Length + 36
            _socket.Send(_sendBuffer, part.Length + 36, SocketFlags.None)
            part.Sent = True
        End SyncLock
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Try
                    _receiveThread.Abort()
                Catch ex As Exception
                End Try
                Try
                    _cleanThread.Abort()
                Catch ex As Exception
                End Try
                Try
                    _socket.Dispose()
                Catch ex As Exception
                End Try
            End If
        End If
        disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub
#End Region
End Class
