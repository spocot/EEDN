using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEDN.Editor
{
    public struct TextLocation
    {
        public int LineIdx { get; set; }
        public int ColIdx { get; set; }

        public TextLocation(int lineIdx, int colIdx)
            => (LineIdx, ColIdx) = (lineIdx, colIdx);

        public static implicit operator TextLocation((int, int) idxTuple)
            => new TextLocation(idxTuple.Item1, idxTuple.Item2);
    }
}
