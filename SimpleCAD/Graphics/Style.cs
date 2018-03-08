using System;
using System.ComponentModel;
using System.IO;

namespace SimpleCAD.Graphics
{
    public enum DashStyle
    {
        Solid = 0,
        Dash = 1,
        Dot = 2,
        DashDot = 3,
        DashDotDot = 4,
    }

    [Serializable]
    [TypeConverter(typeof(StyleConverter))]
    public class Style : IPersistable
    {
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

        public Style(BinaryReader reader)
        {
            Color = new Color(reader.ReadUInt32());
            LineWeight = reader.ReadSingle();
            DashStyle = (DashStyle)reader.ReadInt32();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Color.Argb);
            writer.Write(LineWeight);
            writer.Write((int)DashStyle);
        }
    }
}
