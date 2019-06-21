namespace MocauManagement
{
    partial class F_UPLOAD_VERSION
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbCustomer = new System.Windows.Forms.ComboBox();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.txtLocalPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowser = new System.Windows.Forms.Button();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.txtCurrentVersion = new System.Windows.Forms.TextBox();
            this.lblUpgradeVersion = new System.Windows.Forms.Label();
            this.txtUploadedVersion = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // cbCustomer
            // 
            this.cbCustomer.FormattingEnabled = true;
            this.cbCustomer.Location = new System.Drawing.Point(127, 17);
            this.cbCustomer.Name = "cbCustomer";
            this.cbCustomer.Size = new System.Drawing.Size(121, 21);
            this.cbCustomer.TabIndex = 0;
            this.cbCustomer.SelectedIndexChanged += new System.EventHandler(this.cbCustomer_SelectedIndexChanged);
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(12, 20);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(95, 13);
            this.lblCustomer.TabIndex = 1;
            this.lblCustomer.Text = "Chọn Khách Hàng";
            // 
            // txtLocalPath
            // 
            this.txtLocalPath.Enabled = false;
            this.txtLocalPath.Location = new System.Drawing.Point(127, 65);
            this.txtLocalPath.Name = "txtLocalPath";
            this.txtLocalPath.Size = new System.Drawing.Size(222, 20);
            this.txtLocalPath.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Local Path";
            // 
            // btnBrowser
            // 
            this.btnBrowser.Location = new System.Drawing.Point(380, 65);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(93, 23);
            this.btnBrowser.TabIndex = 4;
            this.btnBrowser.Text = "Browser...";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Location = new System.Drawing.Point(12, 118);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(79, 13);
            this.lblCurrentVersion.TabIndex = 6;
            this.lblCurrentVersion.Text = "Current Version";
            // 
            // txtCurrentVersion
            // 
            this.txtCurrentVersion.Enabled = false;
            this.txtCurrentVersion.Location = new System.Drawing.Point(127, 115);
            this.txtCurrentVersion.Name = "txtCurrentVersion";
            this.txtCurrentVersion.Size = new System.Drawing.Size(222, 20);
            this.txtCurrentVersion.TabIndex = 5;
            // 
            // lblUpgradeVersion
            // 
            this.lblUpgradeVersion.AutoSize = true;
            this.lblUpgradeVersion.Location = new System.Drawing.Point(12, 162);
            this.lblUpgradeVersion.Name = "lblUpgradeVersion";
            this.lblUpgradeVersion.Size = new System.Drawing.Size(86, 13);
            this.lblUpgradeVersion.TabIndex = 8;
            this.lblUpgradeVersion.Text = "Upgrade Version";
            // 
            // txtUploadedVersion
            // 
            this.txtUploadedVersion.Location = new System.Drawing.Point(127, 159);
            this.txtUploadedVersion.Name = "txtUploadedVersion";
            this.txtUploadedVersion.Size = new System.Drawing.Size(222, 20);
            this.txtUploadedVersion.TabIndex = 7;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(103, 222);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(145, 47);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(305, 222);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(145, 47);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(77, 316);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(438, 23);
            this.progressBar1.TabIndex = 11;
            // 
            // F_UPLOAD_VERSION
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 398);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblUpgradeVersion);
            this.Controls.Add(this.txtUploadedVersion);
            this.Controls.Add(this.lblCurrentVersion);
            this.Controls.Add(this.txtCurrentVersion);
            this.Controls.Add(this.btnBrowser);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLocalPath);
            this.Controls.Add(this.lblCustomer);
            this.Controls.Add(this.cbCustomer);
            this.Name = "F_UPLOAD_VERSION";
            this.Text = "F_UPLOAD_VERSION";
            this.Load += new System.EventHandler(this.F_UPLOAD_VERSION_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbCustomer;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.TextBox txtLocalPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.TextBox txtCurrentVersion;
        private System.Windows.Forms.Label lblUpgradeVersion;
        private System.Windows.Forms.TextBox txtUploadedVersion;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}