using System.ComponentModel;
using System.Windows.Forms;

namespace SimpleCAD
{
    [Docking(DockingBehavior.Ask)]
    public partial class CADWindow : UserControl
    {
        private CADDocument doc = new CADDocument();

        [Browsable(false)]
        public CADView View { get; private set; }
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

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.DoubleBuffer, false);
            DoubleBuffered = false;
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            BorderStyle = BorderStyle.Fixed3D;
        }
    }
}
