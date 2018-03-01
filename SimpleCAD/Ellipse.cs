using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class Ellipse : Drawable
    {
        private Point2D center;

        public Point2D Center { get => center; set { center = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return Center.X; } }
        [Browsable(false)]
        public float Y { get { return Center.Y; } }

        private float semiMajorAxis;
        private float semiMinorAxis;
        private float rotation;

        public float SemiMajorAxis { get => semiMajorAxis; set { semiMajorAxis = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float SemiMinorAxis { get => semiMinorAxis; set { semiMinorAxis = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float Rotation { get => rotation; set { rotation = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        private Polyline poly;
        float curveLength = 4;

        public Ellipse(Point2D center, float semiMajor, float semiMinor, float rotation = 0)
        {
            Center = center;
            SemiMajorAxis = semiMajor;
            SemiMinorAxis = semiMinor;
            Rotation = rotation;
            UpdatePolyline();
        }

        public Ellipse(float x, float y, float semiMajor, float semiMinor, float rotation = 0)
            : this(new Point2D(x, y), semiMajor, semiMinor, rotation)
        {
            ;
        }

        private void UpdatePolyline()
        {
            poly = new Polyline();
            // Represent curved features by at most 4 pixels
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;
            float a = 0;
            for (int i = 0; i < n; i++)
            {
                float x = SemiMajorAxis * MathF.Cos(a);
                float y = SemiMinorAxis * MathF.Sin(a);
                poly.Points.Add(x, y);
                a += da;
            }
            poly.Closed = true;
            poly.TransformBy(TransformationMatrix2D.Rotation(Rotation));
            poly.TransformBy(TransformationMatrix2D.Translation(Center.X, Center.Y));
        }

        public override void Draw(DrawParams param)
        {
            // Approximate perimeter (Ramanujan)
            float p = 2 * MathF.PI * (3 * (SemiMajorAxis + SemiMinorAxis) - MathF.Sqrt((3 * SemiMajorAxis + SemiMinorAxis) * (SemiMajorAxis + 3 * SemiMinorAxis)));
            float newCurveLength = param.ModelToView(p);
            if (!MathF.IsEqual(newCurveLength, curveLength))
            {
                curveLength = newCurveLength;
                UpdatePolyline();
            }
            poly.OutlineStyle = OutlineStyle;
            poly.Draw(param);
        }

        public override Extents GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Point2D p = Center;
            p.TransformBy(transformation);
            Center = p;

            Vector2D dir = Vector2D.FromAngle(Rotation);
            dir.TransformBy(transformation);
            Rotation = dir.Angle;

            Vector2D unit = Vector2D.XAxis;
            unit.TransformBy(transformation);
            SemiMajorAxis = unit.Length * SemiMajorAxis;
            SemiMinorAxis = unit.Length * SemiMinorAxis;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return poly.Contains(pt, pickBoxSize);
        }
    }
}
