using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class Hatch : Polyline
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Closed { get => true; set => throw new InvalidOperationException("Hatch must be a closed area."); }

        public Hatch() : base()
        {
            ;
        }

        public Hatch(Point2DCollection pts) : base(pts)
        {
            ;
        }

        public Hatch(params Point2D[] pts) : base(pts)
        {
            ;
        }

        public Hatch(PointF[] pts) : base(pts)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            if (Points.Count > 2)
            {
                PointF[] pts = Points.ToPointF();
                using (Brush brush = new SolidBrush(Outline.Color))
                {
                    param.Graphics.FillPolygon(brush, pts);
                }
                // The graphics engine overrides CreatePen to highlight the hatch boundary
                // during selection. Normally a transparent boundary is drawn.s
                using (Pen pen = Outline.Transparent.CreatePen(param))
                {
                    param.Graphics.DrawPolygon(pen, pts);
                }
            }
        }
    }
}
