using System.Threading.Tasks;

namespace SimpleCAD.Commands
{
    public class DocumentNew : Command
    {
        public override string RegisteredName => "Document.New";
        public override string Name => "New";

        public override Task Apply(CADDocument doc, params string[] args)
        {
            doc.New();
            return Task.FromResult(default(object));
        }
    }

    public class DocumentOpen : Command
    {
        public override string RegisteredName => "Document.Open";
        public override string Name => "Open";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.FilenameResult res = await ed.GetOpenFilename("Open file");
            if (res.Result == Editor.ResultMode.OK)
            {
                doc.Open(res.Value);
            }
        }
    }

    public class DocumentSave : Command
    {
        public override string RegisteredName => "Document.Save";
        public override string Name => "Save";

        public override Task Apply(CADDocument doc, params string[] args)
        {
            doc.Save(doc.FileName);
            return Task.FromResult(default(object));
        }
    }

    public class DocumentSaveAs : Command
    {
        public override string RegisteredName => "Document.SaveAs";
        public override string Name => "Save As";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            Editor.FilenameResult res = await ed.GetSaveFilename("Save file");
            if (res.Result == Editor.ResultMode.OK)
            {
                doc.Save(res.Value);
            }
        }
    }
}
