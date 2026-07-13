using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Common {
    internal class DefinitionBase {
        public string Key { get; internal set; } = string.Empty;
        public string DisplayName { get; internal set; } = string.Empty;
        public int Id { get; internal set; } = -1;
        //holds our features
        public List<FeatureInterface> Features { get; internal set; } = [];
        // Helper method to quickly find a specific component
        public T GetFeature<T>() where T : FeatureInterface {
            return Features.OfType<T>().FirstOrDefault();
        }
    }
}
