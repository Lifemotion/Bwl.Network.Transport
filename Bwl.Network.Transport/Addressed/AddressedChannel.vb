
Imports System.Net.Sockets
Imports Bwl.Network.Transport

Public Class AddressedChannel
    Inherits AddressedChannelBase
    Implements IAddressedChannel

    Public Sub New(channel As IPacketChannel)
        MyBase.New(channel)
    End Sub

    Public Overrides Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IAddressedChannel.RegisterMe
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
        MyBase.RegisterMe(id, password, serviceName, options)
    End Sub

    Public Overrides Function GetPeersList(serviceName As String, Optional timeout As Single = 20) As String() Implements IAddressedChannel.GetPeersList
        Dim request As New StructuredPacket
        request.Add("@GetPeersList", serviceName)
        Dim response = SendPacketWaitAnswer(request, timeout)
        If response Is Nothing Then Throw New Exception("GetPeersList: no response")
        If response.Parts.ContainsKey("@PeersList") = False Then Throw New Exception("GetPeersList: no PeersList part")
        Dim peersList As String() = response.Parts("@PeersList")
        Return peersList
    End Function
End Class

