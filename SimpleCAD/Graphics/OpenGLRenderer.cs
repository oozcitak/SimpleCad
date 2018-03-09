using SimpleCAD.Geometry;
using System;
using System.Runtime.InteropServices;

namespace SimpleCAD.Graphics
{
    public class OpenGLRenderer : Renderer
    {
        private System.Drawing.Graphics gdi;
        private System.Windows.Forms.Control control;
        private IntPtr hDC;
        private IntPtr glContext;
        private GLContextSwitch glSwitch;

        private string currentFontFmaily;
        private FontStyle currentFontStyle;
        private uint vectorBase;
        TEXTMETRIC textMetric;

        public bool IsAccelerated { get; private set; }

        public OpenGLRenderer(CADView view) : base(view)
        {
            ;
        }

        public override void Init(System.Windows.Forms.Control ctrl)
        {
            // Get the device context
            control = ctrl;
            hDC = GetDC(control.Handle);

            // Choose a pixel format
            PIXELFORMATDESCRIPTOR pfd = new PIXELFORMATDESCRIPTOR();
            pfd.nSize = (ushort)Marshal.SizeOf(typeof(PIXELFORMATDESCRIPTOR));
            pfd.nVersion = 1;
            pfd.dwFlags = PFD_FLAGS.PFD_DRAW_TO_WINDOW | PFD_FLAGS.PFD_SUPPORT_OPENGL | PFD_FLAGS.PFD_DOUBLEBUFFER | PFD_FLAGS.PFD_SUPPORT_COMPOSITION;
            pfd.iPixelType = PFD_PIXEL_TYPE.PFD_TYPE_RGBA;
            pfd.cColorBits = 32;
            pfd.cRedBits = pfd.cRedShift = pfd.cGreenBits = pfd.cGreenShift = pfd.cBlueBits = pfd.cBlueShift = 0;
            pfd.cAlphaBits = pfd.cAlphaShift = 0;
            pfd.cAccumBits = pfd.cAccumRedBits = pfd.cAccumGreenBits = pfd.cAccumBlueBits = pfd.cAccumAlphaBits = 0;
            pfd.cDepthBits = 32;
            pfd.cStencilBits = pfd.cAuxBuffers = 0;
            pfd.iLayerType = PFD_LAYER_TYPES.PFD_MAIN_PLANE;
            pfd.bReserved = 0;
            pfd.dwLayerMask = pfd.dwVisibleMask = pfd.dwDamageMask = 0;

            // Set the format
            int iPixelFormat = ChoosePixelFormat(hDC, ref pfd);
            SetPixelFormat(hDC, iPixelFormat, ref pfd);
            IsAccelerated = (pfd.dwFlags & PFD_FLAGS.PFD_GENERIC_FORMAT) == 0;

            // Create the render context
            glContext = wglCreateContext(hDC);
            wglMakeCurrent(hDC, glContext);

            // Set the viewport
            glViewport(0, 0, 300, 300);

            // Set OpenGL parameters
            glDisable(GL_LIGHTING);
            glShadeModel(GL_FLAT);
            glEnable(GL_BLEND);
            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
        }

        public override void InitFrame(System.Drawing.Graphics graphics)
        {
            gdi = graphics;

            glSwitch = new GLContextSwitch(hDC, glContext);

            glMatrixMode(GL_PROJECTION);
            glLoadIdentity();
            glOrtho(View.CameraPosition.X - ((float)control.ClientRectangle.Width) * View.ZoomFactor / 2,
                View.CameraPosition.X + ((float)control.ClientRectangle.Width) * View.ZoomFactor / 2,
                View.CameraPosition.Y - ((float)control.ClientRectangle.Height) * View.ZoomFactor / 2,
                View.CameraPosition.Y + ((float)control.ClientRectangle.Height) * View.ZoomFactor / 2,
                -1.0f, 1.0f);

            // Set the model matrix as the current matrix
            glMatrixMode(GL_MODELVIEW);
            glLoadIdentity();
        }

        public override void EndFrame()
        {
            // Swap buffers
            SwapBuffers(hDC);

            glSwitch.Dispose();
        }

        public override void Resize(int width, int height)
        {
            using (new GLContextSwitch(hDC, glContext))
            {
                // Reset the current viewport
                glViewport(0, 0, width, height);
            }
        }

        public override void Dispose()
        {
            glDeleteLists(vectorBase, 256);
            wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            wglDeleteContext(glContext);
            if (control != null)
                ReleaseDC(control.Handle, hDC);
        }

        public override void ClearFrame(Color color)
        {
            glClearColor(((float)color.R) / 255, ((float)color.G) / 255, ((float)color.B) / 255, ((float)color.A) / 255);
            glClear(GL_COLOR_BUFFER_BIT);
        }

        public override void DrawLine(Style style, Point2D p1, Point2D p2)
        {
            glLoadIdentity();
            CreatePen(style);

            glBegin(GL_LINES);
            glVertex2f(p1.X, p1.Y);
            glVertex2f(p2.X, p2.Y);
            glEnd();
        }

        public override void DrawRectangle(Style style, Point2D p1, Point2D p2)
        {
            glLoadIdentity();

            if (style.Fill)
            {
                CreateBrush(style);

                glBegin(GL_TRIANGLE_FAN);
                glVertex2f(p1.X, p1.Y);
                glVertex2f(p1.X, p2.Y);
                glVertex2f(p2.X, p2.Y);
                glVertex2f(p2.X, p1.Y);
                glEnd();
            }
            else
            {
                CreatePen(style);

                glBegin(GL_LINE_LOOP);
                glVertex2f(p1.X, p1.Y);
                glVertex2f(p2.X, p1.Y);
                glVertex2f(p2.X, p2.Y);
                glVertex2f(p1.X, p2.Y);
                glEnd();
            }
        }

        public override void DrawCircle(Style style, Point2D center, float radius)
        {
            glLoadIdentity();
            glTranslatef(center.X, center.Y, 0);

            float curveLength = MathF.PI * radius * radius;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;

            if (style.Fill)
            {
                CreateBrush(style);

                glBegin(GL_TRIANGLE_FAN);
                glVertex2f(center.X, center.Y);

                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = radius * MathF.Cos(a);
                    float y = radius * MathF.Sin(a);
                    glVertex2f(x, y);
                    a += da;
                }
                glVertex2f(radius, 0);
                glEnd();
            }
            else
            {
                CreatePen(style);

                glBegin(GL_LINE_LOOP);
                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = radius * MathF.Cos(a);
                    float y = radius * MathF.Sin(a);
                    glVertex2f(x, y);
                    a += da;
                }
                glEnd();
            }
        }

        public override void DrawArc(Style style, Point2D center, float radius, float startAngle, float endAngle)
        {
            glLoadIdentity();
            CreatePen(style);

            glBegin(GL_LINE_STRIP);
            float sweepAngle = endAngle - startAngle;
            while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
            while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
            float curveLength = MathF.PI * radius * radius * sweepAngle;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = sweepAngle / n;
            float a = startAngle;
            for (int i = 0; i <= n; i++)
            {
                Vector2D dir = Vector2D.FromAngle(a);
                float x = radius * dir.X;
                float y = radius * dir.Y;
                glVertex2f(x, y);
                a += da;
            }
            glEnd();
        }

        public override void DrawEllipse(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float rotation)
        {
            glLoadIdentity();
            glRotatef(rotation, 0, 0, 1);
            glTranslatef(center.X, center.Y, 0);

            float p = 2 * MathF.PI * (3 * (semiMajorAxis + semiMinorAxis) - MathF.Sqrt((3 * semiMajorAxis + semiMinorAxis) * (semiMajorAxis + 3 * semiMinorAxis)));
            float curveLength = View.WorldToScreen(new Vector2D(p, 0)).X;
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * MathF.PI / n;

            if (style.Fill)
            {
                CreateBrush(style);

                glBegin(GL_TRIANGLE_FAN);
                glVertex2f(0, 0);
                float a = 0;
                for (int i = 0; i < n; i++)
                {
                    float x = semiMajorAxis * MathF.Cos(a);
                    float y = semiMinorAxis * MathF.Sin(a);
                    glVertex2f(x, y);
                    a += da;
                }
                glVertex2f(semiMajorAxis, 0);
                glEnd();
            }
            else
            {
                CreatePen(style);

                glBegin(GL_LINE_LOOP);
                float a = 0;
                for (int i = 0; i <= n; i++)
                {
                    float x = semiMajorAxis * MathF.Cos(a);
                    float y = semiMinorAxis * MathF.Sin(a);
                    glVertex2f(x, y);
                    a += da;
                }
                glEnd();
            }
        }

        public override void DrawEllipticArc(Style style, Point2D center, float semiMajorAxis, float semiMinorAxis, float startAngle, float endAngle, float rotation)
        {
            glLoadIdentity();
            glRotatef(rotation, 0, 0, 1);
            glTranslatef(center.X, center.Y, 0);

            float p = 2 * MathF.PI * (3 * (semiMajorAxis + semiMinorAxis) - MathF.Sqrt((3 * semiMajorAxis + semiMinorAxis) * (semiMajorAxis + 3 * semiMinorAxis)));
            float curveLength = View.WorldToScreen(new Vector2D(p, 0)).X;
            float sweepAngle = endAngle - startAngle;
            while (sweepAngle < 0) sweepAngle += 2 * MathF.PI;
            while (sweepAngle > 2 * MathF.PI) sweepAngle -= 2 * MathF.PI;
            int n = (int)Math.Max(4, curveLength / 4);
            float a = startAngle;
            float da = sweepAngle / n;

            CreatePen(style);

            glBegin(GL_LINE_STRIP);
            for (int i = 0; i <= n; i++)
            {
                float dx = MathF.Cos(a) * semiMinorAxis;
                float dy = MathF.Sin(a) * semiMajorAxis;
                float t = MathF.Atan2(dy, dx);

                float x = semiMajorAxis * MathF.Cos(t);
                float y = semiMinorAxis * MathF.Sin(t);

                glVertex2f(x, y);
                a += da;
            }
            glEnd();
        }

        public override void DrawPolyline(Style style, Point2DCollection points, bool closed)
        {
            if (points.Count > 1)
            {
                glLoadIdentity();
                CreatePen(style);

                glBegin(closed ? GL_LINE_LOOP : GL_LINE_STRIP);
                foreach (Point2D pt in points)
                {
                    glVertex2f(pt.X, pt.Y);
                }
                glEnd();
            }
        }

        public override void DrawPolygon(Style style, Point2DCollection points)
        {
            if (points.Count > 1)
            {
                glLoadIdentity();
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

                    glBegin(GL_TRIANGLE_FAN);
                    glVertex2f(x, y);
                    foreach (Point2D pt in points)
                    {
                        glVertex2f(pt.X, pt.Y);
                    }
                    glVertex2f(points[0].X, points[0].Y);
                    glEnd();
                }
                else
                {
                    CreatePen(style);

                    glBegin(GL_LINE_LOOP);
                    foreach (Point2D pt in points)
                    {
                        glVertex2f(pt.X, pt.Y);
                    }
                    glEnd();
                }
            }
        }

        public override Vector2D MeasureString(string text, string fontFamily, FontStyle fontStyle, float textHeight)
        {
            CreateFont(fontFamily, FontStyle.Regular);

            int width = 0; // pixels
            foreach (char c in text)
            {
                width += MeasureCharWidth(c);
            }

            POINT pt0 = new POINT();
            pt0.x = 0;
            pt0.y = 0;
            POINT pt1 = new POINT();
            pt1.x = width;
            pt1.y = textMetric.tmHeight;
            POINT[] pts = new POINT[] { pt0, pt1 };
            LPtoDP(hDC, ref pts, 2);
            width = pts[1].x - pts[0].x;
            float height = pts[1].y - pts[0].y;

            return View.ScreenToWorld(new Vector2D(width, height)) * textHeight;
        }

        private int MeasureCharWidth(char c)
        {
            ABC[] glyph = new ABC[1];
            GetCharABCWidths(hDC, c, c, glyph);
            return glyph[0].A + (int)glyph[0].B + glyph[0].C;
        }

        public override void DrawString(Style style, Point2D pt, string text,
            string fontFamily, float textHeight, FontStyle fontStyle,
            float rotation, TextHorizontalAlignment hAlign, TextVerticalAlignment vAlign)
        {
            CreateFont(fontFamily, FontStyle.Regular);

            CreateBrush(style);

            // Calculate alignment in pixel coordinates
            float dx = 0;
            float dy = 0;
            var sz = MeasureString(text, fontFamily, fontStyle, textHeight);

            if (hAlign == TextHorizontalAlignment.Right)
                dx = -sz.X;
            else if (hAlign == TextHorizontalAlignment.Center)
                dx = -sz.X / 2;

            if (vAlign == TextVerticalAlignment.Bottom)
                dy = -sz.Y;
            else if (vAlign == TextVerticalAlignment.Middle)
                dy = -sz.Y / 2;

            glLoadIdentity();

            glTranslatef(dx, dy, 0);
            glScalef(textHeight, textHeight, textHeight);

            glListBase(vectorBase);

            glLoadIdentity();
            glTranslatef(pt.X, pt.Y, 0);
            glRotatef(rotation, 0, 0, 1);
            glScalef(textHeight, textHeight, textHeight);

            IntPtr str = Marshal.StringToHGlobalAnsi(text);
            glCallLists(text.Length, GL_UNSIGNED_BYTE, str);
            Marshal.FreeHGlobal(str);
        }

        public override void Draw(Drawable item)
        {
            item.Draw(this);
        }

        private System.Drawing.Pen CreatePen(Style style)
        {
            if (StyleOverride != null)
                style = StyleOverride;

            glLineWidth(1);
            glColor4ub(style.Color.R, style.Color.G, style.Color.B, style.Color.A);
            glLineWidth(GetScaledLineWeight(style.LineWeight));
            // DashStyle??

            return new System.Drawing.Pen(System.Drawing.Color.Transparent);
        }

        private System.Drawing.Brush CreateBrush(Style style)
        {
            if (StyleOverride != null)
                style = StyleOverride;

            glColor4ub(style.Color.R, style.Color.G, style.Color.B, style.Color.A);

            return new System.Drawing.SolidBrush(System.Drawing.Color.Transparent);
        }

        private void CreateFont(string fontFamily, FontStyle fontStyle)
        {
            if (fontFamily == currentFontFmaily && fontStyle == currentFontStyle)
                return;

            using (System.Drawing.Font font = new System.Drawing.Font(fontFamily, 1, (System.Drawing.FontStyle)fontStyle, System.Drawing.GraphicsUnit.World))
            {
                glDeleteLists(vectorBase, 256);

                IntPtr oldFont = SelectObject(hDC, font.ToHfont());
                vectorBase = glGenLists(256);
                // wglUseFontBitmaps(hDC, 0, 256, rasterBase);
                wglUseFontOutlines(hDC, 0, 256, vectorBase, 0, 0, WGL_FONT_POLYGONS, null);

                textMetric = new TEXTMETRIC();
                GetTextMetrics(hDC, ref textMetric);

                SelectObject(hDC, oldFont);
            }

            currentFontFmaily = fontFamily;
            currentFontStyle = fontStyle;
        }

        #region GLContextSwitch Class
        private class GLContextSwitch : IDisposable
        {
            private IntPtr hOldDC;
            private IntPtr oldContext;
            bool contextDifferent;

            public GLContextSwitch(IntPtr hDC, IntPtr context)
            {
                // Save previous context and make our context current
                contextDifferent = (wglGetCurrentContext() != context);
                hOldDC = IntPtr.Zero;
                oldContext = IntPtr.Zero;

                if (contextDifferent)
                {
                    hOldDC = wglGetCurrentDC();
                    oldContext = wglGetCurrentContext();
                    wglMakeCurrent(hDC, context);
                }
            }

            public void Dispose()
            {
                // Restore previous context
                if (contextDifferent)
                {
                    wglMakeCurrent(hOldDC, oldContext);
                }
            }
        }
        #endregion

        #region Platform Invoke
        private const uint GL_BYTE = 0x1400;
        private const uint GL_UNSIGNED_BYTE = 0x1401;
        private const uint GL_SHORT = 0x1402;
        private const uint GL_UNSIGNED_SHORT = 0x1403;
        private const uint GL_INT = 0x1404;
        private const uint GL_UNSIGNED_INT = 0x1405;
        private const uint GL_FLOAT = 0x1406;
        private const uint GL_2_BYTES = 0x1407;
        private const uint GL_3_BYTES = 0x1408;
        private const uint GL_4_BYTES = 0x1409;
        private const uint GL_DOUBLE = 0x140A;

        private const uint GL_FLAT = 0x1D00;
        private const uint GL_SMOOTH = 0x1D01;

        private const uint GL_BLEND_DST = 0x0BE0;
        private const uint GL_BLEND_SRC = 0x0BE1;
        private const uint GL_BLEND = 0x0BE2;

        private const uint GL_ZERO = 0;
        private const uint GL_ONE = 1;
        private const uint GL_SRC_COLOR = 0x0300;
        private const uint GL_ONE_MINUS_SRC_COLOR = 0x0301;
        private const uint GL_SRC_ALPHA = 0x0302;
        private const uint GL_ONE_MINUS_SRC_ALPHA = 0x0303;
        private const uint GL_DST_ALPHA = 0x0304;
        private const uint GL_ONE_MINUS_DST_ALPHA = 0x0305;

        private const uint GL_LIGHTING = 0x0B50;

        private const uint GL_MODELVIEW = 0x1700;
        private const uint GL_PROJECTION = 0x1701;
        private const uint GL_TEXTURE = 0x1702;

        private const uint GL_CURRENT_BIT = 0x00000001;
        private const uint GL_POINT_BIT = 0x00000002;
        private const uint GL_LINE_BIT = 0x00000004;
        private const uint GL_POLYGON_BIT = 0x00000008;
        private const uint GL_POLYGON_STIPPLE_BIT = 0x00000010;
        private const uint GL_PIXEL_MODE_BIT = 0x00000020;
        private const uint GL_LIGHTING_BIT = 0x00000040;
        private const uint GL_FOG_BIT = 0x00000080;
        private const uint GL_DEPTH_BUFFER_BIT = 0x00000100;
        private const uint GL_ACCUM_BUFFER_BIT = 0x00000200;
        private const uint GL_STENCIL_BUFFER_BIT = 0x00000400;
        private const uint GL_VIEWPORT_BIT = 0x00000800;
        private const uint GL_TRANSFORM_BIT = 0x00001000;
        private const uint GL_ENABLE_BIT = 0x00002000;
        private const uint GL_COLOR_BUFFER_BIT = 0x00004000;
        private const uint GL_HINT_BIT = 0x00008000;
        private const uint GL_EVAL_BIT = 0x00010000;
        private const uint GL_LIST_BIT = 0x00020000;
        private const uint GL_TEXTURE_BIT = 0x00040000;
        private const uint GL_SCISSOR_BIT = 0x00080000;
        private const uint GL_ALL_ATTRIB_BITS = 0x000fffff;

        private const uint GL_POINTS = 0x0000;
        private const uint GL_LINES = 0x0001;
        private const uint GL_LINE_LOOP = 0x0002;
        private const uint GL_LINE_STRIP = 0x0003;
        private const uint GL_TRIANGLES = 0x0004;
        private const uint GL_TRIANGLE_STRIP = 0x0005;
        private const uint GL_TRIANGLE_FAN = 0x0006;
        private const uint GL_QUADS = 0x0007;
        private const uint GL_QUAD_STRIP = 0x0008;
        private const uint GL_POLYGON = 0x0009;

        private const uint WGL_FONT_LINES = 0;
        private const uint WGL_FONT_POLYGONS = 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [StructLayout(LayoutKind.Sequential)]
        private struct PIXELFORMATDESCRIPTOR
        {
            public ushort nSize;
            public ushort nVersion;
            public PFD_FLAGS dwFlags;
            public PFD_PIXEL_TYPE iPixelType;
            public byte cColorBits;
            public byte cRedBits;
            public byte cRedShift;
            public byte cGreenBits;
            public byte cGreenShift;
            public byte cBlueBits;
            public byte cBlueShift;
            public byte cAlphaBits;
            public byte cAlphaShift;
            public byte cAccumBits;
            public byte cAccumRedBits;
            public byte cAccumGreenBits;
            public byte cAccumBlueBits;
            public byte cAccumAlphaBits;
            public byte cDepthBits;
            public byte cStencilBits;
            public byte cAuxBuffers;
            public PFD_LAYER_TYPES iLayerType;
            public byte bReserved;
            public uint dwLayerMask;
            public uint dwVisibleMask;
            public uint dwDamageMask;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ABC
        {
            public int A;
            public uint B;
            public int C;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TEXTMETRIC
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct GLYPHMETRICSFLOAT
        {
            public float gmfBlackBoxX;
            public float gmfBlackBoxY;
            public POINTFLOAT gmfptGlyphOrigin;
            public float gmfCellIncX;
            public float gmfCellIncY;
        }

        private struct POINTFLOAT
        {
            public float x;
            public float y;
        }

        private struct POINT
        {
            public int x;
            public int y;
        }

        [Flags]
        private enum PFD_FLAGS : uint
        {
            PFD_DOUBLEBUFFER = 0x00000001,
            PFD_STEREO = 0x00000002,
            PFD_DRAW_TO_WINDOW = 0x00000004,
            PFD_DRAW_TO_BITMAP = 0x00000008,
            PFD_SUPPORT_GDI = 0x00000010,
            PFD_SUPPORT_OPENGL = 0x00000020,
            PFD_GENERIC_FORMAT = 0x00000040,
            PFD_NEED_PALETTE = 0x00000080,
            PFD_NEED_SYSTEM_PALETTE = 0x00000100,
            PFD_SWAP_EXCHANGE = 0x00000200,
            PFD_SWAP_COPY = 0x00000400,
            PFD_SWAP_LAYER_BUFFERS = 0x00000800,
            PFD_GENERIC_ACCELERATED = 0x00001000,
            PFD_SUPPORT_DIRECTDRAW = 0x00002000,
            PFD_DIRECT3D_ACCELERATED = 0x00004000,
            PFD_SUPPORT_COMPOSITION = 0x00008000,
            PFD_DEPTH_DONTCARE = 0x20000000,
            PFD_DOUBLEBUFFER_DONTCARE = 0x40000000,
            PFD_STEREO_DONTCARE = 0x80000000
        }

        private enum PFD_LAYER_TYPES : byte
        {
            PFD_MAIN_PLANE = 0,
            PFD_OVERLAY_PLANE = 1,
            PFD_UNDERLAY_PLANE = 255
        }

        private enum PFD_PIXEL_TYPE : byte
        {
            PFD_TYPE_RGBA = 0,
            PFD_TYPE_COLORINDEX = 1
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ChoosePixelFormat(IntPtr hdc, ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetPixelFormat(IntPtr hdc, int iPixelFormat, ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool SwapBuffers(IntPtr deviceContext);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool GetCharABCWidths(IntPtr hdc, uint uFirstChar, uint uLastChar, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeConst = 1)] ABC[] lpabc);
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool GetTextMetrics(IntPtr hdc, ref TEXTMETRIC lptm);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool LPtoDP(IntPtr hdc, [In, Out] ref POINT[] lpPoints, int nCount);

        [DllImport("opengl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr wglCreateContext(IntPtr hDC);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern int wglMakeCurrent(IntPtr hdc, IntPtr hrc);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern int wglDeleteContext(IntPtr hdc);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern IntPtr wglGetCurrentContext();
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern IntPtr wglGetCurrentDC();
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern bool wglUseFontBitmaps(IntPtr hdc, uint first, uint count, uint listBase);
        [DllImport("opengl32", SetLastError = true)]
        private static extern bool wglUseFontOutlines(IntPtr hDC, uint first, uint count, uint listBase, float deviation, float extrusion, uint format, [Out, MarshalAs(UnmanagedType.LPArray)] GLYPHMETRICSFLOAT[] lpgmf);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glViewport(int x, int y, int width, int height);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glShadeModel(uint mode);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glEnable(uint cap);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glDisable(uint cap);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glBlendFunc(uint src, uint dest);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glMatrixMode(uint mode);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glLoadIdentity();
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glTranslatef(float x, float y, float z);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glScalef(float x, float y, float z);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glRotatef(float angle, float x, float y, float z);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glOrtho(double left, double right, double bottom, double top, double zNear, double zFar);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glClearColor(float red, float green, float blue, float alpha);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glClear(uint mask);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glFinish();

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glBegin(uint mode);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glEnd();

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glColor4ub(byte red, byte green, byte blue, byte alpha);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glVertex2f(float x, float y);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glLineWidth(float width);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern uint glGenLists(int range);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glDeleteLists(uint list, int range);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glCallList(uint list);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glListBase(uint listbase);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glRasterPos2f(float x, float y);
        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern void glCallLists(int n, uint type, IntPtr lists);
        #endregion
    }
}
