using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States.Managers {
    public class TimeState {
        //TODO:use minutes/hours/days to store time, maybe its own ADT
        public long TimeElapsed;
        public int TicksElapsed;
    }
}
