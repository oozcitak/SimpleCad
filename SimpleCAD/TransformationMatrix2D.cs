using System;
using System.Drawing.Drawing2D;

namespace SimpleCAD
{
    public struct TransformationMatrix2D
    {
        public float M11 { get; set; }
        public float M12 { get; set; }
        public float M21 { get; set; }
        public float M22 { get; set; }
        public float DX { get; set; }
        public float DY { get; set; }
        public float RotationAngle { get { return -MathF.Asin(M12); } }

        public static TransformationMatrix2D Identity { get { return new TransformationMatrix2D(1, 0, 0, 1, 0, 0); } }

        public TransformationMatrix2D(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            M11 = m11; M12 = m12;
            M21 = m21; M22 = m22;
            DX = dx; DY = dy;
        }

        public static TransformationMatrix2D Transformation(float xScale, float yScale, float rotation, float dx, float dy)
        {
            float m11 = xScale * MathF.Cos(rotation);
            float m12 = -MathF.Sin(rotation);
            float m21 = MathF.Sin(rotation);
            float m22 = yScale * MathF.Cos(rotation);

            return new TransformationMatrix2D(m11, m12, m21, m22, dx, dy);
        }

        public static TransformationMatrix2D Scale(float xScale, float yScale)
        {
            float m11 = xScale;
            float m22 = yScale;

            return new TransformationMatrix2D(m11, 0, 0, m22, 0, 0);
        }

        public static TransformationMatrix2D Rotation(float rotation)
        {
            float m11 = MathF.Cos(-rotation);
            float m12 = MathF.Sin(-rotation);
            float m21 = -MathF.Sin(-rotation);
            float m22 = MathF.Cos(-rotation);

            return new TransformationMatrix2D(m11, m12, m21, m22, 0, 0);
        }

        public static TransformationMatrix2D Translation(Vector2D delta)
        {
            return Translation(delta.X, delta.Y);
        }

        public static TransformationMatrix2D Translation(float dx, float dy)
        {
            return new TransformationMatrix2D(1, 0, 0, 1, dx, dy);
        }

        public TransformationMatrix2D Multiply(TransformationMatrix2D m)
        {
            float m11 = M11 * m.M11 + M12 * m.M21 + DX * 0;
            float m12 = M11 * m.M12 + M22 * m.M21 + DX * 0;
            float dx = M11 * m.DX + M22 * m.DY + DX * 1;
            float m21 = M21 * m.M11 + M22 * m.M21 + DY * 0;
            float m22 = M21 * m.M12 + M22 * m.M22 + DY * 0;
            float dy = M21 * m.DX + M22 * m.DY + DY * 1;
            return new TransformationMatrix2D(m11, m12, m21, m22, dx, dy);
        }

        public static explicit operator Matrix(TransformationMatrix2D t)
        {
            return new Matrix(t.M11, t.M12, t.M21, t.M22, t.DX, t.DY);
        }

        public override string ToString()
        {
            string str = "|" + M11.ToString() + ", " + M12.ToString() + ", " + DX.ToString() + "|" + Environment.NewLine +
                         "|" + M21.ToString() + ", " + M22.ToString() + ", " + DY.ToString() + "|" + Environment.NewLine +
                         "|0, 0, 1|";
            return str;
        }
    }
}
