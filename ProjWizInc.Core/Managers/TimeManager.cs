using ProjWizInc.Core.Data;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    public class TimeState {
        //we will use minutes/hours/days to store time, probably its own ADT
        public long TimeElapsed;
        public int TicksElapsed;
    }
    public class TimeManager {
        private readonly EventManager _events;
        private readonly TimeState _state = new();
        public TimeState State => _state;

        public TimeManager(EventManager events) {
            _events = events;
            _events.Subscribe<UpdateLogicEvent>(Update);
        }
        public void Init() {
        }
        public long GetTime() {
            return _state.TimeElapsed;
        }
        //we have to use a small paremeter here, even if we dont need anything, because of signalling reasons
        //but if we change UpdateFrameEvent with parameters later, we can check it with e
        public void Update(UpdateLogicEvent e) {
            AdvanceTick();
        }
        private void AdvanceTick() {
            _state.TicksElapsed++;
            if (_state.TicksElapsed >= Globals.TICKS_PER_SECOND) {
                _state.TicksElapsed -= Globals.TICKS_PER_SECOND;
                _state.TimeElapsed++;
            }
        }
    }
}
