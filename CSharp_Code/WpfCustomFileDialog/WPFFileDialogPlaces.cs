// Copyright © Decebal Mihailescu 2015

// All rights reserved.
// This code is released under The Code Project Open License (CPOL) 1.02
// The full licensing terms are available at http://www.codeproject.com/info/cpol10.aspx
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
// REMAINS UNCHANGED.
using System;
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows;

namespace WpfCustomFileDialog
{
    //see http://msdn.microsoft.com/en-us/magazine/cc300434.aspx
    //public  class FileDialogPlaces
    public abstract partial class FileDialogExt<T> //: Microsoft.Win32.CommonDialog, IFileDlgExt where T : System.Windows.Controls.ContentControl, IWindowExt, new()
    {
        private readonly string TempKeyName = "TempPredefKey_" + Guid.NewGuid().ToString();
        private const string Key_PlacesBar = @"Software\Microsoft\Windows\CurrentVersion\Policies\ComDlg32\PlacesBar";
        private RegistryKey _fakeKey;
        private IntPtr _overriddenKey;
        private object[] m_places;

        public void SetPlaces(object[] places)
        {
            if (m_places == null)
                m_places = new object[5];
            else
                m_places.Initialize();

            if (places != null)
            {
                for (int i = 0; i < m_places.GetLength(0); i++)
                {
                    m_places[i] = places[i];

                }
            }
        }

        public void ResetPlaces()
        {
            if (_overriddenKey != IntPtr.Zero)
            {
                ResetRegistry(_overriddenKey);
                _overriddenKey = IntPtr.Zero;
            }
            if (_fakeKey != null)
            {
                _fakeKey.Close();
                _fakeKey = null;
            }
            //delete the key tree
            Registry.CurrentUser.DeleteSubKeyTree(TempKeyName);
            m_places = null;
        }

        private void SetupFakeRegistryTree()
        {
            try
            {
                _fakeKey = Registry.CurrentUser.CreateSubKey(TempKeyName);
                _overriddenKey = InitializeRegistry();

                // at this point, m_TempKeyName equals places key
                // write dynamic places here reading from Places

                RegistryKey reg = Registry.CurrentUser.CreateSubKey(Key_PlacesBar);
                for (int i = 0; i < m_places.GetLength(0); i++)
                {
                    if (m_places[i] != null)
                    {
                        reg.SetValue("Place" + i.ToString(), m_places[i]);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {

                MessageBox.Show(ex.Message + Environment.NewLine + "You might need to restart as an administrator!", "Unable to set Places for Open/Save dialog");
            }
        }

        //public  IntPtr GetRegistryHandle(RegistryKey registryKey)
        //{
        //    Type type = registryKey.GetType();
        //    FieldInfo fieldInfo = type.GetField("hkey", BindingFlags.Instance | BindingFlags.NonPublic);
        //    return (IntPtr)fieldInfo.GetValue(registryKey);
        //}
        readonly UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);
        private IntPtr InitializeRegistry()
        {
            IntPtr hkMyCU;
            NativeMethods.RegCreateKeyW(HKEY_CURRENT_USER, TempKeyName, out hkMyCU);
            NativeMethods.RegOverridePredefKey(HKEY_CURRENT_USER, hkMyCU);
            return hkMyCU;
        }


        private void ResetRegistry(IntPtr hkMyCU)
        {
            NativeMethods.RegOverridePredefKey(HKEY_CURRENT_USER, IntPtr.Zero);
            NativeMethods.RegCloseKey(hkMyCU);
            return;
        }
    }

    //http://www.codeguru.com/cpp/misc/misc/system/article.php/c13407/
    // fot .net 4.0 and up use CustomPlaces instead :http://msdn.microsoft.com/en-us/library/microsoft.win32.filedialog.customplaces.aspx
    public enum Places
    {
        [Description("Desktop")]
        Desktop = 0,

        [Description("Internet Explorer ")]
        InternetExplorer = 1,

        [Description("Program Files")]
        Programs = 2,

        [Description("Control Panel")]
        ControlPanel = 3,

        [Description("Printers")]
        Printers = 4,

        [Description("My Documents")]
        MyDocuments = 5,

        [Description("Favorites")]
        Favorites = 6,

        [Description("Startup folder")]
        StartupFolder = 7,

        [Description("Recent Files")]
        RecentFiles = 8,

        [Description("Send To")]
        SendTo = 9,

        [Description("Recycle Bin")]
        RecycleBin = 0xa,

        [Description("Start menu")]
        StartMenu = 0xb,

        [Description("Logical My Documents")]
        Logical_MyDocuments = 0xc,

        [Description("My Music")]
        MyMusic = 0xd,

        [Description("My Videos")]
        MyVideos = 0xe,

        [Description("<user name>\\Desktop")]
        UserName_Desktop = 0x10,

        [Description("My Computer")]
        MyComputer = 0x11,


        [Description("My Network Places")]
        MyNetworkPlaces = 18,

        [Description("<user name>\nethood")]
        User_Name_Nethood = 0x13,

        [Description("Fonts")]
        Fonts = 0x14,

        [Description("All Users\\Start Menu")]
        All_Users_StartMenu = 0x16,


        [Description("All Users\\Start Menu\\Programs ")]
        All_Users_StartMenu_Programs = 0x17,

        [Description("All Users\\Startup")]
        All_Users_Startup = 0x18,

        [Description("All Users\\Desktop")]
        All_Users_Desktop = 0x19,


        [Description("<user name>\\Application Data ")]
        User_name_ApplicationData = 0x1a,


        [Description("<user name>\\PrintHood ")]
        User_Name_PrintHood = 0x1b,

        [Description("<user name>\\Local Settings\\Applicaiton Data (nonroaming)")]
        Local_ApplicaitonData = 0x1c,

        [Description("Nonlocalized common startup ")]
        NonlocalizedCommonStartup = 0x1e,

        [Description("")]
        CommonFavorites = 0x1f,


        [Description("Internet Cache ")]
        InternetCache = 0x20,


        [Description("Cookies ")]
        Cookies = 0x21,


        [Description("History")]
        History = 0x22,


        [Description("All Users\\Application Data ")]
        All_Users_ApplicationData = 0x23,


        [Description("Windows Directory")]
        WindowsDirectory = 0x24,


        [Description("System Directory")]
        SystemDirectory = 0x25,


        [Description("Program Files ")]
        ProgramFiles = 0x26,


        [Description("My Pictures ")]
        MyPictures = 0x27,


        [Description("USERPROFILE")]
        USERPROFILE = 0x28,

        [Description("system directory on RISC")]
        SYSTEN_RISC = 0x29,

        [Description("Program Files on RISC ")]
        Program_Files_RISC = 0x2a,


        [Description("Program Files\\Common")]
        Common = 0x2b,
        [Description("Program Files\\Common on RISC")]
        Common_RISC = 0x2c,
        [Description("All Users\\Templates ")]
        Templates = 0x2d,
        [Description("All Users\\Documents")]
        All_Users_Documents = 0x2e,
        [Description("All Users\\Start Menu\\Programs\\Administrative Tools")]
        AdministrativeTools = 0x2f,
        [Description("<user name>\\Start Menu\\Programs\\Administrative Tools")]
        USER_AdministrativeTools = 0x30,
        [Description("Network and Dial-up Connections")]
        Network_DialUp_Connections = 0x31,
        [Description("All Users\\My Music")]
        All_Users_MyMusic = 0x35,
        [Description("All Users\\My Pictures")]
        All_Users_MyPictures = 0x36,
        [Description("All Users\\My Video")]
        All_Users_MyVideo = 0x37,
        [Description("Resource Directory")]
        Resource = 0x38,
        [Description("Localized Resource Directory ")]
        Localized_Resource = 0x39,
        [Description("OEM specific apps")]
        OEM_Specific = 0x3a,
        [Description("USERPROFILE\\Local Settings\\Application Data\\Microsoft\\CD Burning")]
        CDBurning = 0x3b

    }
}