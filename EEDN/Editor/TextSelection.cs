using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEDN.Editor
{
    public record TextSelection
    {
        public TextLocation StartLoc { get; init; }
        public TextLocation EndLoc { get; init; }

        public TextSelection(TextLocation startLoc, TextLocation endLoc)
            => (StartLoc, EndLoc) = (startLoc, endLoc);

        public static implicit operator TextSelection((TextLocation, TextLocation) locTuple)
            => new TextSelection(locTuple.Item1, locTuple.Item2);
    }
}
