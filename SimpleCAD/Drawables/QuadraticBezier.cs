using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class QuadraticBezier : Curve
    {
        private Point2D p0;
        private Point2D p1;
        private Point2D p2;

        public Point2D P0 { get => p0; set { p0 = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public Point2D P1 { get => p1; set { p1 = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public Point2D P2 { get => p2; set { p2 = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X0 { get { return P0.X; } }
        [Browsable(false)]
        public float Y0 { get { return P0.Y; } }
        [Browsable(false)]
        public float X1 { get { return P1.X; } }
        [Browsable(false)]
        public float Y1 { get { return P1.Y; } }
        [Browsable(false)]
        public float X2 { get { return P2.X; } }
        [Browsable(false)]
        public float Y2 { get { return P2.Y; } }

        private Polyline poly;
        private float curveLength = 4;
        private float cpSize = 0;

        public QuadraticBezier() { }

        public QuadraticBezier(Point2D p0, Point2D p1, Point2D p2)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
            UpdatePolyline();
        }

        public QuadraticBezier(float x0, float y0, float x1, float y1, float x2, float y2)
            : this(new Point2D(x0, y0), new Point2D(x1, y1), new Point2D(x2, y2))
        {
            ;
        }

        public override void Draw(Renderer renderer)
        {
            cpSize = 2 * renderer.View.ScreenToWorld(new Vector2D(renderer.View.Document.Settings.ControlPointSize, 0)).X;
            float p = poly.Length;
            float newCurveLength = renderer.View.WorldToScreen(new Vector2D(p, 0)).X;
            if (!MathF.IsEqual(newCurveLength, curveLength))
            {
                curveLength = newCurveLength;
                UpdatePolyline();
            }
            poly.Style = Style.ApplyLayer(Layer);
            renderer.Draw(poly);
        }

        private Point2D GetPoint(Point2D[] points, float t)
        {
            // de Casteljau's algorithm
            if (points.Length == 1)
            {
                return points[0];
            }
            else
            {
                Point2D[] newpoints = new Point2D[points.Length - 1];
                for (int i = 0; i < newpoints.Length; i++)
                    newpoints[i] = Point2D.Sum((1 - t) * points[i], t * points[i + 1]);
                return GetPoint(newpoints, t);
            }
        }

        private void UpdatePolyline()
        {
            poly = new Polyline();
            // Represent curved features by at most 4 pixels
            int n = (int)Math.Min(MaxCurveSegments, Math.Max(MinCurveSegments, curveLength / 4));
            float t = 0;
            float dt = 1f / n;
            for (int i = 0; i <= n; i++)
            {
                Point2D pt = GetPoint(new Point2D[] { P0, P1, P2 }, t);
                poly.Points.Add(pt);
                t += dt;
            }
            poly.Closed = false;
        }

        public override Extents2D GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(Matrix2D transformation)
        {
            P0 = P0.Transform(transformation);
            P1 = P1.Transform(transformation);
            P2 = P2.Transform(transformation);
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return poly.Contains(pt, pickBoxSize);
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Start point", P0),
                new ControlPoint("Control point", P1),
                new ControlPoint("End point", P2),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            return new[]
            {
                new SnapPoint("Start point", P0),
                new SnapPoint("End point", P2),
            };
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                if (index == 0)
                    P0 = P0.Transform(transformation);
                else if (index == 1)
                    P1 = P1.Transform(transformation);
                else if (index == 2)
                    P2 = P2.Transform(transformation);
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            P0 = reader.ReadPoint2D();
            P1 = reader.ReadPoint2D();
            P2 = reader.ReadPoint2D();
            UpdatePolyline();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(P0);
            writer.Write(P1);
            writer.Write(P2);
        }

        public override float StartParam => 0;
        public override float EndParam => 1;

        [Browsable(false)]
        public override float Area => 0;

        [Browsable(false)]
        public override bool Closed => false;

        public override float GetDistAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);

            float C1 = 1;
            float C2 = 1;
            float t1 = -1 / MathF.Sqrt(3);
            float t2 = 1 / MathF.Sqrt(3);

            return param / 2 * (C1 * ArcLengthFunc(param / 2 * t1 + param / 2) + C2 * ArcLengthFunc(param / 2 * t2 + param / 2));

            float ArcLengthFunc(float z)
            {
                float dx_dt = 2 * (1 - z) * (X1 - X0) + 2 * z * (X2 - X1);
                float dy_dt = 2 * (1 - z) * (Y1 - Y0) + 2 * z * (Y2 - Y1);
                return MathF.Sqrt(dx_dt * dx_dt + dy_dt * dy_dt);
            }
        }

        public override Point2D GetPointAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            float x = (1 - param) * (1 - param) * X0 + 2 * (1 - param) * param * X1 + param * param * X2;
            float y = (1 - param) * (1 - param) * Y0 + 2 * (1 - param) * param * Y1 + param * param * Y2;
            return new Point2D(x, y);
        }

        public override Vector2D GetNormalAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            float x = 2 * (1 - param) * (X1 - X0) + 2 * param * (X2 - X1);
            float y = 2 * (1 - param) * (Y1 - Y0) + 2 * param * (Y2 - Y1);
            return new Vector2D(x, y).Perpendicular;
        }

        public override void Reverse()
        {
            MathF.Swap(ref p0, ref p2);
        }
    }
}
