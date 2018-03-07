using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using System.Threading.Tasks;

namespace SimpleCAD.Commands
{
    public class DrawLine : Command
    {
        public override string RegisteredName => "Primitives.Line";
        public override string Name => "Line";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("First point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Point2D lastPt = p1.Value;
            while (true)
            {
                Editor.PointResult p2 = await ed.GetPoint("Next point: ", lastPt);
                if (p2.Result != Editor.ResultMode.OK) return;

                Drawable newItem = new Line(lastPt, p2.Value);
                doc.Model.Add(newItem);

                lastPt = p2.Value;
            }
        }
    }

    public class DrawArc : Command
    {
        public override string RegisteredName => "Primitives.Arc";
        public override string Name => "Arc";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("Center point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Arc consArc = new Arc(p1.Value, 0, 0, 2 * MathF.PI);
            doc.Jigged.Add(consArc);
            Editor.DistanceResult p2 = await ed.GetDistance("Radius: ", p1.Value, (p) => consArc.Radius = p);
            if (p2.Result != Editor.ResultMode.OK) { doc.Jigged.Remove(consArc); return; }
            Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Value, (p) => consArc.StartAngle = p);
            if (a1.Result != Editor.ResultMode.OK) { doc.Jigged.Remove(consArc); return; }
            Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p1.Value, (p) => consArc.EndAngle = p);
            doc.Jigged.Remove(consArc);
            if (a2.Result != Editor.ResultMode.OK) return;

            Drawable newItem = new Arc(p1.Value, p2.Value, a1.Value, a2.Value);
            doc.Model.Add(newItem);
        }
    }

    public class DrawCircle : Command
    {
        public override string RegisteredName => "Primitives.Circle";
        public override string Name => "Circle";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("Center point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Circle consCircle = new Circle(p1.Value, 0);
            doc.Jigged.Add(consCircle);
            Editor.DistanceResult d1 = await ed.GetDistance("Radius: ", p1.Value, (p) => consCircle.Radius = p);
            doc.Jigged.Remove(consCircle);
            if (d1.Result != Editor.ResultMode.OK) return;

            Drawable newItem = new Circle(p1.Value, d1.Value);
            doc.Model.Add(newItem);
        }
    }

    public class DrawEllipse : Command
    {
        public override string RegisteredName => "Primitives.Ellipse";
        public override string Name => "Ellipse";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("Center point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Editor.DistanceResult d1 = await ed.GetDistance("Semi major axis: ", p1.Value);
            if (d1.Result != Editor.ResultMode.OK) return;
            Ellipse consEllipse = new Ellipse(p1.Value, d1.Value, 0, (d1.Point - p1.Value).Angle);
            doc.Jigged.Add(consEllipse);
            Editor.DistanceResult d2 = await ed.GetDistance("Semi minor axis: ", p1.Value, (p) => consEllipse.SemiMinorAxis = p);
            if (d2.Result != Editor.ResultMode.OK) { doc.Jigged.Remove(consEllipse); return; }
            doc.Jigged.Remove(consEllipse);

            Drawable newItem = new Ellipse(p1.Value, d1.Value, d2.Value, (d1.Point - p1.Value).Angle);
            doc.Model.Add(newItem);
        }
    }

    public class DrawEllipticArc : Command
    {
        public override string RegisteredName => "Primitives.Elliptic_Arc";
        public override string Name => "Elliptic Arc";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("Center point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Editor.DistanceResult d1 = await ed.GetDistance("Semi major axis: ", p1.Value);
            if (d1.Result != Editor.ResultMode.OK) return;
            float rot = (p1.Value - d1.Point).Angle;
            EllipticArc consArc = new EllipticArc(
                p1.Value,
                d1.Value, d1.Value / 10,
                0, 2 * MathF.PI,
                rot);
            doc.Jigged.Add(consArc);
            Editor.DistanceResult d2 = await ed.GetDistance("Semi minor axis: ", p1.Value, (p) => consArc.SemiMinorAxis = p);
            if (d2.Result != Editor.ResultMode.OK) { doc.Jigged.Remove(consArc); return; }
            Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Value);
            if (a1.Result != Editor.ResultMode.OK) { doc.Jigged.Remove(consArc); return; }
            consArc.StartAngle = a1.Value - rot;
            Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p1.Value, (p) => consArc.EndAngle = p - rot);
            if (a2.Result != Editor.ResultMode.OK) { doc.Jigged.Remove(consArc); return; }
            doc.Jigged.Remove(consArc);

            Drawable newItem = new EllipticArc(
                p1.Value,
                d1.Value, d2.Value,
                a1.Value - rot, a2.Value - rot,
                (p1.Value - d1.Point).Angle);
            doc.Model.Add(newItem);
        }
    }

    public class DrawText : Command
    {
        public override string RegisteredName => "Primitives.Text";
        public override string Name => "Text";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("Base point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Editor.AngleResult a1 = await ed.GetAngle("Rotation: ", p1.Value);
            if (a1.Result != Editor.ResultMode.OK) return;
            Editor.DistanceResult d1 = await ed.GetDistance("Text height: ", p1.Value);
            if (d1.Result != Editor.ResultMode.OK) return;
            Text consText = new Text(p1.Value, " ", d1.Value);
            consText.Rotation = a1.Value;
            doc.Jigged.Add(consText);
            Editor.TextResult t1 = await ed.GetText("Text string: ", (p) => consText.String = p);
            doc.Jigged.Remove(consText);
            if (t1.Result != Editor.ResultMode.OK) return;

            Text newItem = new Text(p1.Value, t1.Value, d1.Value);
            newItem.Rotation = a1.Value;
            doc.Model.Add(newItem);
        }
    }

    public class DrawDimension : Command
    {
        public override string RegisteredName => "Primitives.Dimension";
        public override string Name => "Dimension";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("Start point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Editor.PointResult p2 = await ed.GetPoint("End point: ", p1.Value);
            if (p2.Result != Editor.ResultMode.OK) return;
            Editor.DistanceResult p3 = await ed.GetDistance("Text height: ", p1.Value);
            if (p3.Result != Editor.ResultMode.OK) return;

            Drawable newItem = new Dimension(p1.Value, p2.Value, p3.Value);
            doc.Model.Add(newItem);
        }
    }

    public class DrawParabola : Command
    {
        public override string RegisteredName => "Primitives.Parabola";
        public override string Name => "Parabola";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("Start point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Editor.PointResult p2 = await ed.GetPoint("End point: ", p1.Value);
            if (p2.Result != Editor.ResultMode.OK) return;
            Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Value);
            if (a1.Result != Editor.ResultMode.OK) return;
            Parabola consPb = new Parabola(p1.Value, p2.Value, a1.Value, 0);
            doc.Jigged.Add(consPb);
            Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p2.Value, (p) => consPb.EndAngle = p);
            doc.Jigged.Remove(consPb);
            if (a2.Result != Editor.ResultMode.OK) return;

            Drawable newItem = new Parabola(p1.Value, p2.Value, a1.Value, a2.Value);
            doc.Model.Add(newItem);
        }
    }

    public class DrawPolyline : Command
    {
        public override string RegisteredName => "Primitives.Polyline";
        public override string Name => "Polyline";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("First point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Point2D pt = p1.Value;
            Polyline consPoly = new Polyline(new Point2D[] { pt, pt });
            doc.Jigged.Add(consPoly);

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
                    doc.Jigged.Remove(consPoly);
                    return;
                }
                else if (pNext.Result == Editor.ResultMode.Keyword)
                {
                    if (points.Count < 2)
                    {
                        doc.Jigged.Remove(consPoly);
                        return;
                    }

                    if (pNext.Keyword == "End")
                        done = true;
                    if (pNext.Keyword == "Close")
                        done = closed = true;
                }
            }

            doc.Jigged.Remove(consPoly);
            Polyline newItem = new Polyline(points);
            if (closed) newItem.Closed = true;
            doc.Model.Add(newItem);
        }
    }

    public class DrawHatch : Command
    {
        public override string RegisteredName => "Primitives.Hatch";
        public override string Name => "Hatch";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("First point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Point2D pt = p1.Value;
            Polyline consPoly = new Polyline(new Point2D[] { pt, pt });
            consPoly.Closed = true;
            doc.Jigged.Add(consPoly);

            Point2DCollection points = new Point2DCollection();
            points.Add(pt);

            bool done = false;
            while (!done)
            {
                Editor.PointOptions options = new Editor.PointOptions("Next point: ", pt, (p) => consPoly.Points[consPoly.Points.Count - 1] = p);
                options.AddKeyword("End", true);
                Editor.PointResult pNext = await ed.GetPoint(options);
                if (pNext.Result == Editor.ResultMode.OK)
                {
                    pt = pNext.Value;
                    consPoly.Points.Add(pt);
                    points.Add(pt);
                }
                else if (pNext.Result == Editor.ResultMode.Cancel)
                {
                    doc.Jigged.Remove(consPoly);
                    return;
                }
                else if (pNext.Result == Editor.ResultMode.Keyword)
                {
                    if (points.Count < 2)
                    {
                        doc.Jigged.Remove(consPoly);
                        return;
                    }

                    if (pNext.Keyword == "End")
                        done = true;
                }
            }

            doc.Jigged.Remove(consPoly);
            Hatch newItem = new Hatch(points);
            doc.Model.Add(newItem);
        }
    }

    public class DrawRectangle : Command
    {
        public override string RegisteredName => "Primitives.Rectangle";
        public override string Name => "Rectangle";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("Center point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Rectangle consRec = new Rectangle(p1.Value, 0, 0);
            doc.Jigged.Add(consRec);
            Editor.PointResult p2 = await ed.GetPoint("Corner point: ", p1.Value, (p) => consRec.Corner = p);
            if (p2.Result != Editor.ResultMode.OK) { doc.Jigged.Remove(consRec); return; }
            doc.Jigged.Remove(consRec);

            Drawable newItem = new Rectangle(p1.Value, p2.Value);
            doc.Model.Add(newItem);
        }
    }

    public class DrawTriangle : Command
    {
        public override string RegisteredName => "Primitives.Triangle";
        public override string Name => "Triangle";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.PointResult p1 = await ed.GetPoint("First point: ");
            if (p1.Result != Editor.ResultMode.OK) return;
            Editor.PointResult p2 = await ed.GetPoint("Second point: ", p1.Value);
            if (p2.Result != Editor.ResultMode.OK) return;
            Triangle consTri = new Triangle(p1.Value, p2.Value, p2.Value);
            doc.Jigged.Add(consTri);
            Editor.PointResult p3 = await ed.GetPoint("Third point: ", p1.Value, (p) => consTri.Point3 = p);
            doc.Jigged.Remove(consTri);
            if (p3.Result != Editor.ResultMode.OK) return;

            Drawable newItem = new Triangle(p1.Value, p2.Value, p3.Value);
            doc.Model.Add(newItem);
        }
    }
}

