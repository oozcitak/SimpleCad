using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;

namespace SimpleCAD
{
    [Flags]
    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8
    }

    public enum TextHorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    public enum TextVerticalAlignment
    {
        Top,
        Middle,
        Bottom
    }

    public abstract class Renderer : IDisposable
    {
        public abstract string Name { get; }

        public CADView View { get; private set; }
        public bool ScaleLineWeights { get; set; }
        internal Style StyleOverride { get; set; }

        protected Renderer(CADView view)
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

        public abstract void Init(System.Windows.Forms.Control control);
        public abstract void InitFrame(System.Drawing.Graphics graphics);
        public abstract void EndFrame();
        public abstract void Resize(int width, int height);
        public abstract void Dispose();

        public abstract void Clear(Color color);
        public abstract void DrawLine(Style style, Point2D p1, Point2D p2);
        public abstract void DrawRectangle(Style style, Point2D p1, Point2D p2);
        public abstract void DrawCircle(Style style, Point2D center, float radius);
        public abstract void DrawArc(Style style, Point2D center, float radius, float startAngle, float endAngle);
        public abstract void DrawEllipse(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float rotation);
        public abstract void DrawEllipticArc(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float startAngle, float endAngle, float rotation);
        public abstract void DrawPolyline(Style style, Point2DCollection points, bool closed);
        public abstract void DrawPolygon(Style style, Point2DCollection points);
        public abstract Vector2D MeasureString(string text, string fontFamily, FontStyle fontStyle, float textHeight);
        public abstract void DrawString(Style style, Point2D pt, string text,
            string fontFamily, float textHeight, FontStyle fontStyle = FontStyle.Regular,
            float rotation = 0,
            TextHorizontalAlignment hAlign = TextHorizontalAlignment.Left,
            TextVerticalAlignment vAlign = TextVerticalAlignment.Bottom);
        public abstract void Draw(Drawable item);

        public override string ToString()
        {
            return Name;
        }
    }
}
