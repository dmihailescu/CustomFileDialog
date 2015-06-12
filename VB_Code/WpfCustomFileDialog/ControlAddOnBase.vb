Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
' Copyright © Decebal Mihailescu 2015

' All rights reserved.
' This code is released under The Code Project Open License (CPOL) 1.02
' The full licensing terms are available at http://www.codeproject.com/info/cpol10.aspx
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
' KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
' IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
' PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
' REMAINS UNCHANGED.
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes

Namespace WpfCustomFileDialog
    ''' <summary>
    ''' Interaction logic for ControlAddOnBase.xaml
    ''' </summary>
    Partial Public Class ControlAddOnBase
        Inherits UserControl
        Implements IWindowExt

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

        Protected ReadOnly Property ParentDialog() As IFileDlgExt
            Get
                Return _parentDlg
            End Get
        End Property

#End Region

    End Class
End Namespace
