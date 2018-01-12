using System;

namespace SimpleCAD
{
    public struct Point2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static Point2D Zero { get { return new Point2D(0, 0); } }

        public Point2D(float x, float y)
            : this()
        {
            X = x;
            Y = y;
        }

        public void TransformBy(Matrix2D transformation)
        {
            float x = transformation.M11 * X + transformation.M12 * Y + transformation.DX;
            float y = transformation.M21 * X + transformation.M22 * Y + transformation.DY;
            X = x;
            Y = y;
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

        public static implicit operator System.Drawing.PointF(Point2D p)
        {
            return new System.Drawing.PointF(p.X, p.Y);
        }

        public static implicit operator Point2D(System.Drawing.PointF p)
        {
            return new Point2D(p.X, p.Y);
        }

        public Vector2D AsVector2D()
        {
            return new Vector2D(X, Y);
        }

        public override string ToString()
        {
            return "{" + X.ToString() + ", " + Y.ToString() + "}";
        }
    }
}
