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
Imports System.Windows
Imports System.Runtime.InteropServices
Imports System.Windows.Controls

Namespace WpfCustomFileDialog

    Public NotInheritable Class SaveFileDialog(Of T As {ContentControl, IWindowExt, New})
        Inherits WpfCustomFileDialog.FileDialogExt(Of T)


        <SecurityCritical()> _
        Public Sub New()
            Me.Initialize()
        End Sub

        <SecurityCritical()> _
        Private Overloads Sub Initialize()
            MyBase.SetOption(2, True)
        End Sub

        <SecurityCritical()> _
        Public Function OpenFile() As Stream
            Dim str As String = If((MyBase.FileNamesInternal.Length > 0), MyBase.FileNamesInternal(0), Nothing)
            If String.IsNullOrEmpty(str) Then
                Throw New InvalidOperationException("FileNameMustNotBeNull")
            End If
            CType(New FileIOPermission(FileIOPermissionAccess.Append Or FileIOPermissionAccess.Write Or FileIOPermissionAccess.Read, str), FileIOPermission).Assert()
            Return New FileStream(str, FileMode.Create, FileAccess.ReadWrite)
        End Function

        <SecurityCritical()> _
        Private Function PromptFileCreate(ByVal fileName As String) As Boolean
            Return MyBase.MessageBoxWithFocusRestore(String.Format("Do you want to create {0} {1}?", Environment.NewLine, fileName), MessageBoxButton.YesNo, MessageBoxImage.Exclamation)
        End Function

        <SecurityCritical()> _
        Private Function PromptFileOverwrite(ByVal fileName As String) As Boolean
            Return MyBase.MessageBoxWithFocusRestore(String.Format("Do you want to overwite {0} {1}?", Environment.NewLine, fileName), MessageBoxButton.YesNo, MessageBoxImage.Exclamation)
        End Function

        <SecurityCritical()> _
        Friend Overrides Function PromptUserIfAppropriate(ByVal fileName As String) As Boolean
            Dim flag As Boolean
            If Not MyBase.PromptUserIfAppropriate(fileName) Then
                Return False
            End If
            CType(New FileIOPermission(PermissionState.Unrestricted), FileIOPermission).Assert()
            Try
                flag = File.Exists(Path.GetFullPath(fileName))
            Finally
                CodeAccessPermission.RevertAssert()
            End Try
            If (Me.CreatePrompt AndAlso (Not flag)) AndAlso (Not Me.PromptFileCreate(fileName)) Then
                Return False
            End If
            If (Me.OverwritePrompt AndAlso flag) AndAlso (Not Me.PromptFileOverwrite(fileName)) Then
                Return False
            End If
            Return True
        End Function

        <SecurityCritical()> _
        Public Overrides Sub Reset()
            MyBase.Reset()
            Me.Initialize()
        End Sub

        <SecurityCritical()> _
        Friend Overrides Function RunFileDialog(ByVal ofn As OPENFILENAME_I) As Boolean
            Dim saveFileName As Boolean = False
            saveFileName = NativeMethods.GetSaveFileName(ofn)
            If Not saveFileName Then
                Select Case NativeMethods.CommDlgExtendedError()
                    Case &H3001
                        Throw New InvalidOperationException("FileDialogSubClassFailure")

                    Case &H3002
                        Throw New InvalidOperationException("FileDialogInvalidFileName" & MyBase.SafeFileName)

                    Case &H3003
                        Throw New InvalidOperationException("FileDialogBufferTooSmall")
                End Select
            End If
            Return saveFileName
        End Function

        Public Property CreatePrompt() As Boolean
            Get
                Return MyBase.GetOption(&H2000)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)

                MyBase.SetOption(&H2000, value)
            End Set
        End Property

        Public Property OverwritePrompt() As Boolean
            Get
                Return MyBase.GetOption(2)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)
                MyBase.SetOption(2, value)
            End Set
        End Property
    End Class
End Namespace

