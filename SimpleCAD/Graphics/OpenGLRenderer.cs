using SimpleCAD.Geometry;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenGL;

namespace SimpleCAD.Graphics
{
    public class OpenGLRenderer : Renderer
    {
        private System.Drawing.Graphics gdi;
        private Control control;
        private DeviceContext device;
        private IntPtr glContext;
        private uint textTextureID;

        public override string Name => "OpenGL Renderer";
        public bool IsAccelerated { get; private set; }

        public OpenGLRenderer(CADView view) : base(view)
        {
            ;
        }

        public override void Init(Control ctrl)
        {
            control = ctrl;

            try
            {
                // Disable double buffering
                Type type = control.GetType();
                MethodInfo method = type.GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(control, new object[] { ControlStyles.DoubleBuffer, false });
            }
            catch (System.Security.SecurityException)
            {
                ;
            }

            device = DeviceContext.Create(IntPtr.Zero, control.Handle);
            device.ChoosePixelFormat(new DevicePixelFormat(32));

            // Create the render context
            glContext = device.CreateContext(IntPtr.Zero);
            device.MakeCurrent(glContext);

            // Set the viewport
            Gl.Viewport(0, 0, ctrl.Width, ctrl.Height);

            // Set OpenGL parameters
            Gl.Disable(EnableCap.Lighting);
            Gl.ShadeModel(ShadingModel.Flat);
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.Enable(EnableCap.PolygonSmooth);
            Gl.Enable(EnableCap.PointSmooth);
            Gl.Hint(HintTarget.PolygonSmoothHint, HintMode.DontCare);
            Gl.Enable(EnableCap.Texture2d);

            uint[] textures = new uint[1];
            Gl.GenTextures(textures);
            textTextureID = textures[0];
        }

        public override void InitFrame(System.Drawing.Graphics graphics)
        {
            gdi = graphics;

            device.MakeCurrent(glContext);

            // Set model-view transformation
            Gl.MatrixMode(MatrixMode.Projection);
            Gl.LoadIdentity();
            Gl.Ortho((double)View.Camera.Position.X - ((double)control.ClientRectangle.Width) * (double)View.Camera.Zoom / 2.0,
                (double)View.Camera.Position.X + ((double)control.ClientRectangle.Width) * (double)View.Camera.Zoom / 2.0,
                (double)View.Camera.Position.Y - ((double)control.ClientRectangle.Height) * (double)View.Camera.Zoom / 2.0,
                (double)View.Camera.Position.Y + ((double)control.ClientRectangle.Height) * (double)View.Camera.Zoom / 2.0,
                -1.0, 1.0);

            // Set the model matrix as the current matrix
            Gl.MatrixMode(MatrixMode.Modelview);
            Gl.LoadIdentity();
        }

        public override void EndFrame()
        {
            // Swap buffers
            device.SwapBuffers();
        }

        public override void Resize(int width, int height)
        {
            device.MakeCurrent(glContext);
            Gl.Viewport(0, 0, width, height);
        }

        protected override void Dispose(bool disposing)
        {
            device.MakeCurrent(IntPtr.Zero);
            device.DeleteContext(glContext);
        }

        public override void Clear(Color color)
        {
            Gl.ClearColor(((float)color.R) / 255, ((float)color.G) / 255, ((float)color.B) / 255, ((float)color.A) / 255);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public override void DrawLine(Style style, Point2D p1, Point2D p2)
        {
            Gl.LoadIdentity();
            CreatePen(style);

            Gl.Begin(PrimitiveType.Lines);
            Gl.Vertex2(p1.X, p1.Y);
            Gl.Vertex2(p2.X, p2.Y);
            Gl.End();
        }

        public override void DrawRectangle(Style style, Point2D p1, Point2D p2)
        {
            Gl.LoadIdentity();

            if (style.Fill)
            {
                CreateBrush(style);

                Gl.Begin(PrimitiveType.TriangleFan);
                Gl.Vertex2(p1.X, p1.Y);
                Gl.Vertex2(p1.X, p2.Y);
                Gl.Vertex2(p2.X, p2.Y);
                Gl.Vertex2(p2.X, p1.Y);
                Gl.End();
            }
            else
            {
                CreatePen(style);

                Gl.Begin(PrimitiveType.Lines);
                Gl.Vertex2(p1.X, p1.Y);
                Gl.Vertex2(p2.X, p1.Y);
                Gl.Vertex2(p2.X, p2.Y);
                Gl.Vertex2(p1.X, p2.Y);
                Gl.End();
            }
        }

        public override void DrawCircle(Style style, Point2D center, float radius)
        {
            Gl.LoadIdentity();
            Gl.Translate(center.X, center.Y, 0);

            float curveLength = MathF.PI * radius * radius;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;

            if (style.Fill)
            {
                CreateBrush(style);

                Gl.Begin(PrimitiveType.TriangleFan);
                Gl.Vertex2(center.X, center.Y);

                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = radius * MathF.Cos(a);
                    float y = radius * MathF.Sin(a);
                    Gl.Vertex2(x, y);
                    a += da;
                }
                Gl.Vertex2(radius, 0);
                Gl.End();
            }
            else
            {
                CreatePen(style);

                Gl.Begin(PrimitiveType.Lines);
                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = radius * MathF.Cos(a);
                    float y = radius * MathF.Sin(a);
                    Gl.Vertex2(x, y);
                    a += da;
                }
                Gl.End();
            }
        }

        public override void DrawArc(Style style, Point2D center, float radius, float startAngle, float endAngle)
        {
            Gl.LoadIdentity();
            CreatePen(style);

            Gl.Begin(PrimitiveType.LineStrip);
            float sweepAngle = endAngle - startAngle;
            while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
            while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
            float p = MathF.PI * radius * sweepAngle;
            float curveLength = View.WorldToScreen(new Vector2D(p, 0)).X;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = sweepAngle / n;
            float a = startAngle;
            for (int i = 0; i <= n; i++)
            {
                Vector2D dir = Vector2D.FromAngle(a);
                float x = center.X + radius * dir.X;
                float y = center.Y + radius * dir.Y;
                Gl.Vertex2(x, y);
                a += da;
            }
            Gl.End();
        }

        public override void DrawEllipse(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float rotation)
        {
            Gl.LoadIdentity();
            Gl.Translate(center.X, center.Y, 0);
            Gl.Rotate(rotation * 180 / MathF.PI, 0, 0, 1);

            float p = 2 * MathF.PI * (3 * (semiMajorAxis + semiMinorAxis) - MathF.Sqrt((3 * semiMajorAxis + semiMinorAxis) * (semiMajorAxis + 3 * semiMinorAxis)));
            float curveLength = View.WorldToScreen(new Vector2D(p, 0)).X;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;

            if (style.Fill)
            {
                CreateBrush(style);

                Gl.Begin(PrimitiveType.TriangleFan);
                Gl.Vertex2(0, 0);
                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = semiMajorAxis * MathF.Cos(a);
                    float y = semiMinorAxis * MathF.Sin(a);
                    Gl.Vertex2(x, y);
                    a += da;
                }
                Gl.Vertex2(semiMajorAxis, 0);
                Gl.End();
            }
            else
            {
                CreatePen(style);

                Gl.Begin(PrimitiveType.Lines);
                float a = 0;
                for (int i = 0; i <= n; i++)
                {
                    float x = semiMajorAxis * MathF.Cos(a);
                    float y = semiMinorAxis * MathF.Sin(a);
                    Gl.Vertex2(x, y);
                    a += da;
                }
                Gl.End();
            }
        }

        public override void DrawEllipticArc(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float startAngle, float endAngle, float rotation)
        {
            Gl.LoadIdentity();
            Gl.Translate(center.X, center.Y, 0);
            Gl.Rotate(rotation * 180 / MathF.PI, 0, 0, 1);

            float p = 2 * MathF.PI * (3 * (semiMajorAxis + semiMinorAxis) - MathF.Sqrt((3 * semiMajorAxis + semiMinorAxis) * (semiMajorAxis + 3 * semiMinorAxis)));
            float curveLength = View.WorldToScreen(new Vector2D(p, 0)).X;
            float sweepAngle = endAngle - startAngle;
            while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
            while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
            int n = (int)Math.Max(4, curveLength / 4);
            float a = startAngle;
            float da = sweepAngle / n;

            CreatePen(style);

            Gl.Begin(PrimitiveType.LineStrip);
            for (int i = 0; i <= n; i++)
            {
                float dx = MathF.Cos(a) * semiMinorAxis;
                float dy = MathF.Sin(a) * semiMajorAxis;
                float t = MathF.Atan2(dy, dx);

                float x = semiMajorAxis * MathF.Cos(t);
                float y = semiMinorAxis * MathF.Sin(t);

                Gl.Vertex2(x, y);
                a += da;
            }
            Gl.End();
        }

        public override void DrawPolyline(Style style, Point2DCollection points, bool closed)
        {
            if (points.Count > 1)
            {
                Gl.LoadIdentity();
                CreatePen(style);

                Gl.Begin(closed ? PrimitiveType.Lines : PrimitiveType.LineStrip);
                foreach (Point2D pt in points)
                {
                    Gl.Vertex2(pt.X, pt.Y);
                }
                Gl.End();
            }
        }

        public override void DrawPolygon(Style style, Point2DCollection points)
        {
            if (points.Count > 1)
            {
                Gl.LoadIdentity();
                if (style.Fill)
                {
                    float x = 0;
                    float y = 0;
                    foreach (Point2D pt in points)
                    {
                        x += pt.X / points.Count;
                        y += pt.Y / points.Count;
                    }

                    CreateBrush(style);

                    Gl.Begin(PrimitiveType.TriangleFan);
                    Gl.Vertex2(x, y);
                    foreach (Point2D pt in points)
                    {
                        Gl.Vertex2(pt.X, pt.Y);
                    }
                    Gl.Vertex2(points[0].X, points[0].Y);
                    Gl.End();
                }
                else
                {
                    CreatePen(style);

                    Gl.Begin(PrimitiveType.Lines);
                    foreach (Point2D pt in points)
                    {
                        Gl.Vertex2(pt.X, pt.Y);
                    }
                    Gl.End();
                }
            }
        }

        public override Vector2D MeasureString(string text, string fontFamily, FontStyle fontStyle, float textHeight)
        {
            // Revert transformation to identity while drawing text
            var oldMatrix = gdi.Transform;
            gdi.ResetTransform();

            // Calculate alignment in pixel coordinates
            float height = Math.Abs(View.WorldToScreen(new Vector2D(0, textHeight)).Y);
            Vector2D szWorld;
            using (var font = new System.Drawing.Font(fontFamily, height, (System.Drawing.FontStyle)fontStyle, System.Drawing.GraphicsUnit.Pixel))
            {
                var sz = gdi.MeasureString(text, font);
                szWorld = View.ScreenToWorld(new Vector2D(Math.Abs(sz.Width), Math.Abs(sz.Height)));
            }

            // Restore old transformation
            gdi.Transform = oldMatrix;

            return new Vector2D(Math.Abs(szWorld.X), Math.Abs(szWorld.Y));
        }

        public override void DrawString(Style style, Point2D pt, string text,
            string fontFamily, float textHeight, FontStyle fontStyle,
            float rotation, TextHorizontalAlignment hAlign, TextVerticalAlignment vAlign)
        {
            float height = Math.Abs(View.WorldToScreen(new Vector2D(0, textHeight)).Y);
            using (var font = new System.Drawing.Font(fontFamily, height, (System.Drawing.FontStyle)fontStyle, System.Drawing.GraphicsUnit.Pixel))
            using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb((int)style.Color.Argb)))
            {
                var sz = gdi.MeasureString(text, font);
                var szWorld = View.ScreenToWorld(new Vector2D(Math.Abs(sz.Width), Math.Abs(sz.Height)));

                using (var srcimage = new System.Drawing.Bitmap((int)MathF.Ceiling(sz.Width), (int)MathF.Ceiling(sz.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                using (var srcgraph = System.Drawing.Graphics.FromImage(srcimage))
                {
                    srcgraph.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    using (var backBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0, 0)))
                    {
                        srcgraph.FillRectangle(backBrush, 0, 0, srcimage.Width, srcimage.Height);
                    }
                    srcgraph.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                    srcgraph.DrawString(text, font, brush, 0, 0);

                    var data = srcimage.LockBits(new System.Drawing.Rectangle(0, 0, srcimage.Width, srcimage.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly, srcimage.PixelFormat);

                    Gl.BindTexture(TextureTarget.Texture2d, textTextureID);

                    //GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_MODULATE);
                    Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, srcimage.Width, srcimage.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    srcimage.UnlockBits(data);

                    Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                    Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                    // Calculate alignment offset
                    float dx = 0;
                    float dy = 0;

                    if (hAlign == TextHorizontalAlignment.Right)
                        dx = -sz.Width;
                    else if (hAlign == TextHorizontalAlignment.Center)
                        dx = -sz.Width / 2;

                    if (vAlign == TextVerticalAlignment.Middle)
                        dy = -sz.Height / 2;
                    else if (vAlign == TextVerticalAlignment.Top)
                        dy = -sz.Height;

                    Gl.Color4(255, 255, 255, 255);

                    Gl.LoadIdentity();
                    Gl.Translate(pt.X, pt.Y, 0);
                    Gl.Rotate(rotation * 180 / MathF.PI, 0, 0, 1);
                    Gl.Scale(textHeight / height, textHeight / height, textHeight / height);
                    Gl.Translate(dx, dy, 0);

                    Gl.Begin(PrimitiveType.TriangleFan);

                    Gl.TexCoord2(0, 1);
                    Gl.Vertex2(0, 0);

                    Gl.TexCoord2(1, 1);
                    Gl.Vertex2(srcimage.Width, 0);

                    Gl.TexCoord2(1, 0);
                    Gl.Vertex2(srcimage.Width, srcimage.Height);

                    Gl.TexCoord2(0, 0);
                    Gl.Vertex2(0, srcimage.Height);

                    Gl.End();

                    Gl.BindTexture(TextureTarget.Texture2d, 0);
                }
            }
        }

        public override void Draw(Drawable item)
        {
            item.Draw(this);
        }

        private void CreatePen(Style style)
        {
            if (StyleOverride != null)
                style = StyleOverride;

            Gl.LineWidth(1);
            Gl.Color4(style.Color.R, style.Color.G, style.Color.B, style.Color.A);
            Gl.LineWidth(GetScaledLineWeight(style.LineWeight));

            if (style.DashStyle == DashStyle.Solid)
                Gl.Disable(EnableCap.LineStipple);
            else
                Gl.Enable(EnableCap.LineStipple);

            switch (style.DashStyle)
            {
                case DashStyle.Dot:
                    Gl.LineStipple(1, 0b0001000100010001);
                    break;
                case DashStyle.Dash:
                    Gl.LineStipple(1, 0b0000111100001111);
                    break;
                case DashStyle.DashDot:
                    Gl.LineStipple(1, 0b0001110001000111);
                    break;
                case DashStyle.DashDotDot:
                    Gl.LineStipple(1, 0b0011110001000100);
                    break;
            }
        }

        private void CreateBrush(Style style)
        {
            if (StyleOverride != null)
                style = StyleOverride;

            Gl.Color4(style.Color.R, style.Color.G, style.Color.B, style.Color.A);
        }
    }
}
