using OpenTK.Mathematics;

namespace EEDN.Render
{
    public class Size
    {
        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public Size(float z)
        {
            Width = z;
            Height = z;
        }

        public float Width { get; set; }
        public float Height { get; set; }

        public static implicit operator Size(Vector2i sizeVector)
            => new Size(sizeVector.X, sizeVector.Y);
    }
}
