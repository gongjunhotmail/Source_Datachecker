using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NSXYPlot
{
    public partial class XYPlotsSettingDlg : Form
    {
        public XYPlotsSetting XYPlotsSetting;

        public XYPlotsSettingDlg()
        {
            InitializeComponent();
        }

        private void XYPlotsSettingDlg_Load(object sender, EventArgs e)
        {

        }
    }

    public class XYPlotsSetting
    {
        public bool IsPortrait;
        public int TotalXYPlots;
        public XYPlotSetting[] XYPlotSettings;

        // width/height in percentage of plots
        public double[] Widths;
        public bool[] ShowPlot;
    }
}
