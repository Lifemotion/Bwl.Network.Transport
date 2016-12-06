Friend Class CRC32
    Public Const NBits = 8
    Public Const BitLength = 32

    Public Shared Function Calculate(array As Byte()) As Byte()
        Return Calculate({array})
    End Function

    Public Shared Function Calculate(arrays As IEnumerable(Of Byte())) As Byte()
        Dim i As Integer, j As Integer
        Dim b As UInteger, crc32 As UInteger, mask As UInteger

        crc32 = &HFFFFFFFFUI
        For Each array In arrays
            For i = 0 To array.Length - 1
                b = array(i)
                crc32 = crc32 Xor b
                For j = 7 To 0 Step -1
                    mask = If(crc32 And 1, &HFFFFFFFFUI, 0)
                    crc32 = (crc32 >> 1) Xor (&HEDB88320UI And mask)
                Next
            Next
        Next

        crc32 = Not crc32

        Dim crc32b = New Byte((BitLength / NBits) - 1) {}
        crc32b(0) = CByte((crc32 And &HFF000000UI) >> 24)
        crc32b(1) = CByte((crc32 And &HFF0000) >> 16)
        crc32b(2) = CByte((crc32 And &HFF00) >> 8)
        crc32b(3) = CByte((crc32 And &HFF) >> 0)

        Return crc32b
    End Function
End Class
