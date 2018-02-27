using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public delegate void DocumentChangedEventHandler(object sender, EventArgs e);
    public delegate void TransientsChangedEventHandler(object sender, EventArgs e);
    public delegate void SelectionChangedEventHandler(object sender, EventArgs e);
    public delegate void EditorPromptEventHandler(object sender, EditorPromptEventArgs e);

    public class EditorPromptEventArgs : EventArgs
    {
        public string Status { get; private set; }

        public EditorPromptEventArgs(string status) : base()
        {
            Status = status;
        }
    }
}
