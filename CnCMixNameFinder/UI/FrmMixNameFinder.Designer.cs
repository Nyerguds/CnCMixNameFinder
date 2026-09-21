namespace CnCMixNameFinder.UI
{
    partial class FrmMixNameFinder
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
            this.txtStart = new System.Windows.Forms.TextBox();
            this.lblStart = new System.Windows.Forms.Label();
            this.txtEnd = new System.Windows.Forms.TextBox();
            this.lblEnd = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.lblId = new System.Windows.Forms.Label();
            this.txtExtension = new System.Windows.Forms.TextBox();
            this.lblExtension = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.nmrMaxLength = new System.Windows.Forms.NumericUpDown();
            this.lblMaxLength = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnAbort = new System.Windows.Forms.Button();
            this.nmrMinLength = new System.Windows.Forms.NumericUpDown();
            this.lblMinLength = new System.Windows.Forms.Label();
            this.chkFindAllMatches = new System.Windows.Forms.CheckBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.lblHashMethod = new System.Windows.Forms.Label();
            this.cmbHashMethod = new System.Windows.Forms.ComboBox();
            this.txtChars = new System.Windows.Forms.TextBox();
            this.lblChars = new System.Windows.Forms.Label();
            this.btnQuickChars = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nmrMaxLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrMinLength)).BeginInit();
            this.SuspendLayout();
            // 
            // txtStart
            // 
            this.txtStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStart.Location = new System.Drawing.Point(112, 62);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(300, 20);
            this.txtStart.TabIndex = 6;
            this.txtStart.TextChanged += new System.EventHandler(this.TextBoxUppercase);
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(12, 65);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(70, 13);
            this.lblStart.TabIndex = 5;
            this.lblStart.Text = "Start of name";
            // 
            // txtEnd
            // 
            this.txtEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEnd.Location = new System.Drawing.Point(112, 88);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(300, 20);
            this.txtEnd.TabIndex = 8;
            this.txtEnd.TextChanged += new System.EventHandler(this.TextBoxUppercase);
            // 
            // lblEnd
            // 
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(12, 91);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(67, 13);
            this.lblEnd.TabIndex = 7;
            this.lblEnd.Text = "End of name";
            // 
            // txtId
            // 
            this.txtId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtId.Location = new System.Drawing.Point(112, 38);
            this.txtId.MaxLength = 8;
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(300, 20);
            this.txtId.TabIndex = 4;
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(12, 41);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(49, 13);
            this.lblId.TabIndex = 3;
            this.lblId.Text = "Name ID";
            // 
            // txtExtension
            // 
            this.txtExtension.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExtension.Location = new System.Drawing.Point(112, 114);
            this.txtExtension.Name = "txtExtension";
            this.txtExtension.Size = new System.Drawing.Size(300, 20);
            this.txtExtension.TabIndex = 10;
            this.txtExtension.TextChanged += new System.EventHandler(this.TextBoxUppercase);
            // 
            // lblExtension
            // 
            this.lblExtension.AutoSize = true;
            this.lblExtension.Location = new System.Drawing.Point(12, 117);
            this.lblExtension.Name = "lblExtension";
            this.lblExtension.Size = new System.Drawing.Size(92, 13);
            this.lblExtension.TabIndex = 9;
            this.lblExtension.Text = "Extension (no dot)";
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Location = new System.Drawing.Point(12, 299);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Size = new System.Drawing.Size(410, 166);
            this.txtResult.TabIndex = 32;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(13, 283);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(45, 13);
            this.lblResult.TabIndex = 31;
            this.lblResult.Text = "Results:";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(16, 240);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(80, 23);
            this.btnGenerate.TabIndex = 20;
            this.btnGenerate.Text = "Generate!";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.BtnGenerate_Click);
            // 
            // nmrMaxLength
            // 
            this.nmrMaxLength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nmrMaxLength.Location = new System.Drawing.Point(112, 166);
            this.nmrMaxLength.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nmrMaxLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrMaxLength.Name = "nmrMaxLength";
            this.nmrMaxLength.Size = new System.Drawing.Size(300, 20);
            this.nmrMaxLength.TabIndex = 14;
            this.nmrMaxLength.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // lblMaxLength
            // 
            this.lblMaxLength.AutoSize = true;
            this.lblMaxLength.Location = new System.Drawing.Point(12, 168);
            this.lblMaxLength.Name = "lblMaxLength";
            this.lblMaxLength.Size = new System.Drawing.Size(83, 13);
            this.lblMaxLength.TabIndex = 13;
            this.lblMaxLength.Text = "Maximum length";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStatus.Location = new System.Drawing.Point(13, 266);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(83, 13);
            this.lblStatus.TabIndex = 30;
            this.lblStatus.Text = "STATUSLABEL";
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(0, 471);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(434, 20);
            this.txtStatus.TabIndex = 30;
            // 
            // btnAbort
            // 
            this.btnAbort.Enabled = false;
            this.btnAbort.Location = new System.Drawing.Point(188, 240);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(80, 23);
            this.btnAbort.TabIndex = 22;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.BtnAbort_Click);
            // 
            // nmrMinLength
            // 
            this.nmrMinLength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nmrMinLength.Location = new System.Drawing.Point(112, 140);
            this.nmrMinLength.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nmrMinLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrMinLength.Name = "nmrMinLength";
            this.nmrMinLength.Size = new System.Drawing.Size(300, 20);
            this.nmrMinLength.TabIndex = 12;
            this.nmrMinLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblMinLength
            // 
            this.lblMinLength.AutoSize = true;
            this.lblMinLength.Location = new System.Drawing.Point(12, 142);
            this.lblMinLength.Name = "lblMinLength";
            this.lblMinLength.Size = new System.Drawing.Size(80, 13);
            this.lblMinLength.TabIndex = 11;
            this.lblMinLength.Text = "Minimum length";
            // 
            // chkFindAllMatches
            // 
            this.chkFindAllMatches.AutoSize = true;
            this.chkFindAllMatches.Checked = true;
            this.chkFindAllMatches.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFindAllMatches.Location = new System.Drawing.Point(112, 217);
            this.chkFindAllMatches.Name = "chkFindAllMatches";
            this.chkFindAllMatches.Size = new System.Drawing.Size(102, 17);
            this.chkFindAllMatches.TabIndex = 18;
            this.chkFindAllMatches.Text = "Find all matches";
            this.chkFindAllMatches.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(102, 240);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(80, 23);
            this.btnPause.TabIndex = 21;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.BtnPause_Click);
            // 
            // lblHashMethod
            // 
            this.lblHashMethod.AutoSize = true;
            this.lblHashMethod.Location = new System.Drawing.Point(12, 15);
            this.lblHashMethod.Name = "lblHashMethod";
            this.lblHashMethod.Size = new System.Drawing.Size(85, 13);
            this.lblHashMethod.TabIndex = 1;
            this.lblHashMethod.Text = "Hashing Method";
            this.lblHashMethod.DoubleClick += new System.EventHandler(this.LblHashMethod_DoubleClick);
            // 
            // cmbHashMethod
            // 
            this.cmbHashMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbHashMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHashMethod.FormattingEnabled = true;
            this.cmbHashMethod.Location = new System.Drawing.Point(112, 11);
            this.cmbHashMethod.Name = "cmbHashMethod";
            this.cmbHashMethod.Size = new System.Drawing.Size(300, 21);
            this.cmbHashMethod.TabIndex = 2;
            // 
            // txtChars
            // 
            this.txtChars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChars.Location = new System.Drawing.Point(112, 191);
            this.txtChars.Name = "txtChars";
            this.txtChars.Size = new System.Drawing.Size(278, 20);
            this.txtChars.TabIndex = 16;
            this.txtChars.Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
            this.txtChars.TextChanged += new System.EventHandler(this.TextBoxUppercase);
            this.txtChars.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateUniqueUppercase);
            // 
            // lblChars
            // 
            this.lblChars.AutoSize = true;
            this.lblChars.Location = new System.Drawing.Point(16, 194);
            this.lblChars.Name = "lblChars";
            this.lblChars.Size = new System.Drawing.Size(90, 13);
            this.lblChars.TabIndex = 15;
            this.lblChars.Text = "Characters to use";
            // 
            // btnQuickChars
            // 
            this.btnQuickChars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuickChars.Location = new System.Drawing.Point(388, 190);
            this.btnQuickChars.Name = "btnQuickChars";
            this.btnQuickChars.Size = new System.Drawing.Size(24, 22);
            this.btnQuickChars.TabIndex = 17;
            this.btnQuickChars.Text = "...";
            this.btnQuickChars.UseVisualStyleBackColor = true;
            this.btnQuickChars.Click += new System.EventHandler(this.BtnQuickChars_Click);
            // 
            // FrmMixNameFinder
            // 
            this.AcceptButton = this.btnGenerate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 491);
            this.Controls.Add(this.txtChars);
            this.Controls.Add(this.btnQuickChars);
            this.Controls.Add(this.lblChars);
            this.Controls.Add(this.cmbHashMethod);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.chkFindAllMatches);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.nmrMinLength);
            this.Controls.Add(this.nmrMaxLength);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.lblExtension);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblMinLength);
            this.Controls.Add(this.lblMaxLength);
            this.Controls.Add(this.lblHashMethod);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.txtExtension);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.txtEnd);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblStart);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.txtStart);
            this.Icon = global::CnCMixNameFinder.Properties.Resources.cchasher;
            this.MinimumSize = new System.Drawing.Size(300, 460);
            this.Name = "FrmMixNameFinder";
            this.Text = "MixNameFinder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMixNameFinder_FormClosing);
            this.Shown += new System.EventHandler(this.FrmMixNameFinder_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.nmrMaxLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrMinLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtStart;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.TextBox txtEnd;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtExtension;
        private System.Windows.Forms.Label lblExtension;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown nmrMaxLength;
        private System.Windows.Forms.Label lblMaxLength;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.NumericUpDown nmrMinLength;
        private System.Windows.Forms.Label lblMinLength;
        private System.Windows.Forms.CheckBox chkFindAllMatches;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Label lblHashMethod;
        private System.Windows.Forms.ComboBox cmbHashMethod;
        private System.Windows.Forms.TextBox txtChars;
        private System.Windows.Forms.Label lblChars;
        private System.Windows.Forms.Button btnQuickChars;
    }
}

