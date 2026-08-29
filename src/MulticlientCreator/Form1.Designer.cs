namespace MulticlientCreator
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblNostalePath = new Label();
            txtNostalePath = new TextBox();
            btnBrowse = new Button();
            lblIP = new Label();
            txtIP = new TextBox();
            lblLanguage = new Label();
            cboLanguage = new ComboBox();
            lblFileName = new Label();
            txtFileName = new TextBox();
            btnGenerate = new Button();
            SuspendLayout();
            //
            // lblNostalePath
            //
            lblNostalePath.AutoSize = true;
            lblNostalePath.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNostalePath.Location = new Point(16, 74);
            lblNostalePath.Name = "lblNostalePath";
            lblNostalePath.Text = "NostaleClientX.exe";
            //
            // txtNostalePath
            //
            txtNostalePath.Location = new Point(16, 94);
            txtNostalePath.Name = "txtNostalePath";
            txtNostalePath.Size = new Size(420, 23);
            txtNostalePath.TabIndex = 0;
            //
            // btnBrowse
            //
            btnBrowse.Location = new Point(444, 93);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(80, 25);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "Browse...";
            btnBrowse.Click += btnBrowse_Click;
            //
            // lblIP
            //
            lblIP.AutoSize = true;
            lblIP.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblIP.Location = new Point(16, 137);
            lblIP.Name = "lblIP";
            lblIP.Text = "IP";
            //
            // txtIP
            //
            txtIP.Location = new Point(110, 134);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(220, 23);
            txtIP.TabIndex = 2;
            txtIP.Text = "Enter IP address";
            txtIP.Enter += txtIP_Enter;
            txtIP.Leave += txtIP_Leave;
            //
            // lblLanguage
            //
            lblLanguage.AutoSize = true;
            lblLanguage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLanguage.Location = new Point(16, 173);
            lblLanguage.Name = "lblLanguage";
            lblLanguage.Text = "Language";
            //
            // cboLanguage
            //
            cboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLanguage.Location = new Point(110, 170);
            cboLanguage.Name = "cboLanguage";
            cboLanguage.Size = new Size(220, 23);
            cboLanguage.TabIndex = 3;
            // Order is the client region index and must match the emulator's RegionLanguageType enum
            // (EN=0, DE=1, FR=2, IT=3, PL=4, ES=5, CZ=6, RU=7, TR=8): the index is both the login
            // port offset (4000 + index) and the region byte the client sends in its login packet.
            cboLanguage.Items.AddRange(new object[] { "UK (English)", "DE (Deutsch)", "FR (Français)", "IT (Italiano)", "PL (Polski)", "ES (Español)", "CZ (Čeština)", "RU (Русский)", "TR (Türkçe)" });
            cboLanguage.SelectedIndex = 2;
            //
            // lblFileName
            //
            lblFileName.AutoSize = true;
            lblFileName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFileName.Location = new Point(16, 209);
            lblFileName.Name = "lblFileName";
            lblFileName.Text = "File Name";
            //
            // txtFileName
            //
            txtFileName.Location = new Point(110, 206);
            txtFileName.Name = "txtFileName";
            txtFileName.Size = new Size(330, 23);
            txtFileName.TabIndex = 4;
            //
            // btnGenerate
            //
            btnGenerate.Location = new Point(16, 250);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(508, 42);
            btnGenerate.TabIndex = 5;
            btnGenerate.Text = "Generate Multiclient";
            btnGenerate.Click += btnGenerate_Click;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(540, 314);
            Controls.Add(btnGenerate);
            Controls.Add(txtFileName);
            Controls.Add(lblFileName);
            Controls.Add(cboLanguage);
            Controls.Add(lblLanguage);
            Controls.Add(txtIP);
            Controls.Add(lblIP);
            Controls.Add(btnBrowse);
            Controls.Add(txtNostalePath);
            Controls.Add(lblNostalePath);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Multiclient Creator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNostalePath;
        private TextBox txtNostalePath;
        private Button btnBrowse;
        private Label lblIP;
        private TextBox txtIP;
        private Label lblLanguage;
        private ComboBox cboLanguage;
        private Label lblFileName;
        private TextBox txtFileName;
        private Button btnGenerate;
    }
}
