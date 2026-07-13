using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Features {
    
    public class RequiresTicksFeature : FeatureInterface {
        public BigNum RequiredTicks { get; set; } = 1000;
    }
}
