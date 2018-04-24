using SimpleCAD.Graphics;
using System.ComponentModel;
using System.Windows.Forms;

namespace SimpleCAD
{
    [Docking(DockingBehavior.Ask)]
    public partial class CADWindow : UserControl
    {
        private CADDocument doc;

        [Browsable(false)]
        public CADView View { get; private set; }

        public override System.Drawing.Color BackColor
        {
            get => System.Drawing.Color.FromArgb((int)Document.Settings.Get<Color>("BackColor").Argb);
            set => Document.Settings.Set("BackColor", Color.FromArgb((uint)value.ToArgb()));
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

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.DoubleBuffer, false);
            UpdateStyles();
            DoubleBuffered = false;

            BorderStyle = BorderStyle.Fixed3D;

            Document = new CADDocument();
        }
    }
}
