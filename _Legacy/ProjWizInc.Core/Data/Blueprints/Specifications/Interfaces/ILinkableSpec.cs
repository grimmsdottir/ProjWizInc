using ProjWizInc.Core.Simulation.Registries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Data.Blueprints.Specifications.Interfaces {
    public interface ILinkableSpec {
        void ResolveLinks(BlueprintRegistry manager);
    }
}
