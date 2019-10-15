using SimpleCAD.Geometry;
using System;
using System.Collections.Generic;

namespace SimpleCAD
{
    public class SnapPointCollection
    {
        private class SnapPointEntry : IComparable<SnapPointEntry>
        {
            public readonly float Distance;
            public readonly SnapPoint SnapPoint;

            public SnapPointEntry(float distance, SnapPoint snapPoint)
            {
                Distance = distance;
                SnapPoint = snapPoint;
            }

            public int CompareTo(SnapPointEntry other)
            {
                int distCmp = Distance.CompareTo(other.Distance);
                if (distCmp == 0)
                    return ((int)SnapPoint.Type < (int)other.SnapPoint.Type ? -1 : 1);
                else
                    return distCmp;
            }
        }

        private SortedList<SnapPointEntry, bool> items = new SortedList<SnapPointEntry, bool>();
        private int currentIndex = 0;

        public bool IsEmpty { get => items.Count == 0; }

        public void Add(float distance, SnapPoint point)
        {
            items.Add(new SnapPointEntry(distance, point), false);
        }

        public void AddFromDrawable(Drawable item, Point2D cursorLocation, SnapPointType snapMode, float snapDistance)
        {
            foreach (SnapPoint pt in item.GetSnapPoints())
            {
                if ((snapMode & pt.Type) == pt.Type)
                {
                    float dist = (pt.Location - cursorLocation).Length;
                    if (dist <= snapDistance)
                    {
                        Add(dist, pt);
                    }
                }
            }
        }

        public void Clear()
        {
            items.Clear();
            currentIndex = 0;
        }

        public void Next()
        {
            currentIndex++;
            if (currentIndex == items.Count)
                currentIndex = 0;
        }

        public void Previous()
        {
            currentIndex--;
            if (currentIndex == -1)
                currentIndex = items.Count - 1;
        }

        public SnapPoint Current()
        {
            return items.Keys[currentIndex].SnapPoint;
        }

        public static implicit operator Point2D(SnapPointCollection collection)
        {
            return collection.Current().Location;
        }
    }
}
