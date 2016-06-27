using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using BWNSDataset;
//using BWNSUtility;

namespace NSXYPlot
{
    public enum PlotType
    {
        XYPlot,
        XYPlotCurve,
        XYPlotTimeSignal,
        XYPlotFrequence,
        XYPlotImage,
        XYPlotTimeDepth,
    }

    public partial class XYPlotSettingDlg : Form
    {
        private XYPlotSetting xyPlotSetting;
        private XYPlot xyPlot;

        public XYPlotSettingDlg(XYPlotSetting plotSetting, XYPlot plot)
        {
            InitializeComponent();

            xyPlotSetting = plotSetting;
            xyPlot = plot;

            //plotType = plot.PlotType;
        }

        public void InitialDialog()
        {
            InitialDialog(xyPlot);
        }

        private void InitialDialog(XYPlot plot)
        {
            XYPlotSetting plotSetting = plot.XYPlotSettings;
            this.Text = plot.Title + " Plot Settings";

            # region General page
            this.checkBoxXIsLog.Checked             = plotSetting.XAxis.IsLog;
            this.textBoxXTicks.Text                 = plotSetting.XAxis.TotalTicks.ToString();
            double temp;
            temp = Utility.FormatFloatingNumber(plotSetting.XAxis.ViewMin, 4);
            this.textBoxMinX.Text                   = temp.ToString();
            temp = Utility.FormatFloatingNumber(plotSetting.XAxis.ViewMax, 4);
            this.textBoxMaxX.Text                   = temp.ToString();
            this.buttonXAxisColor.BackColor         = plotSetting.XAxis.AxisColor;
            this.numericUpDownXAxisWidth.Value      = (decimal)plotSetting.XAxis.AxisWidth;
            this.checkBoxXAxisReversedScale.Checked = plotSetting.XAxis.ReversedScale;
            this.checkBoxXAxisName.Checked          = plotSetting.XAxis.EnableName;
            this.buttonXTickFontColor.BackColor     = plotSetting.XAxis.TickColor;
            this.numericUpDownXTickFontSize.Value   = (decimal)plotSetting.XAxis.TickSize;
            this.checkBoxXTickEnable.Checked        = plotSetting.XAxis.EnableTick;
            this.checkBoxXTickVertically.Checked    = plotSetting.XAxis.IsTickVertical;
            this.textBoxXAxisName.Text              = plotSetting.XAxis.Name;

            this.checkBoxYIsLog.Checked             = plotSetting.YAxis.IsLog;
            this.textBoxYTicks.Text                 = plotSetting.YAxis.TotalTicks.ToString();
            temp = Utility.FormatFloatingNumber(plotSetting.YAxis.ViewMin, 4);
            this.textBoxMinY.Text                   = temp.ToString();
            temp = Utility.FormatFloatingNumber(plotSetting.YAxis.ViewMax, 4);
            this.textBoxMaxY.Text                   = temp.ToString();
            this.buttonYAxisColor.BackColor         = plotSetting.YAxis.AxisColor;
            this.numericUpDownYAxisWidth.Value      = (decimal)plotSetting.YAxis.AxisWidth;
            this.checkBoxYAxisReversedScale.Checked = plotSetting.YAxis.ReversedScale;
            this.checkBoxYAxisName.Checked          = plotSetting.YAxis.EnableName;
            this.buttonYTickFontColor.BackColor     = plotSetting.YAxis.TickColor;
            this.numericUpDownYTickFontSize.Value   = (decimal)plotSetting.YAxis.TickSize;
            this.checkBoxYTickEnable.Checked        = plotSetting.YAxis.EnableTick;
            this.checkBoxYTickVertically.Checked    = plotSetting.YAxis.IsTickVertical;
            this.textBoxYAxisName.Text              = plotSetting.YAxis.Name;

            this.checkBoxMajorGrid.Checked          = plotSetting.ShowMajorGrid;
            this.buttonMajorGridColor.BackColor     = plotSetting.MajorGridColor;
            this.numericUpDownMajorGridWidth.Value  = (decimal)plotSetting.MajorGridWidth;
            this.checkBoxMinorGrid.Checked          = plotSetting.ShowMinorGrid;
            this.buttonMinorGridColor.BackColor     = plotSetting.MinorGridColor;
            this.numericUpDownMinorGridWidth.Value  = (decimal)plotSetting.MinorGridWidth;
            this.checkBoxTitle.Checked              = plotSetting.EnableTitle;
            this.checkBoxLegend.Checked             = plotSetting.EnableLegend;
            this.buttonBackColor.BackColor          = plotSetting.BackColor;

            # endregion

            #region Curve page
            this.listViewAvailableObjects.Items.Clear();
            this.listViewDisplayedObjects.Items.Clear();

            this.checkBoxShowLine.Checked = true;
            this.buttonCurveColor.BackColor = XYPlotSetting.GetDefaultColor(0);
            this.numericUpDownCurveWidth.Value = (decimal)1.0;
            this.textBoxThredshold.Text = plotSetting.DataMissingThredshold.ToString();

            this.comboBoxCurveSymbol.Items.AddRange(XYPlotSymbol.GetSymbolList());
            this.comboBoxCurveSymbol.SelectedIndex = 0;
            this.numericUpDownCurveMarkSize.Value = 4;
            this.buttonCurveMarkColor.BackColor = XYPlotSetting.GetDefaultColor(0);
            # endregion

        }

        public bool SavePlotSetting()
        {
            return SavePlotSetting(xyPlot,false);
        }

        public bool SavePlotSetting(XYPlot plot, bool settingOnly)
        {
            XYPlot xyPlot = plot;
            XYPlotSetting plotSetting = plot.XYPlotSettings;
            double viewMinX = plotSetting.XAxis.ViewMin;
            double viewMaxX = plotSetting.XAxis.ViewMax;
            double viewMinY = plotSetting.YAxis.ViewMin;
            double viewMaxY = plotSetting.YAxis.ViewMax;

            # region General page
            plotSetting.XAxis.IsLog             = this.checkBoxXIsLog.Checked;
            plotSetting.XAxis.TotalTicks        = int.Parse(this.textBoxXTicks.Text);
            plotSetting.XAxis.ViewMin           = double.Parse(this.textBoxMinX.Text);
            plotSetting.XAxis.ViewMax           = double.Parse(this.textBoxMaxX.Text);
            plotSetting.XAxis.AxisColor         = this.buttonXAxisColor.BackColor;
            plotSetting.XAxis.AxisWidth         = (float)this.numericUpDownXAxisWidth.Value;
            plotSetting.XAxis.ReversedScale     = this.checkBoxXAxisReversedScale.Checked;
            plotSetting.XAxis.EnableName        = this.checkBoxXAxisName.Checked;
            plotSetting.XAxis.TickColor         = this.buttonXTickFontColor.BackColor;
            plotSetting.XAxis.TickSize          = (float)this.numericUpDownXTickFontSize.Value;
            plotSetting.XAxis.EnableTick        = this.checkBoxXTickEnable.Checked;
            plotSetting.XAxis.IsTickVertical    = this.checkBoxXTickVertically.Checked;
            plotSetting.XAxis.Name              = this.textBoxXAxisName.Text;

            plotSetting.YAxis.IsLog             = this.checkBoxYIsLog.Checked;
            plotSetting.YAxis.TotalTicks        = int.Parse(this.textBoxYTicks.Text);
            plotSetting.YAxis.ViewMin           = double.Parse(this.textBoxMinY.Text);
            plotSetting.YAxis.ViewMax           = double.Parse(this.textBoxMaxY.Text);
            plotSetting.YAxis.AxisColor         = this.buttonYAxisColor.BackColor;
            plotSetting.YAxis.AxisWidth         = (float)this.numericUpDownYAxisWidth.Value;
            plotSetting.YAxis.ReversedScale     = this.checkBoxYAxisReversedScale.Checked;
            plotSetting.YAxis.EnableName        = this.checkBoxYAxisName.Checked;
            plotSetting.YAxis.TickColor         = this.buttonYTickFontColor.BackColor;
            plotSetting.YAxis.TickSize          = (float)this.numericUpDownYTickFontSize.Value;
            plotSetting.YAxis.EnableTick        = this.checkBoxYTickEnable.Checked;
            plotSetting.YAxis.IsTickVertical    = this.checkBoxYTickVertically.Checked;
            plotSetting.YAxis.Name              = this.textBoxYAxisName.Text;

            plotSetting.ShowMajorGrid           = this.checkBoxMajorGrid.Checked;
            plotSetting.MajorGridColor          = this.buttonMajorGridColor.BackColor;
            plotSetting.MajorGridWidth          = (float)this.numericUpDownMajorGridWidth.Value;
            plotSetting.ShowMinorGrid           = this.checkBoxMinorGrid.Checked;
            plotSetting.MinorGridColor          = this.buttonMinorGridColor.BackColor;
            plotSetting.MinorGridWidth          = (float)this.numericUpDownMinorGridWidth.Value;
            plotSetting.EnableTitle             = false;// this.checkBoxTitle.Checked;
            plotSetting.EnableLegend            = this.checkBoxLegend.Checked;
            plotSetting.BackColor               = this.buttonBackColor.BackColor;

            # endregion

            # region Curve page
            plotSetting.DataMissingThredshold = double.Parse(this.textBoxThredshold.Text);

            for (int i = 0; i < this.listViewDisplayedObjects.Items.Count; i++)
            {
                if (this.listViewDisplayedObjects.Items[i].Selected)
                {
                    String curveName = this.listViewDisplayedObjects.Items[i].Text;
                    DisplayProperty dp = plotSetting.GetDisplaySetting(curveName);
                    dp.Display = true;
                    dp.ShowLine = this.checkBoxShowLine.Checked;
                    dp.Color = this.buttonCurveColor.BackColor;
                    dp.Width = (float)this.numericUpDownCurveWidth.Value;
                    dp.ShowSymbol = this.checkBoxShowPoint.Checked;
                    if (this.comboBoxCurveSymbol.SelectedItem != null)
                    {
                        dp.Symbol = this.comboBoxCurveSymbol.SelectedItem.ToString();
                    }
                    else
                    {
                        dp.Symbol = XYPlotSymbol.GetDefaultSymbol();
                    }
                    dp.SymbolSize = (short)this.numericUpDownCurveMarkSize.Value;
                    dp.SymbolColor = this.buttonCurveMarkColor.BackColor;
                    dp.ShowText = this.checkBoxShowText.Checked;
                    dp.FontColor = this.buttonTextColor.BackColor;
                    dp.FontSize = (float)this.numericUpDownTextSize.Value;
                    dp.ShowLine2 = this.checkBoxShowLine2.Checked;
                    dp.Color2 = this.buttonCurveColor2.BackColor;
                    dp.Width2 = (float)this.numericUpDownCurveWidth2.Value;

                    // overwrite primary scale
                    dp.TempAxisOverWrite = this.checkBoxTempAxisOverWrite.Checked;
                    dp.TempAxisViewMin = double.Parse(this.textBoxTempAxisMinValue.Text);
                    dp.TempAxisViewMax = double.Parse(this.textBoxTempAxisMaxValue.Text);
                    dp.TempAxisIsLog = this.checkBoxTempAxisIsLog.Checked;
                    dp.TempAxisReversedScale = this.checkBoxTempAxisReversedScale.Checked;
                    dp.TempAxisDisplayScale = this.checkBoxDisplayScale.Checked;

                    int index = plotSetting.GetDisplaySettingIndex(curveName);
                    if (index >= 0 && index < xyPlotSetting.DisplayPropertyList.Count)
                    {
                        plotSetting.DisplayPropertyList[index] = dp;
                    }
                    else
                    {
                        plotSetting.DisplayPropertyList.Add(dp);
                    }
                }
                else
                {
                    String curveName = this.listViewDisplayedObjects.Items[i].Text;
                    String type = this.listViewDisplayedObjects.Items[i].SubItems[1].Text;
                    plotSetting.SetDisplaySettingDisplay(curveName, true, type);
                }
            }
            # endregion

            if (viewMinX != plotSetting.XAxis.ViewMin || viewMaxX != plotSetting.XAxis.ViewMax) plot.OnChangeViewX();
            if (viewMinY != plotSetting.YAxis.ViewMin || viewMaxY != plotSetting.YAxis.ViewMax) plot.OnChangeViewY();

            return true;
        }

        private bool InvalidateFormData()
        {
            if (Utility.ValidateIntegerTextControl("# of X Ticks", this.textBoxXTicks.Text,
                TextFormat.PositiveInteger, 1, 100) == false ||
                Utility.ValidateIntegerTextControl("# of Y Ticks", this.textBoxYTicks.Text,
                TextFormat.PositiveInteger, 1, 100) == false ||
                Utility.ValidateFloatingNumberTextControl("Min X", this.textBoxMinX.Text,
                TextFormat.FloatingNumber, -1000000, 1000000) == false ||
                Utility.ValidateFloatingNumberTextControl("Max X", this.textBoxMaxX.Text,
                TextFormat.FloatingNumber, -1000000, 1000000) == false ||
                Utility.ValidateFloatingNumberTextControl("Min Y", this.textBoxMinY.Text,
                TextFormat.FloatingNumber, -1000000, 1000000) == false ||
                Utility.ValidateFloatingNumberTextControl("Max Y", this.textBoxMaxY.Text,
                TextFormat.FloatingNumber, -1000000, 1000000) == false ||
                Utility.ValidateFloatingNumberTextControl("Data missing thredshold", this.textBoxThredshold.Text,
                TextFormat.FloatingNumber, 1, 1000) == false)
            {
                return false;
            }

            if (double.Parse(this.textBoxMinX.Text) >= double.Parse(this.textBoxMaxX.Text))
            {
                MessageBox.Show("Min X should be less than Max X!");
                return false;
            }

            if (double.Parse(this.textBoxMinY.Text) >= double.Parse(this.textBoxMaxY.Text))
            {
                MessageBox.Show("Min Y should be less than Max Y!");
                return false;
            }

            if (this.checkBoxTempAxisOverWrite.Checked == true)
            {
                if (Utility.ValidateFloatingNumberTextControl("Min value", this.textBoxTempAxisMinValue.Text,
                    TextFormat.FloatingNumber, -1000000, 1000000) == false ||
                    Utility.ValidateFloatingNumberTextControl("Max value", this.textBoxTempAxisMaxValue.Text,
                    TextFormat.FloatingNumber, -1000000, 1000000) == false)
                {
                    return false;
                }

                if (double.Parse(this.textBoxTempAxisMinValue.Text) >= double.Parse(this.textBoxTempAxisMaxValue.Text))
                {
                    MessageBox.Show("Min value should be less than Max value!");
                    return false;
                }

                if (this.checkBoxTempAxisIsLog.Checked == true &&
                    double.Parse(this.textBoxTempAxisMinValue.Text) <= 0)
                {
                    MessageBox.Show("Only positive values are allowed on a logarithmic track!");
                    return false;
                }
            }

            return true;
        }

        // events
        private void XYPlotSettingDlg_Load(object sender, EventArgs e)
        {
            refreshListView(xyPlot);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            buttonApply_Click(sender, e);

            this.DialogResult = DialogResult.OK;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            //using (new WaitCursor())
            {
                if (SavePlotSetting() == false) return;
                xyPlot.ParentControl.Invalidate(true);
            }
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            Button colorButton = (Button)sender;
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                colorButton.BackColor = dlg.Color;
            }
        }

        // Curve page
        #region checkedListBox
        //private void checkedListBoxCurves_ItemCheck(object sender, ItemCheckEventArgs e)
        //{
        //    Color curveColor = XYPlotSetting.GetDefaultColor(0);
        //    float curveWidth = 1.0F;
        //    String symbol = Symbol.GetDefaultSymbol();
        //    int symbolSize = 4;
        //    Color symbolColor = XYPlotSetting.GetDefaultColor(0);
        //    bool useAxis2 = false;

        //    bool bFirstCheckedItem = true;
        //    for (int i = 0; i < checkedListBoxCurves.Items.Count; i++)
        //    {
        //        if ((i == e.Index && checkedListBoxCurves.GetItemChecked(i) == false) ||
        //            (i != e.Index && checkedListBoxCurves.GetItemChecked(i) == true))
        //        {
        //            String curveName = checkedListBoxCurves.Items[i].ToString();
        //            DisplayProperty dp = getCurveDisplaySetting(curveName);
                    
        //            if (bFirstCheckedItem == true)
        //            {
        //                curveColor = dp.Color;
        //                curveWidth = dp.Width;
        //                symbol = dp.Symbol;
        //                symbolSize = dp.SymbolSize;
        //                symbolColor = dp.SymbolColor;
        //                useAxis2 = dp.UseAxis2;
        //                bFirstCheckedItem = false;
        //            }
        //            else
        //            {
        //                if (dp.Color != curveColor)
        //                {
        //                    curveColor = XYPlotSetting.GetDefaultColor(0);
        //                }
        //                if (dp.Width != curveWidth)
        //                {
        //                    curveWidth = 1;
        //                }
        //                if (dp.Symbol != symbol)
        //                {
        //                    symbol = Symbol.GetDefaultSymbol();
        //                }
        //                if (dp.SymbolSize != symbolSize)
        //                {
        //                    symbolSize = 4;
        //                }
        //                if (dp.SymbolColor != symbolColor)
        //                {
        //                    symbolColor = XYPlotSetting.GetDefaultColor(0);
        //                }
        //                if (dp.UseAxis2 != useAxis2)
        //                {
        //                    useAxis2 = false;
        //                }
        //            }
        //        }

        //        this.buttonCurveColor.BackColor = curveColor;
        //        this.numericUpDownCurveWidth.Value = (decimal)curveWidth;
        //        this.comboBoxCurveSymbol.SelectedIndex = this.comboBoxCurveSymbol.FindStringExact(symbol);
        //        this.numericUpDownCurveMarkSize.Value = symbolSize;
        //        this.buttonCurveMarkColor.BackColor = symbolColor;
        //        this.checkBoxUseAxis2.Checked = useAxis2;
        //    }
        //}
        # endregion

        private void listViewDisplayedObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool showLine = true;
            Color curveColor = XYPlotSetting.GetDefaultColor(0);
            float curveWidth = 1.0F;
            bool showSymbol = true;
            String symbol = XYPlotSymbol.GetDefaultSymbol();
            int symbolSize = 4;
            Color symbolColor = XYPlotSetting.GetDefaultColor(0);
            bool showText = false;
            Color textColor = Color.LightGreen;
            float textSize = 8;
            bool useAxis2 = false;
            bool showHoleSize = false;
            bool showLine2 = true;
            Color curveColor2 = XYPlotSetting.GetDefaultColor(1);
            float curveWidth2 = 1.0F;
            bool overwrite = false;
            double viewMin = 0.0;
            double viewMax = 1.0;
            bool isLog = false;
            bool reversedScale = false;
            bool displayScale = false;
            
            bool bFirstSelectedItem = true;
            foreach (int index in listViewDisplayedObjects.SelectedIndices)
            {
                String curveName = listViewDisplayedObjects.Items[index].Text;
                DisplayProperty dp = xyPlotSetting.GetDisplaySetting(curveName);

                if (bFirstSelectedItem == true)
                {
                    showLine = dp.ShowLine;
                    curveColor = dp.Color;
                    curveWidth = dp.Width;
                    showSymbol = dp.ShowSymbol;
                    symbol = dp.Symbol;
                    symbolSize = dp.SymbolSize;
                    symbolColor = dp.SymbolColor;
                    showText = dp.ShowText;
                    textColor = dp.FontColor;
                    textSize = dp.FontSize;
                    useAxis2 = dp.UseAxis2;
                    showHoleSize = dp.ShowHoleSize;
                    showLine2 = dp.ShowLine2;
                    curveColor2 = dp.Color2;
                    curveWidth2 = dp.Width2;
                    overwrite = dp.TempAxisOverWrite;
                    viewMin = dp.TempAxisViewMin;
                    viewMax = dp.TempAxisViewMax;
                    isLog = dp.TempAxisIsLog;
                    reversedScale = dp.TempAxisReversedScale;
                    displayScale = dp.TempAxisDisplayScale;

                    bFirstSelectedItem = false;

                    panelNormalCurves.Visible = (dp.CurveType != BWDataType.typeDataImageDirRes.ToString() &&
                                                 dp.CurveType != BWDataType.typeDataImageNBGamma.ToString());    // (dp.CurveType == BWDataType.typeDataCurve);
                    panelImageCurve.Visible = (dp.CurveType == BWDataType.typeDataImageDirRes.ToString() ||
                                               dp.CurveType == BWDataType.typeDataImageNBGamma.ToString());
                }
                else
                {
                    if (dp.ShowLine != showLine)
                    {
                        showLine = false;
                    }
                    if (dp.Color != curveColor)
                    {
                        curveColor = XYPlotSetting.GetDefaultColor(0);
                    }
                    if (dp.Width != curveWidth)
                    {
                        curveWidth = 1;
                    }
                    if (dp.ShowSymbol != showSymbol)
                    {
                        showSymbol = false;
                    }
                    if (dp.Symbol != symbol)
                    {
                        symbol = XYPlotSymbol.GetDefaultSymbol();
                    }
                    if (dp.SymbolSize != symbolSize)
                    {
                        symbolSize = 4;
                    }
                    if (dp.SymbolColor != symbolColor)
                    {
                        symbolColor = XYPlotSetting.GetDefaultColor(0);
                    }
                    if (dp.ShowText != showText)
                    {
                        showText = false;
                    }
                    if (dp.FontColor != textColor)
                    {
                        textColor = Color.LightGreen;
                    }
                    if (dp.FontSize != textSize)
                    {
                        textSize = 8;
                    }
                    if (dp.UseAxis2 != useAxis2)
                    {
                        useAxis2 = false;
                    }
                    if (dp.ShowHoleSize != showHoleSize)
                    {
                        showHoleSize = false;
                    }
                    if (dp.ShowLine2 != showLine2)
                    {
                        showLine2 = false;
                    }
                    if (dp.Color2 != curveColor2)
                    {
                        curveColor2 = XYPlotSetting.GetDefaultColor(1);
                    }
                    if (dp.Width2 != curveWidth2)
                    {
                        curveWidth = 1;
                    }
                    if (dp.TempAxisOverWrite != overwrite)
                    {
                        overwrite = false;
                    }
                    if (dp.TempAxisViewMin != viewMin)
                    {
                        viewMin = 0.0;
                    }
                    if (dp.TempAxisViewMax != viewMax)
                    {
                        viewMax = 1.0;
                    }
                    if (dp.TempAxisIsLog != isLog)
                    {
                        isLog = false;
                    }
                    if (dp.TempAxisReversedScale != reversedScale)
                    {
                        reversedScale = false;
                    }
                    if (dp.TempAxisDisplayScale != displayScale)
                    {
                        displayScale = false;
                    }
                }

                this.checkBoxShowLine.Checked = showLine;
                this.buttonCurveColor.BackColor = curveColor;
                this.numericUpDownCurveWidth.Value = (decimal)curveWidth;
                this.checkBoxShowPoint.Checked = showSymbol;
                this.comboBoxCurveSymbol.SelectedIndex = this.comboBoxCurveSymbol.FindStringExact(symbol);
                this.numericUpDownCurveMarkSize.Value = symbolSize;
                this.buttonCurveMarkColor.BackColor = symbolColor;
                this.checkBoxShowText.Checked = showText;
                this.buttonTextColor.BackColor = textColor;
                this.numericUpDownTextSize.Value = (decimal)textSize;
                this.checkBoxShowLine2.Checked = showLine2;
                this.buttonCurveColor2.BackColor = curveColor2;
                this.numericUpDownCurveWidth2.Value = (decimal)curveWidth2;
                this.checkBoxTempAxisOverWrite.Checked = overwrite;
                this.textBoxTempAxisMinValue.Text = viewMin.ToString();
                this.textBoxTempAxisMaxValue.Text = viewMax.ToString();
                this.checkBoxTempAxisIsLog.Checked = isLog;
                this.checkBoxTempAxisReversedScale.Checked = reversedScale;
                this.checkBoxDisplayScale.Checked = displayScale;
            }

            EnableDisableControls();
        }

        private void EnableDisableControls()
        {
        }

        private void checkBoxXIsLog_Click(object sender, System.EventArgs e)
        {
            CheckBox checkBoxIsLog = (CheckBox)sender;
            double viewMin = double.Parse(this.textBoxMinX.Text);
            if (checkBoxXIsLog.Checked && viewMin < 1.0e-6)
            {
                viewMin = 0.000001;
                this.textBoxMinX.Text = viewMin.ToString();
            }
            if (!checkBoxXIsLog.Checked && viewMin == 1.0e-6)
            {
                viewMin = 0.0;
                this.textBoxMinX.Text = viewMin.ToString();
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            int totalRemovedItems = 0;
            foreach (int index in this.listViewAvailableObjects.SelectedIndices)
            {
                int i = index - totalRemovedItems;
                ListViewItem item = listViewAvailableObjects.Items[i];
                String type = item.SubItems[1].Text;
                listViewAvailableObjects.Items.RemoveAt(i);
                listViewDisplayedObjects.Items.Add(item);
                totalRemovedItems++;

                xyPlotSetting.SetDisplaySettingDisplay(item.Text, true, type);
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            int totalRemovedItems = 0;
            //foreach (int index in checkedListBoxCurves.CheckedIndices)
            foreach (int index in this.listViewDisplayedObjects.SelectedIndices)
            {
                int i = index - totalRemovedItems;
                ListViewItem item = listViewDisplayedObjects.Items[i];

                listViewDisplayedObjects.Items.RemoveAt(i);
                listViewAvailableObjects.Items.Add(item);
                totalRemovedItems++;

                xyPlotSetting.SetDisplaySettingDisplay(item.Text, false, item.SubItems[1].Text);
            }
        }

        private void buttonAddAll_Click(object sender, EventArgs e)
        {
            //foreach (String curveName in checkedListBoxAllCurves.Items)
            foreach (ListViewItem item in listViewAvailableObjects.Items)
            {
                String curveName = item.Text;
                listViewDisplayedObjects.Items.Add((ListViewItem)item.Clone());
                String type = item.SubItems[1].Text;
                xyPlotSetting.SetDisplaySettingDisplay(item.Text, true, type);
            }
            //checkedListBoxAllCurves.Items.Clear();
            listViewAvailableObjects.Items.Clear();
        }

        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            //foreach (String curveName in checkedListBoxCurves.Items)
            foreach (ListViewItem item in listViewDisplayedObjects.Items)
            {
                String curveName = item.Text;
                //checkedListBoxAllCurves.Items.Add(curveName);
                listViewAvailableObjects.Items.Add((ListViewItem)item.Clone());

                String type = item.SubItems[1].Text;
                xyPlotSetting.SetDisplaySettingDisplay(item.Text, false, type);
            }
            //checkedListBoxCurves.Items.Clear();
            listViewDisplayedObjects.Items.Clear();
            //xyPlotSetting.DisplayPropertyList.Clear();
        }

        private void checkBoxTempAxisOverWrite_CheckedChanged(object sender, EventArgs e)
        {
            bool overwrite = checkBoxTempAxisOverWrite.Checked;
            textBoxTempAxisMinValue.Enabled = overwrite;
            textBoxTempAxisMaxValue.Enabled = overwrite;
            checkBoxTempAxisIsLog.Enabled = overwrite;
            checkBoxTempAxisReversedScale.Enabled = overwrite;
            checkBoxDisplayScale.Enabled = overwrite;
        }

        private void checkBoxShowAll_CheckedChanged(object sender, EventArgs e)
        {
            refreshAvailableListView();
        }

        private void refreshListView(XYPlot plot)
        {
            refreshDisplayedListView(plot);
            refreshAvailableListView();
        }

        private void refreshAvailableListView()
        {
            if (xyPlot == null || xyPlotSetting == null) return;

            this.listViewAvailableObjects.Items.Clear();
            if (xyPlot.DatasetList != null)
            {
                for (int i = 0; i < xyPlot.DatasetList.Count; i++)
                {
                    if (xyPlot.DatasetList[i] == null) continue;

                    String curveName = xyPlot.DatasetList[i].Name;
                    bool display = xyPlotSetting.GetDisplaySetting(curveName).Display;
                    if (display) continue;

                    ListViewItem item = new ListViewItem(curveName);
                    String type = BWDatasetUtility.GetDataTypeName(xyPlot.DatasetList[i].DataType);
                    item.SubItems.Add(type);
                    this.listViewAvailableObjects.Items.Add(item);
                }
            }
        }

        private void refreshDisplayedListView(XYPlot plot)
        {
            if (xyPlot == null || xyPlotSetting == null) return;

            this.listViewDisplayedObjects.Items.Clear();
            if (plot.DatasetList != null)
            {
                for (int i = 0; i < plot.DatasetList.Count; i++)
                {
                    if (xyPlot.DatasetList[i] == null) continue;

                    String curveName = xyPlot.DatasetList[i].Name;
                    bool display = xyPlotSetting.GetDisplaySetting(curveName).Display;
                    if (display == false) continue;

                    ListViewItem item = new ListViewItem(curveName);
                    String type = BWDatasetUtility.GetDataTypeName(xyPlot.DatasetList[i].DataType);
                    item.SubItems.Add(type);
                    this.listViewDisplayedObjects.Items.Add(item);
                }
            }
        }

        private void listViewDisplayedObjects_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
        if (e.IsSelected && listViewDisplayedObjects.SelectedIndices.Count > 1)
            {
                //compare current selected item with a previously selected item
                foreach (ListViewItem item in listViewDisplayedObjects.SelectedItems)
                {
                    if (item == e.Item) continue;
                    DisplayProperty dp1 = xyPlotSetting.GetDisplaySetting(item.Text);
                    DisplayProperty dp2 = xyPlotSetting.GetDisplaySetting(e.Item.Text);
                    if (dp1.CurveType != dp2.CurveType)
                    {
                        ((ListView)sender).Items[e.ItemIndex].Selected = false;
                    }
                    break;
                }
            }
        }

        private void checkBoxReverseColor_CheckedChanged(object sender, EventArgs e)
        {
            ColorMap colorMap = new ColorMap(this.checkBoxReverseColor.Checked);
            this.buttonMinColor.BackColor = colorMap.MinColor;
            this.buttonMaxColor.BackColor = colorMap.MaxColor;
        } 
    }

    public static class XYPlotSymbol
    {
        public const int NumberOfSymbols = 6;
        public static String[] GetSymbolList()
        {
            return new String[] { "PlusSign", "Cross", "Circle", "Square", "Diamond", "Triangle" };
        }
        public static String GetDefaultSymbol() { return "PlusSign"; }
    }

    public static class LineStyle
    {
        public const int NumberOfLineStyles = 5;
        public static String[] GetLineStyleList()
        {
            return new String[] { "Dash", "DashDot", "DashDotDot", "Dot", "Solid" };
        }
        public static String GetDefaultLineStyle() { return "Solid"; }
        public static DashStyle GetLineStyle(String style)
        {
            if (style == String.Empty) return DashStyle.Solid;

            switch (style)
            {
                case "Dash": return DashStyle.Dash;
                case "DashDot": return DashStyle.DashDot;
                case "DashDotDot": return DashStyle.DashDotDot;
                case "Dot": return DashStyle.Dot;
                case "Solid": return DashStyle.Solid;
                default: return DashStyle.Solid;
            }
        }
    }

    [Serializable]
    public class Axis : ICloneable
    {
        // major grid settings
        private bool isLog;
        public bool IsLog
        {
            get { return isLog; }
            set
            {
                isLog = value;
                if (isLog == true)
                {
                    ViewMin = Math.Max(ViewMin, 1.0e-4);
                    ViewMax = Math.Max(ViewMax, 1.0e-3);
                }
            }
        }
        public int TotalTicks;

        // tick font and color
        //[NonSerialized]
        public Font TickFont
        {
            get { return new Font(TickName, TickSize); }
            set { TickName = value.FontFamily.Name; TickSize = value.Size; }
        }
        public String TickName;
        public Color TickColor;
        public float TickSize;
        public bool EnableTick;
        public bool IsTickVertical;

        // axis pen property
        public Color AxisColor;
        public float AxisWidth;
        public bool ReversedScale;

        // view property
        public double ViewMin;
        public double ViewMax;

        public Boolean Enabled;
        public Boolean SameAsPrimary;
        public String Name;
        public bool EnableName;

        public Axis()
        {
            commonConstruct();
        }

        public Axis(String name)
        {
            commonConstruct();
            Name = name;
        }

        public Object Clone()
        {
            Axis axis = new Axis();
            axis.isLog = this.isLog;
            axis.TotalTicks = this.TotalTicks;
            axis.TickFont = this.TickFont;
            axis.TickName = this.TickName;
            axis.TickColor = this.TickColor;
            axis.TickSize = this.TickSize;
            axis.EnableTick = this.EnableTick;
            axis.IsTickVertical = this.IsTickVertical;

            // axis pen property
            axis.AxisColor = this.AxisColor;
            axis.AxisWidth = this.AxisWidth;
            axis.ReversedScale = this.ReversedScale;

            // view property
            axis.ViewMin = this.ViewMin;
            axis.ViewMax = this.ViewMax;

            axis.Enabled = this.Enabled;
            axis.SameAsPrimary = this.SameAsPrimary;
            axis.Name = this.Name;
            axis.EnableName = this.EnableName;

            return ((Object)axis);
        }

        private void commonConstruct()
        {
            isLog = false;
            TotalTicks = 5;

            TickSize = 8;
            TickName = "Arial";
            TickFont = new Font(TickName, TickSize);
            TickColor = Color.LightCoral;
            EnableTick = true;
            IsTickVertical = false;

            AxisColor = Color.Blue;
            AxisWidth = 1.5F;
            ReversedScale = false;

            ViewMin = 0.0;
            ViewMax = 100;

            Enabled = true;
            SameAsPrimary = true;
            Name = "";
            EnableName = true;
        }
    }

    public enum LineMode
    {
        lineDirect,
        lineStepUp,
        lineStepDown,
    }

    [Serializable]
    public struct DisplayProperty
    {
        public String Name;
        public Boolean Display;
        public Boolean ShowLine;
        public Color Color;
        public float Width;
        public Boolean ShowSymbol;
        public String Symbol;
        public short SymbolSize;
        public Color SymbolColor;
        public Boolean ShowText;
        public String FontName;
        public Color FontColor;
        public float FontSize;
        public Font TextFont
        {
            get
            {
                if (FontName == null || FontSize == 0)
                {
                    return new Font("Arial", 8);
                }
                else
                {
                    return new Font(FontName, FontSize);
                }
            }
            set { FontName = value.FontFamily.Name; FontSize = value.Size; }
        }
        public Boolean UseAxis2;
        // display well path thickness in actual wellbore hole size
        public Boolean ShowHoleSize;
        // used on Rvd plots
        public Boolean ShowLine2;
        public Color Color2;
        public float Width2;

        // display in scale different from primary axis
        public Boolean TempAxisOverWrite;
        public double TempAxisViewMin;
        public double TempAxisViewMax;
        public Boolean TempAxisIsLog;
        public Boolean TempAxisReversedScale;
        public Boolean TempAxisDisplayScale;

        //[OptionalField(VersionAdded = 1)]
        public String CurveType;
        //[OptionalField(VersionAdded = 1)]
        public LineMode lineMode;

        public DisplayProperty(String name)
        {
            Name = name;
            Display = true;
            ShowLine = true;
            Color = XYPlotSetting.GetDefaultColor(0);
            Width = 1.0F;
            lineMode = LineMode.lineDirect;

            ShowSymbol = false;
            Symbol = XYPlotSymbol.GetDefaultSymbol();
            SymbolSize = 4;
            SymbolColor = Color;
            ShowText = false;
            FontName = "Arial";
            FontSize = 8;
            FontColor = Color.LightCoral;
            UseAxis2 = false;
            ShowLine2 = true;
            Color2 = XYPlotSetting.GetDefaultColor(1);
            Width2 = 1.0F;

            ShowHoleSize = false;

            TempAxisOverWrite = false;
            TempAxisViewMin = 0.0;
            TempAxisViewMax = 1.0;
            TempAxisIsLog = false;
            TempAxisReversedScale = false;
            TempAxisDisplayScale = false;

            CurveType = "Curve";

            TextFont = new Font(FontName, FontSize);
        }

        public DisplayProperty(DisplayProperty dp)
        {
            Name = dp.Name;
            Display = dp.Display;
            ShowLine = dp.ShowLine;
            Color = dp.Color;
            Width = dp.Width;
            ShowSymbol = dp.ShowSymbol;
            Symbol = dp.Symbol;
            SymbolSize = dp.SymbolSize;
            SymbolColor = dp.SymbolColor;
            ShowText = dp.ShowText;
            FontName = dp.FontName;
            FontSize = dp.FontSize;
            FontColor = dp.FontColor;
            UseAxis2 = dp.UseAxis2;
            lineMode = dp.lineMode;
            ShowLine2 = dp.ShowLine2;
            Color2 = dp.Color2;
            Width2 = dp.Width2;

            ShowHoleSize = dp.ShowHoleSize;

            TempAxisOverWrite = dp.TempAxisOverWrite;
            TempAxisViewMin = dp.TempAxisViewMin;
            TempAxisViewMax = dp.TempAxisViewMax;
            TempAxisIsLog = dp.TempAxisIsLog;
            TempAxisReversedScale = dp.TempAxisReversedScale;
            TempAxisDisplayScale = dp.TempAxisDisplayScale;

            CurveType = dp.CurveType;

            TextFont = new Font(FontName, FontSize);
        }
    }

    [Serializable]
    public class XYPlotSetting : ICloneable
    {
        public String PlotName;

        public bool EnableTitle;
        public bool EnableLegend;
        public bool Landscape;
        public Color BackColor;

        public Axis XAxis;
        public Axis YAxis;

        public Axis XAxis2;
        public Axis YAxis2;

        // major and minor grids pen property
        public Boolean ShowMajorGrid;
        public Color MajorGridColor;
        public float MajorGridWidth;

        public Boolean ShowMinorGrid;
        public Color MinorGridColor;
        public float MinorGridWidth;

        public int TotalCurves;
        public List<DisplayProperty> DisplayPropertyList;
        private double dataMissingThredshold;
        public double DataMissingThredshold
        {
            get
            {
                if (dataMissingThredshold <= 0) dataMissingThredshold = 10;
                return dataMissingThredshold;
            }
            set
            {
                dataMissingThredshold = value;
            }
        }

        public bool BedOnTopOfGrid;
        public int Transparent;

        // image page
        public Color MinColor;
        public Color MaxColor;
        public float MinValue;
        public float MaxValue;
        public bool ReverseColor;
        public Boolean ShowColorBar;
        public Boolean ShowAvgCurve;
        public InterpolationMode InterpMode;

        //[OptionalField(VersionAdded = 1)]
        public bool showLineSmiley, showPointSmiley, showGMP;

        // members added at implementing plot template
        //[OptionalField(VersionAdded = 1)]
        public String templateName = "";

        public XYPlotSetting()
        {
            commonConstruct();
        }

        public XYPlotSetting(String name)
        {
            commonConstruct();
            PlotName = name;
        }

        public Object Clone()
        {
            XYPlotSetting setting = new XYPlotSetting();
            setting.PlotName = this.PlotName;
            setting.EnableTitle = this.EnableTitle;
            setting.EnableLegend = this.EnableLegend;
            setting.BackColor = this.BackColor;

            setting.XAxis = (Axis)this.XAxis.Clone();
            setting.YAxis = (Axis)this.YAxis.Clone();
            setting.XAxis2 = (Axis)this.XAxis2.Clone();
            setting.YAxis2 = (Axis)this.YAxis2.Clone();

            setting.ShowMajorGrid = this.ShowMajorGrid;
            setting.MajorGridColor = this.MajorGridColor;
            setting.MajorGridWidth = this.MajorGridWidth;

            setting.ShowMinorGrid = this.ShowMinorGrid;
            setting.MinorGridColor = this.MinorGridColor;
            setting.MinorGridWidth = this.MinorGridWidth;

            setting.TotalCurves = this.TotalCurves;
            foreach (DisplayProperty prop in this.DisplayPropertyList)
            {
                DisplayProperty newProp = new DisplayProperty(prop);
                setting.DisplayPropertyList.Add(newProp);
            }
            setting.DataMissingThredshold = this.DataMissingThredshold;

            //setting.SurveyColors = (SurveyColors)this.SurveyColors.Clone();
            setting.BedOnTopOfGrid = this.BedOnTopOfGrid;
            setting.Transparent = this.Transparent;

            // image page
            setting.MinColor = this.MinColor;
            setting.MaxColor = this.MaxColor;
            setting.MinValue = this.MinValue;
            setting.MaxValue = this.MaxValue;
            setting.ReverseColor = this.ReverseColor;
            setting.ShowColorBar = this.ShowColorBar;
            setting.ShowAvgCurve = this.ShowAvgCurve;
            setting.InterpMode = this.InterpMode;
            setting.showGMP = this.showGMP;
            setting.showLineSmiley = this.showLineSmiley;
            setting.showPointSmiley = this.showPointSmiley;

            return ((Object)setting);
        }

        private void commonConstruct()
        {
            PlotName = "";
            EnableTitle = false;
            EnableLegend = true;
            Landscape = true;
            BackColor = Color.FromArgb(240, 254, 255);  // Color.White;    

            XAxis = new Axis("X");
            YAxis = new Axis("Y");
            XAxis2 = new Axis("X2");
            YAxis2 = new Axis("Y2");

            XAxis2.Enabled = false;
            YAxis2.Enabled = false;

            ShowMajorGrid = true;
            MajorGridColor = Color.SkyBlue;
            MajorGridWidth = 1;

            ShowMinorGrid = true;
            MinorGridColor = Color.FromArgb(230, 230, 230);
            MinorGridWidth = 1;

            TotalCurves = 0;
            DisplayPropertyList = new List<DisplayProperty>();
            //LithologyDisplayPropertyList = new List<BWLithologyDisplayProperty>();
            DataMissingThredshold = 10;

            MinColor = Color.Yellow;
            MaxColor = Color.Pink;
            MinValue = 0;
            MaxValue = 1;
            ReverseColor = false;
            ShowColorBar = false;
            ShowAvgCurve = true;
            InterpMode = InterpolationMode.Default;
            showLineSmiley = showPointSmiley = showGMP = true;

            //SurveyColors.Init();
            BedOnTopOfGrid = true;
            Transparent = 255;
        }

        //public void Load(XmlTextReader tr)
        //{
        //    while (tr.Read())
        //    {
        //        if (tr.NodeType == XmlNodeType.Element)
        //        {
        //            if (tr.Name == "BWLWDDAXYPlotSettings")
        //            {
        //                // view min max
        //                tr.Read();
        //                XAxis.ViewMin = double.Parse(tr.ReadString());
        //                tr.Read();
        //                XAxis.ViewMax = double.Parse(tr.ReadString());
        //                tr.Read();
        //                YAxis.ViewMin = double.Parse(tr.ReadString());
        //                tr.Read();
        //                YAxis.ViewMax = double.Parse(tr.ReadString());

        //                // curve display properties
        //                tr.Read();
        //                TotalCurves = int.Parse(tr.ReadString());
        //                for (int i = 0; i < TotalCurves; i++)
        //                {
        //                    DisplayProperty dp = new DisplayProperty();
        //                    tr.Read();
        //                    dp.Symbol = tr.ReadString();
        //                    tr.Read();
        //                    dp.SymbolColor = Color.FromName(tr.ReadString());
        //                    tr.Read();
        //                    dp.SymbolSize = short.Parse(tr.ReadString());
        //                    tr.Read();
        //                    dp.Color = Color.FromName(tr.ReadString());
        //                    tr.Read();
        //                    dp.Width = float.Parse(tr.ReadString());
        //                    DisplayPropertyList[i] = dp;
        //                }
        //            }
        //        }
        //    }
        //}

        //public void SaveOld(XmlTextWriter tw)
        //{
        //    if (tw == null) return;

        //    // plot setting node
        //    tw.WriteStartElement("BWLWDDAXYPlotSettings");
        //    // view min max
        //    if (XAxis != null && YAxis != null)
        //    {
        //        tw.WriteElementString("MinX", XAxis.ViewMin.ToString());
        //        tw.WriteElementString("MaxX", XAxis.ViewMax.ToString());
        //        tw.WriteElementString("MinY", YAxis.ViewMin.ToString());
        //        tw.WriteElementString("MaxY", YAxis.ViewMax.ToString());
        //    }
        //    else
        //    {
        //        tw.WriteElementString("MinX", "-1");
        //        tw.WriteElementString("MaxX", "-1");
        //        tw.WriteElementString("MinY", "-1");
        //        tw.WriteElementString("MaxY", "-1");
        //    }

        //    // curve display properties
        //    tw.WriteElementString("TotalObjects", TotalCurves.ToString());
        //    for (int i = 0; i < TotalCurves; i++)
        //    {
        //        DisplayProperty dp = DisplayPropertyList[i];
        //        tw.WriteElementString("Symbol", dp.Symbol);
        //        tw.WriteElementString("SymbolColor", dp.SymbolColor.Name);
        //        tw.WriteElementString("SymbolSize", dp.SymbolSize.ToString());
        //        tw.WriteElementString("LineColor", dp.Color.Name);
        //        tw.WriteElementString("LineWidth", dp.Width.ToString());
        //    }

        //    tw.WriteEndElement();
        //}

        //public void Save(XmlTextWriter tw)
        //{
        //    if (tw == null) return;

        //    String fileName = "";
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(fileName);
        //    XmlNode oldNode = doc.SelectSingleNode("BWLWDDAXYPlotSettings");
        //    XmlElement newNode = doc.CreateElement("BWLWDDAXYPlotSettings");

        //    // view min max
        //    XmlElement eleMinX = doc.CreateElement("MinX");
        //    XmlElement eleMaxX = doc.CreateElement("MaxX");
        //    XmlElement eleMinY = doc.CreateElement("MinY");
        //    XmlElement eleMaxY = doc.CreateElement("MaxY");
        //    if (XAxis != null && YAxis != null)
        //    {
        //        eleMinX.InnerText = XAxis.ViewMin.ToString();
        //        eleMaxX.InnerText = XAxis.ViewMax.ToString();
        //        eleMinY.InnerText = YAxis.ViewMin.ToString();
        //        eleMaxY.InnerText = YAxis.ViewMax.ToString();
        //    }
        //    else
        //    {
        //        eleMinX.InnerText = "-1";
        //        eleMaxX.InnerText = "-1";
        //        eleMinY.InnerText = "-1";
        //        eleMaxY.InnerText = "-1";
        //    }
        //    newNode.AppendChild(eleMinX);
        //    newNode.AppendChild(eleMaxX);
        //    newNode.AppendChild(eleMinY);
        //    newNode.AppendChild(eleMaxY);

        //    // curve display properties
        //    XmlElement eleTemp = doc.CreateElement("TotalObjects");
        //    eleTemp.InnerText = TotalCurves.ToString();
        //    for (int i = 0; i < TotalCurves; i++)
        //    {
        //        DisplayProperty dp = DisplayPropertyList[i];
        //        tw.WriteElementString("Symbol", dp.Symbol);
        //        tw.WriteElementString("SymbolColor", dp.SymbolColor.Name);
        //        tw.WriteElementString("SymbolSize", dp.SymbolSize.ToString());
        //        tw.WriteElementString("LineColor", dp.Color.Name);
        //        tw.WriteElementString("LineWidth", dp.Width.ToString());
        //    }

        //    tw.WriteEndElement();
        //}

        //public void SetDefaultCurveProperty(int totalCurves)
        //{
        //    TotalCurves = totalCurves;
        //    displayPropertyList = new DisplayProperty[TotalCurves];
        //    for (int i = 0; i < TotalCurves; i++)
        //    {
        //        displayPropertyList[i] = new DisplayProperty("Curve" + i.ToString());
        //    }
        //}

        //public void SetCurveProperty(List<BWDataset> datasetList)
        //{
        //    if (datasetList == null)
        //    {
        //        return;
        //    }

        //    TotalCurves = datasetList.Count;
        //    List<DisplayProperty> newCDS = new List<DisplayProperty>(TotalCurves);

        //    for (int i = 0; i < TotalCurves; i++)
        //    {
        //        // try to find old settings
        //        int indexFound = GetCurveDisplaySettingIndex(datasetList[i].Name);

        //        if (indexFound == -1)
        //        {   // not found
        //            DisplayProperty dp = new DisplayProperty(datasetList[i].Name);
        //            dp.Color = XYPlotSetting.GetDefaultColor(i);
        //            dp.SymbolColor = newCDS[i].Color;
        //            newCDS[i] = dp;

        //            //newCDS[i] = new DisplayProperty(datasetList[i].Name);
        //            //newCDS[i].Color = XYPlotSetting.GetDefaultColor(i);
        //            //newCDS[i].SymbolColor = newCDS[i].Color;
        //        }
        //        else
        //        {   // found
        //            newCDS[i] = displayPropertyList[indexFound];
        //        }
        //    }

        //    displayPropertyList = newCDS;
        //}

        public bool IsDisplaySettingInList(String curveName)
        {
            foreach (DisplayProperty item in DisplayPropertyList)
            {
                if (item.Name.ToUpper() == curveName.ToUpper())
                {
                    return true;
                }
            }

            return false;
        }

        public DisplayProperty GetDisplaySetting(String curveName)
        {
            if (DisplayPropertyList == null) return new DisplayProperty();

            foreach (DisplayProperty item in DisplayPropertyList)
            {
                if (item.Name.ToUpper() == curveName.ToUpper())
                {
                    return item;
                }
            }

            return new DisplayProperty(curveName);
        }

        public int GetDisplaySettingIndex(String curveName)
        {
            for (int i = 0; i < DisplayPropertyList.Count; i++)
            {
                if (DisplayPropertyList[i].Name.ToUpper() == curveName.ToUpper())
                {
                    return i;
                }
            }

            return -1;
        }

        public void SetDisplaySettingDisplay(String curveName, bool display, String type)
        {
            int index = GetDisplaySettingIndex(curveName);
            if (index >= 0 && index < DisplayPropertyList.Count)
            {
                DisplayProperty dp = DisplayPropertyList[index];
                dp.Display = display;
                DisplayPropertyList[index] = dp;
            }
            else
            {
                DisplayProperty dp = new DisplayProperty(curveName);
                dp.Display = display;
                dp.CurveType = type;
                DisplayPropertyList.Add(dp);
            }
        }

        public void SetDisplayColorWidth(String curveName, Color color, int width)
        {
            int index = GetDisplaySettingIndex(curveName);
            if (index >= 0 && index < DisplayPropertyList.Count)
            {
                DisplayProperty dp = DisplayPropertyList[index];
                dp.Color = color;
                dp.Width = width;
                DisplayPropertyList[index] = dp;
            }
            else
            {
                DisplayProperty dp = new DisplayProperty(curveName);
                index = DisplayPropertyList.Count;
                dp.Color = color;
                dp.Width = width;
                DisplayPropertyList.Add(dp);
            }
        }

        public void SetDefaultColor(String curveName)
        {
            int index = GetDisplaySettingIndex(curveName);
            if (index >= 0 && index < DisplayPropertyList.Count)
            {
                DisplayProperty dp = DisplayPropertyList[index];
                dp.Color = GetDefaultColor(index);
                DisplayPropertyList[index] = dp;
            }
            else
            {
                DisplayProperty dp = new DisplayProperty(curveName);
                index = DisplayPropertyList.Count;
                dp.Color = GetDefaultColor(index);
                DisplayPropertyList.Add(dp);
            }
        }

        public static Color[] DefaultColors
        {
            get
            {
                Color[] colors = new Color[]
                {
                    Color.Red     ,
                    Color.Blue    ,
                    Color.Olive   ,
                    Color.Orange  ,
                    Color.Maroon  ,
                    Color.Magenta ,
                    Color.Green   ,
                    Color.Cyan    ,
                    Color.Lime    ,
                    Color.Purple  ,
                    Color.Black   ,
                    Color.Navy    ,
                    Color.Teal    ,
                    Color.Gray    ,
                    Color.Yellow  ,
                    Color.Silver
                };

                return colors;
            }
        }

        public static Color GetDefaultColor(int index)
        {
            if (index < 0 || index >= DefaultColors.Length) return Color.Black;

            return DefaultColors[index % DefaultColors.Length];

            //switch (index)
            //{
            //    case 0: return Color.Red;
            //    case 1: return Color.Green;
            //    case 2: return Color.Blue;
            //    case 3: return Color.Yellow;
            //    case 4: return Color.Cyan;
            //    case 5: return Color.Magenta;
            //    case 6: return Color.Pink;
            //    case 7: return Color.Purple;
            //    case 8: return Color.Violet;
            //    case 9: return Color.Gray;
            //    case 10: return Color.Orange;
            //    case 11: return Color.Beige;
            //    default: return Color.Black;
            //}
        }

        public void RemoveDuplicatedItem()
        {
            // remove all duplicated items
            int count = DisplayPropertyList.Count;
            if (count <= 0) return;

            bool[] tobeDeleted = new bool[count];
            for (int i = 0; i < count; i++)
            {
                tobeDeleted[i] = false;
            }

            for (int i = 0; i < count; i++)
            {
                DisplayProperty dp = DisplayPropertyList[i];

                // delete all objects with the same name
                for (int j = i + 1; j < count; j++)
                {
                    DisplayProperty cds2 = DisplayPropertyList[j];
                    bool ignoreCase = true;
                    if (String.Compare(cds2.Name, dp.Name, ignoreCase) == 0)
                    {
                        tobeDeleted[j] = true;
                    }
                }
            }

            int k = count - 1;
            while (k >= 0)
            {
                if (tobeDeleted[k] == true)
                {
                    DisplayPropertyList.RemoveAt(k);
                }
                k--;
            }
        }

        public int GetInterpModeIndex()
        {
            int index = 0;
            switch (InterpMode)
            {
                case InterpolationMode.Default:
                    index = 0;
                    break;
                case InterpolationMode.NearestNeighbor:
                    index = 1;
                    break;
                case InterpolationMode.Bilinear:
                    index = 2;
                    break;
                case InterpolationMode.Bicubic:
                    index = 3;
                    break;
                default:
                    index = 0;
                    break;
            }

            return index;
        }

        public void SetInterpMode(int index)
        {
            InterpMode = InterpolationMode.Default;
            switch (index)
            {
                case 0:
                    InterpMode = InterpolationMode.Default;
                    break;
                case 1:
                    InterpMode = InterpolationMode.NearestNeighbor;
                    break;
                case 2:
                    InterpMode = InterpolationMode.Bilinear;
                    break;
                case 3:
                    InterpMode = InterpolationMode.Bicubic;
                    break;
                default:
                    InterpMode = InterpolationMode.Default;
                    break;
            }
        }
    }

    public enum TextFormat
    {
        PositiveInteger,
        NonNegativeInteger,
        Integer,
        PositiveFloatingNumber,
        NonNegativeFloatingNumber,
        FloatingNumber,
        TextString,
    }

    public static class Utility
    {
        public static bool ValidateIntegerTextControl(String label, String text, TextFormat type,
            int min, int max)
        {
            if (text == String.Empty)
            {
                MessageBox.Show(label + ": please enter an integer number!");
                return false;
            }

            try
            {
                int iValue = -1;
                if (int.TryParse(text, out iValue) == false)
                {
                    MessageBox.Show(label + ": incorrect format!");
                    return false;
                }

                if (type == TextFormat.PositiveInteger && iValue <= 0)
                {
                    MessageBox.Show(label + ": please enter a positive integer number!");
                    return false;
                }
                if (type == TextFormat.NonNegativeInteger && iValue < 0)
                {
                    MessageBox.Show(label + ": please enter a non-negative integer number!");
                    return false;
                }
                if (iValue < min || iValue > max)
                {
                    MessageBox.Show(label + ": please enter an integer number between " + min + " and " + max + "!");
                    return false;
                }
            }
            catch (ArgumentException ae)
            {
                MessageBox.Show(label + ": " + ae.Message);
                return false;
            }

            return true;
        }
        public static bool ValidateFloatingNumberTextControl(String label, String text, TextFormat type,
            double min, double max)
        {
            if (text == String.Empty)
            {
                MessageBox.Show(label + ": please enter a floating number!");
                return false;
            }

            try
            {
                double dValue = -1;
                if (double.TryParse(text, out dValue) == false)
                {
                    MessageBox.Show(label + ": incorrect format!");
                    return false;
                }

                if (type == TextFormat.PositiveFloatingNumber && dValue <= 0)
                {
                    MessageBox.Show(label + ": please enter a positive floating number!");
                    return false;
                }
                if (type == TextFormat.NonNegativeFloatingNumber && dValue < 0)
                {
                    MessageBox.Show(label + ": please enter a non-negative floating number!");
                    return false;
                }
                if (dValue < min || dValue > max)
                {
                    MessageBox.Show(label + ": please enter a floating number between " + min + " and " + max + "!");
                    return false;
                }
            }
            catch (ArgumentException ae)
            {
                MessageBox.Show(label + ": " + ae.Message);
                return false;
            }

            return true;
        }
        public static bool ValidateStringTextControl(String label, String text, int length)
        {
            if (text == String.Empty)
            {
                MessageBox.Show(label + ": please enter a string!");
                return false;
            }

            if (length > 0 && text.Length > length)
            {
                MessageBox.Show(label + ": please keep string length <= " + length + "!");
                return false;
            }

            return true;
        }
        public static double FormatFloatingNumber(double d)
        {
            return FormatFloatingNumber(d, 0);
        }
        public static double FormatFloatingNumber(double d, int n)
        {
            // n: number of effective digits, must be >= 0
            if (n < 0) n = 0;
            if (d == 0) return d;
            if (d > 0)
            {
                if (d <= 0.0001) d = 0.0001;
                else if (d <= 0.001) d = Math.Round(d, 4 + n);
                else if (d <= 0.01) d = Math.Round(d, 3 + n);
                else if (d <= 0.1) d = Math.Round(d, 2 + n);
                else if (d <= 1) d = Math.Round(d, 1 + n);
                else if (d <= 10) d = Math.Round(d / 10, 1 + n) * 10;
                else if (d <= 100) d = Math.Round(d / 100, 1 + n) * 100;
                else if (d <= 1000) d = Math.Round(d / 1000, 1 + n) * 1000;
                else if (d <= 10000) d = Math.Round(d / 10000, 1 + n) * 10000;
                else if (d <= 100000) d = Math.Round(d / 100000, 1 + n) * 100000;
            }
            else
            {
                if (d >= -0.0001) d = -0.0001;
                else if (d >= -0.001) d = Math.Round(d, 4 + n);
                else if (d >= -0.01) d = Math.Round(d, 3 + n);
                else if (d >= -0.1) d = Math.Round(d, 2 + n);
                else if (d >= -1) d = Math.Round(d, 1 + n);
                else if (d >= -10) d = Math.Round(d / 10, 1 + n) * 10;
                else if (d >= -100) d = Math.Round(d / 100, 1 + n) * 100;
                else if (d >= -1000) d = Math.Round(d / 1000, 1 + n) * 1000;
                else if (d >= -10000) d = Math.Round(d / 10000, 1 + n) * 10000;
                else if (d >= -100000) d = Math.Round(d / 100000, 1 + n) * 100000;
            }

            return d;
        }
        public static double RelativeDifference(double a, double b)
        {
            if (a == 0 && b == 0) return 0;
            return Math.Abs(b - a) / (0.5 * (Math.Abs(a) + Math.Abs(b)));
        }
        public static bool LineSegmentIntersectsRectangle(double x1, double y1, double x2, double y2,
            double a, double b, double c, double d)
        {
            if ((x1 < Math.Min(a, b) && x2 < Math.Min(a, b)) ||
                (x1 > Math.Max(a, b) && x2 > Math.Max(a, b)) ||
                (y1 < Math.Min(c, d) && y2 < Math.Min(c, d)) ||
                (y1 > Math.Max(c, d) && y2 > Math.Max(c, d))) return false;

            double A = y2 - y1;
            double B = -(x2 - x1);
            double C = -A * x1 - B * y1;

            //double c1 = A * a + B * c + C;
            //double c2 = A * b + B * d + C;
            //double c3 = A * a + B * d + C;
            //double c4 = A * b + B * c + C;
            if ((A * a + B * c + C) * (A * b + B * d + C) <= 0 ||
                (A * a + B * d + C) * (A * b + B * c + C) <= 0) return true;
            else return false;
        }
    }
}
