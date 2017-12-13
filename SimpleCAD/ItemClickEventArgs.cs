using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCAD
{
    public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);

    public class ItemClickEventArgs : MouseEventArgs
    {
        public Drawable Item { get; private set; }

        public ItemClickEventArgs(Drawable item, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            Item = item;
        }
    }
}
