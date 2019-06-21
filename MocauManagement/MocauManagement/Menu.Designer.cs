namespace MocauManagement
{
    partial class Menu
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
            this.btnLicenseList = new System.Windows.Forms.Button();
            this.btnUploadVersion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLicenseList
            // 
            this.btnLicenseList.Location = new System.Drawing.Point(55, 88);
            this.btnLicenseList.Name = "btnLicenseList";
            this.btnLicenseList.Size = new System.Drawing.Size(142, 75);
            this.btnLicenseList.TabIndex = 0;
            this.btnLicenseList.Text = "License List";
            this.btnLicenseList.UseVisualStyleBackColor = true;
            this.btnLicenseList.Click += new System.EventHandler(this.btnLicenseList_Click);
            // 
            // btnUploadVersion
            // 
            this.btnUploadVersion.Location = new System.Drawing.Point(265, 88);
            this.btnUploadVersion.Name = "btnUploadVersion";
            this.btnUploadVersion.Size = new System.Drawing.Size(142, 75);
            this.btnUploadVersion.TabIndex = 1;
            this.btnUploadVersion.Text = "Upgrade Version";
            this.btnUploadVersion.UseVisualStyleBackColor = true;
            this.btnUploadVersion.Click += new System.EventHandler(this.btnUploadVersion_Click);
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 343);
            this.Controls.Add(this.btnUploadVersion);
            this.Controls.Add(this.btnLicenseList);
            this.Name = "Menu";
            this.Text = "Menu";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLicenseList;
        private System.Windows.Forms.Button btnUploadVersion;
    }
}