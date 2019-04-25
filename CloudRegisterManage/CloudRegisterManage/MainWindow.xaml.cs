using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using CloudRegisterManage.UI;
using MessageBox = System.Windows.MessageBox;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using TextBox = System.Windows.Controls.TextBox;
using System.ServiceProcess;
using System.Diagnostics;

namespace CloudRegisterManage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFunctionList();
        }

        private void LoadFunctionList()
        {

            foreach (var item in funcPanel.Children)
            {
                ListMenuButton menu = item as ListMenuButton;
                menu.MenuSelected += Menu_MenuSelected;
            }
        }

        private void Menu_MenuSelected(object sender, EventArgs e)
        {
            try
            {
                this.moduleGrid.Children.Clear();

                foreach (var item in funcPanel.Children)
                {
                    ListMenuButton menu = item as ListMenuButton;

                    if (!menu.Equals(sender))
                    {
                        menu.Selected = false;
                    }
                    else
                    {

                        switch (menu.Name)
                        {
                            case "regProduct":
                                {
                                    this.moduleGrid.Children.Add(new RegProduct());
                                    break;
                                }
                            case "importLic":
                                {
                                    ImportLic importLic = new CloudRegisterManage.ImportLic();
                                    importLic.ShowLicClick += ImportLic_ShowLicClick;
                                    this.moduleGrid.Children.Add(importLic);
                                    break;
                                }
                            case "AssignLic":
                                {
                                    this.moduleGrid.Children.Add(new AssignLic());
                                    break;
                                }
                            case "backupLic":
                                {
                                    this.moduleGrid.Children.Add(new BackupLic());
                                    break;
                                }
                            case "showHardCode":
                                {
                                    this.moduleGrid.Children.Add(new hardCode());
                                    break;
                                }
                            case "showLicUseInfo":
                                {
                                    this.moduleGrid.Children.Add(new LicUseInfo());
                                    break;
                                }
                            case "userCompare":
                                {
                                    this.moduleGrid.Children.Add(new UserList());
                                    break;
                                }
                            case "selfService":
                                {
                                    this.moduleGrid.Children.Add(new SelfService());
                                    break;
                                }



                            default:
                                {
                                    this.moduleGrid.Children.Add(new RegProduct());
                                }
                                break;
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ImportLic_ShowLicClick()
        {
            foreach (var item in funcPanel.Children)
            {
                ListMenuButton menu = item as ListMenuButton;
                if (menu.Name == "showLicUseInfo")
                {
                    menu.Selected = true;
                }
                else
                {
                    menu.Selected = false;
                }
            }

        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.SystemDirectory + "\\CloudRegisterHelp.pdf";
            System.Diagnostics.Process.Start(path);
        }
    }


}
