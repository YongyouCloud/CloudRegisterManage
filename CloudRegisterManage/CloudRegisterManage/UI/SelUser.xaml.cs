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
using System.Windows.Shapes;

namespace CloudRegisterManage.UI
{
    /// <summary>
    /// SelUser.xaml 的交互逻辑
    /// </summary>
    public partial class SelUser : Window
    {

        public Dictionary<string, string> DicAllUserName = new Dictionary<string, string>();//存储友户通ID与U8用户对照关系
        public int CanSelectCount = 0;
        public List<YhtUserInfo> ListUserSel = new List<YhtUserInfo>();
        private List<YhtUserInfo> allUserInfo = new List<YhtUserInfo>();

        public SelUser(string groupId, List<string> filterList)
        {
            InitializeComponent();

            allUserInfo = CloudRegisterUtil.GetYHTAllUserInfo();

            foreach (var user in allUserInfo)
            {
                //if (!user.Groups.Contains(groupId))
                //{
                if (filterList.Contains(user.UserCloudId)) continue;
                ListUserSel.Add(user);
                //}
            }
            ListUserSel = ListUserSel.OrderBy(o => o.U8UsersStr).ToList();
            dgUser.ItemsSource = ListUserSel;

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void cb_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            YhtUserInfo user = dgUser.SelectedItem as YhtUserInfo;
            if (cb.IsChecked == true)
            {
                if (ListUserSel.Count(x => x.Checked) >= CanSelectCount)
                {
                    MessageBox.Show("添加的用户数不能超过该领域分组的许可数");
                    cb.IsChecked = false;
                }
                else
                {
                    user.Checked = true;
                }
            }
            else
            {
                user.Checked = false;
            }


        }

        private void tbSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            if (tbSearch.Text == "搜索用户") tbSearch.Text = "";
        }

        private void btnUnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            List<YhtUserInfo> list = dgUser.ItemsSource as List<YhtUserInfo>;
            foreach (var item in list)
            {
                item.Checked = false;
            }
            dgUser.ItemsSource = null;
            dgUser.ItemsSource = list;
        }

        private void btnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            List<YhtUserInfo> list = dgUser.ItemsSource as List<YhtUserInfo>;
            foreach (var item in list)
            {
                item.Checked = true;
            }
            dgUser.ItemsSource = null;
            dgUser.ItemsSource = list;


        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (tbSearch.Text == "搜索用户") return;
            string str = tbSearch.Text;
            if (string.IsNullOrEmpty(str))
            {
                dgUser.ItemsSource = null;
                ListUserSel = ListUserSel.OrderBy(o => o.U8UsersStr).ToList();
                dgUser.ItemsSource = ListUserSel;
            }
            else
            {
                List<YhtUserInfo> list = ListUserSel.Where(o => o.Mobile.Contains(str) || o.U8UsersStr.Contains(str)).OrderBy(ojb => ojb.U8UsersStr).ToList();
                dgUser.ItemsSource = null;
                dgUser.ItemsSource = list;
            }
        }
    }
}
