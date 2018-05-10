namespace SimpleCAD
{
    public enum ResultMode
    {
        OK,
        Keyword,
        Cancel
    }

    internal enum CancelReason
    {
        None,
        Init,
        Escape,
        Space,
        Enter
    }

    public class InputResult<TInput>
    {
        public ResultMode Result { get; private set; }
        public TInput Value { get; private set; }
        public string Keyword { get; private set; }
        internal CancelReason CancelReason { get; private set; }

        internal static InputResult<TInput> CancelResult(CancelReason cancelReason) => new InputResult<TInput>(ResultMode.Cancel, default(TInput), "", cancelReason);
        internal static InputResult<TInput> KeywordResult(string keyword) => new InputResult<TInput>(ResultMode.Keyword, default(TInput), keyword);
        internal static InputResult<TInput> AcceptResult(TInput value) => new InputResult<TInput>(ResultMode.OK, value, "");

        private InputResult(ResultMode result, TInput value, string keyword, CancelReason cancelReason = CancelReason.None)
        {
            Result = result;
            Value = value;
            Keyword = keyword;
            CancelReason = cancelReason;
        }
    }
}
