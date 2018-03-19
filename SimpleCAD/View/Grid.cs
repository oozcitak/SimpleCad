using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCAD.Geometry;
using SimpleCAD.Graphics;

namespace SimpleCAD.View
{
    internal class Grid : Drawable
    {
        public override void Draw(Renderer renderer)
        {
            var view = renderer.View;
            var doc = view.Document;

            float spacing = 1;
            // Dynamic grid spacing
            while (view.WorldToScreen(new Vector2D(spacing, 0)).X > 12)
                spacing /= 10;

            while (view.WorldToScreen(new Vector2D(spacing, 0)).X < 4)
                spacing *= 10;

            Extents2D bounds = view.GetViewPort();
            Style majorStyle = new Style(doc.Settings.Get<Color>("MajorGridColor"));
            Style minorStyle = new Style(doc.Settings.Get<Color>("MinorGridColor"));

            int k = 0;
            for (float i = 0; i > bounds.Xmin; i -= spacing)
            {
                Style style = (k == 0 ? majorStyle : minorStyle);
                k = (k + 1) % 10;
                renderer.DrawLine(style, new Point2D(i, bounds.Ymax), new Point2D(i, bounds.Ymin));
            }
            k = 0;
            for (float i = 0; i < bounds.Xmax; i += spacing)
            {
                Style style = (k == 0 ? majorStyle : minorStyle);
                k = (k + 1) % 10;
                renderer.DrawLine(style, new Point2D(i, bounds.Ymax), new Point2D(i, bounds.Ymin));
            }
            k = 0;
            for (float i = 0; i < bounds.Ymax; i += spacing)
            {
                Style style = (k == 0 ? majorStyle : minorStyle);
                k = (k + 1) % 10;
                renderer.DrawLine(style, new Point2D(bounds.Xmin, i), new Point2D(bounds.Xmax, i));
            }
            k = 0;
            for (float i = 0; i > bounds.Ymin; i -= spacing)
            {
                Style style = (k == 0 ? majorStyle : minorStyle);
                k = (k + 1) % 10;
                renderer.DrawLine(style, new Point2D(bounds.Xmin, i), new Point2D(bounds.Xmax, i));
            }
        }

        public override Extents2D GetExtents()
        {
            return Extents2D.Empty;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            ;
        }
    }
}
