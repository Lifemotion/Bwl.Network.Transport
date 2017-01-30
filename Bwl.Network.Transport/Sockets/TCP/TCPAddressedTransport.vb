Imports Bwl.Network.Transport

Public Class TCPAddressedTransport
    Inherits TCPTransport
    Implements IAddressedTransport

    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IAddressedTransport.RegisterClientRequest
    Public Shadows Event PacketReceived(source As IAddressedTransport, packet As StructuredPacket) Implements IAddressedTransport.PacketReceived
    Public Shadows Event PacketSent(source As IAddressedTransport, packet As StructuredPacket) Implements IAddressedTransport.PacketSent
    Public ReadOnly Property MyID As String = "" Implements IAddressedTransport.MyID
    Public ReadOnly Property MyServiceName As String = "" Implements IAddressedTransport.MyServiceName

    Public Sub New()
        AddHandler MyBase.PacketReceived, AddressOf BytePacketReceived
    End Sub

    Private Sub BytePacketReceived(packet As BytePacket)
        Try
            Dim sbp As New StructuredPacket(packet)
            RaiseEvent PacketReceived(Me, sbp)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IAddressedTransport.RegisterMe
        Dim request As New StructuredPacket
        request.Add("@RegisterMe", id)
        request.Add("@RegisterMethod", "simple")
        request.Add("@RegisterService", serviceName)
        request.Add("@RegisterOptions", options)
        request.Add("@RegisterPassword", password)
        Dim response = SendPacketWaitAnswer(request)
        If response Is Nothing Then Throw New Exception("RegisterMe: no response")
        If response.Parts.ContainsKey("@RegisterResult") = False Then Throw New Exception("RegisterMe: no PeersList part")
        Dim result As String = response.Parts("@RegisterResult")
        If result <> "OK" Then Throw New Exception(result)
        _MyID = id
        _MyServiceName = serviceName
    End Sub

    Public Shadows Sub SendPacket(message As StructuredPacket) Implements IAddressedTransport.SendPacket
        Dim bp = message.ToBytePacket
        MyBase.SendPacket(bp)
        RaiseEvent PacketSent(Me, message)
    End Sub

    Public Shadows Sub SendPacketAsync(message As StructuredPacket) Implements IAddressedTransport.SendPacketAsync
        Dim bp = message.ToBytePacket
        MyBase.SendPacketAsync(bp)
        RaiseEvent PacketSent(Me, message)
    End Sub

    Public Function GetPeersList(serviceName As String, Optional timeout As Single = 20) As String() Implements IAddressedTransport.GetPeersList
        Dim request As New StructuredPacket
        request.Add("@GetPeersList", serviceName)
        Dim response = SendPacketWaitAnswer(request, timeout)
        If response Is Nothing Then Throw New Exception("GetPeersList: no response")
        If response.Parts.ContainsKey("PeersList") = False Then Throw New Exception("GetPeersList: no PeersList part")
        Dim peersList As String() = response.Parts("PeersList")
        Return peersList
    End Function

    Public Function SendPacketWaitAnswer(message As StructuredPacket, Optional timeout As Single = 20) As StructuredPacket Implements IAddressedTransport.SendPacketWaitAnswer
        SendPacketAsync(message)
        Return WaitPacket(timeout, message.MsgID)
    End Function

    Public Function WaitPacket(Optional timeout As Single = 20, Optional answerToId As Integer = -1, Optional partKey As String = "") As Object Implements IAddressedTransport.WaitPacket
        Dim received As StructuredPacket = Nothing
        AddHandler Me.PacketReceived, Sub(source As IAddressedTransport, packet As StructuredPacket)
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
