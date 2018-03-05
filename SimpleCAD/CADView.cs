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
        private Point2D currentMouseLocationWorld;
        private bool hasMouse;
        private ControlPoint mouseDownCP;

        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control responds to interactive user input.")]
        public bool Interactive { get; set; } = true;
        [Category("Behavior"), DefaultValue(4), Description("Determines the size of the pick box around the selection cursor.")]
        public int PickBoxSize { get; set; } = 4;
        [Category("Behavior"), DefaultValue(4), Description("Determines the size of the control points of drawable objects.")]
        public int ControlPointSize { get; set; } = 7;

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

            control.Resize += CadView_Resize;
            control.MouseDown += CadView_MouseDown;
            control.MouseUp += CadView_MouseUp;
            control.MouseMove += CadView_MouseMove;
            control.MouseClick += CadView_MouseClick;
            control.MouseDoubleClick += CadView_MouseDoubleClick;
            control.MouseWheel += CadView_MouseWheel;
            control.KeyDown += CadView_KeyDown;
            control.KeyPress += CadView_KeyPress;
            control.Paint += CadView_Paint;
            control.MouseEnter += CadView_MouseEnter;
            control.MouseLeave += CadView_MouseLeave;
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
                control.MouseEnter -= CadView_MouseEnter;
                control.MouseLeave -= CadView_MouseLeave;
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
            param.Mode = DrawParams.DrawingMode.Selection;
            foreach (Drawable selected in Document.Editor.Selection)
            {
                selected.Draw(param);
            }
            param.Mode = DrawParams.DrawingMode.ControlPoint;
            foreach (ControlPoint pt in Document.Editor.ControlPoints)
            {
                DrawControlPoint(param, pt);
            }

            // Render jigged objects
            param.Mode = DrawParams.DrawingMode.Jigged;
            Document.Jigged.Draw(param);

            // Render transient objects
            param.Mode = DrawParams.DrawingMode.Transients;
            Document.Transients.Draw(param);

            // Render cursor
            param.Mode = DrawParams.DrawingMode.Cursor;
            DrawCursor(param);
        }

        private void DrawControlPoint(DrawParams param, ControlPoint pt)
        {
            using (Pen pen = Outline.ControlPointStyle.CreatePen(param))
            {
                pen.Width = param.GetScaledLineWeight(2);
                float cpSize = ScreenToWorld(new Size(ControlPointSize, ControlPointSize)).Width;
                param.Graphics.DrawRectangle(pen, pt.Location.X - cpSize / 2, pt.Location.Y - cpSize / 2, cpSize, cpSize);
            }
        }

        private void DrawCursor(DrawParams param)
        {
            if (hasMouse)
            {
                using (Pen pen = Outline.CursorStyle.CreatePen(param))
                {
                    RectangleF ex = GetViewPort();
                    param.Graphics.DrawLine(pen, ex.Left, currentMouseLocationWorld.Y, ex.Right, currentMouseLocationWorld.Y);
                    param.Graphics.DrawLine(pen, currentMouseLocationWorld.X, ex.Top, currentMouseLocationWorld.X, ex.Bottom);
                }
            }
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
            }
            else if (e.Button == MouseButtons.Left && Interactive)
            {
                mouseDownItem = FindItemAtScreenCoordinates(e.X, e.Y, PickBoxSize);
                mouseDownCP = FindControlPointAtScreenCoordinates(e.X, e.Y, ControlPointSize + 4);
            }
        }

        async void CadView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && Interactive && panning)
            {
                panning = false;
                control.Invalidate();
            }
            else if (e.Button == MouseButtons.Left && Interactive)
            {
                if (mouseDownItem != null)
                {
                    Drawable mouseUpItem = FindItemAtScreenCoordinates(e.X, e.Y, PickBoxSize);
                    if (mouseUpItem != null && ReferenceEquals(mouseDownItem, mouseUpItem) && !Document.Editor.Selection.Contains(mouseDownItem))
                    {
                        if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
                        {
                            Document.Editor.ControlPoints.RemoveAll(p => ReferenceEquals(p.Owner, mouseDownItem));
                            Document.Editor.Selection.Remove(mouseDownItem);
                        }
                        else
                        {
                            float cpSize = ScreenToWorld(new Size(ControlPointSize + 4, 0)).Width;
                            Document.Editor.ControlPoints.AddRange(ControlPoint.FromDrawable(mouseDownItem, cpSize));
                            Document.Editor.Selection.Add(mouseDownItem);
                        }
                    }
                }

                if (mouseDownCP != null)
                {
                    ControlPoint mouseUpCP = FindControlPointAtScreenCoordinates(e.X, e.Y, ControlPointSize + 4);
                    if (mouseUpCP != null && ReferenceEquals(mouseDownCP, mouseUpCP))
                    {
                        ControlPoint cp = mouseDownCP;
                        Drawable consItem = cp.Owner.Clone();
                        Document.Transients.Add(consItem);
                        Editor.ResultMode result = Editor.ResultMode.Cancel;
                        TransformationMatrix2D trans = TransformationMatrix2D.Identity;
                        if (cp.Type == ControlPoint.ControlPointType.Point)
                        {
                            Editor.PointResult res = await Document.Editor.GetPoint("New point: ", cp.BasePoint,
                                (p) =>
                                {
                                    trans = TransformationMatrix2D.Translation(p - cp.BasePoint);
                                    consItem.TransformControlPoint(cp, trans);
                                });
                            trans = TransformationMatrix2D.Translation(res.Value - cp.BasePoint);
                            result = res.Result;
                        }
                        else if (cp.Type == ControlPoint.ControlPointType.Angle)
                        {
                            float orjVal = (cp.Location - cp.BasePoint).Angle;
                            Editor.AngleResult res = await Document.Editor.GetAngle("New angle: ", cp.BasePoint,
                                (p) =>
                                {
                                    trans = TransformationMatrix2D.Rotation(cp.BasePoint, p - orjVal);
                                    consItem.TransformControlPoint(cp, trans);
                                });
                            trans = TransformationMatrix2D.Rotation(cp.BasePoint, res.Value - orjVal);
                            result = res.Result;
                        }
                        else if (cp.Type == ControlPoint.ControlPointType.Distance)
                        {
                            Vector2D dir = (cp.Location - cp.BasePoint).Normal;
                            float orjVal = (cp.Location - cp.BasePoint).Length;
                            Editor.DistanceResult res = await Document.Editor.GetDistance("New distance: ", cp.BasePoint,
                                (p) =>
                                {
                                    trans = TransformationMatrix2D.Translation(dir * (p - orjVal));
                                    consItem.TransformControlPoint(cp, trans);
                                });
                            trans = TransformationMatrix2D.Translation(dir * (res.Value - orjVal));
                            result = res.Result;
                        }
                        if (result == Editor.ResultMode.OK)
                        {
                            float cpSize = ScreenToWorld(new Size(ControlPointSize + 4, 0)).Width;
                            cp.Owner.TransformControlPoint(cp, trans);
                            Document.Editor.ControlPoints.RemoveAll(p => ReferenceEquals(p.Owner, cp.Owner));
                            Document.Editor.ControlPoints.AddRange(ControlPoint.FromDrawable(cp.Owner, cpSize));
                        }
                        Document.Transients.Remove(consItem);
                    }
                }
            }
        }

        void CadView_MouseMove(object sender, MouseEventArgs e)
        {
            currentMouseLocationWorld = new Point2D(ScreenToWorld(e.Location));
            control.Invalidate();

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

        private void CadView_MouseLeave(object sender, EventArgs e)
        {
            hasMouse = false;
            Cursor.Show();
            control.Invalidate();
        }

        private void CadView_MouseEnter(object sender, EventArgs e)
        {
            hasMouse = true;
            Cursor.Hide();
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
                Document.Editor.ControlPoints.Clear();
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

        private ControlPoint FindControlPointAtScreenCoordinates(int x, int y, int controlPointSize)
        {
            PointF pt = ScreenToWorld(x, y);
            float size = ScreenToWorld(new Size(controlPointSize, 0)).Width;
            foreach (ControlPoint cp in Document.Editor.ControlPoints)
            {
                if (pt.X >= cp.Location.X - size / 2 && pt.X <= cp.Location.X + size / 2 &&
                    pt.Y >= cp.Location.Y - size / 2 && pt.Y <= cp.Location.Y + size / 2)
                    return cp;
            }
            return null;
        }
    }
}
