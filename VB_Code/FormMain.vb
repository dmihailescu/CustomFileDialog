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
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports AboutUtil
Imports FileDialogExtenders

Namespace CustomControls

	Partial Public Class FormMain
		Inherits Form
		Public Sub New()
			InitializeComponent()

		End Sub

		Protected Overrides Sub OnLoad(ByVal e As EventArgs)
			lblFilePath.Text = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "profiles\All Users\Documents\My Pictures\Sample Pictures\winter.jpg")
			MyBase.OnLoad(e)
		End Sub
		Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _btnSelect.Click, _btnSave.Click, _btnExtension.Click, _btnSaveExt.Click, _btnExit.Click

			If sender.Equals(_btnSelect) Then
				Using openDialog As New MyOpenFileDialogControl()
					If openDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
						lblFilePath.Text = openDialog.MSDialog.FileName
					End If
				End Using

			ElseIf sender.Equals(_btnSave) Then
				Using saveDialog As New MySaveDialogControl(lblFilePath.Text, Me)
					If saveDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
						lblFilePath.Text = saveDialog.MSDialog.FileName
					End If
				End Using
			ElseIf sender.Equals(Me._btnExtension) Then
				Using openDialogCtrl As New MyOpenFileDialogControl()
					openDialogCtrl.FileDlgInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
					Dim openDialog As New OpenFileDialog()
					openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
					openDialog.AddExtension = True
					openDialog.Filter = "Image Files(*.bmp)|*.bmp |Image Files(*.JPG)|*.JPG|Image Files(*.jpeg)|*.jpeg|Image Files(*.GIF)|*.GIF|Image Files(*.emf)|*emf.|Image Files(*.ico)|*.ico|Image Files(*.png)|*.png|Image Files(*.tif)|*.tif|Image Files(*.wmf)|*.wmf|Image Files(*.exif)|*.exif"
					openDialog.FilterIndex = 2
					openDialog.CheckFileExists = True
					openDialog.DefaultExt = "jpg"
					openDialog.FileName = "Select Picture"
					openDialog.DereferenceLinks = True
					'openDialog.ShowHelp = true;
					If Environment.OSVersion.Version.Major < 6 Then
						 openDialog.SetPlaces(New Object() { "c:\", CInt(Places.MyComputer), CInt(Places.Favorites), CInt(Places.Printers), CInt(Places.Fonts) })
					End If
					If openDialog.ShowDialog(openDialogCtrl, Me) = System.Windows.Forms.DialogResult.OK Then
						lblFilePath.Text = openDialog.FileName
					End If
				End Using
			ElseIf sender.Equals(_btnSaveExt) Then
				Using saveDialog As New SaveFileDialog()
					Dim saveDialogCtrl As New MySaveDialogControl(lblFilePath.Text, Me)
					saveDialogCtrl.FileDlgInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
					saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
					saveDialog.AddExtension = True
					saveDialog.Filter = "Image Files(*.bmp)|*.bmp |Image Files(*.JPG)|*.JPG|Image Files(*.jpeg)|*.jpeg|Image Files(*.GIF)|*.GIF|Image Files(*.emf)|*emf.|Image Files(*.ico)|*.ico|Image Files(*.png)|*.png|Image Files(*.tif)|*.tif|Image Files(*.wmf)|*.wmf|Image Files(*.exif)|*.exif"
					saveDialog.FilterIndex = 2
					saveDialog.CheckFileExists = True
					saveDialog.DefaultExt = "jpg"
					saveDialog.FileName = "Change Picture"
					saveDialog.DereferenceLinks = True
					'saveDialog.ShowHelp = true;
					If Environment.OSVersion.Version.Major < 6 Then
						saveDialog.SetPlaces(New Object() { CInt(Places.Desktop), CInt(Places.Printers), CInt(Places.Favorites), CInt(Places.Programs), CInt(Places.Fonts) })
					End If
					If saveDialog.ShowDialog(saveDialogCtrl, Me) = System.Windows.Forms.DialogResult.OK Then
						lblFilePath.Text = saveDialog.FileName
					End If
				End Using
			ElseIf sender.Equals(_btnExit) Then
				Me.Close()
			End If

			System.GC.Collect()
			GC.WaitForPendingFinalizers()
		End Sub

		Private Sub _btnAbout_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _btnAbout.Click
			Dim dlg As New About(Me)
			dlg.ShowDialog(Me)
		End Sub

	End Class
End Namespace