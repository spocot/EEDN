using EEDN.Editor;
using EEDN.Render;
using EEDN.Themes;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace EEDN
{
    public class EednEngine
    {
        public List<string> Affixes { get; set; } = new List<string>();
        public int CurrentFrameIdx { get; set; } = -1;
        public List<Frame> Frames { get; set; } = new List<Frame>();
        public Frame CurrentFrame
        {
            get => Frames[CurrentFrameIdx];
        }
        public TextBuffer CurrentBuffer
        {
            get => CurrentFrame.Buffer;
        }
        public Dictionary<KeyActionState, MotionAction> MotionBindings { get; set; } = new Dictionary<KeyActionState, MotionAction>();
        public Dictionary<KeyActionState, CommandAction> CommandBindings { get; set; } = new Dictionary<KeyActionState, CommandAction>();
        public Dictionary<KeyActionState, StandaloneAction> StandaloneBindings { get; set; } = new Dictionary<KeyActionState, StandaloneAction>();
        public EditorMode CurrentMode
        {
            get => CurrentFrame.CurrentMode;
            set => CurrentFrame.CurrentMode = value;
        }
        public Theme CurrentTheme { get; set; }

        private DrawBuffer _DrawBuffer;
        private Color4 _LineCursorColor;
        private Color4 _BlockCursorColor;
        private Color4 _SelectionColor;

        public TextSelection? CurrentSelection
        {
            get => CurrentFrame.CurrentSelection;
            set => CurrentFrame.CurrentSelection = value;
        }

        public KeyActions Actions { get; init; }

        public EednEngine(DrawBuffer drawBuffer, Theme theme)
        {
            _DrawBuffer = drawBuffer;
            Frames.Add(new Frame(new Point(0, 0), new Point(_DrawBuffer.ScreenSize.Width, _DrawBuffer.ScreenSize.Height), new TextBuffer()));
            CurrentFrameIdx = 0;
            CurrentTheme = theme;

            _LineCursorColor = new Color4(CurrentTheme.CursorColor.R, CurrentTheme.CursorColor.G, CurrentTheme.CursorColor.B, 1.0f);
            _BlockCursorColor = new Color4(CurrentTheme.CursorColor.R, CurrentTheme.CursorColor.G, CurrentTheme.CursorColor.B, 0.5f);
            _SelectionColor = new Color4(CurrentTheme.SelectionColor.R, CurrentTheme.SelectionColor.G, CurrentTheme.SelectionColor.B, 0.5f);

            Actions = new KeyActions(this);
        }

        public void ProcessKey(KeyboardKeyEventArgs e)
            => Actions.ProcessKey(e);

        public void Draw()
        {
            foreach (Frame frame in Frames)
            {
                float x = frame.StartPoint.X;
                float y = frame.StartPoint.Y;

                TextBuffer tBuffer = frame.Buffer;
                for (var i = 0; i < tBuffer.Lines.Count; i++)
                {
                    // Draw buffer text
                    var line = tBuffer.Lines[i];
                    _DrawBuffer.DrawString(CurrentTheme.FgColor, line, tBuffer.FontSize, (x, y));
                    y += tBuffer.FontSize + 8;

                    // Draw cursor
                    if (i == tBuffer.LineIdx && DateTime.Now.Millisecond >= 500)
                    {
                        var size = _DrawBuffer.MeasureString(tBuffer.ColIdx != line.Length ? line.Remove(tBuffer.ColIdx) : line, tBuffer.FontSize);
                        _DrawBuffer.DrawRect(
                            CurrentMode switch
                            {
                                EditorMode.Insert => _LineCursorColor,
                                _ => _BlockCursorColor
                            },
                            new Rect(
                                new Point(x + size.Width, y - tBuffer.FontSize - 2),
                                new Size((CurrentMode switch
                                {
                                    EditorMode.Insert => 2,
                                    _ => tBuffer.FontSize - 4
                                }), tBuffer.FontSize)));
                    }
                }

                // Highlight current selection
                if (CurrentSelection is not null)
                {
                    int startLineIdx = CurrentSelection.StartLoc.LineIdx;
                    int endLineIdx = CurrentSelection.EndLoc.LineIdx;

                    int startColX = CurrentSelection.StartLoc.ColIdx;
                    int endColX = CurrentSelection.EndLoc.ColIdx;

                    if (endLineIdx < startLineIdx)
                    {
                        int temp = startLineIdx;
                        startLineIdx = endLineIdx;
                        endLineIdx = temp;

                        temp = startColX;
                        startColX = endColX;
                        endColX = temp;
                    }
                    else if (startLineIdx == endLineIdx && endColX < startColX)
                    {
                        int temp = startColX;
                        startColX = endColX;
                        endColX = temp;
                    }

                    for (int i = startLineIdx; i <= endLineIdx; ++i)
                    {
                        float startX, endX;

                        if (i == startLineIdx)
                        {
                            startX = _DrawBuffer.MeasureString(tBuffer.Lines[i].Remove(startColX), tBuffer.FontSize).Width;
                        }
                        else
                        {
                            startX = 0;
                        }

                        if (i == endLineIdx)
                        {
                            endX = _DrawBuffer.MeasureString(tBuffer.Lines[i].Remove(endColX), tBuffer.FontSize).Width;
                        }
                        else
                        {
                            endX = _DrawBuffer.MeasureString(tBuffer.Lines[i], tBuffer.FontSize).Width;
                        }

                        if (endX < startX)
                        {
                            float temp = startX;
                            startX = endX;
                            endX = temp;
                        }

                        float dX = x + startX;
                        float dY = (i * (tBuffer.FontSize + 8)) + 6;

                        float width = endX + tBuffer.FontSize - startX;
                        float height = tBuffer.FontSize;

                        //Console.WriteLine($"Width: [{width}] Height: [{height}] [{endX}][{tBuffer.FontSize}][{startX}]");

                        _DrawBuffer.DrawRect(
                            _SelectionColor,
                            new Rect(
                                new Point(dX, dY),
                                new Size(width, height)));
                    }
                }

                // Draw current mode indicator
                _DrawBuffer.DrawString(CurrentTheme.CursorColor, CurrentMode.ToString(), 16, new Point(0, _DrawBuffer.ScreenSize.Height - 20));
            }
        }
    }
}
