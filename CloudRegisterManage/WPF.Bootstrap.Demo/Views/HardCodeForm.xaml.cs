using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CloudRegisterManage.Views
{
    /// <summary>
    /// HardCodeForm.xaml 的交互逻辑
    /// </summary>
    public partial class HardCodeForm : UserControl
    {
        private const int BUFFSIZE = 20480;
        private const char RPCDELIMITER = '\u001D';//RPCCALL命令的分割符  

        public HardCodeForm()
        {
            InitializeComponent();

            //string result = GetHardCode();
            //this.txtHardCode.Text = result;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExportHardCode_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "文本文件|*.txt";
            dialog.FileName = "HardCode.txt";
            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, this.txtHardCode.Text);
                MessageBox.Show("保存成功");
            }
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
            char[] cInArray = CloudRegisterUtil.Arrayconcat(cmd, cIn);

            if (CloudRegisterUtil.RPCCall(Environment.MachineName.ToCharArray(), cInArray, cOut) == 0)
            {
                CloudRegisterUtil.NewDecode(cOut, ref cIn);


            }
            return cIn;
        }
    }
}
