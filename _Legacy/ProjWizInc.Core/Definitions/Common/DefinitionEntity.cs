namespace ProjWizInc.Core.Definitions.Common {
    public class DefinitionEntity : DefinitionBase{
        //holds our components
        public List<IDefinitionComponentInterface> Components { get; set; } = [];
        // Helper method to quickly find a specific component
        public T GetComponent<T>() where T : IDefinitionComponentInterface {
            int count = Components.Count;
            for (int i = 0; i < count; i = i + 1) {
                IDefinitionComponentInterface component = Components[i];
                if (component is T) {
                    return (T)component;
                }
            }
            throw new KeyNotFoundException("Critical Error: Required component of type " + typeof(T).Name + " could not be found.");
        }
    }
}
