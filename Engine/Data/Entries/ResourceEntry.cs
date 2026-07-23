using ProjWizInc.Engine.Data.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Entries {
    public class ResourceEntry {
        [JsonIgnore]
        public int Id {  get; set; }
        public string Key { get; set; }
        public BigNum Amount { get; set; }
    }
}
