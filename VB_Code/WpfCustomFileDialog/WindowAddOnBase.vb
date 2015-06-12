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
Imports System.Windows
Imports System.Runtime.InteropServices

Namespace WpfCustomFileDialog
    Public Class WindowAddOnBase
        Inherits Window
        Implements IWindowExt
        'WPF: Inheriting from custom class instead of Window
        Private _source As System.Windows.Interop.HwndSource
        Private _parentDlg As IFileDlgExt
#Region "IWindowExt Members"
        Public WriteOnly Property Source() As System.Windows.Interop.HwndSource Implements IWindowExt.Source
            Set(ByVal value As System.Windows.Interop.HwndSource)
                _source = value
            End Set

        End Property

        Public WriteOnly Property ParentDlg() As IFileDlgExt Implements IWindowExt.ParentDlg
            Set(ByVal value As IFileDlgExt)
                _parentDlg = value
            End Set
        End Property
#End Region
        Protected Overrides Function ArrangeOverride(ByVal arrangeBounds As Size) As Size

            If Height > 0 AndAlso Width > 0 Then
                arrangeBounds.Height = Me.Height
                arrangeBounds.Width = Me.Width
            End If
            Return MyBase.ArrangeOverride(arrangeBounds)
        End Function
        Protected ReadOnly Property ParentDialog() As IFileDlgExt
            Get
                Return _parentDlg
            End Get
        End Property
    End Class
End Namespace
