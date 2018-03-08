using SimpleCAD.Geometry;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace SimpleCAD.Drawables
{
    public class Line : Drawable, IPersistable
    {
        private Point2D p1;
        private Point2D p2;

        public Point2D StartPoint { get => p1; set { p1 = value; NotifyPropertyChanged(); } }
        public Point2D EndPoint { get => p2; set { p2 = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X1 { get { return StartPoint.X; } }
        [Browsable(false)]
        public float Y1 { get { return StartPoint.Y; } }
        [Browsable(false)]
        public float X2 { get { return EndPoint.X; } }
        [Browsable(false)]
        public float Y2 { get { return EndPoint.Y; } }

        public Line(Point2D p1, Point2D p2)
        {
            StartPoint = p1;
            EndPoint = p2;
        }

        public Line(float x1, float y1, float x2, float y2)
            : this(new Point2D(x1, y1), new Point2D(x2, y2))
        {
            ;
        }

        public override void Draw(Renderer renderer)
        {
            renderer.DrawLine(Style, StartPoint, EndPoint);
        }

        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();
            extents.Add(X1, Y1);
            extents.Add(X2, Y2);
            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            StartPoint = StartPoint.Transform(transformation);
            EndPoint = EndPoint.Transform(transformation);
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            Vector2D w = pt - StartPoint;
            Vector2D vL = (EndPoint - StartPoint);
            float b = w.DotProduct(vL) / vL.DotProduct(vL);
            float dist = (w - b * vL).Length;
            return b >= 0 && b <= 1 && dist <= pickBoxSize / 2;
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("StartPoint"),
                new ControlPoint("EndPoint"),
            };
        }

        public Line(BinaryReader reader) : base(reader)
        {
            StartPoint = new Point2D(reader);
            EndPoint = new Point2D(reader);
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            StartPoint.Save(writer);
            EndPoint.Save(writer);
        }
    }
}
