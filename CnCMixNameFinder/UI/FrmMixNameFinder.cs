using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CnCMixNameFinder.Domain;
using Nyerguds.Util.UI;

namespace CnCMixNameFinder.UI
{
    public partial class FrmMixNameFinder : Form, NameFinderReporter
    {
        protected Thread m_processingThread;
        protected NameFinder m_namefinder;
        protected HashSet<Object> m_editingText = new HashSet<Object>();
        protected readonly String StrButtonPause = "Pause";
        protected readonly String StrButtonUnpause = "Unpause";
        protected const String CHARS_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        protected const String CHARS_NUMERIC = "0123456789";
        protected const String CHARS_FULL = CHARS_ALPHABET + CHARS_NUMERIC + "_";
        protected const String CHARS_ALPHABET_S = " " + CHARS_ALPHABET;
        protected const String CHARS_ALPHABET_U = CHARS_ALPHABET + "_";
        protected string initValue = "";

        protected enum State
        {
            READY,
            BUSY
        }

        public FrmMixNameFinder()
        {
            InitializeComponent();
            cmbHashMethod.DataSource = HashMethod.GetRegisteredMethods();
            this.Text = "MixNameFinder " + ProgramVersion();
            ModeChanged();
            //IniFile inf = new IniFile("settings.ini", "");
        }

        public static String ProgramVersion()
        {
            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            //Version v = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version;
            String version = String.Format("v{0}.{1}", ver.FileMajorPart, ver.FileMinorPart);
            if (ver.FileBuildPart > 0)
                version += "." + ver.FileBuildPart;
            if (ver.FilePrivatePart > 0)
                version += "." + ver.FilePrivatePart;
            return version;
        }

        protected virtual void BtnGenerate_Click(Object sender, EventArgs e)
        {
            String[] ids = txtId.Text.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<UInt32> fileIds = new List<UInt32>();
            foreach (String id in ids)
            {
                UInt32 fileId;
                try
                {
                    fileId = Convert.ToUInt32(id, 16);
                }
                catch
                {
                    MessageBox.Show(this, "Error interpreting id value \"" + id + "\" as hex bytes!", "MixNameFinder");
                    return;
                }
                fileIds.Add(fileId);
            }
            if (fileIds.Count == 0)
            {
                MessageBox.Show(this, "No id values given!", "MixNameFinder");
                return;
            }
            if (nmrMinLength.Value > nmrMaxLength.Value)
            {
                MessageBox.Show(this, "Minimum length can not be larger than maximum length!", "MixNameFinder");
                return;
            }
            String dictionaryFilename;
            Char[] chars;
            if (rdbBruteForce.Checked)
            {
                if (this.txtChars.Text.Length == 0)
                {
                    MessageBox.Show(this, "No characters set to generate names from!", "MixNameFinder");
                    return;
                }
                chars = this.txtChars.Text.ToCharArray();
                dictionaryFilename = null;
            }
            else
            {
                if (String.IsNullOrEmpty(this.txtDictionaryFile.Text))
                {
                    MessageBox.Show(this, "No file set to use as dictionary!", "MixNameFinder");
                    return;
                }
                chars = null;
                dictionaryFilename = this.txtDictionaryFile.Text;
                if (!File.Exists(dictionaryFilename))
                {
                    MessageBox.Show(this, "File not found \"" + dictionaryFilename + "\"!", "MixNameFinder");
                    return;
                }
            }
            int[] initValues;
            if (String.IsNullOrEmpty(initValue))
            {
                initValues = null;
            }
            else
            {
                initValues = GetInitValues(initValue);
            }
            Object[] arrParams = {
                txtStart.Text, txtEnd.Text, fileIds.ToArray(), nmrMinLength.IntValue, nmrMaxLength.IntValue,
                chars, dictionaryFilename, chkFindAllMatches.Checked, chkSpaces.Checked, txtSeparator.Text, cmbHashMethod.SelectedValue, initValues
            };
            m_processingThread = new Thread(Generate);
            m_processingThread.Start(arrParams);
        }

        protected virtual void Generate(Object parameters)
        {
            EnableComponents(false);
            Object[] arrParams = (Object[])parameters;
            String startStr = (String)arrParams[0];
            String endStr = (String)arrParams[1];
            UInt32[] fileIds = (UInt32[])arrParams[2];
            Int32 minLength = (Int32)arrParams[3];
            Int32 maxLength = (Int32)arrParams[4];
            Char[] chars = (Char[])arrParams[5];
            String filename = (String)arrParams[6];
            Boolean getAllMatches = (Boolean)arrParams[7];
            // Can be either the "skip space-trimmable" option or the "add spaces" one, depending on the mode.
            Boolean spaces = (Boolean)arrParams[8];
            String separator = (String)arrParams[9];
            HashMethod hashMethod = (HashMethod)arrParams[10];
            Int32[] initialValues = arrParams[11] as Int32[];

            Boolean isDictionary = filename != null;

            txtResult.Invoke(new Action(() => this.txtResult.Text = String.Empty));
            txtStatus.Invoke(new Action(() => this.txtStatus.Text = String.Empty));

            if (isDictionary)
            {
                String[] dictionary = File.ReadAllLines(filename);
                m_namefinder = new NameFinder(hashMethod, startStr, endStr, dictionary, fileIds, minLength, maxLength, spaces ? separator : null, this);
            }
            else
            {
                m_namefinder = new NameFinder(hashMethod, startStr, endStr, chars, fileIds, minLength, maxLength, spaces, this);
            }

            m_namefinder.FindName(getAllMatches, initialValues);
            if (!m_namefinder.RunWasCancelledForClose)
            {
                if (!m_namefinder.IsMatched)
                {
                    txtResult.Invoke(new Action(() => this.txtResult.Text = "(NOT FOUND)"));
                }
                EnableComponents(true);
            }
        }

        protected virtual Int32[] GetInitValues(String initValue)
        {
            String initStr = initValue.Replace(" ", "");
            String[] initStrValues = initStr.Split(new[] { ',', ';' });
            int maxLen = Math.Min(initStrValues.Length, nmrMaxLength.IntValue);
            int[] initValues = new int[maxLen];
            for (int i = 0; i < maxLen; ++i)
            {
                if (Int32.TryParse(initStrValues[i], out int val))
                {
                    initValues[i] = val;
                }
            }
            return initValues;
        }

        protected String GetInitValueString(int[] initValues)
        {
            return String.Join(",", initValues.Select(v => v.ToString()).ToArray());
        }

        protected virtual void EnableComponents(Boolean enabled)
        {
            this.cmbHashMethod.Invoke(new Action(() => SetControlEnabled(this.cmbHashMethod, enabled)));
            this.txtId.Invoke(new Action(() => this.txtId.ReadOnly = !enabled));
            this.txtStart.Invoke(new Action(() => this.txtStart.ReadOnly = !enabled));
            this.txtEnd.Invoke(new Action(() => this.txtEnd.ReadOnly = !enabled));
            this.nmrMinLength.Invoke(new Action(() => this.nmrMinLength.ReadOnly = !enabled));
            this.nmrMaxLength.Invoke(new Action(() => this.nmrMaxLength.ReadOnly = !enabled));
            this.txtChars.Invoke(new Action(() => this.txtChars.ReadOnly = !enabled));
            this.btnQuickChars.Invoke(new Action(() => this.btnQuickChars.Enabled = enabled));
            this.chkFindAllMatches.Invoke(new Action(() => this.chkFindAllMatches.Enabled = enabled));
            this.chkSpaces.Invoke(new Action(() => this.chkSpaces.Enabled = enabled));
            if (enabled) this.Invoke(new Action(() => CheckForSpaces()));
            this.btnGenerate.Invoke(new Action(() => this.btnGenerate.Enabled = enabled));
            this.btnContinueState.Invoke(new Action(() => this.btnContinueState.Enabled = enabled));
            this.btnPause.Invoke(new Action(() => this.btnPause.Enabled = !enabled));
            this.btnPause.Invoke(new Action(() => this.btnPause.Text = this.StrButtonPause));
            this.btnAbort.Invoke(new Action(() => this.btnAbort.Enabled = !enabled));
            if (enabled)
                this.lblStatus.Invoke(new Action(() => this.SetStatusText(State.READY)));
            else
                this.lblStatus.Invoke(new Action(() => this.SetStatusText(State.BUSY)));
        }

        protected virtual void SetStatusText(State state)
        {
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

        protected virtual void SetControlEnabled(Control c, Boolean b)
        {
            c.Enabled = b;
            if (c is ListBox lb)
            {
                try
                {
                    lb.BackColor = lb.Enabled ? SystemColors.Window : SystemColors.Control;
                }
                catch { /* ignore */ }
            }
        }

        protected virtual void FrmMixNameFinder_Shown(Object sender, EventArgs e)
        {
            this.lblStatus.Invoke(new Action(() => this.SetStatusText(State.READY)));
        }

        #region NameFinderReporter Members

        public void ShowStatus(NameFinder origin, ProcessingStatus status, String currentStr, Int32 keyLength)
        {
            ShowStatus(origin, status, String.Empty, currentStr, keyLength);
        }

        public void ShowStatus(NameFinder origin, ProcessingStatus status, String currentKey, String currentStr, Int32 keyLength)
        {
            String report;
            switch (status)
            {
                case ProcessingStatus.FOUND:
                    report = "Match found: " + currentKey + " = \"" + currentStr + "\"";
                    break;
                case ProcessingStatus.RUNNING:
                default:
                    report = "Generating string \"" + currentStr + "\" (generated length " + keyLength + ")";
                    break;
                case ProcessingStatus.PAUSED:
                    report = "Paused at \"" + currentStr + "\" (length " + keyLength + ")";
                    break;
                case ProcessingStatus.ABORTED:
                    report = "Aborted. Last string: \"" + currentStr + "\" (generated length " + keyLength + ")";
                    break;
                case ProcessingStatus.ENDED:
                    if (String.IsNullOrEmpty(currentStr))
                        report = "Generating ended. No results found.";
                    else
                        report = "Generating ended. Last match: " + currentStr;
                    break;
            }
            if (!origin.RunWasCancelledForClose)
            {
                this.txtStatus.Invoke(new Action(() => this.txtStatus.Text = report));
                if (status == ProcessingStatus.ABORTED)
                {
                    this.Invoke(new Action(() => AskIfSave(currentKey, keyLength)));
                }
            }
            if (status == ProcessingStatus.FOUND)
            {
                String str = currentKey + " = " + currentStr;
                if (!origin.RunWasCancelledForClose)
                {
                    try
                    {
                        string screenStr = str;
                        int? txtLen = (int?)txtResult.Invoke(new Func<int?>(() => { return txtResult.TextLength; }));
                        if (txtLen.HasValue && txtLen.Value > 0)
                            screenStr = Environment.NewLine + str;
                        this.txtResult.Invoke(new Action(() => this.txtResult.AppendText(screenStr)));
                    }
                    catch { /* ignore */ }
                }
                String logFileName = origin.HashMethodSimpleName + "_" + origin.TimeStarted.Ticks.ToString() + ".txt";
                FileInfo logFile = new FileInfo(logFileName);
                /*/
                if (!logFile.Exists || logFile.Length == 0)
                {
                    IniFile init = new IniFile(logFileName, String.Empty, Encoding.ASCII);
                    init.SetStringValue("HashSettings", "Method", origin.HashMethodSimpleName);
                    init.SetStringValue("HashSettings", "Hashes", String.Join(",", origin.NameIds.Select(n => n.ToString("X8")).ToArray()));
                    init.SetStringValue("HashSettings", "StartString", this.EscapeString(origin.StartString));
                    init.SetStringValue("HashSettings", "EndString", this.EscapeString(origin.EndString));
                    init.SetIntValue("HashSettings", "MinimumLength", origin.MinLength);
                    init.SetIntValue("HashSettings", "MaximumLength", origin.MaxLength);

                    init.WriteIni();
                    // TODO write ini
                    //File.AppendAllText("");
                }
                //*/
                File.AppendAllText(logFile.FullName, Environment.NewLine + str, Encoding.ASCII);
            }
        }

        protected virtual String EscapeString(String str)
        {
            if (str.Length > 0 && str.StartsWith(" ") || str.EndsWith(" "))
            {
                str = "\"" + str.Replace("\"", "\"\"") + "\"";
            }
            return str;
        }

        protected virtual String UnescapeString(String str)
        {
            String testStr = str.Trim();
            if (testStr.Length > 2 && testStr.StartsWith("\"") && testStr.EndsWith("\""))
            {
                testStr = testStr.Substring(1, str.Length - 2);
                str = testStr.Replace("\"\"", "\"");
            }
            return str;
        }

        protected virtual void AskIfSave(String currentKey, Int32 keyLength)
        {
            String message = "Do you want to store the current progress?" +
                "\nThis will set the minimum length to the currently generated length," +
                " and fill in the \"initial state\" preset with the end-state of the current run.";
            if (DialogResult.Yes == MessageBox.Show(message, "MixNameFinder", MessageBoxButtons.YesNo))
            {
                this.nmrMinLength.IntValue = keyLength;
                initValue = currentKey;
            }
        }

        #endregion

        protected virtual void FrmMixNameFinder_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (m_processingThread != null && m_processingThread.IsAlive)
            {
                if (m_namefinder != null)
                {
                    m_namefinder.CancelRun(true);
                }
            }
        }

        protected virtual void BtnPause_Click(Object sender, EventArgs e)
        {
            if (m_namefinder != null)
            {
                Boolean isPaused = m_namefinder.RunPaused;
                m_namefinder.RunPaused = !isPaused;
                btnPause.Text = (isPaused ? StrButtonPause : StrButtonUnpause);
            }
        }

        protected virtual void BtnAbort_Click(Object sender, EventArgs e)
        {
            if (m_namefinder != null)
                m_namefinder.CancelRun(false);
        }

        protected virtual void btnContinueState_Click(Object sender, EventArgs e)
        {
            int[] values;
            if (initValue == null)
            {
                values = new int[nmrMinLength.IntValue];
            }
            else
            {
                values = GetInitValues(initValue);
            }
            string newInitValue = GetInitValueString(values);
            string newVal = InputBox.Show("Give initial array state:", "MixNamefinder", newInitValue, s => CheckNum(s));
            // null means cancel was pressed.
            if (newVal != null)
            {
                int[] vals = GetInitValues(newVal);
                if (vals.All(v => v == 0))
                {
                    initValue = null;
                }
                else
                {
                    initValue = GetInitValueString(vals);
                    nmrMinLength.IntValue = vals.Length;
                }
            }
        }

        protected String CheckNum(String s)
        {
            if (s == null)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            int length = s.Length;
            for (int i = 0; i < length; i++)
            {
                char c = s[i];
                if (c == ' ' || c == ',' || c == ';' || (c >= '0' && c <= '9'))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        protected virtual void lblHashMethod_Click(Object sender, EventArgs e)
        {
            using (FrmTestHash hash = new FrmTestHash((HashMethod)cmbHashMethod.SelectedValue))
            {
                hash.StartPosition = FormStartPosition.CenterParent;
                hash.ShowDialog(this);
            }
        }

        protected virtual void txtStart_TextChanged(Object sender, EventArgs e)
        {
            TextBoxUppercase(sender);
        }

        protected virtual void txtEnd_TextChanged(Object sender, EventArgs e)
        {
            TextBoxUppercase(sender);
        }

        protected virtual void txtChars_TextChanged(Object sender, EventArgs e)
        {
            TextBoxUppercase(sender);
            CheckForSpaces();
        }

        protected virtual void TextBoxUppercase(Object sender)
        {
            if (m_editingText.Contains(sender))
                return;
            try
            {
                if (!(sender is TextBox))
                    return;
                m_editingText.Add(sender);
                TextBox textbox = (TextBox)sender;
                Int32 selStart = textbox.SelectionStart;
                textbox.Text = textbox.Text.ToUpperInvariant();
                textbox.SelectionStart = selStart;
                textbox.SelectionLength = 0;
            }
            finally
            {
                m_editingText.Remove(sender);
            }
        }

        protected virtual void CheckForSpaces()
        {
            chkSpaces.Enabled = !rdbBruteForce.Checked || txtChars.Text.Contains(' ');
        }

        protected virtual void ValidateUniqueUppercase(Object sender, System.ComponentModel.CancelEventArgs e)
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

        protected virtual void BtnQuickChars_Click(Object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
                OpenInsertToolStrip(button, this.txtChars);
        }

        protected virtual void btnSelectDictionaryFile_Click(Object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                ofd.Multiselect = false;
                String currentPath = this.txtDictionaryFile.Text;
                ofd.FileName = String.IsNullOrEmpty(currentPath) ? String.Empty : Path.GetFileName(currentPath);
                ofd.InitialDirectory = String.IsNullOrEmpty(currentPath) ? String.Empty : Path.GetDirectoryName(currentPath);
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    this.txtDictionaryFile.Text = ofd.FileName;
                }
            }
        }

        protected virtual void OpenInsertToolStrip(Button button, TextBox target)
        {
            ContextMenuStrip cms = new ContextMenuStrip { ShowImageMargin = false };
            ToolStripMenuItem tsmiAlphabet = new ToolStripMenuItem("Set to &alphabet characters");
            tsmiAlphabet.Click += (sender, e) => target.Invoke(new Action(() => target.Text = CHARS_ALPHABET));
            cms.Items.Add(tsmiAlphabet);
            ToolStripMenuItem tsmiAlphabetSp = new ToolStripMenuItem("Set to alphabet characters plus &space");
            tsmiAlphabetSp.Click += (sender, e) => target.Invoke(new Action(() => target.Text = CHARS_ALPHABET_S));
            cms.Items.Add(tsmiAlphabetSp);
            ToolStripMenuItem tsmiAlphabetUn = new ToolStripMenuItem("Set to alphabet characters plus &underscore");
            tsmiAlphabetUn.Click += (sender, e) => target.Invoke(new Action(() => target.Text = CHARS_ALPHABET_U));
            cms.Items.Add(tsmiAlphabetUn);
            ToolStripMenuItem tsmiAlphanumeric = new ToolStripMenuItem("Set to alpha&numeric characters plus underscore");
            tsmiAlphanumeric.Click += (sender, e) => target.Invoke(new Action(() => target.Text = CHARS_FULL));
            cms.Items.Add(tsmiAlphanumeric);
            Point ptLowerLeft = new Point(0, button.Height);
            ptLowerLeft = button.PointToScreen(ptLowerLeft);
            cms.Show(ptLowerLeft);
        }

        protected virtual void txtId_TextChanged(Object sender, EventArgs e)
        {
            if (m_editingText.Contains(sender))
                return;
            try
            {
                m_editingText.Add(sender);
                String input = txtId.Text.ToUpperInvariant();
                Int32 selStart = this.txtId.SelectionStart;
                String output = new String(input.Where(x => (x >= '0' && x <= '9') || (x >= 'A' && x <= 'F') || x == ' ' || x == ',' || x == ';').ToArray());
                if (String.Equals(txtId.Text, output))
                    return;
                txtId.Text = output;
                if (Math.Min(selStart, txtId.Text.Length) > 0 && selStart <= txtId.Text.Length && output[selStart - 1] != input[selStart - 1])
                    selStart--;
                this.txtId.SelectionStart = Math.Min(selStart, txtId.Text.Length);
            }
            finally
            {
                m_editingText.Remove(sender);
            }
        }

        protected virtual void TextFieldKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                if (sender is TextBox tb)
                {
                    tb.SelectAll();
                }
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        protected virtual void RadioButtonChanged(Object sender, EventArgs e)
        {
            // Only execute for checked radiobutton.
            if (sender is RadioButton rdb && rdb.Checked)
            {
                ModeChanged();
            }
        }

        protected virtual void ModeChanged()
        {
            bool isBruteForce = rdbBruteForce.Checked;
            txtChars.ReadOnly = !isBruteForce;
            btnQuickChars.Enabled = isBruteForce;
            btnSelectDictionaryFile.Enabled = !isBruteForce;
            chkSpaces.Text = isBruteForce ? "Skip space-trimmable entries" : "Also test with separator";
            txtSeparator.Visible = !isBruteForce;
            txtSeparator.Enabled = !isBruteForce && chkSpaces.Checked;
            int posLeft = chkSpaces.Right + chkSpaces.Margin.Right;
            txtSeparator.Left = posLeft;
            txtSeparator.Width = cmbHashMethod.Right - posLeft;
            CheckForSpaces();
        }

        protected virtual void nmrMinLength_ValueEntered(Object sender, ValueEnteredEventArgs e)
        {
            if (e.Oldvalue != e.Newvalue)
            {
                this.initValue = null;
            }
        }
    }
}
