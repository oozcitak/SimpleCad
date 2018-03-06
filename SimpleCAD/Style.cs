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
    public partial struct Style : IPersistable
    {
        public Color Color { get; set; }
        public float LineWeight { get; set; }
        public DashStyle DashStyle { get; set; }

        internal static Style SelectionHighlightStyle { get { return new Style(Color.FromArgb(64, 46, 116, 251)); } }
        internal static Style SelectionWindowStyle { get { return new Style(Color.FromArgb(64, 46, 116, 251)); } }
        internal static Style SelectionBorderStyle { get { return new Style(Color.White, 1, DashStyle.Solid); } }
        internal static Style ReverseSelectionWindowStyle { get { return new Style(Color.FromArgb(64, 46, 251, 116)); } }
        internal static Style ReverseSelectionBorderStyle { get { return new Style(Color.White, 1, DashStyle.Dash); } }
        internal static Style JiggedStyle { get { return new Style(Color.Orange, 1, DashStyle.Dash); } }
        internal static Style CursorStyle { get { return new Style(Color.White, 1, DashStyle.Solid); } }
        internal static Style CursorPromptBackStyle { get { return new Style(Color.FromArgb(84, 58, 84)); } }
        internal static Style CursorPromptForeStyle { get { return new Style(Color.FromArgb(128, Color.White)); } }
        internal static Style ControlPointStyle { get { return new Style(Color.FromArgb(46, 116, 251)); } }

        public Style(Color color, float lineWeight, DashStyle dashStyle)
            : this()
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
            if (param.Mode == DrawParams.DrawingMode.Selection)
            {
                Pen pen = new Pen(SelectionHighlightStyle.Color, param.GetScaledLineWeight(LineWeight + 6));
                pen.DashStyle = DashStyle.Solid;
                return pen;
            }
            else if (param.Mode == DrawParams.DrawingMode.Jigged)
            {
                Style style = JiggedStyle;
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
