using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCAD
{
    public partial class Commands
    {
        public class SelectionClear : Command
        {
            public override string RegisteredName => "Selection.Clear";
            public override string Name => "Clear Selection";

            public override Task Apply(CADDocument doc, params string[] args)
            {
                doc.Editor.CurrentSelection.Clear();
                return Task.FromResult(default(object));
            }
        }
    }
}