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

        private TaskCompletionSource<SelectionResult> selectionCompletion;
        private TaskCompletionSource<PointResult> pointCompletion;
        private TaskCompletionSource<AngleResult> angleCompletion;
        private TaskCompletionSource<TextResult> textCompletion;
        private TaskCompletionSource<DistanceResult> distanceCompletion;
        private bool inputCompleted;

        private InputOptions currentOptions;
        private Point2D currentMouseLocation;
        private string currentText;
        private Polyline consLine;
        private Hatch consHatch;
        private bool selectionClickedFirstPoint;

        public SelectionSet Selection { get; private set; } = new SelectionSet();
        internal List<ControlPoint> ControlPoints { get; private set; } = new List<ControlPoint>();

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

        public async Task<SelectionResult> GetSelection(string message)
        {
            return await GetSelection(new SelectionOptions(message));
        }

        public async Task<SelectionResult> GetSelection(SelectionOptions options)
        {
            if (Selection.Count != 0)
            {
                // Immediately return existing selection if any
                selectionCompletion = new TaskCompletionSource<SelectionResult>();
                selectionCompletion.SetResult(new SelectionResult(Selection));
                return await selectionCompletion.Task;
            }
            else
            {
                SelectionResult res = new SelectionResult(ResultMode.Cancel);

                Mode = InputMode.Selection;
                currentOptions = options;
                selectionClickedFirstPoint = false;
                OnPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

                inputCompleted = false;
                while (!inputCompleted)
                {
                    selectionCompletion = new TaskCompletionSource<SelectionResult>();
                    res = await selectionCompletion.Task;
                    Document.Transients.Remove(consLine);
                    Document.Transients.Remove(consHatch);
                }

                Mode = InputMode.None;
                OnPrompt(new EditorPromptEventArgs(""));

                return res;
            }
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
                    consLine = new Polyline(options.BasePoint, options.BasePoint);
                    Document.Jigged.Add(consLine);
                }
                pointCompletion = new TaskCompletionSource<PointResult>();
                res = await pointCompletion.Task;
                Document.Jigged.Remove(consLine);
            }

            Mode = InputMode.None;
            OnPrompt(new EditorPromptEventArgs(""));

            return res;
        }

        public async Task<AngleResult> GetAngle(string message, Point2D basePoint, Action<float> jig)
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
                consLine = new Polyline(options.BasePoint, options.BasePoint);
                Document.Jigged.Add(consLine);
                angleCompletion = new TaskCompletionSource<AngleResult>();
                res = await angleCompletion.Task;
                Document.Jigged.Remove(consLine);
            }

            Mode = InputMode.None;
            OnPrompt(new EditorPromptEventArgs(""));

            return res;
        }

        public async Task<DistanceResult> GetDistance(string message, Point2D basePoint, Action<float> jig)
        {
            return await GetDistance(new DistanceOptions(message, basePoint, jig));
        }

        public async Task<DistanceResult> GetDistance(string message, Point2D basePoint)
        {
            return await GetDistance(new DistanceOptions(message, basePoint));
        }

        public async Task<DistanceResult> GetDistance(DistanceOptions options)
        {
            DistanceResult res = new DistanceResult(ResultMode.Cancel);

            Mode = InputMode.Distance;
            currentText = "";
            currentOptions = options;
            OnPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

            inputCompleted = false;
            while (!inputCompleted)
            {
                consLine = new Polyline(options.BasePoint, options.BasePoint);
                Document.Jigged.Add(consLine);
                distanceCompletion = new TaskCompletionSource<DistanceResult>();
                res = await distanceCompletion.Task;
                Document.Jigged.Remove(consLine);
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
                case InputMode.Selection:
                    if (selectionClickedFirstPoint)
                    {
                        // Update the selection window
                        Point2D p1 = consLine.Points[0];
                        Point2D p2 = new Point2D(point.X, p1.Y);
                        Point2D p3 = point;
                        Point2D p4 = new Point2D(p1.X, point.Y);
                        consLine.Points[0] = p1;
                        consLine.Points[1] = p2;
                        consLine.Points[2] = p3;
                        consLine.Points[3] = p4;
                        consHatch.Points[0] = p1;
                        consHatch.Points[1] = p2;
                        consHatch.Points[2] = p3;
                        consHatch.Points[3] = p4;
                        if (point.X > p1.X)
                        {
                            consHatch.Outline = Outline.SelectionWindowStyle;
                            consLine.Outline = Outline.SelectionBorderStyle;
                        }
                        else
                        {
                            consHatch.Outline = Outline.ReverseSelectionWindowStyle;
                            consLine.Outline = Outline.ReverseSelectionBorderStyle;
                        }
                    }
                    break;
                case InputMode.Point:
                    if (((PointOptions)currentOptions).HasBasePoint)
                        consLine.Points[1] = currentMouseLocation;
                    ((PointOptions)currentOptions).Jig(currentMouseLocation);
                    break;
                case InputMode.Angle:
                    consLine.Points[1] = currentMouseLocation;
                    ((AngleOptions)currentOptions).Jig((currentMouseLocation - ((AngleOptions)currentOptions).BasePoint).Angle);
                    break;
                case InputMode.Distance:
                    consLine.Points[1] = currentMouseLocation;
                    ((DistanceOptions)currentOptions).Jig((currentMouseLocation - ((DistanceOptions)currentOptions).BasePoint).Length);
                    break;
            }
        }

        internal void OnViewMouseClick(object sender, MouseEventArgs e, Point2D point)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (Mode)
                {
                    case InputMode.Selection:
                        if (!selectionClickedFirstPoint)
                        {
                            selectionClickedFirstPoint = true;
                            // Create the selection window
                            consHatch = new Hatch(point, point, point, point);
                            consHatch.Outline = Outline.SelectionWindowStyle;
                            Document.Transients.Add(consHatch);
                            consLine = new Polyline(point, point, point, point);
                            consLine.Closed = true;
                            consLine.Outline = Outline.SelectionBorderStyle;
                            Document.Transients.Add(consLine);
                        }
                        else
                        {
                            inputCompleted = true;
                            SelectionSet set = new SelectionSet();
                            Extents2D ex = consHatch.GetExtents();
                            bool windowSelection = (consHatch.Points[2].X > consHatch.Points[0].X);
                            foreach (Drawable item in Document.Model)
                            {
                                Extents2D exItem = item.GetExtents();
                                if (windowSelection && ex.Contains(exItem) || !windowSelection && ex.IntersectsWith(exItem))
                                    set.Add(item);
                            }
                            Selection = set;
                            selectionCompletion.SetResult(new SelectionResult(set));
                        }
                        break;
                    case InputMode.Point:

                        inputCompleted = true;
                        pointCompletion.SetResult(new PointResult(point));
                        break;
                    case InputMode.Angle:
                        inputCompleted = true;
                        angleCompletion.SetResult(new AngleResult((point - ((AngleOptions)currentOptions).BasePoint).Angle)); ;
                        break;
                    case InputMode.Distance:
                        inputCompleted = true;
                        distanceCompletion.SetResult(new DistanceResult((point - ((DistanceOptions)currentOptions).BasePoint).Length));
                        break;
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
                            angleCompletion.SetResult(new AngleResult(((Vector2D)conv.ConvertFrom(currentText)).Angle));
                        }
                        else if (float.TryParse(currentText, out float angle))
                        {
                            inputCompleted = true;
                            angleCompletion.SetResult(new AngleResult(angle * MathF.PI / 180));
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
                        angleCompletion.SetResult(new AngleResult(ResultMode.Cancel));
                    }
                    break;
                case InputMode.Distance:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return || e.KeyCode == Keys.Space)
                    {
                        if (float.TryParse(currentText, out float dist))
                        {
                            inputCompleted = true;
                            distanceCompletion.SetResult(new DistanceResult(dist));
                        }
                        else if (!string.IsNullOrEmpty(keyword))
                        {
                            inputCompleted = true;
                            distanceCompletion.SetResult(new DistanceResult(keyword));
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
                        angleCompletion.SetResult(new AngleResult(ResultMode.Cancel));
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
