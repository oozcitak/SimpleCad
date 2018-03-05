using SimpleCAD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleCADTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            cadWindow1.Document.SelectionChanged += CadWindow1_SelectionChanged;
            cadWindow1.Document.Editor.Prompt += Editor_Prompt;
        }

        private void Editor_Prompt(object sender, EditorPromptEventArgs e)
        {
            statusLabel.Text = string.IsNullOrEmpty(e.Status) ? "Ready" : e.Status;
        }

        private void CadWindow1_SelectionChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObjects = cadWindow1.Document.Editor.Selection.ToArray();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            cadWindow1.Refresh();
        }

        private void cadWindow1_MouseMove(object sender, MouseEventArgs e)
        {
            Point2D pt = new Point2D(cadWindow1.View.ScreenToWorld(e.X, e.Y));
            statusCoords.Text = pt.X.ToString("F2") + ", " + pt.Y.ToString("F2");
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Open(SaveFileName);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Save(SaveFileName);
        }

        private string SaveFileName
        {
            get
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                return Path.Combine(path, "save.bin");
            }
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
    }
}
