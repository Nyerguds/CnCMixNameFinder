using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MissionNameChecker.domain;

namespace MissionNameChecker
{
    public partial class idcalc : Form
    {
        public idcalc()
        {
            InitializeComponent();
            button1_Click(this, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = NameGenerator.getNameIdHexString(textBox1.Text);
        }
    }
}
