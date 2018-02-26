using System;
using System.Runtime.Serialization;

namespace SimpleCAD
{
    public abstract class Drawable
    {
        public virtual OutlineStyle OutlineStyle { get; set; } = OutlineStyle.White;
        public virtual FillStyle FillStyle { get; set; } = FillStyle.Transparent;
        public virtual bool Visible { get; set; } = true;

        public abstract void Draw(DrawParams param);
        public abstract Extents GetExtents();
        public virtual bool Contains(Point2D pt, float pickBoxSize) { return GetExtents().Contains(pt); }
        public abstract void TransformBy(TransformationMatrix2D transformation);

        protected Drawable()
        {
            ;
        }
    }
}
