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
    [TypeConverter(typeof(OutlineConverter))]
    public partial struct Outline : IPersistable
    {
        public Color Color { get; set; }
        public float LineWeight { get; set; }
        public DashStyle DashStyle { get; set; }

        internal static Outline SelectionHighlightStyle { get { return new Outline(Color.FromArgb(64, 46, 116, 251)); } }
        internal static Outline SelectionWindowStyle { get { return new Outline(Color.FromArgb(64, 46, 116, 251)); } }
        internal static Outline SelectionBorderStyle { get { return new Outline(Color.White, 1, DashStyle.Solid); } }
        internal static Outline ReverseSelectionWindowStyle { get { return new Outline(Color.FromArgb(64, 46, 251, 116)); } }
        internal static Outline ReverseSelectionBorderStyle { get { return new Outline(Color.White, 1, DashStyle.Dash); } }
        internal static Outline JiggedStyle { get { return new Outline(Color.Orange, 1, DashStyle.Dash); } }
        internal static Outline CursorStyle { get { return new Outline(Color.White, 1, DashStyle.Solid); } }
        internal static Outline ControlPointStyle { get { return new Outline(Color.FromArgb(46, 116, 251)); } }

        public Outline(Color color, float lineWeight, DashStyle dashStyle)
            : this()
        {
            Color = color;
            LineWeight = lineWeight;
            DashStyle = dashStyle;
        }

        public Outline(Color color, float lineWeight)
            : this(color, lineWeight, DashStyle.Solid)
        {
            ;
        }

        public Outline(Color color)
            : this(color, 0, DashStyle.Solid)
        {
            ;
        }

        public Pen CreatePen(DrawParams param)
        {
            if (param.Mode == DrawParams.DrawingMode.Selection)
            {
                Pen pen = new Pen(SelectionHighlightStyle.Color, param.GetScaledLineWeight(LineWeight + 6));
                pen.DashStyle = DashStyle.Solid;
                return pen;
            }
            else if (param.Mode == DrawParams.DrawingMode.Jigged)
            {
                Outline style = JiggedStyle;
                Pen pen = new Pen(style.Color, param.GetScaledLineWeight(style.LineWeight));
                pen.DashStyle = style.DashStyle;
                return pen;
            }
            else // (param.Mode == DrawParams.DrawingMode.Normal)
            {
                Pen pen = new Pen(Color, param.GetScaledLineWeight(LineWeight));
                pen.DashStyle = DashStyle;
                return pen;
            }
        }

        public Outline(BinaryReader reader)
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
