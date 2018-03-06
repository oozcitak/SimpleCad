using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCAD
{
    public partial class Editor
    {
        private static Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public CADDocument Document { get; private set; }
        internal InputMode Mode { get; private set; }

        public event EditorPromptEventHandler Prompt;
        internal event CursorPromptEventHandler CursorPrompt;

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

        internal SelectionSet CurrentSelection { get; private set; } = new SelectionSet();
        public SelectionSet PickedSelection { get; private set; } = new SelectionSet();

        static Editor()
        {
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
                        commands.Add(com.RegisteredName, com);
                    }
                }
            }
        }

        public Editor(CADDocument doc)
        {
            Document = doc;
        }

        public void RunCommand(string registeredName, params string[] args)
        {
            if (commands.ContainsKey(registeredName))
            {
                Command com = commands[registeredName];
                Command clearSelection = new Commands.SelectionClear();
                Task runTask = com.Apply(Document, args);
                runTask.ContinueWith(a => clearSelection.Apply(Document, args));
            }
            else
            {
                throw new InvalidOperationException("Unknown command name: " + registeredName);
            }
        }

        public async Task<FilenameResult> GetOpenFilename(string message)
        {
            return await GetOpenFilename(new FilenameOptions(message));
        }

        public async Task<FilenameResult> GetOpenFilename(string message, string filename)
        {
            return await GetOpenFilename(new FilenameOptions(message, filename));
        }

        public async Task<FilenameResult> GetOpenFilename(string message, string filename, string filter)
        {
            return await GetOpenFilename(new FilenameOptions(message, filename, filter));
        }

        public async Task<FilenameResult> GetOpenFilename(string message, string filename, string filter, string ext)
        {
            return await GetOpenFilename(new FilenameOptions(message, filename, filter, ext));
        }

        public async Task<FilenameResult> GetOpenFilename(FilenameOptions options)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = options.Message;
            ofd.Filter = options.Filter;
            ofd.DefaultExt = "scf";
            string filename = "";
            string path = "";
            try
            {
                filename = Path.GetFileName(options.FileName);
                path = Path.GetDirectoryName(options.FileName);
            }
            catch
            {
                ;
            }
            if (!string.IsNullOrEmpty(filename)) ofd.FileName = filename;
            if (!string.IsNullOrEmpty(path)) ofd.InitialDirectory = path;

            TaskCompletionSource<FilenameResult> completion = new TaskCompletionSource<FilenameResult>();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                completion.SetResult(new FilenameResult(ofd.FileName));
            }
            else
            {
                completion.SetResult(new FilenameResult(ResultMode.Cancel));
            }
            return await completion.Task;
        }

        public async Task<FilenameResult> GetSaveFilename(string message)
        {
            return await GetSaveFilename(new FilenameOptions(message));
        }

        public async Task<FilenameResult> GetSaveFilename(string message, string filename)
        {
            return await GetSaveFilename(new FilenameOptions(message, filename));
        }

        public async Task<FilenameResult> GetSaveFilename(string message, string filename, string filter)
        {
            return await GetSaveFilename(new FilenameOptions(message, filename, filter));
        }

        public async Task<FilenameResult> GetSaveFilename(string message, string filename, string filter, string ext)
        {
            return await GetSaveFilename(new FilenameOptions(message, filename, filter, ext));
        }

        public async Task<FilenameResult> GetSaveFilename(FilenameOptions options)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = options.Message;
            sfd.Filter = options.Filter;
            sfd.DefaultExt = "scf";
            string filename = "";
            string path = "";
            try
            {
                filename = Path.GetFileName(options.FileName);
                path = Path.GetDirectoryName(options.FileName);
            }
            catch
            {
                ;
            }
            if (!string.IsNullOrEmpty(filename)) sfd.FileName = filename;
            if (!string.IsNullOrEmpty(path)) sfd.InitialDirectory = path;

            TaskCompletionSource<FilenameResult> completion = new TaskCompletionSource<FilenameResult>();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                completion.SetResult(new FilenameResult(sfd.FileName));
            }
            else
            {
                completion.SetResult(new FilenameResult(ResultMode.Cancel));
            }
            return await completion.Task;
        }

        public async Task<SelectionResult> GetSelection(string message)
        {
            return await GetSelection(new SelectionOptions(message));
        }

        public async Task<SelectionResult> GetSelection(SelectionOptions options)
        {
            if (PickedSelection.Count != 0)
            {
                // Immediately return existing picked-selection if any
                SelectionSet picked = PickedSelection;
                PickedSelection = new SelectionSet();
                selectionCompletion = new TaskCompletionSource<SelectionResult>();
                selectionCompletion.SetResult(new SelectionResult(picked));
                return await selectionCompletion.Task;
            }
            else
            {
                SelectionResult res = new SelectionResult(ResultMode.Cancel);

                Mode = InputMode.Selection;
                currentOptions = options;
                selectionClickedFirstPoint = false;
                OnEditorPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

                inputCompleted = false;
                while (!inputCompleted)
                {
                    selectionCompletion = new TaskCompletionSource<SelectionResult>();
                    res = await selectionCompletion.Task;
                    Document.Transients.Remove(consLine);
                    Document.Transients.Remove(consHatch);
                }

                Mode = InputMode.None;
                OnEditorPrompt(new EditorPromptEventArgs());

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
            OnEditorPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

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
            OnEditorPrompt(new EditorPromptEventArgs());

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
            OnEditorPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

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
            OnEditorPrompt(new EditorPromptEventArgs());

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
            OnEditorPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

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
            OnEditorPrompt(new EditorPromptEventArgs());

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
            OnEditorPrompt(new EditorPromptEventArgs(options.GetFullPrompt()));

            inputCompleted = false;
            while (!inputCompleted)
            {
                textCompletion = new TaskCompletionSource<TextResult>();
                res = await textCompletion.Task;
            }

            Mode = InputMode.None;
            OnEditorPrompt(new EditorPromptEventArgs());

            return res;
        }

        internal void OnViewMouseMove(object sender, CursorEventArgs e)
        {
            currentMouseLocation = e.Location;
            string cursorMessage = "";
            switch (Mode)
            {
                case InputMode.Selection:
                    if (selectionClickedFirstPoint)
                    {
                        // Update the selection window
                        Point2D p1 = consLine.Points[0];
                        Point2D p2 = new Point2D(e.X, p1.Y);
                        Point2D p3 = e.Location;
                        Point2D p4 = new Point2D(p1.X, e.Y);
                        consLine.Points[0] = p1;
                        consLine.Points[1] = p2;
                        consLine.Points[2] = p3;
                        consLine.Points[3] = p4;
                        consHatch.Points[0] = p1;
                        consHatch.Points[1] = p2;
                        consHatch.Points[2] = p3;
                        consHatch.Points[3] = p4;
                        if (e.X > p1.X)
                        {
                            consHatch.Style = new Style(Document.Settings.Get<Color>("SelectionWindowColor"));
                            consLine.Style = new Style(Document.Settings.Get<Color>("SelectionWindowBorderColor"));
                        }
                        else
                        {
                            consHatch.Style = new Style(Document.Settings.Get<Color>("ReverseSelectionWindowColor"));
                            consLine.Style = new Style(Document.Settings.Get<Color>("SelectionWindowBorderColor"), 0, DashStyle.Dash);
                        }
                        cursorMessage = p1.ToString(Document.Settings.NumberFormat) + " - " + currentMouseLocation.ToString(Document.Settings.NumberFormat);
                        OnCursorPrompt(new CursorPromptEventArgs(cursorMessage));
                    }
                    else
                    {
                        cursorMessage = currentMouseLocation.ToString(Document.Settings.NumberFormat);
                        OnCursorPrompt(new CursorPromptEventArgs(cursorMessage));
                    }
                    break;
                case InputMode.Point:
                    if (((PointOptions)currentOptions).HasBasePoint)
                        consLine.Points[1] = currentMouseLocation;
                    cursorMessage = currentMouseLocation.ToString(Document.Settings.NumberFormat);
                    OnCursorPrompt(new CursorPromptEventArgs(cursorMessage));
                    ((PointOptions)currentOptions).Jig(currentMouseLocation);
                    break;
                case InputMode.Angle:
                    consLine.Points[1] = currentMouseLocation;
                    float angle = (currentMouseLocation - ((AngleOptions)currentOptions).BasePoint).Angle;
                    cursorMessage = angle.ToString("F", Document.Settings.NumberFormat);
                    OnCursorPrompt(new CursorPromptEventArgs(cursorMessage));
                    ((AngleOptions)currentOptions).Jig(angle);
                    break;
                case InputMode.Distance:
                    consLine.Points[1] = currentMouseLocation;
                    float dist = (currentMouseLocation - ((DistanceOptions)currentOptions).BasePoint).Length;
                    cursorMessage = dist.ToString("F", Document.Settings.NumberFormat);
                    OnCursorPrompt(new CursorPromptEventArgs(cursorMessage));
                    ((DistanceOptions)currentOptions).Jig(dist);
                    break;
            }
        }

        internal void OnViewMouseClick(object sender, CursorEventArgs e)
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
                            consHatch = new Hatch(e.Location, e.Location, e.Location, e.Location);
                            Document.Transients.Add(consHatch);
                            consLine = new Polyline(e.Location, e.Location, e.Location, e.Location);
                            consLine.Closed = true;
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
                            CurrentSelection = set;
                            OnCursorPrompt(new CursorPromptEventArgs());
                            selectionCompletion.SetResult(new SelectionResult(set));
                        }
                        break;
                    case InputMode.Point:
                        inputCompleted = true;
                        OnCursorPrompt(new CursorPromptEventArgs());
                        pointCompletion.SetResult(new PointResult(e.Location));
                        break;
                    case InputMode.Angle:
                        inputCompleted = true;
                        OnCursorPrompt(new CursorPromptEventArgs());
                        angleCompletion.SetResult(new AngleResult((e.Location - ((AngleOptions)currentOptions).BasePoint).Angle)); ;
                        break;
                    case InputMode.Distance:
                        inputCompleted = true;
                        OnCursorPrompt(new CursorPromptEventArgs());
                        distanceCompletion.SetResult(new DistanceResult((e.Location - ((DistanceOptions)currentOptions).BasePoint).Length));
                        break;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Right click equals return key
                OnViewKeyDown(sender, new KeyEventArgs(Keys.Return));
            }
        }

        internal void OnViewKeyDown(object sender, KeyEventArgs e)
        {
            string keyword = currentOptions.MatchKeyword(currentText);
            switch (Mode)
            {
                case InputMode.Selection:
                    if (e.KeyCode == Keys.Escape)
                    {
                        inputCompleted = true;
                        OnCursorPrompt(new CursorPromptEventArgs());
                        selectionCompletion.SetResult(new SelectionResult(ResultMode.Cancel));
                    }
                    break;
                case InputMode.Point:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return || e.KeyCode == Keys.Space)
                    {
                        Point2DConverter conv = new Point2DConverter();
                        if (conv.IsValid(currentText))
                        {
                            inputCompleted = true;
                            OnCursorPrompt(new CursorPromptEventArgs());
                            pointCompletion.SetResult(new PointResult((Point2D)conv.ConvertFrom(currentText)));
                        }
                        else if (!string.IsNullOrEmpty(keyword))
                        {
                            inputCompleted = true;
                            OnCursorPrompt(new CursorPromptEventArgs());
                            pointCompletion.SetResult(new PointResult(keyword));
                        }
                        else
                        {
                            currentText = "";
                            OnEditorPrompt(new EditorPromptEventArgs(currentOptions.GetFullPrompt() + "*Invalid input*"));
                        }
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        inputCompleted = true;
                        OnCursorPrompt(new CursorPromptEventArgs());
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
                            OnCursorPrompt(new CursorPromptEventArgs());
                            angleCompletion.SetResult(new AngleResult(((Vector2D)conv.ConvertFrom(currentText)).Angle));
                        }
                        else if (float.TryParse(currentText, out float angle))
                        {
                            inputCompleted = true;
                            OnCursorPrompt(new CursorPromptEventArgs());
                            angleCompletion.SetResult(new AngleResult(angle));
                        }
                        else if (!string.IsNullOrEmpty(keyword))
                        {
                            inputCompleted = true;
                            OnCursorPrompt(new CursorPromptEventArgs());
                            angleCompletion.SetResult(new AngleResult(keyword));
                        }
                        else
                        {
                            currentText = "";
                            OnEditorPrompt(new EditorPromptEventArgs(currentOptions.GetFullPrompt() + "*Invalid input*"));
                        }
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        inputCompleted = true;
                        OnCursorPrompt(new CursorPromptEventArgs());
                        angleCompletion.SetResult(new AngleResult(ResultMode.Cancel));
                    }
                    break;
                case InputMode.Distance:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return || e.KeyCode == Keys.Space)
                    {
                        if (float.TryParse(currentText, out float dist))
                        {
                            inputCompleted = true;
                            OnCursorPrompt(new CursorPromptEventArgs());
                            distanceCompletion.SetResult(new DistanceResult(dist));
                        }
                        else if (!string.IsNullOrEmpty(keyword))
                        {
                            inputCompleted = true;
                            OnCursorPrompt(new CursorPromptEventArgs());
                            distanceCompletion.SetResult(new DistanceResult(keyword));
                        }
                        else
                        {
                            currentText = "";
                            OnEditorPrompt(new EditorPromptEventArgs(currentOptions.GetFullPrompt() + "*Invalid input*"));
                        }
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        inputCompleted = true;
                        OnCursorPrompt(new CursorPromptEventArgs());
                        distanceCompletion.SetResult(new DistanceResult(ResultMode.Cancel));
                    }
                    break;
                case InputMode.Text:
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                    {
                        inputCompleted = true;
                        OnCursorPrompt(new CursorPromptEventArgs());
                        textCompletion.SetResult(new TextResult(currentText));
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        inputCompleted = true;
                        OnCursorPrompt(new CursorPromptEventArgs());
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
                OnCursorPrompt(new CursorPromptEventArgs(currentText));

                if (Mode == InputMode.Text)
                    ((TextOptions)currentOptions).Jig(currentText);
            }
        }

        protected void OnEditorPrompt(EditorPromptEventArgs e)
        {
            Prompt?.Invoke(this, e);
        }

        internal void OnCursorPrompt(CursorPromptEventArgs e)
        {
            CursorPrompt?.Invoke(this, e);
        }
    }
}
