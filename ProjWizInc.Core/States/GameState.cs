using ProjWizInc.Core.States.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States {
    /* 
     * Despite how important GameState will be, it is ultimately just a box of boxes
     */
    public class GameState {
        public EconomyState economyState { get; set; } = new();
        public JobState jobState { get; set; } = new();
        public TimeState timeState { get; set; } = new();
    }
}
