using System;
using System.Collections.Generic;
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
    /// LicUseInfo.xaml 的交互逻辑
    /// </summary>
    public partial class LicUseInfo : UserControl
    {
        public LicUseInfo()
        {
            InitializeComponent();
            initTv();
        }

        private void initTv()
        {

            TreeViewItem item1 = new TreeViewItem() { Header = "U8V13.1产品卡(在线)" };
         
            TreeViewItem item11 = new TreeViewItem() { Header = "领域" };
            item11.Items.Add("供应链领域[1/2]");
            item11.Items.Add("生产制造领域[1/2]");
            item1.Items.Add(item11);
            tvPdrduct.Items.Add(item1);
          

        }
    }
}
