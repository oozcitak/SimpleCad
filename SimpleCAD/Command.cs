using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public abstract partial class Command
    {
        public abstract string RegisteredName { get; }
        public abstract string Name { get; }

        public virtual Task Apply(CADDocument doc, params string[] args)
        {
            return Task.FromResult(default(object));
        }
    }
}
