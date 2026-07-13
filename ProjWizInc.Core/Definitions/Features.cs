using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions {
    public class PayoutFeature : FeatureInterface {
        public List<ResourcePayoutEntry> PayoutEntries { get; set; } = [];
    }
    public class RequiresTicksFeature : FeatureInterface {
        public long RequiredTicks { get; set; } = 1000;
    }
}
