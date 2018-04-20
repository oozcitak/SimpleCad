using SimpleCAD.Geometry;
using System.Drawing;

namespace SimpleCAD.Drawables
{
    public class Polygon : Polyline
    {
        public override bool Closed => true;

        public Polygon() : base() { }
        public Polygon(Point2DCollection pts) : base(pts) { }
        public Polygon(params Point2D[] pts) : base(pts) { }
        public Polygon(PointF[] pts) : base(pts) { }
    }
}
