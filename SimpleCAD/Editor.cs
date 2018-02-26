using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCAD
{
    public partial class Editor
    {
        public static Dictionary<string, Command> Commands { get; private set; }

        public CADDocument Document { get; private set; }
        internal InputMode Mode { get; private set; }

        private TaskCompletionSource<PointResult> pointCompletion;
        private Point2D lastMouseLocation;

        public SelectionSet Selection { get; private set; } = new SelectionSet();
        public Color SelectionHighlight { get; set; } = Color.FromArgb(64, 46, 116, 251);

        static Editor()
        {
            Commands = new Dictionary<string, Command>();
            // Search the assembly for commands
            Assembly assembly = Assembly.GetAssembly(typeof(CADDocument));
            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType == typeof(Command))
                {
                    Command com = (Command)assembly.CreateInstance(type.FullName);
                    if (com == null)
                    {
                        assembly = Assembly.GetExecutingAssembly();
                        com = (Command)Assembly.GetExecutingAssembly().CreateInstance(type.FullName);
                    }
                    if (com != null)
                    {
                        Commands.Add(com.RegisteredName, com);
                    }
                }
            }
        }

        public Editor(CADDocument doc)
        {
            Document = doc;
        }

        public void RunCommand(string registeredName)
        {
            Command com = Commands[registeredName];
            com.Apply(Document);
        }

        public async Task<PointResult> GetPoint(string message)
        {
            Mode = InputMode.Point;
            pointCompletion = new TaskCompletionSource<PointResult>();
            PointResult res = await pointCompletion.Task;
            Mode = InputMode.None;
            return res;
        }

        internal void OnViewMouseMove(object sender, MouseEventArgs e, Point2D point)
        {
            lastMouseLocation = point;
        }

        internal void OnViewMouseClick(object sender, MouseEventArgs e, Point2D point)
        {
            switch (Mode)
            {
                case InputMode.Point:
                    if (e.Button == MouseButtons.Left)
                        pointCompletion.SetResult(new PointResult(true, point));
                    else
                        pointCompletion.SetResult(new PointResult(false, point));
                    break;
            }
        }

        internal void OnViewKeyDown(object sender, KeyEventArgs e)
        {
            switch (Mode)
            {
                case InputMode.Point:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                        pointCompletion.SetResult(new PointResult(true, lastMouseLocation));
                    else if (e.KeyCode == Keys.Escape)
                        pointCompletion.SetResult(new PointResult(false, lastMouseLocation));
                    break;
            }
        }
    }
}
