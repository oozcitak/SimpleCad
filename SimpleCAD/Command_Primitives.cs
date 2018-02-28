using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public partial class Command
    {
        public class DrawLine : Command
        {
            public override string RegisteredName => "Primitives.Line";
            public override string Name => "Line";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("Start point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Editor.PointResult p2 = await ed.GetPoint("End point: ", p1.Value);
                if (p2.Result != Editor.ResultMode.OK) return;

                Drawable newItem = new Line(p1.Value, p2.Value);
                doc.Model.Add(newItem);
            }
        }

        public class DrawArc : Command
        {
            public override string RegisteredName => "Primitives.Arc";
            public override string Name => "Arc";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("Center point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Arc consArc = new Arc(p1.Value, 0, 0, 2 * (float)Math.PI);
                consArc.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consArc);
                Editor.PointResult p2 = await ed.GetPoint("Radius: ", p1.Value, (p) => consArc.Radius = (p - consArc.Center).Length);
                if (p2.Result != Editor.ResultMode.OK) { doc.Transients.Remove(consArc); return; }
                Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Value, (p) => consArc.StartAngle = p.Angle);
                if (a1.Result != Editor.ResultMode.OK) { doc.Transients.Remove(consArc); return; }
                Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p1.Value, (p) => consArc.EndAngle = p.Angle);
                doc.Transients.Remove(consArc);
                if (a2.Result != Editor.ResultMode.OK) return;

                Drawable newItem = new Arc(p1.Value,
                    (p2.Value - p1.Value).Length,
                    a1.Value.Angle, a2.Value.Angle);
                doc.Model.Add(newItem);
            }
        }

        public class DrawCircle : Command
        {
            public override string RegisteredName => "Primitives.Circle";
            public override string Name => "Circle";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("Center point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Circle consCircle = new Circle(p1.Value, 0);
                consCircle.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consCircle);
                Editor.PointResult p2 = await ed.GetPoint("Radius: ", p1.Value, (p) => consCircle.Radius = (p - consCircle.Center).Length);
                doc.Transients.Remove(consCircle);
                if (p2.Result != Editor.ResultMode.OK) return;

                Drawable newItem = new Circle(p1.Value, (p2.Value - p1.Value).Length);
                doc.Model.Add(newItem);
            }
        }

        public class DrawEllipse : Command
        {
            public override string RegisteredName => "Primitives.Ellipse";
            public override string Name => "Ellipse";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("Center point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Editor.PointResult p2 = await ed.GetPoint("Semi major axis: ", p1.Value);
                if (p2.Result != Editor.ResultMode.OK) return;
                Ellipse consEllipse = new Ellipse(p1.Value, (p2.Value - p1.Value).Length, 0);
                consEllipse.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consEllipse);
                Editor.PointResult p3 = await ed.GetPoint("Semi minor axis: ", p1.Value, (p) => consEllipse.SemiMinorAxis = (p - consEllipse.Center).Length);
                doc.Transients.Remove(consEllipse);
                if (p3.Result != Editor.ResultMode.OK) return;

                Drawable newItem = new Ellipse(p1.Value,
                    (p2.Value - p1.Value).Length, (p3.Value - p1.Value).Length);
                doc.Model.Add(newItem);
            }
        }

        public class DrawEllipticArc : Command
        {
            public override string RegisteredName => "Primitives.Elliptic_Arc";
            public override string Name => "Elliptic Arc";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("Center point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Editor.PointResult p2 = await ed.GetPoint("Semi major axis: ", p1.Value);
                if (p2.Result != Editor.ResultMode.OK) return;
                EllipticArc consArc = new EllipticArc(p1.Value, (p2.Value - p1.Value).Length, (p2.Value - p1.Value).Length / 10, 0, 2 * (float)Math.PI);
                consArc.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consArc);
                Editor.PointResult p3 = await ed.GetPoint("Semi minor axis: ", p1.Value, (p) => consArc.SemiMinorAxis = (p - consArc.Center).Length);
                if (p3.Result != Editor.ResultMode.OK) { doc.Transients.Remove(consArc); return; }
                Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Value, (p) => consArc.StartAngle = p.Angle);
                if (a1.Result != Editor.ResultMode.OK) { doc.Transients.Remove(consArc); return; }
                Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p1.Value, (p) => consArc.EndAngle = p.Angle);
                doc.Transients.Remove(consArc);
                if (a2.Result != Editor.ResultMode.OK) return;

                Drawable newItem = new EllipticArc(p1.Value,
                    (p2.Value - p1.Value).Length, (p3.Value - p1.Value).Length,
                    a1.Value.Angle, a2.Value.Angle);
                doc.Model.Add(newItem);
            }
        }

        public class DrawText : Command
        {
            public override string RegisteredName => "Primitives.Text";
            public override string Name => "Text";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("Base point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Editor.AngleResult a1 = await ed.GetAngle("Rotation: ", p1.Value);
                if (a1.Result != Editor.ResultMode.OK) return;
                Editor.PointResult p2 = await ed.GetPoint("Text height: ", p1.Value);
                if (p2.Result != Editor.ResultMode.OK) return;
                Text consText = new Text(p1.Value, " ", (p2.Value - p1.Value).Length);
                consText.Rotation = a1.Value.Angle;
                consText.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consText);
                Editor.TextResult t1 = await ed.GetText("Text string: ", (p) => consText.String = p);
                doc.Transients.Remove(consText);
                if (t1.Result != Editor.ResultMode.OK) return;

                Drawable newItem = new Text(p1.Value, t1.Value, (p2.Value - p1.Value).Length);
                ((Text)newItem).Rotation = a1.Value.Angle;
                doc.Model.Add(newItem);
            }
        }

        public class DrawDimension : Command
        {
            public override string RegisteredName => "Primitives.Dimension";
            public override string Name => "Dimension";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("Start point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Editor.PointResult p2 = await ed.GetPoint("End point: ", p1.Value);
                if (p2.Result != Editor.ResultMode.OK) return;
                Editor.PointResult p3 = await ed.GetPoint("Text height: ", p1.Value);
                if (p3.Result != Editor.ResultMode.OK) return;

                Drawable newItem = new Dimension(p1.Value, p2.Value, (p3.Value - p1.Value).Length);
                doc.Model.Add(newItem);
            }
        }

        public class DrawParabola : Command
        {
            public override string RegisteredName => "Primitives.Parabola";
            public override string Name => "Parabola";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("Start point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Editor.PointResult p2 = await ed.GetPoint("End point: ", p1.Value);
                if (p2.Result != Editor.ResultMode.OK) return;
                Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Value);
                if (a1.Result != Editor.ResultMode.OK) return;
                Parabola consPb = new Parabola(p1.Value, p2.Value, a1.Value.Angle, 0);
                consPb.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consPb);
                Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p2.Value, (p) => consPb.EndAngle = p.Angle);
                doc.Transients.Remove(consPb);
                if (a2.Result != Editor.ResultMode.OK) return;

                Drawable newItem = new Parabola(p1.Value, p2.Value, a1.Value.Angle, a2.Value.Angle);
                doc.Model.Add(newItem);
            }
        }

        public class DrawPolyline : Command
        {
            public override string RegisteredName => "Primitives.Polyline";
            public override string Name => "Polyline";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("First point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Point2D pt = p1.Value;
                Polyline consPoly = new Polyline(new Point2D[] { pt, pt });
                consPoly.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consPoly);

                Point2DCollection points = new Point2DCollection();
                points.Add(pt);

                bool done = false;
                bool closed = false;
                while (!done)
                {
                    Editor.PointOptions options = new Editor.PointOptions("Next point: ", pt, (p) => consPoly.Points[consPoly.Points.Count - 1] = p);
                    options.AddKeyword("End", true);
                    options.AddKeyword("Close");
                    Editor.PointResult pNext = await ed.GetPoint(options);
                    if (pNext.Result == Editor.ResultMode.OK)
                    {
                        pt = pNext.Value;
                        consPoly.Points.Add(pt);
                        points.Add(pt);
                    }
                    else if (pNext.Result == Editor.ResultMode.Cancel)
                    {
                        doc.Transients.Remove(consPoly);
                        return;
                    }
                    else if (pNext.Result == Editor.ResultMode.Keyword)
                    {
                        if (points.Count < 2)
                        {
                            doc.Transients.Remove(consPoly);
                            return;
                        }

                        if (pNext.Keyword == "End")
                            done = true;
                        if (pNext.Keyword == "Close")
                            done = closed = true;
                    }
                }

                doc.Transients.Remove(consPoly);
                Polyline newItem = new Polyline(points);
                if (closed) newItem.Closed = true;
                doc.Model.Add(newItem);
            }
        }
    }
}
