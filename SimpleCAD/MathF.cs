using System;

namespace SimpleCAD
{
    public static class MathF
    {
        public const float Epsilon = 1e-7f;
        public const float PI = 3.14159265358979323846264338327950288419716939937510582f;

        public static bool IsZero(float value)
        {
            return value > -Epsilon && value < Epsilon;
        }

        public static bool IsEqual(float a, float b)
        {
            return IsZero(a - b);
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
    }
}
