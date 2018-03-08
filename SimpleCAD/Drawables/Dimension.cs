using SimpleCAD.Geometry;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace SimpleCAD.Drawables
{
    public class Dimension : Drawable
    {
        private Point2D p1;
        private Point2D p2;

        public Point2D StartPoint { get => p1; set { p1 = value; NotifyPropertyChanged(); } }
        public Point2D EndPoint { get => p2; set { p2 = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X1 { get { return StartPoint.X; } }
        [Browsable(false)]
        public float Y1 { get { return StartPoint.Y; } }
        [Browsable(false)]
        public float X2 { get { return EndPoint.X; } }
        [Browsable(false)]
        public float Y2 { get { return EndPoint.Y; } }

        private float offset;
        private string str;
        private string fontFamily;
        private FontStyle fontStyle;
        private float textHeight;
        private float scale;
        private int precision;

        public float Offset { get => offset; set { offset = value; NotifyPropertyChanged(); } }
        public string String { get => str; set { str = value; NotifyPropertyChanged(); } }
        public string FontFamily { get => fontFamily; set { fontFamily = value; NotifyPropertyChanged(); } }
        public FontStyle FontStyle { get => fontStyle; set { fontStyle = value; NotifyPropertyChanged(); } }
        public float TextHeight { get => textHeight; set { textHeight = value; NotifyPropertyChanged(); } }
        public float Scale { get => scale; set { scale = value; NotifyPropertyChanged(); } }
        public int Precision { get => precision; set { precision = value; NotifyPropertyChanged(); } }

        public Dimension(Point2D p1, Point2D p2, float textHeight)
        {
            StartPoint = p1;
            EndPoint = p2;

            Offset = 0.4f;
            TextHeight = textHeight;
            String = "<>";
            FontFamily = "Arial";
            FontStyle = FontStyle.Regular;
            Scale = 1;
            Precision = 2;
        }

        public Dimension(float x1, float y1, float x2, float y2, float textHeight)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), textHeight)
        {
            ;
        }

        public override void Draw(Renderer renderer)
        {
            renderer.Draw(GetSubItems());
        }

        public override Extents2D GetExtents()
        {
            float offset = Math.Sign(Offset) * (0.5f * TextHeight + Math.Abs(Offset));

            Vector2D dir = EndPoint - StartPoint;
            float angle = dir.Angle;
            float len = dir.Length;
            Point2D p1 = new Point2D(0, 0);
            Point2D p2 = new Point2D(len, 0);
            Point2D p3 = p1 + new Vector2D(0, offset);
            Point2D p4 = p2 + new Vector2D(0, offset);
            TransformationMatrix2D trans = TransformationMatrix2D.Transformation(1, 1, angle, StartPoint.X, StartPoint.Y);
            p1 = p1.Transform(trans);
            p2 = p2.Transform(trans);
            p3 = p3.Transform(trans);
            p4 = p4.Transform(trans);

            Extents2D extents = new Extents2D();
            extents.Add(p1);
            extents.Add(p2);
            extents.Add(p3);
            extents.Add(p4);
            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            StartPoint = StartPoint.Transform(transformation);
            EndPoint = EndPoint.Transform(transformation);
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return GetSubItems().Contains(pt, pickBoxSize);
        }

        private Composite GetSubItems()
        {
            Composite items = new Composite();

            float tickSize = 0.5f * TextHeight;

            Vector2D dir = EndPoint - StartPoint;
            float angle = dir.Angle;
            float len = dir.Length;

            // Dimension line
            Line dim = new Line(0, Offset, len, Offset);
            dim.Style = Style;
            items.Add(dim);

            // Left tick
            Line tick1 = new Line(0, -tickSize + Offset, 0, tickSize + Offset);
            tick1.Style = Style;
            items.Add(tick1);

            // Right tick
            Line tick2 = new Line(len, -tickSize + Offset, len, tickSize + Offset);
            tick2.Style = Style;
            items.Add(tick2);

            // Text
            float dist = (StartPoint - EndPoint).Length * Scale;
            string txt = String.Replace("<>", dist.ToString("F" + Precision.ToString()));
            Text textObj = new Text(len / 2, Offset, txt, TextHeight);
            textObj.FontFamily = FontFamily;
            textObj.FontStyle = FontStyle;
            textObj.HorizontalAlignment = TextHorizontalAlignment.Center;
            textObj.VerticalAlignment = TextVerticalAlignment.Middle;
            textObj.Style = Style;
            items.Add(textObj);

            TransformationMatrix2D trans = TransformationMatrix2D.Transformation(1, 1, angle, StartPoint.X, StartPoint.Y);
            items.TransformBy(trans);

            return items;
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("StartPoint"),
                new ControlPoint("EndPoint"),
            };
        }

        public Dimension(BinaryReader reader) : base(reader)
        {
            StartPoint = new Point2D(reader);
            EndPoint = new Point2D(reader);
            TextHeight = reader.ReadSingle();
            Offset = reader.ReadSingle();
            String = reader.ReadString();
            FontFamily = reader.ReadString();
            FontStyle = (FontStyle)reader.ReadInt32();
            Scale = reader.ReadSingle();
            Precision = reader.ReadInt32();
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            StartPoint.Save(writer);
            EndPoint.Save(writer);
            writer.Write(TextHeight);
            writer.Write(Offset);
            writer.Write(String);
            writer.Write(FontFamily);
            writer.Write((int)FontStyle);
            writer.Write(Scale);
            writer.Write(Precision);
        }
    }
}
