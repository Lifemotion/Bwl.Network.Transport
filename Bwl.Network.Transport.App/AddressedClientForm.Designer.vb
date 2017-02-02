<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddressedClientForm

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cbIsConnected = New System.Windows.Forms.CheckBox()
        Me.bClose = New System.Windows.Forms.Button()
        Me.SettingField5 = New Bwl.Framework.SettingField()
        Me.SettingField3 = New Bwl.Framework.SettingField()
        Me.SettingField4 = New Bwl.Framework.SettingField()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbAddressTo = New System.Windows.Forms.TextBox()
        Me.bSend = New System.Windows.Forms.Button()
        Me.tbMessage = New System.Windows.Forms.TextBox()
        Me.SettingField2 = New Bwl.Framework.SettingField()
        Me.bClient = New System.Windows.Forms.Button()
        Me.SettingField1 = New Bwl.Framework.SettingField()
        Me.SuspendLayout()
        '
        'logWriter
        '
        Me.logWriter.ExtendedView = False
        Me.logWriter.Location = New System.Drawing.Point(0, 328)
        Me.logWriter.Size = New System.Drawing.Size(828, 322)
        '
        'cbIsConnected
        '
        Me.cbIsConnected.AutoSize = True
        Me.cbIsConnected.Enabled = False
        Me.cbIsConnected.Location = New System.Drawing.Point(5, 289)
        Me.cbIsConnected.Name = "cbIsConnected"
        Me.cbIsConnected.Size = New System.Drawing.Size(86, 17)
        Me.cbIsConnected.TabIndex = 47
        Me.cbIsConnected.Text = "IsConnected"
        Me.cbIsConnected.UseVisualStyleBackColor = True
        '
        'bClose
        '
        Me.bClose.Location = New System.Drawing.Point(118, 260)
        Me.bClose.Name = "bClose"
        Me.bClose.Size = New System.Drawing.Size(95, 23)
        Me.bClose.TabIndex = 45
        Me.bClose.Text = "Отключить"
        Me.bClose.UseVisualStyleBackColor = True
        '
        'SettingField5
        '
        Me.SettingField5.AssignedSetting = Nothing
        Me.SettingField5.DesignText = Nothing
        Me.SettingField5.Location = New System.Drawing.Point(0, 211)
        Me.SettingField5.Name = "SettingField5"
        Me.SettingField5.Size = New System.Drawing.Size(218, 43)
        Me.SettingField5.TabIndex = 44
        '
        'SettingField3
        '
        Me.SettingField3.AssignedSetting = Nothing
        Me.SettingField3.DesignText = Nothing
        Me.SettingField3.Location = New System.Drawing.Point(0, 119)
        Me.SettingField3.Name = "SettingField3"
        Me.SettingField3.Size = New System.Drawing.Size(218, 43)
        Me.SettingField3.TabIndex = 43
        '
        'SettingField4
        '
        Me.SettingField4.AssignedSetting = Nothing
        Me.SettingField4.DesignText = Nothing
        Me.SettingField4.Location = New System.Drawing.Point(0, 165)
        Me.SettingField4.Name = "SettingField4"
        Me.SettingField4.Size = New System.Drawing.Size(218, 43)
        Me.SettingField4.TabIndex = 42
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(239, 26)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 13)
        Me.Label3.TabIndex = 41
        Me.Label3.Text = "IdTo"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(236, 72)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 13)
        Me.Label1.TabIndex = 40
        Me.Label1.Text = "Message"
        '
        'tbAddressTo
        '
        Me.tbAddressTo.Location = New System.Drawing.Point(239, 44)
        Me.tbAddressTo.Name = "tbAddressTo"
        Me.tbAddressTo.Size = New System.Drawing.Size(104, 20)
        Me.tbAddressTo.TabIndex = 39
        '
        'bSend
        '
        Me.bSend.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bSend.Location = New System.Drawing.Point(721, 260)
        Me.bSend.Name = "bSend"
        Me.bSend.Size = New System.Drawing.Size(95, 23)
        Me.bSend.TabIndex = 38
        Me.bSend.Text = "Отправить"
        Me.bSend.UseVisualStyleBackColor = True
        '
        'tbMessage
        '
        Me.tbMessage.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbMessage.Location = New System.Drawing.Point(239, 91)
        Me.tbMessage.Multiline = True
        Me.tbMessage.Name = "tbMessage"
        Me.tbMessage.Size = New System.Drawing.Size(577, 158)
        Me.tbMessage.TabIndex = 37
        '
        'SettingField2
        '
        Me.SettingField2.AssignedSetting = Nothing
        Me.SettingField2.DesignText = Nothing
        Me.SettingField2.Location = New System.Drawing.Point(0, 73)
        Me.SettingField2.Name = "SettingField2"
        Me.SettingField2.Size = New System.Drawing.Size(218, 43)
        Me.SettingField2.TabIndex = 36
        '
        'bClient
        '
        Me.bClient.Location = New System.Drawing.Point(5, 260)
        Me.bClient.Name = "bClient"
        Me.bClient.Size = New System.Drawing.Size(95, 23)
        Me.bClient.TabIndex = 34
        Me.bClient.Text = "Подключить"
        Me.bClient.UseVisualStyleBackColor = True
        '
        'SettingField1
        '
        Me.SettingField1.AssignedSetting = Nothing
        Me.SettingField1.DesignText = Nothing
        Me.SettingField1.Location = New System.Drawing.Point(0, 27)
        Me.SettingField1.Name = "SettingField1"
        Me.SettingField1.Size = New System.Drawing.Size(218, 43)
        Me.SettingField1.TabIndex = 35
        '
        'AddressedClientForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(828, 648)
        Me.Controls.Add(Me.cbIsConnected)
        Me.Controls.Add(Me.bClose)
        Me.Controls.Add(Me.SettingField5)
        Me.Controls.Add(Me.SettingField3)
        Me.Controls.Add(Me.SettingField4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tbAddressTo)
        Me.Controls.Add(Me.bSend)
        Me.Controls.Add(Me.tbMessage)
        Me.Controls.Add(Me.SettingField2)
        Me.Controls.Add(Me.bClient)
        Me.Controls.Add(Me.SettingField1)
        Me.Name = "AddressedClientForm"
        Me.Text = "AddressedClientForm"
        Me.Controls.SetChildIndex(Me.logWriter, 0)
        Me.Controls.SetChildIndex(Me.SettingField1, 0)
        Me.Controls.SetChildIndex(Me.bClient, 0)
        Me.Controls.SetChildIndex(Me.SettingField2, 0)
        Me.Controls.SetChildIndex(Me.tbMessage, 0)
        Me.Controls.SetChildIndex(Me.bSend, 0)
        Me.Controls.SetChildIndex(Me.tbAddressTo, 0)
        Me.Controls.SetChildIndex(Me.Label1, 0)
        Me.Controls.SetChildIndex(Me.Label3, 0)
        Me.Controls.SetChildIndex(Me.SettingField4, 0)
        Me.Controls.SetChildIndex(Me.SettingField3, 0)
        Me.Controls.SetChildIndex(Me.SettingField5, 0)
        Me.Controls.SetChildIndex(Me.bClose, 0)
        Me.Controls.SetChildIndex(Me.cbIsConnected, 0)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cbIsConnected As CheckBox
    Friend WithEvents bClose As Button
    Friend WithEvents SettingField5 As Framework.SettingField
    Friend WithEvents SettingField3 As Framework.SettingField
    Friend WithEvents SettingField4 As Framework.SettingField
    Friend WithEvents Label3 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents tbAddressTo As TextBox
    Friend WithEvents bSend As Button
    Friend WithEvents tbMessage As TextBox
    Friend WithEvents SettingField2 As Framework.SettingField
    Friend WithEvents bClient As Button
    Friend WithEvents SettingField1 As Framework.SettingField
End Class
