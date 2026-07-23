using ProjWizInc.Core.Data.ADT;
using ProjWizInc.Core.Data.Blueprints.Roots;

namespace ProjWizInc.Core.Simulation.Registries {

    public class BlueprintRegistry {
        private readonly Dictionary<Type, IDualKeyMap> _typeMaps;
        //instead of the old double dicts, we know use a single dict which is just type and IDualKey
        public BlueprintRegistry(Dictionary<Type, IDualKeyMap> typeMaps) {
            if (typeMaps == null) {
                throw new ArgumentNullException("typeMaps", "[DefinitionManager] Registry cannot be null.");
            }
            _typeMaps = typeMaps;
        }
        public Dictionary<Type, IDualKeyMap>.ValueCollection GetAllMaps() {
            return _typeMaps.Values;
        }

        // Primarily used by the managers on the hot path
        public T GetDefinition<T>(int id) where T : BlueprintBase, new() {
            IDualKeyMap rawMap;
            if (_typeMaps.TryGetValue(typeof(T), out rawMap)) {
                DualKeyMap<T> typedMap = (DualKeyMap<T>)rawMap;
                return typedMap.GetValue(id);
            }
            throw new ArgumentException("Attempted to get definition of " + typeof(T).Name + ", which does not exist");
        }
        public DualKeyMap<T> GetMap<T>() where T : BlueprintBase, new() {
            IDualKeyMap rawMap;
            if (!_typeMaps.TryGetValue(typeof(T), out rawMap)) {
                return (DualKeyMap<T>)rawMap;
            }
            throw new ArgumentException("Map of type " + typeof(T).Name + " does not exist.");
        }

        // Primarily used by definitions for resolving dynamic links
        public int GetID<T>(string key) where T : BlueprintBase, new() {
            IDualKeyMap rawMap;
            if (_typeMaps.TryGetValue(typeof(T), out rawMap)) {
                DualKeyMap<T> typedMap = (DualKeyMap<T>)rawMap;
                return typedMap.GetId(key);
            }
            throw new ArgumentException("Attempted to get ID for definition of " + typeof(T).Name + ", which does not exist");
        }

        public int GetDefCount<T>() where T : BlueprintBase {
            IDualKeyMap rawMap;
            if (_typeMaps.TryGetValue(typeof(T), out rawMap)) {
                return rawMap.Length;
            }
            throw new InvalidOperationException(
                "[DefinitionManager] Type " + typeof(T).Name + " has not been registered yet. " +
                "Make sure definitions are loaded before calling this.");
        }
    }
}