using ProjWizInc.Core.Data.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Data.Entries {
    public class ResourceEntry : IEntryInterface {
        public string ResourceKey { get; set; }
        public BigNum Amount { get; set; }
        //we ignore resourceIDs because it is generated on bootup and not important
        [JsonIgnore]
        public int ResourceID { get; set; }

    }
}
