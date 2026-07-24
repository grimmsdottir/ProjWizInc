using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Definitions.Commons {
    /* Represents the base of all our static data that have been parsed from disk to memory
     * Contains basic metadata for easier identification
     */
    public class DefinitionBase {
        public string Key { get; set; } = string.Empty;
        [JsonIgnore]
        public int Id { get; set; } = -1;
        public DefinitionBase() {
        }
        public DefinitionBase(string key) {
            Key = key;
        }
    }
}
