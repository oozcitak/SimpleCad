using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleCAD.Geometry
{
    [TypeConverter(typeof(Segment2DConverter))]
    public struct Segment2D
    {
        public Point2D P1 { get; }
        public Point2D P2 { get; }

        public float X1 => P1.X;
        public float Y1 => P1.Y;
        public float X2 => P2.X;
        public float Y2 => P2.Y;

        public Vector2D Direction => (P2 - P1).Normal;
        public float Length => (P2 - P1).Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment2D(Point2D p1, Point2D p2)
        {
            P1 = p1;
            P2 = p2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment2D(float x1, float y1, float x2, float y2)
            : this(new Point2D(x1, y1), new Point2D(x2, y2))
        {
            ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment2D Transform(Matrix2D transformation)
        {
            Point2D p1 = P1.Transform(transformation);
            Point2D p2 = P2.Transform(transformation);
            return new Segment2D(p1, p2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Segment2D operator +(Segment2D s, Vector2D v)
        {
            return new Segment2D(s.P1 + v, s.P2 + v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Segment2D operator -(Segment2D s, Vector2D v)
        {
            return new Segment2D(s.P1 - v, s.P2 - v);
        }

        public bool Contains(Point2D pt, float tolerance, out float t)
        {
            Vector2D w = pt - P1;
            Vector2D vL = (P2 - P1);
            t = w.DotProduct(vL) / vL.DotProduct(vL);
            float dist = (w - t * vL).Length;
            return dist < tolerance;
        }

        public string ToString(IFormatProvider provider)
        {
            return ToString("{0:F}, {1:F} : {2:F}, {3:F}", provider);
        }

        public string ToString(string format = "{0:F}, {1:F} : {2:F}, {3:F}", IFormatProvider provider = null)
        {
            return (provider == null) ?
                string.Format(format, X1, Y1, X2, Y2) :
                string.Format(provider, format, X1, Y1, X2, Y2);
        }

        public static bool TryParse(string s, out Segment2D result)
        {
            Segment2DConverter conv = new Segment2DConverter();
            if (conv.IsValid(s))
            {
                result = (Segment2D)conv.ConvertFrom(s);
                return true;
            }
            else
            {
                result = new Segment2D();
                return false;
            }
        }
    }
}
