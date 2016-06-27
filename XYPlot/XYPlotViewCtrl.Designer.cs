namespace NSXYPlot
{
    partial class XYPlotViewCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStripProperty = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemAddPlot = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePlotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPlotProperty = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPlotViewProperty = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStripProperty.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripProperty
            // 
            this.contextMenuStripProperty.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddPlot,
            this.deletePlotToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItemPlotProperty,
            this.toolStripMenuItemPlotViewProperty});
            this.contextMenuStripProperty.Name = "contextMenuStripProperty";
            this.contextMenuStripProperty.Size = new System.Drawing.Size(181, 120);
            // 
            // toolStripMenuItemAddPlot
            // 
            this.toolStripMenuItemAddPlot.Name = "toolStripMenuItemAddPlot";
            this.toolStripMenuItemAddPlot.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemAddPlot.Text = "Add Plot";
            this.toolStripMenuItemAddPlot.Click += new System.EventHandler(this.toolStripMenuItemAddPlot_Click);
            // 
            // deletePlotToolStripMenuItem
            // 
            this.deletePlotToolStripMenuItem.Name = "deletePlotToolStripMenuItem";
            this.deletePlotToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deletePlotToolStripMenuItem.Text = "Delete Plot";
            this.deletePlotToolStripMenuItem.Click += new System.EventHandler(this.deletePlotToolStripMenuItem_Click);
            // 
            // toolStripMenuItemPlotProperty
            // 
            this.toolStripMenuItemPlotProperty.Name = "toolStripMenuItemPlotProperty";
            this.toolStripMenuItemPlotProperty.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemPlotProperty.Text = "Plot Property ...";
            this.toolStripMenuItemPlotProperty.Click += new System.EventHandler(this.toolStripMenuItemPlotProperty_Click);
            // 
            // toolStripMenuItemPlotViewProperty
            // 
            this.toolStripMenuItemPlotViewProperty.Name = "toolStripMenuItemPlotViewProperty";
            this.toolStripMenuItemPlotViewProperty.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemPlotViewProperty.Text = "PlotView Property ...";
            this.toolStripMenuItemPlotViewProperty.Click += new System.EventHandler(this.toolStripMenuItemPlotViewProperty_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // XYPlotViewCtrl
            // 
            this.BackColor = System.Drawing.Color.Snow;
            this.ContextMenuStrip = this.contextMenuStripProperty;
            this.Name = "XYPlotViewCtrl";
            this.Size = new System.Drawing.Size(493, 336);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.XYPlotViewCtrl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.XYPlotViewCtrl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.XYPlotViewCtrl_MouseUp);
            this.MouseWheel += XYPlotViewCtrl_MouseWheel;
            this.contextMenuStripProperty.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStripPlotViewProperty;
        private System.Windows.Forms.ToolStripMenuItem plotViewPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plotPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPlotToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripProperty;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddPlot;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPlotProperty;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPlotViewProperty;
        private System.Windows.Forms.ToolStripMenuItem deletePlotToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
