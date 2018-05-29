using SimpleCAD.Geometry;
using System;
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
        Control control;
        private System.Drawing.BufferedGraphics gdiBuffer;
        private System.Drawing.Graphics gdi;

        public CADView View { get; private set; }
        public bool ScaleLineWeights { get; set; }
        public Matrix2D Transform { get => new Matrix2D(gdi.Transform); set { gdi.Transform = (System.Drawing.Drawing2D.Matrix)value; } }
        internal Style StyleOverride { get; set; }

        public Renderer(CADView view)
        {
            View = view;
            StyleOverride = null;
        }

        #region Life-time functions
        public void Init(Control control)
        {
            this.control = control;
        }

        public void InitFrame(System.Drawing.Graphics graphics)
        {
            if (gdiBuffer == null)
                CreateGraphicsBuffer(graphics);

            gdi = gdiBuffer.Graphics;

            gdi.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            gdi.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            // Calculate model to view transformation
            gdi.ResetTransform();
            gdi.TranslateTransform(-View.Camera.Position.X, -View.Camera.Position.Y);
            gdi.ScaleTransform(1.0f / View.Camera.Zoom, -1.0f / View.Camera.Zoom, System.Drawing.Drawing2D.MatrixOrder.Append);
            gdi.TranslateTransform(View.Width / 2, View.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Append);
        }

        public void EndFrame(System.Drawing.Graphics graphics)
        {
            if (gdiBuffer != null)
                gdiBuffer.Render(graphics);
        }

        public void Resize(int width, int height)
        {
            ResetGraphicsBuffer();
        }

        private void ResetGraphicsBuffer()
        {
            if (gdiBuffer != null)
                gdiBuffer.Dispose();
            gdiBuffer = null;
        }

        private void CreateGraphicsBuffer(System.Drawing.Graphics graphics)
        {
            var bufferContext = System.Drawing.BufferedGraphicsManager.Current;

            int width = Math.Max(control.Width, 1);
            int height = Math.Max(control.Height, 1);

            bufferContext.MaximumBuffer = new System.Drawing.Size(width, height);

            ResetGraphicsBuffer();

            gdiBuffer = bufferContext.Allocate(graphics, new System.Drawing.Rectangle(0, 0, width, height));
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
            using (var pen = CreatePen(style))
            {
                gdi.DrawRectangle(pen, Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y),
                    Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
            }
        }

        public void FillRectangle(Style style, Point2D p1, Point2D p2)
        {
            using (var brush = CreateBrush(style))
            {
                gdi.FillRectangle(brush, Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y),
                    Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
            }
        }

        public void DrawCircle(Style style, Point2D center, float radius)
        {
            using (var pen = CreatePen(style))
            {
                gdi.DrawEllipse(pen, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
            }
        }

        public void FillCircle(Style style, Point2D center, float radius)
        {
            using (var brush = CreateBrush(style))
            {
                gdi.FillEllipse(brush, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
            }
        }

        public void DrawArc(Style style, Point2D center, float radius, float startAngle, float endAngle)
        {
            using (var pen = CreatePen(style))
            {
                float sweepAngle = endAngle - startAngle;
                while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
                while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
                if (radius > 0)
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

        public void FillEllipse(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float rotation)
        {
            using (var brush = CreateBrush(style))
            {
                var matrix = gdi.Transform;
                gdi.TranslateTransform(center.X, center.Y);
                gdi.RotateTransform(rotation * 180 / MathF.PI);
                gdi.FillEllipse(brush, -semiMajorAxis, -semiMinorAxis, 2 * semiMajorAxis, 2 * semiMinorAxis);
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
                using (var pen = CreatePen(style))
                {
                    gdi.DrawPolygon(pen, pts);
                }
            }
        }

        public void FillPolygon(Style style, Point2DCollection points)
        {
            if (points.Count > 1)
            {
                var pts = points.ToPointF();
                using (var brush = CreateBrush(style))
                {
                    gdi.FillPolygon(brush, pts);
                }
            }
        }

        public Vector2D MeasureString(string text, TextStyle textStyle, float textHeight)
        {
            using (var font = CreateFont(textStyle, textHeight))
            using (var format = new System.Drawing.StringFormat(System.Drawing.StringFormatFlags.FitBlackBox))
            {
                return MeasureString(text, font, format);
            }
        }

        public Vector2D MeasureString(string text, System.Drawing.Font font, System.Drawing.StringFormat format)
        {
            var sz = gdi.MeasureString(text, font);
            return new Vector2D(sz.Width, sz.Height);
        }

        public void DrawString(Style style, Point2D pt, string text,
            TextStyle textStyle, float textHeight,
            float rotation, TextHorizontalAlignment hAlign, TextVerticalAlignment vAlign)
        {
            using (var font = CreateFont(textStyle, textHeight))
            using (var format = new System.Drawing.StringFormat(System.Drawing.StringFormatFlags.FitBlackBox))
            using (var brush = CreateBrush(style))
            {
                // Keep old transform
                var oldTrans = gdi.Transform;

                // Calculate alignment offset
                float dx = 0;
                float dy = 0;
                var sz = MeasureString(text, font, format);

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
            if (View.GetViewport().IntersectsWith(item.GetExtents()))
            {
                item.Draw(this);

                if (item.InModel)
                    View.VisibleItems.Add(item);
            }
        }

        private System.Drawing.Pen CreatePen(Style style)
        {
            Style appliedStyle = (StyleOverride ?? style);

            var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb((int)(appliedStyle.Color.IsByLayer ? Color.White : appliedStyle.Color).Argb));
            pen.Width = GetScaledLineWeight(appliedStyle.LineWeight == Style.ByLayer ? 1 : appliedStyle.LineWeight);
            pen.DashStyle = appliedStyle.DashStyle == DashStyle.ByLayer ? System.Drawing.Drawing2D.DashStyle.Solid : (System.Drawing.Drawing2D.DashStyle)appliedStyle.DashStyle;
            return pen;
        }

        private System.Drawing.Brush CreateBrush(Style style)
        {
            Style appliedStyle = (StyleOverride ?? style);

            return new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb((int)(appliedStyle.Color.IsByLayer ? Color.White : appliedStyle.Color).Argb));
        }

        private System.Drawing.Font CreateFont(TextStyle style, float height)
        {
            return new System.Drawing.Font(style.FontFamily, height, (System.Drawing.FontStyle)style.FontStyle, System.Drawing.GraphicsUnit.Pixel);
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
