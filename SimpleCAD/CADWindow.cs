using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleCAD
{
    [Docking(DockingBehavior.Ask)]
    public partial class CADWindow : UserControl
    {
        private CADDocument doc;

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
                if (View != null) View.Detach();
                View = new CADView(doc);
                View.Attach(this);
            }
        }

        public CADWindow()
        {
            InitializeComponent();

            DoubleBuffered = true;
            BorderStyle = BorderStyle.Fixed3D;
            BackColor = Color.FromArgb(33, 40, 48);

            Document = new CADDocument();
        }
    }
}
