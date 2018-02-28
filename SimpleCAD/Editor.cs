using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        public event EditorPromptEventHandler Prompt;

        private TaskCompletionSource<PointResult> pointCompletion;
        private TaskCompletionSource<AngleResult> angleCompletion;
        private TaskCompletionSource<TextResult> textCompletion;

        private PointOptions currentPointOptions;
        private AngleOptions currentAngleOptions;
        private TextOptions currentTextOptions;

        private Point2D lastMouseLocation;
        private string lastText;
        private Line consLine;

        public SelectionSet Selection { get; private set; } = new SelectionSet();
        public Color SelectionHighlight { get; set; } = Color.FromArgb(64, 46, 116, 251);
        public OutlineStyle TransientStyle { get; set; } = new OutlineStyle(Color.Orange, 1, DashStyle.Dash);

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

        public async Task<PointResult> GetPoint(string message, Action<Point2D> jig)
        {
            return await GetPoint(new PointOptions(message, jig));
        }

        public async Task<PointResult> GetPoint(string message, Point2D basePoint)
        {
            return await GetPoint(new PointOptions(message, basePoint));
        }

        public async Task<PointResult> GetPoint(string message, Point2D basePoint, Action<Point2D> jig)
        {
            return await GetPoint(new PointOptions(message, basePoint, jig));
        }

        public async Task<PointResult> GetPoint(PointOptions options)
        {
            Mode = InputMode.Point;
            OnPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));
            currentPointOptions = options;
            if (options.HasBasePoint)
            {
                consLine = new Line(options.BasePoint, options.BasePoint);
                consLine.OutlineStyle = TransientStyle;
                Document.Transients.Add(consLine);
            }
            pointCompletion = new TaskCompletionSource<PointResult>();
            PointResult res = await pointCompletion.Task;
            Mode = InputMode.None;
            OnPrompt(new EditorPromptEventArgs(""));
            Document.Transients.Remove(consLine);
            return res;
        }

        public async Task<AngleResult> GetAngle(string message, Point2D basePoint, Action<Vector2D> jig)
        {
            return await GetAngle(new AngleOptions(message, basePoint, jig));
        }

        public async Task<AngleResult> GetAngle(string message, Point2D basePoint)
        {
            return await GetAngle(new AngleOptions(message, basePoint));
        }

        public async Task<AngleResult> GetAngle(AngleOptions options)
        {
            Mode = InputMode.Angle;
            OnPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));
            currentAngleOptions = options;
            consLine = new Line(options.BasePoint, options.BasePoint);
            consLine.OutlineStyle = TransientStyle;
            Document.Transients.Add(consLine);
            angleCompletion = new TaskCompletionSource<AngleResult>();
            AngleResult res = await angleCompletion.Task;
            Mode = InputMode.None;
            OnPrompt(new EditorPromptEventArgs(""));
            Document.Transients.Remove(consLine);
            return res;
        }

        public async Task<TextResult> GetText(string message, Action<string> jig)
        {
            return await GetText(new TextOptions(message, jig));
        }

        public async Task<TextResult> GetText(string message)
        {
            return await GetText(new TextOptions(message));
        }

        public async Task<TextResult> GetText(TextOptions options)
        {
            Mode = InputMode.Text;
            OnPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));
            currentTextOptions = options;
            lastText = "";
            textCompletion = new TaskCompletionSource<TextResult>();
            TextResult res = await textCompletion.Task;
            Mode = InputMode.None;
            OnPrompt(new EditorPromptEventArgs(""));
            return res;
        }

        internal void OnViewMouseMove(object sender, MouseEventArgs e, Point2D point)
        {
            lastMouseLocation = point;
            switch (Mode)
            {
                case InputMode.Point:
                    if (currentPointOptions.HasBasePoint)
                        consLine.P2 = lastMouseLocation;
                    currentPointOptions.Jig(lastMouseLocation);
                    break;
                case InputMode.Angle:
                    consLine.P2 = lastMouseLocation;
                    currentAngleOptions.Jig(lastMouseLocation - currentAngleOptions.BasePoint);
                    break;
            }
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
                        angleCompletion.SetResult(new AngleResult(true, point - currentAngleOptions.BasePoint));
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
                        angleCompletion.SetResult(new AngleResult(true, lastMouseLocation - currentAngleOptions.BasePoint));
                    else if (e.KeyCode == Keys.Escape)
                        angleCompletion.SetResult(new AngleResult(false, lastMouseLocation - currentAngleOptions.BasePoint));
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
                        if (lastText.Length > 0)
                            lastText = lastText.Remove(lastText.Length - 1);
                    }
                    else if (!char.IsControl(e.KeyChar))
                    {
                        lastText += e.KeyChar;
                    }
                    currentTextOptions.Jig(lastText);
                    break;
            }
        }

        protected void OnPrompt(EditorPromptEventArgs e)
        {
            Prompt?.Invoke(this, e);
        }
    }
}
