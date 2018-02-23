namespace SimpleCADTest
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusCoords = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnDrawLine = new System.Windows.Forms.ToolStripButton();
            this.btnDrawCircle = new System.Windows.Forms.ToolStripButton();
            this.btnDrawEllipse = new System.Windows.Forms.ToolStripButton();
            this.btnDrawArc = new System.Windows.Forms.ToolStripButton();
            this.btnDrawEllipticArc = new System.Windows.Forms.ToolStripButton();
            this.btnDrawText = new System.Windows.Forms.ToolStripButton();
            this.cadWindow1 = new SimpleCAD.CADWindow();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1008, 469);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1008, 516);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.statusCoords});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip1.TabIndex = 0;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(968, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusCoords
            // 
            this.statusCoords.Name = "statusCoords";
            this.statusCoords.Size = new System.Drawing.Size(25, 17);
            this.statusCoords.Text = "0, 0";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cadWindow1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(1008, 469);
            this.splitContainer1.SplitterDistance = 759;
            this.splitContainer1.TabIndex = 2;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(245, 469);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDrawLine,
            this.btnDrawCircle,
            this.btnDrawEllipse,
            this.btnDrawArc,
            this.btnDrawEllipticArc,
            this.btnDrawText});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(258, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // btnDrawLine
            // 
            this.btnDrawLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawLine.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawLine.Image")));
            this.btnDrawLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawLine.Name = "btnDrawLine";
            this.btnDrawLine.Size = new System.Drawing.Size(33, 22);
            this.btnDrawLine.Text = "Line";
            this.btnDrawLine.ToolTipText = "Draw Line";
            this.btnDrawLine.Click += new System.EventHandler(this.btnDrawLine_Click);
            // 
            // btnDrawCircle
            // 
            this.btnDrawCircle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawCircle.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawCircle.Image")));
            this.btnDrawCircle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawCircle.Name = "btnDrawCircle";
            this.btnDrawCircle.Size = new System.Drawing.Size(41, 22);
            this.btnDrawCircle.Text = "Circle";
            this.btnDrawCircle.Click += new System.EventHandler(this.btnDrawCircle_Click);
            // 
            // btnDrawEllipse
            // 
            this.btnDrawEllipse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawEllipse.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawEllipse.Image")));
            this.btnDrawEllipse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawEllipse.Name = "btnDrawEllipse";
            this.btnDrawEllipse.Size = new System.Drawing.Size(44, 22);
            this.btnDrawEllipse.Text = "Ellipse";
            this.btnDrawEllipse.Click += new System.EventHandler(this.btnDrawEllipse_Click);
            // 
            // btnDrawArc
            // 
            this.btnDrawArc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawArc.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawArc.Image")));
            this.btnDrawArc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawArc.Name = "btnDrawArc";
            this.btnDrawArc.Size = new System.Drawing.Size(29, 22);
            this.btnDrawArc.Text = "Arc";
            this.btnDrawArc.Click += new System.EventHandler(this.btnDrawArc_Click);
            // 
            // btnDrawEllipticArc
            // 
            this.btnDrawEllipticArc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawEllipticArc.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawEllipticArc.Image")));
            this.btnDrawEllipticArc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawEllipticArc.Name = "btnDrawEllipticArc";
            this.btnDrawEllipticArc.Size = new System.Drawing.Size(67, 22);
            this.btnDrawEllipticArc.Text = "Elliptic Arc";
            this.btnDrawEllipticArc.Click += new System.EventHandler(this.btnDrawEllipticArc_Click);
            // 
            // btnDrawText
            // 
            this.btnDrawText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawText.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawText.Image")));
            this.btnDrawText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawText.Name = "btnDrawText";
            this.btnDrawText.Size = new System.Drawing.Size(32, 22);
            this.btnDrawText.Text = "Text";
            this.btnDrawText.Click += new System.EventHandler(this.btnDrawText_Click);
            // 
            // cadWindow1
            // 
            this.cadWindow1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(40)))), ((int)(((byte)(48)))));
            this.cadWindow1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.cadWindow1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.cadWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cadWindow1.Location = new System.Drawing.Point(0, 0);
            this.cadWindow1.Name = "cadWindow1";
            this.cadWindow1.Size = new System.Drawing.Size(759, 469);
            this.cadWindow1.TabIndex = 0;
            this.cadWindow1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cadWindow1_MouseClick);
            this.cadWindow1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cadWindow1_MouseMove);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 516);
            this.Controls.Add(this.toolStripContainer1);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "SimpleCAD Test Form";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SimpleCAD.CADWindow cadWindow1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnDrawLine;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripButton btnDrawArc;
        private System.Windows.Forms.ToolStripButton btnDrawCircle;
        private System.Windows.Forms.ToolStripButton btnDrawEllipse;
        private System.Windows.Forms.ToolStripStatusLabel statusCoords;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripButton btnDrawEllipticArc;
        private System.Windows.Forms.ToolStripButton btnDrawText;
    }
}

