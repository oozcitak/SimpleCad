using SimpleCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
