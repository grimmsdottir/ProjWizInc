using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions {
    internal class JobDefinition : DefinitionBase {
        //holds our features
        public List<FeatureInterface> Features { get; set; } = [];
        // Helper method to quickly find a specific component
        public T GetFeature<T>() where T : FeatureInterface {
            return Features.OfType<T>().FirstOrDefault();
        }
    }
}
