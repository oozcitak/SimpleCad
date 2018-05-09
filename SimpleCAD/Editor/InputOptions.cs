using SimpleCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleCAD
{
    public abstract class InputOptions<T>
    {
        private static Regex upperOnly = new Regex("[^A-Z]", RegexOptions.Compiled);
        public string Message { get; set; }
        internal List<string> Keywords { get; private set; }
        internal List<string> Aliases { get; private set; }
        internal string DefaultKeyword { get; private set; }
        public Action<T> Jig { get; private set; }

        public InputOptions(string message, Action<T> jig)
        {
            Message = message;
            Keywords = new List<string>();
            Aliases = new List<string>();
            DefaultKeyword = "";
            Jig = jig;
        }

        public InputOptions(string message) : this(message, (p) => { }) { }

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
                StringBuilder sb = new StringBuilder(Message.TrimEnd(' ', ':'));
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

    public class FilenameOptions : InputOptions<string>
    {
        public string FileName { get; set; }
        public string Filter { get; set; }
        public string Extension { get; set; }

        public FilenameOptions(string message, string filename, string filter, string ext) : base(message)
        {
            FileName = filename;
            Filter = filter;
            Extension = ext;
        }

        public FilenameOptions(string message, string filename, string filter)
            : this(message, filename, filter, "scf")
        {
            ;
        }

        public FilenameOptions(string message, string filename)
            : this(message, filename, "SimpleCAD file (*.scf)|*.scf|All files (*.*)|*.*", "scf")
        {
            ;
        }

        public FilenameOptions(string message)
            : this(message, "", "SimpleCAD file (*.scf)|*.scf|All files (*.*)|*.*", "scf")
        {
            ;
        }
    }

    public class SelectionOptions : InputOptions<SelectionSet>
    {
        public bool UsePickedSelection { get; set; }

        public SelectionOptions(string message, bool usePickedSelection) : base(message)
        {
            UsePickedSelection = usePickedSelection;
        }

        public SelectionOptions(string message) : this(message, true)
        {
            ;
        }
    }

    public class PointOptions : InputOptions<Point2D>
    {
        public bool HasBasePoint { get; private set; }
        public Point2D BasePoint { get; private set; }

        public PointOptions(string message, Point2D basePoint, Action<Point2D> jig) : base(message, jig)
        {
            HasBasePoint = true;
            BasePoint = basePoint;
        }

        public PointOptions(string message, Point2D basePoint) : this(message, basePoint, (p) => { })
        {
            ;
        }

        public PointOptions(string message, Action<Point2D> jig) : base(message, jig)
        {
            HasBasePoint = false;
            BasePoint = Point2D.Zero;
        }

        public PointOptions(string message) : this(message, (p) => { })
        {
            ;
        }
    }

    public class CornerOptions : InputOptions<Point2D>
    {
        public Point2D BasePoint { get; private set; }

        public CornerOptions(string message, Point2D basePoint, Action<Point2D> jig) : base(message, jig)
        {
            BasePoint = basePoint;
        }

        public CornerOptions(string message, Point2D basePoint) : this(message, basePoint, (p) => { })
        {
            ;
        }
    }

    public class AngleOptions : InputOptions<float>
    {
        public Point2D BasePoint { get; set; }

        public AngleOptions(string message, Point2D basePoint, Action<float> jig) : base(message, jig)
        {
            BasePoint = basePoint;
        }

        public AngleOptions(string message, Point2D basePoint) : this(message, basePoint, (p) => { })
        {
            ;
        }
    }

    public class DistanceOptions : InputOptions<float>
    {
        public Point2D BasePoint { get; set; }

        public DistanceOptions(string message, Point2D basePoint, Action<float> jig) : base(message, jig)
        {
            BasePoint = basePoint;
        }

        public DistanceOptions(string message, Point2D basePoint) : this(message, basePoint, (p) => { })
        {
            ;
        }
    }

    public class TextOptions : InputOptions<string>
    {
        public TextOptions(string message, Action<string> jig) : base(message, jig)
        {
            ;
        }

        public TextOptions(string message) : this(message, (p) => { })
        {
            ;
        }

        internal override string GetFullPrompt()
        {
            return Message.TrimEnd(' ', ':') + ": ";
        }
    }

    public class NumberOptions<T> : InputOptions<T>
    {
        public bool AllowZero { get; set; } = true;
        public bool AllowNegative { get; set; } = true;
        public bool AllowPositive { get; set; } = true;

        public NumberOptions(string message, Action<T> jig) : base(message, jig)
        {
            ;
        }

        public NumberOptions(string message) : this(message, (p) => { })
        {
            ;
        }
    }

    public class IntOptions : NumberOptions<int>
    {
        public IntOptions(string message, Action<int> jig) : base(message, jig)
        {
            ;
        }

        public IntOptions(string message) : this(message, (p) => { })
        {
            ;
        }
    }

    public class FloatOptions : NumberOptions<float>
    {
        public FloatOptions(string message, Action<float> jig) : base(message, jig)
        {
            ;
        }

        public FloatOptions(string message) : this(message, (p) => { })
        {
            ;
        }
    }
}
