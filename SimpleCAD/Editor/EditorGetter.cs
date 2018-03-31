using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCAD
{
    public abstract class EditorGetter<TOptions, TResult, TValue> where TOptions : InputOptions where TResult : InputResult<TValue>
    {
        private Drawable jigged = null;
        private string currentText = "";

        protected Editor Editor { get; private set; }
        protected TOptions Options { get; private set; }
        protected Drawable Jigged
        {
            get { return jigged; }
            set
            {
                if (jigged != null)
                    Editor.Document.Jigged.Remove(jigged);
                jigged = value;
                if (jigged != null)
                    Editor.Document.Jigged.Add(jigged);
            }
        }
        protected bool SpaceAccepts { get; set; } = true;
        protected TaskCompletionSource<TResult> Completion { get; private set; }

        public async Task<TResult> Run(Editor editor, TOptions options)
        {
            Editor = editor;
            Options = options;
            Editor.DoPrompt("");

            Editor.CursorMove += Editor_CursorMove;
            Editor.CursorClick += Editor_CursorClick;
            Editor.KeyDown += Editor_KeyDown;
            Editor.KeyPress += Editor_KeyPress;

            currentText = "";
            Init();
            Completion = new TaskCompletionSource<TResult>();
            TResult result = await Completion.Task;

            Editor.DoPrompt("");
            Jigged = null;

            return result;
        }

        private void Editor_CursorMove(object sender, CursorEventArgs e)
        {
            MouseCoordsChanged(e.Location);
        }

        private void Editor_CursorClick(object sender, CursorEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                bool inputCompleted = AcceptMouseClick(e.Location, out TValue value, out string errorMessage);
                if (inputCompleted)
                {
                    Editor.DoPrompt("");
                    TResult result = (TResult)typeof(TResult).GetConstructor(
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                        null, new Type[] { typeof(TValue) }, null).Invoke(new object[] { value });
                    Completion.SetResult(result);
                }
                else
                {
                    currentText = "";
                    Editor.DoPrompt(Options.GetFullPrompt() + errorMessage);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Right click equals return key
                Editor_KeyDown(sender, new KeyEventArgs(Keys.Return));
            }
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Editor.DoPrompt("");
                CancelInput();
                Completion.SetCanceled();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return || (SpaceAccepts && e.KeyCode == Keys.Space))
            {
                string keyword = Options.MatchKeyword(currentText);

                if (!string.IsNullOrEmpty(keyword))
                {
                    Editor.DoPrompt("");
                    TResult result = (TResult)typeof(TResult).GetConstructor(
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                        null, new Type[] { typeof(string) }, null).Invoke(new object[] { keyword });
                    Completion.SetResult(result);
                }
                else
                {
                    bool inputCompleted = AcceptInput(currentText, out TValue value, out string errorMessage);
                    if (inputCompleted)
                    {
                        Editor.DoPrompt("");
                        TResult result = (TResult)typeof(TResult).GetConstructor(
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                            null, new Type[] { typeof(TValue) }, null).Invoke(new object[] { value });
                        Completion.SetResult(result);
                    }
                    else
                    {
                        currentText = "";
                        Editor.DoPrompt(Options.GetFullPrompt() + errorMessage);
                    }
                }
            }
        }

        private void Editor_KeyPress(object sender, KeyPressEventArgs e)
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
            else if (e.KeyChar == ' ' && !SpaceAccepts)
            {
                currentText += e.KeyChar;
                textChanged = true;
            }
            else if (!char.IsControl(e.KeyChar))
            {
                currentText += e.KeyChar;
                textChanged = true;
            }

            if (textChanged)
            {
                Editor.DoPrompt(Options.GetFullPrompt() + currentText);
                TextChanged(currentText);
            }
        }

        protected virtual void Init() { }
        protected virtual void MouseCoordsChanged(Point2D pt) { }
        protected virtual void TextChanged(string text) { }
        protected virtual bool AcceptMouseClick(Point2D pt, out TValue value, out string errorMessage)
        {
            value = default(TValue);
            errorMessage = "*Invalid input*";
            return true;
        }
        protected virtual bool AcceptInput(string text, out TValue value, out string errorMessage)
        {
            value = default(TValue);
            errorMessage = "*Invalid input*";
            return true;
        }
        protected virtual void CancelInput() { }
    }

    public class PointGetter : EditorGetter<PointOptions, PointResult, Point2D>
    {
        protected override void Init()
        {
            if (Options.HasBasePoint)
            {
                Jigged = new Line(Options.BasePoint, Options.BasePoint);
            }
        }

        protected override void MouseCoordsChanged(Point2D pt)
        {
            if (Options.HasBasePoint)
                (Jigged as Line).EndPoint = pt;
            string cursorMessage = pt.ToString(Editor.Document.Settings.NumberFormat);
            Editor.DoPrompt(Options.GetFullPrompt() + cursorMessage);
            Options.Jig(pt);
        }

        protected override bool AcceptMouseClick(Point2D pt, out Point2D value, out string errorMessage)
        {
            value = pt;
            errorMessage = "";
            return true;
        }

        protected override bool AcceptInput(string text, out Point2D value, out string errorMessage)
        {
            if (Point2D.TryParse(text, out value))
            {
                errorMessage = "";
                return true;
            }

            return base.AcceptInput(text, out value, out errorMessage);
        }
    }
}
