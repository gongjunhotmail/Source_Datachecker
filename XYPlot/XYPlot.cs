using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
//using System.Xml.Serialization;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters;
//using System.Runtime.Serialization.Formatters.Binary;
using BWNSDataset;
using BWNSUtility;


namespace NSXYPlot
{
    # region XYPlotEvent
    public enum XYPlotEventNumber
    {
        // for WellListChanged, TargetsChanged
        Add = 0,
        Update = 1,
        Delete = 2,
        Combination = 3,

        // for WellChanged
        Dataset = 0,
        Survey = 1,
    }

    // XYPlotEvent
    public delegate void XYPlotEvent(object sender, XYPlotEventArgs e);

    // event args
    public class XYPlotEventArgs : EventArgs
    {
        // eventNumber:
        // WellListChanged: 0 -- add; 1 -- update; 2 -- delete; 3 -- combination
        // TargetsChanged:  0 -- add; 1 -- update; 2 -- delete; 3 -- combination 
        // WellChanged:     0 -- dataset; 1 -- survey
        private XYPlotEventNumber eventNumber;
        public XYPlotEventNumber EventNumber
        {
            get { return eventNumber; }
            set { eventNumber = value; }
        }

        // md
        private double md;
        public double MD
        {
            get { return md; }
            set { md = value; }
        }

        // time
        private int time;
        public int Time
        {
            get { return time; }
            set { time = value; }
        }

        // data
        private double[] data;
        public double[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public XYPlotEventArgs(XYPlotEventNumber en)
            : base()
        {
            eventNumber = en;
            md = 0;
            time = -1;
            data = null;
        }
    }
    # endregion

    [Serializable]
    public class XYPlot
    {
        public String Name;

        // private
        [NonSerialized]
        private List<BWDataset> datasetList;

        // primary inputs
        [NonSerialized]
        public Graphics Graphics;
        public Point Location;
        public Size Size;

        public XYPlotSetting XYPlotSettings;

        [NonSerialized]
        public Control ParentControl;       // used to calculate screen coordinates
        public List<BWDataset> DatasetList
        {
            get { return datasetList; }
            set { datasetList = value; }
        }

        public Boolean Visible;
        public String Title;
        public PlotType PlotType;

        // TBD
        public double ViewMDMin;
        public double ViewMDMax;

        public static bool IsPrinting = false;
        public static int PrintingFontSizeRatio = 4;

        public bool ZoomX
        {
            get
            {
                if (XYPlotSettings != null && XYPlotSettings.Landscape == true) return true;
                return false;
            }
        }
        public bool ZoomY
        {
            get
            {
                if (XYPlotSettings != null && XYPlotSettings.Landscape == true) return false;
                return true;
            }
        }

        [NonSerialized]
        protected Rectangle rectClientArea;
        [NonSerialized]
        protected Rectangle rectPlotArea;
        [NonSerialized]
        protected Rectangle rectLegend;

        // For view to calculate and set max widths/heights of all plots in the view
        public int LegendWH
        {
            get
            {
                if (XYPlotSettings == null || XYPlotSettings.DisplayPropertyList == null || XYPlotSettings.DisplayPropertyList.Count == 0) return 0;
                if (DatasetList == null || DatasetList.Count == 0) return 0;

                int count = 0;
                foreach (BWDataset ds in DatasetList)
                {
                    if (ds == null) continue;
                    DisplayProperty dp = XYPlotSettings.GetDisplaySetting(ds.Name);
                    if (dp.Display == true) count++;
                }

                if (XYPlotSettings.Landscape == true)
                {
                    int w = (int)(XYPlotSettings.XAxis.TickSize * count * 2);
                    return w;
                }
                else
                {
                    int h = (int)(XYPlotSettings.YAxis.TickSize * count * 2);
                    return h;
                }
            }
        }
        public int LegendWHView;

        //// plot area needs to be aligned for md plots and the wellpath plot.
        //// LeftMarginMDPlot and RigthMarginMDPlot are the maximum margins for all MD plots
        //// the WellPath plot. They are calculated in graphViewPZS and assigned on all plots.
        //public int LeftMarginMDPlot;
        //public int RigthMarginMDPlot;
        [NonSerialized]
        private const int shink = 2;

        // event and delegate, TBD
        public event XYPlotEvent ViewXChanged;
        public event XYPlotEvent ViewYChanged;
        public event XYPlotEvent PlotSettingChanged;

        // constructors
        public XYPlot()
        {
            ;
        }

        public XYPlot(Graphics g, Control parentControl)
        {
            Graphics = g;
            Location = new Point(0, 0);
            Size = new Size(200, 300);
            ParentControl = parentControl;
            commonConstructor();
        }

        public XYPlot(Graphics g, Control parentControl, Point location, Size size)
        {
            Graphics = g;
            Location = location;
            Size = size;
            ParentControl = parentControl;
            commonConstructor();
        }

        public XYPlot(Graphics g, Control parentControl, Rectangle rect)
        {
            Graphics = g;
            Location = rect.Location;
            Size = rect.Size;
            ParentControl = parentControl;
            commonConstructor();
        }

        // TBD
        public virtual void CloneSerialized(XYPlot plot)
        {
            datasetList = plot.datasetList;
            Location = plot.Location;
            Size = plot.Size;
            XYPlotSettings = (XYPlotSetting)plot.XYPlotSettings.Clone();
            Visible = plot.Visible;
            Title = plot.Title;
            PlotType = plot.PlotType;

            ViewMDMin = plot.ViewMDMin;
            ViewMDMax = plot.ViewMDMax;
            //ZoomX = plot.ZoomX;
            //ZoomY = plot.ZoomY;
        }

        public void SetPlotTitle()
        {
            Title = XYPlotSettings.XAxis.Name + " - " + XYPlotSettings.YAxis.Name;
        }

        public void AddDataset(BWDataset dataset)
        {
            if (dataset == null) return;

            DatasetList.Add(dataset);

            // auto set display color
            XYPlotSettings.SetDefaultColor(dataset.Name);
        }

        public void RemoveDataset(String datasetName)
        {
            if (DatasetList == null || DatasetList.Count == 0) return;
            for (int i = DatasetList.Count - 1; i >= 0; i--)
            {
                if (DatasetList[i].Name.ToUpper() == datasetName.ToUpper())
                {
                    DatasetList.RemoveAt(i);
                }
            }

            ////DatasetListChanged();
        }

        // TBD
        public virtual void DatasetListChanged()
        {
            //datasetList.Clear();
            //datasetList.AddRange(parentDatasetList);
        }

        // public methods
        public virtual void Draw()
        {
            CalculateClientRectangle();

            Graphics = ParentControl.CreateGraphics();
            Graphics.SetClip(rectClientArea);
            Graphics.Clear(XYPlotSettings.BackColor);
            DrawGrid();
            DrawAxis();
            Graphics.SetClip(rectPlotArea);
            DrawCurve();
        }

        public virtual void CalculateClientRectangle()
        {
            rectClientArea = new Rectangle(Location, Size);
            adjustPlotArea();
        }

        private void adjustPlotArea()
        {
            if (XYPlotSettings == null) return;

            int leftMargin = calculateMargin(XYPlotSettings.YAxis, false);
            int bottomMargin = calculateMargin(XYPlotSettings.XAxis, true);
            int rightMargin = calculateMargin(XYPlotSettings.YAxis2, false);
            int topMargin = calculateMargin(XYPlotSettings.XAxis2, true);

            if (XYPlotSettings.EnableTitle == true)
            {
                topMargin += (int)(XYPlotSettings.YAxis.TickFont.Height * 2);
            }

            // create rectLegend
            if (XYPlotSettings.Landscape == true)
            {
                int w = (int)(Math.Max(LegendWH, LegendWHView) * Graphics.DpiX / 72);
                rectLegend = new Rectangle(rectClientArea.Right - w, rectPlotArea.Top, w, rectPlotArea.Height);
            }
            else
            {
                int h = (int)(Math.Max(LegendWH, LegendWHView) * Graphics.DpiY / 72);
                rectLegend = new Rectangle(rectPlotArea.Left, rectClientArea.Top, rectPlotArea.Width, h);
            }

            if (XYPlotSettings.EnableLegend == true)
            {
                //if (XYPlotSettings.Landscape == true) rightMargin += (int)(XYPlotSettings.XAxis.TickSize * 10);
                //else topMargin += (int)(XYPlotSettings.YAxis.TickSize * 10);
                if (XYPlotSettings.Landscape == true) rightMargin += Math.Max(LegendWH, LegendWHView) + 4;
                else topMargin += Math.Max(LegendWH, LegendWHView) + 4;
            }

            // image plot color bar
            //if (PlotType == BWPlotType.XYPlotImage && XYPlotSettings.ShowColorBar == true)
            if (PlotType == PlotType.XYPlotImage)
            {
                if (XYPlotSettings.Landscape == true) topMargin += XYPlotSettings.YAxis.TickFont.Height;
                else rightMargin += XYPlotSettings.XAxis.TickFont.Height;
            }

            //// enforce view's left and right margins for MD plots and the WellPath plot
            //if (LeftMarginMDPlot > leftMargin) leftMargin = LeftMarginMDPlot;
            //if (RigthMarginMDPlot > rightMargin) rightMargin = RigthMarginMDPlot;

            leftMargin = (int)(leftMargin * Graphics.DpiX / 72);
            bottomMargin = (int)(bottomMargin * Graphics.DpiY / 72);
            rightMargin = (int)(rightMargin * Graphics.DpiX / 72);
            topMargin = (int)(topMargin * Graphics.DpiY / 72);

            int left = rectClientArea.Left + leftMargin;
            int bottom = rectClientArea.Bottom - bottomMargin;
            int right = rectClientArea.Right - rightMargin;
            int top = rectClientArea.Top + topMargin;
            int width = Math.Max(right - left, 0);
            int height = Math.Max(bottom - top, 0);

            rectPlotArea = new Rectangle(left, top, width, height);
            if (width >= shink && height >= shink) rectPlotArea.Inflate((int)(-shink * Graphics.DpiX / 72), (int)(-shink * Graphics.DpiY / 72));
        }

        private int calculateMargin(Axis axis, bool isXAxis)
        {
            int margin = 0;
            if (axis != null && axis.Enabled == true)
            {
                if (axis.EnableTick == true)
                {
                    if ((isXAxis == true && axis.IsTickVertical == true) ||
                        (isXAxis == false && axis.IsTickVertical == false))
                    {   // simply leave 4 spaces
                        margin += (int)axis.TickSize * 4;
                    }
                    else
                    {
                        margin += axis.TickFont.Height;
                    }
                }
                if (axis.EnableName == true)
                {
                    margin += axis.TickFont.Height + 1;
                }
            }

            return margin;
        }

        public virtual void FullView()
        {
            double viewMinX = -1;
            double viewMaxX = -1;
            double viewMinY = -1;
            double viewMaxY = -1;

            if (XYPlotSettings.Landscape == true)
            {   // assuming x axis is depth, y axis is value
                GetStartEndDepths(out viewMinX, out viewMaxX);
                GetMinMaxValues(out viewMinY, out viewMaxY);
            }
            else
            {   // assuming y axis is depth, x axis is value
                GetMinMaxValues(out viewMinX, out viewMaxX);
                GetStartEndDepths(out viewMinY, out viewMaxY);
            }

            if (viewMinX == -1 && viewMaxX == -1)
            {
                viewMinX = XYPlotSettings.XAxis.IsLog ? 0.2 : 0;
                viewMaxX = XYPlotSettings.XAxis.IsLog ? 200 : 100;
            }
            if (viewMinY == -1 && viewMaxY == -1)
            {
                viewMinY = XYPlotSettings.YAxis.IsLog ? 0.2 : 0;
                viewMaxY = XYPlotSettings.YAxis.IsLog ? 200 : 100;
            }

            int Z = 30;
            viewMinX = viewMinX - (viewMaxX - viewMinX) / Z;
            viewMaxX = viewMaxX + (viewMaxX - viewMinX) / Z;
            viewMinY = viewMinY - (viewMaxY - viewMinY) / Z;
            viewMaxY = viewMaxY + (viewMaxY - viewMinY) / Z;

            if ((viewMinX != -1 || viewMaxX != -1) && ZoomX == true)
            {
                if (viewMinX < viewMaxX)
                {
                    XYPlotSettings.XAxis.ViewMin = viewMinX;
                    XYPlotSettings.XAxis.ViewMax = viewMaxX;
                }
                else if (viewMinX == viewMaxX)
                {
                    XYPlotSettings.XAxis.ViewMin = viewMinX - 50;
                    XYPlotSettings.XAxis.ViewMax = viewMaxX + 50;
                }

                OnChangeViewX();
            }
            if ((viewMinY != -1 || viewMaxY != -1) && ZoomY == true)
            {
                if (viewMinY < viewMaxY)
                {
                    XYPlotSettings.YAxis.ViewMin = viewMinY;
                    XYPlotSettings.YAxis.ViewMax = viewMaxY;
                }
                else if (viewMinY == viewMaxY)
                {
                    XYPlotSettings.YAxis.ViewMin = viewMinY - 50;
                    XYPlotSettings.YAxis.ViewMax = viewMaxY + 50;
                }

                OnChangeViewY();
            }
        }

        public virtual void GetStartEndDepths(out double minValue, out double maxValue)
        {
            minValue = -1;
            maxValue = -1;

            bool bFirst = true;
            foreach (BWDataset obj in DatasetList)
            {
                obj.CalculateDerivedProperties();
                if (bFirst)
                {
                    minValue = obj.StartDepth;
                    maxValue = obj.EndDepth;
                    bFirst = false;
                }
                else
                {
                    minValue = Math.Min(minValue, obj.StartDepth);
                    maxValue = Math.Max(maxValue, obj.EndDepth);
                }
            }
        }
        public void GetMinMaxValues(out double minValue, out double maxValue)
        {
            minValue = -1;
            maxValue = -1;

            bool bFirst = true;
            foreach (BWDataset obj in DatasetList)
            {
                obj.CalculateDerivedProperties();
                if (bFirst)
                {
                    minValue = obj.MinValue;
                    maxValue = obj.MaxValue;
                    bFirst = false;
                }
                else
                {
                    minValue = Math.Min(minValue, obj.MinValue);
                    maxValue = Math.Max(maxValue, obj.MaxValue);
                }
            }
        }

        public void Zoom(Point ptStart, Point ptEnd)
        {
            if (ptStart == ptEnd) return;

            int left = Math.Min(ptStart.X, ptEnd.X);
            int right = Math.Max(ptStart.X, ptEnd.X);
            int top = Math.Min(ptStart.Y, ptEnd.Y);
            int bottom = Math.Max(ptStart.Y, ptEnd.Y);
            bool bXZoomIn = (right - left) < this.rectPlotArea.Width;
            bool bYZoomIn = (bottom - top) < this.rectPlotArea.Height;

            double xViewMin = PixelToXValue(left);
            double xViewMax = PixelToXValue(right);
            double yViewMin = PixelToYValue(bottom);
            double yViewMax = PixelToYValue(top);

            // for reversed scale axis
            if (xViewMin > xViewMax)
            {
                double temp = xViewMax;
                xViewMax = xViewMin;
                xViewMin = temp;
            }
            if (yViewMin > yViewMax)
            {
                double temp = yViewMax;
                yViewMax = yViewMin;
                yViewMin = temp;
            }

            // zooming limit
            const double tolerance = 1.0e-8;
            if (Utility.RelativeDifference(xViewMin, xViewMax) < tolerance &&
                Utility.RelativeDifference(yViewMin, yViewMax) < tolerance)
            {
                return;
            }

            if (ZoomX == true)
            {   // zommable in X axis
                if (!bXZoomIn || Utility.RelativeDifference(xViewMin, xViewMax) >= tolerance)
                {   // excludes unlimited zoom in
                    double z = Utility.RelativeDifference(xViewMin, xViewMax);

                    XYPlotSettings.XAxis.ViewMin = xViewMin;
                    XYPlotSettings.XAxis.ViewMax = xViewMax;
                    OnChangeViewX();
                }
            }

            if (ZoomY == true)
            {   // zoomable in Y axis
                if (!bYZoomIn || Utility.RelativeDifference(yViewMin, yViewMax) >= tolerance)
                {   // excludes unlimited zoom in
                    XYPlotSettings.YAxis.ViewMin = yViewMin;
                    XYPlotSettings.YAxis.ViewMax = yViewMax;
                    OnChangeViewY();
                }
            }

            //Draw();
        }

        public void Zoom(int delta)
        {
            if (delta == 0) return;

            double zoomFactor = 0.1 * delta / 120.0;

            bool bXZoomIn = (delta > 0);
            bool bYZoomIn = (delta < 0);

            double xViewMin = XYPlotSettings.XAxis.ViewMin;
            double xViewMax = XYPlotSettings.XAxis.ViewMax;
            double yViewMin = XYPlotSettings.YAxis.ViewMin;
            double yViewMax = XYPlotSettings.YAxis.ViewMax;

            xViewMin = xViewMin + (xViewMax - xViewMin) * zoomFactor;
            xViewMax = xViewMax - (xViewMax - xViewMin) * zoomFactor;
            yViewMin = yViewMin + (yViewMax - yViewMin) * zoomFactor;
            yViewMax = yViewMax - (yViewMax - yViewMin) * zoomFactor;


            // for reversed scale axis
            if (xViewMin > xViewMax)
            {
                double temp = xViewMax;
                xViewMax = xViewMin;
                xViewMin = temp;
            }
            if (yViewMin > yViewMax)
            {
                double temp = yViewMax;
                yViewMax = yViewMin;
                yViewMin = temp;
            }

            // zooming limit
            const double tolerance = 1.0e-8;
            if (Utility.RelativeDifference(xViewMin, xViewMax) < tolerance &&
                Utility.RelativeDifference(yViewMin, yViewMax) < tolerance)
            {
                return;
            }

            if (ZoomX == true)
            {   // zoomable in X
                if (!bXZoomIn || Utility.RelativeDifference(xViewMin, xViewMax) >= tolerance)
                {   // excludes unlimited zoom in
                    XYPlotSettings.XAxis.ViewMin = xViewMin;
                    XYPlotSettings.XAxis.ViewMax = xViewMax;
                    OnChangeViewX();
                }
            }

            if (ZoomY == true)
            {   // zoomable in Y
                if (!bYZoomIn || Utility.RelativeDifference(yViewMin, yViewMax) >= tolerance)
                {   // excludes unlimited zoom in
                    XYPlotSettings.YAxis.ViewMin = yViewMin;
                    XYPlotSettings.YAxis.ViewMax = yViewMax;
                    OnChangeViewY();
                }
            }

            Draw();
        }

        public void Pan(Point ptStart, Point ptEnd)
        {
            if (ptStart == ptEnd) return;

            double startX = PixelToXValue(ptStart.X);
            double startY = PixelToYValue(ptStart.Y);
            double endX = PixelToXValue(ptEnd.X);
            double endY = PixelToYValue(ptEnd.Y);
            //double xPan = 0.5 * ((xViewMin + xViewMax) - (XYPlotSettings.XAxis.ViewMin +
            //    XYPlotSettings.XAxis.ViewMax));
            //double yPan = 0.5 * ((yViewMin + yViewMax) - (XYPlotSettings.YAxis.ViewMin +
            //    XYPlotSettings.YAxis.ViewMax));
            double xPan = endX - startX;
            double yPan = endY - startY;

            if (ZoomX == true)
            {   // zoomable in X
                XYPlotSettings.XAxis.ViewMin -= xPan;
                XYPlotSettings.XAxis.ViewMax -= xPan;
                OnChangeViewX();
            }

            if (ZoomY == true)
            {   // zoomable in Y
                XYPlotSettings.YAxis.ViewMin -= yPan;
                XYPlotSettings.YAxis.ViewMax -= yPan;
                OnChangeViewY();
            }

            //Draw();
        }

        public RectangleF Select(Point ptStart, Point ptEnd)
        {
            if (ptStart == ptEnd) return RectangleF.Empty;

            int left = Math.Min(ptStart.X, ptEnd.X);
            int right = Math.Max(ptStart.X, ptEnd.X);
            int top = Math.Min(ptStart.Y, ptEnd.Y);
            int bottom = Math.Max(ptStart.Y, ptEnd.Y);

            double xLeft = PixelToXValue(left);
            double xRight = PixelToXValue(right);
            double yBottom = PixelToYValue(bottom);
            double yTop = PixelToYValue(top);

            // for reversed scale axis
            if (xLeft > xRight)
            {
                double temp = xRight;
                xRight = xLeft;
                xLeft = temp;
            }
            if (yBottom > yTop)
            {
                double temp = yTop;
                yTop = yBottom;
                yBottom = temp;
            }

            // zooming limit
            const double tolerance = 1.0e-4;
            if (Utility.RelativeDifference(xLeft, xRight) < tolerance &&
                Utility.RelativeDifference(yBottom, yTop) < tolerance)
            {
                return RectangleF.Empty;
            }

            PointF position = new PointF((float)xLeft, (float)yBottom);
            SizeF size = new SizeF((float)xRight - (float)xLeft, (float)yTop - (float)yBottom);
            RectangleF rect = new RectangleF(position, size);
            return rect;
        }

        public void OnChangeViewX()
        {
            XYPlotEventArgs e = new XYPlotEventArgs(XYPlotEventNumber.Update);
            if (ViewXChanged != null)
            {
                ViewXChanged(this, e);
            }
        }

        public void OnChangeViewY()
        {
            XYPlotEventArgs e = new XYPlotEventArgs(XYPlotEventNumber.Update);
            if (ViewYChanged != null)
            {
                ViewYChanged(this, e);
            }
        }

        public void DrawReversibleCrossLines(Point pt)
        {
            // test
            Point p1 = new Point(rectPlotArea.Left, pt.Y);
            Point p2 = new Point(rectPlotArea.Right, pt.Y);
            //String strTemp = "DrawReversibleCrossLines: P1: " + p1.ToString() + " P2: " + p2.ToString();
            //BWNSUtility.Utility.DebugLog(strTemp);
            p1 = this.ParentControl.PointToScreen(p1);
            p2 = this.ParentControl.PointToScreen(p2);
            ControlPaint.DrawReversibleLine(p1, p2, Color.Transparent);
            p1 = new Point(pt.X, rectPlotArea.Top);
            p2 = new Point(pt.X, rectPlotArea.Bottom);
            p1 = this.ParentControl.PointToScreen(p1);
            p2 = this.ParentControl.PointToScreen(p2);
            ControlPaint.DrawReversibleLine(p1, p2, Color.Transparent);
        }

        public void DrawReversibleCrossLines(double value, bool isXValue)
        {
            // test
            //System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\Temp\\testout.txt", true);
            if (value == BWDataset.NO_READING) return;
            if (isXValue == true)
            {
                if (value < XYPlotSettings.XAxis.ViewMin ||
                    value > XYPlotSettings.XAxis.ViewMax) return;
            }
            else
            {
                if (value < XYPlotSettings.YAxis.ViewMin ||
                    value > XYPlotSettings.YAxis.ViewMax) return;
            }

            Point p1 = Point.Empty;
            Point p2 = Point.Empty;
            if (isXValue)
            {
                int xPixel = XValueToPixel(value);
                p1 = new Point(xPixel, rectPlotArea.Top);
                p2 = new Point(xPixel, rectPlotArea.Bottom);
            }
            else
            {
                int yPixel = YValueToPixel(value);
                p1 = new Point(rectPlotArea.Left, yPixel);
                p2 = new Point(rectPlotArea.Right, yPixel);
            }
            //String strTemp = "DrawReversibleCrossLines: P1: " + p1.ToString() + " P2: " + p2.ToString();
            //sw.WriteLine(strTemp);
            //sw.Close();
            p1 = this.ParentControl.PointToScreen(p1);
            p2 = this.ParentControl.PointToScreen(p2);
            ControlPaint.DrawReversibleLine(p1, p2, Color.Transparent);
        }

        public void DrawRectangle(Pen pen, double x1, double x2, double y1, double y2)
        {
            int x1p = XValueToPixel(x1);
            int x2p = XValueToPixel(x2);
            int y1p = YValueToPixel(y1);
            int y2p = YValueToPixel(y2);
            Rectangle rect = new Rectangle();
            rect.Location = new Point(x1p, y2p);
            rect.Size = new Size(Math.Abs(x2p - x1p), Math.Abs(y2p - y1p));
            Graphics.SetClip(rectPlotArea);
            //Graphics.DrawRectangle(pen, rect);
            XYPlot.DrawRectangle(Graphics, pen, rect);
        }

        // protected methods
        protected virtual void commonConstructor()
        {
            Name = "Plot 1";

            CalculateClientRectangle();

            datasetList = new List<BWDataset>();

            XYPlotSettings = new XYPlotSetting();
            Visible = false;
            Title = "Default Plot Title";
            PlotType = PlotType.XYPlot;
            ViewMDMin = 0;
            ViewMDMax = 0;

            //IsPrinting = false;
            //PrintingFontSizeRatio = 4;

            //ZoomX = true;
            //ZoomY = true;
        }

        // Curve specific
        public virtual void DrawCurve()
        {
            for (int i = 0; i < DatasetList.Count; i++)
            {
                BWDataset dataset = DatasetList[i];
                DrawRegularCurve(dataset);
            }
        }

        protected virtual void DrawRegularCurve(BWDataset dataset)
        {
            int nTotalRecords = 0;
            double[] dData = null;
            dataset.GetData(out nTotalRecords, out dData);
            DisplayProperty dp = XYPlotSettings.GetDisplaySetting(dataset.Name);
            if (dp.Display == false) return;
            //Pen penCurve = new Pen(dp.Color, dp.Width);
            Axis xAxis = XYPlotSettings.XAxis;
            Axis yAxis = XYPlotSettings.YAxis;

            // check if overwrite primary y axis
            if (dp.TempAxisOverWrite == true)
            {
                Axis axis = new Axis();
                axis.IsLog = dp.TempAxisIsLog;
                axis.ViewMin = dp.TempAxisViewMin;
                axis.ViewMax = dp.TempAxisViewMax;
                axis.ReversedScale = dp.TempAxisReversedScale;

                if (XYPlotSettings.Landscape == true) yAxis = axis;
                else xAxis = axis;
            }

            if (xAxis.IsLog && (xAxis.ViewMin <= 0 || xAxis.ViewMax <= 0)) return;
            if (yAxis.IsLog && (yAxis.ViewMin <= 0 || yAxis.ViewMax <= 0)) return;

            // find startIndex1 and endIndex1
            int startIndex = -1;
            int endIndex = nTotalRecords;
            int nTotalColumns = dataset.TotalColumns;
            if (XYPlotSettings.Landscape == true)
            {
                FindStartEndIndices(nTotalRecords, dData, nTotalColumns, 0, 1, xAxis.ViewMin,
                    xAxis.ViewMax, yAxis.ViewMin, yAxis.ViewMax, ref startIndex, ref endIndex);
            }
            else
            {
                FindStartEndIndices(nTotalRecords, dData, nTotalColumns, 0, 1, yAxis.ViewMin,
                    yAxis.ViewMax, xAxis.ViewMin, xAxis.ViewMax, ref startIndex, ref endIndex);
            }

            if (startIndex >= 0 && endIndex < nTotalRecords)
            {
                if (startIndex > 0) startIndex--;
                if (endIndex < nTotalRecords - 1) endIndex++;
                int count = endIndex - startIndex + 1;
                if (count >= 1)
                {
                    List<Point> pointList = new List<Point>();
                    // stop and draw as
                    // 1. step > DataMissingThredshold
                    bool isDown = true;     // natural order or first order
                    for (int j = startIndex; j <= endIndex; j++)
                    {
                        // special handling for LWD data, which may have 4 columns: Depth, Value, OnBottom, IsBad
                        if (PlotType == PlotType.XYPlotCurve && nTotalColumns >= 4)
                        {
                            bool onBottom = Math.Abs(dData[nTotalColumns * j + 2] - 1) < 0.1 ? true : false;
                            bool isBad = Math.Abs(dData[nTotalColumns * j + 3] - 1) < 0.1 ? true : false;

                            //if (onBottom == false || isBad == true) continue;
                            if (isBad == true) continue;
                        }

                        double x = dData[nTotalColumns * j];
                        double y = dData[nTotalColumns * j + 1];
                        if (XYPlotSettings.Landscape == false)
                        {
                            y = dData[nTotalColumns * j];
                            x = dData[nTotalColumns * j + 1];
                        }

                        if (j > startIndex)
                        {
                            double depthLast = dData[nTotalColumns * (j - 1)];
                            double depth = dData[nTotalColumns * j];
                            bool isDataMissing = (Math.Abs(depth - depthLast) > XYPlotSettings.DataMissingThredshold);
                            if (isDataMissing)
                            {   // draw points and lines
                                drawPointsLines(pointList, dp, isDown, dataset.InterpolateFlag);
                                pointList.Clear();
                            }
                        }

                        int pixelX = XValueToPixel(x, xAxis);
                        int pixelY = YValueToPixel(y, yAxis);
                        Point pt = new Point(pixelX, pixelY);
                        pointList.Add(pt);
                    }

                    drawPointsLines(pointList, dp, isDown, dataset.InterpolateFlag);
                }
            }
        }

        protected virtual void drawPointsLines(List<Point> pointList, DisplayProperty dp, bool isDown,
            BWInterpolateFlag flag)
        {
            if (pointList == null || pointList.Count == 0) return;

            if (dp.ShowSymbol == true)
            {
                foreach (Point pt in pointList) DrawSymbol(pt, dp);
            }

            Point[] ptCurve = pointList.ToArray();
            if (dp.ShowLine == true && isDown == true)
            {
                Pen penCurve = new Pen(dp.Color, dp.Width);
                switch (flag)
                {
                    case BWInterpolateFlag.flagInterpolateLine:
                        XYPlot.DrawLines(Graphics, penCurve, ptCurve);
                        break;
                    default:
                        XYPlot.DrawLines(Graphics, penCurve, ptCurve, flag, false);
                        break;
                }
                penCurve.Dispose();
            }
        }

        protected virtual void DrawXGrid()
        {
            Pen penMajorGrid = new Pen(XYPlotSettings.MajorGridColor, XYPlotSettings.MajorGridWidth);
            penMajorGrid.Alignment = PenAlignment.Center;
            Pen penMinorGrid = new Pen(XYPlotSettings.MinorGridColor, XYPlotSettings.MinorGridWidth);
            //Graphics.DrawRectangle(penMinorGrid, rectClientArea);
            XYPlot.DrawRectangle(Graphics, penMinorGrid, rectClientArea);

            // vertical major grid
            int totalTicks = XYPlotSettings.XAxis.TotalTicks;
            double min = XYPlotSettings.XAxis.ViewMin;
            double max = XYPlotSettings.XAxis.ViewMax;
            Color color = XYPlotSettings.XAxis.TickColor;
            Font font = XYPlotSettings.XAxis.TickFont;
            List<double> majorGrids = CalculateMajorGrids(XYPlotSettings.XAxis);
            totalTicks = majorGrids.Count;

            if (XYPlotSettings.ShowMinorGrid)
            {
                DrawMinorGrids(XYPlotSettings.XAxis, majorGrids, penMinorGrid, true);
            }

            for (int i = 0; i < totalTicks; i++)
            {
                double dXTick = majorGrids[i];
                int pixelXTick = XValueToPixel(dXTick);
                Point p1 = new Point(pixelXTick, rectPlotArea.Bottom);
                Point p2 = new Point(pixelXTick, rectPlotArea.Top);

                if (XYPlotSettings.ShowMajorGrid)
                {
                    DrawLine(Graphics, penMajorGrid, p1, p2);
                }

                if (XYPlotSettings.XAxis.EnableTick)
                {
                    Brush brushXMark = new SolidBrush(color);
                    String xTick = dXTick.ToString();
                    bool bVertical = XYPlotSettings.XAxis.IsTickVertical;
                    DrawString(Graphics, xTick, font, brushXMark, p1, bVertical, StringAlignment.Center, 1);
                }
            }

            penMajorGrid.Dispose();
            penMinorGrid.Dispose();
        }

        protected virtual void DrawYGrid()
        {
            Pen penMajorGrid = new Pen(XYPlotSettings.MajorGridColor, XYPlotSettings.MajorGridWidth);
            penMajorGrid.Alignment = PenAlignment.Center;
            Pen penMinorGrid = new Pen(XYPlotSettings.MinorGridColor, XYPlotSettings.MinorGridWidth);
            //Graphics.DrawRectangle(penMinorGrid, rectClientArea);
            XYPlot.DrawRectangle(Graphics, penMinorGrid, rectClientArea);

            int totalTicks = XYPlotSettings.YAxis.TotalTicks;
            double min = XYPlotSettings.YAxis.ViewMin;
            double max = XYPlotSettings.YAxis.ViewMax;
            Color color = XYPlotSettings.YAxis.TickColor;
            Font font = XYPlotSettings.YAxis.TickFont;
            List<double> majorGrids = CalculateMajorGrids(XYPlotSettings.YAxis);
            totalTicks = majorGrids.Count;

            if (XYPlotSettings.ShowMinorGrid)
            {
                DrawMinorGrids(XYPlotSettings.YAxis, majorGrids, penMinorGrid, false);
            }

            // horizontal major grid
            for (int j = 0; j < totalTicks; j++)
            {
                //double dYTick = min + j * (max - min) / totalTicks;
                double dYTick = majorGrids[j];
                int pixelYTick = YValueToPixel(dYTick);
                Point p1 = new Point(rectPlotArea.Left, pixelYTick);
                Point p2 = new Point(rectPlotArea.Right, pixelYTick);

                if (XYPlotSettings.ShowMajorGrid)
                {
                    DrawLine(Graphics, penMajorGrid, p1, p2);
                }

                if (XYPlotSettings.YAxis.EnableTick)
                {
                    Brush brushYMark = new SolidBrush(color);
                    String yTick = dYTick.ToString();
                    bool bVertical = XYPlotSettings.YAxis.IsTickVertical;
                    DrawString(Graphics, yTick, font, brushYMark, p1, bVertical, StringAlignment.Center, 5);
                }
            }

            penMajorGrid.Dispose();
            penMinorGrid.Dispose();
        }

        protected virtual void DrawGrid()
        {
            DrawXGrid();
            DrawYGrid();
        }

        protected virtual void DrawAxis()
        {
            // X axis
            Pen penXAxis = new Pen(XYPlotSettings.XAxis.AxisColor, XYPlotSettings.XAxis.AxisWidth);
            Point p1 = new Point(rectPlotArea.Left, rectPlotArea.Bottom);
            Point p2 = new Point(rectPlotArea.Right, rectPlotArea.Bottom);
            penXAxis.EndCap = LineCap.ArrowAnchor;
            DrawLine(Graphics, penXAxis, p1, p2);

            // x axis unit, use first dataset unit
            Brush brushXUnit = new SolidBrush(XYPlotSettings.XAxis.TickColor);
            String depthUnit = (datasetList.Count > 0 && datasetList[0] != null) ? datasetList[0].DepthUnit : String.Empty;
            //Graphics.DrawString(depthUnit, XYPlotSettings.XAxis.TickFont, brushXUnit, p2);
            DrawString(Graphics, depthUnit, XYPlotSettings.XAxis.TickFont, brushXUnit, p2,
                false, StringAlignment.Center, 0);
            // x axis label
            if (XYPlotSettings.XAxis.EnableName == true)
            {
                Point pXLabel = new Point((p1.X + p2.X) / 2, p1.Y);
                if (XYPlotSettings.XAxis.EnableTick == true)
                {
                    if (XYPlotSettings.XAxis.IsTickVertical == false)
                    {
                        pXLabel.Y += XYPlotSettings.XAxis.TickFont.Height;
                    }
                    else
                    {
                        pXLabel.Y += (int)XYPlotSettings.XAxis.TickSize * 4;
                    }
                }
                DrawString(Graphics, XYPlotSettings.XAxis.Name, XYPlotSettings.XAxis.TickFont, brushXUnit,
                    pXLabel, false, StringAlignment.Center, 1);
            }

            // Y axis
            Pen penYAxis = new Pen(XYPlotSettings.YAxis.AxisColor, XYPlotSettings.YAxis.AxisWidth);
            p1 = new Point(rectPlotArea.Left, rectPlotArea.Bottom);
            p2 = new Point(rectPlotArea.Left, rectPlotArea.Top);
            penYAxis.EndCap = LineCap.ArrowAnchor;
            DrawLine(Graphics, penYAxis, p1, p2);

            // y axis unit, use first dataset unit
            Brush brushYUnit = new SolidBrush(XYPlotSettings.YAxis.TickColor);
            String valueUnit = (datasetList.Count > 0 && datasetList[0] != null) ? datasetList[0].ValueUnit : String.Empty;
            //Graphics.DrawString(valueUnit, XYPlotSettings.YAxis.TickFont, brushYUnit, p2);
            DrawString(Graphics, valueUnit, XYPlotSettings.YAxis.TickFont, brushYUnit, p2,
                false, StringAlignment.Center, 0);
            // y axis label
            if (XYPlotSettings.YAxis.EnableName == true)
            {
                Point pYLabel = new Point(p1.X, (p1.Y + p2.Y) / 2);
                if (XYPlotSettings.YAxis.EnableTick == true)
                {
                    if (XYPlotSettings.YAxis.IsTickVertical == false)
                    {
                        pYLabel.X -= (int)XYPlotSettings.YAxis.TickSize * 4;
                    }
                    else
                    {
                        pYLabel.X -= XYPlotSettings.YAxis.TickFont.Height;
                    }
                }
                DrawString(Graphics, XYPlotSettings.YAxis.Name, XYPlotSettings.YAxis.TickFont, brushYUnit,
                    pYLabel, true, StringAlignment.Center, 5);
            }

            // title
            if (XYPlotSettings.EnableTitle == true)
            {
                DrawString(Graphics, Title, XYPlotSettings.XAxis.TickFont, brushXUnit, rectClientArea.Location,
                    false, StringAlignment.Center, 0);
            }

            // legend. do not draw legend on image plot
            if (XYPlotSettings.EnableLegend == true)
            {
                if (XYPlotSettings.Landscape == true) DrawLegendForLandscapePlot();
                else DrawLegendForPortraitPlot();
            }

            penXAxis.Dispose();
            penYAxis.Dispose();
            brushXUnit.Dispose();
            brushYUnit.Dispose();
        }

        protected void DrawMinorGrids(Axis axis, List<double> majorGrids, Pen penMinorGrid, bool isVertical)
        {
            if (axis == null || majorGrids == null || majorGrids.Count == 0)
            {
                return;
            }

            int totalTicks = majorGrids.Count;
            double interval = 10.0;
            if (axis.IsLog == false)
            {
                if (totalTicks > 1) interval = Math.Abs(majorGrids[1] - majorGrids[0]) / 5.0;
                if (interval < 1.0e-6) return;

                double minGrid = Math.Min(majorGrids[0], majorGrids[totalTicks - 1]) - interval * 5;
                double maxGrid = Math.Max(majorGrids[0], majorGrids[totalTicks - 1]) + interval * 5;
                double tick = minGrid;
                while (tick < maxGrid)
                {
                    if ((tick - axis.ViewMin) * (axis.ViewMax - tick) >= 0)
                    {
                        if (isVertical == false)
                        {
                            int pixelTick = YValueToPixel(tick);
                            Point p1 = new Point(rectPlotArea.Left, pixelTick);
                            Point p2 = new Point(rectPlotArea.Right, pixelTick);
                            DrawLine(Graphics, penMinorGrid, p1, p2);
                        }
                        else
                        {
                            int pixelTick = XValueToPixel(tick);
                            Point p1 = new Point(pixelTick, rectPlotArea.Top);
                            Point p2 = new Point(pixelTick, rectPlotArea.Bottom);
                            DrawLine(Graphics, penMinorGrid, p1, p2);
                        }
                    }

                    tick += interval;
                }
            }
            else
            {
                double minGrid = Math.Min(majorGrids[0], majorGrids[totalTicks - 1]) / 10.0;
                double maxGrid = Math.Max(majorGrids[0], majorGrids[totalTicks - 1]) * 10.0;
                double intervalMajor = minGrid * 9;
                interval = intervalMajor / 9.0;
                double tick = minGrid;
                //int index = (axis.ReversedScale == false) ? 0 : totalTicks - 1;
                int index = 0;
                double torelance = 1.0e-6;
                while (tick < maxGrid)
                {
                    if ((tick - axis.ViewMin) * (axis.ViewMax - tick) >= 0)
                    {
                        if (isVertical == false)
                        {
                            int pixelTick = YValueToPixel(tick);
                            Point p1 = new Point(rectPlotArea.Left, pixelTick);
                            Point p2 = new Point(rectPlotArea.Right, pixelTick);
                            DrawLine(Graphics, penMinorGrid, p1, p2);
                        }
                        else
                        {
                            int pixelTick = XValueToPixel(tick);
                            Point p1 = new Point(pixelTick, rectPlotArea.Top);
                            Point p2 = new Point(pixelTick, rectPlotArea.Bottom);
                            DrawLine(Graphics, penMinorGrid, p1, p2);
                        }
                    }

                    if (tick >= majorGrids[index] - torelance && tick < majorGrids[index] + interval - torelance)
                    {
                        // reset tick starting point
                        tick = majorGrids[index];

                        //// recalculate interval
                        //if (axis.ReversedScale == false && index < totalTicks - 1)
                        //{
                        //    index++;
                        //    interval = (majorGrids[index] - majorGrids[index - 1]) / 9.0;
                        //}
                        //else if (axis.ReversedScale == true && index > 0)
                        //{
                        //    index--;
                        //    interval = (majorGrids[index+1] - majorGrids[index]) / 9.0;
                        //}
                        //else
                        //{
                        //    interval *= 10;
                        //}
                        // recalculate interval
                        if (index < totalTicks - 1)
                        {
                            index++;
                            interval = (majorGrids[index] - majorGrids[index - 1]) / 9.0;
                        }
                        else
                        {
                            interval *= 10;
                        }
                    }

                    tick += interval;
                }
            }
        }

        protected virtual void DrawLegendForLandscapePlot()
        {
            int width = (int)(Math.Max(LegendWH, LegendWHView) * Graphics.DpiX / 72);
            rectLegend = new Rectangle(rectClientArea.Right - width, rectPlotArea.Top, width, rectPlotArea.Height);
            int fontSize = (int)(XYPlotSettings.XAxis.TickSize * Graphics.DpiX / 72);
            int xPixel = rectLegend.Left + fontSize * 2;

            DrawRectangle(Graphics, new Pen(Color.Black, 1f), rectLegend);
            for (int i = 0; i < DatasetList.Count; i++)
            {
                if (DatasetList[i] == null) continue;
                if (DatasetList[i].DataType == BWDataType.typeDataImageDirRes ||
                    DatasetList[i].DataType == BWDataType.typeDataImageNBGamma) continue;
                DisplayProperty dp = XYPlotSettings.GetDisplaySetting(DatasetList[i].Name);
                if (dp.Display == false) continue;

                int curveWidth = (int)dp.Width;
                Pen penT = new Pen(dp.Color, curveWidth);
                Point p1T = new Point(xPixel, rectLegend.Top);
                Point p2T = new Point(xPixel, rectLegend.Bottom);
                DrawLine(Graphics, penT, p1T, p2T);
                xPixel -= fontSize * 6 / 10;
                Point pT = new Point(xPixel, (rectLegend.Top + rectLegend.Bottom) / 2);
                //Graphics.DrawString(DatasetList[i].Name, XYPlotSettings.XAxis.TickFont, brushXUnit, pT);
                Brush brushT = new SolidBrush(dp.Color);
                DrawString(Graphics, DatasetList[i].Name, XYPlotSettings.XAxis.TickFont, brushT, pT, true, StringAlignment.Center, 4);

                double minValue = BWUtility.FormatFloatingNumber(XYPlotSettings.YAxis.ViewMin, 2);
                double maxValue = BWUtility.FormatFloatingNumber(XYPlotSettings.YAxis.ViewMax, 2);
                String leftText = XYPlotSettings.YAxis.ReversedScale ? maxValue.ToString() : minValue.ToString();
                String riteText = XYPlotSettings.YAxis.ReversedScale ? minValue.ToString() : maxValue.ToString();
                //if (dp.TempAxisOverWrite == true && dp.TempAxisDisplayScale == true)
                if (dp.TempAxisOverWrite == true)
                {
                    minValue = BWUtility.FormatFloatingNumber(dp.TempAxisViewMin, 4);
                    maxValue = BWUtility.FormatFloatingNumber(dp.TempAxisViewMax, 4);
                    leftText = dp.TempAxisReversedScale ? maxValue.ToString() : minValue.ToString();
                    riteText = dp.TempAxisReversedScale ? minValue.ToString() : maxValue.ToString();
                }

                pT = new Point(xPixel, rectLegend.Bottom - fontSize * 2);
                //Graphics.DrawString(leftText, XYPlotSettings.XAxis.TickFont, brushXUnit, pT);
                //Graphics.DrawString(riteText, XYPlotSettings.XAxis.TickFont, brushXUnit, pT);
                DrawString(Graphics, leftText, XYPlotSettings.XAxis.TickFont, brushT, pT, true, StringAlignment.Center, 4);
                pT = new Point(xPixel, rectLegend.Top + fontSize * 2);
                DrawString(Graphics, riteText, XYPlotSettings.XAxis.TickFont, brushT, pT, true, StringAlignment.Center, 4);

                xPixel += fontSize * 24 / 10;
                penT.Dispose();
                brushT.Dispose();
            }
        }

        protected virtual void DrawLegendForPortraitPlot()
        {
            int height = (int)(Math.Max(LegendWH, LegendWHView) * Graphics.DpiY / 72);
            rectLegend = new Rectangle(rectPlotArea.Left, rectClientArea.Top, rectPlotArea.Width, height);
            int fontSize = (int)(XYPlotSettings.YAxis.TickSize * Graphics.DpiY / 72);
            int yPixel = rectLegend.Top + fontSize * 2;

            DrawRectangle(Graphics, new Pen(Color.Black, 1f), rectLegend);
            for (int i = 0; i < DatasetList.Count; i++)
            {
                if (DatasetList[i] == null) continue;
                //if (DatasetList[i].DataType == BWDataType.typeDataImageDirRes ||
                //    DatasetList[i].DataType == BWDataType.typeDataImageNBGamma) continue;
                DisplayProperty dp = XYPlotSettings.GetDisplaySetting(DatasetList[i].Name);
                if (dp.Display == false) continue;

                int curveWidth = (int)dp.Width;
                Pen penT = new Pen(dp.Color, curveWidth);
                Point p1T = new Point(rectLegend.Left, yPixel);
                Point p2T = new Point(rectLegend.Right, yPixel);
                DrawLine(Graphics, penT, p1T, p2T);
                yPixel -= fontSize * 6 / 10;
                Point pT = new Point((rectLegend.Left + rectLegend.Right) / 2, yPixel);
                Brush brushT = new SolidBrush(dp.Color);
                DrawString(Graphics, DatasetList[i].Name, XYPlotSettings.YAxis.TickFont, brushT, pT, false, StringAlignment.Center, 4);

                double minValue = BWUtility.FormatFloatingNumber(XYPlotSettings.XAxis.ViewMin, 2);
                double maxValue = BWUtility.FormatFloatingNumber(XYPlotSettings.XAxis.ViewMax, 2);
                String leftText = XYPlotSettings.XAxis.ReversedScale ? maxValue.ToString() : minValue.ToString();
                String riteText = XYPlotSettings.XAxis.ReversedScale ? minValue.ToString() : maxValue.ToString();
                //if (dp.TempAxisOverWrite == true && dp.TempAxisDisplayScale == true)
                if (dp.TempAxisOverWrite == true)
                {
                    minValue = BWUtility.FormatFloatingNumber(dp.TempAxisViewMin, 2);
                    maxValue = BWUtility.FormatFloatingNumber(dp.TempAxisViewMax, 2);
                    leftText = dp.TempAxisReversedScale ? maxValue.ToString() : minValue.ToString();
                    riteText = dp.TempAxisReversedScale ? minValue.ToString() : maxValue.ToString();
                }

                pT = new Point(rectLegend.Left + fontSize * 2, yPixel);
                //Graphics.DrawString(leftText, XYPlotSettings.XAxis.TickFont, brushXUnit, pT);
                //Graphics.DrawString(riteText, XYPlotSettings.XAxis.TickFont, brushXUnit, pT);
                DrawString(Graphics, leftText, XYPlotSettings.YAxis.TickFont, brushT, pT, false, StringAlignment.Center, 4);
                pT = new Point(rectLegend.Right - fontSize * 2, yPixel);
                DrawString(Graphics, riteText, XYPlotSettings.XAxis.TickFont, brushT, pT, false, StringAlignment.Center, 4);

                yPixel += fontSize * 27 / 10;
                penT.Dispose();
                brushT.Dispose();
            }
        }

        protected int XValueToPixel(double value)
        {
            return XValueToPixel(value, XYPlotSettings.XAxis);
        }

        protected int YValueToPixel(double value)
        {
            return YValueToPixel(value, XYPlotSettings.YAxis);
        }

        protected int XValueToPixel(double value, Axis axis)
        {
            double viewMin = axis.ViewMin;
            double viewMax = axis.ViewMax;
            Boolean isLog = axis.IsLog;
            bool isReversedScale = axis.ReversedScale;

            // viewMin                     value       viewMax
            // rectPlotArea.Left           pixel       rectPlotArea.Right
            const double tolerance = 1.0e-8;
            if (0.5 * (Math.Abs(viewMin) + Math.Abs(viewMax)) < tolerance)
            {
                viewMax = viewMin + tolerance;
            }
            else if (Utility.RelativeDifference(viewMin, viewMax) < tolerance)
            {
                viewMax = viewMin + (0.5 * (Math.Abs(viewMin) + Math.Abs(viewMax))) * tolerance;
            }

            double pixel = 0;
            if (isLog == true)
            {
                if (value <= tolerance || viewMin <= tolerance || viewMax <= tolerance)
                {
                    return -1;
                }

                value = Math.Log10(value);
                viewMin = Math.Log10(viewMin);
                viewMax = Math.Log10(viewMax);
            }

            pixel = rectPlotArea.Left + ((value - viewMin) /
                (viewMax - viewMin) * (rectPlotArea.Right - rectPlotArea.Left));

            if (isReversedScale == true) pixel = rectPlotArea.Right - (pixel - rectPlotArea.Left);

            return (int)pixel;
        }

        protected int YValueToPixel(double value, Axis axis)
        {
            double viewMin = axis.ViewMin;
            double viewMax = axis.ViewMax;
            Boolean isLog = axis.IsLog;
            bool isReversedScale = axis.ReversedScale;

            // viewMin                     value       viewMax
            // rectPlotArea.Bottom         pixel       rectPlotArea.Top
            const double tolerance = 1.0e-6;
            if (0.5 * (Math.Abs(viewMin) + Math.Abs(viewMax)) < tolerance)
            {
                viewMax = viewMin + tolerance;
            }
            else if (Utility.RelativeDifference(viewMin, viewMax) < tolerance)
            {
                viewMax = viewMin + (0.5 * (Math.Abs(viewMin) + Math.Abs(viewMax))) * tolerance;
            }

            double pixel = 0;
            if (isLog == true)
            {
                if (value <= tolerance || viewMin <= tolerance || viewMax <= tolerance)
                {
                    return -1;
                }

                value = Math.Log10(value);
                viewMin = Math.Log10(viewMin);
                viewMax = Math.Log10(viewMax);
            }

            pixel = rectPlotArea.Bottom + ((value - viewMin) /
                (viewMax - viewMin) * (rectPlotArea.Top - rectPlotArea.Bottom));

            if (isReversedScale == true) pixel = rectPlotArea.Bottom - (pixel - rectPlotArea.Top);

            return (int)pixel;
        }

        protected void DrawSymbol(Point center, DisplayProperty dp)
        {
            // draw symbol
            String symbol = dp.Symbol;
            int symbolSize = dp.SymbolSize;
            Color symbolColor = dp.SymbolColor;
            Pen symbolPen = new Pen(symbolColor);
            Point location = new Point(center.X - symbolSize / 2, center.Y - symbolSize / 2);
            Rectangle symbolRectangle = new Rectangle(location, new Size(symbolSize, symbolSize));
            //int xMid = (symbolRectangle.Left + symbolRectangle.Right) / 2;
            //int yMid = (symbolRectangle.Top + symbolRectangle.Bottom) / 2;
            int xMid = center.X;
            int yMid = center.Y;

            if (symbol == "PlusSign")
            {
                Point hLeft = new Point(symbolRectangle.Left, yMid);
                Point hRite = new Point(symbolRectangle.Right, yMid);
                Point vTop = new Point(xMid, symbolRectangle.Top);
                Point vBtm = new Point(xMid, symbolRectangle.Bottom);
                DrawLine(Graphics, symbolPen, hLeft, hRite);
                DrawLine(Graphics, symbolPen, vTop, vBtm);
            }
            else if (symbol == "Cross")
            {
                Point topLeft = new Point(symbolRectangle.Left, symbolRectangle.Top);
                Point topRite = new Point(symbolRectangle.Right, symbolRectangle.Top);
                Point btmLeft = new Point(symbolRectangle.Left, symbolRectangle.Bottom);
                Point btmRite = new Point(symbolRectangle.Right, symbolRectangle.Bottom);
                DrawLine(Graphics, symbolPen, topLeft, btmRite);
                DrawLine(Graphics, symbolPen, topRite, btmLeft);
            }
            else if (symbol == "Circle")
            {
                //Graphics.DrawEllipse(symbolPen, symbolRectangle);
                XYPlot.DrawEllipse(Graphics, symbolPen, symbolRectangle);
            }
            else if (symbol == "Square")
            {
                //Graphics.DrawRectangle(symbolPen, symbolRectangle);
                XYPlot.DrawRectangle(Graphics, symbolPen, symbolRectangle);
            }
            else if (symbol == "Diamond")
            {
                Point top = new Point(xMid, symbolRectangle.Top);
                Point left = new Point(symbolRectangle.Left, yMid);
                Point rite = new Point(symbolRectangle.Right, yMid);
                Point btm = new Point(xMid, symbolRectangle.Bottom);
                Graphics.DrawPolygon(symbolPen, new Point[] { top, left, btm, rite });
            }
            else if (symbol == "Triangle")
            {
                Point top = new Point(xMid, symbolRectangle.Top);
                Point left = new Point(symbolRectangle.Left, symbolRectangle.Bottom);
                Point rite = new Point(symbolRectangle.Right, symbolRectangle.Bottom);
                Graphics.DrawPolygon(symbolPen, new Point[] { top, left, rite });
            }

            symbolPen.Dispose();
        }

        //// plot: 
        ////  0:  Vertical plot
        ////  1:  Map Plot
        //protected void DrawTargetSymbol(Point center, BWTarget target, int symbolSize, int plot)
        //{
        //    if (center.X > 10000 || center.X < -10000 ||
        //        center.Y > 10000 || center.Y < -10000) return;

        //    // draw symbol
        //    if (symbolSize <= 4) symbolSize = 12;

        //    // color
        //    Color colorBigCircle = Color.Blue;
        //    Color colorSmallCircle = Color.Blue;
        //    Color colorTriangle = Color.Blue;
        //    Color colorPlus = Color.Red;
        //    if (target.TargetData.Active == false)
        //    {
        //        colorBigCircle = Color.Gray;
        //        colorSmallCircle = Color.Gray;
        //        colorTriangle = Color.Gray;
        //        colorPlus = Color.Gray;
        //    }

        //    // width
        //    bool selected = target.Selected;
        //    float widthBigCircle = selected ? 3 : 1;

        //    // pen
        //    Pen penBigCircle = new Pen(colorBigCircle, widthBigCircle);
        //    Pen penSmallCircle = new Pen(colorSmallCircle);
        //    Pen penTriangle = new Pen(colorTriangle);
        //    Pen penPlus = new Pen(colorPlus);

        //    Point location = new Point(center.X - symbolSize / 2, center.Y - symbolSize / 2);
        //    Rectangle symbolRectangle = new Rectangle(location, new Size(symbolSize, symbolSize));
        //    //int xMid = (symbolRectangle.Left + symbolRectangle.Right) / 2;
        //    //int yMid = (symbolRectangle.Top + symbolRectangle.Bottom) / 2;
        //    int xMid = center.X;
        //    int yMid = center.Y;
        //    Point smallLocation = new Point(center.X - symbolSize / 4, center.Y - symbolSize / 4);
        //    Rectangle smallRectangle = new Rectangle(smallLocation, new Size(symbolSize/2, symbolSize/2));

        //    Point hLeft = new Point(symbolRectangle.Left, yMid);
        //    Point hRite = new Point(symbolRectangle.Right, yMid);
        //    Point vTop = new Point(xMid, symbolRectangle.Top);
        //    Point vBtm = new Point(xMid, symbolRectangle.Bottom);
        //    DrawLine(Graphics, penPlus, hLeft, hRite);
        //    DrawLine(Graphics, penPlus, vTop, vBtm);
        //    //Graphics.DrawEllipse(penBigCircle, symbolRectangle);
        //    XYPlot.DrawEllipse(Graphics, penBigCircle, symbolRectangle);

        //    // hit?
        //    //bool hit = (target.Distance < 10e-4);
        //    if (target.Hit)
        //    {
        //        //Graphics.DrawEllipse(penSmallCircle, smallRectangle);
        //        XYPlot.DrawEllipse(Graphics, penSmallCircle, smallRectangle);
        //    }

        //    // past?
        //    bool past = !(target.Future);
        //    if (past)
        //    {
        //        Point top = new Point(xMid, symbolRectangle.Top);
        //        Point left = new Point(smallRectangle.Left, smallRectangle.Bottom);
        //        Point rite = new Point(smallRectangle.Right, smallRectangle.Bottom);
        //        Graphics.DrawPolygon(penTriangle, new Point[] { top, left, rite });
        //    }

        //    double inc = target.TargetData.Inclination;
        //    double azi = target.TargetData.Azimuth;
        //    if (plot == 0)
        //    {   // Vertical plot
        //        // method 1
        //        Point p2 = Utility.CalcP2Inc(center, inc, RatioOfPixelPerUnit());
        //        // method 2
        //        //Location P2 = Utility.CalcP2IncH(target.TargetData.TargetLocation,
        //        //    inc, azi, 50);
        //        //Point p2 = new Point();
        //        //p2.X = 0;

        //        Pen penInc = new Pen(colorPlus);
        //        penPlus.EndCap = LineCap.ArrowAnchor;
        //        DrawLine(Graphics, penPlus, center, p2);
        //    }
        //    else if (plot == 1)
        //    {   // Map plot
        //        Point p2 = Utility.CalcP2Azi(center, azi);
        //        Pen penInc = new Pen(colorPlus);
        //        penPlus.EndCap = LineCap.ArrowAnchor;
        //        DrawLine(Graphics, penPlus, center, p2);
        //    }
        //}

        // find the first and the last indices of the data that in the view window
        protected void FindStartEndIndices(int nTotalRecords, double[] dData,
            double xViewMin, double xViewMax, double yViewMin, double yViewMax,
            ref int startIndex, ref int endIndex)
        {
            // find startIndex1 and endIndex1
            startIndex = -1;
            endIndex = nTotalRecords;
            for (int j = 0; j < nTotalRecords - 1; j++)
            {
                double x1 = dData[2 * j];
                double y1 = dData[2 * j + 1];
                double x2 = dData[2 * (j + 1)];
                double y2 = dData[2 * (j + 1) + 1];
                if (Utility.LineSegmentIntersectsRectangle(x1, y1, x2, y2, xViewMin, xViewMax,
                    yViewMin, yViewMax) == true)
                {
                    startIndex = j;
                    break;
                }
            }
            for (int j = nTotalRecords - 1; j > 0; j--)
            {
                double x1 = dData[2 * j];
                double y1 = dData[2 * j + 1];
                double x2 = dData[2 * (j - 1)];
                double y2 = dData[2 * (j - 1) + 1];
                if (Utility.LineSegmentIntersectsRectangle(x1, y1, x2, y2, xViewMin, xViewMax,
                    yViewMin, yViewMax) == true)
                {
                    endIndex = j;
                    break;
                }
            }
        }

        // find the first and the last indices of the data that in the view window
        // multiple column data
        protected void FindStartEndIndices(int nTotalRecords, double[] dData,
            int nTotalColumns, int xColIndex, int yColIndex,
            double xViewMin, double xViewMax, double yViewMin, double yViewMax,
            ref int startIndex, ref int endIndex)
        {
            // find startIndex1 and endIndex1
            startIndex = -1;
            endIndex = nTotalRecords;
            int j = 0;
            try
            {
                for (j = 0; j < nTotalRecords - 1; j++)
                {
                    double x1 = dData[nTotalColumns * j + xColIndex];
                    double y1 = dData[nTotalColumns * j + yColIndex];
                    double x2 = dData[nTotalColumns * (j + 1) + xColIndex];
                    double y2 = dData[nTotalColumns * (j + 1) + yColIndex];
                    if (Utility.LineSegmentIntersectsRectangle(x1, y1, x2, y2, xViewMin, xViewMax,
                        yViewMin, yViewMax) == true)
                    {
                        startIndex = j;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                int i = j;
            }

            try
            {
                for (j = nTotalRecords - 1; j > 0; j--)
                {
                    double x1 = dData[nTotalColumns * j + xColIndex];
                    double y1 = dData[nTotalColumns * j + yColIndex];
                    double x2 = dData[nTotalColumns * (j - 1) + xColIndex];
                    double y2 = dData[nTotalColumns * (j - 1) + yColIndex];
                    if (Utility.LineSegmentIntersectsRectangle(x1, y1, x2, y2, xViewMin, xViewMax,
                        yViewMin, yViewMax) == true)
                    {
                        endIndex = j;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                int i = j;
            }
        }

        // private methods
        private double PixelToXValue(int pixel)
        {
            return PixelToXValue(pixel, XYPlotSettings.XAxis);
        }

        private double PixelToYValue(int pixel)
        {
            return PixelToYValue(pixel, XYPlotSettings.YAxis);
        }

        private double PixelToXValue(int pixel, Axis axis)
        {
            double viewMin = axis.ViewMin;
            double viewMax = axis.ViewMax;
            Boolean isLog = axis.IsLog;
            bool isReversedScale = axis.ReversedScale;
            if (isReversedScale == true) pixel = rectPlotArea.Right - (pixel - rectPlotArea.Left);

            // viewMin                     value       viewMax
            // rectPlotArea.Left           pixel       rectPlotArea.Right
            const double tolerance = 1.0e-6;
            if (Utility.RelativeDifference(viewMin, viewMax) < tolerance)
            {
                viewMax = viewMin + (0.5 * (Math.Abs(viewMin) + Math.Abs(viewMax))) * tolerance;
            }

            if (isLog == true)
            {
                if (viewMin <= tolerance || viewMax <= tolerance)
                {
                    return -1;
                }

                viewMin = Math.Log10(viewMin);
                viewMax = Math.Log10(viewMax);
            }

            double value = viewMin + ((double)(pixel - rectPlotArea.Left) /
                (rectPlotArea.Right - rectPlotArea.Left) * (viewMax - viewMin));

            if (isLog == true)
            {
                value = Math.Pow(10, value);
            }

            return value;
        }

        private double PixelToYValue(int pixel, Axis axis)
        {
            double viewMin = axis.ViewMin;
            double viewMax = axis.ViewMax;
            Boolean isLog = axis.IsLog;
            bool isReversedScale = axis.ReversedScale;
            if (isReversedScale == true) pixel = rectPlotArea.Bottom - (pixel - rectPlotArea.Top);

            // viewMin                     value       viewMax
            // rectPlotArea.Bottom         pixel       rectPlotArea.Top
            const double tolerance = 1.0e-6;
            if (Utility.RelativeDifference(viewMin, viewMax) < tolerance)
            {
                viewMax = viewMin + (0.5 * (Math.Abs(viewMin) + Math.Abs(viewMax))) * tolerance;
            }

            if (isLog == true)
            {
                if (viewMin <= tolerance || viewMax <= tolerance)
                {
                    return -1;
                }

                viewMin = Math.Log10(viewMin);
                viewMax = Math.Log10(viewMax);
            }

            double value = viewMin + ((double)(pixel - rectPlotArea.Bottom) /
                (rectPlotArea.Top - rectPlotArea.Bottom) * (viewMax - viewMin));

            if (isLog == true)
            {
                value = Math.Pow(10, value);
            }

            return value;
        }

        /// <summary>
        /// It calculates # of pixels per unit for both x and y axes, returns
        /// the ratio of x# to y#.
        /// </summary>
        /// <returns>
        /// xPixelPerUnit = width/(xMax-xMin);
        /// yPixelPerUnit = height/(yMax-yMin);
        /// ratio = xPixelPerUnit / yPixelPerUnit;
        /// </returns>
        public double RatioOfPixelPerUnit()
        {
            double ratio = 1.0;

            double xViewMin = XYPlotSettings.XAxis.ViewMin;
            double xViewMax = XYPlotSettings.XAxis.ViewMax;
            double yViewMin = XYPlotSettings.YAxis.ViewMin;
            double yViewMax = XYPlotSettings.YAxis.ViewMax;
            const double tolerance = 1.0e-6;
            if (Math.Abs(xViewMax - xViewMin) > tolerance &&
                Math.Abs(yViewMax - yViewMin) > tolerance)
            {
                double xPixelPerUnit = rectPlotArea.Width / (xViewMax - xViewMin);
                double yPixelPerUnit = rectPlotArea.Height / (yViewMax - yViewMin); ;
                if (xPixelPerUnit > tolerance) ratio = xPixelPerUnit / yPixelPerUnit;
            }

            return ratio;
        }

        public static List<double> CalculateMajorGrids(Axis axis)
        {
            int totalTicks = axis.TotalTicks;
            double min = axis.ViewMin;
            double max = axis.ViewMax;
            bool isLog = axis.IsLog;

            List<double> majorGrids = new List<double>(totalTicks);

            double first = 0.0;
            double interval = 10.0;
            if (isLog == false)
            {
                double diff = max - min;
                interval = diff / totalTicks;
                interval = Utility.FormatFloatingNumber(interval);

                int n = (int)Math.Ceiling(min / interval);
                first = n * interval;

                double tick = first;
                while (tick < max)
                {
                    if (tick >= min && tick <= max) majorGrids.Add(tick);
                    tick += interval;
                }
            }
            else
            {
                first = .1;
                double tick = first;
                while (tick < max)
                {
                    if (tick >= min && tick <= max) majorGrids.Add(tick);
                    tick *= 10;
                }

                // some case like min = 2, max = 6, etc
                if (majorGrids.Count == 0)
                {
                    tick = Utility.FormatFloatingNumber(0.5 * (min + max));
                    majorGrids.Add(tick);
                }
            }

            return majorGrids;
        }

        // Draw a text string with given font and brush in a rectangle.
        // Rectangle position is determined by the point and pointPosition;
        // Rectangle size will be calculated according to the length of string and font.
        // The text string can be drawn either horizontal or vertical, 
        // with given alignment in the rectangle.
        // The rectangle will be returned
        public static Rectangle DrawString(Graphics g, String text, Font font, Brush brush, Point point,
            bool isVertical, StringAlignment alignment, int pointPosition)
        {
            StringFormat format = null;
            if (isVertical == false) format = new StringFormat();
            else format = new StringFormat(StringFormatFlags.DirectionVertical);
            format.Alignment = alignment;

            Font printingFont = new Font(font.FontFamily, font.Size / PrintingFontSizeRatio);
            Size size = g.MeasureString(text, font, 100, format).ToSize();
            if (IsPrinting == true)
            {
                size = g.MeasureString(text, printingFont, 100, format).ToSize();
            }

            // size adjustment
            if (isVertical)
            {
                size.Height += 1;
            }
            else
            {
                size.Width += 1;
            }

            Point location = point;
            switch (pointPosition)
            {
                case 0: // top left
                    break;
                case 1: // top middle
                    location.X -= size.Width / 2;
                    break;
                case 2: // top right
                    location.X -= size.Width;
                    break;
                case 3: // left middle
                    location.Y -= size.Height / 2;
                    break;
                case 4: // center
                    location.X -= size.Width / 2;
                    location.Y -= size.Height / 2;
                    break;
                case 5: // right middle
                    location.X -= size.Width;
                    location.Y -= size.Height / 2;
                    break;
                case 6: // bottom left
                    location.Y -= size.Height;
                    break;
                case 7: // bottom middle
                    location.X -= size.Width / 2;
                    location.Y -= size.Height;
                    break;
                case 8: // bottom right
                    location.X -= size.Width;
                    location.Y -= size.Height;
                    break;
                default: // top left
                    break;
            }
            Rectangle rect = new Rectangle(location, size);
            if (IsValidRectangle(rect) == false) return rect;

            // make vertical text in another direction
            Matrix matrixOriginal = g.Transform;
            Matrix matrixNew = matrixOriginal.Clone();
            if (isVertical == true)
            {
                matrixNew.RotateAt(180, rect.Location);
                matrixNew.Translate(-rect.Width, -rect.Height);
                g.Transform = matrixNew;
            }

            if (IsPrinting == true)
            {
                g.DrawString(text, printingFont, brush, rect, format);
            }
            else
            {
                g.DrawString(text, font, brush, rect, format);
            }

            // reset matrix to original
            if (isVertical == true)
            {
                g.Transform = matrixOriginal;
            }
            //Pen pen = new Pen(brush);
            //g.DrawRectangle(pen, rect);

            return rect;
        }

        public static void DrawLine(Graphics g, Pen pen, Point p1, Point p2)
        {
            if (g == null ||
                pen == null ||
                IsValidPoint(p1) == false ||
                IsValidPoint(p2) == false) return;

            try
            {
                g.DrawLine(pen, p1, p2);
            }
            catch
            {
            }
        }

        public static void DrawLines(Graphics g, Pen pen, Point[] ptArray)
        {
            if (g == null ||
                pen == null ||
                ptArray.Length <= 1) return;

            try
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLines(pen, ptArray);
            }
            catch
            {
            }
        }

        //public static void DrawLines(Graphics g, Pen pen, Point[] ptArray, bool connectAbove, bool isReversed)
        public static void DrawLines(Graphics g, Pen pen, Point[] ptArray, BWInterpolateFlag flag, bool isXYSwitched)
        {
            if (g == null ||
                pen == null ||
                ptArray.Length <= 1) return;

            try
            {
                for (int i = 1; i < ptArray.Length; i++)
                {
                    DrawStepLines(g, pen, ptArray[i - 1], ptArray[i], flag, isXYSwitched);
                }
            }
            catch
            {
            }
        }

        //draw square lines
        private static void DrawStepLines(Graphics g, Pen pen, Point p1, Point p2, BWInterpolateFlag flag, bool isXYSwitched)
        {
            if (g == null ||
                pen == null) return;

            try
            {
                float xm, ym;
                if (!isXYSwitched)
                {
                    if (flag == BWInterpolateFlag.flagInterpolateUseStartValue)
                    {
                        xm = p2.X;
                        ym = p1.Y;
                    }
                    else if (flag == BWInterpolateFlag.flagInterpolateUseEndValue)
                    {
                        xm = p1.X;
                        ym = p2.Y;
                    }
                    else return;
                }
                else
                {
                    if (flag == BWInterpolateFlag.flagInterpolateUseStartValue)
                    {
                        xm = p1.X;
                        ym = p2.Y;
                    }
                    else if (flag == BWInterpolateFlag.flagInterpolateUseEndValue)
                    {
                        xm = p2.X;
                        ym = p1.Y;
                    }
                    else return;
                }
                g.DrawLine(pen, p1.X, p1.Y, xm, ym);
                g.DrawLine(pen, xm, ym, p2.X, p2.Y);
            }
            catch
            {
            }
        }

        public static void DrawRectangle(Graphics g, Pen pen, Rectangle rect)
        {
            if (g == null ||
                pen == null ||
                IsValidRectangle(rect) == false) return;

            try
            {
                g.DrawRectangle(pen, rect);
            }
            catch
            {
            }
        }

        public static void DrawEllipse(Graphics g, Pen pen, Rectangle rect)
        {
            if (g == null ||
                pen == null ||
                IsValidRectangle(rect) == false) return;

            try
            {
                g.DrawEllipse(pen, rect);
            }
            catch
            {
            }
        }

        public static void DrawArc(Graphics g, Pen pen, Rectangle rect, double startAngle, double sweepAngle)
        {
            if (g == null ||
                pen == null ||
                IsValidRectangle(rect) == false) return;

            try
            {
                g.DrawArc(pen, rect, (float)startAngle, (float)sweepAngle);
            }
            catch
            {
            }
        }

        public static void FillRectangle(Graphics g, Brush brush, Rectangle rect)
        {
            if (g == null ||
                brush == null ||
                IsValidRectangle(rect) == false) return;

            try
            {
                g.FillRectangle(brush, rect);
            }
            catch
            {
            }
        }
        public static void FillRectangle(Graphics g, Brush brush, RectangleF rect)
        {
            if (g == null ||
                brush == null ||
                IsValidRectangle(rect) == false) return;

            try
            {
                g.FillRectangle(brush, rect);
            }
            catch
            {
            }
        }

        public static void FillPolygon(Graphics g, Brush brush, Point[] points)
        {
            if (g == null ||
                brush == null) return;
            foreach (Point pt in points) if (IsValidPoint(pt) == false) return;

            try
            {
                g.FillPolygon(brush, points, FillMode.Alternate);
            }
            catch
            {
            }
        }

        protected static bool IsValidPoint(PointF p)
        {
            if (Math.Abs(p.X) < 1.0e8 && Math.Abs(p.Y) < 1.0e8) return true;
            else return false;
        }
        protected static bool IsValidRectangle(RectangleF rect)
        {
            if (rect != Rectangle.Empty &&
                IsValidPoint(rect.Location) &&
                rect.Size.Width > 0 &&
                rect.Size.Height > 0)
            {
                return true;
            }

            return false;
        }

        //// used for Rvd plots actual well curve display
        //protected static bool IsActualWellDataset(BWDataset dataset)
        //{
        //    if (dataset == null) return false;
        //    if (dataset.Name.EndsWith("_A") == true) return true;
        //    else return false;
        //}

        public double GetXValue(Point point)
        {
            if (point == Point.Empty) return BWDataset.NO_READING;

            return PixelToXValue(point.X);
        }
        public double GetYValue(Point point)
        {
            if (point == Point.Empty) return BWDataset.NO_READING;

            return PixelToYValue(point.Y);
        }
        public int GetXPixel(double value)
        {
            return XValueToPixel(value);
        }
        public int GetYPixel(double value)
        {
            return YValueToPixel(value);
        }

        public void DrawReversibleRectangle(Point p1, Point p2)
        {
            if (p1 == p2) return;
            p1 = this.ParentControl.PointToScreen(p1);
            p2 = this.ParentControl.PointToScreen(p2);
            int left = Math.Min(p1.X, p2.X);
            int right = Math.Max(p1.X, p2.X);
            int top = Math.Min(p1.Y, p2.Y);
            int bottom = Math.Max(p1.Y, p2.Y);
            Rectangle rect = new Rectangle(left, top, right - left, bottom - top);
            Color color = Color.LightGreen;
            //color.A = 0.5;
            //ControlPaint.DrawReversibleFrame(rect, color, FrameStyle.Dashed);
            //ControlPaint.FillReversibleRectangle(rect, Color.Transparent);
            ControlPaint.FillReversibleRectangle(rect, color);
        }
    }

    [Serializable]
    public class XYPlotMudPulse : XYPlot
    {
        // constructors
        public XYPlotMudPulse(Graphics g, Control parentControl, Rectangle rect)
            : base(g, parentControl, rect)
        {
            PlotType = PlotType.XYPlotTimeSignal;
            XYPlotSettings.XAxis.Name = "Time";
            //ZoomX = true;
            //ZoomY = false;
        }

        // public methods
        public override void Draw()
        {
            //if (Graphics != null) Graphics.Dispose();
            if (ParentControl != null) Graphics = ParentControl.CreateGraphics();

            CalculateClientRectangle();
            if (Graphics == null) return;
            Graphics.SetClip(rectClientArea);
            Graphics.Clear(XYPlotSettings.BackColor);

            DrawGrid();
            DrawAxis();
            Graphics.SetClip(rectPlotArea);
            DrawCurve();
        }

        // Curve specific
        public override void DrawCurve()
        {
            for (int i = 0; i < DatasetList.Count; i++)
            {
                BWDataset dataset = DatasetList[i];
                DrawRegularCurve(dataset);
            }
        }

        protected override void DrawRegularCurve(BWDataset dataset)
        {
            if (dataset == null || dataset.TotalRecords == 0) return;

            int nTotalRecords = 0;
            double[] dData = null;
            dataset.GetData(out nTotalRecords, out dData);
            DisplayProperty dp = XYPlotSettings.GetDisplaySetting(dataset.Name);
            if (dp.Display == false) return;
            //Pen penCurve = new Pen(dp.Color, dp.Width);
            Axis xAxis = XYPlotSettings.XAxis;
            Axis yAxis = XYPlotSettings.YAxis;

            // check if overwrite primary y axis
            if (dp.TempAxisOverWrite == true)
            {
                yAxis = new Axis();
                yAxis.IsLog = dp.TempAxisIsLog;
                yAxis.ViewMin = dp.TempAxisViewMin;
                yAxis.ViewMax = dp.TempAxisViewMax;
                yAxis.ReversedScale = dp.TempAxisReversedScale;
            }

            if (xAxis.IsLog && (xAxis.ViewMin <= 0 || xAxis.ViewMax <= 0)) return;
            if (yAxis.IsLog && (yAxis.ViewMin <= 0 || yAxis.ViewMax <= 0)) return;

            // find startIndex1 and endIndex1
            int startIndex = -1;
            int endIndex = nTotalRecords;
            int nTotalColumns = dataset.TotalColumns;
            FindStartEndIndices(nTotalRecords, dData, nTotalColumns, 0, 1, xAxis.ViewMin,
                xAxis.ViewMax, yAxis.ViewMin, yAxis.ViewMax, ref startIndex, ref endIndex);

            if (startIndex >= 0 && endIndex < nTotalRecords)
            {
                if (startIndex > 0) startIndex--;
                if (endIndex < nTotalRecords - 1) endIndex++;
                int count = endIndex - startIndex + 1;
                if (count > 1)
                {
                    List<Point> pointList = new List<Point>();
                    // stop and draw as
                    // 1. step > DataMissingThredshold
                    bool isDown = true;     // natural order or first order
                    for (int j = startIndex; j <= endIndex; j++)
                    {
                        double x = dData[nTotalColumns * j];
                        //if (dataset.Name == "Filtered Data") x += 4.8;
                        double y = dData[nTotalColumns * j + 1];

                        if (j > startIndex)
                        {
                            double xLast = dData[nTotalColumns * (j - 1)];
                            bool isDataMissing = (Math.Abs(xLast - x) > XYPlotSettings.DataMissingThredshold);
                            if (isDataMissing)
                            {   // draw points and lines
                                drawPointsLines(pointList, dp, isDown, dataset.InterpolateFlag);
                                pointList.Clear();
                            }
                        }

                        int pixelX = XValueToPixel(x, xAxis);
                        int pixelY = YValueToPixel(y, yAxis);
                        Point pt = new Point(pixelX, pixelY);
                        pointList.Add(pt);
                    }

                    drawPointsLines(pointList, dp, isDown, dataset.InterpolateFlag);
                }
            }
        }
    }

    /// <summary>
    /// a plot used to compare pump on/off time between realtime and memory, 
    /// its main purpose is to correct memory time
    /// DatasetList contains 3 datasets:
    /// 1. pumpDSRealtime,  "Realtime Pump"     "Pump", "StartTime", "StopTime", "Duration"
    /// 2. pumpDSMemory,    "Memory Pump"       "Pump", "StartTime", "StopTime", "Duration"
    /// 3. pumpDSTimeDepth, "TimeDepth Pump"    "Pump", "StartTime", "StopTime", "Duration", "StartDepth", "EndDepth", "Length"
    /// </summary>

    public enum PumpDSColumn
    {
        Pump = 0,
        StartTime = 1,
        StopTime = 2,
        Duration = 3,
        StartDepth = 4,
        EndDepth = 5,
        Length = 6,
    }

    [Serializable]
    public class XYPlotTimeDepth : XYPlot
    {
        // don't display pumpons if duration < DurationDisplayLimit (seconds)
        public int DurationDisplayLimit;

        // constructors
        public XYPlotTimeDepth(Graphics g, Control parentControl, Rectangle rect)
            : base(g, parentControl, rect)
        {
            PlotType = PlotType.XYPlotTimeDepth;
            XYPlotSettings.Landscape = true;
            XYPlotSettings.XAxis.Name = "Time";
            //ZoomX = true;
            //ZoomY = false;

            DurationDisplayLimit = 0;
        }

        // public methods
        public override void Draw()
        {
            //if (Graphics != null) Graphics.Dispose();
            if (ParentControl != null) Graphics = ParentControl.CreateGraphics();

            CalculateClientRectangle();
            if (Graphics == null) return;
            Graphics.SetClip(rectClientArea);
            Graphics.Clear(XYPlotSettings.BackColor);
            //drawDayNight();
            DrawGrid();
            DrawAxis();
            Graphics.SetClip(rectPlotArea);
            DrawCurve();
        }

        // X axis time display as DateTime
        protected override void DrawXGrid()
        {
            Pen penMajorGrid = new Pen(XYPlotSettings.MajorGridColor, XYPlotSettings.MajorGridWidth);
            penMajorGrid.Alignment = PenAlignment.Center;
            Pen penMinorGrid = new Pen(XYPlotSettings.MinorGridColor, XYPlotSettings.MinorGridWidth);
            //Graphics.DrawRectangle(penMinorGrid, rectClientArea);
            XYPlot.DrawRectangle(Graphics, penMinorGrid, rectClientArea);
            //XYPlotSettings.XAxis.IsTickVertical = false;
            // vertical major grid
            int totalTicks = XYPlotSettings.XAxis.TotalTicks;
            double min = XYPlotSettings.XAxis.ViewMin;
            double max = XYPlotSettings.XAxis.ViewMax;
            Color color = XYPlotSettings.XAxis.TickColor;
            Font font = XYPlotSettings.XAxis.TickFont;
            List<double> majorGrids = CalculateTimeMajorGrids(XYPlotSettings.XAxis);
            totalTicks = majorGrids.Count;

            if (XYPlotSettings.ShowMinorGrid)
            {
                DrawMinorGrids(XYPlotSettings.XAxis, majorGrids, penMinorGrid, true);
            }

            for (int i = 0; i < totalTicks; i++)
            {
                double dXTick = majorGrids[i];
                int pixelXTick = XValueToPixel(dXTick);
                Point p1 = new Point(pixelXTick, rectPlotArea.Bottom);
                Point p2 = new Point(pixelXTick, rectPlotArea.Top);

                if (XYPlotSettings.ShowMajorGrid)
                {
                    DrawLine(Graphics, penMajorGrid, p1, p2);
                }

                if (XYPlotSettings.XAxis.EnableTick)
                {
                    Brush brushXMark = new SolidBrush(color);
                    String xTick = BWConverter.SecondsToLongTime(dXTick);
                    bool bVertical = XYPlotSettings.XAxis.IsTickVertical;
                    DrawString(Graphics, xTick, font, brushXMark, p1, bVertical, StringAlignment.Center, 1);
                }
            }

            penMajorGrid.Dispose();
            penMinorGrid.Dispose();
        }

        public override void FullView()
        {
            double viewMinX = -1;
            double viewMaxX = -1;
            double viewMinY = -1;
            double viewMaxY = -1;

            double dummy = -1;
            int startTimeColIndex = 1;
            int stopTimeColIndex = 2;
            int startDepthColIndex = 4;
            int endDepthColIndex = 5;
            getMinMaxValue(startTimeColIndex, out viewMinX, out dummy);
            getMinMaxValue(stopTimeColIndex, out dummy, out viewMaxX);
            getMinMaxValue(startDepthColIndex, out viewMinY, out dummy);
            getMinMaxValue(endDepthColIndex, out dummy, out viewMaxY);

            if (viewMinX == -1 && viewMaxX == -1)
            {
                viewMinX = XYPlotSettings.XAxis.IsLog ? 0.2 : 0;
                viewMaxX = XYPlotSettings.XAxis.IsLog ? 200 : 100;
            }
            if (viewMinY == -1 && viewMaxY == -1)
            {
                viewMinY = XYPlotSettings.YAxis.IsLog ? 0.2 : 0;
                viewMaxY = XYPlotSettings.YAxis.IsLog ? 200 : 100;
            }

            int Z = 30;
            viewMinX = viewMinX - (viewMaxX - viewMinX) / Z;
            viewMaxX = viewMaxX + (viewMaxX - viewMinX) / Z;
            viewMinY = viewMinY - (viewMaxY - viewMinY) / Z;
            viewMaxY = viewMaxY + (viewMaxY - viewMinY) / Z;

            if ((viewMinX != -1 || viewMaxX != -1) && ZoomX == true)
            {
                if (viewMinX < viewMaxX)
                {
                    XYPlotSettings.XAxis.ViewMin = viewMinX;
                    XYPlotSettings.XAxis.ViewMax = viewMaxX;
                }
                else if (viewMinX == viewMaxX)
                {
                    XYPlotSettings.XAxis.ViewMin = viewMinX - 50;
                    XYPlotSettings.XAxis.ViewMax = viewMaxX + 50;
                }

                OnChangeViewX();
            }
            //if ((viewMinY != -1 || viewMaxY != -1) && ZoomY == true)
            if ((viewMinY != -1 || viewMaxY != -1))
            {
                if (viewMinY < viewMaxY)
                {
                    XYPlotSettings.YAxis.ViewMin = viewMinY;
                    XYPlotSettings.YAxis.ViewMax = viewMaxY;
                }
                else if (viewMinY == viewMaxY)
                {
                    XYPlotSettings.YAxis.ViewMin = viewMinY - 50;
                    XYPlotSettings.YAxis.ViewMax = viewMaxY + 50;
                }

                OnChangeViewY();
            }
        }

        // Curve specific
        public override void DrawCurve()
        {
            drawDayNight();

            for (int i = 0; i < DatasetList.Count; i++)
            {
                BWDataset dataset = DatasetList[i];
                DrawRegularCurve(dataset);
            }
        }
       
        protected override void DrawRegularCurve(BWDataset dataset)
        {
            if (dataset == null || dataset.TotalRecords == 0) return;
            //CleanData(dataset);
            int nTotalRecords = 0;
            double[] dData = null;
            dataset.GetData(out nTotalRecords, out dData);
            DisplayProperty dp = XYPlotSettings.GetDisplaySetting(dataset.Name);
            if (dp.Display == false) return;
            //Pen penCurve = new Pen(dp.Color, dp.Width);
            Axis xAxis = XYPlotSettings.XAxis;
            Axis yAxis = XYPlotSettings.YAxis;

            // check if overwrite primary y axis
            if (dp.TempAxisOverWrite == true)
            {
                yAxis = new Axis();
                yAxis.IsLog = dp.TempAxisIsLog;
                yAxis.ViewMin = dp.TempAxisViewMin;
                yAxis.ViewMax = dp.TempAxisViewMax;
                yAxis.ReversedScale = dp.TempAxisReversedScale;
            }

            if (xAxis.IsLog && (xAxis.ViewMin <= 0 || xAxis.ViewMax <= 0)) return;
            if (yAxis.IsLog && (yAxis.ViewMin <= 0 || yAxis.ViewMax <= 0)) return;

            // find startIndex1 and endIndex1
            int startIndex = -1;
            int endIndex = nTotalRecords;
            int nTotalColumns = dataset.TotalColumns;
            int runNumberColIndex = 0;
            int startTimeColIndex = 1;
            int stopTimeColIndex = 2;
            int durationColIndex = 3;
            int startDepthColIndex = 4;
            int endDepthColIndex = 5;
            FindStartEndIndices(nTotalRecords, dData, nTotalColumns, startTimeColIndex, stopTimeColIndex, xAxis.ViewMin,
                xAxis.ViewMax, ref startIndex, ref endIndex);

            if (startIndex >= 0 && endIndex < nTotalRecords)
            {
                if (startIndex > 0) startIndex--;
                if (endIndex < nTotalRecords - 1) endIndex++;
                int count = endIndex - startIndex + 1;
                if (count >= 1)
                {
                    Font font = XYPlotSettings.XAxis.TickFont;
                    bool bVertical = XYPlotSettings.XAxis.IsTickVertical;

                    for (int j = startIndex; j <= endIndex; j++)
                    {
                        double x1 = dData[nTotalColumns * j + startTimeColIndex];
                        double x2 = dData[nTotalColumns * j + stopTimeColIndex];
                        double duration = dData[nTotalColumns * j + durationColIndex];
                        if (duration * 60 < DurationDisplayLimit) continue;

                        int pixelX1 = XValueToPixel(x1, xAxis);
                        int pixelX2 = XValueToPixel(x2, xAxis);

                        String startTime = BWConverter.SecondsToLongTime(x1);
                        String stopTime = BWConverter.SecondsToLongTime(x2);
                        String runNumber = ((int)dData[nTotalColumns * j + runNumberColIndex]).ToString();

                        if (String.Compare(dataset.Name, "TimeDepth Pump", false) == 0)
                        {
                            double y1 = dData[nTotalColumns * j + startDepthColIndex];
                            double y2 = dData[nTotalColumns * j + endDepthColIndex];
                            int pixelY1 = YValueToPixel(y1, yAxis);
                            int pixelY2 = YValueToPixel(y2, yAxis);
                            Point pt1 = new Point(pixelX1, pixelY1);
                            Point pt2 = new Point(pixelX2, pixelY2);
                            Point pt3 = new Point(pixelX2, rectPlotArea.Bottom);
                            Point pt4 = new Point(pixelX1, rectPlotArea.Bottom);
                            Point[] pts = { pt1, pt2, pt3, pt4 };
                            //Brush brush = new SolidBrush(Color.FromArgb(128, Color.LightGreen));
                            Brush brush = new SolidBrush(Color.FromArgb(48, dp.Color));
                            FillPolygon(Graphics, brush, pts);

                            // draw StartTime, StopTime, Run #
                            if (XYPlotSettings.XAxis.EnableTick)
                            {
                                //Brush brushXMark = new SolidBrush(Color.Green);
                                Brush brushXMark = new SolidBrush(dp.Color);
                                Point p1 = new Point(pixelX1, Math.Max(pixelY1, rectPlotArea.Top));
                                Point p2 = new Point(pixelX2, Math.Max(pixelY1, rectPlotArea.Top));
                                Point p3 = new Point((pixelX1 + pixelX2) / 2, Math.Max(pixelY1, rectPlotArea.Top));
                                DrawString(Graphics, startTime, font, brushXMark, p1, true, StringAlignment.Center, 1);
                                DrawString(Graphics, stopTime, font, brushXMark, p2, true, StringAlignment.Center, 1);
                                DrawString(Graphics, runNumber, font, brushXMark, p3, bVertical, StringAlignment.Center, 4);
                            }
                        }
                        else if (String.Compare(dataset.Name, "Realtime Pump", false) == 0)
                        {
                            int pixelY = rectPlotArea.Top + rectPlotArea.Height * 4 / 10;
                            //Brush brush = new SolidBrush(Color.FromArgb(128, Color.LightCoral));
                            Brush brush = new SolidBrush(Color.FromArgb(48, dp.Color));
                            Rectangle rect = new Rectangle(pixelX1, pixelY, (pixelX2 - pixelX1), rectPlotArea.Height * 6 / 10);
                            FillRectangle(Graphics, brush, rect);

                            // draw StartTime, StopTime, Run #
                            if (XYPlotSettings.XAxis.EnableTick)
                            {
                                //Brush brushXMark = new SolidBrush(Color.Red);
                                Brush brushXMark = new SolidBrush(dp.Color);
                                Point p1 = new Point(pixelX1, pixelY);
                                Point p2 = new Point(pixelX2, pixelY);
                                Point p3 = new Point((pixelX1 + pixelX2) / 2, pixelY);
                                DrawString(Graphics, startTime, font, brushXMark, p1, true, StringAlignment.Center, 1);
                                DrawString(Graphics, stopTime, font, brushXMark, p2, true, StringAlignment.Center, 1);
                                DrawString(Graphics, runNumber, font, brushXMark, p3, bVertical, StringAlignment.Center, 4);
                            }
                        }
                        else if (String.Compare(dataset.Name, "Memory Pump", false) == 0)
                        {
                            int pixelY = rectPlotArea.Top + rectPlotArea.Height * 6 / 10;
                            //Brush brush = new SolidBrush(Color.FromArgb(192, Color.LightBlue));
                            Brush brush = new SolidBrush(Color.FromArgb(48, dp.Color));
                            Rectangle rect = new Rectangle(pixelX1, pixelY, (pixelX2 - pixelX1), rectPlotArea.Height * 4 / 10);
                            FillRectangle(Graphics, brush, rect);

                            // draw StartTime, StopTime, Run #
                            if (XYPlotSettings.XAxis.EnableTick)
                            {
                                //Brush brushXMark = new SolidBrush(Color.Blue);
                                Brush brushXMark = new SolidBrush(dp.Color);
                                Point p1 = new Point(pixelX1, pixelY);
                                Point p2 = new Point(pixelX2, pixelY);
                                Point p3 = new Point((pixelX1 + pixelX2) / 2, pixelY);
                                DrawString(Graphics, startTime, font, brushXMark, p1, true, StringAlignment.Center, 1);
                                DrawString(Graphics, stopTime, font, brushXMark, p2, true, StringAlignment.Center, 1);
                                DrawString(Graphics, runNumber, font, brushXMark, p3, bVertical, StringAlignment.Center, 4);
                            }
                        }
                    }
                }
            }
        }
        public static List<double> CalculateTimeMajorGrids(Axis axis)
        {
            int totalTicks = axis.TotalTicks;
            double min = axis.ViewMin;
            double max = axis.ViewMax;
            bool isLog = axis.IsLog;           
            List<double> majorGrids = new List<double>(totalTicks);

            double first = 0.0;
            double interval = 10.0;
            if (isLog == false)
            {
                double diff = max - min;
                interval = diff / totalTicks;
                interval = Utility.FormatFloatingNumber(interval);
                interval = NormalizeFloatingTime(interval);
                int n = (int)Math.Ceiling(min / interval);
                first = n * interval;             
                double tick = first;
                while (tick < max)
                {
                    if (tick >= min && tick <= max) majorGrids.Add(tick);
                    tick += interval;
                }
            }
            else
            {
                first = .1;
                double tick = first;
                while (tick < max)
                {
                    if (tick >= min && tick <= max) majorGrids.Add(tick);
                    tick *= 10;
                }

                // some case like min = 2, max = 6, etc
                if (majorGrids.Count == 0)
                {
                    tick = Utility.FormatFloatingNumber(0.5 * (min + max));
                    majorGrids.Add(tick);
                }
            }

            return majorGrids;
        }
        private void drawDayNight()
        {
            Pen penMajorGrid = new Pen(XYPlotSettings.MajorGridColor, XYPlotSettings.MajorGridWidth * 2);
            penMajorGrid.Alignment = PenAlignment.Center;

            double min = XYPlotSettings.XAxis.ViewMin;
            double max = XYPlotSettings.XAxis.ViewMax;
            Color color = XYPlotSettings.XAxis.TickColor;
            Font font = XYPlotSettings.XAxis.TickFont;
            Font font2 = new Font(font.FontFamily, font.Size, FontStyle.Bold);

            // draw day and night
            int n = (int)min;
            int r = n % 86400;
            int q = (n - r) / 86400;
            int t = q * 86400;
            while (t < max)
            {   // [t1, t2]: day; [t2, t3]: night
                int t1 = t + 6 * 3600;
                int t2 = t + 18 * 3600;
                int t3 = t + 24 * 3600;
                int pixelt = XValueToPixel(t);
                int pixelt1 = XValueToPixel(t1);
                int pixelt2 = XValueToPixel(t2);
                int pixelt3 = XValueToPixel(t3);

                Graphics.SetClip(rectPlotArea);
                Rectangle rectNight1 = new Rectangle(pixelt, rectPlotArea.Top, pixelt1 - pixelt, rectPlotArea.Height);
                Rectangle rectNight2 = new Rectangle(pixelt2, rectPlotArea.Top, pixelt3 - pixelt2, rectPlotArea.Height);
                FillRectangle(Graphics, new SolidBrush(Color.FromArgb(64, Color.Gray)), rectNight1);
                FillRectangle(Graphics, new SolidBrush(Color.FromArgb(64, Color.Gray)), rectNight2);
                Graphics.SetClip(rectClientArea);
                Point p1 = new Point(pixelt, rectPlotArea.Bottom);
                Point p2 = new Point(pixelt, rectPlotArea.Top);
                DrawLine(Graphics, penMajorGrid, p1, p2);

                Brush brushXMark = new SolidBrush(color);
                String xDate = BWConverter.SecondsToLongTime(t, false, true);
                xDate = xDate.Substring(0, 10);
                bool bVertical = XYPlotSettings.XAxis.IsTickVertical;
                DrawString(Graphics, xDate, font2, brushXMark, p1, bVertical, StringAlignment.Center, 1);

                t += 86400;
            }
        }
        public static double NormalizeFloatingTime(double d)
        {
            //int n = (int)(d /86400);
            //if (n > 0)
            //    d = n * 86400;
            //else
            //{
               int n = (int)(d / 3600);
                if (n > 0)
                    d = n * 3600;
            //}
            return d;
        }
        // find the first and the last indices of the data that in the view window
        // multiple column data
        protected void FindStartEndIndices(int nTotalRecords, double[] dData,
            int nTotalColumns, int x1ColIndex, int x2ColIndex,
            double xViewMin, double xViewMax,
            ref int startIndex, ref int endIndex)
        {
            // find startIndex1 and endIndex1
            startIndex = 0;
            endIndex = nTotalRecords - 1;
            int j = 0;
            try
            {
                for (j = 0; j < nTotalRecords - 1; j++)
                {
                    double x1 = dData[nTotalColumns * j + x1ColIndex];
                    double x2 = dData[nTotalColumns * j + x2ColIndex];
                    if (x1 >= xViewMin)
                    {
                        startIndex = j;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                int i = j;
            }

            try
            {
                for (j = nTotalRecords - 1; j > 0; j--)
                {
                    double x1 = dData[nTotalColumns * j + x1ColIndex];
                    double x2 = dData[nTotalColumns * j + x2ColIndex];
                    if (x2 <= xViewMax)
                    {
                        endIndex = j;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                int i = j;
            }
        }

        private void getMinMaxTime(out double minTime, out double maxTime)
        {
            minTime = BWDataset.NO_READING;
            maxTime = BWDataset.NO_READING;
            if (DatasetList == null || DatasetList.Count == 0) return;

            bool first = true;
            foreach (BWDataset dataset in DatasetList)
            {
                if (dataset == null) continue;

                int colIndex = 1;
                if (first == true)
                {
                    dataset.GetMinMaxValues(colIndex, out minTime, out maxTime);
                    first = false;
                }
                else
                {
                    double min, max = BWDataset.NO_READING;
                    dataset.GetMinMaxValues(colIndex, out min, out max);
                    if (min < minTime) minTime = min;
                    if (max > maxTime) maxTime = max;
                }
            }
        }

        private void getMinMaxValue(int colIndex, out double minValue, out double maxValue)
        {
            minValue = BWDataset.NO_READING;
            maxValue = BWDataset.NO_READING;
            if (DatasetList == null || DatasetList.Count == 0) return;

            bool first = true;
            foreach (BWDataset dataset in DatasetList)
            {
                if (dataset == null || dataset.TotalColumns <= colIndex) continue;

                if (first == true)
                {
                    dataset.GetMinMaxValues(colIndex, out minValue, out maxValue);
                    first = false;
                }
                else
                {
                    double min, max = BWDataset.NO_READING;
                    dataset.GetMinMaxValues(colIndex, out min, out max);
                    if (min < minValue) minValue = min;
                    if (max > maxValue) maxValue = max;
                }
            }
        }
    }

    public class ColorMap
    {
        # region color matrix
        private static Color[] colorMatrix = {
            Color.FromArgb(250,	255,	179),
            Color.FromArgb(250,	254,	176),
            Color.FromArgb(250,	253,	173),
            Color.FromArgb(250,	252,	171),
            Color.FromArgb(250,	252,	168),
            Color.FromArgb(250,	251,	166),
            Color.FromArgb(250,	250,	163),
            Color.FromArgb(250,	249,	161),
            Color.FromArgb(250,	249,	158),
            Color.FromArgb(250,	248,	156),
            Color.FromArgb(250,	247,	153),
            Color.FromArgb(250,	247,	151),
            Color.FromArgb(250,	246,	148),
            Color.FromArgb(250,	245,	146),
            Color.FromArgb(250,	244,	143),
            Color.FromArgb(250,	244,	141),
            Color.FromArgb(251,	243,	138),
            Color.FromArgb(251,	242,	136),
            Color.FromArgb(251,	242,	133),
            Color.FromArgb(251,	241,	131),
            Color.FromArgb(251,	240,	128),
            Color.FromArgb(251,	239,	126),
            Color.FromArgb(251,	239,	123),
            Color.FromArgb(251,	238,	121),
            Color.FromArgb(251,	237,	118),
            Color.FromArgb(251,	237,	116),
            Color.FromArgb(251,	236,	113),
            Color.FromArgb(251,	235,	111),
            Color.FromArgb(251,	234,	108),
            Color.FromArgb(251,	234,	106),
            Color.FromArgb(251,	233,	103),
            Color.FromArgb(251,	232,	101),
            Color.FromArgb(252,	232,	98),
            Color.FromArgb(252,	231,	95),
            Color.FromArgb(252,	230,	93),
            Color.FromArgb(252,	229,	90),
            Color.FromArgb(252,	229,	88),
            Color.FromArgb(252,	228,	85),
            Color.FromArgb(252,	227,	83),
            Color.FromArgb(252,	226,	80),
            Color.FromArgb(252,	226,	78),
            Color.FromArgb(252,	225,	75),
            Color.FromArgb(252,	224,	73),
            Color.FromArgb(252,	224,	70),
            Color.FromArgb(252,	223,	68),
            Color.FromArgb(252,	222,	65),
            Color.FromArgb(252,	221,	63),
            Color.FromArgb(252,	221,	60),
            Color.FromArgb(253,	220,	58),
            Color.FromArgb(253,	219,	55),
            Color.FromArgb(253,	219,	53),
            Color.FromArgb(253,	218,	50),
            Color.FromArgb(253,	217,	48),
            Color.FromArgb(253,	216,	45),
            Color.FromArgb(253,	216,	43),
            Color.FromArgb(253,	215,	40),
            Color.FromArgb(253,	214,	38),
            Color.FromArgb(253,	214,	35),
            Color.FromArgb(253,	213,	33),
            Color.FromArgb(253,	212,	30),
            Color.FromArgb(253,	211,	28),
            Color.FromArgb(253,	211,	25),
            Color.FromArgb(253,	210,	23),
            Color.FromArgb(253,	209,	20),
            Color.FromArgb(254,	209,	18),
            Color.FromArgb(253,	207,	17),
            Color.FromArgb(253,	206,	17),
            Color.FromArgb(252,	205,	17),
            Color.FromArgb(252,	203,	17),
            Color.FromArgb(252,	202,	16),
            Color.FromArgb(251,	201,	16),
            Color.FromArgb(251,	200,	16),
            Color.FromArgb(251,	198,	16),
            Color.FromArgb(250,	197,	16),
            Color.FromArgb(250,	196,	15),
            Color.FromArgb(250,	195,	15),
            Color.FromArgb(249,	193,	15),
            Color.FromArgb(249,	192,	15),
            Color.FromArgb(249,	191,	15),
            Color.FromArgb(248,	190,	14),
            Color.FromArgb(248,	188,	14),
            Color.FromArgb(248,	187,	14),
            Color.FromArgb(247,	186,	14),
            Color.FromArgb(247,	184,	14),
            Color.FromArgb(247,	183,	13),
            Color.FromArgb(246,	182,	13),
            Color.FromArgb(246,	181,	13),
            Color.FromArgb(246,	179,	13),
            Color.FromArgb(245,	178,	13),
            Color.FromArgb(245,	177,	12),
            Color.FromArgb(245,	176,	12),
            Color.FromArgb(244,	174,	12),
            Color.FromArgb(244,	173,	12),
            Color.FromArgb(244,	172,	12),
            Color.FromArgb(243,	171,	11),
            Color.FromArgb(243,	169,	11),
            Color.FromArgb(243,	168,	11),
            Color.FromArgb(242,	167,	11),
            Color.FromArgb(242,	165,	11),
            Color.FromArgb(241,	164,	10),
            Color.FromArgb(241,	163,	10),
            Color.FromArgb(241,	162,	10),
            Color.FromArgb(240,	160,	10),
            Color.FromArgb(240,	159,	10),
            Color.FromArgb(240,	158,	9),
            Color.FromArgb(239,	157,	9),
            Color.FromArgb(239,	155,	9),
            Color.FromArgb(239,	154,	9),
            Color.FromArgb(238,	153,	9),
            Color.FromArgb(238,	152,	8),
            Color.FromArgb(238,	150,	8),
            Color.FromArgb(237,	149,	8),
            Color.FromArgb(237,	148,	8),
            Color.FromArgb(237,	146,	8),
            Color.FromArgb(236,	145,	7),
            Color.FromArgb(236,	144,	7),
            Color.FromArgb(236,	143,	7),
            Color.FromArgb(235,	141,	7),
            Color.FromArgb(235,	140,	7),
            Color.FromArgb(235,	139,	6),
            Color.FromArgb(234,	138,	6),
            Color.FromArgb(234,	136,	6),
            Color.FromArgb(234,	135,	6),
            Color.FromArgb(233,	134,	6),
            Color.FromArgb(233,	133,	5),
            Color.FromArgb(233,	131,	5),
            Color.FromArgb(232,	130,	5),
            Color.FromArgb(232,	129,	5),
            Color.FromArgb(232,	128,	5),
            Color.FromArgb(230,	127,	5),
            Color.FromArgb(229,	126,	5),
            Color.FromArgb(228,	125,	5),
            Color.FromArgb(227,	125,	5),
            Color.FromArgb(225,	124,	5),
            Color.FromArgb(224,	123,	5),
            Color.FromArgb(223,	123,	5),
            Color.FromArgb(222,	122,	5),
            Color.FromArgb(220,	121,	5),
            Color.FromArgb(219,	120,	5),
            Color.FromArgb(218,	120,	5),
            Color.FromArgb(217,	119,	5),
            Color.FromArgb(215,	118,	5),
            Color.FromArgb(214,	118,	5),
            Color.FromArgb(213,	117,	5),
            Color.FromArgb(212,	116,	5),
            Color.FromArgb(210,	115,	5),
            Color.FromArgb(209,	115,	5),
            Color.FromArgb(208,	114,	5),
            Color.FromArgb(207,	113,	5),
            Color.FromArgb(205,	113,	5),
            Color.FromArgb(204,	112,	5),
            Color.FromArgb(203,	111,	5),
            Color.FromArgb(202,	110,	5),
            Color.FromArgb(200,	110,	5),
            Color.FromArgb(199,	109,	5),
            Color.FromArgb(198,	108,	5),
            Color.FromArgb(197,	108,	5),
            Color.FromArgb(195,	107,	5),
            Color.FromArgb(194,	106,	5),
            Color.FromArgb(193,	106,	5),
            Color.FromArgb(192,	105,	5),
            Color.FromArgb(190,	104,	5),
            Color.FromArgb(189,	103,	5),
            Color.FromArgb(188,	103,	5),
            Color.FromArgb(187,	102,	5),
            Color.FromArgb(185,	101,	5),
            Color.FromArgb(184,	101,	5),
            Color.FromArgb(183,	100,	5),
            Color.FromArgb(182,	99,	5),
            Color.FromArgb(180,	98,	5),
            Color.FromArgb(179,	98,	5),
            Color.FromArgb(178,	97,	5),
            Color.FromArgb(177,	96,	5),
            Color.FromArgb(176,	96,	5),
            Color.FromArgb(174,	95,	5),
            Color.FromArgb(173,	94,	5),
            Color.FromArgb(172,	93,	5),
            Color.FromArgb(171,	93,	5),
            Color.FromArgb(169,	92,	5),
            Color.FromArgb(168,	91,	5),
            Color.FromArgb(167,	91,	5),
            Color.FromArgb(166,	90,	5),
            Color.FromArgb(164,	89,	5),
            Color.FromArgb(163,	89,	5),
            Color.FromArgb(162,	88,	5),
            Color.FromArgb(161,	87,	5),
            Color.FromArgb(159,	86,	5),
            Color.FromArgb(158,	86,	5),
            Color.FromArgb(157,	85,	5),
            Color.FromArgb(156,	84,	5),
            Color.FromArgb(154,	84,	5),
            Color.FromArgb(153,	83,	5),
            Color.FromArgb(152,	82,	6),
            Color.FromArgb(151,	81,	6),
            Color.FromArgb(149,	81,	6),
            Color.FromArgb(148,	80,	6),
            Color.FromArgb(147,	79,	6),
            Color.FromArgb(146,	79,	6),
            Color.FromArgb(144,	78,	6),
            Color.FromArgb(143,	77,	6),
            Color.FromArgb(142,	76,	6),
            Color.FromArgb(141,	76,	6),
            Color.FromArgb(139,	75,	6),
            Color.FromArgb(138,	74,	6),
            Color.FromArgb(137,	74,	6),
            Color.FromArgb(136,	73,	6),
            Color.FromArgb(134,	72,	6),
            Color.FromArgb(133,	72,	6),
            Color.FromArgb(132,	71,	6),
            Color.FromArgb(131,	70,	6),
            Color.FromArgb(129,	69,	6),
            Color.FromArgb(128,	69,	6),
            Color.FromArgb(127,	68,	6),
            Color.FromArgb(126,	67,	6),
            Color.FromArgb(125,	67,	6),
            Color.FromArgb(123, 66,	6),
            Color.FromArgb(122,	65,	6),
            Color.FromArgb(121,	64,	6),
            Color.FromArgb(120,	64,	6),
            Color.FromArgb(118,	63,	6),
            Color.FromArgb(117,	62,	6),
            Color.FromArgb(116,	62,	6),
            Color.FromArgb(115,	61,	6),
            Color.FromArgb(113,	60,	6),
            Color.FromArgb(112,	59,	6),
            Color.FromArgb(111,	59,	6),
            Color.FromArgb(110,	58,	6),
            Color.FromArgb(108,	57,	6),
            Color.FromArgb(107,	57,	6),
            Color.FromArgb(106,	56,	6),
            Color.FromArgb(105,	55,	6),
            Color.FromArgb(103,	55,	6),
            Color.FromArgb(102,	54,	6),
            Color.FromArgb(101,	53,	6),
            Color.FromArgb(100,	52,	6),
            Color.FromArgb(98,	52,	6),
            Color.FromArgb(97,	51,	6),
            Color.FromArgb(96,	50,	6),
            Color.FromArgb(95,	50,	6),
            Color.FromArgb(93,	49,	6),
            Color.FromArgb(92,	48,	6),
            Color.FromArgb(91,	47,	6),
            Color.FromArgb(90,	47,	6),
            Color.FromArgb(88,	46,	6),
            Color.FromArgb(87,	45,	6),
            Color.FromArgb(86,	45,	6),
            Color.FromArgb(85,	44,	6),
            Color.FromArgb(83,	43,	6),
            Color.FromArgb(82,	42,	6),
            Color.FromArgb(81,	42,	6),
            Color.FromArgb(80,	41,	6),
            Color.FromArgb(78,	40,	6),
            Color.FromArgb(77,	40,	6),
            Color.FromArgb(76,	39,	6),
            Color.FromArgb(75,	38,	6),
            Color.FromArgb(74,	38,	7)
        };
        # endregion

        private static int numberOfColors = 256;
        private static int minColorIndex = numberOfColors - 1;
        private static int maxColorIndex = 0;
        private bool reverseColor;

        public bool ReverseColor
        {
            get { return reverseColor; }
            set { reverseColor = value; }
        }

        public Color MinColor
        {
            get { return reverseColor ? colorMatrix[maxColorIndex] : colorMatrix[minColorIndex]; }
        }

        public Color MaxColor
        {
            get { return reverseColor ? colorMatrix[minColorIndex] : colorMatrix[maxColorIndex]; }
        }

        public ColorMap(bool revserse)
        {
            reverseColor = revserse;
        }

        public Color GetColor(int index)
        {
            if (index < 0) index = 0;
            if (index > numberOfColors - 1) index = numberOfColors - 1;

            return reverseColor ? colorMatrix[index] : colorMatrix[numberOfColors - 1 - index];
        }
    }
}

