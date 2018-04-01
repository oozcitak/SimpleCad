using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System.Threading.Tasks;

namespace SimpleCAD
{
    internal class SelectionGetter : EditorGetter<SelectionOptions, SelectionSet>
    {
        protected override void Init(InitArgs<SelectionSet> args)
        {
            // Immediately return existing picked-selection if any
            if (Editor.PickedSelection.Count != 0)
            {
                SelectionSet picked = Editor.PickedSelection;
                Editor.PickedSelection.Clear();
                args.Value = picked;
                args.ContinueAsync = false;
            }
        }

        protected override async Task InitAsync(InitArgs<SelectionSet> args)
        {
            var firstPoint = await Editor.GetPoint(Options.Message);
            if (firstPoint.Result != ResultMode.OK)
            {
                args.InputValid = false;
            }
            else
            {
                var consHatch = new Hatch(firstPoint.Value, firstPoint.Value, firstPoint.Value, firstPoint.Value);
                Editor.Document.Transients.Add(consHatch);
                var consLine = new Polygon(firstPoint.Value, firstPoint.Value, firstPoint.Value, firstPoint.Value);
                Editor.Document.Transients.Add(consLine);

                var secondPoint = await Editor.GetPoint(Options.Message, (p) =>
                {
                        // Update the selection window
                        Point2D p1 = consLine.Points[0];
                    Point2D p2 = new Point2D(p.X, p1.Y);
                    Point2D p3 = p;
                    Point2D p4 = new Point2D(p1.X, p.Y);
                    consLine.Points[0] = p1;
                    consLine.Points[1] = p2;
                    consLine.Points[2] = p3;
                    consLine.Points[3] = p4;
                    consHatch.Points[0] = p1;
                    consHatch.Points[1] = p2;
                    consHatch.Points[2] = p3;
                    consHatch.Points[3] = p4;
                    if (p.X > p1.X)
                    {
                        consHatch.Style = new Style(Editor.Document.Settings.Get<Color>("SelectionWindowColor"));
                        consLine.Style = new Style(Editor.Document.Settings.Get<Color>("SelectionWindowBorderColor"));
                    }
                    else
                    {
                        consHatch.Style = new Style(Editor.Document.Settings.Get<Color>("ReverseSelectionWindowColor"));
                        consLine.Style = new Style(Editor.Document.Settings.Get<Color>("SelectionWindowBorderColor"), 0, DashStyle.Dash);
                    }
                });
                Editor.Document.Transients.Remove(consHatch);
                Editor.Document.Transients.Remove(consLine);

                if (secondPoint.Result != ResultMode.OK)
                {
                    args.InputValid = false;
                }
                else
                {
                    Editor.CurrentSelection.Clear();
                    Extents2D ex = consHatch.GetExtents();
                    bool windowSelection = (consHatch.Points[2].X > consHatch.Points[0].X);
                    foreach (Drawable item in Editor.Document.Model)
                    {
                        Extents2D exItem = item.GetExtents();
                        if (windowSelection && ex.Contains(exItem) || !windowSelection && ex.IntersectsWith(exItem))
                            Editor.CurrentSelection.Add(item);
                    }
                    args.Value = Editor.CurrentSelection;
                }
            }

            args.ContinueAsync = false;
        }
    }
}
