using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CloudRegisterManage
{
    public class CloudRegisterUtil
    {
        private const int BUFFSIZE = 20480;
        private const char RPCDELIMITER = '\u001D';//RPCCALL命令的分割符

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int RPCCall([In, MarshalAs(UnmanagedType.LPArray)]char[] cServer,
        [In, MarshalAs(UnmanagedType.LPArray)]char[] cIn,
        [Out, MarshalAs(UnmanagedType.LPArray)]char[] cOut);

        [DllImportAttribute("UFPAClient.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern
        int ImportYhtLic([In, MarshalAs(UnmanagedType.LPArray)]char[] cServer,
        [In, MarshalAs(UnmanagedType.LPArray)]byte[] fz,
        [In, MarshalAs(UnmanagedType.I4)]int fLength,
        [Out, MarshalAs(UnmanagedType.LPArray)]byte[] response,
        [Out, MarshalAs(UnmanagedType.I4)]int responseLen);

        [DllImportAttribute("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetCurrentThreadId();
        [DllImportAttribute("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetCurrentProcessId();
        [DllImportAttribute("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetProcessHeap();

        [DllImportAttribute("UMiscell", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern bool UnlockInfo2([In, MarshalAs(UnmanagedType.LPArray)]char[] sSource,
        [Out, MarshalAs(UnmanagedType.LPArray)]char[] sDest, [In, MarshalAs(UnmanagedType.I4)] int lKey);

        internal static void NewDecode(char[] cIn, ref string cOut)
        {
            int key;
            key = GenerateKey();
            char[] cDest = new char[BUFFSIZE];

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

    }
}
