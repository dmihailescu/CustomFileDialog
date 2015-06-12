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
Imports System.Windows.Interop
Imports System.ComponentModel

Namespace WpfCustomFileDialog
    Public Interface IFileDlgExt
        Inherits IWin32Window
        Event EventFileNameChanged As PathChangedEventHandler
        Event EventFolderNameChanged As PathChangedEventHandler
        Event EventFilterChanged As FilterChangedEventHandler
        Property FileDlgStartLocation() As AddonWindowLocation
        Property FileDlgOkCaption() As String
        Property FileDlgEnableOkBtn() As Boolean
        WriteOnly Property FixedSize() As Boolean
        Property FileDlgDefaultViewMode() As NativeMethods.FolderViewMode
    End Interface
    'consider http://geekswithblogs.net/lbugnion/archive/2007/03/02/107747.aspx instead
    Public Interface IWindowExt

        WriteOnly Property Source() As HwndSource
        WriteOnly Property ParentDlg() As IFileDlgExt
    End Interface
End Namespace
