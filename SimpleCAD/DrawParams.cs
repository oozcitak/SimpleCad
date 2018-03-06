using System;
using System.Drawing;

namespace SimpleCAD
{
    public class DrawParams
    {
        public CADView View { get; private set; }
        public Graphics Graphics { get; private set; }
        public bool ScaleLineWeights { get; private set; }
        public float ZoomFactor { get; private set; }
        internal Style StyleOverride { get; set; }

        public DrawParams(CADView view, Graphics graphics, bool scaleLineWeights, float zoomFactor)
        {
            View = view;
            Graphics = graphics;
            ScaleLineWeights = scaleLineWeights;
            ZoomFactor = zoomFactor;
            StyleOverride = null;
        }

        public float GetScaledLineWeight(float lineWeight)
        {
            if (ScaleLineWeights)
                return lineWeight;
            else
                return ViewToModel(lineWeight);
        }

        public float ModelToView(float dim)
        {
            return dim / ZoomFactor;
        }

        public float ViewToModel(float dim)
        {
            return dim * ZoomFactor;
        }
    }
}
