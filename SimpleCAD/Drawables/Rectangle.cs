using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class Rectangle : Curve
    {
        private Point2D center;

        public Point2D Center { get => center; set { center = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        private float width;
        private float height;
        private float rotation;

        public float Width { get => width; set { width = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float Height { get => height; set { height = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float Rotation { get => rotation; set { rotation = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        private Polyline poly;
        private float cpSize = 0;

        public Rectangle() { }

        public Rectangle(Point2D center, float w, float h, float rotation = 0)
        {
            Center = center;
            Width = w;
            Height = h;
            Rotation = rotation;
            UpdatePolyline();
        }

        public Rectangle(float x, float y, float w, float h, float rotation = 0)
            : this(new Point2D(x, y), w, h, rotation)
        {
            ;
        }

        private void UpdatePolyline()
        {
            poly = new Polyline();
            poly.Points.Add(-Width / 2, -Height / 2);
            poly.Points.Add(+Width / 2, -Height / 2);
            poly.Points.Add(+Width / 2, +Height / 2);
            poly.Points.Add(-Width / 2, +Height / 2);
            poly.Closed = true;
            poly.TransformBy(Matrix2D.Rotation(Rotation));
            poly.TransformBy(Matrix2D.Translation(Center.X, Center.Y));
        }

        public override void Draw(Renderer renderer)
        {
            cpSize = 2 * renderer.View.ScreenToWorld(new Vector2D(renderer.View.Document.Settings.ControlPointSize, 0)).X;
            poly.Style = Style.ApplyLayer(Layer);
            renderer.Draw(poly);
        }

        public override Extents2D GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(Matrix2D transformation)
        {
            Center = Center.Transform(transformation);
            Rotation = Vector2D.FromAngle(Rotation).Transform(transformation).Angle;
            Width = (Vector2D.XAxis * Width).Transform(transformation).Length;
            Height = (Vector2D.YAxis * Height).Transform(transformation).Length;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return poly.Contains(pt, pickBoxSize);
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Center point", Center),
                new ControlPoint("Width", ControlPointType.Distance, Center, Center + Width / 2 * Vector2D.FromAngle(Rotation)),
                new ControlPoint("Height", ControlPointType.Distance, Center, Center + Height / 2 * Vector2D.FromAngle(Rotation).Perpendicular),
                new ControlPoint("Rotation", ControlPointType.Angle, Center, Center + (Width / 2 + cpSize) * Vector2D.FromAngle(Rotation)),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            return poly.GetSnapPoints();
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                if (index == 0)
                    Center = Center.Transform(transformation);
                else if (index == 1)
                    Width = Vector2D.XAxis.Transform(transformation).Length * Width;
                else if (index == 2)
                    Height = Vector2D.YAxis.Transform(transformation).Length * Height;
                else if (index == 3)
                    Rotation = Vector2D.FromAngle(Rotation).Transform(transformation).Angle;
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            Center = reader.ReadPoint2D();
            Width = reader.ReadFloat();
            Height = reader.ReadFloat();
            Rotation = reader.ReadFloat();
            UpdatePolyline();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Center);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Rotation);
        }

        public override float StartParam => poly.StartParam;
        public override float EndParam => poly.EndParam;

        public override float Area => Width * Height;

        [Browsable(false)]
        public override bool Closed => true;

        public override float GetDistAtParam(float param) => poly.GetDistAtParam(param);
        public override Point2D GetPointAtParam(float param) => poly.GetPointAtParam(param);
        public override Vector2D GetNormalAtParam(float param) => poly.GetNormalAtParam(param);
        public override float GetParamAtPoint(Point2D pt) => poly.GetParamAtPoint(pt);

        public override void Reverse() { }

        public override bool Split(float[] @params, out Curve[] subCurves) => poly.Split(@params, out subCurves);
    }
}
