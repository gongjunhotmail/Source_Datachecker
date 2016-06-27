namespace DataChecker
{
    partial class DataCheckerConfigForm
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
            this.ConfigCloseButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ConfigCancelButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.SignalNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpBoundaryColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LowBoundaryColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ConfigCloseButton
            // 
            this.ConfigCloseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConfigCloseButton.Location = new System.Drawing.Point(70, 579);
            this.ConfigCloseButton.Name = "ConfigCloseButton";
            this.ConfigCloseButton.Size = new System.Drawing.Size(116, 24);
            this.ConfigCloseButton.TabIndex = 10;
            this.ConfigCloseButton.Text = "Close";
            this.ConfigCloseButton.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(11, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(528, 551);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(520, 525);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(520, 525);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Signal Limits";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ConfigCancelButton
            // 
            this.ConfigCancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConfigCancelButton.Location = new System.Drawing.Point(317, 579);
            this.ConfigCancelButton.Name = "ConfigCancelButton";
            this.ConfigCancelButton.Size = new System.Drawing.Size(116, 24);
            this.ConfigCancelButton.TabIndex = 14;
            this.ConfigCancelButton.Text = "Cancel";
            this.ConfigCancelButton.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SignalNameColumn,
            this.UpBoundaryColumn,
            this.LowBoundaryColumn});
            this.dataGridView1.Location = new System.Drawing.Point(26, 21);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(344, 324);
            this.dataGridView1.TabIndex = 0;
            // 
            // SignalNameColumn
            // 
            this.SignalNameColumn.HeaderText = "Signal Name";
            this.SignalNameColumn.Name = "SignalNameColumn";
            // 
            // UpBoundaryColumn
            // 
            this.UpBoundaryColumn.HeaderText = "Up Boundary";
            this.UpBoundaryColumn.Name = "UpBoundaryColumn";
            // 
            // LowBoundaryColumn
            // 
            this.LowBoundaryColumn.HeaderText = "Low Boundary";
            this.LowBoundaryColumn.Name = "LowBoundaryColumn";
            // 
            // DataCheckerConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 615);
            this.Controls.Add(this.ConfigCancelButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.ConfigCloseButton);
            this.Name = "DataCheckerConfigForm";
            this.Text = "Data Checker Configuatation";
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ConfigCloseButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button ConfigCancelButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SignalNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpBoundaryColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LowBoundaryColumn;
    }
}