namespace PMLicenseManagement
{
    partial class F_LICENSE_KEY_DETAIL
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvLicenseList = new System.Windows.Forms.DataGridView();
            this.colHWKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActivedDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCurrentDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExpiredDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSoftwareVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLicenseList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLicenseList
            // 
            this.dgvLicenseList.AllowUserToAddRows = false;
            this.dgvLicenseList.AllowUserToDeleteRows = false;
            this.dgvLicenseList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLicenseList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLicenseList.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLicenseList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLicenseList.ColumnHeadersHeight = 40;
            this.dgvLicenseList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLicenseList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHWKey,
            this.colActivedDate,
            this.colCurrentDate,
            this.colExpiredDate,
            this.colSoftwareVersion});
            this.dgvLicenseList.Location = new System.Drawing.Point(12, 16);
            this.dgvLicenseList.MultiSelect = false;
            this.dgvLicenseList.Name = "dgvLicenseList";
            this.dgvLicenseList.ReadOnly = true;
            this.dgvLicenseList.RowHeadersVisible = false;
            this.dgvLicenseList.RowHeadersWidth = 20;
            this.dgvLicenseList.Size = new System.Drawing.Size(1009, 563);
            this.dgvLicenseList.TabIndex = 0;
            // 
            // colHWKey
            // 
            this.colHWKey.DataPropertyName = "HWKey";
            this.colHWKey.HeaderText = "HWKey";
            this.colHWKey.Name = "colHWKey";
            this.colHWKey.ReadOnly = true;
            // 
            // colActivedDate
            // 
            this.colActivedDate.DataPropertyName = "ActivedDate";
            this.colActivedDate.HeaderText = "Actived Date";
            this.colActivedDate.Name = "colActivedDate";
            this.colActivedDate.ReadOnly = true;
            // 
            // colCurrentDate
            // 
            this.colCurrentDate.DataPropertyName = "CurrentDate";
            this.colCurrentDate.HeaderText = "Current Date";
            this.colCurrentDate.Name = "colCurrentDate";
            this.colCurrentDate.ReadOnly = true;
            // 
            // colExpiredDate
            // 
            this.colExpiredDate.DataPropertyName = "ExpiredDate";
            this.colExpiredDate.HeaderText = "Expired Date";
            this.colExpiredDate.Name = "colExpiredDate";
            this.colExpiredDate.ReadOnly = true;
            // 
            // colSoftwareVersion
            // 
            this.colSoftwareVersion.DataPropertyName = "SoftwareVersion";
            this.colSoftwareVersion.HeaderText = "Version";
            this.colSoftwareVersion.Name = "colSoftwareVersion";
            this.colSoftwareVersion.ReadOnly = true;
            // 
            // F_LICENSE_KEY_DETAIL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1033, 591);
            this.Controls.Add(this.dgvLicenseList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "F_LICENSE_KEY_DETAIL";
            this.Text = "License Detail";
            this.Load += new System.EventHandler(this.F_LICENSE_KEY_DETAIL_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLicenseList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLicenseList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHWKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActivedDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCurrentDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colExpiredDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSoftwareVersion;
    }
}

