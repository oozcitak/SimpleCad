using System;
using System.ComponentModel;

namespace SimpleCAD.Graphics
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Layer : IPersistable
    {
        public static Layer Default => new Layer("0", new Style(Color.White));

        public string Name { get; set; }
        public Style Style { get; set; } = new Style(Color.White, 0, DashStyle.Solid);
        public bool Visible { get; set; } = true;

        public Layer()
        {
            ;
        }

        public Layer(string name, Style style)
        {
            Name = name;
            Style = style;
        }

        public void Load(DocumentReader reader)
        {
            Name = reader.ReadString();
            Style = new Style();
            Style.Load(reader);
            Visible = reader.ReadBoolean();
        }

        public void Save(DocumentWriter writer)
        {
            writer.Write(Name);
            Style.Save(writer);
            writer.Write(Visible);
        }
    }
}
