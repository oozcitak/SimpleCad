using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCAD.Geometry;
using SimpleCAD.Graphics;

namespace SimpleCAD.View
{
    internal class Cursor : Drawable
    {
        public Point2D Location { get; set; }
        public string FontFamily { get; set; }
        public float TextHeight { get; set; }
        public string Message { get; set; }

        public Cursor()
        {
            // Assign the default system font by default
            FontFamily = System.Drawing.SystemFonts.MessageBoxFont.FontFamily.Name;
            // Default text height in pixels
            TextHeight = 12;
        }

        public override void Draw(Renderer renderer)
        {
            var view = renderer.View;
            var doc = view.Document;

            Extents2D ex = view.GetViewPort();
            Style cursorStyle = new Style(doc.Settings.Get<Color>("CursorColor"));

            // Draw cursor
            renderer.DrawLine(cursorStyle, new Point2D(ex.Xmin, Location.Y), new Point2D(ex.Xmax, Location.Y));
            renderer.DrawLine(cursorStyle, new Point2D(Location.X, ex.Ymin), new Point2D(Location.X, ex.Ymax));

            // Draw cursor prompt
            if (!string.IsNullOrEmpty(Message))
            {
                float height = Math.Abs(view.ScreenToWorld(new Vector2D(0, TextHeight)).Y);
                float margin = Math.Abs(view.ScreenToWorld(new Vector2D(4, 0)).X);
                float offset = Math.Abs(view.ScreenToWorld(new Vector2D(2, 0)).X);

                // position cursor prompt to lower-right of cursor by default
                float x = Location.X + margin + offset;
                float y = Location.Y - margin - offset;
                Vector2D sz = renderer.MeasureString(Message, FontFamily, FontStyle.Regular, height);
                Point2D lowerRight = new Point2D(ex.Xmax, ex.Ymin);
                // check if the prompt text fits into the window horizontally
                if (x + sz.X + offset > lowerRight.X)
                {
                    x = Location.X - margin - offset - sz.X;
                }
                // check if the prompt text fits into the window vertically
                if (y - sz.Y - offset < lowerRight.Y)
                {
                    y = Location.Y + margin + offset + sz.Y;
                }

                // Draw cursor prompt
                Style fore = new Style(doc.Settings.Get<Color>("CursorPromptForeColor"));
                Style back = new Style(doc.Settings.Get<Color>("CursorPromptBackColor"));
                back.Fill = true;
                renderer.DrawRectangle(back, new Point2D(x - offset, y + offset), new Point2D(x + offset + sz.X, y - offset - sz.Y));
                back.Fill = false;
                renderer.DrawRectangle(fore, new Point2D(x - offset, y + offset), new Point2D(x + offset + sz.X, y - offset - sz.Y));
                renderer.DrawString(fore, new Point2D(x, y), Message, FontFamily, height,
                    hAlign: TextHorizontalAlignment.Left, vAlign: TextVerticalAlignment.Top);
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
