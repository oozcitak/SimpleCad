using SimpleCAD.Geometry;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public abstract class Curve : Drawable
    {
        public const int MinCurveSegments = 4;
        public const int MaxCurveSegments = 200;

        [Browsable(false)]
        public abstract float StartParam { get; }
        [Browsable(false)]
        public abstract float EndParam { get; }

        public virtual float Length => GetDistAtParam(EndParam);

        public abstract float GetDistAtParam(float param);
        public abstract Point2D GetPointAtParam(float param);
        public abstract Vector2D GetNormalAtParam(float param);
    }
}
