using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public string PropertyName { get; private set; }
        public int PropertyIndex { get; private set; }
        public ControlPointType Type { get; private set; }
        public Point2D BasePoint { get; private set; }
        public Point2D Location { get; private set; }
        private bool pointSet;

        public ControlPoint(string propertyName)
            : this(propertyName, -1, ControlPointType.Point, Point2D.Zero, Point2D.Zero)
        {
            pointSet = false;
        }

        public ControlPoint(string propertyName, int propertyIndex)
            : this(propertyName, propertyIndex, ControlPointType.Point, Point2D.Zero, Point2D.Zero)
        {
            pointSet = false;
        }

        public ControlPoint(string propertyName, ControlPointType type, Point2D basePoint, Point2D location)
            : this(propertyName, -1, type, basePoint, location)
        {
            ;
        }

        public ControlPoint(string propertyName, int propertyIndex, ControlPointType type, Point2D basePoint, Point2D location)
        {
            PropertyName = propertyName;
            PropertyIndex = propertyIndex;
            Type = type;
            BasePoint = basePoint;
            Location = location;
            pointSet = true;
        }

        internal static ControlPoint[] FromDrawable(Drawable item)
        {
            ControlPoint[] points = item.GetControlPoints();
            for (int i = 0; i < points.Length; i++)
            {
                ControlPoint cp = points[i];
                if (!cp.pointSet)
                {
                    // Read the point location with reflection
                    PropertyInfo prop = item.GetType().GetProperty(points[i].PropertyName);
                    Point2D pt;
                    if (cp.PropertyIndex == -1)
                    {
                        pt = (Point2D)prop.GetValue(item);
                    }
                    else
                    {
                        IList<Point2D> itemPoints = (IList<Point2D>)prop.GetValue(item);
                        pt = itemPoints[cp.PropertyIndex];
                    }
                    cp.BasePoint = pt;
                    cp.Location = pt;
                    cp.pointSet = true;
                }
            }
            return points;
        }
    }
}
