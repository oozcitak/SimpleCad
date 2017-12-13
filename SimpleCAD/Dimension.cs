using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SimpleCAD
{
    public class Dimension : Drawable
    {
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }

        public float X1 { get { return P1.X; } }
        public float Y1 { get { return P1.Y; } }
        public float X2 { get { return P2.X; } }
        public float Y2 { get { return P2.Y; } }

        public float Offset { get; set; }
        public string String { get; set; }
        public string FontFamily { get; set; }
        public FontStyle FontStyle { get; set; }
        public float TextHeight { get; set; }

        public Dimension(Point2D p1, Point2D p2, string text, float textHeight)
        {
            P1 = p1;
            P2 = p2;

            Offset = 0.4f;
            TextHeight = textHeight;
            String = text;
            FontFamily = "Arial";
            FontStyle = FontStyle.Regular;
        }

        public Dimension(float x1, float y1, float x2, float y2, string text, float textHeight)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), text, textHeight)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            float tickSize = 0.5f * TextHeight;

            Vector2D dir = P2 - P1;
            float angle = dir.Angle;
            float len = dir.Length;

            Matrix2D trans = Matrix2D.Transformation(1, 1, angle, P1.X, P1.Y);

            // Dimension line
            Line dim = new Line(0, Offset, len, Offset);
            dim.OutlineStyle = OutlineStyle;
            dim.TransformBy(trans);
            dim.Draw(param);

            // Left tick
            Line tick1 = new Line(0, -tickSize + Offset, 0, tickSize + Offset);
            tick1.OutlineStyle = OutlineStyle;
            tick1.TransformBy(trans);
            tick1.Draw(param);

            // Right tick
            Line tick2 = new Line(len, -tickSize + Offset, len, tickSize + Offset);
            tick2.OutlineStyle = OutlineStyle;
            tick2.TransformBy(trans);
            tick2.Draw(param);

            // Text
            Text textObj = new Text(len / 2, Offset, String, TextHeight);
            textObj.FontFamily = FontFamily;
            textObj.FontStyle = FontStyle;
            textObj.HorizontalAlignment = StringAlignment.Center;
            textObj.VerticalAlignment = StringAlignment.Center;
            textObj.FillStyle = FillStyle;
            textObj.OutlineStyle = OutlineStyle;
            textObj.TransformBy(trans);
            textObj.Draw(param);
        }

        public override Extents GetExtents()
        {
            float offset = Math.Sign(Offset) * (0.5f * TextHeight + Math.Abs(Offset));

            Vector2D dir = P2 - P1;
            float angle = dir.Angle;
            float len = dir.Length;
            Point2D p1 = new Point2D(0, 0);
            Point2D p2 = new Point2D(len, 0);
            Point2D p3 = p1 + new Vector2D(0, offset);
            Point2D p4 = p2 + new Vector2D(0, offset);
            Matrix2D trans = Matrix2D.Transformation(1, 1, angle, P1.X, P1.Y);
            p1.TransformBy(trans);
            p2.TransformBy(trans);
            p3.TransformBy(trans);
            p4.TransformBy(trans);

            Extents extents = new Extents();
            extents.Add(p1);
            extents.Add(p2);
            extents.Add(p3);
            extents.Add(p4);
            return extents;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            Point2D p1 = P1;
            Point2D p2 = P2;
            p1.TransformBy(transformation);
            p2.TransformBy(transformation);
            P1 = p1;
            P2 = p2;
        }
    }
}
