Imports System.Text

Public Class MessageBytePacket
    Private ReadOnly _parts As List(Of Byte())

#Region "Parts"
    Public ReadOnly Property Count() As Integer
        Get
            Return _parts.Count
        End Get
    End Property

    Public ReadOnly Property GetPartType(index As Int32) As MessageBytePartType
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            Return GetMessageBytePartType(_parts(index).Take(1).ToArray().First())
        End Get

    End Property

    Public Property Part(index As Int32) As Byte()
        Get

            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.ByteType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return _parts(index).Skip(9).ToArray()
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.ByteType)
            Dim length As Long = Value.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            Value.CopyTo(array, 9)

            _parts(index) = array
        End Set
    End Property

    Public Property PartBoolean(index As Int32) As Boolean
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.BooleanType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return BitConverter.ToBoolean(_parts(index).Skip(9).ToArray(), 0)
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim res = BitConverter.GetBytes(Value)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.BooleanType)
            Dim length As Long = res.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            res.CopyTo(array, 9)
            _parts(index) = array
        End Set
    End Property

    Public Property PartChar(index As Int32) As Char
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.CharType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return BitConverter.ToChar(_parts(index).Skip(9).ToArray(), 0)
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim res = BitConverter.GetBytes(Value)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.CharType)
            Dim length As Long = res.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            res.CopyTo(array, 9)
            _parts(index) = array
        End Set
    End Property

    Public Property PartDate(index As Int32) As Date
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.DateType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return New DateTime(BitConverter.ToInt64(_parts(index).Skip(9).ToArray(), 0))
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim res = BitConverter.GetBytes(Value.Ticks)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.DateType)
            Dim length As Long = res.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            res.CopyTo(array, 9)
            _parts(index) = array
        End Set
    End Property

    Public Property PartDouble(index As Int32) As Double
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.DoubleType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return BitConverter.ToDouble(_parts(index).Skip(9).ToArray(), 0)
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim res = BitConverter.GetBytes(Value)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.DoubleType)
            Dim length As Long = res.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            res.CopyTo(array, 9)
            _parts(index) = array
        End Set
    End Property

    Public Property PartInteger(index As Int32) As Integer
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.IntegerType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return BitConverter.ToInt32(_parts(index).Skip(9).ToArray(), 0)
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim res = BitConverter.GetBytes(Value)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.IntegerType)
            Dim length As Long = res.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            res.CopyTo(array, 9)
            _parts(index) = array
        End Set
    End Property

    Public Property PartLong(index As Int32) As Long
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.LongType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return BitConverter.ToInt64(_parts(index).Skip(9).ToArray(), 0)
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim res = BitConverter.GetBytes(Value)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.LongType)
            Dim length As Long = res.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            res.CopyTo(array, 9)
            _parts(index) = array
        End Set
    End Property

    Public Property PartShort(index As Int32) As Short
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.ShortType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return BitConverter.ToInt16(_parts(index).Skip(9).ToArray(), 0)
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim res = BitConverter.GetBytes(Value)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.ShortType)
            Dim length As Long = res.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            res.CopyTo(array, 9)
            _parts(index) = array
        End Set
    End Property

    Public Property PartSingle(index As Int32) As Single
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.SingleType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return BitConverter.ToSingle(_parts(index).Skip(9).ToArray(), 0)
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim res = BitConverter.GetBytes(Value)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.SingleType)
            Dim length As Long = res.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            res.CopyTo(array, 9)
            _parts(index) = array
        End Set
    End Property

    Public Property PartString(index As Int32) As String
        Get
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then Return Nothing
            If Not GetPartType(index) = MessageBytePartType.StringType Then
                Throw New Exception("Значение не соответствует изначальному типу. Воспользуйтесь GetPartType, чтобы узнать тип части")
            End If
            Return Encoding.UTF8.GetString(_parts(index).Skip(9).ToArray())
        End Get
        Set
            If index < 0 Then Throw New Exception("Индекс не может быть меньше 0!")
            If Not CheckPartCount(index) Then GetEnoughParts(index)
            Dim res = Encoding.UTF8.GetBytes(Value)
            Dim type As Byte = GetMessageBytePartType(MessageBytePartType.StringType)
            Dim length As Long = res.Count()

            Dim array = New Byte(8 + length) {}
            array(0) = type
            BitConverter.GetBytes(length).CopyTo(array, 1)
            res.CopyTo(array, 9)
            _parts(index) = array
        End Set
    End Property
#End Region

    Public Sub New(bytes As Byte())
        _parts = GetParts(bytes)
    End Sub

    Public Sub New()
        _parts = New List(Of Byte())
    End Sub

    Public Function GetParts(bytes As Byte()) As List(Of Byte())
        Dim result = New List(Of Byte())()
        Try

            Dim partCount = 0
            While partCount < bytes.Count()
                Dim partLength = BitConverter.ToInt64(bytes.Skip(partCount + 1).Take(8).ToArray(), 0)
                Dim bytesPart = bytes.Skip(partCount).Take(9 + partLength).ToArray()
                result.Add(bytesPart)
                partCount += 9 + partLength
            End While

        Catch ex As Exception
            Throw
        End Try
        Return result
    End Function

    Public Function GetBytePacket() As BytePacket

        Dim result As BytePacket = Nothing

        Try
            Dim array = _parts.SelectMany(Function(x) x).ToArray()
            result = New BytePacket(array)
        Catch
        End Try

        Return result
    End Function

    Private Function GetMessageBytePartType(partType As Byte) As MessageBytePartType
        Dim result As MessageBytePartType = 0
        Select Case partType
            Case 0
                result = MessageBytePartType.ByteType
            Case 1
                result = MessageBytePartType.BooleanType
            Case 2
                result = MessageBytePartType.CharType
            Case 3
                result = MessageBytePartType.DateType
            Case 4
                result = MessageBytePartType.DoubleType
            Case 5
                result = MessageBytePartType.IntegerType
            Case 6
                result = MessageBytePartType.LongType
            Case 7
                result = MessageBytePartType.ShortType
            Case 8
                result = MessageBytePartType.SingleType
            Case 9
                result = MessageBytePartType.StringType
        End Select
        Return result
    End Function

    Private Function GetMessageBytePartType(partType As MessageBytePartType) As Byte
        Dim result As Byte = 0
        Select Case partType
            Case MessageBytePartType.ByteType
                result = 0
            Case MessageBytePartType.BooleanType
                result = 1
            Case MessageBytePartType.CharType
                result = 2
            Case MessageBytePartType.DateType
                result = 3
            Case MessageBytePartType.DoubleType
                result = 4
            Case MessageBytePartType.IntegerType
                result = 5
            Case MessageBytePartType.LongType
                result = 6
            Case MessageBytePartType.ShortType
                result = 7
            Case MessageBytePartType.SingleType
                result = 8
            Case MessageBytePartType.StringType
                result = 9
        End Select
        Return result
    End Function

    Private Function CheckPartCount(index As Integer) As Boolean
        Return Not Count <= index
    End Function

    Private Sub GetEnoughParts(index As Int32)
        While (Count <= index)
            _parts.Add(New Byte() {})
        End While
    End Sub


End Class