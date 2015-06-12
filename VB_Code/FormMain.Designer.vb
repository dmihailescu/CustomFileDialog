Imports Microsoft.VisualBasic
Imports System
Namespace CustomControls
	Partial Public Class FormMain
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
            Me._btnSelect = New System.Windows.Forms.Button()
            Me._FileSize = New System.Windows.Forms.Label()
            Me._btnSave = New System.Windows.Forms.Button()
            Me.lblFilePath = New System.Windows.Forms.Label()
            Me._btnAbout = New System.Windows.Forms.Button()
            Me._btnExtension = New System.Windows.Forms.Button()
            Me._btnSaveExt = New System.Windows.Forms.Button()
            Me.groupBox1 = New System.Windows.Forms.GroupBox()
            Me.groupBox2 = New System.Windows.Forms.GroupBox()
            Me._btnExit = New System.Windows.Forms.Button()
            Me.groupBox2.SuspendLayout()
            Me.SuspendLayout()
            '
            '_btnSelect
            '
            Me._btnSelect.Location = New System.Drawing.Point(68, 17)
            Me._btnSelect.Name = "_btnSelect"
            Me._btnSelect.Size = New System.Drawing.Size(75, 23)
            Me._btnSelect.TabIndex = 24
            Me._btnSelect.Text = "Se&lect"
            Me._btnSelect.UseVisualStyleBackColor = True
            '
            '_FileSize
            '
            Me._FileSize.AutoSize = True
            Me._FileSize.Location = New System.Drawing.Point(138, 446)
            Me._FileSize.Name = "_FileSize"
            Me._FileSize.Size = New System.Drawing.Size(16, 13)
            Me._FileSize.TabIndex = 31
            Me._FileSize.Text = "   "
            '
            '_btnSave
            '
            Me._btnSave.Location = New System.Drawing.Point(33, 9)
            Me._btnSave.Name = "_btnSave"
            Me._btnSave.Size = New System.Drawing.Size(75, 23)
            Me._btnSave.TabIndex = 24
            Me._btnSave.Text = "&Save"
            Me._btnSave.UseVisualStyleBackColor = True
            '
            'lblFilePath
            '
            Me.lblFilePath.AutoSize = True
            Me.lblFilePath.Location = New System.Drawing.Point(12, 121)
            Me.lblFilePath.Name = "lblFilePath"
            Me.lblFilePath.Size = New System.Drawing.Size(50, 13)
            Me.lblFilePath.TabIndex = 34
            Me.lblFilePath.Text = "File path:"
            '
            '_btnAbout
            '
            Me._btnAbout.Location = New System.Drawing.Point(311, 151)
            Me._btnAbout.Name = "_btnAbout"
            Me._btnAbout.Size = New System.Drawing.Size(75, 23)
            Me._btnAbout.TabIndex = 35
            Me._btnAbout.Text = "&About"
            Me._btnAbout.UseVisualStyleBackColor = True
            '
            '_btnExtension
            '
            Me._btnExtension.Location = New System.Drawing.Point(68, 60)
            Me._btnExtension.Name = "_btnExtension"
            Me._btnExtension.Size = New System.Drawing.Size(167, 23)
            Me._btnExtension.TabIndex = 24
            Me._btnExtension.Text = "Select (&Extension method)"
            Me._btnExtension.UseVisualStyleBackColor = True
            '
            '_btnSaveExt
            '
            Me._btnSaveExt.Location = New System.Drawing.Point(33, 52)
            Me._btnSaveExt.Name = "_btnSaveExt"
            Me._btnSaveExt.Size = New System.Drawing.Size(152, 23)
            Me._btnSaveExt.TabIndex = 24
            Me._btnSaveExt.Text = "S&ave(extension Method)"
            Me._btnSaveExt.UseVisualStyleBackColor = True
            '
            'groupBox1
            '
            Me.groupBox1.Location = New System.Drawing.Point(35, 8)
            Me.groupBox1.Name = "groupBox1"
            Me.groupBox1.Size = New System.Drawing.Size(200, 100)
            Me.groupBox1.TabIndex = 36
            Me.groupBox1.TabStop = False
            '
            'groupBox2
            '
            Me.groupBox2.Controls.Add(Me._btnSave)
            Me.groupBox2.Controls.Add(Me._btnSaveExt)
            Me.groupBox2.Location = New System.Drawing.Point(278, 8)
            Me.groupBox2.Name = "groupBox2"
            Me.groupBox2.Size = New System.Drawing.Size(200, 100)
            Me.groupBox2.TabIndex = 37
            Me.groupBox2.TabStop = False
            '
            '_btnExit
            '
            Me._btnExit.Location = New System.Drawing.Point(68, 151)
            Me._btnExit.Name = "_btnExit"
            Me._btnExit.Size = New System.Drawing.Size(75, 23)
            Me._btnExit.TabIndex = 38
            Me._btnExit.Text = "E&xit"
            Me._btnExit.UseVisualStyleBackColor = True
            '
            'FormMain
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(500, 186)
            Me.Controls.Add(Me._btnExit)
            Me.Controls.Add(Me._btnAbout)
            Me.Controls.Add(Me.lblFilePath)
            Me.Controls.Add(Me._FileSize)
            Me.Controls.Add(Me._btnExtension)
            Me.Controls.Add(Me._btnSelect)
            Me.Controls.Add(Me.groupBox1)
            Me.Controls.Add(Me.groupBox2)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "FormMain"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Click To Open a Dialog"
            Me.groupBox2.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

		#End Region

		Private WithEvents _btnSelect As System.Windows.Forms.Button
		Private _FileSize As System.Windows.Forms.Label
		Private WithEvents _btnSave As System.Windows.Forms.Button
		Private lblFilePath As System.Windows.Forms.Label
		Private WithEvents _btnAbout As System.Windows.Forms.Button
		Private WithEvents _btnExtension As System.Windows.Forms.Button
		Private WithEvents _btnSaveExt As System.Windows.Forms.Button
		Private groupBox1 As System.Windows.Forms.GroupBox
		Private groupBox2 As System.Windows.Forms.GroupBox
		Private WithEvents _btnExit As System.Windows.Forms.Button


	End Class
End Namespace

