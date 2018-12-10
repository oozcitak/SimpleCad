using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SimpleCADTest
{
    public partial class MainForm : Form
    {
        private CheckBox btnShowGrid;
        private CheckBox btnShowAxes;
        private ToolStripControlHost tsShowGrid;
        private ToolStripControlHost tsShowAxes;

        private SimpleCAD.CADDocument doc;
        private SimpleCAD.Editor ed;

        public MainForm()
        {
            InitializeComponent();

            doc = cadWindow1.Document;
            ed = doc.Editor;

            doc.DocumentChanged += doc_DocumentChanged;
            doc.SelectionChanged += doc_SelectionChanged;
            cadWindow1.MouseMove += cadWindow1_MouseMove;

            btnShowGrid = new CheckBox();
            btnShowGrid.Appearance = Appearance.Button;
            btnShowGrid.Image = Properties.Resources.grid;
            btnShowGrid.Text = "Grid";
            btnShowGrid.TextImageRelation = TextImageRelation.ImageBeforeText;
            tsShowGrid = new ToolStripControlHost(btnShowGrid);
            tsShowGrid.Click += btnShowGrid_Click;
            statusStrip1.Items.Add(tsShowGrid);

            btnShowAxes = new CheckBox();
            btnShowAxes.Appearance = Appearance.Button;
            btnShowAxes.Image = Properties.Resources.axis;
            btnShowAxes.Text = "Axes";
            btnShowAxes.TextImageRelation = TextImageRelation.ImageBeforeText;
            tsShowAxes = new ToolStripControlHost(btnShowAxes);
            tsShowAxes.Click += btnShowAxes_Click;
            statusStrip1.Items.Add(tsShowAxes);

            UpdateUI();
        }

        private void doc_DocumentChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObjects = ed.PickedSelection.ToArray();
        }

        private void doc_SelectionChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObjects = ed.PickedSelection.ToArray();

            UpdateUI();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            cadWindow1.Refresh();
        }

        private void cadWindow1_MouseMove(object sender, MouseEventArgs e)
        {
            statusCoords.Text = cadWindow1.View.CursorLocation.ToString(doc.Settings.NumberFormat);
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!EnsureDocumentSaved())
                e.Cancel = true;
        }

        private bool EnsureDocumentSaved()
        {
            if (!doc.IsModified)
                return true;

            DialogResult res = MessageBox.Show(
                "Do you want to save the changes to the document?",
                "SimpleCAD Test", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (res == DialogResult.Cancel)
                return false;
            else if (res == DialogResult.No)
                return true;
            else
            {
                ed.RunCommand("Document.Save");
                return !doc.IsModified;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (EnsureDocumentSaved())
                ed.RunCommand("Document.New");
            UpdateUI();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (EnsureDocumentSaved())
                ed.RunCommand("Document.Open", SaveFileName);
            UpdateUI();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(doc.FileName))
                ed.RunCommand("Document.SaveAs", SaveFileName);
            else
                ed.RunCommand("Document.Save");
            UpdateUI();
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Document.SaveAs", doc.FileName ?? SaveFileName);
            UpdateUI();
        }

        private string SaveFileName
        {
            get
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                return Path.Combine(path, "save.scf");
            }
        }

        private void UpdateUI()
        {
            btnSnap.Checked = doc.Settings.Snap;
            btnSnapEnd.Checked = (doc.Settings.SnapMode & SimpleCAD.SnapPointType.End) != SimpleCAD.SnapPointType.None;
            btnSnapMiddle.Checked = (doc.Settings.SnapMode & SimpleCAD.SnapPointType.Middle) != SimpleCAD.SnapPointType.None;
            btnSnapCenter.Checked = (doc.Settings.SnapMode & SimpleCAD.SnapPointType.Center) != SimpleCAD.SnapPointType.None;
            btnSnapQuadrant.Checked = (doc.Settings.SnapMode & SimpleCAD.SnapPointType.Quadrant) != SimpleCAD.SnapPointType.None;
            btnSnapPoint.Checked = (doc.Settings.SnapMode & SimpleCAD.SnapPointType.Point) != SimpleCAD.SnapPointType.None;

            btnAngleRadians.Checked = (doc.Settings.AngleMode == SimpleCAD.AngleMode.Radians);
            btnAngleDegrees.Checked = (doc.Settings.AngleMode == SimpleCAD.AngleMode.Degrees);
            btnAngleGrads.Checked = (doc.Settings.AngleMode == SimpleCAD.AngleMode.Grads);
            btnAngleDMS.Checked = (doc.Settings.AngleMode == SimpleCAD.AngleMode.DegreesMinutesSeconds);
            btnAngleSurveyor.Checked = (doc.Settings.AngleMode == SimpleCAD.AngleMode.Surveyor);

            btnAngleMode.Text =
                (doc.Settings.AngleMode == SimpleCAD.AngleMode.Radians ? "Radians" :
                doc.Settings.AngleMode == SimpleCAD.AngleMode.Degrees ? "Degrees" :
                doc.Settings.AngleMode == SimpleCAD.AngleMode.Grads ? "Grads" :
                doc.Settings.AngleMode == SimpleCAD.AngleMode.DegreesMinutesSeconds ? "Degrees/Minutes/Seconds" :
                doc.Settings.AngleMode == SimpleCAD.AngleMode.Surveyor ? "Surveyor" : "<Unkown>");

            btnSnapMode.Text = "Snap " + (doc.Settings.Snap ? "" : "(Off)") + ":" +
                (doc.Settings.SnapMode == SimpleCAD.SnapPointType.None ? " None" :
                (((doc.Settings.SnapMode & SimpleCAD.SnapPointType.End) != SimpleCAD.SnapPointType.None) ? " E" : "") +
                (((doc.Settings.SnapMode & SimpleCAD.SnapPointType.Middle) != SimpleCAD.SnapPointType.None) ? " M" : "") +
                (((doc.Settings.SnapMode & SimpleCAD.SnapPointType.Center) != SimpleCAD.SnapPointType.None) ? " C" : "") +
                (((doc.Settings.SnapMode & SimpleCAD.SnapPointType.Quadrant) != SimpleCAD.SnapPointType.None) ? " Q" : "") +
                (((doc.Settings.SnapMode & SimpleCAD.SnapPointType.Point) != SimpleCAD.SnapPointType.None) ? " P" : ""));

            btnShowGrid.Checked = cadWindow1.View.ShowGrid;
            btnShowAxes.Checked = cadWindow1.View.ShowAxes;

            if (ed.PickedSelection.Count == 0)
                lblSelection.Text = "No selection";
            else if (ed.PickedSelection.Count == 1)
                lblSelection.Text = ed.PickedSelection.First().GetType().Name;
            else
                lblSelection.Text = ed.PickedSelection.Count.ToString() + " selected";
        }

        private void btnDrawPoint_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Point");
        }

        private void btnDrawLine_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Line");
        }

        private void btnDrawArc_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Arc");
        }

        private void btnDrawCircle_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Circle");
        }

        private void btnDrawEllipse_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Ellipse");
        }

        private void btnDrawEllipticArc_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Elliptic_Arc");
        }

        private void btnDrawText_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Text");
        }

        private void btnDrawDimension_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Dimension");
        }

        private void btnDrawParabola_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Parabola");
        }

        private void btnDrawPolyline_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Polyline");
        }

        private void btnDrawHatch_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Hatch");
        }

        private void btnDrawRectangle_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Rectangle");
        }

        private void btnDrawQuadraticBezier_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Primitives.Quadratic_Bezier");
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Transform.Move");
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Transform.Copy");
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Transform.Rotate");
        }

        private void btnScale_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Transform.Scale");
        }

        private void btnMirror_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Transform.Mirror");
        }

        private void btnStretch_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Transform.MoveControlPoints");
        }

        private void btnRotateCP_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Transform.RotateControlPoints");
        }

        private void btnScaleCP_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Transform.ScaleControlPoints");
        }

        private void btnZoom_Click(object sender, EventArgs e)
        {
            ed.RunCommand("View.Zoom");
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            ed.RunCommand("View.Pan");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Edit.Delete");
        }

        private void btnCreateComposite_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Composite.Create");
        }

        private void btnAngleDegrees_Click(object sender, EventArgs e)
        {
            doc.Settings.AngleMode = SimpleCAD.AngleMode.Degrees;
            UpdateUI();
        }

        private void btnAngleRadians_Click(object sender, EventArgs e)
        {
            doc.Settings.AngleMode = SimpleCAD.AngleMode.Radians;
            UpdateUI();
        }

        private void btnAngleGrads_Click(object sender, EventArgs e)
        {
            doc.Settings.AngleMode = SimpleCAD.AngleMode.Grads;
            UpdateUI();
        }

        private void btnAngleDMS_Click(object sender, EventArgs e)
        {
            doc.Settings.AngleMode = SimpleCAD.AngleMode.DegreesMinutesSeconds;
            UpdateUI();
        }

        private void btnAngleSurveyor_Click(object sender, EventArgs e)
        {
            doc.Settings.AngleMode = SimpleCAD.AngleMode.Surveyor;
            UpdateUI();
        }

        private void btnSnap_Click(object sender, EventArgs e)
        {
            doc.Settings.Snap = btnSnap.Checked;
            UpdateUI();
        }

        private void btnSnapEnd_Click(object sender, EventArgs e)
        {
            if (btnSnapEnd.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.End;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.End;
            UpdateUI();
        }

        private void btnSnapMiddle_Click(object sender, EventArgs e)
        {
            if (btnSnapMiddle.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.Middle;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.Middle;
            UpdateUI();
        }

        private void btnSnapCenter_Click(object sender, EventArgs e)
        {
            if (btnSnapCenter.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.Center;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.Center;
            UpdateUI();
        }

        private void btnSnapQuadrant_Click(object sender, EventArgs e)
        {
            if (btnSnapQuadrant.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.Quadrant;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.Quadrant;
            UpdateUI();
        }

        private void btnSnapPoint_Click(object sender, EventArgs e)
        {
            if (btnSnapPoint.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.Point;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.Point;
            UpdateUI();
        }

        private void btnShowGrid_Click(object sender, EventArgs e)
        {
            cadWindow1.View.ShowGrid = !cadWindow1.View.ShowGrid;
            cadWindow1.Focus();
            UpdateUI();
        }

        private void btnShowAxes_Click(object sender, EventArgs e)
        {
            cadWindow1.View.ShowAxes = !cadWindow1.View.ShowAxes;
            cadWindow1.Focus();
            UpdateUI();
        }
    }
}
