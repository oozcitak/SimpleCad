using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD.Drawables
{
    public abstract class Curve : Drawable
    {
        public const int MinCurveSegments = 4;
        public const int MaxCurveSegments = 200;
    }
}
