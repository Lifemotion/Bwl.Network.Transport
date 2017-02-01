Imports System.Net.Sockets
Imports System.Net
Imports Bwl.Network.Transport

''' <summary>
''' UDP-based Packet Transport with only protocol-based features. Packet length must be less than 64k, no acknowledgments, only async sending.
''' If SendPacket completed (no exception), it means that packet is sent, but no information about delivery status.
''' </summary>
Public Class UDPChannelSimple
    Implements IPacketChannel, IDisposable
    Public Event PacketReceived(transport As IPacketChannel, packet As BytePacket) Implements IPacketChannel.PacketReceived
    Public Event PacketSent(transport As IPacketChannel, packet As BytePacket) Implements IPacketChannel.PacketSent
    Public Property DefaultSettings As New BytePacketSettings Implements IPacketChannel.DefaultSettings
    Public ReadOnly Property Stats As New PacketTransportStats Implements IPacketChannel.Stats

    Private _socket As Socket
    Private _receiveThread As New Threading.Thread(AddressOf ReceiveThread)
    Private _receiveBuffer(65535) As Byte
    Private _receiveThreadDelay As TimeSpan = TimeSpan.FromTicks(1)

    Private Shared _sharedID As Long
    Private Shared _sharedIDSync As New Object

    Public ReadOnly Property ID As Long Implements IPacketChannel.ID

    Public Sub New()
        SyncLock _sharedIDSync
            _sharedID += 1
            ID = _sharedID
        End SyncLock

        _receiveThread.Name = "UDPTransport_ReceiveThread"
        _receiveThread.Start()
    End Sub

    Private Sub ReceiveThread()
        Do
            Try
                If _socket IsNot Nothing AndAlso _socket.Connected Then
                    Dim read = _socket.Receive(_receiveBuffer)
                    If read > 0 Then
                        Dim bytes(read - 1) As Byte
                        Array.Copy(_receiveBuffer, bytes, read)
                        Dim pkt As New BytePacket(bytes)
                        pkt.State.TransmitComplete = True
                        pkt.State.TransmitProgress = 1
                        pkt.State.TransmitStarted = 1
                        pkt.State.TransmitStartTime = Now
                        RaiseEvent PacketReceived(Me, pkt)
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

    Public ReadOnly Property IsConnected As Boolean Implements IPacketChannel.IsConnected
        Get
            If _socket Is Nothing Then Return False
            Return _socket.Connected
        End Get
    End Property

    Public Sub Close() Implements IPacketChannel.Close
        Try
            _socket.Close()
        Catch ex As Exception
        End Try
        _socket = Nothing
    End Sub

    Public Sub Open(address As String, options As String) Implements IPacketChannel.Open
        Close()
        Dim parts = address.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length < 2 Or parts.Length > 3 Then Throw New Exception("Address has wrong format! Must be remote_hostname:remote_port or remote_hostname:remote_port:local_port")
        If IsNumeric(parts(1)) = False Then Throw New Exception("Address has wrong format! Must be remote_hostname:remote_port or remote_hostname:remote_port:local_port")
        _socket = New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
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

    Public Sub SendPacketAsync(packet As BytePacket) Implements IPacketChannel.SendPacketAsync
        _socket.Send(packet.Bytes, packet.Bytes.Length, SocketFlags.None)
    End Sub

    Public Sub SendPacket(packet As BytePacket) Implements IPacketChannel.SendPacket
        Throw New InvalidOperationException("Sync SendPacket not supported. Use SendPacketAsync")
    End Sub

    Public Function Ping(maximumTimeoutMs As Integer) As Integer Implements IPacketChannel.Ping
        Dim pkt As New BytePacket({}, New BytePacketSettings With {.SendTimeoutMs = maximumTimeoutMs})
        Try
            SendPacket(pkt)
            Return (pkt.State.TransmitFinishTime - pkt.State.TransmitStartTime).TotalMilliseconds
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
