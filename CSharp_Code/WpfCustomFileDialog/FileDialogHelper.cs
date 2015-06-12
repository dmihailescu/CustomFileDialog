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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

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

    #region Delegates
    public delegate void PathChangedEventHandler(IFileDlgExt sender, string filePath);
    public delegate void FilterChangedEventHandler(IFileDlgExt sender, int index);
    #endregion



    public enum AddonWindowLocation
    {
        BottomRight = 0,
        Right = 1,
        Bottom = 2,

    }

    public abstract partial class FileDialogExt<T> : Microsoft.Win32.CommonDialog, IFileDlgExt where T : ContentControl, IWindowExt, new()
    {
        #region IWin32Window Members

        public IntPtr Handle
        {
            get { return this._hwndFileDialog; }
        }

        #endregion

        #region IFileDlgExt Members

        #region Events


        event PathChangedEventHandler IFileDlgExt.EventFileNameChanged
        {
            add
            {
                if (_eventFileNameChanged == null)
                    _eventFileNameChanged = value;
                else
                    lock (_eventFileNameChanged) { _eventFileNameChanged += value; }
            }
            remove
            {
                if (_eventFileNameChanged != null && (_eventFileNameChanged.GetInvocationList().Length > 0))
                    lock (_eventFileNameChanged) { _eventFileNameChanged -= value; }
            }
        }
        private PathChangedEventHandler _eventFileNameChanged;

        event PathChangedEventHandler IFileDlgExt.EventFolderNameChanged
        {
            add
            {
                if (_eventFolderNameChanged == null)
                    _eventFolderNameChanged = value;
                else
                    lock (_eventFolderNameChanged) { _eventFolderNameChanged += value; }
            }
            remove
            {
                if (_eventFolderNameChanged != null && (_eventFolderNameChanged.GetInvocationList().Length > 0))
                    lock (_eventFolderNameChanged) { _eventFolderNameChanged -= value; }
            }
        }
        private PathChangedEventHandler _eventFolderNameChanged;


        event FilterChangedEventHandler IFileDlgExt.EventFilterChanged
        {
            add
            {
                if (_eventFilterChanged == null)
                    _eventFilterChanged = value;
                else
                    lock (_eventFilterChanged) { _eventFilterChanged += value; }
            }
            remove
            {
                if (_eventFilterChanged != null && (_eventFilterChanged.GetInvocationList().Length > 0))
                    lock (_eventFilterChanged) { _eventFilterChanged -= value; }
            }
        }
        private FilterChangedEventHandler _eventFilterChanged;
        #endregion
        AddonWindowLocation _location;
        public AddonWindowLocation FileDlgStartLocation
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }

        bool _EnableOkBtn = true;

        public bool FileDlgEnableOkBtn
        {
            get { return _EnableOkBtn; }
            set
            {
                _EnableOkBtn = value;
                if (HandleRef.ToIntPtr(_hOKButton) != IntPtr.Zero)
                    NativeMethods.EnableWindow(_hOKButton, _EnableOkBtn);
            }
        }
        #endregion
        T _childWnd;
        static internal readonly uint MSG_POST_CREATION = NativeMethods.RegisterWindowMessage("Post Creation Message");
        public T ChildWnd
        {
            get { return _childWnd; }
            set { _childWnd = value; }
        }
        HwndSource _source;

        RECT _OriginalRect = new RECT();
        private IntPtr _ComboFolders;
        private WINDOWINFO _ComboFoldersInfo;
        private IntPtr _hGroupButtons;
        private WINDOWINFO _GroupButtonsInfo;
        private IntPtr _hComboFileName;
        private WINDOWINFO _ComboFileNameInfo;
        private IntPtr _hComboExtensions;
        private WINDOWINFO _ComboExtensionsInfo;
        HandleRef _hOKButton;
        WINDOWINFO _OKButtonInfo;
        private IntPtr _hCancelButton;
        private WINDOWINFO _CancelButtonInfo;
        private IntPtr _hHelpButton;
        private WINDOWINFO _HelpButtonInfo;
        private IntPtr _hToolBarFolders;
        private WINDOWINFO _ToolBarFoldersInfo;
        private IntPtr _hLabelFileName;
        private WINDOWINFO _LabelFileNameInfo;
        private IntPtr _hLabelFileType;
        private WINDOWINFO _LabelFileTypeInfo;
        private IntPtr _hChkReadOnly;
        private WINDOWINFO _ChkReadOnlyInfo;

        string _OKCaption = "&Open";
        private void SetChildStyle(bool isChild)
        {
            long styles = (long)NativeMethods.GetWindowLongPtr(new HandleRef(_childWnd, _source.Handle), GWL.GWL_STYLE);
            if (isChild)
            {
                if (IntPtr.Size == 4)
                {
                    styles |= System.Convert.ToInt64(NativeMethods.WindowStyles.WS_CHILD);
                }
                else
                {
                    styles |= (long)NativeMethods.WindowStyles.WS_CHILD;
                }
            }
            else
            {
                NativeMethods.WindowStyles nonChild = (NativeMethods.WindowStyles)0xffffffff ^ NativeMethods.WindowStyles.WS_CHILD;
                if (IntPtr.Size == 4)
                {
                    styles &= System.Convert.ToInt64(nonChild);
                }
                else
                {
                    styles &= (long)nonChild;
                }
            }
            NativeMethods.CriticalSetWindowLong(new HandleRef(this, _source.Handle), (int)GWL.GWL_STYLE, new IntPtr(styles));
        }

        private void SetFixedSize(IntPtr hwnd)
        {
            long styles = (long)NativeMethods.GetWindowLongPtr(new HandleRef(this, hwnd), GWL.GWL_STYLE);

            {
                if (IntPtr.Size == 4)
                {

                    styles ^= System.Convert.ToInt64(NativeMethods.WindowStyles.WS_THICKFRAME);
                }
                else
                {
                    styles |= (long)NativeMethods.WindowStyles.WS_BORDER;
                }
            }
            NativeMethods.CriticalSetWindowLong(new HandleRef(this, hwnd), (int)GWL.GWL_STYLE, new IntPtr(styles));
        }

        public string FileDlgOkCaption
        {
            get { return _OKCaption; }
            set
            {
                _OKCaption = value;
                if (HandleRef.ToIntPtr(_hOKButton) != IntPtr.Zero)
                    NativeMethods.SetWindowText(_hOKButton, _OKCaption);
            }
        }
        bool _bFixedSize = true;
        public bool FixedSize
        {
            set
            {
                _bFixedSize = value;
            }
        }

        NativeMethods.FolderViewMode _DefaultViewMode;
        public NativeMethods.FolderViewMode FileDlgDefaultViewMode
        {
            get { return _DefaultViewMode; }
            set { _DefaultViewMode = value; }
        }

        private void PopulateWindowsHandlers()
        {
            NativeMethods.EnumChildWindows(_hwndFileDialog, new NativeMethods.EnumWindowsCallBack(FileDialogEnumWindowCallBack), 0);
        }
        IntPtr _hListViewPtr;
        private void UpdateListView()
        {
            IntPtr _hListViewPtr = NativeMethods.GetDlgItem(this._hwndFileDialog, (int)NativeMethods.ControlsId.DefaultView);
            if (IntPtr.Zero == _hListViewPtr)
            {
                UpdateListView(_hListViewPtr);
            }
        }
        private void UpdateListView(IntPtr hListViewPtr)
        {
            if (FileDlgDefaultViewMode != NativeMethods.FolderViewMode.Default && hListViewPtr != IntPtr.Zero)
            {
                NativeMethods.SendMessage(new HandleRef(this, hListViewPtr), (int)NativeMethods.Msg.WM_COMMAND, (IntPtr)(int)FileDlgDefaultViewMode, IntPtr.Zero);
            }
        }
        string _oldPath;
        void OnPathChanged(IFileDlgExt sender, string pathName)
        {

            if (string.IsNullOrEmpty(System.IO.Path.GetFileName(pathName)) || _oldPath == pathName)
                return;
            _oldPath = pathName;
            if (System.IO.File.Exists(pathName))
            {
                if (_eventFileNameChanged != null)
                    _eventFileNameChanged(sender, pathName);
            }
            else if (System.IO.Directory.Exists(pathName))
            {
                if (_eventFolderNameChanged != null)
                    _eventFolderNameChanged(sender, pathName);
            }

        }
        int _oldFilterIndex = -1;
        void OnFilterChanged(IFileDlgExt sender, int index)
        {
            if (_oldFilterIndex != index)
            {
                _oldFilterIndex = index;
                if (_eventFilterChanged != null)
                    _eventFilterChanged(sender, index);
            }
        }

        private bool FileDialogEnumWindowCallBack(IntPtr hwnd, int lParam)
        {
            StringBuilder className = new StringBuilder(256);
            NativeMethods.GetClassName(new HandleRef(this, hwnd), className, className.Capacity);
            int controlID = NativeMethods.GetDlgCtrlID(hwnd);
            WINDOWINFO windowInfo;
            NativeMethods.GetWindowInfo(hwnd, out windowInfo);

            // Dialog Window
            if (className.ToString().StartsWith("#32770"))
            {
                _hwndFileDialogEmbedded = hwnd;

                return true;
            }

            switch ((NativeMethods.ControlsId)controlID)
            {
                //not available at startup
                case NativeMethods.ControlsId.DefaultView:
                    UpdateListView(hwnd);
                    break;
                case NativeMethods.ControlsId.ComboFolder:
                    _ComboFolders = hwnd;
                    _ComboFoldersInfo = windowInfo;
                    break;
                case NativeMethods.ControlsId.ComboFileType:
                    _hComboExtensions = hwnd;
                    _ComboExtensionsInfo = windowInfo;
                    break;
                case NativeMethods.ControlsId.ComboFileName:
                    if (className.ToString().ToLower() == "comboboxex32")
                    {
                        _hComboFileName = hwnd;
                        _ComboFileNameInfo = windowInfo;
                    }
                    break;
                case NativeMethods.ControlsId.GroupFolder:
                    _hGroupButtons = hwnd;
                    _GroupButtonsInfo = windowInfo;
                    break;
                case NativeMethods.ControlsId.LeftToolBar:
                    _hToolBarFolders = hwnd;
                    _ToolBarFoldersInfo = windowInfo;
                    break;
                case NativeMethods.ControlsId.ButtonOk:
                    _hOKButton = new HandleRef(this, hwnd);
                    _OKButtonInfo = windowInfo;
                    break;
                case NativeMethods.ControlsId.ButtonCancel:
                    _hCancelButton = hwnd;
                    _CancelButtonInfo = windowInfo;
                    break;
                case NativeMethods.ControlsId.ButtonHelp:
                    _hHelpButton = hwnd;
                    _HelpButtonInfo = windowInfo;
                    break;
                case NativeMethods.ControlsId.CheckBoxReadOnly:
                    _hChkReadOnly = hwnd;
                    _ChkReadOnlyInfo = windowInfo;
                    break;
                case NativeMethods.ControlsId.LabelFileName:
                    _hLabelFileName = hwnd;
                    _LabelFileNameInfo = windowInfo;
                    break;
                case NativeMethods.ControlsId.LabelFileType:
                    _hLabelFileType = hwnd;
                    _LabelFileTypeInfo = windowInfo;
                    break;
            }

            return true;
        }

        public static readonly IntPtr InvalidIntPtr = (IntPtr)(-1);

        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private void SetProperty(object target, string fieldName, object value)
        {
            Type type = target.GetType();
            PropertyInfo mi = type.GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty);
            mi.SetValue(target, value, null);
        }
        protected object InvokeMethod(object instance, string methodName, params object[] args)
        {
            Type type = (instance is Type) ? instance as Type : instance.GetType();
            MethodInfo mi = type.GetMethod(methodName, (instance is Type) ? BindingFlags.NonPublic | BindingFlags.Static : BindingFlags.NonPublic | BindingFlags.Instance);//invoking the method                 
            //null- no parameter for the function [or] we can pass the array of parameters            
            return (args == null || args.Length == 0) ? mi.Invoke(instance, null) : mi.Invoke(instance, args);
        }
        protected object GetProperty(object target, string fieldName)
        {
            Type type = target.GetType();
            PropertyInfo mi = type.GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            if (mi == null)
                return null;
            return mi.GetValue(target, null);
        }

        internal class UnicodeCharBuffer : CharBuffer
        {
            // Fields
            internal char[] buffer;
            internal int offset;

            // Methods
            internal UnicodeCharBuffer(int size)
            {
                this.buffer = new char[size];
            }

            internal override IntPtr AllocCoTaskMem()
            {
                IntPtr destination = Marshal.AllocCoTaskMem(this.buffer.Length * 2);
                Marshal.Copy(this.buffer, 0, destination, this.buffer.Length);
                return destination;
            }

            internal override string GetString()
            {
                int offset = this.offset;
                while ((offset < this.buffer.Length) && (this.buffer[offset] != '\0'))
                {
                    offset++;
                }
                string str = new string(this.buffer, this.offset, offset - this.offset);
                if (offset < this.buffer.Length)
                {
                    offset++;
                }
                this.offset = offset;
                return str;
            }

            internal override void PutCoTaskMem(IntPtr ptr)
            {
                Marshal.Copy(ptr, this.buffer, 0, this.buffer.Length);
                this.offset = 0;
            }

            internal override void PutString(string s)
            {
                int count = Math.Min(s.Length, this.buffer.Length - this.offset);
                s.CopyTo(0, this.buffer, this.offset, count);
                this.offset += count;
                if (this.offset < this.buffer.Length)
                {
                    this.buffer[this.offset++] = '\0';
                }
            }

            // Properties
            internal override int Length
            {
                get
                {
                    return this.buffer.Length;
                }
            }
        }

        internal class AnsiCharBuffer : CharBuffer
        {
            // Fields
            internal byte[] buffer;
            internal int offset;

            // Methods
            internal AnsiCharBuffer(int size)
            {
                this.buffer = new byte[size];
            }

            internal override IntPtr AllocCoTaskMem()
            {
                IntPtr destination = Marshal.AllocCoTaskMem(this.buffer.Length);
                Marshal.Copy(this.buffer, 0, destination, this.buffer.Length);
                return destination;
            }

            internal override string GetString()
            {
                int offset = this.offset;
                while ((offset < this.buffer.Length) && (this.buffer[offset] != 0))
                {
                    offset++;
                }
                string str = Encoding.Default.GetString(this.buffer, this.offset, offset - this.offset);
                if (offset < this.buffer.Length)
                {
                    offset++;
                }
                this.offset = offset;
                return str;
            }

            internal override void PutCoTaskMem(IntPtr ptr)
            {
                Marshal.Copy(ptr, this.buffer, 0, this.buffer.Length);
                this.offset = 0;
            }

            internal override void PutString(string s)
            {
                byte[] bytes = Encoding.Default.GetBytes(s);
                int length = Math.Min(bytes.Length, this.buffer.Length - this.offset);
                Array.Copy(bytes, 0, this.buffer, this.offset, length);
                this.offset += length;
                if (this.offset < this.buffer.Length)
                {
                    this.buffer[this.offset++] = 0;
                }
            }

            // Properties
            internal override int Length
            {
                get
                {
                    return this.buffer.Length;
                }
            }
        }

        internal abstract class CharBuffer
        {
            // Methods
            protected CharBuffer()
            {
            }

            internal abstract IntPtr AllocCoTaskMem();
            [SecurityCritical]
            internal static CharBuffer CreateBuffer(int size)
            {
                if (Marshal.SystemDefaultCharSize == 1)
                {
                    return new AnsiCharBuffer(size);
                }
                return new UnicodeCharBuffer(size);
            }

            internal abstract string GetString();
            internal abstract void PutCoTaskMem(IntPtr ptr);
            internal abstract void PutString(string s);

            // Properties
            internal abstract int Length { get; }
        }

        private void InitControls()
        {
            PopulateWindowsHandlers();
            if (HandleRef.ToIntPtr(_hOKButton) != IntPtr.Zero)
            {
                NativeMethods.EnableWindow(_hOKButton, _EnableOkBtn);
                NativeMethods.SetWindowText(_hOKButton, _OKCaption);
            }
        }

        //http://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type
        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (object obj in LogicalTreeHelper.GetChildren(depObj))
                {
                    DependencyObject child = obj as DependencyObject;
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    if (child != null)
                        foreach (T childOfChild in FindLogicalChildren<T>(child))
                        {
                            yield return childOfChild;
                        }
                }

            }
        }
        //RECT _originalDialogClientRect = new RECT();
        /// <summary>
        /// WndProc for the custom child window
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        IntPtr EmbededWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr hres = IntPtr.Zero;

            const int DLGC_WANTALLKEYS = 0x0004;

            switch ((NativeMethods.Msg)msg)
            {
                case NativeMethods.Msg.WM_ACTIVATE:
                    break;

                case NativeMethods.Msg.WM_SHOWWINDOW:
                    break;
                case NativeMethods.Msg.WM_DESTROY:
                    break;
                case NativeMethods.Msg.WM_SYSKEYDOWN:

                    SetChildStyle(true);

                    break;
                case NativeMethods.Msg.WM_SYSKEYUP:

                    SetChildStyle(false);

                    break;

                case NativeMethods.Msg.WM_KEYDOWN:
                case NativeMethods.Msg.WM_KEYUP:
                    handled = false;
                    break;
                //see http://support.microsoft.com/kb/83302
                case NativeMethods.Msg.WM_GETDLGCODE:
                    if (lParam != IntPtr.Zero)
                    {
                        hres = (IntPtr)DLGC_WANTALLKEYS;
                    }
                    handled = true;

                    break;
                case NativeMethods.Msg.WM_NCDESTROY:
                    break;

                case NativeMethods.Msg.WM_WINDOWPOSCHANGING:

                    break;

                case NativeMethods.Msg.WM_SIZING:
                    break;
                case NativeMethods.Msg.WM_SIZE:

                    break;

            }//switch ends

            return hres;
        }

        IntPtr EmbededCtrlProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr hres = IntPtr.Zero;

            const int DLGC_WANTALLKEYS = 0x0004;

            switch ((NativeMethods.Msg)msg)
            {
                case NativeMethods.Msg.WM_ACTIVATE:
                    break;

                case NativeMethods.Msg.WM_SHOWWINDOW:
                    break;
                case NativeMethods.Msg.WM_DESTROY:
                    break;
                case NativeMethods.Msg.WM_KEYDOWN:
                case NativeMethods.Msg.WM_KEYUP:
                    handled = false;
                    break;
                //see http://support.microsoft.com/kb/83302
                case NativeMethods.Msg.WM_GETDLGCODE:
                    if (lParam != IntPtr.Zero)
                    {
                        hres = (IntPtr)DLGC_WANTALLKEYS;
                        //const int DLGC_WANTCHARS = 0x00000080;
                        //const int DLGC_WANTTAB = 0x00000002;

                        //MSG dlgmsg = (MSG)Marshal.PtrToStructure(lParam, typeof(MSG));
                        //if ((int)dlgmsg.wParam == 9)
                        //{
                        //    hres = (IntPtr)DLGC_WANTTAB;
                        //}
                        //else
                        //{
                        //    hres = (IntPtr)DLGC_WANTCHARS;
                        //}
                    }
                    handled = true;

                    break;
                case NativeMethods.Msg.WM_NCDESTROY:
                    break;

                case NativeMethods.Msg.WM_WINDOWPOSCHANGING:

                    break;

                case NativeMethods.Msg.WM_SIZING:
                    break;
                case NativeMethods.Msg.WM_SIZE:

                    break;

            }//switch ends

            return hres;
        }
        private void ShowChild()
        {
            _childWnd = new T();
            try
            {
                _childWnd.ParentDlg = this;
            }
            catch
            {
                return;
            }

            RECT dialogWindowRect = new RECT();
            RECT dialogClientRect = new RECT();

            Size size = new Size(dialogWindowRect.Width, dialogWindowRect.Height);
            NativeMethods.GetClientRect(new HandleRef(this, _hwndFileDialog), ref dialogClientRect);
            NativeMethods.GetWindowRect(new HandleRef(this, _hwndFileDialog), ref dialogWindowRect);
            int dy = (int)(dialogWindowRect.Height - dialogClientRect.Height);
            int dx = (int)(dialogWindowRect.Width - dialogClientRect.Width);
            size = new Size(dialogWindowRect.Width, dialogWindowRect.Height);

            if (_childWnd is Window)
            {
                Window wnd = _childWnd as Window;
                wnd.WindowStyle = WindowStyle.None;
                wnd.ResizeMode = ResizeMode.NoResize;//will fix the child window!!
                wnd.ShowInTaskbar = false;
                //won't flash on screen
                wnd.WindowStartupLocation = WindowStartupLocation.Manual;
                wnd.Left = -10000;
                wnd.Top = -10000;
                wnd.SourceInitialized += delegate(object sender, EventArgs e)
              {
                  try
                  {
                      _source = System.Windows.PresentationSource.FromVisual(_childWnd as Window) as HwndSource;
                      _source.AddHook(EmbededWndProc);
                      _childWnd.Source = _source;
                  }
                  catch
                  {
                  }

              };

                wnd.Show();

                long styles = (long)NativeMethods.GetWindowLongPtr(new HandleRef(_childWnd, _source.Handle), GWL.GWL_STYLE);
                if (IntPtr.Size == 4)
                {
                    styles |= System.Convert.ToInt64(NativeMethods.WindowStyles.WS_CHILD);
                    styles ^= System.Convert.ToInt64(NativeMethods.WindowStyles.WS_SYSMENU);
                }
                else
                {
                    styles |= (long)NativeMethods.WindowStyles.WS_CHILD;
                    styles ^= (long)NativeMethods.WindowStyles.WS_SYSMENU;
                }
                NativeMethods.CriticalSetWindowLong(new HandleRef(this, _source.Handle), (int)GWL.GWL_STYLE, new IntPtr(styles));

                // Everything is ready, now lets change the parent
                NativeMethods.SetParent(new HandleRef(_childWnd, _source.Handle), new HandleRef(this, _hwndFileDialog));
            }
            else
            {// To do: what if the child is not a Window 
                //see http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/b2cff333-cbd9-4742-beba-ba19a15eeaee
                ContentControl ctrl = _childWnd as ContentControl;
                HwndSourceParameters parameters = new HwndSourceParameters("WPFDlgControl", (int)ctrl.Width, (int)ctrl.Height);
                parameters.WindowStyle = (int)NativeMethods.WindowStyles.WS_VISIBLE | (int)NativeMethods.WindowStyles.WS_CHILD;
                parameters.SetPosition((int)_OriginalRect.Width, (int)_OriginalRect.Height);
                parameters.ParentWindow = _hwndFileDialog;
                parameters.AdjustSizingForNonClientArea = false;
                switch (this.FileDlgStartLocation)
                {
                    case AddonWindowLocation.Right:
                        parameters.PositionX = (int)_OriginalRect.Width - dx/2;
                        parameters.PositionY = 0;
                        if (ctrl.Height < _OriginalRect.Height - dy)
                            ctrl.Height = parameters.Height = (int)_OriginalRect.Height - dy;
                        break;

                    case AddonWindowLocation.Bottom:
                        parameters.PositionX = 0;
                        parameters.PositionY = (int)(_OriginalRect.Height - dy + dx/2);
                        if (ctrl.Width < _OriginalRect.Width - dx)
                            ctrl.Width = parameters.Width = (int)_OriginalRect.Width - dx;
                        break;
                    case AddonWindowLocation.BottomRight:
                        parameters.PositionX = (int)_OriginalRect.Width - dx/2 ;
                        parameters.PositionY = (int)(_OriginalRect.Height - dy + dx/2);
                        break;
                }

                _source = new HwndSource(parameters);
                _source.CompositionTarget.BackgroundColor = System.Windows.Media.Colors.LightGray;
                _source.RootVisual = _childWnd as System.Windows.Media.Visual;
                _source.AddHook(new HwndSourceHook(EmbededCtrlProc));
            }
            switch (this.FileDlgStartLocation)
            {
                case AddonWindowLocation.Right:
                    size.Width = _OriginalRect.Width + _childWnd.Width;
                    size.Height = _OriginalRect.Height;
                    break;

                case AddonWindowLocation.Bottom:
                    size.Width = _OriginalRect.Width;
                    size.Height = _OriginalRect.Height + _childWnd.Height;
                    break;
                case AddonWindowLocation.BottomRight:
                    size.Height = _OriginalRect.Height + _childWnd.Height;
                    size.Width = _OriginalRect.Width + _childWnd.Width;
                    break;
            }
            NativeMethods.SetWindowPos(new HandleRef(this, _hwndFileDialog), new HandleRef(this, (IntPtr)NativeMethods.ZOrderPos.HWND_BOTTOM),
      0, 0, (int)size.Width, (int)size.Height, NativeMethods.SetWindowPosFlags.SWP_NOZORDER);
        }


        private void CustomPostCreation()
        {
            _hListViewPtr = NativeMethods.GetDlgItem(this._hwndFileDialog, (int)NativeMethods.ControlsId.DefaultView);
            UpdateListView(_hListViewPtr);
            if (_bFixedSize)
                SetFixedSize(_hwndFileDialog);
            RECT dialogWndRect = new RECT();
            NativeMethods.GetWindowRect(new HandleRef(this, this._hwndFileDialog), ref dialogWndRect);
            RECT dialogClientRect = new RECT();
            NativeMethods.GetClientRect(new HandleRef(this, this._hwndFileDialog), ref dialogClientRect);
            uint dx = dialogWndRect.Width - dialogClientRect.Width;
            uint dy = dialogWndRect.Height - dialogClientRect.Height;
            if (_childWnd is Window)
            {
                Window wnd = _childWnd as Window;
                //restore the original size
                switch (FileDlgStartLocation)
                {
                    case AddonWindowLocation.Bottom:
                        int left = (Environment.Version.Major >= 4) ? -(int)dx / 2 : dialogWndRect.left;
                        if (wnd.Width >= _OriginalRect.Width - dx)
                        {
                            NativeMethods.MoveWindow(new HandleRef(this, this._hwndFileDialog), left, dialogWndRect.top, (int)(wnd.ActualWidth + dx / 2), (int)(_OriginalRect.Height + wnd.ActualHeight), true);
                        }
                        else
                        {
                            NativeMethods.MoveWindow(new HandleRef(this, this._hwndFileDialog), left, dialogWndRect.top, (int)(_OriginalRect.Width), (int)(_OriginalRect.Height + wnd.ActualHeight), true);
                            wnd.Width = _OriginalRect.Width - dx / 2;
                        }
                        wnd.Left = 0;
                        wnd.Top = _OriginalRect.Height - dy + dx / 2;
                        break;
                    case AddonWindowLocation.Right:
                        int top = (Environment.Version.Major >= 4) ? (int)(dx / 2 - dy) : dialogWndRect.top;
                        if (wnd.Height >= _OriginalRect.Height - dy)
                            NativeMethods.MoveWindow(new HandleRef(this, _hwndFileDialog), (int)(dialogWndRect.left), top, (int)(_OriginalRect.Width + wnd.ActualWidth), (int)(wnd.ActualHeight + dy - dx / 2), true);
                        else
                        {
                            NativeMethods.MoveWindow(new HandleRef(this, _hwndFileDialog), (int)(dialogWndRect.left), top, (int)(_OriginalRect.Width + wnd.ActualWidth), (int)(_OriginalRect.Height - dx / 2), true);
                            wnd.Height = _OriginalRect.Height - dy;
                        }
                            wnd.Top = 0;
                            wnd.Left = _OriginalRect.Width - dx / 2;
                        break;
                    case AddonWindowLocation.BottomRight:
                        NativeMethods.MoveWindow(new HandleRef(this, _hwndFileDialog), dialogWndRect.left, dialogWndRect.top, (int)(_OriginalRect.Width + wnd.Width), (int)(int)(_OriginalRect.Height + wnd.Height), true);
                        wnd.Top = _OriginalRect.Height - dy + dx / 2;
                        wnd.Left = _OriginalRect.Width - dx / 2;
                        break;
                }

            }
            else
            {
                ContentControl ctrl = _childWnd as ContentControl;
                //restore the original size
                const NativeMethods.SetWindowPosFlags flags = NativeMethods.SetWindowPosFlags.SWP_NOZORDER | NativeMethods.SetWindowPosFlags.SWP_NOMOVE;//| SetWindowPosFlags.SWP_NOREPOSITION | SetWindowPosFlags.SWP_ASYNCWINDOWPOS | SetWindowPosFlags.SWP_SHOWWINDOW | SetWindowPosFlags.SWP_DRAWFRAME;
                switch (FileDlgStartLocation)
                {
                    case AddonWindowLocation.Bottom:
                        NativeMethods.SetWindowPos(new HandleRef(this, this._hwndFileDialog), new HandleRef(this,(IntPtr)ZOrderPos.HWND_BOTTOM),
                            dialogWndRect.left, dialogWndRect.top, (int)(ctrl.ActualWidth + dx / 2), (int)(_OriginalRect.Height + ctrl.ActualHeight), flags);
                        //NativeMethods.MoveWindow(new HandleRef(this, this._hwndFileDialog), dialogWndRect.left, dialogWndRect.top, (int)(ctrl.ActualWidth + dx / 2), (int)(_OriginalRect.Height + ctrl.ActualHeight), true);
                        NativeMethods.SetWindowPos(new HandleRef(ctrl, _source.Handle), new HandleRef(_source, (IntPtr)ZOrderPos.HWND_BOTTOM),
                                                   0, (int)(_OriginalRect.Height - dy + dx / 2), (int)(ctrl.Width), (int)(ctrl.Height), flags);
                        //NativeMethods.MoveWindow(new HandleRef(ctrl, _source.Handle), 0, (int)(_OriginalRect.Height - dy + dx / 2), (int)(ctrl.Width), (int)(ctrl.Height), true);
                        break;
                    case AddonWindowLocation.Right:
                        NativeMethods.SetWindowPos(new HandleRef(this, this._hwndFileDialog), new HandleRef(this, (IntPtr)ZOrderPos.HWND_BOTTOM),
                            (int)(dialogWndRect.left), dialogWndRect.top, (int)(_OriginalRect.Width + ctrl.ActualWidth - dx / 2), (int)(ctrl.ActualHeight + dy - dx / 2), flags);
                        //NativeMethods.MoveWindow(new HandleRef(this, _hwndFileDialog), (int)(dialogWndRect.left), dialogWndRect.top, (int)(_OriginalRect.Width + ctrl.ActualWidth - dx / 2), (int)(ctrl.ActualHeight + dy - dx / 2), true);
                        NativeMethods.SetWindowPos(new HandleRef(ctrl, _source.Handle), new HandleRef(_source, (IntPtr)ZOrderPos.HWND_BOTTOM),
                                                   (int)(_OriginalRect.Width - dx), (int)(0), (int)(ctrl.Width), (int)(ctrl.Height), flags);
                        //NativeMethods.MoveWindow(new HandleRef(ctrl, _source.Handle), (int)(_OriginalRect.Width - dx), (int)(0), (int)(ctrl.Width), (int)(ctrl.Height), true);
                        break;
                    case AddonWindowLocation.BottomRight:
                        //NativeMethods.MoveWindow(new HandleRef(this, _hwndFileDialog), dialogWndRect.left, dialogWndRect.top, (int)(_OriginalRect.Width + ctrl.Width), (int)(_OriginalRect.Height + ctrl.Height), true);
                        NativeMethods.SetWindowPos(new HandleRef(this, this._hwndFileDialog), new HandleRef(this, (IntPtr)ZOrderPos.HWND_BOTTOM),
                             dialogWndRect.left, dialogWndRect.top, (int)(_OriginalRect.Width + ctrl.Width), (int)(_OriginalRect.Height + ctrl.Height), flags);
                        break;
                }

            }
            CenterDialogToScreen();
            NativeMethods.InvalidateRect(new HandleRef(this, _source.Handle), IntPtr.Zero, true);

        }
    }
}
