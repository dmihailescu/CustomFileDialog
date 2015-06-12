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
using System.Windows;
using System.Runtime.InteropServices;

namespace WpfCustomFileDialog
{
    public class WindowAddOnBase : Window, IWindowExt
    {// to be used as here: http://geekswithblogs.net/lbugnion/archive/2007/03/02/107747.aspx
        //WPF: Inheriting from custom class instead of Window
        System.Windows.Interop.HwndSource _source;
        IFileDlgExt _parentDlg;
        #region IWindowExt Members
        public System.Windows.Interop.HwndSource Source
        {
            set
            {_source = value;} 
            
        }

        public IFileDlgExt ParentDlg
        {
            set { _parentDlg = value; }
            get { return _parentDlg; }
        }
        #endregion
        protected override Size ArrangeOverride(Size arrangeBounds)
        {

            if (Height > 0 && Width > 0)
            {
                arrangeBounds.Height = this.Height;
                arrangeBounds.Width = this.Width;
            }
            return base.ArrangeOverride(arrangeBounds);
        }
       
    }
}
