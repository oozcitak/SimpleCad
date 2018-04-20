using System;
using System.ComponentModel;

namespace SimpleCAD.Graphics
{
    public enum DashStyle
    {
        ByLayer = -1,
        Solid = 0,
        Dash = 1,
        Dot = 2,
        DashDot = 3,
        DashDotDot = 4,
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Style : IPersistable
    {
        public static Style Default => new Style(Color.ByLayer, ByLayer, DashStyle.ByLayer);

        public const float ByLayer = -1;

        public Color Color { get; set; }
        public float LineWeight { get; set; }
        public DashStyle DashStyle { get; set; }
        public bool Fill { get; set; }

        public Style(Color color, float lineWeight, DashStyle dashStyle)
        {
            Color = color;
            LineWeight = lineWeight;
            DashStyle = dashStyle;
        }

        public Style(Color color, float lineWeight)
            : this(color, lineWeight, DashStyle.Solid)
        {
            ;
        }

        public Style(Color color)
            : this(color, 0, DashStyle.Solid)
        {
            ;
        }

        public Style()
            : this(Color.ByLayer, ByLayer, DashStyle.ByLayer)
        {
            ;
        }

        public Style ApplyLayer(Layer layer)
        {
            Style style = new Style(Color, LineWeight, DashStyle);
            style.Fill = Fill;
            if (layer != null)
            {
                if (Color.IsByLayer) style.Color = layer.Style.Color;
                if (LineWeight == ByLayer) style.LineWeight = layer.Style.LineWeight;
                if (DashStyle == DashStyle.ByLayer) style.DashStyle = layer.Style.DashStyle;
            }
            return style;
        }

        public void Load(DocumentReader reader)
        {
            Color = reader.ReadColor();
            LineWeight = reader.ReadFloat();
            DashStyle = (DashStyle)reader.ReadInt();
            Fill = reader.ReadBoolean();
        }

        public void Save(DocumentWriter writer)
        {
            writer.Write(Color.Argb);
            writer.Write(LineWeight);
            writer.Write((int)DashStyle);
            writer.Write(Fill);
        }
    }
}
