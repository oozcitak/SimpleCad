using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class Dimension : Drawable
    {
        private Lazy<TextStyle> textStyleRef = new Lazy<TextStyle>(() => TextStyle.Default);
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
        private float textHeight;
        private float scale;
        private int precision;

        public float Offset { get => offset; set { offset = value; NotifyPropertyChanged(); } }
        public string String { get => str; set { str = value; NotifyPropertyChanged(); } }
        public TextStyle TextStyle { get => textStyleRef.Value; set => textStyleRef = new Lazy<TextStyle>(() => value); }
        public float TextHeight { get => textHeight; set { textHeight = value; NotifyPropertyChanged(); } }
        public float Scale { get => scale; set { scale = value; NotifyPropertyChanged(); } }
        public int Precision { get => precision; set { precision = value; NotifyPropertyChanged(); } }

        public Dimension() { }

        public Dimension(Point2D p1, Point2D p2, float textHeight)
        {
            StartPoint = p1;
            EndPoint = p2;

            Offset = 0.4f;
            TextHeight = textHeight;
            String = "<>";
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
            Matrix2D trans = Matrix2D.Transformation(1, 1, angle, StartPoint.X, StartPoint.Y);
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

        public override void TransformBy(Matrix2D transformation)
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
            dim.Style = Style.ApplyLayer(Layer);
            items.Add(dim);

            // Left tick
            Line tick1 = new Line(0, -tickSize + Offset, 0, tickSize + Offset);
            tick1.Style = Style.ApplyLayer(Layer);
            items.Add(tick1);

            // Right tick
            Line tick2 = new Line(len, -tickSize + Offset, len, tickSize + Offset);
            tick2.Style = Style.ApplyLayer(Layer);
            items.Add(tick2);

            // Text
            float dist = (StartPoint - EndPoint).Length * Scale;
            string txt = String.Replace("<>", dist.ToString("F" + Precision.ToString()));
            Text textObj = new Text(len / 2, Offset, txt, TextHeight);
            textObj.TextStyle = TextStyle;
            textObj.HorizontalAlignment = TextHorizontalAlignment.Center;
            textObj.VerticalAlignment = TextVerticalAlignment.Middle;
            textObj.Style = Style.ApplyLayer(Layer);
            items.Add(textObj);

            Matrix2D trans = Matrix2D.Transformation(1, 1, angle, StartPoint.X, StartPoint.Y);
            items.TransformBy(trans);

            return items;
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Start point", StartPoint),
                new ControlPoint("End point", EndPoint),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            return new[]
            {
                new SnapPoint("Start point", SnapPointType.Point, StartPoint),
                new SnapPoint("End point", SnapPointType.Point, EndPoint),
            };
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                if (index == 0)
                    StartPoint = StartPoint.Transform(transformation);
                else if (index == 1)
                    EndPoint = EndPoint.Transform(transformation);
            }
        }

        public override void Load(DocumentReader reader)
        {
            var doc = reader.Document;
            base.Load(reader);
            StartPoint = reader.ReadPoint2D();
            EndPoint = reader.ReadPoint2D();
            TextHeight = reader.ReadFloat();
            Offset = reader.ReadFloat();
            String = reader.ReadString();
            string textStyleName= reader.ReadString();
            textStyleRef = new Lazy<TextStyle>(() => doc.TextStyles[textStyleName]);
            Scale = reader.ReadFloat();
            Precision = reader.ReadInt();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(StartPoint);
            writer.Write(EndPoint);
            writer.Write(TextHeight);
            writer.Write(Offset);
            writer.Write(String);
            writer.Write(TextStyle.Name);
            writer.Write(Scale);
            writer.Write(Precision);
        }
    }
}
