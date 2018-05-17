using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class Arc : Curve
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

        public Arc() { }

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
            renderer.DrawArc(Style.ApplyLayer(Layer), Center, Radius, StartAngle, EndAngle);
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
                new ControlPoint("Center point", Center),
                new ControlPoint("Radius", ControlPointType.Distance, Center, Center + Radius * Vector2D.FromAngle((StartAngle + EndAngle) / 2)),
                new ControlPoint("Start angle", ControlPointType.Angle, Center, Center + Radius * Vector2D.FromAngle(StartAngle)),
                new ControlPoint("End angle", ControlPointType.Angle, Center, Center + Radius * Vector2D.FromAngle(EndAngle)),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            return new[]
            {
                new SnapPoint("Center point", SnapPointType.Center, Center),
                new SnapPoint("Start point", Center + Radius * Vector2D.FromAngle(StartAngle)),
                new SnapPoint("End point", Center + Radius * Vector2D.FromAngle(EndAngle)),
                new SnapPoint("Mid point", SnapPointType.Middle, Center + Radius * Vector2D.FromAngle(StartAngle + (EndAngle - StartAngle) / 2)),
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
                else if (index == 2)
                    StartAngle = Vector2D.FromAngle(StartAngle).Transform(transformation).Angle;
                else if (index == 3)
                    EndAngle = Vector2D.FromAngle(EndAngle).Transform(transformation).Angle;
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            Center = reader.ReadPoint2D();
            Radius = reader.ReadFloat();
            StartAngle = reader.ReadFloat();
            EndAngle = reader.ReadFloat();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Center);
            writer.Write(Radius);
            writer.Write(StartAngle);
            writer.Write(EndAngle);
        }

        public override float StartParam => StartAngle;
        public override float EndParam => EndAngle;

        public override float Area => (Math.Abs(EndAngle - StartAngle) - MathF.Sin(System.Math.Abs(EndAngle - StartAngle))) / 2 * Radius * Radius;

        [Browsable(false)]
        public override bool Closed => false;

        public override float GetDistAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            return (param - StartParam) * Radius;
        }

        public override Point2D GetPointAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            return Center + Vector2D.FromAngle(param) * Radius;
        }

        public override Vector2D GetNormalAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            return Vector2D.FromAngle(param);
        }

        public override float GetParamAtDist(float dist)
        {
            float param = dist / Radius + StartParam;
            return MathF.Clamp(param, StartParam, EndParam);
        }

        public override float GetParamAtPoint(Point2D pt)
        {
            float param = ((pt - Center) / Radius).Angle;
            return MathF.Clamp(param, StartParam, EndParam);
        }

        public override void Reverse()
        {
            MathF.Swap(ref startAngle, ref endAngle);
        }
    }
}
