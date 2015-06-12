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

namespace WpfCustomFileDialog
{


    #region POINT

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;

        #region Constructors
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public POINT(Point point)
        {
            x = (int)point.X;
            y = (int)point.Y;
        }
        #endregion
    }
    #endregion

    #region MinMax
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };
    #endregion

    [StructLayout(LayoutKind.Sequential)]
    internal class OFNOTIFY
    {
        internal IntPtr hdr_hwndFrom;
        internal IntPtr hdr_idFrom;
        internal int hdr_code;
        internal IntPtr lpOFN;
        internal IntPtr pszFile;
    }
    #region DialogChangeProperties
    internal enum DialogChangeProperties
    {
        CDM_FIRST = (NativeMethods.Msg.WM_USER + 100),
        CDM_GETSPEC = (CDM_FIRST + 0x0000),
        CDM_GETFILEPATH = (CDM_FIRST + 0x0001),
        CDM_GETFOLDERPATH = (CDM_FIRST + 0x0002),
        CDM_GETFOLDERIDLIST = (CDM_FIRST + 0x0003),
        CDM_SETCONTROLTEXT = (CDM_FIRST + 0x0004),
        CDM_HIDECONTROL = (CDM_FIRST + 0x0005),
        CDM_SETDEFEXT = (CDM_FIRST + 0x0006)
    }
    #endregion
    #region ImeNotify

    internal enum ImeNotify
    {
        IMN_CLOSESTATUSWINDOW = 0x0001,
        IMN_OPENSTATUSWINDOW = 0x0002,
        IMN_CHANGECANDIDATE = 0x0003,
        IMN_CLOSECANDIDATE = 0x0004,
        IMN_OPENCANDIDATE = 0x0005,
        IMN_SETCONVERSIONMODE = 0x0006,
        IMN_SETSENTENCEMODE = 0x0007,
        IMN_SETOPENSTATUS = 0x0008,
        IMN_SETCANDIDATEPOS = 0x0009,
        IMN_SETCOMPOSITIONFONT = 0x000A,
        IMN_SETCOMPOSITIONWINDOW = 0x000B,
        IMN_SETSTATUSWINDOWPOS = 0x000C,
        IMN_GUIDELINE = 0x000D,
        IMN_PRIVATE = 0x000E
    }
    #endregion
#region 
    internal enum GWL
    {
        GWL_WNDPROC = (-4),
        GWL_HINSTANCE = (-6),
        GWL_HWNDPARENT = (-8),
        GWL_STYLE = (-16),
        GWL_EXSTYLE = (-20),
        GWL_USERDATA = (-21),
        GWL_ID = (-12)
    }
    #endregion
    #region FolderViewMode

    public enum FolderViewMode
    {
        Default = 0x7028,
        Icon = Default + 1,
        SmallIcon = Default + 2,
        List = Default + 3,
        Details = Default + 4,
        Thumbnails = Default + 5,
        Title = Default + 6,
        Thumbstrip = Default + 7,
    }
    #endregion
    #region RECT

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        #region Properties

        public POINT Location
        {
            get { return new POINT((int)left, (int)top); }
            set
            {
                right -= (left - value.x);
                bottom -= (bottom - value.y);
                left = value.x;
                top = value.y;
            }
        }

        internal uint Width
        {
            get { return (uint)Math.Abs(right - left); }
            set { right = left + (int)value; }
        }

        internal uint Height
        {
            get { return (uint)Math.Abs(bottom - top); }
            set { bottom = top + (int)value; }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return left + ":" + top + ":" + right + ":" + bottom;
        }
        #endregion
        public bool IsEmpty
        {
            get
            {
                if (this.left < this.right)
                {
                    return (this.top >= this.bottom);
                }
                return true;
            }
        }
    }
    #endregion
    #region SWP_Flags
    [Flags]
    public enum SWP_Flags
    {
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_NOZORDER = 0x0004,
        SWP_NOACTIVATE = 0x0010,
        SWP_FRAMECHANGED = 0x0020, /* The frame changed: send WM_NCCALCSIZE */
        SWP_SHOWWINDOW = 0x0040,
        SWP_HIDEWINDOW = 0x0080,
        SWP_NOOWNERZORDER = 0x0200, /* Don't do owner Z ordering */

        SWP_DRAWFRAME = SWP_FRAMECHANGED,
        SWP_NOREPOSITION = SWP_NOOWNERZORDER
    }
    #endregion

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class OPENFILENAME_I
    {
        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        public int lStructSize = Marshal.SizeOf(typeof(OPENFILENAME_I));
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public IntPtr lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public IntPtr lpstrFile;
        public int nMaxFile = 260;
        public IntPtr lpstrFileTitle;
        public int nMaxFileTitle = 260;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public WndProc lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int FlagsEx;
    }
    #region WINDOWPOS

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        public IntPtr hwnd;
        public IntPtr hwndAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public uint flags;

        #region Overrides
        public override string ToString()
        {
            return x + ":" + y + ":" + cx + ":" + cy + ":" + ((SWP_Flags)flags).ToString();
        }
        #endregion
    }
    #endregion

    #region WINDOWPLACEMENT
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public int ptMinPosition_x;
        public int ptMinPosition_y;
        public int ptMaxPosition_x;
        public int ptMaxPosition_y;
        public int rcNormalPosition_left;
        public int rcNormalPosition_top;
        public int rcNormalPosition_right;
        public int rcNormalPosition_bottom;
    }


 

    #endregion
    #region ZOrderPos
    internal enum ZOrderPos
    {
        HWND_TOP = 0,
        HWND_BOTTOM = 1,
        HWND_TOPMOST = -1,
        HWND_NOTOPMOST = -2
    }
    #endregion
	#region SetWindowPosFlags
    
    [Flags]
    internal enum SetWindowPosFlags
	{
		SWP_NOSIZE          = 0x0001,
		SWP_NOMOVE          = 0x0002,
		SWP_NOZORDER        = 0x0004,
		SWP_NOREDRAW        = 0x0008,
		SWP_NOACTIVATE      = 0x0010,
		SWP_FRAMECHANGED    = 0x0020,
		SWP_SHOWWINDOW      = 0x0040,
		SWP_HIDEWINDOW      = 0x0080,
		SWP_NOCOPYBITS      = 0x0100,
		SWP_NOOWNERZORDER   = 0x0200, 
		SWP_NOSENDCHANGING  = 0x0400,
		SWP_DRAWFRAME       = 0x0020,
		SWP_NOREPOSITION    = 0x0200,
		SWP_DEFERERASE      = 0x2000,
		SWP_ASYNCWINDOWPOS  = 0x4000
	}
	#endregion

    #region WINDOWINFO

    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWINFO
    {
        public UInt32 cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public UInt32 dwStyle;
        public UInt32 dwExStyle;
        public UInt32 dwWindowStatus;
        public UInt32 cxWindowBorders;
        public UInt32 cyWindowBorders;
        public UInt16 atomWindowType;
        public UInt16 wCreatorVersion;
    }
    #endregion
    public static class NativeMethods
    {
        #region Delegates
        public delegate bool EnumWindowsCallBack(IntPtr hWnd, int lParam);
        #endregion
        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRect(
            ref RECT lpRect,
            Int32 dwStyle,
            bool bMenu
        );
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetOpenFileName([In, Out] OPENFILENAME_I ofn);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool GetSaveFileName([In, Out] OPENFILENAME_I ofn);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool MoveWindow(HandleRef hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClassName(HandleRef hwnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowInfo(IntPtr hwnd, out WINDOWINFO pwi);
        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, NativeMethods.EnumWindowsCallBack lpEnumFunc, int lParam);
        [DllImport("User32.Dll")]
        public static extern int GetDlgCtrlID(IntPtr hWndCtl);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetParent(HandleRef hWnd, HandleRef hWndParent);
        [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IntSetWindowText(HandleRef hWnd, string text);
        [SecurityCritical, SecuritySafeCritical]
        internal static void SetWindowText(HandleRef hWnd, string text)
        {
            if (!IntSetWindowText(hWnd, text))
            {
                throw new Win32Exception();
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint RegisterWindowMessage(string lpString);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetFocus();
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "GetParent", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr IntGetParent(HandleRef hWnd);
        [SecurityCritical]
        internal static IntPtr GetParent(HandleRef hWnd)
        {
            SetLastError(0);
            IntPtr ptr = IntGetParent(hWnd);
            int error = Marshal.GetLastWin32Error();
            if ((ptr == IntPtr.Zero) && (error != 0))
            {
                throw new Win32Exception(error);
            }
            return ptr;
        }
        [SecurityCritical]
        internal static int GetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount)
        {
            SetLastError(0);
            int num = IntGetWindowText(hWnd, lpString, nMaxCount);
            if (num == 0)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return num;
        }

        [SecurityCritical]
        internal static IntPtr CriticalSetWindowLong(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return new IntPtr(IntCriticalSetWindowLong(hWnd, nIndex, (int)dwNewLong.ToInt64()));
            }
            return IntCriticalSetWindowLongPtr(hWnd, nIndex, dwNewLong);
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        private static extern int IntCriticalSetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);


        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr IntCriticalSetWindowLongPtr(HandleRef hWnd, int nIndex, OPENFILENAME_I.WndProc dwNewLong);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern IntPtr IntCriticalSetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        internal static extern IntPtr UnsafeSendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool IsWindow(HandleRef hWnd);


        [DllImport("user32.dll", EntryPoint = "GetWindowPlacement", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool IntGetWindowPlacement(HandleRef hWnd, ref WINDOWPLACEMENT placement);

        [SecurityCritical, SecuritySafeCritical]
        internal static void GetWindowPlacement(HandleRef hWnd, ref WINDOWPLACEMENT placement)
        {
            if (!IntGetWindowPlacement(hWnd, ref placement))
            {
                throw new Win32Exception();
            }
        }



        [SecuritySafeCritical]
        internal static void SetWindowPlacement(HandleRef hWnd, [In] ref WINDOWPLACEMENT placement)
        {
            if (!IntSetWindowPlacement(hWnd, ref placement))
            {
                throw new Win32Exception();
            }
        }


        [DllImport("user32.dll", EntryPoint = "SetWindowPlacement", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool IntSetWindowPlacement(HandleRef hWnd, [In] ref WINDOWPLACEMENT placement);
 

        [SecurityCritical]
        public static bool EnableWindow(HandleRef hWnd, bool enable)
        {
            SetLastError(0);
            bool flag = IntEnableWindow(hWnd, enable);
            if (!flag)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return flag;
        }


        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "EnableWindow", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool IntEnableWindow(HandleRef hWnd, bool enable);

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int IntGetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount);
        [SecurityCritical]
        internal static int GetWindowTextLength(HandleRef hWnd)
        {
            SetLastError(0);
            int num = IntGetWindowTextLength(hWnd);
            if (num == 0)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return num;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint = "GetWindowTextLength", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int IntGetWindowTextLength(HandleRef hWnd);

        [DllImport("user32.dll")]
        internal static extern bool InvalidateRect(HandleRef hWnd, IntPtr lpRect, bool bErase);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint = "SetFocus", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr IntSetFocus(HandleRef hWnd);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern int CommDlgExtendedError();
        internal static IntPtr SetFocus(HandleRef hWnd)
        {
            SetLastError(0);
            IntPtr ptr = IntSetFocus(hWnd);
            int error = Marshal.GetLastWin32Error();
            if ((ptr == IntPtr.Zero) && (error != 0))
            {
                throw new Win32Exception(error);
            }
            return ptr;
        }
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern void SetLastError(int dwErrorCode);

        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(HandleRef parentHandle, HandleRef childAfter, string className, string windowTitle);

        [SecurityCritical]
        public static void GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect)
        {
            if (!IntGetWindowRect(hWnd, ref rect))
            {
                throw new Win32Exception();
            }
        }
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", EntryPoint = "GetWindowRect", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool IntGetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        [SecurityCritical]
        public static void GetClientRect(HandleRef hWnd, [In, Out] ref RECT rect)
        {
            if (!IntGetClientRect(hWnd, ref rect))
            {
                throw new Win32Exception();
            }
        }
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", EntryPoint = "GetClientRect", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool IntGetClientRect(HandleRef hWnd, [In, Out] ref RECT rect);
        [SecurityCritical]
        internal static int GetWindowLong(HandleRef hWnd, int nIndex)
        {
            int num = 0;
            IntPtr zero = IntPtr.Zero;
            int num2 = 0;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                num = IntGetWindowLong(hWnd, nIndex);
                num2 = Marshal.GetLastWin32Error();
                zero = new IntPtr(num);
            }
            else
            {
                zero = IntGetWindowLongPtr(hWnd, nIndex);
                num2 = Marshal.GetLastWin32Error();
                num = (int)zero.ToInt64();
            }
            if (zero == IntPtr.Zero)
            {
            }
            return num;
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int IntGetWindowLong(HandleRef hWnd, int nIndex);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr IntGetWindowLongPtr(HandleRef hWnd, int nIndex);
        [SecurityCritical]
        internal static IntPtr GetWindowLongPtr(HandleRef hWnd, GWL nIndex)
        {
            IntPtr zero = IntPtr.Zero;
            int num = 0;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                int num2 = IntGetWindowLong(hWnd, (int)nIndex);
                num = Marshal.GetLastWin32Error();
                zero = new IntPtr(num2);
            }
            else
            {
                zero = IntGetWindowLongPtr(hWnd, (int)nIndex);
                num = Marshal.GetLastWin32Error();
            }
            if (zero == IntPtr.Zero)
            {
            }
            return zero;
        }
        [DllImport("advapi32.dll", EntryPoint = "RegCreateKeyW")]
        public static extern int RegCreateKeyW([In] UIntPtr hKey, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey, out IntPtr phkResult);

        [DllImport("advapi32.dll", EntryPoint = "RegOverridePredefKey")]
        public static extern int RegOverridePredefKey([In] UIntPtr hKey, [In] IntPtr hNewHKey);

        [DllImport("advapi32.dll", EntryPoint = "RegCloseKey")]
        public static extern int RegCloseKey([In] IntPtr hKey);


        #region Enums

        #region ZOrderPos

        internal enum ZOrderPos
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2
        }
        #endregion
        [Flags]
        public enum SetWindowPosFlags
        {
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_DRAWFRAME = 0x0020,
            SWP_NOREPOSITION = 0x0200,
            SWP_DEFERERASE = 0x2000,
            SWP_ASYNCWINDOWPOS = 0x4000
        }
        #endregion
        #region DialogChangeStatus
        internal enum DialogChangeStatus : int
        {
            CDN_FIRST = -601,
            CDN_INITDONE = (CDN_FIRST - 0x0000),
            CDN_SELCHANGE = (CDN_FIRST - 0x0001),
            CDN_FOLDERCHANGE = (CDN_FIRST - 0x0002),
            CDN_SHAREVIOLATION = (CDN_FIRST - 0x0003),
            CDN_HELP = (CDN_FIRST - 0x0004),
            CDN_FILEOK = (CDN_FIRST - 0x0005),
            CDN_TYPECHANGE = (CDN_FIRST - 0x0006),
            CDN_INCLUDEITEM = (CDN_FIRST - 0x0007)
        }
        #endregion


        #region NCCALCSIZE_PARAMS

        [StructLayout(LayoutKind.Sequential)]
        internal struct NCCALCSIZE_PARAMS
        {
            public RECT rgrc0, rgrc1, rgrc2;
            public IntPtr lppos;
        }
        #endregion

        #region FolderViewMode


        public enum FolderViewMode
        {
            Default = 0x7028,
            Icon = Default + 1,
            SmallIcon = Default + 2,
            List = Default + 3,
            Details = Default + 4,
            Thumbnails = Default + 5,
            Tiles = Default + 6,
            Thumbstrip = Default + 7,
        }
        #endregion
        #region Window Styles

        [Flags]
        internal enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_CAPTION = 0x00C00000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_TILED = 0x00000000,
            WS_ICONIC = 0x20000000,
            WS_SIZEBOX = 0x00040000,
            WS_POPUPWINDOW = 0x80880000,
            WS_OVERLAPPEDWINDOW = 0x00CF0000,
            WS_TILEDWINDOW = 0x00CF0000,
            WS_CHILDWINDOW = 0x40000000
        }
        #endregion

        #region Window Extended Styles

        [Flags]
        internal enum WindowExtendedStyles
        {
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_OVERLAPPEDWINDOW = 0x00000300,
            WS_EX_PALETTEWINDOW = 0x00000188,
            WS_EX_LAYERED = 0x00080000
        }
        #endregion
        public enum AddonWindowLocation
        {
            BottomRight = 0,
            Right = 1,
            Bottom = 2
        }
        #region ControlIds
        internal enum ControlsId : int
        {
            ButtonOk = 0x1,
            ButtonCancel = 0x2,
            ButtonHelp = 0x40E,//0x0000040e
            GroupFolder = 0x440,
            LabelFileType = 0x441,
            LabelFileName = 0x442,
            LabelLookIn = 0x443,
            DefaultView = 0x461,
            LeftToolBar = 0x4A0,
            ComboFileName = 0x47c,
            ComboFileType = 0x470,
            ComboFolder = 0x471,
            CheckBoxReadOnly = 0x410
        }
        #endregion
        public enum Msg
        {
            WM_NULL = 0x0000,
            WM_CREATE = 0x0001,
            WM_DESTROY = 0x0002,
            WM_MOVE = 0x0003,
            WM_SIZE = 0x0005,
            WM_ACTIVATE = 0x0006,
            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_ENABLE = 0x000A,
            WM_SETREDRAW = 0x000B,
            WM_SETTEXT = 0x000C,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            WM_PAINT = 0x000F,
            WM_CLOSE = 0x0010,
            WM_QUERYENDSESSION = 0x0011,
            WM_QUIT = 0x0012,
            WM_QUERYOPEN = 0x0013,
            WM_ERASEBKGND = 0x0014,
            WM_SYSCOLORCHANGE = 0x0015,
            WM_ENDSESSION = 0x0016,
            WM_SHOWWINDOW = 0x0018,
            WM_CTLCOLOR = 0x0019,
            WM_WININICHANGE = 0x001A,
            WM_SETTINGCHANGE = 0x001A,
            WM_DEVMODECHANGE = 0x001B,
            WM_ACTIVATEAPP = 0x001C,
            WM_FONTCHANGE = 0x001D,
            WM_TIMECHANGE = 0x001E,
            WM_CANCELMODE = 0x001F,
            WM_SETCURSOR = 0x0020,
            WM_MOUSEACTIVATE = 0x0021,
            WM_CHILDACTIVATE = 0x0022,
            WM_QUEUESYNC = 0x0023,
            WM_GETMINMAXINFO = 0x0024,
            WM_PAINTICON = 0x0026,
            WM_ICONERASEBKGND = 0x0027,
            WM_NEXTDLGCTL = 0x0028,
            WM_SPOOLERSTATUS = 0x002A,
            WM_DRAWITEM = 0x002B,
            WM_MEASUREITEM = 0x002C,
            WM_DELETEITEM = 0x002D,
            WM_VKEYTOITEM = 0x002E,
            WM_CHARTOITEM = 0x002F,
            WM_SETFONT = 0x0030,
            WM_GETFONT = 0x0031,
            WM_SETHOTKEY = 0x0032,
            WM_GETHOTKEY = 0x0033,
            WM_QUERYDRAGICON = 0x0037,
            WM_COMPAREITEM = 0x0039,
            WM_GETOBJECT = 0x003D,
            WM_COMPACTING = 0x0041,
            WM_COMMNOTIFY = 0x0044,
            WM_WINDOWPOSCHANGING = 0x0046,
            WM_WINDOWPOSCHANGED = 0x0047,
            WM_POWER = 0x0048,
            WM_COPYDATA = 0x004A,
            WM_CANCELJOURNAL = 0x004B,
            WM_NOTIFY = 0x004E,
            WM_INPUTLANGCHANGEREQUEST = 0x0050,
            WM_INPUTLANGCHANGE = 0x0051,
            WM_TCARD = 0x0052,
            WM_HELP = 0x0053,
            WM_USERCHANGED = 0x0054,
            WM_NOTIFYFORMAT = 0x0055,
            WM_CONTEXTMENU = 0x007B,
            WM_STYLECHANGING = 0x007C,
            WM_STYLECHANGED = 0x007D,
            WM_DISPLAYCHANGE = 0x007E,
            WM_GETICON = 0x007F,
            WM_SETICON = 0x0080,
            WM_NCCREATE = 0x0081,
            WM_NCDESTROY = 0x0082,
            WM_NCCALCSIZE = 0x0083,
            WM_NCHITTEST = 0x0084,
            WM_NCPAINT = 0x0085,
            WM_NCACTIVATE = 0x0086,
            WM_GETDLGCODE = 0x0087,
            WM_SYNCPAINT = 0x0088,
            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9,
            WM_NCXBUTTONDOWN = 0x00AB,
            WM_NCXBUTTONUP = 0x00AC,
            WM_NCXBUTTONDBLCLK = 0x00AD,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,
            WM_KEYLAST = 0x0108,
            WM_IME_STARTCOMPOSITION = 0x010D,
            WM_IME_ENDCOMPOSITION = 0x010E,
            WM_IME_COMPOSITION = 0x010F,
            WM_IME_KEYLAST = 0x010F,
            WM_INITDIALOG = 0x0110,
            WM_COMMAND = 0x0111,
            WM_SYSCOMMAND = 0x0112,
            WM_TIMER = 0x0113,
            WM_HSCROLL = 0x0114,
            WM_VSCROLL = 0x0115,
            WM_INITMENU = 0x0116,
            WM_INITMENUPOPUP = 0x0117,
            WM_MENUSELECT = 0x011F,
            WM_MENUCHAR = 0x0120,
            WM_ENTERIDLE = 0x0121,
            WM_MENURBUTTONUP = 0x0122,
            WM_MENUDRAG = 0x0123,
            WM_MENUGETOBJECT = 0x0124,
            WM_UNINITMENUPOPUP = 0x0125,
            WM_MENUCOMMAND = 0x0126,
            WM_CTLCOLORMSGBOX = 0x0132,
            WM_CTLCOLOREDIT = 0x0133,
            WM_CTLCOLORLISTBOX = 0x0134,
            WM_CTLCOLORBTN = 0x0135,
            WM_CTLCOLORDLG = 0x0136,
            WM_CTLCOLORSCROLLBAR = 0x0137,
            WM_CTLCOLORSTATIC = 0x0138,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,
            WM_MOUSEWHEEL = 0x020A,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C,
            WM_XBUTTONDBLCLK = 0x020D,
            WM_PARENTNOTIFY = 0x0210,
            WM_ENTERMENULOOP = 0x0211,
            WM_EXITMENULOOP = 0x0212,
            WM_NEXTMENU = 0x0213,
            WM_SIZING = 0x0214,
            WM_CAPTURECHANGED = 0x0215,
            WM_MOVING = 0x0216,
            WM_DEVICECHANGE = 0x0219,
            WM_MDICREATE = 0x0220,
            WM_MDIDESTROY = 0x0221,
            WM_MDIACTIVATE = 0x0222,
            WM_MDIRESTORE = 0x0223,
            WM_MDINEXT = 0x0224,
            WM_MDIMAXIMIZE = 0x0225,
            WM_MDITILE = 0x0226,
            WM_MDICASCADE = 0x0227,
            WM_MDIICONARRANGE = 0x0228,
            WM_MDIGETACTIVE = 0x0229,
            WM_MDISETMENU = 0x0230,
            WM_ENTERSIZEMOVE = 0x0231,
            WM_EXITSIZEMOVE = 0x0232,
            WM_DROPFILES = 0x0233,
            WM_MDIREFRESHMENU = 0x0234,
            WM_IME_SETCONTEXT = 0x0281,
            WM_IME_NOTIFY = 0x0282,
            WM_IME_CONTROL = 0x0283,
            WM_IME_COMPOSITIONFULL = 0x0284,
            WM_IME_SELECT = 0x0285,
            WM_IME_CHAR = 0x0286,
            WM_IME_REQUEST = 0x0288,
            WM_IME_KEYDOWN = 0x0290,
            WM_IME_KEYUP = 0x0291,
            WM_MOUSEHOVER = 0x02A1,
            WM_MOUSELEAVE = 0x02A3,
            WM_CUT = 0x0300,
            WM_COPY = 0x0301,
            WM_PASTE = 0x0302,
            WM_CLEAR = 0x0303,
            WM_UNDO = 0x0304,
            WM_RENDERFORMAT = 0x0305,
            WM_RENDERALLFORMATS = 0x0306,
            WM_DESTROYCLIPBOARD = 0x0307,
            WM_DRAWCLIPBOARD = 0x0308,
            WM_PAINTCLIPBOARD = 0x0309,
            WM_VSCROLLCLIPBOARD = 0x030A,
            WM_SIZECLIPBOARD = 0x030B,
            WM_ASKCBFORMATNAME = 0x030C,
            WM_CHANGECBCHAIN = 0x030D,
            WM_HSCROLLCLIPBOARD = 0x030E,
            WM_QUERYNEWPALETTE = 0x030F,
            WM_PALETTEISCHANGING = 0x0310,
            WM_PALETTECHANGED = 0x0311,
            WM_HOTKEY = 0x0312,
            WM_PRINT = 0x0317,
            WM_PRINTCLIENT = 0x0318,
            WM_THEME_CHANGED = 0x031A,
            WM_HANDHELDFIRST = 0x0358,
            WM_HANDHELDLAST = 0x035F,
            WM_AFXFIRST = 0x0360,
            WM_AFXLAST = 0x037F,
            WM_PENWINFIRST = 0x0380,
            WM_PENWINLAST = 0x038F,
            WM_APP = 0x8000,
            WM_USER = 0x0400,
            WM_REFLECT = WM_USER + 0x1c00
        }


        /// ////////////////////////////////////////////////////////////
        //[DllImport("user32.dll")]
        //public static extern IntPtr GetDC(HandleRef hWnd);
        //[DllImport("user32.dll")]
        //public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        //[DllImport("gdi32.dll")]
        //public static extern int GetBkColor(IntPtr hdc);
        //[DllImport("gdi32.dll")]
        //public static extern uint SetBkColor(IntPtr hdc, int crColor);
        //[StructLayout(LayoutKind.Explicit, Size = 4)]
        //public struct COLORREF
        //{
        //    public COLORREF(byte r, byte g, byte b)
        //    {
        //        this.Value = 0;
        //        this.R = r;
        //        this.G = g;
        //        this.B = b;
        //    }

        //    public COLORREF(int value)
        //    {
        //        this.R = 0;
        //        this.G = 0;
        //        this.B = 0;
        //        unchecked{
        //            this.Value = value & (int)0x00FFFFFF;
        //        };
        //    }

        //    [FieldOffset(0)]
        //    public byte R;
        //    [FieldOffset(1)]
        //    public byte G;
        //    [FieldOffset(2)]
        //    public byte B;

        //    [FieldOffset(0)]
        //    public int Value;
        //}
    }
}


