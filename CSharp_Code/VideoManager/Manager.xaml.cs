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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCustomFileDialog;
using System.IO;
using WMEncoderLib;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;

//mostly from http://www.codeproject.com/KB/audio-video/ConvertVideoFileFormats.aspx
namespace VideoManager
{
    struct strucEncodeInfo
    {
        public string Source;
        public string Destination;
        public string Profile;
        public string DRMProfile;
        public string Title;
        public string Description;
        public string Author;
        public string Copyright;
        public bool Crop;
        public long CropLeft;
        public long CropTop;
        public long CropRight;
        public long CropBottom;
        public WMENC_VIDEO_OPTIMIZATION Preproc;
        public bool TwoPass;
    }
    /// <summary>
    /// Interaction logic for Manager.xaml
    /// </summary>
    public partial class Manager : Window
    {
        public Manager()
        {

            InitializeComponent();
        }


        bool _TwoPassEncoding = false;
        int glbPassNumber;

        strucEncodeInfo _encodeInfo;
        private void btnSource_Click(object sender, RoutedEventArgs e)
        {
            WpfCustomFileDialog.OpenFileDialog<SelectWindow> ofd = new WpfCustomFileDialog.OpenFileDialog<SelectWindow>();
            ofd.Filter = "avi files (*.avi)|*.avi|wmv files (*.wmv)|*.wmv|All files (*.*)|*.*";
            ofd.Multiselect = false;
            ofd.Title = "Select Media file using a window";
            ofd.FileDlgStartLocation = AddonWindowLocation.Right;
            ofd.FileDlgDefaultViewMode = NativeMethods.FolderViewMode.Tiles;
            ofd.FileDlgOkCaption = "&Select";
            ofd.FileDlgEnableOkBtn = false;
            ofd.SetPlaces(new object[] { @"c:\", (int)Places.MyComputer, (int)Places.Favorites, (int)Places.All_Users_MyVideo, (int)Places.MyVideos });
            bool? res = ofd.ShowDialog(this);

            if (res.Value == true)
            {
                txtSource.Text = ofd.FileName;
                _encodeInfo.Source = txtSource.Text;
            }
        }

        private void btnTarget_Click(object sender, RoutedEventArgs e)
        {

            WpfCustomFileDialog.SaveFileDialog<TargetWindow> sfd = new WpfCustomFileDialog.SaveFileDialog<TargetWindow>();
            sfd.ValidateNames = true;
            sfd.Title = "Save as using a Window";
            sfd.FileDlgStartLocation = AddonWindowLocation.Bottom;
            sfd.Filter = "wmv files (*.wmv)|*.wmv|avi files (*.avi)|*.avi|wma files (*.wma)|*.wma|All files (*.*)|*.*";
            sfd.CheckPathExists = true;
            if (File.Exists(txtSource.Text))
            {
                sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(txtSource.Text) + "_converted";
            }
            Microsoft.Win32.OpenFileDialog test= new Microsoft.Win32.OpenFileDialog();
            sfd.SetPlaces(new object[] { @"c:\", (int)Places.MyComputer, (int)Places.Favorites, (int)Places.All_Users_MyVideo, (int)Places.MyVideos });
            bool? res = sfd.ShowDialog();
            if (res.Value == true)
            {
                _encodeInfo = (sfd.ChildWnd as TargetWindow).EncodeInfo;
                _encodeInfo.Destination = txtDest.Text = sfd.FileName;

            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            return;

        }

        int glbintSourceDuration;

        private string sReturnExtension(string strValue)
        {
            try
            {
                return strValue.Substring(strValue.Length - 3).ToLower();
            }
            catch (Exception)
            {
            }
            return "";
        }
        private void sRemoveSrcGrpColl()
        {
            try
            {
                if (_SrcGrpColl != null)
                {
                    _SrcGrpColl.Remove(0);
                    //_boolSrcGrpColl = false;
                }
            }
            catch (Exception ex)
            {
                string strError = ex.ToString();
                sDisplayErrMessage(strError);
            }
        }

        private void sReportPercentComplete(WMEncoder glbEncoder)
        {
            if (glbEncoder.RunState == WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING)
            {
                IWMEncStatistics Stats = glbEncoder.Statistics;
                IWMEncFileArchiveStats FileStats = (IWMEncFileArchiveStats)Stats.FileArchiveStats;
                int intCurrentFileDuration;
                int intPercentComplete;
                intCurrentFileDuration = System.Convert.ToInt32(FileStats.FileDuration * 10);
                try
                {
                    intPercentComplete = 100 * intCurrentFileDuration / glbintSourceDuration;
                    _backgroundWorker.ReportProgress(intPercentComplete, intPercentComplete.ToString() + "% Complete");

                }
                catch (Exception ex)
                {
                    string strError = ex.ToString();
                    sDisplayErrMessage(strError);
                }
                finally
                {
                    FileStats = null;
                    Stats = null;
                }
            }

        }
        private void sDisplayErrMessage(string strError)
        {
            MessageBox.Show(this, strError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void sSetInitialValues()
        {

            try
            {
                StatusBar.Content = "Ready";
            }
            catch (Exception)
            {

                return;
            }
            try
            {

            }
            catch (Exception ex)
            {
                string strError = ex.ToString();
                sDisplayErrMessage(strError);
            }
        }


        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {


            string glbstrErrLocation;
            WMEncoder Encoder = null;
            WMEncProfile2 Profile = null;
            IWMDRMContentAuthor DRMAuthor = null;
            IWMDRMProfileCollection DRMProColl = null;
            IWMDRMProfile DRMPro = null;
            IWMEncSourceGroup SrcGrp = null;
            IWMEncAudioSource SrcAud = null;
            IWMEncVideoSource2 SrcVid = null;
            _TwoPassEncoding = false;


            bool glbboolEncodingContinue = true; ;

            DateTime time = DateTime.Now;
            try
            {

                Encoder = new WMEncoder();
                Encoder.OnStateChange += new _IWMEncoderEvents_OnStateChangeEventHandler(this.Encoder_OnStateChange);
                _SrcGrpColl = Encoder.SourceGroupCollection;

                try
                {
                    DRMAuthor = Encoder.EncoderDRMContentAuthor;
                    DRMProColl = DRMAuthor.DRMProfileCollection;
                    DRMPro = null;

                    object vKeyID = null;
                    if (_encodeInfo.DRMProfile != "None")
                    {
                        int intDRMProCount = 0;
                        for (intDRMProCount = 0; intDRMProCount <= DRMProColl.Count - 1; intDRMProCount++)
                        {
                            if (DRMProColl.Item(intDRMProCount).Name == _encodeInfo.DRMProfile)
                            {
                                DRMPro = DRMProColl.Item(intDRMProCount);
                                break;
                            }
                        }
                        DRMAuthor.SetSessionDRMProfile(DRMPro.ID, ref vKeyID);
                    }
                    else
                    {
                        DRMAuthor.SetSessionDRMProfile(System.DBNull.Value.ToString(), ref vKeyID);
                        DRMAuthor = null;
                        DRMProColl = null;
                        DRMPro = null;
                        vKeyID = null;
                    }
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Specify DRM Profile - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);
                }
                Profile = new WMEncProfile2();
                Int32 intProContentType = 0;
                int intProVBRModeAudio = 0;
                int intProVBRModeVideo = 0;
                try
                {
                    Profile.LoadFromFile(_encodeInfo.Profile);
                    intProContentType = Profile.ContentType;
                    intProVBRModeAudio = (int)Profile.get_VBRMode(WMENC_SOURCE_TYPE.WMENC_AUDIO, 0);
                    intProVBRModeVideo = (int)Profile.get_VBRMode(WMENC_SOURCE_TYPE.WMENC_VIDEO, 0);
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Load Profile - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);
                }

                try
                {
                    SrcGrp = _SrcGrpColl.Add("BatchEncode");
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Source Group - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);
                }
                string strDestExtension = System.IO.Path.GetExtension(_encodeInfo.Destination);

                try
                {
                    if (intProContentType == 1)
                    {
                        SrcAud = (WMEncoderLib.IWMEncAudioSource)SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_AUDIO);
                        SrcAud.SetInput(_encodeInfo.Source, "", "");
                    }
                    else if (intProContentType == 16)
                    {
                        SrcVid = (WMEncoderLib.IWMEncVideoSource2)SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_VIDEO);
                        SrcVid.SetInput(_encodeInfo.Source, "", "");
                    }
                    else if (intProContentType == 17)
                    {
                        SrcAud = (WMEncoderLib.IWMEncAudioSource)SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_AUDIO);
                        SrcAud.SetInput(_encodeInfo.Source, "", "");
                        SrcVid = (WMEncoderLib.IWMEncVideoSource2)SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_VIDEO);
                        SrcVid.SetInput(_encodeInfo.Source, "", "");
                    }
                    else
                    {
                        _backgroundWorker.ReportProgress(0, "BatchEncode does not support this type of profile");
                    }
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Video / Audio Source - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);

                }
                try
                {
                    SrcGrp.set_Profile(Profile);
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Encoding Profile - " + ex.Message.ToString();

                    throw new ApplicationException(glbstrErrLocation, ex);
                }
                IWMEncDisplayInfo Descr = Encoder.DisplayInfo;
                try
                {
                    if (_encodeInfo.Title != "")
                    {
                        Descr.Title = _encodeInfo.Title;
                    }
                    if (_encodeInfo.Description != "")
                    {
                        Descr.Description = _encodeInfo.Description;
                    }
                    if (_encodeInfo.Author != "")
                    {
                        Descr.Author = _encodeInfo.Author;
                    }
                    if (_encodeInfo.Copyright != "")
                    {
                        Descr.Copyright = _encodeInfo.Copyright;
                    }
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Display Information - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);
                }
                try
                {
                    if (intProContentType == 16 || intProContentType == 17)
                    {
                        if (_encodeInfo.Crop == true)
                        {
                            SrcVid.CroppingBottomMargin = (int)_encodeInfo.CropBottom;
                            SrcVid.CroppingTopMargin = (int)_encodeInfo.CropTop;
                            SrcVid.CroppingLeftMargin = (int)_encodeInfo.CropLeft;
                            SrcVid.CroppingRightMargin = (int)_encodeInfo.CropRight;
                        }
                    }
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Cropping - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);
                }
                try
                {
                    if (intProContentType == 16 || intProContentType == 17)
                    {
                        SrcVid.Optimization = _encodeInfo.Preproc;
                    }
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Video Optimization - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);
                }

                try
                {
                    if (intProContentType == 1)
                    {
                        if (_encodeInfo.TwoPass == false)
                        {
                            SrcAud.PreProcessPass = 0;
                            _TwoPassEncoding = false;
                        }
                        else
                        {
                            SrcAud.PreProcessPass = 1;
                            _TwoPassEncoding = true;
                            glbPassNumber = 1;
                        }
                    }
                    else if (intProContentType == 16)
                    {
                        if (_encodeInfo.TwoPass == false)
                        {
                            SrcVid.PreProcessPass = 0;
                            _TwoPassEncoding = false;
                        }
                        else
                        {
                            SrcVid.PreProcessPass = 1;
                            _TwoPassEncoding = true;
                            glbPassNumber = 1;
                        }
                    }
                    else if (intProContentType == 17)
                    {
                        if (_encodeInfo.TwoPass == false)
                        {
                            SrcAud.PreProcessPass = 0;
                            SrcVid.PreProcessPass = 0;
                            _TwoPassEncoding = false;
                        }
                        else
                        {
                            switch (intProVBRModeAudio)
                            {
                                case 1:
                                    SrcAud.PreProcessPass = 1;
                                    break;
                                case 2:
                                    SrcAud.PreProcessPass = 1;
                                    break;
                                case 3:
                                    SrcAud.PreProcessPass = 0;
                                    break;
                                case 4:
                                    SrcAud.PreProcessPass = 1;
                                    break;
                            }
                            switch (intProVBRModeVideo)
                            {
                                case 1:
                                    SrcVid.PreProcessPass = 1;
                                    break;
                                case 2:
                                    SrcVid.PreProcessPass = 1;
                                    break;
                                case 3:
                                    SrcVid.PreProcessPass = 0;
                                    break;
                                case 4:
                                    SrcVid.PreProcessPass = 1;
                                    break;
                            }
                            _TwoPassEncoding = true;
                            glbPassNumber = 1;

                        }

                    }
                    else
                    {
                        _backgroundWorker.ReportProgress(0, "BatchEncode does not support this type of profile");
                    }
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Passes - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);
                }
                IWMEncFile2 File = (IWMEncFile2)Encoder.File;
                try
                {
                    File.LocalFileName = _encodeInfo.Destination;
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Output File - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);
                }
                int intDurationAudio = 0;
                int intDurationVideo = 0;
                int intDurationFinal;
                try
                {
                    _backgroundWorker.ReportProgress(0, "Preparing to encode");
                    Encoder.PrepareToEncode(true);

                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Encoder Prepare - " + ex.Message.ToString();


                    throw new ApplicationException(glbstrErrLocation, ex);

                }
                try
                {
                    if (SrcAud != null)
                        intDurationAudio = System.Convert.ToInt32(SrcAud.Duration / 1000);
                }
                catch (Exception)
                {
                }
                try
                {
                    if (SrcVid != null)
                        intDurationVideo = System.Convert.ToInt32(SrcVid.Duration / 1000);
                }
                catch (Exception)
                {
                }
                if (intDurationAudio == 0)
                {
                    intDurationFinal = intDurationVideo;
                }
                else if (intDurationVideo == 0)
                {
                    intDurationFinal = intDurationAudio;
                }
                else
                {
                    if (intDurationVideo >= intDurationAudio)
                    {
                        intDurationFinal = intDurationVideo;
                    }
                    else
                    {
                        intDurationFinal = intDurationAudio;
                    }
                }
                glbintSourceDuration = intDurationFinal;
                try
                {
                    if (glbboolEncodingContinue == true)
                    {
                        Encoder.Start();
                        do
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                Encoder.Stop();
                                e.Cancel = true;
                                _ev.Set();
                            }

                            sReportPercentComplete(Encoder);
                        }
                        while (!_ev.WaitOne(1000));

                    }
                }
                catch (Exception ex)
                {
                    glbstrErrLocation = "Encoder Start - " + ex.Message.ToString();
                    throw new ApplicationException(glbstrErrLocation, ex);
                }
                if (e.Cancel)
                    return;
                else
                {
                    _backgroundWorker.ReportProgress(0, "Encoding Complete");
                    return;
                }
            }
            finally
            {

                if (_SrcGrpColl != null)
                {
                    try
                    {
                        Encoder.Stop();
                        _SrcGrpColl.Remove(0);

                    }
                    catch
                    {

                    }
                    Marshal.ReleaseComObject(_SrcGrpColl);
                    _SrcGrpColl = null;
                }
                if (Profile != null)
                {
                    Marshal.ReleaseComObject(Profile);
                    Profile = null;
                }
                if (Encoder != null)
                {
                    Encoder.OnStateChange -= new _IWMEncoderEvents_OnStateChangeEventHandler(this.Encoder_OnStateChange);
                    Marshal.ReleaseComObject(Encoder);
                    Encoder = null;
                }
                e.Result = DateTime.Now - time;
            }
        }
        // Completed Method
        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                StatusBar.Content = "Cancelled";
            }
            else if (e.Error != null)
            {
                StatusBar.Content = "Error: " + e.Error.Message;
            }
            else
            {
                StatusBar.Content = string.Format("Completed in {0:#,#.##} seconds", ((TimeSpan)e.Result).TotalSeconds);
            }
            btnConvert.IsEnabled = txtDest.Text.Length > 0 && txtSource.Text.Length > 0;
            btnConvertCtrl.IsEnabled = txtDestCtrl.Text.Length > 0 && txtSourceCtrl.Text.Length > 0;
            btnConvertCtrl.Content = btnConvert.Content = "Convert";

        }
        void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {

                StatusBar.Content = _backgroundWorker.CancellationPending ? "Cancelling..." : e.UserState as string;

            }
            catch (Exception)
            {

            }
        }
        BackgroundWorker _backgroundWorker = new BackgroundWorker();
        IWMEncSourceGroupCollection _SrcGrpColl = null;
        //see http://www.softpedia.com/progDownload/Windows-Media-Encoder-Download-1393.html
        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {


            _encodeInfo.Destination = txtDest.Text;
            _encodeInfo.Source = txtSource.Text;


            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
                this.btnConvert.IsEnabled = false;
                StatusBar.Content = "Cancelling...";

            }
            else
            {

                if (!_backgroundWorker.CancellationPending)
                {
                    StatusBar.Content = "Ready";
                    btnConvert.Content = "Cancel";
                    _backgroundWorker.RunWorkerAsync(this._encodeInfo);

                }

            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
            _backgroundWorker.WorkerReportsProgress = _backgroundWorker.WorkerSupportsCancellation = true;
        }

        ManualResetEvent _ev = new ManualResetEvent(false);
        private void Encoder_OnStateChange(WMEncoderLib.WMENC_ENCODER_STATE enumState)
        {
            string strRunState = "";
            switch (enumState)
            {
                case WMENC_ENCODER_STATE.WMENC_ENCODER_STARTING:

                    strRunState = "Encoder Starting";
                    _ev.Reset();

                    break;
                case WMENC_ENCODER_STATE.WMENC_ENCODER_RUNNING:
                    if (_TwoPassEncoding == true)
                    {
                        if (glbPassNumber == 1)
                        {
                            strRunState = "Encoder Running Pass 1 of 2 (No Preview)";
                        }
                        else
                        {
                            strRunState = "Encoder Running Pass 2 of 2";
                        }
                    }
                    else
                    {
                        strRunState = "Encoder Running Pass 1 of 1";
                    }
                    break;
                case WMENC_ENCODER_STATE.WMENC_ENCODER_END_PREPROCESS:
                    strRunState = "Encoder End Preprocess";
                    glbPassNumber = 2;
                    break;
                case WMENC_ENCODER_STATE.WMENC_ENCODER_PAUSING:
                    strRunState = "Encoder Pausing";
                    break;
                case WMENC_ENCODER_STATE.WMENC_ENCODER_PAUSED:
                    strRunState = "Encoder Paused";
                    break;
                case WMENC_ENCODER_STATE.WMENC_ENCODER_STOPPING:
                    strRunState = "Encoder Stopping";
                    break;
                case WMENC_ENCODER_STATE.WMENC_ENCODER_STOPPED:

                    strRunState = "Encoder Stopped";

                    _ev.Set();
                    break;
                default:
                    break;

            }
            _backgroundWorker.ReportProgress(0, strRunState);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            this.btnConvert.IsEnabled = txtDest.Text.Length > 0 && txtSource.Text.Length > 0;
        }
        private void btnControl_Click(object sender, RoutedEventArgs e)
        {
            WpfCustomFileDialog.OpenFileDialog<SelectControl> ofd = new WpfCustomFileDialog.OpenFileDialog<SelectControl>();
            ofd.Filter = "avi files (*.avi)|*.avi|wmv files (*.wmv)|*.wmv|All files (*.*)|*.*";
            ofd.Multiselect = false;
            ofd.Title = "Select Media file using a control";
            ofd.FileDlgStartLocation = AddonWindowLocation.Right;
            ofd.FileDlgDefaultViewMode = NativeMethods.FolderViewMode.Tiles;
            ofd.FileDlgOkCaption = "&Select";
            ofd.FileDlgEnableOkBtn = false;
            ofd.SetPlaces(new object[] { @"c:\", (int)Places.MyComputer, (int)Places.Favorites, (int)Places.All_Users_MyVideo, (int)Places.MyVideos });
            bool? res = ofd.ShowDialog(this);

            if (res.Value == true)
            {
                txtSourceCtrl.Text = ofd.FileName;
                _encodeInfo.Source = txtSourceCtrl.Text;
            }
        }
        private void btnTargetControl_Click(object sender, RoutedEventArgs e)
        {
            WpfCustomFileDialog.SaveFileDialog<TargetControl> sfd = new WpfCustomFileDialog.SaveFileDialog<TargetControl>();
            sfd.ValidateNames = true;
            sfd.FileDlgStartLocation = AddonWindowLocation.Bottom;
            sfd.Title = "Save as using a Control";
            sfd.Filter = "wmv files (*.wmv)|*.wmv|avi files (*.avi)|*.avi|wma files (*.wma)|*.wma|All files (*.*)|*.*";
            sfd.CheckPathExists = true;
            if (File.Exists(txtSourceCtrl.Text))
            {
                sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(this.txtSourceCtrl.Text) + "_converted";
            }
            sfd.SetPlaces(new object[] { @"c:\", (int)Places.MyComputer, (int)Places.Favorites, (int)Places.All_Users_MyVideo, (int)Places.MyVideos });
            bool? res = sfd.ShowDialog();
            if (res.Value == true)
            {
                _encodeInfo = (sfd.ChildWnd as TargetControl).EncodeInfo;
                _encodeInfo.Destination = this.txtDestCtrl.Text = sfd.FileName;

            }
        }

        private void Ctrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.btnConvertCtrl.IsEnabled = this.txtDestCtrl.Text.Length > 0 && txtSourceCtrl.Text.Length > 0;
        }


        private void btnConvertCtrl_Click(object sender, RoutedEventArgs e)
        {


            _encodeInfo.Destination = this.txtDestCtrl.Text;
            _encodeInfo.Source = this.txtSourceCtrl.Text;


            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
                this.btnConvertCtrl.IsEnabled = false;
                StatusBar.Content = "Cancelling...";

            }
            else
            {

                if (!_backgroundWorker.CancellationPending)
                {
                    StatusBar.Content = "Ready";
                    btnConvertCtrl.Content = "Cancel";
                    _backgroundWorker.RunWorkerAsync(this._encodeInfo);

                }

            }
        }

    }
}
