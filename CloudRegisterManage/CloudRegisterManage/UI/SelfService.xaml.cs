using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
//using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.Win32;

namespace CloudRegisterManage
{
    /// <summary>
    /// Interaction logic for SysOptionsView.xaml
    /// </summary>
    public partial class SelfService : UserControl
    {
    
        public SelfService()
        {
            InitializeComponent();
         
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Hyperlink link = sender as Hyperlink;
                Process.Start(new ProcessStartInfo("http://fwq.yonyou.com/up_service/index.php?r=up_mask/home&topic_id=27"));
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            
        }
    }



}
