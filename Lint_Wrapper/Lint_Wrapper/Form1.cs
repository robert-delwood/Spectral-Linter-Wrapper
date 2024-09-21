//E:\Development\member_experience_June_21_2021.yaml


// https://www.dreamincode.net/forums/topic/153776-running-command-line-commands-from-winform/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lint_Wrapper
{
    public partial class Form1 : Form
    {
        List<BrokenRules> gBrokenRules = new List<BrokenRules>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Can also be configured through property settings
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToResizeColumns = true;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            // Get settings strings
            textBox2.Text = Properties.Settings.Default.rulesFile;
            textBox1.Text = Properties.Settings.Default.targetFile;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //Boolean didRun = RunCommandLine(false);
            //if(didRun == true) SetDefaultCheckBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = textBox1.Text.Trim();
            if (fileName.Length == 0 || File.Exists(fileName) == false)
            {
                MessageBox.Show(string.Format("The following file could not be found:\r\n\r\n{0}", fileName), "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string theRuleset = textBox2.Text.Trim();
            if (fileName.Length == 0 || File.Exists(theRuleset) == false)
            {
                MessageBox.Show(string.Format("The following file could not be found:\r\n\r\n{0}", theRuleset), "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            gBrokenRules.Clear();
            DeleteRadioButtons();

            Boolean didRun = RunCommandLine(true);
            if (didRun == true)
            {
                SetDefaultCheckBox();
            }
        }

        private void DeleteRadioButtons()
        {
            panel1.Controls.Clear();
        }

        private Boolean RunCommandLine(Boolean showFileNotFound)
        {
            //Process.Start("cmd.exe", @"/C ping www.google.com > e:\development\log.log");

            string missingFiles = string.Empty;

            string fileName = textBox1.Text.Trim();
            if (fileName.Length == 0 || File.Exists(fileName) == false)
            {
                if (showFileNotFound == true) missingFiles += string.Format("{0}\r\n", fileName);
            }

            string theRuleset = textBox2.Text.Trim();
            if (theRuleset.Length == 0 || File.Exists(theRuleset) == false)
            {
                if (showFileNotFound == true) missingFiles += string.Format("{0}\r\n", theRuleset);
            }

            if(missingFiles.Trim().Length != 0)
            {
                MessageBox.Show(string.Format("The following file(s) could not be found:\r\n\r\n{0}", missingFiles), "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            //Set up the command line command
            string theCommand = @"spectral lint";
            string arguments = string.Format("/C {0} --ruleset {1} {2}", theCommand, theRuleset, fileName);

            //Capture the output to a string for this application.
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            //p.StartInfo.Arguments = @"/C spectral lint --ruleset e:\development\spectral.yaml e:\development\swagger_petstore.yaml";
            p.StartInfo.Arguments = arguments;

            //p.StartInfo.Arguments = @"vale Accounts.txt";

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (output.Length == 0)
            {
                MessageBox.Show("It looks like the command did not run properly\r\n\r\nPlease make the command is correct and try again.", "Command Did Not Run", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            dataGridView1.DataSource = null;

            string[] subs = output.Split('\n');
            List<string> myList = new List<string>(subs);
            myList.RemoveAll(x => x.Length == 0);

            myList.RemoveAt(0);

            label2.Text = myList[myList.Count - 1].Substring(4);
            myList.RemoveAt(myList.Count - 1);

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            for (int i = 0; i < myList.Count; i++)
            {
                myList[i] = regex.Replace(myList[i].Trim(), "#");

                string[] newItem = myList[i].Split('#');
                //var x = newItem[4] != null ? newItem[4] : null;

                var certainValue = newItem.ElementAtOrDefault(4) != null ? newItem[4] : "(Not found.)";

                gBrokenRules.Add(new BrokenRules(newItem[0], newItem[1], newItem[2], newItem[3], certainValue));
            }

            var rulesList = gBrokenRules.Select(x => x.ruleName).Distinct();
            int theLocation = 0;
            int startPoint = 10;
            int startLeftEdge = 10;

            theLocation = CreateRadioButton(theLocation, "Show all", startLeftEdge, startPoint, true, gBrokenRules.Count);

            var listCount = gBrokenRules.Where(x => x.warningLevel.ToLower() == "error".ToLower()).ToList().Count;
            theLocation = CreateRadioButton(theLocation, "Errors only", startLeftEdge, startPoint, false, listCount);

            listCount = gBrokenRules.Where(x => x.warningLevel.ToLower() == "warning".ToLower()).ToList().Count;
            theLocation = CreateRadioButton(theLocation, "Warnings only", startLeftEdge, startPoint, false, listCount);
            theLocation++;

            foreach (var item in rulesList)
            {
                listCount = gBrokenRules.Where(x => x.ruleName.ToLower() == item).ToList().Count;
                theLocation = CreateRadioButton(theLocation, item, startLeftEdge, startPoint, false, listCount);
            }

            var gridStartPoint = CalculateStringLengthPixels();
            dataGridView1.Left = gridStartPoint + 5;
            dataGridView1.Width = this.Width - dataGridView1.Left - 30;
            panel1.Width = gridStartPoint - 10;
            label2.Left = dataGridView1.Left;
            label3.Left = dataGridView1.Left;

            Application.DoEvents();
            return true;
        }

        private int CalculateStringLengthPixels()
        {
            int gridStartPoint = -1;
            foreach (var item in panel1.Controls)
            {
                //var y = ((Control)item).Text;

                using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
                {
                    SizeF size = graphics.MeasureString(((Control)item).Text, new Font("Segoe UI", 11, FontStyle.Regular, GraphicsUnit.Point));
                    if (size.Width > gridStartPoint) gridStartPoint = (int)size.Width;
                }
            }

            return gridStartPoint;
        }

        private int CreateRadioButton(int theLocation, string item, int startLeftEdge, int startPoint, Boolean isChecked, int TheCount)
        {
            RadioButton box;

            box = new RadioButton();
            box.Text = string.Format("{0} ({1})", item, TheCount);
            box.Tag = item;
            box.Checked = isChecked;
            box.AutoSize = true;
            box.Location = new Point(startLeftEdge, theLocation++ * 20 + startPoint); //vertical //box.Location = new Point(i * 50, 10); //horizontal
            box.CheckedChanged += new EventHandler(radioButton_CheckedChanged);

            panel1.Controls.Add(box);

            return theLocation;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton box = (RadioButton)sender;

            Application.DoEvents();

            dataGridView1.DataSource = null;
            if (box.Checked == true)
            {
                if (box.Tag.ToString() == "Show all")
                {
                    dataGridView1.DataSource = gBrokenRules;
                }
                else if (box.Tag.ToString() == "Errors only")
                {
                    dataGridView1.DataSource = gBrokenRules.Where(x => x.warningLevel.ToLower() == "error".ToLower()).ToList();
                }
                else if (box.Tag.ToString() == "Warnings only")
                {
                    dataGridView1.DataSource = gBrokenRules.Where(x => x.warningLevel.ToLower() == "warning".ToLower()).ToList();
                }
                else
                {
                    dataGridView1.DataSource = gBrokenRules.Where(x => x.ruleName.ToLower() == box.Tag.ToString().ToLower()).ToList();
                }

                label3.Text = string.Format("Items in list: {0}", dataGridView1.Rows.Count);
            }
        }

        private void SetDefaultCheckBox()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = gBrokenRules;
            label3.Text = string.Format("Items in list: {0}", dataGridView1.Rows.Count);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string collectedString = string.Empty;
            List<BrokenRules> myBrokenRules = (List<BrokenRules>)dataGridView1.DataSource;
            if (myBrokenRules == null || myBrokenRules.Count == 0)
            {
                MessageBox.Show("No results found.\r\n\r\nNothing was copied to the clipboard.", "No results found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            foreach (var item in myBrokenRules)
            {
                collectedString += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\r\n", item.lineNumber, item.warningLevel, item.ruleName, item.description, item.paths);
            }

            Clipboard.SetText(collectedString);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dialogReturn = OpenFile();
            if (dialogReturn == string.Empty) return;

            if (dialogReturn.ToLower() != textBox1.Text.ToLower().Trim()) dataGridView1.DataSource = null;

            textBox1.Text = dialogReturn;
        }

        private string OpenFile()
        {
            string selectedFileName = string.Empty;

            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"E:\XXDevelopment",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "yaml",
                Filter = "YAML files (*.yaml, *.yml)|*.yaml; *.yml|txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK) selectedFileName = openFileDialog1.FileName;

            return selectedFileName.Trim();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.targetFile = textBox1.Text.Trim();
            Properties.Settings.Default.Save();
        }
    }

    public class BrokenRules
    {
        //The DisplayName is used for column headings in a datagridview.
        [DisplayName("The line location (line:column)")]
        public string lineNumber { get; set; }

        [DisplayName("The warning level")]
        public string warningLevel { get; set; }

        [DisplayName("The rule name")]
        public string ruleName { get; set; }

        [DisplayName("The rule description")]
        public string description { get; set; }

        [DisplayName("The violation path")]
        public string paths { get; set; }

        //Constructor
        public BrokenRules(string item1, string item2, string item3, string item4, string item5)
        {
            this.lineNumber = item1;
            this.warningLevel = item2;
            this.ruleName = item3;
            this.description = item4;
            this.paths = item5;
        }
    }
}