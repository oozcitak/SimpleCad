using SimpleCAD.Geometry;

namespace SimpleCAD.Graphics
{
    public abstract class GDIRenderer : Renderer
    {
        private System.Drawing.Graphics gdi;

        protected GDIRenderer(CADView view, System.Drawing.Graphics graphicsData) : base(view, graphicsData)
        {
            gdi = graphicsData;
        }

        public override void DrawLine(Style style, Point2D p1, Point2D p2)
        {
            using (var pen = CreatePen(style))
            {
                gdi.DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
            }
        }

        public override void DrawRectangle(Style style, Point2D p1, Point2D p2)
        {
            if (style.Fill)
            {
                using (var pen = CreatePen(style))
                {
                    Vector2D size = p2 - p1;
                    gdi.DrawRectangle(pen, p1.X, p1.Y, size.X, size.Y);
                }
            }
            else
            {
                using (var brush = CreateBrush(style))
                {
                    Vector2D size = p2 - p1;
                    gdi.FillRectangle(brush, p1.X, p1.Y, size.X, size.Y);
                }
            }
        }

        public override void DrawCircle(Style style, Point2D center, float radius)
        {
            using (var pen = CreatePen(style))
            {
                gdi.DrawEllipse(pen, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
            }
        }

        public override void DrawArc(Style style, Point2D center, float radius, float startAngle, float endAngle)
        {
            using (var pen = CreatePen(style))
            {
                float sweepAngle = endAngle - startAngle;
                while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
                while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
                gdi.DrawArc(pen, center.X - radius, center.Y - radius, 2 * radius, 2 * radius, startAngle * 180 / MathF.PI, sweepAngle * 180 / MathF.PI);
            }
        }

        public override void DrawEllipse(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis)
        {
            using (var pen = CreatePen(style))
            {
                gdi.DrawEllipse(pen, center.X - semiMajorAxis, center.Y - semiMinorAxis, 2 * semiMajorAxis, 2 * semiMinorAxis);
            }
        }

        public override void DrawEllipticArc(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float startAngle, float endAngle)
        {
            using (var pen = CreatePen(style))
            {
                float sweepAngle = endAngle - startAngle;
                while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
                while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
                gdi.DrawArc(pen, center.X - semiMajorAxis, center.Y - semiMinorAxis, 2 * semiMajorAxis, 2 * semiMinorAxis, startAngle * 180 / MathF.PI, sweepAngle * 180 / MathF.PI);
            }
        }

        public override void DrawPolyline(Style style, Point2DCollection points, bool closed)
        {
            if (points.Count > 0)
            {
                var pts = points.ToPointF();
                using (var pen = CreatePen(style))
                {
                    if (closed)
                        gdi.DrawPolygon(pen, pts);
                    else
                        gdi.DrawLines(pen, pts);
                }
            }
        }

        public override void DrawPolygon(Style style, Point2DCollection points)
        {
            if (points.Count > 0)
            {
                var pts = points.ToPointF();
                if (style.Fill)
                {
                    using (var brush = CreateBrush(style))
                    {
                        gdi.FillPolygon(brush, pts);
                    }
                }
                else
                {
                    using (var pen = CreatePen(style))
                    {
                        gdi.DrawPolygon(pen, pts);
                    }
                }
            }
        }

        public override Vector2D MeasureString(string text, string fontFamily, float textHeight)
        {
            float height = View.WorldToScreen(new Vector2D(textHeight, 0)).X;
            using (var font = new System.Drawing.Font(fontFamily, height, System.Drawing.GraphicsUnit.Pixel))
            {
                var sz = gdi.MeasureString(text, font);
                var vec = new Vector2D(sz.Width, sz.Height);
                return View.ScreenToWorld(vec);
            }
        }

        public override void DrawString(Style style, Point2D pt, string text, string fontFamily, float textHeight)
        {
            float height = View.WorldToScreen(new Vector2D(textHeight, 0)).X;
            using (var font = new System.Drawing.Font(fontFamily, height, System.Drawing.GraphicsUnit.Pixel))
            using (var brush = CreateBrush(style))
            {
                gdi.DrawString(text, font, brush, 0, 0);
            }
        }

        public override void Draw(Drawable item)
        {
            item.Draw(this);
        }

        private System.Drawing.Pen CreatePen(Style style)
        {
            if (StyleOverride != null)
            {
                var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb((int)StyleOverride.Color.Argb), GetScaledLineWeight(StyleOverride.LineWeight));
                pen.DashStyle = (System.Drawing.Drawing2D.DashStyle)StyleOverride.DashStyle;
                return pen;
            }
            else
            {
                var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb((int)style.Color.Argb), GetScaledLineWeight(style.LineWeight));
                pen.DashStyle = (System.Drawing.Drawing2D.DashStyle)style.DashStyle;
                return pen;
            }
        }

        private System.Drawing.Brush CreateBrush(Style style)
        {
            if (StyleOverride != null)
            {
                return new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb((int)StyleOverride.Color.Argb));
            }
            else
            {
                return new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb((int)style.Color.Argb));
            }
        }
    }
}
