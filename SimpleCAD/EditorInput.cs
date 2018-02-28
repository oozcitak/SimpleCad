using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public partial class Editor
    {
        internal enum InputMode
        {
            None,
            Point,
            Angle,
            Text
        }

        public enum ResultMode
        {
            OK,
            Keyword,
            Cancel
        }

        public class InputResult<T>
        {
            public ResultMode Result { get; private set; }
            public T Value { get; private set; }
            public string Keyword { get; private set; }

            protected InputResult(ResultMode result, T value, string keyword)
            {
                Result = result;
                Value = value;
                Keyword = keyword;
            }
        }

        public class PointResult : InputResult<Point2D>
        {
            internal PointResult(ResultMode result) : base(result, Point2D.Zero, "") { }
            internal PointResult(Point2D value) : base(ResultMode.OK, value, "") { }
            internal PointResult(string keyword) : base(ResultMode.Keyword, Point2D.Zero, keyword) { }
        }

        public class AngleResult : InputResult<Vector2D>
        {
            internal AngleResult(ResultMode result) : base(result, Vector2D.XAxis, "") { }
            internal AngleResult(Vector2D value) : base(ResultMode.OK, value, "") { }
            internal AngleResult(string keyword) : base(ResultMode.Keyword, Vector2D.XAxis, keyword) { }
        }

        public class TextResult : InputResult<string>
        {
            internal TextResult(ResultMode result) : base(result, "", "") { }
            internal TextResult(string value) : base(ResultMode.OK, value, "") { }
        }

        public class InputOptions
        {
            private static Regex upperOnly = new Regex("[^A-Z]", RegexOptions.Compiled);
            public string Message { get; private set; }
            internal List<string> Keywords { get; private set; }
            internal List<string> Aliases { get; private set; }
            internal string DefaultKeyword { get; private set; }

            public InputOptions(string message)
            {
                Message = message;
                Keywords = new List<string>();
                Aliases = new List<string>();
                DefaultKeyword = "";
            }

            public void AddKeyword(string keyword, bool isDefault = false)
            {
                Keywords.Add(keyword);
                string alias = upperOnly.Replace(keyword, "");
                Aliases.Add(alias);

                if (isDefault) SetDefaultKeyword(keyword);
            }

            internal string MatchKeyword(string input)
            {
                if (string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(DefaultKeyword))
                    return DefaultKeyword;
                if (string.IsNullOrEmpty(input))
                    return "";

                for (int i = 0; i < Aliases.Count; i++)
                {
                    if (string.Compare(Aliases[i], input, StringComparison.OrdinalIgnoreCase) == 0)
                        return Keywords[i];
                }
                for (int i = 0; i < Keywords.Count; i++)
                {
                    if (Keywords[i].StartsWith(input, StringComparison.OrdinalIgnoreCase))
                        return Keywords[i];
                }
                return "";
            }

            internal void SetDefaultKeyword(string keyword)
            {
                DefaultKeyword = keyword;
            }

            internal virtual string GetFullPrompt()
            {
                if (Keywords.Count == 0)
                {
                    return Message.TrimEnd(' ', ':') + ": ";
                }
                else
                {
                    StringBuilder sb = new StringBuilder(Message);
                    sb.Append(" [");
                    sb.Append(string.Join(", ", Keywords));
                    sb.Append("]");
                    if (Keywords.Contains(DefaultKeyword))
                    {
                        sb.Append(" <");
                        sb.Append(DefaultKeyword);
                        sb.Append(">");
                    }
                    sb.Append(": ");
                    return sb.ToString();
                }
            }
        }

        public class JigOptions<T> : InputOptions
        {
            public Action<T> Jig { get; private set; }

            public JigOptions(string message) : base(message)
            {
                Jig = (p) => { };
            }

            public JigOptions(string message, Action<T> jig) : base(message)
            {
                Jig = jig;
            }
        }

        public class PointOptions : JigOptions<Point2D>
        {
            public bool HasBasePoint { get; private set; }
            public Point2D BasePoint { get; private set; }

            public PointOptions(string message, Point2D basePoint, Action<Point2D> jig) : base(message, jig)
            {
                HasBasePoint = true;
                BasePoint = basePoint;
            }

            public PointOptions(string message, Point2D basePoint) : base(message)
            {
                HasBasePoint = true;
                BasePoint = basePoint;
            }

            public PointOptions(string message, Action<Point2D> jig) : base(message, jig)
            {
                HasBasePoint = false;
                BasePoint = Point2D.Zero;
            }

            public PointOptions(string message) : base(message)
            {
                HasBasePoint = false;
                BasePoint = Point2D.Zero;
            }
        }

        public class AngleOptions : JigOptions<Vector2D>
        {
            public Point2D BasePoint { get; private set; }

            public AngleOptions(string message, Point2D basePoint, Action<Vector2D> jig) : base(message, jig)
            {
                BasePoint = basePoint;
            }

            public AngleOptions(string message, Point2D basePoint) : base(message)
            {
                BasePoint = basePoint;
            }
        }

        public class TextOptions : JigOptions<string>
        {
            public TextOptions(string message, Action<string> jig) : base(message, jig)
            {
                ;
            }

            public TextOptions(string message) : base(message)
            {
                ;
            }

            internal override string GetFullPrompt()
            {
                return Message.TrimEnd(' ', ':') + ": ";
            }
        }
    }
}
