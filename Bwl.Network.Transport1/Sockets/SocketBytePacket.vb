Public Class SocketBytePacket
    Inherits BytePacket
    Public Class Part
        Public Property Offset As Int32
        Public Property Length As Int32
        Public Property Transmitted As Boolean
        Public Property Sent As Boolean
        Public Property Retransmits As Integer
        Public Property SendTime As DateTime
    End Class

    Public ReadOnly Property Parts As New List(Of Part)
    Public Property TransmittedCount As Integer

    Public Property PacketID As UInt64

    Public Sub New()

    End Sub

    Public Sub New(packet As BytePacket)
        MyBase.New(packet)
    End Sub

    Public Sub New(id As UInt64, partsCount As Integer, totalBytes As Integer)
        PacketID = id
        ReDim Bytes(totalBytes - 1)
        For i = 0 To partsCount - 1
            Parts.Add(New Part)
        Next
    End Sub

    Public Sub CalculateTransmittedCount()
        Dim transmitted As Integer = 0
        For Each prt In Parts
            If prt.Transmitted Then transmitted += 1
        Next
        _TransmittedCount = transmitted
    End Sub
End Class
