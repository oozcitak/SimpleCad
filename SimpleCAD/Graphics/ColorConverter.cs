using System;
using System.ComponentModel;
using System.Globalization;

namespace SimpleCAD.Graphics
{
    public class ColorConverter : TypeConverter
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
            if (value is string str)
            {
                if (str.StartsWith("#"))
                    str = str.Substring(1);

                if (uint.TryParse(str, out uint _))
                    return true;

                if (Enum.TryParse(str, out KnownColor _))
                    return true;
            }

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string str)
            {
                if (str.StartsWith("#"))
                    str = str.Substring(1);

                if (uint.TryParse(str, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out uint argb))
                {
                    byte a = 0xff;
                    byte r = 0;
                    byte g = 0;
                    byte b = 0;
                    if (str.Length > 8) str = str.Substring(0, 8);
                    if (str.Length > 6)
                    {
                        a = byte.Parse(str.Substring(0, str.Length - 6), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                        r = byte.Parse(str.Substring(str.Length - 6, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                        g = byte.Parse(str.Substring(str.Length - 4, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                        b = byte.Parse(str.Substring(str.Length - 2, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                    }
                    else if (str.Length > 4)
                    {
                        r = byte.Parse(str.Substring(0, str.Length - 4), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                        g = byte.Parse(str.Substring(str.Length - 4, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                        b = byte.Parse(str.Substring(str.Length - 2, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                    }
                    else if (str.Length > 2)
                    {
                        r = byte.Parse(str.Substring(0, str.Length - 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                        g = byte.Parse(str.Substring(str.Length - 2, 1), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                        b = byte.Parse(str.Substring(str.Length - 1, 1), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                        str = str.Substring(str.Length - 2);
                    }
                    else
                    {
                        r = byte.Parse(str, NumberStyles.HexNumber, CultureInfo.CurrentCulture);
                    }
                    return Color.FromArgb(a, r, g, b);
                }

                if (Enum.TryParse(str, out KnownColor knownColor))
                    return Color.FromKnownColor(knownColor);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Color col)
            {
                if (col.IsKnownColor())
                    return col.ToKnownColor().ToString();
                else
                    return col.ToHex();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
