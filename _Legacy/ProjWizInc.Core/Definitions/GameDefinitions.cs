using ProjWizInc.Core.Definitions.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions {
    public class GameDefinitions {
        //our single source of truth for definitions, if we add any new definitions, it has to come here
        public List<ResourceDefinition> Resources { get; set; } = [];
        public List<JobDefinition> Jobs { get; set; } = [];
    }
}
