using System.IO;

namespace SimpleCAD
{
    public interface IPersistable
    {
        // Constructor(BinaryReader reader);
        void Save(BinaryWriter writer);
    }
}
