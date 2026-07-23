using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Configs {
    public static class GlobalConfigs {
        public static readonly string DEFS_FOLDER = "defs";
        public static readonly string SAVE_FOLDER = "save.json";
        public const int TARGET_TPS = 50;
        // 
        public const double TICK_LENGTH = 1.0 / TARGET_TPS;
        // how much lag time are we allowed to process.
        public const double MAX_BURST = 2;
        // our PPS can be pretty low actually, as long as the length is short enough that people cant notice lag
        public const int TARGET_PPS = 30;
        public const double PRESENTATION_LENGTH = 1.0 / TARGET_PPS;

    }
}
