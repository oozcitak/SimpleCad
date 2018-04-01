using SimpleCAD.Drawables;
using SimpleCAD.Geometry;

namespace SimpleCAD
{
    internal class AngleGetter : EditorGetter<AngleOptions, float>
    {
        protected override void Init(InitArgs<float> args)
        {
            Jigged = new Line(Options.BasePoint, Options.BasePoint);
        }

        protected override void CoordsChanged(Point2D pt)
        {
            float angle = (pt - Options.BasePoint).Angle;
            SetCursorText(angle.ToString(Editor.Document.Settings.NumberFormat));

            (Jigged as Line).EndPoint = pt;

            Options.Jig(angle);
        }

        protected override void AcceptCoordsInput(InputArgs<Point2D, float> args) =>
            args.Value = (args.Input - Options.BasePoint).Angle;

        protected override void AcceptTextInput(InputArgs<string, float> args)
        {
            if (Vector2D.TryParse(args.Input, out Vector2D vec))
            {
                args.Value = vec.Angle;
            }
            else if (float.TryParse(args.Input, out args.Value))
            {
                ;
            }
            else
            {
                args.InputValid = false;
            }
        }
    }
}
