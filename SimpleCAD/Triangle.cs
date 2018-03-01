using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class Triangle : Drawable
    {
        private Point2D p1;
        private Point2D p2;
        private Point2D p3;

        public Point2D P1 { get { return p1; } set { p1 = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public Point2D P2 { get { return p2; } set { p2 = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public Point2D P3 { get { return p3; } set { p3 = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X1 { get { return P1.X; } }
        [Browsable(false)]
        public float Y1 { get { return P1.Y; } }
        [Browsable(false)]
        public float X2 { get { return P2.X; } }
        [Browsable(false)]
        public float Y2 { get { return P2.Y; } }
        [Browsable(false)]
        public float X3 { get { return P3.X; } }
        [Browsable(false)]
        public float Y3 { get { return P3.Y; } }

        private Polyline poly;

        public Triangle(Point2D p1, Point2D p2, Point2D p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
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
            poly.Outline = Outline;
            poly.Draw(param);
        }

        public override Extents2D GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            p1.TransformBy(transformation);
            p2.TransformBy(transformation);
            p3.TransformBy(transformation);
            UpdatePolyline();
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return poly.Contains(pt, pickBoxSize);
        }
    }
}
