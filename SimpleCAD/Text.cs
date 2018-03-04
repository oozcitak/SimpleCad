using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SimpleCAD
{
    public class Text : Drawable
    {
        private Point2D p;

        public Point2D Location { get => p; set { p = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return Location.X; } }
        [Browsable(false)]
        public float Y { get { return Location.Y; } }

        private string str;
        private string fontFamily;
        private FontStyle fontStyle;
        private float textHeight;
        private float rotation;
        private StringAlignment horizontalAlignment;
        private StringAlignment verticalAlignment;

        public string String { get => str; set { str = value; NotifyPropertyChanged(); } }
        public string FontFamily { get => fontFamily; set { fontFamily = value; NotifyPropertyChanged(); } }
        public FontStyle FontStyle { get => fontStyle; set { fontStyle = value; NotifyPropertyChanged(); } }
        public float Height { get => textHeight; set { textHeight = value; NotifyPropertyChanged(); } }
        public float Width { get; private set; }
        public float Rotation { get => rotation; set { rotation = value; NotifyPropertyChanged(); } }
        public StringAlignment HorizontalAlignment { get => horizontalAlignment; set { horizontalAlignment = value; NotifyPropertyChanged(); } }
        public StringAlignment VerticalAlignment { get => verticalAlignment; set { verticalAlignment = value; NotifyPropertyChanged(); } }

        public Text(Point2D p, string text, float height)
        {
            Location = p;
            Height = height;
            Width = height;
            String = text;
            Rotation = 0;
            HorizontalAlignment = StringAlignment.Near;
            VerticalAlignment = StringAlignment.Near;
            FontFamily = "Arial";
            FontStyle = FontStyle.Regular;
        }

        public Text(float x, float y, string text, float height)
            : this(new Point2D(x, y), text, height)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            float height = param.ModelToView(Height);
            using (Pen pen = Outline.CreatePen(param))
            using (Brush brush = new SolidBrush(pen.Color))
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
                float dx = 0;
                float dy = 0;
                SizeF sz = param.Graphics.MeasureString(String, font);
                Width = param.ViewToModel(sz.Width);
                if (HorizontalAlignment == StringAlignment.Far)
                    dx = -sz.Width;
                else if (HorizontalAlignment == StringAlignment.Center)
                    dx = -sz.Width / 2;
                if (VerticalAlignment == StringAlignment.Near)
                    dy = -sz.Height;
                else if (VerticalAlignment == StringAlignment.Center)
                    dy = -sz.Height / 2;

                param.Graphics.TranslateTransform(dx, dy, MatrixOrder.Append);
                param.Graphics.RotateTransform(-Rotation * 180 / MathF.PI, MatrixOrder.Append);
                param.Graphics.TranslateTransform(x, y, MatrixOrder.Append);

                param.Graphics.DrawString(String, font, brush, 0, 0);

                // Restore old transformation
                param.Graphics.Transform = oldMatrix;
            }
        }

        public override Extents2D GetExtents()
        {
            float angle = Rotation;
            float thHeight = Height;
            float thWidth = Width;
            Point2D p1 = new Point2D(0, 0);
            Point2D p2 = new Point2D(thWidth, 0);
            Point2D p3 = new Point2D(0, thHeight);
            Point2D p4 = new Point2D(thWidth, thHeight);
            float dx = 0;
            float dy = 0;
            if (HorizontalAlignment == StringAlignment.Far)
                dx = -thWidth;
            else if (HorizontalAlignment == StringAlignment.Center)
                dx = -thWidth / 2;
            if (VerticalAlignment == StringAlignment.Far)
                dy = -thHeight;
            else if (VerticalAlignment == StringAlignment.Center)
                dy = -thHeight / 2;
            Vector2D offset = new Vector2D(dx, dy);
            p1 = p1 + offset;
            p2 = p2 + offset;
            p3 = p3 + offset;
            p4 = p4 + offset;
            TransformationMatrix2D trans = TransformationMatrix2D.Transformation(1, 1, angle, Location.X, Location.Y);
            p1.TransformBy(trans);
            p2.TransformBy(trans);
            p3.TransformBy(trans);
            p4.TransformBy(trans);

            Extents2D extents = new Extents2D();
            extents.Add(p1);
            extents.Add(p2);
            extents.Add(p3);
            extents.Add(p4);
            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Point2D p = Location;
            p.TransformBy(transformation);
            Location = p;

            Vector2D dir = Vector2D.XAxis * Height;
            dir.TransformBy(transformation);
            Height = dir.Length;

            Rotation += transformation.RotationAngle;
        }
    }
}
