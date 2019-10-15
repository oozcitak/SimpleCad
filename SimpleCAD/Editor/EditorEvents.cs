using System;

namespace SimpleCAD
{
    public delegate void EditorPromptEventHandler(object sender, EditorPromptEventArgs e);
    public delegate void EditorErrorEventHandler(object sender, EditorErrorEventArgs e);

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

    public class EditorErrorEventArgs : EventArgs
    {
        public Exception Error { get; private set; }

        public EditorErrorEventArgs(Exception  error) : base()
        {
            Error = error;
        }
    }
}
