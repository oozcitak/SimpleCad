using SimpleCAD.Drawables;
using SimpleCAD.Geometry;

namespace SimpleCAD
{
    internal class TextGetter : EditorGetter<TextOptions, string>
    {
        public TextGetter()
        {
            SpaceAccepts = false;
        }

        protected override void TextChanged(string text)
        {
            SetCursorText(text);

            Options.Jig(text);
        }

        protected override void AcceptCoordsInput(InputArgs<Point2D, string> args) =>
            args.InputValid = false;

        protected override void AcceptTextInput(InputArgs<string, string> args) =>
            args.Value = args.Input;
    }
}
