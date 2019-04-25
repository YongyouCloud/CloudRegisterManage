namespace CloudRegisterManage
{
    partial class HardCodeForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtHardCode = new System.Windows.Forms.TextBox();
            this.btnExportHardCode = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "本机特征码";
            // 
            // txtHardCode
            // 
            this.txtHardCode.Location = new System.Drawing.Point(126, 26);
            this.txtHardCode.Name = "txtHardCode";
            this.txtHardCode.Size = new System.Drawing.Size(334, 21);
            this.txtHardCode.TabIndex = 1;
            // 
            // btnExportHardCode
            // 
            this.btnExportHardCode.Location = new System.Drawing.Point(466, 25);
            this.btnExportHardCode.Name = "btnExportHardCode";
            this.btnExportHardCode.Size = new System.Drawing.Size(75, 23);
            this.btnExportHardCode.TabIndex = 2;
            this.btnExportHardCode.Text = "导出特征码";
            this.btnExportHardCode.UseVisualStyleBackColor = true;
            this.btnExportHardCode.Click += new System.EventHandler(this.btnExportHardCode_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(466, 152);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // HardCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 203);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnExportHardCode);
            this.Controls.Add(this.txtHardCode);
            this.Controls.Add(this.label1);
            this.Name = "HardCodeForm";
            this.Text = "硬件特征码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHardCode;
        private System.Windows.Forms.Button btnExportHardCode;
        private System.Windows.Forms.Button btnClose;
    }
}