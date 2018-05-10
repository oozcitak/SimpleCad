using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCAD.Commands
{
    public class TransformMove : Command
    {
        public override string RegisteredName => "Transform.Move";
        public override string Name => "Move";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;

            var s = await ed.GetSelection("Select objects: ");
            if (s.Result != ResultMode.OK || s.Value.Count == 0) return;
            var p1 = await ed.GetPoint("Base point: ");
            if (p1.Result != ResultMode.OK) return;
            Composite consItems = new Composite();
            foreach (Drawable item in s.Value)
            {
                consItems.Add(item.Clone());
            }
            doc.Transients.Add(consItems);
            Point2D lastPt = p1.Value;
            var p2 = await ed.GetPoint("Second point: ", p1.Value,
                (p) =>
                {
                    consItems.TransformBy(Matrix2D.Translation(p - lastPt));
                    lastPt = p;
                });
            doc.Transients.Remove(consItems);
            if (p2.Result != ResultMode.OK) return;

            foreach (Drawable item in s.Value)
            {
                item.TransformBy(Matrix2D.Translation(p2.Value - p1.Value));
            }
        }
    }

    public class TransformCopy : Command
    {
        public override string RegisteredName => "Transform.Copy";
        public override string Name => "Copy";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;

            var s = await ed.GetSelection("Select objects: ");
            if (s.Result != ResultMode.OK || s.Value.Count == 0) return;
            var p1 = await ed.GetPoint("Base point: ");
            if (p1.Result != ResultMode.OK) return;
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
                var p2 = await ed.GetPoint("Second point: ", p1.Value,
                    (p) =>
                    {
                        consItems.TransformBy(Matrix2D.Translation(p - lastPt));
                        lastPt = p;
                    });

                if (p2.Result != ResultMode.OK)
                {
                    flag = false;
                }
                else
                {
                    foreach (Drawable item in s.Value)
                    {
                        Drawable newItem = item.Clone();
                        newItem.TransformBy(Matrix2D.Translation(p2.Value - p1.Value));
                        doc.Model.Add(newItem);
                    }
                }
            }

            doc.Transients.Remove(consItems);
        }
    }

    public class TransformRotate : Command
    {
        public override string RegisteredName => "Transform.Rotate";
        public override string Name => "Rotate";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;

            var s = await ed.GetSelection("Select objects: ");
            if (s.Result != ResultMode.OK || s.Value.Count == 0) return;
            var p1 = await ed.GetPoint("Base point: ");
            if (p1.Result != ResultMode.OK) return;
            Composite consItems = new Composite();
            foreach (Drawable item in s.Value)
            {
                consItems.Add(item.Clone());
            }
            doc.Transients.Add(consItems);
            float lastAngle = 0;
            var p2 = await ed.GetAngle("Rotation angle: ", p1.Value,
                (p) =>
                {
                    consItems.TransformBy(Matrix2D.Rotation(p1.Value, p - lastAngle));
                    lastAngle = p;
                });
            doc.Transients.Remove(consItems);
            if (p2.Result != ResultMode.OK) return;

            foreach (Drawable item in s.Value)
            {
                item.TransformBy(Matrix2D.Rotation(p1.Value, p2.Value));
            }
        }
    }

    public class TransformScale : Command
    {
        public override string RegisteredName => "Transform.Scale";
        public override string Name => "Scale";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;

            var s = await ed.GetSelection("Select objects: ");
            if (s.Result != ResultMode.OK || s.Value.Count == 0) return;
            var p1 = await ed.GetPoint("Base point: ");
            if (p1.Result != ResultMode.OK) return;
            Composite consItems = new Composite();
            var ext = new Extents2D();
            foreach (Drawable item in s.Value)
            {
                ext.Add(item.GetExtents());
                consItems.Add(item.Clone());
            }
            doc.Transients.Add(consItems);
            float size = System.Math.Max(ext.Width, ext.Height);
            float lastScale = 1;
            var d1 = await ed.GetDistance("Scale: ", p1.Value,
                (p) =>
                {
                    p /= size;
                    consItems.TransformBy(Matrix2D.Scale(p1.Value, p / lastScale));
                    lastScale = p;
                });
            doc.Transients.Remove(consItems);
            if (d1.Result != ResultMode.OK) return;

            foreach (Drawable item in s.Value)
            {
                item.TransformBy(Matrix2D.Scale(p1.Value, d1.Value));
            }
        }
    }

    public class TransformMirror : Command
    {
        public override string RegisteredName => "Transform.Mirror";
        public override string Name => "Mirror";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;

            var s = await ed.GetSelection("Select objects: ");
            if (s.Result != ResultMode.OK || s.Value.Count == 0) return;
            var p1 = await ed.GetPoint("Base point: ");
            if (p1.Result != ResultMode.OK) return;
            Composite consItems = new Composite();
            foreach (Drawable item in s.Value)
            {
                consItems.Add(item.Clone());
            }
            doc.Transients.Add(consItems);
            Matrix2D lastTrans = Matrix2D.Identity;
            var p2 = await ed.GetPoint("Second point: ", p1.Value,
                (p) =>
                {
                    Matrix2D mirror = Matrix2D.Mirror(p1.Value, p - p1.Value);
                    consItems.TransformBy(lastTrans.Inverse);
                    consItems.TransformBy(mirror);
                    lastTrans = mirror;
                });
            doc.Transients.Remove(consItems);
            if (p2.Result != ResultMode.OK) return;

            foreach (Drawable item in s.Value)
            {
                Drawable newItem = item.Clone();
                newItem.TransformBy(Matrix2D.Mirror(p1.Value, p2.Value - p1.Value));
                doc.Model.Add(newItem);
            }
        }
    }

    public class TransformMoveControlPoints : Command
    {
        public override string RegisteredName => "Transform.MoveControlPoints";
        public override string Name => "MoveControlPoints";

        public override async Task Apply(CADDocument doc, params string[] args)
        {
            Editor ed = doc.Editor;

            var s = await ed.GetControlPoints("Select objects: ");
            if (s.Result != ResultMode.OK || s.Value.Count == 0) return;
            var p1 = await ed.GetPoint("Base point: ");
            if (p1.Result != ResultMode.OK) return;
            Composite consItems = new Composite();
            CPSelectionSet consSet = new CPSelectionSet();
            foreach (var pair in s.Value)
            {
                Drawable item = pair.Key.Clone();
                consItems.Add(item);
                consSet.Add(item, pair.Value);
            }
            doc.Transients.Add(consItems);
            Point2D lastPt = p1.Value;
            var p2 = await ed.GetPoint("Second point: ", p1.Value,
                (p) =>
                {
                    foreach (Drawable item in consItems)
                    {
                        item.TransformControlPoints(consSet[item], Matrix2D.Translation(p - lastPt));
                    }
                    lastPt = p;
                });
            doc.Transients.Remove(consItems);
            if (p2.Result != ResultMode.OK) return;

            foreach (var pair in s.Value)
            {
                pair.Key.TransformControlPoints(pair.Value.ToArray(), Matrix2D.Translation(p2.Value - p1.Value));
            }
        }
    }
}
