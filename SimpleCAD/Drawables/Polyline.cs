using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SimpleCAD.Drawables
{
    public class Polyline : Curve
    {
        private const float PointOnLineTolerance = 1e-5f;

        private bool closed;

        public Point2DCollection Points { get; private set; }
        public override bool Closed => closed;

        public Polyline()
        {
            Points = new Point2DCollection();
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public Polyline(Point2DCollection pts)
        {
            Points = new Point2DCollection(pts);
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public Polyline(params Point2D[] pts)
        {
            Points = new Point2DCollection(pts);
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public Polyline(PointF[] pts)
        {
            Points = new Point2DCollection(pts);
            Points.CollectionChanged += Points_CollectionChanged;
        }

        private void Points_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Points");
        }

        public override void Draw(Renderer renderer)
        {
            renderer.DrawPolyline(Style.ApplyLayer(Layer), Points, Closed);
        }

        public override Extents2D GetExtents()
        {
            return Points.GetExtents();
        }

        public override void TransformBy(Matrix2D transformation)
        {
            Points.TransformBy(transformation);
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            int iend = (Closed ? Points.Count - 1 : Points.Count - 2);
            for (int i = 0; i <= iend; i++)
            {
                int j = (i == Points.Count - 1 ? 0 : i + 1);
                Line line = new Line(Points[i], Points[j]);
                if (line.Contains(pt, pickBoxSize))
                    return true;
            }
            return false;
        }

        public override Drawable Clone()
        {
            Polyline newPolyline = (Polyline)base.Clone();
            newPolyline.Points = new Point2DCollection(Points);
            return newPolyline;
        }

        public override ControlPoint[] GetControlPoints()
        {
            ControlPoint[] cp = new ControlPoint[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                cp[i] = new ControlPoint("Vertex (" + (i + 1).ToString() + ")", Points[i]);
            }
            return cp;
        }

        public override SnapPoint[] GetSnapPoints()
        {
            SnapPoint[] cp = new SnapPoint[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                cp[i] = new SnapPoint("Vertex (" + (i + 1).ToString() + ")", Points[i]);
            }
            return cp;
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                Points[index] = Points[index].Transform(transformation);
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            Points = reader.ReadPoint2DCollection();
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Points);
        }

        public void Open() => closed = false;
        public void Close() => closed = true;

        public override float StartParam => 0;
        public override float EndParam => Points.Count - 1;

        public override float Area
        {
            get
            {
                if (Points.Count < 3)
                    return 0;

                Segment2D[] segments = new Segment2D[Points.Count];
                for (int i = 0; i < Points.Count; i++)
                {
                    Point2D p1 = Points[i];
                    Point2D p2 = (i == Points.Count - 1) ? Points[0] : Points[i + 1];
                    segments[i] = new Segment2D(p1, p2);
                }
                float area = 0;
                foreach (Segment2D s in segments)
                {
                    area += (s.X2 - s.X1) * (s.Y1 + s.Y2) / 2;
                }
                return Math.Abs(area);
            }
        }

        public override float GetDistAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            float dist = 0;
            var lastPt = Points[0];
            for (int i = 1; i < Points.Count; i++)
            {
                if (param >= i)
                {
                    dist += (Points[i] - lastPt).Length;
                }
                else
                {
                    dist += (param - (i - 1)) * (Points[i] - lastPt).Length;
                    return dist;
                }
                lastPt = Points[i];
            }
            return dist;
        }

        public override Point2D GetPointAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            int i = (int)Math.Floor(param);
            if (i == Points.Count - 1) i--;
            return Points[i] + (param - i) * (Points[i + 1] - Points[i]);
        }

        public override Vector2D GetNormalAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            int i = (int)Math.Floor(param);
            if (i == Points.Count - 1) i--;
            return (Points[i + 1] - Points[i]).Perpendicular;
        }

        public override float GetParamAtDist(float dist)
        {
            if (dist < 0) return StartParam;

            for (int i = 1; i < Points.Count; i++)
            {
                float segmentLength = (Points[i] - Points[i - 1]).Length;

                if (MathF.IsZero(dist))
                    return i - 1;
                else if (MathF.IsEqual(dist, segmentLength))
                    return i;
                else if (segmentLength > dist)
                    return dist / segmentLength + (i - 1);

                dist -= segmentLength;
            }

            return EndParam;
        }

        public override float GetParamAtPoint(Point2D pt)
        {
            for (int i = 1; i < Points.Count; i++)
            {
                float dist = (pt - Points[i - 1]).Length;
                Segment2D seg = new Segment2D(Points[i - 1], Points[i]);

                bool onSegment = seg.Contains(pt, PointOnLineTolerance, out float t);
                if (!onSegment && t < 0 && i - 1 == 0)
                    return StartParam;
                else if (!onSegment && t > 1 && i == Points.Count - 1)
                    return EndParam;
                else if (!onSegment)
                    continue;

                float segmentLength = seg.Length;
                if (MathF.IsZero(dist))
                    return i - 1;
                else if (MathF.IsEqual(dist, segmentLength))
                    return i;
                else if (segmentLength > dist)
                    return dist / segmentLength + (i - 1);
            }

            return EndParam;
        }

        public override void Reverse()
        {
            for (int i = 0; i < Points.Count / 2; i++)
            {
                int j = Points.Count - 1 - i;
                Point2D temp = Points[i];
                Points[i] = Points[j];
                Points[j] = temp;
            }
        }

        public override bool Split(float[] @params, out Curve[] subCurves)
        {
            @params = ValidateParams(@params);
            if (!Closed)
            {
                if (@params.Length == 0)
                {
                    subCurves = new Curve[0];
                    return false;
                }

                subCurves = new Curve[@params.Length + 1];
                for (int i = 0; i < @params.Length + 1; i++)
                {
                    float ps = (i == 0 ? StartParam : @params[i - 1]);
                    float pe = (i == @params.Length ? EndParam : @params[i]);

                    Point2DCollection newPoints = new Point2DCollection();
                    foreach (float p in ParamIterator(ps, pe))
                    {
                        newPoints.Add(GetPointAtParam(p));
                    }
                    subCurves[i] = new Polyline(newPoints);
                }

                return true;
            }
            else
            {
                if (@params.Length < 2)
                {
                    subCurves = new Curve[0];
                    return false;
                }

                subCurves = new Curve[@params.Length];
                for (int i = 0; i < @params.Length; i++)
                {
                    float ps = @params[i];
                    float pe = (i == @params.Length - 1 ? @params[0] : @params[i + 1]);

                    Point2DCollection newPoints = new Point2DCollection();
                    foreach (float p in ParamIterator(ps, pe))
                    {
                        newPoints.Add(GetPointAtParam(p));
                    }
                    subCurves[i] = new Polyline(newPoints);
                }

                return true;
            }
        }

        private IEnumerable<float> ParamIterator(float startParam, float endParam)
        {
            yield return startParam;
            float p = (int)MathF.Floor(startParam) + 1;
            bool flag = true;
            while (flag)
            {
                yield return p;
                p++;
                if (p == Points.Count) p = 0;

                if (endParam > startParam && p > endParam)
                    flag = false;
                else if (endParam < startParam && p < startParam && p > endParam)
                    flag = false;
            }
            yield return endParam;
        }
    }
}
