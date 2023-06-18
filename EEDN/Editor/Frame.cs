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
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public TextBuffer Buffer { get; set; }
        public EditorMode CurrentMode { get; set; } = EditorMode.Insert;
        public TextSelection? CurrentSelection { get; set; } = null;

        public Frame(Point startPoint, Point endPoint, TextBuffer textBuffer)
            => (StartPoint, EndPoint, Buffer) = (startPoint, endPoint, textBuffer);
    }
}
