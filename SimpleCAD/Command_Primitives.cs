using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public partial class Command
    {
        public class DrawLine : Command
        {
            public override string RegisteredName => "Primitives.Line";
            public override string Name => "Line";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.PointResult p1 = await ed.GetPoint("Start point: ");
                if (!p1.Success) return;
                Editor.PointResult p2 = await ed.GetPoint("End point: ");
                if (!p2.Success) return;

                Drawable newItem = new Line(p1.Location, p2.Location);
                doc.Model.Add(newItem);
            }
        }
    }
}
