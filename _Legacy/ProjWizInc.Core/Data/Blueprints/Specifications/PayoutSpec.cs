using ProjWizInc.Core.Data.ADT;
using ProjWizInc.Core.Data.Blueprints.Blueprints;
using ProjWizInc.Core.Data.Blueprints.Specifications.Interfaces;
using ProjWizInc.Core.Data.Entries;
using ProjWizInc.Core.Data.Events;
using ProjWizInc.Core.Simulation.Registries;
using System.Resources;
using System.Text.Json.Serialization;

namespace ProjWizInc.Core.Data.Blueprints.Specifications {
    
    public class PayoutSpec : ISpecification, ILinkableSpec {
        public List<ResourceEntry> PayoutEntries { get; internal set; } = [];
        public void ResolveLinks(BlueprintRegistry manager) {
            foreach (ResourceEntry entry in PayoutEntries) {
                entry.ResourceID = manager.GetID<ResourceBlueprint>(entry.ResourceKey);
            }   
        }
    }
}
