using System.Runtime.CompilerServices;

namespace ProjWizInc.Core.ADT {
    /*
     * DualKeyMap is a data structure that allows you to store values with two keys.
     * It is implemented as a an array of values, as well as 2 additonal maps that translate between the string keys and the integer keys.
     *  
     * This allows for efficient lookups and insertions, as well as easy iteration over all values associated with a given first key.
     * where TValue : new() means that we must have a noarg constructor, for our future proofing default value creation
     */
    public class DualKeyMap<TValue> : IDualKeyMap where TValue : new(){
        //holds our payload in an array for fast access
        private readonly TValue[] _values;
        //translates string to keys, for those who dont know the id yet
        private readonly Dictionary<string, int> _keyIdMap;
        //translates keys to string, for hydration and dehydration
        private readonly string[] _idKeyMap;
        //our constructor 
        public DualKeyMap(Dictionary<string, int> keyIdMap) {
            if (keyIdMap == null) {
                throw new ArgumentNullException("keyIdMap", "Critical Boot Failure: The keyIdMap provided to DualKeyMap is null.");
            }

            // Phase 1: Scan the map to find the maximum ID value and verify there are no negative IDs.
            // This allows us to dynamically derive the safe array size.
            int maxId = -1;
            foreach (KeyValuePair<string, int> kvp in keyIdMap) {
                if (kvp.Value < 0) {
                    throw new ArgumentOutOfRangeException("keyIdMap", "Critical Boot Failure: Key ID " + kvp.Value.ToString() + " for key '" + kvp.Key + "' is negative and invalid.");
                }

                if (kvp.Value > maxId) {
                    maxId = kvp.Value;
                }
            }

            // Phase 2: Calculate the exact safe size.
            // If the map was empty (maxId remains -1), the size is 0. Otherwise, the size is maxId + 1.
            int size = 0;
            if (maxId != -1) {
                size = maxId + 1;
            }

            // Allocate our contiguous, flat runtime arrays to the exact derived safe capacity.
            _values = new TValue[size];
            _keyIdMap = keyIdMap;
            _idKeyMap = new string[size];

            // Phase 3: Populate the reverse mapping array.
            // Because the size was derived directly from the maximum ID in the map,
            // this loop is guaranteed never to throw an IndexOutOfRangeException.
            foreach (KeyValuePair<string, int> kvp in keyIdMap) {
                _idKeyMap[kvp.Value] = kvp.Key;
            }
        }
        public System.Array RawArray {
            get { return _values; }
        }
        public Dictionary<string, int> KeyIdMap {
            get {
                return _keyIdMap;
            }
        }
        //this lets us use DualKeyMap like an array, so we can do dualKeyMap[id] to get the value
        //we route our indexer through our getters and setter for future-proofing, in case we want to add validation or logging later
        public TValue this[int id] {
            get {
                //return _values[id];
                return GetValue(id);
            }
            set {
                //_values[id] = value;
                SetValue(id, value);
            }
        }
        //this decorator line tells the compiler to inline this method, which can improve performance by eliminating the overhead of a method call
        //[MethodImpl(MethodImplOptions.NoInlining)]
        //although we may want to use the NoInlining for heavier stuff, but theres none for now
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetValue(int id) { return _values[id]; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(int id, TValue value) { _values[id] = value; }
        //lets use get the length, just like an array
        public int Length {
            get { return _values.Length; }
        }
        //extra overloaded getters and setters for string keys
        public TValue GetValue(string key) {
            if (key == null) {
                throw new KeyNotFoundException("Key 'null' not found in DualKeyMap.");
            }
            if (_keyIdMap.TryGetValue(key, out int id)) {
                return _values[id];
            } else {
                throw new KeyNotFoundException($"Key '{key}' not found in DualKeyMap.");
            }
        }
        public void SetValue(string key, TValue value) {
            if (key == null) {
                throw new KeyNotFoundException("Key 'null' not found in DualKeyMap.");
            }
            if (_keyIdMap.TryGetValue(key, out int id)) {
                _values[id] = value;
            } else {
                throw new KeyNotFoundException($"Key '{key}' not found in DualKeyMap.");
            }
        }
        public bool TryGetValue(int id, out TValue value) {
            if (id >= 0 && id < Length) {
                value = _values[id];
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
                value = _values[id];
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
        //during boot or load, we hydrate to turn key-values into id-values
        //so our hydrate got extra complex in order to handle loading older save files that may not have all the keys we expect in the current version of the game
        public void Hydrate(Dictionary<string, TValue> data, bool expectingMissing) {
            // If the loaded save data is completely null (e.g., a brand new player or a missing file),
            // we process all indices as missing.
            if (data == null) {
                for (int i = 0; i < _values.Length; i++) {
                    if (expectingMissing) {
                        _values[i] = new TValue();
                    } else {
                        string key = _idKeyMap[i];
                        throw new KeyNotFoundException("Critical Boot Failure: Missing expected key '" + key + "' in the save file.");
                    }
                }
                return;
            }

            // Iterate through our current runtime definitions (the source of truth)
            for (int i = 0; i < _values.Length; i++) {
                string key = _idKeyMap[i];

                // Fail-Fast Check: If our mapping contains a null key, our configuration is broken.
                if (key == null) {
                    throw new InvalidOperationException("Critical Boot Failure: Array slot at index " + i.ToString() + " does not have a registered stable string key.");
                }

                TValue loadedValue;
                if (data.TryGetValue(key, out loadedValue)) {
                    // Key is present: Hydrate the value into our active runtime array
                    _values[i] = loadedValue;
                } else {
                    // Key is missing: Evaluate the system state flag
                    if (expectingMissing) {
                        // Healthy Migration Path:
                        // The key did not exist in the older save version, so we initialize it to its default state.
                        _values[i] = new TValue();
                    } else {
                        // Unhealthy Corruption Path:
                        // The key should be present in this current-version save, but is missing.
                        throw new KeyNotFoundException("Critical Boot Failure: Missing expected key '" + key + "' in the save file.");
                    }
                }
            }
        }
        //when we save, we dehydrate to turn id-values into key-values
        public Dictionary<string,TValue> Dehydrate() {
            Dictionary<string,TValue> saveData = new Dictionary<string,TValue>();
            for (int i = 0; i < _values.Length; i++) {
                string key = _idKeyMap[i];
                // Fail-Fast Check: If our mapping contains a null key, our configuration is broken.
                if (key == null) {
                    throw new InvalidOperationException("Critical Save Failure: Array slot at index " + i.ToString() + " does not have a registered stable string key.");
                }
                saveData[key] = _values[i];
            }
            return saveData;
        }
        //allows the bootstrapper to derive a definition's Id from its key
        public int GetId(string key) {
            if (key == null || !ContainsKey(key)) {
                throw new KeyNotFoundException(key +" could not be found");
            }
            return _keyIdMap[key];
        }
        // Reset the DualKeyMap to its default state by reinitializing all values to their default.
        public void Reset() {
            for (int i = 0; i < _values.Length; i++) {
                _values[i] = new TValue();
            }
        }
    }
}
