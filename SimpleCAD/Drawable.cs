using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public abstract class Drawable
    {
        public virtual OutlineStyle OutlineStyle { get; set; }
        public virtual FillStyle FillStyle { get; set; }
        public virtual bool Visible { get; set; }

        public abstract void Draw(DrawParams param);
        public abstract Extents GetExtents();
        public virtual bool Contains(Point2D pt, float pickBoxSize) { return GetExtents().Contains(pt); }
        public abstract void TransformBy(TransformationMatrix2D transformation);

        protected Drawable()
        {
            OutlineStyle = OutlineStyle.White;
            FillStyle = FillStyle.Transparent;
            Visible = true;
        }
    }
}
