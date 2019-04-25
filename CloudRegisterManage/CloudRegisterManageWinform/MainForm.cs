using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CloudRegisterManage
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnImportLicence_Click(object sender, EventArgs e)
        {

            byte[] resultData = new byte[23423];
            int resultLength = 0;
            string resultStr = "";
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                byte[] data = File.ReadAllBytes(dialog.FileName);
                CloudRegisterUtil.ImportYhtLic(Environment.MachineName.ToCharArray(), data, data.Length, resultData, resultLength);

                string result = System.Text.Encoding.Default.GetString(resultData);     //Pass:import yht_lic_file succ.
                CloudRegisterUtil.NewDecode(result.ToArray(), ref resultStr);

                if (resultStr.Contains("succ"))
                {
                    MessageBox.Show("导入成功");
                }
                else
                {
                    MessageBox.Show(resultStr);
                }


            }

        }

        private void btnProductReg_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://uastest.yyuap.com/");
        }

        private void btnShowLicUseInfo_Click(object sender, EventArgs e)
        {
            LicUseViewForm form = new LicUseViewForm();
            form.ShowDialog();
        }

        private void btnShowHadrCode_Click(object sender, EventArgs e)
        {
            HardCodeForm hardCodeForm = new HardCodeForm();
            hardCodeForm.ShowDialog();
        }

        private void btnAssignLic_Click(object sender, EventArgs e)
        {

        }

        private void btnBackupLic_Click(object sender, EventArgs e)
        {

        }
    }
}
