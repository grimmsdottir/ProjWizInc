using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions.Common;
using ProjWizInc.Core.Managers;
using System.Resources;
using System.Text.Json.Serialization;

namespace ProjWizInc.Core.Definitions.Features {
    internal class ResourcePayoutEntry : EntryInterface {
        public string ResourceKey {  get; set; }
        public BigNum Amount { get; set; }
        //we ignore resourceIDs because it is generated on bootup and not important
        [JsonIgnore]
        public int ResourceID { get; internal set; }
        
    }
    internal class PayoutFeature : FeatureInterface, LinkableDefinitionInterface {
        public List<ResourcePayoutEntry> PayoutEntries { get; set; } = [];
        public void ResolveLinks(DefinitionManager manager) {
            foreach (ResourcePayoutEntry entry in PayoutEntries) {
                entry.ResourceID = manager.GetID<ResourceDefinition>(entry.ResourceKey);
            }   
        }
        public void Payout(EconomyManager manager) {
            foreach (ResourcePayoutEntry entry in PayoutEntries) {
                manager.AddResource(entry.ResourceID,entry.Amount);
            }
        }
    }
}
