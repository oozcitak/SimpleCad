using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SimpleCAD
{
    public class CADView
    {
        private PointF mCameraPosition;
        private float mZoomFactor;

        public float ZoomFactor
        {
            get
            {
                return mZoomFactor;
            }
            set
            {
                mZoomFactor = value;

                if (float.IsNaN(mZoomFactor) || float.IsNegativeInfinity(mZoomFactor) || float.IsPositiveInfinity(mZoomFactor) ||
                    mZoomFactor < float.Epsilon * 1000.0f || mZoomFactor > float.MaxValue / 1000.0f)
                {
                    mZoomFactor = 1;
                }
            }
        }

        public PointF CameraPosition
        {
            get
            {
                return mCameraPosition;
            }
            set
            {
                mCameraPosition = value;
                float x = mCameraPosition.X;
                float y = mCameraPosition.Y;
                if (float.IsNaN(x) || float.IsNegativeInfinity(x) || float.IsPositiveInfinity(x) ||
                    x < float.MinValue / 1000.0f || x > float.MaxValue / 1000.0f)
                {
                    x = 0;
                }
                if (float.IsNaN(y) || float.IsNegativeInfinity(y) || float.IsPositiveInfinity(y) ||
                    y < float.MinValue / 1000.0f || y > float.MaxValue / 1000.0f)
                {
                    y = 0;
                }
                mCameraPosition = new PointF(x, y);
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public CADDocument Document { get; private set; }

        public CADView(CADDocument document, int width, int height)
        {
            Document = document;

            Width = width;
            Height = height;

            mZoomFactor = 5.0f / 3.0f;
            mCameraPosition = new PointF(0, 0);
        }

        public void Render(Graphics graphics)
        {
            DrawParams param = new DrawParams(graphics, false, ZoomFactor);

            // Set an orthogonal projection matrix
            ScaleGraphics(graphics);

            // Render drawing objects
            Document.Model.Draw(param);

            // Render selected objects
            param.SelectionColor = Document.Editor.SelectionHighlight;
            param.SelectionMode = true;
            foreach (Drawable selected in Document.Editor.Selection)
            {
                selected.Draw(param);
            }
            param.SelectionMode = false;
        }

        /// <summary>
        /// Converts the given point from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="x">X coordinate in world coordinates.</param>
        /// <param name="y">Y coordinate in world coordinates.</param>
        /// <returns>A Point in screen coordinates.</returns>
        public Point WorldToScreen(float x, float y)
        {
            return new Point((int)((x - CameraPosition.X) / ZoomFactor) + Width / 2,
                -(int)((y - CameraPosition.Y) / ZoomFactor) + Height / 2);
        }
        /// <summary>
        /// Converts the given point from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="pt">Location in world coordinates.</param>
        /// <returns>A Point in screen coordinates.</returns>
        public Point WorldToScreen(PointF pt) { return WorldToScreen(pt.X, pt.Y); }
        /// <summary>
        /// Converts the given vector from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="sz">Size in world coordinates.</param>
        /// <returns>A Size in screen coordinates.</returns>
        public Size WorldToScreen(SizeF sz)
        {
            Point pt1 = WorldToScreen(0.0f, 0.0f);
            Point pt2 = WorldToScreen(sz.Width, sz.Height);
            return new Size(pt2.X - pt1.X, pt2.Y - pt1.Y);
        }

        /// <summary>
        /// Converts the given point from screen coordinates to world coordinates.
        /// </summary>
        /// <param name="x">X coordinate in screen coordinates.</param>
        /// <param name="y">Y coordinate in screen coordinates.</param>
        /// <returns>A PointF in world coordinates.</returns>
        public PointF ScreenToWorld(int x, int y)
        {
            return new PointF((x - Width / 2) * ZoomFactor + CameraPosition.X,
                -(y - Height / 2) * ZoomFactor + CameraPosition.Y);
        }
        /// <summary>
        /// Converts the given point from screen coordinates to world coordinates.
        /// </summary>
        /// <param name="pt">Location in screen coordinates.</param>
        /// <returns>A PointF in world coordinates.</returns>
        public PointF ScreenToWorld(Point pt) { return ScreenToWorld(pt.X, pt.Y); }
        /// <summary>
        /// Converts the given vector from screen coordinates to world coordinates.
        /// </summary>
        /// <param name="sz">Size in screen coordinates.</param>
        /// <returns>A SizeF in world coordinates.</returns>
        public SizeF ScreenToWorld(Size sz)
        {
            PointF pt1 = ScreenToWorld(0, 0);
            PointF pt2 = ScreenToWorld(sz.Width, sz.Height);
            return new SizeF(pt2.X - pt1.X, pt2.Y - pt1.Y);
        }

        /// <summary>
        /// Returns the coordinates of the viewport in world coordinates.
        /// </summary>
        public RectangleF GetViewPort()
        {
            PointF bl = ScreenToWorld(0, 0);
            PointF tr = ScreenToWorld(Width, Height);
            return new RectangleF(bl.X, bl.Y, tr.X - bl.X, tr.Y - bl.Y);
        }

        /// <summary>
        /// Sets the viewport to the given model coordinates.
        /// </summary>
        /// <param name="x1">X coordinate of the bottom left corner of the viewport in model coordinates.</param>
        /// <param name="y1">X coordinate of the bottom left corner of the viewport in model coordinates.</param>
        /// <param name="x2">X coordinate of the top right corner of the viewport in model coordinates.</param>
        /// <param name="y2">X coordinate of the top right corner of the viewport in model coordinates.</param>
        public void ZoomToWindow(float x1, float y1, float x2, float y2)
        {
            float h = Math.Abs(y1 - y2);
            float w = Math.Abs(x1 - x2);
            CameraPosition = new PointF((x1 + x2) / 2, (y1 + y2) / 2);
            if ((Height != 0) && (Width != 0))
                ZoomFactor = Math.Max(h / Height, w / Width);
            else
                ZoomFactor = 1;
        }

        /// <summary>
        /// Sets the viewport to the drawing extents.
        /// </summary>
        public void ZoomToExtents()
        {
            RectangleF limits = Document.Model.GetExtents();

            if (limits.IsEmpty) limits = new RectangleF(-250, -250, 500, 500);

            ZoomToWindow(limits.X, limits.Y, limits.X + limits.Width, limits.Y + limits.Height);
            ZoomOut();
        }

        public void Zoom(float zoomFactor)
        {
            ZoomFactor *= zoomFactor;
        }

        public void ZoomIn()
        {
            Zoom(0.9f);
        }

        public void ZoomOut()
        {
            Zoom(1.1f);
        }

        public void Pan(SizeF distance)
        {
            CameraPosition -= distance;
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Calculates graphics scale
        /// </summary>
        /// <param name="g"></param>
        /// <param name="modelWidth"></param>
        /// <param name="modelHeight"></param>
        /// <param name="deviceOffset"></param>
        private void ScaleGraphics(Graphics g)
        {
            g.ResetTransform();
            g.TranslateTransform(-CameraPosition.X, -CameraPosition.Y);
            g.ScaleTransform(1.0f / ZoomFactor, -1.0f / ZoomFactor, MatrixOrder.Append);
            g.TranslateTransform(Width / 2, Height / 2, MatrixOrder.Append);
        }
    }
}
