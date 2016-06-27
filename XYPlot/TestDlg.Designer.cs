namespace NSXYPlot
{
    partial class TestDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.xyPlotCtrl1 = new NSXYPlot.XYPlotCtrl();
            this.xyPlotViewCtrl1 = new NSXYPlot.XYPlotViewCtrl();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // xyPlotCtrl1
            // 
            this.xyPlotCtrl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.xyPlotCtrl1.Location = new System.Drawing.Point(165, 261);
            this.xyPlotCtrl1.Name = "xyPlotCtrl1";
            this.xyPlotCtrl1.Size = new System.Drawing.Size(322, 222);
            this.xyPlotCtrl1.TabIndex = 1;
            // 
            // xyPlotViewCtrl1
            // 
            this.xyPlotViewCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xyPlotViewCtrl1.BackColor = System.Drawing.Color.Snow;
            this.xyPlotViewCtrl1.Location = new System.Drawing.Point(0, 0);
            this.xyPlotViewCtrl1.Name = "xyPlotViewCtrl1";
            this.xyPlotViewCtrl1.Size = new System.Drawing.Size(504, 420);
            this.xyPlotViewCtrl1.TabIndex = 0;
            // 
            // TestDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(634, 515);
            this.Controls.Add(this.xyPlotCtrl1);
            this.Controls.Add(this.xyPlotViewCtrl1);
            this.Name = "TestDlg";
            this.Text = "TestDlg";
            this.Load += new System.EventHandler(this.TestDlg_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private XYPlotViewCtrl xyPlotViewCtrl1;
        private XYPlotCtrl xyPlotCtrl1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}