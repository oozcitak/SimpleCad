using System;
using System.ComponentModel;

namespace SimpleCAD
{
    [TypeConverter(typeof(Vector2DConverter))]
    public struct Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Length { get { return MathF.Sqrt(X * X + Y * Y); } }
        public float Angle { get { return AngleTo(Vector2D.XAxis); } }

        public static Vector2D XAxis { get { return new Vector2D(1, 0); } }
        public static Vector2D YAxis { get { return new Vector2D(0, 1); } }

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void TransformBy(TransformationMatrix2D transformation)
        {
            float x = transformation.M11 * X + transformation.M12 * Y;
            float y = transformation.M21 * X + transformation.M22 * Y;
            X = x;
            Y = y;
        }

        public Vector2D GetNormalized()
        {
            float len = Length;
            return new Vector2D(X / len, Y / len);
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

        public float CrossProduct(Vector2D v)
        {
            return X * v.Y - Y * v.X;
        }

        public float AngleTo(Vector2D v)
        {
            return ClampAngle(SignedAngleTo(v));
        }

        public bool IsBetween(Vector2D a, Vector2D b)
        {
            float ang = ClampAngle(b.SignedAngleTo(a), true, false);
            float ang1 = ClampAngle(this.SignedAngleTo(a), true, false);
            float ang2 = ClampAngle(b.SignedAngleTo(this), true, false);

            return Math.Abs(ang2 + ang1 - ang) < 0.0001f;
        }

        private float SignedAngleTo(Vector2D v)
        {
            float dot = this.DotProduct(v);
            float det = X * v.Y - v.X * Y;
            float ang = -MathF.Atan2(det, dot);
            return ang;
        }

        private static float ClampAngle(float ang)
        {
            return ClampAngle(ang, true, true);
        }

        private static float ClampAngle(float ang, bool low, bool high)
        {
            if (low) { while (ang < 0) ang += 2 * MathF.PI; }
            if (high) { while (ang > 2 * MathF.PI) ang -= 2 * MathF.PI; }
            return ang;
        }

        public static Vector2D FromAngle(float angle)
        {
            return new Vector2D(MathF.Cos(angle), MathF.Sin(angle));
        }

        public Vector2D GetPerpendicularVector()
        {
            return new Vector2D(-Y, X);
        }

        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X - b.X, a.Y - b.Y);
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
