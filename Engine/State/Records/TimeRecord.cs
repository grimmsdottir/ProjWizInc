using ProjWizInc.Engine.State.Records.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.State.Records {
    public class TimeRecord : IReadTime, IWriteTime {
        private long _totalTicks;
        public long GetTime() {
            return _totalTicks;
        }
        public void AdvanceTime() {
            _totalTicks++;
        }
    }
}
