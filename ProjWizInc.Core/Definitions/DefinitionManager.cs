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
        //this one is like a get all dicts, but as an IEnumarable, which is basically like a copy/immutable
        //collection. Primary use is for linking right now
        public IEnumerable<Array> GetAllDefinitions() {
            return _typeIdDefMap.Values;
        }
        //primarily used by the managers
        public T GetDefinition<T>(int id) where T:DefinitionBase {
            Array rawArray;
            if (_typeIdDefMap.TryGetValue(typeof(T), out rawArray)) {
                T[] typedArray = (T[])rawArray;
                return typedArray[id];
            }
            throw new ArgumentException("Attempted to get definition of " + typeof(T).Name + ", which does not exist");
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
