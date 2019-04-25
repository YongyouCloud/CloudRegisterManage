using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace CloudRegisterManage
{
    public class CloudRegisterUtil
    {
        private const int BUFFSIZE = 20480;
        private const char RPCDELIMITER = '\u001D';//RPCCALL命令的分割符

        [DllImport("secucomm.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal extern static long CommDecode([MarshalAs(UnmanagedType.LPStr)] string szEncoded, [MarshalAs(UnmanagedType.LPStr)] StringBuilder szDecoded);


        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int RPCCall([In, MarshalAs(UnmanagedType.LPArray)]char[] cServer,
        [In, MarshalAs(UnmanagedType.LPArray)]char[] cIn,
        [Out, MarshalAs(UnmanagedType.LPArray)]char[] cOut);


        [DllImport("UFPAClient.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, EntryPoint = "RPCCall")]
        internal extern static int RPCCall2([MarshalAs(UnmanagedType.LPStr)] string szServer, [MarshalAs(UnmanagedType.LPStr)] string szCommand, [MarshalAs(UnmanagedType.LPStr)] StringBuilder szOut);


        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int ImportYhtLic([In, MarshalAs(UnmanagedType.LPArray)]char[] cServer,
        [In, MarshalAs(UnmanagedType.LPArray)]byte[] fz,
        [In, MarshalAs(UnmanagedType.I4)]int fLength,
        [Out, MarshalAs(UnmanagedType.LPArray)]byte[] response,
        [Out, MarshalAs(UnmanagedType.I4)]int responseLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern
        int GetYhtUserCount([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [Out, MarshalAs(UnmanagedType.LPArray)]char[] pResult,
        [In, Out, MarshalAs(UnmanagedType.I4)]ref int iResultLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern
        int GetYhtAllUserPaged([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [In, MarshalAs(UnmanagedType.I4)]int iPage,
        [In, MarshalAs(UnmanagedType.I4)]int iPageSize,
        [Out, MarshalAs(UnmanagedType.LPArray)]char[] pResult,
        [In, Out, MarshalAs(UnmanagedType.I4)]ref int iResultLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern
       int GetYhtAllUser2Paged([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
       [In, MarshalAs(UnmanagedType.I4)]int iPage,
       [In, MarshalAs(UnmanagedType.I4)]int iPageSize,
       [Out, MarshalAs(UnmanagedType.LPArray)]char[] pResult,
       [In, Out, MarshalAs(UnmanagedType.I4)]ref int iResultLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern
        int GetYhtAllUser([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [Out, MarshalAs(UnmanagedType.LPArray)]char[] pResult,
        [Out, MarshalAs(UnmanagedType.I4)]int iResultLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern
        int GetYhtAllUser2([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [Out, MarshalAs(UnmanagedType.LPArray)]char[] pResult,
        [Out, MarshalAs(UnmanagedType.I4)]int iResultLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int GetYhtCompany([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [Out, MarshalAs(UnmanagedType.LPArray)]char[] pResult,
        [Out, MarshalAs(UnmanagedType.I4)]int iResultLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int GetSubsysGroup([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [Out, MarshalAs(UnmanagedType.LPArray)] char[] pResult,
        [Out, MarshalAs(UnmanagedType.I4)]int iResultLen);

        [DllImport("UFPAClient.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal extern static void GetCurrentID([MarshalAs(UnmanagedType.LPStr)] StringBuilder szID);

        [DllImport("UMiscell.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal extern static int UnlockInfo2([MarshalAs(UnmanagedType.LPStr)] string szSource, [MarshalAs(UnmanagedType.LPStr)] StringBuilder szDest, uint uKey);


        [DllImportAttribute("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetCurrentThreadId();
        [DllImportAttribute("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetCurrentProcessId();
        [DllImportAttribute("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetProcessHeap();

        [DllImportAttribute("UMiscell", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern bool UnlockInfo2([In, MarshalAs(UnmanagedType.LPArray)]char[] sSource,
        [Out, MarshalAs(UnmanagedType.LPArray)]char[] sDest, [In, MarshalAs(UnmanagedType.I4)] int lKey);




        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int OfflineSetUserReleam([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [In, MarshalAs(UnmanagedType.LPArray)]char[] szYhtIdAndReleam,
        [Out, MarshalAs(UnmanagedType.LPArray)] char[] pResult,
        [Out, MarshalAs(UnmanagedType.I4)] out int iResultLen);


        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int SyncYhtLic([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [In, MarshalAs(UnmanagedType.LPArray)]char[] szID,
        [Out, MarshalAs(UnmanagedType.LPArray)] char[] szErrMsg,
        [In, Out, MarshalAs(UnmanagedType.I4)] ref int iErrMsgLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int GetSyncYhtLicState([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [In, MarshalAs(UnmanagedType.I4)]int lSyncHandler,
        [Out, MarshalAs(UnmanagedType.LPArray)] char[] pResult,
        [Out, MarshalAs(UnmanagedType.I4)]  out int iErrMsgLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int GetLicenseState([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [Out, MarshalAs(UnmanagedType.LPArray)] char[] pResult,
        [Out, MarshalAs(UnmanagedType.I4)]  int iResultLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int LicenceBackup([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [In, MarshalAs(UnmanagedType.LPArray)]char[] szBackupPath,
        [Out, MarshalAs(UnmanagedType.LPArray)] char[] szResponse,
        [In, Out, MarshalAs(UnmanagedType.I4)] ref int iResponseLen);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int LicenceRestore([In, MarshalAs(UnmanagedType.LPArray)]char[] szServer,
        [In, MarshalAs(UnmanagedType.LPArray)]char[] szBackupPath,
        [Out, MarshalAs(UnmanagedType.LPArray)] char[] szResponse,
        [In, Out, MarshalAs(UnmanagedType.I4)] ref int iResponseLen);


        internal static void NewDecode(char[] cIn, ref string cOut)
        {
            int key;
            key = GenerateKey();
            char[] cDest = new char[cIn.Length];

            UnlockInfo2(cIn, cDest, key);
            StringBuilder sTar = new StringBuilder();
            foreach (char s in cDest)
            {
                if (s == '\u0000')
                {
                    break;
                }
                else
                {
                    sTar.Append(s);
                }

            }
            cOut = sTar.ToString();
        }

        internal static int GenerateKey()
        {
            int hHeap;
            int dwThreadID;
            int dwProcessID;
            int dwKey;


            hHeap = GetProcessHeap();
            dwThreadID = GetCurrentThreadId();
            dwProcessID = GetCurrentProcessId();


            dwKey = (hHeap << 2) | (dwThreadID << 16);
            dwKey = dwKey + dwProcessID;
            dwKey = dwKey + (dwThreadID >> 2);

            return dwKey;
        }


        internal static char[] Arrayconcat(byte[] array, string str)
        {
            char[] temp = new char[str.Length + array.GetLength(0)];
            Array.Copy(Bytetostring(array), 0, temp, 0, array.GetLength(0));
            Array.Copy(str.ToCharArray(), 0, temp, array.GetLength(0), str.Length);
            return temp;
        }

        internal static char[] Bytetostring(byte[] value)
        {
            char[] chararray = new char[value.GetLength(0)];
            for (int i = 0; i < value.GetLength(0); i++)
            {
                chararray[i] = Convert.ToChar(value[i]);
            }
            return chararray;
        }

        internal Int32 GenerateDecodeKey()
        {
            int hHeap = GetProcessHeap();
            int uThreadId = GetCurrentThreadId();
            int uProcessId = GetCurrentProcessId();
            return ((hHeap << 2) | uThreadId << 16) + uProcessId + (uThreadId >> 2);
        }


        internal static List<AuthGroup> GetAuthGroupInfo()
        {
            //商务模块编号
            //模块分组信息  [{"c":["AP","AR"],"p":"HM"},{"c":["IA","MR"],"p":"HP"},{"c":["WA","EG"],"p":"EP"}]
            char[] resultData = new char[2342300];
            int resultLength = 0;
            string resultStr = "";
            CloudRegisterUtil.GetSubsysGroup(Environment.MachineName.ToCharArray(), resultData, resultLength);
            CloudRegisterUtil.NewDecode(resultData, ref resultStr);

            if (resultStr.Contains("failed"))
            {
                string errInfo = GetErrInfo(resultStr);
                throw new Exception(errInfo);
            }

            List<AuthGroup> ls = new List<CloudRegisterManage.AuthGroup>();
            if (resultStr != "{}")
            {

                try
                {
                    ls = JsonConvert.DeserializeObject<List<AuthGroup>>(resultStr);
                }
                catch (Exception)
                {

                    throw new Exception(resultStr);
                }

            }

            ls = ls.Where(o => o.p != "DK" && o.p != "J7").ToList();
            return ls;

        }

        internal static string GetErrInfo(string errCode)
        {
            if (errCode.Contains("succ"))
            {
                return "导入成功";

            }
            else if (errCode.Contains("failed: 1037"))
            {
                return "导入的许可文件不正确：签名校验不通过";
            }
            else if (errCode.Contains("failed: 1038"))
            {
                return "导入的许可文件不正确：硬件特征码不匹配";
            }
            else if (errCode.Contains("failed: 2002"))
            {
                return "连接到服务器的过程出现意外";
            }

            return errCode;
        }
        public static BrushConverter brushConverter = new BrushConverter();
        public static void BackgroudColor(TreeViewItem item, int usedNum, string allNum)
        {
            if (CheckOverflow(usedNum, allNum))
            {
                //item.Background = (Brush)brushConverter.ConvertFromString("#FFF74D27");
                item.Foreground = (Brush)brushConverter.ConvertFromString("#FFF74D27");
            }
            else
            {
                //item.Background = (Brush)brushConverter.ConvertFromString("#FFFFFF");
                item.Foreground = (Brush)brushConverter.ConvertFromString("#FF000000");
            }
        }
        public static bool CheckOverflow(int usedNum, string allNum)
        {
            int totalNum = 0;
            bool result = false;
            if (int.TryParse(allNum, out totalNum) && usedNum > totalNum)
            {
                result = true;
            }
            return result;
        }

        internal static string GetYHTAllUserInfoNew()
        {
            char[] resultData = new char[6342300];
            int resultLength = 0;
            string resultStr = "";
            string resultUserAll = "";



            CloudRegisterUtil.GetYhtUserCount(Environment.MachineName.ToCharArray(), resultData, ref resultLength);
            CloudRegisterUtil.NewDecode(resultData, ref resultStr);

            int count = 0;
            int.TryParse(resultStr, out count);
            int page = 0;
            while (count > 0)
            {
                string restlt1 = "";
                CloudRegisterUtil.GetYhtAllUserPaged(Environment.MachineName.ToCharArray(), page, 100, resultData, ref resultLength);
                CloudRegisterUtil.NewDecode(resultData, ref restlt1);
                resultUserAll = resultUserAll + restlt1 + "\r\n";

                count = count - 100;
                page++;

            }

            return resultUserAll;

        }

        internal static string GetYHTAllUserInfo2New()
        {
            char[] resultData = new char[6342300];
            int resultLength = 0;
            string resultStr = "";
            string resultUserAll = "";



            CloudRegisterUtil.GetYhtUserCount(Environment.MachineName.ToCharArray(), resultData, ref resultLength);
            CloudRegisterUtil.NewDecode(resultData, ref resultStr);

            int count = 0;
            int.TryParse(resultStr, out count);
            int page = 0;
            while (count > 0)
            {
                string restlt1 = "";
                CloudRegisterUtil.GetYhtAllUser2Paged(Environment.MachineName.ToCharArray(), page, 100, resultData, ref resultLength);
                CloudRegisterUtil.NewDecode(resultData, ref restlt1);
                resultUserAll = resultUserAll + restlt1 + "\r\n";

                count = count - 100;
                page++;

            }

            return resultUserAll;

        }


        internal static List<YhtUserInfo> GetYHTAllUserInfo()
        {

            char[] resultData = new char[6342300];
            int resultLength = 0;

            //0,ybt@yonyou.com,18612104080,38922574-be3b-4fca-97d2-ee708d842800,杨博涛,ybt,ybt1
            string resultStr1 = GetYHTAllUserInfoNew();
            var userArray1 = resultStr1.Split("\r\n".ToArray()).Where(o => o != "" && o.Split(',').Length >= 4);

            //0,ybt@yonyou.com,18612104080,38922574-be3b-4fca-97d2-ee708d842800,杨博涛,HM,DK
            string resultStr2 = GetYHTAllUserInfo2New();
            var userArray2 = resultStr2.Split("\r\n".ToArray()).Where(o => o != "" && o.Split(',').Length >= 4);

            var query = from user1 in userArray1
                        where (user1 != "" && user1.Split(',').Length >= 4)
                        join user2 in userArray2
                        on user1.Split(',')[3] equals user2.Split(',')[3]
                        select new { user1, user2 };

            List<YhtUserInfo> listResult = new List<YhtUserInfo>();

            foreach (var obj in query)
            {
                string user1 = obj.user1;

                if (user1 == "") continue;
                string[] array1 = user1.Split(',');
                if (array1.Length < 4) continue;

                YhtUserInfo userInfo = new YhtUserInfo();
                if (array1[0] == "1")
                {
                    userInfo.State = "已认证";
                }
                else
                {
                    userInfo.State = "未认证";
                }
                userInfo.Email = array1[1];
                userInfo.Mobile = array1[2];
                userInfo.UserCloudId = array1[3];
                userInfo.UserCloudName = array1[4];

                var u8Users = array1.Skip(5).ToArray();
                int length = u8Users.Length;
                Dictionary<string, string> dic = new Dictionary<string, string>();


                for (int i = 0; i < length; i++)
                {
                    if (i % 2 != 0) continue;
                    if (!dic.Keys.Contains(u8Users[i])) dic.Add(u8Users[i], u8Users[i + 1]);
                }

                string u8UsersStr = "";
                foreach (var item in dic)
                {
                    u8UsersStr += "(" + item.Key + " " + item.Value + "),";
                }
                u8UsersStr = u8UsersStr.TrimEnd(',');

                userInfo.U8Users = dic;
                userInfo.U8UsersStr = u8UsersStr;
                userInfo.Groups = obj.user2.Split(',').Skip(5).ToList(); ;
                listResult.Add(userInfo);

            }

            return listResult;

        }


        internal static List<UserLoginState> GetLoginedInfo()
        {

            Dictionary<string, string> dicGroupTitle = GetGroupTitle();
            Dictionary<string, string> dicModuleTitle = GetModuleTitle();

            //取当前产品登录情况
            string szCommand = string.Format("ListAllSubSys,0,9801,,{0},0", GetU8ProductID());
            szCommand = szCommand.Replace(',', (char)0x1D);
            StringBuilder szOut = new StringBuilder(2048000);
            //取缓冲区长度
            int len = CloudRegisterUtil.RPCCall2(Environment.MachineName, szCommand, szOut);

            //{ 34E5A54F - C842 - 4C2B - 8B5E - 3E12942FE01C}WIN - O0LMUCVN1RG@2,C,SA,HJ,18211090782,108305496,0,1,1,lizhq1,lizhq1
            string resultStr = szOut.ToString();

            var list = resultStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(o => o != "" && o.Split(',').Length >= 5);
            List<UserLoginState> result = new List<CloudRegisterManage.UserLoginState>();

            foreach (var item in list)
            {
                string[] userStrArray = item.Split(new char[] { ',' }, StringSplitOptions.None);

                UserLoginState user = new UserLoginState();
                user.State = "已登录";
                user.ClientID = userStrArray[0];

                if (user.ClientID.Contains("@@"))
                {
                    user.ClientPCName = user.ClientID.Split('}')[1].Split("@@".ToCharArray())[0];
                }
                else if (user.ClientID.Contains("}"))
                {
                    user.ClientPCName = user.ClientID.Split('}')[1].Split('@')[0];
                }
                else if (user.ClientID.Contains("|"))
                {
                    user.ClientPCName = user.ClientID.Split('|')[0].Split('@')[1];
                }
                else if (user.ClientID.Contains("["))
                {
                    user.ClientPCName = user.ClientID.Split('[')[0];
                }

                switch (userStrArray[1])
                {
                    case "C":
                        {
                            user.PortalName = "CS门户";
                            break;
                        }
                    case "B":
                        {
                            user.PortalName = "BS门户";
                            break;
                        }
                    case "H":
                        {
                            user.PortalName = "CRM门户";
                            break;
                        }
                    case "E":
                        {
                            user.PortalName = "电商门户";
                            break;
                        }
                    default:
                        break;
                }

                user.ModuleId = userStrArray[2];
                if (!dicModuleTitle.Keys.Contains(user.ModuleId)) continue;  //该模块不控制许可
                user.ModuleName = dicModuleTitle[user.ModuleId];
                user.GroupId = userStrArray[3];
                if (dicGroupTitle.Keys.Contains(user.GroupId))
                {
                    user.GroupName = dicGroupTitle[user.GroupId];
                }

                user.UserCloudId = userStrArray[4];
                user.TaskID = userStrArray[5];

                if (userStrArray[6] == "-1")
                {
                    user.BException = "异常(-1)";
                }
                else
                {
                    user.BException = "正常(" + userStrArray[6] + ")";
                }

                if (userStrArray[7] == "1")
                {
                    user.BDemo = "是";
                }
                else
                {
                    user.BDemo = "否";
                }

                if (userStrArray[8] == "1")
                {
                    user.BCount = "是";
                }
                else
                {
                    user.BCount = "否";
                }

                user.UserId = userStrArray[9];
                user.UserName = userStrArray[10];

                result.Add(user);
            }



            return result;
        }

        private static string GetU8ProductID()
        {
            RegistryKey productKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\UfSoft\\WF\\V8.700\\GUID");
            if (productKey != null)
            {
                return productKey.GetValue("").ToString();
            }
            return Guid.NewGuid().ToString();
        }


        internal static Dictionary<string, string> GetGroupTitle()
        {

            Dictionary<string, string> dicGroupTitle = new Dictionary<string, string>();
            //添加分组信息
            dicGroupTitle.Add("DP", "UAP设计平台");
            dicGroupTitle.Add("HT", "UU");
            dicGroupTitle.Add("KE", "U8远程");
            dicGroupTitle.Add("J6", "全员应用");
            dicGroupTitle.Add("HJ", "财务会计");
            dicGroupTitle.Add("HP", "客户关系管理");
            dicGroupTitle.Add("HM", "生产制造");
            dicGroupTitle.Add("HN", "人力资源");
            dicGroupTitle.Add("HK", "管理会计");
            dicGroupTitle.Add("HO", "集团应用");
            dicGroupTitle.Add("HG", "电商管理");
            dicGroupTitle.Add("HL", "供应链");
            return dicGroupTitle;
        }

        internal static CompanyInfo GetCompanyInfo()
        {
            char[] resultData = new char[2342300];
            int resultLength = 0;
            string resultStr = "";
            GetYhtCompany(Environment.MachineName.ToCharArray(), resultData, resultLength);
            NewDecode(resultData, ref resultStr);
            return null;
        }

        /// <summary>
        /// U8模块 的名称
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, string> GetModuleTitle()
        {
            Dictionary<string, string> dicModuleTitle = new Dictionary<string, string>();
            //添加分组信息
            string path = Environment.SystemDirectory + "\\U8Product.xml";
            string U8ProductInfo = File.ReadAllText(path);
            XDocument doc = XDocument.Parse(U8ProductInfo);

            string langid = Thread.CurrentThread.CurrentUICulture.Name;

            foreach (var item in doc.Root.Elements())
            {
                string id = item.Attribute("ID").Value;
                if (dicModuleTitle.Keys.Contains(id)) continue;
                dicModuleTitle.Add(id, item.Attribute(langid).Value);

            }

            return dicModuleTitle;
        }


        #region  wpf客户端 导出DataGrid数据到Excel

        /// <summary>
        /// CSV格式化
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>格式化数据</returns>
        private static string FormatCsvField(string data)
        {
            return String.Format("\"{0}\"", data.Replace("\"", "\"\"\"").Replace("\n", "").Replace("\r", ""));
        }



        /// <summary>
        /// 导出DataGrid数据到Excel
        /// </summary>
        /// <param name="withHeaders">是否需要表头</param>
        /// <param name="grid">DataGrid</param>
        /// <param name="dataBind"></param>
        /// <returns>Excel内容字符串</returns>
        private static string ExportDataGrid(bool withHeaders, System.Windows.Controls.DataGrid grid, bool dataBind)
        {
            try
            {
                var strBuilder = new System.Text.StringBuilder();
                var source = (grid.ItemsSource as System.Collections.IList);
                if (source == null) return "";
                var headers = new List<string>();
                List<string> bt = new List<string>();

                foreach (var hr in grid.Columns)
                {

                    if (hr.Visibility == Visibility.Hidden) continue;
                    //   DataGridTextColumn textcol = hr. as DataGridTextColumn;
                    headers.Add(hr.Header.ToString());
                    if (hr is DataGridTextColumn)//列绑定数据
                    {
                        DataGridTextColumn textcol = hr as DataGridTextColumn;
                        if (textcol != null)
                            bt.Add((textcol.Binding as Binding).Path.Path.ToString());        //获取绑定源      

                    }
                    else if (hr is DataGridTemplateColumn)
                    {
                        if (hr.Header.Equals("操作"))
                            bt.Add("Id");
                    }
                    else
                    {

                    }
                }
                strBuilder.Append(String.Join(",", headers.ToArray())).Append("\r\n");
                foreach (var data in source)
                {
                    var csvRow = new List<string>();
                    foreach (var ab in bt)
                    {
                        string s = ReflectionUtil.GetProperty(data, ab).ToString();
                        if (s != null)
                        {
                            csvRow.Add(FormatCsvField(s));
                        }
                        else
                        {
                            csvRow.Add("\t");
                        }
                    }
                    strBuilder.Append(String.Join(",", csvRow.ToArray())).Append("\r\n");
                    // strBuilder.Append(String.Join(",", csvRow.ToArray())).Append("\t");
                }
                return strBuilder.ToString();
            }
            catch (Exception)
            {

                return "";
            }
        }
        /// <summary>
        /// 导出DataGrid数据到Excel为CVS文件
        /// 使用utf8编码 中文是乱码 改用Unicode编码
        /// 
        /// </summary>
        /// <param name="withHeaders">是否带列头</param>
        /// <param name="grid">DataGrid</param>
        internal static void ExportDataGridSaveAs(bool withHeaders, System.Windows.Controls.DataGrid grid)
        {
            try
            {
                string data = ExportDataGrid(true, grid, true);
                var sfd = new Microsoft.Win32.SaveFileDialog
                {
                    DefaultExt = "csv",
                    Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*",
                    FilterIndex = 1
                };
                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        using (var writer = new StreamWriter(stream, System.Text.Encoding.Unicode))
                        {
                            data = data.Replace(",", "\t");
                            writer.Write(data);
                            writer.Close();
                        }
                        stream.Close();
                    }
                    MessageBox.Show("导出成功！");
                }

            }
            catch (Exception)
            {

            }
        }

        #endregion 导出DataGrid数据到Excel

    }


    internal class ReflectionUtil
    {

        public static object GetProperty(object obj, string propertyName)
        {
            PropertyInfo info = obj.GetType().GetProperty(propertyName);

            if (info == null && propertyName.Split('.').Count() > 0)
            {
                object o = ReflectionUtil.GetProperty(obj, propertyName.Split('.')[0]);
                int index = propertyName.IndexOf('.');
                string end = propertyName.Substring(index + 1, propertyName.Length - index - 1);
                return ReflectionUtil.GetProperty(o, end);
            }

            object result = null;

            try
            {
                result = info.GetValue(obj, null);
            }

            catch (TargetException)
            {
                return "";
            }

            return result == null ? "" : result;

        }

    }


    public class YhtUserInfo
    {
        /// <summary>
        /// 认证状态 状态
        /// </summary>
        public string State { get; set; }

        public string Email { get; set; }
        public string UserCloudId { get; set; }
        public string UserCloudName { get; set; }
        public string Mobile { get; set; }

        /// <summary>
        /// 对应的U8用户信息 ，一个友户通ID可能对应多个U8账号
        /// </summary>
        public Dictionary<string, string> U8Users { get; set; }

        /// <summary>
        /// 对应的U8用户信息 界面显示用
        /// </summary>
        public string U8UsersStr { get; set; }

        /// <summary>
        /// 用户 可以使用的领域分组
        /// </summary>
        public List<string> Groups { get; set; }

        /// <summary>
        /// 是否被选中，界面使用
        /// </summary>
        public bool Checked { get; set; }
        public YhtUserInfo Copy()
        {
            return new YhtUserInfo()
            {
                State = this.State,
                Email = this.Email,
                UserCloudId = this.UserCloudId,
                UserCloudName = this.UserCloudName,
                Mobile = this.Mobile,
                U8Users = this.U8Users,
                U8UsersStr = this.U8UsersStr,
                Groups = this.Groups,
                Checked = this.Checked
            };
        }

    }

    public class UserLoginState
    {
        /// <summary>
        ///友户通ID
        /// </summary>
        public string UserCloudId { get; set; }

        /// <summary>
        ///友户通名称
        /// </summary>
        public string UserCloudName { get; set; }

        /// <summary>
        ///操作员编码
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///操作员名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///状态
        /// </summary>
        public string State { get; set; }

        /// <summary>
        ///模块编号
        /// </summary>
        public string ModuleId { get; set; }
        /// <summary>
        ///模块名称
        /// </summary>
        public string ModuleName { get; set; }

        public string ModuleForShow { get; set; }

        /// <summary>
        ///模块 所属领域分组编号
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        ///模块 所属领域分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        ///手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        ///登录的门户
        /// </summary>
        public string PortalName { get; set; }

        /// <summary>
        ///客户端机器名
        /// </summary>
        public string ClientPCName { get; set; }

        /// <summary>
        ///客户端标识
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        ///任务ID
        /// </summary>
        public string TaskID { get; set; }

        /// <summary>
        ///异常情况
        /// </summary>
        public string BException { get; set; }

        /// <summary>
        ///是否演示期
        /// </summary>
        public string BDemo { get; set; }

        /// <summary>
        ///是否计数
        /// </summary>
        public string BCount { get; set; }
    }


    /// <summary>   
    /// 适用与ZIP压缩   
    /// </summary>   
    public class ZipHelper
    {
        #region 压缩  

        /// <summary>   
        /// 递归压缩文件夹的内部方法   
        /// </summary>   
        /// <param name="folderToZip">要压缩的文件夹路径</param>   
        /// <param name="zipStream">压缩输出流</param>   
        /// <param name="parentFolderName">此文件夹的上级文件夹</param>   
        /// <returns></returns>   
        private static bool ZipDirectory(string folderToZip, ZipOutputStream zipStream, string parentFolderName)
        {




            bool result = true;
            string[] folders, files;
            ZipEntry ent = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();

            try
            {
                ent = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/"));
                zipStream.PutNextEntry(ent);
                zipStream.Flush();

                files = Directory.GetFiles(folderToZip);
                foreach (string file in files)
                {
                    fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ent = new ZipEntry(Path.Combine(parentFolderName, Path.GetFileName(folderToZip) + "/" + Path.GetFileName(file)));
                    ent.DateTime = DateTime.Now;
                    ent.Size = fs.Length;

                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    ent.Crc = crc.Value;
                    zipStream.PutNextEntry(ent);
                    zipStream.Write(buffer, 0, buffer.Length);
                }

            }
            catch
            {
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }

            folders = Directory.GetDirectories(folderToZip);
            foreach (string folder in folders)
                if (!ZipDirectory(folder, zipStream, folderToZip))
                    return false;

            return result;
        }

        /// <summary>   
        /// 压缩文件夹    
        /// </summary>   
        /// <param name="folderToZip">要压缩的文件夹路径</param>   
        /// <param name="zipedFile">压缩文件完整路径</param>   
        /// <param name="password">密码</param>   
        /// <returns>是否压缩成功</returns>   
        public static bool ZipDirectory(string folderToZip, string zipedFile, string password)
        {
            bool result = false;
            if (!Directory.Exists(folderToZip))
                return result;

            ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile));
            zipStream.SetLevel(6);

            result = ZipDirectory(folderToZip, zipStream, "");

            zipStream.Finish();
            zipStream.Close();

            return result;
        }

        /// <summary>   
        /// 压缩文件夹   
        /// </summary>   
        /// <param name="folderToZip">要压缩的文件夹路径</param>   
        /// <param name="zipedFile">压缩文件完整路径</param>   
        /// <returns>是否压缩成功</returns>   
        public static bool ZipDirectory(string folderToZip, string zipedFile)
        {
            bool result = ZipDirectory(folderToZip, zipedFile, null);
            return result;
        }

        /// <summary>   
        /// 压缩文件   
        /// </summary>   
        /// <param name="fileToZip">要压缩的文件全名</param>   
        /// <param name="zipedFile">压缩后的文件名</param>   
        /// <param name="password">密码</param>   
        /// <returns>压缩结果</returns>   
        public static bool ZipFile(string fileToZip, string zipedFile, string password)
        {
            bool result = true;
            ZipOutputStream zipStream = null;
            FileStream fs = null;
            ZipEntry ent = null;

            if (!File.Exists(fileToZip))
                return false;

            try
            {
                fs = File.OpenRead(fileToZip);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                fs = File.Create(zipedFile);
                zipStream = new ZipOutputStream(fs);

                ent = new ZipEntry(Path.GetFileName(fileToZip));
                zipStream.PutNextEntry(ent);
                zipStream.SetLevel(6);

                zipStream.Write(buffer, 0, buffer.Length);

            }
            catch
            {
                result = false;
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Finish();
                    zipStream.Close();
                }
                if (ent != null)
                {
                    ent = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            GC.Collect();
            GC.Collect(1);

            return result;
        }

        /// <summary>   
        /// 压缩文件   
        /// </summary>   
        /// <param name="fileToZip">要压缩的文件全名</param>   
        /// <param name="zipedFile">压缩后的文件名</param>   
        /// <returns>压缩结果</returns>   
        public static bool ZipFile(string fileToZip, string zipedFile)
        {
            bool result = ZipFile(fileToZip, zipedFile, null);
            return result;
        }

        /// <summary>   
        /// 压缩文件或文件夹   
        /// </summary>   
        /// <param name="fileToZip">要压缩的路径</param>   
        /// <param name="zipedFile">压缩后的文件名</param>   
        /// <param name="password">密码</param>   
        /// <returns>压缩结果</returns>   
        public static bool Zip(string fileToZip, string zipedFile, string password)
        {
            bool result = false;
            if (Directory.Exists(fileToZip))
                result = ZipDirectory(fileToZip, zipedFile, password);
            else if (File.Exists(fileToZip))
                result = ZipFile(fileToZip, zipedFile, password);

            return result;
        }

        /// <summary>   
        /// 压缩文件或文件夹   
        /// </summary>   
        /// <param name="fileToZip">要压缩的路径</param>   
        /// <param name="zipedFile">压缩后的文件名</param>   
        /// <returns>压缩结果</returns>   
        public static bool Zip(string fileToZip, string zipedFile)
        {
            bool result = Zip(fileToZip, zipedFile, null);
            return result;

        }

        #endregion

        #region 解压  

        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <param name="password">密码</param>   
        /// <returns>解压结果</returns>   
        public static bool UnZip(string fileToUnZip, string zipedFolder, string password)
        {
            bool result = true;

            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!File.Exists(fileToUnZip))
                return false;

            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);

            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi   

                        if (fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }

                        using (FileStream fs = File.Create(fileName))
                        {
                            int size = 2048;
                            byte[] data = new byte[size];
                            while (true)
                            {
                                size = zipStream.Read(data, 0, data.Length);
                                if (size > 0)
                                    fs.Write(data, 0, data.Length);
                                else
                                    break;
                            }
                        }

                    }
                }
            }
            catch
            {
                result = false;
            }
            finally
            {

                if (zipStream != null)
                {

                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }

            }
            return result;
        }

        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <returns>解压结果</returns>   
        public static bool UnZip(string fileToUnZip, string zipedFolder)
        {
            bool result = UnZip(fileToUnZip, zipedFolder, null);
            return result;
        }

        #endregion
    }

    internal class CompanyInfo
    {
        /// <summary>
        /// 
        /// </summary>
        //string LicenseType = "";


    }

}

