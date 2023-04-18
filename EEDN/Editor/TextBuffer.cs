using EEDN.Render;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEDN.Editor
{
    public class TextBuffer
    {
        public int FontSize { get; set; } = 32;
        public int LineIdx { get; set; }
        public int ColIdx { get; set; }
        public List<string> Lines { get; set; } = new List<string>() { "" };

        public void Insert(char c)
        {
            Lines[LineIdx] = Lines[LineIdx].Insert(ColIdx, c.ToString());
            ColIdx++;
        }

        public void Insert(int i) => Insert((char)i);
    }
}
