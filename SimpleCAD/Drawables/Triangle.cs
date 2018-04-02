using SimpleCAD.Geometry;
using System.ComponentModel;
using System.IO;

namespace SimpleCAD.Drawables
{
    public class Triangle : Drawable
    {
        private Point2D p1;
        private Point2D p2;
        private Point2D p3;

        public Point2D Point1 { get { return p1; } set { p1 = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public Point2D Point2 { get { return p2; } set { p2 = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public Point2D Point3 { get { return p3; } set { p3 = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X1 { get { return Point1.X; } }
        [Browsable(false)]
        public float Y1 { get { return Point1.Y; } }
        [Browsable(false)]
        public float X2 { get { return Point2.X; } }
        [Browsable(false)]
        public float Y2 { get { return Point2.Y; } }
        [Browsable(false)]
        public float X3 { get { return Point3.X; } }
        [Browsable(false)]
        public float Y3 { get { return Point3.Y; } }

        private Polyline poly;

        public Triangle(Point2D p1, Point2D p2, Point2D p3)
        {
            Point1 = p1;
            Point2 = p2;
            Point3 = p3;
            UpdatePolyline();
        }

        public Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), new Point2D(x3, y3))
        {
            ;
        }

        private void UpdatePolyline()
        {
            poly = new Polyline();
            poly.Points.Add(p1);
            poly.Points.Add(p2);
            poly.Points.Add(p3);
            poly.Closed = true;
        }

        public override void Draw(Renderer renderer)
        {
            poly.Style = Style;
            renderer.Draw(poly);
        }

        public override Extents2D GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(Matrix2D transformation)
        {
            p1 = p1.Transform(transformation);
            p2 = p2.Transform(transformation);
            p3 = p3.Transform(transformation);
            UpdatePolyline();
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return poly.Contains(pt, pickBoxSize);
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("First point", Point1),
                new ControlPoint("Second point", Point2),
                new ControlPoint("Third point", Point3),
            };
        }

        public override void TransformControlPoint(int index, Matrix2D transformation)
        {
            if (index == 0)
                Point1 = Point1.Transform(transformation);
            else if (index == 1)
                Point2 = Point2.Transform(transformation);
            else if (index == 2)
                Point3 = Point3.Transform(transformation);
        }

        public Triangle(BinaryReader reader) : base(reader)
        {
            Point1 = new Point2D(reader);
            Point2 = new Point2D(reader);
            Point3 = new Point2D(reader);
            UpdatePolyline();
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            Point1.Save(writer);
            Point2.Save(writer);
            Point3.Save(writer);
        }
    }
}
