using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public abstract class CommandBase
    {
        public abstract string RegisteredName { get; }
        public abstract string Name { get; }
    }

    public abstract partial class SyncCommand : CommandBase
    {
        public virtual void Apply(CADDocument doc, params string[] args)
        {
            ;
        }
    }

    public abstract partial class AsyncCommand : CommandBase
    {
        public virtual Task Apply(CADDocument doc, params string[] args)
        {
            return Task.FromResult(default(object));
        }
    }
}
