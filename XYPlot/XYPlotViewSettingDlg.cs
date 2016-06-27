using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NSXYPlot
{
    public partial class XYPlotViewSettingDlg : Form
    {
        private XYPlotViewSetting plotViewSetting;
        private XYPlotViewCtrl plotViewCtrl;

        public XYPlotViewSettingDlg(XYPlotViewSetting setting, XYPlotViewCtrl ctrl)
        {
            InitializeComponent();

            plotViewSetting = setting;
            plotViewCtrl = ctrl;
        }

        private void XYPlotViewSettingDlg_Load(object sender, EventArgs e)
        {
            if (plotViewSetting == null) return;
            if (plotViewSetting.PlotViewStructList == null || plotViewSetting.PlotViewStructList.Count <= 0) return;

            checkBoxIsPortrait.Checked = plotViewSetting.IsPortrait;
            int x1 = 13; int x2 = 135;
            int y = 92;
            int dy = 23;
            for (int i = 0; i < plotViewSetting.PlotViewStructList.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Text = plotViewSetting.PlotViewStructList[i].PlotSetting.PlotName;
                checkBox.Name = "checkBox" + checkBox.Text;
                checkBox.Checked = plotViewSetting.PlotViewStructList[i].ShowPlot;
                checkBox.Location = new Point(x1, y + i * dy + 2);
                this.Controls.Add(checkBox);

                TextBox textBox = new TextBox();
                textBox.Text = plotViewSetting.PlotViewStructList[i].Width.ToString();
                textBox.Name = "textBox" + plotViewSetting.PlotViewStructList[i].PlotSetting.PlotName;
                textBox.Location = new Point(x2, y + i * dy);
                this.Controls.Add(textBox);
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (save() == false) return;

            if (plotViewCtrl != null) plotViewCtrl.Invalidate();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool save()
        {
            if (plotViewSetting.IsPortrait != checkBoxIsPortrait.Checked)
            {
                plotViewSetting.IsPortrait = checkBoxIsPortrait.Checked;
                orientationChanged();
            }

            for (int i = 0; i < plotViewSetting.PlotViewStructList.Count; i++)
            {
                String nameCB = "checkBox" + plotViewSetting.PlotViewStructList[i].PlotSetting.PlotName;
                Control[] controls = this.Controls.Find(nameCB, true);
                CheckBox checkBox = null;
                if (controls != null && controls.Length > 0) checkBox = (CheckBox)controls[0];

                String nameTB = "textBox" + plotViewSetting.PlotViewStructList[i].PlotSetting.PlotName;
                controls = this.Controls.Find(nameTB, true);
                TextBox textBox = null;
                if (controls != null && controls.Length > 0) textBox = (TextBox)controls[0];

                if (checkBox != null && textBox != null)
                {
                    XYPlotViewStruct s = new XYPlotViewStruct(plotViewSetting.PlotViewStructList[i].PlotSetting,
                        plotViewSetting.PlotViewStructList[i].Width, plotViewSetting.PlotViewStructList[i].ShowPlot);
                    s.Width = double.Parse(textBox.Text);
                    s.ShowPlot = checkBox.Checked;
                    plotViewSetting.PlotViewStructList[i] = s;
                }
            }

            return true;
        }

        // user switches between Portrait and Landscape
        private void orientationChanged()
        {
            // switch x, y axis
            foreach (XYPlotViewStruct s in plotViewSetting.PlotViewStructList)
            {
                Axis axis = s.PlotSetting.XAxis;
                s.PlotSetting.XAxis = s.PlotSetting.YAxis;
                s.PlotSetting.YAxis = axis;
            }
        }

        private bool validateInput()
        {
            for (int i = 0; i < plotViewSetting.PlotViewStructList.Count; i++)
            {
                String nameCB = "checkBox" + plotViewSetting.PlotViewStructList[i].PlotSetting.PlotName;
                Control[] controls = this.Controls.Find(nameCB, true);
                CheckBox checkBox = null;
                if (controls != null && controls.Length > 0) checkBox = (CheckBox)controls[0];

                String nameTB = "textBox" + plotViewSetting.PlotViewStructList[i].PlotSetting.PlotName;
                controls = this.Controls.Find(nameTB, true);
                TextBox textBox = null;
                if (controls != null && controls.Length > 0) textBox = (TextBox)controls[0];

                if (checkBox != null && textBox != null)
                {
                    XYPlotViewStruct s = new XYPlotViewStruct(plotViewSetting.PlotViewStructList[i].PlotSetting,
                        plotViewSetting.PlotViewStructList[i].Width, plotViewSetting.PlotViewStructList[i].ShowPlot);
                    s.Width = double.Parse(textBox.Text);
                    s.ShowPlot = checkBox.Checked;
                    plotViewSetting.PlotViewStructList[i] = s;
                }
            }

            return true;
        }
    }

    public struct XYPlotViewStruct
    {
        public XYPlotSetting PlotSetting;
        public double Width;
        public bool ShowPlot;

        public XYPlotViewStruct(XYPlotSetting setting, double width, bool show)
        {
            PlotSetting = setting;
            Width = width;
            ShowPlot = show;
        }
    }
    public class XYPlotViewSetting
    {
        public bool IsPortrait;
        public List<XYPlotViewStruct> PlotViewStructList;

        public double AverageWidth { get { return TotalWidth() / NumOfPlotsShown(); } }

        public XYPlotViewSetting()
        {
            IsPortrait = true;
            PlotViewStructList = new List<XYPlotViewStruct>();
        }

        public double TotalWidth()
        {
            if (PlotViewStructList == null || PlotViewStructList.Count == 0) return 100;

            double totalWidth = 0;
            foreach (XYPlotViewStruct s in PlotViewStructList)
            {
                if (s.ShowPlot) totalWidth += s.Width;
            }

            return totalWidth;
        }

        public int NumOfPlotsShown()
        {
            if (PlotViewStructList == null || PlotViewStructList.Count == 0) return 1;

            int plotsShown = 0;
            foreach (XYPlotViewStruct s in PlotViewStructList)
            {
                if (s.ShowPlot) plotsShown++;
            }

            return plotsShown;
        }
    }
}
