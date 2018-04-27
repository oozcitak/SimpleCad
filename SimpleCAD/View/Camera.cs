using SimpleCAD.Geometry;
using System.ComponentModel;

namespace SimpleCAD
{
    public class Camera
    {
        private Point2D _position;
        private float _zoom;

        [Category("Appearance"), DefaultValue(5f / 3f), Description("Determines the zoom factor of the view.")]
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;

                if (float.IsNaN(_zoom) || float.IsNegativeInfinity(_zoom) || float.IsPositiveInfinity(_zoom) ||
                    _zoom < float.Epsilon * 1000.0f || _zoom > float.MaxValue / 1000.0f)
                {
                    _zoom = 1;
                }
            }
        }

        [Category("Appearance"), DefaultValue(typeof(System.Drawing.PointF), "0,0"), Description("Determines the location of the camera.")]
        public Point2D Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                float x = _position.X;
                float y = _position.Y;
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
                _position = new Point2D(x, y);
            }
        }

        public Camera(Point2D position, float zoom)
        {
            _position = position;
            _zoom = zoom;
        }
    }
}
