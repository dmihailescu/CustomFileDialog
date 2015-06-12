// Copyright © Decebal Mihailescu 2015
// Some code was obtained by reverse engineering the PresentationFramework.dll using Reflector

// All rights reserved.
// This code is released under The Code Project Open License (CPOL) 1.02
// The full licensing terms are available at http://www.codeproject.com/info/cpol10.aspx
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
// REMAINS UNCHANGED.
namespace WpfCustomFileDialog
{
    using MS.Win32;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows;
    using System.Reflection;
    using System.Windows.Interop; 
    using System.Windows.Controls;
    public abstract partial class FileDialogExt<T> : Microsoft.Win32.CommonDialog, IFileDlgExt where T : ContentControl, IWindowExt, new()
    {

        [SecurityCritical]
        private CharBuffer _charBuffer;
        private string _defaultExtension;
        private int _dialogOptions;
        [SecurityCritical]
        private string[] _fileNames;
        private int _fileOkNotificationCount;
        private string _filter;
        private int _filterIndex;
        [SecurityCritical]
        private IntPtr _hwndFileDialog;
        private IntPtr _hwndFileDialogEmbedded;
        private bool _ignoreSecondFileOkNotification;
        private string _initialDirectory;
        private string _title;
        private const int FILEBUFSIZE = 0x2000;
        private const int OPTION_ADDEXTENSION = -2147483648;
        public event CancelEventHandler FileOk;

        [SecurityCritical, SecuritySafeCritical]
        protected FileDialogExt()
        {
            this.Initialize();
        }

        [SecurityCritical]
        private bool DoFileOk(IntPtr lpOFN)
        {
            OPENFILENAME_I openfilename_i = (OPENFILENAME_I)Marshal.PtrToStructure(lpOFN, typeof(OPENFILENAME_I));
            int num = this._dialogOptions;
            int num2 = this._filterIndex;
            string[] strArray = this._fileNames;
            bool flag = false;
            try
            {
                this._dialogOptions = (this._dialogOptions & -2) | (openfilename_i.Flags & 1);
                this._filterIndex = openfilename_i.nFilterIndex;
                this._charBuffer.PutCoTaskMem(openfilename_i.lpstrFile);
                if (!this.GetOption(0x200))
                {
                    this._fileNames = new string[] { this._charBuffer.GetString() };
                }
                else
                {
                    this._fileNames = GetMultiselectFiles(this._charBuffer);
                }
                if (this.ProcessFileNames())
                {
                    CancelEventArgs e = new CancelEventArgs();
                    this.OnFileOk(e);
                    flag = !e.Cancel;
                }
            }
            finally
            {
                if (!flag)
                {
                    this._dialogOptions = num;
                    this._filterIndex = num2;
                    this._fileNames = strArray;
                }
            }
            return flag;
        }

        private string[] GetFilterExtensions()
        {
            string str = this._filter;
            List<string> list = new List<string>();
            if (this._defaultExtension != null)
            {
                list.Add(this._defaultExtension);
            }
            if (str != null)
            {
                string[] strArray = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                int index = (this._filterIndex * 2) - 1;
                if (index >= strArray.Length)
                {
                    throw new InvalidOperationException("FileDialogInvalidFilterIndex");
                }
                if (this._filterIndex > 0)
                {
                    foreach (string str2 in strArray[index].Split(new char[] { ';' }))
                    {
                        int num2 = str2.LastIndexOf('.');
                        if (num2 >= 0)
                        {
                            list.Add(str2.Substring(num2 + 1, str2.Length - (num2 + 1)));
                        }
                    }
                }
            }
            return list.ToArray();
        }

        private static string[] GetMultiselectFiles(CharBuffer charBuffer)
        {
            string str = charBuffer.GetString();
            string str2 = charBuffer.GetString();
            if (str2.Length == 0)
            {
                return new string[] { str };
            }
            if (!str.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                str = str + Path.DirectorySeparatorChar;
            }
            List<string> list = new List<string>();
            do
            {
                bool flag = (str2[0] == Path.DirectorySeparatorChar) && (str2[1] == Path.DirectorySeparatorChar);
                bool flag2 = ((str2.Length > 3) && (str2[1] == Path.VolumeSeparatorChar)) && (str2[2] == Path.DirectorySeparatorChar);
                if (!flag && !flag2)
                {
                    str2 = str + str2;
                }
                list.Add(str2);
                str2 = charBuffer.GetString();
            }
            while (!string.IsNullOrEmpty(str2));
            return list.ToArray();
        }

        internal bool GetOption(int option)
        {
            return ((this._dialogOptions & option) != 0);
        }


        //HookProc has been changed
        [SecurityCritical]
        protected override IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr hres = IntPtr.Zero;

            switch ((NativeMethods.Msg)msg)
            {

                case NativeMethods.Msg.WM_NOTIFY:
                    hres = ProcOnNotify(hwnd, lParam);
                    break;

                case NativeMethods.Msg.WM_SHOWWINDOW:
                    InitControls();
                    ShowChild();
                    break;
                case NativeMethods.Msg.WM_DESTROY:
                    break;
                case NativeMethods.Msg.WM_NCDESTROY:
                    _source.Dispose();
                    break;

                case NativeMethods.Msg.WM_INITDIALOG:
                    _hwndFileDialog = NativeMethods.GetParent(new HandleRef(this, hwnd));
                    _hwndFileDialogEmbedded = hwnd;
                    NativeMethods.GetWindowRect(new HandleRef(this, _hwndFileDialog), ref _OriginalRect);
                    break;
                case NativeMethods.Msg.WM_WINDOWPOSCHANGING:
                    // PositionChanging(lParam, ref dialogWndRect);
                    break;
                case NativeMethods.Msg.WM_SIZE:
                    break;
                case NativeMethods.Msg.WM_NCCALCSIZE:
                    //CalcSize(wParam, lParam);
                    break;
                case NativeMethods.Msg.WM_WINDOWPOSCHANGED:
                    break;

                case NativeMethods.Msg.WM_SETREDRAW:
                    break;
                case NativeMethods.Msg.WM_CHILDACTIVATE:
                    break;
                case NativeMethods.Msg.WM_SETFONT:
                    break;

                default:
                    if (msg == (int)MSG_POST_CREATION)
                    {
                        CustomPostCreation();
                    }
                    break;
            }//switch ends

            return hres;
        }

        private static void CalcSize(IntPtr wParam, IntPtr lParam)
        {
            if (wParam == IntPtr.Zero)
            {
                RECT rc = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                Marshal.StructureToPtr(rc, lParam, true);
            }
            else
            {

                NativeMethods.NCCALCSIZE_PARAMS csp;
                csp = (NativeMethods.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(NativeMethods.NCCALCSIZE_PARAMS));
                WINDOWPOS pos = (WINDOWPOS)Marshal.PtrToStructure(csp.lppos, typeof(WINDOWPOS));
                Marshal.StructureToPtr(pos, csp.lppos, true);
                Marshal.StructureToPtr(csp, lParam, true);

            }
        }



        private void PositionChanging(IntPtr lParam, ref RECT dialogClientRect)
        {
            Window wnd = _childWnd as Window;
            //Resize FileDialog to make fit our extra window
            //called only when initializig
            WINDOWPOS pos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
            if (pos.flags != 0 && ((pos.flags & (int)SWP_Flags.SWP_NOSIZE) != (int)SWP_Flags.SWP_NOSIZE))
            {
                NativeMethods.GetClientRect(new HandleRef(this, _hwndFileDialog), ref dialogClientRect);
                switch (this.FileDlgStartLocation)
                {
                    case AddonWindowLocation.Right:
                        pos.cx += (int)dialogClientRect.Width;
                        Marshal.StructureToPtr(pos, lParam, true);
                        wnd.Height = dialogClientRect.Height;
                        break;

                    case AddonWindowLocation.Bottom:
                        pos.cy += (int)dialogClientRect.Height;
                        Marshal.StructureToPtr(pos, lParam, true);
                        wnd.Width = dialogClientRect.Width;
                        break;

                    case AddonWindowLocation.BottomRight:
                        pos.cy += (int)dialogClientRect.Height;
                        pos.cx += (int)dialogClientRect.Width;
                        Marshal.StructureToPtr(pos, lParam, true);
                        break;
                }
            }
        }

        private IntPtr ProcOnNotify(IntPtr hwnd, IntPtr lParam)
        {
            IntPtr hres = IntPtr.Zero;
            OFNOTIFY structure = (OFNOTIFY)Marshal.PtrToStructure(lParam, typeof(OFNOTIFY));
            switch ((NativeMethods.DialogChangeStatus)structure.hdr_code)
            {
                case NativeMethods.DialogChangeStatus.CDN_FILEOK://- 606:
                    if (this._ignoreSecondFileOkNotification)
                    {
                        if (this._fileOkNotificationCount != 0)
                        {
                            this._ignoreSecondFileOkNotification = false;
                            NativeMethods.CriticalSetWindowLong(new HandleRef(this, hwnd), 0, InvalidIntPtr);
                            hres = InvalidIntPtr;
                            break;
                        }
                        this._fileOkNotificationCount = 1;
                    }

                    if (!this.DoFileOk(structure.lpOFN))
                    {
                        NativeMethods.CriticalSetWindowLong(new HandleRef(this, hwnd), 0, InvalidIntPtr);
                        hres = InvalidIntPtr;
                    }
                    break;
                case NativeMethods.DialogChangeStatus.CDN_TYPECHANGE:
                    {
                        OPENFILENAME_I ofn = (OPENFILENAME_I)Marshal.PtrToStructure(structure.lpOFN, typeof(OPENFILENAME_I));
                        int i = ofn.nFilterIndex;

                        OnFilterChanged(this, i);

                    }
                    break;
                case NativeMethods.DialogChangeStatus.CDN_HELP:// - 605:
                    break;
                case NativeMethods.DialogChangeStatus.CDN_FOLDERCHANGE://- 603:
                    {
                        StringBuilder folderPath = new StringBuilder(256);
                        NativeMethods.SendMessage(new HandleRef(this, structure.hdr_hwndFrom), (int)DialogChangeProperties.CDM_GETFOLDERPATH, (IntPtr)256, folderPath);
                        OnPathChanged(this, folderPath.ToString());
                        folderPath.Length = 0;
                    }
                    break;

                case NativeMethods.DialogChangeStatus.CDN_SHAREVIOLATION://- 604:
                    this._ignoreSecondFileOkNotification = true;
                    this._fileOkNotificationCount = 0;
                    break;

                case NativeMethods.DialogChangeStatus.CDN_SELCHANGE://- 602:
                    {
                        OPENFILENAME_I openfilename_i = (OPENFILENAME_I)Marshal.PtrToStructure(structure.lpOFN, typeof(OPENFILENAME_I));
                        int num = (int)NativeMethods.UnsafeSendMessage(this._hwndFileDialog, 0x464, IntPtr.Zero, IntPtr.Zero);
                        if (num > openfilename_i.nMaxFile)
                        {
                            int size = num + 0x800;
                            CharBuffer buffer = CharBuffer.CreateBuffer(size);
                            IntPtr ptr2 = buffer.AllocCoTaskMem();
                            Marshal.FreeCoTaskMem(openfilename_i.lpstrFile);
                            openfilename_i.lpstrFile = ptr2;
                            openfilename_i.nMaxFile = size;
                            this._charBuffer = buffer;
                            Marshal.StructureToPtr(openfilename_i, structure.lpOFN, true);
                            Marshal.StructureToPtr(structure, lParam, true);
                        }
                        StringBuilder filePath = new StringBuilder(256);
                        NativeMethods.SendMessage(new HandleRef(this, structure.hdr_hwndFrom), (uint)DialogChangeProperties.CDM_GETFILEPATH, (IntPtr)256, filePath);
                        OnPathChanged(this, filePath.ToString());
                        filePath.Length = 0;
                        break;
                    }

                case NativeMethods.DialogChangeStatus.CDN_INITDONE://- 601:
                    {
                        NativeMethods.PostMessage(new HandleRef(this, this._hwndFileDialogEmbedded), MSG_POST_CREATION, IntPtr.Zero, IntPtr.Zero);
                    }
                    break;
            }
            return hres;
        }

        private void CenterDialogToScreen()
        {
            object ownerWindowHandle = GetProperty(this, "OwnerWindowHandle");
            if (ownerWindowHandle == null)//if (Environment.Version.Major >= 4)
                InvokeMethod(this, "MoveToScreenCenter", new HandleRef(this, this._hwndFileDialog));
            else
            {
                InvokeMethod(typeof(Microsoft.Win32.CommonDialog), "MoveToScreenCenter", new HandleRef(this, this._hwndFileDialog), new HandleRef(this, (IntPtr)ownerWindowHandle));
            }
        }



        [SecurityCritical]
        private void Initialize()
        {
            this._dialogOptions = 0;
            this.SetOption(4, true);
            this.SetOption(0x800, true);
            this.SetOption(-2147483648, true);
            this._title = null;
            this._initialDirectory = null;
            this._defaultExtension = null;
            this._fileNames = null;
            this._filter = null;
            this._filterIndex = 1;
            this._ignoreSecondFileOkNotification = false;
            this._fileOkNotificationCount = 0;
        }

        private static string MakeFilterString(string s, bool dereferenceLinks)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (!dereferenceLinks || (Environment.OSVersion.Version.Major < 5))
                {
                    return null;
                }
                s = " |*.*";
            }
            StringBuilder builder = new StringBuilder(s);
            builder.Replace('|', '\0');
            builder.Append('\0');
            builder.Append('\0');
            return builder.ToString();
        }


        [SecurityCritical]
        internal bool MessageBoxWithFocusRestore(string message, MessageBoxButton buttons, MessageBoxImage image)
        {
            bool flag = false;
            IntPtr focus = NativeMethods.GetFocus();
            try
            {
                flag = MessageBox.Show(message, this.DialogCaption, buttons, image, MessageBoxResult.OK, MessageBoxOptions.None) == MessageBoxResult.Yes;
            }
            finally
            {
                NativeMethods.SetFocus(new HandleRef(this, focus));
            }
            return flag;
        }

        protected void OnFileOk(CancelEventArgs e)
        {
            if (this.FileOk != null)
            {
                this.FileOk(this, e);
            }
        }

        [SecurityCritical, SecuritySafeCritical]
        private bool ProcessFileNames()
        {
            if (!this.GetOption(0x100))
            {
                string[] filterExtensions = this.GetFilterExtensions();
                for (int i = 0; i < this._fileNames.Length; i++)
                {
                    string path = this._fileNames[i];
                    if (this.AddExtension && !Path.HasExtension(path))
                    {
                        for (int j = 0; j < filterExtensions.Length; j++)
                        {
                            string extension = Path.GetExtension(path);
                            StringBuilder builder = new StringBuilder(path.Substring(0, path.Length - extension.Length));
                            if (filterExtensions[j].IndexOfAny(new char[] { '*', '?' }) == -1)
                            {
                                builder.Append(".");
                                builder.Append(filterExtensions[j]);
                            }
                            if (!this.GetOption(0x1000) || File.Exists(builder.ToString()))
                            {
                                path = builder.ToString();
                                break;
                            }
                        }
                        this._fileNames[i] = path;
                    }
                    if (!this.PromptUserIfAppropriate(path))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [SecurityCritical]
        private void PromptFileNotFound(string fileName)
        {
            this.MessageBoxWithFocusRestore("FileDialogFileNotFound" + fileName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        [SecurityCritical]
        internal virtual bool PromptUserIfAppropriate(string fileName)
        {
            bool flag = true;
            if (this.GetOption(0x1000))
            {
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery | FileIOPermissionAccess.Read, fileName).Assert();
                try
                {
                    flag = File.Exists(Path.GetFullPath(fileName));
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
                if (!flag)
                {
                    this.PromptFileNotFound(fileName);
                }
            }
            return flag;
        }

        [SecurityCritical]
        public override void Reset()
        {
            this.Initialize();
        }

        [SecurityCritical]
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            bool flag;
            OPENFILENAME_I.WndProc proc = new OPENFILENAME_I.WndProc(this.HookProc);
            OPENFILENAME_I ofn = new OPENFILENAME_I();
            try
            {
                this._charBuffer = CharBuffer.CreateBuffer(0x2000);
                if (this._fileNames != null)
                {
                    this._charBuffer.PutString(this._fileNames[0]);
                }
                ofn.lStructSize = Marshal.SizeOf(typeof(OPENFILENAME_I));
                ofn.hwndOwner = hwndOwner;
                ofn.hInstance = IntPtr.Zero;
                ofn.lpstrFilter = MakeFilterString(this._filter, this.DereferenceLinks);
                ofn.nFilterIndex = this._filterIndex;
                ofn.lpstrFile = this._charBuffer.AllocCoTaskMem();
                ofn.nMaxFile = this._charBuffer.Length;
                ofn.lpstrInitialDir = this._initialDirectory;
                ofn.lpstrTitle = this._title;
                ofn.Flags = this.Options | 0x880020;
                ofn.lpfnHook = proc;
                ofn.FlagsEx = 0x1000000;
                if ((this._defaultExtension != null) && this.AddExtension)
                {
                    ofn.lpstrDefExt = this._defaultExtension;
                }
                if (_fakeKey != null)
                    ResetPlaces();
                if(m_places != null)
                    SetupFakeRegistryTree();
                flag = this.RunFileDialog(ofn);
 
            }
            finally
            {
                this._charBuffer = null;
                if (ofn.lpstrFile != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(ofn.lpstrFile);
                }
                try
                {
                    if (m_places != null)
                        ResetPlaces();
                }
                catch
                {
                }
            }
            return flag;
        }

        internal abstract bool RunFileDialog(OPENFILENAME_I ofn);
        [SecurityCritical]
        internal void SetOption(int option, bool value)
        {
            if (value)
            {
                this._dialogOptions |= option;
            }
            else
            {
                this._dialogOptions &= ~option;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(base.ToString() + ": Title: " + this.Title + ", FileName: ");
            builder.Append(this.FileName);
            return builder.ToString();
        }

        public bool AddExtension
        {
            get
            {
                return this.GetOption(-2147483648);
            }
            [SecurityCritical]
            set
            {
                this.SetOption(-2147483648, value);
            }
        }

        public virtual bool CheckFileExists
        {
            get
            {
                return this.GetOption(0x1000);
            }
            [SecurityCritical]
            set
            {
                this.SetOption(0x1000, value);
            }
        }

        public bool CheckPathExists
        {
            get
            {
                return this.GetOption(0x800);
            }
            [SecurityCritical]
            set
            {
                this.SetOption(0x800, value);
            }
        }

        private string CriticalFileName
        {
            [SecurityCritical]
            get
            {
                if ((this._fileNames != null) && (this._fileNames[0].Length > 0))
                {
                    return this._fileNames[0];
                }
                return string.Empty;
            }
        }

        public string DefaultExt
        {
            get
            {
                if (this._defaultExtension != null)
                {
                    return this._defaultExtension;
                }
                return string.Empty;
            }
            set
            {
                if (value != null)
                {
                    if (value.StartsWith(".", StringComparison.Ordinal))
                    {
                        value = value.Substring(1);
                    }
                    else if (value.Length == 0)
                    {
                        value = null;
                    }
                }
                this._defaultExtension = value;
            }
        }

        public bool DereferenceLinks
        {
            get
            {
                return !this.GetOption(0x100000);
            }
            [SecurityCritical]
            set
            {
                this.SetOption(0x100000, !value);
            }
        }




        private string DialogCaption
        {
            [SecurityCritical]
            get
            {
                if (!NativeMethods.IsWindow(new HandleRef(this, this._hwndFileDialog)))
                {
                    return string.Empty;
                }
                StringBuilder lpString = new StringBuilder(NativeMethods.GetWindowTextLength(new HandleRef(this, this._hwndFileDialog)) + 1);
                NativeMethods.GetWindowText(new HandleRef(this, this._hwndFileDialog), lpString, lpString.Capacity);
                return lpString.ToString();
            }
        }

        public string FileName
        {
            [SecurityCritical]
            get
            {
                return this.CriticalFileName;
            }
            [SecurityCritical]
            set
            {
                if (value == null)
                {
                    this._fileNames = null;
                }
                else
                {
                    this._fileNames = new string[] { value };
                }
            }
        }

        public string[] FileNames
        {
            [SecurityCritical]
            get
            {
                return this.FileNamesInternal;
            }
        }

        internal string[] FileNamesInternal
        {
            [SecurityCritical]
            get
            {
                if (this._fileNames == null)
                {
                    return new string[0];
                }
                return (string[])this._fileNames.Clone();
            }
        }

        public string Filter
        {
            get
            {
                if (this._filter != null)
                {
                    return this._filter;
                }
                return string.Empty;
            }
            set
            {
                if (string.CompareOrdinal(value, this._filter) != 0)
                {
                    string str = value;
                    if (!string.IsNullOrEmpty(str))
                    {
                        if ((str.Split(new char[] { '|' }).Length % 2) != 0)
                        {
                            throw new ArgumentException("FileDialogInvalidFilter");
                        }
                    }
                    else
                    {
                        str = null;
                    }
                    this._filter = str;
                }
            }
        }

        public int FilterIndex
        {
            get
            {
                return this._filterIndex;
            }
            set
            {
                this._filterIndex = value;
            }
        }

        public string InitialDirectory
        {
            get
            {
                if (this._initialDirectory != null)
                {
                    return this._initialDirectory;
                }
                return string.Empty;
            }
            [SecurityCritical]
            set
            {
                this._initialDirectory = value;
            }
        }

        protected int Options
        {
            get
            {
                return (this._dialogOptions & 0x100b0d);
            }
        }

        public bool RestoreDirectory
        {
            get
            {
                return this.GetOption(8);
            }
            [SecurityCritical]
            set
            {

                this.SetOption(8, value);
            }
        }

        public string SafeFileName
        {
            [SecurityCritical]
            get
            {
                string fileName = Path.GetFileName(this.CriticalFileName);
                if (fileName == null)
                {
                    fileName = string.Empty;
                }
                return fileName;
            }
        }

        public string[] SafeFileNames
        {
            [SecurityCritical]
            get
            {
                string[] fileNamesInternal = this.FileNamesInternal;
                string[] strArray2 = new string[fileNamesInternal.Length];
                for (int i = 0; i < fileNamesInternal.Length; i++)
                {
                    strArray2[i] = Path.GetFileName(fileNamesInternal[i]);
                    if (strArray2[i] == null)
                    {
                        strArray2[i] = string.Empty;
                    }
                }
                return strArray2;
            }
        }

        public string Title
        {
            get
            {
                if (this._title != null)
                {
                    return this._title;
                }
                return string.Empty;
            }
            [SecurityCritical]
            set
            {

                this._title = value;
            }
        }

        public bool ValidateNames
        {
            get
            {
                return !this.GetOption(0x100);
            }
            [SecurityCritical]
            set
            {
                this.SetOption(0x100, !value);
            }
        }

    }
}

