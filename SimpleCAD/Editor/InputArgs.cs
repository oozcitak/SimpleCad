using SimpleCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleCAD
{
    internal class InputArgs<TInput, TValue>
    {
        public readonly TInput Input;
        public TValue Value;
        public string ErrorMessage;
        public bool InputValid;
        public bool InputCompleted;

        public InputArgs(TInput input)
        {
            Input = input;
            Value = default(TValue);
            InputValid = true;
            InputCompleted = true;
            ErrorMessage = "*Invalid input*";
        }
    }
}
