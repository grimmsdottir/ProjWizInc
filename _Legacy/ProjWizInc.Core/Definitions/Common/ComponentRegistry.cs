using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Definitions.Common {
    public static class ComponentRegistry {
        private static int _counter = 0;
        public static int NextId() {
            int id = _counter;
            _counter = _counter + 1;
            return id;
            //could be simplified, but its kinda confusing
            //return _counter++;
        }
        public static int Capacity() {
            return _counter;
        }
    }
    public static class ComponentType<T> where T : IDefinitionComponentInterface {
        public static readonly int Id = ComponentRegistry.NextId();
    }
}
