using SimpleCAD.Geometry;
using System;
using System.ComponentModel;
using System.IO;

namespace SimpleCAD.Drawables
{
    public class Parabola : Drawable
    {
        private Point2D p1;
        private Point2D p2;

        public Point2D StartPoint { get => p1; set { p1 = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public Point2D EndPoint { get => p2; set { p2 = value; UpdatePolyline(); NotifyPropertyChanged(); } }

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

        public float StartAngle { get => startAngle; set { startAngle = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float EndAngle { get => endAngle; set { endAngle = value; UpdatePolyline(); NotifyPropertyChanged(); } }

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

        private Polyline poly;
        private float curveLength = 4;
        private float cpSize = 0;

        public Parabola(Point2D p1, Point2D p2, float startAngle, float endAngle)
        {
            StartPoint = p1;
            EndPoint = p2;
            StartAngle = startAngle;
            EndAngle = endAngle;
            UpdatePolyline();
        }

        public Parabola(float x1, float y1, float x2, float y2, float startAngle, float endAngle)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), startAngle, endAngle)
        {
            ;
        }

        public override void Draw(Renderer renderer)
        {
            cpSize = 2 * renderer.View.ScreenToWorld(new Vector2D(renderer.View.Document.Settings.Get<int>("ControlPointSize"), 0)).X;
            poly.Style = Style;
            renderer.Draw(poly);
        }

        private void UpdatePolyline()
        {
            poly = new Polyline();
            // Represent curved features by at most 4 pixels
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;
            float t = 0;
            float dt = 1f / n;
            for (int i = 0; i <= n; i++)
            {
                float x = (1 - t) * (1 - t) * X1 + 2 * (1 - t) * t * XI + t * t * X2;
                float y = (1 - t) * (1 - t) * Y1 + 2 * (1 - t) * t * YI + t * t * Y2;
                poly.Points.Add(x, y);
                t += dt;
            }
            poly.Closed = false;
        }

        public override Extents2D GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(Matrix2D transformation)
        {
            StartPoint = StartPoint.Transform(transformation);
            EndPoint = EndPoint.Transform(transformation);
            StartAngle = Vector2D.FromAngle(StartAngle).Transform(transformation).Angle;
            EndAngle = Vector2D.FromAngle(EndAngle).Transform(transformation).Angle;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return poly.Contains(pt, pickBoxSize);
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

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Start point", StartPoint),
                new ControlPoint("End point", EndPoint),
                new ControlPoint("Start angle", ControlPoint.ControlPointType.Angle, StartPoint, StartPoint + cpSize * Vector2D.FromAngle(StartAngle)),
                new ControlPoint("End angle", ControlPoint.ControlPointType.Angle, EndPoint, EndPoint + cpSize * Vector2D.FromAngle(EndAngle)),
            };
        }

        public override void TransformControlPoint(int index, Matrix2D transformation)
        {
            if (index == 0)
                StartPoint = StartPoint.Transform(transformation);
            else if (index == 1)
                EndPoint = EndPoint.Transform(transformation);
            else if (index == 2)
                StartAngle = Vector2D.FromAngle(StartAngle).Transform(transformation).Angle;
            else if (index == 3)
                EndAngle = Vector2D.FromAngle(EndAngle).Transform(transformation).Angle;
        }

        public Parabola(BinaryReader reader) : base(reader)
        {
            StartPoint = new Point2D(reader);
            EndPoint = new Point2D(reader);
            StartAngle = reader.ReadSingle();
            EndAngle = reader.ReadSingle();
            UpdatePolyline();
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            StartPoint.Save(writer);
            EndPoint.Save(writer);
            writer.Write(StartAngle);
            writer.Write(EndAngle);
        }
    }
}
