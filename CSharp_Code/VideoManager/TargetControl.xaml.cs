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
using System.Collections.Generic;

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfCustomFileDialog;
using WMEncoderLib;
using System.Runtime.InteropServices;

namespace VideoManager
{
    /// <summary>
    /// Interaction logic for TargetControl.xaml
    /// </summary>
    public partial class TargetControl : UserControl, IWindowExt
    {
        public TargetControl()
        {
            InitializeComponent();
        }

        const WMENC_VIDEO_OPTIMIZATION optSTANDARD = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_STANDARD;
        const WMENC_VIDEO_OPTIMIZATION optDEINTERLACE = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_DEINTERLACE;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_AUTO = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_AA_TOP = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_AA_TOP;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_BB_TOP = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_BB_TOP;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_BC_TOP = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_BC_TOP;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_CD_TOP = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_CD_TOP;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_DD_TOP = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_DD_TOP;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_AA_BOTTOM = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_AA_BOTTOM;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_BB_BOTTOM = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_BB_BOTTOM;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_BC_BOTTOM = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_BC_BOTTOM;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_CD_BOTTOM = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_CD_BOTTOM;
        const WMENC_VIDEO_OPTIMIZATION optTELECINE_DD_BOTTOM = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INVERSETELECINE | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_TELECINE_DD_BOTTOM;
        const WMENC_VIDEO_OPTIMIZATION optINTERLACED_AUTO = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_PROCESS_INTERLACED | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INTERLACED_AUTO;
        const WMENC_VIDEO_OPTIMIZATION optINTERLACED_TOP_FIRST = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_PROCESS_INTERLACED | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INTERLACED_TOP_FIRST;
        const WMENC_VIDEO_OPTIMIZATION optINTERLACED_BOTTOM_FIRST = WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_PROCESS_INTERLACED | WMENC_VIDEO_OPTIMIZATION.WMENC_VIDEO_INTERLACED_BOTTOM_FIRST;

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            _parentDlg.FileDlgDefaultViewMode = NativeMethods.FolderViewMode.List;
            SaveFileDialog<TargetControl> fd = _parentDlg as SaveFileDialog<TargetControl>;
            fd.FileOk += new System.ComponentModel.CancelEventHandler(CreateEncodeInfo);
         }

        void CreateEncodeInfo(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _encodeInfo = new strucEncodeInfo();
            try
            {
                _encodeInfo.Profile = ProfileFile.Text;
                if (ComboBox_PreProc.Text == "Standard")
                {
                    _encodeInfo.Preproc = optSTANDARD;
                }
                else if (ComboBox_PreProc.Text == "Deinterlace")
                {
                    _encodeInfo.Preproc = optDEINTERLACE;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine Auto")
                {
                    _encodeInfo.Preproc = optTELECINE_AUTO;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine AA Top")
                {
                    _encodeInfo.Preproc = optTELECINE_AA_TOP;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine BB Top")
                {
                    _encodeInfo.Preproc = optTELECINE_BB_TOP;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine BC Top")
                {
                    _encodeInfo.Preproc = optTELECINE_BC_TOP;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine CD Top")
                {
                    _encodeInfo.Preproc = optTELECINE_CD_TOP;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine DD Top")
                {
                    _encodeInfo.Preproc = optTELECINE_DD_TOP;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine AA Bottom")
                {
                    _encodeInfo.Preproc = optTELECINE_AA_BOTTOM;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine BB Bottom")
                {
                    _encodeInfo.Preproc = optTELECINE_BB_BOTTOM;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine BC Bottom")
                {
                    _encodeInfo.Preproc = optTELECINE_BC_BOTTOM;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine CD Bottom")
                {
                    _encodeInfo.Preproc = optTELECINE_CD_BOTTOM;
                }
                else if (ComboBox_PreProc.Text == "Inverse Telecine DD Bottom")
                {
                    _encodeInfo.Preproc = optTELECINE_DD_BOTTOM;
                }
                else if (ComboBox_PreProc.Text == "Process Interlaced Auto")
                {
                    _encodeInfo.Preproc = optINTERLACED_AUTO;
                }
                else if (ComboBox_PreProc.Text == "Process Interlaced Top First")
                {
                    _encodeInfo.Preproc = optINTERLACED_TOP_FIRST;
                }
                else if (ComboBox_PreProc.Text == "Process Interlaced Bottom First")
                {
                    _encodeInfo.Preproc = optINTERLACED_BOTTOM_FIRST;
                }
                _encodeInfo.DRMProfile = ComboBox_DRMProfile.Text;
                if (ComboBox_DRMProfile.Text.Length == 0)
                    _encodeInfo.DRMProfile = "None";
                _encodeInfo.Crop = CheckBox_Crop.IsChecked == true;
                _encodeInfo.CropTop = (Text_Top.Text.Length == 0) ? 0 : long.Parse(Text_Top.Text);
                _encodeInfo.CropLeft = (Text_Left.Text.Length == 0) ? 0 : long.Parse(Text_Left.Text);
                _encodeInfo.CropRight = (Text_Right.Text.Length == 0) ? 0 : long.Parse(Text_Right.Text);
                _encodeInfo.CropBottom = (Text_Bottom.Text.Length == 0) ? 0 : long.Parse(Text_Bottom.Text);
                _encodeInfo.TwoPass = CheckBox_TwoPass.IsChecked.Value;
                _encodeInfo.Title = TextBox_Title.Text;
                _encodeInfo.Description = TextBox_Description.Text;
                _encodeInfo.Author = TextBox_Author.Text;
                _encodeInfo.Copyright = TextBox_Copyright.Text;
   
 
           
    }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message, "error in the encoder parameters", MessageBoxButton.OK, MessageBoxImage.Stop);
                e.Cancel = true;
            }
        }

        strucEncodeInfo _encodeInfo;

        internal strucEncodeInfo EncodeInfo
        {
            get { return _encodeInfo; }
            set { _encodeInfo = value; }
        }

        private void sEnumPreprocess()
        {
            try
            {
                if (ComboBox_PreProc.Items.Count > 0)
                    return;
                ComboBox_PreProc.Items.Add("Standard");
                ComboBox_PreProc.Items.Add("Deinterlace");
                ComboBox_PreProc.Items.Add("Inverse Telecine Auto");
                ComboBox_PreProc.Items.Add("Inverse Telecine AA Top");
                ComboBox_PreProc.Items.Add("Inverse Telecine BB Top");
                ComboBox_PreProc.Items.Add("Inverse Telecine BC Top");
                ComboBox_PreProc.Items.Add("Inverse Telecine CD Top");
                ComboBox_PreProc.Items.Add("Inverse Telecine DD Top");
                ComboBox_PreProc.Items.Add("Inverse Telecine AA Bottom");
                ComboBox_PreProc.Items.Add("Inverse Telecine BB Bottom");
                ComboBox_PreProc.Items.Add("Inverse Telecine BC Bottom");
                ComboBox_PreProc.Items.Add("Inverse Telecine CD Bottom");
                ComboBox_PreProc.Items.Add("Inverse Telecine DD Bottom");
                ComboBox_PreProc.Items.Add("Process Interlaced Auto");
                ComboBox_PreProc.Items.Add("Process Interlaced Top First");
                ComboBox_PreProc.Items.Add("Process Interlaced Bottom First");
                ComboBox_PreProc.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                string strError = ex.ToString();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;
            if (e.AddedItems[0] == DRMProfile)
            {
                sEnumDRMProfiles();
            }
            else if (e.AddedItems[0] == Profile)
            {
                sEnumPreprocess();
                if (ProfileFile.Text.Length == 0)
                {
                    ComboBox_PreProc.IsEnabled = false;
                    ComboBox_PreProc.SelectedIndex = -1;
                }
                else
                {
                    ComboBox_PreProc.IsEnabled = true;
                }
            }

        }


        System.Windows.Interop.HwndSource _source;
        IFileDlgExt _parentDlg;
        #region IWindowExt Members

        public System.Windows.Interop.HwndSource Source
        {
            set
            {
                _source = value;
            }
        }

        public IFileDlgExt ParentDlg
        {
            set { _parentDlg = value; }
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

 

        private void SelectProfile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.InitialDirectory = @"C:\Program Files\Windows Media Components\Encoder\Profiles";
            ofd.Filter = "prx files (*.prx)|*.prx|All files (*.*)|*.*";
            bool? res = ofd.ShowDialog();
            if (res.HasValue && res.Value)
            {
                ProfileFile.Text = ofd.FileName;
                sEvalProfile(ofd.FileName);
                ComboBox_PreProc.SelectedIndex = -1;
                ComboBox_PreProc.IsEnabled = true;
            }
        }
        private void sEvalProfile(string strProfilePath)
        {
            WMEncProfile2 pro = null;
            int intVideoMode = 0;
            int intAudioMode = 0;
            bool boolVideoTwoPassLock = false;
            int intVideoPasses = 0;
            bool boolAudioTwoPassLock = false;
            int intAudioPasses = 0;
            int intProContentType = 0;
            try
            {
                pro = new WMEncProfile2();
                pro.LoadFromFile(strProfilePath);
                intProContentType = pro.ContentType;
                intVideoMode = (int)pro.get_VBRMode(WMENC_SOURCE_TYPE.WMENC_VIDEO, 0);
                intAudioMode = (int)pro.get_VBRMode(WMENC_SOURCE_TYPE.WMENC_AUDIO, 0);
                if (intVideoMode == 1)
                {
                    boolVideoTwoPassLock = false;
                    intVideoPasses = 0;
                }
                else if (intVideoMode == 2)
                {
                    boolVideoTwoPassLock = true;
                    intVideoPasses = 2;
                }
                else if (intVideoMode == 3)
                {
                    boolVideoTwoPassLock = true;
                    intVideoPasses = 1;
                }
                else if (intVideoMode == 4)
                {
                    boolVideoTwoPassLock = true;
                    intVideoPasses = 2;
                }
                if (intAudioMode == 1)
                {
                    boolAudioTwoPassLock = false;
                    intAudioPasses = 0;
                }
                else if (intAudioMode == 2)
                {
                    boolAudioTwoPassLock = true;
                    intAudioPasses = 2;
                }
                else if (intAudioMode == 3)
                {
                    boolAudioTwoPassLock = true;
                    intAudioPasses = 1;
                }
                else if (intAudioMode == 4)
                {
                    boolAudioTwoPassLock = true;
                    intAudioPasses = 2;
                }
                if (boolAudioTwoPassLock == true | boolVideoTwoPassLock == true)
                {
                    CheckBox_TwoPass.IsEnabled = false;
                }
                else
                {
                    CheckBox_TwoPass.IsEnabled = true;
                }
                if (intVideoPasses > intAudioPasses)
                {
                    if (intVideoPasses == 0)
                    {
                        CheckBox_TwoPass.IsChecked = false;
                    }
                    else if (intVideoPasses == 1)
                    {
                        CheckBox_TwoPass.IsChecked = false;
                    }
                    else if (intVideoPasses == 2)
                    {
                        CheckBox_TwoPass.IsChecked = true;
                    }
                }
                else
                {
                    if (intAudioPasses == 0)
                    {
                        CheckBox_TwoPass.IsChecked = false;
                    }
                    else if (intAudioPasses == 1)
                    {
                        CheckBox_TwoPass.IsChecked = false;
                    }
                    else if (intAudioPasses == 2)
                    {
                        CheckBox_TwoPass.IsChecked = true;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                InstallEncoderQuery(ex);
            }
            catch (Exception ex)
            {
                string strError = ex.ToString();
                _parentDlg.FileDlgEnableOkBtn = false;
            }
            finally
            {
                if(pro != null)
                Marshal.ReleaseComObject(pro);

            }
        }


        private void sEnumDRMProfiles()
        {
            if (ComboBox_DRMProfile.Items.Count > 0)
                return;
            try
            {
                WMEncoder tempEncoder = new WMEncoder();
                IWMDRMContentAuthor DRM = tempEncoder.EncoderDRMContentAuthor;
                IWMDRMProfileCollection DRMProColl = DRM.DRMProfileCollection;
 int i;

                ComboBox_DRMProfile.Items.Add("None");
                for (i = 0; i < DRMProColl.Count; i++)
                {
                    ComboBox_DRMProfile.Items.Add(DRMProColl.Item(i).Name);
                }
                ComboBox_DRMProfile.SelectedIndex = 0;
                tempEncoder = null;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                InstallEncoderQuery(ex);
            }
            catch (Exception ex)
            {
                string strError = ex.ToString();
            }
        }

        private void SelectProfile_Initialized(object sender, EventArgs e)
        {


        }

        private void ComboBox_PreProc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WMEncProfile2 pro = null;
            try
            {
                if (System.IO.File.Exists(ProfileFile.Text) && ComboBox_PreProc.SelectedIndex != -1)
                {
                    pro =  new WMEncProfile2();
                    pro.LoadFromFile(ProfileFile.Text);
                    int intProContentType = pro.ContentType;
                    string strPreProc = (sender as ComboBox).SelectedValue.ToString();
                    strPreProc = strPreProc.Remove(0, strPreProc.LastIndexOf(':') + 1);

                    if (intProContentType == 16 || intProContentType == 17)
                    {
                        pro.set_InterlaceMode(0, false);
                        switch (strPreProc)
                        {
                            case "Process Interlaced Auto":
                            case "Process Interlaced Top First":
                            case "Process Interlaced Bottom First":
                                MessageBox.Show("The profile does not support the type of preprocessing selected." + Environment.NewLine + Environment.NewLine + "Change the preprocessing selected or check the box" + Environment.NewLine + "in the profile editor next to 'Allow Interlaced Processing' for this profile.",
                                    "Error");
                                ComboBox_PreProc.Focus();
                                ComboBox_PreProc.SelectedIndex = -1;
                                break;
                        }

                    }

                    if (intProContentType == 1)
                    {
                        SaveFileDialog<TargetWindow> dlg = _parentDlg as SaveFileDialog<TargetWindow>;
                        dlg.FilterIndex = 3;
                    }

                }
                _parentDlg.FileDlgEnableOkBtn = ComboBox_PreProc.SelectedIndex != -1;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                InstallEncoderQuery(ex);
            }
            finally
            {
                if (pro != null)
                    Marshal.ReleaseComObject(pro);

            }

        }

        private void InstallEncoderQuery(COMException ex)
        {
            if ((uint)ex.ErrorCode == 0x80040154)
            {
                if (MessageBox.Show("Windows Media Encoder 9 is not installed." + Environment.NewLine +
                    "Do you want to download and install it?", "Error", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start("www.softpedia.com/progDownload/Windows-Media-Encoder-Download-1393.html");
                }
                else
                {
                    //Application.Current.Shutdown(-1);
                }
                _parentDlg.FileDlgEnableOkBtn = false;
            }
        }

        private void CheckBox_Crop_Checked(object sender, RoutedEventArgs e)
        {
            Text_Top.IsEnabled = true;
            Text_Top.IsReadOnly = false;
            Text_Top.Focusable = true;

        }
    }
}
