using SimpleCAD.Geometry;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace SimpleCAD.Graphics
{
    #region Enums
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
    #endregion

    public class Renderer : IDisposable
    {
        private System.Drawing.Graphics gdi;

        public CADView View { get; private set; }
        public bool ScaleLineWeights { get; set; }
        internal Style StyleOverride { get; set; }

        public Renderer(CADView view)
        {
            View = view;
            StyleOverride = null;
        }

        #region Life-time functions
        public void Init(Control control)
        {
            try
            {
                // Enable double buffering
                Type type = control.GetType();
                MethodInfo method = type.GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(control, new object[] { ControlStyles.DoubleBuffer, true });
            }
            catch (System.Security.SecurityException)
            {
                ;
            }
        }

        public void InitFrame(System.Drawing.Graphics graphics)
        {
            gdi = graphics;

            gdi.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            gdi.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            // Calculate model to view transformation
            gdi.ResetTransform();
            gdi.TranslateTransform(-View.Camera.Position.X, -View.Camera.Position.Y);
            gdi.ScaleTransform(1.0f / View.Camera.Zoom, -1.0f / View.Camera.Zoom, System.Drawing.Drawing2D.MatrixOrder.Append);
            gdi.TranslateTransform(View.Width / 2, View.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Append);
        }

        public void EndFrame()
        {
            ;
        }

        public void Resize(int width, int height)
        {
            ;
        }
        #endregion

        #region Drawing functions
        public void Clear(Color color)
        {
            gdi.Clear(System.Drawing.Color.FromArgb((int)color.Argb));
        }

        public void DrawLine(Style style, Point2D p1, Point2D p2)
        {
            using (var pen = CreatePen(style))
            {
                gdi.DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
            }
        }

        public void DrawRectangle(Style style, Point2D p1, Point2D p2)
        {
            if (style.Fill)
            {
                using (var brush = CreateBrush(style))
                {
                    gdi.FillRectangle(brush, Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y),
                        Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
                }
            }
            else
            {
                using (var pen = CreatePen(style))
                {
                    gdi.DrawRectangle(pen, Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y),
                        Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
                }
            }
        }

        public void DrawCircle(Style style, Point2D center, float radius)
        {
            if (style.Fill)
            {
                using (var brush = CreateBrush(style))
                {
                    gdi.FillEllipse(brush, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
                }
            }
            else
            {
                using (var pen = CreatePen(style))
                {
                    gdi.DrawEllipse(pen, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
                }
            }
        }

        public void DrawArc(Style style, Point2D center, float radius, float startAngle, float endAngle)
        {
            using (var pen = CreatePen(style))
            {
                float sweepAngle = endAngle - startAngle;
                while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
                while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
                gdi.DrawArc(pen, center.X - radius, center.Y - radius, 2 * radius, 2 * radius, startAngle * 180 / MathF.PI, sweepAngle * 180 / MathF.PI);
            }
        }

        public void DrawEllipse(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float rotation)
        {
            using (var pen = CreatePen(style))
            {
                var matrix = gdi.Transform;
                gdi.TranslateTransform(center.X, center.Y);
                gdi.RotateTransform(rotation * 180 / MathF.PI);
                gdi.DrawEllipse(pen, -semiMajorAxis, -semiMinorAxis, 2 * semiMajorAxis, 2 * semiMinorAxis);
                gdi.Transform = matrix;
            }
        }

        public void DrawEllipticArc(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float startAngle, float endAngle, float rotation)
        {
            using (var pen = CreatePen(style))
            {
                var matrix = gdi.Transform;
                gdi.TranslateTransform(center.X, center.Y);
                gdi.RotateTransform(rotation * 180 / MathF.PI);
                float sweepAngle = endAngle - startAngle;
                while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
                while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
                gdi.DrawArc(pen, -semiMajorAxis, -semiMinorAxis, 2 * semiMajorAxis, 2 * semiMinorAxis, startAngle * 180 / MathF.PI, sweepAngle * 180 / MathF.PI);
                gdi.Transform = matrix;
            }
        }

        public void DrawPolyline(Style style, Point2DCollection points, bool closed)
        {
            if (points.Count > 1)
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

        public void DrawPolygon(Style style, Point2DCollection points)
        {
            if (points.Count > 1)
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

        public Vector2D MeasureString(string text, string fontFamily, FontStyle fontStyle, float textHeight)
        {
            using (var font = new System.Drawing.Font(fontFamily, textHeight, System.Drawing.GraphicsUnit.Pixel))
            {
                var sz = gdi.MeasureString(text, font);
                return new Vector2D(sz.Width, sz.Height);
            }
        }

        public void DrawString(Style style, Point2D pt, string text,
            string fontFamily, float textHeight, FontStyle fontStyle,
            float rotation, TextHorizontalAlignment hAlign, TextVerticalAlignment vAlign)
        {
            //float height = Math.Abs(View.WorldToScreen(new Vector2D(0, textHeight)).Y);
            using (var font = new System.Drawing.Font(fontFamily, textHeight, (System.Drawing.FontStyle)fontStyle, System.Drawing.GraphicsUnit.Pixel))
            using (var brush = CreateBrush(style))
            {
                // Keep old transform
                var oldTrans = gdi.Transform;

                // Calculate alignment offset
                float dx = 0;
                float dy = 0;
                var sz = MeasureString(text, fontFamily, fontStyle,textHeight);

                if (hAlign == TextHorizontalAlignment.Right)
                    dx = -sz.X;
                else if (hAlign == TextHorizontalAlignment.Center)
                    dx = -sz.X / 2;

                if (vAlign == TextVerticalAlignment.Bottom)
                    dy = -sz.Y;
                else if (vAlign == TextVerticalAlignment.Middle)
                    dy = -sz.Y / 2;

                gdi.TranslateTransform(pt.X, pt.Y, System.Drawing.Drawing2D.MatrixOrder.Prepend);
                gdi.RotateTransform(rotation * 180 / MathF.PI, System.Drawing.Drawing2D.MatrixOrder.Prepend);
                gdi.ScaleTransform(1, -1, System.Drawing.Drawing2D.MatrixOrder.Prepend);
                gdi.TranslateTransform(dx, dy, System.Drawing.Drawing2D.MatrixOrder.Prepend);

                gdi.DrawString(text, font, brush, 0, 0);

                // Restore old transformation
                gdi.Transform = oldTrans;
            }
        }

        public void Draw(Drawable item)
        {
            item.Draw(this);
        }

        private System.Drawing.Pen CreatePen(Style style)
        {
            Style appliedStyle = (StyleOverride == null ? style : StyleOverride);

            var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb((int)(appliedStyle.Color.IsByLayer ? Color.White : appliedStyle.Color).Argb));
            pen.Width = GetScaledLineWeight(appliedStyle.LineWeight == Style.ByLayer ? 1 : appliedStyle.LineWeight);
            pen.DashStyle = appliedStyle.DashStyle == DashStyle.ByLayer ? System.Drawing.Drawing2D.DashStyle.Solid : (System.Drawing.Drawing2D.DashStyle)appliedStyle.DashStyle;
            return pen;
        }

        private System.Drawing.Brush CreateBrush(Style style)
        {
            Style appliedStyle = (StyleOverride == null ? style : StyleOverride);

            return new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb((int)(appliedStyle.Color.IsByLayer ? Color.White : appliedStyle.Color).Argb));
        }

        public float GetScaledLineWeight(float lineWeight)
        {
            if (ScaleLineWeights)
                return lineWeight;
            else
                return View.ScreenToWorld(new Vector2D(lineWeight, 0)).X;
        }
        #endregion

        #region Disposable pattern
        public void Dispose()
        {

        }
        #endregion
    }
}
