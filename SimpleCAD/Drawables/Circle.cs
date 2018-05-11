using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class Circle : Curve
    {
        private Point2D center;
        private float radius;

        public Point2D Center { get => center; set { center = value; NotifyPropertyChanged(); } }
        public float Radius { get => radius; set { radius = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return Center.X; } }
        [Browsable(false)]
        public float Y { get { return Center.Y; } }

        public Circle() { }

        public Circle(Point2D center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Circle(float x, float y, float radius)
            : this(new Point2D(x, y), radius)
        {
            ;
        }

        public override void Draw(Renderer renderer)
        {
            renderer.DrawCircle(Style.ApplyLayer(Layer), Center, Radius);
        }

        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();
            extents.Add(X - Radius, Y - Radius);
            extents.Add(X + Radius, Y + Radius);
            return extents;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            Center = Center.Transform(transformation);
            Radius = (Vector2D.XAxis * Radius).Transform(transformation).Length;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            float dist = (pt - Center).Length;
            return dist <= Radius + pickBoxSize / 2 && dist >= Radius - pickBoxSize / 2;
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Center point", Center),
                new ControlPoint("Radius", ControlPointType.Distance, Center, Center + Radius * Vector2D.XAxis),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            return new[]
            {
                new SnapPoint("Center point", SnapPointType.Center, Center),
                new SnapPoint("East quadrant", SnapPointType.Quadrant, Center + Radius * Vector2D.XAxis),
                new SnapPoint("North quadrant", SnapPointType.Quadrant, Center + Radius * Vector2D.YAxis),
                new SnapPoint("West quadrant", SnapPointType.Quadrant, Center - Radius * Vector2D.XAxis),
                new SnapPoint("South quadrant", SnapPointType.Quadrant, Center - Radius * Vector2D.YAxis),
            };
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                if (index == 0)
                    Center = Center.Transform(transformation);
                else if (index == 1)
                    Radius = Vector2D.XAxis.Transform(transformation).Length * Radius;
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            Center = reader.ReadPoint2D();
            Radius = reader.ReadFloat();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Center);
            writer.Write(Radius);
        }
    }
}
