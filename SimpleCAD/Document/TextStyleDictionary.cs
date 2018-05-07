using SimpleCAD.Graphics;

namespace SimpleCAD
{
    public class TextStyleDictionary : PersistableDictionaryWithDefault<TextStyle>
    {
        public TextStyleDictionary() : base("0", TextStyle.Default)
        {
            ;
        }
    }
}
