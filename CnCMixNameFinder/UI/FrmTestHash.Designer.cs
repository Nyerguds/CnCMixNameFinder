namespace CnCMixNameFinder.UI
{
    partial class FrmTestHash
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
            this.cmbHashMethod = new System.Windows.Forms.ComboBox();
            this.lblHashMethod = new System.Windows.Forms.Label();
            this.lblId = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.lblFilename = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cmbHashMethod
            // 
            this.cmbHashMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbHashMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHashMethod.FormattingEnabled = true;
            this.cmbHashMethod.Location = new System.Drawing.Point(117, 11);
            this.cmbHashMethod.Name = "cmbHashMethod";
            this.cmbHashMethod.Size = new System.Drawing.Size(200, 21);
            this.cmbHashMethod.TabIndex = 34;
            this.cmbHashMethod.SelectedIndexChanged += new System.EventHandler(this.cmbHashMethod_SelectedIndexChanged);
            // 
            // lblHashMethod
            // 
            this.lblHashMethod.AutoSize = true;
            this.lblHashMethod.Location = new System.Drawing.Point(17, 15);
            this.lblHashMethod.Name = "lblHashMethod";
            this.lblHashMethod.Size = new System.Drawing.Size(88, 13);
            this.lblHashMethod.TabIndex = 33;
            this.lblHashMethod.Text = "Hashing Method:";
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(17, 67);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(52, 13);
            this.lblId.TabIndex = 35;
            this.lblId.Text = "Name ID:";
            // 
            // txtId
            // 
            this.txtId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtId.BackColor = System.Drawing.SystemColors.Control;
            this.txtId.Location = new System.Drawing.Point(117, 64);
            this.txtId.MaxLength = 8;
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(200, 20);
            this.txtId.TabIndex = 36;
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(17, 41);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(52, 13);
            this.lblFilename.TabIndex = 37;
            this.lblFilename.Text = "Filename:";
            // 
            // txtFilename
            // 
            this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilename.Location = new System.Drawing.Point(117, 38);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(200, 20);
            this.txtFilename.TabIndex = 38;
            this.txtFilename.TextChanged += new System.EventHandler(this.TxtFilename_TextChanged);
            // 
            // FrmTestHash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 99);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.cmbHashMethod);
            this.Controls.Add(this.lblHashMethod);
            this.Icon = global::CnCMixNameFinder.Properties.Resources.cchasher;
            this.MinimumSize = new System.Drawing.Size(350, 138);
            this.Name = "FrmTestHash";
            this.Text = "Test hashing algorithms";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbHashMethod;
        private System.Windows.Forms.Label lblHashMethod;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.TextBox txtFilename;
    }
}