using System.Collections.Generic;
using SimpleCAD.Graphics;
using System.Globalization;
using System.IO;

namespace SimpleCAD
{
    public class Settings : IPersistable
    {
        private Dictionary<string, Setting> items = new Dictionary<string, Setting>();

        private class Setting : IPersistable
        {
            public string Name { get; protected set; }
            public object Value { get; set; }

            public Setting(string name, object value)
            {
                Name = name;
                Value = value;
            }

            public Setting(BinaryReader reader)
            {
                Name = reader.ReadString();
                string valueType = reader.ReadString();
                if (valueType == "int")
                {
                    Value = reader.ReadInt32();
                }
                else if (valueType == "color")
                {
                    Value = Color.FromArgb(reader.ReadUInt32());
                }
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(Name);
                if (Value is int)
                {
                    writer.Write("int");
                    writer.Write((int)Value);
                }
                else if (Value is Color)
                {
                    writer.Write("color");
                    writer.Write(((Color)Value).Argb);
                }
            }
        }

        public void Set(string name, object value)
        {
            if (items.TryGetValue(name, out Setting s))
            {
                s.Value = value;
            }
            else
            {
                items.Add(name, new Setting(name, value));
            }
        }

        public object Get(string name)
        {
            return items[name].Value;
        }

        public T Get<T>(string name)
        {
            return (T)Get(name);
        }

        public NumberFormatInfo NumberFormat { get; private set; }

        public Settings()
        {
            Set("DisplayPrecision", 2);

            Set("BackColor", Color.FromArgb(33, 40, 48));

            Set("CursorColor", Color.White);
            Set("CursorPromptBackColor", Color.FromArgb(84, 58, 84));
            Set("CursorPromptForeColor", Color.FromArgb(128, Color.White));

            Set("SelectionWindowColor", Color.FromArgb(64, 46, 116, 251));
            Set("SelectionWindowBorderColor", Color.White);
            Set("ReverseSelectionWindowColor", Color.FromArgb(64, 46, 251, 116));
            Set("ReverseSelectionWindowBorderColor", Color.White);

            Set("SelectionHighlightColor", Color.FromArgb(64, 46, 116, 251));
            Set("JigColor", Color.Orange);
            Set("ControlPointColor", Color.FromArgb(46, 116, 251));
            Set("ActiveControlPointColor", Color.FromArgb(251, 116, 46));

            Set("MinorGridColor", Color.FromArgb(64, 64, 64));
            Set("MajorGridColor", Color.FromArgb(96, 96, 96));
            Set("AxisColor", Color.FromArgb(128, 128, 64));

            Set("PickBoxSize", 6);
            Set("ControlPointSize", 7);

            UpdateSettings();
        }

        public Settings(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Setting s = new Setting(reader);
                items.Add(s.Name, s);
            }

            UpdateSettings();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(items.Count);
            foreach (Setting s in items.Values)
            {
                s.Save(writer);
            }
        }

        private void UpdateSettings()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = Get<int>("DisplayPrecision");
            nfi.NumberDecimalSeparator = ".";
            NumberFormat = nfi;
        }
    }
}
