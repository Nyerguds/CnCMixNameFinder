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
        }

        private void CalculateNameId()
        {
            HashMethod hashMethod = (HashMethod)cmbHashMethod.SelectedValue;
            txtId.Text = hashMethod.GetNameIdHexString(txtFilename.Text);
        }

        private void cmbHashMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateNameId();
        }

        private void TxtFilename_TextChanged(object sender, EventArgs e)
        {
            CalculateNameId();
        }
    }
}
