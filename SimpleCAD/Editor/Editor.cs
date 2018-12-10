using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleCAD.Geometry;

namespace SimpleCAD
{
    public class Editor
    {
        private static Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public CADDocument Document { get; private set; }
        internal bool InputMode { get; set; } = false;

        internal SelectionSet CurrentSelection { get; private set; } = new SelectionSet();
        public SelectionSet PickedSelection { get; private set; } = new SelectionSet();

        public SnapPointType SnapMode { get => Document.Settings.SnapMode; }
        internal SnapPointCollection SnapPoints { get; set; } = new SnapPointCollection();

        internal string LastCommandName { get; private set; } = string.Empty;
        internal string[] LastCommandArgs { get; private set; } = new string[0];

        public bool CommandInProgress { get; private set; } = false;

        static Editor()
        {
            // Search the assembly for commands
            Assembly assembly = Assembly.GetAssembly(typeof(CADDocument));
            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType == typeof(Command))
                {
                    Command com = (Command)Activator.CreateInstance(type);
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

        public void DoPrompt(string message)
        {
            OnPrompt(new EditorPromptEventArgs(message));
        }

        public void RunCommand(string registeredName, params string[] args)
        {
            if (commands.ContainsKey(registeredName))
            {
                CommandInProgress = true;
                LastCommandName = registeredName;
                LastCommandArgs = args;

                Command com = commands[registeredName];
                Command clearSelection = new Commands.SelectionClear();
                Task runTask = com.Apply(Document, args).ContinueWith(
                    (t) =>
                    {
                        if (t.IsFaulted)
                            OnError(new EditorErrorEventArgs(t.Exception));
                        else if (t.IsCompleted)
                            clearSelection.Apply(Document, args);
                    }
                ).ContinueWith(
                    (t) => 
                    {
                        CommandInProgress = false;
                    }
                );
            }
            else
            {
                OnError(new EditorErrorEventArgs(new InvalidOperationException("Unknown command name: " + registeredName)));
            }
        }

        public void RepeatCommand()
        {
            if (!string.IsNullOrEmpty(LastCommandName))
            {
                RunCommand(LastCommandName, LastCommandArgs);
            }
        }

        #region Helper Functions
        public string AngleToString(float angle)
        {
            switch (Document.Settings.AngleMode)
            {
                case AngleMode.Radians:
                    return angle.ToString(Document.Settings.NumberFormat);
                case AngleMode.Degrees:
                    return (angle * 180f / MathF.PI).ToString(Document.Settings.NumberFormat);
                case AngleMode.Grads:
                    return (angle * 200f / MathF.PI).ToString(Document.Settings.NumberFormat);
                case AngleMode.DegreesMinutesSeconds:
                    float totalDegrees = angle * 180f / MathF.PI;
                    float degrees = MathF.Floor(totalDegrees);
                    float totalMinutes = (totalDegrees - degrees) * 60f;
                    float minutes = MathF.Floor(totalMinutes);
                    float totalSeconds = (totalMinutes - minutes) * 60f;
                    return string.Format("{0:0}° {1:0}' {2}\"", degrees, minutes, totalSeconds.ToString(Document.Settings.NumberFormat));
                case AngleMode.Surveyor:
                    if (angle < MathF.PI / 2) // NE
                        return string.Format("N {0} E", ((MathF.PI / 2f - angle) * 180f / MathF.PI).ToString(Document.Settings.NumberFormat));
                    else if (angle < MathF.PI) // NW
                        return string.Format("N {0} W", ((angle - MathF.PI / 2f) * 180f / MathF.PI).ToString(Document.Settings.NumberFormat));
                    else if (angle < 3f * MathF.PI / 2f) // SW
                        return string.Format("S {0} W", ((3f * MathF.PI / 2f - angle) * 180f / MathF.PI).ToString(Document.Settings.NumberFormat));
                    else  // SE
                        return string.Format("S {0} E", ((angle - 3f * MathF.PI / 2f) * 180f / MathF.PI).ToString(Document.Settings.NumberFormat));
                default:
                    return angle.ToString(Document.Settings.NumberFormat);
            }
        }

        public bool TryAngleFromString(string str, out float angle)
        {
            str = str.Replace(" ", "");
            angle = 0f;

            switch (Document.Settings.AngleMode)
            {
                case AngleMode.Radians:
                    return float.TryParse(str, out angle);
                case AngleMode.Degrees:
                    if (float.TryParse(str, out angle))
                    {
                        angle = angle * MathF.PI / 180f;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case AngleMode.Grads:
                    if (float.TryParse(str, out angle))
                    {
                        angle = angle * MathF.PI / 200f;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case AngleMode.DegreesMinutesSeconds:
                    float degrees = 0f;
                    float minutes = 0f;
                    float seconds = 0f;

                    int degreeSign = str.IndexOf('°');
                    if (degreeSign != -1)
                    {
                        if (!float.TryParse(str.Substring(0, degreeSign), out degrees))
                            return false;
                        str = str.Substring(degreeSign + 1);
                    }
                    int minuteSign = str.IndexOf('\'');
                    if (minuteSign != -1)
                    {
                        if (!float.TryParse(str.Substring(0, minuteSign), out minutes))
                            return false;
                        str = str.Substring(minuteSign + 1);
                    }
                    int secondSign = str.IndexOf('"');
                    if (secondSign != -1)
                    {
                        if (!float.TryParse(str.Substring(0, secondSign), out seconds))
                            return false;
                        str = str.Substring(secondSign + 1);
                    }

                    angle = (degrees + minutes / 60f + seconds / 3600f) * MathF.PI / 180f;
                    return true;
                case AngleMode.Surveyor:
                    if (str.Length < 3) return false;
                    bool fromNorth = str.StartsWith("N");
                    bool fromSouth = str.StartsWith("S");
                    if (!fromNorth && !fromSouth) return false;
                    bool toWest = str.EndsWith("W");
                    bool toEast = str.EndsWith("E");
                    if (!toWest && !toEast) return false;
                    if (float.TryParse(str.Substring(1, str.Length - 2), out angle))
                    {
                        angle = angle * MathF.PI / 180f;
                        if (fromNorth && toEast) // NE
                            angle = MathF.PI / 2f - angle;
                        else if (fromNorth && toWest) // NW
                            angle = angle + MathF.PI / 2f;
                        else if (fromSouth && toWest) // SW
                            angle = 3f * MathF.PI / 2f - angle;
                        else  // SE
                            angle = angle + 3f * MathF.PI / 2f;
                        return true;
                    }
                    return false;
                default:
                    return float.TryParse(str, out angle);
            }
        }
        #endregion

        #region Editor Getters
        public async Task<InputResult<string>> GetOpenFilename(string message)
        {
            return await GetOpenFilename(new FilenameOptions(message));
        }

        public async Task<InputResult<string>> GetOpenFilename(string message, string filename)
        {
            return await GetOpenFilename(new FilenameOptions(message, filename));
        }

        public async Task<InputResult<string>> GetOpenFilename(string message, string filename, string filter)
        {
            return await GetOpenFilename(new FilenameOptions(message, filename, filter));
        }

        public async Task<InputResult<string>> GetOpenFilename(string message, string filename, string filter, string ext)
        {
            return await GetOpenFilename(new FilenameOptions(message, filename, filter, ext));
        }

        public async Task<InputResult<string>> GetOpenFilename(FilenameOptions options)
        {
            return await OpenFilenameGetter.Run<OpenFilenameGetter>(this, options);
        }

        public async Task<InputResult<string>> GetSaveFilename(string message)
        {
            return await GetSaveFilename(new FilenameOptions(message));
        }

        public async Task<InputResult<string>> GetSaveFilename(string message, string filename)
        {
            return await GetSaveFilename(new FilenameOptions(message, filename));
        }

        public async Task<InputResult<string>> GetSaveFilename(string message, string filename, string filter)
        {
            return await GetSaveFilename(new FilenameOptions(message, filename, filter));
        }

        public async Task<InputResult<string>> GetSaveFilename(string message, string filename, string filter, string ext)
        {
            return await GetSaveFilename(new FilenameOptions(message, filename, filter, ext));
        }

        public async Task<InputResult<string>> GetSaveFilename(FilenameOptions options)
        {
            return await OpenFilenameGetter.Run<OpenFilenameGetter>(this, options);
        }

        public async Task<InputResult<SelectionSet>> GetSelection(string message)
        {
            return await GetSelection(new SelectionOptions(message));
        }

        public async Task<InputResult<SelectionSet>> GetSelection(SelectionOptions options)
        {
            CurrentSelection.Clear();

            while (true)
            {
                var result = await SelectionGetter.Run<SelectionGetter>(this, options);
                if (result.Result == ResultMode.Cancel && (result.CancelReason == CancelReason.Escape || result.CancelReason == CancelReason.Init))
                {
                    return result;
                }
                else if (result.Result == ResultMode.Cancel && (result.CancelReason == CancelReason.Enter || result.CancelReason == CancelReason.Space))
                {
                    return InputResult<SelectionSet>.AcceptResult(CurrentSelection, AcceptReason.Coords);
                }
                else if (result.Result == ResultMode.OK)
                {
                    CurrentSelection.UnionWith(result.Value);
                }

                if (result.Result == ResultMode.OK && result.AcceptReason == AcceptReason.Init)
                    return InputResult<SelectionSet>.AcceptResult(CurrentSelection, AcceptReason.Init);
            }
        }

        public async Task<InputResult<CPSelectionSet>> GetControlPoints(string message)
        {
            return await GetControlPoints(new CPSelectionOptions(message));
        }

        public async Task<InputResult<CPSelectionSet>> GetControlPoints(CPSelectionOptions options)
        {
            CurrentSelection.Clear();

            CPSelectionSet ss = new CPSelectionSet();
            while (true)
            {
                var result = await CPSelectionGetter.Run<CPSelectionGetter>(this, options);
                if (result.Result == ResultMode.Cancel && result.CancelReason == CancelReason.Escape)
                {
                    return result;
                }
                else if (result.Result == ResultMode.Cancel && (result.CancelReason == CancelReason.Enter || result.CancelReason == CancelReason.Space))
                {
                    CurrentSelection = ss.ToSelectionSet();
                    return InputResult<CPSelectionSet>.AcceptResult(ss, AcceptReason.Coords);
                }
                else if (result.Result == ResultMode.OK)
                {
                    ss.UnionWith(result.Value);
                    CurrentSelection = ss.ToSelectionSet();
                }
            }
        }

        public async Task<InputResult<Point2D>> GetPoint(string message)
        {
            return await GetPoint(new PointOptions(message));
        }

        public async Task<InputResult<Point2D>> GetPoint(string message, Action<Point2D> jig)
        {
            return await GetPoint(new PointOptions(message, jig));
        }

        public async Task<InputResult<Point2D>> GetPoint(string message, Point2D basePoint)
        {
            return await GetPoint(new PointOptions(message, basePoint));
        }

        public async Task<InputResult<Point2D>> GetPoint(string message, Point2D basePoint, Action<Point2D> jig)
        {
            return await GetPoint(new PointOptions(message, basePoint, jig));
        }

        public async Task<InputResult<Point2D>> GetPoint(PointOptions options)
        {
            return await PointGetter.Run<PointGetter>(this, options);
        }

        public async Task<InputResult<Point2D>> GetCorner(string message, Point2D basePoint)
        {
            return await GetCorner(new CornerOptions(message, basePoint));
        }

        public async Task<InputResult<Point2D>> GetCorner(string message, Point2D basePoint, Action<Point2D> jig)
        {
            return await GetCorner(new CornerOptions(message, basePoint, jig));
        }

        public async Task<InputResult<Point2D>> GetCorner(CornerOptions options)
        {
            return await CornerGetter.Run<CornerGetter>(this, options);
        }

        public async Task<InputResult<float>> GetAngle(string message, Point2D basePoint, Action<float> jig)
        {
            return await GetAngle(new AngleOptions(message, basePoint, jig));
        }

        public async Task<InputResult<float>> GetAngle(string message, Point2D basePoint)
        {
            return await GetAngle(new AngleOptions(message, basePoint));
        }

        public async Task<InputResult<float>> GetAngle(AngleOptions options)
        {
            return await AngleGetter.Run<AngleGetter>(this, options);
        }

        public async Task<InputResult<float>> GetDistance(string message, Point2D basePoint, Action<float> jig)
        {
            return await GetDistance(new DistanceOptions(message, basePoint, jig));
        }

        public async Task<InputResult<float>> GetDistance(string message, Point2D basePoint)
        {
            return await GetDistance(new DistanceOptions(message, basePoint));
        }

        public async Task<InputResult<float>> GetDistance(DistanceOptions options)
        {
            return await DistanceGetter.Run<DistanceGetter>(this, options);
        }

        public async Task<InputResult<string>> GetText(string message, Action<string> jig)
        {
            return await GetText(new TextOptions(message, jig));
        }

        public async Task<InputResult<string>> GetText(string message)
        {
            return await GetText(new TextOptions(message));
        }

        public async Task<InputResult<string>> GetText(TextOptions options)
        {
            return await TextGetter.Run<TextGetter>(this, options);
        }

        public async Task<InputResult<int>> GetInt(string message, Action<int> jig)
        {
            return await GetInt(new IntOptions(message, jig));
        }

        public async Task<InputResult<int>> GetInt(IntOptions options)
        {
            return await IntGetter.Run<IntGetter>(this, options);
        }

        public async Task<InputResult<float>> GetFloat(string message, Action<float> jig)
        {
            return await GetFloat(new FloatOptions(message, jig));
        }

        public async Task<InputResult<float>> GetFloat(FloatOptions options)
        {
            return await FloatGetter.Run<FloatGetter>(this, options);
        }
        #endregion

        #region View Events
        internal event CursorEventHandler CursorMove;
        internal event CursorEventHandler CursorClick;
        internal event KeyEventHandler KeyDown;
        internal event KeyPressEventHandler KeyPress;

        internal void OnViewMouseMove(object sender, CursorEventArgs e)
        {
            if (CommandInProgress)
                CursorMove?.Invoke(sender, e);
        }

        internal void OnViewMouseClick(object sender, CursorEventArgs e)
        {
            if (CommandInProgress)
                CursorClick?.Invoke(sender, e);
        }

        internal void OnViewKeyDown(object sender, KeyEventArgs e)
        {
            if (CommandInProgress)
                KeyDown?.Invoke(sender, e);
        }

        internal void OnViewKeyPress(object sender, KeyPressEventArgs e)
        {
            if (CommandInProgress)
                KeyPress?.Invoke(sender, e);
        }
        #endregion

        #region Events
        public event EditorPromptEventHandler Prompt;
        public event EditorErrorEventHandler Error;

        protected void OnPrompt(EditorPromptEventArgs e)
        {
            Prompt?.Invoke(this, e);
        }

        protected void OnError(EditorErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }
        #endregion
    }
}
