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
        public long Gold { get; set; }
        public long Wood { get; set; }
        public long Stone { get; set; }
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
        }
        //we have to use a small paremeter here, even if we dont need anything, because of signalling reasons
        //but if we change UpdateFrameEvent with parameters later, we can check it with e
        public void Update(UpdateLogicEvent e) {
            
        }

    }
}
