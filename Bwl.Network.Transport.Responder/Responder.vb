Module Responder
    Dim t1 As New UDPTransport

    Sub Main()
        t1.Open("20.20.25.80:3066:8066", "")
        Do
            Threading.Thread.Sleep(1)
        Loop
    End Sub

End Module
