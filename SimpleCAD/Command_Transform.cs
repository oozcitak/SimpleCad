using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public partial class Command
    {
        public class TransformMove : Command
        {
            public override string RegisteredName => "Transform.Move";
            public override string Name => "Move";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.SelectionResult s = await ed.GetSelection("Select objects: ");
                if (s.Result != Editor.ResultMode.OK || s.Value.Count == 0) return;
                Editor.PointResult p1 = await ed.GetPoint("Base point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Composite consItems = new Composite();
                foreach (Drawable item in s.Value)
                {
                    consItems.Add(item.Clone());
                }
                consItems.Outline = doc.Editor.TransientStyle;
                consItems.CopyStyleToChildren();
                doc.Transients.Add(consItems);
                Point2D lastPt = p1.Value;
                Editor.PointResult p2 = await ed.GetPoint("Translated point: ", p1.Value,
                    (p) => { consItems.TransformBy(TransformationMatrix2D.Translation(p - lastPt)); lastPt = p; });
                if (p2.Result != Editor.ResultMode.OK) return;
                doc.Transients.Remove(consItems);

                foreach (Drawable item in s.Value)
                {
                    item.TransformBy(TransformationMatrix2D.Translation(p2.Value - p1.Value));
                }

                ed.Selection.Clear();
            }
        }
    }
}