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
        public class DocumentNew : SyncCommand
        {
            public override string RegisteredName => "Document.New";
            public override string Name => "New";

            public override void Apply(CADDocument doc, params string[] args)
            {
                doc.New();
            }
        }

        public class DocumentOpen : SyncCommand
        {
            public override string RegisteredName => "Document.Open";
            public override string Name => "Open";

            public override void Apply(CADDocument doc, params string[] args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "SimpleCAD file (*.scf)|*.scf|All files (*.*)|*.*";
                ofd.DefaultExt = "scf";
                if (args.Length > 0)
                {
                    ofd.FileName = Path.GetFileName(args[0]);
                    ofd.InitialDirectory = Path.GetDirectoryName(args[0]);
                }
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    doc.Open(ofd.FileName);
                }
            }
        }

        public class DocumentSave : SyncCommand
        {
            public override string RegisteredName => "Document.Save";
            public override string Name => "Save";

            public override void Apply(CADDocument doc, params string[] args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "SimpleCAD file (*.scf)|*.scf|All files (*.*)|*.*";
                sfd.DefaultExt = "scf";
                if (args.Length > 0)
                {
                    sfd.FileName = Path.GetFileName(args[0]);
                    sfd.InitialDirectory = Path.GetDirectoryName(args[0]);
                }
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    doc.Save(sfd.FileName);
                }
            }
        }
    }
}