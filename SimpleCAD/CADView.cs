using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SimpleCAD
{
    public class CADView
    {
        public delegate void CursorEventHandler(object sender, CursorEventArgs e);

        private Control control;
        private Point2D mCameraPosition;
        private float mZoomFactor;

        private bool panning;
        private Point2D lastMouseLocationWorld;
        private Drawable mouseDownItem;
        private Point2D currentMouseLocationWorld;
        private bool hasMouse;
        private Drawable mouseDownCPItem;
        private ControlPoint mouseDownCP;
        private string cursorMessage;
        private Type rendererType;

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

        [Category("Appearance"), DefaultValue(typeof(System.Drawing.PointF), "0,0"), Description("Determines the location of the camera.")]
        public Point2D CameraPosition
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
                mCameraPosition = new Point2D(x, y);
            }
        }

        [Browsable(false)]
        public int Width { get; private set; }
        [Browsable(false)]
        public int Height { get; private set; }

        [Browsable(false)]
        public CADDocument Document { get; private set; }

        [Browsable(false)]
        public Point2D CursorLocation { get { return currentMouseLocationWorld; } }

        public CADView(CADDocument document)
        {
            Document = document;

            Width = 1;
            Height = 1;

            mZoomFactor = 5.0f / 3.0f;
            mCameraPosition = new Point2D(0, 0);

            panning = false;

            Document.DocumentChanged += Document_Changed;
            Document.TransientsChanged += Document_TransientsChanged;
            Document.SelectionChanged += Document_SelectionChanged;
            Document.Editor.CursorPrompt += Editor_CursorPrompt;
        }

        public void Attach(Control ctrl)
        {
            control = ctrl;
            Color backColor = Document.Settings.Get<Color>("BackColor");
            control.BackColor = System.Drawing.Color.FromArgb(backColor.A, backColor.R, backColor.G, backColor.B);

            Width = ctrl.ClientRectangle.Width;
            Height = ctrl.ClientRectangle.Height;

            mZoomFactor = 5.0f / 3.0f;
            mCameraPosition = new Point2D(0, 0);

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
                mCameraPosition = new Point2D(0, 0);

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

        public void SetRenderer(Type type)
        {
            rendererType = type;
        }

        public void Render(System.Drawing.Graphics graphics)
        {
            Renderer renderer = (Renderer)Activator.CreateInstance(rendererType, this, graphics);

            // Set an orthogonal projection matrix
            ScaleGraphics(graphics);

            // Render drawing objects
            Document.Model.Draw(renderer);

            // Render selected objects
            DrawSelection(renderer);

            // Render jigged objects
            DrawJigged(renderer);

            // Render transient objects
            Document.Transients.Draw(renderer);

            // Render cursor
            DrawCursor(renderer);
        }

        private void DrawSelection(Renderer renderer)
        {
            renderer.StyleOverride = new Style(Document.Settings.Get<Color>("SelectionHighlightColor"), 5, DashStyle.Solid);
            // Current selection
            foreach (Drawable selected in Document.Editor.CurrentSelection)
            {
                renderer.Draw(selected);
            }
            // Picked objects
            foreach (Drawable selected in Document.Editor.PickedSelection)
            {
                renderer.Draw(selected);
            }
            renderer.StyleOverride = null;
            // Control points
            Style cpStyle = new Style(Document.Settings.Get<Color>("ControlPointColor"), 2);
            float cpSize = ScreenToWorld(new Vector2D(ControlPointSize, 0)).X;

            foreach (Drawable selected in Document.Editor.PickedSelection)
            {
                foreach (ControlPoint pt in ControlPoint.FromDrawable(selected))
                {
                    renderer.DrawRectangle(cpStyle,
                        new Point2D(pt.Location.X - cpSize / 2, pt.Location.Y - cpSize / 2),
                        new Point2D(pt.Location.X + cpSize / 2, pt.Location.Y + cpSize / 2));
                }
            }
        }

        private void DrawJigged(Renderer renderer)
        {
            renderer.StyleOverride = new Style(Document.Settings.Get<Color>("JigColor"), 0, DashStyle.Dash);
            renderer.Draw(Document.Jigged);
            renderer.StyleOverride = null;
        }

        private void DrawCursor(Renderer renderer)
        {
            if (hasMouse)
            {
                Extents2D ex = GetViewPort();
                Style cursorStyle = new Style(Document.Settings.Get<Color>("CursorColor"));

                // Draw cursor
                renderer.DrawLine(cursorStyle, new Point2D(ex.XMin, CursorLocation.Y), new Point2D(ex.XMax, CursorLocation.Y));
                renderer.DrawLine(cursorStyle, new Point2D(CursorLocation.X, ex.YMin), new Point2D(CursorLocation.X, ex.YMax));

                // Draw cursor prompt
                if (!string.IsNullOrEmpty(cursorMessage))
                {
                    string fontFamily = control.Font.FontFamily.Name;
                    float textHeight = ScreenToWorld(new Vector2D(8, 0)).X;
                    float margin = ScreenToWorld(new Vector2D(4, 0)).X;
                    float offset = ScreenToWorld(new Vector2D(2, 0)).X;

                    // position cursor prompt to lower-right of cursor by default
                    float x = CursorLocation.X + margin + offset;
                    float y = CursorLocation.Y + margin + offset;
                    Vector2D sz = renderer.MeasureString(cursorMessage, fontFamily, textHeight);
                    Point2D lowerRight = new Point2D(ex.XMax, ex.YMax);
                    // check if the prompt text fits into the window horizontally
                    if (x + sz.X + offset > lowerRight.X)
                    {
                        x = CursorLocation.X - margin - offset - sz.X;
                    }
                    // check if the prompt text fits into the window vertically
                    if (y + sz.Y + offset > lowerRight.Y)
                    {
                        y = CursorLocation.Y - margin - offset - sz.Y;
                    }

                    // Draw cursor prompt
                    Style fore = new Style(Document.Settings.Get<Color>("CursorPromptForeColor"));
                    Style back = new Style(Document.Settings.Get<Color>("CursorPromptBackColor"));
                    back.Fill = true;
                    renderer.DrawRectangle(back, new Point2D(x - offset, y - offset), new Point2D(x + 2 * offset + sz.X, y + 2 * offset + sz.Y));
                    renderer.DrawRectangle(fore, new Point2D(x - offset, y - offset), new Point2D(x + 2 * offset + sz.X, y + 2 * offset + sz.Y));
                    renderer.DrawString(fore, new Point2D(x, y), cursorMessage, fontFamily, textHeight);
                }
            }
        }

        /// <summary>
        /// Converts the given point from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="x">X coordinate in world coordinates.</param>
        /// <param name="y">Y coordinate in world coordinates.</param>
        /// <returns>A Point in screen coordinates.</returns>
        public Point2D WorldToScreen(float x, float y)
        {
            return new Point2D(((x - CameraPosition.X) / ZoomFactor) + Width / 2,
                -((y - CameraPosition.Y) / ZoomFactor) + Height / 2);
        }
        /// <summary>
        /// Converts the given point from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="pt">Location in world coordinates.</param>
        /// <returns>A Point in screen coordinates.</returns>
        public Point2D WorldToScreen(Point2D pt) { return WorldToScreen(pt.X, pt.Y); }
        /// <summary>
        /// Converts the given vector from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="sz">Size in world coordinates.</param>
        /// <returns>A Size in screen coordinates.</returns>
        public Vector2D WorldToScreen(Vector2D sz)
        {
            Point2D pt1 = WorldToScreen(0.0f, 0.0f);
            Point2D pt2 = WorldToScreen(sz.X, sz.Y);
            return (pt2 - pt1);
        }

        /// <summary>
        /// Converts the given point from screen coordinates to world coordinates.
        /// </summary>
        /// <param name="x">X coordinate in screen coordinates.</param>
        /// <param name="y">Y coordinate in screen coordinates.</param>
        /// <returns>A PointF in world coordinates.</returns>
        public Point2D ScreenToWorld(float x, float y)
        {
            return new Point2D((x - Width / 2) * ZoomFactor + CameraPosition.X,
                -(y - Height / 2) * ZoomFactor + CameraPosition.Y);
        }
        /// <summary>
        /// Converts the given point from screen coordinates to world coordinates.
        /// </summary>
        /// <param name="pt">Location in screen coordinates.</param>
        /// <returns>A PointF in world coordinates.</returns>
        public Point2D ScreenToWorld(Point2D pt) { return ScreenToWorld(pt.X, pt.Y); }
        /// <summary>
        /// Converts the given vector from screen coordinates to world coordinates.
        /// </summary>
        /// <param name="sz">Size in screen coordinates.</param>
        /// <returns>A SizeF in world coordinates.</returns>
        public Vector2D ScreenToWorld(Vector2D sz)
        {
            Point2D pt1 = ScreenToWorld(0, 0);
            Point2D pt2 = ScreenToWorld(sz.X, sz.Y);
            return (pt2 - pt1);
        }

        /// <summary>
        /// Returns the coordinates of the viewport in world coordinates.
        /// </summary>
        public Extents2D GetViewPort()
        {
            return new Extents2D(0, 0, Width, Height);
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
            ZoomToWindow(new Extents2D(x1, y1, x2, y2));
        }

        /// <summary>
        /// Sets the viewport to the given model coordinates.
        /// </summary>
        /// <param name="limits">The new limits of the viewport in model coordinates.</param>
        public void ZoomToWindow(Extents2D limits)
        {
            CameraPosition = limits.Center;
            if ((Height != 0) && (Width != 0))
                ZoomFactor = Math.Max(limits.Height / Height, limits.Width / Width);
            else
                ZoomFactor = 1;
        }

        /// <summary>
        /// Sets the viewport to the drawing extents.
        /// </summary>
        public void ZoomToExtents()
        {
            Extents2D limits = Document.Model.GetExtents();
            if (limits.IsEmpty) limits = new Extents2D(-250, -250, 250, 250);

            ZoomToWindow(limits);
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

        public void Pan(Vector2D distance)
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
        private void ScaleGraphics(System.Drawing.Graphics g)
        {
            g.ResetTransform();
            g.TranslateTransform(-CameraPosition.X, -CameraPosition.Y);
            g.ScaleTransform(1.0f / ZoomFactor, -1.0f / ZoomFactor, System.Drawing.Drawing2D.MatrixOrder.Append);
            g.TranslateTransform(Width / 2, Height / 2, System.Drawing.Drawing2D.MatrixOrder.Append);
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

        private void Editor_CursorPrompt(object sender, CursorPromptEventArgs e)
        {
            cursorMessage = e.Status;
            control.Invalidate();
        }

        void CadView_Resize(object sender, EventArgs e)
        {
            Resize(control.ClientRectangle.Width, control.ClientRectangle.Height);
            control.Invalidate();
        }

        void CadView_MouseDown(object sender, MouseEventArgs e)
        {
            CadView_CursorDown(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }

        void CadView_MouseUp(object sender, MouseEventArgs e)
        {
            CadView_CursorUp(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }

        void CadView_MouseMove(object sender, MouseEventArgs e)
        {
            CadView_CursorMove(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }

        private void CadView_MouseClick(object sender, MouseEventArgs e)
        {
            CadView_CursorClick(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }

        private void CadView_MouseWheel(object sender, MouseEventArgs e)
        {
            CadView_CursorWheel(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }

        private void CadView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CadView_CursorDoubleClick(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }

        void CadView_CursorDown(object sender, CursorEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && Interactive)
            {
                panning = true;
                lastMouseLocationWorld = e.Location;
            }
            else if (e.Button == MouseButtons.Left && Interactive)
            {
                mouseDownItem = FindItem(e.Location, ScreenToWorld(new Vector2D(PickBoxSize, 0)).X);
                Tuple<Drawable, ControlPoint> find = FindControlPoint(e.Location, ScreenToWorld(new Vector2D(ControlPointSize, 0)).X);
                mouseDownCPItem = find.Item1;
                mouseDownCP = find.Item2;
            }
        }

        async void CadView_CursorUp(object sender, CursorEventArgs e)
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
                    Drawable mouseUpItem = FindItem(e.Location, ScreenToWorld(new Vector2D(PickBoxSize, 0)).X);
                    if (mouseUpItem != null && ReferenceEquals(mouseDownItem, mouseUpItem) && !Document.Editor.PickedSelection.Contains(mouseDownItem))
                    {
                        if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
                        {
                            Document.Editor.PickedSelection.Remove(mouseDownItem);
                        }
                        else
                        {
                            float cpSize = ScreenToWorld(new Vector2D(ControlPointSize + 4, 0)).X;
                            Document.Editor.PickedSelection.Add(mouseDownItem);
                        }
                    }
                }

                if (mouseDownCP != null)
                {
                    Tuple<Drawable, ControlPoint> find = FindControlPoint(e.Location, ScreenToWorld(new Vector2D(ControlPointSize, 0)).X);
                    Drawable item = find.Item1;
                    ControlPoint mouseUpCP = find.Item2;
                    if (mouseUpCP != null && ReferenceEquals(mouseDownCPItem, item) &&
                        mouseDownCP.PropertyName == mouseUpCP.PropertyName && mouseDownCP.PropertyIndex == mouseUpCP.PropertyIndex)
                    {
                        ControlPoint cp = mouseDownCP;
                        Drawable consItem = item.Clone();
                        Document.Transients.Add(consItem);
                        ResultMode result = ResultMode.Cancel;
                        TransformationMatrix2D trans = TransformationMatrix2D.Identity;
                        if (cp.Type == ControlPoint.ControlPointType.Point)
                        {
                            PointResult res = await Document.Editor.GetPoint("New point: ", cp.BasePoint,
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
                            AngleResult res = await Document.Editor.GetAngle("New angle: ", cp.BasePoint,
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
                            DistanceResult res = await Document.Editor.GetDistance("New distance: ", cp.BasePoint,
                                (p) =>
                                {
                                    trans = TransformationMatrix2D.Translation(dir * (p - orjVal));
                                    consItem.TransformControlPoint(cp, trans);
                                });
                            trans = TransformationMatrix2D.Translation(dir * (res.Value - orjVal));
                            result = res.Result;
                        }
                        if (result == ResultMode.OK)
                        {
                            item.TransformControlPoint(cp, trans);
                        }
                        Document.Transients.Remove(consItem);
                    }
                }
            }
        }

        void CadView_CursorMove(object sender, CursorEventArgs e)
        {
            currentMouseLocationWorld = e.Location;
            control.Invalidate();

            if (e.Button == MouseButtons.Middle && panning)
            {
                // Relative mouse movement
                Point2D scrPt = WorldToScreen(e.Location);
                Pan(currentMouseLocationWorld - lastMouseLocationWorld);
                lastMouseLocationWorld = ScreenToWorld(scrPt);
                control.Invalidate();
            }

            if (Document.Editor.Mode != InputMode.None)
            {
                Document.Editor.OnViewMouseMove(this, e);
            }
        }

        private void CadView_CursorClick(object sender, CursorEventArgs e)
        {
            if (Document.Editor.Mode != InputMode.None)
            {
                Document.Editor.OnViewMouseClick(this, e);
            }
        }

        void CadView_CursorWheel(object sender, CursorEventArgs e)
        {
            if (Interactive)
            {
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

        void CadView_CursorDoubleClick(object sender, CursorEventArgs e)
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
            if (Document.Editor.Mode != InputMode.None)
            {
                Document.Editor.OnViewKeyDown(this, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Document.Editor.PickedSelection.Clear();
            }
        }

        private void CadView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Document.Editor.Mode != InputMode.None)
            {
                Document.Editor.OnViewKeyPress(this, e);
            }
        }

        void CadView_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            e.Graphics.Clear(control.BackColor);

            Render(e.Graphics);
        }

        private Drawable FindItem(Point2D pt, float pickBox)
        {
            float pickBoxWorld = ScreenToWorld(new Vector2D(pickBox, 0)).X;
            foreach (Drawable d in Document.Model)
            {
                if (d.Contains(pt, pickBoxWorld)) return d;
            }
            return null;
        }

        private Tuple<Drawable, ControlPoint> FindControlPoint(Point2D pt, float controlPointSize)
        {
            foreach (Drawable item in Document.Editor.PickedSelection)
            {
                foreach (ControlPoint cp in ControlPoint.FromDrawable(item))
                {
                    if (pt.X >= cp.Location.X - controlPointSize / 2 && pt.X <= cp.Location.X + controlPointSize / 2 &&
                        pt.Y >= cp.Location.Y - controlPointSize / 2 && pt.Y <= cp.Location.Y + controlPointSize / 2)
                        return new Tuple<Drawable, ControlPoint>(item, cp);
                }
            }
            return new Tuple<Drawable, ControlPoint>(null, null);
        }
    }
}
