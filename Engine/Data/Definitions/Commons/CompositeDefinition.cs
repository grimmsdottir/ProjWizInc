using ProjWizInc.Engine.Data.Definitions.Specifications.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Data.Definitions.Commons {
    /*  
     *  Represents our more complex static data, that adopts an ECS style Entity(Blueprints) with attachable Components(Specifications)
     *  Blueprints and Specs where chosen for the names to avoid clashing with Entity and Components later, as we decided
     *  that Blueprints and Specs will be static, while Entities and Components will be volatile
     */
    public class CompositeDefinition : DefinitionBase {
        public CompositeDefinition() : base() { }
        public CompositeDefinition(string key) : base(key) { }

        public List<ISpecification> Specifications { get; set; } = new List<ISpecification>();
        public T GetSpecification<T>() where T : ISpecification {
            for (int i = 0; i < Specifications.Count; i++) {
                if (Specifications[i] is T) {
                    return (T)Specifications[i];
                }
            }
            throw new KeyNotFoundException("Critial Boot Failure: Could not find component of type " +typeof(T).Name);
        }

    }
}
