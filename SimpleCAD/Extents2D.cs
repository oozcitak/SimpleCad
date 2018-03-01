using System;
using System.Drawing;
using System.Collections.Generic;

namespace SimpleCAD
{
    public class Extents2D
    {
        public bool IsEmpty { get; private set; }
        public float XMin { get; private set; }
        public float YMin { get; private set; }
        public float XMax { get; private set; }
        public float YMax { get; private set; }
        public float Width { get { return Math.Abs(XMax - XMin); } }
        public float Height { get { return Math.Abs(YMax - YMin); } }

        public Extents2D()
        {
            IsEmpty = true;
        }

        public void Reset()
        {
            IsEmpty = true;
        }

        public void Add(float x, float y)
        {
            if (IsEmpty || x < XMin) XMin = x;
            if (IsEmpty || y < YMin) YMin = y;
            if (IsEmpty || x > XMax) XMax = x;
            if (IsEmpty || y > YMax) YMax = y;

            IsEmpty = false;
        }

        public void Add(Point2D pt)
        {
            Add(pt.X, pt.Y);
        }

        public void Add(IEnumerable<Point2D> points)
        {
            foreach (Point2D pt in points)
            {
                Add(pt.X, pt.Y);
            }
        }

        public void Add(RectangleF rectangle)
        {
            Add(rectangle.X, rectangle.Y);
            Add(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
        }

        public void Add(Extents2D extents)
        {
            if (!extents.IsEmpty)
            {
                Add(extents.XMin, extents.YMin);
                Add(extents.XMax, extents.YMax);
            }
        }

        public static implicit operator RectangleF(Extents2D extents)
        {
            if (extents.IsEmpty)
                return RectangleF.Empty;
            else
                return new RectangleF(extents.XMin, extents.YMin, extents.XMax - extents.XMin, extents.YMax - extents.YMin);
        }

        public bool Contains(Point2D pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public bool Contains(float x, float y)
        {
            if (IsEmpty)
                return false;
            else
                return (x >= XMin && x <= XMax && y >= YMin && y <= YMax);
        }

        public static Extents2D Empty { get { return new Extents2D(); } }

        public bool Contains(Extents2D other)
        {
            return (XMin <= other.XMin && XMax >= other.XMax && YMin <= other.YMin && YMax >= other.YMax);
        }

        public bool IntersectsWith(Extents2D other)
        {
            return (XMax >= other.XMin && XMin <= other.XMax && YMax >= other.YMin && YMin <= other.YMax);
        }
    }
}
