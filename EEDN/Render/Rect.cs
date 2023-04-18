using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEDN.Render
{
    public class Rect
    {
        public readonly Point Location;
        public readonly Size Size;

        public Rect(Point location, Size size) => (Location, Size) = (location, size);
    }
}
