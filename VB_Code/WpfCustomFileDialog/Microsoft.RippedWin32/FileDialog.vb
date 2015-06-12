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
Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Permissions
Imports System.Text
Imports System.Windows
Imports System.Reflection
Imports System.Windows.Interop
Imports System.Windows.Controls

Namespace WpfCustomFileDialog
    Partial Public MustInherit Class FileDialogExt(Of T As {ContentControl, IWindowExt, New})
        Inherits Microsoft.Win32.CommonDialog
        Implements IFileDlgExt

        <SecurityCritical()> _
        Private _charBuffer As CharBuffer
        Private _defaultExtension As String
        Private _dialogOptions As Integer
        <SecurityCritical()> _
        Private _fileNames() As String
        Private _fileOkNotificationCount As Integer
        Private _filter As String
        Private _filterIndex As Integer
        <SecurityCritical()> _
        Private _hwndFileDialog As IntPtr
        Private _hwndFileDialogEmbedded As IntPtr
        Private _ignoreSecondFileOkNotification As Boolean
        Private _initialDirectory As String
        Private _title As String
        Private Const FILEBUFSIZE As Integer = &H2000
        Private Const OPTION_ADDEXTENSION As Integer = -2147483648
        Public Event FileOk As CancelEventHandler

        <SecurityCritical(), SecurityTreatAsSafe()> _
        Protected Sub New()
            Me.Initialize()
        End Sub

        <SecurityCritical()> _
        Private Function DoFileOk(ByVal lpOFN As IntPtr) As Boolean
            Dim openfilename_i As OPENFILENAME_I = CType(Marshal.PtrToStructure(lpOFN, GetType(OPENFILENAME_I)), OPENFILENAME_I)
            Dim num As Integer = Me._dialogOptions
            Dim num2 As Integer = Me._filterIndex
            Dim strArray() As String = Me._fileNames
            Dim flag As Boolean = False
            Try
                Me._dialogOptions = (Me._dialogOptions And -2) Or (openfilename_i.Flags And 1)
                Me._filterIndex = openfilename_i.nFilterIndex
                Me._charBuffer.PutCoTaskMem(openfilename_i.lpstrFile)
                If Not Me.GetOption(&H200) Then
                    Me._fileNames = New String() {Me._charBuffer.GetString()}
                Else
                    Me._fileNames = GetMultiselectFiles(Me._charBuffer)
                End If
                If Me.ProcessFileNames() Then
                    Dim e As New CancelEventArgs()
                    Me.OnFileOk(e)
                    flag = Not e.Cancel
                End If
            Finally
                If Not flag Then
                    Me._dialogOptions = num
                    Me._filterIndex = num2
                    Me._fileNames = strArray
                End If
            End Try
            Return flag
        End Function

        Private Function GetFilterExtensions() As String()
            Dim str As String = Me._filter
            Dim list As New List(Of String)()
            If Me._defaultExtension IsNot Nothing Then
                list.Add(Me._defaultExtension)
            End If
            If str IsNot Nothing Then
                Dim strArray() As String = str.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
                Dim index As Integer = (Me._filterIndex * 2) - 1
                If index >= strArray.Length Then
                    Throw New InvalidOperationException("FileDialogInvalidFilterIndex")
                End If
                If Me._filterIndex > 0 Then
                    For Each str2 As String In strArray(index).Split(New Char() {";"c})
                        Dim num2 As Integer = str2.LastIndexOf("."c)
                        If num2 >= 0 Then
                            list.Add(str2.Substring(num2 + 1, str2.Length - (num2 + 1)))
                        End If
                    Next str2
                End If
            End If
            Return list.ToArray()
        End Function

        Private Shared Function GetMultiselectFiles(ByVal charBuffer As CharBuffer) As String()
            Dim str As String = charBuffer.GetString()
            Dim str2 As String = charBuffer.GetString()
            If str2.Length = 0 Then
                Return New String() {str}
            End If
            If Not str.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) Then
                str = str & Path.DirectorySeparatorChar
            End If
            Dim list As New List(Of String)()
            Do
                Dim flag As Boolean = (str2.Chars(0) = Path.DirectorySeparatorChar) AndAlso (str2.Chars(1) = Path.DirectorySeparatorChar)
                Dim flag2 As Boolean = ((str2.Length > 3) AndAlso (str2.Chars(1) = Path.VolumeSeparatorChar)) AndAlso (str2.Chars(2) = Path.DirectorySeparatorChar)
                If (Not flag) AndAlso (Not flag2) Then
                    str2 = str & str2
                End If
                list.Add(str2)
                str2 = charBuffer.GetString()
            Loop While Not String.IsNullOrEmpty(str2)
            Return list.ToArray()
        End Function

        Friend Function GetOption(ByVal [option] As Integer) As Boolean
            Return ((Me._dialogOptions And [option]) <> 0)
        End Function


        'HookProc has been changed
        <SecurityCritical()> _
        Protected Overrides Function HookProc(ByVal hwnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
            Dim hres As IntPtr = IntPtr.Zero

            Select Case CType(msg, NativeMethods.Msg)

                Case NativeMethods.Msg.WM_NOTIFY
                    hres = ProcOnNotify(hwnd, lParam)

                Case NativeMethods.Msg.WM_SHOWWINDOW
                    InitControls()
                    ShowChild()
                Case NativeMethods.Msg.WM_DESTROY
                Case NativeMethods.Msg.WM_NCDESTROY
                    _source.Dispose()

                Case NativeMethods.Msg.WM_INITDIALOG
                    _hwndFileDialog = NativeMethods.GetParent(New HandleRef(Me, hwnd))
                    _hwndFileDialogEmbedded = hwnd
                    NativeMethods.GetWindowRect(New HandleRef(Me, _hwndFileDialog), _OriginalRect)
                Case NativeMethods.Msg.WM_WINDOWPOSCHANGING
                    ' PositionChanging(lParam, ref dialogWndRect);
                Case NativeMethods.Msg.WM_SIZE
                Case NativeMethods.Msg.WM_NCCALCSIZE
                    'CalcSize(wParam, lParam);
                Case NativeMethods.Msg.WM_WINDOWPOSCHANGED

                Case NativeMethods.Msg.WM_SETREDRAW
                Case NativeMethods.Msg.WM_CHILDACTIVATE
                Case NativeMethods.Msg.WM_SETFONT

                Case Else
                    If msg = CInt(Fix(MSG_POST_CREATION)) Then
                        CustomPostCreation()
                    End If
                    Exit Select
            End Select 'switch ends

            Return hres
        End Function

        Private Shared Sub CalcSize(ByVal wParam As IntPtr, ByVal lParam As IntPtr)
            If wParam = IntPtr.Zero Then
                Dim rc As RECT = CType(Marshal.PtrToStructure(lParam, GetType(RECT)), RECT)
                Marshal.StructureToPtr(rc, lParam, True)
            Else

                Dim csp As NativeMethods.NCCALCSIZE_PARAMS
                csp = CType(Marshal.PtrToStructure(lParam, GetType(NativeMethods.NCCALCSIZE_PARAMS)), NativeMethods.NCCALCSIZE_PARAMS)
                Dim pos As WINDOWPOS = CType(Marshal.PtrToStructure(csp.lppos, GetType(WINDOWPOS)), WINDOWPOS)
                Marshal.StructureToPtr(pos, csp.lppos, True)
                Marshal.StructureToPtr(csp, lParam, True)

            End If
        End Sub



        Private Sub PositionChanging(ByVal lParam As IntPtr, ByRef dialogClientRect As RECT)
            Dim wnd As Window = TryCast(_childWnd, Window)
            'Resize FileDialog to make fit our extra window
            'called only when initializig
            Dim pos As WINDOWPOS = CType(Marshal.PtrToStructure(lParam, GetType(WINDOWPOS)), WINDOWPOS)
            If pos.flags <> 0 AndAlso ((pos.flags And CInt(SWP_Flags.SWP_NOSIZE)) <> CInt(SWP_Flags.SWP_NOSIZE)) Then
                NativeMethods.GetClientRect(New HandleRef(Me, _hwndFileDialog), dialogClientRect)
                Select Case Me.FileDlgStartLocation
                    Case AddonWindowLocation.Right
                        pos.cx += CInt(Fix(dialogClientRect.Width))
                        Marshal.StructureToPtr(pos, lParam, True)
                        wnd.Height = dialogClientRect.Height

                    Case AddonWindowLocation.Bottom
                        pos.cy += CInt(Fix(dialogClientRect.Height))
                        Marshal.StructureToPtr(pos, lParam, True)
                        wnd.Width = dialogClientRect.Width

                    Case AddonWindowLocation.BottomRight
                        pos.cy += CInt(Fix(dialogClientRect.Height))
                        pos.cx += CInt(Fix(dialogClientRect.Width))
                        Marshal.StructureToPtr(pos, lParam, True)
                End Select
            End If
        End Sub

        Private Function ProcOnNotify(ByVal hwnd As IntPtr, ByVal lParam As IntPtr) As IntPtr
            Dim hres As IntPtr = IntPtr.Zero
            Dim [structure] As OFNOTIFY = CType(Marshal.PtrToStructure(lParam, GetType(OFNOTIFY)), OFNOTIFY)
            Select Case CType([structure].hdr_code, NativeMethods.DialogChangeStatus)
                Case NativeMethods.DialogChangeStatus.CDN_FILEOK '- 606:
                    If Me._ignoreSecondFileOkNotification Then
                        If Me._fileOkNotificationCount <> 0 Then
                            Me._ignoreSecondFileOkNotification = False
                            NativeMethods.CriticalSetWindowLong(New HandleRef(Me, hwnd), 0, InvalidIntPtr)
                            hres = InvalidIntPtr
                            Exit Select
                        End If
                        Me._fileOkNotificationCount = 1
                    End If

                    If Not Me.DoFileOk([structure].lpOFN) Then
                        NativeMethods.CriticalSetWindowLong(New HandleRef(Me, hwnd), 0, InvalidIntPtr)
                        hres = InvalidIntPtr
                    End If
                Case NativeMethods.DialogChangeStatus.CDN_TYPECHANGE
                    Dim ofn As OPENFILENAME_I = CType(Marshal.PtrToStructure([structure].lpOFN, GetType(OPENFILENAME_I)), OPENFILENAME_I)
                    Dim i As Integer = ofn.nFilterIndex

                    OnFilterChanged(Me, i)

                Case NativeMethods.DialogChangeStatus.CDN_HELP, NativeMethods.DialogChangeStatus.CDN_FOLDERCHANGE ' - 605:
                    Dim folderPath As New StringBuilder(256)
                    NativeMethods.SendMessage(New HandleRef(Me, [structure].hdr_hwndFrom), CInt(DialogChangeProperties.CDM_GETFOLDERPATH), CType(256, IntPtr), folderPath)
                    OnPathChanged(Me, folderPath.ToString())
                    folderPath.Length = 0

                Case NativeMethods.DialogChangeStatus.CDN_SHAREVIOLATION '- 604:
                    Me._ignoreSecondFileOkNotification = True
                    Me._fileOkNotificationCount = 0

                Case NativeMethods.DialogChangeStatus.CDN_SELCHANGE '- 602:
                    Dim openfilename_i As OPENFILENAME_I = CType(Marshal.PtrToStructure([structure].lpOFN, GetType(OPENFILENAME_I)), OPENFILENAME_I)
                    Dim num As Integer = CInt(NativeMethods.UnsafeSendMessage(Me._hwndFileDialog, &H464, IntPtr.Zero, IntPtr.Zero))
                    If num > openfilename_i.nMaxFile Then
                        Dim size As Integer = num + &H800
                        Dim buffer As CharBuffer = CharBuffer.CreateBuffer(size)
                        Dim ptr2 As IntPtr = buffer.AllocCoTaskMem()
                        Marshal.FreeCoTaskMem(openfilename_i.lpstrFile)
                        openfilename_i.lpstrFile = ptr2
                        openfilename_i.nMaxFile = size
                        Me._charBuffer = buffer
                        Marshal.StructureToPtr(openfilename_i, [structure].lpOFN, True)
                        Marshal.StructureToPtr([structure], lParam, True)
                    End If
                    Dim filePath As New StringBuilder(256)
                    NativeMethods.SendMessage(New HandleRef(Me, [structure].hdr_hwndFrom), CUInt(DialogChangeProperties.CDM_GETFILEPATH), CType(256, IntPtr), filePath)
                    OnPathChanged(Me, filePath.ToString())
                    filePath.Length = 0
                    Exit Select

                Case NativeMethods.DialogChangeStatus.CDN_INITDONE '- 601:
                    NativeMethods.PostMessage(New HandleRef(Me, Me._hwndFileDialogEmbedded), MSG_POST_CREATION, IntPtr.Zero, IntPtr.Zero)
            End Select
            Return hres
        End Function




        <SecurityCritical()> _
        Private Sub Initialize()
            Me._dialogOptions = 0
            Me.SetOption(4, True)
            Me.SetOption(&H800, True)
            Me.SetOption(-2147483648, True)
            Me._title = Nothing
            Me._initialDirectory = Nothing
            Me._defaultExtension = Nothing
            Me._fileNames = Nothing
            Me._filter = Nothing
            Me._filterIndex = 1
            Me._ignoreSecondFileOkNotification = False
            Me._fileOkNotificationCount = 0
        End Sub

        Private Shared Function MakeFilterString(ByVal s As String, ByVal dereferenceLinks As Boolean) As String
            If String.IsNullOrEmpty(s) Then
                If (Not dereferenceLinks) OrElse (Environment.OSVersion.Version.Major < 5) Then
                    Return Nothing
                End If
                s = " |*.*"
            End If
            Dim builder As New StringBuilder(s)
            builder.Replace("|"c, ControlChars.NullChar)
            builder.Append(ControlChars.NullChar)
            builder.Append(ControlChars.NullChar)
            Return builder.ToString()
        End Function


        <SecurityCritical()> _
        Friend Function MessageBoxWithFocusRestore(ByVal message As String, ByVal buttons As MessageBoxButton, ByVal image As MessageBoxImage) As Boolean
            Dim flag As Boolean = False
            Dim focus As IntPtr = NativeMethods.GetFocus()
            Try
                flag = MessageBox.Show(message, Me.DialogCaption, buttons, image, MessageBoxResult.OK, MessageBoxOptions.None) = MessageBoxResult.Yes
            Finally
                NativeMethods.SetFocus(New HandleRef(Me, focus))
            End Try
            Return flag
        End Function

        Protected Sub OnFileOk(ByVal e As CancelEventArgs)
            If Me.FileOkEvent IsNot Nothing Then
                RaiseEvent FileOk(Me, e)
            End If
        End Sub

        <SecurityCritical(), SecurityTreatAsSafe()> _
        Private Function ProcessFileNames() As Boolean
            If Not Me.GetOption(&H100) Then
                Dim filterExtensions() As String = Me.GetFilterExtensions()
                For i As Integer = 0 To Me._fileNames.Length - 1
                    Dim path As String = Me._fileNames(i)
                    If Me.AddExtension AndAlso (Not System.IO.Path.HasExtension(path)) Then
                        For j As Integer = 0 To filterExtensions.Length - 1
                            Dim extension As String = System.IO.Path.GetExtension(path)
                            Dim builder As New StringBuilder(path.Substring(0, path.Length - extension.Length))
                            If filterExtensions(j).IndexOfAny(New Char() {"*"c, "?"c}) = -1 Then
                                builder.Append(".")
                                builder.Append(filterExtensions(j))
                            End If
                            If (Not Me.GetOption(&H1000)) OrElse File.Exists(builder.ToString()) Then
                                path = builder.ToString()
                                Exit For
                            End If
                        Next j
                        Me._fileNames(i) = path
                    End If
                    If Not Me.PromptUserIfAppropriate(path) Then
                        Return False
                    End If
                Next i
            End If
            Return True
        End Function

        <SecurityCritical()> _
        Private Sub PromptFileNotFound(ByVal fileName As String)
            Me.MessageBoxWithFocusRestore("FileDialogFileNotFound" & fileName, MessageBoxButton.OK, MessageBoxImage.Exclamation)
        End Sub

        <SecurityCritical()> _
        Friend Overridable Function PromptUserIfAppropriate(ByVal fileName As String) As Boolean
            Dim flag As Boolean = True
            If Me.GetOption(&H1000) Then
                CType(New FileIOPermission(FileIOPermissionAccess.PathDiscovery Or FileIOPermissionAccess.Read, fileName), FileIOPermission).Assert()
                Try
                    flag = File.Exists(Path.GetFullPath(fileName))
                Finally
                    CodeAccessPermission.RevertAssert()
                End Try
                If Not flag Then
                    Me.PromptFileNotFound(fileName)
                End If
            End If
            Return flag
        End Function

        <SecurityCritical()> _
        Public Overrides Sub Reset()
            Me.Initialize()
        End Sub

        <SecurityCritical()> _
        Protected Overrides Function RunDialog(ByVal hwndOwner As IntPtr) As Boolean
            Dim flag As Boolean
            Dim proc As New OPENFILENAME_I.WndProc(AddressOf Me.HookProc)
            Dim ofn As New OPENFILENAME_I()
            Try
                Me._charBuffer = CharBuffer.CreateBuffer(&H2000)
                If Me._fileNames IsNot Nothing Then
                    Me._charBuffer.PutString(Me._fileNames(0))
                End If
                ofn.lStructSize = Marshal.SizeOf(GetType(OPENFILENAME_I))
                ofn.hwndOwner = hwndOwner
                ofn.hInstance = IntPtr.Zero
                ofn.lpstrFilter = MakeFilterString(Me._filter, Me.DereferenceLinks)
                ofn.nFilterIndex = Me._filterIndex
                ofn.lpstrFile = Me._charBuffer.AllocCoTaskMem()
                ofn.nMaxFile = Me._charBuffer.Length
                ofn.lpstrInitialDir = Me._initialDirectory
                ofn.lpstrTitle = Me._title
                ofn.Flags = Me.Options Or &H880020
                ofn.lpfnHook = proc
                ofn.FlagsEx = &H1000000
                If (Me._defaultExtension IsNot Nothing) AndAlso Me.AddExtension Then
                    ofn.lpstrDefExt = Me._defaultExtension
                End If
                If _fakeKey IsNot Nothing Then
                    ResetPlaces()
                End If
                If m_places IsNot Nothing Then
                    SetupFakeRegistryTree()
                End If
                flag = Me.RunFileDialog(ofn)

            Finally
                Me._charBuffer = Nothing
                If ofn.lpstrFile <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(ofn.lpstrFile)
                End If
                Try
                    If m_places IsNot Nothing Then
                        ResetPlaces()
                    End If
                Catch
                End Try
            End Try
            Return flag
        End Function
        Private Sub CenterDialogToScreen()
            Dim ownerWindowHandle As Object = GetProperty(Me, "OwnerWindowHandle")
            If ownerWindowHandle Is Nothing Then 'if (Environment.Version.Major >= 4)
                InvokeMethod(Me, "MoveToScreenCenter", New HandleRef(Me, Me._hwndFileDialog))
            Else
                InvokeMethod(GetType(Microsoft.Win32.CommonDialog), "MoveToScreenCenter", New HandleRef(Me, Me._hwndFileDialog), New HandleRef(Me, CType(ownerWindowHandle, IntPtr)))
            End If
        End Sub

        Friend MustOverride Function RunFileDialog(ByVal ofn As OPENFILENAME_I) As Boolean
        <SecurityCritical()> _
        Friend Sub SetOption(ByVal [option] As Integer, ByVal value As Boolean)
            If value Then
                Me._dialogOptions = Me._dialogOptions Or [option]
            Else
                Me._dialogOptions = Me._dialogOptions And Not [option]
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim builder As New StringBuilder(MyBase.ToString() & ": Title: " & Me.Title & ", FileName: ")
            builder.Append(Me.FileName)
            Return builder.ToString()
        End Function

        Public Property AddExtension() As Boolean
            Get
                Return Me.GetOption(-2147483648)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)
                Me.SetOption(-2147483648, value)
            End Set
        End Property

        Public Overridable Property CheckFileExists() As Boolean
            Get
                Return Me.GetOption(&H1000)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)
                Me.SetOption(&H1000, value)
            End Set
        End Property

        Public Property CheckPathExists() As Boolean
            Get
                Return Me.GetOption(&H800)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)
                Me.SetOption(&H800, value)
            End Set
        End Property

        Private ReadOnly Property CriticalFileName() As String
            <SecurityCritical()> _
            Get
                If (Me._fileNames IsNot Nothing) AndAlso (Me._fileNames(0).Length > 0) Then
                    Return Me._fileNames(0)
                End If
                Return String.Empty
            End Get
        End Property

        Public Property DefaultExt() As String
            Get
                If Me._defaultExtension IsNot Nothing Then
                    Return Me._defaultExtension
                End If
                Return String.Empty
            End Get
            Set(ByVal value As String)
                If value IsNot Nothing Then
                    If value.StartsWith(".", StringComparison.Ordinal) Then
                        value = value.Substring(1)
                    ElseIf value.Length = 0 Then
                        value = Nothing
                    End If
                End If
                Me._defaultExtension = value
            End Set
        End Property

        Public Property DereferenceLinks() As Boolean
            Get
                Return Not Me.GetOption(&H100000)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)
                Me.SetOption(&H100000, (Not value))
            End Set
        End Property




        Private ReadOnly Property DialogCaption() As String
            <SecurityCritical()> _
            Get
                If Not NativeMethods.IsWindow(New HandleRef(Me, Me._hwndFileDialog)) Then
                    Return String.Empty
                End If
                Dim lpString As New StringBuilder(NativeMethods.GetWindowTextLength(New HandleRef(Me, Me._hwndFileDialog)) + 1)
                NativeMethods.GetWindowText(New HandleRef(Me, Me._hwndFileDialog), lpString, lpString.Capacity)
                Return lpString.ToString()
            End Get
        End Property

        Public Property FileName() As String
            <SecurityCritical()> _
            Get
                Return Me.CriticalFileName
            End Get
            <SecurityCritical()> _
            Set(ByVal value As String)
                If value Is Nothing Then
                    Me._fileNames = Nothing
                Else
                    Me._fileNames = New String() {value}
                End If
            End Set
        End Property

        Public ReadOnly Property FileNames() As String()
            <SecurityCritical()> _
            Get
                Return Me.FileNamesInternal
            End Get
        End Property

        Friend ReadOnly Property FileNamesInternal() As String()
            <SecurityCritical()> _
            Get
                If Me._fileNames Is Nothing Then
                    Return New String() {}
                End If
                Return CType(Me._fileNames.Clone(), String())
            End Get
        End Property

        Public Property Filter() As String
            Get
                If Me._filter IsNot Nothing Then
                    Return Me._filter
                End If
                Return String.Empty
            End Get
            Set(ByVal value As String)
                If String.CompareOrdinal(value, Me._filter) <> 0 Then
                    Dim str As String = value
                    If Not String.IsNullOrEmpty(str) Then
                        If (str.Split(New Char() {"|"c}).Length Mod 2) <> 0 Then
                            Throw New ArgumentException("FileDialogInvalidFilter")
                        End If
                    Else
                        str = Nothing
                    End If
                    Me._filter = str
                End If
            End Set
        End Property

        Public Property FilterIndex() As Integer
            Get
                Return Me._filterIndex
            End Get
            Set(ByVal value As Integer)
                Me._filterIndex = value
            End Set
        End Property

        Public Property InitialDirectory() As String
            Get
                If Me._initialDirectory IsNot Nothing Then
                    Return Me._initialDirectory
                End If
                Return String.Empty
            End Get
            <SecurityCritical()> _
            Set(ByVal value As String)
                Me._initialDirectory = value
            End Set
        End Property

        Protected ReadOnly Property Options() As Integer
            Get
                Return (Me._dialogOptions And &H100B0D)
            End Get
        End Property

        Public Property RestoreDirectory() As Boolean
            Get
                Return Me.GetOption(8)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)

                Me.SetOption(8, value)
            End Set
        End Property

        Public ReadOnly Property SafeFileName() As String
            <SecurityCritical()> _
            Get
                Dim fileName As String = Path.GetFileName(Me.CriticalFileName)
                If fileName Is Nothing Then
                    fileName = String.Empty
                End If
                Return fileName
            End Get
        End Property

        Public ReadOnly Property SafeFileNames() As String()
            <SecurityCritical()> _
            Get
                Dim fileNamesInternal() As String = Me.FileNamesInternal
                Dim strArray2(fileNamesInternal.Length - 1) As String
                For i As Integer = 0 To fileNamesInternal.Length - 1
                    strArray2(i) = Path.GetFileName(fileNamesInternal(i))
                    If strArray2(i) Is Nothing Then
                        strArray2(i) = String.Empty
                    End If
                Next i
                Return strArray2
            End Get
        End Property

        Public Property Title() As String
            Get
                If Me._title IsNot Nothing Then
                    Return Me._title
                End If
                Return String.Empty
            End Get
            <SecurityCritical()> _
            Set(ByVal value As String)

                Me._title = value
            End Set
        End Property

        Public Property ValidateNames() As Boolean
            Get
                Return Not Me.GetOption(&H100)
            End Get
            <SecurityCritical()> _
            Set(ByVal value As Boolean)
                Me.SetOption(&H100, (Not value))
            End Set
        End Property


    End Class
End Namespace

