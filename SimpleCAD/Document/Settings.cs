using System.Collections.Generic;
using System.Globalization;
using SimpleCAD.Graphics;

namespace SimpleCAD
{
    public partial class Settings : IPersistable
    {
        public static Dictionary<string, object> Defaults
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { "DisplayPrecision", 2 },
                    { "BackColor", Color.FromArgb(33,40,48) },
                    { "CursorPromptBackColor", Color.FromArgb(84,58,84) },
                    { "CursorPromptForeColor", Color.FromArgb(128,Color.White) },
                    { "SelectionWindowColor", Color.FromArgb(64,46,116,251) },
                    { "SelectionWindowBorderColor", Color.White },
                    { "ReverseSelectionWindowColor", Color.FromArgb(64,46,251,116) },
                    { "ReverseSelectionWindowBorderColor", Color.White },
                    { "SelectionHighlightColor", Color.FromArgb(64,46,116,251) },
                    { "JigColor", Color.Orange },
                    { "ControlPointColor", Color.FromArgb(46,116,251) },
                    { "ActiveControlPointColor", Color.FromArgb(251,116,46) },
                    { "SnapPointColor", Color.FromArgb(251,251,116) },
                    { "MinorGridColor", Color.FromArgb(64,64,64) },
                    { "MajorGridColor", Color.FromArgb(96,96,96) },
                    { "AxisColor", Color.FromArgb(128,128,64) },
                    { "PickBoxSize", 6 },
                    { "ControlPointSize", 7 },
                    { "PointSize", 6 },
                    { "Snap", true },
                    { "SnapPointSize", 11 },
                    { "SnapDistance", 25 },
                    { "SnapMode", SnapPointType.All },
                    { "AngleMode", AngleMode.Degrees },
                };
            }
        }

        private Dictionary<string, object> items = new Dictionary<string, object>();

        public NumberFormatInfo NumberFormat { get; private set; }

        public Settings()
        {
            Reset();
        }

        public void Set(string name, object value)
        {
            if (items.ContainsKey(name))
            {
                items[name] = value;
            }
            else
            {
                items.Add(name, value);
            }
        }

        public object Get(string name)
        {
            return items[name];
        }

        public T Get<T>(string name)
        {
            return (T)Get(name);
        }

        public void Reset()
        {
            items.Clear();
            foreach (var pair in Defaults)
            {

                items.Add(pair.Key, pair.Value);
            }

            UpdateSettings();
        }

        public void Load(DocumentReader reader)
        {
            items = new Dictionary<string, object>();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                string valueType = reader.ReadString();
                object value = null;
                if (valueType == "bool")
                {
                    value = reader.ReadBoolean();
                }
                else if (valueType == "int")
                {
                    value = reader.ReadInt();
                }
                else if (valueType == "color")
                {
                    value = reader.ReadColor();
                }

                items.Add(name, value);
            }

            UpdateSettings();
        }

        public void Save(DocumentWriter writer)
        {
            writer.Write(items.Count);
            foreach (var pair in items)
            {
                writer.Write(pair.Key);
                if (pair.Value is bool)
                {
                    writer.Write("bool");
                    writer.Write((bool)pair.Value);
                }
                else if (pair.Value is int || pair.Value is System.Enum)
                {
                    writer.Write("int");
                    writer.Write((int)pair.Value);
                }
                else if (pair.Value is Color)
                {
                    writer.Write("color");
                    writer.Write((Color)pair.Value);
                }
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
