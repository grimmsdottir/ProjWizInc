using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Data;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Definitions.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States.Managers {
    public class JobState {
        public int ActiveJobId { get; set; } = -1;
        //we keep a cache of the active job's def, so we only need to search it up whenver we change jobs
        public JobDefinition? ActiveJobDef { get; set; } = null;
        //we also keep a cache of any components and their state vars like ticks and xp
        public RequiresTicksComponent? JobTicksRequired { get; set; } = null;
        public BigNum Ticks { get; set; } = 0;
        public PayoutComponent? JobPayout { get; set; } = null;
        //a handy reset function for nulling everything
        public void Reset() {
            ActiveJobId = -1;
            ActiveJobDef = null;
            JobTicksRequired = null;
            Ticks = 0;
            JobPayout = null;
        }
    }
}
