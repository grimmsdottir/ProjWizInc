using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Data {
    public static class Globals {
        public const int TICKS_PER_SECOND = 100;
        public const int FRAMES_PER_SECOND = 60;

        public static readonly double TICK_LENGTH = 1.0 / TICKS_PER_SECOND;
        public static readonly double FRAME_LENGTH_MS = 1000.0 / FRAMES_PER_SECOND;
    }
    

}
