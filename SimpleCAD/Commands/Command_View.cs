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
            Editor ed = doc.Editor;

            PointResult p1 = await ed.GetPoint("Corner of zoom window: ");
            if (p1.Result != ResultMode.OK) return;
            PointResult p2 = await ed.GetCorner("Opposite corner of zoom window: ", p1.Value);
            if (p2.Result != ResultMode.OK) return;

            CADView view = doc.ActiveView;
            if (view == null) return;

            view.SetViewport(p1.Value, p2.Value);
        }
    }
}
