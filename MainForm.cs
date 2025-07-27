using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PowerPlanViewer
{
    public partial class MainForm : Form
    {
        private List<PowerPlan> plans = new List<PowerPlan>();

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private CheckBox chkMinimizeToTray;

        public MainForm()
        {
            InitializeComponent();
            this.Icon = new Icon("icon.ico");
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            InitializeTray();
            ApplyDarkTheme();
            LoadPowerPlans();

            this.FormClosing += MainForm_FormClosing;
        }

        private void InitializeTray()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Loading plans...", null); // placeholder
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Exit", null, OnExit);

            trayIcon = new NotifyIcon
            {
                Text = "Power Plan Manager",
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            try
            {
                trayIcon.Icon = new Icon("icon.ico");
            }
            catch
            {
                trayIcon.Icon = SystemIcons.Application;
            }

            trayIcon.DoubleClick += trayIcon_DoubleClick;
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (chkMinimizeToTray.Checked && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();

                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.BalloonTipTitle = "Power Plan Manager";
                trayIcon.BalloonTipText = "App is running in the background.";
                trayIcon.ShowBalloonTip(2000);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
            Application.Exit();
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();  // Prevents ghost tray icons
            base.OnFormClosed(e);
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            listBox1.BackColor = Color.FromArgb(45, 45, 48);
            listBox1.ForeColor = Color.Cyan;
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.Font = new Font("Consolas", 12, FontStyle.Bold);
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.DrawItem += new DrawItemEventHandler(this.listBox1_DrawItem);
            listBox1.ItemHeight = 28;

            listBox1.Width = 400;
            listBox1.Left = (this.ClientSize.Width - listBox1.Width) / 2;

            btnActivate.FlatStyle = FlatStyle.Flat;
            btnActivate.BackColor = Color.FromArgb(60, 60, 60);
            btnActivate.ForeColor = Color.Lime;
            btnActivate.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnActivate.FlatAppearance.BorderColor = Color.Lime;

            btnUltimate.FlatStyle = FlatStyle.Flat;
            btnUltimate.BackColor = Color.FromArgb(60, 60, 60);
            btnUltimate.ForeColor = Color.DeepSkyBlue;
            btnUltimate.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnUltimate.FlatAppearance.BorderColor = Color.DeepSkyBlue;

            chkMinimizeToTray = new CheckBox();
            chkMinimizeToTray.Text = "Minimize to tray";
            chkMinimizeToTray.Checked = true;
            chkMinimizeToTray.ForeColor = Color.White;
            chkMinimizeToTray.BackColor = this.BackColor;
            chkMinimizeToTray.AutoSize = true;
            chkMinimizeToTray.Location = new Point((this.ClientSize.Width - 130), this.ClientSize.Height - 35);
            this.Controls.Add(chkMinimizeToTray);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string itemText = listBox1.Items[e.Index].ToString();
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            e.Graphics.FillRectangle(
                new SolidBrush(isSelected ? Color.DarkSlateGray : listBox1.BackColor),
                e.Bounds
            );

            Color textColor = itemText.StartsWith("[ACTIVE]") ? Color.Lime : listBox1.ForeColor;

            using (var font = new Font("Consolas", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(itemText, font, brush, e.Bounds);
            }
        }

        private void LoadPowerPlans()
        {
            plans.Clear();
            listBox1.Items.Clear();

            var output = RunCommand("powercfg", "/L");

            string pattern = @"Power Scheme GUID: ([a-f0-9\-]+)\s+\(([^)]+)\)(\s+\*)?";
            var matches = Regex.Matches(output, pattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                var plan = new PowerPlan
                {
                    Guid = match.Groups[1].Value,
                    Name = match.Groups[2].Value,
                    IsActive = match.Groups[3].Success
                };
                plans.Add(plan);
                listBox1.Items.Add(plan.ToString());
            }

            trayIcon.Text = $"Power Plan Manager - {plans.FirstOrDefault(p => p.IsActive)?.Name ?? "Unknown"}";
            UpdateTrayPowerPlansMenu();
        }
        private void UpdateTrayPowerPlansMenu()
        {
            while (trayMenu.Items.Count > 2)
                trayMenu.Items.RemoveAt(0);

            foreach (var plan in plans)
            {
                var item = new ToolStripMenuItem(plan.Name)
                {
                    Checked = plan.IsActive,
                    Tag = plan.Guid
                };
                item.Click += (s, e) =>
                {
                    var clicked = s as ToolStripMenuItem;
                    string guid = clicked.Tag.ToString();
                    RunCommand("powercfg", "/S " + guid);
                    LoadPowerPlans();
                };
                trayMenu.Items.Insert(trayMenu.Items.Count - 2, item);
            }
        }


        private string RunCommand(string cmd, string args)
        {
            var process = new Process();
            process.StartInfo.FileName = cmd;
            process.StartInfo.Arguments = args;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a power plan first.");
                return;
            }

            var selectedPlan = plans[listBox1.SelectedIndex];
            RunCommand("powercfg", "/S " + selectedPlan.Guid);
            LoadPowerPlans();

            trayIcon.BalloonTipTitle = "Power Plan Switched";
            trayIcon.BalloonTipText = $"Now using: {selectedPlan.Name}";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.ShowBalloonTip(2000);
        }

        private void btnUltimate_Click(object sender, EventArgs e)
        {
            string ultimateGUID = "e9a42b02-d5df-448d-aa00-03f14749eb61";
            bool alreadyExists = plans.Any(p => p.Guid.Equals(ultimateGUID, StringComparison.OrdinalIgnoreCase) || p.Name.Contains("Ultimate Performance"));

            if (alreadyExists)
            {
                MessageBox.Show("Ultimate Performance plan is already available.");
                return;
            }

            string output = RunCommand("powercfg", "/Duplicatescheme " + ultimateGUID);

            if (output.Contains("GUID") || output.Trim() == "")
            {
                MessageBox.Show("Ultimate Performance plan enabled.");
                LoadPowerPlans();
            }
            else
            {
                MessageBox.Show("Failed to enable Ultimate Performance.\n\nMaybe your system doesn’t support it.");
            }
        }

    }
}
