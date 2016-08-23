Imports System.Net.Sockets
Imports System.Net

Public Class TCPServer
    Implements IPortListener
    Private _listener As TcpListener
    Private _listenThread As New Threading.Thread(AddressOf ListenThread)
    Public Event NewConnection(server As IPortListener, transport As IPacketTransport) Implements IPortListener.NewConnection

    Private Sub ListenThread()
        Do
            Try
                If _listener.Pending Then
                    Dim sck = _listener.AcceptSocket
                    sck.NoDelay = True
                    Dim Transport As New TCPTransport(sck)
                    RaiseEvent NewConnection(Me, transport)
                End If
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(10)
        Loop
    End Sub

    Public Sub New(port As Integer)
        _listener = New TcpListener(IPAddress.Any, port)
        _listener.Start()
        _listenThread.IsBackground = True
        _listenThread.Name = "TCPServer"
        _listenThread.Start()
    End Sub

End Class
