using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Common {
    public class DefinitionBase {
        public string Key { get; protected set; } = string.Empty;
        public string DisplayName { get; protected set; } = string.Empty;
        public int Id { get; protected set; } = -1;
    }
}
