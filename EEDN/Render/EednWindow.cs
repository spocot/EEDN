using EEDN.Themes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace EEDN.Render
{
    public class EednWindow : GameWindow
    {
        public Theme CurrentTheme { get; set; } = new DarculaTheme();

        private MultisampleRenderTarget _Target;
        private EednEngine _Engine;
        private DrawBuffer _DrawBuffer;
        private GlyphCache _GlyfCache = new GlyphCache();

        public EednWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            Title = "EEDN";
            VSync = VSyncMode.Off;

            _DrawBuffer = new DrawBuffer(_GlyfCache, Size);
            _Engine = new EednEngine(_DrawBuffer, CurrentTheme);
            _Target = new MultisampleRenderTarget(_DrawBuffer, Size.X, Size.Y);
        }

        protected override void OnLoad()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthClamp);
            GL.Enable(EnableCap.Multisample);
            GL.ClearColor(CurrentTheme.BgColor);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            _DrawBuffer.Resize(e.Width, e.Height);
            _Target.Dispose();
            _Target = new MultisampleRenderTarget(_DrawBuffer, e.Width, e.Height);
        }

        private List<float> _fpsAvg = new List<float>();

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            _Target.Bind();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _fpsAvg.Add((float) (1f / (args.Time + 0.00000001f)));

            _Engine.Draw();

            string fpsString = $"Fps: {MathF.Round(_fpsAvg.Average(), MidpointRounding.ToEven)}";
            _DrawBuffer.DrawString(CurrentTheme.GutterColor, fpsString, 24,
                new Point(_DrawBuffer.ScreenSize.Width - (fpsString.Length * 24), 0));

            _DrawBuffer.Flush();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            _Target.Blit();

            SwapBuffers();

            if (_fpsAvg.Count == 1000) _fpsAvg.RemoveAt(0);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) => _Engine.ProcessKey(e);
    }
}
