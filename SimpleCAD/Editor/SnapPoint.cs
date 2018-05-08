using SimpleCAD.Geometry;
using System;

namespace SimpleCAD
{
    [Flags]
    public enum SnapPointType
    {
        None = 0,
        End = 1,
        Middle = 2,
        Center = 4,
        Quadrant = 8,
        Point = 16,
        All = End | Middle | Center | Quadrant | Point
    }

    public class SnapPoint
    {
        public string Name { get; private set; }
        public SnapPointType Type { get; private set; }
        public Point2D Location { get; private set; }

        public SnapPoint(string name, Point2D location) : this(name, SnapPointType.End, location)
        {
            ;
        }

        public SnapPoint(string name, SnapPointType type, Point2D location)
        {
            Name = name;
            Type = type;
            Location = location;
        }
    }
}
