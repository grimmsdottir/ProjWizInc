using ProjWizInc.Engine.Data.ADT;
using ProjWizInc.Engine.Data.Definitions;

namespace ProjWizInc.Engine.Simulation.Registries {
    public class DefinitionRegistry{
        private readonly Dictionary<Type, object> _registries = new Dictionary<Type, object>();
        //this adds a new deftype into the registry, but actual population is done by the bootstrapper
        public void RegisterMap<T>(DualKeyMap<T> map) where T : class, new() {
            Type typeKey = typeof(T);
            if (_registries.ContainsKey(typeKey)) {
                throw new InvalidOperationException("Registry Error: A map for type " + typeKey.Name + " has already been registered.");
            }
            _registries.Add(typeKey, map);
        }
        public Dictionary<Type, object> Registries { get { return _registries; } }
        public DualKeyMap<T> GetMap<T>() where T : class, new() {
            Type typeKey = typeof(T);
            if (!_registries.TryGetValue(typeKey, out object map)) {
                throw new KeyNotFoundException("Registry Error: No map found for type " + typeKey.Name);
            }
            return (DualKeyMap<T>)map;
        }
    }
}
