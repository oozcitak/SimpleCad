using System;
using System.Drawing.Drawing2D;

namespace SimpleCAD.Geometry
{
    public struct Matrix2D
    {
        private readonly float _m11;
        private readonly float _m12;
        private readonly float _m21;
        private readonly float _m22;
        private readonly float _dx;
        private readonly float _dy;

        public float M11 { get { return _m11; } }
        public float M12 { get { return _m12; } }
        public float M21 { get { return _m21; } }
        public float M22 { get { return _m22; } }
        public float DX { get { return _dx; } }
        public float DY { get { return _dy; } }

        public float RotationAngle { get { return Vector2D.XAxis.Transform(this).Angle; } }

        public static Matrix2D Identity { get { return new Matrix2D(1, 0, 0, 1, 0, 0); } }

        public Matrix2D(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            _m11 = m11; _m12 = m12;
            _m21 = m21; _m22 = m22;
            _dx = dx; _dy = dy;
        }

        public Matrix2D Inverse
        {
            get
            {
                float det = M11 * M22 - M12 * M21;
                float invdet = 1 / det;

                float m11 = M22 * invdet;
                float m12 = -M12 * invdet;
                float dx = (M12 * DY - DX * M22) * invdet;
                float m21 = -M21 * invdet;
                float m22 = M11 * invdet;
                float dy = (M21 * DX - M11 * DY) * invdet;

                return new Matrix2D(m11, m12, m21, m22, dx, dy);
            }
        }

        public static Matrix2D Transformation(float xScale, float yScale, float rotation, float dx, float dy)
        {
            float m11 = xScale * MathF.Cos(rotation);
            float m12 = -MathF.Sin(rotation);
            float m21 = MathF.Sin(rotation);
            float m22 = yScale * MathF.Cos(rotation);

            return new Matrix2D(m11, m12, m21, m22, dx, dy);
        }

        public static Matrix2D Scale(float xScale, float yScale)
        {
            float m11 = xScale;
            float m22 = yScale;

            return new Matrix2D(m11, 0, 0, m22, 0, 0);
        }

        public static Matrix2D Scale(float uniformScale)
        {
            return Scale(uniformScale, uniformScale);
        }

        public static Matrix2D Scale(Point2D basePoint, float xScale, float yScale)
        {
            return Translation(basePoint.X, basePoint.Y) *
                Scale(xScale, yScale) *
                Translation(-basePoint.X, -basePoint.Y);
        }

        public static Matrix2D Scale(Point2D basePoint, float uniformScale)
        {
            return Scale(basePoint, uniformScale, uniformScale);
        }

        public static Matrix2D Mirror(Point2D basePoint, Vector2D direction)
        {
            return Translation(basePoint.X, basePoint.Y) *
                Rotation(direction.Angle) *
                Scale(1, -1) *
                Rotation(-direction.Angle) *
                Translation(-basePoint.X, -basePoint.Y);
        }

        public static Matrix2D Rotation(float rotation)
        {
            float m11 = MathF.Cos(-rotation);
            float m12 = MathF.Sin(-rotation);
            float m21 = -MathF.Sin(-rotation);
            float m22 = MathF.Cos(-rotation);

            return new Matrix2D(m11, m12, m21, m22, 0, 0);
        }

        public static Matrix2D Rotation(Point2D basePoint, float rotation)
        {
            return Translation(basePoint.X, basePoint.Y) *
                Rotation(rotation) *
                Translation(-basePoint.X, -basePoint.Y);
        }

        public static Matrix2D Translation(Vector2D delta)
        {
            return Translation(delta.X, delta.Y);
        }

        public static Matrix2D Translation(float dx, float dy)
        {
            return new Matrix2D(1, 0, 0, 1, dx, dy);
        }

        public static Matrix2D operator *(Matrix2D a, Matrix2D b)
        {
            float m11 = a.M11 * b.M11 + a.M12 * b.M21 + a.DX * 0;
            float m12 = a.M11 * b.M12 + a.M12 * b.M22 + a.DX * 0;
            float dx = a.M11 * b.DX + a.M12 * b.DY + a.DX * 1;
            float m21 = a.M21 * b.M11 + a.M22 * b.M21 + a.DY * 0;
            float m22 = a.M21 * b.M12 + a.M22 * b.M22 + a.DY * 0;
            float dy = a.M21 * b.DX + a.M22 * b.DY + a.DY * 1;
            return new Matrix2D(m11, m12, m21, m22, dx, dy);
        }

        public static explicit operator Matrix(Matrix2D t)
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
