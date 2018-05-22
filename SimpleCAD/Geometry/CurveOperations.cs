using System;

namespace SimpleCAD.Geometry
{
    public static class CurveOperations
    {
        public static bool IntersectLines(Point2D p1, Vector2D r1, Point2D p2, Vector2D r2, out float t1, out float t2, out Point2D intPt)
        {
            float r1xr2 = r1.CrossProduct(r2);

            if (MathF.IsZero(r1xr2))
            {
                t1 = 0;
                t2 = 0;
                intPt = Point2D.Zero;
                return false;
            }

            t1 = (p2 - p1).CrossProduct(r2) / r1xr2;
            t2 = (p2 - p1).CrossProduct(r1) / r1xr2;
            intPt = p1 + t1 * r1;
            return true;
        }

        public static bool IntersectLines(Point2D p1, Point2D p2, Point2D p3, Point2D p4, out float t1, out float t2, out Point2D intPt)
        {
            return IntersectLines(p1, p2 - p1, p3, p4 - p3, out t1, out t2, out intPt);
        }

        public static bool IntersectLineCircle(Point2D p1, Point2D p2, Point2D center, float r, out float[] t1, out float[] t2, out Point2D[] points)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float dr = MathF.Sqrt(dx * dx + dy * dy);
            float D = MathF.Determinant(p1.X, p2.X, p1.Y, p2.Y);
            float discriminant = r * r * dr * dr - D * D;

            if (!MathF.IsZero(dr))
            {
                float x1 = D * dy + Math.Sign(dy) * dx * MathF.Sqrt(discriminant) / (dr * dr);
                float y1 = -D * dx + Math.Abs(dy) * dx * MathF.Sqrt(discriminant) / (dr * dr);
                float x2 = D * dy - Math.Sign(dy) * dx * MathF.Sqrt(discriminant) / (dr * dr);
                float y2 = -D * dx - Math.Abs(dy) * dx * MathF.Sqrt(discriminant) / (dr * dr);

                if (MathF.IsZero(discriminant))
                {
                    // tangent
                    Point2D intPt = new Point2D(x1, y1);
                    points = new Point2D[] { intPt };
                    t1 = new float[] { (intPt - p1).Length / dr };
                    t2 = new float[] { (intPt - center).Angle / MathF.TAU };
                    return true;
                }
                else if (discriminant < 0)
                {
                    // no intersection
                    points = new Point2D[0];
                    t1 = new float[0];
                    t2 = new float[0];
                    return false;
                }
                else
                {
                    // intersection
                    Point2D intPt1 = new Point2D(x1, y1);
                    Point2D intPt2 = new Point2D(x2, y2);
                    points = new Point2D[] { intPt1, intPt2 };
                    t1 = new float[] { (intPt1 - p1).Length / dr, (intPt2 - p1).Length / dr };
                    t2 = new float[] { (intPt1 - center).Angle / MathF.TAU, (intPt2 - center).Angle / MathF.TAU };
                    return true;
                }
            }
            else
            {
                points = new Point2D[0];
                t1 = new float[0];
                t2 = new float[0];
                return false;
            }
        }

        public static bool IntersectLineQuadraticBezier(Point2D p1, Point2D p2, Point2D pb1, Point2D pb2, Point2D pb3, out float[] t1, out float[] t2, out Point2D[] points)
        {
            Point2D[] p = new Point2D[] { pb1, pb2, pb3 };
            Point2D[] tp = AlignPointsToLine(p, p1, p2);

            float a = tp[0].Y;
            float b = tp[1].Y;
            float c = tp[2].Y;
            float d = a - 2 * b + c;

            if (!MathF.IsZero(d))
            {
                float m1 = -MathF.Sqrt(b * b - a * c);
                float m2 = -a + b;
                float v1 = -(m1 + m2) / d;
                float v2 = -(-m1 + m2) / d;
                points = new Point2D[] { GetQuadraticBezierPoint(p[0], p[1], p[2], v1), GetQuadraticBezierPoint(p[0], p[1], p[2], v2) };
                t1 = new float[] { (points[0] - p1).Length / (p2 - p1).Length, (points[1] - p1).Length / (p2 - p1).Length };
                t2 = new float[] { v1, v2 };
                return true;
            }
            else if (!MathF.IsEqual(b, c))
            {
                float v = (2 * b - c) / 2 * (b - c);
                points = new Point2D[] { GetQuadraticBezierPoint(p[0], p[1], p[2], v) };
                t1 = new float[] { (points[0] - p1).Length / (p2 - p1).Length };
                t2 = new float[] { v };
                return true;
            }
            else
            {
                points = new Point2D[0];
                t1 = new float[0];
                t2 = new float[0];
                return false;
            }
        }

        public static bool IntersectCircles(Point2D c1, float r1, Point2D c2, float r2, out float[] t1, out float[] t2, out Point2D[] points)
        {
            float d = (c2 - c1).Length;
            if (MathF.IsZero(d) || d > r1 + r2 || d < Math.Abs(r1 - r2))
            {
                points = new Point2D[0];
                t1 = new float[0];
                t2 = new float[0];
                return false;
            }

            float a = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
            Point2D midPt = c1 + a * (c2 - c1) / d;

            if (MathF.IsEqual(d, r1 + r2))
            {
                // circles touch at one point
                points = new Point2D[] { midPt };
                t1 = new float[] { (midPt - c1).Angle / MathF.TAU };
                t2 = new float[] { (midPt - c2).Angle / MathF.TAU };
            }
            else
            {
                // intersection
                float h = MathF.Sqrt(r1 * r1 - a * a);
                Vector2D normal = (c2 - c1).Perpendicular.Normal;
                Point2D intPt1 = midPt + h * normal;
                Point2D intPt2 = midPt - h * normal;
                points = new Point2D[] { intPt1, intPt2 };
                t1 = new float[] { (intPt1 - c1).Angle / MathF.TAU, (intPt2 - c1).Angle / MathF.TAU };
                t2 = new float[] { (intPt1 - c2).Angle / MathF.TAU, (intPt2 - c2).Angle / MathF.TAU };
            }

            return true;
        }

        private static Point2D[] AlignPointsToLine(Point2D[] p, Point2D p1, Point2D p2)
        {
            float tx = p1.X;
            float ty = p1.Y;
            float a = -MathF.Atan2(p2.Y - ty, p2.X - tx);
            Point2D[] tp = new Point2D[p.Length];
            for (int i = 0; i < 3; i++)
            {
                Point2D v = p[i];
                v = new Point2D((v.X - tx) * MathF.Cos(a) - (v.Y - ty) * MathF.Sin(a),
                    (v.X - tx) * MathF.Sin(a) + (v.Y - ty) * MathF.Cos(a));
                tp[i] = v;
            }
            return tp;
        }

        private static Point2D GetQuadraticBezierPoint(Point2D p1, Point2D p2, Point2D p3, float t)
        {
            return Point2D.Sum((1 - t) * (1 - t) * p1, 2 * (1 - t) * t * p2, t * t * p3);
        }
    }
}
