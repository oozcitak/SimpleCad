using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SimpleCADTest
{
    public partial class MainForm : Form
    {
        private SimpleCAD.CADDocument doc;
        private SimpleCAD.Editor ed;

        public MainForm()
        {
            InitializeComponent();

            doc = cadWindow1.Document;
            ed = doc.Editor;

            doc.DocumentChanged += Document_DocumentChanged;
            doc.SelectionChanged += CadWindow1_SelectionChanged;
            cadWindow1.MouseMove += cadWindow1_MouseMove;

            UpdateUI();
        }

        private void Document_DocumentChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObjects = ed.PickedSelection.ToArray();
        }

        private void CadWindow1_SelectionChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObjects = ed.PickedSelection.ToArray();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            cadWindow1.Refresh();
        }

        private void cadWindow1_MouseMove(object sender, MouseEventArgs e)
        {
            statusCoords.Text = cadWindow1.View.CursorLocation.ToString(doc.Settings.NumberFormat);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void btnShowGrid_Click(object sender, EventArgs e)
        {
            cadWindow1.View.ShowGrid = btnShowGrid.Checked;
        }

        private void btnShowAxes_Click(object sender, EventArgs e)
        {
            cadWindow1.View.ShowAxes = btnShowAxes.Checked;
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

        private void btnSnap_Click(object sender, EventArgs e)
        {
            doc.Settings.Snap = btnSnap.Checked;
        }

        private void btnSnapEnd_Click(object sender, EventArgs e)
        {
            if (btnSnapEnd.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.End;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.End;
        }

        private void btnSnapMiddle_Click(object sender, EventArgs e)
        {
            if (btnSnapMiddle.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.Middle;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.Middle;
        }

        private void btnSnapCenter_Click(object sender, EventArgs e)
        {
            if (btnSnapCenter.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.Center;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.Center;
        }

        private void btnSnapQuadrant_Click(object sender, EventArgs e)
        {
            if (btnSnapQuadrant.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.Quadrant;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.Quadrant;
        }

        private void btnSnapPoint_Click(object sender, EventArgs e)
        {
            if (btnSnapPoint.Checked)
                doc.Settings.SnapMode |= SimpleCAD.SnapPointType.Point;
            else
                doc.Settings.SnapMode &= ~SimpleCAD.SnapPointType.Point;
        }

        private void btnCreateComposite_Click(object sender, EventArgs e)
        {
            ed.RunCommand("Composite.Create");
        }
    }
}
