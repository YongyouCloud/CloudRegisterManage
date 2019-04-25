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

namespace CloudRegisterManage
{
    /// <summary>
    /// Interaction logic for SysOptionsView.xaml
    /// </summary>
    public partial class BackupLic : UserControl
    {
        private static readonly Regex rCmdadi2Err = new Regex(@"^failed: (\d+), (.+)$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        public BackupLic()
        {
            InitializeComponent();

        }

        private void btnBackupLic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "许可备份文件|*.u8bak";
                dialog.FileName = "licence.u8bak";
                if (dialog.ShowDialog() == true)
                {
                    StringBuilder szID = new StringBuilder(1024);
                    //CloudRegisterUtil.GetCurrentID(szID);

                    char[] resultData = new char[2342300];
                    int key = CloudRegisterUtil.GenerateKey();
                    int resultLength = 0;
                    CloudRegisterUtil.LicenceBackup(Environment.MachineName.ToCharArray(), dialog.FileName.ToCharArray(), resultData, ref resultLength);
                    StringBuilder szAuths = new StringBuilder(65536);
                    CloudRegisterUtil.UnlockInfo2(new string(resultData), szAuths, (uint)key);

                    string resultStr = szAuths.ToString();
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
                        MessageBox.Show("备份成功");
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
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            //licence.yht
        }

        private void btnRecovery_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "许可备份文件|*.u8bak";
                dialog.FileName = "licence.u8bak";
                if (dialog.ShowDialog() == true)
                {
                    FileInfo licFile = new FileInfo(dialog.FileName);
                    List<string> filterExtension = new List<string>() { ".u8bak" };
                    if (!filterExtension.Contains(licFile.Extension.ToLower()))
                    {

                        MessageBox.Show("请导入许可类型文件,此文件非官方导出文件");
                        return;
                    }
                    StringBuilder szID = new StringBuilder(1024);
                    char[] resultData = new char[2342300];
                    int key = CloudRegisterUtil.GenerateKey();
                    int resultLength = 0;
                    CloudRegisterUtil.LicenceRestore(Environment.MachineName.ToCharArray(), dialog.FileName.ToCharArray(), resultData, ref resultLength);
                    StringBuilder szAuths = new StringBuilder(65536);
                    CloudRegisterUtil.UnlockInfo2(new string(resultData), szAuths, (uint)key);

                    string resultStr = szAuths.ToString();
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
                        MessageBox.Show("恢复成功");
                        //string companyInfo = GetCompanyInfo();
                        //if (companyInfo == "" || companyInfo == ",null,,,," || companyInfo.Contains("failed:"))
                        //{
                        //    return;
                        //}
                        //string type = companyInfo.Split(',')[1];
                        //switch (type)
                        //{
                        //    case "online":
                        //    case "adviser":
                        //    case "onlineborrow":
                        //    case "ontrial":
                        //        MessageBox.Show("许可文件更新成功。为避免无法正常使用产品，请尽快登录U8系统管理，在用户管理界面执行同步操作");
                        //        break;
                        //    case "offline":
                        //    case "offlineborrow":
                        //        MessageBox.Show("许可文件更新成功。为避免无法正常使用产品，请尽快按照如下步骤操作：\n\r1、登录U8系统管理，在用户管理界面执行同步操作；\n\r2、在云注册管理的分配许可界面，执行许可分配；");
                        //        break;
                        //    default:
                        //        MessageBox.Show("导入成功");
                        //        break;
                        //}
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

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
    }
}
