using SimpleCAD.Drawables;
using System.Collections.Specialized;

namespace SimpleCAD
{
    public class Model : Composite
    {
        public CADDocument Document { get; private set; }

        public Model(CADDocument doc)
        {
            Document = doc;
        }

        public override void Add(Drawable item)
        {
            item.InModel = true;
            base.Add(item);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.NewItems != null)
            {
                foreach (Drawable item in e.NewItems)
                {
                    // add layers
                    CheckOrAddLayer(item);

                    // add text styles
                    if (item is Text)
                        CheckOrAddTextStyle(item as Text);
                    else if (item is Dimension)
                        CheckOrAddTextStyle(item as Dimension);
                }
            }
        }

        private void CheckOrAddLayer(Drawable item)
        {
            var layer = item.Layer;
            if (Document.Layers.TryGetValue(layer.Name, out var docLayer))
            {
                item.Layer = docLayer;
            }
            else
            {
                Document.Layers.Add(layer.Name, layer);
            }
        }

        private void CheckOrAddTextStyle(Text item)
        {
            var textStyle = item.TextStyle;
            if (Document.TextStyles.TryGetValue(textStyle.Name, out var docTextStyle))
            {
                item.TextStyle = docTextStyle;
            }
            else
            {
                Document.TextStyles.Add(textStyle.Name, textStyle);
            }
        }

        private void CheckOrAddTextStyle(Dimension item)
        {
            var textStyle = item.TextStyle;
            if (Document.TextStyles.TryGetValue(textStyle.Name, out var docTextStyle))
            {
                item.TextStyle = docTextStyle;
            }
            else
            {
                Document.TextStyles.Add(textStyle.Name, textStyle);
            }
        }
    }
}
