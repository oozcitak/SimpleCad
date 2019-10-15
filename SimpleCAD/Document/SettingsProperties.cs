
namespace SimpleCAD
{
    public partial class Settings
    {
        public System.Int32 DisplayPrecision 
        {
            get => Get<System.Int32>("DisplayPrecision");
            set => Set("DisplayPrecision", value);
        }
        public SimpleCAD.Graphics.Color BackColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("BackColor");
            set => Set("BackColor", value);
        }
        public SimpleCAD.Graphics.Color CursorPromptBackColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("CursorPromptBackColor");
            set => Set("CursorPromptBackColor", value);
        }
        public SimpleCAD.Graphics.Color CursorPromptForeColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("CursorPromptForeColor");
            set => Set("CursorPromptForeColor", value);
        }
        public SimpleCAD.Graphics.Color SelectionWindowColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("SelectionWindowColor");
            set => Set("SelectionWindowColor", value);
        }
        public SimpleCAD.Graphics.Color SelectionWindowBorderColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("SelectionWindowBorderColor");
            set => Set("SelectionWindowBorderColor", value);
        }
        public SimpleCAD.Graphics.Color ReverseSelectionWindowColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("ReverseSelectionWindowColor");
            set => Set("ReverseSelectionWindowColor", value);
        }
        public SimpleCAD.Graphics.Color ReverseSelectionWindowBorderColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("ReverseSelectionWindowBorderColor");
            set => Set("ReverseSelectionWindowBorderColor", value);
        }
        public SimpleCAD.Graphics.Color SelectionHighlightColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("SelectionHighlightColor");
            set => Set("SelectionHighlightColor", value);
        }
        public SimpleCAD.Graphics.Color JigColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("JigColor");
            set => Set("JigColor", value);
        }
        public SimpleCAD.Graphics.Color ControlPointColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("ControlPointColor");
            set => Set("ControlPointColor", value);
        }
        public SimpleCAD.Graphics.Color ActiveControlPointColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("ActiveControlPointColor");
            set => Set("ActiveControlPointColor", value);
        }
        public SimpleCAD.Graphics.Color SnapPointColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("SnapPointColor");
            set => Set("SnapPointColor", value);
        }
        public SimpleCAD.Graphics.Color MinorGridColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("MinorGridColor");
            set => Set("MinorGridColor", value);
        }
        public SimpleCAD.Graphics.Color MajorGridColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("MajorGridColor");
            set => Set("MajorGridColor", value);
        }
        public SimpleCAD.Graphics.Color AxisColor 
        {
            get => Get<SimpleCAD.Graphics.Color>("AxisColor");
            set => Set("AxisColor", value);
        }
        public System.Int32 PickBoxSize 
        {
            get => Get<System.Int32>("PickBoxSize");
            set => Set("PickBoxSize", value);
        }
        public System.Int32 ControlPointSize 
        {
            get => Get<System.Int32>("ControlPointSize");
            set => Set("ControlPointSize", value);
        }
        public System.Int32 PointSize 
        {
            get => Get<System.Int32>("PointSize");
            set => Set("PointSize", value);
        }
        public System.Boolean Snap 
        {
            get => Get<System.Boolean>("Snap");
            set => Set("Snap", value);
        }
        public System.Int32 SnapPointSize 
        {
            get => Get<System.Int32>("SnapPointSize");
            set => Set("SnapPointSize", value);
        }
        public System.Int32 SnapDistance 
        {
            get => Get<System.Int32>("SnapDistance");
            set => Set("SnapDistance", value);
        }
        public SimpleCAD.SnapPointType SnapMode 
        {
            get => Get<SimpleCAD.SnapPointType>("SnapMode");
            set => Set("SnapMode", value);
        }
    }
}
