//  Copyright (c) 2006, Gustavo Franco
//  Copyright © Decebal Mihailescu 2007-2015

//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using FileDialogExtenders;


namespace CustomControls
{
    public partial class MySaveDialogControl : FileDialogControlBase
    {
        Bitmap _changedImage;
        internal Image _originalImage;
        RotateFlipType _rft;
        bool _canDisplay = true;
        ImageFormat _format = ImageFormat.Jpeg;
        System.IO.MemoryStream _memstream;
        string _imageFile;
        public MySaveDialogControl()
        {
            InitializeComponent();
        }

        protected override void OnPrepareMSDialog()
        {
            base.OnPrepareMSDialog();
            MSDialog.FilterIndex = GetIndexfromFile(_imageFile);
            if (Environment.OSVersion.Version.Major < 6)
                MSDialog.SetPlaces(new object[] { (int)Places.Desktop, (int)Places.Printers, (int)Places.Favorites, (int)Places.Programs, (int)Places.Fonts, });
            FileDlgInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        public MySaveDialogControl(string originalfile, IWin32Window parent)
        {
            try
            {
                _imageFile = originalfile;
                _originalImage = Image.FromFile(originalfile);                
            }
            catch 
            {
                _imageFile = null;
            }

            if (_imageFile == null || _imageFile.Length == 0)
            {

                MyOpenFileDialogControl openDialog = new MyOpenFileDialogControl();
                try
                {
                    if (parent == null)
                        parent = Application.OpenForms[0];
                    if (openDialog.ShowDialog(parent) == DialogResult.OK)
                    {
                        _imageFile = openDialog.MSDialog.FileName;
                        _originalImage =  Image.FromFile(_imageFile);
                        InitializeComponent();
                    }
                    else
                    {
                        Dispose();
                    }
                }
                finally
                {
                    if (openDialog != null)
                        openDialog.Dispose();
                    openDialog = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            else
            {
                
                InitializeComponent();
            }
            
        }
        private void MySaveDialogControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                FileInfo fi = new FileInfo(_imageFile);
                _lblOriginalFileLen.Text = string.Format("Original File Size: {0} bytes", fi.Length);
            }
            _horizontal.Minimum = _pbChanged.MinimumSize.Width;
            _horizontal.Maximum = _pbChanged.MaximumSize.Width/2;   
            _vertical.Minimum = _pbChanged.MinimumSize.Height;
            _vertical.Maximum = _pbChanged.MaximumSize.Height/2; 
            _horizontal.Value = _pbChanged.MaximumSize.Width/2;
            _vertical.Value = _pbChanged.MaximumSize.Height/2;         
            _groupBox2.Text = System.IO.Path.GetFileName(_imageFile);
            _groupBox1.Text = "New Image";
            _rotateflip.DataSource = Enum.GetNames(typeof(RotateFlipType));
            _rotateFlipOriginal.DataSource = Enum.GetNames(typeof(RotateFlipType));
            _cbxOriginalMode.DataSource = Enum.GetNames(typeof(System.Windows.Forms.PictureBoxSizeMode));
            _cbxNewViewMode.DataSource = Enum.GetNames(typeof(System.Windows.Forms.PictureBoxSizeMode));
            _cbxNewViewMode.SelectedIndex = (int)PictureBoxSizeMode.Zoom;
            _cbxOriginalMode.SelectedIndex = (int)PictureBoxSizeMode.Zoom;
            _pbChanged.SizeMode = PictureBoxSizeMode.Zoom;
            _pbOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            _pbChanged.MaximumSize = _pbChanged.Size;
            _pbOriginal.MaximumSize = _pbOriginal.Size;
            if (_originalImage != null && !DesignMode)
            {
                _pbOriginal.Image = _originalImage;
                _format = _originalImage.RawFormat;
                _lblOrigInfo.Text = string.Format("Colors: {0}\nSize: {1} X {2} pixels\nHorizontal Resolution {3}\nVertical resolution: {4}",
                    MyOpenFileDialogControl.GetColorsCountFromImage(_pbOriginal.Image),_originalImage.Width,
                    _originalImage.Height,_originalImage.HorizontalResolution, _originalImage.VerticalResolution);
                Display();
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (_memstream != null)
            {
                _memstream.Dispose();
                _memstream = null;
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void _rotateflip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_changedImage == null)
                return;
            if (sender == _rotateflip)
            {
                _rft = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), _rotateflip.SelectedItem.ToString());
                Display();
            }
            else
            {
                _pbOriginal.Image.RotateFlip((RotateFlipType)Enum.Parse(typeof(RotateFlipType), _rotateFlipOriginal.SelectedItem.ToString()));
                _pbOriginal.Refresh();
            }


        }

        private void _horizontal_ValueChanged(object sender, EventArgs e)
        {
            if (_originalImage == null || !_canDisplay)
                return;
            if (_ckbPreserveaspect.Checked)
                _vertical.Value = _horizontal.Value;
        }


        private void _vertical_ValueChanged(object sender, EventArgs e)
        {
            if (_originalImage == null || !_canDisplay)
                return;
        }

        private void _Update_Click(object sender, EventArgs e)
        {
            Display();
        }

        bool Display()
        {
            try
            {
                if (_originalImage == null && _canDisplay)
                    return false;
                double aspectfactor = (double)_originalImage.Height / _originalImage.Width;
                int verticalVal =  (int)this._vertical.Value ;
                int horizontalVal = (int)this._horizontal.Value ;
                if (aspectfactor > 1)
                {//portrait
                    horizontalVal = (int)(horizontalVal / aspectfactor);
                }
                else
                {//landscape
                    verticalVal = (int)(verticalVal * aspectfactor);
                }
                Bitmap newImg = _originalImage.GetThumbnailImage(horizontalVal, verticalVal, null, IntPtr.Zero) as Bitmap;
                if (_changedImage != null)
                    _changedImage.Dispose();
                newImg.RotateFlip(_rft);
                _pbChanged.SuspendLayout();
                _pbOriginal.SuspendLayout();
                if (_cbxNewViewMode.SelectedItem == null)
                    _pbChanged.SizeMode = PictureBoxSizeMode.AutoSize;
                else
                    _pbChanged.SizeMode = (PictureBoxSizeMode)Enum.Parse(typeof(PictureBoxSizeMode), _cbxNewViewMode.SelectedItem.ToString());
                if (_cbxOriginalMode.SelectedItem == null)
                    _pbOriginal.SizeMode = PictureBoxSizeMode.AutoSize;
                else
                    _pbOriginal.SizeMode = (PictureBoxSizeMode)Enum.Parse(typeof(PictureBoxSizeMode), _cbxOriginalMode.SelectedItem.ToString());
                _pbChanged.Image = _changedImage = newImg;
                _pbChanged.Size = _pbChanged.MaximumSize;
                _pbChanged.ResumeLayout(true);
                _pbOriginal.ResumeLayout(true);
                return ShowImageInfo();
            }
            catch
            {
                return false;
            }
        }

        private bool ShowImageInfo()
        {
            try
            {
                if (_memstream != null)
                {
                    _memstream.Dispose();
                    _memstream = null;
                }
                _memstream = new MemoryStream();
                _changedImage.Save(_memstream, _format);
                //correct the format bug
                ImageFormat ifrm = default(ImageFormat);
                if (_format.Equals(ImageFormat.Bmp))
                    ifrm = ImageFormat.Bmp;
                else if (_format.Equals(ImageFormat.Gif))
                    ifrm = ImageFormat.Gif;
                else if (_format.Equals(ImageFormat.Jpeg))
                    ifrm = ImageFormat.Jpeg;
                else if (_format.Equals(ImageFormat.Png))
                    ifrm = ImageFormat.Png;
                else if (_format.Equals(ImageFormat.Tiff))
                    ifrm = ImageFormat.Tiff;
                else if (_format.Equals(ImageFormat.Emf))
                    ifrm = ImageFormat.Emf;
                else if (_format.Equals(ImageFormat.Icon))
                    ifrm = ImageFormat.Icon;
                else if (_format.Equals(ImageFormat.Wmf))
                    ifrm = ImageFormat.Wmf;
                else if (_format.Equals(ImageFormat.Exif))
                    ifrm = ImageFormat.Exif;


                _FileSize.Text = string.Format("Picture Size: {0} bytes for {3}\nWidth = {1} Height ={2} pixels",
                    _memstream.Length, (int)_horizontal.Value, (int)_vertical.Value, ifrm.ToString());
                return true;
            }
            catch 
            {
                throw;
            }
        }

        private void UPDOWN_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is UpDownBase)
                _canDisplay = false;
            if (_ckbPreserveaspect.Checked)
                _vertical.Value = _horizontal.Value;

        }

        private void UPDOWN_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is UpDownBase)
            if (_ckbPreserveaspect.Checked)
                _vertical.Value = _horizontal.Value;
                _canDisplay = true;
            Display();
        }

        private void MySaveDialogControl_FilterChanged(IWin32Window sender, int index)
        {
            FileDlgEnableOkBtn = GetFormatFromIndex(index);
        }

        private bool GetFormatFromIndex(int index)
        {
            string ext = string.Empty;
            ImageFormat oldform = _format;
            try
            {
                string[] extensions = MSDialog.Filter.Split('|');
                index--;
                ext = extensions[2 * index + 1];
                int pos = ext.LastIndexOf('.');
                ext = ext.Substring(pos + 1).Trim().ToLower();
                switch (ext)
                {
                    case "jpg":
                    case "jpeg":
                        _format = ImageFormat.Jpeg;
                        break;
                    case "bmp":
                        _format = ImageFormat.Bmp;
                        break;
                    case "gif":
                        _format = ImageFormat.Gif;
                        break;
                    case "emf":
                        _format = ImageFormat.Emf;
                        break;
                    case "ico":
                        _format = ImageFormat.Icon;
                        break;
                    case "png":
                        _format = ImageFormat.Png;
                        break;
                    case "tif":
                        _format = ImageFormat.Tiff;
                        break;
                    case "wmf":
                        _format = ImageFormat.Wmf;
                        break;
                    case "exif":
                        _format = ImageFormat.Exif;
                        break;
                    default:
                        throw new ApplicationException(ext + " is an unknown format.Select a known one.");
                }
                return ShowImageInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Conversion error for ." +ext+ " type",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                //revert to last good format
                MSDialog.FilterIndex = GetIndexfromFile("dummyfile." + oldform.ToString());
                _format = oldform;
                return false;
            }         
        }

       private int GetIndexfromFile(string file)
        {

            try
            {
            string ext = System.IO.Path.GetExtension(file).ToLower();
            int index = MSDialog.Filter.ToLower().IndexOf(ext);
            string[] extensions = MSDialog.Filter.Substring(0,index).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return extensions.Length / 2 + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Unknown Format!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }         
        }
        private void MySaveDialogControl_ClosingDialog(object sender, CancelEventArgs e)
        {
            if (_memstream == null)
                return;
            System.IO.FileStream br = null;
            try
            {

                    br = new FileStream(MSDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    _memstream.WriteTo(br);
                    br.Flush();
                    br.Close();
                    e.Cancel = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, "Unable to save the file because:\n" 
                    + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                e.Cancel = true;
            }
            finally
            {
                if(br != null)
                    br.Dispose();
                _memstream.Dispose();
                _memstream = null;
            }

        }


        private void ckbOrigStretch_CheckedChanged(object sender, EventArgs e)
        {
            Display();
        }

        private void ckbPreserveaspect_CheckedChanged(object sender, EventArgs e)
        {
            _vertical.Enabled = !_ckbPreserveaspect.Checked;
            if (_ckbPreserveaspect.Checked)
                _vertical.Value = _horizontal.Value;
        }

        private void cbxNewViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(sender == _cbxNewViewMode)
                _pbChanged.SizeMode = (PictureBoxSizeMode)Enum.Parse(typeof(PictureBoxSizeMode), _cbxNewViewMode.SelectedItem.ToString());
            else
                _pbOriginal.SizeMode = (PictureBoxSizeMode)Enum.Parse(typeof(PictureBoxSizeMode), _cbxOriginalMode.SelectedItem.ToString());
            Display();
        }

        private void MySaveDialogControl_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MessageBox.Show("Please add some specific help here");
        }

    }
}

