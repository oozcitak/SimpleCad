using SimpleCAD.Geometry;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace SimpleCAD.Drawables
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
                using (Brush brush = new SolidBrush(Style.Color))
                {
                    param.Graphics.FillPolygon(brush, pts);
                }
                // The graphics engine overrides CreatePen to highlight the hatch boundary
                // during selection. Outside selection an invisible border is drawn.
                using (Pen pen = Style.Transparent.CreatePen(param))
                {
                    param.Graphics.DrawPolygon(pen, pts);
                }
            }
        }

        public Hatch(BinaryReader reader) : base(reader)
        {
            ;
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
        }
    }
}
