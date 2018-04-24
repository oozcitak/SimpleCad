using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SimpleCAD.Geometry
{
    [TypeConverter(typeof(Point2DConverter))]
    public struct Point2D
    {
        public float X { get; }
        public float Y { get; }

        public static Point2D Zero { get { return new Point2D(0, 0); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point2D(System.Drawing.PointF pt)
        {
            X = pt.X;
            Y = pt.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point2D Transform(Matrix2D transformation)
        {
            float x = transformation.M11 * X + transformation.M12 * Y + transformation.DX;
            float y = transformation.M21 * X + transformation.M22 * Y + transformation.DY;
            return new Point2D(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Point2D p1, Point2D p2)
        {
            return (p1 - p2).Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point2D operator +(Point2D p, Vector2D v)
        {
            return new Point2D(p.X + v.X, p.Y + v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point2D operator -(Point2D p, Vector2D v)
        {
            return new Point2D(p.X - v.X, p.Y - v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2D operator -(Point2D p1, Point2D p2)
        {
            return new Vector2D(p1.X - p2.X, p1.Y - p2.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point2D operator *(Point2D p, float f)
        {
            return new Point2D(p.X * f, p.Y * f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point2D operator *(float f, Point2D p)
        {
            return new Point2D(p.X * f, p.Y * f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point2D operator /(Point2D p, float f)
        {
            return new Point2D(p.X / f, p.Y / f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator System.Drawing.PointF(Point2D a)
        {
            return new System.Drawing.PointF(a.X, a.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2D AsVector2D()
        {
            return new Vector2D(X, Y);
        }

        public static Point2D Average(params Point2D[] points)
        {
            int n = points.Length;
            float x = 0;
            float y = 0;
            foreach (Point2D pt in points)
            {
                x += pt.X / n;
                y += pt.Y / n;
            }
            return new Point2D(x, y);
        }

        public string ToString(IFormatProvider provider)
        {
            return ToString("{0:F}, {1:F}", provider);
        }

        public string ToString(string format = "{0:F}, {1:F}", IFormatProvider provider = null)
        {
            return (provider == null) ?
                string.Format(format, X, Y) :
                string.Format(provider, format, X, Y);
        }

        public static bool TryParse(string s, out Point2D result)
        {
            Point2DConverter conv = new Point2DConverter();
            if (conv.IsValid(s))
            {
                result = (Point2D)conv.ConvertFrom(s);
                return true;
            }
            else
            {
                result = Point2D.Zero;
                return false;
            }
        }
    }
}
