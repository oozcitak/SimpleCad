using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class Parabola : Curve
    {
        private Point2D p1;
        private Point2D p2;

        public new Point2D StartPoint { get => p1; set { p1 = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public new Point2D EndPoint { get => p2; set { p2 = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        public Point2D IntersectionPoint
        {
            get
            {
                CurveOperations.IntersectLines(StartPoint, StartPoint + Vector2D.FromAngle(StartAngle), EndPoint, EndPoint + Vector2D.FromAngle(EndAngle), out _, out _, out Point2D c);
                return c;
            }
        }

        private float startAngle;
        private float endAngle;

        public float StartAngle { get => startAngle; set { startAngle = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float EndAngle { get => endAngle; set { endAngle = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X1 { get { return StartPoint.X; } }
        [Browsable(false)]
        public float Y1 { get { return StartPoint.Y; } }
        [Browsable(false)]
        public float X2 { get { return EndPoint.X; } }
        [Browsable(false)]
        public float Y2 { get { return EndPoint.Y; } }
        [Browsable(false)]
        public float XI { get { return IntersectionPoint.X; } }
        [Browsable(false)]
        public float YI { get { return IntersectionPoint.Y; } }

        private QuadraticBezier poly;
        private float curveLength = 4;
        private float cpSize = 0;

        public Parabola() { }

        public Parabola(Point2D p1, Point2D p2, float startAngle, float endAngle)
        {
            StartPoint = p1;
            EndPoint = p2;
            StartAngle = startAngle;
            EndAngle = endAngle;
            UpdatePolyline();
        }

        public Parabola(float x1, float y1, float x2, float y2, float startAngle, float endAngle)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), startAngle, endAngle)
        {
            ;
        }

        public Parabola(Point2D p1, Point2D intPoint, Point2D p2) : this(p1, p2, (intPoint - p1).Angle, (intPoint - p2).Angle)
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

        private void UpdatePolyline()
        {
            poly = new QuadraticBezier(StartPoint, IntersectionPoint, EndPoint);
        }

        public override Extents2D GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(Matrix2D transformation)
        {
            StartPoint = StartPoint.Transform(transformation);
            EndPoint = EndPoint.Transform(transformation);
            StartAngle = Vector2D.FromAngle(StartAngle).Transform(transformation).Angle;
            EndAngle = Vector2D.FromAngle(EndAngle).Transform(transformation).Angle;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return poly.Contains(pt, pickBoxSize);
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Start point", StartPoint),
                new ControlPoint("End point", EndPoint),
                new ControlPoint("Start angle", ControlPointType.Angle, StartPoint, StartPoint + cpSize * Vector2D.FromAngle(StartAngle)),
                new ControlPoint("End angle", ControlPointType.Angle, EndPoint, EndPoint + cpSize * Vector2D.FromAngle(EndAngle)),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            return new[]
            {
                new SnapPoint("Start point", StartPoint),
                new SnapPoint("End point", EndPoint),
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
                else if (index == 2)
                    StartAngle = Vector2D.FromAngle(StartAngle).Transform(transformation).Angle;
                else if (index == 3)
                    EndAngle = Vector2D.FromAngle(EndAngle).Transform(transformation).Angle;
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            StartPoint = reader.ReadPoint2D();
            EndPoint = reader.ReadPoint2D();
            StartAngle = reader.ReadFloat();
            EndAngle = reader.ReadFloat();
            UpdatePolyline();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(StartPoint);
            writer.Write(EndPoint);
            writer.Write(StartAngle);
            writer.Write(EndAngle);
        }

        public override float StartParam => 0;
        public override float EndParam => 1;

        [Browsable(false)]
        public override float Area => 0;

        [Browsable(false)]
        public override bool Closed => false;

        public override float GetDistAtParam(float z)
        {
            float C1 = 1;
            float C2 = 1;
            float t1 = -1 / MathF.Sqrt(3);
            float t2 = 1 / MathF.Sqrt(3);

            return z / 2 * (C1 * ArcLengthFunc(z / 2 * t1 + z / 2) + C2 * ArcLengthFunc(z / 2 * t2 + z / 2));
        }

        private float ArcLengthFunc(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            float dx_dt = 2 * (1 - param) * (XI - X1) + 2 * param * (X2 - XI);
            float dy_dt = 2 * (1 - param) * (YI - Y1) + 2 * param * (Y2 - YI);
            return MathF.Sqrt(dx_dt * dx_dt + dy_dt * dy_dt);
        }

        public override Point2D GetPointAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            float x = (1 - param) * (1 - param) * X1 + 2 * (1 - param) * param * XI + param * param * X2;
            float y = (1 - param) * (1 - param) * Y1 + 2 * (1 - param) * param * YI + param * param * Y2;
            return new Point2D(x, y);
        }

        public override Vector2D GetNormalAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            float x = 2 * (1 - param) * (XI - X1) + 2 * param * (X2 - XI);
            float y = 2 * (1 - param) * (YI - Y1) + 2 * param * (Y2 - YI);
            return new Vector2D(x, y).Perpendicular;
        }

        public override void Reverse()
        {
            MathF.Swap(ref p1, ref p2);
            MathF.Swap(ref startAngle, ref endAngle);
        }

        public override bool Split(float[] @params, out Curve[] subCurves)
        {
            QuadraticBezier bezier = new QuadraticBezier(StartPoint, IntersectionPoint, EndPoint);
            if (!bezier.Split(@params, out subCurves))
                return false;

            for (int i = 0; i < subCurves.Length; i++)
            {
                QuadraticBezier bezierpiece = (QuadraticBezier)subCurves[i];
                subCurves[i] = new Parabola(bezierpiece.P0, bezierpiece.P1, bezierpiece.P2);
            }

            return true;
        }
    }
}
