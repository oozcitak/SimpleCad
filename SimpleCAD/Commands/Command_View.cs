using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using System.Threading.Tasks;

namespace SimpleCAD.Commands
{
    public class ViewZoom : Command
    {
        public override string RegisteredName => "View.Zoom";
        public override string Name => "Zoom";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            CADView view = doc.ActiveView;
            if (view == null) return;

            Editor ed = doc.Editor;

            PointOptions opts = new PointOptions("Corner of zoom window: ");
            opts.AddKeyword("Extents");
            opts.AddKeyword("Scale");
            opts.AddKeyword("Object");
            PointResult p1 = await ed.GetPoint(opts);
            if (p1.Result == ResultMode.Cancel) return;

            if (p1.Result == ResultMode.Keyword && p1.Keyword == "Extents")
            {
                view.SetViewport();
            }
            else if (p1.Result == ResultMode.Keyword && p1.Keyword == "Scale")
            {
                FloatOptions fopts = new FloatOptions("Scale factor: ");
                fopts.AllowNegative = false;
                fopts.AllowZero = false;
                fopts.AllowPositive = true;
                FloatResult f1 = await ed.GetFloat(fopts);
                if (f1.Result != ResultMode.OK) return;
                view.Zoom(1f / f1.Value);
            }
            else if (p1.Result == ResultMode.Keyword && p1.Keyword == "Object")
            {
                SelectionResult s1 = await ed.GetSelection("Select objects: ");
                if (s1.Result != ResultMode.OK || s1.Value.Count == 0) return;
                Extents2D ex = new Extents2D();
                foreach (Drawable item in s1.Value)
                {
                    ex.Add(item.GetExtents());
                }
                view.SetViewport(ex);
            }
            else
            {
                PointResult p2 = await ed.GetCorner("Opposite corner of zoom window: ", p1.Value);
                if (p2.Result != ResultMode.OK) return;
                view.SetViewport(p1.Value, p2.Value);
            }
        }
    }

    public class ViewPan : Command
    {
        public override string RegisteredName => "View.Pan";
        public override string Name => "Pan";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            CADView view = doc.ActiveView;
            if (view == null) return;

            Editor ed = doc.Editor;

            PointResult p1 = await ed.GetPoint("Base point: ");
            if (p1.Result == ResultMode.Cancel) return;
            DistanceResult p2 = await ed.GetDistance("Second point: ", p1.Value);
            if (p2.Result != ResultMode.OK) return;
            view.Pan(p2.Point - p1.Value);
        }
    }
}
