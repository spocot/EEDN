using OpenTK.Mathematics;

namespace EEDN.Themes
{
    public class Color4Helper
    {
        public Color4 ConvertedColor { get; init; }

        public static implicit operator Color4Helper(uint hexInt)
        {
            var bytes = BitConverter.GetBytes(hexInt);
            return new Color4Helper
            {
                ConvertedColor = new Color4(bytes[0], bytes[1], bytes[2], bytes[3])
            };
        }

        public static implicit operator Color4(Color4Helper colorHelper)
            => colorHelper.ConvertedColor;
    }
}
