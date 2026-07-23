using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Common {
    public class DefinitionBase {
        public string Key { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Id { get; set; } = -1;
        public DefinitionBase() {
        }
        public DefinitionBase(string key, int id) {
            Key = key;
            Id = id;
        }
    }
}
