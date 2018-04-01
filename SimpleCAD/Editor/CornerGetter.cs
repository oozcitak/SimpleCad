using SimpleCAD.Drawables;
using SimpleCAD.Geometry;

namespace SimpleCAD
{
    internal class CornerGetter : EditorGetter<CornerOptions, Point2D>
    {
        protected override void Init(InitArgs<Point2D> args)
        {
            Jigged = new Polygon(Options.BasePoint, Options.BasePoint, Options.BasePoint, Options.BasePoint);
        }

        protected override void CoordsChanged(Point2D pt)
        {
            SetCursorText(pt.ToString(Editor.Document.Settings.NumberFormat));

            Polygon poly = Jigged as Polygon;
            Point2D pc1 = poly.Points[0];
            Point2D pc2 = new Point2D(pt.X, pc1.Y);
            Point2D pc3 = pt;
            Point2D pc4 = new Point2D(pc1.X, pt.Y);
            poly.Points[0] = pc1;
            poly.Points[1] = pc2;
            poly.Points[2] = pc3;
            poly.Points[3] = pc4;

            Options.Jig(pt);
        }

        protected override void AcceptCoordsInput(InputArgs<Point2D, Point2D> args) =>
            args.Value = args.Input;

        protected override void AcceptTextInput(InputArgs<string, Point2D> args) =>
            args.InputValid = Point2D.TryParse(args.Input, out args.Value);
    }
}
