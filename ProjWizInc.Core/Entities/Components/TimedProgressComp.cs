using ProjWizInc.Core.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Entities.Components {
    internal class TimedProgressComp {
        private int _ticks = 0;
        private int _ticksRequired;
        public TimedProgressComp(int TicksRequired) {
            _ticksRequired = TicksRequired;
            ContextManager.Instance.Subscribe();
        }
    }
}
