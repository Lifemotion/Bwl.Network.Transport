Imports System.Net.Sockets
Imports System.Net
Imports Bwl.Network.Transport

Public Class TCPConnection
    Implements IConnectedClient
    Public ReadOnly Property TcpTransport As TCPTransport

    Public ReadOnly Property Transport As IPacketTransport Implements IConnectedClient.Transport
        Get
            Return TcpTransport
        End Get
    End Property

    Public Property Info As Object Implements IConnectedClient.Info

    Public Sub New(transport As TCPTransport)
        TcpTransport = transport
    End Sub
End Class

Public Class TCPPortListener
    Implements IPortListener, IDisposable

    Private _activeConnections As New List(Of TCPConnection)
    Private _listener As TcpListener
    Private _listenThread As New Threading.Thread(AddressOf ListenThread)
    Private _cleanThread As New Threading.Thread(AddressOf CleanThread)
    Private _parameters As TCPTransport.TCPTransportParameters

    Public Event NewConnection(server As IPortListener, connection As IConnectedClient) Implements IPortListener.NewConnection

    Private Sub ListenThread()
        Do
            If _listener IsNot Nothing Then
                Try
                    If _listener.Pending Then
                        Dim sck = _listener.AcceptSocket
                        Dim conn As New TCPConnection(New TCPTransport(sck, _parameters))
                        SyncLock _activeConnections
                            _activeConnections.Add(conn)
                        End SyncLock
                        RaiseEvent NewConnection(Me, conn)
                    End If
                Catch ex As Exception
                End Try
                Threading.Thread.Sleep(10)
            End If
        Loop
    End Sub

    Public Sub New()
        _listenThread.IsBackground = True
        _listenThread.Name = "TCPServer_ListenThread"
        _listenThread.Start()

        _cleanThread.Name = "TCPServer_CleanThread"
        _cleanThread.IsBackground = True
        _cleanThread.Start()
    End Sub

    Public Sub Open(address As String, options As String) Implements IConnectionControl.Open
        Dim parts = address.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length <> 2 Then Throw New Exception("Address has wrong format! Must be hostname:port")
        If IsNumeric(parts(1)) = False Then Throw New Exception("Address has wrong format! Must be hostname:port")
        Open(CInt(Val(parts(1))))
    End Sub

    Public Sub Open(port As Integer)
        Open(port, New TCPTransport.TCPTransportParameters)
    End Sub

    Public Sub Open(port As Integer, parameters As TCPTransport.TCPTransportParameters)
        Close()
        _parameters = parameters
        _listener = New TcpListener(IPAddress.Any, port)
        _listener.Start()
    End Sub

    Public Sub Close() Implements IConnectionControl.Close
        If _listener IsNot Nothing Then
            _listener.Stop()
            _listener = Nothing
        End If
    End Sub

    Private Sub CleanThread()
        Do
            If _listener IsNot Nothing Then
                Try
                    Dim removeTransport As TCPConnection = Nothing
                    SyncLock _activeConnections
                        For Each trn In _activeConnections
                            If trn.Transport.IsConnected = False Then
                                removeTransport = trn
                            End If
                        Next
                        If removeTransport IsNot Nothing Then
                            removeTransport.TcpTransport.Dispose()
                            _activeConnections.Remove(removeTransport)
                        End If
                    End SyncLock
                Catch ex As Exception
                End Try
            End If
            Threading.Thread.Sleep(10)
        Loop
    End Sub

    Public ReadOnly Property ActiveConnections As IConnectedClient() Implements IPortListener.ActiveConnections
        Get
            Return _activeConnections.ToArray
        End Get
    End Property

    Public ReadOnly Property ID As Long Implements IConnectionControl.ID

    Public ReadOnly Property IsConnected As Boolean Implements IConnectionControl.IsConnected
        Get
            Return (_listener IsNot Nothing)
        End Get
    End Property

#Region "IDisposable Support"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Try
                    _listenThread.Abort()
                Catch ex As Exception
                End Try
                Try
                    _cleanThread.Abort()
                Catch ex As Exception
                End Try
                Try
                    _listener.Stop()
                    _listener = Nothing
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
