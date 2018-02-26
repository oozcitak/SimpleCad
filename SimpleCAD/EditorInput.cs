using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public partial class Editor
    {
        internal enum InputMode
        {
            None,
            Point
        }

        public class InputResult
        {
            public bool Success { get; private set; }

            public InputResult(bool success)
            {
                Success = success;
            }
        }

        public class PointResult : InputResult
        {
            public Point2D Location { get; private set; }

            public PointResult(bool success, Point2D location) : base(success)
            {
                Location = location;
            }
        }
    }
}
