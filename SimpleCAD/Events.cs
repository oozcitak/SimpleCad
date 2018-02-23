using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

    public class SelectionChangedEventArgs : EventArgs
    {
        public HashSet<Drawable> SelectedItems { get; private set; }

        public SelectionChangedEventArgs(HashSet<Drawable> selection)
        {
            SelectedItems = selection;
        }
    }
}
