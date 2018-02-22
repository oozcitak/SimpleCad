using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public abstract class Drawable
    {
        public virtual OutlineStyle OutlineStyle { get; set; }
        public virtual FillStyle FillStyle { get; set; }

        public abstract void Draw(DrawParams param);
        public abstract Extents GetExtents();
        public abstract void TransformBy(TransformationMatrix2D transformation);
        public virtual bool Contains(Point2D pt, float pickBoxSize) { return GetExtents().Contains(pt); }
        public virtual bool Visible { get; set; }

        protected Drawable()
        {
            OutlineStyle = OutlineStyle.Black;
            FillStyle = FillStyle.Transparent;
            Visible = true;
        }
    }
}
