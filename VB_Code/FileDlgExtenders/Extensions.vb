'  Copyright (c) 2006, Gustavo Franco
'  Copyright © Decebal Mihailescu 2015

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
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports Win32Types
Imports System.ComponentModel
Imports Microsoft.Win32

Namespace FileDialogExtenders
	Public Module Extensions
		#Region "extension Methods"
		<System.Runtime.CompilerServices.Extension> _
		Public Function ShowDialog(ByVal fdlg As FileDialog, ByVal ctrl As FileDialogControlBase, ByVal owner As IWin32Window) As DialogResult 'where T : FileDialogControlBase, new()
			ctrl.FileDlgType = If((TypeOf fdlg Is SaveFileDialog), FileDialogType.SaveFileDlg, FileDialogType.OpenFileDlg)
			If ctrl.ShowDialogExt(fdlg, owner) = DialogResult.OK Then
				Return DialogResult.OK
			Else
				Return DialogResult.Ignore
			End If
		End Function
		#End Region
	End Module

	'see http://msdn.microsoft.com/en-us/magazine/cc300434.aspx
	Public Module FileDialogPlaces
		Private ReadOnly TempKeyName As String = "TempPredefKey_" & Guid.NewGuid().ToString()
		Private Const Key_PlacesBar As String = "Software\Microsoft\Windows\CurrentVersion\Policies\ComDlg32\PlacesBar"
		Private _fakeKey As RegistryKey
		Private _overriddenKey As IntPtr
		Private m_places() As Object

		<System.Runtime.CompilerServices.Extension> _
		Public Sub SetPlaces(ByVal fd As FileDialog, ByVal places() As Object)
			If fd Is Nothing OrElse places Is Nothing Then
				Return
			End If
			If m_places Is Nothing Then
				m_places = New Object(places.GetLength(0) - 1){}
			End If

			For i As Integer = 0 To m_places.GetLength(0) - 1
				m_places(i) = places(i)
			Next i

			If _fakeKey IsNot Nothing Then
				ResetPlaces(fd)
			End If
			SetupFakeRegistryTree()
			If fd IsNot Nothing Then
				AddHandler fd.Disposed, Function(sender, e) AnonymousMethod1(sender, e, fd)
			End If
		End Sub
		
		Private Function AnonymousMethod1(ByVal sender As Object, ByVal e As EventArgs, ByVal fd As FileDialog) As Object
			If m_places IsNot Nothing AndAlso fd IsNot Nothing Then
				ResetPlaces(fd)
			End If
			Return Nothing
		End Function

'		static FileDialogPlaces()
'		{
'		}

		<System.Runtime.CompilerServices.Extension> _
		Public Sub ResetPlaces(ByVal fd As FileDialog)
			If _overriddenKey <> IntPtr.Zero Then
				ResetRegistry(_overriddenKey)
				_overriddenKey = IntPtr.Zero
			End If
			If _fakeKey IsNot Nothing Then
				_fakeKey.Close()
				_fakeKey = Nothing
			End If
			'delete the key tree
			Registry.CurrentUser.DeleteSubKeyTree(TempKeyName)
			m_places = Nothing
		End Sub

		Private Sub SetupFakeRegistryTree()
			Try
				_fakeKey = Registry.CurrentUser.CreateSubKey(TempKeyName)
				_overriddenKey = InitializeRegistry()

				' at this point, m_TempKeyName equals places key
				' write dynamic places here reading from Places
				Dim reg As RegistryKey = Registry.CurrentUser.CreateSubKey(Key_PlacesBar)
				For i As Integer = 0 To m_places.GetLength(0) - 1
					If m_places(i) IsNot Nothing Then
						reg.SetValue("Place" & i.ToString(), m_places(i))
					End If
				Next i

            Catch ex As UnauthorizedAccessException
                MessageBox.Show(ex.Message + Environment.NewLine + "You might need to restart as an administrator!", "Unable to set Places for Open/Save dialog")
            End Try

		End Sub

		'public static IntPtr GetRegistryHandle(RegistryKey registryKey)
		'{
		'    Type type = registryKey.GetType();
		'    FieldInfo fieldInfo = type.GetField("hkey", BindingFlags.Instance | BindingFlags.NonPublic);
		'    return (IntPtr)fieldInfo.GetValue(registryKey);
		'}
        Private ReadOnly HKEY_CURRENT_USER As New UIntPtr(&H80000001L)
		Private Function InitializeRegistry() As IntPtr
			Dim hkMyCU As IntPtr
			NativeMethods.RegCreateKeyW(HKEY_CURRENT_USER, TempKeyName, hkMyCU)
			NativeMethods.RegOverridePredefKey(HKEY_CURRENT_USER, hkMyCU)
			Return hkMyCU
		End Function


		Private Sub ResetRegistry(ByVal hkMyCU As IntPtr)
			Try
				NativeMethods.RegOverridePredefKey(HKEY_CURRENT_USER, IntPtr.Zero)
				NativeMethods.RegCloseKey(hkMyCU)
			Catch
			End Try
		End Sub

	End Module

    'Public Enum Places
    '	<Description("Desktop")> _
    '	Desktop = 0
    '	<Description("Program Files")> _
    '	Programs = 2
    '	<Description("Control Panel")> _
    '	ControlPanel = 3
    '	<Description("Printers")> _
    '	Printers = 4
    '	<Description("My Documents")> _
    '	MyDocuments = 5
    '	<Description("Favorites")> _
    '	Favorites = 6
    '	<Description("Startup folder")> _
    '	StartupFolder = 7
    '	<Description("Recent Files")> _
    '	RecentFiles = 8
    '	<Description("Send To")> _
    '	SendTo = 9
    '	<Description("Recycle Bin")> _
    '	RecycleBin = 10
    '	<Description("Start menu")> _
    '	StartMenu = 12
    '	<Description("My Computer")> _
    '	MyComputer = 17
    '	<Description("My Network Places")> _
    '	MyNetworkPlaces = 18
    '	<Description("Fonts")> _
    '	Fonts = 20
    'End Enum
    'http://www.codeguru.com/cpp/misc/misc/system/article.php/c13407/
    ' fot .net 4.0 and up use CustomPlaces instead :http://msdn.microsoft.com/en-us/library/microsoft.win32.filedialog.customplaces.aspx
    Public Enum Places
        <Description("Desktop")> _
        Desktop = 0
        <Description("Internet Explorer ")> _
        InternetExplorer = 1
        <Description("Program Files")> _
        Programs = 2
        <Description("Control Panel")> _
        ControlPanel = 3
        <Description("Printers")> _
        Printers = 4
        <Description("My Documents")> _
        MyDocuments = 5
        <Description("Favorites")> _
        Favorites = 6
        <Description("Startup folder")> _
        StartupFolder = 7

        <Description("Recent Files")> _
        RecentFiles = 8

        <Description("Send To")> _
        SendTo = 9

        <Description("Recycle Bin")> _
        RecycleBin = &HA

        <Description("Start menu")> _
        StartMenu = &HB

        <Description("Logical My Documents")> _
        Logical_MyDocuments = &HC

        <Description("My Music")> _
        MyMusic = &HD

        <Description("My Videos")> _
        MyVideos = &HE

        <Description("<user name>\Desktop")> _
        UserName_Desktop = &H10

        <Description("My Computer")> _
        MyComputer = &H11


        <Description("My Network Places")> _
        MyNetworkPlaces = 18

        <Description("<user name>" & Constants.vbLf & "ethood")> _
        User_Name_Nethood = &H13

        <Description("Fonts")> _
        Fonts = &H14

        <Description("All Users\Start Menu")> _
        All_Users_StartMenu = &H16


        <Description("All Users\Start Menu\Programs ")> _
        All_Users_StartMenu_Programs = &H17

        <Description("All Users\Startup")> _
        All_Users_Startup = &H18

        <Description("All Users\Desktop")> _
        All_Users_Desktop = &H19


        <Description("<user name>\Application Data ")> _
        User_name_ApplicationData = &H1A


        <Description("<user name>\PrintHood ")> _
        User_Name_PrintHood = &H1B

        <Description("<user name>\Local Settings\Applicaiton Data (nonroaming)")> _
        Local_ApplicaitonData = &H1C

        <Description("Nonlocalized common startup ")> _
        NonlocalizedCommonStartup = &H1E

        <Description("")> _
        CommonFavorites = &H1F


        <Description("Internet Cache ")> _
        InternetCache = &H20


        <Description("Cookies ")> _
        Cookies = &H21


        <Description("History")> _
        History = &H22


        <Description("All Users\Application Data ")> _
        All_Users_ApplicationData = &H23


        <Description("Windows Directory")> _
        WindowsDirectory = &H24


        <Description("System Directory")> _
        SystemDirectory = &H25


        <Description("Program Files ")> _
        ProgramFiles = &H26


        <Description("My Pictures ")> _
        MyPictures = &H27


        <Description("USERPROFILE")> _
        USERPROFILE = &H28

        <Description("system directory on RISC")> _
        SYSTEN_RISC = &H29

        <Description("Program Files on RISC ")> _
        Program_Files_RISC = &H2A


        <Description("Program Files\Common")> _
        Common = &H2B
        <Description("Program Files\Common on RISC")> _
        Common_RISC = &H2C
        <Description("All Users\Templates ")> _
        Templates = &H2D
        <Description("All Users\Documents")> _
        All_Users_Documents = &H2E
        <Description("All Users\Start Menu\Programs\Administrative Tools")> _
        AdministrativeTools = &H2F
        <Description("<user name>\Start Menu\Programs\Administrative Tools")> _
        USER_AdministrativeTools = &H30
        <Description("Network and Dial-up Connections")> _
        Network_DialUp_Connections = &H31
        <Description("All Users\My Music")> _
        All_Users_MyMusic = &H35
        <Description("All Users\My Pictures")> _
        All_Users_MyPictures = &H36
        <Description("All Users\My Video")> _
        All_Users_MyVideo = &H37
        <Description("Resource Directory")> _
        Resource = &H38
        <Description("Localized Resource Directory ")> _
        Localized_Resource = &H39
        <Description("OEM specific apps")> _
        OEM_Specific = &H3A
        <Description("USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning")> _
        CDBurning = &H3B

	End Enum
End Namespace
