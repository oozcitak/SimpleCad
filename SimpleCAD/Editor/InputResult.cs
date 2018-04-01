using SimpleCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleCAD
{
    public enum ResultMode
    {
        OK,
        Keyword,
        Cancel
    }

    public class InputResult<TInput>
    {
        public ResultMode Result { get; private set; }
        public TInput Value { get; private set; }
        public string Keyword { get; private set; }

        internal static InputResult<TInput> CancelResult() => new InputResult<TInput>(ResultMode.Cancel, default(TInput), "");
        internal static InputResult<TInput> KeywordResult(string keyword) => new InputResult<TInput>(ResultMode.Keyword, default(TInput), keyword);
        internal static InputResult<TInput> AcceptResult(TInput value) => new InputResult<TInput>(ResultMode.OK, value, "");

        private InputResult(ResultMode result, TInput value, string keyword)
        {
            Result = result;
            Value = value;
            Keyword = keyword;
        }
    }
}
