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

    public partial class HardCodeForm : Form
    {
        private const int BUFFSIZE = 20480;
        private const char RPCDELIMITER = '\u001D';//RPCCALL命令的分割符


        public HardCodeForm()
        {
            InitializeComponent();

            string result = GetHardCode();
            this.txtHardCode.Text = result;
        }

        private string GetHardCode()
        {

            char[] cOut = new char[BUFFSIZE];
            string cIn = string.Empty;
            //bool Connected=false;
            string processId = string.Empty;
            long LogDate = (DateTime.Today.Year % 100) * 100 + DateTime.Today.Month;

            byte[] cmd = { 0x43, 0x6D, 0x64, 0x6D, 0x69, 0x64 };//Cmdmid
            cIn = string.Format("{0}0{0}{1}{0}{2}{0}{3}{0}{4}", RPCDELIMITER, LogDate.ToString(), "UA", "{" + Guid.NewGuid() + "}" + Environment.MachineName + "@1", "0");
            char[] cInArray = arrayconcat(cmd, cIn);

            if (CloudRegisterUtil.RPCCall(Environment.MachineName.ToCharArray(), cInArray, cOut) == 0)
            {
                CloudRegisterUtil.NewDecode(cOut, ref cIn);


            }
            return cIn;
        }


        private static char[] arrayconcat(byte[] array, string str)
        {
            char[] temp = new char[str.Length + array.GetLength(0)];
            Array.Copy(bytetostring(array), 0, temp, 0, array.GetLength(0));
            Array.Copy(str.ToCharArray(), 0, temp, array.GetLength(0), str.Length);
            return temp;
        }

        private static char[] bytetostring(byte[] value)
        {
            char[] chararray = new char[value.GetLength(0)];
            for (int i = 0; i < value.GetLength(0); i++)
            {
                chararray[i] = Convert.ToChar(value[i]);
            }
            return chararray;
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExportHardCode_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "文本文件|*.txt";
            dialog.FileName = "HardCode.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, this.txtHardCode.Text);
                MessageBox.Show("保存成功");
            }


        }
    }
}
