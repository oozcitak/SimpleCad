using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleCAD
{
    [Serializable]
    public abstract class Drawable : INotifyPropertyChanged, IPersistable
    {
        public Style Style { get; set; } = Style.Default;
        public Layer Layer { get; set; }
        public bool Visible { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void Draw(Renderer renderer);
        public abstract Extents2D GetExtents();
        public virtual bool Contains(Point2D pt, float pickBoxSize) { return GetExtents().Contains(pt); }
        public abstract void TransformBy(Matrix2D transformation);
        public virtual ControlPoint[] GetControlPoints() { return new ControlPoint[0]; }
        public virtual void TransformControlPoint(int index, Matrix2D transformation) { }

        public virtual Drawable Clone() { return (Drawable)MemberwiseClone(); }

        protected Drawable() { }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Load(DocumentReader reader)
        {
            string layerName = reader.ReadString();
            Layer = reader.Document.Layers[layerName];
            Style = new Style();
            Style.Load(reader);
            Visible = reader.ReadBoolean();
        }

        public virtual void Save(DocumentWriter writer)
        {
            writer.Write(Layer.Name);
            Style.Save(writer);
            writer.Write(Visible);
        }
    }
}
