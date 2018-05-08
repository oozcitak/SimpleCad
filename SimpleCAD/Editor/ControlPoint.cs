using SimpleCAD.Geometry;

namespace SimpleCAD
{
    public enum ControlPointType
    {
        Point,
        Angle,
        Distance
    }

    public class ControlPoint
    {
        public string Name { get; private set; }
        public ControlPointType Type { get; private set; }
        public Point2D BasePoint { get; private set; }
        public Point2D Location { get; private set; }
        internal int Index;

        public ControlPoint(string name, Point2D basePoint)
            : this(name, ControlPointType.Point, basePoint, basePoint)
        {
            ;
        }

        public ControlPoint(string name, ControlPointType type, Point2D basePoint, Point2D location)
        {
            Name = name;
            Type = type;
            BasePoint = basePoint;
            Location = location;
        }
    }
}
