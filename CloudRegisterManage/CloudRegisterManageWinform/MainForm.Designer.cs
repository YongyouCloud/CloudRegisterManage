namespace CloudRegisterManage
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnImportLicence = new System.Windows.Forms.Button();
            this.btnProductReg = new System.Windows.Forms.Button();
            this.btnShowHardCode = new System.Windows.Forms.Button();
            this.btnShowLicUseInfo = new System.Windows.Forms.Button();
            this.btnAssignLic = new System.Windows.Forms.Button();
            this.btnBackupLic = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnImportLicence
            // 
            this.btnImportLicence.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(192)))), ((int)(((byte)(222)))));
            this.btnImportLicence.Location = new System.Drawing.Point(260, 67);
            this.btnImportLicence.Name = "btnImportLicence";
            this.btnImportLicence.Size = new System.Drawing.Size(117, 40);
            this.btnImportLicence.TabIndex = 0;
            this.btnImportLicence.Text = "导入许可";
            this.btnImportLicence.UseVisualStyleBackColor = false;
            this.btnImportLicence.Click += new System.EventHandler(this.btnImportLicence_Click);
            // 
            // btnProductReg
            // 
            this.btnProductReg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(192)))), ((int)(((byte)(222)))));
            this.btnProductReg.Location = new System.Drawing.Point(83, 67);
            this.btnProductReg.Name = "btnProductReg";
            this.btnProductReg.Size = new System.Drawing.Size(117, 40);
            this.btnProductReg.TabIndex = 1;
            this.btnProductReg.Text = "注册产品";
            this.btnProductReg.UseVisualStyleBackColor = false;
            this.btnProductReg.Click += new System.EventHandler(this.btnProductReg_Click);
            // 
            // btnShowHardCode
            // 
            this.btnShowHardCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(192)))), ((int)(((byte)(222)))));
            this.btnShowHardCode.Location = new System.Drawing.Point(260, 188);
            this.btnShowHardCode.Name = "btnShowHardCode";
            this.btnShowHardCode.Size = new System.Drawing.Size(117, 42);
            this.btnShowHardCode.TabIndex = 2;
            this.btnShowHardCode.Text = "查看特征码";
            this.btnShowHardCode.UseVisualStyleBackColor = false;
            this.btnShowHardCode.Click += new System.EventHandler(this.btnShowHadrCode_Click);
            // 
            // btnShowLicUseInfo
            // 
            this.btnShowLicUseInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(192)))), ((int)(((byte)(222)))));
            this.btnShowLicUseInfo.Location = new System.Drawing.Point(432, 188);
            this.btnShowLicUseInfo.Name = "btnShowLicUseInfo";
            this.btnShowLicUseInfo.Size = new System.Drawing.Size(117, 42);
            this.btnShowLicUseInfo.TabIndex = 3;
            this.btnShowLicUseInfo.Text = "查看许可占用情况";
            this.btnShowLicUseInfo.UseVisualStyleBackColor = false;
            this.btnShowLicUseInfo.Click += new System.EventHandler(this.btnShowLicUseInfo_Click);
            // 
            // btnAssignLic
            // 
            this.btnAssignLic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(192)))), ((int)(((byte)(222)))));
            this.btnAssignLic.Location = new System.Drawing.Point(432, 67);
            this.btnAssignLic.Name = "btnAssignLic";
            this.btnAssignLic.Size = new System.Drawing.Size(117, 40);
            this.btnAssignLic.TabIndex = 4;
            this.btnAssignLic.Text = "分配许可";
            this.btnAssignLic.UseVisualStyleBackColor = false;
            this.btnAssignLic.Click += new System.EventHandler(this.btnAssignLic_Click);
            // 
            // btnBackupLic
            // 
            this.btnBackupLic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(192)))), ((int)(((byte)(222)))));
            this.btnBackupLic.Location = new System.Drawing.Point(83, 188);
            this.btnBackupLic.Name = "btnBackupLic";
            this.btnBackupLic.Size = new System.Drawing.Size(117, 42);
            this.btnBackupLic.TabIndex = 5;
            this.btnBackupLic.Text = "备份许可";
            this.btnBackupLic.UseVisualStyleBackColor = false;
            this.btnBackupLic.Click += new System.EventHandler(this.btnBackupLic_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(634, 361);
            this.Controls.Add(this.btnBackupLic);
            this.Controls.Add(this.btnAssignLic);
            this.Controls.Add(this.btnShowLicUseInfo);
            this.Controls.Add(this.btnShowHardCode);
            this.Controls.Add(this.btnProductReg);
            this.Controls.Add(this.btnImportLicence);
            this.Name = "MainForm";
            this.Text = "云注册管理";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnImportLicence;
        private System.Windows.Forms.Button btnProductReg;
        private System.Windows.Forms.Button btnShowHardCode;
        private System.Windows.Forms.Button btnShowLicUseInfo;
        private System.Windows.Forms.Button btnAssignLic;
        private System.Windows.Forms.Button btnBackupLic;
    }
}

