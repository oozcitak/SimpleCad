using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace SimpleCAD.Geometry
{
    public class Extents2D
    {
        public bool IsEmpty { get; private set; }
        public float Xmin { get; private set; }
        public float Ymin { get; private set; }
        public float Xmax { get; private set; }
        public float Ymax { get; private set; }
        public float Width { get => Math.Abs(Xmax - Xmin); }
        public float Height { get => Math.Abs(Ymax - Ymin); }
        public Point2D Center { get => IsEmpty ? Point2D.Zero : new Point2D((Xmin + Xmax) / 2, (Ymin + Ymax) / 2); }
        public Point2D Ptmin { get => IsEmpty ? Point2D.Zero : new Point2D(Xmin, Ymin); }
        public Point2D Ptmax { get => IsEmpty ? Point2D.Zero : new Point2D(Xmax, Ymax); }

        public static Extents2D Empty { get { return new Extents2D(); } }
        public static Extents2D Infinity { get { return new Extents2D(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Extents2D()
        {
            IsEmpty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Extents2D(float xmin, float ymin, float xmax, float ymax)
        {
            IsEmpty = true;
            Add(xmin, ymin);
            Add(xmax, ymax);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            IsEmpty = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(float x, float y)
        {
            if (IsEmpty || x < Xmin) Xmin = x;
            if (IsEmpty || y < Ymin) Ymin = y;
            if (IsEmpty || x > Xmax) Xmax = x;
            if (IsEmpty || y > Ymax) Ymax = y;

            IsEmpty = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Point2D pt)
        {
            Add(pt.X, pt.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(IEnumerable<Point2D> points)
        {
            foreach (Point2D pt in points)
            {
                Add(pt.X, pt.Y);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(RectangleF rectangle)
        {
            Add(rectangle.X, rectangle.Y);
            Add(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Extents2D extents)
        {
            if (!extents.IsEmpty)
            {
                Add(extents.Xmin, extents.Ymin);
                Add(extents.Xmax, extents.Ymax);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator RectangleF(Extents2D extents)
        {
            if (extents.IsEmpty)
                return RectangleF.Empty;
            else
                return new RectangleF(extents.Xmin, extents.Ymin, extents.Xmax - extents.Xmin, extents.Ymax - extents.Ymin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Point2D pt)
        {
            return Contains(pt.X, pt.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(float x, float y)
        {
            if (IsEmpty)
                return false;
            else
                return (x >= Xmin && x <= Xmax && y >= Ymin && y <= Ymax);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Extents2D other)
        {
            return (Xmin <= other.Xmin && Xmax >= other.Xmax && Ymin <= other.Ymin && Ymax >= other.Ymax);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsWith(Extents2D other)
        {
            return (Xmax >= other.Xmin && Xmin <= other.Xmax && Ymax >= other.Ymin && Ymin <= other.Ymax);
        }
    }
}
