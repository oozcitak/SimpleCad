using SimpleCAD.Geometry;
using System.ComponentModel;

namespace SimpleCAD
{
    public class Camera
    {
        private Point2D position;
        private float zoom;

        [Category("Appearance"), DefaultValue(5f / 3f), Description("Determines the zoom factor of the view.")]
        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value;

                if (float.IsNaN(zoom) || float.IsNegativeInfinity(zoom) || float.IsPositiveInfinity(zoom) ||
                    zoom < float.Epsilon * 1000.0f || zoom > float.MaxValue / 1000.0f)
                {
                    zoom = 1;
                }
            }
        }

        [Category("Appearance"), DefaultValue(typeof(System.Drawing.PointF), "0,0"), Description("Determines the location of the camera.")]
        public Point2D Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                float x = position.X;
                float y = position.Y;
                if (float.IsNaN(x) || float.IsNegativeInfinity(x) || float.IsPositiveInfinity(x) ||
                    x < float.MinValue / 1000.0f || x > float.MaxValue / 1000.0f)
                {
                    x = 0;
                }
                if (float.IsNaN(y) || float.IsNegativeInfinity(y) || float.IsPositiveInfinity(y) ||
                    y < float.MinValue / 1000.0f || y > float.MaxValue / 1000.0f)
                {
                    y = 0;
                }
                position = new Point2D(x, y);
            }
        }

        public Camera(Point2D position, float zoom)
        {
            this.position = position;
            this.zoom = zoom;
        }
    }
}
