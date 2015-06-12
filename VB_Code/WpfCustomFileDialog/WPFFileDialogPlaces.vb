' Copyright © Decebal Mihailescu 2015

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
Imports System.ComponentModel
Imports Microsoft.Win32
Imports System.Windows


Namespace WpfCustomFileDialog
	'see http://msdn.microsoft.com/en-us/magazine/cc300434.aspx
    Partial Public MustInherit Class FileDialogExt(Of T As {System.Windows.Controls.ContentControl, IWindowExt, New})
        Private ReadOnly TempKeyName As String = "TempPredefKey_" & Guid.NewGuid().ToString()
        Private Const Key_PlacesBar As String = "Software\Microsoft\Windows\CurrentVersion\Policies\ComDlg32\PlacesBar"
        Private _fakeKey As RegistryKey
        Private _overriddenKey As IntPtr
        Private m_places() As Object

        Public Sub SetPlaces(ByVal places() As Object)
            If m_places Is Nothing Then
                m_places = New Object(4) {}
            Else
                m_places.Initialize()
            End If

            If places IsNot Nothing Then
                For i As Integer = 0 To m_places.GetLength(0) - 1
                    m_places(i) = places(i)

                Next i
            End If
        End Sub

        Public Sub ResetPlaces()
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

        'public  IntPtr GetRegistryHandle(RegistryKey registryKey)
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
            NativeMethods.RegOverridePredefKey(HKEY_CURRENT_USER, IntPtr.Zero)
            NativeMethods.RegCloseKey(hkMyCU)
            Return
        End Sub
    End Class

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