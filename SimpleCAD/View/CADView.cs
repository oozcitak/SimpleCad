using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SimpleCAD
{
    public class CADView : IDisposable
    {
        public delegate void CursorEventHandler(object sender, CursorEventArgs e);

        private Control control;

        private bool panning;
        private Point2D lastMouseLocationWorld;
        private Drawable mouseDownItem;
        private Drawable mouseDownCPItem;
        private ControlPoint mouseDownCP;
        private ControlPoint activeCP;
        private Renderer renderer;
        private Type rendererType;

        private View.ViewItems ViewItems { get; set; } = new View.ViewItems();

        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control responds to interactive user input.")]
        public bool Interactive { get; set; } = true;

        [Browsable(false)]
        [Category("Appearance"), DefaultValue(true), Description("Determines the viewing position.")]
        public Camera Camera { get; private set; }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the cartesian grid is shown.")]
        public bool ShowGrid
        {
            get
            {
                return ViewItems.Grid.Visible;
            }
            set
            {
                ViewItems.Grid.Visible = value;
                if (control != null)
                    control.Invalidate();
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the X and Y axes are shown.")]
        public bool ShowAxes
        {
            get
            {
                return ViewItems.Axes.Visible;
            }
            set
            {
                ViewItems.Axes.Visible = value;
                if (control != null)
                    control.Invalidate();
            }
        }

        [Browsable(false)]
        public Type Renderer
        {
            get
            {
                return (renderer?.GetType());
            }
            set
            {
                if (renderer != null)
                {
                    renderer.Dispose();
                    renderer = null;
                }

                rendererType = value;

                if (rendererType != null)
                    renderer = (Renderer)Activator.CreateInstance(rendererType, this);

                if (renderer != null && control != null)
                {
                    renderer.Init(control);
                    control.Invalidate();
                }
            }
        }

        [Browsable(false)]
        public int Width { get; private set; }
        [Browsable(false)]
        public int Height { get; private set; }

        [Browsable(false)]
        public CADDocument Document { get; private set; }

        [Browsable(false)]
        public Point2D CursorLocation { get; private set; }

        public CADView(CADDocument document)
        {
            Document = document;

            Width = 1;
            Height = 1;

            Camera = new Camera(new Point2D(0, 0), 5.0f / 3.0f);
            Renderer = typeof(DirectXRenderer);

            panning = false;

            Document.DocumentChanged += Document_Changed;
            Document.TransientsChanged += Document_TransientsChanged;
            Document.SelectionChanged += Document_SelectionChanged;
            Document.Editor.Prompt += Editor_Prompt;
        }

        public void Attach(Control ctrl)
        {
            if (control != null)
            {
                Width = 1;
                Height = 1;

                Camera = new Camera(new Point2D(0, 0), 5.0f / 3.0f);

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
                control.GotFocus -= Control_GotFocus;
                control.LostFocus -= Control_LostFocus;
            }

            if (renderer != null)
            {
                rendererType = renderer.GetType();
                renderer.Dispose();
            }

            control = ctrl;

            if (rendererType != null)
                Renderer = rendererType;

            Color backColor = Document.Settings.Get<Color>("BackColor");
            control.BackColor = System.Drawing.Color.FromArgb(backColor.A, backColor.R, backColor.G, backColor.B);

            Width = ctrl.ClientRectangle.Width;
            Height = ctrl.ClientRectangle.Height;

            Camera = new Camera(new Point2D(0, 0), 5.0f / 3.0f);

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
            control.GotFocus += Control_GotFocus;
            control.LostFocus += Control_LostFocus;

            control.Invalidate();
        }

        private void Control_LostFocus(object sender, EventArgs e)
        {
            if (ReferenceEquals(Document.ActiveView, this))
                Document.ActiveView = null;
        }

        private void Control_GotFocus(object sender, EventArgs e)
        {
            Document.ActiveView = this;
        }

        public void Render(System.Drawing.Graphics graphics)
        {
            // Start drawing
            renderer.InitFrame(graphics);
            renderer.Clear(Document.Settings.Get<Color>("BackColor"));

            // Grid and axes
            renderer.Draw(ViewItems.Background);

            // Render drawing objects
            renderer.Draw(Document.Model);

            // Render selected objects
            DrawSelection(renderer);

            // Render jigged objects
            DrawJigged(renderer);

            // Render transient objects
            renderer.Draw(Document.Transients);

            // Render cursor
            renderer.Draw(ViewItems.Foreground);

            // End drawing view
            renderer.EndFrame();
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
            Style cpActiveStyle = new Style(Document.Settings.Get<Color>("ActiveControlPointColor"), 2);
            float cpSize = ScreenToWorld(new Vector2D(Document.Settings.Get<int>("ControlPointSize"), 0)).X;

            foreach (Drawable selected in Document.Editor.PickedSelection)
            {
                foreach (ControlPoint pt in ControlPoint.FromDrawable(selected))
                {
                    renderer.DrawRectangle(pt.Equals(activeCP) ? cpActiveStyle : cpStyle,
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

        /// <summary>
        /// Converts the given point from world coordinates to screen coordinates.
        /// </summary>
        /// <param name="x">X coordinate in world coordinates.</param>
        /// <param name="y">Y coordinate in world coordinates.</param>
        /// <returns>A Point in screen coordinates.</returns>
        public Point2D WorldToScreen(float x, float y)
        {
            return new Point2D(((x - Camera.Position.X) / Camera.Zoom) + Width / 2,
                -((y - Camera.Position.Y) / Camera.Zoom) + Height / 2);
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
            return new Point2D((x - Width / 2) * Camera.Zoom + Camera.Position.X,
                -(y - Height / 2) * Camera.Zoom + Camera.Position.Y);
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
        public Extents2D GetViewport()
        {
            Extents2D ex = new Extents2D();
            ex.Add((ScreenToWorld(new Point2D(0, 0))));
            ex.Add((ScreenToWorld(new Point2D(Width, Height))));
            return ex;
        }

        /// <summary>
        /// Sets the viewport to the given model coordinates.
        /// </summary>
        /// <param name="x1">X coordinate of the bottom left corner of the viewport in model coordinates.</param>
        /// <param name="y1">X coordinate of the bottom left corner of the viewport in model coordinates.</param>
        /// <param name="x2">X coordinate of the top right corner of the viewport in model coordinates.</param>
        /// <param name="y2">X coordinate of the top right corner of the viewport in model coordinates.</param>
        public void SetViewport(float x1, float y1, float x2, float y2)
        {
            SetViewport(new Extents2D(x1, y1, x2, y2));
        }

        /// <summary>
        /// Sets the viewport to the given model coordinates.
        /// </summary>
        /// <param name="p1">One corner of the viewport in model coordinates.</param>
        /// <param name="p2">Other corner of the viewport in model coordinates.</param>
        public void SetViewport(Point2D p1, Point2D p2)
        {
            SetViewport(new Extents2D(p1.X, p1.Y, p2.X, p2.Y));
        }

        /// <summary>
        /// Sets the viewport to the given model coordinates.
        /// </summary>
        /// <param name="limits">The new limits of the viewport in model coordinates.</param>
        public void SetViewport(Extents2D limits)
        {
            Camera.Position = limits.Center;
            if ((Height != 0) && (Width != 0))
                Camera.Zoom = Math.Max(limits.Height / Height, limits.Width / Width);
            else
                Camera.Zoom = 1;
        }

        /// <summary>
        /// Sets the viewport to the drawing extents.
        /// </summary>
        public void SetViewport()
        {
            Extents2D limits = Document.Model.GetExtents();
            if (limits.IsEmpty) limits = new Extents2D(-250, -250, 250, 250);

            SetViewport(limits);
            ZoomOut();
        }

        public void Zoom(float zoomFactor)
        {
            Camera.Zoom *= zoomFactor;
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
            Camera.Position += distance;
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            renderer.Resize(width, height);
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

        private void Editor_Prompt(object sender, EditorPromptEventArgs e)
        {
            ViewItems.Cursor.Message = e.Status;
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
                mouseDownItem = FindItem(e.Location, ScreenToWorld(new Vector2D(Document.Settings.Get<int>("PickBoxSize"), 0)).X);
                Tuple<Drawable, ControlPoint> find = FindControlPoint(e.Location, ScreenToWorld(new Vector2D(Document.Settings.Get<int>("ControlPointSize"), 0)).X);
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
            else if (e.Button == MouseButtons.Left && Interactive && Document.Editor.Mode == InputMode.None)
            {
                if (mouseDownItem != null)
                {
                    Drawable mouseUpItem = FindItem(e.Location, ScreenToWorld(new Vector2D(Document.Settings.Get<int>("PickBoxSize"), 0)).X);
                    if (mouseUpItem != null && ReferenceEquals(mouseDownItem, mouseUpItem) && !Document.Editor.PickedSelection.Contains(mouseDownItem))
                    {
                        if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
                        {
                            Document.Editor.PickedSelection.Remove(mouseDownItem);
                        }
                        else
                        {
                            float cpSize = ScreenToWorld(new Vector2D(Document.Settings.Get<int>("ControlPointSize") + 4, 0)).X;
                            Document.Editor.PickedSelection.Add(mouseDownItem);
                        }
                    }
                }

                if (mouseDownCP != null)
                {
                    Tuple<Drawable, ControlPoint> find = FindControlPoint(e.Location, ScreenToWorld(new Vector2D(Document.Settings.Get<int>("ControlPointSize"), 0)).X);
                    Drawable item = find.Item1;
                    ControlPoint mouseUpCP = find.Item2;
                    if (mouseUpCP != null && mouseDownCP.Equals(mouseUpCP))
                    {
                        activeCP = mouseDownCP;
                        ControlPoint cp = mouseDownCP;
                        Drawable consItem = item.Clone();
                        Document.Transients.Add(consItem);
                        ResultMode result = ResultMode.Cancel;
                        Matrix2D trans = Matrix2D.Identity;
                        if (cp.Type == ControlPoint.ControlPointType.Point)
                        {
                            PointResult res = await Document.Editor.GetPoint("New point: ", cp.BasePoint,
                                (p) =>
                                {
                                    trans = Matrix2D.Translation(p - cp.BasePoint);
                                    consItem.TransformControlPoint(cp, trans);
                                });
                            trans = Matrix2D.Translation(res.Value - cp.BasePoint);
                            result = res.Result;
                        }
                        else if (cp.Type == ControlPoint.ControlPointType.Angle)
                        {
                            float orjVal = (cp.Location - cp.BasePoint).Angle;
                            AngleResult res = await Document.Editor.GetAngle("New angle: ", cp.BasePoint,
                                (p) =>
                                {
                                    trans = Matrix2D.Rotation(cp.BasePoint, p - orjVal);
                                    consItem.TransformControlPoint(cp, trans);
                                });
                            trans = Matrix2D.Rotation(cp.BasePoint, res.Value - orjVal);
                            result = res.Result;
                        }
                        else if (cp.Type == ControlPoint.ControlPointType.Distance)
                        {
                            Vector2D dir = (cp.Location - cp.BasePoint).Normal;
                            float orjVal = (cp.Location - cp.BasePoint).Length;
                            DistanceResult res = await Document.Editor.GetDistance("New distance: ", cp.BasePoint,
                                (p) =>
                                {
                                    trans = Matrix2D.Translation(dir * (p - orjVal));
                                    consItem.TransformControlPoint(cp, trans);
                                });
                            trans = Matrix2D.Translation(dir * (res.Value - orjVal));
                            result = res.Result;
                        }

                        // Transform the control point1
                        if (result == ResultMode.OK)
                        {
                            item.TransformControlPoint(cp, trans);
                        }
                        Document.Transients.Remove(consItem);
                        activeCP = null;
                    }
                }
                mouseDownItem = null;
                mouseDownCPItem = null;
                mouseDownCP = null;
            }
        }

        void CadView_CursorMove(object sender, CursorEventArgs e)
        {
            CursorLocation = e.Location;
            ViewItems.Cursor.Location = CursorLocation;
            control.Invalidate();

            if (e.Button == MouseButtons.Middle && panning)
            {
                // Relative mouse movement
                Point2D scrPt = WorldToScreen(e.Location);
                Pan(lastMouseLocationWorld - CursorLocation);
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
                SetViewport();
            }
        }

        private void CadView_MouseLeave(object sender, EventArgs e)
        {
            ViewItems.Cursor.Visible = false;
            Cursor.Show();
            control.Invalidate();
        }

        private void CadView_MouseEnter(object sender, EventArgs e)
        {
            ViewItems.Cursor.Visible = true;
            Cursor.Hide();
            control.Invalidate();
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (renderer != null)
                renderer.Dispose();
            renderer = null;
            rendererType = null;
        }
    }
}
