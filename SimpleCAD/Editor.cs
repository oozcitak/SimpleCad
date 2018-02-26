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
        private TaskCompletionSource<AngleResult> angleCompletion;
        private TaskCompletionSource<TextResult> textCompletion;
        private bool inputHasBasePoint;
        private Point2D inputBasePoint;
        private Point2D lastMouseLocation;
        private string lastText;

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
            return await GetPoint(new PointOptions(message));
        }

        public async Task<PointResult> GetPoint(string message, Point2D basePoint)
        {
            return await GetPoint(new PointOptions(message, basePoint));
        }

        public async Task<PointResult> GetPoint(PointOptions options)
        {
            Mode = InputMode.Point;
            inputHasBasePoint = options.HasBasePoint;
            if (options.HasBasePoint) inputBasePoint = options.BasePoint;
            pointCompletion = new TaskCompletionSource<PointResult>();
            PointResult res = await pointCompletion.Task;
            Mode = InputMode.None;
            return res;
        }

        public async Task<AngleResult> GetAngle(string message, Point2D basePoint)
        {
            return await GetAngle(new AngleOptions(message, basePoint));
        }

        public async Task<AngleResult> GetAngle(AngleOptions options)
        {
            Mode = InputMode.Angle;
            inputHasBasePoint = true;
            inputBasePoint = options.BasePoint;
            angleCompletion = new TaskCompletionSource<AngleResult>();
            AngleResult res = await angleCompletion.Task;
            Mode = InputMode.None;
            return res;
        }

        public async Task<TextResult> GetText(string message)
        {
            return await GetText(new InputOptions(message));
        }

        public async Task<TextResult> GetText(InputOptions options)
        {
            Mode = InputMode.Text;
            inputHasBasePoint = false;
            lastText = "";
            textCompletion = new TaskCompletionSource<TextResult>();
            TextResult res = await textCompletion.Task;
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
                    break;
                case InputMode.Angle:
                    if (e.Button == MouseButtons.Left)
                        angleCompletion.SetResult(new AngleResult(true, point - inputBasePoint));
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
                case InputMode.Angle:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                        angleCompletion.SetResult(new AngleResult(true, lastMouseLocation - inputBasePoint));
                    else if (e.KeyCode == Keys.Escape)
                        angleCompletion.SetResult(new AngleResult(false, lastMouseLocation - inputBasePoint));
                    break;
                case InputMode.Text:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                        textCompletion.SetResult(new TextResult(true, lastText));
                    else if (e.KeyCode == Keys.Escape)
                        textCompletion.SetResult(new TextResult(false, lastText));
                    break;
            }
        }

        internal void OnViewKeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Mode)
            {
                case InputMode.Text:
                    if (e.KeyChar == '\b') // backspace
                    {
                        lastText.Remove(lastText.Length - 1);
                    }
                    else if (!char.IsControl(e.KeyChar))
                    {
                        lastText += e.KeyChar;
                    }
                    break;
            }
        }
    }
}
