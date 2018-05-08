using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SimpleCADTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            cadWindow1.Document.DocumentChanged += Document_DocumentChanged;
            cadWindow1.Document.SelectionChanged += CadWindow1_SelectionChanged;
            cadWindow1.MouseMove += cadWindow1_MouseMove;

            UpdateUI();
        }

        private void Document_DocumentChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObjects = cadWindow1.Document.Editor.PickedSelection.ToArray();
        }

        private void CadWindow1_SelectionChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObjects = cadWindow1.Document.Editor.PickedSelection.ToArray();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            cadWindow1.Refresh();
        }

        private void cadWindow1_MouseMove(object sender, MouseEventArgs e)
        {
            statusCoords.Text = cadWindow1.View.CursorLocation.ToString(cadWindow1.Document.Settings.NumberFormat);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!EnsureDocumentSaved())
                e.Cancel = true;
        }

        private bool EnsureDocumentSaved()
        {
            if (!cadWindow1.Document.IsModified)
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
                cadWindow1.Document.Editor.RunCommand("Document.Save");
                return !cadWindow1.Document.IsModified;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (EnsureDocumentSaved())
                cadWindow1.Document.Editor.RunCommand("Document.New");
            UpdateUI();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (EnsureDocumentSaved())
                cadWindow1.Document.Editor.RunCommand("Document.Open", SaveFileName);
            UpdateUI();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cadWindow1.Document.FileName))
                cadWindow1.Document.Editor.RunCommand("Document.SaveAs", SaveFileName);
            else
                cadWindow1.Document.Editor.RunCommand("Document.Save");
            UpdateUI();
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Document.SaveAs", cadWindow1.Document.FileName ?? SaveFileName);
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
            btnSnap.Checked = cadWindow1.Document.Settings.Get<bool>("Snap");
            btnSnapEnd.Checked = (cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & SimpleCAD.SnapPointType.End) != SimpleCAD.SnapPointType.None;
            btnSnapMiddle.Checked = (cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & SimpleCAD.SnapPointType.Middle) != SimpleCAD.SnapPointType.None;
            btnSnapCenter.Checked = (cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & SimpleCAD.SnapPointType.Center) != SimpleCAD.SnapPointType.None;
            btnSnapQuadrant.Checked = (cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & SimpleCAD.SnapPointType.Quadrant) != SimpleCAD.SnapPointType.None;
            btnSnapPoint.Checked = (cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & SimpleCAD.SnapPointType.Point) != SimpleCAD.SnapPointType.None;
        }

        private void btnDrawPoint_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Point");
        }

        private void btnDrawLine_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Line");
        }

        private void btnDrawArc_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Arc");
        }

        private void btnDrawCircle_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Circle");
        }

        private void btnDrawEllipse_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Ellipse");
        }

        private void btnDrawEllipticArc_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Elliptic_Arc");
        }

        private void btnDrawText_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Text");
        }

        private void btnDrawDimension_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Dimension");
        }

        private void btnDrawParabola_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Parabola");
        }

        private void btnDrawPolyline_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Polyline");
        }

        private void btnDrawHatch_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Hatch");
        }

        private void btnDrawRectangle_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Rectangle");
        }

        private void btnDrawTriangle_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Triangle");
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Transform.Move");
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Transform.Copy");
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Transform.Rotate");
        }

        private void btnScale_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Transform.Scale");
        }

        private void btnMirror_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Transform.Mirror");
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
            cadWindow1.Document.Editor.RunCommand("View.Zoom");
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("View.Pan");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Edit.Delete");
        }

        private void btnSnap_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Settings.Set("Snap", btnSnap.Checked);
        }

        private void btnSnapEnd_Click(object sender, EventArgs e)
        {
            if (btnSnapEnd.Checked)
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") | SimpleCAD.SnapPointType.End);
            else
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & ~SimpleCAD.SnapPointType.End);
        }

        private void btnSnapMiddle_Click(object sender, EventArgs e)
        {
            if (btnSnapMiddle.Checked)
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") | SimpleCAD.SnapPointType.Middle);
            else
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & ~SimpleCAD.SnapPointType.Middle);
        }

        private void btnSnapCenter_Click(object sender, EventArgs e)
        {
            if (btnSnapCenter.Checked)
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") | SimpleCAD.SnapPointType.Center);
            else
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & ~SimpleCAD.SnapPointType.Center);
        }

        private void btnSnapQuadrant_Click(object sender, EventArgs e)
        {
            if (btnSnapQuadrant.Checked)
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") | SimpleCAD.SnapPointType.Quadrant);
            else
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & ~SimpleCAD.SnapPointType.Quadrant);
        }

        private void btnSnapPoint_Click(object sender, EventArgs e)
        {
            if (btnSnapPoint.Checked)
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") | SimpleCAD.SnapPointType.Point);
            else
                cadWindow1.Document.Settings.Set("SnapMode", cadWindow1.Document.Settings.Get<SimpleCAD.SnapPointType>("SnapMode") & ~SimpleCAD.SnapPointType.Point);
        }
    }
}
