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
            SimpleCAD.CADDocument cadDocument2 = new SimpleCAD.CADDocument();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusCoords = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsStandard = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveAs = new System.Windows.Forms.ToolStripButton();
            this.tsGraphics = new System.Windows.Forms.ToolStrip();
            this.lblRenderer = new System.Windows.Forms.ToolStripLabel();
            this.btnRenderer = new System.Windows.Forms.ToolStripComboBox();
            this.btnShowGrid = new System.Windows.Forms.ToolStripButton();
            this.btnShowAxes = new System.Windows.Forms.ToolStripButton();
            this.tsPrimitives = new System.Windows.Forms.ToolStrip();
            this.btnDrawLine = new System.Windows.Forms.ToolStripButton();
            this.btnDrawCircle = new System.Windows.Forms.ToolStripButton();
            this.btnDrawEllipse = new System.Windows.Forms.ToolStripButton();
            this.btnDrawArc = new System.Windows.Forms.ToolStripButton();
            this.btnDrawEllipticArc = new System.Windows.Forms.ToolStripButton();
            this.btnDrawText = new System.Windows.Forms.ToolStripButton();
            this.btnDrawDimension = new System.Windows.Forms.ToolStripButton();
            this.btnDrawParabola = new System.Windows.Forms.ToolStripButton();
            this.btnDrawPolyline = new System.Windows.Forms.ToolStripButton();
            this.btnDrawRectangle = new System.Windows.Forms.ToolStripButton();
            this.btnDrawTriangle = new System.Windows.Forms.ToolStripButton();
            this.btnDrawHatch = new System.Windows.Forms.ToolStripButton();
            this.tsTransform = new System.Windows.Forms.ToolStrip();
            this.btnMove = new System.Windows.Forms.ToolStripButton();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnRotate = new System.Windows.Forms.ToolStripButton();
            this.btnScale = new System.Windows.Forms.ToolStripButton();
            this.btnMirror = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnZoom = new System.Windows.Forms.ToolStripButton();
            this.btnPan = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cadWindow1 = new SimpleCAD.CADWindow();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tsEdit = new System.Windows.Forms.ToolStrip();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tsStandard.SuspendLayout();
            this.tsGraphics.SuspendLayout();
            this.tsPrimitives.SuspendLayout();
            this.tsTransform.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tsEdit.SuspendLayout();
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
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1008, 394);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1008, 516);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsTransform);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsGraphics);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsPrimitives);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsStandard);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsEdit);
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
            // tsStandard
            // 
            this.tsStandard.Dock = System.Windows.Forms.DockStyle.None;
            this.tsStandard.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.btnSaveAs});
            this.tsStandard.Location = new System.Drawing.Point(3, 50);
            this.tsStandard.Name = "tsStandard";
            this.tsStandard.Size = new System.Drawing.Size(173, 25);
            this.tsStandard.TabIndex = 1;
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(35, 22);
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(40, 22);
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(35, 22);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveAs.Image")));
            this.btnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(51, 22);
            this.btnSaveAs.Text = "Save As";
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // tsGraphics
            // 
            this.tsGraphics.Dock = System.Windows.Forms.DockStyle.None;
            this.tsGraphics.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblRenderer,
            this.btnRenderer,
            this.btnShowGrid,
            this.btnShowAxes});
            this.tsGraphics.Location = new System.Drawing.Point(222, 0);
            this.tsGraphics.Name = "tsGraphics";
            this.tsGraphics.Size = new System.Drawing.Size(323, 25);
            this.tsGraphics.TabIndex = 3;
            // 
            // lblRenderer
            // 
            this.lblRenderer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblRenderer.Image = ((System.Drawing.Image)(resources.GetObject("lblRenderer.Image")));
            this.lblRenderer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lblRenderer.Name = "lblRenderer";
            this.lblRenderer.Size = new System.Drawing.Size(57, 22);
            this.lblRenderer.Text = "Renderer:";
            // 
            // btnRenderer
            // 
            this.btnRenderer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.btnRenderer.Name = "btnRenderer";
            this.btnRenderer.Size = new System.Drawing.Size(120, 25);
            this.btnRenderer.SelectedIndexChanged += new System.EventHandler(this.btnRenderer_SelectedIndexChanged);
            // 
            // btnShowGrid
            // 
            this.btnShowGrid.Checked = true;
            this.btnShowGrid.CheckOnClick = true;
            this.btnShowGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnShowGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnShowGrid.Image = ((System.Drawing.Image)(resources.GetObject("btnShowGrid.Image")));
            this.btnShowGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowGrid.Name = "btnShowGrid";
            this.btnShowGrid.Size = new System.Drawing.Size(65, 22);
            this.btnShowGrid.Text = "Show Grid";
            this.btnShowGrid.Click += new System.EventHandler(this.btnShowGrid_Click);
            // 
            // btnShowAxes
            // 
            this.btnShowAxes.Checked = true;
            this.btnShowAxes.CheckOnClick = true;
            this.btnShowAxes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnShowAxes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnShowAxes.Image = ((System.Drawing.Image)(resources.GetObject("btnShowAxes.Image")));
            this.btnShowAxes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowAxes.Name = "btnShowAxes";
            this.btnShowAxes.Size = new System.Drawing.Size(67, 22);
            this.btnShowAxes.Text = "Show Axes";
            this.btnShowAxes.Click += new System.EventHandler(this.btnShowAxes_Click);
            // 
            // tsPrimitives
            // 
            this.tsPrimitives.Dock = System.Windows.Forms.DockStyle.None;
            this.tsPrimitives.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDrawLine,
            this.btnDrawCircle,
            this.btnDrawEllipse,
            this.btnDrawArc,
            this.btnDrawEllipticArc,
            this.btnDrawText,
            this.btnDrawDimension,
            this.btnDrawParabola,
            this.btnDrawPolyline,
            this.btnDrawRectangle,
            this.btnDrawTriangle,
            this.btnDrawHatch});
            this.tsPrimitives.Location = new System.Drawing.Point(89, 25);
            this.tsPrimitives.Name = "tsPrimitives";
            this.tsPrimitives.Size = new System.Drawing.Size(595, 25);
            this.tsPrimitives.TabIndex = 0;
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
            // btnDrawDimension
            // 
            this.btnDrawDimension.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawDimension.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawDimension.Image")));
            this.btnDrawDimension.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawDimension.Name = "btnDrawDimension";
            this.btnDrawDimension.Size = new System.Drawing.Size(68, 22);
            this.btnDrawDimension.Text = "Dimension";
            this.btnDrawDimension.Click += new System.EventHandler(this.btnDrawDimension_Click);
            // 
            // btnDrawParabola
            // 
            this.btnDrawParabola.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawParabola.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawParabola.Image")));
            this.btnDrawParabola.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawParabola.Name = "btnDrawParabola";
            this.btnDrawParabola.Size = new System.Drawing.Size(57, 22);
            this.btnDrawParabola.Text = "Parabola";
            this.btnDrawParabola.Click += new System.EventHandler(this.btnDrawParabola_Click);
            // 
            // btnDrawPolyline
            // 
            this.btnDrawPolyline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawPolyline.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawPolyline.Image")));
            this.btnDrawPolyline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawPolyline.Name = "btnDrawPolyline";
            this.btnDrawPolyline.Size = new System.Drawing.Size(53, 22);
            this.btnDrawPolyline.Text = "Polyline";
            this.btnDrawPolyline.Click += new System.EventHandler(this.btnDrawPolyline_Click);
            // 
            // btnDrawRectangle
            // 
            this.btnDrawRectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawRectangle.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawRectangle.Image")));
            this.btnDrawRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawRectangle.Name = "btnDrawRectangle";
            this.btnDrawRectangle.Size = new System.Drawing.Size(63, 22);
            this.btnDrawRectangle.Text = "Rectangle";
            this.btnDrawRectangle.Click += new System.EventHandler(this.btnDrawRectangle_Click);
            // 
            // btnDrawTriangle
            // 
            this.btnDrawTriangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawTriangle.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawTriangle.Image")));
            this.btnDrawTriangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawTriangle.Name = "btnDrawTriangle";
            this.btnDrawTriangle.Size = new System.Drawing.Size(53, 22);
            this.btnDrawTriangle.Text = "Triangle";
            this.btnDrawTriangle.Click += new System.EventHandler(this.btnDrawTriangle_Click);
            // 
            // btnDrawHatch
            // 
            this.btnDrawHatch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawHatch.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawHatch.Image")));
            this.btnDrawHatch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawHatch.Name = "btnDrawHatch";
            this.btnDrawHatch.Size = new System.Drawing.Size(43, 22);
            this.btnDrawHatch.Text = "Hatch";
            this.btnDrawHatch.Click += new System.EventHandler(this.btnDrawHatch_Click);
            // 
            // tsTransform
            // 
            this.tsTransform.Dock = System.Windows.Forms.DockStyle.None;
            this.tsTransform.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnMove,
            this.btnCopy,
            this.btnRotate,
            this.btnScale,
            this.btnMirror});
            this.tsTransform.Location = new System.Drawing.Point(3, 0);
            this.tsTransform.Name = "tsTransform";
            this.tsTransform.Size = new System.Drawing.Size(219, 25);
            this.tsTransform.TabIndex = 2;
            // 
            // btnMove
            // 
            this.btnMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnMove.Image = ((System.Drawing.Image)(resources.GetObject("btnMove.Image")));
            this.btnMove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(41, 22);
            this.btnMove.Text = "Move";
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("btnCopy.Image")));
            this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(39, 22);
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnRotate
            // 
            this.btnRotate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRotate.Image = ((System.Drawing.Image)(resources.GetObject("btnRotate.Image")));
            this.btnRotate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(45, 22);
            this.btnRotate.Text = "Rotate";
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
            // 
            // btnScale
            // 
            this.btnScale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnScale.Image = ((System.Drawing.Image)(resources.GetObject("btnScale.Image")));
            this.btnScale.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScale.Name = "btnScale";
            this.btnScale.Size = new System.Drawing.Size(38, 22);
            this.btnScale.Text = "Scale";
            this.btnScale.Click += new System.EventHandler(this.btnScale_Click);
            // 
            // btnMirror
            // 
            this.btnMirror.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnMirror.Image = ((System.Drawing.Image)(resources.GetObject("btnMirror.Image")));
            this.btnMirror.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMirror.Name = "btnMirror";
            this.btnMirror.Size = new System.Drawing.Size(44, 22);
            this.btnMirror.Text = "Mirror";
            this.btnMirror.Click += new System.EventHandler(this.btnMirror_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoom,
            this.btnPan});
            this.toolStrip1.Location = new System.Drawing.Point(3, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(86, 25);
            this.toolStrip1.TabIndex = 4;
            // 
            // btnZoom
            // 
            this.btnZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnZoom.Image = ((System.Drawing.Image)(resources.GetObject("btnZoom.Image")));
            this.btnZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.Size = new System.Drawing.Size(43, 22);
            this.btnZoom.Text = "Zoom";
            this.btnZoom.Click += new System.EventHandler(this.btnZoom_Click);
            // 
            // btnPan
            // 
            this.btnPan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPan.Image = ((System.Drawing.Image)(resources.GetObject("btnPan.Image")));
            this.btnPan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPan.Name = "btnPan";
            this.btnPan.Size = new System.Drawing.Size(31, 22);
            this.btnPan.Text = "Pan";
            this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
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
            this.splitContainer1.Size = new System.Drawing.Size(1008, 394);
            this.splitContainer1.SplitterDistance = 759;
            this.splitContainer1.TabIndex = 2;
            // 
            // cadWindow1
            // 
            this.cadWindow1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(40)))), ((int)(((byte)(48)))));
            this.cadWindow1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.cadWindow1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.cadWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cadWindow1.Document = cadDocument2;
            this.cadWindow1.Location = new System.Drawing.Point(0, 0);
            this.cadWindow1.Name = "cadWindow1";
            this.cadWindow1.Size = new System.Drawing.Size(759, 394);
            this.cadWindow1.TabIndex = 0;
            this.cadWindow1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cadWindow1_MouseMove);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(245, 394);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // tsEdit
            // 
            this.tsEdit.Dock = System.Windows.Forms.DockStyle.None;
            this.tsEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDelete});
            this.tsEdit.Location = new System.Drawing.Point(3, 75);
            this.tsEdit.Name = "tsEdit";
            this.tsEdit.Size = new System.Drawing.Size(87, 25);
            this.tsEdit.TabIndex = 5;
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(44, 22);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tsStandard.ResumeLayout(false);
            this.tsStandard.PerformLayout();
            this.tsGraphics.ResumeLayout(false);
            this.tsGraphics.PerformLayout();
            this.tsPrimitives.ResumeLayout(false);
            this.tsPrimitives.PerformLayout();
            this.tsTransform.ResumeLayout(false);
            this.tsTransform.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tsEdit.ResumeLayout(false);
            this.tsEdit.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SimpleCAD.CADWindow cadWindow1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip tsPrimitives;
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
        private System.Windows.Forms.ToolStripButton btnDrawDimension;
        private System.Windows.Forms.ToolStripButton btnDrawParabola;
        private System.Windows.Forms.ToolStripButton btnDrawPolyline;
        private System.Windows.Forms.ToolStripButton btnDrawRectangle;
        private System.Windows.Forms.ToolStripButton btnDrawTriangle;
        private System.Windows.Forms.ToolStrip tsStandard;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStrip tsTransform;
        private System.Windows.Forms.ToolStripButton btnMove;
        private System.Windows.Forms.ToolStripButton btnRotate;
        private System.Windows.Forms.ToolStripButton btnScale;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripButton btnDrawHatch;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnMirror;
        private System.Windows.Forms.ToolStripButton btnSaveAs;
        private System.Windows.Forms.ToolStrip tsGraphics;
        private System.Windows.Forms.ToolStripLabel lblRenderer;
        private System.Windows.Forms.ToolStripComboBox btnRenderer;
        private System.Windows.Forms.ToolStripButton btnShowGrid;
        private System.Windows.Forms.ToolStripButton btnShowAxes;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnZoom;
        private System.Windows.Forms.ToolStripButton btnPan;
        private System.Windows.Forms.ToolStrip tsEdit;
        private System.Windows.Forms.ToolStripButton btnDelete;
    }
}

