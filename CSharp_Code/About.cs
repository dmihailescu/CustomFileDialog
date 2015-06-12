/*
 * Please leave this Copyright notice in your code if you use it
 * Written by Decebal Mihailescu [http://www.codeproject.com/script/articles/list_articles.asp?userid=634640]
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace AboutUtil
{
    /// <summary>
    /// Summary description for About.
    /// </summary>
    public class About : System.Windows.Forms.Form
    {
        private System.Windows.Forms.LinkLabel _linkLabelAbout;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel _lnksecond;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public About(System.Windows.Forms.Form owner)
        {
            Owner = owner;
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this._linkLabelAbout = new System.Windows.Forms.LinkLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._lnksecond = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // _linkLabelAbout
            // 
            this._linkLabelAbout.LinkArea = new System.Windows.Forms.LinkArea(36, 0);
            this._linkLabelAbout.Location = new System.Drawing.Point(12, 233);
            this._linkLabelAbout.Name = "_linkLabelAbout";
            this._linkLabelAbout.Size = new System.Drawing.Size(249, 24);
            this._linkLabelAbout.TabIndex = 0;
            this._linkLabelAbout.Text = "Copyright © 2007-2012  Decebal Mihailescu";
            this._linkLabelAbout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._linkLabelAbout.UseCompatibleTextRendering = true;
            this._linkLabelAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAbout_LinkClicked);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(214, 269);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "&OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 57);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(477, 134);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "text from reflection";
            // 
            // _lnksecond
            // 
            this._lnksecond.LinkArea = new System.Windows.Forms.LinkArea(13, 9);
            this._lnksecond.Location = new System.Drawing.Point(12, 194);
            this._lnksecond.Name = "_lnksecond";
            this._lnksecond.Size = new System.Drawing.Size(249, 24);
            this._lnksecond.TabIndex = 0;
            this._lnksecond.TabStop = true;
            this._lnksecond.Text = "Copyright ©  CastorTiu 2006";
            this._lnksecond.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._lnksecond.UseCompatibleTextRendering = true;
            this._lnksecond.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAbout_LinkClicked);
            // 
            // About
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(501, 295);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._lnksecond);
            this.Controls.Add(this._linkLabelAbout);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowInTaskbar = false;
            this.Text = "About";
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void linkLabelAbout_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            System.Windows.Forms.LinkLabel llbl = sender as System.Windows.Forms.LinkLabel;
            if (llbl != null)
            {
                llbl.Links[llbl.Links.IndexOf(e.Link)].Visited = true;
                string target = e.Link.LinkData as string;
                if (target != null && target.Length > 0)
                    System.Diagnostics.Process.Start(target);
            }
        }

        private void About_Load(object sender, System.EventArgs e)
        {


            Assembly crt = Assembly.GetExecutingAssembly();
            object[] attr = crt.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
            AssemblyDescriptionAttribute desc = attr[0] as AssemblyDescriptionAttribute;
            attr = crt.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
            AssemblyFileVersionAttribute ver = attr[0] as AssemblyFileVersionAttribute;
            Version v = crt.GetName().Version;
            label1.Text = string.Format("{0}\nFile Version: {1}\nAssembly Version: {2}",
                desc.Description, ver.Version, v.ToString());
            attr = crt.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
            AssemblyTitleAttribute ta = attr[0] as AssemblyTitleAttribute;
            Text = string.Format("About  {0} {1}.{2}", ta.Title, v.Major, v.Minor); //Owner.Text     

            attr = crt.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            AssemblyCopyrightAttribute copyright = attr[0] as AssemblyCopyrightAttribute;
            string strcpy = copyright.Copyright;

            _linkLabelAbout.Text = strcpy;
            attr = crt.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            AssemblyCompanyAttribute company = attr[0] as AssemblyCompanyAttribute;
            // look for the name in copyright string to activate if found
            int start = strcpy.IndexOf(company.Company, 0, StringComparison.InvariantCultureIgnoreCase);
            if (start != -1)
            {
                this._linkLabelAbout.LinkArea = new System.Windows.Forms.LinkArea(start, company.Company.Length);
            }
            else
            {// company not found in copyright string, so check for Copyright ©
                int size = "Copyright © ".Length;
                start = strcpy.IndexOf("Copyright ©", 0, StringComparison.InvariantCultureIgnoreCase);
                if (start != -1)
                {
                    this._linkLabelAbout.LinkArea = new System.Windows.Forms.LinkArea(size, strcpy.Length - size);
                }
                else
                {// no Copyright © string, activate the whole area
                    this._linkLabelAbout.LinkArea = new System.Windows.Forms.LinkArea(0, strcpy.Length);
                }
            }
            if (_linkLabelAbout.LinkArea != null && !_linkLabelAbout.LinkArea.IsEmpty)
                this._linkLabelAbout.Links[0].LinkData = @"www.codeproject.com/script/Articles/list_articles.asp?userid=634640";

            _lnksecond.Links[0].LinkData = @"http://www.codeproject.com/script/Articles/list_articles.asp?userid=240897";

        }
    }
}
