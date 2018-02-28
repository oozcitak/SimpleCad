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
                if (!p1.Success) return;
                Editor.PointResult p2 = await ed.GetPoint("End point: ", p1.Location);
                if (!p2.Success) return;

                Drawable newItem = new Line(p1.Location, p2.Location);
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
                if (!p1.Success) return;
                Arc consArc = new Arc(p1.Location, 0, 0, 2 * (float)Math.PI);
                consArc.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consArc);
                Editor.PointResult p2 = await ed.GetPoint("Radius: ", p1.Location, (p) => consArc.Radius = (p - consArc.Center).Length);
                if (!p2.Success) { doc.Transients.Remove(consArc); return; }
                Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Location, (p) => consArc.StartAngle = p.Angle);
                if (!a1.Success) { doc.Transients.Remove(consArc); return; }
                Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p1.Location, (p) => consArc.EndAngle = p.Angle);
                doc.Transients.Remove(consArc);
                if (!a2.Success) return;

                Drawable newItem = new Arc(p1.Location,
                    (p2.Location - p1.Location).Length,
                    a1.Direction.Angle, a2.Direction.Angle);
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
                if (!p1.Success) return;
                Circle consCircle = new Circle(p1.Location, 0);
                consCircle.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consCircle);
                Editor.PointResult p2 = await ed.GetPoint("Radius: ", p1.Location, (p) => consCircle.Radius = (p - consCircle.Center).Length);
                doc.Transients.Remove(consCircle);
                if (!p2.Success) return;

                Drawable newItem = new Circle(p1.Location, (p2.Location - p1.Location).Length);
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
                if (!p1.Success) return;
                Editor.PointResult p2 = await ed.GetPoint("Semi major axis: ", p1.Location);
                if (!p2.Success) return;
                Ellipse consEllipse = new Ellipse(p1.Location, (p2.Location - p1.Location).Length, 0);
                consEllipse.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consEllipse);
                Editor.PointResult p3 = await ed.GetPoint("Semi minor axis: ", p1.Location, (p) => consEllipse.SemiMinorAxis = (p - consEllipse.Center).Length);
                doc.Transients.Remove(consEllipse);
                if (!p3.Success) return;

                Drawable newItem = new Ellipse(p1.Location,
                    (p2.Location - p1.Location).Length, (p3.Location - p1.Location).Length);
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
                if (!p1.Success) return;
                Editor.PointResult p2 = await ed.GetPoint("Semi major axis: ", p1.Location);
                if (!p2.Success) return;
                EllipticArc consArc = new EllipticArc(p1.Location, (p2.Location - p1.Location).Length, (p2.Location - p1.Location).Length / 10, 0, 2 * (float)Math.PI);
                consArc.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consArc);
                Editor.PointResult p3 = await ed.GetPoint("Semi minor axis: ", p1.Location, (p) => consArc.SemiMinorAxis = (p - consArc.Center).Length);
                if (!p3.Success) { doc.Transients.Remove(consArc); return; }
                Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Location, (p) => consArc.StartAngle = p.Angle);
                if (!a1.Success) { doc.Transients.Remove(consArc); return; }
                Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p1.Location, (p) => consArc.EndAngle = p.Angle);
                doc.Transients.Remove(consArc);
                if (!a2.Success) return;

                Drawable newItem = new EllipticArc(p1.Location,
                    (p2.Location - p1.Location).Length, (p3.Location - p1.Location).Length,
                    a1.Direction.Angle, a2.Direction.Angle);
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
                if (!p1.Success) return;
                Editor.AngleResult a1 = await ed.GetAngle("Rotation: ", p1.Location);
                if (!a1.Success) return;
                Editor.PointResult p2 = await ed.GetPoint("Text height: ", p1.Location);
                if (!p2.Success) return;
                Text consText = new Text(p1.Location, " ", (p2.Location - p1.Location).Length);
                consText.Rotation = a1.Direction.Angle;
                consText.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consText);
                Editor.TextResult t1 = await ed.GetText("Text string: ", (p) => consText.String = p);
                doc.Transients.Remove(consText);
                if (!t1.Success) return;

                Drawable newItem = new Text(p1.Location, t1.Text, (p2.Location - p1.Location).Length);
                ((Text)newItem).Rotation = a1.Direction.Angle;
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
                if (!p1.Success) return;
                Editor.PointResult p2 = await ed.GetPoint("End point: ", p1.Location);
                if (!p2.Success) return;
                Editor.PointResult p3 = await ed.GetPoint("Text height: ", p1.Location);
                if (!p3.Success) return;

                Drawable newItem = new Dimension(p1.Location, p2.Location, (p3.Location - p1.Location).Length);
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
                if (!p1.Success) return;
                Editor.PointResult p2 = await ed.GetPoint("End point: ", p1.Location);
                if (!p2.Success) return;
                Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Location);
                if (!a1.Success) return;
                Parabola consPb = new Parabola(p1.Location, p2.Location, a1.Direction.Angle, 0);
                consPb.OutlineStyle = doc.Editor.TransientStyle;
                doc.Transients.Add(consPb);
                Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p2.Location, (p) => consPb.EndAngle = p.Angle);
                doc.Transients.Remove(consPb);
                if (!a2.Success) return;

                Drawable newItem = new Parabola(p1.Location, p2.Location, a1.Direction.Angle, a2.Direction.Angle);
                doc.Model.Add(newItem);
            }
        }
    }
}
