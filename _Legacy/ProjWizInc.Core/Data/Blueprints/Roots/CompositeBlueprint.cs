using ProjWizInc.Core.Data.Blueprints.Specifications.Interfaces;

namespace ProjWizInc.Core.Data.Blueprints.Roots {
    public class CompositeBlueprint : BlueprintBase{
        //holds our components
        public List<ISpecification> Components { get; set; } = [];
        // Helper method to quickly find a specific component
        public T GetComponent<T>() where T : ISpecification {
            int count = Components.Count;
            for (int i = 0; i < count; i = i + 1) {
                ISpecification component = Components[i];
                if (component is T) {
                    return (T)component;
                }
            }
            throw new KeyNotFoundException("Critical Error: Required component of type " + typeof(T).Name + " could not be found.");
        }
    }
}
