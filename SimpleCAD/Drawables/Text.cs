using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class Text : Drawable
    {
        private Point2D location;

        public Point2D Location { get => location; set { location = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return Location.X; } }
        [Browsable(false)]
        public float Y { get { return Location.Y; } }

        private string str;
        private float textHeight;
        private float rotation;
        private TextHorizontalAlignment horizontalAlignment;
        private TextVerticalAlignment verticalAlignment;

        public TextStyle TextStyle { get; set; } = TextStyle.Default;
        public string String { get => str; set { str = value; NotifyPropertyChanged(); } }
        public float TextHeight { get => textHeight; set { textHeight = value; NotifyPropertyChanged(); } }
        public float Width { get; private set; }
        public float Rotation { get => rotation; set { rotation = value; NotifyPropertyChanged(); } }
        public TextHorizontalAlignment HorizontalAlignment { get => horizontalAlignment; set { horizontalAlignment = value; NotifyPropertyChanged(); } }
        public TextVerticalAlignment VerticalAlignment { get => verticalAlignment; set { verticalAlignment = value; NotifyPropertyChanged(); } }

        private float cpSize = 0;

        public Text() { }

        public Text(Point2D loc, string text, float height)
        {
            Location = loc;
            TextHeight = height;
            Width = height;
            String = text;
            Rotation = 0;
            HorizontalAlignment = TextHorizontalAlignment.Left;
            VerticalAlignment = TextVerticalAlignment.Bottom;
        }

        public Text(float x, float y, string text, float height)
            : this(new Point2D(x, y), text, height)
        {
            ;
        }

        public override void Draw(Renderer renderer)
        {
            cpSize = 2 * renderer.View.ScreenToWorld(new Vector2D(renderer.View.Document.Settings.Get<int>("ControlPointSize"), 0)).X;
            Width = renderer.MeasureString(String, TextStyle, TextHeight).X;
            renderer.DrawString(Style.ApplyLayer(Layer), Location, String, TextStyle, TextHeight, Rotation, HorizontalAlignment, VerticalAlignment);
        }

        public override Extents2D GetExtents()
        {
            float angle = Rotation;
            float thHeight = TextHeight;
            float thWidth = Width;
            Point2D p1 = new Point2D(0, 0);
            Point2D p2 = new Point2D(thWidth, 0);
            Point2D p3 = new Point2D(0, thHeight);
            Point2D p4 = new Point2D(thWidth, thHeight);
            float dx = 0;
            float dy = 0;
            if (HorizontalAlignment == TextHorizontalAlignment.Right)
                dx = -thWidth;
            else if (HorizontalAlignment == TextHorizontalAlignment.Center)
                dx = -thWidth / 2;
            if (VerticalAlignment == TextVerticalAlignment.Top)
                dy = -thHeight;
            else if (VerticalAlignment == TextVerticalAlignment.Middle)
                dy = -thHeight / 2;
            Vector2D offset = new Vector2D(dx, dy);
            p1 = p1 + offset;
            p2 = p2 + offset;
            p3 = p3 + offset;
            p4 = p4 + offset;
            Matrix2D trans = Matrix2D.Transformation(1, 1, angle, Location.X, Location.Y);
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
            Location = Location.Transform(transformation);
            TextHeight = (Vector2D.XAxis * TextHeight).Transform(transformation).Length;
            Rotation += transformation.RotationAngle;
        }

        public override ControlPoint[] GetControlPoints()
        {
            Vector2D upDir = Vector2D.FromAngle(Rotation).Perpendicular;
            return new[]
            {
                new ControlPoint("Location", Location),
                new ControlPoint("Rotation", ControlPoint.ControlPointType.Angle, Location, Location + System.Math.Max(Width, cpSize) * Vector2D.FromAngle(Rotation)),
                new ControlPoint("Text height", ControlPoint.ControlPointType.Distance, Location, Location + TextHeight * upDir),
            };
        }

        public override void TransformControlPoint(int index, Matrix2D transformation)
        {
            if (index == 0)
                Location = Location.Transform(transformation);
            else if (index == 1)
                Rotation = Vector2D.FromAngle(Rotation).Transform(transformation).Angle;
            else if (index == 2)
                TextHeight = Vector2D.XAxis.Transform(transformation).Length * TextHeight;
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            Location = reader.ReadPoint2D();
            TextHeight = reader.ReadFloat();
            String = reader.ReadString();
            TextStyle = reader.ReadPersistable<TextStyle>();
            Rotation = reader.ReadFloat();
            HorizontalAlignment = (TextHorizontalAlignment)reader.ReadInt();
            VerticalAlignment = (TextVerticalAlignment)reader.ReadInt();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Location);
            writer.Write(TextHeight);
            writer.Write(String);
            writer.Write(TextStyle);
            writer.Write(Rotation);
            writer.Write((int)HorizontalAlignment);
            writer.Write((int)VerticalAlignment);
        }
    }
}
