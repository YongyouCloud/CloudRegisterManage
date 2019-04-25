using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CloudRegisterManage
{
    public partial class LicUseViewForm : Form
    {
        public LicUseViewForm()
        {
            InitializeComponent();
            initTv();
        }


        private void initTv()
        {
            TreeNode nodeRoot = tvPdrduct.Nodes.Add("U8V13.1产品卡(在线)");

            TreeNode node = nodeRoot.Nodes.Add("领域");
            node.Nodes.Add("供应链领域[1/2]");
            node.Nodes.Add("生产制造领域[1/2]");

        }

    }
}
