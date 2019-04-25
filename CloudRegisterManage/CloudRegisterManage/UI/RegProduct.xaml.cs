using Microsoft.Win32;
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

namespace CloudRegisterManage
{
    /// <summary>
    /// Interaction logic for SysOptionsView.xaml
    /// </summary>
    public partial class RegProduct : UserControl
    {


        public RegProduct()
        {
            InitializeComponent();

        }

        private void btnRegProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object serverReg = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Ufsoft\WF\V8.700\ServerReg", "YonyouYhtSrv", "");
                if (serverReg != null)
                {
                    string yhtUrl = serverReg.ToString();
                    if (string.IsNullOrEmpty(yhtUrl))
                    {
                        MessageBox.Show("未获取友户通地址，请到U8应用服务管理器，加密服务配置项中注册友户通地址");
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(yhtUrl + "/apptenant");
                    }
                }
                else
                {
                    MessageBox.Show("未获取友户通地址，请到U8应用服务管理器，加密服务配置项中注册友户通地址");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }

        }
    }
}
