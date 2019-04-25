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
using Newtonsoft.Json.Linq;
using System.Threading;

namespace CloudRegisterManage
{
    /// <summary>
    /// Interaction logic for SysOptionsView.xaml
    /// </summary>
    public partial class ImportLic : UserControl
    {
        static bool isSync = false;
        private const char RPCDELIMITER = '\u001D';//RPCCALL命令的分割符
        public event Action ShowLicClick;
        private static readonly Regex rCmdadi2Err = new Regex(@"^failed: (\d+), (.+)$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public ImportLic()
        {
            Init();

        }
        public void Init()
        {
            InitializeComponent();
            ShowLicInfo();
        }
        public void DisplayAll()
        {
            Display(Version);
            Display(labVersion);
            Display(LicType);
            Display(labLicType);
            Display(CompanyName);
            Display(labCompanyName);
            Display(ServiceCode);
            Display(labServiceCode);
            Display(CertificationDate);
            Display(labCertificationDate);
            Display(LicenseRefreshTime);
            Display(labLicenseRefreshTime);
            Display(HardCode);
            Display(labHardCode);
            Display(CloudlicenseCheck);
            Display(labCloudlicenseCheck);
            Display(LicenseRegisterDate);
            Display(labLicenseRegisterDate);
            Display(LastCloudlicenseCheck);
            Display(labLastCloudlicenseCheck);
            Display(ProductUserId);
            Display(labProductUserId);
            Display(TenantId);
            Display(labTenantId);
            Hidden(LastCloudlicenseCheckWarn);
            Hidden(labLastCloudlicenseCheckWarn);
            Hidden(LicenseRegisterDateWarn);
            Hidden(labLicenseRegisterDateWarn);
        }

        private void ShowLicInfo()
        {
            DisplayAll();
            //0111142138,online,用友软件股份有限公司—测试,2017-11-23 23:59:59,h2bbmz9f
            string companyInfo = GetCompanyInfo();
            btnShowLic.IsEnabled = true;
            if (companyInfo == "" || companyInfo == ",null,,,," || companyInfo.Contains("failed:"))
            {
                //没有许可
                btnShowLic.IsEnabled = false;
                btnSyncLic.IsEnabled = false;
                return;
            }

            string[] companyInfos = companyInfo.Split(',');
            Dictionary<string, string> dicLicInfo = GetLicInfo();

            if (dicLicInfo.Keys.Contains("RC"))
            {
                string rc = dicLicInfo["RC"];
                labVersion.Content = "U8V" + rc.Substring(0, 2) + "." + rc.Substring(2, 1);
            }
            labHardCode.Content = GetHardCode();
            string mode = "";
            string type = companyInfo.Split(',')[1];
            List<string> onlineList = new List<string>() { "online", "adviser", "onlineborrow" };//, "ontrial" };
            List<string> offlineList = new List<string>() { "offline", "offlineborrow" };
            switch (type)
            {
                case "online":
                    {
                        mode = "在线正式加密";
                        btnSyncLic.IsEnabled = true;
                        Hidden(CertificationDate);
                        Hidden(labCertificationDate);
                        Hidden(LicenseRegisterDate);
                        Hidden(labLicenseRegisterDate);

                        break;
                    }
                case "offline":
                    {
                        mode = "离线正式加密";
                        btnSyncLic.IsEnabled = false;
                        Hidden(CertificationDate);
                        Hidden(labCertificationDate);
                        Hidden(CloudlicenseCheck);
                        Hidden(labCloudlicenseCheck);
                        Hidden(LastCloudlicenseCheck);
                        Hidden(labLastCloudlicenseCheck);
                        Hidden(LastCloudlicenseCheckWarn);
                        Hidden(labLastCloudlicenseCheckWarn);
                        break;
                    }
                case "adviser":
                    {
                        mode = "顾问加密"; //(在线的一种)
                        btnSyncLic.IsEnabled = true;
                        Hidden(LicenseRegisterDate);
                        Hidden(labLicenseRegisterDate);
                        break;
                    }
                case "onlineborrow":
                    {
                        mode = "在线借用";
                        btnSyncLic.IsEnabled = true;
                        Hidden(LicenseRegisterDate);
                        Hidden(labLicenseRegisterDate);
                        break;
                    }
                case "offlineborrow":
                    {
                        mode = "离线借用";
                        btnSyncLic.IsEnabled = false;
                        Hidden(CloudlicenseCheck);
                        Hidden(labCloudlicenseCheck);
                        break;
                    }
                case "ontrial":
                    {
                        mode = "试用许可"; //(在线的一种)
                        btnSyncLic.IsEnabled = false;
                        btnShowLic.IsEnabled = false;
                        labHardCode.Content = string.Empty;
                        Hidden(CompanyName);
                        Hidden(labCompanyName);
                        Hidden(ServiceCode);
                        Hidden(labServiceCode);
                        Hidden(CertificationDate);
                        Hidden(labCertificationDate);
                        Hidden(HardCode);
                        Hidden(labHardCode);
                        Hidden(ProductUserId);
                        Hidden(labProductUserId);
                        Hidden(CloudlicenseCheck);
                        Hidden(labCloudlicenseCheck);
                        Hidden(LastCloudlicenseCheck);
                        Hidden(labLastCloudlicenseCheck);
                        Hidden(LicenseRegisterDate);
                        Hidden(labLicenseRegisterDate);
                        break; //如果当前环境使用的是产品试用许可，该界面不显示任何内容
                    }
                default:
                    {

                        mode = "在线正式加密";
                        btnSyncLic.IsEnabled = true;
                        Hidden(LicenseRegisterDate);
                        Hidden(labLicenseRegisterDate);
                        break;
                    }
            }

            labLicType.Content = mode;
            labCompanyName.Content = companyInfos[2];
            labServiceCode.Content = companyInfos[0];
            labTenantId.Content = companyInfos[4];
            string licStateStr = GetYhtLicState();
            if (licStateStr == "" || licStateStr == ",null,,,," || licStateStr.Contains("failed:"))
            {
                return;
            }
            JObject jObj = JObject.Parse(licStateStr);
            labCertificationDate.Content = jObj["certificationdate"] == null ? string.Empty : jObj["certificationdate"].ToString();
            labLicenseRefreshTime.Content = jObj["licenserefreshtime"] == null ? string.Empty : jObj["licenserefreshtime"].ToString();
            string labGracePeriodParaStr = jObj["graceperiodpara"] == null ? string.Empty : jObj["graceperiodpara"].ToString();
            if (!string.IsNullOrEmpty(labGracePeriodParaStr))
            {
                labGracePeriodParaStr = labGracePeriodParaStr.Replace("-d", "天");
            }
            string stateStr = jObj["graceperiodstate"] == null ? string.Empty : jObj["graceperiodstate"].ToString();
            string handResultStr = jObj["lasthandshakeresult"] == null ? string.Empty : jObj["lasthandshakeresult"].ToString();
            bool handResult = true;
            if (handResultStr == "fail")
            {
                handResult = false;
                handResultStr = "失败";
            }
            else
            {
                handResultStr = "成功";
            }
            string lasthandshaketime = jObj["lasthandshaketime"] == null ? string.Empty : jObj["lasthandshaketime"].ToString();
            DateTime lasthandshaketimeDT = new DateTime();
            if (DateTime.TryParse(lasthandshaketime, out lasthandshaketimeDT))
            {
                labLastCloudlicenseCheck.Content = "最近校验--" + lasthandshaketime + "( " + handResultStr + " )";
                if (onlineList.Contains(type))
                {
                    Display(LastCloudlicenseCheck);
                    Display(labLastCloudlicenseCheck);
                }
            }
            else
            {
                Hidden(LastCloudlicenseCheck);
                Hidden(labLastCloudlicenseCheck);
                labLastCloudlicenseCheck.Content = String.Empty;
            }
            string labGracePeriodTimeStr = jObj["graceperiodtime"] == null ? string.Empty : jObj["graceperiodtime"].ToString();
            if (!handResult && onlineList.Contains(type))
            {
                Display(LastCloudlicenseCheckWarn);
                Display(labLastCloudlicenseCheckWarn);
                labLastCloudlicenseCheckWarn.Content = "由于您的证书在云端校验不通过，" + labGracePeriodTimeStr + "后不允许继续使用当前产品";
            }
            if (stateStr != "0" && onlineList.Contains(type))
            {
                if (handResult)
                {
                    Hidden(labLastCloudlicenseCheck);
                    Hidden(LastCloudlicenseCheck);
                    Display(LastCloudlicenseCheckWarn);
                    Display(labLastCloudlicenseCheckWarn);
                    labLastCloudlicenseCheckWarn.Content = "由于您的证书在云端校验不通过，" + labGracePeriodTimeStr + "后不允许继续使用当前产品";
                }
            }
            if (stateStr != "0" && offlineList.Contains(type))
            {
                Display(LicenseRegisterDateWarn);
                Display(labLicenseRegisterDateWarn);
                labLicenseRegisterDateWarn.Content = "由于您未按时在云端重注册加密，" + labGracePeriodTimeStr + "后不允许继续使用当前产品。";
            }

            string nexthandshaketime = jObj["nexthandshaketime"] == null ? string.Empty : jObj["nexthandshaketime"].ToString();
            DateTime nexthandshaketimeDT = new DateTime();
            if (DateTime.TryParse(nexthandshaketime, out nexthandshaketimeDT))
            {
                string datePre = "校验计划--每日  ";
                string labTime = string.Format("{0:T}", nexthandshaketimeDT);
                labCloudlicenseCheck.Content = datePre + labTime;
                labLicenseRegisterDate.Content = nexthandshaketime;
            }
            else
            {
                labCloudlicenseCheck.Content = string.Empty;
                labLicenseRegisterDate.Content = string.Empty;

            }
            labProductUserId.Content = jObj["productuserid"] == null ? string.Empty : jObj["productuserid"].ToString();

        }
        private void Hidden(Label label)
        {
            label.Visibility = Visibility.Collapsed;
            label.IsEnabled = false;
        }
        private void Display(Label label)
        {
            label.Visibility = Visibility.Visible;
            label.IsEnabled = true;
        }
        private void DisplayLoading(bool isDisplay)
        {
            if (isDisplay)
            {
                this._loading.Visibility = Visibility.Visible;
            }
            else
            {
                this._loading.Visibility = Visibility.Collapsed;
            }
        }

        private void btnImportLic_Click(object sender, RoutedEventArgs e)
        {

            byte[] resultData = new byte[2342300];
            int resultLength = 0;
            string resultStr = "";
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    FileInfo licFile = new FileInfo(dialog.FileName);
                    List<string> filterExtension = new List<string>() { ".yht", ".zip" };
                    if (filterExtension.Contains(licFile.Extension.ToLower()))
                    {
                        byte[] data = File.ReadAllBytes(dialog.FileName);
                        CloudRegisterUtil.ImportYhtLic(Environment.MachineName.ToCharArray(), data, data.Length, resultData, resultLength);

                        string result = System.Text.Encoding.Default.GetString(resultData);     //Pass:import yht_lic_file succ.
                        CloudRegisterUtil.NewDecode(result.ToArray(), ref resultStr);
                    }
                    else
                    {
                        resultStr = "请导入许可类型文件,此文件非官方导出文件";
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                Match me = rCmdadi2Err.Match(resultStr);
                string sErrMsg;
                if (me.Success)
                {
                    string sErrCode = me.Groups[1].Value;
                    sErrMsg = me.Groups[2].Value;
                }
                else
                {
                    sErrMsg = resultStr;
                }

                if (resultStr.Contains("succ"))
                {
                    string companyInfo = GetCompanyInfo();
                    if (companyInfo == "" || companyInfo == ",null,,,," || companyInfo.Contains("failed:"))
                    {
                        return;
                    }
                    string type = companyInfo.Split(',')[1];
                    switch (type)
                    {
                        case "online":
                        case "adviser":
                        case "onlineborrow":
                        case "ontrial":
                            MessageBox.Show("许可文件更新成功。为避免无法正常使用产品，请尽快登录U8系统管理，在用户管理界面执行同步操作");
                            break;
                        case "offline":
                        case "offlineborrow":
                            MessageBox.Show("许可文件更新成功。为避免无法正常使用产品，请尽快按照如下步骤操作：\n\r1、登录U8系统管理，在用户管理界面执行同步操作；\n\r2、在云注册管理的分配许可界面，执行许可分配；");
                            break;
                        default:
                            MessageBox.Show("导入成功");
                            break;
                    }
                }
                else if (resultStr.Contains("failed"))
                {
                    string sMsg;
                    int iErrCode;
                    object[] resArg;
                    CodeTranslateHelper.GetUfpaMessage(resultStr, out iErrCode, out sMsg);
                    string resid = CodeTranslateHelper.GetYhtResid(iErrCode, sMsg, out resArg);
                    LogDebug.WriteLog(resultStr + iErrCode + sMsg + resid + resArg);
                    if (resArg != null)
                    {
                        resultStr = CodeTranslateHelper.GetFormatterString(resid, resArg);
                    }
                    else
                    {
                        resultStr = CodeTranslateHelper.GetStrByCode(resid);
                    }
                    MessageBox.Show(resultStr);
                }
                else
                {
                    MessageBox.Show(resultStr);
                }
            }
            Init();

        }

        private void btnShowLic_Click(object sender, RoutedEventArgs e)
        {
            if (ShowLicClick != null)
            {
                ShowLicClick();
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
        private string GetYhtLicState()
        {
            char[] resultData = new char[2342300];
            int resultLength = 0;
            string resultStr = "";
            var LicStateStr = CloudRegisterUtil.GetLicenseState(Environment.MachineName.ToCharArray(), resultData, resultLength);
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



        private void btnSyncLic_Click(object sender, RoutedEventArgs e)
        {

            StringBuilder szID = new StringBuilder(1024);
            CloudRegisterUtil.GetCurrentID(szID);

            char[] resultData = new char[2342300];
            int key = CloudRegisterUtil.GenerateKey();
            int resultLength = key;

            CloudRegisterUtil.SyncYhtLic(Environment.MachineName.ToCharArray(), szID.ToString().ToCharArray(), resultData, ref resultLength);
            StringBuilder szAuths = new StringBuilder(65536);
            CloudRegisterUtil.UnlockInfo2(new string(resultData), szAuths, (uint)key);

            string result = szAuths.ToString();
            if (result.Contains("succ"))
            {
                int lSyncHandler = NumbCheck(result);
                GenThread(lSyncHandler);
                DisplayLoading(true);
            }
            else
            {

                if (result.Contains("failed: 2002"))
                {
                    MessageBox.Show("连接到服务器的过程出现意外");
                }
                else
                {
                    MessageBox.Show(result);
                }
            }



        }
        public int NumbCheck(string checkStr)
        {
            int result = 0;
            string re = "[0-9]+";
            Regex reg = new Regex(re);
            Match match = reg.Match(checkStr);
            var bo = match.Success;
            if (match.Success)
            {
                result = int.Parse(match.Value);
            }
            return result;
        }
        private delegate string returnStrDelegate();
        public void GenThread(int lSyncHandler)
        {
            if (isSync != true)
            {
                Thread syncThread = new Thread(new ParameterizedThreadStart(DoService));
                //设置线程为后台线程,那样进程里就不会有未关闭的程序了
                syncThread.IsBackground = true;
                syncThread.Start(lSyncHandler);//起线程
                isSync = true;

            }
        }
        private void DoService(object lSyncHandler)
        {
            bool stop = false;
            int i = 0;
            char[] resultData = new char[2342300];
            int key = CloudRegisterUtil.GenerateKey();
            int resultLength = key;
            string resultStr = string.Empty;
            StringBuilder resultSB = new StringBuilder(65536);
            while (!stop)
            {
                i++;

                CloudRegisterUtil.GetSyncYhtLicState(Environment.MachineName.ToCharArray(), (int)lSyncHandler, resultData, out resultLength);

                CloudRegisterUtil.UnlockInfo2(new string(resultData), resultSB, (uint)key);
                System.Threading.Thread.Sleep(1000 * 3);
                resultStr = resultSB.ToString();
                if (resultStr.Contains("fin"))
                {
                    stop = true;
                    string companyInfo = GetCompanyInfo();
                    if (companyInfo == "" || companyInfo == ",null,,,," || companyInfo.Contains("failed:"))
                    {
                        return;
                    }
                    string type = companyInfo.Split(',')[1];
                    switch (type)
                    {
                        case "online":
                        case "adviser":
                        case "onlineborrow":
                        case "ontrial":
                            resultStr = "许可文件更新成功。为避免无法正常使用产品，请尽快登录U8系统管理，在用户管理界面执行同步操作";
                            break;
                        case "offline":
                        case "offlineborrow":
                            resultStr = "许可文件更新成功。为避免无法正常使用产品，请尽快按照如下步骤操作：\n\r1、登录U8系统管理，在用户管理界面执行同步操作；\n\r2、在云注册管理的分配许可界面，执行许可分配；";
                            break;
                        default:
                            resultStr = "许可同步成功";
                            break;
                    }
                }
                if (resultStr.Contains("failed"))
                {
                    stop = true;
                    string sMsg;
                    int iErrCode;
                    object[] resArg = null;
                    CodeTranslateHelper.GetUfpaMessage(resultStr, out iErrCode, out sMsg);
                    string resid = CodeTranslateHelper.GetYhtResid(iErrCode, sMsg, out resArg);
                    LogDebug.WriteLog(resultStr + iErrCode + sMsg + resid + resArg);
                    if (resArg != null)
                    {
                        resultStr = CodeTranslateHelper.GetFormatterString(resid, resArg);
                    }
                    else
                    {
                        resultStr = CodeTranslateHelper.GetStrByCode(resid);
                    }

                }
                if (i > 20)
                {
                    resultStr = "等待友户通响应超时";
                    //this.Dispatcher.Invoke(new Action(() => MessageBox.Show("等待友户通响应超时")));
                    stop = true;
                }
            }
            if (stop)
            {
                isSync = false;

                this.Dispatcher.Invoke(new Action(() => MessageBox.Show(resultStr)));
                this.Dispatcher.Invoke(new Action(() => DisplayLoading(false)));
                this.Dispatcher.Invoke(new Action(() => ShowLicInfo()));


            }
        }
        public string CutString(string sourceStr, string startStr, string endStr)
        {
            string result = string.Empty;
            Regex reg = new Regex(@"(?<=" + startStr + ")(.+?)(?=" + endStr + ")");

            Match match = reg.Match(sourceStr);
            if (match.Success)
            {
                result = match.Value;
            }
            return result;

        }
    }
}
