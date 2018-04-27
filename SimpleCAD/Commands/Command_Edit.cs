using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleCAD.Commands
{
    public class EditDelete : Command
    {
        public override string RegisteredName => "Edit.Delete";
        public override string Name => "Delete";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;

            var s = await ed.GetSelection("Select objects: ");
            if (s.Result != ResultMode.OK || s.Value.Count == 0) return;
            List<Drawable> toDelete = new List<Drawable>();
            foreach (Drawable item in s.Value)
            {
                toDelete.Add(item);
            }
            foreach(Drawable item in toDelete )
            {
                doc.Model.Remove(item);
            }
        }
    }
}
