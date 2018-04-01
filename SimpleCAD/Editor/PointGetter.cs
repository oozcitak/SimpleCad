using SimpleCAD.Drawables;
using SimpleCAD.Geometry;

namespace SimpleCAD
{
    internal class PointGetter : EditorGetter<PointOptions, Point2D>
    {
        protected override void Init(InitArgs<Point2D> args)
        {
            if (Options.HasBasePoint)
                Jigged = new Line(Options.BasePoint, Options.BasePoint);
        }

        protected override void CoordsChanged(Point2D pt)
        {
            SetCursorText(pt.ToString(Editor.Document.Settings.NumberFormat));

            if (Options.HasBasePoint)
                (Jigged as Line).EndPoint = pt;

            Options.Jig(pt);
        }

        protected override void AcceptCoordsInput(InputArgs<Point2D, Point2D> args) =>
            args.Value = args.Input;

        protected override void AcceptTextInput(InputArgs<string, Point2D> args) =>
            args.InputValid = Point2D.TryParse(args.Input, out args.Value);
    }
}
