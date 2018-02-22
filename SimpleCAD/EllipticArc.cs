using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class EllipticArc : Drawable
    {
        public Point2D Center { get; set; }

        [Browsable(false)]
        public float X { get { return Center.X; } }
        [Browsable(false)]
        public float Y { get { return Center.Y; } }

        private Vector2D dir;

        public float SemiMajorAxis { get; set; }
        public float SemiMinorAxis { get; set; }

        public float StartAngle { get; set; }
        public float EndAngle { get; set; }

        public EllipticArc(Point2D center, float semiMajor, float semiMinor, float startAngle, float endAngle)
        {
            Center = center;
            SemiMajorAxis = semiMajor;
            SemiMinorAxis = semiMinor;
            dir = Vector2D.XAxis;
            StartAngle = startAngle;
            EndAngle = endAngle;
        }

        public EllipticArc(float x, float y, float semiMajor, float semiMinor, float startAngle, float endAngle)
            : this(new Point2D(x, y), semiMajor, semiMinor, startAngle, endAngle)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            System.Drawing.Drawing2D.Matrix orgTr = param.Graphics.Transform;
            param.Graphics.RotateTransform(dir.Angle * 180 / (float)Math.PI, System.Drawing.Drawing2D.MatrixOrder.Append);
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawArc(pen, X - SemiMajorAxis, Y - SemiMinorAxis, 2 * SemiMajorAxis, 2 * SemiMinorAxis,
                    StartAngle * 180f / (float)Math.PI, (EndAngle - StartAngle) * 180f / (float)Math.PI);
            }
            param.Graphics.Transform = orgTr;
        }

        public override Extents GetExtents()
        {
            Extents extents = new Extents();
            extents.Add(X - SemiMajorAxis, Y - SemiMinorAxis);
            extents.Add(X + SemiMajorAxis, Y + SemiMinorAxis);
            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Point2D p = Center;
            p.TransformBy(transformation);
            Center = p;

            dir.TransformBy(transformation);

            Vector2D unit = Vector2D.XAxis;
            unit.TransformBy(transformation);
            SemiMajorAxis = dir.Length * SemiMajorAxis;
            SemiMinorAxis = dir.Length * SemiMinorAxis;

            Vector2D a1 = Vector2D.FromAngle(StartAngle);
            Vector2D a2 = Vector2D.FromAngle(EndAngle);
            a1.TransformBy(transformation);
            a2.TransformBy(transformation);
            StartAngle = a1.Angle;
            EndAngle = a2.Angle;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            Vector2D ptDir = pt - Center;
            float a1 = SemiMajorAxis - pickBoxSize / 2;
            float a2 = SemiMajorAxis + pickBoxSize / 2;
            float b1 = SemiMinorAxis - pickBoxSize / 2;
            float b2 = SemiMinorAxis + pickBoxSize / 2;
            float rot = dir.Angle;
            float xx = (pt.X - X) * (float)Math.Cos(rot) + (pt.Y - Y) * (float)Math.Sin(rot);
            float yy = (pt.X - X) * (float)Math.Sin(rot) - (pt.Y - Y) * (float)Math.Cos(rot);
            return (xx * xx / a1 / a1 + yy * yy / b1 / b1 >= 1) && (xx * xx / a2 / a2 + yy * yy / b2 / b2 <= 1) &&
                ptDir.Angle >= StartAngle && ptDir.Angle <= EndAngle;
        }
    }
}
