using OpenTK.Graphics.OpenGL;

namespace EEDN.Render
{
    public class Mesh
    {
        private readonly int vbo;
        private readonly int ibo;
        private readonly int ab;

        public int Size { get; set; }

        public Mesh()
        {
            vbo = GL.GenBuffer();
            ibo = GL.GenBuffer();
            ab = GL.GenVertexArray();
        }

        public void Load(params Point[] vertices)
        {
            var vertexData = new List<byte>();

            foreach (var d in vertices)
            {
                vertexData.AddRange(BitConverter.GetBytes(d.X));
                vertexData.AddRange(BitConverter.GetBytes(d.Y));
            }

            var data = vertexData.ToArray();

            Size = vertices.Count();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length, data, BufferUsageHint.StaticDraw);

            var buf = new List<byte>();
            for (int i = 0; i < Size; ++i)
            {
                buf.AddRange(BitConverter.GetBytes(i));
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, buf.Count, buf.ToArray(),
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ibo);
        }

        public void Draw()
        {
            GL.BindVertexArray(ab);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * 4, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

            GL.DrawElements(BeginMode.Triangles, Size, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
