using ProjWizInc.Engine.Data.ADT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Engine.Tests.TestGenerators {
    public static class TestDataFactory {
        // A clean, explicit factory method to build a minimal, valid DualKeyMap
        public static DualKeyMap<T> CreateMinimalMap<T>(int size) where T : class, new() {
            string[] keys = new string[size];
            for (int i = 0; i < size; i++) {
                keys[i] = "item_" + i;
            }

            DualKeyMap<T> map = new DualKeyMap<T>(keys);

            // Populate with default populated items so the map is not empty
            for (int i = 0; i < size; i++) {
                map[i] = new T();
            }

            return map;
        }
    }
}
