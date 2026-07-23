using ProjWizInc.Core.Data.ADT;
using ProjWizInc.Core.Data.Blueprints.Specifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Data.Blueprints.Specifications {
    
    public class RequiresTicksSpec : ISpecification {
        public BigNum RequiredTicks { get; set; } = 1000;
    }
}
