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
        Circle trPoint;

        public MainForm()
        {
            InitializeComponent();
            trPoint = new Circle(0, 0, 20);
            trPoint.Outline = new Outline(Color.Red, 3);
            cadWindow1.Document.Model.Add(trPoint);
            cadWindow1.Document.SelectionChanged += CadWindow1_SelectionChanged;
            cadWindow2.Document = cadWindow1.Document;

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

        private void cadWindow1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                trPoint.Center = new Point2D(cadWindow1.View.ScreenToWorld(e.X, e.Y));
                cadWindow1.Refresh();
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

        private void btnDrawRectangle_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Rectangle");
        }

        private void btnDrawTriangle_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Editor.RunCommand("Primitives.Triangle");
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(20, 0));
                    break;
                case Keys.Left:
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(-20, 0));
                    break;
                case Keys.Down:
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(0, -20));
                    break;
                case Keys.Up:
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(0, 20));
                    break;
                case Keys.PageDown:
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Rotation(-5 * (float)Math.PI / 180));
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                    break;
                case Keys.PageUp:
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Rotation(5 * (float)Math.PI / 180));
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                    break;
                case Keys.Home:
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Scale(0.8f, 0.8f));
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                    break;
                case Keys.End:
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Scale(1.2f, 1.2f));
                    TransformItems(cadWindow1.Document.Editor.Selection, TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                    break;
                case Keys.Delete:
                    Drawable[] toDelete = cadWindow1.Document.Editor.Selection.ToArray();
                    foreach (Drawable item in toDelete)
                    {
                        cadWindow1.Document.Model.Remove(item);
                    }
                    break;
            }
            propertyGrid1.Refresh();
            cadWindow1.Refresh();
        }

        private void TransformItems(IEnumerable<Drawable> items, TransformationMatrix2D trans)
        {
            foreach (Drawable item in items)
            {
                item.TransformBy(trans);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            cadWindow1.Document.Save(SaveFileName);
        }

        private void ReadFile()
        {
            cadWindow1.Document.Open(SaveFileName);
        }

        private string SaveFileName
        {
            get
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                return Path.Combine(path, "save.bin");
            }
        }
    }
}
