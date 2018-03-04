using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public class ControlPoint
    {
        public enum ControlPointType
        {
            Point,
            Angle,
            Distance
        }

        internal Drawable Owner { get; set; }
        public string PropertyName { get; private set; }
        public int PropertyIndex { get; private set; }
        public ControlPointType Type { get; private set; }
        public Point2D BasePoint { get; private set; }
        public Point2D Location { get; private set; }

        public ControlPoint(string propertyName, ControlPointType type, Point2D basePoint, Point2D location)
        {
            Owner = null;
            PropertyName = propertyName;
            PropertyIndex = -1;
            Type = type;
            BasePoint = basePoint;
            Location = location;
        }

        public ControlPoint(string propertyName, int propertyIndex, ControlPointType type, Point2D basePoint, Point2D location)
        {
            Owner = null;
            PropertyName = propertyName;
            PropertyIndex = propertyIndex;
            Type = type;
            BasePoint = basePoint;
            Location = location;
        }

        internal static ControlPoint[] FromDrawable(Drawable item, float size)
        {
            ControlPoint[] points = item.GetControlPoints(size);
            for (int i = 0; i < points.Length; i++)
                points[i].Owner = item;
            return points;
        }
    }
}
