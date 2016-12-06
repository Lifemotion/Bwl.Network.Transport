Public Class StructuredPacketPart
    ReadOnly Property Type As Type
    ReadOnly Property Value As Object
    ReadOnly Property Key As String
    ReadOnly Property Coder As StructuredPacketPartCoders.IStructuredPacketCoder

    Public Sub New(key As String, value As Object)

    End Sub
End Class
