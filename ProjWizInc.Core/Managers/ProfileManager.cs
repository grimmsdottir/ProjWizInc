using ProjWizInc.Core.Data;
using ProjWizInc.Core.Events;
using ProjWizInc.Core.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjWizInc.Core.Managers {
    
    public class ProfileManager{
        private readonly EventBroker _events;
        public ProfileManager(EventBroker events) {
            _events = events; 
            _events.Subscribe<UpdateLogicEvent>(Update);
        }
        public void Init() {
        }
        //we have to use a small paremeter here, even if we dont need anything, because of signalling reasons
        //but if we change UpdateFrameEvent with parameters later, we can check it with e
        public void Update(UpdateLogicEvent e) {
            
        }

    }
}
