Imports Microsoft.VisualBasic
Imports System.ComponentModel
Namespace CustomControls
	Partial Public Class MyOpenFileDialogControl
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
			Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(MyOpenFileDialogControl))
			Me.groupBox1 = New System.Windows.Forms.GroupBox()
			Me.pbxPreview = New System.Windows.Forms.PictureBox()
			Me.lblColors = New System.Windows.Forms.Label()
			Me.lblFormat = New System.Windows.Forms.Label()
			Me.lblSize = New System.Windows.Forms.Label()
			Me.lblSizeValue = New System.Windows.Forms.Label()
			Me.lblFormatValue = New System.Windows.Forms.Label()
			Me.lblColorsValue = New System.Windows.Forms.Label()
			Me.groupBox1.SuspendLayout()
			CType(Me.pbxPreview, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			' 
			' groupBox1
			' 
			Me.groupBox1.Controls.Add(Me.pbxPreview)
			Me.groupBox1.Location = New System.Drawing.Point(5, 3)
			Me.groupBox1.Name = "groupBox1"
			Me.groupBox1.Size = New System.Drawing.Size(260, 261)
			Me.groupBox1.TabIndex = 0
			Me.groupBox1.TabStop = False
			Me.groupBox1.Text = "Preview"
			' 
			' pbxPreview
			' 
			Me.pbxPreview.Dock = System.Windows.Forms.DockStyle.Fill
			Me.pbxPreview.Location = New System.Drawing.Point(3, 16)
			Me.pbxPreview.MaximumSize = New System.Drawing.Size(256, 256)
			Me.pbxPreview.Name = "pbxPreview"
			Me.pbxPreview.Size = New System.Drawing.Size(254, 242)
			Me.pbxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
			Me.pbxPreview.TabIndex = 4
			Me.pbxPreview.TabStop = False
			' 
			' lblColors
			' 
			Me.lblColors.Anchor = (CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles))
			Me.lblColors.Location = New System.Drawing.Point(5, 309)
			Me.lblColors.Name = "lblColors"
			Me.lblColors.Size = New System.Drawing.Size(42, 13)
			Me.lblColors.TabIndex = 3
			Me.lblColors.Text = "Colors:"
			' 
			' lblFormat
			' 
			Me.lblFormat.Anchor = (CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles))
			Me.lblFormat.Location = New System.Drawing.Point(5, 273)
			Me.lblFormat.Name = "lblFormat"
			Me.lblFormat.Size = New System.Drawing.Size(42, 13)
			Me.lblFormat.TabIndex = 4
			Me.lblFormat.Text = "Format:"
			' 
			' lblSize
			' 
			Me.lblSize.Anchor = (CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles))
			Me.lblSize.Location = New System.Drawing.Point(5, 291)
			Me.lblSize.Name = "lblSize"
			Me.lblSize.Size = New System.Drawing.Size(42, 13)
			Me.lblSize.TabIndex = 5
			Me.lblSize.Text = "Size:"
			' 
			' lblSizeValue
			' 
			Me.lblSizeValue.Anchor = (CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles))
			Me.lblSizeValue.Location = New System.Drawing.Point(53, 291)
			Me.lblSizeValue.Name = "lblSizeValue"
			Me.lblSizeValue.Size = New System.Drawing.Size(178, 13)
			Me.lblSizeValue.TabIndex = 8
			' 
			' lblFormatValue
			' 
			Me.lblFormatValue.Anchor = (CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles))
			Me.lblFormatValue.Location = New System.Drawing.Point(53, 273)
			Me.lblFormatValue.Name = "lblFormatValue"
			Me.lblFormatValue.Size = New System.Drawing.Size(178, 13)
			Me.lblFormatValue.TabIndex = 7
			' 
			' lblColorsValue
			' 
			Me.lblColorsValue.Anchor = (CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles))
			Me.lblColorsValue.Location = New System.Drawing.Point(53, 309)
			Me.lblColorsValue.Name = "lblColorsValue"
			Me.lblColorsValue.Size = New System.Drawing.Size(178, 13)
			Me.lblColorsValue.TabIndex = 6
			' 
			' MyOpenFileDialogControl
			' 
			Me.BackColor = System.Drawing.SystemColors.Control
			Me.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.Controls.Add(Me.lblSizeValue)
			Me.Controls.Add(Me.lblFormatValue)
			Me.Controls.Add(Me.lblColorsValue)
			Me.Controls.Add(Me.lblSize)
			Me.Controls.Add(Me.lblFormat)
			Me.Controls.Add(Me.groupBox1)
			Me.Controls.Add(Me.lblColors)
			Me.FileDlgCaption = "Select an Image"
			Me.FileDlgDefaultViewMode = Win32Types.FolderViewMode.Thumbnails
			Me.FileDlgEnableOkBtn = False
			Me.FileDlgFileName = "Select Picture"
			Me.FileDlgFilter = resources.GetString("$this.FileDlgFilter")
			Me.FileDlgFilterIndex = 2
			Me.FileDlgOkCaption = "Select"
			Me.ImeMode = System.Windows.Forms.ImeMode.NoControl
			Me.Name = "MyOpenFileDialogControl"
			Me.Size = New System.Drawing.Size(270, 342)
'			Me.EventFileNameChanged += New FileDialogExtenders.FileDialogControlBase.PathChangedEventHandler(Me.MyOpenFileDialogControl_FileNameChanged)
'			Me.EventFolderNameChanged += New FileDialogExtenders.FileDialogControlBase.PathChangedEventHandler(Me.MyOpenFileDialogControl_FolderNameChanged)
'			Me.EventClosingDialog += New System.ComponentModel.CancelEventHandler(Me.MyOpenFileDialogControl_ClosingDialog)
'			Me.HelpRequested += New System.Windows.Forms.HelpEventHandler(Me.MyOpenFileDialogControl_HelpRequested)
			Me.groupBox1.ResumeLayout(False)
			CType(Me.pbxPreview, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub

		#End Region

		Private groupBox1 As System.Windows.Forms.GroupBox
		Private lblColors As System.Windows.Forms.Label
		Private pbxPreview As System.Windows.Forms.PictureBox
		Private lblFormat As System.Windows.Forms.Label
		Private lblSize As System.Windows.Forms.Label
		Private lblSizeValue As System.Windows.Forms.Label
		Private lblFormatValue As System.Windows.Forms.Label
		Private lblColorsValue As System.Windows.Forms.Label
	End Class
End Namespace