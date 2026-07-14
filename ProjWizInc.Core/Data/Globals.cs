using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Data {
    public static class Globals {
        public const int LOGIC_TPS = 100;
        public const int PRESENTATION_PPS = 30;

        public static readonly double TICK_LENGTH = 1.0 / LOGIC_TPS;
        public static readonly double FRAME_LENGTH_MS = 1000.0 / PRESENTATION_PPS;

        
    }
    public enum ResourceType {
        Gold,
        Wood,
        Stone
    }

}
