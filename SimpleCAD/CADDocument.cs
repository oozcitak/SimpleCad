using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public class CADDocument
    {
        [Browsable(false)]
        public Composite Model { get; private set; }
        [Browsable(false)]
        public Composite Jigged { get; private set; }
        [Browsable(false)]
        public Composite Transients { get; private set; }
        [Browsable(false)]
        public Editor Editor { get; private set; }

        public event DocumentChangedEventHandler DocumentChanged;
        public event TransientsChangedEventHandler TransientsChanged;
        public event SelectionChangedEventHandler SelectionChanged;

        public CADDocument()
        {
            Editor = new Editor(this);
            Model = new Composite();
            Jigged = new Composite();
            Transients = new Composite();
            Editor.Selection.CollectionChanged += Selection_CollectionChanged;
            Model.CollectionChanged += Model_CollectionChanged;
            Jigged.CollectionChanged += Transients_CollectionChanged;
        }

        public void Open(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                IFormatter formatter = new BinaryFormatter();
                Model.CollectionChanged -= Model_CollectionChanged;
                Jigged.CollectionChanged -= Transients_CollectionChanged;
                //Model = (Composite)formatter.Deserialize(stream);
                Model.CollectionChanged += Model_CollectionChanged;
                Jigged.CollectionChanged += Transients_CollectionChanged;
            }
        }

        public void Save(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                IFormatter formatter = new BinaryFormatter();
                //formatter.Serialize(stream, Model);
            }
        }

        private void Model_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnDocumentChanged(new EventArgs());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach(Drawable item in e.OldItems.Cast<Drawable>())
                    {
                        Editor.Selection.Remove(item);
                    }
                    OnDocumentChanged(new EventArgs());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Editor.Selection.Clear();
                    OnDocumentChanged(new EventArgs());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    OnDocumentChanged(new EventArgs());
                    break;
            }
        }

        private void Transients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnTransientsChanged(new EventArgs());
        }

        protected void OnDocumentChanged(EventArgs e)
        {
            DocumentChanged?.Invoke(this, e);
        }

        protected void OnTransientsChanged(EventArgs e)
        {
            TransientsChanged?.Invoke(this, e);
        }

        private void Selection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnSelectionChanged(new EventArgs());
        }

        protected void OnSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
