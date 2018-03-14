using SimpleCAD.Geometry;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace SimpleCAD.Drawables
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

        public override void Draw(Renderer renderer)
        {
            renderer.DrawArc(Style, Center, Radius, StartAngle, EndAngle);
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
            StartAngle = Vector2D.FromAngle(StartAngle).Transform(transformation).Angle;
            EndAngle = Vector2D.FromAngle(EndAngle).Transform(transformation).Angle;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            Vector2D dir = pt - Center;
            float dist = dir.Length;
            return (dist >= Radius - pickBoxSize / 2 && dist <= Radius + pickBoxSize / 2 &&
                dir.IsBetween(Vector2D.FromAngle(StartAngle), Vector2D.FromAngle(EndAngle)));
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Center"),
                new ControlPoint("Radius", ControlPoint.ControlPointType.Distance, Center, Center + Radius * Vector2D.FromAngle((StartAngle + EndAngle) / 2)),
                new ControlPoint("StartAngle", ControlPoint.ControlPointType.Angle, Center, Center + Radius * Vector2D.FromAngle(StartAngle)),
                new ControlPoint("EndAngle", ControlPoint.ControlPointType.Angle, Center, Center + Radius * Vector2D.FromAngle(EndAngle)),
            };
        }

        public Arc(BinaryReader reader) : base(reader)
        {
            Center = new Point2D(reader);
            Radius = reader.ReadSingle();
            StartAngle = reader.ReadSingle();
            EndAngle = reader.ReadSingle();
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            Center.Save(writer);
            writer.Write(Radius);
            writer.Write(StartAngle);
            writer.Write(EndAngle);
        }
    }
}
