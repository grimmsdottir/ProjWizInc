using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Common {
    public class DefinitionEntity : DefinitionBase{
        //holds our components
        public List<IDefinitionComponentInterface> Components { get; set; } = [];
        // Helper method to quickly find a specific component
        public T GetComponent<T>() where T : IDefinitionComponentInterface {
            return Components.OfType<T>().FirstOrDefault();
        }
    }
}
