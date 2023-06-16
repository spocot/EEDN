using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEDN.Editor
{
    // TODO: Switch current state management over to this.
    public class ModeManager
    {
        private Dictionary<string, Mode> _StateMap;
        private Mode _CurrentState;

        private Mode CurrentState
        {
            get => _CurrentState;
            set
            {
                _CurrentState = value;
                _CurrentState.OnModeEnter();
            }
        }

        public ModeManager(Dictionary<string, Mode> stateMap, Mode initialState)
            => (_StateMap, _CurrentState) = (stateMap, initialState);
    }
}
