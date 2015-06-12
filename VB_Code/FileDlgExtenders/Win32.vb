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
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports FileDialogExtenders
Namespace Win32Types
	Friend NotInheritable Class NativeMethods
		#Region "Delegates"
		Friend Delegate Function EnumWindowsCallBack(ByVal hWnd As IntPtr, ByVal lParam As Integer) As Boolean
		#End Region

		#Region "USER32"

		<DllImport("user32.dll")> _
		Public Shared Function IsWindow(ByVal hwnd As IntPtr) As Boolean
		End Function
		<DllImport("user32.dll")> _
		Friend Shared Function CreateWindowEx(ByVal dwExStyle As UInteger, ByVal lpClassName As String, ByVal lpWindowName As String, ByVal dwStyle As UInteger, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hWndParent As IntPtr, ByVal hMenu As IntPtr, ByVal hInstance As IntPtr, ByVal lpParam As IntPtr) As IntPtr
		End Function
		<DllImport("user32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function GetParent(ByVal hWnd As IntPtr) As IntPtr
		End Function
		<DllImport("user32.dll")> _
		Friend Shared Function EnableWindow(ByVal hWnd As IntPtr, ByVal bEnable As Boolean) As Boolean
		End Function
		<DllImport("user32.dll")> _
		Friend Shared Function GetDlgItem(ByVal hDlg As IntPtr, ByVal nIDDlgItem As Integer) As IntPtr
		End Function
		<DllImport("user32.dll")> _
		Friend Shared Function SetDlgItemText(ByVal hDlg As IntPtr, ByVal nIDDlgItem As Integer, ByVal lpString As String) As Boolean
		End Function
		<DllImport("User32.Dll")> _
		Public Shared Function GetDlgCtrlID(ByVal hWndCtl As IntPtr) As Integer
		End Function
		<DllImport("user32.dll", SetLastError := True)> _
		Friend Shared Function GetWindowInfo(ByVal hwnd As HandleRef, <System.Runtime.InteropServices.Out()> ByRef pwi As WINDOWINFO) As <MarshalAs(UnmanagedType.Bool)> Boolean
		End Function
		<DllImport("user32.dll")> _
		Public Shared Function SetWindowText(ByVal hWnd As HandleRef, ByVal lpString As String) As <MarshalAs(UnmanagedType.Bool)> Boolean
		End Function
		<DllImport("User32.Dll")> _
		Public Shared Sub GetClassName(ByVal hWnd As HandleRef, ByVal param As StringBuilder, ByVal length As Integer)
		End Sub
		<DllImport("user32.Dll")> _
		Public Shared Function EnumChildWindows(ByVal hWndParent As HandleRef, ByVal lpEnumFunc As EnumWindowsCallBack, ByVal lParam As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
		End Function
		<DllImport("user32.dll", SetLastError := True)> _
		Public Shared Function FindWindowEx(ByVal parentHandle As IntPtr, ByVal childAfter As IntPtr, ByVal className As String, ByVal windowTitle As String) As IntPtr
		End Function
		<DllImport("user32.dll", SetLastError := True)> _
		Public Shared Function FindWindowEx(ByVal parentHandle As IntPtr, ByVal childAfter As IntPtr, ByVal className As String, ByVal windowTitle As IntPtr) As IntPtr
		End Function
		<DllImport("user32.dll")> _
		Friend Shared Function SetParent(ByVal hWndChild As HandleRef, ByVal hWndNewParent As HandleRef) As IntPtr
		End Function
		<DllImport("user32.dll", CharSet := CharSet.Auto, SetLastError := False)> _
		Friend Shared Function SendMessage(ByVal hWnd As HandleRef, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
		End Function
		<DllImport("user32.dll", CharSet := CharSet.Auto, SetLastError := False)> _
		Friend Shared Function SendMessage(ByVal hWnd As HandleRef, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As StringBuilder) As IntPtr
		End Function
		<DllImport("user32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal Width As Integer, ByVal Height As Integer, ByVal flags As SetWindowPosFlags) As Boolean
		End Function
		<DllImport("user32.dll")> _
		Friend Shared Function GetWindowRect(ByVal hwnd As HandleRef, ByRef rect As RECT) As <MarshalAs(UnmanagedType.Bool)> Boolean
		End Function
		<DllImport("user32.dll")> _
		Friend Shared Function GetClientRect(ByVal hwnd As HandleRef, ByRef rect As RECT) As <MarshalAs(UnmanagedType.Bool)> Boolean
		End Function
		<DllImport("user32.dll")> _
		Public Shared Function DestroyWindow(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
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
'
'        [DllImport("user32.dll")]
'        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
'        [DllImport("user32.dll")]
'        public static extern IntPtr SetForegeoundWindow(IntPtr hWnd);
'  
		#End Region
	End Class
End Namespace
