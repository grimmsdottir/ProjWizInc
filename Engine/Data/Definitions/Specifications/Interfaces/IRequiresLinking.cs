using ProjWizInc.Engine.Simulation.Registries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces {
    public interface IRequiresLinking {
        void ResolveLinks(DefinitionRegistry registry);
    }
}
