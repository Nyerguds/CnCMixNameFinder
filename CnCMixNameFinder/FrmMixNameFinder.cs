using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CnCMixNameFinder.domain;
using System.Threading;

namespace CnCMixNameFinder
{
    public partial class FrmMixNameFinder : Form, NameFinderReporter
    {

        public delegate void invoke_delegate_single_parameter(Object c, Object value);
        public delegate void invoke_delegate_with_arg(Object value);
        private Thread processingThread;
        private readonly String StrButtonPause = "Pause";
        private readonly String StrButtonUnpause = "Unpause";
        private NameFinder m_namefinder;


        private enum State
        {
            READY,
            BUSY
        }

        public FrmMixNameFinder()
        {
            InitializeComponent();
        }

        private void Generate(Object parameters)
        {
            EnableComponents(false);
            Object[] arrParams = (Object[])parameters;
            String startStr = (String)arrParams[0];
            String endStr = (String)arrParams[1];
            String extension = (String)arrParams[2];
            Boolean alphabetOnly = (Boolean)arrParams[3];
            UInt32 fileId = (UInt32)arrParams[4];
            Int32 minLength = (Int32)arrParams[5];
            Int32 maxLength = (Int32)arrParams[6];
            Boolean getAllMatches = (Boolean)arrParams[7];

            txtResult.Invoke(new invoke_delegate_single_parameter(setTextValue), new object[] { txtResult, String.Empty });
            txtStatus.Invoke(new invoke_delegate_single_parameter(setTextValue), new object[] { txtStatus, String.Empty });

            m_namefinder = new NameFinder(startStr, endStr, extension, fileId, minLength, maxLength, this);
            m_namefinder.FindName(alphabetOnly, getAllMatches);

            if (!m_namefinder.IsMatched)
            {
                txtResult.Invoke(new invoke_delegate_single_parameter(setTextValue), new object[] { txtResult, "(NOT FOUND)" });
            }
            EnableComponents(true);
        }

        private void btnGenerate_Click(Object sender, EventArgs e)
        {
            UInt32 fileId = 0;
            try
            {
                fileId = Convert.ToUInt32(txtId.Text, 16);
            }
            catch
            {
                MessageBox.Show(this, "Error interpreting value \"" +  txtId.Text+ "\" as hex bytes!", "MixNameFinder");
                return;
            }
            if (nmrMinLength.Value > nmrMaxLength.Value)
            {
                MessageBox.Show(this, "Minimum length can not be larger than maximum length!", "MixNameFinder");
                return;
            }

            Object[] arrParams = new Object[] 
            {
                txtStart.Text, txtEnd.Text, txtExtension.Text, chkAlphabetOnly.Checked,
                fileId, (Int32)nmrMinLength.Value, (Int32)nmrMaxLength.Value, chkFindAllMatches.Checked
            };

            processingThread = new Thread(Generate);
            processingThread.Start(arrParams);
        }

        private void EnableComponents(Boolean enabled)
        {
            txtId.Invoke(new invoke_delegate_single_parameter(setReadOnly), new object[] { txtId, !enabled });
            txtStart.Invoke(new invoke_delegate_single_parameter(setReadOnly), new object[] { txtStart, !enabled });
            txtEnd.Invoke(new invoke_delegate_single_parameter(setReadOnly), new object[] { txtEnd, !enabled });
            txtExtension.Invoke(new invoke_delegate_single_parameter(setReadOnly), new object[] { txtExtension, !enabled });
            nmrMinLength.Invoke(new invoke_delegate_single_parameter(setReadOnly), new object[] { nmrMinLength, !enabled });
            nmrMaxLength.Invoke(new invoke_delegate_single_parameter(setReadOnly), new object[] { nmrMaxLength, !enabled });
            chkAlphabetOnly.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkAlphabetOnly, enabled });
            chkFindAllMatches.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkFindAllMatches, enabled });
            btnGenerate.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { btnGenerate, enabled });
            btnPause.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { btnPause, !enabled });
            btnPause.Invoke(new invoke_delegate_single_parameter(setTextValue), new object[] { btnPause, StrButtonPause });
            btnAbort.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { btnAbort, !enabled });
            if (enabled)
                this.lblStatus.Invoke(new invoke_delegate_with_arg(setStatusText), State.READY);
            else
                this.lblStatus.Invoke(new invoke_delegate_with_arg(setStatusText), State.BUSY);
        }

        private void setStatusText(object s)
        {
            State state = (State)s;
            if (state == State.READY)
            {
                lblStatus.Text = "Ready.";
                lblStatus.ForeColor = Color.FromArgb(0, 192, 0);
            }
            else
            {
                lblStatus.Text = "Working...";
                lblStatus.ForeColor = Color.FromArgb(192, 0, 0);
            }
        }

        private void setControlEnabled(Object obj, object b)
        {
            if (!(obj is Control))
                return;

            Control c = (Control)obj;

            if (b is Boolean)
            {
                c.Enabled = (Boolean)b;
                if (c is ListBox)
                {
                    try
                    {
                        ListBox lb = (ListBox)c;
                        if (!lb.Enabled)
                            lb.BackColor = SystemColors.Control;
                        else
                            lb.BackColor = SystemColors.Window;
                    }
                    catch { }
                }
            }
        }

        private void setReadOnly(Object c, object value)
        {
            if (c is TextBoxBase)
            {
                ((TextBoxBase)c).ReadOnly = (Boolean)value;
            }
            else if (c is UpDownBase)
            {
                ((UpDownBase)c).ReadOnly = (Boolean)value;
            }
        }

        private void setTextValue(Object c, object value)
        {
            if (!(c is Control))
                return;

            ((Control)c).Text = Convert.ToString(value);
        }

        private String getTextValue(Object c)
        {
            if (!(c is Control))
                return null;

            return ((Control)c).Text;
        }


        private void appendTextToTextbox(Object c, object value)
        {
            if (!(c is TextBox))
                return;

            ((TextBox)c).AppendText(Convert.ToString(value));
        }


        private void FrmMixNameFinder_Shown(object sender, EventArgs e)
        {
            this.lblStatus.Invoke(new invoke_delegate_with_arg(setStatusText), State.READY);
        }


        #region NameFinderReporter Members

        public void ShowStatus(ProcessingStatus status, String currentStr, Int32 keyLength)
        {
            String report;
            switch(status)
            {
                case ProcessingStatus.FOUND:
                    report = "Match found: " + currentStr;
                    break;
                case ProcessingStatus.RUNNING:
                default:
                    report = "Generating string \"" + currentStr + "\" (length " + keyLength + ")";
                    break;
                case ProcessingStatus.PAUSED:
                    report = "Paused at \"" + currentStr + "\" (length " + keyLength + ")";
                    break;
                case ProcessingStatus.ABORTED:
                    report = "Aborted. Last string: \"" + currentStr + "\" (length " + keyLength + ")";
                    break;
                case ProcessingStatus.ENDED:
                    if (String.IsNullOrEmpty(currentStr))
                        report = "Generating ended. No results found.";
                    else
                        report = "Generating ended. Last match: " + currentStr;
                    break;
            }   
            this.txtStatus.Invoke(new invoke_delegate_single_parameter(setTextValue), new object[] { txtStatus, report });
            if (status == ProcessingStatus.FOUND)
            {
                if (txtResult.Text.Length > 0)
                    currentStr = Environment.NewLine + currentStr;
                this.txtResult.Invoke(new invoke_delegate_single_parameter(appendTextToTextbox), new object[] { txtResult, currentStr });
            }
        }

        #endregion

        private void FrmMixNameFinder_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (processingThread != null && processingThread.IsAlive)
                processingThread.Abort();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            if (m_namefinder != null)
                m_namefinder.CancelRun();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (m_namefinder != null)
            {
                Boolean isPaused = m_namefinder.RunPaused;
                m_namefinder.RunPaused = !isPaused;
                btnPause.Text = (isPaused ? StrButtonPause : StrButtonUnpause);
            }
        }
    }
}
