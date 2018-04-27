namespace SimpleCAD
{
    internal class InitArgs<TValue>
    {
        public TValue Value;
        public string ErrorMessage;
        public bool InputValid;
        public bool ContinueAsync;

        public InitArgs()
        {
            Value = default(TValue);
            InputValid = true;
            ContinueAsync = true;
            ErrorMessage = "*Invalid input*";
        }
    }
}
