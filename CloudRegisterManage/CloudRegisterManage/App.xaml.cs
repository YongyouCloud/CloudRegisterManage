using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace CloudRegisterManage
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string appPath;
        public static String AppPath
        {
            get
            {
                if(appPath != null)
                {
                    return appPath;
                }
                DirectoryInfo currentPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
                if (currentPath.Name.ToLower() == "debug")
                {
                    appPath = currentPath.Parent.Parent.FullName;
                }
                else
                {
                    appPath = currentPath.FullName;
                }

                App.Current.Properties.Add("apppath", appPath);

                return appPath;
            }
        }
    }
}
