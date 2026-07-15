using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Common {
    public interface LinkableDefinitionInterface {
        void ResolveLinks(DefinitionManager manager);
    }
}
