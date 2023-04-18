using EEDN.TTF;

namespace EEDN.Render
{
    public class GlyphCache
    {
        private Dictionary<Glyph, Mesh> _cache = new Dictionary<Glyph, Mesh>();

        public Mesh this[Glyph glyf]
        {
            get
            {
                if (_cache.ContainsKey(glyf)) return _cache[glyf];

                // Load it not already cached
                var mesh = new Mesh();
                var verts = new List<Point>();

                foreach (var triangle in glyf.Triangles)
                {
                    verts.Add((triangle.A.X, triangle.A.Y));
                    verts.Add((triangle.B.X, triangle.B.Y));
                    verts.Add((triangle.C.X, triangle.C.Y));
                }

                mesh.Load(verts.ToArray());
                _cache.Add(glyf, mesh);

                return mesh;
            }
        }
    }
}
