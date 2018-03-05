using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SimpleCAD
{
    public abstract class Drawable : INotifyPropertyChanged
    {
        public virtual Outline Outline { get; set; } = Outline.White;
        public virtual bool Visible { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void Draw(DrawParams param);
        public abstract Extents2D GetExtents();
        public virtual bool Contains(Point2D pt, float pickBoxSize) { return GetExtents().Contains(pt); }
        public abstract void TransformBy(TransformationMatrix2D transformation);
        public virtual ControlPoint[] GetControlPoints(float size) { return new ControlPoint[0]; }
        public virtual void TransformControlPoint(ControlPoint cp, TransformationMatrix2D transformation)
        {
            PropertyInfo prop = GetType().GetProperty(cp.PropertyName);
            Point2D point = cp.Location.Transform(transformation);

            if (cp.PropertyIndex == -1)
            {
                if (cp.Type == ControlPoint.ControlPointType.Point)
                    prop.SetValue(this, point);
                else if (cp.Type == ControlPoint.ControlPointType.Angle)
                    prop.SetValue(this, (point - cp.BasePoint).Angle);
                else if (cp.Type == ControlPoint.ControlPointType.Distance)
                    prop.SetValue(this, (point - cp.BasePoint).Length);
            }
            else
            {
                if (cp.Type == ControlPoint.ControlPointType.Point)
                {
                    IList<Point2D> items = (IList<Point2D>)prop.GetValue(this);
                    items[cp.PropertyIndex] = point;
                }
                else if (cp.Type == ControlPoint.ControlPointType.Angle)
                {
                    IList<float> items = (IList<float>)prop.GetValue(this);
                    items[cp.PropertyIndex] = (point - cp.BasePoint).Angle;
                }
                else if (cp.Type == ControlPoint.ControlPointType.Distance)
                {
                    IList<float> items = (IList<float>)prop.GetValue(this);
                    items[cp.PropertyIndex] = (point - cp.BasePoint).Length;
                }
            }
        }

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
