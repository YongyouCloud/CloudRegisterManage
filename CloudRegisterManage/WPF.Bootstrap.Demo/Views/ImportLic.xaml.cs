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
    /// ImportLic.xaml 的交互逻辑
    /// </summary>
    public partial class ImportLic : UserControl
    {
        public ImportLic()
        {
            InitializeComponent();
        }

    

        private void btnImportLic_Click(object sender, RoutedEventArgs e)
        {
            byte[] resultData = new byte[23423];
            int resultLength = 0;
            string resultStr = "";
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
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
    }
}
