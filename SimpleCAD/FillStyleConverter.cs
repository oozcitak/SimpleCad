using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SimpleCAD
{
    public class FillStyleConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(FillStyle);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string str = value as string;

            if (str != null)
            {
                string[] parts = str.Replace(" ", "").Split(';');
                if (parts.Length == 1)
                {
                    Color col = Color.FromName(parts[0]);
                    return new FillStyle(col);
                }
                else if (parts.Length == 3)
                {
                    Color col = Color.FromName(parts[0]);
                    Color fillcol = Color.FromName(parts[1]);
                    HatchStyle hs = (HatchStyle)Enum.Parse(typeof(HatchStyle), parts[2]);
                    return new FillStyle(col, fillcol, hs);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is FillStyle)
            {
                FillStyle fs = (FillStyle)value;
                if (fs.Type == FillStyle.FillType.Solid)
                    return fs.Color.Name;
                else
                    return fs.Color.Name + "; " + fs.FillColor.Name + "; " + fs.HatchStyle.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
