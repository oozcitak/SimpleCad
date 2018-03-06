using System.Threading.Tasks;

namespace SimpleCAD
{
    public abstract class Command
    {
        public abstract string RegisteredName { get; }
        public abstract string Name { get; }

        public virtual Task Apply(CADDocument doc, params string[] args)
        {
            return Task.FromResult(default(object));
        }
    }
}
