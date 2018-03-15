using SimpleCAD.Geometry;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleCAD.Graphics
{
    public class OpenGLRenderer : Renderer
    {
        #region Context switch class
        public class ContextSwitch : IDisposable
        {
            private IntPtr hOldDC;
            private IntPtr oldContext;
            bool contextDifferent;

            public ContextSwitch(IntPtr hDC, IntPtr context)
            {
                // Save previous context and make our context current
                contextDifferent = (SafeNativeMethods.wglGetCurrentContext() != context);
                hOldDC = IntPtr.Zero;
                oldContext = IntPtr.Zero;

                if (contextDifferent)
                {
                    hOldDC = SafeNativeMethods.wglGetCurrentDC();
                    oldContext = SafeNativeMethods.wglGetCurrentContext();
                    SafeNativeMethods.wglMakeCurrent(hDC, context);
                }
            }

            ~ContextSwitch()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected void Dispose(bool disposing)
            {
                // Restore previous context
                if (contextDifferent)
                {
                    SafeNativeMethods.wglMakeCurrent(hOldDC, oldContext);
                }
            }
        }
        #endregion

        private System.Drawing.Graphics gdi;
        private Control control;
        private IntPtr hDC;
        private IntPtr glContext;
        private SafeNativeMethods.ContextSwitch glSwitch;
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
            hDC = SafeNativeMethods.GetDC(control.Handle);

            // Choose a pixel format
            SafeNativeMethods.PIXELFORMATDESCRIPTOR pfd = new SafeNativeMethods.PIXELFORMATDESCRIPTOR();
            pfd.nSize = (ushort)Marshal.SizeOf(typeof(SafeNativeMethods.PIXELFORMATDESCRIPTOR));
            pfd.nVersion = 1;
            pfd.dwFlags = SafeNativeMethods.PFD_FLAGS.PFD_DRAW_TO_WINDOW | SafeNativeMethods.PFD_FLAGS.PFD_SUPPORT_OPENGL | SafeNativeMethods.PFD_FLAGS.PFD_DOUBLEBUFFER | SafeNativeMethods.PFD_FLAGS.PFD_SUPPORT_COMPOSITION;
            pfd.iPixelType = SafeNativeMethods.PFD_PIXEL_TYPE.PFD_TYPE_RGBA;
            pfd.cColorBits = 32;
            pfd.cRedBits = pfd.cRedShift = pfd.cGreenBits = pfd.cGreenShift = pfd.cBlueBits = pfd.cBlueShift = 0;
            pfd.cAlphaBits = pfd.cAlphaShift = 0;
            pfd.cAccumBits = pfd.cAccumRedBits = pfd.cAccumGreenBits = pfd.cAccumBlueBits = pfd.cAccumAlphaBits = 0;
            pfd.cDepthBits = 32;
            pfd.cStencilBits = pfd.cAuxBuffers = 0;
            pfd.iLayerType = SafeNativeMethods.PFD_LAYER_TYPES.PFD_MAIN_PLANE;
            pfd.bReserved = 0;
            pfd.dwLayerMask = pfd.dwVisibleMask = pfd.dwDamageMask = 0;

            // Set the format
            int iPixelFormat = SafeNativeMethods.ChoosePixelFormat(hDC, ref pfd);
            SafeNativeMethods.SetPixelFormat(hDC, iPixelFormat, ref pfd);
            IsAccelerated = (pfd.dwFlags & SafeNativeMethods.PFD_FLAGS.PFD_GENERIC_FORMAT) == 0;

            // Create the render context
            glContext = SafeNativeMethods.wglCreateContext(hDC);
            SafeNativeMethods.wglMakeCurrent(hDC, glContext);

            // Set the viewport
            SafeNativeMethods.glViewport(0, 0, ctrl.Width, ctrl.Height);

            // Set OpenGL parameters
            SafeNativeMethods.glDisable(SafeNativeMethods.GL_LIGHTING);
            SafeNativeMethods.glShadeModel(SafeNativeMethods.GL_FLAT);
            SafeNativeMethods.glEnable(SafeNativeMethods.GL_BLEND);
            SafeNativeMethods.glBlendFunc(SafeNativeMethods.GL_SRC_ALPHA, SafeNativeMethods.GL_ONE_MINUS_SRC_ALPHA);
            SafeNativeMethods.glEnable(SafeNativeMethods.GL_POLYGON_SMOOTH);
            SafeNativeMethods.glEnable(SafeNativeMethods.GL_POINT_SMOOTH);
            SafeNativeMethods.glHint(SafeNativeMethods.GL_POLYGON_SMOOTH_HINT, SafeNativeMethods.GL_DONT_CARE);
            SafeNativeMethods.glEnable(SafeNativeMethods.GL_TEXTURE_2D);

            SafeNativeMethods.glGenTextures(1, out textTextureID);
        }

        public override void InitFrame(System.Drawing.Graphics graphics)
        {
            gdi = graphics;

            glSwitch = new SafeNativeMethods.ContextSwitch(hDC, glContext);

            // Set model-view transformation
            SafeNativeMethods.glMatrixMode(SafeNativeMethods.GL_PROJECTION);
            SafeNativeMethods.glLoadIdentity();
            SafeNativeMethods.glOrtho(View.Camera.Position.X - ((float)control.ClientRectangle.Width) * View.Camera.Zoom / 2,
                View.Camera.Position.X + ((float)control.ClientRectangle.Width) * View.Camera.Zoom / 2,
                View.Camera.Position.Y - ((float)control.ClientRectangle.Height) * View.Camera.Zoom / 2,
                View.Camera.Position.Y + ((float)control.ClientRectangle.Height) * View.Camera.Zoom / 2,
                -1.0f, 1.0f);

            // Set the model matrix as the current matrix
            SafeNativeMethods.glMatrixMode(SafeNativeMethods.GL_MODELVIEW);
            SafeNativeMethods.glLoadIdentity();
        }

        public override void EndFrame()
        {
            // Swap buffers
            SafeNativeMethods.SwapBuffers(hDC);

            glSwitch.Dispose();
        }

        public override void Resize(int width, int height)
        {
            using (new SafeNativeMethods.ContextSwitch(hDC, glContext))
            {
                // Reset the current viewport
                SafeNativeMethods.glViewport(0, 0, width, height);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                glSwitch.Dispose();
            }

            SafeNativeMethods.glDeleteTextures(1, ref textTextureID);

            SafeNativeMethods.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            SafeNativeMethods.wglDeleteContext(glContext);
            if (control != null)
                SafeNativeMethods.ReleaseDC(control.Handle, hDC);
        }

        public override void Clear(Color color)
        {
            SafeNativeMethods.glClearColor(((float)color.R) / 255, ((float)color.G) / 255, ((float)color.B) / 255, ((float)color.A) / 255);
            SafeNativeMethods.glClear(SafeNativeMethods.GL_COLOR_BUFFER_BIT);
        }

        public override void DrawLine(Style style, Point2D p1, Point2D p2)
        {
            SafeNativeMethods.glLoadIdentity();
            CreatePen(style);

            SafeNativeMethods.glBegin(SafeNativeMethods.GL_LINES);
            SafeNativeMethods.glVertex2f(p1.X, p1.Y);
            SafeNativeMethods.glVertex2f(p2.X, p2.Y);
            SafeNativeMethods.glEnd();
        }

        public override void DrawRectangle(Style style, Point2D p1, Point2D p2)
        {
            SafeNativeMethods.glLoadIdentity();

            if (style.Fill)
            {
                CreateBrush(style);

                SafeNativeMethods.glBegin(SafeNativeMethods.GL_TRIANGLE_FAN);
                SafeNativeMethods.glVertex2f(p1.X, p1.Y);
                SafeNativeMethods.glVertex2f(p1.X, p2.Y);
                SafeNativeMethods.glVertex2f(p2.X, p2.Y);
                SafeNativeMethods.glVertex2f(p2.X, p1.Y);
                SafeNativeMethods.glEnd();
            }
            else
            {
                CreatePen(style);

                SafeNativeMethods.glBegin(SafeNativeMethods.GL_LINE_LOOP);
                SafeNativeMethods.glVertex2f(p1.X, p1.Y);
                SafeNativeMethods.glVertex2f(p2.X, p1.Y);
                SafeNativeMethods.glVertex2f(p2.X, p2.Y);
                SafeNativeMethods.glVertex2f(p1.X, p2.Y);
                SafeNativeMethods.glEnd();
            }
        }

        public override void DrawCircle(Style style, Point2D center, float radius)
        {
            SafeNativeMethods.glLoadIdentity();
            SafeNativeMethods.glTranslatef(center.X, center.Y, 0);

            float curveLength = MathF.PI * radius * radius;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;

            if (style.Fill)
            {
                CreateBrush(style);

                SafeNativeMethods.glBegin(SafeNativeMethods.GL_TRIANGLE_FAN);
                SafeNativeMethods.glVertex2f(center.X, center.Y);

                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = radius * MathF.Cos(a);
                    float y = radius * MathF.Sin(a);
                    SafeNativeMethods.glVertex2f(x, y);
                    a += da;
                }
                SafeNativeMethods.glVertex2f(radius, 0);
                SafeNativeMethods.glEnd();
            }
            else
            {
                CreatePen(style);

                SafeNativeMethods.glBegin(SafeNativeMethods.GL_LINE_LOOP);
                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = radius * MathF.Cos(a);
                    float y = radius * MathF.Sin(a);
                    SafeNativeMethods.glVertex2f(x, y);
                    a += da;
                }
                SafeNativeMethods.glEnd();
            }
        }

        public override void DrawArc(Style style, Point2D center, float radius, float startAngle, float endAngle)
        {
            SafeNativeMethods.glLoadIdentity();
            CreatePen(style);

            SafeNativeMethods.glBegin(SafeNativeMethods.GL_LINE_STRIP);
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
                SafeNativeMethods.glVertex2f(x, y);
                a += da;
            }
            SafeNativeMethods.glEnd();
        }

        public override void DrawEllipse(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float rotation)
        {
            SafeNativeMethods.glLoadIdentity();
            SafeNativeMethods.glTranslatef(center.X, center.Y, 0);
            SafeNativeMethods.glRotatef(rotation * 180 / MathF.PI, 0, 0, 1);

            float p = 2 * MathF.PI * (3 * (semiMajorAxis + semiMinorAxis) - MathF.Sqrt((3 * semiMajorAxis + semiMinorAxis) * (semiMajorAxis + 3 * semiMinorAxis)));
            float curveLength = View.WorldToScreen(new Vector2D(p, 0)).X;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;

            if (style.Fill)
            {
                CreateBrush(style);

                SafeNativeMethods.glBegin(SafeNativeMethods.GL_TRIANGLE_FAN);
                SafeNativeMethods.glVertex2f(0, 0);
                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = semiMajorAxis * MathF.Cos(a);
                    float y = semiMinorAxis * MathF.Sin(a);
                    SafeNativeMethods.glVertex2f(x, y);
                    a += da;
                }
                SafeNativeMethods.glVertex2f(semiMajorAxis, 0);
                SafeNativeMethods.glEnd();
            }
            else
            {
                CreatePen(style);

                SafeNativeMethods.glBegin(SafeNativeMethods.GL_LINE_LOOP);
                float a = 0;
                for (int i = 0; i <= n; i++)
                {
                    float x = semiMajorAxis * MathF.Cos(a);
                    float y = semiMinorAxis * MathF.Sin(a);
                    SafeNativeMethods.glVertex2f(x, y);
                    a += da;
                }
                SafeNativeMethods.glEnd();
            }
        }

        public override void DrawEllipticArc(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float startAngle, float endAngle, float rotation)
        {
            SafeNativeMethods.glLoadIdentity();
            SafeNativeMethods.glTranslatef(center.X, center.Y, 0);
            SafeNativeMethods.glRotatef(rotation * 180 / MathF.PI, 0, 0, 1);

            float p = 2 * MathF.PI * (3 * (semiMajorAxis + semiMinorAxis) - MathF.Sqrt((3 * semiMajorAxis + semiMinorAxis) * (semiMajorAxis + 3 * semiMinorAxis)));
            float curveLength = View.WorldToScreen(new Vector2D(p, 0)).X;
            float sweepAngle = endAngle - startAngle;
            while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
            while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
            int n = (int)Math.Max(4, curveLength / 4);
            float a = startAngle;
            float da = sweepAngle / n;

            CreatePen(style);

            SafeNativeMethods.glBegin(SafeNativeMethods.GL_LINE_STRIP);
            for (int i = 0; i <= n; i++)
            {
                float dx = MathF.Cos(a) * semiMinorAxis;
                float dy = MathF.Sin(a) * semiMajorAxis;
                float t = MathF.Atan2(dy, dx);

                float x = semiMajorAxis * MathF.Cos(t);
                float y = semiMinorAxis * MathF.Sin(t);

                SafeNativeMethods.glVertex2f(x, y);
                a += da;
            }
            SafeNativeMethods.glEnd();
        }

        public override void DrawPolyline(Style style, Point2DCollection points, bool closed)
        {
            if (points.Count > 1)
            {
                SafeNativeMethods.glLoadIdentity();
                CreatePen(style);

                SafeNativeMethods.glBegin(closed ? SafeNativeMethods.GL_LINE_LOOP : SafeNativeMethods.GL_LINE_STRIP);
                foreach (Point2D pt in points)
                {
                    SafeNativeMethods.glVertex2f(pt.X, pt.Y);
                }
                SafeNativeMethods.glEnd();
            }
        }

        public override void DrawPolygon(Style style, Point2DCollection points)
        {
            if (points.Count > 1)
            {
                SafeNativeMethods.glLoadIdentity();
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

                    SafeNativeMethods.glBegin(SafeNativeMethods.GL_TRIANGLE_FAN);
                    SafeNativeMethods.glVertex2f(x, y);
                    foreach (Point2D pt in points)
                    {
                        SafeNativeMethods.glVertex2f(pt.X, pt.Y);
                    }
                    SafeNativeMethods.glVertex2f(points[0].X, points[0].Y);
                    SafeNativeMethods.glEnd();
                }
                else
                {
                    CreatePen(style);

                    SafeNativeMethods.glBegin(SafeNativeMethods.GL_LINE_LOOP);
                    foreach (Point2D pt in points)
                    {
                        SafeNativeMethods.glVertex2f(pt.X, pt.Y);
                    }
                    SafeNativeMethods.glEnd();
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

                    SafeNativeMethods.glBindTexture(SafeNativeMethods.GL_TEXTURE_2D, textTextureID);

                    //GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_MODULATE);
                    SafeNativeMethods.glTexImage2D(SafeNativeMethods.GL_TEXTURE_2D, 0, (int)SafeNativeMethods.GL_RGBA, srcimage.Width, srcimage.Height, 0, SafeNativeMethods.GL_BGRA_EXT, SafeNativeMethods.GL_UNSIGNED_BYTE, data.Scan0);
                    srcimage.UnlockBits(data);

                    SafeNativeMethods.glTexParameteri(SafeNativeMethods.GL_TEXTURE_2D, SafeNativeMethods.GL_TEXTURE_MIN_FILTER, (int)SafeNativeMethods.GL_LINEAR);
                    SafeNativeMethods.glTexParameteri(SafeNativeMethods.GL_TEXTURE_2D, SafeNativeMethods.GL_TEXTURE_MAG_FILTER, (int)SafeNativeMethods.GL_LINEAR);
                    SafeNativeMethods.glTexParameteri(SafeNativeMethods.GL_TEXTURE_2D, SafeNativeMethods.GL_TEXTURE_WRAP_S, (int)SafeNativeMethods.GL_REPEAT);
                    SafeNativeMethods.glTexParameteri(SafeNativeMethods.GL_TEXTURE_2D, SafeNativeMethods.GL_TEXTURE_WRAP_T, (int)SafeNativeMethods.GL_REPEAT);

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

                    SafeNativeMethods.glColor4ub(255, 255, 255, 255);

                    SafeNativeMethods.glLoadIdentity();
                    SafeNativeMethods.glTranslatef(pt.X, pt.Y, 0);
                    SafeNativeMethods.glRotatef(rotation * 180 / MathF.PI, 0, 0, 1);
                    SafeNativeMethods.glScalef(textHeight / height, textHeight / height, textHeight / height);
                    SafeNativeMethods.glTranslatef(dx, dy, 0);

                    SafeNativeMethods.glBegin(SafeNativeMethods.GL_TRIANGLE_FAN);

                    SafeNativeMethods.glTexCoord2f(0, 1);
                    SafeNativeMethods.glVertex2f(0, 0);

                    SafeNativeMethods.glTexCoord2f(1, 1);
                    SafeNativeMethods.glVertex2f(srcimage.Width, 0);

                    SafeNativeMethods.glTexCoord2f(1, 0);
                    SafeNativeMethods.glVertex2f(srcimage.Width, srcimage.Height);

                    SafeNativeMethods.glTexCoord2f(0, 0);
                    SafeNativeMethods.glVertex2f(0, srcimage.Height);

                    SafeNativeMethods.glEnd();

                    SafeNativeMethods.glBindTexture(SafeNativeMethods.GL_TEXTURE_2D, 0);
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

            SafeNativeMethods.glLineWidth(1);
            SafeNativeMethods.glColor4ub(style.Color.R, style.Color.G, style.Color.B, style.Color.A);
            SafeNativeMethods.glLineWidth(GetScaledLineWeight(style.LineWeight));
            // DashStyle??
        }

        private void CreateBrush(Style style)
        {
            if (StyleOverride != null)
                style = StyleOverride;

            SafeNativeMethods.glColor4ub(style.Color.R, style.Color.G, style.Color.B, style.Color.A);
        }
    }
}
