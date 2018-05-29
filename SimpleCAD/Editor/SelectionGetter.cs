using SimpleCAD.Drawables;
using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using SimpleCAD.View;

namespace SimpleCAD
{
    internal class SelectionGetter : EditorGetter<SelectionOptions, SelectionSet>
    {
        Point2D firstPoint;
        bool getFirstPoint;
        SelectionWindow consLine;

        protected override void Init(InitArgs<SelectionSet> args)
        {
            // Immediately return existing picked-selection if any
            if (Options.UsePickedSelection && Editor.PickedSelection.Count != 0)
            {
                SelectionSet ss = new SelectionSet();
                foreach (Drawable item in Editor.PickedSelection)
                {
                    if (Options.AllowedClasses.Count == 0 || Options.AllowedClasses.Contains(item.GetType()))
                        ss.Add(item);
                }
                Editor.PickedSelection.Clear();
                args.Value = ss;
                args.ContinueAsync = false;
            }
            else
            {
                getFirstPoint = false;
            }
        }

        protected override void CoordsChanged(Point2D pt)
        {
            SetCursorText(pt.ToString(Editor.Document.Settings.NumberFormat));

            // Update the selection window
            if (getFirstPoint)
                consLine.P2 = pt;
        }

        protected override void AcceptCoordsInput(InputArgs<Point2D, SelectionSet> args)
        {
            if (!getFirstPoint)
            {
                firstPoint = args.Input;
                getFirstPoint = true;
                args.InputCompleted = false;

                consLine = new SelectionWindow(firstPoint, firstPoint);
                Editor.Document.Transients.Add(consLine);
            }
            else
            {
                args.Value = GetSelectionFromWindow();
                args.InputCompleted = true;

                Editor.Document.Transients.Remove(consLine);
            }
        }

        protected override void AcceptTextInput(InputArgs<string, SelectionSet> args)
        {
            args.InputValid = Point2D.TryParse(args.Input, out Point2D pt);
            if (args.InputValid)
            {
                if (!getFirstPoint)
                {
                    firstPoint = pt;
                    getFirstPoint = true;
                    args.InputCompleted = false;

                    consLine = new SelectionWindow(firstPoint, firstPoint);
                    Editor.Document.Transients.Add(consLine);
                }
                else
                {
                    args.Value = GetSelectionFromWindow();
                    args.InputCompleted = true;

                    Editor.Document.Transients.Remove(consLine);
                }
            }
        }

        protected override void CancelInput()
        {
            Editor.Document.Transients.Remove(consLine);
        }

        private SelectionSet GetSelectionFromWindow()
        {
            Extents2D ex = consLine.GetExtents();
            SelectionSet ss = new SelectionSet();
            foreach (Drawable item in Editor.Document.ActiveView.VisibleItems)
            {
                Extents2D exItem = item.GetExtents();
                if (consLine.WindowSelection && ex.Contains(exItem) || !consLine.WindowSelection && ex.IntersectsWith(exItem))
                {
                    if (Options.AllowedClasses.Count == 0 || Options.AllowedClasses.Contains(item.GetType()))
                        ss.Add(item);
                }
            }
            return ss;
        }
    }
}
