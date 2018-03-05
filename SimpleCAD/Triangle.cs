using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace SimpleCAD
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

        public override void Draw(DrawParams param)
        {
            poly.Style = Style;
            poly.Draw(param);
        }

        public override Extents2D GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(TransformationMatrix2D transformation)
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
                new ControlPoint("Point1"),
                new ControlPoint("Point2"),
                new ControlPoint("Point3"),
            };
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
