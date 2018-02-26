using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace SimpleCAD
{
    public class CADView
    {
        private Control control;
        private PointF mCameraPosition;
        private float mZoomFactor;

        private bool panning;
        private Point lastMouse;
        private Drawable mouseDownItem;

        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control responds to interactive user input.")]
        public bool Interactive { get; set; } = true;
        [Category("Behavior"), DefaultValue(4), Description("Determines the size of the pick box around the selection cursor.")]
        public int PickBoxSize { get; set; } = 4;

        [Category("Appearance"), DefaultValue(5f / 3f), Description("Determines the zoom factor of the view.")]
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

        [Category("Appearance"), DefaultValue(typeof(PointF), "0,0"), Description("Determines the location of the camera.")]
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

        [Browsable(false)]
        public int Width { get; private set; }
        [Browsable(false)]
        public int Height { get; private set; }

        [Browsable(false)]
        public CADDocument Document { get; private set; }

        public CADView(CADDocument document)
        {
            Document = document;

            Width = 1;
            Height = 1;

            mZoomFactor = 5.0f / 3.0f;
            mCameraPosition = new PointF(0, 0);

            panning = false;

            Document.DocumentChanged += Document_Changed;
            Document.TransientsChanged += Document_TransientsChanged;
            Document.SelectionChanged += Document_SelectionChanged;
        }

        public void Attach(Control ctrl)
        {
            control = ctrl;

            Width = ctrl.ClientRectangle.Width;
            Height = ctrl.ClientRectangle.Height;

            mZoomFactor = 5.0f / 3.0f;
            mCameraPosition = new PointF(0, 0);

            ctrl.Resize += CadView_Resize;
            ctrl.MouseDown += CadView_MouseDown;
            ctrl.MouseUp += CadView_MouseUp;
            ctrl.MouseMove += CadView_MouseMove;
            ctrl.MouseClick += CadView_MouseClick;
            ctrl.MouseDoubleClick += CadView_MouseDoubleClick;
            ctrl.MouseWheel += CadView_MouseWheel;
            ctrl.KeyDown += CadView_KeyDown;
            ctrl.KeyPress += CadView_KeyPress;
            ctrl.Paint += CadView_Paint;
        }

        public void Detach()
        {
            if (control != null)
            {
                Width = 1;
                Height = 1;

                mZoomFactor = 5.0f / 3.0f;
                mCameraPosition = new PointF(0, 0);

                control.Resize -= CadView_Resize;
                control.MouseDown -= CadView_MouseDown;
                control.MouseUp -= CadView_MouseUp;
                control.MouseMove -= CadView_MouseMove;
                control.MouseClick -= CadView_MouseClick;
                control.MouseDoubleClick -= CadView_MouseDoubleClick;
                control.MouseWheel -= CadView_MouseWheel;
                control.KeyDown -= CadView_KeyDown;
                control.KeyPress -= CadView_KeyPress;
                control.Paint -= CadView_Paint;
            }
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

            // Render transient objects
            Document.Transients.Draw(param);
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

        private void Document_SelectionChanged(object sender, EventArgs e)
        {
            control.Invalidate();
        }

        private void Document_Changed(object sender, EventArgs e)
        {
            control.Invalidate();
        }

        private void Document_TransientsChanged(object sender, EventArgs e)
        {
            control.Invalidate();
        }

        void CadView_Resize(object sender, EventArgs e)
        {
            Resize(control.ClientRectangle.Width, control.ClientRectangle.Height);
            control.Invalidate();
        }

        void CadView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && Interactive)
            {
                panning = true;
                lastMouse = e.Location;
                control.Cursor = Cursors.NoMove2D;
            }

            if (e.Button == MouseButtons.Left && Interactive)
            {
                mouseDownItem = FindItemAtScreenCoordinates(e.X, e.Y, PickBoxSize);
            }
        }

        void CadView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && panning)
            {
                panning = false;
                control.Invalidate();
            }

            control.Cursor = Cursors.Cross;

            if (e.Button == MouseButtons.Left && Interactive && mouseDownItem != null)
            {
                Drawable mouseUpItem = FindItemAtScreenCoordinates(e.X, e.Y, PickBoxSize);
                if (mouseUpItem != null && ReferenceEquals(mouseDownItem, mouseUpItem))
                {
                    if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
                        Document.Editor.Selection.Remove(mouseDownItem);
                    else
                        Document.Editor.Selection.Add(mouseDownItem);
                }
            }
        }

        void CadView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && panning)
            {
                // Relative mouse movement
                PointF cloc = ScreenToWorld(e.Location);
                PointF ploc = ScreenToWorld(lastMouse);
                SizeF delta = new SizeF(cloc.X - ploc.X, cloc.Y - ploc.Y);
                Pan(delta);
                lastMouse = e.Location;
                control.Invalidate();
            }

            if (Document.Editor.Mode != Editor.InputMode.None)
            {
                Document.Editor.OnViewMouseMove(this, e, new Point2D(ScreenToWorld(e.Location)));
            }
        }

        private void CadView_MouseClick(object sender, MouseEventArgs e)
        {
            if (Document.Editor.Mode != Editor.InputMode.None)
            {
                Document.Editor.OnViewMouseClick(this, e, new Point2D(ScreenToWorld(e.Location)));
            }
        }

        void CadView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Interactive)
            {
                Point pt = e.Location;
                PointF ptw = ScreenToWorld(pt);

                if (e.Delta > 0)
                {
                    ZoomIn();
                }
                else
                {
                    ZoomOut();
                }
                control.Invalidate();
            }
        }

        void CadView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && Interactive)
            {
                ZoomToExtents();
            }
        }

        private void CadView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Document.Editor.Mode != Editor.InputMode.None)
            {
                Document.Editor.OnViewKeyDown(this, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Document.Editor.Selection.Clear();
            }
        }

        private void CadView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Document.Editor.Mode != Editor.InputMode.None)
            {
                Document.Editor.OnViewKeyPress(this, e);
            }
        }

        void CadView_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            e.Graphics.Clear(control.BackColor);

            Render(e.Graphics);
        }

        public Drawable FindItemAtScreenCoordinates(int x, int y, int pickBox)
        {
            PointF pt = ScreenToWorld(x, y);
            float pickBoxWorld = ScreenToWorld(new Size(pickBox, 0)).Width;
            foreach (Drawable d in Document.Model)
            {
                if (d.Contains(new Point2D(pt), pickBoxWorld)) return d;
            }
            return null;
        }
    }
}
