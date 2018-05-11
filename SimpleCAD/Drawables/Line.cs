using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class Line : Curve
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

        public Line() { }

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
            renderer.DrawLine(Style.ApplyLayer(Layer), StartPoint, EndPoint);
        }

        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();
            extents.Add(X1, Y1);
            extents.Add(X2, Y2);
            return extents;
        }

        public override void TransformBy(Matrix2D transformation)
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
                new ControlPoint("Start point", StartPoint),
                new ControlPoint("End point", EndPoint),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            return new[]
            {
                new SnapPoint("Start point", StartPoint),
                new SnapPoint("End point", EndPoint),
                new SnapPoint("Mid point", SnapPointType.Middle, Point2D.Average(StartPoint, EndPoint)),
            };
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                if (index == 0)
                    StartPoint = StartPoint.Transform(transformation);
                else if (index == 1)
                    EndPoint = EndPoint.Transform(transformation);
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            StartPoint = reader.ReadPoint2D();
            EndPoint = reader.ReadPoint2D();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(StartPoint);
            writer.Write(EndPoint);
        }
    }
}
