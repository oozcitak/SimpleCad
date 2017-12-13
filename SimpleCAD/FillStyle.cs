using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SimpleCAD
{
    public struct FillStyle
    {
        public enum FillType
        {
            None = 0,
            Solid = 1,
            Hatch = 2
        }

        public FillType Type { get; set; }
        public Color Color { get; set; }
        public Color FillColor { get; set; }
        public HatchStyle HatchStyle { get; set; }

        public static FillStyle Default { get { return new FillStyle(Color.White); } }

        public FillStyle(Color color)
            : this()
        {
            Type = FillType.Solid;
            Color = color;
            FillColor = Color.Transparent;
            HatchStyle = HatchStyle.Percent50;
        }

        public FillStyle(Color color, Color fillColor, HatchStyle hatchStyle)
            : this()
        {
            Type = FillType.Hatch;
            Color = color;
            FillColor = fillColor;
            HatchStyle = hatchStyle;
        }

        public Brush CreateBrush(DrawParams param)
        {
            if (Type == FillType.Solid)
                return new SolidBrush(Color);
            else if (Type == FillType.Hatch)
                return new HatchBrush(HatchStyle, Color, FillColor);

            return new SolidBrush(Color.Transparent);
        }
    }
}
