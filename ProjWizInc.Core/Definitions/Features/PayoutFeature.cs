using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Features {
    internal class ResourcePayoutEntry : EntryInterface {
        public string ResourceID { get; internal set; }
        public BigNum Ammount { get; internal set; }
    }
    internal class PayoutFeature : FeatureInterface, LinkableDefinitionInterface {
        public List<ResourcePayoutEntry> PayoutEntries { get; set; } = [];
        public void ResolveLinks(DefinitionManager manager) {
            foreach (var entry in PayoutEntries) {
                //entry.ResourceID = manager;
            }
        }
    }
}
