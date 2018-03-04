using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCAD
{
    public partial class Command
    {
        public class TransformMove : Command
        {
            public override string RegisteredName => "Transform.Move";
            public override string Name => "Move";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.SelectionResult s = await ed.GetSelection("Select objects: ");
                if (s.Result != Editor.ResultMode.OK || s.Value.Count == 0) return;
                Editor.PointResult p1 = await ed.GetPoint("Base point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Composite consItems = new Composite();
                foreach (Drawable item in s.Value)
                {
                    consItems.Add(item.Clone());
                }
                doc.Transients.Add(consItems);
                Point2D lastPt = p1.Value;
                Editor.PointResult p2 = await ed.GetPoint("Second point: ", p1.Value,
                    (p) =>
                    {
                        consItems.TransformBy(TransformationMatrix2D.Translation(p - lastPt));
                        lastPt = p;
                    });
                doc.Transients.Remove(consItems);
                if (p2.Result != Editor.ResultMode.OK) return;

                foreach (Drawable item in s.Value)
                {
                    item.TransformBy(TransformationMatrix2D.Translation(p2.Value - p1.Value));
                }

                ed.Selection.Clear();
            }
        }

        public class TransformCopy : Command
        {
            public override string RegisteredName => "Transform.Copy";
            public override string Name => "Copy";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.SelectionResult s = await ed.GetSelection("Select objects: ");
                if (s.Result != Editor.ResultMode.OK || s.Value.Count == 0) return;
                Editor.PointResult p1 = await ed.GetPoint("Base point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Composite consItems = new Composite();
                foreach (Drawable item in s.Value)
                {
                    consItems.Add(item.Clone());
                }
                doc.Transients.Add(consItems);
                Point2D lastPt = p1.Value;
                bool flag = true;
                while (flag)
                {
                    Editor.PointResult p2 = await ed.GetPoint("Second point: ", p1.Value,
                        (p) =>
                        {
                            consItems.TransformBy(TransformationMatrix2D.Translation(p - lastPt));
                            lastPt = p;
                        });

                    if (p2.Result != Editor.ResultMode.OK)
                    {
                        flag = false;
                    }
                    else
                    {
                        foreach (Drawable item in s.Value)
                        {
                            Drawable newItem = item.Clone();
                            newItem.TransformBy(TransformationMatrix2D.Translation(p2.Value - p1.Value));
                            doc.Model.Add(newItem);
                        }
                    }
                }

                doc.Transients.Remove(consItems);
                ed.Selection.Clear();
            }
        }

        public class TransformRotate : Command
        {
            public override string RegisteredName => "Transform.Rotate";
            public override string Name => "Rotate";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.SelectionResult s = await ed.GetSelection("Select objects: ");
                if (s.Result != Editor.ResultMode.OK || s.Value.Count == 0) return;
                Editor.PointResult p1 = await ed.GetPoint("Base point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Composite consItems = new Composite();
                foreach (Drawable item in s.Value)
                {
                    consItems.Add(item.Clone());
                }
                doc.Transients.Add(consItems);
                float lastAngle = 0;
                Editor.AngleResult p2 = await ed.GetAngle("Rotation angle: ", p1.Value,
                    (p) =>
                    {
                        consItems.TransformBy(TransformationMatrix2D.Rotation(p1.Value, p - lastAngle));
                        lastAngle = p;
                    });
                doc.Transients.Remove(consItems);
                if (p2.Result != Editor.ResultMode.OK) return;

                foreach (Drawable item in s.Value)
                {
                    item.TransformBy(TransformationMatrix2D.Rotation(p1.Value, p2.Value));
                }

                ed.Selection.Clear();
            }
        }

        public class TransformScale : Command
        {
            public override string RegisteredName => "Transform.Scale";
            public override string Name => "Scale";

            public override async Task Apply(CADDocument doc)
            {
                Editor ed = doc.Editor;

                Editor.SelectionResult s = await ed.GetSelection("Select objects: ");
                if (s.Result != Editor.ResultMode.OK || s.Value.Count == 0) return;
                Editor.PointResult p1 = await ed.GetPoint("Base point: ");
                if (p1.Result != Editor.ResultMode.OK) return;
                Composite consItems = new Composite();
                foreach (Drawable item in s.Value)
                {
                    consItems.Add(item.Clone());
                }
                doc.Transients.Add(consItems);
                float lastScale = 1;
                Editor.DistanceResult d1 = await ed.GetDistance("Scale: ", p1.Value,
                    (p) =>
                    {
                        consItems.TransformBy(TransformationMatrix2D.Scale(p1.Value, p / lastScale));
                        lastScale = p;
                    });
                doc.Transients.Remove(consItems);
                if (d1.Result != Editor.ResultMode.OK) return;

                foreach (Drawable item in s.Value)
                {
                    item.TransformBy(TransformationMatrix2D.Scale(p1.Value, d1.Value));
                }

                ed.Selection.Clear();
            }
        }
    }
}