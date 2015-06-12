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
Imports MS.Win32
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Permissions
Imports System.Text
Imports System.Windows
Imports System.Reflection

Namespace WpfCustomFileDialog


	#Region "POINT"

	<StructLayout(LayoutKind.Sequential)> _
	Public Structure POINT
		Public x As Integer
		Public y As Integer

		#Region "Constructors"
		Public Sub New(ByVal x As Integer, ByVal y As Integer)
			Me.x = x
			Me.y = y
		End Sub

		Public Sub New(ByVal point As Point)
			x = CInt(Fix(point.X))
			y = CInt(Fix(point.Y))
		End Sub
		#End Region
	End Structure
	#End Region

	#Region "MinMax"
	<StructLayout(LayoutKind.Sequential)> _
	Public Structure MINMAXINFO
		Public ptReserved As POINT
		Public ptMaxSize As POINT
		Public ptMaxPosition As POINT
		Public ptMinTrackSize As POINT
		Public ptMaxTrackSize As POINT
	End Structure
	#End Region

	<StructLayout(LayoutKind.Sequential)> _
	Friend Class OFNOTIFY
		Friend hdr_hwndFrom As IntPtr
		Friend hdr_idFrom As IntPtr
		Friend hdr_code As Integer
		Friend lpOFN As IntPtr
		Friend pszFile As IntPtr
	End Class
	#Region "DialogChangeProperties"
	Friend Enum DialogChangeProperties
		CDM_FIRST = (NativeMethods.Msg.WM_USER + 100)
		CDM_GETSPEC = (CDM_FIRST + &H0000)
		CDM_GETFILEPATH = (CDM_FIRST + &H0001)
		CDM_GETFOLDERPATH = (CDM_FIRST + &H0002)
		CDM_GETFOLDERIDLIST = (CDM_FIRST + &H0003)
		CDM_SETCONTROLTEXT = (CDM_FIRST + &H0004)
		CDM_HIDECONTROL = (CDM_FIRST + &H0005)
		CDM_SETDEFEXT = (CDM_FIRST + &H0006)
	End Enum
	#End Region
	#Region "ImeNotify"

	Friend Enum ImeNotify
		IMN_CLOSESTATUSWINDOW = &H0001
		IMN_OPENSTATUSWINDOW = &H0002
		IMN_CHANGECANDIDATE = &H0003
		IMN_CLOSECANDIDATE = &H0004
		IMN_OPENCANDIDATE = &H0005
		IMN_SETCONVERSIONMODE = &H0006
		IMN_SETSENTENCEMODE = &H0007
		IMN_SETOPENSTATUS = &H0008
		IMN_SETCANDIDATEPOS = &H0009
		IMN_SETCOMPOSITIONFONT = &H000A
		IMN_SETCOMPOSITIONWINDOW = &H000B
		IMN_SETSTATUSWINDOWPOS = &H000C
		IMN_GUIDELINE = &H000D
		IMN_PRIVATE = &H000E
	End Enum
	#End Region
#Region ""
	Friend Enum GWL
		GWL_WNDPROC = (-4)
		GWL_HINSTANCE = (-6)
		GWL_HWNDPARENT = (-8)
		GWL_STYLE = (-16)
		GWL_EXSTYLE = (-20)
		GWL_USERDATA = (-21)
		GWL_ID = (-12)
	End Enum
	#End Region
	#Region "FolderViewMode"

    Public Enum FolderViewMode
        [Default] = &H7028
        Icon = [Default] + 1
        SmallIcon = [Default] + 2
        List = [Default] + 3
        Details = [Default] + 4
        Thumbnails = [Default] + 5
        Title = [Default] + 6
        Thumbstrip = [Default] + 7
    End Enum
	#End Region
	#Region "RECT"

	<StructLayout(LayoutKind.Sequential)> _
	Public Structure RECT
		Public left As Integer
		Public top As Integer
		Public right As Integer
		Public bottom As Integer

		#Region "Properties"

		Public Property Location() As POINT
			Get
				Return New POINT(CInt(Fix(left)), CInt(Fix(top)))
			End Get
			Set(ByVal value As POINT)
				right -= (left - value.x)
				bottom -= (bottom - value.y)
				left = value.x
				top = value.y
			End Set
		End Property

		Friend Property Width() As UInteger
			Get
				Return CUInt(Math.Abs(right - left))
			End Get
			Set(ByVal value As UInteger)
				right = left + CInt(Fix(value))
			End Set
		End Property

		Friend Property Height() As UInteger
			Get
				Return CUInt(Math.Abs(bottom - top))
			End Get
			Set(ByVal value As UInteger)
				bottom = top + CInt(Fix(value))
			End Set
		End Property
		#End Region

		#Region "Overrides"
		Public Overrides Function ToString() As String
			Return left & ":" & top & ":" & right & ":" & bottom
		End Function
		#End Region
		Public ReadOnly Property IsEmpty() As Boolean
			Get
				If Me.left < Me.right Then
					Return (Me.top >= Me.bottom)
				End If
				Return True
			End Get
		End Property
	End Structure
	#End Region
	#Region "SWP_Flags"
	<Flags> _
	Public Enum SWP_Flags
		SWP_NOSIZE = &H0001
		SWP_NOMOVE = &H0002
		SWP_NOZORDER = &H0004
		SWP_NOACTIVATE = &H0010
		SWP_FRAMECHANGED = &H0020 ' The frame changed: send WM_NCCALCSIZE 
		SWP_SHOWWINDOW = &H0040
		SWP_HIDEWINDOW = &H0080
		SWP_NOOWNERZORDER = &H0200 ' Don't do owner Z ordering 

		SWP_DRAWFRAME = SWP_FRAMECHANGED
		SWP_NOREPOSITION = SWP_NOOWNERZORDER
	End Enum
	#End Region

	<StructLayout(LayoutKind.Sequential, CharSet := CharSet.Unicode)> _
	Public Class OPENFILENAME_I
		Public Delegate Function WndProc(ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
		Public lStructSize As Integer = Marshal.SizeOf(GetType(OPENFILENAME_I))
		Public hwndOwner As IntPtr
		Public hInstance As IntPtr
		Public lpstrFilter As String
		Public lpstrCustomFilter As IntPtr
		Public nMaxCustFilter As Integer
		Public nFilterIndex As Integer
		Public lpstrFile As IntPtr
		Public nMaxFile As Integer = 260
		Public lpstrFileTitle As IntPtr
		Public nMaxFileTitle As Integer = 260
		Public lpstrInitialDir As String
		Public lpstrTitle As String
		Public Flags As Integer
		Public nFileOffset As Short
		Public nFileExtension As Short
		Public lpstrDefExt As String
		Public lCustData As IntPtr
		Public lpfnHook As WndProc
		Public lpTemplateName As String
		Public pvReserved As IntPtr
		Public dwReserved As Integer
		Public FlagsEx As Integer
	End Class
	#Region "WINDOWPOS"

	<StructLayout(LayoutKind.Sequential)> _
	Public Structure WINDOWPOS
		Public hwnd As IntPtr
		Public hwndAfter As IntPtr
		Public x As Integer
		Public y As Integer
		Public cx As Integer
		Public cy As Integer
		Public flags As UInteger

		#Region "Overrides"
		Public Overrides Function ToString() As String
			Return x & ":" & y & ":" & cx & ":" & cy & ":" & (CType(flags, SWP_Flags)).ToString()
		End Function
		#End Region
	End Structure
	#End Region

	#Region "WINDOWPLACEMENT"
	<StructLayout(LayoutKind.Sequential)> _
	Public Structure WINDOWPLACEMENT
		Public length As Integer
		Public flags As Integer
		Public showCmd As Integer
		Public ptMinPosition_x As Integer
		Public ptMinPosition_y As Integer
		Public ptMaxPosition_x As Integer
		Public ptMaxPosition_y As Integer
		Public rcNormalPosition_left As Integer
		Public rcNormalPosition_top As Integer
		Public rcNormalPosition_right As Integer
		Public rcNormalPosition_bottom As Integer
	End Structure




	#End Region
	#Region "ZOrderPos"
	Friend Enum ZOrderPos
		HWND_TOP = 0
		HWND_BOTTOM = 1
		HWND_TOPMOST = -1
		HWND_NOTOPMOST = -2
	End Enum
	#End Region
	#Region "SetWindowPosFlags"

	<Flags> _
	Friend Enum SetWindowPosFlags
		SWP_NOSIZE = &H0001
		SWP_NOMOVE = &H0002
		SWP_NOZORDER = &H0004
		SWP_NOREDRAW = &H0008
		SWP_NOACTIVATE = &H0010
		SWP_FRAMECHANGED = &H0020
		SWP_SHOWWINDOW = &H0040
		SWP_HIDEWINDOW = &H0080
		SWP_NOCOPYBITS = &H0100
		SWP_NOOWNERZORDER = &H0200
		SWP_NOSENDCHANGING = &H0400
		SWP_DRAWFRAME = &H0020
		SWP_NOREPOSITION = &H0200
		SWP_DEFERERASE = &H2000
		SWP_ASYNCWINDOWPOS = &H4000
	End Enum
	#End Region

	#Region "WINDOWINFO"

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure WINDOWINFO
		Public cbSize As UInt32
		Public rcWindow As RECT
		Public rcClient As RECT
		Public dwStyle As UInt32
		Public dwExStyle As UInt32
		Public dwWindowStatus As UInt32
		Public cxWindowBorders As UInt32
		Public cyWindowBorders As UInt32
		Public atomWindowType As UInt16
		Public wCreatorVersion As UInt16
	End Structure
	#End Region
	Public NotInheritable Class NativeMethods
		#Region "Delegates"
		Public Delegate Function EnumWindowsCallBack(ByVal hWnd As IntPtr, ByVal lParam As Integer) As Boolean
		#End Region
		<DllImport("user32.dll")> _
		Public Shared Function AdjustWindowRect(ByRef lpRect As RECT, ByVal dwStyle As Int32, ByVal bMenu As Boolean) As Boolean
		End Function
		<SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("comdlg32.dll", CharSet := CharSet.Unicode, SetLastError := True)> _
		Friend Shared Function GetOpenFileName(<[In](), Out()> ByVal ofn As OPENFILENAME_I) As Boolean
		End Function
		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("comdlg32.dll", CharSet := CharSet.Unicode, SetLastError := True)> _
		Friend Shared Function GetSaveFileName(<[In](), Out()> ByVal ofn As OPENFILENAME_I) As Boolean
		End Function
		<DllImport("user32.dll", CharSet := CharSet.Auto, SetLastError := True)> _
		Friend Shared Function MoveWindow(ByVal hWnd As HandleRef, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bRepaint As Boolean) As Boolean
		End Function
		<DllImport("user32.dll", CharSet := CharSet.Auto, SetLastError := False)> _
		Friend Shared Function SendMessage(ByVal hWnd As HandleRef, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
		End Function
		<DllImport("user32.dll", CharSet := CharSet.Auto, SetLastError := True)> _
		Friend Shared Function PostMessage(ByVal hWnd As HandleRef, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Boolean
		End Function
		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet := CharSet.Auto, SetLastError := True)> _
		Public Shared Function GetClassName(ByVal hwnd As HandleRef, ByVal lpClassName As StringBuilder, ByVal nMaxCount As Integer) As Integer
		End Function
		<DllImport("user32.dll", SetLastError := True)> _
		Friend Shared Function GetWindowInfo(ByVal hwnd As IntPtr, <System.Runtime.InteropServices.Out()> ByRef pwi As WINDOWINFO) As <MarshalAs(UnmanagedType.Bool)> Boolean
		End Function
		<DllImport("user32.Dll")> _
		Public Shared Function EnumChildWindows(ByVal hWndParent As IntPtr, ByVal lpEnumFunc As NativeMethods.EnumWindowsCallBack, ByVal lParam As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
		End Function
		<DllImport("User32.Dll")> _
		Public Shared Function GetDlgCtrlID(ByVal hWndCtl As IntPtr) As Integer
		End Function
		<DllImport("user32.dll")> _
		Friend Shared Function GetDlgItem(ByVal hDlg As IntPtr, ByVal nIDDlgItem As Integer) As IntPtr
		End Function
		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet := CharSet.Auto, ExactSpelling := True)> _
		Public Shared Function SetParent(ByVal hWnd As HandleRef, ByVal hWndParent As HandleRef) As IntPtr
		End Function
		<DllImport("user32.dll", EntryPoint := "SetWindowText", CharSet := CharSet.Auto, SetLastError := True)> _
		Private Shared Function IntSetWindowText(ByVal hWnd As HandleRef, ByVal text As String) As Boolean
		End Function
		Private Sub New()
		End Sub
		<SecurityCritical, SecuritySafeCritical> _
		Friend Shared Sub SetWindowText(ByVal hWnd As HandleRef, ByVal text As String)
			If Not IntSetWindowText(hWnd, text) Then
				Throw New Win32Exception()
			End If
		End Sub
		<DllImport("user32.dll", CharSet := CharSet.Auto, SetLastError := False)> _
		Friend Shared Function SendMessage(ByVal hWnd As HandleRef, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As StringBuilder) As IntPtr
		End Function
		<DllImport("user32.dll", SetLastError := True, CharSet := CharSet.Auto)> _
		Friend Shared Function RegisterWindowMessage(ByVal lpString As String) As UInteger
		End Function
		<SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet := CharSet.Auto, ExactSpelling := True)> _
		Public Shared Function GetFocus() As IntPtr
		End Function
		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint := "GetParent", CharSet := CharSet.Auto, SetLastError := True, ExactSpelling := True)> _
		Private Shared Function IntGetParent(ByVal hWnd As HandleRef) As IntPtr
		End Function
		<SecurityCritical> _
		Friend Shared Function GetParent(ByVal hWnd As HandleRef) As IntPtr
			SetLastError(0)
			Dim ptr As IntPtr = IntGetParent(hWnd)
			Dim [error] As Integer = Marshal.GetLastWin32Error()
			If (ptr = IntPtr.Zero) AndAlso ([error] <> 0) Then
				Throw New Win32Exception([error])
			End If
			Return ptr
		End Function
		<SecurityCritical> _
		Friend Shared Function GetWindowText(ByVal hWnd As HandleRef, <Out()> ByVal lpString As StringBuilder, ByVal nMaxCount As Integer) As Integer
			SetLastError(0)
			Dim num As Integer = IntGetWindowText(hWnd, lpString, nMaxCount)
			If num = 0 Then
				Dim [error] As Integer = Marshal.GetLastWin32Error()
				If [error] <> 0 Then
					Throw New Win32Exception([error])
				End If
			End If
			Return num
		End Function

		<SecurityCritical> _
		Friend Shared Function CriticalSetWindowLong(ByVal hWnd As HandleRef, ByVal nIndex As Integer, ByVal dwNewLong As IntPtr) As IntPtr
			If IntPtr.Size = 4 Then
				Return New IntPtr(IntCriticalSetWindowLong(hWnd, nIndex, CInt(Fix(dwNewLong.ToInt64()))))
			End If
			Return IntCriticalSetWindowLongPtr(hWnd, nIndex, dwNewLong)
		End Function

		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint := "SetWindowLong", CharSet := CharSet.Auto)> _
		Private Shared Function IntCriticalSetWindowLong(ByVal hWnd As HandleRef, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Integer
		End Function


		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint := "SetWindowLongPtr", CharSet := CharSet.Auto, SetLastError := True)> _
		Private Shared Function IntCriticalSetWindowLongPtr(ByVal hWnd As HandleRef, ByVal nIndex As Integer, ByVal dwNewLong As OPENFILENAME_I.WndProc) As IntPtr
		End Function
		<SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint := "SetWindowLongPtr", CharSet := CharSet.Auto)> _
		Private Shared Function IntCriticalSetWindowLongPtr(ByVal hWnd As HandleRef, ByVal nIndex As Integer, ByVal dwNewLong As IntPtr) As IntPtr
		End Function
		<SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint := "SendMessage", CharSet := CharSet.Auto)> _
		Friend Shared Function UnsafeSendMessage(ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
		End Function
		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet := CharSet.Auto, ExactSpelling := True)> _
		Public Shared Function IsWindow(ByVal hWnd As HandleRef) As Boolean
		End Function


		<DllImport("user32.dll", EntryPoint := "GetWindowPlacement", CharSet := CharSet.Auto, SetLastError := True, ExactSpelling := True)> _
		Private Shared Function IntGetWindowPlacement(ByVal hWnd As HandleRef, ByRef placement As WINDOWPLACEMENT) As Boolean
		End Function

		<SecurityCritical, SecuritySafeCritical> _
		Friend Shared Sub GetWindowPlacement(ByVal hWnd As HandleRef, ByRef placement As WINDOWPLACEMENT)
			If Not IntGetWindowPlacement(hWnd, placement) Then
				Throw New Win32Exception()
			End If
		End Sub



		<SecuritySafeCritical> _
		Friend Shared Sub SetWindowPlacement(ByVal hWnd As HandleRef, <[In]()> ByRef placement As WINDOWPLACEMENT)
			If Not IntSetWindowPlacement(hWnd, placement) Then
				Throw New Win32Exception()
			End If
		End Sub


		<DllImport("user32.dll", EntryPoint := "SetWindowPlacement", CharSet := CharSet.Auto, SetLastError := True, ExactSpelling := True)> _
		Private Shared Function IntSetWindowPlacement(ByVal hWnd As HandleRef, <[In]()> ByRef placement As WINDOWPLACEMENT) As Boolean
		End Function


		<SecurityCritical> _
		Public Shared Function EnableWindow(ByVal hWnd As HandleRef, ByVal enable As Boolean) As Boolean
			SetLastError(0)
			Dim flag As Boolean = IntEnableWindow(hWnd, enable)
			If Not flag Then
				Dim [error] As Integer = Marshal.GetLastWin32Error()
				If [error] <> 0 Then
					Throw New Win32Exception([error])
				End If
			End If
			Return flag
		End Function


		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint := "EnableWindow", CharSet := CharSet.Auto, SetLastError := True, ExactSpelling := True)> _
		Public Shared Function IntEnableWindow(ByVal hWnd As HandleRef, ByVal enable As Boolean) As Boolean
		End Function

		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint := "GetWindowText", CharSet := CharSet.Auto, SetLastError := True)> _
		Private Shared Function IntGetWindowText(ByVal hWnd As HandleRef, <Out()> ByVal lpString As StringBuilder, ByVal nMaxCount As Integer) As Integer
		End Function
		<SecurityCritical> _
		Friend Shared Function GetWindowTextLength(ByVal hWnd As HandleRef) As Integer
			SetLastError(0)
			Dim num As Integer = IntGetWindowTextLength(hWnd)
			If num = 0 Then
				Dim [error] As Integer = Marshal.GetLastWin32Error()
				If [error] <> 0 Then
					Throw New Win32Exception([error])
				End If
			End If
			Return num
		End Function

		<SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint := "GetWindowTextLength", CharSet := CharSet.Auto, SetLastError := True)> _
		Private Shared Function IntGetWindowTextLength(ByVal hWnd As HandleRef) As Integer
		End Function

		<DllImport("user32.dll")> _
		Friend Shared Function InvalidateRect(ByVal hWnd As HandleRef, ByVal lpRect As IntPtr, ByVal bErase As Boolean) As Boolean
		End Function
		<SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint := "SetFocus", CharSet := CharSet.Auto, SetLastError := True, ExactSpelling := True)> _
		Private Shared Function IntSetFocus(ByVal hWnd As HandleRef) As IntPtr
		End Function
		<SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("comdlg32.dll", CharSet := CharSet.Auto, SetLastError := True, ExactSpelling := True)> _
		Friend Shared Function CommDlgExtendedError() As Integer
		End Function
		Friend Shared Function SetFocus(ByVal hWnd As HandleRef) As IntPtr
			SetLastError(0)
			Dim ptr As IntPtr = IntSetFocus(hWnd)
			Dim [error] As Integer = Marshal.GetLastWin32Error()
			If (ptr = IntPtr.Zero) AndAlso ([error] <> 0) Then
				Throw New Win32Exception([error])
			End If
			Return ptr
		End Function
		<SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet := CharSet.Auto, ExactSpelling := True)> _
		Friend Shared Sub SetLastError(ByVal dwErrorCode As Integer)
		End Sub

        <SuppressUnmanagedCodeSecurity(), SecurityCritical(), DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
  Friend Shared Function SetWindowPos(ByVal hWnd As HandleRef, ByVal hWndInsertAfter As HandleRef, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal flags As SetWindowPosFlags) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function


		<DllImport("user32.dll", SetLastError := True)> _
		Public Shared Function FindWindowEx(ByVal parentHandle As HandleRef, ByVal childAfter As HandleRef, ByVal className As String, ByVal windowTitle As String) As IntPtr
		End Function

		<SecurityCritical> _
		Public Shared Sub GetWindowRect(ByVal hWnd As HandleRef, <[In](), Out()> ByRef rect As RECT)
			If Not IntGetWindowRect(hWnd, rect) Then
				Throw New Win32Exception()
			End If
		End Sub
        <DllImport("user32.dll", EntryPoint:="GetWindowRect", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
  Public Shared Function IntGetWindowRect(ByVal hWnd As HandleRef, <[In](), Out()> ByRef rect As RECT) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

		<SecurityCritical> _
		Public Shared Sub GetClientRect(ByVal hWnd As HandleRef, <[In](), Out()> ByRef rect As RECT)
			If Not IntGetClientRect(hWnd, rect) Then
				Throw New Win32Exception()
			End If
		End Sub
        <DllImport("user32.dll", EntryPoint:="GetClientRect", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
  Public Shared Function IntGetClientRect(ByVal hWnd As HandleRef, <[In](), Out()> ByRef rect As RECT) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function
		<SecurityCritical> _
		Friend Shared Function GetWindowLong(ByVal hWnd As HandleRef, ByVal nIndex As Integer) As Integer
			Dim num As Integer = 0
			Dim zero As IntPtr = IntPtr.Zero
			Dim num2 As Integer = 0
			SetLastError(0)
			If IntPtr.Size = 4 Then
				num = IntGetWindowLong(hWnd, nIndex)
				num2 = Marshal.GetLastWin32Error()
				zero = New IntPtr(num)
			Else
				zero = IntGetWindowLongPtr(hWnd, nIndex)
				num2 = Marshal.GetLastWin32Error()
				num = CInt(Fix(zero.ToInt64()))
			End If
			If zero = IntPtr.Zero Then
			End If
			Return num
		End Function

		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint := "GetWindowLong", CharSet := CharSet.Auto, SetLastError := True)> _
		Private Shared Function IntGetWindowLong(ByVal hWnd As HandleRef, ByVal nIndex As Integer) As Integer
		End Function
		<SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint := "GetWindowLongPtr", CharSet := CharSet.Auto, SetLastError := True)> _
		Private Shared Function IntGetWindowLongPtr(ByVal hWnd As HandleRef, ByVal nIndex As Integer) As IntPtr
		End Function
		<SecurityCritical> _
		Friend Shared Function GetWindowLongPtr(ByVal hWnd As HandleRef, ByVal nIndex As GWL) As IntPtr
			Dim zero As IntPtr = IntPtr.Zero
			Dim num As Integer = 0
			SetLastError(0)
			If IntPtr.Size = 4 Then
				Dim num2 As Integer = IntGetWindowLong(hWnd, CInt(Fix(nIndex)))
				num = Marshal.GetLastWin32Error()
				zero = New IntPtr(num2)
			Else
				zero = IntGetWindowLongPtr(hWnd, CInt(Fix(nIndex)))
				num = Marshal.GetLastWin32Error()
			End If
			If zero = IntPtr.Zero Then
			End If
			Return zero
		End Function
		<DllImport("advapi32.dll", EntryPoint := "RegCreateKeyW")> _
		Public Shared Function RegCreateKeyW(<[In]()> ByVal hKey As UIntPtr, <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpSubKey As String, <System.Runtime.InteropServices.Out()> ByRef phkResult As IntPtr) As Integer
		End Function

		<DllImport("advapi32.dll", EntryPoint := "RegOverridePredefKey")> _
		Public Shared Function RegOverridePredefKey(<[In]()> ByVal hKey As UIntPtr, <[In]()> ByVal hNewHKey As IntPtr) As Integer
		End Function

		<DllImport("advapi32.dll", EntryPoint := "RegCloseKey")> _
		Public Shared Function RegCloseKey(<[In]()> ByVal hKey As IntPtr) As Integer
		End Function


		#Region "Enums"

		#Region "ZOrderPos"

		Friend Enum ZOrderPos
			HWND_TOP = 0
			HWND_BOTTOM = 1
			HWND_TOPMOST = -1
			HWND_NOTOPMOST = -2
		End Enum
		#End Region
		<Flags> _
		Public Enum SetWindowPosFlags
			SWP_NOSIZE = &H0001
			SWP_NOMOVE = &H0002
			SWP_NOZORDER = &H0004
			SWP_NOREDRAW = &H0008
			SWP_NOACTIVATE = &H0010
			SWP_FRAMECHANGED = &H0020
			SWP_SHOWWINDOW = &H0040
			SWP_HIDEWINDOW = &H0080
			SWP_NOCOPYBITS = &H0100
			SWP_NOOWNERZORDER = &H0200
			SWP_NOSENDCHANGING = &H0400
			SWP_DRAWFRAME = &H0020
			SWP_NOREPOSITION = &H0200
			SWP_DEFERERASE = &H2000
			SWP_ASYNCWINDOWPOS = &H4000
		End Enum
		#End Region
		#Region "DialogChangeStatus"
		Friend Enum DialogChangeStatus As Integer
			CDN_FIRST = -601
			CDN_INITDONE = (CDN_FIRST - &H0000)
			CDN_SELCHANGE = (CDN_FIRST - &H0001)
			CDN_FOLDERCHANGE = (CDN_FIRST - &H0002)
			CDN_SHAREVIOLATION = (CDN_FIRST - &H0003)
			CDN_HELP = (CDN_FIRST - &H0004)
			CDN_FILEOK = (CDN_FIRST - &H0005)
			CDN_TYPECHANGE = (CDN_FIRST - &H0006)
			CDN_INCLUDEITEM = (CDN_FIRST - &H0007)
		End Enum
		#End Region


		#Region "NCCALCSIZE_PARAMS"

		<StructLayout(LayoutKind.Sequential)> _
		Friend Structure NCCALCSIZE_PARAMS
			Public rgrc0, rgrc1, rgrc2 As RECT
			Public lppos As IntPtr
		End Structure
		#End Region

		#Region "FolderViewMode"


        Public Enum FolderViewMode
            [Default] = &H7028
            Icon = [Default] + 1
            SmallIcon = [Default] + 2
            List = [Default] + 3
            Details = [Default] + 4
            Thumbnails = [Default] + 5
            Tiles = [Default] + 6
            Thumbstrip = [Default] + 7
        End Enum
		#End Region
		#Region "Window Styles"

		<Flags> _
		Friend Enum WindowStyles As UInteger
			WS_OVERLAPPED = &H00000000
			WS_POPUP = &H80000000L
			WS_CHILD = &H40000000
			WS_MINIMIZE = &H20000000
			WS_VISIBLE = &H10000000
			WS_DISABLED = &H08000000
			WS_CLIPSIBLINGS = &H04000000
			WS_CLIPCHILDREN = &H02000000
			WS_MAXIMIZE = &H01000000
			WS_CAPTION = &H00C00000
			WS_BORDER = &H00800000
			WS_DLGFRAME = &H00400000
			WS_VSCROLL = &H00200000
			WS_HSCROLL = &H00100000
			WS_SYSMENU = &H00080000
			WS_THICKFRAME = &H00040000
			WS_GROUP = &H00020000
			WS_TABSTOP = &H00010000
			WS_MINIMIZEBOX = &H00020000
			WS_MAXIMIZEBOX = &H00010000
			WS_TILED = &H00000000
			WS_ICONIC = &H20000000
			WS_SIZEBOX = &H00040000
			WS_POPUPWINDOW = &H80880000L
			WS_OVERLAPPEDWINDOW = &H00CF0000
			WS_TILEDWINDOW = &H00CF0000
			WS_CHILDWINDOW = &H40000000
		End Enum
		#End Region

		#Region "Window Extended Styles"

		<Flags> _
		Friend Enum WindowExtendedStyles
			WS_EX_DLGMODALFRAME = &H00000001
			WS_EX_NOPARENTNOTIFY = &H00000004
			WS_EX_TOPMOST = &H00000008
			WS_EX_ACCEPTFILES = &H00000010
			WS_EX_TRANSPARENT = &H00000020
			WS_EX_MDICHILD = &H00000040
			WS_EX_TOOLWINDOW = &H00000080
			WS_EX_WINDOWEDGE = &H00000100
			WS_EX_CLIENTEDGE = &H00000200
			WS_EX_CONTEXTHELP = &H00000400
			WS_EX_RIGHT = &H00001000
			WS_EX_LEFT = &H00000000
			WS_EX_RTLREADING = &H00002000
			WS_EX_LTRREADING = &H00000000
			WS_EX_LEFTSCROLLBAR = &H00004000
			WS_EX_RIGHTSCROLLBAR = &H00000000
			WS_EX_CONTROLPARENT = &H00010000
			WS_EX_STATICEDGE = &H00020000
			WS_EX_APPWINDOW = &H00040000
			WS_EX_OVERLAPPEDWINDOW = &H00000300
			WS_EX_PALETTEWINDOW = &H00000188
			WS_EX_LAYERED = &H00080000
		End Enum
		#End Region
		Public Enum AddonWindowLocation
			BottomRight = 0
			Right = 1
			Bottom = 2
		End Enum
		#Region "ControlIds"
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
		Public Enum Msg
			WM_NULL = &H0000
			WM_CREATE = &H0001
			WM_DESTROY = &H0002
			WM_MOVE = &H0003
			WM_SIZE = &H0005
			WM_ACTIVATE = &H0006
			WM_SETFOCUS = &H0007
			WM_KILLFOCUS = &H0008
			WM_ENABLE = &H000A
			WM_SETREDRAW = &H000B
			WM_SETTEXT = &H000C
			WM_GETTEXT = &H000D
			WM_GETTEXTLENGTH = &H000E
			WM_PAINT = &H000F
			WM_CLOSE = &H0010
			WM_QUERYENDSESSION = &H0011
			WM_QUIT = &H0012
			WM_QUERYOPEN = &H0013
			WM_ERASEBKGND = &H0014
			WM_SYSCOLORCHANGE = &H0015
			WM_ENDSESSION = &H0016
			WM_SHOWWINDOW = &H0018
			WM_CTLCOLOR = &H0019
			WM_WININICHANGE = &H001A
			WM_SETTINGCHANGE = &H001A
			WM_DEVMODECHANGE = &H001B
			WM_ACTIVATEAPP = &H001C
			WM_FONTCHANGE = &H001D
			WM_TIMECHANGE = &H001E
			WM_CANCELMODE = &H001F
			WM_SETCURSOR = &H0020
			WM_MOUSEACTIVATE = &H0021
			WM_CHILDACTIVATE = &H0022
			WM_QUEUESYNC = &H0023
			WM_GETMINMAXINFO = &H0024
			WM_PAINTICON = &H0026
			WM_ICONERASEBKGND = &H0027
			WM_NEXTDLGCTL = &H0028
			WM_SPOOLERSTATUS = &H002A
			WM_DRAWITEM = &H002B
			WM_MEASUREITEM = &H002C
			WM_DELETEITEM = &H002D
			WM_VKEYTOITEM = &H002E
			WM_CHARTOITEM = &H002F
			WM_SETFONT = &H0030
			WM_GETFONT = &H0031
			WM_SETHOTKEY = &H0032
			WM_GETHOTKEY = &H0033
			WM_QUERYDRAGICON = &H0037
			WM_COMPAREITEM = &H0039
			WM_GETOBJECT = &H003D
			WM_COMPACTING = &H0041
			WM_COMMNOTIFY = &H0044
			WM_WINDOWPOSCHANGING = &H0046
			WM_WINDOWPOSCHANGED = &H0047
			WM_POWER = &H0048
			WM_COPYDATA = &H004A
			WM_CANCELJOURNAL = &H004B
			WM_NOTIFY = &H004E
			WM_INPUTLANGCHANGEREQUEST = &H0050
			WM_INPUTLANGCHANGE = &H0051
			WM_TCARD = &H0052
			WM_HELP = &H0053
			WM_USERCHANGED = &H0054
			WM_NOTIFYFORMAT = &H0055
			WM_CONTEXTMENU = &H007B
			WM_STYLECHANGING = &H007C
			WM_STYLECHANGED = &H007D
			WM_DISPLAYCHANGE = &H007E
			WM_GETICON = &H007F
			WM_SETICON = &H0080
			WM_NCCREATE = &H0081
			WM_NCDESTROY = &H0082
			WM_NCCALCSIZE = &H0083
			WM_NCHITTEST = &H0084
			WM_NCPAINT = &H0085
			WM_NCACTIVATE = &H0086
			WM_GETDLGCODE = &H0087
			WM_SYNCPAINT = &H0088
			WM_NCMOUSEMOVE = &H00A0
			WM_NCLBUTTONDOWN = &H00A1
			WM_NCLBUTTONUP = &H00A2
			WM_NCLBUTTONDBLCLK = &H00A3
			WM_NCRBUTTONDOWN = &H00A4
			WM_NCRBUTTONUP = &H00A5
			WM_NCRBUTTONDBLCLK = &H00A6
			WM_NCMBUTTONDOWN = &H00A7
			WM_NCMBUTTONUP = &H00A8
			WM_NCMBUTTONDBLCLK = &H00A9
			WM_NCXBUTTONDOWN = &H00AB
			WM_NCXBUTTONUP = &H00AC
			WM_NCXBUTTONDBLCLK = &H00AD
			WM_KEYDOWN = &H0100
			WM_KEYUP = &H0101
			WM_CHAR = &H0102
			WM_DEADCHAR = &H0103
			WM_SYSKEYDOWN = &H0104
			WM_SYSKEYUP = &H0105
			WM_SYSCHAR = &H0106
			WM_SYSDEADCHAR = &H0107
			WM_KEYLAST = &H0108
			WM_IME_STARTCOMPOSITION = &H010D
			WM_IME_ENDCOMPOSITION = &H010E
			WM_IME_COMPOSITION = &H010F
			WM_IME_KEYLAST = &H010F
			WM_INITDIALOG = &H0110
			WM_COMMAND = &H0111
			WM_SYSCOMMAND = &H0112
			WM_TIMER = &H0113
			WM_HSCROLL = &H0114
			WM_VSCROLL = &H0115
			WM_INITMENU = &H0116
			WM_INITMENUPOPUP = &H0117
			WM_MENUSELECT = &H011F
			WM_MENUCHAR = &H0120
			WM_ENTERIDLE = &H0121
			WM_MENURBUTTONUP = &H0122
			WM_MENUDRAG = &H0123
			WM_MENUGETOBJECT = &H0124
			WM_UNINITMENUPOPUP = &H0125
			WM_MENUCOMMAND = &H0126
			WM_CTLCOLORMSGBOX = &H0132
			WM_CTLCOLOREDIT = &H0133
			WM_CTLCOLORLISTBOX = &H0134
			WM_CTLCOLORBTN = &H0135
			WM_CTLCOLORDLG = &H0136
			WM_CTLCOLORSCROLLBAR = &H0137
			WM_CTLCOLORSTATIC = &H0138
			WM_MOUSEMOVE = &H0200
			WM_LBUTTONDOWN = &H0201
			WM_LBUTTONUP = &H0202
			WM_LBUTTONDBLCLK = &H0203
			WM_RBUTTONDOWN = &H0204
			WM_RBUTTONUP = &H0205
			WM_RBUTTONDBLCLK = &H0206
			WM_MBUTTONDOWN = &H0207
			WM_MBUTTONUP = &H0208
			WM_MBUTTONDBLCLK = &H0209
			WM_MOUSEWHEEL = &H020A
			WM_XBUTTONDOWN = &H020B
			WM_XBUTTONUP = &H020C
			WM_XBUTTONDBLCLK = &H020D
			WM_PARENTNOTIFY = &H0210
			WM_ENTERMENULOOP = &H0211
			WM_EXITMENULOOP = &H0212
			WM_NEXTMENU = &H0213
			WM_SIZING = &H0214
			WM_CAPTURECHANGED = &H0215
			WM_MOVING = &H0216
			WM_DEVICECHANGE = &H0219
			WM_MDICREATE = &H0220
			WM_MDIDESTROY = &H0221
			WM_MDIACTIVATE = &H0222
			WM_MDIRESTORE = &H0223
			WM_MDINEXT = &H0224
			WM_MDIMAXIMIZE = &H0225
			WM_MDITILE = &H0226
			WM_MDICASCADE = &H0227
			WM_MDIICONARRANGE = &H0228
			WM_MDIGETACTIVE = &H0229
			WM_MDISETMENU = &H0230
			WM_ENTERSIZEMOVE = &H0231
			WM_EXITSIZEMOVE = &H0232
			WM_DROPFILES = &H0233
			WM_MDIREFRESHMENU = &H0234
			WM_IME_SETCONTEXT = &H0281
			WM_IME_NOTIFY = &H0282
			WM_IME_CONTROL = &H0283
			WM_IME_COMPOSITIONFULL = &H0284
			WM_IME_SELECT = &H0285
			WM_IME_CHAR = &H0286
			WM_IME_REQUEST = &H0288
			WM_IME_KEYDOWN = &H0290
			WM_IME_KEYUP = &H0291
			WM_MOUSEHOVER = &H02A1
			WM_MOUSELEAVE = &H02A3
			WM_CUT = &H0300
			WM_COPY = &H0301
			WM_PASTE = &H0302
			WM_CLEAR = &H0303
			WM_UNDO = &H0304
			WM_RENDERFORMAT = &H0305
			WM_RENDERALLFORMATS = &H0306
			WM_DESTROYCLIPBOARD = &H0307
			WM_DRAWCLIPBOARD = &H0308
			WM_PAINTCLIPBOARD = &H0309
			WM_VSCROLLCLIPBOARD = &H030A
			WM_SIZECLIPBOARD = &H030B
			WM_ASKCBFORMATNAME = &H030C
			WM_CHANGECBCHAIN = &H030D
			WM_HSCROLLCLIPBOARD = &H030E
			WM_QUERYNEWPALETTE = &H030F
			WM_PALETTEISCHANGING = &H0310
			WM_PALETTECHANGED = &H0311
			WM_HOTKEY = &H0312
			WM_PRINT = &H0317
			WM_PRINTCLIENT = &H0318
			WM_THEME_CHANGED = &H031A
			WM_HANDHELDFIRST = &H0358
			WM_HANDHELDLAST = &H035F
			WM_AFXFIRST = &H0360
			WM_AFXLAST = &H037F
			WM_PENWINFIRST = &H0380
			WM_PENWINLAST = &H038F
			WM_APP = &H8000
			WM_USER = &H0400
			WM_REFLECT = WM_USER + &H1c00
		End Enum


		''' ////////////////////////////////////////////////////////////
		'[DllImport("user32.dll")]
		'public static extern IntPtr GetDC(HandleRef hWnd);
		'[DllImport("user32.dll")]
		'public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		'[DllImport("gdi32.dll")]
		'public static extern int GetBkColor(IntPtr hdc);
		'[DllImport("gdi32.dll")]
		'public static extern uint SetBkColor(IntPtr hdc, int crColor);
		'[StructLayout(LayoutKind.Explicit, Size = 4)]
		'public struct COLORREF
		'{
		'    public COLORREF(byte r, byte g, byte b)
		'    {
		'        this.Value = 0;
		'        this.R = r;
		'        this.G = g;
		'        this.B = b;
		'    }

		'    public COLORREF(int value)
		'    {
		'        this.R = 0;
		'        this.G = 0;
		'        this.B = 0;
		'        unchecked{
		'            this.Value = value & (int)0x00FFFFFF;
		'        };
		'    }

		'    [FieldOffset(0)]
		'    public byte R;
		'    [FieldOffset(1)]
		'    public byte G;
		'    [FieldOffset(2)]
		'    public byte B;

		'    [FieldOffset(0)]
		'    public int Value;
		'}
	End Class
End Namespace


