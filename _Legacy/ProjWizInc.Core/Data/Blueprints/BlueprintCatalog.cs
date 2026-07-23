using ProjWizInc.Core.Data.Blueprints.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Data.Blueprints {
    public class BlueprintCatalog {
        //our single source of truth for definitions, if we add any new definitions, it has to come here
        public List<ResourceBlueprint> Resources { get; set; } = [];
        public List<JobBlueprint> Jobs { get; set; } = [];
    }
}
