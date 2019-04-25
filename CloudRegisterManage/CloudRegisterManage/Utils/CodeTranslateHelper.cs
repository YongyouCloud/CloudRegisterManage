using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UFSoft.U8.Framework.Resource;

namespace CloudRegisterManage
{
    public class CodeTranslateHelper
    {

        public static string CutString(string sourceStr, string startStr, string endStr, string fitStr = "")
        {

            string result = string.Empty;
            Regex reg = new Regex(@"(?<=" + startStr + ")(." + fitStr + "+?)(?=" + endStr + ")");

            Match match = reg.Match(sourceStr);
            if (match.Success)
            {
                result = match.Value;
            }
            return result;
        }
        public static string GetStrByCode(string code)
        {
            return ResourceHelper.GetString(code);
        }
        public static string GetFormatterString(string resId, object[] args)
        {
            return ResourceFormatter.GetString(resId, args);
        }


        private static readonly Regex rCmdadi2Err = new Regex(@"^failed: (\d+), (.+)$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex rParam = new Regex(@"\[(.*?)\]", RegexOptions.Singleline | RegexOptions.Compiled);

        public static bool GetUfpaMessage(string sInput, out int iErrCode, out string sMsg)
        {
            iErrCode = 0;
            sMsg = String.Empty;
            Match me = rCmdadi2Err.Match(sInput);
            if (me.Success)
            {
                if (me.Groups.Count != 3) return false;
                string sErrCode = me.Groups[1].Value;

                if (!Int32.TryParse(sErrCode, out iErrCode))
                {
                    return false;
                }
                sMsg = me.Groups[2].Value;
                return true;
            }
            return false;
        }


        public static string GetYhtResid(int iErrCode, string sErrMsg, out object[] resArg)
        {
            string errResID = "U8.AA.LoginBO.YhtErrCode" + iErrCode;


            var ms = rParam.Matches(sErrMsg);
            if (ms == null)
            {
                resArg = null;
                return errResID;
            }


            object[] a = new object[ms.Count];
            int j = 0;
            foreach (Match m in ms)
            {
                a[j++] = m.Groups[1].Value;
            }

            resArg = a;
            return errResID;
        }
    }
}
