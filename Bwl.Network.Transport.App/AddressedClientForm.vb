Imports Bwl.Framework
Imports Bwl.Network.Transport

Public Class AddressedClientForm
    Inherits FormAppBase
    Protected _address As New StringSetting(_storage, "Client Address", "localhost:4080")
    Protected _options As New StringSetting(_storage, "Client Options", "")
    Protected _login As New StringSetting(_storage, "Client Login", "Client 1")
    Protected _password As New StringSetting(_storage, "Client Password", "")
    Protected _service As New StringSetting(_storage, "Client Service", "Service")

    Protected WithEvents _client As IAddressedClient = New TCPAddressedChannel

    Private Sub AddressedClientForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SettingField1.AssignedSetting = _address
        SettingField2.AssignedSetting = _options
        SettingField3.AssignedSetting = _login
        SettingField4.AssignedSetting = _password
        SettingField5.AssignedSetting = _service
    End Sub

    Private Sub bClient_Click(sender As Object, e As EventArgs) Handles bClient.Click
        Try
            _client.Open(_address.Value, _options.Value)
            _client.RegisterMe(_login.Value, _password.Value, _service.Value, "")
            _logger.AddMessage("client.Open ok")
        Catch ex As Exception
            _logger.AddError("client.Open " + ex.Message)
        End Try
    End Sub

    Private Sub bClose_Click(sender As Object, e As EventArgs) Handles bClose.Click
        Try
            _client.Close()
            _logger.AddMessage("client.Close ok")
        Catch ex As Exception
            _logger.AddError("client.Close " + ex.Message)
        End Try
    End Sub

    Private Sub _client_PacketReceived(channel As IAddressedChannel, packet As StructuredPacket) Handles _client.PacketReceived
        _logger.AddMessage("R: " + packet.ToString)
    End Sub

    Private Sub bSend_Click(sender As Object, e As EventArgs) Handles bSend.Click
        Dim pkt As New StructuredPacket
        pkt.AddressTo = tbAddressTo.Text
        Dim lines = tbMessage.Lines
        For Each line In lines
            Dim parts = line.Split("=")
            If parts.Length = 2 Then
                Dim value = parts(1)
                If IsNumeric(value) Then
                    If CInt(Val(value)) = value Then
                        pkt.Add(parts(0), CInt(Val(value)))
                    Else
                        pkt.Add(parts(0), Val(value))
                    End If
                Else
                    pkt.Add(parts(0), value)
                End If
            End If
        Next
        _client.SendPacket(pkt)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim results = TransportNetFinder.Find(1000)
    End Sub

    Private Sub _btnGetPeers_Click(sender As Object, e As EventArgs) Handles _btnGetPeers.Click
        Try
            Dim peers = _client.GetPeersList("")
            Dim str = "peers "
            For Each peer In peers
                str += peer + " ; "
            Next
            _logger.AddInformation(str)
        Catch ex As Exception
            _logger.AddWarning(ex.Message)
        End Try

    End Sub
End Class