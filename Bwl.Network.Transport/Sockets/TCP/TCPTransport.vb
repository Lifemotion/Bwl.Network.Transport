Imports System.Net.Sockets
Imports System.Net
Imports Bwl.Network.Transport

''' <summary>
''' TCP-based Packet Transport with big packets support, sync or async sending.
''' If SendPacket completed (no exception), it means packet fully sent and fully received by other side.
''' </summary>
Public Class TCPTransport
    Implements IPacketTransport, IDisposable

    Public Class TCPTransportParameters
        Public Property DefaultPartSize As Integer = 1024 * 61
        Public Property BuffersSize As Integer = 1024 * 256
        Public Property UseReceiverThreadDelays As Boolean = True
    End Class

    Public Event ReceivedPacket(packet As BytePacket) Implements IPacketTransport.ReceivedPacket
    Public Event SentPacket(packet As BytePacket) Implements IPacketTransport.SentPacket
    Public Property DefaultSettings As New BytePacketSettings Implements IPacketTransport.DefaultSettings
    Public ReadOnly Property Stats As New PacketTransportStats Implements IPacketTransport.Stats

    Private Const _headerSize As Integer = 36
    Private _socket As Socket
    Private _receiveThread As New Threading.Thread(AddressOf ReceiveThread)
    Private _receiveBodyBuffer() As Byte
    Private _receiveHeadBuffer(_headerSize - 1) As Byte

    Private _sendBuffer() As Byte
    Private _rnd As New Random

    Private _cleanThread As New Threading.Thread(AddressOf CleanThread)

    Private _receivingPackets As New List(Of SocketBytePacket)
    Private _sendingPackets As New List(Of SocketBytePacket)
    Private _parameters As TCPTransportParameters

    Public Sub New()
        Me.New(Nothing, New TCPTransportParameters)
    End Sub

    Private Shared _sharedID As Long
    Private Shared _sharedIDSync As New Object

    Public ReadOnly Property ID As Long Implements IPacketTransport.ID

    Private Sub SetupSocket(socket As Socket)
        If socket IsNot Nothing Then
            socket.NoDelay = True
            socket.Blocking = True
            If socket.ReceiveBufferSize <> _receiveBodyBuffer.Length Then socket.ReceiveBufferSize = _receiveBodyBuffer.Length
            If socket.SendBufferSize <> _sendBuffer.Length Then socket.SendBufferSize = _sendBuffer.Length
        End If
    End Sub

    Public Sub New(socket As Socket, parameters As TCPTransportParameters)
        SyncLock _sharedIDSync
            _sharedID += 1
            ID = _sharedID
        End SyncLock

        _parameters = parameters
        _socket = socket
        _receiveThread.Name = "UDPTransport_ReceiveThread"
        _receiveThread.Start()

        _cleanThread.Name = "UDPTransport_CleanThread"
        _cleanThread.IsBackground = True
        _cleanThread.Start()

        ReDim _receiveBodyBuffer(_parameters.BuffersSize - 1)
        ReDim _sendBuffer(_parameters.BuffersSize - 1)

        SetupSocket(_socket)

        DefaultSettings.AckWaitWindow = 1
        DefaultSettings.MaxAllowedRetransmits = 0
        DefaultSettings.PartSize = _parameters.DefaultPartSize
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
                        If _socket.Available >= _headerSize Then
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
                                    Case 3
                                        'ping
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
                                Array.Copy(_receiveBodyBuffer, 0, receivingPacket.Bytes, part.Offset, part.Length)

                                If receivingPacket.TransmittedCount = receivingPacket.Parts.Count Then
                                    receivingPacket.State.TransmitStarted = True
                                    receivingPacket.State.TransmitComplete = True
                                    receivingPacket.State.TransmitProgress = 1
                                Else
                                    receivingPacket.State.TransmitProgress = receivingPacket.TransmittedCount / receivingPacket.Parts.Count
                                End If
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
                Stop
            End Try
            If _socket Is Nothing OrElse _socket.Connected = False OrElse _socket.Available = 0 Then
                If _parameters.UseReceiverThreadDelays Then
                    Threading.Thread.Sleep(1)
                Else
                    Threading.Thread.Sleep(0)
                End If
            End If
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
        SetupSocket(_socket)
        If parts.Length = 3 Then
            If IsNumeric(parts(2)) = False Then Throw New Exception("Address has wrong format! Must be remote_hostname:remote_port:local_port")
            Dim locport = CInt(parts(2))
            _socket.Bind(New IPEndPoint(IPAddress.Any, locport))
        End If
        If parts(0) = "*" Then
            '  _socket.Connect(New IPEndPoint(IPAddress.Broadcast, CInt(parts(1))))
        Else
            Try
                _socket.Connect(parts(0), CInt(parts(1)))
            Catch ex As Exception
                If options.ToLower.Contains("ignoreerrors") = False Then Throw ex
            End Try
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
        Try

            Do While sent < spacket.Parts.Count And (Now - started).TotalMilliseconds < spacket.Settings.SendTimeoutMs
                SendPart(spacket, sent)
                spacket.State.TransmitProgress = sent / spacket.Parts.Count
                sent += 1
            Loop
            _sendingPackets.Remove(spacket)
            spacket.State.TransmitFinishTime = Now
            spacket.State.TransmitProgress = 1
            spacket.State.TransmitComplete = True
            Stats.PacketsSent += 1
        Catch ex As Exception
            Stats.PacketsSendFailed += 1
            Throw New Exception("Transmit incomplete, timeout, only " + sent.ToString + " parts from " + spacket.Parts.Count.ToString + " transmitted")
        End Try
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

            Array.Copy(packet.Bytes, part.Offset, _sendBuffer, _headerSize, part.Length)
            part.SendTime = Now
            Stats.BytesSent += part.Length + _headerSize
            _socket.Send(_sendBuffer, part.Length + _headerSize, SocketFlags.None)
            part.Sent = True
            part.Transmitted = True
        End SyncLock
    End Sub

    Public Function Ping(maximumTimeoutMs As Integer) As Integer Implements IPacketTransport.Ping
        Try
            Dim startTime As DateTime
            Dim finishTime As DateTime
            SyncLock _sendBuffer
                startTime = Now
                For i = 0 To _headerSize - 1
                    _sendBuffer(i) = 0
                Next
                _sendBuffer(0) = 3 'ping
                'sync sequence
                _sendBuffer(30) = 45
                _sendBuffer(31) = 7
                _sendBuffer(32) = 244
                _sendBuffer(33) = 0
                _sendBuffer(34) = 170
                _sendBuffer(35) = 4
                Stats.BytesSent += 36
                Dim send = _socket.Send(_sendBuffer, 36, SocketFlags.None)
                '_socket.Send(_sendBuffer, 36, SocketFlags.None)
                finishTime = Now
            End SyncLock
            Return (finishTime - startTime).TotalMilliseconds + 1
        Catch ex As Exception
            Return -1
        End Try
    End Function

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
