using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SimpleCAD
{
    public class CADView : IDisposable, IPersistable
    {
        private bool panning;
        private Point2D lastMouseLocationWorld;
        private Drawable mouseDownItem;
        private Drawable mouseDownCPItem;
        private ControlPoint mouseDownCP;
        private ControlPoint activeCP;
        private Renderer renderer;

        private View.Grid viewGrid = new View.Grid();
        private View.Axes viewAxes = new View.Axes();
        private View.Cursor viewCursor = new View.Cursor();
        private bool showGrid = true;
        private bool showAxes = true;
        private bool showCursor = true;

        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control responds to interactive user input.")]
        public bool Interactive { get; set; } = true;

        [Browsable(false)]
        [Category("Appearance"), DefaultValue(true), Description("Determines the viewing position.")]
        public Camera Camera { get; private set; }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the cartesian grid is shown.")]
        public bool ShowGrid
        {
            get => showGrid;
            set
            {
                showGrid = value;
                Redraw();
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the X and Y axes are shown.")]
        public bool ShowAxes
        {
            get => showAxes;
            set
            {
                showAxes = value;
                Redraw();
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the cursor is shown.")]
        public bool ShowCursor
        {
            get => showCursor;
            set
            {
                showCursor = value;
                Redraw();
            }
        }

        [Browsable(false)]
        public int Width { get; private set; }
        [Browsable(false)]
        public int Height { get; private set; }

        [Browsable(false)]
        public Control Control { get; private set; }

        [Browsable(false)]
        public CADDocument Document { get; private set; }

        [Browsable(false)]
        public Point2D CursorLocation { get; private set; }

        public CADView(Control ctrl, CADDocument document)
        {
            Control = ctrl;
            Document = document;

            Width = 1;
            Height = 1;

            Camera = new Camera(new Point2D(0, 0), 5.0f / 3.0f);
            renderer = new Renderer(this);
            renderer.Init(Control);
            Redraw();

            panning = false;

            Width = ctrl.ClientRectangle.Width;
            Height = ctrl.ClientRectangle.Height;

            Control.Resize += CadView_Resize;
            Control.MouseDown += CadView_MouseDown;
            Control.MouseUp += CadView_MouseUp;
            Control.MouseMove += CadView_MouseMove;
            Control.MouseClick += CadView_MouseClick;
            Control.MouseDoubleClick += CadView_MouseDoubleClick;
            Control.MouseWheel += CadView_MouseWheel;
            Control.KeyDown += CadView_KeyDown;
            Control.KeyPress += CadView_KeyPress;
            Control.Paint += CadView_Paint;
            Control.MouseEnter += CadView_MouseEnter;
            Control.MouseLeave += CadView_MouseLeave;

            Document.DocumentChanged += Document_Changed;
            Document.TransientsChanged += Document_TransientsChanged;
            Document.SelectionChanged += Document_SelectionChanged;
            Document.Editor.Prompt += Editor_Prompt;
            Document.Editor.Error += Editor_Error;
        }

        public void Redraw()
        {
            Control.Invalidate();
        }

        public void Render(System.Drawing.Graphics graphics)
        {
            // Start drawing
            renderer.InitFrame(graphics);
            renderer.Clear(Document.Settings.Get<Color>("BackColor"));

            // Grid and axes
            if (showGrid && viewGrid.Visible) renderer.Draw(viewGrid);
            if (showAxes && viewAxes.Visible) renderer.Draw(viewAxes);

            // Render drawing objects
            renderer.Draw(Document.Model);

            // Render selected objects
            DrawSelection(renderer);

            // Render jigged objects
            DrawJigged(renderer);

            // Render transient objects
            renderer.Draw(Document.Transients);

            // Render cursor
            if (showCursor && viewCursor.Visible) renderer.Draw(viewCursor);

            // Render snap point
            DrawSnapPoint(renderer);

            // End drawing view
            renderer.EndFrame(graphics);
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
                foreach (ControlPoint pt in selected.GetControlPoints())
                {
                    renderer.DrawRectangle(pt.Equals(activeCP) ? cpActiveStyle : cpStyle,
                        new Point2D(pt.Location.X - cpSize / 2, pt.Location.Y - cpSize / 2),
                        new Point2D(pt.Location.X + cpSize / 2, pt.Location.Y + cpSize / 2));
                }
            }
        }

        private void DrawSnapPoint(Renderer renderer)
        {
            if (!Document.Editor.SnapPoints.IsEmpty)
            {
                var pt = Document.Editor.SnapPoints.Current();
                Style style = new Style(Document.Settings.Get<Color>("SnapPointColor"), 2);
                float size = ScreenToWorld(new Vector2D(Document.Settings.Get<int>("SnapPointSize"), 0)).X;

                switch (pt.Type)
                {
                    case SnapPointType.End:
                        renderer.DrawRectangle(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y + size / 2));
                        break;
                    case SnapPointType.Middle:
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y - size / 2));
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2));
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2),
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y - size / 2));
                        break;
                    case SnapPointType.Point:
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y + size / 2));
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y + size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y - size / 2));
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2));
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y));
                        break;
                    case SnapPointType.Center:
                        renderer.DrawCircle(style, pt.Location, size / 2);
                        break;
                    case SnapPointType.Quadrant:
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y));
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y),
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2));
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2),
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y));
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y),
                            new Point2D(pt.Location.X, pt.Location.Y - size / 2));
                        break;
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

            if (renderer != null)
                renderer.Resize(width, height);
        }

        private void Document_SelectionChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void Document_Changed(object sender, EventArgs e)
        {
            Redraw();
        }

        private void Document_TransientsChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void Editor_Prompt(object sender, EditorPromptEventArgs e)
        {
            viewCursor.Message = e.Status;
            Redraw();
        }

        private void Editor_Error(object sender, EditorErrorEventArgs e)
        {
            viewCursor.Message = e.Error.Message;
            Redraw();
        }

        void CadView_Resize(object sender, EventArgs e)
        {
            Resize(Control.ClientRectangle.Width, Control.ClientRectangle.Height);
            Redraw();
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
            else if (e.Button == MouseButtons.Left && Interactive && !Document.Editor.InputMode)
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
                Redraw();
            }
            else if (e.Button == MouseButtons.Left && Interactive && !Document.Editor.InputMode)
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
                    if (ReferenceEquals(item, mouseDownCPItem) && mouseDownCP.Index == mouseUpCP.Index)
                    {
                        activeCP = mouseDownCP;
                        ControlPoint cp = mouseDownCP;
                        Drawable consItem = item.Clone();
                        Document.Transients.Add(consItem);
                        ResultMode result = ResultMode.Cancel;
                        Matrix2D trans = Matrix2D.Identity;
                        if (cp.Type == ControlPointType.Point)
                        {
                            var res = await Document.Editor.GetPoint(cp.Name, cp.BasePoint,
                                (p) =>
                                {
                                    consItem.TransformControlPoint(cp.Index, trans.Inverse);
                                    trans = Matrix2D.Translation(p - cp.BasePoint);
                                    consItem.TransformControlPoint(cp.Index, trans);
                                });
                            trans = Matrix2D.Translation(res.Value - cp.BasePoint);
                            result = res.Result;
                        }
                        else if (cp.Type == ControlPointType.Angle)
                        {
                            float orjVal = (cp.Location - cp.BasePoint).Angle;
                            var res = await Document.Editor.GetAngle(cp.Name, cp.BasePoint,
                                (p) =>
                                {
                                    consItem.TransformControlPoint(cp.Index, trans.Inverse);
                                    trans = Matrix2D.Rotation(cp.BasePoint, p - orjVal);
                                    consItem.TransformControlPoint(cp.Index, trans);
                                });
                            trans = Matrix2D.Rotation(cp.BasePoint, res.Value - orjVal);
                            result = res.Result;
                        }
                        else if (cp.Type == ControlPointType.Distance)
                        {
                            Vector2D dir = (cp.Location - cp.BasePoint).Normal;
                            float orjVal = (cp.Location - cp.BasePoint).Length;
                            var res = await Document.Editor.GetDistance(cp.Name, cp.BasePoint,
                                (p) =>
                                {
                                    consItem.TransformControlPoint(cp.Index, trans.Inverse);
                                    trans = Matrix2D.Scale(cp.BasePoint, p / orjVal);
                                    consItem.TransformControlPoint(cp.Index, trans);
                                });
                            trans = Matrix2D.Scale(cp.BasePoint, res.Value / orjVal);
                            result = res.Result;
                        }

                        // Transform the control point
                        if (result == ResultMode.OK)
                        {
                            item.TransformControlPoint(cp.Index, trans);
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
            viewCursor.Location = CursorLocation;
            Redraw();

            if (e.Button == MouseButtons.Middle && panning)
            {
                // Relative mouse movement
                Point2D scrPt = WorldToScreen(e.Location);
                Pan(lastMouseLocationWorld - CursorLocation);
                lastMouseLocationWorld = ScreenToWorld(scrPt);
                Redraw();
            }

            if (Document.Editor.InputMode)
            {
                Document.Editor.OnViewMouseMove(this, e);
            }
        }

        private void CadView_CursorClick(object sender, CursorEventArgs e)
        {
            if (Document.Editor.InputMode)
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
                Redraw();
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
            viewCursor.Visible = false;
            Cursor.Show();

            if (ReferenceEquals(Document.ActiveView, this))
                Document.ActiveView = null;

            Redraw();
        }

        private void CadView_MouseEnter(object sender, EventArgs e)
        {
            viewCursor.Visible = true;
            Cursor.Hide();

            Document.ActiveView = this;

            Redraw();
        }

        private void CadView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Document.Editor.InputMode)
            {
                Document.Editor.OnViewKeyDown(this, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Document.Editor.PickedSelection.Clear();
                viewCursor.Message = "";
            }
        }

        private void CadView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Document.Editor.InputMode)
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
                int i = 0;
                foreach (ControlPoint cp in item.GetControlPoints())
                {
                    cp.Index = i;
                    i++;
                    if (pt.X >= cp.Location.X - controlPointSize / 2 && pt.X <= cp.Location.X + controlPointSize / 2 &&
                        pt.Y >= cp.Location.Y - controlPointSize / 2 && pt.Y <= cp.Location.Y + controlPointSize / 2)
                        return new Tuple<Drawable, ControlPoint>(item, cp);
                }
            }
            return new Tuple<Drawable, ControlPoint>(null, null);
        }

        public void Dispose()
        {
            if (Document != null)
            {
                Document.DocumentChanged -= Document_Changed;
                Document.TransientsChanged -= Document_TransientsChanged;
                Document.SelectionChanged -= Document_SelectionChanged;
                Document.Editor.Prompt -= Editor_Prompt;
                Document.Editor.Error -= Editor_Error;
            }

            if (Control != null)
            {
                Control.Resize -= CadView_Resize;
                Control.MouseDown -= CadView_MouseDown;
                Control.MouseUp -= CadView_MouseUp;
                Control.MouseMove -= CadView_MouseMove;
                Control.MouseClick -= CadView_MouseClick;
                Control.MouseDoubleClick -= CadView_MouseDoubleClick;
                Control.MouseWheel -= CadView_MouseWheel;
                Control.KeyDown -= CadView_KeyDown;
                Control.KeyPress -= CadView_KeyPress;
                Control.Paint -= CadView_Paint;
                Control.MouseEnter -= CadView_MouseEnter;
                Control.MouseLeave -= CadView_MouseLeave;
            }

            if (renderer != null)
            {
                renderer.Dispose();
                renderer = null;
            }
        }

        public System.Drawing.Image ToBitmap()
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                Render(g);
                g.Flush();
            }
            return bmp;
        }

        public void Load(DocumentReader reader)
        {
            Camera = reader.ReadCamera();
            ShowGrid = reader.ReadBoolean();
            ShowAxes = reader.ReadBoolean();
        }

        public void Save(DocumentWriter writer)
        {
            writer.Write(Camera);
            writer.Write(ShowGrid);
            writer.Write(ShowAxes);
        }
    }
}
