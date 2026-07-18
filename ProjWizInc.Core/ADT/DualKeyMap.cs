using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.ADT {
    /*
     * DualKeyMap is a data structure that allows you to store values with two keys.
     * It is implemented as a dictionary of dictionaries, where the first key is used to access the outer dictionary,
     * and the second key is used to access the inner dictionary.
     * 
     * This allows for efficient lookups and insertions, as well as easy iteration over all values associated with a given first key.
     * where TValue : new() means that we must have a noarg constructor, for our future proofing default value creation
     */
    public class DualKeyMap<TValue> where TValue : new(){
        //holds our payload in an array for fast access
        private readonly TValue[] _values;
        //translates string to keys, for those who dont know the id yet
        private readonly Dictionary<string, int> _keyIdMap;
        //translates keys to string, for hydration and dehydration
        private readonly string[] _idKeyMap;

        public DualKeyMap(int size, Dictionary<string,int> keyIdMap) {
            _values = new TValue[size];
            _keyIdMap = keyIdMap;
            _idKeyMap = new string[size];
            //idKey is just the reverse of keyId, so we can use it to get the string key from the id
            //apparently this is just the long way some ToDictionary
            foreach (KeyValuePair<string,int> kvp in keyIdMap ) {
                if (kvp.Value < 0 || kvp.Value >= size) {
                    throw new ArgumentOutOfRangeException($"Key ID {kvp.Value} for key '{kvp.Key}' is out of range for the specified size {size}.");
                }
                _idKeyMap[kvp.Value] = kvp.Key;
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
    }
}
