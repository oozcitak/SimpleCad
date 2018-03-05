using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class Parabola : Drawable
    {
        private Point2D p1;
        private Point2D p2;

        public Point2D StartPoint { get => p1; set { p1 = value; NotifyPropertyChanged(); } }
        public Point2D EndPoint { get => p2; set { p2 = value; NotifyPropertyChanged(); } }

        public Point2D IntersectionPoint
        {
            get
            {
                Point2D p3 = StartPoint + Vector2D.FromAngle(StartAngle);
                Point2D p4 = EndPoint + Vector2D.FromAngle(EndAngle);
                Intersect(StartPoint, p3, EndPoint, p4, out Point2D c);
                return c;
            }
        }

        private float startAngle;
        private float endAngle;

        public float StartAngle { get => startAngle; set { startAngle = value; NotifyPropertyChanged(); } }
        public float EndAngle { get => endAngle; set { endAngle = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X1 { get { return StartPoint.X; } }
        [Browsable(false)]
        public float Y1 { get { return StartPoint.Y; } }
        [Browsable(false)]
        public float X2 { get { return EndPoint.X; } }
        [Browsable(false)]
        public float Y2 { get { return EndPoint.Y; } }
        [Browsable(false)]
        public float XI { get { return IntersectionPoint.X; } }
        [Browsable(false)]
        public float YI { get { return IntersectionPoint.Y; } }

        public Parabola(Point2D p1, Point2D p2, float startAngle, float endAngle)
        {
            StartPoint = p1;
            EndPoint = p2;
            StartAngle = startAngle;
            EndAngle = endAngle;
        }

        public Parabola(float x1, float y1, float x2, float y2, float startAngle, float endAngle)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), startAngle, endAngle)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            Point2D c1 = StartPoint * 1 / 3 + (IntersectionPoint * 2 / 3).ToVector2D();
            Point2D c2 = EndPoint * 1 / 3 + (IntersectionPoint * 2 / 3).ToVector2D();

            using (Pen pen = Outline.CreatePen(param))
            {
                param.Graphics.DrawBezier(pen, StartPoint.ToPointF(), c1.ToPointF(), c2.ToPointF(), EndPoint.ToPointF());
            }
        }

        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();

            float t = 0;
            for (int i = 0; i < 20; i++)
            {
                t += 0.05f;
                // Points on the Quadratic bezier curve
                float x = (1 - t) * (1 - t) * X1 + 2 * (1 - t) * t * XI + t * t * X2;
                float y = (1 - t) * (1 - t) * Y1 + 2 * (1 - t) * t * YI + t * t * Y2;
                extents.Add(x, y);
            }

            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            StartPoint = StartPoint.Transform(transformation);
            EndPoint = EndPoint.Transform(transformation);

            Vector2D a1 = Vector2D.FromAngle(StartAngle);
            Vector2D a2 = Vector2D.FromAngle(EndAngle);
            a1.TransformBy(transformation);
            a2.TransformBy(transformation);
            StartAngle = a1.Angle;
            EndAngle = a2.Angle;
        }

        private bool Intersect(Point2D p1, Point2D p2, Point2D p3, Point2D p4, out Point2D p)
        {
            p = Point2D.Zero;

            float x1 = p1.X; float y1 = p1.Y;
            float x2 = p2.X; float y2 = p2.Y;
            float x3 = p3.X; float y3 = p3.Y;
            float x4 = p4.X; float y4 = p4.Y;

            float det = Determinant(x1 - x2, y1 - y2, x3 - x4, y3 - y4);
            if (Math.Abs(det) <= 10.0f * float.Epsilon) return false;

            float p1p2det = Determinant(x1, y1, x2, y2);
            float p3p4det = Determinant(x3, y3, x4, y4);

            float xdet = Determinant(p1p2det, x1 - x2, p3p4det, x3 - x4);
            float ydet = Determinant(p1p2det, y1 - y2, p3p4det, y3 - y4);

            p = new Point2D(xdet / det, ydet / det);
            return true;
        }

        /// <summary>
        /// Returns the determinant of the given 2x2 matrix.
        /// | a1 b1 |
        /// | a2 b2 |
        /// </summary>
        private float Determinant(float a1, float b1, float a2, float b2)
        {
            return a1 * b2 - a2 * b1;
        }

        public override ControlPoint[] GetControlPoints(float size)
        {
            return new[]
            {
                new ControlPoint("StartPoint"),
                new ControlPoint("EndPoint"),
                new ControlPoint("StartAngle", ControlPoint.ControlPointType.Angle, StartPoint, StartPoint + size * Vector2D.FromAngle(StartAngle)),
                new ControlPoint("EndAngle", ControlPoint.ControlPointType.Angle, EndPoint, EndPoint + size * Vector2D.FromAngle(EndAngle)),
            };
        }
    }
}
