using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class Line : Drawable
    {
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }

        [Browsable(false)]
        public float X1 { get { return P1.X; } }
        [Browsable(false)]
        public float Y1 { get { return P1.Y; } }
        [Browsable(false)]
        public float X2 { get { return P2.X; } }
        [Browsable(false)]
        public float Y2 { get { return P2.Y; } }

        public Line(Point2D p1, Point2D p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public Line(float x1, float y1, float x2, float y2)
            : this(new Point2D(x1, y1), new Point2D(x2, y2))
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawLine(pen, X1, Y1, X2, Y2);
            }
        }

        public override Extents GetExtents()
        {
            Extents extents = new Extents();
            extents.Add(X1, Y1);
            extents.Add(X2, Y2);
            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Point2D p1 = P1;
            Point2D p2 = P2;
            p1.TransformBy(transformation);
            p2.TransformBy(transformation);
            P1 = p1;
            P2 = p2;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            Vector2D w = pt - P1;
            Vector2D vL = (P2 - P1);
            float b = w.DotProduct(vL) / vL.DotProduct(vL);
            float dist = (w - b * vL).Length;
            return b >= 0 && b <= 1 && dist <= pickBoxSize / 2;
        }
    }
}
