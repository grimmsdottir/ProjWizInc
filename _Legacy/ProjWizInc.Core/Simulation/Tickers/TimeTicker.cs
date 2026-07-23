using ProjWizInc.Core.Data;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjWizInc.Core.Data.Events;
using ProjWizInc.Core.States.Records;
using ProjWizInc.Core.Simulation.Events;

namespace ProjWizInc.Core.Simulation.Tickers {
    
    public class TimeTicker {
        private readonly EventHub _events;
        private readonly TimeRecord _state;
        public TimeRecord State => _state;

        public TimeTicker(EventHub events, TimeRecord timeState) {
            _events = events;
            _state = timeState;
            _events.Subscribe<UpdateLogicEvent>(Update);
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
            if (_state.TicksElapsed >= GlobalConsts.LOGIC_TPS) {
                _state.TicksElapsed -= GlobalConsts.LOGIC_TPS;
                _state.TimeElapsed++;
            }
        }
    }
}
