using ProjWizInc.Engine.Data.Definitions.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Definitions {
    public class DefinitionCatalog {
        public List<ResourceDef> resourceDefs { get; set; }
        public List<JobDef> jobDefs { get; set; }
    }
}
