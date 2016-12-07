Public Module Tools
    Public Function PrepareData(mb As Single) As Byte()
        Dim rnd As New Random
        Dim buff(1024 * 1024 * mb - 1) As Byte
        For i = 0 To buff.Length - 1
            buff(i) = rnd.Next(0, 255)
        Next
        Return buff
    End Function

    Public Sub CompareArrays(Of T As IComparable)(array1 As T(), array2 As T())
        If array1.Length <> array2.Length Then Throw New Exception("Arrays lengths differs")
        For i = 0 To array1.Length - 1
            If array1(i).CompareTo(array2(i)) <> 0 Then Throw New Exception("Arrays differs at " + i.ToString)
        Next
    End Sub

    Public Delegate Sub TestFailSub()

    Public Sub TestException(mustExceptSub As TestFailSub, Optional message As String = "")
        Dim exception = False
        Try
            mustExceptSub()
        Catch ex As Exception
            exception = True
        End Try
        If Not exception Then Throw New Exception("Function didn't generate exception " + message)
    End Sub
End Module
