using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Definitions.Common;
using ProjWizInc.Core.Definitions.Entries;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.Managers;
using System.Resources;
using System.Text.Json.Serialization;

namespace ProjWizInc.Core.Definitions.Components {
    
    public class PayoutComponent : IDefinitionComponentInterface, ILinkableDefinitionInterface {
        public List<ResourcePayoutEntry> PayoutEntries { get; internal set; } = [];
        public void ResolveLinks(DefinitionManager manager) {
            foreach (ResourcePayoutEntry entry in PayoutEntries) {
                entry.ResourceID = manager.GetID<ResourceDefinition>(entry.ResourceKey);
            }   
        }
    }
}
