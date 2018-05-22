using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleCAD
{
    public abstract class Drawable : INotifyPropertyChanged, IPersistable
    {
        public Lazy<Layer> layerRef = new Lazy<Layer>(() => Layer.Default);

        public Style Style { get; set; } = Style.Default;
        public Layer Layer { get => layerRef.Value; set => layerRef = new Lazy<Layer>(() => value); }
        public bool Visible { get; set; } = true;
        internal bool InModel { get; set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void Draw(Renderer renderer);
        public abstract Extents2D GetExtents();
        public virtual bool Contains(Point2D pt, float pickBoxSize) { return GetExtents().Contains(pt); }
        public abstract void TransformBy(Matrix2D transformation);
        public virtual ControlPoint[] GetControlPoints() { return new ControlPoint[0]; }
        public virtual ControlPoint[] GetStretchPoints() { return GetControlPoints(); }
        public virtual SnapPoint[] GetSnapPoints() { return new SnapPoint[0]; }
        public virtual void TransformControlPoints(int[] indices, Matrix2D transformation) { }
        public virtual void TransformStretchPoints(int[] indices, Matrix2D transformation) { TransformControlPoints(indices, transformation); }

        public virtual Drawable Clone() { return (Drawable)MemberwiseClone(); }

        protected Drawable() { }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Load(DocumentReader reader)
        {
            var doc = reader.Document;
            string layerName = reader.ReadString();
            layerRef = new Lazy<Layer>(() => doc.Layers[layerName]);
            Style = reader.ReadPersistable<Style>();
            Visible = reader.ReadBoolean();
        }

        public virtual void Save(DocumentWriter writer)
        {
            writer.Write(Layer.Name);
            writer.Write(Style);
            writer.Write(Visible);
        }
    }
}
