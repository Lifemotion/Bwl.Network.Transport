Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
<TestClass()> Public Class StructuredPacketTests

    <TestMethod()> Public Sub SP_CrcCheck()
        Dim pkt As New StructuredPacket

        Dim int1 As Integer = 10
        Dim bytes As Byte() = {1, 2, 3, 4, 5}
        pkt.Add("Bytes", bytes)
        pkt.EnableCRC = True
        Dim coded = pkt.ToBytePacket
        coded.Bytes(coded.Bytes.Length - 1) = coded.Bytes(coded.Bytes.Length - 1) Xor 170
        TestException(Sub()
                          Dim decoded As New StructuredPacket(coded)
                      End Sub, "CRC error must be found")
        Assert.AreEqual(True, pkt.EnableCRC)
    End Sub

    <TestMethod()> Public Sub SP_CodeDecode()
        Dim pkt As New StructuredPacket

        Dim int1 As Integer = 10
        Dim long1 As Long = -645645645645645747
        Dim sng1 As Single = 55646.5
        Dim dbl1 As Double = -55435646.5
        Dim str1 As String = "CatsCatsCats" + vbCrLf + "rerwrwer"
        Dim bool1 As Boolean = True
        Dim datetime1 As DateTime = Now.ToLocalTime
        Dim datetime2 As DateTime = Now.ToUniversalTime
        Dim bytes = PrepareData(0.1)

        pkt.Add("Int1", int1)
        pkt.Add("Long1", long1)
        pkt.Add("Sng1", sng1)
        pkt.Add("Dbl1", dbl1)
        pkt.Add("String1", str1)
        pkt.Add("Bool1", bool1)
        pkt.Add("Date1", datetime1)
        pkt.Add("Date2", datetime2)
        pkt.Add("Bytes", bytes)
        pkt.AddressFrom = "Addr1"
        pkt.AddressTo = "Addr2"
        pkt.ServiceID = "ServiceId1"
        pkt.EnableCRC = True
        Dim coded = pkt.ToBytePacket
        Dim decoded As New StructuredPacket(coded)
        Assert.AreEqual(pkt.AddressFrom, decoded.AddressFrom)
        Assert.AreEqual(pkt.AddressTo, decoded.AddressTo)
        Assert.AreEqual(pkt.ServiceID, decoded.ServiceID)
        Assert.AreEqual(decoded.Parts("Int1"), int1)
        Assert.AreEqual(decoded.Parts("Long1"), long1)
        Assert.AreEqual(decoded.Parts("Sng1"), sng1)
        Assert.AreEqual(decoded.Parts("Dbl1"), dbl1)
        Assert.AreEqual(decoded.Parts("String1"), str1)
        Assert.AreEqual(decoded.Parts("Bool1"), bool1)
        Assert.AreEqual(decoded.Parts("Date1"), datetime1)
        Assert.AreEqual(decoded.Parts("Date2"), datetime2)
        Assert.IsInstanceOfType(decoded.Parts("Int1"), GetType(Integer))
        Assert.IsInstanceOfType(decoded.Parts("Long1"), GetType(Long))
        Assert.IsInstanceOfType(decoded.Parts("Sng1"), GetType(Single))
        Assert.IsInstanceOfType(decoded.Parts("Dbl1"), GetType(Double))
        Assert.IsInstanceOfType(decoded.Parts("String1"), GetType(String))
        Assert.IsInstanceOfType(decoded.Parts("Date1"), GetType(DateTime))
        Assert.IsInstanceOfType(decoded.Parts("Bytes"), GetType(Byte()))
        Assert.AreEqual(True, decoded.EnableCRC)
        CompareArrays(Of Byte)(bytes, decoded.Parts("Bytes"))
        ' Assert.AreSame()
    End Sub

    <TestMethod()> Public Sub SP_CodeDecodeEmpty()
        Dim pkt As New StructuredPacket
        Dim coded = pkt.ToBytePacket
        Dim decoded As New StructuredPacket(coded)
        If decoded.Parts.Count <> 0 Then Throw New Exception
    End Sub


    <TestMethod()> Public Sub SP_Keys()
        Dim pkt As New StructuredPacket
        TestException(Sub()
                          pkt.Add("Key1", 1)
                          pkt.Add("Key1", 1)
                      End Sub, "Same keys must not be added")
        TestException(Sub()
                          pkt.Add("", 1)
                      End Sub, "Empty keys must not be added")
        TestException(Sub()
                          pkt.Add(Nothing, 1)
                      End Sub, "Null keys must not be added")
    End Sub


End Class