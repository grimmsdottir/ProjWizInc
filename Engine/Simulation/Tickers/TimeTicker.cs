using ProjWizInc.Engine.Simulation.Tickers.Interfaces;
using ProjWizInc.Engine.State.Records.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Simulation.Tickers {
    //realised a bit too late theres no real point in have a class just to tick up time when the heartbeat can do it
    public class TimeTicker : ITimeTicker {
        private IWriteTime _timeWriter;
        public TimeTicker(IWriteTime timeWriter) {
            _timeWriter = timeWriter;
        }
        public void Tick() {
            _timeWriter.AdvanceTime();
        }

    }
}
