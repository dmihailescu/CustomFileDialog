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
Imports System.IO
Imports System.Security
Imports System.Security.Permissions
Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.Reflection
Imports System.Windows.Controls

Namespace WpfCustomFileDialog
    'using MS.Internal.PresentationFramework;

    Public NotInheritable Class OpenFileDialog(Of T As {ContentControl, IWindowExt, New})
        Inherits WpfCustomFileDialog.FileDialogExt(Of T)




        <SecurityCritical()> _
        Public Sub New()
            Me.Initialize()

        End Sub


        <SecurityCritical()> _
        Private Overloads Sub Initialize()
            MyBase.SetOption(&H1000, True)
        End Sub

        <SecurityCritical()> _
        Public Function OpenFile() As Stream
            Dim str As String = MyBase.FileNamesInternal(0)
            If String.IsNullOrEmpty(str) Then
                Throw New InvalidOperationException("FileNameMustNotBeNull")
            End If
            Dim stream As FileStream = Nothing
            CType(New FileIOPermission(FileIOPermissionAccess.Read, str), FileIOPermission).Assert()
            Try
                stream = New FileStream(str, FileMode.Open, FileAccess.Read, FileShare.Read)
            Finally
                CodeAccessPermission.RevertAssert()
            End Try
            Return stream
        End Function

        <SecurityCritical()> _
        Public Function OpenFiles() As Stream()
            Dim fileNamesInternal() As String = MyBase.FileNamesInternal
            Dim streamArray(fileNamesInternal.Length - 1) As Stream
            For i As Integer = 0 To fileNamesInternal.Length - 1
                Dim str As String = fileNamesInternal(i)
                If String.IsNullOrEmpty(str) Then
                    Throw New InvalidOperationException("FileNameMustNotBeNull")
                End If
                Dim stream As FileStream = Nothing
                CType(New FileIOPermission(FileIOPermissionAccess.Read, str), FileIOPermission).Assert()
                Try
                    stream = New FileStream(str, FileMode.Open, FileAccess.Read, FileShare.Read)
                Finally
                    CodeAccessPermission.RevertAssert()
                End Try
                streamArray(i) = stream
            Next i
            Return streamArray
        End Function

        <SecurityCritical()> _
        Public Overrides Sub Reset()
            MyBase.Reset()
            Me.Initialize()
        End Sub


        <SecurityCritical()> _
        Friend Overrides Function RunFileDialog(ByVal ofn As OPENFILENAME_I) As Boolean

            Dim openFileName As Boolean = False
            openFileName = NativeMethods.GetOpenFileName(ofn)
            If Not openFileName Then

                Select Case NativeMethods.CommDlgExtendedError()

                    Case &H3001
                        Throw New InvalidOperationException("FileDialogSubClassFailure")

                    Case &H3002
                        Throw New InvalidOperationException("FileDialogInvalidFileName" & MyBase.SafeFileName)

                    Case &H3003
                        Throw New InvalidOperationException("FileDialogBufferTooSmall")
                End Select
            End If
            Return openFileName
        End Function

        Public Property Multiselect() As Boolean
            Get
                Return MyBase.GetOption(&H200)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)
                MyBase.SetOption(&H200, value)
            End Set
        End Property

        Public Property ReadOnlyChecked() As Boolean
            Get
                Return MyBase.GetOption(1)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)
                MyBase.SetOption(1, value)
            End Set
        End Property

        Public Property ShowReadOnly() As Boolean
            Get
                Return Not MyBase.GetOption(4)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)
                MyBase.SetOption(4, (Not value))
            End Set
        End Property
    End Class
End Namespace

