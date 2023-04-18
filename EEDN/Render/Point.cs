using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEDN.Render
{
    public record Point
    {
        public readonly float X;
        public readonly float Y;

        public Point(float x, float y) => (X, Y) = (x, y);

        public static implicit operator Point((float, float) point)
            => new Point(point.Item1, point.Item2);
    }
}
