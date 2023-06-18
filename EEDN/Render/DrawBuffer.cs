using EEDN.TTF;
using OpenTK.Mathematics;

namespace EEDN.Render
{
    public class DrawBuffer
    {
        public const float LINE_HEIGHT = 1.0f;
        public const int SPACES_IN_TAB = 4;

        public Size ScreenSize;

        private Shader _lineShader;
        private Shader _rectShader;
        private Matrix4f _orthMat;
        private Mesh _quad;
        private GlyphCache _GlyfCache;
        private TtfFile _ttf = new TtfFile();

        public DrawBuffer(GlyphCache glyfCache, Size size)
        {
            _GlyfCache = glyfCache;
            ScreenSize = size;

            _ttf.Load("./Fonts/Sauce Code Pro Nerd Font Complete Mono Windows Compatible.ttf");
            _lineShader = new Shader(File.ReadAllText("./Shaders/line.glsl"));
            _rectShader = new Shader(File.ReadAllText("./Shaders/rect.glsl"));
            _quad = new Mesh();
            _quad.Load(
                (0, 0),
                (1, 0),
                (1, 1),
                (1, 1),
                (0, 1),
                (0, 0));

            _orthMat = new Matrix4f().InitIdentity().InitOrthographic(0, size.Width, size.Height, 0, -1, 1);
        }

        public void Resize(int width, int height)
        {
            var orth = new Matrix4f().InitIdentity().InitOrthographic(0, width, height, 0, -1, 1);

            _orthMat = orth;

            ScreenSize = new Size(width, height);
        }

        public void Flush() { }

        public void DrawRect(Color4 c, Rect r)
        {
            _rectShader.Apply();

            var trans = new Matrix4f().InitIdentity().InitTranslation(r.Location.X, r.Location.Y, 0);
            var scale = new Matrix4f().InitIdentity().InitScale(r.Size.Width, r.Size.Height, 0);

            _rectShader.SetUniform("mvp", _orthMat * trans * scale);
            _rectShader.SetUniform("uColor", c);
            _quad.Draw();
        }

        public Size MeasureString(string s, float size)
        {
            var lineHeight = 1.0f;
            var spacesInTab = 4;

            var xOff = 0f;
            var yOff = 0f;

            foreach (var c in s)
            {
                var gl = _ttf.Glyfs[(byte)c];
                var mesh = _GlyfCache[gl];
                //we need to scale things down
                var maxWidth = _ttf.Header.Xmax + 0.0000000001f;
                var maxHeight = _ttf.Header.Ymax + 0.0000000001f;

                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n')
                    {
                        yOff += (size - (size * (_ttf.HorizontalHeaderTable.lineGap / maxHeight)) * lineHeight);
                        xOff = 0;
                    }
                    else if (c == '\t')
                    {
                        xOff += size * spacesInTab;
                    }
                    else
                    {
                        xOff += size;
                    }

                    continue;
                }

                xOff += size * (_ttf.longHorMetrics[0].advanceWidth / maxWidth);
            }

            return new Size(xOff, yOff);
        }

        public void DrawString(Color4 color, string s, float size, Point p)
        {
            Point currOffset = p;

            foreach (var c in s)
            {
                currOffset = DrawChar(color, c, size, currOffset);
            }
        }

        public Point DrawChar(Color4 color, char c, float size, Point p)
        {
            Point newOffset = new Point(p.X, p.Y);

            var gl = _ttf.Glyfs[(byte)c];
            var mesh = _GlyfCache[gl];

            // Need to scale down
            var maxWidth = _ttf.Header.Xmax + 0.0000000001f;
            var maxHeight = _ttf.Header.Ymax + 0.0000000001f;

            if (char.IsWhiteSpace(c))
            {
                if (c == '\n')
                {
                    return new Point(0, p.Y + (size - (size * (_ttf.HorizontalHeaderTable.lineGap / maxHeight)) * LINE_HEIGHT));
                }
                else if (c == '\t')
                {
                    return new Point(p.X + (size * SPACES_IN_TAB), p.Y);
                }

                // Render as a space
                return new Point(p.X + size, p.Y);
            }

            var scaleFactorX = 1f / maxWidth;
            var scaleFactorY = 1f / maxHeight;

            var scaleX = size * scaleFactorX;
            var scaleY = size * scaleFactorY;

            var trans = new Matrix4f().InitIdentity().InitTranslation(p.X, p.Y + size, 0);
            var scale = new Matrix4f().InitIdentity().InitScale(scaleX, -scaleY, 0);

            _rectShader.Apply();
            _rectShader.SetUniform("mvp", _orthMat * trans * scale);
            _rectShader.SetUniform("uColor", color);

            mesh.Draw();

            return new Point(p.X + (size * (_ttf.longHorMetrics[0].advanceWidth / maxWidth)), p.Y);
        }
    }
}
