using System.ComponentModel;
namespace CustomControls
{
    partial class MySaveDialogControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MySaveDialogControl));
            this._pbChanged = new System.Windows.Forms.PictureBox();
            this._pbOriginal = new System.Windows.Forms.PictureBox();
            this._lblOriginalFileLen = new System.Windows.Forms.Label();
            this._lblOrigInfo = new System.Windows.Forms.Label();
            this._groupBox1 = new System.Windows.Forms.GroupBox();
            this._label4 = new System.Windows.Forms.Label();
            this._cbxNewViewMode = new System.Windows.Forms.ComboBox();
            this._ckbPreserveaspect = new System.Windows.Forms.CheckBox();
            this._horizontal = new System.Windows.Forms.NumericUpDown();
            this._label1 = new System.Windows.Forms.Label();
            this._rotateflip = new System.Windows.Forms.ComboBox();
            this._FileSize = new System.Windows.Forms.Label();
            this._vertical = new System.Windows.Forms.NumericUpDown();
            this._label2 = new System.Windows.Forms.Label();
            this._groupBox2 = new System.Windows.Forms.GroupBox();
            this._label5 = new System.Windows.Forms.Label();
            this._cbxOriginalMode = new System.Windows.Forms.ComboBox();
            this._rotateFlipOriginal = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._pbChanged)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbOriginal)).BeginInit();
            this._groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._horizontal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._vertical)).BeginInit();
            this._groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pbChanged
            // 
            this._pbChanged.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this._pbChanged.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pbChanged.Location = new System.Drawing.Point(6, 43);
            this._pbChanged.MaximumSize = new System.Drawing.Size(256, 256);
            this._pbChanged.MinimumSize = new System.Drawing.Size(1, 1);
            this._pbChanged.Name = "_pbChanged";
            this._pbChanged.Size = new System.Drawing.Size(256, 256);
            this._pbChanged.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pbChanged.TabIndex = 30;
            this._pbChanged.TabStop = false;
            this._pbChanged.WaitOnLoad = true;
            // 
            // _pbOriginal
            // 
            this._pbOriginal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this._pbOriginal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pbOriginal.Location = new System.Drawing.Point(6, 43);
            this._pbOriginal.MaximumSize = new System.Drawing.Size(256, 256);
            this._pbOriginal.MinimumSize = new System.Drawing.Size(1, 1);
            this._pbOriginal.Name = "_pbOriginal";
            this._pbOriginal.Size = new System.Drawing.Size(256, 256);
            this._pbOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pbOriginal.TabIndex = 30;
            this._pbOriginal.TabStop = false;
            this._pbOriginal.WaitOnLoad = true;
            // 
            // _lblOriginalFileLen
            // 
            this._lblOriginalFileLen.AutoSize = true;
            this._lblOriginalFileLen.Location = new System.Drawing.Point(6, 352);
            this._lblOriginalFileLen.Name = "_lblOriginalFileLen";
            this._lblOriginalFileLen.Size = new System.Drawing.Size(40, 13);
            this._lblOriginalFileLen.TabIndex = 1;
            this._lblOriginalFileLen.Text = "No File";
            // 
            // _lblOrigInfo
            // 
            this._lblOrigInfo.AutoSize = true;
            this._lblOrigInfo.Location = new System.Drawing.Point(6, 383);
            this._lblOrigInfo.Name = "_lblOrigInfo";
            this._lblOrigInfo.Size = new System.Drawing.Size(94, 13);
            this._lblOrigInfo.TabIndex = 2;
            this._lblOrigInfo.Text = "Original Image info";
            // 
            // _groupBox1
            // 
            this._groupBox1.Controls.Add(this._label4);
            this._groupBox1.Controls.Add(this._cbxNewViewMode);
            this._groupBox1.Controls.Add(this._ckbPreserveaspect);
            this._groupBox1.Controls.Add(this._pbChanged);
            this._groupBox1.Controls.Add(this._horizontal);
            this._groupBox1.Controls.Add(this._label1);
            this._groupBox1.Controls.Add(this._rotateflip);
            this._groupBox1.Controls.Add(this._FileSize);
            this._groupBox1.Controls.Add(this._vertical);
            this._groupBox1.Controls.Add(this._label2);
            this._groupBox1.Location = new System.Drawing.Point(278, 6);
            this._groupBox1.Name = "_groupBox1";
            this._groupBox1.Size = new System.Drawing.Size(268, 441);
            this._groupBox1.TabIndex = 1;
            this._groupBox1.TabStop = false;
            // 
            // _label4
            // 
            this._label4.AutoSize = true;
            this._label4.Location = new System.Drawing.Point(12, 22);
            this._label4.Name = "_label4";
            this._label4.Size = new System.Drawing.Size(57, 13);
            this._label4.TabIndex = 33;
            this._label4.Text = "ViewMode";
            // 
            // _cbxNewViewMode
            // 
            this._cbxNewViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbxNewViewMode.FormattingEnabled = true;
            this._cbxNewViewMode.Location = new System.Drawing.Point(97, 19);
            this._cbxNewViewMode.Name = "_cbxNewViewMode";
            this._cbxNewViewMode.Size = new System.Drawing.Size(149, 21);
            this._cbxNewViewMode.TabIndex = 32;
            this._cbxNewViewMode.SelectedIndexChanged += new System.EventHandler(this.cbxNewViewMode_SelectedIndexChanged);
            // 
            // _ckbPreserveaspect
            // 
            this._ckbPreserveaspect.AutoSize = true;
            this._ckbPreserveaspect.Location = new System.Drawing.Point(128, 309);
            this._ckbPreserveaspect.Name = "_ckbPreserveaspect";
            this._ckbPreserveaspect.Size = new System.Drawing.Size(104, 17);
            this._ckbPreserveaspect.TabIndex = 31;
            this._ckbPreserveaspect.Text = "Preserve Aspect";
            this._ckbPreserveaspect.UseVisualStyleBackColor = true;
            this._ckbPreserveaspect.CheckedChanged += new System.EventHandler(this.ckbPreserveaspect_CheckedChanged);
            // 
            // _horizontal
            // 
            this._horizontal.Location = new System.Drawing.Point(128, 337);
            this._horizontal.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._horizontal.Name = "_horizontal";
            this._horizontal.Size = new System.Drawing.Size(59, 20);
            this._horizontal.TabIndex = 4;
            this._horizontal.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this._horizontal.ValueChanged += new System.EventHandler(this._horizontal_ValueChanged);
            this._horizontal.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UPDOWN_MouseDown);
            this._horizontal.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UPDOWN_MouseUp);
            // 
            // _label1
            // 
            this._label1.AutoSize = true;
            this._label1.Location = new System.Drawing.Point(21, 344);
            this._label1.Name = "_label1";
            this._label1.Size = new System.Drawing.Size(84, 13);
            this._label1.TabIndex = 3;
            this._label1.Text = "horizontal pixels:";
            // 
            // _rotateflip
            // 
            this._rotateflip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._rotateflip.FormattingEnabled = true;
            this._rotateflip.Location = new System.Drawing.Point(0, 305);
            this._rotateflip.Name = "_rotateflip";
            this._rotateflip.Size = new System.Drawing.Size(121, 21);
            this._rotateflip.TabIndex = 2;
            this._rotateflip.SelectedIndexChanged += new System.EventHandler(this._rotateflip_SelectedIndexChanged);
            // 
            // _FileSize
            // 
            this._FileSize.AutoSize = true;
            this._FileSize.Location = new System.Drawing.Point(2, 398);
            this._FileSize.Name = "_FileSize";
            this._FileSize.Size = new System.Drawing.Size(33, 13);
            this._FileSize.TabIndex = 7;
            this._FileSize.Text = "None";
            // 
            // _vertical
            // 
            this._vertical.Location = new System.Drawing.Point(128, 363);
            this._vertical.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._vertical.Name = "_vertical";
            this._vertical.Size = new System.Drawing.Size(59, 20);
            this._vertical.TabIndex = 6;
            this._vertical.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this._vertical.ValueChanged += new System.EventHandler(this._vertical_ValueChanged);
            this._vertical.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UPDOWN_MouseDown);
            this._vertical.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UPDOWN_MouseUp);
            // 
            // _label2
            // 
            this._label2.AutoSize = true;
            this._label2.Location = new System.Drawing.Point(32, 370);
            this._label2.Name = "_label2";
            this._label2.Size = new System.Drawing.Size(73, 13);
            this._label2.TabIndex = 5;
            this._label2.Text = "vertical pixels:";
            // 
            // _groupBox2
            // 
            this._groupBox2.Controls.Add(this._label5);
            this._groupBox2.Controls.Add(this._cbxOriginalMode);
            this._groupBox2.Controls.Add(this._pbOriginal);
            this._groupBox2.Controls.Add(this._lblOriginalFileLen);
            this._groupBox2.Controls.Add(this._lblOrigInfo);
            this._groupBox2.Controls.Add(this._rotateFlipOriginal);
            this._groupBox2.Location = new System.Drawing.Point(3, 6);
            this._groupBox2.Name = "_groupBox2";
            this._groupBox2.Size = new System.Drawing.Size(269, 441);
            this._groupBox2.TabIndex = 0;
            this._groupBox2.TabStop = false;
            // 
            // _label5
            // 
            this._label5.AutoSize = true;
            this._label5.Location = new System.Drawing.Point(3, 19);
            this._label5.Name = "_label5";
            this._label5.Size = new System.Drawing.Size(57, 13);
            this._label5.TabIndex = 33;
            this._label5.Text = "ViewMode";
            // 
            // _cbxOriginalMode
            // 
            this._cbxOriginalMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbxOriginalMode.FormattingEnabled = true;
            this._cbxOriginalMode.Location = new System.Drawing.Point(88, 16);
            this._cbxOriginalMode.Name = "_cbxOriginalMode";
            this._cbxOriginalMode.Size = new System.Drawing.Size(149, 21);
            this._cbxOriginalMode.TabIndex = 32;
            this._cbxOriginalMode.SelectedIndexChanged += new System.EventHandler(this.cbxNewViewMode_SelectedIndexChanged);
            // 
            // _rotateFlipOriginal
            // 
            this._rotateFlipOriginal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._rotateFlipOriginal.FormattingEnabled = true;
            this._rotateFlipOriginal.Location = new System.Drawing.Point(6, 305);
            this._rotateFlipOriginal.Name = "_rotateFlipOriginal";
            this._rotateFlipOriginal.Size = new System.Drawing.Size(121, 21);
            this._rotateFlipOriginal.TabIndex = 2;
            this._rotateFlipOriginal.SelectedIndexChanged += new System.EventHandler(this._rotateflip_SelectedIndexChanged);
            // 
            // MySaveDialogControl
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this._groupBox2);
            this.Controls.Add(this._groupBox1);
            this.FileDlgCaption = "Save Thumbnail as:";
            this.FileDlgCheckFileExists = false;
            this.FileDlgDefaultViewMode = Win32Types.FolderViewMode.List;
            this.FileDlgFileName = "Change Picture";
            this.FileDlgFilter = resources.GetString("$this.FileDlgFilter");
            this.FileDlgOkCaption = "Create";
            this.FileDlgStartLocation = FileDialogExtenders.AddonWindowLocation.Bottom;
            this.FileDlgType = Win32Types.FileDialogType.SaveFileDlg;
            this.Name = "MySaveDialogControl";
            this.Size = new System.Drawing.Size(548, 446);
            this.Load += new System.EventHandler(this.MySaveDialogControl_Load);
            this.EventClosingDialog += new System.ComponentModel.CancelEventHandler(this.MySaveDialogControl_ClosingDialog);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.MySaveDialogControl_HelpRequested);
            this.EventFilterChanged += new FileDialogExtenders.FileDialogControlBase.FilterChangedEventHandler(this.MySaveDialogControl_FilterChanged);
            ((System.ComponentModel.ISupportInitialize)(this._pbChanged)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbOriginal)).EndInit();
            this._groupBox1.ResumeLayout(false);
            this._groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._horizontal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._vertical)).EndInit();
            this._groupBox2.ResumeLayout(false);
            this._groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _pbChanged;
        private System.Windows.Forms.PictureBox _pbOriginal;
        private System.Windows.Forms.Label _lblOriginalFileLen;
        private System.Windows.Forms.Label _lblOrigInfo;
        private System.Windows.Forms.GroupBox _groupBox1;
        private System.Windows.Forms.Label _FileSize;
        private System.Windows.Forms.Label _label2;
        private System.Windows.Forms.NumericUpDown _vertical;
        private System.Windows.Forms.Label _label1;
        private System.Windows.Forms.NumericUpDown _horizontal;
        private System.Windows.Forms.ComboBox _rotateflip;
        private System.Windows.Forms.GroupBox _groupBox2;
        private System.Windows.Forms.CheckBox _ckbPreserveaspect;
        private System.Windows.Forms.ComboBox _cbxNewViewMode;
        private System.Windows.Forms.Label _label4;
        private System.Windows.Forms.Label _label5;
        private System.Windows.Forms.ComboBox _cbxOriginalMode;
        private System.Windows.Forms.ComboBox _rotateFlipOriginal;
    }
}
