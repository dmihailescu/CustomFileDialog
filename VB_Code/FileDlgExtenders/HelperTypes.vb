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
Imports System.Text
Imports System.Data
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D
Imports Win32Types

Namespace FileDialogExtenders
	Partial Public Class FileDialogControlBase
		#Region "Helper Classes"

		Private Class MSFileDialogWrapper
			Inherits NativeWindow
			Implements IDisposable
			Public Const UFLAGSSIZE As SetWindowPosFlags = SetWindowPosFlags.SWP_NOACTIVATE Or SetWindowPosFlags.SWP_NOOWNERZORDER Or SetWindowPosFlags.SWP_NOMOVE
			#Region "Delegates"

			#End Region

			#Region "Events"
			#End Region

			#Region "Variables Declaration"
			Private _filterIndex As Integer
			Private _CustomCtrl As FileDialogControlBase
			#End Region

			#Region "Constructors"
			Public Sub New(ByVal fd As FileDialogControlBase)
				_CustomCtrl = fd
				If _CustomCtrl IsNot Nothing Then
					AddHandler fd.MSDialog.Disposed, AddressOf NativeDialogWrapper_Disposed
				End If

			End Sub

			Private Sub NativeDialogWrapper_Disposed(ByVal sender As Object, ByVal e As EventArgs)
				Dispose()
			End Sub
			#End Region

			#Region "Methods"
			Public Sub Dispose() Implements IDisposable.Dispose
				If _CustomCtrl IsNot Nothing Then
					If _CustomCtrl.MSDialog IsNot Nothing Then
						RemoveHandler _CustomCtrl.MSDialog.Disposed, AddressOf NativeDialogWrapper_Disposed
						_CustomCtrl.MSDialog.Dispose()
						If _CustomCtrl IsNot Nothing Then
							_CustomCtrl.MSDialog = Nothing
						End If
					End If
					If _CustomCtrl IsNot Nothing Then
						If Not _CustomCtrl.IsDisposed Then
							_CustomCtrl.Dispose()
						End If
						_CustomCtrl = Nothing
					End If
				End If
				 DestroyHandle()

			End Sub
			#End Region

			#Region "Overrides"
			Protected Overrides Sub WndProc(ByRef m As Message)
				Select Case CType(m.Msg, Msg)
					Case Msg.WM_NOTIFY
						Dim ofNotify As OFNOTIFY = CType(Marshal.PtrToStructure(m.LParam, GetType(OFNOTIFY)), OFNOTIFY)
						Select Case ofNotify.hdr.code
							Case CUInt(DialogChangeStatus.CDN_SELCHANGE)
									Dim filePath As New StringBuilder(256)
									NativeMethods.SendMessage(New HandleRef(Me, NativeMethods.GetParent(Handle)), CUInt(DialogChangeProperties.CDM_GETFILEPATH), CType(256, IntPtr), filePath)
									If _CustomCtrl IsNot Nothing Then
										_CustomCtrl.OnFileNameChanged(Me, filePath.ToString())
									End If
							Case CUInt(DialogChangeStatus.CDN_FOLDERCHANGE)
									Dim folderPath As New StringBuilder(256)
									NativeMethods.SendMessage(New HandleRef(Me, NativeMethods.GetParent(Handle)), CInt(DialogChangeProperties.CDM_GETFOLDERPATH), CType(256, IntPtr), folderPath)
									If _CustomCtrl IsNot Nothing Then
										_CustomCtrl.OnFolderNameChanged(Me, folderPath.ToString())
									End If
							Case CUInt(DialogChangeStatus.CDN_TYPECHANGE)
									Dim ofn As OPENFILENAME = CType(Marshal.PtrToStructure(ofNotify.OpenFileName, GetType(OPENFILENAME)), OPENFILENAME)
									Dim i As Integer = ofn.nFilterIndex
									If _CustomCtrl IsNot Nothing AndAlso _filterIndex <> i Then
										_filterIndex = i
										_CustomCtrl.OnFilterChanged(TryCast(Me, IWin32Window), i)
									End If
							Case CUInt(DialogChangeStatus.CDN_INITDONE)
							Case CUInt(DialogChangeStatus.CDN_SHAREVIOLATION)
							Case CUInt(DialogChangeStatus.CDN_HELP)
							Case CUInt(DialogChangeStatus.CDN_INCLUDEITEM)

							Case CUInt(DialogChangeStatus.CDN_FILEOK) '0xfffffda2:
'INSTANT VB TODO TASK: There is no equivalent to #pragma directives in VB.NET:
'#pragma warning disable 1690, 0414
								'NativeMethods.SetWindowPos(_CustomCtrl._hFileDialogHandle, IntPtr.Zero,
								'(int)_CustomCtrl._OpenDialogWindowRect.left,
								'(int)_CustomCtrl._OpenDialogWindowRect.top,
								'(int)_CustomCtrl._OpenDialogWindowRect.Width,
								'(int)_CustomCtrl._OpenDialogWindowRect.Height,
								'FileDialogControlBase.MSFileDialogWrapper.UFLAGSSIZE);
'INSTANT VB TODO TASK: There is no equivalent to #pragma directives in VB.NET:
'#pragma warning restore 1690, 0414
							Case Else


						End Select
					Case Msg.WM_COMMAND
						Select Case NativeMethods.GetDlgCtrlID(m.LParam) 'switch (m.WParam & 0x0000ffff)
							Case CInt(ControlsId.ButtonOk) 'OK
							Case CInt(ControlsId.ButtonCancel) 'Cancel
							Case CInt(ControlsId.ButtonHelp) '0x0000040e://help
						End Select
					Case Else
				End Select
				MyBase.WndProc(m)
			End Sub
			#End Region
		End Class

		Private Class WholeDialogWrapper
			Inherits NativeWindow
			Implements IDisposable
			#Region "Constants Declaration"
			Private Const UFLAGSSIZEEX As SetWindowPosFlags = SetWindowPosFlags.SWP_NOACTIVATE Or SetWindowPosFlags.SWP_NOOWNERZORDER Or SetWindowPosFlags.SWP_NOMOVE Or SetWindowPosFlags.SWP_ASYNCWINDOWPOS Or SetWindowPosFlags.SWP_DEFERERASE
			Private Const UFLAGSHIDE As SetWindowPosFlags = SetWindowPosFlags.SWP_NOACTIVATE Or SetWindowPosFlags.SWP_NOOWNERZORDER Or SetWindowPosFlags.SWP_NOMOVE Or SetWindowPosFlags.SWP_NOSIZE Or SetWindowPosFlags.SWP_HIDEWINDOW
			Private Const UFLAGSZORDER As SetWindowPosFlags = SetWindowPosFlags.SWP_NOACTIVATE Or SetWindowPosFlags.SWP_NOMOVE Or SetWindowPosFlags.SWP_NOSIZE
			Private Const WS_VISIBLE As UInteger = &H10000000
			Private Shared ReadOnly HWND_MESSAGE As New IntPtr(-3)
			Private Shared ReadOnly NULL As IntPtr = IntPtr.Zero
			#End Region

			#Region "Variables Declaration"
			Private _hDummyWnd As IntPtr = NULL
			Private mResized As Boolean
			Private _CustomControl As FileDialogControlBase = Nothing
			Private _WatchForActivate As Boolean = False
			Private mOriginalSize As Size
			Private _hFileDialogHandle As IntPtr
			Private _ListViewInfo As WINDOWINFO
			Private _BaseDialogNative As MSFileDialogWrapper
			Private _ComboFolders As IntPtr
			Private _ComboFoldersInfo As WINDOWINFO
			Private _hGroupButtons As IntPtr
			Private _GroupButtonsInfo As WINDOWINFO
			Private _hComboFileName As IntPtr
			Private _ComboFileNameInfo As WINDOWINFO
			Private _hComboExtensions As IntPtr
			Private _ComboExtensionsInfo As WINDOWINFO
			Private _hOKButton As IntPtr
			Private _OKButtonInfo As WINDOWINFO
			Private _hCancelButton As IntPtr
			Private _CancelButtonInfo As WINDOWINFO
			Private _hHelpButton As IntPtr
			Private _HelpButtonInfo As WINDOWINFO
			Private _hToolBarFolders As IntPtr
			Private _ToolBarFoldersInfo As WINDOWINFO
			Private _hLabelFileName As IntPtr
			Private _LabelFileNameInfo As WINDOWINFO
			Private _hLabelFileType As IntPtr
			Private _LabelFileTypeInfo As WINDOWINFO
			Private _hChkReadOnly As IntPtr
			Private _ChkReadOnlyInfo As WINDOWINFO
			Private mIsClosing As Boolean = False
			Private mInitializated As Boolean = False
			Private _DialogWindowRect As New RECT()
			Private _DialogClientRect As New RECT()

			#End Region

			#Region "Constructors"

			Public Sub New(ByVal fileDialogEx As FileDialogControlBase)
				'create the FileDialog &  custom control without UI yet
				_CustomControl = fileDialogEx
				'_CustomControl.MSDialog = new FDLG();
				AssignDummyWindow()
				_WatchForActivate = True

			End Sub
			#End Region

			#Region "Events"

			#End Region

			#Region "Methods"

			Private Sub AssignDummyWindow()
				'_hDummyWnd = Win32.CreateWindowEx(0x00050100, "Message", null, 0x16C80000, -10000, -10000, 0, 0,
				'parent.Handle, NULL, NULL, NULL);
				_hDummyWnd = NativeMethods.CreateWindowEx(0, "Message", Nothing, WS_VISIBLE, 0, 0, 0, 0, HWND_MESSAGE, NULL, NULL, NULL)
				If _hDummyWnd = NULL OrElse (Not NativeMethods.IsWindow(_hDummyWnd)) Then
					Throw New ApplicationException("Unable to create a dummy window")
				End If
				AssignHandle(_hDummyWnd)
			End Sub


			Public Sub Dispose() Implements IDisposable.Dispose

				If _CustomControl IsNot Nothing Then
					If Not _CustomControl.IsDisposed Then
						If _CustomControl.MSDialog IsNot Nothing Then
							RemoveHandler _CustomControl.MSDialog.Disposed, AddressOf DialogWrappper_Disposed
							_CustomControl.MSDialog.Dispose()

						End If
						'might have been nulled by MSDialog.Dispose()
						If _CustomControl IsNot Nothing Then
							_CustomControl.MSDialog = Nothing
							_CustomControl.Dispose()
						End If
						_CustomControl = Nothing
					End If
				End If
				If _BaseDialogNative IsNot Nothing Then
					_BaseDialogNative.Dispose()
					_BaseDialogNative = Nothing
				End If
				If _hDummyWnd <> IntPtr.Zero Then
					NativeMethods.DestroyWindow(_hDummyWnd)
					DestroyHandle()
					_hDummyWnd = IntPtr.Zero
				End If
			End Sub
			#End Region

			#Region "Private Methods"
			Private Sub PopulateWindowsHandlers()
				NativeMethods.EnumChildWindows(New HandleRef(Me,_hFileDialogHandle), New NativeMethods.EnumWindowsCallBack(AddressOf FileDialogEnumWindowCallBack), 0)
			End Sub

			Private Function FileDialogEnumWindowCallBack(ByVal hwnd As IntPtr, ByVal lParam As Integer) As Boolean
				Dim className As New StringBuilder(256)
				NativeMethods.GetClassName(New HandleRef(Me,hwnd), className, className.Capacity)
				Dim controlID As Integer = NativeMethods.GetDlgCtrlID(hwnd)
				Dim windowInfo As WINDOWINFO
				NativeMethods.GetWindowInfo(New HandleRef(Me,hwnd), windowInfo)

				' Dialog Window
				If className.ToString().StartsWith("#32770") Then
					_BaseDialogNative = New MSFileDialogWrapper(_CustomControl)
					_BaseDialogNative.AssignHandle(hwnd)
					Return True
				End If

				Select Case CType(controlID, ControlsId)
					Case ControlsId.DefaultView
						_CustomControl._hListViewPtr = hwnd
						NativeMethods.GetWindowInfo(New HandleRef(Me,hwnd), _ListViewInfo)
						_CustomControl.UpdateListView()
					Case ControlsId.ComboFolder
						_ComboFolders = hwnd
						_ComboFoldersInfo = windowInfo
					Case ControlsId.ComboFileType
						_hComboExtensions = hwnd
						_ComboExtensionsInfo = windowInfo
					Case ControlsId.ComboFileName
						If className.ToString().ToLower() = "comboboxex32" Then
							_hComboFileName = hwnd
							_ComboFileNameInfo = windowInfo
						End If
					Case ControlsId.GroupFolder
						_hGroupButtons = hwnd
						_GroupButtonsInfo = windowInfo
					Case ControlsId.LeftToolBar
						_hToolBarFolders = hwnd
						_ToolBarFoldersInfo = windowInfo
					Case ControlsId.ButtonOk
						_hOKButton = hwnd
						_OKButtonInfo = windowInfo
						_CustomControl._hOKButton = hwnd
						'Win32Types.NativeMethods.EnableWindow(_hOKButton, false);
					Case ControlsId.ButtonCancel
						_hCancelButton = hwnd
						_CancelButtonInfo = windowInfo
					Case ControlsId.ButtonHelp
						_hHelpButton = hwnd
						_HelpButtonInfo = windowInfo
					Case ControlsId.CheckBoxReadOnly
						_hChkReadOnly = hwnd
						_ChkReadOnlyInfo = windowInfo
					Case ControlsId.LabelFileName
						_hLabelFileName = hwnd
						_LabelFileNameInfo = windowInfo
					Case ControlsId.LabelFileType
						_hLabelFileType = hwnd
						_LabelFileTypeInfo = windowInfo
				End Select

				Return True
			End Function

			Private Sub InitControls()
				mInitializated = True

				' Lets get information about the current open dialog
				NativeMethods.GetClientRect(New HandleRef(Me,_hFileDialogHandle), _DialogClientRect)
				NativeMethods.GetWindowRect(New HandleRef(Me,_hFileDialogHandle), _DialogWindowRect)

				' Lets borrow the Handles from the open dialog control
				PopulateWindowsHandlers()

				Select Case _CustomControl.FileDlgStartLocation
					Case AddonWindowLocation.Right
						' Now we transfer the control to the open dialog
                        _CustomControl.Location = New System.Drawing.Point(CInt(Fix(_DialogClientRect.Width - _CustomControl.Width)), 0)
					Case AddonWindowLocation.Bottom
						' Now we transfer the control to the open dialog
                        _CustomControl.Location = New System.Drawing.Point(0, CInt(Fix(_DialogClientRect.Height - _CustomControl.Height)))
					Case AddonWindowLocation.BottomRight
						' We don't have to do too much in this case, just the default thing
                        _CustomControl.Location = New System.Drawing.Point(CInt(Fix(_DialogClientRect.Width - _CustomControl.Width)), CInt(Fix(_DialogClientRect.Height - _CustomControl.Height)))
				End Select
				' Everything is ready, now lets change the parent
				NativeMethods.SetParent(New HandleRef(_CustomControl,_CustomControl.Handle), New HandleRef(Me,_hFileDialogHandle))

				' Send the control to the back
				' NativeMethods.SetWindowPos(_CustomControl.Handle, (IntPtr)ZOrderPos.HWND_BOTTOM, 0, 0, 0, 0, UFLAGSZORDER);
				AddHandler _CustomControl.MSDialog.Disposed, AddressOf DialogWrappper_Disposed
			End Sub

			Private Sub DialogWrappper_Disposed(ByVal sender As Object, ByVal e As EventArgs)
				Dispose()
			End Sub
			#End Region


			#Region "Overrides"
			'this is a child window for the whole Dialog
			Protected Overrides Sub WndProc(ByRef m As Message)
				Dim currentSize As New RECT()
				Const flags As SetWindowPosFlags = SetWindowPosFlags.SWP_NOZORDER Or SetWindowPosFlags.SWP_NOMOVE '| SetWindowPosFlags.SWP_NOREPOSITION | SetWindowPosFlags.SWP_ASYNCWINDOWPOS | SetWindowPosFlags.SWP_SHOWWINDOW | SetWindowPosFlags.SWP_DRAWFRAME;
				Select Case CType(m.Msg, Msg)
					Case Msg.WM_SHOWWINDOW
						InitControls()
						NativeMethods.GetWindowRect(New HandleRef(Me,_hFileDialogHandle), currentSize)
						'restore original sizes
						Dim top As Integer = If((_CustomControl.Parent Is Nothing), currentSize.top, _CustomControl.Parent.Top)
						Dim right As Integer = If((_CustomControl.Parent Is Nothing), currentSize.right, _CustomControl.Parent.Right)
						Dim currentClientSize As New RECT()
						NativeMethods.GetClientRect(New HandleRef(Me,_hFileDialogHandle), currentClientSize)
						Dim dy As Integer = CInt(Fix(currentSize.Height - currentClientSize.Height))
						Dim dx As Integer = CInt(Fix(currentSize.Width - currentClientSize.Width))
						Dim Height As Integer = 0
						Dim Width As Integer = 0
						Select Case _CustomControl.FileDlgStartLocation
							Case AddonWindowLocation.Bottom
								Width = Math.Max(_CustomControl.OriginalCtrlSize.Width + dx, CInt(Fix(FileDialogControlBase.OriginalDlgWidth)))
								NativeMethods.SetWindowPos(_hFileDialogHandle, CType(ZOrderPos.HWND_BOTTOM, IntPtr), right, top, Width, CInt(Fix(currentSize.Height)), flags)
							Case AddonWindowLocation.Right
								Height = Math.Max(_CustomControl.OriginalCtrlSize.Height + dy, CInt(Fix(FileDialogControlBase.OriginalDlgHeight)))
								NativeMethods.SetWindowPos(_hFileDialogHandle, CType(ZOrderPos.HWND_BOTTOM, IntPtr), right, top, CInt(Fix(currentSize.Width)), Height, flags)
						End Select
					Case Msg.WM_SIZE
							NativeMethods.GetClientRect(New HandleRef(Me,_hFileDialogHandle), currentSize)
							Select Case _CustomControl.FileDlgStartLocation
								Case AddonWindowLocation.Bottom
									If (Not mInitializated) AndAlso FileDialogControlBase.OriginalDlgWidth = 0 Then
										FileDialogControlBase.OriginalDlgWidth = currentSize.Width
									End If
									If currentSize.Width <> _CustomControl.Width Then
										_CustomControl.Width = CInt(Fix(currentSize.Width))
									End If
								Case AddonWindowLocation.Right
									If (Not mInitializated) AndAlso FileDialogControlBase.OriginalDlgHeight = 0 Then
										FileDialogControlBase.OriginalDlgHeight = currentSize.Height
									End If
									If currentSize.Height <> _CustomControl.Height Then
										_CustomControl.Height = CInt(Fix(currentSize.Height))
									End If
							End Select

					Case Msg.WM_SIZING

						NativeMethods.GetClientRect(New HandleRef(Me,_hFileDialogHandle), currentSize)
						Select Case _CustomControl.FileDlgStartLocation
							Case AddonWindowLocation.Right
								If currentSize.Height <> _CustomControl.Height Then
									NativeMethods.SetWindowPos(_CustomControl.Handle, CType(ZOrderPos.HWND_BOTTOM, IntPtr), 0, 0, CInt(Fix(_CustomControl.Width)), CInt(Fix(currentSize.Height)), UFLAGSSIZEEX)
								End If
							Case AddonWindowLocation.Bottom
								If currentSize.Height <> _CustomControl.Height Then
									NativeMethods.SetWindowPos(_CustomControl.Handle, CType(ZOrderPos.HWND_BOTTOM, IntPtr), 0, 0, CInt(Fix(currentSize.Width)), CInt(Fix(_CustomControl.Height)), UFLAGSSIZEEX)
								End If
							Case AddonWindowLocation.BottomRight
								If currentSize.Width <> _CustomControl.Width OrElse currentSize.Height <> _CustomControl.Height Then
									NativeMethods.SetWindowPos(_CustomControl.Handle, CType(ZOrderPos.HWND_BOTTOM, IntPtr), CInt(Fix(currentSize.Width)), CInt(Fix(currentSize.Height)), CInt(Fix(currentSize.Width)), CInt(Fix(currentSize.Height)), UFLAGSSIZEEX)
								End If
						End Select
					Case Msg.WM_WINDOWPOSCHANGING
						If Not mIsClosing Then
							If (Not mInitializated) AndAlso (Not mResized) Then
								' Resize OpenDialog to make fit our extra form
								Dim pos As WINDOWPOS = CType(Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)
								If pos.flags <> 0 AndAlso ((pos.flags And CInt(SWP_Flags.SWP_NOSIZE)) <> CInt(SWP_Flags.SWP_NOSIZE)) Then
									Select Case _CustomControl.FileDlgStartLocation
										Case AddonWindowLocation.Right
											mOriginalSize = New Size(pos.cx, pos.cy)

											pos.cx += _CustomControl.Width
											Marshal.StructureToPtr(pos, m.LParam, True)

											currentSize = New RECT()
											NativeMethods.GetClientRect(New HandleRef(Me,_hFileDialogHandle), currentSize)
											If _CustomControl.Height < CInt(Fix(currentSize.Height)) Then
												_CustomControl.Height = CInt(Fix(currentSize.Height))
											End If

										Case AddonWindowLocation.Bottom
											mOriginalSize = New Size(pos.cx, pos.cy)

											pos.cy += _CustomControl.Height
											Marshal.StructureToPtr(pos, m.LParam, True)

											currentSize = New RECT()
											NativeMethods.GetClientRect(New HandleRef(Me,_hFileDialogHandle), currentSize)
											If _CustomControl.Width < CInt(Fix(currentSize.Width)) Then
												_CustomControl.Width = CInt(Fix(currentSize.Width))
											End If

										Case AddonWindowLocation.BottomRight
											mOriginalSize = New Size(pos.cx, pos.cy)

											pos.cy += _CustomControl.Height
											pos.cx += _CustomControl.Width
											Marshal.StructureToPtr(pos, m.LParam, True)

									End Select
									mResized = True ' Don't resize again
								End If
							End If
						End If

					Case Msg.WM_IME_NOTIFY
						If m.WParam = CType(ImeNotify.IMN_CLOSESTATUSWINDOW, IntPtr) Then
							mIsClosing = True
							NativeMethods.SetWindowPos(_hFileDialogHandle, IntPtr.Zero, 0, 0, 0, 0, UFLAGSHIDE)
							NativeMethods.GetWindowRect(New HandleRef(Me,_hFileDialogHandle), _DialogWindowRect)
							NativeMethods.SetWindowPos(_hFileDialogHandle, IntPtr.Zero, CInt(Fix(_DialogWindowRect.left)), CInt(Fix(_DialogWindowRect.top)), CInt(Fix(mOriginalSize.Width)), CInt(Fix(mOriginalSize.Height)), FileDialogControlBase.MSFileDialogWrapper.UFLAGSSIZE)
						End If
					Case Msg.WM_PAINT

					Case Msg.WM_NCCREATE

					Case Msg.WM_CREATE

					Case Msg.WM_ACTIVATE
						If _WatchForActivate AndAlso (Not mIsClosing) Then 'WM_NCACTIVATE works too
							_WatchForActivate = False
							'Now we save the real dialog window handle
							_hFileDialogHandle = m.LParam
							ReleaseHandle() 'release the dummy window
							AssignHandle(_hFileDialogHandle) 'assign the native open file handle to grab the messages
'INSTANT VB TODO TASK: There is no equivalent to #pragma directives in VB.NET:
'#pragma warning disable 0197, 0414
							NativeMethods.GetWindowRect(New HandleRef(Me,_hFileDialogHandle), _CustomControl._OpenDialogWindowRect)
'INSTANT VB TODO TASK: There is no equivalent to #pragma directives in VB.NET:
'#pragma warning restore 0197, 0414
							_CustomControl._hFileDialogHandle = _hFileDialogHandle

						End If
					Case Msg.WM_COMMAND
						Select Case NativeMethods.GetDlgCtrlID(m.LParam)
							Case CInt(ControlsId.ButtonOk) 'OK
							Case CInt(ControlsId.ButtonCancel) 'Cancel
							Case CInt(ControlsId.ButtonHelp) 'help
							Case 0
							Case Else
								Exit Select
						End Select 'switch(NativeMethods.GetDlgCtrlID(m.LParam)) ends
					Case Else
						Exit Select
				End Select 'switch ((Msg)m.Msg) ends
				MyBase.WndProc(m)
			End Sub
			#End Region
			#Region "Properties"
			#End Region
		End Class
		#End Region
	End Class

	#Region "Enums"
	Public Enum AddonWindowLocation
		BottomRight = 0
		Right = 1
		Bottom = 2
	End Enum

	Friend Enum ControlsId As Integer
		ButtonOk = &H1
		ButtonCancel = &H2
		ButtonHelp = &H40E '0x0000040e
		GroupFolder = &H440
		LabelFileType = &H441
		LabelFileName = &H442
		LabelLookIn = &H443
		DefaultView = &H461
		LeftToolBar = &H4A0
		ComboFileName = &H47c
		ComboFileType = &H470
		ComboFolder = &H471
		CheckBoxReadOnly = &H410
	End Enum
	#End Region

End Namespace
