using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace SimpleCAD
{
    public partial class CADWindow : UserControl
    {
        public event ItemClickEventHandler ItemClick;

        private bool panning;
        private Point lastMouse;
        private Drawable mouseDownItem;

        public CADView View { get; private set; }
        public CADModel Model { get; private set; }
        public float DrawingScale { get { return View.ZoomFactor; } }

        public bool AllowZoomAndPan { get; set; }
        public bool AllowItemClick { get; set; }

        public CADWindow()
        {
            InitializeComponent();

            DoubleBuffered = true;

            Model = new CADModel();
            View = new CADView(Model, ClientRectangle.Width, ClientRectangle.Height);

            AllowZoomAndPan = true;
            panning = false;
            AllowItemClick = true;

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
                mouseDownItem = FindItemAtScreenCoordinates(e.X, e.Y);
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
                Drawable mouseUpItem = FindItemAtScreenCoordinates(e.X, e.Y);
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

        public Drawable FindItemAtScreenCoordinates(int x, int y)
        {
            PointF pt = View.ScreenToWorld(x, y);
            foreach (Drawable d in Model)
            {
                if (d.Contains(pt)) return d;
            }
            return null;
        }
    }
}
