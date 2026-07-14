using ProjWizInc.Core.ADT;
using ProjWizInc.Core.Data;
using ProjWizInc.Core.Definitions;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {

    
    internal class EconomyManager{
        public class EconomyState {

            //changed the dictionary to using ints instead of strings for faster searching
            public BigNum[]? Resources { get; internal set; }
        }

        private readonly EventBroker _events;
        private readonly EconomyState _state = new();
        private readonly DefinitionManager _defs;
        private readonly object _lock = new ();
        public EconomyManager(EventBroker events, DefinitionManager defs) {
            _events = events; 
            _defs= defs;
            _events.Subscribe<UpdateLogicEvent>(Update);
        }
        public EconomyState State => _state;
        public void Init() {
            _state.Resources = new BigNum[_defs.GetDefCount<ResourceDefinition>()];
            _events.Subscribe<ResourceGainedEvent>(AddResource);
        }
        //we have to use a small paremeter here, even if we dont need anything, because of signalling reasons
        //but if we change UpdateFrameEvent with parameters later, we can check it with e
        public void Update(UpdateLogicEvent e) {
            
        }
        private void AddResource(ResourceGainedEvent e) {
            lock (_lock) {
                if (e.Id >= _state.Resources.Length) {
                    throw new IndexOutOfRangeException("Attempted to access resource at index "+ e.Id+". There are" +
                        "only "+_state.Resources.Length + " resource types");
                }
                _state.Resources[e.Id] += e.Amount;
            }            
        }
        public BigNum GetResource(int resourceID) {
            lock (_lock) {
                if (resourceID >= _state.Resources.Length) {
                    throw new IndexOutOfRangeException("Attempted to access resource at index " + resourceID + ". There are" +
                        "only " + _state.Resources.Length + " resource types");
                }
                return _state.Resources[resourceID];
            }
           

        }

    }
}
