Imports System.Net.Sockets
Imports Bwl.Network.Transport

Public Class AddressedChannelBase
    Implements IAddressedClient, IConnectionControl

    Protected _channel As IPacketChannel

    Public Shadows Event PacketReceived(transport As IAddressedChannel, packet As StructuredPacket) Implements IAddressedChannel.PacketReceived
    Public Shadows Event PacketSent(transport As IAddressedChannel, packet As StructuredPacket) Implements IAddressedChannel.PacketSent
    Public ReadOnly Property MyID As String = "" Implements IAddressedChannel.MyID
    Public ReadOnly Property MyServiceName As String = "" Implements IAddressedChannel.MyServiceName

    Public ReadOnly Property Channel As IPacketChannel Implements IAddressedClient.Channel
        Get
            Return _channel
        End Get
    End Property

    Public ReadOnly Property ID As Long Implements IConnectionControl.ID
        Get
            Return _channel.ID
        End Get
    End Property

    Public ReadOnly Property IsConnected As Boolean Implements IConnectionControl.IsConnected
        Get
            Return _channel.IsConnected
        End Get
    End Property

    Public Sub Dispose() Implements IDisposable.Dispose
        _channel.Dispose()
    End Sub

    Public Sub Open(address As String, options As String) Implements IConnectionControl.Open
        _channel.Open(address, options)
    End Sub

    Public Sub Close() Implements IConnectionControl.Close
        _channel.Close()
    End Sub

    Public Sub New(channel As IPacketChannel)
        _channel = channel
        AddHandler _channel.PacketReceived, AddressOf BytePacketReceived
    End Sub

    Private Sub BytePacketReceived(transport As IPacketChannel, packet As BytePacket)
        Try
            Dim sbp As New StructuredPacket(packet)
            RaiseEvent PacketReceived(Me, sbp)
        Catch ex As Exception
        End Try
    End Sub

    Public Overridable Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IAddressedChannel.RegisterMe
        _MyID = id
        _MyServiceName = serviceName
    End Sub

    Public Shadows Sub SendPacket(message As StructuredPacket) Implements IAddressedChannel.SendPacket
        If message.AddressFrom = "" Then message.AddressFrom = MyID
        Dim bp = message.ToBytePacket
        _channel.SendPacket(bp)
        RaiseEvent PacketSent(Me, message)
    End Sub

    Public Shadows Sub SendPacketAsync(message As StructuredPacket) Implements IAddressedChannel.SendPacketAsync
        If message.AddressFrom = "" Then message.AddressFrom = MyID
        Dim bp = message.ToBytePacket
        _channel.SendPacketAsync(bp)
        RaiseEvent PacketSent(Me, message)
    End Sub

    Public Overridable Function GetPeersList(serviceName As String, Optional timeout As Single = 20) As String() Implements IAddressedChannel.GetPeersList
        Throw New NotImplementedException
    End Function

    Public Function SendPacketWaitAnswer(message As StructuredPacket, Optional timeout As Single = 20) As StructuredPacket Implements IAddressedChannel.SendPacketWaitAnswer
        SendPacketAsync(message)
        Return WaitPacket(timeout, message.MsgID)
    End Function

    Public Function WaitPacket(Optional timeout As Single = 20, Optional answerToId As Integer = -1, Optional partKey As String = "") As Object Implements IAddressedChannel.WaitPacket
        Dim received As StructuredPacket = Nothing
        AddHandler Me.PacketReceived, Sub(transport As IAddressedChannel, packet As StructuredPacket)
                                          If packet.ReplyToID = answerToId Then received = packet
                                          If partKey > "" AndAlso packet.Parts.ContainsKey(partKey) Then received = packet
                                          If answerToId = -1 And partKey = "" Then received = packet
                                      End Sub
        Dim start = Now
        Do While (Now - start).TotalSeconds < timeout And received Is Nothing
            Threading.Thread.Sleep(1)
        Loop
        Return received
    End Function


End Class