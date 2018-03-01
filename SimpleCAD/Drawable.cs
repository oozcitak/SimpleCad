using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SimpleCAD
{
    public abstract class Drawable : INotifyPropertyChanged
    {
        public virtual OutlineStyle OutlineStyle { get; set; } = OutlineStyle.White;
        public virtual FillStyle FillStyle { get; set; } = FillStyle.Transparent;
        public virtual bool Visible { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void Draw(DrawParams param);
        public abstract Extents GetExtents();
        public virtual bool Contains(Point2D pt, float pickBoxSize) { return GetExtents().Contains(pt); }
        public abstract void TransformBy(TransformationMatrix2D transformation);

        public virtual Drawable Clone() { return (Drawable)MemberwiseClone(); }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected Drawable()
        {
            ;
        }
    }
}
