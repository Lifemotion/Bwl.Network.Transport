Public Interface IConnectionControl
    Inherits IDisposable
    ReadOnly Property ID As Long
    Sub Open(address As String, options As String)
    Sub Close()
    ReadOnly Property IsConnected As Boolean
End Interface
