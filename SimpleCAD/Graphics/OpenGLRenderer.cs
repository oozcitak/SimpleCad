using SimpleCAD.Geometry;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleCAD.Graphics
{
    public class OpenGLRenderer : Renderer
    {
        private System.Drawing.Graphics gdi;
        private Control control;
        private IntPtr hDC;
        private IntPtr glContext;
        private GL.ContextSwitch glSwitch;
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

            // Get the device context
            hDC = GL.GetDC(control.Handle);

            // Choose a pixel format
            GL.PIXELFORMATDESCRIPTOR pfd = new GL.PIXELFORMATDESCRIPTOR();
            pfd.nSize = (ushort)Marshal.SizeOf(typeof(GL.PIXELFORMATDESCRIPTOR));
            pfd.nVersion = 1;
            pfd.dwFlags = GL.PFD_FLAGS.PFD_DRAW_TO_WINDOW | GL.PFD_FLAGS.PFD_SUPPORT_OPENGL | GL.PFD_FLAGS.PFD_DOUBLEBUFFER | GL.PFD_FLAGS.PFD_SUPPORT_COMPOSITION;
            pfd.iPixelType = GL.PFD_PIXEL_TYPE.PFD_TYPE_RGBA;
            pfd.cColorBits = 32;
            pfd.cRedBits = pfd.cRedShift = pfd.cGreenBits = pfd.cGreenShift = pfd.cBlueBits = pfd.cBlueShift = 0;
            pfd.cAlphaBits = pfd.cAlphaShift = 0;
            pfd.cAccumBits = pfd.cAccumRedBits = pfd.cAccumGreenBits = pfd.cAccumBlueBits = pfd.cAccumAlphaBits = 0;
            pfd.cDepthBits = 32;
            pfd.cStencilBits = pfd.cAuxBuffers = 0;
            pfd.iLayerType = GL.PFD_LAYER_TYPES.PFD_MAIN_PLANE;
            pfd.bReserved = 0;
            pfd.dwLayerMask = pfd.dwVisibleMask = pfd.dwDamageMask = 0;

            // Set the format
            int iPixelFormat = GL.ChoosePixelFormat(hDC, ref pfd);
            GL.SetPixelFormat(hDC, iPixelFormat, ref pfd);
            IsAccelerated = (pfd.dwFlags & GL.PFD_FLAGS.PFD_GENERIC_FORMAT) == 0;

            // Create the render context
            glContext = GL.wglCreateContext(hDC);
            GL.wglMakeCurrent(hDC, glContext);

            // Set the viewport
            GL.glViewport(0, 0, ctrl.Width, ctrl.Height);

            // Set OpenGL parameters
            GL.glDisable(GL.GL_LIGHTING);
            GL.glShadeModel(GL.GL_FLAT);
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.glEnable(GL.GL_POLYGON_SMOOTH);
            GL.glEnable(GL.GL_POINT_SMOOTH);
            GL.glHint(GL.GL_POLYGON_SMOOTH_HINT, GL.GL_DONT_CARE);
            GL.glEnable(GL.GL_TEXTURE_2D);

            GL.glGenTextures(1, out textTextureID);
        }

        public override void InitFrame(System.Drawing.Graphics graphics)
        {
            gdi = graphics;

            glSwitch = new GL.ContextSwitch(hDC, glContext);

            // Set model-view transformation
            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();
            GL.glOrtho(View.CameraPosition.X - ((float)control.ClientRectangle.Width) * View.ZoomFactor / 2,
                View.CameraPosition.X + ((float)control.ClientRectangle.Width) * View.ZoomFactor / 2,
                View.CameraPosition.Y - ((float)control.ClientRectangle.Height) * View.ZoomFactor / 2,
                View.CameraPosition.Y + ((float)control.ClientRectangle.Height) * View.ZoomFactor / 2,
                -1.0f, 1.0f);

            // Set the model matrix as the current matrix
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();
        }

        public override void EndFrame()
        {
            // Swap buffers
            GL.SwapBuffers(hDC);

            glSwitch.Dispose();
        }

        public override void Resize(int width, int height)
        {
            using (new GL.ContextSwitch(hDC, glContext))
            {
                // Reset the current viewport
                GL.glViewport(0, 0, width, height);
            }
        }

        public override void Dispose()
        {
            GL.glDeleteTextures(1, ref textTextureID);

            GL.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            GL.wglDeleteContext(glContext);
            if (control != null)
                GL.ReleaseDC(control.Handle, hDC);
        }

        public override void Clear(Color color)
        {
            GL.glClearColor(((float)color.R) / 255, ((float)color.G) / 255, ((float)color.B) / 255, ((float)color.A) / 255);
            GL.glClear(GL.GL_COLOR_BUFFER_BIT);
        }

        public override void DrawLine(Style style, Point2D p1, Point2D p2)
        {
            GL.glLoadIdentity();
            CreatePen(style);

            GL.glBegin(GL.GL_LINES);
            GL.glVertex2f(p1.X, p1.Y);
            GL.glVertex2f(p2.X, p2.Y);
            GL.glEnd();
        }

        public override void DrawRectangle(Style style, Point2D p1, Point2D p2)
        {
            GL.glLoadIdentity();

            if (style.Fill)
            {
                CreateBrush(style);

                GL.glBegin(GL.GL_TRIANGLE_FAN);
                GL.glVertex2f(p1.X, p1.Y);
                GL.glVertex2f(p1.X, p2.Y);
                GL.glVertex2f(p2.X, p2.Y);
                GL.glVertex2f(p2.X, p1.Y);
                GL.glEnd();
            }
            else
            {
                CreatePen(style);

                GL.glBegin(GL.GL_LINE_LOOP);
                GL.glVertex2f(p1.X, p1.Y);
                GL.glVertex2f(p2.X, p1.Y);
                GL.glVertex2f(p2.X, p2.Y);
                GL.glVertex2f(p1.X, p2.Y);
                GL.glEnd();
            }
        }

        public override void DrawCircle(Style style, Point2D center, float radius)
        {
            GL.glLoadIdentity();
            GL.glTranslatef(center.X, center.Y, 0);

            float curveLength = MathF.PI * radius * radius;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;

            if (style.Fill)
            {
                CreateBrush(style);

                GL.glBegin(GL.GL_TRIANGLE_FAN);
                GL.glVertex2f(center.X, center.Y);

                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = radius * MathF.Cos(a);
                    float y = radius * MathF.Sin(a);
                    GL.glVertex2f(x, y);
                    a += da;
                }
                GL.glVertex2f(radius, 0);
                GL.glEnd();
            }
            else
            {
                CreatePen(style);

                GL.glBegin(GL.GL_LINE_LOOP);
                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = radius * MathF.Cos(a);
                    float y = radius * MathF.Sin(a);
                    GL.glVertex2f(x, y);
                    a += da;
                }
                GL.glEnd();
            }
        }

        public override void DrawArc(Style style, Point2D center, float radius, float startAngle, float endAngle)
        {
            GL.glLoadIdentity();
            CreatePen(style);

            GL.glBegin(GL.GL_LINE_STRIP);
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
                GL.glVertex2f(x, y);
                a += da;
            }
            GL.glEnd();
        }

        public override void DrawEllipse(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float rotation)
        {
            GL.glLoadIdentity();
            GL.glTranslatef(center.X, center.Y, 0);
            GL.glRotatef(rotation * 180 / MathF.PI, 0, 0, 1);

            float p = 2 * MathF.PI * (3 * (semiMajorAxis + semiMinorAxis) - MathF.Sqrt((3 * semiMajorAxis + semiMinorAxis) * (semiMajorAxis + 3 * semiMinorAxis)));
            float curveLength = View.WorldToScreen(new Vector2D(p, 0)).X;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;

            if (style.Fill)
            {
                CreateBrush(style);

                GL.glBegin(GL.GL_TRIANGLE_FAN);
                GL.glVertex2f(0, 0);
                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = semiMajorAxis * MathF.Cos(a);
                    float y = semiMinorAxis * MathF.Sin(a);
                    GL.glVertex2f(x, y);
                    a += da;
                }
                GL.glVertex2f(semiMajorAxis, 0);
                GL.glEnd();
            }
            else
            {
                CreatePen(style);

                GL.glBegin(GL.GL_LINE_LOOP);
                float a = 0;
                for (int i = 0; i <= n; i++)
                {
                    float x = semiMajorAxis * MathF.Cos(a);
                    float y = semiMinorAxis * MathF.Sin(a);
                    GL.glVertex2f(x, y);
                    a += da;
                }
                GL.glEnd();
            }
        }

        public override void DrawEllipticArc(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float startAngle, float endAngle, float rotation)
        {
            GL.glLoadIdentity();
            GL.glTranslatef(center.X, center.Y, 0);
            GL.glRotatef(rotation * 180 / MathF.PI, 0, 0, 1);

            float p = 2 * MathF.PI * (3 * (semiMajorAxis + semiMinorAxis) - MathF.Sqrt((3 * semiMajorAxis + semiMinorAxis) * (semiMajorAxis + 3 * semiMinorAxis)));
            float curveLength = View.WorldToScreen(new Vector2D(p, 0)).X;
            float sweepAngle = endAngle - startAngle;
            while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
            while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
            int n = (int)Math.Max(4, curveLength / 4);
            float a = startAngle;
            float da = sweepAngle / n;

            CreatePen(style);

            GL.glBegin(GL.GL_LINE_STRIP);
            for (int i = 0; i <= n; i++)
            {
                float dx = MathF.Cos(a) * semiMinorAxis;
                float dy = MathF.Sin(a) * semiMajorAxis;
                float t = MathF.Atan2(dy, dx);

                float x = semiMajorAxis * MathF.Cos(t);
                float y = semiMinorAxis * MathF.Sin(t);

                GL.glVertex2f(x, y);
                a += da;
            }
            GL.glEnd();
        }

        public override void DrawPolyline(Style style, Point2DCollection points, bool closed)
        {
            if (points.Count > 1)
            {
                GL.glLoadIdentity();
                CreatePen(style);

                GL.glBegin(closed ? GL.GL_LINE_LOOP : GL.GL_LINE_STRIP);
                foreach (Point2D pt in points)
                {
                    GL.glVertex2f(pt.X, pt.Y);
                }
                GL.glEnd();
            }
        }

        public override void DrawPolygon(Style style, Point2DCollection points)
        {
            if (points.Count > 1)
            {
                GL.glLoadIdentity();
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

                    GL.glBegin(GL.GL_TRIANGLE_FAN);
                    GL.glVertex2f(x, y);
                    foreach (Point2D pt in points)
                    {
                        GL.glVertex2f(pt.X, pt.Y);
                    }
                    GL.glVertex2f(points[0].X, points[0].Y);
                    GL.glEnd();
                }
                else
                {
                    CreatePen(style);

                    GL.glBegin(GL.GL_LINE_LOOP);
                    foreach (Point2D pt in points)
                    {
                        GL.glVertex2f(pt.X, pt.Y);
                    }
                    GL.glEnd();
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

                    GL.glBindTexture(GL.GL_TEXTURE_2D, textTextureID);

                    //GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_MODULATE);
                    GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGBA, srcimage.Width, srcimage.Height, 0, GL.GL_BGRA_EXT, GL.GL_UNSIGNED_BYTE, data.Scan0);
                    srcimage.UnlockBits(data);

                    GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                    GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);
                    GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_REPEAT);
                    GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_REPEAT);

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

                    GL.glColor4ub(255, 255, 255, 255);

                    GL.glLoadIdentity();
                    GL.glTranslatef(pt.X, pt.Y, 0);
                    GL.glRotatef(rotation * 180 / MathF.PI, 0, 0, 1);
                    GL.glScalef(textHeight / height, textHeight / height, textHeight / height);
                    GL.glTranslatef(dx, dy, 0);

                    GL.glBegin(GL.GL_TRIANGLE_FAN);

                    GL.glTexCoord2f(0, 1);
                    GL.glVertex2f(0, 0);

                    GL.glTexCoord2f(1, 1);
                    GL.glVertex2f(srcimage.Width, 0);

                    GL.glTexCoord2f(1, 0);
                    GL.glVertex2f(srcimage.Width, srcimage.Height);

                    GL.glTexCoord2f(0, 0);
                    GL.glVertex2f(0, srcimage.Height);

                    GL.glEnd();

                    GL.glBindTexture(GL.GL_TEXTURE_2D, 0);
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

            GL.glLineWidth(1);
            GL.glColor4ub(style.Color.R, style.Color.G, style.Color.B, style.Color.A);
            GL.glLineWidth(GetScaledLineWeight(style.LineWeight));
            // DashStyle??
        }

        private void CreateBrush(Style style)
        {
            if (StyleOverride != null)
                style = StyleOverride;

            GL.glColor4ub(style.Color.R, style.Color.G, style.Color.B, style.Color.A);
        }
    }
}
