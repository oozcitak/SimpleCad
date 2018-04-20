namespace SimpleCAD
{
    public interface IPersistable
    {
        void Load(DocumentReader reader);
        void Save(DocumentWriter writer);
    }
}
