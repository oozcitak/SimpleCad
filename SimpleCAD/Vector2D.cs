using System;

namespace SimpleCAD
{
    public struct Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Length { get { return (float)Math.Sqrt(X * X + Y * Y); } }
        public float Angle { get { return AngleTo(Vector2D.XAxis); } }

        public static Vector2D XAxis { get { return new Vector2D(1, 0); } }
        public static Vector2D YAxis { get { return new Vector2D(0, 1); } }

        public Vector2D(float x, float y)
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

        public void Normalize()
        {
            float len = Length;
            X /= len;
            Y /= len;
        }

        public float DotProduct(Vector2D v)
        {
            return X * v.X + Y * v.Y;
        }

        public float AngleTo(Vector2D v)
        {
            return (float)Math.Acos(DotProduct(v) / Length / v.Length);
        }

        public static Vector2D FromAngle(float angle)
        {
            return new Vector2D((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public Vector2D GetPerpendicularVector()
        {
            return new Vector2D(-Y, X);
        }

        public static Vector2D operator *(Vector2D p, float f)
        {
            return new Vector2D(p.X * f, p.Y * f);
        }

        public static Vector2D operator *(float f, Vector2D p)
        {
            return new Vector2D(p.X * f, p.Y * f);
        }

        public static Vector2D operator /(Vector2D p, float f)
        {
            return new Vector2D(p.X / f, p.Y / f);
        }

        public override string ToString()
        {
            return "{" + X.ToString() + ", " + Y.ToString() + "}";
        }
    }
}
