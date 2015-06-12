'  Copyright (c) 2006, Gustavo Franco
'  Copyright © Decebal Mihailescu 2007-2015

'  Email:  gustavo_franco@hotmail.com
'  All rights reserved.

'  Redistribution and use in source and binary forms, with or without modification, 
'  are permitted provided that the following conditions are met:

'  Redistributions of source code must retain the above copyright notice, 
'  this list of conditions and the following disclaimer. 
'  Redistributions in binary form must reproduce the above copyright notice, 
'  this list of conditions and the following disclaimer in the documentation 
'  and/or other materials provided with the distribution. 

'  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
'  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
'  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
'  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
'  REMAINS UNCHANGED.


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.Drawing.Imaging
Imports System.IO
Imports FileDialogExtenders


Namespace CustomControls
	Partial Public Class MySaveDialogControl
		Inherits FileDialogControlBase
		Private _changedImage As Bitmap
		Friend _originalImage As Image
		Private _rft As RotateFlipType
		Private _canDisplay As Boolean = True
		Private _format As ImageFormat = ImageFormat.Jpeg
		Private _memstream As System.IO.MemoryStream
		Private _imageFile As String
		Public Sub New()
			InitializeComponent()
		End Sub

		Protected Overrides Sub OnPrepareMSDialog()
			MyBase.OnPrepareMSDialog()
			MSDialog.FilterIndex = GetIndexfromFile(_imageFile)
			If Environment.OSVersion.Version.Major < 6 Then
				MSDialog.SetPlaces(New Object() { CInt(Places.Desktop), CInt(Places.Printers), CInt(Places.Favorites), CInt(Places.Programs), CInt(Places.Fonts) })
			End If
			FileDlgInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
		End Sub

		Public Sub New(ByVal originalfile As String, ByVal parent As IWin32Window)
			Try
				_imageFile = originalfile
				_originalImage = Image.FromFile(originalfile)
			Catch
				_imageFile = Nothing
			End Try

			If _imageFile Is Nothing OrElse _imageFile.Length = 0 Then

				Dim openDialog As New MyOpenFileDialogControl()
				Try
					If parent Is Nothing Then
						parent = Application.OpenForms(0)
					End If
					If openDialog.ShowDialog(parent) = DialogResult.OK Then
						_imageFile = openDialog.MSDialog.FileName
						_originalImage = Image.FromFile(_imageFile)
						InitializeComponent()
					Else
						Dispose()
					End If
				Finally
					If openDialog IsNot Nothing Then
						openDialog.Dispose()
					End If
					openDialog = Nothing
					GC.Collect()
					GC.WaitForPendingFinalizers()
				End Try
			Else

				InitializeComponent()
			End If

		End Sub
		Private Sub MySaveDialogControl_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			If Not DesignMode Then
				Dim fi As New FileInfo(_imageFile)
				_lblOriginalFileLen.Text = String.Format("Original File Size: {0} bytes", fi.Length)
			End If
			_horizontal.Minimum = _pbChanged.MinimumSize.Width
			_horizontal.Maximum = _pbChanged.MaximumSize.Width\2
			_vertical.Minimum = _pbChanged.MinimumSize.Height
			_vertical.Maximum = _pbChanged.MaximumSize.Height\2
			_horizontal.Value = _pbChanged.MaximumSize.Width\2
			_vertical.Value = _pbChanged.MaximumSize.Height\2
			_groupBox2.Text = System.IO.Path.GetFileName(_imageFile)
			_groupBox1.Text = "New Image"
			_rotateflip.DataSource = System.Enum.GetNames(GetType(RotateFlipType))
			_rotateFlipOriginal.DataSource = System.Enum.GetNames(GetType(RotateFlipType))
			_cbxOriginalMode.DataSource = System.Enum.GetNames(GetType(System.Windows.Forms.PictureBoxSizeMode))
			_cbxNewViewMode.DataSource = System.Enum.GetNames(GetType(System.Windows.Forms.PictureBoxSizeMode))
			_cbxNewViewMode.SelectedIndex = CInt(PictureBoxSizeMode.Zoom)
			_cbxOriginalMode.SelectedIndex = CInt(PictureBoxSizeMode.Zoom)
			_pbChanged.SizeMode = PictureBoxSizeMode.Zoom
			_pbOriginal.SizeMode = PictureBoxSizeMode.Zoom
			_pbChanged.MaximumSize = _pbChanged.Size
			_pbOriginal.MaximumSize = _pbOriginal.Size
			If _originalImage IsNot Nothing AndAlso (Not DesignMode) Then
				_pbOriginal.Image = _originalImage
				_format = _originalImage.RawFormat
				_lblOrigInfo.Text = String.Format("Colors: {0}" & Constants.vbLf & "Size: {1} X {2} pixels" & Constants.vbLf & "Horizontal Resolution {3}" & Constants.vbLf & "Vertical resolution: {4}", MyOpenFileDialogControl.GetColorsCountFromImage(_pbOriginal.Image),_originalImage.Width, _originalImage.Height,_originalImage.HorizontalResolution, _originalImage.VerticalResolution)
				Display()
			End If
		End Sub

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If _memstream IsNot Nothing Then
				_memstream.Dispose()
				_memstream = Nothing
			End If
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		Private Sub _rotateflip_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _rotateflip.SelectedIndexChanged, _rotateFlipOriginal.SelectedIndexChanged
			If _changedImage Is Nothing Then
				Return
			End If
			If sender Is _rotateflip Then
				_rft = CType(System.Enum.Parse(GetType(RotateFlipType), _rotateflip.SelectedItem.ToString()), RotateFlipType)
				Display()
			Else
				_pbOriginal.Image.RotateFlip(CType(System.Enum.Parse(GetType(RotateFlipType), _rotateFlipOriginal.SelectedItem.ToString()), RotateFlipType))
				_pbOriginal.Refresh()
			End If


		End Sub

		Private Sub _horizontal_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _horizontal.ValueChanged
			If _originalImage Is Nothing OrElse (Not _canDisplay) Then
				Return
			End If
			If _ckbPreserveaspect.Checked Then
				_vertical.Value = _horizontal.Value
			End If
		End Sub


		Private Sub _vertical_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _vertical.ValueChanged
			If _originalImage Is Nothing OrElse (Not _canDisplay) Then
				Return
			End If
		End Sub

		Private Sub _Update_Click(ByVal sender As Object, ByVal e As EventArgs)
			Display()
		End Sub

		Private Function Display() As Boolean
			Try
				If _originalImage Is Nothing AndAlso _canDisplay Then
					Return False
				End If
				Dim aspectfactor As Double = CDbl(_originalImage.Height) / _originalImage.Width
				Dim verticalVal As Integer = CInt(Fix(Me._vertical.Value))
				Dim horizontalVal As Integer = CInt(Fix(Me._horizontal.Value))
				If aspectfactor > 1 Then
					horizontalVal = CInt(Fix(horizontalVal / aspectfactor))
				Else
					verticalVal = CInt(Fix(verticalVal * aspectfactor))
				End If
				Dim newImg As Bitmap = TryCast(_originalImage.GetThumbnailImage(horizontalVal, verticalVal, Nothing, IntPtr.Zero), Bitmap)
				If _changedImage IsNot Nothing Then
					_changedImage.Dispose()
				End If
				newImg.RotateFlip(_rft)
				_pbChanged.SuspendLayout()
				_pbOriginal.SuspendLayout()
				If _cbxNewViewMode.SelectedItem Is Nothing Then
					_pbChanged.SizeMode = PictureBoxSizeMode.AutoSize
				Else
					_pbChanged.SizeMode = CType(System.Enum.Parse(GetType(PictureBoxSizeMode), _cbxNewViewMode.SelectedItem.ToString()), PictureBoxSizeMode)
				End If
				If _cbxOriginalMode.SelectedItem Is Nothing Then
					_pbOriginal.SizeMode = PictureBoxSizeMode.AutoSize
				Else
					_pbOriginal.SizeMode = CType(System.Enum.Parse(GetType(PictureBoxSizeMode), _cbxOriginalMode.SelectedItem.ToString()), PictureBoxSizeMode)
				End If
				_changedImage = newImg
				_pbChanged.Image = _changedImage
				_pbChanged.Size = _pbChanged.MaximumSize
				_pbChanged.ResumeLayout(True)
				_pbOriginal.ResumeLayout(True)
				Return ShowImageInfo()
			Catch
				Return False
			End Try
		End Function

		Private Function ShowImageInfo() As Boolean
			Try
				If _memstream IsNot Nothing Then
					_memstream.Dispose()
					_memstream = Nothing
				End If
				_memstream = New MemoryStream()
				_changedImage.Save(_memstream, _format)
				'correct the format bug
				Dim ifrm As ImageFormat = Nothing
				If _format.Equals(ImageFormat.Bmp) Then
					ifrm = ImageFormat.Bmp
				ElseIf _format.Equals(ImageFormat.Gif) Then
					ifrm = ImageFormat.Gif
				ElseIf _format.Equals(ImageFormat.Jpeg) Then
					ifrm = ImageFormat.Jpeg
				ElseIf _format.Equals(ImageFormat.Png) Then
					ifrm = ImageFormat.Png
				ElseIf _format.Equals(ImageFormat.Tiff) Then
					ifrm = ImageFormat.Tiff
				ElseIf _format.Equals(ImageFormat.Emf) Then
					ifrm = ImageFormat.Emf
				ElseIf _format.Equals(ImageFormat.Icon) Then
					ifrm = ImageFormat.Icon
				ElseIf _format.Equals(ImageFormat.Wmf) Then
					ifrm = ImageFormat.Wmf
				ElseIf _format.Equals(ImageFormat.Exif) Then
					ifrm = ImageFormat.Exif
				End If


				_FileSize.Text = String.Format("Picture Size: {0} bytes for {3}" & Constants.vbLf & "Width = {1} Height ={2} pixels", _memstream.Length, CInt(Fix(_horizontal.Value)), CInt(Fix(_vertical.Value)), ifrm.ToString())
				Return True
			Catch
				Throw
			End Try
		End Function

		Private Sub UPDOWN_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles _horizontal.MouseDown, _vertical.MouseDown
			If e.Button = MouseButtons.Left AndAlso TypeOf sender Is UpDownBase Then
				_canDisplay = False
			End If
			If _ckbPreserveaspect.Checked Then
				_vertical.Value = _horizontal.Value
			End If

		End Sub

		Private Sub UPDOWN_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles _horizontal.MouseUp, _vertical.MouseUp
			If e.Button = MouseButtons.Left AndAlso TypeOf sender Is UpDownBase Then
			If _ckbPreserveaspect.Checked Then
				_vertical.Value = _horizontal.Value
			End If
			End If
				_canDisplay = True
			Display()
		End Sub

		Private Sub MySaveDialogControl_FilterChanged(ByVal sender As IWin32Window, ByVal index As Integer) Handles MyBase.EventFilterChanged
			FileDlgEnableOkBtn = GetFormatFromIndex(index)
		End Sub

		Private Function GetFormatFromIndex(ByVal index As Integer) As Boolean
			Dim ext As String = String.Empty
			Dim oldform As ImageFormat = _format
			Try
				Dim extensions() As String = MSDialog.Filter.Split("|"c)
				index -= 1
				ext = extensions(2 * index + 1)
				Dim pos As Integer = ext.LastIndexOf("."c)
				ext = ext.Substring(pos + 1).Trim().ToLower()
				Select Case ext
					Case "jpg", "jpeg"
						_format = ImageFormat.Jpeg
					Case "bmp"
						_format = ImageFormat.Bmp
					Case "gif"
						_format = ImageFormat.Gif
					Case "emf"
						_format = ImageFormat.Emf
					Case "ico"
						_format = ImageFormat.Icon
					Case "png"
						_format = ImageFormat.Png
					Case "tif"
						_format = ImageFormat.Tiff
					Case "wmf"
						_format = ImageFormat.Wmf
					Case "exif"
						_format = ImageFormat.Exif
					Case Else
						Throw New ApplicationException(ext & " is an unknown format.Select a known one.")
				End Select
				Return ShowImageInfo()
			Catch ex As Exception
				MessageBox.Show(Me, ex.Message, "Conversion error for ." & ext & " type", MessageBoxButtons.OK, MessageBoxIcon.Error)
				'revert to last good format
				MSDialog.FilterIndex = GetIndexfromFile("dummyfile." & oldform.ToString())
				_format = oldform
				Return False
			End Try
		End Function

	   Private Function GetIndexfromFile(ByVal file As String) As Integer

			Try
			Dim ext As String = System.IO.Path.GetExtension(file).ToLower()
			Dim index As Integer = MSDialog.Filter.ToLower().IndexOf(ext)
			Dim extensions() As String = MSDialog.Filter.Substring(0,index).Split(New Char() { "|"c }, StringSplitOptions.RemoveEmptyEntries)
			Return extensions.Length \ 2 + 1
			Catch ex As Exception
				MessageBox.Show(Me, ex.Message, "Unknown Format!", MessageBoxButtons.OK, MessageBoxIcon.Error)
				Return 1
			End Try
	   End Function
		Private Sub MySaveDialogControl_ClosingDialog(ByVal sender As Object, ByVal e As CancelEventArgs) Handles MyBase.EventClosingDialog
			If _memstream Is Nothing Then
				Return
			End If
			Dim br As System.IO.FileStream = Nothing
			Try

					br = New FileStream(MSDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write)
					_memstream.WriteTo(br)
					br.Flush()
					br.Close()
					e.Cancel = False
			Catch ex As Exception
				MessageBox.Show(Me, "Unable to save the file because:" & Constants.vbLf + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
				e.Cancel = True
			Finally
				If br IsNot Nothing Then
					br.Dispose()
				End If
				_memstream.Dispose()
				_memstream = Nothing
			End Try

		End Sub


		Private Sub ckbOrigStretch_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
			Display()
		End Sub

		Private Sub ckbPreserveaspect_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _ckbPreserveaspect.CheckedChanged
			_vertical.Enabled = Not _ckbPreserveaspect.Checked
			If _ckbPreserveaspect.Checked Then
				_vertical.Value = _horizontal.Value
			End If
		End Sub

		Private Sub cbxNewViewMode_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _cbxNewViewMode.SelectedIndexChanged, _cbxOriginalMode.SelectedIndexChanged
			If sender Is _cbxNewViewMode Then
				_pbChanged.SizeMode = CType(System.Enum.Parse(GetType(PictureBoxSizeMode), _cbxNewViewMode.SelectedItem.ToString()), PictureBoxSizeMode)
			Else
				_pbOriginal.SizeMode = CType(System.Enum.Parse(GetType(PictureBoxSizeMode), _cbxOriginalMode.SelectedItem.ToString()), PictureBoxSizeMode)
			End If
			Display()
		End Sub

		Private Sub MySaveDialogControl_HelpRequested(ByVal sender As Object, ByVal hlpevent As HelpEventArgs) Handles MyBase.HelpRequested
			MessageBox.Show("Please add some specific help here")
		End Sub

	End Class
End Namespace

