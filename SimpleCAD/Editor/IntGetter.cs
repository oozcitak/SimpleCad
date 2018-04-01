using SimpleCAD.Drawables;
using SimpleCAD.Geometry;

namespace SimpleCAD
{
    internal class IntGetter : EditorGetter<IntOptions, int>
    {
        protected override void AcceptCoordsInput(InputArgs<Point2D, int> args) =>
            args.InputValid = false;

        protected override void AcceptTextInput(InputArgs<string, int> args)
        {
            if (int.TryParse(args.Input, out args.Value))
            {
                if (!Options.AllowNegative && args.Value < 0)
                {
                    args.ErrorMessage = "*Negative numbers are not allowed*";
                    args.InputValid = false;
                }
                else if (!Options.AllowPositive && args.Value > 0)
                {
                    args.ErrorMessage = "*Positive numbers are not allowed*";
                    args.InputValid = false;
                }
                else if (!Options.AllowZero && args.Value == 0)
                {
                    args.ErrorMessage = "*Zero is not allowed*";
                    args.InputValid = false;
                }
            }
            else
            {
                args.InputValid = false;
            }
        }
    }
}
