using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEDN.Editor
{
    public enum EditorMode
    {
        Command,
        Insert,
        Visual
    }

    public delegate int ModeEnterAction();
    public delegate int ModeKeyPressAction(Keys key);

    public class Mode
    {
        public ModeEnterAction OnModeEnter { get; init; }
        public ModeKeyPressAction OnModeKeyPress { get; init; }

        public Mode(ModeEnterAction onModeEnter, ModeKeyPressAction onModeKeyPress)
            => (OnModeEnter, OnModeKeyPress) = (onModeEnter, onModeKeyPress);
    }
}
