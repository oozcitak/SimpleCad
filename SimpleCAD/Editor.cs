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
        private bool inputCompleted;

        private InputOptions currentOptions;
        private Point2D currentMouseLocation;
        private string currentText;
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
            PointResult res = new PointResult(ResultMode.Cancel);

            Mode = InputMode.Point;
            currentText = "";
            currentOptions = options;
            OnPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

            inputCompleted = false;
            while (!inputCompleted)
            {
                if (options.HasBasePoint)
                {
                    consLine = new Line(options.BasePoint, options.BasePoint);
                    consLine.OutlineStyle = TransientStyle;
                    Document.Transients.Add(consLine);
                }
                pointCompletion = new TaskCompletionSource<PointResult>();
                res = await pointCompletion.Task;
                Document.Transients.Remove(consLine);
            }

            Mode = InputMode.None;
            OnPrompt(new EditorPromptEventArgs(""));

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
            AngleResult res = new AngleResult(ResultMode.Cancel);

            Mode = InputMode.Angle;
            currentText = "";
            currentOptions = options;
            OnPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

            inputCompleted = false;
            while (!inputCompleted)
            {
                consLine = new Line(options.BasePoint, options.BasePoint);
                consLine.OutlineStyle = TransientStyle;
                Document.Transients.Add(consLine);
                angleCompletion = new TaskCompletionSource<AngleResult>();
                res = await angleCompletion.Task;
                Document.Transients.Remove(consLine);
            }

            Mode = InputMode.None;
            OnPrompt(new EditorPromptEventArgs(""));

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
            TextResult res = new TextResult(ResultMode.Cancel);

            Mode = InputMode.Text;
            currentText = "";
            currentOptions = options;
            OnPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

            inputCompleted = false;
            while (!inputCompleted)
            {
                textCompletion = new TaskCompletionSource<TextResult>();
                res = await textCompletion.Task;
            }

            Mode = InputMode.None;
            OnPrompt(new EditorPromptEventArgs(""));

            return res;
        }

        internal void OnViewMouseMove(object sender, MouseEventArgs e, Point2D point)
        {
            currentMouseLocation = point;
            switch (Mode)
            {
                case InputMode.Point:
                    if (((PointOptions)currentOptions).HasBasePoint)
                        consLine.P2 = currentMouseLocation;
                    ((PointOptions)currentOptions).Jig(currentMouseLocation);
                    break;
                case InputMode.Angle:
                    consLine.P2 = currentMouseLocation;
                    ((AngleOptions)currentOptions).Jig(currentMouseLocation - ((AngleOptions)currentOptions).BasePoint);
                    break;
            }
        }

        internal void OnViewMouseClick(object sender, MouseEventArgs e, Point2D point)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Mode == InputMode.Point)
                {
                    inputCompleted = true;
                    pointCompletion.SetResult(new PointResult(point));
                }
                else if (Mode == InputMode.Angle)
                {
                    inputCompleted = true;
                    angleCompletion.SetResult(new AngleResult(point - ((AngleOptions)currentOptions).BasePoint));
                }
            }
        }

        internal void OnViewKeyDown(object sender, KeyEventArgs e)
        {
            string keyword = currentOptions.MatchKeyword(currentText);
            switch (Mode)
            {
                case InputMode.Point:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return || e.KeyCode == Keys.Space)
                    {
                        Point2DConverter conv = new Point2DConverter();
                        if (conv.IsValid(currentText))
                        {
                            inputCompleted = true;
                            pointCompletion.SetResult(new PointResult((Point2D)conv.ConvertFrom(currentText)));
                        }
                        else if (!string.IsNullOrEmpty(keyword))
                        {
                            inputCompleted = true;
                            pointCompletion.SetResult(new PointResult(keyword));
                        }
                        else
                        {
                            currentText = "";
                            OnPrompt(new EditorPromptEventArgs(currentOptions.GetFullPrompt() + "*Invalid input*"));
                        }
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        inputCompleted = true;
                        pointCompletion.SetResult(new PointResult(ResultMode.Cancel));
                    }
                    break;
                case InputMode.Angle:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return || e.KeyCode == Keys.Space)
                    {
                        Vector2DConverter conv = new Vector2DConverter();
                        if (conv.IsValid(currentText))
                        {
                            inputCompleted = true;
                            angleCompletion.SetResult(new AngleResult((Vector2D)conv.ConvertFrom(currentText)));
                        }
                        if (float.TryParse(currentText, out float angle))
                        {
                            inputCompleted = true;
                            angleCompletion.SetResult(new AngleResult(Vector2D.FromAngle(angle * MathF.PI / 180)));
                        }
                        else if (!string.IsNullOrEmpty(keyword))
                        {
                            inputCompleted = true;
                            angleCompletion.SetResult(new AngleResult(keyword));
                        }
                        else
                        {
                            currentText = "";
                            OnPrompt(new EditorPromptEventArgs(currentOptions.GetFullPrompt() + "*Invalid input*"));
                        }
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        inputCompleted = true;
                        pointCompletion.SetResult(new PointResult(ResultMode.Cancel));
                    }
                    break;
                case InputMode.Text:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                    {
                        inputCompleted = true;
                        textCompletion.SetResult(new TextResult(currentText));
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        inputCompleted = true;
                        textCompletion.SetResult(new TextResult(ResultMode.Cancel));
                    }
                    break;
            }
        }

        internal void OnViewKeyPress(object sender, KeyPressEventArgs e)
        {
            bool textChanged = false;

            if (e.KeyChar == '\b') // backspace
            {
                if (currentText.Length > 0)
                {
                    currentText = currentText.Remove(currentText.Length - 1);
                    textChanged = true;
                }
            }
            else if (e.KeyChar == ' ')
            {
                if (Mode == InputMode.Text)
                {
                    currentText += e.KeyChar;
                    textChanged = true;
                }
            }
            else if (!char.IsControl(e.KeyChar))
            {
                currentText += e.KeyChar;
                textChanged = true;
            }

            if (textChanged)
            {
                OnPrompt(new EditorPromptEventArgs(currentOptions.GetFullPrompt() + currentText));

                if (Mode == InputMode.Text)
                    ((TextOptions)currentOptions).Jig(currentText);
            }
        }

        protected void OnPrompt(EditorPromptEventArgs e)
        {
            Prompt?.Invoke(this, e);
        }
    }
}
