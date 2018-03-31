using SimpleCAD.Geometry;
using System;
using System.Windows.Forms;

namespace SimpleCAD
{
    public delegate void EditorPromptEventHandler(object sender, EditorPromptEventArgs e);

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
}
