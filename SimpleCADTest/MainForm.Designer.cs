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
            this.cadWindow1 = new SimpleCAD.CADWindow();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.tsPrimitives = new System.Windows.Forms.ToolStrip();
            this.btnDrawPoint = new System.Windows.Forms.ToolStripButton();
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
            this.tsStandard = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveAs = new System.Windows.Forms.ToolStripButton();
            this.tsEdit = new System.Windows.Forms.ToolStrip();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnZoom = new System.Windows.Forms.ToolStripButton();
            this.btnPan = new System.Windows.Forms.ToolStripButton();
            this.tsGraphics = new System.Windows.Forms.ToolStrip();
            this.btnShowGrid = new System.Windows.Forms.ToolStripButton();
            this.btnShowAxes = new System.Windows.Forms.ToolStripButton();
            this.tsTransform = new System.Windows.Forms.ToolStrip();
            this.btnMove = new System.Windows.Forms.ToolStripButton();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnRotate = new System.Windows.Forms.ToolStripButton();
            this.btnScale = new System.Windows.Forms.ToolStripButton();
            this.btnMirror = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnStretch = new System.Windows.Forms.ToolStripButton();
            this.btnRotateCP = new System.Windows.Forms.ToolStripButton();
            this.btnScaleCP = new System.Windows.Forms.ToolStripButton();
            this.tsSnap = new System.Windows.Forms.ToolStrip();
            this.btnSnap = new System.Windows.Forms.ToolStripButton();
            this.btnSnapEnd = new System.Windows.Forms.ToolStripButton();
            this.btnSnapMiddle = new System.Windows.Forms.ToolStripButton();
            this.btnSnapCenter = new System.Windows.Forms.ToolStripButton();
            this.btnSnapQuadrant = new System.Windows.Forms.ToolStripButton();
            this.btnSnapPoint = new System.Windows.Forms.ToolStripButton();
            this.btnDrawQuadraticBezier = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tsPrimitives.SuspendLayout();
            this.tsStandard.SuspendLayout();
            this.tsEdit.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tsGraphics.SuspendLayout();
            this.tsTransform.SuspendLayout();
            this.tsSnap.SuspendLayout();
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
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1008, 344);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1008, 516);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsPrimitives);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsStandard);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsEdit);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsGraphics);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsTransform);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsSnap);
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
            this.splitContainer1.Size = new System.Drawing.Size(1008, 344);
            this.splitContainer1.SplitterDistance = 759;
            this.splitContainer1.TabIndex = 2;
            // 
            // cadWindow1
            // 
            this.cadWindow1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.cadWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cadWindow1.Location = new System.Drawing.Point(0, 0);
            this.cadWindow1.Name = "cadWindow1";
            this.cadWindow1.Size = new System.Drawing.Size(759, 344);
            this.cadWindow1.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(245, 344);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // tsPrimitives
            // 
            this.tsPrimitives.Dock = System.Windows.Forms.DockStyle.None;
            this.tsPrimitives.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDrawPoint,
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
            this.btnDrawHatch,
            this.btnDrawQuadraticBezier});
            this.tsPrimitives.Location = new System.Drawing.Point(3, 0);
            this.tsPrimitives.Name = "tsPrimitives";
            this.tsPrimitives.Size = new System.Drawing.Size(762, 25);
            this.tsPrimitives.TabIndex = 0;
            // 
            // btnDrawPoint
            // 
            this.btnDrawPoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawPoint.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawPoint.Image")));
            this.btnDrawPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawPoint.Name = "btnDrawPoint";
            this.btnDrawPoint.Size = new System.Drawing.Size(39, 22);
            this.btnDrawPoint.Text = "Point";
            this.btnDrawPoint.Click += new System.EventHandler(this.btnDrawPoint_Click);
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
            // tsStandard
            // 
            this.tsStandard.Dock = System.Windows.Forms.DockStyle.None;
            this.tsStandard.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.btnSaveAs});
            this.tsStandard.Location = new System.Drawing.Point(3, 25);
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
            // tsEdit
            // 
            this.tsEdit.Dock = System.Windows.Forms.DockStyle.None;
            this.tsEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDelete});
            this.tsEdit.Location = new System.Drawing.Point(3, 50);
            this.tsEdit.Name = "tsEdit";
            this.tsEdit.Size = new System.Drawing.Size(56, 25);
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
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoom,
            this.btnPan});
            this.toolStrip1.Location = new System.Drawing.Point(3, 75);
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
            // tsGraphics
            // 
            this.tsGraphics.Dock = System.Windows.Forms.DockStyle.None;
            this.tsGraphics.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnShowGrid,
            this.btnShowAxes});
            this.tsGraphics.Location = new System.Drawing.Point(89, 75);
            this.tsGraphics.Name = "tsGraphics";
            this.tsGraphics.Size = new System.Drawing.Size(144, 25);
            this.tsGraphics.TabIndex = 3;
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
            // tsTransform
            // 
            this.tsTransform.Dock = System.Windows.Forms.DockStyle.None;
            this.tsTransform.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnMove,
            this.btnCopy,
            this.btnRotate,
            this.btnScale,
            this.btnMirror,
            this.toolStripSeparator1,
            this.btnStretch,
            this.btnRotateCP,
            this.btnScaleCP});
            this.tsTransform.Location = new System.Drawing.Point(3, 100);
            this.tsTransform.Name = "tsTransform";
            this.tsTransform.Size = new System.Drawing.Size(514, 25);
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnStretch
            // 
            this.btnStretch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStretch.Image = ((System.Drawing.Image)(resources.GetObject("btnStretch.Image")));
            this.btnStretch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStretch.Name = "btnStretch";
            this.btnStretch.Size = new System.Drawing.Size(48, 22);
            this.btnStretch.Text = "Stretch";
            this.btnStretch.Click += new System.EventHandler(this.btnStretch_Click);
            // 
            // btnRotateCP
            // 
            this.btnRotateCP.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRotateCP.Image = ((System.Drawing.Image)(resources.GetObject("btnRotateCP.Image")));
            this.btnRotateCP.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRotateCP.Name = "btnRotateCP";
            this.btnRotateCP.Size = new System.Drawing.Size(124, 22);
            this.btnRotateCP.Text = "Rotate Control Points";
            this.btnRotateCP.Click += new System.EventHandler(this.btnRotateCP_Click);
            // 
            // btnScaleCP
            // 
            this.btnScaleCP.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnScaleCP.Image = ((System.Drawing.Image)(resources.GetObject("btnScaleCP.Image")));
            this.btnScaleCP.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScaleCP.Name = "btnScaleCP";
            this.btnScaleCP.Size = new System.Drawing.Size(117, 22);
            this.btnScaleCP.Text = "Scale Control Points";
            this.btnScaleCP.Click += new System.EventHandler(this.btnScaleCP_Click);
            // 
            // tsSnap
            // 
            this.tsSnap.Dock = System.Windows.Forms.DockStyle.None;
            this.tsSnap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSnap,
            this.btnSnapEnd,
            this.btnSnapMiddle,
            this.btnSnapCenter,
            this.btnSnapQuadrant,
            this.btnSnapPoint});
            this.tsSnap.Location = new System.Drawing.Point(3, 125);
            this.tsSnap.Name = "tsSnap";
            this.tsSnap.Size = new System.Drawing.Size(274, 25);
            this.tsSnap.TabIndex = 6;
            // 
            // btnSnap
            // 
            this.btnSnap.CheckOnClick = true;
            this.btnSnap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSnap.Image = ((System.Drawing.Image)(resources.GetObject("btnSnap.Image")));
            this.btnSnap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSnap.Name = "btnSnap";
            this.btnSnap.Size = new System.Drawing.Size(37, 22);
            this.btnSnap.Text = "Snap";
            this.btnSnap.Click += new System.EventHandler(this.btnSnap_Click);
            // 
            // btnSnapEnd
            // 
            this.btnSnapEnd.CheckOnClick = true;
            this.btnSnapEnd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSnapEnd.Image = ((System.Drawing.Image)(resources.GetObject("btnSnapEnd.Image")));
            this.btnSnapEnd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSnapEnd.Name = "btnSnapEnd";
            this.btnSnapEnd.Size = new System.Drawing.Size(31, 22);
            this.btnSnapEnd.Text = "End";
            this.btnSnapEnd.Click += new System.EventHandler(this.btnSnapEnd_Click);
            // 
            // btnSnapMiddle
            // 
            this.btnSnapMiddle.CheckOnClick = true;
            this.btnSnapMiddle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSnapMiddle.Image = ((System.Drawing.Image)(resources.GetObject("btnSnapMiddle.Image")));
            this.btnSnapMiddle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSnapMiddle.Name = "btnSnapMiddle";
            this.btnSnapMiddle.Size = new System.Drawing.Size(48, 22);
            this.btnSnapMiddle.Text = "Middle";
            this.btnSnapMiddle.Click += new System.EventHandler(this.btnSnapMiddle_Click);
            // 
            // btnSnapCenter
            // 
            this.btnSnapCenter.CheckOnClick = true;
            this.btnSnapCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSnapCenter.Image = ((System.Drawing.Image)(resources.GetObject("btnSnapCenter.Image")));
            this.btnSnapCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSnapCenter.Name = "btnSnapCenter";
            this.btnSnapCenter.Size = new System.Drawing.Size(46, 22);
            this.btnSnapCenter.Text = "Center";
            this.btnSnapCenter.Click += new System.EventHandler(this.btnSnapCenter_Click);
            // 
            // btnSnapQuadrant
            // 
            this.btnSnapQuadrant.CheckOnClick = true;
            this.btnSnapQuadrant.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSnapQuadrant.Image = ((System.Drawing.Image)(resources.GetObject("btnSnapQuadrant.Image")));
            this.btnSnapQuadrant.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSnapQuadrant.Name = "btnSnapQuadrant";
            this.btnSnapQuadrant.Size = new System.Drawing.Size(61, 22);
            this.btnSnapQuadrant.Text = "Quadrant";
            this.btnSnapQuadrant.Click += new System.EventHandler(this.btnSnapQuadrant_Click);
            // 
            // btnSnapPoint
            // 
            this.btnSnapPoint.CheckOnClick = true;
            this.btnSnapPoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSnapPoint.Image = ((System.Drawing.Image)(resources.GetObject("btnSnapPoint.Image")));
            this.btnSnapPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSnapPoint.Name = "btnSnapPoint";
            this.btnSnapPoint.Size = new System.Drawing.Size(39, 22);
            this.btnSnapPoint.Text = "Point";
            this.btnSnapPoint.Click += new System.EventHandler(this.btnSnapPoint_Click);
            // 
            // btnDrawQuadraticBezier
            // 
            this.btnDrawQuadraticBezier.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDrawQuadraticBezier.Image = ((System.Drawing.Image)(resources.GetObject("btnDrawQuadraticBezier.Image")));
            this.btnDrawQuadraticBezier.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDrawQuadraticBezier.Name = "btnDrawQuadraticBezier";
            this.btnDrawQuadraticBezier.Size = new System.Drawing.Size(97, 22);
            this.btnDrawQuadraticBezier.Text = "Quadratic Bezier";
            this.btnDrawQuadraticBezier.Click += new System.EventHandler(this.btnDrawQuadraticBezier_Click);
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
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tsPrimitives.ResumeLayout(false);
            this.tsPrimitives.PerformLayout();
            this.tsStandard.ResumeLayout(false);
            this.tsStandard.PerformLayout();
            this.tsEdit.ResumeLayout(false);
            this.tsEdit.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tsGraphics.ResumeLayout(false);
            this.tsGraphics.PerformLayout();
            this.tsTransform.ResumeLayout(false);
            this.tsTransform.PerformLayout();
            this.tsSnap.ResumeLayout(false);
            this.tsSnap.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
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
        private System.Windows.Forms.ToolStripButton btnShowGrid;
        private System.Windows.Forms.ToolStripButton btnShowAxes;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnZoom;
        private System.Windows.Forms.ToolStripButton btnPan;
        private System.Windows.Forms.ToolStrip tsEdit;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private SimpleCAD.CADWindow cadWindow1;
        private System.Windows.Forms.ToolStripButton btnDrawPoint;
        private System.Windows.Forms.ToolStrip tsSnap;
        private System.Windows.Forms.ToolStripButton btnSnapEnd;
        private System.Windows.Forms.ToolStripButton btnSnapMiddle;
        private System.Windows.Forms.ToolStripButton btnSnapCenter;
        private System.Windows.Forms.ToolStripButton btnSnapQuadrant;
        private System.Windows.Forms.ToolStripButton btnSnapPoint;
        private System.Windows.Forms.ToolStripButton btnSnap;
        private System.Windows.Forms.ToolStripButton btnStretch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnRotateCP;
        private System.Windows.Forms.ToolStripButton btnScaleCP;
        private System.Windows.Forms.ToolStripButton btnDrawQuadraticBezier;
    }
}

