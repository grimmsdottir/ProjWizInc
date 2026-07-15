using Microsoft.Win32;
using ProjWizInc.Core.Definitions.Blueprints;
using ProjWizInc.Core.Definitions.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static ProjWizInc.Core.Definitions.GameDefinitions;

namespace ProjWizInc.Core.Definitions {

    
    public class DefinitionManager {
        //now we use Type for keys. this first dictionary is a dictionary of dictionaries
        //each type has its own dictionary, which maps strings to int
        private readonly Dictionary<Type, Dictionary<string, int>> _typeKeyIdMap = [];
        //for each Type, it will have its own array, we use a generic Array, because we dont know the typing yet,
        //but it should basically be like <Type,Type[]>
        //the int ids from the first dictionary map to the position in the defStore, for high performance
        private readonly Dictionary<Type, Array> _typeIdDefMap = [];
        internal DefinitionManager(
            Dictionary<Type, Dictionary<string, int>> typeKeyIdMap,
            Dictionary<Type, Array> typeIdDefMap
            ) {
            _typeKeyIdMap = typeKeyIdMap;
            _typeIdDefMap = typeIdDefMap;
        }
        /*
        public void Init() {
            //same as the GameDefinitions, we gotta do this one manually
            RegisterDefinition(data.Resources);
            RegisterDefinition(data.Jobs);
            //we only deal with the links once everything is done, cant do plumbing of an in construction house they say
            ResolveAllLinks();

        }
        //this function accepts a list of whatever, in this case resources and jobs for now
        private void RegisterDefinition<T>(List<T> list) where T : DefinitionBase {
            //string to int map for the type
            var keyIdMap = new Dictionary<string, int>();
            //array that holds all the defs
            var idDefMap = new T[list.Count];
            for (int i = 0; i < list.Count; i++) {
                var def = list[i];
                def.Id = i;
                keyIdMap[def.Key] = i;
                idDefMap[i] = def;
            }
            _typeKeyIdMap[typeof(T)] = keyIdMap;
            _typeIdDefMap[typeof(T)] = idDefMap;
        }
        private void ResolveAllLinks() {
            foreach (var idDefMap in _typeIdDefMap.Values) {
                foreach (var def in idDefMap) {
                    if (def is DefinitionBase entity) {
                        foreach (var feature in entity.Components.OfType<ILinkableDefinitionInterface>()) {
                            feature.ResolveLinks(this);
                        }
                    }
                }
            }
        }
        */
        //primarily used by the managers
        public T GetDefinition<T>(int id) where T:DefinitionBase {
            if (_typeIdDefMap.TryGetValue(typeof(T), out var array)) {
                var typedArray = (T[])array;
                return typedArray[id];
            }
            throw new ArgumentException("Attempted to get definition of "+typeof(T)+", which does not exist");
        }
        //primarily used by defs for linking
        public int GetID<T>(string key) where T:DefinitionBase{
            return _typeKeyIdMap[typeof (T)][key];
        }
        public int GetDefCount<T>() where T : DefinitionBase {
            if (!_typeIdDefMap.ContainsKey(typeof(T))) {
                throw new InvalidOperationException(
                    $"[DefinitionManager] Type {typeof(T).Name} has not been registered yet. " +
                    "Make sure definitions are loaded before calling this.");
            }
            return _typeIdDefMap[typeof(T)].Length;
        }
    }
}
