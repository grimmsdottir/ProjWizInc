using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions {
    public class ResourceDefinition : DefinitionBase{
        public ResourceDefinition() {
        }
    }
    public class JobDefinition : DefinitionBase {
        public List<FeatureInterface> Features { get; set; } = new();

        // Helper method to quickly find a specific component
        public T GetComponent<T>() where T : FeatureInterface {
            return Features.OfType<T>().FirstOrDefault();
        }
    }
}
