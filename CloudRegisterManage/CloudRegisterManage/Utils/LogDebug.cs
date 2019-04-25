using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CloudRegisterManage
{
    public class LogDebug
    {
        static string filepath = @"C:\logtemp\cloudRegisterlog.txt";
        public static void WriteLog(string message)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    StreamWriter sw = File.AppendText(filepath);
                    sw.WriteLine("" + DateTime.Now + "  " + message);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
