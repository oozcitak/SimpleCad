using System;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;

namespace SimpleCAD.Geometry
{
    public struct Matrix2D
    {
        public float M11 { get; }
        public float M12 { get; }
        public float M21 { get; }
        public float M22 { get; }
        public float DX { get; }
        public float DY { get; }

        public float RotationAngle { get { return Vector2D.XAxis.Transform(this).Angle; } }

        public static Matrix2D Identity { get { return new Matrix2D(1, 0, 0, 1, 0, 0); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2D(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            M11 = m11; M12 = m12;
            M21 = m21; M22 = m22;
            DX = dx; DY = dy;
        }

        public Matrix2D Inverse
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Transformation(float xScale, float yScale, float rotation, float dx, float dy)
        {
            float m11 = xScale * MathF.Cos(rotation);
            float m12 = -MathF.Sin(rotation);
            float m21 = MathF.Sin(rotation);
            float m22 = yScale * MathF.Cos(rotation);

            return new Matrix2D(m11, m12, m21, m22, dx, dy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Scale(float xScale, float yScale)
        {
            float m11 = xScale;
            float m22 = yScale;

            return new Matrix2D(m11, 0, 0, m22, 0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Scale(float uniformScale)
        {
            return Scale(uniformScale, uniformScale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Scale(Point2D basePoint, float xScale, float yScale)
        {
            return Translation(basePoint.X, basePoint.Y) *
                Scale(xScale, yScale) *
                Translation(-basePoint.X, -basePoint.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Scale(Point2D basePoint, float uniformScale)
        {
            return Scale(basePoint, uniformScale, uniformScale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Mirror(Point2D basePoint, Vector2D direction)
        {
            return Translation(basePoint.X, basePoint.Y) *
                Rotation(direction.Angle) *
                Scale(1, -1) *
                Rotation(-direction.Angle) *
                Translation(-basePoint.X, -basePoint.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Rotation(float rotation)
        {
            float m11 = MathF.Cos(-rotation);
            float m12 = MathF.Sin(-rotation);
            float m21 = -MathF.Sin(-rotation);
            float m22 = MathF.Cos(-rotation);

            return new Matrix2D(m11, m12, m21, m22, 0, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Rotation(Point2D basePoint, float rotation)
        {
            return Translation(basePoint.X, basePoint.Y) *
                Rotation(rotation) *
                Translation(-basePoint.X, -basePoint.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Translation(Vector2D delta)
        {
            return Translation(delta.X, delta.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2D Translation(float dx, float dy)
        {
            return new Matrix2D(1, 0, 0, 1, dx, dy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
