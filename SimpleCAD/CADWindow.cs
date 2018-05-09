using SimpleCAD.Graphics;
using System.ComponentModel;
using System.Windows.Forms;

namespace SimpleCAD
{
    [Docking(DockingBehavior.Ask)]
    public partial class CADWindow : UserControl
    {
        [Browsable(false)]
        public CADDocument Document { get; private set; }

        [Browsable(false)]
        public CADView View { get; private set; }

        public override System.Drawing.Color BackColor
        {
            get => System.Drawing.Color.FromArgb((int)Document.Settings.BackColor.Argb);
            set => Document.Settings.BackColor = Color.FromArgb((uint)value.ToArgb());
        }

        [Category("Behavior"), DefaultValue(true), Description("Indicates whether the control responds to interactive user input.")]
        public bool Interactive { get => View.Interactive; set => View.Interactive = value; }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the cartesian grid is shown.")]
        public bool ShowGrid { get => View.ShowGrid; set => View.ShowGrid = value; }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the X and Y axes are shown.")]
        public bool ShowAxes { get => View.ShowAxes; set => View.ShowAxes = value; }

        [Category("Appearance"), DefaultValue(true), Description("Determines whether the cursor is shown.")]
        public bool ShowCursor { get => View.ShowCursor; set => View.ShowCursor = value; }

        public CADWindow()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.DoubleBuffer, false);
            UpdateStyles();
            DoubleBuffered = false;

            BorderStyle = BorderStyle.Fixed3D;

            Document = new CADDocument();
            View = new CADView(this, Document);

            Disposed += CADWindow_Disposed;
        }

        private void CADWindow_Disposed(object sender, System.EventArgs e)
        {
            if (View != null)
                View.Dispose();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Tab) == Keys.Tab)
                return true;
            else if ((keyData & Keys.Escape) == Keys.Escape)
                return true;
            else if ((keyData & Keys.Return) == Keys.Return)
                return true;
            else if ((keyData & Keys.Enter) == Keys.Enter)
                return true;
            else
                return base.IsInputKey(keyData);
        }
    }
}
