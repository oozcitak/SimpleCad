using System;
using System.ComponentModel;

namespace SimpleCAD.Geometry
{
    public class Segment2DConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            string str = value as string;

            if (str == null) return false;

            string[] parts = str.Replace(" ", "").Split(';', ',');
            if (parts.Length != 4) return false;
            foreach (string part in parts)
            {
                if (!float.TryParse(part, out _))
                    return false;
            }

            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string str = value as string;

            if (str != null)
            {
                string[] parts = str.Replace(" ", "").Split(';', ',');
                if (parts.Length == 4)
                {
                    float x1 = float.Parse(parts[0]);
                    float y1 = float.Parse(parts[1]);
                    float x2 = float.Parse(parts[2]);
                    float y2 = float.Parse(parts[3]);
                    return new Segment2D(x1, y1, x2, y2);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Segment2D)
            {
                Segment2D s = (Segment2D)value;
                return s.X1.ToString("F2") + "; " + s.Y1.ToString("F2") + "; " + s.X2.ToString("F2") + "; " + s.Y2.ToString("F2");
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
