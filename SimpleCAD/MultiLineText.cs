using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SimpleCAD
{
    public class MultiLineText : Drawable
    {
        private Point2D p;

        public Point2D P { get => p; set { p = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return P.X; } }
        [Browsable(false)]
        public float Y { get { return P.Y; } }

        private string[] lines;
        private string fontFamily;
        private FontStyle fontStyle;
        private float textHeight;
        private float rotation;

        public string[] Lines { get => lines; set { lines = value; NotifyPropertyChanged(); } }
        public string FontFamily { get => fontFamily; set { fontFamily = value; NotifyPropertyChanged(); } }
        public FontStyle FontStyle { get => fontStyle; set { fontStyle = value; NotifyPropertyChanged(); } }
        public float Height { get => textHeight; set { textHeight = value; NotifyPropertyChanged(); } }
        public float Width { get; private set; }
        public float Rotation { get => rotation; set { rotation = value; NotifyPropertyChanged(); } }

        public MultiLineText(Point2D p, string[] lines, float height)
        {
            P = p;
            Height = height;
            Width = height;
            Lines = lines;
            Rotation = 0;
            FontFamily = "Arial";
            FontStyle = FontStyle.Regular;
        }

        public MultiLineText(float x, float y, string[] lines, float height)
            : this(new Point2D(x, y), lines, height)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            float height = param.ModelToView(Height);
            using (Pen pen = OutlineStyle.CreatePen(param))
            using (Brush brush = new SolidBrush(pen.Color))
            using (Brush back = new SolidBrush(FillStyle.Color))
            using (Font font = new Font(FontFamily, height, FontStyle, GraphicsUnit.Pixel))
            {
                // Convert the text alignment point (x, y) to pixel coordinates
                PointF[] pt = new PointF[] { new PointF(X, Y) };
                param.Graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, pt);
                float x = pt[0].X;
                float y = pt[0].Y;

                // Revert transformation to identity while drawing text
                Matrix oldMatrix = param.Graphics.Transform;
                param.Graphics.ResetTransform();

                // Calculate alignment in pixel coordinates
                float thWidth = 0;
                foreach (string line in Lines)
                {
                    thWidth = Math.Max(thWidth, param.Graphics.MeasureString(Lines[0], font).Width);
                }
                Width = param.ViewToModel(thWidth);
                float dy = param.Graphics.MeasureString(Lines[0], font).Height;
                float thHeight = dy * Lines.Length;

                param.Graphics.RotateTransform(-Rotation, MatrixOrder.Append);
                param.Graphics.TranslateTransform(x, y, MatrixOrder.Append);

                // Fill background
                param.Graphics.FillRectangle(back, 0, 0, thWidth, thHeight);
                float yy = 0;
                foreach (string line in Lines)
                {
                    param.Graphics.DrawString(line, font, brush, 0, yy);
                    yy += dy;
                }

                // Restore old transformation
                param.Graphics.Transform = oldMatrix;
            }
        }

        public override Extents GetExtents()
        {
            float thHeight = Height * Lines.Length * 1.425f;
            float thWidth = Width;
            float angle = Rotation / 180 * MathF.PI;
            Point2D p1 = new Point2D(0, 0);
            Point2D p2 = new Point2D(thWidth, 0);
            Point2D p3 = new Point2D(0, -thHeight);
            Point2D p4 = new Point2D(thWidth, -thHeight);
            TransformationMatrix2D trans = TransformationMatrix2D.Transformation(1, 1, angle, P.X, P.Y);
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

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Point2D p = P;
            p.TransformBy(transformation);
            P = p;

            Vector2D dir = Vector2D.XAxis * Height;
            dir.TransformBy(transformation);
            Height = dir.Length;

            Rotation += transformation.RotationAngle * 180 / MathF.PI;
        }
    }
}
