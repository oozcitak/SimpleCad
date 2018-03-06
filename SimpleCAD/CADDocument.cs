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
        [Browsable(false)]
        public Settings Settings { get; private set; }

        public string FileName { get; private set; }
        public bool IsModified { get; private set; } = false;

        public event DocumentChangedEventHandler DocumentChanged;
        public event TransientsChangedEventHandler TransientsChanged;
        public event SelectionChangedEventHandler SelectionChanged;

        public CADDocument()
        {
            Model = new Composite();
            Editor = new Editor(this);
            Settings = new Settings();
            Jigged = new Composite();
            Transients = new Composite();
            Editor.PickedSelection.CollectionChanged += Selection_CollectionChanged;
            Model.CollectionChanged += Model_CollectionChanged;
            Jigged.CollectionChanged += Transients_CollectionChanged;
        }

        public void New()
        {
            Editor.PickedSelection.CollectionChanged -= Selection_CollectionChanged;
            Model.CollectionChanged -= Model_CollectionChanged;
            Jigged.CollectionChanged -= Transients_CollectionChanged;
            Model = new Composite();
            Editor = new Editor(this);
            Settings = new Settings();
            Jigged = new Composite();
            Transients = new Composite();
            Editor.PickedSelection.CollectionChanged += Selection_CollectionChanged;
            Model.CollectionChanged += Model_CollectionChanged;
            Jigged.CollectionChanged += Transients_CollectionChanged;
            OnDocumentChanged(new EventArgs());
            IsModified = false;
            FileName = "";
        }

        public void Open(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                Editor.PickedSelection.CollectionChanged -= Selection_CollectionChanged;
                Model.CollectionChanged -= Model_CollectionChanged;
                Jigged.CollectionChanged -= Transients_CollectionChanged;
                Model = new Composite(reader);
                Settings = new Settings(reader);
                Editor = new Editor(this);
                Jigged = new Composite();
                Transients = new Composite();
                Editor.PickedSelection.CollectionChanged += Selection_CollectionChanged;
                Model.CollectionChanged += Model_CollectionChanged;
                Jigged.CollectionChanged += Transients_CollectionChanged;
                OnDocumentChanged(new EventArgs());
                FileName = "";
                IsModified = false;
            }
        }

        public void Open(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Open(stream);
                FileName = filename;
                IsModified = false;
            }
        }

        public void Save(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                Model.Save(writer);
                Settings.Save(writer);
                FileName = "";
                IsModified = false;
            }
        }

        public void Save(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(stream);
                FileName = filename;
                IsModified = false;
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
                    foreach (Drawable item in e.OldItems.Cast<Drawable>())
                    {
                        Editor.PickedSelection.Remove(item);
                    }
                    OnDocumentChanged(new EventArgs());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Editor.PickedSelection.Clear();
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
            IsModified = true;
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
