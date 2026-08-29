using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using MulticlientCreator.Helpers;

namespace MulticlientCreator
{
    public partial class Form1 : Form
    {
        private const string OfficialName = "NostaleClientX.exe";
        private const int BaseLoginPort = 4000;
        private const string Pattern = "0C00000037392E3131302E38342E373500000000";

        private string selectedFilePath = "";

        public Form1()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            SeovaTheme.Apply(this);
            BackgroundImage = null;
            Font = new Font("Segoe UI", 9f);

            Controls.Add(SeovaTheme.Header("Multiclient Creator", "IP · Language"));

            foreach (var lbl in new[] { lblNostalePath, lblIP, lblFileName, lblLanguage })
            {
                lbl.ForeColor = SeovaTheme.Fg;
                lbl.BackColor = Color.Transparent;
            }

            foreach (var tb in new[] { txtNostalePath, txtIP, txtFileName })
            {
                tb.BackColor = SeovaTheme.Input;
                tb.ForeColor = SeovaTheme.Fg;
                tb.BorderStyle = BorderStyle.FixedSingle;
            }

            txtIP.ForeColor = SeovaTheme.Dim;

            cboLanguage.BackColor = SeovaTheme.Input;
            cboLanguage.ForeColor = SeovaTheme.Fg;
            cboLanguage.FlatStyle = FlatStyle.Flat;

            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.FlatAppearance.MouseOverBackColor = SeovaTheme.Bar;
            btnBrowse.BackColor = SeovaTheme.Input;
            btnBrowse.ForeColor = SeovaTheme.Fg;
            btnBrowse.UseVisualStyleBackColor = false;

            var gen = SeovaTheme.Button("Generate Multiclient", btnGenerate.Width, btnGenerate.Height);
            gen.Location = btnGenerate.Location;
            gen.Anchor = btnGenerate.Anchor;
            gen.Click += btnGenerate_Click;
            Controls.Remove(btnGenerate);
            Controls.Add(gen);
            gen.BringToFront();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Executable files|NostaleClientX.exe";
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    selectedFilePath = dialog.FileName;
                    txtNostalePath.Text = selectedFilePath;
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string nostalePath = selectedFilePath;

            if (string.IsNullOrEmpty(nostalePath) || !File.Exists(nostalePath))
            {
                MessageBox.Show("Invalid NostaleClientX.exe file selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var processes = System.Diagnostics.Process.GetProcessesByName("NostaleClientX");
                if (processes.Length > 0)
                {
                    MessageBox.Show("Please close NostaleClientX.exe before modifying it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var fileBytes = File.ReadAllBytes(nostalePath);
                var fileHex = BitConverter.ToString(fileBytes).Replace("-", "");

                if (!fileHex.Contains(Pattern))
                {
                    MessageBox.Show("Please select an original NostaleClientX.exe file. The selected file is already modified.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string ip = txtIP.Text.Trim();
                if (!IsIpValid(ip))
                {
                    MessageBox.Show("Please enter a valid IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int languageIndex = cboLanguage.SelectedIndex < 0 ? 0 : cboLanguage.SelectedIndex;
                string port = (BaseLoginPort + languageIndex).ToString();

                string newFileName = txtFileName.Text.Trim();
                if (string.IsNullOrEmpty(newFileName))
                {
                    MessageBox.Show("Please enter a file name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string tempPath = Path.Combine(Path.GetDirectoryName(nostalePath), "temp.exe");

                using (FileStream fs = new FileStream(nostalePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (FileStream tempFs = File.Create(tempPath))
                    {
                        fs.CopyTo(tempFs);
                    }
                }

                var newIpPattern = GenerateIpPattern(ip);
                var newPortPattern = GeneratePortPattern(port);

                var finder = new HexFinder(tempPath, newIpPattern, newPortPattern);
                var patch = finder.ReplaceIpPattern(Pattern);

                // Never move a half-patched client into place. An unpatched port table leaves the
                // client dialing its stock port (often 4000) whatever language was picked, which on
                // the player's side looks exactly like "the client cannot find the server".
                if (!patch.Success)
                {
                    if (File.Exists(tempPath)) File.Delete(tempPath);

                    string detail;
                    if (!patch.IpPatched && !patch.PortPatched)
                        detail = "Neither the IP address nor the login port table could be located.";
                    else if (!patch.IpPatched)
                        detail = "The login port was patched but the IP address could not be located.";
                    else
                        detail = "The IP address was patched but the login port table could not be located, "
                               + $"so the client would keep dialing its stock port instead of {port}.";

                    MessageBox.Show(
                        $"{detail}\n\nThe file was left untouched. It is most likely not an original "
                        + $"{OfficialName}, or it is a build this tool does not recognise.",
                        "Patch failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (File.Exists(nostalePath))
                {
                    File.Delete(nostalePath);
                }
                File.Move(tempPath, nostalePath);
                System.Threading.Thread.Sleep(100);
                CreateShortcut(nostalePath, newFileName, languageIndex);
                MessageBox.Show(
                    $"Multiclient \"{newFileName}\" has been successfully generated!\n\n"
                    + $"Endpoint : {ip}:{port}\n"
                    + $"Language : {cboLanguage.SelectedItem} (index {languageIndex})",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateShortcut(string targetPath, string fileName, int languageIndex = 0)
        {
            try
            {
                string shortcutLocation = Path.Combine(Path.GetDirectoryName(targetPath), $"{fileName}.lnk");
                Type t = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(t);
                dynamic shortcut = shell.CreateShortcut(shortcutLocation);

                shortcut.TargetPath = targetPath;
                shortcut.Arguments = $"\"EntwellNostaleClient\" {languageIndex}";
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating shortcut: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsIpValid(string ipAddress) => IPAddress.TryParse(ipAddress, out _);

        private string GeneratePortPattern(string basePort)
        {
            int port = int.Parse(basePort);
            string hexPort = port.ToString("X4");
            string hexPortRearranged = hexPort.Substring(2, 2) + hexPort[..2];
            var builder = new StringBuilder();

            builder.Append("00");

            for (int i = 0; i < 7; i++)
            {
                builder.Append(hexPortRearranged).Append("0000");
            }

            builder.Append("000000000000000000000000");

            return builder.ToString();
        }

        private string GenerateIpPattern(string ip)
        {
            var split = ip.Split('.');
            var builder = new StringBuilder();
            builder.Append("0" + Convert.ToString(ip.Length, 16).ToUpper() + "000000");

            for (var i = 0; i < 4; i++)
            {
                builder.Append(HexHelper.ToHexString(split[i]));

                if (i == 3) break;

                builder.Append("2E");
            }

            for (var j = builder.Length; j < 40; j++)
            {
                builder.Append("0");
            }

            return builder.ToString();
        }

        private void txtIP_Enter(object sender, EventArgs e)
        {
            if (txtIP.Text == "Enter IP address")
            {
                txtIP.Text = "";
                txtIP.ForeColor = SeovaTheme.Fg;
            }
        }

        private void txtIP_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIP.Text))
            {
                txtIP.Text = "Enter IP address";
                txtIP.ForeColor = SeovaTheme.Dim;
            }
        }
    }
}
