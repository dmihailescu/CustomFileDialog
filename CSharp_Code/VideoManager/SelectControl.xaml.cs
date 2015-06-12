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
using System.Windows.Shapes;
using System.Windows.Interop;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using WpfCustomFileDialog;

namespace VideoManager
{
    /// <summary>
    /// Interaction logic for SelectControl.xaml
    /// </summary>
    public partial class SelectControl : ControlAddOnBase
    {
        public SelectControl()
        {
            InitializeComponent();
        }
// comes mostly from http://msdn.microsoft.com/en-us/magazine/cc163455.aspx
        void mediaPlay(Object sender, EventArgs e)
        {
            if (_isPaused)
            {
                _Media.SpeedRatio = _speed;
                _Media.Play();
                _isPaused = false;
                _playButton.Content = "Pause";
                this._comboSpeed.IsEnabled = true;
                if (!_timer.IsEnabled)
                    _timer.Start();
            }
            else
            {
                _Media.Pause();
                _isPaused = true;
                _playButton.Content = "Play";
            }

        }
        bool _isPaused;


        double _volume;
        void mediaMute(Object sender, EventArgs e)
        {
            if (_Media.Volume > 0)
            {
                _volume = _Media.Volume;
                _Media.Volume = 0;
                _muteButt.Content = "Listen";
            }
            else
            {
                _Media.Volume = _volume;
                _muteButt.Content = "Mute";
            }
        }
        internal IntPtr _hFileDialogWrapperHandle = IntPtr.Zero;


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            TimeSlider.Value = 0;
            TimeSlider.Maximum = 100;
            TimeSlider.Minimum = 0;
            TimeSlider.TickFrequency = 1;
            TimeSlider.Delay = 500;
            TimeSlider.IsEnabled = false;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            ParentDlg.EventFileNameChanged += new PathChangedEventHandler(ParentDlg_EventFileNameChanged);
            ParentDlg.EventFolderNameChanged += new PathChangedEventHandler(ParentDlg_EventFolderNameChanged);
            ParentDlg.EventFilterChanged += new FilterChangedEventHandler(ParentDlg_EventFilterChanged);
            _comboSpeed.IsEnabled = false;
        }



        void ParentDlg_EventFilterChanged(IFileDlgExt sender, int index)
        {

        }

        void ParentDlg_EventFolderNameChanged(IFileDlgExt sender, string filePath)
        {
            _Media.Stop();
            _Media.Source = null;
            sender.FileDlgEnableOkBtn = false;
            _fsize.Content = string.Empty;
            _size.Content = string.Empty;
            _duration.Content = string.Empty;
            TimeSlider.IsEnabled = false;
            _playButton.IsEnabled = false;
            _muteButt.IsEnabled = false;
        }
        string _filePath;
        void ParentDlg_EventFileNameChanged(IFileDlgExt sender, string filePath)
        {
            _Media.Stop();
            _Media.SpeedRatio = 1;
            _comboSpeed.SelectedIndex = 1;
            _comboSpeed.IsEnabled = true;
            if (!string.IsNullOrEmpty(System.IO.Path.GetFileName(filePath)))
            {
                sender.FileDlgEnableOkBtn = true;
                using (System.IO.FileStream file = System.IO.File.OpenRead(filePath))
                {
                    _fsize.Content = string.Format("{0:#,#} bytes", file.Length);
                    if (file.Length > 0)
                        _filePath = filePath;
                }
                _Media.Source = new Uri(_filePath);
                _playButton.IsEnabled = false;
                _muteButt.IsEnabled = false;
                _Media.Volume = 50;
                _Media.SpeedRatio = _speed;
                _Media.Play();
                _playButton.Content = "Pause";
                _isPaused = false;
                _muteButt.IsEnabled = false;
            }
            else
            {
                sender.FileDlgEnableOkBtn = false;
                _fsize.Content = string.Empty;
                _size.Content = string.Empty;
                _duration.Content = string.Empty;
                _Media.Source = null;
                _playButton.Content = "Play";
                _playButton.IsEnabled = false;
                _muteButt.IsEnabled = false;
            }
            TimeSlider.IsEnabled = false;
            _muteButt.Content = "Mute";
            _size.Content = string.Empty;
            
            this._duration.Content = string.Empty;

        }

        private void _Media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            COMException ex = e.ErrorException as COMException;
            if (ex != null && (uint)ex.ErrorCode == 0x8898050c || 	(uint)ex.ErrorCode ==0xc00d11b1 )
            {


            }
            else if (e.ErrorException is System.IO.FileNotFoundException)
            {
                TimeSlider.IsEnabled = false;
                this._playButton.IsEnabled = false;
                this._muteButt.IsEnabled = false;
            }
            _comboSpeed.IsEnabled = false;
            _comboSpeed.SelectedIndex = 1;
            _Media.SpeedRatio = 1;
        }

        private void _Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            Duration dur = _Media.NaturalDuration;
            if (dur.HasTimeSpan)
            {
                _duration.Content = string.Format("{0}:{1}:{2}",dur.TimeSpan.Hours,dur.TimeSpan.Minutes,dur.TimeSpan.Seconds);
                int height = _Media.NaturalVideoHeight;
                int width = _Media.NaturalVideoWidth;
                _size.Content = string.Format("{0} X {1}", width, height);
                if (!_timer.IsEnabled)
                    _timer.Start();
                TimeSlider.IsEnabled = true;
                _comboSpeed.IsEnabled = true;
                
                this._playButton.IsEnabled = true;
                this._muteButt.IsEnabled = true;
            }
            else
            {
                TimeSlider.IsEnabled = false;
                _size.Content = string.Empty;
                _duration.Content = string.Empty;
                this._playButton.IsEnabled = false;
                this._muteButt.IsEnabled = false;
                _comboSpeed.IsEnabled = false;
            }
            _comboSpeed.SelectedIndex = 1;
        }

        private void _Media_Loaded(object sender, RoutedEventArgs e)
        {
            // Create a Timer with a Normal Priority
            _timer = new DispatcherTimer(DispatcherPriority.Normal, this.Dispatcher);


            // Set the Interval to 1 second
            _timer.Interval = TimeSpan.FromMilliseconds(1000);

            // Set the callback to just show the time ticking away
            // NOTE: We are using a control so this has to run on 
            // the UI thread
            _timer.Tick += new EventHandler(delegate(object s, EventArgs a)
            {
                if (!_mouseDown)
                {
                    try
                    {
                        Duration dur = _Media.NaturalDuration;
                        if (dur.HasTimeSpan)
                        {
                            long val = 100 * _Media.Position.Ticks / _Media.NaturalDuration.TimeSpan.Ticks;
                            TimeSlider.Value = val;
                        }
                    }
                    catch (Exception)
                    {

                    }

                }

            });
        }



        private void _Media_Unloaded(object sender, RoutedEventArgs e)
        {
            _Media.Source = null;
            _timer.Stop();
            _timer = null;
        }

        private void _Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            _Media.Stop();
            _Media.Position = new TimeSpan(0L);
            TimeSlider.Value = 0;
            _timer.Stop();
            _playButton.Content = "Play";
            _isPaused = true;
        }

        double _speed = 1;
        private void comboSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                string str = (sender as ComboBox).SelectedValue.ToString();
                str = str.Remove(0, str.LastIndexOf(':') + 1);
                _speed = double.Parse(str);
                if (_Media.SpeedRatio != _speed)
                {
                    _Media.SpeedRatio = _speed;
                }
            }
            catch (Exception)
            {


            }
        }
        DispatcherTimer _timer;
        double _sliderVal;
        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _sliderVal = e.NewValue;
        }



        private void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = false;
            try
            {
                _Media.Position = new TimeSpan(Convert.ToInt64(_Media.NaturalDuration.TimeSpan.Ticks * _sliderVal / 100));
                if (!_isPaused)
                    _Media.Play();
            }
            catch (Exception)
            {

            }
        }

        bool _mouseDown;
        private void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;
            if (!_isPaused)
                _Media.Stop();
        }

    }
}
