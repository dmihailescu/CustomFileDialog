
namespace CustomControls
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this._btnSelect = new System.Windows.Forms.Button();
            this._FileSize = new System.Windows.Forms.Label();
            this._btnSave = new System.Windows.Forms.Button();
            this.lblFilePath = new System.Windows.Forms.Label();
            this._btnAbout = new System.Windows.Forms.Button();
            this._btnExtension = new System.Windows.Forms.Button();
            this._btnSaveExt = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._btnExit = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _btnSelect
            // 
            this._btnSelect.Location = new System.Drawing.Point(68, 17);
            this._btnSelect.Name = "_btnSelect";
            this._btnSelect.Size = new System.Drawing.Size(75, 23);
            this._btnSelect.TabIndex = 24;
            this._btnSelect.Text = "Se&lect";
            this._btnSelect.UseVisualStyleBackColor = true;
            this._btnSelect.Click += new System.EventHandler(this.button1_Click);
            // 
            // _FileSize
            // 
            this._FileSize.AutoSize = true;
            this._FileSize.Location = new System.Drawing.Point(138, 446);
            this._FileSize.Name = "_FileSize";
            this._FileSize.Size = new System.Drawing.Size(16, 13);
            this._FileSize.TabIndex = 31;
            this._FileSize.Text = "   ";
            // 
            // _btnSave
            // 
            this._btnSave.Location = new System.Drawing.Point(33, 9);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(75, 23);
            this._btnSave.TabIndex = 24;
            this._btnSave.Text = "&Save";
            this._btnSave.UseVisualStyleBackColor = true;
            this._btnSave.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Location = new System.Drawing.Point(12, 121);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(50, 13);
            this.lblFilePath.TabIndex = 34;
            this.lblFilePath.Text = "File path:";
            // 
            // _btnAbout
            // 
            this._btnAbout.Location = new System.Drawing.Point(311, 151);
            this._btnAbout.Name = "_btnAbout";
            this._btnAbout.Size = new System.Drawing.Size(75, 23);
            this._btnAbout.TabIndex = 35;
            this._btnAbout.Text = "&About";
            this._btnAbout.UseVisualStyleBackColor = true;
            this._btnAbout.Click += new System.EventHandler(this._btnAbout_Click);
            // 
            // _btnExtension
            // 
            this._btnExtension.Location = new System.Drawing.Point(68, 60);
            this._btnExtension.Name = "_btnExtension";
            this._btnExtension.Size = new System.Drawing.Size(167, 23);
            this._btnExtension.TabIndex = 24;
            this._btnExtension.Text = "Select (&Extension method)";
            this._btnExtension.UseVisualStyleBackColor = true;
            this._btnExtension.Click += new System.EventHandler(this.button1_Click);
            // 
            // _btnSaveExt
            // 
            this._btnSaveExt.Location = new System.Drawing.Point(33, 52);
            this._btnSaveExt.Name = "_btnSaveExt";
            this._btnSaveExt.Size = new System.Drawing.Size(152, 23);
            this._btnSaveExt.TabIndex = 24;
            this._btnSaveExt.Text = "S&ave(extension Method)";
            this._btnSaveExt.UseVisualStyleBackColor = true;
            this._btnSaveExt.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(35, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._btnSave);
            this.groupBox2.Controls.Add(this._btnSaveExt);
            this.groupBox2.Location = new System.Drawing.Point(278, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            // 
            // _btnExit
            // 
            this._btnExit.Location = new System.Drawing.Point(68, 151);
            this._btnExit.Name = "_btnExit";
            this._btnExit.Size = new System.Drawing.Size(75, 23);
            this._btnExit.TabIndex = 38;
            this._btnExit.Text = "E&xit";
            this._btnExit.UseVisualStyleBackColor = true;
            this._btnExit.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 186);
            this.Controls.Add(this._btnExit);
            this.Controls.Add(this._btnAbout);
            this.Controls.Add(this.lblFilePath);
            this.Controls.Add(this._FileSize);
            this.Controls.Add(this._btnExtension);
            this.Controls.Add(this._btnSelect);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Click To Open a Dialog";
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnSelect;
        private System.Windows.Forms.Label _FileSize;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Button _btnAbout;
        private System.Windows.Forms.Button _btnExtension;
        private System.Windows.Forms.Button _btnSaveExt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button _btnExit;


    }
}

