using SimpleCAD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleCADTest
{
    public partial class MainForm : Form
    {
        string currentCommand = "";
        int commandStep = 0;
        Point2D[] pickedPoints = new Point2D[20];
        Drawable newItem;
        Circle trPoint;

        public MainForm()
        {
            InitializeComponent();
            trPoint = new Circle(0, 0, 20);
            trPoint.FillStyle = FillStyle.Orange;
            trPoint.OutlineStyle = new OutlineStyle(Color.Red, 3);
            cadWindow1.Model.Add(trPoint);
            cadWindow1.SelectionChanged += CadWindow1_SelectionChanged;
        }

        private void CadWindow1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            propertyGrid1.SelectedObjects = e.SelectedItems.ToArray();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            cadWindow1.Refresh();
        }

        private void cadWindow1_MouseMove(object sender, MouseEventArgs e)
        {
            Point2D pt = new Point2D(cadWindow1.View.ScreenToWorld(e.X, e.Y));
            statusCoords.Text = pt.X.ToString("F2") + ", " + pt.Y.ToString("F2");

            if (!string.IsNullOrEmpty(currentCommand))
            {
                pickedPoints[commandStep] = pt;
                ApplyCommand();
            }
        }

        private void cadWindow1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentCommand) && e.Button == MouseButtons.Left)
            {
                commandStep++;
                ApplyCommand();
            }
            else if (e.Button == MouseButtons.Right)
            {
                trPoint.Center = new Point2D(cadWindow1.View.ScreenToWorld(e.X, e.Y));
                cadWindow1.Refresh();
            }
        }

        private void ApplyCommand()
        {
            if (currentCommand == "LINE" && commandStep == 0)
            {
                newItem = new Line(pickedPoints[0], pickedPoints[0]);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
                commandStep++;
            }
            else if (currentCommand == "LINE" && commandStep == 1)
            {
                statusLabel.Text = "Select start point of line";
                Line line = newItem as Line;
                line.P1 = pickedPoints[1];
                line.P2 = pickedPoints[1];
                cadWindow1.Refresh();
            }
            else if (currentCommand == "LINE" && commandStep == 2)
            {
                statusLabel.Text = "Select end point of line";
                Line line = newItem as Line;
                line.P2 = pickedPoints[2];
                cadWindow1.Refresh();
            }
            else if (currentCommand == "LINE" && commandStep == 3)
            {
                ResetCommand();
            }
            else if (currentCommand == "ARC" && commandStep == 0)
            {
                newItem = new Arc(pickedPoints[0], 1, 0, 2 * (float)Math.PI);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
                commandStep++;
            }
            else if (currentCommand == "ARC" && commandStep == 1)
            {
                statusLabel.Text = "Select center point of arc";
                Arc arc = newItem as Arc;
                arc.Center = pickedPoints[1];
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ARC" && commandStep == 2)
            {
                statusLabel.Text = "Select radius of arc";
                Arc arc = newItem as Arc;
                arc.Radius = (pickedPoints[2] - pickedPoints[1]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ARC" && commandStep == 3)
            {
                statusLabel.Text = "Select start angle of arc";
                Arc arc = newItem as Arc;
                arc.StartAngle = (pickedPoints[3] - pickedPoints[1]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ARC" && commandStep == 4)
            {
                statusLabel.Text = "Select end angle of arc";
                Arc arc = newItem as Arc;
                arc.EndAngle = (pickedPoints[4] - pickedPoints[1]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ARC" && commandStep == 5)
            {
                ResetCommand();
            }
            else if (currentCommand == "CIRCLE" && commandStep == 0)
            {
                newItem = new Circle(pickedPoints[0], 1);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
                commandStep++;
            }
            else if (currentCommand == "CIRCLE" && commandStep == 1)
            {
                statusLabel.Text = "Select center point of circle";
                Circle cir = newItem as Circle;
                cir.Center = pickedPoints[1];
                cadWindow1.Refresh();
            }
            else if (currentCommand == "CIRCLE" && commandStep == 2)
            {
                statusLabel.Text = "Select radius of circle";
                Circle cir = newItem as Circle;
                cir.Radius = (pickedPoints[2] - pickedPoints[1]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "CIRCLE" && commandStep == 3)
            {
                ResetCommand();
            }
            else if (currentCommand == "ELLIPSE" && commandStep == 0)
            {
                newItem = new Ellipse(pickedPoints[0], 1, 1);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
                commandStep++;
            }
            else if (currentCommand == "ELLIPSE" && commandStep == 1)
            {
                statusLabel.Text = "Select center point of ellipse";
                Ellipse eli = newItem as Ellipse;
                eli.Center = pickedPoints[1];
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPSE" && commandStep == 2)
            {
                statusLabel.Text = "Select semi-major axis of ellipse";
                Ellipse eli = newItem as Ellipse;
                eli.SemiMajorAxis = (pickedPoints[2] - pickedPoints[1]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPSE" && commandStep == 3)
            {
                statusLabel.Text = "Select semi-minor axis of ellipse";
                Ellipse eli = newItem as Ellipse;
                eli.SemiMinorAxis = (pickedPoints[3] - pickedPoints[1]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPSE" && commandStep == 4)
            {
                ResetCommand();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 0)
            {
                newItem = new EllipticArc(pickedPoints[0], 1, 1, 0, 2 * (float)Math.PI);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
                commandStep++;
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 1)
            {
                statusLabel.Text = "Select center point of elliptic arc";
                EllipticArc eli = newItem as EllipticArc;
                eli.Center = pickedPoints[1];
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 2)
            {
                statusLabel.Text = "Select semi-major axis of elliptic arc";
                EllipticArc eli = newItem as EllipticArc;
                eli.SemiMajorAxis = (pickedPoints[2] - pickedPoints[1]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 3)
            {
                statusLabel.Text = "Select semi-minor axis of elliptic arc";
                EllipticArc eli = newItem as EllipticArc;
                eli.SemiMinorAxis = (pickedPoints[3] - pickedPoints[1]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 4)
            {
                statusLabel.Text = "Select start angle of elliptic arc";
                EllipticArc arc = newItem as EllipticArc;
                arc.StartAngle = (pickedPoints[4] - pickedPoints[1]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 5)
            {
                statusLabel.Text = "Select end angle of elliptic arc";
                EllipticArc arc = newItem as EllipticArc;
                arc.EndAngle = (pickedPoints[5] - pickedPoints[1]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 6)
            {
                ResetCommand();
            }
            else if (currentCommand == "TEXT" && commandStep == 0)
            {
                newItem = new Text(pickedPoints[0], "abc", 1);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
                commandStep++;
            }
            else if (currentCommand == "TEXT" && commandStep == 1)
            {
                statusLabel.Text = "Select insertion point of text";
                Text txt = newItem as Text;
                txt.P = pickedPoints[1];
                cadWindow1.Refresh();
            }
            else if (currentCommand == "TEXT" && commandStep == 2)
            {
                statusLabel.Text = "Select text rotation";
                Text txt = newItem as Text;
                txt.Rotation = (pickedPoints[2] - pickedPoints[1]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "TEXT" && commandStep == 3)
            {
                statusLabel.Text = "Select text height";
                Text txt = newItem as Text;
                txt.Height = (pickedPoints[3] - pickedPoints[1]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "TEXT" && commandStep == 4)
            {
                ResetCommand();
            }
        }

        private void BeginCommand(string name)
        {
            currentCommand = name;
            commandStep = 0;
            ApplyCommand();
        }

        private void ResetCommand()
        {
            currentCommand = "";
            commandStep = 0;
            statusLabel.Text = "Ready";
            newItem = null;
            cadWindow1.Refresh();
        }

        private void btnDrawLine_Click(object sender, EventArgs e)
        {
            BeginCommand("LINE");
        }

        private void btnDrawArc_Click(object sender, EventArgs e)
        {
            BeginCommand("ARC");
        }

        private void btnDrawCircle_Click(object sender, EventArgs e)
        {
            BeginCommand("CIRCLE");
        }

        private void btnDrawEllipse_Click(object sender, EventArgs e)
        {
            BeginCommand("ELLIPSE");
        }

        private void btnDrawEllipticArc_Click(object sender, EventArgs e)
        {
            BeginCommand("ELLIPTIC_ARC");
        }

        private void btnDrawText_Click(object sender, EventArgs e)
        {
            BeginCommand("TEXT");
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(20, 0));
                    break;
                case Keys.Left:
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(-20, 0));
                    break;
                case Keys.Down:
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(0, -20));
                    break;
                case Keys.Up:
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(0, 20));
                    break;
                case Keys.PageDown:
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Rotation(5 * (float)Math.PI / 180));
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                    break;
                case Keys.PageUp:
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Rotation(-5 * (float)Math.PI / 180));
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                    break;
                case Keys.Home:
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Scale(0.8f, 0.8f));
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                    break;
                case Keys.End:
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Scale(1.2f, 1.2f));
                    TransformItems(cadWindow1.Editor.Selection, TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                    break;
                case Keys.Delete:
                    Drawable[] toDelete = cadWindow1.Editor.Selection.ToArray();
                    foreach (Drawable item in toDelete)
                    {
                        cadWindow1.Model.Remove(item);
                    }
                    break;
            }
            cadWindow1.Refresh();
        }

        private void TransformItems(IEnumerable<Drawable> items, TransformationMatrix2D trans)
        {
            foreach (Drawable item in items)
            {
                item.TransformBy(trans);
            }
        }
    }
}
