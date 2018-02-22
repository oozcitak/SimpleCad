using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SimpleCAD
{
    public class OutlineStyleConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(OutlineStyle);
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
                    return new OutlineStyle(col);
                }
                else if (parts.Length == 2)
                {
                    Color col = Color.FromName(parts[0]);
                    float lw = float.Parse(parts[1]);
                    return new OutlineStyle(col, lw);
                }
                else if (parts.Length == 3)
                {
                    Color col = Color.FromName(parts[0]);
                    float lw = float.Parse(parts[1]);
                    DashStyle ds = (DashStyle)Enum.Parse(typeof(DashStyle), parts[2]);
                    return new OutlineStyle(col, lw, ds);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is OutlineStyle)
            {
                OutlineStyle os = (OutlineStyle)value;
                if (os.DashStyle == DashStyle.Solid && os.LineWeight == 0)
                    return os.Color.Name;
                else if (os.DashStyle == DashStyle.Solid)
                    return os.Color.Name + "; " + os.LineWeight.ToString("F2");
                else
                    return os.Color.Name + "; " + os.LineWeight.ToString("F2") + "; " + os.DashStyle.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
