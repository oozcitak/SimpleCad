using System;
using System.Drawing;
using System.Collections.Generic;

namespace SimpleCAD
{
    public class Extents
    {
        public bool IsEmpty { get; private set; }
        public float XMin { get; private set; }
        public float YMin { get; private set; }
        public float XMax { get; private set; }
        public float YMax { get; private set; }
        public float Width { get { return Math.Abs(XMax - XMin); } }
        public float Height { get { return Math.Abs(YMax - YMin); } }

        public Extents()
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

        public void Add(Extents extents)
        {
            if (!extents.IsEmpty)
            {
                Add(extents.XMin, extents.YMin);
                Add(extents.XMax, extents.YMax);
            }
        }

        public static implicit operator RectangleF(Extents extents)
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

        public static Extents Empty { get { return new Extents(); } }
    }
}
