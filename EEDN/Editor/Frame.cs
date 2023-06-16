using EEDN.Render;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEDN.Editor
{
    public class Frame
    {
        public int X {  get; set; }
        public int Y {  get; set; }
        public Size FrameSize { get; set; }
        public TextBuffer Buffer { get; set; }
        public EditorMode CurrentMode { get; set; } = EditorMode.Insert;

        public Frame(int x, int y, Size size, TextBuffer textBuffer)
            => (X, Y, FrameSize, Buffer) = (x, y, size, textBuffer);
    }
}
