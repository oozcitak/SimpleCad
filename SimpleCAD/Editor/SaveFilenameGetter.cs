using System.IO;
using System.Windows.Forms;

namespace SimpleCAD
{
    internal class SaveFilenameGetter : EditorGetter<FilenameOptions, string>
    {
        protected override void Init(InitArgs<string> args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = Options.Message;
            sfd.Filter = Options.Filter;
            sfd.DefaultExt = "scf";
            string filename = "";
            string path = "";
            try
            {
                filename = Path.GetFileName(Options.FileName);
                path = Path.GetDirectoryName(Options.FileName);
            }
            catch
            {
                ;
            }
            if (!string.IsNullOrEmpty(filename)) sfd.FileName = filename;
            if (!string.IsNullOrEmpty(path)) sfd.InitialDirectory = path;

            if (sfd.ShowDialog() == DialogResult.OK)
                args.Value = sfd.FileName;
            else
                args.InputValid = false;

            args.ContinueAsync = false;
        }
    }
}
