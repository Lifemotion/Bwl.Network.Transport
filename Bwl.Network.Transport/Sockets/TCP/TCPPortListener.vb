Imports System.Net.Sockets
Imports System.Net

Public Class TCPPortListener
    Implements IPortListener, IDisposable

    Private _activeConnections As New List(Of TCPTransport)
    Private _listener As TcpListener
    Private _listenThread As New Threading.Thread(AddressOf ListenThread)
    Private _cleanThread As New Threading.Thread(AddressOf CleanThread)
    Private _parameters As TCPTransport.TCPTransportParameters

    Public Event NewConnection(server As IPortListener, transport As IPacketTransport) Implements IPortListener.NewConnection

    Private Sub ListenThread()
        Do
            Try
                If _listener.Pending Then
                    Dim sck = _listener.AcceptSocket
                    Dim trn As New TCPTransport(sck, _parameters)
                    SyncLock _activeConnections
                        _activeConnections.Add(trn)
                    End SyncLock
                    RaiseEvent NewConnection(Me, trn)
                End If
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(10)
        Loop
    End Sub

    Public Sub New(port As Integer)
        Me.New(port, New TCPTransport.TCPTransportParameters)
    End Sub

    Public Sub New(port As Integer, parameters As TCPTransport.TCPTransportParameters)
        _parameters = parameters

        _listener = New TcpListener(IPAddress.Any, port)
        _listener.Start()
        _listenThread.IsBackground = True
        _listenThread.Name = "TCPServer_ListenThread"
        _listenThread.Start()

        _cleanThread.Name = "TCPServer_CleanThread"
        _cleanThread.IsBackground = True
        _cleanThread.Start()
    End Sub

    Private Sub CleanThread()
        Do
            Try
                Dim removeTransport As TCPTransport = Nothing
                SyncLock _activeConnections
                    For Each trn In _activeConnections
                        If trn.IsConnected = False Then
                            removeTransport = trn
                        End If
                    Next
                    If removeTransport IsNot Nothing Then
                        removeTransport.Dispose()
                        _activeConnections.Remove(removeTransport)
                    End If
                End SyncLock
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(10)
        Loop
    End Sub

    Public ReadOnly Property ActiveConnections As IPacketTransport() Implements IPortListener.ActiveConnections
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
