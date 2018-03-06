using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SimpleCAD
{
    [Serializable]
    [TypeConverter(typeof(StyleConverter))]
    public partial class Style : IPersistable
    {
        public Color Color { get; set; }
        public float LineWeight { get; set; }
        public DashStyle DashStyle { get; set; }

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

        public Pen CreatePen(DrawParams param)
        {
            if (param.StyleOverride != null)
            {
                Pen pen = new Pen(param.StyleOverride.Color, param.GetScaledLineWeight(param.StyleOverride.LineWeight));
                pen.DashStyle = param.StyleOverride.DashStyle;
                return pen;
            }
            else
            {
                Pen pen = new Pen(Color, param.GetScaledLineWeight(LineWeight));
                pen.DashStyle = DashStyle;
                return pen;
            }
        }

        public Style(BinaryReader reader)
        {
            Color = Color.FromArgb(reader.ReadInt32());
            LineWeight = reader.ReadSingle();
            DashStyle = (DashStyle)reader.ReadInt32();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Color.ToArgb());
            writer.Write(LineWeight);
            writer.Write((int)DashStyle);
        }
    }
}
