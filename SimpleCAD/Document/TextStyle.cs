using SimpleCAD.Graphics;
using System.ComponentModel;

namespace SimpleCAD
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TextStyle : IPersistable
    {
        public static TextStyle Default => new TextStyle("0", "Calibri Light", FontStyle.Regular);

        public string Name { get; private set; }
        public string FontFamily { get; set; }
        public FontStyle FontStyle { get; set; }

        public TextStyle()
        {
            ;
        }

        public TextStyle(string name, string fontFamily, FontStyle fontStyle)
        {
            Name = name;
            FontFamily = fontFamily;
            FontStyle = fontStyle;
        }

        public void Load(DocumentReader reader)
        {
            Name = reader.ReadString();
            FontFamily = reader.ReadString();
            FontStyle = (FontStyle)reader.ReadInt();
        }

        public void Save(DocumentWriter writer)
        {
            writer.Write(Name);
            writer.Write(FontFamily);
            writer.Write((int)FontStyle);
        }
    }
}
