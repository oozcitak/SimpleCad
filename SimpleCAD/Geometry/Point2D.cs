using System;
using System.ComponentModel;
using System.IO;

namespace SimpleCAD.Geometry
{
    [TypeConverter(typeof(Point2DConverter))]
    public struct Point2D : IPersistable
    {
        private readonly float _x;
        private readonly float _y;

        public float X { get { return _x; } }
        public float Y { get { return _y; } }

        public static Point2D Zero { get { return new Point2D(0, 0); } }

        public Point2D(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public Point2D(System.Drawing.PointF pt)
        {
            _x = pt.X;
            _y = pt.Y;
        }

        public Point2D Transform(Matrix2D transformation)
        {
            float x = transformation.M11 * X + transformation.M12 * Y + transformation.DX;
            float y = transformation.M21 * X + transformation.M22 * Y + transformation.DY;
            return new Point2D(x, y);
        }

        public static float Distance(Point2D p1, Point2D p2)
        {
            return (p1 - p2).Length;
        }

        public static Point2D operator +(Point2D p, Vector2D v)
        {
            return new Point2D(p.X + v.X, p.Y + v.Y);
        }

        public static Point2D operator -(Point2D p, Vector2D v)
        {
            return new Point2D(p.X - v.X, p.Y - v.Y);
        }

        public static Vector2D operator -(Point2D p1, Point2D p2)
        {
            return new Vector2D(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point2D operator *(Point2D p, float f)
        {
            return new Point2D(p.X * f, p.Y * f);
        }

        public static Point2D operator *(float f, Point2D p)
        {
            return new Point2D(p.X * f, p.Y * f);
        }

        public static Point2D operator /(Point2D p, float f)
        {
            return new Point2D(p.X / f, p.Y / f);
        }

        public static explicit operator System.Drawing.PointF(Point2D a)
        {
            return new System.Drawing.PointF(a.X, a.Y);
        }

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
            return ToString("({0:F}, {1:F})", provider);
        }

        public string ToString(string format = "({0:F}, {1:F})", IFormatProvider provider = null)
        {
            return (provider == null) ?
                string.Format(format, X, Y) :
                string.Format(provider, format, X, Y);
        }

        public Point2D(BinaryReader reader)
        {
            _x = reader.ReadSingle();
            _y = reader.ReadSingle();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(_x);
            writer.Write(_y);
        }
    }
}
