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
        private const string Pattern = "0C00000037392E3131302E38342E373500000000";
        private const string PortPattern = "00A00F0000A10F0000A20F0000A00F0000A00F0000A00F0000A30F0000000000000000000000000000";

        private string selectedFilePath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "MulticlientCreator by Fizo";
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
                string port = txtPort.Text.Trim();

                if (!IsValidPort(port) || !IsIpValid(ip))
                {
                    MessageBox.Show("Please enter a valid IP address and port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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
                if (finder.ReplaceIpPattern(Pattern, PortPattern))
                {
                    if (File.Exists(nostalePath))
                    {
                        File.Delete(nostalePath);
                    }
                    File.Move(tempPath, nostalePath);
                    System.Threading.Thread.Sleep(100);
                    CreateShortcut(nostalePath, newFileName);
                    MessageBox.Show($"Multiclient \"{newFileName}\" has been successfully generated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (File.Exists(tempPath)) File.Delete(tempPath);
                    MessageBox.Show("Failed to modify IP and port in the copied file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateShortcut(string targetPath, string fileName)
        {
            try
            {
                string shortcutLocation = Path.Combine(Path.GetDirectoryName(targetPath), $"{fileName}.lnk");
                Type t = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(t);
                dynamic shortcut = shell.CreateShortcut(shortcutLocation);

                shortcut.TargetPath = targetPath;
                shortcut.Arguments = "\"EntwellNostaleClient\"";
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating shortcut: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsIpValid(string ipAddress) => IPAddress.TryParse(ipAddress, out _);

        private bool IsValidPort(string port)
        {
            if (int.TryParse(port, out int portNumber))
            {
                return portNumber >= 0 && portNumber <= 65535;
            }
            return false;
        }
        private byte[] ReadBytesFromFile(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return DeserializationHelper.ReadFully(fileStream);
            }
        }

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
                txtIP.ForeColor = SystemColors.GrayText;
            }
        }
        private void txtPort_Enter(object sender, EventArgs e)
        {
            if (txtPort.Text == "Enter port")
            {
                txtPort.Text = "";
                txtPort.ForeColor = SystemColors.GrayText;
            }
        }
        private void txtIP_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIP.Text))
            {
                txtIP.Text = "Enter IP address";
                txtIP.ForeColor = SystemColors.GrayText;
            }
        }
        private void txtPort_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPort.Text))
            {
                txtPort.Text = "Enter port";
                txtPort.ForeColor = SystemColors.GrayText;
            }
        }
    }
}