Namespace StructuredPacketPartCoders
    Public Interface IStructuredPacketCoder
        ReadOnly Property Type As Type
        ReadOnly Property TypeCode As Byte
        Function ToBytes(value As Object) As Byte()
        Function ToObject(bytes() As Byte, index As Integer, count As Integer)
    End Interface

    Friend Class CoderTools
        Private Shared _utf8 As Text.Encoding = Text.Encoding.UTF8

        Public Shared Function StringToBytes(str As String) As Byte()
            Return _utf8.GetBytes(str)
        End Function

        Public Shared Function BytesToString(bytes As Byte(), index As Integer, count As Integer) As String
            Return _utf8.GetString(bytes, index, count)
        End Function
    End Class

    Public Class IntegerCoder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(Integer) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 10 Implements IStructuredPacketCoder.TypeCode
        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Return CoderTools.StringToBytes(value.ToString)
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Return CInt(CoderTools.BytesToString(bytes, index, count))
        End Function
    End Class

    Public Class LongCoder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(Long) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 11 Implements IStructuredPacketCoder.TypeCode
        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Return CoderTools.StringToBytes(value.ToString)
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Return CLng(CoderTools.BytesToString(bytes, index, count))
        End Function
    End Class

    Public Class SingleCoder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(Single) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 20 Implements IStructuredPacketCoder.TypeCode
        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Dim singleValue As Single = value
            Return CoderTools.StringToBytes(singleValue.ToString(Globalization.CultureInfo.InvariantCulture))
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Return Single.Parse(CoderTools.BytesToString(bytes, index, count), Globalization.CultureInfo.InvariantCulture)
        End Function
    End Class

    Public Class DoubleCoder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(Double) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 21 Implements IStructuredPacketCoder.TypeCode
        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Dim singleValue As Double = value
            Return CoderTools.StringToBytes(singleValue.ToString(Globalization.CultureInfo.InvariantCulture))
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Return Double.Parse(CoderTools.BytesToString(bytes, index, count), Globalization.CultureInfo.InvariantCulture)
        End Function
    End Class

    Public Class StringCoder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(String) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 30 Implements IStructuredPacketCoder.TypeCode
        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Return CoderTools.StringToBytes(value.ToString)
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Return CoderTools.BytesToString(bytes, index, count)
        End Function
    End Class



    Public Class BooleanCoder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(Boolean) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 31 Implements IStructuredPacketCoder.TypeCode
        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Return CoderTools.StringToBytes(value.ToString)
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Return CoderTools.BytesToString(bytes, index, count).ToLower.Trim = "true"
        End Function
    End Class

    Public Class DateCoder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(Date) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 32 Implements IStructuredPacketCoder.TypeCode

        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Dim dateValue As Date = value
            Return CoderTools.StringToBytes(dateValue.ToBinary.ToString)
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Return Date.FromBinary(CLng(CoderTools.BytesToString(bytes, index, count)))
        End Function
    End Class

    Public Class ByteArrayCoder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(Byte()) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 0 Implements IStructuredPacketCoder.TypeCode

        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Return value
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Dim result(count - 1) As Byte
            Array.Copy(bytes, index, result, 0, count)
            Return result
        End Function
    End Class

    Public Class StringArrayCoder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(String()) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 40 Implements IStructuredPacketCoder.TypeCode

        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Dim vals As String() = value
            Dim ms As New IO.MemoryStream
            For i = 0 To vals.Length - 1
                Dim valBytes = CoderTools.StringToBytes(vals(i))
                ms.Write(valBytes, 0, valBytes.Length)
                ms.WriteByte(0)
            Next
            Dim bytes = ms.ToArray
            Return bytes
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Dim result As New List(Of String)
            Dim start = index
            For i = index To index + count - 1
                If bytes(i) = 0 Then
                    result.Add(CoderTools.BytesToString(bytes, start, i - start))
                    start = i + 1
                End If
            Next
            Return result.ToArray
        End Function
    End Class

    Public Class ChecksumCRC32Coder
        Implements IStructuredPacketCoder
        Public ReadOnly Property Type As Type = GetType(ChecksumCRC32Coder) Implements IStructuredPacketCoder.Type
        Public ReadOnly Property TypeCode As Byte = 254 Implements IStructuredPacketCoder.TypeCode

        Public Function ToBytes(value As Object) As Byte() Implements IStructuredPacketCoder.ToBytes
            Throw New NotImplementedException
        End Function

        Public Function ToObject(bytes() As Byte, index As Integer, count As Integer) As Object Implements IStructuredPacketCoder.ToObject
            Throw New NotImplementedException
        End Function
    End Class

    Public Class Coders
        Public Shared Property List As New List(Of IStructuredPacketCoder)

        Public Shared Sub AddCoder(newCoder As IStructuredPacketCoder)
            For Each coder In List
                If coder.TypeCode = newCoder.TypeCode Then Throw New Exception("Coder with this TypeCode exists")
                If coder.Type = newCoder.Type Then Throw New Exception("Coder with this Type exists")
            Next
            List.Add(newCoder)
        End Sub

        Public Shared Function Find(type As Type) As IStructuredPacketCoder
            For Each coder In List
                If coder.Type = type Then Return coder
            Next
            Return Nothing
        End Function

        Public Shared Function Find(typeCode As Byte) As IStructuredPacketCoder
            For Each coder In List
                If coder.TypeCode = typeCode Then Return coder
            Next
            Return Nothing
        End Function

        Shared Sub New()
            AddCoder(New IntegerCoder)
            AddCoder(New LongCoder)
            AddCoder(New SingleCoder)
            AddCoder(New DoubleCoder)
            AddCoder(New StringCoder)
            AddCoder(New BooleanCoder)
            AddCoder(New DateCoder)
            AddCoder(New ByteArrayCoder)
            AddCoder(New ChecksumCRC32Coder)
            AddCoder(New StringArrayCoder)
        End Sub
    End Class
End Namespace
