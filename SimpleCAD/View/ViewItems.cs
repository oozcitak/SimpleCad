using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD.View
{
    internal class ViewItems
    {
        public Drawables.DrawableDictionary Foreground { get; } = new Drawables.DrawableDictionary();
        public Drawables.DrawableDictionary Background { get; } = new Drawables.DrawableDictionary();

        public Cursor Cursor { get => (Cursor)Foreground["Cursor"]; }
        public Grid Grid { get => (Grid)Foreground["Grid"]; }
        public Axes Axes { get => (Axes)Foreground["Axes"]; }

        public ViewItems()
        {
            Foreground.Add("Cursor", new Cursor());
            Background.Add("Grid", new Grid());
            Background.Add("Axes", new Axes());
        }
    }
}
