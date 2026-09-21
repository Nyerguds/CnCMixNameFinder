using CnCMixNameFinder.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CnCMixNameFinder.UI
{
    public partial class FrmTestHash : Form
    {
        public FrmTestHash()
        {
            InitializeComponent();
            cmbHashMethod.DataSource = HashMethod.GetRegisteredMethods();
            this.TriggerCalculateHash(this.txtFilename, null);
        }
        
        private void TriggerCalculateHash(Object sender, EventArgs e)
        {
            TextBoxUppercase(sender, e);
            txtId.Text = ((HashMethod)cmbHashMethod.SelectedValue).GetNameIdHexString(this.txtFilename.Text);
        }

        private void TextBoxUppercase(Object sender, EventArgs e)
        {
            if (!(sender is TextBox))
                return;
            TextBox textbox = (TextBox)sender;
            Int32 selStart = textbox.SelectionStart;
            Int32 selLen = textbox.SelectionStart;
            textbox.Text = textbox.Text.ToUpperInvariant();
            textbox.SelectionStart = selStart;
            textbox.SelectionStart = selLen;
        }

        private void TxtFilename_Validating(Object sender, CancelEventArgs e)
        {
            this.TriggerCalculateHash(sender, e);
        }

        private void FrmTestHash_KeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
