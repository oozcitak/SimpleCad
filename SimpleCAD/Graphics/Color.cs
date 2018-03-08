using System.Collections.Generic;
using System.Globalization;

namespace SimpleCAD.Graphics
{
    #region KnownColor Enum
    public enum KnownColor
    {
        ByLayer,
        Transparent,
        AliceBlue,
        AntiqueWhite,
        Aqua,
        Aquamarine,
        Azure,
        Beige,
        Bisque,
        Black,
        BlanchedAlmond,
        Blue,
        BlueViolet,
        Brown,
        BurlyWood,
        CadetBlue,
        Chartreuse,
        Chocolate,
        Coral,
        CornflowerBlue,
        Cornsilk,
        Crimson,
        Cyan,
        DarkBlue,
        DarkCyan,
        DarkGoldenrod,
        DarkGray,
        DarkGreen,
        DarkKhaki,
        DarkMagenta,
        DarkOliveGreen,
        DarkOrange,
        DarkOrchid,
        DarkRed,
        DarkSalmon,
        DarkSeaGreen,
        DarkSlateBlue,
        DarkSlateGray,
        DarkTurquoise,
        DarkViolet,
        DeepPink,
        DeepSkyBlue,
        DimGray,
        DodgerBlue,
        Firebrick,
        FloralWhite,
        ForestGreen,
        Fuchsia,
        Gainsboro,
        GhostWhite,
        Gold,
        Goldenrod,
        Gray,
        Green,
        GreenYellow,
        Honeydew,
        HotPink,
        IndianRed,
        Indigo,
        Ivory,
        Khaki,
        Lavender,
        LavenderBlush,
        LawnGreen,
        LemonChiffon,
        LightBlue,
        LightCoral,
        LightCyan,
        LightGoldenrodYellow,
        LightGray,
        LightGreen,
        LightPink,
        LightSalmon,
        LightSeaGreen,
        LightSkyBlue,
        LightSlateGray,
        LightSteelBlue,
        LightYellow,
        Lime,
        LimeGreen,
        Linen,
        Magenta,
        Maroon,
        MediumAquamarine,
        MediumBlue,
        MediumOrchid,
        MediumPurple,
        MediumSeaGreen,
        MediumSlateBlue,
        MediumSpringGreen,
        MediumTurquoise,
        MediumVioletRed,
        MidnightBlue,
        MintCream,
        MistyRose,
        Moccasin,
        NavajoWhite,
        Navy,
        OldLace,
        Olive,
        OliveDrab,
        Orange,
        OrangeRed,
        Orchid,
        PaleGoldenrod,
        PaleGreen,
        PaleTurquoise,
        PaleVioletRed,
        PapayaWhip,
        PeachPuff,
        Peru,
        Pink,
        Plum,
        PowderBlue,
        Purple,
        Red,
        RosyBrown,
        RoyalBlue,
        SaddleBrown,
        Salmon,
        SandyBrown,
        SeaGreen,
        SeaShell,
        Sienna,
        Silver,
        SkyBlue,
        SlateBlue,
        SlateGray,
        Snow,
        SpringGreen,
        SteelBlue,
        Tan,
        Teal,
        Thistle,
        Tomato,
        Turquoise,
        Violet,
        Wheat,
        White,
        WhiteSmoke,
        Yellow,
        YellowGreen,
    }
    #endregion

    public struct Color
    {
        #region Properties
        private readonly bool _byLayer;

        private readonly uint _argb;
        private readonly byte _a;
        private readonly byte _r;
        private readonly byte _g;
        private readonly byte _b;

        private bool IsByLayer { get { return _byLayer; } }

        public uint Argb { get { return _argb; } }
        public byte A { get { return _a; } }
        public byte R { get { return _r; } }
        public byte G { get { return _g; } }
        public byte B { get { return _b; } }
        #endregion

        #region Constructors
        private Color(bool byLayer)
        {
            _byLayer = byLayer;
            _argb = 0;
            _a = _r = _g = _b = 0;
        }

        private Color(KnownColor colorName)
        {
            if (colorName == KnownColor.ByLayer)
            {
                _byLayer = true;
                _argb = 0;
                _a = _r = _g = _b = 0;
            }
            else
            {
                _byLayer = false;
                _argb = knownColorLookup[colorName];
                _a = (byte)((_argb >> 24) & 255);
                _r = (byte)((_argb >> 16) & 255);
                _g = (byte)((_argb >> 8) & 255);
                _b = (byte)(_argb & 255);
            }
        }

        public Color(uint color) : this(false)
        {
            _argb = color;
            _a = (byte)((color >> 24) & 255);
            _r = (byte)((color >> 16) & 255);
            _g = (byte)((color >> 8) & 255);
            _b = (byte)(color & 255);
        }

        public Color(byte r, byte g, byte b) : this(0, r, g, b)
        {
            ;
        }

        public Color(byte a, byte r, byte g, byte b) : this(false)
        {
            _argb = ((uint)a << 24) + ((uint)r << 16) + ((uint)g << 8) + (uint)b;
            _a = a;
            _r = r;
            _g = g;
            _b = b;
        }

        public Color(byte alpha, Color color) : this(0, color.R, color.G, color.B)
        {
            ;
        }
        #endregion

        #region Public Methods
        public override bool Equals(object obj)
        {
            if (!(obj is Color))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            Color col = (Color)obj;

            return this == col;
        }

        public override int GetHashCode()
        {
            return (int)_argb;
        }

        public string ToHex()
        {
            return "#" + _argb.ToString("H8");
        }
        #endregion

        #region Static Color Constructor Methods
        public static Color FromHex(string hex)
        {
            uint argb = uint.Parse(hex.Replace("#", ""), NumberStyles.HexNumber);
            return new Color(argb);
        }

        public static Color FromArgb(uint argb)
        {
            return new Color(argb);
        }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(a, r, g, b);
        }

        public static Color FromArgb(byte r, byte g, byte b)
        {
            return new Color(r, g, b);
        }

        public static Color FromArgb(byte alpha, Color color)
        {
            return new Color(alpha, color.R, color.G, color.B);
        }

        public static Color FromKnownColor(KnownColor colorName)
        {
            return new Color(colorName);
        }
        #endregion

        #region Operators
        public static bool operator ==(Color a, Color b)
        {
            if (a.IsByLayer && b.IsByLayer)
                return true;
            else
                return a.Argb == b.Argb;
        }

        public static bool operator !=(Color a, Color b)
        {
            if (a.IsByLayer && b.IsByLayer)
                return false;
            else
                return a.Argb != b.Argb;
        }
        #endregion

        #region KnownColor Values
        private static Dictionary<KnownColor, uint> knownColorLookup = new Dictionary<KnownColor, uint>
        {
            { KnownColor.ByLayer, 0x0 },
            { KnownColor.Transparent, 0x0 },
            { KnownColor.AliceBlue, 0xFFF0F8FF },
            { KnownColor.AntiqueWhite, 0xFFFAEBD7 },
            { KnownColor.Aqua, 0xFF00FFFF },
            { KnownColor.Aquamarine, 0xFF7FFFD4 },
            { KnownColor.Azure, 0xFFF0FFFF },
            { KnownColor.Beige, 0xFFF5F5DC },
            { KnownColor.Bisque, 0xFFFFE4C4 },
            { KnownColor.Black, 0xFF000000 },
            { KnownColor.BlanchedAlmond, 0xFFFFEBCD },
            { KnownColor.Blue, 0xFF0000FF },
            { KnownColor.BlueViolet, 0xFF8A2BE2 },
            { KnownColor.Brown, 0xFFA52A2A },
            { KnownColor.BurlyWood, 0xFFDEB887 },
            { KnownColor.CadetBlue, 0xFF5F9EA0 },
            { KnownColor.Chartreuse, 0xFF7FFF00 },
            { KnownColor.Chocolate, 0xFFD2691E },
            { KnownColor.Coral, 0xFFFF7F50 },
            { KnownColor.CornflowerBlue, 0xFF6495ED },
            { KnownColor.Cornsilk, 0xFFFFF8DC },
            { KnownColor.Crimson, 0xFFDC143C },
            { KnownColor.Cyan, 0xFF00FFFF },
            { KnownColor.DarkBlue, 0xFF00008B },
            { KnownColor.DarkCyan, 0xFF008B8B },
            { KnownColor.DarkGoldenrod, 0xFFB8860B },
            { KnownColor.DarkGray, 0xFFA9A9A9 },
            { KnownColor.DarkGreen, 0xFF006400 },
            { KnownColor.DarkKhaki, 0xFFBDB76B },
            { KnownColor.DarkMagenta, 0xFF8B008B },
            { KnownColor.DarkOliveGreen, 0xFF556B2F },
            { KnownColor.DarkOrange, 0xFFFF8C00 },
            { KnownColor.DarkOrchid, 0xFF9932CC },
            { KnownColor.DarkRed, 0xFF8B0000 },
            { KnownColor.DarkSalmon, 0xFFE9967A },
            { KnownColor.DarkSeaGreen, 0xFF8FBC8F },
            { KnownColor.DarkSlateBlue, 0xFF483D8B },
            { KnownColor.DarkSlateGray, 0xFF2F4F4F },
            { KnownColor.DarkTurquoise, 0xFF00CED1 },
            { KnownColor.DarkViolet, 0xFF9400D3 },
            { KnownColor.DeepPink, 0xFFFF1493 },
            { KnownColor.DeepSkyBlue, 0xFF00BFFF },
            { KnownColor.DimGray, 0xFF696969 },
            { KnownColor.DodgerBlue, 0xFF1E90FF },
            { KnownColor.Firebrick, 0xFFB22222 },
            { KnownColor.FloralWhite, 0xFFFFFAF0 },
            { KnownColor.ForestGreen, 0xFF228B22 },
            { KnownColor.Fuchsia, 0xFFFF00FF },
            { KnownColor.Gainsboro, 0xFFDCDCDC },
            { KnownColor.GhostWhite, 0xFFF8F8FF },
            { KnownColor.Gold, 0xFFFFD700 },
            { KnownColor.Goldenrod, 0xFFDAA520 },
            { KnownColor.Gray, 0xFF808080 },
            { KnownColor.Green, 0xFF008000 },
            { KnownColor.GreenYellow, 0xFFADFF2F },
            { KnownColor.Honeydew, 0xFFF0FFF0 },
            { KnownColor.HotPink, 0xFFFF69B4 },
            { KnownColor.IndianRed, 0xFFCD5C5C },
            { KnownColor.Indigo, 0xFF4B0082 },
            { KnownColor.Ivory, 0xFFFFFFF0 },
            { KnownColor.Khaki, 0xFFF0E68C },
            { KnownColor.Lavender, 0xFFE6E6FA },
            { KnownColor.LavenderBlush, 0xFFFFF0F5 },
            { KnownColor.LawnGreen, 0xFF7CFC00 },
            { KnownColor.LemonChiffon, 0xFFFFFACD },
            { KnownColor.LightBlue, 0xFFADD8E6 },
            { KnownColor.LightCoral, 0xFFF08080 },
            { KnownColor.LightCyan, 0xFFE0FFFF },
            { KnownColor.LightGoldenrodYellow, 0xFFFAFAD2 },
            { KnownColor.LightGray, 0xFFD3D3D3 },
            { KnownColor.LightGreen, 0xFF90EE90 },
            { KnownColor.LightPink, 0xFFFFB6C1 },
            { KnownColor.LightSalmon, 0xFFFFA07A },
            { KnownColor.LightSeaGreen, 0xFF20B2AA },
            { KnownColor.LightSkyBlue, 0xFF87CEFA },
            { KnownColor.LightSlateGray, 0xFF778899 },
            { KnownColor.LightSteelBlue, 0xFFB0C4DE },
            { KnownColor.LightYellow, 0xFFFFFFE0 },
            { KnownColor.Lime, 0xFF00FF00 },
            { KnownColor.LimeGreen, 0xFF32CD32 },
            { KnownColor.Linen, 0xFFFAF0E6 },
            { KnownColor.Magenta, 0xFFFF00FF },
            { KnownColor.Maroon, 0xFF800000 },
            { KnownColor.MediumAquamarine, 0xFF66CDAA },
            { KnownColor.MediumBlue, 0xFF0000CD },
            { KnownColor.MediumOrchid, 0xFFBA55D3 },
            { KnownColor.MediumPurple, 0xFF9370DB },
            { KnownColor.MediumSeaGreen, 0xFF3CB371 },
            { KnownColor.MediumSlateBlue, 0xFF7B68EE },
            { KnownColor.MediumSpringGreen, 0xFF00FA9A },
            { KnownColor.MediumTurquoise, 0xFF48D1CC },
            { KnownColor.MediumVioletRed, 0xFFC71585 },
            { KnownColor.MidnightBlue, 0xFF191970 },
            { KnownColor.MintCream, 0xFFF5FFFA },
            { KnownColor.MistyRose, 0xFFFFE4E1 },
            { KnownColor.Moccasin, 0xFFFFE4B5 },
            { KnownColor.NavajoWhite, 0xFFFFDEAD },
            { KnownColor.Navy, 0xFF000080 },
            { KnownColor.OldLace, 0xFFFDF5E6 },
            { KnownColor.Olive, 0xFF808000 },
            { KnownColor.OliveDrab, 0xFF6B8E23 },
            { KnownColor.Orange, 0xFFFFA500 },
            { KnownColor.OrangeRed, 0xFFFF4500 },
            { KnownColor.Orchid, 0xFFDA70D6 },
            { KnownColor.PaleGoldenrod, 0xFFEEE8AA },
            { KnownColor.PaleGreen, 0xFF98FB98 },
            { KnownColor.PaleTurquoise, 0xFFAFEEEE },
            { KnownColor.PaleVioletRed, 0xFFDB7093 },
            { KnownColor.PapayaWhip, 0xFFFFEFD5 },
            { KnownColor.PeachPuff, 0xFFFFDAB9 },
            { KnownColor.Peru, 0xFFCD853F },
            { KnownColor.Pink, 0xFFFFC0CB },
            { KnownColor.Plum, 0xFFDDA0DD },
            { KnownColor.PowderBlue, 0xFFB0E0E6 },
            { KnownColor.Purple, 0xFF800080 },
            { KnownColor.Red, 0xFFFF0000 },
            { KnownColor.RosyBrown, 0xFFBC8F8F },
            { KnownColor.RoyalBlue, 0xFF4169E1 },
            { KnownColor.SaddleBrown, 0xFF8B4513 },
            { KnownColor.Salmon, 0xFFFA8072 },
            { KnownColor.SandyBrown, 0xFFF4A460 },
            { KnownColor.SeaGreen, 0xFF2E8B57 },
            { KnownColor.SeaShell, 0xFFFFF5EE },
            { KnownColor.Sienna, 0xFFA0522D },
            { KnownColor.Silver, 0xFFC0C0C0 },
            { KnownColor.SkyBlue, 0xFF87CEEB },
            { KnownColor.SlateBlue, 0xFF6A5ACD },
            { KnownColor.SlateGray, 0xFF708090 },
            { KnownColor.Snow, 0xFFFFFAFA },
            { KnownColor.SpringGreen, 0xFF00FF7F },
            { KnownColor.SteelBlue, 0xFF4682B4 },
            { KnownColor.Tan, 0xFFD2B48C },
            { KnownColor.Teal, 0xFF008080 },
            { KnownColor.Thistle, 0xFFD8BFD8 },
            { KnownColor.Tomato, 0xFFFF6347 },
            { KnownColor.Turquoise, 0xFF40E0D0 },
            { KnownColor.Violet, 0xFFEE82EE },
            { KnownColor.Wheat, 0xFFF5DEB3 },
            { KnownColor.White, 0xFFFFFFFF },
            { KnownColor.WhiteSmoke, 0xFFF5F5F5 },
            { KnownColor.Yellow, 0xFFFFFF00 },
            { KnownColor.YellowGreen, 0xFF9ACD32 },
        };
        #endregion

        #region Predefined Color Values
        public static Color ByLayer { get { return new Color(true); } }
        public static Color Transparent { get { return FromHex("0"); } }

        public static Color AliceBlue { get { return FromHex("FFF0F8FF"); } }
        public static Color AntiqueWhite { get { return FromHex("FFFAEBD7"); } }
        public static Color Aqua { get { return FromHex("FF00FFFF"); } }
        public static Color Aquamarine { get { return FromHex("FF7FFFD4"); } }
        public static Color Azure { get { return FromHex("FFF0FFFF"); } }
        public static Color Beige { get { return FromHex("FFF5F5DC"); } }
        public static Color Bisque { get { return FromHex("FFFFE4C4"); } }
        public static Color Black { get { return FromHex("FF000000"); } }
        public static Color BlanchedAlmond { get { return FromHex("FFFFEBCD"); } }
        public static Color Blue { get { return FromHex("FF0000FF"); } }
        public static Color BlueViolet { get { return FromHex("FF8A2BE2"); } }
        public static Color Brown { get { return FromHex("FFA52A2A"); } }
        public static Color BurlyWood { get { return FromHex("FFDEB887"); } }
        public static Color CadetBlue { get { return FromHex("FF5F9EA0"); } }
        public static Color Chartreuse { get { return FromHex("FF7FFF00"); } }
        public static Color Chocolate { get { return FromHex("FFD2691E"); } }
        public static Color Coral { get { return FromHex("FFFF7F50"); } }
        public static Color CornflowerBlue { get { return FromHex("FF6495ED"); } }
        public static Color Cornsilk { get { return FromHex("FFFFF8DC"); } }
        public static Color Crimson { get { return FromHex("FFDC143C"); } }
        public static Color Cyan { get { return FromHex("FF00FFFF"); } }
        public static Color DarkBlue { get { return FromHex("FF00008B"); } }
        public static Color DarkCyan { get { return FromHex("FF008B8B"); } }
        public static Color DarkGoldenrod { get { return FromHex("FFB8860B"); } }
        public static Color DarkGray { get { return FromHex("FFA9A9A9"); } }
        public static Color DarkGreen { get { return FromHex("FF006400"); } }
        public static Color DarkKhaki { get { return FromHex("FFBDB76B"); } }
        public static Color DarkMagenta { get { return FromHex("FF8B008B"); } }
        public static Color DarkOliveGreen { get { return FromHex("FF556B2F"); } }
        public static Color DarkOrange { get { return FromHex("FFFF8C00"); } }
        public static Color DarkOrchid { get { return FromHex("FF9932CC"); } }
        public static Color DarkRed { get { return FromHex("FF8B0000"); } }
        public static Color DarkSalmon { get { return FromHex("FFE9967A"); } }
        public static Color DarkSeaGreen { get { return FromHex("FF8FBC8F"); } }
        public static Color DarkSlateBlue { get { return FromHex("FF483D8B"); } }
        public static Color DarkSlateGray { get { return FromHex("FF2F4F4F"); } }
        public static Color DarkTurquoise { get { return FromHex("FF00CED1"); } }
        public static Color DarkViolet { get { return FromHex("FF9400D3"); } }
        public static Color DeepPink { get { return FromHex("FFFF1493"); } }
        public static Color DeepSkyBlue { get { return FromHex("FF00BFFF"); } }
        public static Color DimGray { get { return FromHex("FF696969"); } }
        public static Color DodgerBlue { get { return FromHex("FF1E90FF"); } }
        public static Color Firebrick { get { return FromHex("FFB22222"); } }
        public static Color FloralWhite { get { return FromHex("FFFFFAF0"); } }
        public static Color ForestGreen { get { return FromHex("FF228B22"); } }
        public static Color Fuchsia { get { return FromHex("FFFF00FF"); } }
        public static Color Gainsboro { get { return FromHex("FFDCDCDC"); } }
        public static Color GhostWhite { get { return FromHex("FFF8F8FF"); } }
        public static Color Gold { get { return FromHex("FFFFD700"); } }
        public static Color Goldenrod { get { return FromHex("FFDAA520"); } }
        public static Color Gray { get { return FromHex("FF808080"); } }
        public static Color Green { get { return FromHex("FF008000"); } }
        public static Color GreenYellow { get { return FromHex("FFADFF2F"); } }
        public static Color Honeydew { get { return FromHex("FFF0FFF0"); } }
        public static Color HotPink { get { return FromHex("FFFF69B4"); } }
        public static Color IndianRed { get { return FromHex("FFCD5C5C"); } }
        public static Color Indigo { get { return FromHex("FF4B0082"); } }
        public static Color Ivory { get { return FromHex("FFFFFFF0"); } }
        public static Color Khaki { get { return FromHex("FFF0E68C"); } }
        public static Color Lavender { get { return FromHex("FFE6E6FA"); } }
        public static Color LavenderBlush { get { return FromHex("FFFFF0F5"); } }
        public static Color LawnGreen { get { return FromHex("FF7CFC00"); } }
        public static Color LemonChiffon { get { return FromHex("FFFFFACD"); } }
        public static Color LightBlue { get { return FromHex("FFADD8E6"); } }
        public static Color LightCoral { get { return FromHex("FFF08080"); } }
        public static Color LightCyan { get { return FromHex("FFE0FFFF"); } }
        public static Color LightGoldenrodYellow { get { return FromHex("FFFAFAD2"); } }
        public static Color LightGray { get { return FromHex("FFD3D3D3"); } }
        public static Color LightGreen { get { return FromHex("FF90EE90"); } }
        public static Color LightPink { get { return FromHex("FFFFB6C1"); } }
        public static Color LightSalmon { get { return FromHex("FFFFA07A"); } }
        public static Color LightSeaGreen { get { return FromHex("FF20B2AA"); } }
        public static Color LightSkyBlue { get { return FromHex("FF87CEFA"); } }
        public static Color LightSlateGray { get { return FromHex("FF778899"); } }
        public static Color LightSteelBlue { get { return FromHex("FFB0C4DE"); } }
        public static Color LightYellow { get { return FromHex("FFFFFFE0"); } }
        public static Color Lime { get { return FromHex("FF00FF00"); } }
        public static Color LimeGreen { get { return FromHex("FF32CD32"); } }
        public static Color Linen { get { return FromHex("FFFAF0E6"); } }
        public static Color Magenta { get { return FromHex("FFFF00FF"); } }
        public static Color Maroon { get { return FromHex("FF800000"); } }
        public static Color MediumAquamarine { get { return FromHex("FF66CDAA"); } }
        public static Color MediumBlue { get { return FromHex("FF0000CD"); } }
        public static Color MediumOrchid { get { return FromHex("FFBA55D3"); } }
        public static Color MediumPurple { get { return FromHex("FF9370DB"); } }
        public static Color MediumSeaGreen { get { return FromHex("FF3CB371"); } }
        public static Color MediumSlateBlue { get { return FromHex("FF7B68EE"); } }
        public static Color MediumSpringGreen { get { return FromHex("FF00FA9A"); } }
        public static Color MediumTurquoise { get { return FromHex("FF48D1CC"); } }
        public static Color MediumVioletRed { get { return FromHex("FFC71585"); } }
        public static Color MidnightBlue { get { return FromHex("FF191970"); } }
        public static Color MintCream { get { return FromHex("FFF5FFFA"); } }
        public static Color MistyRose { get { return FromHex("FFFFE4E1"); } }
        public static Color Moccasin { get { return FromHex("FFFFE4B5"); } }
        public static Color NavajoWhite { get { return FromHex("FFFFDEAD"); } }
        public static Color Navy { get { return FromHex("FF000080"); } }
        public static Color OldLace { get { return FromHex("FFFDF5E6"); } }
        public static Color Olive { get { return FromHex("FF808000"); } }
        public static Color OliveDrab { get { return FromHex("FF6B8E23"); } }
        public static Color Orange { get { return FromHex("FFFFA500"); } }
        public static Color OrangeRed { get { return FromHex("FFFF4500"); } }
        public static Color Orchid { get { return FromHex("FFDA70D6"); } }
        public static Color PaleGoldenrod { get { return FromHex("FFEEE8AA"); } }
        public static Color PaleGreen { get { return FromHex("FF98FB98"); } }
        public static Color PaleTurquoise { get { return FromHex("FFAFEEEE"); } }
        public static Color PaleVioletRed { get { return FromHex("FFDB7093"); } }
        public static Color PapayaWhip { get { return FromHex("FFFFEFD5"); } }
        public static Color PeachPuff { get { return FromHex("FFFFDAB9"); } }
        public static Color Peru { get { return FromHex("FFCD853F"); } }
        public static Color Pink { get { return FromHex("FFFFC0CB"); } }
        public static Color Plum { get { return FromHex("FFDDA0DD"); } }
        public static Color PowderBlue { get { return FromHex("FFB0E0E6"); } }
        public static Color Purple { get { return FromHex("FF800080"); } }
        public static Color Red { get { return FromHex("FFFF0000"); } }
        public static Color RosyBrown { get { return FromHex("FFBC8F8F"); } }
        public static Color RoyalBlue { get { return FromHex("FF4169E1"); } }
        public static Color SaddleBrown { get { return FromHex("FF8B4513"); } }
        public static Color Salmon { get { return FromHex("FFFA8072"); } }
        public static Color SandyBrown { get { return FromHex("FFF4A460"); } }
        public static Color SeaGreen { get { return FromHex("FF2E8B57"); } }
        public static Color SeaShell { get { return FromHex("FFFFF5EE"); } }
        public static Color Sienna { get { return FromHex("FFA0522D"); } }
        public static Color Silver { get { return FromHex("FFC0C0C0"); } }
        public static Color SkyBlue { get { return FromHex("FF87CEEB"); } }
        public static Color SlateBlue { get { return FromHex("FF6A5ACD"); } }
        public static Color SlateGray { get { return FromHex("FF708090"); } }
        public static Color Snow { get { return FromHex("FFFFFAFA"); } }
        public static Color SpringGreen { get { return FromHex("FF00FF7F"); } }
        public static Color SteelBlue { get { return FromHex("FF4682B4"); } }
        public static Color Tan { get { return FromHex("FFD2B48C"); } }
        public static Color Teal { get { return FromHex("FF008080"); } }
        public static Color Thistle { get { return FromHex("FFD8BFD8"); } }
        public static Color Tomato { get { return FromHex("FFFF6347"); } }
        public static Color Turquoise { get { return FromHex("FF40E0D0"); } }
        public static Color Violet { get { return FromHex("FFEE82EE"); } }
        public static Color Wheat { get { return FromHex("FFF5DEB3"); } }
        public static Color White { get { return FromHex("FFFFFFFF"); } }
        public static Color WhiteSmoke { get { return FromHex("FFF5F5F5"); } }
        public static Color Yellow { get { return FromHex("FFFFFF00"); } }
        public static Color YellowGreen { get { return FromHex("FF9ACD32"); } }
        #endregion
    }
}
