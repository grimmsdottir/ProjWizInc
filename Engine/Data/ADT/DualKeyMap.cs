using System.Runtime.CompilerServices;

namespace ProjWizInc.Engine.Data.ADT {
    /*
     * DualKeyMap is a data structure that allows you to store values with two keys.
     * It is implemented as a an array of values, as well as 2 additonal maps that translate between the string keys and the integer keys.
     *  
     * This allows for efficient lookups and insertions, as well as easy iteration over all values associated with a given first key.
     * where TValue : new() means that we must have a noarg constructor, for our future proofing default value creation
     */
    public class DualKeyMap<TValue> where TValue : new(){
        //holds our payload in an array for fast access
        private readonly TValue[] _idValueMap;//aka actual payload
        //translates string to keys, for those who dont know the id 
        private readonly Dictionary<string, int> _keyIdMap;//aka reverse map
        //translates keys to string, for hydration and dehydration
        private readonly string[] _idKeyMap;//aka forward map
        private bool _isHydrated;
        //refactored DualKeyMap to accept an array of keys, and to generate the forward and reverse maps
        public DualKeyMap(string[] idKeyMap) {
            //some basic saftey checks first
            //we can skip some of our previous checks, like negative ids, bounded ids and id holes(1,2,4)
            if (idKeyMap == null) {
                throw new ArgumentNullException("keyIdMap", "Critical Boot Failure: The idKey provided to DualKeyMap is null.");
            }
            //generate our keyIdMap, and check for malformed data while we are at it
            _keyIdMap = [];
            for (int i = 0; i < idKeyMap.Length; i++) {
                if (idKeyMap[i] == null) {
                    throw new InvalidDataException("Crtical Boot Failure: The idKey provided to DualKeyMap is malformed. Null data at index "+i);
                }
                _keyIdMap.Add(idKeyMap[i], i);

            }
            _idKeyMap = idKeyMap;
            _idValueMap = new TValue[idKeyMap.Length];
            _isHydrated = false;
        }
        //specialised secondary constructor for building directly from key-value maps, which is most static defs
        //note: it should NOT be used for state data, because reasons
        public DualKeyMap(Dictionary<string,TValue> sourceMap) {
            if (sourceMap == null) {
                throw new ArgumentNullException("sourceMap", "Critical Boot Failure: Source map provided to DualKeyMap is null.");
            }
            _keyIdMap = new Dictionary<string, int>();
            _idKeyMap = new string[sourceMap.Count];
            _idValueMap = new TValue[sourceMap.Count];
            int index = 0;
            foreach(KeyValuePair<string,TValue> kvp in sourceMap) {
                string key = kvp.Key;
                TValue value = kvp.Value;
                if (key == null) {
                    throw new InvalidDataException("Critical Boot Failure: Source map contains a null key.");
                }
                if (value == null) {
                    throw new InvalidDataException("Critical Boot Failure: Value associated with key '" + key + "' is null.");
                }
                _keyIdMap.Add(key, index);
                _idKeyMap[index] = key;
                _idValueMap[index] = value;

                index++;
            }
            _isHydrated = true;
        }
        //Non-Generic Interface Implementation (For BlueprintRegistry casting)
        //Really only used by the BlueprintRegistry
        public System.Array RawArray {
            get {
                return _idValueMap;
            }
        }
        //Generic, Type-Safe Read-Only Property (For safe gameplay queries)
        //The Bootstrapper hands these out to the handlers and tickers
        public IReadOnlyList<TValue> Values {
            get {
                return _idValueMap;
            }
        }
        public Dictionary<string, int> KeyIdMap {
            get {
                return _keyIdMap;
            }
        }
        //this lets us use DualKeyMap like an array, so we can do dualKeyMap[id] to get the value
        //we route our indexer through our getters and setter for future-proofing, in case we want to add validation or logging later
        public TValue this[int id] {
            get { return GetValue(id); }
            set { SetValue(id, value); }
        }
        public TValue this[string key] {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }
        //this decorator line tells the compiler to inline this method, which can improve performance by eliminating the overhead of a method call
        //[MethodImpl(MethodImplOptions.NoInlining)]
        //although we may want to use the NoInlining for heavier stuff, but theres none for now
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetValue(int id) { return _idValueMap[id]; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(int id, TValue value) { _idValueMap[id] = value; }
        //lets use get the length, just like an array
        public int Length {
            get { return _idValueMap.Length; }
        }
        //extra overloaded getters and setters for string keys
        public TValue GetValue(string key) {
            if (key == null) {
                throw new KeyNotFoundException("Key 'null' not found in DualKeyMap.");
            }
            if (_keyIdMap.TryGetValue(key, out int id)) {
                return _idValueMap[id];
            } else {
                throw new KeyNotFoundException($"Key '{key}' not found in DualKeyMap.");
            }
        }
        public void SetValue(string key, TValue value) {
            if (key == null) {
                throw new KeyNotFoundException("Key 'null' not found in DualKeyMap.");
            }
            if (_keyIdMap.TryGetValue(key, out int id)) {
                _idValueMap[id] = value;
            } else {
                throw new KeyNotFoundException($"Key '{key}' not found in DualKeyMap.");
            }
        }
        public bool TryGetValue(int id, out TValue value) {
            if (id >= 0 && id < Length) {
                value = _idValueMap[id];
                return true;
            } else {
                value = new TValue();
                return false;
            }
        }
        public bool TryGetValue(string key, out TValue value) {
            // 1. Guard against null keys to prevent Dictionary ArgumentNullException
            if (key == null) {
                value = new TValue();
                return false;
            }
            // 2. Perform a single dictionary lookup
            int id;
            if (_keyIdMap.TryGetValue(key, out id)) {
                // 3. Optimization: Read from our array directly using the retrieved 'id'
                // This avoids calling GetValue(key) and repeating the dictionary query
                value = _idValueMap[id];
                return true;
            } else {
                value = new TValue();
                return false;
            }
        }
        public bool ContainsKey(string key) {
            return _keyIdMap.ContainsKey(key);
        }
        public bool ContainsId(int id) {
            if (id >= 0 && id < Length && _idKeyMap[id] != null) {
                return true;
            } else {
                return false;
            }
            //return id >= 0 && id < Length; copilot really wanted to use the short version
        }
        //so hydration is complex thing, and means different things depending on the database etc
        //in the case of our DualKeyMap, it is actually closer to Populating.
        //dry stable data = key-value map, wet fluid data = id-value map
        //this step takes the key-value map, and create the id-value map we use during actual simulation
        //post-boot, we should ideally never have to deal with strings anymore
        public void Hydrate(Dictionary<string, TValue> keyValueMap, bool expectingMissing) {
            if (_isHydrated) { throw new InvalidOperationException("Attempted to hydrate an already hydrated DualKeyMap"); }
            // If the loaded save data is completely null (e.g., a brand new player or a missing file),
            // we process all indices as missing.
            if (keyValueMap == null) {
                for (int i = 0; i < _idValueMap.Length; i++) {
                    if (expectingMissing) {
                        _idValueMap[i] = new TValue();
                    } else {
                        string key = _idKeyMap[i];
                        throw new KeyNotFoundException("Critical Boot Failure: Missing expected key '" + key + "' in the save file.");
                    }
                }
                //we return because if we were expecting missing data, then an empty data also counts being missing lol
                return;
            }
            //we can safely use _values.length, that we derived from the initial constructor
            for (int i = 0; i < Length; i++) {
                string key = _idKeyMap[i];
                TValue value;

                // CRITICAL FIX: Explicitly call TryGetValue on the incoming keyValueMap dictionary,
                // rather than 'this' instance's unpopulated internal array.
                if (!keyValueMap.TryGetValue(key, out value) || value == null) {
                    if (expectingMissing) {
                        value = new TValue();
                    } else {
                        throw new InvalidDataException("Crtical Boot Failure: Value assciated with \"" + key + "\" is null or missing. Possible data corruption.");
                    }
                }
                _idValueMap[i] = value;
            }
            _isHydrated = true;
        }
        //for dehydration, it involves turning our currently id-value map back into a key-value map
        //we technically already had a key-value map to begin with, but we didnt keep that in memory
        //also, most likely the value were changed, so it wouldnt be useful either ways
        //btw, we should never need to dehydrate static data aka defs
        public Dictionary<string,TValue> Dehydrate() {
            Dictionary<string,TValue> keyValueMap = new Dictionary<string,TValue>();
            for (int i = 0; i < _idValueMap.Length; i++) {
                //our hand idKeyMap that we kept since construction, so we dont have to querry value.key
                //we could have totally done that, but it would be terrible for memory reasons
                string key = _idKeyMap[i];
                //We should definitely have no null values in our id value map, since if it was null before, its
                //definitely filled by a basic constructor by now
                if (key == null) {
                    throw new InvalidDataException("Critical Save Failure: Data at index "+i+" has a null key");
                }
                if (_idValueMap[i] == null) {
                    throw new NullReferenceException("Critical Save Failure: Data associated with " + key + " is null");
                }
                
                keyValueMap[key] = _idValueMap[i];
            }
            return keyValueMap;
        }
        //allows the bootstrapper to derive a definition's Id from its key
        public int GetId(string key) {
            if (key == null || !ContainsKey(key)) {
                throw new KeyNotFoundException(key +" could not be found");
            }
            return _keyIdMap[key];
        }
        public string GetKey(int id) {
            if (!ContainsId(id)) {
                throw new KeyNotFoundException(id + " could not be found");
            }
            return _idKeyMap[id];
        }
        //lets us do a deep copy
        public void CopyTo(DualKeyMap<TValue> target) {
            if (target == null) { throw new ArgumentNullException("Attempted to copy to null target. Has target been initialised?"); }
            if (target.Length != this.Length) { throw new InvalidOperationException("Attempted to copy to target with mismatched size."); }
            //fast path
            if (!object.ReferenceEquals(this._idKeyMap, target._idKeyMap)) {
                // Slow Path: Fallback to element-by-element validation if they have separate key arrays
                for (int i = 0; i < _idKeyMap.Length; i++) {
                    if (this._idKeyMap[i] != target._idKeyMap[i]) {
                        throw new InvalidOperationException(
                            "Cannot copy DualKeyMap: Key configuration mismatch at index " + i +
                            ". Source key: '" + this._idKeyMap[i] + "', Target key: '" + target._idKeyMap[i] + "'."
                        );
                    }
                }
            }
            for (int i = 0; i < _idKeyMap.Length; i++) {
                target._idValueMap[i] = _idValueMap[i];
            }
        }
        // Reset the DualKeyMap to its default state by reinitializing all values to their default.
        public void Reset() {
            for (int i = 0; i < _idValueMap.Length; i++) {
                _idValueMap[i] = new TValue();
            }
            _isHydrated = false;
        }
    }
}
