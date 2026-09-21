using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CnCMixNameFinder.Domain;

namespace CnCMixNameFinder.UI
{
    public partial class FrmMixNameFinder : Form, NameFinderReporter
    {

        public delegate void invoke_delegate_single_parameter(Object c, Object value);
        public delegate void invoke_delegate_with_arg(Object value);
        private Thread processingThread;
        private readonly String StrButtonPause = "Pause";
        private readonly String StrButtonUnpause = "Unpause";
        private NameFinder m_namefinder;
        private const String CHARS_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const String CHARS_NUMERIC = "0123456789";
        private const String CHARS_FULL = CHARS_ALPHABET + CHARS_NUMERIC + "_";
        
        private enum State
        {
            READY,
            BUSY
        }

        public FrmMixNameFinder()
        {
            InitializeComponent();
            cmbHashMethod.DataSource = HashMethod.GetRegisteredMethods();
        }

        private void Generate(Object parameters)
        {
            EnableComponents(false);
            Object[] arrParams = (Object[])parameters;
            String startStr = (String)arrParams[0];
            String endStr = (String)arrParams[1];
            String extension = (String)arrParams[2];
            UInt32 fileId = (UInt32)arrParams[3];
            Int32 minLength = (Int32)arrParams[4];
            Int32 maxLength = (Int32)arrParams[5];
            Char[] chars = (Char[])arrParams[6];
            Boolean getAllMatches = (Boolean)arrParams[7];
            HashMethod hashMethod = (HashMethod)arrParams[8];

            txtResult.Invoke(new invoke_delegate_single_parameter(this.SetTextValue), this.txtResult, String.Empty);
            txtStatus.Invoke(new invoke_delegate_single_parameter(this.SetTextValue), this.txtStatus, String.Empty);

            m_namefinder = new NameFinder(hashMethod, startStr, endStr, extension, chars, fileId, minLength, maxLength, this);
            m_namefinder.FindName(getAllMatches);

            if (!m_namefinder.IsMatched)
            {
                txtResult.Invoke(new invoke_delegate_single_parameter(this.SetTextValue), this.txtResult, "(NOT FOUND)");
            }
            EnableComponents(true);
        }

        private void BtnGenerate_Click(Object sender, EventArgs e)
        {
            UInt32 fileId;
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
            if (this.txtChars.Text.Length == 0)
            {
                MessageBox.Show(this, "No characters set to generate names from!", "MixNameFinder");
                return;
            }
            Object[] arrParams = {
                txtStart.Text.ToUpperInvariant(), txtEnd.Text.ToUpperInvariant(), txtExtension.Text.ToUpperInvariant(), fileId,
                (Int32)nmrMinLength.Value, (Int32)nmrMaxLength.Value, this.txtChars.Text.ToCharArray(),
                chkFindAllMatches.Checked, cmbHashMethod.SelectedValue
            };

            processingThread = new Thread(Generate);
            processingThread.Start(arrParams);
        }

        private void EnableComponents(Boolean enabled)
        {
            this.txtId.Invoke(new invoke_delegate_single_parameter(this.SetReadOnly), this.txtId, !enabled);
            this.txtStart.Invoke(new invoke_delegate_single_parameter(this.SetReadOnly), this.txtStart, !enabled);
            this.txtEnd.Invoke(new invoke_delegate_single_parameter(this.SetReadOnly), this.txtEnd, !enabled);
            this.txtExtension.Invoke(new invoke_delegate_single_parameter(this.SetReadOnly), this.txtExtension, !enabled);
            this.nmrMinLength.Invoke(new invoke_delegate_single_parameter(this.SetReadOnly), this.nmrMinLength, !enabled);
            this.nmrMaxLength.Invoke(new invoke_delegate_single_parameter(this.SetReadOnly), this.nmrMaxLength, !enabled);
            this.txtChars.Invoke(new invoke_delegate_single_parameter(this.SetControlEnabled), this.txtChars, enabled);
            this.btnQuickChars.Invoke(new invoke_delegate_single_parameter(this.SetControlEnabled), this.btnQuickChars, enabled);
            this.chkFindAllMatches.Invoke(new invoke_delegate_single_parameter(this.SetControlEnabled), this.chkFindAllMatches, enabled);
            this.btnGenerate.Invoke(new invoke_delegate_single_parameter(this.SetControlEnabled), this.btnGenerate, enabled);
            this.btnPause.Invoke(new invoke_delegate_single_parameter(this.SetControlEnabled), this.btnPause, !enabled);
            this.btnPause.Invoke(new invoke_delegate_single_parameter(this.SetTextValue), this.btnPause, this.StrButtonPause);
            this.btnAbort.Invoke(new invoke_delegate_single_parameter(this.SetControlEnabled), this.btnAbort, !enabled);
            if (enabled)
                this.lblStatus.Invoke(new invoke_delegate_with_arg(this.SetStatusText), State.READY);
            else
                this.lblStatus.Invoke(new invoke_delegate_with_arg(this.SetStatusText), State.BUSY);
		}

        private void SetStatusText(Object s)
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

        private void SetControlEnabled(Object obj, Object b)
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
                    catch { /* ignore */ }
                }
            }
        }

        private void SetReadOnly(Object c, Object value)
        {
            if (c is TextBoxBase)
                ((TextBoxBase)c).ReadOnly = (Boolean)value;
            else if (c is UpDownBase)
                ((UpDownBase)c).ReadOnly = (Boolean)value;
        }

        private void SetTextValue(Object c, Object value)
        {
            if (!(c is Control))
                return;
            ((Control)c).Text = Convert.ToString(value);
        }

        private String GetTextValue(Object c)
        {
            if (!(c is Control))
                return null;
            return ((Control)c).Text;
        }

        private void AppendTextToTextbox(Object c, Object value)
        {
            if (!(c is TextBox))
                return;
            ((TextBox)c).AppendText(Convert.ToString(value));
        }


        private void FrmMixNameFinder_Shown(Object sender, EventArgs e)
        {
            this.lblStatus.Invoke(new invoke_delegate_with_arg(this.SetStatusText), State.READY);
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
            this.txtStatus.Invoke(new invoke_delegate_single_parameter(this.SetTextValue), this.txtStatus, report);
            if (status == ProcessingStatus.FOUND)
            {
                if (txtResult.Text.Length > 0)
                    currentStr = Environment.NewLine + currentStr;
                this.txtResult.Invoke(new invoke_delegate_single_parameter(this.AppendTextToTextbox), this.txtResult, currentStr);
            }
        }

        #endregion

        private void FrmMixNameFinder_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (processingThread != null && processingThread.IsAlive)
                processingThread.Abort();
        }

        private void BtnAbort_Click(Object sender, EventArgs e)
        {
            if (m_namefinder != null)
                m_namefinder.CancelRun();
        }

        private void BtnPause_Click(Object sender, EventArgs e)
        {
            if (m_namefinder != null)
            {
                Boolean isPaused = m_namefinder.RunPaused;
                m_namefinder.RunPaused = !isPaused;
                btnPause.Text = (isPaused ? StrButtonPause : StrButtonUnpause);
            }
        }

        private void LblHashMethod_DoubleClick(Object sender, EventArgs e)
        {
            new FrmTestHash().ShowDialog(this);
        }

        private void TextBoxUppercase(object sender, EventArgs e)
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

        private void ValidateUniqueUppercase(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            TextBox textbox = (TextBox)sender;
            List<Char> uniquechars = new List<Char>();
            foreach (Char c in textbox.Text)
            {
                Char cu = c.ToString().ToUpperInvariant()[0];
                if (!uniquechars.Contains(cu))
                    uniquechars.Add(cu);
            }
            textbox.Text = new String(uniquechars.ToArray());
        }

        private void BtnQuickChars_Click(object sender, EventArgs e)
        {
            if (sender is Button)
                OpenInsertToolStrip((Button)sender, this.txtChars);
        }

        private void OpenInsertToolStrip(Button button, TextBox target)
        {
            ContextMenuStrip cms = new ContextMenuStrip { ShowImageMargin = false };
            ToolStripMenuItem tsmiAlphabet = new ToolStripMenuItem("Set to &alphabet characters");
            tsmiAlphabet.Click += (sender, e) => target.Invoke(new invoke_delegate_single_parameter(this.SetTextValue), target, CHARS_ALPHABET);
            cms.Items.Add(tsmiAlphabet);
            ToolStripMenuItem tsmiAlphanumeric = new ToolStripMenuItem("Set to alpha&numeric characters plus underscore");
            tsmiAlphanumeric.Click += (sender, e) => target.Invoke(new invoke_delegate_single_parameter(this.SetTextValue), target, CHARS_FULL);
            cms.Items.Add(tsmiAlphanumeric);
            Point ptLowerLeft = new Point(0, button.Height);
            ptLowerLeft = button.PointToScreen(ptLowerLeft);
            cms.Show(ptLowerLeft);
        }
    }
}
