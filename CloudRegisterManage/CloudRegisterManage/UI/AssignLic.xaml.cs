using CloudRegisterManage.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
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

namespace CloudRegisterManage
{
    /// <summary>
    /// 分配许可
    /// </summary>
    public partial class AssignLic : UserControl
    {

        private Dictionary<string, string> DicModuleMap = new Dictionary<string, string>();//U8模块和商务模块的对应关系  key=U8模块
        private Dictionary<string, string> DicLicInfo = new Dictionary<string, string>();//加密信息 和 模块购买点数
        private Dictionary<string, int> DicLicUserInfo = new Dictionary<string, int>();//加密点数分配 信息
        //private Dictionary<string, string> DicAllUserName = new Dictionary<string, string>();//存储友户通ID与U8用户对照关系
        private HashSet<string> ChangedUser = new HashSet<string>();//有改动的用户
        //private Dictionary<string, HashSet<string>> userModuleMap = new Dictionary<string, HashSet<string>>();//保存界面上选择的用户（保存用）
        private Dictionary<string, List<YhtUserInfo>> moduleUserMap = new Dictionary<string, List<YhtUserInfo>>();//保存界面上选择的用户(显示用)
        private List<AuthGroup> listAuthGroup = new List<AuthGroup>();

        private List<YhtUserInfo> allUserInfo = new List<YhtUserInfo>();

        Dictionary<string, string> dicGroupTitle = CloudRegisterUtil.GetGroupTitle(); //分组名称

        public AssignLic()
        {
            InitializeComponent();
            Init();
        }

        private void btnRegProduct_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://uastest.yyuap.com/");
        }

        private void Init(string groupId = "")
        {
            //userModuleMap.Clear();
            ChangedUser.Clear();
            moduleUserMap.Clear();
            allUserInfo = CloudRegisterUtil.GetYHTAllUserInfo();
            dicGroupTitle = CloudRegisterUtil.GetGroupTitle();
            DicLicInfo = GetLicInfo();
            DicModuleMap = GetModuleMap();
            listAuthGroup = CloudRegisterUtil.GetAuthGroupInfo();
            DicLicUserInfo = GetLicUserInfo();
            InitTv(groupId);
        }



        private void InitTv(string groupId)
        {
            tvlic.Items.Clear();
            string companyInfo = GetCompanyInfo();
            if (companyInfo == "" || companyInfo == ",null,,,," || companyInfo.Contains("failed:"))
            {
                return;
            }
            string type = companyInfo.Split(',')[1];
            string mode = "";
            switch (type)
            {
                case "online":
                    {
                        mode = "在线正式加密";
                        btnSave.IsEnabled = false;
                        btnSelUser.IsEnabled = false;
                        btnDelUser.IsEnabled = false;

                        break;
                    }
                case "offline":
                    {
                        mode = "离线正式加密";
                        btnSave.IsEnabled = true;
                        btnSelUser.IsEnabled = true;
                        btnDelUser.IsEnabled = true;
                        dgUser.Columns[1].Visibility = Visibility.Hidden;
                        dgUser.Columns[4].Visibility = Visibility.Hidden;
                        break;
                    }
                case "adviser":
                    {
                        mode = "顾问加密"; //(在线的一种)
                        btnSave.IsEnabled = false;
                        btnSelUser.IsEnabled = false;
                        btnDelUser.IsEnabled = false;
                        break;
                    }
                case "onlineborrow":
                    {
                        mode = "在线借用";
                        btnSave.IsEnabled = false;
                        btnSelUser.IsEnabled = false;
                        btnDelUser.IsEnabled = false;
                        break;
                    }
                case "offlineborrow":
                    {
                        mode = "离线借用";
                        btnSave.IsEnabled = true;
                        btnSelUser.IsEnabled = true;
                        btnDelUser.IsEnabled = true;
                        dgUser.Columns[1].Visibility = Visibility.Hidden;
                        dgUser.Columns[4].Visibility = Visibility.Hidden;
                        break;
                    }
                case "ontrial":
                    {
                        mode = "试用许可"; //(在线的一种)
                        btnSave.IsEnabled = false;
                        btnSelUser.IsEnabled = false;
                        btnDelUser.IsEnabled = false;
                        btnCancel.IsEnabled = false;
                        tbSearch.IsEnabled = false;
                        return; //如果当前环境使用的是产品试用许可，该界面不显示任何内容
                    }
                default:
                    {

                        mode = "在线正式加密";
                        btnSave.IsEnabled = false;
                        btnSelUser.IsEnabled = false;
                        btnDelUser.IsEnabled = false;
                        break;
                    }
            }

            string rc = "";
            if (DicLicInfo.Keys.Contains("RC")) rc = DicLicInfo["RC"];

            TreeViewItem itemRoot = new TreeViewItem();
            itemRoot.Tag = "";

            itemRoot.IsExpanded = true;

            if (rc != "")
            {
                itemRoot.Header = "U8V" + rc.Substring(0, 2) + "." + rc.Substring(2, 1) + "产品卡(" + mode + ")";
            }
            else
            {
                itemRoot.Header = "U8V" + "产品卡(" + mode + ")";
            }

            var listShow = listAuthGroup.Where(o => o.p.ToLower() != "dk" && o.p.ToLower() != "others");

            foreach (AuthGroup group in listShow)
            {
                TreeViewItem itemRroup = new TreeViewItem();

                if (!dicGroupTitle.Keys.Contains(group.p)) continue;
                string headerRroup = dicGroupTitle[group.p];

                int UsedNum = 0;
                if (DicLicUserInfo.Keys.Contains(group.p)) UsedNum = DicLicUserInfo[group.p];

                string allNum = "0";
                if (DicLicInfo.Keys.Contains(group.p)) allNum = DicLicInfo[group.p];


                itemRroup.Tag = group.p;
                itemRroup.Header = headerRroup + "[" + UsedNum + "/" + allNum + "]";
                CloudRegisterUtil.BackgroudColor(itemRroup, UsedNum, allNum);
                itemRroup.Selected += ItemRroup_Selected;

                itemRoot.Items.Add(itemRroup);

            }
            tvlic.Items.Add(itemRoot);


            if (itemRoot.Items.Count == 0) return;
            if (groupId == "")
            {
                (itemRoot.Items[0] as TreeViewItem).IsSelected = true;
                return;
            }


            foreach (var item in itemRoot.Items)
            {
                TreeViewItem selItem = (item as TreeViewItem);
                if (selItem.Tag.ToString() == groupId)
                {
                    selItem.IsSelected = true;
                    return;
                }
            }
            ShowDgUser(groupId);

        }

        private void ItemRroup_Selected(object sender, RoutedEventArgs e)
        {
            dgUser.ItemsSource = null;
            TreeViewItem item = e.Source as TreeViewItem;
            if (item != null && item.Tag != null)
            {
                string group = item.Tag.ToString();
                ShowDgUser(group);
            }
        }

        private void ShowDgUser(string groupID)
        {
            RefreshTVlicHead();
            if (string.IsNullOrEmpty(groupID))
            {
                dgUser.ItemsSource = new List<YhtUserInfo>();
                return;
            }
            dgUser.ItemsSource = null;

            if (!moduleUserMap.ContainsKey(groupID))
            {
                moduleUserMap.Add(groupID, new List<YhtUserInfo>());
            }
            List<YhtUserInfo> list = new List<YhtUserInfo>();
            string str = tbSearch.Text == "搜索用户" ? "" : tbSearch.Text;
            if (!string.IsNullOrEmpty(str))
            {
                IEnumerable<YhtUserInfo> temp = moduleUserMap[groupID].FindAll(o => o.UserCloudName.Contains(str) || o.Mobile.Contains(str) || o.U8UsersStr.Contains(str) || o.UserCloudId.Contains(str));
                if (temp != null)
                {
                    list = temp.ToList();
                }
            }
            else
            {
                list = moduleUserMap[groupID];
            }
            dgUser.ItemsSource = null;
            dgUser.ItemsSource = list;

        }
        private void RefreshTVlicHead()
        {
            if (!tvlic.HasItems)
            { return; }
            TreeViewItem rootTv = (tvlic.Items[0] as TreeViewItem);
            if (!rootTv.HasItems) { return; }
            int count = rootTv.Items.Count;
            for (int i = 0; i < count; i++)
            {
                TreeViewItem tv = (rootTv.Items[i] as TreeViewItem);
                if (tv.Tag != null && !string.IsNullOrEmpty(tv.Tag.ToString()))
                {
                    string groupStr = tv.Tag.ToString();
                    if (string.IsNullOrEmpty(groupStr)) continue;
                    string allNum = "";
                    int usedNum = 0;
                    string headerRroup = string.Empty;
                    if (dicGroupTitle.ContainsKey(groupStr)) headerRroup = dicGroupTitle[groupStr];
                    if (DicLicInfo.Keys.Contains(groupStr)) allNum = DicLicInfo[groupStr];
                    if (moduleUserMap.ContainsKey(groupStr)) usedNum = moduleUserMap[groupStr].Count;
                    CloudRegisterUtil.BackgroudColor(tv, usedNum, allNum);
                    tv.Header = headerRroup + "[" + usedNum + "/" + allNum + "]";
                }
            }
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

        private Dictionary<string, string> GetLicInfo()
        {
            //1A:1,1G:10,1L:1,1R:1,1S:1,1U:1,2A:1,2B:-7,2E:-7,2J:10,2R:10,2S:10,2U:10,3B:10,3D:-18,3E:10,3G:10,3H:10,3J:-18,3K:10,3L:2017,3N:-18,3P:10,3Q:10,3S:1,3T:-18,3U:10,3V:10,3X:10,3Y:10,4D:-18,4P:-18,AP:-1,AR:-1,AS:-2,CA:-2,CD:,CN:test==0111141725,CR:-6,EA:-4,EB:-4,EF:-5,EG:-5,EH:-5,EI:-4,EJ:-3,EL:-5,EQ:-2,ER:-3,ET:-3,EW:-3,FA:-1,FP:-1,GL:-1,HH:-3,HI:10,HJ:10,HK:10,HL:15,HM:10,HN:10,HO:10,HP:20,HX:-5,HY:-4,HZ:1,IA:-3,J1:1,JB:10,JD:10,KJ:-1,L7:1,LE:1,MR:-1,N3:1,PACKAGE:,PU:-3,QD:-6,RC:131,REG:[2017.10.31 13:50:43:302],SA:-3,SC:-1,SN:P1000001,SO:10,ST:-3,U1:1,U6:10,U7:-1,U8:-3,UA:-6,UB:-5,UD:10,UF:-5,UJ:10,UK:-1,UR:-3,US:-3,UT:-3,UU:-4,UV:-4,UW:-4,UX:-4,UY:-4,UZ:-4,WA:-5,XS:-2,YX:-3,DN:U00www2222,DogID:10000020,DogCus:测试企业名称==7uuuuuue

            int dyear = DateTime.Now.Year - (DateTime.Now.Year / 100) * 100;
            int dmonth = DateTime.Now.Month;
            int ddate = dyear * 100 + dmonth;
            StringBuilder szEncode = new StringBuilder(2048010);
            StringBuilder szID = new StringBuilder(33);
            CloudRegisterUtil.GetCurrentID(szID);

            string szCommand = string.Format("Cmdgs,0,{2},RC,{{{0}}}{1}@0,0", szID, Dns.GetHostName(), ddate.ToString());
            szCommand = szCommand.Replace(',', (char)0x1D);
            //取缓冲区长度
            CloudRegisterUtil.RPCCall2(Dns.GetHostName(), szCommand, szEncode);
            StringBuilder szAuths = new StringBuilder(2 * szEncode.Length);
            CloudRegisterUtil.UnlockInfo2(szEncode.ToString(), szAuths, (uint)CloudRegisterUtil.GenerateKey());

            string result = szAuths.ToString();

            Dictionary<string, string> LicDic = result.Split(',').Where(str => !str.StartsWith(":") && str.Contains(":")).Distinct().ToDictionary(o => o.Split(':')[0], o => o.Split(':')[1]);
            return LicDic;
        }

        private Dictionary<string, string> GetModuleMap()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string path2 = Environment.SystemDirectory + "\\U8Service.cfg";
            string U8ServiceInfo = File.ReadAllText(path2);
            string[] U8ServiceArray = File.ReadAllLines(path2);

            foreach (var item in U8ServiceArray)
            {
                StringBuilder sb = new StringBuilder(512);
                CloudRegisterUtil.CommDecode(item, sb);
                string str = sb.ToString();

                string[] array = str.Split('='); //U8模块=商务模块
                dic.Add(array[0], array[1]);
            }
            return dic;

        }

        private Dictionary<string, int> GetLicUserInfo()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            //0,ybt@yonyou.com,18612104080,38922574-be3b-4fca-97d2-ee708d842800,HJ
            foreach (AuthGroup group in listAuthGroup)
            {
                int count = 0;
                foreach (var user in allUserInfo)
                {

                    if (user.Groups.Contains(group.p))
                    {
                        count++;
                        //  AssignUserInfo assignUserInfo = GenUserInfo(user);
                        AddModuleUserMap(moduleUserMap, group.p, user);
                        //AddUserModuleMap(userModuleMap, assignUserInfo.UserCloudId, group.p);
                    }
                }
                dic.Add(group.p, count);
            }
            return dic;
        }

        internal void AddModuleUserMap(Dictionary<string, List<YhtUserInfo>> moduleUserMap, string key, YhtUserInfo value)
        {
            if (moduleUserMap.Keys.Contains(key))
            {
                if (!moduleUserMap[key].Exists(x => x.UserCloudId == value.UserCloudId))
                {
                    moduleUserMap[key].Add(value.Copy());
                }
            }
            else
            {
                moduleUserMap.Add(key, new List<YhtUserInfo>() { value.Copy() });
            }
            moduleUserMap[key] = moduleUserMap[key].OrderBy(o => o.U8UsersStr).ToList();
        }
        private bool CheckSave()
        {
            bool result = true;
            if (!tvlic.HasItems)
            {
                return false;
            }
            TreeViewItem rootTv = (tvlic.Items[0] as TreeViewItem);
            int count = rootTv.Items.Count;
            List<string> OverflowModules = new List<string>();
            for (int i = 0; i < count; i++)
            {
                TreeViewItem tv = (rootTv.Items[i] as TreeViewItem);
                if (tv.Tag != null && !string.IsNullOrEmpty(tv.Tag.ToString()))
                {
                    string groupStr = tv.Tag.ToString();
                    if (string.IsNullOrEmpty(groupStr)) continue;
                    string allNum = "";
                    int usedNum = 0;
                    if (DicLicInfo.Keys.Contains(groupStr)) allNum = DicLicInfo[groupStr];
                    if (moduleUserMap.ContainsKey(groupStr)) usedNum = moduleUserMap[groupStr].Count;
                    if (CloudRegisterUtil.CheckOverflow(usedNum, allNum))
                    {
                        OverflowModules.Add(dicGroupTitle[groupStr]);
                    }
                }
            }
            StringBuilder displayStr = new StringBuilder();
            if (OverflowModules.Count > 0)
            {
                result = false;
                OverflowModules.ForEach(x => displayStr.Append("领域" + x + ","));
                MessageBox.Show(displayStr.ToString().TrimEnd(',') + "实际购买的许可数，与该领域分配的用户数不匹配，请修改。");
            }
            return result;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSave())
            {
                return;
            }
            StringBuilder displaystr = new StringBuilder();
            TreeViewItem item = tvlic.SelectedItem as TreeViewItem;
            if (item == null) return;
            Dictionary<string, HashSet<string>> userModuleMap = new Dictionary<string, HashSet<string>>();
            foreach (string user in ChangedUser)
            {
                userModuleMap.Add(user, new HashSet<string>());
            }
            foreach (var key in moduleUserMap.Keys)
            {
                moduleUserMap[key].ForEach(x =>
                {
                    if (ChangedUser.Contains(x.UserCloudId))
                    {
                        userModuleMap[x.UserCloudId].Add(key);
                    }
                }
                );
            }
            //清空之前模块信息
            foreach (var clearUser in ChangedUser)
            {
                string para = clearUser;
                char[] resultData = new char[2342300];
                int resultLength = 0;
                string resultStr = "";
                CloudRegisterUtil.OfflineSetUserReleam(Environment.MachineName.ToCharArray(), para.ToCharArray(), resultData, out resultLength);
                CloudRegisterUtil.NewDecode(resultData, ref resultStr);
            }


            foreach (var usermodule in userModuleMap)
            {

                string para = usermodule.Key + "," + string.Join(",", usermodule.Value.ToArray());
                char[] resultData = new char[2342300];
                int resultLength = 0;
                string resultStr = "";
                CloudRegisterUtil.OfflineSetUserReleam(Environment.MachineName.ToCharArray(), para.ToCharArray(), resultData, out resultLength);
                CloudRegisterUtil.NewDecode(resultData, ref resultStr);
                displaystr.Append(usermodule.Key + "用户添加模块成功\n\r");
            }

            //MessageBox.Show("保存成功");

            //Init(groupId);
            btnDelUser.IsEnabled = true;
            //}
            //if (!string.IsNullOrEmpty(displaystr.ToString()))
            //{
            //    MessageBox.Show(displaystr.ToString());

            //}
            MessageBox.Show("保存成功");
            TreeViewItem viewItem = tvlic.SelectedItem as TreeViewItem;
            string groupId = "";
            if (viewItem != null && viewItem.Tag != null)
            {
                groupId = viewItem.Tag.ToString();
            }
            Init(groupId);

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem viewItem = tvlic.SelectedItem as TreeViewItem;
            string groupId = "";
            if (viewItem != null && viewItem.Tag != null)
            {
                groupId = viewItem.Tag.ToString();
            }
            Init(groupId);
            ShowDgUser(groupId);
        }

        private void btnSelUser_Click(object sender, RoutedEventArgs e)
        {
            DisplaySelList();
        }
        private void DisplaySelList()
        {
            TreeViewItem item = tvlic.SelectedItem as TreeViewItem;

            if (item == null || item.Tag == null || string.IsNullOrEmpty(item.Tag.ToString()))
            {
                MessageBox.Show("请选择一个领域");
                return;
            }
            string groupId = item.Tag.ToString();
            List<string> filterList = moduleUserMap[groupId].Select(x => x.UserCloudId).ToList();
            SelUser window = new UI.SelUser(groupId, filterList);
            int countAll = Convert.ToInt32(DicLicInfo[groupId]);
            window.CanSelectCount = countAll - filterList.Count;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Closed += Window_Closed;
            window.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TreeViewItem viewItem = tvlic.SelectedItem as TreeViewItem;
            if (viewItem == null) return;
            string groupId = viewItem.Tag.ToString();
            int countAll = Convert.ToInt32(DicLicInfo[groupId]);

            SelUser window = sender as SelUser;
            List<YhtUserInfo> result = window.ListUserSel;

            List<YhtUserInfo> list = result.FindAll(o => o.Checked == true).ToList();

            if (list.Count == 0) return;

            if (list.Count + moduleUserMap[groupId].Count > countAll)
            {
                MessageBox.Show("添加的用户数不能超过该领域分组的许可数");
                btnDelUser.IsEnabled = true;
                //DisplaySelList();
                return;
            }
            foreach (var temp in list)
            {
                temp.Checked = false;
                ChangedUser.Add(temp.UserCloudId);
                AddModuleUserMap(moduleUserMap, groupId, temp);
            }
            dgUser.ItemsSource = moduleUserMap[groupId];
            RefreshTVlicHead();

        }

        private void btnDelUser_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = tvlic.SelectedItem as TreeViewItem;
            string groupId = "";
            if (item != null && item.Tag != null)
            {
                groupId = item.Tag.ToString();
            }

            // List<AssignUserInfo> list = dgUser.ItemsSource as List<AssignUserInfo>;
            bool isDel = false;
            foreach (var temp in moduleUserMap.Values)
            {
                if (temp.Exists(o => o.Checked == true))
                {
                    isDel = true;
                    break;
                }
            }
            if (isDel)
            {
                MessageBoxResult result = MessageBox.Show("请确认是否要移除?", "", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) return;
            }
            else
            {
                MessageBox.Show("请选择要移除的用户");
                return;
            }
            //HashSet<string> modules = new HashSet<string>();
            List<string> keys = moduleUserMap.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                //List<AssignUserInfo> listDel = new List<AssignUserInfo>();
                HashSet<string> ids = new HashSet<string>();
                foreach (var tem in moduleUserMap[keys[i]])
                {
                    if (tem.Checked == true)
                    {
                        ChangedUser.Add(tem.UserCloudId);
                        ids.Add(tem.UserCloudId);
                    }
                }
                if (ids.Count > 0)
                {
                    moduleUserMap[keys[i]] = moduleUserMap[keys[i]].FindAll(x => !ids.Contains(x.UserCloudId)).ToList();
                }
            }
            //RefreshTVlicHead();
            ShowDgUser(groupId);
            //(groupId);
        }

        private void cb_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            TreeViewItem item = tvlic.SelectedItem as TreeViewItem;
            string groupStr = item.Tag.ToString();
            YhtUserInfo user = dgUser.SelectedItem as YhtUserInfo;
            ChangedUser.Add(user.UserCloudId);
            if (cb.IsChecked == true)
            {
                user.Checked = true;
            }
            else
            {
                user.Checked = false;
            }
            if (moduleUserMap.ContainsKey(groupStr))
            {
                for (int i = 0; i < moduleUserMap[groupStr].Count; i++)
                {
                    if (moduleUserMap[groupStr][i].UserCloudId == user.UserCloudId)
                    {
                        moduleUserMap[groupStr][i].Checked = user.Checked;
                        break;
                    }

                }
            }
        }

        private void tbSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            if (tbSearch.Text == "搜索用户") tbSearch.Text = "";

        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbSearch.Text == "搜索用户") return;

            TreeViewItem item = tvlic.SelectedItem as TreeViewItem;
            if (item == null || item.Tag == null || string.IsNullOrEmpty(item.Tag.ToString())) return;
            string groupId = item.Tag == null ? "" : item.Tag.ToString();
            ShowDgUser(groupId);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem viewItem = tvlic.SelectedItem as TreeViewItem;
            string groupId = "";
            if (viewItem != null && viewItem.Tag != null)
            {
                groupId = viewItem.Tag.ToString();
            }
            Init(groupId);
        }
    }
}

