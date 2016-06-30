namespace DataChecker
{
    partial class Form1
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Err 1 @ 12:34:56");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Gamma Signal", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Gamma", new System.Windows.Forms.TreeNode[] {
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Err 1 @ 12:34:56");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Err 2 @ 12:37:12");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Resistivity 1", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Resistivity 2");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Resistivity 3");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Resistivity", new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7,
            treeNode8});
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Test", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode9});
            this.btnAddFile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnDeleteFile = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnShowChecked = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.treeView1 = new NSTreeViewEx.TreeViewEx();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDrawTitle = new System.Windows.Forms.Label();
            this.xyPlotCtrl0 = new NSXYPlot.XYPlotCtrl();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddFile
            // 
            this.btnAddFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddFile.Location = new System.Drawing.Point(119, 32);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(97, 27);
            this.btnAddFile.TabIndex = 6;
            this.btnAddFile.Text = "Add";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnDeleteFile
            // 
            this.btnDeleteFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteFile.Location = new System.Drawing.Point(223, 32);
            this.btnDeleteFile.Name = "btnDeleteFile";
            this.btnDeleteFile.Size = new System.Drawing.Size(96, 28);
            this.btnDeleteFile.TabIndex = 30;
            this.btnDeleteFile.Text = "Remove";
            this.btnDeleteFile.UseVisualStyleBackColor = true;
            this.btnDeleteFile.Click += new System.EventHandler(this.btnDeleteFile_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(923, 24);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(161, 43);
            this.btnTest.TabIndex = 31;
            this.btnTest.Text = "btnTest";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 16);
            this.label1.TabIndex = 33;
            this.label1.Text = "Dump Files:";
            // 
            // btnShowChecked
            // 
            this.btnShowChecked.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowChecked.Location = new System.Drawing.Point(325, 32);
            this.btnShowChecked.Name = "btnShowChecked";
            this.btnShowChecked.Size = new System.Drawing.Size(105, 27);
            this.btnShowChecked.TabIndex = 34;
            this.btnShowChecked.Text = "Check";
            this.btnShowChecked.UseVisualStyleBackColor = true;
            this.btnShowChecked.Click += new System.EventHandler(this.btnShowChecked_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1548, 24);
            this.menuStrip1.TabIndex = 38;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.fileToolStripMenuItem.Text = "Config";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(13, 65);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "";
            treeNode1.Text = "Err 1 @ 12:34:56";
            treeNode2.Name = "";
            treeNode2.Text = "Gamma Signal";
            treeNode3.Name = "";
            treeNode3.Text = "Gamma";
            treeNode4.Name = "";
            treeNode4.Text = "Err 1 @ 12:34:56";
            treeNode5.Name = "";
            treeNode5.Text = "Err 2 @ 12:37:12";
            treeNode6.Name = "";
            treeNode6.Text = "Resistivity 1";
            treeNode7.Name = "";
            treeNode7.Text = "Resistivity 2";
            treeNode8.Name = "";
            treeNode8.Text = "Resistivity 3";
            treeNode9.Name = "";
            treeNode9.Text = "Resistivity";
            treeNode10.Name = "";
            treeNode10.Text = "Test";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode10});
            this.treeView1.Size = new System.Drawing.Size(417, 669);
            this.treeView1.TabIndex = 40;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.lblDrawTitle);
            this.panel1.Controls.Add(this.xyPlotCtrl0);
            this.panel1.Location = new System.Drawing.Point(457, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1088, 669);
            this.panel1.TabIndex = 41;
            // 
            // lblDrawTitle
            // 
            this.lblDrawTitle.AutoSize = true;
            this.lblDrawTitle.Location = new System.Drawing.Point(351, 640);
            this.lblDrawTitle.Name = "lblDrawTitle";
            this.lblDrawTitle.Size = new System.Drawing.Size(41, 15);
            this.lblDrawTitle.TabIndex = 41;
            this.lblDrawTitle.Text = "label2";
            // 
            // xyPlotCtrl0
            // 
            this.xyPlotCtrl0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.xyPlotCtrl0.Location = new System.Drawing.Point(3, 3);
            this.xyPlotCtrl0.Name = "xyPlotCtrl0";
            this.xyPlotCtrl0.Size = new System.Drawing.Size(1055, 631);
            this.xyPlotCtrl0.TabIndex = 40;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1548, 749);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.btnShowChecked);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnDeleteFile);
            this.Controls.Add(this.btnAddFile);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Data Checker";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnDeleteFile;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnShowChecked;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private NSTreeViewEx.TreeViewEx treeView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblDrawTitle;
        private NSXYPlot.XYPlotCtrl xyPlotCtrl0;
    }
}

