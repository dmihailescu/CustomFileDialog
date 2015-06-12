'
' * Please leave this Copyright notice in your code if you use it
' * Written by Decebal Mihailescu [http://www.codeproject.com/script/articles/list_articles.asp?userid=634640]
' 

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Reflection

Namespace AboutUtil
	''' <summary>
	''' Summary description for About.
	''' </summary>
	Public Class About
		Inherits System.Windows.Forms.Form
		Private WithEvents _linkLabelAbout As System.Windows.Forms.LinkLabel
		Private button1 As System.Windows.Forms.Button
		Private textBox1 As System.Windows.Forms.TextBox
		Private label1 As System.Windows.Forms.Label
		Private WithEvents _lnksecond As System.Windows.Forms.LinkLabel
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.Container = Nothing

		Public Sub New(ByVal owner As System.Windows.Forms.Form)
			Me.Owner = owner
			'
			' Required for Windows Form Designer support
			'
			InitializeComponent()

			'
			' TODO: Add any constructor code after InitializeComponent call
			'
		End Sub

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If components IsNot Nothing Then
					components.Dispose()
				End If
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"
		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(About))
            Me._linkLabelAbout = New System.Windows.Forms.LinkLabel
            Me.button1 = New System.Windows.Forms.Button
            Me.textBox1 = New System.Windows.Forms.TextBox
            Me.label1 = New System.Windows.Forms.Label
            Me._lnksecond = New System.Windows.Forms.LinkLabel
            Me.SuspendLayout()
            '
            '_linkLabelAbout
            '
            Me._linkLabelAbout.LinkArea = New System.Windows.Forms.LinkArea(36, 0)
            Me._linkLabelAbout.Location = New System.Drawing.Point(12, 233)
            Me._linkLabelAbout.Name = "_linkLabelAbout"
            Me._linkLabelAbout.Size = New System.Drawing.Size(249, 24)
            Me._linkLabelAbout.TabIndex = 0
            Me._linkLabelAbout.Text = "Copyright © 2007-2012  Decebal Mihailescu"
            Me._linkLabelAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me._linkLabelAbout.UseCompatibleTextRendering = True
            '
            'button1
            '
            Me.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.button1.Location = New System.Drawing.Point(214, 269)
            Me.button1.Name = "button1"
            Me.button1.Size = New System.Drawing.Size(75, 23)
            Me.button1.TabIndex = 1
            Me.button1.Text = "&OK"
            Me.button1.UseVisualStyleBackColor = True
            '
            'textBox1
            '
            Me.textBox1.Location = New System.Drawing.Point(12, 57)
            Me.textBox1.Multiline = True
            Me.textBox1.Name = "textBox1"
            Me.textBox1.ReadOnly = True
            Me.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
            Me.textBox1.Size = New System.Drawing.Size(477, 134)
            Me.textBox1.TabIndex = 2
            Me.textBox1.Text = resources.GetString("textBox1.Text")
            '
            'label1
            '
            Me.label1.AutoSize = True
            Me.label1.Location = New System.Drawing.Point(12, 9)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(93, 13)
            Me.label1.TabIndex = 3
            Me.label1.Text = "text from reflection"
            '
            '_lnksecond
            '
            Me._lnksecond.LinkArea = New System.Windows.Forms.LinkArea(13, 9)
            Me._lnksecond.Location = New System.Drawing.Point(12, 194)
            Me._lnksecond.Name = "_lnksecond"
            Me._lnksecond.Size = New System.Drawing.Size(249, 24)
            Me._lnksecond.TabIndex = 0
            Me._lnksecond.TabStop = True
            Me._lnksecond.Text = "Copyright ©  CastorTiu 2006"
            Me._lnksecond.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me._lnksecond.UseCompatibleTextRendering = True
            '
            'About
            '
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(501, 295)
            Me.ControlBox = False
            Me.Controls.Add(Me.label1)
            Me.Controls.Add(Me.textBox1)
            Me.Controls.Add(Me.button1)
            Me.Controls.Add(Me._lnksecond)
            Me.Controls.Add(Me._linkLabelAbout)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "About"
            Me.ShowInTaskbar = False
            Me.Text = "About"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
		#End Region

		Private Sub linkLabelAbout_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles _linkLabelAbout.LinkClicked, _lnksecond.LinkClicked
			Dim llbl As System.Windows.Forms.LinkLabel = TryCast(sender, System.Windows.Forms.LinkLabel)
			If llbl IsNot Nothing Then
				llbl.Links(llbl.Links.IndexOf(e.Link)).Visited = True
				Dim target As String = TryCast(e.Link.LinkData, String)
				If target IsNot Nothing AndAlso target.Length > 0 Then
					System.Diagnostics.Process.Start(target)
				End If
			End If
		End Sub

		Private Sub About_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load


			Dim crt As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
			Dim attr() As Object = crt.GetCustomAttributes(GetType(AssemblyDescriptionAttribute), True)
			Dim desc As AssemblyDescriptionAttribute = TryCast(attr(0), AssemblyDescriptionAttribute)
			attr = crt.GetCustomAttributes(GetType(AssemblyFileVersionAttribute), True)
			Dim ver As AssemblyFileVersionAttribute = TryCast(attr(0), AssemblyFileVersionAttribute)
			Dim v As Version = crt.GetName().Version
			label1.Text = String.Format("{0}" & Constants.vbLf & "File Version: {1}" & Constants.vbLf & "Assembly Version: {2}", desc.Description, ver.Version, v.ToString())
			attr = crt.GetCustomAttributes(GetType(AssemblyTitleAttribute), True)
			Dim ta As AssemblyTitleAttribute = TryCast(attr(0), AssemblyTitleAttribute)
			Text = String.Format("About  {0} {1}.{2}", ta.Title, v.Major, v.Minor) 'Owner.Text

			attr = crt.GetCustomAttributes(GetType(AssemblyCopyrightAttribute), True)
			Dim copyright As AssemblyCopyrightAttribute = TryCast(attr(0), AssemblyCopyrightAttribute)
			Dim strcpy As String = copyright.Copyright

			_linkLabelAbout.Text = strcpy
			attr = crt.GetCustomAttributes(GetType(AssemblyCompanyAttribute), True)
			Dim company As AssemblyCompanyAttribute = TryCast(attr(0), AssemblyCompanyAttribute)
			' look for the name in copyright string to activate if found
			Dim start As Integer = strcpy.IndexOf(company.Company, 0, StringComparison.InvariantCultureIgnoreCase)
			If start <> -1 Then
				Me._linkLabelAbout.LinkArea = New System.Windows.Forms.LinkArea(start, company.Company.Length)
			Else
				Dim size As Integer = "Copyright © ".Length
				start = strcpy.IndexOf("Copyright ©", 0, StringComparison.InvariantCultureIgnoreCase)
				If start <> -1 Then
					Me._linkLabelAbout.LinkArea = New System.Windows.Forms.LinkArea(size, strcpy.Length - size)
				Else
					Me._linkLabelAbout.LinkArea = New System.Windows.Forms.LinkArea(0, strcpy.Length)
				End If
			End If
            If (Not _linkLabelAbout.LinkArea.IsEmpty) Then
                Me._linkLabelAbout.Links(0).LinkData = "www.codeproject.com/script/Articles/list_articles.asp?userid=634640"
            End If

			_lnksecond.Links(0).LinkData = "http://www.codeproject.com/script/Articles/list_articles.asp?userid=240897"

		End Sub
	End Class
End Namespace
