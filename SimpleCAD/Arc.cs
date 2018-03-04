using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class Arc : Drawable
    {
        private Point2D center;
        private float radius;
        private float startAngle;
        private float endAngle;

        public Point2D Center { get => center; set { center = value; NotifyPropertyChanged(); } }
        public float Radius { get => radius; set { radius = value; NotifyPropertyChanged(); } }
        public float StartAngle { get => startAngle; set { startAngle = value; NotifyPropertyChanged(); } }
        public float EndAngle { get => endAngle; set { endAngle = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return Center.X; } }
        [Browsable(false)]
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
            using (Pen pen = Outline.CreatePen(param))
            {
                // Represent curved features by at most 4 pixels
                float sweep = EndAngle - StartAngle;
                while (sweep < 0) sweep += 2 * MathF.PI;
                while (sweep > 2 * MathF.PI) sweep -= 2 * MathF.PI;
                float curveLength = param.ModelToView(sweep * Radius);
                int n = (int)Math.Max(4, curveLength / 4);
                float a = StartAngle;
                float da = sweep / n;
                PointF[] pts = new PointF[n + 1];
                for (int i = 0; i <= n; i++)
                {
                    pts[i] = new PointF(X + Radius * MathF.Cos(a), Y + Radius * MathF.Sin(a));
                    a += da;
                }
                param.Graphics.DrawLines(pen, pts);
            }
        }

        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();
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

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            Vector2D dir = pt - Center;
            float dist = dir.Length;
            return (dist >= Radius - pickBoxSize / 2 && dist <= Radius + pickBoxSize / 2 &&
                dir.IsBetween(Vector2D.FromAngle(StartAngle), Vector2D.FromAngle(EndAngle)));
        }

        public override Point2D[] GetControlPoints()
        {
            return new Point2D[]
            {
                Center,
                Center + Radius * Vector2D.FromAngle((StartAngle + EndAngle) / 2),
                Center + Radius * Vector2D.FromAngle(StartAngle),
                Center + Radius * Vector2D.FromAngle(EndAngle)
            };
        }

        public override void TransformControlPoint(int index, TransformationMatrix2D transformation)
        {
            if (index == 0)
            {
                Point2D p = Center;
                p.TransformBy(transformation);
                Center = p;
            }
            else if (index == 1)
            {
                Point2D pt = Center + Radius * Vector2D.FromAngle((StartAngle + EndAngle) / 2);
                pt.TransformBy(transformation);
                Radius = (pt - Center).Length;
            }
            else if (index == 2)
            {
                Point2D pt = Center + Radius * Vector2D.FromAngle(StartAngle);
                pt.TransformBy(transformation);
                StartAngle = (pt - Center).Angle;
            }
            else if (index == 3)
            {
                Point2D pt = Center + Radius * Vector2D.FromAngle(EndAngle);
                pt.TransformBy(transformation);
                EndAngle = (pt - Center).Angle;
            }
        }
    }
}
