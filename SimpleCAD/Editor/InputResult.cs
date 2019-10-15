namespace SimpleCAD
{
    public enum ResultMode
    {
        OK,
        Keyword,
        Cancel
    }

    internal enum AcceptReason
    {
        None,
        Init,
        Coords,
        Text,
        Keyword
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
        internal AcceptReason AcceptReason { get; private set; }
        internal CancelReason CancelReason { get; private set; }

        internal static InputResult<TInput> CancelResult(CancelReason cancelReason) => new InputResult<TInput>(ResultMode.Cancel, default(TInput), "", AcceptReason.None, cancelReason);
        internal static InputResult<TInput> KeywordResult(string keyword) => new InputResult<TInput>(ResultMode.Keyword, default(TInput), keyword, AcceptReason.Keyword, CancelReason.None);
        internal static InputResult<TInput> AcceptResult(TInput value, AcceptReason acceptReason) => new InputResult<TInput>(ResultMode.OK, value, "", acceptReason, CancelReason.None);

        private InputResult(ResultMode result, TInput value, string keyword, AcceptReason acceptReason, CancelReason cancelReason)
        {
            Result = result;
            Value = value;
            Keyword = keyword;
            AcceptReason = acceptReason;
            CancelReason = cancelReason;
        }
    }
}
