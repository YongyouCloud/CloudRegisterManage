using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CloudRegisterManage.UI
{
    /// <summary>
    /// Interaction logic for ListMenuButton.xaml
    /// </summary>
    public partial class ListMenuButton : UserControl
    {
        
        public ListMenuButton()
        {
            InitializeComponent();
            this.MouseEnter += ListMenuButton_MouseEnter;
            this.MouseLeave += ListMenuButton_MouseLeave;
            this.IsEnabledChanged += ListMenuButton_IsEnabledChanged;
            this.MouseUp += ListMenuButton_MouseUp;
        }

        public event EventHandler MenuSelected;

        private void ListMenuButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Selected)
            {
                return;
            }
            this.Selected = true;
            
        }

        private void ListMenuButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEnabled)
            {
                this.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                this.Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            }
        }

        public string Title
        {
            get { return titleText.Text; }
            set { titleText.Text = value; }
        }

        private void ListMenuButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.selected)
            {
                this.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void ListMenuButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.selected)
            {
                this.Background = new SolidColorBrush(Color.FromArgb(30,255,255,255));
            }
        }


        private bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set {
                selected = value;
                if (value) { 
                    this.Background = new SolidColorBrush(Color.FromArgb(255,60,90,110));
                    if (this.MenuSelected != null)
                    {
                        MenuSelected(this, EventArgs.Empty);
                    }
                }
                else if(this.IsEnabled)
                {
                    this.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }



    }
}
