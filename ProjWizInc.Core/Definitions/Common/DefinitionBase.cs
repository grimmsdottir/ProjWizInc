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
        //holds our features
        public List<IFeatureInterface> Features { get; set; } = [];
        // Helper method to quickly find a specific component
        public T GetFeature<T>() where T : IFeatureInterface {
            return Features.OfType<T>().FirstOrDefault();
        }
    }
}
