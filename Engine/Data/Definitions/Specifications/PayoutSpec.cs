using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.Data.Definitions.Defs;
using ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces;
using ProjWizInc.Engine.Data.Entries;
using ProjWizInc.Engine.Simulation.Registries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Definitions.Specifications {
    public class PayoutSpec :ISpecification,IRequiresLinking{
        
        public ResourceEntry[] PayoutEntries { get; init; }
        public PayoutSpec(ResourceEntry[] payoutEntries) {
            this.PayoutEntries = payoutEntries;
        }
        public void ResolveLinks(DefinitionRegistry registry) {
            DualKeyMap<ResourceDef> resourceMap = registry.GetMap<ResourceDef>();
            for (int i = 0; i < PayoutEntries.Length; i++) {
                ResourceEntry entry = PayoutEntries[i];
                if (entry == null) {
                    throw new InvalidDataException("Critical Boot Failure: Resource Entry is null");
                }
                if (entry.Key == null) {
                    throw new InvalidDataException("Critical Boot Failure: Resource Entry has null key");
                }
                entry.Id = resourceMap.GetId(entry.Key);
            }
        }
    }
}
