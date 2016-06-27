using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace NSXYPlot
{
    public partial class XYPlotCtrl : UserControl
    {
        public XYPlot XYPlot1;

        public Point PtMouseStart;
        public Point PtMouseEnd;
        protected PlotViewMouseMode MouseMode;

        public XYPlotCtrl()
        {
            InitializeComponent();

            XYPlot1 = new XYPlot(null, this, ClientRectangle);
        }

        public void SetPlotType(PlotType plotType)
        {
            switch (plotType)
            {
                case PlotType.XYPlotTimeDepth:
                    XYPlot1 = new XYPlotTimeDepth(null, this, ClientRectangle);
                    break;
                default:
                    XYPlot1 = new XYPlot(null, this, ClientRectangle);
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Rectangle rect = ClientRectangle;
            rect.Inflate(-2, -2);
            if (XYPlot1 == null) return;                
            
            XYPlot1.Location = rect.Location;
            XYPlot1.Size = rect.Size;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (XYPlot1 == null) return;

            XYPlot1.Graphics = e.Graphics;
            XYPlot1.Draw();
        }

        public virtual XYPlot GetXYPlot(Point pt)
        {
            XYPlot xyPlot = null;

            Rectangle rect = new Rectangle(XYPlot1.Location, XYPlot1.Size);
            if (rect.Contains(pt)) xyPlot = XYPlot1;

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

        private void plotPropertyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XYPlotSettingDlg dlg = new XYPlotSettingDlg(XYPlot1.XYPlotSettings, XYPlot1);
            dlg.InitialDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void XYPlotCtrl_MouseUp(object sender, MouseEventArgs e)
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

        private void XYPlotCtrl_MouseDown(object sender, MouseEventArgs e)
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

        private void XYPlotCtrl_MouseMove(object sender, MouseEventArgs e)
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

        private void XYPlotCtrl_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = e.Delta;
            Point pt = new Point(e.Location.X, e.Location.Y);
            XYPlot xyPlot = GetXYPlot(pt);
            if (xyPlot != null)
            {
                xyPlot.Zoom(delta);
                this.Invalidate();
            }
        }
    }
}
