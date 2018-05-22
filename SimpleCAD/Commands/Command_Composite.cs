using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleCAD.Commands
{
    public class CompositeCreate : Command
    {
        public override string RegisteredName => "Composite.Create";
        public override string Name => "Create Composite";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;

            var s = await ed.GetSelection("Select objects: ");
            if (s.Result != ResultMode.OK || s.Value.Count == 0) return;
            var p = await ed.GetPoint("Base point: ");
            if (p.Result != ResultMode.OK) return;
            var t = await ed.GetText("Name: ");
            if (t.Result != ResultMode.OK || string.IsNullOrEmpty(t.Value)) return;

            Composite composite = new Composite();
            composite.Name = t.Value;

            Matrix2D matrix = Matrix2D.Translation(-1 * p.Value.AsVector2D());
            List<Drawable> toDelete = new List<Drawable>();
            foreach (Drawable item in s.Value)
            {
                item.TransformBy(matrix);
                toDelete.Add(item);
                composite.Add(item);
            }

            doc.Composites.Add(composite.Name, composite);

            foreach (Drawable item in toDelete)
            {
                doc.Model.Remove(item);
            }

            CompositeReference compRef = new CompositeReference();
            compRef.Composite = composite;
            compRef.Location = p.Value;
            doc.Model.Add(compRef);
        }
    }
}
