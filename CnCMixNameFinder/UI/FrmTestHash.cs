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
        public FrmTestHash() : this(null) { }
        public FrmTestHash(HashMethod selectedValue)
        {
            this.InitializeComponent();
            HashMethod[] methods = HashMethod.GetRegisteredMethods();
            this.cmbHashMethod.DataSource = methods;
            Type selType = selectedValue == null ? null : selectedValue.GetType();
            HashMethod toSelect;
            if (selType != null && (toSelect = methods.FirstOrDefault(m => m.GetType() == selType)) != null)
                this.cmbHashMethod.SelectedItem = toSelect;
            this.TriggerCalculateHash(this.txtFilename, null);
        }
        
        private void TriggerCalculateHash(Object sender, EventArgs e)
        {
            TextBox textbox = txtFilename;
            this.TextBoxUppercase(textbox, e);
            this.txtId.Text = ((HashMethod)this.cmbHashMethod.SelectedValue).GetNameIdHexString(textbox.Text);
        }

        private void TextBoxUppercase(Object sender, EventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox == null)
                return;
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
    }
}
