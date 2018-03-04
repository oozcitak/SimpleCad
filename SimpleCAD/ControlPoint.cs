using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    internal class ControlPoint
    {
        public Drawable Parent { get; private set; }
        public Point2D P { get; private set; }
        public int Index { get; private set; }
        public bool IsHot { get; set; }

        public float X { get { return P.X; } }
        public float Y { get { return P.Y; } }

        public ControlPoint(Drawable parent, Point2D pt, int index)
        {
            Parent = parent;
            P = pt;
            Index = index;
            IsHot = false;
        }

        public static ControlPoint[] FromDrawable(Drawable item)
        {
            Point2D[] points = item.GetControlPoints();
            ControlPoint[] cPoints = new ControlPoint[points.Length];
            for (int i = 0; i < points.Length; i++)
                cPoints[i] = new ControlPoint(item, points[i], i);
            return cPoints;
        }
    }
}
