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
Imports System.Text
Imports System.Drawing
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports FileDialogExtenders

Namespace Win32Types
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

	#Region "POINT"

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure POINT
		Public x As Integer
		Public y As Integer

		#Region "Constructors"
		Public Sub New(ByVal x As Integer, ByVal y As Integer)
			Me.x = x
			Me.y = y
		End Sub

		Public Sub New(ByVal point As Point)
			x = point.X
			y = point.Y
		End Sub
		#End Region
	End Structure
	#End Region

	#Region "RECT"

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure RECT
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
	End Structure
	#End Region

	#Region "WINDOWPOS"

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure WINDOWPOS
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

	'#region NCCALCSIZE_PARAMS
	'internal struct NCCALCSIZE_PARAMS
	'{
	'    public RECT     rgrc1;
	'    public RECT     rgrc2;
	'    public RECT     rgrc3;
	'    public IntPtr   lppos;
	'}
	'#endregion

	#Region "NMHDR"

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure NMHDR
		Public hwndFrom As IntPtr
		Public idFrom As IntPtr
		Public code As UInteger
	End Structure
	#End Region
	#Region "NMHEADER"
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure NMHEADER
		Friend hdr As NMHDR
		Friend iItem As Integer
		Friend iButton As Integer
		Friend pItem As IntPtr
	End Structure
	#End Region
	#Region "OPENFILENAME"

	''' <summary>
	''' Defines the shape of hook procedures that can be called by the OpenFileDialog
	''' </summary>
	Friend Delegate Function OfnHookProc(ByVal hWnd As IntPtr, ByVal msg As UInt16, ByVal wParam As Int32, ByVal lParam As Int32) As IntPtr
	''' <summary>
	''' See the documentation for OPENFILENAME
	''' </summary>
	'typedef struct tagOFN { 
	'  DWORD         lStructSize; 
	'  HWND          hwndOwner; 
	'  HINSTANCE     hInstance; 
	'  LPCTSTR       lpstrFilter; 
	'  LPTSTR        lpstrCustomFilter; 
	'  DWORD         nMaxCustFilter; 
	'  DWORD         nFilterIndex; 
	'  LPTSTR        lpstrFile; 
	'  DWORD         nMaxFile; 
	'  LPTSTR        lpstrFileTitle; 
	'  DWORD         nMaxFileTitle; 
	'  LPCTSTR       lpstrInitialDir; 
	'  LPCTSTR       lpstrTitle; 
	'  DWORD         Flags; 
	'  WORD          nFileOffset; 
	'  WORD          nFileExtension; 
	'  LPCTSTR       lpstrDefExt; 
	'  LPARAM        lCustData; 
	'  LPOFNHOOKPROC lpfnHook; 
	'  LPCTSTR       lpTemplateName; 
	'#if (_WIN32_WINNT >= 0x0500)
	'  void *        pvReserved;
	'  DWORD         dwReserved;
	'  DWORD         FlagsEx;
	'#endif // (_WIN32_WINNT >= 0x0500)
	'} OPENFILENAME, *LPOPENFILENAME;
	<StructLayout(LayoutKind.Sequential, CharSet := CharSet.Auto)> _
	Friend Structure OPENFILENAME
		Public lStructSize As UInt32
		Public hwndOwner As IntPtr
		Public hInstance As IntPtr
		Public lpstrFilter As String
		Public lpstrCustomFilter As String
		Public nMaxCustFilter As UInt32
		Public nFilterIndex As Int32
		Public lpstrFile As String
		Public nMaxFile As UInt32
		Public lpstrFileTitle As String
		Public nMaxFileTitle As UInt32
		Public lpstrInitialDir As String
		Public lpstrTitle As String
		Public Flags As UInt32
		Public nFileOffset As UInt16
		Public nFileExtension As UInt16
		Public lpstrDefExt As String
		Public lCustData As IntPtr
		Public lpfnHook As OfnHookProc
		Public lpTemplateName As String
		Public pvReserved As IntPtr
		Public dwReserved As UInt32
		Public FlagsEx As UInt32
	End Structure
	#End Region
	#Region "OFNOTIFY"

	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure OFNOTIFY
		Public hdr As NMHDR
		Public OpenFileName As IntPtr
		Public fileNameShareViolation As IntPtr
	End Structure
	#End Region
End Namespace
