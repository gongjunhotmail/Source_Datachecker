using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace NSXYPlot
{
    public enum PlotViewMouseMode
    {
        // View
        None,
        Zoom,
        Pan,
        Select,

        // Move boundary
        MoveBoundary,
        MovePlot,
    };

    public partial class XYPlotViewCtrl : UserControl
    {
        public List<XYPlot> XYPlots;
        public XYPlotViewSetting PlotViewSetting;

        // properties
        public int TotalXYPlots { get { return (XYPlots == null) ? 0 : XYPlots.Count; } }
        public List<String> PlotNameList
        {
            get
            {
                if (XYPlots == null) XYPlots = new List<XYPlot>();

                List<String> plotNameList = new List<string>();
                foreach (XYPlot plot in XYPlots)
                {
                    if (plot != null) plotNameList.Add(plot.Name);
                }

                return plotNameList;
            }
        }

        public Point PtMouseStart;
        public Point PtMouseEnd;
        protected PlotViewMouseMode MouseMode;

        public XYPlotViewCtrl()
        {
            InitializeComponent();

            PlotViewSetting = new XYPlotViewSetting();
            XYPlots = new List<XYPlot>();

            addPlot("Plot 1");
            addPlot("Plot 2");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //LayoutPlots();

            //if (XYPlots == null) return;

            //// update Graphics and ClientRectangle
            //Graphics g = CreateGraphics();

            //// update XYPlots' Graphics
            //foreach (XYPlot plot in XYPlots) plot.Graphics = g;

            //// re-arrange XYPlots
            //LayoutPlots();

            //// update XYPlots' ClientRectangle
            //foreach (XYPlot plot in XYPlots) plot.CalculateClientRectangle();

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // update XYPlots' Graphics
            //foreach (XYPlot plot in XYPlots) plot.Graphics = e.Graphics;

            base.OnPaint(e);

            if (XYPlots == null) return;

            // re-arrange XYPlots
            LayoutPlots();

            foreach (XYPlot plot in XYPlots)
            {
                if (plot == null) continue;

                plot.Graphics = e.Graphics;
                plot.XYPlotSettings.Landscape = PlotViewSetting.IsPortrait == false;
                //plot.Draw();
            }

            Draw();
        }

        public virtual void Draw()
        {
            //if (ReDraw == false) return;

            // calculate max legend width/height
            int legendWHView = 0;
            foreach (XYPlot plot in XYPlots)
            {
                if (plot == null) continue;
                if (plot.Visible == true)
                {
                    if (plot.LegendWH > legendWHView) legendWHView = plot.LegendWH;
                }
            }

            foreach (XYPlot plot in XYPlots)
            {
                if (plot == null) continue;
                if (plot.Visible == true)
                {
                    plot.LegendWHView = legendWHView;

                    plot.Draw();
                }
            }
        }

        public void FullView()
        {
            if (XYPlots == null) return;

            foreach (XYPlot plot in XYPlots)
            {
                if (plot == null) continue;
                plot.XYPlotSettings.Landscape = PlotViewSetting.IsPortrait == false;
                plot.FullView();
            }

            Invalidate();
        }

        // mouse events
        private void XYPlotViewCtrl_MouseUp(object sender, MouseEventArgs e)
        {
            //using (new WaitCursor())
            {
                PlotViewMouseMode mouseMode = MouseMode;
                if (Control.ModifierKeys == Keys.Shift)
                {
                    mouseMode = PlotViewMouseMode.Zoom;
                }
                else if (Control.ModifierKeys == Keys.Control)
                {
                    mouseMode = PlotViewMouseMode.Pan;
                }

                if (e.Button == MouseButtons.Left)
                {
                    PtMouseEnd = e.Location;

                    XYPlot xyPlot = GetXYPlot(PtMouseStart);
                    if (xyPlot != null)
                    {
                        switch (mouseMode)
                        {
                            case PlotViewMouseMode.Zoom:
                                if (PtMouseStart != Point.Empty)
                                {
                                    xyPlot.Zoom(PtMouseStart, PtMouseEnd);
                                    DrawReversibleRectangle(PtMouseStart, PtMouseEnd);
                                    this.Invalidate();
                                }
                                break;

                            case PlotViewMouseMode.Pan:
                                if (PtMouseStart != Point.Empty)
                                {
                                    xyPlot.Pan(PtMouseStart, PtMouseEnd);
                                    //DrawReversibleRectangle(PtMouseStart, PtMouseEnd);
                                    DrawReversibleLine(PtMouseStart, PtMouseEnd);
                                    this.Invalidate();
                                }
                                break;

                            //case PlotViewMouseMode.Select:
                            //    Select(xyPlot, PtMouseStart, PtMouseEnd);
                            //    DrawReversibleRectangle(PtMouseStart, PtMouseEnd);
                            //    this.Invalidate();
                            //    break;

                            case PlotViewMouseMode.None:
                                DrawReversibleCrossLines(PtMouseEnd);

                                // move plot
                                XYPlot xyPlot2 = GetXYPlot(PtMouseEnd);
                                if (xyPlot != null && xyPlot2 != null && xyPlot != xyPlot2) movePlot(xyPlot, xyPlot2);
                                break;

                            default:
                                break;
                        }
                    }
                    PtMouseStart = Point.Empty;
                    PtMouseEnd = Point.Empty;
                }
            }
        }

        private void XYPlotViewCtrl_MouseDown(object sender, MouseEventArgs e)
        {
            PlotViewMouseMode mouseMode = MouseMode;
            if (Control.ModifierKeys == Keys.Shift)
            {
                mouseMode = PlotViewMouseMode.Zoom;
            }
            else if (Control.ModifierKeys == Keys.Control)
            {
                mouseMode = PlotViewMouseMode.Pan;
            }

            if (e.Button == MouseButtons.Left)
            {
                PtMouseStart = e.Location;
                PtMouseEnd = PtMouseStart;

                switch (mouseMode)
                {
                    case PlotViewMouseMode.None:
                        DrawReversibleCrossLines(PtMouseStart);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                PtMouseStart = e.Location;
            }
        }

        private void XYPlotViewCtrl_MouseMove(object sender, MouseEventArgs e)
        {
            Point ptCurrent = e.Location;
            if (ptCurrent == PtMouseStart) return;
            //if (Math.Abs(ptCurrent.X - PtMouseStart.X) <= 2 &&
            //    Math.Abs(ptCurrent.Y - PtMouseStart.Y) <= 2) return;

            PlotViewMouseMode mouseMode = MouseMode;
            if (Control.ModifierKeys == Keys.Shift)
            {
                mouseMode = PlotViewMouseMode.Zoom;
            }
            else if (Control.ModifierKeys == Keys.Control)
            {
                mouseMode = PlotViewMouseMode.Pan;
            }

            // Check if the left mouse button is pressed.
            if (e.Button == MouseButtons.Left)
            {
                switch (mouseMode)
                {
                    case PlotViewMouseMode.Zoom:
                        // erase previous rectangle
                        DrawReversibleRectangle(PtMouseStart, PtMouseEnd);

                        // draw new rectangle
                        DrawReversibleRectangle(PtMouseStart, ptCurrent);
                        break;

                    case PlotViewMouseMode.Pan:
                        // erase previous rectangle
                        //DrawReversibleRectangle(PtMouseStart, PtMouseEnd);
                        DrawReversibleLine(PtMouseStart, PtMouseEnd);

                        // draw new rectangle
                        //DrawReversibleRectangle(PtMouseStart, ptCurrent);
                        DrawReversibleLine(PtMouseStart, ptCurrent);
                        break;

                    case PlotViewMouseMode.Select:
                        // erase previous rectangle
                        DrawReversibleRectangle(PtMouseStart, PtMouseEnd);

                        // draw new rectangle
                        DrawReversibleRectangle(PtMouseStart, ptCurrent);
                        break;

                    case PlotViewMouseMode.None:
                        XYPlot xyPlot = GetXYPlot(PtMouseStart);
                        if (xyPlot != null)
                        {
                            // erase previous cross lines
                            DrawReversibleCrossLines(PtMouseEnd);

                            // draw new cross lines
                            DrawReversibleCrossLines(ptCurrent);
                        }
                        break;

                    default:
                        break;
                }
            }

            PtMouseEnd = ptCurrent;
        }

        private void XYPlotViewCtrl_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = e.Delta;
            Point pt = new Point(e.Location.X - this.Left, e.Location.Y - this.Top);
            XYPlot xyPlot = GetXYPlot(pt);
            if (xyPlot != null)
            {
                xyPlot.Zoom(delta);
                this.Invalidate();
            }
        }

        // plot related events, used to change view limits together
        protected void SubscribePlotEvents(XYPlot plot)
        {
            plot.ViewXChanged += new XYPlotEvent(OnViewXChanged);
            plot.ViewYChanged += new XYPlotEvent(OnViewYChanged);
        }
        protected void OnViewXChanged(object sender, XYPlotEventArgs e)
        {
            XYPlot plot = (XYPlot)sender;
            if (plot == null) return;

            double minView = plot.XYPlotSettings.XAxis.ViewMin;
            double maxView = plot.XYPlotSettings.XAxis.ViewMax;

            foreach (XYPlot xyplot in XYPlots)
            {
                if (xyplot.ZoomX == false) continue;
                xyplot.XYPlotSettings.XAxis.ViewMin = minView;
                xyplot.XYPlotSettings.XAxis.ViewMax = maxView;
            }
        }
        protected void OnViewYChanged(object sender, XYPlotEventArgs e)
        {
            XYPlot plot = (XYPlot)sender;
            if (plot == null) return;

            double minView = plot.XYPlotSettings.YAxis.ViewMin;
            double maxView = plot.XYPlotSettings.YAxis.ViewMax;

            foreach (XYPlot xyplot in XYPlots)
            {
                if (xyplot.ZoomY == false) continue;
                xyplot.XYPlotSettings.YAxis.ViewMin = minView;
                xyplot.XYPlotSettings.YAxis.ViewMax = maxView;
            }
        }

        // context menu
        private void toolStripMenuItemAddPlot_Click(object sender, EventArgs e)
        {
            XYPlot xyPlot = addPlot("Test");
        }

        private void deletePlotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point location = this.PtMouseStart;
            XYPlot xyPlot = GetXYPlot(location);
            if (xyPlot != null)
            {
                deletePlot(xyPlot);
            }
        }

        private void toolStripMenuItemPlotProperty_Click(object sender, EventArgs e)
        {
            Point location = this.PtMouseStart;
            XYPlot xyPlot = GetXYPlot(location);
            if (xyPlot != null)
            {
                xyPlot.SetPlotTitle();
                XYPlotSettingDlg dlg = new XYPlotSettingDlg(xyPlot.XYPlotSettings, xyPlot);
                dlg.InitialDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }

        private void toolStripMenuItemPlotViewProperty_Click(object sender, EventArgs e)
        {
            XYPlotViewSettingDlg dlg = new XYPlotViewSettingDlg(PlotViewSetting, this);

            if (dlg.ShowDialog() == DialogResult.OK)
            {

            }
        }

        // public methods
        public void LayoutPlots()
        {
            Rectangle rect = ClientRectangle;
            rect.Inflate(-2, -2);
            if (XYPlots == null) return;
            if (PlotViewSetting == null || PlotViewSetting.PlotViewStructList == null) return;

            // calculate total width/height
            double totalWidth = PlotViewSetting.TotalWidth();

            int x = rect.Location.X;
            int y = rect.Location.Y;
            if (PlotViewSetting.IsPortrait == true)
            {
                for (int i = 0; i < TotalXYPlots; i++)
                {
                    if (XYPlots[i] == null) continue;
                    if (PlotViewSetting.PlotViewStructList[i].ShowPlot == false) continue;

                    XYPlots[i].Location = new Point(x, y);
                    int width = (int)(PlotViewSetting.PlotViewStructList[i].Width * rect.Width / totalWidth);
                    XYPlots[i].Size = new Size(width, rect.Height);
                    x += width;
                }
            }
            else
            {
                for (int i = 0; i < TotalXYPlots; i++)
                {
                    if (XYPlots[i] == null) continue;
                    if (PlotViewSetting.PlotViewStructList[i].ShowPlot == false) continue;

                    XYPlots[i].Location = new Point(x, y);
                    int height = (int)(PlotViewSetting.PlotViewStructList[i].Width * rect.Height / totalWidth);
                    XYPlots[i].Size = new Size(rect.Width, height);
                    y += height;
                }
            }
        }

        public virtual XYPlot GetXYPlot(Point pt)
        {
            XYPlot xyPlot = null;

            foreach (XYPlot plot in XYPlots)
            {
                //if (plot.Visible == true)
                {
                    Rectangle rect = new Rectangle(plot.Location, plot.Size);
                    if (rect.Contains(pt)) xyPlot = plot;
                }
            }

            return xyPlot;
        }

        protected void DrawReversibleRectangle(Point p1, Point p2)
        {
            if (p1 == p2) return;
            p1 = this.PointToScreen(p1);
            p2 = this.PointToScreen(p2);
            int left = Math.Min(p1.X, p2.X);
            int right = Math.Max(p1.X, p2.X);
            int top = Math.Min(p1.Y, p2.Y);
            int bottom = Math.Max(p1.Y, p2.Y);
            Rectangle rect = new Rectangle(left, top, right - left, bottom - top);
            Color color = Color.LightGray;
            //color.A = 0.5;
            //ControlPaint.DrawReversibleFrame(rect, color, FrameStyle.Dashed);
            ControlPaint.FillReversibleRectangle(rect, Color.Transparent);
        }

        protected void DrawReversibleLine(Point p1, Point p2)
        {
            if (p1 == p2) return;
            p1 = this.PointToScreen(p1);
            p2 = this.PointToScreen(p2);
            ControlPaint.DrawReversibleLine(p1, p2, Color.Transparent);
        }

        public virtual void DrawReversibleCrossLines(Point point)
        {
            return;
        }

        // private methods
        private XYPlot addPlot(String name)
        {
            XYPlot xyPlot1 = new XYPlot(null, this, ClientRectangle);
            xyPlot1.Name = newPlotName(name);
            xyPlot1.Visible = true;
            xyPlot1.XYPlotSettings.PlotName = xyPlot1.Name;

            XYPlots.Add(xyPlot1);
            double width = PlotViewSetting.AverageWidth;
            XYPlotViewStruct s = new XYPlotViewStruct(xyPlot1.XYPlotSettings, width, xyPlot1.Visible);
            PlotViewSetting.PlotViewStructList.Add(s);
            SubscribePlotEvents(xyPlot1);

            this.Invalidate();

            return xyPlot1;
        }

        private void deletePlot(XYPlot xyPlot)
        {
            int index = -1;
            for (int i = 0; i < TotalXYPlots; i++)
            {
                if (XYPlots[i] == xyPlot)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                XYPlots.RemoveAt(index);
                PlotViewSetting.PlotViewStructList.RemoveAt(index);
            }

            this.Invalidate();
        }

        private void movePlot(XYPlot plotToMove, XYPlot plotMoveTo)
        {
            int indexToMove = -1;
            int indexMoveTo = -1;
            for (int i = 0; i < TotalXYPlots; i++)
            {
                if (XYPlots[i] == plotToMove)
                {
                    indexToMove = i;
                }
                else if (XYPlots[i] == plotMoveTo)
                {
                    indexMoveTo = i;
                }
            }

            if (indexToMove >= 0 && indexMoveTo >= 0 && indexToMove != indexMoveTo)
            {
                XYPlots.RemoveAt(indexToMove);
                XYPlots.Insert(indexMoveTo, plotToMove);
                XYPlotViewStruct s = PlotViewSetting.PlotViewStructList[indexToMove];
                PlotViewSetting.PlotViewStructList.RemoveAt(indexToMove);
                PlotViewSetting.PlotViewStructList.Insert(indexMoveTo, s);
            }

            this.Invalidate();
        }

        private String newPlotName(String prefix)
        {
            if (XYPlots == null) XYPlots = new List<XYPlot>();

            String name = prefix;
            if (PlotNameList.Contains(name) == false) return name;

            int i = 1;
            while (i < 1000)
            {
                name = prefix + " " + i;
                if (PlotNameList.Contains(name) == false) return name;
                i++;
            }

            return name;
        }
    }
}
