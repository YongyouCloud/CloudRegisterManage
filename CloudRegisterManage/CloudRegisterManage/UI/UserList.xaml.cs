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
    public partial class UserList : UserControl
    {
        private const char RPCDELIMITER = '\u001D';//RPCCALL命令的分割符
        public List<YhtUserInfo> allUserInfo = new List<YhtUserInfo>();


        public UserList()
        {
            InitializeComponent();
            allUserInfo = CloudRegisterUtil.GetYHTAllUserInfo();
            ShowUserList();
        }

        private void ShowUserList()
        {
            string companyInfo = GetCompanyInfo();
            if (companyInfo == "" || companyInfo == ",null,,,," || companyInfo.Contains("failed:"))
            {
                return;
            }

            string type = companyInfo.Split(',')[1];
            //string mode = "";
            switch (type)
            {
                case "online":
                    {
                        //mode = "在线加密";
                        break;
                    }
                case "offline":
                    {
                        //mode = "离线加密";
                        dgUserList.Columns[1].Visibility = Visibility.Hidden;
                        dgUserList.Columns[3].Visibility = Visibility.Hidden;
                        break;
                    }
                case "adviser":
                    {
                        //mode = "顾问许可"; //(在线的一种)
                        break;
                    }
                case "onlineborrow":
                    {
                        //mode = "在线借用";
                        break;
                    }
                case "offlineborrow":
                    {
                        //mode = "离线借用";
                        dgUserList.Columns[1].Visibility = Visibility.Hidden;
                        dgUserList.Columns[3].Visibility = Visibility.Hidden;
                        break;
                    }
                case "ontrial":
                    {
                        // mode = "试用许可"; //(在线的一种)
                        break;
                    }
                default:
                    {
                        // mode = "在线加密";
                        break;
                    }
            }
            dgUserList.ItemsSource = allUserInfo;
            TbNum.Text = "总数:" + allUserInfo.Count;
        }

        private string GetCompanyInfo()
        {
            char[] resultData = new char[2342300];
            int resultLength = 0;
            string resultStr = "";
            CloudRegisterUtil.GetYhtCompany(Environment.MachineName.ToCharArray(), resultData, resultLength);

            CloudRegisterUtil.NewDecode(resultData, ref resultStr);

            return resultStr;
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            string str = tbSearch.Text == "搜索用户" ? "" : tbSearch.Text;
            if (string.IsNullOrEmpty(str))
            {
                ShowUserList();
            }
            else
            {
                var list = allUserInfo.Where(o => o.UserCloudId.Contains(str) || o.UserCloudName.Contains(str) || o.Mobile.Contains(str) || o.U8UsersStr.Contains(str)).ToList();
                list = list.OrderBy(o => o.U8UsersStr).ToList();

                dgUserList.ItemsSource = list;
            }
        }


        private void tbSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            if (tbSearch.Text == "搜索用户") tbSearch.Text = "";

        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            CloudRegisterUtil.ExportDataGridSaveAs(false, dgUserList);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            allUserInfo = CloudRegisterUtil.GetYHTAllUserInfo();
            ShowUserList();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbSearch.Text == "搜索用户") return;
            string str = tbSearch.Text;
            if (string.IsNullOrEmpty(str))
            {
                ShowUserList();
            }
            else
            {
                var list = allUserInfo.Where(o => o.UserCloudId.Contains(str) || o.UserCloudName.Contains(str) || o.Mobile.Contains(str) || o.U8UsersStr.Contains(str)).ToList();
                list = list.OrderBy(o => o.U8UsersStr).ToList();

                dgUserList.ItemsSource = list;
            }
        }
    }



}
