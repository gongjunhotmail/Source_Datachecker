using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BWNSDataset;

namespace NSXYPlot
{
    public partial class TestDlg : Form
    {
        public List<BWDataset> DatasetList;

        public TestDlg()
        {
            InitializeComponent();
        }

        private void TestDlg_Load(object sender, EventArgs e)
        {
            if (xyPlotViewCtrl1.XYPlots == null || xyPlotViewCtrl1.XYPlots.Count < 2) return;
            if (DatasetList == null) return;

            // assign datasets to plots
            foreach (BWDataset dataset in DatasetList)
            {
                if (dataset == null) continue;

                if (dataset.DataType == BWDataType.typeDataResistivity)
                {
                    xyPlotViewCtrl1.XYPlots[0].AddDataset(dataset);
                }
                else if (dataset.DataType == BWDataType.typeDataGamma)
                {
                    xyPlotViewCtrl1.XYPlots[1].AddDataset(dataset);                    
                }
            }

            // resistivity plot
            xyPlotViewCtrl1.XYPlots[0].XYPlotSettings.XAxis.ViewMin = 0.2;
            xyPlotViewCtrl1.XYPlots[0].XYPlotSettings.XAxis.ViewMax = 2000;
            xyPlotViewCtrl1.XYPlots[0].XYPlotSettings.XAxis.IsLog = true;
            xyPlotViewCtrl1.XYPlots[0].XYPlotSettings.XAxis.Name = "Resistivity";
            xyPlotViewCtrl1.XYPlots[0].XYPlotSettings.YAxis.ReversedScale = true;
            // gamma plot
            xyPlotViewCtrl1.XYPlots[1].XYPlotSettings.XAxis.ViewMin = 0;
            xyPlotViewCtrl1.XYPlots[1].XYPlotSettings.XAxis.ViewMax = 200;
            xyPlotViewCtrl1.XYPlots[1].XYPlotSettings.XAxis.IsLog = false;
            xyPlotViewCtrl1.XYPlots[1].XYPlotSettings.XAxis.Name = "Gamma";
            xyPlotViewCtrl1.XYPlots[1].XYPlotSettings.YAxis.ReversedScale = true;

            xyPlotViewCtrl1.FullView();
        }
    }
}
