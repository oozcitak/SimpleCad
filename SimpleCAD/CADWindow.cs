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
        private bool panning;
        private Point lastMouse;
        private Drawable mouseDownItem;

        public event SelectionChangedEventHandler SelectionChanged;

        [Browsable(false)]
        public CADView View { get; private set; }
        [Browsable(false)]
        public Composite Model { get; private set; }
        [Browsable(false)]
        public Editor Editor { get; private set; }
        [Browsable(false)]
        public float DrawingScale { get { return View.ZoomFactor; } }

        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control allows zooming and panning using the mouse.")]
        public bool AllowZoomAndPan { get; set; } = true;
        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control allows selecting items using the mouse.")]
        public bool AllowSelect { get; set; } = true;
        [Category("Behavior"), DefaultValue(4), Description("Determines the size of the pick box around the selection cursor.")]
        public int PickBoxSize { get; set; } = 4;

        public CADWindow()
        {
            InitializeComponent();

            DoubleBuffered = true;

            Model = new Composite();
            Editor = new Editor();
            View = new CADView(Model, Editor, ClientRectangle.Width, ClientRectangle.Height);

            panning = false;

            BorderStyle = BorderStyle.Fixed3D;
            BackColor = Color.FromArgb(33, 40, 48);
            Cursor = Cursors.Cross;

            Resize += CadView_Resize;
            MouseDown += CadView_MouseDown;
            MouseUp += CadView_MouseUp;
            MouseMove += CadView_MouseMove;
            MouseDoubleClick += CadView_MouseDoubleClick;
            MouseWheel += CadView_MouseWheel;
            KeyDown += CADWindow_KeyDown;
            Paint += CadView_Paint;
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
            if (e.Button == MouseButtons.Middle && AllowZoomAndPan)
            {
                panning = true;
                lastMouse = e.Location;
                Cursor = Cursors.NoMove2D;
            }

            if (e.Button == MouseButtons.Left && AllowSelect)
            {
                mouseDownItem = FindItemAtScreenCoordinates(e.X, e.Y, PickBoxSize);
            }
        }

        void CadView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && panning)
            {
                panning = false;
                Invalidate();
            }

            Cursor = Cursors.Cross;

            if (e.Button == MouseButtons.Left && AllowSelect && mouseDownItem != null)
            {
                Drawable mouseUpItem = FindItemAtScreenCoordinates(e.X, e.Y, PickBoxSize);
                if (mouseUpItem != null && ReferenceEquals(mouseDownItem, mouseUpItem))
                {
                    if ((ModifierKeys & Keys.Shift) != Keys.None)
                        Editor.Selection.Remove(mouseDownItem);
                    else
                        Editor.Selection.Add(mouseDownItem);
                    Invalidate();
                    OnSelectionChanged(new SelectionChangedEventArgs(Editor.Selection));
                }
            }
        }

        void CadView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && panning)
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
            if (e.Button == MouseButtons.Middle && AllowZoomAndPan)
            {
                View.ZoomToExtents();
            }
        }

        private void CADWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Editor.Selection.Clear();
                Invalidate();
                OnSelectionChanged(new SelectionChangedEventArgs(Editor.Selection));
            }
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

        protected void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged.Invoke(this, e);
        }
    }
}
