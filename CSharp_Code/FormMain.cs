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
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using AboutUtil;
using FileDialogExtenders;

namespace CustomControls
{

    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

        }

        protected override void OnLoad(EventArgs e)
        {
            lblFilePath.Text = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), @"profiles\All Users\Documents\My Pictures\Sample Pictures\winter.jpg");
            base.OnLoad(e);
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (sender.Equals(_btnSelect))
            {
                using (MyOpenFileDialogControl openDialog = new MyOpenFileDialogControl())
                {
                    if (openDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        lblFilePath.Text = openDialog.MSDialog.FileName;
                    }
                }

            }
            else if (sender.Equals(_btnSave))
            {
                using (MySaveDialogControl saveDialog = new MySaveDialogControl(lblFilePath.Text, this))
                {
                    if (saveDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        lblFilePath.Text = saveDialog.MSDialog.FileName;
                    }
                }
            }
            else if (sender.Equals(this._btnExtension))
            {
                using (MyOpenFileDialogControl openDialogCtrl = new MyOpenFileDialogControl())
                {
                    openDialogCtrl.FileDlgInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    OpenFileDialog openDialog = new OpenFileDialog();
                    openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    openDialog.AddExtension = true;
                    openDialog.Filter = "Image Files(*.bmp)|*.bmp |Image Files(*.JPG)|*.JPG|Image Files(*.jpeg)|*.jpeg|Image Files(*.GIF)|*.GIF|Image Files(*.emf)|*emf.|Image Files(*.ico)|*.ico|Image Files(*.png)|*.png|Image Files(*.tif)|*.tif|Image Files(*.wmf)|*.wmf|Image Files(*.exif)|*.exif";
                    openDialog.FilterIndex = 2;
                    openDialog.CheckFileExists = true;
                    openDialog.DefaultExt = "jpg";
                    openDialog.FileName = "Select Picture";
                    openDialog.DereferenceLinks = true;
                    //openDialog.ShowHelp = true;
                    if (Environment.OSVersion.Version.Major < 6)
                         openDialog.SetPlaces(new object[] { @"c:\", (int)Places.MyComputer, (int)Places.Favorites, (int)Places.Printers, (int)Places.Fonts });
                    if (openDialog.ShowDialog(openDialogCtrl, this) == DialogResult.OK)
                    {
                        lblFilePath.Text = openDialog.FileName;
                    }
                }
            }
            else if (sender.Equals(_btnSaveExt))
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    MySaveDialogControl saveDialogCtrl = new MySaveDialogControl(lblFilePath.Text, this);
                    saveDialogCtrl.FileDlgInitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    saveDialog.AddExtension = true;
                    saveDialog.Filter = "Image Files(*.bmp)|*.bmp |Image Files(*.JPG)|*.JPG|Image Files(*.jpeg)|*.jpeg|Image Files(*.GIF)|*.GIF|Image Files(*.emf)|*emf.|Image Files(*.ico)|*.ico|Image Files(*.png)|*.png|Image Files(*.tif)|*.tif|Image Files(*.wmf)|*.wmf|Image Files(*.exif)|*.exif";
                    saveDialog.FilterIndex = 2;
                    saveDialog.CheckFileExists = true;
                    saveDialog.DefaultExt = "jpg";
                    saveDialog.FileName = "Change Picture";
                    saveDialog.DereferenceLinks = true;
                    //saveDialog.ShowHelp = true;
                    if (Environment.OSVersion.Version.Major < 6)
                        saveDialog.SetPlaces(new object[] { (int)Places.Desktop, (int)Places.Printers, (int)Places.Favorites, (int)Places.Programs, (int)Places.Fonts, });
                    if (saveDialog.ShowDialog(saveDialogCtrl, this) == DialogResult.OK)
                    {
                        lblFilePath.Text = saveDialog.FileName;
                    }
                }
            }
            else if (sender.Equals(_btnExit))
                this.Close();

            System.GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void _btnAbout_Click(object sender, EventArgs e)
        {
            About dlg = new About(this);
            dlg.ShowDialog(this);
        }

    }
}