using SimpleCAD.Geometry;
using SimpleCAD.Graphics;

namespace SimpleCAD
{
    public abstract class Renderer
    {
        public CADView View { get; private set; }
        public bool ScaleLineWeights { get; set; }
        internal Style StyleOverride { get; set; }

        protected Renderer(CADView view, dynamic graphicsData)
        {
            View = view;
            StyleOverride = null;
        }

        public float GetScaledLineWeight(float lineWeight)
        {
            if (ScaleLineWeights)
                return lineWeight;
            else
                return View.ScreenToWorld(new Vector2D(lineWeight, 0)).X;
        }

        public abstract void DrawLine(Style style, Point2D p1, Point2D p2);
        public abstract void DrawRectangle(Style style, Point2D p1, Point2D p2);
        public abstract void DrawCircle(Style style, Point2D center, float radius);
        public abstract void DrawArc(Style style, Point2D center, float radius, float startAngle, float endAngle);
        public abstract void DrawEllipse(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis);
        public abstract void DrawEllipticArc(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float startAngle, float endAngle);
        public abstract void DrawPolyline(Style style, Point2DCollection points, bool closed);
        public abstract void DrawPolygon(Style style, Point2DCollection points);
        public abstract Vector2D MeasureString(string text, string fontFamily, float textHeight);
        public abstract void DrawString(Style style, Point2D pt, string text, string fontFamily, float textHeight);
        public abstract void Draw(Drawable item);
    }
}
