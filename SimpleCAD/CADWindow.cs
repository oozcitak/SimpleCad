using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleCAD
{
    [Docking(DockingBehavior.Ask)]
    public partial class CADWindow : UserControl
    {
        [Browsable(false)]
        public CADView View { get; private set; }
        [Browsable(false)]
        public CADDocument Document { get; private set; }

        public CADWindow()
        {
            InitializeComponent();

            DoubleBuffered = true;

            Document = new CADDocument();
            View = new CADView(Document);
            View.Attach(this);

            BorderStyle = BorderStyle.Fixed3D;
            BackColor = Color.FromArgb(33, 40, 48);
            Cursor = Cursors.Cross;
        }
    }
}
