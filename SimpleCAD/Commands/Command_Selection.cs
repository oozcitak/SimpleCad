using System.Threading.Tasks;

namespace SimpleCAD.Commands
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
