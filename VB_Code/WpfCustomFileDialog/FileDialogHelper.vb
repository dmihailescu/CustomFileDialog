' Copyright © Decebal Mihailescu 2015
' Some code was obtained by reverse engineering the PresentationFramework.dll using Reflector

' All rights reserved.
' This code is released under The Code Project Open License (CPOL) 1.02
' The full licensing terms are available at http://www.codeproject.com/info/cpol10.aspx
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
' KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
' IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
' PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
' REMAINS UNCHANGED.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows.Controls

Imports MS.Win32
Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Permissions
Imports System.Windows
Imports System.Reflection
Imports System.Windows.Interop

Namespace WpfCustomFileDialog

	#Region "Delegates"
	Public Delegate Sub PathChangedEventHandler(ByVal sender As IFileDlgExt, ByVal filePath As String)
	Public Delegate Sub FilterChangedEventHandler(ByVal sender As IFileDlgExt, ByVal index As Integer)
	#End Region



	Public Enum AddonWindowLocation
		BottomRight = 0
		Right = 1
		Bottom = 2

	End Enum

	Public MustInherit Partial Class FileDialogExt(Of T As {ContentControl, IWindowExt, New})
		Inherits Microsoft.Win32.CommonDialog
		Implements IFileDlgExt
		#Region "IWin32Window Members"

		Public ReadOnly Property Handle() As IntPtr Implements System.Windows.Interop.IWin32Window.Handle
			Get
				Return Me._hwndFileDialog
			End Get
		End Property

		#End Region

		#Region "IFileDlgExt Members"

		#Region "Events"


		Private Custom Event EventFileNameChanged As PathChangedEventHandler Implements IFileDlgExt.EventFileNameChanged
			AddHandler(ByVal value As PathChangedEventHandler)
				If _eventFileNameChangedEvent Is Nothing Then
					_eventFileNameChangedEvent = value
				Else
					SyncLock _eventFileNameChangedEvent
						AddHandler _eventFileNameChanged, value
					End SyncLock
				End If
			End AddHandler
			RemoveHandler(ByVal value As PathChangedEventHandler)
				If _eventFileNameChangedEvent IsNot Nothing AndAlso (_eventFileNameChangedEvent.GetInvocationList().Length > 0) Then
					SyncLock _eventFileNameChangedEvent
						RemoveHandler _eventFileNameChanged, value
					End SyncLock
				End If
			End RemoveHandler
			RaiseEvent(ByVal sender As IFileDlgExt, ByVal filePath As String)
			End RaiseEvent
		End Event
		Private Event _eventFileNameChanged As PathChangedEventHandler

		Private Custom Event EventFolderNameChanged As PathChangedEventHandler Implements IFileDlgExt.EventFolderNameChanged
			AddHandler(ByVal value As PathChangedEventHandler)
				If _eventFolderNameChangedEvent Is Nothing Then
					_eventFolderNameChangedEvent = value
				Else
					SyncLock _eventFolderNameChangedEvent
						AddHandler _eventFolderNameChanged, value
					End SyncLock
				End If
			End AddHandler
			RemoveHandler(ByVal value As PathChangedEventHandler)
				If _eventFolderNameChangedEvent IsNot Nothing AndAlso (_eventFolderNameChangedEvent.GetInvocationList().Length > 0) Then
					SyncLock _eventFolderNameChangedEvent
						RemoveHandler _eventFolderNameChanged, value
					End SyncLock
				End If
			End RemoveHandler
			RaiseEvent(ByVal sender As IFileDlgExt, ByVal filePath As String)
			End RaiseEvent
		End Event
		Private Event _eventFolderNameChanged As PathChangedEventHandler


		Private Custom Event EventFilterChanged As FilterChangedEventHandler Implements IFileDlgExt.EventFilterChanged
			AddHandler(ByVal value As FilterChangedEventHandler)
				If _eventFilterChangedEvent Is Nothing Then
					_eventFilterChangedEvent = value
				Else
					SyncLock _eventFilterChangedEvent
						AddHandler _eventFilterChanged, value
					End SyncLock
				End If
			End AddHandler
			RemoveHandler(ByVal value As FilterChangedEventHandler)
				If _eventFilterChangedEvent IsNot Nothing AndAlso (_eventFilterChangedEvent.GetInvocationList().Length > 0) Then
					SyncLock _eventFilterChangedEvent
						RemoveHandler _eventFilterChanged, value
					End SyncLock
				End If
			End RemoveHandler
			RaiseEvent(ByVal sender As IFileDlgExt, ByVal index As Integer)
			End RaiseEvent
		End Event
		Private Event _eventFilterChanged As FilterChangedEventHandler
		#End Region
		Private _location As AddonWindowLocation
		Public Property FileDlgStartLocation() As AddonWindowLocation Implements IFileDlgExt.FileDlgStartLocation
			Get
				Return _location
			End Get
			Set(ByVal value As AddonWindowLocation)
				_location = value
			End Set
		End Property

		Private _EnableOkBtn As Boolean = True

		Public Property FileDlgEnableOkBtn() As Boolean Implements IFileDlgExt.FileDlgEnableOkBtn
			Get
				Return _EnableOkBtn
			End Get
			Set(ByVal value As Boolean)
				_EnableOkBtn = value
				If HandleRef.ToIntPtr(_hOKButton) <> IntPtr.Zero Then
					NativeMethods.EnableWindow(_hOKButton, _EnableOkBtn)
				End If
			End Set
		End Property
		#End Region
		Private _childWnd As T
		Friend Shared ReadOnly MSG_POST_CREATION As UInteger = NativeMethods.RegisterWindowMessage("Post Creation Message")
		Public Property ChildWnd() As T
			Get
				Return _childWnd
			End Get
			Set(ByVal value As T)
				_childWnd = value
			End Set
		End Property
		Private _source As HwndSource

		Private _OriginalRect As New RECT()
		Private _ComboFolders As IntPtr
		Private _ComboFoldersInfo As WINDOWINFO
		Private _hGroupButtons As IntPtr
		Private _GroupButtonsInfo As WINDOWINFO
		Private _hComboFileName As IntPtr
		Private _ComboFileNameInfo As WINDOWINFO
		Private _hComboExtensions As IntPtr
		Private _ComboExtensionsInfo As WINDOWINFO
		Private _hOKButton As HandleRef
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

		Private _OKCaption As String = "&Open"
		Private Sub SetChildStyle(ByVal isChild As Boolean)
			Dim styles As Long = CLng(NativeMethods.GetWindowLongPtr(New HandleRef(_childWnd, _source.Handle), GWL.GWL_STYLE))
			If isChild Then
				If IntPtr.Size = 4 Then
					styles = styles Or System.Convert.ToInt64(NativeMethods.WindowStyles.WS_CHILD)
				Else
					styles = styles Or CLng(Fix(NativeMethods.WindowStyles.WS_CHILD))
				End If
			Else
				Dim nonChild As NativeMethods.WindowStyles = CType(&HffffffffL, NativeMethods.WindowStyles) Xor NativeMethods.WindowStyles.WS_CHILD
				If IntPtr.Size = 4 Then
					styles = styles And System.Convert.ToInt64(nonChild)
				Else
					styles = styles And CLng(Fix(nonChild))
				End If
			End If
			NativeMethods.CriticalSetWindowLong(New HandleRef(Me, _source.Handle), CInt(GWL.GWL_STYLE), New IntPtr(styles))
		End Sub

		Private Sub SetFixedSize(ByVal hwnd As IntPtr)
			Dim styles As Long = CLng(NativeMethods.GetWindowLongPtr(New HandleRef(Me, hwnd), GWL.GWL_STYLE))

				If IntPtr.Size = 4 Then

					styles = styles Xor System.Convert.ToInt64(NativeMethods.WindowStyles.WS_THICKFRAME)
				Else
					styles = styles Or CLng(Fix(NativeMethods.WindowStyles.WS_BORDER))
				End If
			NativeMethods.CriticalSetWindowLong(New HandleRef(Me, hwnd), CInt(GWL.GWL_STYLE), New IntPtr(styles))
		End Sub

		Public Property FileDlgOkCaption() As String Implements IFileDlgExt.FileDlgOkCaption
			Get
				Return _OKCaption
			End Get
			Set(ByVal value As String)
				_OKCaption = value
				If HandleRef.ToIntPtr(_hOKButton) <> IntPtr.Zero Then
					NativeMethods.SetWindowText(_hOKButton, _OKCaption)
				End If
			End Set
		End Property
		Private _bFixedSize As Boolean = True
		Public WriteOnly Property FixedSize() As Boolean Implements IFileDlgExt.FixedSize
			Set(ByVal value As Boolean)
				_bFixedSize = value
			End Set
		End Property

		Private _DefaultViewMode As NativeMethods.FolderViewMode
		Public Property FileDlgDefaultViewMode() As NativeMethods.FolderViewMode Implements IFileDlgExt.FileDlgDefaultViewMode
			Get
				Return _DefaultViewMode
			End Get
			Set(ByVal value As NativeMethods.FolderViewMode)
				_DefaultViewMode = value
			End Set
		End Property

		Private Sub PopulateWindowsHandlers()
			NativeMethods.EnumChildWindows(_hwndFileDialog, New NativeMethods.EnumWindowsCallBack(AddressOf FileDialogEnumWindowCallBack), 0)
		End Sub
		Private _hListViewPtr As IntPtr
		Private Sub UpdateListView()
			Dim _hListViewPtr As IntPtr = NativeMethods.GetDlgItem(Me._hwndFileDialog, CInt(Fix(NativeMethods.ControlsId.DefaultView)))
			If IntPtr.Zero = _hListViewPtr Then
				UpdateListView(_hListViewPtr)
			End If
		End Sub
		Private Sub UpdateListView(ByVal hListViewPtr As IntPtr)
			If FileDlgDefaultViewMode <> NativeMethods.FolderViewMode.Default AndAlso hListViewPtr <> IntPtr.Zero Then
				NativeMethods.SendMessage(New HandleRef(Me, hListViewPtr), CInt(Fix(NativeMethods.Msg.WM_COMMAND)), CType(CInt(Fix(FileDlgDefaultViewMode)), IntPtr), IntPtr.Zero)
			End If
		End Sub
		Private _oldPath As String
		Private Sub OnPathChanged(ByVal sender As IFileDlgExt, ByVal pathName As String)

			If String.IsNullOrEmpty(System.IO.Path.GetFileName(pathName)) OrElse _oldPath = pathName Then
				Return
			End If
			_oldPath = pathName
			If System.IO.File.Exists(pathName) Then
				RaiseEvent _eventFileNameChanged(sender, pathName)
			ElseIf System.IO.Directory.Exists(pathName) Then
				RaiseEvent _eventFolderNameChanged(sender, pathName)
			End If

		End Sub
		Private _oldFilterIndex As Integer = -1
		Private Sub OnFilterChanged(ByVal sender As IFileDlgExt, ByVal index As Integer)
			If _oldFilterIndex <> index Then
				_oldFilterIndex = index
				RaiseEvent _eventFilterChanged(sender, index)
			End If
		End Sub

		Private Function FileDialogEnumWindowCallBack(ByVal hwnd As IntPtr, ByVal lParam As Integer) As Boolean
			Dim className As New StringBuilder(256)
			NativeMethods.GetClassName(New HandleRef(Me, hwnd), className, className.Capacity)
			Dim controlID As Integer = NativeMethods.GetDlgCtrlID(hwnd)
			Dim windowInfo As WINDOWINFO
			NativeMethods.GetWindowInfo(hwnd, windowInfo)

			' Dialog Window
			If className.ToString().StartsWith("#32770") Then
				_hwndFileDialogEmbedded = hwnd

				Return True
			End If

			Select Case CType(controlID, NativeMethods.ControlsId)
				'not available at startup
				Case NativeMethods.ControlsId.DefaultView
					UpdateListView(hwnd)
				Case NativeMethods.ControlsId.ComboFolder
					_ComboFolders = hwnd
					_ComboFoldersInfo = windowInfo
				Case NativeMethods.ControlsId.ComboFileType
					_hComboExtensions = hwnd
					_ComboExtensionsInfo = windowInfo
				Case NativeMethods.ControlsId.ComboFileName
					If className.ToString().ToLower() = "comboboxex32" Then
						_hComboFileName = hwnd
						_ComboFileNameInfo = windowInfo
					End If
				Case NativeMethods.ControlsId.GroupFolder
					_hGroupButtons = hwnd
					_GroupButtonsInfo = windowInfo
				Case NativeMethods.ControlsId.LeftToolBar
					_hToolBarFolders = hwnd
					_ToolBarFoldersInfo = windowInfo
				Case NativeMethods.ControlsId.ButtonOk
					_hOKButton = New HandleRef(Me, hwnd)
					_OKButtonInfo = windowInfo
				Case NativeMethods.ControlsId.ButtonCancel
					_hCancelButton = hwnd
					_CancelButtonInfo = windowInfo
				Case NativeMethods.ControlsId.ButtonHelp
					_hHelpButton = hwnd
					_HelpButtonInfo = windowInfo
				Case NativeMethods.ControlsId.CheckBoxReadOnly
					_hChkReadOnly = hwnd
					_ChkReadOnlyInfo = windowInfo
				Case NativeMethods.ControlsId.LabelFileName
					_hLabelFileName = hwnd
					_LabelFileNameInfo = windowInfo
				Case NativeMethods.ControlsId.LabelFileType
					_hLabelFileType = hwnd
					_LabelFileTypeInfo = windowInfo
			End Select

			Return True
		End Function

		Public Shared ReadOnly InvalidIntPtr As IntPtr = CType(-1, IntPtr)

		Friend Shared ReadOnly INVALID_HANDLE_VALUE As New IntPtr(-1)

		Private Sub SetProperty(ByVal target As Object, ByVal fieldName As String, ByVal value As Object)
			Dim type As Type = target.GetType()
			Dim mi As PropertyInfo = type.GetProperty(fieldName, BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.SetProperty)
			mi.SetValue(target, value, Nothing)
		End Sub
		Protected Function InvokeMethod(ByVal instance As Object, ByVal methodName As String, ParamArray ByVal args() As Object) As Object
			Dim type As Type = If((TypeOf instance Is Type), TryCast(instance, Type), instance.GetType())
			Dim mi As MethodInfo = type.GetMethod(methodName,If((TypeOf instance Is Type), BindingFlags.NonPublic Or BindingFlags.Static, BindingFlags.NonPublic Or BindingFlags.Instance)) 'invoking the method
			'null- no parameter for the function [or] we can pass the array of parameters            
			Return If((args Is Nothing OrElse args.Length = 0), mi.Invoke(instance, Nothing), mi.Invoke(instance, args))
		End Function
		Protected Function GetProperty(ByVal target As Object, ByVal fieldName As String) As Object
            Dim type As Type = target.GetType()
            Dim mi As PropertyInfo = type.GetProperty(fieldName, BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.GetProperty)
            If mi Is Nothing Then
                Return Nothing
            End If
            Return mi.GetValue(target, Nothing)
		End Function

        <SecurityCritical(SecurityCriticalScope.Everything)> _
        Friend Class UnicodeCharBuffer
            Inherits CharBuffer
            ' Fields
            Friend buffer() As Char
            Friend offset As Integer

            ' Methods
            Friend Sub New(ByVal size As Integer)
                Me.buffer = New Char(size - 1) {}
            End Sub

            Friend Overrides Function AllocCoTaskMem() As IntPtr
                Dim destination As IntPtr = Marshal.AllocCoTaskMem(Me.buffer.Length * 2)
                Marshal.Copy(Me.buffer, 0, destination, Me.buffer.Length)
                Return destination
            End Function

            Friend Overrides Function GetString() As String
                Dim offset As Integer = Me.offset
                Do While (offset < Me.buffer.Length AndAlso (Me.buffer(offset) <> ControlChars.NullChar))
                    offset += 1
                Loop
                Dim str As New String(Me.buffer, Me.offset, offset - Me.offset)
                If offset < Me.buffer.Length Then
                    offset += 1
                End If
                Me.offset = offset
                Return str
            End Function

            Friend Overrides Sub PutCoTaskMem(ByVal ptr As IntPtr)
                Marshal.Copy(ptr, Me.buffer, 0, Me.buffer.Length)
                Me.offset = 0
            End Sub

            Friend Overrides Sub PutString(ByVal s As String)
                Dim count As Integer = Math.Min(s.Length, Me.buffer.Length - Me.offset)
                s.CopyTo(0, Me.buffer, Me.offset, count)
                Me.offset += count
                If Me.offset < Me.buffer.Length Then
                    Me.buffer(Me.offset) = ControlChars.NullChar
                    Me.offset += 1
                End If
            End Sub

            ' Properties
            Friend Overrides ReadOnly Property Length() As Integer
                Get
                    Return Me.buffer.Length
                End Get
            End Property
        End Class

		Friend Class AnsiCharBuffer
			Inherits CharBuffer
			' Fields
			Friend buffer() As Byte
			Friend offset As Integer

			' Methods
			Friend Sub New(ByVal size As Integer)
				Me.buffer = New Byte(size - 1){}
			End Sub

			Friend Overrides Function AllocCoTaskMem() As IntPtr
				Dim destination As IntPtr = Marshal.AllocCoTaskMem(Me.buffer.Length)
				Marshal.Copy(Me.buffer, 0, destination, Me.buffer.Length)
				Return destination
			End Function

			Friend Overrides Function GetString() As String
				Dim offset As Integer = Me.offset
				Do While (offset < Me.buffer.Length) AndAlso (Me.buffer(offset) <> 0)
					offset += 1
				Loop
				Dim str As String = Encoding.Default.GetString(Me.buffer, Me.offset, offset - Me.offset)
				If offset < Me.buffer.Length Then
					offset += 1
				End If
				Me.offset = offset
				Return str
			End Function

			Friend Overrides Sub PutCoTaskMem(ByVal ptr As IntPtr)
				Marshal.Copy(ptr, Me.buffer, 0, Me.buffer.Length)
				Me.offset = 0
			End Sub

			Friend Overrides Sub PutString(ByVal s As String)
				Dim bytes() As Byte = Encoding.Default.GetBytes(s)
				Dim length As Integer = Math.Min(bytes.Length, Me.buffer.Length - Me.offset)
				Array.Copy(bytes, 0, Me.buffer, Me.offset, length)
				Me.offset += length
				If Me.offset < Me.buffer.Length Then
					Me.buffer(Me.offset) = 0
					Me.offset += 1
				End If
			End Sub

			' Properties
			Friend Overrides ReadOnly Property Length() As Integer
				Get
					Return Me.buffer.Length
				End Get
			End Property
		End Class

		Friend MustInherit Class CharBuffer
			' Methods
			Protected Sub New()
			End Sub

			Friend MustOverride Function AllocCoTaskMem() As IntPtr
			<SecurityCritical> _
			Friend Shared Function CreateBuffer(ByVal size As Integer) As CharBuffer
				If Marshal.SystemDefaultCharSize = 1 Then
					Return New AnsiCharBuffer(size)
				End If
				Return New UnicodeCharBuffer(size)
			End Function

			Friend MustOverride Function GetString() As String
			Friend MustOverride Sub PutCoTaskMem(ByVal ptr As IntPtr)
			Friend MustOverride Sub PutString(ByVal s As String)

			' Properties
			Friend MustOverride ReadOnly Property Length() As Integer
		End Class

		Private Sub InitControls()
			PopulateWindowsHandlers()
			If HandleRef.ToIntPtr(_hOKButton) <> IntPtr.Zero Then
				NativeMethods.EnableWindow(_hOKButton, _EnableOkBtn)
				NativeMethods.SetWindowText(_hOKButton, _OKCaption)
			End If
		End Sub

        'RECT _originalDialogClientRect = new RECT();
        ''' <summary>
        ''' WndProc for the custom child window
        ''' </summary>
        ''' <param name="hwnd"></param>
        ''' <param name="msg"></param>
        ''' <param name="wParam"></param>
        ''' <param name="lParam"></param>
        ''' <param name="handled"></param>
        ''' <returns></returns>
		Private Function EmbededWndProc(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr, ByRef handled As Boolean) As IntPtr
			Dim hres As IntPtr = IntPtr.Zero

			Const DLGC_WANTALLKEYS As Integer = &H0004

			Select Case CType(msg, NativeMethods.Msg)
				Case NativeMethods.Msg.WM_ACTIVATE

				Case NativeMethods.Msg.WM_SHOWWINDOW
				Case NativeMethods.Msg.WM_DESTROY
				Case NativeMethods.Msg.WM_SYSKEYDOWN

					SetChildStyle(True)

				Case NativeMethods.Msg.WM_SYSKEYUP

					SetChildStyle(False)


				Case NativeMethods.Msg.WM_KEYDOWN, NativeMethods.Msg.WM_KEYUP
					handled = False
				'see http://support.microsoft.com/kb/83302
				Case NativeMethods.Msg.WM_GETDLGCODE
					If lParam <> IntPtr.Zero Then
						hres = New IntPtr(DLGC_WANTALLKEYS)
					End If
					handled = True

				Case NativeMethods.Msg.WM_NCDESTROY

				Case NativeMethods.Msg.WM_WINDOWPOSCHANGING


				Case NativeMethods.Msg.WM_SIZING
				Case NativeMethods.Msg.WM_SIZE

					Exit Select

			End Select 'switch ends

			Return hres
		End Function

		Private Function EmbededCtrlProc(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr, ByRef handled As Boolean) As IntPtr
			Dim hres As IntPtr = IntPtr.Zero

			Const DLGC_WANTALLKEYS As Integer = &H0004

			Select Case CType(msg, NativeMethods.Msg)
				Case NativeMethods.Msg.WM_ACTIVATE

				Case NativeMethods.Msg.WM_SHOWWINDOW
				Case NativeMethods.Msg.WM_DESTROY
				Case NativeMethods.Msg.WM_KEYDOWN, NativeMethods.Msg.WM_KEYUP
					handled = False
				'see http://support.microsoft.com/kb/83302
				Case NativeMethods.Msg.WM_GETDLGCODE
					If lParam <> IntPtr.Zero Then
						hres = New IntPtr(DLGC_WANTALLKEYS)
						'const int DLGC_WANTCHARS = 0x00000080;
						'const int DLGC_WANTTAB = 0x00000002;

						'MSG dlgmsg = (MSG)Marshal.PtrToStructure(lParam, typeof(MSG));
						'if ((int)dlgmsg.wParam == 9)
						'{
						'    hres = (IntPtr)DLGC_WANTTAB;
						'}
						'else
						'{
						'    hres = (IntPtr)DLGC_WANTCHARS;
						'}
					End If
					handled = True

				Case NativeMethods.Msg.WM_NCDESTROY

				Case NativeMethods.Msg.WM_WINDOWPOSCHANGING


				Case NativeMethods.Msg.WM_SIZING
				Case NativeMethods.Msg.WM_SIZE

					Exit Select

			End Select 'switch ends

			Return hres
		End Function
		Private Sub ShowChild()
			_childWnd = New T()
			Try
				_childWnd.ParentDlg = Me
			Catch
				Return
			End Try

			Dim dialogWindowRect As New RECT()
			Dim dialogClientRect As New RECT()

			Dim size As New Size(dialogWindowRect.Width, dialogWindowRect.Height)
			NativeMethods.GetClientRect(New HandleRef(Me, _hwndFileDialog), dialogClientRect)
			NativeMethods.GetWindowRect(New HandleRef(Me, _hwndFileDialog), dialogWindowRect)
			Dim dy As Integer = CInt(Fix(dialogWindowRect.Height - dialogClientRect.Height))
			Dim dx As Integer = CInt(Fix(dialogWindowRect.Width - dialogClientRect.Width))
			size = New Size(dialogWindowRect.Width, dialogWindowRect.Height)

			If TypeOf _childWnd Is Window Then
				Dim wnd As Window = TryCast(_childWnd, Window)
				wnd.WindowStyle = WindowStyle.None
				wnd.ResizeMode = ResizeMode.NoResize 'will fix the child window!!
				wnd.ShowInTaskbar = False
				'won't flash on screen
				wnd.WindowStartupLocation = WindowStartupLocation.Manual
				wnd.Left = -10000
				wnd.Top = -10000
				AddHandler wnd.SourceInitialized, Function(sender, e) AnonymousMethod1(sender, e)

				wnd.Show()

				Dim styles As Long = CLng(NativeMethods.GetWindowLongPtr(New HandleRef(_childWnd, _source.Handle), GWL.GWL_STYLE))
				If IntPtr.Size = 4 Then
					styles = styles Or System.Convert.ToInt64(NativeMethods.WindowStyles.WS_CHILD)
					styles = styles Xor System.Convert.ToInt64(NativeMethods.WindowStyles.WS_SYSMENU)
				Else
					styles = styles Or CLng(Fix(NativeMethods.WindowStyles.WS_CHILD))
					styles = styles Xor CLng(Fix(NativeMethods.WindowStyles.WS_SYSMENU))
				End If
				NativeMethods.CriticalSetWindowLong(New HandleRef(Me, _source.Handle), CInt(GWL.GWL_STYLE), New IntPtr(styles))

				' Everything is ready, now lets change the parent
				NativeMethods.SetParent(New HandleRef(_childWnd, _source.Handle), New HandleRef(Me, _hwndFileDialog))
			Else
				'see http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/b2cff333-cbd9-4742-beba-ba19a15eeaee
				Dim ctrl As ContentControl = TryCast(_childWnd, ContentControl)
				Dim parameters As New HwndSourceParameters("WPFDlgControl", CInt(Fix(ctrl.Width)), CInt(Fix(ctrl.Height)))
				parameters.WindowStyle = CInt(Fix(NativeMethods.WindowStyles.WS_VISIBLE)) Or CInt(Fix(NativeMethods.WindowStyles.WS_CHILD))
				parameters.SetPosition(CInt(Fix(_OriginalRect.Width)), CInt(Fix(_OriginalRect.Height)))
				parameters.ParentWindow = _hwndFileDialog
				parameters.AdjustSizingForNonClientArea = False
				Select Case Me.FileDlgStartLocation
					Case AddonWindowLocation.Right
						parameters.PositionX = CInt(Fix(_OriginalRect.Width)) - dx\2
						parameters.PositionY = 0
						If ctrl.Height < _OriginalRect.Height - dy Then
							parameters.Height = CInt(Fix(_OriginalRect.Height)) - dy
							ctrl.Height = parameters.Height
						End If

					Case AddonWindowLocation.Bottom
						parameters.PositionX = 0
						parameters.PositionY = CInt(Fix(_OriginalRect.Height - dy + dx\2))
						If ctrl.Width < _OriginalRect.Width - dx Then
							parameters.Width = CInt(Fix(_OriginalRect.Width)) - dx
							ctrl.Width = parameters.Width
						End If
					Case AddonWindowLocation.BottomRight
						parameters.PositionX = CInt(Fix(_OriginalRect.Width)) - dx\2
						parameters.PositionY = CInt(Fix(_OriginalRect.Height - dy + dx\2))
				End Select

				_source = New HwndSource(parameters)
				_source.CompositionTarget.BackgroundColor = System.Windows.Media.Colors.LightGray
				_source.RootVisual = TryCast(_childWnd, System.Windows.Media.Visual)
				_source.AddHook(New HwndSourceHook(AddressOf EmbededCtrlProc))
			End If
			Select Case Me.FileDlgStartLocation
				Case AddonWindowLocation.Right
					size.Width = _OriginalRect.Width + _childWnd.Width
					size.Height = _OriginalRect.Height

				Case AddonWindowLocation.Bottom
					size.Width = _OriginalRect.Width
					size.Height = _OriginalRect.Height + _childWnd.Height
				Case AddonWindowLocation.BottomRight
					size.Height = _OriginalRect.Height + _childWnd.Height
					size.Width = _OriginalRect.Width + _childWnd.Width
			End Select
			NativeMethods.SetWindowPos(New HandleRef(Me, _hwndFileDialog), New HandleRef(Me, CType(NativeMethods.ZOrderPos.HWND_BOTTOM, IntPtr)), 0, 0, CInt(Fix(size.Width)), CInt(Fix(size.Height)), NativeMethods.SetWindowPosFlags.SWP_NOZORDER)
		End Sub
		
		Private Function AnonymousMethod1(ByVal sender As Object, ByVal e As EventArgs) As Object
		  Try
			  _source = TryCast(System.Windows.PresentationSource.FromVisual(TryCast(_childWnd, Window)), HwndSource)
			  _source.AddHook(AddressOf EmbededWndProc)
			  _childWnd.Source = _source
		  Catch
		  End Try
			Return Nothing
		End Function


		Private Sub CustomPostCreation()
			_hListViewPtr = NativeMethods.GetDlgItem(Me._hwndFileDialog, CInt(Fix(NativeMethods.ControlsId.DefaultView)))
			UpdateListView(_hListViewPtr)
			If _bFixedSize Then
				SetFixedSize(_hwndFileDialog)
			End If
			Dim dialogWndRect As New RECT()
			NativeMethods.GetWindowRect(New HandleRef(Me, Me._hwndFileDialog), dialogWndRect)
			Dim dialogClientRect As New RECT()
			NativeMethods.GetClientRect(New HandleRef(Me, Me._hwndFileDialog), dialogClientRect)
			Dim dx As UInteger = dialogWndRect.Width - dialogClientRect.Width
			Dim dy As UInteger = dialogWndRect.Height - dialogClientRect.Height
			If TypeOf _childWnd Is Window Then
				Dim wnd As Window = TryCast(_childWnd, Window)
				'restore the original size
				Select Case FileDlgStartLocation
					Case AddonWindowLocation.Bottom
						Dim left As Integer = If((Environment.Version.Major >= 4), -CInt(Fix(dx)) / 2, dialogWndRect.left)
						If wnd.Width >= _OriginalRect.Width - dx Then
							NativeMethods.MoveWindow(New HandleRef(Me, Me._hwndFileDialog), left, dialogWndRect.top, CInt(Fix(wnd.ActualWidth + dx \ 2)), CInt(Fix(_OriginalRect.Height + wnd.ActualHeight)), True)
						Else
							NativeMethods.MoveWindow(New HandleRef(Me, Me._hwndFileDialog), left, dialogWndRect.top, CInt(Fix(_OriginalRect.Width)), CInt(Fix(_OriginalRect.Height + wnd.ActualHeight)), True)
							wnd.Width = _OriginalRect.Width - dx \ 2
						End If
						wnd.Left = 0
						wnd.Top = _OriginalRect.Height - dy + dx \ 2
					Case AddonWindowLocation.Right
						Dim top As Integer = If((Environment.Version.Major >= 4), CInt(Fix(dx \ 2 - dy)), dialogWndRect.top)
						If wnd.Height >= _OriginalRect.Height - dy Then
							NativeMethods.MoveWindow(New HandleRef(Me, _hwndFileDialog), CInt(Fix(dialogWndRect.left)), top, CInt(Fix(_OriginalRect.Width + wnd.ActualWidth)), CInt(Fix(wnd.ActualHeight + dy - dx \ 2)), True)
						Else
							NativeMethods.MoveWindow(New HandleRef(Me, _hwndFileDialog), CInt(Fix(dialogWndRect.left)), top, CInt(Fix(_OriginalRect.Width + wnd.ActualWidth)), CInt(Fix(_OriginalRect.Height - dx \ 2)), True)
							wnd.Height = _OriginalRect.Height - dy
						End If
							wnd.Top = 0
							wnd.Left = _OriginalRect.Width - dx \ 2
					Case AddonWindowLocation.BottomRight
						NativeMethods.MoveWindow(New HandleRef(Me, _hwndFileDialog), dialogWndRect.left, dialogWndRect.top, CInt(Fix(_OriginalRect.Width + wnd.Width)), CInt(Fix(CInt(Fix(_OriginalRect.Height + wnd.Height)))), True)
						wnd.Top = _OriginalRect.Height - dy + dx \ 2
						wnd.Left = _OriginalRect.Width - dx \ 2
				End Select

			Else
				Dim ctrl As ContentControl = TryCast(_childWnd, ContentControl)
				'restore the original size
				Const flags As NativeMethods.SetWindowPosFlags = NativeMethods.SetWindowPosFlags.SWP_NOZORDER Or NativeMethods.SetWindowPosFlags.SWP_NOMOVE '| SetWindowPosFlags.SWP_NOREPOSITION | SetWindowPosFlags.SWP_ASYNCWINDOWPOS | SetWindowPosFlags.SWP_SHOWWINDOW | SetWindowPosFlags.SWP_DRAWFRAME;
				Select Case FileDlgStartLocation
					Case AddonWindowLocation.Bottom
						NativeMethods.SetWindowPos(New HandleRef(Me, Me._hwndFileDialog), New HandleRef(Me,CType(ZOrderPos.HWND_BOTTOM, IntPtr)), dialogWndRect.left, dialogWndRect.top, CInt(Fix(ctrl.ActualWidth + dx \ 2)), CInt(Fix(_OriginalRect.Height + ctrl.ActualHeight)), flags)
						'NativeMethods.MoveWindow(new HandleRef(this, this._hwndFileDialog), dialogWndRect.left, dialogWndRect.top, (int)(ctrl.ActualWidth + dx / 2), (int)(_OriginalRect.Height + ctrl.ActualHeight), true);
						NativeMethods.SetWindowPos(New HandleRef(ctrl, _source.Handle), New HandleRef(_source, CType(ZOrderPos.HWND_BOTTOM, IntPtr)), 0, CInt(Fix(_OriginalRect.Height - dy + dx \ 2)), CInt(Fix(ctrl.Width)), CInt(Fix(ctrl.Height)), flags)
						'NativeMethods.MoveWindow(new HandleRef(ctrl, _source.Handle), 0, (int)(_OriginalRect.Height - dy + dx / 2), (int)(ctrl.Width), (int)(ctrl.Height), true);
					Case AddonWindowLocation.Right
						NativeMethods.SetWindowPos(New HandleRef(Me, Me._hwndFileDialog), New HandleRef(Me, CType(ZOrderPos.HWND_BOTTOM, IntPtr)), CInt(Fix(dialogWndRect.left)), dialogWndRect.top, CInt(Fix(_OriginalRect.Width + ctrl.ActualWidth - dx \ 2)), CInt(Fix(ctrl.ActualHeight + dy - dx \ 2)), flags)
						'NativeMethods.MoveWindow(new HandleRef(this, _hwndFileDialog), (int)(dialogWndRect.left), dialogWndRect.top, (int)(_OriginalRect.Width + ctrl.ActualWidth - dx / 2), (int)(ctrl.ActualHeight + dy - dx / 2), true);
						NativeMethods.SetWindowPos(New HandleRef(ctrl, _source.Handle), New HandleRef(_source, CType(ZOrderPos.HWND_BOTTOM, IntPtr)), CInt(Fix(_OriginalRect.Width - dx)), CInt(Fix(0)), CInt(Fix(ctrl.Width)), CInt(Fix(ctrl.Height)), flags)
						'NativeMethods.MoveWindow(new HandleRef(ctrl, _source.Handle), (int)(_OriginalRect.Width - dx), (int)(0), (int)(ctrl.Width), (int)(ctrl.Height), true);
					Case AddonWindowLocation.BottomRight
						'NativeMethods.MoveWindow(new HandleRef(this, _hwndFileDialog), dialogWndRect.left, dialogWndRect.top, (int)(_OriginalRect.Width + ctrl.Width), (int)(_OriginalRect.Height + ctrl.Height), true);
						NativeMethods.SetWindowPos(New HandleRef(Me, Me._hwndFileDialog), New HandleRef(Me, CType(ZOrderPos.HWND_BOTTOM, IntPtr)), dialogWndRect.left, dialogWndRect.top, CInt(Fix(_OriginalRect.Width + ctrl.Width)), CInt(Fix(_OriginalRect.Height + ctrl.Height)), flags)
				End Select

			End If
			CenterDialogToScreen()
			NativeMethods.InvalidateRect(New HandleRef(Me, _source.Handle), IntPtr.Zero, True)

		End Sub
	End Class
End Namespace
