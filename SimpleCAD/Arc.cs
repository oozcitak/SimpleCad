using System;
using System.Drawing;

namespace SimpleCAD
{
    public class Arc : Drawable
    {
        public Point2D Center { get; set; }
        public float Radius { get; set; }
        public float StartAngle { get; set; }
        public float EndAngle { get; set; }

        public float X { get { return Center.X; } }
        public float Y { get { return Center.Y; } }

        public Arc(Point2D center, float radius, float startAngle, float endAngle)
        {
            Center = center;
            Radius = radius;
            StartAngle = startAngle;
            EndAngle = endAngle;
        }

        public Arc(float x, float y, float radius, float startAngle, float endAngle)
            : this(new Point2D(x, y), radius, startAngle, endAngle)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawArc(pen, X - Radius, Y - Radius, 2f * Radius, 2f * Radius, StartAngle * 180f / (float)Math.PI, (EndAngle - StartAngle) * 180f / (float)Math.PI);
            }
        }

        public override Extents GetExtents()
        {
            Extents extents = new Extents();
            extents.Add(X - Radius, Y - Radius);
            extents.Add(X + Radius, Y + Radius);
            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Point2D p = Center;
            p.TransformBy(transformation);
            Center = p;

            Vector2D dir = Vector2D.XAxis * Radius;
            dir.TransformBy(transformation);
            Radius = dir.Length;

            Vector2D a1 = Vector2D.FromAngle(StartAngle);
            Vector2D a2 = Vector2D.FromAngle(EndAngle);
            a1.TransformBy(transformation);
            a2.TransformBy(transformation);
            StartAngle = a1.Angle;
            EndAngle = a2.Angle;
        }
    }
}
