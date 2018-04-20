using SimpleCAD.Graphics;

namespace SimpleCAD
{
    public class LayerDictionary : PersistableDictionaryWithDefault<Layer>
    {
        public LayerDictionary() : base("0", Layer.Default)
        {
            ;
        }
    }
}
