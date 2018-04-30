using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.IO;

namespace SimpleCAD
{
    public class DocumentWriter : IDisposable
    {
        BinaryWriter writer;

        public CADDocument Document { get; private set; }

        public DocumentWriter(CADDocument document, Stream stream)
        {
            Document = document;
            writer = new BinaryWriter(stream);
        }

        public void Write(bool value)
        {
            writer.Write(value);
        }

        public void Write(float value)
        {
            writer.Write(value);
        }

        public void Write(int value)
        {
            writer.Write(value);
        }

        public void Write(uint value)
        {
            writer.Write(value);
        }

        public void Write(string value)
        {
            writer.Write(value);
        }

        public void Write(Point2D value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public void Write(Point2DCollection value)
        {
            Write(value.Count);
            foreach (Point2D point in value)
                Write(point);
        }

        public void Write(Vector2D value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public void Write(Color value)
        {
            Write(value.IsByLayer);
            Write(value.Argb);
        }

        public void Write(Camera value)
        {
            Write(value.Position);
            Write(value.Zoom);
        }

        public void Write(IPersistable item)
        {
            Write(item.GetType().FullName);
            item.Save(this);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
