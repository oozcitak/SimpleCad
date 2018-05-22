using System;

namespace SimpleCAD
{
    internal static class MathF
    {
        public const float Epsilon = 1e-7f;
        public const float PI = 3.14159265358979323846264338327950288419716939937510582f;
        public const float TAU = 6.28318530717958647692528676655900576839433879875021164f;

        public static bool IsZero(float value)
        {
            return value > -Epsilon && value < Epsilon;
        }

        public static bool IsEqual(float a, float b)
        {
            return IsZero(a - b);
        }

        public static int Compare(float a, float b)
        {
            return IsEqual(a, b) ? 0 : (a < b ? -1 : 1);
        }

        public static bool IsBetween(float v, float a, float b, bool includeEnds)
        {
            if (a > b) Swap(ref a, ref b);

            if (includeEnds && (IsEqual(v, a) || IsEqual(v, b)))
                return true;
            if (!includeEnds && (IsEqual(v, a) || IsEqual(v, b)))
                return false;

            return v > a && v < b;
        }

        public static float NormalizeAngle(float a)
        {
            while (a < 0) a += TAU;
            while (a > TAU) a -= TAU;
            return a;
        }

        public static bool IsBetweenAngle(float v, float a, float b, bool includeEnds)
        {
            a = NormalizeAngle(a);
            b = NormalizeAngle(b);
            v = NormalizeAngle(v);

            if (includeEnds && (IsEqual(v, a) || IsEqual(v, b)))
                return true;
            if (!includeEnds && (IsEqual(v, a) || IsEqual(v, b)))
                return false;

            if (b > a)
                return v > a && v < b;
            else
                return v > a && v <= TAU && v >= 0 && v < b;
        }

        public static float Sin(float a)
        {
            return (float)Math.Sin(a);
        }

        public static float Cos(float a)
        {
            return (float)Math.Cos(a);
        }

        public static float Asin(float a)
        {
            return (float)Math.Asin(a);
        }

        public static float Acos(float a)
        {
            return (float)Math.Acos(a);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        public static float Sqrt(float a)
        {
            return (float)Math.Sqrt(a);
        }

        public static float Log(float a, float newBase)
        {
            return (float)Math.Log(a, newBase);
        }

        public static float Pow(float a, float power)
        {
            return (float)Math.Pow(a, power);
        }

        public static float Floor(float a)
        {
            return (float)Math.Floor(a);
        }

        public static float Ceiling(float a)
        {
            return (float)Math.Ceiling(a);
        }

        public static float Clamp(float val, float min, float max)
        {
            return val < min ? min : (val > max ? max : val);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Returns the determinant of the given 2x2 matrix.
        /// | a1 b1 |
        /// | a2 b2 |
        /// </summary>
        public static float Determinant(float a1, float b1, float a2, float b2)
        {
            return a1 * b2 - a2 * b1;
        }
    }
}
