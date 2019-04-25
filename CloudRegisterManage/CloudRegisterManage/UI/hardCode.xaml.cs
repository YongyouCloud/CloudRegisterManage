using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace CloudRegisterManage
{
    /// <summary>
    /// Interaction logic for SysOptionsView.xaml
    /// </summary>
    public partial class hardCode : UserControl
    {

        private const char RPCDELIMITER = '\u001D';//RPCCALL命令的分割符

        public hardCode()
        {
            InitializeComponent();
            string result = GetHardCode();
            if (result.Contains("failed"))
            {
                string errInfo = CloudRegisterUtil.GetErrInfo(result);
                MessageBox.Show(errInfo);
                return;

            }

            this.txtHardCode.Text = result;
        }

        private string GetHardCode()
        {
            char[] cOut = new char[20480];
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



        private void btnbtnExportHardCode_Click(object sender, RoutedEventArgs e)
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
    }
}
