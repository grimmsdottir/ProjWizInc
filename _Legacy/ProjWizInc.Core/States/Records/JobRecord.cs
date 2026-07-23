using ProjWizInc.Core.Data;
using ProjWizInc.Core.Data.ADT;
using ProjWizInc.Core.Data.Blueprints.Blueprints;
using ProjWizInc.Core.Data.Blueprints.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.States.Records {
    public class JobRecord {
        public int ActiveJobId { get; set; } = -1;
        //we keep a cache of the active job's def, so we only need to search it up whenver we change jobs
        public JobBlueprint? ActiveJobDef { get; set; } = null;
        //we also keep a cache of any components and their state vars like ticks and xp
        public RequiresTicksSpec? JobTicksRequired { get; set; } = null;
        public BigNum Ticks { get; set; } = 0;
        public PayoutSpec? JobPayout { get; set; } = null;
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
