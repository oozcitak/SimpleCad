using SimpleCAD.Geometry;
using System;
using System.Windows.Forms;

namespace SimpleCAD
{
    public delegate void DocumentChangedEventHandler(object sender, EventArgs e);
    public delegate void TransientsChangedEventHandler(object sender, EventArgs e);
    public delegate void SelectionChangedEventHandler(object sender, EventArgs e);
    public delegate void EditorPromptEventHandler(object sender, EditorPromptEventArgs e);
    internal delegate void CursorPromptEventHandler(object sender, CursorPromptEventArgs e);
    public delegate void CursorEventHandler(object sender, CursorEventArgs e);

    public class EditorPromptEventArgs : EventArgs
    {
        public string Status { get; private set; }

        public EditorPromptEventArgs() : this("")
        {
            ;
        }

        public EditorPromptEventArgs(string status) : base()
        {
            Status = status;
        }
    }

    public class CursorEventArgs : EventArgs
    {
        public MouseButtons Button { get; private set; }
        public Point2D Location { get; private set; }
        public int Clicks { get; private set; }
        public int Delta { get; private set; }

        public float X { get { return Location.X; } }
        public float Y { get { return Location.Y; } }

        public CursorEventArgs(MouseButtons button, int clicks, Point2D location, int delta)
        {
            Button = button;
            Location = location;
            Clicks = clicks;
            Delta = delta;
        }

        public CursorEventArgs(MouseButtons button, int clicks, float x, float y, int delta)
            : this(button, clicks, new Point2D(x, y), delta)
        {
            ;
        }
    }

    internal class CursorPromptEventArgs : EventArgs
    {
        public string Status { get; private set; }

        public CursorPromptEventArgs() : this("")
        {
            ;
        }

        public CursorPromptEventArgs(string status) : base()
        {
            Status = status;
        }
    }
}
