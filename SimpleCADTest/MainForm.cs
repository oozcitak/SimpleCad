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
        Drawable hoverItem;
        OutlineStyle hoverItemStyle;
        Circle trPoint;

        public MainForm()
        {
            InitializeComponent();
            trPoint = new Circle(0, 0, 20);
            trPoint.FillStyle = FillStyle.Orange;
            trPoint.OutlineStyle = new OutlineStyle(Color.Red, 3);
            cadWindow1.Model.Add(trPoint);
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
            else
            {
                Drawable sel = cadWindow1.FindItemAtScreenCoordinates(e.X, e.Y, 5);
                if (sel != trPoint && sel != hoverItem)
                {
                    if (hoverItem != null)
                    {
                        hoverItem.OutlineStyle = hoverItemStyle;
                        cadWindow1.Refresh();
                    }
                    hoverItem = sel;
                    if (hoverItem != null)
                    {
                        hoverItemStyle = hoverItem.OutlineStyle;
                        hoverItem.OutlineStyle = OutlineStyle.CornflowerBlue;
                        propertyGrid1.SelectedObject = hoverItem;
                        cadWindow1.Refresh();
                    }
                }
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
                statusLabel.Text = "Select start point of line";
                newItem = new Line(pickedPoints[0], pickedPoints[0]);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
            }
            else if (currentCommand == "LINE" && commandStep == 1)
            {
                statusLabel.Text = "Select end point of line";
                Line line = newItem as Line;
                line.P2 = pickedPoints[1];
                cadWindow1.Refresh();
            }
            else if (currentCommand == "LINE" && commandStep == 2)
            {
                ResetCommand();
            }
            else if (currentCommand == "ARC" && commandStep == 0)
            {
                statusLabel.Text = "Select center point of arc";
                newItem = new Arc(pickedPoints[0], 0, 0, 2 * (float)Math.PI);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ARC" && commandStep == 1)
            {
                statusLabel.Text = "Select radius of arc";
                Arc arc = newItem as Arc;
                arc.Radius = (pickedPoints[1] - pickedPoints[0]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ARC" && commandStep == 2)
            {
                statusLabel.Text = "Select start angle of arc";
                Arc arc = newItem as Arc;
                arc.StartAngle = (pickedPoints[2] - pickedPoints[0]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ARC" && commandStep == 3)
            {
                statusLabel.Text = "Select end angle of arc";
                Arc arc = newItem as Arc;
                arc.EndAngle = (pickedPoints[3] - pickedPoints[0]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ARC" && commandStep == 4)
            {
                ResetCommand();
            }
            else if (currentCommand == "CIRCLE" && commandStep == 0)
            {
                statusLabel.Text = "Select center point of circle";
                newItem = new Circle(pickedPoints[0], 0);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
            }
            else if (currentCommand == "CIRCLE" && commandStep == 1)
            {
                statusLabel.Text = "Select radius of circle";
                Circle cir = newItem as Circle;
                cir.Radius = (pickedPoints[1] - pickedPoints[0]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "CIRCLE" && commandStep == 2)
            {
                ResetCommand();
            }
            else if (currentCommand == "ELLIPSE" && commandStep == 0)
            {
                statusLabel.Text = "Select center point of ellipse";
                newItem = new Ellipse(pickedPoints[0], 0, 0);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPSE" && commandStep == 1)
            {
                statusLabel.Text = "Select semi-major axis of ellipse";
                Ellipse eli = newItem as Ellipse;
                eli.SemiMajorAxis = (pickedPoints[1] - pickedPoints[0]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPSE" && commandStep == 2)
            {
                statusLabel.Text = "Select semi-minor axis of ellipse";
                Ellipse eli = newItem as Ellipse;
                eli.SemiMinorAxis = (pickedPoints[2] - pickedPoints[0]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPSE" && commandStep == 3)
            {
                ResetCommand();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 0)
            {
                statusLabel.Text = "Select center point of elliptic arc";
                newItem = new EllipticArc(pickedPoints[0], 0, 0, 0, 2 * (float)Math.PI);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 1)
            {
                statusLabel.Text = "Select semi-major axis of elliptic arc";
                EllipticArc eli = newItem as EllipticArc;
                eli.SemiMajorAxis = (pickedPoints[1] - pickedPoints[0]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 2)
            {
                statusLabel.Text = "Select semi-minor axis of elliptic arc";
                EllipticArc eli = newItem as EllipticArc;
                eli.SemiMinorAxis = (pickedPoints[2] - pickedPoints[0]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 3)
            {
                statusLabel.Text = "Select start angle of elliptic arc";
                EllipticArc arc = newItem as EllipticArc;
                arc.StartAngle = (pickedPoints[3] - pickedPoints[0]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 4)
            {
                statusLabel.Text = "Select end angle of elliptic arc";
                EllipticArc arc = newItem as EllipticArc;
                arc.EndAngle = (pickedPoints[4] - pickedPoints[0]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "ELLIPTIC_ARC" && commandStep == 5)
            {
                ResetCommand();
            }
            else if (currentCommand == "TEXT" && commandStep == 0)
            {
                statusLabel.Text = "Select insertion point of text";
                newItem = new Text(pickedPoints[0], "abc", 1);
                cadWindow1.Model.Add(newItem);
                cadWindow1.Refresh();
            }
            else if (currentCommand == "TEXT" && commandStep == 1)
            {
                statusLabel.Text = "Select text rotation";
                Text txt = newItem as Text;
                txt.Rotation = (pickedPoints[1] - pickedPoints[0]).Angle;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "TEXT" && commandStep == 2)
            {
                statusLabel.Text = "Select text height";
                Text txt = newItem as Text;
                txt.Height = (pickedPoints[2] - pickedPoints[0]).Length;
                cadWindow1.Refresh();
            }
            else if (currentCommand == "TEXT" && commandStep == 3)
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
            if (hoverItem != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(20, 0));
                        break;
                    case Keys.Left:
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(-20, 0));
                        break;
                    case Keys.Down:
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(0, -20));
                        break;
                    case Keys.Up:
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(0, 20));
                        break;
                    case Keys.PageDown:
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                        hoverItem.TransformBy(TransformationMatrix2D.Rotation(5 * (float)Math.PI / 180));
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                        break;
                    case Keys.PageUp:
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                        hoverItem.TransformBy(TransformationMatrix2D.Rotation(-5 * (float)Math.PI / 180));
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                        break;
                    case Keys.Home:
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                        hoverItem.TransformBy(TransformationMatrix2D.Scale(0.8f, 0.8f));
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                        break;
                    case Keys.End:
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(-trPoint.X, -trPoint.Y));
                        hoverItem.TransformBy(TransformationMatrix2D.Scale(1.2f, 1.2f));
                        hoverItem.TransformBy(TransformationMatrix2D.Translation(trPoint.X, trPoint.Y));
                        break;
                }
                cadWindow1.Refresh();
            }
        }
    }
}
