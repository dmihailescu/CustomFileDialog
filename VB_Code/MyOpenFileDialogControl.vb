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
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Drawing.Imaging
Imports System.Collections.Generic

Imports FileDialogExtenders

Namespace CustomControls

	Partial Public Class MyOpenFileDialogControl
		Inherits FileDialogControlBase
		#Region "Constructors"
		Public Sub New()
			InitializeComponent()
		End Sub
		#End Region

		#Region "Overrides"
		Protected Overrides Sub OnPrepareMSDialog()
			MyBase.FileDlgInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
			If Environment.OSVersion.Version.Major < 6 Then
				MSDialog.SetPlaces(New Object() { "c:\", CInt(Places.MyComputer), CInt(Places.Favorites), CInt(Places.Printers), CInt(Places.Fonts) })
			End If
			MyBase.OnPrepareMSDialog()
		End Sub
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1807:AvoidUnnecessaryStringCreation", MessageId := "filePath")> _
		Private Sub MyOpenFileDialogControl_FileNameChanged(ByVal sender As IWin32Window, ByVal filePath As String) Handles MyBase.EventFileNameChanged
			If filePath.ToLower().EndsWith(".bmp") OrElse filePath.ToLower().EndsWith(".jpg") OrElse filePath.ToLower().EndsWith(".png") OrElse filePath.ToLower().EndsWith(".tif") OrElse filePath.ToLower().EndsWith(".gif") Then
				If pbxPreview.Image IsNot Nothing Then
					pbxPreview.Image.Dispose()
				End If

				Try
					Dim fi As New FileInfo(filePath)
					pbxPreview.Image = Bitmap.FromFile(filePath)
					lblSizeValue.Text = (fi.Length \ 1024).ToString() & "KB"
					lblColorsValue.Text = GetColorsCountFromImage(pbxPreview.Image)
					lblFormatValue.Text = GetFormatFromImage(pbxPreview.Image)
					FileDlgEnableOkBtn = True
				Catch e1 As Exception
					FileDlgEnableOkBtn = False
				End Try
			Else
				If pbxPreview.Image IsNot Nothing Then
					pbxPreview.Image.Dispose()
				End If
				pbxPreview.Image = Nothing
			End If
		End Sub

		#End Region

		#Region "Private Methods"
		Friend Shared Function GetColorsCountFromImage(ByVal image As Image) As String
			Select Case image.PixelFormat
				Case PixelFormat.Format16bppArgb1555, PixelFormat.Format16bppGrayScale, PixelFormat.Format16bppRgb555, PixelFormat.Format16bppRgb565
					Return "16 bits (65536 colors)"
				Case PixelFormat.Format1bppIndexed
					Return "1 bit (Black & White)"
				Case PixelFormat.Format24bppRgb
					Return "24 bits (True Colors)"
				Case PixelFormat.Format32bppArgb, PixelFormat.Format32bppPArgb, PixelFormat.Format32bppRgb
					Return "32 bits (Alpha Channel)"
				Case PixelFormat.Format4bppIndexed
					Return "4 bits (16 colors)"
				Case PixelFormat.Format8bppIndexed
					Return "8 bits (256 colors)"
			End Select
			Return String.Empty
		End Function

		Private Shared Function GetFormatFromImage(ByVal image As Image) As String
			If image.RawFormat.Equals(ImageFormat.Bmp) Then
				Return "BMP"
			ElseIf image.RawFormat.Equals(ImageFormat.Gif) Then
				Return "GIF"
			ElseIf image.RawFormat.Equals(ImageFormat.Jpeg) Then
				Return "JPG"
			ElseIf image.RawFormat.Equals(ImageFormat.Png) Then
				Return "PNG"
			ElseIf image.RawFormat.Equals(ImageFormat.Tiff) Then
				Return "TIFF"
			End If
			Return String.Empty
		End Function
		#End Region

		Private Sub MyOpenFileDialogControl_ClosingDialog(ByVal sender As Object, ByVal e As CancelEventArgs) Handles MyBase.EventClosingDialog
			If pbxPreview.Image IsNot Nothing Then
				pbxPreview.Image.Dispose()
			End If
			e.Cancel = False
		End Sub

		Private Sub MyOpenFileDialogControl_FolderNameChanged(ByVal sender As IWin32Window, ByVal filePath As String) Handles MyBase.EventFolderNameChanged
			If pbxPreview.Image IsNot Nothing Then
				pbxPreview.Image.Dispose()
			End If
			pbxPreview.Image = Nothing
			lblSizeValue.Text = String.Empty
			lblColorsValue.Text = String.Empty
			lblFormatValue.Text = String.Empty
		End Sub

		Private Sub MyOpenFileDialogControl_HelpRequested(ByVal sender As Object, ByVal hlpevent As HelpEventArgs) Handles MyBase.HelpRequested
			MessageBox.Show("Please add some specific help here")
		End Sub


	End Class
End Namespace