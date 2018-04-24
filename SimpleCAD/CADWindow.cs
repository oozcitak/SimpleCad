using SimpleCAD.Graphics;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SimpleCAD
{
    [Docking(DockingBehavior.Ask)]
    public partial class CADWindow : UserControl
    {
        private CADDocument doc;
        System.Drawing.BufferedGraphics bufferedGraphics = null;
        private bool creatingGraphics = false;

        private const int LazyRefreshInterval = 25;
        private DateTime lastRefreshTime = DateTime.Now;
        private Timer lazyRefreshTimer = new Timer();
        private bool needsPaint = false;

        [Browsable(false)]
        public CADView View { get; private set; }

        public override System.Drawing.Color BackColor
        {
            get
            {
                return System.Drawing.Color.FromArgb((int)Document.Settings.Get<Color>("BackColor").Argb);
            }
            set
            {
                Document.Settings.Set("BackColor", Color.FromArgb((uint)value.ToArgb()));
            }
        }

        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control responds to interactive user input.")]
        public bool Interactive { get => View.Interactive; set => View.Interactive = value; }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the cartesian grid is shown.")]
        public bool ShowGrid { get => View.ShowGrid; set => View.ShowGrid = value; }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the X and Y axes are shown.")]
        public bool ShowAxes { get => View.ShowAxes; set => View.ShowAxes = value; }

        [Browsable(false)]
        public CADDocument Document
        {
            get
            {
                return doc;
            }
            set
            {
                doc = value;
                View = new CADView(doc);
                View.Attach(this);
            }
        }

        public CADWindow()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, false);
            UpdateStyles();
            DoubleBuffered = false;

            BorderStyle = BorderStyle.Fixed3D;

            Document = new CADDocument();

            lazyRefreshTimer.Enabled = false;
            lazyRefreshTimer.Interval = LazyRefreshInterval;
            lazyRefreshTimer.Tick += LazyRefreshTimer_Tick;
        }

        private void LazyRefreshTimer_Tick(object sender, EventArgs e)
        {
            needsPaint = true;
            lazyRefreshTimer.Enabled = false;
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (bufferedGraphics == null)
            {
                if (!RecreateBuffer(e.Graphics)) return;
            }

            if (lazyRefreshTimer.Enabled)
            {
                bufferedGraphics.Render(e.Graphics);
                return;
            }
            else if (!needsPaint)
            {
                lazyRefreshTimer.Enabled = true;
                bufferedGraphics.Render(e.Graphics);
                return;
            }
            needsPaint = false;

            base.OnPaint(new PaintEventArgs(bufferedGraphics.Graphics, ClientRectangle));
            bufferedGraphics.Render(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

        }

        private void ClearBuffer()
        {
            if (bufferedGraphics != null)
                bufferedGraphics.Dispose();
            bufferedGraphics = null;
        }

        private bool RecreateBuffer(System.Drawing.Graphics graphics)
        {
            if (creatingGraphics) return false;

            creatingGraphics = true;

            var bufferContext = System.Drawing.BufferedGraphicsManager.Current;

            int width = Math.Max(Width, 1);
            int height = Math.Max(Height, 1);

            bufferContext.MaximumBuffer = new System.Drawing.Size(width, height);

            ClearBuffer();

            bufferedGraphics = bufferContext.Allocate(graphics, new System.Drawing.Rectangle(0, 0, width, height));

            creatingGraphics = false;

            return true;
        }
    }
}
