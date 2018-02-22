using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleCAD
{
    [Docking(DockingBehavior.Ask)]
    public partial class CADWindow : UserControl
    {
        [Category("Action"), Description("Occurs when an item is clicked with the mouse.")]
        public event ItemClickEventHandler ItemClick;

        private bool panning;
        private Point lastMouse;
        private Drawable mouseDownItem;

        [Browsable(false)]
        public CADView View { get; private set; }
        [Browsable(false)]
        public Composite Model { get; private set; }
        [Browsable(false)]
        public float DrawingScale { get { return View.ZoomFactor; } }

        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control allows zooming and panning using the mouse.")]
        public bool AllowZoomAndPan { get; set; }
        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control allows picking items using the mouse.")]
        public bool AllowItemClick { get; set; }

        public CADWindow()
        {
            InitializeComponent();

            DoubleBuffered = true;

            Model = new Composite();
            View = new CADView(Model, ClientRectangle.Width, ClientRectangle.Height);

            AllowZoomAndPan = true;
            panning = false;
            AllowItemClick = true;

            BorderStyle = BorderStyle.Fixed3D;
            BackColor = Color.White;
            Cursor = Cursors.Cross;

            Resize += new EventHandler(CadView_Resize);
            MouseDown += new MouseEventHandler(CadView_MouseDown);
            MouseUp += new MouseEventHandler(CadView_MouseUp);
            MouseMove += new MouseEventHandler(CadView_MouseMove);
            MouseDoubleClick += new MouseEventHandler(CadView_MouseDoubleClick);
            MouseWheel += new MouseEventHandler(CadView_MouseWheel);
            Paint += new PaintEventHandler(CadView_Paint);
        }

        public void ZoomIn()
        {
            View.ZoomIn();
            Invalidate();
        }

        public void ZoomOut()
        {
            View.ZoomOut();
            Invalidate();
        }

        public void ZoomToExtents()
        {
            View.ZoomToExtents();
            Invalidate();
        }

        void CadView_Resize(object sender, EventArgs e)
        {
            View.Resize(ClientRectangle.Width, ClientRectangle.Height);
            Invalidate();
        }

        void CadView_MouseDown(object sender, MouseEventArgs e)
        {
            if (((e.Button & MouseButtons.Middle) != MouseButtons.None) && AllowZoomAndPan)
            {
                panning = true;
                lastMouse = e.Location;
                Cursor = Cursors.NoMove2D;
            }

            if (AllowItemClick)
            {
                mouseDownItem = FindItemAtScreenCoordinates(e.X, e.Y, 4);
            }
        }

        void CadView_MouseUp(object sender, MouseEventArgs e)
        {
            if (((e.Button & MouseButtons.Middle) != MouseButtons.None) && panning)
            {
                panning = false;
                Invalidate();
            }

            Cursor = Cursors.Cross;

            if (AllowItemClick && mouseDownItem != null)
            {
                Drawable mouseUpItem = FindItemAtScreenCoordinates(e.X, e.Y, 4);
                if (mouseUpItem != null && ReferenceEquals(mouseDownItem, mouseUpItem))
                {
                    OnItemClick(new ItemClickEventArgs(mouseUpItem, e.Button, e.Clicks, e.X, e.Y, e.Delta));
                }
            }
        }

        void CadView_MouseMove(object sender, MouseEventArgs e)
        {
            if (((e.Button & MouseButtons.Middle) != MouseButtons.None) && panning)
            {
                // Relative mouse movement
                PointF cloc = View.ScreenToWorld(e.Location);
                PointF ploc = View.ScreenToWorld(lastMouse);
                SizeF delta = new SizeF(cloc.X - ploc.X, cloc.Y - ploc.Y);
                View.Pan(delta);
                lastMouse = e.Location;
                Invalidate();
            }
        }

        void CadView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (AllowZoomAndPan)
            {
                Point pt = e.Location;
                PointF ptw = View.ScreenToWorld(pt);

                if (e.Delta > 0)
                {
                    View.ZoomIn();
                }
                else
                {
                    View.ZoomOut();
                }
                Invalidate();
            }
        }

        void CadView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (((e.Button & MouseButtons.Middle) != MouseButtons.None) && AllowZoomAndPan)
            {
                View.ZoomToExtents();
            }
        }

        protected virtual void OnItemClick(ItemClickEventArgs e)
        {
            ItemClick?.Invoke(this, e);
        }

        void CadView_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            e.Graphics.Clear(BackColor);

            View.Render(e.Graphics);
        }

        public Drawable FindItemAtScreenCoordinates(int x, int y, int pickBox)
        {
            PointF pt = View.ScreenToWorld(x, y);
            float pickBoxWorld = View.ScreenToWorld(new Size(pickBox, 0)).Width;
            foreach (Drawable d in Model)
            {
                if (d.Contains(new Point2D(pt), pickBoxWorld)) return d;
            }
            return null;
        }
    }
}
