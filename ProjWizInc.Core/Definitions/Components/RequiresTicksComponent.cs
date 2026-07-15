using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Components {
    
    public class RequiresTicksComponent : IDefinitionComponentInterface {
        public BigNum RequiredTicks { get; set; } = 1000;
    }
}
