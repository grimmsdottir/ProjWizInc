using ProjWizInc.Core.Data;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {

    public class ResourceState {
        //changed the dictionary to using ints instead of strings for faster searching
        //do we use a state even if it is just one variable(i know its a list, but it still counts as one)
        public Dictionary<int, long> Resources { get; } = new() {
        };
    }
    public class ResourceManager{
        
        private readonly EventBroker _events;
        private readonly ResourceState _state = new();
        public ResourceManager(EventBroker events) {
            _events = events; 
            _events.Subscribe<UpdateLogicEvent>(Update);
        }
        public ResourceState State => _state;
        public void Init() {
            //read json later, for now we have hard coded thing
            Dictionary<string, long> ResourceDefs = new() {
                {"stone",0 },
                {"wood",0 },
                {"gold",0 }
            };
            int i = 0;
            foreach (var r in ResourceDefs) {
                _state.Resources.Add(i, r.Value);
                i++;
            }
        }
        //we have to use a small paremeter here, even if we dont need anything, because of signalling reasons
        //but if we change UpdateFrameEvent with parameters later, we can check it with e
        public void Update(UpdateLogicEvent e) {
            
        }
        public void AddResource(int resourceID, long amount) {
            if (!_state.Resources.ContainsKey(resourceID)) {
                _state.Resources[resourceID] = 0;
            }
            _state.Resources[resourceID] += amount;
        }
        public long GetResource(int resourceID) {
            if (!_state.Resources.ContainsKey(resourceID)) {
                _state.Resources[resourceID] = 0;
            }
            return _state.Resources[resourceID];

        }

    }
}
