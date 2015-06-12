using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Copyright © Decebal Mihailescu 2015

// All rights reserved.
// This code is released under The Code Project Open License (CPOL) 1.02
// The full licensing terms are available at http://www.codeproject.com/info/cpol10.aspx
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
// REMAINS UNCHANGED.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfCustomFileDialog
{
    /// <summary>
    /// Interaction logic for ControlAddOnBase.xaml
    /// </summary>
    public partial class ControlAddOnBase : UserControl, IWindowExt
    {

        System.Windows.Interop.HwndSource _source;
        IFileDlgExt _parentDlg;
        #region IWindowExt Members
        public System.Windows.Interop.HwndSource Source
        {
            set
            { _source = value; }

        }

        public IFileDlgExt ParentDlg
        {
            set { _parentDlg = value; }
            get { return _parentDlg; }
        }
        #endregion

    }
}
