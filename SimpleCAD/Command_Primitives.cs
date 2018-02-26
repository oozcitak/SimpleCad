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
                Editor.PointResult p2 = await ed.GetPoint("Radius: ", p1.Location);
                if (!p2.Success) return;
                Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Location);
                if (!a1.Success) return;
                Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p1.Location);
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
                Editor.PointResult p2 = await ed.GetPoint("Radius: ", p1.Location);
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
                Editor.PointResult p3 = await ed.GetPoint("Semi minor axis: ", p1.Location);
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
                Editor.PointResult p3 = await ed.GetPoint("Semi minor axis: ", p1.Location);
                if (!p3.Success) return;
                Editor.AngleResult a1 = await ed.GetAngle("Start angle: ", p1.Location);
                if (!a1.Success) return;
                Editor.AngleResult a2 = await ed.GetAngle("End angle: ", p1.Location);
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
                Editor.TextResult t1 = await ed.GetText("Text string: ");
                if (!t1.Success) return;

                Drawable newItem = new Text(p1.Location,t1.Text, (p2.Location - p1.Location).Length);
                ((Text)newItem).Rotation = a1.Direction.Angle;
                doc.Model.Add(newItem);
            }
        }
    }
}
