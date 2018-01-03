using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MonitorPorts
{
    public partial class Warning : Form
    {
        string[] NetworkCard;
        public static string SelectedNetCard;
        int NumOfNetCard = 0;

        public Warning(string[] _networkCard)
        {
            InitializeComponent();
            this.NetworkCard = _networkCard;
            CreateCheck();
        }

        public void CreateCheck()
        {
            foreach (var item in NetworkCard)
            {
                NetCardListCheckbox.Items.Add(item);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            int SelectedNetCardCount = this.NetCardListCheckbox.CheckedItems.Count;
            for (int i = 0; i < NetCardListCheckbox.Items.Count; i++)
            {
                if (NetCardListCheckbox.GetItemChecked(i))
                {
                    SelectedNetCard = NetCardListCheckbox.Items[i].ToString();
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NetCardListCheckbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < NetCardListCheckbox.Items.Count; i++)
            {
                NetCardListCheckbox.SetItemChecked(i, false);
            }
            if (NetCardListCheckbox.CheckedItems == null)
            {
                NetCardListCheckbox.SetItemChecked(NetCardListCheckbox.SelectedIndex, false);
            }
            else
            {
                NetCardListCheckbox.SetItemChecked(NetCardListCheckbox.SelectedIndex, true);
            }
        }

    }
}
