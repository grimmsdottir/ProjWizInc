using ProjWizInc.Core.States.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States {
    /* 
     * Despite how important GameState will be, it is ultimately just a box of boxes
     */
    public class RecordRoot {
        public EconomyRecord economyState { get; set; } = new();
        public JobRecord jobState { get; set; } = new();
        public TimeRecord timeState { get; set; } = new();
    }
}
