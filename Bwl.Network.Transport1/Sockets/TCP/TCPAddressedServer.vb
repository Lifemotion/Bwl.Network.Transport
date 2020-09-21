Public Class TCPAddressedServer
    Inherits AddressedServer

    Public Sub New()
        MyBase.New(New TCPServer)
    End Sub
End Class
