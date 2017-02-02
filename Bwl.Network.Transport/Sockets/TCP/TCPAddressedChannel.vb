Public Class TCPAddressedChannel
    Inherits AddressedChannel

    Public Sub New()
        MyBase.New(New TCPChannel)
    End Sub
End Class
