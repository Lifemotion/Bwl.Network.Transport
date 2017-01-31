Imports System.Net.Sockets
Imports System.Net

Public Class TCPConnection
    Implements IConnectionInfo
    Public ReadOnly Property TcpTransport As TCPTransport

    Public ReadOnly Property Transport As IPacketTransport Implements IConnectionInfo.Transport
        Get
            Return TcpTransport
        End Get
    End Property

    Public Property Info As Object Implements IConnectionInfo.Info

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

    Public Event NewConnection(server As IPortListener, connection As IConnectionInfo) Implements IPortListener.NewConnection

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

    Public Sub Start(port As Integer)
        Start(port, New TCPTransport.TCPTransportParameters)
    End Sub

    Public Sub Start(port As Integer, parameters As TCPTransport.TCPTransportParameters)
        Close()
        _parameters = parameters
        _listener = New TcpListener(IPAddress.Any, port)
        _listener.Start()
    End Sub

    Public Sub Close()
        If _listener IsNot Nothing Then
            _listener.Stop()
            _listener = Nothing
        End If
    End Sub

    Public Function IsWorking() As Boolean
        Return (_listener IsNot Nothing)
    End Function

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

    Public ReadOnly Property ActiveConnections As IConnectionInfo() Implements IPortListener.ActiveConnections
        Get
            Return _activeConnections.ToArray
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
