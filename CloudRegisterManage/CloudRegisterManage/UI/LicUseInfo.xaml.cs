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
using Newtonsoft.Json.Linq;
using System.Windows.Automation.Peers;

namespace CloudRegisterManage
{
    /// <summary>
    /// Interaction logic for SysOptionsView.xaml
    /// </summary>
    public partial class LicUseInfo : UserControl
    {
        private const char RPCDELIMITER = '\u001D';//RPCCALL命令的分割符

        private Dictionary<string, string> DicModuleMap = new Dictionary<string, string>();//U8模块和商务模块的对应关系  key=U8模块
        private List<AuthGroup> listAuthGroup = new List<AuthGroup>();
        private Dictionary<string, string> DicLicInfo = new Dictionary<string, string>();//加密信息 和 模块购买点数
        //private Dictionary<string, int> DicLoginInfo = new Dictionary<string, int>(); //模块已登录点数
        Dictionary<string, string> dicGroupTitle = CloudRegisterUtil.GetGroupTitle(); //分组名称
        Dictionary<string, string> dicModuleTitle = CloudRegisterUtil.GetModuleTitle(); //模块名称

        private List<YhtUserInfo> allUserInfo = new List<YhtUserInfo>();

        private List<UserLoginState> allLoginedInfo = new List<UserLoginState>();

        //EA=OpenAPI,加密狗ID=U2
        //EM=多工厂，加密狗ID=EM
        //GC=集团管控，加密狗ID=KY
        //EO=U8协同API,加密狗ID=EO
        //KU=电商开电票,加密狗ID=KU
        //U5=收入核算,加密狗ID=U5
        //这几个模块特殊处理
        private readonly HashSet<string> HAVEORNOTCODELIST = new HashSet<string>(new string[] { "EA", "EM", "GC", "EO", "KU", "U5", "3W" }, StringComparer.OrdinalIgnoreCase);

        public LicUseInfo()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            allUserInfo = CloudRegisterUtil.GetYHTAllUserInfo();
            allLoginedInfo = CloudRegisterUtil.GetLoginedInfo();

            dicGroupTitle = CloudRegisterUtil.GetGroupTitle();
            dicModuleTitle = CloudRegisterUtil.GetModuleTitle();

            DicModuleMap = GetModuleMap();
            listAuthGroup = CloudRegisterUtil.GetAuthGroupInfo();
            DicLicInfo = GetLicInfo();
            //DicLoginInfo = GetLoginInfo();
            InitTv();
        }

        private void InitTv()
        {
            tvlic.Items.Clear();
            dgGroup.ItemsSource = null;
            dgDetail.ItemsSource = null;

            //7uuuuuue,online,测试企业名称,2017-10-25,awqr8k1g
            //服务识别码,在线模式,测试企业名称,2017-10-25,云数据中心ID
            string companyInfo = GetCompanyInfo();
            if (companyInfo == "" || companyInfo == ",null,,,," || companyInfo.Contains("failed:"))
            {
                return;
            }
            string certificationdate = string.Empty;
            string licStateStr = GetYhtLicState();
            if (licStateStr != "" && licStateStr != ",null,,,," && !licStateStr.Contains("failed:"))
            {
                JObject jObj = JObject.Parse(licStateStr);
                string strCertificationdate = jObj["certificationdate"] == null ? "" : jObj["certificationdate"].ToString();

                DateTime cerTime = new DateTime();
                if (DateTime.TryParse(strCertificationdate, out cerTime))
                {
                    certificationdate = cerTime.ToString("yyyy-MM-dd");
                }

            }

            string mode = "";
            switch (companyInfo.Split(',')[1])
            {
                case "online":
                    {
                        mode = "在线正式加密";
                        break;
                    }
                case "offline":
                    {
                        mode = "离线正式加密";
                        //dgDetail.Columns[0].Visibility = Visibility.Hidden;
                        dgDetail.Columns[1].Visibility = Visibility.Hidden;
                        break;
                    }
                case "adviser":
                    {
                        mode = "顾问加密"; //(在线的一种)
                        break;
                    }
                case "onlineborrow":
                    {
                        mode = "在线借用";
                        break;
                    }
                case "offlineborrow":
                    {
                        mode = "离线借用";
                        //dgDetail.Columns[0].Visibility = Visibility.Hidden;
                        dgDetail.Columns[1].Visibility = Visibility.Hidden;
                        break;
                    }
                case "ontrial":
                    {
                        mode = "试用许可"; //(在线的一种)
                        break;
                    }
                default:
                    {
                        mode = "在线正式加密";
                        break;
                    }
            }
            string rc = "";
            if (DicLicInfo.Keys.Contains("RC")) rc = DicLicInfo["RC"];

            TreeViewItem itemRoot = new TreeViewItem();
            itemRoot.IsExpanded = true;

            if (rc != "")
            {
                itemRoot.Header = "U8" + rc.Substring(0, 2) + "." + rc.Substring(2, 1) + "产品卡(" + mode + certificationdate + ")";
            }
            else
            {
                itemRoot.Header = "U8" + "产品卡(" + mode + ")";
            }

            TreeViewItem itemDomainReg = new TreeViewItem();
            itemDomainReg.Header = "注册制产品";
            itemDomainReg.IsExpanded = true;
            itemDomainReg.Selected += ItemDomain_Selected;
            itemRoot.Items.Add(itemDomainReg);

            var listAuthGroupReg = listAuthGroup.Where(o => o.p != "others");
            var groupLoginInfo = allLoginedInfo.Select(o => new { group = o.GroupId, userCID = o.UserCloudId }).Distinct(); //领域 登录了几个人

            foreach (AuthGroup group in listAuthGroupReg)
            {
                if (!dicGroupTitle.Keys.Contains(group.p)) continue;

                TreeViewItem itemRroup = new TreeViewItem();
                string headerRroup = dicGroupTitle[group.p];
                int loginedNum = groupLoginInfo.Count(o => o.group == group.p);

                string allNum = "0";
                if (DicLicInfo.Keys.Contains(group.p)) allNum = DicLicInfo[group.p];
                CloudRegisterUtil.BackgroudColor(itemRroup, loginedNum, allNum);
                itemRroup.Header = headerRroup + "[" + loginedNum + "/" + allNum + "]";
                if (group.c != null)
                {
                    foreach (string module in group.c)
                    {
                        if (!dicModuleTitle.Keys.Contains(module)) continue;

                        TreeViewItem itemModule = new TreeViewItem();
                        string headerModule = dicModuleTitle[module];
                        itemModule.Header = headerModule;
                        itemRroup.Items.Add(itemModule);

                    }
                }

                itemDomainReg.Items.Add(itemRroup);

            }

            var groupNoReg = listAuthGroup.FirstOrDefault(o => o.p == "others");
            var moduleLoginInfo = allLoginedInfo.Select(o => new { module = o.ModuleId, userCID = o.UserCloudId, userId = o.UserId, clientID = o.ClientID }).Distinct(); //模块 登录了几个人,非注册制产品 按照 客户端 计数
            if (groupNoReg != null)
            {
                TreeViewItem itemDomainNoReg = new TreeViewItem();
                itemDomainNoReg.Header = "非注册制产品";
                itemDomainNoReg.IsExpanded = true;
                itemDomainNoReg.Selected += ItemDomainNoReg_Selected;

                itemRoot.Items.Add(itemDomainNoReg);
                if (groupNoReg.c != null)
                {
                    foreach (string module in groupNoReg.c)
                    {
                        if (!dicModuleTitle.Keys.Contains(module)) continue;

                        TreeViewItem itemModule = new TreeViewItem();
                        string headerModule = dicModuleTitle[module];

                        int loginedNum = moduleLoginInfo.Count(o => o.module == module);

                        string allNum = "0";
                        string businessModule = DicModuleMap[module];//商务模块号
                        if (DicLicInfo.Keys.Contains(businessModule)) allNum = DicLicInfo[businessModule];
                        if (HAVEORNOTCODELIST.Contains(module) && allNum != "0") allNum = "1";
                        CloudRegisterUtil.BackgroudColor(itemModule, loginedNum, allNum);
                        itemModule.Header = headerModule + "[" + loginedNum + "/" + allNum + "]";
                        itemDomainNoReg.Items.Add(itemModule);
                    }
                }
            }


            tvlic.Items.Add(itemRoot);

            itemDomainReg.IsSelected = true;

        }

        private void ItemDomainNoReg_Selected(object sender, RoutedEventArgs e)
        {

            List<GroupInfoShow> lsShow = new List<GroupInfoShow>();
            var groupNoReg = listAuthGroup.FirstOrDefault(o => o.p == "others");
            if (groupNoReg == null) return;
            if (groupNoReg.c == null) return;


            int no = 0;

            var moduleLoginInfo = allLoginedInfo.Select(o => new { module = o.ModuleId, userCID = o.UserCloudId, userId = o.UserId, clientID = o.ClientID }).Distinct(); //模块 登录了几个人
            foreach (string module in groupNoReg.c)
            {
                if (!dicModuleTitle.Keys.Contains(module)) continue;

                GroupInfoShow groupShow = new GroupInfoShow();
                groupShow.IsGroup = false;
                groupShow.No = no++;
                groupShow.SubSysID = module;
                groupShow.LoginedNumber = moduleLoginInfo.Count(o => o.module == module);
                groupShow.SubSysName = dicModuleTitle[module];

                string businessModule = DicModuleMap[module];//商务模块号

                if (DicLicInfo.Keys.Contains(businessModule))
                {
                    int allNumber = Convert.ToInt32(DicLicInfo[businessModule]);
                    if (HAVEORNOTCODELIST.Contains(module) && allNumber > 0) allNumber = 1;

                    groupShow.AllAuthNumber = allNumber;

                }
                lsShow.Add(groupShow);
            }

            dgGroup.ItemsSource = lsShow;


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

        private void ItemDomain_Selected(object sender, RoutedEventArgs e)
        {
            List<GroupInfoShow> lsShow = new List<GroupInfoShow>();
            var groupLoginInfo = allLoginedInfo.Select(o => new { group = o.GroupId, userCID = o.UserCloudId }).Distinct(); //领域 登录了几个人

            int no = 0;
            foreach (AuthGroup group in listAuthGroup)
            {
                if (!dicGroupTitle.Keys.Contains(group.p)) continue;

                GroupInfoShow groupShow = new GroupInfoShow();
                groupShow.IsGroup = true;
                groupShow.No = no++;
                groupShow.SubSysID = group.p;
                groupShow.LoginedNumber = groupLoginInfo.Count(o => o.group == group.p);
                groupShow.SubSysName = dicGroupTitle[group.p];

                if (DicLicInfo.Keys.Contains(group.p))
                {
                    groupShow.AllAuthNumber = Convert.ToInt32(DicLicInfo[group.p]);
                }
                lsShow.Add(groupShow);
            }

            dgGroup.ItemsSource = lsShow;
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

        private string GetCompanyInfo()
        {
            char[] resultData = new char[2342300];
            int resultLength = 0;
            string resultStr = "";
            CloudRegisterUtil.GetYhtCompany(Environment.MachineName.ToCharArray(), resultData, resultLength);

            CloudRegisterUtil.NewDecode(resultData, ref resultStr);

            return resultStr;
        }

        private void dgGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            dgDetail.ItemsSource = null;
            GroupInfoShow group = dgGroup.SelectedItem as GroupInfoShow;
            if (group == null) return;

            List<UserLoginState> list = new List<UserLoginState>();

            if (group.IsGroup == true) //注册制产品组
            {
                //可以使用这个 领域的用户列表
                var userList = allUserInfo.Where(o => o.Groups.Contains(group.SubSysID));

                foreach (var user in userList)
                {
                    foreach (var u8User in user.U8Users)
                    {
                        string userCloudId = user.UserCloudId;
                        string u8UserId = u8User.Key;

                        //用户有可能在多个客户端同时登录， 也可能登录不同的模块
                        var loginUserList = allLoginedInfo.Where(o => o.GroupId == group.SubSysID && o.UserId == u8UserId && o.UserCloudId == userCloudId);

                        if (loginUserList.Count() == 0)
                        {

                            UserLoginState userLoginState = new UserLoginState();
                            userLoginState.UserCloudId = user.UserCloudId;
                            userLoginState.UserCloudName = user.UserCloudName;
                            userLoginState.UserId = u8User.Key;
                            userLoginState.UserName = u8User.Value;
                            userLoginState.State = "未登录";
                            list.Add(userLoginState);
                        }
                        else
                        {

                            foreach (var loginedUser in loginUserList)
                            {
                                UserLoginState userLoginState = new UserLoginState();
                                userLoginState.UserCloudId = user.UserCloudId;
                                userLoginState.UserCloudName = user.UserCloudName;
                                userLoginState.UserId = u8User.Key;
                                userLoginState.UserName = u8User.Value;

                                //UserLoginState firstUser = loginUserList.First();
                                userLoginState.State = "已登录";
                                userLoginState.ModuleForShow = dicModuleTitle[loginedUser.ModuleId];

                                userLoginState.ClientPCName = loginedUser.ClientPCName;
                                userLoginState.ClientID = loginedUser.ClientID;
                                userLoginState.TaskID = loginedUser.TaskID;
                                userLoginState.PortalName = loginedUser.PortalName;
                                userLoginState.BException = loginedUser.BException;
                                userLoginState.BCount = loginedUser.BCount;

                                list.Add(userLoginState);

                            }

                        }
                    }

                }
            }
            else //非注册制产品模块
            {
                //非注册制产品模块  跟用户之前没有对应关系，直接从 登录信息中获取
                list = allLoginedInfo.Where(o => o.ModuleId == group.SubSysID && (allUserInfo.Count(o1 => o1.UserCloudId == o.UserCloudId) > 0)).ToList();
                foreach (var item in list)
                {
                    item.UserCloudName = allUserInfo.First(o => o.UserCloudId == item.UserCloudId).UserCloudName;
                    item.ModuleForShow = dicModuleTitle[item.ModuleId];
                }

            }

            list = list.OrderByDescending(o => o.State).ToList();
            dgDetail.ItemsSource = list;
        }
        private string GetYhtLicState()
        {
            char[] resultData = new char[2342300];
            int resultLength = 0;
            string resultStr = "";
            var LicStateStr = CloudRegisterUtil.GetLicenseState(Environment.MachineName.ToCharArray(), resultData, resultLength);
            CloudRegisterUtil.NewDecode(resultData, ref resultStr);

            return resultStr;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {


            Init();
        }
    }

    public class AuthGroup
    {
        public string p { get; set; }

        public string[] c { get; set; }
    }

    public class GroupInfoShow
    {
        /// <summary>
        ///序号
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 子系统ID
        /// </summary>
        public string SubSysID { get; set; }

        /// <summary>
        /// 子系统名称
        /// </summary>
        public string SubSysName { get; set; }

        /// <summary>
        /// 已登录数
        /// </summary>
        public int LoginedNumber { get; set; }

        /// <summary>
        /// 总授权数
        /// </summary>
        public int AllAuthNumber { get; set; }

        /// <summary>
        ///true:注册制产品分组  false:非注册制产品模块  
        /// </summary>
        public bool IsGroup { get; set; }

    }



}
