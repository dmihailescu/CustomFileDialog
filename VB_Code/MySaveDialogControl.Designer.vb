Imports Microsoft.VisualBasic
Imports System.ComponentModel
Namespace CustomControls
	Partial Public Class MySaveDialogControl
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing



		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(MySaveDialogControl))
			Me._pbChanged = New System.Windows.Forms.PictureBox()
			Me._pbOriginal = New System.Windows.Forms.PictureBox()
			Me._lblOriginalFileLen = New System.Windows.Forms.Label()
			Me._lblOrigInfo = New System.Windows.Forms.Label()
			Me._groupBox1 = New System.Windows.Forms.GroupBox()
			Me._label4 = New System.Windows.Forms.Label()
			Me._cbxNewViewMode = New System.Windows.Forms.ComboBox()
			Me._ckbPreserveaspect = New System.Windows.Forms.CheckBox()
			Me._horizontal = New System.Windows.Forms.NumericUpDown()
			Me._label1 = New System.Windows.Forms.Label()
			Me._rotateflip = New System.Windows.Forms.ComboBox()
			Me._FileSize = New System.Windows.Forms.Label()
			Me._vertical = New System.Windows.Forms.NumericUpDown()
			Me._label2 = New System.Windows.Forms.Label()
			Me._groupBox2 = New System.Windows.Forms.GroupBox()
			Me._label5 = New System.Windows.Forms.Label()
			Me._cbxOriginalMode = New System.Windows.Forms.ComboBox()
			Me._rotateFlipOriginal = New System.Windows.Forms.ComboBox()
			CType(Me._pbChanged, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me._pbOriginal, System.ComponentModel.ISupportInitialize).BeginInit()
			Me._groupBox1.SuspendLayout()
			CType(Me._horizontal, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me._vertical, System.ComponentModel.ISupportInitialize).BeginInit()
			Me._groupBox2.SuspendLayout()
			Me.SuspendLayout()
			' 
			' _pbChanged
			' 
			Me._pbChanged.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			Me._pbChanged.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			Me._pbChanged.Location = New System.Drawing.Point(6, 43)
			Me._pbChanged.MaximumSize = New System.Drawing.Size(256, 256)
			Me._pbChanged.MinimumSize = New System.Drawing.Size(1, 1)
			Me._pbChanged.Name = "_pbChanged"
			Me._pbChanged.Size = New System.Drawing.Size(256, 256)
			Me._pbChanged.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
			Me._pbChanged.TabIndex = 30
			Me._pbChanged.TabStop = False
			Me._pbChanged.WaitOnLoad = True
			' 
			' _pbOriginal
			' 
			Me._pbOriginal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			Me._pbOriginal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			Me._pbOriginal.Location = New System.Drawing.Point(6, 43)
			Me._pbOriginal.MaximumSize = New System.Drawing.Size(256, 256)
			Me._pbOriginal.MinimumSize = New System.Drawing.Size(1, 1)
			Me._pbOriginal.Name = "_pbOriginal"
			Me._pbOriginal.Size = New System.Drawing.Size(256, 256)
			Me._pbOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
			Me._pbOriginal.TabIndex = 30
			Me._pbOriginal.TabStop = False
			Me._pbOriginal.WaitOnLoad = True
			' 
			' _lblOriginalFileLen
			' 
			Me._lblOriginalFileLen.AutoSize = True
			Me._lblOriginalFileLen.Location = New System.Drawing.Point(6, 352)
			Me._lblOriginalFileLen.Name = "_lblOriginalFileLen"
			Me._lblOriginalFileLen.Size = New System.Drawing.Size(40, 13)
			Me._lblOriginalFileLen.TabIndex = 1
			Me._lblOriginalFileLen.Text = "No File"
			' 
			' _lblOrigInfo
			' 
			Me._lblOrigInfo.AutoSize = True
			Me._lblOrigInfo.Location = New System.Drawing.Point(6, 383)
			Me._lblOrigInfo.Name = "_lblOrigInfo"
			Me._lblOrigInfo.Size = New System.Drawing.Size(94, 13)
			Me._lblOrigInfo.TabIndex = 2
			Me._lblOrigInfo.Text = "Original Image info"
			' 
			' _groupBox1
			' 
			Me._groupBox1.Controls.Add(Me._label4)
			Me._groupBox1.Controls.Add(Me._cbxNewViewMode)
			Me._groupBox1.Controls.Add(Me._ckbPreserveaspect)
			Me._groupBox1.Controls.Add(Me._pbChanged)
			Me._groupBox1.Controls.Add(Me._horizontal)
			Me._groupBox1.Controls.Add(Me._label1)
			Me._groupBox1.Controls.Add(Me._rotateflip)
			Me._groupBox1.Controls.Add(Me._FileSize)
			Me._groupBox1.Controls.Add(Me._vertical)
			Me._groupBox1.Controls.Add(Me._label2)
			Me._groupBox1.Location = New System.Drawing.Point(278, 6)
			Me._groupBox1.Name = "_groupBox1"
			Me._groupBox1.Size = New System.Drawing.Size(268, 441)
			Me._groupBox1.TabIndex = 1
			Me._groupBox1.TabStop = False
			' 
			' _label4
			' 
			Me._label4.AutoSize = True
			Me._label4.Location = New System.Drawing.Point(12, 22)
			Me._label4.Name = "_label4"
			Me._label4.Size = New System.Drawing.Size(57, 13)
			Me._label4.TabIndex = 33
			Me._label4.Text = "ViewMode"
			' 
			' _cbxNewViewMode
			' 
			Me._cbxNewViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._cbxNewViewMode.FormattingEnabled = True
			Me._cbxNewViewMode.Location = New System.Drawing.Point(97, 19)
			Me._cbxNewViewMode.Name = "_cbxNewViewMode"
			Me._cbxNewViewMode.Size = New System.Drawing.Size(149, 21)
			Me._cbxNewViewMode.TabIndex = 32
'			Me._cbxNewViewMode.SelectedIndexChanged += New System.EventHandler(Me.cbxNewViewMode_SelectedIndexChanged)
			' 
			' _ckbPreserveaspect
			' 
			Me._ckbPreserveaspect.AutoSize = True
			Me._ckbPreserveaspect.Location = New System.Drawing.Point(128, 309)
			Me._ckbPreserveaspect.Name = "_ckbPreserveaspect"
			Me._ckbPreserveaspect.Size = New System.Drawing.Size(104, 17)
			Me._ckbPreserveaspect.TabIndex = 31
			Me._ckbPreserveaspect.Text = "Preserve Aspect"
			Me._ckbPreserveaspect.UseVisualStyleBackColor = True
'			Me._ckbPreserveaspect.CheckedChanged += New System.EventHandler(Me.ckbPreserveaspect_CheckedChanged)
			' 
			' _horizontal
			' 
			Me._horizontal.Location = New System.Drawing.Point(128, 337)
			Me._horizontal.Minimum = New Decimal(New Integer() { 1, 0, 0, 0})
			Me._horizontal.Name = "_horizontal"
			Me._horizontal.Size = New System.Drawing.Size(59, 20)
			Me._horizontal.TabIndex = 4
			Me._horizontal.Value = New Decimal(New Integer() { 100, 0, 0, 0})
'			Me._horizontal.ValueChanged += New System.EventHandler(Me._horizontal_ValueChanged)
'			Me._horizontal.MouseDown += New System.Windows.Forms.MouseEventHandler(Me.UPDOWN_MouseDown)
'			Me._horizontal.MouseUp += New System.Windows.Forms.MouseEventHandler(Me.UPDOWN_MouseUp)
			' 
			' _label1
			' 
			Me._label1.AutoSize = True
			Me._label1.Location = New System.Drawing.Point(21, 344)
			Me._label1.Name = "_label1"
			Me._label1.Size = New System.Drawing.Size(84, 13)
			Me._label1.TabIndex = 3
			Me._label1.Text = "horizontal pixels:"
			' 
			' _rotateflip
			' 
			Me._rotateflip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._rotateflip.FormattingEnabled = True
			Me._rotateflip.Location = New System.Drawing.Point(0, 305)
			Me._rotateflip.Name = "_rotateflip"
			Me._rotateflip.Size = New System.Drawing.Size(121, 21)
			Me._rotateflip.TabIndex = 2
'			Me._rotateflip.SelectedIndexChanged += New System.EventHandler(Me._rotateflip_SelectedIndexChanged)
			' 
			' _FileSize
			' 
			Me._FileSize.AutoSize = True
			Me._FileSize.Location = New System.Drawing.Point(2, 398)
			Me._FileSize.Name = "_FileSize"
			Me._FileSize.Size = New System.Drawing.Size(33, 13)
			Me._FileSize.TabIndex = 7
			Me._FileSize.Text = "None"
			' 
			' _vertical
			' 
			Me._vertical.Location = New System.Drawing.Point(128, 363)
			Me._vertical.Minimum = New Decimal(New Integer() { 1, 0, 0, 0})
			Me._vertical.Name = "_vertical"
			Me._vertical.Size = New System.Drawing.Size(59, 20)
			Me._vertical.TabIndex = 6
			Me._vertical.Value = New Decimal(New Integer() { 100, 0, 0, 0})
'			Me._vertical.ValueChanged += New System.EventHandler(Me._vertical_ValueChanged)
'			Me._vertical.MouseDown += New System.Windows.Forms.MouseEventHandler(Me.UPDOWN_MouseDown)
'			Me._vertical.MouseUp += New System.Windows.Forms.MouseEventHandler(Me.UPDOWN_MouseUp)
			' 
			' _label2
			' 
			Me._label2.AutoSize = True
			Me._label2.Location = New System.Drawing.Point(32, 370)
			Me._label2.Name = "_label2"
			Me._label2.Size = New System.Drawing.Size(73, 13)
			Me._label2.TabIndex = 5
			Me._label2.Text = "vertical pixels:"
			' 
			' _groupBox2
			' 
			Me._groupBox2.Controls.Add(Me._label5)
			Me._groupBox2.Controls.Add(Me._cbxOriginalMode)
			Me._groupBox2.Controls.Add(Me._pbOriginal)
			Me._groupBox2.Controls.Add(Me._lblOriginalFileLen)
			Me._groupBox2.Controls.Add(Me._lblOrigInfo)
			Me._groupBox2.Controls.Add(Me._rotateFlipOriginal)
			Me._groupBox2.Location = New System.Drawing.Point(3, 6)
			Me._groupBox2.Name = "_groupBox2"
			Me._groupBox2.Size = New System.Drawing.Size(269, 441)
			Me._groupBox2.TabIndex = 0
			Me._groupBox2.TabStop = False
			' 
			' _label5
			' 
			Me._label5.AutoSize = True
			Me._label5.Location = New System.Drawing.Point(3, 19)
			Me._label5.Name = "_label5"
			Me._label5.Size = New System.Drawing.Size(57, 13)
			Me._label5.TabIndex = 33
			Me._label5.Text = "ViewMode"
			' 
			' _cbxOriginalMode
			' 
			Me._cbxOriginalMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._cbxOriginalMode.FormattingEnabled = True
			Me._cbxOriginalMode.Location = New System.Drawing.Point(88, 16)
			Me._cbxOriginalMode.Name = "_cbxOriginalMode"
			Me._cbxOriginalMode.Size = New System.Drawing.Size(149, 21)
			Me._cbxOriginalMode.TabIndex = 32
'			Me._cbxOriginalMode.SelectedIndexChanged += New System.EventHandler(Me.cbxNewViewMode_SelectedIndexChanged)
			' 
			' _rotateFlipOriginal
			' 
			Me._rotateFlipOriginal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._rotateFlipOriginal.FormattingEnabled = True
			Me._rotateFlipOriginal.Location = New System.Drawing.Point(6, 305)
			Me._rotateFlipOriginal.Name = "_rotateFlipOriginal"
			Me._rotateFlipOriginal.Size = New System.Drawing.Size(121, 21)
			Me._rotateFlipOriginal.TabIndex = 2
'			Me._rotateFlipOriginal.SelectedIndexChanged += New System.EventHandler(Me._rotateflip_SelectedIndexChanged)
			' 
			' MySaveDialogControl
			' 
			Me.BackColor = System.Drawing.SystemColors.Control
			Me.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.Controls.Add(Me._groupBox2)
			Me.Controls.Add(Me._groupBox1)
			Me.FileDlgCaption = "Save Thumbnail as:"
			Me.FileDlgCheckFileExists = False
			Me.FileDlgDefaultViewMode = Win32Types.FolderViewMode.List
			Me.FileDlgFileName = "Change Picture"
			Me.FileDlgFilter = resources.GetString("$this.FileDlgFilter")
			Me.FileDlgOkCaption = "Create"
			Me.FileDlgStartLocation = FileDialogExtenders.AddonWindowLocation.Bottom
			Me.FileDlgType = Win32Types.FileDialogType.SaveFileDlg
			Me.Name = "MySaveDialogControl"
			Me.Size = New System.Drawing.Size(548, 446)
'			Me.Load += New System.EventHandler(Me.MySaveDialogControl_Load)
'			Me.EventClosingDialog += New System.ComponentModel.CancelEventHandler(Me.MySaveDialogControl_ClosingDialog)
'			Me.HelpRequested += New System.Windows.Forms.HelpEventHandler(Me.MySaveDialogControl_HelpRequested)
'			Me.EventFilterChanged += New FileDialogExtenders.FileDialogControlBase.FilterChangedEventHandler(Me.MySaveDialogControl_FilterChanged)
			CType(Me._pbChanged, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me._pbOriginal, System.ComponentModel.ISupportInitialize).EndInit()
			Me._groupBox1.ResumeLayout(False)
			Me._groupBox1.PerformLayout()
			CType(Me._horizontal, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me._vertical, System.ComponentModel.ISupportInitialize).EndInit()
			Me._groupBox2.ResumeLayout(False)
			Me._groupBox2.PerformLayout()
			Me.ResumeLayout(False)

		End Sub

		#End Region

		Private _pbChanged As System.Windows.Forms.PictureBox
		Private _pbOriginal As System.Windows.Forms.PictureBox
		Private _lblOriginalFileLen As System.Windows.Forms.Label
		Private _lblOrigInfo As System.Windows.Forms.Label
		Private _groupBox1 As System.Windows.Forms.GroupBox
		Private _FileSize As System.Windows.Forms.Label
		Private _label2 As System.Windows.Forms.Label
		Private WithEvents _vertical As System.Windows.Forms.NumericUpDown
		Private _label1 As System.Windows.Forms.Label
		Private WithEvents _horizontal As System.Windows.Forms.NumericUpDown
		Private WithEvents _rotateflip As System.Windows.Forms.ComboBox
		Private _groupBox2 As System.Windows.Forms.GroupBox
		Private WithEvents _ckbPreserveaspect As System.Windows.Forms.CheckBox
		Private WithEvents _cbxNewViewMode As System.Windows.Forms.ComboBox
		Private _label4 As System.Windows.Forms.Label
		Private _label5 As System.Windows.Forms.Label
		Private WithEvents _cbxOriginalMode As System.Windows.Forms.ComboBox
		Private WithEvents _rotateFlipOriginal As System.Windows.Forms.ComboBox
	End Class
End Namespace
