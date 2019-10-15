using SimpleCAD.Geometry;
using SimpleCAD.Graphics;

namespace SimpleCAD.View
{
    internal class SelectionWindow : Drawable
    {
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }

        public bool WindowSelection => P2.X > P1.X;

        public SelectionWindow(Point2D p1, Point2D p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public override void Draw(Renderer renderer)
        {
            var doc = renderer.View.Document;

            Style fillStyle = (WindowSelection ? new Style(doc.Settings.SelectionWindowColor) : new Style(doc.Settings.ReverseSelectionWindowColor));
            Style outlineStyle = (WindowSelection ? new Style(doc.Settings.SelectionWindowBorderColor) : new Style(doc.Settings.SelectionWindowBorderColor, 0, DashStyle.Dash));

            renderer.FillRectangle(fillStyle, P1, P2);
            renderer.DrawRectangle(outlineStyle, P1, P2);
        }

        public override Extents2D GetExtents()
        {
            Extents2D ex = new Extents2D();
            ex.Add(P1);
            ex.Add(P2);
            return ex;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            ;
        }
    }
}
