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
	#Region "Base class"

	Partial Public Class FileDialogControlBase ', IMessageFilter
		Inherits UserControl
		#Region "Delegates"
		Public Delegate Sub PathChangedEventHandler(ByVal sender As IWin32Window, ByVal filePath As String)
		Public Delegate Sub FilterChangedEventHandler(ByVal sender As IWin32Window, ByVal index As Integer)
		#End Region

		#Region "Events"
		'for weird reasons the designer wants the events public not protected    
		<Category("FileDialogExtenders")> _
		Public Event EventFileNameChanged As PathChangedEventHandler
		<Category("FileDialogExtenders")> _
		Public Event EventFolderNameChanged As PathChangedEventHandler
		<Category("FileDialogExtenders")> _
		Public Event EventFilterChanged As FilterChangedEventHandler
		<Category("FileDialogExtenders")> _
		Public Event EventClosingDialog As CancelEventHandler
		#End Region

		#Region "Constants Declaration"
		Private Const UFLAGSHIDE As SetWindowPosFlags = SetWindowPosFlags.SWP_NOACTIVATE Or SetWindowPosFlags.SWP_NOOWNERZORDER Or SetWindowPosFlags.SWP_NOMOVE Or SetWindowPosFlags.SWP_NOSIZE Or SetWindowPosFlags.SWP_HIDEWINDOW
		#End Region

		#Region "Variables Declaration"
		Private _MSdialog As System.Windows.Forms.FileDialog
		Private _dlgWrapper As NativeWindow
		Private _StartLocation As AddonWindowLocation = AddonWindowLocation.Right
        Private _DefaultViewMode As FolderViewMode = FolderViewMode.Defaultm
		Private _hFileDialogHandle As IntPtr = IntPtr.Zero
		Private _FileDlgType As FileDialogType
		Private _InitialDirectory As String = String.Empty
		Private _Filter As String = "All files (*.*)|*.*"
		Private _DefaultExt As String = "jpg"
		Private _FileName As String = String.Empty
		Private _Caption As String = "Save"
		Private _OKCaption As String = "&Open"
		Private _FilterIndex As Integer = 1
		Private _AddExtension As Boolean = True
		Private _CheckFileExists As Boolean = True
		Private _EnableOkBtn As Boolean = True
		Private _DereferenceLinks As Boolean = True
		Private _ShowHelp As Boolean
		Private _OpenDialogWindowRect As New RECT()
		Private _hOKButton As IntPtr = IntPtr.Zero
		Private _hasRunInitMSDialog As Boolean
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")> _
		Private _hListViewPtr As IntPtr

		#End Region

		#Region "Constructors"
		Public Sub New()
			InitializeComponent()
		End Sub
		#End Region

		#Region "Properties"
		Private Shared _originalDlgHeight, _originalDlgWidth As UInteger

		Friend Shared Property OriginalDlgWidth() As UInteger
			Get
				Return FileDialogControlBase._originalDlgWidth
			End Get
			Set(ByVal value As UInteger)
				FileDialogControlBase._originalDlgWidth = value
			End Set
		End Property

		Friend Shared Property OriginalDlgHeight() As UInteger
			Get
				Return FileDialogControlBase._originalDlgHeight
			End Get
			Set(ByVal value As UInteger)
				FileDialogControlBase._originalDlgHeight = value
			End Set
		End Property

		<Browsable(False)> _
		Public ReadOnly Property FileDlgFileNames() As String()
			Get
				Return If(DesignMode, Nothing, MSDialog.FileNames)
			End Get
		End Property

		<Browsable(False)> _
		Public Property MSDialog() As FileDialog
			Set(ByVal value As FileDialog)
				_MSdialog = value
			End Set
			Get
				Return _MSdialog
			End Get
		End Property

		<Category("FileDialogExtenders"), DefaultValue(AddonWindowLocation.Right)> _
		Public Property FileDlgStartLocation() As AddonWindowLocation
			Get
				Return _StartLocation
			End Get
			Set(ByVal value As AddonWindowLocation)
				_StartLocation = value
				If DesignMode Then
					Me.Refresh()
				End If
			End Set
		End Property

		Private _OriginalCtrlSize As Size
		Friend Property OriginalCtrlSize() As Size
			Get
				Return _OriginalCtrlSize
			End Get
			Set(ByVal value As Size)
				_OriginalCtrlSize = value
			End Set
		End Property

        <Category("FileDialogExtenders"), DefaultValue(FolderViewMode.Defaultm)> _
  Public Property FileDlgDefaultViewMode() As FolderViewMode
            Get
                Return _DefaultViewMode
            End Get
            Set(ByVal value As FolderViewMode)
                _DefaultViewMode = value
            End Set
        End Property

		<Category("FileDialogExtenders"), DefaultValue(FileDialogType.OpenFileDlg)> _
		Public Property FileDlgType() As FileDialogType
			Get
				Return _FileDlgType
			End Get
			Set(ByVal value As FileDialogType)
				_FileDlgType = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue("")> _
		Public Property FileDlgInitialDirectory() As String
			Get
				Return If(DesignMode, _InitialDirectory, MSDialog.InitialDirectory)
			End Get
			Set(ByVal value As String)
				_InitialDirectory = value
				If (Not DesignMode) AndAlso MSDialog IsNot Nothing Then
					MSDialog.InitialDirectory = value
				End If
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue("")> _
		Public Property FileDlgFileName() As String
			Get
				Return If(DesignMode, _FileName, MSDialog.FileName)
			End Get
			Set(ByVal value As String)
				_FileName = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue("")> _
		Public Property FileDlgCaption() As String
			Get
				Return _Caption
			End Get
			Set(ByVal value As String)
				_Caption = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue("&Open")> _
		Public Property FileDlgOkCaption() As String
			Get
				Return _OKCaption
			End Get
			Set(ByVal value As String)
				_OKCaption = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue("jpg")> _
		Public Property FileDlgDefaultExt() As String
			Get
				Return If(DesignMode, _DefaultExt, MSDialog.DefaultExt)
			End Get
			Set(ByVal value As String)
				_DefaultExt = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue("All files (*.*)|*.*")> _
		Public Property FileDlgFilter() As String
			Get
				Return If(DesignMode, _Filter, MSDialog.Filter)
			End Get
			Set(ByVal value As String)
				_Filter = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue(1)> _
		Public Property FileDlgFilterIndex() As Integer
			Get
				Return If(DesignMode, _FilterIndex, MSDialog.FilterIndex)
			End Get
			Set(ByVal value As Integer)
				_FilterIndex = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue(True)> _
		Public Property FileDlgAddExtension() As Boolean
			Get
				Return If(DesignMode, _AddExtension, MSDialog.AddExtension)
			End Get
			Set(ByVal value As Boolean)
				_AddExtension = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue(True)> _
		Public Property FileDlgEnableOkBtn() As Boolean
			Get
				Return _EnableOkBtn
			End Get
			Set(ByVal value As Boolean)
				_EnableOkBtn = value
				If (Not DesignMode) AndAlso MSDialog IsNot Nothing AndAlso _hOKButton <> IntPtr.Zero Then
					Win32Types.NativeMethods.EnableWindow(_hOKButton, _EnableOkBtn)
				End If
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue(True)> _
		Public Property FileDlgCheckFileExists() As Boolean
			Get
				Return If(DesignMode, _CheckFileExists, MSDialog.CheckFileExists)
			End Get
			Set(ByVal value As Boolean)
				_CheckFileExists = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue(False)> _
		Public Property FileDlgShowHelp() As Boolean
			Get
				Return If(DesignMode, _ShowHelp, MSDialog.ShowHelp)
			End Get
			Set(ByVal value As Boolean)
				_ShowHelp = value
			End Set
		End Property

		<Category("FileDialogExtenders"), DefaultValue(True)> _
		Public Property FileDlgDereferenceLinks() As Boolean
			Get
				Return If(DesignMode, _DereferenceLinks, MSDialog.DereferenceLinks)
			End Get
			Set(ByVal value As Boolean)
				_DereferenceLinks = value
			End Set
		End Property
		#End Region

		#Region "Virtuals"
		'this is a hidden child window dor the whole dialog
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")> _
		Protected Overrides Sub OnLoad(ByVal e As EventArgs)
			MyBase.OnLoad(e)
			If Not DesignMode Then
				If MSDialog IsNot Nothing Then
					AddHandler MSDialog.FileOk, AddressOf FileDialogControlBase_ClosingDialog
					AddHandler MSDialog.Disposed, AddressOf FileDialogControlBase_DialogDisposed
					AddHandler MSDialog.HelpRequest, AddressOf FileDialogControlBase_HelpRequest
					FileDlgEnableOkBtn = _EnableOkBtn 'that's design time value
					NativeMethods.SetWindowText(New HandleRef(_dlgWrapper,_dlgWrapper.Handle), _Caption)
					'will work only for open dialog, save dialog will be overriden internally by windows
					NativeMethods.SetWindowText(New HandleRef(Me,_hOKButton), _OKCaption) 'SetDlgItemText fails too
					'bool res = NativeMethods.SetDlgItemText(NativeMethods.GetParent(Handle), (int)ControlsId.ButtonOk, FileDlgOkCaption);
				End If
			End If
		End Sub

		Public Sub SortViewByColumn(ByVal index As Integer)
			Try
				'handle of the "defView" --> container of the listView  
				Dim hWndWin As IntPtr = NativeMethods.FindWindowEx(_dlgWrapper.Handle, IntPtr.Zero, "SHELLDLL_DefView", "")

				If hWndWin <> IntPtr.Zero Then
					'change to details view
					NativeMethods.SendMessage(New HandleRef(Me, hWndWin), CInt(Msg.WM_COMMAND), CType(CInt(DefaultViewType.Details), IntPtr), IntPtr.Zero)

'					#Region " sort by date"
					Dim HDN_FIRST As Integer = (-300)
					Dim HDN_ITEMCLICKW As Integer = (HDN_FIRST - 22)

					'get the ListView//s hWnd
					Dim hWndLV As IntPtr = NativeMethods.FindWindowEx(hWndWin, IntPtr.Zero, "SysListView32", IntPtr.Zero)
					'get the ColumnHeaders hWnd
					Dim hWndColHd As IntPtr = NativeMethods.FindWindowEx(hWndLV, IntPtr.Zero, "SysHeader32", IntPtr.Zero)

					'now click on column 3 to sort for date
					Dim NMH As New NMHEADER()
					NMH.hdr.hwndFrom = hWndColHd
					NMH.hdr.code = CUInt(HDN_ITEMCLICKW)
					NMH.iItem = index
					NMH.iButton = 0

					' Initialize unmanged memory to hold the struct.
					Dim ptrNMH As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(NMH))
					Try

						' Copy the struct to unmanaged memory.
						Marshal.StructureToPtr(NMH, ptrNMH, False)

						NativeMethods.SendMessage(New HandleRef(Me, hWndLV), CUInt(Msg.WM_NOTIFY), IntPtr.Zero, ptrNMH)
						'click again for descending order = newest files first
						NativeMethods.SendMessage(New HandleRef(Me, hWndLV), CUInt(Msg.WM_NOTIFY), IntPtr.Zero, ptrNMH)
					Finally
						' Free the unmanaged memory.
						Marshal.FreeHGlobal(ptrNMH)
					End Try



					'//if wanted give the dialog a larger size here
					'If DialogXSize > 0 And DialogYSize > 0 Then
					'   SetWindowPos hWndDlg, 0&, 0&, 0&, DialogXSize, DialogYSize, 0&
					'End If
					'}
'					#End Region
				End If
			Catch
			End Try
		End Sub
		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If IsDisposed Then
				Return
			End If
			If MSDialog IsNot Nothing Then
				RemoveHandler MSDialog.FileOk, AddressOf FileDialogControlBase_ClosingDialog
				RemoveHandler MSDialog.Disposed, AddressOf FileDialogControlBase_DialogDisposed
				RemoveHandler MSDialog.HelpRequest, AddressOf FileDialogControlBase_HelpRequest
				MSDialog.Dispose()
				MSDialog = Nothing
			End If
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		Public Overridable Sub OnFileNameChanged(ByVal sender As IWin32Window, ByVal fileName As String)
			RaiseEvent EventFileNameChanged(sender, fileName)
		End Sub

		Public Sub OnFolderNameChanged(ByVal sender As IWin32Window, ByVal folderName As String)
			RaiseEvent EventFolderNameChanged(sender, folderName)
			UpdateListView()
		End Sub

		Private Sub UpdateListView()
			_hListViewPtr = Win32Types.NativeMethods.GetDlgItem(_hFileDialogHandle, CInt(ControlsId.DefaultView))
            If FileDlgDefaultViewMode <> FolderViewMode.Defaultm AndAlso _hFileDialogHandle <> IntPtr.Zero Then
                NativeMethods.SendMessage(New HandleRef(Me, _hListViewPtr), CInt(Msg.WM_COMMAND), CType(CInt(Fix(FileDlgDefaultViewMode)), IntPtr), IntPtr.Zero)
                If FileDlgDefaultViewMode = FolderViewMode.Details OrElse FileDlgDefaultViewMode = FolderViewMode.List Then
                    SortViewByColumn(0)
                End If
            End If
		End Sub

		Friend Sub OnFilterChanged(ByVal sender As IWin32Window, ByVal index As Integer)
			RaiseEvent EventFilterChanged(sender, index)
		End Sub

		Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
			If DesignMode Then
				Dim gr As Graphics = e.Graphics
					Dim hb As HatchBrush = Nothing
					Dim p As Pen = Nothing
					Try
						Select Case Me.FileDlgStartLocation
							Case AddonWindowLocation.Right
								hb = New System.Drawing.Drawing2D.HatchBrush(HatchStyle.NarrowHorizontal, Color.Black, Color.Red)
								p = New Pen(hb, 5)
								gr.DrawLine(p, 0, 0, 0, Me.Height)
							Case AddonWindowLocation.Bottom
								hb = New System.Drawing.Drawing2D.HatchBrush(HatchStyle.NarrowVertical, Color.Black, Color.Red)
								p = New Pen(hb, 5)
								gr.DrawLine(p, 0, 0, Me.Width, 0)
							Case Else
								hb = New System.Drawing.Drawing2D.HatchBrush(HatchStyle.Sphere, Color.Black, Color.Red)
								p = New Pen(hb, 5)
								gr.DrawLine(p, 0, 0, 4, 4)
						End Select
					Finally
						If p IsNot Nothing Then
							p.Dispose()
						End If
						If hb IsNot Nothing Then
							hb.Dispose()
						End If
					End Try
			End If
			MyBase.OnPaint(e)
		End Sub


		#End Region

		#Region "Methods"
		Public Function ShowDialog() As DialogResult
			Return ShowDialog(Nothing)
		End Function
		Protected Overridable Sub OnPrepareMSDialog()
			InitMSDialog()
		End Sub
		Private Sub InitMSDialog()
			MSDialog.InitialDirectory = If(_InitialDirectory.Length = 0, Path.GetDirectoryName(Application.ExecutablePath), _InitialDirectory)
			MSDialog.AddExtension = _AddExtension
			MSDialog.Filter = _Filter
			MSDialog.FilterIndex = _FilterIndex
			MSDialog.CheckFileExists = _CheckFileExists
			MSDialog.DefaultExt = _DefaultExt
			MSDialog.FileName = _FileName
			MSDialog.DereferenceLinks = _DereferenceLinks
			MSDialog.ShowHelp = _ShowHelp
			_hasRunInitMSDialog = True
		End Sub

		Public Function ShowDialog(ByVal owner As IWin32Window) As DialogResult
			Dim returnDialogResult As DialogResult = DialogResult.Cancel
			If Me.IsDisposed Then
				Return returnDialogResult
			End If
			If owner Is Nothing OrElse owner.Handle = IntPtr.Zero Then
				Dim wr As New WindowWrapper(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle)
				owner = wr
			End If
			OriginalCtrlSize = Me.Size
            If FileDlgType = FileDialogType.OpenFileDlg Then
                _MSdialog = New OpenFileDialog()
            Else
                _MSdialog = New SaveFileDialog()
            End If
			_dlgWrapper = New WholeDialogWrapper(Me)
			OnPrepareMSDialog()
			If Not _hasRunInitMSDialog Then
				InitMSDialog()
			End If
			Try
				Dim AutoUpgradeInfo As System.Reflection.PropertyInfo = MSDialog.GetType().GetProperty("AutoUpgradeEnabled")
				If AutoUpgradeInfo IsNot Nothing Then
					AutoUpgradeInfo.SetValue(MSDialog, False, Nothing)
				End If
				returnDialogResult = _MSdialog.ShowDialog(owner)
			' Sometimes if you open a animated .gif on the preview and the Form is closed, .Net class throw an exception
			' Lets ignore this exception and keep closing the form.
			Catch e1 As ObjectDisposedException
			Catch ex As Exception
				MessageBox.Show("unable to get the modal dialog handle", ex.Message)
			End Try
			Return returnDialogResult
		End Function

		Friend Function ShowDialogExt(ByVal fdlg As FileDialog, ByVal owner As IWin32Window) As DialogResult
			Dim returnDialogResult As DialogResult = DialogResult.Cancel
			If Me.IsDisposed Then
				Return returnDialogResult
			End If
			If owner Is Nothing OrElse owner.Handle = IntPtr.Zero Then
				Dim wr As New WindowWrapper(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle)
				owner = wr
			End If
			OriginalCtrlSize = Me.Size
			MSDialog = fdlg
			_dlgWrapper = New WholeDialogWrapper(Me)

			Try
				Dim AutoUpgradeInfo As System.Reflection.PropertyInfo = MSDialog.GetType().GetProperty("AutoUpgradeEnabled")
				If AutoUpgradeInfo IsNot Nothing Then
					AutoUpgradeInfo.SetValue(MSDialog, False, Nothing)
				End If
				returnDialogResult = _MSdialog.ShowDialog(owner)
			' Sometimes if you open a animated .gif on the preview and the Form is closed, .Net class throw an exception
			' Lets ignore this exception and keep closing the form.
			Catch e1 As ObjectDisposedException
			Catch ex As Exception
				MessageBox.Show("unable to get the modal dialog handle", ex.Message)
			End Try
			Return returnDialogResult
		End Function
		#End Region


		#Region "event handlers"
		Private Sub FileDialogControlBase_DialogDisposed(ByVal sender As Object, ByVal e As EventArgs)
			Dispose(True)
		End Sub

		Private Sub FileDialogControlBase_ClosingDialog(ByVal sender As Object, ByVal e As CancelEventArgs)
			RaiseEvent EventClosingDialog(Me, e)
		End Sub


		Private Sub FileDialogControlBase_HelpRequest(ByVal sender As Object, ByVal e As EventArgs)
			'this is a virtual call that should call the event in the subclass
            OnHelpRequested(New HelpEventArgs(New System.Drawing.Point()))
		End Sub

		#End Region
		#Region "helper types"

		Public Class WindowWrapper
			Implements System.Windows.Forms.IWin32Window
			Public Sub New(ByVal handle As IntPtr)
				_hwnd = handle
			End Sub

			Public ReadOnly Property Handle() As IntPtr Implements System.Windows.Forms.IWin32Window.Handle
				Get
					Return _hwnd
				End Get
			End Property

			Private _hwnd As IntPtr
		End Class
		#End Region
	End Class
	#End Region


End Namespace