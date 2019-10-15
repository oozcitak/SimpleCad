using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.Collections.Generic;

namespace SimpleCAD.Drawables
{
    public class CompositeReference : Drawable
    {
        private Lazy<Composite> compositeRef = new Lazy<Composite>();
        private Point2D location = Point2D.Zero;
        private float rotation = 0;
        private float scale = 1;

        public Point2D Location { get => location; set { location = value; NotifyPropertyChanged(); } }
        public float Scale { get => scale; set { scale = value; NotifyPropertyChanged(); } }
        public float Rotation { get => rotation; set { rotation = value; NotifyPropertyChanged(); } }
        public Composite Composite { get => compositeRef.Value; set { compositeRef = new Lazy<Composite>(() => value); NotifyPropertyChanged(); } }

        public CompositeReference() { }

        public override void Load(DocumentReader reader)
        {
            var doc = reader.Document;
            base.Load(reader);
            Location = reader.ReadPoint2D();
            Scale = reader.ReadFloat();
            Rotation = reader.ReadFloat();
            string compositeName = reader.ReadString();
            compositeRef = new Lazy<Composite>(() => doc.Composites[compositeName]);
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Location);
            writer.Write(Scale);
            writer.Write(Rotation);
            writer.Write(Composite.Name);
        }

        public override void Draw(Renderer renderer)
        {
            var matrix = renderer.Transform;
            renderer.Transform = renderer.Transform * GetMatrix();
            renderer.Draw(Composite);
            renderer.Transform = matrix;
        }

        public override Extents2D GetExtents()
        {
            Extents2D extents = Composite.GetExtents();
            extents.TransformBy(GetMatrix());
            return extents;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            pt = pt.Transform(GetMatrix().Inverse);
            return Composite.Contains(pt, pickBoxSize);
        }

        private Matrix2D GetMatrix()
        {
            return Matrix2D.Transformation(Scale, Scale, Rotation, Location.X, Location.Y);
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Location", Location),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            List<SnapPoint> points = new List<SnapPoint>();
            points.Add(new SnapPoint("Location", SnapPointType.Point, Location));
            var matrix = GetMatrix();
            foreach (Drawable d in Composite)
            {
                if (d.Visible && (d.Layer == null || d.Layer.Visible))
                {
                    foreach (SnapPoint pt in d.GetSnapPoints())
                    {
                        points.Add(new SnapPoint(pt.Name, pt.Type, pt.Location.Transform(matrix)));
                    }
                }
            }
            return points.ToArray();
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                if (index == 0)
                    Location = Location.Transform(transformation);
            }
        }

        public override void TransformBy(Matrix2D transformation)
        {
            Location = Location.Transform(transformation);
            Rotation = Vector2D.FromAngle(Rotation).Transform(transformation).Angle;
            Scale = new Vector2D(Scale, 0).Transform(transformation).Length;
        }

        public override Drawable Clone()
        {
            return (Drawable)MemberwiseClone();
        }
    }
}
