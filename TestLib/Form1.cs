using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using TPLinkSmartDevices.Devices;

namespace TestLib
{
    public partial class Form1 : MetroForm
    {
        public Form1()
        {
            InitializeComponent();
            listView1.Columns.Add("Alias");
            listView1.Columns.Add("DeviceId");
            listView1.Columns.Add("Hostname");
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            RefreshData().Wait(5000);
        }

        private async Task RefreshData()
        {
            metroToggle1.Enabled = false;
            button1.Enabled = false;
            listView1.Items.Clear();

            // Runs in a Task<List<TPLinkSmartDevice>>
            var discoveredDevices = await new TPLinkSmartDevices.TPLinkDiscovery().Discover();
            Debug.WriteLine(discoveredDevices.Count);
            foreach (var device in discoveredDevices)
            {
                var lvi = listView1.Items.Add(device.Alias);
                lvi.SubItems.Add(device.DeviceId);
                lvi.SubItems.Add(device.Hostname);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            metroToggle1.Enabled = (discoveredDevices.Count > 0);
            button1.Enabled = true;


        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await RefreshData();
        }

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            var hostname = listView1.SelectedItems[0].SubItems[2].Text;
            TPLinkSmartPlug plug = new TPLinkSmartPlug(hostname);
            plug.OutletPowered = metroToggle1.Checked;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                metroToggle1.Enabled = true;

                // Get plug state
                var hostname = listView1.SelectedItems[0].SubItems[2].Text;
                TPLinkSmartPlug plug=new TPLinkSmartPlug(hostname);
                // Update toggle
                metroToggle1.Checked = plug.OutletPowered;
            }
            else
            {
                metroToggle1.Enabled = false;
            }
        }
    }
}
