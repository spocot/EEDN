using OpenTK.Windowing.GraphicsLibraryFramework;

namespace EEDN.Editor
{
    public record KeyActionState
    {
        public bool IsShiftActive { get; init; } = false;
        public bool IsAltActive { get; init; } = false;
        public bool IsCommandActive { get; init; } = false;
        public bool IsControlActive { get; init; } = false;
        public EditorMode Mode { get; init; }
        public Keys Key { get; init; }

        public KeyActionState(Keys key, EditorMode mode) => (Key, Mode) = (key, mode);
    }
}
